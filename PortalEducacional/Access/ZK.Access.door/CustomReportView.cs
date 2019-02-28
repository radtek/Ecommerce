/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Utils;
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
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class CustomReportView : Office2007Form
	{
		private int rpid;

		private DataTable dtReportField;

		private DataTable dtUser;

		private DataTable dtDoor;

		private DataTable dtMachine;

		private DataTable dtReader;

		private DataTable dtAuxiliary;

		private DataTable dtLinkAgeIO;

		private DataTable dtEvent;

		private DataTable dtVT;

		private DataTable dtET;

		private DataTable dtIoState;

		private CustomReport Report;

		private ImagesForm frmLoading;

		private Dictionary<int, DataRow> dicDoorId_DoorRow;

		private IContainer components = null;

		private GridControl gridControl1;

		private GridView gridView1;

		private ExpandableSplitter expandableSplitter1;

		private GroupBox gbx_searchCondition;

		private DateEdit dtEndTime;

		private DateEdit dtEndDate;

		private DateEdit dtBeginTime;

		private DateEdit dtBeginDate;

		private TextBox txt_userName;

		private Label lbl_userName;

		private ButtonX btn_reset;

		private ButtonX btn_search;

		private System.Windows.Forms.ComboBox cmb_eventType;

		private Label lbl_evnetType;

		private System.Windows.Forms.ComboBox cmb_inoutState;

		private Label lbl_inoutState;

		private System.Windows.Forms.ComboBox cmb_verified;

		private Label lbl_verified;

		private System.Windows.Forms.ComboBox cmb_devName;

		private Label lbl_deviceName;

		private TextBox txt_cardNo;

		private Label lbl_cardNo;

		private TextBox txt_userPin;

		private Label lbl_userPin;

		private Label label2;

		private Label lbl_dateTime;

		private TextBox txtLastName;

		private Label label1;

		private ButtonX btnExport;

		public CustomReportView(int CustomReportId)
		{
			this.InitializeComponent();
			this.rpid = CustomReportId;
			initLang.LocaleForm(this, base.Name);
			base.MaximizeBox = true;
			base.MinimizeBox = true;
			base.ShowInTaskbar = false;
			base.FormBorderStyle = FormBorderStyle.Sizable;
			this.gridView1.OptionsView.ColumnAutoWidth = false;
			this.gridView1.OptionsBehavior.AutoPopulateColumns = false;
			this.LoadTime();
		}

		private void expandableSplitter1_ExpandedChanged(object sender, ExpandedChangeEventArgs e)
		{
			this.gbx_searchCondition.Height = (this.expandableSplitter1.Expanded ? 150 : 0);
		}

		private void txt_userPin_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 9);
			if (e.KeyChar == '\r')
			{
				this.btn_search.PerformClick();
			}
		}

		private void txt_cardNo_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 10);
			if (e.KeyChar == '\r')
			{
				this.btn_search.PerformClick();
			}
		}

		private void txt_userName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 50);
			if (e.KeyChar == '\r')
			{
				this.btn_search.PerformClick();
			}
		}

		private void btn_clear_Click(object sender, EventArgs e)
		{
			this.txt_cardNo.Text = "";
			this.txt_userPin.Text = "";
			this.txt_userName.Text = "";
			this.txtLastName.Text = "";
			this.cmb_devName.SelectedValue = DBNull.Value;
			this.cmb_eventType.SelectedValue = DBNull.Value;
			this.cmb_inoutState.SelectedValue = DBNull.Value;
			this.cmb_verified.SelectedValue = DBNull.Value;
			this.LoadTime();
		}

		private void LoadTime()
		{
			this.dtBeginTime.EditValue = new DateTime(2000, 1, 1, 0, 0, 0);
			this.dtEndTime.EditValue = new DateTime(2000, 1, 1, 23, 59, 59);
			this.dtBeginDate.EditValue = DateTime.Now;
			this.dtEndDate.EditValue = DateTime.Now;
		}

		private void CustomReportView_Load(object sender, EventArgs e)
		{
			this.frmLoading = new ImagesForm();
			this.frmLoading.Show();
			ThreadPool.QueueUserWorkItem(delegate
			{
				this.InitializeData();
			});
		}

		private void InitializeData()
		{
			try
			{
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.btn_search.Enabled = false;
					});
				}
				else
				{
					this.btn_search.Enabled = false;
				}
				CustomReportBLL customReportBLL = new CustomReportBLL(MainForm._ia);
				this.Report = customReportBLL.GetModel(this.rpid);
				if (this.Report == null)
				{
					if (base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.frmLoading.Hide();
						});
					}
					else
					{
						this.frmLoading.Hide();
					}
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("CustomReportNotExists", "自定义报表不存在"));
					return;
				}
				ReportFieldBLL reportFieldBLL = new ReportFieldBLL(MainForm._ia);
				DataSet list = reportFieldBLL.GetList("CRId = " + this.Report.Id + " order by ShowIndex");
				if (list == null || list.Tables.Count <= 0 || list.Tables[0].Rows.Count <= 0)
				{
					if (base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.frmLoading.Hide();
						});
					}
					else
					{
						this.frmLoading.Hide();
					}
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("CustomReportNoField", "自定义报表没有字段"));
					return;
				}
				this.dtReportField = list.Tables[0];
				this.LoadVerifyType();
				this.LoadEventType();
				this.LoadIoState();
				this.LoadUserInfo();
				this.LoadDoorInfo();
				this.LoadMachineInfo();
				this.LoadReaderInfo();
				this.LoadAuxiliaryInfo();
				this.LoadLinkAgeIOInfo();
				this.BindingDataSource();
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.btn_search.Enabled = true;
						this.Text += $" [{this.Report.ReportName}]";
					});
				}
				else
				{
					this.btn_search.Enabled = true;
					this.Text += $" [{this.Report.ReportName}]";
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.frmLoading.Hide();
				});
			}
			else
			{
				this.frmLoading.Hide();
			}
		}

		private void LoadUserInfo()
		{
			UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
			DataSet list = userInfoBll.GetList("", true);
			if (list != null && list.Tables.Count > 0)
			{
				this.dtUser = list.Tables[0];
			}
		}

		private void LoadDoorInfo()
		{
			this.dicDoorId_DoorRow = new Dictionary<int, DataRow>();
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			DataSet allList = accDoorBll.GetAllList();
			if (allList != null && allList.Tables.Count > 0)
			{
				this.dtDoor = allList.Tables[0];
				this.dtDoor.Columns.Add("DoorFlag", typeof(string));
				foreach (DataRow row in this.dtDoor.Rows)
				{
					int.TryParse(row["id"].ToString(), out int key);
					int.TryParse(row["device_id"].ToString(), out int _);
					if (!this.dicDoorId_DoorRow.ContainsKey(key))
					{
						this.dicDoorId_DoorRow.Add(key, row);
					}
					row["DoorFlag"] = string.Format("{0}_{1}", row["device_id"], row["door_no"]);
				}
			}
		}

		private void LoadMachineInfo()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			DataSet allList = machinesBll.GetAllList();
			if (allList != null && allList.Tables.Count > 0)
			{
				this.dtMachine = allList.Tables[0];
				DataRow dataRow = this.dtMachine.NewRow();
				dataRow["ID"] = -1;
				dataRow["MachineAlias"] = "---------------------";
				this.dtMachine.Rows.InsertAt(dataRow, 0);
			}
		}

		private void LoadReaderInfo()
		{
			AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
			DataSet allList = accReaderBll.GetAllList();
			if (allList != null && allList.Tables.Count > 0)
			{
				this.dtReader = allList.Tables[0];
				this.dtReader.Columns.Add("ReaderFlag", typeof(string));
				foreach (DataRow row in this.dtReader.Rows)
				{
					if (int.TryParse(row["door_id"].ToString(), out int key) && this.dicDoorId_DoorRow.ContainsKey(key))
					{
						row["ReaderFlag"] = string.Format("{0}_{1}_0_0_{2}", this.dicDoorId_DoorRow[key]["device_id"], this.dicDoorId_DoorRow[key]["door_no"], row["reader_state"]);
					}
				}
			}
		}

		private void LoadAuxiliaryInfo()
		{
			AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
			DataSet allList = accAuxiliaryBll.GetAllList();
			if (allList != null && allList.Tables.Count > 0)
			{
				this.dtAuxiliary = allList.Tables[0];
				this.dtAuxiliary.Columns.Add("EventFlag", typeof(string));
				foreach (DataRow row in this.dtAuxiliary.Rows)
				{
					int.TryParse(row["aux_state"].ToString(), out int num);
					if (num >= 0 && num <= 1)
					{
						num++;
					}
					row["EventFlag"] = string.Format("{0}_{1}_{2}_{3}", row["device_id"], num, num, row["aux_no"]);
				}
			}
		}

		private void LoadLinkAgeIOInfo()
		{
			AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
			DataSet allList = accLinkAgeIoBll.GetAllList();
			if (allList != null && allList.Tables.Count > 0)
			{
				this.dtLinkAgeIO = allList.Tables[0];
				this.dtLinkAgeIO.Columns.Add("EventFlag", typeof(string));
				foreach (DataRow row in this.dtLinkAgeIO.Rows)
				{
					row["EventFlag"] = string.Format("{0}_3_0_{1}", row["device_id"], row["id"]);
				}
			}
		}

		private void LoadEventInfo(string filter)
		{
			AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
			DataSet list = accMonitorLogBll.GetList(filter);
			if (list != null && list.Tables.Count > 0)
			{
				this.dtEvent = list.Tables[0];
				this.dtEvent.Columns.Add("EventFlag", typeof(string));
				this.dtEvent.Columns.Add("ReaderFlag", typeof(string));
				this.dtEvent.Columns.Add("DoorFlag", typeof(string));
				foreach (DataRow row in this.dtEvent.Rows)
				{
					int.TryParse(row["event_type"].ToString(), out int num);
					int num2 = num;
					int num3 = (num2 == 6) ? 3 : (((uint)(num2 - 12) <= 1u) ? 2 : (((uint)(num2 - 220) <= 1u) ? 1 : 0));
					row["EventFlag"] = row["event_point_id"].ToString() + "_" + row["state"].ToString();
					row["EventFlag"] = string.Format("{0}_{1}_{2}_{3}", row["device_id"], num3, row["event_point_type"], row["event_point_id"]);
					if (num3 == 0)
					{
						row["ReaderFlag"] = string.Format("{0}_{1}_0_0_{2}", row["device_id"], row["event_point_id"], row["state"]);
						row["DoorFlag"] = string.Format("{0}_{1}", row["device_id"], row["event_point_id"]);
					}
				}
			}
		}

		private void LoadVerifyType()
		{
			this.dtVT = new DataTable();
			this.dtVT.Columns.Add("id", typeof(int));
			this.dtVT.Columns.Add("verified", typeof(string));
			DataRow dataRow = this.dtVT.NewRow();
			dataRow["id"] = -1;
			dataRow["verified"] = "---------------------";
			this.dtVT.Rows.Add(dataRow);
			Dictionary<int, string> dic = PullSDKVerifyTypeInfos.GetDic();
			if (dic != null && dic.Count > 0)
			{
				foreach (KeyValuePair<int, string> item in dic)
				{
					dataRow = this.dtVT.NewRow();
					dataRow["id"] = item.Key;
					dataRow["verified"] = item.Value;
					this.dtVT.Rows.Add(dataRow);
				}
			}
		}

		private void LoadEventType()
		{
			this.dtET = new DataTable();
			this.dtET.Columns.Add("id", typeof(int));
			this.dtET.Columns.Add("event_type", typeof(string));
			DataRow dataRow = this.dtET.NewRow();
			dataRow["id"] = -1;
			dataRow["event_type"] = "---------------------";
			this.dtET.Rows.Add(dataRow);
			Dictionary<int, string> dic = PullSDKEventInfos.GetDic();
			if (dic != null && dic.Count > 0)
			{
				foreach (KeyValuePair<int, string> item in dic)
				{
					if (item.Key != -1)
					{
						dataRow = this.dtET.NewRow();
						dataRow["id"] = item.Key;
						dataRow["event_type"] = item.Value;
						this.dtET.Rows.Add(dataRow);
					}
				}
			}
		}

		private void LoadIoState()
		{
			this.dtIoState = new DataTable();
			this.dtIoState.Columns.Add("id", typeof(int));
			this.dtIoState.Columns.Add("state", typeof(string));
			DataRow dataRow = this.dtIoState.NewRow();
			dataRow["id"] = -1;
			dataRow["state"] = "---------------------";
			this.dtIoState.Rows.Add(dataRow);
			Dictionary<int, string> dic = InOutStateInfo.GetDic();
			if (dic != null && dic.Count > 0)
			{
				foreach (KeyValuePair<int, string> item in dic)
				{
					dataRow = this.dtIoState.NewRow();
					dataRow["id"] = item.Key;
					dataRow["state"] = item.Value;
					this.dtIoState.Rows.Add(dataRow);
				}
			}
		}

		private void BindingDataSource()
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.BindingDataSource();
				});
			}
			else
			{
				this.cmb_devName.DataSource = this.dtMachine;
				this.cmb_devName.ValueMember = "ID";
				this.cmb_devName.DisplayMember = "MachineAlias";
				this.cmb_eventType.DataSource = new DataView(this.dtET, "", "event_type", DataViewRowState.Added);
				this.cmb_eventType.ValueMember = "id";
				this.cmb_eventType.DisplayMember = "event_type";
				this.cmb_inoutState.DataSource = this.dtIoState;
				this.cmb_inoutState.ValueMember = "id";
				this.cmb_inoutState.DisplayMember = "state";
				this.cmb_verified.DataSource = this.dtVT;
				this.cmb_verified.ValueMember = "id";
				this.cmb_verified.DisplayMember = "verified";
			}
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

		private void btn_search_Click(object sender, EventArgs e)
		{
			if (this.Check())
			{
				DateTime dateTime = (DateTime)this.dtBeginDate.EditValue;
				DateTime dateTime2 = (DateTime)this.dtEndDate.EditValue;
				DateTime dateTime3 = (DateTime)this.dtBeginTime.EditValue;
				DateTime dateTime4 = (DateTime)this.dtEndTime.EditValue;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("time>={0}{1} {2}{0} and time <={0}{3} {4}{0}", (AppSite.Instance.DataType == DataType.SqlServer) ? "'" : "#", dateTime.ToString("yyyy-MM-dd"), dateTime3.ToString("HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd"), dateTime4.ToString("HH:mm:ss"));
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
				if (!string.IsNullOrEmpty(this.txtLastName.Text))
				{
					stringBuilder.Append(" and user_lastname like '%" + this.txt_userName.Text + "%'");
				}
				if (!string.IsNullOrEmpty(this.cmb_devName.Text) && this.cmb_devName.Text != "---------------------")
				{
					stringBuilder.Append(" and device_name='" + this.cmb_devName.Text + "'");
				}
				int num = default(int);
				if (this.cmb_verified.SelectedValue != null && int.TryParse(this.cmb_verified.SelectedValue.ToString(), out num) && num >= 0)
				{
					stringBuilder.Append(" and verified=" + this.cmb_verified.SelectedValue.ToString());
				}
				if (this.cmb_inoutState.SelectedValue != null && int.TryParse(this.cmb_inoutState.SelectedValue.ToString(), out num) && num >= 0)
				{
					stringBuilder.Append(" and state=" + this.cmb_inoutState.SelectedValue.ToString());
				}
				if (this.cmb_eventType.SelectedValue != null && int.TryParse(this.cmb_eventType.SelectedValue.ToString(), out num) && num >= 0)
				{
					switch (num)
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
						stringBuilder.Append(" and event_type=" + num);
						break;
					}
				}
				this.GenerateReport(stringBuilder.ToString());
			}
		}

		private void GenerateReport(string filter)
		{
			int visibleIndex = 0;
			this.LoadEventInfo(filter);
			this.gridView1.Columns.Clear();
			foreach (DataRow row in this.dtReportField.Rows)
			{
				bool flag = false;
				string text = "";
				DataTable dataTable = null;
				string text2 = row["FieldName"].ToString().Trim();
				string text3 = row["TableName"].ToString().ToUpper().Trim();
				if (!("" == text2) && !("" == text3))
				{
					int.TryParse(row["ShowIndex"].ToString(), out visibleIndex);
					GridColumn gridColumn = new GridColumn();
					gridColumn.VisibleIndex = visibleIndex;
					gridColumn.Caption = TableFieldLocalizer.GetFieldTitle(text3, text2);
					switch (text3)
					{
					case "ACC_MONITOR_LOG":
						flag = true;
						gridColumn.FieldName = text2;
						dataTable = this.dtEvent;
						if (this.dtEvent.Columns.Contains(text2) && !(this.dtEvent.Columns[text2].DataType == typeof(bool)) && (this.dtEvent.Columns[text2].DataType == typeof(DateTime) || this.dtEvent.Columns[text2].DataType == typeof(DateTime?)))
						{
							gridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
							gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
						}
						switch (text2.ToLower())
						{
						case "verified":
							gridColumn.FieldName = "verified";
							text = "id";
							flag = false;
							dataTable = this.dtVT;
							break;
						case "event_type":
							gridColumn.FieldName = "event_type";
							text = "id";
							flag = false;
							dataTable = this.dtET;
							break;
						case "state":
							gridColumn.FieldName = "state";
							text = "id";
							flag = false;
							dataTable = this.dtIoState;
							break;
						}
						break;
					case "USERINFO":
						gridColumn.FieldName = "pin";
						text = "Badgenumber";
						flag = false;
						dataTable = this.dtUser;
						break;
					case "MACHINES":
						gridColumn.FieldName = "device_id";
						text = "ID";
						flag = false;
						dataTable = this.dtMachine;
						break;
					case "ACC_DOOR":
						gridColumn.FieldName = "DoorFlag";
						text = "DoorFlag";
						flag = false;
						dataTable = this.dtDoor;
						break;
					case "ACC_READER":
						gridColumn.FieldName = "ReaderFlag";
						text = "ReaderFlag";
						flag = false;
						dataTable = this.dtReader;
						break;
					case "ACC_AUXILIARY":
						gridColumn.FieldName = "EventFlag";
						text = "EventFlag";
						flag = false;
						dataTable = this.dtAuxiliary;
						break;
					case "ACC_LINKAGEIO":
						gridColumn.FieldName = "EventFlag";
						text = "EventFlag";
						flag = false;
						dataTable = this.dtLinkAgeIO;
						break;
					default:
						LogServer.Log(text3, true);
						break;
					}
					if (!flag && dataTable != null && dataTable.Columns.Contains(text2))
					{
						string text4 = text2;
						if (dataTable.Columns.Contains(text))
						{
							text = dataTable.Columns[text].ColumnName;
						}
						if (dataTable.Columns.Contains(text4))
						{
							text4 = dataTable.Columns[text4].ColumnName;
						}
						if (!(dataTable.Columns[text2].DataType == typeof(bool)) && (dataTable.Columns[text2].DataType == typeof(DateTime) || dataTable.Columns[text2].DataType == typeof(DateTime?)))
						{
							RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
							repositoryItemLookUpEdit.ValueMember = text;
							repositoryItemLookUpEdit.DisplayMember = text4;
							repositoryItemLookUpEdit.DataSource = dataTable;
							repositoryItemLookUpEdit.NullText = "";
							repositoryItemLookUpEdit.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
							repositoryItemLookUpEdit.DisplayFormat.FormatType = FormatType.DateTime;
							gridColumn.ColumnEdit = repositoryItemLookUpEdit;
							flag = true;
						}
					}
					if (!flag && dataTable != null)
					{
						string text4 = text2;
						if (dataTable.Columns.Contains(text))
						{
							text = dataTable.Columns[text].ColumnName;
						}
						if (dataTable.Columns.Contains(text4))
						{
							text4 = dataTable.Columns[text4].ColumnName;
						}
						RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
						repositoryItemLookUpEdit.ValueMember = text;
						repositoryItemLookUpEdit.DisplayMember = text4;
						repositoryItemLookUpEdit.DataSource = dataTable;
						repositoryItemLookUpEdit.NullText = "";
						gridColumn.ColumnEdit = repositoryItemLookUpEdit;
					}
					this.gridView1.Columns.Add(gridColumn);
				}
			}
			this.gridControl1.DataSource = this.dtEvent;
			this.gridView1.BestFitColumns();
		}

		private void CustomReportView_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.frmLoading != null && !this.frmLoading.IsDisposed)
			{
				this.frmLoading.Dispose();
				this.frmLoading = null;
			}
		}

		private void gridView1_RowStyle(object sender, RowStyleEventArgs e)
		{
			GridView gridView = sender as GridView;
			int num;
			int num2;
			if (e.RowHandle >= 0 && this.dtEvent.Columns.Contains("event_type"))
			{
				DataRow dataRow = gridView.GetDataRow(e.RowHandle);
				if (dataRow != null && int.TryParse(dataRow[17].ToString(), out num))
				{
					if (num >= 1000 && num < 10000)
					{
						num -= 1000;
					}
					if ((num < 20 || num >= 100) && num != 100000)
					{
						num2 = ((num == 101001) ? 1 : 0);
						goto IL_00b5;
					}
					num2 = 1;
					goto IL_00b5;
				}
			}
			return;
			IL_00b5:
			if (num2 != 0)
			{
				e.Appearance.ForeColor = Color.Orange;
				return;
			}
			int num3;
			if ((num < 100 || num >= 200) && num != 28 && num != 100001 && num != 100032 && num != 100034 && num != 100055)
			{
				num3 = ((num == 100058) ? 1 : 0);
				goto IL_0115;
			}
			num3 = 1;
			goto IL_0115;
			IL_0115:
			if (num3 != 0)
			{
				e.Appearance.ForeColor = Color.Red;
			}
			else
			{
				e.Appearance.ForeColor = Color.Green;
			}
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			string fileName = this.Report.ReportName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
			DevExpressHelper.OutData(this.gridView1, fileName);
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
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			SerializableAppearanceObject appearance2 = new SerializableAppearanceObject();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CustomReportView));
			this.gridControl1 = new GridControl();
			this.gridView1 = new GridView();
			this.expandableSplitter1 = new ExpandableSplitter();
			this.gbx_searchCondition = new GroupBox();
			this.btnExport = new ButtonX();
			this.txtLastName = new TextBox();
			this.label1 = new Label();
			this.dtEndTime = new DateEdit();
			this.dtEndDate = new DateEdit();
			this.dtBeginTime = new DateEdit();
			this.dtBeginDate = new DateEdit();
			this.txt_userName = new TextBox();
			this.lbl_userName = new Label();
			this.btn_reset = new ButtonX();
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
			((ISupportInitialize)this.gridControl1).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			this.gbx_searchCondition.SuspendLayout();
			((ISupportInitialize)this.dtEndTime.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtEndTime.Properties).BeginInit();
			((ISupportInitialize)this.dtEndDate.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtEndDate.Properties).BeginInit();
			((ISupportInitialize)this.dtBeginTime.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtBeginTime.Properties).BeginInit();
			((ISupportInitialize)this.dtBeginDate.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.dtBeginDate.Properties).BeginInit();
			base.SuspendLayout();
			this.gridControl1.Cursor = Cursors.Default;
			this.gridControl1.Dock = DockStyle.Fill;
			this.gridControl1.Location = new Point(0, 174);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new Size(906, 317);
			this.gridControl1.TabIndex = 1;
			this.gridControl1.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridView1
			});
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.RowStyle += this.gridView1_RowStyle;
			this.expandableSplitter1.BackColor2 = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.BackColor2SchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.BackColorSchemePart = eColorSchemePart.PanelBackground;
			this.expandableSplitter1.Dock = DockStyle.Top;
			this.expandableSplitter1.ExpandFillColor = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.ExpandFillColorSchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.ExpandLineColor = Color.FromArgb(0, 0, 0);
			this.expandableSplitter1.ExpandLineColorSchemePart = eColorSchemePart.ItemText;
			this.expandableSplitter1.GripDarkColor = Color.FromArgb(0, 0, 0);
			this.expandableSplitter1.GripDarkColorSchemePart = eColorSchemePart.ItemText;
			this.expandableSplitter1.GripLightColor = Color.FromArgb(227, 239, 255);
			this.expandableSplitter1.GripLightColorSchemePart = eColorSchemePart.BarBackground;
			this.expandableSplitter1.HotBackColor = Color.FromArgb(252, 151, 61);
			this.expandableSplitter1.HotBackColor2 = Color.FromArgb(255, 184, 94);
			this.expandableSplitter1.HotBackColor2SchemePart = eColorSchemePart.ItemPressedBackground2;
			this.expandableSplitter1.HotBackColorSchemePart = eColorSchemePart.ItemPressedBackground;
			this.expandableSplitter1.HotExpandFillColor = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.HotExpandFillColorSchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.HotExpandLineColor = Color.FromArgb(0, 0, 0);
			this.expandableSplitter1.HotExpandLineColorSchemePart = eColorSchemePart.ItemText;
			this.expandableSplitter1.HotGripDarkColor = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.HotGripDarkColorSchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.HotGripLightColor = Color.FromArgb(227, 239, 255);
			this.expandableSplitter1.HotGripLightColorSchemePart = eColorSchemePart.BarBackground;
			this.expandableSplitter1.Location = new Point(0, 163);
			this.expandableSplitter1.MinExtra = 900;
			this.expandableSplitter1.Name = "expandableSplitter1";
			this.expandableSplitter1.Size = new Size(906, 11);
			this.expandableSplitter1.Style = eSplitterStyle.Office2007;
			this.expandableSplitter1.TabIndex = 30;
			this.expandableSplitter1.TabStop = false;
			this.expandableSplitter1.ExpandedChanged += this.expandableSplitter1_ExpandedChanged;
			this.gbx_searchCondition.BackColor = Color.White;
			this.gbx_searchCondition.Controls.Add(this.btnExport);
			this.gbx_searchCondition.Controls.Add(this.txtLastName);
			this.gbx_searchCondition.Controls.Add(this.label1);
			this.gbx_searchCondition.Controls.Add(this.dtEndTime);
			this.gbx_searchCondition.Controls.Add(this.dtEndDate);
			this.gbx_searchCondition.Controls.Add(this.dtBeginTime);
			this.gbx_searchCondition.Controls.Add(this.dtBeginDate);
			this.gbx_searchCondition.Controls.Add(this.txt_userName);
			this.gbx_searchCondition.Controls.Add(this.lbl_userName);
			this.gbx_searchCondition.Controls.Add(this.btn_reset);
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
			this.gbx_searchCondition.Location = new Point(0, 0);
			this.gbx_searchCondition.Name = "gbx_searchCondition";
			this.gbx_searchCondition.Size = new Size(906, 163);
			this.gbx_searchCondition.TabIndex = 31;
			this.gbx_searchCondition.TabStop = false;
			this.gbx_searchCondition.Text = "查询";
			this.btnExport.AccessibleRole = AccessibleRole.PushButton;
			this.btnExport.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnExport.Location = new Point(757, 131);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new Size(103, 25);
			this.btnExport.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnExport.TabIndex = 24;
			this.btnExport.Text = "导出";
			this.btnExport.Click += this.btnExport_Click;
			this.txtLastName.Location = new Point(739, 99);
			this.txtLastName.Name = "txtLastName";
			this.txtLastName.Size = new Size(121, 20);
			this.txtLastName.TabIndex = 23;
			this.txtLastName.KeyPress += this.txt_userName_KeyPress;
			this.label1.Location = new Point(632, 103);
			this.label1.Name = "label1";
			this.label1.Size = new Size(101, 14);
			this.label1.TabIndex = 22;
			this.label1.Text = "姓氏";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
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
			this.btn_reset.AccessibleRole = AccessibleRole.PushButton;
			this.btn_reset.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_reset.Location = new Point(502, 131);
			this.btn_reset.Name = "btn_reset";
			this.btn_reset.Size = new Size(103, 25);
			this.btn_reset.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_reset.TabIndex = 21;
			this.btn_reset.Text = "重置";
			this.btn_reset.Click += this.btn_clear_Click;
			this.btn_search.AccessibleRole = AccessibleRole.PushButton;
			this.btn_search.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_search.Location = new Point(634, 131);
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
			this.lbl_evnetType.Location = new Point(357, 103);
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
			this.lbl_inoutState.Location = new Point(16, 103);
			this.lbl_inoutState.Name = "lbl_inoutState";
			this.lbl_inoutState.Size = new Size(101, 13);
			this.lbl_inoutState.TabIndex = 14;
			this.lbl_inoutState.Text = "出入状态";
			this.lbl_inoutState.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_verified.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_verified.FormattingEnabled = true;
			this.cmb_verified.Location = new Point(133, 132);
			this.cmb_verified.Name = "cmb_verified";
			this.cmb_verified.Size = new Size(121, 21);
			this.cmb_verified.TabIndex = 19;
			this.lbl_verified.Location = new Point(16, 137);
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
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(906, 491);
			base.Controls.Add(this.gridControl1);
			base.Controls.Add(this.expandableSplitter1);
			base.Controls.Add(this.gbx_searchCondition);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "CustomReportView";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "查看报表";
			base.WindowState = FormWindowState.Maximized;
			base.FormClosed += this.CustomReportView_FormClosed;
			base.Load += this.CustomReportView_Load;
			((ISupportInitialize)this.gridControl1).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
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
			base.ResumeLayout(false);
		}
	}
}
