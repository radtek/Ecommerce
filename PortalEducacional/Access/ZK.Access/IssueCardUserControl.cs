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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class IssueCardUserControl : UserControl
	{
		private string personnelNo;

		private string name;

		private string cardNo;

		private bool isDouble = false;

		private DataTable m_datatable = new DataTable();

		private PersonnelIssuecardBll bll = new PersonnelIssuecardBll(MainForm._ia);

		private WaitForm m_wait = WaitForm.Instance;

		private IContainer components = null;

		public PanelEx pnl_issueCard;

		public PanelEx MenuPanelEx;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_del;

		private ToolStripMenuItem Menu_export;

		private ToolStripMenuItem Menu_issueCard;

		private ToolStripMenuItem Menu_log;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_badgenumber;

		private GridColumn column_name;

		private GridColumn column_code;

		private GridColumn column_deptname;

		private GridColumn column_cardno;

		private GridColumn column_create_time;

		private ToolStrip toolStrip1;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_del;

		private ToolStripButton btn_export;

		private ToolStripButton btn_log;

		private GridColumn column_check;

		private GridColumn column_lastName;

		public ToolStripButton btn_issueCard;

		public IssueCardUserControl()
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
			}
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.LoadData();
				initLang.LocaleForm(this, base.Name);
				this.ChangeSking();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
			this.CheckPermission();
		}

		private void ChangeSking()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_add.Image = ResourceIPC.add;
				this.btn_del.Image = ResourceIPC.delete;
				this.btn_edit.Image = ResourceIPC.edit;
				this.btn_export.Image = ResourceIPC.Export;
				this.btn_log.Image = ResourceIPC.Log_Entries;
				this.btn_issueCard.Image = ResourceIPC.Issue_Card1;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Personnel))
			{
				this.btn_add.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_del.Enabled = false;
				this.btn_issueCard.Enabled = false;
				this.Menu_add.Enabled = false;
				this.Menu_del.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_issueCard.Enabled = false;
				this.btn_export.Enabled = false;
				this.btn_log.Enabled = false;
				this.btn_export.Enabled = false;
				this.Menu_export.Enabled = false;
				this.Menu_log.Enabled = false;
				this.grd_view.DoubleClick -= this.grd_view_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("badgenumber").DataType = typeof(int);
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("lastname");
			this.m_datatable.Columns.Add("code");
			this.m_datatable.Columns.Add("deptname");
			this.m_datatable.Columns.Add("cardno");
			this.m_datatable.Columns.Add("create_time");
			this.m_datatable.Columns.Add("check");
			this.column_badgenumber.FieldName = "badgenumber";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_code.FieldName = "code";
			this.column_deptname.FieldName = "deptname";
			this.column_cardno.FieldName = "cardno";
			this.column_create_time.FieldName = "create_time";
			this.column_check.FieldName = "check";
		}

		private void LoadData()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				DataSet allList = this.bll.GetAllList();
				if (allList != null && allList.Tables.Count > 0)
				{
					DataTable dataTable = allList.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = dataTable.Rows[i]["id"].ToString();
						if (dataTable.Rows[i]["badgenumber"] != DBNull.Value)
						{
							dataRow[1] = dataTable.Rows[i]["badgenumber"].ToString();
							dataRow[2] = dataTable.Rows[i]["name"].ToString();
							dataRow[3] = dataTable.Rows[i]["lastname"].ToString();
							dataRow[4] = dataTable.Rows[i]["code"].ToString();
							dataRow[5] = dataTable.Rows[i]["deptname"].ToString();
							dataRow[6] = dataTable.Rows[i]["cardno"].ToString();
							dataRow[7] = dataTable.Rows[i]["create_time"].ToString();
							dataRow[8] = false;
							this.m_datatable.Rows.Add(dataRow);
						}
						else
						{
							this.bll.Delete(int.Parse(dataTable.Rows[i]["id"].ToString()));
						}
					}
				}
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_edit.Enabled = true;
					this.btn_export.Enabled = true;
					this.btn_del.Enabled = true;
					this.Menu_edit.Enabled = true;
					this.Menu_del.Enabled = true;
					this.Menu_export.Enabled = true;
				}
				else
				{
					this.btn_edit.Enabled = false;
					this.btn_export.Enabled = false;
					this.btn_del.Enabled = false;
					this.Menu_edit.Enabled = false;
					this.Menu_export.Enabled = false;
					this.Menu_del.Enabled = false;
				}
				this.CheckPermission();
				this.grd_view.DataSource = this.m_datatable;
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void addIssueForm_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadData();
		}

		private void issueCardForm_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadData();
		}

		private void issueCardSearchForm_SearchIssueEvent(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					PersonnelIssuecard personnelIssuecard = sender as PersonnelIssuecard;
					this.name = personnelIssuecard.name;
					this.personnelNo = personnelIssuecard.personnelNo;
					this.cardNo = personnelIssuecard.cardno;
				}
				this.LoadData();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_del_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						for (int i = 0; i < checkedRows.Length; i++)
						{
							this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
							if (checkedRows[i] < 0 || checkedRows[i] >= this.m_datatable.Rows.Count)
							{
								break;
							}
							this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + this.m_datatable.Rows[checkedRows[i]][1].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						}
						this.LoadData();
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_export_Click(object sender, EventArgs e)
		{
			object[] obj = new object[8]
			{
				this.pnl_issueCard.Text,
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
			DevExpressHelper.OutData(this.grd_mainView, fileName);
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			AddIssueForm addIssueForm = new AddIssueForm();
			addIssueForm.RefreshDataEvent += this.addIssueForm_RefreshDataEvent;
			addIssueForm.ShowDialog();
			addIssueForm.RefreshDataEvent -= this.addIssueForm_RefreshDataEvent;
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 0)
				{
					int[] array = null;
					if (this.isDouble)
					{
						array = this.grd_mainView.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.grd_mainView, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
					{
						if (array.Length == 1)
						{
							AddIssueForm addIssueForm = new AddIssueForm(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							addIssueForm.RefreshDataEvent += this.addIssueForm_RefreshDataEvent;
							addIssueForm.ShowDialog();
							addIssueForm.RefreshDataEvent -= this.addIssueForm_RefreshDataEvent;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
					}
				}
				else if (!this.isDouble)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
		}

		private void btn_issueCard_Click(object sender, EventArgs e)
		{
			BatchIssueCardForm batchIssueCardForm = new BatchIssueCardForm();
			batchIssueCardForm.RefreshDataEvent += this.issueCardForm_RefreshDataEvent;
			batchIssueCardForm.ShowDialog();
			batchIssueCardForm.RefreshDataEvent -= this.issueCardForm_RefreshDataEvent;
		}

		private void grd_mainView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void grd_mainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_mainView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void btn_log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("IssueCard");
			logsInfoForm.ShowDialog();
		}

		private void grd_view_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(sender, null);
			this.isDouble = false;
		}

		private void Menu_edit_Click(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(sender, null);
			this.isDouble = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(IssueCardUserControl));
			GridLevelNode gridLevelNode = new GridLevelNode();
			this.MenuPanelEx = new PanelEx();
			this.toolStrip1 = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_del = new ToolStripButton();
			this.btn_export = new ToolStripButton();
			this.btn_issueCard = new ToolStripButton();
			this.btn_log = new ToolStripButton();
			this.pnl_issueCard = new PanelEx();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_del = new ToolStripMenuItem();
			this.Menu_export = new ToolStripMenuItem();
			this.Menu_issueCard = new ToolStripMenuItem();
			this.Menu_log = new ToolStripMenuItem();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_badgenumber = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_code = new GridColumn();
			this.column_deptname = new GridColumn();
			this.column_cardno = new GridColumn();
			this.column_create_time = new GridColumn();
			this.MenuPanelEx.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.MenuPanelEx.CanvasColor = SystemColors.Control;
			this.MenuPanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MenuPanelEx.Controls.Add(this.toolStrip1);
			this.MenuPanelEx.Dock = DockStyle.Top;
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 35);
			this.MenuPanelEx.Style.Alignment = StringAlignment.Center;
			this.MenuPanelEx.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.MenuPanelEx.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.MenuPanelEx.Style.Border = eBorderType.SingleLine;
			this.MenuPanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MenuPanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenuPanelEx.Style.GradientAngle = 90;
			this.MenuPanelEx.TabIndex = 0;
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Items.AddRange(new ToolStripItem[6]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_del,
				this.btn_export,
				this.btn_issueCard,
				this.btn_log
			});
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(684, 35);
			this.toolStrip1.TabIndex = 8;
			this.toolStrip1.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(65, 32);
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(65, 32);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_del.Image = (Image)componentResourceManager.GetObject("btn_del.Image");
			this.btn_del.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_del.ImageTransparentColor = Color.Magenta;
			this.btn_del.Name = "btn_del";
			this.btn_del.Size = new Size(65, 32);
			this.btn_del.Text = "删除";
			this.btn_del.Click += this.btn_del_Click;
			this.btn_export.Image = (Image)componentResourceManager.GetObject("btn_export.Image");
			this.btn_export.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_export.ImageTransparentColor = Color.Magenta;
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(65, 32);
			this.btn_export.Text = "导出";
			this.btn_export.Click += this.btn_export_Click;
			this.btn_issueCard.Image = (Image)componentResourceManager.GetObject("btn_issueCard.Image");
			this.btn_issueCard.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_issueCard.ImageTransparentColor = Color.Magenta;
			this.btn_issueCard.Name = "btn_issueCard";
			this.btn_issueCard.Size = new Size(89, 32);
			this.btn_issueCard.Text = "批量发卡";
			this.btn_issueCard.Click += this.btn_issueCard_Click;
			this.btn_log.Image = (Image)componentResourceManager.GetObject("btn_log.Image");
			this.btn_log.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_log.ImageTransparentColor = Color.Magenta;
			this.btn_log.Name = "btn_log";
			this.btn_log.Size = new Size(89, 32);
			this.btn_log.Text = "日志记录";
			this.btn_log.Click += this.btn_log_Click;
			this.pnl_issueCard.CanvasColor = SystemColors.Control;
			this.pnl_issueCard.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_issueCard.Dock = DockStyle.Top;
			this.pnl_issueCard.Location = new Point(0, 35);
			this.pnl_issueCard.Name = "pnl_issueCard";
			this.pnl_issueCard.Size = new Size(684, 23);
			this.pnl_issueCard.Style.Alignment = StringAlignment.Center;
			this.pnl_issueCard.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_issueCard.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_issueCard.Style.Border = eBorderType.SingleLine;
			this.pnl_issueCard.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_issueCard.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_issueCard.Style.GradientAngle = 90;
			this.pnl_issueCard.TabIndex = 1;
			this.pnl_issueCard.Text = "人员发卡";
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[6]
			{
				this.Menu_add,
				this.Menu_edit,
				this.Menu_del,
				this.Menu_export,
				this.Menu_issueCard,
				this.Menu_log
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(119, 136);
			this.Menu_add.Image = Resources.add;
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(118, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.btn_add_Click;
			this.Menu_edit.Image = Resources.edit;
			this.Menu_edit.Name = "Menu_edit";
			this.Menu_edit.Size = new Size(118, 22);
			this.Menu_edit.Text = "编辑";
			this.Menu_edit.Click += this.Menu_edit_Click;
			this.Menu_del.Image = Resources.delete;
			this.Menu_del.Name = "Menu_del";
			this.Menu_del.Size = new Size(118, 22);
			this.Menu_del.Text = "删除";
			this.Menu_del.Click += this.btn_del_Click;
			this.Menu_export.Image = Resources.Import;
			this.Menu_export.Name = "Menu_export";
			this.Menu_export.Size = new Size(118, 22);
			this.Menu_export.Text = "导出";
			this.Menu_export.Click += this.btn_export_Click;
			this.Menu_issueCard.Image = (Image)componentResourceManager.GetObject("Menu_issueCard.Image");
			this.Menu_issueCard.Name = "Menu_issueCard";
			this.Menu_issueCard.Size = new Size(118, 22);
			this.Menu_issueCard.Text = "批量发卡";
			this.Menu_issueCard.Click += this.btn_issueCard_Click;
			this.Menu_log.Image = Resources.Log_Entries;
			this.Menu_log.Name = "Menu_log";
			this.Menu_log.Size = new Size(118, 22);
			this.Menu_log.Text = "日志记录";
			this.Menu_log.Click += this.btn_log_Click;
			this.grd_view.Dock = DockStyle.Fill;
			gridLevelNode.RelationName = "Level1";
			this.grd_view.LevelTree.Nodes.AddRange(new GridLevelNode[1]
			{
				gridLevelNode
			});
			this.grd_view.Location = new Point(0, 58);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 391);
			this.grd_view.TabIndex = 4;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_view.DoubleClick += this.grd_view_DoubleClick;
			this.grd_mainView.Columns.AddRange(new GridColumn[8]
			{
				this.column_check,
				this.column_badgenumber,
				this.column_name,
				this.column_lastName,
				this.column_code,
				this.column_deptname,
				this.column_cardno,
				this.column_create_time
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsBehavior.Editable = false;
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.column_check.Caption = "gridColumn1";
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 20;
			this.column_badgenumber.AppearanceCell.Options.UseTextOptions = true;
			this.column_badgenumber.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			this.column_badgenumber.Caption = "人员编号";
			this.column_badgenumber.Name = "column_badgenumber";
			this.column_badgenumber.Visible = true;
			this.column_badgenumber.VisibleIndex = 1;
			this.column_badgenumber.Width = 107;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 2;
			this.column_name.Width = 107;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_code.Caption = "部门编号";
			this.column_code.Name = "column_code";
			this.column_code.Visible = true;
			this.column_code.VisibleIndex = 5;
			this.column_code.Width = 107;
			this.column_deptname.Caption = "部门名称";
			this.column_deptname.Name = "column_deptname";
			this.column_deptname.Visible = true;
			this.column_deptname.VisibleIndex = 6;
			this.column_deptname.Width = 107;
			this.column_cardno.Caption = "卡号";
			this.column_cardno.Name = "column_cardno";
			this.column_cardno.Visible = true;
			this.column_cardno.VisibleIndex = 4;
			this.column_cardno.Width = 107;
			this.column_create_time.Caption = "发卡日期";
			this.column_create_time.Name = "column_create_time";
			this.column_create_time.Visible = true;
			this.column_create_time.VisibleIndex = 7;
			this.column_create_time.Width = 111;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			this.ContextMenuStrip = this.contextMenuStrip1;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_issueCard);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "IssueCardUserControl";
			base.Size = new Size(684, 449);
			this.MenuPanelEx.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.contextMenuStrip1.ResumeLayout(false);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
