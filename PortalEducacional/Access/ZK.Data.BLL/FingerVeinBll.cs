using System.Collections.Generic;
using System.Data;
using System.Text;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class FingerVeinBll
	{
		private IApplication _ia = null;

		private readonly IFingerVein dal = null;

		public FingerVeinBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IFingerVein));
			if (service != null)
			{
				this.dal = (service as IFingerVein);
			}
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int FVID)
		{
			return this.dal.Exists(FVID);
		}

		public int Add(FingerVein model)
		{
			this.dal.DeleteBy($"UserID={model.UserID} and FingerID={model.FingerID}");
			int num = this.dal.Add(model);
			if (num > 0)
			{
				UserInfoBll.ClearDs();
			}
			return num;
		}

		public void Add(List<FingerVein> list)
		{
			if (list != null && list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("1=0 ");
				for (int i = 0; i < list.Count; i++)
				{
					stringBuilder.AppendFormat("or (UserID={0} and FingerID={1}) ", list[i].UserID, list[i].FingerID);
					if (i + 1 == list.Count || (i + 1) % 50 == 0)
					{
						this.dal.DeleteBy(stringBuilder.ToString());
						stringBuilder.Clear();
						stringBuilder.Append("1=0 ");
					}
				}
				UserInfoBll.ClearDs();
				this.dal.Add(list);
			}
		}

		public bool Update(FingerVein model)
		{
			return this.dal.Update(model);
		}

		public void Update(List<FingerVein> list)
		{
			this.dal.Update(list);
		}

		public bool Delete(int FVID)
		{
			UserInfoBll.ClearDs();
			return this.dal.Delete(FVID);
		}

		public bool DeleteList(string FVIDlist)
		{
			UserInfoBll.ClearDs();
			return this.dal.DeleteList(FVIDlist);
		}

		public FingerVein GetModel(int FVID)
		{
			return this.dal.GetModel(FVID);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<FingerVein> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<FingerVein> DataTableToList(DataTable dt)
		{
			List<FingerVein> list = new List<FingerVein>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					FingerVein item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public Dictionary<int, int> GetUserId_FingerVeinCountDic()
		{
			return this.dal.GetUserId_FingerVeinCountDic();
		}

		public DataTable GetFields(string Fields, string strWhere)
		{
			return this.dal.GetFields(Fields, strWhere);
		}

		public void InitTable()
		{
			this.dal.InitTable();
		}

		public int DeleteBy(string where)
		{
			return this.dal.DeleteBy(where);
		}
	}
}
