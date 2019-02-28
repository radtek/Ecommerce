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

namespace ZK.Access.door
{
	public class DoorUC : UserControl
	{
		private Dictionary<int, string> verifyTypeList = new Dictionary<int, string>();

		private Dictionary<int, string> timeZoneList = new Dictionary<int, string>();

		private DataTable m_datatable = new DataTable();

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private AccDoorBll bll = new AccDoorBll(MainForm._ia);

		private bool isDouble = false;

		private string strCondtion;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem menu_edit;

		private SaveFileDialog saveFileDialog1;

		private ToolStripButton btn_edit;

		public PanelEx panelEx2;

		private GridControl grd_view;

		private GridColumn column_doorNo;

		private GridColumn column_doorName;

		private GridColumn column_dev;

		public ToolStrip MenuPanelEx;

		private GridColumn column_lockDelay;

		private GridColumn column_sensorDelay;

		private GridColumn column_openDoorType;

		private GridColumn column_LockActiveID;

		private GridColumn column_longOpenID;

		private GridColumn column_doorSensorStatus;

		private GridColumn column_check;

		public GridView grd_mainView;

		private ToolStripButton btn_search;

		private ToolStripButton btn_log;

		private GridColumn column_readerIOState;

		public DoorUC()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.LoadTimeSeg();
				this.LoadVerifyType();
				this.InitMachines();
				this.LoadData("");
				initLang.LocaleForm(this, base.Name);
				this.ChangeSking();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataError", "数据加载失败!"));
			}
			this.CheckPermission();
		}

		private void ChangeSking()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_edit.Image = ResourceIPC.edit;
				this.btn_log.Image = ResourceIPC.Log_Entries;
				this.btn_search.Image = ResourceIPC.search;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel))
			{
				this.btn_edit.Enabled = false;
				this.menu_edit.Enabled = false;
				this.btn_search.Enabled = false;
				this.grd_mainView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void LoadVerifyType()
		{
			this.verifyTypeList.Clear();
			this.verifyTypeList = PullSDKVerifyTypeInfos.GetDic();
		}

		private void LoadTimeSeg()
		{
			try
			{
				this.timeZoneList.Clear();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.timeZoneList.Add(modelList[i].id, modelList[i].timeseg_name);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("doorNo");
			this.m_datatable.Columns.Add("doorName");
			this.m_datatable.Columns.Add("devicename");
			this.m_datatable.Columns.Add("lockActiveID");
			this.m_datatable.Columns.Add("longOpenID");
			this.m_datatable.Columns.Add("lockDelay");
			this.m_datatable.Columns.Add("sensorDelay");
			this.m_datatable.Columns.Add("openDoorType");
			this.m_datatable.Columns.Add("doorSensorStatus");
			this.m_datatable.Columns.Add("check");
			this.m_datatable.Columns.Add("ReaderIOState");
			this.m_datatable.Columns.Add("DeviceId");
			this.column_doorNo.FieldName = "doorNo";
			this.column_doorName.FieldName = "doorName";
			this.column_dev.FieldName = "devicename";
			this.column_LockActiveID.FieldName = "lockActiveID";
			this.column_longOpenID.FieldName = "longOpenID";
			this.column_lockDelay.FieldName = "lockDelay";
			this.column_sensorDelay.FieldName = "sensorDelay";
			this.column_openDoorType.FieldName = "openDoorType";
			this.column_doorSensorStatus.FieldName = "doorSensorStatus";
			this.column_check.FieldName = "check";
			this.column_readerIOState.FieldName = "ReaderIOState";
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				List<Machines> list = null;
				list = this.mbll.GetModelList("");
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

		private void LoadData(string dataType)
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AccDoor> list = null;
				list = ((!(dataType == "")) ? this.bll.GetModelList(dataType) : this.bll.GetModelList(""));
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (this.mlist.ContainsKey(list[i].device_id))
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = list[i].id;
							dataRow[1] = list[i].door_no;
							dataRow[2] = list[i].door_name;
							dataRow[3] = this.mlist[list[i].device_id].MachineAlias;
							if (this.timeZoneList.ContainsKey(list[i].lock_active_id))
							{
								dataRow[4] = this.timeZoneList[list[i].lock_active_id];
							}
							else
							{
								dataRow[4] = "";
							}
							if (this.mlist[list[i].device_id].DevSDKType == SDKType.StandaloneSDK)
							{
								if (list[i].long_open_id > 0)
								{
									dataRow[5] = list[i].long_open_id;
								}
								else
								{
									dataRow[5] = "";
								}
							}
							else if (this.timeZoneList.ContainsKey(list[i].long_open_id))
							{
								dataRow[5] = this.timeZoneList[list[i].long_open_id];
							}
							else
							{
								dataRow[5] = "";
							}
							dataRow[6] = list[i].lock_delay;
							if (this.mlist[list[i].device_id].platform != null && (this.mlist[list[i].device_id].platform.ToUpper().Contains("ZEM560") || this.mlist[list[i].device_id].platform.ToUpper().Contains("ZEM500")))
							{
								dataRow[6] = Math.Round((double)(list[i].lock_delay * 4) / 100.0, 1);
							}
							dataRow[7] = list[i].sensor_delay;
							if (this.verifyTypeList.ContainsKey(list[i].opendoor_type))
							{
								dataRow[8] = this.verifyTypeList[list[i].opendoor_type];
							}
							else
							{
								dataRow[8] = "";
							}
							if (list[i].door_sensor_status == 0)
							{
								dataRow[9] = ShowMsgInfos.GetInfo("SDoorSensorStatusNone", "无");
							}
							else if (list[i].door_sensor_status == 1)
							{
								dataRow[9] = ShowMsgInfos.GetInfo("SDoorSensorStatusNormalOpen", "常开");
							}
							else if (list[i].door_sensor_status == 2)
							{
								dataRow[9] = ShowMsgInfos.GetInfo("SDoorSensorStatusNormalClose", "常闭");
							}
							else
							{
								dataRow[9] = "";
							}
							dataRow[10] = false;
							if (list[i].readerIOState == 1)
							{
								dataRow[11] = ShowMsgInfos.GetInfo("SDoorReaderIOStateIn", "入");
							}
							else
							{
								dataRow[11] = ShowMsgInfos.GetInfo("SDoorReaderIOStateOut", "出");
							}
							dataRow[12] = list[i].device_id;
							this.m_datatable.Rows.Add(dataRow);
						}
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_edit.Enabled = true;
					this.menu_edit.Enabled = true;
					this.btn_search.Enabled = true;
				}
				else
				{
					this.btn_edit.Enabled = false;
					this.menu_edit.Enabled = false;
					this.btn_search.Enabled = false;
				}
				this.CheckPermission();
				this.column_check.ImageIndex = 1;
				this.grd_mainView.BestFitColumns();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void frmDoor_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadData("");
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
							doorSettingForm doorSettingForm = new doorSettingForm(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							doorSettingForm.refreshDataEvent += this.frmDoor_RefreshDataEvent;
							doorSettingForm.ShowDialog();
							doorSettingForm.refreshDataEvent -= this.frmDoor_RefreshDataEvent;
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

		private void btn_search_Click(object sender, EventArgs e)
		{
			try
			{
				DoorSearchForm doorSearchForm = new DoorSearchForm();
				doorSearchForm.Text = this.btn_search.Text;
				doorSearchForm.ShowDialog();
				if (doorSearchForm.DialogResult == DialogResult.OK)
				{
					this.strCondtion = doorSearchForm.ConditionStr;
					this.LoadData(this.strCondtion);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("door");
			logsInfoForm.ShowDialog();
		}

		private void grd_mainView_RowStyle(object sender, RowStyleEventArgs e)
		{
			if (this.mlist != null && e != null)
			{
				GridView gridView = sender as GridView;
				if (gridView != null && e.RowHandle >= 0)
				{
					DataRow dataRow = gridView.GetDataRow(e.RowHandle);
					int key;
					if (dataRow != null && int.TryParse(dataRow[12].ToString(), out key) && this.mlist.ContainsKey(key))
					{
						Machines machines = this.mlist[key];
						if (machines.DevSDKType == SDKType.StandaloneSDK)
						{
							e.Appearance.BackColor = Color.Yellow;
						}
					}
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DoorUC));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_edit = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.MenuPanelEx = new ToolStrip();
			this.btn_edit = new ToolStripButton();
			this.btn_search = new ToolStripButton();
			this.btn_log = new ToolStripButton();
			this.panelEx2 = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_doorName = new GridColumn();
			this.column_doorNo = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_LockActiveID = new GridColumn();
			this.column_longOpenID = new GridColumn();
			this.column_lockDelay = new GridColumn();
			this.column_sensorDelay = new GridColumn();
			this.column_openDoorType = new GridColumn();
			this.column_doorSensorStatus = new GridColumn();
			this.column_readerIOState = new GridColumn();
			this.contextMenuStrip1.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[1]
			{
				this.menu_edit
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(99, 26);
			this.menu_edit.Image = Resources.edit;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(98, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[3]
			{
				this.btn_edit,
				this.btn_search,
				this.btn_log
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(801, 41);
			this.MenuPanelEx.TabIndex = 7;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(73, 38);
			this.btn_edit.Text = "Editar";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_search.Image = (Image)componentResourceManager.GetObject("btn_search.Image");
			this.btn_search.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_search.ImageTransparentColor = Color.Magenta;
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(67, 38);
			this.btn_search.Text = "查找";
			this.btn_search.Click += this.btn_search_Click;
			this.btn_log.Image = (Image)componentResourceManager.GetObject("btn_log.Image");
			this.btn_log.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_log.ImageTransparentColor = Color.Magenta;
			this.btn_log.Name = "btn_log";
			this.btn_log.Size = new Size(91, 38);
			this.btn_log.Text = "日志记录";
			this.btn_log.Click += this.btn_log_Click;
			this.panelEx2.CanvasColor = SystemColors.Control;
			this.panelEx2.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx2.Dock = DockStyle.Top;
			this.panelEx2.Location = new Point(0, 41);
			this.panelEx2.Name = "panelEx2";
			this.panelEx2.Size = new Size(801, 25);
			this.panelEx2.Style.Alignment = StringAlignment.Center;
			this.panelEx2.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx2.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx2.Style.Border = eBorderType.SingleLine;
			this.panelEx2.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx2.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx2.Style.GradientAngle = 90;
			this.panelEx2.TabIndex = 8;
			this.panelEx2.Text = "门设置";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 66);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(801, 433);
			this.grd_view.TabIndex = 9;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[11]
			{
				this.column_check,
				this.column_doorName,
				this.column_doorNo,
				this.column_dev,
				this.column_LockActiveID,
				this.column_longOpenID,
				this.column_lockDelay,
				this.column_sensorDelay,
				this.column_openDoorType,
				this.column_doorSensorStatus,
				this.column_readerIOState
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.RowStyle += this.grd_mainView_RowStyle;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 35;
			this.column_doorName.Caption = "门名称";
			this.column_doorName.Name = "column_doorName";
			this.column_doorName.Visible = true;
			this.column_doorName.VisibleIndex = 1;
			this.column_doorName.Width = 80;
			this.column_doorNo.Caption = "门编号";
			this.column_doorNo.Name = "column_doorNo";
			this.column_doorNo.Visible = true;
			this.column_doorNo.VisibleIndex = 2;
			this.column_doorNo.Width = 80;
			this.column_dev.Caption = "设备名称";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 3;
			this.column_dev.Width = 80;
			this.column_LockActiveID.Caption = "门有效时间段";
			this.column_LockActiveID.Name = "column_LockActiveID";
			this.column_LockActiveID.Visible = true;
			this.column_LockActiveID.VisibleIndex = 4;
			this.column_LockActiveID.Width = 72;
			this.column_longOpenID.Caption = "门常开时间段";
			this.column_longOpenID.Name = "column_longOpenID";
			this.column_longOpenID.Visible = true;
			this.column_longOpenID.VisibleIndex = 5;
			this.column_longOpenID.Width = 83;
			this.column_lockDelay.Caption = "锁驱动时长";
			this.column_lockDelay.Name = "column_lockDelay";
			this.column_lockDelay.Visible = true;
			this.column_lockDelay.VisibleIndex = 6;
			this.column_lockDelay.Width = 72;
			this.column_sensorDelay.Caption = "门磁延时";
			this.column_sensorDelay.Name = "column_sensorDelay";
			this.column_sensorDelay.Visible = true;
			this.column_sensorDelay.VisibleIndex = 7;
			this.column_sensorDelay.Width = 72;
			this.column_openDoorType.Caption = "验证方式";
			this.column_openDoorType.Name = "column_openDoorType";
			this.column_openDoorType.Visible = true;
			this.column_openDoorType.VisibleIndex = 8;
			this.column_openDoorType.Width = 72;
			this.column_doorSensorStatus.Caption = "门磁类型";
			this.column_doorSensorStatus.Name = "column_doorSensorStatus";
			this.column_doorSensorStatus.Visible = true;
			this.column_doorSensorStatus.VisibleIndex = 9;
			this.column_doorSensorStatus.Width = 60;
			this.column_readerIOState.Caption = "出入状态";
			this.column_readerIOState.Name = "column_readerIOState";
			this.column_readerIOState.Visible = true;
			this.column_readerIOState.VisibleIndex = 10;
			this.column_readerIOState.Width = 35;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.panelEx2);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "DoorUC";
			base.Size = new Size(801, 499);
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
