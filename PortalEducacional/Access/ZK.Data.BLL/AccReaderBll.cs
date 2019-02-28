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
	public class AccReaderBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccReader> m_data = new DataCollections<AccReader>();

		private static bool m_isloadall = false;

		private readonly IAccReader dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccReaderBll.m_isUpdate;
			}
			set
			{
				AccReaderBll.m_isUpdate = value;
			}
		}

		public AccReaderBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccReader));
			if (service != null)
			{
				this.dal = (service as IAccReader);
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

		public int Add(AccReader model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.Id = this.GetMaxId() - 1;
				AccReaderBll.m_data.Add(model.Id.ToString(), model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddReader", "添加读头") + ":" + model.ReaderName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Reader";
				this.actionLogBll.Add(this.actionLog);
				AccReaderBll.m_isUpdate = true;
			}
			return num;
		}

		public int Add(List<AccReader> lstModel)
		{
			int num = this.dal.Add(lstModel);
			if (num > 0)
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogBatchAddReader", "批量添加读头");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Reader";
				this.actionLogBll.Add(this.actionLog);
				AccReaderBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccReader model)
		{
			if (this.dal.Update(model))
			{
				AccReaderBll.m_data.Update(model.Id.ToString(), model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogUpdateReader", "更新读头") + ":" + model.ReaderName;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "Reader";
				this.actionLogBll.Add(this.actionLog);
				AccReaderBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelReader", "删除读头") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "Reader";
			this.actionLogBll.Add(this.actionLog);
			AccReaderBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogBatchDelReader", "批量删除读头");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "Reader";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccReaderBll.m_data.Remove(array[i]);
					}
				}
				AccReaderBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccReader GetModel(int id)
		{
			AccReader accReader = AccReaderBll.m_data.Get(id);
			if (accReader != null)
			{
				return accReader;
			}
			accReader = this.dal.GetModel(id);
			if (accReader != null)
			{
				AccReaderBll.m_data.Update(id, accReader);
			}
			return accReader;
		}

		public DataSet GetList(string strWhere)
		{
			string text = "door_id in (Select id from acc_door where device_id in (Select id from machines))";
			if (strWhere != null && strWhere.Trim() != "")
			{
				text = text + " and " + strWhere;
			}
			return this.dal.GetList(text);
		}

		public List<AccReader> GetModelList(string strWhere)
		{
			List<AccReader> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccReaderBll.m_data.Update(list[i].ToString(), list[i]);
				}
			}
			return list;
		}

		public List<AccReader> DataTableToList(DataTable dt)
		{
			List<AccReader> list = new List<AccReader>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccReader item = this.dal.DataConvert(dt.Rows[i]);
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
			AccReaderBll.m_data.Clear();
			AccReaderBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
