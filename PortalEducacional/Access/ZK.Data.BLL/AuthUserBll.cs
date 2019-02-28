using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class AuthUserBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly IAuthUser dal = null;

		public AuthUserBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAuthUser));
			if (service != null)
			{
				this.dal = (service as IAuthUser);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool ExistsRoleID(int roleId)
		{
			return this.dal.ExistsRoleID(roleId);
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public bool Exists(string userName)
		{
			return this.dal.Exists(userName);
		}

		public int Add(AuthUser model)
		{
			int num = this.dal.Add(model);
			if (num >= 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddUser", "添加用户") + model.username;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AuthUser";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(AuthUser model)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyUser", "修改用户") + model.username;
			this.actionLog.action_flag = 3;
			this.actionLog.object_repr = "AuthUser";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Update(model);
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelUser", "删除用户") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AuthUser";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelUser", "删除用户");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AuthUser";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteList(idlist);
		}

		public AuthUser GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public AuthUser GetModel(string userName)
		{
			return this.dal.GetModel(userName);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AuthUser> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<AuthUser> DataTableToList(DataTable dt)
		{
			List<AuthUser> list = new List<AuthUser>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AuthUser item = this.dal.DataConvert(dt.Rows[i]);
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
