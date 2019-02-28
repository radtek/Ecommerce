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
	public class AccInterlockBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccInterlock> m_data = new DataCollections<AccInterlock>();

		private static bool m_isloadall = false;

		private readonly IAccInterlock dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccInterlockBll.m_isUpdate;
			}
			set
			{
				AccInterlockBll.m_isUpdate = value;
			}
		}

		public AccInterlockBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccInterlock));
			if (service != null)
			{
				this.dal = (service as IAccInterlock);
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

		public int Add(AccInterlock model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccInterlockBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddInterlock", "添加设备互锁信息") + model.device_id;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccInterlock";
				this.actionLogBll.Add(this.actionLog);
				AccInterlockBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccInterlock model)
		{
			if (this.dal.Update(model))
			{
				AccInterlockBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyInterlock", "修改设备互锁信息") + model.device_id;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccInterlock";
				this.actionLogBll.Add(this.actionLog);
				AccInterlockBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccInterlockBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelInterlock", "删除设备互锁信息") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccInterlock";
			this.actionLogBll.Add(this.actionLog);
			AccInterlockBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelInterlock", "删除设备互锁信息");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccInterlock";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccInterlockBll.m_data.Remove(array[i]);
					}
				}
				AccInterlockBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccInterlock GetModel(int id)
		{
			AccInterlock accInterlock = AccInterlockBll.m_data.Get(id);
			if (accInterlock != null)
			{
				return accInterlock;
			}
			accInterlock = this.dal.GetModel(id);
			if (accInterlock != null)
			{
				AccInterlockBll.m_data.Update(id, accInterlock);
			}
			return accInterlock;
		}

		public DataSet GetList(string strWhere)
		{
			if (this.dal != null)
			{
				if (string.IsNullOrEmpty(SysInfos.Areas))
				{
					return this.dal.GetList(strWhere);
				}
				strWhere = ((!string.IsNullOrEmpty(strWhere)) ? (" device_id in (select ID from  Machines where area_id in " + SysInfos.Areas + " ) and (" + strWhere + " )") : (" device_id in (select ID from Machines where area_id in " + SysInfos.Areas + " ) "));
				return this.dal.GetList(strWhere);
			}
			return null;
		}

		public List<AccInterlock> GetModelList(string strWhere)
		{
			List<AccInterlock> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccInterlockBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccInterlock> DataTableToList(DataTable dt)
		{
			List<AccInterlock> list = new List<AccInterlock>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccInterlock item = this.dal.DataConvert(dt.Rows[i]);
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
			AccInterlockBll.m_data.Clear();
			AccInterlockBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
