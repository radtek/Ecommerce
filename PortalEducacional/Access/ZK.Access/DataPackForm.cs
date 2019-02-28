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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access
{
	public class DataPackForm : Office2007Form
	{
		private IContainer components = null;

		private ButtonX btn_browser;

		private CheckBox chk_bakData;

		private TextBox txt_dataBak;

		private Label lbl_dataBak;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		private FolderBrowserDialog folderBrowserDialog1;

		public DataPackForm()
		{
			this.InitializeComponent();
			this.BackPath();
			initLang.LocaleForm(this, base.Name);
			this.txt_dataBak_TextChanged(null, null);
		}

		private void BackPath()
		{
			this.txt_dataBak.Text = AppSite.Instance.GetNodeValueByName("BakDBPath");
			if (AppSite.Instance.GetNodeValueByName("IsBackDB") == "1")
			{
				this.chk_bakData.Checked = true;
			}
		}

		public static bool ISChinese(string strWord)
		{
			string pattern = "[\\u4e00-\\u9fa5]";
			return Regex.IsMatch(strWord, pattern);
		}

		private void btn_browser_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = this.folderBrowserDialog1;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.txt_dataBak.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_dataBak.Text))
			{
				AppSite.Instance.SetNodeValue("BakDBPath", this.txt_dataBak.Text);
				if (this.chk_bakData.Checked)
				{
					AppSite.Instance.SetNodeValue("IsBackDB", "1");
				}
				else
				{
					AppSite.Instance.SetNodeValue("IsBackDB", "0");
				}
				base.Close();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("dataBackupPathNull", "数据库备份路径不能为空"));
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_dataBak_TextChanged(object sender, EventArgs e)
		{
			this.btn_ok.Enabled = false;
			if (!string.IsNullOrEmpty(this.txt_dataBak.Text))
			{
				this.btn_ok.Enabled = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DataPackForm));
			this.btn_browser = new ButtonX();
			this.chk_bakData = new CheckBox();
			this.txt_dataBak = new TextBox();
			this.lbl_dataBak = new Label();
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.folderBrowserDialog1 = new FolderBrowserDialog();
			base.SuspendLayout();
			this.btn_browser.AccessibleRole = AccessibleRole.PushButton;
			this.btn_browser.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_browser.Location = new Point(455, 17);
			this.btn_browser.Name = "btn_browser";
			this.btn_browser.Size = new Size(82, 23);
			this.btn_browser.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_browser.TabIndex = 2;
			this.btn_browser.Text = "浏览";
			this.btn_browser.Click += this.btn_browser_Click;
			this.chk_bakData.AutoSize = true;
			this.chk_bakData.Location = new Point(12, 51);
			this.chk_bakData.Name = "chk_bakData";
			this.chk_bakData.Size = new Size(144, 16);
			this.chk_bakData.TabIndex = 3;
			this.chk_bakData.Text = "关闭系统时备份数据库";
			this.chk_bakData.UseVisualStyleBackColor = true;
			this.txt_dataBak.BackColor = SystemColors.Window;
			this.txt_dataBak.Location = new Point(205, 18);
			this.txt_dataBak.Name = "txt_dataBak";
			this.txt_dataBak.ReadOnly = true;
			this.txt_dataBak.Size = new Size(238, 21);
			this.txt_dataBak.TabIndex = 1;
			this.txt_dataBak.TextChanged += this.txt_dataBak_TextChanged;
			this.lbl_dataBak.Location = new Point(12, 21);
			this.lbl_dataBak.Name = "lbl_dataBak";
			this.lbl_dataBak.Size = new Size(187, 14);
			this.lbl_dataBak.TabIndex = 18;
			this.lbl_dataBak.Text = "数据库备份路径";
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(361, 94);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 4;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(455, 94);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(549, 129);
			base.Controls.Add(this.txt_dataBak);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.btn_browser);
			base.Controls.Add(this.chk_bakData);
			base.Controls.Add(this.lbl_dataBak);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DataPackForm";
			this.Text = "数据库备份";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
