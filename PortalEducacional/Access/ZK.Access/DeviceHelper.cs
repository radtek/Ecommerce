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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Data.Model.StdSDK;
using ZK.MachinesManager.UDisk;
using ZK.Utils;
using ZK.Utils.Dialogs;

namespace ZK.Access
{
	public class DeviceHelper
	{
		public delegate void ShowInfoHandler(string info);

		public delegate void ShowProgressHandler(int progress);

		private static AccTimeseg m_time = null;

		private static int m_errorCode = -1002;

		public static AccTimeseg TimeSeg
		{
			get
			{
				if (DeviceHelper.m_time == null)
				{
					AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
					DeviceHelper.m_time = accTimesegBll.GetdefaultTime();
				}
				if (DeviceHelper.m_time == null)
				{
					DeviceHelper.m_time = new AccTimeseg();
					DeviceHelper.m_time.id = 1;
				}
				return DeviceHelper.m_time;
			}
		}

		public static int SyncDeviceTime(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				DateTime now = DateTime.Now;
				result = deviceParamBll.SetDateTime(now);
			}
			return result;
		}

		public static int SetBaudrate(DeviceServerBll devServerBll, int newBaudrate)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				result = deviceParamBll.SetBaudrate(newBaudrate);
			}
			return result;
		}

		public static int SetPasswd(DeviceServerBll devServerBll, string passwd)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				result = deviceParamBll.SetPasswd(passwd);
			}
			return result;
		}

		public static int SetWiegand(DeviceServerBll devServerBll)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
				MachinesBll machinesBll = new MachinesBll(devServerBll.Application);
				Machines model = machinesBll.GetModel(devServerBll.DevInfo.ID);
				for (int i = 0; i < modelList.Count; i++)
				{
					string deviceParam = PullSDKDataConvertHelper.SetWiegandParam(modelList[i], model);
					num = devServerBll.SetDeviceParam(deviceParam);
					if (num < 0)
					{
						break;
					}
				}
			}
			return num;
		}

		public static int SetMasterSlave(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				MachinesBll machinesBll = new MachinesBll(devServerBll.Application);
				Machines model = machinesBll.GetModel(devServerBll.DevInfo.ID);
				if (model != null && model.deviceOption != null && (model.device_type == 101 || model.device_type == 102 || model.device_type == 103))
				{
					byte[] deviceOption = model.deviceOption;
					string @string = Encoding.ASCII.GetString(deviceOption, 0, deviceOption.Length);
					if (!string.IsNullOrEmpty(@string))
					{
						result = devServerBll.SetDeviceParam(@string);
					}
				}
			}
			return result;
		}

		public static int DeleteDeviceData(DeviceServerBll devServerBll, bool isUptemplate)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				num = DeviceHelper.DeleteFirstCard(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteHoliday(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteInOutFun(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteMultimCard(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteUserAuthorize(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteUserInfo(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteTimeZone(devServerBll);
				if (num < 0)
				{
					return num;
				}
				if (isUptemplate)
				{
					num = DeviceHelper.DeleteTemplate(devServerBll);
					if (num < 0)
					{
						return num;
					}
				}
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				deviceParamBll.SetAntiPassback(0);
				deviceParamBll.SetInterLock(0);
			}
			return num;
		}

		public static int DeleteDeviceData(DeviceServerBll devServerBll)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				num = DeviceHelper.DeleteFirstCard(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteHoliday(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteInOutFun(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteMultimCard(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteUserAuthorize(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteTimeZone(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteUserInfo(devServerBll);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteUserFingerVein(devServerBll);
				if (num < 0)
				{
					return num;
				}
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				deviceParamBll.SetAntiPassback(0);
				deviceParamBll.SetInterLock(0);
			}
			return num;
		}

		public static int DeleteTemplate(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				TemplateV10Bll templateV10Bll = new TemplateV10Bll(devServerBll.Application);
				result = templateV10Bll.Delete();
			}
			return result;
		}

		public static int DeleteFaceTemplate(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				ZK.Data.BLL.PullSDK.FaceTempBll faceTempBll = new ZK.Data.BLL.PullSDK.FaceTempBll(devServerBll.Application);
				result = faceTempBll.Delete();
			}
			return result;
		}

		public static int DeleteTimeZone(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				TimeZoneBll timeZoneBll = new TimeZoneBll(devServerBll.Application);
				result = timeZoneBll.Delete();
			}
			return result;
		}

		public static int DeleteUserInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				UserBll userBll = new UserBll(devServerBll.Application);
				result = userBll.Delete();
			}
			return result;
		}

		public static int DeleteUserAuthorize(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				UserAuthorizeBll userAuthorizeBll = new UserAuthorizeBll(devServerBll.Application);
				result = userAuthorizeBll.Delete();
			}
			return result;
		}

		public static int DeleteTransAction(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				TransActionBll transActionBll = new TransActionBll(devServerBll.Application);
				result = transActionBll.Delete();
			}
			return result;
		}

		public static int DeleteMultimCard(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				MultimCardBll multimCardBll = new MultimCardBll(devServerBll.Application);
				result = multimCardBll.Delete();
			}
			return result;
		}

		public static int DeleteInOutFun(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				InOutFunBll inOutFunBll = new InOutFunBll(devServerBll.Application);
				result = inOutFunBll.Delete();
			}
			return result;
		}

		public static int DeleteHoliday(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				HolidayBll holidayBll = new HolidayBll(devServerBll.Application);
				result = holidayBll.Delete();
			}
			return result;
		}

		public static int DeleteFirstCard(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				FirstCardBll firstCardBll = new FirstCardBll(devServerBll.Application);
				result = firstCardBll.Delete();
			}
			return result;
		}

		public static int DeleteUserFingerVein(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				FVTemplateBll fVTemplateBll = new FVTemplateBll(devServerBll.Application);
				result = fVTemplateBll.Delete();
			}
			return result;
		}

		public static int UpdateDoorSetings(DeviceServerBll devServerBll)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				num = 0;
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
				for (int i = 0; i < modelList.Count; i++)
				{
					num = DeviceHelper.UpdateDoorSetings(devServerBll, modelList[i]);
					if (num < 0)
					{
						return num;
					}
				}
			}
			return num;
		}

		public static int UpdateDoorSetings(AccDoor door)
		{
			int result = DeviceHelper.m_errorCode;
			if (door != null)
			{
				result = 0;
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				Machines model = machinesBll.GetModel(door.device_id);
				if (model != null)
				{
					DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
					if (deviceServer != null)
					{
						return DeviceHelper.UpdateDoorSetings(deviceServer, door);
					}
				}
			}
			return result;
		}

		public static ObjDoorInfo CreateDoor(int no)
		{
			ObjDoorInfo result = null;
			switch (no)
			{
			case 1:
				result = new ObjDoorInfo(DoorType.Door1);
				break;
			case 2:
				result = new ObjDoorInfo(DoorType.Door2);
				break;
			case 3:
				result = new ObjDoorInfo(DoorType.Door3);
				break;
			case 4:
				result = new ObjDoorInfo(DoorType.Door4);
				break;
			}
			return result;
		}

		public static int UpdateDoorSetings(DeviceServerBll devServerBll, AccDoor door)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
				AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
				ObjDoorInfo objDoorInfo = DeviceHelper.CreateDoor(door.door_no);
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				Machines model = machinesBll.GetModel(door.device_id);
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
					result = deviceParamBll.SetDoorParam(objDoorInfo, model);
				}
			}
			return result;
		}

		public static int SetInterLockInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				AccInterlockBll accInterlockBll = new AccInterlockBll(MainForm._ia);
				List<AccInterlock> modelList = accInterlockBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				result = ((modelList == null || modelList.Count <= 0) ? deviceParamBll.SetInterLock(0) : deviceParamBll.SetInterLock(modelList[0].InterlockType));
			}
			return result;
		}

		public static int SetAntiPassbackInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				AccAntibackBll accAntibackBll = new AccAntibackBll(MainForm._ia);
				List<AccAntiback> modelList = accAntibackBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServerBll.Application);
				result = ((modelList == null || modelList.Count <= 0) ? deviceParamBll.SetAntiPassback(0) : deviceParamBll.SetAntiPassback(modelList[0].AntibackType));
			}
			return result;
		}

		public static int SetHolidayInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
				List<AccHolidays> modelList = accHolidaysBll.GetModelList("");
				List<ObjHoliday> list = new List<ObjHoliday>();
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						list.AddRange(DeviceHelper.HolidayConvert(modelList[i]));
					}
				}
				HolidayBll holidayBll = new HolidayBll(devServerBll.Application);
				result = ((list.Count > 0) ? holidayBll.Add(list) : 0);
			}
			return result;
		}

		public static List<ObjHoliday> HolidayConvert(AccHolidays holiday)
		{
			List<ObjHoliday> list = new List<ObjHoliday>();
			if (holiday != null)
			{
				DateTime value = holiday.start_date.Value;
				DateTime value2 = holiday.end_date.Value;
				int num = (value2 - value).Days + 1;
				for (int i = 0; i < num; i++)
				{
					ObjHoliday objHoliday = new ObjHoliday();
					objHoliday.HolidayType = (HolidayType)holiday.holiday_type;
					objHoliday.Loop = (LoopType)holiday.loop_by_year;
					DateTime dateTime = value.AddDays((double)i);
					objHoliday.Holiday = (dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day).ToString();
					list.Add(objHoliday);
				}
			}
			return list;
		}

		public static long ZKDateTimeConvert(DateTime startTime, DateTime endTime)
		{
			if (endTime <= startTime)
			{
				return 0L;
			}
			if (startTime.Hour == endTime.Hour && startTime.Minute == endTime.Minute)
			{
				return 0L;
			}
			return (startTime.Hour * 100 + startTime.Minute << 16) + (endTime.Hour * 100 + endTime.Minute);
		}

		public static long ZKDateTimeConvert()
		{
			return 2359L;
		}

		public static ObjTimeZone ZKDateTimeConvert(AccTimeseg timeSet)
		{
			if (timeSet != null)
			{
				ObjTimeZone objTimeZone = new ObjTimeZone();
				objTimeZone.TimezoneId = timeSet.id.ToString();
				ObjTimeZone objTimeZone2 = objTimeZone;
				long num = DeviceHelper.ZKDateTimeConvert(timeSet.sunday_start1.Value, timeSet.sunday_end1.Value);
				objTimeZone2.SunTime1 = num.ToString();
				ObjTimeZone objTimeZone3 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.sunday_start2.Value, timeSet.sunday_end2.Value);
				objTimeZone3.SunTime2 = num.ToString();
				ObjTimeZone objTimeZone4 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.sunday_start3.Value, timeSet.sunday_end3.Value);
				objTimeZone4.SunTime3 = num.ToString();
				ObjTimeZone objTimeZone5 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.monday_start1.Value, timeSet.monday_end1.Value);
				objTimeZone5.MonTime1 = num.ToString();
				ObjTimeZone objTimeZone6 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.monday_start2.Value, timeSet.monday_end2.Value);
				objTimeZone6.MonTime2 = num.ToString();
				ObjTimeZone objTimeZone7 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.monday_start3.Value, timeSet.monday_end3.Value);
				objTimeZone7.MonTime3 = num.ToString();
				ObjTimeZone objTimeZone8 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.tuesday_start1.Value, timeSet.tuesday_end1.Value);
				objTimeZone8.TueTime1 = num.ToString();
				ObjTimeZone objTimeZone9 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.tuesday_start2.Value, timeSet.tuesday_end2.Value);
				objTimeZone9.TueTime2 = num.ToString();
				ObjTimeZone objTimeZone10 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.tuesday_start3.Value, timeSet.tuesday_end3.Value);
				objTimeZone10.TueTime3 = num.ToString();
				ObjTimeZone objTimeZone11 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.wednesday_start1.Value, timeSet.wednesday_end1.Value);
				objTimeZone11.WedTime1 = num.ToString();
				ObjTimeZone objTimeZone12 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.wednesday_start2.Value, timeSet.wednesday_end2.Value);
				objTimeZone12.WedTime2 = num.ToString();
				ObjTimeZone objTimeZone13 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.wednesday_start3.Value, timeSet.wednesday_end3.Value);
				objTimeZone13.WedTime3 = num.ToString();
				ObjTimeZone objTimeZone14 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.thursday_start1.Value, timeSet.thursday_end1.Value);
				objTimeZone14.ThuTime1 = num.ToString();
				ObjTimeZone objTimeZone15 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.thursday_start2.Value, timeSet.thursday_end2.Value);
				objTimeZone15.ThuTime2 = num.ToString();
				ObjTimeZone objTimeZone16 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.thursday_start3.Value, timeSet.thursday_end3.Value);
				objTimeZone16.ThuTime3 = num.ToString();
				ObjTimeZone objTimeZone17 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.friday_start1.Value, timeSet.friday_end1.Value);
				objTimeZone17.FriTime1 = num.ToString();
				ObjTimeZone objTimeZone18 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.friday_start2.Value, timeSet.friday_end2.Value);
				objTimeZone18.FriTime2 = num.ToString();
				ObjTimeZone objTimeZone19 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.friday_start3.Value, timeSet.friday_end3.Value);
				objTimeZone19.FriTime3 = num.ToString();
				ObjTimeZone objTimeZone20 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.saturday_start1.Value, timeSet.saturday_end1.Value);
				objTimeZone20.SatTime1 = num.ToString();
				ObjTimeZone objTimeZone21 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.saturday_start2.Value, timeSet.saturday_end2.Value);
				objTimeZone21.SatTime2 = num.ToString();
				ObjTimeZone objTimeZone22 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.saturday_start3.Value, timeSet.saturday_end3.Value);
				objTimeZone22.SatTime3 = num.ToString();
				ObjTimeZone objTimeZone23 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype1_start1.Value, timeSet.holidaytype1_end1.Value);
				objTimeZone23.Hol1Time1 = num.ToString();
				ObjTimeZone objTimeZone24 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype1_start2.Value, timeSet.holidaytype1_end2.Value);
				objTimeZone24.Hol1Time2 = num.ToString();
				ObjTimeZone objTimeZone25 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype1_start3.Value, timeSet.holidaytype1_end3.Value);
				objTimeZone25.Hol1Time3 = num.ToString();
				ObjTimeZone objTimeZone26 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype2_start1.Value, timeSet.holidaytype2_end1.Value);
				objTimeZone26.Hol2Time1 = num.ToString();
				ObjTimeZone objTimeZone27 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype2_start2.Value, timeSet.holidaytype2_end2.Value);
				objTimeZone27.Hol2Time2 = num.ToString();
				ObjTimeZone objTimeZone28 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype2_start3.Value, timeSet.holidaytype2_end3.Value);
				objTimeZone28.Hol2Time3 = num.ToString();
				ObjTimeZone objTimeZone29 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype3_start1.Value, timeSet.holidaytype3_end1.Value);
				objTimeZone29.Hol3Time1 = num.ToString();
				ObjTimeZone objTimeZone30 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype3_start2.Value, timeSet.holidaytype3_end2.Value);
				objTimeZone30.Hol3Time2 = num.ToString();
				ObjTimeZone objTimeZone31 = objTimeZone;
				num = DeviceHelper.ZKDateTimeConvert(timeSet.holidaytype3_start3.Value, timeSet.holidaytype3_end3.Value);
				objTimeZone31.Hol3Time3 = num.ToString();
				if (DeviceHelper.TimeSeg.id == timeSet.id)
				{
					objTimeZone.TimezoneId = "1";
				}
				return objTimeZone;
			}
			return null;
		}

		public static ObjTimeZone Get24HourDateTime()
		{
			ObjTimeZone objTimeZone = new ObjTimeZone();
			objTimeZone.TimezoneId = "1";
			ObjTimeZone objTimeZone2 = objTimeZone;
			long num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone2.SunTime1 = num.ToString();
			objTimeZone.SunTime2 = "0";
			objTimeZone.SunTime3 = "0";
			ObjTimeZone objTimeZone3 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone3.MonTime1 = num.ToString();
			objTimeZone.MonTime2 = "0";
			objTimeZone.MonTime3 = "0";
			ObjTimeZone objTimeZone4 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone4.TueTime1 = num.ToString();
			objTimeZone.TueTime2 = "0";
			objTimeZone.TueTime3 = "0";
			ObjTimeZone objTimeZone5 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone5.WedTime1 = num.ToString();
			objTimeZone.WedTime2 = "0";
			objTimeZone.WedTime3 = "0";
			ObjTimeZone objTimeZone6 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone6.ThuTime1 = num.ToString();
			objTimeZone.ThuTime2 = "0";
			objTimeZone.ThuTime3 = "0";
			ObjTimeZone objTimeZone7 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone7.FriTime1 = num.ToString();
			objTimeZone.FriTime2 = "0";
			objTimeZone.FriTime3 = "0";
			ObjTimeZone objTimeZone8 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone8.SatTime1 = num.ToString();
			objTimeZone.SatTime2 = "0";
			objTimeZone.SatTime3 = "0";
			ObjTimeZone objTimeZone9 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone9.Hol1Time1 = num.ToString();
			objTimeZone.Hol1Time2 = "0";
			objTimeZone.Hol1Time3 = "0";
			ObjTimeZone objTimeZone10 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone10.Hol2Time1 = num.ToString();
			objTimeZone.Hol2Time2 = "0";
			objTimeZone.Hol2Time3 = "0";
			ObjTimeZone objTimeZone11 = objTimeZone;
			num = DeviceHelper.ZKDateTimeConvert();
			objTimeZone11.Hol3Time1 = num.ToString();
			objTimeZone.Hol3Time2 = "0";
			objTimeZone.Hol3Time3 = "0";
			return objTimeZone;
		}

		public static int SetTZInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				TimeZoneBll timeZoneBll = new TimeZoneBll(devServerBll.Application);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				List<ObjTimeZone> list = new List<ObjTimeZone>();
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						ObjTimeZone item = DeviceHelper.ZKDateTimeConvert(modelList[i]);
						list.Add(item);
					}
				}
				else
				{
					timeZoneBll.Add(DeviceHelper.Get24HourDateTime());
				}
				result = ((list.Count > 0) ? timeZoneBll.Add(list) : 0);
			}
			return result;
		}

		public static Dictionary<int, AccDoor> GetDoors()
		{
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					dictionary.Add(modelList[i].id, modelList[i]);
				}
			}
			return dictionary;
		}

		public static List<ObjFirstCard> GetFirstCardInfo(AccFirstOpen model)
		{
			Dictionary<int, AccDoor> doors = DeviceHelper.GetDoors();
			AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
			UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
			Dictionary<int, string> userId_BadgenumberDic = userInfoBll.GetUserId_BadgenumberDic("");
			List<ObjFirstCard> list = new List<ObjFirstCard>();
			List<AccFirstOpenEmp> modelList = accFirstOpenEmpBll.GetModelList("accfirstopen_id=" + model.id);
			for (int i = 0; i < modelList.Count; i++)
			{
				if (userId_BadgenumberDic.ContainsKey(modelList[i].employee_id) && doors.ContainsKey(model.door_id))
				{
					ObjFirstCard objFirstCard = new ObjFirstCard();
					ObjFirstCard objFirstCard2 = objFirstCard;
					int num = model.timeseg_id;
					objFirstCard2.TimezoneID = num.ToString();
					if (DeviceHelper.TimeSeg.id == model.timeseg_id)
					{
						objFirstCard.TimezoneID = "1";
					}
					ObjFirstCard objFirstCard3 = objFirstCard;
					num = doors[model.door_id].door_no;
					objFirstCard3.DoorID = num.ToString();
					objFirstCard.Pin = userId_BadgenumberDic[modelList[i].employee_id];
					list.Add(objFirstCard);
				}
			}
			return list;
		}

		public static int SetNormalOpenInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				Dictionary<int, AccDoor> doors = DeviceHelper.GetDoors();
				AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
				List<AccFirstOpen> modelList = accFirstOpenBll.GetModelList("door_id in (select id from acc_door where  device_id=" + devServerBll.DevInfo.ID + ")");
				if (modelList != null && modelList.Count > 0)
				{
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
					List<ObjFirstCard> list = new List<ObjFirstCard>();
					for (int i = 0; i < modelList.Count; i++)
					{
						AccFirstOpenEmpBll accFirstOpenEmpBll2 = accFirstOpenEmpBll;
						int num = modelList[i].id;
						List<AccFirstOpenEmp> modelList2 = accFirstOpenEmpBll2.GetModelList("accfirstopen_id=" + num.ToString());
						for (int j = 0; j < modelList2.Count; j++)
						{
							UserInfo model = userInfoBll.GetModel(modelList2[j].employee_id);
							if (model != null && doors.ContainsKey(modelList[i].door_id))
							{
								ObjFirstCard objFirstCard = new ObjFirstCard();
								ObjFirstCard objFirstCard2 = objFirstCard;
								num = modelList[i].timeseg_id;
								objFirstCard2.TimezoneID = num.ToString();
								if (DeviceHelper.TimeSeg.id == modelList[i].timeseg_id)
								{
									objFirstCard.TimezoneID = "1";
								}
								ObjFirstCard objFirstCard3 = objFirstCard;
								num = doors[modelList[i].door_id].door_no;
								objFirstCard3.DoorID = num.ToString();
								objFirstCard.Pin = model.BadgeNumber;
								list.Add(objFirstCard);
							}
						}
					}
					FirstCardBll firstCardBll = new FirstCardBll(devServerBll.Application);
					if (list.Count > 0)
					{
						result = firstCardBll.Add(list);
					}
				}
			}
			return result;
		}

		public static ObjUser UserDataConvert(UserInfo user)
		{
			ObjUser objUser = new ObjUser();
			ObjUser objUser2 = objUser;
			int num = user.UserId;
			objUser2.Id = num.ToString();
			objUser.Pin = user.BadgeNumber;
			objUser.Name = user.Name;
			objUser.CardNo = user.CardNo;
			objUser.Privilege = user.Privilege;
			if (string.IsNullOrEmpty(objUser.CardNo))
			{
				objUser.CardNo = string.Empty;
			}
			if (AccCommon.IsECardTong == 1)
			{
				objUser.Password = user.MVerifyPass;
			}
			else
			{
				objUser.Password = user.PassWord;
			}
			if (string.IsNullOrEmpty(objUser.Password))
			{
				objUser.Password = string.Empty;
			}
			ObjUser objUser3 = objUser;
			num = user.MorecardGroupId;
			objUser3.Group = num.ToString();
			if (user.AccStartDate.HasValue && user.AccEndDate.HasValue)
			{
				DateTime value = user.AccStartDate.Value;
				ObjUser objUser4 = objUser;
				num = value.Year;
				string str = num.ToString();
				num = value.Month;
				string str2 = num.ToString("00");
				num = value.Day;
				objUser4.StartTime = str + str2 + num.ToString("00");
				value = user.AccEndDate.Value;
				ObjUser objUser5 = objUser;
				num = value.Year;
				string str3 = num.ToString();
				num = value.Month;
				string str4 = num.ToString("00");
				num = value.Day;
				objUser5.EndTime = str3 + str4 + num.ToString("00");
			}
			else
			{
				objUser.StartTime = string.Empty;
				objUser.EndTime = string.Empty;
			}
			return objUser;
		}

		public static int DelUserTemplate(DeviceServerBll devServerBll, ObjUser devUserModel)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				TemplateV10Bll templateV10Bll = new TemplateV10Bll(devServerBll.Application);
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				objTemplateV.Pin = devUserModel.Pin;
				result = templateV10Bll.Delete(objTemplateV);
			}
			return result;
		}

		public static int DelUserTemplate(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				TemplateV10Bll templateV10Bll = new TemplateV10Bll(devServerBll.Application);
				List<ObjTemplateV10> list = new List<ObjTemplateV10>();
				for (int i = 0; i < devUsers.Count; i++)
				{
					ObjTemplateV10 objTemplateV = new ObjTemplateV10();
					objTemplateV.Pin = devUsers[i].Pin;
					list.Add(objTemplateV);
				}
				result = templateV10Bll.Delete(list);
			}
			return result;
		}

		public static ObjTemplateV10 UserTemplateConvert(Template template)
		{
			if (template != null)
			{
				ObjTemplateV10 objTemplateV = new ObjTemplateV10();
				objTemplateV.FingerID = (FingerType)Enum.Parse(typeof(FingerType), template.FINGERID.ToString());
				objTemplateV.Pin = template.Pin;
				objTemplateV.Valid = (ValidType)Enum.Parse(typeof(ValidType), template.Flag.ToString());
				if (template.TEMPLATE4 != null)
				{
					objTemplateV.Template = Convert.ToBase64String(template.TEMPLATE4);
				}
				else if (template.TEMPLATE3 != null)
				{
					objTemplateV.Template = Convert.ToBase64String(template.TEMPLATE3);
				}
				else
				{
					objTemplateV.Template = "";
				}
				return objTemplateV;
			}
			return null;
		}

		public static int SetUserTemplate(DeviceServerBll devServerBll, ObjUser devUserModel)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null && devUserModel != null && !string.IsNullOrEmpty(devUserModel.Id))
			{
				result = 0;
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				List<ObjTemplateV10> list = new List<ObjTemplateV10>();
				List<Template> list2 = null;
				try
				{
					list2 = templateBll.GetModelList("userid=" + devUserModel.Id);
				}
				catch
				{
				}
				if (list2 != null && list2.Count > 0)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						ObjTemplateV10 objTemplateV = new ObjTemplateV10();
						objTemplateV.FingerID = (FingerType)Enum.Parse(typeof(FingerType), list2[i].FINGERID.ToString());
						objTemplateV.Pin = devUserModel.Pin;
						objTemplateV.Valid = (ValidType)Enum.Parse(typeof(ValidType), list2[i].Flag.ToString());
						objTemplateV.Template = Convert.ToBase64String(list2[i].TEMPLATE4);
						list.Add(objTemplateV);
					}
					TemplateV10Bll templateV10Bll = new TemplateV10Bll(devServerBll.Application);
					result = templateV10Bll.Add(list);
				}
			}
			return result;
		}

		public static int SetUserTemplate(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null && devUsers != null && devUsers.Count > 0)
			{
				num = 0;
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				List<ObjTemplateV10> list = new List<ObjTemplateV10>();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("userid in(" + devUsers[0].Id);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(devUsers[0].Id, devUsers[0].Pin);
				for (int i = 1; i < devUsers.Count; i++)
				{
					if (!string.IsNullOrEmpty(devUsers[i].Id) && !dictionary.ContainsKey(devUsers[i].Id))
					{
						stringBuilder.Append("," + devUsers[i].Id);
						dictionary.Add(devUsers[i].Id, devUsers[i].Pin);
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
					for (int j = 0; j < list2.Count; j++)
					{
						ObjTemplateV10 objTemplateV = new ObjTemplateV10();
						ObjTemplateV10 objTemplateV2 = objTemplateV;
						Type typeFromHandle = typeof(FingerType);
						int num2 = list2[j].FINGERID;
						objTemplateV2.FingerID = (FingerType)Enum.Parse(typeFromHandle, num2.ToString());
						Dictionary<string, string> dictionary2 = dictionary;
						num2 = list2[j].USERID;
						if (dictionary2.ContainsKey(num2.ToString()))
						{
							ObjTemplateV10 objTemplateV3 = objTemplateV;
							Dictionary<string, string> dictionary3 = dictionary;
							num2 = list2[j].USERID;
							objTemplateV3.Pin = dictionary3[num2.ToString()];
							objTemplateV.Valid = (ValidType)Enum.Parse(typeof(ValidType), list2[j].Flag.ToString());
							if (list2[j].TEMPLATE4 != null)
							{
								objTemplateV.Template = Convert.ToBase64String(list2[j].TEMPLATE4);
								list.Add(objTemplateV);
							}
						}
					}
				}
				TemplateV10Bll templateV10Bll = new TemplateV10Bll(devServerBll.Application);
				if (list.Count > 0)
				{
					num = templateV10Bll.Add(list);
					if (num >= 0)
					{
						return list.Count;
					}
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		public static int SetUserFaceTemplate(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null && devUsers != null && devUsers.Count > 0)
			{
				num = 0;
				ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
				List<ObjFaceTemp> list = new List<ObjFaceTemp>();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("UserNo in('" + devUsers[0].Id + "'");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(devUsers[0].Id, devUsers[0].Pin);
				for (int i = 1; i < devUsers.Count; i++)
				{
					if (!string.IsNullOrEmpty(devUsers[i].Id) && !dictionary.ContainsKey(devUsers[i].Id))
					{
						stringBuilder.Append(",'" + devUsers[i].Id + "'");
						dictionary.Add(devUsers[i].Id, devUsers[i].Pin);
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
				if (list2 != null && list2.Count > 0)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						ObjFaceTemp objFaceTemp = new ObjFaceTemp();
						if (dictionary.ContainsKey(list2[j].UserId.ToString()) && list2[j].Template != null)
						{
							list.AddRange(FaceDataConverter.DBFace2PullFace(list2[j]));
						}
					}
				}
				ZK.Data.BLL.PullSDK.FaceTempBll faceTempBll2 = new ZK.Data.BLL.PullSDK.FaceTempBll(devServerBll.Application);
				if (list.Count > 0)
				{
					num = faceTempBll2.Add(list);
					if (num >= 0)
					{
						return list.Count;
					}
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		public static ObjFaceTemp UserFaceTempConvert(FaceTemp model)
		{
			if (model != null)
			{
				ObjFaceTemp objFaceTemp = new ObjFaceTemp();
				objFaceTemp.ActiveTime = model.ActiveTime;
				objFaceTemp.Face = Convert.ToBase64String(model.Template);
				objFaceTemp.FaceID = model.FaceId;
				objFaceTemp.Pin = model.Pin.ToString();
				objFaceTemp.Reserved = model.Reserve;
				objFaceTemp.Size = model.Size;
				objFaceTemp.Valid = model.Valid;
				objFaceTemp.VfCount = model.VFCOUNT;
				return objFaceTemp;
			}
			return null;
		}

		public static ObjFVTemplate UserFingerVeinConvert(FingerVein model)
		{
			if (model != null)
			{
				ObjFVTemplate objFVTemplate = new ObjFVTemplate();
				objFVTemplate.Size = model.Size;
				objFVTemplate.FingerID = model.FingerID;
				objFVTemplate.Pin2 = model.Pin;
				objFVTemplate.Duress = model.DuressFlag;
				objFVTemplate.UserCode = model.UserCode;
				objFVTemplate.Fv = Convert.ToBase64String(model.Template);
				return objFVTemplate;
			}
			return null;
		}

		public static int SetUserFingerVein(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServerBll != null && devUsers != null && devUsers.Count > 0)
			{
				num = 0;
				FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
				List<ObjFVTemplate> list = new List<ObjFVTemplate>();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("userid in(" + devUsers[0].Id);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(devUsers[0].Id, devUsers[0].Pin);
				for (int i = 1; i < devUsers.Count; i++)
				{
					if (!string.IsNullOrEmpty(devUsers[i].Id) && !dictionary.ContainsKey(devUsers[i].Id))
					{
						stringBuilder.Append("," + devUsers[i].Id);
						dictionary.Add(devUsers[i].Id, devUsers[i].Pin);
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
				if (list2 != null && list2.Count > 0)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						ObjFVTemplate objFVTemplate = new ObjFVTemplate();
						objFVTemplate.Size = list2[j].Size;
						objFVTemplate.FingerID = list2[j].FingerID;
						Dictionary<string, string> dictionary2 = dictionary;
						int userID = list2[j].UserID;
						if (dictionary2.ContainsKey(userID.ToString()))
						{
							ObjFVTemplate objFVTemplate2 = objFVTemplate;
							Dictionary<string, string> dictionary3 = dictionary;
							userID = list2[j].UserID;
							objFVTemplate2.Pin2 = dictionary3[userID.ToString()];
							objFVTemplate.Duress = list2[j].DuressFlag;
							objFVTemplate.UserCode = list2[j].UserCode;
							if (list2[j].Template != null)
							{
								objFVTemplate.Fv = Convert.ToBase64String(list2[j].Template);
								list.Add(objFVTemplate);
							}
							objFVTemplate.Fv_ID_Index = list2[j].Fv_ID_Index;
						}
					}
				}
				FVTemplateBll fVTemplateBll = new FVTemplateBll(devServerBll.Application);
				if (list.Count > 0)
				{
					num = fVTemplateBll.Add(list);
					if (num >= 0)
					{
						return list.Count;
					}
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		public static ObjInOutFun InOutFunConvert(AccLinkAgeIo LinkAgeIO)
		{
			ObjInOutFun objInOutFun = new ObjInOutFun();
			if (LinkAgeIO != null)
			{
				ObjInOutFun objInOutFun2 = objInOutFun;
				int num = LinkAgeIO.id;
				objInOutFun2.Index = num.ToString();
				objInOutFun.EventType = (EventType)LinkAgeIO.trigger_opt;
				objInOutFun.InAddr = (InAddrType)LinkAgeIO.in_address;
				objInOutFun.OutType = (OutType)LinkAgeIO.out_type_hide;
				objInOutFun.OutAddr = (OutAddrType)LinkAgeIO.out_address;
				ObjInOutFun objInOutFun3 = objInOutFun;
				num = LinkAgeIO.delay_time;
				objInOutFun3.OutTime = num.ToString();
				return objInOutFun;
			}
			return null;
		}

		public static int SetInOutFunInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				result = 0;
				AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
				List<AccLinkAgeIo> modelList = accLinkAgeIoBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
				if (modelList != null && modelList.Count > 0)
				{
					List<ObjInOutFun> list = new List<ObjInOutFun>();
					for (int i = 0; i < modelList.Count; i++)
					{
						ObjInOutFun objInOutFun = new ObjInOutFun();
						ObjInOutFun objInOutFun2 = objInOutFun;
						int num = modelList[i].id;
						objInOutFun2.Index = num.ToString();
						objInOutFun.EventType = (EventType)modelList[i].trigger_opt;
						objInOutFun.InAddr = (InAddrType)modelList[i].in_address;
						objInOutFun.OutType = (OutType)modelList[i].out_type_hide;
						objInOutFun.OutAddr = (OutAddrType)modelList[i].out_address;
						ObjInOutFun objInOutFun3 = objInOutFun;
						num = modelList[i].delay_time;
						objInOutFun3.OutTime = num.ToString();
						list.Add(objInOutFun);
					}
					InOutFunBll inOutFunBll = new InOutFunBll(devServerBll.Application);
					result = inOutFunBll.Add(list);
				}
			}
			return result;
		}

		public static ObjMultimCard MultimCardConvert(AccMorecardset model, Machines machine = null)
		{
			AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
			List<AccMorecardGroup> modelList = accMorecardGroupBll.GetModelList("comb_id=" + model.id);
			if (modelList != null && modelList.Count > 0)
			{
				Dictionary<int, AccDoor> doors = DeviceHelper.GetDoors();
				ObjMultimCard objMultimCard = new ObjMultimCard();
				ObjMultimCard objMultimCard2 = objMultimCard;
				int num = doors[model.door_id].door_no;
				objMultimCard2.DoorId = num.ToString();
				ObjMultimCard objMultimCard3 = objMultimCard;
				num = model.id;
				objMultimCard3.Index = num.ToString();
				if (machine != null && machine.DevSDKType == SDKType.StandaloneSDK)
				{
					ObjMultimCard objMultimCard4 = objMultimCard;
					num = model.CombNo;
					objMultimCard4.Index = num.ToString();
				}
				int num2 = 0;
				for (int i = 0; i < modelList.Count; i++)
				{
					for (int j = 0; j < modelList[i].opener_number; j++)
					{
						switch (num2)
						{
						case 0:
						{
							ObjMultimCard objMultimCard9 = objMultimCard;
							num = modelList[i].group_id;
							objMultimCard9.Group1 = num.ToString();
							break;
						}
						case 1:
						{
							ObjMultimCard objMultimCard8 = objMultimCard;
							num = modelList[i].group_id;
							objMultimCard8.Group2 = num.ToString();
							break;
						}
						case 2:
						{
							ObjMultimCard objMultimCard7 = objMultimCard;
							num = modelList[i].group_id;
							objMultimCard7.Group3 = num.ToString();
							break;
						}
						case 3:
						{
							ObjMultimCard objMultimCard6 = objMultimCard;
							num = modelList[i].group_id;
							objMultimCard6.Group4 = num.ToString();
							break;
						}
						case 4:
						{
							ObjMultimCard objMultimCard5 = objMultimCard;
							num = modelList[i].group_id;
							objMultimCard5.Group5 = num.ToString();
							break;
						}
						}
						num2++;
					}
				}
				return objMultimCard;
			}
			return null;
		}

		public static int SetMultimCardInfo(DeviceServerBll devServerBll)
		{
			int result = DeviceHelper.m_errorCode;
			if (devServerBll != null)
			{
				Dictionary<int, AccDoor> doors = DeviceHelper.GetDoors();
				result = 0;
				AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
				List<ObjMultimCard> list = new List<ObjMultimCard>();
				List<AccMorecardset> modelList = accMorecardsetBll.GetModelList("door_id in (select id from acc_door where device_id=" + devServerBll.DevInfo.ID + ") ");
				if (modelList != null && modelList.Count > 0)
				{
					AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
					for (int i = 0; i < modelList.Count; i++)
					{
						List<AccMorecardGroup> modelList2 = accMorecardGroupBll.GetModelList("comb_id=" + modelList[i].id);
						if (modelList2 != null && modelList2.Count > 0)
						{
							ObjMultimCard objMultimCard = new ObjMultimCard();
							ObjMultimCard objMultimCard2 = objMultimCard;
							int num = doors[modelList[i].door_id].door_no;
							objMultimCard2.DoorId = num.ToString();
							ObjMultimCard objMultimCard3 = objMultimCard;
							num = modelList[i].id;
							objMultimCard3.Index = num.ToString();
							int num2 = 0;
							for (int j = 0; j < modelList2.Count; j++)
							{
								for (int k = 0; k < modelList2[j].opener_number; k++)
								{
									switch (num2)
									{
									case 0:
									{
										ObjMultimCard objMultimCard8 = objMultimCard;
										num = modelList2[j].group_id;
										objMultimCard8.Group1 = num.ToString();
										break;
									}
									case 1:
									{
										ObjMultimCard objMultimCard7 = objMultimCard;
										num = modelList2[j].group_id;
										objMultimCard7.Group2 = num.ToString();
										break;
									}
									case 2:
									{
										ObjMultimCard objMultimCard6 = objMultimCard;
										num = modelList2[j].group_id;
										objMultimCard6.Group3 = num.ToString();
										break;
									}
									case 3:
									{
										ObjMultimCard objMultimCard5 = objMultimCard;
										num = modelList2[j].group_id;
										objMultimCard5.Group4 = num.ToString();
										break;
									}
									case 4:
									{
										ObjMultimCard objMultimCard4 = objMultimCard;
										num = modelList2[j].group_id;
										objMultimCard4.Group5 = num.ToString();
										break;
									}
									}
									num2++;
								}
							}
							list.Add(objMultimCard);
						}
					}
					MultimCardBll multimCardBll = new MultimCardBll(devServerBll.Application);
					if (list.Count > 0)
					{
						result = multimCardBll.Add(list);
					}
				}
			}
			return result;
		}

		public static int GetDeviceParams(Machines model)
		{
			try
			{
				int num = DeviceHelper.m_errorCode;
				DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
				if (deviceServer != null)
				{
					if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
					{
						num = deviceServer.STD_GetDeviceParam(model);
					}
					else
					{
						DeviceParamBll deviceParamBll = new DeviceParamBll(deviceServer.Application);
						ObjDeviceParam objDeviceParam = new ObjDeviceParam();
						num = deviceParamBll.GetBaseParam(objDeviceParam);
						if (num >= 0)
						{
							if (!string.IsNullOrEmpty(objDeviceParam.NetMask) && !RegexServer.IsIPAddress(objDeviceParam.NetMask))
							{
								if (objDeviceParam.NetMask.IndexOf(" ") > 0)
								{
									objDeviceParam.NetMask = objDeviceParam.NetMask.Substring(0, objDeviceParam.NetMask.IndexOf(" "));
								}
								if (!RegexServer.IsIPAddress(objDeviceParam.NetMask))
								{
									objDeviceParam.NetMask = string.Empty;
								}
							}
							if (!string.IsNullOrEmpty(objDeviceParam.GATEIPAddress) && !RegexServer.IsIPAddress(objDeviceParam.GATEIPAddress))
							{
								if (objDeviceParam.GATEIPAddress.IndexOf(" ") > 0)
								{
									objDeviceParam.GATEIPAddress = objDeviceParam.GATEIPAddress.Substring(0, objDeviceParam.GATEIPAddress.IndexOf(" "));
								}
								if (!RegexServer.IsIPAddress(objDeviceParam.GATEIPAddress))
								{
									objDeviceParam.GATEIPAddress = string.Empty;
								}
							}
							model.acpanel_type = objDeviceParam.LockCount;
							if (model.acpanel_type < 1)
							{
								return -1008;
							}
							if (objDeviceParam.Door4ToDoor2 == 1)
							{
								model.four_to_two = true;
							}
							else
							{
								model.four_to_two = false;
							}
							model.FpVersion = int.Parse(string.IsNullOrEmpty(objDeviceParam.ZKFPVersion) ? "9" : objDeviceParam.ZKFPVersion);
							if (objDeviceParam.IsOnlyRFMachine == 1)
							{
								model.max_finger_count = 0;
								model.FpVersion = 0;
								model.fp_mthreshold = 0;
							}
							else
							{
								model.max_finger_count = objDeviceParam.MaxUserFingerCount;
								if (!string.IsNullOrEmpty(objDeviceParam.ZKFPVersion))
								{
									try
									{
										model.FpVersion = int.Parse(objDeviceParam.ZKFPVersion);
									}
									catch
									{
									}
								}
								if (!string.IsNullOrEmpty(objDeviceParam.MThreshold))
								{
									model.fp_mthreshold = int.Parse(objDeviceParam.MThreshold);
								}
							}
							model.IsOnlyRFMachine = objDeviceParam.IsOnlyRFMachine;
							model.sn = objDeviceParam.SerialNumber;
							model.FirmwareVersion = objDeviceParam.FirmVer;
							model.MachineNumber = objDeviceParam.DeviceID;
							if (!string.IsNullOrEmpty(objDeviceParam.DeviceName))
							{
								model.device_name = objDeviceParam.DeviceName;
							}
							model.subnet_mask = objDeviceParam.NetMask;
							model.gateway = objDeviceParam.GATEIPAddress;
							model.door_count = objDeviceParam.LockCount;
							model.reader_count = objDeviceParam.ReaderCount;
							model.aux_in_count = objDeviceParam.AuxInCount;
							model.aux_out_count = objDeviceParam.AuxOutCount;
							if (!string.IsNullOrEmpty(objDeviceParam.RS232BaudRate))
							{
								model.Baudrate = int.Parse(objDeviceParam.RS232BaudRate);
							}
							if (!string.IsNullOrEmpty(objDeviceParam.MachineType))
							{
								model.device_type = int.Parse(objDeviceParam.MachineType);
							}
							else
							{
								model.device_type = 1;
							}
							model.simpleEventType = objDeviceParam.SimpleEventType;
							model.FvFunOn = objDeviceParam.FvFunOn;
							model.RFCardOn = objDeviceParam.RFCardOn;
							model.FaceFunOn = objDeviceParam.FaceFunOn;
							model.FingerFunOn = objDeviceParam.FingerFunOn;
							model.CompatOldFirmware = objDeviceParam.CompatOldFirmware;
							string empty = string.Empty;
							string str = empty;
							int num2 = objDeviceParam.PC485AsInbio485;
							empty = str + "PC485AsInbio485=" + num2.ToString();
							string str2 = empty;
							num2 = objDeviceParam.MasterInbio485;
							empty = str2 + ",MasterInbio485=" + num2.ToString();
							empty = empty + ",RS232BaudRate=" + objDeviceParam.RS232BaudRate.ToString();
							string str3 = empty;
							num2 = objDeviceParam.CardFormatFunOn;
							empty = str3 + ",~CardFormatFunOn=" + num2.ToString();
							byte[] array = model.deviceOption = Encoding.ASCII.GetBytes(empty);
							model.Ext485ReaderFunOn = objDeviceParam.Ext485ReaderFunOn;
						}
					}
				}
				return num;
			}
			catch (Exception)
			{
				return DeviceHelper.m_errorCode;
			}
		}

		public static int SetDoor4ToDoor2(Machines model, bool isDoor4ToDoor2)
		{
			int num = DeviceHelper.m_errorCode;
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
			if (deviceServer != null)
			{
				num = 0;
				if (isDoor4ToDoor2 && model.acpanel_type == 4)
				{
					DeviceParamBll deviceParamBll = new DeviceParamBll(deviceServer.Application);
					num = deviceParamBll.SetDoor4ToDoor2(true);
					if (num >= 0)
					{
						model.acpanel_type = 2;
						model.door_count = 2;
						model.four_to_two = true;
					}
					deviceServer.RebootDevice();
					Thread.Sleep(20000);
					deviceServer.Disconnect();
					deviceServer.Connect(3000);
				}
				else if (model.four_to_two && model.acpanel_type == 2 && !isDoor4ToDoor2)
				{
					DeviceParamBll deviceParamBll2 = new DeviceParamBll(deviceServer.Application);
					num = deviceParamBll2.SetDoor4ToDoor2(false);
					if (num >= 0)
					{
						model.acpanel_type = 4;
						model.door_count = 4;
						model.four_to_two = false;
					}
					deviceServer.RebootDevice();
					Thread.Sleep(20000);
					deviceServer.Disconnect();
					deviceServer.Connect(3000);
				}
			}
			return num;
		}

		public static string SaveDoorInfo(Machines machinesModel, bool isResetDoorInfo)
		{
			AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
			List<AccWiegandfmt> modelList = accWiegandfmtBll.GetModelList("");
			modelList = (modelList ?? new List<AccWiegandfmt>());
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			for (int i = 0; i < modelList.Count; i++)
			{
				if (!dictionary.ContainsKey(modelList[i].wiegand_name.ToUpper()))
				{
					dictionary.Add(modelList[i].wiegand_name.ToUpper(), modelList[i].id);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			int errorCode = DeviceHelper.m_errorCode;
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machinesModel);
			if (deviceServer != null)
			{
				errorCode = 0;
				DeviceParamBll deviceParamBll = new DeviceParamBll(deviceServer.Application);
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				AccTimeseg accTimeseg = accTimesegBll.GetdefaultTime();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				int acpanel_type = machinesModel.acpanel_type;
				int iD = machinesModel.ID;
				for (int j = 0; j < acpanel_type; j++)
				{
					AccDoor accDoor = new AccDoor();
					accDoor.is_att = true;
					accDoor.device_id = iD;
					accDoor.door_name = machinesModel.MachineAlias + "-" + (j + 1).ToString();
					accDoor.door_no = j + 1;
					accDoor.long_open_id = 0;
					accDoor.force_pwd = string.Empty;
					accDoor.supper_pwd = string.Empty;
					ObjDoorInfo objDoorInfo = DeviceHelper.CreateDoor(accDoor.door_no);
					if (dictionary.ContainsKey("Wiegand26".ToUpper()))
					{
						accDoor.wiegand_fmt_id = dictionary["Wiegand26".ToUpper()];
					}
					int device_type = machinesModel.device_type;
					if ((uint)(device_type - 1) <= 9u && dictionary.ContainsKey("AutoMatchWiegandFmt".ToUpper()))
					{
						accDoor.wiegand_fmt_id = dictionary["AutoMatchWiegandFmt".ToUpper()];
					}
					errorCode = deviceParamBll.GetDoorInfo(objDoorInfo);
					if (errorCode >= 0)
					{
						accDoor.opendoor_type = objDoorInfo.DoorVerifyType;
						accDoor.card_intervaltime = objDoorInfo.DoorIntertime;
					}
					else if (machinesModel.device_type == 102 && machinesModel.FvFunOn == 1)
					{
						accDoor.opendoor_type = 0;
					}
					else if (machinesModel.device_type == 103 || machinesModel.device_type == 1000)
					{
						accDoor.opendoor_type = 0;
						accDoor.card_intervaltime = 0;
					}
					else if (machinesModel.IsOnlyRFMachine == 0 || machinesModel.device_type == 12 || machinesModel.device_type == 101)
					{
						accDoor.opendoor_type = 6;
						accDoor.card_intervaltime = 0;
					}
					else
					{
						accDoor.opendoor_type = 4;
					}
					accDoor.lock_active_id = accTimeseg.id;
					if (isResetDoorInfo)
					{
						accDoor.door_sensor_status = 0;
						accDoor.lock_delay = 5;
						accDoor.card_intervaltime = ((machinesModel.device_type != 101) ? 3 : 0);
						accDoor.sensor_delay = 15;
						objDoorInfo.DoorSensorType = accDoor.door_sensor_status;
						objDoorInfo.DoorVerifyType = accDoor.opendoor_type;
						objDoorInfo.DoorIntertime = accDoor.card_intervaltime;
						objDoorInfo.DoorDrivertime = 5;
						objDoorInfo.DoorFirstCardOpenDoor = 0;
						objDoorInfo.DoorMultiCardOpenDoor = 0;
						objDoorInfo.DoorKeepOpenTimeZone = 0;
						objDoorInfo.DoorValidTZ = 1;
						objDoorInfo.DoorForcePassWord = string.Empty;
						objDoorInfo.DoorSupperPassWord = string.Empty;
						objDoorInfo.DoorDetectortime = 15;
						if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
						{
							objDoorInfo.ManualCtlMode = accDoor.ManualCtlMode;
						}
						errorCode = deviceParamBll.SetDoorParam(objDoorInfo, machinesModel);
						if (errorCode >= 0)
						{
							stringBuilder.Append(ShowMsgInfos.GetInfo("SetDoorInfoOk", "从PC同步门设置信息到设备成功") + ":" + accDoor.door_name + "\r\n");
						}
						else
						{
							stringBuilder.Append(ShowMsgInfos.GetInfo("SetDoorInfoFalse", "从PC同步门设置信息到设备失败") + "-" + accDoor.door_name + ":" + PullSDkErrorInfos.GetInfo(errorCode) + "\r\n");
						}
					}
					accDoorBll.Add(accDoor);
				}
				if (isResetDoorInfo)
				{
					errorCode = DeviceHelper.DeleteTimeZone(deviceServer);
					if (errorCode >= 0)
					{
						errorCode = DeviceHelper.SetTZInfo(deviceServer);
						if (errorCode >= 0)
						{
							stringBuilder.Append(ShowMsgInfos.GetInfo("SetTimeZoneOk", "从PC同步门禁时间段信息到设备成功") + "\r\n");
						}
						else
						{
							stringBuilder.Append(ShowMsgInfos.GetInfo("SetTimeZoneFalse", "从PC同步门禁时间段信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(errorCode) + "\r\n");
						}
					}
					else
					{
						stringBuilder.Append(ShowMsgInfos.GetInfo("DelTimeZoneFalse", "门禁时间段重置失败") + ":" + PullSDkErrorInfos.GetInfo(errorCode) + "\r\n");
					}
					errorCode = DeviceHelper.DeleteHoliday(deviceServer);
					if (errorCode >= 0)
					{
						errorCode = DeviceHelper.SetHolidayInfo(deviceServer);
						if (errorCode >= 0)
						{
							stringBuilder.Append(ShowMsgInfos.GetInfo("SetHolidayOk", "从PC同步节假日信息到设备成功") + "\r\n");
						}
						else
						{
							stringBuilder.Append(ShowMsgInfos.GetInfo("SetHolidayFalse", "从PC同步节假日信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(errorCode) + "\r\n");
						}
					}
					else
					{
						stringBuilder.Append(ShowMsgInfos.GetInfo("DelHolidayFalse", "节假日重置失败") + ":" + PullSDkErrorInfos.GetInfo(errorCode) + "\r\n");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static int GetUserInfo(DeviceServerBll devServerBll, List<UserInfo> userList)
		{
			int result = DeviceHelper.m_errorCode;
			if (userList != null)
			{
				result = 0;
				userList.Clear();
				if (devServerBll != null)
				{
					UserBll userBll = new UserBll(devServerBll.Application);
					List<ObjUser> list = userBll.GetList(ref result);
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							ObjUser objUser = list[i];
							UserInfo userInfo = new UserInfo();
							userInfo.BadgeNumber = objUser.Pin;
							if (objUser.Name != null)
							{
								userInfo.Name = objUser.Name;
							}
							else
							{
								userInfo.Name = objUser.Pin;
							}
							if (!string.IsNullOrEmpty(objUser.CardNo) && objUser.CardNo != "0")
							{
								if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
								{
									if (ulong.TryParse(objUser.CardNo, out ulong num))
									{
										userInfo.CardNo = num.ToString("X");
									}
								}
								else
								{
									userInfo.CardNo = objUser.CardNo;
								}
							}
							if (!string.IsNullOrEmpty(objUser.Group))
							{
								userInfo.AccGroup = int.Parse(objUser.Group);
								userInfo.MorecardGroupId = int.Parse(objUser.Group);
							}
							userInfo.Privilege = objUser.Privilege;
							if (AccCommon.IsECardTong == 1)
							{
								userInfo.MVerifyPass = objUser.Password;
							}
							else
							{
								userInfo.PassWord = objUser.Password;
							}
							try
							{
								if (!string.IsNullOrEmpty(objUser.StartTime) && objUser.StartTime.Length == 8)
								{
									userInfo.AccStartDate = DateTime.Parse(objUser.StartTime.Substring(0, 4) + "-" + objUser.StartTime.Substring(4, 2) + "-" + objUser.StartTime.Substring(6, 2));
								}
								if (!string.IsNullOrEmpty(objUser.EndTime) && objUser.EndTime.Length == 8)
								{
									userInfo.AccEndDate = DateTime.Parse(objUser.EndTime.Substring(0, 4) + "-" + objUser.EndTime.Substring(4, 2) + "-" + objUser.EndTime.Substring(6, 2));
								}
							}
							catch
							{
							}
							userList.Add(userInfo);
						}
					}
				}
			}
			return result;
		}

		public static int GetUserTemplate(DeviceServerBll devServerBll, List<Template> tempList, List<UserInfo> userList)
		{
			int result = DeviceHelper.m_errorCode;
			if (tempList != null)
			{
				tempList.Clear();
				if (devServerBll != null)
				{
					result = 0;
					TemplateV10Bll templateV10Bll = new TemplateV10Bll(devServerBll.Application);
					List<ObjTemplateV10> list = null;
					if (userList == null || userList.Count == 0)
					{
						list = templateV10Bll.GetList(ref result);
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < userList.Count; i++)
						{
							stringBuilder.Append("Pin=" + userList[i].BadgeNumber + ",");
						}
						list = templateV10Bll.GetList(stringBuilder.ToString(), "", ref result);
					}
					if (list != null && list.Count > 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							ObjTemplateV10 objTemplateV = list[j];
							if (!string.IsNullOrEmpty(objTemplateV.Template))
							{
								Template template = new Template();
								template.FINGERID = (int)objTemplateV.FingerID;
								template.Flag = (short)objTemplateV.Valid;
								template.TempPlate = Convert.FromBase64String(objTemplateV.Template);
								if (devServerBll.DevInfo.FPVersion == 10)
								{
									template.TEMPLATE4 = template.TempPlate;
								}
								else
								{
									template.TEMPLATE3 = template.TempPlate;
								}
								template.Pin = objTemplateV.Pin;
								tempList.Add(template);
							}
						}
					}
				}
			}
			return result;
		}

		public static int GetUserFingerVein(DeviceServerBll devServerBll, List<FingerVein> tempList, List<UserInfo> userList)
		{
			int result = DeviceHelper.m_errorCode;
			if (tempList != null)
			{
				tempList.Clear();
				if (devServerBll != null)
				{
					result = 0;
					FVTemplateBll fVTemplateBll = new FVTemplateBll(devServerBll.Application);
					List<ObjFVTemplate> list = null;
					if (userList == null || userList.Count == 0)
					{
						list = fVTemplateBll.GetList(ref result);
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < userList.Count; i++)
						{
							stringBuilder.Append("Pin=" + userList[i].BadgeNumber + ",");
						}
						list = fVTemplateBll.GetList(stringBuilder.ToString(), "", ref result);
					}
					if (list != null && list.Count > 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							ObjFVTemplate objFVTemplate = list[j];
							if (!string.IsNullOrEmpty(objFVTemplate.Fv) && !string.IsNullOrEmpty(objFVTemplate.Pin2))
							{
								FingerVein fingerVein = new FingerVein();
								fingerVein.FingerID = objFVTemplate.FingerID;
								fingerVein.DuressFlag = objFVTemplate.Duress;
								fingerVein.Template = Convert.FromBase64String(objFVTemplate.Fv);
								string pin = objFVTemplate.Pin2;
								fingerVein.Pin = pin.Trim();
								fingerVein.UserCode = objFVTemplate.UserCode;
								fingerVein.Size = objFVTemplate.Size;
								fingerVein.Fv_ID_Index = objFVTemplate.Fv_ID_Index;
								tempList.Add(fingerVein);
							}
						}
					}
				}
			}
			return result;
		}

		public static int GetTimeInfo(DeviceServerBll devServerBll, List<AccTimeseg> timeList)
		{
			int result = DeviceHelper.m_errorCode;
			if (timeList != null)
			{
				timeList.Clear();
				if (devServerBll != null)
				{
					result = 0;
					TimeZoneBll timeZoneBll = new TimeZoneBll(devServerBll.Application);
					List<ObjTimeZone> list = timeZoneBll.GetList(ref result);
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							ObjTimeZone objTimeZone = list[i];
							DateTime now = DateTime.Now;
							DateTime now2 = DateTime.Now;
							AccTimeseg accTimeseg = new AccTimeseg();
							DeviceHelper.GetDateTime(objTimeZone.MonTime1, ref now, ref now2);
							accTimeseg.monday_start1 = now;
							accTimeseg.monday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.MonTime2, ref now, ref now2);
							accTimeseg.monday_start2 = now;
							accTimeseg.monday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.MonTime3, ref now, ref now2);
							accTimeseg.monday_start3 = now;
							accTimeseg.monday_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.FriTime1, ref now, ref now2);
							accTimeseg.friday_start1 = now;
							accTimeseg.friday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.FriTime2, ref now, ref now2);
							accTimeseg.friday_start2 = now;
							accTimeseg.friday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.FriTime3, ref now, ref now2);
							accTimeseg.friday_start3 = now;
							accTimeseg.friday_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol1Time1, ref now, ref now2);
							accTimeseg.holidaytype1_start1 = now;
							accTimeseg.holidaytype1_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol1Time2, ref now, ref now2);
							accTimeseg.holidaytype1_start2 = now;
							accTimeseg.holidaytype1_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol1Time3, ref now, ref now2);
							accTimeseg.holidaytype1_start3 = now;
							accTimeseg.holidaytype1_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol2Time1, ref now, ref now2);
							accTimeseg.holidaytype2_start1 = now;
							accTimeseg.holidaytype2_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol2Time2, ref now, ref now2);
							accTimeseg.holidaytype2_start2 = now;
							accTimeseg.holidaytype2_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol2Time3, ref now, ref now2);
							accTimeseg.holidaytype2_start3 = now;
							accTimeseg.holidaytype2_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol3Time1, ref now, ref now2);
							accTimeseg.holidaytype3_start1 = now;
							accTimeseg.holidaytype3_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol3Time2, ref now, ref now2);
							accTimeseg.holidaytype3_start2 = now;
							accTimeseg.holidaytype3_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.Hol3Time3, ref now, ref now2);
							accTimeseg.holidaytype3_start3 = now;
							accTimeseg.holidaytype3_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.SatTime1, ref now, ref now2);
							accTimeseg.saturday_start1 = now;
							accTimeseg.saturday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.SatTime2, ref now, ref now2);
							accTimeseg.saturday_start2 = now;
							accTimeseg.saturday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.SatTime3, ref now, ref now2);
							accTimeseg.saturday_start3 = now;
							accTimeseg.saturday_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.SunTime1, ref now, ref now2);
							accTimeseg.sunday_start1 = now;
							accTimeseg.sunday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.SunTime2, ref now, ref now2);
							accTimeseg.sunday_start2 = now;
							accTimeseg.sunday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.SunTime3, ref now, ref now2);
							accTimeseg.sunday_start3 = now;
							accTimeseg.sunday_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.ThuTime1, ref now, ref now2);
							accTimeseg.thursday_start1 = now;
							accTimeseg.thursday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.ThuTime2, ref now, ref now2);
							accTimeseg.thursday_start2 = now;
							accTimeseg.thursday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.ThuTime3, ref now, ref now2);
							accTimeseg.thursday_start3 = now;
							accTimeseg.thursday_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.TueTime1, ref now, ref now2);
							accTimeseg.tuesday_start1 = now;
							accTimeseg.tuesday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.TueTime2, ref now, ref now2);
							accTimeseg.tuesday_start2 = now;
							accTimeseg.tuesday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.TueTime3, ref now, ref now2);
							accTimeseg.tuesday_start3 = now;
							accTimeseg.tuesday_end3 = now2;
							DeviceHelper.GetDateTime(objTimeZone.WedTime1, ref now, ref now2);
							accTimeseg.wednesday_start1 = now;
							accTimeseg.wednesday_end1 = now2;
							DeviceHelper.GetDateTime(objTimeZone.WedTime2, ref now, ref now2);
							accTimeseg.wednesday_start2 = now;
							accTimeseg.wednesday_end2 = now2;
							DeviceHelper.GetDateTime(objTimeZone.WedTime3, ref now, ref now2);
							accTimeseg.wednesday_start3 = now;
							accTimeseg.wednesday_end3 = now2;
							if (!string.IsNullOrEmpty(objTimeZone.TimezoneId))
							{
								accTimeseg.id = int.Parse(objTimeZone.TimezoneId);
							}
							if (1 == accTimeseg.id)
							{
								accTimeseg.id = DeviceHelper.TimeSeg.id;
							}
							timeList.Add(accTimeseg);
						}
					}
				}
			}
			return result;
		}

		private static void GetDateTime(string number, ref DateTime start, ref DateTime end)
		{
			if (!string.IsNullOrEmpty(number))
			{
				try
				{
					int number2 = int.Parse(number);
					DeviceHelper.GetDateTime(number2, ref start, ref end);
				}
				catch
				{
					DeviceHelper.GetDateTime(0, ref start, ref end);
				}
			}
			else
			{
				DeviceHelper.GetDateTime(0, ref start, ref end);
			}
		}

		private static void GetDateTime(int number, ref DateTime start, ref DateTime end)
		{
			if (number > 0)
			{
				int num = number >> 16;
				int num2 = number - (num << 16);
				int minite = num / 100 * 60 + num % 100;
				start = AccTimesegBll.GetDate(minite);
				minite = num2 / 100 * 60 + num2 % 100;
				end = AccTimesegBll.GetDate(minite);
			}
			else
			{
				start = AccTimesegBll.GetDate(0);
				end = AccTimesegBll.GetDate(0);
			}
		}

		public static int GetFaceTemplate(DeviceServerBll devServerBll, List<FaceTemp> tempList, List<UserInfo> userList)
		{
			int result = DeviceHelper.m_errorCode;
			if (tempList != null)
			{
				tempList.Clear();
				if (devServerBll != null)
				{
					result = 0;
					ZK.Data.BLL.PullSDK.FaceTempBll faceTempBll = new ZK.Data.BLL.PullSDK.FaceTempBll(devServerBll.Application);
					List<ObjFaceTemp> list = null;
					if (userList == null || userList.Count == 0)
					{
						list = faceTempBll.GetList(ref result);
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = 0; i < userList.Count; i++)
						{
							stringBuilder.Append("Pin=" + userList[i].BadgeNumber + ",");
						}
						list = faceTempBll.GetList(stringBuilder.ToString(), "", ref result);
					}
					if (list != null && list.Count > 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							ObjFaceTemp objFaceTemp = list[j];
							if (!string.IsNullOrEmpty(objFaceTemp.Face))
							{
								FaceTemp faceTemp = new FaceTemp();
								faceTemp.ActiveTime = objFaceTemp.ActiveTime;
								faceTemp.FaceId = objFaceTemp.FaceID;
								faceTemp.Pin = objFaceTemp.Pin;
								faceTemp.Reserve = objFaceTemp.Reserved;
								faceTemp.Size = objFaceTemp.Size;
								faceTemp.Template = Convert.FromBase64String(objFaceTemp.Face);
								faceTemp.Valid = objFaceTemp.Valid;
								faceTemp.VFCOUNT = objFaceTemp.VfCount;
								faceTemp.FaceType = 0;
								tempList.Add(faceTemp);
							}
						}
					}
				}
			}
			return result;
		}

		public static int TestConnect(Machines machine)
		{
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machine);
			if (deviceServer.IsConnected)
			{
				deviceServer.Disconnect();
			}
			return deviceServer.Connect(3000);
		}

		public static bool UpLoadDataToDevice(Machines model, DeviceServerBll devServerBll, ShowInfoHandler ShowUpLoadInfo, ShowProgressHandler ShowProgress, Form owner)
		{
			int num = 0;
			ObjDevice devInfo = devServerBll.DevInfo;
			AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
			List<AccLevelset> modelList = accLevelsetBll.GetModelList("id in (Select acclevelset_id from acc_levelset_door_group where accdoor_id in (select id from acc_door where device_id=" + model.ID + "))");
			Dictionary<int, AccLevelset> dictionary = new Dictionary<int, AccLevelset>();
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					dictionary.Add(modelList[i].id, modelList[i]);
				}
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList2 = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group where accdoor_device_id=" + model.ID + ")");
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList3 = accTimesegBll.GetModelList("TimeZone1Id>0 or TimeZone2Id>0 or TimeZone3Id>0 or TimeZoneHolidayId>0");
				Dictionary<int, AccTimeseg> dictionary2 = new Dictionary<int, AccTimeseg>();
				if (modelList3 != null)
				{
					for (int j = 0; j < modelList3.Count; j++)
					{
						if (!dictionary2.ContainsKey(modelList3[j].id))
						{
							dictionary2.Add(modelList3[j].id, modelList3[j]);
						}
					}
				}
				AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
				List<AccLevelsetEmp> modelList4 = accLevelsetEmpBll.GetModelList("acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + model.ID + ")");
				Dictionary<int, AccTimeseg> dictionary3 = new Dictionary<int, AccTimeseg>();
				for (int k = 0; k < modelList4.Count; k++)
				{
					AccLevelsetEmp accLevelsetEmp = modelList4[k];
					if (!dictionary3.ContainsKey(accLevelsetEmp.employee_id))
					{
						if (dictionary.ContainsKey(accLevelsetEmp.acclevelset_id))
						{
							dictionary3.Add(accLevelsetEmp.employee_id, dictionary2[dictionary[accLevelsetEmp.acclevelset_id].level_timeseg_id]);
						}
					}
					else if (dictionary.ContainsKey(accLevelsetEmp.acclevelset_id))
					{
						dictionary3[accLevelsetEmp.employee_id] = dictionary2[dictionary[accLevelsetEmp.acclevelset_id].level_timeseg_id];
					}
				}
				num = devServerBll.STD_ClearUser();
				if (num < 0)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteUserInfoFailed", "重置人员信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					return false;
				}
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteUserInfoSucceed", "重置人员信息成功"));
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingTimeZone", "正在设置时间段"));
					num = devServerBll.STD_SetTimeZone(modelList3, DeviceHelper.TimeSeg.id);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneFailed", "设置时间段失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				catch (Exception ex)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneFailed", "设置时间段失败") + ": " + ex.Message);
					return false;
				}
				ShowProgress(1);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingHoliday", "正在设置节假日"));
					AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
					List<AccHolidays> modelList5 = accHolidaysBll.GetModelList("");
					num = devServerBll.STD_SetHoliday(modelList5);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayFailed", "设置节假日失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				catch (Exception ex2)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayFailed", "设置节假日失败") + ": " + ex2.Message);
					return false;
				}
				ShowProgress(2);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingDeviceParam", "正在设置设备参数"));
					num = devServerBll.STD_SetDeviceParam(model);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDeviceParamFailed", "设置设备参数失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				catch (Exception ex3)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDeviceParamFailed", "设置设备参数失败") + ": " + ex3.Message);
					return false;
				}
				ShowProgress(5);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingAntiback", "正在设置反潜规则"));
					AccAntibackBll accAntibackBll = new AccAntibackBll(MainForm._ia);
					List<AccAntiback> modelList6 = accAntibackBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
					num = devServerBll.STD_SetAntiback(modelList6);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackFailed", "设置反潜规则失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				catch (Exception ex4)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackFailed", "设置反潜规则失败") + ": " + ex4.Message);
					return false;
				}
				ShowProgress(7);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingDoorParam", "正在设置门参数"));
					if (modelList2 != null && modelList2.Count > 0)
					{
						for (int l = 0; l < modelList2.Count; l++)
						{
							AccDoor accDoor = modelList2[l];
							if (dictionary2.ContainsKey(accDoor.lock_active_id))
							{
								AccTimeseg accTimeseg = dictionary2[accDoor.lock_active_id];
								accDoor.GroupTZ1 = accTimeseg.TimeZone1Id;
								accDoor.GroupTZ2 = accTimeseg.TimeZone2Id;
								accDoor.GroupTZ3 = accTimeseg.TimeZone3Id;
							}
							if (dictionary2.ContainsKey(accDoor.long_open_id))
							{
								AccTimeseg accTimeseg = dictionary2[accDoor.long_open_id];
								accDoor.NormalOpenTZ = accTimeseg.TimeZone1Id;
							}
							num = devServerBll.STD_SetDoorParam(accDoor, DeviceHelper.TimeSeg.id);
							if (num < 0)
							{
								break;
							}
						}
						if (num < 0)
						{
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDoorParamFailed", "设置门参数失败") + ": " + PullSDkErrorInfos.GetInfo(num));
							return false;
						}
					}
				}
				catch (Exception ex5)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDoorParamFailed", "设置门参数失败") + ": " + ex5.Message);
					return false;
				}
				ShowProgress(9);
				Dictionary<int, int> dictionary4;
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingGroup", "正在设置人员组"));
					AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
					List<AccMorecardempGroup> modelList7 = accMorecardempGroupBll.GetModelList("");
					modelList7 = (modelList7 ?? new List<AccMorecardempGroup>());
					List<ZK.Data.Model.StdSDK.Group> lstGroup = CommandServer.ConvertGroup(model, modelList7);
					dictionary4 = new Dictionary<int, int>();
					for (int m = 0; m < modelList7.Count; m++)
					{
						if (!dictionary4.ContainsKey(modelList7[m].id))
						{
							dictionary4.Add(modelList7[m].id, (modelList7[m].StdGroupNo <= 0) ? 1 : modelList7[m].StdGroupNo);
						}
					}
					num = devServerBll.STD_SetGroup(lstGroup);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetGroupFailed", "设置人员组失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				catch (Exception ex6)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetGroupFailed", "设置人员组失败") + ": " + ex6.Message);
					return false;
				}
				ShowProgress(10);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingWiegandFmt", "正在设置韦根格式"));
					STD_WiegandFmtBll sTD_WiegandFmtBll = new STD_WiegandFmtBll(MainForm._ia);
					List<STD_WiegandFmt> modelList8 = sTD_WiegandFmtBll.GetModelList("DoorId in (Select id from acc_door where device_id in(Select id from Machines where id=" + model.ID + "))");
					if (modelList8 != null && modelList8.Count > 0)
					{
						for (int n = 0; n < modelList8.Count; n++)
						{
							num = devServerBll.STD_SetWiegandFmt(modelList8[n]);
							if (num < 0)
							{
								ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetWiegandFmtFailed", "设置韦根格式失败") + ": " + PullSDkErrorInfos.GetInfo(num));
								return false;
							}
						}
					}
				}
				catch (Exception ex7)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetWiegandFmtFailed", "设置韦根格式失败") + ": " + ex7.Message);
					return false;
				}
				ShowProgress(12);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingOpenDoorCombination", "正在同步开门组合到设备"));
					List<ObjMultimCard> allUnlockGroup = CommandServer.GetAllUnlockGroup(model);
					num = devServerBll.STD_SetUnlockGroup(allUnlockGroup);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SyncOpenDoorCombinationSucceed", "从PC同步开门组合到设备成功") + ", " + ShowMsgInfos.GetInfo("OpenDoorCombinationCount", "组合数量") + ": " + num);
						goto end_IL_0936;
					}
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SyncOpenDoorCombinationFailed", "从PC同步开门组合到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
					return false;
					end_IL_0936:;
				}
				catch (Exception ex8)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SyncOpenDoorCombinationFailed", "从PC同步开门组合到设备失败") + ": " + ex8.Message);
					return false;
				}
				ShowProgress(30);
				List<UserInfo> list = null;
				Dictionary<int, UserInfo> dictionary5 = null;
				List<UserInfo> list2 = new List<UserInfo>();
				List<UserVerifyType> list4;
				int num5;
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingUser", "正在同步人员信息到设备"));
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					list = userInfoBll.GetModelList("USERID in (Select employee_id from acc_levelset_emp where acclevelset_id in (select acclevelset_id from acc_levelset where acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + model.ID + ")))");
					list = (list ?? new List<UserInfo>());
					List<UserInfo> list3 = new List<UserInfo>();
					int num2 = (devInfo.DevSDKType != SDKType.StandaloneSDK) ? 6 : (devInfo.IsTFTMachine ? 8 : 5);
					dictionary5 = new Dictionary<int, UserInfo>();
					Dictionary<int, int> dictionary6 = new Dictionary<int, int>();
					list4 = new List<UserVerifyType>();
					for (int num3 = 0; num3 < list.Count; num3++)
					{
						UserInfo userInfo = list[num3];
						if (!dictionary5.ContainsKey(userInfo.UserId))
						{
							dictionary5.Add(userInfo.UserId, userInfo);
						}
						UserVerifyType userVerifyType = new UserVerifyType();
						userVerifyType.Pin = int.Parse(userInfo.BadgeNumber);
						userVerifyType.VerifyType = 0;
						userVerifyType.UseGroupVT = true;
						if (dictionary4.ContainsKey(userInfo.MorecardGroupId))
						{
							userVerifyType.GroupNo = dictionary4[userInfo.MorecardGroupId];
						}
						else
						{
							userVerifyType.GroupNo = 1;
						}
						list4.Add(userVerifyType);
						if (userInfo.PassWord != null && userInfo.PassWord.Length > num2)
						{
							list3.Add(userInfo);
						}
					}
					MultiChoiceDialog multiChoiceDialog = new MultiChoiceDialog();
					List<DialogChoice> list5 = new List<DialogChoice>();
					DialogResult dialogResult = DialogResult.None;
					list5.Add(new DialogChoice(1, ShowMsgInfos.GetInfo("CutPwd", "截断密码"), ShowMsgInfos.GetInfo("CutPwdDesc", "忽略密码超长的部分，只上传匹配长度的密码")));
					list5.Add(new DialogChoice(2, ShowMsgInfos.GetInfo("ClearPwd", "忽略密码"), ShowMsgInfos.GetInfo("NotUploadPwd", "不上传密码")));
					list5.Add(new DialogChoice(3, ShowMsgInfos.GetInfo("Skip", "跳过"), ShowMsgInfos.GetInfo("DonotUpload", "不上传此用户")));
					for (int num4 = 0; num4 < list3.Count; num4++)
					{
						UserInfo userInfo = list3[num4];
						dialogResult = DialogResult.OK;
						if (dialogResult != DialogResult.OK)
						{
							ShowProgress(100);
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("UserCanceled", "用户取消操作"));
							return false;
						}
						int? selectedChoice = multiChoiceDialog.SelectedChoice;
						int? nullable = selectedChoice;
						if (nullable.HasValue)
						{
							num5 = nullable.GetValueOrDefault();
							switch (num5)
							{
							case 1:
								userInfo.PassWord = userInfo.PassWord.Substring(0, num2);
								break;
							case 2:
								userInfo.PassWord = "";
								break;
							case 3:
								list.Remove(userInfo);
								if (dictionary6.ContainsKey(int.Parse(userInfo.BadgeNumber)))
								{
									dictionary6.Remove(int.Parse(userInfo.BadgeNumber));
								}
								break;
							}
						}
					}
					num = devServerBll.STD_SetUserInfo(list, dictionary4);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoOk", "从PC同步人员信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ": " + num);
						goto end_IL_0a38;
					}
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoFalse", "从PC同步人员信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num) + (devInfo.BatchUpdate ? ("\t" + ShowMsgInfos.GetInfo("CancelBatchUpdateAndRetry", "可取消批量上传功能后重试")) : ""));
					return false;
					end_IL_0a38:;
				}
				catch (Exception ex9)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoFalse", "从PC同步人员信息到设备失败") + ": " + ex9.Message);
					return false;
				}
				ShowProgress(40);
				ShowProgress(50);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingUserVerifyMode", "正在同步人员验证方式到设备"));
					num = devServerBll.STD_SetUserVerifyMode(list4);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserVerifyMode", "从PC同步人员验证模式到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + num);
						goto end_IL_0e6b;
					}
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserVerifyModeFaild", "从PC同步人员验证模式到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + (devInfo.BatchUpdate ? ("\t" + ShowMsgInfos.GetInfo("CancelBatchUpdateAndRetry", "可取消批量上传功能后重试")) : ""));
					return false;
					end_IL_0e6b:;
				}
				catch (Exception ex10)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserVerifyModeSucceed", "同步人员验证模式到设备失败") + ": " + ex10.Message);
					return false;
				}
				ShowProgress(70);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingFingerPring", "正在同步指纹信息到设备"));
					TemplateBll templateBll = new TemplateBll(MainForm._ia);
					List<Template> modelList9 = templateBll.GetModelList("USERID in (select USERINFO.USERID from USERINFO where USERINFO.USERID in (Select employee_id from acc_levelset_emp where acclevelset_id in (select acclevelset_id from acc_levelset where acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + model.ID + "))))");
					modelList9 = (modelList9 ?? new List<Template>());
					for (int num6 = 0; num6 < modelList9.Count; num6++)
					{
						Template template = modelList9[num6];
						if (dictionary5.ContainsKey(template.USERID))
						{
							template.Pin = dictionary5[template.USERID].BadgeNumber;
						}
					}
					num = devServerBll.STD_SetUserFPTemplate(modelList9);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateOk", "从PC同步指纹信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ": " + list.Count + " " + ShowMsgInfos.GetInfo("templateCount", "指纹数") + ": " + num);
						modelList9 = null;
						goto end_IL_0f80;
					}
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num) + (devInfo.BatchUpdate ? ("\t" + ShowMsgInfos.GetInfo("CancelBatchUpdateAndRetry", "可取消批量上传功能后重试")) : ""));
					return false;
					end_IL_0f80:;
				}
				catch (Exception ex11)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ": " + ex11.Message);
					return false;
				}
				ShowProgress(80);
				if (devInfo.FaceFunOn == 1)
				{
					try
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingFaceTemplate", "正在同步面部信息到设备"));
						ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
						List<FaceTemp> modelList10 = faceTempBll.GetModelList(((AppSite.Instance.DataType == DataType.Access) ? "clng(USERNO) in (select USERINFO.USERID from USERINFO " : "USERNO in (select USERINFO.USERID from USERINFO ") + "where USERINFO.USERID in (Select employee_id from acc_levelset_emp where acclevelset_id in (select acclevelset_id from acc_levelset where acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + model.ID + "))))");
						modelList10 = (modelList10 ?? new List<FaceTemp>());
						num = devServerBll.STD_SetUserFaceTemplate(modelList10);
						if (num >= 0)
						{
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateOk", "从PC同步面部信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + list.Count + " " + ShowMsgInfos.GetInfo("FacetemplateCount", "面部模版数") + ":" + num);
							modelList10 = null;
							goto end_IL_1176;
						}
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateFalse", "从PC同步面部信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num) + (devInfo.BatchUpdate ? ("\t" + ShowMsgInfos.GetInfo("CancelBatchUpdateAndRetry", "可取消批量上传功能后重试")) : ""));
						return false;
						end_IL_1176:;
					}
					catch (Exception ex12)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateFalse", "从PC同步面部信息到设备失败") + ": " + ex12.Message);
						return false;
					}
				}
				ShowProgress(90);
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingUserTimeZone", "正在同步人员时间段到设备"));
					List<ObjUserAuthorize> list6 = new List<ObjUserAuthorize>();
					for (int num7 = 0; num7 < list.Count; num7++)
					{
						UserInfo userInfo = list[num7];
						ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
						objUserAuthorize.Pin = userInfo.BadgeNumber;
						int num8 = 0;
						for (int num9 = 0; num9 < modelList2.Count; num9++)
						{
							num8 |= 1 << modelList2[num9].door_no - 1;
						}
						objUserAuthorize.AuthorizeDoorId = num8.ToString();
						if (dictionary3.ContainsKey(userInfo.UserId))
						{
							AccTimeseg accTimeseg = dictionary3[userInfo.UserId];
							ObjUserAuthorize objUserAuthorize2 = objUserAuthorize;
							num5 = dictionary3[userInfo.UserId].id;
							objUserAuthorize2.AuthorizeTimezoneId = num5.ToString();
							objUserAuthorize.TimeZone1 = accTimeseg.TimeZone1Id;
							objUserAuthorize.TimeZone2 = accTimeseg.TimeZone2Id;
							objUserAuthorize.TimeZone3 = accTimeseg.TimeZone3Id;
						}
						list6.Add(objUserAuthorize);
					}
					num = devServerBll.STD_SetUserTimeZone(list6);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserTimeZoneOk", "设置人员时间段成功"));
						goto end_IL_132a;
					}
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserTimeZoneFalse", "设置人员时间段失败") + ": " + PullSDkErrorInfos.GetInfo(num));
					return false;
					end_IL_132a:;
				}
				catch (Exception ex13)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserTimeZoneFalse", "设置人员时间段失败") + ": " + ex13.Message);
					return false;
				}
				ShowProgress(100);
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoAndLevelsetFinish", "从PC同步人员及权限信息到设备完成"));
				DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
				devCmdsBll.UpdateStatus(2, "status=0 and SN_id=" + model.ID);
				return true;
			}
			ShowUpLoadInfo(ShowMsgInfos.GetInfo("MachineNotInLevelSet", "设备未加入权限组"));
			return false;
		}

		public static bool UpLoadDataToDeviceEx(Machines model, DeviceServerBll devServerBll, ShowInfoHandler ShowUpLoadInfo, ShowProgressHandler ShowProgress, Form owner, out bool NeedReboot)
		{
			int num = 0;
			int num2 = -101000;
			NeedReboot = false;
			if (model == null)
			{
				return false;
			}
			int num3 = 0;
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			List<DevCmds> modelList = devCmdsBll.GetModelList($"SN_id={model.ID} and status=0 order by id asc ");
			if (modelList == null || modelList.Count <= 0)
			{
				return true;
			}
			for (int i = 0; i < modelList.Count; i++)
			{
				ShowProgress(i * 100 / modelList.Count);
				num3 = 0;
				DevCmds devCmds = modelList[i];
				if (devCmds != null && devCmds.CmdContent != null && !(devCmds.CmdContent.Trim() == ""))
				{
					string[] array = devCmds.CmdContent.Split("$".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					string cmdContent = devCmds.CmdContent;
					cmdContent = Regex.Replace(cmdContent, "Password\\=.*\\t", "Password=*****\t");
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadingDataStart", "开始同步命令") + ":" + cmdContent);
					if (array.Length > 1)
					{
						if (devCmds.CmdContent.ToLower().StartsWith("set$AntiPassback=".ToLower()))
						{
							List<AccAntiback> list = new List<AccAntiback>();
							if (!AccAntiback.PullCmd2Model(array[1], out AccAntiback item))
							{
								num3 = num2;
							}
							else
							{
								list.Add(item);
								num3 = devServerBll.STD_SetAntiback(list);
							}
						}
						else
						{
							AccDoor objDoorParam;
							switch (array[0].ToLower())
							{
							case "set":
								NeedReboot = true;
								num3 = (AccDoor.PullCmd2Model(array[1], out objDoorParam) ? devServerBll.STD_SetDoorParam(objDoorParam, DeviceHelper.TimeSeg.id) : num2);
								break;
							case "setdev":
								NeedReboot = true;
								num3 = (Machines.PullCmd2Model(array[1], out Machines objDevParam) ? devServerBll.STD_SetDeviceParam(objDevParam) : num2);
								break;
							case "setwiegand":
								NeedReboot = true;
								num3 = (STD_WiegandFmt.PullCmd2Model(array[1], out STD_WiegandFmt wiegandFmt) ? devServerBll.STD_SetWiegandFmt(wiegandFmt) : num2);
								break;
							default:
							{
								string[] array2 = array[0].Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
								if (array2.Length < 2)
								{
									num3 = 0;
								}
								else
								{
									switch (array2[0].ToLower())
									{
									case "set":
										NeedReboot = true;
										num3 = (AccDoor.PullCmd2Model(array2[1], out objDoorParam) ? devServerBll.STD_SetDoorParam(objDoorParam, DeviceHelper.TimeSeg.id) : num2);
										break;
									case "update":
									case "sendfile":
										num3 = DeviceHelper.UpdateTable(array2[1], array[1], devServerBll, ShowUpLoadInfo, ShowProgress);
										break;
									case "delete":
										num3 = DeviceHelper.DeleteTable(array2[1], array[1], devServerBll, ShowUpLoadInfo, ShowProgress);
										break;
									}
								}
								break;
							}
							}
						}
					}
					if (num3 < 0)
					{
						devCmds.status = ((num3 == -100000) ? 1 : 0);
						devCmds.CmdReturnContent = num3.ToString() + "->" + PullSDkErrorInfos.GetInfo(num3);
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadingDataFail", "同步命令失败") + ": " + PullSDkErrorInfos.GetInfo(num3));
						num++;
					}
					else
					{
						devCmds.status = 1;
						devCmds.CmdReturnContent = "ok";
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadingDataFinish", "同步命令结束"));
					}
					devCmdsBll.Update(devCmds);
				}
			}
			return true;
		}

		private static int UpdateTable(string tablename, string rows, DeviceServerBll devServerBll, ShowInfoHandler ShowUpLoadInfo, ShowProgressHandler ShowProgress)
		{
			int result = 0;
			int result2 = -101000;
			if (tablename == null || tablename.Trim() == "")
			{
				return result2;
			}
			string[] array = rows.Split("\r\n#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return 0;
			}
			UserInfo userInfo;
			switch (tablename.ToLower())
			{
			case "user":
			{
				List<UserInfo> list2 = new List<UserInfo>();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int j = 0; j < array.Length; j++)
				{
					if (!UserInfo.PullCmd2Model(array[j], out userInfo))
					{
						return result2;
					}
					list2.Add(userInfo);
					if (userInfo.MorecardGroupId > 0 && !dictionary.ContainsKey(userInfo.MorecardGroupId))
					{
						dictionary.Add(userInfo.MorecardGroupId, userInfo.MorecardGroupId);
					}
				}
				result = devServerBll.STD_SetUserInfo(list2, dictionary);
				break;
			}
			case "usergroup":
			{
				List<UserInfo> list2 = new List<UserInfo>();
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					if (!UserInfo.PullCmd2Model(array[num2], out userInfo))
					{
						return result2;
					}
					list2.Add(userInfo);
				}
				result = devServerBll.STD_SetUserGroup(list2, dictionary);
				break;
			}
			case "userauthorize":
			{
				List<ObjUserAuthorize> list3 = new List<ObjUserAuthorize>();
				for (int k = 0; k < array.Length; k++)
				{
					if (!ObjUserAuthorize.PullCmd2Model(array[k], out ObjUserAuthorize item2))
					{
						return result2;
					}
					list3.Add(item2);
				}
				result = devServerBll.STD_SetUserTimeZone(list3);
				break;
			}
			case "holiday":
			{
				List<AccHolidays> list6 = new List<AccHolidays>();
				for (int n = 0; n < array.Length; n++)
				{
					if (!AccHolidays.PullCmd2Model(array[n], out AccHolidays item4))
					{
						return result2;
					}
					list6.Add(item4);
				}
				result = devServerBll.STD_SetHoliday(list6);
				break;
			}
			case "timezone":
			{
				List<AccTimeseg> list8 = new List<AccTimeseg>();
				for (int num3 = 0; num3 < array.Length; num3++)
				{
					if (!AccTimeseg.PullCmd2Model(array[num3], out AccTimeseg item6))
					{
						return result2;
					}
					list8.Add(item6);
				}
				result = devServerBll.STD_SetTimeZone(list8, DeviceHelper.TimeSeg.id);
				break;
			}
			case "ssrface7":
			{
				List<FaceTemp> list5 = new List<FaceTemp>();
				for (int m = 0; m < array.Length; m++)
				{
					if (!FaceTemp.PullCmd2Model(array[m], out FaceTemp faceTemp))
					{
						return result2;
					}
					if (devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
					{
						faceTemp.FaceType = 1;
					}
					list5.Add(faceTemp);
				}
				result = devServerBll.STD_SetUserFaceTemplate(list5);
				break;
			}
			case "multimcard":
			{
				List<ObjMultimCard> list9 = new List<ObjMultimCard>();
				for (int num4 = 0; num4 < array.Length; num4++)
				{
					if (!ObjMultimCard.PullCmd2Model(array[num4], out ObjMultimCard item7))
					{
						return result2;
					}
					list9.Add(item7);
				}
				result = devServerBll.STD_SetUnlockGroup(list9);
				break;
			}
			case "templatev10":
			{
				List<Template> list7 = new List<Template>();
				for (int num = 0; num < array.Length; num++)
				{
					if (!Template.PullCmd2Model(array[num], out Template item5))
					{
						return result2;
					}
					list7.Add(item5);
				}
				result = devServerBll.STD_SetUserFPTemplate(list7);
				break;
			}
			case "group":
			{
				List<ZK.Data.Model.StdSDK.Group> list4 = new List<ZK.Data.Model.StdSDK.Group>();
				for (int l = 0; l < array.Length; l++)
				{
					if (!ZK.Data.Model.StdSDK.Group.PullCmd2Model(array[l], out ZK.Data.Model.StdSDK.Group item3))
					{
						return result2;
					}
					list4.Add(item3);
				}
				result = devServerBll.STD_SetGroup(list4);
				break;
			}
			case "uservt":
			{
				List<UserVerifyType> list = new List<UserVerifyType>();
				for (int i = 0; i < array.Length; i++)
				{
					if (!UserVerifyType.PullCmd2Model(array[i], out UserVerifyType item))
					{
						return result2;
					}
					list.Add(item);
				}
				result = devServerBll.STD_SetUserVerifyMode(list);
				break;
			}
			}
			return result;
		}

		private static int DeleteTable(string tablename, string rows, DeviceServerBll devServerBll, ShowInfoHandler ShowUpLoadInfo, ShowProgressHandler ShowProgress)
		{
			int result = 0;
			int result2 = -101000;
			if (tablename == null || tablename.Trim() == "")
			{
				return result2;
			}
			string[] array = rows.Split("\r\n#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return 0;
			}
			switch (tablename.ToLower())
			{
			case "user":
			{
				List<UserInfo> list3 = new List<UserInfo>();
				for (int k = 0; k < array.Length; k++)
				{
					if (!UserInfo.PullCmd2Model(array[k], out UserInfo item3))
					{
						return result2;
					}
					list3.Add(item3);
				}
				result = devServerBll.STD_DeleteUserInfo(list3);
				break;
			}
			case "holiday":
			{
				List<AccHolidays> list2 = new List<AccHolidays>();
				for (int j = 0; j < array.Length; j++)
				{
					if (!AccHolidays.PullCmd2Model(array[j], out AccHolidays item2))
					{
						return result2;
					}
					list2.Add(item2);
				}
				break;
			}
			case "ssrface7":
			{
				List<FaceTemp> list4 = new List<FaceTemp>();
				for (int l = 0; l < array.Length; l++)
				{
					if (!FaceTemp.PullCmd2Model(array[l], out FaceTemp faceTemp))
					{
						return result2;
					}
					if (devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
					{
						faceTemp.FaceType = 1;
					}
					list4.Add(faceTemp);
				}
				result = devServerBll.STD_DeleteUserFaceTemplate(list4);
				break;
			}
			case "templatev10":
			{
				List<Template> list = new List<Template>();
				for (int i = 0; i < array.Length; i++)
				{
					if (!Template.PullCmd2Model(array[i], out Template item))
					{
						return result2;
					}
					list.Add(item);
				}
				result = devServerBll.STD_DeleteUserFPTemplate(list);
				break;
			}
			}
			return result;
		}

		private static string GetTimeZoneString(DateTime StartDate, DateTime EndDate)
		{
			TimeSpan timeSpan = EndDate - StartDate;
			if (timeSpan.Hours <= 0 && timeSpan.Minutes <= 0)
			{
				return " ";
			}
			return StartDate.ToString("HHmm") + EndDate.ToString("HHmm");
		}

		private static int GetSTDAloneSDKTimeZoneDic(List<AccTimeseg> lstTimeZone, out Dictionary<int, Dictionary<int, string>> DicTimeZone, out int HolidayTZId)
		{
			int num = 0;
			DicTimeZone = new Dictionary<int, Dictionary<int, string>>();
			HolidayTZId = 0;
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(1, "00002359000023590000235900002359000023590000235900002359");
			num++;
			DicTimeZone.Add(1, dictionary);
			for (int i = 0; i < lstTimeZone.Count; i++)
			{
				AccTimeseg accTimeseg = lstTimeZone[i];
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.sunday_start1.Value, accTimeseg.sunday_end1.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.monday_start1.Value, accTimeseg.monday_end1.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.tuesday_start1.Value, accTimeseg.tuesday_end1.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.wednesday_start1.Value, accTimeseg.wednesday_end1.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.thursday_start1.Value, accTimeseg.thursday_end1.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.friday_start1.Value, accTimeseg.friday_end1.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.saturday_start1.Value, accTimeseg.saturday_end1.Value));
				if (!string.IsNullOrEmpty(stringBuilder.ToString().Trim()))
				{
					dictionary = ((!DicTimeZone.ContainsKey(accTimeseg.id)) ? new Dictionary<int, string>() : DicTimeZone[accTimeseg.id]);
					num++;
					dictionary.Add(num, stringBuilder.ToString().Replace(" ", "00000000"));
					if (DicTimeZone.ContainsKey(accTimeseg.id))
					{
						DicTimeZone[accTimeseg.id] = dictionary;
					}
					else
					{
						DicTimeZone.Add(accTimeseg.id, dictionary);
					}
				}
				stringBuilder = new StringBuilder();
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.sunday_start2.Value, accTimeseg.sunday_end2.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.monday_start2.Value, accTimeseg.monday_end2.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.tuesday_start2.Value, accTimeseg.tuesday_end2.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.wednesday_start2.Value, accTimeseg.wednesday_end2.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.thursday_start2.Value, accTimeseg.thursday_end2.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.friday_start2.Value, accTimeseg.friday_end2.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.saturday_start2.Value, accTimeseg.saturday_end2.Value));
				if (!string.IsNullOrEmpty(stringBuilder.ToString().Trim()))
				{
					dictionary = ((!DicTimeZone.ContainsKey(accTimeseg.id)) ? new Dictionary<int, string>() : DicTimeZone[accTimeseg.id]);
					num++;
					dictionary.Add(num, stringBuilder.ToString().Replace(" ", "00000000"));
					if (DicTimeZone.ContainsKey(accTimeseg.id))
					{
						DicTimeZone[accTimeseg.id] = dictionary;
					}
					else
					{
						DicTimeZone.Add(accTimeseg.id, dictionary);
					}
				}
				stringBuilder = new StringBuilder();
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.sunday_start3.Value, accTimeseg.sunday_end3.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.monday_start3.Value, accTimeseg.monday_end3.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.tuesday_start3.Value, accTimeseg.tuesday_end3.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.wednesday_start3.Value, accTimeseg.wednesday_end3.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.thursday_start3.Value, accTimeseg.thursday_end3.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.friday_start3.Value, accTimeseg.friday_end3.Value));
				stringBuilder.Append(DeviceHelper.GetTimeZoneString(accTimeseg.saturday_start3.Value, accTimeseg.saturday_end3.Value));
				if (!string.IsNullOrEmpty(stringBuilder.ToString().Trim()))
				{
					dictionary = ((!DicTimeZone.ContainsKey(accTimeseg.id)) ? new Dictionary<int, string>() : DicTimeZone[accTimeseg.id]);
					num++;
					dictionary.Add(num, stringBuilder.ToString().Replace(" ", "00000000"));
					if (DicTimeZone.ContainsKey(accTimeseg.id))
					{
						DicTimeZone[accTimeseg.id] = dictionary;
					}
					else
					{
						DicTimeZone.Add(accTimeseg.id, dictionary);
					}
				}
				if (!DicTimeZone.ContainsKey(accTimeseg.id))
				{
					stringBuilder = new StringBuilder();
					stringBuilder.Append("00000000");
					stringBuilder.Append("00000000");
					stringBuilder.Append("00000000");
					stringBuilder.Append("00000000");
					stringBuilder.Append("00000000");
					stringBuilder.Append("00000000");
					stringBuilder.Append("00000000");
					num++;
					dictionary = new Dictionary<int, string>();
					dictionary.Add(num, stringBuilder.ToString());
					DicTimeZone.Add(accTimeseg.id, dictionary);
				}
			}
			string text = "";
			for (int num2 = lstTimeZone.Count - 1; num2 >= 0; num2--)
			{
				AccTimeseg accTimeseg = lstTimeZone[num2];
				text = DeviceHelper.GetTimeZoneString(accTimeseg.holidaytype1_start1.Value, accTimeseg.holidaytype1_end1.Value);
				if (!string.IsNullOrEmpty(text.Trim()))
				{
					break;
				}
			}
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text);
				stringBuilder.Append(text);
				stringBuilder.Append(text);
				stringBuilder.Append(text);
				stringBuilder.Append(text);
				stringBuilder.Append(text);
				stringBuilder.Append(text);
				num = (HolidayTZId = num + 1);
				dictionary = new Dictionary<int, string>();
				dictionary.Add(num, stringBuilder.ToString());
				DicTimeZone.Add(0, dictionary);
			}
			return num;
		}

		public static int UpdateMachineParameter(int MachineId)
		{
			int result = -999;
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			Machines model = machinesBll.GetModel(MachineId);
			if (model == null)
			{
				return result;
			}
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
			if (deviceServer == null)
			{
				return result;
			}
			result = deviceServer.Connect(3000);
			if (result < 0)
			{
				return result;
			}
			Machines machines = model.Copy();
			result = DeviceHelper.GetDeviceParams(model);
			deviceServer.Disconnect();
			if (result >= 0)
			{
				model.IP = machines.IP;
				machinesBll.Update(model);
				AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
				List<AccReader> modelList = accReaderBll.GetModelList("door_id in (select id from acc_door where device_id = " + model.ID + ")");
				if ((modelList == null || modelList.Count <= 0) && model.DevSDKType != SDKType.StandaloneSDK)
				{
					DeviceHelper.AddReader(model);
				}
				AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
				List<AccAuxiliary> modelList2 = accAuxiliaryBll.GetModelList("device_id = " + model.ID);
				if ((modelList2 == null || modelList2.Count <= 0) && (model.aux_in_count > 0 || model.aux_out_count > 0))
				{
					DeviceHelper.AddAuxiliary(model);
				}
			}
			return result;
		}

		private static bool AddReader(Machines model)
		{
			int num = 0;
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + model.ID + " Order by door_no");
			AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
			List<AccReader> list = new List<AccReader>();
			for (int i = 0; i < modelList.Count; i++)
			{
				num++;
				AccReader accReader = new AccReader();
				accReader.ReaderNo = num;
				accReader.DoorId = modelList[i].id;
				accReader.ReaderName = modelList[i].door_name + " " + ShowMsgInfos.GetInfo("AccReaderInState", "入");
				accReader.ReaderState = AccReader.AccReaderState.In;
				list.Add(accReader);
				int device_type = model.device_type;
				if (device_type == 2)
				{
					if (model.Ext485ReaderFunOn > 0)
					{
						num++;
						accReader = new AccReader();
						accReader.ReaderNo = num;
						accReader.DoorId = modelList[i].id;
						accReader.ReaderName = modelList[i].door_name + " " + ShowMsgInfos.GetInfo("AccReaderOutState", "出");
						accReader.ReaderState = AccReader.AccReaderState.Out;
						list.Add(accReader);
					}
				}
				else
				{
					num++;
					accReader = new AccReader();
					accReader.ReaderNo = num;
					accReader.DoorId = modelList[i].id;
					accReader.ReaderName = modelList[i].door_name + " " + ShowMsgInfos.GetInfo("AccReaderOutState", "出");
					accReader.ReaderState = AccReader.AccReaderState.Out;
					list.Add(accReader);
				}
			}
			return accReaderBll.Add(list) > 0;
		}

		private static bool AddAuxiliary(Machines model)
		{
			int num = 0;
			int[] array = null;
			int[] array2 = null;
			AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
			List<AccAuxiliary> list = new List<AccAuxiliary>();
			switch (model.device_type)
			{
			case 3:
			case 6:
				array = new int[4]
				{
					9,
					10,
					11,
					12
				};
				array2 = new int[6]
				{
					2,
					4,
					6,
					8,
					9,
					10
				};
				break;
			case 5:
				array = new int[4]
				{
					409,
					410,
					411,
					412
				};
				array2 = new int[4]
				{
					2,
					4,
					409,
					410
				};
				break;
			default:
			{
				int[] array3 = new int[InAddressInfo.GetDic().Count];
				int[] array4 = new int[OutAddressInfo.GetDic().Count];
				InAddressInfo.GetDic().Keys.CopyTo(array3, 0);
				OutAddressInfo.GetDic().Keys.CopyTo(array4, 0);
				array = new int[model.aux_in_count];
				array2 = new int[model.aux_out_count];
				try
				{
					for (int i = 1; i <= model.aux_in_count; i++)
					{
						array[i - 1] = array3[i];
					}
					for (int j = 1; j <= model.aux_out_count; j++)
					{
						array2[j - 1] = array4[j];
					}
				}
				catch (IndexOutOfRangeException)
				{
					throw new Exception(ShowMsgInfos.GetInfo("AddInOutAuxiliaryCountError", "辅助输入输出数量错误"));
				}
				break;
			}
			}
			for (num = 0; num < array.Length; num++)
			{
				AccAuxiliary accAuxiliary = new AccAuxiliary();
				accAuxiliary.AuxName = InAddressInfo.GetInfo(array[num]);
				accAuxiliary.AuxNo = array[num] % 100;
				accAuxiliary.AuxState = AccAuxiliary.AccAuxiliaryState.In;
				accAuxiliary.DeviceId = model.ID;
				accAuxiliary.PrinterNumber = InAddressInfo.GetInfo(array[num]);
				list.Add(accAuxiliary);
			}
			for (num = 0; num < array2.Length; num++)
			{
				AccAuxiliary accAuxiliary2 = new AccAuxiliary();
				accAuxiliary2.AuxName = OutAddressInfo.GetInfo(array2[num]);
				accAuxiliary2.AuxNo = array2[num] % 100;
				accAuxiliary2.AuxState = AccAuxiliary.AccAuxiliaryState.Out;
				accAuxiliary2.DeviceId = model.ID;
				accAuxiliary2.PrinterNumber = OutAddressInfo.GetInfo(array2[num]);
				list.Add(accAuxiliary2);
			}
			return accAuxiliaryBll.Add(list) > 0;
		}

		public static int UpdateDataCount(Machines machine, ShowInfoHandler ShowInfo = null)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int errorCode = DeviceHelper.m_errorCode;
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machine);
			if (deviceServer == null)
			{
				return errorCode;
			}
			errorCode = deviceServer.Connect(3000);
			if (errorCode < 0)
			{
				return errorCode;
			}
			if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
			{
				errorCode = deviceServer.STD_GetRecordCount(MachineDataStatusCode.RegistedUserCount, out num2);
				if (errorCode < 0)
				{
					return errorCode;
				}
				errorCode = deviceServer.STD_GetRecordCount(MachineDataStatusCode.TemplateCount, out num);
				if (errorCode < 0)
				{
					return errorCode;
				}
				if (machine.FaceFunOn == 1)
				{
					errorCode = deviceServer.STD_GetRecordCount(MachineDataStatusCode.FaceTemplateCount, out num4);
					if (errorCode < 0)
					{
						return errorCode;
					}
				}
			}
			else
			{
				num2 = deviceServer.GetDeviceDataCount("user", "", "");
				if (num2 < 0)
				{
					return num2;
				}
				if (machine.SupportFingerprint)
				{
					num = deviceServer.GetDeviceDataCount("templatev10", "", "");
					if (num < 0)
					{
						return num;
					}
				}
				if (machine.SupportFingerVein)
				{
					num3 = deviceServer.GetDeviceDataCount("fvtemplate", "", "");
					if (num3 < 0)
					{
						return num3;
					}
				}
				if (machine.SupportFace)
				{
					num4 = deviceServer.GetDeviceDataCount("ssrface7", "", "");
					if (num4 < 0)
					{
						return num4;
					}
				}
			}
			num2 = ((num2 >= 0) ? num2 : 0);
			num = ((num >= 0) ? num : 0);
			num3 = ((num3 >= 0) ? num3 : 0);
			num4 = ((num4 >= 0) ? num4 : 0);
			machine.usercount = (short)num2;
			machine.fingercount = (short)num;
			machine.fvcount = (short)num3;
			machine.FaceCount = num4;
			if (ShowInfo != null)
			{
				ShowInfo(ShowMsgInfos.GetInfo("UserCount", "人员数: ") + num2);
				ShowInfo(ShowMsgInfos.GetInfo("FingerPrintCount", "指纹数: ") + num);
				ShowInfo(ShowMsgInfos.GetInfo("FingerVeinCount", "指静脉数: ") + num3);
				ShowInfo(ShowMsgInfos.GetInfo("FaceCount", "人脸数: ") + num4);
			}
			return errorCode;
		}

		public static bool PcToDevice(Machines machine, ShowInfoHandler ShowUpLoadInfo, ShowProgressHandler ShowProgress)
		{
			int num = 0;
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machine);
			if (deviceServer == null)
			{
				return false;
			}
			num = 0;
			bool flag = true;
			if (machine.DevSDKType == SDKType.StandaloneSDK)
			{
				num = deviceServer.STD_ClearUser();
				if (num < 0)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteUserInfoFailed", "重置人员信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					return false;
				}
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteUserInfoSucceed", "重置人员信息成功"));
			}
			else
			{
				flag = true;
				num = deviceServer.DeleteDeviceData("UserAuthorize", "", "");
				if (num < 0)
				{
					flag = false;
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelLevelsetFalse", "重置人员权限失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
				if (flag)
				{
					num = deviceServer.DeleteDeviceData("User", "", "");
					if (num < 0)
					{
						flag = false;
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelUserInfoFalse", "重置用户信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				if (flag && machine.SupportFingerprint)
				{
					num = deviceServer.DeleteDeviceData("templatev10", "", "");
					if (num < 0)
					{
						flag = false;
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteTemplateFalse", "重置用户指纹信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				if (flag && machine.SupportFingerVein)
				{
					num = deviceServer.DeleteDeviceData("fvtemplate", "", "");
					if (num < 0)
					{
						flag = false;
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteFVFalse", "重置用户指静脉信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				if (flag && machine.SupportFace)
				{
					num = deviceServer.DeleteDeviceData("ssrface7", "", "");
					if (num < 0)
					{
						flag = false;
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteFaceTemplateFalse", "重置面部识别信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
			}
			ShowProgress(3);
			num = 0;
			Dictionary<int, AccTimeseg> dictionary = new Dictionary<int, AccTimeseg>();
			ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadTZInfo", "正在从PC同步时间段信息到设备..."));
			try
			{
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
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
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					num = deviceServer.DeleteDeviceData("TimeZone", "", "");
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelTimeZoneFalse", "门禁时间段重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
				}
				if (num >= 0)
				{
					if (modelList == null || modelList.Count <= 0)
					{
						List<AccTimeseg> list = new List<AccTimeseg>();
						AccTimeseg accTimeseg = new AccTimeseg();
						accTimeseg.id = 1;
						accTimeseg.sunday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.sunday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.sunday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.sunday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.sunday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.sunday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.monday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.monday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.monday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.monday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.monday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.monday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.tuesday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.tuesday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.tuesday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.tuesday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.tuesday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.tuesday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.wednesday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.wednesday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.wednesday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.wednesday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.wednesday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.wednesday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.thursday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.thursday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.thursday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.thursday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.thursday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.thursday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.friday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.friday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.friday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.friday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.friday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.friday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.saturday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.saturday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.saturday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.saturday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.saturday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.saturday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype1_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype1_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype1_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype1_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype1_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype1_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype2_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype2_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype2_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype2_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype2_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype2_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype3_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype3_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype3_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
						accTimeseg.holidaytype3_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype3_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.holidaytype3_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
						accTimeseg.TimeZone1Id = 1;
						list.Add(accTimeseg);
						num = deviceServer.STD_SetTimeZone(list, DeviceHelper.TimeSeg.id);
					}
					else
					{
						num = deviceServer.STD_SetTimeZone(modelList, DeviceHelper.TimeSeg.id);
					}
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneFalse", "从PC同步门禁时间段信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
					else
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneOk", "从PC同步门禁时间段信息到设备成功"));
					}
				}
			}
			catch (Exception ex)
			{
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneFalse", "从PC同步门禁时间段信息到设备失败") + ":" + ex.Message);
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					goto end_IL_0b51;
				}
				return false;
				end_IL_0b51:;
			}
			ShowProgress(6);
			num = 0;
			ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadHolidayInfo", "正在从PC同步节假日信息到设备..."));
			try
			{
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					num = deviceServer.DeleteDeviceData("Holiday", "", "");
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelHolidayFalse", "节假日重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
				}
				if (num >= 0)
				{
					AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
					List<AccHolidays> modelList2 = accHolidaysBll.GetModelList("");
					num = deviceServer.STD_SetHoliday(modelList2);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayFalse", "从PC同步节假日信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
					else
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayOk", "从PC同步节假日信息到设备成功"));
					}
				}
			}
			catch (Exception ex2)
			{
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayFailed", "设置节假日失败") + ": " + ex2.Message);
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					goto end_IL_0cc7;
				}
				return false;
				end_IL_0cc7:;
			}
			ShowProgress(8);
			num = 0;
			if (machine.DevSDKType == SDKType.StandaloneSDK)
			{
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingDeviceParam", "正在设置设备参数"));
					num = deviceServer.STD_SetDeviceParam(machine);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDeviceParamFailed", "设置设备参数失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
					else
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDeviceParamSucceed", "从PC同步设备参数成功"));
					}
				}
				catch (Exception ex3)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDeviceParamFailed", "设置设备参数失败") + ": " + ex3.Message);
					if (machine.DevSDKType != SDKType.StandaloneSDK)
					{
						goto end_IL_0daf;
					}
					return false;
					end_IL_0daf:;
				}
			}
			ShowProgress(12);
			num = 0;
			ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadAntiPassbackInfo", "正在从PC同步反潜设置信息到设备..."));
			try
			{
				AccAntibackBll accAntibackBll = new AccAntibackBll(MainForm._ia);
				List<AccAntiback> modelList3 = accAntibackBll.GetModelList("device_id=" + deviceServer.DevInfo.ID);
				num = deviceServer.STD_SetAntiback(modelList3);
				if (num < 0)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackTypeFalse", "从PC同步反潜设置信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
					if (machine.DevSDKType == SDKType.StandaloneSDK)
					{
						return false;
					}
				}
				else
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackTypeOk", "从PC同步反潜设置信息到设备成功"));
				}
			}
			catch (Exception ex4)
			{
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackTypeFalse", "从PC同步反潜设置信息到设备失败") + ": " + ex4.Message);
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					goto end_IL_0eb6;
				}
				return false;
				end_IL_0eb6:;
			}
			ShowProgress(13);
			num = 0;
			Dictionary<int, AccDoor> dictionary2 = new Dictionary<int, AccDoor>();
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList4 = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group where accdoor_device_id=" + machine.ID + ")");
			ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingDoorParam", "正在设置门参数"));
			try
			{
				if (modelList4 != null && modelList4.Count > 0)
				{
					for (int j = 0; j < modelList4.Count; j++)
					{
						AccDoor accDoor = modelList4[j];
						if (!dictionary2.ContainsKey(accDoor.id))
						{
							dictionary2.Add(accDoor.id, accDoor);
						}
						if (dictionary.ContainsKey(accDoor.lock_active_id))
						{
							AccTimeseg accTimeseg = dictionary[accDoor.lock_active_id];
							accDoor.GroupTZ1 = accTimeseg.TimeZone1Id;
							accDoor.GroupTZ2 = accTimeseg.TimeZone2Id;
							accDoor.GroupTZ3 = accTimeseg.TimeZone3Id;
						}
						if (dictionary.ContainsKey(accDoor.long_open_id))
						{
							AccTimeseg accTimeseg = dictionary[accDoor.long_open_id];
							accDoor.NormalOpenTZ = accTimeseg.TimeZone1Id;
						}
						num = deviceServer.STD_SetDoorParam(accDoor, DeviceHelper.TimeSeg.id);
						if (num < 0)
						{
							break;
						}
					}
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDoorParamFailed", "设置门参数失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
				}
			}
			catch (Exception ex5)
			{
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDoorParamFailed", "设置门参数失败") + ": " + ex5.Message);
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					goto end_IL_10bd;
				}
				return false;
				end_IL_10bd:;
			}
			ShowProgress(16);
			num = 0;
			Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
			if (machine.DevSDKType == SDKType.StandaloneSDK)
			{
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingGroup", "正在设置人员组"));
					AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
					List<AccMorecardempGroup> modelList5 = accMorecardempGroupBll.GetModelList("");
					modelList5 = (modelList5 ?? new List<AccMorecardempGroup>());
					List<ZK.Data.Model.StdSDK.Group> lstGroup = CommandServer.ConvertGroup(machine, modelList5);
					for (int k = 0; k < modelList5.Count; k++)
					{
						if (!dictionary3.ContainsKey(modelList5[k].id))
						{
							dictionary3.Add(modelList5[k].id, (modelList5[k].StdGroupNo <= 0) ? 1 : modelList5[k].StdGroupNo);
						}
					}
					num = deviceServer.STD_SetGroup(lstGroup);
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetGroupFailed", "设置人员组失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				catch (Exception ex6)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetGroupFailed", "设置人员组失败") + ": " + ex6.Message);
					return false;
				}
			}
			ShowProgress(18);
			try
			{
				num = 0;
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingWiegandFmt", "正在设置韦根格式"));
				SDKType devSDKType = machine.DevSDKType;
				if (devSDKType == SDKType.StandaloneSDK)
				{
					STD_WiegandFmtBll sTD_WiegandFmtBll = new STD_WiegandFmtBll(MainForm._ia);
					List<STD_WiegandFmt> modelList6 = sTD_WiegandFmtBll.GetModelList("DoorId in (Select id from acc_door where device_id in(Select id from Machines where id=" + machine.ID + "))");
					if (modelList6 != null && modelList6.Count > 0)
					{
						for (int l = 0; l < modelList6.Count; l++)
						{
							num = deviceServer.STD_SetWiegandFmt(modelList6[l]);
							if (num < 0)
							{
								ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetWiegandFmtFailed", "设置韦根格式失败") + ": " + PullSDkErrorInfos.GetInfo(num));
								return false;
							}
						}
					}
				}
				else if (modelList4 != null && modelList4.Count > 0)
				{
					for (int m = 0; m < modelList4.Count; m++)
					{
						AccDoor accDoor = modelList4[m];
						string deviceParam = PullSDKDataConvertHelper.SetWiegandParam(accDoor, machine);
						num = deviceServer.SetDeviceParam(deviceParam);
						if (num < 0)
						{
							ShowUpLoadInfo(accDoor.door_name + ": " + ShowMsgInfos.GetInfo("SetWiegandFmtFailed", "设置韦根格式失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						}
					}
				}
			}
			catch (Exception ex7)
			{
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetWiegandFmtFailed", "设置韦根格式失败") + ": " + ex7.Message);
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					goto end_IL_13f8;
				}
				return false;
				end_IL_13f8:;
			}
			ShowProgress(20);
			if (machine.DevSDKType != SDKType.StandaloneSDK)
			{
				num = 0;
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadInterLockInfo", "正在从PC同步互锁设置信息到设备..."));
				try
				{
					AccInterlockBll accInterlockBll = new AccInterlockBll(MainForm._ia);
					List<AccInterlock> modelList7 = accInterlockBll.GetModelList("device_id=" + machine.ID);
					num = deviceServer.SetInterlock(modelList7);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInterlockTypeOk", "从PC同步互锁设置信息到设备成功"));
					}
					else
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInterlockTypeFalse", "从PC同步互锁设置信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
					}
				}
				catch (Exception ex8)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInterlockTypeFalse", "从PC同步互锁设置信息到设备失败") + ": " + ex8.Message);
				}
			}
			ShowProgress(22);
			if (machine.DevSDKType != SDKType.StandaloneSDK)
			{
				num = 0;
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadInOutFunInfo", "正在从PC同步联动设置信息到设备..."));
				try
				{
					AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
					List<AccLinkAgeIo> modelList8 = accLinkAgeIoBll.GetModelList("device_id=" + machine.ID);
					num = deviceServer.DeleteDeviceData("InOutFun", "", "");
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInOutFunFalse", "从PC同步联动信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
					}
					else
					{
						num = deviceServer.SetLinkage(modelList8);
						if (num >= 0)
						{
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInOutFunOk", "从PC同步联动信息到设备成功"));
						}
						else
						{
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInOutFunFalse", "从PC同步联动信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						}
					}
				}
				catch (Exception ex9)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInOutFunFalse", "从PC同步联动信息到设备失败") + ": " + ex9.Message);
				}
			}
			ShowProgress(24);
			try
			{
				num = 0;
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadMultimCardInfo", "正在从PC同步多卡开门设置信息到设备..."));
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					num = deviceServer.DeleteDeviceData("multimcard", "", "");
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelMultimCardFalse", "多卡开门重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int n = 1; n <= machine.door_count; n++)
						{
							stringBuilder.AppendFormat("Door{0}MultiCardOpenDoor=0,", n);
						}
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Remove(stringBuilder.Length - 1, 1);
							deviceServer.SetDeviceParam(stringBuilder.ToString());
						}
					}
				}
				if (num >= 0)
				{
					List<ObjMultimCard> allUnlockGroup = CommandServer.GetAllUnlockGroup(machine);
					num = deviceServer.STD_SetUnlockGroup(allUnlockGroup);
					if (num >= 0)
					{
						if (machine.DevSDKType != SDKType.StandaloneSDK && allUnlockGroup != null && allUnlockGroup.Count > 0)
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int num2 = 0; num2 < allUnlockGroup.Count; num2++)
							{
								stringBuilder.AppendFormat("Door{0}MultiCardOpenDoor=1,", allUnlockGroup[num2].DoorId);
							}
							if (stringBuilder.Length > 0)
							{
								stringBuilder.Remove(stringBuilder.Length - 1, 1);
								deviceServer.SetDeviceParam(stringBuilder.ToString());
							}
						}
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetMultimCardOk", "从PC同步多卡开门信息到设备成功") + ((machine.DevSDKType == SDKType.StandaloneSDK) ? (", " + ShowMsgInfos.GetInfo("OpenDoorCombinationCount", "组合数量") + ": " + num) : ""));
					}
					else
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetMultimCardFalse", "从PC同步多卡开门信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						if (machine.DevSDKType == SDKType.StandaloneSDK)
						{
							return false;
						}
					}
				}
			}
			catch (Exception ex10)
			{
				ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetMultimCardFalse", "从PC同步多卡开门信息到设备失败") + ": " + ex10.Message);
				if (machine.DevSDKType != SDKType.StandaloneSDK)
				{
					goto end_IL_18c0;
				}
				return false;
				end_IL_18c0:;
			}
			ShowProgress(25);
			List<UserInfo> list2 = null;
			Dictionary<int, UserInfo> dictionary4 = null;
			num = 0;
			int num3 = 500;
			List<UserInfo> list3 = new List<UserInfo>();
			List<UserVerifyType> list4 = new List<UserVerifyType>();
			int num7;
			if (flag)
			{
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingUser", "正在同步人员信息到设备"));
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					list2 = userInfoBll.GetModelList("USERID in (Select employee_id from acc_levelset_emp where acclevelset_id in (select acclevelset_id from acc_levelset where acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + machine.ID + ")))");
					list2 = (list2 ?? new List<UserInfo>());
					List<UserInfo> list5 = new List<UserInfo>();
					int num4 = (machine.DevSDKType != SDKType.StandaloneSDK) ? 6 : (machine.IsTFTMachine ? 8 : 5);
					dictionary4 = new Dictionary<int, UserInfo>();
					Dictionary<int, int> dictionary5 = new Dictionary<int, int>();
					for (int num5 = 0; num5 < list2.Count; num5++)
					{
						UserInfo userInfo = list2[num5];
						if (!dictionary4.ContainsKey(userInfo.UserId))
						{
							dictionary4.Add(userInfo.UserId, userInfo);
						}
						UserVerifyType userVerifyType = new UserVerifyType();
						userVerifyType.Pin = int.Parse(userInfo.BadgeNumber);
						userVerifyType.VerifyType = 0;
						userVerifyType.UseGroupVT = true;
						if (dictionary3.ContainsKey(userInfo.MorecardGroupId))
						{
							userVerifyType.GroupNo = dictionary3[userInfo.MorecardGroupId];
						}
						else
						{
							userVerifyType.GroupNo = 1;
						}
						list4.Add(userVerifyType);
						if (userInfo.PassWord != null && userInfo.PassWord.Length > num4)
						{
							list5.Add(userInfo);
						}
					}
					MultiChoiceDialog multiChoiceDialog = new MultiChoiceDialog();
					List<DialogChoice> list6 = new List<DialogChoice>();
					DialogResult dialogResult = DialogResult.None;
					list6.Add(new DialogChoice(1, ShowMsgInfos.GetInfo("CutPwd", "截断密码"), ShowMsgInfos.GetInfo("CutPwdDesc", "忽略密码超长的部分，只上传匹配长度的密码")));
					list6.Add(new DialogChoice(2, ShowMsgInfos.GetInfo("ClearPwd", "忽略密码"), ShowMsgInfos.GetInfo("NotUploadPwd", "不上传密码")));
					list6.Add(new DialogChoice(3, ShowMsgInfos.GetInfo("Skip", "跳过"), ShowMsgInfos.GetInfo("DonotUpload", "不上传此用户")));
					for (int num6 = 0; num6 < list5.Count; num6++)
					{
						UserInfo userInfo = list5[num6];
						dialogResult = DialogResult.OK;
						if (dialogResult != DialogResult.OK)
						{
							ShowProgress(100);
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("UserCanceled", "用户取消操作"));
							return false;
						}
						int? selectedChoice = multiChoiceDialog.SelectedChoice;
						int? nullable = selectedChoice;
						if (nullable.HasValue)
						{
							num7 = nullable.GetValueOrDefault();
							switch (num7)
							{
							case 1:
								userInfo.PassWord = userInfo.PassWord.Substring(0, num4);
								break;
							case 2:
								userInfo.PassWord = "";
								break;
							case 3:
								list2.Remove(userInfo);
								if (dictionary5.ContainsKey(int.Parse(userInfo.BadgeNumber)))
								{
									dictionary5.Remove(int.Parse(userInfo.BadgeNumber));
								}
								break;
							}
						}
					}
					List<UserInfo> list7 = new List<UserInfo>();
					for (int num8 = 0; num8 < list2.Count; num8++)
					{
						list7.Add(list2[num8]);
						if ((num8 > 0 && (num8 + 1) % num3 == 0) || num8 == list2.Count - 1)
						{
							num = deviceServer.STD_SetUserInfo(list7, dictionary3);
							if (num >= 0)
							{
								ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoOk", "从PC同步人员信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ": " + ((machine.DevSDKType == SDKType.StandaloneSDK) ? num : list7.Count));
							}
							else
							{
								ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoFalse", "从PC同步人员信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
								if (machine.DevSDKType == SDKType.StandaloneSDK)
								{
									return false;
								}
							}
							list7 = new List<UserInfo>();
						}
					}
				}
				catch (Exception ex11)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoFalse", "从PC同步人员信息到设备失败") + ": " + ex11.Message);
					if (machine.DevSDKType != SDKType.StandaloneSDK)
					{
						goto end_IL_1d83;
					}
					return false;
					end_IL_1d83:;
				}
			}
			ShowProgress(30);
			AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
			List<AccLevelset> modelList9 = accLevelsetBll.GetModelList("id in (Select acclevelset_id from acc_levelset_door_group where accdoor_id in (select id from acc_door where device_id=" + machine.ID + "))");
			Dictionary<int, AccLevelset> dictionary6 = new Dictionary<int, AccLevelset>();
			if (modelList9 != null && modelList9.Count > 0)
			{
				for (int num9 = 0; num9 < modelList9.Count; num9++)
				{
					dictionary6.Add(modelList9[num9].id, modelList9[num9]);
				}
			}
			if (flag)
			{
				try
				{
					num = 0;
					num3 = 200;
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingUserTimeZone", "正在同步人员时间段到设备"));
					List<ObjUserAuthorize> list8 = new List<ObjUserAuthorize>();
					int num12;
					if (machine.DevSDKType == SDKType.StandaloneSDK)
					{
						AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
						List<AccLevelsetEmp> modelList10 = accLevelsetEmpBll.GetModelList("acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + machine.ID + ")");
						Dictionary<int, AccTimeseg> dictionary7 = new Dictionary<int, AccTimeseg>();
						Dictionary<int, List<AccLevelset>> dictionary8 = new Dictionary<int, List<AccLevelset>>();
						for (int num10 = 0; num10 < modelList10.Count; num10++)
						{
							AccLevelsetEmp accLevelsetEmp = modelList10[num10];
							if (!dictionary7.ContainsKey(accLevelsetEmp.employee_id))
							{
								if (dictionary6.ContainsKey(accLevelsetEmp.acclevelset_id))
								{
									dictionary7.Add(accLevelsetEmp.employee_id, dictionary[dictionary6[accLevelsetEmp.acclevelset_id].level_timeseg_id]);
								}
							}
							else if (dictionary6.ContainsKey(accLevelsetEmp.acclevelset_id))
							{
								dictionary7[accLevelsetEmp.employee_id] = dictionary[dictionary6[accLevelsetEmp.acclevelset_id].level_timeseg_id];
							}
							if (!dictionary8.ContainsKey(accLevelsetEmp.employee_id))
							{
								if (dictionary6.ContainsKey(accLevelsetEmp.acclevelset_id))
								{
									List<AccLevelset> list9 = new List<AccLevelset>();
									list9.Add(dictionary6[accLevelsetEmp.acclevelset_id]);
									dictionary8.Add(accLevelsetEmp.employee_id, list9);
								}
							}
							else
							{
								List<AccLevelset> list9 = dictionary8[accLevelsetEmp.employee_id];
								if (!list9.Contains(dictionary6[accLevelsetEmp.acclevelset_id]))
								{
									list9.Add(dictionary6[accLevelsetEmp.acclevelset_id]);
								}
							}
						}
						for (int num11 = 0; num11 < list2.Count; num11++)
						{
							UserInfo userInfo = list2[num11];
							ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
							objUserAuthorize.Pin = userInfo.BadgeNumber;
							num12 = 0;
							for (int num13 = 0; num13 < modelList4.Count; num13++)
							{
								num12 |= 1 << modelList4[num13].door_no - 1;
							}
							objUserAuthorize.AuthorizeDoorId = num12.ToString();
							if (dictionary7.ContainsKey(userInfo.UserId))
							{
								AccTimeseg accTimeseg = dictionary7[userInfo.UserId];
								ObjUserAuthorize objUserAuthorize2 = objUserAuthorize;
								num7 = dictionary7[userInfo.UserId].id;
								objUserAuthorize2.AuthorizeTimezoneId = num7.ToString();
								objUserAuthorize.TimeZone1 = accTimeseg.TimeZone1Id;
								objUserAuthorize.TimeZone2 = accTimeseg.TimeZone2Id;
								objUserAuthorize.TimeZone3 = accTimeseg.TimeZone3Id;
							}
							list8.Add(objUserAuthorize);
						}
					}
					else
					{
						AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
						foreach (AccLevelset item in modelList9)
						{
							modelList4 = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group where acclevelset_id =" + item.id + ") and device_id=" + machine.ID);
							List<AccLevelsetEmp> modelList10 = accLevelsetEmpBll.GetModelList("acclevelset_id =" + item.id);
							ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
							num12 = 0;
							for (int num14 = 0; num14 < modelList4.Count; num14++)
							{
								num12 |= 1 << modelList4[num14].door_no - 1;
							}
							objUserAuthorize.AuthorizeDoorId = num12.ToString();
							for (int num15 = 0; num15 < modelList10.Count; num15++)
							{
								if (dictionary4.ContainsKey(modelList10[num15].employee_id))
								{
									UserInfo userInfo = dictionary4[modelList10[num15].employee_id];
									objUserAuthorize.Pin = userInfo.BadgeNumber;
									ObjUserAuthorize objUserAuthorize3 = objUserAuthorize;
									num7 = ((item.level_timeseg_id == DeviceHelper.TimeSeg.id) ? 1 : item.level_timeseg_id);
									objUserAuthorize3.AuthorizeTimezoneId = num7.ToString();
									if (dictionary.ContainsKey(item.level_timeseg_id))
									{
										AccTimeseg accTimeseg = dictionary[item.level_timeseg_id];
										objUserAuthorize.TimeZone1 = accTimeseg.TimeZone1Id;
										objUserAuthorize.TimeZone2 = accTimeseg.TimeZone2Id;
										objUserAuthorize.TimeZone3 = accTimeseg.TimeZone3Id;
									}
									list8.Add(objUserAuthorize.Copy());
									if (list8.Count >= num3 || num15 == modelList10.Count - 1)
									{
										num = deviceServer.STD_SetUserTimeZone(list8);
										if (num >= 0)
										{
											ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetLevelsetOk", "从PC同步人员权限信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserAuthorizeInfoCount", "权限记录数") + ":" + list8.Count);
										}
										else
										{
											ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetLevelsetFalse", "从PC同步人员权限数据到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + " " + ShowMsgInfos.GetInfo("UpUserAuthorizeInfoCount", "权限记录数") + ":" + list8.Count);
											if (machine.DevSDKType == SDKType.StandaloneSDK)
											{
												return false;
											}
										}
										list8 = new List<ObjUserAuthorize>();
									}
								}
							}
						}
					}
				}
				catch (Exception ex12)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ": " + ex12.Message);
					if (machine.DevSDKType != SDKType.StandaloneSDK)
					{
						goto end_IL_247b;
					}
					return false;
					end_IL_247b:;
				}
			}
			ShowProgress(40);
			if (flag && machine.DevSDKType == SDKType.StandaloneSDK)
			{
				num = 0;
				num3 = 30;
				if (deviceServer.DevInfo.IsTFTMachine)
				{
					try
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("UploadingUserPhoto", "正在同步人员照片到设备"));
						StringBuilder stringBuilder2 = new StringBuilder();
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						for (int num16 = 0; num16 < list2.Count; num16++)
						{
							stringBuilder2.AppendFormat("{0},", list2[num16].UserId);
							if (((num16 > 0 && (num16 + 1) % num3 == 0) || num16 == list2.Count - 1) && stringBuilder2 != null && stringBuilder2.Length > 0)
							{
								List<UserInfo> modelList11 = userInfoBll.GetModelList("UserId in (" + stringBuilder2.Remove(stringBuilder2.Length - 1, 1).ToString() + ")", true);
								if (modelList11 == null || modelList11.Count <= 0)
								{
									stringBuilder2 = new StringBuilder();
									modelList11 = new List<UserInfo>();
								}
								else
								{
									num = deviceServer.STD_UploadUserPhoto(modelList11);
									if (num >= 0)
									{
										ShowUpLoadInfo(ShowMsgInfos.GetInfo("UploadUserPhotoSucceed", "同步人员照片到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + num);
									}
									else
									{
										ShowUpLoadInfo(ShowMsgInfos.GetInfo("UploadUserPhotoFailed", "同步人员照片到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
										if (machine.DevSDKType == SDKType.StandaloneSDK)
										{
											return false;
										}
									}
									stringBuilder2 = new StringBuilder();
									modelList11 = new List<UserInfo>();
								}
							}
						}
					}
					catch (Exception ex13)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("UploadUserPhotoFailed", "同步人员照片到设备失败") + ": " + ex13.Message);
						if (machine.DevSDKType != SDKType.StandaloneSDK)
						{
							goto end_IL_26c6;
						}
						return false;
						end_IL_26c6:;
					}
				}
			}
			ShowProgress(50);
			if (flag && machine.DevSDKType == SDKType.StandaloneSDK)
			{
				num = 0;
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingUserVerifyMode", "正在同步人员验证方式到设备"));
					num = deviceServer.STD_SetUserVerifyMode(list4);
					if (num >= 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserVerifyMode", "从PC同步人员验证模式到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + num);
						goto end_IL_2731;
					}
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserVerifyModeFaild", "从PC同步人员验证模式到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + (machine.BatchUpdate ? ("\t" + ShowMsgInfos.GetInfo("CancelBatchUpdateAndRetry", "可取消批量上传功能后重试")) : ""));
					return false;
					end_IL_2731:;
				}
				catch (Exception ex14)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserVerifyModeSucceed", "同步人员验证模式到设备失败") + ": " + ex14.Message);
					return false;
				}
			}
			ShowProgress(60);
			if (flag && machine.SupportFingerprint)
			{
				num = 0;
				num3 = 200;
				try
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingFingerPring", "正在同步指纹信息到设备"));
					TemplateBll templateBll = new TemplateBll(MainForm._ia);
					List<Template> modelList12 = templateBll.GetModelList("USERID in (select USERINFO.USERID from USERINFO where USERINFO.USERID in (Select employee_id from acc_levelset_emp where acclevelset_id in (select acclevelset_id from acc_levelset where acclevelset_id in (Select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + machine.ID + "))))");
					modelList12 = (modelList12 ?? new List<Template>());
					for (int num17 = 0; num17 < modelList12.Count; num17++)
					{
						Template template = modelList12[num17];
						if (dictionary4.ContainsKey(template.USERID))
						{
							template.Pin = dictionary4[template.USERID].BadgeNumber;
						}
					}
					List<Template> list10 = new List<Template>();
					for (int num18 = 0; num18 < modelList12.Count; num18++)
					{
						list10.Add(modelList12[num18]);
						if ((num18 > 0 && (num18 + 1) % num3 == 0) || num18 == modelList12.Count - 1)
						{
							num = deviceServer.STD_SetUserFPTemplate(list10);
							if (num >= 0)
							{
								ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateOk", "从PC同步指纹信息到设备成功") + " " + ShowMsgInfos.GetInfo("templateCount", "指纹数") + ": " + list10.Count);
							}
							else
							{
								ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
								if (machine.DevSDKType == SDKType.StandaloneSDK)
								{
									return false;
								}
							}
							list10 = new List<Template>();
						}
					}
					modelList12 = null;
				}
				catch (Exception ex15)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ": " + ex15.Message);
					if (machine.DevSDKType != SDKType.StandaloneSDK)
					{
						goto end_IL_2a32;
					}
					return false;
					end_IL_2a32:;
				}
			}
			ShowProgress(70);
			ShowUpLoadInfo("Verificando se possui suporte a FingerVein");
			if (flag && machine.SupportFingerVein)
			{
				ShowUpLoadInfo("Suporte a FingerVein confirmado");
				try
				{
					num3 = 198;
					StringBuilder stringBuilder3 = new StringBuilder();
					FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
					if (list2 != null)
					{
						for (int num19 = 0; num19 < list2.Count; num19++)
						{
							stringBuilder3.Append(list2[num19].UserId + ",");
							if ((num19 + 1) % num3 == 0 || num19 == list2.Count - 1)
							{
								List<FingerVein> modelList13 = fingerVeinBll.GetModelList("UserId in (" + stringBuilder3.Remove(stringBuilder3.Length - 1, 1).ToString() + ") order by UserID,FingerID");
								if (modelList13 != null && modelList13.Count > 0)
								{
									List<FingerVein> list11 = new List<FingerVein>();
									Dictionary<string, string> dictionary9 = new Dictionary<string, string>();
									for (int num20 = 0; num20 < modelList13.Count; num20++)
									{
										FingerVein fingerVein = modelList13[num20];
										if (dictionary4.ContainsKey(fingerVein.UserID))
										{
											fingerVein.Pin = dictionary4[fingerVein.UserID].BadgeNumber;
										}
										list11.Add(fingerVein);
										string text = fingerVein.UserID + "-" + fingerVein.FingerID;
										if (!dictionary9.ContainsKey(text))
										{
											dictionary9.Add(text, text);
										}
										if (num20 + 1 == modelList13.Count || (list11.Count != 0 && list11.Count % num3 == 0))
										{
											num = deviceServer.SetFvTemplate(list11);
											if (num >= 0)
											{
												ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFingerVeinOk", "从PC同步指静脉信息到设备成功") + " " + ShowMsgInfos.GetInfo("FingerVeinCount", "指静脉数") + ":" + dictionary9.Count);
											}
											else
											{
												ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFingerVeinFalse", "从PC同步指静脉信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
												if (machine.DevSDKType == SDKType.StandaloneSDK)
												{
													return false;
												}
											}
											list11.Clear();
											dictionary9.Clear();
										}
									}
								}
								stringBuilder3.Clear();
								modelList13.Clear();
							}
						}
					}
				}
				catch (Exception ex16)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFingerVeinFalse", "从PC同步指静脉信息到设备失败") + ":" + ex16.Message);
					if (machine.DevSDKType != SDKType.StandaloneSDK)
					{
						goto end_IL_2d50;
					}
					return false;
					end_IL_2d50:;
				}
			}
			ShowProgress(80);
			if (flag && machine.SupportFingerprint)
			{
				num = 0;
				num3 = 15;
				if (machine.FaceFunOn == 1)
				{
					try
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SettingFaceTemplate", "正在同步面部信息到设备"));
						StringBuilder stringBuilder4 = new StringBuilder();
						ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
						string sQLString = "select employee_id from acc_levelset_emp where employee_id in (select " + ((AppSite.Instance.DataType == DataType.SqlServer) ? "Cast(USERNO as int)" : "clng(UserNo)") + " from facetemp) and acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id = " + machine.ID + ") and employee_id in (select userid from userinfo)";
						DataSet dataSet = (AppSite.Instance.DataType == DataType.SqlServer) ? DbHelperSQL.Query(sQLString) : DbHelperOleDb.Query(sQLString);
						List<FaceTemp> modelList14;
						if (dataSet != null && dataSet.Tables.Count > 0)
						{
							DataTable dataTable = dataSet.Tables[0];
							for (int num21 = 0; num21 < dataTable.Rows.Count; num21++)
							{
								stringBuilder4.AppendFormat("{0},", dataTable.Rows[num21]["employee_id"]);
								if ((num21 > 0 && (num21 + 1) % num3 == 0) || num21 == dataTable.Rows.Count - 1)
								{
									stringBuilder4.Remove(stringBuilder4.Length - 1, 1);
									modelList14 = faceTempBll.GetModelList((AppSite.Instance.DataType == DataType.SqlServer) ? ("Cast(USERNO as int) in (" + stringBuilder4.ToString() + ")") : ("clng(USERNO) in (" + stringBuilder4.ToString() + ")"));
									num = deviceServer.STD_SetUserFaceTemplate(modelList14);
									if (num >= 0)
									{
										ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateOk", "从PC同步面部信息到设备成功") + " " + ShowMsgInfos.GetInfo("FacetemplateCount", "面部模版数") + ":" + modelList14.Count);
									}
									else
									{
										ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateFalse", "从PC同步面部信息到设备失败") + ": " + PullSDkErrorInfos.GetInfo(num));
										if (machine.DevSDKType == SDKType.StandaloneSDK)
										{
											return false;
										}
									}
									stringBuilder4 = new StringBuilder();
								}
							}
						}
						modelList14 = null;
					}
					catch (Exception ex17)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateFalse", "从PC同步面部信息到设备失败") + ": " + ex17.Message);
						if (machine.DevSDKType != SDKType.StandaloneSDK)
						{
							goto end_IL_30da;
						}
						return false;
						end_IL_30da:;
					}
				}
			}
			ShowProgress(90);
			if (machine.DevSDKType != SDKType.StandaloneSDK)
			{
				try
				{
					num = 0;
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadNormalOpenInfo", "正在从PC同步首卡开门设置信息到设备..."));
					num = deviceServer.DeleteDeviceData("FirstCard", "", "");
					if (num < 0)
					{
						ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelFirstCardFalse", "首卡开门重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
					else
					{
						List<ObjFirstCard> list12 = new List<ObjFirstCard>();
						AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
						List<AccFirstOpen> modelList15 = accFirstOpenBll.GetModelList("door_id in (select id from acc_door where  device_id=" + machine.ID + ")");
						if (modelList15 != null && modelList15.Count > 0)
						{
							AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
							foreach (AccFirstOpen item2 in modelList15)
							{
								if (dictionary2.ContainsKey(item2.door_id))
								{
									ObjFirstCard objFirstCard = new ObjFirstCard();
									ObjFirstCard objFirstCard2 = objFirstCard;
									num7 = dictionary2[item2.door_id].door_no;
									objFirstCard2.DoorID = num7.ToString();
									ObjFirstCard objFirstCard3 = objFirstCard;
									object timezoneID;
									if (DeviceHelper.TimeSeg.id != item2.timeseg_id)
									{
										num7 = item2.timeseg_id;
										timezoneID = num7.ToString();
									}
									else
									{
										timezoneID = "1";
									}
									objFirstCard3.TimezoneID = (string)timezoneID;
									List<AccFirstOpenEmp> modelList16 = accFirstOpenEmpBll.GetModelList("accfirstopen_id=" + item2.id);
									if (modelList16 != null && modelList16.Count > 0)
									{
										for (int num22 = 0; num22 < modelList16.Count; num22++)
										{
											if (dictionary4.ContainsKey(modelList16[num22].employee_id))
											{
												objFirstCard.Pin = dictionary4[modelList16[num22].employee_id].BadgeNumber;
												list12.Add(objFirstCard.Copy());
											}
										}
									}
								}
							}
						}
						num = deviceServer.SetFirstCard(list12);
						if (num >= 0)
						{
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFirstCardOk", "从PC同步首卡开门信息到设备成功"));
						}
						else
						{
							ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFirstCardFalse", "从PC同步首卡开门信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
					}
				}
				catch (Exception ex18)
				{
					ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFirstCardFalse", "从PC同步首卡开门信息到设备失败") + ":" + ex18.Message);
				}
			}
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			DevCmds devCmds = new DevCmds();
			devCmds.SN_id = machine.ID;
			devCmds.CmdTransTime = DateTime.Now;
			devCmds.CmdReturn = 0;
			devCmds.status = 2;
			devCmdsBll.UpdateEx(devCmds);
			return true;
		}

		public static int DeletePullDeviceData(Machines machine, DeviceServerBll devServer)
		{
			return DeviceHelper.DeletePullDeviceData(machine, devServer, false);
		}

		public static int DeletePullDeviceData(Machines machine, DeviceServerBll devServer, bool resetIP)
		{
			int num = DeviceHelper.m_errorCode;
			if (devServer != null)
			{
				num = DeviceHelper.DeleteFirstCard(devServer);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteHoliday(devServer);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteInOutFun(devServer);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteMultimCard(devServer);
				if (num < 0)
				{
					return num;
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (resetIP)
				{
					stringBuilder.AppendFormat("IPAddress=192.168.1.201,");
				}
				for (int i = 1; i <= machine.door_count; i++)
				{
					stringBuilder.AppendFormat("Door{0}MultiCardOpenDoor=0,", i);
					stringBuilder.AppendFormat("Door{0}KeepOpenTimeZone=0,", i);
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					devServer.SetDeviceParam(stringBuilder.ToString());
				}
				num = DeviceHelper.DeleteUserAuthorize(devServer);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteTimeZone(devServer);
				if (num < 0)
				{
					return num;
				}
				num = DeviceHelper.DeleteUserInfo(devServer);
				if (num < 0)
				{
					return num;
				}
				if (machine.SupportFingerVein)
				{
					num = DeviceHelper.DeleteUserFingerVein(devServer);
					if (num < 0)
					{
						return num;
					}
				}
				if (machine.SupportFingerprint)
				{
					num = DeviceHelper.DeleteTemplate(devServer);
					if (num < 0)
					{
						return num;
					}
				}
				if (machine.SupportFace)
				{
					ZK.Data.BLL.PullSDK.FaceTempBll faceTempBll = new ZK.Data.BLL.PullSDK.FaceTempBll(devServer.Application);
					num = faceTempBll.Delete();
				}
				DeviceParamBll deviceParamBll = new DeviceParamBll(devServer.Application);
				deviceParamBll.SetAntiPassback(0);
				deviceParamBll.SetInterLock(0);
			}
			return num;
		}
	}
}
