using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.DBUtility;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;
using ZKTimeNet.AccessDataClass;

namespace ZK.Data.BLL
{
	public class DepartmentsBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<Departments> m_data = new DataCollections<Departments>();

		private static bool m_isloadall = false;

		private readonly IDepartments dal = null;

		public DepartmentsBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IDepartments));
			if (service != null)
			{
				this.dal = (service as IDepartments);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int DEPTID)
		{
			return this.dal.Exists(DEPTID);
		}

		public bool ExistsDeptName(string name)
		{
			return this.dal.ExistsDeptName(name);
		}

		public bool ExistsCode(string code)
		{
			return this.dal.ExistsCode(code);
		}

		public int Add(Departments model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num >= 0)
			{
				model.DEPTID = this.GetMaxId() - 1;
				DepartmentsBll.m_data.Add(model.DEPTID, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddDept", "添加部门") + model.DEPTNAME;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Departments";
				this.actionLogBll.Add(this.actionLog);
				AccessDepartment accessDepartment = new AccessDepartment();
				accessDepartment.dept_code = model.DEPTID;
				accessDepartment.old_dept_code = model.DEPTID;
				accessDepartment.dept_name = model.DEPTNAME;
				accessDepartment.dept_parentcode = model.SUPDEPTID;
				ZKTimeNetLite.accessExce.CreateEntity(new List<object>
				{
					(object)accessDepartment
				}, true);
			}
			return num;
		}

		public bool Update(Departments model)
		{
			if (this.dal.Update(model))
			{
				DepartmentsBll.m_data.Update(model.DEPTID, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyDept", "修改部门") + model.DEPTNAME;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "Departments";
				this.actionLogBll.Add(this.actionLog);
				AccessDepartment accessDepartment = new AccessDepartment();
				accessDepartment.dept_code = model.DEPTID;
				accessDepartment.old_dept_code = model.DEPTID;
				accessDepartment.dept_name = model.DEPTNAME;
				accessDepartment.dept_parentcode = model.SUPDEPTID;
				ZKTimeNetLite.accessExce.UpdateEntity(new List<object>
				{
					(object)accessDepartment
				}, false);
				return true;
			}
			return false;
		}

		public bool Delete(int DEPTID)
		{
			if (!this.Exists(DEPTID))
			{
				return true;
			}
			Departments model = this.GetModel(DEPTID);
			if (model != null)
			{
				AccessDepartment accessDepartment = new AccessDepartment();
				accessDepartment.dept_code = model.DEPTID;
				accessDepartment.old_dept_code = model.DEPTID;
				accessDepartment.dept_name = model.DEPTNAME;
				accessDepartment.dept_parentcode = model.SUPDEPTID;
				ZKTimeNetLite.accessExce.DeleteEntity(new List<object>
				{
					(object)accessDepartment
				}, false);
			}
			DepartmentsBll.m_data.Remove(DEPTID);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			string dptCode = this.dal.GetDptCode(DEPTID);
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelDept", "删除部门") + dptCode;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Departments";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(DEPTID);
		}

		public bool DeleteList(string DEPTIDlist)
		{
			List<Departments> modelList = this.GetModelList("id in (" + DEPTIDlist + ")");
			if (modelList != null)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < modelList.Count; i++)
				{
					Departments departments = modelList[i];
					AccessDepartment accessDepartment = new AccessDepartment();
					accessDepartment.dept_code = departments.DEPTID;
					accessDepartment.old_dept_code = departments.DEPTID;
					accessDepartment.dept_name = departments.DEPTNAME;
					accessDepartment.dept_parentcode = departments.SUPDEPTID;
					list.Add(accessDepartment);
				}
				ZKTimeNetLite.accessExce.DeleteEntity(list, true);
			}
			if (this.dal.DeleteList(DEPTIDlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelDept", "删除部门");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "Departments";
				this.actionLogBll.Add(this.actionLog);
				DEPTIDlist = DEPTIDlist.Replace("'", "");
				string[] array = DEPTIDlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int j = 0; j < array.Length; j++)
					{
						DepartmentsBll.m_data.Remove(array[j]);
					}
				}
				return true;
			}
			return false;
		}

		public Departments GetModel(int DEPTID)
		{
			Departments departments = DepartmentsBll.m_data.Get(DEPTID);
			if (departments != null)
			{
				return departments;
			}
			departments = this.dal.GetModel(DEPTID);
			if (departments != null)
			{
				DepartmentsBll.m_data.Update(DEPTID, departments);
			}
			return departments;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Depts) || SysInfos.SysUserInfo.username == "admin")
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" DEPTID in " + SysInfos.Depts + " and (" + strWhere + " )") : (" DEPTID in " + SysInfos.Depts + "  "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<Departments> GetModelList(string strWhere)
		{
			List<Departments> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					DepartmentsBll.m_data.Update(list[i].DEPTID, list[i]);
				}
			}
			return list;
		}

		public List<Departments> DataTableToList(DataTable dt)
		{
			List<Departments> list = new List<Departments>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					Departments item = this.dal.DataConvert(dt.Rows[i]);
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
			DepartmentsBll.m_data.Clear();
			DepartmentsBll.m_isloadall = false;
			this.dal.InitTable();
			ZKTimeNetLite.accessExce.ClearDepartment();
		}

		public void Sync2ZKTime()
		{
			List<Departments> modelList = this.GetModelList("");
			if (modelList != null)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < modelList.Count; i++)
				{
					Departments departments = modelList[i];
					AccessDepartment accessDepartment = new AccessDepartment();
					accessDepartment.dept_code = departments.DEPTID;
					accessDepartment.old_dept_code = departments.DEPTID;
					accessDepartment.dept_name = departments.DEPTNAME;
					accessDepartment.dept_parentcode = departments.SUPDEPTID;
					list.Add(accessDepartment);
				}
				ZKTimeNetLite.accessExce.CreateEntity(list, true);
			}
		}
	}
}
