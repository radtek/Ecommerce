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

namespace ZK.Access
{
	public class DataConfigSet : Office2007Form
	{
		private int m_dataType = 0;

		private IContainer components = null;

		private PanelEx pnl_bottom;

		private ButtonX btn_cancel;

		private ButtonX btn_Next;

		private PanelEx pnl_excel;

		private Label label1;

		private RadioButton rd_sqlserver;

		private RadioButton rd_access;

		public int DataType => this.m_dataType;

		public DataConfigSet()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.m_dataType = 0;
			if (AppSite.Instance.DataType == ZK.ConfigManager.DataType.Access)
			{
				this.rd_access.Checked = true;
			}
			else
			{
				this.rd_sqlserver.Checked = true;
			}
		}

		private void btn_Next_Click(object sender, EventArgs e)
		{
			if (this.rd_access.Checked)
			{
				this.m_dataType = 1;
			}
			else
			{
				this.m_dataType = 2;
			}
			base.Close();
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.m_dataType = 0;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DataConfigSet));
			this.pnl_bottom = new PanelEx();
			this.btn_cancel = new ButtonX();
			this.btn_Next = new ButtonX();
			this.pnl_excel = new PanelEx();
			this.label1 = new Label();
			this.rd_sqlserver = new RadioButton();
			this.rd_access = new RadioButton();
			this.pnl_bottom.SuspendLayout();
			this.pnl_excel.SuspendLayout();
			base.SuspendLayout();
			this.pnl_bottom.CanvasColor = SystemColors.Control;
			this.pnl_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_bottom.Controls.Add(this.btn_cancel);
			this.pnl_bottom.Controls.Add(this.btn_Next);
			this.pnl_bottom.Dock = DockStyle.Bottom;
			this.pnl_bottom.Location = new Point(0, 123);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(291, 50);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 1;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(197, 15);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_Next.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Next.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Next.Location = new Point(95, 15);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(82, 23);
			this.btn_Next.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Next.TabIndex = 2;
			this.btn_Next.Text = "下一步";
			this.btn_Next.Click += this.btn_Next_Click;
			this.pnl_excel.CanvasColor = SystemColors.Control;
			this.pnl_excel.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_excel.Controls.Add(this.label1);
			this.pnl_excel.Controls.Add(this.rd_sqlserver);
			this.pnl_excel.Controls.Add(this.rd_access);
			this.pnl_excel.Dock = DockStyle.Fill;
			this.pnl_excel.Location = new Point(0, 0);
			this.pnl_excel.Name = "pnl_excel";
			this.pnl_excel.Size = new Size(291, 123);
			this.pnl_excel.Style.Alignment = StringAlignment.Center;
			this.pnl_excel.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_excel.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_excel.Style.Border = eBorderType.SingleLine;
			this.pnl_excel.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_excel.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_excel.Style.GradientAngle = 90;
			this.pnl_excel.TabIndex = 2;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new Size(41, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "数据库";
			this.rd_sqlserver.AutoSize = true;
			this.rd_sqlserver.Location = new Point(58, 83);
			this.rd_sqlserver.Name = "rd_sqlserver";
			this.rd_sqlserver.Size = new Size(149, 16);
			this.rd_sqlserver.TabIndex = 1;
			this.rd_sqlserver.Text = "Microsoft SQL Server ";
			this.rd_sqlserver.UseVisualStyleBackColor = true;
			this.rd_access.AutoSize = true;
			this.rd_access.Checked = true;
			this.rd_access.Location = new Point(58, 48);
			this.rd_access.Name = "rd_access";
			this.rd_access.Size = new Size(59, 16);
			this.rd_access.TabIndex = 0;
			this.rd_access.TabStop = true;
			this.rd_access.Text = "Access";
			this.rd_access.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(291, 173);
			base.Controls.Add(this.pnl_excel);
			base.Controls.Add(this.pnl_bottom);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DataConfigSet";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "数据库";
			this.pnl_bottom.ResumeLayout(false);
			this.pnl_excel.ResumeLayout(false);
			this.pnl_excel.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
