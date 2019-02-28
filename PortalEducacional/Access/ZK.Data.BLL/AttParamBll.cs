using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class AttParamBll
	{
		private IApplication _ia = null;

		private readonly IAttParam dal = null;

		public AttParamBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAttParam));
			if (service != null)
			{
				this.dal = (service as IAttParam);
			}
		}

		public bool Exists(string PARANAME)
		{
			return this.dal.Exists(PARANAME);
		}

		public void Add(AttParam model)
		{
			this.dal.Add(model);
		}

		public bool Update(AttParam model)
		{
			return this.dal.Update(model);
		}

		public bool Delete(string PARANAME)
		{
			return this.dal.Delete(PARANAME);
		}

		public bool DeleteList(string PARANAMElist)
		{
			return this.dal.DeleteList(PARANAMElist);
		}

		public AttParam GetModel(string PARANAME)
		{
			return this.dal.GetModel(PARANAME);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AttParam> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<AttParam> DataTableToList(DataTable dt)
		{
			List<AttParam> list = new List<AttParam>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AttParam item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}
	}
}
