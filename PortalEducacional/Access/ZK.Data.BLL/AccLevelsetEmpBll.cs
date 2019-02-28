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
	public class AccLevelsetEmpBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccLevelsetEmp> m_data = new DataCollections<AccLevelsetEmp>();

		private static bool m_isloadall = false;

		private readonly IAccLevelsetEmp dal = null;

		public AccLevelsetEmpBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccLevelsetEmp));
			if (service != null)
			{
				this.dal = (service as IAccLevelsetEmp);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public void Add(List<AccLevelsetEmp> models)
		{
			this.dal.Add(models);
		}

		public void AddEx(AccLevelsetEmp model)
		{
			this.dal.AddEx(model);
		}

		public bool AccLevelsetEmp_Addnew(List<AccLevelsetEmp> models)
		{
			return this.dal.AccLevelsetEmp_Addnew(models);
		}

		public int Add(AccLevelsetEmp model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccLevelsetEmpBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddLevelsetEmp", "添加人员权限信息");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccLevelsetEmp";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(AccLevelsetEmp model)
		{
			if (this.dal.Update(model))
			{
				AccLevelsetEmpBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyLevelsetEmp", "修改人员权限信息");
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccLevelsetEmp";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public bool DeleteEmployee(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetEmp", "删除人员权限信息");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLevelsetEmp";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteEmployee(id);
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetEmp", "删除人员权限信息");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLevelsetEmp";
			this.actionLogBll.Add(this.actionLog);
			AccLevelsetEmpBll.m_data.Remove(id);
			return this.dal.Delete(id);
		}

		public bool DeleteByAcclevelsetID(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetEmp", "删除人员权限信息");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLevelsetEmp";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteByAcclevelsetID(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetEmp", "删除人员权限信息");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccLevelsetEmp";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccLevelsetEmpBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public AccLevelsetEmp GetModel(int id)
		{
			AccLevelsetEmp accLevelsetEmp = AccLevelsetEmpBll.m_data.Get(id);
			if (accLevelsetEmp != null)
			{
				return accLevelsetEmp;
			}
			accLevelsetEmp = this.dal.GetModel(id);
			if (accLevelsetEmp != null)
			{
				AccLevelsetEmpBll.m_data.Update(id, accLevelsetEmp);
			}
			return accLevelsetEmp;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccLevelsetEmp> GetModelList(string strWhere)
		{
			List<AccLevelsetEmp> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccLevelsetEmpBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccLevelsetEmp> DataTableToList(DataTable dt)
		{
			List<AccLevelsetEmp> list = new List<AccLevelsetEmp>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccLevelsetEmp item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public Dictionary<int, List<int>> GetUserLevelsetIdDic()
		{
			return this.dal.GetUserLevelsetIdDic();
		}

		public void InitTable()
		{
			AccLevelsetEmpBll.m_data.Clear();
			AccLevelsetEmpBll.m_isloadall = false;
			this.dal.InitTable();
		}

		public void IsConnectDb(bool isConnect)
		{
			this.dal.IsConnectDb(isConnect);
		}
	}
}
