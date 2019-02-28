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
	public class AccHolidaysBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccHolidays> m_data = new DataCollections<AccHolidays>();

		private static bool m_isloadall = false;

		private readonly IAccHolidays dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccHolidaysBll.m_isUpdate;
			}
			set
			{
				AccHolidaysBll.m_isUpdate = value;
			}
		}

		public AccHolidaysBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccHolidays));
			if (service != null)
			{
				this.dal = (service as IAccHolidays);
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

		public bool Exists(string name)
		{
			return this.dal.Exists(name);
		}

		public int Add(AccHolidays model)
		{
			int num = SysInfos.SysUserInfo.id;
			model.create_operator = num.ToString();
			int num2 = this.dal.Add(model);
			if (num2 > 0)
			{
				num = this.GetMaxId() - 1;
				model.id = num.ToString();
				AccHolidaysBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddHoliday", "添加节假日") + model.holiday_name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccHolidays";
				this.actionLogBll.Add(this.actionLog);
				AccHolidaysBll.m_isUpdate = true;
			}
			return num2;
		}

		public bool Update(AccHolidays model)
		{
			if (this.dal.Update(model))
			{
				AccHolidaysBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyHoliday", "修改节假日") + model.holiday_name;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccHolidays";
				this.actionLogBll.Add(this.actionLog);
				AccHolidaysBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public int Search(string strdata)
		{
			return this.dal.Search(strdata);
		}

		public bool Delete(int id)
		{
			AccHolidaysBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelHoliday", "删除节假日") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccHolidays";
			this.actionLogBll.Add(this.actionLog);
			AccHolidaysBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelHoliday", "删除节假日");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccHolidays";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccHolidaysBll.m_data.Remove(array[i]);
					}
				}
				AccHolidaysBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccHolidays GetModel(int id)
		{
			AccHolidays accHolidays = AccHolidaysBll.m_data.Get(id);
			if (accHolidays != null)
			{
				return accHolidays;
			}
			accHolidays = this.dal.GetModel(id);
			if (accHolidays != null)
			{
				AccHolidaysBll.m_data.Update(id, accHolidays);
			}
			return accHolidays;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccHolidays> GetModelList(string strWhere)
		{
			List<AccHolidays> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccHolidaysBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccHolidays> DataTableToList(DataTable dt)
		{
			List<AccHolidays> list = new List<AccHolidays>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccHolidays item = this.dal.DataConvert(dt.Rows[i]);
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
			AccHolidaysBll.m_data.Clear();
			AccHolidaysBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
