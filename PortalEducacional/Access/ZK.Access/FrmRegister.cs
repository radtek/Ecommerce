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

namespace ZK.Access
{
	public class FrmRegister : Office2007Form
	{
		private int m_days = 0;

		private string m_applyN0 = string.Empty;

		private string m_sn = string.Empty;

		private bool m_isRegister = false;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		private Label lb_info;

		private Label lb_applyNo;

		private TextBox txt_no;

		private TextBox txt_1;

		private TextBox txt_2;

		private TextBox txt_3;

		private TextBox txt_4;

		private TextBox txt_5;

		private Label lb_1;

		private Label lb_2;

		private Label lb_3;

		private Label lb_4;

		private CheckBox ch_registerInfo;

		private Label lb_register;

		private ButtonX btn_using;

		public bool IsRegister => this.m_isRegister;

		public FrmRegister(int days, string applyNo, string sn)
		{
			this.InitializeComponent();
			this.m_days = days;
			this.m_applyN0 = applyNo;
			this.m_sn = sn;
			initLang.LocaleForm(this, base.Name);
			this.lb_info.Text = this.lb_info.Text.Replace("{0}", days.ToString());
			this.txt_no.Text = applyNo;
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			this.ch_registerInfo_CheckedChanged(null, null);
			string text = this.txt_1.Text;
			text = text.ToUpper();
			if (text == this.m_sn)
			{
				AppSite.Instance.SetNodeValue("SN", this.m_sn);
				this.m_isRegister = true;
				base.Close();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("NoValidSN", "不是有效的注册码"));
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.ch_registerInfo_CheckedChanged(null, null);
			base.Close();
		}

