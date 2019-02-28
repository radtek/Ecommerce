/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class AddDeviceForm : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		public delegate void FinishHandle(Machines model, bool isSuccess);

		public delegate bool IsContinusHadle(string msg);

		private enum enumDeviceType
		{
			InBio480 = 3,
			InBio280 = 5,
			InBio480_280,
			InBio160 = 8,
			InBio260,
			InBio460
		}

		private int m_ID;

		private int type = 1;

		private const int COMMU_MODE_PULL_TCPIP = 1;

		private const int COMMU_MODE_PULL_RS485 = 0;

		private MachinesBll bll = new MachinesBll(MainForm._ia);

		private AccDoorBll doorBll = new AccDoorBll(MainForm._ia);

		private Dictionary<int, string> SDKErrorList = new Dictionary<int, string>();

		public bool devHasAdd = false;

		private bool m_iscontinus = false;

		private DateTime m_finishDate = DateTime.Now;

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private List<PersonnelArea> Arealist = null;

		private WaitForm m_wait = WaitForm.Instance;

		private Thread m_thread = null;

		private Machines oldModel = new Machines();

		private WaitForm m_waitForm = WaitForm.Instance;

		private ImagesForm imgForm;

		private bool IsTestingConnection;

		private DataTable dtCardMode;

		private DataTable dtFreeType;

		private DataTable dtDateFmt;

		private DataTable dtFpVersion;

		private DataTable dtLanguage;

		private Machines MachineOld;

		private IContainer components = null;

		private Label lbl_deviceName;

		private Label lbl_commType;

		private Label lbl_IP;

		private Label lbl_IPPort;

		private Label lbl_password;

		private Label lbl_deviceType;

		private Label lbl_syncTime;

		private Label lbl_area;

		private Label lbl_deleteData;

		private TextBox txt_deviceName;

		private RadioButton rbtn_TCP;

		private RadioButton rbtn_RS485;

		private TextBox txt_IPPort;

		private TextBox txt_password;

		private ComboBox cbo_acpanelType;

		private CheckBoxX chk_syncTime;

		private CheckBoxX chk_deleteData;

		private ComboBox cbo_area;

		private Panel panel1;

		private Label label1;

		private Label label4;

		private Label lbl_star;

		private CheckBoxX chk_fourToTwo;

		private Label lbl_four_to_two;

		private Label label2;

		private System.Windows.Forms.Timer time_close;

		private Panel pnl_commParams;

		private Label lbl_starIP;

		private Label label3;

		private TextBox txt_RS485;

		private Label lbl_startPort;

		private ComboBox cbo_baudRate;

		private ComboBox cbo_serialNo;

		private Label lbl_BaudRate;

		private Label lbl_RS485;

		private Label lbl_SerialNo;

		private Panel panel2;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		public ButtonX btn_saveContinue;

		private GroupBox lbl_485master_slaves;

		private Label lbl_option_seting;

		private CheckBoxX rdo_485_slave;

		private CheckBoxX rdo_485_master;

		private TextBox txt_Rs485Reader;

		private Label lbl_Rs485Reader;

		private Label lbl_BaudRateRange;

		private ButtonX btnTestConnect;

		private GroupBox groupBox1;

		private CheckBox ckbTcpIp;

		private XtraTabControl xtraTabControl1;

		private XtraTabPage tabBaseSetting;

		private XtraTabPage tabVPSetting;

		private XtraTabPage tabDeviceControl;

		private GroupBox groupBox2;

		private ComboBox cbbCardMode;

		private Label label12;

		private CheckBox ckbMifireMustRegistered;

		private Label label11;

		private CheckBox ckbOnlyCheckCard;

		private Label label10;

		private CheckBox ckbOnly1_1Mode;

		private Label label9;

		private Label label8;

		private Label label7;

		private Label label6;

		private Label label5;

		public ButtonX btnInitDeviceData;

		public ButtonX btnSetTime;

		public ButtonX btnClearAdmin;

		public ButtonX btnShutdown;

		public ButtonX btnClearVerifyRecord;

		public ButtonX btnReboot;

		private GroupBox groupBox4;

		private ComboBox cbbFpVersion;

		private Label label16;

		private ComboBox cbbDateFmt;

		private Label label15;

		private CheckBox ckbKeypadVoice;

		private CheckBox ckbVirifyVoice;

		private CheckBox ckbTipsVoice;

		private Label label21;

		private Label label22;

		private Label label19;

		private Label label20;

		private ComboBox cbbLanguage;

		private Label label17;

		private Label label18;

		private NumericUpDown numOpOverTime;

		private NumericUpDown numFreeTime;

		private Label label14;

		private ComboBox cbbFreeType;

		private Label label13;

		private GroupBox groupBox3;

		private CheckBox ckbRS485;

		private CheckBox ckbRS232;

		private CheckBox ckbBatchUpdate;

		private ComboBox cbbFace121;

		private ComboBox cbbFace12N;

		private ComboBox cbbFP12N;

		private ComboBox cbbFP121;

		private ComboBox cbbVolume;

		private ErrorProvider errorProvider1;

		private CheckBoxX ckbInputPwd;

		private TextBox txt_IP;

		private Label lblIdElevatorTerminal;

		private NumericUpDown numElevatorTerminalId;

		private Label lblElevatorSourceFloor;

		private TextBox txtElevatorSourceFloor;

		private Panel panel3;

		private TextBox txtElevatorServerPort;

		private TextBox txtElevatorServerIP;

		private Label lblElevatorServerPort;

		private Label lblElevatorServerIp;

		public event EventHandler refreshDataEvent = null;

		public AddDeviceForm()
		{
			this.InitializeComponent();
			this.panel3.Visible = MainForm._elevatorEnabled;
			this.ckbInputPwd.Checked = false;
			DevLogServer.LockEx();
			this.devHasAdd = false;
			this.DeviceType();
			this.InitDefaultsValue();
			this.LoadAreaMsg();
			this.type = 1;
			this.rbtn_TCP.Checked = true;
			this.Rbtn_TCP_CheckedChanged(null, null);
			initLang.LocaleForm(this, base.Name);
			this.enabled(false);
			this.InitialSTDDataSource();
			this.BindDataSource();
		}

		public AddDeviceForm(int id)
			: this()
		{
			try
			{
				this.m_ID = id;
				this.BindData();
				if (id > 0)
				{
					this.cbo_acpanelType.Enabled = false;
				}
				else
				{
					this.rdo_485_master.Checked = false;
					this.rdo_485_slave.Checked = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void enabled(bool b)
		{
			this.lbl_485master_slaves.Visible = b;
			this.lbl_Rs485Reader.Visible = b;
			this.txt_Rs485Reader.Visible = b;
			this.lbl_BaudRateRange.Visible = b;
		}

		public AddDeviceForm(string ip)
			: this()
		{
			try
			{
				if (!string.IsNullOrEmpty(ip))
				{
					this.rbtn_RS485.Checked = false;
					this.rbtn_TCP.Checked = true;
					this.rbtn_RS485.Enabled = false;
					this.txt_IP.Text = ip;
					this.txt_IP.Enabled = false;
					this.txt_deviceName.Text = ip;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public AddDeviceForm(int com, int number, int baudRate)
			: this()
		{
			try
			{
				if (com > 0 && com <= 255)
				{
					this.txt_deviceName.Text = "COM" + com + "-" + number;
					this.cbo_serialNo.SelectedIndex = com - 1;
					this.txt_RS485.Text = number.ToString();
					int num = 0;
					while (num < this.cbo_baudRate.Items.Count)
					{
						if (!(this.cbo_baudRate.Items[num].ToString().Trim().ToLower() == baudRate.ToString()))
						{
							num++;
							continue;
						}
						this.cbo_baudRate.SelectedIndex = num;
						break;
					}
					this.rbtn_TCP.Checked = false;
					this.rbtn_RS485.Checked = true;
					this.rbtn_TCP.Enabled = false;
					this.rbtn_RS485.Enabled = true;
					this.Rbtn_RS485_CheckedChanged(null, null);
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

		private void SaveDoorInfo(Machines model)
		{
			if (model != null)
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				AccTimeseg accTimeseg = accTimesegBll.GetdefaultTime();
				int acpanel_type = model.acpanel_type;
				int iD = model.ID;
				for (int i = 0; i < acpanel_type; i++)
				{
					AccDoor accDoor = new AccDoor();
					accDoor.device_id = iD;
					accDoor.door_name = model.MachineAlias + "-" + (i + 1).ToString();
					accDoor.door_no = i + 1;
					accDoor.lock_active_id = accTimeseg.id;
					accDoor.long_open_id = 0;
					accDoor.force_pwd = string.Empty;
					accDoor.supper_pwd = string.Empty;
					if (model.device_type == 12 || model.device_type == 101 || model.device_type == 102 || model.device_type == 103)
					{
						accDoor.opendoor_type = 0;
					}
					else if (model.IsOnlyRFMachine == 0)
					{
						accDoor.opendoor_type = 6;
					}
					else
					{
						accDoor.opendoor_type = 4;
					}
					accDoor.door_sensor_status = 0;
					accDoor.lock_delay = 5;
					accDoor.card_intervaltime = 3;
					accDoor.sensor_delay = 15;
					accDoorBll.Add(accDoor);
				}
			}
		}

		private void AddMachines(Machines model)
		{
			if (model != null)
			{
				if (model.acpanel_type < 1 && this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_typeDic["0"];
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						if (this.cbo_acpanelType.Text == item.Value)
						{
							model.acpanel_type = int.Parse(item.Key);
							break;
						}
					}
				}
				if (this.bll.Add(model) >= 0)
				{
					if (this.refreshDataEvent != null)
					{
						this.refreshDataEvent(this, null);
					}
					this.time_close.Tag = true;
					DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
					if (deviceServer != null)
					{
						model.ID = this.bll.GetMaxId() - 1;
						deviceServer.IsAddOk = false;
						deviceServer.DevInfo.ID = model.ID;
						this.SaveDoorInfo(model);
					}
					this.devHasAdd = true;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
				}
			}
		}

		private void OnFinish(Machines model, bool isSuccess)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new FinishHandle(this.OnFinish), model, isSuccess);
				}
				else
				{
					this.Cursor = Cursors.Default;
					this.time_close.Tag = isSuccess;
					this.m_waitForm.Stop();
					if (isSuccess)
					{
						if (this.refreshDataEvent != null)
						{
							this.refreshDataEvent(this, null);
						}
						this.devHasAdd = true;
					}
					else
					{
						try
						{
							if (model != null)
							{
								if (model.device_type == 1000 && model.DevSDKType == SDKType.Undefined)
								{
									model.DevSDKType = SDKType.StandaloneSDK;
								}
								if (AccCommon.IsECardTong == 1)
								{
									Thread.Sleep(1500);
									this.m_waitForm.Hide();
									if (string.IsNullOrEmpty(Enum.GetName(typeof(enumDeviceType), model.device_type)) && this.IsContinus(this.m_waitForm.LastInfo + ", " + ShowMsgInfos.GetInfo("SAddDeviceFailedAndContinus", "是否添加到系统中?")))
									{
										this.AddMachines(model);
									}
								}
								else
								{
									Thread.Sleep(1500);
									this.m_waitForm.Hide();
									if (this.IsContinus(this.m_waitForm.LastInfo + ", " + ShowMsgInfos.GetInfo("SAddDeviceFailedAndContinus", "是否添加到系统中?")))
									{
										this.AddMachines(model);
									}
								}
							}
						}
						catch (Exception ex)
						{
							this.ShowInfos(ex.Message);
						}
					}
					this.m_finishDate = DateTime.Now;
					if (this.m_iscontinus)
					{
						this.time_close.Tag = false;
						this.DeviceType();
						this.InitDefaultsValue();
						this.LoadAreaMsg();
						this.txt_deviceName.Text = "";
						this.txt_password.Text = "";
						this.txt_RS485.Text = "";
						this.chk_syncTime.Checked = true;
						this.chk_deleteData.Checked = true;
						this.txt_IP.Text = null;
					}
					this.time_close.Enabled = true;
					this.btn_OK.Enabled = true;
					this.btn_cancel.Enabled = true;
					this.Cursor = Cursors.Default;
					this.m_waitForm.HideEx(false);
				}
			}
		}

		private bool IsContinus(string msg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new IsContinusHadle(this.IsContinus), msg);
				}
				else if (SysDialogs.ShowQueseMessage(msg, MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					return true;
				}
			}
			return false;
		}

		private void InitDefaultsValue()
		{
			this.cbo_serialNo.Items.Clear();
			for (int i = 1; i < 255; i++)
			{
				this.cbo_serialNo.Items.Add("COM" + i.ToString());
			}
			if (this.cbo_baudRate.Items.Count > 2)
			{
				this.cbo_baudRate.SelectedIndex = 2;
			}
			if (this.cbo_serialNo.Items.Count > 0)
			{
				this.cbo_serialNo.SelectedIndex = 0;
			}
			this.txt_IPPort.Text = "4370";
			if (this.cbo_acpanelType.Items.Count > 1)
			{
				this.cbo_acpanelType.SelectedIndex = 1;
			}
		}

		private void DeviceType()
		{
			this.m_typeDic = initLang.GetComboxInfo("DeviceType");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("1", "Controladora de 01 porta");
				dictionary.Add("2", "Controladora de 02 portas");
				dictionary.Add("4", "Controladora de 04 portas");
				dictionary.Add("100", "Standalone Multi-Protocolo");
				dictionary.Add("1000", "Standalone Padrão");
				this.m_typeDic.Add("0", dictionary);
				initLang.SetComboxInfo("DeviceType", this.m_typeDic);
				initLang.Save();
			}
			this.cbo_acpanelType.Items.Clear();
			if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary2 = this.m_typeDic["0"];
				foreach (KeyValuePair<string, string> item in dictionary2)
				{
					this.cbo_acpanelType.Items.Add(item.Value);
				}
			}
			initLang.ComboBoxAutoSize(this.cbo_acpanelType, this);
		}

		private void LoadAreaMsg()
		{
			this.cbo_area.Items.Clear();
			this.cbo_area.Items.Add("-------------");
			PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
			this.Arealist = personnelAreaBll.GetModelList("");
			if (this.Arealist != null && this.Arealist.Count > 0)
			{
				for (int i = 0; i < this.Arealist.Count; i++)
				{
					this.cbo_area.Items.Add(this.Arealist[i].areaname);
				}
			}
			if (this.cbo_area.Items.Count > 0)
			{
				this.cbo_area.SelectedIndex = 0;
			}
		}

		private void BindDataSource()
		{
			string[] array = new string[5]
			{
				ShowMsgInfos.GetInfo("VeryHeight", "很高"),
				ShowMsgInfos.GetInfo("Height", "高"),
				ShowMsgInfos.GetInfo("Medium", "中"),
				ShowMsgInfos.GetInfo("Low", "低"),
				ShowMsgInfos.GetInfo("VeryLow", "很低")
			};
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Value", typeof(int));
			dataTable.Columns.Add("Text", typeof(string));
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			dictionary.Add(0, 35);
			dictionary.Add(1, 25);
			dictionary.Add(2, 15);
			dictionary.Add(3, 10);
			dictionary.Add(4, 5);
			foreach (KeyValuePair<int, int> item in dictionary)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = item.Value;
				dataRow[1] = array[item.Key];
				dataTable.Rows.Add(dataRow);
			}
			this.cbbFP121.DisplayMember = "Text";
			this.cbbFP121.ValueMember = "Value";
			this.cbbFP121.DataSource = dataTable;
			DataTable dataTable2 = dataTable.Clone();
			dictionary = new Dictionary<int, int>();
			dictionary.Add(0, 50);
			dictionary.Add(1, 45);
			dictionary.Add(2, 35);
			dictionary.Add(3, 25);
			dictionary.Add(4, 15);
			foreach (KeyValuePair<int, int> item2 in dictionary)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow[0] = item2.Value;
				dataRow[1] = array[item2.Key];
				dataTable2.Rows.Add(dataRow);
			}
			this.cbbFP12N.DisplayMember = "Text";
			this.cbbFP12N.ValueMember = "Value";
			this.cbbFP12N.DataSource = dataTable2;
			DataTable dataTable3 = dataTable.Clone();
			dictionary = new Dictionary<int, int>();
			dictionary.Add(1, 80);
			dictionary.Add(2, 70);
			dictionary.Add(3, 65);
			foreach (KeyValuePair<int, int> item3 in dictionary)
			{
				DataRow dataRow = dataTable3.NewRow();
				dataRow[0] = item3.Value;
				dataRow[1] = array[item3.Key];
				dataTable3.Rows.Add(dataRow);
			}
			this.cbbFace121.DisplayMember = "Text";
			this.cbbFace121.ValueMember = "Value";
			this.cbbFace121.DataSource = dataTable3;
			DataTable dataTable4 = dataTable.Clone();
			dictionary = new Dictionary<int, int>();
			dictionary.Add(1, 90);
			dictionary.Add(2, 80);
			dictionary.Add(3, 75);
			foreach (KeyValuePair<int, int> item4 in dictionary)
			{
				DataRow dataRow = dataTable4.NewRow();
				dataRow[0] = item4.Value;
				dataRow[1] = array[item4.Key];
				dataTable4.Rows.Add(dataRow);
			}
			this.cbbFace12N.DisplayMember = "Text";
			this.cbbFace12N.ValueMember = "Value";
			this.cbbFace12N.DataSource = dataTable4;
			DataTable dataTable5 = dataTable.Clone();
			dictionary = new Dictionary<int, int>();
			dictionary.Add(1, 90);
			dictionary.Add(2, 55);
			dictionary.Add(3, 25);
			foreach (KeyValuePair<int, int> item5 in dictionary)
			{
				DataRow dataRow = dataTable5.NewRow();
				dataRow[0] = item5.Value;
				dataRow[1] = array[item5.Key];
				dataTable5.Rows.Add(dataRow);
			}
			this.cbbVolume.DisplayMember = "Text";
			this.cbbVolume.ValueMember = "Value";
			this.cbbVolume.DataSource = dataTable5;
		}

		private int GetLevelVolume(int volume)
		{
			if (volume >= 80)
			{
				return 90;
			}
			if (volume >= 50)
			{
				return 55;
			}
			return 25;
		}

		private int GetLevelThreshold(int Threshold, bool Is12N, bool IsFp)
		{
			if (IsFp)
			{
				if (Is12N)
				{
					if (Threshold >= 50)
					{
						return 50;
					}
					if (Threshold >= 45)
					{
						return 45;
					}
					if (Threshold >= 35)
					{
						return 35;
					}
					if (Threshold >= 25)
					{
						return 25;
					}
					return 15;
				}
				if (Threshold >= 35)
				{
					return 35;
				}
				if (Threshold >= 25)
				{
					return 25;
				}
				if (Threshold >= 15)
				{
					return 15;
				}
				if (Threshold >= 10)
				{
					return 10;
				}
				return 5;
			}
			if (Is12N)
			{
				if (Threshold >= 90)
				{
					return 90;
				}
				if (Threshold >= 80)
				{
					return 80;
				}
				return 75;
			}
			if (Threshold >= 80)
			{
				return 80;
			}
			if (Threshold >= 70)
			{
				return 70;
			}
			return 65;
		}

		private void BindData()
		{
			if (this.m_ID > 0)
			{
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				Machines machines = this.oldModel = machinesBll.GetModel(this.m_ID);
				this.MachineOld = machines.Copy();
				this.lbl_deleteData.Enabled = false;
				this.chk_deleteData.Enabled = false;
				this.chk_deleteData.Checked = false;
				this.lbl_four_to_two.Enabled = false;
				this.chk_fourToTwo.Enabled = false;
				this.txt_password.Enabled = false;
				if (machines != null)
				{
					this.cbbFpVersion.SelectedValue = machines.FpVersion;
					this.numElevatorTerminalId.Value = machines.ElevatorTerminalId;
					this.txtElevatorSourceFloor.Text = machines.ElevatorSourceFloor;
					this.txtElevatorServerIP.Text = machines.ElevatorIp;
					this.txtElevatorServerPort.Text = machines.ElevatorPort;
					if (machines.DevSDKType == SDKType.StandaloneSDK)
					{
						this.tabVPSetting.PageVisible = true;
						this.tabDeviceControl.PageVisible = true;
						this.ckbTcpIp.Checked = (machines.NetOn == 1);
						this.ckbRS232.Checked = (machines.RS232On == 1);
						this.ckbRS485.Checked = (machines.RS485On == 1);
						this.cbbFP121.SelectedValue = this.GetLevelThreshold(machines.FP1_1Threshold, false, true);
						this.cbbFP12N.SelectedValue = this.GetLevelThreshold(machines.FP1_NThreshold, true, true);
						this.cbbFace121.SelectedValue = this.GetLevelThreshold(machines.Face1_1Threshold, false, false);
						this.cbbFace12N.SelectedValue = this.GetLevelThreshold(machines.Face1_NThreshold, true, false);
						this.ckbOnly1_1Mode.Checked = (machines.Only1_1Mode == 1);
						this.ckbOnlyCheckCard.Checked = (machines.OnlyCheckCard == 1);
						this.ckbMifireMustRegistered.Checked = (machines.MifireMustRegistered == 1);
						int num = (machines.RFCardOn != 1 || machines.Mifire != 0 || machines.MifireId != 0) ? ((machines.RFCardOn == 1 && machines.Mifire == 1 && machines.MifireId == 1) ? 1 : ((machines.RFCardOn != 1 || machines.Mifire != 1 || machines.MifireId != 0) ? (-1) : 2)) : 0;
						this.cbbCardMode.SelectedValue = num;
						this.cbbFreeType.Enabled = !machines.IsTFTMachine;
						this.cbbFreeType.SelectedValue = (machines.IsTFTMachine ? 88 : machines.FreeType);
						this.numFreeTime.Value = machines.FreeTime;
						this.cbbDateFmt.SelectedValue = machines.DateFormat;
						this.cbbFpVersion.SelectedValue = machines.FpVersion;
						this.cbbLanguage.SelectedValue = machines.UILanguage;
						this.ckbKeypadVoice.Checked = (machines.KeyPadBeep == 1);
						this.ckbTipsVoice.Checked = (machines.VoiceTipsOn == 1);
						this.ckbVirifyVoice.Checked = (machines.VRYVH == 1);
						this.numOpOverTime.Value = (((decimal)machines.TOMenu < this.numOpOverTime.Minimum) ? this.numOpOverTime.Minimum : (((decimal)machines.TOMenu > this.numOpOverTime.Maximum) ? this.numOpOverTime.Maximum : ((decimal)machines.TOMenu)));
						this.cbbVolume.SelectedValue = this.GetLevelVolume(machines.VOLUME);
						this.ckbBatchUpdate.Checked = machines.BatchUpdate;
					}
					this.txt_deviceName.Text = machines.MachineAlias;
					this.txt_IP.Text = machines.IP;
					this.txt_IP.Enabled = false;
					this.txt_IPPort.Enabled = false;
					this.txt_password.Text = machines.CommPassword;
					TextBox textBox = this.txt_IPPort;
					int num2 = machines.Port;
					textBox.Text = num2.ToString();
					TextBox textBox2 = this.txt_RS485;
					num2 = machines.MachineNumber;
					textBox2.Text = num2.ToString();
					if (this.Arealist != null)
					{
						int num3 = 0;
						while (num3 < this.Arealist.Count)
						{
							if (this.Arealist[num3].id != machines.area_id)
							{
								num3++;
								continue;
							}
							this.cbo_area.SelectedIndex = num3 + 1;
							break;
						}
					}
					if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
					{
						Dictionary<string, string> dictionary = this.m_typeDic["0"];
						if (machines.device_type < 100)
						{
							foreach (KeyValuePair<string, string> item in dictionary)
							{
								num2 = machines.acpanel_type;
								if (num2.ToString() == item.Key)
								{
									this.cbo_acpanelType.Text = item.Value;
									break;
								}
							}
						}
						else
						{
							Dictionary<string, string> dictionary2 = dictionary;
							num2 = machines.device_type;
							if (dictionary2.ContainsKey(num2.ToString()))
							{
								ComboBox comboBox = this.cbo_acpanelType;
								Dictionary<string, string> dictionary3 = dictionary;
								num2 = machines.device_type;
								comboBox.Text = dictionary3[num2.ToString()];
							}
							else
							{
								foreach (KeyValuePair<string, string> item2 in dictionary)
								{
									if (item2.Key == "100" && (machines.device_type == 12 || machines.device_type == 101 || machines.device_type == 102 || machines.device_type == 103))
									{
										this.cbo_acpanelType.Text = item2.Value;
										break;
									}
								}
							}
						}
					}
					if (machines.ConnectType == 0)
					{
						this.rbtn_RS485.Checked = true;
						this.rbtn_TCP.Enabled = false;
						for (int i = 0; i < this.cbo_baudRate.Items.Count; i++)
						{
							string a = this.cbo_baudRate.Items[i].ToString().Trim().ToLower();
							num2 = machines.Baudrate;
							if (a == num2.ToString())
							{
								this.cbo_baudRate.SelectedIndex = i;
								break;
							}
						}
						this.cbo_serialNo.SelectedIndex = machines.SerialPort - 1;
					}
					else
					{
						this.rbtn_RS485.Enabled = false;
						this.rbtn_TCP.Checked = true;
					}
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					TextBox textBox3 = this.txt_Rs485Reader;
					num2 = machines.MachineNumber;
					textBox3.Text = num2.ToString();
					this.rdo_485_master.Checked = false;
					this.rdo_485_slave.Checked = false;
					if (machines.deviceOption != null)
					{
						byte[] deviceOption = machines.deviceOption;
						string @string = Encoding.ASCII.GetString(deviceOption, 0, deviceOption.Length);
						string[] array = @string.Split(',');
						List<string> list = new List<string>();
						if (array.Length != 0)
						{
							for (int j = 0; j < array.Length; j++)
							{
								list.Add(array[j]);
							}
						}
						if (list.Contains("PC485AsInbio485=0") && list.Contains("MasterInbio485=0"))
						{
							this.rdo_485_master.Checked = false;
							this.rdo_485_slave.Checked = false;
						}
						else if (list.Contains("PC485AsInbio485=1") && list.Contains("MasterInbio485=1"))
						{
							this.rdo_485_master.Checked = true;
						}
						else if (list.Contains("PC485AsInbio485=1") && list.Contains("MasterInbio485=0"))
						{
							this.rdo_485_slave.Checked = true;
						}
					}
				}
				this.ckbInputPwd.Checked = false;
				this.ckbInputPwd.Enabled = false;
			}
			else
			{
				this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				this.ckbInputPwd.Checked = false;
				this.ckbInputPwd.Enabled = true;
			}
		}

		private void Rbtn_RS485_CheckedChanged(object sender, EventArgs e)
		{
			this.type = 0;
			this.pnl_commParams.Visible = true;
			this.panel1.Visible = false;
		}

		private void Rbtn_TCP_CheckedChanged(object sender, EventArgs e)
		{
			this.type = 1;
			this.pnl_commParams.Visible = false;
			this.panel1.Visible = true;
		}

		private bool BindModel(Machines model)
		{
			try
			{
				if (this.cbo_area.SelectedIndex > 0 && this.cbo_area.SelectedIndex <= this.Arealist.Count)
				{
					model.area_id = this.Arealist[this.cbo_area.SelectedIndex - 1].id;
				}
				model.MachineAlias = this.txt_deviceName.Text;
				model.CommPassword = this.txt_password.Text;
				model.ElevatorTerminalId = Convert.ToInt32(this.numElevatorTerminalId.Value);
				model.ElevatorSourceFloor = this.txtElevatorSourceFloor.Text;
				if (AccCommon.IsECardTong > 0)
				{
					model.AccFun = 255;
				}
				if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_typeDic["0"];
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						if (this.cbo_acpanelType.Text == item.Value)
						{
							string key = item.Key;
							if (!(key == "1000"))
							{
								if (key == "100")
								{
									model.acpanel_type = 1;
									model.device_type = 101;
									model.DevSDKType = SDKType.PullSDK;
								}
								else
								{
									model.acpanel_type = int.Parse(item.Key);
									model.DevSDKType = SDKType.PullSDK;
								}
							}
							else
							{
								model.acpanel_type = 1;
								model.device_type = 1000;
								model.DevSDKType = SDKType.StandaloneSDK;
							}
							break;
						}
					}
				}
				if (this.type == 1)
				{
					model.IP = this.txt_IP.Text;
					model.ConnectType = 1;
					if (!string.IsNullOrEmpty(this.txt_IPPort.Text))
					{
						model.Port = int.Parse(this.txt_IPPort.Text);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(this.txt_RS485.Text))
					{
						model.MachineNumber = int.Parse(this.txt_RS485.Text);
					}
					if (!string.IsNullOrEmpty(this.cbo_baudRate.Text))
					{
						model.Baudrate = int.Parse(this.cbo_baudRate.Text);
					}
					if (!string.IsNullOrEmpty(this.cbo_serialNo.Text) && this.cbo_serialNo.Text.Length > 3)
					{
						model.SerialPort = int.Parse(this.cbo_serialNo.Text.Substring(3, this.cbo_serialNo.Text.Length - 3));
					}
					model.ConnectType = 0;
				}
				model.NetOn = (this.ckbTcpIp.Checked ? 1 : 0);
				model.RS232On = (this.ckbRS232.Checked ? 1 : 0);
				model.RS485On = (this.ckbRS485.Checked ? 1 : 0);
				model.FP1_1Threshold = int.Parse((this.cbbFP121.SelectedValue ?? ((object)this.oldModel.FP1_1Threshold)).ToString());
				model.FP1_NThreshold = int.Parse((this.cbbFP12N.SelectedValue ?? ((object)this.oldModel.FP1_NThreshold)).ToString());
				model.Face1_1Threshold = int.Parse((this.cbbFace121.SelectedValue ?? ((object)this.oldModel.Face1_1Threshold)).ToString());
				model.Face1_NThreshold = int.Parse((this.cbbFace12N.SelectedValue ?? ((object)this.oldModel.Face1_NThreshold)).ToString());
				model.Only1_1Mode = (this.ckbOnly1_1Mode.Checked ? 1 : 0);
				model.OnlyCheckCard = (this.ckbOnlyCheckCard.Checked ? 1 : 0);
				model.MifireMustRegistered = (this.ckbMifireMustRegistered.Checked ? 1 : 0);
				model.ElevatorTerminalId = (short)this.numElevatorTerminalId.Value;
				model.ElevatorSourceFloor = this.txtElevatorSourceFloor.Text;
				model.ElevatorPort = this.txtElevatorServerPort.Text;
				model.ElevatorIp = this.txtElevatorServerIP.Text;
				if (this.cbbCardMode.SelectedValue == null)
				{
					model.RFCardOn = this.oldModel.RFCardOn;
					model.Mifire = this.oldModel.Mifire;
					model.MifireId = this.oldModel.MifireId;
				}
				else if (int.Parse((this.cbbCardMode.SelectedValue ?? ((object)0)).ToString()) == 1)
				{
					model.RFCardOn = 1;
					model.Mifire = 1;
					model.MifireId = 1;
				}
				else if (int.Parse((this.cbbCardMode.SelectedValue ?? ((object)0)).ToString()) == 2)
				{
					model.RFCardOn = 1;
					model.Mifire = 1;
					model.MifireId = 0;
				}
				else
				{
					model.RFCardOn = 1;
					model.Mifire = 0;
					model.MifireId = 0;
				}
				if (!model.IsTFTMachine)
				{
					model.FreeType = int.Parse((this.cbbFreeType.SelectedValue ?? ((object)this.oldModel.FreeType)).ToString());
				}
				model.FreeTime = (int)this.numFreeTime.Value;
				model.DateFormat = short.Parse((this.cbbDateFmt.SelectedValue ?? ((object)this.oldModel.DateFormat)).ToString());
				model.FpVersion = int.Parse((this.cbbFpVersion.SelectedValue ?? ((object)this.oldModel.FpVersion)).ToString());
				model.VoiceTipsOn = (this.ckbTipsVoice.Checked ? 1 : 0);
				model.KeyPadBeep = (this.ckbKeypadVoice.Checked ? 1 : 0);
				model.VRYVH = (this.ckbVirifyVoice.Checked ? 1 : 0);
				model.TOMenu = (int)this.numOpOverTime.Value;
				model.VOLUME = int.Parse((this.cbbVolume.SelectedValue ?? ((object)this.oldModel.VOLUME)).ToString());
				model.BatchUpdate = this.ckbBatchUpdate.Checked;
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool UpdateModel(Machines model)
		{
			try
			{
				this.bll.Update(model);
				if (this.IsModelChanged(this.MachineOld, model))
				{
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					DevCmds devCmds = new DevCmds();
					devCmds.SN_id = model.ID;
					devCmds.status = 0;
					devCmds.CmdContent = "setdev$" + model.ToPullCmdString();
					devCmds.CmdCommitTime = DateTime.Now;
					devCmds.CmdReturnContent = string.Empty;
					devCmdsBll.Add(devCmds);
					if (MainForm._elevatorEnabled)
					{
						string[][] array = new string[4][]
						{
							new string[2]
							{
								"ElevatorId",
								model.ElevatorTerminalId.ToString()
							},
							new string[2]
							{
								"SourceFoor",
								model.ElevatorSourceFloor
							},
							new string[2]
							{
								"ElevatorIp",
								model.ElevatorIp
							},
							new string[2]
							{
								"ElevatorPort",
								model.ElevatorPort.ToString()
							}
						};
						for (int i = 0; i < array.Length; i++)
						{
							devCmds = new DevCmds();
							devCmds.SN_id = model.ID;
							devCmds.status = 0;
							devCmds.CmdContent = "set$" + array[i][0] + "=" + array[i][1];
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							devCmdsBll.Add(devCmds);
						}
					}
					FrmShowUpdata.Instance.ShowEx();
				}
				DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
				if (deviceServer != null && deviceServer.DevInfo != null)
				{
					deviceServer.DevInfo.Port = model.Port;
					deviceServer.DevInfo.CommPassword = model.CommPassword;
					deviceServer.DevInfo.Baudrate = model.Baudrate;
					deviceServer.DevInfo.SerialPort = model.SerialPort;
					deviceServer.DevInfo.MachineNumber = model.MachineNumber;
					if (deviceServer.IsConnected)
					{
						deviceServer.Disconnect();
						deviceServer.Connect(3000);
					}
				}
				if (this.refreshDataEvent != null)
				{
					this.refreshDataEvent(this, null);
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool Door4ToDoor2(Machines model)
		{
			try
			{
				int num = DeviceHelper.SetDoor4ToDoor2(model, this.chk_fourToTwo.Checked);
				if (num < 0)
				{
					this.ShowInfos(ShowMsgInfos.GetInfo("SetDoor4ToDoor2False", "四门转两门失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				this.ShowInfos(ex.Message);
				return false;
			}
		}

		private bool SyncDeviceTime(Machines model, DeviceServerBll devServerBll)
		{
			try
			{
				if (this.chk_syncTime.Checked)
				{
					int num = (model.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.SyncDeviceTime(devServerBll) : devServerBll.STD_SetDeviceTime(null);
					if (num < 0)
					{
						this.ShowInfos(ShowMsgInfos.GetInfo("SSyncDevTimeFailed", "同步时间失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				this.ShowInfos(ex.Message);
				return false;
			}
		}

		private bool DeleteDeviceData(Machines model, DeviceServerBll devServerBll)
		{
			try
			{
				if (this.chk_deleteData.Checked)
				{
					int num = 0;
					num = DeviceHelper.DeletePullDeviceData(model, devServerBll);
					if (num < 0)
					{
						this.ShowInfos(ShowMsgInfos.GetInfo("SDelDevDataFailed", "删除设备数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
					this.ShowInfos(ShowMsgInfos.GetInfo("SDelDevDataSucceed", "删除设备数据成功"));
				}
				return true;
			}
			catch (Exception ex)
			{
				this.ShowInfos(ex.Message);
				return false;
			}
		}

		private bool Add(Machines model)
		{
			try
			{
				Machines machines = model.Copy();
				int id = -1002;
				DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
				if (deviceServer != null)
				{
					this.ShowProgress(5);
					id = DeviceHelper.TestConnect(model);
					int num = id;
					if (id < 0)
					{
						if (-14 == id || -100006 == id)
						{
							string str = ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败") + ": ";
							str += PullSDkErrorInfos.GetInfo(id);
							this.ShowInfos(str);
							return false;
						}
						model.DevSDKType = ((model.DevSDKType == SDKType.StandaloneSDK) ? SDKType.PullSDK : SDKType.StandaloneSDK);
						if (model.ConnectType == 0)
						{
							Thread.Sleep(1000);
						}
						num = DeviceHelper.TestConnect(model);
					}
					if (num < 0)
					{
						model.DevSDKType = SDKType.Undefined;
						string str = ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败") + ": ";
						str += PullSDkErrorInfos.GetInfo(id);
						if (id != num && PullSDkErrorInfos.GetInfo(id) != PullSDkErrorInfos.GetInfo(num))
						{
							str = str + " " + ShowMsgInfos.GetInfo("LogicOr", "或") + " " + PullSDkErrorInfos.GetInfo(num);
						}
						this.ShowInfos(str);
						return false;
					}
					deviceServer = DeviceServers.Instance.GetDeviceServer(model);
					this.ShowProgress(10);
					this.ShowInfos(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + model.MachineAlias + ")");
					id = deviceServer.Connect(3000);
					if (id >= 0)
					{
						this.ShowProgress(20);
						this.ShowInfos(ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功") + "(" + model.MachineAlias + ")");
						int acpanel_type = model.acpanel_type;
						id = ((model.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.GetDeviceParams(model) : deviceServer.STD_GetDeviceParam(model));
						model.IP = this.txt_IP.Text;
						if (model.ConnectType == 0)
						{
							model.MachineNumber = int.Parse(this.txt_RS485.Text);
						}
						if (model.DevSDKType == SDKType.StandaloneSDK && model.RS232On != 1 && model.RS485On != 1)
						{
							model.RS485On = 1;
						}
						if (id >= 0)
						{
							if (AccCommon.IsECardTong == 1 && !string.IsNullOrEmpty(Enum.GetName(typeof(enumDeviceType), model.device_type)))
							{
								this.ShowInfos(ShowMsgInfos.GetInfo("NonsupportDeviceType", "不支持该类型设备!") + "(" + model.MachineAlias + ")");
								deviceServer.Disconnect();
								if (model.ConnectType == 0)
								{
									Program.KillPlrscagent();
								}
								return false;
							}
							int num2;
							if (acpanel_type != model.acpanel_type)
							{
								if (acpanel_type == 4 && model.acpanel_type == 2 && model.four_to_two)
								{
									if (!this.chk_fourToTwo.Checked)
									{
										this.Door4ToDoor2(model);
									}
								}
								else if (model.DevSDKType == SDKType.StandaloneSDK)
								{
									num2 = 1000;
									string text = num2.ToString();
									if (this.m_typeDic != null && this.m_typeDic.Count > 0)
									{
										Dictionary<string, string> dictionary = this.m_typeDic["0"];
										if (dictionary.ContainsKey(text))
										{
											text = dictionary[text];
										}
									}
									model.device_type = 1000;
									this.ShowInfos(ShowMsgInfos.GetInfo("SAcpanelTypeWrongIsCorrection", "控制器类型与实际不符,将修改为正确的控制器类型") + ":" + text);
								}
								else if (model.device_type == 12 || model.device_type == 101 || model.device_type == 102 || model.device_type == 103)
								{
									object obj;
									if (model.DevSDKType != SDKType.StandaloneSDK)
									{
										obj = "100";
									}
									else
									{
										num2 = 1000;
										obj = num2.ToString();
									}
									string text2 = (string)obj;
									if (this.m_typeDic != null && this.m_typeDic.Count > 0)
									{
										Dictionary<string, string> dictionary2 = this.m_typeDic["0"];
										if (dictionary2.ContainsKey(text2))
										{
											text2 = dictionary2[text2];
										}
									}
									this.ShowInfos(ShowMsgInfos.GetInfo("SAcpanelTypeWrongIsCorrection", "控制器类型与实际不符,将修改为正确的控制器类型") + ":" + text2);
								}
								else
								{
									num2 = model.acpanel_type;
									string str2 = num2.ToString();
									if (this.m_typeDic != null && this.m_typeDic.Count > 0)
									{
										Dictionary<string, string> dictionary3 = this.m_typeDic["0"];
										Dictionary<string, string> dictionary4 = dictionary3;
										num2 = model.acpanel_type;
										if (dictionary4.ContainsKey(num2.ToString()))
										{
											Dictionary<string, string> dictionary5 = dictionary3;
											num2 = model.acpanel_type;
											str2 = dictionary5[num2.ToString()];
										}
									}
									this.ShowInfos(ShowMsgInfos.GetInfo("SAcpanelTypeWrongIsCorrection", "控制器类型与实际不符,将修改为正确的控制器类型") + ":" + str2);
								}
							}
							else if (model.acpanel_type == 4 && this.chk_fourToTwo.Checked)
							{
								this.Door4ToDoor2(model);
							}
							else if (model.acpanel_type == 1 && model.DevSDKType != machines.DevSDKType)
							{
								if (this.m_typeDic != null && this.m_typeDic.Count > 0)
								{
									object obj2;
									if (model.DevSDKType != SDKType.StandaloneSDK)
									{
										obj2 = "100";
									}
									else
									{
										num2 = 1000;
										obj2 = num2.ToString();
									}
									string text3 = (string)obj2;
									Dictionary<string, string> dictionary6 = this.m_typeDic["0"];
									if (dictionary6.ContainsKey(text3))
									{
										text3 = dictionary6[text3];
									}
									this.ShowInfos(ShowMsgInfos.GetInfo("SAcpanelTypeWrongIsCorrection", "控制器类型与实际不符,将修改为正确的控制器类型") + ":" + text3);
								}
								model.device_type = ((model.DevSDKType == SDKType.StandaloneSDK) ? 1000 : 100);
							}
							this.ShowProgress(35);
							if (model.DevSDKType == SDKType.StandaloneSDK)
							{
								if (this.chk_syncTime.Checked)
								{
									id = deviceServer.STD_SetDeviceTime(null);
									if (id >= 0)
									{
										this.ShowInfos(ShowMsgInfos.GetInfo("SSyncDevTimeSucceed", "同步时间成功"));
									}
									else
									{
										this.ShowInfos(ShowMsgInfos.GetInfo("SSyncDevTimeFailed", "同步时间失败") + ":" + PullSDkErrorInfos.GetInfo(id));
									}
								}
							}
							else
							{
								this.SyncDeviceTime(model, deviceServer);
							}
							this.ShowProgress(55);
							if (this.chk_deleteData.Checked)
							{
								if (model.DevSDKType == SDKType.StandaloneSDK)
								{
									id = deviceServer.STD_InitializeDeviceData(false);
									if (id >= 0)
									{
										this.ShowInfos(ShowMsgInfos.GetInfo("SDelDevDataSucceed", "删除设备数据成功"));
									}
									else
									{
										this.ShowInfos(ShowMsgInfos.GetInfo("SDelDevDataFailed", "删除设备数据失败") + ":" + PullSDkErrorInfos.GetInfo(id));
									}
								}
								else
								{
									this.DeleteDeviceData(model, deviceServer);
								}
							}
							id = DeviceHelper.UpdateDataCount(model, null);
							this.ShowProgress(65);
							if (this.bll.Add(model) > 0)
							{
								try
								{
									model.ID = this.bll.GetMaxId() - 1;
									deviceServer.DevInfo.ID = model.ID;
									this.ShowProgress(75);
									bool @checked = this.chk_deleteData.Checked;
									if (model.DevSDKType == SDKType.StandaloneSDK)
									{
										this.ShowInfos(this.AddSTDDoorInfo(model, @checked));
									}
									else
									{
										this.ShowInfos(DeviceHelper.SaveDoorInfo(model, @checked));
									}
									if (model.DevSDKType != SDKType.StandaloneSDK)
									{
										if (this.AddReader(model))
										{
											this.ShowProgress(85);
											this.ShowInfos(ShowMsgInfos.GetInfo("AddAccReaderSucceed", "添加读头成功完成"));
										}
										else
										{
											this.ShowInfos(ShowMsgInfos.GetInfo("AddAccReaderFailed", "添加读头失败"));
										}
									}
									if (model.aux_in_count > 0 || model.aux_out_count > 0)
									{
										if (this.AddAuxiliary(model))
										{
											this.ShowInfos(ShowMsgInfos.GetInfo("AddAccAuxiliarySucceed", "添加辅助输入输出成功完成"));
											this.ShowProgress(95);
										}
										else
										{
											this.ShowInfos(ShowMsgInfos.GetInfo("AddAccAuxiliaryFailed", "添加辅助输入输出失败"));
										}
									}
									if (MainForm._elevatorEnabled)
									{
										string[][] obj3 = new string[4][];
										string[] obj4 = new string[2]
										{
											"ElevatorId",
											null
										};
										num2 = model.ElevatorTerminalId;
										obj4[1] = num2.ToString();
										obj3[0] = obj4;
										obj3[1] = new string[2]
										{
											"SourceFoor",
											model.ElevatorSourceFloor
										};
										obj3[2] = new string[2]
										{
											"ElevatorIp",
											model.ElevatorIp
										};
										obj3[3] = new string[2]
										{
											"ElevatorPort",
											model.ElevatorPort.ToString()
										};
										string[][] array = obj3;
										this.ShowInfos(ShowMsgInfos.GetInfo("SettingElevatorParams", "Enviando parametros do Elevador..."));
										for (int i = 0; i < array.Length; i++)
										{
											deviceServer.SetDeviceParam(array[i][0] + "=" + array[i][1]);
											this.ShowInfos(array[i][0] + ": OK");
										}
										this.ShowInfos(ShowMsgInfos.GetInfo("SettingElevatorParamsOK", "PArametros de Elevador configurados corretamente!"));
									}
									this.ShowProgress(100);
									return true;
								}
								catch (Exception)
								{
									this.bll.Delete(model);
									throw;
								}
							}
							this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败"));
						}
						else
						{
							this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败") + ":" + PullSDkErrorInfos.GetInfo(id));
						}
					}
					else
					{
						this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败") + ":" + PullSDkErrorInfos.GetInfo(id));
					}
					deviceServer.Disconnect();
					if (model.ConnectType == 0)
					{
						Program.KillPlrscagent();
					}
				}
				else
				{
					this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败") + ":" + PullSDkErrorInfos.GetInfo(id));
				}
				return false;
			}
			catch (Exception ex2)
			{
				this.ShowInfos(ex2.Message);
				return false;
			}
		}

		private string AddSTDDoorInfo(Machines model, bool isResetDoorInfo)
		{
			DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
			StringBuilder stringBuilder = new StringBuilder();
			if (deviceServer != null)
			{
				int num = 0;
				int iD = model.ID;
				int acpanel_type = model.acpanel_type;
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				STD_WiegandFmtBll sTD_WiegandFmtBll = new STD_WiegandFmtBll(MainForm._ia);
				AccTimeseg accTimeseg = accTimesegBll.GetdefaultTime();
				for (int i = 0; i < acpanel_type; i++)
				{
					AccDoor accDoor = new AccDoor();
					try
					{
						accDoor.is_att = true;
						accDoor.device_id = iD;
						accDoor.door_name = model.MachineAlias + "-" + (i + 1).ToString();
						accDoor.door_no = i + 1;
						accDoor.long_open_id = 0;
						accDoor.force_pwd = string.Empty;
						accDoor.supper_pwd = string.Empty;
						accDoor.opendoor_type = 0;
						accDoor.card_intervaltime = 0;
						accDoor.lock_active_id = accTimeseg.id;
						if (isResetDoorInfo)
						{
							accDoor.door_sensor_status = 0;
							accDoor.lock_delay = 5;
							accDoor.card_intervaltime = 0;
							accDoor.sensor_delay = 15;
							accDoor.GroupTZ1 = accTimeseg.TimeZone1Id;
							accDoor.GroupTZ2 = accTimeseg.TimeZone2Id;
							accDoor.GroupTZ3 = accTimeseg.TimeZone3Id;
							num = deviceServer.STD_SetDoorParam(accDoor, DeviceHelper.TimeSeg.id);
							if (num >= 0)
							{
								stringBuilder.Append(ShowMsgInfos.GetInfo("SetDoorInfoOk", "从PC同步门设置信息到设备成功") + ":" + accDoor.door_name + "\r\n");
							}
							else
							{
								stringBuilder.Append(ShowMsgInfos.GetInfo("SetDoorInfoFalse", "从PC同步门设置信息到设备失败") + "-" + accDoor.door_name + ":" + PullSDkErrorInfos.GetInfo(num) + "\r\n");
							}
						}
						else
						{
							deviceServer.STD_GetDoorParam(accDoor);
						}
					}
					catch (Exception ex)
					{
						SysLogServer.WriteLog("ZK.Access.AddDeviceForm.AddSTDDoorInfo throwed exception:\r\n" + ex.Message);
					}
					accDoorBll.Add(accDoor);
					try
					{
						num = deviceServer.STD_GetWiegandFmt(out STD_WiegandFmt sTD_WiegandFmt);
						if (num < 0)
						{
							sTD_WiegandFmt = new STD_WiegandFmt();
						}
						STD_WiegandFmt modelByDoorId = sTD_WiegandFmtBll.GetModelByDoorId(accDoor.id);
						if (modelByDoorId != null)
						{
							sTD_WiegandFmtBll.Delete(modelByDoorId.Id);
						}
						sTD_WiegandFmt.DoorId = accDoor.id;
						sTD_WiegandFmtBll.Add(sTD_WiegandFmt);
					}
					catch (Exception ex2)
					{
						SysLogServer.WriteLog("ZK.Access.AddDeviceForm.AddSTDDoorInfo throwed exception:\r\n" + ex2.Message);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private bool AddAuxiliary(Machines model)
		{
			int num = 0;
			int[] array = null;
			int[] array2 = null;
			AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
			List<AccAuxiliary> list = new List<AccAuxiliary>();
			switch (model.device_type)
			{
			case 3:
			case 6:
				array = new int[4]
				{
					9,
					10,
					11,
					12
				};
				array2 = new int[6]
				{
					2,
					4,
					6,
					8,
					9,
					10
				};
				break;
			case 5:
				array = new int[4]
				{
					409,
					410,
					411,
					412
				};
				array2 = new int[4]
				{
					2,
					4,
					409,
					410
				};
				break;
			default:
			{
				int[] array3 = new int[InAddressInfo.GetDic().Count];
				int[] array4 = new int[OutAddressInfo.GetDic().Count];
				InAddressInfo.GetDic().Keys.CopyTo(array3, 0);
				OutAddressInfo.GetDic().Keys.CopyTo(array4, 0);
				array = new int[model.aux_in_count];
				array2 = new int[model.aux_out_count];
				try
				{
					for (int i = 1; i <= model.aux_in_count; i++)
					{
						array[i - 1] = array3[i];
					}
					for (int j = 1; j <= model.aux_out_count; j++)
					{
						array2[j - 1] = array4[j];
					}
				}
				catch (IndexOutOfRangeException)
				{
					throw new Exception(ShowMsgInfos.GetInfo("AddInOutAuxiliaryCountError", "辅助输入输出数量错误"));
				}
				break;
			}
			}
			for (num = 0; num < array.Length; num++)
			{
				AccAuxiliary accAuxiliary = new AccAuxiliary();
				accAuxiliary.AuxName = InAddressInfo.GetInfo(array[num]);
				accAuxiliary.AuxNo = array[num] % 100;
				accAuxiliary.AuxState = AccAuxiliary.AccAuxiliaryState.In;
				accAuxiliary.DeviceId = model.ID;
				accAuxiliary.PrinterNumber = InAddressInfo.GetInfo(array[num]);
				list.Add(accAuxiliary);
			}
			for (num = 0; num < array2.Length; num++)
			{
				AccAuxiliary accAuxiliary2 = new AccAuxiliary();
				accAuxiliary2.AuxName = OutAddressInfo.GetInfo(array2[num]);
				accAuxiliary2.AuxNo = array2[num] % 100;
				accAuxiliary2.AuxState = AccAuxiliary.AccAuxiliaryState.Out;
				accAuxiliary2.DeviceId = model.ID;
				accAuxiliary2.PrinterNumber = OutAddressInfo.GetInfo(array2[num]);
				list.Add(accAuxiliary2);
			}
			return accAuxiliaryBll.Add(list) > 0;
		}

		private bool AddReader(Machines model)
		{
			int num = 0;
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + model.ID + " Order by door_no");
			AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
			List<AccReader> list = new List<AccReader>();
			for (int i = 0; i < modelList.Count; i++)
			{
				num++;
				AccReader accReader = new AccReader();
				accReader.ReaderNo = num;
				accReader.DoorId = modelList[i].id;
				accReader.ReaderName = modelList[i].door_name + " " + ShowMsgInfos.GetInfo("AccReaderInState", "入");
				accReader.ReaderState = AccReader.AccReaderState.In;
				list.Add(accReader);
				int device_type = model.device_type;
				if (device_type == 2)
				{
					if (model.Ext485ReaderFunOn > 0)
					{
						num++;
						accReader = new AccReader();
						accReader.ReaderNo = num;
						accReader.DoorId = modelList[i].id;
						accReader.ReaderName = modelList[i].door_name + " " + ShowMsgInfos.GetInfo("AccReaderOutState", "出");
						accReader.ReaderState = AccReader.AccReaderState.Out;
						list.Add(accReader);
					}
				}
				else
				{
					num++;
					accReader = new AccReader();
					accReader.ReaderNo = num;
					accReader.DoorId = modelList[i].id;
					accReader.ReaderName = modelList[i].door_name + " " + ShowMsgInfos.GetInfo("AccReaderOutState", "出");
					accReader.ReaderState = AccReader.AccReaderState.Out;
					list.Add(accReader);
				}
			}
			return accReaderBll.Add(list) > 0;
		}

		private bool Check(bool IsTestConnect = false)
		{
			if (!this.chk_deleteData.Checked && this.m_ID <= 0 && !IsTestConnect && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SNotDelDevData", "你没有选择[新增时删除设备中数据]，该功能仅用于系统功能演示和测试。请及时手动同步数据到设备已确保系统中和设备中的权限一致，确定要继续？"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				return false;
			}
			try
			{
				int num = 0;
				num = ((AccCommon.IsECardTong != 1) ? this.bll.MachineCount("") : this.bll.MachineCount(" AccFun = 255 "));
				if (AccCommon.IsECardTong == 1)
				{
					if (num >= 30)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMachineCountOverflow", "最大设备数量不能超过") + 30);
						return false;
					}
				}
				else if (num >= 100)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMachineCountOverflow", "最大设备数量不能超过") + 100);
					return false;
				}
				if (!IsTestConnect)
				{
					if (this.m_ID > 0 && this.oldModel != null && this.oldModel.device_name != null && this.oldModel.DevSDKType == SDKType.StandaloneSDK && this.ckbOnly1_1Mode.Checked)
					{
						switch (this.oldModel.device_name.ToUpper())
						{
						case "F6":
						case "F11":
						case "MA300":
						case "SF101":
							if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("1T1InvalidVT", "启用1:1验证会导致该设备无法正常使用. 确定要继续吗?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
							{
								break;
							}
							this.xtraTabControl1.SelectedTabPage = this.tabVPSetting;
							this.ckbOnly1_1Mode.Focus();
							return false;
						}
					}
					if (string.IsNullOrEmpty(this.txt_deviceName.Text.Trim()))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputDeviceName", "请输入设备名称!"));
						this.txt_deviceName.Focus();
						return false;
					}
					if (this.m_ID == 0)
					{
						if (AccCommon.IsECardTong == 1)
						{
							if (this.bll.ExistsDevice(this.txt_deviceName.Text.Trim(), "AccFun = 255"))
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNameRepeat", "设备名称重复!"));
								this.txt_deviceName.Focus();
								return false;
							}
						}
						else if (this.bll.ExistsDevice(this.txt_deviceName.Text.Trim(), ""))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNameRepeat", "设备名称重复!"));
							this.txt_deviceName.Focus();
							return false;
						}
					}
					if (this.cbo_area.SelectedIndex == 0 || string.IsNullOrEmpty(this.cbo_area.Text.Trim()))
					{
						if (SkinParameters.SkinOption == 0)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeviceArea", "请选择设备区域"));
							this.cbo_area.Focus();
							return false;
						}
						if (this.cbo_area.Items.Count > 1)
						{
							this.cbo_area.SelectedIndex = 1;
						}
					}
				}
				if ((this.rdo_485_master.Checked || this.rdo_485_slave.Checked) && string.IsNullOrEmpty(this.txt_Rs485Reader.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputRS485Address", "请输入RS485地址"));
					this.txt_Rs485Reader.Focus();
					return false;
				}
				if (this.type == 1)
				{
					if (string.IsNullOrEmpty(this.txt_IP.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputIPAddress", "请输入设备IP地址"));
						this.txt_IP.Focus();
						return false;
					}
					if (this.m_ID == 0 && !IsTestConnect)
					{
						List<Machines> modelList = this.bll.GetModelList($"ip='{this.txt_IP.Text}' and port={this.txt_IPPort.Text}");
						if (AccCommon.IsECardTong == 1)
						{
							if (this.bll.ExistsIP(this.txt_IP.Text, "AccFun = 255 and Port=" + this.txt_IPPort.Text))
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIPAddressRepeat", "IP地址重复!"));
								this.txt_IP.Focus();
								return false;
							}
						}
						else if (this.bll.ExistsIP(this.txt_IP.Text, "Port=" + this.txt_IPPort.Text))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIPAddressRepeat", "IP地址重复!"));
							this.txt_IP.Focus();
							return false;
						}
					}
					if (string.IsNullOrEmpty(this.txt_IPPort.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputIPPort", "请输入设备端口地址"));
						this.txt_IPPort.Focus();
						return false;
					}
					return true;
				}
				if (this.type == 0)
				{
					int num2 = 0;
					if (this.cbo_serialNo.SelectedIndex < 0 || string.IsNullOrEmpty(this.cbo_serialNo.Text) || this.cbo_serialNo.Text.Length < 4)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectSerialNo", "请选择串口号"));
						this.cbo_serialNo.Focus();
						return false;
					}
					num2 = int.Parse(this.cbo_serialNo.Text.Substring(3));
					if (string.IsNullOrEmpty(this.txt_RS485.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputRS485Address", "请输入RS485地址"));
						this.txt_RS485.Focus();
						return false;
					}
					if (this.m_ID == 0)
					{
						if (AccCommon.IsECardTong == 1)
						{
							if (this.bll.ExistsRS485(int.Parse(this.txt_RS485.Text), num2, "AccFun = 255"))
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressRepeat", "RS485地址重复!"));
								this.txt_RS485.Focus();
								return false;
							}
						}
						else if (this.bll.ExistsRS485(int.Parse(this.txt_RS485.Text), num2, ""))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressRepeat", "RS485地址重复!"));
							this.txt_RS485.Focus();
							return false;
						}
						return true;
					}
					if (int.Parse(this.txt_RS485.Text) > 255)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressToobig", "RS485地址不能大于255"));
						this.txt_RS485.Focus();
						return false;
					}
					if (this.cbo_baudRate.SelectedIndex < 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectbaudRate", "请选择波特率"));
						this.cbo_baudRate.Focus();
						return false;
					}
					if (this.cbo_baudRate.SelectedIndex < 0 || string.IsNullOrEmpty(this.cbo_baudRate.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectbaudRate", "请选择波特率"));
						this.cbo_baudRate.Focus();
						return false;
					}
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					List<Machines> list = null;
					list = machinesBll.GetModelList("");
					int num3 = int.Parse(this.cbo_baudRate.Text);
					num2 = int.Parse(this.cbo_serialNo.Text.Substring(3));
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].ConnectType == 0 && list[i].SerialPort == num2 && num3 != list[i].Baudrate)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIsAddNotSameSerialNo", "已添加过波特率不为") + ":" + num3 + " " + ShowMsgInfos.GetInfo("SIsNotAllowSameSerialNoDiffDev", "的设备！同一个串口下不允许存在多个波特率不同的设备。请重新选择波特率"));
								this.cbo_baudRate.Focus();
								return false;
							}
						}
					}
					return true;
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void Start(object obj)
		{
			if (obj != null)
			{
				Machines machines = obj as Machines;
				if (machines != null)
				{
					if (this.Add(machines))
					{
						this.OnFinish(machines, true);
					}
					else
					{
						this.OnFinish(machines, false);
					}
				}
				else
				{
					this.OnFinish(machines, false);
				}
			}
			else
			{
				this.OnFinish(null, false);
			}
			this.m_thread = null;
		}

		private bool Save()
		{
			try
			{
				if (this.Check(false))
				{
					Machines machines = null;
					if (this.m_ID > 0)
					{
						machines = this.bll.GetModel(this.m_ID);
						this.oldModel = machines.Copy();
					}
					else
					{
						machines = new Machines();
					}
					if (this.BindModel(machines))
					{
						if (machines.ID > 0)
						{
							this.time_close.Enabled = false;
							this.btn_OK.Enabled = false;
							this.btn_cancel.Enabled = false;
							this.Cursor = Cursors.WaitCursor;
							machines.device_type = this.oldModel.device_type;
							this.UpdateModel(machines);
							this.time_close.Enabled = true;
							this.btn_OK.Enabled = true;
							this.btn_cancel.Enabled = true;
							this.Cursor = Cursors.Default;
							return true;
						}
						this.time_close.Enabled = false;
						this.btn_OK.Enabled = false;
						this.btn_cancel.Enabled = false;
						this.Cursor = Cursors.WaitCursor;
						this.m_waitForm.ShowEx();
						if (this.m_thread == null)
						{
							this.m_thread = new Thread(this.Start);
							this.m_thread.Start(machines);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			return false;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void cbo_doorType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cbo_acpanelType.SelectedIndex == 2)
			{
				if (this.m_ID <= 0)
				{
					this.lbl_four_to_two.Enabled = true;
					this.chk_fourToTwo.Enabled = true;
				}
				this.lbl_485master_slaves.Visible = false;
				this.lbl_Rs485Reader.Visible = false;
				this.txt_Rs485Reader.Visible = false;
				this.lbl_BaudRateRange.Visible = false;
			}
			else if (this.cbo_acpanelType.SelectedIndex == 3 && this.m_ID > 0 && this.rbtn_TCP.Checked)
			{
				this.enabled(true);
			}
			else
			{
				this.lbl_four_to_two.Enabled = false;
				this.chk_fourToTwo.Enabled = false;
				this.chk_fourToTwo.Checked = false;
				this.enabled(false);
			}
		}

		private void txt_RS485_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 3);
		}

		private void txt_password_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 8);
			if (e.KeyChar == ' ')
			{
				e.Handled = true;
			}
		}

		private void txt_deviceName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_IPPort_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 4);
		}

		private void time_close_Tick(object sender, EventArgs e)
		{
			if (this.m_thread == null)
			{
				this.time_close.Enabled = false;
				this.m_waitForm.HideEx(false);
				if (this.time_close.Tag != null && this.time_close.Tag.ToString().ToLower() == "true")
				{
					base.Close();
				}
			}
		}

		private void AddDeviceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.m_thread != null || this.IsTestingConnection)
			{
				e.Cancel = true;
			}
		}

		private void AddDeviceForm_Click(object sender, EventArgs e)
		{
			if (this.m_thread == null)
			{
				this.m_waitForm.HideEx(false);
			}
		}

		private void AddDeviceForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			DevLogServer.UnLockEx();
		}

		private void btn_saveContinue_Click(object sender, EventArgs e)
		{
			this.m_iscontinus = true;
			this.Save();
		}

		private void rdo_485_master_Click(object sender, EventArgs e)
		{
			this.rdo_485_slave.Checked = false;
		}

		private void rdo_485_slave_Click(object sender, EventArgs e)
		{
			this.rdo_485_master.Checked = false;
		}

		private void txt_Rs485Reader_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 255L);
		}

		private void lbl_485master_slaves_Enter(object sender, EventArgs e)
		{
		}

		private void btnTestConnect_Click(object sender, EventArgs e)
		{
			if (this.Check(true))
			{
				Machines machines = new Machines();
				if (this.BindModel(machines))
				{
					this.imgForm = new ImagesForm();
					Thread thread = new Thread(this.TestConnect);
					this.panel2.Enabled = false;
					this.imgForm.Show();
					thread.Start(machines);
				}
			}
		}

		private void TestConnect(object obj)
		{
			try
			{
				this.IsTestingConnection = true;
				Machines machines = obj as Machines;
				int num = DeviceHelper.TestConnect(machines);
				if (num >= 0 && this.oldModel != null && this.oldModel.ID > 0)
				{
					DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machines);
					deviceServer?.Disconnect();
				}
				if (num < 0 && (this.oldModel == null || this.oldModel.ID <= 0) && -14 != num && -100006 != num)
				{
					if (machines.DevSDKType == SDKType.StandaloneSDK)
					{
						machines.DevSDKType = SDKType.PullSDK;
					}
					else
					{
						machines.DevSDKType = SDKType.StandaloneSDK;
					}
					if (machines.ConnectType == 0)
					{
						Thread.Sleep(1000);
					}
					num = DeviceHelper.TestConnect(machines);
					if (num >= 0 && this.oldModel != null && this.oldModel.ID > 0)
					{
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machines);
						deviceServer?.Disconnect();
					}
				}
				this.TestConnectFinished(num);
			}
			catch (Exception ex)
			{
				this.TestConnectFinished(-99);
				SysDialogs.ShowInfoMessage(ex.Message);
			}
			this.IsTestingConnection = false;
		}

		private void TestConnectFinished(int result)
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.TestConnectFinished(result);
				});
			}
			else
			{
				if (this.imgForm != null)
				{
					this.imgForm.Close();
				}
				this.panel2.Enabled = true;
				if (result >= 0)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ConnectSuceed", "连接成功"));
				}
				else
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ConnectFailed", "连接失败") + ": " + PullSDkErrorInfos.GetInfo(result));
				}
			}
		}

		private void ckbRS232_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ckbRS232.Checked)
			{
				this.ckbRS485.Checked = false;
			}
		}

		private void ckbRS485_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ckbRS485.Checked)
			{
				this.ckbRS232.Checked = false;
			}
		}

		private bool GetDeviceServer(out DeviceServerBll devBll)
		{
			int num = 0;
			devBll = null;
			Machines machines = this.oldModel;
			if (machines == null)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("MachineNotExists", "设备不存在"));
				return false;
			}
			devBll = DeviceServers.Instance.GetDeviceServer(machines);
			if (devBll == null)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ConnectFailed", "设备连接失败"));
				return false;
			}
			num = devBll.Connect(3000);
			if (num < 0)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ConnectFailed", "设备连接失败"));
				return false;
			}
			return true;
		}

		private void btnReboot_Click(object sender, EventArgs e)
		{
			ImagesForm imagesForm = new ImagesForm();
			imagesForm.Owner = this;
			imagesForm.Show();
			Application.DoEvents();
			if (!this.GetDeviceServer(out DeviceServerBll deviceServerBll))
			{
				imagesForm.Close();
			}
			else
			{
				int num = deviceServerBll.RebootDevice();
				imagesForm.Close();
				deviceServerBll.Disconnect();
			}
		}

		private void btnShutdown_Click(object sender, EventArgs e)
		{
			ImagesForm imagesForm = new ImagesForm();
			imagesForm.Owner = this;
			imagesForm.Show();
			Application.DoEvents();
			if (!this.GetDeviceServer(out DeviceServerBll deviceServerBll))
			{
				imagesForm.Close();
			}
			else
			{
				int num = deviceServerBll.STD_ShutdownDevice();
				imagesForm.Close();
				deviceServerBll.Disconnect();
			}
		}

		private void btnSetTime_Click(object sender, EventArgs e)
		{
			ImagesForm imagesForm = new ImagesForm();
			imagesForm.Owner = this;
			imagesForm.Show();
			Application.DoEvents();
			if (!this.GetDeviceServer(out DeviceServerBll deviceServerBll))
			{
				imagesForm.Close();
			}
			else
			{
				int num = deviceServerBll.STD_SetDeviceTime(null);
				imagesForm.Close();
				if (num < 0)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgFailed", "操作失败") + "\r\n" + PullSDkErrorInfos.GetInfo(num));
				}
				else
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgSucceed", "操作成功"));
					deviceServerBll.Disconnect();
				}
			}
		}

		private void btnClearAdmin_Click(object sender, EventArgs e)
		{
			ImagesForm imagesForm = new ImagesForm();
			imagesForm.Owner = this;
			imagesForm.Show();
			Application.DoEvents();
			if (!this.GetDeviceServer(out DeviceServerBll deviceServerBll))
			{
				imagesForm.Close();
			}
			else
			{
				int num = deviceServerBll.STD_ClearAdministrators();
				imagesForm.Close();
				if (num < 0)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgFailed", "操作失败") + "\r\n" + PullSDkErrorInfos.GetInfo(num));
				}
				else
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgSucceed", "操作成功"));
					deviceServerBll.Disconnect();
				}
			}
		}

		private void btnInitDeviceData_Click(object sender, EventArgs e)
		{
			ImagesForm imagesForm = new ImagesForm();
			imagesForm.Owner = this;
			imagesForm.Show();
			Application.DoEvents();
			if (!this.GetDeviceServer(out DeviceServerBll deviceServerBll))
			{
				imagesForm.Close();
			}
			else
			{
				int num = deviceServerBll.STD_InitializeDeviceData(false);
				imagesForm.Close();
				if (num < 0)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgFailed", "操作失败") + "\r\n" + PullSDkErrorInfos.GetInfo(num));
				}
				else
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgSucceed", "操作成功"));
					deviceServerBll.Disconnect();
				}
			}
		}

		private void btnClearVerifyRecord_Click(object sender, EventArgs e)
		{
			if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("ClearingEventLogs", "验证记录清除后将无法恢复，请确保设备上的验证记录已经成功下载\r\n\r\n" + ShowMsgInfos.GetInfo("CheckForContinue", "确定要继续清除验证记录吗")), MessageBoxButtons.OKCancel, MessageBoxDefaultButton.Button1) == DialogResult.OK)
			{
				ImagesForm imagesForm = new ImagesForm();
				imagesForm.Owner = this;
				imagesForm.Show();
				Application.DoEvents();
				if (!this.GetDeviceServer(out DeviceServerBll deviceServerBll))
				{
					imagesForm.Close();
				}
				else
				{
					int num = deviceServerBll.STD_ClearGLog();
					imagesForm.Close();
					if (num < 0)
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgFailed", "操作失败") + "\r\n" + PullSDkErrorInfos.GetInfo(num));
					}
					else
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("OperationgSucceed", "操作成功"));
						deviceServerBll.Disconnect();
					}
				}
			}
		}

		private void InitialSTDDataSource()
		{
			this.dtCardMode = new DataTable();
			this.dtCardMode.Columns.Add("Value", typeof(int));
			this.dtCardMode.Columns.Add("DisplayText", typeof(string));
			Dictionary<int, string> dic = STD_CardMode.GetDic();
			foreach (KeyValuePair<int, string> item in dic)
			{
				DataRow dataRow = this.dtCardMode.NewRow();
				dataRow[0] = item.Key;
				dataRow[1] = item.Value;
				this.dtCardMode.Rows.Add(dataRow);
			}
			this.dtFreeType = this.dtCardMode.Clone();
			dic = STD_FreeType.GetDic();
			foreach (KeyValuePair<int, string> item2 in dic)
			{
				DataRow dataRow = this.dtFreeType.NewRow();
				dataRow[0] = item2.Key;
				dataRow[1] = item2.Value;
				this.dtFreeType.Rows.Add(dataRow);
			}
			this.dtDateFmt = this.dtCardMode.Clone();
			dic = STD_DateFormat.GetDic();
			foreach (KeyValuePair<int, string> item3 in dic)
			{
				DataRow dataRow = this.dtDateFmt.NewRow();
				dataRow[0] = item3.Key;
				dataRow[1] = item3.Value;
				this.dtDateFmt.Rows.Add(dataRow);
			}
			this.dtFpVersion = this.dtCardMode.Clone();
			dic = STD_FpVersion.GetDic();
			foreach (KeyValuePair<int, string> item4 in dic)
			{
				DataRow dataRow = this.dtFpVersion.NewRow();
				dataRow[0] = item4.Key;
				dataRow[1] = item4.Value;
				this.dtFpVersion.Rows.Add(dataRow);
			}
			this.dtLanguage = this.dtCardMode.Clone();
			dic = STD_Language.GetDic();
			foreach (KeyValuePair<int, string> item5 in dic)
			{
				DataRow dataRow = this.dtLanguage.NewRow();
				dataRow[0] = item5.Key;
				dataRow[1] = item5.Value;
				this.dtLanguage.Rows.Add(dataRow);
			}
			this.cbbCardMode.DataSource = this.dtCardMode;
			this.cbbCardMode.ValueMember = "Value";
			this.cbbCardMode.DisplayMember = "DisplayText";
			this.cbbDateFmt.DataSource = this.dtDateFmt;
			this.cbbDateFmt.ValueMember = "Value";
			this.cbbDateFmt.DisplayMember = "DisplayText";
			this.cbbFpVersion.DataSource = this.dtFpVersion;
			this.cbbFpVersion.ValueMember = "Value";
			this.cbbFpVersion.DisplayMember = "DisplayText";
			this.cbbFreeType.DataSource = this.dtFreeType;
			this.cbbFreeType.ValueMember = "Value";
			this.cbbFreeType.DisplayMember = "DisplayText";
			this.cbbLanguage.DataSource = this.dtLanguage;
			this.cbbLanguage.ValueMember = "Value";
			this.cbbLanguage.DisplayMember = "DisplayText";
		}

		private bool IsModelChanged(Machines oldMachine, Machines newMachine)
		{
			if (oldMachine.UserExtFmt != newMachine.UserExtFmt || oldMachine.FP1_NThreshold != newMachine.FP1_NThreshold || oldMachine.FP1_1Threshold != newMachine.FP1_1Threshold || oldMachine.Face1_NThreshold != newMachine.Face1_NThreshold || oldMachine.Face1_1Threshold != newMachine.Face1_1Threshold || oldMachine.Only1_1Mode != newMachine.Only1_1Mode || oldMachine.OnlyCheckCard != newMachine.OnlyCheckCard || oldMachine.MifireMustRegistered != newMachine.MifireMustRegistered || oldMachine.RFCardOn != newMachine.RFCardOn || oldMachine.Mifire != newMachine.Mifire || oldMachine.MifireId != newMachine.MifireId || oldMachine.NetOn != newMachine.NetOn || oldMachine.RS232On != newMachine.RS232On || oldMachine.RS485On != newMachine.RS485On || oldMachine.FreeType != newMachine.FreeType || oldMachine.FreeTime != newMachine.FreeTime || oldMachine.DateFormat != newMachine.DateFormat || oldMachine.NoDisplayFun != newMachine.NoDisplayFun || oldMachine.UILanguage != newMachine.UILanguage || oldMachine.VoiceTipsOn != newMachine.VoiceTipsOn || oldMachine.VRYVH != newMachine.VRYVH || oldMachine.KeyPadBeep != newMachine.KeyPadBeep || oldMachine.FpVersion != newMachine.FpVersion || oldMachine.VOLUME != newMachine.VOLUME || oldMachine.TOMenu != newMachine.TOMenu || oldMachine.ElevatorTerminalId != newMachine.ElevatorTerminalId || !oldMachine.ElevatorSourceFloor.Equals(newMachine.ElevatorSourceFloor) || !oldMachine.ElevatorPort.Equals(newMachine.ElevatorPort) || !oldMachine.ElevatorIp.Equals(newMachine.ElevatorIp))
			{
				return true;
			}
			return false;
		}

		private void cbbFpVersion_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.MachineOld == null)
			{
				this.errorProvider1.SetError(this.cbbFpVersion, "");
			}
			else
			{
				int.TryParse((this.cbbFpVersion.SelectedValue ?? ((object)this.MachineOld.FpVersion)).ToString(), out int num);
				if (this.MachineOld.FpVersion != num)
				{
					this.errorProvider1.SetError(this.cbbFpVersion, ShowMsgInfos.GetInfo("WillValidAfterReboot", "重启后才生效"));
				}
				else
				{
					this.errorProvider1.SetError(this.cbbFpVersion, "");
				}
			}
		}

		private void ckbInputPwd_CheckedChanged(object sender, EventArgs e)
		{
			this.txt_password.Enabled = this.ckbInputPwd.Checked;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddDeviceForm));
			this.lbl_deviceName = new Label();
			this.lbl_commType = new Label();
			this.lbl_IP = new Label();
			this.lbl_IPPort = new Label();
			this.lbl_password = new Label();
			this.lbl_deviceType = new Label();
			this.lbl_syncTime = new Label();
			this.lbl_area = new Label();
			this.lbl_deleteData = new Label();
			this.txt_deviceName = new TextBox();
			this.rbtn_TCP = new RadioButton();
			this.rbtn_RS485 = new RadioButton();
			this.txt_IPPort = new TextBox();
			this.txt_password = new TextBox();
			this.cbo_acpanelType = new ComboBox();
			this.chk_syncTime = new CheckBoxX();
			this.chk_deleteData = new CheckBoxX();
			this.cbo_area = new ComboBox();
			this.panel1 = new Panel();
			this.txt_IP = new TextBox();
			this.txt_Rs485Reader = new TextBox();
			this.lbl_BaudRateRange = new Label();
			this.lbl_Rs485Reader = new Label();
			this.lbl_star = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.label4 = new Label();
			this.chk_fourToTwo = new CheckBoxX();
			this.lbl_four_to_two = new Label();
			this.time_close = new System.Windows.Forms.Timer(this.components);
			this.pnl_commParams = new Panel();
			this.lbl_starIP = new Label();
			this.label3 = new Label();
			this.txt_RS485 = new TextBox();
			this.lbl_startPort = new Label();
			this.cbo_baudRate = new ComboBox();
			this.cbo_serialNo = new ComboBox();
			this.lbl_BaudRate = new Label();
			this.lbl_RS485 = new Label();
			this.lbl_SerialNo = new Label();
			this.panel2 = new Panel();
			this.btn_saveContinue = new ButtonX();
			this.btnTestConnect = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lbl_485master_slaves = new GroupBox();
			this.rdo_485_slave = new CheckBoxX();
			this.rdo_485_master = new CheckBoxX();
			this.lbl_option_seting = new Label();
			this.groupBox1 = new GroupBox();
			this.ckbRS485 = new CheckBox();
			this.ckbRS232 = new CheckBox();
			this.ckbTcpIp = new CheckBox();
			this.xtraTabControl1 = new XtraTabControl();
			this.tabBaseSetting = new XtraTabPage();
			this.panel3 = new Panel();
			this.txtElevatorServerPort = new TextBox();
			this.txtElevatorServerIP = new TextBox();
			this.lblElevatorServerPort = new Label();
			this.lblElevatorServerIp = new Label();
			this.lblIdElevatorTerminal = new Label();
			this.txtElevatorSourceFloor = new TextBox();
			this.numElevatorTerminalId = new NumericUpDown();
			this.lblElevatorSourceFloor = new Label();
			this.ckbInputPwd = new CheckBoxX();
			this.tabVPSetting = new XtraTabPage();
			this.groupBox2 = new GroupBox();
			this.cbbCardMode = new ComboBox();
			this.ckbMifireMustRegistered = new CheckBox();
			this.ckbOnlyCheckCard = new CheckBox();
			this.ckbOnly1_1Mode = new CheckBox();
			this.cbbFace12N = new ComboBox();
			this.cbbFace121 = new ComboBox();
			this.cbbFP12N = new ComboBox();
			this.cbbFP121 = new ComboBox();
			this.label12 = new Label();
			this.label11 = new Label();
			this.label10 = new Label();
			this.label9 = new Label();
			this.label8 = new Label();
			this.label7 = new Label();
			this.label6 = new Label();
			this.label5 = new Label();
			this.tabDeviceControl = new XtraTabPage();
			this.ckbBatchUpdate = new CheckBox();
			this.groupBox3 = new GroupBox();
			this.cbbFreeType = new ComboBox();
			this.numFreeTime = new NumericUpDown();
			this.label13 = new Label();
			this.label14 = new Label();
			this.groupBox4 = new GroupBox();
			this.cbbLanguage = new ComboBox();
			this.ckbKeypadVoice = new CheckBox();
			this.ckbVirifyVoice = new CheckBox();
			this.ckbTipsVoice = new CheckBox();
			this.numOpOverTime = new NumericUpDown();
			this.cbbVolume = new ComboBox();
			this.cbbFpVersion = new ComboBox();
			this.cbbDateFmt = new ComboBox();
			this.label22 = new Label();
			this.label21 = new Label();
			this.label19 = new Label();
			this.label20 = new Label();
			this.label17 = new Label();
			this.label18 = new Label();
			this.label16 = new Label();
			this.label15 = new Label();
			this.btnInitDeviceData = new ButtonX();
			this.btnSetTime = new ButtonX();
			this.btnClearAdmin = new ButtonX();
			this.btnShutdown = new ButtonX();
			this.btnClearVerifyRecord = new ButtonX();
			this.btnReboot = new ButtonX();
			this.errorProvider1 = new ErrorProvider(this.components);
			this.panel1.SuspendLayout();
			this.pnl_commParams.SuspendLayout();
			this.panel2.SuspendLayout();
			this.lbl_485master_slaves.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.xtraTabControl1).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.tabBaseSetting.SuspendLayout();
			this.panel3.SuspendLayout();
			((ISupportInitialize)this.numElevatorTerminalId).BeginInit();
			this.tabVPSetting.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabDeviceControl.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((ISupportInitialize)this.numFreeTime).BeginInit();
			this.groupBox4.SuspendLayout();
			((ISupportInitialize)this.numOpOverTime).BeginInit();
			((ISupportInitialize)this.errorProvider1).BeginInit();
			base.SuspendLayout();
			this.lbl_deviceName.Location = new Point(10, 9);
			this.lbl_deviceName.Name = "lbl_deviceName";
			this.lbl_deviceName.Size = new Size(254, 13);
			this.lbl_deviceName.TabIndex = 0;
			this.lbl_deviceName.Text = "设备名称";
			this.lbl_deviceName.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_commType.Location = new Point(11, 207);
			this.lbl_commType.Name = "lbl_commType";
			this.lbl_commType.Size = new Size(212, 13);
			this.lbl_commType.TabIndex = 17;
			this.lbl_commType.Text = "通信方式";
			this.lbl_commType.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_IP.Location = new Point(5, 14);
			this.lbl_IP.Name = "lbl_IP";
			this.lbl_IP.Size = new Size(196, 13);
			this.lbl_IP.TabIndex = 0;
			this.lbl_IP.Text = "IP地址";
			this.lbl_IP.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_IPPort.Location = new Point(5, 41);
			this.lbl_IPPort.Name = "lbl_IPPort";
			this.lbl_IPPort.Size = new Size(193, 13);
			this.lbl_IPPort.TabIndex = 4;
			this.lbl_IPPort.Text = "IP端口号";
			this.lbl_IPPort.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_password.Location = new Point(10, 40);
			this.lbl_password.Name = "lbl_password";
			this.lbl_password.Size = new Size(254, 13);
			this.lbl_password.TabIndex = 3;
			this.lbl_password.Text = "通信密码";
			this.lbl_password.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_deviceType.Location = new Point(10, 68);
			this.lbl_deviceType.Name = "lbl_deviceType";
			this.lbl_deviceType.Size = new Size(254, 13);
			this.lbl_deviceType.TabIndex = 6;
			this.lbl_deviceType.Text = "门禁控制器类型";
			this.lbl_deviceType.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_syncTime.Location = new Point(10, 125);
			this.lbl_syncTime.Name = "lbl_syncTime";
			this.lbl_syncTime.Size = new Size(255, 13);
			this.lbl_syncTime.TabIndex = 10;
			this.lbl_syncTime.Text = "连接时同步设备时间";
			this.lbl_syncTime.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_area.Location = new Point(11, 152);
			this.lbl_area.Name = "lbl_area";
			this.lbl_area.Size = new Size(254, 13);
			this.lbl_area.TabIndex = 12;
			this.lbl_area.Text = "所属区域";
			this.lbl_area.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_deleteData.BackColor = Color.Transparent;
			this.lbl_deleteData.ForeColor = Color.Red;
			this.lbl_deleteData.Location = new Point(11, 179);
			this.lbl_deleteData.Name = "lbl_deleteData";
			this.lbl_deleteData.Size = new Size(255, 13);
			this.lbl_deleteData.TabIndex = 15;
			this.lbl_deleteData.Text = "新增时删除设备中数据";
			this.lbl_deleteData.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_deviceName.Location = new Point(368, 5);
			this.txt_deviceName.Name = "txt_deviceName";
			this.txt_deviceName.Size = new Size(186, 20);
			this.txt_deviceName.TabIndex = 1;
			this.txt_deviceName.KeyPress += this.txt_deviceName_KeyPress;
			this.rbtn_TCP.AutoSize = true;
			this.rbtn_TCP.Checked = true;
			this.rbtn_TCP.Location = new Point(368, 206);
			this.rbtn_TCP.Name = "rbtn_TCP";
			this.rbtn_TCP.Size = new Size(61, 17);
			this.rbtn_TCP.TabIndex = 18;
			this.rbtn_TCP.TabStop = true;
			this.rbtn_TCP.Text = "TCP/IP";
			this.rbtn_TCP.UseVisualStyleBackColor = true;
			this.rbtn_TCP.CheckedChanged += this.Rbtn_TCP_CheckedChanged;
			this.rbtn_RS485.AutoSize = true;
			this.rbtn_RS485.Location = new Point(474, 206);
			this.rbtn_RS485.Name = "rbtn_RS485";
			this.rbtn_RS485.Size = new Size(96, 17);
			this.rbtn_RS485.TabIndex = 19;
			this.rbtn_RS485.Text = "RS485/RS232";
			this.rbtn_RS485.UseVisualStyleBackColor = true;
			this.rbtn_RS485.CheckedChanged += this.Rbtn_RS485_CheckedChanged;
			this.txt_IPPort.Location = new Point(362, 36);
			this.txt_IPPort.Name = "txt_IPPort";
			this.txt_IPPort.Size = new Size(184, 20);
			this.txt_IPPort.TabIndex = 5;
			this.txt_IPPort.Text = "4370";
			this.txt_IPPort.KeyPress += this.txt_IPPort_KeyPress;
			this.txt_password.Location = new Point(393, 37);
			this.txt_password.Name = "txt_password";
			this.txt_password.PasswordChar = '*';
			this.txt_password.Size = new Size(161, 20);
			this.txt_password.TabIndex = 5;
			this.txt_password.KeyPress += this.txt_password_KeyPress;
			this.cbo_acpanelType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_acpanelType.FormattingEnabled = true;
			this.cbo_acpanelType.Items.AddRange(new object[3]
			{
				"Controladora de 01 porta",
				"Controladora de 02 portas",
				"Controladora de 04 portas"
			});
			this.cbo_acpanelType.Location = new Point(368, 67);
			this.cbo_acpanelType.Name = "cbo_acpanelType";
			this.cbo_acpanelType.Size = new Size(186, 21);
			this.cbo_acpanelType.TabIndex = 7;
			this.cbo_acpanelType.SelectedIndexChanged += this.cbo_doorType_SelectedIndexChanged;
			this.chk_syncTime.BackColor = Color.White;
			this.chk_syncTime.BackgroundStyle.Class = "";
			this.chk_syncTime.Checked = true;
			this.chk_syncTime.CheckState = CheckState.Checked;
			this.chk_syncTime.CheckValue = "Y";
			this.chk_syncTime.Location = new Point(368, 121);
			this.chk_syncTime.Name = "chk_syncTime";
			this.chk_syncTime.Size = new Size(20, 21);
			this.chk_syncTime.Style = eDotNetBarStyle.StyleManagerControlled;
			this.chk_syncTime.TabIndex = 11;
			this.chk_deleteData.BackColor = Color.White;
			this.chk_deleteData.BackgroundStyle.BackColor2 = Color.FromArgb(255, 0, 0);
			this.chk_deleteData.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBarBackground2;
			this.chk_deleteData.BackgroundStyle.BackgroundImagePosition = eStyleBackgroundImage.Center;
			this.chk_deleteData.BackgroundStyle.BorderColor = Color.FromArgb(255, 0, 0);
			this.chk_deleteData.BackgroundStyle.BorderTopColor = Color.FromArgb(255, 0, 0);
			this.chk_deleteData.BackgroundStyle.Class = "";
			this.chk_deleteData.Location = new Point(368, 177);
			this.chk_deleteData.Name = "chk_deleteData";
			this.chk_deleteData.Size = new Size(19, 21);
			this.chk_deleteData.Style = eDotNetBarStyle.StyleManagerControlled;
			this.chk_deleteData.TabIndex = 16;
			this.cbo_area.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_area.FormattingEnabled = true;
			this.cbo_area.Location = new Point(368, 148);
			this.cbo_area.Name = "cbo_area";
			this.cbo_area.Size = new Size(186, 21);
			this.cbo_area.TabIndex = 13;
			this.panel1.Controls.Add(this.txt_IP);
			this.panel1.Controls.Add(this.txt_Rs485Reader);
			this.panel1.Controls.Add(this.txt_IPPort);
			this.panel1.Controls.Add(this.lbl_BaudRateRange);
			this.panel1.Controls.Add(this.lbl_Rs485Reader);
			this.panel1.Controls.Add(this.lbl_IP);
			this.panel1.Controls.Add(this.lbl_star);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.lbl_IPPort);
			this.panel1.Location = new Point(7, 226);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(588, 92);
			this.panel1.TabIndex = 20;
			this.txt_IP.Location = new Point(362, 10);
			this.txt_IP.Name = "txt_IP";
			this.txt_IP.Size = new Size(184, 20);
			this.txt_IP.TabIndex = 1;
			this.txt_Rs485Reader.Enabled = false;
			this.txt_Rs485Reader.Location = new Point(362, 63);
			this.txt_Rs485Reader.Name = "txt_Rs485Reader";
			this.txt_Rs485Reader.Size = new Size(120, 20);
			this.txt_Rs485Reader.TabIndex = 8;
			this.txt_Rs485Reader.Text = "1";
			this.txt_Rs485Reader.KeyPress += this.txt_Rs485Reader_KeyPress;
			this.lbl_BaudRateRange.BackColor = Color.Transparent;
			this.lbl_BaudRateRange.ForeColor = SystemColors.HotTrack;
			this.lbl_BaudRateRange.Location = new Point(499, 67);
			this.lbl_BaudRateRange.Name = "lbl_BaudRateRange";
			this.lbl_BaudRateRange.Size = new Size(47, 13);
			this.lbl_BaudRateRange.TabIndex = 0;
			this.lbl_BaudRateRange.Text = "(1-255)";
			this.lbl_Rs485Reader.Location = new Point(5, 69);
			this.lbl_Rs485Reader.Name = "lbl_Rs485Reader";
			this.lbl_Rs485Reader.Size = new Size(193, 13);
			this.lbl_Rs485Reader.TabIndex = 7;
			this.lbl_Rs485Reader.Text = "RS485地址";
			this.lbl_Rs485Reader.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_star.AutoSize = true;
			this.lbl_star.ForeColor = Color.Red;
			this.lbl_star.Location = new Point(552, 41);
			this.lbl_star.Name = "lbl_star";
			this.lbl_star.Size = new Size(11, 13);
			this.lbl_star.TabIndex = 6;
			this.lbl_star.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(552, 12);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "*";
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(559, 9);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "*";
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(560, 152);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 13);
			this.label4.TabIndex = 14;
			this.label4.Text = "*";
			this.chk_fourToTwo.BackColor = Color.White;
			this.chk_fourToTwo.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBackground;
			this.chk_fourToTwo.BackgroundStyle.Class = "";
			this.chk_fourToTwo.Enabled = false;
			this.chk_fourToTwo.Location = new Point(368, 96);
			this.chk_fourToTwo.Name = "chk_fourToTwo";
			this.chk_fourToTwo.Size = new Size(19, 21);
			this.chk_fourToTwo.Style = eDotNetBarStyle.StyleManagerControlled;
			this.chk_fourToTwo.TabIndex = 9;
			this.lbl_four_to_two.BackColor = Color.Transparent;
			this.lbl_four_to_two.Location = new Point(10, 99);
			this.lbl_four_to_two.Name = "lbl_four_to_two";
			this.lbl_four_to_two.Size = new Size(255, 13);
			this.lbl_four_to_two.TabIndex = 8;
			this.lbl_four_to_two.Text = "切换为两门双向";
			this.lbl_four_to_two.TextAlign = ContentAlignment.MiddleLeft;
			this.time_close.Tick += this.time_close_Tick;
			this.pnl_commParams.Controls.Add(this.lbl_starIP);
			this.pnl_commParams.Controls.Add(this.label3);
			this.pnl_commParams.Controls.Add(this.txt_RS485);
			this.pnl_commParams.Controls.Add(this.lbl_startPort);
			this.pnl_commParams.Controls.Add(this.cbo_baudRate);
			this.pnl_commParams.Controls.Add(this.cbo_serialNo);
			this.pnl_commParams.Controls.Add(this.lbl_BaudRate);
			this.pnl_commParams.Controls.Add(this.lbl_RS485);
			this.pnl_commParams.Controls.Add(this.lbl_SerialNo);
			this.pnl_commParams.Location = new Point(7, 226);
			this.pnl_commParams.Name = "pnl_commParams";
			this.pnl_commParams.Size = new Size(588, 87);
			this.pnl_commParams.TabIndex = 55;
			this.lbl_starIP.AutoSize = true;
			this.lbl_starIP.ForeColor = Color.Red;
			this.lbl_starIP.Location = new Point(552, 41);
			this.lbl_starIP.Name = "lbl_starIP";
			this.lbl_starIP.Size = new Size(11, 13);
			this.lbl_starIP.TabIndex = 25;
			this.lbl_starIP.Text = "*";
			this.label3.AutoSize = true;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(552, 68);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 13);
			this.label3.TabIndex = 55;
			this.label3.Text = "*";
			this.txt_RS485.Location = new Point(361, 36);
			this.txt_RS485.MaxLength = 3;
			this.txt_RS485.Name = "txt_RS485";
			this.txt_RS485.Size = new Size(186, 20);
			this.txt_RS485.TabIndex = 10;
			this.txt_RS485.KeyPress += this.txt_RS485_KeyPress;
			this.lbl_startPort.AutoSize = true;
			this.lbl_startPort.ForeColor = Color.Red;
			this.lbl_startPort.Location = new Point(552, 12);
			this.lbl_startPort.Name = "lbl_startPort";
			this.lbl_startPort.Size = new Size(11, 13);
			this.lbl_startPort.TabIndex = 24;
			this.lbl_startPort.Text = "*";
			this.cbo_baudRate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_baudRate.FormattingEnabled = true;
			this.cbo_baudRate.Items.AddRange(new object[5]
			{
				"9600",
				"19200 ",
				"38400 ",
				"57600 ",
				"115200"
			});
			this.cbo_baudRate.Location = new Point(361, 63);
			this.cbo_baudRate.Name = "cbo_baudRate";
			this.cbo_baudRate.Size = new Size(186, 21);
			this.cbo_baudRate.TabIndex = 11;
			this.cbo_serialNo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_serialNo.FormattingEnabled = true;
			this.cbo_serialNo.Items.AddRange(new object[15]
			{
				"COM1",
				"COM2",
				"COM3",
				"COM4",
				"COM5",
				"COM6",
				"COM7",
				"COM8",
				"COM9",
				"COM10",
				"COM11",
				"COM12",
				"COM13",
				"COM14",
				"COM15"
			});
			this.cbo_serialNo.Location = new Point(361, 9);
			this.cbo_serialNo.Name = "cbo_serialNo";
			this.cbo_serialNo.Size = new Size(186, 21);
			this.cbo_serialNo.TabIndex = 9;
			this.lbl_BaudRate.Location = new Point(6, 69);
			this.lbl_BaudRate.Name = "lbl_BaudRate";
			this.lbl_BaudRate.Size = new Size(196, 13);
			this.lbl_BaudRate.TabIndex = 40;
			this.lbl_BaudRate.Text = "波特率";
			this.lbl_BaudRate.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_RS485.Location = new Point(5, 41);
			this.lbl_RS485.Name = "lbl_RS485";
			this.lbl_RS485.Size = new Size(213, 13);
			this.lbl_RS485.TabIndex = 39;
			this.lbl_RS485.Text = "RS485地址";
			this.lbl_RS485.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_SerialNo.Location = new Point(5, 14);
			this.lbl_SerialNo.Name = "lbl_SerialNo";
			this.lbl_SerialNo.Size = new Size(198, 13);
			this.lbl_SerialNo.TabIndex = 38;
			this.lbl_SerialNo.Text = "串口号";
			this.lbl_SerialNo.TextAlign = ContentAlignment.MiddleLeft;
			this.panel2.Controls.Add(this.btn_saveContinue);
			this.panel2.Controls.Add(this.btnTestConnect);
			this.panel2.Controls.Add(this.btn_cancel);
			this.panel2.Controls.Add(this.btn_OK);
			this.panel2.Location = new Point(7, 528);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(588, 26);
			this.panel2.TabIndex = 56;
			this.btn_saveContinue.AccessibleRole = AccessibleRole.PushButton;
			this.btn_saveContinue.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_saveContinue.Location = new Point(209, 1);
			this.btn_saveContinue.Name = "btn_saveContinue";
			this.btn_saveContinue.Size = new Size(154, 25);
			this.btn_saveContinue.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_saveContinue.TabIndex = 1;
			this.btn_saveContinue.Text = "保存并继续";
			this.btn_saveContinue.Click += this.btn_saveContinue_Click;
			this.btnTestConnect.AccessibleRole = AccessibleRole.PushButton;
			this.btnTestConnect.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnTestConnect.Location = new Point(3, 1);
			this.btnTestConnect.Name = "btnTestConnect";
			this.btnTestConnect.Size = new Size(181, 25);
			this.btnTestConnect.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnTestConnect.TabIndex = 0;
			this.btnTestConnect.Text = "测试连接";
			this.btnTestConnect.Click += this.btnTestConnect_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(481, 1);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(386, 1);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 25);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 2;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lbl_485master_slaves.Controls.Add(this.rdo_485_slave);
			this.lbl_485master_slaves.Controls.Add(this.rdo_485_master);
			this.lbl_485master_slaves.Controls.Add(this.lbl_option_seting);
			this.lbl_485master_slaves.Location = new Point(6, 321);
			this.lbl_485master_slaves.Name = "lbl_485master_slaves";
			this.lbl_485master_slaves.Size = new Size(589, 34);
			this.lbl_485master_slaves.TabIndex = 21;
			this.lbl_485master_slaves.TabStop = false;
			this.lbl_485master_slaves.Enter += this.lbl_485master_slaves_Enter;
			this.rdo_485_slave.BackColor = Color.Transparent;
			this.rdo_485_slave.BackgroundImageLayout = ImageLayout.None;
			this.rdo_485_slave.BackgroundStyle.BackColor2 = Color.FromArgb(255, 0, 0);
			this.rdo_485_slave.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBarBackground2;
			this.rdo_485_slave.BackgroundStyle.BackgroundImagePosition = eStyleBackgroundImage.Center;
			this.rdo_485_slave.BackgroundStyle.BorderColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_slave.BackgroundStyle.BorderTopColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_slave.BackgroundStyle.Class = "";
			this.rdo_485_slave.Cursor = Cursors.Default;
			this.rdo_485_slave.Enabled = false;
			this.rdo_485_slave.Location = new Point(472, 15);
			this.rdo_485_slave.Name = "rdo_485_slave";
			this.rdo_485_slave.Size = new Size(82, 18);
			this.rdo_485_slave.Style = eDotNetBarStyle.StyleManagerControlled;
			this.rdo_485_slave.TabIndex = 2;
			this.rdo_485_slave.Text = "从机";
			this.rdo_485_slave.Click += this.rdo_485_slave_Click;
			this.rdo_485_master.BackColor = Color.Transparent;
			this.rdo_485_master.BackgroundStyle.BackColor2 = Color.FromArgb(255, 0, 0);
			this.rdo_485_master.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBarBackground2;
			this.rdo_485_master.BackgroundStyle.BackgroundImagePosition = eStyleBackgroundImage.Center;
			this.rdo_485_master.BackgroundStyle.BorderColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_master.BackgroundStyle.BorderTopColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_master.BackgroundStyle.Class = "";
			this.rdo_485_master.Enabled = false;
			this.rdo_485_master.Location = new Point(359, 14);
			this.rdo_485_master.Name = "rdo_485_master";
			this.rdo_485_master.Size = new Size(69, 20);
			this.rdo_485_master.Style = eDotNetBarStyle.StyleManagerControlled;
			this.rdo_485_master.TabIndex = 1;
			this.rdo_485_master.Text = "主机";
			this.rdo_485_master.Click += this.rdo_485_master_Click;
			this.lbl_option_seting.Location = new Point(5, 16);
			this.lbl_option_seting.Name = "lbl_option_seting";
			this.lbl_option_seting.Size = new Size(180, 13);
			this.lbl_option_seting.TabIndex = 0;
			this.lbl_option_seting.Text = "RS485主从机配置";
			this.lbl_option_seting.TextAlign = ContentAlignment.MiddleLeft;
			this.groupBox1.Controls.Add(this.ckbRS485);
			this.groupBox1.Controls.Add(this.ckbRS232);
			this.groupBox1.Controls.Add(this.ckbTcpIp);
			this.groupBox1.Location = new Point(6, 21);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(589, 55);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "协议启用状态";
			this.ckbRS485.AutoSize = true;
			this.ckbRS485.Location = new Point(361, 23);
			this.ckbRS485.Name = "ckbRS485";
			this.ckbRS485.Size = new Size(59, 17);
			this.ckbRS485.TabIndex = 2;
			this.ckbRS485.Text = "RS485";
			this.ckbRS485.UseVisualStyleBackColor = true;
			this.ckbRS485.CheckedChanged += this.ckbRS485_CheckedChanged;
			this.ckbRS232.AutoSize = true;
			this.ckbRS232.Location = new Point(188, 23);
			this.ckbRS232.Name = "ckbRS232";
			this.ckbRS232.Size = new Size(59, 17);
			this.ckbRS232.TabIndex = 1;
			this.ckbRS232.Text = "RS232";
			this.ckbRS232.UseVisualStyleBackColor = true;
			this.ckbRS232.CheckedChanged += this.ckbRS232_CheckedChanged;
			this.ckbTcpIp.AutoSize = true;
			this.ckbTcpIp.Location = new Point(7, 23);
			this.ckbTcpIp.Name = "ckbTcpIp";
			this.ckbTcpIp.Size = new Size(62, 17);
			this.ckbTcpIp.TabIndex = 0;
			this.ckbTcpIp.Text = "TCP/IP";
			this.ckbTcpIp.UseVisualStyleBackColor = true;
			this.xtraTabControl1.AppearancePage.Header.BackColor = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.Header.BackColor2 = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.Header.Options.UseBackColor = true;
			this.xtraTabControl1.AppearancePage.HeaderActive.BackColor = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.HeaderActive.BackColor2 = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.HeaderActive.Options.UseBackColor = true;
			this.xtraTabControl1.AppearancePage.HeaderDisabled.BackColor = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.HeaderDisabled.BackColor2 = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.HeaderDisabled.Options.UseBackColor = true;
			this.xtraTabControl1.AppearancePage.HeaderHotTracked.BackColor = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.HeaderHotTracked.BackColor2 = Color.FromArgb(194, 217, 247);
			this.xtraTabControl1.AppearancePage.HeaderHotTracked.Options.UseBackColor = true;
			this.xtraTabControl1.Dock = DockStyle.Top;
			this.xtraTabControl1.Location = new Point(0, 0);
			this.xtraTabControl1.LookAndFeel.Style = LookAndFeelStyle.Office2003;
			this.xtraTabControl1.LookAndFeel.UseDefaultLookAndFeel = false;
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.tabBaseSetting;
			this.xtraTabControl1.Size = new Size(608, 522);
			this.xtraTabControl1.TabIndex = 0;
			this.xtraTabControl1.TabPages.AddRange(new XtraTabPage[3]
			{
				this.tabBaseSetting,
				this.tabVPSetting,
				this.tabDeviceControl
			});
			this.tabBaseSetting.Appearance.Header.BackColor = Color.FromArgb(194, 217, 247);
			this.tabBaseSetting.Appearance.Header.BackColor2 = Color.FromArgb(194, 217, 247);
			this.tabBaseSetting.Appearance.Header.Options.UseBackColor = true;
			this.tabBaseSetting.Appearance.PageClient.BackColor = Color.FromArgb(194, 217, 247);
			this.tabBaseSetting.Appearance.PageClient.BackColor2 = Color.FromArgb(194, 217, 247);
			this.tabBaseSetting.Appearance.PageClient.Options.UseBackColor = true;
			this.tabBaseSetting.Controls.Add(this.panel3);
			this.tabBaseSetting.Controls.Add(this.ckbInputPwd);
			this.tabBaseSetting.Controls.Add(this.rbtn_RS485);
			this.tabBaseSetting.Controls.Add(this.rbtn_TCP);
			this.tabBaseSetting.Controls.Add(this.chk_deleteData);
			this.tabBaseSetting.Controls.Add(this.cbo_area);
			this.tabBaseSetting.Controls.Add(this.chk_syncTime);
			this.tabBaseSetting.Controls.Add(this.chk_fourToTwo);
			this.tabBaseSetting.Controls.Add(this.cbo_acpanelType);
			this.tabBaseSetting.Controls.Add(this.txt_password);
			this.tabBaseSetting.Controls.Add(this.txt_deviceName);
			this.tabBaseSetting.Controls.Add(this.lbl_commType);
			this.tabBaseSetting.Controls.Add(this.lbl_deviceName);
			this.tabBaseSetting.Controls.Add(this.lbl_485master_slaves);
			this.tabBaseSetting.Controls.Add(this.lbl_deviceType);
			this.tabBaseSetting.Controls.Add(this.label4);
			this.tabBaseSetting.Controls.Add(this.lbl_password);
			this.tabBaseSetting.Controls.Add(this.lbl_area);
			this.tabBaseSetting.Controls.Add(this.lbl_deleteData);
			this.tabBaseSetting.Controls.Add(this.label1);
			this.tabBaseSetting.Controls.Add(this.lbl_syncTime);
			this.tabBaseSetting.Controls.Add(this.lbl_four_to_two);
			this.tabBaseSetting.Controls.Add(this.panel1);
			this.tabBaseSetting.Controls.Add(this.pnl_commParams);
			this.tabBaseSetting.Name = "tabBaseSetting";
			this.tabBaseSetting.Size = new Size(606, 498);
			this.tabBaseSetting.Text = "基本参数";
			this.panel3.Controls.Add(this.txtElevatorServerPort);
			this.panel3.Controls.Add(this.txtElevatorServerIP);
			this.panel3.Controls.Add(this.lblElevatorServerPort);
			this.panel3.Controls.Add(this.lblElevatorServerIp);
			this.panel3.Controls.Add(this.lblIdElevatorTerminal);
			this.panel3.Controls.Add(this.txtElevatorSourceFloor);
			this.panel3.Controls.Add(this.numElevatorTerminalId);
			this.panel3.Controls.Add(this.lblElevatorSourceFloor);
			this.panel3.Location = new Point(6, 361);
			this.panel3.Name = "panel3";
			this.panel3.Size = new Size(588, 134);
			this.panel3.TabIndex = 61;
			this.txtElevatorServerPort.Location = new Point(364, 91);
			this.txtElevatorServerPort.Name = "txtElevatorServerPort";
			this.txtElevatorServerPort.Size = new Size(119, 20);
			this.txtElevatorServerPort.TabIndex = 64;
			this.txtElevatorServerIP.Location = new Point(364, 65);
			this.txtElevatorServerIP.Name = "txtElevatorServerIP";
			this.txtElevatorServerIP.Size = new Size(120, 20);
			this.txtElevatorServerIP.TabIndex = 63;
			this.lblElevatorServerPort.AutoSize = true;
			this.lblElevatorServerPort.Location = new Point(7, 94);
			this.lblElevatorServerPort.Name = "lblElevatorServerPort";
			this.lblElevatorServerPort.Size = new Size(119, 13);
			this.lblElevatorServerPort.TabIndex = 62;
			this.lblElevatorServerPort.Text = "Porta Servidor Elevador";
			this.lblElevatorServerIp.AutoSize = true;
			this.lblElevatorServerIp.Location = new Point(6, 68);
			this.lblElevatorServerIp.Name = "lblElevatorServerIp";
			this.lblElevatorServerIp.Size = new Size(104, 13);
			this.lblElevatorServerIp.TabIndex = 61;
			this.lblElevatorServerIp.Text = "IP Servidor Elevador";
			this.lblIdElevatorTerminal.AutoSize = true;
			this.lblIdElevatorTerminal.Location = new Point(7, 15);
			this.lblIdElevatorTerminal.Name = "lblIdElevatorTerminal";
			this.lblIdElevatorTerminal.Size = new Size(63, 13);
			this.lblIdElevatorTerminal.TabIndex = 57;
			this.lblIdElevatorTerminal.Text = "ID Elevador";
			this.txtElevatorSourceFloor.Location = new Point(364, 39);
			this.txtElevatorSourceFloor.Name = "txtElevatorSourceFloor";
			this.txtElevatorSourceFloor.Size = new Size(120, 20);
			this.txtElevatorSourceFloor.TabIndex = 60;
			this.numElevatorTerminalId.Location = new Point(364, 13);
			this.numElevatorTerminalId.Maximum = new decimal(new int[4]
			{
				255,
				0,
				0,
				0
			});
			this.numElevatorTerminalId.Name = "numElevatorTerminalId";
			this.numElevatorTerminalId.Size = new Size(120, 20);
			this.numElevatorTerminalId.TabIndex = 58;
			this.lblElevatorSourceFloor.AutoSize = true;
			this.lblElevatorSourceFloor.Location = new Point(7, 42);
			this.lblElevatorSourceFloor.Name = "lblElevatorSourceFloor";
			this.lblElevatorSourceFloor.Size = new Size(80, 13);
			this.lblElevatorSourceFloor.TabIndex = 59;
			this.lblElevatorSourceFloor.Text = "Andar Elevador";
			this.ckbInputPwd.BackColor = Color.White;
			this.ckbInputPwd.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBackground;
			this.ckbInputPwd.BackgroundStyle.Class = "";
			this.ckbInputPwd.Checked = true;
			this.ckbInputPwd.CheckState = CheckState.Checked;
			this.ckbInputPwd.CheckValue = "Y";
			this.ckbInputPwd.Location = new Point(368, 38);
			this.ckbInputPwd.Name = "ckbInputPwd";
			this.ckbInputPwd.Size = new Size(19, 21);
			this.ckbInputPwd.Style = eDotNetBarStyle.StyleManagerControlled;
			this.ckbInputPwd.TabIndex = 4;
			this.ckbInputPwd.CheckedChanged += this.ckbInputPwd_CheckedChanged;
			this.tabVPSetting.Appearance.Header.BackColor = Color.FromArgb(194, 217, 247);
			this.tabVPSetting.Appearance.Header.BackColor2 = Color.FromArgb(194, 217, 247);
			this.tabVPSetting.Appearance.Header.Options.UseBackColor = true;
			this.tabVPSetting.Appearance.PageClient.BackColor = Color.FromArgb(194, 217, 247);
			this.tabVPSetting.Appearance.PageClient.BackColor2 = Color.FromArgb(194, 217, 247);
			this.tabVPSetting.Appearance.PageClient.Options.UseBackColor = true;
			this.tabVPSetting.Controls.Add(this.groupBox2);
			this.tabVPSetting.Controls.Add(this.groupBox1);
			this.tabVPSetting.Name = "tabVPSetting";
			this.tabVPSetting.PageVisible = false;
			this.tabVPSetting.Size = new Size(606, 498);
			this.tabVPSetting.Text = "验证及协议";
			this.groupBox2.Controls.Add(this.cbbCardMode);
			this.groupBox2.Controls.Add(this.ckbMifireMustRegistered);
			this.groupBox2.Controls.Add(this.ckbOnlyCheckCard);
			this.groupBox2.Controls.Add(this.ckbOnly1_1Mode);
			this.groupBox2.Controls.Add(this.cbbFace12N);
			this.groupBox2.Controls.Add(this.cbbFace121);
			this.groupBox2.Controls.Add(this.cbbFP12N);
			this.groupBox2.Controls.Add(this.cbbFP121);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Location = new Point(6, 112);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(589, 243);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "验证";
			this.cbbCardMode.FormattingEnabled = true;
			this.cbbCardMode.Location = new Point(355, 210);
			this.cbbCardMode.Name = "cbbCardMode";
			this.cbbCardMode.Size = new Size(161, 21);
			this.cbbCardMode.TabIndex = 15;
			this.cbbCardMode.Visible = false;
			this.ckbMifireMustRegistered.AutoSize = true;
			this.ckbMifireMustRegistered.Location = new Point(355, 185);
			this.ckbMifireMustRegistered.Name = "ckbMifireMustRegistered";
			this.ckbMifireMustRegistered.Size = new Size(15, 14);
			this.ckbMifireMustRegistered.TabIndex = 13;
			this.ckbMifireMustRegistered.UseVisualStyleBackColor = true;
			this.ckbOnlyCheckCard.AutoSize = true;
			this.ckbOnlyCheckCard.Location = new Point(355, 161);
			this.ckbOnlyCheckCard.Name = "ckbOnlyCheckCard";
			this.ckbOnlyCheckCard.Size = new Size(15, 14);
			this.ckbOnlyCheckCard.TabIndex = 11;
			this.ckbOnlyCheckCard.UseVisualStyleBackColor = true;
			this.ckbOnly1_1Mode.AutoSize = true;
			this.ckbOnly1_1Mode.Location = new Point(355, 138);
			this.ckbOnly1_1Mode.Name = "ckbOnly1_1Mode";
			this.ckbOnly1_1Mode.Size = new Size(15, 14);
			this.ckbOnly1_1Mode.TabIndex = 9;
			this.ckbOnly1_1Mode.UseVisualStyleBackColor = true;
			this.cbbFace12N.FormattingEnabled = true;
			this.cbbFace12N.Location = new Point(355, 108);
			this.cbbFace12N.Name = "cbbFace12N";
			this.cbbFace12N.Size = new Size(161, 21);
			this.cbbFace12N.TabIndex = 18;
			this.cbbFace121.FormattingEnabled = true;
			this.cbbFace121.Location = new Point(355, 79);
			this.cbbFace121.Name = "cbbFace121";
			this.cbbFace121.Size = new Size(161, 21);
			this.cbbFace121.TabIndex = 19;
			this.cbbFP12N.FormattingEnabled = true;
			this.cbbFP12N.Location = new Point(355, 50);
			this.cbbFP12N.Name = "cbbFP12N";
			this.cbbFP12N.Size = new Size(161, 21);
			this.cbbFP12N.TabIndex = 17;
			this.cbbFP121.FormattingEnabled = true;
			this.cbbFP121.Location = new Point(355, 21);
			this.cbbFP121.Name = "cbbFP121";
			this.cbbFP121.Size = new Size(161, 21);
			this.cbbFP121.TabIndex = 16;
			this.label12.AutoSize = true;
			this.label12.Location = new Point(7, 213);
			this.label12.Name = "label12";
			this.label12.Size = new Size(43, 13);
			this.label12.TabIndex = 14;
			this.label12.Text = "卡模式";
			this.label12.Visible = false;
			this.label11.AutoSize = true;
			this.label11.Location = new Point(7, 187);
			this.label11.Name = "label11";
			this.label11.Size = new Size(96, 13);
			this.label11.TabIndex = 12;
			this.label11.Text = "Mifare卡必须注册";
			this.label10.AutoSize = true;
			this.label10.Location = new Point(7, 164);
			this.label10.Name = "label10";
			this.label10.Size = new Size(79, 13);
			this.label10.TabIndex = 10;
			this.label10.Text = "只验证号码卡";
			this.label9.AutoSize = true;
			this.label9.Location = new Point(7, 140);
			this.label9.Name = "label9";
			this.label9.Size = new Size(70, 13);
			this.label9.TabIndex = 8;
			this.label9.Text = "只用1:1比对";
			this.label8.AutoSize = true;
			this.label8.Location = new Point(7, 113);
			this.label8.Name = "label8";
			this.label8.Size = new Size(96, 13);
			this.label8.TabIndex = 6;
			this.label8.Text = "人脸1:N匹配阈值";
			this.label7.AutoSize = true;
			this.label7.Location = new Point(7, 83);
			this.label7.Name = "label7";
			this.label7.Size = new Size(94, 13);
			this.label7.TabIndex = 4;
			this.label7.Text = "人脸1:1匹配阈值";
			this.label6.AutoSize = true;
			this.label6.Location = new Point(7, 54);
			this.label6.Name = "label6";
			this.label6.Size = new Size(96, 13);
			this.label6.TabIndex = 2;
			this.label6.Text = "指纹1:N匹配阈值";
			this.label5.AutoSize = true;
			this.label5.Location = new Point(7, 25);
			this.label5.Name = "label5";
			this.label5.Size = new Size(94, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "指纹1:1匹配阈值";
			this.tabDeviceControl.Appearance.Header.BackColor = Color.FromArgb(194, 217, 247);
			this.tabDeviceControl.Appearance.Header.BackColor2 = Color.FromArgb(194, 217, 247);
			this.tabDeviceControl.Appearance.Header.Options.UseBackColor = true;
			this.tabDeviceControl.Appearance.PageClient.BackColor = Color.FromArgb(194, 217, 247);
			this.tabDeviceControl.Appearance.PageClient.BackColor2 = Color.FromArgb(194, 217, 247);
			this.tabDeviceControl.Appearance.PageClient.Options.UseBackColor = true;
			this.tabDeviceControl.Controls.Add(this.ckbBatchUpdate);
			this.tabDeviceControl.Controls.Add(this.groupBox3);
			this.tabDeviceControl.Controls.Add(this.groupBox4);
			this.tabDeviceControl.Controls.Add(this.btnInitDeviceData);
			this.tabDeviceControl.Controls.Add(this.btnSetTime);
			this.tabDeviceControl.Controls.Add(this.btnClearAdmin);
			this.tabDeviceControl.Controls.Add(this.btnShutdown);
			this.tabDeviceControl.Controls.Add(this.btnClearVerifyRecord);
			this.tabDeviceControl.Controls.Add(this.btnReboot);
			this.tabDeviceControl.Name = "tabDeviceControl";
			this.tabDeviceControl.PageVisible = false;
			this.tabDeviceControl.Size = new Size(606, 498);
			this.tabDeviceControl.Text = "其它设置";
			this.ckbBatchUpdate.CheckAlign = ContentAlignment.MiddleRight;
			this.ckbBatchUpdate.Checked = true;
			this.ckbBatchUpdate.CheckState = CheckState.Checked;
			this.ckbBatchUpdate.Location = new Point(416, 105);
			this.ckbBatchUpdate.Name = "ckbBatchUpdate";
			this.ckbBatchUpdate.Size = new Size(154, 25);
			this.ckbBatchUpdate.TabIndex = 29;
			this.ckbBatchUpdate.Text = "启用批量上传";
			this.ckbBatchUpdate.UseVisualStyleBackColor = true;
			this.groupBox3.Controls.Add(this.cbbFreeType);
			this.groupBox3.Controls.Add(this.numFreeTime);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Location = new Point(12, 156);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(583, 57);
			this.groupBox3.TabIndex = 26;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "电源管理";
			this.cbbFreeType.FormattingEnabled = true;
			this.cbbFreeType.Location = new Point(157, 21);
			this.cbbFreeType.Name = "cbbFreeType";
			this.cbbFreeType.Size = new Size(96, 21);
			this.cbbFreeType.TabIndex = 23;
			this.numFreeTime.Location = new Point(434, 20);
			this.numFreeTime.Maximum = new decimal(new int[4]
			{
				999,
				0,
				0,
				0
			});
			this.numFreeTime.Name = "numFreeTime";
			this.numFreeTime.Size = new Size(43, 20);
			this.numFreeTime.TabIndex = 25;
			this.numFreeTime.Value = new decimal(new int[4]
			{
				50,
				0,
				0,
				0
			});
			this.label13.AutoSize = true;
			this.label13.Location = new Point(6, 25);
			this.label13.Name = "label13";
			this.label13.Size = new Size(55, 13);
			this.label13.TabIndex = 22;
			this.label13.Text = "空闲方式";
			this.label14.AutoSize = true;
			this.label14.Location = new Point(291, 25);
			this.label14.Name = "label14";
			this.label14.Size = new Size(73, 13);
			this.label14.TabIndex = 24;
			this.label14.Text = "空闲时间(分)";
			this.groupBox4.Controls.Add(this.cbbLanguage);
			this.groupBox4.Controls.Add(this.ckbKeypadVoice);
			this.groupBox4.Controls.Add(this.ckbVirifyVoice);
			this.groupBox4.Controls.Add(this.ckbTipsVoice);
			this.groupBox4.Controls.Add(this.numOpOverTime);
			this.groupBox4.Controls.Add(this.cbbVolume);
			this.groupBox4.Controls.Add(this.cbbFpVersion);
			this.groupBox4.Controls.Add(this.cbbDateFmt);
			this.groupBox4.Controls.Add(this.label22);
			this.groupBox4.Controls.Add(this.label21);
			this.groupBox4.Controls.Add(this.label19);
			this.groupBox4.Controls.Add(this.label20);
			this.groupBox4.Controls.Add(this.label17);
			this.groupBox4.Controls.Add(this.label18);
			this.groupBox4.Controls.Add(this.label16);
			this.groupBox4.Controls.Add(this.label15);
			this.groupBox4.Location = new Point(11, 226);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new Size(584, 140);
			this.groupBox4.TabIndex = 23;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "参数";
			this.cbbLanguage.FormattingEnabled = true;
			this.cbbLanguage.Location = new Point(435, 106);
			this.cbbLanguage.Name = "cbbLanguage";
			this.cbbLanguage.Size = new Size(96, 21);
			this.cbbLanguage.TabIndex = 7;
			this.cbbLanguage.Visible = false;
			this.ckbKeypadVoice.AutoSize = true;
			this.ckbKeypadVoice.Location = new Point(435, 80);
			this.ckbKeypadVoice.Name = "ckbKeypadVoice";
			this.ckbKeypadVoice.Size = new Size(15, 14);
			this.ckbKeypadVoice.TabIndex = 18;
			this.ckbKeypadVoice.UseVisualStyleBackColor = true;
			this.ckbVirifyVoice.AutoSize = true;
			this.ckbVirifyVoice.Location = new Point(435, 52);
			this.ckbVirifyVoice.Name = "ckbVirifyVoice";
			this.ckbVirifyVoice.Size = new Size(15, 14);
			this.ckbVirifyVoice.TabIndex = 17;
			this.ckbVirifyVoice.UseVisualStyleBackColor = true;
			this.ckbTipsVoice.AutoSize = true;
			this.ckbTipsVoice.Location = new Point(435, 24);
			this.ckbTipsVoice.Name = "ckbTipsVoice";
			this.ckbTipsVoice.Size = new Size(15, 14);
			this.ckbTipsVoice.TabIndex = 16;
			this.ckbTipsVoice.UseVisualStyleBackColor = true;
			this.numOpOverTime.Location = new Point(158, 105);
			this.numOpOverTime.Maximum = new decimal(new int[4]
			{
				1200,
				0,
				0,
				0
			});
			this.numOpOverTime.Name = "numOpOverTime";
			this.numOpOverTime.Size = new Size(43, 20);
			this.numOpOverTime.TabIndex = 21;
			this.numOpOverTime.Value = new decimal(new int[4]
			{
				50,
				0,
				0,
				0
			});
			this.numOpOverTime.Visible = false;
			this.cbbVolume.FormattingEnabled = true;
			this.cbbVolume.Location = new Point(158, 77);
			this.cbbVolume.Name = "cbbVolume";
			this.cbbVolume.Size = new Size(96, 21);
			this.cbbVolume.TabIndex = 22;
			this.cbbFpVersion.FormattingEnabled = true;
			this.cbbFpVersion.Location = new Point(158, 49);
			this.cbbFpVersion.Name = "cbbFpVersion";
			this.cbbFpVersion.Size = new Size(96, 21);
			this.cbbFpVersion.TabIndex = 3;
			this.cbbFpVersion.SelectedIndexChanged += this.cbbFpVersion_SelectedIndexChanged;
			this.cbbDateFmt.FormattingEnabled = true;
			this.cbbDateFmt.Location = new Point(158, 21);
			this.cbbDateFmt.Name = "cbbDateFmt";
			this.cbbDateFmt.Size = new Size(96, 21);
			this.cbbDateFmt.TabIndex = 1;
			this.label22.AutoSize = true;
			this.label22.Location = new Point(292, 81);
			this.label22.Name = "label22";
			this.label22.Size = new Size(43, 13);
			this.label22.TabIndex = 12;
			this.label22.Text = "键盘音";
			this.label21.AutoSize = true;
			this.label21.Location = new Point(7, 81);
			this.label21.Name = "label21";
			this.label21.Size = new Size(31, 13);
			this.label21.TabIndex = 14;
			this.label21.Text = "音量";
			this.label19.AutoSize = true;
			this.label19.Location = new Point(7, 109);
			this.label19.Name = "label19";
			this.label19.Size = new Size(79, 13);
			this.label19.TabIndex = 10;
			this.label19.Text = "操作超时时间";
			this.label19.Visible = false;
			this.label20.AutoSize = true;
			this.label20.Location = new Point(292, 53);
			this.label20.Name = "label20";
			this.label20.Size = new Size(55, 13);
			this.label20.TabIndex = 8;
			this.label20.Text = "验证语音";
			this.label17.AutoSize = true;
			this.label17.Location = new Point(292, 109);
			this.label17.Name = "label17";
			this.label17.Size = new Size(60, 13);
			this.label17.TabIndex = 6;
			this.label17.Text = "语音/语言";
			this.label17.Visible = false;
			this.label18.AutoSize = true;
			this.label18.Location = new Point(292, 25);
			this.label18.Name = "label18";
			this.label18.Size = new Size(55, 13);
			this.label18.TabIndex = 4;
			this.label18.Text = "语音提示";
			this.label16.AutoSize = true;
			this.label16.Location = new Point(7, 53);
			this.label16.Name = "label16";
			this.label16.Size = new Size(55, 13);
			this.label16.TabIndex = 2;
			this.label16.Text = "指纹版本";
			this.label15.AutoSize = true;
			this.label15.Location = new Point(7, 25);
			this.label15.Name = "label15";
			this.label15.Size = new Size(55, 13);
			this.label15.TabIndex = 0;
			this.label15.Text = "日期格式";
			this.btnInitDeviceData.AccessibleRole = AccessibleRole.PushButton;
			this.btnInitDeviceData.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnInitDeviceData.Location = new Point(58, 105);
			this.btnInitDeviceData.Name = "btnInitDeviceData";
			this.btnInitDeviceData.Size = new Size(154, 25);
			this.btnInitDeviceData.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnInitDeviceData.TabIndex = 22;
			this.btnInitDeviceData.Text = "初始化设备";
			this.btnInitDeviceData.Click += this.btnInitDeviceData_Click;
			this.btnSetTime.AccessibleRole = AccessibleRole.PushButton;
			this.btnSetTime.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnSetTime.Location = new Point(58, 62);
			this.btnSetTime.Name = "btnSetTime";
			this.btnSetTime.Size = new Size(154, 25);
			this.btnSetTime.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnSetTime.TabIndex = 21;
			this.btnSetTime.Text = "同步时间";
			this.btnSetTime.Click += this.btnSetTime_Click;
			this.btnClearAdmin.AccessibleRole = AccessibleRole.PushButton;
			this.btnClearAdmin.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnClearAdmin.Location = new Point(416, 18);
			this.btnClearAdmin.Name = "btnClearAdmin";
			this.btnClearAdmin.Size = new Size(154, 25);
			this.btnClearAdmin.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnClearAdmin.TabIndex = 20;
			this.btnClearAdmin.Text = "清除管理员权限";
			this.btnClearAdmin.Click += this.btnClearAdmin_Click;
			this.btnShutdown.AccessibleRole = AccessibleRole.PushButton;
			this.btnShutdown.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnShutdown.Location = new Point(228, 18);
			this.btnShutdown.Name = "btnShutdown";
			this.btnShutdown.Size = new Size(154, 25);
			this.btnShutdown.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnShutdown.TabIndex = 19;
			this.btnShutdown.Text = "关机";
			this.btnShutdown.Visible = false;
			this.btnShutdown.Click += this.btnShutdown_Click;
			this.btnClearVerifyRecord.AccessibleRole = AccessibleRole.PushButton;
			this.btnClearVerifyRecord.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnClearVerifyRecord.Location = new Point(228, 62);
			this.btnClearVerifyRecord.Name = "btnClearVerifyRecord";
			this.btnClearVerifyRecord.Size = new Size(154, 25);
			this.btnClearVerifyRecord.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnClearVerifyRecord.TabIndex = 18;
			this.btnClearVerifyRecord.Text = "清除验证记录";
			this.btnClearVerifyRecord.Visible = false;
			this.btnClearVerifyRecord.Click += this.btnClearVerifyRecord_Click;
			this.btnReboot.AccessibleRole = AccessibleRole.PushButton;
			this.btnReboot.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnReboot.Location = new Point(58, 18);
			this.btnReboot.Name = "btnReboot";
			this.btnReboot.Size = new Size(154, 25);
			this.btnReboot.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnReboot.TabIndex = 17;
			this.btnReboot.Text = "重启设备";
			this.btnReboot.Click += this.btnReboot_Click;
			this.errorProvider1.ContainerControl = this;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(608, 566);
			base.Controls.Add(this.xtraTabControl1);
			base.Controls.Add(this.panel2);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddDeviceForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "新增";
			base.FormClosing += this.AddDeviceForm_FormClosing;
			base.FormClosed += this.AddDeviceForm_FormClosed;
			base.Click += this.AddDeviceForm_Click;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.pnl_commParams.ResumeLayout(false);
			this.pnl_commParams.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.lbl_485master_slaves.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((ISupportInitialize)this.xtraTabControl1).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.tabBaseSetting.ResumeLayout(false);
			this.tabBaseSetting.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((ISupportInitialize)this.numElevatorTerminalId).EndInit();
			this.tabVPSetting.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabDeviceControl.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((ISupportInitialize)this.numFreeTime).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			((ISupportInitialize)this.numOpOverTime).EndInit();
			((ISupportInitialize)this.errorProvider1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
