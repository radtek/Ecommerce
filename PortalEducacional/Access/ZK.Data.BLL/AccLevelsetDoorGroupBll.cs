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
	public class AccLevelsetDoorGroupBll
	{
		private IApplication _ia = null;

		private ActionLogBll actionLogBll = null;

		private ActionLog actionLog = new ActionLog();

		private static DataCollections<AccLevelsetDoorGroup> m_data = new DataCollections<AccLevelsetDoorGroup>();

		private static bool m_isloadall = false;

		private readonly IAccLevelsetDoorGroup dal = null;

		public AccLevelsetDoorGroupBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IAccLevelsetDoorGroup));
			if (service != null)
			{
				this.dal = (service as IAccLevelsetDoorGroup);
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

		public int Add(AccLevelsetDoorGroup model)
		{
			int num = this.dal.Add(model);
			if (num > 0)
			{
				model.id = this.GetMaxId() - 1;
				AccLevelsetDoorGroupBll.m_data.Add(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogAddLevelsetDoorGroup", "添加人员权限组信息");
				this.actionLog.action_flag = 1;
				this.actionLog.object_repr = "AccLevelsetDoorGroup";
				this.actionLogBll.Add(this.actionLog);
			}
			return num;
		}

		public bool Update(AccLevelsetDoorGroup model)
		{
			if (this.dal.Update(model))
			{
				AccLevelsetDoorGroupBll.m_data.Update(model.id, model);
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogModifyLevelsetDoorGroup", "修改人员权限组信息");
				this.actionLog.action_flag = 3;
				this.actionLog.object_repr = "AccLevelsetDoorGroup";
				this.actionLogBll.Add(this.actionLog);
				return true;
			}
			return false;
		}

		public bool Delete(int id)
		{
			AccLevelsetDoorGroupBll.m_data.Remove(id);
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetDoorGroup", "删除人员权限组信息");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLevelsetDoorGroup";
			this.actionLogBll.Add(this.actionLog);
			return this.dal.Delete(id);
		}

		public bool DeleteByAcclevelsetID(int id)
		{
			this.actionLog.action_time = DateTime.Now;
			this.actionLog.user_id = SysInfos.SysUserInfo.id;
			this.actionLog.content_type_id = 3;
			this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetDoorGroup", "删除人员权限组信息");
			this.actionLog.action_flag = 2;
			this.actionLog.object_repr = "AccLevelsetDoorGroup";
			this.actionLogBll.Add(this.actionLog);
			AccLevelsetDoorGroupBll.m_isloadall = false;
			return this.dal.DeleteByAcclevelsetID(id);
		}

		public bool DeleteList(string idlist)
		{
			if (this.dal.DeleteList(idlist))
			{
				this.actionLog.action_time = DateTime.Now;
				this.actionLog.user_id = SysInfos.SysUserInfo.id;
				this.actionLog.content_type_id = 3;
				this.actionLog.change_message = ShowMsgInfos.GetInfo("SLogDelLevelsetDoorGroup", "删除人员权限组信息");
				this.actionLog.action_flag = 2;
				this.actionLog.object_repr = "AccLevelsetDoorGroup";
				this.actionLogBll.Add(this.actionLog);
				idlist = idlist.Replace("'", "");
				string[] array = idlist.Split(',');
				if (array != null && array.Length != 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						AccLevelsetDoorGroupBll.m_data.Remove(array[i]);
					}
				}
				return true;
			}
			return false;
		}

		public AccLevelsetDoorGroup GetModel(int id)
		{
			AccLevelsetDoorGroup accLevelsetDoorGroup = AccLevelsetDoorGroupBll.m_data.Get(id);
			if (accLevelsetDoorGroup != null)
			{
				return accLevelsetDoorGroup;
			}
			accLevelsetDoorGroup = this.dal.GetModel(id);
			if (accLevelsetDoorGroup != null)
			{
				AccLevelsetDoorGroupBll.m_data.Update(id, accLevelsetDoorGroup);
			}
			return accLevelsetDoorGroup;
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<AccLevelsetDoorGroup> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			List<AccLevelsetDoorGroup> list2 = this.DataTableToList(list.Tables[0]);
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					AccLevelsetDoorGroupBll.m_data.Update(list2[i].id, list2[i]);
				}
			}
			return list2;
		}

		public List<AccLevelsetDoorGroup> DataTableToList(DataTable dt)
		{
			List<AccLevelsetDoorGroup> list = new List<AccLevelsetDoorGroup>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AccLevelsetDoorGroup item = this.dal.DataConvert(dt.Rows[i]);
					list.Add(item);
				}
			}
			return list;
		}

		public DataSet GetAllList()
		{
			return this.GetList("");
		}

		public Dictionary<int, List<int>> GetLevelsetMachineIdDic()
		{
			return this.dal.GetLevelsetMachineIdDic();
		}

		public void InitTable()
		{
			AccLevelsetDoorGroupBll.m_data.Clear();
			AccLevelsetDoorGroupBll.m_isloadall = false;
			this.dal.InitTable();
		}

		public bool AccLevelsetDoorGroup_Addnew(List<AccLevelsetDoorGroup> list)
		{
			return this.dal.AccLevelsetDoorGroup_Addnew(list);
		}
	}
}
