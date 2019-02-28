using System;
using ZK.Data.IDAL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Framework;

namespace ZK.Data.BLL.PullSDK
{
	public class DeviceParamBll
	{
		private IApplication _ia = null;

		private int m_errorNo = -999;

		private readonly IDeviceParam dal = null;

		public DeviceParamBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IDeviceParam));
			if (service != null)
			{
				this.dal = (service as IDeviceParam);
			}
		}

		public int GetBaseParam(ObjDeviceParam model)
		{
			if (this.dal != null)
			{
				return this.dal.GetBaseParam(model);
			}
			return this.m_errorNo;
		}

		public int GetDoorInfo(ObjDoorInfo model)
		{
			if (this.dal != null)
			{
				return this.dal.GetDoorInfo(model);
			}
			return this.m_errorNo;
		}

		public int SetDoorParam(ObjDoorInfo door, Machines dev)
		{
			if (this.dal != null)
			{
				return this.dal.SetDoorParam(door, dev);
			}
			return this.m_errorNo;
		}

		public int SetMultimCard(DoorType dtype, bool isOpen)
		{
			if (this.dal != null)
			{
				return this.dal.SetMultimCard(dtype, isOpen);
			}
			return this.m_errorNo;
		}

		public int SetFirstCard(DoorType dtype, bool isOpen)
		{
			if (this.dal != null)
			{
				return this.dal.SetFirstCard(dtype, isOpen);
			}
			return this.m_errorNo;
		}

		public int SetKeepOpenTimeZone(DoorType dtype, int TimeID)
		{
			if (this.dal != null)
			{
				return this.dal.SetKeepOpenTimeZone(dtype, TimeID);
			}
			return this.m_errorNo;
		}

		public int SetValidTZ(DoorType dtype, int TimeID)
		{
			if (this.dal != null)
			{
				return this.dal.SetValidTZ(dtype, TimeID);
			}
			return this.m_errorNo;
		}

		public int SetForcePassWord(DoorType dtype, string pwd)
		{
			if (this.dal != null)
			{
				return this.dal.SetForcePassWord(dtype, pwd);
			}
			return this.m_errorNo;
		}

		public int SetSupperPassWord(DoorType dtype, string pwd)
		{
			if (this.dal != null)
			{
				return this.dal.SetSupperPassWord(dtype, pwd);
			}
			return this.m_errorNo;
		}

		public int SetCommunication(string comPwd, string iPAddress, string gATEIPAddress, string netMask)
		{
			if (this.dal != null)
			{
				return this.dal.SetCommunication(comPwd, iPAddress, gATEIPAddress, netMask);
			}
			return this.m_errorNo;
		}

		public int SetCommunication(ObjDeviceParam param)
		{
			if (this.dal != null)
			{
				return this.dal.SetCommunication(param);
			}
			return this.m_errorNo;
		}

		public int SetAntiPassback(int passbackType)
		{
			if (this.dal != null)
			{
				return this.dal.SetAntiPassback(passbackType);
			}
			return this.m_errorNo;
		}

		public int SetInterLock(int interLock)
		{
			if (this.dal != null)
			{
				return this.dal.SetInterLock(interLock);
			}
			return this.m_errorNo;
		}

		public int SetDateTime(DateTime dt)
		{
			if (this.dal != null)
			{
				return this.dal.SetDateTime(dt);
			}
			return this.m_errorNo;
		}

		public int SetBackupTime(int hour)
		{
			if (this.dal != null)
			{
				return this.dal.SetBackupTime(hour);
			}
			return this.m_errorNo;
		}

		public int SetWatchDog(bool isOpen)
		{
			if (this.dal != null)
			{
				return this.dal.SetWatchDog(isOpen);
			}
			return this.m_errorNo;
		}

		public int SetDoor4ToDoor2(bool isOpen)
		{
			if (this.dal != null)
			{
				return this.dal.SetDoor4ToDoor2(isOpen);
			}
			return this.m_errorNo;
		}

		public int SetBaudrate(int newBaudrate)
		{
			if (this.dal != null)
			{
				return this.dal.SetBaudrate(newBaudrate);
			}
			return this.m_errorNo;
		}

		public int SetPasswd(string passwd)
		{
			if (this.dal != null)
			{
				return this.dal.SetPasswd(passwd);
			}
			return this.m_errorNo;
		}

		public int SetFPThreshold(int FPThreshold)
		{
			if (this.dal != null)
			{
				return this.dal.SetFPThreshold(FPThreshold);
			}
			return this.m_errorNo;
		}

		public int GetParamsValues(string ParamNames, out string Values)
		{
			Values = "";
			if (this.dal != null)
			{
				return this.dal.GetParamsValues(ParamNames, out Values);
			}
			return this.m_errorNo;
		}
	}
}
