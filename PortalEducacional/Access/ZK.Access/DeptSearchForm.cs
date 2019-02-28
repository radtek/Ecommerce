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
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DeptSearchForm : Office2007Form
	{
		private IContainer components = null;

		private Label lb_deptName;

		private Label lb_deptNo;

		private TextBox txt_code;

		private TextBox txt_dept;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		private Label label3;

		private Label label4;

		public event EventHandler SearchDeptEvent = null;

		public DeptSearchForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			try
			{
				Departments departments = new Departments();
				if (!string.IsNullOrEmpty(this.txt_code.Text) && CheckInfo.IsNumber(this.txt_code.Text))
				{
					departments.code = this.txt_code.Text;
				}
				if (!string.IsNullOrEmpty(this.txt_dept.Text))
				{
					departments.DEPTNAME = this.txt_dept.Text;
				}
				if (this.SearchDeptEvent != null)
				{
					this.SearchDeptEvent(departments, null);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_clear_Click(object sender, EventArgs e)
		{
			this.txt_dept.Text = "";
			this.txt_code.Text = "";
			this.txt_code.Focus();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DeptSearchForm));
			this.lb_deptName = new Label();
			this.lb_deptNo = new Label();
			this.txt_code = new TextBox();
			this.txt_dept = new TextBox();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.label3 = new Label();
			this.label4 = new Label();
			base.SuspendLayout();
			this.lb_deptName.Location = new Point(12, 21);
			this.lb_deptName.Name = "lb_deptName";
			this.lb_deptName.Size = new Size(132, 12);
			this.lb_deptName.TabIndex = 0;
			this.lb_deptName.Text = "部门编号";
			this.lb_deptName.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_deptNo.Location = new Point(12, 55);
			this.lb_deptNo.Name = "lb_deptNo";
			this.lb_deptNo.Size = new Size(132, 12);
			this.lb_deptNo.TabIndex = 1;
			this.lb_deptNo.Text = "部门名称";
			this.lb_deptNo.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_code.Location = new Point(159, 16);
			this.txt_code.Name = "txt_code";
			this.txt_code.Size = new Size(162, 21);
			this.txt_code.TabIndex = 0;
			this.txt_dept.Location = new Point(159, 50);
			this.txt_dept.Name = "txt_dept";
			this.txt_dept.Size = new Size(162, 21);
			this.txt_dept.TabIndex = 1;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(159, 99);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 2;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(253, 99);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.label3.AutoSize = true;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(324, 55);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 24;
			this.label3.Text = "*";
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(324, 20);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 25;
			this.label4.Text = "*";
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(347, 134);
			base.Controls.Add(this.txt_dept);
			base.Controls.Add(this.txt_code);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_deptNo);
			base.Controls.Add(this.lb_deptName);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DeptSearchForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "部门查找";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
