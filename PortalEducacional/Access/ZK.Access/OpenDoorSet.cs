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
	public class OpenDoorSet : Office2007Form
	{
		private int m_second = 0;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private RadioButton rdb_normal;

		private RadioButton rdb_openNormal;

		private RadioButton rdb_open;

		private Label lb_second;

		private TextBox txt_time;

		private ButtonX btn_ok;

		public int Second
		{
			get
			{
				return this.m_second;
			}
			set
			{
				this.m_second = value;
			}
		}

		public OpenDoorSet()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.m_second = 0;
			base.Close();
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			if (this.rdb_open.Checked)
			{
				if (!string.IsNullOrEmpty(this.txt_time.Text) && this.txt_time.Text != "0")
				{
					try
					{
						this.m_second = int.Parse(this.txt_time.Text);
						if (this.m_second < 1 || this.m_second > 255)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("STimeSetFailed", "时间设置不正确"));
						}
						else
						{
							base.Close();
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("STimeSetFailed", "时间设置不正确"));
				}
			}
			else if (this.rdb_openNormal.Checked)
			{
				this.m_second = -100;
				base.Close();
			}
			else
			{
				this.m_second = -255;
				base.Close();
			}
		}

		private void txt_time_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 254L);
		}

		private void rdb_open_CheckedChanged(object sender, EventArgs e)
		{
			if (this.rdb_open.Checked)
			{
				this.txt_time.Enabled = true;
			}
			else
			{
				this.txt_time.Enabled = false;
			}
		}

		private void rdb_start_CheckedChanged(object sender, EventArgs e)
		{
			if (this.rdb_openNormal.Checked)
			{
				this.txt_time.Enabled = false;
			}
		}

		private void rdb_normal_CheckedChanged(object sender, EventArgs e)
		{
			if (this.rdb_normal.Checked)
			{
				this.txt_time.Enabled = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(OpenDoorSet));
			this.btn_cancel = new ButtonX();
			this.rdb_normal = new RadioButton();
			this.rdb_openNormal = new RadioButton();
			this.rdb_open = new RadioButton();
			this.lb_second = new Label();
			this.txt_time = new TextBox();
			this.btn_ok = new ButtonX();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(272, 117);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.rdb_normal.AutoSize = true;
			this.rdb_normal.Location = new Point(17, 79);
			this.rdb_normal.Name = "rdb_normal";
			this.rdb_normal.Size = new Size(47, 16);
			this.rdb_normal.TabIndex = 3;
			this.rdb_normal.Text = "常开";
			this.rdb_normal.UseVisualStyleBackColor = true;
			this.rdb_normal.CheckedChanged += this.rdb_normal_CheckedChanged;
			this.rdb_openNormal.AutoSize = true;
			this.rdb_openNormal.Location = new Point(17, 50);
			this.rdb_openNormal.Name = "rdb_openNormal";
			this.rdb_openNormal.Size = new Size(131, 16);
			this.rdb_openNormal.TabIndex = 2;
			this.rdb_openNormal.Text = "启用当天常开时间段";
			this.rdb_openNormal.UseVisualStyleBackColor = true;
			this.rdb_openNormal.CheckedChanged += this.rdb_start_CheckedChanged;
			this.rdb_open.AutoSize = true;
			this.rdb_open.Checked = true;
			this.rdb_open.Location = new Point(17, 21);
			this.rdb_open.Name = "rdb_open";
			this.rdb_open.Size = new Size(47, 16);
			this.rdb_open.TabIndex = 0;
			this.rdb_open.TabStop = true;
			this.rdb_open.Text = "开门";
			this.rdb_open.UseVisualStyleBackColor = true;
			this.rdb_open.CheckedChanged += this.rdb_open_CheckedChanged;
			this.lb_second.Location = new Point(191, 21);
			this.lb_second.Name = "lb_second";
			this.lb_second.Size = new Size(144, 12);
			this.lb_second.TabIndex = 16;
			this.lb_second.Text = "秒(1-254)";
			this.txt_time.Location = new Point(153, 17);
			this.txt_time.Name = "txt_time";
			this.txt_time.Size = new Size(32, 21);
			this.txt_time.TabIndex = 1;
			this.txt_time.Text = "15";
			this.txt_time.KeyPress += this.txt_time_KeyPress;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(170, 117);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 4;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(366, 152);
			base.Controls.Add(this.txt_time);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.rdb_normal);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.rdb_openNormal);
			base.Controls.Add(this.rdb_open);
			base.Controls.Add(this.lb_second);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "OpenDoorSet";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "远程开门";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
