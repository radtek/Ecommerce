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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.personnel
{
	public class SelectedMultiPersonnelForm : Office2007Form
	{
		public delegate void SelectUserHandle(List<UserInfo> list);

		private string m_strWhere = string.Empty;

		private ImagesForm imgForm = new ImagesForm();

		private DataTable m_datatable = new DataTable();

		private DataTable m_browdatatable = new DataTable();

		private static SelectedMultiPersonnelForm m_instance = null;

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private bool IsLoadingData = false;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private ButtonX btn_allUserRMove;

		private ButtonX btn_userRMove;

		private ButtonX btn_userLMove;

		private ButtonX btn_allUserLMove;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_no;

		private GridColumn column_name;

		private GridColumn column_dept;

		private GridColumn column_cardno;

		private GridColumn column_check;

		private GridControl grd_brow;

		private GridView grd_browview;

		private GridColumn column_checkuser;

		private GridColumn column_noex;

		private GridColumn column_nameex;

		private GridColumn column_deptex;

		private GridColumn column_cardnoex;

		private GridView gridView1;

		private Label lb_users;

		private Label label1;

		private GridColumn column_lastName;

		private GridColumn column_lastNameex;

		private GridColumn column_gender;

		private GridColumn column_genderx;

		public event SelectUserHandle SelectUserEvent;

		public SelectedMultiPersonnelForm(string strWhere)
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
				this.column_lastNameex.Visible = false;
			}
			try
			{
				this.m_strWhere = strWhere;
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				DevExpressHelper.InitImageList(this.grd_browview, "column_checkuser");
				this.GenderType();
				this.InitDataTableSet();
				this.InitBrowDataTableSet();
				this.DataBind();
				SelectedMultiPersonnelForm.m_instance = this;
				initLang.LocaleForm(this, base.Name);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("dataFailedToLoad", "数据加载失败"));
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("no").DataType = typeof(int);
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("lastname");
			this.m_datatable.Columns.Add("Gender");
			this.m_datatable.Columns.Add("dept");
			this.m_datatable.Columns.Add("cardno");
			this.m_datatable.Columns.Add("check");
			this.column_no.FieldName = "no";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_gender.FieldName = "Gender";
			this.column_dept.FieldName = "dept";
			this.column_cardno.FieldName = "cardno";
			this.column_check.FieldName = "check";
			this.grd_view.DataSource = this.m_datatable;
		}

		private void InitBrowDataTableSet()
		{
			this.m_browdatatable.Columns.Add("id");
			this.m_browdatatable.Columns.Add("no").DataType = typeof(int);
			this.m_browdatatable.Columns.Add("name");
			this.m_browdatatable.Columns.Add("lastname");
			this.m_browdatatable.Columns.Add("Gender");
			this.m_browdatatable.Columns.Add("dept");
			this.m_browdatatable.Columns.Add("cardno");
			this.m_browdatatable.Columns.Add("check");
			this.column_noex.FieldName = "no";
			this.column_nameex.FieldName = "name";
			this.column_lastNameex.FieldName = "lastname";
			this.column_genderx.FieldName = "Gender";
			this.column_deptex.FieldName = "dept";
			this.column_cardnoex.FieldName = "cardno";
			this.column_checkuser.FieldName = "check";
			this.grd_brow.DataSource = this.m_browdatatable;
		}

		private void GenderType()
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

		private void DataBind()
		{
			this.imgForm.TopMost = true;
			this.imgForm.Show();
			Thread thread = new Thread(this.LoadDataInThread);
			thread.Start();
		}

		private void LoadDataInThread()
		{
			this.IsLoadingData = true;
			try
			{
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.CheckBtn();
						this.grd_view.DataSource = null;
					});
				}
				else
				{
					this.CheckBtn();
					this.grd_view.DataSource = null;
				}
				this.m_datatable.Rows.Clear();
				string value = string.Empty;
				string value2 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					value = dictionary["m"];
					value2 = dictionary["f"];
				}
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				DataSet dataSet = null;
				dataSet = ((!string.IsNullOrEmpty(this.m_strWhere)) ? userInfoBll.GetList(this.m_strWhere) : userInfoBll.GetAllList());
				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = dataTable.Rows[i]["userId"].ToString();
						dataRow[1] = dataTable.Rows[i]["Badgenumber"].ToString();
						dataRow[2] = dataTable.Rows[i]["Name"].ToString();
						dataRow[3] = dataTable.Rows[i]["lastname"].ToString();
						if (dataTable.Rows[i]["Gender"].ToString() == "F")
						{
							dataRow[4] = value2;
						}
						else
						{
							dataRow[4] = value;
						}
						dataRow[5] = dataTable.Rows[i]["DEPTNAME"].ToString();
						dataRow[6] = dataTable.Rows[i]["CardNo"].ToString();
						dataRow[7] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			if (this != null && !base.Disposing && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.grd_view.DataSource = this.m_datatable;
						this.CheckBtn();
						this.column_check.ImageIndex = 1;
					});
				}
				else
				{
					this.grd_view.DataSource = this.m_datatable;
					this.CheckBtn();
					this.column_check.ImageIndex = 1;
				}
				if (this.imgForm.InvokeRequired)
				{
					this.imgForm.Invoke((MethodInvoker)delegate
					{
						this.imgForm.Hide();
					});
				}
				else
				{
					this.imgForm.Hide();
				}
				this.IsLoadingData = false;
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				if (this.m_browdatatable.Rows.Count > 0)
				{
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					List<UserInfo> list = new List<UserInfo>();
					for (int i = 0; i < this.m_browdatatable.Rows.Count; i++)
					{
						if (this.m_browdatatable.Rows[i][0] != null)
						{
							int userId = int.Parse(this.m_browdatatable.Rows[i][0].ToString());
							UserInfo userInfo = new UserInfo();
							userInfo.UserId = userId;
							userInfo.BadgeNumber = this.m_browdatatable.Rows[i][1].ToString();
							userInfo.Name = this.m_browdatatable.Rows[i][2].ToString();
							userInfo.LastName = this.m_browdatatable.Rows[i][3].ToString();
							userInfo.Gender = this.m_browdatatable.Rows[i][4].ToString();
							userInfo.CardNo = this.m_browdatatable.Rows[i][6].ToString();
							userInfo.DeptName = this.m_browdatatable.Rows[i][5].ToString();
							list.Add(userInfo);
						}
					}
					if (this.SelectUserEvent != null)
					{
						this.SelectUserEvent(list);
					}
					FrmShowUpdata.Instance.ShowEx();
					this.Cursor = Cursors.Default;
					base.Close();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
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

		private void CheckBtn()
		{
			if (this.m_datatable.Rows.Count > 0)
			{
				this.btn_allUserRMove.Enabled = true;
				this.btn_userRMove.Enabled = true;
			}
			else
			{
				this.btn_allUserRMove.Enabled = false;
				this.btn_userRMove.Enabled = false;
			}
			if (this.m_browdatatable.Rows.Count > 0)
			{
				this.btn_userLMove.Enabled = true;
				this.btn_allUserLMove.Enabled = true;
				this.btn_OK.Enabled = true;
			}
			else
			{
				this.btn_userLMove.Enabled = false;
				this.btn_allUserLMove.Enabled = false;
				this.btn_OK.Enabled = false;
			}
		}

		private void btn_allUserRMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 0)
				{
					for (int i = 0; i < this.m_datatable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_browdatatable.NewRow();
						dataRow[0] = this.m_datatable.Rows[i][0].ToString();
						dataRow[1] = this.m_datatable.Rows[i][1].ToString();
						dataRow[2] = this.m_datatable.Rows[i][2].ToString();
						dataRow[3] = this.m_datatable.Rows[i][3].ToString();
						dataRow[4] = this.m_datatable.Rows[i][4].ToString();
						dataRow[5] = this.m_datatable.Rows[i][5].ToString();
						dataRow[6] = this.m_datatable.Rows[i][6].ToString();
						dataRow[7] = false;
						this.m_browdatatable.Rows.Add(dataRow);
					}
					this.m_datatable.Rows.Clear();
					this.column_checkuser.ImageIndex = 1;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_userRMove_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							DataRow dataRow = this.m_browdatatable.NewRow();
							dataRow[0] = this.m_datatable.Rows[checkedRows[i]][0].ToString();
							dataRow[1] = this.m_datatable.Rows[checkedRows[i]][1].ToString();
							dataRow[2] = this.m_datatable.Rows[checkedRows[i]][2].ToString();
							dataRow[3] = this.m_datatable.Rows[checkedRows[i]][3].ToString();
							dataRow[4] = this.m_datatable.Rows[checkedRows[i]][4].ToString();
							dataRow[5] = this.m_datatable.Rows[checkedRows[i]][5].ToString();
							dataRow[6] = this.m_datatable.Rows[checkedRows[i]][6].ToString();
							dataRow[7] = false;
							this.m_browdatatable.Rows.Add(dataRow);
							this.m_datatable.Rows[checkedRows[i]][0] = -1;
						}
					}
					for (int j = 0; j < this.m_datatable.Rows.Count; j++)
					{
						if (this.m_datatable.Rows[j][0].ToString() == "-1")
						{
							this.m_datatable.Rows.RemoveAt(j);
							j--;
						}
					}
					if (this.m_datatable.Rows.Count > 0)
					{
						this.grd_mainView.SelectRow(0);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_userLMove_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_browview, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_browdatatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_browdatatable.Rows.Count)
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = this.m_browdatatable.Rows[checkedRows[i]][0].ToString();
							dataRow[1] = this.m_browdatatable.Rows[checkedRows[i]][1].ToString();
							dataRow[2] = this.m_browdatatable.Rows[checkedRows[i]][2].ToString();
							dataRow[3] = this.m_browdatatable.Rows[checkedRows[i]][3].ToString();
							dataRow[4] = this.m_browdatatable.Rows[checkedRows[i]][4].ToString();
							dataRow[5] = this.m_browdatatable.Rows[checkedRows[i]][5].ToString();
							dataRow[6] = this.m_browdatatable.Rows[checkedRows[i]][6].ToString();
							dataRow[7] = false;
							this.m_datatable.Rows.Add(dataRow);
							this.m_browdatatable.Rows[checkedRows[i]][0] = -1;
						}
					}
					for (int j = 0; j < this.m_browdatatable.Rows.Count; j++)
					{
						if (this.m_browdatatable.Rows[j][0].ToString() == "-1")
						{
							this.m_browdatatable.Rows.RemoveAt(j);
							j--;
						}
					}
					if (this.m_browdatatable.Rows.Count > 0)
					{
						this.grd_browview.SelectRow(0);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_allUserLMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browdatatable.Rows.Count > 0)
				{
					for (int i = 0; i < this.m_browdatatable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = this.m_browdatatable.Rows[i][0].ToString();
						dataRow[1] = this.m_browdatatable.Rows[i][1].ToString();
						dataRow[2] = this.m_browdatatable.Rows[i][2].ToString();
						dataRow[3] = this.m_browdatatable.Rows[i][3].ToString();
						dataRow[4] = this.m_browdatatable.Rows[i][4].ToString();
						dataRow[5] = this.m_browdatatable.Rows[i][5].ToString();
						dataRow[6] = this.m_browdatatable.Rows[i][6].ToString();
						dataRow[7] = false;
						this.m_datatable.Rows.Add(dataRow);
					}
					this.m_browdatatable.Rows.Clear();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
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

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				int[] selectedRows = this.grd_mainView.GetSelectedRows();
				selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.grd_mainView, selectedRows);
				if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < selectedRows.Length; i++)
					{
						if (selectedRows[i] >= 0 && selectedRows[i] < this.m_datatable.Rows.Count)
						{
							DataRow dataRow = this.m_browdatatable.NewRow();
							dataRow[0] = this.m_datatable.Rows[selectedRows[i]][0].ToString();
							dataRow[1] = this.m_datatable.Rows[selectedRows[i]][1].ToString();
							dataRow[2] = this.m_datatable.Rows[selectedRows[i]][2].ToString();
							dataRow[3] = this.m_datatable.Rows[selectedRows[i]][3].ToString();
							dataRow[4] = this.m_datatable.Rows[selectedRows[i]][4].ToString();
							dataRow[5] = this.m_datatable.Rows[selectedRows[i]][5].ToString();
							dataRow[6] = this.m_datatable.Rows[selectedRows[i]][6].ToString();
							dataRow[7] = false;
							this.m_browdatatable.Rows.Add(dataRow);
							this.m_datatable.Rows[selectedRows[i]][0] = -1;
						}
					}
					for (int j = 0; j < this.m_datatable.Rows.Count; j++)
					{
						if (this.m_datatable.Rows[j][0].ToString() == "-1")
						{
							this.m_datatable.Rows.RemoveAt(j);
							j--;
						}
					}
					if (this.m_datatable.Rows.Count > 0)
					{
						this.grd_mainView.SelectRow(0);
					}
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void grd_browview_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				int[] selectedRows = this.grd_browview.GetSelectedRows();
				selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.grd_browview, selectedRows);
				if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < this.m_browdatatable.Rows.Count)
				{
					for (int i = 0; i < selectedRows.Length; i++)
					{
						if (selectedRows[i] >= 0 && selectedRows[i] < this.m_browdatatable.Rows.Count)
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = this.m_browdatatable.Rows[selectedRows[i]][0].ToString();
							dataRow[1] = this.m_browdatatable.Rows[selectedRows[i]][1].ToString();
							dataRow[2] = this.m_browdatatable.Rows[selectedRows[i]][2].ToString();
							dataRow[3] = this.m_browdatatable.Rows[selectedRows[i]][3].ToString();
							dataRow[4] = this.m_browdatatable.Rows[selectedRows[i]][4].ToString();
							dataRow[5] = this.m_browdatatable.Rows[selectedRows[i]][5].ToString();
							dataRow[6] = this.m_browdatatable.Rows[selectedRows[i]][6].ToString();
							dataRow[7] = false;
							this.m_datatable.Rows.Add(dataRow);
							this.m_browdatatable.Rows[selectedRows[i]][0] = -1;
						}
					}
					for (int j = 0; j < this.m_browdatatable.Rows.Count; j++)
					{
						if (this.m_browdatatable.Rows[j][0].ToString() == "-1")
						{
							this.m_browdatatable.Rows.RemoveAt(j);
							j--;
						}
					}
					if (this.m_browdatatable.Rows.Count > 0)
					{
						this.grd_browview.SelectRow(0);
					}
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SelectedMultiPersonnelForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.IsLoadingData)
			{
				e.Cancel = true;
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("LoadingData", "正在加载数据,请稍候"));
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
            this.btn_cancel = new DevComponents.DotNetBar.ButtonX();
            this.btn_OK = new DevComponents.DotNetBar.ButtonX();
            this.btn_allUserRMove = new DevComponents.DotNetBar.ButtonX();
            this.btn_userRMove = new DevComponents.DotNetBar.ButtonX();
            this.btn_userLMove = new DevComponents.DotNetBar.ButtonX();
            this.btn_allUserLMove = new DevComponents.DotNetBar.ButtonX();
            this.grd_view = new DevExpress.XtraGrid.GridControl();
            this.grd_mainView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.column_check = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_no = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_lastName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_gender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_dept = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_cardno = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grd_brow = new DevExpress.XtraGrid.GridControl();
            this.grd_browview = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.column_checkuser = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_noex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_nameex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_lastNameex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_genderx = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_deptex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.column_cardnoex = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lb_users = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grd_view)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_mainView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_brow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_browview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_cancel
            // 
            this.btn_cancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_cancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_cancel.Location = new System.Drawing.Point(1317, 561);
            this.btn_cancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(109, 31);
            this.btn_cancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_cancel.TabIndex = 5;
            this.btn_cancel.Text = "取消";
            // 
            // btn_OK
            // 
            this.btn_OK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_OK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_OK.Location = new System.Drawing.Point(1192, 561);
            this.btn_OK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(109, 31);
            this.btn_OK.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_OK.TabIndex = 4;
            this.btn_OK.Text = "确定";
            // 
            // btn_allUserRMove
            // 
            this.btn_allUserRMove.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_allUserRMove.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_allUserRMove.Location = new System.Drawing.Point(675, 123);
            this.btn_allUserRMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_allUserRMove.Name = "btn_allUserRMove";
            this.btn_allUserRMove.Size = new System.Drawing.Size(91, 31);
            this.btn_allUserRMove.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_allUserRMove.TabIndex = 0;
            this.btn_allUserRMove.Text = ">>";
            // 
            // btn_userRMove
            // 
            this.btn_userRMove.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_userRMove.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_userRMove.Location = new System.Drawing.Point(675, 199);
            this.btn_userRMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_userRMove.Name = "btn_userRMove";
            this.btn_userRMove.Size = new System.Drawing.Size(91, 31);
            this.btn_userRMove.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_userRMove.TabIndex = 1;
            this.btn_userRMove.Text = ">";
            // 
            // btn_userLMove
            // 
            this.btn_userLMove.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_userLMove.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_userLMove.Location = new System.Drawing.Point(675, 275);
            this.btn_userLMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_userLMove.Name = "btn_userLMove";
            this.btn_userLMove.Size = new System.Drawing.Size(91, 31);
            this.btn_userLMove.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_userLMove.TabIndex = 2;
            this.btn_userLMove.Text = "<";
            // 
            // btn_allUserLMove
            // 
            this.btn_allUserLMove.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_allUserLMove.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_allUserLMove.Location = new System.Drawing.Point(675, 351);
            this.btn_allUserLMove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_allUserLMove.Name = "btn_allUserLMove";
            this.btn_allUserLMove.Size = new System.Drawing.Size(91, 31);
            this.btn_allUserLMove.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_allUserLMove.TabIndex = 3;
            this.btn_allUserLMove.Text = "<<";
            // 
            // grd_view
            // 
            // 
            // 
            // 
            this.grd_view.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grd_view.Location = new System.Drawing.Point(19, 37);
            this.grd_view.MainView = this.grd_mainView;
            this.grd_view.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grd_view.Name = "grd_view";
            this.grd_view.Size = new System.Drawing.Size(640, 503);
            this.grd_view.TabIndex = 6;
            this.grd_view.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grd_mainView});
            // 
            // grd_mainView
            // 
            this.grd_mainView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.column_check,
            this.column_no,
            this.column_name,
            this.column_lastName,
            this.column_gender,
            this.column_dept,
            this.column_cardno});
            this.grd_mainView.GridControl = this.grd_view;
            this.grd_mainView.Name = "grd_mainView";
            this.grd_mainView.OptionsBehavior.Editable = false;
            this.grd_mainView.OptionsSelection.MultiSelect = true;
            this.grd_mainView.PaintStyleName = "Office2003";
            this.grd_mainView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.column_no, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // column_check
            // 
            this.column_check.Name = "column_check";
            this.column_check.Visible = true;
            this.column_check.VisibleIndex = 0;
            this.column_check.Width = 40;
            // 
            // column_no
            // 
            this.column_no.AppearanceCell.Options.UseTextOptions = true;
            this.column_no.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.column_no.Caption = "人员编号";
            this.column_no.Name = "column_no";
            this.column_no.Visible = true;
            this.column_no.VisibleIndex = 1;
            this.column_no.Width = 72;
            // 
            // column_name
            // 
            this.column_name.Caption = "姓名";
            this.column_name.Name = "column_name";
            this.column_name.Visible = true;
            this.column_name.VisibleIndex = 2;
            this.column_name.Width = 72;
            // 
            // column_lastName
            // 
            this.column_lastName.Caption = "姓氏";
            this.column_lastName.Name = "column_lastName";
            this.column_lastName.Visible = true;
            this.column_lastName.VisibleIndex = 3;
            // 
            // column_gender
            // 
            this.column_gender.Caption = "性别";
            this.column_gender.Name = "column_gender";
            this.column_gender.Visible = true;
            this.column_gender.VisibleIndex = 4;
            // 
            // column_dept
            // 
            this.column_dept.Caption = "部门名称";
            this.column_dept.Name = "column_dept";
            this.column_dept.Visible = true;
            this.column_dept.VisibleIndex = 6;
            this.column_dept.Width = 72;
            // 
            // column_cardno
            // 
            this.column_cardno.Caption = "卡号";
            this.column_cardno.Name = "column_cardno";
            this.column_cardno.Visible = true;
            this.column_cardno.VisibleIndex = 5;
            this.column_cardno.Width = 78;
            // 
            // grd_brow
            // 
            // 
            // 
            // 
            this.grd_brow.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grd_brow.Location = new System.Drawing.Point(787, 37);
            this.grd_brow.MainView = this.grd_browview;
            this.grd_brow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grd_brow.Name = "grd_brow";
            this.grd_brow.Size = new System.Drawing.Size(640, 503);
            this.grd_brow.TabIndex = 6;
            this.grd_brow.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grd_browview,
            this.gridView1});
            // 
            // grd_browview
            // 
            this.grd_browview.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.column_checkuser,
            this.column_noex,
            this.column_nameex,
            this.column_lastNameex,
            this.column_genderx,
            this.column_deptex,
            this.column_cardnoex});
            this.grd_browview.GridControl = this.grd_brow;
            this.grd_browview.Name = "grd_browview";
            this.grd_browview.OptionsBehavior.Editable = false;
            this.grd_browview.OptionsSelection.MultiSelect = true;
            this.grd_browview.PaintStyleName = "Office2003";
            this.grd_browview.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.column_noex, DevExpress.Data.ColumnSortOrder.Ascending)});
            // 
            // column_checkuser
            // 
            this.column_checkuser.Name = "column_checkuser";
            this.column_checkuser.Visible = true;
            this.column_checkuser.VisibleIndex = 0;
            this.column_checkuser.Width = 40;
            // 
            // column_noex
            // 
            this.column_noex.AppearanceCell.Options.UseTextOptions = true;
            this.column_noex.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.column_noex.Caption = "人员编号";
            this.column_noex.Name = "column_noex";
            this.column_noex.Visible = true;
            this.column_noex.VisibleIndex = 1;
            this.column_noex.Width = 72;
            // 
            // column_nameex
            // 
            this.column_nameex.Caption = "姓名";
            this.column_nameex.Name = "column_nameex";
            this.column_nameex.Visible = true;
            this.column_nameex.VisibleIndex = 2;
            this.column_nameex.Width = 72;
            // 
            // column_lastNameex
            // 
            this.column_lastNameex.Caption = "姓氏";
            this.column_lastNameex.Name = "column_lastNameex";
            this.column_lastNameex.Visible = true;
            this.column_lastNameex.VisibleIndex = 3;
            // 
            // column_genderx
            // 
            this.column_genderx.Caption = "性别";
            this.column_genderx.Name = "column_genderx";
            this.column_genderx.Visible = true;
            this.column_genderx.VisibleIndex = 4;
            // 
            // column_deptex
            // 
            this.column_deptex.Caption = "部门名称";
            this.column_deptex.Name = "column_deptex";
            this.column_deptex.Visible = true;
            this.column_deptex.VisibleIndex = 6;
            this.column_deptex.Width = 72;
            // 
            // column_cardnoex
            // 
            this.column_cardnoex.Caption = "卡号";
            this.column_cardnoex.Name = "column_cardnoex";
            this.column_cardnoex.Visible = true;
            this.column_cardnoex.VisibleIndex = 5;
            this.column_cardnoex.Width = 78;
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.grd_brow;
            this.gridView1.Name = "gridView1";
            // 
            // lb_users
            // 
            this.lb_users.AutoSize = true;
            this.lb_users.Location = new System.Drawing.Point(16, 17);
            this.lb_users.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_users.Name = "lb_users";
            this.lb_users.Size = new System.Drawing.Size(64, 17);
            this.lb_users.TabIndex = 22;
            this.lb_users.Text = "备选人员";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(784, 17);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 23;
            this.label1.Text = "已选人员";
            // 
            // SelectedMultiPersonnelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1443, 608);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lb_users);
            this.Controls.Add(this.grd_brow);
            this.Controls.Add(this.grd_view);
            this.Controls.Add(this.btn_allUserLMove);
            this.Controls.Add(this.btn_userLMove);
            this.Controls.Add(this.btn_userRMove);
            this.Controls.Add(this.btn_allUserRMove);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_OK);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectedMultiPersonnelForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加人员";
            ((System.ComponentModel.ISupportInitialize)(this.grd_view)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_mainView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_brow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grd_browview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
