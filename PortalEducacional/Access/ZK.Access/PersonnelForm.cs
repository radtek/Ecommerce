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
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZK.Access.data;
using ZK.Access.personnel;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class PersonnelForm : UserControl
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private string personnelCode = string.Empty;

		private string name = string.Empty;

		private string cardNO = string.Empty;

		private string mobilePhone = string.Empty;

		private string dept = string.Empty;

		private string IDnumber = string.Empty;

		private int deptID = 0;

		private bool isDouble;

		private bool adjustDept = false;

		private WaitForm m_wait = WaitForm.Instance;

		private ImagesForm imagesForm = new ImagesForm();

		private UserInfoBll bll = new UserInfoBll(MainForm._ia);

		private DepartmentsBll deptbll = new DepartmentsBll(MainForm._ia);

		private Dictionary<string, Departments> m_drpts = new Dictionary<string, Departments>();

		private Dictionary<int, Departments> m_deptDic = new Dictionary<int, Departments>();

		private List<Departments> deptlist = null;

		private Dictionary<string, string> m_nums = new Dictionary<string, string>();

		private Dictionary<string, string> m_cardnos = new Dictionary<string, string>();

		private DataTable m_datatable = new DataTable();

		private Dictionary<int, int> dicUserId_Fp10Count;

		private Dictionary<int, int> dicUserId_Fp9Count;

		private Dictionary<int, int> dicUserId_FaceCount;

		private int personnelnumber = 0;

		private WaitForm m_waitForm = WaitForm.Instance;

		private ImportDataHelper import = null;

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private int defaultId = -1;

		private List<UserInfo> m_addUsers = new List<UserInfo>();

		private PersonnelIssuecardBll cardBll = new PersonnelIssuecardBll(MainForm._ia);

		private List<PersonnelIssuecard> lstIssueCard = new List<PersonnelIssuecard>();

		private Dictionary<string, UserInfo> dicPin_User;

		private string strCondtion;

		private List<Machines> lstAllMachines;

		private int CardCount;

		private IContainer components = null;

		private BindingSource uSERINFOBindingSource;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_delete;

		private ToolStripMenuItem Menu_import;

		private ToolStripMenuItem Menu_export;

		private ToolStripMenuItem Menu_adjustDept;

		private ToolStripMenuItem Menu_log;

		private ToolStripMenuItem Menu_batchAddPersonnel;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_delete;

		private ToolStripButton btn_import;

		private ToolStripButton btn_export;

		private ToolStripButton btn_adjustDept;

		private ToolStripButton btn_log;

		private ToolStripButton btn_batchAddPersonnel;

		public PanelEx pnl_personnel;

		private GridControl grd_view;

		private GridView grd_userInfoView;

		private GridColumn column_check;

		private GridColumn column_badgenumber;

		private GridColumn column_cardno;

		private GridColumn column_name;

		private GridColumn column_lastName;

		private GridColumn column_code;

		private GridColumn column_deptname;

		private GridColumn column_gender;

		private GridColumn column_acount;

		public ToolStrip MenuPanelEx;

		private Timer timer1;

		private ToolStripButton btn_search;

		private GridColumn column_acountFv;

		private GridColumn colFp9Count;

		private GridColumn colPullFaceCount;

		private GridColumn colStdFaceCount;

		public PersonnelForm()
		{
			this.InitializeComponent();
			Application.DoEvents();
			this.CheckPermission();
			this.GetFaceCountDic();
			this.GetFpCountDic();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
			}
			try
			{
				DevExpressHelper.InitImageList(this.grd_userInfoView, "column_check");
				this.InitDataTableSet();
				this.LoadDept();
				this.GenderType();
				initLang.LocaleForm(this, base.Name);
				this.ChangeSking();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("dataFailedToLoad", "数据加载失败"));
			}
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				ToolStripItem toolStripItem = this.MenuPanelEx.Items.Add(ShowMsgInfos.GetInfo("ClearAllAntyBackState", "清除所有反潜状态"), Resource.clear_antyback, this.ClearAllAntyBackState);
				toolStripItem.ImageScaling = ToolStripItemImageScaling.None;
			}
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
				this.btn_adjustDept.Image = ResourceIPC.Adjust_Department;
				this.btn_search.Image = ResourceIPC.search;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Personnel))
			{
				this.btn_add.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_adjustDept.Enabled = false;
				this.btn_batchAddPersonnel.Enabled = false;
				this.btn_delete.Enabled = false;
				this.btn_import.Enabled = false;
				this.Menu_add.Enabled = false;
				this.Menu_delete.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_import.Enabled = false;
				this.btn_export.Enabled = false;
				this.btn_log.Enabled = false;
				this.Menu_adjustDept.Enabled = false;
				this.Menu_batchAddPersonnel.Enabled = false;
				this.Menu_export.Enabled = false;
				this.Menu_log.Enabled = false;
				this.btn_search.Enabled = false;
			}
			else
			{
				this.btn_add.Enabled = true;
				this.btn_edit.Enabled = true;
				this.btn_adjustDept.Enabled = true;
				this.btn_batchAddPersonnel.Enabled = true;
				this.btn_delete.Enabled = true;
				this.btn_import.Enabled = true;
				this.Menu_add.Enabled = true;
				this.Menu_delete.Enabled = true;
				this.Menu_edit.Enabled = true;
				this.Menu_import.Enabled = false;
				this.btn_export.Enabled = true;
				this.btn_log.Enabled = true;
				this.Menu_adjustDept.Enabled = true;
				this.Menu_batchAddPersonnel.Enabled = true;
				this.Menu_export.Enabled = true;
				this.Menu_log.Enabled = true;
				this.btn_search.Enabled = true;
			}
		}

		private void LoadDept()
		{
			try
			{
				this.m_drpts.Clear();
				this.m_deptDic.Clear();
				this.deptlist = this.deptbll.GetModelList("");
				if (this.deptlist != null)
				{
					for (int i = 0; i < this.deptlist.Count; i++)
					{
						if (!this.m_drpts.ContainsKey(this.deptlist[i].code))
						{
							this.m_drpts.Add(this.deptlist[i].code, this.deptlist[i]);
							this.m_deptDic.Add(this.deptlist[i].DEPTID, this.deptlist[i]);
						}
						Application.DoEvents();
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
			this.m_datatable.Columns.Add("USERID").DataType = typeof(int);
			this.m_datatable.Columns.Add("Badgenumber").DataType = typeof(int);
			this.m_datatable.Columns.Add("cardno");
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("lastname");
			this.m_datatable.Columns.Add("code");
			this.m_datatable.Columns.Add("deptname");
			this.m_datatable.Columns.Add("Gender");
			this.m_datatable.Columns.Add("acount");
			this.m_datatable.Columns.Add("acountFv");
			this.m_datatable.Columns.Add("check");
			this.m_datatable.Columns.Add("Fp9Count");
			this.m_datatable.Columns.Add("PullFaceCount");
			this.m_datatable.Columns.Add("StdFaceCount");
			this.column_badgenumber.FieldName = "Badgenumber";
			this.column_cardno.FieldName = "cardno";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_code.FieldName = "code";
			this.column_deptname.FieldName = "deptname";
			this.column_gender.FieldName = "Gender";
			this.column_acount.FieldName = "acount";
			this.column_acountFv.FieldName = "acountFv";
			this.column_check.FieldName = "check";
			this.colFp9Count.FieldName = "Fp9Count";
			this.colPullFaceCount.FieldName = "PullFaceCount";
			this.colStdFaceCount.FieldName = "StdFaceCount";
			this.grd_userInfoView.OptionsView.ShowFooter = true;
			this.column_acount.SummaryItem.FieldName = "acount";
			this.column_acount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.column_acountFv.SummaryItem.FieldName = "acountFv";
			this.column_acountFv.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.colFp9Count.SummaryItem.FieldName = "Fp9Count";
			this.colFp9Count.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.colPullFaceCount.SummaryItem.FieldName = "PullFaceCount";
			this.colPullFaceCount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.colStdFaceCount.SummaryItem.FieldName = "StdFaceCount";
			this.colStdFaceCount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.column_cardno.SummaryItem.FieldName = "cardno";
			this.column_cardno.SummaryItem.SummaryType = SummaryItemType.Custom;
		}

		private void Add_button_Click(object sender, EventArgs e)
		{
			if (!this.isDouble)
			{
				this.isDouble = true;
				try
				{
					Program.IsRegistZKECardTong();
					if (AccCommon.IsECardTong > 0 && this.personnelnumber >= 3000)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotMoreThanPersonnel", "系统最大支持人数:") + 3000);
						return;
					}
					PersonnelManagementForm personnelManagementForm = new PersonnelManagementForm(0, 0, "0");
					personnelManagementForm.refreshDataEvent += this.addDept_RefreshDataEvent;
					personnelManagementForm.ShowDialog();
					personnelManagementForm.refreshDataEvent -= this.addDept_RefreshDataEvent;
				}
				catch (FileNotFoundException ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
				catch (Exception ex2)
				{
					SysDialogs.ShowWarningMessage(ex2.Message);
				}
				this.isDouble = false;
			}
		}

		private void PersonnelForm_Load(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			this.timer1.Interval = 500;
			this.timer1.Enabled = true;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			this.imagesForm.TopMost = true;
			this.imagesForm.Show();
			Application.DoEvents();
			this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
			this.imagesForm.Hide();
			Application.DoEvents();
			this.Cursor = Cursors.Default;
		}

		private void addIssueForm_RefreshDataEvent(object sender, EventArgs e)
		{
			this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
		}

		private void searchForm_RefreshDataEvent(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					UserInfo userInfo = sender as UserInfo;
					if (userInfo != null)
					{
						this.personnelCode = userInfo.BadgeNumber.ToString();
						this.name = userInfo.Name;
						this.cardNO = userInfo.CardNo;
						this.mobilePhone = userInfo.Pager;
						this.dept = userInfo.DeptName;
						this.IDnumber = userInfo.IdentityCard;
					}
				}
				this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void GetFpCountDic()
		{
			try
			{
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				this.dicUserId_Fp10Count = new Dictionary<int, int>();
				DataTable fields = templateBll.GetFields(" USERID,COUNT(*) as Fp10Count ", " TEMPLATE4 is not null group by USERID ");
				int num;
				int num2;
				for (int i = 0; i < fields.Rows.Count; i++)
				{
					DataRow dataRow = fields.Rows[i];
					int.TryParse(dataRow[0].ToString(), out num);
					int.TryParse(dataRow[1].ToString(), out num2);
					if (!this.dicUserId_Fp10Count.ContainsKey(num))
					{
						this.dicUserId_Fp10Count.Add(num, num2);
					}
					else
					{
						Dictionary<int, int> dictionary = this.dicUserId_Fp10Count;
						int key = num;
						dictionary[key] += num2;
					}
				}
				this.dicUserId_Fp9Count = new Dictionary<int, int>();
				fields = templateBll.GetFields(" USERID,COUNT(*) as Fp9Count ", " TEMPLATE3 is not null group by USERID ");
				for (int j = 0; j < fields.Rows.Count; j++)
				{
					DataRow dataRow = fields.Rows[j];
					int.TryParse(dataRow[0].ToString(), out num);
					int.TryParse(dataRow[1].ToString(), out num2);
					if (!this.dicUserId_Fp9Count.ContainsKey(num))
					{
						this.dicUserId_Fp9Count.Add(num, num2);
					}
					else
					{
						Dictionary<int, int> dictionary = this.dicUserId_Fp9Count;
						int key = num;
						dictionary[key] += num2;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void GetFaceCountDic()
		{
			try
			{
				ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
				this.dicUserId_FaceCount = new Dictionary<int, int>();
				DataTable fields = faceTempBll.GetFields(" USERNO,COUNT(*) as FaceCount ", " TEMPLATE is not null group by USERNO ");
				for (int i = 0; i < fields.Rows.Count; i++)
				{
					DataRow dataRow = fields.Rows[i];
					int.TryParse(dataRow[0].ToString(), out int key);
					int.TryParse(dataRow[1].ToString(), out int _);
					if (!this.dicUserId_FaceCount.ContainsKey(key))
					{
						this.dicUserId_FaceCount.Add(key, 1);
					}
					else
					{
						this.dicUserId_FaceCount[key] = 1;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void addDept_RefreshDataEvent(object sender, EventArgs e)
		{
			this.GetFpCountDic();
			if (this.m_datatable.Rows.Count > 0)
			{
				int[] array = null;
				array = this.grd_userInfoView.GetSelectedRows();
				int num = 0;
				int num2 = 0;
				if (sender is PersonnelManagementForm)
				{
					num = (sender as PersonnelManagementForm).Add_Event_Userid;
				}
				int focusedRowHandle = array[0];
				array = ((!this.isDouble) ? DevExpressHelper.GetCheckedRows(this.grd_userInfoView, "check") : DevExpressHelper.GetDataSourceRowIndexs(this.grd_userInfoView, array));
				if (num > 0)
				{
					this.DataBind(" DEFAULTDEPTID <> -1", num);
					DataRow[] array2 = this.m_datatable.Select("USERID=" + num);
					if (array2.Length != 0)
					{
						int dataSourceIndex = this.m_datatable.Rows.IndexOf(array2[0]);
						dataSourceIndex = this.grd_userInfoView.GetRowHandle(dataSourceIndex);
						this.grd_userInfoView.SelectRow(dataSourceIndex);
						this.grd_userInfoView.FocusedRowHandle = dataSourceIndex;
					}
				}
				else if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
				{
					if (array.Length == 1)
					{
						num2 = int.Parse(this.m_datatable.Rows[array[0]][0].ToString());
						this.DataBind(" DEFAULTDEPTID <> -1", num2);
						this.grd_userInfoView.FocusedRowHandle = focusedRowHandle;
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
				}
			}
			else if (!this.isDouble)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
			}
			else if (this.isDouble)
			{
				this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
			}
		}

		private void DataBind(string dataFilter, int value = 0)
		{
			try
			{
				UserInfoBll.ClearDs();
				this.m_cardnos.Clear();
				this.m_nums.Clear();
				string value2 = string.Empty;
				string value3 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					value2 = dictionary["m"];
					value3 = dictionary["f"];
				}
				DataSet dataSet = (!(dataFilter == "")) ? this.bll.GetList(dataFilter, "N", false) : this.bll.GetAllList("N", false);
				this.m_datatable.BeginLoadData();
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					dataTable.BeginLoadData();
					int key;
					if (this.m_datatable.Rows.Count > 0 && value > 0)
					{
						DataRow[] array = this.m_datatable.Select("USERID=" + value);
						DataRow[] array2 = dataTable.Select("USERID=" + value);
						if (array.Length != 0 && array2.Length != 0)
						{
							array[0].BeginEdit();
							array[0]["USERID"] = array2[0]["USERID"];
							array[0]["Badgenumber"] = array2[0]["Badgenumber"];
							array[0]["CardNo"] = array2[0]["CardNo"];
							array[0]["Name"] = array2[0]["Name"];
							array[0]["lastname"] = array2[0]["lastname"];
							int num = int.Parse(array2[0]["DEFAULTDEPTID"].ToString());
							if (num == 0)
							{
								num = 1;
							}
							array[0]["code"] = this.m_deptDic[num].code;
							array[0]["deptname"] = this.m_deptDic[num].DEPTNAME;
							if (array2[0]["Gender"].ToString().Trim() == "F")
							{
								array[0]["Gender"] = value3;
							}
							else
							{
								array[0]["Gender"] = value2;
							}
							if (array2[0]["ACOUNT"].ToString().Trim() == "")
							{
								array[0]["ACOUNT"] = "0";
							}
							else
							{
								array[0]["ACOUNT"] = array2[0]["ACOUNT"];
							}
							if ("" == array2[0]["ACOUNTFV"].ToString().Trim())
							{
								array[0]["ACOUNTFV"] = "0";
							}
							else
							{
								array[0]["ACOUNTFV"] = array2[0]["ACOUNTFV"];
							}
							array[0]["check"] = false;
							int.TryParse(array2[0]["USERID"].ToString(), out key);
							array[0]["acount"] = (this.dicUserId_Fp10Count.ContainsKey(key) ? this.dicUserId_Fp10Count[key] : 0);
							array[0]["Fp9Count"] = (this.dicUserId_Fp9Count.ContainsKey(key) ? this.dicUserId_Fp9Count[key] : 0);
							array[0]["PullFaceCount"] = (this.dicUserId_FaceCount.ContainsKey(key) ? this.dicUserId_FaceCount[key] : 0);
							array[0].EndEdit();
							if (!this.m_cardnos.ContainsKey(array[0]["CardNo"].ToString()))
							{
								this.m_cardnos.Add(array[0]["CardNo"].ToString(), array[0]["Name"].ToString());
							}
							if (!this.m_nums.ContainsKey(array[0]["USERID"].ToString()))
							{
								this.m_nums.Add(array[0]["USERID"].ToString(), array[0]["Name"].ToString());
							}
						}
						else
						{
							DataRow dataRow = this.m_datatable.NewRow();
							dataRow[0] = array2[0]["USERID"];
							dataRow[1] = array2[0]["Badgenumber"];
							dataRow[2] = array2[0]["CardNo"];
							dataRow[3] = array2[0]["Name"];
							dataRow[4] = array2[0]["lastname"];
							int num = int.Parse(array2[0]["DEFAULTDEPTID"].ToString());
							if (num == 0)
							{
								num = 1;
							}
							dataRow[5] = this.m_deptDic[num].code;
							dataRow[6] = this.m_deptDic[num].DEPTNAME;
							if (array2[0]["Gender"].ToString().Trim() == "F")
							{
								dataRow[7] = value3;
							}
							else
							{
								dataRow[7] = value2;
							}
							if (array2[0]["ACOUNT"].ToString().Trim() == "")
							{
								dataRow[8] = "0";
							}
							else
							{
								dataRow[8] = array2[0]["ACOUNT"];
							}
							if ("" == array2[0]["ACOUNTFV"].ToString().Trim())
							{
								dataRow[9] = "0";
							}
							else
							{
								dataRow[9] = array2[0]["ACOUNTFV"];
							}
							dataRow[10] = false;
							int.TryParse(array2[0]["USERID"].ToString(), out key);
							dataRow[8] = (this.dicUserId_Fp10Count.ContainsKey(key) ? this.dicUserId_Fp10Count[key] : 0);
							dataRow[11] = (this.dicUserId_Fp9Count.ContainsKey(key) ? this.dicUserId_Fp9Count[key] : 0);
							dataRow[12] = (this.dicUserId_FaceCount.ContainsKey(key) ? this.dicUserId_FaceCount[key] : 0);
							this.m_datatable.Rows.Add(dataRow);
							if (!this.m_cardnos.ContainsKey(dataRow[2].ToString()))
							{
								this.m_cardnos.Add(dataRow[2].ToString(), dataRow[3].ToString());
							}
							if (!this.m_nums.ContainsKey(dataRow[1].ToString()))
							{
								this.m_nums.Add(dataRow[1].ToString(), dataRow[3].ToString());
							}
						}
					}
					else
					{
						this.m_datatable.Rows.Clear();
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							DataRow dataRow2 = this.m_datatable.NewRow();
							dataRow2[0] = dataTable.Rows[i]["USERID"].ToString();
							dataRow2[1] = dataTable.Rows[i]["Badgenumber"].ToString();
							dataRow2[2] = dataTable.Rows[i]["CardNo"].ToString();
							dataRow2[3] = dataTable.Rows[i]["Name"].ToString();
							dataRow2[4] = dataTable.Rows[i]["lastname"].ToString();
							int num = int.Parse(dataTable.Rows[i]["DEFAULTDEPTID"].ToString());
							if (num == 0)
							{
								num = 1;
							}
							if (this.m_deptDic.ContainsKey(num))
							{
								dataRow2[5] = this.m_deptDic[num].code;
								dataRow2[6] = this.m_deptDic[num].DEPTNAME;
							}
							if (dataTable.Rows[i]["Gender"].ToString() == "F")
							{
								dataRow2[7] = value3;
							}
							else
							{
								dataRow2[7] = value2;
							}
							string text = dataTable.Rows[i]["ACOUNT"].ToString();
							if (text == "")
							{
								dataRow2[8] = "0";
							}
							else
							{
								dataRow2[8] = text;
							}
							string text2 = dataTable.Rows[i]["ACOUNTFV"].ToString();
							if ("" == text2)
							{
								dataRow2[9] = "0";
							}
							else
							{
								dataRow2[9] = text2;
							}
							dataRow2[10] = false;
							int.TryParse(dataTable.Rows[i]["USERID"].ToString(), out key);
							dataRow2[8] = (this.dicUserId_Fp10Count.ContainsKey(key) ? this.dicUserId_Fp10Count[key] : 0);
							dataRow2[11] = (this.dicUserId_Fp9Count.ContainsKey(key) ? this.dicUserId_Fp9Count[key] : 0);
							dataRow2[12] = (this.dicUserId_FaceCount.ContainsKey(key) ? this.dicUserId_FaceCount[key] : 0);
							this.m_datatable.Rows.Add(dataRow2);
							if (!this.m_cardnos.ContainsKey(dataRow2[2].ToString()))
							{
								this.m_cardnos.Add(dataRow2[2].ToString(), dataRow2[3].ToString());
							}
							if (!this.m_nums.ContainsKey(dataRow2[1].ToString()))
							{
								this.m_nums.Add(dataRow2[1].ToString(), dataRow2[3].ToString());
							}
						}
					}
					dataTable.EndLoadData();
				}
				if (this.m_datatable.Rows.Count > 0)
				{
					this.personnelnumber = this.m_datatable.Rows.Count;
					this.CheckPermission();
				}
				else
				{
					this.btn_edit.Enabled = false;
					this.btn_export.Enabled = false;
					this.btn_delete.Enabled = false;
					this.btn_adjustDept.Enabled = false;
					this.btn_batchAddPersonnel.Enabled = false;
					this.Menu_edit.Enabled = false;
					this.Menu_export.Enabled = false;
					this.Menu_delete.Enabled = false;
					this.Menu_adjustDept.Enabled = false;
					this.Menu_batchAddPersonnel.Enabled = false;
					this.btn_search.Enabled = false;
				}
				this.grd_view.BeginUpdate();
				this.grd_view.DataSource = this.m_datatable;
				this.grd_view.EndUpdate();
				this.m_datatable.EndLoadData();
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void deleteAllUser(int[] rows)
		{
			for (int i = 0; i < rows.Length; i++)
			{
				string empty = string.Empty;
				this.m_wait.ShowProgress(i * 100 / rows.Length);
				if (rows[i] < 0 || rows[i] >= this.m_datatable.Rows.Count)
				{
					break;
				}
				int num = 0;
				UserInfo model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[rows[i]][0].ToString()));
				try
				{
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					List<Machines> modelList = machinesBll.GetModelList("id in(select  device_id from acc_door where id in(select  accdoor_id from acc_levelset_door_group where acclevelset_id in(select acclevelset_id from acc_levelset_emp where employee_id=" + model.UserId + ")))");
					if (this.bll.Delete(model))
					{
						CommandServer.DeleteTemplateCmd(model, modelList);
						CommandServer.DelCmd(model, modelList);
						empty = PullSDKDataConvertHelper.DeleteUserAuthorize(model, true);
						CommandServer.AddCmd(model, true, empty, modelList);
						num = this.cardBll.Search(int.Parse(this.m_datatable.Rows[rows[i]][0].ToString()));
						if (num > 0)
						{
							this.cardBll.Delete(num);
						}
					}
					this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + this.m_datatable.Rows[rows[i]][1].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
				}
				catch (Exception ex)
				{
					this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + this.m_datatable.Rows[rows[i]][1].ToString() + ":" + ShowMsgInfos.GetInfo("DeleteFailed", "删除失败") + ", " + ex.Message + "\r\n");
				}
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			string empty = string.Empty;
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				int[] selectedRows = this.grd_userInfoView.GetSelectedRows();
				int focusedRowHandle = selectedRows[0];
				selectedRows = DevExpressHelper.GetCheckedRows(this.grd_userInfoView, "check");
				if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < this.m_datatable.Rows.Count)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						TemplateBll templateBll = new TemplateBll(MainForm._ia);
						Dictionary<int, int> userId_TemplateCountDic = templateBll.GetUserId_TemplateCountDic("not TEMPLATE4 is null");
						Dictionary<int, int> userId_TemplateCountDic2 = templateBll.GetUserId_TemplateCountDic("not TEMPLATE3 is null");
						FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
						Dictionary<int, int> userId_FingerVeinCountDic = fingerVeinBll.GetUserId_FingerVeinCountDic();
						ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
						Dictionary<int, int> userId_FaceTempCountDic = faceTempBll.GetUserId_FaceTempCountDic("");
						AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
						Dictionary<int, List<int>> userLevelsetIdDic = accLevelsetEmpBll.GetUserLevelsetIdDic();
						AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
						Dictionary<int, List<int>> levelsetMachineIdDic = accLevelsetDoorGroupBll.GetLevelsetMachineIdDic();
						MachinesBll machinesBll = new MachinesBll(MainForm._ia);
						List<Machines> modelList = machinesBll.GetModelList("");
						Dictionary<int, Machines> dictionary = new Dictionary<int, Machines>();
						foreach (Machines item in modelList)
						{
							dictionary.Add(item.ID, item);
						}
						int num = 500;
						StringBuilder stringBuilder = new StringBuilder();
						List<DevCmds> list = new List<DevCmds>();
						DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						List<UserInfo> modelList2 = userInfoBll.GetModelList("");
						Dictionary<int, UserInfo> dictionary2 = new Dictionary<int, UserInfo>();
						if (modelList2 != null && modelList2.Count > 0)
						{
							for (int i = 0; i < modelList2.Count; i++)
							{
								if (!dictionary2.ContainsKey(modelList2[i].UserId))
								{
									dictionary2.Add(modelList2[i].UserId, modelList2[i]);
								}
							}
						}
						Dictionary<int, string> dictionary3 = new Dictionary<int, string>();
						Dictionary<int, string> dictionary4 = new Dictionary<int, string>();
						Dictionary<int, string> dictionary5 = new Dictionary<int, string>();
						Dictionary<int, string> dictionary6 = new Dictionary<int, string>();
						Dictionary<int, string> dictionary7 = new Dictionary<int, string>();
						int key;
						string text;
						for (int j = 0; j < selectedRows.Length; j++)
						{
							Application.DoEvents();
							if (int.TryParse(this.m_datatable.Rows[selectedRows[j]]["USERID"].ToString(), out int num2) && dictionary2.ContainsKey(num2))
							{
								string value = this.m_datatable.Rows[selectedRows[j]]["Badgenumber"].ToString();
								if (num2 != 0 && !string.IsNullOrEmpty(value))
								{
									modelList.Clear();
									if (userLevelsetIdDic.ContainsKey(num2))
									{
										foreach (int item2 in userLevelsetIdDic[num2])
										{
											if (levelsetMachineIdDic.ContainsKey(item2))
											{
												foreach (int item3 in levelsetMachineIdDic[item2])
												{
													if (dictionary.ContainsKey(item3) && !modelList.Contains(dictionary[item3]))
													{
														modelList.Add(dictionary[item3]);
													}
												}
											}
										}
									}
									if (modelList.Count > 0)
									{
										List<UserInfo> list2 = new List<UserInfo>();
										list2.Add(dictionary2[num2]);
										for (int k = 0; k < modelList.Count; k++)
										{
											if (!dictionary6.ContainsKey(modelList[k].ID))
											{
												dictionary6.Add(modelList[k].ID, "");
											}
											if (!dictionary4.ContainsKey(modelList[k].ID))
											{
												dictionary4.Add(modelList[k].ID, "");
											}
											if (!dictionary5.ContainsKey(modelList[k].ID))
											{
												dictionary5.Add(modelList[k].ID, "");
											}
											if (!dictionary3.ContainsKey(modelList[k].ID))
											{
												dictionary3.Add(modelList[k].ID, "");
											}
											if (!dictionary7.ContainsKey(modelList[k].ID))
											{
												dictionary7.Add(modelList[k].ID, "");
											}
											SDKType devSDKType = modelList[k].DevSDKType;
											Dictionary<int, string> dictionary8;
											if (devSDKType != SDKType.StandaloneSDK)
											{
												if (userId_FaceTempCountDic.ContainsKey(num2))
												{
													CommandServer.DeleteFace(modelList[k], list2, false, out empty);
													dictionary8 = dictionary6;
													key = modelList[k].ID;
													dictionary8[key] += empty;
												}
												if (userId_FingerVeinCountDic.ContainsKey(num2))
												{
													CommandServer.DeleteFingerVein(modelList[k], list2, false, out empty);
													dictionary8 = dictionary5;
													key = modelList[k].ID;
													dictionary8[key] += empty;
												}
												if (this.dicUserId_Fp10Count.ContainsKey(num2))
												{
													CommandServer.DeleteFingerPrint(modelList[k], list2, false, out empty);
													dictionary8 = dictionary4;
													key = modelList[k].ID;
													dictionary8[key] += empty;
												}
												CommandServer.DeleteUserAuthorize(modelList[k], list2, false, out empty);
												dictionary8 = dictionary7;
												key = modelList[k].ID;
												dictionary8[key] += empty;
											}
											CommandServer.DeleteUser(modelList[k], list2, false, out empty);
											dictionary8 = dictionary3;
											key = modelList[k].ID;
											dictionary8[key] += empty;
										}
									}
									stringBuilder.Append(num2.ToString() + ",");
									if (j != 0 && j % num == 0)
									{
										foreach (KeyValuePair<int, string> item4 in dictionary6)
										{
											if (item4.Value != null && item4.Value.Trim() != "")
											{
												CommandServer.SaveCmdInfo(item4.Key, "delete|ssrface7$" + item4.Value);
											}
										}
										foreach (KeyValuePair<int, string> item5 in dictionary5)
										{
											if (item5.Value != null && item5.Value.Trim() != "")
											{
												CommandServer.SaveCmdInfo(item5.Key, "delete|fvtemplate$" + item5.Value);
											}
										}
										foreach (KeyValuePair<int, string> item6 in dictionary4)
										{
											if (item6.Value != null && item6.Value.Trim() != "")
											{
												CommandServer.SaveCmdInfo(item6.Key, "delete|templatev10$" + item6.Value);
											}
										}
										foreach (KeyValuePair<int, string> item7 in dictionary3)
										{
											if (item7.Value != null && item7.Value.Trim() != "")
											{
												CommandServer.SaveCmdInfo(item7.Key, "delete|user$" + item7.Value);
											}
										}
										foreach (KeyValuePair<int, string> item8 in dictionary7)
										{
											if (item8.Value != null && item8.Value.Trim() != "")
											{
												CommandServer.SaveCmdInfo(item8.Key, "delete|userauthorize$" + item8.Value);
											}
										}
										dictionary6.Clear();
										dictionary5.Clear();
										dictionary4.Clear();
										dictionary3.Clear();
										dictionary7.Clear();
										text = stringBuilder.ToString();
										if (!string.IsNullOrEmpty(text))
										{
											text = text.Substring(0, text.Length - 1);
											userInfoBll.DeleteList(text);
											personnelIssuecardBll.DeleteListByUserIDs(text);
											WaitForm wait = this.m_wait;
											string info = ShowMsgInfos.GetInfo("SDelUserCount", "删除人数");
											key = text.Split(',').Length;
											wait.ShowInfos(info + key.ToString() + "\r\n");
										}
										list.Clear();
										stringBuilder = new StringBuilder();
									}
								}
							}
						}
						foreach (KeyValuePair<int, string> item9 in dictionary6)
						{
							if (item9.Value != null && item9.Value.Trim() != "")
							{
								CommandServer.SaveCmdInfo(item9.Key, "delete|ssrface7$" + item9.Value);
							}
						}
						foreach (KeyValuePair<int, string> item10 in dictionary5)
						{
							if (item10.Value != null && item10.Value.Trim() != "")
							{
								CommandServer.SaveCmdInfo(item10.Key, "delete|fvtemplate$" + item10.Value);
							}
						}
						foreach (KeyValuePair<int, string> item11 in dictionary4)
						{
							if (item11.Value != null && item11.Value.Trim() != "")
							{
								CommandServer.SaveCmdInfo(item11.Key, "delete|templatev10$" + item11.Value);
							}
						}
						foreach (KeyValuePair<int, string> item12 in dictionary3)
						{
							if (item12.Value != null && item12.Value.Trim() != "")
							{
								CommandServer.SaveCmdInfo(item12.Key, "delete|user$" + item12.Value);
							}
						}
						foreach (KeyValuePair<int, string> item13 in dictionary7)
						{
							if (item13.Value != null && item13.Value.Trim() != "")
							{
								CommandServer.SaveCmdInfo(item13.Key, "delete|userauthorize$" + item13.Value);
							}
						}
						dictionary6.Clear();
						dictionary5.Clear();
						dictionary4.Clear();
						dictionary3.Clear();
						dictionary7.Clear();
						text = stringBuilder.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							text = text.Substring(0, text.Length - 1);
							userInfoBll.DeleteList(text);
							personnelIssuecardBll.DeleteListByUserIDs(text);
							WaitForm wait2 = this.m_wait;
							string info2 = ShowMsgInfos.GetInfo("SDelUserCount", "删除人数");
							key = text.Split(',').Length;
							wait2.ShowInfos(info2 + key.ToString() + "\r\n");
						}
						this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
						this.grd_userInfoView.FocusedRowHandle = focusedRowHandle;
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
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			try
			{
				if (SysInfos.IsOwerControlPermission(SysInfos.Personnel))
				{
					if (this.m_datatable.Rows.Count > 0)
					{
						int[] array = null;
						if (this.isDouble)
						{
							array = this.grd_userInfoView.GetSelectedRows();
							array = DevExpressHelper.GetDataSourceRowIndexs(this.grd_userInfoView, array);
						}
						else
						{
							array = DevExpressHelper.GetCheckedRows(this.grd_userInfoView, "check");
						}
						if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
						{
							if (array.Length == 1)
							{
								string s = this.m_datatable.Rows[array[0]][0].ToString();
								string text = this.m_datatable.Rows[array[0]][6].ToString();
								string text2 = this.m_datatable.Rows[array[0]][5].ToString();
								string deptName = text;
								Program.IsRegistZKECardTong();
								PersonnelManagementForm personnelManagementForm = new PersonnelManagementForm(int.Parse(s), 0, deptName);
								personnelManagementForm.refreshDataEvent += this.addDept_RefreshDataEvent;
								personnelManagementForm.ShowDialog();
								personnelManagementForm.refreshDataEvent -= this.addDept_RefreshDataEvent;
							}
							else
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
							}
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
						}
					}
					else if (!this.isDouble)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			catch (Exception ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
			}
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
		}

		private void btn_issueCard_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_userInfoView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length == 1)
					{
						if (string.IsNullOrEmpty(this.m_datatable.Rows[checkedRows[0]][2].ToString()))
						{
							UserInfo userInfo = new UserInfo();
							userInfo.UserId = int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString());
							userInfo.BadgeNumber = this.m_datatable.Rows[checkedRows[0]][1].ToString();
							userInfo.CardNo = this.m_datatable.Rows[checkedRows[0]][2].ToString();
							userInfo.Name = this.m_datatable.Rows[checkedRows[0]][3].ToString();
							AddIssueForm addIssueForm = new AddIssueForm(userInfo);
							addIssueForm.RefreshDataEvent += this.addIssueForm_RefreshDataEvent;
							addIssueForm.ShowDialog();
							addIssueForm.RefreshDataEvent -= this.addIssueForm_RefreshDataEvent;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("cardHaveBeenIssued", "该人员已发卡!"));
						}
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
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void AdjustDeptForm_RefreshDataEvent(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					this.deptID = int.Parse(sender.ToString());
					this.adjustDept = true;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_adjustDept_Click(object sender, EventArgs e)
		{
			try
			{
				bool flag = true;
				string text = "";
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_userInfoView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length && checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count; i++)
					{
						text = text + this.m_datatable.Rows[checkedRows[i]][1].ToString() + " ";
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
					flag = false;
				}
				if (flag)
				{
					AdjustDeptForm adjustDeptForm = new AdjustDeptForm(text);
					adjustDeptForm.RefreshDataEvent += this.AdjustDeptForm_RefreshDataEvent;
					adjustDeptForm.ShowDialog();
					adjustDeptForm.RefreshDataEvent -= this.AdjustDeptForm_RefreshDataEvent;
					if (this.adjustDept && checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						for (int j = 0; j < checkedRows.Length && checkedRows[j] >= 0 && checkedRows[j] < this.m_datatable.Rows.Count; j++)
						{
							UserInfo model = this.bll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[j]][0].ToString()));
							if (model != null && model.DefaultDeptId != this.deptID)
							{
								model.DefaultDeptId = this.deptID;
								this.bll.Update(model);
							}
						}
						this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_export_Click(object sender, EventArgs e)
		{
			Program.IsRegistZKECardTong();
			object[] obj = new object[8]
			{
				this.pnl_personnel.Text,
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
			DevExpressHelper.OutData(this.grd_userInfoView, fileName);
		}

		private void btn_log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("UserInfo");
			logsInfoForm.ShowDialog();
			this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
		}

		private void grd_userInfoView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void grd_userInfoView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_userInfoView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void btn_batchAddPersonnel_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_userInfoView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length == 1)
					{
						string s = this.m_datatable.Rows[checkedRows[0]][0].ToString();
						Program.IsRegistZKECardTong();
						BatchAddPersonnelForm batchAddPersonnelForm = new BatchAddPersonnelForm(int.Parse(s));
						batchAddPersonnelForm.ShowDialog();
						this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectBatchAddPersonnel", "批量添加人员需要选择一个人员作为复制的对象"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
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
					Program.IsRegistZKECardTong();
					this.import = new ImportDataHelper();
					this.import.SetImportColumnsEvent += this.import_SetImportColumnsEvent;
					this.import.GetRowModelEvent += this.import_GetRowModelEvent;
					this.import.DataLoaded += this.import_DataLoaded;
					this.import.SaveModelEvent += this.import_SaveModelEvent;
					this.import.CheckDataEvent += this.import_CheckDataEvent;
					this.import.FinishEvent += this.import_FinishEvent;
					this.import.ShowInfoEvent += this.ShowInfos;
					this.import.ShowProgressEvent += this.ShowProgress;
					this.import.ShowEvent += this.import_ShowEvent;
					this.m_addUsers.Clear();
					this.lstIssueCard.Clear();
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
						if (this.m_addUsers.Count > 0)
						{
							this.bll.Add(this.m_addUsers, null);
							this.m_addUsers.Clear();
						}
						if (this.lstIssueCard.Count > 0)
						{
							this.LoadAllUser();
							for (int num = this.lstIssueCard.Count - 1; num >= 0; num--)
							{
								if (this.dicPin_User.ContainsKey(this.lstIssueCard[num].personnelNo))
								{
									this.lstIssueCard[num].UserID_id = this.dicPin_User[this.lstIssueCard[num].personnelNo].UserId;
								}
								else
								{
									this.lstIssueCard.RemoveAt(num);
								}
							}
							this.cardBll.Add(this.lstIssueCard);
							this.lstIssueCard.Clear();
						}
						this.DataBind(" DEFAULTDEPTID <> -1 ", 0);
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

		private void import_CheckDataEvent(DataConfig config)
		{
			if (config != null)
			{
				try
				{
					string caption = this.column_badgenumber.Caption;
					if (config.ColumnsToColumnsDic.ContainsKey(caption))
					{
						int num = 0;
						while (true)
						{
							if (num < config.SelectDataSource.Rows.Count)
							{
								DataRow dataRow = config.SelectDataSource.Rows[num];
								if (dataRow[config.ColumnsToColumnsDic[caption]] == null)
								{
									break;
								}
								string text = dataRow[config.ColumnsToColumnsDic[caption]].ToString();
								if (!string.IsNullOrEmpty(text))
								{
									if (text.Length > 9)
									{
										config.Check = false;
										return;
									}
									num++;
									continue;
								}
								if (num == config.SelectDataSource.Rows.Count - 1)
								{
									int num2 = 0;
									while (true)
									{
										if (num2 < config.SelectDataSource.Columns.Count)
										{
											if (!config.DataSource.Columns.Contains(config.DataSource.Columns[num2].ColumnName))
											{
												break;
											}
											if (dataRow[config.DataSource.Columns[num2].ColumnName].ToString() != config.DataSource.Rows[config.DataSource.Rows.Count - 1][num2].ToString())
											{
												config.Check = false;
											}
											num2++;
											continue;
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
							return;
						}
						config.Check = false;
					}
					else
					{
						config.Check = false;
					}
				}
				catch (Exception ex)
				{
					config.Check = false;
					if (this.import != null)
					{
						this.import.ShowInfo(ex.Message);
					}
				}
			}
		}

		private void GenderType()
		{
			this.m_gender = initLang.GetComboxInfo("gender");
			if (this.m_gender == null || this.m_gender.Count == 0)
			{
				this.m_gender = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("m", "男");
				dictionary.Add("f", "女");
				this.m_gender.Add("0", dictionary);
				initLang.SetComboxInfo("gender", this.m_gender);
				initLang.Save();
			}
		}

		private void import_DataLoaded(DataTable dt)
		{
			bool flag = true;
			DataRow dataRow = dt.Rows[dt.Rows.Count - 2];
			for (int i = 1; i < dt.Columns.Count; i++)
			{
				flag = (flag && dataRow[i].ToString().Trim() == "");
				if (!flag)
				{
					break;
				}
			}
			if (flag)
			{
				dt.Rows.Remove(dataRow);
			}
			dataRow = dt.Rows[dt.Rows.Count - 1];
			string fieldName = this.column_badgenumber.FieldName;
			if (dt.Columns.Contains(fieldName) && "" == dataRow[fieldName].ToString().Trim())
			{
				dt.Rows.Remove(dataRow);
			}
		}

		private bool import_SaveModelEvent(object model)
		{
			if (model != null)
			{
				try
				{
					UserInfo userInfo = model as UserInfo;
					if (userInfo != null)
					{
						if (userInfo.BadgeNumber != null && "" != userInfo.BadgeNumber.Trim())
						{
							try
							{
								userInfo.BadgeNumber = int.Parse(userInfo.BadgeNumber).ToString();
							}
							catch
							{
							}
							if (!this.m_nums.ContainsKey(userInfo.BadgeNumber))
							{
								string empty = string.Empty;
								string b = string.Empty;
								if (this.m_gender != null && this.m_gender.ContainsKey("0"))
								{
									Dictionary<string, string> dictionary = this.m_gender["0"];
									empty = dictionary["m"];
									b = dictionary["f"];
								}
								if (userInfo.Gender == b)
								{
									userInfo.Gender = "F";
								}
								else
								{
									userInfo.Gender = "M";
								}
								this.m_nums.Add(userInfo.BadgeNumber, "");
								if (!string.IsNullOrEmpty(userInfo.DeptName))
								{
									if (!this.m_drpts.ContainsKey(userInfo.DeptName))
									{
										if (this.defaultId != -1)
										{
											userInfo.DefaultDeptId = this.defaultId;
										}
										else
										{
											if (this.deptlist != null && this.deptlist.Count > 0)
											{
												int num = 0;
												while (num < this.deptlist.Count)
												{
													if (this.deptlist[num].SUPDEPTID != -1 && this.deptlist[num].SUPDEPTID != 0)
													{
														num++;
														continue;
													}
													userInfo.DefaultDeptId = this.deptlist[num].DEPTID;
													this.defaultId = this.deptlist[num].DEPTID;
													break;
												}
												if (this.defaultId == -1)
												{
													this.defaultId = this.deptlist[0].DEPTID;
												}
											}
											userInfo.DefaultDeptId = this.defaultId;
										}
									}
									else
									{
										userInfo.DefaultDeptId = this.m_drpts[userInfo.DeptName].DEPTID;
									}
								}
								userInfo.DeptName = string.Empty;
								if (string.IsNullOrEmpty(userInfo.CardNo))
								{
									this.m_addUsers.Add(userInfo);
								}
								else if (!this.m_cardnos.ContainsKey(userInfo.CardNo))
								{
									PersonnelIssuecard personnelIssuecard = new PersonnelIssuecard();
									personnelIssuecard.create_time = DateTime.Now;
									personnelIssuecard.UserID_id = userInfo.UserId;
									personnelIssuecard.cardno = userInfo.CardNo;
									personnelIssuecard.personnelNo = userInfo.BadgeNumber;
									this.lstIssueCard.Add(personnelIssuecard);
									this.m_cardnos.Add(userInfo.CardNo, "");
									this.m_addUsers.Add(userInfo);
								}
								else
								{
									userInfo.CardNo = string.Empty;
									this.m_addUsers.Add(userInfo);
								}
								if (this.m_addUsers.Count > 500)
								{
									this.bll.Add(this.m_addUsers, "N");
									this.m_addUsers.Clear();
								}
								return true;
							}
							this.import.Config.CheckErrorInfo = ShowMsgInfos.GetInfo("personnelNumberRepeat", "人员编号已存在");
						}
						else
						{
							this.import.Config.CheckErrorInfo = ShowMsgInfos.GetInfo("personnelNumberNotNull", "人员编号不能为空");
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

		private void LoadAllUser()
		{
			this.dicPin_User = new Dictionary<string, UserInfo>();
			UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
			List<UserInfo> modelList = userInfoBll.GetModelList("");
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!this.dicPin_User.ContainsKey(modelList[i].BadgeNumber))
					{
						this.dicPin_User.Add(modelList[i].BadgeNumber, modelList[i]);
					}
				}
			}
		}

		private object import_GetRowModelEvent()
		{
			return new UserInfo();
		}

		private void import_SetImportColumnsEvent(DataConfig config)
		{
			if (config != null)
			{
				try
				{
					config.ColumnsToModelDic.Clear();
					config.ImportColumns.Clear();
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("UserNo", "人员编号"));
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("CardNo", "卡号"));
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("lastName", "姓名"));
					if (initLang.Lang.ToLower() != "chs")
					{
						config.ImportColumns.Add(ShowMsgInfos.GetInfo("FirstName", "姓氏"));
					}
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("DeptNo", "部门编号"));
					config.ImportColumns.Add(ShowMsgInfos.GetInfo("sex", "性别"));
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("UserNo", "人员编号"), "Badgenumber");
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("CardNo", "卡号"), "CardNo");
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("lastName", "姓名"), "Name");
					if (initLang.Lang.ToLower() != "chs")
					{
						config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("FirstName", "姓氏"), "lastname");
					}
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("DeptNo", "部门编号"), "DeptName");
					config.ColumnsToModelDic.Add(ShowMsgInfos.GetInfo("sex", "性别"), "gender");
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

		private void grd_userInfoView_DoubleClick(object sender, EventArgs e)
		{
			if (!this.isDouble)
			{
				this.isDouble = true;
				this.btn_edit_Click(null, null);
				this.isDouble = false;
			}
		}

		private void Menu_edit_Click(object sender, EventArgs e)
		{
			this.btn_edit_Click(null, null);
		}

		private void btn_search_Click_1(object sender, EventArgs e)
		{
			try
			{
				PersonnelSearchForm personnelSearchForm = new PersonnelSearchForm(false);
				personnelSearchForm.Text = this.btn_search.Text;
				personnelSearchForm.ShowDialog();
				if (personnelSearchForm.DialogResult == DialogResult.OK)
				{
					this.strCondtion = personnelSearchForm.ConditionStr;
					this.DataBind(" DEFAULTDEPTID <> -1 " + this.strCondtion, 0);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void ClearAllAntyBackState(object sender, EventArgs e)
		{
			this.LoadAllMachines();
			this.m_wait = WaitForm.Instance;
			this.m_wait.ShowProgress(0);
			this.m_wait.ShowEx();
			Application.DoEvents();
			for (int i = 0; i < this.lstAllMachines.Count; i++)
			{
				try
				{
					DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(this.lstAllMachines[i]);
					if (deviceServer == null)
					{
						this.m_wait.ShowInfos(this.lstAllMachines[i].MachineAlias + ":" + PullSDkErrorInfos.GetInfo(-1002));
						this.m_wait.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.lstAllMachines.Count * 100m));
					}
					else
					{
						int num;
						if (!deviceServer.IsConnected)
						{
							num = deviceServer.Connect(3000);
							if (num < 0)
							{
								this.m_wait.ShowInfos(this.lstAllMachines[i].MachineAlias + ":" + PullSDkErrorInfos.GetInfo(num));
								this.m_wait.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.lstAllMachines.Count * 100m));
								goto end_IL_003a;
							}
						}
						num = deviceServer.ControlDevice(17, 0, 0, 0, 0, "");
						if (num < 0)
						{
							this.m_wait.ShowInfos(this.lstAllMachines[i].MachineAlias + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.m_wait.ShowInfos(this.lstAllMachines[i].MachineAlias + ":" + ShowMsgInfos.GetInfo("ClearAntyBackSuceed", "清除成功"));
						}
						this.m_wait.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.lstAllMachines.Count * 100m));
					}
					end_IL_003a:;
				}
				catch (Exception ex)
				{
					this.m_wait.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.lstAllMachines.Count * 100m));
					this.m_wait.ShowInfos(this.lstAllMachines[i].MachineAlias + ":" + ex.Message);
				}
			}
			this.m_wait.ShowProgress(100);
			this.m_wait.HideEx(false);
		}

		private void LoadAllMachines()
		{
			if (this.lstAllMachines == null)
			{
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				this.lstAllMachines = machinesBll.GetModelList("");
				if (this.lstAllMachines == null)
				{
					this.lstAllMachines = new List<Machines>();
				}
			}
		}

		private void grd_userInfoView_CustomSummaryCalculate(object sender, CustomSummaryEventArgs e)
		{
			object obj = null;
			bool flag = false;
			string fieldName = (e.Item as GridSummaryItem).FieldName;
			GridView gridView = sender as GridView;
			switch (e.SummaryProcess)
			{
			case CustomSummaryProcess.Start:
				this.CardCount = 0;
				break;
			case CustomSummaryProcess.Calculate:
			{
				obj = gridView.GetRowCellValue(e.RowHandle, "Discontinued");
				if (obj != null)
				{
					flag = (bool)obj;
				}
				string a2 = fieldName.ToUpper();
				if (a2 == "CARDNO" && !flag && e.FieldValue != null && e.FieldValue.ToString() != "" && e.FieldValue.ToString() != "0")
				{
					this.CardCount++;
				}
				break;
			}
			case CustomSummaryProcess.Finalize:
			{
				string a = fieldName.ToUpper();
				if (a == "CARDNO")
				{
					e.TotalValue = this.CardCount;
				}
				break;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PersonnelForm));
			this.uSERINFOBindingSource = new BindingSource(this.components);
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_delete = new ToolStripMenuItem();
			this.Menu_adjustDept = new ToolStripMenuItem();
			this.Menu_batchAddPersonnel = new ToolStripMenuItem();
			this.Menu_import = new ToolStripMenuItem();
			this.Menu_export = new ToolStripMenuItem();
			this.Menu_log = new ToolStripMenuItem();
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.btn_search = new ToolStripButton();
			this.btn_adjustDept = new ToolStripButton();
			this.btn_batchAddPersonnel = new ToolStripButton();
			this.btn_import = new ToolStripButton();
			this.btn_export = new ToolStripButton();
			this.btn_log = new ToolStripButton();
			this.pnl_personnel = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_userInfoView = new GridView();
			this.column_check = new GridColumn();
			this.column_badgenumber = new GridColumn();
			this.column_cardno = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_code = new GridColumn();
			this.column_deptname = new GridColumn();
			this.column_gender = new GridColumn();
			this.column_acount = new GridColumn();
			this.colFp9Count = new GridColumn();
			this.column_acountFv = new GridColumn();
			this.colPullFaceCount = new GridColumn();
			this.colStdFaceCount = new GridColumn();
			this.timer1 = new Timer(this.components);
			((ISupportInitialize)this.uSERINFOBindingSource).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_userInfoView).BeginInit();
			base.SuspendLayout();
			this.uSERINFOBindingSource.DataMember = "USERINFO";
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[8]
			{
				this.Menu_add,
				this.Menu_edit,
				this.Menu_delete,
				this.Menu_adjustDept,
				this.Menu_batchAddPersonnel,
				this.Menu_import,
				this.Menu_export,
				this.Menu_log
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(147, 180);
			this.Menu_add.Image = (Image)componentResourceManager.GetObject("Menu_add.Image");
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(146, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.Add_button_Click;
			this.Menu_edit.Image = (Image)componentResourceManager.GetObject("Menu_edit.Image");
			this.Menu_edit.Name = "Menu_edit";
			this.Menu_edit.Size = new Size(146, 22);
			this.Menu_edit.Text = "编辑";
			this.Menu_edit.Click += this.Menu_edit_Click;
			this.Menu_delete.Image = (Image)componentResourceManager.GetObject("Menu_delete.Image");
			this.Menu_delete.Name = "Menu_delete";
			this.Menu_delete.Size = new Size(146, 22);
			this.Menu_delete.Text = "删除";
			this.Menu_delete.Click += this.btn_delete_Click;
			this.Menu_adjustDept.Image = (Image)componentResourceManager.GetObject("Menu_adjustDept.Image");
			this.Menu_adjustDept.Name = "Menu_adjustDept";
			this.Menu_adjustDept.Size = new Size(146, 22);
			this.Menu_adjustDept.Text = "调整部门";
			this.Menu_adjustDept.Click += this.btn_adjustDept_Click;
			this.Menu_batchAddPersonnel.Image = Resources.batchPersonnel;
			this.Menu_batchAddPersonnel.Name = "Menu_batchAddPersonnel";
			this.Menu_batchAddPersonnel.Size = new Size(146, 22);
			this.Menu_batchAddPersonnel.Text = "批量添加人员";
			this.Menu_batchAddPersonnel.Click += this.btn_batchAddPersonnel_Click;
			this.Menu_import.Image = (Image)componentResourceManager.GetObject("Menu_import.Image");
			this.Menu_import.Name = "Menu_import";
			this.Menu_import.Size = new Size(146, 22);
			this.Menu_import.Text = "导入";
			this.Menu_import.Click += this.btn_import_Click;
			this.Menu_export.Image = (Image)componentResourceManager.GetObject("Menu_export.Image");
			this.Menu_export.Name = "Menu_export";
			this.Menu_export.Size = new Size(146, 22);
			this.Menu_export.Text = "导出";
			this.Menu_export.Click += this.btn_export_Click;
			this.Menu_log.Image = Resources.Log_Entries;
			this.Menu_log.Name = "Menu_log";
			this.Menu_log.Size = new Size(146, 22);
			this.Menu_log.Text = "日志记录";
			this.Menu_log.Click += this.btn_log_Click;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[9]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_delete,
				this.btn_search,
				this.btn_adjustDept,
				this.btn_batchAddPersonnel,
				this.btn_import,
				this.btn_export,
				this.btn_log
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 41);
			this.MenuPanelEx.TabIndex = 15;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(94, 38);
			this.btn_add.Text = "Adicionar";
			this.btn_add.Click += this.Add_button_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(73, 38);
			this.btn_edit.Text = "Editar";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.Image = (Image)componentResourceManager.GetObject("btn_delete.Image");
			this.btn_delete.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delete.ImageTransparentColor = Color.Magenta;
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(67, 38);
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.btn_search.Image = (Image)componentResourceManager.GetObject("btn_search.Image");
			this.btn_search.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_search.ImageTransparentColor = Color.Magenta;
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(67, 38);
			this.btn_search.Text = "查找";
			this.btn_search.Click += this.btn_search_Click_1;
			this.btn_adjustDept.Image = (Image)componentResourceManager.GetObject("btn_adjustDept.Image");
			this.btn_adjustDept.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_adjustDept.ImageTransparentColor = Color.Magenta;
			this.btn_adjustDept.Name = "btn_adjustDept";
			this.btn_adjustDept.Size = new Size(91, 38);
			this.btn_adjustDept.Text = "调整部门";
			this.btn_adjustDept.Click += this.btn_adjustDept_Click;
			this.btn_batchAddPersonnel.Image = (Image)componentResourceManager.GetObject("btn_batchAddPersonnel.Image");
			this.btn_batchAddPersonnel.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_batchAddPersonnel.ImageTransparentColor = Color.Magenta;
			this.btn_batchAddPersonnel.Name = "btn_batchAddPersonnel";
			this.btn_batchAddPersonnel.Size = new Size(115, 38);
			this.btn_batchAddPersonnel.Text = "批量添加人员";
			this.btn_batchAddPersonnel.Click += this.btn_batchAddPersonnel_Click;
			this.btn_import.Image = (Image)componentResourceManager.GetObject("btn_import.Image");
			this.btn_import.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_import.ImageTransparentColor = Color.Magenta;
			this.btn_import.Name = "btn_import";
			this.btn_import.Size = new Size(67, 38);
			this.btn_import.Text = "导入";
			this.btn_import.Click += this.btn_import_Click;
			this.btn_export.Image = (Image)componentResourceManager.GetObject("btn_export.Image");
			this.btn_export.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_export.ImageTransparentColor = Color.Magenta;
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(67, 38);
			this.btn_export.Text = "导出";
			this.btn_export.Click += this.btn_export_Click;
			this.btn_log.Image = (Image)componentResourceManager.GetObject("btn_log.Image");
			this.btn_log.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_log.ImageTransparentColor = Color.Magenta;
			this.btn_log.Name = "btn_log";
			this.btn_log.Size = new Size(91, 36);
			this.btn_log.Text = "日志记录";
			this.btn_log.Click += this.btn_log_Click;
			this.pnl_personnel.CanvasColor = SystemColors.Control;
			this.pnl_personnel.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_personnel.Dock = DockStyle.Top;
			this.pnl_personnel.Location = new Point(0, 41);
			this.pnl_personnel.Name = "pnl_personnel";
			this.pnl_personnel.Size = new Size(684, 25);
			this.pnl_personnel.Style.Alignment = StringAlignment.Center;
			this.pnl_personnel.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_personnel.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_personnel.Style.Border = eBorderType.SingleLine;
			this.pnl_personnel.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_personnel.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_personnel.Style.GradientAngle = 90;
			this.pnl_personnel.TabIndex = 16;
			this.pnl_personnel.Text = "人员";
			this.grd_view.Cursor = Cursors.Default;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 66);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_userInfoView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(684, 427);
			this.grd_view.TabIndex = 17;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_userInfoView
			});
			this.grd_userInfoView.Columns.AddRange(new GridColumn[13]
			{
				this.column_check,
				this.column_badgenumber,
				this.column_cardno,
				this.column_name,
				this.column_lastName,
				this.column_code,
				this.column_deptname,
				this.column_gender,
				this.column_acount,
				this.colFp9Count,
				this.column_acountFv,
				this.colPullFaceCount,
				this.colStdFaceCount
			});
			this.grd_userInfoView.GridControl = this.grd_view;
			this.grd_userInfoView.Name = "grd_userInfoView";
			this.grd_userInfoView.OptionsBehavior.Editable = false;
			this.grd_userInfoView.OptionsView.ShowGroupPanel = false;
			this.grd_userInfoView.SortInfo.AddRange(new GridColumnSortInfo[1]
			{
				new GridColumnSortInfo(this.column_badgenumber, ColumnSortOrder.Ascending)
			});
			this.grd_userInfoView.CustomDrawColumnHeader += this.grd_userInfoView_CustomDrawColumnHeader;
			this.grd_userInfoView.CustomDrawCell += this.grd_userInfoView_CustomDrawCell;
			this.grd_userInfoView.CustomSummaryCalculate += this.grd_userInfoView_CustomSummaryCalculate;
			this.grd_userInfoView.Click += this.grd_userInfoView_Click;
			this.grd_userInfoView.DoubleClick += this.grd_userInfoView_DoubleClick;
			this.column_check.Caption = "gridColumn1";
			this.column_check.Name = "column_check";
			this.column_check.OptionsColumn.AllowSort = DefaultBoolean.False;
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 20;
			this.column_badgenumber.AppearanceCell.Options.UseTextOptions = true;
			this.column_badgenumber.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			this.column_badgenumber.Caption = "人员编号";
			this.column_badgenumber.Name = "column_badgenumber";
			this.column_badgenumber.Visible = true;
			this.column_badgenumber.VisibleIndex = 1;
			this.column_badgenumber.Width = 73;
			this.column_cardno.Caption = "卡号";
			this.column_cardno.Name = "column_cardno";
			this.column_cardno.Visible = true;
			this.column_cardno.VisibleIndex = 4;
			this.column_cardno.Width = 73;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 2;
			this.column_name.Width = 73;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_lastName.Width = 66;
			this.column_code.Caption = "部门编号";
			this.column_code.Name = "column_code";
			this.column_code.Visible = true;
			this.column_code.VisibleIndex = 5;
			this.column_code.Width = 73;
			this.column_deptname.Caption = "部门名称";
			this.column_deptname.Name = "column_deptname";
			this.column_deptname.Visible = true;
			this.column_deptname.VisibleIndex = 6;
			this.column_deptname.Width = 73;
			this.column_gender.Caption = "性别";
			this.column_gender.Name = "column_gender";
			this.column_gender.Visible = true;
			this.column_gender.VisibleIndex = 7;
			this.column_gender.Width = 73;
			this.column_acount.Caption = "10.0指纹数";
			this.column_acount.Name = "column_acount";
			this.column_acount.Visible = true;
			this.column_acount.VisibleIndex = 8;
			this.column_acount.Width = 80;
			this.colFp9Count.Caption = "9.0指纹数";
			this.colFp9Count.Name = "colFp9Count";
			this.colFp9Count.Visible = true;
			this.colFp9Count.VisibleIndex = 9;
			this.column_acountFv.Caption = "静脉数";
			this.column_acountFv.Name = "column_acountFv";
			this.column_acountFv.Visible = true;
			this.column_acountFv.VisibleIndex = 10;
			this.column_acountFv.Width = 80;
			this.colPullFaceCount.Caption = "人脸数";
			this.colPullFaceCount.Name = "colPullFaceCount";
			this.colPullFaceCount.Visible = true;
			this.colPullFaceCount.VisibleIndex = 11;
			this.colStdFaceCount.Caption = "人脸数(脱机)";
			this.colStdFaceCount.Name = "colStdFaceCount";
			this.colStdFaceCount.OptionsColumn.ShowInCustomizationForm = false;
			this.colStdFaceCount.OptionsColumn.ShowInExpressionEditor = false;
			this.timer1.Tick += this.timer1_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			this.ContextMenuStrip = this.contextMenuStrip1;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_personnel);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "PersonnelForm";
			base.Size = new Size(684, 493);
			base.Load += this.PersonnelForm_Load;
			((ISupportInitialize)this.uSERINFOBindingSource).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_userInfoView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
