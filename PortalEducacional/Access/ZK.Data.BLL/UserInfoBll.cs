using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.DBUtility;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZKTimeNet.AccessDataClass;

namespace ZK.Data.BLL
{
	public class UserInfoBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static bool m_isloadall = false;

		private readonly IUserInfo dal = null;

		private static bool m_isUpdate = false;

		private static DataSet m_ds = null;

		public static bool IsUpdate
		{
			get
			{
				return UserInfoBll.m_isUpdate;
			}
			set
			{
				UserInfoBll.m_isUpdate = value;
			}
		}

		public UserInfoBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IUserInfo));
			if (service != null)
			{
				this.dal = (service as IUserInfo);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public int GetMaxBadgeNumber()
		{
			return this.dal.GetMaxBadgeNumber();
		}

		public bool Exists(int USERID)
		{
			return this.dal.Exists(USERID);
		}

		public bool ExistsBadgenumber(string number)
		{
			return this.dal.ExistsBadgenumber(number);
		}

		public bool ExistsCardNo(string cardno)
		{
			return this.dal.ExistsCardNo(cardno);
		}

		public int GetUserID(string number)
		{
			return this.dal.GetUserID(number);
		}

		public bool ExistsDept(int deptID)
		{
			return this.dal.ExistsDept(deptID);
		}

		public bool ExistsIdentitycard(string IDnumber)
		{
			return this.dal.ExistsIdentitycard(IDnumber);
		}

		private bool CheckPin(UserInfo model)
		{
			if (model.BadgeNumber == null || "" == model.BadgeNumber.Trim())
			{
				throw new ArgumentNullException("BadgeNumber", ShowMsgInfos.GetInfo("personnelNumberNotNull", "人员编号不能为空"));
			}
			if (!int.TryParse(model.BadgeNumber, out int _))
			{
				throw new ArithmeticException(ShowMsgInfos.GetInfo("PinMustBeNumber", "人员编号必须为数字"));
			}
			return true;
		}

		public int Add(UserInfo model, string visitorFlag)
		{
			if (!this.CheckPin(model))
			{
				return 0;
			}
			int num = this.dal.Add(model, visitorFlag);
			if (num > 0)
			{
				UserInfoBll.m_isUpdate = true;
				model.UserId = this.GetMaxId() - 1;
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddUserInfo", "添加人员") + model.Name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "UserInfo";
				this.actionLogBll.Add(this.actionLog);
				UserInfoBll.m_ds = null;
				AccessEmployee accessEmployee = new AccessEmployee();
				accessEmployee.emp_cardNumber = (model.CardNo ?? "");
				accessEmployee.emp_dept = model.DefaultDeptId;
				accessEmployee.emp_firstname = (model.Name ?? "");
				accessEmployee.emp_lastname = (model.LastName ?? "");
				accessEmployee.emp_pin = int.Parse(model.BadgeNumber ?? "0");
				accessEmployee.old_emp_pin = accessEmployee.emp_pin;
				accessEmployee.emp_hiredate = (model.HiredDay ?? DateTime.Now);
				ZKTimeNetLite.accessExce.CreateEntity(new List<object>
				{
					(object)accessEmployee
				}, true);
			}
			return num;
		}

		public int Add(List<UserInfo> list, string visitorFlag)
		{
			int num = 0;
			if (list != null && list.Count > 0)
			{
				UserInfoBll.m_isUpdate = true;
				num = this.dal.Add(list, visitorFlag);
				if (num >= 0)
				{
					this.actionLog.action_time = DateTime.Now;
					this.actionLog.user_id = SysInfos.SysUserInfo.id;
					this.actionLog.content_type_id = 3;
					this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddUserInfo", "添加人员");
					ActionLog obj = this.actionLog;
					obj.change_message += ":";
					for (int i = 0; i < list.Count; i++)
					{
						ActionLog obj2 = this.actionLog;
						obj2.change_message = obj2.change_message + list[i].BadgeNumber + " ";
					}
					this.actionLog.action_flag = 1;
					this.actionLog.object_repr = "UserInfo";
					this.actionLogBll.Add(this.actionLog);
					UserInfoBll.m_ds = null;
					List<object> list2 = new List<object>();
					for (int j = 0; j < list.Count; j++)
					{
						UserInfo userInfo = list[j];
						AccessEmployee accessEmployee = new AccessEmployee();
						accessEmployee.emp_cardNumber = (userInfo.CardNo ?? "");
						accessEmployee.emp_dept = userInfo.DefaultDeptId;
						accessEmployee.emp_firstname = (userInfo.Name ?? "");
						accessEmployee.emp_lastname = (userInfo.LastName ?? "");
						accessEmployee.emp_pin = int.Parse(userInfo.BadgeNumber ?? "0");
						accessEmployee.old_emp_pin = accessEmployee.emp_pin;
						accessEmployee.emp_hiredate = (userInfo.HiredDay ?? DateTime.Now);
						list2.Add(accessEmployee);
					}
					ZKTimeNetLite.accessExce.CreateEntity(list2, true);
				}
			}
			return num;
		}

		public void AddMutilGroup(int groupid, string userIDlist)
		{
			UserInfoBll.m_isUpdate = true;
			this.dal.AddMutilGroup(groupid, userIDlist);
			UserInfoBll.m_ds = null;
		}

		public void DelMutilGroup(int groupid)
		{
			UserInfoBll.m_isUpdate = true;
			this.dal.DelMutilGroup(groupid);
			UserInfoBll.m_ds = null;
		}

		public void DelMutilGroup(string userIDlist)
		{
			UserInfoBll.m_isUpdate = true;
			this.dal.DelMutilGroup(userIDlist);
			UserInfoBll.m_ds = null;
		}

		public bool Update(UserInfo model)
		{
			if (!this.CheckPin(model))
			{
				return false;
			}
			if (this.dal.Update(model))
			{
				UserInfoBll.m_isUpdate = true;
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyUserInfo", "修改人员") + model.Name;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "UserInfo";
				this.actionLogBll.Add(this.actionLog);
				UserInfoBll.m_ds = null;
				AccessEmployee accessEmployee = new AccessEmployee();
				accessEmployee.emp_cardNumber = (model.CardNo ?? "");
				accessEmployee.emp_dept = model.DefaultDeptId;
				accessEmployee.emp_firstname = (model.Name ?? "");
				accessEmployee.emp_lastname = (model.LastName ?? "");
				accessEmployee.emp_pin = int.Parse(model.BadgeNumber ?? "0");
				accessEmployee.old_emp_pin = accessEmployee.emp_pin;
				accessEmployee.emp_hiredate = (model.HiredDay ?? DateTime.Now);
				ZKTimeNetLite.accessExce.UpdateEntity(new List<object>
				{
					(object)accessEmployee
				}, false);
				return true;
			}
			return false;
		}

		public bool Update(List<UserInfo> list)
		{
			if (list != null && list.Count > 0 && this.dal.Update(list))
			{
				UserInfoBll.m_isUpdate = true;
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyUserInfo", "修改人员");
				ActionLog obj = this.actionLog;
				obj.change_message += ":";
				for (int i = 0; i < list.Count; i++)
				{
					ActionLog obj2 = this.actionLog;
					obj2.change_message = obj2.change_message + list[i].BadgeNumber + " ";
				}
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "UserInfo";
				this.actionLogBll.Add(this.actionLog);
				UserInfoBll.m_ds = null;
				List<object> list2 = new List<object>();
				for (int j = 0; j < list.Count; j++)
				{
					UserInfo userInfo = list[j];
					AccessEmployee accessEmployee = new AccessEmployee();
					accessEmployee.emp_cardNumber = (userInfo.CardNo ?? "");
					accessEmployee.emp_dept = userInfo.DefaultDeptId;
					accessEmployee.emp_firstname = (userInfo.Name ?? "");
					accessEmployee.emp_lastname = (userInfo.LastName ?? "");
					accessEmployee.emp_pin = int.Parse(userInfo.BadgeNumber ?? "0");
					accessEmployee.old_emp_pin = accessEmployee.emp_pin;
					accessEmployee.emp_hiredate = (userInfo.HiredDay ?? DateTime.Now);
					list2.Add(accessEmployee);
				}
				ZKTimeNetLite.accessExce.UpdateEntity(list2, true);
				return true;
			}
			return false;
		}

		public int UpdateFields(string FieldValues, string filter)
		{
			return this.dal.UpdateFields(FieldValues, filter);
		}

		public bool Delete(int USERID)
		{
			UserInfoBll.m_isUpdate = true;
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelUserInfo", "删除人员") + USERID;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "UserInfo";
			this.actionLogBll.Add(this.actionLog);
			UserInfoBll.m_ds = null;
			bool result = this.dal.Delete(USERID);
			UserInfo model = this.GetModel(USERID);
			if (model != null)
			{
				AccessEmployee accessEmployee = new AccessEmployee();
				accessEmployee.emp_cardNumber = (model.CardNo ?? "");
				accessEmployee.emp_dept = model.DefaultDeptId;
				accessEmployee.emp_firstname = (model.Name ?? "");
				accessEmployee.emp_lastname = (model.LastName ?? "");
				accessEmployee.emp_pin = int.Parse(model.BadgeNumber ?? "0");
				accessEmployee.old_emp_pin = accessEmployee.emp_pin;
				accessEmployee.emp_hiredate = (model.HiredDay ?? DateTime.Now);
				ZKTimeNetLite.accessExce.DeleteEntity(new List<object>
				{
					(object)accessEmployee
				}, false);
			}
			return result;
		}

		public bool Delete(UserInfo model)
		{
			bool flag = false;
			if (model != null)
			{
				UserInfoBll.m_isUpdate = true;
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelUserInfo", "删除人员") + model.BadgeNumber;
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "UserInfo";
				this.actionLogBll.Add(this.actionLog);
				UserInfoBll.m_ds = null;
				flag = this.dal.Delete(model.UserId);
				AccessEmployee accessEmployee = new AccessEmployee();
				accessEmployee.emp_cardNumber = (model.CardNo ?? "");
				accessEmployee.emp_dept = model.DefaultDeptId;
				accessEmployee.emp_firstname = (model.Name ?? "");
				accessEmployee.emp_lastname = (model.LastName ?? "");
				accessEmployee.emp_pin = int.Parse(model.BadgeNumber ?? "0");
				accessEmployee.old_emp_pin = accessEmployee.emp_pin;
				accessEmployee.emp_hiredate = (model.HiredDay ?? DateTime.Now);
				ZKTimeNetLite.accessExce.DeleteEntity(new List<object>
				{
					(object)accessEmployee
				}, false);
				return flag;
			}
			return false;
		}

		public bool DeleteList(string USERIDlist)
		{
			List<UserInfo> modelList = this.GetModelList("userid in (" + USERIDlist + ")", null, false);
			if (modelList != null)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < modelList.Count; i++)
				{
					UserInfo userInfo = modelList[i];
					AccessEmployee accessEmployee = new AccessEmployee();
					accessEmployee.emp_cardNumber = (userInfo.CardNo ?? "");
					accessEmployee.emp_dept = userInfo.DefaultDeptId;
					accessEmployee.emp_firstname = (userInfo.Name ?? "");
					accessEmployee.emp_lastname = (userInfo.LastName ?? "");
					accessEmployee.emp_pin = int.Parse(userInfo.BadgeNumber ?? "0");
					accessEmployee.old_emp_pin = accessEmployee.emp_pin;
					accessEmployee.emp_hiredate = (userInfo.HiredDay ?? DateTime.Now);
					list.Add(accessEmployee);
				}
				ZKTimeNetLite.accessExce.DeleteEntity(list, true);
			}
			if (this.dal.DeleteList(USERIDlist))
			{
				UserInfoBll.m_isUpdate = true;
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelUserInfo", "删除人员") + USERIDlist;
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "UserInfo";
				this.actionLogBll.Add(this.actionLog);
				USERIDlist = USERIDlist.Replace("'", "");
				string[] array = USERIDlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int j = 0; j < array.Length; j++)
					{
					}
				}
				UserInfoBll.m_ds = null;
				return true;
			}
			return false;
		}

		public UserInfo GetModel(int USERID)
		{
			return this.dal.GetModel(USERID);
		}

		public static void ClearDs()
		{
		}

		public void Load()
		{
			this.Load(null, false);
		}

		public void Load(string visitorFlag, bool WithPhoto)
		{
			UserInfoBll.ClearDs();
			this.GetList("", visitorFlag, WithPhoto);
		}

		public DataSet GetList(string strWhere)
		{
			return this.GetList(strWhere, null, false);
		}

		public DataSet GetList(string strWhere, bool WithPhoto)
		{
			return this.GetList(strWhere, null, false, WithPhoto);
		}

		public DataSet GetList(string strWhere, string visitoFlag, bool WithPhoto)
		{
			return this.GetList(strWhere, visitoFlag, false, WithPhoto);
		}

		public DataSet GetList(string strWhere, string visitorFlag, bool activeonly, bool WithPhoto)
		{
			DataSet result = null;
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Depts) || SysInfos.SysUserInfo.username == "admin")
				{
					result = this.dal.GetList(strWhere, visitorFlag, activeonly, WithPhoto);
				}
				else if (string.IsNullOrEmpty(strWhere))
				{
					strWhere = " DEFAULTDEPTID in " + SysInfos.Depts + "  ";
					result = this.dal.GetList(strWhere, visitorFlag, activeonly, WithPhoto);
					strWhere = string.Empty;
				}
				else
				{
					strWhere = " DEFAULTDEPTID in " + SysInfos.Depts + " and (" + strWhere + " )";
					result = this.dal.GetList(strWhere, visitorFlag, activeonly, WithPhoto);
				}
			}
			return result;
		}

		public List<UserInfo> GetModelList(string strWhere)
		{
			return this.GetModelList(strWhere, null, false);
		}

		public List<UserInfo> GetModelList(string strWhere, bool WithPhoto)
		{
			return this.GetModelList(strWhere, null, WithPhoto);
		}

		public List<UserInfo> GetModelList(string strWhere, string visitorFlag, bool WithPhoto)
		{
			List<UserInfo> list = null;
			DataSet list2 = this.GetList(strWhere, visitorFlag, WithPhoto);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
				}
			}
			return list;
		}

		public List<UserInfo> DataTableToList(DataTable dt)
		{
			List<UserInfo> list = new List<UserInfo>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					UserInfo item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetAllList(null, false);
		}

		public DataSet GetAllList(string visitorFlag, bool WithPhoto)
		{
			return this.GetList("", visitorFlag, WithPhoto);
		}

		public Dictionary<string, int> GetBadgenumber_UserIdDic(string sqlWhere)
		{
			return this.dal.GetBadgenumber_UserIdDic(sqlWhere);
		}

		public Dictionary<int, string> GetUserId_BadgenumberDic(string sqlWhere)
		{
			return this.dal.GetUserId_BadgenumberDic(sqlWhere);
		}

		public void InitTable()
		{
			UserInfoBll.m_ds = null;
			UserInfoBll.m_isloadall = false;
			this.dal.InitTable();
			ZKTimeNetLite.accessExce.ClearEmployee();
		}

		public void DeleteGroupID(int groupId)
		{
			this.dal.DeleteGroupID(groupId);
		}

		public void DeleteGroupID()
		{
			UserInfoBll.m_ds = null;
			UserInfoBll.m_isloadall = false;
			this.dal.DeleteGroupID();
		}

		public void Sync2ZKTime()
		{
			List<UserInfo> modelList = this.GetModelList("", null, false);
			if (modelList != null)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < modelList.Count; i++)
				{
					UserInfo userInfo = modelList[i];
					AccessEmployee accessEmployee = new AccessEmployee();
					accessEmployee.emp_cardNumber = (userInfo.CardNo ?? "");
					accessEmployee.emp_dept = userInfo.DefaultDeptId;
					accessEmployee.emp_firstname = (userInfo.Name ?? "");
					accessEmployee.emp_lastname = (userInfo.LastName ?? "");
					accessEmployee.emp_pin = int.Parse(userInfo.BadgeNumber ?? "0");
					accessEmployee.old_emp_pin = accessEmployee.emp_pin;
					accessEmployee.emp_hiredate = (userInfo.HiredDay ?? DateTime.Now);
					list.Add(accessEmployee);
				}
				ZKTimeNetLite.accessExce.CreateEntity(list, true);
			}
		}
	}
}
