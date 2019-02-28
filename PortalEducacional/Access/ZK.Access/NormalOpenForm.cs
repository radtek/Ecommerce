/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.Access.door;
using ZK.Access.personnel;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class NormalOpenForm : Office2007Form
	{
		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private DataTable m_datatable = new DataTable();

		private DataTable m_browdatatable = new DataTable();

		private AccDoorBll mbll = new AccDoorBll(MainForm._ia);

		private Dictionary<int, AccDoor> mlist = new Dictionary<int, AccDoor>();

		private Dictionary<int, AccTimeseg> tlist = new Dictionary<int, AccTimeseg>();

		private AccFirstOpenBll bll = new AccFirstOpenBll(MainForm._ia);

		private int selectid = -1;

		private WaitForm m_wait = WaitForm.Instance;

		private bool isDouble = false;

		private IContainer components = null;

		private ButtonX btn_add;

		private ButtonX btn_edit;

		private ButtonX btn_delete;

		private ButtonX btn_addPerson;

		private ButtonX btn_deletePerson;

		private ContextMenuStrip contextMenuStrip1;

		private ContextMenuStrip contextMenuStrip2;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		private ToolStripMenuItem menu_delall;

		private ToolStripMenuItem menu_adduer;

		private ToolStripMenuItem menu_delex;

		private ToolStripMenuItem menu_delallex;

		private ButtonX btn_cancel;

		private GridControl grd_view;

		private GridControl grd_brow;

		private GridView grd_browview;

		private GridColumn column_no;

		private GridColumn column_name;

		private GridColumn column_cardno;

		private BandedGridView bandedGridView1;

		private GridBand gridBand1;

		private BandedGridColumn bandedGridColumn1;

		private GridView grd_mainView;

		private GridColumn column_dev;

		private GridColumn column_interLock;

		private LabelX lb_normalOpenSet;

		private LabelX lb_brow;

		private GridColumn column_check;

		private GridColumn column_checkuser;

		private GridColumn column_lastName;

		private GridColumn column_gender;

		private GridColumn column_dept;

		public NormalOpenForm()
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
			}
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				DevExpressHelper.InitImageList(this.grd_browview, "column_checkuser");
				this.GenderType();
				this.InitDataTableSet();
				this.InitBrowDataTableSet();
				this.InitMachines();
				this.InitTimeseg();
				this.LoadData();
				initLang.LocaleForm(this, base.Name);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void GenderType()
		{
			try
			{
				this.m_gender = initLang.GetComboxInfo("gender");
				if (this.m_gender == null || this.m_gender.Count == 0)
				{
					this.m_gender = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("M", "男");
					dictionary.Add("F", "女");
					this.m_gender.Add("0", dictionary);
					initLang.SetComboxInfo("gender", this.m_gender);
					initLang.Save();
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
			this.m_datatable.Columns.Add("devicename");
			this.m_datatable.Columns.Add("InterlockType");
			this.m_datatable.Columns.Add("check");
			this.column_dev.FieldName = "devicename";
			this.column_interLock.FieldName = "InterlockType";
			this.column_check.FieldName = "check";
		}

		private void InitBrowDataTableSet()
		{
			this.m_browdatatable.Columns.Add("id");
			this.m_browdatatable.Columns.Add("no").DataType = typeof(int);
			this.m_browdatatable.Columns.Add("name");
			this.m_browdatatable.Columns.Add("lastname");
			this.m_browdatatable.Columns.Add("Gender");
			this.m_browdatatable.Columns.Add("cardno");
			this.m_browdatatable.Columns.Add("DEPTNAME");
			this.m_browdatatable.Columns.Add("fid");
			this.m_browdatatable.Columns.Add("check");
			this.column_cardno.FieldName = "cardno";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_gender.FieldName = "Gender";
			this.column_no.FieldName = "no";
			this.column_dept.FieldName = "DEPTNAME";
			this.column_checkuser.FieldName = "check";
			this.grd_brow.DataSource = this.m_browdatatable;
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				List<AccDoor> list = null;
				list = this.mbll.GetModelList("");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.mlist.ContainsKey(list[i].id))
						{
							this.mlist.Add(list[i].id, list[i]);
						}
					}
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
						if (this.mlist.ContainsKey(modelList[i].door_id))
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = modelList[i].id;
							dataRow[1] = this.mlist[modelList[i].door_id].door_name;
							if (this.tlist.ContainsKey(modelList[i].timeseg_id))
							{
								dataRow[2] = this.tlist[modelList[i].timeseg_id].timeseg_name;
							}
							else
							{
								dataRow[2] = modelList[i].timeseg_id;
							}
							dataRow[3] = false;
							this.m_datatable.Rows.Add(dataRow);
						}
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_delete.Enabled = true;
					this.btn_edit.Enabled = true;
					this.btn_addPerson.Enabled = true;
				}
				else
				{
					this.btn_delete.Enabled = false;
					this.btn_edit.Enabled = false;
					this.btn_addPerson.Enabled = false;
				}
				this.SelectionChanged();
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
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					FirstOpenEidt firstOpenEidt = new FirstOpenEidt(0);
					firstOpenEidt.RefreshDataEvent += this.fopen_RefreshDataEvent;
					firstOpenEidt.ShowDialog();
					firstOpenEidt.RefreshDataEvent -= this.fopen_RefreshDataEvent;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNodoors", "没有门"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void fopen_RefreshDataEvent(object sender, EventArgs e)
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
							FirstOpenEidt firstOpenEidt = new FirstOpenEidt(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							firstOpenEidt.RefreshDataEvent += this.fopen_RefreshDataEvent;
							firstOpenEidt.ShowDialog();
							firstOpenEidt.RefreshDataEvent -= this.fopen_RefreshDataEvent;
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
							this.m_wait.ShowEx();
							bool flag = false;
							for (int i = 0; i < checkedRows.Length; i++)
							{
								this.m_wait.ShowProgress(100 * i / checkedRows.Length);
								if (checkedRows[i] < 0 || checkedRows[i] >= this.m_datatable.Rows.Count)
								{
									break;
								}
								AccFirstOpen model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									CommandServer.DelCmd(model);
									this.bll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
									flag = true;
								}
							}
							if (flag)
							{
								this.LoadData();
							}
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
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

		private void btn_addPerson_Click(object sender, EventArgs e)
		{
			try
			{
				this.selectid = -1;
				if (this.m_datatable.Rows.Count > 0)
				{
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						if (checkedRows.Length == 1)
						{
							this.selectid = int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString());
							SelectedMultiPersonnelForm selectedMultiPersonnelForm = new SelectedMultiPersonnelForm(" USERINFO.USERID not in(select employee_id from acc_firstopen_emp where accfirstopen_id=" + this.selectid + ") ");
							selectedMultiPersonnelForm.SelectUserEvent += this.frmSelectUser_SelectUserEvent;
							selectedMultiPersonnelForm.Text = this.btn_addPerson.Text;
							selectedMultiPersonnelForm.ShowDialog();
							selectedMultiPersonnelForm.SelectUserEvent -= this.frmSelectUser_SelectUserEvent;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
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

		private void frmSelectUser_SelectUserEvent(List<UserInfo> list)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				if (this.selectid > 0 && list != null && list.Count > 0)
				{
					this.m_wait.ShowEx();
					AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
					List<AccFirstOpenEmp> list2 = new List<AccFirstOpenEmp>();
					for (int i = 0; i < list.Count; i++)
					{
						AccFirstOpenEmp accFirstOpenEmp = new AccFirstOpenEmp();
						accFirstOpenEmp.employee_id = list[i].UserId;
						accFirstOpenEmp.accfirstopen_id = this.selectid;
						list2.Add(accFirstOpenEmp);
						this.m_wait.ShowProgress(50 * i / list.Count);
					}
					this.m_wait.ShowProgress(50);
					accFirstOpenEmpBll.Add(list2);
					this.m_wait.ShowProgress(55);
					AccFirstOpen model = this.bll.GetModel(this.selectid);
					if (model != null)
					{
						CommandServer.AddCmd(model, list2);
					}
					this.m_wait.ShowProgress(90);
					this.SelectionChanged();
					this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
				}
				this.Cursor = Cursors.Default;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_deletePerson_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				if (this.m_browdatatable.Rows.Count > 0)
				{
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_browview, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_browdatatable.Rows.Count)
					{
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							this.m_wait.ShowEx();
							AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
							int num = int.Parse(this.m_browdatatable.Rows[checkedRows[0]][7].ToString());
							List<int> list = new List<int>();
							if (checkedRows.Length < 10)
							{
								for (int i = 0; i < checkedRows.Length; i++)
								{
									this.m_wait.ShowProgress(100 * i / this.m_datatable.Rows.Count);
									if (checkedRows[i] < 0 || checkedRows[i] >= this.m_browdatatable.Rows.Count)
									{
										break;
									}
									int num2 = int.Parse(this.m_browdatatable.Rows[checkedRows[i]][0].ToString());
									list.Add(num2);
									accFirstOpenEmpBll.DeleteByUserAndOpenID(num2, num);
								}
							}
							else
							{
								StringBuilder stringBuilder = new StringBuilder();
								stringBuilder.Append(this.m_browdatatable.Rows[checkedRows[0]][0].ToString());
								int item = int.Parse(this.m_browdatatable.Rows[checkedRows[0]][0].ToString());
								list.Add(item);
								for (int j = 1; j < checkedRows.Length; j++)
								{
									this.m_wait.ShowProgress(100 * j / this.m_datatable.Rows.Count);
									if (checkedRows[j] < 0 || checkedRows[j] >= this.m_browdatatable.Rows.Count)
									{
										break;
									}
									stringBuilder.Append("," + this.m_browdatatable.Rows[checkedRows[j]][0].ToString());
									item = int.Parse(this.m_browdatatable.Rows[checkedRows[j]][0].ToString());
									list.Add(item);
									if (j % 1000 == 0)
									{
										accFirstOpenEmpBll.DeleteByOpenIDAndUserList(num, stringBuilder.ToString());
										stringBuilder = new StringBuilder();
										if (checkedRows.Length > j + 1)
										{
											stringBuilder.Append(this.m_browdatatable.Rows[checkedRows[j + 1]][0].ToString());
										}
									}
								}
								if (!string.IsNullOrEmpty(stringBuilder.ToString()))
								{
									accFirstOpenEmpBll.DeleteByOpenIDAndUserList(num, stringBuilder.ToString());
								}
							}
							AccFirstOpen model = this.bll.GetModel(num);
							CommandServer.DelCmd(model, list);
							this.SelectionChanged();
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
							this.m_wait.ShowProgress(100);
							this.m_wait.HideEx(false);
							FrmShowUpdata.Instance.ShowEx();
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
				this.Cursor = Cursors.Default;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
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
						for (int i = 0; i < this.m_datatable.Rows.Count; i++)
						{
							this.m_wait.ShowProgress(100 * i / this.m_datatable.Rows.Count);
							if (this.m_datatable.Rows[i][0] == null)
							{
								break;
							}
							AccFirstOpen model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							if (model != null)
							{
								CommandServer.DelCmd(model);
								this.bll.Delete(model.id);
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								CommandServer.SetDoorParam(model.door_id);
							}
						}
						this.m_datatable.Rows.Clear();
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
				this.Cursor = Cursors.Default;
			}
		}

		private void menu_delallex_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browdatatable.Rows.Count > 0)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						int num = int.Parse(this.m_browdatatable.Rows[0][5].ToString());
						AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
						this.m_wait.ShowProgress(10);
						accFirstOpenEmpBll.DeleteByOpenID(num);
						this.m_wait.ShowProgress(30);
						AccFirstOpen model = this.bll.GetModel(num);
						this.m_wait.ShowProgress(50);
						if (model != null)
						{
							CommandServer.DelCmd(model);
						}
						this.m_wait.ShowProgress(80);
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						this.m_browdatatable.Rows.Clear();
						this.column_checkuser.ImageIndex = 1;
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
				this.Cursor = Cursors.Default;
			}
		}

		private void SelectionChanged()
		{
			try
			{
				this.m_browdatatable.Rows.Clear();
				string value = string.Empty;
				string value2 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					value = dictionary["m"];
					value2 = dictionary["f"];
				}
				this.btn_deletePerson.Enabled = false;
				int[] selectedRows = this.grd_mainView.GetSelectedRows();
				selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.grd_mainView, selectedRows);
				if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < this.m_datatable.Rows.Count)
				{
					int num = int.Parse(this.m_datatable.Rows[selectedRows[0]][0].ToString());
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					List<UserInfo> modelList = userInfoBll.GetModelList(" USERINFO.USERID in(select employee_id from acc_firstopen_emp where accfirstopen_id=" + num + ") ");
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							DataRow dataRow = this.m_browdatatable.NewRow();
							dataRow[0] = modelList[i].UserId.ToString();
							dataRow[1] = modelList[i].BadgeNumber;
							dataRow[2] = modelList[i].Name;
							dataRow[3] = modelList[i].LastName;
							if (!string.IsNullOrEmpty(modelList[i].Gender) && modelList[i].Gender == "F")
							{
								dataRow[4] = value2;
							}
							else
							{
								dataRow[4] = value;
							}
							dataRow[5] = modelList[i].CardNo;
							dataRow[6] = modelList[i].DeptName;
							dataRow[7] = num;
							dataRow[8] = false;
							this.m_browdatatable.Rows.Add(dataRow);
						}
						this.btn_deletePerson.Enabled = true;
					}
				}
				this.column_checkuser.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
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
			this.SelectionChanged();
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

		private void grd_browview_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NormalOpenForm));
			this.btn_add = new ButtonX();
			this.btn_edit = new ButtonX();
			this.btn_delete = new ButtonX();
			this.btn_addPerson = new ButtonX();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.menu_delall = new ToolStripMenuItem();
			this.menu_adduer = new ToolStripMenuItem();
			this.btn_deletePerson = new ButtonX();
			this.contextMenuStrip2 = new ContextMenuStrip(this.components);
			this.menu_delex = new ToolStripMenuItem();
			this.menu_delallex = new ToolStripMenuItem();
			this.btn_cancel = new ButtonX();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_interLock = new GridColumn();
			this.bandedGridView1 = new BandedGridView();
			this.grd_brow = new GridControl();
			this.grd_browview = new GridView();
			this.column_checkuser = new GridColumn();
			this.column_no = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_gender = new GridColumn();
			this.column_cardno = new GridColumn();
			this.column_dept = new GridColumn();
			this.gridBand1 = new GridBand();
			this.bandedGridColumn1 = new BandedGridColumn();
			this.lb_normalOpenSet = new LabelX();
			this.lb_brow = new LabelX();
			this.contextMenuStrip1.SuspendLayout();
			this.contextMenuStrip2.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			((ISupportInitialize)this.bandedGridView1).BeginInit();
			((ISupportInitialize)this.grd_brow).BeginInit();
			((ISupportInitialize)this.grd_browview).BeginInit();
			base.SuspendLayout();
			this.btn_add.AccessibleRole = AccessibleRole.PushButton;
			this.btn_add.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_add.Location = new Point(15, 470);
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(90, 23);
			this.btn_add.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_add.TabIndex = 0;
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_edit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_edit.Location = new Point(121, 470);
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(90, 23);
			this.btn_edit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_edit.TabIndex = 1;
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.AccessibleRole = AccessibleRole.PushButton;
			this.btn_delete.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_delete.Location = new Point(227, 471);
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(90, 23);
			this.btn_delete.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_delete.TabIndex = 2;
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.btn_addPerson.AccessibleRole = AccessibleRole.PushButton;
			this.btn_addPerson.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_addPerson.Location = new Point(333, 471);
			this.btn_addPerson.Name = "btn_addPerson";
			this.btn_addPerson.Size = new Size(188, 23);
			this.btn_addPerson.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_addPerson.TabIndex = 3;
			this.btn_addPerson.Text = "添加开门人员";
			this.btn_addPerson.Click += this.btn_addPerson_Click;
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[5]
			{
				this.menu_add,
				this.menu_edit,
				this.menu_del,
				this.menu_delall,
				this.menu_adduer
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(153, 136);
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(152, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.btn_add_Click;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(152, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(152, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
			this.menu_delall.Name = "menu_delall";
			this.menu_delall.Size = new Size(152, 22);
			this.menu_delall.Text = "删除全部";
			this.menu_delall.Visible = false;
			this.menu_delall.Click += this.menu_delall_Click;
			this.menu_adduer.Name = "menu_adduer";
			this.menu_adduer.Size = new Size(152, 22);
			this.menu_adduer.Text = "添加开门人员";
			this.menu_adduer.Click += this.btn_addPerson_Click;
			this.btn_deletePerson.AccessibleRole = AccessibleRole.PushButton;
			this.btn_deletePerson.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_deletePerson.Location = new Point(538, 471);
			this.btn_deletePerson.Name = "btn_deletePerson";
			this.btn_deletePerson.Size = new Size(188, 23);
			this.btn_deletePerson.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_deletePerson.TabIndex = 4;
			this.btn_deletePerson.Text = "删除开门人员";
			this.btn_deletePerson.Click += this.btn_deletePerson_Click;
			this.contextMenuStrip2.Items.AddRange(new ToolStripItem[2]
			{
				this.menu_delex,
				this.menu_delallex
			});
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new Size(119, 48);
			this.menu_delex.Name = "menu_delex";
			this.menu_delex.Size = new Size(118, 22);
			this.menu_delex.Text = "删除";
			this.menu_delex.Click += this.btn_deletePerson_Click;
			this.menu_delallex.Name = "menu_delallex";
			this.menu_delallex.Size = new Size(118, 22);
			this.menu_delallex.Text = "删除全部";
			this.menu_delallex.Visible = false;
			this.menu_delallex.Click += this.menu_delallex_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(944, 471);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(100, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.grd_view.Location = new Point(15, 42);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(506, 415);
			this.grd_view.TabIndex = 50;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[2]
			{
				this.grd_mainView,
				this.bandedGridView1
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
			this.grd_mainView.OptionsSelection.MultiSelect = true;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.SummaryItem.SummaryType = SummaryItemType.Count;
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 40;
			this.column_dev.Caption = "门";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 128;
			this.column_interLock.Caption = "门禁时间段";
			this.column_interLock.Name = "column_interLock";
			this.column_interLock.Visible = true;
			this.column_interLock.VisibleIndex = 2;
			this.column_interLock.Width = 131;
			this.bandedGridView1.GridControl = this.grd_view;
			this.bandedGridView1.Name = "bandedGridView1";
			this.bandedGridView1.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			this.grd_brow.Location = new Point(538, 42);
			this.grd_brow.MainView = this.grd_browview;
			this.grd_brow.Name = "grd_brow";
			this.grd_brow.Size = new Size(506, 415);
			this.grd_brow.TabIndex = 60;
			this.grd_brow.TabStop = false;
			this.grd_brow.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_browview
			});
			this.grd_browview.Columns.AddRange(new GridColumn[7]
			{
				this.column_checkuser,
				this.column_no,
				this.column_name,
				this.column_lastName,
				this.column_gender,
				this.column_cardno,
				this.column_dept
			});
			this.grd_browview.GridControl = this.grd_brow;
			this.grd_browview.IndicatorWidth = 35;
			this.grd_browview.Name = "grd_browview";
			this.grd_browview.PaintStyleName = "Office2003";
			this.grd_browview.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_browview.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_browview.Click += this.grd_browview_Click;
			this.column_checkuser.Name = "column_checkuser";
			this.column_checkuser.Visible = true;
			this.column_checkuser.VisibleIndex = 0;
			this.column_checkuser.Width = 40;
			this.column_no.AppearanceCell.Options.UseTextOptions = true;
			this.column_no.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			this.column_no.Caption = "人员编号";
			this.column_no.Name = "column_no";
			this.column_no.SummaryItem.SummaryType = SummaryItemType.Count;
			this.column_no.Visible = true;
			this.column_no.VisibleIndex = 1;
			this.column_no.Width = 83;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 2;
			this.column_name.Width = 83;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_gender.Caption = "性别";
			this.column_gender.Name = "column_gender";
			this.column_gender.Visible = true;
			this.column_gender.VisibleIndex = 4;
			this.column_cardno.Caption = "卡号";
			this.column_cardno.Name = "column_cardno";
			this.column_cardno.Visible = true;
			this.column_cardno.VisibleIndex = 5;
			this.column_cardno.Width = 89;
			this.column_dept.Caption = "部门名称";
			this.column_dept.Name = "column_dept";
			this.column_dept.Visible = true;
			this.column_dept.VisibleIndex = 6;
			this.gridBand1.Name = "gridBand1";
			this.bandedGridColumn1.Name = "bandedGridColumn1";
			this.lb_normalOpenSet.BackgroundStyle.Class = "";
			this.lb_normalOpenSet.Location = new Point(15, 19);
			this.lb_normalOpenSet.Name = "lb_normalOpenSet";
			this.lb_normalOpenSet.Size = new Size(386, 18);
			this.lb_normalOpenSet.TabIndex = 51;
			this.lb_normalOpenSet.Text = "首卡常开设置";
			this.lb_brow.BackgroundStyle.Class = "";
			this.lb_brow.Location = new Point(538, 21);
			this.lb_brow.Name = "lb_brow";
			this.lb_brow.Size = new Size(405, 18);
			this.lb_brow.TabIndex = 61;
			this.lb_brow.Text = "浏览开门人员";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1056, 506);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.lb_normalOpenSet);
			base.Controls.Add(this.lb_brow);
			base.Controls.Add(this.grd_brow);
			base.Controls.Add(this.btn_edit);
			base.Controls.Add(this.btn_add);
			base.Controls.Add(this.btn_delete);
			base.Controls.Add(this.btn_addPerson);
			base.Controls.Add(this.btn_deletePerson);
			base.Controls.Add(this.btn_cancel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NormalOpenForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "设置";
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip2.ResumeLayout(false);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			((ISupportInitialize)this.bandedGridView1).EndInit();
			((ISupportInitialize)this.grd_brow).EndInit();
			((ISupportInitialize)this.grd_browview).EndInit();
			base.ResumeLayout(false);
		}
	}
}
