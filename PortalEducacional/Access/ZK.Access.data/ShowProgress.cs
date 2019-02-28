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
using ZK.ConfigManager;

namespace ZK.Access.data
{
	public class ShowProgress : Office2007Form
	{
		public delegate void ShowProgressHandle(int currProgress);

		public delegate void ShowInfo(string info);

		private bool m_isCanClose = false;

		private IContainer components = null;

		private ProgressBar progressBarUp;

		private RichTextBox txt_info;

		public ShowProgress(int max)
		{
			this.InitializeComponent();
			this.progressBarUp.Maximum = max;
			initLang.LocaleForm(this, base.Name);
			this.m_isCanClose = false;
		}

		public void SetProgress(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.SetProgress), prg);
				}
				else
				{
					if (prg >= 100)
					{
						prg = 100;
					}
					if (prg == 0)
					{
						prg = 1;
					}
					this.progressBarUp.Value = prg;
					this.Refresh();
				}
			}
		}

		public void SetInfo(string msg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.SetInfo), msg);
				}
				else
				{
					this.txt_info.AppendText(msg + "\r\n");
				}
			}
		}

		public void CloseEx()
		{
			this.m_isCanClose = true;
			base.Close();
		}

		private void ShowProgress_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!this.m_isCanClose)
			{
				e.Cancel = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ShowProgress));
			this.progressBarUp = new ProgressBar();
			this.txt_info = new RichTextBox();
			base.SuspendLayout();
			this.progressBarUp.Location = new Point(13, 12);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(268, 23);
			this.progressBarUp.TabIndex = 10;
			this.txt_info.Location = new Point(13, 41);
			this.txt_info.Name = "txt_info";
			this.txt_info.Size = new Size(267, 169);
			this.txt_info.TabIndex = 12;
			this.txt_info.Text = "";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(194, 217, 247);
			base.ClientSize = new Size(292, 222);
			base.Controls.Add(this.txt_info);
			base.Controls.Add(this.progressBarUp);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ShowProgress";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "进度";
			base.TopMost = true;
			base.FormClosing += this.ShowProgress_FormClosing;
			base.ResumeLayout(false);
		}
	}
}
