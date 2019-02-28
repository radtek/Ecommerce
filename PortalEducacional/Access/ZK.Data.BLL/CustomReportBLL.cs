using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class CustomReportBLL
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly ICustomReport dal = null;

		public CustomReportBLL(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(ICustomReport));
			if (service != null)
			{
				this.dal = (service as ICustomReport);
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

		public int Add(CustomReport model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.Id = this.GetMaxId() - 1;
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddCustomReport", "添加自定义报表") + ":" + model.ReportName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "CustomReport";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public int Add(List<CustomReport> lstModel)
		{
			int num = this.dal.Add(lstModel);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddCustomReport", "批量添加自定义报表");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "CustomReport";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(CustomReport model)
		{
			if (this.dal.Update(model))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogUpdateCustomReport", "更新自定义报表") + ":" + model.ReportName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "CustomReport";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelCustomReport", "删除自定义报表") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "CustomReport";
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
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogBatchDelCustomReport", "批量删除自定义报表");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "CustomReport";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public CustomReport GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<CustomReport> GetModelList(string strWhere)
		{
			List<CustomReport> list = null;
			DataSet list2 = this.GetList(strWhere);
			return this.DataTableToList(list2.Tables[0]);
		}

		public List<CustomReport> DataTableToList(DataTable dt)
		{
			List<CustomReport> list = new List<CustomReport>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					CustomReport item = this.dal.DataConvert(dt.Rows[i]);
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
