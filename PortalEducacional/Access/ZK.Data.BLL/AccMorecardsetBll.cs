using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccMorecardsetBll
	{
		private IApplication _ia = null;

		private static DataCollections<AccMorecardset> m_data = new DataCollections<AccMorecardset>();

		private static bool m_isloadall = false;

		private readonly IAccMorecardset dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccMorecardsetBll.m_isUpdate;
			}
			set
			{
				AccMorecardsetBll.m_isUpdate = value;
			}
		}

		public AccMorecardsetBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccMorecardset));
			if (service != null)
			{
				this.dal = (service as IAccMorecardset);
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

		public int Add(AccMorecardset model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccMorecardsetBll.m_data.Add(model.id, model);
				AccMorecardsetBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccMorecardset model)
		{
			if (this.dal.Update(model))
			{
				AccMorecardsetBll.m_data.Update(model.id, model);
				AccMorecardsetBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccMorecardsetBll.m_data.Remove(id);
			AccMorecardsetBll.m_isUpdate = true;
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
						AccMorecardsetBll.m_data.Remove(array[i]);
					}
				}
				AccMorecardsetBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccMorecardset GetModel(int id)
		{
			AccMorecardset accMorecardset = AccMorecardsetBll.m_data.Get(id);
			if (accMorecardset != null)
			{
				return accMorecardset;
			}
			accMorecardset = this.dal.GetModel(id);
			if (accMorecardset != null)
			{
				AccMorecardsetBll.m_data.Update(id, accMorecardset);
			}
			return accMorecardset;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" create_operator ='" + SysInfos.SysUserInfo.id + "' and door_id in (select id from acc_door where device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) ) and (" + strWhere + " )") : (" create_operator ='" + SysInfos.SysUserInfo.id + "' and door_id in (select id from acc_door where device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) )"));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccMorecardset> GetModelList(string strWhere)
		{
			List<AccMorecardset> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccMorecardsetBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccMorecardset> DataTableToList(DataTable dt)
		{
			List<AccMorecardset> list = new List<AccMorecardset>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccMorecardset item = this.dal.DataConvert(dt.Rows[i]);
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
			AccMorecardsetBll.m_data.Clear();
			AccMorecardsetBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
