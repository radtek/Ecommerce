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
	public class InterlockUC : UserControl
	{
		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private DataTable m_datatable = new DataTable();

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private int lockcount = 0;

		private AccInterlockBll bll = new AccInterlockBll(MainForm._ia);

		private bool isDouble = false;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		private SaveFileDialog saveFileDialog1;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		public PanelEx pnl_Interlock;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_dev;

		private GridColumn column_interLock;

		public ToolStrip MenuPanelEx;

		private GridColumn column_check;

		public InterlockUC()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitInterlockType();
				this.InitDataTableSet();
				this.InitMachines();
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

		private void InitInterlockType()
		{
			this.m_typeDic = initLang.GetInterlockComboxInfo();
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

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				this.lockcount = 0;
				List<Machines> list = null;
				list = this.mbll.GetModelList("DevSDKType <> 2");
				if (list != null && list.Count > 0)
				{
					bool flag = false;
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.mlist.ContainsKey(list[i].ID))
						{
							this.mlist.Add(list[i].ID, list[i]);
						}
						if (list[i].door_count > 1)
						{
							flag = true;
							this.lockcount++;
						}
					}
					if (flag)
					{
						this.btn_add.Enabled = true;
						this.menu_add.Enabled = true;
						this.btn_add.Tag = "1";
					}
					else
					{
						this.btn_add.Enabled = false;
						this.menu_add.Enabled = false;
						this.btn_add.Tag = "0";
					}
				}
				else
				{
					this.btn_add.Enabled = false;
					this.menu_add.Enabled = false;
					this.btn_add.Tag = "0";
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
				List<AccInterlock> modelList = this.bll.GetModelList("");
				this.m_datatable.Rows.Clear();
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
							int num;
							int num2;
							if (this.m_typeDic != null)
							{
								Dictionary<string, Dictionary<string, string>> typeDic = this.m_typeDic;
								num = this.mlist[modelList[i].device_id].acpanel_type;
								num2 = (typeDic.ContainsKey(num.ToString()) ? 1 : 0);
							}
							else
							{
								num2 = 0;
							}
							if (num2 != 0)
							{
								Dictionary<string, Dictionary<string, string>> typeDic2 = this.m_typeDic;
								num = this.mlist[modelList[i].device_id].acpanel_type;
								Dictionary<string, string> dictionary = typeDic2[num.ToString()];
								Dictionary<string, string> dictionary2 = dictionary;
								num = modelList[i].InterlockType;
								if (dictionary2.ContainsKey(num.ToString()))
								{
									Dictionary<string, string> dictionary3 = dictionary;
									num = modelList[i].InterlockType;
									value = dictionary3[num.ToString()];
								}
							}
							dataRow[2] = value;
							dataRow[3] = false;
							this.m_datatable.Rows.Add(dataRow);
						}
					}
					if (this.m_datatable.Rows.Count >= this.lockcount)
					{
						this.btn_add.Enabled = false;
						this.menu_add.Enabled = false;
					}
					else if (this.lockcount > 0 && this.btn_add.Tag != null && this.btn_add.Tag.ToString() == "1")
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
				InterlockEdit interlockEdit = new InterlockEdit(0);
				interlockEdit.RefreshDataEvent += this.frmInterlock_RefreshDataEvent;
				interlockEdit.ShowDialog();
				interlockEdit.RefreshDataEvent -= this.frmInterlock_RefreshDataEvent;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void frmInterlock_RefreshDataEvent(object sender, EventArgs e)
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
							InterlockEdit interlockEdit = new InterlockEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							interlockEdit.RefreshDataEvent += this.frmInterlock_RefreshDataEvent;
							interlockEdit.ShowDialog();
							interlockEdit.RefreshDataEvent -= this.frmInterlock_RefreshDataEvent;
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
								AccInterlock model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									CommandServer.SetInterLock(0, model.device_id);
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
							AccInterlock model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							if (model != null)
							{
								CommandServer.SetInterLock(0, model.device_id);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(InterlockUC));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.pnl_Interlock = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_interLock = new GridColumn();
			this.contextMenuStrip1.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
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
			this.contextMenuStrip1.Size = new Size(95, 70);
			this.menu_add.Image = Resources.add;
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(152, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.btn_add_Click;
			this.menu_edit.Image = Resources.edit;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(152, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_del.Image = Resources.delete;
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(152, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
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
			this.MenuPanelEx.TabIndex = 4;
			this.MenuPanelEx.Text = "toolStrip1";
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
			this.pnl_Interlock.CanvasColor = SystemColors.Control;
			this.pnl_Interlock.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_Interlock.Dock = DockStyle.Top;
			this.pnl_Interlock.Location = new Point(0, 38);
			this.pnl_Interlock.Name = "pnl_Interlock";
			this.pnl_Interlock.Size = new Size(684, 23);
			this.pnl_Interlock.Style.Alignment = StringAlignment.Center;
			this.pnl_Interlock.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_Interlock.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_Interlock.Style.Border = eBorderType.SingleLine;
			this.pnl_Interlock.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_Interlock.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_Interlock.Style.GradientAngle = 90;
			this.pnl_Interlock.TabIndex = 5;
			this.pnl_Interlock.Text = "互锁";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 310);
			this.grd_view.TabIndex = 6;
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
			this.column_dev.Caption = "设备";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 302;
			this.column_interLock.Caption = "互锁设置信息";
			this.column_interLock.Name = "column_interLock";
			this.column_interLock.Visible = true;
			this.column_interLock.VisibleIndex = 2;
			this.column_interLock.Width = 305;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_Interlock);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "InterlockUC";
			base.Size = new Size(684, 371);
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
