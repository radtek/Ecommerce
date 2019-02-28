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
using System.Text;
using System.Threading;
using System.Timers;
using ZK.Access.door;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model.PullSDK;
using ZK.MachinesManager;
using ZK.Utils;

namespace ZK.Access.device
{
	public class PullDeviceMonitor : IDeviceMonitor, IDisposable
	{
		public delegate void OnOfflineLogEventHandler(DeviceServerBll sender, OnOfflineLogEventArgs e);

		public delegate void OnOfflineLogCountEventHandler();

		public delegate void OnOfflineLogEndEventHandler();

		private int _SerialPort;

		private int GetLogInterval = 1000;

		private object objMonitorLock = new object();

		private object objControlDeviceLock = new object();

		private System.Timers.Timer tmrMonitor;

		private Dictionary<string, DeviceServerBll> dicDevFlag_DeviceServer = new Dictionary<string, DeviceServerBll>();

		private readonly int err_connection_break = -1001;

		private bool ShouldStopMonitor;

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

		public PullDeviceMonitor(int comPort = 0)
		{
			this._SerialPort = comPort;
			this.SYNCTimeInterval = 30;
		}

		public void ProcessOfflineEvents()
		{
			foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
			{
				DeviceServerBll value = item.Value;
				if (value != null && value.IsNeedListen)
				{
					if (!value.IsConnected)
					{
						value.Connect(3000);
					}
					this._getOfflineEvents(value);
				}
			}
		}

		public void ProcessOfflineEvents(DeviceServerBll devServer)
		{
			if (devServer != null && devServer.IsNeedListen)
			{
				if (!devServer.IsConnected)
				{
					devServer.Connect(3000);
				}
				this._getOfflineEvents(devServer);
			}
		}

		private void _getOfflineEvents(DeviceServerBll devServer)
		{
			lock (PullDeviceMonitor._lock)
			{
				if (!PullDeviceMonitor._locked)
				{
					PullDeviceMonitor._locked = true;
					goto end_IL_0009;
				}
				return;
				end_IL_0009:;
			}
			SysLogServer.WriteLog("PullDeviceMonitor: _getOfflineEvents", true);
			try
			{
				int num = 20971520;
				byte[] array = new byte[num];
				string fileName = "*";
				int deviceData = devServer.GetDeviceData(ref array[0], num, "transaction", fileName, "", "");
				if (deviceData >= 0)
				{
					array = DataConvert.GetDataBuffer(array);
					string @string = Encoding.Default.GetString(array);
					if (!string.IsNullOrEmpty(@string))
					{
						string[] array2 = @string.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						if (array2.Length > 1)
						{
							if (this.OnOfflineLog != null)
							{
								this.OnOfflineLog(devServer, new OnOfflineLogEventArgs(devServer.DevInfo.DeviceName, array2.Length - 1));
							}
							AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
							DateTime lastTimeByDeviceId = accMonitorLogBll.getLastTimeByDeviceId(devServer.DevInfo.ID);
							for (int i = 1; i < array2.Length; i++)
							{
								string[] array3 = array2[i].Split(",".ToCharArray());
								ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
								objRTLogInfo.DevID = devServer.DevInfo.ID;
								objRTLogInfo.DoorID = array3[3];
								objRTLogInfo.InOutStatus = array3[5];
								objRTLogInfo.CardNo = array3[0];
								objRTLogInfo.IP = devServer.DevInfo.IP;
								objRTLogInfo.Pin = array3[1];
								objRTLogInfo.StatusInfo = "";
								objRTLogInfo.WarningStatus = "";
								objRTLogInfo.StatusInfo = PullSDkErrorInfos.GetInfo(0);
								objRTLogInfo.EType = (EventType)int.Parse(array3[4]);
								objRTLogInfo.VerifyType = array3[2];
								objRTLogInfo.Date = this.DecodeZKTime(uint.Parse(array3[6]));
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
					}
				}
			}
			finally
			{
				PullDeviceMonitor._locked = false;
			}
		}

		public void StartMonitor()
		{
			try
			{
				if (this.objMonitorLock != null)
				{
					lock (this.objMonitorLock)
					{
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
						this.ClearOldRTEvent();
						this.tmrMonitor_Elapsed(null, null);
					});
				}
			}
			catch (Exception ex)
			{
				LogServer.Log("PullDeviceMonitor.StartMonitor Exception: " + ex.Message, true);
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
					if (this.dicDevFlag_DeviceServer == null || this.dicDevFlag_DeviceServer.Count <= 0)
					{
						goto end_IL_001a;
					}
					end_IL_001a:;
				}
			}
		}

