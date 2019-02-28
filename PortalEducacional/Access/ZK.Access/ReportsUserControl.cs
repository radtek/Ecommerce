/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class ReportsUserControl : UserControl
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

		private DataTable m_dataTable = new DataTable();

		private IContainer components = null;

		private ToolStripButton btn_export;

		private ToolStripButton btn_clear;

		private ToolStripButton btn_search;

		public PanelEx pnl_reports;

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

		public ToolStrip MenuPanelEx;

		private Timer timer1;

		public ReportsUserControl()
		{
			this.InitializeComponent();
			this.CheckPermission();
		}

		public ReportsUserControl(string strRecordsType)
			: this()
		{
			try
			{
				initLang.LocaleForm(this, base.Name);
				this.strRecordsType = strRecordsType;
				this.LoadEventType();
				this.LoadStateInfo();
				this.LoadverifiedInfo();
				this.LoadInAddressInfo();
				this.LoadOutAddressInfo();
				this.LoadlinkAgeIOInfo();
				this.LoadUserInfo();
				this.InitDataTableSet();
				this.DataBind(strRecordsType);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInfoLoadError", "数据加载失败"));
			}
			this.CheckPermission();
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Report))
			{
				this.btn_export.Enabled = false;
				this.btn_clear.Enabled = false;
			}
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
				DateTime now = DateTime.Now;
				int num;
				DateTime now2;
				if (this.strRecordsType == "all")
				{
					this.ds = this.accMonitorLogBll.GetList(strSql);
				}
				else if (this.strRecordsType == "exception")
				{
					this.ds = this.accMonitorLogBll.GetList("event_type>=20 and event_type<=200 and " + strSql);
				}
				else if (this.strRecordsType == "day")
				{
					string[] obj = new string[6];
					num = now.Year;
					obj[0] = num.ToString();
					obj[1] = "-";
					num = now.Month;
					obj[2] = num.ToString();
					obj[3] = "-";
					num = now.Day;
					obj[4] = num.ToString();
					obj[5] = " 00:00:00";
					string text = string.Concat(obj);
					string[] obj2 = new string[6];
					num = now.Year;
					obj2[0] = num.ToString();
					obj2[1] = "-";
					num = now.Month;
					obj2[2] = num.ToString();
					obj2[3] = "-";
					num = now.Day;
					obj2[4] = num.ToString();
					obj2[5] = " 23:59:00";
					string text2 = string.Concat(obj2);
					if (AppSite.Instance.DataType == DataType.Access)
					{
						this.ds = this.accMonitorLogBll.GetList("time>=#" + text + "# and time<=#" + text2 + "# and " + strSql);
					}
					else
					{
						this.ds = this.accMonitorLogBll.GetList("time>='" + text + "' and time<='" + text2 + "' and " + strSql);
					}
				}
				else if (this.strRecordsType == "halfweek")
				{
					now2 = DateTime.Now;
					DateTime dateTime = now2.AddDays(-3.0);
					string[] obj3 = new string[6];
					num = dateTime.Year;
					obj3[0] = num.ToString();
					obj3[1] = "-";
					num = dateTime.Month;
					obj3[2] = num.ToString();
					obj3[3] = "-";
					num = dateTime.Day;
					obj3[4] = num.ToString();
					obj3[5] = " 00:00:00";
					string text = string.Concat(obj3);
					string[] obj4 = new string[6];
					num = now.Year;
					obj4[0] = num.ToString();
					obj4[1] = "-";
					num = now.Month;
					obj4[2] = num.ToString();
					obj4[3] = "-";
					num = now.Day;
					obj4[4] = num.ToString();
					obj4[5] = " 23:59:00";
					string text2 = string.Concat(obj4);
					if (AppSite.Instance.DataType == DataType.Access)
					{
						this.ds = this.accMonitorLogBll.GetList("time>=#" + text + "# and time<=#" + text2 + "# and " + strSql);
					}
					else
					{
						this.ds = this.accMonitorLogBll.GetList("time>='" + text + "' and time<='" + text2 + "' and " + strSql);
					}
				}
				else
				{
					now2 = DateTime.Now;
					DateTime dateTime2 = now2.AddDays(-7.0);
					string[] obj5 = new string[6];
					num = dateTime2.Year;
					obj5[0] = num.ToString();
					obj5[1] = "-";
					num = dateTime2.Month;
					obj5[2] = num.ToString();
					obj5[3] = "-";
					num = dateTime2.Day;
					obj5[4] = num.ToString();
					obj5[5] = " 00:00:00";
					string text = string.Concat(obj5);
					string[] obj6 = new string[6];
					num = now.Year;
					obj6[0] = num.ToString();
					obj6[1] = "-";
					num = now.Month;
					obj6[2] = num.ToString();
					obj6[3] = "-";
					num = now.Day;
					obj6[4] = num.ToString();
					obj6[5] = " 23:59:00";
					string text2 = string.Concat(obj6);
					if (AppSite.Instance.DataType == DataType.Access)
					{
						this.ds = this.accMonitorLogBll.GetList("time>=#" + text + "# and time<=#" + text2 + "# and " + strSql);
					}
					else
					{
						this.ds = this.accMonitorLogBll.GetList("time>='" + text + "' and time<='" + text2 + "' and " + strSql);
					}
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
				this.m_dataTable.Rows.Clear();
				if (this.strRecordsType == "all")
				{
					this.btn_clear.Visible = true;
				}
				else
				{
					this.btn_clear.Visible = false;
				}
				DateTime now2;
				int num;
				if (recordsType == "all")
				{
					this.ds = this.accMonitorLogBll.GetAllList();
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SAllRecords", "全部门禁事件");
				}
				else if (recordsType == "halfweek")
				{
					DateTime now = DateTime.Now;
					now2 = DateTime.Now;
					DateTime dateTime = now2.AddDays(-3.0);
					string[] obj = new string[6];
					num = dateTime.Year;
					obj[0] = num.ToString();
					obj[1] = "-";
					num = dateTime.Month;
					obj[2] = num.ToString();
					obj[3] = "-";
					num = dateTime.Day;
					obj[4] = num.ToString();
					obj[5] = " 00:00:00";
					string text = string.Concat(obj);
					string[] obj2 = new string[6];
					num = now.Year;
					obj2[0] = num.ToString();
					obj2[1] = "-";
					num = now.Month;
					obj2[2] = num.ToString();
					obj2[3] = "-";
					num = now.Day;
					obj2[4] = num.ToString();
					obj2[5] = " 23:59:00";
					string text2 = string.Concat(obj2);
					if (AppSite.Instance.DataType == DataType.Access)
					{
						this.ds = this.accMonitorLogBll.GetList("time>=#" + text + "# and time<=#" + text2 + "#");
					}
					else
					{
						this.ds = this.accMonitorLogBll.GetList("time>='" + text + "' and time<='" + text2 + "'");
					}
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfHalfWeek", "最近三天门禁事件");
				}
				else if (recordsType == "day")
				{
					DateTime now3 = DateTime.Now;
					string[] obj3 = new string[6];
					num = now3.Year;
					obj3[0] = num.ToString();
					obj3[1] = "-";
					num = now3.Month;
					obj3[2] = num.ToString();
					obj3[3] = "-";
					num = now3.Day;
					obj3[4] = num.ToString();
					obj3[5] = " 00:00:00";
					string text3 = string.Concat(obj3);
					string[] obj4 = new string[6];
					num = now3.Year;
					obj4[0] = num.ToString();
					obj4[1] = "-";
					num = now3.Month;
					obj4[2] = num.ToString();
					obj4[3] = "-";
					num = now3.Day;
					obj4[4] = num.ToString();
					obj4[5] = " 23:59:00";
					string text4 = string.Concat(obj4);
					DateTime dateTime2 = DateTime.Parse(text3);
					if (AppSite.Instance.DataType == DataType.Access)
					{
						this.ds = this.accMonitorLogBll.GetList("time>=#" + text3 + "# and time<=#" + text4 + "#");
					}
					else
					{
						this.ds = this.accMonitorLogBll.GetList("time>='" + text3 + "' and time<='" + text4 + "'");
					}
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfDay", "当天门禁事件");
				}
				else if (recordsType == "exception")
				{
					this.ds = this.accMonitorLogBll.GetList("event_type>=20 and event_type<=200");
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SExceptionRecords", "门禁异常事件");
				}
				else if (recordsType == "week")
				{
					DateTime now4 = DateTime.Now;
					now2 = DateTime.Now;
					DateTime dateTime3 = now2.AddDays(-7.0);
					string[] obj5 = new string[6];
					num = dateTime3.Year;
					obj5[0] = num.ToString();
					obj5[1] = "-";
					num = dateTime3.Month;
					obj5[2] = num.ToString();
					obj5[3] = "-";
					num = dateTime3.Day;
					obj5[4] = num.ToString();
					obj5[5] = " 00:00:00";
					string text5 = string.Concat(obj5);
					string[] obj6 = new string[6];
					num = now4.Year;
					obj6[0] = num.ToString();
					obj6[1] = "-";
					num = now4.Month;
					obj6[2] = num.ToString();
					obj6[3] = "-";
					num = now4.Day;
					obj6[4] = num.ToString();
					obj6[5] = " 23:59:00";
					string text6 = string.Concat(obj6);
					if (AppSite.Instance.DataType == DataType.Access)
					{
						this.ds = this.accMonitorLogBll.GetList("time>=#" + text5 + "# and time<=#" + text6 + "#");
					}
					else
					{
						this.ds = this.accMonitorLogBll.GetList("time>='" + text5 + "' and time<='" + text6 + "'");
					}
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SRecordsOfWeek", "最近一周门禁事件");
				}
				else if (recordsType == "condition")
				{
					this.GetRecordsByCondtion(this.strCondtion);
				}
				else
				{
					this.ds = this.accMonitorLogBll.GetList("event_type>=20 and event_type<=200");
					this.pnl_reports.Text = ShowMsgInfos.GetInfo("SExceptionRecords", "门禁异常事件");
				}
				if (this.ds != null && this.ds.Tables.Count > 0)
				{
					DataTable dataTable = this.ds.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_dataTable.NewRow();
						dataRow[0] = dataTable.Rows[i]["time"].ToString();
						if (dataTable.Rows[i]["pin"].ToString() == "0")
						{
							dataRow[1] = "";
						}
						else
						{
							dataRow[1] = dataTable.Rows[i]["pin"].ToString();
						}
						dataRow[2] = dataTable.Rows[i]["user_name"].ToString();
						dataRow[3] = dataTable.Rows[i]["user_lastname"].ToString();
						if (dataTable.Rows[i]["card_no"].ToString() == "0")
						{
							dataRow[4] = "";
						}
						else
						{
							dataRow[4] = dataTable.Rows[i]["card_no"].ToString();
						}
						dataRow[5] = dataTable.Rows[i]["device_name"].ToString();
						if (dataTable.Rows[i]["door_id"].ToString() == "0")
						{
							dataRow[6] = "";
						}
						else
						{
							dataRow[6] = dataTable.Rows[i]["door_name"].ToString();
						}
						if (this.inAddressList.ContainsKey(int.Parse(dataTable.Rows[i]["in_address"].ToString())))
						{
							dataRow[7] = this.inAddressList[int.Parse(dataTable.Rows[i]["in_address"].ToString())];
						}
						else
						{
							dataRow[7] = "";
						}
						if (this.outAddressList.ContainsKey(int.Parse(dataTable.Rows[i]["out_address"].ToString())))
						{
							dataRow[8] = this.outAddressList[int.Parse(dataTable.Rows[i]["out_address"].ToString())];
						}
						else
						{
							dataRow[8] = "";
						}
						if (this.verifiedList.ContainsKey(int.Parse(dataTable.Rows[i]["verified"].ToString())))
						{
							dataRow[9] = this.verifiedList[int.Parse(dataTable.Rows[i]["verified"].ToString())];
						}
						else
						{
							dataRow[9] = "";
						}
						if (this.stateList.ContainsKey(int.Parse(dataTable.Rows[i]["state"].ToString())))
						{
							dataRow[10] = this.stateList[int.Parse(dataTable.Rows[i]["state"].ToString())];
						}
						else
						{
							dataRow[10] = "";
						}
						if (this.eventTypeList.ContainsKey(int.Parse(dataTable.Rows[i]["event_type"].ToString())))
						{
							dataRow[11] = this.eventTypeList[int.Parse(dataTable.Rows[i]["event_type"].ToString())];
						}
						else
						{
							dataRow[11] = "";
						}
						if (this.linkAgeIOList.ContainsKey(int.Parse(dataTable.Rows[i]["trigger_opt"].ToString())))
						{
							dataRow[12] = this.linkAgeIOList[int.Parse(dataTable.Rows[i]["trigger_opt"].ToString())];
						}
						else
						{
							dataRow[12] = "";
						}
						dataRow[13] = dataTable.Rows[i]["event_type"].ToString();
						this.m_dataTable.Rows.Add(dataRow);
					}
				}
				this.grd_checkInOutView.DataSource = this.m_dataTable;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadStateInfo()
		{
			this.stateList.Clear();
			this.stateList = InOutStateInfo.GetDic();
		}

		private void LoadverifiedInfo()
		{
			this.verifiedList.Clear();
			this.verifiedList = PullSDKVerifyTypeInfos.GetDic();
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
						this.userInfoDicList.Add(modelList[i].BadgeNumber, modelList[i].Name);
						this.userInfoDicListlastName.Add(modelList[i].BadgeNumber, modelList[i].LastName);
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
			this.eventTypeList.Clear();
			this.eventTypeList = PullSDKEventInfos.GetDic();
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
			if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteDataAll", "是否") + this.btn_clear.Text, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes && this.accMonitorLogBll.DeleteAll())
			{
				this.DataBind("all");
			}
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

		private void btn_search_Click(object sender, EventArgs e)
		{
			try
			{
				ReportSearchForm reportSearchForm = new ReportSearchForm();
				reportSearchForm.ShowDialog();
				if (reportSearchForm.DialogResult == DialogResult.OK)
				{
					this.strCondtion = reportSearchForm.sqlConditionStr;
					this.DataBind("condition");
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void grd_reportView_RowStyle(object sender, RowStyleEventArgs e)
		{
		}

		private void ReportsUserControl_Load(object sender, EventArgs e)
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReportsUserControl));
			this.MenuPanelEx = new ToolStrip();
			this.btn_export = new ToolStripButton();
			this.btn_clear = new ToolStripButton();
			this.btn_search = new ToolStripButton();
			this.pnl_reports = new PanelEx();
			this.grd_checkInOutView = new GridControl();
			this.grd_reportView = new GridView();
			this.column_outAddress = new GridColumn();
			this.column_checkTime = new GridColumn();
			this.column_userID = new GridColumn();
			this.column_userName = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_cardNo = new GridColumn();
			this.column_deviceID = new GridColumn();
			this.column_doorNo = new GridColumn();
			this.column_inAddress = new GridColumn();
			this.column_verified = new GridColumn();
			this.column_state = new GridColumn();
			this.column_eventType = new GridColumn();
			this.column_triggerOpt = new GridColumn();
			this.timer1 = new Timer(this.components);
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_checkInOutView).BeginInit();
			((ISupportInitialize)this.grd_reportView).BeginInit();
			base.SuspendLayout();
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[3]
			{
				this.btn_export,
				this.btn_clear,
				this.btn_search
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 38);
			this.MenuPanelEx.TabIndex = 17;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_export.Image = (Image)componentResourceManager.GetObject("btn_export.Image");
			this.btn_export.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_export.ImageTransparentColor = Color.Magenta;
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(89, 35);
			this.btn_export.Text = "导出报表";
			this.btn_export.Click += this.btn_export_Click;
			this.btn_clear.Image = (Image)componentResourceManager.GetObject("btn_clear.Image");
			this.btn_clear.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_clear.ImageTransparentColor = Color.Magenta;
			this.btn_clear.Name = "btn_clear";
			this.btn_clear.Size = new Size(137, 35);
			this.btn_clear.Text = "清空全部事件记录";
			this.btn_clear.Click += this.btn_clear_Click;
			this.btn_search.Image = (Image)componentResourceManager.GetObject("btn_search.Image");
			this.btn_search.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_search.ImageTransparentColor = Color.Magenta;
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(65, 35);
			this.btn_search.Text = "查找";
			this.btn_search.Click += this.btn_search_Click;
			this.pnl_reports.CanvasColor = SystemColors.Control;
			this.pnl_reports.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_reports.Dock = DockStyle.Top;
			this.pnl_reports.Location = new Point(0, 38);
			this.pnl_reports.Name = "pnl_reports";
			this.pnl_reports.Size = new Size(684, 23);
			this.pnl_reports.Style.Alignment = StringAlignment.Center;
			this.pnl_reports.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_reports.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_reports.Style.Border = eBorderType.SingleLine;
			this.pnl_reports.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_reports.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_reports.Style.GradientAngle = 90;
			this.pnl_reports.TabIndex = 18;
			this.pnl_reports.Text = "报表";
			this.grd_checkInOutView.Dock = DockStyle.Fill;
			this.grd_checkInOutView.Location = new Point(0, 61);
			this.grd_checkInOutView.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_checkInOutView.MainView = this.grd_reportView;
			this.grd_checkInOutView.Name = "grd_checkInOutView";
			this.grd_checkInOutView.Size = new Size(684, 439);
			this.grd_checkInOutView.TabIndex = 19;
			this.grd_checkInOutView.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_reportView
			});
			this.grd_reportView.Columns.AddRange(new GridColumn[13]
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
				this.column_triggerOpt
			});
			this.grd_reportView.GridControl = this.grd_checkInOutView;
			this.grd_reportView.IndicatorWidth = 35;
			this.grd_reportView.Name = "grd_reportView";
			this.grd_reportView.OptionsBehavior.Editable = false;
			this.column_outAddress.Caption = "辅助输出点";
			this.column_outAddress.Name = "column_outAddress";
			this.column_outAddress.Visible = true;
			this.column_outAddress.VisibleIndex = 8;
			this.column_outAddress.Width = 39;
			this.column_checkTime.Caption = "时间";
			this.column_checkTime.Name = "column_checkTime";
			this.column_checkTime.ShowUnboundExpressionMenu = true;
			this.column_checkTime.Visible = true;
			this.column_checkTime.VisibleIndex = 0;
			this.column_checkTime.Width = 80;
			this.column_userID.Caption = "人员编号";
			this.column_userID.Name = "column_userID";
			this.column_userID.Visible = true;
			this.column_userID.VisibleIndex = 1;
			this.column_userID.Width = 50;
			this.column_userName.Caption = "姓名";
			this.column_userName.Name = "column_userName";
			this.column_userName.Visible = true;
			this.column_userName.VisibleIndex = 2;
			this.column_userName.Width = 40;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_lastName.Width = 45;
			this.column_cardNo.Caption = "卡号";
			this.column_cardNo.Name = "column_cardNo";
			this.column_cardNo.Visible = true;
			this.column_cardNo.VisibleIndex = 4;
			this.column_cardNo.Width = 42;
			this.column_deviceID.Caption = "设备名称";
			this.column_deviceID.Name = "column_deviceID";
			this.column_deviceID.Visible = true;
			this.column_deviceID.VisibleIndex = 5;
			this.column_deviceID.Width = 42;
			this.column_doorNo.Caption = "门事件点";
			this.column_doorNo.Name = "column_doorNo";
			this.column_doorNo.Visible = true;
			this.column_doorNo.VisibleIndex = 6;
			this.column_doorNo.Width = 60;
			this.column_inAddress.Caption = "辅助输入点";
			this.column_inAddress.Name = "column_inAddress";
			this.column_inAddress.Visible = true;
			this.column_inAddress.VisibleIndex = 7;
			this.column_inAddress.Width = 39;
			this.column_verified.Caption = "验证方式";
			this.column_verified.Name = "column_verified";
			this.column_verified.Visible = true;
			this.column_verified.VisibleIndex = 9;
			this.column_verified.Width = 39;
			this.column_state.Caption = "出入状态";
			this.column_state.Name = "column_state";
			this.column_state.Visible = true;
			this.column_state.VisibleIndex = 10;
			this.column_state.Width = 39;
			this.column_eventType.Caption = "事件描述";
			this.column_eventType.Name = "column_eventType";
			this.column_eventType.Visible = true;
			this.column_eventType.VisibleIndex = 11;
			this.column_eventType.Width = 47;
			this.column_triggerOpt.Caption = "联动触发条件 ";
			this.column_triggerOpt.Name = "column_triggerOpt";
			this.column_triggerOpt.Visible = true;
			this.column_triggerOpt.VisibleIndex = 12;
			this.column_triggerOpt.Width = 85;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_checkInOutView);
			base.Controls.Add(this.pnl_reports);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "ReportsUserControl";
			base.Size = new Size(684, 500);
			base.Load += this.ReportsUserControl_Load;
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_checkInOutView).EndInit();
			((ISupportInitialize)this.grd_reportView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
