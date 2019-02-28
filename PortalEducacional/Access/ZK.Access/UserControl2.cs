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
using ZK.Access.system;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class UserControl2 : UserControl
	{
		private AuthUserBll bll = new AuthUserBll(MainForm._ia);

		private DataTable m_datatable = new DataTable();

		private Dictionary<int, AuthGroup> mlist = new Dictionary<int, AuthGroup>();

		private bool isDouble = false;

		private IContainer components = null;

		public PanelEx pnl_user;

		public PanelEx MenupanelEx;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_name;

		private GridColumn column_role;

		private GridColumn column_remark;

		private GridColumn column_date;

		private ToolStrip toolStrip1;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_del;

		private ToolStripButton btn_dept;

		private ToolStripButton btn_area;

		private GridColumn column_check;

		private ContextMenuStrip gv_contextMenu;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		private ToolStripMenuItem menu_dept;

		private ToolStripMenuItem menu_area;

		public UserControl2()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.InitGroup();
				this.LoadDaba();
				this.grd_mainView.SelectAll();
				initLang.LocaleForm(this, base.Name);
				this.menu_add.Text = this.btn_add.Text;
				this.menu_del.Text = this.btn_del.Text;
				this.menu_edit.Text = this.btn_edit.Text;
				this.menu_area.Text = this.btn_area.Text;
				this.menu_dept.Text = this.btn_dept.Text;
				this.menu_area.Image = this.btn_area.Image;
				this.menu_dept.Image = this.btn_dept.Image;
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
				this.btn_area.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_del.Enabled = false;
				this.btn_dept.Enabled = false;
				this.menu_del.Enabled = false;
				this.menu_edit.Enabled = false;
				this.menu_add.Enabled = false;
				this.menu_area.Enabled = false;
				this.menu_dept.Enabled = false;
				this.grd_mainView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("remark");
			this.m_datatable.Columns.Add("roleId");
			this.m_datatable.Columns.Add("lastdate");
			this.m_datatable.Columns.Add("check");
			this.column_name.FieldName = "name";
			this.column_remark.FieldName = "remark";
			this.column_date.FieldName = "lastdate";
			this.column_role.FieldName = "roleId";
			this.column_check.FieldName = "check";
			this.grd_view.DataSource = this.m_datatable;
		}

		private void InitGroup()
		{
			try
			{
				this.mlist.Clear();
				AuthGroupBll authGroupBll = new AuthGroupBll(MainForm._ia);
				List<AuthGroup> modelList = authGroupBll.GetModelList("");
				if (modelList == null || modelList.Count == 0)
				{
					AuthGroup authGroup = new AuthGroup();
					authGroup.name = "administrator";
					authGroup.Permission = "333333333333333333333333333333333333333333333333333333333";
					authGroup.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
					authGroupBll.Add(authGroup);
					modelList = authGroupBll.GetModelList("");
				}
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.mlist.ContainsKey(modelList[i].id))
						{
							this.mlist.Add(modelList[i].id, modelList[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadDaba()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AuthUser> modelList = this.bll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = modelList[i].id;
						dataRow[1] = modelList[i].username;
						dataRow[2] = modelList[i].Remark;
						if (this.mlist.ContainsKey(modelList[i].RoleID))
						{
							dataRow[3] = this.mlist[modelList[i].RoleID].name;
						}
						dataRow[4] = modelList[i].last_login;
						dataRow[5] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_del.Enabled = true;
					this.btn_edit.Enabled = true;
					this.menu_del.Enabled = true;
					this.menu_edit.Enabled = true;
					this.menu_area.Enabled = true;
					this.menu_dept.Enabled = true;
				}
				else
				{
					this.btn_del.Enabled = false;
					this.btn_edit.Enabled = false;
					this.menu_del.Enabled = false;
					this.menu_edit.Enabled = false;
					this.menu_area.Enabled = false;
					this.menu_dept.Enabled = false;
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
			UserEdit userEdit = new UserEdit(0);
			userEdit.RefreshDataEvent += this.frmUser_RefreshDataEvent;
			userEdit.Text = this.btn_add.Text;
			userEdit.ShowDialog();
			userEdit.RefreshDataEvent -= this.frmUser_RefreshDataEvent;
		}

		private void frmUser_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadDaba();
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
							if (this.m_datatable.Rows[array[0]][1].ToString().ToLower() == "admin")
							{
								if (!this.isDouble)
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotEditAdmin", "不能编辑超级管理员"));
								}
							}
							else
							{
								UserEdit userEdit = new UserEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
								userEdit.RefreshDataEvent += this.frmUser_RefreshDataEvent;
								userEdit.Text = this.btn_edit.Text;
								userEdit.ShowDialog();
								userEdit.RefreshDataEvent -= this.frmUser_RefreshDataEvent;
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
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
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
				if (this.m_datatable.Rows.Count > 0)
				{
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
							{
								if (this.m_datatable.Rows[checkedRows[i]][1].ToString().ToLower() == "admin")
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelAdmin", "不能删除超级管理员"));
								}
								else if (SysInfos.SysUserInfo != null && SysInfos.SysUserInfo.id == int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()))
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelMySelf", "不能删除自己"));
								}
								else
								{
									this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
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
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
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
				if (this.m_datatable.Rows.Count > 0)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						for (int i = 0; i < this.m_datatable.Rows.Count && this.m_datatable.Rows[i][0] != null; i++)
						{
							if (this.m_datatable.Rows[i][1].ToString().ToLower() == "admin")
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelAdmin", "不能删除超级管理员"));
							}
							else if (SysInfos.SysUserInfo != null && SysInfos.SysUserInfo.id == int.Parse(this.m_datatable.Rows[i][0].ToString()))
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDel", "不能删除"));
							}
							else
							{
								this.bll.Delete(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							}
						}
						this.m_datatable.Rows.Clear();
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_dept_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length == 1)
					{
						if (this.m_datatable.Rows[checkedRows[0]][1].ToString().ToLower() == "admin")
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAdminControlAll", "超级管理员可管理所有部门，不需要设置"));
						}
						else
						{
							UserDeptManager userDeptManager = new UserDeptManager(int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString()));
							userDeptManager.ShowDialog();
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_area_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length == 1)
					{
						if (this.m_datatable.Rows[checkedRows[0]][1].ToString().ToLower() == "admin")
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAdminControlAllArea", "超级管理员可管理所有区域，不需要设置"));
						}
						else
						{
							UserAreaManager userAreaManager = new UserAreaManager(int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString()));
							userAreaManager.ShowDialog();
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择记录"));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserControl2));
			this.MenupanelEx = new PanelEx();
			this.toolStrip1 = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_del = new ToolStripButton();
			this.btn_dept = new ToolStripButton();
			this.btn_area = new ToolStripButton();
			this.pnl_user = new PanelEx();
			this.grd_view = new GridControl();
			this.gv_contextMenu = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.menu_dept = new ToolStripMenuItem();
			this.menu_area = new ToolStripMenuItem();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_name = new GridColumn();
			this.column_role = new GridColumn();
			this.column_remark = new GridColumn();
			this.column_date = new GridColumn();
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
			this.MenupanelEx.Size = new Size(684, 35);
			this.MenupanelEx.Style.Alignment = StringAlignment.Center;
			this.MenupanelEx.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.MenupanelEx.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.MenupanelEx.Style.Border = eBorderType.SingleLine;
			this.MenupanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MenupanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenupanelEx.Style.GradientAngle = 90;
			this.MenupanelEx.TabIndex = 0;
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Items.AddRange(new ToolStripItem[5]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_del,
				this.btn_dept,
				this.btn_area
			});
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(684, 38);
			this.toolStrip1.TabIndex = 5;
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
			this.btn_dept.Image = (Image)componentResourceManager.GetObject("btn_dept.Image");
			this.btn_dept.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_dept.ImageTransparentColor = Color.Magenta;
			this.btn_dept.Name = "btn_dept";
			this.btn_dept.Size = new Size(92, 35);
			this.btn_dept.Text = "管理部门";
			this.btn_dept.Click += this.btn_dept_Click;
			this.btn_area.Image = (Image)componentResourceManager.GetObject("btn_area.Image");
			this.btn_area.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_area.ImageTransparentColor = Color.Magenta;
			this.btn_area.Name = "btn_area";
			this.btn_area.Size = new Size(92, 35);
			this.btn_area.Text = "管理区域";
			this.btn_area.Click += this.btn_area_Click;
			this.pnl_user.CanvasColor = SystemColors.Control;
			this.pnl_user.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_user.Dock = DockStyle.Top;
			this.pnl_user.Location = new Point(0, 35);
			this.pnl_user.Name = "pnl_user";
			this.pnl_user.Size = new Size(684, 26);
			this.pnl_user.Style.Alignment = StringAlignment.Center;
			this.pnl_user.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_user.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_user.Style.Border = eBorderType.SingleLine;
			this.pnl_user.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_user.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_user.Style.GradientAngle = 90;
			this.pnl_user.TabIndex = 1;
			this.pnl_user.Text = "用户";
			this.grd_view.ContextMenuStrip = this.gv_contextMenu;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 384);
			this.grd_view.TabIndex = 6;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.gv_contextMenu.Items.AddRange(new ToolStripItem[5]
			{
				this.menu_add,
				this.menu_edit,
				this.menu_del,
				this.menu_dept,
				this.menu_area
			});
			this.gv_contextMenu.Name = "gv_contextMenu";
			this.gv_contextMenu.Size = new Size(119, 114);
			this.menu_add.Image = Resources.add;
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(118, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.btn_add_Click;
			this.menu_edit.Image = Resources.edit;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(118, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_del.Image = Resources.delete;
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(118, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
			this.menu_dept.Name = "menu_dept";
			this.menu_dept.Size = new Size(118, 22);
			this.menu_dept.Text = "管理部门";
			this.menu_dept.Click += this.btn_dept_Click;
			this.menu_area.Name = "menu_area";
			this.menu_area.Size = new Size(118, 22);
			this.menu_area.Text = "管理区域";
			this.menu_area.Click += this.btn_area_Click;
			this.grd_mainView.Columns.AddRange(new GridColumn[5]
			{
				this.column_check,
				this.column_name,
				this.column_role,
				this.column_remark,
				this.column_date
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
			this.column_name.Caption = "账号";
			this.column_name.Name = "column_name";
			this.column_name.SummaryItem.SummaryType = SummaryItemType.Count;
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 1;
			this.column_name.Width = 120;
			this.column_role.Caption = "角色";
			this.column_role.Name = "column_role";
			this.column_role.Visible = true;
			this.column_role.VisibleIndex = 2;
			this.column_role.Width = 120;
			this.column_remark.Caption = "备注说明";
			this.column_remark.Name = "column_remark";
			this.column_remark.Visible = true;
			this.column_remark.VisibleIndex = 4;
			this.column_remark.Width = 120;
			this.column_date.Caption = "最近一次登录时间";
			this.column_date.Name = "column_date";
			this.column_date.Visible = true;
			this.column_date.VisibleIndex = 3;
			this.column_date.Width = 127;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.ActiveCaptionText;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_user);
			base.Controls.Add(this.MenupanelEx);
			base.Name = "UserControl2";
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
