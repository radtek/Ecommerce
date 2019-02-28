using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Data.OleDbDAL;
using ZK.Data.SqlDbDAL;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccDoorBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccDoor> m_data = new DataCollections<AccDoor>();

		private static bool m_isloadall = false;

		private readonly IAccDoor dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccDoorBll.m_isUpdate;
			}
			set
			{
				AccDoorBll.m_isUpdate = value;
			}
		}

		public AccDoorBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccDoor));
			if (service != null)
			{
				this.dal = (service as IAccDoor);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public static IAccDoor getDALObject()
		{
			if (AppSite.Instance.DataType == DataType.SqlServer)
			{
				return new ZK.Data.SqlDbDAL.AccDoorDal();
			}
			return new ZK.Data.OleDbDAL.AccDoorDal();
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public int Add(AccDoor model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccDoorBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddDoor", "添加门") + model.door_name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "door";
				this.actionLogBll.Add(this.actionLog);
				AccDoorBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccDoor model)
		{
			if (this.dal.Update(model))
			{
				AccDoorBll.m_data.Update(model.id, model);
				AccDoorBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccDoorBll.m_data.Remove(id);
			AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(this._ia);
			List<AccFirstOpen> modelList = accFirstOpenBll.GetModelList("door_id=" + id);
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					accFirstOpenBll.Delete(modelList[i].id);
				}
			}
			AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(this._ia);
			List<AccMorecardset> modelList2 = accMorecardsetBll.GetModelList("door_id=" + id);
			if (modelList2 != null)
			{
				for (int j = 0; j < modelList2.Count; j++)
				{
					accMorecardsetBll.Delete(modelList2[j].id);
				}
			}
			AccMapdoorposBll accMapdoorposBll = new AccMapdoorposBll(this._ia);
			List<AccMapdoorpos> modelList3 = accMapdoorposBll.GetModelList("map_door_id=" + id);
			if (modelList3 != null)
			{
				for (int k = 0; k < modelList3.Count; k++)
				{
					accMapdoorposBll.Delete(modelList3[k].id);
				}
			}
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelDoor", "删除门") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "door";
			this.actionLogBll.Add(this.actionLog);
			AccDoorBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = "删除门";
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "door";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccDoorBll.m_data.Remove(array[i]);
					}
				}
				AccDoorBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccDoor GetModel(int id)
		{
			AccDoor accDoor = AccDoorBll.m_data.Get(id);
			if (accDoor != null)
			{
				return accDoor;
			}
			accDoor = this.dal.GetModel(id);
			if (accDoor != null)
			{
				AccDoorBll.m_data.Update(id, accDoor);
			}
			return accDoor;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (strWhere.ToString() == "get_leve_emp_record_in_dev")
				{
					return this.dal.GetList(strWhere);
				}
				if (string.IsNullOrEmpty(SysInfos.Areas))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" device_id in (select ID from  Machines where area_id in " + SysInfos.Areas + " ) and (" + strWhere + " )") : (" device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccDoor> GetModelList(string strWhere)
		{
			List<AccDoor> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccDoorBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public void IsConnectDb(bool isConnect)
		{
			this.dal.IsConnectDb(isConnect);
		}

		public int GetListRecordCount(int device, int levelset, int employee, int newmode)
		{
			if (this.dal != null)
			{
				string strWhere = "SELECT door.id, door.device_id,  door_group.acclevelset_id, door_group.accdoor_id,levelset_emp.acclevelset_id, levelset_emp.employee_id FROM acc_door AS door, acc_levelset_door_group AS door_group, acc_levelset_emp AS levelset_emp WHERE door.id=door_group.accdoor_id And door_group.acclevelset_id=levelset_emp.acclevelset_id And door.device_id=" + device + " And levelset_emp.acclevelset_id=" + levelset + " And  levelset_emp.employee_id=" + employee + " and levelset_emp.acclevelset_id <>" + newmode;
				return this.dal.GetListCount(strWhere);
			}
			return 0;
		}

		public int GetListRecordCountEx(int device, int employee, int newmode)
		{
			if (this.dal != null)
			{
				string strWhere = "SELECT door.id, door.device_id,door_group.acclevelset_id, door_group.accdoor_id,levelset_emp.acclevelset_id, levelset_emp.employee_id FROM acc_door AS door, acc_levelset_door_group AS door_group, acc_levelset_emp AS levelset_emp WHERE door.id=door_group.accdoor_id And door_group.acclevelset_id=levelset_emp.acclevelset_id And door.device_id=" + device + " And  levelset_emp.employee_id=" + employee + " and levelset_emp.acclevelset_id <>" + newmode;
				return this.dal.GetListCount(strWhere);
			}
			return 0;
		}

		public int GetListRecordCountEx(int device, int newmode)
		{
			if (this.dal != null)
			{
				string strWhere = "SELECT door.id, door.device_id,door.door_no,door_group.acclevelset_id, door_group.accdoor_id FROM acc_door AS door,acc_levelset_door_group AS door_group WHERE door.device_id =" + device + " and door.id = door_group.accdoor_id and door_group.acclevelset_id <>" + newmode;
				return this.dal.GetListCount(strWhere);
			}
			return 0;
		}

		public int GetListRecordCountEx1(AccLevelset model, int deviceID, int employeeID, int newmodeID, int timezonesID, int TimeSegID)
		{
			if (this.dal != null)
			{
				int num = 0;
				for (int i = 0; i < model.device_door_exp.Count; i++)
				{
					string text = model.device_door_exp[i];
					string[] array = text.Split('_');
					if (deviceID == int.Parse(array[0]) && array[1] != "")
					{
						num = int.Parse(array[1]);
						string strWhere = "SELECT door_group.accdoor_device_id, door_group.acclevelset_id, door_group.accdoor_no_exp,levelset_emp.acclevelset_id, levelset_emp.employee_id,levelset.id, levelset.level_timeseg_id FROM   acc_levelset_door_group AS door_group,acc_levelset_emp AS levelset_emp,acc_levelset As levelset WHERE        door_group.accdoor_device_id =" + deviceID + " and door_group.accdoor_no_exp =" + num + " and door_group.acclevelset_id <>" + newmodeID + " and door_group.acclevelset_id = levelset.id and levelset_emp.employee_id =" + employeeID + " and levelset_emp.acclevelset_id =door_group.acclevelset_id and levelset_emp.acclevelset_id =levelset.id and levelset.level_timeseg_id =" + timezonesID;
						return this.dal.GetListCount(strWhere);
					}
				}
			}
			return 0;
		}

		public List<AccDoor> DataTableToList(DataTable dt)
		{
			List<AccDoor> list = new List<AccDoor>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccDoor item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public void InitTable()
		{
			AccDoorBll.m_data.Clear();
			AccDoorBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
