/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Utils;

namespace ZK.Access.door
{
	public class CustomReportList : UserControl
	{
		private bool IsEditing;

		private DataTable dtReports;

		private IContainer components = null;

		public ToolStrip MenuPanelEx;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		private ToolStripButton btn_log;

		public PanelEx pnl_CustomReport;

		private GridControl grd_checkInOutView;

		private GridView grd_reportView;

		private GridColumn colCheck;

		private GridColumn colReportName;

		private GridColumn colMemo;

		private RepositoryItemLookUpEdit lueVerifyType;

		private RepositoryItemLookUpEdit lueEventType;

		private RepositoryItemLookUpEdit lueUserName;

		private RepositoryItemLookUpEdit lueLastName;

		private ToolStripButton btnViewReport;

		public CustomReportList()
		{
			this.InitializeComponent();
			DevExpressHelper.InitImageList(this.grd_reportView, this.colCheck.Name);
			initLang.LocaleForm(this, base.Name);
			this.LoadReports();
		}

		private void LoadReports()
		{
			try
			{
				CustomReportBLL customReportBLL = new CustomReportBLL(MainForm._ia);
				DataSet allList = customReportBLL.GetAllList();
				if (allList != null && allList.Tables.Count > 0)
				{
					this.dtReports = allList.Tables[0];
				}
				else
				{
					this.dtReports = new DataTable();
					this.dtReports.Columns.Add("id", typeof(int));
					this.dtReports.Columns.Add("ReportName", typeof(string));
					this.dtReports.Columns.Add("Memo", typeof(string));
				}
				this.dtReports.Columns.Add("check", typeof(bool));
				this.grd_checkInOutView.DataSource = this.dtReports;
				this.colCheck.FieldName = "check";
				this.colReportName.FieldName = "ReportName";
				this.colMemo.FieldName = "Memo";
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
			this.CheckPermission();
		}

		private void CheckPermission()
		{
			if (this.dtReports == null || !SysInfos.IsOwerControlPermission(SysInfos.Report))
			{
				this.btn_add.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_delete.Enabled = false;
				this.btn_log.Enabled = false;
				this.btnViewReport.Enabled = false;
			}
			else
			{
				this.btn_add.Enabled = true;
				this.btn_edit.Enabled = (this.dtReports.Rows.Count > 0);
				this.btn_delete.Enabled = (this.dtReports.Rows.Count > 0);
				this.btnViewReport.Enabled = (this.dtReports.Rows.Count > 0);
				this.btn_log.Enabled = true;
			}
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			CustomReportEditor customReportEditor = new CustomReportEditor();
			if (customReportEditor.ShowDialog() == DialogResult.OK)
			{
				this.LoadReports();
			}
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			if (this.dtReports != null && this.dtReports.Rows.Count > 0)
			{
				DataRow[] array = this.dtReports.Select("check=true");
				if (array == null || array.Length == 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
				}
				else if (array.Length > 1)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
				}
				else
				{
					CustomReportEditor customReportEditor = new CustomReportEditor((int)array[0]["id"]);
					if (customReportEditor.ShowDialog() == DialogResult.OK)
					{
						this.LoadReports();
					}
				}
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			if (this.dtReports != null && this.dtReports.Rows.Count > 0)
			{
				DataRow[] array = this.dtReports.Select("check=true");
				if (array == null || array.Length == 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的记录"));
				}
				else if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					StringBuilder stringBuilder = new StringBuilder();
					int num = 0;
					DataRow[] array2 = array;
					foreach (DataRow dataRow in array2)
					{
						if (num > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.AppendFormat("{0}", dataRow["id"]);
						num++;
					}
					CustomReportBLL customReportBLL = new CustomReportBLL(MainForm._ia);
					customReportBLL.DeleteList(stringBuilder.ToString());
					this.LoadReports();
				}
			}
		}

		private void grd_reportView_DoubleClick(object sender, EventArgs e)
		{
			if (!this.IsEditing)
			{
				this.IsEditing = true;
				DataRow focusedDataRow = this.grd_reportView.GetFocusedDataRow();
				if (focusedDataRow != null)
				{
					CustomReportEditor customReportEditor = new CustomReportEditor((int)focusedDataRow["id"]);
					if (customReportEditor.ShowDialog() == DialogResult.OK)
					{
						this.LoadReports();
					}
				}
				this.IsEditing = false;
			}
		}

		private void grd_reportView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void grd_reportView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_reportView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void btn_log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("CustomReport");
			logsInfoForm.ShowDialog();
		}

