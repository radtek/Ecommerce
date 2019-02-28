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
using System.Data;
using System.Text;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Data.Model.StdSDK;
using ZK.MachinesManager.UDisk;
using ZK.Utils;

namespace ZK.Access
{
	public class CommandServer
	{
		private static AccDoorBll doorbll = new AccDoorBll(MainForm._ia);

		private static Dictionary<int, AccDoor> doorlist = new Dictionary<int, AccDoor>();

		public static int PcToDevice(DeviceServerBll devServerBll, string CommandInfo)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(CommandInfo) && devServerBll != null)
			{
				string[] array = CommandInfo.Split("$".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				if (array != null && array.Length >= 2)
				{
					if (array[0].ToLower() == "set" || array[0].ToLower() == "setfirstcardstate" || array[0].ToLower() == "setmulticardstate")
					{
						array[1] = array[1].Replace("#", "\r\n");
						SysLogServer.WriteLog("set..." + array[1], true);
						result = devServerBll.SetDeviceParam(array[1]);
					}
					else
					{
						string[] array2 = array[0].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						if (array2 != null && array2.Length != 0)
						{
							array[1] = array[1].Replace("#", "\r\n");
							switch (array2[0].ToLower())
							{
							case "update":
								if (array2.Length >= 2)
								{
									SysLogServer.WriteLog("update..." + array2[1] + "_" + array[1], true);
									result = devServerBll.SetDeviceData(array2[1], array[1], "");
								}
								break;
							case "delete":
								if (array2.Length >= 2)
								{
									SysLogServer.WriteLog("delete..." + array2[1] + "_" + array[1], true);
									result = devServerBll.DeleteDeviceData(array2[1], array[1], "");
								}
								break;
							case "set":
								SysLogServer.WriteLog("set..._" + array[1], true);
								result = devServerBll.SetDeviceParam(array[1]);
								break;
							}
						}
					}
				}
			}
			return result;
		}

		public static int PcToDevice(DeviceServerBll devServerBll, DevCmds model)
		{
			return CommandServer.PcToDevice(devServerBll, model.CmdContent);
		}

		public static int PcToDevice(DevCmds model)
		{
			int result = 0;
			DeviceServerBll deviceServerBll = null;
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			if (model.SN_id > 0)
			{
				Machines model2 = machinesBll.GetModel(model.SN_id);
				if (model2 != null)
				{
					deviceServerBll = DeviceServers.Instance.GetDeviceServer(model2);
					if (deviceServerBll != null)
					{
						result = CommandServer.PcToDevice(deviceServerBll, model.CmdContent);
					}
				}
			}
			return result;
		}

		public static void SetInterLock(int interLock, Machines dev)
		{
			string cmdContent = "set$InterLock=" + interLock;
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = dev.ID;
			devCmds.status = 0;
			devCmds.CmdContent = cmdContent;
			devCmds.CmdCommitTime = DateTime.Now;
			devCmds.CmdReturnContent = string.Empty;
			devCmdsBll.Add(devCmds);
		}

		public static void SetInterLock(int interLock, int deviceid)
		{
			string cmdContent = "set$InterLock=" + interLock;
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = deviceid;
			devCmds.status = 0;
			devCmds.CmdContent = cmdContent;
			devCmds.CmdCommitTime = DateTime.Now;
			devCmds.CmdReturnContent = string.Empty;
			devCmdsBll.Add(devCmds);
		}

		public static void SetDoorParam(int doorid)
		{
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			AccDoor model = accDoorBll.GetModel(doorid);
			if (model != null)
			{
				CommandServer.SetDoorParam(model);
			}
		}

