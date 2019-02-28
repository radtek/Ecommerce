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
	public class HolidaysUserControl : UserControl
	{
		private DataTable m_datatable = new DataTable();

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private AccHolidaysBll bll = new AccHolidaysBll(MainForm._ia);

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

		public PanelEx pnl_holiday;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_holidayName;

		private GridColumn column_type;

		private GridColumn column_start;

		private GridColumn column_end;

		private GridColumn column_isloop;

		private GridColumn column_remark;

		public ToolStrip MenuPanelEx;

		private GridColumn column_check;

		public HolidaysUserControl()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.LoadHolidaysType();
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
			this.m_datatable.Columns.Add("start");
			this.m_datatable.Columns.Add("end");
			this.m_datatable.Columns.Add("isloop");
			this.m_datatable.Columns.Add("remark");
			this.m_datatable.Columns.Add("check");
			this.column_holidayName.FieldName = "devicename";
			this.column_type.FieldName = "InterlockType";
			this.column_end.FieldName = "end";
			this.column_isloop.FieldName = "isloop";
			this.column_remark.FieldName = "remark";
			this.column_start.FieldName = "start";
			this.column_check.FieldName = "check";
		}

		private void LoadHolidaysType()
		{
			this.m_typeDic = initLang.GetComboxInfo("holidaysType");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("1", "假日类型1");
				dictionary.Add("2", "假日类型2");
				dictionary.Add("3", "假日类型3");
				this.m_typeDic.Add("0", dictionary);
				initLang.SetComboxInfo("holidaysType", this.m_typeDic);
				initLang.Save();
			}
		}

		private void LoadData()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AccHolidays> modelList = this.bll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = modelList[i].id;
						dataRow[1] = modelList[i].holiday_name;
						if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
						{
							Dictionary<string, string> dictionary = this.m_typeDic["0"];
							Dictionary<string, string> dictionary2 = dictionary;
							int holiday_type = modelList[i].holiday_type;
							if (dictionary2.ContainsKey(holiday_type.ToString()))
							{
								DataRow dataRow2 = dataRow;
								Dictionary<string, string> dictionary3 = dictionary;
								holiday_type = modelList[i].holiday_type;
								dataRow2[2] = dictionary3[holiday_type.ToString()];
							}
						}
						DataRow dataRow3 = dataRow;
						DateTime value = modelList[i].start_date.Value;
						dataRow3[3] = value.ToShortDateString();
						DataRow dataRow4 = dataRow;
						value = modelList[i].end_date.Value;
						dataRow4[4] = value.ToShortDateString();
						if (modelList[i].loop_by_year == 1)
						{
							dataRow[5] = ShowMsgInfos.GetInfo("SHolidayLoop", "是");
						}
						else
						{
							dataRow[5] = ShowMsgInfos.GetInfo("SHolidayNotLoop", "否");
						}
						dataRow[6] = modelList[i].memo;
						dataRow[7] = false;
						this.m_datatable.Rows.Add(dataRow);
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
			HolidaysEdit holidaysEdit = new HolidaysEdit(0);
			holidaysEdit.RefreshDataEvent += this.frmholiday_RefreshDataEvent;
			holidaysEdit.ShowDialog();
			holidaysEdit.RefreshDataEvent -= this.frmholiday_RefreshDataEvent;
		}

		private void frmholiday_RefreshDataEvent(object sender, EventArgs e)
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
							HolidaysEdit holidaysEdit = new HolidaysEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							holidaysEdit.RefreshDataEvent += this.frmholiday_RefreshDataEvent;
							holidaysEdit.ShowDialog();
							holidaysEdit.RefreshDataEvent -= this.frmholiday_RefreshDataEvent;
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
								AccHolidays model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									CommandServer.DelCmd(model);
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
							AccHolidays model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							if (model != null)
							{
								CommandServer.DelCmd(model);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(HolidaysUserControl));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.pnl_holiday = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_holidayName = new GridColumn();
			this.column_type = new GridColumn();
			this.column_start = new GridColumn();
			this.column_end = new GridColumn();
			this.column_isloop = new GridColumn();
			this.column_remark = new GridColumn();
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
			this.MenuPanelEx.TabIndex = 6;
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
			this.pnl_holiday.CanvasColor = SystemColors.Control;
			this.pnl_holiday.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_holiday.Dock = DockStyle.Top;
			this.pnl_holiday.Location = new Point(0, 38);
			this.pnl_holiday.Name = "pnl_holiday";
			this.pnl_holiday.Size = new Size(684, 23);
			this.pnl_holiday.Style.Alignment = StringAlignment.Center;
			this.pnl_holiday.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_holiday.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_holiday.Style.Border = eBorderType.SingleLine;
			this.pnl_holiday.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_holiday.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_holiday.Style.GradientAngle = 90;
			this.pnl_holiday.TabIndex = 7;
			this.pnl_holiday.Text = "门禁节假日";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 454);
			this.grd_view.TabIndex = 8;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[7]
			{
				this.column_check,
				this.column_holidayName,
				this.column_type,
				this.column_start,
				this.column_end,
				this.column_isloop,
				this.column_remark
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
			this.column_holidayName.Caption = "节假日名称";
			this.column_holidayName.Name = "column_holidayName";
			this.column_holidayName.Visible = true;
			this.column_holidayName.VisibleIndex = 1;
			this.column_holidayName.Width = 92;
			this.column_type.Caption = "节假日类型";
			this.column_type.Name = "column_type";
			this.column_type.Visible = true;
			this.column_type.VisibleIndex = 2;
			this.column_type.Width = 92;
			this.column_start.Caption = "开始日期";
			this.column_start.Name = "column_start";
			this.column_start.Visible = true;
			this.column_start.VisibleIndex = 3;
			this.column_start.Width = 92;
			this.column_end.Caption = "结束日期";
			this.column_end.Name = "column_end";
			this.column_end.Visible = true;
			this.column_end.VisibleIndex = 4;
			this.column_end.Width = 92;
			this.column_isloop.Caption = "是否按年循环";
			this.column_isloop.Name = "column_isloop";
			this.column_isloop.Visible = true;
			this.column_isloop.VisibleIndex = 5;
			this.column_isloop.Width = 92;
			this.column_remark.Caption = "备注";
			this.column_remark.Name = "column_remark";
			this.column_remark.Visible = true;
			this.column_remark.VisibleIndex = 6;
			this.column_remark.Width = 92;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_holiday);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "HolidaysUserControl";
			base.Size = new Size(684, 515);
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
