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
	public class MultiCardOpenUC : UserControl
	{
		private ImagesForm imagesForm = new ImagesForm();

		private DataTable m_datatable = new DataTable();

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private AccDoorBll dbll = new AccDoorBll(MainForm._ia);

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private Dictionary<int, List<AccMorecardGroup>> glist = new Dictionary<int, List<AccMorecardGroup>>();

		private Dictionary<int, string> groupnameidlist = new Dictionary<int, string>();

		private AccMorecardsetBll bll = new AccMorecardsetBll(MainForm._ia);

		private WaitForm m_wait = WaitForm.Instance;

		private bool isDouble = false;

		private bool IsEditing = false;

		private IContainer components = null;

		private SaveFileDialog saveFileDialog1;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_addMultiCard;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_delete;

		private ToolStripButton btn_addMultiCard;

		private ToolStripButton btn_multiCard;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		public PanelEx pnl_multiCard;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_dev;

		private GridColumn column_interLock;

		private GridColumn column_start;

		private GridColumn column_end;

		private GridColumn column_isloop;

		private GridColumn column_remark;

		private GridColumn column_check;

		public ToolStrip MenupanelEx;

		public MultiCardOpenUC()
		{
			this.imagesForm.TopMost = true;
			this.imagesForm.Show();
			Application.DoEvents();
			this.InitializeComponent();
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				this.InitDataTableSet();
				this.InitMachines();
				this.InitGroupName();
				this.InitGroup();
				this.InitDoor();
				this.LoadData();
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
				this.btn_multiCard.Image = ResourceIPC.add;
				this.btn_delete.Image = ResourceIPC.delete;
				this.btn_edit.Image = ResourceIPC.edit;
				this.btn_addMultiCard.Image = ResourceIPC.openDoorTeam;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel))
			{
				this.btn_edit.Enabled = false;
				this.btn_addMultiCard.Enabled = false;
				this.btn_multiCard.Enabled = false;
				this.btn_delete.Enabled = false;
				this.Menu_addMultiCard.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_delete.Enabled = false;
				this.Menu_add.Enabled = false;
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
			this.column_dev.FieldName = "devicename";
			this.column_interLock.FieldName = "InterlockType";
			this.column_end.FieldName = "end";
			this.column_isloop.FieldName = "isloop";
			this.column_remark.FieldName = "remark";
			this.column_start.FieldName = "start";
			this.column_check.FieldName = "check";
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
				this.Cursor = Cursors.Default;
			}
		}

		private void InitDoor()
		{
			try
			{
				this.dlist.Clear();
				List<AccDoor> modelList = this.dbll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
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
					this.btn_multiCard.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void InitGroup()
		{
			try
			{
				this.glist.Clear();
				AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
				List<AccMorecardGroup> modelList = accMorecardGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.glist.ContainsKey(modelList[i].comb_id))
						{
							this.glist[modelList[i].comb_id].Add(modelList[i]);
						}
						else
						{
							List<AccMorecardGroup> list = new List<AccMorecardGroup>();
							list.Add(modelList[i]);
							this.glist.Add(modelList[i].comb_id, list);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void InitGroupName()
		{
			try
			{
				this.groupnameidlist.Clear();
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				List<AccMorecardempGroup> modelList = accMorecardempGroupBll.GetModelList("");
				if (modelList != null)
				{
					bool flag = false;
					if (modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							if (!this.groupnameidlist.ContainsKey(modelList[i].id))
							{
								this.groupnameidlist.Add(modelList[i].id, modelList[i].group_name);
							}
							if (modelList[i].Acount > 0)
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						this.btn_multiCard.Enabled = false;
						this.Menu_add.Enabled = false;
					}
					else if (this.dlist.Count > 0)
					{
						this.btn_multiCard.Enabled = true;
						this.Menu_add.Enabled = true;
					}
				}
				else
				{
					this.btn_multiCard.Enabled = false;
					this.Menu_add.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_addMultiCard_Click(object sender, EventArgs e)
		{
			MultiCardOpenGroupSet multiCardOpenGroupSet = new MultiCardOpenGroupSet();
			multiCardOpenGroupSet.Text = this.btn_addMultiCard.Text;
			multiCardOpenGroupSet.ShowDialog();
			this.InitGroupName();
		}

		private void LoadData()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				List<AccMorecardempGroup> modelList = accMorecardempGroupBll.GetModelList("");
				List<AccMorecardset> modelList2 = this.bll.GetModelList("");
				if (modelList2 != null)
				{
					for (int i = 0; i < modelList2.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = modelList2[i].id;
						dataRow[1] = modelList2[i].comb_name;
						if (this.dlist.ContainsKey(modelList2[i].door_id))
						{
							if (this.mlist.ContainsKey(this.dlist[modelList2[i].door_id].device_id))
							{
								dataRow[2] = this.mlist[this.dlist[modelList2[i].door_id].device_id].MachineAlias;
							}
							else
							{
								dataRow[2] = "";
							}
							dataRow[3] = this.dlist[modelList2[i].door_id].door_no;
							dataRow[4] = this.dlist[modelList2[i].door_id].door_name;
						}
						else
						{
							dataRow[2] = "";
							dataRow[3] = "";
							dataRow[4] = "";
						}
						int num = 0;
						string text = string.Empty;
						if (this.glist.ContainsKey(modelList2[i].id))
						{
							List<AccMorecardGroup> list = this.glist[modelList2[i].id];
							if (list != null && list.Count > 0)
							{
								for (int j = 0; j < list.Count; j++)
								{
									num += list[j].opener_number;
									if (modelList != null && modelList.Count > 0)
									{
										for (int k = 0; k < modelList.Count; k++)
										{
											if (modelList[k].id == list[j].group_id)
											{
												text = ((!this.groupnameidlist.ContainsKey(list[j].group_id)) ? (text + this.groupnameidlist[list[j].group_id] + ";") : (text + this.groupnameidlist[list[j].group_id] + "-" + modelList[k].Acount + ";"));
											}
										}
									}
								}
							}
						}
						dataRow[5] = num;
						dataRow[6] = text;
						dataRow[7] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_delete.Enabled = true;
					this.btn_edit.Enabled = true;
					this.Menu_delete.Enabled = true;
					this.Menu_edit.Enabled = true;
				}
				else
				{
					this.btn_delete.Enabled = false;
					this.btn_edit.Enabled = false;
					this.Menu_delete.Enabled = false;
					this.Menu_edit.Enabled = false;
				}
				this.CheckPermission();
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_multiCard_Click(object sender, EventArgs e)
		{
			try
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					List<AccMorecardempGroup> modelList2 = accMorecardempGroupBll.GetModelList("");
					if (modelList2 != null && modelList2.Count > 0)
					{
						bool flag = false;
						int num = 0;
						while (num < modelList2.Count)
						{
							if (modelList2[num].Acount <= 0)
							{
								num++;
								continue;
							}
							flag = true;
							break;
						}
						if (flag)
						{
							MultiCardOpenSet multiCardOpenSet = new MultiCardOpenSet(0);
							multiCardOpenSet.RefreshDataEvent += this.fopen_RefreshDataEvent;
							multiCardOpenSet.ShowDialog();
							multiCardOpenSet.RefreshDataEvent -= this.fopen_RefreshDataEvent;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoMorecardempGroup", "请先设置多卡开门人员组"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoMorecardempGroup", "请先设置多卡开门人员组"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNodoors", "没有门"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			if (!this.IsEditing)
			{
				this.IsEditing = true;
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
								MultiCardOpenSet multiCardOpenSet = new MultiCardOpenSet(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
								multiCardOpenSet.RefreshDataEvent += this.fopen_RefreshDataEvent;
								multiCardOpenSet.ShowDialog();
								multiCardOpenSet.RefreshDataEvent -= this.fopen_RefreshDataEvent;
							}
							else
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
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
					this.Cursor = Cursors.Default;
				}
				this.IsEditing = false;
			}
		}

		private void fopen_RefreshDataEvent(object sender, EventArgs e)
		{
			this.InitGroup();
			this.LoadData();
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				Machines machines = null;
				if (this.m_datatable.Rows.Count > 0)
				{
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							this.m_wait.ShowEx();
							bool flag = false;
							for (int i = 0; i < checkedRows.Length; i++)
							{
								this.m_wait.ShowProgress(90 * i / checkedRows.Length);
								if (checkedRows[i] < 0 || checkedRows[i] >= this.m_datatable.Rows.Count)
								{
									break;
								}
								AccMorecardset model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									if (this.dlist.ContainsKey(model.door_id))
									{
										AccDoor accDoor = this.dlist[model.door_id];
										if (this.mlist.ContainsKey(accDoor.device_id))
										{
											machines = this.mlist[accDoor.device_id];
										}
									}
									if (machines == null || machines.DevSDKType != SDKType.StandaloneSDK)
									{
										CommandServer.DelCmd(model);
									}
									this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									if (machines != null && machines.DevSDKType == SDKType.StandaloneSDK)
									{
										CommandServer.AddAllUnlockGroupCmd(machines);
									}
									flag = true;
								}
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
				this.Cursor = Cursors.Default;
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
						this.m_wait.ShowEx();
						for (int i = 0; i < this.m_datatable.Rows.Count && this.m_datatable.Rows[i][0] != null; i++)
						{
							this.m_wait.ShowProgress(90 * i / this.m_datatable.Rows.Count);
							AccMorecardset model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							if (model != null)
							{
								CommandServer.DelCmd(model);
								this.bll.Delete(int.Parse(this.m_datatable.Rows[i][0].ToString()));
								flag = true;
							}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MultiCardOpenUC));
			this.saveFileDialog1 = new SaveFileDialog();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_addMultiCard = new ToolStripMenuItem();
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_delete = new ToolStripMenuItem();
			this.MenupanelEx = new ToolStrip();
			this.btn_addMultiCard = new ToolStripButton();
			this.btn_multiCard = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.pnl_multiCard = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_dev = new GridColumn();
			this.column_interLock = new GridColumn();
			this.column_start = new GridColumn();
			this.column_end = new GridColumn();
			this.column_isloop = new GridColumn();
			this.column_remark = new GridColumn();
			this.column_check = new GridColumn();
			this.contextMenuStrip1.SuspendLayout();
			this.MenupanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[4]
			{
				this.Menu_addMultiCard,
				this.Menu_add,
				this.Menu_edit,
				this.Menu_delete
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(179, 92);
			this.Menu_addMultiCard.Image = (Image)componentResourceManager.GetObject("Menu_addMultiCard.Image");
			this.Menu_addMultiCard.Name = "Menu_addMultiCard";
			this.Menu_addMultiCard.Size = new Size(178, 22);
			this.Menu_addMultiCard.Text = "多卡开门人员组设置";
			this.Menu_addMultiCard.Click += this.btn_addMultiCard_Click;
			this.Menu_add.Image = Resources.add;
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(178, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.btn_multiCard_Click;
			this.Menu_edit.Image = Resources.edit;
			this.Menu_edit.Name = "Menu_edit";
			this.Menu_edit.Size = new Size(178, 22);
			this.Menu_edit.Text = "编辑";
			this.Menu_edit.Click += this.btn_edit_Click;
			this.Menu_delete.Image = Resources.delete;
			this.Menu_delete.Name = "Menu_delete";
			this.Menu_delete.Size = new Size(178, 22);
			this.Menu_delete.Text = "删除";
			this.Menu_delete.Click += this.btn_delete_Click;
			this.MenupanelEx.AutoSize = false;
			this.MenupanelEx.Items.AddRange(new ToolStripItem[4]
			{
				this.btn_addMultiCard,
				this.btn_multiCard,
				this.btn_edit,
				this.btn_delete
			});
			this.MenupanelEx.Location = new Point(0, 0);
			this.MenupanelEx.Name = "MenupanelEx";
			this.MenupanelEx.Size = new Size(684, 38);
			this.MenupanelEx.TabIndex = 9;
			this.MenupanelEx.Text = "toolStrip1";
			this.btn_addMultiCard.Image = (Image)componentResourceManager.GetObject("btn_addMultiCard.Image");
			this.btn_addMultiCard.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_addMultiCard.ImageTransparentColor = Color.Magenta;
			this.btn_addMultiCard.Name = "btn_addMultiCard";
			this.btn_addMultiCard.Size = new Size(160, 35);
			this.btn_addMultiCard.Text = "多卡开门人员组设置";
			this.btn_addMultiCard.Click += this.btn_addMultiCard_Click;
			this.btn_multiCard.Image = (Image)componentResourceManager.GetObject("btn_multiCard.Image");
			this.btn_multiCard.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_multiCard.ImageTransparentColor = Color.Magenta;
			this.btn_multiCard.Name = "btn_multiCard";
			this.btn_multiCard.Size = new Size(69, 35);
			this.btn_multiCard.Text = "新增";
			this.btn_multiCard.Click += this.btn_multiCard_Click;
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
			this.pnl_multiCard.CanvasColor = SystemColors.Control;
			this.pnl_multiCard.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_multiCard.Dock = DockStyle.Top;
			this.pnl_multiCard.Location = new Point(0, 38);
			this.pnl_multiCard.Name = "pnl_multiCard";
			this.pnl_multiCard.Size = new Size(684, 23);
			this.pnl_multiCard.Style.Alignment = StringAlignment.Center;
			this.pnl_multiCard.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_multiCard.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_multiCard.Style.Border = eBorderType.SingleLine;
			this.pnl_multiCard.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_multiCard.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_multiCard.Style.GradientAngle = 90;
			this.pnl_multiCard.TabIndex = 10;
			this.pnl_multiCard.Text = "多卡开门";
			this.grd_view.ContextMenuStrip = this.contextMenuStrip1;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 388);
			this.grd_view.TabIndex = 11;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[7]
			{
				this.column_dev,
				this.column_interLock,
				this.column_start,
				this.column_end,
				this.column_isloop,
				this.column_remark,
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
			this.column_dev.Caption = "组合名称";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 92;
			this.column_interLock.Caption = "设备";
			this.column_interLock.Name = "column_interLock";
			this.column_interLock.Visible = true;
			this.column_interLock.VisibleIndex = 2;
			this.column_interLock.Width = 92;
			this.column_start.Caption = "门编号";
			this.column_start.Name = "column_start";
			this.column_start.Visible = true;
			this.column_start.VisibleIndex = 3;
			this.column_start.Width = 92;
			this.column_end.Caption = "门名称";
			this.column_end.Name = "column_end";
			this.column_end.Visible = true;
			this.column_end.VisibleIndex = 4;
			this.column_end.Width = 92;
			this.column_isloop.Caption = "同时刷卡数";
			this.column_isloop.Name = "column_isloop";
			this.column_isloop.Width = 92;
			this.column_remark.Caption = "多卡开门组";
			this.column_remark.Name = "column_remark";
			this.column_remark.Visible = true;
			this.column_remark.VisibleIndex = 5;
			this.column_remark.Width = 92;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 40;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_multiCard);
			base.Controls.Add(this.MenupanelEx);
			base.Name = "MultiCardOpenUC";
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
