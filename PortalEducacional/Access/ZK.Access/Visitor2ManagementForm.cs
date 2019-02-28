/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.Access.personnel;
using ZK.Access.Properties;
using ZK.Access.system;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.DBUtility;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Data.Model.StdSDK;
using ZK.MachinesManager;
using ZK.Utils;
using zkonline;

namespace ZK.Access
{
	public class Visitor2ManagementForm : Office2007Form
	{
		private bool PhotoChanged;

		private bool _releaseCardNumber = false;

		private bool readStart = true;

		public int Add_Event_Userid = 0;

		private string cardNumber = string.Empty;

		private string IDNumber = string.Empty;

		private string oldEmpLevelIDs = string.Empty;

		private string newEmpLevelIDs = string.Empty;

		private List<Machines> levelMachines = null;

		private List<Machines> oldLevelMachines = null;

		private UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);

		private UserInfo oldUser = null;

		private bool m_visitorActive = false;

		private int previousUserWithCard = 0;

		private int m_id = 0;

		private int m_parentID = 0;

		private string fileName = "";

		private DataTable m_dataTable = new DataTable();

		private DataTable m_browDataTable = new DataTable();

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private Dictionary<string, Dictionary<string, string>> m_type = null;

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		public Dictionary<string, Dictionary<string, string>> m_Permissions = null;

		private DataTable dtGender = new DataTable();

		private Dictionary<int, string> mlist = new Dictionary<int, string>();

		private Dictionary<int, int> itemlist = new Dictionary<int, int>();

		private Dictionary<int, string> levelList = new Dictionary<int, string>();

		private Dictionary<int, List<string>> tempList = new Dictionary<int, List<string>>();

		private Dictionary<int, List<byte[]>> templates = new Dictionary<int, List<byte[]>>();

		private string m_oldfingerSign = "0000000000";

		private bool isClickTemplateBtn = false;

		private IDeviceMonitor DevMonitor;

		private List<AccDoor> m_selectedDoor = null;

		private bool isDouble = false;

		private string text;

		private int key_len = 0;

		private FPRegister register;

		private DeviceServerBll devServer = null;

		private Dictionary<int, Template> dicFingerId_FpTemplate;

		private Dictionary<int, Template> dicFingerId_FpTemplate_Old;

		private string PreIDCardNo;

		private BackgroundWorker bwReader;

		private IDReader idReader;

		private bool showValidationMessage = true;

		private IContainer components = null;

		private DevComponents.DotNetBar.TabControl tabControl1;

		private TabControlPanel tabControlPanel1;

		private TabItem tabItem_basic;

		private TabControlPanel tabControlPanel2;

		private TabItem tabItem_detail;

		private PictureBox pic_photo;

		private TextBox txt_cardNO;

		private TextBox txt_name;

		private TextBox txt_personnel;

		private Label lab_gender;

		private Label lab_name;

		private Label lab_cardNO;

		private Label lab_personnel;

		private TextBox txt_origin;

		private TextBox txt_jobTitle;

		private TextBox txt_ethnic;

		private TextBox txt_city;

		private TextBox txt_nationality;

		private TextBox txt_homeAddress;

		private TextBox txt_workAddress;

		private TextBox txt_IDnumber;

		private TextBox txt_homeTelephone;

		private TextBox txt_officeTelephone;

		private TextBox txt_politicalStatus;

		private TextBox txt_education;

		private Label lab_homeAddress;

		private Label lab_origin;

		private Label lab_jobTitle;

		private Label lab_ethnic;

		private Label lab_city;

		private Label lab_nationality;

		private Label lab_workAddress;

		private Label lab_IDnumber;

		private Label lab_homeTelephone;

		private Label lab_officeTelephone;

		private Label lab_politicalStatus;

		private Label lab_Education;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		private OpenFileDialog openFileDialog1;

		private System.Windows.Forms.ComboBox cbo_gender;

		private ButtonX btn_browse;

		public ButtonX btn_saveContinue;

		private LinkLabel linklbl_FPRegister;

		private Label lab_FPRegister;

		private Label label3;

		private TextBox txt_lastName;

		private Label lab_lastName;

		private TabControlPanel tabControlPanel3;

		private Label lab_levels;

		private TabItem tabItem3;

		private TextBox txt_password;

		private Label lab_password;

		private ButtonX btn_allLMove;

		private ButtonX btn_LMove;

		private ButtonX btn_RMove;

		private ButtonX btn_allRMove;

		private Label label4;

		private Label lab_birthday;

		private TextBox txt_mobilPhone;

		private Label lab_mobilePhone;

		private TextBox txt_email;

		private Label lab_email;

		private TextBox txt_postalCode;

		private Label lab_postalCode;

		private GridControl dgrd_optionalLevel;

		private GridView gridView1;

		private GridColumn column_check;

		private GridColumn column_level;

		private GridColumn column_time;

		private GridControl dgrd_selectedLevel;

		private GridView gridView2;

		private GridColumn column_check2;

		private GridColumn column_selectedLevel;

		private GridColumn column_selectedTime;

		private PictureBox pic_Readding;

		private Label lbl_picture;

		private Label lab_multiCard;

		private System.Windows.Forms.ComboBox cbo_MultiCard;

		private Label lab_End;

		private Label lab_start;

		private CheckBox chk_setValidTime;

		private Label lab_setValidTime;

		private PictureBox link_read;

		private System.Windows.Forms.ToolTip toolTip1;

		private BackgroundWorker backgroundWorker1;

		private FlowLayoutPanel flowLayoutPanel1;

		private LinkLabel lnk_ClearAntyBack;

		private LinkLabel lnkRegFpViaDev;

		private FlowLayoutPanel flowLayoutPanel2;

		private DateEdit DTPick_birthday;

		private DateEdit cbo_datEnd;

		private DateEdit cbo_datStart;

		private LinkLabel lnkIDReader;

		private Label lblIDReaderFailed;

		public ButtonX btnFPCollector;

		public ButtonX btnMachine;

		public ButtonX btnIDReader;

		private Label lblActiveVisit;

		private TextBox txtDefaultFloor;

		private Label lblElevatorFloor;

		private Panel panel1;

		private Label lblElevatorSpecialNeeds;

		private CheckBox chkElevatorSpecialNeeds;

		private ButtonX buttonX1;

		private ButtonX buttonX2;

		private PictureBox pictureBox1;

		public event EventHandler refreshDataEvent = null;

		public Visitor2ManagementForm()
		{
			this.InitializeComponent();
			this.panel1.Visible = MainForm._elevatorEnabled;
			try
			{
				this.GenderType();
				DevExpressHelper.InitImageList(this.gridView1, "column_check");
				DevExpressHelper.InitImageList(this.gridView2, "column_check2");
				if (initLang.Lang != "chs")
				{
					this.lab_origin.Visible = false;
					this.txt_origin.Visible = false;
					this.lnkIDReader.Visible = false;
				}
				else
				{
					this.txt_lastName.ReadOnly = true;
				}
				this.EmploymentType();
				this.Type();
				this.InitDataTableSet();
				this.InitBrowDataTableSet();
				this.LevelDataBind();
				this.MultiCard();
				this.DataStaff();
				initLang.LocaleForm(this, base.Name);
				this.DTPick_birthday.EditValue = DateTime.Now.AddDays(-7.0);
				this.txt_email.LostFocus += this.txt_email_LostFocus;
				this.text = this.txt_name.Text;
				if (AccCommon.CodeVersion != CodeVersionType.ZKNN_20130412_01)
				{
					goto IL_0277;
				}
				goto IL_0277;
				IL_0277:
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					this.txt_cardNO.CharacterCasing = CharacterCasing.Upper;
					this.lab_mobilePhone.Visible = false;
					this.txt_mobilPhone.Visible = false;
					this.lnk_ClearAntyBack.Visible = true;
					this.lnk_ClearAntyBack.Enabled = false;
					this.txt_personnel.Text = this.GetMaxPin();
				}
				this.CompactPhotoMsgPosition();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.btnMachine.Visible = true;
		}

		private void _checkVisitorInVisit()
		{
			IVisitInfo dALObject = VisitInfoBll.getDALObject();
			this.m_visitorActive = dALObject.VisitorActive(this.m_id);
		}

		private void CompactPhotoMsgPosition()
		{
			Size size = this.pic_photo.Size;
			int width = size.Width;
			size = this.lbl_picture.Size;
			int num = width - size.Width;
			int num2 = num / 2;
			Label label = this.lbl_picture;
			Point location = this.pic_photo.Location;
			int x = location.X + num2;
			location = this.lbl_picture.Location;
			label.Location = new Point(x, location.Y);
		}

		public Visitor2ManagementForm(string cardNo)
			: this()
		{
			this.txt_cardNO.Text = cardNo;
		}

		public Visitor2ManagementForm(int id, int parentID, string deptName)
			: this()
		{
			this.m_id = id;
			this.m_parentID = parentID;
			this._checkVisitorInVisit();
			this.BindData();
		}

		private void InitDataTableSet()
		{
			this.m_dataTable.Columns.Add("id");
			this.m_dataTable.Columns.Add("level_name");
			this.m_dataTable.Columns.Add("timeseg_name");
			this.m_dataTable.Columns.Add("check");
			this.column_level.FieldName = "level_name";
			this.column_time.FieldName = "timeseg_name";
			this.column_check.FieldName = "check";
			this.dgrd_optionalLevel.DataSource = this.m_dataTable;
		}

		private void InitBrowDataTableSet()
		{
			this.m_browDataTable.Columns.Add("id");
			this.m_browDataTable.Columns.Add("level_name");
			this.m_browDataTable.Columns.Add("timeseg_name");
			this.m_browDataTable.Columns.Add("check");
			this.column_selectedLevel.FieldName = "level_name";
			this.column_selectedTime.FieldName = "timeseg_name";
			this.column_check2.FieldName = "check";
			this.dgrd_selectedLevel.DataSource = this.m_browDataTable;
		}

