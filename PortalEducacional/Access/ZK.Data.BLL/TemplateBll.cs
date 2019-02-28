using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class TemplateBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private readonly ITemplate dal = null;

		public TemplateBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(ITemplate));
			if (service != null)
			{
				this.dal = (service as ITemplate);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int TEMPLATEID)
		{
			return this.dal.Exists(TEMPLATEID);
		}

		public int Add(Template model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				UserInfoBll.ClearDs();
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddTemplate", "登记指纹");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Template";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public void Add(List<Template> list)
		{
			UserInfoBll.ClearDs();
			this.dal.Add(list);
		}

		public bool Update(Template model)
		{
			return this.dal.Update(model);
		}

		public void Update(List<Template> list)
		{
			this.dal.Update(list);
		}

		public void Update(List<Template> list, int FpVersion)
		{
			this.dal.Update(list, FpVersion);
		}

		public bool Delete(int TEMPLATEID)
		{
			UserInfoBll.ClearDs();
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelTemplate", "删除指纹");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Template";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(TEMPLATEID);
		}

		public bool DeleteList(string TEMPLATEIDlist)
		{
			UserInfoBll.ClearDs();
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelTemplate", "删除指纹");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Template";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.DeleteList(TEMPLATEIDlist);
		}

		public Template GetModel(int TEMPLATEID)
		{
			return this.dal.GetModel(TEMPLATEID);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<Template> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<Template> DataTableToList(DataTable dt)
		{
			List<Template> list = new List<Template>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					Template item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public Dictionary<int, int> GetUserId_TemplateCountDic(string Filter = "not TEMPLATE4 is null")
		{
			return this.dal.GetUserId_TemplateCountDic(Filter);
		}

		public DataTable GetFields(string Fields, string strWhere)
		{
			return this.dal.GetFields(Fields, strWhere);
		}

		public void InitTable()
		{
			this.dal.InitTable();
		}
	}
}
