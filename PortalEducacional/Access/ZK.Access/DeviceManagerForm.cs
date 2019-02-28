/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevComponents.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DeviceManagerForm : Office2007Form
	{
		private int m_ID;

		private MachinesBll machinesBll = new MachinesBll(MainForm._ia);

		private Machines machinesModel = null;

		private DeviceServerBll devServerBll = null;

		private int FOriBandrate = 0;

		private string FOriIP;

		private string FOriPasswd;

		private string FOriGateway;

		private string FOriNetmask;

		private string FOriFPThreadHold;

		private List<PersonnelArea> Arealist = null;

		private IContainer components = null;

		private DevComponents.DotNetBar.TabControl tab_devParameters;

		private TabControlPanel tabControlPanel1;

		private TabItem tabItem_comm;

		private TabControlPanel tabControlPanel2;

		private TabItem tabItem_accessControl;

		private ButtonX btn_setDevInfo;

		private ButtonX btn_getDevInfo;

		private GroupBox grb_commParam;

		private Label lab_deviceName;

		private TextBox txt_machineNumber;

		private Label lab_machineNumber;

		private ComboBox cbo_conStyle;

		private Label lab_connStyle;

		private TextBox txt_deviceName;

		private ButtonX btn_testConnection;

		private Label lab_comPasswd;

		private TextBox txt_passwd;

		private TextBox txt_ipPort;

		private Label lab_ipPort;

		private Label lab_ip;

		private IpAddressInput txt_IP;

		private Label lab_baudrate;

		private ComboBox cbo_baudRate;

		private ComboBox cbo_serialNo;

		private ButtonX btn_syncTime;

		private GroupBox grb_CommSet;

		private IpAddressInput txt_IPSet;

		private Label lab_passwdSet;

		private TextBox txt_passwdSet;

		private Label lab_IPSet;

		private Label lab_baudrateSet;

		private ComboBox cbo_baudRateSet;

		private ButtonX btn_upgradefireware;

		private ButtonX btn_restart;

		private Label lab_gatewaySet;

		private IpAddressInput txt_gatewaySet;

		private Label lab_subnetMask;

		private IpAddressInput txt_subnetMaskSet;

		private TabControlPanel tabControlPanel3;

		private GroupBox grp_devCapacity;

		private Label lab_logCapacityValue;

		private Label lab_logCapacity;

		private Label lab_FPCapacityValue;

		private Label lab_FPCapacity;

		private Label lab_userCapacityValue;

		private Label lab_userCapacity;

		private GroupBox grp_devInfo;

		private Label lab_manuDateValue;

		private Label lab_manuDate;

		private Label lab_logCountValue;

		private Label lab_logCount;

		private Label lab_productTypeValue;

		private Label lab_productType;

		private Label lab_serialNumberValue;

		private Label lab_serialNumber;

		private Label lab_frmwareVersionValue;

		private Label lab_frmwareVersion;

		private Label lab_fingerCountValue;

		private Label lab_fingerCount;

		private Label lab_userCountValue;

		private Label lab_userCount;

		private TabItem tabItem_deviceInfo;

		private TextBox txt_FPThreadHold;

		private Panel pnl_spliter;

		private Label lab_FPThreadHold;

		private Label lbl_valueRange;

		private ComboBox cbo_area;

		private Label lbl_area;

		public event EventHandler refreshDataEvent = null;

		public DeviceManagerForm(int machineid)
		{
			this.InitializeComponent();
			try
			{
				this.InitDefaultsValue();
				this.LoadAreaMsg();
				this.m_ID = machineid;
				this.BindData();
				this.tab_devParameters.SelectedTabIndex = 0;
				initLang.LocaleForm(this, base.Name);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadAreaMsg()
		{
			this.cbo_area.Items.Clear();
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

		private void BindData()
		{
			try
			{
				if (this.m_ID > 0)
				{
					this.machinesModel = this.machinesBll.GetModel(this.m_ID);
					if (this.machinesModel != null)
					{
						this.cbo_conStyle.SelectedIndex = this.machinesModel.ConnectType;
						this.txt_deviceName.Text = this.machinesModel.MachineAlias;
						TextBox textBox = this.txt_machineNumber;
						int num = this.machinesModel.MachineNumber;
						textBox.Text = num.ToString();
						this.txt_passwd.Text = this.machinesModel.CommPassword;
						this.txt_IP.Value = this.machinesModel.IP;
						TextBox textBox2 = this.txt_ipPort;
						num = this.machinesModel.Port;
						textBox2.Text = num.ToString();
						for (int i = 0; i < this.cbo_baudRate.Items.Count; i++)
						{
							string a = this.cbo_baudRate.Items[i].ToString();
							num = this.machinesModel.Baudrate;
							if (a == num.ToString())
							{
								this.cbo_baudRate.SelectedIndex = i;
								break;
							}
						}
						this.cbo_serialNo.SelectedIndex = this.machinesModel.SerialPort - 1;
						if (this.Arealist != null)
						{
							int num2 = 0;
							while (true)
							{
								if (num2 < this.Arealist.Count)
								{
									if (this.Arealist[num2].id != this.machinesModel.area_id)
									{
										num2++;
										continue;
									}
									break;
								}
								return;
							}
							this.cbo_area.SelectedIndex = num2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitDefaultsValue()
		{
			this.cbo_serialNo.Items.Clear();
			for (int i = 1; i < 255; i++)
			{
				this.cbo_serialNo.Items.Add("COM" + i);
			}
		}

		private void cbo_conStyle_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cbo_conStyle.SelectedIndex == 0)
			{
				this.lab_baudrate.Visible = true;
				this.cbo_baudRate.Visible = true;
				this.cbo_serialNo.Visible = true;
				this.lab_ip.Visible = false;
				this.txt_IP.Visible = false;
				this.txt_ipPort.Visible = false;
			}
			else if (this.cbo_conStyle.SelectedIndex == 1)
			{
				this.lab_baudrate.Visible = false;
				this.cbo_baudRate.Visible = false;
				this.cbo_serialNo.Visible = false;
				this.lab_ip.Visible = true;
				this.txt_IP.Visible = true;
				this.txt_ipPort.Visible = true;
			}
		}

		private void btn_syncTime_Click(object sender, EventArgs e)
		{
			try
			{
				this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
				if (this.devServerBll != null)
				{
					DeviceHelper.SyncDeviceTime(this.devServerBll);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_restart_Click(object sender, EventArgs e)
		{
			try
			{
				this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
				if (this.devServerBll != null)
				{
					this.devServerBll.RebootDevice();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_initDevice_Click(object sender, EventArgs e)
		{
			try
			{
				this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
				if (this.devServerBll != null)
				{
					if (this.machinesModel.IsOnlyRFMachine == 0)
					{
						DeviceHelper.DeleteDeviceData(this.devServerBll, true);
					}
					else
					{
						DeviceHelper.DeleteDeviceData(this.devServerBll, false);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetCommVisible(bool enable)
		{
			this.cbo_baudRateSet.Enabled = enable;
			this.txt_passwdSet.Enabled = enable;
			this.txt_IPSet.Enabled = enable;
			this.txt_gatewaySet.Enabled = enable;
			this.txt_subnetMaskSet.Enabled = enable;
			this.btn_setDevInfo.Enabled = enable;
		}

		private void txt_FPThreadHold_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 70L);
		}

		private void btn_getDevInfo_Click(object sender, EventArgs e)
		{
			try
			{
				this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
				if (this.devServerBll != null)
				{
					int num;
					if (this.tab_devParameters.SelectedTabIndex == 0)
					{
						this.SetCommVisible(true);
						for (int i = 0; i < this.cbo_baudRate.Items.Count; i++)
						{
							string a = this.cbo_baudRateSet.Items[i].ToString();
							num = this.machinesModel.Baudrate;
							if (a == num.ToString())
							{
								this.cbo_baudRateSet.SelectedIndex = i;
								break;
							}
						}
						this.FOriBandrate = this.cbo_baudRateSet.SelectedIndex;
						this.txt_passwdSet.Text = this.machinesModel.CommPassword;
						this.FOriPasswd = this.txt_passwdSet.Text;
						this.txt_IPSet.Value = this.machinesModel.IP;
						this.FOriIP = this.txt_IPSet.Value;
						this.txt_subnetMaskSet.Value = this.machinesModel.subnet_mask;
						this.FOriNetmask = this.txt_subnetMaskSet.Value;
						this.txt_gatewaySet.Value = this.machinesModel.gateway;
						this.FOriGateway = this.txt_gatewaySet.Value;
					}
					else if (this.tab_devParameters.SelectedTabIndex == 2)
					{
						TextBox textBox = this.txt_FPThreadHold;
						num = this.machinesModel.fp_mthreshold;
						textBox.Text = num.ToString();
						this.FOriFPThreadHold = this.txt_FPThreadHold.Text;
					}
					else if (this.tab_devParameters.SelectedTabIndex == 1)
					{
						int num2 = 0;
						int num3 = 0;
						int num4 = 0;
						Label label = this.lab_productTypeValue;
						num = this.machinesModel.ProduceKind;
						label.Text = num.ToString();
						this.lab_serialNumberValue.Text = this.machinesModel.sn;
						this.lab_frmwareVersionValue.Text = this.machinesModel.FirmwareVersion;
						if (this.machinesModel.max_user_count != 0)
						{
							Label label2 = this.lab_userCapacityValue;
							num = this.machinesModel.max_user_count;
							label2.Text = num.ToString();
						}
						if (this.machinesModel.max_finger_count != 0)
						{
							Label label3 = this.lab_FPCapacityValue;
							num = this.machinesModel.max_finger_count;
							label3.Text = num.ToString();
						}
						if (this.machinesModel.max_attlog_count != 0)
						{
							Label label4 = this.lab_logCapacityValue;
							num = this.machinesModel.max_attlog_count;
							label4.Text = num.ToString();
						}
						num2 = this.devServerBll.GetDeviceDataCount("user", "", "");
						if (this.machinesModel.IsOnlyRFMachine == 0)
						{
							num3 = this.devServerBll.GetDeviceDataCount("templatev10", "", "");
						}
						num4 = this.devServerBll.GetDeviceDataCount("transaction", "", "");
						if (num2 >= 0)
						{
							this.lab_userCountValue.Text = num2.ToString();
							this.machinesModel.usercount = (short)num2;
						}
						if (num3 >= 0)
						{
							this.lab_fingerCountValue.Text = num3.ToString();
							this.machinesModel.fingercount = (short)num3;
						}
						this.lab_logCountValue.Text = num4.ToString();
						this.machinesBll.Update(this.machinesModel);
					}
					else
					{
						MessageBox.Show("0");
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_setDevInfo_Click(object sender, EventArgs e)
		{
			try
			{
				bool flag = false;
				this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
				DeviceParamBll deviceParamBll = new DeviceParamBll(this.devServerBll.Application);
				int num;
				if (this.devServerBll != null)
				{
					if (this.tab_devParameters.SelectedTabIndex == 0)
					{
						if (this.FOriBandrate != this.cbo_baudRateSet.SelectedIndex)
						{
							int baudrate = int.Parse(this.cbo_baudRateSet.Text);
							deviceParamBll.SetBaudrate(baudrate);
							this.machinesModel.Baudrate = baudrate;
							flag = true;
						}
						if (this.FOriPasswd != this.txt_passwdSet.Text.Trim())
						{
							deviceParamBll.SetPasswd(this.txt_passwdSet.Text.Trim());
							this.machinesModel.CommPassword = this.txt_passwdSet.Text.Trim();
							flag = true;
						}
						if ((this.FOriIP != this.txt_IPSet.Value || this.FOriNetmask != this.txt_subnetMaskSet.Value || this.FOriGateway != this.txt_gatewaySet.Value) && this.txt_IPSet.Value != "0.0.0.0" && this.txt_subnetMaskSet.Value != "0.0.0.0")
						{
							num = ((this.txt_gatewaySet.Value != "0.0.0.0") ? 1 : 0);
							goto IL_0185;
						}
						num = 0;
						goto IL_0185;
					}
					if (this.tab_devParameters.SelectedTabIndex == 2 && this.FOriFPThreadHold != this.txt_FPThreadHold.Text)
					{
						if (int.Parse(this.txt_FPThreadHold.Text) >= 35 && int.Parse(this.txt_FPThreadHold.Text) <= 70)
						{
							if (deviceParamBll.SetFPThreshold(int.Parse(this.txt_FPThreadHold.Text)) >= 0)
							{
								this.machinesModel.fp_mthreshold = int.Parse(this.txt_FPThreadHold.Text);
							}
						}
						else
						{
							this.txt_FPThreadHold.Focus();
						}
					}
					goto IL_02d5;
				}
				goto end_IL_0001;
				IL_02d5:
				this.machinesBll.Update(this.machinesModel);
				string text = ShowMsgInfos.GetInfo("SOperationFinish", "操作完成");
				if (flag)
				{
					text = text + "," + ShowMsgInfos.GetInfo("SRestartDevice", "请重启设备");
				}
				SysDialogs.ShowInfoMessage(text);
				goto end_IL_0001;
				IL_0185:
				if (num != 0)
				{
					deviceParamBll.SetCommunication(this.txt_passwdSet.Text.Trim(), this.txt_IPSet.Value, this.txt_gatewaySet.Value, this.txt_subnetMaskSet.Value);
					this.machinesModel.IP = this.txt_IPSet.Value;
					this.machinesModel.subnet_mask = this.txt_subnetMaskSet.Value;
					this.machinesModel.gateway = this.txt_gatewaySet.Value;
					flag = true;
				}
				goto IL_02d5;
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_testConnection_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.cbo_area.SelectedIndex >= 0 && this.cbo_area.SelectedIndex <= this.Arealist.Count)
				{
					this.machinesModel.area_id = this.Arealist[this.cbo_area.SelectedIndex].id;
				}
				this.machinesModel.MachineAlias = this.txt_deviceName.Text;
				this.machinesBll.Update(this.machinesModel);
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
			}
			catch
			{
			}
		}

		private void tab_devParameters_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
		{
			if (this.tab_devParameters.SelectedTabIndex == 1)
			{
				this.btn_setDevInfo.Visible = false;
			}
			else
			{
				this.btn_setDevInfo.Visible = true;
			}
			this.btn_setDevInfo.Enabled = false;
		}

		private void btn_upgradefireware_Click(object sender, EventArgs e)
		{
			if (this.machinesModel != null)
			{
				List<int> list = new List<int>();
				list.Add(this.machinesModel.ID);
				UpgradeFirmvareForm upgradeFirmvareForm = new UpgradeFirmvareForm(list);
				upgradeFirmvareForm.ShowDialog();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DeviceManagerForm));
			this.tab_devParameters = new DevComponents.DotNetBar.TabControl();
			this.tabControlPanel1 = new TabControlPanel();
			this.grb_commParam = new GroupBox();
			this.cbo_area = new ComboBox();
			this.lbl_area = new Label();
			this.cbo_serialNo = new ComboBox();
			this.lab_baudrate = new Label();
			this.cbo_baudRate = new ComboBox();
			this.btn_testConnection = new ButtonX();
			this.lab_comPasswd = new Label();
			this.txt_passwd = new TextBox();
			this.txt_ipPort = new TextBox();
			this.lab_ipPort = new Label();
			this.lab_ip = new Label();
			this.txt_IP = new IpAddressInput();
			this.txt_machineNumber = new TextBox();
			this.lab_machineNumber = new Label();
			this.cbo_conStyle = new ComboBox();
			this.lab_connStyle = new Label();
			this.txt_deviceName = new TextBox();
			this.lab_deviceName = new Label();
			this.grb_CommSet = new GroupBox();
			this.lab_gatewaySet = new Label();
			this.txt_gatewaySet = new IpAddressInput();
			this.lab_subnetMask = new Label();
			this.txt_subnetMaskSet = new IpAddressInput();
			this.lab_baudrateSet = new Label();
			this.cbo_baudRateSet = new ComboBox();
			this.lab_IPSet = new Label();
			this.txt_IPSet = new IpAddressInput();
			this.lab_passwdSet = new Label();
			this.txt_passwdSet = new TextBox();
			this.tabItem_comm = new TabItem(this.components);
			this.tabControlPanel2 = new TabControlPanel();
			this.lbl_valueRange = new Label();
			this.lab_FPThreadHold = new Label();
			this.txt_FPThreadHold = new TextBox();
			this.pnl_spliter = new Panel();
			this.btn_upgradefireware = new ButtonX();
			this.btn_restart = new ButtonX();
			this.btn_syncTime = new ButtonX();
			this.tabItem_accessControl = new TabItem(this.components);
			this.tabControlPanel3 = new TabControlPanel();
			this.grp_devCapacity = new GroupBox();
			this.lab_logCapacityValue = new Label();
			this.lab_logCapacity = new Label();
			this.lab_FPCapacityValue = new Label();
			this.lab_FPCapacity = new Label();
			this.lab_userCapacityValue = new Label();
			this.lab_userCapacity = new Label();
			this.grp_devInfo = new GroupBox();
			this.lab_manuDateValue = new Label();
			this.lab_manuDate = new Label();
			this.lab_logCountValue = new Label();
			this.lab_logCount = new Label();
			this.lab_productTypeValue = new Label();
			this.lab_productType = new Label();
			this.lab_serialNumberValue = new Label();
			this.lab_serialNumber = new Label();
			this.lab_frmwareVersionValue = new Label();
			this.lab_frmwareVersion = new Label();
			this.lab_fingerCountValue = new Label();
			this.lab_fingerCount = new Label();
			this.lab_userCountValue = new Label();
			this.lab_userCount = new Label();
			this.tabItem_deviceInfo = new TabItem(this.components);
			this.btn_setDevInfo = new ButtonX();
			this.btn_getDevInfo = new ButtonX();
			((ISupportInitialize)this.tab_devParameters).BeginInit();
			this.tab_devParameters.SuspendLayout();
			this.tabControlPanel1.SuspendLayout();
			this.grb_commParam.SuspendLayout();
			((ISupportInitialize)this.txt_IP).BeginInit();
			this.grb_CommSet.SuspendLayout();
			((ISupportInitialize)this.txt_gatewaySet).BeginInit();
			((ISupportInitialize)this.txt_subnetMaskSet).BeginInit();
			((ISupportInitialize)this.txt_IPSet).BeginInit();
			this.tabControlPanel2.SuspendLayout();
			this.tabControlPanel3.SuspendLayout();
			this.grp_devCapacity.SuspendLayout();
			this.grp_devInfo.SuspendLayout();
			base.SuspendLayout();
			this.tab_devParameters.BackColor = Color.FromArgb(194, 217, 247);
			this.tab_devParameters.CanReorderTabs = true;
			this.tab_devParameters.Controls.Add(this.tabControlPanel1);
			this.tab_devParameters.Controls.Add(this.tabControlPanel2);
			this.tab_devParameters.Controls.Add(this.tabControlPanel3);
			this.tab_devParameters.Location = new Point(1, 2);
			this.tab_devParameters.Name = "tab_devParameters";
			this.tab_devParameters.SelectedTabFont = new Font("SimSun", 9f, FontStyle.Bold);
			this.tab_devParameters.SelectedTabIndex = 0;
			this.tab_devParameters.Size = new Size(584, 329);
			this.tab_devParameters.TabIndex = 0;
			this.tab_devParameters.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
			this.tab_devParameters.Tabs.Add(this.tabItem_comm);
			this.tab_devParameters.Tabs.Add(this.tabItem_deviceInfo);
			this.tab_devParameters.Tabs.Add(this.tabItem_accessControl);
			this.tab_devParameters.Text = "tabControl1";
			this.tab_devParameters.SelectedTabChanged += this.tab_devParameters_SelectedTabChanged;
			this.tabControlPanel1.Controls.Add(this.grb_commParam);
			this.tabControlPanel1.Controls.Add(this.grb_CommSet);
			this.tabControlPanel1.Dock = DockStyle.Fill;
			this.tabControlPanel1.Location = new Point(0, 26);
			this.tabControlPanel1.Name = "tabControlPanel1";
			this.tabControlPanel1.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel1.Size = new Size(584, 303);
			this.tabControlPanel1.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel1.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel1.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel1.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel1.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel1.Style.GradientAngle = 90;
			this.tabControlPanel1.TabIndex = 1;
			this.tabControlPanel1.TabItem = this.tabItem_comm;
			this.grb_commParam.BackColor = Color.FromArgb(194, 217, 247);
			this.grb_commParam.Controls.Add(this.cbo_serialNo);
			this.grb_commParam.Controls.Add(this.txt_machineNumber);
			this.grb_commParam.Controls.Add(this.cbo_area);
			this.grb_commParam.Controls.Add(this.txt_passwd);
			this.grb_commParam.Controls.Add(this.cbo_baudRate);
			this.grb_commParam.Controls.Add(this.cbo_conStyle);
			this.grb_commParam.Controls.Add(this.txt_deviceName);
			this.grb_commParam.Controls.Add(this.lbl_area);
			this.grb_commParam.Controls.Add(this.lab_baudrate);
			this.grb_commParam.Controls.Add(this.btn_testConnection);
			this.grb_commParam.Controls.Add(this.lab_comPasswd);
			this.grb_commParam.Controls.Add(this.txt_ipPort);
			this.grb_commParam.Controls.Add(this.lab_ipPort);
			this.grb_commParam.Controls.Add(this.lab_ip);
			this.grb_commParam.Controls.Add(this.txt_IP);
			this.grb_commParam.Controls.Add(this.lab_machineNumber);
			this.grb_commParam.Controls.Add(this.lab_connStyle);
			this.grb_commParam.Controls.Add(this.lab_deviceName);
			this.grb_commParam.Location = new Point(5, 0);
			this.grb_commParam.Name = "grb_commParam";
			this.grb_commParam.Size = new Size(575, 161);
			this.grb_commParam.TabIndex = 0;
			this.grb_commParam.TabStop = false;
			this.grb_commParam.Text = "连接参数";
			this.cbo_area.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_area.FormattingEnabled = true;
			this.cbo_area.Location = new Point(406, 20);
			this.cbo_area.Name = "cbo_area";
			this.cbo_area.Size = new Size(137, 20);
			this.cbo_area.TabIndex = 24;
			this.lbl_area.Location = new Point(322, 23);
			this.lbl_area.Name = "lbl_area";
			this.lbl_area.Size = new Size(76, 12);
			this.lbl_area.TabIndex = 25;
			this.lbl_area.Text = "所属区域";
			this.lbl_area.TextAlign = ContentAlignment.MiddleLeft;
			this.cbo_serialNo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_serialNo.Enabled = false;
			this.cbo_serialNo.FormattingEnabled = true;
			this.cbo_serialNo.Items.AddRange(new object[5]
			{
				"9600",
				"19200 ",
				"38400 ",
				"57600 ",
				"115200"
			});
			this.cbo_serialNo.Location = new Point(406, 86);
			this.cbo_serialNo.Name = "cbo_serialNo";
			this.cbo_serialNo.Size = new Size(137, 20);
			this.cbo_serialNo.TabIndex = 23;
			this.lab_baudrate.Location = new Point(18, 89);
			this.lab_baudrate.Name = "lab_baudrate";
			this.lab_baudrate.Size = new Size(126, 12);
			this.lab_baudrate.TabIndex = 22;
			this.lab_baudrate.Text = "波尔率";
			this.cbo_baudRate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_baudRate.Enabled = false;
			this.cbo_baudRate.FormattingEnabled = true;
			this.cbo_baudRate.Items.AddRange(new object[5]
			{
				"9600",
				"19200 ",
				"38400 ",
				"57600 ",
				"115200"
			});
			this.cbo_baudRate.Location = new Point(150, 85);
			this.cbo_baudRate.Name = "cbo_baudRate";
			this.cbo_baudRate.Size = new Size(137, 20);
			this.cbo_baudRate.TabIndex = 21;
			this.btn_testConnection.AccessibleRole = AccessibleRole.PushButton;
			this.btn_testConnection.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_testConnection.Location = new Point(406, 117);
			this.btn_testConnection.Name = "btn_testConnection";
			this.btn_testConnection.Size = new Size(137, 23);
			this.btn_testConnection.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_testConnection.TabIndex = 19;
			this.btn_testConnection.Text = "保存设置";
			this.btn_testConnection.Click += this.btn_testConnection_Click;
			this.lab_comPasswd.Location = new Point(18, 122);
			this.lab_comPasswd.Name = "lab_comPasswd";
			this.lab_comPasswd.Size = new Size(126, 12);
			this.lab_comPasswd.TabIndex = 16;
			this.lab_comPasswd.Text = "通讯密码";
			this.txt_passwd.Enabled = false;
			this.txt_passwd.Location = new Point(150, 117);
			this.txt_passwd.Name = "txt_passwd";
			this.txt_passwd.PasswordChar = '*';
			this.txt_passwd.Size = new Size(137, 21);
			this.txt_passwd.TabIndex = 15;
			this.txt_ipPort.Enabled = false;
			this.txt_ipPort.Location = new Point(406, 85);
			this.txt_ipPort.Name = "txt_ipPort";
			this.txt_ipPort.Size = new Size(137, 21);
			this.txt_ipPort.TabIndex = 14;
			this.lab_ipPort.Location = new Point(322, 89);
			this.lab_ipPort.Name = "lab_ipPort";
			this.lab_ipPort.Size = new Size(76, 12);
			this.lab_ipPort.TabIndex = 13;
			this.lab_ipPort.Text = "端口";
			this.lab_ip.Location = new Point(18, 89);
			this.lab_ip.Name = "lab_ip";
			this.lab_ip.Size = new Size(70, 12);
			this.lab_ip.TabIndex = 12;
			this.lab_ip.Text = "IP地址";
			this.txt_IP.AutoOverwrite = true;
			this.txt_IP.BackgroundStyle.Class = "DateTimeInputBackground";
			this.txt_IP.ButtonFreeText.Shortcut = eShortcut.F2;
			this.txt_IP.ButtonFreeText.Visible = true;
			this.txt_IP.Enabled = false;
			this.txt_IP.Location = new Point(150, 84);
			this.txt_IP.Name = "txt_IP";
			this.txt_IP.Size = new Size(137, 21);
			this.txt_IP.Style = eDotNetBarStyle.StyleManagerControlled;
			this.txt_IP.TabIndex = 11;
			this.txt_machineNumber.Enabled = false;
			this.txt_machineNumber.Location = new Point(406, 52);
			this.txt_machineNumber.Name = "txt_machineNumber";
			this.txt_machineNumber.Size = new Size(137, 21);
			this.txt_machineNumber.TabIndex = 5;
			this.lab_machineNumber.Location = new Point(322, 56);
			this.lab_machineNumber.Name = "lab_machineNumber";
			this.lab_machineNumber.Size = new Size(76, 12);
			this.lab_machineNumber.TabIndex = 4;
			this.lab_machineNumber.Text = "机器号";
			this.cbo_conStyle.Enabled = false;
			this.cbo_conStyle.FormattingEnabled = true;
			this.cbo_conStyle.Items.AddRange(new object[2]
			{
				"RS232/RS485",
				"TCP/IP"
			});
			this.cbo_conStyle.Location = new Point(150, 53);
			this.cbo_conStyle.Name = "cbo_conStyle";
			this.cbo_conStyle.Size = new Size(137, 20);
			this.cbo_conStyle.TabIndex = 3;
			this.cbo_conStyle.SelectedIndexChanged += this.cbo_conStyle_SelectedIndexChanged;
			this.lab_connStyle.Location = new Point(18, 56);
			this.lab_connStyle.Name = "lab_connStyle";
			this.lab_connStyle.Size = new Size(126, 12);
			this.lab_connStyle.TabIndex = 2;
			this.lab_connStyle.Text = "通讯方式";
			this.txt_deviceName.Location = new Point(150, 19);
			this.txt_deviceName.Name = "txt_deviceName";
			this.txt_deviceName.Size = new Size(137, 21);
			this.txt_deviceName.TabIndex = 1;
			this.lab_deviceName.Location = new Point(18, 23);
			this.lab_deviceName.Name = "lab_deviceName";
			this.lab_deviceName.Size = new Size(126, 12);
			this.lab_deviceName.TabIndex = 0;
			this.lab_deviceName.Text = "名称";
			this.grb_CommSet.BackColor = Color.FromArgb(194, 217, 247);
			this.grb_CommSet.Controls.Add(this.txt_subnetMaskSet);
			this.grb_CommSet.Controls.Add(this.txt_passwdSet);
			this.grb_CommSet.Controls.Add(this.txt_gatewaySet);
			this.grb_CommSet.Controls.Add(this.txt_IPSet);
			this.grb_CommSet.Controls.Add(this.cbo_baudRateSet);
			this.grb_CommSet.Controls.Add(this.lab_gatewaySet);
			this.grb_CommSet.Controls.Add(this.lab_subnetMask);
			this.grb_CommSet.Controls.Add(this.lab_baudrateSet);
			this.grb_CommSet.Controls.Add(this.lab_IPSet);
			this.grb_CommSet.Controls.Add(this.lab_passwdSet);
			this.grb_CommSet.Location = new Point(5, 175);
			this.grb_CommSet.Name = "grb_CommSet";
			this.grb_CommSet.Size = new Size(575, 124);
			this.grb_CommSet.TabIndex = 1;
			this.grb_CommSet.TabStop = false;
			this.grb_CommSet.Text = "通讯设置";
			this.lab_gatewaySet.Location = new Point(18, 99);
			this.lab_gatewaySet.Name = "lab_gatewaySet";
			this.lab_gatewaySet.Size = new Size(99, 12);
			this.lab_gatewaySet.TabIndex = 27;
			this.lab_gatewaySet.Text = "网关地址";
			this.txt_gatewaySet.AutoOverwrite = true;
			this.txt_gatewaySet.BackgroundStyle.Class = "DateTimeInputBackground";
			this.txt_gatewaySet.ButtonFreeText.Shortcut = eShortcut.F2;
			this.txt_gatewaySet.ButtonFreeText.Visible = true;
			this.txt_gatewaySet.Enabled = false;
			this.txt_gatewaySet.Location = new Point(150, 97);
			this.txt_gatewaySet.Name = "txt_gatewaySet";
			this.txt_gatewaySet.Size = new Size(137, 21);
			this.txt_gatewaySet.Style = eDotNetBarStyle.StyleManagerControlled;
			this.txt_gatewaySet.TabIndex = 26;
			this.txt_gatewaySet.Value = "0.0.0.0";
			this.lab_subnetMask.Location = new Point(322, 64);
			this.lab_subnetMask.Name = "lab_subnetMask";
			this.lab_subnetMask.Size = new Size(76, 12);
			this.lab_subnetMask.TabIndex = 25;
			this.lab_subnetMask.Text = "子网掩码";
			this.txt_subnetMaskSet.AutoOverwrite = true;
			this.txt_subnetMaskSet.BackgroundStyle.Class = "DateTimeInputBackground";
			this.txt_subnetMaskSet.ButtonFreeText.Shortcut = eShortcut.F2;
			this.txt_subnetMaskSet.ButtonFreeText.Visible = true;
			this.txt_subnetMaskSet.Enabled = false;
			this.txt_subnetMaskSet.Location = new Point(400, 61);
			this.txt_subnetMaskSet.Name = "txt_subnetMaskSet";
			this.txt_subnetMaskSet.Size = new Size(137, 21);
			this.txt_subnetMaskSet.Style = eDotNetBarStyle.StyleManagerControlled;
			this.txt_subnetMaskSet.TabIndex = 24;
			this.txt_subnetMaskSet.Value = "0.0.0.0";
			this.lab_baudrateSet.Location = new Point(18, 30);
			this.lab_baudrateSet.Name = "lab_baudrateSet";
			this.lab_baudrateSet.Size = new Size(126, 12);
			this.lab_baudrateSet.TabIndex = 23;
			this.lab_baudrateSet.Text = "波特率";
			this.cbo_baudRateSet.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_baudRateSet.Enabled = false;
			this.cbo_baudRateSet.FormattingEnabled = true;
			this.cbo_baudRateSet.Items.AddRange(new object[5]
			{
				"9600",
				"19200 ",
				"38400 ",
				"57600 ",
				"115200"
			});
			this.cbo_baudRateSet.Location = new Point(150, 27);
			this.cbo_baudRateSet.Name = "cbo_baudRateSet";
			this.cbo_baudRateSet.Size = new Size(137, 20);
			this.cbo_baudRateSet.TabIndex = 22;
			this.lab_IPSet.Location = new Point(18, 64);
			this.lab_IPSet.Name = "lab_IPSet";
			this.lab_IPSet.Size = new Size(126, 12);
			this.lab_IPSet.TabIndex = 19;
			this.lab_IPSet.Text = "IP地址";
			this.txt_IPSet.AutoOverwrite = true;
			this.txt_IPSet.BackgroundStyle.Class = "DateTimeInputBackground";
			this.txt_IPSet.ButtonFreeText.Shortcut = eShortcut.F2;
			this.txt_IPSet.ButtonFreeText.Visible = true;
			this.txt_IPSet.Enabled = false;
			this.txt_IPSet.Location = new Point(150, 61);
			this.txt_IPSet.Name = "txt_IPSet";
			this.txt_IPSet.Size = new Size(137, 21);
			this.txt_IPSet.Style = eDotNetBarStyle.StyleManagerControlled;
			this.txt_IPSet.TabIndex = 18;
			this.txt_IPSet.Value = "0.0.0.0";
			this.lab_passwdSet.Location = new Point(322, 26);
			this.lab_passwdSet.Name = "lab_passwdSet";
			this.lab_passwdSet.Size = new Size(76, 12);
			this.lab_passwdSet.TabIndex = 17;
			this.lab_passwdSet.Text = "通讯密码";
			this.txt_passwdSet.Enabled = false;
			this.txt_passwdSet.Location = new Point(400, 26);
			this.txt_passwdSet.Name = "txt_passwdSet";
			this.txt_passwdSet.PasswordChar = '*';
			this.txt_passwdSet.Size = new Size(137, 21);
			this.txt_passwdSet.TabIndex = 2;
			this.tabItem_comm.AttachedControl = this.tabControlPanel1;
			this.tabItem_comm.Name = "tabItem_comm";
			this.tabItem_comm.Text = "通讯设置";
			this.tabControlPanel2.Controls.Add(this.txt_FPThreadHold);
			this.tabControlPanel2.Controls.Add(this.lbl_valueRange);
			this.tabControlPanel2.Controls.Add(this.lab_FPThreadHold);
			this.tabControlPanel2.Controls.Add(this.pnl_spliter);
			this.tabControlPanel2.Controls.Add(this.btn_upgradefireware);
			this.tabControlPanel2.Controls.Add(this.btn_restart);
			this.tabControlPanel2.Controls.Add(this.btn_syncTime);
			this.tabControlPanel2.Dock = DockStyle.Fill;
			this.tabControlPanel2.Location = new Point(0, 26);
			this.tabControlPanel2.Name = "tabControlPanel2";
			this.tabControlPanel2.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel2.Size = new Size(584, 303);
			this.tabControlPanel2.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel2.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel2.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel2.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel2.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel2.Style.GradientAngle = 90;
			this.tabControlPanel2.TabIndex = 2;
			this.tabControlPanel2.TabItem = this.tabItem_accessControl;
			this.lbl_valueRange.BackColor = Color.Transparent;
			this.lbl_valueRange.Location = new Point(372, 173);
			this.lbl_valueRange.Name = "lbl_valueRange";
			this.lbl_valueRange.Size = new Size(126, 12);
			this.lbl_valueRange.TabIndex = 40;
			this.lbl_valueRange.Text = "(范围35-70)";
			this.lab_FPThreadHold.BackColor = Color.Transparent;
			this.lab_FPThreadHold.Location = new Point(20, 173);
			this.lab_FPThreadHold.Name = "lab_FPThreadHold";
			this.lab_FPThreadHold.Size = new Size(189, 12);
			this.lab_FPThreadHold.TabIndex = 27;
			this.lab_FPThreadHold.Text = "指纹比对阀值";
			this.lab_FPThreadHold.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_FPThreadHold.Location = new Point(215, 170);
			this.txt_FPThreadHold.Name = "txt_FPThreadHold";
			this.txt_FPThreadHold.Size = new Size(137, 21);
			this.txt_FPThreadHold.TabIndex = 26;
			this.txt_FPThreadHold.KeyPress += this.txt_FPThreadHold_KeyPress;
			this.pnl_spliter.Location = new Point(4, 156);
			this.pnl_spliter.Name = "pnl_spliter";
			this.pnl_spliter.Size = new Size(573, 2);
			this.pnl_spliter.TabIndex = 24;
			this.btn_upgradefireware.AccessibleRole = AccessibleRole.PushButton;
			this.btn_upgradefireware.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_upgradefireware.Location = new Point(30, 71);
			this.btn_upgradefireware.Name = "btn_upgradefireware";
			this.btn_upgradefireware.Size = new Size(109, 23);
			this.btn_upgradefireware.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_upgradefireware.TabIndex = 22;
			this.btn_upgradefireware.Text = "升级固件";
			this.btn_upgradefireware.Click += this.btn_upgradefireware_Click;
			this.btn_restart.AccessibleRole = AccessibleRole.PushButton;
			this.btn_restart.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_restart.Location = new Point(158, 26);
			this.btn_restart.Name = "btn_restart";
			this.btn_restart.Size = new Size(109, 23);
			this.btn_restart.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_restart.TabIndex = 21;
			this.btn_restart.Text = "重启设备";
			this.btn_restart.Click += this.btn_restart_Click;
			this.btn_syncTime.AccessibleRole = AccessibleRole.PushButton;
			this.btn_syncTime.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_syncTime.Location = new Point(30, 26);
			this.btn_syncTime.Name = "btn_syncTime";
			this.btn_syncTime.Size = new Size(109, 23);
			this.btn_syncTime.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_syncTime.TabIndex = 20;
			this.btn_syncTime.Text = "同步时间";
			this.btn_syncTime.Click += this.btn_syncTime_Click;
			this.tabItem_accessControl.AttachedControl = this.tabControlPanel2;
			this.tabItem_accessControl.Name = "tabItem_accessControl";
			this.tabItem_accessControl.Text = "门禁";
			this.tabControlPanel3.Controls.Add(this.grp_devCapacity);
			this.tabControlPanel3.Controls.Add(this.grp_devInfo);
			this.tabControlPanel3.Dock = DockStyle.Fill;
			this.tabControlPanel3.Location = new Point(0, 26);
			this.tabControlPanel3.Name = "tabControlPanel3";
			this.tabControlPanel3.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel3.Size = new Size(584, 303);
			this.tabControlPanel3.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel3.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel3.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel3.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel3.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel3.Style.GradientAngle = 90;
			this.tabControlPanel3.TabIndex = 0;
			this.tabControlPanel3.TabItem = this.tabItem_deviceInfo;
			this.grp_devCapacity.BackColor = Color.FromArgb(194, 217, 247);
			this.grp_devCapacity.Controls.Add(this.lab_logCapacityValue);
			this.grp_devCapacity.Controls.Add(this.lab_logCapacity);
			this.grp_devCapacity.Controls.Add(this.lab_FPCapacityValue);
			this.grp_devCapacity.Controls.Add(this.lab_FPCapacity);
			this.grp_devCapacity.Controls.Add(this.lab_userCapacityValue);
			this.grp_devCapacity.Controls.Add(this.lab_userCapacity);
			this.grp_devCapacity.Location = new Point(5, 194);
			this.grp_devCapacity.Name = "grp_devCapacity";
			this.grp_devCapacity.Size = new Size(575, 105);
			this.grp_devCapacity.TabIndex = 2;
			this.grp_devCapacity.TabStop = false;
			this.grp_devCapacity.Text = "容量信息";
			this.lab_logCapacityValue.Location = new Point(430, 33);
			this.lab_logCapacityValue.Name = "lab_logCapacityValue";
			this.lab_logCapacityValue.Size = new Size(73, 12);
			this.lab_logCapacityValue.TabIndex = 39;
			this.lab_logCapacity.Location = new Point(306, 33);
			this.lab_logCapacity.Name = "lab_logCapacity";
			this.lab_logCapacity.Size = new Size(126, 12);
			this.lab_logCapacity.TabIndex = 38;
			this.lab_logCapacity.Text = "记录总数";
			this.lab_FPCapacityValue.Location = new Point(151, 73);
			this.lab_FPCapacityValue.Name = "lab_FPCapacityValue";
			this.lab_FPCapacityValue.Size = new Size(59, 12);
			this.lab_FPCapacityValue.TabIndex = 31;
			this.lab_FPCapacity.Location = new Point(18, 73);
			this.lab_FPCapacity.Name = "lab_FPCapacity";
			this.lab_FPCapacity.Size = new Size(107, 12);
			this.lab_FPCapacity.TabIndex = 30;
			this.lab_FPCapacity.Text = "指纹总数";
			this.lab_userCapacityValue.Location = new Point(140, 33);
			this.lab_userCapacityValue.Name = "lab_userCapacityValue";
			this.lab_userCapacityValue.Size = new Size(76, 12);
			this.lab_userCapacityValue.TabIndex = 29;
			this.lab_userCapacity.Location = new Point(18, 33);
			this.lab_userCapacity.Name = "lab_userCapacity";
			this.lab_userCapacity.Size = new Size(107, 12);
			this.lab_userCapacity.TabIndex = 28;
			this.lab_userCapacity.Text = "人员总数";
			this.grp_devInfo.BackColor = Color.FromArgb(194, 217, 247);
			this.grp_devInfo.Controls.Add(this.lab_manuDateValue);
			this.grp_devInfo.Controls.Add(this.lab_manuDate);
			this.grp_devInfo.Controls.Add(this.lab_logCountValue);
			this.grp_devInfo.Controls.Add(this.lab_logCount);
			this.grp_devInfo.Controls.Add(this.lab_productTypeValue);
			this.grp_devInfo.Controls.Add(this.lab_productType);
			this.grp_devInfo.Controls.Add(this.lab_serialNumberValue);
			this.grp_devInfo.Controls.Add(this.lab_serialNumber);
			this.grp_devInfo.Controls.Add(this.lab_frmwareVersionValue);
			this.grp_devInfo.Controls.Add(this.lab_frmwareVersion);
			this.grp_devInfo.Controls.Add(this.lab_fingerCountValue);
			this.grp_devInfo.Controls.Add(this.lab_fingerCount);
			this.grp_devInfo.Controls.Add(this.lab_userCountValue);
			this.grp_devInfo.Controls.Add(this.lab_userCount);
			this.grp_devInfo.Location = new Point(4, 4);
			this.grp_devInfo.Name = "grp_devInfo";
			this.grp_devInfo.Size = new Size(575, 172);
			this.grp_devInfo.TabIndex = 1;
			this.grp_devInfo.TabStop = false;
			this.grp_devInfo.Text = "信息";
			this.lab_manuDateValue.Location = new Point(141, 138);
			this.lab_manuDateValue.Name = "lab_manuDateValue";
			this.lab_manuDateValue.Size = new Size(103, 12);
			this.lab_manuDateValue.TabIndex = 39;
			this.lab_manuDate.Location = new Point(18, 138);
			this.lab_manuDate.Name = "lab_manuDate";
			this.lab_manuDate.Size = new Size(107, 12);
			this.lab_manuDate.TabIndex = 38;
			this.lab_manuDate.Text = "出厂日期";
			this.lab_logCountValue.Location = new Point(433, 30);
			this.lab_logCountValue.Name = "lab_logCountValue";
			this.lab_logCountValue.Size = new Size(136, 12);
			this.lab_logCountValue.TabIndex = 37;
			this.lab_logCountValue.Text = "0";
			this.lab_logCount.Location = new Point(307, 30);
			this.lab_logCount.Name = "lab_logCount";
			this.lab_logCount.Size = new Size(126, 12);
			this.lab_logCount.TabIndex = 36;
			this.lab_logCount.Text = "记录总数";
			this.lab_productTypeValue.Location = new Point(435, 65);
			this.lab_productTypeValue.Name = "lab_productTypeValue";
			this.lab_productTypeValue.Size = new Size(132, 12);
			this.lab_productTypeValue.TabIndex = 35;
			this.lab_productType.Location = new Point(307, 66);
			this.lab_productType.Name = "lab_productType";
			this.lab_productType.Size = new Size(126, 12);
			this.lab_productType.TabIndex = 34;
			this.lab_productType.Text = "产品型号";
			this.lab_serialNumberValue.Location = new Point(435, 102);
			this.lab_serialNumberValue.Name = "lab_serialNumberValue";
			this.lab_serialNumberValue.Size = new Size(134, 12);
			this.lab_serialNumberValue.TabIndex = 33;
			this.lab_serialNumber.Location = new Point(307, 102);
			this.lab_serialNumber.Name = "lab_serialNumber";
			this.lab_serialNumber.Size = new Size(126, 12);
			this.lab_serialNumber.TabIndex = 32;
			this.lab_serialNumber.Text = "序列号";
			this.lab_frmwareVersionValue.Location = new Point(141, 102);
			this.lab_frmwareVersionValue.Name = "lab_frmwareVersionValue";
			this.lab_frmwareVersionValue.Size = new Size(169, 12);
			this.lab_frmwareVersionValue.TabIndex = 31;
			this.lab_frmwareVersion.Location = new Point(18, 102);
			this.lab_frmwareVersion.Name = "lab_frmwareVersion";
			this.lab_frmwareVersion.Size = new Size(107, 12);
			this.lab_frmwareVersion.TabIndex = 30;
			this.lab_frmwareVersion.Text = "固件版本";
			this.lab_fingerCountValue.Location = new Point(141, 65);
			this.lab_fingerCountValue.Name = "lab_fingerCountValue";
			this.lab_fingerCountValue.Size = new Size(76, 12);
			this.lab_fingerCountValue.TabIndex = 29;
			this.lab_fingerCountValue.Text = "0";
			this.lab_fingerCount.Location = new Point(18, 66);
			this.lab_fingerCount.Name = "lab_fingerCount";
			this.lab_fingerCount.Size = new Size(107, 12);
			this.lab_fingerCount.TabIndex = 28;
			this.lab_fingerCount.Text = "指纹总数";
			this.lab_userCountValue.Location = new Point(141, 30);
			this.lab_userCountValue.Name = "lab_userCountValue";
			this.lab_userCountValue.Size = new Size(77, 12);
			this.lab_userCountValue.TabIndex = 27;
			this.lab_userCountValue.Text = "0";
			this.lab_userCount.Location = new Point(18, 30);
			this.lab_userCount.Name = "lab_userCount";
			this.lab_userCount.Size = new Size(99, 12);
			this.lab_userCount.TabIndex = 26;
			this.lab_userCount.Text = "人员总数";
			this.tabItem_deviceInfo.AttachedControl = this.tabControlPanel3;
			this.tabItem_deviceInfo.Name = "tabItem_deviceInfo";
			this.tabItem_deviceInfo.Text = "设备信息";
			this.btn_setDevInfo.AccessibleRole = AccessibleRole.PushButton;
			this.btn_setDevInfo.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_setDevInfo.Enabled = false;
			this.btn_setDevInfo.Location = new Point(460, 342);
			this.btn_setDevInfo.Name = "btn_setDevInfo";
			this.btn_setDevInfo.Size = new Size(120, 23);
			this.btn_setDevInfo.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_setDevInfo.TabIndex = 18;
			this.btn_setDevInfo.Text = "应用设置";
			this.btn_setDevInfo.Click += this.btn_setDevInfo_Click;
			this.btn_getDevInfo.AccessibleRole = AccessibleRole.PushButton;
			this.btn_getDevInfo.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_getDevInfo.Location = new Point(320, 342);
			this.btn_getDevInfo.Name = "btn_getDevInfo";
			this.btn_getDevInfo.Size = new Size(120, 23);
			this.btn_getDevInfo.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_getDevInfo.TabIndex = 17;
			this.btn_getDevInfo.Text = "读取设置";
			this.btn_getDevInfo.Click += this.btn_getDevInfo_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(586, 375);
			base.Controls.Add(this.tab_devParameters);
			base.Controls.Add(this.btn_getDevInfo);
			base.Controls.Add(this.btn_setDevInfo);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DeviceManagerForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "设备管理";
			((ISupportInitialize)this.tab_devParameters).EndInit();
			this.tab_devParameters.ResumeLayout(false);
			this.tabControlPanel1.ResumeLayout(false);
			this.grb_commParam.ResumeLayout(false);
			this.grb_commParam.PerformLayout();
			((ISupportInitialize)this.txt_IP).EndInit();
			this.grb_CommSet.ResumeLayout(false);
			this.grb_CommSet.PerformLayout();
			((ISupportInitialize)this.txt_gatewaySet).EndInit();
			((ISupportInitialize)this.txt_subnetMaskSet).EndInit();
			((ISupportInitialize)this.txt_IPSet).EndInit();
			this.tabControlPanel2.ResumeLayout(false);
			this.tabControlPanel2.PerformLayout();
			this.tabControlPanel3.ResumeLayout(false);
			this.grp_devCapacity.ResumeLayout(false);
			this.grp_devInfo.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
