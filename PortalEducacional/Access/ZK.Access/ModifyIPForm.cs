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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class ModifyIPForm : Office2007Form
	{
		private string MAC;

		private int type = 0;

		private string password;

		private int machineID;

		public bool ModifyIPSuccess = false;

		public string newIP;

		public string newNetMask;

		public string newGateway;

		private int newPort;

		private Dictionary<int, string> SDKErrorList = new Dictionary<int, string>();

		private IContainer components = null;

		private Label lab_originalIP;

		private Label lab_newIP;

		private Label lab_subnetMask;

		private Label lab_Gateway;

		private IpAddressInput ipInput_subnetMask;

		private IpAddressInput ipInput_Gateway;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		private Label label4;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label lblPort;

		private NumericUpDown numPort;

		private TextBox ipInput_originalIP;

		private TextBox ipInput_newIP;

		public event EventHandler refreshDataEvent = null;

		public ModifyIPForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public ModifyIPForm(string ip, string mac, string netMask, string gateIPAddress)
			: this()
		{
			this.ipInput_originalIP.Text = ip;
			this.MAC = mac;
			this.ipInput_newIP.Text = ip;
			this.ipInput_subnetMask.Value = netMask;
			this.ipInput_Gateway.Value = gateIPAddress;
			this.ipInput_originalIP.ReadOnly = true;
		}

		public ModifyIPForm(int machineID, int Type, string ip, string Password)
			: this()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			Machines model = machinesBll.GetModel(machineID);
			this.machineID = machineID;
			this.type = Type;
			this.ipInput_originalIP.Text = ip;
			this.ipInput_newIP.Text = ip;
			this.ipInput_subnetMask.Value = model.subnet_mask;
			this.ipInput_Gateway.Value = model.gateway;
			this.password = Password;
			if (model != null)
			{
				this.numPort.Value = model.Port;
				this.numPort.Enabled = (model.DevSDKType != SDKType.StandaloneSDK);
			}
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private bool CheckInputValue()
		{
			if (this.ipInput_originalIP.Text == null || this.ipInput_newIP.Text == null || this.ipInput_Gateway.Value == null || this.ipInput_subnetMask.Value == null)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputValue", "请输入数据"));
				return false;
			}
			if (this.ipInput_subnetMask.Value.Replace(".", "") == "0000")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMaskError", "子网俺码错误"));
				this.ipInput_subnetMask.Focus();
				return false;
			}
			if (this.ipInput_subnetMask.Value == "255.255.255.255")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMaskError", "子网俺码错误"));
				this.ipInput_subnetMask.Focus();
				return false;
			}
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList($"IP='{this.ipInput_newIP.Text}' and port={this.numPort.Value}");
			if (modelList != null && modelList.Count > 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("IPAddressExists", "指定的IP地址已存在"));
				this.ipInput_newIP.Focus();
				return false;
			}
			return true;
		}

		[DllImport("plcommpro.dll")]
		public static extern int ModifyIPAddress(string commtype, string address, string buffer);

		private void btn_ok_Click(object sender, EventArgs e)
		{
			bool flag = false;
			try
			{
				if (this.CheckInputValue())
				{
					decimal value;
					if (this.type != 1)
					{
						if (this.ipInput_newIP.Text != "")
						{
							int num = 0;
							string commtype = "UDP";
							string address = "255.255.255.255";
							string[] obj = new string[10]
							{
								"MAC=",
								this.MAC,
								",IPAddress=",
								this.ipInput_newIP.Text,
								",NetMask=",
								this.ipInput_subnetMask.Value,
								",GATEIPAddress=",
								this.ipInput_Gateway.Value,
								",TCPPort=",
								null
							};
							value = this.numPort.Value;
							obj[9] = value.ToString();
							string buffer = string.Concat(obj);
							num = ModifyIPForm.ModifyIPAddress(commtype, address, buffer);
							if (num < 0)
							{
								this.ModifyIPSuccess = false;
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + " " + PullSDkErrorInfos.GetInfo(num));
							}
							else
							{
								this.ModifyIPSuccess = true;
								this.newIP = this.ipInput_newIP.Text;
								this.newGateway = this.ipInput_Gateway.Value;
								this.newNetMask = this.ipInput_subnetMask.Value;
								this.newPort = (int)this.numPort.Value;
								SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
							}
						}
						goto IL_0605;
					}
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					Machines machines = null;
					machines = machinesBll.GetModel(this.machineID);
					if (machines != null)
					{
						if (this.ipInput_newIP.Text != "")
						{
							DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machines);
							DeviceParamBll deviceParamBll = new DeviceParamBll(deviceServer.Application);
							int num2 = -1;
							string[] obj2 = new string[8]
							{
								"IPAddress=",
								this.ipInput_newIP.Text,
								",NetMask=",
								this.ipInput_subnetMask.Value,
								",GATEIPAddress=",
								this.ipInput_Gateway.Value,
								",TCPPort=",
								null
							};
							value = this.numPort.Value;
							obj2[7] = value.ToString();
							string deviceParam = string.Concat(obj2);
							if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
							{
								num2 = deviceServer.Connect(3000);
								if (num2 >= 0)
								{
									deviceServer.GetOptionValue("DHCP", out string text);
									if (text != null && text.Trim() == "1")
									{
										deviceServer.Disconnect();
										SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ": " + ShowMsgInfos.GetInfo("DHCPEnabled", "DHCP功能已启用"));
										goto end_IL_0003;
									}
									num2 = deviceServer.STD_SetSubnetMask(this.ipInput_subnetMask.Value, false);
									if (num2 >= 0)
									{
										num2 = deviceServer.STD_SetGateWay(this.ipInput_Gateway.Value, false);
										if (num2 >= 0)
										{
											int num3 = deviceServer.STD_SetIpAddress(this.ipInput_newIP.Text);
											num3 = 0;
											if (num3 >= 0)
											{
												deviceServer.Disconnect();
												deviceServer.DevInfo.IP = this.ipInput_newIP.Text;
												Thread.Sleep(1000);
												int num4 = deviceServer.Connect(3000);
												if (num4 < 0)
												{
													flag = true;
												}
												deviceServer.DevInfo.IP = machines.IP;
											}
											else
											{
												flag = true;
											}
										}
									}
								}
							}
							else
							{
								deviceServer.GetOptionValue("DHCP", out string text2);
								if (text2?.Contains("=") ?? false)
								{
									int num5 = 0;
									string[] array = text2.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
									if (array != null && array.Length > 1 && int.TryParse(array[1], out num5) && 1 == num5)
									{
										deviceServer.Disconnect();
										SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ": " + ShowMsgInfos.GetInfo("DHCPEnabled", "DHCP功能已启用"));
										goto end_IL_0003;
									}
								}
								num2 = deviceServer.SetDeviceParam(deviceParam);
							}
							if (num2 >= 0)
							{
								machines.IP = this.ipInput_newIP.Text;
								machines.gateway = this.ipInput_Gateway.Value;
								machines.subnet_mask = this.ipInput_subnetMask.Value;
								machines.Port = (int)this.numPort.Value;
								machinesBll.Update(machines);
								if (deviceServer.DevInfo.DevSDKType != SDKType.StandaloneSDK)
								{
									SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + ". " + (flag ? ShowMsgInfos.GetInfo("ManualRestart", "请手动重启设备") : ShowMsgInfos.GetInfo("SSrestartChanged", "设备将重启...")));
									deviceServer.RebootDevice();
								}
								else
								{
									SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + ". " + (flag ? ShowMsgInfos.GetInfo("ManualRestart", "请手动重启设备") : ""));
								}
								if (this.refreshDataEvent != null)
								{
									this.refreshDataEvent(this, null);
								}
							}
							else
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + " " + PullSDkErrorInfos.GetInfo(num2));
							}
							deviceServer.Disconnect();
						}
						goto IL_0605;
					}
				}
				goto end_IL_0003;
				IL_0605:
				base.Close();
				end_IL_0003:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModifyIPForm));
			this.lab_originalIP = new Label();
			this.lab_newIP = new Label();
			this.lab_subnetMask = new Label();
			this.lab_Gateway = new Label();
			this.ipInput_subnetMask = new IpAddressInput();
			this.ipInput_Gateway = new IpAddressInput();
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.label4 = new Label();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.lblPort = new Label();
			this.numPort = new NumericUpDown();
			this.ipInput_originalIP = new TextBox();
			this.ipInput_newIP = new TextBox();
			((ISupportInitialize)this.ipInput_subnetMask).BeginInit();
			((ISupportInitialize)this.ipInput_Gateway).BeginInit();
			((ISupportInitialize)this.numPort).BeginInit();
			base.SuspendLayout();
			this.lab_originalIP.Location = new Point(12, 18);
			this.lab_originalIP.Name = "lab_originalIP";
			this.lab_originalIP.Size = new Size(140, 12);
			this.lab_originalIP.TabIndex = 0;
			this.lab_originalIP.Text = "原IP地址";
			this.lab_originalIP.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_newIP.Location = new Point(12, 45);
			this.lab_newIP.Name = "lab_newIP";
			this.lab_newIP.Size = new Size(140, 12);
			this.lab_newIP.TabIndex = 1;
			this.lab_newIP.Text = "新IP地址";
			this.lab_newIP.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_subnetMask.Location = new Point(12, 73);
			this.lab_subnetMask.Name = "lab_subnetMask";
			this.lab_subnetMask.Size = new Size(140, 12);
			this.lab_subnetMask.TabIndex = 2;
			this.lab_subnetMask.Text = "子网掩码";
			this.lab_subnetMask.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_Gateway.Location = new Point(12, 101);
			this.lab_Gateway.Name = "lab_Gateway";
			this.lab_Gateway.Size = new Size(140, 12);
			this.lab_Gateway.TabIndex = 3;
			this.lab_Gateway.Text = "网关地址";
			this.lab_Gateway.TextAlign = ContentAlignment.MiddleLeft;
			this.ipInput_subnetMask.AutoOverwrite = true;
			this.ipInput_subnetMask.BackgroundStyle.Class = "DateTimeInputBackground";
			this.ipInput_subnetMask.ButtonFreeText.Shortcut = eShortcut.F2;
			this.ipInput_subnetMask.ButtonFreeText.Visible = true;
			this.ipInput_subnetMask.Location = new Point(163, 69);
			this.ipInput_subnetMask.Name = "ipInput_subnetMask";
			this.ipInput_subnetMask.Size = new Size(151, 21);
			this.ipInput_subnetMask.Style = eDotNetBarStyle.StyleManagerControlled;
			this.ipInput_subnetMask.TabIndex = 2;
			this.ipInput_Gateway.AutoOverwrite = true;
			this.ipInput_Gateway.BackgroundStyle.Class = "DateTimeInputBackground";
			this.ipInput_Gateway.ButtonFreeText.Shortcut = eShortcut.F2;
			this.ipInput_Gateway.ButtonFreeText.Visible = true;
			this.ipInput_Gateway.Location = new Point(163, 96);
			this.ipInput_Gateway.Name = "ipInput_Gateway";
			this.ipInput_Gateway.Size = new Size(151, 21);
			this.ipInput_Gateway.Style = eDotNetBarStyle.StyleManagerControlled;
			this.ipInput_Gateway.TabIndex = 3;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(145, 173);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 4;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(247, 173);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(315, 45);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 26;
			this.label4.Text = "*";
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(317, 73);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 27;
			this.label1.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(317, 101);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 28;
			this.label2.Text = "*";
			this.label3.AutoSize = true;
			this.label3.BackColor = Color.Transparent;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(316, 18);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 29;
			this.label3.Text = "*";
			this.lblPort.Location = new Point(12, 129);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new Size(140, 12);
			this.lblPort.TabIndex = 30;
			this.lblPort.Text = "端口";
			this.lblPort.TextAlign = ContentAlignment.MiddleLeft;
			this.numPort.Location = new Point(163, 125);
			this.numPort.Maximum = new decimal(new int[4]
			{
				65535,
				0,
				0,
				0
			});
			this.numPort.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numPort.Name = "numPort";
			this.numPort.Size = new Size(64, 21);
			this.numPort.TabIndex = 31;
			this.numPort.Value = new decimal(new int[4]
			{
				4370,
				0,
				0,
				0
			});
			this.ipInput_originalIP.Location = new Point(163, 14);
			this.ipInput_originalIP.Name = "ipInput_originalIP";
			this.ipInput_originalIP.ReadOnly = true;
			this.ipInput_originalIP.Size = new Size(151, 21);
			this.ipInput_originalIP.TabIndex = 32;
			this.ipInput_newIP.Location = new Point(163, 41);
			this.ipInput_newIP.Name = "ipInput_newIP";
			this.ipInput_newIP.Size = new Size(151, 21);
			this.ipInput_newIP.TabIndex = 33;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(341, 208);
			base.Controls.Add(this.ipInput_newIP);
			base.Controls.Add(this.ipInput_originalIP);
			base.Controls.Add(this.numPort);
			base.Controls.Add(this.lblPort);
			base.Controls.Add(this.ipInput_Gateway);
			base.Controls.Add(this.ipInput_subnetMask);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lab_Gateway);
			base.Controls.Add(this.lab_subnetMask);
			base.Controls.Add(this.lab_newIP);
			base.Controls.Add(this.lab_originalIP);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ModifyIPForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "修改IP地址";
			((ISupportInitialize)this.ipInput_subnetMask).EndInit();
			((ISupportInitialize)this.ipInput_Gateway).EndInit();
			((ISupportInitialize)this.numPort).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
