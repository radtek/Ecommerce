/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System.Collections.Generic;
using System.Text;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;

namespace ZK.Access
{
	public class PullSDKDataConvertHelper
	{
		public static string SetDoorParam(AccDoor door, Machines dev)
		{
			AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
			AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
			ObjDoorInfo objDoorInfo = DeviceHelper.CreateDoor(door.door_no);
			if (objDoorInfo != null)
			{
				objDoorInfo.DoorForcePassWord = door.force_pwd;
				objDoorInfo.DoorSupperPassWord = door.supper_pwd;
				objDoorInfo.DoorSensorType = door.door_sensor_status;
				objDoorInfo.DoorVerifyType = door.opendoor_type;
				objDoorInfo.DoorValidTZ = door.lock_active_id;
				if (DeviceHelper.TimeSeg.id == objDoorInfo.DoorValidTZ)
				{
					objDoorInfo.DoorValidTZ = 1;
				}
				objDoorInfo.DoorKeepOpenTimeZone = door.long_open_id;
				if (DeviceHelper.TimeSeg.id == objDoorInfo.DoorKeepOpenTimeZone)
				{
					objDoorInfo.DoorKeepOpenTimeZone = 1;
				}
				objDoorInfo.DoorDrivertime = door.lock_delay;
				objDoorInfo.DoorIntertime = door.card_intervaltime;
				objDoorInfo.DoorDetectortime = door.sensor_delay;
				if (door.back_lock)
				{
					objDoorInfo.DoorCloseAndLock = 1;
				}
				else
				{
					objDoorInfo.DoorCloseAndLock = 0;
				}
				List<AccMorecardset> modelList = accMorecardsetBll.GetModelList("door_id =" + door.id + " ");
				if (modelList != null && modelList.Count > 0)
				{
					objDoorInfo.DoorMultiCardOpenDoor = 1;
				}
				else
				{
					objDoorInfo.DoorFirstCardOpenDoor = 0;
				}
				List<AccFirstOpen> modelList2 = accFirstOpenBll.GetModelList(("door_id =" + door.id) ?? "");
				if (modelList2 != null && modelList2.Count > 0)
				{
					objDoorInfo.DoorFirstCardOpenDoor = 1;
				}
				else
				{
					objDoorInfo.DoorFirstCardOpenDoor = 0;
				}
				objDoorInfo.DoorReaderIOState = door.readerIOState;
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					objDoorInfo.ManualCtlMode = door.ManualCtlMode;
				}
				string str = PullSDKDataConvert.SetDoorParam(objDoorInfo, dev);
				return "set$" + str;
			}
			return string.Empty;
		}

