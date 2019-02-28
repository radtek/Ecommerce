using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccMapdoorposBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccMapdoorpos> m_data = new DataCollections<AccMapdoorpos>();

		private static bool m_isloadall = false;

		private readonly IAccMapdoorpos dal = null;

		public AccMapdoorposBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccMapdoorpos));
			if (service != null)
			{
				this.dal = (service as IAccMapdoorpos);
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

		public int Add(AccMapdoorpos model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccMapdoorposBll.m_data.Add(model.id, model);
			}
			return num;
		}

		public bool Update(AccMapdoorpos model)
		{
			if (this.dal.Update(model))
			{
				AccMapdoorposBll.m_data.Update(model.id, model);
				return true;
			}
			return false;
		}

		public bool DeleteByDoorID(int id)
		{
			return this.dal.DeleteByDoorID(id);
		}

		public bool DeleteByMapID(int id)
		{
			return this.dal.DeleteByMapID(id);
		}

		public bool Delete(int id)
		{
			AccMapdoorposBll.m_data.Remove(id);
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
						AccMapdoorposBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public AccMapdoorpos GetModel(int id)
		{
			AccMapdoorpos accMapdoorpos = AccMapdoorposBll.m_data.Get(id);
			if (accMapdoorpos != null)
			{
				return accMapdoorpos;
			}
			accMapdoorpos = this.dal.GetModel(id);
			if (accMapdoorpos != null)
			{
				AccMapdoorposBll.m_data.Update(id, accMapdoorpos);
			}
			return accMapdoorpos;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccMapdoorpos> GetModelList(string strWhere)
		{
			List<AccMapdoorpos> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccMapdoorposBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccMapdoorpos> DataTableToList(DataTable dt)
		{
			List<AccMapdoorpos> list = new List<AccMapdoorpos>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccMapdoorpos item = this.dal.DataConvert(dt.Rows[i]);
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
			AccMapdoorposBll.m_data.Clear();
			AccMapdoorposBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
