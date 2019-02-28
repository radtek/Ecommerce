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
	public class AdjustDeptForm : Office2007Form
	{
		private int deptid;

		private int m_id = 0;

		private IContainer components = null;

		private Label label1;

		private ButtonX btn_Ok;

		private ButtonX btn_cancel;

		private Label label4;

		private TextBox txt_SelectedEmployee;

		private ComboBox cbo_dept;

		private Label label5;

		private Label label6;

		public event EventHandler RefreshDataEvent = null;

		public AdjustDeptForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public AdjustDeptForm(string personnelNO)
			: this()
		{
			this.txt_SelectedEmployee.Text = personnelNO;
			this.txt_SelectedEmployee.Enabled = false;
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void cbo_dept_Click(object sender, EventArgs e)
		{
			CboDeptTreeForm cboDeptTreeForm = new CboDeptTreeForm();
			cboDeptTreeForm.SelectDeptEvent += this.frm_SelectDeptEvent;
			cboDeptTreeForm.ShowDialog();
			cboDeptTreeForm.SelectDeptEvent -= this.frm_SelectDeptEvent;
		}

		private void frm_SelectDeptEvent(object sender, EventArgs e)
		{
			if (sender != null)
			{
				Departments departments = sender as Departments;
				if (departments != null)
				{
					this.cbo_dept.Items.Add(departments.DEPTNAME);
					this.cbo_dept.Text = departments.DEPTNAME;
					this.cbo_dept.Text = departments.DEPTNAME;
					this.deptid = departments.DEPTID;
				}
			}
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.cbo_dept.Text))
			{
				if (this.RefreshDataEvent != null)
				{
					this.RefreshDataEvent(this.deptid, null);
				}
				base.Close();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("chooseDept", "请选择部门!"));
				this.cbo_dept.Focus();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AdjustDeptForm));
			this.label1 = new Label();
			this.btn_Ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.label4 = new Label();
			this.txt_SelectedEmployee = new TextBox();
			this.cbo_dept = new ComboBox();
			this.label5 = new Label();
			this.label6 = new Label();
			base.SuspendLayout();
			this.label1.Location = new Point(10, 48);
			this.label1.Name = "label1";
			this.label1.Size = new Size(167, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "调整到的部门";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(183, 90);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 2;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(277, 90);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.label4.Location = new Point(12, 19);
			this.label4.Name = "label4";
			this.label4.Size = new Size(165, 12);
			this.label4.TabIndex = 5;
			this.label4.Text = "选择人员";
			this.label4.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_SelectedEmployee.Location = new Point(183, 15);
			this.txt_SelectedEmployee.Name = "txt_SelectedEmployee";
			this.txt_SelectedEmployee.Size = new Size(161, 21);
			this.txt_SelectedEmployee.TabIndex = 0;
			this.cbo_dept.FormattingEnabled = true;
			this.cbo_dept.Location = new Point(183, 44);
			this.cbo_dept.Name = "cbo_dept";
			this.cbo_dept.Size = new Size(161, 20);
			this.cbo_dept.TabIndex = 1;
			this.cbo_dept.Click += this.cbo_dept_Click;
			this.label5.AutoSize = true;
			this.label5.ForeColor = Color.Red;
			this.label5.Location = new Point(348, 48);
			this.label5.Name = "label5";
			this.label5.Size = new Size(11, 12);
			this.label5.TabIndex = 8;
			this.label5.Text = "*";
			this.label6.AutoSize = true;
			this.label6.ForeColor = Color.Red;
			this.label6.Location = new Point(348, 19);
			this.label6.Name = "label6";
			this.label6.Size = new Size(11, 12);
			this.label6.TabIndex = 7;
			this.label6.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(371, 125);
			base.Controls.Add(this.cbo_dept);
			base.Controls.Add(this.txt_SelectedEmployee);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AdjustDeptForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "调整部门";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
