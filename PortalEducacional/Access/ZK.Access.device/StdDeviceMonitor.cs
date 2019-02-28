/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Timers;
using ZK.Access.door;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.device
{
	public class StdDeviceMonitor : IDeviceMonitor, IDisposable
	{
		public delegate void OnOfflineLogEventHandler(DeviceServerBll sender, OnOfflineLogEventArgs e);

		public delegate void OnOfflineLogCountEventHandler();

		public delegate void OnOfflineLogEndEventHandler();

		private int _SerialPort;

		private int GetLogInterval = 10000;

		private object objMonitorLock = new object();

		private object objControlDeviceLock = new object();

		private System.Timers.Timer tmrMonitor;

		private Dictionary<string, DeviceServerBll> dicDevFlag_DeviceServer = new Dictionary<string, DeviceServerBll>();

		private static int err_connection_break = -1001;

		private bool ShouldStopMonitor;

		private bool _IsNeedPullEvent;

		private static object _lock = new object();

		private static bool _locked = false;

		private bool IsDisposing = false;

		public bool IsMonitoring => this.tmrMonitor != null && this.tmrMonitor.Enabled;

		public int SerialPort => this._SerialPort;

		public int ServerCount
		{
			get
			{
				if (this.dicDevFlag_DeviceServer == null)
				{
					return 0;
				}
				return this.dicDevFlag_DeviceServer.Count;
			}
		}

		public int MonitorInterval
		{
			get
			{
				return this.GetLogInterval;
			}
			set
			{
				if (value != this.GetLogInterval)
				{
					bool enabled = this.tmrMonitor.Enabled;
					this.GetLogInterval = value;
					this.tmrMonitor.Enabled = false;
					this.tmrMonitor.Interval = (double)this.GetLogInterval;
					this.tmrMonitor.Enabled = enabled;
				}
			}
		}

		public int SYNCTimeInterval
		{
			get;
			set;
		}

		public event OnOfflineLogEventHandler OnOfflineLog;

		public event OnOfflineLogCountEventHandler OnOfflineLogCount;

		public event OnOfflineLogEndEventHandler OnOfflineLogEnd;

		public StdDeviceMonitor(int comPort = 0)
		{
			this._SerialPort = comPort;
			this.SYNCTimeInterval = 30;
		}

		public void ProcessOfflineEvents()
		{
			foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
			{
				DeviceServerBll value = item.Value;
				if (value != null)
				{
					value.STD_GetAllTransaction(out List<ObjTransAction> list);
					if (list != null && list.Count > 0)
					{
						this._getOfflineEvents(value, list);
					}
				}
			}
		}

		public void ProcessOfflineEvents(DeviceServerBll devServer)
		{
			if (devServer != null)
			{
				devServer.STD_GetAllTransaction(out List<ObjTransAction> list);
				if (list != null && list.Count > 0)
				{
					this._getOfflineEvents(devServer, list);
				}
			}
		}

		private void _getOfflineEvents(DeviceServerBll devServer, List<ObjTransAction> lst)
		{
			lock (StdDeviceMonitor._lock)
			{
				if (!StdDeviceMonitor._locked)
				{
					StdDeviceMonitor._locked = true;
					goto end_IL_0009;
				}
				return;
				end_IL_0009:;
			}
			SysLogServer.WriteLog("StdDeviceMonitor: _getOfflineEvents", true);
			try
			{
				if (this.OnOfflineLog != null)
				{
					this.OnOfflineLog(devServer, new OnOfflineLogEventArgs(devServer.DevInfo.DeviceName, lst.Count));
				}
				AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
				DateTime lastTimeByDeviceId = accMonitorLogBll.getLastTimeByDeviceId(devServer.DevInfo.ID);
				for (int i = 0; i < lst.Count; i++)
				{
					ObjTransAction objTransAction = lst[i];
					ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
					objRTLogInfo.DevID = devServer.DevInfo.ID;
					objRTLogInfo.CardNo = objTransAction.Cardno;
					objRTLogInfo.DoorID = objTransAction.DoorID;
					objRTLogInfo.EType = objTransAction.EventType;
					objRTLogInfo.InOutStatus = objTransAction.InOutState;
					objRTLogInfo.VerifyType = objTransAction.Verified.ToString();
					objRTLogInfo.Pin = objTransAction.Pin;
					objRTLogInfo.Date = DateTime.ParseExact(objTransAction.Time_second, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
					if (objRTLogInfo.Date.CompareTo(lastTimeByDeviceId) > 0)
					{
						RTEventProcessor.devSever_RTEvent(devServer, objRTLogInfo);
					}
					if (this.OnOfflineLogCount != null)
					{
						this.OnOfflineLogCount();
					}
				}
				if (this.OnOfflineLogEnd != null)
				{
					this.OnOfflineLogEnd();
				}
			}
			finally
			{
				StdDeviceMonitor._locked = false;
			}
		}

		public void StartMonitor()
		{
			if (this.objMonitorLock != null)
			{
				try
				{
					lock (this.objMonitorLock)
					{
						try
						{
							if (this.SerialPort > 0 && this.SerialPort <= 255)
							{
								if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.Count > 0)
								{
									this._IsNeedPullEvent = true;
									this.MonitorInterval = 1000;
								}
								else
								{
									this._IsNeedPullEvent = false;
								}
							}
							else
							{
								this._IsNeedPullEvent = false;
							}
						}
						catch (Exception ex)
						{
							LogServer.Log("dicDevFlag_DeviceServer is null? " + (this.dicDevFlag_DeviceServer == null).ToString());
							LogServer.Log("StdDeviceMonitor.StartMonitor CheckingSDKVersion Exception: " + ex.Message, true);
						}
						if (this.tmrMonitor == null)
						{
							this.tmrMonitor = new System.Timers.Timer((double)this.MonitorInterval);
							this.tmrMonitor.AutoReset = true;
							this.tmrMonitor.Elapsed += this.tmrMonitor_Elapsed;
						}
						if (!this.tmrMonitor.Enabled)
						{
							this.tmrMonitor.Enabled = true;
						}
						this.ShouldStopMonitor = false;
					}
					ThreadPool.QueueUserWorkItem(delegate
					{
						if (this._IsNeedPullEvent)
						{
							this.ClearOldRTEvent();
						}
						this.tmrMonitor_Elapsed(null, null);
					});
				}
				catch (Exception ex2)
				{
					LogServer.Log("StdDeviceMonitor.StartMonitor Exception: " + ex2.Message, true);
				}
			}
		}

		public void StopMonitor()
		{
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					if (this.tmrMonitor != null)
					{
						this.tmrMonitor.Enabled = false;
					}
					this.ShouldStopMonitor = true;
					if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.Count > 0)
					{
						foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
						{
							item.Value.STD_StopMonitor();
						}
					}
				}
			}
		}

		private void MonitoringWithoutLock()
		{
			int num = 0;
			if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.Count > 0 && this.dicDevFlag_DeviceServer != null)
			{
				foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
				{
					if (this.ShouldStopMonitor)
					{
						break;
					}
					if (item.Value.IsNeedListen)
					{
						lock (this.objControlDeviceLock)
						{
							if (item.Value.IsConnected)
							{
								if (this._IsNeedPullEvent)
								{
									item.Value.GetRTLogs(ref num);
								}
								else
								{
									num = item.Value.STD_GetDoorState(out int _);
								}
								if (num < 0)
								{
									item.Value.STD_StopMonitor();
									item.Value.Disconnect();
								}
								else
								{
									item.Value.STD_StartMonitor();
									ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
									objRTLogInfo.DevID = item.Value.DevInfo.ID;
									objRTLogInfo.DoorID = "1";
									objRTLogInfo.InOutStatus = "2";
									objRTLogInfo.IP = item.Value.DevInfo.IP;
									objRTLogInfo.Pin = "";
									objRTLogInfo.StatusInfo = "";
									objRTLogInfo.WarningStatus = "";
									objRTLogInfo.CardNo = "";
									objRTLogInfo.DoorStatus = "0";
									objRTLogInfo.StatusInfo = "";
									objRTLogInfo.EType = EventType.Type255;
									objRTLogInfo.VerifyType = "";
									objRTLogInfo.Date = DateTime.Now;
									RTEventProcessor.devSever_RTEvent(item.Value, objRTLogInfo);
									this.SYNCTime(item.Value);
								}
							}
							else
							{
								num = item.Value.Connect(3000);
								if (num >= 0)
								{
									this.SYNCTime(item.Value);
									item.Value.STD_StartMonitor();
									ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
									objRTLogInfo.DevID = item.Value.DevInfo.ID;
									objRTLogInfo.DoorID = "1";
									objRTLogInfo.InOutStatus = "2";
									objRTLogInfo.IP = item.Value.DevInfo.IP;
									objRTLogInfo.Pin = "";
									objRTLogInfo.StatusInfo = "";
									objRTLogInfo.WarningStatus = "";
									objRTLogInfo.CardNo = "";
									objRTLogInfo.DoorStatus = "0";
									objRTLogInfo.StatusInfo = "";
									objRTLogInfo.EType = EventType.Type255;
									objRTLogInfo.VerifyType = "";
									objRTLogInfo.Date = DateTime.Now;
									RTEventProcessor.devSever_RTEvent(item.Value, objRTLogInfo);
								}
							}
						}
					}
				}
			}
		}

		private void SYNCTime(DeviceServerBll deviceServerBll)
		{
			TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan lastSetDeviceTime = deviceServerBll.LastSetDeviceTime;
			if (!((timeSpan - deviceServerBll.LastSetDeviceTime).TotalMinutes < (double)this.SYNCTimeInterval))
			{
				deviceServerBll.STD_SetDeviceTime(null);
				deviceServerBll.LastSetDeviceTime = timeSpan;
			}
		}

		private void tmrMonitor_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (this.objMonitorLock != null && Monitor.TryEnter(this.objMonitorLock))
			{
				try
				{
					this.MonitoringWithoutLock();
				}
				catch (Exception ex)
				{
					LogServer.Log("StdDeviceMonitor.tmrMonitor_Elapsed Exception: " + ex.Message, true);
				}
				finally
				{
					Monitor.Exit(this.objMonitorLock);
				}
			}
		}

		private void devServer_RTEvent(ObjDevice sender, ObjRTLogInfo RTLog)
		{
			if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.ContainsKey(sender.ToString()))
			{
				DeviceServerBll deviceServerBll = this.dicDevFlag_DeviceServer[sender.ToString()];
				if (deviceServerBll.IsNeedListen)
				{
					RTEventProcessor.devSever_RTEvent(deviceServerBll, RTLog);
				}
			}
		}

		public void AddDeviceServer(DeviceServerBll devServer)
		{
			if (devServer.Monitor != null)
			{
				throw new Exception("This server already belongs to another monitor");
			}
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					if (this.dicDevFlag_DeviceServer != null && !this.dicDevFlag_DeviceServer.ContainsKey(devServer.DevInfo.ToString()))
					{
						this.dicDevFlag_DeviceServer.Add(devServer.DevInfo.ToString(), devServer);
						devServer.Monitor = this;
						devServer.MissCount = 0;
						devServer.RTEvent += this.devServer_RTEvent;
					}
				}
			}
		}

		public void RemoveDeviceServer(DeviceServerBll devServer)
		{
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.ContainsKey(devServer.DevInfo.ToString()))
					{
						this.dicDevFlag_DeviceServer.Remove(devServer.DevInfo.ToString());
						devServer.Monitor = null;
						devServer.RTEvent -= this.devServer_RTEvent;
					}
				}
			}
		}

		public void ClearDeviceServer()
		{
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					if (this.dicDevFlag_DeviceServer != null)
					{
						foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
						{
							item.Value.Monitor = null;
							item.Value.RTEvent -= this.devServer_RTEvent;
						}
						this.dicDevFlag_DeviceServer.Clear();
					}
				}
			}
		}

		public void ClearOldRTEvent()
		{
			int num = 0;
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.Count > 0)
					{
						foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
						{
							if (!item.Value.IsConnected)
							{
								ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
								objRTLogInfo.DevID = item.Value.DevInfo.ID;
								objRTLogInfo.DoorID = "1";
								objRTLogInfo.InOutStatus = "2";
								objRTLogInfo.IP = item.Value.DevInfo.IP;
								objRTLogInfo.Pin = "";
								objRTLogInfo.StatusInfo = "";
								objRTLogInfo.WarningStatus = "";
								num = item.Value.Connect(3000);
								if (num >= 0)
								{
									objRTLogInfo.CardNo = "";
									objRTLogInfo.DoorStatus = "0";
									objRTLogInfo.StatusInfo = "";
									objRTLogInfo.EType = EventType.Type255;
									objRTLogInfo.VerifyType = "";
									objRTLogInfo.Date = DateTime.Now;
									RTEventProcessor.devSever_RTEvent(item.Value, objRTLogInfo);
									goto IL_01e2;
								}
								objRTLogInfo.StatusInfo = PullSDkErrorInfos.GetInfo(num);
								objRTLogInfo.EType = EventType.Type300;
								objRTLogInfo.VerifyType = "200";
								objRTLogInfo.Date = DateTime.Now;
								RTEventProcessor.devSever_RTEvent(item.Value, objRTLogInfo);
								break;
							}
							goto IL_01e2;
							IL_01e2:
							List<ObjRTLogInfo> rTLogs = item.Value.GetRTLogs(ref num);
							if (rTLogs != null && rTLogs.Count > 0)
							{
								ThreadPool.QueueUserWorkItem(delegate(object obj)
								{
									List<ObjRTLogInfo> list = obj as List<ObjRTLogInfo>;
									if (list != null && list.Count > 0)
									{
										for (int i = 0; i < list.Count; i++)
										{
											RTEventProcessor.CheckAlarm(item.Value, list[i]);
											RTEventProcessor.SaveRecord(item.Value, list[i]);
										}
									}
								}, rTLogs);
							}
						}
					}
				}
			}
		}

		~StdDeviceMonitor()
		{
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					this.Dispose(false);
				}
			}
			else
			{
				this.Dispose(false);
			}
		}

		public void Dispose()
		{
			if (this.objMonitorLock != null)
			{
				lock (this.objMonitorLock)
				{
					this.Dispose(true);
				}
			}
			else
			{
				this.Dispose(true);
			}
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool freeManagedObjectsAlso)
		{
			if (!this.IsDisposing)
			{
				this.IsDisposing = true;
				if (freeManagedObjectsAlso)
				{
					if (this.objMonitorLock == null)
					{
						if (this.tmrMonitor != null)
						{
							this.tmrMonitor.Enabled = false;
							this.tmrMonitor = null;
						}
						if (this.dicDevFlag_DeviceServer != null)
						{
							this.ClearDeviceServer();
							this.dicDevFlag_DeviceServer = null;
						}
					}
					else
					{
						lock (this.objMonitorLock)
						{
							if (this.tmrMonitor != null)
							{
								this.tmrMonitor.Enabled = false;
								this.tmrMonitor = null;
							}
							if (this.dicDevFlag_DeviceServer != null)
							{
								this.ClearDeviceServer();
								this.dicDevFlag_DeviceServer = null;
							}
						}
						this.objMonitorLock = null;
					}
				}
			}
		}

		public bool Contains(DeviceServerBll devServer)
		{
			lock (this.objMonitorLock)
			{
				if (this.dicDevFlag_DeviceServer == null)
				{
					return false;
				}
				return this.dicDevFlag_DeviceServer.ContainsKey(devServer.DevInfo.ToString());
			}
		}

		public void DisConnectAll()
		{
			if (this.objMonitorLock != null)
			{
				try
				{
					lock (this.objMonitorLock)
					{
						if (this.dicDevFlag_DeviceServer != null)
						{
							foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
							{
								item.Value.Disconnect();
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogServer.Log("StdDeviceMonitor.DisConnectAll Exception: " + ex.Message, true);
				}
			}
		}

		public void OpenDoors(List<DevControl> lstDoorDev, OpenDoorSet frmOpenDoor, WaitForm frmWait, out int SucceedCount, out int FailedCount)
		{
			lock (this.objControlDeviceLock)
			{
				SucceedCount = 0;
				FailedCount = 0;
				for (int i = 0; i < lstDoorDev.Count; i++)
				{
					if (lstDoorDev[i].DevInfo != null && this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.ContainsKey(lstDoorDev[i].DevInfo.ToString()))
					{
						int num = StdDeviceMonitor.err_connection_break;
						if (!lstDoorDev[i].DevServer.IsConnected)
						{
							FailedCount++;
							frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							switch (frmOpenDoor.Second)
							{
							case -100:
								num = lstDoorDev[i].NormalOpen(true);
								break;
							case -255:
								num = lstDoorDev[i].OpenDoor();
								break;
							default:
								if (frmOpenDoor.Second > 0 && frmOpenDoor.Second < 255)
								{
									num = lstDoorDev[i].OpenDoor(frmOpenDoor.Second);
								}
								break;
							}
							if (num < 0)
							{
								FailedCount++;
								frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							else
							{
								SucceedCount++;
								frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
							}
						}
					}
				}
			}
		}

		public void CloseDoor(List<DevControl> lstDoorDev, CloseDoorSet frmCloseDoor, WaitForm frmWait, out int SucceedCount, out int FailedCount)
		{
			lock (this.objControlDeviceLock)
			{
				SucceedCount = 0;
				FailedCount = 0;
				for (int i = 0; i < lstDoorDev.Count; i++)
				{
					if (lstDoorDev[i].DevInfo != null && this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.ContainsKey(lstDoorDev[i].DevInfo.ToString()))
					{
						int num = StdDeviceMonitor.err_connection_break;
						if (!lstDoorDev[i].DevServer.IsConnected)
						{
							FailedCount++;
							frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							switch (frmCloseDoor.Second)
							{
							case 0:
								num = lstDoorDev[i].CloseDoor();
								break;
							case 2:
								num = lstDoorDev[i].NormalOpen(false);
								break;
							}
							if (num < 0)
							{
								FailedCount++;
								frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							else
							{
								SucceedCount++;
								frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
							}
						}
					}
				}
			}
		}

		public void CancelAlarm(List<DevControl> lstDoorDev, WaitForm frmWait, out int SucceedCount, out int FailedCount)
		{
			lock (this.objControlDeviceLock)
			{
				SucceedCount = 0;
				FailedCount = 0;
				for (int i = 0; i < lstDoorDev.Count; i++)
				{
					if (lstDoorDev[i].DevInfo != null && this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.ContainsKey(lstDoorDev[i].DevInfo.ToString()))
					{
						int id = StdDeviceMonitor.err_connection_break;
						if (!lstDoorDev[i].DevServer.IsConnected)
						{
							FailedCount++;
							frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(id));
						}
						else
						{
							id = lstDoorDev[i].CancelAlarm();
							if (id < 0)
							{
								FailedCount++;
								frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(id));
							}
							else
							{
								SucceedCount++;
								frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
							}
						}
					}
				}
			}
		}
	}
}
