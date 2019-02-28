using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class MachinesBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<Machines> m_data = new DataCollections<Machines>();

		private static bool m_isloadall = false;

		private readonly IMachines dal = null;

		public MachinesBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IMachines));
			if (service != null)
			{
				this.dal = (service as IMachines);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int ID)
		{
			return this.dal.Exists(ID);
		}

		public int MachineCount(string strWhere)
		{
			return this.dal.MachineCount(strWhere);
		}

		public bool ExistsDevice(string device, string filter = "")
		{
			return this.dal.ExistsDevice(device, filter);
		}

		public bool ExistsIP(string ip, string filter = "")
		{
			return this.dal.ExistsIP(ip, filter);
		}

		public bool ExistsRS485(int rs485, string filter = "")
		{
			return this.dal.ExistsRS485(rs485, filter);
		}

		public bool ExistsRS485(int rs485, int serialPort, string filter = "")
		{
			return this.dal.ExistsRS485(rs485, serialPort, filter);
		}

		public bool ExistsArea(string areaID)
		{
			return this.dal.ExistsArea(areaID);
		}

		public int Add(Machines model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num >= 0)
			{
				model.ID = this.GetMaxId() - 1;
				MachinesBll.m_data.Add(model.ID, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddMachine", "添加设备") + model.MachineAlias;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Machines";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(Machines model)
		{
			if (this.dal.Update(model))
			{
				MachinesBll.m_data.Update(model.ID, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyMachine", "修改设备") + model.MachineAlias;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "Machines";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public bool Delete(int ID)
		{
			MachinesBll.m_data.Remove(ID);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelMachine", "删除设备") + ID;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Machines";
			this.actionLogBll.Add(this.actionLog);
			DevCmdsBll devCmdsBll = new DevCmdsBll(this._ia);
			devCmdsBll.DeleteByMachineId(ID.ToString());
			return this.dal.Delete(ID);
		}

		public bool Delete(Machines model)
		{
			MachinesBll.m_data.Remove(model.ID);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelMachine", "删除设备") + model.MachineAlias;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Machines";
			this.actionLogBll.Add(this.actionLog);
			DevCmdsBll devCmdsBll = new DevCmdsBll(this._ia);
			devCmdsBll.DeleteByMachineId(model.ID.ToString());
			return this.dal.Delete(model.ID);
		}

		public bool DeleteList(string IDlist)
		{
			if (this.dal.DeleteList(IDlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelMachine", "删除设备");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "Machines";
				this.actionLogBll.Add(this.actionLog);
				IDlist = IDlist.Replace("'", "");
				string[] array = IDlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						MachinesBll.m_data.Remove(array[i]);
					}
				}
				DevCmdsBll devCmdsBll = new DevCmdsBll(this._ia);
				devCmdsBll.DeleteByMachineId(IDlist);
				return true;
			}
			return false;
		}

		public Machines GetModel(int ID)
		{
			Machines machines = MachinesBll.m_data.Get(ID);
			if (machines != null)
			{
				return machines;
			}
			machines = this.dal.GetModel(ID);
			if (machines != null)
			{
				MachinesBll.m_data.Update(ID, machines);
			}
			return machines;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? ("  area_id in " + SysInfos.Areas + " and (" + strWhere + " )") : ("  area_id in " + SysInfos.Areas + " "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<Machines> GetModelList(string strWhere)
		{
			List<Machines> list = null;
			string empty = string.Empty;
			empty = ((AccCommon.IsECardTong != 1) ? strWhere : ((!string.IsNullOrEmpty(strWhere)) ? (strWhere + " and " + $" AccFun = {255} ") : $" AccFun = {255} "));
			DataSet list2 = this.GetList(empty);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					MachinesBll.m_data.Update(list[i].ID, list[i]);
				}
			}
			return list;
		}

		public List<Machines> DataTableToList(DataTable dt)
		{
			List<Machines> list = new List<Machines>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					Machines item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public Machines DataConvert(DataRow dr)
		{
			return this.dal.DataConvert(dr);
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public void InitTable()
		{
			MachinesBll.m_data.Clear();
			MachinesBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
