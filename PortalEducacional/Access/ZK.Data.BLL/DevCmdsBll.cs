using System;
using System.Collections.Generic;
using System.Data;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class DevCmdsBll
	{
		private IApplication _ia = null;

		private readonly IDevCmds dal = null;

		public DevCmdsBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IDevCmds));
			if (service != null)
			{
				this.dal = (service as IDevCmds);
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

		public void Add(DevCmds model)
		{
			model.create_operator = DevCmds.CreateOperatorLogin;
			model.create_time = DateTime.Now;
			model.CmdImmediately = DevCmds.RunImmediatelyEnabled;
			if (!string.IsNullOrEmpty(model.CmdContent))
			{
				this.dal.Add(model);
			}
		}

		public void AddEx(DevCmds model)
		{
			model.create_operator = DevCmds.CreateOperatorLogin;
			model.create_time = DateTime.Now;
			model.CmdImmediately = DevCmds.RunImmediatelyEnabled;
			if (!string.IsNullOrEmpty(model.CmdContent))
			{
				this.dal.AddEx(model);
			}
		}

		public int Add(List<DevCmds> modelList)
		{
			int num = 0;
			foreach (DevCmds model in modelList)
			{
				model.create_operator = DevCmds.CreateOperatorLogin;
				model.create_time = DateTime.Now;
				model.CmdImmediately = DevCmds.RunImmediatelyEnabled;
			}
			return this.dal.Add(modelList);
		}

		public bool Update(DevCmds model)
		{
			return this.dal.Update(model);
		}

		public bool UpdateEx(DevCmds model)
		{
			return this.dal.UpdateEx(model);
		}

		public bool Update(List<DevCmds> lstModel)
		{
			return this.dal.Update(lstModel);
		}

		public int UpdateStatus(int status, string strWhere)
		{
			return this.dal.UpdateStatus(status, strWhere);
		}

		public bool Delete(int id)
		{
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			return this.dal.DeleteList(idlist);
		}

		public int DeleteByMachineId(string idList)
		{
			return this.dal.DeleteByMachineId(idList);
		}

		public DevCmds GetModel(int id)
		{
			return this.dal.GetModel(id);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<DevCmds> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<DevCmds> DataTableToList(DataTable dt)
		{
			List<DevCmds> list = new List<DevCmds>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					DevCmds item = this.dal.DataConvert(dt.Rows[i]);
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

		public void Clear()
		{
			this.dal.Clear();
		}

		public long GetCount(string strWhere)
		{
			return this.dal.GetCount(strWhere);
		}
	}
}