		private void MonitoringWithoutLock()
		{
			int num = 0;
			if (this.dicDevFlag_DeviceServer != null && this.dicDevFlag_DeviceServer.Count > 0)
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
								DeviceServerBll value = item.Value;
								if (value != null)
								{
									List<ObjRTLogInfo> rTLogs = value.GetRTLogs(ref num);
									if (num >= 0)
									{
										value.MissCount = 0;
										value.LastConnectTime = DateTime.Now;
										if (rTLogs != null && rTLogs.Count > 0)
										{
											for (int i = 0; i < rTLogs.Count; i++)
											{
												RTEventProcessor.devSever_RTEvent(value, rTLogs[i]);
											}
										}
										this.SYNCTime(item.Value);
									}
									else
									{
										value.MissCount++;
										if (value.MissCount >= 3)
										{
											value.Disconnect();
											ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
											objRTLogInfo.DevID = value.DevInfo.ID;
											objRTLogInfo.DoorID = "0";
											objRTLogInfo.InOutStatus = "2";
											objRTLogInfo.IP = value.DevInfo.IP;
											objRTLogInfo.Pin = "";
											objRTLogInfo.StatusInfo = "";
											objRTLogInfo.WarningStatus = "";
											objRTLogInfo.StatusInfo = PullSDkErrorInfos.GetInfo(num);
											objRTLogInfo.EType = EventType.Type300;
											objRTLogInfo.VerifyType = "200";
											objRTLogInfo.Date = DateTime.Now;
											RTEventProcessor.devSever_RTEvent(value, objRTLogInfo);
										}
									}
								}
							}
							else
							{
								DeviceServerBll value = item.Value;
								if (value != null)
								{
									int num2 = 1000;
									int num3 = 0;
									DateTime dateTime = DateTime.Now;
									TimeSpan t = new TimeSpan(dateTime.Ticks);
									dateTime = value.LastConnectTime;
									TimeSpan t2 = new TimeSpan(dateTime.Ticks);
									if (value.MissCount <= 0)
									{
										num2 = 1000;
										num3 = 0;
									}
									else if (value.MissCount <= 10)
									{
										num2 = 1500;
										num3 = 10;
									}
									else if (value.MissCount <= 600)
									{
										num2 = 2000;
										num3 = value.MissCount / 10 * 10;
									}
									else
									{
										num2 = 3000;
										num3 = 600;
									}
									if ((t - t2).TotalSeconds >= (double)num3)
									{
										num = value.Connect(num2);
										if (value.IsConnected)
										{
											this.SYNCTime(value);
										}
										else
										{
											value.MissCount++;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private DateTime DecodeZKTime(uint t)
		{
			uint second = t % 60u;
			t /= 60u;
			uint minute = t % 60u;
			t /= 60u;
			uint hour = t % 24u;
			t /= 24u;
			uint day = t % 31u + 1;
			t /= 31u;
			uint month = t % 12u + 1;
			t /= 12u;
			uint year = t + 2000;
			DateTime result;
			try
			{
				return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
			}
			catch (Exception)
			{
				result = new DateTime(1970, 1, 1, 1, 1, 1);
			}
			return result;
		}

		private int EncodeZKTime(DateTime t)
		{
			return t.Year % 100 * 12 * 31 + (t.Month - 1) * 31 + (t.Day - 1) * 24 * 60 * 60 + (t.Hour * 60 + t.Minute) * 60 + t.Second;
		}

		private void SYNCTime(DeviceServerBll deviceServerBll)
		{
			TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks);
			TimeSpan lastSetDeviceTime = deviceServerBll.LastSetDeviceTime;
			if (!((timeSpan - deviceServerBll.LastSetDeviceTime).TotalMinutes < (double)this.SYNCTimeInterval))
			{
				DeviceParamBll deviceParamBll = new DeviceParamBll(deviceServerBll.Application);
				deviceParamBll.SetDateTime(DateTime.Now);
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
					LogServer.Log("PullDeviceMonitor.tmrMonitor_Elapsed Exception: " + ex.Message, true);
				}
				finally
				{
					Monitor.Exit(this.objMonitorLock);
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
					foreach (KeyValuePair<string, DeviceServerBll> item in this.dicDevFlag_DeviceServer)
					{
						item.Value.Monitor = null;
					}
					this.dicDevFlag_DeviceServer.Clear();
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
								num = item.Value.Connect(3000);
								if (num < 0)
								{
									break;
								}
							}
							List<ObjRTLogInfo> list = new List<ObjRTLogInfo>();
							while (true)
							{
								List<ObjRTLogInfo> rTLogs = item.Value.GetRTLogs(ref num);
								if (rTLogs != null && rTLogs.Count > 0)
								{
									list.AddRange(rTLogs);
									if (rTLogs[rTLogs.Count - 1].EType != EventType.Type255)
									{
										bool flag = true;
										continue;
									}
								}
								break;
							}
							if (list != null && list.Count > 0)
							{
								ThreadPool.QueueUserWorkItem(delegate(object obj)
								{
									List<ObjRTLogInfo> list2 = obj as List<ObjRTLogInfo>;
									if (list2 != null && list2.Count > 0)
									{
										for (int i = 0; i < list2.Count; i++)
										{
											RTEventProcessor.CheckAlarm(item.Value, list2[i]);
											RTEventProcessor.SaveRecord(item.Value, list2[i]);
										}
									}
								}, list);
							}
						}
					}
				}
			}
		}

		~PullDeviceMonitor()
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
						int num = this.err_connection_break;
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
						int num = this.err_connection_break;
						if (!lstDoorDev[i].DevServer.IsConnected)
						{
							FailedCount++;
							frmWait.ShowInfos(lstDoorDev[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							switch (frmCloseDoor.Second)
							{
							case 1:
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
						int id = this.err_connection_break;
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
