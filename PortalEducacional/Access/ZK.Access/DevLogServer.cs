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
using System.IO;
using System.Media;
using System.Text;
using System.Threading;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class DevLogServer
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private static int m_count = 1;

		public static string AlarmFilePath = string.Empty;

		public static int ReMonitorTime = 2;

		private static int m_reConnectCount = 10;

		private static int m_sleepTime = 1000;

		private static List<MonitorThread> m_theads = new List<MonitorThread>();

		private static bool m_isStop = false;

		private static List<int> com_theads = new List<int>();

		private static Thread thread;

		private static bool m_isLock;

		private static MachinesBll machinesbll;

		private static AccDoorBll doorBll;

		private static DeviceHelperEx helper;

		private static bool m_reConnect;

		private static bool m_lookFinish;

		private static int m_openType;

		private static int m_closeType;

		private static int m_right;

		private static int m_error;

		private static int m_allCount;

		private static bool m_startOpenOrCloseDoor;

		private static bool m_setFinish;

		private static bool m_isOpenOrCloseFinish;

		public static List<DevControl> m_openDevControl;

		public static List<DevControl> m_closeDevControl;

		public static List<DevControl> m_finishDevControl;

		private static bool is_OnAarrm;

		private static bool m_lockOpenDev;

		private static bool m_lockCloseDev;

		private static StringBuilder sb;

		public static int TheadCount
		{
			get
			{
				return DevLogServer.m_count;
			}
			set
			{
				DevLogServer.m_count = value;
			}
		}

		public static int ReConnectCount
		{
			get
			{
				return DevLogServer.m_reConnectCount;
			}
			set
			{
				DevLogServer.m_reConnectCount = value;
			}
		}

		public static int SleepTime
		{
			get
			{
				return DevLogServer.m_sleepTime;
			}
			set
			{
				if (value > 0)
				{
					DevLogServer.m_sleepTime = value;
				}
			}
		}

		public static bool IsStop
		{
			get
			{
				return DevLogServer.m_isStop;
			}
			set
			{
				DevLogServer.m_isStop = value;
			}
		}

		public static int OpenType
		{
			get
			{
				return DevLogServer.m_openType;
			}
			set
			{
				DevLogServer.m_openType = value;
			}
		}

		public static int CloseType
		{
			get
			{
				return DevLogServer.m_closeType;
			}
			set
			{
				DevLogServer.m_closeType = value;
			}
		}

		public static int RightCount
		{
			get
			{
				return DevLogServer.m_right;
			}
			set
			{
				DevLogServer.m_right = value;
			}
		}

		public static int ErrorCount
		{
			get
			{
				return DevLogServer.m_error;
			}
			set
			{
				DevLogServer.m_error = value;
			}
		}

		public static int AllCount
		{
			get
			{
				return DevLogServer.m_allCount;
			}
			set
			{
				DevLogServer.m_allCount = value;
			}
		}

		public static bool SetFinish
		{
			get
			{
				return DevLogServer.m_setFinish;
			}
			set
			{
				DevLogServer.m_setFinish = value;
			}
		}

		public static bool IsOpenOrCloseFinish => DevLogServer.m_isOpenOrCloseFinish;

		public static bool IsCmdFinish
		{
			get
			{
				if (DevLogServer.m_openDevControl.Count > 0 || DevLogServer.m_closeDevControl.Count > 0)
				{
					return false;
				}
				return true;
			}
		}

		public static event ShowInfo ShowInfoEvent;

		public static event ShowProgressHandle ShowProgressEvent;

		public static event EventHandler FinishEvent;

		public static event EventHandler DeleteDoorEvent;

		public static bool OnStart()
		{
			if (DevLogServer.thread == null)
			{
				DevLogServer.thread = new Thread(DevLogServer.Start);
				DevLogServer.thread.Start();
				SysLogServer.WriteLog("DevLogServer:OnStart ");
			}
			return true;
		}

		public static void LockEx()
		{
			DevLogServer.m_isLock = true;
		}

		public static void UnLockEx()
		{
			DevLogServer.m_isLock = false;
		}

		private static void AddMachines(DeviceServerBll devServer)
		{
			try
			{
				if (devServer != null && devServer.ReConectCount < DevLogServer.ReConnectCount * 4)
				{
					int num = devServer.Connect(3000);
					if (num >= 0)
					{
						Machines model = DevLogServer.machinesbll.GetModel(devServer.DevInfo.ID);
						if (model != null)
						{
							int acpanel_type = model.acpanel_type;
							num = DeviceHelper.GetDeviceParams(model);
							if (num >= 0 && model.acpanel_type > 0)
							{
								SysLogServer.WriteLog("DevLogServer,GetDeviceParams success  " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias + " oldacpanel_type " + acpanel_type + " realacpanel_type " + model.acpanel_type + "\r\n");
								if (model.acpanel_type != acpanel_type)
								{
									List<AccDoor> modelList = DevLogServer.doorBll.GetModelList("device_id=" + model.ID);
									if (modelList != null && modelList.Count > 0)
									{
										for (int i = 0; i < modelList.Count; i++)
										{
											DevLogServer.doorBll.Delete(modelList[i].id);
										}
									}
									DevLogServer.machinesbll.Update(model);
									SysLogServer.WriteLog(DeviceHelper.SaveDoorInfo(model, true));
									devServer.IsAddOk = true;
									if (DevLogServer.DeleteDoorEvent != null)
									{
										DevLogServer.DeleteDoorEvent(model, null);
									}
								}
								else
								{
									DevLogServer.machinesbll.Update(model);
									DevLogServer.helper.UpdateDoorSetings(devServer);
									devServer.IsAddOk = true;
								}
							}
							else
							{
								SysLogServer.WriteLog("DevLogServer,GetDeviceParams Fail  " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias + " ret:" + PullSDkErrorInfos.GetInfo(num) + "\r\n");
							}
						}
					}
					else
					{
						SysLogServer.WriteLog("DevLogServer,Connect Fail  " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias + " ret:" + PullSDkErrorInfos.GetInfo(num) + "\r\n");
					}
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer,AddMachines  " + ex.Message + ex.Source);
			}
		}

		private static void ListenThead()
		{
			try
			{
				for (int i = 0; i < DevLogServer.m_theads.Count; i++)
				{
					if (DevLogServer.m_theads[i].DevList.Count > 0 && DevLogServer.m_theads[i] != null && !DevLogServer.m_theads[i].IsRun)
					{
						DevLogServer.m_theads[i].OnStart();
					}
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer,ListenThead  " + ex.Message + ex.Source);
			}
		}

		private static void ListenDevAndCreateThead(DeviceServerBll devServer)
		{
			try
			{
				if (devServer.Index == -1 && devServer.IsNeedListen && devServer.IsHaveListener)
				{
					if (devServer.ReConectCount == DevLogServer.ReConnectCount * 2)
					{
						devServer.ReConectCount++;
					}
					if (devServer.ReConectCount < DevLogServer.ReConnectCount * 2 || (devServer.ReConectCount > DevLogServer.ReConnectCount * 2 && devServer.LastConnectTime.AddMinutes((double)DevLogServer.ReMonitorTime) < DateTime.Now))
					{
						if (devServer.DevInfo.ConnectType == 0)
						{
							bool flag = false;
							int num = 0;
							while (num < DevLogServer.m_theads.Count)
							{
								if (DevLogServer.m_theads[num].Index != devServer.DevInfo.SerialPort)
								{
									num++;
									continue;
								}
								devServer.Index = DevLogServer.m_theads[num].Index;
								DevLogServer.m_theads[num].DevList.Add(devServer);
								flag = true;
								if (!DevLogServer.m_theads[num].IsRun)
								{
									DevLogServer.m_theads[num].OnStart();
									if (devServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
									{
										devServer.STD_StartMonitor();
									}
								}
								SysLogServer.WriteLog("DevLogServer,Add devive to Thread,Thread index:" + devServer.Index + " name " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias);
								break;
							}
							if (!flag && !DevLogServer.com_theads.Contains(devServer.DevInfo.SerialPort))
							{
								DevLogServer.com_theads.Add(devServer.Index);
								MonitorThread monitorThread = new MonitorThread(devServer.DevInfo.SerialPort);
								DevLogServer.m_theads.Add(monitorThread);
								devServer.Index = monitorThread.Index;
								monitorThread.DevList.Add(devServer);
								monitorThread.OnStart();
								if (devServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
								{
									devServer.STD_StartMonitor();
								}
								SysLogServer.WriteLog("DevLogServer create Thread,Thread index:" + devServer.Index + " name " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias);
							}
						}
						else
						{
							bool flag2 = false;
							devServer.LastConnectTime = DateTime.Now;
							int num2 = 0;
							while (num2 < DevLogServer.m_theads.Count)
							{
								if (DevLogServer.m_theads[num2].DevList.Count >= DevLogServer.m_count)
								{
									num2++;
									continue;
								}
								devServer.Index = DevLogServer.m_theads[num2].Index;
								DevLogServer.m_theads[num2].DevList.Add(devServer);
								flag2 = true;
								if (!DevLogServer.m_theads[num2].IsRun)
								{
									DevLogServer.m_theads[num2].OnStart();
								}
								SysLogServer.WriteLog("DevLogServer,Add devive to Thread,Thread index:" + devServer.Index + " name " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias);
								break;
							}
							if (!flag2)
							{
								MonitorThread monitorThread2 = new MonitorThread(DevLogServer.m_theads.Count + 255);
								DevLogServer.m_theads.Add(monitorThread2);
								devServer.Index = monitorThread2.Index;
								monitorThread2.DevList.Add(devServer);
								monitorThread2.OnStart();
								SysLogServer.WriteLog("DevLogServer create Thread,Thread index:" + devServer.Index + " name " + devServer.DevInfo.ID + "_" + devServer.DevInfo.MachineAlias);
							}
						}
					}
				}
				else if (devServer.Index == -1 && devServer.IsHaveListener && !devServer.IsNeedListen)
				{
					ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
					objRTLogInfo.DevID = devServer.DevInfo.ID;
					objRTLogInfo.EType = EventType.Type302;
					objRTLogInfo.StatusInfo = string.Empty;
					devServer.OnRTLogEvent(objRTLogInfo);
				}
				else if (devServer.Index == -1 && devServer.IsHaveListener && devServer.IsNeedListen && devServer.ReConectCount >= DevLogServer.ReConnectCount * 2)
				{
					ObjRTLogInfo objRTLogInfo2 = new ObjRTLogInfo();
					objRTLogInfo2.DevID = devServer.DevInfo.ID;
					objRTLogInfo2.EType = EventType.Type300;
					objRTLogInfo2.StatusInfo = PullSDkErrorInfos.GetInfo(-1007);
					devServer.OnRTLogEvent(objRTLogInfo2);
				}
				if (!devServer.IsNeedListen && devServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
				{
					devServer.STD_StopMonitor();
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer,ListenDevAndCreateThead  " + ex.Message + ex.Source);
			}
		}

		private static void Start()
		{
			SysLogServer.WriteLog("DevLogServer:Start!");
			UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
			DateTime now = DateTime.Now;
			try
			{
				userInfoBll.Load();
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				devCmdsBll.Clear();
			}
			catch
			{
			}
			int num = 0;
			DevLogServer.com_theads.Clear();
			while (!DevLogServer.m_isStop)
			{
				try
				{
					DevLogServer.OnOpenOrCloseEvent();
					DevLogServer.OnIsAlarm();
					if (!DevLogServer.m_isLock)
					{
						DevLogServer.m_isLock = true;
						DevLogServer.ListenThead();
						bool flag = false;
						for (int i = 0; i < DeviceServers.Instance.Count; i++)
						{
							DeviceServerBll deviceServerBll = DeviceServers.Instance[i];
							if (deviceServerBll != null && !deviceServerBll.IsDel && deviceServerBll.DevInfo != null && deviceServerBll.DevInfo.ID > 0)
							{
								if (deviceServerBll.IsAddOk && deviceServerBll.IsHaveListener)
								{
									flag = true;
									if (deviceServerBll.Index < 0)
									{
										if (num == 0)
										{
											if (deviceServerBll.DevInfo.ConnectType != 0)
											{
												DevLogServer.ListenDevAndCreateThead(deviceServerBll);
											}
										}
										else
										{
											DevLogServer.ListenDevAndCreateThead(deviceServerBll);
										}
									}
								}
								else if (!deviceServerBll.IsAddOk)
								{
									flag = true;
									if (num > 50)
									{
										DevLogServer.AddMachines(deviceServerBll);
									}
								}
								else if (deviceServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
								{
									deviceServerBll.IsFirst = true;
								}
							}
						}
						if (flag)
						{
							if (num < 100)
							{
								num++;
							}
						}
						else if (num < 100)
						{
							num++;
						}
						DevLogServer.m_reConnect = false;
						DevLogServer.m_isLock = false;
					}
					if (now.AddMinutes(2.0) < DateTime.Now)
					{
						try
						{
							userInfoBll.Load();
						}
						catch (Exception ex)
						{
							SysLogServer.WriteLog("DevLogServer Start function bll.Load() :" + ex.Message + " " + ex.Source);
						}
						now = DateTime.Now;
					}
					else
					{
						Thread.Sleep(DevLogServer.m_sleepTime);
					}
				}
				catch (Exception ex2)
				{
					SysLogServer.WriteLog("DevLogServer Start function :" + ex2.Message + " " + ex2.Source);
				}
			}
			SysLogServer.WriteLog("DevLogServer:Finish!");
			DevLogServer.thread = null;
		}

		public static bool OnStop()
		{
			DevLogServer.m_isStop = true;
			DevLogServer.m_isLock = true;
			SysLogServer.WriteLog("DevLogServer:start stop");
			Thread.Sleep(DevLogServer.m_sleepTime);
			for (int i = 0; i < DevLogServer.m_theads.Count; i++)
			{
				try
				{
					if (DevLogServer.m_theads[i] != null)
					{
						DevLogServer.m_theads[i].OnStop();
					}
				}
				catch
				{
				}
			}
			SysLogServer.WriteLog("DevLogServer:stop finish ", true);
			return true;
		}

		public static void ReConnect()
		{
			DevLogServer.m_reConnect = true;
		}

		public static void Lock()
		{
			try
			{
				for (int i = 0; i < DevLogServer.m_theads.Count; i++)
				{
					if (DevLogServer.m_theads[i] != null)
					{
						DevLogServer.m_theads[i].RequestLock();
					}
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer Lock function :" + ex.Message + " " + ex.Source);
			}
			DevLogServer.m_lookFinish = true;
		}

		public static void UnLock()
		{
			try
			{
				for (int i = 0; i < DevLogServer.m_theads.Count; i++)
				{
					if (DevLogServer.m_theads[i] != null)
					{
						DevLogServer.m_theads[i].UnRequestLock();
					}
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer UnLock function :" + ex.Message + " " + ex.Source);
			}
		}

		public static void ReSet()
		{
			DevLogServer.m_lookFinish = false;
			DevLogServer.m_allCount = 0;
			DevLogServer.m_error = 0;
			DevLogServer.m_right = 0;
			DevLogServer.m_closeType = -1;
			DevLogServer.m_openType = -1;
			DevLogServer.m_startOpenOrCloseDoor = true;
			DevLogServer.m_setFinish = false;
			DevLogServer.m_isOpenOrCloseFinish = false;
			DevLogServer.m_finishDevControl.Clear();
		}

		private static void FinishOpenOrClose()
		{
			try
			{
				SysLogServer.WriteLog("startFinishOpenOrClose\r\n");
				DevLogServer.m_isLock = false;
				if (DevLogServer.AllCount != DevLogServer.ErrorCount + DevLogServer.RightCount)
				{
					Thread.Sleep(1000);
				}
				DevLogServer.UnLock();
				DevLogServer.OnShowProgress();
				DevLogServer.OnShowInfo();
				Thread.Sleep(1000);
				DevLogServer.OnShowInfo();
				DevLogServer.OnFinish();
				DevLogServer.m_isOpenOrCloseFinish = true;
				SysLogServer.WriteLog("FinishOpenOrClose\r\n");
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer FinishOpenOrClose function :" + ex.Message + " " + ex.Source);
				try
				{
					DevLogServer.OnFinish();
					DevLogServer.m_isOpenOrCloseFinish = true;
					SysLogServer.WriteLog("FinishOpenOrClose\r\n");
				}
				catch (Exception ex2)
				{
					SysLogServer.WriteLog("DevLogServer FinishOpenOrClose OnFinish :" + ex2.Message + " " + ex2.Source);
				}
			}
		}

		public static void AddOpenDevControl(DevControl dev)
		{
			DevLogServer.m_lockCloseDev = true;
			DevLogServer.m_openDevControl.Add(dev);
			DevLogServer.m_lockCloseDev = false;
		}

		public static void AddCloseDevControl(DevControl dev)
		{
			DevLogServer.m_lockCloseDev = true;
			DevLogServer.m_closeDevControl.Add(dev);
			DevLogServer.m_lockCloseDev = false;
		}

		private static void OnFinish()
		{
			try
			{
				if (DevLogServer.FinishEvent != null)
				{
					SysLogServer.WriteLog("FinishEvent\r\n");
					DevLogServer.FinishEvent(null, null);
				}
				else
				{
					SysLogServer.WriteLog("noFinishEvent\r\n");
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer noFinishEvent function :" + ex.Message + " " + ex.Source);
			}
		}

		private static void OnShowInfo()
		{
			try
			{
				if (DevLogServer.sb != null && DevLogServer.sb.Length > 0 && DevLogServer.ShowInfoEvent != null)
				{
					string text = DevLogServer.sb.ToString();
					DevLogServer.sb.Remove(0, text.Length);
					DevLogServer.ShowInfoEvent(text);
				}
			}
			catch
			{
			}
		}

		private static void OnShowProgress()
		{
			if (DevLogServer.ShowProgressEvent != null && DevLogServer.AllCount > 0)
			{
				try
				{
					int currProgress = (DevLogServer.ErrorCount + DevLogServer.RightCount) * 100 / DevLogServer.AllCount;
					DevLogServer.ShowProgressEvent(currProgress);
				}
				catch
				{
				}
			}
		}

		private static void OnIsAlarm()
		{
			if (!string.IsNullOrEmpty(DevLogServer.AlarmFilePath) && File.Exists(DevLogServer.AlarmFilePath))
			{
				bool flag = false;
				for (int i = 0; i < DeviceServers.Instance.Count; i++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[i];
					if (deviceServerBll != null && !deviceServerBll.IsDel && deviceServerBll.DevInfo != null && deviceServerBll.DevInfo.ID > 0 && deviceServerBll.IsAddOk && deviceServerBll.IsHaveListener)
					{
						int num = 0;
						while (num < deviceServerBll.IsAlarm.Length)
						{
							if (!deviceServerBll.IsAlarm[num])
							{
								num++;
								continue;
							}
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					SoundPlayer soundPlayer = new SoundPlayer(DevLogServer.AlarmFilePath);
					soundPlayer.Play();
				}
			}
		}

		private static void OnOpenOrCloseEvent()
		{
			try
			{
				if (DevLogServer.m_startOpenOrCloseDoor)
				{
					DevLogServer.m_startOpenOrCloseDoor = false;
					DevLogServer.Lock();
				}
				if (DevLogServer.m_theads.Count == 0)
				{
					while (!DevLogServer.IsCmdFinish)
					{
						if (DevLogServer.m_openDevControl.Count > 0)
						{
							DevLogServer.OpenDoor(-1);
						}
						else
						{
							if (DevLogServer.m_closeDevControl.Count <= 0)
							{
								break;
							}
							DevLogServer.CloseDoor(-1);
						}
					}
				}
				DevLogServer.OnShowProgress();
				DevLogServer.OnShowInfo();
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer OnOpenOrCloseEvent function :" + ex.Message + " " + ex.Source);
			}
		}

		private static DevControl GetOpenDevControl(out int isfirstFinish, out int isfirstFinish2, int index)
		{
			isfirstFinish = 0;
			isfirstFinish2 = 0;
			DevControl devControl = null;
			if (DevLogServer.m_setFinish && !DevLogServer.m_lockOpenDev)
			{
				DevLogServer.m_lockOpenDev = true;
				if (DevLogServer.m_openDevControl.Count > 0)
				{
					int num = 0;
					while (num < DevLogServer.m_openDevControl.Count)
					{
						if (DevLogServer.m_openDevControl[num].DevServer == null || (DevLogServer.m_openDevControl[num].DevServer.Index != index && index != -1 && DevLogServer.m_openDevControl[num].DevServer.Index != -1))
						{
							if (DevLogServer.m_openDevControl[num].DevServer == null)
							{
								devControl = DevLogServer.m_openDevControl[num];
								DevLogServer.m_openDevControl.RemoveAt(num);
								break;
							}
							num++;
							continue;
						}
						devControl = DevLogServer.m_openDevControl[num];
						DevLogServer.m_openDevControl.RemoveAt(num);
						break;
					}
					if (DevLogServer.m_openDevControl.Count == 0)
					{
						isfirstFinish = 1;
					}
					else if (devControl == null)
					{
						isfirstFinish2 = 1;
					}
				}
				DevLogServer.m_lockOpenDev = false;
			}
			return devControl;
		}

		private static DevControl GetCloseDevControl(out int isfirstFinish, out int isfirstFinish2, int index)
		{
			isfirstFinish = 0;
			isfirstFinish2 = 0;
			DevControl devControl = null;
			if (DevLogServer.m_setFinish && !DevLogServer.m_lockCloseDev)
			{
				DevLogServer.m_lockCloseDev = true;
				if (DevLogServer.m_closeDevControl.Count > 0)
				{
					int num = 0;
					while (num < DevLogServer.m_closeDevControl.Count)
					{
						if (DevLogServer.m_closeDevControl[num].DevServer == null || (DevLogServer.m_closeDevControl[num].DevServer.Index != index && index != -1 && DevLogServer.m_closeDevControl[num].DevServer.Index != -1))
						{
							if (DevLogServer.m_closeDevControl[num].DevServer == null)
							{
								devControl = DevLogServer.m_closeDevControl[num];
								DevLogServer.m_closeDevControl.RemoveAt(num);
								break;
							}
							num++;
							continue;
						}
						devControl = DevLogServer.m_closeDevControl[num];
						DevLogServer.m_closeDevControl.RemoveAt(num);
						break;
					}
					if (DevLogServer.m_closeDevControl.Count == 0)
					{
						isfirstFinish = 1;
					}
					else if (devControl == null)
					{
						isfirstFinish2 = 1;
					}
				}
				DevLogServer.m_lockCloseDev = false;
			}
			return devControl;
		}

		public static bool OpenDoor(int index)
		{
			try
			{
				if (DevLogServer.m_startOpenOrCloseDoor)
				{
					DevLogServer.m_startOpenOrCloseDoor = false;
					DevLogServer.Lock();
				}
				if (DevLogServer.m_openDevControl.Count >= 0 && DevLogServer.m_lookFinish && DevLogServer.m_setFinish)
				{
					int num = 0;
					int num2 = 0;
					DevControl openDevControl = DevLogServer.GetOpenDevControl(out num, out num2, index);
					if (openDevControl != null && DevLogServer.m_openType != -1 && !DevLogServer.m_finishDevControl.Contains(openDevControl))
					{
						DevLogServer.m_finishDevControl.Add(openDevControl);
						Thread.Sleep(100);
						int num3 = 0;
						if (DevLogServer.OpenType == -255)
						{
							num3 = openDevControl.OpenDoor();
						}
						else if (DevLogServer.OpenType == -100)
						{
							num3 = openDevControl.NormalOpen(true);
						}
						else if (DevLogServer.OpenType < 255 && DevLogServer.OpenType > 0)
						{
							num3 = openDevControl.OpenDoor(DevLogServer.OpenType);
						}
						if (num3 < 0)
						{
							SysLogServer.WriteLog("OpenDoor " + openDevControl.DoorName + ":" + PullSDkErrorInfos.GetInfo(num3) + "\r\n");
							DevLogServer.sb.Append(openDevControl.DoorName + ":" + PullSDkErrorInfos.GetInfo(num3) + "\r\n");
							DevLogServer.m_error++;
						}
						else
						{
							DevLogServer.sb.Append(openDevControl.DoorName + ":" + ShowMsgInfos.GetInfo("SIsSuccess", "成功") + "\r\n");
							SysLogServer.WriteLog("OpenDoor " + openDevControl.DoorName + ": success！ \r\n");
							DevLogServer.m_right++;
						}
					}
					if (num == 1)
					{
						DevLogServer.FinishOpenOrClose();
						SysLogServer.WriteLog("CloseDoor isfirstFinish  \r\n", true);
					}
					else if (openDevControl == null)
					{
						DevLogServer.OnShowProgress();
						DevLogServer.OnShowInfo();
						if (num2 == 1)
						{
							return false;
						}
						Thread.Sleep(500);
					}
				}
				else if (DevLogServer.m_openDevControl.Count == 0)
				{
					DevLogServer.OnShowProgress();
					DevLogServer.OnShowInfo();
					Thread.Sleep(500);
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer OpenDoor function :" + ex.Message + " " + ex.Source);
			}
			return true;
		}

		public static bool CloseDoor(int index)
		{
			try
			{
				if (DevLogServer.m_startOpenOrCloseDoor)
				{
					DevLogServer.m_startOpenOrCloseDoor = false;
					DevLogServer.Lock();
				}
				if (DevLogServer.m_closeDevControl.Count > 0 && DevLogServer.m_lookFinish && DevLogServer.m_setFinish)
				{
					int num = 0;
					int num2 = 0;
					DevControl closeDevControl = DevLogServer.GetCloseDevControl(out num, out num2, index);
					if (closeDevControl != null && DevLogServer.m_closeType != -1 && !DevLogServer.m_finishDevControl.Contains(closeDevControl))
					{
						DevLogServer.m_finishDevControl.Add(closeDevControl);
						Thread.Sleep(100);
						int num3 = 0;
						if (DevLogServer.CloseType == 1)
						{
							num3 = closeDevControl.CloseDoor();
						}
						else if (DevLogServer.CloseType > 0)
						{
							num3 = closeDevControl.NormalOpen(false);
						}
						if (num3 < 0)
						{
							SysLogServer.WriteLog("CloseDoor " + closeDevControl.DoorName + ":" + PullSDkErrorInfos.GetInfo(num3) + "\r\n");
							DevLogServer.sb.Append(closeDevControl.DoorName + ":" + PullSDkErrorInfos.GetInfo(num3) + "\r\n");
							DevLogServer.m_error++;
						}
						else
						{
							DevLogServer.sb.Append(closeDevControl.DoorName + ":" + ShowMsgInfos.GetInfo("SIsSuccess", "成功") + "\r\n");
							SysLogServer.WriteLog("CloseDoor " + closeDevControl.DoorName + ": success！ \r\n");
							DevLogServer.m_right++;
						}
					}
					if (num == 1)
					{
						DevLogServer.FinishOpenOrClose();
						SysLogServer.WriteLog("CloseDoor isfirstFinish  \r\n", true);
					}
					else if (closeDevControl == null)
					{
						DevLogServer.OnShowProgress();
						DevLogServer.OnShowInfo();
						if (num2 == 1)
						{
							return false;
						}
						Thread.Sleep(500);
					}
				}
				else if (DevLogServer.m_closeDevControl.Count == 0)
				{
					DevLogServer.OnShowProgress();
					DevLogServer.OnShowInfo();
					Thread.Sleep(500);
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevLogServer CloseDoor function :" + ex.Message + " " + ex.Source);
			}
			return true;
		}

		static DevLogServer()
		{
			DevLogServer.ShowInfoEvent = null;
			DevLogServer.ShowProgressEvent = null;
			DevLogServer.FinishEvent = null;
			DevLogServer.thread = null;
			DevLogServer.m_isLock = false;
			DevLogServer.machinesbll = new MachinesBll(MainForm._ia);
			DevLogServer.doorBll = new AccDoorBll(MainForm._ia);
			DevLogServer.helper = new DeviceHelperEx();
			DevLogServer.DeleteDoorEvent = null;
			DevLogServer.m_reConnect = false;
			DevLogServer.m_lookFinish = false;
			DevLogServer.m_openType = -1;
			DevLogServer.m_closeType = -1;
			DevLogServer.m_right = 0;
			DevLogServer.m_error = 0;
			DevLogServer.m_allCount = 0;
			DevLogServer.m_startOpenOrCloseDoor = false;
			DevLogServer.m_setFinish = false;
			DevLogServer.m_isOpenOrCloseFinish = false;
			DevLogServer.m_openDevControl = new List<DevControl>();
			DevLogServer.m_closeDevControl = new List<DevControl>();
			DevLogServer.m_finishDevControl = new List<DevControl>();
			DevLogServer.is_OnAarrm = false;
			DevLogServer.m_lockOpenDev = false;
			DevLogServer.m_lockCloseDev = false;
			DevLogServer.sb = new StringBuilder();
		}
	}
}
