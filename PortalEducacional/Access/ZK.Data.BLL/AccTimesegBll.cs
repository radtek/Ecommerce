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
	public class AccTimesegBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccTimeseg> m_data = new DataCollections<AccTimeseg>();

		private static bool m_isloadall = false;

		private readonly IAccTimeseg dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccTimesegBll.m_isUpdate;
			}
			set
			{
				AccTimesegBll.m_isUpdate = value;
			}
		}

		public AccTimesegBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccTimeseg));
			if (service != null)
			{
				this.dal = (service as IAccTimeseg);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool IsUsing(int id)
		{
			return this.dal.IsUsing(id);
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public bool Exists(string name)
		{
			return this.dal.Exists(name);
		}

		public int Add(AccTimeseg model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccTimesegBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddTimeZone", "添加时间段") + model.timeseg_name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccTimeseg";
				this.actionLogBll.Add(this.actionLog);
				AccTimesegBll.m_isUpdate = true;
			}
			return num;
		}

		public static DateTime GetDate(int minite)
		{
			if (minite < 0)
			{
				minite = 0;
			}
			else if (minite >= 1440)
			{
				minite = 1439;
			}
			int num = minite / 60;
			minite %= 60;
			string str = num.ToString("00") + ":" + minite.ToString("00");
			str = "2011-01-01 " + str + ":00";
			return DateTime.Parse(str);
		}

		public AccTimeseg GetdefaultTime()
		{
			List<AccTimeseg> modelList = this.GetModelList("");
			string info = ShowMsgInfos.GetInfo("SDefaultTZ", "24小时通行");
			AccTimeseg accTimeseg = null;
			int num = 0;
			if (modelList != null && modelList.Count > 0)
			{
				if (modelList.Count == 1)
				{
					return modelList[0];
				}
				for (int i = 0; i < modelList.Count; i++)
				{
					if (num == 0)
					{
						num = modelList[i].id;
					}
					else if (modelList[i].id < num)
					{
						num = modelList[i].id;
					}
					if (modelList[i].timeseg_name == info)
					{
						accTimeseg = modelList[i];
						break;
					}
				}
			}
			if (accTimeseg == null)
			{
				if (modelList != null && modelList.Count > 0)
				{
					if (modelList.Count == 1)
					{
						accTimeseg = modelList[0];
					}
					else
					{
						int num2 = 0;
						while (num2 < modelList.Count)
						{
							if (modelList[num2].id != num)
							{
								num2++;
								continue;
							}
							accTimeseg = modelList[num2];
							break;
						}
						if (accTimeseg == null)
						{
							accTimeseg = modelList[0];
						}
					}
				}
				else
				{
					accTimeseg = this.Add(info);
				}
			}
			return accTimeseg;
		}

		public AccTimeseg Add(string timeName)
		{
			AccTimeseg accTimeseg = new AccTimeseg();
			accTimeseg.timeseg_name = timeName;
			accTimeseg.memo = timeName;
			accTimeseg.monday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.monday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.monday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.monday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.monday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.monday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.tuesday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.tuesday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.tuesday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.tuesday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.tuesday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.tuesday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.wednesday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.wednesday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.wednesday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.wednesday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.wednesday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.wednesday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.thursday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.thursday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.thursday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.thursday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.thursday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.thursday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.friday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.friday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.friday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.friday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.friday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.friday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.saturday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.saturday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.saturday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.saturday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.saturday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.saturday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.sunday_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.sunday_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.sunday_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.sunday_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.sunday_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.sunday_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype1_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype1_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.holidaytype1_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype1_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype1_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype1_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype2_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype2_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.holidaytype2_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype2_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype2_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype2_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype3_start1 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype3_end1 = AccTimesegBll.GetDate(1440);
			accTimeseg.holidaytype3_start2 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype3_end2 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype3_start3 = AccTimesegBll.GetDate(0);
			accTimeseg.holidaytype3_end3 = AccTimesegBll.GetDate(0);
			accTimeseg.TimeZone1Id = 1;
			this.Add(accTimeseg);
			accTimeseg.id = this.GetMaxId() - 1;
			return accTimeseg;
		}

		public int Search(string strdata)
		{
			return this.dal.Search(strdata);
		}

		public bool Update(AccTimeseg model)
		{
			if (this.dal.Update(model))
			{
				AccTimesegBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyTimeZone", "修改时间段") + model.timeseg_name;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccTimeseg";
				this.actionLogBll.Add(this.actionLog);
				AccTimesegBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccTimesegBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelTimeZone", "删除时间段") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccTimeseg";
			this.actionLogBll.Add(this.actionLog);
			AccTimesegBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelTimeZone", "删除时间段");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccTimeseg";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccTimesegBll.m_data.Remove(array[i]);
					}
				}
				AccTimesegBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccTimeseg GetModel(int id)
		{
			AccTimeseg accTimeseg = AccTimesegBll.m_data.Get(id);
			if (accTimeseg != null)
			{
				return accTimeseg;
			}
			accTimeseg = this.dal.GetModel(id);
			if (accTimeseg != null)
			{
				AccTimesegBll.m_data.Update(id, accTimeseg);
			}
			return accTimeseg;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccTimeseg> GetModelList(string strWhere)
		{
			List<AccTimeseg> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccTimesegBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccTimeseg> DataTableToList(DataTable dt)
		{
			List<AccTimeseg> list = new List<AccTimeseg>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccTimeseg item = this.dal.DataConvert(dt.Rows[i]);
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
			AccTimesegBll.m_data.Clear();
			AccTimesegBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
