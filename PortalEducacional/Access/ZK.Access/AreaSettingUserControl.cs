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
using System.Text;
using System.Windows.Forms;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class AreaSettingUserControl : UserControl
	{
		private string AreaName;

		private string AreaCode;

		private string AreaRemarks;

		private TreeView tree = new TreeView();

		private NodeManager nodemg = new NodeManager();

		private DataTable m_datatable = new DataTable();

		private int defalutid = 0;

		private List<int> areaIDList = new List<int>();

		private bool isDouble = false;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_LogEntries;

		private ToolStripMenuItem Menu_Export;

		private ToolStrip miniToolStrip;

		private ToolStripButton btn_setting;

		private ToolStripButton btn_LogEntries;

		private ToolStripButton btn_Export;

		public PanelEx panelEx1;

		private GridControl grd_view;

		private GridView grd_personnelAreaView;

		private GridColumn column_areaid;

		private GridColumn column_areaname;

		private GridColumn column_parentName;

		private GridColumn column_remark;

		public ToolStrip MenuPanelEx;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_del;

		private GridColumn column_check;

		private ToolStripMenuItem Menu_treeShow;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_delete;

		public AreaSettingUserControl()
		{
			try
			{
				this.InitializeComponent();
				DevExpressHelper.InitImageList(this.grd_personnelAreaView, "column_check");
				this.InitDataTableSet();
				this.DataBind();
				initLang.LocaleForm(this, base.Name);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.CheckPermission();
		}

		private void CheckPermission()
		{
			if (SysInfos.SysUserInfo.id != SysInfos.AdminID && !SysInfos.IsOwerControlPermission(SysInfos.Device))
			{
				this.btn_setting.Enabled = false;
				this.Menu_treeShow.Enabled = false;
				this.grd_view.DoubleClick -= this.grd_view_DoubleClick;
				this.btn_add.Enabled = false;
				this.btn_edit.Enabled = false;
				this.Menu_add.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.btn_del.Enabled = false;
				this.btn_Export.Enabled = false;
				this.btn_LogEntries.Enabled = false;
				this.btn_setting.Enabled = false;
				this.Menu_delete.Enabled = false;
				this.Menu_Export.Enabled = false;
				this.Menu_LogEntries.Enabled = false;
				this.Menu_treeShow.Enabled = false;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("areaid");
			this.m_datatable.Columns.Add("areaname");
			this.m_datatable.Columns.Add("parentName");
			this.m_datatable.Columns.Add("remark");
			this.m_datatable.Columns.Add("check");
			this.m_datatable.Columns.Add("parent_id");
			this.column_areaid.FieldName = "areaid";
			this.column_areaname.FieldName = "areaname";
			this.column_parentName.FieldName = "parentName";
			this.column_remark.FieldName = "remark";
			this.column_check.FieldName = "check";
		}

		private void DataBind()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
				DataSet allList = personnelAreaBll.GetAllList();
				this.defalutid = -1;
				if (SysInfos.AdminID == SysInfos.SysUserInfo.id && (allList == null || allList.Tables.Count == 0 || allList.Tables[0].Rows.Count == 0))
				{
					PersonnelArea personnelArea = new PersonnelArea();
					personnelArea.areaid = "1";
					personnelArea.areaname = ShowMsgInfos.GetInfo("Sarea", "总部");
					personnelArea.parent_id = 0;
					personnelAreaBll.Add(personnelArea);
					allList = personnelAreaBll.GetAllList();
				}
				if (allList != null && allList.Tables.Count > 0)
				{
					DataTable dataTable = allList.Tables[0];
					this.nodemg = new NodeManager();
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = dataTable.Rows[i]["id"].ToString();
						dataRow[1] = dataTable.Rows[i]["areaid"].ToString();
						dataRow[2] = dataTable.Rows[i]["areaname"].ToString();
						dataRow[3] = dataTable.Rows[i]["parentName"].ToString();
						dataRow[4] = dataTable.Rows[i]["remark"].ToString();
						dataRow[5] = false;
						if (dataTable.Rows[i]["parent_id"].ToString() == "0" || dataTable.Rows[i]["parent_id"].ToString() == "1")
						{
							this.defalutid = int.Parse(dataTable.Rows[i]["id"].ToString());
						}
						this.m_datatable.Rows.Add(dataRow);
						NodeBase nodeBase = new NodeBase();
						nodeBase.ID = dataTable.Rows[i]["id"].ToString();
						nodeBase.Name = nodeBase.ID;
						nodeBase.Tag = nodeBase.ID;
						nodeBase.ParentNodeID = dataTable.Rows[i]["parent_id"].ToString();
						this.nodemg.Datasouce.Add(nodeBase);
					}
					if (this.nodemg.Bind())
					{
						this.nodemg.ConvertToTree(this.tree);
					}
					if (this.defalutid == -1)
					{
						this.defalutid = int.Parse(dataTable.Rows[0]["id"].ToString());
					}
					this.grd_view.DataSource = this.m_datatable;
				}
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool Exist(object id)
		{
			try
			{
				if (id != null)
				{
					this.areaIDList.Add(int.Parse(id.ToString()));
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					if (machinesBll.ExistsArea(id.ToString()))
					{
						this.areaIDList.Clear();
						return true;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool IsHaveMachines(INode node)
		{
			if (node == null)
			{
				return false;
			}
			if (this.Exist(node.Tag))
			{
				return true;
			}
			if (node.Childs.Count > 0)
			{
				for (int i = 0; i < node.Childs.Count; i++)
				{
					if (this.IsHaveMachines(node.Childs[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void RefreshDataEvent(object sender, EventArgs e)
		{
			if (sender != null)
			{
				PersonnelArea personnelArea = sender as PersonnelArea;
				if (personnelArea != null)
				{
					this.AreaCode = personnelArea.areaid;
					this.AreaName = personnelArea.areaname;
					this.AreaRemarks = personnelArea.remark;
				}
			}
			this.DataBind();
		}

		private void btn_DeleteArea_Click(object sender, EventArgs e)
		{
			PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
			int[] selectedRows = this.grd_personnelAreaView.GetSelectedRows();
			selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.grd_personnelAreaView, selectedRows);
			if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < this.m_datatable.Rows.Count && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				for (int i = 0; i < selectedRows.Length; i++)
				{
					personnelAreaBll.DeleteByAreaId(this.m_datatable.Rows[selectedRows[i]][0].ToString());
				}
				this.DataBind();
			}
		}

		private void btn_setting_Click(object sender, EventArgs e)
		{
			AreaTreeForm areaTreeForm = new AreaTreeForm();
			areaTreeForm.refreshDataEvent += this.RefreshDataEvent;
			areaTreeForm.ShowDialog();
			areaTreeForm.refreshDataEvent += this.RefreshDataEvent;
		}

		private void btn_Search_Click(object sender, EventArgs e)
		{
			SearchAreaForm searchAreaForm = new SearchAreaForm();
			searchAreaForm.SearchAreaEvent += this.RefreshDataEvent;
			searchAreaForm.ShowDialog();
			searchAreaForm.SearchAreaEvent -= this.RefreshDataEvent;
		}

		private void btn_Export_Click(object sender, EventArgs e)
		{
			object[] obj = new object[8]
			{
				this.panelEx1.Text,
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
			DevExpressHelper.OutData(this.grd_personnelAreaView, fileName);
		}

		private void btn_LogEntries_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("PersonnelArea");
			logsInfoForm.ShowDialog();
			this.DataBind();
		}

		private void grd_view_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(null, null);
			this.isDouble = false;
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			AddAreaForm addAreaForm = new AddAreaForm(0, int.Parse("1"));
			addAreaForm.ShowDialog();
			addAreaForm.Close();
			this.DataBind();
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			try
			{
				int[] array = null;
				if (this.isDouble)
				{
					array = this.grd_personnelAreaView.GetSelectedRows();
					array = DevExpressHelper.GetDataSourceRowIndexs(this.grd_personnelAreaView, array);
				}
				else
				{
					array = DevExpressHelper.GetCheckedRows(this.grd_personnelAreaView, "check");
				}
				if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
				{
					if (array.Length == 1)
					{
						string s = this.m_datatable.Rows[array[0]][0].ToString();
						AddAreaForm addAreaForm = new AddAreaForm(int.Parse(s), 0);
						addAreaForm.ShowDialog();
						this.DataBind();
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
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_del_Click(object sender, EventArgs e)
		{
			try
			{
				PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_personnelAreaView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (this.m_datatable.Rows.Count > 1)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
							{
								if (this.m_datatable.Rows[checkedRows[i]][1].ToString() == "0" || this.m_datatable.Rows[checkedRows[i]][1].ToString() == "-1")
								{
									if (stringBuilder.Length < 100)
									{
										stringBuilder.Append(this.m_datatable.Rows[checkedRows[i]][2].ToString() + ":" + ShowMsgInfos.GetInfo("SCannotDelRootArea", "不能删除根区域") + "\r\n");
									}
								}
								else
								{
									this.areaIDList.Clear();
									INode node = this.nodemg.NTree.FindNode(this.m_datatable.Rows[checkedRows[i]][0].ToString());
									if (node == null)
									{
										personnelAreaBll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									}
									else if (!this.IsHaveMachines(node))
									{
										foreach (int areaID in this.areaIDList)
										{
											personnelAreaBll.Delete(areaID);
										}
									}
									else if (stringBuilder.Length < 100)
									{
										stringBuilder.Append(this.m_datatable.Rows[checkedRows[i]][2].ToString() + ":" + ShowMsgInfos.GetInfo("SAreaHasDevice", "该区域还有设备，不能删除!") + "\r\n");
									}
								}
							}
							if (!string.IsNullOrEmpty(stringBuilder.ToString()))
							{
								if (stringBuilder.Length > 100)
								{
									stringBuilder.Append("......\r\n");
								}
								SysDialogs.ShowWarningMessage(stringBuilder.ToString());
							}
							this.DataBind();
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelRootArea", "不能删除根区域"));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AreaSettingUserControl));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_treeShow = new ToolStripMenuItem();
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_delete = new ToolStripMenuItem();
			this.Menu_Export = new ToolStripMenuItem();
			this.Menu_LogEntries = new ToolStripMenuItem();
			this.miniToolStrip = new ToolStrip();
			this.MenuPanelEx = new ToolStrip();
			this.btn_setting = new ToolStripButton();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_del = new ToolStripButton();
			this.btn_Export = new ToolStripButton();
			this.btn_LogEntries = new ToolStripButton();
			this.panelEx1 = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_personnelAreaView = new GridView();
			this.column_check = new GridColumn();
			this.column_areaid = new GridColumn();
			this.column_areaname = new GridColumn();
			this.column_parentName = new GridColumn();
			this.column_remark = new GridColumn();
			this.contextMenuStrip1.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_personnelAreaView).BeginInit();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[6]
			{
				this.Menu_treeShow,
				this.Menu_add,
				this.Menu_edit,
				this.Menu_delete,
				this.Menu_Export,
				this.Menu_LogEntries
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(119, 136);
			this.Menu_treeShow.Image = (Image)componentResourceManager.GetObject("Menu_treeShow.Image");
			this.Menu_treeShow.Name = "Menu_treeShow";
			this.Menu_treeShow.Size = new Size(118, 22);
			this.Menu_treeShow.Text = "树型显示";
			this.Menu_treeShow.Click += this.btn_setting_Click;
			this.Menu_add.Image = Resources.add;
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(118, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.btn_add_Click;
			this.Menu_edit.Image = Resources.edit;
			this.Menu_edit.Name = "Menu_edit";
			this.Menu_edit.Size = new Size(118, 22);
			this.Menu_edit.Text = "编辑";
			this.Menu_edit.Click += this.btn_edit_Click;
			this.Menu_delete.Image = Resources.delete;
			this.Menu_delete.Name = "Menu_delete";
			this.Menu_delete.Size = new Size(118, 22);
			this.Menu_delete.Text = "删除";
			this.Menu_delete.Click += this.btn_del_Click;
			this.Menu_Export.Image = Resources.Export;
			this.Menu_Export.Name = "Menu_Export";
			this.Menu_Export.Size = new Size(118, 22);
			this.Menu_Export.Text = "导出";
			this.Menu_Export.Click += this.btn_Export_Click;
			this.Menu_LogEntries.Image = Resources.Log_Entries;
			this.Menu_LogEntries.Name = "Menu_LogEntries";
			this.Menu_LogEntries.Size = new Size(118, 22);
			this.Menu_LogEntries.Text = "日志记录";
			this.Menu_LogEntries.Click += this.btn_LogEntries_Click;
			this.miniToolStrip.AutoSize = false;
			this.miniToolStrip.CanOverflow = false;
			this.miniToolStrip.Dock = DockStyle.None;
			this.miniToolStrip.GripStyle = ToolStripGripStyle.Hidden;
			this.miniToolStrip.Location = new Point(180, 3);
			this.miniToolStrip.Name = "miniToolStrip";
			this.miniToolStrip.Size = new Size(684, 25);
			this.miniToolStrip.TabIndex = 5;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[6]
			{
				this.btn_setting,
				this.btn_add,
				this.btn_edit,
				this.btn_del,
				this.btn_Export,
				this.btn_LogEntries
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 38);
			this.MenuPanelEx.TabIndex = 16;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_setting.Image = (Image)componentResourceManager.GetObject("btn_setting.Image");
			this.btn_setting.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_setting.ImageTransparentColor = Color.Magenta;
			this.btn_setting.Name = "btn_setting";
			this.btn_setting.Size = new Size(95, 35);
			this.btn_setting.Text = "树型显示";
			this.btn_setting.Click += this.btn_setting_Click;
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(69, 35);
			this.btn_add.Text = "新增";
			this.btn_add.TextAlign = ContentAlignment.MiddleRight;
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(69, 35);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_del.Image = (Image)componentResourceManager.GetObject("btn_del.Image");
			this.btn_del.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_del.ImageTransparentColor = Color.Magenta;
			this.btn_del.Name = "btn_del";
			this.btn_del.Size = new Size(69, 35);
			this.btn_del.Text = "删除";
			this.btn_del.Click += this.btn_del_Click;
			this.btn_Export.Image = (Image)componentResourceManager.GetObject("btn_Export.Image");
			this.btn_Export.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_Export.ImageTransparentColor = Color.Magenta;
			this.btn_Export.Name = "btn_Export";
			this.btn_Export.Size = new Size(69, 35);
			this.btn_Export.Text = "导出";
			this.btn_Export.Click += this.btn_Export_Click;
			this.btn_LogEntries.Image = (Image)componentResourceManager.GetObject("btn_LogEntries.Image");
			this.btn_LogEntries.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_LogEntries.ImageTransparentColor = Color.Magenta;
			this.btn_LogEntries.Name = "btn_LogEntries";
			this.btn_LogEntries.Size = new Size(95, 35);
			this.btn_LogEntries.Text = "日志记录";
			this.btn_LogEntries.Click += this.btn_LogEntries_Click;
			this.panelEx1.AntiAlias = false;
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Dock = DockStyle.Top;
			this.panelEx1.Location = new Point(0, 38);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(684, 23);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 17;
			this.panelEx1.Text = "区域";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_personnelAreaView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 388);
			this.grd_view.TabIndex = 18;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_personnelAreaView
			});
			this.grd_view.DoubleClick += this.grd_view_DoubleClick;
			this.grd_personnelAreaView.Columns.AddRange(new GridColumn[5]
			{
				this.column_check,
				this.column_areaid,
				this.column_areaname,
				this.column_parentName,
				this.column_remark
			});
			this.grd_personnelAreaView.GridControl = this.grd_view;
			this.grd_personnelAreaView.Name = "grd_personnelAreaView";
			this.grd_personnelAreaView.OptionsView.ShowGroupPanel = false;
			this.grd_personnelAreaView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_personnelAreaView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_personnelAreaView.Click += this.grd_mainView_Click;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 57;
			this.column_areaid.Caption = "区域编号";
			this.column_areaid.Name = "column_areaid";
			this.column_areaid.Visible = true;
			this.column_areaid.VisibleIndex = 1;
			this.column_areaid.Width = 151;
			this.column_areaname.Caption = "区域名称";
			this.column_areaname.Name = "column_areaname";
			this.column_areaname.Visible = true;
			this.column_areaname.VisibleIndex = 2;
			this.column_areaname.Width = 151;
			this.column_parentName.Caption = "上级区域";
			this.column_parentName.Name = "column_parentName";
			this.column_parentName.Visible = true;
			this.column_parentName.VisibleIndex = 3;
			this.column_parentName.Width = 151;
			this.column_remark.Caption = "备注";
			this.column_remark.Name = "column_remark";
			this.column_remark.Visible = true;
			this.column_remark.VisibleIndex = 4;
			this.column_remark.Width = 156;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.panelEx1);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "AreaSettingUserControl";
			base.Size = new Size(684, 449);
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_personnelAreaView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