		private void GenderType()
		{
			try
			{
				this.dtGender.Columns.Add("Value");
				this.dtGender.Columns.Add("Text");
				this.cbo_gender.DataSource = this.dtGender;
				this.cbo_gender.ValueMember = "Value";
				this.cbo_gender.DisplayMember = "Text";
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

		private void EmploymentType()
		{
			try
			{
				this.m_typeDic = initLang.GetComboxInfo("employmentType");
				if (this.m_typeDic == null || this.m_typeDic.Count == 0)
				{
					this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("1", "合同内");
					dictionary.Add("2", "合同外");
					this.m_typeDic.Add("0", dictionary);
					initLang.SetComboxInfo("employmentType", this.m_typeDic);
					initLang.Save();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void Type()
		{
			try
			{
				this.m_type = initLang.GetComboxInfo("type");
				if (this.m_type == null || this.m_type.Count == 0)
				{
					this.m_type = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("1", "正式员工");
					dictionary.Add("2", "试用员工");
					this.m_type.Add("0", dictionary);
					initLang.SetComboxInfo("type", this.m_type);
					initLang.Save();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataStaff()
		{
			try
			{
				this.dtGender.Rows.Clear();
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						DataRow dataRow = this.dtGender.NewRow();
						dataRow["Value"] = item.Key.ToUpper().Trim();
						dataRow["Text"] = item.Value;
						this.dtGender.Rows.Add(dataRow);
					}
				}
				this.cbo_gender.SelectedValue = "M";
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void txt_email_LostFocus(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_email.Text) && !CheckInfo.IsEmail(this.txt_email.Text))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("emailError", "邮箱格式不正确!"));
			}
		}

		private void CheckBtn()
		{
			if (this.m_dataTable.Rows.Count > 0)
			{
				this.btn_allRMove.Enabled = true;
				this.btn_RMove.Enabled = true;
			}
			else
			{
				this.btn_allRMove.Enabled = false;
				this.btn_RMove.Enabled = false;
			}
			if (this.m_browDataTable.Rows.Count > 0)
			{
				this.btn_LMove.Enabled = true;
				this.btn_allLMove.Enabled = true;
			}
			else
			{
				this.btn_LMove.Enabled = false;
				this.btn_allLMove.Enabled = false;
			}
		}

		private void BindData()
		{
			try
			{
				this.lnkRegFpViaDev.Enabled = (this.m_id > 0);
				this.btnMachine.Enabled = this.lnkRegFpViaDev.Enabled;
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					this.cardNumber = string.Empty;
					this.btn_saveContinue.Visible = false;
					UserInfo userInfo = null;
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					userInfo = userInfoBll.GetModel(this.m_id);
					this.oldUser = userInfo.Copy();
					this.oldUser.Name = (userInfo.Name ?? "");
					this.oldUser.UserId = this.m_id;
					this.oldUser.BadgeNumber = userInfo.BadgeNumber;
					this.oldUser.MorecardGroupId = userInfo.MorecardGroupId;
					this.oldUser.AccStartDate = userInfo.AccStartDate;
					this.oldUser.AccEndDate = userInfo.AccEndDate;
					this.oldUser.Privilege = userInfo.Privilege;
					if (string.IsNullOrEmpty(userInfo.PassWord))
					{
						this.oldUser.PassWord = "";
					}
					else
					{
						this.oldUser.PassWord = userInfo.PassWord;
					}
					if (string.IsNullOrEmpty(userInfo.CardNo))
					{
						this.oldUser.CardNo = "";
					}
					else
					{
						this.oldUser.CardNo = userInfo.CardNo;
					}
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					this.oldLevelMachines = machinesBll.GetModelList("id in(select  device_id from acc_door where id in(select  accdoor_id from acc_levelset_door_group where acclevelset_id in(select acclevelset_id from acc_levelset_emp where employee_id=" + this.oldUser.UserId + ")))");
					if (userInfo != null)
					{
						this.txt_personnel.Text = userInfo.BadgeNumber.ToString();
						this.txt_personnel.Enabled = false;
						this.txt_cardNO.Focus();
						this.txt_name.Text = userInfo.Name;
						this.txtDefaultFloor.Text = userInfo.elevatorDefaultFloor;
						this.txt_cardNO.Text = (this.m_visitorActive ? userInfo.CardNo : "");
						this.txtDefaultFloor.Text = (this.m_visitorActive ? userInfo.elevatorDefaultFloor : "");
						this.chkElevatorSpecialNeeds.Checked = userInfo.elevatorSpecialNeeds;
						this.cardNumber = userInfo.CardNo;
						this.cbo_gender.SelectedValue = (userInfo.Gender ?? "M").ToString();
						if (userInfo.Photo != null)
						{
							try
							{
								MemoryStream stream = new MemoryStream(userInfo.Photo);
								this.pic_photo.Image = Image.FromStream(stream);
								this.PhotoChanged = false;
								Size size = this.pic_photo.Image.Size;
								int width = size.Width;
								size = this.pic_photo.Size;
								int num;
								if (width <= size.Width)
								{
									size = this.pic_photo.Image.Size;
									int height = size.Height;
									size = this.pic_photo.Size;
									num = ((height > size.Height) ? 1 : 0);
								}
								else
								{
									num = 1;
								}
								if (num != 0)
								{
									this.pic_photo.SizeMode = PictureBoxSizeMode.Zoom;
								}
								else
								{
									this.pic_photo.SizeMode = PictureBoxSizeMode.CenterImage;
								}
							}
							catch
							{
							}
						}
						DateTime? nullable = userInfo.AccStartDate;
						if (!string.IsNullOrEmpty(nullable.ToString()) && this.m_visitorActive)
						{
							this.chk_setValidTime.Checked = true;
							this.cbo_datStart.EditValue = userInfo.AccStartDate;
							this.cbo_datEnd.EditValue = userInfo.AccEndDate;
						}
						else
						{
							this.chk_setValidTime.Checked = true;
							this.cbo_datStart.EditValue = DateTime.Now;
							this.cbo_datEnd.EditValue = DateTime.Now;
						}
						this.txt_password.Text = userInfo.PassWord;
						this.txt_jobTitle.Text = userInfo.Title;
						this.txt_mobilPhone.Text = userInfo.Pager;
						DateEdit dTPick_birthday = this.DTPick_birthday;
						nullable = userInfo.Birthday;
						DateTime dateTime;
						if (nullable.HasValue)
						{
							nullable = userInfo.Birthday;
							dateTime = nullable.Value;
						}
						else
						{
							dateTime = DateTime.Now;
						}
						dTPick_birthday.EditValue = dateTime;
						this.txt_workAddress.Text = userInfo.Street;
						this.txt_city.Text = userInfo.City;
						this.txt_nationality.Text = userInfo.State;
						this.txt_postalCode.Text = userInfo.Zip;
						this.txt_officeTelephone.Text = userInfo.OPhone;
						this.txt_homeTelephone.Text = userInfo.FPhone;
						this.txt_lastName.Text = userInfo.LastName;
						this.txt_IDnumber.Text = userInfo.IdentityCard;
						this.IDNumber = userInfo.IdentityCard;
						this.txt_education.Text = userInfo.Education;
						this.txt_origin.Text = userInfo.BirthPlace;
						this.txt_politicalStatus.Text = userInfo.Political;
						this.txt_nationality.Text = userInfo.Contry;
						this.txt_email.Text = userInfo.Email;
						this.txt_homeAddress.Text = userInfo.HomeAddress;
						this.txt_ethnic.Text = userInfo.MinZu;
						if (this.mlist.ContainsKey(userInfo.MorecardGroupId))
						{
							this.cbo_MultiCard.Text = this.mlist[userInfo.MorecardGroupId];
						}
						this.BindLevelset();
					}
				}
				else
				{
					this.DTPick_birthday.EditValue = DateTime.Now;
					this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
					this.txt_personnel.Text = (this.GetMaxPin() ?? "");
				}
				this.CheckBtn();
				this.txt_cardNO.Enabled = !this.m_visitorActive;
				this.btn_allRMove.Enabled = !this.m_visitorActive;
				this.btn_allLMove.Enabled = !this.m_visitorActive;
				this.btn_RMove.Enabled = !this.m_visitorActive;
				this.btn_LMove.Enabled = !this.m_visitorActive;
				this.chk_setValidTime.Enabled = !this.m_visitorActive;
				this.cbo_datStart.Enabled = !this.m_visitorActive;
				this.cbo_datEnd.Enabled = !this.m_visitorActive;
				this.cbo_MultiCard.Enabled = !this.m_visitorActive;
				this.lblActiveVisit.Visible = this.m_visitorActive;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				this.lnk_ClearAntyBack.Enabled = (this.oldLevelMachines != null && this.oldLevelMachines.Count > 0);
			}
		}

		private void BindLevelset()
		{
			try
			{
				if (this.m_id > 0 && this.m_visitorActive)
				{
					AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
					List<AccLevelsetEmp> modelList = accLevelsetEmpBll.GetModelList("employee_id=" + this.m_id);
					if (modelList != null)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							this.oldEmpLevelIDs = this.oldEmpLevelIDs + modelList[i].acclevelset_id + ",";
							int num = 0;
							while (num < this.m_dataTable.Rows.Count)
							{
								if (this.m_dataTable.Rows[num][0].ToString() == null || !(this.m_dataTable.Rows[num][0].ToString() == modelList[i].acclevelset_id.ToString()))
								{
									num++;
									continue;
								}
								DataRow dataRow = this.m_browDataTable.NewRow();
								dataRow[0] = this.m_dataTable.Rows[num][0].ToString();
								dataRow[1] = this.m_dataTable.Rows[num][1].ToString();
								dataRow[2] = this.m_dataTable.Rows[num][2].ToString();
								dataRow[3] = false;
								this.m_browDataTable.Rows.Add(dataRow);
								this.m_dataTable.Rows.RemoveAt(num);
								break;
							}
						}
					}
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void MultiCard()
		{
			try
			{
				this.mlist.Clear();
				this.cbo_MultiCard.Items.Clear();
				this.cbo_MultiCard.Items.Add("-----");
				this.cbo_MultiCard.SelectedIndex = 0;
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				List<AccMorecardempGroup> modelList = accMorecardempGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.cbo_MultiCard.Items.Add(modelList[i].group_name.ToString());
						this.mlist.Add(modelList[i].id, modelList[i].group_name.ToString());
						this.itemlist.Add(i + 1, modelList[i].id);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LevelDataBind()
		{
			try
			{
				this.m_dataTable.Clear();
				AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccLevelset> modelList = accLevelsetBll.GetModelList("", true);
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						DataRow dataRow = this.m_dataTable.NewRow();
						dataRow[0] = modelList[i].id;
						dataRow[1] = modelList[i].level_name;
						AccTimeseg model = accTimesegBll.GetModel(modelList[i].level_timeseg_id);
						if (model != null)
						{
							dataRow[2] = model.timeseg_name;
						}
						dataRow[3] = false;
						this.m_dataTable.Rows.Add(dataRow);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void time_checkBox_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked)
			{
				this.lab_End.Enabled = true;
				this.lab_start.Enabled = true;
				this.cbo_datEnd.Enabled = true;
				this.cbo_datStart.Enabled = true;
			}
			else
			{
				this.lab_End.Enabled = false;
				this.lab_start.Enabled = false;
				this.cbo_datEnd.Enabled = false;
				this.cbo_datStart.Enabled = false;
			}
		}

		private bool CheckData()
		{
			this._releaseCardNumber = false;
			try
			{
				UserInfo userInfo = null;
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				if (this.m_id > 0)
				{
					userInfo = userInfoBll.GetModel(this.m_id);
				}
				if (this.m_browDataTable.Rows.Count <= 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("visitorLevelSetRequired", "Please, select one access level"));
					return false;
				}
				if (userInfo == null)
				{
					userInfo = new UserInfo();
					if (string.IsNullOrEmpty(this.txt_personnel.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personnelNumberNotNull", "人员编号不能为空"));
						this.txt_personnel.Focus();
						return false;
					}
					if (!int.TryParse(this.txt_personnel.Text, out int _))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PinMustBeNumber", "人员编号必须为数字"));
						this.txt_personnel.Focus();
						return false;
					}
					if (userInfoBll.ExistsBadgenumber(this.txt_personnel.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personnelNumberRepeat", "人员编号已存在"));
						this.txt_personnel.Focus();
						return false;
					}
					if (this.chk_setValidTime.Checked && this.cbo_datStart.EditValue != null && !string.IsNullOrEmpty(this.cbo_datStart.EditValue.ToString()))
					{
						string value = this.cbo_datStart.EditValue.ToString();
						string value2 = this.cbo_datEnd.EditValue.ToString();
						DateTime t = Convert.ToDateTime(value);
						DateTime t2 = Convert.ToDateTime(value2);
						if (t < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SStartDateWrong", "门禁有效时间不能小于当前日期"));
							this.cbo_datStart.Focus();
							return false;
						}
						if (t > t2)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("StartDateGreaterThanEndDate", "门禁有效时间的开始日期不能大于结束日期"));
							this.cbo_datStart.Focus();
							return false;
						}
					}
					if (!string.IsNullOrEmpty(this.txt_cardNO.Text) && userInfoBll.ExistsCardNo(this.txt_cardNO.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("cardNumberRepeat", "卡号已存在!"));
						this.txt_cardNO.Focus();
						return false;
					}
					if (!string.IsNullOrEmpty(this.txt_IDnumber.Text))
					{
						if (userInfoBll.ExistsIdentitycard(this.txt_IDnumber.Text))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIdentitycardRepeated", "身份证号已存在!"));
							this.txt_IDnumber.Focus();
							return false;
						}
						if (this.txt_IDnumber.Text.Length < 9 || this.txt_IDnumber.Text.Length > 14)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIdentitycard", "身份证号码不正确!"));
							this.txt_IDnumber.Focus();
							return false;
						}
					}
					if (!string.IsNullOrEmpty(this.txt_password.Text))
					{
						DataSet list = userInfoBll.GetList($"BadgeNumber<>'{this.txt_personnel.Text}' and PASSWORD='{this.txt_password.Text}'");
						if (list != null && list.Tables.Count > 0 && list.Tables[0].Rows.Count > 0)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SamePasswordExists", "密码已存在"));
							this.txt_password.Focus();
							return false;
						}
					}
					if (!string.IsNullOrEmpty(this.txt_email.Text) && !CheckInfo.IsEmail(this.txt_email.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("emailError", "邮箱格式不正确!"));
						this.txt_email.Focus();
						return false;
					}
					if (!string.IsNullOrEmpty(this.txt_password.Text))
					{
						AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
						if (accDoorBll.GetModelList("force_pwd='" + Rijndael.Instatnce.Encrypt(this.txt_password.Text) + "'") != null && accDoorBll.GetModelList("force_pwd='" + Rijndael.Instatnce.Encrypt(this.txt_password.Text) + "'").Count > 0)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PersonnelPwdError", "人员密码不能与任意门禁胁迫密码相同"));
							this.txt_password.Focus();
							return false;
						}
					}
					return true;
				}
				if (string.IsNullOrEmpty(this.txt_personnel.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personnelNumberNotNull", "人员编号不能为空"));
					this.txt_personnel.Focus();
					return false;
				}
				if (this.chk_setValidTime.Checked && this.cbo_datStart.EditValue != null && !string.IsNullOrEmpty(this.cbo_datStart.EditValue.ToString()))
				{
					string value3 = this.cbo_datStart.EditValue.ToString();
					string value4 = this.cbo_datEnd.EditValue.ToString();
					DateTime t3 = Convert.ToDateTime(value3);
					DateTime t4 = Convert.ToDateTime(value4);
					if (t3 > t4)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("StartDateGreaterThanEndDate", "门禁有效时间的开始日期不能大于结束日期"));
						this.cbo_datStart.Focus();
						return false;
					}
				}
				if (userInfo.BadgeNumber != this.txt_personnel.Text && !string.IsNullOrEmpty(this.txt_personnel.Text) && userInfoBll.ExistsBadgenumber(this.txt_personnel.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personnelNumberRepeat", "人员编号已存在"));
					this.txt_personnel.Focus();
					return false;
				}
				if (!string.IsNullOrEmpty(this.txt_cardNO.Text) && userInfo.CardNo != this.txt_cardNO.Text && userInfoBll.ExistsCardNo(this.txt_cardNO.Text))
				{
					List<UserInfo> modelList = this.userInfoBll.GetModelList("CardNo='" + DBHelper.NoSqlInjection(this.txt_cardNO.Text) + "'");
					if (modelList.Count > 0)
					{
						if (modelList[0].isVisitor)
						{
							DialogResult dialogResult = SysDialogs.ShowQueseMessage("Este cartão está em uso por: \n" + modelList[0].Name + " " + modelList[0].LastName + "\n\nGostaria de forçar a liberação do cartão e usá-lo nesta visita?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
							if (dialogResult != DialogResult.Yes)
							{
								return false;
							}
							this._releaseCardNumber = true;
							goto IL_0858;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("cardNumberRepeat", "卡号已存在!") + "\n" + modelList[0].Name + " " + modelList[0].LastName);
						this.txt_cardNO.Focus();
						return false;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("cardNumberRepeat", "卡号已存在!"));
					this.txt_cardNO.Focus();
					return false;
				}
				goto IL_0858;
				IL_0858:
				if (!string.IsNullOrEmpty(this.txt_IDnumber.Text) && userInfo.IdentityCard != this.txt_IDnumber.Text)
				{
					if (userInfoBll.ExistsIdentitycard(this.txt_IDnumber.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIdentitycardRepeated", "身份证号已存在!"));
						this.txt_IDnumber.Focus();
						return false;
					}
					if (this.txt_IDnumber.Text.Length < 9 || this.txt_IDnumber.Text.Length > 14)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIdentitycard", "身份证号码不正确!"));
						this.txt_IDnumber.Focus();
						return false;
					}
				}
				if (!string.IsNullOrEmpty(this.txt_password.Text))
				{
					DataSet list2 = userInfoBll.GetList($"BadgeNumber<>'{this.txt_personnel.Text}' and PASSWORD='{this.txt_password.Text}'");
					if (list2 != null && list2.Tables.Count > 0 && list2.Tables[0].Rows.Count > 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SamePasswordExists", "密码已存在"));
						this.txt_password.Focus();
						return false;
					}
				}
				if (!string.IsNullOrEmpty(this.txt_email.Text) && !CheckInfo.IsEmail(this.txt_email.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("emailError", "邮箱格式不正确!"));
					this.txt_email.Focus();
					return false;
				}
				if (!string.IsNullOrEmpty(this.txt_password.Text))
				{
					AccDoorBll accDoorBll2 = new AccDoorBll(MainForm._ia);
					if (accDoorBll2.GetModelList("force_pwd='" + Rijndael.Instatnce.Encrypt(this.txt_password.Text) + "'") != null && accDoorBll2.GetModelList("force_pwd='" + Rijndael.Instatnce.Encrypt(this.txt_password.Text) + "'").Count > 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PersonnelPwdError", "人员密码不能与任意门禁胁迫密码相同"));
						this.txt_password.Focus();
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void SetModel(UserInfo model)
		{
			try
			{
				if (this.cbo_datStart.Text != "")
				{
					model.AccStartDate = DateTime.Parse(this.cbo_datStart.Text);
				}
				if (this.cbo_datEnd.Text != "")
				{
					model.AccEndDate = DateTime.Parse(this.cbo_datEnd.Text);
				}
				model.DefaultDeptId = 0;
				model.elevatorProfile = "VISITOR";
				if (!string.IsNullOrEmpty(this.txtDefaultFloor.Text.Trim()))
				{
					model.elevatorDefaultFloor = this.txtDefaultFloor.Text;
				}
				model.elevatorSpecialNeeds = this.chkElevatorSpecialNeeds.Checked;
				if (!string.IsNullOrEmpty(this.txt_cardNO.Text))
				{
					model.CardNo = this.txt_cardNO.Text.Trim();
				}
				else
				{
					model.CardNo = string.Empty;
				}
				model.Name = this.txt_name.Text;
				model.Gender = (this.cbo_gender.SelectedValue ?? "M").ToString();
				model.PassWord = this.txt_password.Text;
				model.HiredDay = DateTime.Now;
				model.Birthday = DateTime.Parse(this.DTPick_birthday.Text);
				model.Title = this.txt_jobTitle.Text;
				model.Pager = this.txt_mobilPhone.Text;
				model.Street = this.txt_workAddress.Text;
				model.City = this.txt_city.Text;
				model.State = this.txt_nationality.Text;
				model.Zip = this.txt_postalCode.Text;
				model.OPhone = this.txt_officeTelephone.Text;
				model.FPhone = this.txt_homeTelephone.Text;
				model.LastName = this.txt_lastName.Text;
				model.IdentityCard = this.txt_IDnumber.Text;
				model.Education = this.txt_education.Text;
				model.BirthPlace = this.txt_origin.Text;
				model.Political = this.txt_politicalStatus.Text;
				model.Contry = this.txt_nationality.Text;
				model.Email = this.txt_email.Text;
				model.HomeAddress = this.txt_homeAddress.Text;
				model.MinZu = this.txt_ethnic.Text;
				model.HireType = 0;
				model.EmpType = 0;
				model.Privilege = 0;
				if (this.itemlist.ContainsKey(this.cbo_MultiCard.SelectedIndex))
				{
					model.MorecardGroupId = this.itemlist[this.cbo_MultiCard.SelectedIndex];
				}
				else
				{
					model.MorecardGroupId = 0;
				}
				try
				{
					model.BadgeNumber = int.Parse(this.txt_personnel.Text).ToString();
				}
				catch
				{
				}
				this.SetPhoto(model);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetPhoto(UserInfo model)
		{
			if (this.pic_photo.Image != null && this.PhotoChanged)
			{
				try
				{
					Bitmap bitmap = new Bitmap(this.pic_photo.Image.Width, this.pic_photo.Image.Height);
					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(this.pic_photo.Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					MemoryStream memoryStream = new MemoryStream();
					bitmap.Save(memoryStream, ImageFormat.Bmp);
					graphics.Dispose();
					model.Photo = memoryStream.GetBuffer();
					memoryStream.Close();
				}
				catch
				{
				}
			}
		}

		private void SaveCard(UserInfo model)
		{
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				if (this.m_id > 0)
				{
					personnelIssuecardBll.DeleteByUserId(model.UserId);
				}
				if (!string.IsNullOrEmpty(model.CardNo))
				{
					PersonnelIssuecard personnelIssuecard = new PersonnelIssuecard();
					personnelIssuecard.create_time = DateTime.Now;
					personnelIssuecard.UserID_id = model.UserId;
					personnelIssuecard.cardno = model.CardNo;
					personnelIssuecardBll.Add(personnelIssuecard);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void CheckLevelChange(UserInfo newUser, UserInfo oldUser, int TemplateCount)
		{
			AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
			Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!dictionary.ContainsKey(modelList[i].id))
					{
						dictionary.Add(modelList[i].id, modelList[i]);
					}
				}
			}
			Dictionary<int, AccTimeseg> dictionary2 = new Dictionary<int, AccTimeseg>();
			AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
			List<AccTimeseg> modelList2 = accTimesegBll.GetModelList("");
			if (accTimesegBll != null)
			{
				for (int j = 0; j < modelList2.Count; j++)
				{
					if (!dictionary2.ContainsKey(modelList2[j].id))
					{
						dictionary2.Add(modelList2[j].id, modelList2[j]);
					}
				}
			}
			AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
			List<AccMorecardempGroup> modelList3 = accMorecardempGroupBll.GetModelList("");
			Dictionary<int, AccMorecardempGroup> dictionary3 = new Dictionary<int, AccMorecardempGroup>();
			if (modelList3 != null)
			{
				for (int k = 0; k < modelList3.Count; k++)
				{
					if (!dictionary3.ContainsKey(modelList3[k].id))
					{
						dictionary3.Add(modelList3[k].id, modelList3[k]);
					}
				}
			}
			Dictionary<int, AccTimeseg> dictionary4 = new Dictionary<int, AccTimeseg>();
			List<AccLevelset> modelList4 = accLevelsetBll.GetModelList("", true);
			if (modelList4 != null)
			{
				for (int l = 0; l < modelList4.Count; l++)
				{
					if (!dictionary4.ContainsKey(modelList4[l].id) && dictionary2.ContainsKey(modelList4[l].level_timeseg_id))
					{
						dictionary4.Add(modelList4[l].id, dictionary2[modelList4[l].level_timeseg_id]);
					}
				}
			}
			List<UserInfo> list = new List<UserInfo>();
			list.Add(newUser);
			List<UserVerifyType> list2 = new List<UserVerifyType>();
			UserVerifyType userVerifyType = new UserVerifyType();
			if (dictionary3.ContainsKey(newUser.MorecardGroupId))
			{
				userVerifyType.GroupNo = dictionary3[newUser.MorecardGroupId].StdGroupNo;
			}
			userVerifyType.GroupNo = ((userVerifyType.GroupNo <= 0) ? 1 : userVerifyType.GroupNo);
			userVerifyType.Pin = int.Parse(newUser.BadgeNumber);
			userVerifyType.UseGroupVT = true;
			userVerifyType.VerifyType = 0;
			list2.Add(userVerifyType);
			if (!string.IsNullOrEmpty(this.newEmpLevelIDs) || !string.IsNullOrEmpty(this.oldEmpLevelIDs))
			{
				int userId;
				string text2;
				if (!string.IsNullOrEmpty(this.newEmpLevelIDs) && !string.IsNullOrEmpty(this.oldEmpLevelIDs))
				{
					if (this.newEmpLevelIDs == this.oldEmpLevelIDs)
					{
						if (newUser.CardNo != oldUser.CardNo || newUser.AccStartDate != oldUser.AccStartDate || newUser.AccEndDate != oldUser.AccEndDate || newUser.MorecardGroupId != oldUser.MorecardGroupId || newUser.Privilege != oldUser.Privilege || newUser.PassWord != oldUser.PassWord || newUser.Name != oldUser.Name || this.isClickTemplateBtn)
						{
							for (int m = 0; m < this.levelMachines.Count; m++)
							{
								try
								{
									DevCmds.RunImmediatelyEnabled = true;
									if (this.previousUserWithCard > 0)
									{
										List<UserInfo> modelList5 = this.userInfoBll.GetModelList("USERID = " + this.previousUserWithCard);
										if (modelList5.Count > 0)
										{
											userId = modelList5[0].UserId;
											string text = userId.ToString();
											if (this.levelMachines[m].DevSDKType != SDKType.StandaloneSDK)
											{
												CommandServer.DeleteFace(this.levelMachines[m], modelList5, true, out text2);
												CommandServer.DeleteFingerVein(this.levelMachines[m], modelList5, true, out text2);
												CommandServer.DeleteFingerPrint(this.levelMachines[m], modelList5, true, out text2);
												CommandServer.DeleteUserAuthorize(this.levelMachines[m], modelList5, true, out text2);
											}
											CommandServer.DeleteUser(this.levelMachines[m], modelList5, true, out text2);
										}
									}
									if (this.levelMachines[m].DevSDKType != SDKType.StandaloneSDK)
									{
										CommandServer.DeleteFace(this.levelMachines[m], list, true, out text2);
										CommandServer.DeleteFingerVein(this.levelMachines[m], list, true, out text2);
										CommandServer.DeleteFingerPrint(this.levelMachines[m], list, true, out text2);
										CommandServer.DeleteUserAuthorize(this.levelMachines[m], list, true, out text2);
									}
									CommandServer.DeleteUser(this.levelMachines[m], list, true, out text2);
									CommandServer.AddUser(this.levelMachines[m], list, dictionary3);
									CommandServer.AddUserPhoto(this.levelMachines[m], list);
									CommandServer.AddUserVerifyType(this.levelMachines[m], list2);
									CommandServer.AddFace(this.levelMachines[m], list);
									CommandServer.AddFingerPrint(this.levelMachines[m], list);
									CommandServer.AddFingerVein(this.levelMachines[m], list);
									CommandServer.AddElevator(this.levelMachines[m], list, dictionary3);
									modelList4 = accLevelsetBll.GetModelList("id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + this.levelMachines[m].ID + ") and id in(select acclevelset_id from acc_levelset_emp where employee_id=" + newUser.UserId + ")", true);
									if (modelList4 != null)
									{
										for (int n = 0; n < modelList4.Count; n++)
										{
											if (dictionary4.ContainsKey(modelList4[n].id))
											{
												modelList = accDoorBll.GetModelList("device_id = " + this.levelMachines[m].ID + " and id in (select accdoor_id from acc_levelset_door_group where acclevelset_id = " + modelList4[n].id + ")");
												if (modelList != null && modelList.Count > 0)
												{
													CommandServer.AddUserAuthorize(this.levelMachines[m], dictionary4[modelList4[n].id], list, modelList);
													continue;
												}
												return;
											}
										}
									}
								}
								finally
								{
									DevCmds.RunImmediatelyEnabled = false;
								}
							}
						}
						FrmShowUpdata.Instance.ShowEx(true, true, true);
					}
					else
					{
						DevCmds.RunImmediatelyEnabled = true;
						for (int num = 0; num < this.oldLevelMachines.Count; num++)
						{
							if (this.oldLevelMachines[num].DevSDKType != SDKType.StandaloneSDK)
							{
								CommandServer.DeleteFace(this.oldLevelMachines[num], list, true, out text2);
								CommandServer.DeleteFingerVein(this.oldLevelMachines[num], list, true, out text2);
								CommandServer.DeleteFingerPrint(this.oldLevelMachines[num], list, true, out text2);
								CommandServer.DeleteUserAuthorize(this.oldLevelMachines[num], list, true, out text2);
							}
							CommandServer.DeleteUser(this.oldLevelMachines[num], list, true, out text2);
						}
						DevCmds.RunImmediatelyEnabled = false;
						for (int num2 = 0; num2 < this.levelMachines.Count; num2++)
						{
							try
							{
								DevCmds.RunImmediatelyEnabled = true;
								if (this.previousUserWithCard > 0)
								{
									List<UserInfo> modelList6 = this.userInfoBll.GetModelList("USERID = " + this.previousUserWithCard);
									if (modelList6.Count > 0)
									{
										userId = modelList6[0].UserId;
										string text3 = userId.ToString();
										if (this.levelMachines[num2].DevSDKType != SDKType.StandaloneSDK)
										{
											CommandServer.DeleteFace(this.levelMachines[num2], modelList6, true, out text2);
											CommandServer.DeleteFingerVein(this.levelMachines[num2], modelList6, true, out text2);
											CommandServer.DeleteFingerPrint(this.levelMachines[num2], modelList6, true, out text2);
											CommandServer.DeleteUserAuthorize(this.levelMachines[num2], modelList6, true, out text2);
										}
										CommandServer.DeleteUser(this.levelMachines[num2], modelList6, true, out text2);
									}
								}
								CommandServer.AddUser(this.levelMachines[num2], list, dictionary3);
								CommandServer.AddUserPhoto(this.levelMachines[num2], list);
								CommandServer.AddUserVerifyType(this.levelMachines[num2], list2);
								CommandServer.AddFingerPrint(this.levelMachines[num2], list);
								CommandServer.AddFingerVein(this.levelMachines[num2], list);
								CommandServer.AddFace(this.levelMachines[num2], list);
								CommandServer.AddElevator(this.levelMachines[num2], list, dictionary3);
								modelList4 = accLevelsetBll.GetModelList("id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + this.levelMachines[num2].ID + ") and id in(select acclevelset_id from acc_levelset_emp where employee_id=" + newUser.UserId + ")", true);
								if (modelList4 != null)
								{
									for (int num3 = 0; num3 < modelList4.Count; num3++)
									{
										if (dictionary4.ContainsKey(modelList4[num3].id))
										{
											modelList = accDoorBll.GetModelList("device_id = " + this.levelMachines[num2].ID + " and id in (select accdoor_id from acc_levelset_door_group where acclevelset_id = " + modelList4[num3].id + ")");
											if (modelList != null && modelList.Count > 0)
											{
												CommandServer.AddUserAuthorize(this.levelMachines[num2], dictionary4[modelList4[num3].id], list, modelList);
											}
										}
									}
								}
							}
							finally
							{
								DevCmds.RunImmediatelyEnabled = false;
							}
						}
						FrmShowUpdata.Instance.ShowEx(true, true, true);
					}
				}
				else if (!string.IsNullOrEmpty(this.newEmpLevelIDs) && string.IsNullOrEmpty(this.oldEmpLevelIDs))
				{
					for (int num4 = 0; num4 < this.levelMachines.Count; num4++)
					{
						try
						{
							DevCmds.RunImmediatelyEnabled = true;
							if (this.previousUserWithCard > 0)
							{
								List<UserInfo> modelList7 = this.userInfoBll.GetModelList("USERID = " + this.previousUserWithCard);
								if (modelList7.Count > 0)
								{
									userId = modelList7[0].UserId;
									string text4 = userId.ToString();
									if (this.levelMachines[num4].DevSDKType != SDKType.StandaloneSDK)
									{
										CommandServer.DeleteFace(this.levelMachines[num4], modelList7, true, out text2);
										CommandServer.DeleteFingerVein(this.levelMachines[num4], modelList7, true, out text2);
										CommandServer.DeleteFingerPrint(this.levelMachines[num4], modelList7, true, out text2);
										CommandServer.DeleteUserAuthorize(this.levelMachines[num4], modelList7, true, out text2);
									}
									CommandServer.DeleteUser(this.levelMachines[num4], modelList7, true, out text2);
								}
							}
							CommandServer.AddUser(this.levelMachines[num4], list, dictionary3);
							CommandServer.AddUserPhoto(this.levelMachines[num4], list);
							CommandServer.AddUserVerifyType(this.levelMachines[num4], list2);
							CommandServer.AddFingerPrint(this.levelMachines[num4], list);
							CommandServer.AddFingerVein(this.levelMachines[num4], list);
							CommandServer.AddFace(this.levelMachines[num4], list);
							CommandServer.AddElevator(this.levelMachines[num4], list, dictionary3);
							modelList4 = accLevelsetBll.GetModelList("id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + this.levelMachines[num4].ID + ") and id in(select acclevelset_id from acc_levelset_emp where employee_id=" + newUser.UserId + ")", true);
							if (modelList4 != null)
							{
								for (int num5 = 0; num5 < modelList4.Count; num5++)
								{
									if (dictionary4.ContainsKey(modelList4[num5].id))
									{
										modelList = accDoorBll.GetModelList("device_id = " + this.levelMachines[num4].ID + " and id in (select accdoor_id from acc_levelset_door_group where acclevelset_id = " + modelList4[num5].id + ")");
										if (modelList != null && modelList.Count > 0)
										{
											CommandServer.AddUserAuthorize(this.levelMachines[num4], dictionary4[modelList4[num5].id], list, modelList);
											continue;
										}
										return;
									}
								}
							}
						}
						finally
						{
							DevCmds.RunImmediatelyEnabled = false;
						}
					}
					FrmShowUpdata.Instance.ShowEx(true, true, true);
				}
				else if (string.IsNullOrEmpty(this.newEmpLevelIDs) && !string.IsNullOrEmpty(this.oldEmpLevelIDs))
				{
					for (int num6 = 0; num6 < this.oldLevelMachines.Count; num6++)
					{
						try
						{
							DevCmds.RunImmediatelyEnabled = true;
							if (this.previousUserWithCard > 0)
							{
								List<UserInfo> modelList8 = this.userInfoBll.GetModelList("USERID = " + this.previousUserWithCard);
								if (modelList8.Count > 0)
								{
									userId = modelList8[0].UserId;
									string text5 = userId.ToString();
									if (this.levelMachines[num6].DevSDKType != SDKType.StandaloneSDK)
									{
										CommandServer.DeleteFace(this.levelMachines[num6], modelList8, true, out text2);
										CommandServer.DeleteFingerVein(this.levelMachines[num6], modelList8, true, out text2);
										CommandServer.DeleteFingerPrint(this.levelMachines[num6], modelList8, true, out text2);
										CommandServer.DeleteUserAuthorize(this.levelMachines[num6], modelList8, true, out text2);
									}
									CommandServer.DeleteUser(this.levelMachines[num6], modelList8, true, out text2);
								}
							}
							if (this.oldLevelMachines[num6].DevSDKType != SDKType.StandaloneSDK)
							{
								CommandServer.DeleteFace(this.oldLevelMachines[num6], list, true, out text2);
								CommandServer.DeleteFingerVein(this.oldLevelMachines[num6], list, true, out text2);
								CommandServer.DeleteFingerPrint(this.oldLevelMachines[num6], list, true, out text2);
								CommandServer.DeleteUserAuthorize(this.oldLevelMachines[num6], list, true, out text2);
							}
							CommandServer.DeleteUser(this.oldLevelMachines[num6], list, true, out text2);
						}
						finally
						{
							DevCmds.RunImmediatelyEnabled = false;
						}
					}
					FrmShowUpdata.Instance.ShowEx(true, true, true);
				}
			}
		}

		private bool Save()
		{
			try
			{
				if (this.CheckData())
				{
					UserInfo userInfo = null;
					IVisitInfo dALObject = VisitInfoBll.getDALObject();
					IVisitInfo dALObject2 = VisitInfoBll.getDALObject();
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
					int visitorIDByCard = dALObject2.GetVisitorIDByCard(this.txt_cardNO.Text);
					this.previousUserWithCard = ((visitorIDByCard > 0) ? visitorIDByCard : 0);
					if (this._releaseCardNumber)
					{
						List<UserInfo> modelList = this.userInfoBll.GetModelList("CardNo='" + DBHelper.NoSqlInjection(this.txt_cardNO.Text) + "'");
						if (modelList.Count > 0)
						{
							VisitInfo model = dALObject2.GetModel(modelList[0].UserId);
							if (model != null)
							{
								UserInfo userInfo2 = new UserInfo();
								userInfo2.BadgeNumber = model.pin;
								userInfo2.CardNo = model.card;
							}
							dALObject2.EndVisit(modelList[0].UserId);
							personnelIssuecardBll.DeleteByUserId(modelList[0].UserId);
						}
					}
					if (this.m_id > 0)
					{
						userInfo = this.userInfoBll.GetModel(this.m_id);
					}
					if (userInfo == null)
					{
						userInfo = new UserInfo();
					}
					this.SetModel(userInfo);
					bool flag = false;
					if (userInfo.UserId > 0)
					{
						try
						{
							this.userInfoBll.Update(userInfo);
							if (!this.m_visitorActive)
							{
								dALObject.Add(userInfo);
							}
							flag = true;
						}
						catch (Exception ex)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("ExceptionOnSaveDataFailure", "保存数据失败") + ": " + ex.Message);
						}
					}
					else
					{
						try
						{
							this.userInfoBll.Add(userInfo, "S");
							userInfo.UserId = this.userInfoBll.GetMaxId() - 1;
							this.Add_Event_Userid = userInfo.UserId;
							if (!this.m_visitorActive)
							{
								dALObject.Add(userInfo);
							}
							flag = true;
							if (this.tempList.Count > 0)
							{
								this.SaveTemplate(userInfo.UserId);
							}
						}
						catch (Exception ex2)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("ExceptionOnSaveDataFailure", "保存数据失败") + ": " + ex2.Message);
						}
					}
					if (flag && userInfo.UserId > 0)
					{
						this.SaveCard(userInfo);
						this.SaveLevelset(userInfo.UserId);
						this.CheckLevelChange(userInfo, this.oldUser, this.tempList.Count);
						if (this.refreshDataEvent != null)
						{
							this.refreshDataEvent(this, null);
						}
						return true;
					}
				}
				return false;
			}
			catch (Exception ex3)
			{
				SysDialogs.ShowWarningMessage(ex3.Message);
				return false;
			}
		}

		private void SaveLevelset(int userid)
		{
			bool flag = false;
			try
			{
				AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
				if (this.m_id > 0)
				{
					flag = accLevelsetEmpBll.DeleteEmployee(userid);
				}
				List<AccLevelsetEmp> list = new List<AccLevelsetEmp>();
				for (int i = 0; i < this.m_browDataTable.Rows.Count; i++)
				{
					if (this.m_browDataTable.Rows[i][0] != null)
					{
						AccLevelsetEmp accLevelsetEmp = new AccLevelsetEmp();
						accLevelsetEmp.acclevelset_id = int.Parse(this.m_browDataTable.Rows[i][0].ToString());
						accLevelsetEmp.employee_id = userid;
						this.newEmpLevelIDs = this.newEmpLevelIDs + this.m_browDataTable.Rows[i][0].ToString() + ",";
						list.Add(accLevelsetEmp);
					}
				}
				accLevelsetEmpBll.Add(list);
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				this.levelMachines = machinesBll.GetModelList("id in(select  device_id from acc_door where id in(select  accdoor_id from acc_levelset_door_group where acclevelset_id in(select acclevelset_id from acc_levelset_emp where employee_id=" + userid + ")))");
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("AddLevelFailure", "添加权限失败!"));
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			if (this.Save())
			{
				base.Close();
			}
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void BrowseBtn_Click(object sender, EventArgs e)
		{
			this.openFileDialog1.Filter = "Supported Image Types (*.jpg, *.gif, *.bmp)|*.jpg;*.gif;*.bmp|JPEG Image|*.jpg|GIF Image|*.gif|BITMAP Image|*.bmp";
			this.openFileDialog1.FileName = "";
			this.openFileDialog1.ShowDialog();
			this.fileName = this.openFileDialog1.FileName;
			if (this.fileName != "")
			{
				try
				{
					FileStream fileStream = new FileStream(this.fileName, FileMode.Open);
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
					fileStream.Close();
					MemoryStream memoryStream = new MemoryStream(array);
					this.pic_photo.Image = Image.FromStream(memoryStream);
					this.pic_photo.Image = ZKImage.ResizeImage(140, 120, this.pic_photo.Image);
					this.PhotoChanged = true;
					memoryStream.Close();
					Size size = this.pic_photo.Image.Size;
					int width = size.Width;
					size = this.pic_photo.Size;
					int num;
					if (width <= size.Width)
					{
						size = this.pic_photo.Image.Size;
						int height = size.Height;
						size = this.pic_photo.Size;
						num = ((height > size.Height) ? 1 : 0);
					}
					else
					{
						num = 1;
					}
					if (num != 0)
					{
						this.pic_photo.SizeMode = PictureBoxSizeMode.Zoom;
					}
					else
					{
						this.pic_photo.SizeMode = PictureBoxSizeMode.CenterImage;
					}
					fileStream = null;
					memoryStream = null;
				}
				catch
				{
				}
			}
		}

		private void InitSensor(string FingerSign, object obj)
		{
			AFXOnlineMainClass aFXOnlineMainClass = null;
			if (obj != null)
			{
				aFXOnlineMainClass = (obj as AFXOnlineMainClass);
			}
			if (aFXOnlineMainClass != null)
			{
				aFXOnlineMainClass.FPEngineVersion = "10";
				aFXOnlineMainClass.EnrollCount = 3;
				try
				{
					aFXOnlineMainClass.DisableSound = true;
				}
				catch
				{
				}
				aFXOnlineMainClass.SetLanguageFile("online." + initLang.Lang);
				aFXOnlineMainClass.SetVerHint = "Register fingerprint";
				aFXOnlineMainClass.IsSupportDuress = true;
				aFXOnlineMainClass.CheckFinger = FingerSign;
			}
		}

		private object GetAFXOnlineMain()
		{
			try
			{
				AFXOnlineMainClass aFXOnlineMainClass = null;
				return new AFXOnlineMainClass();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInstallSensorDriver", "请安装指纹仪驱动"));
				return null;
			}
		}

		private void SaveTemplate(int userID)
		{
			try
			{
				if (this.templates.Count > 0)
				{
					TemplateBll templateBll = new TemplateBll(MainForm._ia);
					for (int i = 0; i < 10 && this.m_oldfingerSign.Length > i; i++)
					{
						string text = this.m_oldfingerSign.Substring(i, 1);
						if ((text == "1" || text == "3") && this.templates.ContainsKey(i))
						{
							Template template = new Template();
							template.USERID = userID;
							template.DivisionFP = 10;
							template.FINGERID = i;
							template.Flag = Convert.ToInt16(text);
							template.TEMPLATE4 = this.templates[i][0];
							template.TEMPLATE3 = this.templates[i][1];
							templateBll.Add(template);
						}
					}
					this.tempList.Clear();
					this.m_oldfingerSign = "0000000000";
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				this.isClickTemplateBtn = true;
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				Dictionary<int, Template> dictionary = new Dictionary<int, Template>();
				string text = "0000000000";
				if (this.m_id < 1)
				{
					if (this.tempList.Count > 0)
					{
						text = this.m_oldfingerSign;
					}
				}
				else
				{
					this.tempList.Clear();
					this.m_oldfingerSign = "0000000000";
					List<Template> modelList = templateBll.GetModelList("userid=" + this.m_id.ToString());
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							if (modelList[i].FINGERID >= 0 && modelList[i].FINGERID < 10 && !dictionary.ContainsKey(modelList[i].FINGERID))
							{
								dictionary.Add(modelList[i].FINGERID, modelList[i]);
								text = text.Remove(modelList[i].FINGERID, 1).Insert(modelList[i].FINGERID, modelList[i].Flag.ToString());
							}
						}
					}
				}
				AFXOnlineMainClass aFXOnlineMainClass = null;
				object aFXOnlineMain = this.GetAFXOnlineMain();
				if (aFXOnlineMain != null)
				{
					aFXOnlineMainClass = (aFXOnlineMain as AFXOnlineMainClass);
				}
				if (aFXOnlineMainClass != null)
				{
					try
					{
						this.InitSensor(text, aFXOnlineMainClass);
					}
					catch
					{
						SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SInitSensorFailed", "初始化指纹仪失败"));
						return;
					}
					if (aFXOnlineMainClass.Register())
					{
						for (int j = 0; j < 10; j++)
						{
							string regFingerTemplateEx = aFXOnlineMainClass.GetRegFingerTemplateEx("10", j + 1);
							string regFingerTemplateEx2 = aFXOnlineMainClass.GetRegFingerTemplateEx("9", j + 1);
							if (!string.IsNullOrEmpty(regFingerTemplateEx) || !string.IsNullOrEmpty(regFingerTemplateEx2))
							{
								if (regFingerTemplateEx.Length < 800 || regFingerTemplateEx.Length == regFingerTemplateEx2.Length)
								{
									SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SFingerprintBad", "指纹模板错误，请立即联系开发人员!"));
									return;
								}
								if ((regFingerTemplateEx2.Length < 100 && regFingerTemplateEx.Length > 100) || (regFingerTemplateEx2.Length > 100 && regFingerTemplateEx.Length < 100))
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFPBadRegister", "指纹模板错误，请重新登记!"));
									text = "0000000000";
									break;
								}
								if (this.tempList.ContainsKey(j))
								{
									this.tempList.Remove(j);
								}
								List<string> list = new List<string>();
								if (!string.IsNullOrEmpty(regFingerTemplateEx))
								{
									list.Add(regFingerTemplateEx);
								}
								else
								{
									list.Add("");
								}
								if (!string.IsNullOrEmpty(regFingerTemplateEx2))
								{
									list.Add(regFingerTemplateEx2);
								}
								else
								{
									list.Add("");
								}
								this.tempList.Add(j, list);
							}
						}
					}
					text = (this.m_oldfingerSign = aFXOnlineMainClass.CheckFinger);
					if (this.m_id > 0)
					{
						for (int k = 0; k < 10; k++)
						{
							if (text.Length > k)
							{
								string a = text.Substring(k, 1);
								if (a == "0")
								{
									if (dictionary.ContainsKey(k))
									{
										CommandServer.DeleteTemplateCmd(this.oldUser, this.oldLevelMachines);
										break;
									}
								}
								else if ((a == "1" || a == "3") && this.tempList.ContainsKey(k) && dictionary.ContainsKey(k))
								{
									CommandServer.DeleteTemplateCmd(this.oldUser, this.oldLevelMachines);
									break;
								}
							}
						}
						for (int l = 0; l < 10 && text.Length > l; l++)
						{
							string text2 = text.Substring(l, 1);
							if (text2 == "0")
							{
								if (dictionary.ContainsKey(l))
								{
									templateBll.Delete(dictionary[l].TEMPLATEID);
								}
							}
							else if ((text2 == "1" || text2 == "3") && this.tempList.ContainsKey(l))
							{
								if (dictionary.ContainsKey(l))
								{
									templateBll.Delete(dictionary[l].TEMPLATEID);
								}
								Template template = new Template();
								template.USERID = this.m_id;
								template.DivisionFP = 10;
								template.Fpversion = "10";
								template.FINGERID = l;
								template.Flag = Convert.ToInt16(text2);
								object obj2 = new object();
								bool flag = aFXOnlineMainClass.DecodeTemplate1(this.tempList[l][0], ref obj2);
								template.TEMPLATE4 = (byte[])obj2;
								flag = aFXOnlineMainClass.DecodeTemplate1(this.tempList[l][1], ref obj2);
								template.TEMPLATE3 = (byte[])obj2;
								templateBll.Add(template);
							}
						}
					}
					else if (this.tempList.Count > 0)
					{
						this.templates.Clear();
						for (int m = 0; m < 10 && this.m_oldfingerSign.Length > m; m++)
						{
							string a2 = this.m_oldfingerSign.Substring(m, 1);
							if ((a2 == "1" || a2 == "3") && this.tempList.ContainsKey(m))
							{
								object obj3 = new object();
								bool flag2 = aFXOnlineMainClass.DecodeTemplate1(this.tempList[m][0], ref obj3);
								byte[] item = (byte[])obj3;
								flag2 = aFXOnlineMainClass.DecodeTemplate1(this.tempList[m][1], ref obj3);
								byte[] item2 = (byte[])obj3;
								List<byte[]> list2 = new List<byte[]>();
								list2.Add(item);
								list2.Add(item2);
								this.templates.Add(m, list2);
							}
						}
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

		private void btn_saveContinue_Click(object sender, EventArgs e)
		{
			if (this.Save())
			{
				try
				{
					ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PersonnelManagementForm));
					this.txt_personnel.Text = this.GetMaxPin();
					this.txt_name.Text = "";
					this.txt_cardNO.Text = "";
					this.cardNumber = "";
					this.cbo_gender.Text = "";
					this.pic_photo.Image = (Image)componentResourceManager.GetObject("pic_photo.Image");
					this.PhotoChanged = false;
					Size size = this.pic_photo.Image.Size;
					int width = size.Width;
					size = this.pic_photo.Size;
					int num;
					if (width <= size.Width)
					{
						size = this.pic_photo.Image.Size;
						int height = size.Height;
						size = this.pic_photo.Size;
						num = ((height > size.Height) ? 1 : 0);
					}
					else
					{
						num = 1;
					}
					if (num != 0)
					{
						this.pic_photo.SizeMode = PictureBoxSizeMode.Zoom;
					}
					else
					{
						this.pic_photo.SizeMode = PictureBoxSizeMode.CenterImage;
					}
					this.chk_setValidTime.Checked = false;
					this.txt_password.Text = "";
					this.txt_jobTitle.Text = "";
					this.txt_mobilPhone.Text = "";
					this.DTPick_birthday.Text = "";
					this.txt_workAddress.Text = "";
					this.txt_city.Text = "";
					this.txt_nationality.Text = "";
					this.txt_postalCode.Text = "";
					this.txt_officeTelephone.Text = "";
					this.txt_homeTelephone.Text = "";
					this.txt_lastName.Text = "";
					this.txt_IDnumber.Text = "";
					this.txt_education.Text = "";
					this.txt_origin.Text = "";
					this.txt_politicalStatus.Text = "";
					this.txt_nationality.Text = "";
					this.txt_email.Text = "";
					this.txt_homeAddress.Text = "";
					this.txt_ethnic.Text = "";
					this.cbo_datStart.Text = null;
					this.cbo_datEnd.Text = null;
					this.m_browDataTable.Rows.Clear();
					this.LevelDataBind();
					this.cbo_gender.SelectedValue = "M";
					if (this.cbo_MultiCard.Items.Count > 0)
					{
						this.cbo_MultiCard.SelectedIndex = 0;
					}
					this.CheckBtn();
				}
				catch
				{
				}
				this.DTPick_birthday.EditValue = DateTime.Now;
				this.dicFingerId_FpTemplate = null;
				this.dicFingerId_FpTemplate_Old = null;
				this.lnkRegFpViaDev.Enabled = false;
				this.tabControl1.SelectedTab = this.tabItem_basic;
				this.btn_allLMove_Click(sender, e);
			}
		}

		private void txt_personnel_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == ',')
			{
				e.Handled = true;
			}
			CheckInfo.KeyPress(sender, e, 9);
		}

		private void txt_cardNO_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (AccCommon.CodeVersion)
			{
			case CodeVersionType.JapanAF:
				if (e.KeyChar != '\b' && e.KeyChar != '\r' && (e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'A' || e.KeyChar > 'F') && (e.KeyChar < 'a' || e.KeyChar > 'f'))
				{
					e.Handled = true;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SHexNumberOnly", "仅支持输入十六进制数"));
				}
				else if (e.KeyChar == '0' && this.txt_cardNO.SelectionStart == 0 && this.txt_cardNO.TextLength > this.txt_cardNO.SelectionLength)
				{
					e.Handled = true;
				}
				else if (this.txt_cardNO.Text == "0" && e.KeyChar != '\b' && e.KeyChar != '\r' && this.txt_cardNO.SelectionLength == 0 && this.txt_cardNO.SelectionStart == 1)
				{
					this.txt_cardNO.Text = e.KeyChar.ToString();
					this.txt_cardNO.SelectionStart = 1;
					e.Handled = true;
				}
				else if (this.txt_cardNO.SelectionLength == 0 && this.txt_cardNO.Text.Length >= 16 && e.KeyChar != '\b' && e.KeyChar != '\r')
				{
					e.Handled = true;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputIsOutLen", "输入超出有效长度"));
				}
				break;
			case CodeVersionType.Main:
				CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
				break;
			default:
				CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
				break;
			}
		}

		private void btn_allRMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_dataTable.Rows.Count > 0)
				{
					for (int i = 0; i < this.m_dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_browDataTable.NewRow();
						dataRow[0] = this.m_dataTable.Rows[i][0].ToString();
						dataRow[1] = this.m_dataTable.Rows[i][1].ToString();
						dataRow[2] = this.m_dataTable.Rows[i][2].ToString();
						dataRow[3] = false;
						this.m_browDataTable.Rows.Add(dataRow);
					}
					this.m_dataTable.Rows.Clear();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoLevel", "没有门禁权限组"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_RMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_dataTable.Rows.Count > 0)
				{
					int[] array = null;
					if (this.isDouble)
					{
						array = this.gridView1.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.gridView1, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.gridView1, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_dataTable.Rows.Count)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] >= 0 && array[i] < this.m_dataTable.Rows.Count)
							{
								DataRow dataRow = this.m_browDataTable.NewRow();
								dataRow[0] = this.m_dataTable.Rows[array[i]][0].ToString();
								dataRow[1] = this.m_dataTable.Rows[array[i]][1].ToString();
								dataRow[2] = this.m_dataTable.Rows[array[i]][2].ToString();
								dataRow[3] = false;
								this.m_browDataTable.Rows.Add(dataRow);
								this.m_dataTable.Rows[array[i]][0] = -1;
							}
						}
						for (int j = 0; j < this.m_dataTable.Rows.Count; j++)
						{
							if (this.m_dataTable.Rows[j][0].ToString() == "-1")
							{
								this.m_dataTable.Rows.RemoveAt(j);
								j--;
							}
						}
						if (this.m_dataTable.Rows.Count > 0)
						{
							this.gridView1.SelectRow(0);
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneLevelData", "请选择门禁权限组"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoLevel", "没有门禁权限组"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_LMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browDataTable.Rows.Count > 0)
				{
					int[] array = null;
					if (this.isDouble)
					{
						array = this.gridView2.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.gridView2, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.gridView2, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_browDataTable.Rows.Count)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] >= 0 && array[i] < this.m_browDataTable.Rows.Count)
							{
								DataRow dataRow = this.m_dataTable.NewRow();
								dataRow[0] = this.m_browDataTable.Rows[array[i]][0].ToString();
								dataRow[1] = this.m_browDataTable.Rows[array[i]][1].ToString();
								dataRow[2] = this.m_browDataTable.Rows[array[i]][2].ToString();
								dataRow[3] = false;
								this.m_dataTable.Rows.Add(dataRow);
								this.m_browDataTable.Rows[array[i]][0] = -1;
							}
						}
						for (int j = 0; j < this.m_browDataTable.Rows.Count; j++)
						{
							if (this.m_browDataTable.Rows[j][0].ToString() == "-1")
							{
								this.m_browDataTable.Rows.RemoveAt(j);
								j--;
							}
						}
						if (this.m_browDataTable.Rows.Count > 0)
						{
							this.gridView2.SelectRow(0);
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneLevelData", "请选择门禁权限组"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoLevel", "没有门禁权限组"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_allLMove_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browDataTable.Rows.Count > 0)
				{
					for (int i = 0; i < this.m_browDataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_dataTable.NewRow();
						dataRow[0] = this.m_browDataTable.Rows[i][0].ToString();
						dataRow[1] = this.m_browDataTable.Rows[i][1].ToString();
						dataRow[2] = this.m_browDataTable.Rows[i][2].ToString();
						dataRow[3] = false;
						this.m_dataTable.Rows.Add(dataRow);
					}
					this.m_browDataTable.Rows.Clear();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoLevel", "没有门禁权限组"));
				}
				this.CheckBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == ',')
			{
				e.Handled = true;
			}
			CheckInfo.KeyPress(sender, e, 24);
		}

		private void txt_email_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 40);
		}

		private void txt_lastName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 40);
		}

		private void txt_mobilPhone_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 99999999999L);
		}

		private void txt_password_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 6);
		}

		private void txt_origin_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_nationality_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_education_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_city_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 40);
		}

		private void txt_homeTelephone_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 12);
		}

		private void txt_IDnumber_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 18);
		}

		private void txt_politicalStatus_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_ethnic_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_officeTelephone_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 15);
		}

		private void txt_jobTitle_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_postalCode_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e);
		}

		private void txt_workAddress_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 50);
		}

		private void txt_homeAddress_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 50);
		}

		private void link_read_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.readStart)
			{
				this.txt_cardNO.Text = "";
				DeviceTreeForm deviceTreeForm = new DeviceTreeForm(true, "");
				deviceTreeForm.SelectDeviceEvent += this.devic_Event;
				deviceTreeForm.ShowDialog();
				deviceTreeForm.SelectDeviceEvent -= this.devic_Event;
			}
			else
			{
				this.StopServerRead();
			}
		}

		private void StopServerRead()
		{
			if (DeviceServers.Instance.Count > 0)
			{
				for (int i = 0; i < DeviceServers.Instance.Count; i++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[i];
					deviceServerBll.CardNoRegEvent -= this.server_RTLogEvent;
					deviceServerBll.SwippingCard -= this.server_RTLogEvent;
				}
			}
			if (this.DevMonitor != null)
			{
				this.DevMonitor.StopMonitor();
				this.DevMonitor.ClearDeviceServer();
				this.DevMonitor = null;
			}
			this.link_read.Text = ShowMsgInfos.GetInfo("SStartReading", "读取");
			this.link_read.Image = Resource.punchCard;
			this.pic_Readding.Visible = false;
			this.readStart = true;
		}

		private void devic_Event(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					List<AccDoor> list = this.m_selectedDoor = (sender as List<AccDoor>);
					if (this.m_selectedDoor != null && this.m_selectedDoor.Count > 0)
					{
						bool flag = false;
						for (int i = 0; i < DeviceServers.Instance.Count; i++)
						{
							DeviceServerBll deviceServerBll = DeviceServers.Instance[i];
							if (this.m_selectedDoor != null && this.m_selectedDoor.Count > 0)
							{
								bool flag2 = false;
								int num = 0;
								while (num < this.m_selectedDoor.Count)
								{
									if (this.m_selectedDoor[num].device_id != deviceServerBll.DevInfo.ID)
									{
										num++;
										continue;
									}
									flag2 = true;
									break;
								}
								if (flag2)
								{
									if (deviceServerBll.IsConnected)
									{
										goto IL_0107;
									}
									int num2 = deviceServerBll.Connect(3000);
									if (num2 >= 0)
									{
										goto IL_0107;
									}
								}
							}
							continue;
							IL_0107:
							deviceServerBll.IsNeedListen = true;
							flag = true;
							this.link_read.Text = ShowMsgInfos.GetInfo("SStopReading", "停止");
							this.link_read.Image = Resource.StopPunchCard;
							if (deviceServerBll.DevInfo.DevSDKType != SDKType.StandaloneSDK)
							{
								deviceServerBll.CardNoRegEvent += this.server_RTLogEvent;
								this.DevMonitor = new PullDeviceMonitor(0);
								this.DevMonitor.AddDeviceServer(deviceServerBll);
								this.DevMonitor.StartMonitor();
							}
							else
							{
								int num3 = 0;
								deviceServerBll.GetRTLogs(ref num3);
								deviceServerBll.SwippingCard += this.server_RTLogEvent;
								this.DevMonitor = new StdDeviceMonitor(0);
								this.DevMonitor.AddDeviceServer(deviceServerBll);
								this.DevMonitor.StartMonitor();
							}
							this.pic_Readding.Visible = true;
							this.readStart = false;
						}
						if (!flag)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("NoDeviceServer", "没有刷卡设备，读取失败"));
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void RTLogEvent(ObjRTLogInfo info)
		{
			try
			{
				if (info != null && (info.EType == EventType.Type27 || info.EType == EventType.Type1027) && !string.IsNullOrEmpty(info.CardNo))
				{
					bool flag = false;
					if (this.m_selectedDoor != null && this.m_selectedDoor.Count > 0)
					{
						int num = 0;
						while (num < this.m_selectedDoor.Count)
						{
							if (this.m_selectedDoor[num].device_id != info.DevID || !(this.m_selectedDoor[num].door_no.ToString() == info.DoorID))
							{
								num++;
								continue;
							}
							flag = true;
							break;
						}
					}
					else
					{
						flag = false;
					}
					if (flag)
					{
						if (info.CardNo != "0")
						{
							if (long.TryParse(info.CardNo, out long num2))
							{
								uint num3 = (uint)num2;
								this.txt_cardNO.Text = num3.ToString();
							}
							this.StopServerRead();
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInvalidCard", "无效卡！"));
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void server_RTLogEvent(ObjDevice sender, ObjRTLogInfo info)
		{
			if (!base.IsDisposed && base.Visible)
			{
				if (base.InvokeRequired)
				{
					base.BeginInvoke((MethodInvoker)delegate
					{
						this.server_RTLogEvent(sender, info);
					});
				}
				else
				{
					this.RTLogEvent(info);
				}
			}
		}

		private void txt_cardNO_MouseLeave(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_cardNO.Text))
			{
				this.link_read.Enabled = true;
			}
		}

		private void dgrd_optionalLevel_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_RMove_Click(sender, e);
			this.isDouble = false;
		}

		private void dgrd_selectedLevel_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_LMove_Click(sender, e);
			this.isDouble = false;
		}

		private void gridView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void gridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void link_read_LinkClicked(object sender, EventArgs e)
		{
			if (this.readStart)
			{
				DeviceTreeForm deviceTreeForm = new DeviceTreeForm(true, "DevSDKType <> 2 or (DevSDKType=2 and (CardFun=1 or CardFun=2))");
				deviceTreeForm.lb_info.Visible = true;
				deviceTreeForm.lb_info.Text = ShowMsgInfos.GetInfo("SOnlyReadUnregistered", "刷卡时，仅读取设备中未注册的卡！");
				deviceTreeForm.SelectDeviceEvent += this.devic_Event;
				deviceTreeForm.ShowDialog();
				deviceTreeForm.SelectDeviceEvent -= this.devic_Event;
			}
			else
			{
				this.StopServerRead();
			}
		}

		private void link_read_MouseEnter(object sender, EventArgs e)
		{
			if (this.readStart)
			{
				this.toolTip1.SetToolTip(this.link_read, ShowMsgInfos.GetInfo("SControlIssueCard", "门禁控制器发卡"));
			}
			else
			{
				this.toolTip1.SetToolTip(this.link_read, ShowMsgInfos.GetInfo("SStopIssueCard", "停止读卡"));
			}
		}

		private void txt_name_TextChanged(object sender, EventArgs e)
		{
		}

		private void ckbReadMiCard_MouseEnter(object sender, EventArgs e)
		{
		}

		private void ckbReadMiCard_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void lnk_ClearAntyBack_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.oldUser == null || string.IsNullOrEmpty(this.oldUser.BadgeNumber) || int.Parse(this.oldUser.BadgeNumber) <= 0 || this.oldLevelMachines == null || this.oldLevelMachines.Count <= 0)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("NoLevelMachines", "没有加入权限组"));
			}
			else
			{
				WaitForm instance = WaitForm.Instance;
				instance.ShowProgress(0);
				instance.ShowEx();
				Application.DoEvents();
				for (int i = 0; i < this.oldLevelMachines.Count; i++)
				{
					try
					{
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(this.oldLevelMachines[i]);
						if (deviceServer == null)
						{
							instance.ShowInfos(this.oldLevelMachines[i].MachineAlias + ":" + PullSDkErrorInfos.GetInfo(-1002));
							instance.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.oldLevelMachines.Count * 100m));
						}
						else
						{
							int num;
							if (!deviceServer.IsConnected)
							{
								num = deviceServer.Connect(3000);
								if (num < 0)
								{
									instance.ShowInfos(this.oldLevelMachines[i].MachineAlias + ":" + PullSDkErrorInfos.GetInfo(num));
									instance.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.oldLevelMachines.Count * 100m));
									goto end_IL_008c;
								}
							}
							num = deviceServer.ControlDevice(17, int.Parse(this.oldUser.BadgeNumber), 0, 0, 0, "");
							if (num < 0)
							{
								instance.ShowInfos(this.oldLevelMachines[i].MachineAlias + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							else
							{
								instance.ShowInfos(this.oldLevelMachines[i].MachineAlias + ":" + ShowMsgInfos.GetInfo("ClearAntyBackSuceed", "清除成功"));
							}
							instance.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.oldLevelMachines.Count * 100m));
						}
						end_IL_008c:;
					}
					catch (Exception ex)
					{
						instance.ShowProgress((int)Math.Ceiling((decimal)(i + 1) / (decimal)this.oldLevelMachines.Count * 100m));
						instance.ShowInfos(this.oldLevelMachines[i].MachineAlias + ":" + ex.Message);
					}
				}
				instance.ShowProgress(100);
				instance.HideEx(false);
			}
		}

		private string GetMaxPin()
		{
			string strWhere = (AppSite.Instance.DataType != 0) ? "CAST(BadgeNumber as int)>=(Select max(CAST(BadgeNumber as int)) from UserInfo)" : "clng(BadgeNumber)>=(Select max(clng(BadgeNumber)) from UserInfo)";
			List<UserInfo> modelList = this.userInfoBll.GetModelList(strWhere);
			if (modelList != null && modelList.Count > 0)
			{
				return (int.Parse(modelList[0].BadgeNumber) + 1).ToString();
			}
			return "1";
		}

		private void LoadFP()
		{
			if (this.dicFingerId_FpTemplate == null)
			{
				this.dicFingerId_FpTemplate = new Dictionary<int, Template>();
			}
			if (this.dicFingerId_FpTemplate_Old == null)
			{
				this.dicFingerId_FpTemplate_Old = new Dictionary<int, Template>();
			}
			TemplateBll templateBll = new TemplateBll(MainForm._ia);
			List<Template> modelList = templateBll.GetModelList("UserId=" + this.m_id);
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!this.dicFingerId_FpTemplate_Old.ContainsKey(modelList[i].FINGERID))
					{
						this.dicFingerId_FpTemplate.Add(modelList[i].FINGERID, modelList[i]);
						this.dicFingerId_FpTemplate_Old.Add(modelList[i].FINGERID, modelList[i]);
					}
				}
			}
		}

		private void lnkRegFpViaDev_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.txt_personnel.Text.Trim() == "")
			{
				this.txt_personnel.Focus();
			}
			else
			{
				DeviceTreeForm deviceTreeForm = new DeviceTreeForm(true, "DevSDKType=2");
				deviceTreeForm.SelectDeviceEvent += this.RegFpViaDev;
				deviceTreeForm.ShowDialog();
				deviceTreeForm.SelectDeviceEvent -= this.RegFpViaDev;
			}
		}

		private void RegFpViaDev(object sender, EventArgs e)
		{
			List<AccDoor> list = null;
			try
			{
				int.TryParse(this.txt_personnel.Text, out int pin);
				this.LoadFP();
				list = (sender as List<AccDoor>);
				if (list != null && 0 < list.Count)
				{
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					Machines model = machinesBll.GetModel(list[0].device_id);
					this.devServer = DeviceServers.Instance.GetDeviceServer(model);
					if (this.devServer != null)
					{
						int num = this.devServer.Connect(3000);
						if (0 > num)
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ConnectFailed", "连接失败") + ": " + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.isClickTemplateBtn = true;
							this.register = new FPRegister(this.devServer, pin, this.dicFingerId_FpTemplate);
							if (this.register.ShowDialog() != DialogResult.OK)
							{
								this.devServer.STD_CancelOperation();
								this.devServer.STD_StartIdentify();
								this.devServer.Disconnect();
							}
							else
							{
								string text = "";
								TemplateBll templateBll = new TemplateBll(MainForm._ia);
								foreach (KeyValuePair<int, Template> item in this.dicFingerId_FpTemplate_Old)
								{
									if (this.register.RegisteredFinger == null || !this.register.RegisteredFinger.ContainsKey(item.Key))
									{
										text += $"{item.Value.TEMPLATEID},";
									}
								}
								if (text.Length > 0)
								{
									text = text.Remove(text.Length - 1, 1);
									templateBll.DeleteList(text);
								}
								if (this.register.RegisteredFinger != null && this.register.RegisteredFinger.Count > 0)
								{
									List<Template> list2 = new List<Template>();
									List<Template> list3 = new List<Template>();
									templateBll = new TemplateBll(MainForm._ia);
									foreach (KeyValuePair<int, Template> item2 in this.register.RegisteredFinger)
									{
										if (this.dicFingerId_FpTemplate_Old.ContainsKey(item2.Key))
										{
											Template template = this.dicFingerId_FpTemplate_Old[item2.Key];
											template.Flag = item2.Value.Flag;
											template.Valid = item2.Value.Flag;
											if (this.devServer.DevInfo.FPVersion == 10)
											{
												template.TEMPLATE4 = item2.Value.TEMPLATE4;
											}
											else
											{
												template.TEMPLATE3 = item2.Value.TEMPLATE3;
											}
											list3.Add(template);
										}
										else
										{
											Template template = new Template();
											template.USERID = this.oldUser.UserId;
											template.FINGERID = item2.Key;
											template.Flag = item2.Value.Flag;
											template.Valid = item2.Value.Flag;
											if (this.devServer.DevInfo.FPVersion == 10)
											{
												template.TEMPLATE4 = item2.Value.TEMPLATE4;
											}
											else
											{
												template.TEMPLATE3 = item2.Value.TEMPLATE3;
											}
											list2.Add(template);
										}
									}
									templateBll.Add(list2);
									templateBll.Update(list3);
								}
								this.devServer.STD_CancelOperation();
								this.devServer.STD_StartIdentify();
								this.devServer.Disconnect();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (this.devServer != null && this.devServer.IsConnected)
				{
					this.devServer.STD_CancelOperation();
					this.devServer.STD_StartIdentify();
				}
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private void DTPick_birthday_EditValueChanged(object sender, EventArgs e)
		{
			if (this.DTPick_birthday.EditValue == null)
			{
				this.DTPick_birthday.EditValue = DateTime.Now;
			}
			DateTime t = (DateTime)this.DTPick_birthday.EditValue;
			if (t > DateTime.Now)
			{
				this.DTPick_birthday.EditValue = DateTime.Now;
			}
		}

		private void cbo_datStart_EditValueChanged(object sender, EventArgs e)
		{
			if (this.cbo_datStart.EditValue == null)
			{
				this.cbo_datStart.EditValue = DateTime.Now;
			}
		}

		private void cbo_datEnd_EditValueChanged(object sender, EventArgs e)
		{
			if (this.cbo_datEnd.EditValue == null)
			{
				this.cbo_datEnd.EditValue = DateTime.Now;
			}
		}

		private void PersonnelManagementForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (DeviceServers.Instance.Count > 0)
			{
				for (int i = 0; i < DeviceServers.Instance.Count; i++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[i];
					deviceServerBll.CardNoRegEvent -= this.server_RTLogEvent;
					deviceServerBll.SwippingCard -= this.server_RTLogEvent;
				}
			}
			if (this.DevMonitor != null)
			{
				this.DevMonitor.StopMonitor();
				this.DevMonitor.ClearDeviceServer();
				this.DevMonitor = null;
			}
		}

		private void bwReader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!base.IsDisposed && base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.bwReader_RunWorkerCompleted(sender, e);
				});
			}
			else
			{
				this.lnkIDReader.Text = "身份证阅读器";
				this.btnIDReader.Text = "身份证阅读器";
				this.PreIDCardNo = null;
				this.lblIDReaderFailed.Visible = false;
			}
		}

		private void bwReader_DoWork(object sender, DoWorkEventArgs e)
		{
			Image img = null;
			int num = 0;
			do
			{
				if (num <= 3)
				{
					IDCardInfo info;
					int num2 = this.idReader.ReadIDInfo(1, out info, out img);
					if (num2 >= 1)
					{
						num = 0;
						if (!base.IsDisposed && base.Visible)
						{
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.lblIDReaderFailed.Visible = false;
								});
							}
							else
							{
								this.lblIDReaderFailed.Visible = false;
							}
						}
						if (info.IDNumber != this.PreIDCardNo)
						{
							this.ShowIDInfo(info, img);
							this.PreIDCardNo = info.IDNumber;
						}
					}
					else
					{
						num++;
						this.PreIDCardNo = null;
					}
				}
				else
				{
					int num2;
					if (!this.idReader.IsConnected)
					{
						num2 = this.idReader.Open();
					}
					num2 = this.idReader.GetSAMId(out string _);
					if (num2 <= 0)
					{
						num++;
						this.PreIDCardNo = null;
						this.idReader.Close();
						if (!base.IsDisposed && base.Visible)
						{
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.lblIDReaderFailed.Visible = true;
								});
							}
							else
							{
								this.lblIDReaderFailed.Visible = true;
							}
						}
					}
					else
					{
						num = 0;
						if (!base.IsDisposed && base.Visible)
						{
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.lblIDReaderFailed.Visible = false;
								});
							}
							else
							{
								this.lblIDReaderFailed.Visible = false;
							}
						}
					}
				}
				Thread.Sleep(200);
			}
			while (!base.IsDisposed && base.Visible && !this.bwReader.CancellationPending);
			this.idReader.Close();
		}

		private void ShowIDInfo(IDCardInfo info, Image img)
		{
			if (!base.IsDisposed && base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.ShowIDInfo(info, img);
				});
			}
			else
			{
				this.pic_photo.Image = img;
				this.PhotoChanged = true;
				Size size = this.pic_photo.Image.Size;
				int width = size.Width;
				size = this.pic_photo.Size;
				int num;
				if (width <= size.Width)
				{
					size = this.pic_photo.Image.Size;
					int height = size.Height;
					size = this.pic_photo.Size;
					num = ((height > size.Height) ? 1 : 0);
				}
				else
				{
					num = 1;
				}
				if (num != 0)
				{
					this.pic_photo.SizeMode = PictureBoxSizeMode.Zoom;
				}
				else
				{
					this.pic_photo.SizeMode = PictureBoxSizeMode.CenterImage;
				}
				this.txt_name.Text = info.Name;
				this.txt_workAddress.Text = info.Address;
				this.txt_IDnumber.Text = info.IDNumber;
				this.txt_ethnic.Text = info.MinZu;
				this.cbo_gender.SelectedValue = ((info.Gender == "1") ? "M" : "F");
				if (DateTime.TryParse(info.BirthDate, out DateTime dateTime))
				{
					this.DTPick_birthday.EditValue = dateTime;
				}
			}
		}

		private void lnkIDReader_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.idReader == null)
			{
				this.idReader = new IDReader();
			}
			if (this.bwReader == null)
			{
				this.bwReader = new BackgroundWorker();
				this.bwReader.WorkerSupportsCancellation = true;
				this.bwReader.DoWork += this.bwReader_DoWork;
				this.bwReader.RunWorkerCompleted += this.bwReader_RunWorkerCompleted;
			}
			if (this.lnkIDReader.Text == "身份证阅读器")
			{
				this.bwReader.RunWorkerAsync();
				this.lnkIDReader.Text = "停止";
			}
			else
			{
				this.bwReader.CancelAsync();
			}
		}

		private void btnFPCollector_Click(object sender, EventArgs e)
		{
			this.linkLabel1_LinkClicked(null, null);
		}

		private void btnMachine_Click(object sender, EventArgs e)
		{
			this.lnkRegFpViaDev_LinkClicked(null, null);
		}

		private void btnIDReader_Click(object sender, EventArgs e)
		{
			if (this.idReader == null)
			{
				this.idReader = new IDReader();
			}
			if (this.bwReader == null)
			{
				this.bwReader = new BackgroundWorker();
				this.bwReader.WorkerSupportsCancellation = true;
				this.bwReader.DoWork += this.bwReader_DoWork;
				this.bwReader.RunWorkerCompleted += this.bwReader_RunWorkerCompleted;
			}
			if (this.btnIDReader.Text == "身份证阅读器")
			{
				this.bwReader.RunWorkerAsync();
				this.btnIDReader.Text = "停止";
			}
			else
			{
				this.bwReader.CancelAsync();
			}
		}

		private void tabControl1_Click(object sender, EventArgs e)
		{
		}

		private void dgrd_selectedLevel_Click(object sender, EventArgs e)
		{
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			FrmPictureCapture frmPictureCapture = new FrmPictureCapture();
			frmPictureCapture.OnCapture += this.f_OnCapture;
			frmPictureCapture.ShowDialog();
		}

		private void f_OnCapture(Image img)
		{
			this.pic_photo.Image = ZKImage.ResizeImage(140, 120, img);
			this.PhotoChanged = true;
		}

		private void buttonX2_Click(object sender, EventArgs e)
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PersonnelManagementForm));
			this.pic_photo.Image = (Image)componentResourceManager.GetObject("pic_photo.Image");
			this.PhotoChanged = true;
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			this.msgDateValidationMayBeDisabled(true);
		}

		private void msgDateValidationMayBeDisabled(bool show)
		{
			if (show)
			{
				string info = ShowMsgInfos.GetInfo("SDateValidationMayNotBeAvailable", "此功能仅适用于Pull SDK协议设备。 检查用户手册中添加的设备协议");
				MessageBox.Show(null, info, "Informação", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private void chk_setValidTime_Click(object sender, EventArgs e)
		{
			this.msgDateValidationMayBeDisabled(this.showValidationMessage);
			this.showValidationMessage = false;
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
			GridLevelNode gridLevelNode = new GridLevelNode();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Visitor2ManagementForm));
			this.tabControl1 = new DevComponents.DotNetBar.TabControl();
			this.tabControlPanel3 = new TabControlPanel();
			this.pictureBox1 = new PictureBox();
			this.panel1 = new Panel();
			this.lblElevatorSpecialNeeds = new Label();
			this.chkElevatorSpecialNeeds = new CheckBox();
			this.lblElevatorFloor = new Label();
			this.txtDefaultFloor = new TextBox();
			this.cbo_datStart = new DateEdit();
			this.cbo_datEnd = new DateEdit();
			this.chk_setValidTime = new CheckBox();
			this.cbo_MultiCard = new System.Windows.Forms.ComboBox();
			this.lnk_ClearAntyBack = new LinkLabel();
			this.lab_multiCard = new Label();
			this.lab_End = new Label();
			this.lab_start = new Label();
			this.lab_setValidTime = new Label();
			this.dgrd_selectedLevel = new GridControl();
			this.gridView2 = new GridView();
			this.column_check2 = new GridColumn();
			this.column_selectedLevel = new GridColumn();
			this.column_selectedTime = new GridColumn();
			this.dgrd_optionalLevel = new GridControl();
			this.gridView1 = new GridView();
			this.column_check = new GridColumn();
			this.column_level = new GridColumn();
			this.column_time = new GridColumn();
			this.btn_allLMove = new ButtonX();
			this.btn_LMove = new ButtonX();
			this.btn_RMove = new ButtonX();
			this.btn_allRMove = new ButtonX();
			this.label4 = new Label();
			this.lab_levels = new Label();
			this.tabItem3 = new TabItem(this.components);
			this.tabControlPanel1 = new TabControlPanel();
			this.buttonX2 = new ButtonX();
			this.buttonX1 = new ButtonX();
			this.lblActiveVisit = new Label();
			this.DTPick_birthday = new DateEdit();
			this.flowLayoutPanel2 = new FlowLayoutPanel();
			this.btnFPCollector = new ButtonX();
			this.btnMachine = new ButtonX();
			this.btnIDReader = new ButtonX();
			this.linklbl_FPRegister = new LinkLabel();
			this.lnkRegFpViaDev = new LinkLabel();
			this.lnkIDReader = new LinkLabel();
			this.lblIDReaderFailed = new Label();
			this.txt_mobilPhone = new TextBox();
			this.txt_cardNO = new TextBox();
			this.txt_email = new TextBox();
			this.txt_password = new TextBox();
			this.cbo_gender = new System.Windows.Forms.ComboBox();
			this.txt_lastName = new TextBox();
			this.txt_name = new TextBox();
			this.txt_personnel = new TextBox();
			this.flowLayoutPanel1 = new FlowLayoutPanel();
			this.link_read = new PictureBox();
			this.pic_Readding = new PictureBox();
			this.lbl_picture = new Label();
			this.lab_email = new Label();
			this.lab_birthday = new Label();
			this.lab_mobilePhone = new Label();
			this.lab_password = new Label();
			this.lab_lastName = new Label();
			this.label3 = new Label();
			this.lab_FPRegister = new Label();
			this.btn_browse = new ButtonX();
			this.lab_gender = new Label();
			this.lab_name = new Label();
			this.lab_cardNO = new Label();
			this.lab_personnel = new Label();
			this.pic_photo = new PictureBox();
			this.tabItem_basic = new TabItem(this.components);
			this.tabControlPanel2 = new TabControlPanel();
			this.txt_origin = new TextBox();
			this.txt_ethnic = new TextBox();
			this.txt_IDnumber = new TextBox();
			this.txt_homeTelephone = new TextBox();
			this.txt_city = new TextBox();
			this.txt_nationality = new TextBox();
			this.txt_homeAddress = new TextBox();
			this.txt_workAddress = new TextBox();
			this.txt_jobTitle = new TextBox();
			this.txt_postalCode = new TextBox();
			this.txt_officeTelephone = new TextBox();
			this.txt_politicalStatus = new TextBox();
			this.txt_education = new TextBox();
			this.lab_postalCode = new Label();
			this.lab_homeAddress = new Label();
			this.lab_origin = new Label();
			this.lab_jobTitle = new Label();
			this.lab_ethnic = new Label();
			this.lab_city = new Label();
			this.lab_nationality = new Label();
			this.lab_workAddress = new Label();
			this.lab_IDnumber = new Label();
			this.lab_homeTelephone = new Label();
			this.lab_officeTelephone = new Label();
			this.lab_politicalStatus = new Label();
			this.lab_Education = new Label();
			this.tabItem_detail = new TabItem(this.components);
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			this.btn_saveContinue = new ButtonX();
			this.openFileDialog1 = new OpenFileDialog();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.backgroundWorker1 = new BackgroundWorker();
			((ISupportInitialize)this.tabControl1).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabControlPanel3.SuspendLayout();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.cbo_datStart.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.cbo_datStart.Properties).BeginInit();
			((ISupportInitialize)this.cbo_datEnd.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.cbo_datEnd.Properties).BeginInit();
			((ISupportInitialize)this.dgrd_selectedLevel).BeginInit();
			((ISupportInitialize)this.gridView2).BeginInit();
			((ISupportInitialize)this.dgrd_optionalLevel).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			this.tabControlPanel1.SuspendLayout();
			((ISupportInitialize)this.DTPick_birthday.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.DTPick_birthday.Properties).BeginInit();
			this.flowLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			((ISupportInitialize)this.link_read).BeginInit();
			((ISupportInitialize)this.pic_Readding).BeginInit();
			((ISupportInitialize)this.pic_photo).BeginInit();
			this.tabControlPanel2.SuspendLayout();
			base.SuspendLayout();
			this.tabControl1.BackColor = Color.FromArgb(194, 217, 247);
			this.tabControl1.CanReorderTabs = true;
			this.tabControl1.Controls.Add(this.tabControlPanel3);
			this.tabControl1.Controls.Add(this.tabControlPanel1);
			this.tabControl1.Controls.Add(this.tabControlPanel2);
			this.tabControl1.Location = new Point(-1, 2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedTabFont = new Font("SimSun", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tabControl1.SelectedTabIndex = 0;
			this.tabControl1.Size = new Size(852, 427);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
			this.tabControl1.Tabs.Add(this.tabItem_basic);
			this.tabControl1.Tabs.Add(this.tabItem_detail);
			this.tabControl1.Tabs.Add(this.tabItem3);
			this.tabControl1.Text = "tabControl1";
			this.tabControl1.Click += this.tabControl1_Click;
			this.tabControlPanel3.Controls.Add(this.pictureBox1);
			this.tabControlPanel3.Controls.Add(this.panel1);
			this.tabControlPanel3.Controls.Add(this.cbo_datStart);
			this.tabControlPanel3.Controls.Add(this.cbo_datEnd);
			this.tabControlPanel3.Controls.Add(this.chk_setValidTime);
			this.tabControlPanel3.Controls.Add(this.cbo_MultiCard);
			this.tabControlPanel3.Controls.Add(this.lnk_ClearAntyBack);
			this.tabControlPanel3.Controls.Add(this.lab_multiCard);
			this.tabControlPanel3.Controls.Add(this.lab_End);
			this.tabControlPanel3.Controls.Add(this.lab_start);
			this.tabControlPanel3.Controls.Add(this.lab_setValidTime);
			this.tabControlPanel3.Controls.Add(this.dgrd_selectedLevel);
			this.tabControlPanel3.Controls.Add(this.dgrd_optionalLevel);
			this.tabControlPanel3.Controls.Add(this.btn_allLMove);
			this.tabControlPanel3.Controls.Add(this.btn_LMove);
			this.tabControlPanel3.Controls.Add(this.btn_RMove);
			this.tabControlPanel3.Controls.Add(this.btn_allRMove);
			this.tabControlPanel3.Controls.Add(this.label4);
			this.tabControlPanel3.Controls.Add(this.lab_levels);
			this.tabControlPanel3.Dock = DockStyle.Fill;
			this.tabControlPanel3.Location = new Point(0, 26);
			this.tabControlPanel3.Name = "tabControlPanel3";
			this.tabControlPanel3.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel3.Size = new Size(852, 401);
			this.tabControlPanel3.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel3.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel3.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel3.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel3.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel3.Style.GradientAngle = 90;
			this.tabControlPanel3.TabIndex = 3;
			this.tabControlPanel3.TabItem = this.tabItem3;
			this.pictureBox1.BackColor = Color.Transparent;
			this.pictureBox1.Cursor = Cursors.Hand;
			this.pictureBox1.Image = Resources.help_opera;
			this.pictureBox1.Location = new Point(235, 314);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(19, 22);
			this.pictureBox1.TabIndex = 94;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Click += this.pictureBox1_Click;
			this.panel1.BackColor = Color.Transparent;
			this.panel1.Controls.Add(this.lblElevatorSpecialNeeds);
			this.panel1.Controls.Add(this.chkElevatorSpecialNeeds);
			this.panel1.Controls.Add(this.lblElevatorFloor);
			this.panel1.Controls.Add(this.txtDefaultFloor);
			this.panel1.Location = new Point(44, 364);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(745, 29);
			this.panel1.TabIndex = 93;
			this.lblElevatorSpecialNeeds.AutoSize = true;
			this.lblElevatorSpecialNeeds.Location = new Point(410, 9);
			this.lblElevatorSpecialNeeds.Name = "lblElevatorSpecialNeeds";
			this.lblElevatorSpecialNeeds.Size = new Size(141, 13);
			this.lblElevatorSpecialNeeds.TabIndex = 95;
			this.lblElevatorSpecialNeeds.Text = "Portador de Nesc. Especiais";
			this.chkElevatorSpecialNeeds.AutoSize = true;
			this.chkElevatorSpecialNeeds.Location = new Point(607, 9);
			this.chkElevatorSpecialNeeds.Name = "chkElevatorSpecialNeeds";
			this.chkElevatorSpecialNeeds.Size = new Size(15, 14);
			this.chkElevatorSpecialNeeds.TabIndex = 94;
			this.chkElevatorSpecialNeeds.UseVisualStyleBackColor = true;
			this.lblElevatorFloor.AutoSize = true;
			this.lblElevatorFloor.BackColor = Color.Transparent;
			this.lblElevatorFloor.Location = new Point(6, 9);
			this.lblElevatorFloor.Name = "lblElevatorFloor";
			this.lblElevatorFloor.Size = new Size(89, 13);
			this.lblElevatorFloor.TabIndex = 91;
			this.lblElevatorFloor.Text = "Andar de Destino";
			this.txtDefaultFloor.Location = new Point(163, 6);
			this.txtDefaultFloor.Name = "txtDefaultFloor";
			this.txtDefaultFloor.Size = new Size(122, 20);
			this.txtDefaultFloor.TabIndex = 92;
			this.cbo_datStart.EditValue = null;
			this.cbo_datStart.Enabled = false;
			this.cbo_datStart.Location = new Point(214, 342);
			this.cbo_datStart.Name = "cbo_datStart";
			this.cbo_datStart.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.cbo_datStart.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.cbo_datStart.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.cbo_datStart.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.cbo_datStart.Size = new Size(122, 20);
			this.cbo_datStart.TabIndex = 87;
			this.cbo_datStart.EditValueChanged += this.cbo_datStart_EditValueChanged;
			this.cbo_datEnd.EditValue = null;
			this.cbo_datEnd.Enabled = false;
			this.cbo_datEnd.Location = new Point(651, 340);
			this.cbo_datEnd.Name = "cbo_datEnd";
			this.cbo_datEnd.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.cbo_datEnd.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.cbo_datEnd.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.cbo_datEnd.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.cbo_datEnd.Size = new Size(122, 20);
			this.cbo_datEnd.TabIndex = 86;
			this.cbo_datEnd.EditValueChanged += this.cbo_datEnd_EditValueChanged;
			this.chk_setValidTime.AutoSize = true;
			this.chk_setValidTime.Location = new Point(214, 315);
			this.chk_setValidTime.Name = "chk_setValidTime";
			this.chk_setValidTime.Size = new Size(15, 14);
			this.chk_setValidTime.TabIndex = 5;
			this.chk_setValidTime.UseVisualStyleBackColor = true;
			this.chk_setValidTime.Visible = false;
			this.chk_setValidTime.CheckedChanged += this.time_checkBox_CheckedChanged;
			this.chk_setValidTime.Click += this.chk_setValidTime_Click;
			this.cbo_MultiCard.FormattingEnabled = true;
			this.cbo_MultiCard.Location = new Point(214, 283);
			this.cbo_MultiCard.Name = "cbo_MultiCard";
			this.cbo_MultiCard.Size = new Size(122, 21);
			this.cbo_MultiCard.TabIndex = 4;
			this.lnk_ClearAntyBack.AutoSize = true;
			this.lnk_ClearAntyBack.BackColor = Color.Transparent;
			this.lnk_ClearAntyBack.Location = new Point(457, 290);
			this.lnk_ClearAntyBack.Name = "lnk_ClearAntyBack";
			this.lnk_ClearAntyBack.Size = new Size(106, 13);
			this.lnk_ClearAntyBack.TabIndex = 28;
			this.lnk_ClearAntyBack.TabStop = true;
			this.lnk_ClearAntyBack.Text = "Limpar AntiPassback";
			this.lnk_ClearAntyBack.Visible = false;
			this.lnk_ClearAntyBack.LinkClicked += this.lnk_ClearAntyBack_LinkClicked;
			this.lab_multiCard.AutoEllipsis = true;
			this.lab_multiCard.BackColor = Color.Transparent;
			this.lab_multiCard.Location = new Point(44, 287);
			this.lab_multiCard.Name = "lab_multiCard";
			this.lab_multiCard.Size = new Size(162, 13);
			this.lab_multiCard.TabIndex = 24;
			this.lab_multiCard.Text = "Grupo Acesso Combinado";
			this.lab_multiCard.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_End.BackColor = Color.Transparent;
			this.lab_End.Enabled = false;
			this.lab_End.Location = new Point(455, 343);
			this.lab_End.Name = "lab_End";
			this.lab_End.Size = new Size(113, 13);
			this.lab_End.TabIndex = 27;
			this.lab_End.Text = "Data Fim";
			this.lab_End.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_start.BackColor = Color.Transparent;
			this.lab_start.Enabled = false;
			this.lab_start.Location = new Point(44, 343);
			this.lab_start.Name = "lab_start";
			this.lab_start.Size = new Size(162, 13);
			this.lab_start.TabIndex = 26;
			this.lab_start.Text = "Data Início";
			this.lab_start.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_setValidTime.BackColor = Color.Transparent;
			this.lab_setValidTime.Location = new Point(44, 316);
			this.lab_setValidTime.Name = "lab_setValidTime";
			this.lab_setValidTime.Size = new Size(162, 13);
			this.lab_setValidTime.TabIndex = 25;
			this.lab_setValidTime.Text = "Usa Validade";
			this.lab_setValidTime.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_setValidTime.Visible = false;
			this.dgrd_selectedLevel.Cursor = Cursors.Default;
			this.dgrd_selectedLevel.Location = new Point(457, 35);
			this.dgrd_selectedLevel.MainView = this.gridView2;
			this.dgrd_selectedLevel.Name = "dgrd_selectedLevel";
			this.dgrd_selectedLevel.Size = new Size(350, 238);
			this.dgrd_selectedLevel.TabIndex = 20;
			this.dgrd_selectedLevel.TabStop = false;
			this.dgrd_selectedLevel.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridView2
			});
			this.dgrd_selectedLevel.Click += this.dgrd_selectedLevel_Click;
			this.dgrd_selectedLevel.DoubleClick += this.dgrd_selectedLevel_DoubleClick;
			this.gridView2.Columns.AddRange(new GridColumn[3]
			{
				this.column_check2,
				this.column_selectedLevel,
				this.column_selectedTime
			});
			this.gridView2.GridControl = this.dgrd_selectedLevel;
			this.gridView2.Name = "gridView2";
			this.gridView2.OptionsView.ShowGroupPanel = false;
			this.gridView2.CustomDrawColumnHeader += this.gridView_CustomDrawColumnHeader;
			this.gridView2.CustomDrawCell += this.gridView_CustomDrawCell;
			this.gridView2.Click += this.gridView_Click;
			this.column_check2.Name = "column_check2";
			this.column_check2.Visible = true;
			this.column_check2.VisibleIndex = 0;
			this.column_selectedLevel.Caption = "Nome";
			this.column_selectedLevel.Name = "column_selectedLevel";
			this.column_selectedLevel.Visible = true;
			this.column_selectedLevel.VisibleIndex = 1;
			this.column_selectedTime.Caption = "Faixa";
			this.column_selectedTime.Name = "column_selectedTime";
			this.column_selectedTime.Visible = true;
			this.column_selectedTime.VisibleIndex = 2;
			this.dgrd_optionalLevel.Cursor = Cursors.Default;
			gridLevelNode.RelationName = "Level1";
			this.dgrd_optionalLevel.LevelTree.Nodes.AddRange(new GridLevelNode[1]
			{
				gridLevelNode
			});
			this.dgrd_optionalLevel.Location = new Point(44, 35);
			this.dgrd_optionalLevel.MainView = this.gridView1;
			this.dgrd_optionalLevel.Name = "dgrd_optionalLevel";
			this.dgrd_optionalLevel.Size = new Size(350, 238);
			this.dgrd_optionalLevel.TabIndex = 19;
			this.dgrd_optionalLevel.TabStop = false;
			this.dgrd_optionalLevel.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridView1
			});
			this.dgrd_optionalLevel.DoubleClick += this.dgrd_optionalLevel_DoubleClick;
			this.gridView1.Columns.AddRange(new GridColumn[3]
			{
				this.column_check,
				this.column_level,
				this.column_time
			});
			this.gridView1.GridControl = this.dgrd_optionalLevel;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.CustomDrawColumnHeader += this.gridView_CustomDrawColumnHeader;
			this.gridView1.CustomDrawCell += this.gridView_CustomDrawCell;
			this.gridView1.Click += this.gridView_Click;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 57;
			this.column_level.Caption = "Nome";
			this.column_level.Name = "column_level";
			this.column_level.Visible = true;
			this.column_level.VisibleIndex = 1;
			this.column_time.Caption = "Faixa";
			this.column_time.Name = "column_time";
			this.column_time.Visible = true;
			this.column_time.VisibleIndex = 2;
			this.btn_allLMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allLMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allLMove.Location = new Point(401, 215);
			this.btn_allLMove.Name = "btn_allLMove";
			this.btn_allLMove.Size = new Size(47, 25);
			this.btn_allLMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allLMove.TabIndex = 3;
			this.btn_allLMove.Text = "<<";
			this.btn_allLMove.Click += this.btn_allLMove_Click;
			this.btn_LMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_LMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_LMove.Location = new Point(401, 168);
			this.btn_LMove.Name = "btn_LMove";
			this.btn_LMove.Size = new Size(47, 25);
			this.btn_LMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_LMove.TabIndex = 2;
			this.btn_LMove.Text = "<";
			this.btn_LMove.Click += this.btn_LMove_Click;
			this.btn_RMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_RMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_RMove.Location = new Point(401, 121);
			this.btn_RMove.Name = "btn_RMove";
			this.btn_RMove.Size = new Size(47, 25);
			this.btn_RMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_RMove.TabIndex = 1;
			this.btn_RMove.Text = ">";
			this.btn_RMove.Click += this.btn_RMove_Click;
			this.btn_allRMove.AccessibleRole = AccessibleRole.PushButton;
			this.btn_allRMove.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_allRMove.Location = new Point(401, 75);
			this.btn_allRMove.Name = "btn_allRMove";
			this.btn_allRMove.Size = new Size(47, 25);
			this.btn_allRMove.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_allRMove.TabIndex = 0;
			this.btn_allRMove.Text = ">>";
			this.btn_allRMove.Click += this.btn_allRMove_Click;
			this.label4.BackColor = Color.Transparent;
			this.label4.Location = new Point(455, 15);
			this.label4.Name = "label4";
			this.label4.Size = new Size(318, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "Níveis Selecionados";
			this.lab_levels.BackColor = Color.Transparent;
			this.lab_levels.Location = new Point(44, 15);
			this.lab_levels.Name = "lab_levels";
			this.lab_levels.Size = new Size(318, 13);
			this.lab_levels.TabIndex = 16;
			this.lab_levels.Text = "Níveis de Acesso";
			this.lab_levels.TextAlign = ContentAlignment.MiddleLeft;
			this.tabItem3.AttachedControl = this.tabControlPanel3;
			this.tabItem3.Name = "tabItem3";
			this.tabItem3.Text = "Definição de Acesso";
			this.tabControlPanel1.Controls.Add(this.buttonX2);
			this.tabControlPanel1.Controls.Add(this.buttonX1);
			this.tabControlPanel1.Controls.Add(this.lblActiveVisit);
			this.tabControlPanel1.Controls.Add(this.DTPick_birthday);
			this.tabControlPanel1.Controls.Add(this.flowLayoutPanel2);
			this.tabControlPanel1.Controls.Add(this.txt_mobilPhone);
			this.tabControlPanel1.Controls.Add(this.txt_cardNO);
			this.tabControlPanel1.Controls.Add(this.txt_email);
			this.tabControlPanel1.Controls.Add(this.txt_password);
			this.tabControlPanel1.Controls.Add(this.cbo_gender);
			this.tabControlPanel1.Controls.Add(this.txt_lastName);
			this.tabControlPanel1.Controls.Add(this.txt_name);
			this.tabControlPanel1.Controls.Add(this.txt_personnel);
			this.tabControlPanel1.Controls.Add(this.flowLayoutPanel1);
			this.tabControlPanel1.Controls.Add(this.lbl_picture);
			this.tabControlPanel1.Controls.Add(this.lab_email);
			this.tabControlPanel1.Controls.Add(this.lab_birthday);
			this.tabControlPanel1.Controls.Add(this.lab_mobilePhone);
			this.tabControlPanel1.Controls.Add(this.lab_password);
			this.tabControlPanel1.Controls.Add(this.lab_lastName);
			this.tabControlPanel1.Controls.Add(this.label3);
			this.tabControlPanel1.Controls.Add(this.lab_FPRegister);
			this.tabControlPanel1.Controls.Add(this.btn_browse);
			this.tabControlPanel1.Controls.Add(this.lab_gender);
			this.tabControlPanel1.Controls.Add(this.lab_name);
			this.tabControlPanel1.Controls.Add(this.lab_cardNO);
			this.tabControlPanel1.Controls.Add(this.lab_personnel);
			this.tabControlPanel1.Controls.Add(this.pic_photo);
			this.tabControlPanel1.Dock = DockStyle.Fill;
			this.tabControlPanel1.Location = new Point(0, 26);
			this.tabControlPanel1.Name = "tabControlPanel1";
			this.tabControlPanel1.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel1.Size = new Size(852, 376);
			this.tabControlPanel1.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel1.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel1.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel1.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel1.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel1.Style.GradientAngle = 90;
			this.tabControlPanel1.TabIndex = 1;
			this.tabControlPanel1.TabItem = this.tabItem_basic;
			this.buttonX2.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX2.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX2.Location = new Point(708, 263);
			this.buttonX2.Name = "buttonX2";
			this.buttonX2.Size = new Size(75, 25);
			this.buttonX2.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX2.TabIndex = 85;
			this.buttonX2.Text = "Apagar";
			this.buttonX2.Click += this.buttonX2_Click;
			this.buttonX1.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX1.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX1.Location = new Point(708, 233);
			this.buttonX1.Name = "buttonX1";
			this.buttonX1.Size = new Size(75, 25);
			this.buttonX1.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX1.TabIndex = 84;
			this.buttonX1.Text = "Camera";
			this.buttonX1.Click += this.buttonX1_Click;
			this.lblActiveVisit.BackColor = Color.Transparent;
			this.lblActiveVisit.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lblActiveVisit.ForeColor = Color.Red;
			this.lblActiveVisit.Location = new Point(129, 332);
			this.lblActiveVisit.Name = "lblActiveVisit";
			this.lblActiveVisit.RightToLeft = RightToLeft.No;
			this.lblActiveVisit.Size = new Size(474, 34);
			this.lblActiveVisit.TabIndex = 83;
			this.lblActiveVisit.Text = "VISITA ATIVA";
			this.lblActiveVisit.TextAlign = ContentAlignment.MiddleLeft;
			this.DTPick_birthday.EditValue = null;
			this.DTPick_birthday.Location = new Point(457, 113);
			this.DTPick_birthday.Name = "DTPick_birthday";
			this.DTPick_birthday.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.DTPick_birthday.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.DTPick_birthday.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.DTPick_birthday.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.DTPick_birthday.Size = new Size(121, 20);
			this.DTPick_birthday.TabIndex = 82;
			this.DTPick_birthday.EditValueChanged += this.DTPick_birthday_EditValueChanged;
			this.flowLayoutPanel2.BackColor = Color.Transparent;
			this.flowLayoutPanel2.Controls.Add(this.btnFPCollector);
			this.flowLayoutPanel2.Controls.Add(this.btnMachine);
			this.flowLayoutPanel2.Controls.Add(this.btnIDReader);
			this.flowLayoutPanel2.Controls.Add(this.linklbl_FPRegister);
			this.flowLayoutPanel2.Controls.Add(this.lnkRegFpViaDev);
			this.flowLayoutPanel2.Controls.Add(this.lnkIDReader);
			this.flowLayoutPanel2.Controls.Add(this.lblIDReaderFailed);
			this.flowLayoutPanel2.Location = new Point(132, 286);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new Size(704, 35);
			this.flowLayoutPanel2.TabIndex = 81;
			this.btnFPCollector.AccessibleRole = AccessibleRole.PushButton;
			this.btnFPCollector.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnFPCollector.Location = new Point(3, 3);
			this.btnFPCollector.Name = "btnFPCollector";
			this.btnFPCollector.Size = new Size(103, 25);
			this.btnFPCollector.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnFPCollector.TabIndex = 83;
			this.btnFPCollector.Text = "Sensor de Mesa";
			this.btnFPCollector.Click += this.btnFPCollector_Click;
			this.btnMachine.AccessibleRole = AccessibleRole.PushButton;
			this.btnMachine.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnMachine.Location = new Point(112, 3);
			this.btnMachine.Name = "btnMachine";
			this.btnMachine.Size = new Size(103, 25);
			this.btnMachine.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnMachine.TabIndex = 84;
			this.btnMachine.Text = "Equipamento";
			this.btnMachine.Visible = false;
			this.btnMachine.Click += this.btnMachine_Click;
			this.btnIDReader.AccessibleRole = AccessibleRole.PushButton;
			this.btnIDReader.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnIDReader.Location = new Point(221, 3);
			this.btnIDReader.Name = "btnIDReader";
			this.btnIDReader.Size = new Size(103, 25);
			this.btnIDReader.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnIDReader.TabIndex = 85;
			this.btnIDReader.Text = "Leitor de Cartão";
			this.btnIDReader.Visible = false;
			this.btnIDReader.Click += this.btnIDReader_Click;
			this.linklbl_FPRegister.AutoSize = true;
			this.linklbl_FPRegister.BackColor = Color.Transparent;
			this.linklbl_FPRegister.ForeColor = SystemColors.ActiveCaption;
			this.linklbl_FPRegister.LinkColor = Color.Blue;
			this.linklbl_FPRegister.Location = new Point(327, 0);
			this.linklbl_FPRegister.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.linklbl_FPRegister.Name = "linklbl_FPRegister";
			this.linklbl_FPRegister.Size = new Size(43, 13);
			this.linklbl_FPRegister.TabIndex = 11;
			this.linklbl_FPRegister.TabStop = true;
			this.linklbl_FPRegister.Text = "指纹仪";
			this.linklbl_FPRegister.Visible = false;
			this.linklbl_FPRegister.LinkClicked += this.linkLabel1_LinkClicked;
			this.lnkRegFpViaDev.AutoSize = true;
			this.lnkRegFpViaDev.BackColor = Color.Transparent;
			this.lnkRegFpViaDev.ForeColor = SystemColors.ActiveCaption;
			this.lnkRegFpViaDev.LinkColor = Color.Blue;
			this.lnkRegFpViaDev.Location = new Point(376, 0);
			this.lnkRegFpViaDev.Name = "lnkRegFpViaDev";
			this.lnkRegFpViaDev.Size = new Size(31, 13);
			this.lnkRegFpViaDev.TabIndex = 80;
			this.lnkRegFpViaDev.TabStop = true;
			this.lnkRegFpViaDev.Text = "机器";
			this.lnkRegFpViaDev.Visible = false;
			this.lnkRegFpViaDev.LinkClicked += this.lnkRegFpViaDev_LinkClicked;
			this.lnkIDReader.AutoSize = true;
			this.lnkIDReader.Location = new Point(413, 0);
			this.lnkIDReader.Name = "lnkIDReader";
			this.lnkIDReader.Size = new Size(79, 13);
			this.lnkIDReader.TabIndex = 81;
			this.lnkIDReader.TabStop = true;
			this.lnkIDReader.Text = "身份证阅读器";
			this.lnkIDReader.Visible = false;
			this.lnkIDReader.LinkClicked += this.lnkIDReader_LinkClicked;
			this.lblIDReaderFailed.AutoSize = true;
			this.lblIDReaderFailed.ForeColor = Color.Red;
			this.lblIDReaderFailed.Location = new Point(498, 9);
			this.lblIDReaderFailed.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
			this.lblIDReaderFailed.Name = "lblIDReaderFailed";
			this.lblIDReaderFailed.Size = new Size(73, 13);
			this.lblIDReaderFailed.TabIndex = 82;
			this.lblIDReaderFailed.Text = "Falha no leitor";
			this.lblIDReaderFailed.Visible = false;
			this.txt_mobilPhone.Location = new Point(457, 62);
			this.txt_mobilPhone.Name = "txt_mobilPhone";
			this.txt_mobilPhone.Size = new Size(121, 20);
			this.txt_mobilPhone.TabIndex = 6;
			this.txt_mobilPhone.KeyPress += this.txt_mobilPhone_KeyPress;
			this.txt_cardNO.Location = new Point(457, 18);
			this.txt_cardNO.Name = "txt_cardNO";
			this.txt_cardNO.Size = new Size(121, 20);
			this.txt_cardNO.TabIndex = 4;
			this.txt_cardNO.KeyPress += this.txt_cardNO_KeyPress;
			this.txt_cardNO.MouseLeave += this.txt_cardNO_MouseLeave;
			this.txt_email.Location = new Point(136, 251);
			this.txt_email.Name = "txt_email";
			this.txt_email.Size = new Size(258, 20);
			this.txt_email.TabIndex = 13;
			this.txt_email.KeyPress += this.txt_email_KeyPress;
			this.txt_password.Location = new Point(136, 204);
			this.txt_password.Name = "txt_password";
			this.txt_password.PasswordChar = '*';
			this.txt_password.Size = new Size(121, 20);
			this.txt_password.TabIndex = 9;
			this.txt_password.KeyPress += this.txt_password_KeyPress;
			this.cbo_gender.FormattingEnabled = true;
			this.cbo_gender.Location = new Point(136, 157);
			this.cbo_gender.Name = "cbo_gender";
			this.cbo_gender.Size = new Size(121, 21);
			this.cbo_gender.TabIndex = 7;
			this.txt_lastName.Location = new Point(136, 112);
			this.txt_lastName.Name = "txt_lastName";
			this.txt_lastName.Size = new Size(121, 20);
			this.txt_lastName.TabIndex = 5;
			this.txt_lastName.KeyPress += this.txt_lastName_KeyPress;
			this.txt_name.Location = new Point(136, 65);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(121, 20);
			this.txt_name.TabIndex = 3;
			this.txt_name.TextChanged += this.txt_name_TextChanged;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.txt_personnel.Location = new Point(136, 18);
			this.txt_personnel.Name = "txt_personnel";
			this.txt_personnel.Size = new Size(121, 20);
			this.txt_personnel.TabIndex = 1;
			this.txt_personnel.KeyPress += this.txt_personnel_KeyPress;
			this.flowLayoutPanel1.BackColor = Color.Transparent;
			this.flowLayoutPanel1.Controls.Add(this.link_read);
			this.flowLayoutPanel1.Controls.Add(this.pic_Readding);
			this.flowLayoutPanel1.Location = new Point(584, 15);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new Size(81, 38);
			this.flowLayoutPanel1.TabIndex = 79;
			this.link_read.BackColor = Color.Transparent;
			this.link_read.Image = Resource.punchCard;
			this.link_read.Location = new Point(3, 3);
			this.link_read.Name = "link_read";
			this.link_read.Size = new Size(20, 22);
			this.link_read.SizeMode = PictureBoxSizeMode.StretchImage;
			this.link_read.TabIndex = 74;
			this.link_read.TabStop = false;
			this.link_read.Click += this.link_read_LinkClicked;
			this.link_read.MouseEnter += this.link_read_MouseEnter;
			this.pic_Readding.BackColor = Color.Transparent;
			this.pic_Readding.Image = Resource.loadpage;
			this.pic_Readding.Location = new Point(29, 3);
			this.pic_Readding.Name = "pic_Readding";
			this.pic_Readding.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this.pic_Readding.Size = new Size(20, 22);
			this.pic_Readding.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_Readding.TabIndex = 71;
			this.pic_Readding.TabStop = false;
			this.pic_Readding.Visible = false;
			this.lbl_picture.BackColor = Color.Transparent;
			this.lbl_picture.ForeColor = Color.Firebrick;
			this.lbl_picture.Location = new Point(639, 174);
			this.lbl_picture.Name = "lbl_picture";
			this.lbl_picture.Size = new Size(206, 18);
			this.lbl_picture.TabIndex = 73;
			this.lbl_picture.Text = "( recomendada 120×140 pixels)";
			this.lbl_picture.TextAlign = ContentAlignment.MiddleCenter;
			this.lab_email.BackColor = Color.Transparent;
			this.lab_email.Location = new Point(23, 256);
			this.lab_email.Name = "lab_email";
			this.lab_email.RightToLeft = RightToLeft.No;
			this.lab_email.Size = new Size(106, 13);
			this.lab_email.TabIndex = 20;
			this.lab_email.Text = "E-Mail";
			this.lab_email.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_birthday.BackColor = Color.Transparent;
			this.lab_birthday.Location = new Point(313, 119);
			this.lab_birthday.Name = "lab_birthday";
			this.lab_birthday.RightToLeft = RightToLeft.No;
			this.lab_birthday.Size = new Size(133, 13);
			this.lab_birthday.TabIndex = 27;
			this.lab_birthday.Text = "Data Nascimento";
			this.lab_birthday.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_mobilePhone.BackColor = Color.Transparent;
			this.lab_mobilePhone.Location = new Point(313, 66);
			this.lab_mobilePhone.Name = "lab_mobilePhone";
			this.lab_mobilePhone.RightToLeft = RightToLeft.No;
			this.lab_mobilePhone.Size = new Size(133, 13);
			this.lab_mobilePhone.TabIndex = 24;
			this.lab_mobilePhone.Text = "Celular";
			this.lab_mobilePhone.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_password.BackColor = Color.Transparent;
			this.lab_password.Location = new Point(23, 208);
			this.lab_password.Name = "lab_password";
			this.lab_password.RightToLeft = RightToLeft.No;
			this.lab_password.Size = new Size(106, 13);
			this.lab_password.TabIndex = 25;
			this.lab_password.Text = "Senha";
			this.lab_password.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_lastName.BackColor = Color.Transparent;
			this.lab_lastName.Location = new Point(23, 116);
			this.lab_lastName.Name = "lab_lastName";
			this.lab_lastName.RightToLeft = RightToLeft.No;
			this.lab_lastName.Size = new Size(106, 13);
			this.lab_lastName.TabIndex = 21;
			this.lab_lastName.Text = "Sobrenome";
			this.lab_lastName.TextAlign = ContentAlignment.MiddleLeft;
			this.label3.AutoSize = true;
			this.label3.BackColor = Color.Transparent;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(262, 23);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 13);
			this.label3.TabIndex = 27;
			this.label3.Text = "*";
			this.lab_FPRegister.BackColor = Color.Transparent;
			this.lab_FPRegister.Location = new Point(23, 296);
			this.lab_FPRegister.Name = "lab_FPRegister";
			this.lab_FPRegister.RightToLeft = RightToLeft.No;
			this.lab_FPRegister.Size = new Size(106, 13);
			this.lab_FPRegister.TabIndex = 28;
			this.lab_FPRegister.Text = "Impressões Digitais";
			this.lab_FPRegister.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_browse.AccessibleRole = AccessibleRole.PushButton;
			this.btn_browse.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_browse.Location = new Point(708, 204);
			this.btn_browse.Name = "btn_browse";
			this.btn_browse.Size = new Size(75, 25);
			this.btn_browse.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_browse.TabIndex = 14;
			this.btn_browse.Text = "Procura";
			this.btn_browse.Click += this.BrowseBtn_Click;
			this.lab_gender.BackColor = Color.Transparent;
			this.lab_gender.Location = new Point(23, 161);
			this.lab_gender.Name = "lab_gender";
			this.lab_gender.RightToLeft = RightToLeft.No;
			this.lab_gender.Size = new Size(106, 13);
			this.lab_gender.TabIndex = 23;
			this.lab_gender.Text = "Sexo";
			this.lab_gender.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_name.BackColor = Color.Transparent;
			this.lab_name.Location = new Point(23, 69);
			this.lab_name.Name = "lab_name";
			this.lab_name.RightToLeft = RightToLeft.No;
			this.lab_name.Size = new Size(106, 13);
			this.lab_name.TabIndex = 19;
			this.lab_name.Text = "Nome";
			this.lab_name.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_cardNO.BackColor = Color.Transparent;
			this.lab_cardNO.Location = new Point(313, 22);
			this.lab_cardNO.Name = "lab_cardNO";
			this.lab_cardNO.RightToLeft = RightToLeft.No;
			this.lab_cardNO.Size = new Size(133, 13);
			this.lab_cardNO.TabIndex = 2;
			this.lab_cardNO.Text = "No. Cartão";
			this.lab_cardNO.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_personnel.BackColor = Color.Transparent;
			this.lab_personnel.Location = new Point(23, 23);
			this.lab_personnel.Name = "lab_personnel";
			this.lab_personnel.RightToLeft = RightToLeft.No;
			this.lab_personnel.Size = new Size(106, 13);
			this.lab_personnel.TabIndex = 17;
			this.lab_personnel.Text = "No. Identificação";
			this.lab_personnel.TextAlign = ContentAlignment.MiddleLeft;
			this.pic_photo.BackColor = Color.Transparent;
			this.pic_photo.Image = (Image)componentResourceManager.GetObject("pic_photo.Image");
			this.pic_photo.Location = new Point(671, 18);
			this.pic_photo.Name = "pic_photo";
			this.pic_photo.Size = new Size(140, 152);
			this.pic_photo.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_photo.TabIndex = 13;
			this.pic_photo.TabStop = false;
			this.tabItem_basic.AttachedControl = this.tabControlPanel1;
			this.tabItem_basic.Name = "tabItem_basic";
			this.tabItem_basic.Text = "Perfil";
			this.tabControlPanel2.Controls.Add(this.txt_origin);
			this.tabControlPanel2.Controls.Add(this.txt_ethnic);
			this.tabControlPanel2.Controls.Add(this.txt_IDnumber);
			this.tabControlPanel2.Controls.Add(this.txt_homeTelephone);
			this.tabControlPanel2.Controls.Add(this.txt_city);
			this.tabControlPanel2.Controls.Add(this.txt_nationality);
			this.tabControlPanel2.Controls.Add(this.txt_homeAddress);
			this.tabControlPanel2.Controls.Add(this.txt_workAddress);
			this.tabControlPanel2.Controls.Add(this.txt_jobTitle);
			this.tabControlPanel2.Controls.Add(this.txt_postalCode);
			this.tabControlPanel2.Controls.Add(this.txt_officeTelephone);
			this.tabControlPanel2.Controls.Add(this.txt_politicalStatus);
			this.tabControlPanel2.Controls.Add(this.txt_education);
			this.tabControlPanel2.Controls.Add(this.lab_postalCode);
			this.tabControlPanel2.Controls.Add(this.lab_homeAddress);
			this.tabControlPanel2.Controls.Add(this.lab_origin);
			this.tabControlPanel2.Controls.Add(this.lab_jobTitle);
			this.tabControlPanel2.Controls.Add(this.lab_ethnic);
			this.tabControlPanel2.Controls.Add(this.lab_city);
			this.tabControlPanel2.Controls.Add(this.lab_nationality);
			this.tabControlPanel2.Controls.Add(this.lab_workAddress);
			this.tabControlPanel2.Controls.Add(this.lab_IDnumber);
			this.tabControlPanel2.Controls.Add(this.lab_homeTelephone);
			this.tabControlPanel2.Controls.Add(this.lab_officeTelephone);
			this.tabControlPanel2.Controls.Add(this.lab_politicalStatus);
			this.tabControlPanel2.Controls.Add(this.lab_Education);
			this.tabControlPanel2.Dock = DockStyle.Fill;
			this.tabControlPanel2.Location = new Point(0, 26);
			this.tabControlPanel2.Name = "tabControlPanel2";
			this.tabControlPanel2.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel2.Size = new Size(852, 401);
			this.tabControlPanel2.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel2.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel2.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel2.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel2.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel2.Style.GradientAngle = 90;
			this.tabControlPanel2.TabIndex = 2;
			this.tabControlPanel2.TabItem = this.tabItem_detail;
			this.txt_origin.Location = new Point(503, 200);
			this.txt_origin.Name = "txt_origin";
			this.txt_origin.Size = new Size(162, 20);
			this.txt_origin.TabIndex = 12;
			this.txt_origin.KeyPress += this.txt_origin_KeyPress;
			this.txt_ethnic.Location = new Point(503, 165);
			this.txt_ethnic.Name = "txt_ethnic";
			this.txt_ethnic.Size = new Size(162, 20);
			this.txt_ethnic.TabIndex = 10;
			this.txt_ethnic.KeyPress += this.txt_ethnic_KeyPress;
			this.txt_IDnumber.Location = new Point(503, 129);
			this.txt_IDnumber.Name = "txt_IDnumber";
			this.txt_IDnumber.Size = new Size(162, 20);
			this.txt_IDnumber.TabIndex = 8;
			this.txt_IDnumber.KeyPress += this.txt_IDnumber_KeyPress;
			this.txt_homeTelephone.Location = new Point(503, 93);
			this.txt_homeTelephone.Name = "txt_homeTelephone";
			this.txt_homeTelephone.Size = new Size(162, 20);
			this.txt_homeTelephone.TabIndex = 6;
			this.txt_homeTelephone.KeyPress += this.txt_homeTelephone_KeyPress;
			this.txt_city.Location = new Point(503, 57);
			this.txt_city.Name = "txt_city";
			this.txt_city.Size = new Size(162, 20);
			this.txt_city.TabIndex = 4;
			this.txt_city.KeyPress += this.txt_city_KeyPress;
			this.txt_nationality.Location = new Point(503, 22);
			this.txt_nationality.Name = "txt_nationality";
			this.txt_nationality.Size = new Size(162, 20);
			this.txt_nationality.TabIndex = 2;
			this.txt_nationality.KeyPress += this.txt_nationality_KeyPress;
			this.txt_homeAddress.Location = new Point(142, 304);
			this.txt_homeAddress.Name = "txt_homeAddress";
			this.txt_homeAddress.Size = new Size(523, 20);
			this.txt_homeAddress.TabIndex = 15;
			this.txt_homeAddress.Text = " ";
			this.txt_homeAddress.KeyPress += this.txt_homeAddress_KeyPress;
			this.txt_workAddress.Location = new Point(142, 270);
			this.txt_workAddress.Name = "txt_workAddress";
			this.txt_workAddress.Size = new Size(523, 20);
			this.txt_workAddress.TabIndex = 14;
			this.txt_workAddress.KeyPress += this.txt_workAddress_KeyPress;
			this.txt_jobTitle.Location = new Point(142, 200);
			this.txt_jobTitle.Multiline = true;
			this.txt_jobTitle.Name = "txt_jobTitle";
			this.txt_jobTitle.Size = new Size(162, 58);
			this.txt_jobTitle.TabIndex = 13;
			this.txt_jobTitle.KeyPress += this.txt_jobTitle_KeyPress;
			this.txt_postalCode.Location = new Point(142, 165);
			this.txt_postalCode.Name = "txt_postalCode";
			this.txt_postalCode.Size = new Size(162, 20);
			this.txt_postalCode.TabIndex = 11;
			this.txt_postalCode.KeyPress += this.txt_postalCode_KeyPress;
			this.txt_officeTelephone.Location = new Point(142, 129);
			this.txt_officeTelephone.Name = "txt_officeTelephone";
			this.txt_officeTelephone.Size = new Size(162, 20);
			this.txt_officeTelephone.TabIndex = 9;
			this.txt_officeTelephone.KeyPress += this.txt_officeTelephone_KeyPress;
			this.txt_politicalStatus.Location = new Point(142, 93);
			this.txt_politicalStatus.Name = "txt_politicalStatus";
			this.txt_politicalStatus.Size = new Size(162, 20);
			this.txt_politicalStatus.TabIndex = 7;
			this.txt_politicalStatus.KeyPress += this.txt_politicalStatus_KeyPress;
			this.txt_education.Location = new Point(142, 22);
			this.txt_education.Multiline = true;
			this.txt_education.Name = "txt_education";
			this.txt_education.Size = new Size(162, 58);
			this.txt_education.TabIndex = 1;
			this.txt_education.KeyPress += this.txt_education_KeyPress;
			this.lab_postalCode.BackColor = Color.Transparent;
			this.lab_postalCode.Location = new Point(44, 169);
			this.lab_postalCode.Name = "lab_postalCode";
			this.lab_postalCode.Size = new Size(92, 13);
			this.lab_postalCode.TabIndex = 41;
			this.lab_postalCode.Text = "CEP";
			this.lab_postalCode.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_homeAddress.BackColor = Color.Transparent;
			this.lab_homeAddress.Location = new Point(44, 275);
			this.lab_homeAddress.Name = "lab_homeAddress";
			this.lab_homeAddress.Size = new Size(92, 13);
			this.lab_homeAddress.TabIndex = 20;
			this.lab_homeAddress.Text = "End. Residencial";
			this.lab_homeAddress.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_origin.BackColor = Color.Transparent;
			this.lab_origin.Location = new Point(396, 205);
			this.lab_origin.Name = "lab_origin";
			this.lab_origin.Size = new Size(101, 13);
			this.lab_origin.TabIndex = 19;
			this.lab_origin.Text = "Origem";
			this.lab_origin.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_jobTitle.BackColor = Color.Transparent;
			this.lab_jobTitle.Location = new Point(44, 205);
			this.lab_jobTitle.Name = "lab_jobTitle";
			this.lab_jobTitle.Size = new Size(92, 13);
			this.lab_jobTitle.TabIndex = 17;
			this.lab_jobTitle.Text = "Título";
			this.lab_jobTitle.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_ethnic.BackColor = Color.Transparent;
			this.lab_ethnic.Location = new Point(396, 169);
			this.lab_ethnic.Name = "lab_ethnic";
			this.lab_ethnic.Size = new Size(101, 13);
			this.lab_ethnic.TabIndex = 28;
			this.lab_ethnic.Text = "Etinia";
			this.lab_ethnic.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_city.BackColor = Color.Transparent;
			this.lab_city.Location = new Point(396, 62);
			this.lab_city.Name = "lab_city";
			this.lab_city.Size = new Size(101, 13);
			this.lab_city.TabIndex = 22;
			this.lab_city.Text = "Cidade";
			this.lab_city.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_nationality.BackColor = Color.Transparent;
			this.lab_nationality.Location = new Point(396, 26);
			this.lab_nationality.Name = "lab_nationality";
			this.lab_nationality.Size = new Size(101, 13);
			this.lab_nationality.TabIndex = 20;
			this.lab_nationality.Text = "Nacionalidade";
			this.lab_nationality.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_workAddress.BackColor = Color.Transparent;
			this.lab_workAddress.Location = new Point(44, 309);
			this.lab_workAddress.Name = "lab_workAddress";
			this.lab_workAddress.Size = new Size(92, 13);
			this.lab_workAddress.TabIndex = 9;
			this.lab_workAddress.Text = "End. Comercial";
			this.lab_workAddress.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_IDnumber.BackColor = Color.Transparent;
			this.lab_IDnumber.Location = new Point(396, 133);
			this.lab_IDnumber.Name = "lab_IDnumber";
			this.lab_IDnumber.Size = new Size(101, 13);
			this.lab_IDnumber.TabIndex = 26;
			this.lab_IDnumber.Text = "Documento";
			this.lab_IDnumber.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_homeTelephone.BackColor = Color.Transparent;
			this.lab_homeTelephone.Location = new Point(396, 98);
			this.lab_homeTelephone.Name = "lab_homeTelephone";
			this.lab_homeTelephone.Size = new Size(101, 13);
			this.lab_homeTelephone.TabIndex = 24;
			this.lab_homeTelephone.Text = "Tel. Residencial";
			this.lab_homeTelephone.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_officeTelephone.BackColor = Color.Transparent;
			this.lab_officeTelephone.Location = new Point(44, 133);
			this.lab_officeTelephone.Name = "lab_officeTelephone";
			this.lab_officeTelephone.Size = new Size(92, 13);
			this.lab_officeTelephone.TabIndex = 29;
			this.lab_officeTelephone.Text = "Tel. Comercial";
			this.lab_officeTelephone.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_politicalStatus.BackColor = Color.Transparent;
			this.lab_politicalStatus.Location = new Point(44, 98);
			this.lab_politicalStatus.Name = "lab_politicalStatus";
			this.lab_politicalStatus.Size = new Size(92, 13);
			this.lab_politicalStatus.TabIndex = 27;
			this.lab_politicalStatus.Text = "Estado Civil";
			this.lab_politicalStatus.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_Education.BackColor = Color.Transparent;
			this.lab_Education.Location = new Point(44, 26);
			this.lab_Education.Name = "lab_Education";
			this.lab_Education.Size = new Size(92, 13);
			this.lab_Education.TabIndex = 21;
			this.lab_Education.Text = "Escolaridade";
			this.lab_Education.TextAlign = ContentAlignment.MiddleLeft;
			this.tabItem_detail.AttachedControl = this.tabControlPanel2;
			this.tabItem_detail.Name = "tabItem_detail";
			this.tabItem_detail.Text = "Detalhes";
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(691, 450);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(154, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 18;
			this.btn_cancel.Text = "Cancela";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(520, 450);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(154, 25);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 17;
			this.btn_ok.Text = "OK";
			this.btn_ok.Click += this.btnOk_Click;
			this.btn_saveContinue.AccessibleRole = AccessibleRole.PushButton;
			this.btn_saveContinue.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_saveContinue.Location = new Point(348, 450);
			this.btn_saveContinue.Name = "btn_saveContinue";
			this.btn_saveContinue.Size = new Size(154, 25);
			this.btn_saveContinue.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_saveContinue.TabIndex = 16;
			this.btn_saveContinue.Text = "Salva e Continua";
			this.btn_saveContinue.Click += this.btn_saveContinue_Click;
			this.openFileDialog1.FileName = "openFileDialog1";
			this.toolTip1.Tag = "";
			this.backgroundWorker1.DoWork += this.backgroundWorker1_DoWork;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(851, 487);
			base.Controls.Add(this.tabControl1);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.btn_saveContinue);
			base.Controls.Add(this.btn_cancel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Visitor2ManagementForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Visitante";
			base.FormClosed += this.PersonnelManagementForm_FormClosed;
			((ISupportInitialize)this.tabControl1).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabControlPanel3.ResumeLayout(false);
			this.tabControlPanel3.PerformLayout();
			((ISupportInitialize)this.pictureBox1).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((ISupportInitialize)this.cbo_datStart.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.cbo_datStart.Properties).EndInit();
			((ISupportInitialize)this.cbo_datEnd.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.cbo_datEnd.Properties).EndInit();
			((ISupportInitialize)this.dgrd_selectedLevel).EndInit();
			((ISupportInitialize)this.gridView2).EndInit();
			((ISupportInitialize)this.dgrd_optionalLevel).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			this.tabControlPanel1.ResumeLayout(false);
			this.tabControlPanel1.PerformLayout();
			((ISupportInitialize)this.DTPick_birthday.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.DTPick_birthday.Properties).EndInit();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			((ISupportInitialize)this.link_read).EndInit();
			((ISupportInitialize)this.pic_Readding).EndInit();
			((ISupportInitialize)this.pic_photo).EndInit();
			this.tabControlPanel2.ResumeLayout(false);
			this.tabControlPanel2.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
