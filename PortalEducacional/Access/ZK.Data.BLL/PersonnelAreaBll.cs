using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class PersonnelAreaBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly IPersonnelArea dal = null;

		public PersonnelAreaBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IPersonnelArea));
			if (service != null)
			{
				this.dal = (service as IPersonnelArea);
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

		public bool ExistsAreaID(string areaID)
		{
			return this.dal.ExistsAreaID(areaID);
		}

		public bool ExistsAreaName(string areaName)
		{
			return this.dal.ExistsAreaName(areaName);
		}

		public int Add(PersonnelArea model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num >= 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddArea", "添加区域") + model.areaname;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "PersonnelArea";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(PersonnelArea model)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyArea", "修改区域") + model.areaname;
			this.actionLog.action_flag = 3;
			this.actionLog.object_repr = "PersonnelArea";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Update(model);
		}

		public bool DeleteByAreaId(string areaid)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelArea", "删除区域") + areaid;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "PersonnelArea";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteByAreaId(areaid);
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelArea", "删除区域") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "PersonnelArea";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelArea", "删除区域");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "PersonnelArea";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteList(idlist);
		}

		public PersonnelArea GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas) || SysInfos.SysUserInfo.username == "admin")
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" id in " + SysInfos.Areas + " and (" + strWhere + " )") : (" id in " + SysInfos.Areas + "  "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<PersonnelArea> GetModelList(string strWhere)
		{
			DataSet list = this.GetList(strWhere);
			if (list != null)
			{
				return this.DataTableToList(list.Tables[0]);
			}
			return null;
		}

		public List<PersonnelArea> DataTableToList(DataTable dt)
		{
			List<PersonnelArea> list = new List<PersonnelArea>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					PersonnelArea item = this.dal.DataConvert(dt.Rows[i]);
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
