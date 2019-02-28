/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
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
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class ReaderControl : UserControl
	{
		private DataTable dtReaders;

		private DataTable dtIOState;

		private Dictionary<int, Machines> dicMachines;

		private Dictionary<int, AccDoor> dicDoors;

		private bool IsEditing = false;

		private IContainer components = null;

		private GridControl grd_view;

		public GridView grd_mainView;

		private GridColumn column_check;

		private GridColumn column_doorName;

		private GridColumn column_doorNo;

		private GridColumn column_dev;

		public PanelEx panelEx2;

		public ToolStrip MenuPanelEx;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_log;

		private GridColumn colReaderNo;

		private GridColumn colReaderName;

		private GridColumn colReaderState;

		private RepositoryItemLookUpEdit lueReaderState;

		public ReaderControl()
		{
			this.InitializeComponent();
			DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
			initLang.LocaleForm(this, base.Name);
			this.LoadData("");
			this.CheckPermmition();
			this.ChangeSking();
		}

		private void CheckPermmition()
		{
			bool enabled = SysInfos.IsOwerControlPermission(SysInfos.AccessLevel);
			this.btn_edit.Enabled = enabled;
		}

		private void ChangeSking()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_edit.Image = ResourceIPC.edit;
				this.btn_log.Image = ResourceIPC.Log_Entries;
			}
		}

		private void InitialIOState()
		{
			if (this.dtIOState == null)
			{
				this.dtIOState = new DataTable();
				this.dtIOState.Columns.Add("State", typeof(int));
				this.dtIOState.Columns.Add("ShowText", typeof(string));
				DataRow dataRow = this.dtIOState.NewRow();
				dataRow["State"] = 0;
				dataRow["ShowText"] = ShowMsgInfos.GetInfo("ReaderStateIn", "入");
				this.dtIOState.Rows.Add(dataRow);
				dataRow = this.dtIOState.NewRow();
				dataRow["State"] = 1;
				dataRow["ShowText"] = ShowMsgInfos.GetInfo("ReaderStateOut", "出");
				this.dtIOState.Rows.Add(dataRow);
			}
			this.lueReaderState.DataSource = this.dtIOState;
			this.lueReaderState.ValueMember = "State";
			this.lueReaderState.DisplayMember = "ShowText";
		}

		private void InitialMachines()
		{
			if (this.dicMachines == null)
			{
				this.dicMachines = new Dictionary<int, Machines>();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> modelList = machinesBll.GetModelList("");
				if (modelList != null)
				{
					foreach (Machines item in modelList)
					{
						this.dicMachines.Add(item.ID, item);
					}
				}
			}
		}

		private void InitialDoors()
		{
			if (this.dicDoors == null)
			{
				this.dicDoors = new Dictionary<int, AccDoor>();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null)
				{
					foreach (AccDoor item in modelList)
					{
						this.dicDoors.Add(item.id, item);
					}
				}
			}
		}

		private void btn_log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("Reader");
			logsInfoForm.ShowDialog();
		}

		private void LoadData(string strWhere)
		{
			try
			{
				this.InitialDoors();
				this.InitialIOState();
				this.InitialMachines();
				AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
				DataSet list = accReaderBll.GetList(strWhere);
				if (list != null && list.Tables.Count > 0)
				{
					this.dtReaders = list.Tables[0];
					this.dtReaders.Columns.Add("MachineName");
					this.dtReaders.Columns.Add("DoorNo");
					this.dtReaders.Columns.Add("DoorName");
					this.dtReaders.Columns.Add("Check");
					foreach (DataRow row in this.dtReaders.Rows)
					{
						if (int.TryParse(row["door_id"].ToString(), out int key) && this.dicDoors.ContainsKey(key))
						{
							AccDoor accDoor = this.dicDoors[key];
							row["DoorNo"] = accDoor.door_no;
							row["DoorName"] = accDoor.door_name;
							if (this.dicMachines.ContainsKey(accDoor.device_id))
							{
								row["MachineName"] = this.dicMachines[accDoor.device_id].MachineAlias;
							}
						}
					}
				}
				else if (this.dtReaders != null)
				{
					this.dtReaders.Clear();
				}
				this.grd_view.DataSource = this.dtReaders;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void grd_mainView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "Check");
		}

		private void grd_mainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "Check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_mainView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "Check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			if (SysInfos.IsOwerControlPermission(SysInfos.AccessLevel) && !this.IsEditing)
			{
				this.IsEditing = true;
				try
				{
					DataRow focusedDataRow = this.grd_mainView.GetFocusedDataRow();
					if (focusedDataRow == null)
					{
						this.IsEditing = false;
						return;
					}
					ReaderEdit readerEdit = new ReaderEdit(int.Parse(focusedDataRow["id"].ToString()), focusedDataRow["MachineName"].ToString(), focusedDataRow["DoorNo"].ToString(), focusedDataRow["DoorName"].ToString());
					readerEdit.RefreshData += this.re_RefreshData;
					readerEdit.ShowDialog();
				}
				catch (Exception ex)
				{
					SysDialogs.ShowInfoMessage(ex.Message);
				}
				this.IsEditing = false;
			}
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			if (SysInfos.IsOwerControlPermission(SysInfos.AccessLevel) && !this.IsEditing)
			{
				this.IsEditing = true;
				try
				{
					DataRow[] array = this.dtReaders.Select("Check=true");
					if (array == null || array.Length == 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
						this.IsEditing = false;
						return;
					}
					if (array.Length > 1)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录"));
						this.IsEditing = false;
						return;
					}
					ReaderEdit readerEdit = new ReaderEdit(int.Parse(array[0]["id"].ToString()), array[0]["MachineName"].ToString(), array[0]["DoorNo"].ToString(), array[0]["DoorName"].ToString());
					readerEdit.RefreshData += this.re_RefreshData;
					readerEdit.ShowDialog();
				}
				catch (Exception ex)
				{
					SysDialogs.ShowInfoMessage(ex.Message);
				}
				this.IsEditing = false;
			}
		}

		private void re_RefreshData(object sender, EventArgs e)
		{
			this.LoadData("");
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReaderControl));
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_doorName = new GridColumn();
			this.column_doorNo = new GridColumn();
			this.colReaderNo = new GridColumn();
			this.colReaderName = new GridColumn();
			this.colReaderState = new GridColumn();
			this.lueReaderState = new RepositoryItemLookUpEdit();
			this.panelEx2 = new PanelEx();
			this.MenuPanelEx = new ToolStrip();
			this.btn_edit = new ToolStripButton();
			this.btn_log = new ToolStripButton();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			((ISupportInitialize)this.lueReaderState).BeginInit();
			this.MenuPanelEx.SuspendLayout();
			base.SuspendLayout();
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.RepositoryItems.AddRange(new RepositoryItem[1]
			{
				this.lueReaderState
			});
			this.grd_view.Size = new Size(686, 389);
			this.grd_view.TabIndex = 12;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[7]
			{
				this.column_check,
				this.column_dev,
				this.column_doorName,
				this.column_doorNo,
				this.colReaderNo,
				this.colReaderName,
				this.colReaderState
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
			this.column_check.FieldName = "Check";
			this.column_check.Name = "column_check";
			this.column_check.OptionsColumn.ShowCaption = false;
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 35;
			this.column_dev.Caption = "设备名称";
			this.column_dev.FieldName = "MachineName";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 80;
			this.column_doorName.Caption = "门名称";
			this.column_doorName.FieldName = "DoorName";
			this.column_doorName.Name = "column_doorName";
			this.column_doorName.Visible = true;
			this.column_doorName.VisibleIndex = 2;
			this.column_doorName.Width = 80;
			this.column_doorNo.Caption = "门编号";
			this.column_doorNo.FieldName = "DoorNo";
			this.column_doorNo.Name = "column_doorNo";
			this.column_doorNo.Visible = true;
			this.column_doorNo.VisibleIndex = 3;
			this.column_doorNo.Width = 80;
			this.colReaderNo.Caption = "读头编号";
			this.colReaderNo.FieldName = "reader_no";
			this.colReaderNo.Name = "colReaderNo";
			this.colReaderNo.Visible = true;
			this.colReaderNo.VisibleIndex = 4;
			this.colReaderName.Caption = "读头名称";
			this.colReaderName.FieldName = "reader_name";
			this.colReaderName.Name = "colReaderName";
			this.colReaderName.Visible = true;
			this.colReaderName.VisibleIndex = 5;
			this.colReaderState.Caption = "出入状态";
			this.colReaderState.ColumnEdit = this.lueReaderState;
			this.colReaderState.FieldName = "reader_state";
			this.colReaderState.Name = "colReaderState";
			this.colReaderState.Visible = true;
			this.colReaderState.VisibleIndex = 6;
			this.lueReaderState.AutoHeight = false;
			this.lueReaderState.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueReaderState.Columns.AddRange(new LookUpColumnInfo[1]
			{
				new LookUpColumnInfo("ShowText", "出入状态")
			});
			this.lueReaderState.Name = "lueReaderState";
			this.lueReaderState.NullText = "";
			this.panelEx2.CanvasColor = SystemColors.Control;
			this.panelEx2.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx2.Dock = DockStyle.Top;
			this.panelEx2.Location = new Point(0, 38);
			this.panelEx2.Name = "panelEx2";
			this.panelEx2.Size = new Size(686, 23);
			this.panelEx2.Style.Alignment = StringAlignment.Center;
			this.panelEx2.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx2.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx2.Style.Border = eBorderType.SingleLine;
			this.panelEx2.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx2.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx2.Style.GradientAngle = 90;
			this.panelEx2.TabIndex = 11;
			this.panelEx2.Text = "读头设置";
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[2]
			{
				this.btn_edit,
				this.btn_log
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(686, 38);
			this.MenuPanelEx.TabIndex = 10;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(65, 35);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_log.Image = (Image)componentResourceManager.GetObject("btn_log.Image");
			this.btn_log.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_log.ImageTransparentColor = Color.Magenta;
			this.btn_log.Name = "btn_log";
			this.btn_log.Size = new Size(89, 35);
			this.btn_log.Text = "日志记录";
			this.btn_log.Click += this.btn_log_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.panelEx2);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "ReaderControl";
			base.Size = new Size(686, 450);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			((ISupportInitialize)this.lueReaderState).EndInit();
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
