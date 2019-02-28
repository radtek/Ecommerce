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
using ZK.Access.data;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DepartmentForm : UserControl
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private DepartmentsBll bll = new DepartmentsBll(MainForm._ia);

		private string deptname;

		private string deptcode;

		private DataTable m_datatable = new DataTable();

		private Dictionary<string, int> m_deptNameDic = new Dictionary<string, int>();

		private int defalutid = 0;

		private TreeView tree = new TreeView();

		private NodeManager nodemg = new NodeManager();

		private List<int> deptIdNO = new List<int>();

		private WaitForm m_waitForm = WaitForm.Instance;

		private ImportDataHelper import = null;

		private List<Departments> m_depts = new List<Departments>();

		private bool isDouble = false;

		private IContainer components = null;

		private BindingSource zKaccessDataSetBindingSource;

		private BindingSource dEPARTMENTSBindingSource;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_Tree;

		private ToolStripMenuItem Menu_import;

		private ToolStripMenuItem Menu_export;

		private ToolStripMenuItem Menu_log;

		private ToolStripButton btn_tree;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		private ToolStripButton btn_import;

		private ToolStripButton btn_export;

		private ToolStripButton btn_log;

		public PanelEx pnl_dept;

		private GridControl grd_view;

		private GridView grd_deptView;

		private GridColumn column_check;

		private GridColumn column_code;

		private GridColumn column_deptname;

		private GridColumn column_parentName;

		public ToolStrip MenuPanelEx;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_detele;

		public DepartmentForm()
		{
			try
			{
				this.InitializeComponent();
				DevExpressHelper.InitImageList(this.grd_deptView, "column_check");
				this.InitDataTableSet();
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
				this.btn_export.Image = ResourceIPC.Export;
				this.btn_import.Image = ResourceIPC.Import;
				this.btn_log.Image = ResourceIPC.Log_Entries;
				this.btn_tree.Image = ResourceIPC.Tree_display;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Personnel))
			{
				this.btn_tree.Enabled = false;
				this.btn_import.Enabled = false;
				this.Menu_import.Enabled = false;
				this.Menu_Tree.Enabled = false;
				this.btn_add.Enabled = false;
				this.btn_delete.Enabled = false;
				this.btn_edit.Enabled = false;
				this.Menu_add.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_import.Enabled = false;
				this.Menu_Tree.Enabled = false;
				this.Menu_log.Enabled = false;
				this.btn_log.Enabled = false;
				this.btn_export.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_detele.Enabled = false;
				this.btn_log.Enabled = false;
				this.Menu_export.Enabled = false;
				this.grd_deptView.DoubleClick -= this.grd_mainView_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("DEPTID");
			this.m_datatable.Columns.Add("code");
			this.m_datatable.Columns.Add("deptname");
			this.m_datatable.Columns.Add("parentName");
			this.m_datatable.Columns.Add("check");
			this.m_datatable.Columns.Add("SUPDEPTID");
			this.column_code.FieldName = "code";
			this.column_deptname.FieldName = "deptname";
			this.column_parentName.FieldName = "parentName";
			this.column_check.FieldName = "check";
		}

		private void DataBind()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				this.m_deptNameDic.Clear();
				this.defalutid = -1;
				DataSet allList = this.bll.GetAllList();
				if (SysInfos.AdminID == SysInfos.SysUserInfo.id && (allList == null || allList.Tables.Count == 0 || allList.Tables[0].Rows.Count == 0))
				{
					Departments departments = new Departments();
					departments.code = "1";
					departments.DEPTNAME = ShowMsgInfos.GetInfo("SCompanyName", "总公司");
					departments.SUPDEPTID = 0;
					this.bll.Add(departments);
					allList = this.bll.GetAllList();
				}
				if (allList != null && allList.Tables.Count > 0)
				{
					DataTable dataTable = allList.Tables[0];
					this.nodemg = new NodeManager();
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = dataTable.Rows[i]["DEPTID"].ToString();
						dataRow[1] = dataTable.Rows[i]["code"].ToString();
						dataRow[2] = dataTable.Rows[i]["DEPTNAME"].ToString();
						dataRow[3] = dataTable.Rows[i]["parentName"].ToString();
						if (!this.m_deptNameDic.ContainsKey(dataTable.Rows[i]["DEPTNAME"].ToString()))
						{
							this.m_deptNameDic.Add(dataTable.Rows[i]["DEPTNAME"].ToString(), int.Parse(dataTable.Rows[i]["DEPTID"].ToString()));
						}
						if (dataTable.Rows[i]["SUPDEPTID"].ToString() == "0" || dataTable.Rows[i]["SUPDEPTID"].ToString() == "1")
						{
							this.defalutid = int.Parse(dataTable.Rows[i]["DEPTID"].ToString());
						}
						dataRow[4] = false;
						dataRow[5] = dataTable.Rows[i]["SUPDEPTID"].ToString();
						this.m_datatable.Rows.Add(dataRow);
						NodeBase nodeBase = new NodeBase();
						nodeBase.ID = dataTable.Rows[i]["DEPTID"].ToString();
						nodeBase.Name = nodeBase.ID;
						nodeBase.Tag = nodeBase.ID;
						nodeBase.ParentNodeID = dataTable.Rows[i]["SUPDEPTID"].ToString();
						this.nodemg.Datasouce.Add(nodeBase);
					}
					if (this.nodemg.Bind())
					{
						this.nodemg.ConvertToTree(this.tree);
					}
					if (this.defalutid == -1)
					{
						this.defalutid = int.Parse(dataTable.Rows[0]["DEPTID"].ToString());
					}
					this.grd_view.DataSource = this.m_datatable;
					this.column_check.ImageIndex = 1;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool Excit(object id)
		{
			if (id != null)
			{
				this.deptIdNO.Add(int.Parse(id.ToString()));
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				if (userInfoBll.ExistsDept(int.Parse(id.ToString())))
				{
					this.deptIdNO.Clear();
					return true;
				}
			}
			return false;
		}

		private bool IsHaveUser(INode node)
		{
			if (node == null)
			{
				return false;
			}
			if (this.Excit(node.ID))
			{
				return true;
			}
			if (node.Childs.Count > 0)
			{
				for (int i = 0; i < node.Childs.Count; i++)
				{
					if (this.IsHaveUser(node.Childs[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void deptsetting_Event(object sender, EventArgs e)
		{
			this.DataBind();
		}

		private void Add_button_Click(object sender, EventArgs e)
		{
			SettingDepartmentForm settingDepartmentForm = new SettingDepartmentForm();
			settingDepartmentForm.RefreshDataEvent += this.deptsetting_Event;
			settingDepartmentForm.ShowDialog();
			settingDepartmentForm.RefreshDataEvent -= this.deptsetting_Event;
		}

		private void DepartmentForm_Load(object sender, EventArgs e)
		{
			this.DataBind();
		}

		private void deptsearch_Event(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Departments departments = sender as Departments;
				this.deptname = departments.DEPTNAME;
				this.deptcode = departments.code;
			}
			this.DataBind();
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
			DeptSearchForm deptSearchForm = new DeptSearchForm();
			deptSearchForm.SearchDeptEvent += this.deptsearch_Event;
			deptSearchForm.ShowDialog();
			deptSearchForm.SearchDeptEvent -= this.deptsearch_Event;
		}

		private void btn_log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("Departments");
			logsInfoForm.ShowDialog();
			this.DataBind();
		}

		private void btn_export_Click(object sender, EventArgs e)
		{
			object[] obj = new object[8]
			{
				this.pnl_dept.Text,
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
			DevExpressHelper.OutData(this.grd_deptView, fileName);
		}

		private void ShowInfos(string infoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.ShowInfos), infoStr);
				}
				else
				{
					this.m_waitForm.ShowInfos(infoStr);
				}
			}
		}

		private void ShowProgress(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.ShowProgress), prg);
				}
				else
				{
					this.m_waitForm.ShowProgress(prg);
				}
			}
		}

		private void import_ShowEvent(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new EventHandler(this.import_ShowEvent), sender, e);
				}
				else
				{
					this.m_waitForm.ShowEx();
				}
			}
		}

		private void btn_import_Click(object sender, EventArgs e)
		{
			try
			{
				this.btn_import.Enabled = false;
				if (this.import == null)
				{
					this.import = new ImportDataHelper();
					this.import.SetImportColumnsEvent += this.import_SetImportColumnsEvent;
					this.import.GetRowModelEvent += this.import_GetRowModelEvent;
					this.import.SaveModelEvent += this.import_SaveModelEvent;
					this.import.CheckDataEvent += this.import_CheckDataEvent;
					this.import.FinishEvent += this.import_FinishEvent;
					this.import.ShowInfoEvent += this.ShowInfos;
					this.import.ShowProgressEvent += this.ShowProgress;
					this.import.ShowEvent += this.import_ShowEvent;
					this.m_depts.Clear();
					this.import.Start();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void import_FinishEvent(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				try
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new EventHandler(this.import_FinishEvent), sender, e);
					}
					else
					{
						this.Cursor = Cursors.WaitCursor;
						this.UpdateDept();
						this.DataBind();
						this.btn_import.Enabled = true;
						this.Cursor = Cursors.Default;
						this.import = null;
						this.m_waitForm.HideEx(false);
					}
				}
				catch
				{
				}
			}
		}

		private void UpdateDept()
		{
			try
			{
				for (int i = 0; i < this.m_depts.Count; i++)
				{
					if (!string.IsNullOrEmpty(this.m_depts[i].SupDeptName) && this.m_deptNameDic.ContainsKey(this.m_depts[i].SupDeptName))
					{
						int num = this.m_deptNameDic[this.m_depts[i].SupDeptName];
						if (this.m_depts[i].SUPDEPTID != num)
						{
							this.m_depts[i].SUPDEPTID = num;
							this.bll.Update(this.m_depts[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (this.import != null)
				{
					this.import.ShowInfo(ex.Message);
				}
			}
		}

		private void import_CheckDataEvent(DataConfig config)
		{
			if (config != null)
			{
				try
				{
					string info = ShowMsgInfos.GetInfo("DeptNo", "部门编号");
					string info2 = ShowMsgInfos.GetInfo("deptName", "部门名称");
					if (config.ColumnsToColumnsDic.ContainsKey(info) && !string.IsNullOrEmpty(config.ColumnsToColumnsDic[info]))
					{
						if (config.ColumnsToColumnsDic.ContainsKey(info2) && !string.IsNullOrEmpty(config.ColumnsToColumnsDic[info2]))
						{
							int num = 0;
							while (true)
							{
								if (num < config.SelectDataSource.Rows.Count)
								{
									DataRow dataRow = config.SelectDataSource.Rows[num];
									if (dataRow[config.ColumnsToColumnsDic[info]] == null || dataRow[config.ColumnsToColumnsDic[info2]] == null)
									{
										break;
									}
									string value = dataRow[config.ColumnsToColumnsDic[info]].ToString();
									if (!string.IsNullOrEmpty(value))
									{
										value = dataRow[config.ColumnsToColumnsDic[info2]].ToString();
										if (string.IsNullOrEmpty(value))
										{
											config.Check = false;
											return;
										}
										num++;
										continue;
									}
									config.Check = false;
								}
								return;
							}
							config.Check = false;
						}
						else
						{
							config.Check = false;
						}
					}
					else
					{
						config.Check = false;
					}
				}
				catch (Exception ex)
				{
					if (this.import != null)
					{
						this.import.ShowInfo(ex.Message);
					}
					config.Check = false;
				}
			}
		}

		private bool import_SaveModelEvent(object model)
		{
			if (model != null)
			{
				try
				{
					Departments departments = model as Departments;
					if (departments != null)
					{
						if (!string.IsNullOrEmpty(departments.DEPTNAME))
						{
							if (!this.bll.ExistsDeptName(departments.DEPTNAME))
							{
								if (!string.IsNullOrEmpty(departments.DEPTNAME))
								{
									if (!this.bll.ExistsCode(departments.code))
									{
										if (!string.IsNullOrEmpty(departments.SupDeptName))
										{
											if (this.m_deptNameDic.ContainsKey(departments.SupDeptName))
											{
												departments.SUPDEPTID = this.m_deptNameDic[departments.SupDeptName];
												this.bll.Add(departments);
											}
											else
											{
												departments.SUPDEPTID = this.defalutid;
												this.m_depts.Add(departments);
												this.bll.Add(departments);
											}
										}
										else
										{
											departments.SUPDEPTID = this.defalutid;
											this.m_depts.Add(departments);
											this.bll.Add(departments);
										}
										if (!this.m_deptNameDic.ContainsKey(departments.DEPTNAME))
										{
											departments.DEPTID = this.bll.GetMaxId() - 1;
											this.m_deptNameDic.Add(departments.DEPTNAME, departments.DEPTID);
										}
										return true;
									}
									this.import.Config.CheckErrorInfo = ShowMsgInfos.GetInfo("SDeptCodeRepeat", "部门编号已经存在");
								}
								else
								{
									this.import.Config.CheckErrorInfo = ShowMsgInfos.GetInfo("SDeptCodeNull", "部门编号不能为空!");
								}
							}
							else
							{
								this.import.Config.CheckErrorInfo = ShowMsgInfos.GetInfo("SDeptNameRepeat", "部门名称重复!");
							}
						}
						else
						{
							this.import.Config.CheckErrorInfo = ShowMsgInfos.GetInfo("SDeptNameNull", "部门名称不能为空!");
						}
					}
				}
				catch (Exception ex)
				{
					if (this.import != null)
					{
						this.import.ShowInfo(ex.Message);
					}
				}
			}
			return false;
		}

		private object import_GetRowModelEvent()
		{
			return new Departments();
		}

		private void import_SetImportColumnsEvent(DataConfig config)
		{
			if (config != null)
			{
				try
				{
					config.ImportColumns.Clear();
					config.ColumnsToModelDic.Clear();
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("DeptNo", "部门编号"));
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("deptName", "部门名称"));
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("supDeptName", "上级部门"));
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("DeptNo", "部门编号"), "code");
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("deptName", "部门名称"), "DEPTNAME");
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("supDeptName", "上级部门"), "SupDeptName");
				}
				catch (Exception ex)
				{
					if (this.import != null)
					{
						this.import.ShowInfo(ex.Message);
					}
				}
			}
		}

		private void addDept_RefreshDataEvent(object sender, EventArgs e)
		{
			this.DataBind();
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			AddDept addDept = new AddDept(0, 0);
			addDept.refreshDataEvent += this.addDept_RefreshDataEvent;
			addDept.ShowDialog();
			addDept.refreshDataEvent -= this.addDept_RefreshDataEvent;
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
						array = this.grd_deptView.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.grd_deptView, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.grd_deptView, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
					{
						if (array.Length == 1)
						{
							AddDept addDept = new AddDept(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()), 0);
							addDept.refreshDataEvent += this.addDept_RefreshDataEvent;
							addDept.ShowDialog();
							addDept.refreshDataEvent -= this.addDept_RefreshDataEvent;
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
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_deptView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (this.m_datatable.Rows.Count > 1)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							StringBuilder stringBuilder = new StringBuilder();
							this.m_waitForm.ShowEx();
							for (int i = 0; i < checkedRows.Length; i++)
							{
								this.m_waitForm.ShowProgress(i * 100 / checkedRows.Length);
								if (checkedRows[i] < 0 || checkedRows[i] >= this.m_datatable.Rows.Count)
								{
									break;
								}
								if (this.m_datatable.Rows[checkedRows[i]][5].ToString() == "0" || this.m_datatable.Rows[checkedRows[i]][5].ToString() == "-1")
								{
									this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("SdeptName", "部门名称") + this.m_datatable.Rows[checkedRows[i]][2].ToString() + ":" + ShowMsgInfos.GetInfo("rootNOdelete", "根部门不能删除!"));
								}
								else
								{
									this.deptIdNO.Clear();
									INode node = this.nodemg.NTree.FindNode(this.m_datatable.Rows[checkedRows[i]][0].ToString());
									if (node == null)
									{
										this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
										this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("SdeptName", "部门名称") + this.m_datatable.Rows[checkedRows[i]][2].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
									}
									else if (!this.IsHaveUser(node))
									{
										foreach (int item in this.deptIdNO)
										{
											this.bll.Delete(item);
										}
										this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("SdeptName", "部门名称") + this.m_datatable.Rows[checkedRows[i]][2].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
									}
									else
									{
										this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("SdeptName", "部门名称") + this.m_datatable.Rows[checkedRows[i]][2].ToString() + ":" + ShowMsgInfos.GetInfo("presencePersonnel", "该部门还有人员，不能删除!"));
									}
								}
							}
							this.DataBind();
							this.m_waitForm.ShowProgress(100);
							this.m_waitForm.HideEx(false);
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("rootNOdelete", "根部门不能删除!"));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DepartmentForm));
			this.dEPARTMENTSBindingSource = new BindingSource(this.components);
			this.zKaccessDataSetBindingSource = new BindingSource(this.components);
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_Tree = new ToolStripMenuItem();
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_detele = new ToolStripMenuItem();
			this.Menu_import = new ToolStripMenuItem();
			this.Menu_export = new ToolStripMenuItem();
			this.Menu_log = new ToolStripMenuItem();
			this.MenuPanelEx = new ToolStrip();
			this.btn_tree = new ToolStripButton();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.btn_import = new ToolStripButton();
			this.btn_export = new ToolStripButton();
			this.btn_log = new ToolStripButton();
			this.pnl_dept = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_deptView = new GridView();
			this.column_check = new GridColumn();
			this.column_code = new GridColumn();
			this.column_deptname = new GridColumn();
			this.column_parentName = new GridColumn();
			((ISupportInitialize)this.dEPARTMENTSBindingSource).BeginInit();
			((ISupportInitialize)this.zKaccessDataSetBindingSource).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_deptView).BeginInit();
			base.SuspendLayout();
			this.dEPARTMENTSBindingSource.DataMember = "DEPARTMENTS";
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[7]
			{
				this.Menu_Tree,
				this.Menu_add,
				this.Menu_edit,
				this.Menu_detele,
				this.Menu_import,
				this.Menu_export,
				this.Menu_log
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(119, 158);
			this.Menu_Tree.Image = (Image)componentResourceManager.GetObject("Menu_Tree.Image");
			this.Menu_Tree.Name = "Menu_Tree";
			this.Menu_Tree.Size = new Size(118, 22);
			this.Menu_Tree.Text = "树型显示";
			this.Menu_Tree.Click += this.Add_button_Click;
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
			this.Menu_detele.Image = Resources.delete;
			this.Menu_detele.Name = "Menu_detele";
			this.Menu_detele.Size = new Size(118, 22);
			this.Menu_detele.Text = "删除";
			this.Menu_detele.Click += this.btn_delete_Click;
			this.Menu_import.Image = Resources.Import;
			this.Menu_import.Name = "Menu_import";
			this.Menu_import.Size = new Size(118, 22);
			this.Menu_import.Text = "导入";
			this.Menu_import.Click += this.btn_import_Click;
			this.Menu_export.Image = Resources.Export;
			this.Menu_export.Name = "Menu_export";
			this.Menu_export.Size = new Size(118, 22);
			this.Menu_export.Text = "导出";
			this.Menu_export.Click += this.btn_export_Click;
			this.Menu_log.Image = Resources.Log_Entries;
			this.Menu_log.Name = "Menu_log";
			this.Menu_log.Size = new Size(118, 22);
			this.Menu_log.Text = "日志记录";
			this.Menu_log.Click += this.btn_log_Click;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[7]
			{
				this.btn_tree,
				this.btn_add,
				this.btn_edit,
				this.btn_delete,
				this.btn_import,
				this.btn_export,
				this.btn_log
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 38);
			this.MenuPanelEx.TabIndex = 16;
			this.btn_tree.Image = (Image)componentResourceManager.GetObject("btn_tree.Image");
			this.btn_tree.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_tree.ImageTransparentColor = Color.Magenta;
			this.btn_tree.Name = "btn_tree";
			this.btn_tree.Size = new Size(95, 35);
			this.btn_tree.Text = "树型显示";
			this.btn_tree.Click += this.Add_button_Click;
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
			this.btn_import.Image = (Image)componentResourceManager.GetObject("btn_import.Image");
			this.btn_import.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_import.ImageTransparentColor = Color.Magenta;
			this.btn_import.Name = "btn_import";
			this.btn_import.Size = new Size(69, 35);
			this.btn_import.Text = "导入";
			this.btn_import.Click += this.btn_import_Click;
			this.btn_export.Image = (Image)componentResourceManager.GetObject("btn_export.Image");
			this.btn_export.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_export.ImageTransparentColor = Color.Magenta;
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(69, 35);
			this.btn_export.Text = "导出";
			this.btn_export.Click += this.btn_export_Click;
			this.btn_log.Image = (Image)componentResourceManager.GetObject("btn_log.Image");
			this.btn_log.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_log.ImageTransparentColor = Color.Magenta;
			this.btn_log.Name = "btn_log";
			this.btn_log.Size = new Size(95, 35);
			this.btn_log.Text = "日志记录";
			this.btn_log.Click += this.btn_log_Click;
			this.pnl_dept.CanvasColor = SystemColors.Control;
			this.pnl_dept.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_dept.Dock = DockStyle.Top;
			this.pnl_dept.Location = new Point(0, 38);
			this.pnl_dept.Name = "pnl_dept";
			this.pnl_dept.Size = new Size(684, 23);
			this.pnl_dept.Style.Alignment = StringAlignment.Center;
			this.pnl_dept.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_dept.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_dept.Style.Border = eBorderType.SingleLine;
			this.pnl_dept.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_dept.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_dept.Style.GradientAngle = 90;
			this.pnl_dept.TabIndex = 17;
			this.pnl_dept.Text = "部门";
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 61);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_deptView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 394);
			this.grd_view.TabIndex = 18;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_deptView
			});
			this.grd_deptView.Columns.AddRange(new GridColumn[4]
			{
				this.column_check,
				this.column_code,
				this.column_deptname,
				this.column_parentName
			});
			this.grd_deptView.GridControl = this.grd_view;
			this.grd_deptView.Name = "grd_deptView";
			this.grd_deptView.OptionsBehavior.Editable = false;
			this.grd_deptView.OptionsView.ShowGroupPanel = false;
			this.grd_deptView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_deptView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_deptView.Click += this.grd_mainView_Click;
			this.grd_deptView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 57;
			this.column_code.Caption = "部门编号";
			this.column_code.Name = "column_code";
			this.column_code.Visible = true;
			this.column_code.VisibleIndex = 1;
			this.column_code.Width = 201;
			this.column_deptname.Caption = "部门名称";
			this.column_deptname.Name = "column_deptname";
			this.column_deptname.Visible = true;
			this.column_deptname.VisibleIndex = 2;
			this.column_deptname.Width = 201;
			this.column_parentName.Caption = "上级部门";
			this.column_parentName.Name = "column_parentName";
			this.column_parentName.Visible = true;
			this.column_parentName.VisibleIndex = 3;
			this.column_parentName.Width = 207;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			this.ContextMenuStrip = this.contextMenuStrip1;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_dept);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "DepartmentForm";
			base.Size = new Size(684, 455);
			base.Load += this.DepartmentForm_Load;
			((ISupportInitialize)this.dEPARTMENTSBindingSource).EndInit();
			((ISupportInitialize)this.zKaccessDataSetBindingSource).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_deptView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
