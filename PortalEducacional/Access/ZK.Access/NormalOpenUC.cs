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
	public class NormalOpenUC : UserControl
	{
		private DataTable m_datatable = new DataTable();

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private AccDoorBll dbll = new AccDoorBll(MainForm._ia);

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private Dictionary<int, AccTimeseg> tlist = new Dictionary<int, AccTimeseg>();

		private AccFirstOpenBll bll = new AccFirstOpenBll(MainForm._ia);

		private WaitForm m_wait = WaitForm.Instance;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private SaveFileDialog saveFileDialog1;

		private ToolStripMenuItem menu_del;

		private ToolStripButton btn_setting;

		private ToolStripButton btn_delete;

		public PanelEx pnl_normalOpen;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_dev;

		private GridColumn column_interLock;

		private GridColumn column_start;

		private GridColumn column_end;

		private GridColumn column_check;

		private ToolStripMenuItem Menu_setting;

		public ToolStrip MenupanelEx;

		public NormalOpenUC()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.InitMachines();
				this.InitDoor();
				this.InitTimeseg();
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
				this.btn_setting.Image = ResourceIPC.add;
				this.btn_delete.Image = ResourceIPC.delete;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel))
			{
				this.btn_delete.Enabled = false;
				this.btn_setting.Enabled = false;
				this.menu_del.Enabled = false;
				this.Menu_setting.Enabled = false;
				this.grd_mainView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("devicename");
			this.m_datatable.Columns.Add("InterlockType");
			this.m_datatable.Columns.Add("start");
			this.m_datatable.Columns.Add("end");
			this.m_datatable.Columns.Add("check");
			this.column_dev.FieldName = "devicename";
			this.column_interLock.FieldName = "InterlockType";
			this.column_end.FieldName = "end";
			this.column_start.FieldName = "start";
			this.column_check.FieldName = "check";
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				List<Machines> list = null;
				list = this.mbll.GetModelList("DevSDKType <> 2");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.mlist.ContainsKey(list[i].ID))
						{
							this.mlist.Add(list[i].ID, list[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitDoor()
		{
			try
			{
				this.dlist.Clear();
				List<AccDoor> modelList = this.dbll.GetModelList("device_id in (select ID from machines where DevSDKType <> 2)");
				if (modelList != null && modelList.Count > 0)
				{
					this.btn_setting.Enabled = true;
					this.Menu_setting.Enabled = true;
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dlist.ContainsKey(modelList[i].id))
						{
							this.dlist.Add(modelList[i].id, modelList[i]);
						}
					}
				}
				else
				{
					this.btn_setting.Enabled = false;
					this.Menu_setting.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitTimeseg()
		{
			try
			{
				this.tlist.Clear();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.tlist.ContainsKey(modelList[i].id))
						{
							this.tlist.Add(modelList[i].id, modelList[i]);
						}
					}
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
				List<AccFirstOpen> modelList = this.bll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.dlist.ContainsKey(modelList[i].door_id) && this.mlist.ContainsKey(this.dlist[modelList[i].door_id].device_id))
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = modelList[i].id;
							dataRow[1] = this.mlist[this.dlist[modelList[i].door_id].device_id].MachineAlias;
							dataRow[2] = this.dlist[modelList[i].door_id].door_no;
							dataRow[3] = this.dlist[modelList[i].door_id].door_name;
							if (this.tlist.ContainsKey(modelList[i].timeseg_id))
							{
								dataRow[4] = this.tlist[modelList[i].timeseg_id].timeseg_name;
							}
							else
							{
								dataRow[4] = modelList[i].timeseg_id;
							}
							dataRow[5] = false;
							this.m_datatable.Rows.Add(dataRow);
						}
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_delete.Enabled = true;
					this.menu_del.Enabled = true;
				}
				else
				{
					this.btn_delete.Enabled = false;
					this.menu_del.Enabled = false;
				}
				this.CheckPermission();
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void settingBtn_Click(object sender, EventArgs e)
		{
			NormalOpenForm normalOpenForm = new NormalOpenForm();
			normalOpenForm.ShowDialog();
			this.LoadData();
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
							this.m_wait.ShowEx();
							for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
							{
								AccFirstOpen model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									CommandServer.DelCmd(model);
									this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									flag = true;
									CommandServer.SetDoorParam(model.door_id);
								}
								this.m_wait.ShowProgress(90 * i / checkedRows.Length);
							}
							this.m_wait.ShowProgress(90);
							if (flag)
							{
								FrmShowUpdata.Instance.ShowEx();
								this.LoadData();
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

		private void menu_delall_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 0)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						bool flag = false;
						for (int i = 0; i < this.m_datatable.Rows.Count && this.m_datatable.Rows[i][0] != null; i++)
						{
							AccFirstOpen model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							if (model != null)
							{
								CommandServer.DelCmd(model);
								this.bll.Delete(int.Parse(this.m_datatable.Rows[i][0].ToString()));
								flag = true;
							}
							this.m_wait.ShowProgress(90 * i / this.m_datatable.Rows.Count);
						}
						this.m_wait.ShowProgress(90);
						if (flag)
						{
							FrmShowUpdata.Instance.ShowEx();
							this.m_datatable.Rows.Clear();
						}
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
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
			if (this.btn_setting.Enabled)
			{
				this.settingBtn_Click(null, null);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NormalOpenUC));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_setting = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.MenupanelEx = new ToolStrip();
			this.btn_setting = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.pnl_normalOpen = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_dev = new GridColumn();
			this.column_interLock = new GridColumn();
			this.column_start = new GridColumn();
			this.column_end = new GridColumn();
			this.column_check = new GridColumn();
			this.contextMenuStrip1.SuspendLayout();
			this.MenupanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[2]
			{
				this.Menu_setting,
				this.menu_del
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(95, 48);
			this.Menu_setting.Image = Resources.setting;
			this.Menu_setting.Name = "Menu_setting";
			this.Menu_setting.Size = new Size(94, 22);
			this.Menu_setting.Text = "设置";
			this.Menu_setting.Click += this.settingBtn_Click;
			this.menu_del.Image = Resources.delete;
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(94, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
			this.MenupanelEx.AutoSize = false;
			this.MenupanelEx.Items.AddRange(new ToolStripItem[2]
			{
				this.btn_setting,
				this.btn_delete
			});
			this.MenupanelEx.Location = new Point(0, 0);
			this.MenupanelEx.Name = "MenupanelEx";
			this.MenupanelEx.Size = new Size(684, 38);
			this.MenupanelEx.TabIndex = 8;
			this.MenupanelEx.Text = "toolStrip1";
			this.btn_setting.Image = (Image)componentResourceManager.GetObject("btn_setting.Image");
			this.btn_setting.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_setting.ImageTransparentColor = Color.Magenta;
			this.btn_setting.Name = "btn_setting";
			this.btn_setting.Size = new Size(69, 35);
			this.btn_setting.Text = "设置";
			this.btn_setting.Click += this.settingBtn_Click;
			this.btn_delete.Image = (Image)componentResourceManager.GetObject("btn_delete.Image");
			this.btn_delete.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delete.ImageTransparentColor = Color.Magenta;
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(69, 35);
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.pnl_normalOpen.CanvasColor = SystemColors.Control;
			this.pnl_normalOpen.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_normalOpen.Dock = DockStyle.Top;
			this.pnl_normalOpen.Location = new Point(0, 38);
			this.pnl_normalOpen.Name = "pnl_normalOpen";
			this.pnl_normalOpen.Size = new Size(684, 23);
			this.pnl_normalOpen.Style.Alignment = StringAlignment.Center;
			this.pnl_normalOpen.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_normalOpen.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_normalOpen.Style.Border = eBorderType.SingleLine;
			this.pnl_normalOpen.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_normalOpen.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_normalOpen.Style.GradientAngle = 90;
			this.pnl_normalOpen.TabIndex = 9;
			this.pnl_normalOpen.Text = "首卡常开";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 388);
			this.grd_view.TabIndex = 10;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[5]
			{
				this.column_dev,
				this.column_interLock,
				this.column_start,
				this.column_end,
				this.column_check
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
			this.column_dev.Caption = "设备名称";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 129;
			this.column_interLock.Caption = "门编号";
			this.column_interLock.Name = "column_interLock";
			this.column_interLock.Visible = true;
			this.column_interLock.VisibleIndex = 2;
			this.column_interLock.Width = 129;
			this.column_start.Caption = "门名称";
			this.column_start.Name = "column_start";
			this.column_start.Visible = true;
			this.column_start.VisibleIndex = 3;
			this.column_start.Width = 129;
			this.column_end.Caption = "门常开时间段";
			this.column_end.Name = "column_end";
			this.column_end.Visible = true;
			this.column_end.VisibleIndex = 4;
			this.column_end.Width = 129;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 40;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_normalOpen);
			base.Controls.Add(this.MenupanelEx);
			base.Name = "NormalOpenUC";
			base.Size = new Size(684, 449);
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenupanelEx.ResumeLayout(false);
			this.MenupanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
