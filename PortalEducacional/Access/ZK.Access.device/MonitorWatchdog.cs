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
using System.Linq;
using System.Threading;
using System.Timers;
using ZK.Access.door;
using ZK.ConfigManager;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.device
{
	public class MonitorWatchdog
	{
		public delegate void OnOfflineLogEventHandler(DeviceServerBll sender, OnOfflineLogEventArgs e);

		public delegate void OnOfflineLogCountEventHandler();

		public delegate void OnOfflineLogEndEventHandler();

		private static int Interval = 3000;

		public static string AlarmFile;

		private static object objWatchLock = new object();

		private static List<IDeviceMonitor> lstMonitor = new List<IDeviceMonitor>();

		private static Thread threadWatchdog;

		private static bool ShouldStopWatching;

		public static int WatchInterval
		{
			get
			{
				return MonitorWatchdog.Interval;
			}
			set
			{
				if (value != MonitorWatchdog.Interval)
				{
					MonitorWatchdog.Interval = value;
				}
			}
		}

		public static bool IsMonitoring
		{
			get
			{
				if (MonitorWatchdog.threadWatchdog == null || (MonitorWatchdog.threadWatchdog.ThreadState != 0 && MonitorWatchdog.threadWatchdog.ThreadState != ThreadState.WaitSleepJoin))
				{
					return false;
				}
				return true;
			}
		}

		public static bool ShouldStop => MonitorWatchdog.ShouldStopWatching;

		public static event OnOfflineLogEventHandler OnOfflineLog;

		public static event OnOfflineLogCountEventHandler OnOfflineLogCount;

		public static event OnOfflineLogEndEventHandler OnOfflineLogEnd;

		public static void StartWatchdog()
		{
			try
			{
				lock (MonitorWatchdog.objWatchLock)
				{
					MonitorWatchdog.ShouldStopWatching = false;
					if (MonitorWatchdog.threadWatchdog == null || MonitorWatchdog.threadWatchdog.ThreadState != 0)
					{
						MonitorWatchdog.threadWatchdog = new Thread(MonitorWatchdog.WatchingInThread);
						MonitorWatchdog.threadWatchdog.Start();
					}
				}
			}
			catch (Exception ex)
			{
				LogServer.Log("MonitorWatchdog.StartWatchdog Exception: " + ex.Message, true);
			}
		}

		private static void WatchingInThread()
		{
			SysLogServer.WriteLog("WatchingInThread...");
			while (!MonitorWatchdog.ShouldStopWatching)
			{
				if (Monitor.TryEnter(MonitorWatchdog.objWatchLock))
				{
					try
					{
						MonitorWatchdog.WatchingWithoutLock();
					}
					catch (Exception ex)
					{
						LogServer.Log("MonitorWatchdog.WatchingInThread Exception: " + ex.Message, true);
					}
					finally
					{
						Monitor.Exit(MonitorWatchdog.objWatchLock);
					}
				}
				Thread.Sleep(MonitorWatchdog.WatchInterval);
			}
		}

		private static void WatchingWithoutLock()
		{
			SysLogServer.WriteLog("WatchingWithoutLock...");
			GLOBAL.setMonitorKeepAlive();
			if (MonitorWatchdog.lstMonitor != null && MonitorWatchdog.lstMonitor.Count > 0)
			{
				for (int i = 0; i < MonitorWatchdog.lstMonitor.Count; i++)
				{
					if (MonitorWatchdog.ShouldStopWatching)
					{
						break;
					}
					if (!MonitorWatchdog.lstMonitor[i].IsMonitoring)
					{
						MonitorWatchdog.lstMonitor[i].StartMonitor();
					}
				}
			}
		}

		private static void tmrWatchdog_Elapsed(object sender, ElapsedEventArgs e)
		{
			SysLogServer.WriteLog("tmrWatchdog_Elapsed...");
			if (Monitor.TryEnter(MonitorWatchdog.objWatchLock))
			{
				try
				{
					MonitorWatchdog.WatchingWithoutLock();
				}
				catch (Exception ex)
				{
					LogServer.Log("MonitorWatchdog.tmrWatchdog_Elapsed Exception: " + ex.Message, true);
				}
				finally
				{
					Monitor.Exit(MonitorWatchdog.objWatchLock);
				}
			}
		}

		public static void StopWatchdog()
		{
			lock (MonitorWatchdog.objWatchLock)
			{
				try
				{
					MonitorWatchdog.ShouldStopWatching = true;
					if (MonitorWatchdog.lstMonitor != null && MonitorWatchdog.lstMonitor.Count > 0)
					{
						for (int i = 0; i < MonitorWatchdog.lstMonitor.Count; i++)
						{
							if (MonitorWatchdog.lstMonitor[i].IsMonitoring)
							{
								MonitorWatchdog.lstMonitor[i].StopMonitor();
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogServer.Log("MonitorWatchdog.StopWatchdog Exception: " + ex.Message, true);
				}
			}
		}

		public static void DisConnectAll()
		{
			int num = 0;
			lock (MonitorWatchdog.objWatchLock)
			{
				try
				{
					for (num = MonitorWatchdog.lstMonitor.Count - 1; num >= 0; num--)
					{
						ThreadPool.QueueUserWorkItem(delegate(object obj)
						{
							((IDeviceMonitor)obj).DisConnectAll();
						}, MonitorWatchdog.lstMonitor[num]);
					}
				}
				catch (Exception ex)
				{
					LogServer.Log("MonitorWatchdog.DisConnectAll Exception: " + ex.Message, true);
				}
			}
		}

		public static void AddMonitor(IDeviceMonitor monitor)
		{
			lock (MonitorWatchdog.objWatchLock)
			{
				if (!MonitorWatchdog.lstMonitor.Contains(monitor))
				{
					MonitorWatchdog.lstMonitor.Add(monitor);
				}
			}
		}

		public static void AddRange(List<IDeviceMonitor> lst)
		{
			if (lst != null && lst.Count > 0)
			{
				lock (MonitorWatchdog.objWatchLock)
				{
					for (int i = 0; i < lst.Count; i++)
					{
						if (!MonitorWatchdog.lstMonitor.Contains(lst[i]))
						{
							MonitorWatchdog.lstMonitor.Add(lst[i]);
						}
					}
				}
			}
		}

		public static void RemoveMonitor(IDeviceMonitor monitor)
		{
			lock (MonitorWatchdog.objWatchLock)
			{
				if (MonitorWatchdog.lstMonitor.Contains(monitor))
				{
					MonitorWatchdog.lstMonitor.Remove(monitor);
				}
			}
		}

		public static void RemoveRange(List<IDeviceMonitor> lst)
		{
			if (lst != null && lst.Count > 0)
			{
				lock (MonitorWatchdog.objWatchLock)
				{
					for (int i = 0; i < lst.Count; i++)
					{
						if (MonitorWatchdog.lstMonitor.Contains(lst[i]))
						{
							MonitorWatchdog.lstMonitor.Remove(lst[i]);
						}
					}
				}
			}
		}

		protected static void ClearMonitorWithoutLock()
		{
			for (int i = 0; i < MonitorWatchdog.lstMonitor.Count; i++)
			{
				MonitorWatchdog.lstMonitor[i].Dispose();
			}
			MonitorWatchdog.lstMonitor.Clear();
		}

		public static void ClearMonitor()
		{
			lock (MonitorWatchdog.objWatchLock)
			{
				MonitorWatchdog.ClearMonitorWithoutLock();
			}
		}

		public static void InitialMonitors()
		{
			lock (MonitorWatchdog.objWatchLock)
			{
				MonitorWatchdog.ClearMonitorWithoutLock();
				RTEventProcessor.LoadDictionary(true);
				for (int i = 0; i < DeviceServers.Instance.Count; i++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[i];
					if (deviceServerBll != null && deviceServerBll.DevInfo != null && deviceServerBll.DevInfo.ID > 0 && deviceServerBll.IsAddOk && deviceServerBll.IsNeedListen)
					{
						switch (deviceServerBll.DevInfo.ConnectType)
						{
						case 0:
						{
							IDeviceMonitor deviceMonitor = MonitorWatchdog.Get485Monitor(deviceServerBll);
							deviceMonitor.AddDeviceServer(deviceServerBll);
							break;
						}
						case 1:
						{
							SDKType devSDKType = deviceServerBll.DevInfo.DevSDKType;
							if (devSDKType == SDKType.StandaloneSDK)
							{
								IDeviceMonitor deviceMonitor = new StdDeviceMonitor(0);
								deviceMonitor.AddDeviceServer(deviceServerBll);
								MonitorWatchdog.lstMonitor.Add(deviceMonitor);
							}
							else
							{
								IDeviceMonitor deviceMonitor = new PullDeviceMonitor(0);
								deviceMonitor.AddDeviceServer(deviceServerBll);
								MonitorWatchdog.lstMonitor.Add(deviceMonitor);
							}
							break;
						}
						}
					}
				}
			}
		}

		private static void MonitorWatchdog_OnOfflineLogEnd()
		{
			if (MonitorWatchdog.OnOfflineLogEnd != null)
			{
				MonitorWatchdog.OnOfflineLogEnd();
			}
		}

		private static void MonitorWatchdog_OnOfflineLogCount()
		{
			if (MonitorWatchdog.OnOfflineLogCount != null)
			{
				MonitorWatchdog.OnOfflineLogCount();
			}
		}

		private static void MonitorWatchdog_OnOfflineLog(DeviceServerBll sender, OnOfflineLogEventArgs e)
		{
			if (MonitorWatchdog.OnOfflineLog != null)
			{
				MonitorWatchdog.OnOfflineLog(sender, e);
			}
		}

		private static IDeviceMonitor Get485Monitor(DeviceServerBll devServer)
		{
			for (int i = 0; i < MonitorWatchdog.lstMonitor.Count; i++)
			{
				if (MonitorWatchdog.lstMonitor[i].SerialPort == devServer.DevInfo.SerialPort)
				{
					SDKType devSDKType = devServer.DevInfo.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (MonitorWatchdog.lstMonitor[i] is StdDeviceMonitor)
						{
							return MonitorWatchdog.lstMonitor[i];
						}
					}
					else if (MonitorWatchdog.lstMonitor[i] is PullDeviceMonitor)
					{
						return MonitorWatchdog.lstMonitor[i];
					}
				}
			}
			SDKType devSDKType2 = devServer.DevInfo.DevSDKType;
			IDeviceMonitor deviceMonitor = (devSDKType2 != SDKType.StandaloneSDK) ? ((IDeviceMonitor)new PullDeviceMonitor(devServer.DevInfo.SerialPort)) : ((IDeviceMonitor)new StdDeviceMonitor(devServer.DevInfo.SerialPort));
			MonitorWatchdog.lstMonitor.Add(deviceMonitor);
			return deviceMonitor;
		}

		private static IDeviceMonitor GetNetMonitor(DeviceServerBll devServer)
		{
			for (int i = 0; i < MonitorWatchdog.lstMonitor.Count; i++)
			{
				if (MonitorWatchdog.lstMonitor[i].SerialPort == 0)
				{
					SDKType devSDKType = devServer.DevInfo.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (MonitorWatchdog.lstMonitor[i] is StdDeviceMonitor)
						{
							return MonitorWatchdog.lstMonitor[i];
						}
					}
					else if (MonitorWatchdog.lstMonitor[i] is PullDeviceMonitor)
					{
						return MonitorWatchdog.lstMonitor[i];
					}
				}
			}
			SDKType devSDKType2 = devServer.DevInfo.DevSDKType;
			IDeviceMonitor deviceMonitor = (devSDKType2 != SDKType.StandaloneSDK) ? ((IDeviceMonitor)new PullDeviceMonitor(0)) : ((IDeviceMonitor)new StdDeviceMonitor(0));
			MonitorWatchdog.lstMonitor.Add(deviceMonitor);
			return deviceMonitor;
		}

		public static void OpenDoors(List<DevControl> lstDoorDev, OpenDoorSet frmOpenDoor, WaitForm frmWait)
		{
			IEnumerable<DevControl> enumerable = from door in lstDoorDev
			where door.DevInfo == null
			select door;
			IEnumerable<DevControl> enumerable2 = from door in lstDoorDev
			where door.DevInfo != null
			select door;
			int TotalCount = lstDoorDev.Count;
			int TotalSucceed = 0;
			int TotalFailed = enumerable.Count();
			object objControlLock = new object();
			bool HasFinished = false;
			frmWait.ShowEx();
			frmWait.ShowProgress((TotalSucceed + TotalFailed) * 100 / TotalCount);
			foreach (DevControl item in enumerable)
			{
				frmWait.ShowInfos(item.DoorName + ":" + PullSDkErrorInfos.GetInfo(-1002));
			}
			foreach (IDeviceMonitor item2 in MonitorWatchdog.lstMonitor)
			{
				int SucceedCount = 0;
				int FailedCount = 0;
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					((IDeviceMonitor)obj).OpenDoors(lstDoorDev, frmOpenDoor, frmWait, out SucceedCount, out FailedCount);
					lock (objControlLock)
					{
						TotalSucceed += SucceedCount;
						TotalFailed += FailedCount;
						frmWait.ShowProgress((TotalSucceed + TotalFailed) * 100 / TotalCount);
						if (!HasFinished && TotalSucceed + TotalFailed >= TotalCount)
						{
							frmWait.ShowInfos(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + TotalSucceed + "\t" + ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + TotalFailed + "\r\n");
							HasFinished = true;
							frmWait.HideEx(false);
						}
					}
				}, item2);
			}
		}

		public static void CloseDoor(List<DevControl> lstDoorDev, CloseDoorSet frmCloseDoor, WaitForm frmWait)
		{
			IEnumerable<DevControl> enumerable = from door in lstDoorDev
			where door.DevInfo == null
			select door;
			IEnumerable<DevControl> enumerable2 = from door in lstDoorDev
			where door.DevInfo != null
			select door;
			int TotalCount = lstDoorDev.Count;
			int TotalSucceed = 0;
			int TotalFailed = enumerable.Count();
			object objControlLock = new object();
			bool HasFinished = false;
			frmWait.ShowEx();
			frmWait.ShowProgress((TotalSucceed + TotalFailed) * 100 / TotalCount);
			foreach (DevControl item in enumerable)
			{
				frmWait.ShowInfos(item.DoorName + ":" + PullSDkErrorInfos.GetInfo(-1002));
			}
			foreach (IDeviceMonitor item2 in MonitorWatchdog.lstMonitor)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					int num = 0;
					int num2 = 0;
					((IDeviceMonitor)obj).CloseDoor(lstDoorDev, frmCloseDoor, frmWait, out num, out num2);
					lock (objControlLock)
					{
						TotalSucceed += num;
						TotalFailed += num2;
						frmWait.ShowProgress((TotalSucceed + TotalFailed) * 100 / TotalCount);
						if (!HasFinished && TotalSucceed + TotalFailed >= TotalCount)
						{
							frmWait.ShowInfos(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + TotalSucceed + "\t" + ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + TotalFailed + "\r\n");
							HasFinished = true;
							frmWait.HideEx(false);
						}
					}
				}, item2);
			}
		}

		public static void CancelAlarm(List<DevControl> lstDoorDev, WaitForm frmWait)
		{
			IEnumerable<DevControl> enumerable = from door in lstDoorDev
			where door.DevInfo == null
			select door;
			IEnumerable<DevControl> enumerable2 = from door in lstDoorDev
			where door.DevInfo != null
			select door;
			int TotalCount = lstDoorDev.Count;
			int TotalSucceed = 0;
			int TotalFailed = enumerable.Count();
			object objControlLock = new object();
			bool HasFinished = false;
			frmWait.ShowEx();
			frmWait.ShowProgress((TotalSucceed + TotalFailed) * 100 / TotalCount);
			foreach (DevControl item in enumerable)
			{
				frmWait.ShowInfos(item.DoorName + ":" + PullSDkErrorInfos.GetInfo(-1002));
			}
			foreach (IDeviceMonitor item2 in MonitorWatchdog.lstMonitor)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					int num = 0;
					int num2 = 0;
					((IDeviceMonitor)obj).CancelAlarm(lstDoorDev, frmWait, out num, out num2);
					lock (objControlLock)
					{
						TotalSucceed += num;
						TotalFailed += num2;
						frmWait.ShowProgress((TotalSucceed + TotalFailed) * 100 / TotalCount);
						if (!HasFinished && TotalSucceed + TotalFailed >= TotalCount)
						{
							frmWait.ShowInfos(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + TotalSucceed + "\t" + ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + TotalFailed + "\r\n");
							HasFinished = true;
							frmWait.HideEx(false);
						}
					}
				}, item2);
			}
		}
	}
}
