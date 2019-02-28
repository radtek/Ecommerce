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
	public class AccLinkAgeIoBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccLinkAgeIo> m_data = new DataCollections<AccLinkAgeIo>();

		private static bool m_isloadall = false;

		private readonly IAccLinkAgeIo dal = null;

		private static bool m_isUpdate = false;

		public static bool IsUpdate
		{
			get
			{
				return AccLinkAgeIoBll.m_isUpdate;
			}
			set
			{
				AccLinkAgeIoBll.m_isUpdate = value;
			}
		}

		public AccLinkAgeIoBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccLinkAgeIo));
			if (service != null)
			{
				this.dal = (service as IAccLinkAgeIo);
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

		public bool Exists(string name)
		{
			return this.dal.Exists(name);
		}

		public int Add(AccLinkAgeIo model)
		{
			model.create_operator = SysInfos.SysUserInfo.id.ToString();
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccLinkAgeIoBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddLinkAgeIo", "添加联动信息") + model.linkage_name;
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccLinkAgeIo";
				this.actionLogBll.Add(this.actionLog);
				AccLinkAgeIoBll.m_isUpdate = true;
			}
			return num;
		}

		public bool Update(AccLinkAgeIo model)
		{
			if (this.dal.Update(model))
			{
				AccLinkAgeIoBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyLinkAgeIo", "修改联动信息") + model.linkage_name;
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccLinkAgeIo";
				this.actionLogBll.Add(this.actionLog);
				AccLinkAgeIoBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccLinkAgeIoBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLinkAgeIo", "删除联动信息") + id;
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLinkAgeIo";
			this.actionLogBll.Add(this.actionLog);
			AccLinkAgeIoBll.m_isUpdate = true;
			return this.dal.Delete(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLinkAgeIo", "删除联动信息");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccLinkAgeIo";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccLinkAgeIoBll.m_data.Remove(array[i]);
					}
				}
				AccLinkAgeIoBll.m_isUpdate = true;
				return true;
			}
			return false;
		}

		public AccLinkAgeIo GetModel(int id)
		{
			AccLinkAgeIo accLinkAgeIo = AccLinkAgeIoBll.m_data.Get(id);
			if (accLinkAgeIo != null)
			{
				return accLinkAgeIo;
			}
			accLinkAgeIo = this.dal.GetModel(id);
			if (accLinkAgeIo != null)
			{
				AccLinkAgeIoBll.m_data.Update(id, accLinkAgeIo);
			}
			return accLinkAgeIo;
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

		public List<AccLinkAgeIo> GetModelList(string strWhere)
		{
			List<AccLinkAgeIo> list = null;
			DataSet list2 = this.GetList(strWhere);
			list = this.DataTableToList(list2.Tables[0]);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					AccLinkAgeIoBll.m_data.Update(list[i].id, list[i]);
				}
			}
			return list;
		}

		public List<AccLinkAgeIo> DataTableToList(DataTable dt)
		{
			List<AccLinkAgeIo> list = new List<AccLinkAgeIo>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccLinkAgeIo item = this.dal.DataConvert(dt.Rows[i]);
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
			AccLinkAgeIoBll.m_data.Clear();
			AccLinkAgeIoBll.m_isloadall = false;
			this.dal.InitTable();
		}
	}
}
