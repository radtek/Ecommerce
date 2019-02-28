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
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class LevelUserControl : UserControl
	{
		private DataTable m_datatable = new DataTable();

		private Dictionary<int, List<int>> glist = new Dictionary<int, List<int>>();

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private Dictionary<int, AccTimeseg> timelist = new Dictionary<int, AccTimeseg>();

		private WaitForm m_wait = WaitForm.Instance;

		private bool isDouble = false;

		private ImagesForm imagesForm = new ImagesForm();

		private IContainer components = null;

		private ToolStripButton btn_delete;

		private ToolStripButton btn_edit;

		public PanelEx pnl_userLevel;

		public ToolStrip MenuPanelEx;

		private ToolStripButton btn_add;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_levelName;

		private GridColumn column_doorTime;

		private ToolStripButton btn_userLevel;

		private GridColumn column_door;

		private GridColumn column_check;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_delete;

		private ToolStripMenuItem Menu_level;

		public LevelUserControl()
		{
			this.imagesForm.TopMost = true;
			this.imagesForm.Show();
			Application.DoEvents();
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.LoadDoor();
				this.LoadTimeSeg();
				this.DataBind();
				initLang.LocaleForm(this, base.Name);
				this.ChangeSking();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
			this.CheckPermission();
			this.imagesForm.Hide();
		}

		private void ChangeSking()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_add.Image = ResourceIPC.add;
				this.btn_delete.Image = ResourceIPC.delete;
				this.btn_edit.Image = ResourceIPC.edit;
				this.btn_userLevel.Image = ResourceIPC.level1;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel))
			{
				this.btn_add.Enabled = false;
				this.btn_delete.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_userLevel.Enabled = false;
				this.Menu_add.Enabled = false;
				this.Menu_delete.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_level.Enabled = false;
				this.grd_mainView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("levelName");
			this.m_datatable.Columns.Add("timesegName");
			this.m_datatable.Columns.Add("doors");
			this.m_datatable.Columns.Add("check");
			this.column_levelName.FieldName = "levelName";
			this.column_doorTime.FieldName = "timesegName";
			this.column_door.FieldName = "doors";
			this.column_check.FieldName = "check";
			this.grd_view.DataSource = this.m_datatable;
		}

		private void LoadDoorGroup()
		{
			try
			{
				this.glist.Clear();
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				List<AccLevelsetDoorGroup> modelList = accLevelsetDoorGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						Application.DoEvents();
						if (this.glist.ContainsKey(modelList[i].acclevelset_id))
						{
							if (!this.glist[modelList[i].acclevelset_id].Contains(modelList[i].accdoor_id))
							{
								this.glist[modelList[i].acclevelset_id].Add(modelList[i].accdoor_id);
							}
						}
						else
						{
							List<int> list = new List<int>();
							list.Add(modelList[i].accdoor_id);
							this.glist.Add(modelList[i].acclevelset_id, list);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadDoor()
		{
			try
			{
				this.dlist.Clear();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dlist.ContainsKey(modelList[i].id))
						{
							this.dlist.Add(modelList[i].id, modelList[i]);
						}
						Application.DoEvents();
					}
				}
				else
				{
					this.btn_add.Enabled = false;
					this.Menu_add.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadTimeSeg()
		{
			try
			{
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.timelist.Add(modelList[i].id, modelList[i]);
						Application.DoEvents();
					}
				}
				else
				{
					this.btn_add.Enabled = false;
					this.Menu_add.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataBind()
		{
			try
			{
				this.LoadDoorGroup();
				this.m_datatable.Rows.Clear();
				AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
				List<AccLevelset> modelList = accLevelsetBll.GetModelList("");
				this.m_datatable.BeginLoadData();
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = modelList[i].id.ToString();
						dataRow[1] = modelList[i].level_name.ToString();
						if (this.timelist.ContainsKey(modelList[i].level_timeseg_id))
						{
							dataRow[2] = this.timelist[modelList[i].level_timeseg_id].timeseg_name;
						}
						string text = string.Empty;
						if (this.glist.ContainsKey(modelList[i].id))
						{
							List<int> list = this.glist[modelList[i].id];
							for (int j = 0; j < list.Count; j++)
							{
								if (this.dlist.ContainsKey(list[j]))
								{
									text = text + this.dlist[list[j]].door_name + ";";
								}
							}
						}
						dataRow[3] = text;
						dataRow[4] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.grd_view.BeginUpdate();
				this.grd_view.DataSource = this.m_datatable;
				this.grd_view.EndUpdate();
				this.m_datatable.EndLoadData();
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_delete.Enabled = true;
					this.btn_edit.Enabled = true;
					this.Menu_delete.Enabled = true;
					this.Menu_edit.Enabled = true;
					this.btn_userLevel.Enabled = true;
					this.Menu_level.Enabled = true;
				}
				else
				{
					this.btn_delete.Enabled = false;
					this.btn_edit.Enabled = false;
					this.Menu_delete.Enabled = false;
					this.Menu_edit.Enabled = false;
					this.btn_userLevel.Enabled = false;
					this.Menu_level.Enabled = false;
				}
				this.CheckPermission();
				this.column_check.ImageIndex = 1;
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
					AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
					AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
					AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							this.m_wait.ShowEx();
							bool flag = false;
							for (int i = 0; i < checkedRows.Length; i++)
							{
								this.m_wait.ShowProgress(100 * i / checkedRows.Length);
								if (checkedRows[i] < 0 || checkedRows[i] >= this.m_datatable.Rows.Count)
								{
									break;
								}
								AccLevelset model = accLevelsetBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									CommandServer.DelCmd(model, model.level_timeseg_id, true);
									accLevelsetBll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									accLevelsetDoorGroupBll.DeleteByAcclevelsetID(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									accLevelsetEmpBll.DeleteByAcclevelsetID(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									flag = true;
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SlevelName", "权限名称") + model.level_name + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								}
							}
							if (flag)
							{
								FrmShowUpdata.Instance.ShowEx();
								this.DataBind();
							}
							this.m_wait.ShowProgress(100);
							this.m_wait.HideEx(false);
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

		private void addLevel_RefreshDataEvent(object sender, EventArgs e)
		{
			this.DataBind();
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			AddLevelForm addLevelForm = new AddLevelForm(0);
			addLevelForm.refreshDataEvent += this.addLevel_RefreshDataEvent;
			addLevelForm.ShowDialog();
			addLevelForm.refreshDataEvent -= this.addLevel_RefreshDataEvent;
			addLevelForm.Dispose();
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
							AddLevelForm addLevelForm = new AddLevelForm(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							addLevelForm.refreshDataEvent += this.addLevel_RefreshDataEvent;
							addLevelForm.ShowDialog();
							addLevelForm.refreshDataEvent -= this.addLevel_RefreshDataEvent;
							addLevelForm.Dispose();
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

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(sender, e);
			this.isDouble = false;
		}

		private void btn_userLevel_Click(object sender, EventArgs e)
		{
			LevelUsers levelUsers = new LevelUsers();
			levelUsers.RefreshDataEvent += this.addLevel_RefreshDataEvent;
			levelUsers.ShowDialog();
			levelUsers.RefreshDataEvent -= this.addLevel_RefreshDataEvent;
			levelUsers.Dispose();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LevelUserControl));
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.btn_userLevel = new ToolStripButton();
			this.pnl_userLevel = new PanelEx();
			this.grd_view = new GridControl();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_delete = new ToolStripMenuItem();
			this.Menu_level = new ToolStripMenuItem();
			this.grd_mainView = new GridView();
			this.column_levelName = new GridColumn();
			this.column_doorTime = new GridColumn();
			this.column_door = new GridColumn();
			this.column_check = new GridColumn();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[4]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_delete,
				this.btn_userLevel
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 41);
			this.MenuPanelEx.TabIndex = 17;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(94, 38);
			this.btn_add.Text = "Adicionar";
			this.btn_add.Click += this.btn_add_Click;
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
			this.btn_userLevel.Image = (Image)componentResourceManager.GetObject("btn_userLevel.Image");
			this.btn_userLevel.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_userLevel.ImageTransparentColor = Color.Magenta;
			this.btn_userLevel.Name = "btn_userLevel";
			this.btn_userLevel.Size = new Size(186, 38);
			this.btn_userLevel.Text = "Perfil de Acesso do Usuário";
			this.btn_userLevel.Click += this.btn_userLevel_Click;
			this.pnl_userLevel.CanvasColor = SystemColors.Control;
			this.pnl_userLevel.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_userLevel.Dock = DockStyle.Top;
			this.pnl_userLevel.Location = new Point(0, 41);
			this.pnl_userLevel.Name = "pnl_userLevel";
			this.pnl_userLevel.Size = new Size(684, 25);
			this.pnl_userLevel.Style.Alignment = StringAlignment.Center;
			this.pnl_userLevel.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_userLevel.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_userLevel.Style.Border = eBorderType.SingleLine;
			this.pnl_userLevel.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_userLevel.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_userLevel.Style.GradientAngle = 90;
			this.pnl_userLevel.TabIndex = 18;
			this.pnl_userLevel.Text = "Perfil de Acesso";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 66);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 420);
			this.grd_view.TabIndex = 19;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[4]
			{
				this.Menu_add,
				this.Menu_edit,
				this.Menu_delete,
				this.Menu_level
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(171, 92);
			this.Menu_add.Image = Resources.add;
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(170, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.btn_add_Click;
			this.Menu_edit.Image = Resources.edit;
			this.Menu_edit.Name = "Menu_edit";
			this.Menu_edit.Size = new Size(170, 22);
			this.Menu_edit.Text = "编辑";
			this.Menu_edit.Click += this.btn_edit_Click;
			this.Menu_delete.Image = Resources.delete;
			this.Menu_delete.Name = "Menu_delete";
			this.Menu_delete.Size = new Size(170, 22);
			this.Menu_delete.Text = "删除";
			this.Menu_delete.Click += this.btn_delete_Click;
			this.Menu_level.Image = Resources.level;
			this.Menu_level.Name = "Menu_level";
			this.Menu_level.Size = new Size(170, 22);
			this.Menu_level.Text = "人员门禁权限设置";
			this.Menu_level.Click += this.btn_userLevel_Click;
			this.grd_mainView.Columns.AddRange(new GridColumn[4]
			{
				this.column_levelName,
				this.column_doorTime,
				this.column_door,
				this.column_check
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_levelName.Caption = "权限组名称";
			this.column_levelName.Name = "column_levelName";
			this.column_levelName.Visible = true;
			this.column_levelName.VisibleIndex = 1;
			this.column_levelName.Width = 218;
			this.column_doorTime.Caption = "门禁时间段";
			this.column_doorTime.Name = "column_doorTime";
			this.column_doorTime.Visible = true;
			this.column_doorTime.VisibleIndex = 2;
			this.column_doorTime.Width = 218;
			this.column_door.Caption = "门组合";
			this.column_door.Name = "column_door";
			this.column_door.Visible = true;
			this.column_door.VisibleIndex = 3;
			this.column_door.Width = 171;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 40;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_userLevel);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "LevelUserControl";
			base.Size = new Size(684, 486);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
