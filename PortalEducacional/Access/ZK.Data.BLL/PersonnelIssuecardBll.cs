using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class PersonnelIssuecardBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly IPersonnelIssuecard dal = null;

		public PersonnelIssuecardBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IPersonnelIssuecard));
			if (service != null)
			{
				this.dal = (service as IPersonnelIssuecard);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool ExistsCard(string cardNO)
		{
			return this.dal.ExistsCard(cardNO);
		}

		public int Search(int userID)
		{
			return this.dal.Search(userID);
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public int Add(PersonnelIssuecard model)
		{
			int num = 0;
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			UserInfoBll.ClearDs();
			num = this.dal.Add(model);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SAddUserCard", "人员发卡") + model.cardno;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "IssueCard";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public int Add(List<PersonnelIssuecard> modelList)
		{
			int num = 0;
			num = this.dal.Add(modelList);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SAddUserCard", "人员发卡");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "IssueCard";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public int AddEx(List<PersonnelIssuecard> modelList)
		{
			int num = 0;
			num = this.dal.AddEx(modelList);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SAddUserCard", "人员发卡");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "IssueCard";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(PersonnelIssuecard model)
		{
			bool flag = false;
			UserInfoBll.ClearDs();
			flag = this.dal.Update(model);
			if (flag)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SModifyUserCard", "修改人员卡号") + model.cardno;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "IssueCard";
				this.actionLogBll.Add(this.actionLog);
			}
			return flag;
		}

		public bool DeleteByUserId(int userID)
		{
			UserInfoBll.ClearDs();
			return this.dal.DeleteByUserId(userID);
		}

		public bool Delete(int id)
		{
			bool flag = false;
			UserInfoBll.ClearDs();
			flag = this.dal.Delete(id);
			if (flag)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SDisabledUserCard", "注销卡");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "IssueCard";
				this.actionLogBll.Add(this.actionLog);
			}
			return flag;
		}

		public bool DeleteList(string idlist)
		{
			UserInfoBll.ClearDs();
			return this.dal.DeleteList(idlist);
		}

		public bool DeleteListByUserIDs(string idlist)
		{
			UserInfoBll.ClearDs();
			return this.dal.DeleteListByUserIDs(idlist);
		}

		public PersonnelIssuecard GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Depts) || SysInfos.SysUserInfo.username == "admin")
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" DEFAULTDEPTID in " + SysInfos.Depts + " and (" + strWhere + " )") : (" DEFAULTDEPTID in " + SysInfos.Depts + "  "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<PersonnelIssuecard> GetModelList(string strWhere)
		{
			DataSet list = this.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<PersonnelIssuecard> DataTableToList(DataTable dt)
		{
			List<PersonnelIssuecard> list = new List<PersonnelIssuecard>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					PersonnelIssuecard item = this.dal.DataConvert(dt.Rows[i]);
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
