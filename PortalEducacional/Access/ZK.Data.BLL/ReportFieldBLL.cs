using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class ReportFieldBLL
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly IReportField dal = null;

		public ReportFieldBLL(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IReportField));
			if (service != null)
			{
				this.dal = (service as IReportField);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int Add(ReportField model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddReportField", "添加自定义报表字段") + ":" + model.TableName + "." + model.FieldName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "ReportField";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public int Add(List<ReportField> lstModel)
		{
			int num = this.dal.Add(lstModel);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddReportField", "批量添加自定义报表字段");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "ReportField";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Delete(int CRId, string TableName, string FieldName)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelReportField", "删除自定义报表字段") + ":" + TableName + "." + FieldName;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "ReportField";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(CRId, TableName, FieldName);
		}

		public bool DeleteByReportIdList(string idlist)
		{
			if (this.dal.DeleteByReportIdList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogBatchDelReportField", "批量删除自定义报表字段");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "ReportField";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public ReportField GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			string text = "CRId in (Select Id from CustomReport)";
			if (strWhere != null && strWhere.Trim() != "")
			{
				text = text + " and " + strWhere;
			}
			return this.dal.GetList(text);
		}

		public List<ReportField> GetModelList(string strWhere)
		{
			List<ReportField> list = null;
			DataSet list2 = this.GetList(strWhere);
			return this.DataTableToList(list2.Tables[0]);
		}

		public List<ReportField> DataTableToList(DataTable dt)
		{
			List<ReportField> list = new List<ReportField>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					ReportField item = this.dal.DataConvert(dt.Rows[i]);
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
