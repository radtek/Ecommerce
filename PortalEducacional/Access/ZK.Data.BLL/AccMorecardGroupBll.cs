using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccMorecardGroupBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccMorecardGroup> m_data = new DataCollections<AccMorecardGroup>();

		private static bool m_isloadall = false;

		private readonly IAccMorecardGroup dal = null;

		private bool m_isUpdate = false;

		public bool IsUpdate
		{
			get
			{
				return this.m_isUpdate;
			}
			set
			{
				this.m_isUpdate = value;
			}
		}

		public AccMorecardGroupBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccMorecardGroup));
			if (service != null)
			{
				this.dal = (service as IAccMorecardGroup);
			}
		}

		public bool Exists(string id)
		{
			return this.dal.Exists(id);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public int Add(AccMorecardGroup model)
		{
			int num = SysInfos.SysUserInfo.id;
			model.create_operator = num.ToString();
			int num2 = this.dal.Add(model);
			if (num2 > 0)
			{
				num = this.GetMaxId() - 1;
				model.id = num.ToString();
				AccMorecardGroupBll.m_data.Add(model.id, model);
				this.m_isUpdate = true;
			}
			return num2;
		}

		public bool Update(AccMorecardGroup model)
		{
			if (this.dal.Update(model))
			{
				AccMorecardGroupBll.m_data.Update(model.id, model);
				this.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(string id)
		{
			AccMorecardGroupBll.m_data.Remove(id);
			this.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteByCmbID(int cmbid)
		{
			AccMorecardGroupBll.m_isloadall = false;
			this.m_isUpdate = true;
			return this.dal.DeleteByCmbID(cmbid);
		}

		public bool DeleteByGroupID(int groupid)
		{
			AccMorecardGroupBll.m_isloadall = false;
			this.m_isUpdate = true;
			return this.dal.DeleteByGroupID(groupid);
		}

		public bool DeleteByGroupAndcmbID(int cmbid, int groupid)
		{
			AccMorecardGroupBll.m_isloadall = false;
			this.m_isUpdate = true;
			return this.dal.DeleteByGroupAndcmbID(cmbid, groupid);
		}

		public bool DeleteEx(string strwhere)
		{
			AccMorecardGroupBll.m_isloadall = false;
			this.m_isUpdate = true;
			return this.dal.DeleteEx(strwhere);
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
						AccMorecardGroupBll.m_data.Remove(array[i]);
					}
				}
				this.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccMorecardGroup GetModel(string id)
		{
			AccMorecardGroup accMorecardGroup = AccMorecardGroupBll.m_data.Get(id);
			if (accMorecardGroup != null)
			{
				return accMorecardGroup;
			}
			accMorecardGroup = this.dal.GetModel(id);
			if (accMorecardGroup != null)
			{
				AccMorecardGroupBll.m_data.Update(id, accMorecardGroup);
			}
			return accMorecardGroup;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccMorecardGroup> GetModelList(string strWhere)
		{
			List<AccMorecardGroup> list = null;
			DataSet list2 = this.dal.GetList(strWhere);
			return this.DataTableToList(list2.Tables[0]);
		}

		public List<AccMorecardGroup> DataTableToList(DataTable dt)
		{
			List<AccMorecardGroup> list = new List<AccMorecardGroup>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccMorecardGroup item = this.dal.DataConvert(dt.Rows[i]);
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
			AccMorecardGroupBll.m_data.Clear();
			AccMorecardGroupBll.m_isloadall = false;
			this.dal.InitTable();
		}

		public DataTable GetFields(string Fields, string strWhere)
		{
			return this.dal.GetFields(Fields, strWhere);
		}
	}
}
