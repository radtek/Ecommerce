using System;
using System.Collections.Generic;
using ZK.ConfigManager;
using ZK.Data.IDAL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Data.Model.StdSDK;
using ZK.Framework;

namespace ZK.Data.BLL.PullSDK
{
	public class DeviceServerBll
	{
		public delegate void RTLogEventHangdle(ObjDevice sender, ObjRTLogInfo info);

		private int m_index = -1;

		private int m_missCount = 0;

		public bool[] IsAlarm = new bool[4];

		private DateTime m_LastConnectTime = DateTime.Now;

		private bool m_isAddOk = true;

		private bool m_isRun = false;

		private bool m_isDel = false;

		private bool m_isLock = false;

		private int errorCode = -999;

		private ObjDevice m_dev = null;

		private bool m_isInitDevice = false;

		private DateTime m_lastListienDate = DateTime.Now.AddSeconds(-10.0);

		private bool m_isFirst = true;

		private bool m_isReConnected = false;

		private bool m_isListen = true;

		private IApplication _ia = null;

		private readonly IDeviceServer dal = null;

		public int Index
		{
			get
			{
				return this.m_index;
			}
			set
			{
				this.m_index = value;
			}
		}

		public int MissCount
		{
			get
			{
				return this.m_missCount;
			}
			set
			{
				this.m_missCount = value;
			}
		}

		public DateTime LastConnectTime
		{
			get
			{
				return this.m_LastConnectTime;
			}
			set
			{
				this.m_LastConnectTime = value;
			}
		}

		public bool IsAddOk
		{
			get
			{
				return this.m_isAddOk;
			}
			set
			{
				this.m_isAddOk = value;
			}
		}

		public bool IsRun
		{
			get
			{
				return this.m_isRun;
			}
			set
			{
				this.m_isRun = value;
			}
		}

		public bool IsDel
		{
			get
			{
				return this.m_isDel;
			}
			set
			{
				this.m_isDel = value;
			}
		}

		public bool IsLock
		{
			get
			{
				return this.m_isLock;
			}
			set
			{
				this.m_isLock = value;
			}
		}

		public bool IsInitDevice => this.m_isInitDevice;

		public ObjDevice DevInfo => this.m_dev;

		public DateTime LastListienDate
		{
			get
			{
				return this.m_lastListienDate;
			}
			set
			{
				this.m_lastListienDate = value;
			}
		}

		public bool IsFirst
		{
			get
			{
				return this.m_isFirst;
			}
			set
			{
				this.m_isFirst = value;
			}
		}

		public bool IsReConnected
		{
			get
			{
				return this.m_isReConnected;
			}
			set
			{
				this.m_isReConnected = value;
			}
		}

		public int ReConectCount
		{
			get;
			set;
		}

		public bool IsNeedListen
		{
			get
			{
				return this.m_isListen;
			}
			set
			{
				if (value != this.m_isListen)
				{
					this.m_isListen = value;
					if (!value)
					{
						this.m_isFirst = true;
					}
					if (this.ListenChangedEvent != null)
					{
						this.ListenChangedEvent(this, null);
					}
				}
			}
		}

		public bool IsHaveListener
		{
			get
			{
				if (this.RTLogEvent != null || this.DoorStateEvent != null || this.CardNoRegEvent != null || this.IsMonitoringSwippingCard() || !this.Std_IsRTEventNull)
				{
					return true;
				}
				return false;
			}
		}

		public IApplication Application => this._ia;

		public bool IsConnected
		{
			get
			{
				if (this.dal != null)
				{
					return this.dal.IsConnected;
				}
				return false;
			}
		}

		public TimeSpan LastSetDeviceTime
		{
			get;
			set;
		}

		public bool Std_IsRTEventNull => this.dal.Std_IsRTEventNull;

		public object Monitor
		{
			get;
			set;
		}

		public event EventHandler ListenChangedEvent = null;

		public event RTLogEventHangdle RTLogEvent;

		public event RTLogEventHangdle DoorStateEvent;

		public event RTLogEventHangdle CardNoRegEvent;

		public event EventHandler DeleteDoorEvent = null;

		public event DeviceRTEventHandler RTEvent
		{
			add
			{
				this.dal.RTEvent += value;
			}
			remove
			{
				this.dal.RTEvent -= value;
			}
		}

