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
using System.Threading;
using System.Timers;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.device
{
	public class RTEventProcessor
	{
		private static object auxThreadLock = new object();

		private static object doorThreadLock = new object();

		private static object userThreadLock = new object();

		private static Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_InAuxAddress;

		private static Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_OutAuxAddress;

		private static Dictionary<int, Dictionary<int, AccDoor>> dicDevIdDoorNo_Door;

		private static Dictionary<string, int> dicPin_UserId;

		private static System.Timers.Timer tmrQueue;

		private static object objEventQueueLock = new object();

		private static object objSaveEventQueueLock = new object();

		private static List<AccMonitorLog> lstRTEvent = new List<AccMonitorLog>();

		private static List<CheckInOut> lstCheckInOut = new List<CheckInOut>();

		private static AccMonitorLogBll rtlogBll;

		private static CheckInOutBll attlogBll;

		private static object objTimerLock = new object();

		private static Thread SaveThread;

		private static Thread AlarmThread;

		public static void LoadAuxAddressInfo(bool ForceToLoadFromDB = false)
		{
			if (Monitor.TryEnter(RTEventProcessor.auxThreadLock))
			{
				try
				{
					if (ForceToLoadFromDB || RTEventProcessor.Dev_InAuxAddress == null || RTEventProcessor.Dev_OutAuxAddress == null)
					{
						RTEventProcessor.Dev_InAuxAddress = new Dictionary<int, Dictionary<int, AccAuxiliary>>();
						RTEventProcessor.Dev_OutAuxAddress = new Dictionary<int, Dictionary<int, AccAuxiliary>>();
						AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
						List<AccAuxiliary> modelList = accAuxiliaryBll.GetModelList("");
						RTEventProcessor.Dev_InAuxAddress.Clear();
						RTEventProcessor.Dev_OutAuxAddress.Clear();
						for (int i = 0; i < modelList.Count; i++)
						{
							switch (modelList[i].AuxState)
							{
							case AccAuxiliary.AccAuxiliaryState.In:
								if (RTEventProcessor.Dev_InAuxAddress.ContainsKey(modelList[i].DeviceId))
								{
									Dictionary<int, AccAuxiliary> dictionary = RTEventProcessor.Dev_InAuxAddress[modelList[i].DeviceId];
									if (!dictionary.ContainsKey(modelList[i].AuxNo))
									{
										dictionary.Add(modelList[i].AuxNo, modelList[i]);
									}
								}
								else
								{
									Dictionary<int, AccAuxiliary> dictionary = new Dictionary<int, AccAuxiliary>();
									dictionary.Add(modelList[i].AuxNo, modelList[i]);
									RTEventProcessor.Dev_InAuxAddress.Add(modelList[i].DeviceId, dictionary);
								}
								break;
							case AccAuxiliary.AccAuxiliaryState.Out:
								if (RTEventProcessor.Dev_OutAuxAddress.ContainsKey(modelList[i].DeviceId))
								{
									Dictionary<int, AccAuxiliary> dictionary = RTEventProcessor.Dev_OutAuxAddress[modelList[i].DeviceId];
									if (!dictionary.ContainsKey(modelList[i].AuxNo))
									{
										dictionary.Add(modelList[i].AuxNo, modelList[i]);
									}
								}
								else
								{
									Dictionary<int, AccAuxiliary> dictionary = new Dictionary<int, AccAuxiliary>();
									dictionary.Add(modelList[i].AuxNo, modelList[i]);
									RTEventProcessor.Dev_OutAuxAddress.Add(modelList[i].DeviceId, dictionary);
								}
								break;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					Monitor.Exit(RTEventProcessor.auxThreadLock);
				}
			}
		}

		public static void LoadDoorInfo(bool ForceToLoadFromDB = false)
		{
			if (Monitor.TryEnter(RTEventProcessor.doorThreadLock))
			{
				try
				{
					if (ForceToLoadFromDB || RTEventProcessor.dicDevIdDoorNo_Door == null)
					{
						AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
						List<AccDoor> modelList = accDoorBll.GetModelList("");
						RTEventProcessor.dicDevIdDoorNo_Door = new Dictionary<int, Dictionary<int, AccDoor>>();
						RTEventProcessor.dicDevIdDoorNo_Door.Clear();
						for (int i = 0; i < modelList.Count; i++)
						{
							if (RTEventProcessor.dicDevIdDoorNo_Door.ContainsKey(modelList[i].device_id))
							{
								Dictionary<int, AccDoor> dictionary = RTEventProcessor.dicDevIdDoorNo_Door[modelList[i].device_id];
								if (!dictionary.ContainsKey(modelList[i].door_no))
								{
									dictionary.Add(modelList[i].door_no, modelList[i]);
								}
							}
							else
							{
								Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
								dictionary.Add(modelList[i].door_no, modelList[i]);
								RTEventProcessor.dicDevIdDoorNo_Door.Add(modelList[i].device_id, dictionary);
							}
						}
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					Monitor.Exit(RTEventProcessor.doorThreadLock);
				}
			}
		}

		public static void LoadUserDic(bool ForceToLoadFromDB = false)
		{
			if (Monitor.TryEnter(RTEventProcessor.userThreadLock))
			{
				try
				{
					if (ForceToLoadFromDB || RTEventProcessor.dicPin_UserId == null)
					{
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						RTEventProcessor.dicPin_UserId = userInfoBll.GetBadgenumber_UserIdDic("");
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					Monitor.Exit(RTEventProcessor.userThreadLock);
				}
			}
		}

		public static void LoadDictionary(bool ForceToLoadFromDB = false)
		{
			RTEventProcessor.LoadDoorInfo(ForceToLoadFromDB);
			RTEventProcessor.LoadAuxAddressInfo(ForceToLoadFromDB);
			RTEventProcessor.LoadUserDic(ForceToLoadFromDB);
		}

		public static void devSever_RTEvent(DeviceServerBll devServer, ObjRTLogInfo RTLog)
		{
			if (devServer.IsNeedListen)
			{
				ThreadPool.QueueUserWorkItem(delegate
				{
					RTEventProcessor.CheckAlarm(devServer, RTLog);
					devServer.OnRTLogEvent(RTLog);
					RTEventProcessor.SaveRecord(devServer, RTLog);
				});
			}
		}

		public static void CheckAlarm(DeviceServerBll devServer, ObjRTLogInfo RTLog)
		{
			try
			{
				int num;
				if ((RTLog.EType < (EventType)100 || RTLog.EType >= EventType.Type200) && RTLog.EType != EventType.Type28 && RTLog.EType != EventType.DoorUnexpectedOpened && RTLog.EType != EventType.DuressAlarm && RTLog.EType != EventType.AntibackAlarm && RTLog.EType != EventType.DeviceBroken && RTLog.EType != EventType.VerifyTimesOut)
				{
					num = ((RTLog.EType == EventType.Type51 && AccCommon.CodeVersion == CodeVersionType.JapanAF) ? 1 : 0);
					goto IL_007c;
				}
				num = 1;
				goto IL_007c;
				IL_007c:
				if (num != 0)
				{
					devServer.IsAlarm[int.Parse(RTLog.DoorID) - 1] = true;
					RTEventProcessor.StartAlarmThread();
				}
				if (devServer.DevInfo.simpleEventType == 1)
				{
					int num2 = (int)(RTLog.EType + 1000);
					if (Enum.IsDefined(typeof(EventType), num2))
					{
						RTLog.EType = (EventType)num2;
					}
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("Exception in RTEVentProccessor.CheckAlarm: " + ex.Message, true);
			}
		}

		public static void SaveRecord(DeviceServerBll devServer, ObjRTLogInfo RTLog)
		{
			if (RTLog != null)
			{
				try
				{
					AccDoor accDoor = null;
					CheckInOut checkInOut = null;
					int num = (int)RTLog.EType;
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
						accMonitorLog.device_id = RTLog.DevID;
						accMonitorLog.card_no = RTLog.CardNo;
						accMonitorLog.pin = RTLog.Pin;
						accMonitorLog.verified = int.Parse(RTLog.VerifyType);
						accMonitorLog.event_type = (int)RTLog.EType;
						accMonitorLog.time = RTLog.Date;
						if (!string.IsNullOrEmpty(RTLog.InOutStatus))
						{
							accMonitorLog.state = (int.Parse(RTLog.InOutStatus) & 0xF);
						}
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						int userID = userInfoBll.GetUserID(RTLog.Pin);
						UserInfo model = userInfoBll.GetModel(userID);
						if (model != null)
						{
							accMonitorLog.username = model.Name;
							accMonitorLog.userlastname = model.LastName;
						}
						MachinesBll machinesBll = new MachinesBll(MainForm._ia);
						model2 = machinesBll.GetModel(accMonitorLog.device_id);
						if (model2 != null)
						{
							accMonitorLog.device_name = model2.MachineAlias;
						}
						int num2 = num;
						if (num2 != 6)
						{
							if ((uint)(num2 - 12) > 1u)
							{
								if ((uint)(num2 - 220) <= 1u)
								{
									accMonitorLog.event_point_type = 1;
									accMonitorLog.event_point_id = int.Parse(RTLog.DoorID);
									accMonitorLog.event_point_name = accMonitorLog.device_name + "-" + accMonitorLog.event_point_id;
									if (model2 != null && RTEventProcessor.Dev_InAuxAddress.ContainsKey(model2.ID))
									{
										Dictionary<int, AccAuxiliary> dictionary = RTEventProcessor.Dev_InAuxAddress[model2.ID];
										if (dictionary.ContainsKey(accMonitorLog.event_point_id))
										{
											accMonitorLog.event_point_name = dictionary[accMonitorLog.event_point_id].AuxName;
										}
									}
								}
								else
								{
									int event_point_id = default(int);
									if (RTLog.DoorID != null && int.TryParse(RTLog.DoorID, out event_point_id))
									{
										accMonitorLog.event_point_type = 0;
										accMonitorLog.event_point_id = event_point_id;
									}
									accMonitorLog.event_point_name = accMonitorLog.device_name + "-" + accMonitorLog.event_point_id;
									if (model2 != null && RTEventProcessor.dicDevIdDoorNo_Door.ContainsKey(model2.ID))
									{
										Dictionary<int, AccDoor> dictionary2 = RTEventProcessor.dicDevIdDoorNo_Door[model2.ID];
										foreach (KeyValuePair<int, AccDoor> item in dictionary2)
										{
											if (item.Value.door_no == accMonitorLog.event_point_id)
											{
												accDoor = item.Value;
												accMonitorLog.event_point_name = item.Value.door_name;
											}
										}
									}
								}
							}
							else
							{
								accMonitorLog.event_point_type = 2;
								accMonitorLog.event_point_id = int.Parse(RTLog.DoorID);
								accMonitorLog.event_point_name = accMonitorLog.device_name + "-" + accMonitorLog.event_point_id;
								if (model2 != null && RTEventProcessor.Dev_OutAuxAddress.ContainsKey(model2.ID))
								{
									Dictionary<int, AccAuxiliary> dictionary = RTEventProcessor.Dev_OutAuxAddress[model2.ID];
									if (dictionary.ContainsKey(accMonitorLog.event_point_id))
									{
										accMonitorLog.event_point_name = dictionary[accMonitorLog.event_point_id].AuxName;
									}
								}
							}
						}
						else
						{
							string text = PullSDKEventInfos.GetInfo(RTLog.EType);
							if (RTLog.VerifyType != null && RTLog.VerifyType.Trim() != "" && Enum.IsDefined(typeof(EventType), RTLog.VerifyType.Trim()))
							{
								EventType type = (EventType)Enum.Parse(typeof(EventType), RTLog.VerifyType);
								text = text + "[" + PullSDKEventInfos.GetInfo(type) + "]";
							}
							int id = default(int);
							if (RTLog.CardNo != null && int.TryParse(RTLog.CardNo, out id))
							{
								try
								{
									AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
									AccLinkAgeIo model3 = accLinkAgeIoBll.GetModel(id);
									if (model3 != null)
									{
										text = text + "(" + model3.linkage_name + ")";
									}
								}
								catch (Exception)
								{
								}
							}
							accMonitorLog.description = text;
							accMonitorLog.event_point_type = 0;
							accMonitorLog.event_point_id = int.Parse(RTLog.DoorID);
							if (accMonitorLog.verified == 220 || accMonitorLog.verified == 221)
							{
								accMonitorLog.event_point_type = 1;
								accMonitorLog.event_point_id = int.Parse(RTLog.DoorID);
							}
							accMonitorLog.verified = 200;
							accMonitorLog.card_no = string.Empty;
							accMonitorLog.event_point_name = accMonitorLog.device_name + "-" + accMonitorLog.event_point_id;
							if (model2 != null && RTEventProcessor.dicDevIdDoorNo_Door.ContainsKey(model2.ID))
							{
								Dictionary<int, AccDoor> dictionary2 = RTEventProcessor.dicDevIdDoorNo_Door[model2.ID];
								foreach (KeyValuePair<int, AccDoor> item2 in dictionary2)
								{
									if (item2.Value.door_no == accMonitorLog.event_point_id)
									{
										accDoor = item2.Value;
										accMonitorLog.event_point_name = item2.Value.door_name;
									}
								}
							}
						}
						if (accDoor?.is_att ?? false)
						{
							if ((num < 0 || num > 3) && (num < 14 || num > 19) && (num < 21 || num > 23) && num != 26 && num != 32 && num != 35 && num != 207 && num != 208)
							{
								num3 = ((num == 214) ? 1 : 0);
								goto IL_066f;
							}
							num3 = 1;
							goto IL_066f;
						}
						goto IL_07d8;
					}
					goto end_IL_0012;
					IL_066f:
					if (num3 != 0 && RTLog.Pin != null && RTEventProcessor.dicPin_UserId != null && RTEventProcessor.dicPin_UserId.ContainsKey(RTLog.Pin))
					{
						checkInOut = new CheckInOut();
						checkInOut.VERIFYCODE = accMonitorLog.verified;
						checkInOut.CHECKTIME = accMonitorLog.time;
						CheckInOut checkInOut2 = checkInOut;
						int num4 = model2.MachineNumber;
						checkInOut2.SENSORID = num4.ToString();
						checkInOut.sn = model2.sn;
						checkInOut.CHECKTYPE = "I";
						byte b = (byte)((int.Parse(RTLog.InOutStatus) & 0xF0) >> 4);
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
						num4 = (byte.Parse(RTLog.InOutStatus) & 0xF);
						RTLog.InOutStatus = num4.ToString();
						int.TryParse(RTLog.Pin, out int pin);
						checkInOut.Pin = pin;
						checkInOut.USERID = RTEventProcessor.dicPin_UserId[RTLog.Pin];
					}
					goto IL_07d8;
					IL_07d8:
					lock (RTEventProcessor.objEventQueueLock)
					{
						RTEventProcessor.lstRTEvent.Add(accMonitorLog);
						if (checkInOut != null)
						{
							RTEventProcessor.lstCheckInOut.Add(checkInOut);
						}
					}
					ThreadPool.QueueUserWorkItem(delegate
					{
						try
						{
							RTEventProcessor.StarSaveQueueTimer();
						}
						catch (Exception ex3)
						{
							SysLogServer.WriteLog("Exception in RTEventProccessor.SaveRTLogQueue: " + ex3.Message, true);
						}
					});
					end_IL_0012:;
				}
				catch (Exception ex2)
				{
					SysLogServer.WriteLog("Exception in RTEventProccessor.SaveRecord: " + ex2.Message, true);
				}
			}
		}

		private static void StarSaveQueueTimer()
		{
			if (RTEventProcessor.SaveThread == null || (RTEventProcessor.SaveThread.ThreadState != 0 && RTEventProcessor.SaveThread.ThreadState != ThreadState.WaitSleepJoin))
			{
				RTEventProcessor.SaveThread = new Thread((ParameterizedThreadStart)delegate
				{
					try
					{
						LogServer.Log("SaveRTQueueThread has started.", true);
						while (!MonitorWatchdog.ShouldStop)
						{
							RTEventProcessor.SaveEventQueue(null);
							Thread.Sleep(1000);
						}
						while (true)
						{
							lock (RTEventProcessor.objEventQueueLock)
							{
								if (RTEventProcessor.lstCheckInOut.Count > 0 || RTEventProcessor.lstRTEvent.Count > 0)
								{
									goto end_IL_003b;
								}
								return;
								end_IL_003b:;
							}
							RTEventProcessor.SaveEventQueue(null);
							Thread.Sleep(1000);
						}
					}
					catch (Exception ex)
					{
						LogServer.Log("SaveRTQueueThread Exception: " + ex.Message, true);
					}
					finally
					{
						LogServer.Log("SaveRTQueueThread stoped.", true);
					}
				});
				RTEventProcessor.SaveThread.Name = "SaveRTQueueThread";
				RTEventProcessor.SaveThread.Start();
			}
		}

		private static void SaveEventQueue(object param)
		{
			if (Monitor.TryEnter(RTEventProcessor.objSaveEventQueueLock))
			{
				try
				{
					List<AccMonitorLog> list = default(List<AccMonitorLog>);
					List<CheckInOut> list2 = default(List<CheckInOut>);
					lock (RTEventProcessor.objEventQueueLock)
					{
						list = RTEventProcessor.lstRTEvent;
						RTEventProcessor.lstRTEvent = new List<AccMonitorLog>();
						list2 = RTEventProcessor.lstCheckInOut;
						RTEventProcessor.lstCheckInOut = new List<CheckInOut>();
					}
					RTEventProcessor.rtlogBll = (RTEventProcessor.rtlogBll ?? new AccMonitorLogBll(MainForm._ia));
					RTEventProcessor.attlogBll = (RTEventProcessor.attlogBll ?? new CheckInOutBll(MainForm._ia));
					SysLogServer.WriteLog("RTEventProcessor: SaveEventQueue", true);
					RTEventProcessor.rtlogBll.Add(list);
					RTEventProcessor.attlogBll.Add(list2);
				}
				catch (Exception ex)
				{
					LogServer.Log("RTEventProcessor.tmrQueue.Elapsed Exception: " + ex.Message, true);
				}
				finally
				{
					Monitor.Exit(RTEventProcessor.objSaveEventQueueLock);
				}
			}
			else
			{
				lock (RTEventProcessor.objEventQueueLock)
				{
					LogServer.Log($"RTEventProcessor.tmrQueue.Elapsed Lock Failed. RTLogCount: {RTEventProcessor.lstRTEvent.Count}\tAttLogCount: {RTEventProcessor.lstCheckInOut.Count}", true);
				}
			}
		}

		public static void StartAlarmThread()
		{
			if (MonitorWatchdog.AlarmFile != null && File.Exists(MonitorWatchdog.AlarmFile) && (RTEventProcessor.AlarmThread == null || RTEventProcessor.AlarmThread.ThreadState != 0))
			{
				RTEventProcessor.AlarmThread = new Thread((ThreadStart)delegate
				{
					bool flag = false;
					SoundPlayer soundPlayer = new SoundPlayer(MonitorWatchdog.AlarmFile);
					do
					{
						flag = false;
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
							if (!soundPlayer.IsLoadCompleted)
							{
								soundPlayer.PlayLooping();
							}
						}
						else
						{
							soundPlayer.Stop();
						}
					}
					while (flag);
				});
				RTEventProcessor.AlarmThread.Start();
			}
		}
	}
}
