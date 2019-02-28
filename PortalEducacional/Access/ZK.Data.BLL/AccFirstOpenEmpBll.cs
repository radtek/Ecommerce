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
	public class AccFirstOpenEmpBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccFirstOpenEmp> m_data = new DataCollections<AccFirstOpenEmp>();

		private static bool m_isloadall = false;

		private readonly IAccFirstOpenEmp dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccFirstOpenEmpBll.m_isUpdate;
			}
			set
			{
				AccFirstOpenEmpBll.m_isUpdate = value;
			}
		}

		public AccFirstOpenEmpBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccFirstOpenEmp));
			if (service != null)
			{
				this.dal = (service as IAccFirstOpenEmp);
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

		public int Add(AccFirstOpenEmp model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccFirstOpenEmpBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddFirstOpenEmp", "添加首卡开门人员组") + model.accfirstopen_id;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccFirstOpenEmp";
				this.actionLogBll.Add(this.actionLog);
				AccFirstOpenEmpBll.m_isUpdate = true;
			}
			return num;
		}

		public void Add(List<AccFirstOpenEmp> list)
		{
			this.dal.Add(list);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddFirstOpenEmp", "添加首卡开门人员组");
			this.actionLog.action_flag = 1;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
		}

		public bool Update(AccFirstOpenEmp model)
		{
			if (this.dal.Update(model))
			{
				AccFirstOpenEmpBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyFirstOpenEmp", "修改首卡开门人员组") + model.accfirstopen_id;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccFirstOpenEmp";
				this.actionLogBll.Add(this.actionLog);
				AccFirstOpenEmpBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccFirstOpenEmpBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteByUserID(int uid)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组") + uid;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
			return this.dal.DeleteByUserID(uid);
		}

		public bool DeleteByOpenID(int fid)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组") + fid;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
			return this.dal.DeleteByOpenID(fid);
		}

		public bool DeleteByOpenIDAndUserList(int fid, string uidlist)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组") + fid;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
			return this.dal.DeleteByOpenIDAndUserList(fid, uidlist);
		}

		public bool DeleteByUserAndOpenID(int uid, int fid)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组") + fid;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
			return this.dal.DeleteByUserAndOpenID(uid, fid);
		}

		public bool Delete(string strWhere)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpenEmp";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenEmpBll.m_isUpdate = true;
			return this.dal.Delete(strWhere);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpenEmp", "删除首卡开门人员组");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccFirstOpenEmp";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccFirstOpenEmpBll.m_data.Remove(array[i]);
					}
				}
				AccFirstOpenEmpBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccFirstOpenEmp GetModel(int id)
		{
			AccFirstOpenEmp accFirstOpenEmp = AccFirstOpenEmpBll.m_data.Get(id);
			if (accFirstOpenEmp != null)
			{
				return accFirstOpenEmp;
			}
			accFirstOpenEmp = this.dal.GetModel(id);
			if (accFirstOpenEmp != null)
			{
				AccFirstOpenEmpBll.m_data.Update(id, accFirstOpenEmp);
			}
			return accFirstOpenEmp;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Depts) || SysInfos.SysUserInfo.username == "admin")
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" employee_id in ( select USERID from USERINFO where  DEFAULTDEPTID in " + SysInfos.Depts + " ) and (" + strWhere + " )") : (" employee_id in ( select USERID from USERINFO where  DEFAULTDEPTID in " + SysInfos.Depts + " )  "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccFirstOpenEmp> GetModelList(string strWhere)
		{
			List<AccFirstOpenEmp> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccFirstOpenEmpBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccFirstOpenEmp> DataTableToList(DataTable dt)
		{
			List<AccFirstOpenEmp> list = new List<AccFirstOpenEmp>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccFirstOpenEmp item = this.dal.DataConvert(dt.Rows[i]);
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
			AccFirstOpenEmpBll.m_data.Clear();
			AccFirstOpenEmpBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
