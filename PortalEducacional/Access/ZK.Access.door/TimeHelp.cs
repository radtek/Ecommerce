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

namespace ZK.Access.door
{
	public class TimeHelp : Office2007Form
	{
		private IContainer components = null;

		private ButtonX btn_cancel;

		private Label label1;

		public TimeHelp()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.label1.AutoSize = false;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TimeHelp));
			this.btn_cancel = new ButtonX();
			this.label1 = new Label();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(235, 131);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(142, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 0;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.label1.Location = new Point(18, 22);
			this.label1.Name = "label1";
			this.label1.Size = new Size(349, 59);
			this.label1.TabIndex = 1;
			this.label1.Text = "1，选择时间段后，按下\"上\",\"下\",\"左\",\"右\"或者\"W\",\"Z\",\"A\",\"D\"快捷键，可以增大，减小，左移，右移时间段。";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(389, 166);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.btn_cancel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TimeHelp";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "帮助";
			base.ResumeLayout(false);
		}
	}
}
