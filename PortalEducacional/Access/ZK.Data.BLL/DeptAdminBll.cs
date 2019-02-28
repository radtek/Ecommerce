using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class DeptAdminBll
	{
		private IApplication _ia = null;

		private readonly IDeptAdmin dal = null;

		public DeptAdminBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IDeptAdmin));
			if (service != null)
			{
				this.dal = (service as IDeptAdmin);
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

		public int Add(DeptAdmin model)
		{
			return this.dal.Add(model);
		}

		public bool Update(DeptAdmin model)
		{
			return this.dal.Update(model);
		}

		public bool Delete(int id)
		{
			return this.dal.Delete(id);
		}

		public bool DeleteByUserID(int id)
		{
			return this.dal.DeleteByUserID(id);
		}

		public bool DeleteByDeptID(int id)
		{
			return this.dal.DeleteByDeptID(id);
		}

		public bool DeleteList(string idlist)
		{
			return this.dal.DeleteList(idlist);
		}

		public DeptAdmin GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<DeptAdmin> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<DeptAdmin> DataTableToList(DataTable dt)
		{
			List<DeptAdmin> list = new List<DeptAdmin>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					DeptAdmin item = this.dal.DataConvert(dt.Rows[i]);
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
