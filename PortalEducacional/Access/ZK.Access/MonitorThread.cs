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
using System.Threading;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class MonitorThread
	{
		private int m_index = 0;

		private List<DeviceServerBll> m_list = new List<DeviceServerBll>();

		private bool m_isLock = false;

		private bool m_requestLock = false;

		private bool m_isFirst = true;

		private Dictionary<int, string> inAddressList = InAddressInfo.GetDic();

		private Dictionary<int, string> outAddressList = OutAddressInfo.GetDic();

		private Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_InAuxAddress;

		private Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_OutAuxAddress;

		private Dictionary<int, Dictionary<int, AccDoor>> dicDevIdDoorNo_Door;

		private Thread thread = null;

		private bool m_isRun = false;

		private bool m_isStop = false;

		private Dictionary<int, string> EventInfoList = PullSDKEventInfos.GetDic();

		public int Index => this.m_index;

		public List<DeviceServerBll> DevList => this.m_list;

		public bool IsRun => this.m_isRun;

		public MonitorThread(int index)
		{
			LogServer.Log("MonitorThread initiated");
			this.m_index = index;
			this.thread = new Thread(this.Start);
			this.LoadAuxAddressInfo();
			this.LoadDoorInfo();
		}

		private void LoadAuxAddressInfo()
		{
			this.Dev_InAuxAddress = new Dictionary<int, Dictionary<int, AccAuxiliary>>();
			this.Dev_OutAuxAddress = new Dictionary<int, Dictionary<int, AccAuxiliary>>();
			AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
			List<AccAuxiliary> modelList = accAuxiliaryBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					switch (modelList[i].AuxState)
					{
					case AccAuxiliary.AccAuxiliaryState.In:
						if (this.Dev_InAuxAddress.ContainsKey(modelList[i].DeviceId))
						{
							Dictionary<int, AccAuxiliary> dictionary = this.Dev_InAuxAddress[modelList[i].DeviceId];
							if (!dictionary.ContainsKey(modelList[i].AuxNo))
							{
								dictionary.Add(modelList[i].AuxNo, modelList[i]);
							}
						}
						else
						{
							Dictionary<int, AccAuxiliary> dictionary = new Dictionary<int, AccAuxiliary>();
							dictionary.Add(modelList[i].AuxNo, modelList[i]);
							this.Dev_InAuxAddress.Add(modelList[i].DeviceId, dictionary);
						}
						break;
					case AccAuxiliary.AccAuxiliaryState.Out:
						if (this.Dev_OutAuxAddress.ContainsKey(modelList[i].DeviceId))
						{
							Dictionary<int, AccAuxiliary> dictionary = this.Dev_OutAuxAddress[modelList[i].DeviceId];
							if (!dictionary.ContainsKey(modelList[i].AuxNo))
							{
								dictionary.Add(modelList[i].AuxNo, modelList[i]);
							}
						}
						else
						{
							Dictionary<int, AccAuxiliary> dictionary = new Dictionary<int, AccAuxiliary>();
							dictionary.Add(modelList[i].AuxNo, modelList[i]);
							this.Dev_OutAuxAddress.Add(modelList[i].DeviceId, dictionary);
						}
						break;
					}
				}
			}
		}

		private void LoadDoorInfo()
		{
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			this.dicDevIdDoorNo_Door = new Dictionary<int, Dictionary<int, AccDoor>>();
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (this.dicDevIdDoorNo_Door.ContainsKey(modelList[i].device_id))
					{
						Dictionary<int, AccDoor> dictionary = this.dicDevIdDoorNo_Door[modelList[i].device_id];
						if (!dictionary.ContainsKey(modelList[i].door_no))
						{
							dictionary.Add(modelList[i].door_no, modelList[i]);
						}
					}
					else
					{
						Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
						dictionary.Add(modelList[i].door_no, modelList[i]);
						this.dicDevIdDoorNo_Door.Add(modelList[i].device_id, dictionary);
					}
				}
			}
		}

		private void Instance_DeleteServerEvent(object sender, EventArgs e)
		{
			this.m_requestLock = true;
			this.m_isLock = true;
			if (sender != null)
			{
				DeviceServerBll deviceServerBll = sender as DeviceServerBll;
				if (deviceServerBll != null)
				{
					int num = 0;
					while (num < this.m_list.Count)
					{
						if (!this.m_list[num].IsDel || this.m_list[num] != deviceServerBll)
						{
							num++;
							continue;
						}
						this.m_list.RemoveAt(num);
						break;
					}
				}
			}
			this.m_isLock = false;
			this.m_requestLock = false;
		}

		public void RequestLock()
		{
			this.m_requestLock = true;
			int num = 0;
			while (this.m_isLock)
			{
				Thread.Sleep(DevLogServer.SleepTime / 2);
				num++;
				if (num > 3)
				{
					break;
				}
			}
		}

		public void UnRequestLock()
		{
			this.m_requestLock = false;
		}

		public void OnStart()
		{
			SysLogServer.WriteLog("Thread:index " + this.Index + " OnStart ", true);
			if (!this.m_isRun)
			{
				this.m_isStop = false;
				this.m_isRun = true;
				this.m_isFirst = true;
				if (this.thread != null)
				{
					try
					{
						this.thread.Start();
					}
					catch
					{
						this.thread = null;
					}
				}
				else
				{
					this.thread = new Thread(this.Start);
					try
					{
						this.thread.Start();
					}
					catch
					{
						this.thread = null;
					}
				}
			}
			SysLogServer.WriteLog("Thread:index " + this.Index + " OnStart Finish ", true);
		}

		public void OnStop()
		{
			SysLogServer.WriteLog("Thread:index " + this.Index + "  stop", true);
			int num = 0;
			while (this.m_isRun || this.thread != null)
			{
				this.m_requestLock = true;
				this.m_isStop = true;
				Thread.Sleep(DevLogServer.SleepTime);
				num++;
				if (num > 2)
				{
					try
					{
						this.thread.Abort();
					}
					catch
					{
					}
					break;
				}
			}
			SysLogServer.WriteLog("Thread:index " + this.Index + " stop finish ", true);
		}

		private void OpenOrCloseDoor()
		{
			while (!DevLogServer.IsCmdFinish)
			{
				if (DevLogServer.m_openDevControl.Count > 0)
				{
					if (!DevLogServer.OpenDoor(this.Index))
					{
						break;
					}
				}
				else if (DevLogServer.m_closeDevControl.Count > 0 && !DevLogServer.CloseDoor(this.Index))
				{
					break;
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

		private void Start()
		{
			SysLogServer.WriteLog("hread:index " + this.Index + " run...", true);
			this.m_isRun = true;
			while (!DevLogServer.IsStop && !this.m_isStop && this.m_list.Count > 0)
			{
				this.OpenOrCloseDoor();
				if (!this.m_isLock && !this.m_requestLock)
				{
					try
					{
						this.m_isLock = true;
						List<DeviceServerBll> list = new List<DeviceServerBll>();
						int num = 0;
						while (num < this.m_list.Count)
						{
							if (!this.m_requestLock)
							{
								DeviceServerBll deviceServerBll = this.m_list[num];
								if (deviceServerBll != null && !deviceServerBll.IsDel)
								{
									deviceServerBll.Index = this.m_index;
									deviceServerBll.IsRun = true;
									if (deviceServerBll.IsHaveListener && deviceServerBll.IsNeedListen)
									{
										if (!deviceServerBll.IsConnected)
										{
											deviceServerBll.Connect(3000);
										}
										if (deviceServerBll.IsConnected && !deviceServerBll.IsReConnected)
										{
											if (deviceServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
											{
												deviceServerBll.STD_StartMonitor();
											}
											list.Add(deviceServerBll);
											deviceServerBll.LastConnectTime = DateTime.Now;
										}
										else
										{
											if (deviceServerBll.IsReConnected)
											{
												deviceServerBll.Disconnect();
												if (deviceServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
												{
													deviceServerBll.STD_StopMonitor();
												}
												if (deviceServerBll.DevInfo.ConnectType == 0)
												{
													Program.KillPlrscagent();
												}
											}
											if (deviceServerBll.ReConectCount < DevLogServer.ReConnectCount)
											{
												int num2 = deviceServerBll.Connect(3000);
												if (num2 >= 0)
												{
													if (deviceServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
													{
														deviceServerBll.STD_StartMonitor();
													}
													list.Add(deviceServerBll);
													deviceServerBll.LastConnectTime = DateTime.Now;
												}
												else
												{
													SysLogServer.WriteLog("thread:index " + this.Index + " device ReConnect " + deviceServerBll.DevInfo.ID + "_" + deviceServerBll.DevInfo.MachineAlias + " ReConectCount:" + deviceServerBll.ReConectCount + " ret " + PullSDkErrorInfos.GetInfo(num2));
												}
											}
											else
											{
												deviceServerBll.Index = -1;
												this.m_list.RemoveAt(num);
												SysLogServer.WriteLog("thread:index " + this.Index + " device Remove " + deviceServerBll.DevInfo.ID + "_" + deviceServerBll.DevInfo.MachineAlias + " ReConectCount:" + deviceServerBll.ReConectCount);
											}
										}
									}
									else
									{
										deviceServerBll.Index = -1;
										this.m_list.RemoveAt(num);
										SysLogServer.WriteLog("thread:index " + this.Index + " device Remove " + deviceServerBll.DevInfo.ID + "_" + deviceServerBll.DevInfo.MachineAlias + " No listen :");
									}
								}
								else
								{
									deviceServerBll.Index = -1;
									this.m_list.RemoveAt(num);
									SysLogServer.WriteLog("thread:index " + this.Index + " device Remove " + deviceServerBll.DevInfo.ID + "_" + deviceServerBll.DevInfo.MachineAlias + " isDel :");
								}
								num++;
								continue;
							}
							SysLogServer.WriteLog("thread:index " + this.Index + " requestLock");
							break;
						}
						if (this.m_list.Count > 0 && !this.m_requestLock && list.Count > 0)
						{
							this.GetRecord(list);
						}
						Thread.Sleep(DevLogServer.SleepTime);
					}
					catch (Exception ex)
					{
						SysLogServer.WriteLog("thread:index " + this.Index + " Exception  " + ex.Message, true);
					}
					this.m_isLock = false;
				}
				else
				{
					Thread.Sleep(DevLogServer.SleepTime);
				}
			}
			this.thread = null;
			this.m_isRun = false;
			SysLogServer.WriteLog("thread:index " + this.Index + " run finish  ", true);
		}

		private void GetRecord(List<DeviceServerBll> list)
		{
			if (list.Count > 0)
			{
				int num = 0;
				for (num = 0; num < list.Count; num++)
				{
					DeviceServerBll deviceServerBll = list[num];
					if (deviceServerBll != null && !deviceServerBll.IsLock)
					{
						int id = 0;
						if (deviceServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
						{
							deviceServerBll.STD_StartMonitor();
							if (deviceServerBll.IsFirst)
							{
								deviceServerBll.GetRTLogs(ref id);
								deviceServerBll.IsFirst = false;
								continue;
							}
						}
						List<ObjRTLogInfo> rTLogs = deviceServerBll.GetRTLogs(ref id);
						if (rTLogs != null && rTLogs.Count > 0)
						{
							deviceServerBll.MissCount = 0;
							deviceServerBll.IsReConnected = false;
							if (!this.m_isFirst && !deviceServerBll.IsFirst)
							{
								for (int num2 = 0; num2 < rTLogs.Count; num2++)
								{
									int num3;
									if ((rTLogs[num2].EType < (EventType)100 || rTLogs[num2].EType >= EventType.Type200) && rTLogs[num2].EType != EventType.Type28 && rTLogs[num2].EType != EventType.DoorUnexpectedOpened && rTLogs[num2].EType != EventType.DuressAlarm && rTLogs[num2].EType != EventType.AntibackAlarm && rTLogs[num2].EType != EventType.DeviceBroken && rTLogs[num2].EType != EventType.VerifyTimesOut)
									{
										num3 = ((rTLogs[num2].EType == EventType.Type51 && AccCommon.CodeVersion == CodeVersionType.JapanAF) ? 1 : 0);
										goto IL_01a2;
									}
									num3 = 1;
									goto IL_01a2;
									IL_01a2:
									if (num3 != 0)
									{
										deviceServerBll.IsAlarm[int.Parse(rTLogs[num2].DoorID) - 1] = true;
									}
									if (deviceServerBll.DevInfo.simpleEventType == 1)
									{
										int num4 = (int)(rTLogs[num2].EType + 1000);
										if (this.EventInfoList.ContainsKey(num4))
										{
											rTLogs[num2].EType = (EventType)num4;
										}
									}
									SDKType devSDKType = deviceServerBll.DevInfo.DevSDKType;
									if (devSDKType == SDKType.StandaloneSDK)
									{
										if (rTLogs[num2].EType < EventType.Type0 || rTLogs[num2].EType == EventType.Type255)
										{
											deviceServerBll.OnRTLogEvent(rTLogs[num2]);
										}
									}
									else
									{
										deviceServerBll.OnRTLogEvent(rTLogs[num2]);
									}
								}
								this.SaveRecord(rTLogs, deviceServerBll);
							}
							else
							{
								if (deviceServerBll.DevInfo.simpleEventType == 1)
								{
									for (int i = 0; i < rTLogs.Count; i++)
									{
										int num5 = (int)(rTLogs[i].EType + 1000);
										if (this.EventInfoList.ContainsKey(num5))
										{
											rTLogs[i].EType = (EventType)num5;
										}
									}
								}
								this.SaveRecord(rTLogs, deviceServerBll);
								while (true)
								{
									rTLogs = deviceServerBll.GetRTLogs(ref id);
									if (rTLogs != null && rTLogs.Count > 0)
									{
										if (rTLogs[0].EType == EventType.Type255)
										{
											for (int num6 = 0; num6 < rTLogs.Count; num6++)
											{
												int num7;
												if ((rTLogs[num6].EType < (EventType)100 || rTLogs[num6].EType >= EventType.Type200) && rTLogs[num6].EType != EventType.Type28 && rTLogs[num6].EType != EventType.DoorUnexpectedOpened && rTLogs[num6].EType != EventType.DuressAlarm && rTLogs[num6].EType != EventType.AntibackAlarm && rTLogs[num6].EType != EventType.DeviceBroken && rTLogs[num6].EType != EventType.VerifyTimesOut)
												{
													num7 = ((rTLogs[num6].EType == EventType.Type51 && AccCommon.CodeVersion == CodeVersionType.JapanAF) ? 1 : 0);
													goto IL_0442;
												}
												num7 = 1;
												goto IL_0442;
												IL_0442:
												if (num7 != 0)
												{
													deviceServerBll.IsAlarm[int.Parse(rTLogs[num6].DoorID) - 1] = true;
												}
												if (deviceServerBll.DevInfo.simpleEventType == 1)
												{
													int num8 = (int)(rTLogs[num6].EType + 1000);
													if (this.EventInfoList.ContainsKey(num8))
													{
														rTLogs[num6].EType = (EventType)num8;
													}
												}
												SDKType devSDKType2 = deviceServerBll.DevInfo.DevSDKType;
												if (devSDKType2 == SDKType.StandaloneSDK)
												{
													if (rTLogs[num6].EType < EventType.Type0 || rTLogs[num6].EType == EventType.Type255)
													{
														deviceServerBll.OnRTLogEvent(rTLogs[num6]);
													}
												}
												else
												{
													deviceServerBll.OnRTLogEvent(rTLogs[num6]);
												}
											}
											this.SaveRecord(rTLogs, deviceServerBll);
											break;
										}
										continue;
									}
									this.SaveRecord(rTLogs, deviceServerBll);
									break;
								}
								SysLogServer.WriteLog("thread:index  " + this.Index + " device " + deviceServerBll.DevInfo.ID + "_" + deviceServerBll.DevInfo.MachineAlias + " isFirst ");
							}
							deviceServerBll.IsFirst = false;
							this.m_isFirst = false;
						}
						else
						{
							deviceServerBll.MissCount++;
							if (deviceServerBll.MissCount > 5)
							{
								deviceServerBll.MissCount = 0;
								SysLogServer.WriteLog("thread:index " + this.Index + " device " + deviceServerBll.DevInfo.ID + "_" + deviceServerBll.DevInfo.MachineAlias + " get recordCount " + 0 + " ret " + PullSDkErrorInfos.GetInfo(id));
								deviceServerBll.IsReConnected = true;
								ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
								objRTLogInfo.DevID = deviceServerBll.DevInfo.ID;
								objRTLogInfo.EType = EventType.Type300;
								objRTLogInfo.StatusInfo = PullSDkErrorInfos.GetInfo(id);
								deviceServerBll.OnRTLogEvent(objRTLogInfo);
							}
						}
						deviceServerBll.LastListienDate = DateTime.Now;
					}
				}
			}
		}

		private void SaveRecord(List<ObjRTLogInfo> logs, DeviceServerBll devServer)
		{
			if (logs != null)
			{
				try
				{
					AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
					CheckInOutBll checkInOutBll = new CheckInOutBll(MainForm._ia);
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					for (int i = 0; i < logs.Count; i++)
					{
						int num = (int)logs[i].EType;
						if (num >= 1000 && num < 100000)
						{
							num -= 1000;
						}
						AccMonitorLog accMonitorLog;
						Machines model2;
						int num3;
						if (num >= 0 && num != 255)
						{
							accMonitorLog = new AccMonitorLog();
							accMonitorLog.device_id = logs[i].DevID;
							accMonitorLog.card_no = logs[i].CardNo;
							accMonitorLog.pin = logs[i].Pin;
							accMonitorLog.verified = int.Parse(logs[i].VerifyType);
							accMonitorLog.event_type = (int)logs[i].EType;
							if (!string.IsNullOrEmpty(logs[i].InOutStatus))
							{
								accMonitorLog.state = (int.Parse(logs[i].InOutStatus) & 0xF);
							}
							accMonitorLog.time = logs[i].Date;
							if (accMonitorLog.event_type == 220 || accMonitorLog.event_type == 221)
							{
								accMonitorLog.event_point_type = 1;
								accMonitorLog.event_point_id = int.Parse(logs[i].DoorID);
							}
							else if (accMonitorLog.event_type == 12 || accMonitorLog.event_type == 13)
							{
								accMonitorLog.event_point_type = 2;
								accMonitorLog.event_point_id = int.Parse(logs[i].DoorID);
							}
							else if (accMonitorLog.event_type == 6)
							{
								string text = PullSDKEventInfos.GetInfo(logs[i].EType);
								string cardNo = logs[i].CardNo;
								if (!string.IsNullOrEmpty(cardNo))
								{
									try
									{
										int id = int.Parse(cardNo);
										AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
										AccLinkAgeIo model = accLinkAgeIoBll.GetModel(id);
										if (model != null)
										{
											if (!string.IsNullOrEmpty(logs[i].VerifyType))
											{
												text = text + "[" + PullSDKEventInfos.GetInfo((EventType)Enum.Parse(typeof(EventType), logs[i].VerifyType)) + "]";
											}
											text = text + "(" + model.linkage_name + ")";
										}
									}
									catch
									{
									}
								}
								accMonitorLog.description = text;
								accMonitorLog.event_point_type = 0;
								accMonitorLog.event_point_id = int.Parse(logs[i].DoorID);
								if (accMonitorLog.verified == 220 || accMonitorLog.verified == 221)
								{
									accMonitorLog.event_point_type = 1;
									accMonitorLog.event_point_id = int.Parse(logs[i].DoorID);
								}
								accMonitorLog.verified = 200;
								accMonitorLog.card_no = string.Empty;
							}
							else if (int.Parse(logs[i].DoorID) > 0)
							{
								accMonitorLog.event_point_type = 0;
								accMonitorLog.event_point_id = int.Parse(logs[i].DoorID);
							}
							model2 = machinesBll.GetModel(accMonitorLog.device_id);
							if (model2 != null)
							{
								accMonitorLog.device_name = model2.MachineAlias;
							}
							accMonitorLog.event_point_name = accMonitorLog.device_name + "-" + accMonitorLog.event_point_id;
							switch (accMonitorLog.event_point_type)
							{
							case 1:
								if (model2 != null && this.Dev_InAuxAddress.ContainsKey(model2.ID))
								{
									Dictionary<int, AccAuxiliary> dictionary2 = this.Dev_InAuxAddress[model2.ID];
									if (dictionary2.ContainsKey(accMonitorLog.event_point_id))
									{
										accMonitorLog.event_point_name = dictionary2[accMonitorLog.event_point_id].AuxName;
									}
								}
								break;
							case 2:
								if (model2 != null && this.Dev_OutAuxAddress.ContainsKey(model2.ID))
								{
									Dictionary<int, AccAuxiliary> dictionary2 = this.Dev_OutAuxAddress[model2.ID];
									if (dictionary2.ContainsKey(accMonitorLog.event_point_id))
									{
										accMonitorLog.event_point_name = dictionary2[accMonitorLog.event_point_id].AuxName;
									}
								}
								break;
							default:
								if (model2 != null && this.dicDevIdDoorNo_Door.ContainsKey(model2.ID))
								{
									Dictionary<int, AccDoor> dictionary = this.dicDevIdDoorNo_Door[model2.ID];
									foreach (KeyValuePair<int, AccDoor> item in dictionary)
									{
										if (item.Value.door_no == accMonitorLog.event_point_id)
										{
											accMonitorLog.event_point_name = item.Value.door_name;
										}
									}
								}
								break;
							}
							int num2 = accMonitorLogBll.Add(accMonitorLog);
							if (devServer != null && devServer.DevInfo != null)
							{
								SDKType devSDKType = devServer.DevInfo.DevSDKType;
								if (devSDKType == SDKType.StandaloneSDK && num2 > 0)
								{
									devServer.OnRTLogEvent(logs[i]);
								}
							}
							List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + accMonitorLog.device_id + " and door_no=" + logs[i].DoorID);
							if (modelList != null && modelList.Count > 0 && modelList[0].is_att)
							{
								if ((logs[i].EType < EventType.Type0 || logs[i].EType > EventType.Type3) && (logs[i].EType < (EventType)14 || logs[i].EType > (EventType)19) && (logs[i].EType < EventType.Type21 || logs[i].EType > EventType.Type23) && logs[i].EType != EventType.Type26 && logs[i].EType != (EventType)32 && logs[i].EType != (EventType)35 && logs[i].EType != (EventType)203 && logs[i].EType != EventType.Type207 && logs[i].EType != (EventType)208 && logs[i].EType != (EventType)214 && logs[i].EType != EventType.Type1001 && logs[i].EType != EventType.Type1002 && logs[i].EType != EventType.Type1003 && logs[i].EType != EventType.Type1026)
								{
									num3 = ((logs[i].EType == (EventType)1000) ? 1 : 0);
									goto IL_07d9;
								}
								num3 = 1;
								goto IL_07d9;
							}
						}
						continue;
						IL_07d9:
						if (num3 != 0)
						{
							CheckInOut checkInOut = new CheckInOut();
							checkInOut.VERIFYCODE = accMonitorLog.verified;
							checkInOut.CHECKTIME = accMonitorLog.time;
							CheckInOut checkInOut2 = checkInOut;
							int num4 = model2.MachineNumber;
							checkInOut2.SENSORID = num4.ToString();
							checkInOut.sn = model2.sn;
							checkInOut.CHECKTYPE = "I";
							byte b = (byte)((int.Parse(logs[i].InOutStatus) & 0xF0) >> 4);
							if (model2.DevSDKType != SDKType.StandaloneSDK)
							{
								b = (byte)(b - 1);
							}
							string[] array = new string[6]
							{
								"I",
								"O",
								"0",
								"1",
								"i",
								"o"
							};
							if (b >= 0 && b < 6)
							{
								checkInOut.CHECKTYPE = array[b];
							}
							ObjRTLogInfo objRTLogInfo = logs[i];
							num4 = (byte.Parse(logs[i].InOutStatus) & 0xF);
							objRTLogInfo.InOutStatus = num4.ToString();
							int userID = userInfoBll.GetUserID(logs[i].Pin);
							if (userID > 0)
							{
								int.TryParse(logs[i].Pin, out int pin);
								checkInOut.Pin = pin;
								checkInOut.USERID = userID;
								checkInOutBll.Add(checkInOut);
							}
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
