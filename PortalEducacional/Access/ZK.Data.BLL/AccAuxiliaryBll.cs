using System;
using System.Collections.Generic;
using System.Data;
using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Data.BLL
{
	public class AccAuxiliaryBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccAuxiliary> m_data = new DataCollections<AccAuxiliary>();

		private static bool m_isloadall = false;

		private readonly IAccAuxiliary dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccAuxiliaryBll.m_isUpdate;
			}
			set
			{
				AccAuxiliaryBll.m_isUpdate = value;
			}
		}

		public AccAuxiliaryBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccAuxiliary));
			if (service != null)
			{
				this.dal = (service as IAccAuxiliary);
			}
			this.actionLogBll = new ActionLogBll(this._ia);
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int id)
		{
			return this.dal.Exists(id);
		}

		public int Add(AccAuxiliary model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.Id = this.GetMaxId() - 1;
				AccAuxiliaryBll.m_data.Add(model.ToString(), model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddAuxiliary", "添加辅助输入输出") + ":" + model.AuxName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Auxiliary";
				this.actionLogBll.Add(this.actionLog);
				AccAuxiliaryBll.m_isUpdate = true;
			}
			return num;
		}

		public int Add(List<AccAuxiliary> lstModel)
		{
			int num = this.dal.Add(lstModel);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddAuxiliary", "批量添加辅助输入输出");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Auxiliary";
				this.actionLogBll.Add(this.actionLog);
				AccAuxiliaryBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccAuxiliary model)
		{
			if (this.dal.Update(model))
			{
				AccAuxiliaryBll.m_data.Update(model.ToString(), model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogUpdateAuxiliary", "更新辅助输入输出") + ":" + model.AuxName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Auxiliary";
				this.actionLogBll.Add(this.actionLog);
				AccAuxiliaryBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelAuxiliary", "删除辅助输入输出") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Auxiliary";
			this.actionLogBll.Add(this.actionLog);
			AccAuxiliaryBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogBatchDelAuxiliary", "批量删除辅助输入输出");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "Auxiliary";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccAuxiliaryBll.m_data.Remove(array[i]);
					}
				}
				AccAuxiliaryBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccAuxiliary GetModel(int id)
		{
			AccAuxiliary accAuxiliary = AccAuxiliaryBll.m_data.Get(id);
			if (accAuxiliary != null)
			{
				return accAuxiliary;
			}
			accAuxiliary = this.dal.GetModel(id);
			if (accAuxiliary != null)
			{
				AccAuxiliaryBll.m_data.Update(id, accAuxiliary);
			}
			return accAuxiliary;
		}

		public DataSet GetList(string strWhere)
		{
			string text = "device_id in (Select id from machines)";
			if (strWhere != null && strWhere.Trim() != "")
			{
				text = text + " and " + strWhere;
			}
			return this.dal.GetList(text);
		}

		public List<AccAuxiliary> GetModelList(string strWhere)
		{
			List<AccAuxiliary> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccAuxiliaryBll.m_data.Update(list[i].Id.ToString(), list[i]);
				}
			}
			return list;
		}

		public List<AccAuxiliary> DataTableToList(DataTable dt)
		{
			List<AccAuxiliary> list = new List<AccAuxiliary>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccAuxiliary item = this.dal.DataConvert(dt.Rows[i]);
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
			AccAuxiliaryBll.m_data.Clear();
			AccAuxiliaryBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