		public static string SetWiegandParam(AccDoor door, Machines dev)
		{
			AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
			AccWiegandfmt accWiegandfmt = null;
			string empty = string.Empty;
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = string.Empty;
			if (dev.device_type == 101 || dev.device_type == 102 || dev.device_type == 103)
			{
				accWiegandfmt = accWiegandfmtBll.GetModel(door.wiegand_fmt_id);
				if (accWiegandfmt != null)
				{
					text3 = "WiegandIDIn=" + door.wiegandInType;
					switch (accWiegandfmt.status)
					{
					case 0:
						if (PullSDKDataConvertHelper.checke(accWiegandfmt))
						{
							text = string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), door.door_no);
							text = text + "," + $"Reader{door.door_no}AutoMatch=0";
						}
						break;
					case 1:
						text = $"Reader{door.door_no}AutoMatch=1";
						break;
					case 2:
						text = string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, door.door_no);
						text = text + "," + $"Reader{door.door_no}AutoMatch=0";
						break;
					}
				}
				accWiegandfmt = accWiegandfmtBll.GetModel(door.wiegand_fmt_out_id);
				if (accWiegandfmt != null)
				{
					text4 = "WiegandID=" + door.wiegandOutType;
					switch (accWiegandfmt.status)
					{
					case 0:
						text2 = "WiegandFmt=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt);
						break;
					case 2:
						if (PullSDKDataConvertHelper.checke(accWiegandfmt))
						{
							if (accWiegandfmt.wiegand_name == "SRBOn")
							{
								text2 = "SRBOn=1";
							}
							else
							{
								text2 = "WiegandFmt=" + accWiegandfmt.wiegand_name;
								text2 += ",SRBOn=0";
							}
						}
						break;
					}
				}
				empty = text3 + "," + text + "," + text4 + "," + text2;
			}
			else
			{
				if (dev.deviceOption == null)
				{
					return string.Empty;
				}
				List<string> list = new List<string>();
				byte[] deviceOption = dev.deviceOption;
				string @string = Encoding.ASCII.GetString(deviceOption, 0, deviceOption.Length);
				string[] array = @string.Trim().Split(',');
				if (array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(array[i]);
					}
				}
				accWiegandfmt = accWiegandfmtBll.GetModel(door.wiegand_fmt_id);
				if (accWiegandfmt == null)
				{
					return string.Empty;
				}
				if (dev.door_count == dev.reader_count)
				{
					text3 = "WiegandIDIn=" + door.wiegandInType;
					switch (accWiegandfmt.status)
					{
					case 0:
						if (PullSDKDataConvertHelper.checke(accWiegandfmt))
						{
							text = string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), door.door_no);
							text = text + "," + $"Reader{door.door_no}AutoMatch=0";
						}
						break;
					case 1:
						text = $"Reader{door.door_no}AutoMatch=1";
						break;
					case 2:
						text = string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, door.door_no);
						text = text + "," + $"Reader{door.door_no}AutoMatch=0";
						break;
					}
					empty = text3 + "," + text;
				}
				else
				{
					text3 = "WiegandIDIn=" + door.wiegandInType;
					switch (accWiegandfmt.status)
					{
					case 0:
						if (PullSDKDataConvertHelper.checke(accWiegandfmt))
						{
							if (door.door_no == 1)
							{
								text = $"Reader{1}AutoMatch=0";
								text = text + "," + $"Reader{2}AutoMatch=0";
								text = text + ",Reader1WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt);
								text = text + ",Reader2WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt);
							}
							if (door.door_no == 2)
							{
								text = $"Reader{3}AutoMatch=0";
								text = text + "," + $"Reader{4}AutoMatch=0";
								text = text + "," + string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), 3);
								text = text + "," + string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), 4);
							}
							if (door.door_no == 3)
							{
								text = $"Reader{5}AutoMatch=0";
								text = text + "," + $"Reader{6}AutoMatch=0";
								text = text + "," + string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), 5);
								text = text + "," + string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), 6);
							}
							if (door.door_no == 4)
							{
								text = $"Reader{7}AutoMatch=0";
								text = text + "," + $"Reader{8}AutoMatch=0";
								text = text + "," + string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), 7);
								text = text + "," + string.Format("Reader{0}WGType=" + PullSDKDataConvertHelper.wiegandFmtStr(accWiegandfmt), 8);
							}
						}
						break;
					case 1:
						text = $"Reader{door.door_no * 2 - 1}AutoMatch=1";
						text = text + "," + $"Reader{door.door_no * 2}AutoMatch=1";
						break;
					case 2:
						if (door.door_no == 1)
						{
							text = $"Reader{1}AutoMatch=0";
							text = text + "," + $"Reader{2}AutoMatch=0";
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 1);
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 2);
						}
						if (door.door_no == 2)
						{
							text = $"Reader{3}AutoMatch=0";
							text = text + "," + $"Reader{4}AutoMatch=0";
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 3);
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 4);
						}
						if (door.door_no == 3)
						{
							text = $"Reader{5}AutoMatch=0";
							text = text + "," + $"Reader{6}AutoMatch=0";
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 5);
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 6);
						}
						if (door.door_no == 4)
						{
							text = $"Reader{7}AutoMatch=0";
							text = text + "," + $"Reader{8}AutoMatch=0";
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 7);
							text = text + "," + string.Format("Reader{0}WGType=" + accWiegandfmt.wiegand_name, 8);
						}
						break;
					}
					empty = text3 + "," + text;
				}
			}
			return empty;
		}

		private static bool checke(AccWiegandfmt in_model)
		{
			if (in_model.odd_count > in_model.wiegand_count)
			{
				return false;
			}
			if (in_model.even_count > in_model.wiegand_count)
			{
				return false;
			}
			if (in_model.odd_count + in_model.even_count > in_model.wiegand_count)
			{
				return false;
			}
			if (in_model.cid_count > in_model.wiegand_count)
			{
				return false;
			}
			if (in_model.comp_count > in_model.wiegand_count)
			{
				return false;
			}
			if (in_model.cid_count + in_model.comp_count > in_model.wiegand_count)
			{
				return false;
			}
			return true;
		}

		public static string wiegandFmtStr(AccWiegandfmt model)
		{
			char[] array = new char[model.wiegand_count];
			for (int i = 0; i < model.wiegand_count; i++)
			{
				array[i] = 'b';
			}
			int num = model.odd_start - 1;
			int num2 = model.odd_start + model.odd_count - 2;
			int num3 = model.even_start - 1;
			int num4 = model.even_start + model.even_count - 2;
			for (int j = 0; j < model.wiegand_count; j++)
			{
				if (num <= j && j <= num2)
				{
					array[j] = 'o';
				}
				if (num3 <= j && j <= num4)
				{
					array[j] = 'e';
				}
			}
			string str = new string(array, 0, array.Length);
			for (int k = 0; k < model.wiegand_count; k++)
			{
				if (k == 0 || k == model.wiegand_count - 1)
				{
					array[k] = 'p';
				}
				else
				{
					array[k] = 's';
				}
			}
			int num5 = model.cid_start - 1;
			int num6 = model.cid_start + model.cid_count - 2;
			int num7 = model.comp_start - 1;
			int num8 = model.comp_start + model.comp_count - 2;
			for (int l = 0; l < model.wiegand_count; l++)
			{
				if (num5 <= l && l <= num6)
				{
					array[l] = 'c';
				}
				if (num7 <= l && l <= num8)
				{
					array[l] = 'f';
				}
			}
			string str2 = new string(array, 0, array.Length);
			return str2 + ":" + str;
		}

		public static string AddFirstCard(ObjFirstCard model, bool isAddHead)
		{
			string text = PullSDKDataConvert.AddFirstCard(model);
			if (isAddHead)
			{
				text = "update|FirstCard$" + text;
			}
			return text;
		}

		public static string AddFirstCard(List<ObjFirstCard> list, bool isAddHead)
		{
			string text = PullSDKDataConvert.AddFirstCard(list);
			if (isAddHead)
			{
				text = "update|FirstCard$" + text;
			}
			return text;
		}

		public static string DeleteFirstCard(List<ObjFirstCard> list, bool isAddHead)
		{
			string text = PullSDKDataConvert.DeleteFirstCard(list);
			if (isAddHead)
			{
				text = "delete|FirstCard$" + text;
			}
			return text;
		}

		public static string DeleteFirstCard(int doorid, int timeid, string pin, bool isAddHead)
		{
			ObjFirstCard objFirstCard = new ObjFirstCard();
			objFirstCard.DoorID = doorid.ToString();
			objFirstCard.Pin = pin;
			objFirstCard.TimezoneID = timeid.ToString();
			string text = PullSDKDataConvert.DeleteFirstCard(objFirstCard);
			if (isAddHead)
			{
				text = "delete|FirstCard$" + text;
			}
			return text;
		}

		public static string DeleteFirstCard(int doorid, int timeid, bool isAddHead)
		{
			string text = PullSDKDataConvert.DeleteFirstCard(doorid, timeid);
			if (isAddHead)
			{
				text = "delete|FirstCard$" + text;
			}
			return text;
		}

		public static string DeleteFirstCard(int timeid, bool isAddHead)
		{
			string text = PullSDKDataConvert.DeleteFirstCard(timeid);
			if (isAddHead)
			{
				text = "delete|FirstCard$" + text;
			}
			return text;
		}

		public static string DeleteFirstCard(string pin, bool isAddHead)
		{
			string text = PullSDKDataConvert.DeleteFirstCard(pin);
			if (isAddHead)
			{
				text = "delete|FirstCard$" + text;
			}
			return text;
		}

		public static string AddHoliday(AccHolidays model, bool isAddHead)
		{
			List<ObjHoliday> list = DeviceHelper.HolidayConvert(model);
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddHoliday(list);
				if (isAddHead)
				{
					text = "update|Holiday$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddHoliday(List<AccHolidays> holidayList, bool isAddHead)
		{
			List<ObjHoliday> list = new List<ObjHoliday>();
			if (holidayList != null && holidayList.Count > 0)
			{
				for (int i = 0; i < holidayList.Count; i++)
				{
					list.AddRange(DeviceHelper.HolidayConvert(holidayList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddHoliday(list);
				if (isAddHead)
				{
					text = "update|Holiday$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteHoliday(AccHolidays model, bool isAddHead)
		{
			List<ObjHoliday> list = DeviceHelper.HolidayConvert(model);
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteHoliday(list);
				if (isAddHead)
				{
					text = "delete|Holiday$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteHoliday(List<AccHolidays> holidayList, bool isAddHead)
		{
			List<ObjHoliday> list = new List<ObjHoliday>();
			if (holidayList != null && holidayList.Count > 0)
			{
				for (int i = 0; i < holidayList.Count; i++)
				{
					list.AddRange(DeviceHelper.HolidayConvert(holidayList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteHoliday(list);
				if (isAddHead)
				{
					text = "delete|Holiday$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddInOutFun(AccLinkAgeIo model, bool isAddHead)
		{
			if (model != null)
			{
				ObjInOutFun objInOutFun = new ObjInOutFun();
				string text;
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF && model.action_type == 3)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("EventType=" + model.trigger_opt + "\t");
					stringBuilder.Append("InAddr=" + model.in_address + "\t");
					stringBuilder.Append("Index=" + model.id + "\t");
					stringBuilder.Append("OutAddr=" + model.out_address + "\t");
					stringBuilder.Append("OutTime=" + model.delay_time + "\t");
					stringBuilder.Append("OutType=2\t");
					stringBuilder.Append("Reserved=#");
					text = stringBuilder.ToString();
				}
				else
				{
					objInOutFun = DeviceHelper.InOutFunConvert(model);
					text = PullSDKDataConvert.AddInOutFun(objInOutFun);
				}
				if (isAddHead)
				{
					text = "update|InOutFun$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddInOutFun(List<AccLinkAgeIo> LinkAgeIOList, bool isAddHead)
		{
			List<ObjInOutFun> list = new List<ObjInOutFun>();
			if (LinkAgeIOList != null && LinkAgeIOList.Count > 0)
			{
				for (int i = 0; i < LinkAgeIOList.Count; i++)
				{
					list.Add(DeviceHelper.InOutFunConvert(LinkAgeIOList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddInOutFun(list);
				if (isAddHead)
				{
					text = "update|InOutFun$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteInOutFun(AccLinkAgeIo model, bool isAddHead)
		{
			if (model != null)
			{
				ObjInOutFun objInOutFun = new ObjInOutFun();
				string text;
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					text = "Index=" + model.id + "#";
				}
				else
				{
					objInOutFun = DeviceHelper.InOutFunConvert(model);
					text = PullSDKDataConvert.DeleteInOutFun(objInOutFun);
				}
				if (isAddHead)
				{
					text = "delete|InOutFun$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteInOutFun(List<AccLinkAgeIo> LinkAgeIOList, bool isAddHead)
		{
			List<ObjInOutFun> list = new List<ObjInOutFun>();
			if (LinkAgeIOList != null && LinkAgeIOList.Count > 0)
			{
				for (int i = 0; i < LinkAgeIOList.Count; i++)
				{
					list.Add(DeviceHelper.InOutFunConvert(LinkAgeIOList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteInOutFun(list);
				if (isAddHead)
				{
					text = "delete|InOutFun$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddMultimCard(AccMorecardset model, bool isAddHead, Machines machine = null)
		{
			if (model != null)
			{
				ObjMultimCard objMultimCard = DeviceHelper.MultimCardConvert(model, machine);
				if (objMultimCard != null)
				{
					string text = PullSDKDataConvert.AddMultimCard(objMultimCard);
					if (isAddHead)
					{
						text = "update|multimcard$" + text;
					}
					return text;
				}
				return string.Empty;
			}
			return string.Empty;
		}

		public static string AddMultimCard(List<AccMorecardset> mList, bool isAddHead)
		{
			List<ObjMultimCard> list = new List<ObjMultimCard>();
			if (mList != null && mList.Count > 0)
			{
				for (int i = 0; i < mList.Count; i++)
				{
					ObjMultimCard objMultimCard = DeviceHelper.MultimCardConvert(mList[i], null);
					if (objMultimCard != null)
					{
						list.Add(objMultimCard);
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddMultimCard(list);
				if (isAddHead)
				{
					text = "update|multimcard$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteMultimCard(AccMorecardset model, bool isAddHead)
		{
			if (model != null)
			{
				ObjMultimCard objMultimCard = DeviceHelper.MultimCardConvert(model, null);
				if (objMultimCard != null)
				{
					string text = PullSDKDataConvert.DeleteMultimCard(objMultimCard);
					if (isAddHead)
					{
						text = "delete|multimcard$" + text;
					}
					return text;
				}
				return string.Empty;
			}
			return string.Empty;
		}

		public static string DeleteMultimCard(List<AccMorecardset> mList, bool isAddHead)
		{
			List<ObjMultimCard> list = new List<ObjMultimCard>();
			if (mList != null && mList.Count > 0)
			{
				for (int i = 0; i < mList.Count; i++)
				{
					ObjMultimCard objMultimCard = DeviceHelper.MultimCardConvert(mList[i], null);
					if (objMultimCard != null)
					{
						list.Add(objMultimCard);
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteMultimCard(list);
				if (isAddHead)
				{
					text = "delete|multimcard$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddTimeZone(AccTimeseg model, bool isAddHead)
		{
			if (model != null)
			{
				ObjTimeZone objTimeZone = new ObjTimeZone();
				objTimeZone = DeviceHelper.ZKDateTimeConvert(model);
				string text = PullSDKDataConvert.AddTimeZone(objTimeZone);
				if (isAddHead)
				{
					text = "update|TimeZone$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddTimeZone(List<AccTimeseg> timeZoneList, bool isAddHead)
		{
			List<ObjTimeZone> list = new List<ObjTimeZone>();
			if (timeZoneList != null && timeZoneList.Count > 0)
			{
				for (int i = 0; i < timeZoneList.Count; i++)
				{
					list.Add(DeviceHelper.ZKDateTimeConvert(timeZoneList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddTimeZone(list);
				if (isAddHead)
				{
					text = "update|TimeZone$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteTimeZone(AccTimeseg model, bool isAddHead)
		{
			if (model != null)
			{
				ObjTimeZone objTimeZone = new ObjTimeZone();
				objTimeZone = DeviceHelper.ZKDateTimeConvert(model);
				string text = PullSDKDataConvert.DeleteTimeZone(objTimeZone);
				if (isAddHead)
				{
					text = "delete|TimeZone$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteTimeZone(List<AccTimeseg> timeZoneList, bool isAddHead)
		{
			List<ObjTimeZone> list = new List<ObjTimeZone>();
			if (timeZoneList != null && timeZoneList.Count > 0)
			{
				for (int i = 0; i < timeZoneList.Count; i++)
				{
					list.Add(DeviceHelper.ZKDateTimeConvert(timeZoneList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteTimeZone(list);
				if (isAddHead)
				{
					text = "delete|TimeZone$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddUser(UserInfo model, Machines Dev, bool isAddHead)
		{
			if (model != null)
			{
				ObjUser model2 = DeviceHelper.UserDataConvert(model);
				string text = PullSDKDataConvert.AddUser(model2, Dev);
				if (isAddHead)
				{
					text = "update|User$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddUser(List<UserInfo> userList, Machines Dev, bool isAddHead)
		{
			List<ObjUser> list = new List<ObjUser>();
			if (userList != null && userList.Count > 0)
			{
				for (int i = 0; i < userList.Count; i++)
				{
					list.Add(DeviceHelper.UserDataConvert(userList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddUser(list, Dev);
				if (isAddHead)
				{
					text = "update|User$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteUser(UserInfo model, bool isAddHead)
		{
			if (model != null)
			{
				ObjUser model2 = DeviceHelper.UserDataConvert(model);
				string text = PullSDKDataConvert.DeleteUser(model2);
				if (isAddHead)
				{
					text = "delete|User$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteUser(List<UserInfo> userList, bool isAddHead)
		{
			List<ObjUser> list = new List<ObjUser>();
			if (userList != null && userList.Count > 0)
			{
				for (int i = 0; i < userList.Count; i++)
				{
					list.Add(DeviceHelper.UserDataConvert(userList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteUser(list);
				if (isAddHead)
				{
					text = "delete|User$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddUserAuthorize(ObjUserAuthorize model, bool isAddHead)
		{
			if (model != null)
			{
				string text = PullSDKDataConvert.AddUserAuthorize(model);
				if (isAddHead)
				{
					text = "update|UserAuthorize$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddUserAuthorize(List<ObjUserAuthorize> userList, bool isAddHead)
		{
			string text = PullSDKDataConvert.AddUserAuthorize(userList);
			if (isAddHead)
			{
				text = "update|UserAuthorize$" + text;
			}
			return text;
		}

		public static string DeleteUserAuthorize(ObjUserAuthorize model, bool isAddHead)
		{
			if (model != null)
			{
				string text = PullSDKDataConvert.DeleteUserAuthorize(model);
				if (isAddHead)
				{
					text = "delete|UserAuthorize$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorize(UserInfo model, bool isAddHead)
		{
			if (model != null)
			{
				string text = PullSDKDataConvert.DeleteUserAuthorize(DeviceHelper.UserDataConvert(model));
				if (isAddHead)
				{
					text = "delete|UserAuthorize$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorize(List<ObjUserAuthorize> userList, bool isAddHead)
		{
			string text = "";
			if (userList != null && userList.Count > 0)
			{
				text = PullSDKDataConvert.DeleteUserAuthorize(userList);
				if (isAddHead)
				{
					text = "delete|UserAuthorize$" + text;
				}
			}
			return text;
		}

		public static string DeleteUserAuthorize(List<ObjUserAuthorize> userList)
		{
			string result = "";
			if (userList != null && userList.Count > 0)
			{
				result = PullSDKDataConvert.DeleteUserAuthorizeEx(userList);
				result = "delete|UserAuthorize$" + result;
			}
			return result;
		}

		public static string AddTemplateV10(Template model, bool isAddHead)
		{
			if (model != null)
			{
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				objTemplateV = DeviceHelper.UserTemplateConvert(model);
				string text = PullSDKDataConvert.AddTemplateV10(objTemplateV);
				if (isAddHead)
				{
					text = "update|TemplateV10$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddTemplateV10(List<Template> templateList, bool isAddHead)
		{
			List<ObjTemplateV10> list = new List<ObjTemplateV10>();
			if (templateList != null && templateList.Count > 0)
			{
				for (int i = 0; i < templateList.Count; i++)
				{
					list.Add(DeviceHelper.UserTemplateConvert(templateList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddTemplateV10(list);
				if (isAddHead)
				{
					text = "update|TemplateV10$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteTemplateV10(Template model, bool isAddHead)
		{
			if (model != null)
			{
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				objTemplateV = DeviceHelper.UserTemplateConvert(model);
				string text = PullSDKDataConvert.DeleteTemplateV10(objTemplateV);
				if (isAddHead)
				{
					text = "delete|TemplateV10$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteTemplateV10Ex(Template model, bool isAddHead)
		{
			if (model != null)
			{
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				model.FINGERID = -1;
				objTemplateV = DeviceHelper.UserTemplateConvert(model);
				string text = PullSDKDataConvert.DeleteTemplateV10(objTemplateV);
				if (isAddHead)
				{
					text = "delete|TemplateV10$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteTemplateV10(List<Template> templateList, bool isAddHead)
		{
			List<ObjTemplateV10> list = new List<ObjTemplateV10>();
			if (templateList != null && templateList.Count > 0)
			{
				for (int i = 0; i < templateList.Count; i++)
				{
					list.Add(DeviceHelper.UserTemplateConvert(templateList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteTemplateV10(list);
				if (isAddHead)
				{
					text = "delete|TemplateV10$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddFaceTemp(FaceTemp model, bool isAddHead)
		{
			if (model != null)
			{
				ObjFaceTemp objFaceTemp = new ObjFaceTemp();
				objFaceTemp = DeviceHelper.UserFaceTempConvert(model);
				string text = PullSDKDataConvert.AddFaceTemp(objFaceTemp);
				if (isAddHead)
				{
					text = "update|ssrface7$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddFaceTemp(List<FaceTemp> fvList, bool isAddHead)
		{
			List<ObjFaceTemp> list = new List<ObjFaceTemp>();
			if (fvList != null && fvList.Count > 0)
			{
				for (int i = 0; i < fvList.Count; i++)
				{
					list.Add(DeviceHelper.UserFaceTempConvert(fvList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddFaceTemp(list);
				if (isAddHead)
				{
					text = "update|ssrface7$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteFaceTemp(FaceTemp model, bool isAddHead)
		{
			if (model != null)
			{
				ObjFaceTemp objFaceTemp = new ObjFaceTemp();
				objFaceTemp = DeviceHelper.UserFaceTempConvert(model);
				string text = PullSDKDataConvert.DeleteFaceTemp(objFaceTemp);
				if (isAddHead)
				{
					text = "delete|ssrface7$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteFaceTemp(List<FaceTemp> fvList, bool isAddHead)
		{
			List<ObjFaceTemp> list = new List<ObjFaceTemp>();
			if (fvList != null && fvList.Count > 0)
			{
				for (int i = 0; i < fvList.Count; i++)
				{
					list.Add(DeviceHelper.UserFaceTempConvert(fvList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteFaceTemp(list);
				if (isAddHead)
				{
					text = "delete|ssrface7$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteFaceTemp(UserInfo model, bool isAddHead)
		{
			if (model != null)
			{
				string text = PullSDKDataConvert.DeleteFaceTemp(DeviceHelper.UserDataConvert(model));
				if (isAddHead)
				{
					text = "delete|ssrface7$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddFingerVein(FingerVein model, bool isAddHead)
		{
			if (model != null)
			{
				ObjFVTemplate objFVTemplate = new ObjFVTemplate();
				objFVTemplate = DeviceHelper.UserFingerVeinConvert(model);
				string text = PullSDKDataConvert.AddFingerVein(objFVTemplate);
				if (isAddHead)
				{
					text = "update|fvtemplate$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string AddFingerVein(List<FingerVein> fvList, bool isAddHead)
		{
			List<ObjFVTemplate> list = new List<ObjFVTemplate>();
			if (fvList != null && fvList.Count > 0)
			{
				for (int i = 0; i < fvList.Count; i++)
				{
					list.Add(DeviceHelper.UserFingerVeinConvert(fvList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.AddFingerVein(list);
				if (isAddHead)
				{
					text = "update|fvtemplate$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteFingerVein(FingerVein model, bool isAddHead)
		{
			if (model != null)
			{
				ObjFVTemplate objFVTemplate = new ObjFVTemplate();
				objFVTemplate = DeviceHelper.UserFingerVeinConvert(model);
				string text = PullSDKDataConvert.DeleteFingerVein(objFVTemplate);
				if (isAddHead)
				{
					text = "delete|fvtemplate$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteFingerVein(List<FingerVein> fvList, bool isAddHead)
		{
			List<ObjFVTemplate> list = new List<ObjFVTemplate>();
			if (fvList != null && fvList.Count > 0)
			{
				for (int i = 0; i < fvList.Count; i++)
				{
					list.Add(DeviceHelper.UserFingerVeinConvert(fvList[i]));
				}
			}
			if (list != null && list.Count > 0)
			{
				string text = PullSDKDataConvert.DeleteFingerVein(list);
				if (isAddHead)
				{
					text = "delete|fvtemplate$" + text;
				}
				return text;
			}
			return string.Empty;
		}

		public static string DeleteFingerVein(UserInfo model, bool isAddHead)
		{
			if (model != null)
			{
				string text = PullSDKDataConvert.DeleteFingerVein(DeviceHelper.UserDataConvert(model));
				if (isAddHead)
				{
					text = "delete|fvtemplate$" + text;
				}
				return text;
			}
			return string.Empty;
		}
	}
}
