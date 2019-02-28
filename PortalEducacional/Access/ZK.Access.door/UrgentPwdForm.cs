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
using ZK.Utils;

namespace ZK.Access.door
{
	public class UrgentPwdForm : Office2007Form
	{
		private bool presence = false;

		private string password;

		private IContainer components = null;

		private TextBox txt_urgentNewOk;

		private TextBox txt_urgentNewPwd;

		private Label lbl_urgentNewOk;

		private Label lbl_emergencyData;

		private Label lbl_urgentNewPwd;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		public event EventHandler refreshDataEvent = null;

		public UrgentPwdForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public UrgentPwdForm(bool urgent, string Value)
			: this()
		{
			this.presence = urgent;
			this.password = Value;
			if (this.presence)
			{
				this.txt_urgentNewPwd.Enabled = true;
				this.txt_urgentNewOk.Enabled = true;
				this.lbl_urgentNewPwd.Enabled = true;
				this.lbl_urgentNewOk.Enabled = true;
				this.lbl_emergencyData.Enabled = true;
			}
			else
			{
				this.txt_urgentNewPwd.Enabled = true;
				this.txt_urgentNewOk.Enabled = true;
				this.lbl_urgentNewPwd.Enabled = true;
				this.lbl_urgentNewOk.Enabled = true;
				this.lbl_emergencyData.Enabled = true;
			}
		}

		private void txt_UrgentNewOk_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txt_urgentNewPwd.Text))
			{
				this.txt_urgentNewPwd.Focus();
			}
		}

		private bool check()
		{
			if (this.txt_urgentNewPwd.Text != this.txt_urgentNewOk.Text)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PwdError", "新密码与确认密码不一致!"));
				return false;
			}
			return true;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.refreshDataEvent != null)
			{
				this.refreshDataEvent(this.txt_urgentNewOk.Text, null);
				base.Close();
			}
		}

		private void txt_urgentPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 8);
		}

		private void txt_urgentNewPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 8);
		}

		private void txt_urgentNewOk_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 8);
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_urgentNewPwd_Leave(object sender, EventArgs e)
		{
			if (0 < this.txt_urgentNewPwd.Text.Length && this.txt_urgentNewPwd.Text.Length < 8)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("PwdPut8", "紧紧状态密码必须达到8位整数!"));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UrgentPwdForm));
			this.txt_urgentNewOk = new TextBox();
			this.txt_urgentNewPwd = new TextBox();
			this.lbl_urgentNewOk = new Label();
			this.lbl_emergencyData = new Label();
			this.lbl_urgentNewPwd = new Label();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			base.SuspendLayout();
			this.txt_urgentNewOk.Enabled = false;
			this.txt_urgentNewOk.Location = new Point(145, 44);
			this.txt_urgentNewOk.Name = "txt_urgentNewOk";
			this.txt_urgentNewOk.PasswordChar = '*';
			this.txt_urgentNewOk.Size = new Size(110, 21);
			this.txt_urgentNewOk.TabIndex = 2;
			this.txt_urgentNewOk.Click += this.txt_UrgentNewOk_Click;
			this.txt_urgentNewOk.KeyPress += this.txt_urgentNewOk_KeyPress;
			this.txt_urgentNewOk.Leave += this.txt_urgentNewPwd_Leave;
			this.txt_urgentNewPwd.Enabled = false;
			this.txt_urgentNewPwd.Location = new Point(145, 15);
			this.txt_urgentNewPwd.Name = "txt_urgentNewPwd";
			this.txt_urgentNewPwd.PasswordChar = '*';
			this.txt_urgentNewPwd.Size = new Size(110, 21);
			this.txt_urgentNewPwd.TabIndex = 1;
			this.txt_urgentNewPwd.KeyPress += this.txt_urgentNewPwd_KeyPress;
			this.txt_urgentNewPwd.Leave += this.txt_urgentNewPwd_Leave;
			this.lbl_urgentNewOk.BackColor = Color.Transparent;
			this.lbl_urgentNewOk.Enabled = false;
			this.lbl_urgentNewOk.Location = new Point(12, 47);
			this.lbl_urgentNewOk.Name = "lbl_urgentNewOk";
			this.lbl_urgentNewOk.Size = new Size(120, 12);
			this.lbl_urgentNewOk.TabIndex = 79;
			this.lbl_urgentNewOk.Text = "确认密码";
			this.lbl_urgentNewOk.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_emergencyData.BackColor = Color.Transparent;
			this.lbl_emergencyData.Enabled = false;
			this.lbl_emergencyData.ForeColor = SystemColors.HotTrack;
			this.lbl_emergencyData.Location = new Point(262, 18);
			this.lbl_emergencyData.Name = "lbl_emergencyData";
			this.lbl_emergencyData.Size = new Size(137, 12);
			this.lbl_emergencyData.TabIndex = 76;
			this.lbl_emergencyData.Text = "(只能8位整数)";
			this.lbl_urgentNewPwd.BackColor = Color.Transparent;
			this.lbl_urgentNewPwd.Enabled = false;
			this.lbl_urgentNewPwd.Location = new Point(12, 18);
			this.lbl_urgentNewPwd.Name = "lbl_urgentNewPwd";
			this.lbl_urgentNewPwd.Size = new Size(120, 12);
			this.lbl_urgentNewPwd.TabIndex = 78;
			this.lbl_urgentNewPwd.Text = "新密码";
			this.lbl_urgentNewPwd.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(216, 86);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(318, 86);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(412, 123);
			base.Controls.Add(this.txt_urgentNewOk);
			base.Controls.Add(this.txt_urgentNewPwd);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lbl_urgentNewOk);
			base.Controls.Add(this.lbl_emergencyData);
			base.Controls.Add(this.lbl_urgentNewPwd);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UrgentPwdForm";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "紧急状态密码";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