		public event DeviceRTEventHandler SwippingCard
		{
			add
			{
				this.dal.SwippingCard += value;
			}
			remove
			{
				this.dal.SwippingCard -= value;
			}
		}

		public event STD_FingerFeature FingerFeature
		{
			add
			{
				this.dal.FingerFeature += value;
			}
			remove
			{
				this.dal.FingerFeature -= value;
			}
		}

		public event STD_OnEnrollFinger OnEnrollFinger
		{
			add
			{
				this.dal.OnEnrollFinger += value;
			}
			remove
			{
				this.dal.OnEnrollFinger -= value;
			}
		}

		public void OnDeleteDoorEvent(Machines model)
		{
			if (this.DeleteDoorEvent != null)
			{
				this.DeleteDoorEvent(model, null);
			}
		}

		public void OnRTLogEvent(ObjRTLogInfo info)
		{
			if (this.RTLogEvent != null && info.EType != EventType.Type302 && info.EType != EventType.Type300 && info.EType != EventType.Type301 && info.EType != EventType.Type255)
			{
				this.RTLogEvent(this.DevInfo, info);
			}
			if (this.DoorStateEvent != null && (info.EType == EventType.Type302 || info.EType == EventType.Type300 || info.EType == EventType.Type301 || info.EType == EventType.Type220 || info.EType == EventType.Type220 || info.EType == EventType.Type101 || info.EType == EventType.Type102 || info.EType == EventType.Type103 || info.EType == EventType.Type255 || (info.EType == EventType.Type51 && AccCommon.CodeVersion == CodeVersionType.JapanAF) || info.EType == EventType.AntibackAlarm || info.EType == EventType.DeviceBroken || info.EType == EventType.DoorClosed || info.EType == EventType.DoorNotClosedOrOpened || info.EType == EventType.DoorOpenedByButton || info.EType == EventType.DoorUnexpectedOpened || info.EType == EventType.DuressAlarm || info.EType == EventType.VerifyTimesOut || info.EType == EventType.LinkageAlarm))
			{
				this.DoorStateEvent(this.DevInfo, info);
			}
			else if (this.CardNoRegEvent != null && (info.EType == EventType.Type27 || info.EType == EventType.Type1027))
			{
				this.CardNoRegEvent(this.DevInfo, info);
			}
		}

