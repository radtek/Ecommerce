using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccWiegandfmtBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccWiegandfmt> m_data = new DataCollections<AccWiegandfmt>();

		private static bool m_isloadall = false;

		private readonly IAccWiegandfmt dal = null;

		public AccWiegandfmtBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccWiegandfmt));
			if (service != null)
			{
				this.dal = (service as IAccWiegandfmt);
			}
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(string wiegand_name)
		{
			return this.dal.Exists(wiegand_name);
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public int Add(AccWiegandfmt model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccWiegandfmtBll.m_data.Add(model.id, model);
			}
			return num;
		}

		public bool Update(AccWiegandfmt model)
		{
			if (this.dal.Update(model))
			{
				AccWiegandfmtBll.m_data.Update(model.id, model);
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccWiegandfmtBll.m_data.Remove(id);
			return this.dal.Delete(id);
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
						AccWiegandfmtBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public AccWiegandfmt GetModel(int id)
		{
			AccWiegandfmt accWiegandfmt = AccWiegandfmtBll.m_data.Get(id);
			if (accWiegandfmt != null)
			{
				return accWiegandfmt;
			}
			accWiegandfmt = this.dal.GetModel(id);
			if (accWiegandfmt != null)
			{
				AccWiegandfmtBll.m_data.Update(id, accWiegandfmt);
			}
			return accWiegandfmt;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccWiegandfmt> GetModelList(string strWhere)
		{
			List<AccWiegandfmt> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccWiegandfmtBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccWiegandfmt> DataTableToList(DataTable dt)
		{
			List<AccWiegandfmt> list = new List<AccWiegandfmt>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccWiegandfmt item = this.dal.DataConvert(dt.Rows[i]);
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
			AccWiegandfmtBll.m_data.Clear();
			AccWiegandfmtBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
