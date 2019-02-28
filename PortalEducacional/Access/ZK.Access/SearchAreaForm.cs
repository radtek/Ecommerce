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
	public class SearchAreaForm : Office2007Form
	{
		private IContainer components = null;

		private Label lbl_AreaCode;

		private Label lbl_AreaName;

		private Label lbl_Remarks;

		private TextBox txt_AreaCode;

		private TextBox txt_AreaName;

		private TextBox txt_Remarks;

		private ButtonX btn_OK;

		private ButtonX btn_Cancel;

		public event EventHandler SearchAreaEvent = null;

		public SearchAreaForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			try
			{
				PersonnelArea personnelArea = new PersonnelArea();
				if (!string.IsNullOrEmpty(this.txt_AreaCode.Text) && CheckInfo.IsNumber(this.txt_AreaCode.Text))
				{
					personnelArea.areaid = this.txt_AreaCode.Text;
				}
				if (!string.IsNullOrEmpty(this.txt_AreaName.Text))
				{
					personnelArea.areaname = this.txt_AreaName.Text;
				}
				if (!string.IsNullOrEmpty(this.txt_Remarks.Text))
				{
					personnelArea.remark = this.txt_Remarks.Text;
				}
				if (this.SearchAreaEvent != null)
				{
					this.SearchAreaEvent(personnelArea, null);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SearchAreaForm));
			this.lbl_AreaCode = new Label();
			this.lbl_AreaName = new Label();
			this.lbl_Remarks = new Label();
			this.txt_AreaCode = new TextBox();
			this.txt_AreaName = new TextBox();
			this.txt_Remarks = new TextBox();
			this.btn_OK = new ButtonX();
			this.btn_Cancel = new ButtonX();
			base.SuspendLayout();
			this.lbl_AreaCode.Location = new Point(8, 16);
			this.lbl_AreaCode.Name = "lbl_AreaCode";
			this.lbl_AreaCode.Size = new Size(133, 12);
			this.lbl_AreaCode.TabIndex = 0;
			this.lbl_AreaCode.Text = "区域编号";
			this.lbl_AreaCode.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_AreaName.Location = new Point(8, 47);
			this.lbl_AreaName.Name = "lbl_AreaName";
			this.lbl_AreaName.Size = new Size(133, 12);
			this.lbl_AreaName.TabIndex = 1;
			this.lbl_AreaName.Text = "区域名称";
			this.lbl_AreaName.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_Remarks.Location = new Point(8, 76);
			this.lbl_Remarks.Name = "lbl_Remarks";
			this.lbl_Remarks.Size = new Size(133, 12);
			this.lbl_Remarks.TabIndex = 2;
			this.lbl_Remarks.Text = "备注";
			this.lbl_Remarks.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_AreaCode.Location = new Point(145, 12);
			this.txt_AreaCode.Name = "txt_AreaCode";
			this.txt_AreaCode.Size = new Size(139, 21);
			this.txt_AreaCode.TabIndex = 0;
			this.txt_AreaName.Location = new Point(145, 43);
			this.txt_AreaName.Name = "txt_AreaName";
			this.txt_AreaName.Size = new Size(139, 21);
			this.txt_AreaName.TabIndex = 1;
			this.txt_Remarks.Location = new Point(145, 72);
			this.txt_Remarks.Name = "txt_Remarks";
			this.txt_Remarks.Size = new Size(139, 21);
			this.txt_Remarks.TabIndex = 2;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(115, 108);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(75, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.Location = new Point(208, 108);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(75, 23);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 4;
			this.btn_Cancel.Text = "取消";
			this.btn_Cancel.Click += this.btn_Cancel_Click;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(303, 150);
			base.Controls.Add(this.txt_Remarks);
			base.Controls.Add(this.txt_AreaName);
			base.Controls.Add(this.txt_AreaCode);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lbl_Remarks);
			base.Controls.Add(this.lbl_AreaName);
			base.Controls.Add(this.lbl_AreaCode);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SearchAreaForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "查找区域";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