		private void ch_registerInfo_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ch_registerInfo.Checked)
			{
				AppSite.Instance.SetNodeValue("ShowRegisterInfo", "0");
			}
			else
			{
				AppSite.Instance.SetNodeValue("ShowRegisterInfo", "1");
			}
		}

		private void txt_1_KeyPress(object sender, KeyPressEventArgs e)
		{
			int num;
			if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'A' || e.KeyChar > 'z') && e.KeyChar != '\b')
			{
				num = ((e.KeyChar == '\r') ? 1 : 0);
				goto IL_003f;
			}
			num = 1;
			goto IL_003f;
			IL_003f:
			if (num != 0)
			{
				TextBox textBox = sender as TextBox;
				if (textBox != null && textBox.TextLength >= 29 && textBox.SelectionLength == 0 && e.KeyChar != '\b' && e.KeyChar != '\r')
				{
					e.Handled = true;
					if (textBox == this.txt_1)
					{
						this.txt_2.Focus();
					}
					else if (textBox == this.txt_2)
					{
						this.txt_3.Focus();
					}
					else if (textBox == this.txt_3)
					{
						this.txt_4.Focus();
					}
					else if (textBox == this.txt_4)
					{
						this.txt_5.Focus();
					}
					else if (textBox == this.txt_5)
					{
						this.btn_ok.Focus();
					}
				}
			}
			else
			{
				e.Handled = true;
			}
		}

		private void btn_using_Click(object sender, EventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FrmRegister));
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			this.lb_info = new Label();
			this.lb_applyNo = new Label();
			this.txt_no = new TextBox();
			this.txt_1 = new TextBox();
			this.txt_2 = new TextBox();
			this.txt_3 = new TextBox();
			this.txt_4 = new TextBox();
			this.txt_5 = new TextBox();
			this.lb_1 = new Label();
			this.lb_2 = new Label();
			this.lb_3 = new Label();
			this.lb_4 = new Label();
			this.ch_registerInfo = new CheckBox();
			this.lb_register = new Label();
			this.btn_using = new ButtonX();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(510, 238);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(120, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 9;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(252, 238);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(120, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 8;
			this.btn_ok.Text = "注册";
			this.btn_ok.Click += this.btn_ok_Click;
			this.lb_info.Location = new Point(12, 20);
			this.lb_info.Name = "lb_info";
			this.lb_info.Size = new Size(616, 12);
			this.lb_info.TabIndex = 27;
			this.lb_info.Text = "当前产品为试用版,试用期还剩:{0}天";
			this.lb_applyNo.Location = new Point(12, 48);
			this.lb_applyNo.Name = "lb_applyNo";
			this.lb_applyNo.Size = new Size(616, 12);
			this.lb_applyNo.TabIndex = 28;
			this.lb_applyNo.Text = "申请号:";
			this.txt_no.BackColor = SystemColors.Window;
			this.txt_no.Location = new Point(14, 63);
			this.txt_no.Multiline = true;
			this.txt_no.Name = "txt_no";
			this.txt_no.ReadOnly = true;
			this.txt_no.Size = new Size(616, 57);
			this.txt_no.TabIndex = 0;
			this.txt_1.Location = new Point(14, 147);
			this.txt_1.Name = "txt_1";
			this.txt_1.Size = new Size(308, 21);
			this.txt_1.TabIndex = 2;
			this.txt_1.TextAlign = HorizontalAlignment.Center;
			this.txt_1.KeyPress += this.txt_1_KeyPress;
			this.txt_2.Location = new Point(327, 204);
			this.txt_2.Name = "txt_2";
			this.txt_2.Size = new Size(50, 21);
			this.txt_2.TabIndex = 3;
			this.txt_2.TextAlign = HorizontalAlignment.Center;
			this.txt_2.Visible = false;
			this.txt_2.KeyPress += this.txt_1_KeyPress;
			this.txt_3.Location = new Point(397, 204);
			this.txt_3.Name = "txt_3";
			this.txt_3.Size = new Size(50, 21);
			this.txt_3.TabIndex = 4;
			this.txt_3.TextAlign = HorizontalAlignment.Center;
			this.txt_3.Visible = false;
			this.txt_3.KeyPress += this.txt_1_KeyPress;
			this.txt_4.Location = new Point(467, 204);
			this.txt_4.Name = "txt_4";
			this.txt_4.Size = new Size(50, 21);
			this.txt_4.TabIndex = 5;
			this.txt_4.TextAlign = HorizontalAlignment.Center;
			this.txt_4.Visible = false;
			this.txt_4.KeyPress += this.txt_1_KeyPress;
			this.txt_5.Location = new Point(537, 205);
			this.txt_5.Name = "txt_5";
			this.txt_5.Size = new Size(50, 21);
			this.txt_5.TabIndex = 6;
			this.txt_5.TextAlign = HorizontalAlignment.Center;
			this.txt_5.Visible = false;
			this.txt_5.KeyPress += this.txt_1_KeyPress;
			this.lb_1.AutoSize = true;
			this.lb_1.Location = new Point(311, 208);
			this.lb_1.Name = "lb_1";
			this.lb_1.Size = new Size(11, 12);
			this.lb_1.TabIndex = 36;
			this.lb_1.Text = "-";
			this.lb_1.Visible = false;
			this.lb_2.AutoSize = true;
			this.lb_2.Location = new Point(381, 208);
			this.lb_2.Name = "lb_2";
			this.lb_2.Size = new Size(11, 12);
			this.lb_2.TabIndex = 37;
			this.lb_2.Text = "-";
			this.lb_2.Visible = false;
			this.lb_3.AutoSize = true;
			this.lb_3.Location = new Point(451, 209);
			this.lb_3.Name = "lb_3";
			this.lb_3.Size = new Size(11, 12);
			this.lb_3.TabIndex = 38;
			this.lb_3.Text = "-";
			this.lb_3.Visible = false;
			this.lb_4.AutoSize = true;
			this.lb_4.Location = new Point(521, 209);
			this.lb_4.Name = "lb_4";
			this.lb_4.Size = new Size(11, 12);
			this.lb_4.TabIndex = 39;
			this.lb_4.Text = "-";
			this.lb_4.Visible = false;
			this.ch_registerInfo.Location = new Point(14, 179);
			this.ch_registerInfo.Name = "ch_registerInfo";
			this.ch_registerInfo.Size = new Size(308, 16);
			this.ch_registerInfo.TabIndex = 7;
			this.ch_registerInfo.Text = "下次不再提示";
			this.ch_registerInfo.UseVisualStyleBackColor = true;
			this.ch_registerInfo.CheckedChanged += this.ch_registerInfo_CheckedChanged;
			this.lb_register.Location = new Point(12, 132);
			this.lb_register.Name = "lb_register";
			this.lb_register.Size = new Size(616, 12);
			this.lb_register.TabIndex = 40;
			this.lb_register.Text = "注册码:";
			this.btn_using.AccessibleRole = AccessibleRole.PushButton;
			this.btn_using.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_using.Location = new Point(378, 238);
			this.btn_using.Name = "btn_using";
			this.btn_using.Size = new Size(120, 23);
			this.btn_using.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_using.TabIndex = 41;
			this.btn_using.Text = "试用";
			this.btn_using.Click += this.btn_using_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(642, 273);
			base.Controls.Add(this.btn_using);
			base.Controls.Add(this.lb_register);
			base.Controls.Add(this.ch_registerInfo);
			base.Controls.Add(this.lb_4);
			base.Controls.Add(this.lb_3);
			base.Controls.Add(this.lb_2);
			base.Controls.Add(this.lb_1);
			base.Controls.Add(this.txt_5);
			base.Controls.Add(this.txt_4);
			base.Controls.Add(this.txt_3);
			base.Controls.Add(this.txt_2);
			base.Controls.Add(this.txt_1);
			base.Controls.Add(this.txt_no);
			base.Controls.Add(this.lb_applyNo);
			base.Controls.Add(this.lb_info);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FrmRegister";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "产品注册";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
