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
using ZK.Utils;

namespace ZK.Access
{
	public class ModifyComPwdForm : Office2007Form
	{
		private int m_id = 0;

		private DeviceServerBll devServerBll = null;

		private Machines machinesModel = null;

		private MachinesBll machinesBll = new MachinesBll(MainForm._ia);

		private IContainer components = null;

		private Label lbl_star1;

		private Label lbl_star3;

		private Label lbl_star2;

		private Label lbl_DoubleComPwd;

		private Label lbl_NewComPwd;

		private Label lbl_OldComPwd;

		private TextBox txt_OldComPwd;

		private TextBox txt_NewComPwd;

		private TextBox txt_DoubleComPwd;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		public event EventHandler refreshDataEvent = null;

		public ModifyComPwdForm(int MachineID)
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.m_id = MachineID;
			this.machinesModel = this.machinesBll.GetModel(this.m_id);
			if (string.IsNullOrEmpty(this.machinesModel.CommPassword) || this.machinesModel.CommPassword.Trim() == "0")
			{
				this.txt_OldComPwd.Enabled = false;
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			bool flag = false;
			try
			{
				if (this.m_id > 0)
				{
					if (this.machinesModel != null)
					{
						if (!string.IsNullOrEmpty(this.machinesModel.CommPassword) && this.txt_OldComPwd.Text != this.machinesModel.CommPassword && this.machinesModel.CommPassword.Trim() != "0")
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SComPwdWrong", "通讯密码输入错误"));
							this.txt_OldComPwd.Focus();
						}
						else if (this.txt_NewComPwd.Text != this.txt_DoubleComPwd.Text)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNewAndOldPasswdNotEquel", "新密码输入不一致"));
							this.txt_DoubleComPwd.Focus();
						}
						else
						{
							this.devServerBll = DeviceServers.Instance.GetDeviceServer(this.machinesModel);
							if (this.devServerBll != null)
							{
								int num = this.devServerBll.Connect(3000);
								if (num >= 0)
								{
									string deviceParam = "ComPwd=" + this.txt_NewComPwd.Text;
									if (this.devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
									{
										deviceParam = this.txt_NewComPwd.Text.Trim();
										num = this.devServerBll.STD_SetDeviceCommPwd(int.Parse(string.IsNullOrEmpty(deviceParam) ? "0" : deviceParam));
										num = 0;
										if (num >= 0)
										{
											this.devServerBll.Disconnect();
											this.devServerBll.DevInfo.CommPassword = deviceParam;
											int num2 = this.devServerBll.Connect(3000);
											if (num2 < 0)
											{
												flag = true;
											}
											this.devServerBll.DevInfo.CommPassword = this.machinesModel.CommPassword;
										}
									}
									else
									{
										num = this.devServerBll.SetDeviceParam(deviceParam);
									}
									if (num >= 0)
									{
										this.machinesModel.CommPassword = ((this.txt_NewComPwd.Text.Trim() == "0") ? "" : this.txt_NewComPwd.Text);
										this.machinesBll.Update(this.machinesModel);
										if (this.devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
										{
											SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SModifySuccessDevice", "修改成功") + ". " + (flag ? ShowMsgInfos.GetInfo("ManualRestart", "请手动重启设备") : ""));
										}
										else
										{
											SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SModifySuccessDeviceWillReboot", "修改成功,设备将重启!"));
											this.devServerBll.RebootDevice();
										}
										base.Close();
									}
									else
									{
										SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + " " + PullSDkErrorInfos.GetInfo(num));
									}
									this.devServerBll.Disconnect();
								}
								else
								{
									SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + " " + PullSDkErrorInfos.GetInfo(num));
								}
							}
							else
							{
								SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SConnectFailed", "操作失败") + " " + PullSDkErrorInfos.GetInfo(-1002));
							}
						}
					}
					else
					{
						base.Close();
					}
				}
				else
				{
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void txt_OldComPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			SDKType devSDKType = this.machinesModel.DevSDKType;
			if (devSDKType == SDKType.StandaloneSDK)
			{
				CheckInfo.KeyPress(sender, e, 6);
			}
			else
			{
				CheckInfo.KeyPress(sender, e, 8);
			}
			if (e.KeyChar == ' ')
			{
				e.Handled = true;
			}
		}

		private void txt_NewComPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			SDKType devSDKType = this.machinesModel.DevSDKType;
			if (devSDKType == SDKType.StandaloneSDK)
			{
				CheckInfo.KeyPress(sender, e, 6);
			}
			else
			{
				CheckInfo.KeyPress(sender, e, 8);
			}
			if (e.KeyChar == ' ')
			{
				e.Handled = true;
			}
		}

