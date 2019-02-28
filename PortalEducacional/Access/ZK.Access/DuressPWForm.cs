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
	public class DuressPWForm : Office2007Form
	{
		private bool presence = false;

		private string password;

		private IContainer components = null;

		private TextBox txt_forceNewPwdOk;

		private TextBox txt_forceNewPwd;

		private Label lbl_forceNewPwdOk;

		private Label lab_duressRange;

		private Label lbl_forceNewPwd;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		public event EventHandler refreshDataEvent = null;

		public DuressPWForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public DuressPWForm(bool force, string Value)
			: this()
		{
			this.presence = force;
			this.password = Value;
			if (this.presence)
			{
				this.txt_forceNewPwd.Enabled = true;
				this.txt_forceNewPwdOk.Enabled = true;
				this.lbl_forceNewPwd.Enabled = true;
				this.lbl_forceNewPwdOk.Enabled = true;
				this.lab_duressRange.Enabled = true;
			}
			else
			{
				this.txt_forceNewPwd.Enabled = true;
				this.txt_forceNewPwdOk.Enabled = true;
				this.lbl_forceNewPwd.Enabled = true;
				this.lbl_forceNewPwdOk.Enabled = true;
				this.lab_duressRange.Enabled = true;
			}
		}

		private void txt_forcePwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 8);
		}

		private void txt_forceNewPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 6);
		}

		private void txt_forceNewPwdOk_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txt_forceNewPwd.Text))
			{
				this.txt_forceNewPwd.Focus();
			}
		}

		private void txt_forceNewPwdOk_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 6);
		}

		private bool Set()
		{
			if (this.txt_forceNewPwd.Text != this.txt_forceNewPwdOk.Text)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PwdError", "新密码与确认密码一致!"));
				return false;
			}
			if (!string.IsNullOrEmpty(this.txt_forceNewPwdOk.Text))
			{
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				if (userInfoBll.GetModelList("PASSWORD='" + this.txt_forceNewPwdOk.Text + "'") != null && userInfoBll.GetModelList("PASSWORD='" + this.txt_forceNewPwdOk.Text + "'").Count > 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SForceNewPwdError", "胁迫密码不能与任意人员密码相同"));
					this.txt_forceNewPwdOk.Focus();
					return false;
				}
			}
			return true;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.Set() && this.refreshDataEvent != null)
			{
				this.refreshDataEvent(this.txt_forceNewPwdOk.Text, null);
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DuressPWForm));
			this.txt_forceNewPwdOk = new TextBox();
			this.txt_forceNewPwd = new TextBox();
			this.lbl_forceNewPwdOk = new Label();
			this.lab_duressRange = new Label();
			this.lbl_forceNewPwd = new Label();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			base.SuspendLayout();
			this.txt_forceNewPwdOk.Enabled = false;
			this.txt_forceNewPwdOk.Location = new Point(139, 48);
			this.txt_forceNewPwdOk.Name = "txt_forceNewPwdOk";
			this.txt_forceNewPwdOk.PasswordChar = '*';
			this.txt_forceNewPwdOk.Size = new Size(110, 21);
			this.txt_forceNewPwdOk.TabIndex = 2;
			this.txt_forceNewPwdOk.Click += this.txt_forceNewPwdOk_Click;
			this.txt_forceNewPwdOk.KeyPress += this.txt_forceNewPwdOk_KeyPress;
			this.txt_forceNewPwd.Enabled = false;
			this.txt_forceNewPwd.Location = new Point(139, 15);
			this.txt_forceNewPwd.Name = "txt_forceNewPwd";
			this.txt_forceNewPwd.PasswordChar = '*';
			this.txt_forceNewPwd.Size = new Size(110, 21);
			this.txt_forceNewPwd.TabIndex = 1;
			this.txt_forceNewPwd.KeyPress += this.txt_forceNewPwd_KeyPress;
			this.lbl_forceNewPwdOk.BackColor = Color.Transparent;
			this.lbl_forceNewPwdOk.Enabled = false;
			this.lbl_forceNewPwdOk.Location = new Point(12, 52);
			this.lbl_forceNewPwdOk.Name = "lbl_forceNewPwdOk";
			this.lbl_forceNewPwdOk.Size = new Size(120, 12);
			this.lbl_forceNewPwdOk.TabIndex = 78;
			this.lbl_forceNewPwdOk.Text = "确认密码";
			this.lbl_forceNewPwdOk.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_duressRange.BackColor = Color.Transparent;
			this.lab_duressRange.Enabled = false;
			this.lab_duressRange.ForeColor = SystemColors.HotTrack;
			this.lab_duressRange.Location = new Point(256, 19);
			this.lab_duressRange.Name = "lab_duressRange";
			this.lab_duressRange.Size = new Size(142, 12);
			this.lab_duressRange.TabIndex = 75;
			this.lab_duressRange.Text = "(最大6位整数)";
			this.lbl_forceNewPwd.BackColor = Color.Transparent;
			this.lbl_forceNewPwd.Enabled = false;
			this.lbl_forceNewPwd.Location = new Point(12, 19);
			this.lbl_forceNewPwd.Name = "lbl_forceNewPwd";
			this.lbl_forceNewPwd.Size = new Size(120, 12);
			this.lbl_forceNewPwd.TabIndex = 77;
			this.lbl_forceNewPwd.Text = "新密码";
			this.lbl_forceNewPwd.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(206, 91);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(307, 91);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(401, 127);
			base.Controls.Add(this.txt_forceNewPwdOk);
			base.Controls.Add(this.txt_forceNewPwd);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lbl_forceNewPwdOk);
			base.Controls.Add(this.lab_duressRange);
			base.Controls.Add(this.lbl_forceNewPwd);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DuressPWForm";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "胁迫密码";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
