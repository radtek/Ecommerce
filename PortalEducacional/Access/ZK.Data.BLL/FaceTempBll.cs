using System.Collections.Generic;
using System.Data;
using System.Text;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class FaceTempBll
	{
		private IApplication _ia = null;

		private readonly IFaceTemp dal = null;

		public FaceTempBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IFaceTemp));
			if (service != null)
			{
				this.dal = (service as IFaceTemp);
			}
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int TemplateId)
		{
			return this.dal.Exists(TemplateId);
		}

		public int Add(FaceTemp model)
		{
			string strWhere = $" or pin={model.Pin} ";
			this.DeleteList(strWhere);
			int num = this.dal.Add(model);
			if (num > 0)
			{
				UserInfoBll.ClearDs();
			}
			return num;
		}

		public void Add(List<FaceTemp> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<long, List<byte>> dictionary = new Dictionary<long, List<byte>>();
			for (int i = 0; i < list.Count; i++)
			{
				FaceTemp faceTemp = list[i];
				long key = long.Parse(faceTemp.Pin);
				if (!dictionary.ContainsKey(key))
				{
					stringBuilder.Append($" or pin={faceTemp.Pin}");
					if (!dictionary.ContainsKey(key))
					{
						List<byte> list2 = new List<byte>();
						list2.Add(faceTemp.FaceType);
						dictionary.Add(key, list2);
					}
					if ((i + 1) % 30 == 0)
					{
						this.DeleteList(stringBuilder.ToString());
						stringBuilder = new StringBuilder();
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				this.DeleteList(stringBuilder.ToString());
				stringBuilder = new StringBuilder();
			}
			UserInfoBll.ClearDs();
			this.dal.Add(list);
		}

		public bool Update(FaceTemp model)
		{
			return this.dal.Update(model);
		}

		public void Update(List<FaceTemp> list)
		{
			this.dal.Update(list);
		}

		public bool Delete(int TemplateId)
		{
			UserInfoBll.ClearDs();
			return this.dal.Delete(TemplateId);
		}

		public bool DeleteList(string strWhere)
		{
			UserInfoBll.ClearDs();
			return this.dal.DeleteList(strWhere);
		}

		public FaceTemp GetModel(int TemplateId)
		{
			return this.dal.GetModel(TemplateId);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<FaceTemp> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<FaceTemp> DataTableToList(DataTable dt)
		{
			List<FaceTemp> list = new List<FaceTemp>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					FaceTemp item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public Dictionary<int, int> GetUserId_FaceTempCountDic(string Filter = "FaceType = 0")
		{
			return this.dal.GetUserId_FaceTempCountDic(Filter);
		}

		public void InitTable()
		{
			this.dal.InitTable();
		}

		public Dictionary<int, Dictionary<int, int>> GetPIN_FaceIdDic()
		{
			return this.dal.GetPIN_FaceIdDic();
		}

		public DataTable GetFields(string Fields, string strWhere)
		{
			return this.dal.GetFields(Fields, strWhere);
		}
	}
}
