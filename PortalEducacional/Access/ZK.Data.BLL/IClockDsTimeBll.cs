using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class IClockDsTimeBll
	{
		private IApplication _ia = null;

		private readonly IIClockDsTime dal = null;

		public IClockDsTimeBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IIClockDsTime));
			if (service != null)
			{
				this.dal = (service as IIClockDsTime);
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

		public int Add(IClockDsTime model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			return this.dal.Add(model);
		}

		public bool Update(IClockDsTime model)
		{
			return this.dal.Update(model);
		}

		public bool Delete(int id)
		{
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			return this.dal.DeleteList(idlist);
		}

		public IClockDsTime GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<IClockDsTime> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<IClockDsTime> DataTableToList(DataTable dt)
		{
			List<IClockDsTime> list = new List<IClockDsTime>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					IClockDsTime item = this.dal.DataConvert(dt.Rows[i]);
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
