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
	public class AccLevelsetBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccLevelset> m_data = new DataCollections<AccLevelset>();

		private static bool m_isloadall = false;

		private readonly IAccLevelset dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccLevelsetBll.m_isUpdate;
			}
			set
			{
				AccLevelsetBll.m_isUpdate = value;
			}
		}

		public AccLevelsetBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccLevelset));
			if (service != null)
			{
				this.dal = (service as IAccLevelset);
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

		public bool Exists(string name)
		{
			return this.dal.Exists(name);
		}

		public int Add(AccLevelset model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccLevelsetBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddLevelset", "添加人员权限信息") + model.level_name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccLevelset";
				this.actionLogBll.Add(this.actionLog);
				AccLevelsetBll.m_isUpdate = true;
			}
			return num;
		}

		public int Search(string strdata)
		{
			return this.dal.Search(strdata);
		}

		public bool Update(AccLevelset model)
		{
			if (this.dal.Update(model))
			{
				AccLevelsetBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyLevelset", "修改人员权限信息") + model.level_name;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccLevelset";
				this.actionLogBll.Add(this.actionLog);
				AccLevelsetBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccLevelsetBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelset", "删除人员权限信息") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLevelset";
			this.actionLogBll.Add(this.actionLog);
			AccLevelsetBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelset", "删除人员权限信息");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccLevelset";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccLevelsetBll.m_data.Remove(array[i]);
					}
				}
				AccLevelsetBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccLevelset GetModel(int id)
		{
			AccLevelset accLevelset = AccLevelsetBll.m_data.Get(id);
			if (accLevelset != null)
			{
				return accLevelset;
			}
			accLevelset = this.dal.GetModel(id);
			if (accLevelset != null)
			{
				AccLevelsetBll.m_data.Update(id, accLevelset);
			}
			return accLevelset;
		}

		public DataSet GetList(string strWhere)
		{
			return this.GetList(strWhere, false);
		}

		public DataSet GetList(string strWhere, bool visitOnly)
		{
			return this.dal.GetList(strWhere, visitOnly);
		}

		public List<AccLevelset> GetModelList(string strWhere)
		{
			return this.GetModelList(strWhere, false);
		}

		public List<AccLevelset> GetModelList(string strWhere, bool visitOnly)
		{
			List<AccLevelset> list = null;
			DataSet list2 = this.dal.GetList(strWhere, visitOnly);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccLevelsetBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccLevelset> DataTableToList(DataTable dt)
		{
			List<AccLevelset> list = new List<AccLevelset>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccLevelset item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("", false);
		}

		public void InitTable()
		{
			AccLevelsetBll.m_data.Clear();
			AccLevelsetBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
