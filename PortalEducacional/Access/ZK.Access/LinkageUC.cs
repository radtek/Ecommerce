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
using System.Xml;
using ZK.Access.door;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class LinkageUC : UserControl
	{
		private Dictionary<string, Dictionary<string, string>> m_inAddrTypeDic = null;

		private Dictionary<string, Dictionary<string, string>> m_outAddrTypeDic = null;

		private DataTable m_datatable = new DataTable();

		private Dictionary<int, string> eventTypeList = new Dictionary<int, string>();

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private AccLinkAgeIoBll bll = new AccLinkAgeIoBll(MainForm._ia);

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

		public PanelEx pnl_linkage;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_linkgeName;

		private GridColumn column_devName;

		private GridColumn column_triggerOpt;

		private GridColumn column_inAddress;

		private GridColumn column_outType;

		private GridColumn column_outAddress;

		public ToolStrip MenuPanelEx;

		private GridColumn column_check;

		private GridColumn column_actionType;

		public LinkageUC()
		{
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitOutType();
				this.InitInAddrType();
				this.InitOutAddrType();
				this.InitDataTableSet();
				this.loadEventType();
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

		private void InitOutType()
		{
			XmlNode rootNode = initLang.GetRootNode("LinkOutType");
			if (rootNode != null)
			{
				string addNodeValueByName = initLang.GetAddNodeValueByName("0", rootNode, false);
				if (string.IsNullOrEmpty(addNodeValueByName))
				{
					addNodeValueByName = "门锁";
					initLang.SetAddNodeValue("0", addNodeValueByName, rootNode, false);
				}
				addNodeValueByName = initLang.GetAddNodeValueByName("1", rootNode, false);
				if (string.IsNullOrEmpty(addNodeValueByName))
				{
					addNodeValueByName = "辅助输出";
					initLang.SetAddNodeValue("1", addNodeValueByName, rootNode, false);
					initLang.Save();
				}
			}
		}

		private void InitInAddrType()
		{
			this.m_inAddrTypeDic = initLang.GetComboxInfo("InAddrType");
			if (this.m_inAddrTypeDic == null || this.m_inAddrTypeDic.Count == 0)
			{
				this.m_inAddrTypeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("0", "任意");
				dictionary.Add("1", "门1");
				dictionary.Add("2", "门2");
				dictionary.Add("3", "门3");
				dictionary.Add("4", "门4");
				this.m_inAddrTypeDic.Add("0", dictionary);
				dictionary = new Dictionary<string, string>();
				dictionary.Add("0", "任意");
				dictionary.Add("1", "辅助输入1");
				dictionary.Add("2", "辅助输入2");
				dictionary.Add("3", "辅助输入3");
				dictionary.Add("4", "辅助输入4");
				this.m_inAddrTypeDic.Add("1", dictionary);
				initLang.SetComboxInfo("InAddrType", this.m_inAddrTypeDic);
				initLang.Save();
			}
		}

		private void InitOutAddrType()
		{
			this.m_outAddrTypeDic = initLang.GetComboxInfo("OutAddrType");
			if (this.m_outAddrTypeDic == null || this.m_outAddrTypeDic.Count == 0)
			{
				this.m_inAddrTypeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("1", "门锁1");
				dictionary.Add("2", "门锁2");
				dictionary.Add("3", "门锁3");
				dictionary.Add("4", "门锁4");
				this.m_outAddrTypeDic.Add("0", dictionary);
				dictionary = new Dictionary<string, string>();
				dictionary.Add("1", "辅助输出1");
				dictionary.Add("2", "辅助输出2");
				dictionary.Add("3", "辅助输出3");
				dictionary.Add("4", "辅助输出4");
				dictionary.Add("5", "辅助输出5");
				dictionary.Add("6", "辅助输出6");
				this.m_outAddrTypeDic.Add("1", dictionary);
				initLang.SetComboxInfo("OutAddrType", this.m_outAddrTypeDic);
				initLang.Save();
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("ID");
			this.m_datatable.Columns.Add("linkgeName");
			this.m_datatable.Columns.Add("devName");
			this.m_datatable.Columns.Add("triggerOpt");
			this.m_datatable.Columns.Add("inAddress");
			this.m_datatable.Columns.Add("outType");
			this.m_datatable.Columns.Add("outAddress");
			this.m_datatable.Columns.Add("actionType");
			this.m_datatable.Columns.Add("check");
			this.column_linkgeName.FieldName = "linkgeName";
			this.column_devName.FieldName = "devName";
			this.column_triggerOpt.FieldName = "triggerOpt";
			this.column_inAddress.FieldName = "inAddress";
			this.column_outType.FieldName = "outType";
			this.column_outAddress.FieldName = "outAddress";
			this.column_actionType.FieldName = "actionType";
			this.column_check.FieldName = "check";
			this.grd_view.DataSource = this.m_datatable;
		}

		private void loadEventType()
		{
			this.eventTypeList = PullSDKEventInfos.GetDic();
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				List<Machines> list = null;
				list = this.mbll.GetModelList("DevSDKType <> 2 and device_type < 101");
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
				List<AccLinkAgeIo> modelList = this.bll.GetModelList("");
				XmlNode rootNode = initLang.GetRootNode("LinkOutType");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.mlist.ContainsKey(modelList[i].device_id))
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = modelList[i].id;
							dataRow[1] = modelList[i].linkage_name;
							dataRow[2] = this.mlist[modelList[i].device_id].MachineAlias;
							if (this.eventTypeList.ContainsKey(modelList[i].trigger_opt))
							{
								dataRow[3] = this.eventTypeList[modelList[i].trigger_opt];
							}
							int num;
							if (modelList[i].trigger_opt == 220 || modelList[i].trigger_opt == 221)
							{
								if (this.m_inAddrTypeDic != null && this.m_inAddrTypeDic.ContainsKey("1"))
								{
									Dictionary<string, string> dictionary = this.m_inAddrTypeDic["1"];
									Dictionary<string, string> dictionary2 = dictionary;
									num = modelList[i].in_address;
									if (dictionary2.ContainsKey(num.ToString()))
									{
										DataRow dataRow2 = dataRow;
										Dictionary<string, string> dictionary3 = dictionary;
										num = modelList[i].in_address;
										dataRow2[4] = dictionary3[num.ToString()];
									}
								}
							}
							else if (this.m_inAddrTypeDic != null && this.m_inAddrTypeDic.ContainsKey("0"))
							{
								Dictionary<string, string> dictionary4 = this.m_inAddrTypeDic["0"];
								Dictionary<string, string> dictionary5 = dictionary4;
								num = modelList[i].in_address;
								if (dictionary5.ContainsKey(num.ToString()))
								{
									DataRow dataRow3 = dataRow;
									Dictionary<string, string> dictionary6 = dictionary4;
									num = modelList[i].in_address;
									dataRow3[4] = dictionary6[num.ToString()];
								}
							}
							if (rootNode != null)
							{
								num = modelList[i].out_type_hide;
								string text = (string)(dataRow[5] = initLang.GetAddNodeValueByName(num.ToString(), rootNode, false));
							}
							if (modelList[i].out_type_hide == 1)
							{
								if (this.m_outAddrTypeDic != null && this.m_outAddrTypeDic.ContainsKey("1"))
								{
									Dictionary<string, string> dictionary7 = this.m_outAddrTypeDic["1"];
									Dictionary<string, string> dictionary8 = dictionary7;
									num = modelList[i].out_address;
									if (dictionary8.ContainsKey(num.ToString()))
									{
										DataRow dataRow4 = dataRow;
										Dictionary<string, string> dictionary9 = dictionary7;
										num = modelList[i].out_address;
										dataRow4[6] = dictionary9[num.ToString()];
									}
								}
							}
							else if (this.m_outAddrTypeDic != null && this.m_outAddrTypeDic.ContainsKey("0"))
							{
								Dictionary<string, string> dictionary10 = this.m_outAddrTypeDic["0"];
								Dictionary<string, string> dictionary11 = dictionary10;
								num = modelList[i].out_address;
								if (dictionary11.ContainsKey(num.ToString()))
								{
									DataRow dataRow5 = dataRow;
									Dictionary<string, string> dictionary12 = dictionary10;
									num = modelList[i].out_address;
									dataRow5[6] = dictionary12[num.ToString()];
								}
							}
							if (modelList[i].action_type == 0)
							{
								dataRow[7] = ShowMsgInfos.GetInfo("SLinkAnctionTypeClose", "关闭");
							}
							else if (modelList[i].action_type == 1)
							{
								dataRow[7] = ShowMsgInfos.GetInfo("SLinkAnctionTypeOpen", "打开");
							}
							else if (modelList[i].action_type == 2)
							{
								dataRow[7] = ShowMsgInfos.GetInfo("SLinkAnctionTypeNormalOpen", "常开");
							}
							else
							{
								dataRow[7] = "";
							}
							dataRow[8] = false;
							this.m_datatable.Rows.Add(dataRow);
						}
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
			LinkageEdit linkageEdit = new LinkageEdit(0);
			linkageEdit.RefreshDataEvent += this.frmLink_RefreshDataEvent;
			linkageEdit.Text = this.btn_add.Text;
			linkageEdit.ShowDialog();
			linkageEdit.RefreshDataEvent -= this.frmLink_RefreshDataEvent;
		}

		private void frmLink_RefreshDataEvent(object sender, EventArgs e)
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
							LinkageEdit linkageEdit = new LinkageEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							linkageEdit.RefreshDataEvent += this.frmLink_RefreshDataEvent;
							linkageEdit.Text = this.btn_edit.Text;
							linkageEdit.ShowDialog();
							linkageEdit.RefreshDataEvent -= this.frmLink_RefreshDataEvent;
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
								AccLinkAgeIo model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
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
							AccLinkAgeIo model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LinkageUC));
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.saveFileDialog1 = new SaveFileDialog();
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.pnl_linkage = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_linkgeName = new GridColumn();
			this.column_devName = new GridColumn();
			this.column_triggerOpt = new GridColumn();
			this.column_inAddress = new GridColumn();
			this.column_outType = new GridColumn();
			this.column_outAddress = new GridColumn();
			this.column_actionType = new GridColumn();
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
			this.contextMenuStrip1.Size = new Size(153, 92);
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
			this.MenuPanelEx.TabIndex = 7;
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
			this.pnl_linkage.CanvasColor = SystemColors.Control;
			this.pnl_linkage.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_linkage.Dock = DockStyle.Top;
			this.pnl_linkage.Location = new Point(0, 38);
			this.pnl_linkage.Name = "pnl_linkage";
			this.pnl_linkage.Size = new Size(684, 23);
			this.pnl_linkage.Style.Alignment = StringAlignment.Center;
			this.pnl_linkage.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_linkage.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_linkage.Style.Border = eBorderType.SingleLine;
			this.pnl_linkage.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_linkage.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_linkage.Style.GradientAngle = 90;
			this.pnl_linkage.TabIndex = 8;
			this.pnl_linkage.Text = "联动";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 388);
			this.grd_view.TabIndex = 9;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[8]
			{
				this.column_check,
				this.column_linkgeName,
				this.column_devName,
				this.column_triggerOpt,
				this.column_inAddress,
				this.column_outType,
				this.column_outAddress,
				this.column_actionType
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
			this.column_linkgeName.Caption = "联动设置名称";
			this.column_linkgeName.Name = "column_linkgeName";
			this.column_linkgeName.Visible = true;
			this.column_linkgeName.VisibleIndex = 1;
			this.column_linkgeName.Width = 100;
			this.column_devName.Caption = "设备";
			this.column_devName.Name = "column_devName";
			this.column_devName.Visible = true;
			this.column_devName.VisibleIndex = 2;
			this.column_devName.Width = 100;
			this.column_triggerOpt.Caption = "触发条件";
			this.column_triggerOpt.Name = "column_triggerOpt";
			this.column_triggerOpt.Visible = true;
			this.column_triggerOpt.VisibleIndex = 3;
			this.column_triggerOpt.Width = 100;
			this.column_inAddress.Caption = "输入点地址";
			this.column_inAddress.Name = "column_inAddress";
			this.column_inAddress.Visible = true;
			this.column_inAddress.VisibleIndex = 4;
			this.column_inAddress.Width = 100;
			this.column_outType.Caption = "输出类型";
			this.column_outType.Name = "column_outType";
			this.column_outType.Visible = true;
			this.column_outType.VisibleIndex = 5;
			this.column_outType.Width = 100;
			this.column_outAddress.Caption = "输出点地址";
			this.column_outAddress.Name = "column_outAddress";
			this.column_outAddress.Visible = true;
			this.column_outAddress.VisibleIndex = 6;
			this.column_outAddress.Width = 107;
			this.column_actionType.Caption = "动作类型";
			this.column_actionType.Name = "column_actionType";
			this.column_actionType.Visible = true;
			this.column_actionType.VisibleIndex = 7;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_linkage);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "LinkageUC";
			base.Size = new Size(684, 449);
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
