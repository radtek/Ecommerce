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
using System.Timers;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class DevShowInfo : Form
	{
		private static DevShowInfo m_instance = null;

		private DateTime m_date = DateTime.Now;

		private System.Timers.Timer time_hide = new System.Timers.Timer();

		private IContainer components = null;

		private Label lb_doorNameEx;

		private Label lb_noEx;

		private Label lb_devEx;

		private Label lb_doorName;

		private Label lb_no;

		private Label lb_dev;

		private Label lb_ip;

		private Label lb_ipEx;

		private PanelEx pnl_back;

		private Label lb_info;

		private Label label1;

		public static DevShowInfo Instance
		{
			get
			{
				if (DevShowInfo.m_instance == null)
				{
					DevShowInfo.m_instance = new DevShowInfo();
				}
				return DevShowInfo.m_instance;
			}
		}

		public DevShowInfo()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			DevShowInfo.m_instance = this;
			this.time_hide.Interval = 1000.0;
			this.time_hide.Enabled = false;
			this.time_hide.Elapsed += this.time_hide_Elapsed;
		}

		public void SetInfo(string devName, string no, string doorName, string ip, string info)
		{
			if (!base.IsDisposed)
			{
				Application.DoEvents();
				if (!base.InvokeRequired)
				{
					this.lb_dev.Text = devName;
					this.lb_no.Text = no;
					this.lb_doorName.Text = doorName;
					this.lb_ip.Text = ip;
					this.lb_info.Text = info;
					this.m_date = DateTime.Now;
				}
			}
		}

		public void ShowEx()
		{
			base.Show();
			this.m_date = DateTime.Now;
		}

		public void HideEx()
		{
			if (DateTime.Now >= this.m_date.AddSeconds(1.0))
			{
				base.Hide();
				this.m_date = DateTime.Now;
			}
			else
			{
				this.time_hide.Enabled = true;
			}
		}

		public void hide()
		{
			if (base.Visible && !base.IsDisposed && !base.InvokeRequired)
			{
				this.Refresh();
				base.Hide();
				this.m_date = DateTime.Now;
				this.time_hide.Enabled = true;
			}
		}

		private void time_hide_Elapsed(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ElapsedEventHandler(this.time_hide_Elapsed), sender, e);
				}
				else
				{
					this.HideEx();
				}
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
			this.lb_ip = new Label();
			this.lb_ipEx = new Label();
			this.lb_doorName = new Label();
			this.lb_no = new Label();
			this.lb_dev = new Label();
			this.lb_doorNameEx = new Label();
			this.lb_noEx = new Label();
			this.lb_devEx = new Label();
			this.pnl_back = new PanelEx();
			this.lb_info = new Label();
			this.label1 = new Label();
			this.pnl_back.SuspendLayout();
			base.SuspendLayout();
			this.lb_ip.AutoSize = true;
			this.lb_ip.Location = new Point(112, 31);
			this.lb_ip.Name = "lb_ip";
			this.lb_ip.Size = new Size(11, 12);
			this.lb_ip.TabIndex = 7;
			this.lb_ip.Text = "i";
			this.lb_ipEx.Location = new Point(7, 32);
			this.lb_ipEx.Name = "lb_ipEx";
			this.lb_ipEx.Size = new Size(97, 12);
			this.lb_ipEx.TabIndex = 6;
			this.lb_ipEx.Text = "IP地址";
			this.lb_ipEx.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_doorName.AutoSize = true;
			this.lb_doorName.Location = new Point(112, 78);
			this.lb_doorName.Name = "lb_doorName";
			this.lb_doorName.Size = new Size(11, 12);
			this.lb_doorName.TabIndex = 5;
			this.lb_doorName.Text = "i";
			this.lb_no.AutoSize = true;
			this.lb_no.Location = new Point(112, 54);
			this.lb_no.Name = "lb_no";
			this.lb_no.Size = new Size(11, 12);
			this.lb_no.TabIndex = 4;
			this.lb_no.Text = "i";
			this.lb_dev.AutoSize = true;
			this.lb_dev.Location = new Point(112, 11);
			this.lb_dev.Name = "lb_dev";
			this.lb_dev.Size = new Size(11, 12);
			this.lb_dev.TabIndex = 3;
			this.lb_dev.Text = "i";
			this.lb_doorNameEx.Location = new Point(7, 78);
			this.lb_doorNameEx.Name = "lb_doorNameEx";
			this.lb_doorNameEx.Size = new Size(97, 12);
			this.lb_doorNameEx.TabIndex = 2;
			this.lb_doorNameEx.Text = "门名称";
			this.lb_doorNameEx.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_noEx.Location = new Point(7, 56);
			this.lb_noEx.Name = "lb_noEx";
			this.lb_noEx.Size = new Size(97, 12);
			this.lb_noEx.TabIndex = 1;
			this.lb_noEx.Text = "门编号";
			this.lb_noEx.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_devEx.Location = new Point(7, 11);
			this.lb_devEx.Name = "lb_devEx";
			this.lb_devEx.Size = new Size(97, 12);
			this.lb_devEx.TabIndex = 0;
			this.lb_devEx.Text = "所属设备";
			this.lb_devEx.TextAlign = ContentAlignment.MiddleLeft;
			this.pnl_back.CanvasColor = SystemColors.Control;
			this.pnl_back.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_back.Controls.Add(this.lb_info);
			this.pnl_back.Controls.Add(this.label1);
			this.pnl_back.Controls.Add(this.lb_ip);
			this.pnl_back.Controls.Add(this.lb_ipEx);
			this.pnl_back.Controls.Add(this.lb_devEx);
			this.pnl_back.Controls.Add(this.lb_doorName);
			this.pnl_back.Controls.Add(this.lb_noEx);
			this.pnl_back.Controls.Add(this.lb_no);
			this.pnl_back.Controls.Add(this.lb_doorNameEx);
			this.pnl_back.Controls.Add(this.lb_dev);
			this.pnl_back.Dock = DockStyle.Fill;
			this.pnl_back.Location = new Point(0, 0);
			this.pnl_back.Name = "pnl_back";
			this.pnl_back.Size = new Size(246, 118);
			this.pnl_back.Style.Alignment = StringAlignment.Center;
			this.pnl_back.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_back.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_back.Style.Border = eBorderType.SingleLine;
			this.pnl_back.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_back.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_back.Style.GradientAngle = 90;
			this.pnl_back.TabIndex = 1;
			this.lb_info.AutoSize = true;
			this.lb_info.Location = new Point(112, 99);
			this.lb_info.Name = "lb_info";
			this.lb_info.Size = new Size(11, 12);
			this.lb_info.TabIndex = 9;
			this.lb_info.Text = "i";
			this.label1.Location = new Point(7, 99);
			this.label1.Name = "label1";
			this.label1.Size = new Size(97, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "状态描述";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(246, 118);
			base.ControlBox = false;
			base.Controls.Add(this.pnl_back);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Name = "DevShowInfo";
			base.Opacity = 0.94;
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "门信息";
			base.TopMost = true;
			this.pnl_back.ResumeLayout(false);
			this.pnl_back.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
