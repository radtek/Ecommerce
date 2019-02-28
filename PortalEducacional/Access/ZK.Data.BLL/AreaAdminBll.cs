using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class AreaAdminBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly IAreaAdmin dal = null;

		public AreaAdminBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAreaAdmin));
			if (service != null)
			{
				this.dal = (service as IAreaAdmin);
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

		public int Add(AreaAdmin model)
		{
			int num = this.dal.Add(model);
			if (num >= 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = "添加区域" + model.area_id;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AreaAdmin";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(AreaAdmin model)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = "修改区域" + model.area_id;
			this.actionLog.action_flag = 3;
			this.actionLog.object_repr = "AreaAdmin";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Update(model);
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = "删除区域" + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AreaAdmin";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(id);
		}

		public bool DeleteByUserID(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = "删除区域" + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AreaAdmin";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteByUserID(id);
		}

		public bool DeleteByAreaID(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = "删除区域" + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AreaAdmin";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteByAreaID(id);
		}

		public bool DeleteList(string idlist)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = "删除区域";
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AreaAdmin";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteList(idlist);
		}

		public AreaAdmin GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AreaAdmin> GetModelList(string strWhere)
		{
			DataSet list = this.GetList(strWhere);
			if (list != null)
			{
				return this.DataTableToList(list.Tables[0]);
			}
			return null;
		}

		public List<AreaAdmin> DataTableToList(DataTable dt)
		{
			List<AreaAdmin> list = new List<AreaAdmin>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AreaAdmin item = this.dal.DataConvert(dt.Rows[i]);
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
			this.dal.InitTable();
		}
	}
}
