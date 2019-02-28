/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ZK.Access
{
	public class FrmSimpleProgress : Office2007Form
	{
		private IContainer components = null;

		private ProgressBar progressBar1;

		public FrmSimpleProgress()
		{
			this.InitializeComponent();
		}

		public void setTitulo(string str)
		{
			this.Text = str;
		}

		public void setProgressBar(int min, int max)
		{
			this.progressBar1.Minimum = min;
			this.progressBar1.Maximum = max;
			this.progressBar1.Value = min;
		}

		public void stepProgressBar()
		{
			if (this.progressBar1.Value < this.progressBar1.Maximum)
			{
				this.progressBar1.Value++;
			}
			Application.DoEvents();
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
			this.progressBar1 = new ProgressBar();
			base.SuspendLayout();
			this.progressBar1.Location = new Point(12, 12);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new Size(443, 23);
			this.progressBar1.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(467, 49);
			base.ControlBox = false;
			base.Controls.Add(this.progressBar1);
			base.Name = "FrmSimpleProgress";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Progresso";
			base.ResumeLayout(false);
		}
	}
}
