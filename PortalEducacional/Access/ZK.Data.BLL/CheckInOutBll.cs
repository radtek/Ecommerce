using System.Collections.Generic;
using System.Data;
using ZK.Data.DBUtility;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Framework;
using ZKTimeNet.AccessDataClass;

namespace ZK.Data.BLL
{
	public class CheckInOutBll
	{
		private IApplication _ia = null;

		private readonly ICheckInOut dal = null;

		public CheckInOutBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(ICheckInOut));
			if (service != null)
			{
				this.dal = (service as ICheckInOut);
			}
		}

		public int GetMaxId()
		{
			return this.dal.GetMaxId();
		}

		public bool Exists(int USERID)
		{
			return this.dal.Exists(USERID);
		}

		public int Add(CheckInOut model)
		{
			int num = 0;
			num = this.dal.Add(model);
			if (num > 0)
			{
				AccessPunches accessPunches = new AccessPunches();
				accessPunches.emp_pin = model.Pin;
				switch (model.CHECKTYPE)
				{
				case "I":
					accessPunches.punch_state = 0;
					break;
				case "O":
					accessPunches.punch_state = 1;
					break;
				case "0":
					accessPunches.punch_state = 2;
					break;
				case "1":
					accessPunches.punch_state = 3;
					break;
				case "i":
					accessPunches.punch_state = 4;
					break;
				case "o":
					accessPunches.punch_state = 5;
					break;
				}
				accessPunches.punch_time = model.CHECKTIME.Value;
				accessPunches.workcode = model.WorkCode;
				ZKTimeNetLite.accessExce.CreateEntity(new List<object>
				{
					(object)accessPunches
				}, true);
			}
			return num;
		}

		public void Add(List<CheckInOut> list)
		{
			this.dal.Add(list, out List<CheckInOut> list2);
			if (list2 != null)
			{
				List<object> list3 = new List<object>();
				for (int i = 0; i < list2.Count; i++)
				{
					CheckInOut checkInOut = list2[i];
					AccessPunches accessPunches = new AccessPunches();
					accessPunches.emp_pin = checkInOut.Pin;
					switch (checkInOut.CHECKTYPE)
					{
					case "I":
						accessPunches.punch_state = 0;
						break;
					case "O":
						accessPunches.punch_state = 1;
						break;
					case "0":
						accessPunches.punch_state = 2;
						break;
					case "1":
						accessPunches.punch_state = 3;
						break;
					case "i":
						accessPunches.punch_state = 4;
						break;
					case "o":
						accessPunches.punch_state = 5;
						break;
					}
					accessPunches.punch_time = checkInOut.CHECKTIME.Value;
					accessPunches.workcode = checkInOut.WorkCode;
					list3.Add(accessPunches);
				}
				ZKTimeNetLite.accessExce.CreateEntity(list3, true);
			}
		}

		public bool Update(CheckInOut model)
		{
			bool flag = this.dal.Update(model);
			if (flag)
			{
				AccessPunches accessPunches = new AccessPunches();
				accessPunches.emp_pin = model.Pin;
				switch (model.CHECKTYPE)
				{
				case "I":
					accessPunches.punch_state = 0;
					break;
				case "O":
					accessPunches.punch_state = 1;
					break;
				case "0":
					accessPunches.punch_state = 2;
					break;
				case "1":
					accessPunches.punch_state = 3;
					break;
				case "i":
					accessPunches.punch_state = 4;
					break;
				case "o":
					accessPunches.punch_state = 5;
					break;
				}
				accessPunches.punch_time = model.CHECKTIME.Value;
				accessPunches.workcode = model.WorkCode;
				ZKTimeNetLite.accessExce.UpdateEntity(new List<object>
				{
					(object)accessPunches
				}, false);
			}
			return flag;
		}

		public bool Delete(int USERID)
		{
			return this.dal.Delete(USERID);
		}

		public bool DeleteList(string USERIDlist)
		{
			return this.dal.DeleteList(USERIDlist);
		}

		public CheckInOut GetModel(int USERID)
		{
			return this.dal.GetModel(USERID);
		}

		public DataSet GetList(string strWhere)
		{
			return this.dal.GetList(strWhere);
		}

		public List<CheckInOut> GetModelList(string strWhere)
		{
			DataSet list = this.dal.GetList(strWhere);
			return this.DataTableToList(list.Tables[0]);
		}

		public List<CheckInOut> DataTableToList(DataTable dt)
		{
			List<CheckInOut> list = new List<CheckInOut>();
			int count = dt.Rows.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					CheckInOut item = this.dal.DataConvert(dt.Rows[i]);
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
			ZKTimeNetLite.accessExce.ClearAttLog();
		}

		public void Sync2ZKTime()
		{
			List<CheckInOut> modelList = this.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				UserInfoBll userInfoBll = new UserInfoBll(this._ia);
				List<UserInfo> modelList2 = userInfoBll.GetModelList("", null, false);
				Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
				if (modelList2 != null)
				{
					for (int i = 0; i < modelList2.Count; i++)
					{
						UserInfo userInfo = modelList2[i];
						if (!dictionary.ContainsKey(userInfo.UserId))
						{
							dictionary.Add(userInfo.UserId, userInfo);
						}
					}
				}
				List<object> list = new List<object>();
				for (int j = 0; j < modelList.Count; j++)
				{
					CheckInOut checkInOut = modelList[j];
					if (dictionary.ContainsKey(checkInOut.USERID))
					{
						UserInfo userInfo = dictionary[checkInOut.USERID];
						checkInOut.Pin = int.Parse(userInfo.BadgeNumber);
						AccessPunches accessPunches = new AccessPunches();
						accessPunches.emp_pin = checkInOut.Pin;
						switch (checkInOut.CHECKTYPE)
						{
						case "I":
							accessPunches.punch_state = 0;
							break;
						case "O":
							accessPunches.punch_state = 1;
							break;
						case "0":
							accessPunches.punch_state = 2;
							break;
						case "1":
							accessPunches.punch_state = 3;
							break;
						case "i":
							accessPunches.punch_state = 4;
							break;
						case "o":
							accessPunches.punch_state = 5;
							break;
						}
						accessPunches.punch_time = checkInOut.CHECKTIME.Value;
						accessPunches.workcode = checkInOut.WorkCode;
						list.Add(accessPunches);
					}
				}
				ZKTimeNetLite.accessExce.CreateEntity(list, true);
			}
		}
	}
}
