/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class RS485UC : UserControl
	{
		private IContainer components = null;

		private Label label1;

		private Label label2;

		private Label label3;

		private ComboBox comboBox1;

		private ComboBox comboBox2;

		private TextBox textBox1;

		public RS485UC()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
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
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.comboBox1 = new ComboBox();
			this.comboBox2 = new ComboBox();
			this.textBox1 = new TextBox();
			base.SuspendLayout();
			this.label1.Location = new Point(23, 8);
			this.label1.Name = "label1";
			this.label1.Size = new Size(111, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "串口号";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.Location = new Point(23, 36);
			this.label2.Name = "label2";
			this.label2.Size = new Size(111, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "RS485地址";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label3.Location = new Point(23, 65);
			this.label3.Name = "label3";
			this.label3.Size = new Size(111, 12);
			this.label3.TabIndex = 2;
			this.label3.Text = "波特率";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new Point(136, 3);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new Size(134, 20);
			this.comboBox1.TabIndex = 3;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Location = new Point(136, 62);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new Size(134, 20);
			this.comboBox2.TabIndex = 4;
			this.textBox1.Location = new Point(136, 32);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new Size(134, 21);
			this.textBox1.TabIndex = 5;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.Transparent;
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.comboBox2);
			base.Controls.Add(this.comboBox1);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Name = "RS485UC";
			base.Size = new Size(320, 87);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
