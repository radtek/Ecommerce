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
using ZK.Utils;

namespace ZK.Access
{
	public class ChangeFPThresholdForm : Office2007Form
	{
		public int FPThreshold = 0;

		private IContainer components = null;

		private TextBox txt_FPThreshold;

		private Label lbl_FPThreshold;

		private Label lbl_valueRange;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		public ChangeFPThresholdForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			try
			{
				this.FPThreshold = 0;
				string info = ShowMsgInfos.GetInfo("SInputIsOutOfLimit", "输入超出有效范围({0}-{1})");
				if (!string.IsNullOrEmpty(this.txt_FPThreshold.Text))
				{
					if (int.Parse(this.txt_FPThreshold.Text) < 35 || int.Parse(this.txt_FPThreshold.Text) > 70)
					{
						SysDialogs.ShowWarningMessage(string.Format(info, 35, 70));
						this.txt_FPThreshold.Focus();
					}
					else
					{
						this.FPThreshold = int.Parse(this.txt_FPThreshold.Text);
						base.Close();
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFPThresholdValueNotNull", "指纹比对阀值不能为空"));
					this.txt_FPThreshold.Focus();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_FPThreshold_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 35, 70L);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChangeFPThresholdForm));
			this.txt_FPThreshold = new TextBox();
			this.lbl_FPThreshold = new Label();
			this.lbl_valueRange = new Label();
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			base.SuspendLayout();
			this.txt_FPThreshold.Location = new Point(209, 17);
			this.txt_FPThreshold.Name = "txt_FPThreshold";
			this.txt_FPThreshold.Size = new Size(46, 21);
			this.txt_FPThreshold.TabIndex = 0;
			this.txt_FPThreshold.KeyPress += this.txt_FPThreshold_KeyPress;
			this.lbl_FPThreshold.Location = new Point(12, 20);
			this.lbl_FPThreshold.Name = "lbl_FPThreshold";
			this.lbl_FPThreshold.Size = new Size(185, 13);
			this.lbl_FPThreshold.TabIndex = 37;
			this.lbl_FPThreshold.Text = "指纹比对阈值";
			this.lbl_FPThreshold.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_valueRange.Location = new Point(261, 20);
			this.lbl_valueRange.Name = "lbl_valueRange";
			this.lbl_valueRange.Size = new Size(117, 12);
			this.lbl_valueRange.TabIndex = 39;
			this.lbl_valueRange.Text = "(范围35-70)";
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(291, 79);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 2;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(189, 79);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 1;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(385, 114);
			base.Controls.Add(this.txt_FPThreshold);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lbl_valueRange);
			base.Controls.Add(this.lbl_FPThreshold);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ChangeFPThresholdForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "修改指纹比对阈值";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
