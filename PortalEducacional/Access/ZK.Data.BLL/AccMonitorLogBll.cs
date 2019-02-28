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
	public class AccMonitorLogBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccMonitorLog> m_data = new DataCollections<AccMonitorLog>();

		private static bool m_isloadall = false;

		private readonly IAccMonitorLog dal = null;

		public AccMonitorLogBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccMonitorLog));
			if (service != null)
			{
				this.dal = (service as IAccMonitorLog);
			}
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public DateTime getLastTimeByDeviceId(int device_id)
		{
			return this.dal.getLastTimeByDeviceId(device_id);
		}

		public int Add(AccMonitorLog model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccMonitorLogBll.m_data.Add(model.id, model);
			}
			return num;
		}

		public int Add(List<AccMonitorLog> list)
		{
			return this.dal.Add(list);
		}

		public bool Update(AccMonitorLog model)
		{
			if (this.dal.Update(model))
			{
				AccMonitorLogBll.m_data.Update(model.id, model);
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccMonitorLogBll.m_data.Remove(id);
			return this.dal.Delete(id);
		}

		public bool DeleteAll()
		{
			return this.dal.DeleteAll();
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccMonitorLogBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public int Delete(string where)
		{
			return this.dal.Delete(where);
		}

		public AccMonitorLog GetModel(int id)
		{
			AccMonitorLog accMonitorLog = AccMonitorLogBll.m_data.Get(id);
			if (accMonitorLog != null)
			{
				return accMonitorLog;
			}
			accMonitorLog = this.dal.GetModel(id);
			if (accMonitorLog != null)
			{
				AccMonitorLogBll.m_data.Update(id, accMonitorLog);
			}
			return accMonitorLog;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) and " + strWhere + " ") : (" device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccMonitorLog> GetModelList(string strWhere)
		{
			List<AccMonitorLog> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccMonitorLogBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccMonitorLog> DataTableToList(DataTable dt)
		{
			List<AccMonitorLog> list = new List<AccMonitorLog>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccMonitorLog item = this.dal.DataConvert(dt.Rows[i]);
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
			AccMonitorLogBll.m_data.Clear();
			AccMonitorLogBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
