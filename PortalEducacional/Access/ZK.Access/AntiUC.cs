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
	public class AntiUC : UserControl
	{
		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private DataTable m_datatable = new DataTable();

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private AccAntibackBll bll = new AccAntibackBll(MainForm._ia);

		private bool isDouble = false;

		private Dictionary<int, Machines> dicId_Machine;

		private Dictionary<int, List<AccDoor>> dicMachineId_lstDoor;

		private IContainer components = null;

		private ContextMenuStrip gv_contextMenu;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		private SaveFileDialog saveFileDialog1;

		private GridView grd_mainView;

		private GridColumn column_check;

		private GridColumn column_dev;

		private GridColumn column_anti;

		private GridControl grd_view;

		public PanelEx panelEx2;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		public ToolStrip MenuPanelEx;

		public AntiUC()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitAntiType();
				this.InitDataTableSet();
				this.InitMachines();
				initLang.LocaleForm(this, base.Name);
				this.LoadData();
				this.LoadMachineDoor();
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

		private void InitAntiType()
		{
			this.m_typeDic = initLang.GetAntiComboxInfo();
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("devicename");
			this.m_datatable.Columns.Add("InterlockType");
			this.m_datatable.Columns.Add("check");
			this.column_dev.FieldName = "devicename";
			this.column_anti.FieldName = "InterlockType";
			this.column_check.FieldName = "check";
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> list = null;
				list = machinesBll.GetModelList("");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.mlist.ContainsKey(list[i].ID))
						{
							this.mlist.Add(list[i].ID, list[i]);
						}
					}
					this.btn_add.Enabled = true;
					this.menu_add.Enabled = true;
				}
				else
				{
					this.btn_add.Enabled = false;
					this.menu_add.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadData()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AccAntiback> modelList = this.bll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.mlist.ContainsKey(modelList[i].device_id))
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = modelList[i].id;
							dataRow[1] = this.mlist[modelList[i].device_id].MachineAlias;
							string value = string.Empty;
							int num = this.mlist[modelList[i].device_id].acpanel_type;
							if (this.mlist[modelList[i].device_id].acpanel_type == 2 && this.mlist[modelList[i].device_id].reader_count == 4)
							{
								num = 3;
							}
							if (this.mlist[modelList[i].device_id].acpanel_type == 4 && this.mlist[modelList[i].device_id].device_type == 10)
							{
								num = 5;
							}
							if (this.mlist[modelList[i].device_id].DevSDKType == SDKType.StandaloneSDK)
							{
								num = 1000;
							}
							if (this.m_typeDic != null && this.m_typeDic.ContainsKey(num.ToString()))
							{
								Dictionary<string, string> dictionary = this.m_typeDic[num.ToString()];
								Dictionary<string, string> dictionary2 = dictionary;
								int antibackType = modelList[i].AntibackType;
								if (dictionary2.ContainsKey(antibackType.ToString()))
								{
									Dictionary<string, string> dictionary3 = dictionary;
									antibackType = modelList[i].AntibackType;
									value = dictionary3[antibackType.ToString()];
								}
							}
							dataRow[2] = value;
							dataRow[3] = false;
							this.m_datatable.Rows.Add(dataRow);
						}
					}
					if (modelList.Count >= this.mlist.Count)
					{
						this.btn_add.Enabled = false;
						this.menu_add.Enabled = false;
					}
					else if (this.mlist.Count > 0)
					{
						this.btn_add.Enabled = true;
						this.menu_add.Enabled = true;
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_delete.Enabled = true;
					this.btn_edit.Enabled = true;
					this.menu_del.Enabled = true;
					this.menu_edit.Enabled = true;
				}
				else
				{
					this.btn_delete.Enabled = false;
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
				AntiEdit antiEdit = new AntiEdit(0);
				antiEdit.RefreshDataEvent += this.frmAnti_RefreshDataEvent;
				antiEdit.Text = this.btn_add.Text;
				antiEdit.ShowDialog();
				antiEdit.RefreshDataEvent -= this.frmAnti_RefreshDataEvent;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void frmAnti_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadData();
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
							AntiEdit antiEdit = new AntiEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							antiEdit.RefreshDataEvent += this.frmAnti_RefreshDataEvent;
							antiEdit.Text = this.btn_edit.Text;
							antiEdit.ShowDialog();
							antiEdit.RefreshDataEvent -= this.frmAnti_RefreshDataEvent;
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
							bool flag = false;
							for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
							{
								AccAntiback model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									CommandServer.SetAntiPassback(0, model.device_id);
									if (this.dicId_Machine.ContainsKey(model.device_id) && this.dicMachineId_lstDoor.ContainsKey(model.device_id))
									{
										Machines machines = this.dicId_Machine[model.device_id];
										List<AccDoor> list = this.dicMachineId_lstDoor[model.device_id];
										if (machines.DevSDKType == SDKType.StandaloneSDK && list.Count > 0)
										{
											CommandServer.SetDoorParam(list[0]);
										}
									}
									this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									flag = true;
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
						bool flag = false;
						for (int i = 0; i < this.m_datatable.Rows.Count && this.m_datatable.Rows[i][0] != null; i++)
						{
							AccAntiback model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							if (model != null)
							{
								CommandServer.SetAntiPassback(0, model.device_id);
								this.bll.Delete(int.Parse(this.m_datatable.Rows[i][0].ToString()));
								flag = true;
							}
						}
						if (flag)
						{
							FrmShowUpdata.Instance.ShowEx();
							this.m_datatable.Rows.Clear();
						}
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

		private void LoadMachineDoor()
		{
			try
			{
				this.dicId_Machine = new Dictionary<int, Machines>();
				this.dicMachineId_lstDoor = new Dictionary<int, List<AccDoor>>();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> modelList = machinesBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dicId_Machine.ContainsKey(modelList[i].ID))
						{
							this.dicId_Machine.Add(modelList[i].ID, modelList[i]);
						}
					}
				}
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList2 = accDoorBll.GetModelList("");
				if (modelList2 != null)
				{
					for (int j = 0; j < modelList2.Count; j++)
					{
						if (!this.dicMachineId_lstDoor.ContainsKey(modelList2[j].device_id))
						{
							List<AccDoor> list = new List<AccDoor>();
							list.Add(modelList2[j]);
							this.dicMachineId_lstDoor.Add(modelList2[j].device_id, list);
						}
						else
						{
							List<AccDoor> list = this.dicMachineId_lstDoor[modelList2[j].device_id];
							list.Add(modelList2[j]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AntiUC));
			this.gv_contextMenu = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_anti = new GridColumn();
			this.grd_view = new GridControl();
			this.panelEx2 = new PanelEx();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.MenuPanelEx = new ToolStrip();
			this.gv_contextMenu.SuspendLayout();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			((ISupportInitialize)this.grd_view).BeginInit();
			this.MenuPanelEx.SuspendLayout();
			base.SuspendLayout();
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
				this.column_dev,
				this.column_anti
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsSelection.MultiSelect = true;
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
			this.column_dev.Caption = "设备";
			this.column_dev.Name = "column_dev";
			this.column_dev.OptionsColumn.AllowEdit = false;
			this.column_dev.OptionsColumn.ReadOnly = true;
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 301;
			this.column_anti.Caption = "反潜设置信息";
			this.column_anti.Name = "column_anti";
			this.column_anti.OptionsColumn.AllowEdit = false;
			this.column_anti.OptionsColumn.ReadOnly = true;
			this.column_anti.Visible = true;
			this.column_anti.VisibleIndex = 2;
			this.column_anti.Width = 306;
			this.grd_view.ContextMenuStrip = this.gv_contextMenu;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 388);
			this.grd_view.TabIndex = 7;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.panelEx2.CanvasColor = SystemColors.Control;
			this.panelEx2.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx2.Dock = DockStyle.Top;
			this.panelEx2.Location = new Point(0, 38);
			this.panelEx2.Name = "panelEx2";
			this.panelEx2.Size = new Size(684, 23);
			this.panelEx2.Style.Alignment = StringAlignment.Center;
			this.panelEx2.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx2.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx2.Style.Border = eBorderType.SingleLine;
			this.panelEx2.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx2.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx2.Style.GradientAngle = 90;
			this.panelEx2.TabIndex = 6;
			this.panelEx2.Text = "反潜";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(69, 35);
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(69, 35);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.Image = (Image)componentResourceManager.GetObject("btn_delete.Image");
			this.btn_delete.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delete.ImageTransparentColor = Color.Magenta;
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(69, 35);
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[3]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_delete
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 38);
			this.MenuPanelEx.TabIndex = 5;
			this.MenuPanelEx.Text = "toolStrip1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			this.ContextMenuStrip = this.gv_contextMenu;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.panelEx2);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "AntiUC";
			base.Size = new Size(684, 449);
			this.gv_contextMenu.ResumeLayout(false);
			((ISupportInitialize)this.grd_mainView).EndInit();
			((ISupportInitialize)this.grd_view).EndInit();
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
