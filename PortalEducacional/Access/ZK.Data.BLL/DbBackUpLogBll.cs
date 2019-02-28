using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class DbBackUpLogBll
	{
		private IApplication _ia = null;

		private readonly IDbBackUpLog dal = null;

		public DbBackUpLogBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IDbBackUpLog));
			if (service != null)
			{
				this.dal = (service as IDbBackUpLog);
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

		public int Add(DbBackUpLog model)
		{
			return this.dal.Add(model);
		}

		public bool Update(DbBackUpLog model)
		{
			return this.dal.Update(model);
		}

		public bool Delete(int id)
		{
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			return this.dal.DeleteList(idlist);
		}

		public DbBackUpLog GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<DbBackUpLog> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<DbBackUpLog> DataTableToList(DataTable dt)
		{
			List<DbBackUpLog> list = new List<DbBackUpLog>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					DbBackUpLog item = this.dal.DataConvert(dt.Rows[i]);
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
