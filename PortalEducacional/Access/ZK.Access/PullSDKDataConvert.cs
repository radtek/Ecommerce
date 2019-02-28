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
using System.Net;
using System.Text;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;

namespace ZK.Access
{
	public class PullSDKDataConvert
	{
		public static string AddFirstCard(ObjFirstCard model)
		{
			if (model != null)
			{
				if (DeviceHelper.TimeSeg.id.ToString() == model.TimezoneID)
				{
					model.TimezoneID = "1";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("DoorID=" + model.DoorID + "\t");
				stringBuilder.Append("Pin=" + model.Pin + "\t");
				stringBuilder.Append("TimezoneID=" + model.TimezoneID + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddFirstCard(List<ObjFirstCard> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjFirstCard objFirstCard = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objFirstCard = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddFirstCard(objFirstCard));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFirstCard(ObjFirstCard model)
		{
			if (model != null)
			{
				if (DeviceHelper.TimeSeg.id.ToString() == model.TimezoneID)
				{
					model.TimezoneID = "1";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("DoorID=" + model.DoorID + "\t");
				stringBuilder.Append("Pin=" + model.Pin + "\t");
				stringBuilder.Append("TimezoneID=" + model.TimezoneID + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFirstCard(string pin)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Pin=" + pin + "#");
			return stringBuilder.ToString();
		}

		public static string DeleteFirstCard(int doorid, int timeid)
		{
			if (DeviceHelper.TimeSeg.id == timeid)
			{
				timeid = 1;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("DoorID=" + doorid + "\t");
			stringBuilder.Append("TimezoneID=" + timeid + "#");
			return stringBuilder.ToString();
		}

		public static string DeleteFirstCard(int timeid)
		{
			if (DeviceHelper.TimeSeg.id == timeid)
			{
				timeid = 1;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("TimezoneID=" + timeid + "#");
			return stringBuilder.ToString();
		}

		public static string DeleteFirstCard(List<ObjFirstCard> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjFirstCard objFirstCard = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objFirstCard = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteFirstCard(objFirstCard));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddHoliday(List<ObjHoliday> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjHoliday objHoliday = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objHoliday = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddHoliday(objHoliday));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddHoliday(ObjHoliday model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Holiday=" + model.Holiday + "\t");
				StringBuilder stringBuilder2 = stringBuilder;
				int num = (int)model.HolidayType;
				stringBuilder2.Append("HolidayType=" + num.ToString() + "\t");
				StringBuilder stringBuilder3 = stringBuilder;
				num = (int)model.Loop;
				stringBuilder3.Append("Loop=" + num.ToString() + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteHoliday(ObjHoliday model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Holiday=" + model.Holiday + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteHoliday(List<ObjHoliday> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjHoliday objHoliday = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objHoliday = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteHoliday(objHoliday));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string SetDoorParam(ObjDoorInfo door, Machines dev)
		{
			if (door != null)
			{
				string str = "Door$CloseAndLock=" + door.DoorCloseAndLock;
				str = str + ",Door$ForcePassWord=" + door.DoorForcePassWord;
				str = str + ",Door$SupperPassWord=" + door.DoorSupperPassWord;
				str = str + ",Door$Detectortime=" + door.DoorDetectortime;
				str = str + ",Door$Drivertime=" + door.DoorDrivertime;
				str = str + ",Door$Intertime=" + door.DoorIntertime;
				str = str + ",Door$KeepOpenTimeZone=" + door.DoorKeepOpenTimeZone;
				str = str + ",Door$SensorType=" + door.DoorSensorType;
				str = str + ",Door$ValidTZ=" + door.DoorValidTZ;
				str = str + ",Door$VerifyType=" + door.DoorVerifyType;
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					str = str + ",Door$ManualCtlMode=" + door.ManualCtlMode;
				}
				if (dev.device_type == 12 || dev.device_type == 101 || dev.device_type == 102 || dev.device_type != 103)
				{
					str = str + ",Reader1IOState=" + door.DoorReaderIOState;
				}
				if (dev.DevSDKType == SDKType.StandaloneSDK)
				{
					str += string.Format(",Door{0}{1}={2}", "$", "door_sensor_status", door.DoorSensorType);
					str += string.Format(",Door{0}{1}={2}", "$", "sensor_delay", door.DoorDetectortime);
					str += string.Format(",Door{0}{1}={2}", "$", "readerIOState", door.DoorReaderIOState);
					str += string.Format(",Door{0}{1}={2}", "$", "lock_delay", door.DoorDrivertime);
					str += string.Format(",Door{0}{1}={2}", "$", "DoorSensorTimeout", door.DoorSensorTimeout);
					str += string.Format(",Door{0}{1}={2}", "$", "ERRTimes", door.ERRTimes);
					str += string.Format(",Door{0}{1}={2}", "$", "SRBOn", door.SRBOn);
				}
				switch (door.DoorType)
				{
				case DoorType.Door1:
					str = str.Replace("$", "1");
					break;
				case DoorType.Door2:
					str = str.Replace("$", "2");
					break;
				case DoorType.Door3:
					str = str.Replace("$", "3");
					break;
				case DoorType.Door4:
					str = str.Replace("$", "4");
					break;
				}
				return str;
			}
			return string.Empty;
		}

		public static string AddInOutFun(ObjInOutFun model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("EventType=" + (int)model.EventType + "\t");
				stringBuilder.Append("InAddr=" + (int)model.InAddr + "\t");
				stringBuilder.Append("Index=" + model.Index + "\t");
				stringBuilder.Append("OutAddr=" + (int)model.OutAddr + "\t");
				stringBuilder.Append("OutTime=" + model.OutTime + "\t");
				stringBuilder.Append("OutType=" + (int)model.OutType + "\t");
				stringBuilder.Append("Reserved=" + model.Reserved + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddInOutFun(List<ObjInOutFun> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjInOutFun objInOutFun = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objInOutFun = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddInOutFun(objInOutFun));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteInOutFun(ObjInOutFun model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Index=" + model.Index + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteInOutFun(List<ObjInOutFun> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjInOutFun objInOutFun = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objInOutFun = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteInOutFun(objInOutFun));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddMultimCard(ObjMultimCard model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("DoorId=" + model.DoorId + "\t");
				if (!string.IsNullOrEmpty(model.Group1))
				{
					stringBuilder.Append("Group1=" + model.Group1 + "\t");
				}
				if (!string.IsNullOrEmpty(model.Group2))
				{
					stringBuilder.Append("Group2=" + model.Group2 + "\t");
				}
				if (!string.IsNullOrEmpty(model.Group3))
				{
					stringBuilder.Append("Group3=" + model.Group3 + "\t");
				}
				if (!string.IsNullOrEmpty(model.Group4))
				{
					stringBuilder.Append("Group4=" + model.Group4 + "\t");
				}
				if (!string.IsNullOrEmpty(model.Group5))
				{
					stringBuilder.Append("Group5=" + model.Group5 + "\t");
				}
				stringBuilder.Append("Index=" + model.Index + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddMultimCard(List<ObjMultimCard> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjMultimCard objMultimCard = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objMultimCard = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddMultimCard(objMultimCard));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteMultimCard(ObjMultimCard model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Index=" + model.Index + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteMultimCard(List<ObjMultimCard> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjMultimCard objMultimCard = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objMultimCard = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteMultimCard(objMultimCard));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddTemplateV10(ObjTemplateV10 model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("EndTag=" + model.EndTag + "\t");
				stringBuilder.Append("FingerID=" + (int)model.FingerID + "\t");
				stringBuilder.Append("Pin=" + model.Pin + "\t");
				stringBuilder.Append("Resverd=" + model.Resverd + "\t");
				stringBuilder.Append("Size=" + model.Size + "\t");
				stringBuilder.Append("Template=" + model.Template + "\t");
				stringBuilder.Append("Valid=" + (int)model.Valid + "\t");
				stringBuilder.Append("UID=" + model.UID + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddTemplateV10(List<ObjTemplateV10> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjTemplateV10 objTemplateV = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objTemplateV = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddTemplateV10(objTemplateV));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteTemplateV10(ObjTemplateV10 model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if ((FingerType)(-1) != model.FingerID)
				{
					stringBuilder.Append("Pin=" + model.Pin + "\t");
					stringBuilder.Append("FingerID=" + (int)model.FingerID + "#");
				}
				else
				{
					stringBuilder.Append("Pin=" + model.Pin + "#");
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteTemplateV10(List<ObjTemplateV10> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjTemplateV10 objTemplateV = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objTemplateV = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteTemplateV10(objTemplateV));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddTimeZone(ObjTimeZone model)
		{
			if (model != null)
			{
				if (DeviceHelper.TimeSeg.id.ToString() == model.TimezoneId)
				{
					model.TimezoneId = "1";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("FriTime1=" + model.FriTime1 + "\t");
				stringBuilder.Append("FriTime2=" + model.FriTime2 + "\t");
				stringBuilder.Append("FriTime3=" + model.FriTime3 + "\t");
				stringBuilder.Append("Hol1Time1=" + model.Hol1Time1 + "\t");
				stringBuilder.Append("Hol1Time2=" + model.Hol1Time2 + "\t");
				stringBuilder.Append("Hol1Time3=" + model.Hol1Time3 + "\t");
				stringBuilder.Append("Hol2Time1=" + model.Hol2Time1 + "\t");
				stringBuilder.Append("Hol2Time2=" + model.Hol2Time2 + "\t");
				stringBuilder.Append("Hol2Time3=" + model.Hol2Time3 + "\t");
				stringBuilder.Append("Hol3Time1=" + model.Hol3Time1 + "\t");
				stringBuilder.Append("Hol3Time2=" + model.Hol3Time2 + "\t");
				stringBuilder.Append("Hol3Time3=" + model.Hol3Time3 + "\t");
				stringBuilder.Append("MonTime1=" + model.MonTime1 + "\t");
				stringBuilder.Append("MonTime2=" + model.MonTime2 + "\t");
				stringBuilder.Append("MonTime3=" + model.MonTime3 + "\t");
				stringBuilder.Append("SatTime1=" + model.SatTime1 + "\t");
				stringBuilder.Append("SatTime2=" + model.SatTime2 + "\t");
				stringBuilder.Append("SatTime3=" + model.SatTime3 + "\t");
				stringBuilder.Append("SunTime1=" + model.SunTime1 + "\t");
				stringBuilder.Append("SunTime2=" + model.SunTime2 + "\t");
				stringBuilder.Append("SunTime3=" + model.SunTime3 + "\t");
				stringBuilder.Append("ThuTime1=" + model.ThuTime1 + "\t");
				stringBuilder.Append("ThuTime2=" + model.ThuTime2 + "\t");
				stringBuilder.Append("ThuTime3=" + model.ThuTime3 + "\t");
				stringBuilder.Append("TueTime1=" + model.TueTime1 + "\t");
				stringBuilder.Append("TueTime2=" + model.TueTime2 + "\t");
				stringBuilder.Append("TueTime3=" + model.TueTime3 + "\t");
				stringBuilder.Append("WedTime1=" + model.WedTime1 + "\t");
				stringBuilder.Append("WedTime2=" + model.WedTime2 + "\t");
				stringBuilder.Append("WedTime3=" + model.WedTime3 + "\t");
				stringBuilder.Append("TimezoneId=" + model.TimezoneId + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddTimeZone(List<ObjTimeZone> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjTimeZone objTimeZone = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objTimeZone = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddTimeZone(objTimeZone));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteTimeZone(ObjTimeZone model)
		{
			if (model != null)
			{
				if (DeviceHelper.TimeSeg.id.ToString() == model.TimezoneId)
				{
					model.TimezoneId = "1";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("TimezoneId=" + model.TimezoneId + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteTimeZone(List<ObjTimeZone> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjTimeZone objTimeZone = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objTimeZone = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteTimeZone(objTimeZone));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddUserAuthorize(ObjUserAuthorize model)
		{
			if (model != null)
			{
				if (DeviceHelper.TimeSeg.id.ToString() == model.AuthorizeTimezoneId)
				{
					model.AuthorizeTimezoneId = "1";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("AuthorizeDoorId=" + model.AuthorizeDoorId + "\t");
				stringBuilder.Append("AuthorizeTimezoneId=" + model.AuthorizeTimezoneId + "\t");
				stringBuilder.Append("Pin=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddUserAuthorize(List<ObjUserAuthorize> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjUserAuthorize objUserAuthorize = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objUserAuthorize = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddUserAuthorize(objUserAuthorize));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorize(ObjUserAuthorize model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Pin=" + model.Pin + "\t");
				stringBuilder.Append("AuthorizeTimezoneId=" + model.AuthorizeTimezoneId + "\t");
				stringBuilder.Append("AuthorizeDoorId=" + model.AuthorizeDoorId + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorize(ObjUser model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Pin=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorizeEx(ObjUserAuthorize model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Pin=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorize(List<ObjUserAuthorize> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjUserAuthorize objUserAuthorize = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objUserAuthorize = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteUserAuthorize(objUserAuthorize));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUserAuthorizeEx(List<ObjUserAuthorize> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjUserAuthorize objUserAuthorize = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objUserAuthorize = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteUserAuthorizeEx(objUserAuthorize));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		private static string SwapHex(string HexData)
		{
			long value = long.Parse(HexData);
			string value2 = Convert.ToString(value, 16).ToUpper().PadLeft(8, '0');
			value2 = IPAddress.NetworkToHostOrder(Convert.ToInt64(value2, 16)).ToString("X").PadLeft(8, '0')
				.Substring(0, 8);
			return Convert.ToInt64(value2, 16).ToString();
		}

		public static string AddUser(ObjUser model, Machines Dev)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				switch (AccCommon.CodeVersion)
				{
				case CodeVersionType.Main:
					stringBuilder.Append("CardNo=" + model.CardNo + "\t");
					break;
				case CodeVersionType.JapanAF:
					if (string.IsNullOrEmpty(model.CardNo))
					{
						stringBuilder.Append("CardNo=" + model.CardNo + "\t");
					}
					else
					{
						stringBuilder.Append("CardNo=" + long.Parse(model.CardNo, NumberStyles.HexNumber) + "\t");
					}
					break;
				default:
					stringBuilder.Append("CardNo=" + model.CardNo + "\t");
					break;
				}
				stringBuilder.Append("Pin=" + model.Pin + "\t");
				if (Dev.device_type != 1 && Dev.device_type != 2 && Dev.device_type != 4 && Dev.device_type != 7)
				{
					stringBuilder.Append("Name=" + model.Name + "\t");
				}
				stringBuilder.Append("Password=" + model.Password + "\t");
				stringBuilder.Append("Group=" + model.Group + "\t");
				if (Dev.device_type == 12 || Dev.device_type == 101 || Dev.device_type == 102 || Dev.device_type == 103 || Dev.device_type == 1000)
				{
					stringBuilder.Append("Privilege=" + model.Privilege + "\t");
				}
				stringBuilder.Append("StartTime=" + model.StartTime + "\t");
				stringBuilder.Append("EndTime=" + model.EndTime + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddUser(List<ObjUser> list, Machines Dev)
		{
			if (list != null && list.Count > 0)
			{
				ObjUser objUser = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objUser = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddUser(objUser, Dev));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUser(ObjUser model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Pin=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteUser(List<ObjUser> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjUser objUser = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objUser = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteUser(objUser));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddFaceTemp(ObjFaceTemp model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Size=" + model.Size + "\t");
				stringBuilder.Append("FaceID=" + model.FaceID + "\t");
				stringBuilder.Append("Valid=" + model.Valid + "\t");
				stringBuilder.Append("Reserved=" + model.Reserved + "\t");
				stringBuilder.Append("ActiveTime=" + model.ActiveTime + "\t");
				stringBuilder.Append("VfCount=" + model.VfCount + "\t");
				stringBuilder.Append("Pin=" + model.Pin + "\t");
				stringBuilder.Append("Face=" + model.Face + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddFaceTemp(List<ObjFaceTemp> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjFaceTemp objFaceTemp = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objFaceTemp = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddFaceTemp(objFaceTemp));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFaceTemp(ObjUser model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Pin=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFaceTemp(ObjFaceTemp model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("FaceID=" + model.FaceID + "\t");
				stringBuilder.Append("Pin=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFaceTemp(List<ObjFaceTemp> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjFaceTemp objFaceTemp = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objFaceTemp = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteFaceTemp(objFaceTemp));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddFingerVein(ObjFVTemplate model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Size=" + model.Size + "\t");
				stringBuilder.Append("PIN2=" + model.Pin2 + "\t");
				stringBuilder.Append("FingerID=" + model.FingerID + "\t");
				stringBuilder.Append("Duress=" + model.Duress + "\t");
				stringBuilder.Append("UserCode=" + model.UserCode + "\t");
				stringBuilder.Append("Fv=" + model.Fv + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string AddFingerVein(List<ObjFVTemplate> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjFVTemplate objFVTemplate = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objFVTemplate = list[i];
					stringBuilder.Append(PullSDKDataConvert.AddFingerVein(objFVTemplate));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFingerVein(ObjUser model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("PIN2=" + model.Pin + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFingerVein(ObjFVTemplate model)
		{
			if (model != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("FingerID=" + model.FingerID + "\t");
				stringBuilder.Append("PIN2=" + model.Pin2 + "#");
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string DeleteFingerVein(List<ObjFVTemplate> list)
		{
			if (list != null && list.Count > 0)
			{
				ObjFVTemplate objFVTemplate = null;
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < list.Count; i++)
				{
					objFVTemplate = list[i];
					stringBuilder.Append(PullSDKDataConvert.DeleteFingerVein(objFVTemplate));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}
	}
}