		private void btnViewReport_Click(object sender, EventArgs e)
		{
			if (this.dtReports != null && this.dtReports.Rows.Count > 0)
			{
				DataRow[] array = this.dtReports.Select("check=true");
				if (array == null || array.Length == 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectViewReport", "请选择要查看的报表"));
				}
				else if (array.Length > 1)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
				}
				else
				{
					CustomReportView customReportView = new CustomReportView((int)array[0]["id"]);
					customReportView.ShowDialog();
				}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CustomReportList));
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.btnViewReport = new ToolStripButton();
			this.btn_log = new ToolStripButton();
			this.pnl_CustomReport = new PanelEx();
			this.grd_checkInOutView = new GridControl();
			this.grd_reportView = new GridView();
			this.colCheck = new GridColumn();
			this.colReportName = new GridColumn();
			this.colMemo = new GridColumn();
			this.lueVerifyType = new RepositoryItemLookUpEdit();
			this.lueEventType = new RepositoryItemLookUpEdit();
			this.lueUserName = new RepositoryItemLookUpEdit();
			this.lueLastName = new RepositoryItemLookUpEdit();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_checkInOutView).BeginInit();
			((ISupportInitialize)this.grd_reportView).BeginInit();
			((ISupportInitialize)this.lueVerifyType).BeginInit();
			((ISupportInitialize)this.lueEventType).BeginInit();
			((ISupportInitialize)this.lueUserName).BeginInit();
			((ISupportInitialize)this.lueLastName).BeginInit();
			base.SuspendLayout();
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[5]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_delete,
				this.btnViewReport,
				this.btn_log
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(778, 38);
			this.MenuPanelEx.TabIndex = 16;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(65, 35);
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(65, 35);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.Image = (Image)componentResourceManager.GetObject("btn_delete.Image");
			this.btn_delete.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delete.ImageTransparentColor = Color.Magenta;
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(65, 35);
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.btnViewReport.Image = Resources.View;
			this.btnViewReport.ImageScaling = ToolStripItemImageScaling.None;
			this.btnViewReport.ImageTransparentColor = Color.Magenta;
			this.btnViewReport.Name = "btnViewReport";
			this.btnViewReport.Size = new Size(89, 35);
			this.btnViewReport.Text = "查看报表";
			this.btnViewReport.Click += this.btnViewReport_Click;
			this.btn_log.Image = (Image)componentResourceManager.GetObject("btn_log.Image");
			this.btn_log.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_log.ImageTransparentColor = Color.Magenta;
			this.btn_log.Name = "btn_log";
			this.btn_log.Size = new Size(89, 35);
			this.btn_log.Text = "日志记录";
			this.btn_log.Click += this.btn_log_Click;
			this.pnl_CustomReport.CanvasColor = SystemColors.Control;
			this.pnl_CustomReport.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_CustomReport.Dock = DockStyle.Top;
			this.pnl_CustomReport.Location = new Point(0, 38);
			this.pnl_CustomReport.Name = "pnl_CustomReport";
			this.pnl_CustomReport.Size = new Size(778, 23);
			this.pnl_CustomReport.Style.Alignment = StringAlignment.Center;
			this.pnl_CustomReport.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_CustomReport.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_CustomReport.Style.Border = eBorderType.SingleLine;
			this.pnl_CustomReport.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_CustomReport.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_CustomReport.Style.GradientAngle = 90;
			this.pnl_CustomReport.TabIndex = 17;
			this.pnl_CustomReport.Text = "自定义报表";
			this.grd_checkInOutView.Cursor = Cursors.Default;
			this.grd_checkInOutView.Dock = DockStyle.Fill;
			this.grd_checkInOutView.Location = new Point(0, 61);
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
			this.grd_checkInOutView.Size = new Size(778, 381);
			this.grd_checkInOutView.TabIndex = 18;
			this.grd_checkInOutView.TabStop = false;
			this.grd_checkInOutView.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_reportView
			});
			this.grd_reportView.Columns.AddRange(new GridColumn[3]
			{
				this.colCheck,
				this.colReportName,
				this.colMemo
			});
			this.grd_reportView.GridControl = this.grd_checkInOutView;
			this.grd_reportView.IndicatorWidth = 35;
			this.grd_reportView.Name = "grd_reportView";
			this.grd_reportView.OptionsBehavior.Editable = false;
			this.grd_reportView.OptionsView.ShowGroupPanel = false;
			this.grd_reportView.CustomDrawColumnHeader += this.grd_reportView_CustomDrawColumnHeader;
			this.grd_reportView.CustomDrawCell += this.grd_reportView_CustomDrawCell;
			this.grd_reportView.Click += this.grd_reportView_Click;
			this.grd_reportView.DoubleClick += this.grd_reportView_DoubleClick;
			this.colCheck.Caption = " ";
			this.colCheck.Name = "colCheck";
			this.colCheck.Visible = true;
			this.colCheck.VisibleIndex = 0;
			this.colCheck.Width = 50;
			this.colReportName.Caption = "名称";
			this.colReportName.Name = "colReportName";
			this.colReportName.Visible = true;
			this.colReportName.VisibleIndex = 1;
			this.colReportName.Width = 345;
			this.colMemo.Caption = "备注";
			this.colMemo.Name = "colMemo";
			this.colMemo.Visible = true;
			this.colMemo.VisibleIndex = 2;
			this.colMemo.Width = 346;
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
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.grd_checkInOutView);
			base.Controls.Add(this.pnl_CustomReport);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "CustomReportList";
			base.Size = new Size(778, 442);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_checkInOutView).EndInit();
			((ISupportInitialize)this.grd_reportView).EndInit();
			((ISupportInitialize)this.lueVerifyType).EndInit();
			((ISupportInitialize)this.lueEventType).EndInit();
			((ISupportInitialize)this.lueUserName).EndInit();
			((ISupportInitialize)this.lueLastName).EndInit();
			base.ResumeLayout(false);
		}
	}
}
