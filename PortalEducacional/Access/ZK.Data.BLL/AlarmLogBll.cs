using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class AlarmLogBll
	{
		private IApplication _ia = null;

		private readonly IAlarmLog dal = null;

		public AlarmLogBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAlarmLog));
			if (service != null)
			{
				this.dal = (service as IAlarmLog);
			}
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int ID)
		{
			return this.dal.Exists(ID);
		}

		public int Add(AlarmLog model)
		{
			return this.dal.Add(model);
		}

		public bool Update(AlarmLog model)
		{
			return this.dal.Update(model);
		}

		public bool Delete(int ID)
		{
			return this.dal.Delete(ID);
		}

		public bool DeleteList(string IDlist)
		{
			return this.dal.DeleteList(IDlist);
		}

		public AlarmLog GetModel(int ID)
		{
			return this.dal.GetModel(ID);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AlarmLog> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<AlarmLog> DataTableToList(DataTable dt)
		{
			List<AlarmLog> list = new List<AlarmLog>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AlarmLog item = this.dal.DataConvert(dt.Rows[i]);
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
