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
using ZK.Utils;

namespace ZK.Access
{
	public class ModifyPasswordForm : Office2007Form
	{
		private IContainer components = null;

		private Label lab_oldPassword;

		private Label lab_newPassword;

		private Label lab_ConfirmPassword;

		private TextBox txt_OldPassword;

		private TextBox txt_NewPassword;

		private TextBox txt_ConfirmPassword;

		private ButtonX btn_OK;

		private ButtonX Btn_Cancel;

		private Label label1;

		private Label label2;

		private Label label3;

		public ModifyPasswordForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void Btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private bool Check()
		{
			if (SysInfos.SysUserInfo == null)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNotLogin", "用户未登陆"));
				return false;
			}
			if (SysInfos.SysUserInfo.password != this.txt_OldPassword.Text)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOldPasswdWrong", "旧密码不对"));
				return false;
			}
			if (string.IsNullOrEmpty(this.txt_OldPassword.Text) || string.IsNullOrEmpty(this.txt_NewPassword.Text) || this.txt_NewPassword.Text != this.txt_ConfirmPassword.Text)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SPasswdWrongOrNull", "密码不一致或为空"));
				return false;
			}
			if (this.txt_NewPassword.Text.Length < 4 || this.txt_ConfirmPassword.Text.Length < 4)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserPasswordLength", "密码长度必须大于4位,小于18位"));
				return false;
			}
			return true;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.Check())
				{
					AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
					SysInfos.SysUserInfo.password = this.txt_NewPassword.Text;
					if (authUserBll.Update(SysInfos.SysUserInfo))
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuceed", "操作成功"));
						base.Close();
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void txt_OldPassword_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 18);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ModifyPasswordForm));
			this.lab_oldPassword = new Label();
			this.lab_newPassword = new Label();
			this.lab_ConfirmPassword = new Label();
			this.txt_OldPassword = new TextBox();
			this.txt_NewPassword = new TextBox();
			this.txt_ConfirmPassword = new TextBox();
			this.btn_OK = new ButtonX();
			this.Btn_Cancel = new ButtonX();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			base.SuspendLayout();
			this.lab_oldPassword.Location = new Point(12, 22);
			this.lab_oldPassword.Name = "lab_oldPassword";
			this.lab_oldPassword.Size = new Size(136, 12);
			this.lab_oldPassword.TabIndex = 0;
			this.lab_oldPassword.Text = "旧密码";
			this.lab_oldPassword.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_newPassword.Location = new Point(12, 54);
			this.lab_newPassword.Name = "lab_newPassword";
			this.lab_newPassword.Size = new Size(136, 12);
			this.lab_newPassword.TabIndex = 1;
			this.lab_newPassword.Text = "新密码";
			this.lab_newPassword.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_ConfirmPassword.Location = new Point(12, 86);
			this.lab_ConfirmPassword.Name = "lab_ConfirmPassword";
			this.lab_ConfirmPassword.Size = new Size(136, 12);
			this.lab_ConfirmPassword.TabIndex = 2;
			this.lab_ConfirmPassword.Text = "新密码确认";
			this.lab_ConfirmPassword.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_OldPassword.Location = new Point(170, 17);
			this.txt_OldPassword.Name = "txt_OldPassword";
			this.txt_OldPassword.PasswordChar = '*';
			this.txt_OldPassword.Size = new Size(162, 21);
			this.txt_OldPassword.TabIndex = 0;
			this.txt_OldPassword.KeyPress += this.txt_OldPassword_KeyPress;
			this.txt_NewPassword.Location = new Point(170, 49);
			this.txt_NewPassword.Name = "txt_NewPassword";
			this.txt_NewPassword.PasswordChar = '*';
			this.txt_NewPassword.Size = new Size(162, 21);
			this.txt_NewPassword.TabIndex = 1;
			this.txt_NewPassword.KeyPress += this.txt_OldPassword_KeyPress;
			this.txt_ConfirmPassword.Location = new Point(170, 81);
			this.txt_ConfirmPassword.Name = "txt_ConfirmPassword";
			this.txt_ConfirmPassword.PasswordChar = '*';
			this.txt_ConfirmPassword.Size = new Size(162, 21);
			this.txt_ConfirmPassword.TabIndex = 2;
			this.txt_ConfirmPassword.KeyPress += this.txt_OldPassword_KeyPress;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(170, 126);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.Btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.Btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.Btn_Cancel.Location = new Point(264, 126);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new Size(82, 23);
			this.Btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.Btn_Cancel.TabIndex = 4;
			this.Btn_Cancel.Text = "取消";
			this.Btn_Cancel.Click += this.Btn_Cancel_Click;
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(335, 86);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 24;
			this.label1.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(335, 54);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 25;
			this.label2.Text = "*";
			this.label3.AutoSize = true;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(335, 22);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 26;
			this.label3.Text = "*";
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(358, 161);
			base.Controls.Add(this.txt_ConfirmPassword);
			base.Controls.Add(this.txt_NewPassword);
			base.Controls.Add(this.txt_OldPassword);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.Btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_ConfirmPassword);
			base.Controls.Add(this.lab_newPassword);
			base.Controls.Add(this.lab_oldPassword);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ModifyPasswordForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "修改密码";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
