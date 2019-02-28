/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ZK.Access.mensage
{
	public class Mensage : Form
	{
		private int wait = 0;

		private string initialMsg = "";

		private IContainer components = null;

		private Label lb_msg;

		public Mensage(int w, int h, string title, string msg, bool closable, bool firsplase, int waitTime)
		{
			this.InitializeComponent();
			base.Width = w;
			base.Height = h;
			this.Text = title;
			this.initialMsg = msg;
			this.lb_msg.Text = msg;
			base.CenterToScreen();
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Shown += this.Form1_Shown;
			this.wait = waitTime;
			if (!closable)
			{
				base.FormBorderStyle = FormBorderStyle.None;
			}
			if (firsplase)
			{
				base.TopMost = true;
			}
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			if (this.wait != 0)
			{
				while (this.wait > 0)
				{
					Thread.Sleep(1000);
					this.wait--;
					this.lb_msg.Text = this.initialMsg + "\n" + this.wait + " Segundos para a finização deste processo.";
				}
				base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Mensage));
			this.lb_msg = new Label();
			base.SuspendLayout();
			this.lb_msg.AutoSize = true;
			this.lb_msg.Location = new Point(31, 24);
			this.lb_msg.Name = "lb_msg";
			this.lb_msg.Size = new Size(26, 13);
			this.lb_msg.TabIndex = 0;
			this.lb_msg.Text = "msg";
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(620, 217);
			base.Controls.Add(this.lb_msg);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Mensage";
			this.Text = "Mensagem";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
