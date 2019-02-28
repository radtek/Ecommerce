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
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class CustomReportEditor : Office2007Form
	{
		private int CRId;

		private CustomReport oldReport;

		private DataTable dtFields;

		private DataTable dtSelectedFields;

		private Dictionary<string, DataRow> dicField;

		private IContainer components = null;

		private GridControl gcFields;

		private GridView gvFields;

		private GridColumn colCheck;

		private GridColumn colTableTitle;

		private GridColumn colFieldTitle;

		private RepositoryItemLookUpEdit lueVerifyType;

		private RepositoryItemLookUpEdit lueEventType;

		private RepositoryItemLookUpEdit lueUserName;

		private RepositoryItemLookUpEdit lueLastName;

		private GridControl gcSelectedFields;

		private GridView gvSelectedFields;

		private GridColumn colSelectedCheck;

		private GridColumn colSelectedTableTitle;

		private GridColumn colSelectedFieldTitle;

		private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

		private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

		private RepositoryItemLookUpEdit repositoryItemLookUpEdit3;

		private RepositoryItemLookUpEdit repositoryItemLookUpEdit4;

		private ButtonX btnAll2Left;

		private ButtonX btnToLeft;

		private ButtonX btnToRight;

		private ButtonX btnAll2Right;

		private ButtonX btnCancel;

		private ButtonX btnOK;

		private Label lblReportName;

		private TextBox txtReportName;

		private TextBox txtMemo;

		private Label lblMemo;

		private ButtonX btnDown;

		private ButtonX btnUp;

		public CustomReportEditor()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public CustomReportEditor(int rpid)
			: this()
		{
			this.CRId = rpid;
		}

		private void InitTable()
		{
			this.dtFields = new DataTable();
			this.dtFields.Columns.Add("check", typeof(bool));
			this.dtFields.Columns.Add("CRId", typeof(int));
			this.dtFields.Columns.Add("TableName", typeof(string));
			this.dtFields.Columns.Add("TableTitle", typeof(string));
			this.dtFields.Columns.Add("FieldName", typeof(string));
			this.dtFields.Columns.Add("FieldTitle", typeof(string));
			this.dtFields.Columns.Add("ShowIndex", typeof(int));
			this.dtSelectedFields = this.dtFields.Clone();
		}

		private void LoadData()
		{
			if (this.CRId > 0)
			{
				CustomReportBLL customReportBLL = new CustomReportBLL(MainForm._ia);
				this.oldReport = customReportBLL.GetModel(this.CRId);
				this.dicField = new Dictionary<string, DataRow>();
				ReportFieldBLL reportFieldBLL = new ReportFieldBLL(MainForm._ia);
				List<ReportField> modelList = reportFieldBLL.GetModelList("CRId = " + this.CRId + " order by ShowIndex");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						ReportField reportField = modelList[i];
						DataRow dataRow = this.dtSelectedFields.NewRow();
						dataRow["check"] = false;
						dataRow["CRId"] = reportField.CRId;
						dataRow["TableName"] = reportField.TableName;
						dataRow["TableTitle"] = TableFieldLocalizer.GetTableTitle(reportField.TableName);
						dataRow["FieldName"] = reportField.FieldName;
						dataRow["FieldTitle"] = TableFieldLocalizer.GetFieldTitle(reportField.TableName, reportField.FieldName);
						dataRow["ShowIndex"] = reportField.ShowIndex;
						this.dtSelectedFields.Rows.Add(dataRow);
						this.dicField.Add(reportField.TableName.ToUpper() + reportField.FieldName.ToUpper(), dataRow);
					}
				}
			}
			this.LoadFieldsFromLangugeFile();
			this.BindData();
		}

		private void LoadAllFields()
		{
			string[] array = new string[6]
			{
				"acc_monitor_log",
				"USERINFO",
				"Machines",
				"acc_door",
				"acc_reader",
				"acc_auxiliary"
			};
			string[] array2 = array;
			foreach (string text in array2)
			{
				string sQLString = "Select * from " + text + " where 1<0";
				DataType dataType = AppSite.Instance.DataType;
				DataSet dataSet = (dataType != DataType.SqlServer) ? DbHelperOleDb.Query(sQLString) : DbHelperSQL.Query(sQLString);
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					foreach (DataColumn column in dataTable.Columns)
					{
						if (column.ColumnName != null && !(column.ColumnName.Trim() == "") && (this.dicField == null || !this.dicField.ContainsKey(text.ToUpper() + column.ColumnName.ToUpper())))
						{
							DataRow dataRow = this.dtFields.NewRow();
							dataRow["check"] = false;
							dataRow["CRId"] = this.CRId;
							dataRow["TableName"] = text;
							dataRow["TableTitle"] = TableFieldLocalizer.GetTableTitle(text);
							dataRow["FieldName"] = column.ColumnName;
							dataRow["FieldTitle"] = TableFieldLocalizer.GetFieldTitle(text, column.ColumnName);
							dataRow["ShowIndex"] = 0;
							this.dtFields.Rows.Add(dataRow);
						}
					}
				}
			}
		}

		private void LoadFieldsFromLangugeFile()
		{
			DataTable allFields = TableFieldLocalizer.GetAllFields();
			foreach (DataRow row in allFields.Rows)
			{
				string text = row["TableName"].ToString();
				string text2 = row["FieldName"].ToString();
				if (text != null && !(text.Trim() == "") && text2 != null && !(text2.Trim() == "") && (this.dicField == null || !this.dicField.ContainsKey(text.ToUpper() + text2.ToUpper())))
				{
					DataRow dataRow2 = this.dtFields.NewRow();
					dataRow2["check"] = false;
					dataRow2["CRId"] = this.CRId;
					dataRow2["TableName"] = text;
					dataRow2["TableTitle"] = row["TableTitle"].ToString();
					dataRow2["FieldName"] = text2;
					dataRow2["FieldTitle"] = row["FieldTitle"].ToString();
					dataRow2["ShowIndex"] = 0;
					this.dtFields.Rows.Add(dataRow2);
				}
			}
		}

		private void BindData()
		{
			this.gcFields.DataSource = this.dtFields;
			this.colCheck.FieldName = "check";
			this.colFieldTitle.FieldName = "FieldTitle";
			this.colTableTitle.FieldName = "TableTitle";
			this.colTableTitle.GroupIndex = 0;
			this.gcSelectedFields.DataSource = this.dtSelectedFields;
			this.colSelectedCheck.FieldName = "check";
			this.colSelectedFieldTitle.FieldName = "FieldTitle";
			this.colSelectedTableTitle.FieldName = "TableTitle";
			if (this.oldReport != null)
			{
				this.txtMemo.Text = this.oldReport.Memo;
				this.txtReportName.Text = this.oldReport.ReportName;
			}
		}

		private void LocalizeDataTable(DataTable dt)
		{
			if (dt != null)
			{
				foreach (DataRow row in dt.Rows)
				{
					if (!(row["TableName"].ToString() == ""))
					{
						row["TableTitle"] = TableFieldLocalizer.GetTableTitle(row["TableName"].ToString());
						if (!(row["FieldName"].ToString() == ""))
						{
							row["FieldTitle"] = TableFieldLocalizer.GetFieldTitle(row["TableName"].ToString(), row["FieldName"].ToString());
						}
					}
				}
			}
		}

		private void btnAll2Right_Click(object sender, EventArgs e)
		{
			this.MoveRow(this.dtFields, this.dtSelectedFields, "");
		}

		private void btnToRight_Click(object sender, EventArgs e)
		{
			this.MoveRow(this.dtFields, this.dtSelectedFields, "check=true");
		}

		private void btnToLeft_Click(object sender, EventArgs e)
		{
			this.MoveRow(this.dtSelectedFields, this.dtFields, "check=true");
		}

		private void btnAll2Left_Click(object sender, EventArgs e)
		{
			this.MoveRow(this.dtSelectedFields, this.dtFields, "");
		}

		private void gvFields_DoubleClick(object sender, EventArgs e)
		{
			DataRow focusedDataRow = this.gvFields.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				this.dtSelectedFields.Rows.Add(focusedDataRow.ItemArray);
				this.dtSelectedFields.Rows[this.dtSelectedFields.Rows.Count - 1]["check"] = false;
				this.dtFields.Rows.Remove(focusedDataRow);
			}
		}

		private void gvSelectedFields_DoubleClick(object sender, EventArgs e)
		{
			DataRow focusedDataRow = this.gvSelectedFields.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				this.dtFields.Rows.Add(focusedDataRow.ItemArray);
				this.dtFields.Rows[this.dtFields.Rows.Count - 1]["check"] = false;
				this.dtSelectedFields.Rows.Remove(focusedDataRow);
			}
		}

		private void CheckMovingBtn()
		{
			if (this.dtFields.Rows.Count > 0)
			{
				this.btnToRight.Enabled = true;
				this.btnAll2Right.Enabled = true;
			}
			else
			{
				this.btnToRight.Enabled = false;
				this.btnAll2Right.Enabled = false;
			}
			if (this.dtSelectedFields.Rows.Count > 0)
			{
				this.btnToLeft.Enabled = true;
				this.btnAll2Left.Enabled = true;
			}
			else
			{
				this.btnToLeft.Enabled = false;
				this.btnAll2Left.Enabled = false;
			}
		}

		private void MoveRow(DataTable dtSource, DataTable dtDest, string filter)
		{
			try
			{
				DataRow[] array = dtSource.Select(filter);
				if (array != null && array.Length != 0)
				{
					this.gcFields.DataSource = null;
					this.gcSelectedFields.DataSource = null;
					dtDest.BeginLoadData();
					dtSource.BeginLoadData();
					for (int i = 0; i < array.Length; i++)
					{
						dtDest.Rows.Add(array[i].ItemArray);
						dtDest.Rows[dtDest.Rows.Count - 1]["check"] = false;
						dtSource.Rows.Remove(array[i]);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PlsSelectField", "请选择字段"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			dtDest.EndLoadData();
			dtSource.EndLoadData();
			this.gcFields.DataSource = this.dtFields;
			this.gcSelectedFields.DataSource = this.dtSelectedFields;
			this.CheckMovingBtn();
		}

		private void CustomReportEditor_Load(object sender, EventArgs e)
		{
			try
			{
				this.InitTable();
				this.LoadData();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void gvFields_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void gvFields_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void gvFields_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void gvSelectedFields_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void gvSelectedFields_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void gvSelectedFields_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private bool Check()
		{
			if (this.txtReportName.Text == null || "" == this.txtReportName.Text.Trim())
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("EmptyReportName", "请输入报表名称"));
				this.txtReportName.Focus();
				return false;
			}
			string strWhere = $"ReportName = '{this.txtReportName.Text}' and id <> {this.CRId}";
			CustomReportBLL customReportBLL = new CustomReportBLL(MainForm._ia);
			List<CustomReport> modelList = customReportBLL.GetModelList(strWhere);
			if (modelList != null && modelList.Count > 0)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ReportNameExists", "报表名称已存在"));
				this.txtReportName.Focus();
				return false;
			}
			return true;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.Check())
				{
					CustomReport customReport = null;
					CustomReportBLL customReportBLL = new CustomReportBLL(MainForm._ia);
					customReport = ((this.oldReport == null) ? new CustomReport() : this.oldReport.Copy());
					customReport.ReportName = this.txtReportName.Text;
					customReport.Memo = this.txtMemo.Text;
					if (this.oldReport == null)
					{
						customReportBLL.Add(customReport);
					}
					else if (customReport.Memo != this.oldReport.Memo || customReport.ReportName != this.oldReport.ReportName)
					{
						customReportBLL.Update(customReport);
					}
					this.oldReport = customReport.Copy();
					this.SaveFieldList();
					base.DialogResult = DialogResult.OK;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void SaveFieldList()
		{
			if (this.dtSelectedFields.Rows.Count > 0)
			{
				List<ReportField> list = new List<ReportField>();
				for (int i = 0; i < this.dtSelectedFields.Rows.Count; i++)
				{
					ReportField reportField = this.Convert2FieldModel(this.dtSelectedFields.Rows[i]);
					reportField.CRId = this.oldReport.Id;
					reportField.ShowIndex = i;
					list.Add(reportField);
				}
				ReportFieldBLL reportFieldBLL = new ReportFieldBLL(MainForm._ia);
				reportFieldBLL.DeleteByReportIdList(this.oldReport.Id.ToString());
				reportFieldBLL.Add(list);
			}
		}

		public ReportField Convert2FieldModel(DataRow dr)
		{
			if (dr == null)
			{
				return null;
			}
			ReportField reportField = new ReportField();
			if ("" != dr["CRId"].ToString())
			{
				reportField.CRId = (int)dr["CRId"];
			}
			if ("" != dr["ShowIndex"].ToString())
			{
				reportField.ShowIndex = (int)dr["ShowIndex"];
			}
			reportField.FieldName = dr["FieldName"].ToString();
			reportField.TableName = dr["TableName"].ToString();
			return reportField;
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			this.gcSelectedFields.Focus();
			int focusedRowHandle = this.gvSelectedFields.FocusedRowHandle;
			if (focusedRowHandle >= 1)
			{
				DataRow dataRow = this.dtSelectedFields.Rows[focusedRowHandle];
				DataRow dataRow2 = this.dtSelectedFields.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				this.dtSelectedFields.Rows.Remove(dataRow);
				this.dtSelectedFields.Rows.InsertAt(dataRow2, focusedRowHandle - 1);
				this.gvSelectedFields.FocusedRowHandle = focusedRowHandle - 1;
			}
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
			this.gcSelectedFields.Focus();
			int focusedRowHandle = this.gvSelectedFields.FocusedRowHandle;
			if (focusedRowHandle >= 0 && focusedRowHandle <= this.dtSelectedFields.Rows.Count - 2)
			{
				DataRow dataRow = this.dtSelectedFields.Rows[focusedRowHandle];
				DataRow dataRow2 = this.dtSelectedFields.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				this.dtSelectedFields.Rows.Remove(dataRow);
				this.dtSelectedFields.Rows.InsertAt(dataRow2, focusedRowHandle + 1);
				this.gvSelectedFields.FocusedRowHandle = focusedRowHandle + 1;
			}
		}

		private void gvSelectedFields_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				switch (e.KeyCode)
				{
				case Keys.Up:
					this.btnUp.PerformClick();
					break;
				case Keys.Down:
					this.btnDown.PerformClick();
					break;
				}
				e.Handled = true;
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CustomReportEditor));
			this.gcFields = new GridControl();
			this.gvFields = new GridView();
			this.colCheck = new GridColumn();
			this.colTableTitle = new GridColumn();
			this.colFieldTitle = new GridColumn();
			this.lueVerifyType = new RepositoryItemLookUpEdit();
			this.lueEventType = new RepositoryItemLookUpEdit();
			this.lueUserName = new RepositoryItemLookUpEdit();
			this.lueLastName = new RepositoryItemLookUpEdit();
			this.gcSelectedFields = new GridControl();
			this.gvSelectedFields = new GridView();
			this.colSelectedCheck = new GridColumn();
			this.colSelectedTableTitle = new GridColumn();
			this.colSelectedFieldTitle = new GridColumn();
			this.repositoryItemLookUpEdit1 = new RepositoryItemLookUpEdit();
			this.repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit();
			this.repositoryItemLookUpEdit3 = new RepositoryItemLookUpEdit();
			this.repositoryItemLookUpEdit4 = new RepositoryItemLookUpEdit();
			this.btnAll2Left = new ButtonX();
			this.btnToLeft = new ButtonX();
			this.btnToRight = new ButtonX();
			this.btnAll2Right = new ButtonX();
			this.btnCancel = new ButtonX();
			this.btnOK = new ButtonX();
			this.lblReportName = new Label();
			this.txtReportName = new TextBox();
			this.txtMemo = new TextBox();
			this.lblMemo = new Label();
			this.btnDown = new ButtonX();
			this.btnUp = new ButtonX();
			((ISupportInitialize)this.gcFields).BeginInit();
			((ISupportInitialize)this.gvFields).BeginInit();
			((ISupportInitialize)this.lueVerifyType).BeginInit();
			((ISupportInitialize)this.lueEventType).BeginInit();
			((ISupportInitialize)this.lueUserName).BeginInit();
			((ISupportInitialize)this.lueLastName).BeginInit();
			((ISupportInitialize)this.gcSelectedFields).BeginInit();
			((ISupportInitialize)this.gvSelectedFields).BeginInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit2).BeginInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit3).BeginInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit4).BeginInit();
			base.SuspendLayout();
			this.gcFields.Cursor = Cursors.Default;
			this.gcFields.Location = new Point(12, 64);
			this.gcFields.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.gcFields.MainView = this.gvFields;
			this.gcFields.Name = "gcFields";
			this.gcFields.RepositoryItems.AddRange(new RepositoryItem[4]
			{
				this.lueVerifyType,
				this.lueEventType,
				this.lueUserName,
				this.lueLastName
			});
			this.gcFields.Size = new Size(327, 360);
			this.gcFields.TabIndex = 19;
			this.gcFields.TabStop = false;
			this.gcFields.ViewCollection.AddRange(new BaseView[1]
			{
				this.gvFields
			});
			this.gvFields.Columns.AddRange(new GridColumn[3]
			{
				this.colCheck,
				this.colTableTitle,
				this.colFieldTitle
			});
			this.gvFields.GridControl = this.gcFields;
			this.gvFields.IndicatorWidth = 35;
			this.gvFields.Name = "gvFields";
			this.gvFields.OptionsBehavior.Editable = false;
			this.gvFields.OptionsView.ShowGroupPanel = false;
			this.gvFields.CustomDrawColumnHeader += this.gvFields_CustomDrawColumnHeader;
			this.gvFields.CustomDrawCell += this.gvFields_CustomDrawCell;
			this.gvFields.Click += this.gvFields_Click;
			this.gvFields.DoubleClick += this.gvFields_DoubleClick;
			this.colCheck.Caption = " ";
			this.colCheck.Name = "colCheck";
			this.colCheck.Visible = true;
			this.colCheck.VisibleIndex = 0;
			this.colCheck.Width = 50;
			this.colTableTitle.Caption = "表";
			this.colTableTitle.Name = "colTableTitle";
			this.colTableTitle.Visible = true;
			this.colTableTitle.VisibleIndex = 1;
			this.colTableTitle.Width = 120;
			this.colFieldTitle.Caption = "列";
			this.colFieldTitle.Name = "colFieldTitle";
			this.colFieldTitle.Visible = true;
			this.colFieldTitle.VisibleIndex = 2;
			this.colFieldTitle.Width = 120;
			this.lueVerifyType.AutoHeight = false;
			this.lueVerifyType.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueVerifyType.Name = "lueVerifyType";
			this.lueEventType.AutoHeight = false;
			this.lueEventType.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueEventType.Name = "lueEventType";
			this.lueUserName.AutoHeight = false;
			this.lueUserName.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueUserName.Name = "lueUserName";
			this.lueLastName.AutoHeight = false;
			this.lueLastName.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueLastName.Name = "lueLastName";
			this.gcSelectedFields.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.gcSelectedFields.Cursor = Cursors.Default;
			this.gcSelectedFields.Location = new Point(455, 64);
			this.gcSelectedFields.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.gcSelectedFields.MainView = this.gvSelectedFields;
			this.gcSelectedFields.Name = "gcSelectedFields";
			this.gcSelectedFields.RepositoryItems.AddRange(new RepositoryItem[4]
			{
				this.repositoryItemLookUpEdit1,
				this.repositoryItemLookUpEdit2,
				this.repositoryItemLookUpEdit3,
				this.repositoryItemLookUpEdit4
			});
			this.gcSelectedFields.Size = new Size(327, 360);
			this.gcSelectedFields.TabIndex = 20;
			this.gcSelectedFields.TabStop = false;
			this.gcSelectedFields.ViewCollection.AddRange(new BaseView[1]
			{
				this.gvSelectedFields
			});
			this.gvSelectedFields.Columns.AddRange(new GridColumn[3]
			{
				this.colSelectedCheck,
				this.colSelectedTableTitle,
				this.colSelectedFieldTitle
			});
			this.gvSelectedFields.GridControl = this.gcSelectedFields;
			this.gvSelectedFields.IndicatorWidth = 35;
			this.gvSelectedFields.Name = "gvSelectedFields";
			this.gvSelectedFields.OptionsBehavior.Editable = false;
			this.gvSelectedFields.OptionsView.ShowGroupPanel = false;
			this.gvSelectedFields.CustomDrawColumnHeader += this.gvSelectedFields_CustomDrawColumnHeader;
			this.gvSelectedFields.CustomDrawCell += this.gvSelectedFields_CustomDrawCell;
			this.gvSelectedFields.KeyDown += this.gvSelectedFields_KeyDown;
			this.gvSelectedFields.Click += this.gvSelectedFields_Click;
			this.gvSelectedFields.DoubleClick += this.gvSelectedFields_DoubleClick;
			this.colSelectedCheck.Caption = " ";
			this.colSelectedCheck.Name = "colSelectedCheck";
			this.colSelectedCheck.OptionsColumn.AllowSort = DefaultBoolean.False;
			this.colSelectedCheck.Visible = true;
			this.colSelectedCheck.VisibleIndex = 0;
			this.colSelectedCheck.Width = 50;
			this.colSelectedTableTitle.Caption = "表";
			this.colSelectedTableTitle.Name = "colSelectedTableTitle";
			this.colSelectedTableTitle.OptionsColumn.AllowSort = DefaultBoolean.False;
			this.colSelectedTableTitle.Visible = true;
			this.colSelectedTableTitle.VisibleIndex = 1;
			this.colSelectedTableTitle.Width = 120;
			this.colSelectedFieldTitle.Caption = "列";
			this.colSelectedFieldTitle.Name = "colSelectedFieldTitle";
			this.colSelectedFieldTitle.OptionsColumn.AllowSort = DefaultBoolean.False;
			this.colSelectedFieldTitle.Visible = true;
			this.colSelectedFieldTitle.VisibleIndex = 2;
			this.colSelectedFieldTitle.Width = 120;
			this.repositoryItemLookUpEdit1.AutoHeight = false;
			this.repositoryItemLookUpEdit1.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
			this.repositoryItemLookUpEdit2.AutoHeight = false;
			this.repositoryItemLookUpEdit2.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
			this.repositoryItemLookUpEdit3.AutoHeight = false;
			this.repositoryItemLookUpEdit3.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.repositoryItemLookUpEdit3.Name = "repositoryItemLookUpEdit3";
			this.repositoryItemLookUpEdit4.AutoHeight = false;
			this.repositoryItemLookUpEdit4.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.repositoryItemLookUpEdit4.Name = "repositoryItemLookUpEdit4";
			this.btnAll2Left.AccessibleRole = AccessibleRole.PushButton;
			this.btnAll2Left.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnAll2Left.Location = new Point(372, 252);
			this.btnAll2Left.Name = "btnAll2Left";
			this.btnAll2Left.Size = new Size(50, 23);
			this.btnAll2Left.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnAll2Left.TabIndex = 5;
			this.btnAll2Left.Text = "<<";
			this.btnAll2Left.Click += this.btnAll2Left_Click;
			this.btnToLeft.AccessibleRole = AccessibleRole.PushButton;
			this.btnToLeft.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnToLeft.Location = new Point(372, 214);
			this.btnToLeft.Name = "btnToLeft";
			this.btnToLeft.Size = new Size(50, 23);
			this.btnToLeft.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnToLeft.TabIndex = 4;
			this.btnToLeft.Text = "<";
			this.btnToLeft.Click += this.btnToLeft_Click;
			this.btnToRight.AccessibleRole = AccessibleRole.PushButton;
			this.btnToRight.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnToRight.Location = new Point(372, 176);
			this.btnToRight.Name = "btnToRight";
			this.btnToRight.Size = new Size(50, 23);
			this.btnToRight.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnToRight.TabIndex = 3;
			this.btnToRight.Text = ">";
			this.btnToRight.Click += this.btnToRight_Click;
			this.btnAll2Right.AccessibleRole = AccessibleRole.PushButton;
			this.btnAll2Right.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnAll2Right.Location = new Point(372, 138);
			this.btnAll2Right.Name = "btnAll2Right";
			this.btnAll2Right.Size = new Size(50, 23);
			this.btnAll2Right.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnAll2Right.TabIndex = 2;
			this.btnAll2Right.Text = ">>";
			this.btnAll2Right.Click += this.btnAll2Right_Click;
			this.btnCancel.AccessibleRole = AccessibleRole.PushButton;
			this.btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btnCancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(700, 443);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(82, 23);
			this.btnCancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += this.btnCancel_Click;
			this.btnOK.AccessibleRole = AccessibleRole.PushButton;
			this.btnOK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btnOK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnOK.Location = new Point(598, 443);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new Size(82, 23);
			this.btnOK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "确定";
			this.btnOK.Click += this.btnOK_Click;
			this.lblReportName.AutoSize = true;
			this.lblReportName.Location = new Point(21, 26);
			this.lblReportName.Name = "lblReportName";
			this.lblReportName.Size = new Size(53, 12);
			this.lblReportName.TabIndex = 27;
			this.lblReportName.Text = "报表名称";
			this.txtReportName.Location = new Point(145, 22);
			this.txtReportName.Name = "txtReportName";
			this.txtReportName.Size = new Size(194, 21);
			this.txtReportName.TabIndex = 0;
			this.txtMemo.Location = new Point(567, 22);
			this.txtMemo.Name = "txtMemo";
			this.txtMemo.Size = new Size(215, 21);
			this.txtMemo.TabIndex = 1;
			this.lblMemo.AutoSize = true;
			this.lblMemo.Location = new Point(457, 26);
			this.lblMemo.Name = "lblMemo";
			this.lblMemo.Size = new Size(29, 12);
			this.lblMemo.TabIndex = 29;
			this.lblMemo.Text = "备注";
			this.btnDown.AccessibleRole = AccessibleRole.PushButton;
			this.btnDown.ColorTable = eButtonColor.Flat;
			this.btnDown.Image = (Image)componentResourceManager.GetObject("btnDown.Image");
			this.btnDown.Location = new Point(425, 99);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new Size(24, 16);
			this.btnDown.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnDown.TabIndex = 33;
			this.btnDown.TabStop = false;
			this.btnDown.Tooltip = "Ctrl + ↓";
			this.btnDown.Click += this.btnDown_Click;
			this.btnUp.AccessibleRole = AccessibleRole.PushButton;
			this.btnUp.ColorTable = eButtonColor.Flat;
			this.btnUp.Image = (Image)componentResourceManager.GetObject("btnUp.Image");
			this.btnUp.Location = new Point(425, 77);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new Size(24, 16);
			this.btnUp.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnUp.TabIndex = 34;
			this.btnUp.TabStop = false;
			this.btnUp.Tooltip = "Ctrl + ↑";
			this.btnUp.Click += this.btnUp_Click;
			base.AcceptButton = this.btnOK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(794, 478);
			base.Controls.Add(this.btnUp);
			base.Controls.Add(this.btnDown);
			base.Controls.Add(this.txtMemo);
			base.Controls.Add(this.lblMemo);
			base.Controls.Add(this.txtReportName);
			base.Controls.Add(this.lblReportName);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.btnAll2Left);
			base.Controls.Add(this.btnToLeft);
			base.Controls.Add(this.btnToRight);
			base.Controls.Add(this.btnAll2Right);
			base.Controls.Add(this.gcSelectedFields);
			base.Controls.Add(this.gcFields);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CustomReportEditor";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "自定义报表";
			base.Load += this.CustomReportEditor_Load;
			((ISupportInitialize)this.gcFields).EndInit();
			((ISupportInitialize)this.gvFields).EndInit();
			((ISupportInitialize)this.lueVerifyType).EndInit();
			((ISupportInitialize)this.lueEventType).EndInit();
			((ISupportInitialize)this.lueUserName).EndInit();
			((ISupportInitialize)this.lueLastName).EndInit();
			((ISupportInitialize)this.gcSelectedFields).EndInit();
			((ISupportInitialize)this.gvSelectedFields).EndInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit3).EndInit();
			((ISupportInitialize)this.repositoryItemLookUpEdit4).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
