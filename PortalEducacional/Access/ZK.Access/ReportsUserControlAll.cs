/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZK.Access.door;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class ReportsUserControlAll : UserControl
	{
		private string strRecordsType = string.Empty;

		private string strCondtion;

		private DataSet ds;

		private AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);

		private Dictionary<int, string> eventTypeList = new Dictionary<int, string>();

		private Dictionary<int, string> stateList = new Dictionary<int, string>();

		private Dictionary<int, string> verifiedList = new Dictionary<int, string>();

		private Dictionary<int, string> inAddressList = new Dictionary<int, string>();

		private Dictionary<int, string> outAddressList = new Dictionary<int, string>();

		private Dictionary<int, string> linkAgeIOList = new Dictionary<int, string>();

		private Dictionary<string, string> userInfoDicList = new Dictionary<string, string>();

		private Dictionary<string, string> userInfoDicListlastName = new Dictionary<string, string>();

		private DataTable dtUser = new DataTable();

		private DataTable dtVerifyType;

		private DataTable dtEventType;

		private Dictionary<int, Dictionary<int, AccReader>> dicDoorIdInOutState_Reader;

		private Dictionary<int, Dictionary<int, AccDoor>> dicDevIdDoorNo_Door;

		private DataTable m_dataTable = new DataTable();

		private XPInstantFeedbackSource xpifs;

		private IContainer components = null;

		private ToolStripButton btn_export;

		private ToolStripButton btn_clear;

		public PanelEx pnl_reports;

		public ToolStrip MenuPanelEx;

		private GroupBox gbx_searchCondition;

		private Label label2;

		private Label lbl_dateTime;

		private TextBox txt_userPin;

		private Label lbl_userPin;

		private TextBox txt_cardNo;

		private Label lbl_cardNo;

		private System.Windows.Forms.ComboBox cmb_verified;

		private Label lbl_verified;

		private System.Windows.Forms.ComboBox cmb_devName;

		private Label lbl_deviceName;

		private System.Windows.Forms.ComboBox cmb_eventType;

		private Label lbl_evnetType;

		private System.Windows.Forms.ComboBox cmb_inoutState;

		private Label lbl_inoutState;

		private ButtonX btn_cancel;

		private ButtonX btn_search;

		private GridControl grd_checkInOutView;

		private GridView grd_reportView;

		private GridColumn column_outAddress;

		private GridColumn column_checkTime;

		private GridColumn column_userID;

		private GridColumn column_userName;

		private GridColumn column_lastName;

		private GridColumn column_cardNo;

		private GridColumn column_deviceID;

		private GridColumn column_doorNo;

		private GridColumn column_inAddress;

		private GridColumn column_verified;

		private GridColumn column_state;

		private GridColumn column_eventType;

		private GridColumn column_triggerOpt;

		private TextBox txt_userName;

		private Label lbl_userName;

		private GridColumn colTime;

		private RepositoryItemLookUpEdit lueVerifyType;

		private RepositoryItemLookUpEdit lueEventType;

		private DateEdit dtEndTime;

		private DateEdit dtEndDate;

		private DateEdit dtBeginTime;

		private DateEdit dtBeginDate;

		private RepositoryItemLookUpEdit lueUserName;

		private RepositoryItemLookUpEdit lueLastName;

		public ReportsUserControlAll()
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
			}
			this.CheckPermission();
			this.LoadReader();
			this.LoadVerifyType();
			this.LoadEventTypeToTable();
			this.LoadDoor();
			this.ChangeSking();
		}

		public ReportsUserControlAll(string strRecordsType)
			: this()
		{
			try
			{
				this.InitUserTable();
				initLang.LocaleForm(this, base.Name);
				this.strRecordsType = strRecordsType;
				this.LoadTime();
				this.LoadDevInfo();
				this.LoadEventType();
				this.LoadStateInfo();
				this.LoadverifiedInfo();
				this.LoadInAddressInfo();
				this.LoadOutAddressInfo();
				this.LoadlinkAgeIOInfo();
				this.LoadUserInfo();
				this.InitDataTableSet();
				this.DataBind(strRecordsType);
				this.ChangeSking();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInfoLoadError", "数据加载失败"));
			}
			this.CheckPermission();
		}

		private void ChangeSking()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_export.Image = ResourceIPC.exportReport;
				this.btn_clear.Image = ResourceIPC.clear_all_event;
			}
		}

		private void InitUserTable()
		{
			this.dtUser.Columns.Add("pin");
			this.dtUser.Columns.Add("name");
			this.dtUser.Columns.Add("lastname");
		}

		private void LoadVerifyType()
		{
			this.dtVerifyType = new DataTable();
			this.dtVerifyType.Columns.Add("Key", typeof(int));
			this.dtVerifyType.Columns.Add("Value", typeof(string));
			foreach (KeyValuePair<int, string> item in PullSDKVerifyTypeInfos.GetDic())
			{
				DataRow dataRow = this.dtVerifyType.NewRow();
				dataRow["Key"] = item.Key;
				dataRow["Value"] = item.Value;
				this.dtVerifyType.Rows.Add(dataRow);
			}
			this.lueVerifyType.ValueMember = "Key";
			this.lueVerifyType.DisplayMember = "Value";
			this.lueVerifyType.DataSource = this.dtVerifyType;
		}

		private void LoadEventTypeToTable()
		{
			this.dtEventType = new DataTable();
			this.dtEventType.Columns.Add("Key", typeof(int));
			this.dtEventType.Columns.Add("Value", typeof(string));
			foreach (KeyValuePair<int, string> item in PullSDKEventInfos.GetDic())
			{
				DataRow dataRow = this.dtEventType.NewRow();
				dataRow["Key"] = item.Key;
				dataRow["Value"] = item.Value;
				this.dtEventType.Rows.Add(dataRow);
			}
			this.lueEventType.ValueMember = "Key";
			this.lueEventType.DisplayMember = "Value";
			this.lueEventType.DataSource = this.dtEventType.DefaultView;
		}

		private void LoadDoor()
		{
			try
			{
				this.dicDevIdDoorNo_Door = new Dictionary<int, Dictionary<int, AccDoor>>();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				for (int i = 0; i < modelList.Count; i++)
				{
					AccDoor accDoor = modelList[i];
					if (!this.dicDevIdDoorNo_Door.ContainsKey(accDoor.device_id))
					{
						Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
						dictionary.Add(accDoor.door_no, accDoor);
						this.dicDevIdDoorNo_Door.Add(accDoor.device_id, dictionary);
					}
					else
					{
						Dictionary<int, AccDoor> dictionary = this.dicDevIdDoorNo_Door[accDoor.device_id];
						if (!dictionary.ContainsKey(accDoor.door_no))
						{
							dictionary.Add(accDoor.door_no, accDoor);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private void LoadReader()
		{
			try
			{
				AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
				List<AccReader> modelList = accReaderBll.GetModelList("");
				if (modelList != null)
				{
					this.dicDoorIdInOutState_Reader = new Dictionary<int, Dictionary<int, AccReader>>();
					for (int i = 0; i < modelList.Count; i++)
					{
						AccReader accReader = modelList[i];
						if (!this.dicDoorIdInOutState_Reader.ContainsKey(accReader.DoorId))
						{
							Dictionary<int, AccReader> dictionary = new Dictionary<int, AccReader>();
							dictionary.Add((int)accReader.ReaderState, accReader);
							this.dicDoorIdInOutState_Reader.Add(accReader.DoorId, dictionary);
						}
						else
						{
							Dictionary<int, AccReader> dictionary = this.dicDoorIdInOutState_Reader[accReader.DoorId];
							if (!dictionary.ContainsKey((int)accReader.ReaderState))
							{
								dictionary.Add((int)accReader.ReaderState, accReader);
							}
							else
							{
								dictionary[(int)accReader.ReaderState] = accReader;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Report))
			{
				this.btn_export.Enabled = false;
				this.btn_clear.Enabled = false;
			}
		}

		private void LoadTime()
		{
			this.dtBeginTime.EditValue = new DateTime(2000, 1, 1, 0, 0, 0);
			this.dtEndTime.EditValue = new DateTime(2000, 1, 1, 23, 59, 59);
			DateTime dateTime;
			if (this.strRecordsType == "halfweek")
			{
				DateEdit dateEdit = this.dtBeginDate;
				dateTime = DateTime.Now;
				dateEdit.EditValue = dateTime.AddDays(-2.0);
				this.dtEndDate.EditValue = DateTime.Now;
			}
			else if (this.strRecordsType == "week")
			{
				DateTime now = DateTime.Now;
				this.dtBeginDate.EditValue = now.AddDays((double)(1 - Convert.ToInt32(((Enum)(object)now.DayOfWeek).ToString("d"))));
				DateEdit dateEdit2 = this.dtEndDate;
				dateTime = (DateTime)this.dtBeginDate.EditValue;
				dateEdit2.EditValue = dateTime.AddDays(6.0);
			}
			else if (this.strRecordsType == "lastweek")
			{
				DateTime now2 = DateTime.Now;
				DateEdit dateEdit3 = this.dtBeginDate;
				dateTime = DateTime.Now;
				DateTime now3 = DateTime.Now;
				dateEdit3.EditValue = dateTime.AddDays((double)(Convert.ToInt32(1 - Convert.ToInt32(now3.DayOfWeek)) - 7));
				DateEdit dateEdit4 = this.dtEndDate;
				dateTime = DateTime.Now;
				now3 = DateTime.Now;
				dateTime = dateTime.AddDays((double)(Convert.ToInt32(1 - Convert.ToInt32(now3.DayOfWeek)) - 7));
				dateEdit4.EditValue = dateTime.AddDays(6.0);
			}
			else if (this.strRecordsType == "exception")
			{
				DateEdit dateEdit5 = this.dtBeginDate;
				dateTime = DateTime.Now;
				dateEdit5.EditValue = dateTime.AddDays(-2.0);
				this.dtEndDate.EditValue = DateTime.Now;
			}
			else
			{
				this.dtBeginDate.EditValue = DateTime.Now;
				this.dtEndDate.EditValue = DateTime.Now;
			}
		}

		private void LoadDevInfo()
		{
			this.cmb_devName.Items.Clear();
			this.cmb_devName.Items.Add("-----");
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.cmb_devName.Items.Add(list[i].MachineAlias);
				}
			}
			initLang.ComboBoxAutoSize(this.cmb_devName, this);
			this.cmb_devName.SelectedIndex = 0;
		}

		private void InitDataTableSet()
		{
			this.m_dataTable.Columns.Add("time");
			this.m_dataTable.Columns.Add("pin");
			this.m_dataTable.Columns.Add("userName");
			this.m_dataTable.Columns.Add("lastname");
			this.m_dataTable.Columns.Add("card_no");
			this.m_dataTable.Columns.Add("device_id");
			this.m_dataTable.Columns.Add("door_id");
			this.m_dataTable.Columns.Add("in_address");
			this.m_dataTable.Columns.Add("out_address");
			this.m_dataTable.Columns.Add("verified");
			this.m_dataTable.Columns.Add("state");
			this.m_dataTable.Columns.Add("event_type");
			this.m_dataTable.Columns.Add("trigger_opt");
			this.m_dataTable.Columns.Add("event_typeid");
			this.column_checkTime.FieldName = "time";
			this.column_userID.FieldName = "pin";
			this.column_userName.FieldName = "user_name";
			this.column_lastName.FieldName = "user_lastname";
			this.column_cardNo.FieldName = "card_no";
			this.column_deviceID.FieldName = "device_id";
			this.column_doorNo.FieldName = "door_id";
			this.column_inAddress.FieldName = "in_address";
			this.column_outAddress.FieldName = "out_address";
			this.column_verified.FieldName = "verified";
			this.column_state.FieldName = "state";
			this.column_eventType.FieldName = "event_type";
			this.column_triggerOpt.FieldName = "trigger_opt";
		}

		private void GetRecordsByCondtion(string strSql)
		{
			try
			{
				if (this.strRecordsType == "exception")
				{
					this.ds = this.accMonitorLogBll.GetList("event_type>=20 and event_type<=200 and " + strSql);
				}
				else
				{
					this.ds = this.accMonitorLogBll.GetList(strSql);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataBind(string recordsType)
		{
			try
			{
				string text = "";
				DateTime now = DateTime.Now;
				this.btn_clear.Visible = (this.strRecordsType != "exception");
				string text2 = (AppSite.Instance.DataType == DataType.SqlServer) ? "'" : "#";
				DateTime dateTime2;
				DateTime dateTime;
				DateTime dateTime3;
				switch (recordsType)
				{
				case "all":
				{
					string arg = now.ToString("yyyy-MM-dd") + " 00:00:00";
					string arg2 = now.ToString("yyyy-MM-dd") + " 23:59:59";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SAllRecords", "全部门禁事件");
					text = ((AppSite.Instance.DataType != DataType.SqlServer) ? $"[time]>=#{arg}# and [time]<#{arg2}#" : $"[time]>={{ts '{arg}'}} and [time]<{{ts '{arg2}'}}");
					break;
				}
				case "halfweek":
				{
					dateTime2 = now.AddDays(-2.0);
					string arg = dateTime2.ToString("yyyy-MM-dd") + " 00:00:00";
					string arg2 = now.ToString("yyyy-MM-dd") + " 23:59:59";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfhalfWeek", "最近三天门禁事件");
					text = ((AppSite.Instance.DataType != DataType.SqlServer) ? $"[time]>=#{arg}# and [time]<#{arg2}#" : $"[time]>={{ts '{arg}'}} and [time]<{{ts '{arg2}'}}");
					break;
				}
				case "day":
				{
					string arg = now.ToString("yyyy-MM-dd") + " 00:00:00";
					string arg2 = now.ToString("yyyy-MM-dd") + " 23:59:59";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfDay", "当天门禁事件");
					text = ((AppSite.Instance.DataType != DataType.SqlServer) ? $"[time]>=#{arg}# and [time]<#{arg2}#" : $"[time]>={{ts '{arg}'}} and [time]<{{ts '{arg2}'}}");
					break;
				}
				case "exception":
				{
					dateTime2 = now.AddDays(-2.0);
					string arg = dateTime2.ToString("yyyy-MM-dd") + " 00:00:00";
					string arg2 = now.ToString("yyyy-MM-dd") + " 23:59:59";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SExceptionRecords", "门禁异常事件");
					text = ((AppSite.Instance.DataType != DataType.SqlServer) ? $"[time]>=#{arg}# and [time]<#{arg2}#" : $"[time]>={{ts '{arg}'}} and [time]<{{ts '{arg2}'}}");
					text += " and (event_type>=20 and event_type<=200 or event_type = 100001 or event_type = 100032 or event_type = 100034 or event_type = 100055 or event_type = 100058)";
					break;
				}
				case "week":
				{
					dateTime = now.AddDays((double)(1 - Convert.ToInt32(((Enum)(object)now.DayOfWeek).ToString("d"))));
					dateTime3 = dateTime.AddDays(6.0);
					string arg = dateTime.ToString("yyyy-MM-dd") + " 00:00:00";
					string arg2 = dateTime3.ToString("yyyy-MM-dd") + " 23:59:59";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfWeek", "最近一周门禁事件");
					text = ((AppSite.Instance.DataType != DataType.SqlServer) ? $"[time]>=#{arg}# and [time]<#{arg2}#" : $"[time]>={{ts '{arg}'}} and [time]<{{ts '{arg2}'}}");
					break;
				}
				case "lastweek":
				{
					dateTime = now.AddDays((double)(Convert.ToInt32(1 - Convert.ToInt32(now.DayOfWeek)) - 7));
					dateTime2 = now.AddDays((double)(Convert.ToInt32(1 - Convert.ToInt32(now.DayOfWeek)) - 7));
					dateTime3 = dateTime2.AddDays(6.0);
					string arg = dateTime.ToString("yyyy-MM-dd") + " 00:00:00";
					string arg2 = dateTime3.ToString("yyyy-MM-dd") + " 23:59:59";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfLastWeek", "上周门禁事件");
					text = ((AppSite.Instance.DataType != DataType.SqlServer) ? $"[time]>=#{arg}# and [time]<#{arg2}#" : $"[time]>={{ts '{arg}'}} and [time]<{{ts '{arg2}'}}");
					break;
				}
				case "condition":
					text = this.strCondtion;
					if (this.strRecordsType == "exception")
					{
						text += " and (event_type>=20 and event_type<=200 or event_type = 100001 or event_type = 100004 or event_type = 100032 or event_type = 100034 or event_type = 100055 or event_type = 100058 or event_type = 100000)";
					}
					break;
				default:
					text = string.Format(" (event_type>=20 and event_type<=200") + " or event_type = 100001 or event_type = 100004 or event_type = 100032 or event_type = 100034 or event_type = 100055 or event_type = 100058 or event_type = 100000)";
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SExceptionRecords", "门禁异常事件");
					break;
				}
				this.LoadEventOnBindMode(text);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadDataOnServerMode(string sqlWhere)
		{
			this.grd_checkInOutView.DataSource = null;
			this.xpifs = null;
			this.xpifs = new XPInstantFeedbackSource();
			this.xpifs.FixedFilterString = sqlWhere;
			this.xpifs.ObjectType = typeof(MonitorLog_Contact);
			this.xpifs.DisplayableProperties = "time;CheckTime;UserPin;FirstName;LastName;CardNo;device_name;event_point_name;VerifyType;InOutState;EventType;description";
			this.xpifs.ResolveSession += this.xpifs_ResolveSession;
			this.xpifs.DismissSession += this.xpifs_DismissSession;
			this.colTime.SortOrder = ColumnSortOrder.Descending;
			this.colTime.FieldName = "time";
			this.column_checkTime.FieldName = "CheckTime";
			this.column_userID.FieldName = "UserPin";
			this.column_userName.FieldName = "UserPin";
			this.column_lastName.FieldName = "UserPin";
			this.column_cardNo.FieldName = "CardNo";
			this.column_deviceID.FieldName = "device_name";
			this.column_doorNo.FieldName = "event_point_name";
			this.column_inAddress.FieldName = "time";
			this.column_outAddress.FieldName = "out_address";
			this.column_verified.FieldName = "VerifyType";
			this.column_state.FieldName = "InOutState";
			this.column_eventType.FieldName = "EventType";
			this.column_triggerOpt.FieldName = "description";
			this.grd_checkInOutView.DataSource = this.xpifs;
		}

		private void xpifs_DismissSession(object sender, ResolveSessionEventArgs e)
		{
			IDisposable disposable = e.Session as IDisposable;
			disposable?.Dispose();
		}

		private void xpifs_ResolveSession(object sender, ResolveSessionEventArgs e)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			Session session = new Session();
			if (AppSite.Instance.DataType == DataType.Access)
			{
				string connectionString = DbHelperOleDb.connectionString;
				DbHelperOleDb.GetDatabaseAddress(connectionString, ref empty, ref empty2);
				session.ConnectionString = AccessConnectionProvider.GetConnectionString(empty);
			}
			else
			{
				DbHelperSQL.GetDatabaseAddress(DbHelperSQL.connectionString, out bool flag, out string server, out string database, out string userid, out string password);
				if (flag)
				{
					session.ConnectionString = MSSqlConnectionProvider.GetConnectionString(server, database);
				}
				else
				{
					session.ConnectionString = MSSqlConnectionProvider.GetConnectionString(server, userid, password, database);
				}
			}
			session.Connect();
			e.Session = session;
		}

		private void LoadEventOnBindMode(string SqlWhere)
		{
			try
			{
				DataSet list = this.accMonitorLogBll.GetList(SqlWhere);
				if (list != null && list.Tables.Count > 0)
				{
					this.m_dataTable = list.Tables[0];
					this.m_dataTable.Columns.Add("InOutState", typeof(string));
					for (int i = 0; i < this.m_dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_dataTable.Rows[i];
						if (dataRow["pin"].ToString() == "0")
						{
							dataRow["pin"] = DBNull.Value;
						}
						ulong num;
						if (dataRow["card_no"].ToString() == "0")
						{
							dataRow["card_no"] = DBNull.Value;
						}
						else if (AccCommon.CodeVersion == CodeVersionType.JapanAF && ulong.TryParse(dataRow["card_no"].ToString(), out num))
						{
							dataRow["card_no"] = num.ToString("X");
						}
						if (!(dataRow["state"].ToString().Trim() == ""))
						{
							int.TryParse(dataRow["state"].ToString().Trim(), out int key);
							if (this.stateList.ContainsKey(key))
							{
								dataRow["InOutState"] = this.stateList[key];
							}
							int key2;
							if (int.TryParse(dataRow["event_point_type"].ToString(), out int num2) && num2 == 0 && int.TryParse(dataRow["device_id"].ToString(), out key2) && this.dicDevIdDoorNo_Door.ContainsKey(key2))
							{
								Dictionary<int, AccDoor> dictionary = this.dicDevIdDoorNo_Door[key2];
								if (int.TryParse(dataRow["event_point_id"].ToString(), out int key3) && dictionary.ContainsKey(key3))
								{
									AccDoor accDoor = dictionary[key3];
									if (this.dicDoorIdInOutState_Reader.ContainsKey(accDoor.id))
									{
										Dictionary<int, AccReader> dictionary2 = this.dicDoorIdInOutState_Reader[accDoor.id];
										if (dictionary2.ContainsKey(key))
										{
											dataRow["InOutState"] = dictionary2[key].ReaderName;
										}
									}
								}
							}
						}
					}
					this.column_checkTime.SortOrder = ColumnSortOrder.Descending;
					this.column_checkTime.FieldName = "time";
					this.column_userID.FieldName = "pin";
					this.column_userName.FieldName = "user_name";
					this.column_lastName.FieldName = "user_lastname";
					this.column_cardNo.FieldName = "card_no";
					this.column_deviceID.FieldName = "device_name";
					this.column_doorNo.FieldName = "event_point_name";
					this.column_verified.FieldName = "verified";
					this.column_state.FieldName = "InOutState";
					this.column_eventType.FieldName = "event_type";
					this.column_triggerOpt.FieldName = "description";
					this.grd_checkInOutView.DataSource = this.m_dataTable;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadStateInfo()
		{
			this.stateList.Clear();
			this.cmb_inoutState.Items.Clear();
			this.cmb_inoutState.Items.Add("-----");
			this.stateList = InOutStateInfo.GetDic();
			foreach (int key in this.stateList.Keys)
			{
				this.cmb_inoutState.Items.Add(this.stateList[key]);
			}
			initLang.ComboBoxAutoSize(this.cmb_inoutState, this);
			this.cmb_inoutState.SelectedIndex = 0;
		}

		private void LoadverifiedInfo()
		{
			this.verifiedList.Clear();
			this.cmb_verified.Items.Clear();
			this.cmb_verified.Items.Add("-----");
			this.verifiedList = PullSDKVerifyTypeInfos.GetDic();
			foreach (int key in this.verifiedList.Keys)
			{
				this.cmb_verified.Items.Add(this.verifiedList[key]);
			}
			initLang.ComboBoxAutoSize(this.cmb_verified, this);
			this.cmb_verified.SelectedIndex = 0;
		}

		private void LoadInAddressInfo()
		{
			this.inAddressList.Clear();
			this.inAddressList = InAddressInfo.GetDic();
		}

		private void LoadOutAddressInfo()
		{
			this.outAddressList.Clear();
			this.outAddressList = OutAddressInfo.GetDic();
		}

		private void LoadlinkAgeIOInfo()
		{
			this.linkAgeIOList.Clear();
			this.linkAgeIOList = LinkAgeIOInfo.GetDic();
		}

		private void LoadUserInfo()
		{
			try
			{
				this.userInfoDicList.Clear();
				this.userInfoDicListlastName.Clear();
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				List<UserInfo> modelList = userInfoBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].Name == null)
						{
							modelList[i].Name = modelList[i].BadgeNumber;
						}
						if (modelList[i].LastName == null)
						{
							modelList[i].LastName = "";
						}
						this.userInfoDicList.Add(modelList[i].BadgeNumber, modelList[i].Name);
						this.userInfoDicListlastName.Add(modelList[i].BadgeNumber, modelList[i].LastName);
						DataRow dataRow = this.dtUser.NewRow();
						dataRow["pin"] = modelList[i].BadgeNumber;
						dataRow["name"] = modelList[i].Name;
						dataRow["lastname"] = modelList[i].LastName;
						this.dtUser.Rows.Add(dataRow);
						Application.DoEvents();
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadEventType()
		{
			Dictionary<int, string> dic = PullSDKEventInfos.GetDic();
			IOrderedEnumerable<KeyValuePair<int, string>> orderedEnumerable = from kv in dic
			orderby kv.Value
			select kv;
			this.eventTypeList.Clear();
			foreach (KeyValuePair<int, string> item in orderedEnumerable)
			{
				this.eventTypeList.Add(item.Key, item.Value);
			}
			this.cmb_eventType.Items.Clear();
			this.cmb_eventType.Items.Add("-----");
			foreach (int key in this.eventTypeList.Keys)
			{
				if (!this.cmb_eventType.Items.Contains(this.eventTypeList[key]))
				{
					this.cmb_eventType.Items.Add(this.eventTypeList[key]);
				}
			}
			initLang.ComboBoxAutoSize(this.cmb_eventType, this);
			this.cmb_eventType.SelectedIndex = 0;
		}

		private void btn_export_Click(object sender, EventArgs e)
		{
			object[] obj = new object[8]
			{
				this.pnl_reports.Text,
				"_",
				null,
				null,
				null,
				null,
				null,
				null
			};
			DateTime now = DateTime.Now;
			obj[2] = now.Year;
			now = DateTime.Now;
			int num = now.Month;
			obj[3] = num.ToString("00");
			now = DateTime.Now;
			num = now.Day;
			obj[4] = num.ToString("00");
			now = DateTime.Now;
			num = now.Hour;
			obj[5] = num.ToString("00");
			now = DateTime.Now;
			num = now.Minute;
			obj[6] = num.ToString("00");
			now = DateTime.Now;
			obj[7] = now.Second;
			string fileName = string.Concat(obj);
			DevExpressHelper.OutData(this.grd_reportView, fileName);
		}

		private void btn_clear_Click(object sender, EventArgs e)
		{
			ClearEventOption clearEventOption = new ClearEventOption();
			clearEventOption.ShowDialog();
		}

		private void btn_exceptionEvents_Click(object sender, EventArgs e)
		{
			this.DataBind("exception");
		}

		private void btn_allEvents_Click(object sender, EventArgs e)
		{
			this.DataBind("all");
		}

		private void grd_reportView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
		{
		}

		private void btn_recordsWeek_Click(object sender, EventArgs e)
		{
			this.DataBind("week");
		}

		private void btn_RecordsToday_Click(object sender, EventArgs e)
		{
			this.DataBind("day");
		}

		private void grd_reportView_RowStyle(object sender, RowStyleEventArgs e)
		{
			GridView gridView = sender as GridView;
			int num;
			int num2;
			if (e.RowHandle >= 0)
			{
				DataRow dataRow = gridView.GetDataRow(e.RowHandle);
				if (dataRow != null && int.TryParse(dataRow[17].ToString(), out num))
				{
					if (num >= 1000 && num < 10000)
					{
						num -= 1000;
					}
					if ((num < 20 || num >= 100) && num != 230 && num != 100000)
					{
						num2 = ((num == 101001) ? 1 : 0);
						goto IL_00a9;
					}
					num2 = 1;
					goto IL_00a9;
				}
			}
			return;
			IL_0109:
			int num3;
			if (num3 != 0)
			{
				e.Appearance.ForeColor = Color.Red;
			}
			else
			{
				e.Appearance.ForeColor = Color.Green;
			}
			return;
			IL_00a9:
			if (num2 != 0)
			{
				e.Appearance.ForeColor = Color.Orange;
				return;
			}
			if ((num < 100 || num >= 200) && num != 28 && num != 100001 && num != 100032 && num != 100034 && num != 100055)
			{
				num3 = ((num == 100058) ? 1 : 0);
				goto IL_0109;
			}
			num3 = 1;
			goto IL_0109;
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.Check())
				{
					DateTime dateTime = (DateTime)this.dtBeginDate.EditValue;
					DateTime dateTime2 = (DateTime)this.dtEndDate.EditValue;
					DateTime dateTime3 = (DateTime)this.dtBeginTime.EditValue;
					DateTime dateTime4 = (DateTime)this.dtEndTime.EditValue;
					StringBuilder stringBuilder = new StringBuilder();
					if (AppSite.Instance.DataType == DataType.SqlServer)
					{
						stringBuilder.Append("time>=convert(datetime, '" + dateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + " " + dateTime3.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + "', 120) and time<=convert(datetime, '" + dateTime2.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + " " + dateTime4.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + "', 120)");
					}
					else
					{
						stringBuilder.Append("time>=#" + dateTime.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + " " + dateTime3.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + "# and time<=#" + dateTime2.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + " " + dateTime4.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + "#");
					}
					if (!string.IsNullOrEmpty(this.txt_cardNo.Text))
					{
						stringBuilder.Append(" and card_no ='" + this.txt_cardNo.Text + "'");
					}
					if (!string.IsNullOrEmpty(this.txt_userPin.Text))
					{
						stringBuilder.Append(" and pin='" + this.txt_userPin.Text + "'");
					}
					if (!string.IsNullOrEmpty(this.txt_userName.Text))
					{
						stringBuilder.Append(" and user_name like '%" + this.txt_userName.Text + "%'");
					}
					if (!string.IsNullOrEmpty(this.cmb_devName.Text) && this.cmb_devName.Text != "-----")
					{
						stringBuilder.Append(" and device_name='" + this.cmb_devName.Text + "'");
					}
					if (this.cmb_verified.SelectedIndex != 0 && this.verifiedList != null)
					{
						foreach (KeyValuePair<int, string> verified in this.verifiedList)
						{
							if (verified.Value == this.cmb_verified.Text)
							{
								stringBuilder.Append(" and verified=" + verified.Key);
								break;
							}
						}
					}
					if (this.cmb_inoutState.SelectedIndex != 0 && this.stateList != null)
					{
						foreach (KeyValuePair<int, string> state in this.stateList)
						{
							if (state.Value == this.cmb_inoutState.Text)
							{
								stringBuilder.Append(" and state=" + state.Key);
								break;
							}
						}
					}
					if (this.cmb_eventType.SelectedIndex != 0 && this.eventTypeList != null)
					{
						foreach (KeyValuePair<int, string> eventType in this.eventTypeList)
						{
							if (eventType.Value == this.cmb_eventType.Text)
							{
								switch (eventType.Key)
								{
								case 0:
								case 1000:
									stringBuilder.Append(" and event_type in (0,1000)");
									break;
								case 27:
								case 34:
								case 1027:
									stringBuilder.Append(" and event_type in (27,34,1027)");
									break;
								case 40:
								case 1048:
									stringBuilder.Append(" and event_type in (40,1048)");
									break;
								case 100:
								case 100055:
									stringBuilder.Append(" and event_type in (100,100055)");
									break;
								case 102:
								case 100001:
									stringBuilder.Append(" and event_type in (102,100001)");
									break;
								case 201:
								case 100005:
									stringBuilder.Append(" and event_type in (201,100005)");
									break;
								case 202:
								case 100053:
									stringBuilder.Append(" and event_type in (202,100053)");
									break;
								default:
									stringBuilder.Append(" and event_type=" + eventType.Key);
									break;
								}
								break;
							}
						}
					}
					this.strCondtion = stringBuilder.ToString();
					this.DataBind("condition");
					goto end_IL_0001;
				}
				return;
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				this.btn_search.Enabled = true;
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.btn_search.Enabled = true;
		}

		private bool Check()
		{
			DateTime dateTime = (DateTime)this.dtBeginDate.EditValue;
			string str = dateTime.ToString("yyyy-MM-dd");
			dateTime = (DateTime)this.dtBeginTime.EditValue;
			DateTime t = DateTime.Parse(str + " " + dateTime.ToString("HH:mm:ss"));
			dateTime = (DateTime)this.dtEndDate.EditValue;
			string str2 = dateTime.ToString("yyyy-MM-dd");
			dateTime = (DateTime)this.dtEndTime.EditValue;
			DateTime t2 = DateTime.Parse(str2 + " " + dateTime.ToString("HH:mm:ss"));
			if (t > t2)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("BeginTimeCanNotGreaterEndTime", "开始时间不能大于结束时间"));
				this.dtBeginDate.Focus();
				return false;
			}
			return true;
		}

		private void txt_userPin_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 9);
			if (e.KeyChar == '\r')
			{
				this.btn_search_Click(null, null);
			}
		}

		private void txt_cardNo_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 10);
			if (e.KeyChar == '\r')
			{
				this.btn_search_Click(null, null);
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.txt_cardNo.Text = "";
			this.txt_userPin.Text = "";
			this.txt_userName.Text = "";
			this.cmb_devName.SelectedIndex = 0;
			this.cmb_eventType.SelectedIndex = 0;
			this.cmb_inoutState.SelectedIndex = 0;
			this.cmb_verified.SelectedIndex = 0;
			this.LoadTime();
			this.DataBind(this.strRecordsType);
		}

		private void txt_userName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 50);
			if (e.KeyChar == '\r')
			{
				this.btn_search_Click(null, null);
			}
		}

		private void grd_checkInOutView_Click(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReportsUserControlAll));
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			SerializableAppearanceObject appearance2 = new SerializableAppearanceObject();
			this.MenuPanelEx = new ToolStrip();
			this.btn_export = new ToolStripButton();
			this.btn_clear = new ToolStripButton();
			this.pnl_reports = new PanelEx();
			this.gbx_searchCondition = new GroupBox();
			this.dtEndTime = new DateEdit();
			this.dtEndDate = new DateEdit();
			this.dtBeginTime = new DateEdit();
			this.dtBeginDate = new DateEdit();
			this.txt_userName = new TextBox();
			this.lbl_userName = new Label();
			this.btn_cancel = new ButtonX();
			this.btn_search = new ButtonX();
			this.cmb_eventType = new System.Windows.Forms.ComboBox();
			this.lbl_evnetType = new Label();
			this.cmb_inoutState = new System.Windows.Forms.ComboBox();
			this.lbl_inoutState = new Label();
			this.cmb_verified = new System.Windows.Forms.ComboBox();
			this.lbl_verified = new Label();
			this.cmb_devName = new System.Windows.Forms.ComboBox();
			this.lbl_deviceName = new Label();
			this.txt_cardNo = new TextBox();
			this.lbl_cardNo = new Label();
			this.txt_userPin = new TextBox();
			this.lbl_userPin = new Label();
			this.label2 = new Label();
			this.lbl_dateTime = new Label();
			this.grd_checkInOutView = new GridControl();
			this.grd_reportView = new GridView();
			this.column_outAddress = new GridColumn();
			this.column_checkTime = new GridColumn();
			this.column_userID = new GridColumn();
			this.column_userName = new GridColumn();
			this.lueUserName = new RepositoryItemLookUpEdit();
			this.column_lastName = new GridColumn();
			this.lueLastName = new RepositoryItemLookUpEdit();
			this.column_cardNo = new GridColumn();
			this.column_deviceID = new GridColumn();
			this.column_doorNo = new GridColumn();
			this.column_inAddress = new GridColumn();
			this.column_verified = new GridColumn();
			this.lueVerifyType = new RepositoryItemLookUpEdit();
			this.column_state = new GridColumn();
			this.column_eventType = new GridColumn();
			this.lueEventType = new RepositoryItemLookUpEdit();
			this.column_triggerOpt = new GridColumn();
			this.colTime = new GridColumn();
			this.MenuPanelEx.SuspendLayout();
			this.gbx_searchCondition.SuspendLayout();
			((ISupportInitialize)this.dtEndTime.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtEndTime.Properties).BeginInit();
			((ISupportInitialize)this.dtEndDate.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtEndDate.Properties).BeginInit();
			((ISupportInitialize)this.dtBeginTime.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtBeginTime.Properties).BeginInit();
			((ISupportInitialize)this.dtBeginDate.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtBeginDate.Properties).BeginInit();
			((ISupportInitialize)this.grd_checkInOutView).BeginInit();
			((ISupportInitialize)this.grd_reportView).BeginInit();
			((ISupportInitialize)this.lueUserName).BeginInit();
			((ISupportInitialize)this.lueLastName).BeginInit();
			((ISupportInitialize)this.lueVerifyType).BeginInit();
			((ISupportInitialize)this.lueEventType).BeginInit();
			base.SuspendLayout();
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[2]
			{
				this.btn_export,
				this.btn_clear
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(900, 41);
			this.MenuPanelEx.TabIndex = 0;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_export.Image = (Image)componentResourceManager.GetObject("btn_export.Image");
			this.btn_export.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_export.ImageTransparentColor = Color.Magenta;
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(91, 38);
			this.btn_export.Text = "导出报表";
			this.btn_export.Click += this.btn_export_Click;
			this.btn_clear.Image = (Image)componentResourceManager.GetObject("btn_clear.Image");
			this.btn_clear.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_clear.ImageTransparentColor = Color.Magenta;
			this.btn_clear.Name = "btn_clear";
			this.btn_clear.Size = new Size(115, 38);
			this.btn_clear.Text = "清空事件记录";
			this.btn_clear.Click += this.btn_clear_Click;
			this.pnl_reports.CanvasColor = SystemColors.Control;
			this.pnl_reports.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_reports.Dock = DockStyle.Top;
			this.pnl_reports.Location = new Point(0, 41);
			this.pnl_reports.Name = "pnl_reports";
			this.pnl_reports.Size = new Size(900, 25);
			this.pnl_reports.Style.Alignment = StringAlignment.Center;
			this.pnl_reports.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_reports.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_reports.Style.Border = eBorderType.SingleLine;
			this.pnl_reports.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_reports.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_reports.Style.GradientAngle = 90;
			this.pnl_reports.TabIndex = 1;
			this.pnl_reports.Text = "报表";
			this.gbx_searchCondition.BackColor = Color.White;
			this.gbx_searchCondition.Controls.Add(this.dtEndTime);
			this.gbx_searchCondition.Controls.Add(this.dtEndDate);
			this.gbx_searchCondition.Controls.Add(this.dtBeginTime);
			this.gbx_searchCondition.Controls.Add(this.dtBeginDate);
			this.gbx_searchCondition.Controls.Add(this.txt_userName);
			this.gbx_searchCondition.Controls.Add(this.lbl_userName);
			this.gbx_searchCondition.Controls.Add(this.btn_cancel);
			this.gbx_searchCondition.Controls.Add(this.btn_search);
			this.gbx_searchCondition.Controls.Add(this.cmb_eventType);
			this.gbx_searchCondition.Controls.Add(this.lbl_evnetType);
			this.gbx_searchCondition.Controls.Add(this.cmb_inoutState);
			this.gbx_searchCondition.Controls.Add(this.lbl_inoutState);
			this.gbx_searchCondition.Controls.Add(this.cmb_verified);
			this.gbx_searchCondition.Controls.Add(this.lbl_verified);
			this.gbx_searchCondition.Controls.Add(this.cmb_devName);
			this.gbx_searchCondition.Controls.Add(this.lbl_deviceName);
			this.gbx_searchCondition.Controls.Add(this.txt_cardNo);
			this.gbx_searchCondition.Controls.Add(this.lbl_cardNo);
			this.gbx_searchCondition.Controls.Add(this.txt_userPin);
			this.gbx_searchCondition.Controls.Add(this.lbl_userPin);
			this.gbx_searchCondition.Controls.Add(this.label2);
			this.gbx_searchCondition.Controls.Add(this.lbl_dateTime);
			this.gbx_searchCondition.Dock = DockStyle.Top;
			this.gbx_searchCondition.Location = new Point(0, 66);
			this.gbx_searchCondition.Name = "gbx_searchCondition";
			this.gbx_searchCondition.Size = new Size(900, 163);
			this.gbx_searchCondition.TabIndex = 2;
			this.gbx_searchCondition.TabStop = false;
			this.gbx_searchCondition.Text = "查询";
			this.dtEndTime.EditValue = null;
			this.dtEndTime.Location = new Point(502, 24);
			this.dtEndTime.Name = "dtEndTime";
			this.dtEndTime.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance, "", null, null, true)
			});
			this.dtEndTime.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtEndTime.Properties.Mask.EditMask = "HH:mm";
			this.dtEndTime.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtEndTime.Size = new Size(69, 20);
			this.dtEndTime.TabIndex = 5;
			this.dtEndDate.EditValue = null;
			this.dtEndDate.Location = new Point(375, 23);
			this.dtEndDate.Name = "dtEndDate";
			this.dtEndDate.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dtEndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtEndDate.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.dtEndDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtEndDate.Size = new Size(121, 20);
			this.dtEndDate.TabIndex = 4;
			this.dtBeginTime.EditValue = null;
			this.dtBeginTime.Location = new Point(258, 24);
			this.dtBeginTime.Name = "dtBeginTime";
			this.dtBeginTime.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance2, "", null, null, true)
			});
			this.dtBeginTime.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtBeginTime.Properties.Mask.EditMask = "HH:mm";
			this.dtBeginTime.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtBeginTime.Size = new Size(69, 20);
			this.dtBeginTime.TabIndex = 2;
			this.dtBeginDate.EditValue = null;
			this.dtBeginDate.Location = new Point(133, 24);
			this.dtBeginDate.Name = "dtBeginDate";
			this.dtBeginDate.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dtBeginDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtBeginDate.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.dtBeginDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtBeginDate.Size = new Size(121, 20);
			this.dtBeginDate.TabIndex = 1;
			this.txt_userName.Location = new Point(739, 62);
			this.txt_userName.Name = "txt_userName";
			this.txt_userName.Size = new Size(121, 20);
			this.txt_userName.TabIndex = 13;
			this.txt_userName.KeyPress += this.txt_userName_KeyPress;
			this.lbl_userName.Location = new Point(632, 65);
			this.lbl_userName.Name = "lbl_userName";
			this.lbl_userName.Size = new Size(101, 14);
			this.lbl_userName.TabIndex = 12;
			this.lbl_userName.Text = "人员姓名";
			this.lbl_userName.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(471, 131);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(103, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 21;
			this.btn_cancel.Text = "清除";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_search.AccessibleRole = AccessibleRole.PushButton;
			this.btn_search.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.btn_search.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_search.Location = new Point(348, 131);
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(99, 25);
			this.btn_search.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_search.TabIndex = 20;
			this.btn_search.Text = "查询";
			this.btn_search.Click += this.btn_search_Click;
			this.cmb_eventType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_eventType.FormattingEnabled = true;
			this.cmb_eventType.Location = new Point(464, 99);
			this.cmb_eventType.Name = "cmb_eventType";
			this.cmb_eventType.Size = new Size(121, 21);
			this.cmb_eventType.TabIndex = 17;
			this.lbl_evnetType.Location = new Point(357, 102);
			this.lbl_evnetType.Name = "lbl_evnetType";
			this.lbl_evnetType.Size = new Size(101, 13);
			this.lbl_evnetType.TabIndex = 16;
			this.lbl_evnetType.Text = "事件类型";
			this.lbl_evnetType.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_inoutState.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_inoutState.FormattingEnabled = true;
			this.cmb_inoutState.Location = new Point(133, 99);
			this.cmb_inoutState.Name = "cmb_inoutState";
			this.cmb_inoutState.Size = new Size(121, 21);
			this.cmb_inoutState.TabIndex = 15;
			this.lbl_inoutState.Location = new Point(16, 102);
			this.lbl_inoutState.Name = "lbl_inoutState";
			this.lbl_inoutState.Size = new Size(101, 13);
			this.lbl_inoutState.TabIndex = 14;
			this.lbl_inoutState.Text = "出入状态";
			this.lbl_inoutState.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_verified.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_verified.FormattingEnabled = true;
			this.cmb_verified.Location = new Point(739, 99);
			this.cmb_verified.Name = "cmb_verified";
			this.cmb_verified.Size = new Size(121, 21);
			this.cmb_verified.TabIndex = 19;
			this.lbl_verified.Location = new Point(632, 102);
			this.lbl_verified.Name = "lbl_verified";
			this.lbl_verified.Size = new Size(101, 13);
			this.lbl_verified.TabIndex = 18;
			this.lbl_verified.Text = "验证方式";
			this.lbl_verified.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_devName.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_devName.FormattingEnabled = true;
			this.cmb_devName.Location = new Point(464, 62);
			this.cmb_devName.Name = "cmb_devName";
			this.cmb_devName.Size = new Size(121, 21);
			this.cmb_devName.TabIndex = 11;
			this.lbl_deviceName.Location = new Point(357, 65);
			this.lbl_deviceName.Name = "lbl_deviceName";
			this.lbl_deviceName.Size = new Size(101, 13);
			this.lbl_deviceName.TabIndex = 10;
			this.lbl_deviceName.Text = "设备名称";
			this.lbl_deviceName.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_cardNo.Location = new Point(133, 62);
			this.txt_cardNo.Name = "txt_cardNo";
			this.txt_cardNo.Size = new Size(121, 20);
			this.txt_cardNo.TabIndex = 9;
			this.txt_cardNo.KeyPress += this.txt_cardNo_KeyPress;
			this.lbl_cardNo.Location = new Point(16, 65);
			this.lbl_cardNo.Name = "lbl_cardNo";
			this.lbl_cardNo.Size = new Size(92, 13);
			this.lbl_cardNo.TabIndex = 8;
			this.lbl_cardNo.Text = "卡号";
			this.lbl_cardNo.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_userPin.Location = new Point(739, 25);
			this.txt_userPin.Name = "txt_userPin";
			this.txt_userPin.Size = new Size(121, 20);
			this.txt_userPin.TabIndex = 7;
			this.txt_userPin.KeyPress += this.txt_userPin_KeyPress;
			this.lbl_userPin.Location = new Point(632, 28);
			this.lbl_userPin.Name = "lbl_userPin";
			this.lbl_userPin.Size = new Size(101, 14);
			this.lbl_userPin.TabIndex = 6;
			this.lbl_userPin.Text = "人员编号";
			this.lbl_userPin.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(346, 28);
			this.label2.Name = "label2";
			this.label2.Size = new Size(16, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "---";
			this.lbl_dateTime.Location = new Point(16, 28);
			this.lbl_dateTime.Name = "lbl_dateTime";
			this.lbl_dateTime.Size = new Size(101, 13);
			this.lbl_dateTime.TabIndex = 0;
			this.lbl_dateTime.Text = "起止时间";
			this.lbl_dateTime.TextAlign = ContentAlignment.MiddleLeft;
			this.grd_checkInOutView.Cursor = Cursors.Default;
			this.grd_checkInOutView.Dock = DockStyle.Fill;
			this.grd_checkInOutView.Location = new Point(0, 229);
			this.grd_checkInOutView.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_checkInOutView.MainView = this.grd_reportView;
			this.grd_checkInOutView.Name = "grd_checkInOutView";
			this.grd_checkInOutView.RepositoryItems.AddRange(new RepositoryItem[4]
			{
				this.lueVerifyType,
				this.lueEventType,
				this.lueUserName,
				this.lueLastName
			});
			this.grd_checkInOutView.Size = new Size(900, 313);
			this.grd_checkInOutView.TabIndex = 3;
			this.grd_checkInOutView.TabStop = false;
			this.grd_checkInOutView.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_reportView
			});
			this.grd_checkInOutView.Click += this.grd_checkInOutView_Click;
			this.grd_reportView.Columns.AddRange(new GridColumn[14]
			{
				this.column_outAddress,
				this.column_checkTime,
				this.column_userID,
				this.column_userName,
				this.column_lastName,
				this.column_cardNo,
				this.column_deviceID,
				this.column_doorNo,
				this.column_inAddress,
				this.column_verified,
				this.column_state,
				this.column_eventType,
				this.column_triggerOpt,
				this.colTime
			});
			this.grd_reportView.GridControl = this.grd_checkInOutView;
			this.grd_reportView.IndicatorWidth = 35;
			this.grd_reportView.Name = "grd_reportView";
			this.grd_reportView.OptionsBehavior.Editable = false;
			this.grd_reportView.OptionsView.ShowGroupPanel = false;
			this.grd_reportView.RowStyle += this.grd_reportView_RowStyle;
			this.column_outAddress.Caption = "辅助输出点";
			this.column_outAddress.Name = "column_outAddress";
			this.column_outAddress.Width = 48;
			this.column_checkTime.Caption = "时间";
			this.column_checkTime.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
			this.column_checkTime.DisplayFormat.FormatType = FormatType.DateTime;
			this.column_checkTime.Name = "column_checkTime";
			this.column_checkTime.ShowUnboundExpressionMenu = true;
			this.column_checkTime.Visible = true;
			this.column_checkTime.VisibleIndex = 0;
			this.column_checkTime.Width = 124;
			this.column_userID.Caption = "人员编号";
			this.column_userID.Name = "column_userID";
			this.column_userID.Visible = true;
			this.column_userID.VisibleIndex = 1;
			this.column_userID.Width = 69;
			this.column_userName.Caption = "姓名";
			this.column_userName.Name = "column_userName";
			this.column_userName.Visible = true;
			this.column_userName.VisibleIndex = 2;
			this.column_userName.Width = 55;
			this.lueUserName.AutoHeight = false;
			this.lueUserName.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueUserName.Name = "lueUserName";
			this.lueUserName.NullText = "";
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_lastName.Width = 63;
			this.lueLastName.AutoHeight = false;
			this.lueLastName.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueLastName.Name = "lueLastName";
			this.lueLastName.NullText = "";
			this.column_cardNo.Caption = "卡号";
			this.column_cardNo.Name = "column_cardNo";
			this.column_cardNo.Visible = true;
			this.column_cardNo.VisibleIndex = 4;
			this.column_cardNo.Width = 58;
			this.column_deviceID.Caption = "设备名称";
			this.column_deviceID.Name = "column_deviceID";
			this.column_deviceID.Visible = true;
			this.column_deviceID.VisibleIndex = 5;
			this.column_deviceID.Width = 74;
			this.column_doorNo.Caption = "事件点";
			this.column_doorNo.Name = "column_doorNo";
			this.column_doorNo.Visible = true;
			this.column_doorNo.VisibleIndex = 6;
			this.column_doorNo.Width = 81;
			this.column_inAddress.Caption = "辅助输入点";
			this.column_inAddress.Name = "column_inAddress";
			this.column_inAddress.Width = 48;
			this.column_verified.Caption = "验证方式";
			this.column_verified.ColumnEdit = this.lueVerifyType;
			this.column_verified.Name = "column_verified";
			this.column_verified.Visible = true;
			this.column_verified.VisibleIndex = 7;
			this.column_verified.Width = 62;
			this.lueVerifyType.AutoHeight = false;
			this.lueVerifyType.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueVerifyType.Name = "lueVerifyType";
			this.lueVerifyType.NullText = "";
			this.column_state.Caption = "出入状态";
			this.column_state.Name = "column_state";
			this.column_state.Visible = true;
			this.column_state.VisibleIndex = 8;
			this.column_state.Width = 56;
			this.column_eventType.Caption = "事件类型";
			this.column_eventType.ColumnEdit = this.lueEventType;
			this.column_eventType.Name = "column_eventType";
			this.column_eventType.Visible = true;
			this.column_eventType.VisibleIndex = 9;
			this.column_eventType.Width = 86;
			this.lueEventType.AutoHeight = false;
			this.lueEventType.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueEventType.Name = "lueEventType";
			this.lueEventType.NullText = "";
			this.column_triggerOpt.Caption = "备注";
			this.column_triggerOpt.Name = "column_triggerOpt";
			this.column_triggerOpt.Visible = true;
			this.column_triggerOpt.VisibleIndex = 10;
			this.column_triggerOpt.Width = 135;
			this.colTime.Name = "colTime";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_checkInOutView);
			base.Controls.Add(this.gbx_searchCondition);
			base.Controls.Add(this.pnl_reports);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "ReportsUserControlAll";
			base.Size = new Size(900, 542);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			this.gbx_searchCondition.ResumeLayout(false);
			this.gbx_searchCondition.PerformLayout();
			((ISupportInitialize)this.dtEndTime.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.dtEndTime.Properties).EndInit();
			((ISupportInitialize)this.dtEndDate.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.dtEndDate.Properties).EndInit();
			((ISupportInitialize)this.dtBeginTime.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.dtBeginTime.Properties).EndInit();
			((ISupportInitialize)this.dtBeginDate.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.dtBeginDate.Properties).EndInit();
			((ISupportInitialize)this.grd_checkInOutView).EndInit();
			((ISupportInitialize)this.grd_reportView).EndInit();
			((ISupportInitialize)this.lueUserName).EndInit();
			((ISupportInitialize)this.lueLastName).EndInit();
			((ISupportInitialize)this.lueVerifyType).EndInit();
			((ISupportInitialize)this.lueEventType).EndInit();
			base.ResumeLayout(false);
		}
	}
}
