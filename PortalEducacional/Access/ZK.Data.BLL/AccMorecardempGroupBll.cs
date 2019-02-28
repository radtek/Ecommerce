using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccMorecardempGroupBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccMorecardempGroup> m_data = new DataCollections<AccMorecardempGroup>();

		private static bool m_isloadall = false;

		private readonly IAccMorecardempGroup dal = null;

		public AccMorecardempGroupBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccMorecardempGroup));
			if (service != null)
			{
				this.dal = (service as IAccMorecardempGroup);
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

		public bool Exists(string name)
		{
			return this.dal.Exists(name);
		}

		public int Search(string strdata)
		{
			return this.dal.Search(strdata);
		}

		public int Add(AccMorecardempGroup model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccMorecardempGroupBll.m_data.Add(model.id, model);
			}
			return num;
		}

		public bool Update(AccMorecardempGroup model)
		{
			if (this.dal.Update(model))
			{
				AccMorecardempGroupBll.m_data.Update(model.id, model);
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccMorecardempGroupBll.m_data.Remove(id);
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
						AccMorecardempGroupBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public AccMorecardempGroup GetModel(int id)
		{
			AccMorecardempGroup accMorecardempGroup = AccMorecardempGroupBll.m_data.Get(id);
			if (accMorecardempGroup != null)
			{
				return accMorecardempGroup;
			}
			accMorecardempGroup = this.dal.GetModel(id);
			if (accMorecardempGroup != null)
			{
				AccMorecardempGroupBll.m_data.Update(id, accMorecardempGroup);
			}
			return accMorecardempGroup;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas) && string.IsNullOrEmpty(SysInfos.Depts))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" create_operator ='" + SysInfos.SysUserInfo.id + "'  and (" + strWhere + " )") : (" create_operator ='" + SysInfos.SysUserInfo.id + "' "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccMorecardempGroup> GetModelList(string strWhere)
		{
			List<AccMorecardempGroup> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccMorecardempGroupBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccMorecardempGroup> DataTableToList(DataTable dt)
		{
			List<AccMorecardempGroup> list = new List<AccMorecardempGroup>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccMorecardempGroup item = this.dal.DataConvert(dt.Rows[i]);
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
			AccMorecardempGroupBll.m_data.Clear();
			AccMorecardempGroupBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
