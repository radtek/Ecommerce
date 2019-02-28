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
	public class IssueCardForm : Office2007Form
	{
		private IContainer components = null;

		private Label lab_IssueCard;

		private TextBox txt_IssueCard;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		private Label lab_employee;

		private TextBox txt_employee;

		private Label label1;

		private Label label2;

		public IssueCardForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_IssueCard_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 20);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(IssueCardForm));
			this.lab_IssueCard = new Label();
			this.txt_IssueCard = new TextBox();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.lab_employee = new Label();
			this.txt_employee = new TextBox();
			this.label1 = new Label();
			this.label2 = new Label();
			base.SuspendLayout();
			this.lab_IssueCard.Location = new Point(12, 54);
			this.lab_IssueCard.Name = "lab_IssueCard";
			this.lab_IssueCard.Size = new Size(124, 12);
			this.lab_IssueCard.TabIndex = 6;
			this.lab_IssueCard.Text = "人员发卡";
			this.lab_IssueCard.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_IssueCard.Location = new Point(153, 49);
			this.txt_IssueCard.Name = "txt_IssueCard";
			this.txt_IssueCard.Size = new Size(162, 21);
			this.txt_IssueCard.TabIndex = 2;
			this.txt_IssueCard.KeyPress += this.txt_IssueCard_KeyPress;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(153, 96);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(247, 96);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.lab_employee.Location = new Point(12, 19);
			this.lab_employee.Name = "lab_employee";
			this.lab_employee.Size = new Size(124, 12);
			this.lab_employee.TabIndex = 5;
			this.lab_employee.Text = "选择人员";
			this.lab_employee.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_employee.Location = new Point(153, 14);
			this.txt_employee.Name = "txt_employee";
			this.txt_employee.Size = new Size(162, 21);
			this.txt_employee.TabIndex = 1;
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(318, 54);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(318, 19);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 7;
			this.label2.Text = "*";
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(341, 131);
			base.Controls.Add(this.txt_IssueCard);
			base.Controls.Add(this.txt_employee);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.lab_employee);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_IssueCard);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "IssueCardForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "人员发卡";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
