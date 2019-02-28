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
	public class STD_WiegandFmtBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<STD_WiegandFmt> m_data = new DataCollections<STD_WiegandFmt>();

		private static bool m_isloadall = false;

		private readonly ISTD_WiegandFmt dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return STD_WiegandFmtBll.m_isUpdate;
			}
			set
			{
				STD_WiegandFmtBll.m_isUpdate = value;
			}
		}

		public STD_WiegandFmtBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(ISTD_WiegandFmt));
			if (service != null)
			{
				this.dal = (service as ISTD_WiegandFmt);
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

		public bool ExistsDoorId(int DoorId)
		{
			return this.dal.ExistsDoorId(DoorId);
		}

		public int Add(STD_WiegandFmt model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.Id = this.GetMaxId() - 1;
				STD_WiegandFmtBll.m_data.Add(model.Id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddSTDWiegand", "添加脱机韦根格式") + model.DoorId;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "STDWiegand";
				this.actionLogBll.Add(this.actionLog);
				STD_WiegandFmtBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(STD_WiegandFmt model)
		{
			if (this.dal.Update(model))
			{
				STD_WiegandFmtBll.m_data.Update(model.Id, model);
				STD_WiegandFmtBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			STD_WiegandFmtBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelSTDWiegand", "删除脱机韦根格式") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "STDWiegand";
			this.actionLogBll.Add(this.actionLog);
			STD_WiegandFmtBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = "删除脱机韦根格式";
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "STDWiegand";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						STD_WiegandFmtBll.m_data.Remove(array[i]);
					}
				}
				STD_WiegandFmtBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public STD_WiegandFmt GetModel(int id)
		{
			STD_WiegandFmt sTD_WiegandFmt = STD_WiegandFmtBll.m_data.Get(id);
			if (sTD_WiegandFmt != null)
			{
				return sTD_WiegandFmt;
			}
			sTD_WiegandFmt = this.dal.GetModel(id);
			if (sTD_WiegandFmt != null)
			{
				STD_WiegandFmtBll.m_data.Update(id, sTD_WiegandFmt);
			}
			return sTD_WiegandFmt;
		}

		public STD_WiegandFmt GetModelByDoorId(int DoorId)
		{
			STD_WiegandFmt result = null;
			DataSet list = this.dal.GetList("DoorId=" + DoorId);
			if (list != null && list.Tables.Count > 0 && list.Tables[0].Rows.Count > 0)
			{
				result = this.dal.DataConvert(list.Tables[0].Rows[0]);
			}
			return result;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<STD_WiegandFmt> GetModelList(string strWhere)
		{
			List<STD_WiegandFmt> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					STD_WiegandFmtBll.m_data.Update(list[i].Id, list[i]);
				}
			}
			return list;
		}

		public List<STD_WiegandFmt> DataTableToList(DataTable dt)
		{
			List<STD_WiegandFmt> list = new List<STD_WiegandFmt>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					STD_WiegandFmt item = this.dal.DataConvert(dt.Rows[i]);
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
			STD_WiegandFmtBll.m_data.Clear();
			STD_WiegandFmtBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
