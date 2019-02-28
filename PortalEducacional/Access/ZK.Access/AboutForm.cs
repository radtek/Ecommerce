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
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class AboutForm : Office2007Form
	{
		private IContainer components = null;

		private Label lab_softVersion;

		private Label lab_SDKVersion;

		private Label lab_copyRight;

		private ButtonX Btn_Cancel;

		private Label lblStdVer;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label123;

		public AboutForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			MainForm.SetZKText(this.lab_softVersion);
		}

		private void AboutForm_Load(object sender, EventArgs e)
		{
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\plcommpro.dll");
			this.lab_SDKVersion.Text = ShowMsgInfos.GetInfo("PullSDK", "Pull SDK") + ": " + ((versionInfo == null || versionInfo.FileVersion == null) ? "" : versionInfo.FileVersion.Replace(",", ".").Replace(" ", ""));
			versionInfo = FileVersionInfo.GetVersionInfo(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\zkemkeeper.dll");
			this.lblStdVer.Text = ShowMsgInfos.GetInfo("StdSDK", "StandAlone SDK") + ": " + ((versionInfo == null || versionInfo.FileVersion == null) ? "" : versionInfo.FileVersion.Replace(",", ".").Replace(" ", ""));
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AboutForm));
			this.lab_softVersion = new Label();
			this.lab_SDKVersion = new Label();
			this.lab_copyRight = new Label();
			this.Btn_Cancel = new ButtonX();
			this.lblStdVer = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.label123 = new Label();
			base.SuspendLayout();
			this.lab_softVersion.Location = new Point(331, 13);
			this.lab_softVersion.Name = "lab_softVersion";
			this.lab_softVersion.Size = new Size(39, 13);
			this.lab_softVersion.TabIndex = 40;
			this.lab_softVersion.Text = "SoapAdmin 3.5: Pacote 0.13";
			this.lab_softVersion.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_softVersion.Visible = false;
			this.lab_SDKVersion.Location = new Point(18, 35);
			this.lab_SDKVersion.Name = "lab_SDKVersion";
			this.lab_SDKVersion.Size = new Size(352, 13);
			this.lab_SDKVersion.TabIndex = 41;
			this.lab_SDKVersion.Text = "Pull SDK 版本:";
			this.lab_SDKVersion.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_copyRight.Location = new Point(315, 81);
			this.lab_copyRight.Name = "lab_copyRight";
			this.lab_copyRight.Size = new Size(55, 13);
			this.lab_copyRight.TabIndex = 42;
			this.lab_copyRight.Text = "Intelbras. Totos os direitos reservados";
			this.lab_copyRight.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_copyRight.Visible = false;
			this.Btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.Btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.Btn_Cancel.Location = new Point(315, 133);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new Size(82, 25);
			this.Btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.Btn_Cancel.TabIndex = 43;
			this.Btn_Cancel.Text = "确定";
			this.Btn_Cancel.Click += this.Btn_Cancel_Click;
			this.lblStdVer.Location = new Point(18, 57);
			this.lblStdVer.Name = "lblStdVer";
			this.lblStdVer.Size = new Size(352, 13);
			this.lblStdVer.TabIndex = 44;
			this.lblStdVer.Text = "Standalone SDK 版本:";
			this.lblStdVer.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(289, 107);
			this.label2.Name = "label2";
			this.label2.Size = new Size(81, 13);
			this.label2.TabIndex = 46;
			this.label2.Text = "Build: 0.0.0.694";
			this.label2.Visible = false;
			this.label3.AutoSize = true;
			this.label3.Location = new Point(18, 81);
			this.label3.Name = "label3";
			this.label3.Size = new Size(185, 13);
			this.label3.TabIndex = 48;
			this.label3.Text = "Intelbras. Totos os direitos reservados";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(18, 107);
			this.label4.Name = "label4";
			this.label4.Size = new Size(81, 13);
			this.label4.TabIndex = 49;
			this.label4.Text = "Build: 0.0.0.694";
			this.label123.AutoSize = true;
			this.label123.Location = new Point(18, 13);
			this.label123.Name = "label123";
			this.label123.Size = new Size(143, 13);
			this.label123.TabIndex = 50;
			this.label123.Text = "SoapAdmin 3.5: Pacote 0.13";
			base.AcceptButton = this.Btn_Cancel;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(409, 171);
			base.Controls.Add(this.label123);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.lblStdVer);
			base.Controls.Add(this.Btn_Cancel);
			base.Controls.Add(this.lab_copyRight);
			base.Controls.Add(this.lab_SDKVersion);
			base.Controls.Add(this.lab_softVersion);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AboutForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "关于";
			base.Load += this.AboutForm_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
