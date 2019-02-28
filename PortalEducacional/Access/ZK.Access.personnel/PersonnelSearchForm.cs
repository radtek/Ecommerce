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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.personnel
{
	public class PersonnelSearchForm : Office2007Form
	{
		public string ConditionStr;

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		private TextBox txt_cardNo;

		private TextBox txt_userName;

		private TextBox txt_userNo;

		private Label lbl_sex;

		private Label lbl_cardno;

		private Label lbl_userName;

		private Label lbl_userNo;

		private TextBox txt_deptName;

		private TextBox txt_deptNo;

		private Label lb_deptName;

		private Label lb_deptNo;

		private ComboBox cbo_gender;

		private CheckBox checkBox1;

		public PersonnelSearchForm(bool showVisitOptions = false)
		{
			this.InitializeComponent();
			this.GenderType();
			this.DataStaff();
			initLang.LocaleForm(this, base.Name);
			this.checkBox1.Visible = showVisitOptions;
		}

		public bool getOnlyActive()
		{
			return this.checkBox1.Checked;
		}

		private void GenderType()
		{
			try
			{
				this.m_gender = initLang.GetComboxInfo("gender");
				if (this.m_gender == null || this.m_gender.Count == 0)
				{
					this.m_gender = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("M", "男");
					dictionary.Add("F", "女");
					initLang.SetComboxInfo("gender", this.m_gender);
					initLang.Save();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataStaff()
		{
			try
			{
				this.cbo_gender.Items.Add("-----");
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						this.cbo_gender.Items.Add(item.Value);
					}
				}
				if (this.cbo_gender.Items.Count > 0)
				{
					this.cbo_gender.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("and USERID>0 ");
			if (!string.IsNullOrEmpty(this.txt_userNo.Text))
			{
				stringBuilder.Append(" and Badgenumber like'%" + this.txt_userNo.Text + "%'");
			}
			if (!string.IsNullOrEmpty(this.txt_userName.Text))
			{
				stringBuilder.Append(" and Name like'%" + this.txt_userName.Text + "%'");
			}
			if (!string.IsNullOrEmpty(this.txt_cardNo.Text))
			{
				stringBuilder.Append(" and CardNo like'%" + this.txt_cardNo.Text + "%'");
			}
			if (this.cbo_gender.Text != "-----")
			{
				int num = this.cbo_gender.SelectedIndex - 1;
				string empty = string.Empty;
				empty = ((num != 0) ? "F" : "M");
				stringBuilder.Append(" and Gender ='" + empty + "'");
			}
			if (!string.IsNullOrEmpty(this.txt_deptNo.Text))
			{
				stringBuilder.Append(" and departments.code like'%" + this.txt_deptNo.Text + "%'");
			}
			if (!string.IsNullOrEmpty(this.txt_deptName.Text))
			{
				stringBuilder.Append(" and departments.DEPTNAME like'%" + this.txt_deptName.Text + "%'");
			}
			this.ConditionStr = stringBuilder.ToString();
			base.DialogResult = DialogResult.OK;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_userNo_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
			CheckInfo.NumberKeyPress(sender, e, 1, 999999999L);
		}

		private void txt_userName_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
			CheckInfo.KeyPress(sender, e, 40);
		}

		private void txt_cardNo_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (AccCommon.CodeVersion)
			{
			case CodeVersionType.JapanAF:
				if (e.KeyChar != '\b' && e.KeyChar != '\r' && (e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'A' || e.KeyChar > 'F') && (e.KeyChar < 'a' || e.KeyChar > 'f'))
				{
					e.Handled = true;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SHexNumberOnly", "仅支持输入十六进制数"));
				}
				else if (e.KeyChar == '0' && this.txt_cardNo.SelectionStart == 0 && this.txt_cardNo.TextLength > this.txt_cardNo.SelectionLength)
				{
					e.Handled = true;
				}
				else if (this.txt_cardNo.Text == "0" && e.KeyChar != '\b' && e.KeyChar != '\r' && this.txt_cardNo.SelectionLength == 0 && this.txt_cardNo.SelectionStart == 1)
				{
					this.txt_cardNo.Text = e.KeyChar.ToString();
					this.txt_cardNo.SelectionStart = 1;
					e.Handled = true;
				}
				else if (this.txt_cardNo.SelectionLength == 0 && this.txt_cardNo.Text.Length >= 16 && e.KeyChar != '\b' && e.KeyChar != '\r')
				{
					e.Handled = true;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputIsOutLen", "输入超出有效长度"));
				}
				break;
			case CodeVersionType.Main:
				CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
				break;
			default:
				CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
				break;
			}
		}

		private void cbo_gender_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
		}

		private void txt_deptNo_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_deptName_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
			CheckInfo.KeyPress(sender, e);
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
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			this.txt_cardNo = new TextBox();
			this.txt_userName = new TextBox();
			this.txt_userNo = new TextBox();
			this.lbl_sex = new Label();
			this.lbl_cardno = new Label();
			this.lbl_userName = new Label();
			this.lbl_userNo = new Label();
			this.txt_deptName = new TextBox();
			this.txt_deptNo = new TextBox();
			this.lb_deptName = new Label();
			this.lb_deptNo = new Label();
			this.cbo_gender = new ComboBox();
			this.checkBox1 = new CheckBox();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(421, 141);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 23;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(321, 141);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 25);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 22;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.txt_cardNo.Location = new Point(101, 60);
			this.txt_cardNo.Name = "txt_cardNo";
			this.txt_cardNo.Size = new Size(135, 20);
			this.txt_cardNo.TabIndex = 16;
			this.txt_cardNo.KeyPress += this.txt_cardNo_KeyPress;
			this.txt_userName.Location = new Point(368, 20);
			this.txt_userName.Name = "txt_userName";
			this.txt_userName.Size = new Size(135, 20);
			this.txt_userName.TabIndex = 15;
			this.txt_userName.KeyPress += this.txt_userName_KeyPress;
			this.txt_userNo.Location = new Point(102, 20);
			this.txt_userNo.Name = "txt_userNo";
			this.txt_userNo.Size = new Size(135, 20);
			this.txt_userNo.TabIndex = 14;
			this.txt_userNo.KeyPress += this.txt_userNo_KeyPress;
			this.lbl_sex.AutoSize = true;
			this.lbl_sex.Location = new Point(263, 64);
			this.lbl_sex.Name = "lbl_sex";
			this.lbl_sex.Size = new Size(31, 13);
			this.lbl_sex.TabIndex = 13;
			this.lbl_sex.Text = "性别";
			this.lbl_cardno.AutoSize = true;
			this.lbl_cardno.Location = new Point(14, 64);
			this.lbl_cardno.Name = "lbl_cardno";
			this.lbl_cardno.Size = new Size(31, 13);
			this.lbl_cardno.TabIndex = 12;
			this.lbl_cardno.Text = "卡号";
			this.lbl_userName.AutoSize = true;
			this.lbl_userName.Location = new Point(263, 24);
			this.lbl_userName.Name = "lbl_userName";
			this.lbl_userName.Size = new Size(31, 13);
			this.lbl_userName.TabIndex = 11;
			this.lbl_userName.Text = "姓名";
			this.lbl_userNo.AutoSize = true;
			this.lbl_userNo.Location = new Point(14, 24);
			this.lbl_userNo.Name = "lbl_userNo";
			this.lbl_userNo.Size = new Size(55, 13);
			this.lbl_userNo.TabIndex = 10;
			this.lbl_userNo.Text = "人员编号";
			this.txt_deptName.Location = new Point(368, 100);
			this.txt_deptName.Name = "txt_deptName";
			this.txt_deptName.Size = new Size(135, 20);
			this.txt_deptName.TabIndex = 19;
			this.txt_deptName.KeyPress += this.txt_deptName_KeyPress;
			this.txt_deptNo.Location = new Point(101, 100);
			this.txt_deptNo.Name = "txt_deptNo";
			this.txt_deptNo.Size = new Size(135, 20);
			this.txt_deptNo.TabIndex = 18;
			this.txt_deptNo.KeyPress += this.txt_deptNo_KeyPress;
			this.lb_deptName.AutoSize = true;
			this.lb_deptName.Location = new Point(263, 104);
			this.lb_deptName.Name = "lb_deptName";
			this.lb_deptName.Size = new Size(55, 13);
			this.lb_deptName.TabIndex = 21;
			this.lb_deptName.Text = "部门名称";
			this.lb_deptNo.AutoSize = true;
			this.lb_deptNo.Location = new Point(14, 104);
			this.lb_deptNo.Name = "lb_deptNo";
			this.lb_deptNo.Size = new Size(55, 13);
			this.lb_deptNo.TabIndex = 20;
			this.lb_deptNo.Text = "部门编号";
			this.cbo_gender.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_gender.FormattingEnabled = true;
			this.cbo_gender.Location = new Point(368, 61);
			this.cbo_gender.Name = "cbo_gender";
			this.cbo_gender.Size = new Size(135, 21);
			this.cbo_gender.TabIndex = 17;
			this.cbo_gender.KeyPress += this.cbo_gender_KeyPress;
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new Point(102, 141);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new Size(133, 17);
			this.checkBox1.TabIndex = 24;
			this.checkBox1.Text = "Somente Visitas Ativas";
			this.checkBox1.UseVisualStyleBackColor = true;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(515, 179);
			base.Controls.Add(this.checkBox1);
			base.Controls.Add(this.txt_deptName);
			base.Controls.Add(this.cbo_gender);
			base.Controls.Add(this.txt_userName);
			base.Controls.Add(this.txt_deptNo);
			base.Controls.Add(this.txt_cardNo);
			base.Controls.Add(this.txt_userNo);
			base.Controls.Add(this.lb_deptName);
			base.Controls.Add(this.lb_deptNo);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lbl_sex);
			base.Controls.Add(this.lbl_cardno);
			base.Controls.Add(this.lbl_userName);
			base.Controls.Add(this.lbl_userNo);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PersonnelSearchForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = " 查找";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
