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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class OptionsForm : Office2007Form
	{
		private List<Machines> listModel = null;

		private DeviceServerBll devServerBll = null;

		private MachinesBll bll = new MachinesBll(MainForm._ia);

		private int m_pid = -1;

		private WaitForm m_wait = WaitForm.Instance;

		private IContainer components = null;

		private DevComponents.DotNetBar.TabControl tab_WiegandParameters;

		private TabControlPanel tabControlPanel2;

		private ButtonX btn_RS485MasterSlave_canncel;

		private ButtonX btn_RS485MasterSlave_ok;

		private TabItem tabItem_MasterSlave;

		private CheckBoxX rdo_485_slave;

		private CheckBoxX rdo_485_master;

		private Label lbl_option_seting;

		private Label lbl_BaudRateRange;

		private TextBox txt_Rs485Reader;

		private Label lbl_Rs485Reader;

		private Label label1;

		public event EventHandler refreshDataEvent = null;

		public OptionsForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public OptionsForm(int pid, List<Machines> list)
			: this()
		{
			this.m_pid = pid;
			if (this.m_pid >= 0)
			{
				this.listModel = list;
				this.Bindata();
			}
		}

		private void Bindata()
		{
			if (this.listModel != null && this.listModel.Count > 0)
			{
				this.txt_Rs485Reader.Text = this.listModel[0].MachineNumber.ToString();
				if (this.listModel[0].deviceOption != null)
				{
					this.rdo_485_master.Checked = false;
					this.rdo_485_slave.Checked = false;
					byte[] deviceOption = this.listModel[0].deviceOption;
					string @string = Encoding.ASCII.GetString(deviceOption, 0, deviceOption.Length);
					string[] array = @string.ToLower().Split(',');
					List<string> list = new List<string>();
					if (array.Length != 0)
					{
						for (int i = 0; i < array.Length; i++)
						{
							list.Add(array[i]);
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
		}

		private void btn_RS485MasterSlave_canncel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private bool check()
		{
			if (string.IsNullOrEmpty(this.txt_Rs485Reader.Text))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputRS485Address", "请输入RS485地址"));
				this.txt_Rs485Reader.Focus();
				return false;
			}
			return true;
		}

		private void btn_RS485MasterSlave_ok_Click(object sender, EventArgs e)
		{
			switch (this.m_pid)
			{
			case 0:
				if (this.check())
				{
					if (this.listModel != null && this.listModel.Count > 0)
					{
						this.m_wait.ShowEx();
						this.m_wait.ShowProgress(0);
						int num = int.Parse(this.txt_Rs485Reader.Text);
						for (int i = 0; i < this.listModel.Count; i++)
						{
							bool flag = false;
							this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.listModel[i]);
							if (!this.rdo_485_master.Checked && !this.rdo_485_slave.Checked)
							{
								StringBuilder stringBuilder = new StringBuilder();
								stringBuilder.Append("PC485AsInbio485=0,MasterInbio485=0");
								byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
								this.listModel[i].deviceOption = bytes;
							}
							else if (this.rdo_485_master.Checked)
							{
								StringBuilder stringBuilder2 = new StringBuilder();
								stringBuilder2.Append("PC485AsInbio485=1,MasterInbio485=1,RS232BaudRate=115200");
								byte[] bytes2 = Encoding.ASCII.GetBytes(stringBuilder2.ToString());
								this.listModel[i].deviceOption = bytes2;
							}
							else if (this.rdo_485_slave.Checked)
							{
								StringBuilder stringBuilder3 = new StringBuilder();
								stringBuilder3.Append("PC485AsInbio485=1,MasterInbio485=0,RS232BaudRate=115200");
								byte[] bytes3 = Encoding.ASCII.GetBytes(stringBuilder3.ToString());
								this.listModel[i].deviceOption = bytes3;
							}
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + this.listModel[i].MachineAlias + ")");
							if (this.devServerBll.IsConnected)
							{
								this.devServerBll.Disconnect();
							}
							int num2 = this.devServerBll.Connect(3000);
							if (num2 >= 0)
							{
								SDKType devSDKType = this.devServerBll.DevInfo.DevSDKType;
								if (devSDKType == SDKType.StandaloneSDK)
								{
									int num3 = this.rdo_485_master.Checked ? 1 : 0;
									int num4 = (this.rdo_485_master.Checked || this.rdo_485_slave.Checked) ? 1 : 0;
									num2 = this.devServerBll.STD_SetSysOption("PC485AsInbio485", num4.ToString());
									if (num2 >= 0)
									{
										num2 = this.devServerBll.STD_SetSysOption("MasterInbio485", num3.ToString());
										if (num2 >= 0)
										{
											if (this.rdo_485_master.Checked || this.rdo_485_slave.Checked)
											{
												num2 = this.devServerBll.STD_SetDeviceInfo(DeviceInfoCode.BaudRate, 7);
												this.listModel[i].Baudrate = 115200;
											}
											if (this.rdo_485_master.Checked)
											{
												this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToMaster", "转换485为主机"));
											}
											else
											{
												this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SCancelToMaster", "取消485为主机"));
											}
										}
										else
										{
											this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("Change485StateError", "转换485状态时出错：") + PullSDkErrorInfos.GetInfo(num2));
										}
									}
									else
									{
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("Change485StateError", "转换485状态时出错：") + PullSDkErrorInfos.GetInfo(num2));
									}
									num2 = this.devServerBll.STD_SetDeviceInfo(DeviceInfoCode.MachineNumber, num + i);
									flag = true;
									this.listModel[i].MachineNumber = num + i;
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SDeviceIdIsChange", "Rs485地址更改为:") + this.listModel[i].MachineNumber);
									if (flag)
									{
										this.bll.Update(this.listModel[i]);
									}
									this.devServerBll.Disconnect();
									this.devServerBll.DevInfo.MachineNumber = this.listModel[i].MachineNumber;
									int num5 = this.devServerBll.Connect(3000);
									this.devServerBll.DevInfo.MachineNumber = num - i;
									if (num5 < 0)
									{
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("ManualRestart", "请手动重启设备") + "(" + this.listModel[i].MachineAlias + ")");
									}
									else
									{
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SSrestartChanged", "设备将重启...") + "(" + this.listModel[i].MachineAlias + ")");
										this.devServerBll.RebootDevice();
									}
									this.devServerBll.Disconnect();
								}
								else
								{
									num2 = this.devServerBll.SetDeviceParam("DeviceID=" + (num + i));
									if (num2 >= 0 || num2 == -2)
									{
										this.devServerBll.Disconnect();
										this.listModel[i].MachineNumber = num + i;
										this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.listModel[i]);
										num2 = this.devServerBll.Connect(3000);
										if (num2 >= 0)
										{
											flag = true;
											this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SDeviceIdIsChange", "Rs485地址更改为:") + this.listModel[i].MachineNumber);
											if (!this.rdo_485_master.Checked && !this.rdo_485_slave.Checked)
											{
												num2 = this.devServerBll.SetDeviceParam("PC485AsInbio485=0,MasterInbio485=0");
												if (num2 >= 0)
												{
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToConnectPc", "转换485与软件通讯"));
												}
												else
												{
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToConnectPcFailed", "转换485与软件通讯失败: " + PullSDkErrorInfos.GetInfo(num2)));
												}
											}
											if (this.rdo_485_master.Checked)
											{
												num2 = this.devServerBll.SetDeviceParam("PC485AsInbio485=1,MasterInbio485=1,RS232BaudRate=115200");
												if (num2 >= 0 || num2 == -2)
												{
													this.listModel[i].Baudrate = 115200;
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToMaster", "转换485为主机"));
												}
												else
												{
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToMasterFailed", "转换485为主机失败: " + PullSDkErrorInfos.GetInfo(num2)));
												}
											}
											if (this.rdo_485_slave.Checked)
											{
												num2 = this.devServerBll.SetDeviceParam("PC485AsInbio485=1,MasterInbio485=0,RS232BaudRate=115200");
												if (num2 >= 0 || num2 == -2)
												{
													this.listModel[i].Baudrate = 115200;
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToSlave", "转换485为从机"));
												}
												else
												{
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SChangeToSlaveFailed", "转换485为从机失败: " + PullSDkErrorInfos.GetInfo(num2)));
												}
											}
										}
										else
										{
											this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("ChangeMachineNumberFialed", "Rs485地址更改失败: " + PullSDkErrorInfos.GetInfo(num2)));
										}
									}
									else
									{
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("ChangeMachineNumberFialed", "Rs485地址更改失败: " + PullSDkErrorInfos.GetInfo(num2)));
									}
									if (flag)
									{
										this.bll.Update(this.listModel[i]);
									}
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SSrestartChanged", "设备将重启...") + "(" + this.listModel[i].MachineAlias + ")");
									this.devServerBll.RebootDevice();
									this.devServerBll.Disconnect();
								}
							}
							else
							{
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "连接设备失败") + "(" + this.listModel[i].MachineAlias + "): " + PullSDkErrorInfos.GetInfo(num2));
							}
							this.m_wait.ShowProgress((i + 1) * 100 / this.listModel.Count);
						}
						this.m_wait.HideEx(false);
						if (this.refreshDataEvent != null)
						{
							this.refreshDataEvent(this, null);
						}
					}
					base.Close();
				}
				break;
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(OptionsForm));
			this.tab_WiegandParameters = new DevComponents.DotNetBar.TabControl();
			this.tabControlPanel2 = new TabControlPanel();
			this.label1 = new Label();
			this.rdo_485_slave = new CheckBoxX();
			this.rdo_485_master = new CheckBoxX();
			this.lbl_option_seting = new Label();
			this.lbl_BaudRateRange = new Label();
			this.txt_Rs485Reader = new TextBox();
			this.lbl_Rs485Reader = new Label();
			this.btn_RS485MasterSlave_canncel = new ButtonX();
			this.btn_RS485MasterSlave_ok = new ButtonX();
			this.tabItem_MasterSlave = new TabItem(this.components);
			((ISupportInitialize)this.tab_WiegandParameters).BeginInit();
			this.tab_WiegandParameters.SuspendLayout();
			this.tabControlPanel2.SuspendLayout();
			base.SuspendLayout();
			this.tab_WiegandParameters.BackColor = Color.FromArgb(194, 217, 247);
			this.tab_WiegandParameters.CanReorderTabs = true;
			this.tab_WiegandParameters.Controls.Add(this.tabControlPanel2);
			this.tab_WiegandParameters.Dock = DockStyle.Fill;
			this.tab_WiegandParameters.Location = new Point(0, 0);
			this.tab_WiegandParameters.Name = "tab_WiegandParameters";
			this.tab_WiegandParameters.SelectedTabFont = new Font("SimSun", 9f, FontStyle.Bold);
			this.tab_WiegandParameters.SelectedTabIndex = 0;
			this.tab_WiegandParameters.Size = new Size(436, 197);
			this.tab_WiegandParameters.TabIndex = 23;
			this.tab_WiegandParameters.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
			this.tab_WiegandParameters.Tabs.Add(this.tabItem_MasterSlave);
			this.tab_WiegandParameters.Text = "tabControl1";
			this.tabControlPanel2.Controls.Add(this.rdo_485_slave);
			this.tabControlPanel2.Controls.Add(this.rdo_485_master);
			this.tabControlPanel2.Controls.Add(this.txt_Rs485Reader);
			this.tabControlPanel2.Controls.Add(this.label1);
			this.tabControlPanel2.Controls.Add(this.lbl_option_seting);
			this.tabControlPanel2.Controls.Add(this.lbl_BaudRateRange);
			this.tabControlPanel2.Controls.Add(this.lbl_Rs485Reader);
			this.tabControlPanel2.Controls.Add(this.btn_RS485MasterSlave_canncel);
			this.tabControlPanel2.Controls.Add(this.btn_RS485MasterSlave_ok);
			this.tabControlPanel2.Dock = DockStyle.Fill;
			this.tabControlPanel2.Location = new Point(0, 26);
			this.tabControlPanel2.Name = "tabControlPanel2";
			this.tabControlPanel2.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel2.Size = new Size(436, 171);
			this.tabControlPanel2.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel2.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel2.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel2.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel2.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel2.Style.GradientAngle = 90;
			this.tabControlPanel2.TabIndex = 0;
			this.tabControlPanel2.TabItem = this.tabItem_MasterSlave;
			this.label1.BackColor = Color.Transparent;
			this.label1.Location = new Point(25, 79);
			this.label1.Name = "label1";
			this.label1.Size = new Size(180, 12);
			this.label1.TabIndex = 74;
			this.label1.Text = "RS485从机";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.rdo_485_slave.BackColor = Color.Transparent;
			this.rdo_485_slave.BackgroundImageLayout = ImageLayout.None;
			this.rdo_485_slave.BackgroundStyle.BackColor2 = Color.FromArgb(255, 0, 0);
			this.rdo_485_slave.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBarBackground2;
			this.rdo_485_slave.BackgroundStyle.BackgroundImagePosition = eStyleBackgroundImage.Center;
			this.rdo_485_slave.BackgroundStyle.BorderColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_slave.BackgroundStyle.BorderTopColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_slave.BackgroundStyle.Class = "";
			this.rdo_485_slave.Cursor = Cursors.Default;
			this.rdo_485_slave.Location = new Point(282, 74);
			this.rdo_485_slave.Name = "rdo_485_slave";
			this.rdo_485_slave.Size = new Size(82, 17);
			this.rdo_485_slave.Style = eDotNetBarStyle.StyleManagerControlled;
			this.rdo_485_slave.TabIndex = 73;
			this.rdo_485_slave.Text = "从机";
			this.rdo_485_slave.Click += this.rdo_485_slave_Click;
			this.rdo_485_master.BackColor = Color.Transparent;
			this.rdo_485_master.BackgroundStyle.BackColor2 = Color.FromArgb(255, 0, 0);
			this.rdo_485_master.BackgroundStyle.BackColorSchemePart = eColorSchemePart.MenuBarBackground2;
			this.rdo_485_master.BackgroundStyle.BackgroundImagePosition = eStyleBackgroundImage.Center;
			this.rdo_485_master.BackgroundStyle.BorderColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_master.BackgroundStyle.BorderTopColor = Color.FromArgb(255, 0, 0);
			this.rdo_485_master.BackgroundStyle.Class = "";
			this.rdo_485_master.Location = new Point(281, 47);
			this.rdo_485_master.Name = "rdo_485_master";
			this.rdo_485_master.Size = new Size(69, 18);
			this.rdo_485_master.Style = eDotNetBarStyle.StyleManagerControlled;
			this.rdo_485_master.TabIndex = 72;
			this.rdo_485_master.Text = "主机";
			this.rdo_485_master.Click += this.rdo_485_master_Click;
			this.lbl_option_seting.BackColor = Color.Transparent;
			this.lbl_option_seting.Location = new Point(25, 53);
			this.lbl_option_seting.Name = "lbl_option_seting";
			this.lbl_option_seting.Size = new Size(180, 12);
			this.lbl_option_seting.TabIndex = 71;
			this.lbl_option_seting.Text = "RS485主机";
			this.lbl_option_seting.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_BaudRateRange.BackColor = Color.Transparent;
			this.lbl_BaudRateRange.ForeColor = SystemColors.HotTrack;
			this.lbl_BaudRateRange.Location = new Point(358, 19);
			this.lbl_BaudRateRange.Name = "lbl_BaudRateRange";
			this.lbl_BaudRateRange.Size = new Size(63, 12);
			this.lbl_BaudRateRange.TabIndex = 61;
			this.lbl_BaudRateRange.Text = "(1-255)";
			this.txt_Rs485Reader.Location = new Point(284, 15);
			this.txt_Rs485Reader.Name = "txt_Rs485Reader";
			this.txt_Rs485Reader.Size = new Size(69, 21);
			this.txt_Rs485Reader.TabIndex = 60;
			this.txt_Rs485Reader.Text = "1";
			this.txt_Rs485Reader.KeyPress += this.txt_Rs485Reader_KeyPress;
			this.lbl_Rs485Reader.BackColor = Color.Transparent;
			this.lbl_Rs485Reader.Location = new Point(25, 21);
			this.lbl_Rs485Reader.Name = "lbl_Rs485Reader";
			this.lbl_Rs485Reader.Size = new Size(193, 12);
			this.lbl_Rs485Reader.TabIndex = 59;
			this.lbl_Rs485Reader.Text = "RS485地址";
			this.lbl_Rs485Reader.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_RS485MasterSlave_canncel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_RS485MasterSlave_canncel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_RS485MasterSlave_canncel.Location = new Point(321, 128);
			this.btn_RS485MasterSlave_canncel.Name = "btn_RS485MasterSlave_canncel";
			this.btn_RS485MasterSlave_canncel.Size = new Size(82, 23);
			this.btn_RS485MasterSlave_canncel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_RS485MasterSlave_canncel.TabIndex = 46;
			this.btn_RS485MasterSlave_canncel.Text = "取消";
			this.btn_RS485MasterSlave_canncel.Click += this.btn_RS485MasterSlave_canncel_Click;
			this.btn_RS485MasterSlave_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_RS485MasterSlave_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_RS485MasterSlave_ok.Location = new Point(219, 128);
			this.btn_RS485MasterSlave_ok.Name = "btn_RS485MasterSlave_ok";
			this.btn_RS485MasterSlave_ok.Size = new Size(82, 23);
			this.btn_RS485MasterSlave_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_RS485MasterSlave_ok.TabIndex = 45;
			this.btn_RS485MasterSlave_ok.Text = "确定";
			this.btn_RS485MasterSlave_ok.Click += this.btn_RS485MasterSlave_ok_Click;
			this.tabItem_MasterSlave.AttachedControl = this.tabControlPanel2;
			this.tabItem_MasterSlave.Name = "tabItem_MasterSlave";
			this.tabItem_MasterSlave.Text = "RS485主从机配置";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(436, 197);
			base.Controls.Add(this.tab_WiegandParameters);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "OptionsForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "参数配置";
			((ISupportInitialize)this.tab_WiegandParameters).EndInit();
			this.tab_WiegandParameters.ResumeLayout(false);
			this.tabControlPanel2.ResumeLayout(false);
			this.tabControlPanel2.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
