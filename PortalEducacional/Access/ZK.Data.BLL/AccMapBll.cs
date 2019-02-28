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
	public class AccMapBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccMap> m_data = new DataCollections<AccMap>();

		private static bool m_isloadall = false;

		private readonly IAccMap dal = null;

		public AccMapBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccMap));
			if (service != null)
			{
				this.dal = (service as IAccMap);
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

		public int Add(AccMap model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccMapBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddMap", "添加电子地图") + model.map_name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccMap";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(AccMap model)
		{
			if (this.dal.Update(model))
			{
				AccMapBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyMap", "修改电子地图") + model.map_name;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccMap";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccMapBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelMap", "删除电子地图") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccMap";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelMap", "删除电子地图");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccMap";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccMapBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public AccMap GetModel(int id)
		{
			AccMap accMap = AccMapBll.m_data.Get(id);
			if (accMap != null)
			{
				return accMap;
			}
			accMap = this.dal.GetModel(id);
			if (accMap != null)
			{
				AccMapBll.m_data.Update(id, accMap);
			}
			return accMap;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccMap> GetModelList(string strWhere)
		{
			List<AccMap> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccMapBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccMap> DataTableToList(DataTable dt)
		{
			List<AccMap> list = new List<AccMap>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccMap item = this.dal.DataConvert(dt.Rows[i]);
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
			AccMapBll.m_data.Clear();
			AccMapBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
