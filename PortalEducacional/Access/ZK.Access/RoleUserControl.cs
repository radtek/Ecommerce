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
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class RoleUserControl : UserControl
	{
		private AuthGroupBll gbll = new AuthGroupBll(MainForm._ia);

		private AuthUserBll ubll = new AuthUserBll(MainForm._ia);

		private DataTable m_datatable = new DataTable();

		private bool isDouble = false;

		private IContainer components = null;

		public PanelEx MenupanelEx;

		public PanelEx pnl_role;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_name;

		private GridColumn column_remark;

		private ToolStrip toolStrip1;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_del;

		private GridColumn column_check;

		private ContextMenuStrip gv_contextMenu;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		public RoleUserControl()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.LoadDaba();
				this.grd_mainView.SelectAll();
				initLang.LocaleForm(this, base.Name);
				this.menu_add.Text = this.btn_add.Text;
				this.menu_del.Text = this.btn_del.Text;
				this.menu_edit.Text = this.btn_edit.Text;
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
			this.CheckPermission();
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.SysUser))
			{
				this.btn_add.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_del.Enabled = false;
				this.menu_del.Enabled = false;
				this.menu_edit.Enabled = false;
				this.menu_add.Enabled = false;
				this.grd_mainView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("remark");
			this.m_datatable.Columns.Add("check");
			this.column_name.FieldName = "name";
			this.column_remark.FieldName = "remark";
			this.column_check.FieldName = "check";
			this.grd_view.DataSource = this.m_datatable;
		}

		private void LoadDaba()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AuthGroup> modelList = this.gbll.GetModelList("");
				if (modelList == null || modelList.Count == 0)
				{
					AuthGroup authGroup = new AuthGroup();
					authGroup.name = "administrator";
					authGroup.Permission = "333333333333333333333333333333333333333333333333333333333";
					authGroup.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
					this.gbll.Add(authGroup);
					modelList = this.gbll.GetModelList("");
				}
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = modelList[i].id;
						dataRow[1] = modelList[i].name;
						dataRow[2] = modelList[i].Remark;
						dataRow[3] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.grd_mainView.SelectAll();
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_del.Enabled = true;
					this.btn_edit.Enabled = true;
					this.menu_del.Enabled = true;
					this.menu_edit.Enabled = true;
				}
				else
				{
					this.btn_del.Enabled = false;
					this.btn_edit.Enabled = false;
					this.menu_del.Enabled = false;
					this.menu_edit.Enabled = false;
				}
				this.CheckPermission();
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			try
			{
				RoleEdit roleEdit = new RoleEdit(0);
				roleEdit.RefreshDataEvent += this.frmRole_RefreshDataEvent;
				roleEdit.Text = this.btn_add.Text;
				roleEdit.ShowDialog();
				roleEdit.RefreshDataEvent -= this.frmRole_RefreshDataEvent;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
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
						if (int.Parse(this.m_datatable.Rows[array[0]][0].ToString()) == SysInfos.AdminRoleID)
						{
							if (!this.isDouble)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotEditAdminRole", "超级管理员角色不能编辑"));
							}
						}
						else
						{
							RoleEdit roleEdit = new RoleEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							roleEdit.RefreshDataEvent += this.frmRole_RefreshDataEvent;
							roleEdit.Text = this.btn_edit.Text;
							roleEdit.ShowDialog();
							roleEdit.RefreshDataEvent -= this.frmRole_RefreshDataEvent;
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录"));
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

		private void frmRole_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadDaba();
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 1)
				{
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
							{
								if (int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()) == SysInfos.AdminRoleID)
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelRole", "角色不能删除"));
								}
								else if (this.ubll.ExistsRoleID(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString())))
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelRole", "角色不能删除"));
								}
								else
								{
									this.gbll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								}
							}
							this.LoadDaba();
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的记录"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelRole", "角色不能删除"));
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
			this.btn_edit_Click(null, null);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(RoleUserControl));
			this.MenupanelEx = new PanelEx();
			this.toolStrip1 = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_del = new ToolStripButton();
			this.pnl_role = new PanelEx();
			this.grd_view = new GridControl();
			this.gv_contextMenu = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_name = new GridColumn();
			this.column_remark = new GridColumn();
			this.MenupanelEx.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			this.gv_contextMenu.SuspendLayout();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.MenupanelEx.CanvasColor = SystemColors.Control;
			this.MenupanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MenupanelEx.Controls.Add(this.toolStrip1);
			this.MenupanelEx.Dock = DockStyle.Top;
			this.MenupanelEx.Location = new Point(0, 0);
			this.MenupanelEx.Name = "MenupanelEx";
			this.MenupanelEx.Size = new Size(684, 37);
			this.MenupanelEx.Style.Alignment = StringAlignment.Center;
			this.MenupanelEx.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.MenupanelEx.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.MenupanelEx.Style.Border = eBorderType.SingleLine;
			this.MenupanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MenupanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenupanelEx.Style.GradientAngle = 90;
			this.MenupanelEx.TabIndex = 0;
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Items.AddRange(new ToolStripItem[3]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_del
			});
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(684, 38);
			this.toolStrip1.TabIndex = 4;
			this.toolStrip1.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(68, 35);
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(68, 35);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_del.Image = (Image)componentResourceManager.GetObject("btn_del.Image");
			this.btn_del.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_del.ImageTransparentColor = Color.Magenta;
			this.btn_del.Name = "btn_del";
			this.btn_del.Size = new Size(68, 35);
			this.btn_del.Text = "删除";
			this.btn_del.Click += this.btn_delete_Click;
			this.pnl_role.CanvasColor = SystemColors.Control;
			this.pnl_role.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_role.Dock = DockStyle.Top;
			this.pnl_role.Location = new Point(0, 37);
			this.pnl_role.Name = "pnl_role";
			this.pnl_role.Size = new Size(684, 28);
			this.pnl_role.Style.Alignment = StringAlignment.Center;
			this.pnl_role.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_role.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_role.Style.Border = eBorderType.SingleLine;
			this.pnl_role.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_role.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_role.Style.GradientAngle = 90;
			this.pnl_role.TabIndex = 1;
			this.pnl_role.Text = "角色";
			this.grd_view.ContextMenuStrip = this.gv_contextMenu;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 65);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 380);
			this.grd_view.TabIndex = 5;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.gv_contextMenu.Items.AddRange(new ToolStripItem[3]
			{
				this.menu_add,
				this.menu_edit,
				this.menu_del
			});
			this.gv_contextMenu.Name = "gv_contextMenu";
			this.gv_contextMenu.Size = new Size(95, 70);
			this.menu_add.Image = Resources.add;
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(94, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.btn_add_Click;
			this.menu_edit.Image = Resources.edit;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(94, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_del.Image = Resources.delete;
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(94, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
			this.grd_mainView.Columns.AddRange(new GridColumn[3]
			{
				this.column_check,
				this.column_name,
				this.column_remark
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsSelection.MultiSelect = true;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 40;
			this.column_name.Caption = "角色";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 1;
			this.column_name.Width = 302;
			this.column_remark.Caption = "备注";
			this.column_remark.Name = "column_remark";
			this.column_remark.Visible = true;
			this.column_remark.VisibleIndex = 2;
			this.column_remark.Width = 305;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_role);
			base.Controls.Add(this.MenupanelEx);
			base.Name = "RoleUserControl";
			base.Size = new Size(684, 445);
			this.MenupanelEx.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			this.gv_contextMenu.ResumeLayout(false);
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
