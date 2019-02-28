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
	public class FrmYesNo : Office2007Form
	{
		private bool _accept = false;

		private IContainer components = null;

		private Label label1;

		private ButtonX btn_OK;

		private ButtonX btn_Cancel;

		private Timer timer1;

		private Timer timer2;

		public FrmYesNo(string mensagem)
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.label1.Text = mensagem;
		}

		public bool isAccepted()
		{
			return this._accept;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			this._accept = true;
			base.Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this._accept = false;
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
			this.components = new Container();
			this.label1 = new Label();
			this.btn_OK = new ButtonX();
			this.btn_Cancel = new ButtonX();
			this.timer1 = new Timer(this.components);
			this.timer2 = new Timer(this.components);
			base.SuspendLayout();
			this.label1.Location = new Point(5, 12);
			this.label1.Name = "label1";
			this.label1.Size = new Size(332, 86);
			this.label1.TabIndex = 0;
			this.label1.Text = "mensaenm";
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(12, 101);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(152, 25);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 4;
			this.btn_OK.Text = "Sim";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.DialogResult = DialogResult.Cancel;
			this.btn_Cancel.Location = new Point(181, 101);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(152, 25);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 5;
			this.btn_Cancel.Text = "Não";
			this.btn_Cancel.Click += this.btn_Cancel_Click;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.btn_Cancel;
			base.ClientSize = new Size(349, 138);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.label1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FrmYesNo";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.Manual;
			this.Text = "Pergunta";
			base.ResumeLayout(false);
		}
	}
}
