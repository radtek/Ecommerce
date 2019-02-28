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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using ZK.Access.mensage;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Data.Model.StdSDK;
using ZK.Utils;
using ZK.Utils.Dialogs;

namespace ZK.Access
{
	public class AddLevelForm : Office2007Form
	{
		private int levelID = 0;

		private Dictionary<int, int> timeIDList = new Dictionary<int, int>();

		private ImagesForm imagesForm = new ImagesForm();

		private Dictionary<int, List<int>> glist = new Dictionary<int, List<int>>();

		private DataTable m_dataTable = new DataTable();

		private DataTable m_browDataTable = new DataTable();

		private DataTable m_doorDataTable = new DataTable();

		private DataTable m_browDoorDataTable = new DataTable();

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> dicId_Machine = new Dictionary<int, Machines>();

		private Dictionary<int, AccDoor> dicId_Door = new Dictionary<int, AccDoor>();

		private Dictionary<int, UserInfo> dicId_User = new Dictionary<int, UserInfo>();

		private AccLevelset oldLevel = null;

		private List<AccLevelsetEmp> oldlistEmp = null;

		private List<AccLevelsetDoorGroup> oldlistDoor = null;

		private List<int> m_doorTempList = new List<int>();

		private List<int> m_userTempList = new List<int>();

		private WaitForm m_wait = WaitForm.Instance;

		private int DoorTimeIndex = -1;

		private string LevelNameText = "";

		private int PlanID = -1;

		private bool isDouble = false;

		private Dictionary<int, AccTimeseg> dicId_Timeseg;

		private IContainer components = null;

		private ButtonX btn_exit;

		private ButtonX btn_allow;

		private ButtonX btn_allDoorLMove;

		private ButtonX btn_doorLMove;

		private ButtonX btn_doorRMove;

		private ButtonX btn_allDoorRMove;

		private TextBox txt_levelName;

		private Label lab_doorTime;

		private Label lab_levelName;

		private ButtonX btn_allUserLMove;

		private ButtonX btn_userLMove;

		private ButtonX btn_userRMove;

		private ButtonX btn_allUserRMove;

		private GridControl grd_view;

		private GridView gridUser;

		private GridColumn column_deptName;

		private GridColumn column_Badgenumber;

		private GridColumn column_name;

		private GridColumn column_cardNO;

		private GridControl dgrd_selectedDoor;

		private GridView gridSelectedDoor;

		private GridColumn column_selectedDoor;

		private GridColumn column_selectedDevice;

		private GridControl dgrd_selectedUser;

		private GridView gridSelectedUser;

		private GridColumn column_selectedDept;

		private GridColumn column_selectedPersonnel;

		private GridColumn column_selectedName;

		private GridColumn column_selectedCard;

		private Label lb_doors;

		private Label lb_selectDoors;

		private Label lb_users;

		private Label lb_selectUsers;

		private GridColumn column_checkuser;

		private GridColumn column_checkdoor;

		private GridColumn column_checkSelectUser;

		private Label label2;

		private Label label1;

		private GridColumn column_lastName;

		private GridColumn column_selectedLastName;

		private RepositoryItemCheckEdit re2;

		private GridView gridDoor;

		private GridColumn column_check;

		private GridColumn column_door;

		private GridColumn column_device;

		private GridControl grid_door;

		private ComboBox cbo_doorTime;

		private System.Windows.Forms.Timer time_load;

		private ErrorProvider errorProvider1;

		private CheckBox chkIsVisitLevel;

		private Label label3;

		private Button btn_help_info;

		public event EventHandler refreshDataEvent = null;

