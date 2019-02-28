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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class AboutForm2 : Office2007Form
	{
		private IContainer components = null;

		private ButtonX Btn_Cancel;

		private Label label1;

		public AboutForm2()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void AboutForm_Load(object sender, EventArgs e)
		{
			string text = File.ReadAllText(Application.StartupPath + "\\about.txt");
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("SoftVersion");
			text = ((!(nodeValueByName.Trim() == "")) ? text.Replace("%version%", nodeValueByName) : text.Replace("%version%", Application.ProductVersion));
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\plcommpro.dll");
			string newValue = (versionInfo == null || versionInfo.FileVersion == null) ? "" : versionInfo.FileVersion.Replace(",", ".").Replace(" ", "");
			text = text.Replace("%psdk%", newValue);
			versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\zkemkeeper.dll");
			string newValue2 = (versionInfo == null || versionInfo.FileVersion == null) ? "" : versionInfo.FileVersion.Replace(",", ".").Replace(" ", "");
			text = text.Replace("%ssdk%", newValue2);
			string str = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			text = text + "\n\nBuild: " + str;
			this.label1.Text = text;
		}

		private void Btn_Cancel_Click(object sender, EventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AboutForm2));
			this.Btn_Cancel = new ButtonX();
			this.label1 = new Label();
			base.SuspendLayout();
			this.Btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.Btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.Btn_Cancel.Location = new Point(315, 179);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new Size(82, 25);
			this.Btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.Btn_Cancel.TabIndex = 43;
			this.Btn_Cancel.Text = "确定";
			this.Btn_Cancel.Click += this.Btn_Cancel_Click;
			this.label1.Location = new Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new Size(385, 167);
			this.label1.TabIndex = 45;
			this.label1.Text = "Modificado por ZK Brasil";
			base.AcceptButton = this.Btn_Cancel;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(409, 216);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.Btn_Cancel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AboutForm2";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "关于";
			base.Load += this.AboutForm_Load;
			base.ResumeLayout(false);
		}
	}
}
