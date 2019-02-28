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
using ZK.Access.door;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class TimeSlotUserControl : UserControl
	{
		private DataTable m_datatable = new DataTable();

		private AccTimesegBll bll = new AccTimesegBll(MainForm._ia);

		private bool isDouble = false;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private SaveFileDialog saveFileDialog1;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		public PanelEx pnl_timeZones;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_check;

		private GridColumn column_dev;

		private GridColumn column_interLock;

		public ToolStrip panelEx1;

		public TimeSlotUserControl()
		{
			this.InitializeComponent();
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
				this.btn_delete.Image = ResourceIPC.delete;
				this.btn_edit.Image = ResourceIPC.edit;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel))
			{
				this.btn_add.Enabled = false;
				this.btn_delete.Enabled = false;
				this.btn_edit.Enabled = false;
				this.menu_add.Enabled = false;
				this.menu_del.Enabled = false;
				this.menu_edit.Enabled = false;
				this.grd_mainView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("devicename");
			this.m_datatable.Columns.Add("InterlockType");
			this.m_datatable.Columns.Add("check");
			this.column_dev.FieldName = "devicename";
			this.column_interLock.FieldName = "InterlockType";
			this.column_check.FieldName = "check";
		}

		private void LoadData()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AccTimeseg> list = this.bll.GetModelList("");
				if (list == null || list.Count == 0)
				{
					list = new List<AccTimeseg>();
					AccTimeseg item = this.bll.Add(ShowMsgInfos.GetInfo("SDefaultTZ", "24小时通行"));
					list.Add(item);
					this.btn_delete.Enabled = false;
				}
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = list[i].id;
						dataRow[1] = list[i].timeseg_name;
						dataRow[2] = list[i].memo;
						dataRow[3] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				this.CheckPermission();
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void Add_button_Click(object sender, EventArgs e)
		{
			TimeSet timeSet = new TimeSet(0);
			timeSet.RefreshDataEvent += this.timeSlotForm_RefreshDataEvent;
			timeSet.ShowDialog();
			timeSet.RefreshDataEvent -= this.timeSlotForm_RefreshDataEvent;
		}

		private void timeSlotForm_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadData();
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			try
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
						int num = int.Parse(this.m_datatable.Rows[array[0]][0].ToString());
						AccTimeseg accTimeseg = this.bll.GetdefaultTime();
						TimeSet timeSet = new TimeSet(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
						timeSet.RefreshDataEvent += this.timeSlotForm_RefreshDataEvent;
						timeSet.ShowDialog();
						timeSet.RefreshDataEvent -= this.timeSlotForm_RefreshDataEvent;
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
				}
				else if (!this.isDouble)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (this.m_datatable.Rows.Count > 1)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							AccTimeseg accTimeseg = this.bll.GetdefaultTime();
							bool flag = false;
							for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
							{
								if (int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()) == accTimeseg.id)
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeleteDefaultTZ", "默认时段不能删除"));
								}
								else if (this.bll.IsUsing(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString())))
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeleteIsUingTZ", "时间段 %s 正在使用中,不能删除").Replace("%s", this.m_datatable.Rows[checkedRows[i]][1].ToString()));
								}
								else
								{
									AccTimeseg model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									if (model != null)
									{
										CommandServer.DelCmd(model);
										this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
										flag = true;
									}
								}
							}
							if (flag)
							{
								FrmShowUpdata.Instance.ShowEx();
								this.LoadData();
							}
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeleteDefaultTZ", "默认时间段不能删除"));
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

		private void menu_delall_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 1)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						AccTimeseg accTimeseg = this.bll.GetdefaultTime();
						bool flag = false;
						for (int i = 1; i < this.m_datatable.Rows.Count && this.m_datatable.Rows[i][0] != null; i++)
						{
							if (int.Parse(this.m_datatable.Rows[i][0].ToString()) != accTimeseg.id && !this.bll.IsUsing(int.Parse(this.m_datatable.Rows[i][0].ToString())))
							{
								AccTimeseg model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
								if (model != null)
								{
									CommandServer.DelCmd(model);
									flag = true;
									this.bll.Delete(int.Parse(this.m_datatable.Rows[i][0].ToString()));
								}
							}
						}
						if (flag)
						{
							FrmShowUpdata.Instance.ShowEx();
							this.LoadData();
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeleteDefaultTZ", "默认时间段不能删除"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(sender, e);
			this.isDouble = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TimeSlotUserControl));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.panelEx1 = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.pnl_timeZones = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_interLock = new GridColumn();
			this.contextMenuStrip1.SuspendLayout();
			this.panelEx1.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[3]
			{
				this.menu_add,
				this.menu_edit,
				this.menu_del
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(99, 70);
			this.menu_add.Image = (Image)componentResourceManager.GetObject("menu_add.Image");
			this.menu_add.ImageScaling = ToolStripItemImageScaling.None;
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(98, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.Add_button_Click;
			this.menu_edit.Image = (Image)componentResourceManager.GetObject("menu_edit.Image");
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(98, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_del.Image = (Image)componentResourceManager.GetObject("menu_del.Image");
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(98, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
			this.panelEx1.AutoSize = false;
			this.panelEx1.Items.AddRange(new ToolStripItem[3]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_delete
			});
			this.panelEx1.Location = new Point(0, 0);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(684, 41);
			this.panelEx1.TabIndex = 11;
			this.panelEx1.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(94, 38);
			this.btn_add.Text = "Adicionar";
			this.btn_add.Click += this.Add_button_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(73, 38);
			this.btn_edit.Text = "Editar";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.Image = (Image)componentResourceManager.GetObject("btn_delete.Image");
			this.btn_delete.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delete.ImageTransparentColor = Color.Magenta;
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(77, 38);
			this.btn_delete.Text = "Excluir";
			this.btn_delete.Click += this.btn_delete_Click;
			this.pnl_timeZones.CanvasColor = SystemColors.Control;
			this.pnl_timeZones.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_timeZones.Dock = DockStyle.Top;
			this.pnl_timeZones.Location = new Point(0, 41);
			this.pnl_timeZones.Name = "pnl_timeZones";
			this.pnl_timeZones.Size = new Size(684, 25);
			this.pnl_timeZones.Style.Alignment = StringAlignment.Center;
			this.pnl_timeZones.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_timeZones.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_timeZones.Style.Border = eBorderType.SingleLine;
			this.pnl_timeZones.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_timeZones.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_timeZones.Style.GradientAngle = 90;
			this.pnl_timeZones.TabIndex = 12;
			this.pnl_timeZones.Text = "Zona de Tempo";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 66);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 420);
			this.grd_view.TabIndex = 13;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[3]
			{
				this.column_check,
				this.column_dev,
				this.column_interLock
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 40;
			this.column_dev.Caption = "时间段名称";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 215;
			this.column_interLock.Caption = "备注";
			this.column_interLock.Name = "column_interLock";
			this.column_interLock.Visible = true;
			this.column_interLock.VisibleIndex = 2;
			this.column_interLock.Width = 215;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_timeZones);
			base.Controls.Add(this.panelEx1);
			base.Name = "TimeSlotUserControl";
			base.Size = new Size(684, 486);
			this.contextMenuStrip1.ResumeLayout(false);
			this.panelEx1.ResumeLayout(false);
			this.panelEx1.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