		public static void SetDoorParam(AccDoor door)
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			Machines model = machinesBll.GetModel(door.device_id);
			if (model != null)
			{
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				modelList = ((modelList == null) ? new List<AccTimeseg>() : modelList);
				Dictionary<int, AccTimeseg> dictionary = new Dictionary<int, AccTimeseg>();
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!dictionary.ContainsKey(modelList[i].id))
					{
						dictionary.Add(modelList[i].id, modelList[i]);
					}
				}
				string cmdContent = (model.DevSDKType == SDKType.StandaloneSDK) ? ("set$" + door.ToPullCmdString(model, DeviceHelper.TimeSeg.id, dictionary)) : ((!(model.CompatOldFirmware == "0")) ? PullSDKDataConvertHelper.SetDoorParam(door, model) : ("set$" + door.ToPullCmdString(model, DeviceHelper.TimeSeg.id, dictionary)));
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				DevCmds devCmds = new DevCmds();
				devCmds.SN_id = model.ID;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
			}
		}

		public static void SetWiegandParam(AccDoor door)
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			Machines model = machinesBll.GetModel(door.device_id);
			if (model != null)
			{
				string str = PullSDKDataConvertHelper.SetWiegandParam(door, model);
				str = "set$" + str;
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				DevCmds devCmds = new DevCmds();
				devCmds.SN_id = model.ID;
				devCmds.status = 0;
				devCmds.CmdContent = str;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
			}
		}

		public static void SetAntiPassback(int passbackType, Machines dev)
		{
			string cmdContent = "set$AntiPassback=" + passbackType;
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = dev.ID;
			devCmds.status = 0;
			devCmds.CmdContent = cmdContent;
			devCmds.CmdCommitTime = DateTime.Now;
			devCmds.CmdReturnContent = string.Empty;
			devCmdsBll.Add(devCmds);
		}

		public static void SetAntiPassback(int passbackType, int deviceid)
		{
			string cmdContent = "set$AntiPassback=" + passbackType;
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = deviceid;
			devCmds.status = 0;
			devCmds.CmdContent = cmdContent;
			devCmds.CmdCommitTime = DateTime.Now;
			devCmds.CmdReturnContent = string.Empty;
			devCmdsBll.Add(devCmds);
		}

		public static void DelCmd(AccHolidays model)
		{
			string cmdContent = PullSDKDataConvertHelper.DeleteHoliday(model, true);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					DevCmds devCmds;
					if (list[i].DevSDKType != SDKType.StandaloneSDK)
					{
						devCmds = new DevCmds();
						devCmds.SN_id = list[i].ID;
						devCmds.status = 0;
						if (list[i].DevSDKType == SDKType.StandaloneSDK)
						{
							if (model.HolidayNo <= 0 || model.HolidayTZ <= 0)
							{
								goto IL_00d2;
							}
						}
						else
						{
							devCmds.CmdContent = cmdContent;
						}
						goto IL_00d2;
					}
					continue;
					IL_00d2:
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static void AddCmd(AccHolidays model)
		{
			string cmdContent = PullSDKDataConvertHelper.AddHoliday(model, true);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = list[i].ID;
					devCmds.status = 0;
					if (list[i].DevSDKType == SDKType.StandaloneSDK)
					{
						if (model.HolidayNo > 0)
						{
							devCmds.CmdContent = "update|Holiday$" + model.ToPullCmdString(list[i]);
						}
					}
					else
					{
						devCmds.CmdContent = cmdContent;
					}
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static void DelCmd(AccTimeseg model)
		{
			string cmdContent = PullSDKDataConvertHelper.DeleteTimeZone(model, true);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].DevSDKType != SDKType.StandaloneSDK || model.TimeZone1Id > 0 || model.TimeZone2Id > 0 || model.TimeZone3Id > 0 || model.TimeZoneHolidayId > 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = list[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = cmdContent;
						if (list[i].DevSDKType == SDKType.StandaloneSDK)
						{
							AccTimeseg accTimeseg = new AccTimeseg();
							accTimeseg.id = model.id;
							accTimeseg.TimeZone1Id = model.TimeZone1Id;
							accTimeseg.TimeZone2Id = model.TimeZone2Id;
							accTimeseg.TimeZone3Id = model.TimeZone3Id;
							accTimeseg.TimeZoneHolidayId = model.TimeZoneHolidayId;
							devCmds.CmdContent = "update|TimeZone$" + accTimeseg.ToPullCmdString(list[i], DeviceHelper.TimeSeg.id);
						}
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						devCmdsBll.Add(devCmds);
					}
				}
			}
		}

		public static void AddCmd(AccTimeseg model)
		{
			string text = PullSDKDataConvertHelper.AddTimeZone(model, true);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (modelList[i].DevSDKType != SDKType.StandaloneSDK || model.TimeZone1Id > 0 || model.TimeZone2Id > 0 || model.TimeZone3Id > 0 || model.TimeZoneHolidayId > 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = modelList[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = "update|TimeZone$" + model.ToPullCmdString(modelList[i], DeviceHelper.TimeSeg.id);
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						devCmdsBll.Add(devCmds);
					}
				}
			}
		}

		public static void DelCmd(AccLinkAgeIo model)
		{
			string cmdContent = PullSDKDataConvertHelper.DeleteInOutFun(model, true);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = model.device_id;
			devCmds.status = 0;
			devCmds.CmdContent = cmdContent;
			devCmds.CmdCommitTime = DateTime.Now;
			devCmds.CmdReturnContent = string.Empty;
			devCmdsBll.Add(devCmds);
		}

		public static void AddCmd(AccLinkAgeIo model)
		{
			string cmdContent = PullSDKDataConvertHelper.AddInOutFun(model, true);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = model.device_id;
			devCmds.status = 0;
			devCmds.CmdContent = cmdContent;
			devCmds.CmdCommitTime = DateTime.Now;
			devCmds.CmdReturnContent = string.Empty;
			devCmdsBll.Add(devCmds);
		}

		public static void AddCmd(AccMorecardset model)
		{
			CommandServer.InitDoor();
			if (model != null && CommandServer.doorlist.ContainsKey(model.door_id))
			{
				string cmdContent = PullSDKDataConvertHelper.AddMultimCard(model, true, null);
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				DevCmds devCmds = new DevCmds();
				devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
				cmdContent = $"setMultiCardState$Door{CommandServer.doorlist[model.door_id].door_no}MultiCardOpenDoor=1";
				cmdContent.Remove(cmdContent.Length - 1);
				devCmds = new DevCmds();
				devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
			}
		}

		public static void DelCmd(AccMorecardset model)
		{
			CommandServer.InitDoor();
			if (model != null && CommandServer.doorlist.ContainsKey(model.door_id))
			{
				string cmdContent = PullSDKDataConvertHelper.DeleteMultimCard(model, true);
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				DevCmds devCmds = new DevCmds();
				devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
				AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
				List<AccMorecardset> modelList = accMorecardsetBll.GetModelList($"door_id={model.door_id} and id <> {model.id}");
				if (modelList == null || modelList.Count <= 0)
				{
					cmdContent = $"setMultiCardState$Door{CommandServer.doorlist[model.door_id].door_no}MultiCardOpenDoor=0";
					cmdContent.Remove(cmdContent.Length - 1);
					devCmds = new DevCmds();
					devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static string GetTemplateCmd(UserInfo model)
		{
			TemplateBll templateBll = new TemplateBll(MainForm._ia);
			List<Template> modelList = templateBll.GetModelList("userid=" + model.UserId);
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					modelList[i].Pin = model.BadgeNumber;
				}
				result = PullSDKDataConvertHelper.AddTemplateV10(modelList, true);
			}
			return result;
		}

		public static string GetTemplateCmd(List<UserInfo> users)
		{
			string result = string.Empty;
			if (users != null && users.Count > 0)
			{
				List<Template> list = new List<Template>();
				List<Template> list2 = null;
				List<UserInfo> list3 = new List<UserInfo>();
				List<UserInfo> list4 = new List<UserInfo>();
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				for (int i = 0; i < users.Count; i++)
				{
					list4.Add(users[i]);
					if (i > 0 && i % 10000 == 0)
					{
						list2 = CommandServer.Get_Template_User(list4);
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2);
						}
						list4.Clear();
					}
				}
				list2 = CommandServer.Get_Template_User(list4);
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(list2);
				}
				result = PullSDKDataConvertHelper.AddTemplateV10(list, true);
			}
			return result;
		}

		public static List<Template> Get_Template_User(List<UserInfo> users)
		{
			if (users != null && users.Count > 0)
			{
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				List<Template> list = new List<Template>();
				StringBuilder stringBuilder = new StringBuilder();
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				stringBuilder.Append("userid in(" + users[0].UserId);
				dictionary.Add(users[0].UserId, users[0].BadgeNumber);
				for (int i = 1; i < users.Count; i++)
				{
					if (!dictionary.ContainsKey(users[i].UserId))
					{
						stringBuilder.Append("," + users[i].UserId);
						dictionary.Add(users[i].UserId, users[i].BadgeNumber);
					}
				}
				stringBuilder.Append(")");
				List<Template> list2 = null;
				try
				{
					list2 = templateBll.GetModelList(stringBuilder.ToString());
				}
				catch
				{
				}
				if (list2 != null && list2.Count > 0)
				{
					list.AddRange(list2);
				}
				if (list != null && list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (dictionary.ContainsKey(list[j].USERID))
						{
							list[j].Pin = dictionary[list[j].USERID];
						}
					}
				}
				return list;
			}
			return null;
		}

		public static string DeleteTemplateCmd(UserInfo model)
		{
			TemplateBll templateBll = new TemplateBll(MainForm._ia);
			List<Template> modelList = templateBll.GetModelList("userid=" + model.UserId);
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					modelList[i].Pin = model.BadgeNumber;
				}
				result = PullSDKDataConvertHelper.DeleteTemplateV10(modelList, true);
			}
			return result;
		}

		public static string DeleteTemplateCmdEx(List<UserInfo> model)
		{
			if (model != null)
			{
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < model.Count; i++)
				{
					objTemplateV.Pin = model[i].BadgeNumber;
					objTemplateV.FingerID = (FingerType)Enum.Parse(typeof(FingerType), "-1");
					string value = PullSDKDataConvert.DeleteTemplateV10(objTemplateV);
					stringBuilder.Append(value);
				}
				return "delete|TemplateV10$" + stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static void DeleteTemplateCmdEx(int devID, List<UserInfo> list)
		{
			if (list != null)
			{
				List<UserInfo> list2 = new List<UserInfo>();
				List<UserInfo> list3 = new List<UserInfo>();
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				Dictionary<int, UserInfo> dictionary2 = new Dictionary<int, UserInfo>();
				StringBuilder stringBuilder = new StringBuilder();
				string empty = string.Empty;
				for (int i = 0; i < list.Count; i++)
				{
					list3.Add(list[i]);
					if (i > 0 && i % 10000 == 0)
					{
						CommandServer.del_Template_Cmd(devID, list3);
						list3.Clear();
					}
				}
				CommandServer.del_Template_Cmd(devID, list3);
			}
		}

		public static void del_Template_Cmd(int devID, List<UserInfo> list)
		{
			if (list != null && list.Count > 0)
			{
				List<UserInfo> list2 = new List<UserInfo>();
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				Dictionary<int, UserInfo> dictionary2 = new Dictionary<int, UserInfo>();
				StringBuilder stringBuilder = new StringBuilder();
				string empty = string.Empty;
				dictionary2.Clear();
				stringBuilder.Append("userid in(" + list[0].UserId);
				dictionary2.Add(list[0].UserId, list[0]);
				for (int i = 1; i < list.Count; i++)
				{
					if (!dictionary2.ContainsKey(list[i].UserId))
					{
						stringBuilder.Append("," + list[i].UserId);
						dictionary2.Add(list[i].UserId, list[i]);
					}
				}
				stringBuilder.Append(")");
				List<Template> list3 = null;
				try
				{
					list3 = templateBll.GetModelList(stringBuilder.ToString());
				}
				catch
				{
				}
				if (list3 != null && list3.Count > 0)
				{
					for (int j = 0; j < list3.Count; j++)
					{
						if (!dictionary.ContainsKey(list3[j].USERID))
						{
							dictionary.Add(list3[j].USERID, dictionary2[list3[j].USERID]);
						}
					}
					int num = 0;
					foreach (KeyValuePair<int, UserInfo> item in dictionary)
					{
						list2.Add(item.Value);
						num++;
						if (num > 0 && num % 3000 == 0)
						{
							empty = CommandServer.DeleteTemplateCmdEx(list2);
							CommandServer.SaveCmdInfo(devID, empty);
							empty = string.Empty;
							list2.Clear();
						}
					}
					empty = CommandServer.DeleteTemplateCmdEx(list2);
					CommandServer.SaveCmdInfo(devID, empty);
					empty = string.Empty;
					dictionary.Clear();
					list2.Clear();
				}
			}
		}

		public static string DeleteTemplateCmdEx(UserInfo model)
		{
			TemplateBll templateBll = new TemplateBll(MainForm._ia);
			List<Template> modelList = templateBll.GetModelList("userid=" + model.UserId);
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				int num = 0;
				if (num < modelList.Count)
				{
					modelList[num].Pin = model.BadgeNumber;
					result = PullSDKDataConvertHelper.DeleteTemplateV10Ex(modelList[num], true);
				}
			}
			return result;
		}

		public static string DeleteTemplateCmdEx1(UserInfo model)
		{
			if (model != null)
			{
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				objTemplateV.Pin = model.BadgeNumber;
				objTemplateV.FingerID = (FingerType)Enum.Parse(typeof(FingerType), "-1");
				string str = PullSDKDataConvert.DeleteTemplateV10(objTemplateV);
				return "delete|TemplateV10$" + str;
			}
			return string.Empty;
		}

		public static string DeleteTemplateCmd(UserInfo model, int FingerId)
		{
			TemplateBll templateBll = new TemplateBll(MainForm._ia);
			List<Template> modelList = templateBll.GetModelList("userid=" + model.UserId);
			Template model2 = new Template();
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				int num = 0;
				while (num < modelList.Count)
				{
					if (FingerId != modelList[num].FINGERID)
					{
						num++;
						continue;
					}
					modelList[num].Pin = model.BadgeNumber;
					model2 = modelList[num];
					break;
				}
				result = PullSDKDataConvertHelper.DeleteTemplateV10(model2, true);
			}
			return result;
		}

		public static void DeleteTemplateCmd(UserInfo model, List<Machines> dlist, int FingerId)
		{
			string text = CommandServer.DeleteTemplateCmd(model, FingerId);
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			if (dlist != null && dlist.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				for (int i = 0; i < dlist.Count; i++)
				{
					DevCmds devCmds = new DevCmds();
					if (!string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
					{
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = text;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						devCmdsBll.Add(devCmds);
					}
				}
			}
		}

		public static string GetFaceTempCmd(UserInfo model)
		{
			ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
			List<FaceTemp> modelList = faceTempBll.GetModelList("UserNo='" + model.UserId + "'");
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					modelList[i].Pin = model.BadgeNumber;
				}
				result = PullSDKDataConvertHelper.AddFaceTemp(modelList, true);
			}
			return result;
		}

		public static string GetFaceTempCmd(List<UserInfo> users)
		{
			ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
			List<ObjFaceTemp> list = new List<ObjFaceTemp>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UserNo in('" + users[0].UserId + "'");
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(users[0].UserId, users[0].BadgeNumber);
			for (int i = 1; i < users.Count; i++)
			{
				if (!dictionary.ContainsKey(users[i].UserId))
				{
					stringBuilder.Append(",'" + users[i].UserId + "'");
					dictionary.Add(users[i].UserId, users[i].BadgeNumber);
				}
			}
			stringBuilder.Append(")");
			List<FaceTemp> list2 = null;
			try
			{
				list2 = faceTempBll.GetModelList(stringBuilder.ToString());
			}
			catch
			{
			}
			string result = string.Empty;
			if (list2 != null && list2.Count > 0)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					if (dictionary.ContainsKey(list2[j].UserId))
					{
						list2[j].Pin = dictionary[list2[j].UserId];
					}
				}
				result = PullSDKDataConvertHelper.AddFaceTemp(list2, true);
			}
			return result;
		}

		public static string DeleteFaceTempCmd(UserInfo model)
		{
			ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
			List<FaceTemp> modelList = faceTempBll.GetModelList("UserNo='" + model.UserId + "'");
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					modelList[i].Pin = model.BadgeNumber;
				}
				result = PullSDKDataConvertHelper.DeleteFaceTemp(modelList, true);
			}
			return result;
		}

		public static void DeleteFaceTempCmd(UserInfo model, List<Machines> dlist)
		{
			string text = CommandServer.DeleteFaceTempCmd(model);
			if (dlist != null && dlist.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				List<DevCmds> list = new List<DevCmds>();
				for (int i = 0; i < dlist.Count; i++)
				{
					if (!string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = text;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						list.Add(devCmds);
					}
				}
				devCmdsBll.Add(list);
			}
		}

		public static string GetFingerVeinCmd(UserInfo model)
		{
			FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
			List<FingerVein> modelList = fingerVeinBll.GetModelList("UserID=" + model.UserId);
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					modelList[i].Pin = model.BadgeNumber;
				}
				result = PullSDKDataConvertHelper.AddFingerVein(modelList, true);
			}
			return result;
		}

		public static string GetFingerVeinCmd(List<UserInfo> users)
		{
			FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
			List<ObjFVTemplate> list = new List<ObjFVTemplate>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("userid in(" + users[0].UserId);
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(users[0].UserId, users[0].BadgeNumber);
			for (int i = 1; i < users.Count; i++)
			{
				if (!dictionary.ContainsKey(users[i].UserId))
				{
					stringBuilder.Append("," + users[i].UserId);
					dictionary.Add(users[i].UserId, users[i].BadgeNumber);
				}
			}
			stringBuilder.Append(")");
			List<FingerVein> list2 = null;
			try
			{
				list2 = fingerVeinBll.GetModelList(stringBuilder.ToString());
			}
			catch
			{
			}
			string result = string.Empty;
			if (list2 != null && list2.Count > 0)
			{
				for (int j = 0; j < list2.Count; j++)
				{
					if (dictionary.ContainsKey(list2[j].UserID))
					{
						list2[j].Pin = dictionary[list2[j].UserID];
					}
				}
				result = PullSDKDataConvertHelper.AddFingerVein(list2, true);
			}
			return result;
		}

		public static string DeleteFingerVeinCmd(UserInfo model)
		{
			FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
			List<FingerVein> modelList = fingerVeinBll.GetModelList("UserID=" + model.UserId);
			string result = string.Empty;
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					modelList[i].Pin = model.BadgeNumber;
				}
				result = PullSDKDataConvertHelper.DeleteFingerVein(modelList, true);
			}
			return result;
		}

		public static void DeleteFingerVeinCmd(UserInfo model, List<Machines> dlist)
		{
			string text = CommandServer.DeleteFingerVeinCmd(model);
			if (dlist != null && dlist.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				List<DevCmds> list = new List<DevCmds>();
				for (int i = 0; i < dlist.Count; i++)
				{
					if (!string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = text;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						list.Add(devCmds);
					}
				}
				devCmdsBll.Add(list);
			}
		}

		public static void DeleteFingerVeinCmd(string UserPin, List<Machines> dlist, ref List<DevCmds> lstDevCmds)
		{
			if (!string.IsNullOrEmpty(UserPin) && dlist != null && dlist.Count > 0)
			{
				string text = "delete|fvtemplate$PIN2=" + UserPin + "#";
				for (int i = 0; i < dlist.Count; i++)
				{
					if (dlist[i].DevSDKType != SDKType.StandaloneSDK && !string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = text;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						lstDevCmds.Add(devCmds);
					}
				}
			}
		}

		public static void AddCmd(UserInfo model, List<Machines> dlist, bool isUpdateFP, string cmdContent)
		{
			string templateCmd = CommandServer.GetTemplateCmd(model);
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			if (dlist != null && dlist.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				for (int i = 0; i < dlist.Count; i++)
				{
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = dlist[i].ID;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
					if (isUpdateFP && !string.IsNullOrEmpty(templateCmd) && dlist[i].IsOnlyRFMachine == 0)
					{
						devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = templateCmd;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						devCmdsBll.Add(devCmds);
					}
				}
			}
		}

		public static void DeleteTemplateCmd(UserInfo model, List<Machines> dlist, bool isDelete, string cmdContent)
		{
			string text = string.Empty;
			if (isDelete)
			{
				text = CommandServer.DeleteTemplateCmd(model);
			}
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			if (dlist != null && dlist.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				for (int i = 0; i < dlist.Count; i++)
				{
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = dlist[i].ID;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
					if (isDelete && !string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
					{
						devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = text;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						devCmdsBll.Add(devCmds);
					}
				}
			}
		}

		public static void DeleteTemplateCmd(UserInfo model, List<Machines> dlist)
		{
			string empty = string.Empty;
			empty = CommandServer.DeleteTemplateCmdEx(model);
			if (dlist != null && dlist.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				List<DevCmds> list = new List<DevCmds>();
				for (int i = 0; i < dlist.Count; i++)
				{
					if (!string.IsNullOrEmpty(empty) && dlist[i].IsOnlyRFMachine == 0 && dlist[i].DevSDKType == SDKType.PullSDK)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = empty;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						list.Add(devCmds);
					}
				}
				devCmdsBll.Add(list);
			}
		}

		public static void DeleteTemplateCmd(string UserPin, List<Machines> dlist, ref List<DevCmds> lstDevCmds)
		{
			if (!string.IsNullOrEmpty(UserPin) && dlist != null && dlist.Count > 0)
			{
				string empty = string.Empty;
				empty = "delete|TemplateV10$Pin=" + UserPin + "#";
				for (int i = 0; i < dlist.Count; i++)
				{
					if (dlist[i].DevSDKType != SDKType.StandaloneSDK && !string.IsNullOrEmpty(empty) && dlist[i].IsOnlyRFMachine == 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = empty;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						lstDevCmds.Add(devCmds);
					}
				}
			}
		}

		public static void AddCmd(UserInfo model, bool isDelete, string cmdContent, List<Machines> dlist, ref List<DevCmds> lstDevCmds)
		{
			if (model != null)
			{
				string text = (!isDelete) ? CommandServer.GetTemplateCmd(model) : CommandServer.DeleteTemplateCmd(model);
				if (dlist != null && dlist.Count > 0)
				{
					for (int i = 0; i < dlist.Count; i++)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = cmdContent;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						lstDevCmds.Add(devCmds);
						if (!string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
						{
							devCmds = new DevCmds();
							devCmds.SN_id = dlist[i].ID;
							devCmds.status = 0;
							devCmds.CmdContent = text;
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							lstDevCmds.Add(devCmds);
						}
					}
				}
			}
		}

		public static void DeleteUserAuthorizeCmd(string UserPin, List<Machines> dlist, ref List<DevCmds> lstDevCmd)
		{
			if (!string.IsNullOrEmpty(UserPin) && dlist != null && dlist.Count > 0)
			{
				string cmdContent = "delete|UserAuthorize$Pin=" + UserPin + "#";
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				for (int i = 0; i < dlist.Count; i++)
				{
					if (dlist[i].DevSDKType != SDKType.StandaloneSDK)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = cmdContent;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						lstDevCmd.Add(devCmds);
					}
				}
			}
		}

		public static void AddCmd(UserInfo model, bool isDelete, string cmdContent, List<Machines> dlist)
		{
			if (model != null)
			{
				string text = (!isDelete) ? CommandServer.GetTemplateCmd(model) : CommandServer.DeleteTemplateCmd(model);
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				if (dlist != null && dlist.Count > 0)
				{
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					List<DevCmds> list = new List<DevCmds>();
					for (int i = 0; i < dlist.Count; i++)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = cmdContent;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						list.Add(devCmds);
						if (!string.IsNullOrEmpty(text) && dlist[i].IsOnlyRFMachine == 0)
						{
							devCmds = new DevCmds();
							devCmds.SN_id = dlist[i].ID;
							devCmds.status = 0;
							devCmds.CmdContent = text;
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							list.Add(devCmds);
						}
					}
					devCmdsBll.Add(list);
				}
			}
		}

		public static void AddCmd(UserInfo model)
		{
			CommandServer.AddCmd(model, true, "");
		}

		public static bool AddCmd(UserInfo model, bool isAddTemplate, string devFilter = "")
		{
			bool result = false;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList("id in(select  device_id from acc_door where id in (select  accdoor_id from acc_levelset_door_group where acclevelset_id in (select acclevelset_id from acc_levelset_emp where employee_id=" + model.UserId + "))) " + ((devFilter != null && "" != devFilter.Trim()) ? (" and " + devFilter) : ""));
			if (modelList != null && modelList.Count > 0)
			{
				result = true;
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				for (int i = 0; i < modelList.Count; i++)
				{
					string cmdContent = PullSDKDataConvertHelper.AddUser(model, modelList[i], true);
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = modelList[i].ID;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
					if (isAddTemplate)
					{
						empty = CommandServer.GetTemplateCmd(model);
						empty2 = CommandServer.GetFingerVeinCmd(model);
						empty3 = CommandServer.GetFaceTempCmd(model);
						if (!string.IsNullOrEmpty(empty2) && modelList[i].device_type == 102)
						{
							devCmds = new DevCmds();
							devCmds.SN_id = modelList[i].ID;
							devCmds.status = 0;
							devCmds.CmdContent = empty2;
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							devCmdsBll.Add(devCmds);
						}
						else if (!string.IsNullOrEmpty(empty) && modelList[i].IsOnlyRFMachine == 0)
						{
							devCmds = new DevCmds();
							devCmds.SN_id = modelList[i].ID;
							devCmds.status = 0;
							devCmds.CmdContent = empty;
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							devCmdsBll.Add(devCmds);
						}
						if ((!string.IsNullOrEmpty(empty3) && modelList[i].device_type == 103) || (modelList[i].DevSDKType == SDKType.StandaloneSDK && modelList[i].FaceFunOn == 1))
						{
							devCmds = new DevCmds();
							devCmds.SN_id = modelList[i].ID;
							devCmds.status = 0;
							devCmds.CmdContent = empty3;
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							devCmdsBll.Add(devCmds);
						}
					}
				}
			}
			return result;
		}

		public static void AddCmdEx(UserInfo model)
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList("id in(select  device_id from acc_door where id in(select  accdoor_id from acc_levelset_door_group where acclevelset_id in(select acclevelset_id from acc_levelset_emp where employee_id=" + model.UserId + ")))");
			if (modelList != null && modelList.Count > 0)
			{
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				for (int i = 0; i < modelList.Count; i++)
				{
					string cmdContent = PullSDKDataConvertHelper.AddUser(model, modelList[i], true);
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = modelList[i].ID;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static void DeleteUserCmd(string UserPin, List<Machines> dlist, ref List<DevCmds> lstDevCmds)
		{
			if (!string.IsNullOrEmpty(UserPin) && dlist != null && dlist.Count > 0)
			{
				string cmdContent = "delete|User$Pin=" + UserPin + "#";
				if (dlist != null && dlist.Count > 0)
				{
					for (int i = 0; i < dlist.Count; i++)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = cmdContent;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						lstDevCmds.Add(devCmds);
					}
				}
			}
		}

		public static void DelCmd(UserInfo model, List<Machines> dlist)
		{
			if (model != null)
			{
				string cmdContent = PullSDKDataConvertHelper.DeleteUser(model, true);
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				if (dlist != null && dlist.Count > 0)
				{
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					List<DevCmds> list = new List<DevCmds>();
					for (int i = 0; i < dlist.Count; i++)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = dlist[i].ID;
						devCmds.status = 0;
						devCmds.CmdContent = cmdContent;
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						list.Add(devCmds);
					}
					devCmdsBll.Add(list);
				}
			}
		}

		public static void AddCmd(AccLevelset model, int OldTimeZoneId, bool Processing = false)
		{
			CommandServer.UserCmd(model, OldTimeZoneId, true, Processing);
		}

		public static void DelCmd(AccLevelset model, int OldTimeZoneId, bool Processing = false)
		{
			CommandServer.UserCmd(model, OldTimeZoneId, false, Processing);
		}

		private static void UserCmd(AccLevelset model, int OldTimeZoneId, bool isadd, bool Processing = false)
		{
			AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
			List<AccLevelsetEmp> modelList = accLevelsetEmpBll.GetModelList("acclevelset_id=" + model.id);
			if (modelList != null && modelList.Count > 0)
			{
				CommandServer.UserCmd(model, OldTimeZoneId, isadd, modelList, Processing);
			}
		}

		public static void UserCmd(AccLevelset model, int OldTimeZoneId, bool isadd, List<AccLevelsetEmp> users, bool Processing = false)
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList("id in(select  device_id from acc_door where id in(select  accdoor_id from acc_levelset_door_group where acclevelset_id=" + model.id + "))");
			if (modelList != null && modelList.Count > 0 && users != null && users.Count > 0)
			{
				CommandServer.UserCmd(model, OldTimeZoneId, isadd, modelList, users, Processing);
			}
		}

		public static void UserCmd(AccLevelset model, int OldTimeZoneId, bool isadd, List<AccLevelsetDoorGroup> doors, List<AccLevelsetEmp> users, bool Processing = false)
		{
			if (doors != null && doors.Count > 0)
			{
				int accdoor_id = doors[0].accdoor_id;
				string text = accdoor_id.ToString();
				for (int i = 1; i < doors.Count; i++)
				{
					string str = text;
					accdoor_id = doors[i].accdoor_id;
					text = str + "," + accdoor_id.ToString();
				}
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> modelList = machinesBll.GetModelList("id in(select distinct device_id from acc_door where id in(" + text + "))");
				if (modelList != null && modelList.Count > 0)
				{
					CommandServer.UserCmd(model, OldTimeZoneId, isadd, modelList, users, Processing);
				}
			}
		}

		public static void SaveCmdInfo(int ID, string cmdInfo)
		{
			if (cmdInfo.Trim() != "")
			{
				DevCmds devCmds = new DevCmds();
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				devCmds.SN_id = ID;
				devCmds.status = 0;
				devCmds.CmdContent = cmdInfo;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
			}
		}

		private static void SaveCmdInfoEx(int ID, string cmdInfo)
		{
			if (cmdInfo.Trim() != "")
			{
				DevCmds devCmds = new DevCmds();
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				devCmds.SN_id = ID;
				devCmds.status = 0;
				devCmds.CmdContent = cmdInfo;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.AddEx(devCmds);
			}
		}

		[Obsolete]
		public static void DeleUserToCmd(AccLevelset model, List<AccLevelsetEmp> users, List<AccLevelsetDoorGroup> changeDoors, List<AccLevelsetDoorGroup> oldDoors)
		{
			if (changeDoors != null && changeDoors.Count > 0)
			{
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				if (users.Count > 10)
				{
					List<UserInfo> modelList = userInfoBll.GetModelList("");
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							dictionary.Add(modelList[i].UserId, modelList[i]);
						}
					}
				}
				int level_timeseg_id = model.level_timeseg_id;
				List<UserInfo> list = new List<UserInfo>();
				List<ObjUserAuthorize> list2 = new List<ObjUserAuthorize>();
				if (users != null && users.Count > 0)
				{
					for (int j = 0; j < users.Count; j++)
					{
						UserInfo userInfo = null;
						userInfo = ((!dictionary.ContainsKey(users[j].employee_id)) ? userInfoBll.GetModel(users[j].employee_id) : dictionary[users[j].employee_id]);
						if (userInfo != null)
						{
							ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
							objUserAuthorize.AuthorizeDoorId = "0";
							objUserAuthorize.Pin = userInfo.BadgeNumber;
							objUserAuthorize.AuthorizeTimezoneId = level_timeseg_id.ToString();
							if (DeviceHelper.TimeSeg.id == level_timeseg_id)
							{
								objUserAuthorize.AuthorizeTimezoneId = "1";
							}
							list.Add(userInfo);
							list2.Add(objUserAuthorize);
						}
					}
				}
				AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
				List<AccLevelsetEmp> modelList2 = accLevelsetEmpBll.GetModelList($" acclevelset_id <> {model.id}  order by employee_id ");
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				List<AccLevelsetDoorGroup> modelList3 = accLevelsetDoorGroupBll.GetModelList($" acclevelset_id <> {model.id} ");
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				int accdoor_id = changeDoors[0].accdoor_id;
				string text = accdoor_id.ToString();
				for (int k = 1; k < changeDoors.Count; k++)
				{
					string str = text;
					accdoor_id = changeDoors[k].accdoor_id;
					text = str + "," + accdoor_id.ToString();
				}
				List<Machines> modelList4 = machinesBll.GetModelList("id in(select distinct device_id from acc_door where id in(" + text + "))");
				if (modelList4 != null && modelList4.Count > 0)
				{
					AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
					for (int l = 0; l < modelList4.Count; l++)
					{
						List<ObjUserAuthorize> list3 = new List<ObjUserAuthorize>();
						for (int m = 0; m < list2.Count; m++)
						{
							list3.Add(list2[m]);
						}
						List<int> list4 = new List<int>();
						List<int> list5 = new List<int>();
						List<UserInfo> list6 = new List<UserInfo>();
						List<AccDoor> list7 = null;
						list7 = accDoorBll.GetModelList("device_id=" + modelList4[l].ID + " order by door_no");
						int num = 0;
						for (int n = 0; n < oldDoors.Count; n++)
						{
							for (int num2 = 0; num2 < list7.Count; num2++)
							{
								if (oldDoors[n].accdoor_id == list7[num2].id)
								{
									num += 1 << list7[num2].door_no - 1;
								}
							}
						}
						for (int num3 = 0; num3 < list2.Count; num3++)
						{
							list2[num3].AuthorizeDoorId = num.ToString();
						}
						for (int num4 = 0; num4 < list2.Count; num4++)
						{
							list2[num4].AuthorizeDoorId = num.ToString();
							list3[num4].AuthorizeDoorId = num.ToString();
						}
						accDoorBll.IsConnectDb(true);
						for (int num5 = 0; num5 < list.Count; num5++)
						{
							UserInfo userInfo2 = list[num5];
							list6.Add(userInfo2);
							int num6 = CommandServer.checkdata(num, modelList4[l].ID, userInfo2.UserId, model.id, level_timeseg_id, modelList2, modelList3);
							if (num6 > 0)
							{
								list6.Remove(userInfo2);
								if (num6 == 2)
								{
									list5.Add(int.Parse(userInfo2.BadgeNumber));
								}
							}
							else
							{
								string empty = string.Empty;
								if (modelList4[l].device_type == 102 && modelList4[l].FvFunOn == 1)
								{
									empty = CommandServer.DeleteFingerVeinCmd(userInfo2);
									CommandServer.SaveCmdInfo(modelList4[l].ID, empty);
								}
								else if (modelList4[l].device_type == 103 || (modelList4[l].DevSDKType == SDKType.StandaloneSDK && modelList4[l].FaceFunOn == 1))
								{
									empty = CommandServer.DeleteFaceTempCmd(userInfo2);
									CommandServer.SaveCmdInfo(modelList4[l].ID, empty);
								}
							}
						}
						accDoorBll.IsConnectDb(false);
						string empty2 = string.Empty;
						if (modelList4[l].IsOnlyRFMachine == 0 && modelList4[l].DevSDKType == SDKType.PullSDK)
						{
							CommandServer.DeleteTemplateCmdEx(modelList4[l].ID, list6);
						}
						if (list6.Count < 500)
						{
							empty2 = PullSDKDataConvertHelper.DeleteUser(list6, true);
							CommandServer.SaveCmdInfo(modelList4[l].ID, empty2);
						}
						else
						{
							List<UserInfo> list8 = new List<UserInfo>();
							foreach (UserInfo item in list6)
							{
								list8.Add(item);
								if (list8.Count != 0 && list8.Count % 500 == 0)
								{
									empty2 = PullSDKDataConvertHelper.DeleteUser(list8, true);
									CommandServer.SaveCmdInfo(modelList4[l].ID, empty2);
									list8.Clear();
								}
							}
							empty2 = PullSDKDataConvertHelper.DeleteUser(list8, true);
							CommandServer.SaveCmdInfo(modelList4[l].ID, empty2);
						}
						if (list7 != null && list7.Count > 0 && num != 0 && list2.Count > 0)
						{
							if (list5 != null && list5.Count > 0)
							{
								for (int num7 = list2.Count - 1; num7 >= 0; num7--)
								{
									if (list5.Contains(int.Parse(list2[num7].Pin)))
									{
										list3.RemoveAt(num7);
									}
								}
							}
							if (list3.Count < 500)
							{
								empty2 = PullSDKDataConvertHelper.DeleteUserAuthorize(list3, true);
								CommandServer.SaveCmdInfo(modelList4[l].ID, empty2);
							}
							else
							{
								List<ObjUserAuthorize> list9 = new List<ObjUserAuthorize>();
								foreach (ObjUserAuthorize item2 in list3)
								{
									list9.Add(item2);
									if (list9.Count != 0 && list9.Count % 500 == 0)
									{
										empty2 = PullSDKDataConvertHelper.DeleteUserAuthorize(list9, true);
										CommandServer.SaveCmdInfo(modelList4[l].ID, empty2);
										list9.Clear();
									}
								}
								empty2 = PullSDKDataConvertHelper.DeleteUserAuthorize(list9, true);
								CommandServer.SaveCmdInfo(modelList4[l].ID, empty2);
							}
						}
					}
				}
			}
		}

		public static int checkdata(int deviceID, int levelID, List<AccLevelsetDoorGroup> listDoor)
		{
			int result = 0;
			if (listDoor != null && listDoor.Count > 0)
			{
				bool flag = false;
				int num = 0;
				while (num < listDoor.Count)
				{
					if (listDoor[num].accdoor_device_id != deviceID)
					{
						num++;
						continue;
					}
					flag = true;
					break;
				}
				if (flag)
				{
					result = 1;
				}
			}
			return result;
		}

		public static int checkdata(int authorizeDoorId, int deviceID, int employeeID, int levelID, int timezonesID, List<AccLevelsetEmp> listlevelEmp, List<AccLevelsetDoorGroup> listDoor)
		{
			int result = 0;
			bool flag = false;
			if (listDoor != null && listDoor.Count > 0)
			{
				for (int i = 0; i < listDoor.Count; i++)
				{
					if (listDoor[i].accdoor_device_id == deviceID && listDoor[i].accdoor_no_exp == authorizeDoorId)
					{
						if (listDoor[i].level_timeseg_id == timezonesID)
						{
							int num = 0;
							while (num < listlevelEmp.Count)
							{
								if (employeeID != listlevelEmp[num].employee_id || listDoor[i].acclevelset_id != listlevelEmp[num].acclevelset_id)
								{
									num++;
									continue;
								}
								result = 2;
								flag = true;
								break;
							}
						}
						else
						{
							int num2 = 0;
							while (num2 < listlevelEmp.Count)
							{
								if (employeeID != listlevelEmp[num2].employee_id || listDoor[i].acclevelset_id != listlevelEmp[num2].acclevelset_id)
								{
									num2++;
									continue;
								}
								result = 1;
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			return result;
		}

		public static int checkdata(int authorizeDoorId, int deviceID, int employeeID, DataTable dt, int timezonesID, List<AccLevelsetEmp> listlevelEmp, List<AccLevelsetDoorGroup> listDoor)
		{
			int result = 0;
			if (listDoor != null && listDoor.Count > 0)
			{
				for (int i = 0; i < listDoor.Count; i++)
				{
					if (listDoor[i].accdoor_device_id == deviceID && listDoor[i].accdoor_no_exp == authorizeDoorId)
					{
						DataRow[] array = dt.Select(string.Format("acclevelset_id = {0} and employee_id = {1}", new string[2]
						{
							listDoor[i].acclevelset_id.ToString(),
							employeeID.ToString()
						}));
						if (array.Length != 0)
						{
							result = 1;
							if (listDoor[i].level_timeseg_id == timezonesID)
							{
								result = 2;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		public static void UserCmd(AccLevelset model, int OldTimeZoneId, bool isadd, List<Machines> dlist, List<AccLevelsetEmp> users, bool Processing = false)
		{
			AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
			List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
			Dictionary<int, AccTimeseg> dictionary = new Dictionary<int, AccTimeseg>();
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!dictionary.ContainsKey(modelList[i].id))
					{
						dictionary.Add(modelList[i].id, modelList[i]);
					}
				}
			}
			if (dlist != null && dlist.Count > 0)
			{
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				Dictionary<int, UserInfo> dictionary2 = new Dictionary<int, UserInfo>();
				if (users.Count > 10)
				{
					List<UserInfo> modelList2 = userInfoBll.GetModelList("");
					if (modelList2 != null && modelList2.Count > 0)
					{
						for (int j = 0; j < modelList2.Count; j++)
						{
							dictionary2.Add(modelList2[j].UserId, modelList2[j]);
						}
					}
				}
				int level_timeseg_id = model.level_timeseg_id;
				List<ObjUserAuthorize> list = new List<ObjUserAuthorize>();
				List<UserInfo> list2 = new List<UserInfo>();
				if (users != null && users.Count > 0)
				{
					for (int k = 0; k < users.Count; k++)
					{
						UserInfo userInfo = null;
						userInfo = ((!dictionary2.ContainsKey(users[k].employee_id)) ? userInfoBll.GetModel(users[k].employee_id) : dictionary2[users[k].employee_id]);
						if (userInfo != null)
						{
							ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
							objUserAuthorize.AuthorizeDoorId = "0";
							objUserAuthorize.Pin = userInfo.BadgeNumber;
							objUserAuthorize.AuthorizeTimezoneId = level_timeseg_id.ToString();
							if (dictionary.ContainsKey(level_timeseg_id))
							{
								AccTimeseg accTimeseg = dictionary[level_timeseg_id];
								objUserAuthorize.TimeZone1 = accTimeseg.TimeZone1Id;
								objUserAuthorize.TimeZone2 = accTimeseg.TimeZone2Id;
								objUserAuthorize.TimeZone3 = accTimeseg.TimeZone3Id;
							}
							if (DeviceHelper.TimeSeg.id == level_timeseg_id)
							{
								objUserAuthorize.AuthorizeTimezoneId = "1";
							}
							list2.Add(userInfo);
							list.Add(objUserAuthorize);
						}
					}
				}
				AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
				List<AccLevelsetEmp> modelList3 = accLevelsetEmpBll.GetModelList($" acclevelset_id <> {model.id}  order by employee_id  ");
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				List<AccLevelsetDoorGroup> modelList4 = accLevelsetDoorGroupBll.GetModelList($" acclevelset_id <> {model.id} ");
				for (int l = 0; l < dlist.Count; l++)
				{
					List<ObjUserAuthorize> list3 = new List<ObjUserAuthorize>();
					for (int m = 0; m < list.Count; m++)
					{
						ObjUserAuthorize objUserAuthorize2 = new ObjUserAuthorize();
						objUserAuthorize2 = list[m].Copy();
						objUserAuthorize2.AuthorizeTimezoneId = OldTimeZoneId.ToString();
						list3.Add(objUserAuthorize2);
					}
					List<int> list4 = new List<int>();
					List<int> list5 = new List<int>();
					List<UserInfo> list6 = new List<UserInfo>();
					List<AccDoor> modelList5 = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group where acclevelset_id=" + model.id + " ) and device_id=" + dlist[l].ID);
					int num = 0;
					if (modelList5 != null && modelList5.Count > 0)
					{
						for (int n = 0; n < modelList5.Count; n++)
						{
							num += 1 << modelList5[n].door_no - 1;
						}
						for (int num2 = 0; num2 < list.Count; num2++)
						{
							list[num2].AuthorizeDoorId = num.ToString();
							list3[num2].AuthorizeDoorId = num.ToString();
						}
					}
					if (!isadd)
					{
						if (CommandServer.checkdata(dlist[l].ID, model.id, modelList4) == 0)
						{
							if (modelList5 != null && modelList5.Count > 0)
							{
								string empty = string.Empty;
								accDoorBll.IsConnectDb(true);
								for (int num3 = 0; num3 < list2.Count; num3++)
								{
									if (dlist[l].device_type == 102)
									{
										empty = CommandServer.DeleteFingerVeinCmd(list2[num3]);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty);
									}
									else if (dlist[l].device_type == 103 || (dlist[l].DevSDKType == SDKType.StandaloneSDK && dlist[l].FaceFunOn == 1))
									{
										empty = CommandServer.DeleteFaceTempCmd(list2[num3]);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty);
									}
								}
								accDoorBll.IsConnectDb(false);
								string empty2 = string.Empty;
								if (dlist[l].IsOnlyRFMachine == 0 && dlist[l].DevSDKType == SDKType.PullSDK)
								{
									CommandServer.DeleteTemplateCmdEx(dlist[l].ID, list2);
								}
								if (list2.Count < 500)
								{
									empty2 = PullSDKDataConvertHelper.DeleteUser(list2, true);
									CommandServer.SaveCmdInfo(dlist[l].ID, empty2);
								}
								else
								{
									List<UserInfo> list7 = new List<UserInfo>();
									foreach (UserInfo item in list2)
									{
										list7.Add(item);
										if (list7.Count != 0 && list7.Count % 500 == 0)
										{
											empty2 = PullSDKDataConvertHelper.DeleteUser(list7, true);
											CommandServer.SaveCmdInfo(dlist[l].ID, empty2);
											list7.Clear();
										}
									}
									empty2 = PullSDKDataConvertHelper.DeleteUser(list7, true);
									CommandServer.SaveCmdInfo(dlist[l].ID, empty2);
								}
							}
						}
						else if (modelList5 != null && modelList5.Count > 0)
						{
							accDoorBll.IsConnectDb(true);
							for (int num4 = 0; num4 < list2.Count; num4++)
							{
								UserInfo userInfo2 = list2[num4];
								list6.Add(userInfo2);
								int num5 = CommandServer.checkdata(num, dlist[l].ID, userInfo2.UserId, model.id, level_timeseg_id, modelList3, modelList4);
								if (num5 > 0)
								{
									list6.Remove(userInfo2);
									if (num5 == 2)
									{
										list5.Add(int.Parse(userInfo2.BadgeNumber));
									}
								}
								else
								{
									string empty3 = string.Empty;
									if (dlist[l].device_type == 102 && dlist[l].FvFunOn == 1)
									{
										empty3 = CommandServer.DeleteFingerVeinCmd(userInfo2);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty3);
									}
									else if (dlist[l].device_type == 103 || (dlist[l].DevSDKType == SDKType.StandaloneSDK && dlist[l].FaceFunOn == 1))
									{
										empty3 = CommandServer.DeleteFaceTempCmd(userInfo2);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty3);
									}
								}
							}
							accDoorBll.IsConnectDb(false);
							string empty4 = string.Empty;
							if (dlist[l].IsOnlyRFMachine == 0 && dlist[l].device_type != 102 && dlist[l].FvFunOn != 1 && dlist[l].DevSDKType == SDKType.PullSDK)
							{
								CommandServer.DeleteTemplateCmdEx(dlist[l].ID, list6);
							}
							if (list6.Count > 0)
							{
								if (list6.Count < 500)
								{
									empty4 = PullSDKDataConvertHelper.DeleteUser(list6, true);
									CommandServer.SaveCmdInfo(dlist[l].ID, empty4);
								}
								else
								{
									List<UserInfo> list8 = new List<UserInfo>();
									foreach (UserInfo item2 in list6)
									{
										list8.Add(item2);
										if (list8.Count != 0 && list8.Count % 500 == 0)
										{
											empty4 = PullSDKDataConvertHelper.DeleteUser(list8, true);
											CommandServer.SaveCmdInfo(dlist[l].ID, empty4);
											list8.Clear();
										}
									}
									empty4 = PullSDKDataConvertHelper.DeleteUser(list8, true);
									CommandServer.SaveCmdInfo(dlist[l].ID, empty4);
								}
							}
						}
						if (modelList5 != null && modelList5.Count > 0 && num != 0 && list.Count > 0)
						{
							if (list5 != null && list5.Count > 0)
							{
								for (int num6 = list.Count - 1; num6 >= 0; num6--)
								{
									if (list5.Contains(int.Parse(list[num6].Pin)))
									{
										list3.RemoveAt(num6);
									}
								}
							}
							string empty5 = string.Empty;
							if (list3.Count < 500)
							{
								empty5 = PullSDKDataConvertHelper.DeleteUserAuthorize(list3, true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty5);
							}
							else
							{
								List<ObjUserAuthorize> list9 = new List<ObjUserAuthorize>();
								foreach (ObjUserAuthorize item3 in list3)
								{
									list9.Add(item3);
									if (list9.Count != 0 && list9.Count % 500 == 0)
									{
										empty5 = PullSDKDataConvertHelper.DeleteUserAuthorize(list9, true);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty5);
										list9.Clear();
									}
								}
								empty5 = PullSDKDataConvertHelper.DeleteUserAuthorize(list9, true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty5);
							}
						}
					}
					else if (modelList5 != null && modelList5.Count > 0)
					{
						string empty6 = string.Empty;
						if (Processing)
						{
							int num7 = 500;
							List<UserInfo> list10 = new List<UserInfo>();
							for (int num8 = 0; num8 < list2.Count; num8++)
							{
								list10.Add(list2[num8]);
								if (list10.Count >= num7)
								{
									empty6 = PullSDKDataConvertHelper.AddUser(list10, dlist[l], true);
									CommandServer.SaveCmdInfo(dlist[l].ID, empty6);
									list10.Clear();
								}
							}
							if (list10.Count > 0)
							{
								empty6 = PullSDKDataConvertHelper.AddUser(list10, dlist[l], true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty6);
								list10.Clear();
							}
						}
						if (CommandServer.checkdata(dlist[l].ID, model.id, modelList4) > 0)
						{
							accDoorBll.IsConnectDb(true);
							for (int num9 = 0; num9 < list2.Count; num9++)
							{
								UserInfo userInfo3 = list2[num9];
								list6.Add(userInfo3);
								int num10 = CommandServer.checkdata(num, dlist[l].ID, userInfo3.UserId, model.id, level_timeseg_id, modelList3, modelList4);
								if (num10 > 0)
								{
									list6.Remove(userInfo3);
									if (num10 == 2)
									{
										list5.Add(int.Parse(userInfo3.BadgeNumber));
									}
								}
							}
							if (list5 != null && list5.Count > 0)
							{
								for (int num11 = list.Count - 1; num11 >= 0; num11--)
								{
									if (list5.Contains(int.Parse(list[num11].Pin)))
									{
										list3.RemoveAt(num11);
									}
								}
							}
							accDoorBll.IsConnectDb(false);
							string empty7 = string.Empty;
							if (list3.Count < 500)
							{
								empty7 = PullSDKDataConvertHelper.DeleteUserAuthorize(list3, true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty7);
							}
							else
							{
								List<ObjUserAuthorize> list11 = new List<ObjUserAuthorize>();
								foreach (ObjUserAuthorize item4 in list3)
								{
									list11.Add(item4);
									if (list11.Count != 0 && list11.Count % 500 == 0)
									{
										empty7 = PullSDKDataConvertHelper.DeleteUserAuthorize(list11, true);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty7);
										list11.Clear();
									}
								}
								empty7 = PullSDKDataConvertHelper.DeleteUserAuthorize(list11, true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty7);
							}
						}
						else
						{
							string empty8 = string.Empty;
							if (list3.Count < 500)
							{
								empty8 = PullSDKDataConvertHelper.DeleteUserAuthorize(list3, true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty8);
							}
							else
							{
								List<ObjUserAuthorize> list12 = new List<ObjUserAuthorize>();
								foreach (ObjUserAuthorize item5 in list3)
								{
									list12.Add(item5);
									if (list12.Count != 0 && list12.Count % 500 == 0)
									{
										empty8 = PullSDKDataConvertHelper.DeleteUserAuthorize(list12, true);
										CommandServer.SaveCmdInfo(dlist[l].ID, empty8);
										list12.Clear();
									}
								}
								empty8 = PullSDKDataConvertHelper.DeleteUserAuthorize(list12, true);
								CommandServer.SaveCmdInfo(dlist[l].ID, empty8);
							}
						}
						if (dlist[l].DevSDKType == SDKType.StandaloneSDK)
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append("update|UserAuthorize$");
							for (int num12 = 0; num12 < list.Count; num12++)
							{
								stringBuilder.Append(list[l].ToPullCmdString(dlist[l]) + "#");
							}
							empty6 = stringBuilder.ToString();
						}
						else
						{
							empty6 = PullSDKDataConvertHelper.AddUserAuthorize(list, true);
						}
						CommandServer.SaveCmdInfo(dlist[l].ID, empty6);
						if (Processing)
						{
							if (dlist[l].device_type == 102 && dlist[l].FvFunOn == 1)
							{
								int num13 = 500;
								List<UserInfo> list13 = new List<UserInfo>();
								for (int num14 = 0; num14 < list2.Count; num14++)
								{
									list13.Add(list2[num14]);
									if (list13.Count >= num13)
									{
										string fingerVeinCmd = CommandServer.GetFingerVeinCmd(list13);
										if (!string.IsNullOrEmpty(fingerVeinCmd))
										{
											CommandServer.SaveCmdInfo(dlist[l].ID, fingerVeinCmd);
										}
										list13.Clear();
									}
								}
								if (list13.Count > 0)
								{
									string fingerVeinCmd = CommandServer.GetFingerVeinCmd(list2);
									if (!string.IsNullOrEmpty(fingerVeinCmd))
									{
										CommandServer.SaveCmdInfo(dlist[l].ID, fingerVeinCmd);
									}
									list13.Clear();
								}
							}
							else if (dlist[l].device_type == 103 || (dlist[l].DevSDKType == SDKType.StandaloneSDK && dlist[l].FaceFunOn == 1))
							{
								int num15 = 500;
								List<UserInfo> list14 = new List<UserInfo>();
								for (int num16 = 0; num16 < list2.Count; num16++)
								{
									list14.Add(list2[num16]);
									if (list14.Count >= num15)
									{
										string faceTempCmd = CommandServer.GetFaceTempCmd(list14);
										if (!string.IsNullOrEmpty(faceTempCmd))
										{
											CommandServer.SaveCmdInfo(dlist[l].ID, faceTempCmd);
										}
										list14.Clear();
									}
								}
								if (list14.Count > 0)
								{
									string faceTempCmd = CommandServer.GetFaceTempCmd(list2);
									if (!string.IsNullOrEmpty(faceTempCmd))
									{
										CommandServer.SaveCmdInfo(dlist[l].ID, faceTempCmd);
									}
									list14.Clear();
								}
							}
							if (dlist[l].IsOnlyRFMachine == 0)
							{
								int num17 = 500;
								List<UserInfo> list15 = new List<UserInfo>();
								for (int num18 = 0; num18 < list2.Count; num18++)
								{
									list15.Add(list2[num18]);
									if (list15.Count >= num17)
									{
										string templateCmd = CommandServer.GetTemplateCmd(list15);
										if (!string.IsNullOrEmpty(templateCmd))
										{
											CommandServer.SaveCmdInfo(dlist[l].ID, templateCmd);
										}
										list15.Clear();
									}
								}
								if (list15.Count > 0)
								{
									string templateCmd = CommandServer.GetTemplateCmd(list2);
									if (!string.IsNullOrEmpty(templateCmd))
									{
										CommandServer.SaveCmdInfo(dlist[l].ID, templateCmd);
									}
									list15.Clear();
								}
							}
						}
					}
				}
			}
		}

		private static void InitDoor()
		{
			try
			{
				CommandServer.doorlist.Clear();
				List<AccDoor> modelList = CommandServer.doorbll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!CommandServer.doorlist.ContainsKey(modelList[i].id))
						{
							CommandServer.doorlist.Add(modelList[i].id, modelList[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public static void DelCmd(AccFirstOpen model)
		{
			CommandServer.InitDoor();
			if (model != null && CommandServer.doorlist.ContainsKey(model.door_id))
			{
				string cmdContent = PullSDKDataConvertHelper.DeleteFirstCard(CommandServer.doorlist[model.door_id].door_no, model.timeseg_id, true);
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				DevCmds devCmds = new DevCmds();
				devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
				cmdContent = $"setFirstCardState$Door{CommandServer.doorlist[model.door_id].door_no}FirstCardOpenDoor=0";
				cmdContent.Remove(cmdContent.Length - 1);
				devCmds = new DevCmds();
				devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
			}
		}

		public static void DelCmd(AccFirstOpen model, List<int> userList)
		{
			CommandServer.InitDoor();
			if (model != null && CommandServer.doorlist.ContainsKey(model.door_id))
			{
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				Dictionary<int, string> userId_BadgenumberDic = userInfoBll.GetUserId_BadgenumberDic("");
				List<ObjFirstCard> list = new List<ObjFirstCard>();
				for (int i = 0; i < userList.Count; i++)
				{
					ObjFirstCard objFirstCard = new ObjFirstCard();
					ObjFirstCard objFirstCard2 = objFirstCard;
					int num = model.timeseg_id;
					objFirstCard2.TimezoneID = num.ToString();
					ObjFirstCard objFirstCard3 = objFirstCard;
					num = CommandServer.doorlist[model.door_id].door_no;
					objFirstCard3.DoorID = num.ToString();
					if (userId_BadgenumberDic.ContainsKey(userList[i]))
					{
						objFirstCard.Pin = userId_BadgenumberDic[userList[i]];
						list.Add(objFirstCard);
					}
				}
				string cmdContent = PullSDKDataConvertHelper.DeleteFirstCard(list, true);
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				DevCmds devCmds = new DevCmds();
				devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
				devCmds.status = 0;
				devCmds.CmdContent = cmdContent;
				devCmds.CmdCommitTime = DateTime.Now;
				devCmds.CmdReturnContent = string.Empty;
				devCmdsBll.Add(devCmds);
			}
		}

		public static void AddCmd(AccFirstOpen model, List<AccFirstOpenEmp> addlist)
		{
			if (model != null)
			{
				CommandServer.InitDoor();
				if (CommandServer.doorlist.ContainsKey(model.door_id))
				{
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					Dictionary<int, string> userId_BadgenumberDic = userInfoBll.GetUserId_BadgenumberDic("");
					List<ObjFirstCard> list = new List<ObjFirstCard>();
					for (int i = 0; i < addlist.Count; i++)
					{
						ObjFirstCard objFirstCard = new ObjFirstCard();
						ObjFirstCard objFirstCard2 = objFirstCard;
						int num = model.timeseg_id;
						objFirstCard2.TimezoneID = num.ToString();
						ObjFirstCard objFirstCard3 = objFirstCard;
						num = CommandServer.doorlist[model.door_id].door_no;
						objFirstCard3.DoorID = num.ToString();
						if (userId_BadgenumberDic.ContainsKey(addlist[i].employee_id))
						{
							objFirstCard.Pin = userId_BadgenumberDic[addlist[i].employee_id];
							list.Add(objFirstCard);
						}
					}
					string cmdContent = PullSDKDataConvertHelper.AddFirstCard(list, true);
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static void AddCmd(AccFirstOpen model, List<ObjFirstCard> firslist)
		{
			if (model != null)
			{
				CommandServer.InitDoor();
				if (CommandServer.doorlist.ContainsKey(model.door_id))
				{
					string cmdContent = PullSDKDataConvertHelper.AddFirstCard(firslist, true);
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
					cmdContent = $"setFirstCardState$Door{CommandServer.doorlist[model.door_id].door_no}FirstCardOpenDoor=1";
					cmdContent.Remove(cmdContent.Length - 1);
					devCmds = new DevCmds();
					devCmds.SN_id = CommandServer.doorlist[model.door_id].device_id;
					devCmds.status = 0;
					devCmds.CmdContent = cmdContent;
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static void AddAllUnlockGroupCmd(Machines machine)
		{
			List<ObjMultimCard> allUnlockGroup = CommandServer.GetAllUnlockGroup(machine);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update|multimcard$");
			if (allUnlockGroup != null && allUnlockGroup.Count > 0)
			{
				for (int i = 0; i < allUnlockGroup.Count; i++)
				{
					ObjMultimCard model = allUnlockGroup[i];
					stringBuilder.Append(PullSDKDataConvert.AddMultimCard(model));
				}
			}
			CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
		}

		public static List<ObjMultimCard> GetAllUnlockGroup(Machines machine)
		{
			AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
			List<AccMorecardset> modelList = accMorecardsetBll.GetModelList("door_id in (select id from acc_door where device_id=" + machine.ID + ")");
			AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
			List<AccMorecardempGroup> modelList2 = accMorecardempGroupBll.GetModelList("");
			modelList2 = ((modelList2 == null) ? new List<AccMorecardempGroup>() : modelList2);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < modelList2.Count; i++)
			{
				if (!dictionary.ContainsKey(modelList2[i].id))
				{
					dictionary.Add(modelList2[i].id, modelList2[i].StdGroupNo);
				}
			}
			List<ObjMultimCard> list = new List<ObjMultimCard>();
			if (machine.DevSDKType == SDKType.StandaloneSDK)
			{
				ObjMultimCard objMultimCard = new ObjMultimCard();
				objMultimCard.DoorId = "1";
				objMultimCard.Group1 = "1";
				objMultimCard.Group2 = "0";
				objMultimCard.Group3 = "0";
				objMultimCard.Group4 = "0";
				objMultimCard.Group5 = "0";
				objMultimCard.Index = "1";
				list.Add(objMultimCard);
			}
			if (modelList != null || modelList.Count > 0)
			{
				for (int j = 0; j < modelList.Count; j++)
				{
					ObjMultimCard objMultimCard = DeviceHelper.MultimCardConvert(modelList[j], machine);
					int.TryParse(objMultimCard.Group1, out int num);
					int.TryParse(objMultimCard.Group2, out int num2);
					int.TryParse(objMultimCard.Group3, out int num3);
					int.TryParse(objMultimCard.Group4, out int num4);
					int.TryParse(objMultimCard.Group5, out int num5);
					ObjMultimCard objMultimCard2 = objMultimCard;
					int num6 = (num > 0 && dictionary.ContainsKey(num)) ? dictionary[num] : 0;
					objMultimCard2.Group1 = num6.ToString();
					ObjMultimCard objMultimCard3 = objMultimCard;
					num6 = ((num2 > 0 && dictionary.ContainsKey(num2)) ? dictionary[num2] : 0);
					objMultimCard3.Group2 = num6.ToString();
					ObjMultimCard objMultimCard4 = objMultimCard;
					num6 = ((num3 > 0 && dictionary.ContainsKey(num3)) ? dictionary[num3] : 0);
					objMultimCard4.Group3 = num6.ToString();
					ObjMultimCard objMultimCard5 = objMultimCard;
					num6 = ((num4 > 0 && dictionary.ContainsKey(num4)) ? dictionary[num4] : 0);
					objMultimCard5.Group4 = num6.ToString();
					ObjMultimCard objMultimCard6 = objMultimCard;
					num6 = ((num5 > 0 && dictionary.ContainsKey(num5)) ? dictionary[num5] : 0);
					objMultimCard6.Group5 = num6.ToString();
					list.Add(objMultimCard);
				}
			}
			return list;
		}

		public static Group ConvertGroup(Machines machine, AccMorecardempGroup model)
		{
			List<AccMorecardempGroup> list = new List<AccMorecardempGroup>();
			list.Add(model);
			List<Group> list2 = CommandServer.ConvertGroup(machine, list);
			if (list2 != null && list2.Count > 0)
			{
				return list2[0];
			}
			return new Group();
		}

		public static List<Group> ConvertGroup(Machines machine, List<AccMorecardempGroup> lstModel)
		{
			List<Group> list = new List<Group>();
			if (lstModel == null || lstModel.Count <= 0)
			{
				return list;
			}
			AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
			List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					AccTimeseg accTimeseg = modelList[i];
					if (!dictionary.ContainsKey(accTimeseg.id))
					{
						List<int> list2 = new List<int>();
						list2.Add(accTimeseg.TimeZone1Id);
						list2.Add(accTimeseg.TimeZone2Id);
						list2.Add(accTimeseg.TimeZone3Id);
						dictionary.Add(accTimeseg.id, list2);
					}
				}
			}
			for (int j = 0; j < lstModel.Count; j++)
			{
				AccMorecardempGroup accMorecardempGroup = lstModel[j];
				Group group = new Group();
				group.GroupNo = accMorecardempGroup.StdGroupNo;
				group.ValidOnHoliday = (accMorecardempGroup.StdValidOnHoliday ? 1 : 0);
				group.VerifyMode = accMorecardempGroup.StdGroupVT;
				if (dictionary.ContainsKey(accMorecardempGroup.StdGroupTz))
				{
					List<int> list2 = dictionary[accMorecardempGroup.StdGroupTz];
					group.TimeZoneId1 = list2[0];
					group.TimeZoneId2 = list2[1];
					group.TimeZoneId3 = list2[2];
				}
				list.Add(group);
			}
			return list;
		}

		public static void AddGroup(Machines machine, List<Group> lstModel)
		{
			if (lstModel != null && lstModel.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < lstModel.Count; i++)
				{
					if (machine.DevSDKType != SDKType.StandaloneSDK || lstModel[i].GroupNo > 0)
					{
						stringBuilder.Append(lstModel[i].ToPullCmdString(machine) + "#");
					}
				}
				if (stringBuilder.Length > 0)
				{
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = machine.ID;
					devCmds.status = 0;
					devCmds.CmdContent = "update|group$" + stringBuilder.ToString();
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
				}
			}
		}

		public static void AddGroup(Machines machine, Group model)
		{
			List<Group> list = new List<Group>();
			list.Add(model);
			CommandServer.AddGroup(machine, list);
		}

		public static void SetUserGroup(List<UserInfo> lstUser, List<Machines> lstMachine, Dictionary<int, AccMorecardempGroup> dicGroupId_Group)
		{
			if (lstUser != null && lstUser.Count > 0 && lstMachine != null && lstMachine.Count > 0)
			{
				if (dicGroupId_Group == null)
				{
					dicGroupId_Group = new Dictionary<int, AccMorecardempGroup>();
				}
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("update|usergroup$");
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				UserInfo[] array = new UserInfo[lstUser.Count];
				lstUser.CopyTo(array);
				for (int i = 0; i < array.Length; i++)
				{
					if (dicGroupId_Group.ContainsKey(array[i].MorecardGroupId))
					{
						array[i].MorecardGroupId = dicGroupId_Group[array[i].MorecardGroupId].StdGroupNo;
						if (array[i].MorecardGroupId < 1 || array[i].MorecardGroupId > 99)
						{
							array[i].MorecardGroupId = 1;
						}
					}
					else
					{
						array[i].MorecardGroupId = 1;
					}
				}
				for (int j = 0; j < lstMachine.Count; j++)
				{
					Machines machines = lstMachine[j];
					for (int k = 0; k < array.Length; k++)
					{
						stringBuilder.Append(array[k].ToPullCmdString(machines) + "#");
						if (k > 0 && k % num == 0)
						{
							DevCmds devCmds = new DevCmds();
							devCmds.SN_id = machines.ID;
							devCmds.status = 0;
							devCmds.CmdContent = stringBuilder.ToString();
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							devCmdsBll.Add(devCmds);
							stringBuilder = new StringBuilder();
							stringBuilder.AppendFormat("update|usergroup$");
						}
					}
					if (stringBuilder != null && stringBuilder.Length > 0)
					{
						DevCmds devCmds = new DevCmds();
						devCmds.SN_id = machines.ID;
						devCmds.status = 0;
						devCmds.CmdContent = stringBuilder.ToString();
						devCmds.CmdCommitTime = DateTime.Now;
						devCmds.CmdReturnContent = string.Empty;
						devCmdsBll.Add(devCmds);
					}
				}
			}
		}

		public static void SetUserGroup(UserInfo User, List<Machines> lstMachine, Dictionary<int, AccMorecardempGroup> dicPullGroup_StdGroup)
		{
			List<UserInfo> list = new List<UserInfo>();
			list.Add(User);
			CommandServer.SetUserGroup(list, lstMachine, dicPullGroup_StdGroup);
		}

		public static void AddUser(Machines machine, List<UserInfo> lstUser, Dictionary<int, AccMorecardempGroup> dicId_EmpGroup)
		{
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|user$");
				dicId_EmpGroup = (dicId_EmpGroup ?? new Dictionary<int, AccMorecardempGroup>());
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo;
					if (machine.DevSDKType == SDKType.StandaloneSDK)
					{
						userInfo = lstUser[i].Copy();
						if (dicId_EmpGroup.ContainsKey(userInfo.MorecardGroupId))
						{
							userInfo.MorecardGroupId = dicId_EmpGroup[userInfo.MorecardGroupId].StdGroupNo;
						}
						else
						{
							userInfo.MorecardGroupId = 1;
						}
					}
					else
					{
						userInfo = lstUser[i];
					}
					stringBuilder.Append(userInfo.ToPullCmdString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("update|user$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddElevator(Machines machine, List<UserInfo> lstUser, Dictionary<int, AccMorecardempGroup> dicId_EmpGroup)
		{
			if (MainForm._elevatorEnabled && machine != null && lstUser != null && lstUser.Count > 0)
			{
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|elevator$");
				dicId_EmpGroup = (dicId_EmpGroup ?? new Dictionary<int, AccMorecardempGroup>());
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					stringBuilder.Append(userInfo.ToPullCmdElevatorString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("update|elevator$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void DeleteUser(Machines machine, List<UserInfo> lstUser, bool SaveCmd, out string cmdInfo)
		{
			cmdInfo = "";
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				if (SaveCmd)
				{
					stringBuilder.Append("delete|user$");
				}
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					stringBuilder.Append("Pin=" + userInfo.BadgeNumber + "#");
					if (SaveCmd && i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("delete|user$");
					}
				}
				cmdInfo = stringBuilder.ToString();
				if (SaveCmd && stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddFingerPrint(Machines machine, List<Template> lstFP, Dictionary<int, UserInfo> dicUserId_User)
		{
			if (machine != null && lstFP != null && lstFP.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FingerFunOn == null || machine.FingerFunOn.Trim() != "1")
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.CardFun == 1)
						{
							return;
						}
					}
					else if (machine.IsOnlyRFMachine != 0 || machine.FvFunOn == 1)
					{
						return;
					}
				}
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|templatev10$");
				dicUserId_User = (dicUserId_User ?? new Dictionary<int, UserInfo>());
				for (int i = 0; i < lstFP.Count; i++)
				{
					Template template = lstFP[i];
					if (dicUserId_User.ContainsKey(template.USERID))
					{
						template.Pin = dicUserId_User[template.USERID].BadgeNumber;
					}
					stringBuilder.Append(template.ToPullCmdString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("update|templatev10$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddFingerPrint(Machines machine, List<UserInfo> lstUser)
		{
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FingerFunOn == null || machine.FingerFunOn.Trim() != "1")
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.CardFun == 1)
						{
							return;
						}
					}
					else if (machine.IsOnlyRFMachine != 0 || machine.FvFunOn == 1)
					{
						return;
					}
				}
				int num = 0;
				int num2 = 500;
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("USERID in(");
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("update|templatev10$");
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					stringBuilder.AppendFormat("{0},", userInfo.UserId);
					if (!dictionary.ContainsKey(userInfo.UserId))
					{
						dictionary.Add(userInfo.UserId, userInfo);
					}
					if (i != 0 && i % num2 == 0)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Remove(stringBuilder.Length - 1, 1);
							stringBuilder.Append(")");
							List<Template> modelList = templateBll.GetModelList(stringBuilder.ToString());
							for (int j = 0; j < modelList.Count; j++)
							{
								Template template = modelList[j];
								if (dictionary.ContainsKey(template.USERID))
								{
									template.Pin = dictionary[template.USERID].BadgeNumber;
								}
								stringBuilder2.Append(template.ToPullCmdString(machine) + "#");
								num++;
								if (num != 0 && num % num2 == 0)
								{
									CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
									num = 0;
									stringBuilder2 = new StringBuilder();
									stringBuilder2.Append("update|templatev10$");
								}
							}
						}
						stringBuilder = new StringBuilder();
						stringBuilder.Append("USERID in(");
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					stringBuilder.Append(")");
					if (stringBuilder.ToString().Contains("("))
					{
						List<Template> modelList = templateBll.GetModelList(stringBuilder.ToString());
						for (int k = 0; k < modelList.Count; k++)
						{
							Template template = modelList[k];
							if (dictionary.ContainsKey(template.USERID))
							{
								template.Pin = dictionary[template.USERID].BadgeNumber;
							}
							stringBuilder2.Append(template.ToPullCmdString(machine) + "#");
							num++;
							if (num != 0 && num % num2 == 0)
							{
								CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
								num = 0;
								stringBuilder2 = new StringBuilder();
								stringBuilder2.Append("update|templatev10$");
							}
						}
					}
				}
				if (num != 0 && stringBuilder2.Length > 0)
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
				}
			}
		}

		public static void DeleteFingerPrint(Machines machine, List<Template> lstFP, Dictionary<int, UserInfo> dicUserId_User)
		{
			if (machine != null && lstFP != null && lstFP.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FingerFunOn == null || machine.FingerFunOn.Trim() != "1")
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.CardFun == 1)
						{
							return;
						}
					}
					else if (machine.IsOnlyRFMachine != 0 || machine.FvFunOn == 1)
					{
						return;
					}
				}
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("delete|templatev10$");
				dicUserId_User = (dicUserId_User ?? new Dictionary<int, UserInfo>());
				for (int i = 0; i < lstFP.Count; i++)
				{
					Template template = lstFP[i];
					if (dicUserId_User.ContainsKey(template.USERID))
					{
						template.Pin = dicUserId_User[template.USERID].BadgeNumber;
					}
					stringBuilder.Append(template.ToPullCmdString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("delete|templatev10$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void DeleteFingerPrint(Machines machine, List<UserInfo> lstUser, bool SaveCmd, out string cmdInfo)
		{
			cmdInfo = "";
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FingerFunOn == null || machine.FingerFunOn.Trim() != "1")
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.CardFun == 1)
						{
							return;
						}
					}
					else if (machine.IsOnlyRFMachine != 0 || machine.FvFunOn == 1)
					{
						return;
					}
				}
				int num = 0;
				int num2 = 500;
				StringBuilder stringBuilder = new StringBuilder();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				if (SaveCmd)
				{
					TemplateBll templateBll = new TemplateBll(MainForm._ia);
					dictionary = templateBll.GetUserId_TemplateCountDic((machine.FpVersion == 9) ? "not TEMPLATE3 is null" : "not TEMPLATE4 is null");
					stringBuilder.Append("delete|templatev10$");
				}
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					if (!SaveCmd || dictionary.ContainsKey(userInfo.UserId))
					{
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							stringBuilder.AppendFormat("Pin={0}\tFingerID={1}#", userInfo.BadgeNumber, 13);
						}
						else
						{
							stringBuilder.AppendFormat("Pin={0}#", userInfo.BadgeNumber);
						}
						num++;
						if (SaveCmd && num != 0 && num % num2 == 0)
						{
							CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
							stringBuilder = new StringBuilder();
							stringBuilder.Append("delete|templatev10$");
						}
					}
				}
				cmdInfo = stringBuilder.ToString();
				if (SaveCmd && stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddFingerVein(Machines machine, List<UserInfo> lstUser)
		{
			if (machine != null && machine.DevSDKType != SDKType.StandaloneSDK && lstUser != null && lstUser.Count > 0 && machine.FvFunOn == 1)
			{
				int num = 0;
				int num2 = 198;
				FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("USERID in(");
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("update|fvtemplate$");
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					stringBuilder.AppendFormat("{0},", userInfo.UserId);
					if (!dictionary.ContainsKey(userInfo.UserId))
					{
						dictionary.Add(userInfo.UserId, userInfo);
					}
					if ((i + 1) % num2 == 0)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Remove(stringBuilder.Length - 1, 1);
							stringBuilder.Append(") order by USERID,FingerID");
							List<FingerVein> modelList = fingerVeinBll.GetModelList(stringBuilder.ToString());
							for (int j = 0; j < modelList.Count; j++)
							{
								FingerVein fingerVein = modelList[j];
								if (dictionary.ContainsKey(fingerVein.UserID))
								{
									fingerVein.Pin = dictionary[fingerVein.UserID].BadgeNumber;
								}
								stringBuilder2.Append(fingerVein.ToPullCmdString(machine) + "#");
								num++;
								if (num != 0 && num % num2 == 0)
								{
									CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
									num = 0;
									stringBuilder2 = new StringBuilder();
									stringBuilder2.Append("update|fvtemplate$");
								}
							}
							if (num != 0)
							{
								CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
								num = 0;
								stringBuilder2 = new StringBuilder();
								stringBuilder2.Append("update|fvtemplate$");
							}
						}
						stringBuilder = new StringBuilder();
						stringBuilder.Append("USERID in(");
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					stringBuilder.Append(")");
					if (stringBuilder.ToString().Contains("("))
					{
						List<FingerVein> modelList = fingerVeinBll.GetModelList(stringBuilder.ToString());
						for (int k = 0; k < modelList.Count; k++)
						{
							FingerVein fingerVein = modelList[k];
							if (dictionary.ContainsKey(fingerVein.UserID))
							{
								fingerVein.Pin = dictionary[fingerVein.UserID].BadgeNumber;
							}
							stringBuilder2.Append(fingerVein.ToPullCmdString(machine) + "#");
							num++;
							if (num != 0 && num % num2 == 0)
							{
								CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
								num = 0;
								stringBuilder2 = new StringBuilder();
								stringBuilder2.Append("update|fvtemplate$");
							}
						}
					}
				}
				if (num != 0 && stringBuilder2.Length > 0)
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
				}
			}
		}

		public static void DeleteFingerVein(Machines machine, List<FingerVein> lstFV, Dictionary<int, UserInfo> dicUserId_User)
		{
			if (machine != null && lstFV != null && lstFV.Count > 0 && machine.FvFunOn == 1)
			{
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("delete|fvtemplate$");
				dicUserId_User = (dicUserId_User ?? new Dictionary<int, UserInfo>());
				for (int i = 0; i < lstFV.Count; i++)
				{
					FingerVein fingerVein = lstFV[i];
					if (dicUserId_User.ContainsKey(fingerVein.UserID))
					{
						fingerVein.Pin = dicUserId_User[fingerVein.UserID].BadgeNumber;
					}
					stringBuilder.Append(fingerVein.ToPullCmdString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("delete|fvtemplate$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void DeleteFingerVein(Machines machine, List<UserInfo> lstUser, bool SaveCmd, out string cmdInfo)
		{
			cmdInfo = "";
			if (machine != null && machine.DevSDKType != SDKType.StandaloneSDK && lstUser != null && lstUser.Count > 0 && machine.FvFunOn == 1)
			{
				int num = 0;
				int num2 = 500;
				StringBuilder stringBuilder = new StringBuilder();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				if (SaveCmd)
				{
					FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
					dictionary = fingerVeinBll.GetUserId_FingerVeinCountDic();
					stringBuilder.Append("delete|fvtemplate$");
				}
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					if (!SaveCmd || dictionary.ContainsKey(userInfo.UserId))
					{
						stringBuilder.AppendFormat("PIN2={0}#", userInfo.BadgeNumber);
						num++;
						if (SaveCmd && num != 0 && num % num2 == 0)
						{
							CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
							stringBuilder = new StringBuilder();
							stringBuilder.Append("delete|fvtemplate$");
						}
					}
				}
				cmdInfo = stringBuilder.ToString();
				if (SaveCmd && stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddFace(Machines machine, List<FaceTemp> lstFace, Dictionary<int, UserInfo> dicUserId_User)
		{
			if (machine != null && lstFace != null && lstFace.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FaceFunOn != 1)
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.FaceFunOn != 1)
						{
							return;
						}
					}
					else
					{
						int device_type = machine.device_type;
						if (device_type != 103)
						{
							return;
						}
					}
				}
				int num = 500;
				int num2 = 0;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|ssrface7$");
				dicUserId_User = (dicUserId_User ?? new Dictionary<int, UserInfo>());
				for (int i = 0; i < lstFace.Count; i++)
				{
					FaceTemp faceTemp = lstFace[i];
					if (dicUserId_User.ContainsKey(faceTemp.UserId))
					{
						faceTemp.Pin = dicUserId_User[faceTemp.UserId].BadgeNumber;
					}
					stringBuilder.Append(faceTemp.ToPullCmdString(machine) + "#");
					num2++;
				}
				CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				stringBuilder = new StringBuilder();
				stringBuilder.Append("update|ssrface7$");
			}
		}

		public static void AddFace(Machines machine, List<UserInfo> lstUser)
		{
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FaceFunOn != 1)
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.FaceFunOn != 1)
						{
							return;
						}
					}
					else
					{
						int device_type = machine.device_type;
						if (device_type != 103)
						{
							return;
						}
					}
				}
				int num = 0;
				int num2 = 500;
				int num3 = 0;
				int num4 = 150;
				ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("UserNo in(");
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("update|ssrface7$");
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					stringBuilder.AppendFormat("'{0}',", userInfo.UserId);
					if (!dictionary.ContainsKey(userInfo.UserId))
					{
						dictionary.Add(userInfo.UserId, userInfo);
					}
					if (i != 0 && i % num2 == 0)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Remove(stringBuilder.Length - 1, 1);
							stringBuilder.Append(") order by USERNO,FACEID");
							List<FaceTemp> modelList = faceTempBll.GetModelList(stringBuilder.ToString());
							for (int j = 0; j < modelList.Count; j++)
							{
								num++;
								FaceTemp faceTemp = modelList[j];
								if (dictionary.ContainsKey(faceTemp.UserId))
								{
									faceTemp.Pin = dictionary[faceTemp.UserId].BadgeNumber;
								}
								if (faceTemp.FaceId == 50)
								{
									List<ObjFaceTemp> list = FaceDataConverter.DBFace2PullFace(faceTemp);
									for (int k = 0; k < list.Count; k++)
									{
										stringBuilder2.Append(list[k].ToPullCmdString(machine) + "#");
									}
									num3 += list.Count;
									if (machine.DevSDKType != SDKType.StandaloneSDK && num3 >= num4)
									{
										num3 = 0;
										CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
										num = 0;
										stringBuilder2.Clear();
										stringBuilder2.Append("update|ssrface7$");
									}
								}
								else
								{
									stringBuilder2.Append(faceTemp.ToPullCmdString(machine) + "#");
									num3++;
									if (machine.DevSDKType != SDKType.StandaloneSDK && num3 >= num4 && j < modelList.Count && modelList[j + 1].Pin != faceTemp.Pin)
									{
										num3 = 0;
										CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
										num = 0;
										stringBuilder2.Clear();
										stringBuilder2.Append("update|ssrface7$");
									}
								}
							}
							if (num > 0)
							{
								num3 = 0;
								CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
								num = 0;
								stringBuilder2 = new StringBuilder();
								stringBuilder2.Append("update|ssrface7$");
							}
						}
						stringBuilder = new StringBuilder();
						stringBuilder.Append("UserNo in(");
					}
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					stringBuilder.Append(") order by USERNO,FACEID");
					if (stringBuilder.ToString().Contains("("))
					{
						List<FaceTemp> modelList = faceTempBll.GetModelList(stringBuilder.ToString());
						for (int l = 0; l < modelList.Count; l++)
						{
							num++;
							FaceTemp faceTemp = modelList[l];
							if (dictionary.ContainsKey(faceTemp.UserId))
							{
								faceTemp.Pin = dictionary[faceTemp.UserId].BadgeNumber;
							}
							if (faceTemp.FaceId == 50)
							{
								List<ObjFaceTemp> list2 = FaceDataConverter.DBFace2PullFace(faceTemp);
								for (int m = 0; m < list2.Count; m++)
								{
									stringBuilder2.Append(list2[m].ToPullCmdString(machine) + "#");
								}
								num3 += list2.Count;
								if (machine.DevSDKType != SDKType.StandaloneSDK && num3 >= num4)
								{
									num3 = 0;
									CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
									num = 0;
									stringBuilder2.Clear();
									stringBuilder2.Append("update|ssrface7$");
								}
							}
							else
							{
								stringBuilder2.Append(faceTemp.ToPullCmdString(machine) + "#");
								num3++;
								if (machine.DevSDKType != SDKType.StandaloneSDK && num3 >= num4 && l < modelList.Count && modelList[l + 1].Pin != faceTemp.Pin)
								{
									num3 = 0;
									CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
									num = 0;
									stringBuilder2.Clear();
									stringBuilder2.Append("update|ssrface7$");
								}
							}
						}
						if (num > 0)
						{
							num3 = 0;
							CommandServer.SaveCmdInfo(machine.ID, stringBuilder2.ToString());
							num = 0;
							stringBuilder2 = new StringBuilder();
							stringBuilder2.Append("update|ssrface7$");
						}
					}
				}
			}
		}

		public static void DeleteFace(Machines machine, List<FaceTemp> lstFace, Dictionary<int, UserInfo> dicUserId_User)
		{
			if (machine != null && lstFace != null && lstFace.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FaceFunOn != 1)
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.FaceFunOn != 1)
						{
							return;
						}
					}
					else
					{
						int device_type = machine.device_type;
						if (device_type != 103)
						{
							return;
						}
					}
				}
				int num = 100;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("delete|ssrface7$");
				dicUserId_User = (dicUserId_User ?? new Dictionary<int, UserInfo>());
				for (int i = 0; i < lstFace.Count; i++)
				{
					FaceTemp faceTemp = lstFace[i];
					if (dicUserId_User.ContainsKey(faceTemp.UserId))
					{
						faceTemp.Pin = dicUserId_User[faceTemp.UserId].BadgeNumber;
					}
					stringBuilder.Append(faceTemp.ToPullCmdString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("delete|ssrface7$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void DeleteFace(Machines machine, List<UserInfo> lstUser, bool SaveCmd, out string cmdInfo)
		{
			cmdInfo = "";
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				if (machine.CompatOldFirmware == "0")
				{
					if (machine.FaceFunOn != 1)
					{
						return;
					}
				}
				else
				{
					SDKType devSDKType = machine.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						if (machine.FaceFunOn != 1)
						{
							return;
						}
					}
					else
					{
						int device_type = machine.device_type;
						if (device_type != 103)
						{
							return;
						}
					}
				}
				int num = 0;
				int num2 = 500;
				StringBuilder stringBuilder = new StringBuilder();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				if (SaveCmd)
				{
					ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
					dictionary = faceTempBll.GetUserId_FaceTempCountDic("");
					stringBuilder.Append("delete|ssrface7$");
				}
				for (int i = 0; i < lstUser.Count; i++)
				{
					UserInfo userInfo = lstUser[i];
					if (!SaveCmd || dictionary.ContainsKey(userInfo.UserId))
					{
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							stringBuilder.AppendFormat("Pin={0}\tFaceID={1}#", userInfo.BadgeNumber, 50);
						}
						else
						{
							stringBuilder.AppendFormat("Pin={0}#", userInfo.BadgeNumber);
						}
						num++;
						if (SaveCmd && num != 0 && num % num2 == 0)
						{
							CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
							stringBuilder = new StringBuilder();
							stringBuilder.Append("delete|ssrface7$");
						}
					}
				}
				cmdInfo = stringBuilder.ToString();
				if (SaveCmd && stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddUserPhoto(Machines machine, List<UserInfo> lstUser)
		{
		}

		public static void AddUserAuthorize(Machines machine, AccTimeseg timeseg, List<UserInfo> lstUser, List<AccDoor> lstDoorPermited)
		{
			if (machine != null && timeseg != null && lstUser != null && lstUser.Count > 0 && lstDoorPermited != null && lstDoorPermited.Count > 0)
			{
				int num = 0;
				int num2 = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|userauthorize$");
				for (int i = 0; i < lstDoorPermited.Count; i++)
				{
					num |= 1 << lstDoorPermited[i].door_no - 1;
				}
				ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
				objUserAuthorize.Pin = "1";
				objUserAuthorize.AuthorizeDoorId = num.ToString();
				objUserAuthorize.TimeZone1 = timeseg.TimeZone1Id;
				objUserAuthorize.TimeZone2 = timeseg.TimeZone2Id;
				objUserAuthorize.TimeZone3 = timeseg.TimeZone3Id;
				objUserAuthorize.AuthorizeTimezoneId = ((timeseg.id == DeviceHelper.TimeSeg.id) ? 1 : timeseg.id).ToString();
				for (int j = 0; j < lstUser.Count; j++)
				{
					objUserAuthorize.Pin = lstUser[j].BadgeNumber;
					stringBuilder.Append(objUserAuthorize.ToPullCmdString(machine) + "#");
					if (j != 0 && j % num2 == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("update|userauthorize$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void DeleteUserAuthorize(Machines machine, AccTimeseg timeseg, List<UserInfo> lstUser, List<AccDoor> lstDoorPermited)
		{
			if (machine != null && timeseg != null && lstUser != null && lstUser.Count > 0 && lstDoorPermited != null && lstDoorPermited.Count > 0)
			{
				int num = 0;
				int num2 = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("delete|userauthorize$");
				for (int i = 0; i < lstDoorPermited.Count; i++)
				{
					num |= 1 << lstDoorPermited[i].door_no - 1;
				}
				ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
				objUserAuthorize.Pin = "1";
				objUserAuthorize.AuthorizeDoorId = num.ToString();
				objUserAuthorize.TimeZone1 = timeseg.TimeZone1Id;
				objUserAuthorize.TimeZone2 = timeseg.TimeZone2Id;
				objUserAuthorize.TimeZone3 = timeseg.TimeZone3Id;
				objUserAuthorize.AuthorizeTimezoneId = ((timeseg.id == DeviceHelper.TimeSeg.id) ? 1 : timeseg.id).ToString();
				for (int j = 0; j < lstUser.Count; j++)
				{
					objUserAuthorize.Pin = lstUser[j].BadgeNumber;
					stringBuilder.Append(objUserAuthorize.ToPullCmdString(machine) + "#");
					if (j != 0 && j % num2 == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("delete|userauthorize$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void DeleteUserAuthorize(Machines machine, List<UserInfo> lstUser, bool SaveCmd, out string cmdInfo)
		{
			cmdInfo = "";
			if (machine != null && lstUser != null && lstUser.Count > 0)
			{
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				if (SaveCmd)
				{
					stringBuilder.Append("delete|userauthorize$");
				}
				for (int i = 0; i < lstUser.Count; i++)
				{
					stringBuilder.Append(string.Format("{0}={1}#", "Pin", lstUser[i].BadgeNumber));
					if (SaveCmd && i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("delete|userauthorize$");
					}
				}
				cmdInfo = stringBuilder.ToString();
				if (SaveCmd && stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddUserVerifyType(Machines machine, List<UserVerifyType> lstUserVT)
		{
			if (machine != null && lstUserVT != null && lstUserVT.Count > 0 && machine.DevSDKType == SDKType.StandaloneSDK && machine.UserExtFmt == 1)
			{
				int num = 500;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|uservt$");
				for (int i = 0; i < lstUserVT.Count; i++)
				{
					UserVerifyType userVerifyType = lstUserVT[i];
					stringBuilder.Append(userVerifyType.ToPullCmdString(machine) + "#");
					if (i != 0 && i % num == 0)
					{
						CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
						stringBuilder = new StringBuilder();
						stringBuilder.Append("update|uservt$");
					}
				}
				if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("$"))
				{
					CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
				}
			}
		}

		public static void AddTimeZone(Machines machine, List<AccTimeseg> lstTimeseg)
		{
			if (machine != null && lstTimeseg != null && lstTimeseg.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("update|timezone$");
				for (int i = 0; i < lstTimeseg.Count; i++)
				{
					AccTimeseg accTimeseg = lstTimeseg[i];
					stringBuilder.Append(accTimeseg.ToPullCmdString(machine, DeviceHelper.TimeSeg.id) + "#");
				}
				CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
			}
		}

		public static void AddTimeZone(Machines machine, AccTimeseg Timeseg)
		{
			if (machine != null && Timeseg != null)
			{
				List<AccTimeseg> list = new List<AccTimeseg>();
				list.Add(Timeseg);
				CommandServer.AddTimeZone(machine, list);
			}
		}

		public static void DeleteTimeZone(Machines machine, List<AccTimeseg> lstTimeseg)
		{
			if (machine != null && lstTimeseg != null && lstTimeseg.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("delete|timezone$");
				for (int i = 0; i < lstTimeseg.Count; i++)
				{
					AccTimeseg accTimeseg = lstTimeseg[i];
					stringBuilder.Append(accTimeseg.ToPullCmdString(machine, DeviceHelper.TimeSeg.id) + "#");
				}
				CommandServer.SaveCmdInfo(machine.ID, stringBuilder.ToString());
			}
		}

		public static void DeleteTimeZone(Machines machine, AccTimeseg Timeseg)
		{
			if (machine != null && Timeseg != null)
			{
				List<AccTimeseg> list = new List<AccTimeseg>();
				list.Add(Timeseg);
				CommandServer.DeleteTimeZone(machine, list);
			}
		}

		public static void DeleteAccLevel(List<AccLevelset> lstLvSet)
		{
			if (lstLvSet != null && lstLvSet.Count > 0)
			{
				string text = "";
				for (int i = 0; i < lstLvSet.Count; i++)
				{
					text = text + lstLvSet[i].id + ",";
				}
				text = text.Remove(text.Length - 1, 1);
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				List<UserInfo> modelList = userInfoBll.GetModelList("userid in (select employee_id from acc_levelset_emp where acclevelset_id in (" + text + "))");
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				if (modelList != null && modelList.Count > 0)
				{
					for (int j = 0; j < modelList.Count; j++)
					{
						if (!dictionary.ContainsKey(modelList[j].UserId))
						{
							dictionary.Add(modelList[j].UserId, modelList[j]);
						}
					}
				}
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				Dictionary<int, int> userId_TemplateCountDic = templateBll.GetUserId_TemplateCountDic("not TEMPLATE4 is null");
				Dictionary<int, int> userId_TemplateCountDic2 = templateBll.GetUserId_TemplateCountDic("not TEMPLATE3 is null");
				FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
				Dictionary<int, int> userId_FingerVeinCountDic = fingerVeinBll.GetUserId_FingerVeinCountDic();
				ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
				Dictionary<int, int> userId_FaceTempCountDic = faceTempBll.GetUserId_FaceTempCountDic("FaceType = 0");
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
				AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				for (int k = 0; k < lstLvSet.Count; k++)
				{
					AccLevelset accLevelset = lstLvSet[k];
					List<Machines> modelList2 = machinesBll.GetModelList("");
				}
			}
		}
	}
}