		private void txt_DoubleComPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			SDKType devSDKType = this.machinesModel.DevSDKType;
			if (devSDKType == SDKType.StandaloneSDK)
			{
				CheckInfo.KeyPress(sender, e, 6);
			}
			else
			{
				CheckInfo.KeyPress(sender, e, 8);
			}
			if (e.KeyChar == ' ')
			{
				e.Handled = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModifyComPwdForm));
			this.lbl_star1 = new Label();
			this.lbl_star3 = new Label();
			this.lbl_star2 = new Label();
			this.lbl_DoubleComPwd = new Label();
			this.lbl_NewComPwd = new Label();
			this.lbl_OldComPwd = new Label();
			this.txt_OldComPwd = new TextBox();
			this.txt_NewComPwd = new TextBox();
			this.txt_DoubleComPwd = new TextBox();
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			base.SuspendLayout();
			this.lbl_star1.AutoSize = true;
			this.lbl_star1.BackColor = Color.Transparent;
			this.lbl_star1.ForeColor = Color.Red;
			this.lbl_star1.Location = new Point(357, 20);
			this.lbl_star1.Name = "lbl_star1";
			this.lbl_star1.Size = new Size(11, 12);
			this.lbl_star1.TabIndex = 35;
			this.lbl_star1.Text = "*";
			this.lbl_star3.AutoSize = true;
			this.lbl_star3.ForeColor = Color.Red;
			this.lbl_star3.Location = new Point(357, 85);
			this.lbl_star3.Name = "lbl_star3";
			this.lbl_star3.Size = new Size(11, 12);
			this.lbl_star3.TabIndex = 34;
			this.lbl_star3.Text = "*";
			this.lbl_star2.AutoSize = true;
			this.lbl_star2.ForeColor = Color.Red;
			this.lbl_star2.Location = new Point(357, 53);
			this.lbl_star2.Name = "lbl_star2";
			this.lbl_star2.Size = new Size(11, 12);
			this.lbl_star2.TabIndex = 33;
			this.lbl_star2.Text = "*";
			this.lbl_DoubleComPwd.Location = new Point(12, 83);
			this.lbl_DoubleComPwd.Name = "lbl_DoubleComPwd";
			this.lbl_DoubleComPwd.Size = new Size(181, 12);
			this.lbl_DoubleComPwd.TabIndex = 32;
			this.lbl_DoubleComPwd.Text = "确认通讯密码";
			this.lbl_DoubleComPwd.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_NewComPwd.Location = new Point(12, 51);
			this.lbl_NewComPwd.Name = "lbl_NewComPwd";
			this.lbl_NewComPwd.Size = new Size(181, 12);
			this.lbl_NewComPwd.TabIndex = 31;
			this.lbl_NewComPwd.Text = "新通讯密码";
			this.lbl_NewComPwd.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_OldComPwd.Location = new Point(12, 19);
			this.lbl_OldComPwd.Name = "lbl_OldComPwd";
			this.lbl_OldComPwd.Size = new Size(181, 12);
			this.lbl_OldComPwd.TabIndex = 30;
			this.lbl_OldComPwd.Text = "旧通讯密码";
			this.lbl_OldComPwd.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_OldComPwd.Location = new Point(204, 16);
			this.txt_OldComPwd.Name = "txt_OldComPwd";
			this.txt_OldComPwd.PasswordChar = '*';
			this.txt_OldComPwd.Size = new Size(147, 21);
			this.txt_OldComPwd.TabIndex = 0;
			this.txt_OldComPwd.KeyPress += this.txt_OldComPwd_KeyPress;
			this.txt_NewComPwd.Location = new Point(204, 48);
			this.txt_NewComPwd.Name = "txt_NewComPwd";
			this.txt_NewComPwd.PasswordChar = '*';
			this.txt_NewComPwd.Size = new Size(147, 21);
			this.txt_NewComPwd.TabIndex = 1;
			this.txt_NewComPwd.KeyPress += this.txt_NewComPwd_KeyPress;
			this.txt_DoubleComPwd.Location = new Point(204, 80);
			this.txt_DoubleComPwd.Name = "txt_DoubleComPwd";
			this.txt_DoubleComPwd.PasswordChar = '*';
			this.txt_DoubleComPwd.Size = new Size(147, 21);
			this.txt_DoubleComPwd.TabIndex = 2;
			this.txt_DoubleComPwd.KeyPress += this.txt_DoubleComPwd_KeyPress;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(290, 131);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(188, 131);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 3;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(384, 166);
			base.Controls.Add(this.txt_DoubleComPwd);
			base.Controls.Add(this.txt_NewComPwd);
			base.Controls.Add(this.txt_OldComPwd);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lbl_star1);
			base.Controls.Add(this.lbl_star3);
			base.Controls.Add(this.lbl_star2);
			base.Controls.Add(this.lbl_DoubleComPwd);
			base.Controls.Add(this.lbl_NewComPwd);
			base.Controls.Add(this.lbl_OldComPwd);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ModifyComPwdForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "修改通讯密码";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
