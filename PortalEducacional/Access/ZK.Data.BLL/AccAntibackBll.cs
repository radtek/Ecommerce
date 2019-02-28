using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccAntibackBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccAntiback> m_data = new DataCollections<AccAntiback>();

		private static bool m_isloadall = false;

		private readonly IAccAntiback dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccAntibackBll.m_isUpdate;
			}
			set
			{
				AccAntibackBll.m_isUpdate = value;
			}
		}

		public AccAntibackBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccAntiback));
			if (service != null)
			{
				this.dal = (service as IAccAntiback);
			}
		}

		public int GetMaxId()
		{
			if (this.dal != null)
			{
				return this.dal.GetMaxId();
			}
			return -1;
		}

		public bool Exists(int id)
		{
			if (this.dal != null)
			{
				return this.dal.Exists(id);
			}
			return false;
		}

		public int Add(AccAntiback model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			if (this.dal != null)
			{
				int num = this.dal.Add(model);
				if (num > 0)
				{
					model.id = this.GetMaxId() - 1;
					AccAntibackBll.m_data.Add(model.id, model);
					AccAntibackBll.m_isUpdate = true;
					return num;
				}
			}
			return -1;
		}

		public bool Update(AccAntiback model)
		{
			if (this.dal != null && this.dal.Update(model))
			{
				AccAntibackBll.m_data.Update(model.id, model);
				AccAntibackBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			if (this.dal != null)
			{
				AccAntibackBll.m_data.Remove(id);
				AccAntibackBll.m_isUpdate = true;
				return this.dal.Delete(id);
			}
			return false;
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal != null && this.dal.DeleteList(idlist))
			{
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccAntibackBll.m_data.Remove(array[i]);
					}
				}
				AccAntibackBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccAntiback GetModel(int id)
		{
			if (this.dal != null)
			{
				AccAntiback accAntiback = AccAntibackBll.m_data.Get(id);
				if (accAntiback != null)
				{
					return accAntiback;
				}
				accAntiback = this.dal.GetModel(id);
				if (accAntiback != null)
				{
					AccAntibackBll.m_data.Update(id, accAntiback);
				}
				return accAntiback;
			}
			return null;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) and (" + strWhere + ") ") : (" device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccAntiback> GetModelList(string strWhere)
		{
			if (this.dal != null)
			{
				List<AccAntiback> list = null;
				DataSet list2 = this.GetList(strWhere);
				list = this.DataTableToList(list2.Tables[0]);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						AccAntibackBll.m_data.Update(list[i].id, list[i]);
					}
				}
				return list;
			}
			return null;
		}

		public List<AccAntiback> DataTableToList(DataTable dt)
		{
			if (this.dal != null)
			{
				List<AccAntiback> list = new List<AccAntiback>();
				int count = dt.Rows.Count;
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						AccAntiback item = this.dal.DataConvert(dt.Rows[i]);
						list.Add(item);
					}
				}
				return list;
			}
			return null;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public void InitTable()
		{
			if (this.dal != null)
			{
				AccAntibackBll.m_data.Clear();
				AccAntibackBll.m_isloadall = false;
				this.dal.InitTable();
			}
		}
	}
}