		public DeviceServerBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IDeviceServer));
			if (service != null)
			{
				this.dal = (service as IDeviceServer);
			}
		}

		public bool InitDevice(ObjDevice dev)
		{
			if (dev != null)
			{
				this.m_dev = dev;
				if (this.dal != null)
				{
					this.m_isInitDevice = this.dal.InitDevice(dev);
					if (this.dal.IsConnected)
					{
						ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
						objRTLogInfo.DevID = this.DevInfo.ID;
						objRTLogInfo.EType = EventType.Type301;
						objRTLogInfo.DoorID = "0";
						this.OnRTLogEvent(objRTLogInfo);
					}
					return this.m_isInitDevice;
				}
			}
			this.m_isInitDevice = false;
			return false;
		}

		public int Connect(int timeout = 3000)
		{
			if (this.dal != null)
			{
				int num = this.dal.Connect(timeout);
				if (num >= 0 && this.dal.IsConnected)
				{
					this.ReConectCount = 0;
					if (this.DevInfo != null)
					{
						ObjRTLogInfo objRTLogInfo = new ObjRTLogInfo();
						objRTLogInfo.DevID = this.DevInfo.ID;
						objRTLogInfo.DoorID = "0";
						objRTLogInfo.EType = EventType.Type301;
						objRTLogInfo.StatusInfo = ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功");
						if (this.DoorStateEvent != null)
						{
							this.DoorStateEvent(this.DevInfo, objRTLogInfo);
						}
					}
				}
				else
				{
					this.ReConectCount++;
					if (this.DevInfo != null && num < 0)
					{
						ObjRTLogInfo objRTLogInfo2 = new ObjRTLogInfo();
						objRTLogInfo2.DevID = this.DevInfo.ID;
						objRTLogInfo2.DoorID = "0";
						objRTLogInfo2.EType = EventType.Type300;
						objRTLogInfo2.StatusInfo = PullSDkErrorInfos.GetInfo(num);
						if (this.DoorStateEvent != null)
						{
							this.DoorStateEvent(this.DevInfo, objRTLogInfo2);
						}
					}
				}
				return num;
			}
			return -1;
		}

		public int RebootDevice()
		{
			if (this.dal != null)
			{
				return this.dal.RebootDevice();
			}
			return this.errorCode;
		}

		public int OpenDoor(DoorType doorType)
		{
			if (this.dal != null)
			{
				return this.dal.OpenDoor(doorType);
			}
			return this.errorCode;
		}

		public int OpenDoor(DoorType doorType, int time)
		{
			if (this.dal != null)
			{
				return this.dal.OpenDoor(doorType, time);
			}
			return this.errorCode;
		}

		public int CloseDoor(DoorType doorType)
		{
			if (this.dal != null)
			{
				return this.dal.CloseDoor(doorType);
			}
			return this.errorCode;
		}

		public int CancelAlarm()
		{
			if (this.dal != null)
			{
				return this.dal.CancelAlarm();
			}
			return this.errorCode;
		}

		public int NormalOpenDoor(DoorType doorType, bool state)
		{
			if (this.dal != null)
			{
				return this.dal.NormalOpenDoor(doorType, state);
			}
			return this.errorCode;
		}

		public int Disconnect()
		{
			if (this.dal != null && this.dal.IsConnected)
			{
				return this.dal.Disconnect();
			}
			return this.errorCode;
		}

		public int SetDeviceParam(string itemValues)
		{
			if (this.dal != null)
			{
				return this.dal.SetDeviceParam(itemValues);
			}
			return this.errorCode;
		}

		public int GetDeviceParam(ref byte buffer, int bufferSize, string itemValues)
		{
			if (this.dal != null)
			{
				return this.dal.GetDeviceParam(ref buffer, bufferSize, itemValues);
			}
			return this.errorCode;
		}

		public int ControlDevice(int operationID, int param1, int param2, int param3, int param4, string options)
		{
			if (this.dal != null)
			{
				return this.dal.ControlDevice(operationID, param1, param2, param3, param4, options);
			}
			return this.errorCode;
		}

		public int SetDeviceData(string tableName, string data, string options)
		{
			if (this.dal != null)
			{
				return this.dal.SetDeviceData(tableName, data, options);
			}
			return this.errorCode;
		}

		public int GetDeviceData(ref byte buffer, int bufferSize, string tableName, string fileName, string filter, string options)
		{
			if (this.dal != null)
			{
				return this.dal.GetDeviceData(ref buffer, bufferSize, tableName, fileName, filter, options);
			}
			return this.errorCode;
		}

		public int GetDeviceDataCount(string tableName, string filter, string options)
		{
			if (this.dal != null)
			{
				return this.dal.GetDeviceDataCount(tableName, filter, options);
			}
			return this.errorCode;
		}

		public int DeleteDeviceData(string tableName, string filter, string options)
		{
			if (this.dal != null)
			{
				return this.dal.DeleteDeviceData(tableName, filter, options);
			}
			return this.errorCode;
		}

		public int GetRTLog(ref byte buffer, int bufferSize)
		{
			if (this.dal != null)
			{
				return this.dal.GetRTLog(ref buffer, bufferSize);
			}
			return this.errorCode;
		}

		public List<ObjRTLogInfo> GetRTLogs(ref int errorNo)
		{
			if (this.dal != null)
			{
				return this.dal.GetRTLogs(ref errorNo);
			}
			return null;
		}

		public int SearchDevice(string commType, string address, ref byte buffer)
		{
			if (this.dal != null)
			{
				return this.dal.SearchDevice(commType, address, ref buffer);
			}
			return this.errorCode;
		}

		public List<ObjMachine> SearchDeviceEx(string commType, string address)
		{
			if (this.dal != null)
			{
				return this.dal.SearchDeviceEx(commType, address);
			}
			return null;
		}

		public int ModifyIPAddress(string commType, string address, string buffer)
		{
			if (this.dal != null)
			{
				return this.dal.ModifyIPAddress(commType, address, buffer);
			}
			return this.errorCode;
		}

		public int SetDeviceFileData(string fileName, ref byte buffer, int bufferSize, string options)
		{
			if (this.dal != null)
			{
				return this.dal.SetDeviceFileData(fileName, ref buffer, bufferSize, options);
			}
			return this.errorCode;
		}

		public int GetDeviceFileData(ref byte buffer, ref int bufferSize, string fileName, string options)
		{
			if (this.dal != null)
			{
				return this.dal.GetDeviceFileData(ref buffer, ref bufferSize, fileName, options);
			}
			return this.errorCode;
		}

		public int CloseAuxiliary(int auxiliaryID)
		{
			if (this.dal != null)
			{
				return this.dal.CloseAuxiliary(auxiliaryID);
			}
			return this.errorCode;
		}

		public int UpdateFirmware(string FileName)
		{
			if (this.dal != null)
			{
				return this.dal.STD_UpdateFirmware(FileName);
			}
			return this.errorCode;
		}

		public int GetOptionValue(string OptionNames, out string OptionValues)
		{
			if (this.dal != null)
			{
				return this.dal.GetOptionValue(OptionNames, out OptionValues);
			}
			OptionValues = "";
			return this.errorCode;
		}

		public string GetSDKVersion()
		{
			return this.dal.GetSDKVersion();
		}

		public int STD_SetUserInfo(List<UserInfo> lstUser, Dictionary<int, int> dicPullGroupId_StdGroupId)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetUserInfo(lstUser, dicPullGroupId_StdGroupId);
			}
			return this.errorCode;
		}

		public int STD_SetUserFPTemplate(List<Template> lstTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetUserFPTemplate(lstTemplate);
			}
			return this.errorCode;
		}

		public int SetFvTemplate(List<FingerVein> lstFvTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.SetFvTemplate(lstFvTemplate);
			}
			return this.errorCode;
		}

		public int STD_SetUserFaceTemplate(List<FaceTemp> lstTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetUserFaceTemplate(lstTemplate);
			}
			return this.errorCode;
		}

		public int STD_ClearUser()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ClearUser();
			}
			return this.errorCode;
		}

		public int STD_ClearUserFPTemplate()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ClearUserFPTemplate();
			}
			return this.errorCode;
		}

		public int ClearFvTemplate()
		{
			if (this.dal != null)
			{
				return this.dal.ClearFvTemplate();
			}
			return this.errorCode;
		}

		public int STD_ClearUserFaceTemplate()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ClearUserFaceTemplate();
			}
			return this.errorCode;
		}

		public int SetInterlock(List<AccInterlock> lstInterlock)
		{
			string deviceParam = (lstInterlock != null && lstInterlock.Count > 0) ? ("InterLock=" + lstInterlock[0].InterlockType) : "InterLock=0";
			return this.SetDeviceParam(deviceParam);
		}

		public int SetFirstCard(List<ObjFirstCard> lstFirstCard)
		{
			if (this.dal != null)
			{
				return this.dal.SetFirstCard(lstFirstCard);
			}
			return this.errorCode;
		}

		public int SetLinkage(List<AccLinkAgeIo> lstLinkage)
		{
			if (this.dal != null)
			{
				return this.dal.SetLinkage(lstLinkage);
			}
			return this.errorCode;
		}

		public int STD_SetAntiback(List<AccAntiback> lstAntiback)
		{
			int num = 0;
			SDKType devSDKType = this.DevInfo.DevSDKType;
			if (devSDKType == SDKType.StandaloneSDK)
			{
				if (this.dal != null)
				{
					if (lstAntiback != null && lstAntiback.Count > 0)
					{
						for (int i = 0; i < lstAntiback.Count; i++)
						{
							num = this.dal.STD_SetSysOption("AntiPassbackOn", lstAntiback[i].AntibackType.ToString());
							if (num < 0)
							{
								return num;
							}
						}
					}
					else
					{
						num = this.dal.STD_SetSysOption("AntiPassbackOn", "0");
						if (num < 0)
						{
							return num;
						}
					}
					return num;
				}
				return this.errorCode;
			}
			string deviceParam = (lstAntiback != null && lstAntiback.Count > 0) ? ("AntiPassback=" + lstAntiback[0].AntibackType) : "AntiPassback=0";
			return this.SetDeviceParam(deviceParam);
		}

		public bool IsMonitoringSwippingCard()
		{
			return this.dal.IsMonitoringSwippingCard();
		}

		public int STD_GetDeviceParam(Machines objDevParam)
		{
			if (this.dal != null)
			{
				return this.dal.STD_GetDeviceParam(objDevParam);
			}
			return this.errorCode;
		}

		public int STD_SetDeviceParam(Machines objDevParam)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetDeviceParam(objDevParam);
			}
			return this.errorCode;
		}

		public int STD_GetDoorParam(AccDoor objDoorParam)
		{
			if (this.dal != null)
			{
				return this.dal.STD_GetDoorParam(objDoorParam);
			}
			return this.errorCode;
		}

		public int STD_SetDeviceTime(DateTime? dateTime = default(DateTime?))
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetDeviceTime(dateTime);
			}
			return this.errorCode;
		}

		public int STD_SetDoorParam(AccDoor objDoorParam, int DefaultTimeZoneId)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetDoorParam(objDoorParam, DefaultTimeZoneId);
			}
			return this.errorCode;
		}

		public int STD_GetAllTransaction(out List<ObjTransAction> lstTransaction)
		{
			if (this.dal != null)
			{
				return this.dal.STD_GetAllTransaction(out lstTransaction);
			}
			lstTransaction = new List<ObjTransAction>();
			return this.errorCode;
		}

		public int STD_GetAllUserInfo(out List<UserInfo> lstUser)
		{
			if (this.dal != null)
			{
				return this.dal.STD_GetAllUserInfo(out lstUser);
			}
			lstUser = new List<UserInfo>();
			return this.errorCode;
		}

		public int STD_GetAllUserFPTemplate(out List<Template> lstTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.STD_GetAllUserTemplate(out lstTemplate);
			}
			lstTemplate = new List<Template>();
			return this.errorCode;
		}

		public int STD_GetAllUserFaceTemplate(out List<FaceTemp> lstFaceTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.STD_GetAllUserFaceTemplate(out lstFaceTemplate);
			}
			lstFaceTemplate = new List<FaceTemp>();
			return this.errorCode;
		}

		public int STD_DeleteUserInfo(List<UserInfo> lstUser)
		{
			if (this.dal != null)
			{
				return this.dal.STD_DeleteUserInfo(lstUser);
			}
			return this.errorCode;
		}

		public int STD_DeleteUserFPTemplate(List<Template> lstTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.STD_DeleteUserFPTemplate(lstTemplate);
			}
			return this.errorCode;
		}

		public int STD_DeleteUserFaceTemplate(List<FaceTemp> lstFaceTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.STD_DeleteUserFaceTemplate(lstFaceTemplate);
			}
			return this.errorCode;
		}

		public int STD_SetUserTimeZone(List<ObjUserAuthorize> lstAuthorize)
		{
			if (this.dal != null)
			{
				return this.dal.SetUserAutorize(lstAuthorize);
			}
			return this.errorCode;
		}

		public int STD_SetTimeZone(List<AccTimeseg> lstTimeseg, int DefaultTimesegId)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetTimeZone(lstTimeseg, DefaultTimesegId);
			}
			return this.errorCode;
		}

		public int STD_SetHoliday(List<AccHolidays> lstHoliday)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetHoliday(lstHoliday);
			}
			return this.errorCode;
		}

		public int STD_SetUserGroup(List<UserInfo> lstUser, Dictionary<int, int> dicMCGroupID_GroupId)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetUserGroup(lstUser, dicMCGroupID_GroupId);
			}
			return this.errorCode;
		}

		public int STD_InitializeDeviceData(bool reset = false)
		{
			if (this.dal != null)
			{
				return this.dal.STD_InitializeDeviceData(reset);
			}
			return this.errorCode;
		}

		public int STD_SetIpAddress(string IP)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetIpAddress(IP);
			}
			return this.errorCode;
		}

		public int STD_SetSubnetMask(string SubnetMask, bool EffectiveImmediately = true)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetSubnetMask(SubnetMask, EffectiveImmediately);
			}
			return this.errorCode;
		}

		public int STD_SetGateWay(string GateWay, bool EffectiveImmediately = true)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetGateWay(GateWay, EffectiveImmediately);
			}
			return this.errorCode;
		}

		public int STD_SetDeviceCommPwd(int Pwd)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetDeviceCommPwd(Pwd);
			}
			return this.errorCode;
		}

		public int STD_GetRecordCount(MachineDataStatusCode code, out int count)
		{
			count = 0;
			if (this.dal != null)
			{
				return this.dal.STD_GetRecordCount(code, out count);
			}
			return this.errorCode;
		}

		public int STD_SetDeviceInfo(DeviceInfoCode code, int value)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetDeviceInfo(code, value);
			}
			return this.errorCode;
		}

		public int STD_StartMonitor()
		{
			if (this.dal != null)
			{
				this.dal.STD_StartMonitor();
				return 0;
			}
			return this.errorCode;
		}

		public int STD_StopMonitor()
		{
			if (this.dal != null)
			{
				this.dal.STD_StopMonitor();
				return 0;
			}
			return this.errorCode;
		}

		public int STD_GetDoorState(out int StateCode)
		{
			StateCode = 0;
			if (this.dal != null)
			{
				return this.dal.STD_GetDoorState(out StateCode);
			}
			return this.errorCode;
		}

		public int STD_SetUserVerifyMode(List<UserVerifyType> lstUserVT)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetUserVerifyMode(lstUserVT);
			}
			return this.errorCode;
		}

		public int STD_GetWiegandFmt(out STD_WiegandFmt WiegandFmt)
		{
			WiegandFmt = null;
			if (this.dal != null)
			{
				return this.dal.STD_GetWiegandFmt(out WiegandFmt);
			}
			return this.errorCode;
		}

		public int STD_SetWiegandFmt(STD_WiegandFmt WiegandFmt)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetWiegandFmt(WiegandFmt);
			}
			return this.errorCode;
		}

		public int STD_SetGroup(List<Group> lstGroup)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetGroup(lstGroup);
			}
			return this.errorCode;
		}

		public int STD_SetUnlockGroup(List<ObjMultimCard> lstMultiGroup)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetUnlockGroup(lstMultiGroup);
			}
			return this.errorCode;
		}

		public int STD_ShutdownDevice()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ShutdownDevice();
			}
			return this.errorCode;
		}

		public int STD_ClearAdministrators()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ClearAdministrators();
			}
			return this.errorCode;
		}

		public int STD_ClearKeeperData()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ClearKeeperData();
			}
			return this.errorCode;
		}

		public int STD_ClearGLog()
		{
			if (this.dal != null)
			{
				return this.dal.STD_ClearGLog();
			}
			return this.errorCode;
		}

		public int STD_SetSysOption(string OptName, string OptValue)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetSysOption(OptName, OptValue);
			}
			return this.errorCode;
		}

		public int STD_GetSysOption(string OptName, out string OptValue)
		{
			OptValue = "";
			if (this.dal != null)
			{
				return this.dal.STD_GetSysOption(OptName, out OptValue);
			}
			return this.errorCode;
		}

		private void value(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
		{
			throw new NotImplementedException();
		}

		public int STD_StartEnroll(string Pin, int FingerId, int Flag = 1)
		{
			if (this.dal != null)
			{
				return this.dal.STD_StartEnroll(Pin, FingerId, Flag);
			}
			return this.errorCode;
		}

		public int STD_StartIdentify()
		{
			if (this.dal != null)
			{
				return this.dal.STD_StartIdentify();
			}
			return this.errorCode;
		}

		public int STD_CancelOperation()
		{
			if (this.dal != null)
			{
				return this.dal.STD_CancelOperation();
			}
			return this.errorCode;
		}

		public int STD_GetUserFpTemplate(int Pin, int FingerId, out Template Template)
		{
			Template = null;
			if (this.dal != null)
			{
				return this.dal.STD_GetUserFpTemplate(Pin, FingerId, out Template);
			}
			return this.errorCode;
		}

		public int STD_SetOpenTimeZone(int TZId)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetOpenTimeZone(TZId);
			}
			return this.errorCode;
		}

		public int STD_SetCloseTimeZone(int TZId)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SetCloseTimeZone(TZId);
			}
			return this.errorCode;
		}

		public int STD_SendFile(string FileName, int EnabledTimeOut = 600)
		{
			if (this.dal != null)
			{
				return this.dal.STD_SendFile(FileName, EnabledTimeOut);
			}
			return this.errorCode;
		}

		public int STD_GetFirmwareVersion(out string FirmwareVer)
		{
			FirmwareVer = "";
			if (this.dal != null)
			{
				return this.dal.STD_GetFirmwareVersion(out FirmwareVer);
			}
			return this.errorCode;
		}

		public int STD_UploadUserPhoto(List<UserInfo> lstUser)
		{
			if (this.dal != null)
			{
				return this.dal.STD_UploadUserPhoto(lstUser);
			}
			return this.errorCode;
		}
	}
}