		public AddLevelForm(int id)
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
				this.column_selectedLastName.Visible = false;
			}
			this.levelID = id;
			try
			{
				DevExpressHelper.InitImageList(this.gridUser, "column_checkuser");
				DevExpressHelper.InitImageList(this.gridSelectedUser, "column_checkSelectUser");
				DevExpressHelper.InitImageList(this.gridDoor, "column_check");
				DevExpressHelper.InitImageList(this.gridSelectedDoor, "column_checkdoor");
				this.InitDataTableSet();
				this.InitDoorDataTableSet();
				this.InitBrowDataTableSet();
				this.InitBrowDoorDataTableSet();
				this.InitMachines();
				this.LoadTimeSeg();
				this.LoadDoorInfos();
				this.loadDoor();
				initLang.LocaleForm(this, base.Name);
				this.CheckDoorBtn();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void LoadDoorGroup()
		{
			this.glist.Clear();
			try
			{
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				List<AccLevelsetDoorGroup> modelList = accLevelsetDoorGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.glist.ContainsKey(modelList[i].acclevelset_id))
						{
							if (!this.glist[modelList[i].acclevelset_id].Contains(modelList[i].accdoor_id))
							{
								this.glist[modelList[i].acclevelset_id].Add(modelList[i].accdoor_id);
							}
						}
						else
						{
							List<int> list = new List<int>();
							list.Add(modelList[i].accdoor_id);
							this.glist.Add(modelList[i].acclevelset_id, list);
						}
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
			this.m_dataTable.Columns.Add("userid");
			this.m_dataTable.Columns.Add("DEPTNAME");
			this.m_dataTable.Columns.Add("Badgenumber").DataType = typeof(int);
			this.m_dataTable.Columns.Add("Name");
			this.m_dataTable.Columns.Add("lastname");
			this.m_dataTable.Columns.Add("CardNo");
			this.m_dataTable.Columns.Add("check");
			this.column_deptName.FieldName = "DEPTNAME";
			this.column_Badgenumber.FieldName = "Badgenumber";
			this.column_name.FieldName = "Name";
			this.column_lastName.FieldName = "lastname";
			this.column_cardNO.FieldName = "CardNo";
			this.column_checkuser.FieldName = "check";
			this.grd_view.DataSource = this.m_dataTable;
		}

		private void InitBrowDataTableSet()
		{
			this.m_browDataTable.Columns.Add("userid");
			this.m_browDataTable.Columns.Add("DEPTNAME");
			this.m_browDataTable.Columns.Add("Badgenumber").DataType = typeof(int);
			this.m_browDataTable.Columns.Add("Name");
			this.m_browDataTable.Columns.Add("lastname");
			this.m_browDataTable.Columns.Add("CardNo");
			this.m_browDataTable.Columns.Add("check");
			this.column_selectedDept.FieldName = "DEPTNAME";
			this.column_selectedPersonnel.FieldName = "Badgenumber";
			this.column_selectedName.FieldName = "Name";
			this.column_selectedLastName.FieldName = "lastname";
			this.column_selectedCard.FieldName = "CardNo";
			this.column_checkSelectUser.FieldName = "check";
			this.dgrd_selectedUser.DataSource = this.m_browDataTable;
		}

		private void InitDoorDataTableSet()
		{
			this.m_doorDataTable.Columns.Add("id");
			this.m_doorDataTable.Columns.Add("door_name");
			this.m_doorDataTable.Columns.Add("device_name");
			this.m_doorDataTable.Columns.Add("check");
			this.m_doorDataTable.Columns.Add("door_no");
			this.m_doorDataTable.Columns.Add("device_id");
			this.m_doorDataTable.Columns.Add("SDKType", typeof(int));
			this.column_door.FieldName = "door_name";
			this.column_device.FieldName = "device_name";
			this.column_check.FieldName = "check";
			this.grid_door.DataSource = this.m_doorDataTable;
		}

		private void InitBrowDoorDataTableSet()
		{
			this.m_browDoorDataTable.Columns.Add("id");
			this.m_browDoorDataTable.Columns.Add("door_name");
			this.m_browDoorDataTable.Columns.Add("device_name");
			this.m_browDoorDataTable.Columns.Add("check");
			this.m_browDoorDataTable.Columns.Add("door_no");
			this.m_browDoorDataTable.Columns.Add("device_id");
			this.m_browDoorDataTable.Columns.Add("SDKType", typeof(int));
			this.column_selectedDoor.FieldName = "door_name";
			this.column_selectedDevice.FieldName = "device_name";
			this.column_checkdoor.FieldName = "check";
			this.dgrd_selectedDoor.DataSource = this.m_browDoorDataTable;
		}

		private void InitMachines()
		{
			try
			{
				this.dicId_Machine.Clear();
				List<Machines> list = null;
				list = this.mbll.GetModelList("");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.dicId_Machine.ContainsKey(list[i].ID))
						{
							this.dicId_Machine.Add(list[i].ID, list[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadTimeSeg()
		{
			try
			{
				this.dicId_Timeseg = new Dictionary<int, AccTimeseg>();
				this.cbo_doorTime.Items.Clear();
				this.timeIDList.Clear();
				this.cbo_doorTime.Items.Add("-----");
				this.timeIDList.Add(0, 0);
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.timeIDList.Add(this.cbo_doorTime.Items.Count, modelList[i].id);
						this.cbo_doorTime.Items.Add(modelList[i].timeseg_name);
						if (!this.dicId_Timeseg.ContainsKey(modelList[i].id))
						{
							this.dicId_Timeseg.Add(modelList[i].id, modelList[i]);
						}
					}
				}
				if (this.cbo_doorTime.Items.Count > 0)
				{
					this.cbo_doorTime.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadDoorInfos()
		{
			try
			{
				this.m_doorDataTable.Rows.Clear();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dicId_Door.ContainsKey(modelList[i].id))
						{
							this.dicId_Door.Add(modelList[i].id, modelList[i]);
						}
						if (this.dicId_Machine.ContainsKey(modelList[i].device_id))
						{
							DataRow dataRow = this.m_doorDataTable.NewRow();
							dataRow[0] = modelList[i].id;
							dataRow[1] = modelList[i].door_name;
							dataRow[2] = this.dicId_Machine[modelList[i].device_id].MachineAlias;
							dataRow[3] = false;
							dataRow[4] = modelList[i].door_no;
							dataRow[5] = modelList[i].device_id;
							dataRow[6] = (int)this.dicId_Machine[modelList[i].device_id].DevSDKType;
							this.m_doorDataTable.Rows.Add(dataRow);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadUserInfo(string strWhere = "")
		{
			try
			{
				this.m_dataTable.Rows.Clear();
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				List<UserInfo> modelList = userInfoBll.GetModelList(strWhere);
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				Departments departments = null;
				if (modelList != null && modelList.Count > 0)
				{
					this.m_dataTable.BeginLoadData();
					int i = 0;
					for (int count = modelList.Count; i < count; i++)
					{
						if (!this.dicId_User.ContainsKey(modelList[i].UserId))
						{
							this.dicId_User.Add(modelList[i].UserId, modelList[i]);
						}
						DataRow dataRow = this.m_dataTable.NewRow();
						dataRow[0] = modelList[i].UserId;
						departments = departmentsBll.GetModel(modelList[i].DefaultDeptId);
						if (departments != null)
						{
							dataRow[1] = departments.DEPTNAME;
						}
						dataRow[2] = modelList[i].BadgeNumber;
						dataRow[3] = modelList[i].Name;
						dataRow[4] = modelList[i].LastName;
						dataRow[5] = modelList[i].CardNo;
						dataRow[6] = false;
						this.m_dataTable.Rows.Add(dataRow);
					}
					this.m_dataTable.EndLoadData();
				}
				this.gridUser.MoveFirst();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataBind()
		{
			try
			{
				if (this.levelID > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
					AccLevelset model = accLevelsetBll.GetModel(this.levelID);
					if (model != null)
					{
						this.oldLevel = model;
						this.txt_levelName.Text = model.level_name;
						this.chkIsVisitLevel.Checked = model.isVisitLevel;
						foreach (KeyValuePair<int, int> timeID in this.timeIDList)
						{
							if (timeID.Value == model.level_timeseg_id)
							{
								this.cbo_doorTime.SelectedIndex = timeID.Key;
								break;
							}
						}
						AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
						List<AccLevelsetEmp> list = this.oldlistEmp = accLevelsetEmpBll.GetModelList("acclevelset_id=" + this.levelID + " order by employee_id");
						if (list != null && list.Count > 0)
						{
							this.grd_view.BeginUpdate();
							this.dgrd_selectedUser.BeginUpdate();
							this.m_dataTable.BeginLoadData();
							string empty = string.Empty;
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SSLgetDataing", "获取数据..."));
							string empty2 = string.Empty;
							DataTable dataTable = this.m_dataTable.Copy();
							this.imagesForm.TopMost = true;
							this.imagesForm.Show();
							this.m_browDataTable.BeginLoadData();
							List<int> list2 = new List<int>();
							for (int i = 0; i < list.Count; i++)
							{
								list2.Add(list[i].employee_id);
							}
							this.grd_view.DataSource = null;
							this.dgrd_selectedUser.DataSource = null;
							List<DataRow> list3 = new List<DataRow>();
							for (int j = 0; j < this.m_dataTable.Rows.Count; j++)
							{
								empty2 = this.m_dataTable.Rows[j][0].ToString();
								if (list2.Contains(int.Parse(empty2)))
								{
									DataRow dataRow = this.m_browDataTable.NewRow();
									dataRow[0] = this.m_dataTable.Rows[j][0];
									dataRow[1] = this.m_dataTable.Rows[j][1];
									dataRow[2] = this.m_dataTable.Rows[j][2];
									dataRow[3] = this.m_dataTable.Rows[j][3];
									dataRow[4] = this.m_dataTable.Rows[j][4];
									dataRow[5] = this.m_dataTable.Rows[j][5];
									dataRow[6] = false;
									this.m_browDataTable.Rows.Add(dataRow);
									list3.Add(this.m_dataTable.Rows[j]);
								}
							}
							for (int k = 0; k < list3.Count; k++)
							{
								this.m_dataTable.Rows.Remove(list3[k]);
							}
							this.m_browDataTable.EndLoadData();
							this.imagesForm.Hide();
							this.m_dataTable.EndLoadData();
							this.grd_view.EndUpdate();
							this.dgrd_selectedUser.EndUpdate();
						}
					}
				}
				else
				{
					this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				}
				this.CheckUserBtn();
				this.CheckDoorBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.grd_view.DataSource = this.m_dataTable;
			this.dgrd_selectedUser.DataSource = this.m_browDataTable;
		}

		public object CopyDictionaryData(object obj)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, obj);
			memoryStream.Position = 0L;
			object result = binaryFormatter.Deserialize(memoryStream);
			memoryStream.Close();
			return result;
		}

		private void loadDoor()
		{
			try
			{
				if (this.levelID > 0)
				{
					AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
					List<AccLevelsetDoorGroup> modelList = accLevelsetDoorGroupBll.GetModelList("acclevelset_id=" + this.levelID);
					this.btn_doorLMove.Enabled = false;
					this.btn_allDoorLMove.Enabled = false;
					if (modelList != null)
					{
						this.oldlistDoor = modelList;
						for (int i = 0; i < modelList.Count; i++)
						{
							int num = 0;
							while (num < this.m_doorDataTable.Rows.Count)
							{
								if (!(this.m_doorDataTable.Rows[num][0].ToString() == modelList[i].accdoor_id.ToString()))
								{
									num++;
									continue;
								}
								DataRow dataRow = this.m_browDoorDataTable.NewRow();
								dataRow[0] = this.m_doorDataTable.Rows[num][0].ToString();
								dataRow[1] = this.m_doorDataTable.Rows[num][1].ToString();
								dataRow[2] = this.m_doorDataTable.Rows[num][2].ToString();
								dataRow[3] = false;
								dataRow[4] = this.m_doorDataTable.Rows[num][4].ToString();
								dataRow[5] = this.m_doorDataTable.Rows[num][5].ToString();
								dataRow[6] = this.m_doorDataTable.Rows[num][6].ToString();
								this.m_browDoorDataTable.Rows.Add(dataRow);
								this.m_doorDataTable.Rows.RemoveAt(num);
								this.btn_doorLMove.Enabled = true;
								this.btn_allDoorLMove.Enabled = true;
								break;
							}
						}
					}
					else
					{
						this.btn_allDoorRMove.Enabled = false;
						this.btn_doorRMove.Enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_allUserRMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_dataTable.Rows.Count > 0)
				{
					this.MoveUserRow(this.m_dataTable, this.m_browDataTable, "");
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoUser", "没有人员信息"));
				}
				this.CheckUserBtn();
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
				if (this.m_dataTable.Rows.Count > 0)
				{
					this.MoveUserRow(this.m_dataTable, this.m_browDataTable, "check='true'");
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoUser", "没有人员信息"));
				}
				this.CheckUserBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_userLMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browDataTable.Rows.Count > 0)
				{
					this.MoveUserRow(this.m_browDataTable, this.m_dataTable, "check='true'");
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoUser", "没有人员信息"));
				}
				this.CheckUserBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_allUserLMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browDataTable.Rows.Count > 0)
				{
					this.MoveUserRow(this.m_browDataTable, this.m_dataTable, "");
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoUser", "没有人员信息"));
				}
				this.CheckUserBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void CheckUserBtn()
		{
			if (this.m_dataTable.Rows.Count > 0)
			{
				this.btn_userRMove.Enabled = true;
				this.btn_allUserRMove.Enabled = true;
			}
			else
			{
				this.btn_userRMove.Enabled = false;
				this.btn_allUserRMove.Enabled = false;
			}
			if (this.m_browDataTable.Rows.Count > 0)
			{
				this.btn_allUserLMove.Enabled = true;
				this.btn_userLMove.Enabled = true;
			}
			else
			{
				this.btn_allUserLMove.Enabled = false;
				this.btn_userLMove.Enabled = false;
			}
		}

		private void CheckDoorBtn()
		{
			if (this.m_doorDataTable.Rows.Count > 0)
			{
				this.btn_allDoorRMove.Enabled = true;
				this.btn_doorRMove.Enabled = true;
			}
			else
			{
				this.btn_allDoorRMove.Enabled = false;
				this.btn_doorRMove.Enabled = false;
			}
			if (this.m_browDoorDataTable.Rows.Count > 0)
			{
				this.btn_doorLMove.Enabled = true;
				this.btn_allDoorLMove.Enabled = true;
			}
			else
			{
				this.btn_doorLMove.Enabled = false;
				this.btn_allDoorLMove.Enabled = false;
			}
		}

		private void btn_allDoorRMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_doorDataTable.Rows.Count > 0)
				{
					List<DataRow> list = new List<DataRow>();
					List<DataRow> list2 = new List<DataRow>();
					DialogResult dialogResult = DialogResult.Cancel;
					List<DialogChoice> list3 = new List<DialogChoice>();
					MultiChoiceDialog multiChoiceDialog = new MultiChoiceDialog();
					list3.Add(new DialogChoice(1, ShowMsgInfos.GetInfo("ContinueAdd", "继续"), ShowMsgInfos.GetInfo("KnownAndContinue", "已了解并继续选择此门")));
					list3.Add(new DialogChoice(2, ShowMsgInfos.GetInfo("Skip", "跳过"), ShowMsgInfos.GetInfo("IgnornThisDoor", "不选择此门")));
					for (int i = 0; i < this.m_doorDataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_browDoorDataTable.NewRow();
						dataRow[0] = this.m_doorDataTable.Rows[i][0].ToString();
						dataRow[1] = this.m_doorDataTable.Rows[i][1].ToString();
						dataRow[2] = this.m_doorDataTable.Rows[i][2].ToString();
						dataRow[3] = false;
						dataRow[4] = this.m_doorDataTable.Rows[i][4].ToString();
						dataRow[5] = this.m_doorDataTable.Rows[i][5].ToString();
						dataRow[6] = this.m_doorDataTable.Rows[i][6].ToString();
						if (!int.TryParse(dataRow[5].ToString(), out int key))
						{
							list.Add(dataRow);
							list2.Add(this.m_doorDataTable.Rows[i]);
							continue;
						}
						if (!this.dicId_Machine.ContainsKey(key))
						{
							list.Add(dataRow);
							list2.Add(this.m_doorDataTable.Rows[i]);
							continue;
						}
						Machines machines = this.dicId_Machine[key];
						if (machines.DevSDKType != SDKType.StandaloneSDK)
						{
							list.Add(dataRow);
							list2.Add(this.m_doorDataTable.Rows[i]);
							continue;
						}
						dialogResult = DialogResult.OK;
						if (dialogResult != DialogResult.OK)
						{
							return;
						}
						switch (multiChoiceDialog.SelectedChoice)
						{
						case 1:
							list.Add(dataRow);
							list2.Add(this.m_doorDataTable.Rows[i]);
							break;
						default:
							list.Add(dataRow);
							list2.Add(this.m_doorDataTable.Rows[i]);
							break;
						case 2:
							break;
						}
					}
					for (int j = 0; j < list.Count; j++)
					{
						this.m_browDoorDataTable.Rows.Add(list[j]);
						if (!this.m_doorTempList.Contains(int.Parse(list[j][0].ToString())))
						{
							this.m_doorTempList.Add(int.Parse(list[j][0].ToString()));
						}
						else
						{
							int index = this.m_doorTempList.IndexOf(int.Parse(list[j][0].ToString()));
							this.m_doorTempList.RemoveAt(index);
						}
					}
					for (int k = 0; k < list2.Count; k++)
					{
						this.m_doorDataTable.Rows.Remove(list2[k]);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDoor", "没有门数据"));
				}
				this.CheckDoorBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.CheckStdTimeseg();
		}

		private void btn_doorRMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_doorDataTable.Rows.Count > 0)
				{
					int[] array = null;
					if (this.isDouble)
					{
						array = this.gridDoor.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.gridDoor, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.gridDoor, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_doorDataTable.Rows.Count)
					{
						List<DataRow> list = new List<DataRow>();
						List<DataRow> list2 = new List<DataRow>();
						List<DialogChoice> list3 = new List<DialogChoice>();
						MultiChoiceDialog multiChoiceDialog = new MultiChoiceDialog();
						list3.Add(new DialogChoice(1, ShowMsgInfos.GetInfo("ContinueAdd", "继续"), ShowMsgInfos.GetInfo("KnownAndContinue", "已了解并继续选择此门")));
						list3.Add(new DialogChoice(2, ShowMsgInfos.GetInfo("Skip", "跳过"), ShowMsgInfos.GetInfo("IgnornThisDoor", "不选择此门")));
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] >= 0 && array[i] < this.m_doorDataTable.Rows.Count)
							{
								DataRow dataRow = this.m_browDoorDataTable.NewRow();
								dataRow[0] = this.m_doorDataTable.Rows[array[i]][0].ToString();
								dataRow[1] = this.m_doorDataTable.Rows[array[i]][1].ToString();
								dataRow[2] = this.m_doorDataTable.Rows[array[i]][2].ToString();
								dataRow[3] = false;
								dataRow[4] = this.m_doorDataTable.Rows[array[i]][4].ToString();
								dataRow[5] = this.m_doorDataTable.Rows[array[i]][5].ToString();
								dataRow[6] = this.m_doorDataTable.Rows[array[i]][6].ToString();
								if (!int.TryParse(dataRow[5].ToString(), out int key))
								{
									list.Add(dataRow);
									list2.Add(this.m_doorDataTable.Rows[array[i]]);
									continue;
								}
								if (!this.dicId_Machine.ContainsKey(key))
								{
									list.Add(dataRow);
									list2.Add(this.m_doorDataTable.Rows[array[i]]);
									continue;
								}
								Machines machines = this.dicId_Machine[key];
								if (machines.DevSDKType != SDKType.StandaloneSDK)
								{
									list.Add(dataRow);
									list2.Add(this.m_doorDataTable.Rows[array[i]]);
									continue;
								}
								DialogResult dialogResult = DialogResult.OK;
								if (dialogResult == DialogResult.OK)
								{
									switch (multiChoiceDialog.SelectedChoice)
									{
									case 1:
										list.Add(dataRow);
										list2.Add(this.m_doorDataTable.Rows[array[i]]);
										break;
									default:
										list.Add(dataRow);
										list2.Add(this.m_doorDataTable.Rows[array[i]]);
										break;
									case 2:
										break;
									}
									continue;
								}
								return;
							}
						}
						for (int j = 0; j < list.Count; j++)
						{
							this.m_browDoorDataTable.Rows.Add(list[j]);
							if (!this.m_doorTempList.Contains(int.Parse(list[j][0].ToString())))
							{
								this.m_doorTempList.Add(int.Parse(list[j][0].ToString()));
							}
							else
							{
								int index = this.m_doorTempList.IndexOf(int.Parse(list[j][0].ToString()));
								this.m_doorTempList.RemoveAt(index);
							}
						}
						for (int k = 0; k < list2.Count; k++)
						{
							this.m_doorDataTable.Rows.Remove(list2[k]);
						}
						if (this.m_doorDataTable.Rows.Count > 0)
						{
							this.gridDoor.SelectRow(0);
						}
					}
					else if (!this.isDouble)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneDoorData", "请选择门"));
					}
				}
				else if (!this.isDouble)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDoor", "没有门数据"));
				}
				this.CheckDoorBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.CheckStdTimeseg();
		}

		private void btn_doorLMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browDoorDataTable.Rows.Count > 0)
				{
					int[] array = null;
					if (this.isDouble)
					{
						array = this.gridSelectedDoor.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.gridSelectedDoor, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.gridSelectedDoor, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_browDoorDataTable.Rows.Count)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] >= 0 && array[i] < this.m_browDoorDataTable.Rows.Count)
							{
								DataRow dataRow = this.m_doorDataTable.NewRow();
								dataRow[0] = this.m_browDoorDataTable.Rows[array[i]][0].ToString();
								dataRow[1] = this.m_browDoorDataTable.Rows[array[i]][1].ToString();
								dataRow[2] = this.m_browDoorDataTable.Rows[array[i]][2].ToString();
								dataRow[3] = false;
								dataRow[4] = this.m_browDoorDataTable.Rows[array[i]][4].ToString();
								dataRow[5] = this.m_browDoorDataTable.Rows[array[i]][5].ToString();
								dataRow[6] = this.m_browDoorDataTable.Rows[array[i]][6].ToString();
								this.m_doorDataTable.Rows.Add(dataRow);
								this.m_browDoorDataTable.Rows[array[i]][0] = -1;
								if (this.m_doorTempList.Contains(int.Parse(dataRow[0].ToString())))
								{
									int index = this.m_doorTempList.IndexOf(int.Parse(dataRow[0].ToString()));
									this.m_doorTempList.RemoveAt(index);
								}
								else
								{
									this.m_doorTempList.Add(int.Parse(dataRow[0].ToString()));
								}
							}
						}
						for (int j = 0; j < this.m_browDoorDataTable.Rows.Count; j++)
						{
							if (this.m_browDoorDataTable.Rows[j][0].ToString() == "-1")
							{
								this.m_browDoorDataTable.Rows.RemoveAt(j);
								j--;
							}
						}
						if (this.m_browDoorDataTable.Rows.Count > 0)
						{
							this.gridSelectedDoor.SelectRow(0);
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneDoorData", "请选择门"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDoor", "没有门数据"));
				}
				this.CheckDoorBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.CheckStdTimeseg();
		}

		private void btn_allDoorLMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browDoorDataTable.Rows.Count > 0)
				{
					for (int i = 0; i < this.m_browDoorDataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_doorDataTable.NewRow();
						dataRow[0] = this.m_browDoorDataTable.Rows[i][0].ToString();
						dataRow[1] = this.m_browDoorDataTable.Rows[i][1].ToString();
						dataRow[2] = this.m_browDoorDataTable.Rows[i][2].ToString();
						dataRow[3] = false;
						dataRow[4] = this.m_browDoorDataTable.Rows[i][4].ToString();
						dataRow[5] = this.m_browDoorDataTable.Rows[i][5].ToString();
						dataRow[6] = this.m_browDoorDataTable.Rows[i][6].ToString();
						this.m_doorDataTable.Rows.Add(dataRow);
						if (this.m_doorTempList.Contains(int.Parse(dataRow[0].ToString())))
						{
							int index = this.m_doorTempList.IndexOf(int.Parse(dataRow[0].ToString()));
							this.m_doorTempList.RemoveAt(index);
						}
						else
						{
							this.m_doorTempList.Add(int.Parse(dataRow[0].ToString()));
						}
					}
					this.m_browDoorDataTable.Rows.Clear();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDoor", "没有门数据"));
				}
				this.CheckDoorBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.CheckStdTimeseg();
		}

		private void MoveUserRow(DataTable dtSource, DataTable dtDest, string filter)
		{
			try
			{
				DataRow[] array = dtSource.Select(filter);
				if (array != null && array.Length != 0)
				{
					this.grd_view.DataSource = null;
					this.dgrd_selectedUser.DataSource = null;
					dtDest.BeginLoadData();
					dtSource.BeginLoadData();
					for (int i = 0; i < array.Length; i++)
					{
						DataRow dataRow = dtDest.NewRow();
						dataRow[0] = array[i][0];
						dataRow[1] = array[i][1];
						dataRow[2] = array[i][2];
						dataRow[3] = array[i][3];
						dataRow[4] = array[i][4];
						dataRow[5] = array[i][5];
						dataRow[6] = false;
						dtDest.Rows.Add(dataRow);
						dtSource.Rows.Remove(array[i]);
						if (this.m_userTempList.Contains(int.Parse(dataRow[0].ToString())))
						{
							int index = this.m_userTempList.IndexOf(int.Parse(dataRow[0].ToString()));
							this.m_userTempList.RemoveAt(index);
						}
						else
						{
							this.m_userTempList.Add(int.Parse(dataRow[0].ToString()));
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneUserData", "请选择人员"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			dtDest.EndLoadData();
			dtSource.EndLoadData();
			this.grd_view.DataSource = this.m_dataTable;
			this.dgrd_selectedUser.DataSource = this.m_browDataTable;
			this.CheckUserBtn();
		}

		private bool CheckData()
		{
			try
			{
				if (!string.IsNullOrEmpty(this.txt_levelName.Text.Trim()))
				{
					if (this.cbo_doorTime.SelectedIndex > 0 && this.timeIDList.ContainsKey(this.cbo_doorTime.SelectedIndex))
					{
						if (this.m_browDoorDataTable.Rows.Count > 0)
						{
							return true;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDoor", "请选择门"));
						return false;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSlectTimeZone", "请选择门禁时间段"));
					this.cbo_doorTime.Focus();
					return false;
				}
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputPrivelegeName", "请输入权限名称"));
				this.txt_levelName.Focus();
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool IsExists(int timeid, int levelid)
		{
			AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
			List<AccLevelset> modelList = accLevelsetBll.GetModelList("");
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (modelList[i].level_timeseg_id == timeid && levelid != modelList[i].id && this.glist.ContainsKey(modelList[i].id))
					{
						List<int> list = this.glist[modelList[i].id];
						if (list.Count == this.m_browDoorDataTable.Rows.Count)
						{
							bool flag = false;
							for (int j = 0; j < this.m_browDoorDataTable.Rows.Count; j++)
							{
								if (!list.Contains(int.Parse(this.m_browDoorDataTable.Rows[j][0].ToString())))
								{
									flag = true;
								}
							}
							if (!flag)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private void btn_allow_Click(object sender, EventArgs e)
		{
			if (this.CheckData())
			{
				if (!this.CheckStdTimeseg())
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("TimeZoneNotSuportSTDMachine", "当前时间段不能指定为脱机设备的时间段"));
				}
				else
				{
					this.DoorTimeIndex = this.cbo_doorTime.SelectedIndex;
					this.LevelNameText = this.txt_levelName.Text;
					this.LoadDoorGroup();
					if (this.timeIDList.ContainsKey(this.DoorTimeIndex))
					{
						if (this.IsExists(this.timeIDList[this.DoorTimeIndex], this.levelID))
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SDoorSetSameTime", "同一时段相同门设置已经存在"));
						}
						else
						{
							this.m_wait.ShowEx();
							Thread thread = new Thread(this.SaveLevelGroup);
							thread.Start();
						}
					}
					else
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SSelctTimeZOne", "请选择门禁时间段"));
					}
				}
			}
		}

		private void SaveLevelGroup()
		{
			try
			{
				this.SetWindowState(false);
				this.SetWindowCursor(Cursors.WaitCursor);
				AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
				AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
				AccLevelset accLevelset = null;
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				bool flag = false;
				bool flag2 = false;
				if (this.levelID > 0)
				{
					accLevelset = accLevelsetBll.GetModel(this.levelID);
				}
				if (accLevelset == null)
				{
					accLevelset = new AccLevelset();
					if (accLevelsetBll.Exists(this.LevelNameText))
					{
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.SetFocusToControl(this.txt_levelName);
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
						this.SetWindowCursor(Cursors.Default);
						this.SetWindowState(true);
						goto end_IL_0001;
					}
				}
				else if (accLevelset.level_name != this.LevelNameText)
				{
					flag = true;
					if (accLevelsetBll.Exists(this.LevelNameText))
					{
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.SetFocusToControl(this.txt_levelName);
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
						this.SetWindowState(true);
						goto end_IL_0001;
					}
				}
				accLevelset.level_name = this.LevelNameText;
				accLevelset.isVisitLevel = this.chkIsVisitLevel.Checked;
				if (this.timeIDList.ContainsKey(this.DoorTimeIndex))
				{
					if (accLevelset.level_timeseg_id != this.timeIDList[this.DoorTimeIndex])
					{
						flag2 = true;
					}
					accLevelset.level_timeseg_id = this.timeIDList[this.DoorTimeIndex];
				}
				if (((this.m_doorTempList.Count <= 0 && this.m_userTempList.Count <= 0 && !flag2) & flag) && accLevelset.id > 0 && accLevelsetBll.Update(accLevelset))
				{
					this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("FinishEx", "完成"));
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
					this.SetWindowCursor(Cursors.Default);
					if (this.refreshDataEvent != null)
					{
						if (base.InvokeRequired)
						{
							base.Invoke((MethodInvoker)delegate
							{
								this.refreshDataEvent(this, null);
							});
						}
						else
						{
							this.refreshDataEvent(this, null);
						}
					}
					this.SetWindowState(true);
					this.CloseThisForm();
				}
				else
				{
					if (this.levelID > 0)
					{
						accLevelsetDoorGroupBll.DeleteByAcclevelsetID(this.levelID);
						accLevelsetEmpBll.DeleteByAcclevelsetID(this.levelID);
					}
					bool flag3 = false;
					if (accLevelset.id > 0)
					{
						if (accLevelsetBll.Update(accLevelset))
						{
							flag3 = true;
						}
						else
						{
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
							this.m_wait.ShowProgress(100);
							this.m_wait.HideEx(false);
						}
					}
					else
					{
						try
						{
							if (accLevelsetBll.Add(accLevelset) > 0)
							{
								accLevelset.id = accLevelsetBll.GetMaxId() - 1;
								flag3 = true;
							}
						}
						catch
						{
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
							this.m_wait.ShowProgress(100);
							this.m_wait.HideEx(false);
						}
					}
					if (flag3 && accLevelset.id > 0)
					{
						this.m_wait.ShowProgress(10);
						List<AccLevelsetDoorGroup> list = new List<AccLevelsetDoorGroup>();
						this.m_browDoorDataTable.BeginLoadData();
						for (int i = 0; i < this.m_browDoorDataTable.Rows.Count; i++)
						{
							this.m_wait.ShowProgress(10 + 10 * i / this.m_browDoorDataTable.Rows.Count);
							if (this.m_browDoorDataTable.Rows[i][0] != null)
							{
								AccLevelsetDoorGroup accLevelsetDoorGroup = new AccLevelsetDoorGroup();
								accLevelsetDoorGroup.accdoor_id = int.Parse(this.m_browDoorDataTable.Rows[i][0].ToString());
								accLevelsetDoorGroup.acclevelset_id = accLevelset.id;
								accLevelsetDoorGroup.accdoor_device_id = int.Parse(this.m_browDoorDataTable.Rows[i][5].ToString());
								int num = 0;
								if (this.dicId_Machine.ContainsKey(accLevelsetDoorGroup.accdoor_device_id))
								{
									for (int j = 0; j < this.m_browDoorDataTable.Rows.Count; j++)
									{
										int num2 = int.Parse(this.m_browDoorDataTable.Rows[j][5].ToString());
										if (num2 == accLevelsetDoorGroup.accdoor_device_id)
										{
											num += 1 << int.Parse(this.m_browDoorDataTable.Rows[i][4].ToString()) - 1;
										}
									}
								}
								accLevelsetDoorGroup.accdoor_no_exp = num;
								accLevelsetDoorGroup.level_timeseg_id = accLevelset.level_timeseg_id;
								try
								{
									accLevelsetDoorGroupBll.Add(accLevelsetDoorGroup);
									list.Add(accLevelsetDoorGroup);
								}
								catch
								{
								}
							}
						}
						this.m_browDoorDataTable.EndLoadData();
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelDoorInfoSuccess", "保存门数据成功"));
						this.m_wait.ShowProgress(10);
						List<AccLevelsetEmp> list2 = new List<AccLevelsetEmp>();
						if (this.m_browDataTable.Rows.Count < 20)
						{
							for (int k = 0; k < this.m_browDataTable.Rows.Count; k++)
							{
								this.m_wait.ShowProgress(40 + 20 * k / this.m_browDataTable.Rows.Count);
								if (this.m_browDataTable.Rows[k][0] != null)
								{
									AccLevelsetEmp accLevelsetEmp = new AccLevelsetEmp();
									accLevelsetEmp.employee_id = int.Parse(this.m_browDataTable.Rows[k][0].ToString());
									accLevelsetEmp.acclevelset_id = accLevelset.id;
									try
									{
										accLevelsetEmpBll.Add(accLevelsetEmp);
										list2.Add(accLevelsetEmp);
									}
									catch
									{
									}
								}
							}
						}
						else
						{
							List<AccLevelsetEmp> list3 = new List<AccLevelsetEmp>();
							this.m_browDataTable.BeginLoadData();
							this.m_wait.ShowProgress(40);
							for (int l = 0; l < this.m_browDataTable.Rows.Count; l++)
							{
								if (this.m_browDataTable.Rows[l][0] != null)
								{
									AccLevelsetEmp accLevelsetEmp2 = new AccLevelsetEmp();
									accLevelsetEmp2.employee_id = int.Parse(this.m_browDataTable.Rows[l][0].ToString());
									accLevelsetEmp2.acclevelset_id = accLevelset.id;
									list3.Add(accLevelsetEmp2);
									list2.Add(accLevelsetEmp2);
								}
							}
							this.m_browDataTable.EndLoadData();
							try
							{
								if (!accLevelsetEmpBll.AccLevelsetEmp_Addnew(list3))
								{
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelUserInfoFail", "保存人员权限数据失败"));
								}
							}
							catch
							{
							}
						}
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelUserInfoSuccess", "保存人员权限数据成功"));
						this.m_wait.ShowProgress(50);
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
						Application.DoEvents();
						this.GenerateCommand(accLevelset, list, list2);
						this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoSuccess", "生成命令完成"));
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
						if (this.refreshDataEvent != null)
						{
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.refreshDataEvent(this, null);
								});
							}
							else
							{
								this.refreshDataEvent(this, null);
							}
						}
						if ((this.m_doorTempList.Count > 0 || this.m_userTempList.Count > 0) | flag2 | flag)
						{
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									FrmShowUpdata.Instance.ShowEx();
								});
							}
							else
							{
								FrmShowUpdata.Instance.ShowEx();
							}
						}
						this.SetWindowCursor(Cursors.Default);
						this.SetWindowState(true);
						this.CloseThisForm();
					}
					else
					{
						this.SetWindowCursor(Cursors.Default);
					}
				}
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				this.m_wait.ShowProgress(100);
				this.m_wait.HideEx(false);
				SysDialogs.ShowWarningMessage(ex.Message);
				this.SetWindowCursor(Cursors.Default);
			}
		}

		private void SetWindowCursor(Cursor cursor)
		{
			if (this != null && base.IsHandleCreated && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.SetWindowCursor(cursor);
					});
				}
				else
				{
					this.Cursor = cursor;
				}
			}
		}

		private void SetFocusToControl(Control ctrl)
		{
			if (ctrl.InvokeRequired)
			{
				ctrl.Invoke((MethodInvoker)delegate
				{
					this.SetFocusToControl(ctrl);
				});
			}
			else
			{
				ctrl.Focus();
			}
		}

		private void CloseThisForm()
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.CloseThisForm();
				});
			}
			else
			{
				base.Close();
			}
		}

		private void SetWindowState(bool enabled)
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.SetWindowState(enabled);
				});
			}
			else
			{
				foreach (Control control in base.Controls)
				{
					control.Enabled = enabled;
				}
			}
		}

		[Obsolete]
		private void AddCmd(AccLevelset model, List<AccLevelsetDoorGroup> newDoors, List<AccLevelsetEmp> newUsers)
		{
			int num = (this.oldLevel == null) ? model.level_timeseg_id : this.oldLevel.level_timeseg_id;
			num = ((DeviceHelper.TimeSeg.id == num) ? 1 : num);
			if (this.oldLevel != null && this.oldlistEmp != null && this.oldlistEmp.Count > 0 && this.oldlistDoor != null && this.oldlistDoor.Count > 0)
			{
				if (newDoors.Count > 0 && newUsers.Count > 0)
				{
					if (this.oldLevel.level_timeseg_id != model.level_timeseg_id)
					{
						if (this.m_doorTempList.Count <= 0 && this.m_userTempList.Count <= 0)
						{
							CommandServer.UserCmd(model, num, true, newDoors, newUsers, false);
						}
						else
						{
							CommandServer.DeleUserToCmd(this.oldLevel, this.oldlistEmp, this.oldlistDoor, this.oldlistDoor);
							CommandServer.UserCmd(model, num, true, newDoors, newUsers, true);
						}
					}
					else
					{
						List<AccLevelsetDoorGroup> list = new List<AccLevelsetDoorGroup>();
						List<int> list2 = new List<int>();
						for (int i = 0; i < this.oldlistDoor.Count; i++)
						{
							bool flag = false;
							int num2 = 0;
							while (num2 < newDoors.Count)
							{
								if (this.oldlistDoor[i].accdoor_id != newDoors[num2].accdoor_id)
								{
									num2++;
									continue;
								}
								flag = true;
								break;
							}
							if (!flag)
							{
								list.Add(this.oldlistDoor[i]);
							}
							else
							{
								list2.Add(this.oldlistDoor[i].accdoor_id);
							}
						}
						for (int j = 0; j < newDoors.Count; j++)
						{
							bool flag2 = false;
							int num3 = 0;
							while (num3 < this.oldlistDoor.Count)
							{
								if (newDoors[j].accdoor_id != this.oldlistDoor[num3].accdoor_id)
								{
									num3++;
									continue;
								}
								flag2 = true;
								break;
							}
							if (!flag2)
							{
								list.Add(newDoors[j]);
							}
							else if (!list2.Contains(newDoors[j].accdoor_id))
							{
								list2.Add(newDoors[j].accdoor_id);
							}
						}
						int num4;
						if (list.Count > 0)
						{
							CommandServer.DeleUserToCmd(this.oldLevel, this.oldlistEmp, list, this.oldlistDoor);
							CommandServer.UserCmd(model, num, true, list, newUsers, true);
							if (list2.Count > 0)
							{
								num4 = list[0].accdoor_id;
								string text = num4.ToString();
								for (int k = 1; k < list.Count; k++)
								{
									string str = text;
									num4 = list[k].accdoor_id;
									text = str + "," + num4.ToString();
								}
								num4 = list2[0];
								string text2 = num4.ToString();
								for (int l = 1; l < list2.Count; l++)
								{
									string str2 = text2;
									num4 = list2[l];
									text2 = str2 + "," + num4.ToString();
								}
								MachinesBll machinesBll = new MachinesBll(MainForm._ia);
								List<Machines> modelList = machinesBll.GetModelList("id in(select distinct device_id from acc_door where id in(" + text2 + ") and id not in(" + text + "))");
								if (modelList != null && modelList.Count > 0)
								{
									this.AddCmdEx(model, newUsers, modelList);
								}
							}
						}
						else
						{
							num4 = newDoors[0].accdoor_id;
							string text3 = num4.ToString();
							for (int m = 1; m < newDoors.Count; m++)
							{
								string str3 = text3;
								num4 = newDoors[m].accdoor_id;
								text3 = str3 + "," + num4.ToString();
							}
							MachinesBll machinesBll2 = new MachinesBll(MainForm._ia);
							List<Machines> modelList2 = machinesBll2.GetModelList("id in(select distinct device_id from acc_door where id in(" + text3 + "))");
							if (modelList2 != null && modelList2.Count > 0)
							{
								this.AddCmdEx(model, newUsers, modelList2);
							}
						}
					}
				}
				else
				{
					CommandServer.DeleUserToCmd(this.oldLevel, this.oldlistEmp, this.oldlistDoor, this.oldlistDoor);
				}
			}
			else if (newUsers.Count > 0 && newDoors.Count > 0)
			{
				CommandServer.UserCmd(model, num, true, newDoors, newUsers, true);
			}
		}

		private void GenerateCommand(AccLevelset model, List<AccLevelsetDoorGroup> newDoors, List<AccLevelsetEmp> newUsers)
		{
			AccTimeseg timeseg = (!this.dicId_Timeseg.ContainsKey(model.level_timeseg_id)) ? DeviceHelper.TimeSeg : this.dicId_Timeseg[model.level_timeseg_id];
			AccTimeseg accTimeseg = (this.oldLevel == null || !this.dicId_Timeseg.ContainsKey(this.oldLevel.level_timeseg_id)) ? null : this.dicId_Timeseg[this.oldLevel.level_timeseg_id];
			Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
			Dictionary<int, AccDoor> dictionary2 = new Dictionary<int, AccDoor>();
			Dictionary<int, UserInfo> dictionary3 = new Dictionary<int, UserInfo>();
			Dictionary<int, UserInfo> dictionary4 = new Dictionary<int, UserInfo>();
			Dictionary<int, Machines> dictionary5 = new Dictionary<int, Machines>();
			Dictionary<int, Machines> dictionary6 = new Dictionary<int, Machines>();
			Dictionary<int, Machines> dictionary7 = new Dictionary<int, Machines>();
			Dictionary<int, List<AccDoor>> dictionary8 = new Dictionary<int, List<AccDoor>>();
			Dictionary<int, List<AccDoor>> dictionary9 = new Dictionary<int, List<AccDoor>>();
			Dictionary<int, List<AccDoor>> dictionary10 = new Dictionary<int, List<AccDoor>>();
			Dictionary<int, List<AccDoor>> dictionary11 = new Dictionary<int, List<AccDoor>>();
			List<AccDoor> list = new List<AccDoor>();
			List<AccDoor> list2 = new List<AccDoor>();
			List<UserInfo> list3 = new List<UserInfo>();
			List<UserInfo> list4 = new List<UserInfo>();
			List<UserInfo> list5 = new List<UserInfo>();
			List<UserInfo> list6 = new List<UserInfo>();
			if (newDoors != null)
			{
				for (int i = 0; i < newDoors.Count; i++)
				{
					if (this.dicId_Door.ContainsKey(newDoors[i].accdoor_id) && !dictionary.ContainsKey(newDoors[i].accdoor_id))
					{
						dictionary.Add(newDoors[i].accdoor_id, this.dicId_Door[newDoors[i].accdoor_id]);
					}
					if (!dictionary5.ContainsKey(newDoors[i].accdoor_device_id) && this.dicId_Machine.ContainsKey(newDoors[i].accdoor_device_id))
					{
						Machines machines = this.dicId_Machine[newDoors[i].accdoor_device_id];
						dictionary5.Add(newDoors[i].accdoor_device_id, machines);
						if (!dictionary7.ContainsKey(machines.ID))
						{
							dictionary7.Add(machines.ID, machines);
						}
					}
					if (!dictionary8.ContainsKey(newDoors[i].accdoor_device_id) && this.dicId_Door.ContainsKey(newDoors[i].accdoor_id))
					{
						List<AccDoor> list7 = new List<AccDoor>();
						list7.Add(this.dicId_Door[newDoors[i].accdoor_id]);
						dictionary8.Add(newDoors[i].accdoor_device_id, list7);
					}
					else
					{
						List<AccDoor> list7 = dictionary8[newDoors[i].accdoor_device_id];
						list7.Add(this.dicId_Door[newDoors[i].accdoor_id]);
					}
				}
			}
			if (this.oldlistDoor != null)
			{
				for (int j = 0; j < this.oldlistDoor.Count; j++)
				{
					if (this.dicId_Door.ContainsKey(this.oldlistDoor[j].accdoor_id) && !dictionary2.ContainsKey(this.oldlistDoor[j].accdoor_id))
					{
						dictionary2.Add(this.oldlistDoor[j].accdoor_id, this.dicId_Door[this.oldlistDoor[j].accdoor_id]);
					}
					if (!dictionary6.ContainsKey(this.oldlistDoor[j].accdoor_device_id) && this.dicId_Machine.ContainsKey(this.oldlistDoor[j].accdoor_device_id))
					{
						Machines machines = this.dicId_Machine[this.oldlistDoor[j].accdoor_device_id];
						dictionary6.Add(this.oldlistDoor[j].accdoor_device_id, machines);
						if (!dictionary7.ContainsKey(machines.ID))
						{
							dictionary7.Add(machines.ID, machines);
						}
					}
					if (!dictionary9.ContainsKey(this.oldlistDoor[j].accdoor_device_id) && this.dicId_Door.ContainsKey(this.oldlistDoor[j].accdoor_id))
					{
						List<AccDoor> list7 = new List<AccDoor>();
						list7.Add(this.dicId_Door[this.oldlistDoor[j].accdoor_id]);
						dictionary9.Add(this.oldlistDoor[j].accdoor_device_id, list7);
					}
					else
					{
						List<AccDoor> list7 = dictionary9[this.oldlistDoor[j].accdoor_device_id];
						list7.Add(this.dicId_Door[this.oldlistDoor[j].accdoor_id]);
					}
				}
			}
			if (newUsers != null)
			{
				for (int k = 0; k < newUsers.Count; k++)
				{
					if (this.dicId_User.ContainsKey(newUsers[k].employee_id) && !dictionary3.ContainsKey(newUsers[k].employee_id))
					{
						dictionary3.Add(newUsers[k].employee_id, this.dicId_User[newUsers[k].employee_id]);
					}
					if (this.dicId_User.ContainsKey(newUsers[k].employee_id))
					{
						list5.Add(this.dicId_User[newUsers[k].employee_id]);
					}
				}
			}
			if (this.oldlistEmp != null)
			{
				for (int l = 0; l < this.oldlistEmp.Count; l++)
				{
					if (this.dicId_User.ContainsKey(this.oldlistEmp[l].employee_id) && !dictionary4.ContainsKey(this.oldlistEmp[l].employee_id))
					{
						dictionary4.Add(this.oldlistEmp[l].employee_id, this.dicId_User[this.oldlistEmp[l].employee_id]);
					}
					if (this.dicId_User.ContainsKey(this.oldlistEmp[l].employee_id))
					{
						list6.Add(this.dicId_User[this.oldlistEmp[l].employee_id]);
					}
				}
			}
			foreach (KeyValuePair<int, AccDoor> item in dictionary)
			{
				if (!dictionary2.ContainsKey(item.Value.id))
				{
					list.Add(item.Value);
					if (!dictionary10.ContainsKey(item.Value.device_id))
					{
						List<AccDoor> list7 = new List<AccDoor>();
						list7.Add(item.Value);
						dictionary10.Add(item.Value.device_id, list7);
					}
					else
					{
						List<AccDoor> list7 = dictionary10[item.Value.device_id];
						list7.Add(item.Value);
					}
				}
			}
			foreach (KeyValuePair<int, AccDoor> item2 in dictionary2)
			{
				if (!dictionary.ContainsKey(item2.Value.id))
				{
					list2.Add(item2.Value);
					if (!dictionary11.ContainsKey(item2.Value.device_id))
					{
						List<AccDoor> list7 = new List<AccDoor>();
						list7.Add(item2.Value);
						dictionary11.Add(item2.Value.device_id, list7);
					}
					else
					{
						List<AccDoor> list7 = dictionary11[item2.Value.device_id];
						list7.Add(item2.Value);
					}
				}
			}
			foreach (KeyValuePair<int, UserInfo> item3 in dictionary3)
			{
				if (!dictionary4.ContainsKey(item3.Value.UserId))
				{
					list3.Add(item3.Value);
				}
			}
			foreach (KeyValuePair<int, UserInfo> item4 in dictionary4)
			{
				if (!dictionary3.ContainsKey(item4.Value.UserId))
				{
					list4.Add(item4.Value);
				}
			}
			AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
			List<AccMorecardempGroup> modelList = accMorecardempGroupBll.GetModelList("");
			modelList = (modelList ?? new List<AccMorecardempGroup>());
			Dictionary<int, AccMorecardempGroup> dictionary12 = new Dictionary<int, AccMorecardempGroup>();
			for (int m = 0; m < modelList.Count; m++)
			{
				if (!dictionary12.ContainsKey(modelList[m].id))
				{
					dictionary12.Add(modelList[m].id, modelList[m]);
				}
			}
			foreach (KeyValuePair<int, Machines> item5 in dictionary7)
			{
				Machines machines = item5.Value;
				if ((this.oldLevel == null || this.oldLevel.level_timeseg_id != model.level_timeseg_id) && this.dicId_Timeseg.ContainsKey(model.level_timeseg_id))
				{
					AccTimeseg timeseg2 = this.dicId_Timeseg[model.level_timeseg_id];
					CommandServer.AddTimeZone(machines, timeseg2);
				}
				if (list.Count <= 0 && list3.Count <= 0 && list2.Count <= 0 && list4.Count <= 0)
				{
					if (this.oldLevel != null && this.oldLevel.level_timeseg_id != model.level_timeseg_id)
					{
						if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
						{
							CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
						}
						CommandServer.AddUserAuthorize(machines, timeseg, list6, dictionary8[machines.ID]);
					}
				}
				else
				{
					List<int> list8 = new List<int>();
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
					List<AccLevelsetEmp> modelList2 = accLevelsetEmpBll.GetModelList("acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id = " + machines.ID + " and acclevelset_id <> " + model.id + ")");
					if (modelList2 != null && modelList2.Count > 0)
					{
						for (int n = 0; n < modelList2.Count; n++)
						{
							list8.Add(modelList2[n].employee_id);
						}
					}
					List<UserInfo> list9 = new List<UserInfo>();
					List<UserVerifyType> list10 = new List<UserVerifyType>();
					if (!dictionary9.ContainsKey(machines.ID))
					{
						for (int num = 0; num < list5.Count; num++)
						{
							if (!list8.Contains(list5[num].UserId))
							{
								list9.Add(list5[num]);
								UserVerifyType userVerifyType = new UserVerifyType();
								userVerifyType.Pin = int.Parse(list5[num].BadgeNumber);
								userVerifyType.VerifyType = 0;
								userVerifyType.UseGroupVT = true;
								list10.Add(userVerifyType);
							}
						}
					}
					else if (dictionary8.ContainsKey(machines.ID))
					{
						for (int num2 = 0; num2 < list3.Count; num2++)
						{
							if (!list8.Contains(list3[num2].UserId))
							{
								list9.Add(list3[num2]);
								UserVerifyType userVerifyType = new UserVerifyType();
								userVerifyType.Pin = int.Parse(list3[num2].BadgeNumber);
								userVerifyType.VerifyType = 0;
								userVerifyType.UseGroupVT = true;
								userVerifyType.GroupNo = 1;
								if (dictionary12.ContainsKey(list3[num2].MorecardGroupId))
								{
									AccMorecardempGroup accMorecardempGroup = dictionary12[list3[num2].MorecardGroupId];
									if (accMorecardempGroup.StdGroupNo > 0)
									{
										userVerifyType.GroupNo = accMorecardempGroup.StdGroupNo;
									}
								}
								list10.Add(userVerifyType);
							}
						}
					}
					List<UserInfo> list11 = new List<UserInfo>();
					if (!dictionary8.ContainsKey(machines.ID))
					{
						for (int num3 = 0; num3 < list6.Count; num3++)
						{
							if (!list8.Contains(list6[num3].UserId))
							{
								list11.Add(list6[num3]);
							}
						}
					}
					else if (dictionary9.ContainsKey(machines.ID))
					{
						for (int num4 = 0; num4 < list4.Count; num4++)
						{
							if (!list8.Contains(list4[num4].UserId))
							{
								list11.Add(list4[num4]);
							}
						}
					}
					if (dictionary10.ContainsKey(machines.ID))
					{
						if (dictionary9.ContainsKey(machines.ID))
						{
							if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
							{
								CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
							}
							CommandServer.AddUserAuthorize(machines, timeseg, list6, dictionary8[machines.ID]);
						}
						else if (!flag)
						{
							CommandServer.AddUser(machines, list9, dictionary12);
							CommandServer.AddUserPhoto(machines, list9);
							CommandServer.AddUserVerifyType(machines, list10);
							CommandServer.AddFingerPrint(machines, list9);
							CommandServer.AddFingerVein(machines, list9);
							CommandServer.AddFace(machines, list9);
							if (accTimeseg != null && dictionary9.ContainsKey(machines.ID))
							{
								CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
							}
							CommandServer.AddUserAuthorize(machines, timeseg, list5, dictionary8[machines.ID]);
							flag = true;
						}
						flag3 = true;
					}
					string text;
					if (dictionary11.ContainsKey(machines.ID))
					{
						if (dictionary8.ContainsKey(machines.ID))
						{
							if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
							{
								CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
							}
							CommandServer.AddUserAuthorize(machines, timeseg, list6, dictionary8[machines.ID]);
						}
						else if (!flag2)
						{
							DevCmds.RunImmediatelyEnabled = true;
							if (machines.DevSDKType != SDKType.StandaloneSDK)
							{
								if (accTimeseg != null && dictionary9.ContainsKey(machines.ID))
								{
									CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
								}
								CommandServer.DeleteFingerPrint(machines, list11, true, out text);
								CommandServer.DeleteFingerVein(machines, list11, true, out text);
								CommandServer.DeleteFace(machines, list11, true, out text);
							}
							CommandServer.DeleteUser(machines, list11, true, out text);
							flag2 = true;
							DevCmds.RunImmediatelyEnabled = false;
						}
						flag3 = true;
					}
					if (list9.Count > 0 && !flag)
					{
						CommandServer.AddUser(machines, list9, dictionary12);
						CommandServer.AddUserPhoto(machines, list9);
						CommandServer.AddUserVerifyType(machines, list10);
						CommandServer.AddFingerPrint(machines, list9);
						CommandServer.AddFingerVein(machines, list9);
						CommandServer.AddFace(machines, list9);
						if (!flag3 && model.level_timeseg_id > 0 && (this.oldLevel == null || this.oldLevel.level_timeseg_id != model.level_timeseg_id))
						{
							if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
							{
								CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
							}
							CommandServer.AddUserAuthorize(machines, timeseg, list5, dictionary8[machines.ID]);
							flag3 = true;
						}
						else
						{
							CommandServer.AddUserAuthorize(machines, timeseg, list3, dictionary8[machines.ID]);
						}
						flag = true;
					}
					if (list11.Count > 0 && !flag2)
					{
						if (!flag3 && model.level_timeseg_id > 0 && (this.oldLevel == null || this.oldLevel.level_timeseg_id != model.level_timeseg_id))
						{
							if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
							{
								CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
							}
							CommandServer.AddUserAuthorize(machines, timeseg, list5, dictionary8[machines.ID]);
							flag3 = true;
						}
						else
						{
							DevCmds.RunImmediatelyEnabled = true;
							if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
							{
								CommandServer.DeleteUserAuthorize(machines, accTimeseg, list11, dictionary9[machines.ID]);
							}
							DevCmds.RunImmediatelyEnabled = false;
						}
						DevCmds.RunImmediatelyEnabled = true;
						if (machines.DevSDKType != SDKType.StandaloneSDK)
						{
							CommandServer.DeleteFingerPrint(machines, list11, true, out text);
							CommandServer.DeleteFingerVein(machines, list11, true, out text);
							CommandServer.DeleteFace(machines, list11, true, out text);
						}
						CommandServer.DeleteUser(machines, list11, true, out text);
						flag2 = true;
						DevCmds.RunImmediatelyEnabled = false;
					}
					if (!flag3)
					{
						if (list3.Count != list9.Count)
						{
							List<UserInfo> list12 = new List<UserInfo>();
							for (int num5 = 0; num5 < list3.Count; num5++)
							{
								if (!list9.Contains(list3[num5]) && !list12.Contains(list3[num5]))
								{
									list12.Add(list3[num5]);
								}
							}
							CommandServer.AddUserAuthorize(machines, timeseg, list12, dictionary8[machines.ID]);
						}
						if (list4.Count != list11.Count)
						{
							List<UserInfo> list12 = new List<UserInfo>();
							for (int num6 = 0; num6 < list4.Count; num6++)
							{
								if (!list11.Contains(list4[num6]) && !list12.Contains(list4[num6]))
								{
									list12.Add(list4[num6]);
								}
							}
							if (machines.DevSDKType != SDKType.StandaloneSDK)
							{
								if (accTimeseg != null && dictionary9.ContainsKey(machines.ID))
								{
									CommandServer.DeleteUserAuthorize(machines, accTimeseg, list12, dictionary9[machines.ID]);
								}
							}
							else
							{
								AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
								List<AccLevelset> modelList3 = accLevelsetBll.GetModelList("id <> " + model.id + " and id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + machines.ID + ")");
								if (modelList3 != null && modelList3.Count > 0 && this.dicId_Timeseg.ContainsKey(modelList3[modelList3.Count - 1].level_timeseg_id))
								{
									AccTimeseg timeseg3 = this.dicId_Timeseg[modelList3[modelList3.Count - 1].level_timeseg_id];
									CommandServer.AddUserAuthorize(machines, timeseg3, list12, dictionary8[machines.ID]);
								}
							}
						}
						flag3 = true;
					}
					if (!flag3 && model.level_timeseg_id > 0 && (this.oldLevel == null || this.oldLevel.level_timeseg_id != model.level_timeseg_id))
					{
						if (machines.DevSDKType != SDKType.StandaloneSDK && accTimeseg != null && dictionary9.ContainsKey(machines.ID))
						{
							CommandServer.DeleteUserAuthorize(machines, accTimeseg, list6, dictionary9[machines.ID]);
						}
						CommandServer.AddUserAuthorize(machines, timeseg, list5, dictionary8[machines.ID]);
						flag3 = true;
					}
				}
			}
		}

		private bool findPlan(AccLevelsetEmp p)
		{
			return p.employee_id.Equals(this.PlanID);
		}

		private void AddCmdEx(AccLevelset model, List<AccLevelsetEmp> newUsers, List<Machines> dlist)
		{
			int num = (this.oldLevel == null) ? model.level_timeseg_id : this.oldLevel.level_timeseg_id;
			num = ((DeviceHelper.TimeSeg.id == num) ? 1 : num);
			List<AccLevelsetEmp> list = new List<AccLevelsetEmp>();
			List<AccLevelsetEmp> list2 = new List<AccLevelsetEmp>();
			for (int i = 0; i < this.oldlistEmp.Count; i++)
			{
				this.PlanID = this.oldlistEmp[i].employee_id;
				int num2 = newUsers.FindIndex(this.findPlan);
				if (num2 < 0)
				{
					list.Add(this.oldlistEmp[i]);
				}
			}
			for (int j = 0; j < newUsers.Count; j++)
			{
				this.PlanID = newUsers[j].employee_id;
				int num3 = this.oldlistEmp.FindIndex(this.findPlan);
				if (num3 < 0)
				{
					list2.Add(newUsers[j]);
				}
			}
			if (dlist != null && list.Count > 0)
			{
				CommandServer.UserCmd(model, num, false, dlist, list, true);
			}
			if (dlist != null && list2.Count > 0)
			{
				CommandServer.UserCmd(model, num, true, dlist, list2, true);
			}
		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_levelName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void gridDoor_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void gridDoor_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void gridDoor_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void gridDoor_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_doorRMove_Click(sender, e);
			this.isDouble = false;
		}

		private void gridUser_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			try
			{
				DataRow focusedDataRow = this.gridUser.GetFocusedDataRow();
				if (focusedDataRow != null)
				{
					DataRow dataRow = this.m_browDataTable.NewRow();
					dataRow[0] = focusedDataRow[0];
					dataRow[1] = focusedDataRow[1];
					dataRow[2] = focusedDataRow[2];
					dataRow[3] = focusedDataRow[3];
					dataRow[4] = focusedDataRow[4];
					dataRow[5] = focusedDataRow[5];
					dataRow[6] = false;
					this.m_browDataTable.Rows.Add(dataRow);
					this.m_dataTable.Rows.Remove(focusedDataRow);
					if (this.m_userTempList.Contains(int.Parse(dataRow[0].ToString())))
					{
						int index = this.m_userTempList.IndexOf(int.Parse(dataRow[0].ToString()));
						this.m_userTempList.RemoveAt(index);
					}
					else
					{
						this.m_userTempList.Add(int.Parse(dataRow[0].ToString()));
					}
					goto end_IL_0008;
				}
				return;
				end_IL_0008:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
			this.isDouble = false;
			this.CheckUserBtn();
		}

		private void gridSelectedDoor_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_doorLMove_Click(sender, e);
			this.isDouble = false;
		}

		private void gridSelectedUser_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			try
			{
				DataRow focusedDataRow = this.gridSelectedUser.GetFocusedDataRow();
				if (focusedDataRow != null)
				{
					DataRow dataRow = this.m_dataTable.NewRow();
					dataRow[0] = focusedDataRow[0];
					dataRow[1] = focusedDataRow[1];
					dataRow[2] = focusedDataRow[2];
					dataRow[3] = focusedDataRow[3];
					dataRow[4] = focusedDataRow[4];
					dataRow[5] = focusedDataRow[5];
					dataRow[6] = false;
					this.m_dataTable.Rows.Add(dataRow);
					this.m_browDataTable.Rows.Remove(focusedDataRow);
					if (this.m_userTempList.Contains(int.Parse(dataRow[0].ToString())))
					{
						int index = this.m_userTempList.IndexOf(int.Parse(dataRow[0].ToString()));
						this.m_userTempList.RemoveAt(index);
					}
					else
					{
						this.m_userTempList.Add(int.Parse(dataRow[0].ToString()));
					}
					goto end_IL_0008;
				}
				return;
				end_IL_0008:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
			this.isDouble = false;
			this.CheckUserBtn();
		}

		private void time_load_Tick(object sender, EventArgs e)
		{
			this.time_load.Enabled = false;
			this.LoadUserInfo("");
			this.DataBind();
			this.time_load.Enabled = false;
			this.Cursor = Cursors.Default;
		}

		private void AddLevelForm_Load(object sender, EventArgs e)
		{
			this.time_load.Enabled = true;
			this.Cursor = Cursors.WaitCursor;
		}

		private void txt_levelName_TextChanged(object sender, EventArgs e)
		{
			this.CheckDoorBtn();
		}

		private void cbo_doorTime_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.CheckDoorBtn();
			this.CheckStdTimeseg();
		}

		private void gridSelectedDoor_RowStyle(object sender, RowStyleEventArgs e)
		{
			SDKType sDKType = SDKType.Undefined;
			GridView gridView = sender as GridView;
			if (e.RowHandle >= 0)
			{
				DataRow dataRow = gridView.GetDataRow(e.RowHandle);
				try
				{
					sDKType = (SDKType)(int)dataRow[6];
				}
				catch
				{
				}
				if (sDKType == SDKType.StandaloneSDK)
				{
					e.Appearance.Options.UseBackColor = true;
					e.Appearance.BackColor = Color.Yellow;
					e.Appearance.BackColor2 = Color.Yellow;
				}
			}
		}

		private void gridDoor_RowStyle(object sender, RowStyleEventArgs e)
		{
			SDKType sDKType = SDKType.Undefined;
			GridView gridView = sender as GridView;
			if (e.RowHandle >= 0)
			{
				DataRow dataRow = gridView.GetDataRow(e.RowHandle);
				try
				{
					sDKType = (SDKType)(int)dataRow[6];
				}
				catch
				{
				}
				if (sDKType == SDKType.StandaloneSDK)
				{
					e.Appearance.Options.UseBackColor = true;
					e.Appearance.BackColor = Color.Yellow;
					e.Appearance.BackColor2 = Color.Yellow;
				}
			}
		}

		private bool CheckStdTimeseg()
		{
			if (this.cbo_doorTime.SelectedIndex <= 0 || !this.timeIDList.ContainsKey(this.cbo_doorTime.SelectedIndex))
			{
				this.errorProvider1.SetError(this.cbo_doorTime, "");
				return true;
			}
			int key = this.timeIDList[this.cbo_doorTime.SelectedIndex];
			if (!this.dicId_Timeseg.ContainsKey(key))
			{
				this.errorProvider1.SetError(this.cbo_doorTime, "");
				return true;
			}
			AccTimeseg accTimeseg = this.dicId_Timeseg[key];
			if (accTimeseg.TimeZone1Id <= 0 && accTimeseg.TimeZone2Id <= 0 && accTimeseg.TimeZone2Id <= 0 && accTimeseg.TimeZoneHolidayId <= 0)
			{
				bool flag = false;
				for (int i = 0; i < this.m_browDoorDataTable.Rows.Count; i++)
				{
					DataRow dataRow = this.m_browDoorDataTable.Rows[i];
					int.TryParse(dataRow[6].ToString(), out int num);
					if (num == 2)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.errorProvider1.SetError(this.cbo_doorTime, ShowMsgInfos.GetInfo("TimeZoneNotSuportSTDMachine", "当前时间段不能指定为脱机设备的时间段"));
					return false;
				}
				this.errorProvider1.SetError(this.cbo_doorTime, "");
			}
			else
			{
				this.errorProvider1.SetError(this.cbo_doorTime, "");
			}
			return true;
		}

		private void btn_help_info_Click(object sender, EventArgs e)
		{
			string msg = "Importante: A função perfil de acesso, para os equipamentos SDT SDK(Como por exemplo: Duo SS 210, Duo SS 220,\nBio Inox Plus SS 311, SS 420, Duo SS 230, SS 610, SS 710), funciona como a função “Grupo de acesso”, disponível\nna configuração direta pelo dispositivo stand alone. \nDe modo que, não será permitido que o usuário possua mais de um perfil de acesso associado ao seu cadastro.\nCaso isso ocorra, o último perfil de acesso cadastrado, sobrescreverá o perfil de acesso criado anteriormente. \nEquipamentos Pull SDK, tais como(SS 320, SS 320 MF, SS 411E e a linha de controladoras CT 500), não possuem esta restrição.";
			Mensage mensage = new Mensage(680, 160, "Mensagem", msg, true, false, 0);
			mensage.Show();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddLevelForm));
			this.btn_exit = new ButtonX();
			this.btn_allow = new ButtonX();
			this.btn_allDoorLMove = new ButtonX();
			this.btn_doorLMove = new ButtonX();
			this.btn_doorRMove = new ButtonX();
			this.btn_allDoorRMove = new ButtonX();
			this.txt_levelName = new TextBox();
			this.lab_doorTime = new Label();
			this.lab_levelName = new Label();
			this.btn_allUserLMove = new ButtonX();
			this.btn_userLMove = new ButtonX();
			this.btn_userRMove = new ButtonX();
			this.btn_allUserRMove = new ButtonX();
			this.grd_view = new GridControl();
			this.gridUser = new GridView();
			this.column_checkuser = new GridColumn();
			this.column_deptName = new GridColumn();
			this.column_Badgenumber = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_cardNO = new GridColumn();
			this.dgrd_selectedDoor = new GridControl();
			this.gridSelectedDoor = new GridView();
			this.column_checkdoor = new GridColumn();
			this.column_selectedDoor = new GridColumn();
			this.column_selectedDevice = new GridColumn();
			this.dgrd_selectedUser = new GridControl();
			this.gridSelectedUser = new GridView();
			this.column_checkSelectUser = new GridColumn();
			this.column_selectedDept = new GridColumn();
			this.column_selectedPersonnel = new GridColumn();
			this.column_selectedName = new GridColumn();
			this.column_selectedLastName = new GridColumn();
			this.column_selectedCard = new GridColumn();
			this.lb_doors = new Label();
			this.lb_selectDoors = new Label();
			this.lb_users = new Label();
			this.lb_selectUsers = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.re2 = new RepositoryItemCheckEdit();
			this.gridDoor = new GridView();
			this.column_check = new GridColumn();
			this.column_door = new GridColumn();
			this.column_device = new GridColumn();
			this.grid_door = new GridControl();
			this.cbo_doorTime = new ComboBox();
			this.time_load = new System.Windows.Forms.Timer(this.components);
			this.errorProvider1 = new ErrorProvider(this.components);
			this.label3 = new Label();
			this.chkIsVisitLevel = new CheckBox();
			this.btn_help_info = new Button();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.gridUser).BeginInit();
			((ISupportInitialize)this.dgrd_selectedDoor).BeginInit();
			((ISupportInitialize)this.gridSelectedDoor).BeginInit();
			((ISupportInitialize)this.dgrd_selectedUser).BeginInit();
			((ISupportInitialize)this.gridSelectedUser).BeginInit();
			((ISupportInitialize)this.re2).BeginInit();
			((ISupportInitialize)this.gridDoor).BeginInit();
			((ISupportInitialize)this.grid_door).BeginInit();
			((ISupportInitialize)this.errorProvider1).BeginInit();
			base.SuspendLayout();
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(858, 628);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(82, 25);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 11;
			this.btn_exit.Text = "Cancelar";
			this.btn_exit.Click += this.btn_exit_Click;
			this.btn_allow.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allow.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allow.Location = new Point(770, 628);
			this.btn_allow.Name = "btn_allow";
			this.btn_allow.Size = new Size(82, 25);
			this.btn_allow.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allow.TabIndex = 10;
			this.btn_allow.Text = "OK";
			this.btn_allow.Click += this.btn_allow_Click;
			this.btn_allDoorLMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allDoorLMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allDoorLMove.Location = new Point(453, 267);
			this.btn_allDoorLMove.Name = "btn_allDoorLMove";
			this.btn_allDoorLMove.Size = new Size(50, 25);
			this.btn_allDoorLMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allDoorLMove.TabIndex = 5;
			this.btn_allDoorLMove.Text = "<<";
			this.btn_allDoorLMove.Click += this.btn_allDoorLMove_Click;
			this.btn_doorLMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_doorLMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_doorLMove.Location = new Point(453, 221);
			this.btn_doorLMove.Name = "btn_doorLMove";
			this.btn_doorLMove.Size = new Size(50, 25);
			this.btn_doorLMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_doorLMove.TabIndex = 4;
			this.btn_doorLMove.Text = "<";
			this.btn_doorLMove.Click += this.btn_doorLMove_Click;
			this.btn_doorRMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_doorRMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_doorRMove.Location = new Point(453, 174);
			this.btn_doorRMove.Name = "btn_doorRMove";
			this.btn_doorRMove.Size = new Size(50, 25);
			this.btn_doorRMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_doorRMove.TabIndex = 3;
			this.btn_doorRMove.Text = ">";
			this.btn_doorRMove.Click += this.btn_doorRMove_Click;
			this.btn_allDoorRMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allDoorRMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allDoorRMove.Location = new Point(453, 127);
			this.btn_allDoorRMove.Name = "btn_allDoorRMove";
			this.btn_allDoorRMove.Size = new Size(50, 25);
			this.btn_allDoorRMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allDoorRMove.TabIndex = 2;
			this.btn_allDoorRMove.Text = ">>";
			this.btn_allDoorRMove.Click += this.btn_allDoorRMove_Click;
			this.txt_levelName.Location = new Point(168, 17);
			this.txt_levelName.Name = "txt_levelName";
			this.txt_levelName.Size = new Size(170, 20);
			this.txt_levelName.TabIndex = 0;
			this.txt_levelName.Click += this.btn_help_info_Click;
			this.txt_levelName.TextChanged += this.txt_levelName_TextChanged;
			this.txt_levelName.KeyPress += this.txt_levelName_KeyPress;
			this.lab_doorTime.Location = new Point(543, 21);
			this.lab_doorTime.Name = "lab_doorTime";
			this.lab_doorTime.Size = new Size(150, 16);
			this.lab_doorTime.TabIndex = 33;
			this.lab_doorTime.Text = "Zona de Tempo";
			this.lab_doorTime.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_levelName.Location = new Point(12, 21);
			this.lab_levelName.Name = "lab_levelName";
			this.lab_levelName.Size = new Size(150, 16);
			this.lab_levelName.TabIndex = 32;
			this.lab_levelName.Text = "Perfil de Acesso";
			this.lab_levelName.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_allUserLMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allUserLMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allUserLMove.Location = new Point(453, 535);
			this.btn_allUserLMove.Name = "btn_allUserLMove";
			this.btn_allUserLMove.Size = new Size(50, 25);
			this.btn_allUserLMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allUserLMove.TabIndex = 9;
			this.btn_allUserLMove.Text = "<<";
			this.btn_allUserLMove.Click += this.btn_allUserLMove_Click;
			this.btn_userLMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_userLMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_userLMove.Location = new Point(453, 481);
			this.btn_userLMove.Name = "btn_userLMove";
			this.btn_userLMove.Size = new Size(50, 25);
			this.btn_userLMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_userLMove.TabIndex = 8;
			this.btn_userLMove.Text = "<";
			this.btn_userLMove.Click += this.btn_userLMove_Click;
			this.btn_userRMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_userRMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_userRMove.Location = new Point(453, 429);
			this.btn_userRMove.Name = "btn_userRMove";
			this.btn_userRMove.Size = new Size(50, 25);
			this.btn_userRMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_userRMove.TabIndex = 7;
			this.btn_userRMove.Text = ">";
			this.btn_userRMove.Click += this.btn_userRMove_Click;
			this.btn_allUserRMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allUserRMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allUserRMove.Location = new Point(453, 376);
			this.btn_allUserRMove.Name = "btn_allUserRMove";
			this.btn_allUserRMove.Size = new Size(50, 25);
			this.btn_allUserRMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allUserRMove.TabIndex = 6;
			this.btn_allUserRMove.Text = ">>";
			this.btn_allUserRMove.Click += this.btn_allUserRMove_Click;
			this.grd_view.Cursor = Cursors.Default;
			this.grd_view.Location = new Point(14, 347);
			this.grd_view.MainView = this.gridUser;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(427, 259);
			this.grd_view.TabIndex = 57;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridUser
			});
			this.gridUser.Columns.AddRange(new GridColumn[6]
			{
				this.column_checkuser,
				this.column_deptName,
				this.column_Badgenumber,
				this.column_name,
				this.column_lastName,
				this.column_cardNO
			});
			this.gridUser.GridControl = this.grd_view;
			this.gridUser.IndicatorWidth = 35;
			this.gridUser.Name = "gridUser";
			this.gridUser.OptionsSelection.MultiSelect = true;
			this.gridUser.OptionsView.ShowGroupPanel = false;
			this.gridUser.SortInfo.AddRange(new GridColumnSortInfo[1]
			{
				new GridColumnSortInfo(this.column_Badgenumber, ColumnSortOrder.Ascending)
			});
			this.gridUser.CustomDrawColumnHeader += this.gridDoor_CustomDrawColumnHeader;
			this.gridUser.CustomDrawCell += this.gridDoor_CustomDrawCell;
			this.gridUser.Click += this.gridDoor_Click;
			this.gridUser.DoubleClick += this.gridUser_DoubleClick;
			this.column_checkuser.Name = "column_checkuser";
			this.column_checkuser.Visible = true;
			this.column_checkuser.VisibleIndex = 0;
			this.column_deptName.Caption = "部门名称";
			this.column_deptName.Name = "column_deptName";
			this.column_deptName.Visible = true;
			this.column_deptName.VisibleIndex = 5;
			this.column_deptName.Width = 121;
			this.column_Badgenumber.AppearanceCell.Options.UseTextOptions = true;
			this.column_Badgenumber.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			this.column_Badgenumber.Caption = "人员编号";
			this.column_Badgenumber.Name = "column_Badgenumber";
			this.column_Badgenumber.Visible = true;
			this.column_Badgenumber.VisibleIndex = 1;
			this.column_Badgenumber.Width = 121;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 2;
			this.column_name.Width = 121;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_cardNO.Caption = "卡号";
			this.column_cardNO.Name = "column_cardNO";
			this.column_cardNO.Visible = true;
			this.column_cardNO.VisibleIndex = 4;
			this.column_cardNO.Width = 125;
			this.dgrd_selectedDoor.Location = new Point(516, 113);
			this.dgrd_selectedDoor.MainView = this.gridSelectedDoor;
			this.dgrd_selectedDoor.Name = "dgrd_selectedDoor";
			this.dgrd_selectedDoor.Size = new Size(429, 199);
			this.dgrd_selectedDoor.TabIndex = 60;
			this.dgrd_selectedDoor.TabStop = false;
			this.dgrd_selectedDoor.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridSelectedDoor
			});
			this.gridSelectedDoor.Appearance.FocusedRow.Options.UseBackColor = true;
			this.gridSelectedDoor.Columns.AddRange(new GridColumn[3]
			{
				this.column_checkdoor,
				this.column_selectedDoor,
				this.column_selectedDevice
			});
			this.gridSelectedDoor.GridControl = this.dgrd_selectedDoor;
			this.gridSelectedDoor.IndicatorWidth = 35;
			this.gridSelectedDoor.Name = "gridSelectedDoor";
			this.gridSelectedDoor.OptionsView.ShowGroupPanel = false;
			this.gridSelectedDoor.CustomDrawColumnHeader += this.gridDoor_CustomDrawColumnHeader;
			this.gridSelectedDoor.CustomDrawCell += this.gridDoor_CustomDrawCell;
			this.gridSelectedDoor.RowStyle += this.gridSelectedDoor_RowStyle;
			this.gridSelectedDoor.Click += this.gridDoor_Click;
			this.gridSelectedDoor.DoubleClick += this.gridSelectedDoor_DoubleClick;
			this.column_checkdoor.Name = "column_checkdoor";
			this.column_checkdoor.Visible = true;
			this.column_checkdoor.VisibleIndex = 0;
			this.column_selectedDoor.Caption = "门名称";
			this.column_selectedDoor.Name = "column_selectedDoor";
			this.column_selectedDoor.Visible = true;
			this.column_selectedDoor.VisibleIndex = 1;
			this.column_selectedDevice.Caption = "所属设备";
			this.column_selectedDevice.Name = "column_selectedDevice";
			this.column_selectedDevice.Visible = true;
			this.column_selectedDevice.VisibleIndex = 2;
			this.dgrd_selectedUser.Cursor = Cursors.Default;
			this.dgrd_selectedUser.Location = new Point(516, 347);
			this.dgrd_selectedUser.MainView = this.gridSelectedUser;
			this.dgrd_selectedUser.Name = "dgrd_selectedUser";
			this.dgrd_selectedUser.Size = new Size(429, 259);
			this.dgrd_selectedUser.TabIndex = 61;
			this.dgrd_selectedUser.TabStop = false;
			this.dgrd_selectedUser.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridSelectedUser
			});
			this.gridSelectedUser.Columns.AddRange(new GridColumn[6]
			{
				this.column_checkSelectUser,
				this.column_selectedDept,
				this.column_selectedPersonnel,
				this.column_selectedName,
				this.column_selectedLastName,
				this.column_selectedCard
			});
			this.gridSelectedUser.GridControl = this.dgrd_selectedUser;
			this.gridSelectedUser.IndicatorWidth = 35;
			this.gridSelectedUser.Name = "gridSelectedUser";
			this.gridSelectedUser.OptionsView.ShowGroupPanel = false;
			this.gridSelectedUser.SortInfo.AddRange(new GridColumnSortInfo[1]
			{
				new GridColumnSortInfo(this.column_selectedPersonnel, ColumnSortOrder.Ascending)
			});
			this.gridSelectedUser.CustomDrawColumnHeader += this.gridDoor_CustomDrawColumnHeader;
			this.gridSelectedUser.CustomDrawCell += this.gridDoor_CustomDrawCell;
			this.gridSelectedUser.Click += this.gridDoor_Click;
			this.gridSelectedUser.DoubleClick += this.gridSelectedUser_DoubleClick;
			this.column_checkSelectUser.Name = "column_checkSelectUser";
			this.column_checkSelectUser.Visible = true;
			this.column_checkSelectUser.VisibleIndex = 0;
			this.column_selectedDept.Caption = "部门名称";
			this.column_selectedDept.Name = "column_selectedDept";
			this.column_selectedDept.Visible = true;
			this.column_selectedDept.VisibleIndex = 5;
			this.column_selectedPersonnel.AppearanceCell.Options.UseTextOptions = true;
			this.column_selectedPersonnel.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			this.column_selectedPersonnel.Caption = "人员编号";
			this.column_selectedPersonnel.Name = "column_selectedPersonnel";
			this.column_selectedPersonnel.Visible = true;
			this.column_selectedPersonnel.VisibleIndex = 1;
			this.column_selectedName.Caption = "姓名";
			this.column_selectedName.Name = "column_selectedName";
			this.column_selectedName.Visible = true;
			this.column_selectedName.VisibleIndex = 2;
			this.column_selectedLastName.Caption = "姓氏";
			this.column_selectedLastName.Name = "column_selectedLastName";
			this.column_selectedLastName.Visible = true;
			this.column_selectedLastName.VisibleIndex = 3;
			this.column_selectedCard.Caption = "卡号";
			this.column_selectedCard.Name = "column_selectedCard";
			this.column_selectedCard.Visible = true;
			this.column_selectedCard.VisibleIndex = 4;
			this.lb_doors.Location = new Point(12, 92);
			this.lb_doors.Name = "lb_doors";
			this.lb_doors.Size = new Size(186, 15);
			this.lb_doors.TabIndex = 66;
			this.lb_doors.Text = "Porta";
			this.lb_selectDoors.Location = new Point(516, 92);
			this.lb_selectDoors.Name = "lb_selectDoors";
			this.lb_selectDoors.Size = new Size(177, 15);
			this.lb_selectDoors.TabIndex = 67;
			this.lb_selectDoors.Text = "Portas selecionadas";
			this.lb_users.Location = new Point(12, 326);
			this.lb_users.Name = "lb_users";
			this.lb_users.Size = new Size(186, 16);
			this.lb_users.TabIndex = 68;
			this.lb_users.Text = "Usuário";
			this.lb_selectUsers.Location = new Point(516, 326);
			this.lb_selectUsers.Name = "lb_selectUsers";
			this.lb_selectUsers.Size = new Size(177, 14);
			this.lb_selectUsers.TabIndex = 69;
			this.lb_selectUsers.Text = "Usuários selecionados";
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(344, 21);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 13);
			this.label2.TabIndex = 70;
			this.label2.Text = "*";
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(878, 21);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 13);
			this.label1.TabIndex = 71;
			this.label1.Text = "*";
			this.re2.AccessibleRole = AccessibleRole.CheckButton;
			this.re2.AllowGrayed = true;
			this.re2.AllowHtmlDraw = DefaultBoolean.True;
			this.re2.AutoHeight = false;
			this.re2.Caption = "Check";
			this.re2.Name = "re2";
			this.gridDoor.Appearance.FocusedRow.Options.UseBackColor = true;
			this.gridDoor.Columns.AddRange(new GridColumn[3]
			{
				this.column_check,
				this.column_door,
				this.column_device
			});
			this.gridDoor.GridControl = this.grid_door;
			this.gridDoor.IndicatorWidth = 35;
			this.gridDoor.Name = "gridDoor";
			this.gridDoor.OptionsSelection.MultiSelect = true;
			this.gridDoor.OptionsView.ShowGroupPanel = false;
			this.gridDoor.CustomDrawColumnHeader += this.gridDoor_CustomDrawColumnHeader;
			this.gridDoor.CustomDrawCell += this.gridDoor_CustomDrawCell;
			this.gridDoor.RowStyle += this.gridDoor_RowStyle;
			this.gridDoor.Click += this.gridDoor_Click;
			this.gridDoor.DoubleClick += this.gridDoor_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_door.Caption = "门名称";
			this.column_door.Name = "column_door";
			this.column_door.Visible = true;
			this.column_door.VisibleIndex = 1;
			this.column_door.Width = 165;
			this.column_device.Caption = "所属设备";
			this.column_device.Name = "column_device";
			this.column_device.Visible = true;
			this.column_device.VisibleIndex = 2;
			this.column_device.Width = 167;
			this.grid_door.Location = new Point(13, 113);
			this.grid_door.MainView = this.gridDoor;
			this.grid_door.Name = "grid_door";
			this.grid_door.RepositoryItems.AddRange(new RepositoryItem[1]
			{
				this.re2
			});
			this.grid_door.Size = new Size(428, 199);
			this.grid_door.TabIndex = 59;
			this.grid_door.TabStop = false;
			this.grid_door.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridDoor
			});
			this.cbo_doorTime.FormattingEnabled = true;
			this.cbo_doorTime.Location = new Point(703, 17);
			this.cbo_doorTime.Name = "cbo_doorTime";
			this.cbo_doorTime.Size = new Size(169, 21);
			this.cbo_doorTime.TabIndex = 1;
			this.cbo_doorTime.SelectedIndexChanged += this.cbo_doorTime_SelectedIndexChanged;
			this.time_load.Tick += this.time_load_Tick;
			this.errorProvider1.ContainerControl = this;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(12, 59);
			this.label3.Name = "label3";
			this.label3.Size = new Size(79, 13);
			this.label3.TabIndex = 72;
			this.label3.Text = "Usar em Visitas";
			this.chkIsVisitLevel.AutoSize = true;
			this.chkIsVisitLevel.Location = new Point(168, 58);
			this.chkIsVisitLevel.Name = "chkIsVisitLevel";
			this.chkIsVisitLevel.Size = new Size(15, 14);
			this.chkIsVisitLevel.TabIndex = 73;
			this.chkIsVisitLevel.UseVisualStyleBackColor = true;
			this.btn_help_info.Image = Resources.help_opera;
			this.btn_help_info.Location = new Point(895, 17);
			this.btn_help_info.Name = "btn_help_info";
			this.btn_help_info.Size = new Size(25, 21);
			this.btn_help_info.TabIndex = 74;
			this.btn_help_info.UseVisualStyleBackColor = true;
			this.btn_help_info.Click += this.btn_help_info_Click;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(957, 672);
			base.Controls.Add(this.btn_help_info);
			base.Controls.Add(this.chkIsVisitLevel);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.cbo_doorTime);
			base.Controls.Add(this.txt_levelName);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.lb_selectUsers);
			base.Controls.Add(this.dgrd_selectedUser);
			base.Controls.Add(this.lb_users);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.lb_selectDoors);
			base.Controls.Add(this.dgrd_selectedDoor);
			base.Controls.Add(this.lb_doors);
			base.Controls.Add(this.grid_door);
			base.Controls.Add(this.btn_exit);
			base.Controls.Add(this.btn_allow);
			base.Controls.Add(this.btn_allDoorLMove);
			base.Controls.Add(this.btn_doorLMove);
			base.Controls.Add(this.btn_doorRMove);
			base.Controls.Add(this.btn_allDoorRMove);
			base.Controls.Add(this.lab_doorTime);
			base.Controls.Add(this.lab_levelName);
			base.Controls.Add(this.btn_allUserLMove);
			base.Controls.Add(this.btn_userLMove);
			base.Controls.Add(this.btn_userRMove);
			base.Controls.Add(this.btn_allUserRMove);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddLevelForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			base.Load += this.AddLevelForm_Load;
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.gridUser).EndInit();
			((ISupportInitialize)this.dgrd_selectedDoor).EndInit();
			((ISupportInitialize)this.gridSelectedDoor).EndInit();
			((ISupportInitialize)this.dgrd_selectedUser).EndInit();
			((ISupportInitialize)this.gridSelectedUser).EndInit();
			((ISupportInitialize)this.re2).EndInit();
			((ISupportInitialize)this.gridDoor).EndInit();
			((ISupportInitialize)this.grid_door).EndInit();
			((ISupportInitialize)this.errorProvider1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
