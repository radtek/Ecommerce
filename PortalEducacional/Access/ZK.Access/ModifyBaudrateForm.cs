/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class ModifyBaudrateForm : Office2007Form
	{
		private int MachineID;

		private int OldBaudRate;

		private Machines machinesModel = null;

		private MachinesBll machinesBll = new MachinesBll(MainForm._ia);

		private DeviceServerBll devServerBll = null;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		private ComboBox cbo_baudrate;

		private Label lbl_baudrate;

		public event EventHandler refreshDataEvent = null;

		public ModifyBaudrateForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public ModifyBaudrateForm(int MachineID)
			: this()
		{
			this.MachineID = MachineID;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void ModifyBaudrateForm_Load(object sender, EventArgs e)
		{
			int num = 38400;
			this.machinesModel = this.machinesBll.GetModel(this.MachineID);
			if (this.machinesModel != null)
			{
				num = this.machinesModel.Baudrate;
				this.OldBaudRate = this.machinesModel.Baudrate;
			}
			switch (num)
			{
			case 9600:
				this.cbo_baudrate.SelectedIndex = 0;
				break;
			case 19200:
				this.cbo_baudrate.SelectedIndex = 1;
				break;
			case 38400:
				this.cbo_baudrate.SelectedIndex = 2;
				break;
			case 57600:
				this.cbo_baudrate.SelectedIndex = 3;
				break;
			case 115200:
				this.cbo_baudrate.SelectedIndex = 4;
				break;
			default:
				this.cbo_baudrate.SelectedIndex = 2;
				break;
			}
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			try
			{
				int num = 0;
				int num2 = 38400;
				if (this.machinesModel != null)
				{
					this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
					int num3 = 0;
					if (this.devServerBll != null)
					{
						if (this.cbo_baudrate.SelectedIndex == 0)
						{
							num2 = 9600;
						}
						else if (this.cbo_baudrate.SelectedIndex == 1)
						{
							num2 = 19200;
						}
						else if (this.cbo_baudrate.SelectedIndex == 2)
						{
							num2 = 38400;
						}
						else if (this.cbo_baudrate.SelectedIndex == 3)
						{
							num2 = 57600;
						}
						else if (this.cbo_baudrate.SelectedIndex == 4)
						{
							num2 = 115200;
						}
						if (num2 == this.OldBaudRate)
						{
							base.Close();
						}
						else
						{
							string deviceParam = "RS232BaudRate=" + num2.ToString();
							if (this.devServerBll.IsConnected)
							{
								this.devServerBll.Disconnect();
							}
							num3 = this.devServerBll.Connect(3000);
							if (num3 >= 0)
							{
								if (this.devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
								{
									num3 = this.devServerBll.STD_SetDeviceInfo(DeviceInfoCode.BaudRate, this.cbo_baudrate.SelectedIndex + 3);
									num3 = 0;
									if (num3 >= 0)
									{
										this.devServerBll.Disconnect();
										this.devServerBll.DevInfo.Baudrate = num2;
										num = this.devServerBll.Connect(3000);
										this.devServerBll.DevInfo.Baudrate = this.machinesModel.Baudrate;
									}
									this.machinesModel.Baudrate = num2;
									this.machinesBll.Update(this.machinesModel);
									SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SModifySuccessDevice", "修改成功") + ". " + ((num < 0) ? ShowMsgInfos.GetInfo("ManualRestart", "请手动重启设备") : ""));
									base.Close();
								}
								else
								{
									num3 = this.devServerBll.SetDeviceParam(deviceParam);
									if (num3 >= 0)
									{
										this.machinesModel.Baudrate = num2;
										this.machinesBll.Update(this.machinesModel);
										SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SModifySuccessDeviceWillReboot", "修改成功,设备将重启!"));
										this.devServerBll.RebootDevice();
										base.Close();
									}
									else
									{
										SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ModifyFail", "修改失败") + ":" + PullSDkErrorInfos.GetInfo(num3));
									}
								}
							}
							else
							{
								SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ModifyFail", "修改失败") + ":" + PullSDkErrorInfos.GetInfo(num3));
							}
							this.devServerBll.Disconnect();
						}
					}
					else
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ModifyFail", "修改失败"));
					}
				}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModifyBaudrateForm));
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			this.cbo_baudrate = new ComboBox();
			this.lbl_baudrate = new Label();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(227, 59);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 2;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(125, 59);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 1;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.cbo_baudrate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_baudrate.FormattingEnabled = true;
			this.cbo_baudrate.Items.AddRange(new object[5]
			{
				"9600",
				"19200",
				"38400",
				"57600",
				"115200"
			});
			this.cbo_baudrate.Location = new Point(124, 15);
			this.cbo_baudrate.Name = "cbo_baudrate";
			this.cbo_baudrate.Size = new Size(184, 20);
			this.cbo_baudrate.TabIndex = 0;
			this.lbl_baudrate.Location = new Point(12, 18);
			this.lbl_baudrate.Name = "lbl_baudrate";
			this.lbl_baudrate.Size = new Size(100, 12);
			this.lbl_baudrate.TabIndex = 8;
			this.lbl_baudrate.Text = "波特率";
			this.lbl_baudrate.TextAlign = ContentAlignment.MiddleLeft;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(321, 94);
			base.Controls.Add(this.cbo_baudrate);
			base.Controls.Add(this.lbl_baudrate);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ModifyBaudrateForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "修改波特率";
			base.Load += this.ModifyBaudrateForm_Load;
			base.ResumeLayout(false);
		}
	}
}
