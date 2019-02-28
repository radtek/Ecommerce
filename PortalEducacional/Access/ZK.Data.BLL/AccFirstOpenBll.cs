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
	public class AccFirstOpenBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccFirstOpen> m_data = new DataCollections<AccFirstOpen>();

		private static bool m_isloadall = false;

		private readonly IAccFirstOpen dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccFirstOpenBll.m_isUpdate;
			}
			set
			{
				AccFirstOpenBll.m_isUpdate = value;
			}
		}

		public AccFirstOpenBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccFirstOpen));
			if (service != null)
			{
				this.dal = (service as IAccFirstOpen);
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

		public int Add(AccFirstOpen model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccFirstOpenBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddFirstOpen", "添加首卡开门信息") + model.door_id;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccFirstOpen";
				this.actionLogBll.Add(this.actionLog);
				AccFirstOpenBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccFirstOpen model)
		{
			if (this.dal.Update(model))
			{
				AccFirstOpenBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyFirstOpen", "修改首卡开门信息") + model.door_id;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccFirstOpen";
				this.actionLogBll.Add(this.actionLog);
				AccFirstOpenBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccFirstOpenBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpen", "删除首卡开门信息") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccFirstOpen";
			this.actionLogBll.Add(this.actionLog);
			AccFirstOpenBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelFirstOpen", "删除首卡开门信息");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccFirstOpen";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccFirstOpenBll.m_data.Remove(array[i]);
					}
				}
				AccFirstOpenBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccFirstOpen GetModel(int id)
		{
			AccFirstOpen accFirstOpen = AccFirstOpenBll.m_data.Get(id);
			if (accFirstOpen != null)
			{
				return accFirstOpen;
			}
			accFirstOpen = this.dal.GetModel(id);
			if (accFirstOpen != null)
			{
				AccFirstOpenBll.m_data.Update(id, accFirstOpen);
			}
			return accFirstOpen;
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

		public List<AccFirstOpen> GetModelList(string strWhere)
		{
			List<AccFirstOpen> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccFirstOpenBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccFirstOpen> DataTableToList(DataTable dt)
		{
			List<AccFirstOpen> list = new List<AccFirstOpen>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccFirstOpen item = this.dal.DataConvert(dt.Rows[i]);
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
			AccFirstOpenBll.m_data.Clear();
			AccFirstOpenBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
