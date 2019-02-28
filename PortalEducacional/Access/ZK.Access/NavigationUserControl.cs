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
using System.IO;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class NavigationUserControl : UserControl
	{
		public delegate void ClickHandle(int id);

		private IContainer components = null;

		public PanelEx MenuPanelEx;

		public PanelEx panelEx1;

		private LabelX lb_welcome;

		private Label label9;

		private Label label8;

		private Label label6;

		private Label label5;

		private Label label4;

		private PictureBox pic_reports;

		private PictureBox pic_monitoring;

		private PictureBox pictureBox10;

		private PictureBox pic_level;

		private PictureBox pic_holiday;

		private PictureBox pic_time;

		private Label label3;

		private Label label2;

		private Label label1;

		private PictureBox pictureBox5;

		private PictureBox pic_personnel;

		private PictureBox pic_door;

		private PictureBox pictureBox2;

		private PictureBox pic_device;

		private PictureBox Person_Pic_Log;

		private Panel panel1;

		private Panel panel2;

		private Panel panel3;

		private Panel panel4;

		private Panel panel5;

		private Panel panel6;

		private Panel panel7;

		private Panel panel8;

		public event ClickHandle ClickEvent;

		private void OnBtnCLick(int id)
		{
			if (this.ClickEvent != null)
			{
				this.ClickEvent(id);
			}
		}

		public NavigationUserControl()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.label1.AutoSize = false;
			this.label2.AutoSize = false;
			this.label3.AutoSize = false;
			this.label4.AutoSize = false;
			this.label5.AutoSize = false;
			this.label6.AutoSize = false;
			this.label8.AutoSize = false;
			this.label9.AutoSize = false;
			try
			{
				base.SetStyle(ControlStyles.UserPaint, true);
				base.SetStyle(ControlStyles.DoubleBuffer, true);
				base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				string filename = Path.GetDirectoryName(Application.ExecutablePath) + "\\SoftLog.jpg";
				this.Person_Pic_Log.Image = Image.FromFile(filename);
			}
			catch
			{
			}
			this.lb_welcome.Text = ShowMsgInfos.GetInfo("SHello", "你好") + " " + SysInfos.SysUserInfo.username + ShowMsgInfos.GetInfo("SWelcome", "，欢迎登录ZKAccess3.5门禁管理系统");
			this.CheckPermission();
			MainForm.SetZKText(this.lb_welcome);
			this.ChangeSkin();
		}

		private void ChangeSkin()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				PictureBox pictureBox = this.pictureBox2;
				PictureBox pictureBox2 = this.pictureBox5;
				PictureBox pictureBox3 = this.pictureBox10;
				Image image = pictureBox3.Image = ResourceIPC.thread;
				Image image4 = pictureBox.Image = (pictureBox2.Image = image);
				this.pic_device.Image = ResourceIPC.device;
				this.pic_personnel.Image = ResourceIPC.personnel;
				this.pic_time.Image = ResourceIPC.time;
				this.pic_holiday.Image = ResourceIPC.holidays;
				this.pic_door.Image = ResourceIPC.holidays;
				this.pic_level.Image = ResourceIPC.level;
				this.pic_monitoring.Image = ResourceIPC.monitoring;
				this.pic_reports.Image = ResourceIPC.reports;
				this.Person_Pic_Log.Visible = false;
				this.lb_welcome.Visible = false;
			}
		}

		private void CheckPermission()
		{
			this.SetMenuEnable(this.pic_personnel, SysInfos.Personnel);
			this.SetMenuEnable(this.pic_device, SysInfos.Device);
			this.SetMenuEnable(this.pic_time, SysInfos.AccessLevel);
			this.SetMenuEnable(this.pic_holiday, SysInfos.AccessLevel);
			this.SetMenuEnable(this.pic_door, SysInfos.AccessLevel);
			this.SetMenuEnable(this.pic_level, SysInfos.AccessLevel);
			this.SetMenuEnable(this.pic_monitoring, SysInfos.Monitoring);
			this.SetMenuEnable(this.pic_reports, SysInfos.Report);
		}

		private void SetMenuEnable(PictureBox menu, string permissionName)
		{
			if (SysInfos.IsOwerShowPermission(permissionName))
			{
				menu.Enabled = true;
			}
			else
			{
				menu.Enabled = false;
			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(1);
		}

		private void pic_personnel_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(2);
		}

		private void pic_time_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(3);
		}

		private void pic_holiday_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(4);
		}

		private void pic_door_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(5);
		}

		private void pic_level_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(6);
		}

		private void pic_monitoring_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(7);
		}

		private void pic_reports_Click(object sender, EventArgs e)
		{
			this.OnBtnCLick(8);
		}

		private void pic_device_MouseEnter(object sender, EventArgs e)
		{
			this.pic_device.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.device_s : Resource.device_s);
		}

		private void pic_device_MouseLeave(object sender, EventArgs e)
		{
			this.pic_device.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.device : Resource.device);
		}

		private void pic_personnel_MouseEnter(object sender, EventArgs e)
		{
			this.pic_personnel.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.personnel_s : Resource.personnel_s);
		}

		private void pic_personnel_MouseLeave(object sender, EventArgs e)
		{
			this.pic_personnel.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.personnel : Resource.personnel);
		}

		private void pic_time_MouseEnter(object sender, EventArgs e)
		{
			this.pic_time.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.time_s : Resource.time_s);
		}

		private void pic_time_MouseLeave(object sender, EventArgs e)
		{
			this.pic_time.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.time : Resource.time);
		}

		private void pic_holiday_MouseEnter(object sender, EventArgs e)
		{
			this.pic_holiday.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.holidays_s : Resource.holidays_s);
		}

		private void pic_holiday_MouseLeave(object sender, EventArgs e)
		{
			this.pic_holiday.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.holidays : Resource.holidays);
		}

		private void pic_door_MouseEnter(object sender, EventArgs e)
		{
			this.pic_door.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.door_s : Resource.door_s);
		}

		private void pic_door_MouseLeave(object sender, EventArgs e)
		{
			this.pic_door.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.door : Resource.door);
		}

		private void pic_level_MouseEnter(object sender, EventArgs e)
		{
			this.pic_level.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.level_s : Resource.level_s);
		}

		private void pic_level_MouseLeave(object sender, EventArgs e)
		{
			this.pic_level.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.level : Resource.level);
		}

		private void pic_monitoring_MouseEnter(object sender, EventArgs e)
		{
			this.pic_monitoring.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.monitoring_s : Resource.monitoring_s);
		}

		private void pic_monitoring_MouseLeave(object sender, EventArgs e)
		{
			this.pic_monitoring.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.monitoring : Resource.monitoring);
		}

		private void pic_reports_MouseEnter(object sender, EventArgs e)
		{
			this.pic_reports.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.reports_s : Resource.reports_s);
		}

		private void pic_reports_MouseLeave(object sender, EventArgs e)
		{
			this.pic_reports.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.reports : Resource.reports);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NavigationUserControl));
			this.MenuPanelEx = new PanelEx();
			this.lb_welcome = new LabelX();
			this.panelEx1 = new PanelEx();
			this.label9 = new Label();
			this.label8 = new Label();
			this.label6 = new Label();
			this.label5 = new Label();
			this.label4 = new Label();
			this.pic_reports = new PictureBox();
			this.pic_monitoring = new PictureBox();
			this.pictureBox10 = new PictureBox();
			this.pic_level = new PictureBox();
			this.pic_holiday = new PictureBox();
			this.pic_time = new PictureBox();
			this.label3 = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.pictureBox5 = new PictureBox();
			this.pic_personnel = new PictureBox();
			this.pic_door = new PictureBox();
			this.pictureBox2 = new PictureBox();
			this.pic_device = new PictureBox();
			this.Person_Pic_Log = new PictureBox();
			this.panel1 = new Panel();
			this.panel2 = new Panel();
			this.panel3 = new Panel();
			this.panel4 = new Panel();
			this.panel5 = new Panel();
			this.panel6 = new Panel();
			this.panel7 = new Panel();
			this.panel8 = new Panel();
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.pic_reports).BeginInit();
			((ISupportInitialize)this.pic_monitoring).BeginInit();
			((ISupportInitialize)this.pictureBox10).BeginInit();
			((ISupportInitialize)this.pic_level).BeginInit();
			((ISupportInitialize)this.pic_holiday).BeginInit();
			((ISupportInitialize)this.pic_time).BeginInit();
			((ISupportInitialize)this.pictureBox5).BeginInit();
			((ISupportInitialize)this.pic_personnel).BeginInit();
			((ISupportInitialize)this.pic_door).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			((ISupportInitialize)this.pic_device).BeginInit();
			((ISupportInitialize)this.Person_Pic_Log).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel7.SuspendLayout();
			this.panel8.SuspendLayout();
			base.SuspendLayout();
			this.MenuPanelEx.CanvasColor = SystemColors.Control;
			this.MenuPanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MenuPanelEx.Controls.Add(this.lb_welcome);
			this.MenuPanelEx.Dock = DockStyle.Top;
			this.MenuPanelEx.Location = new Point(0, 20);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(940, 35);
			this.MenuPanelEx.Style.Alignment = StringAlignment.Center;
			this.MenuPanelEx.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.MenuPanelEx.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.MenuPanelEx.Style.Border = eBorderType.SingleLine;
			this.MenuPanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MenuPanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenuPanelEx.Style.GradientAngle = 90;
			this.MenuPanelEx.TabIndex = 27;
			this.lb_welcome.AutoSize = true;
			this.lb_welcome.BackgroundStyle.Class = "";
			this.lb_welcome.Location = new Point(8, 10);
			this.lb_welcome.Name = "lb_welcome";
			this.lb_welcome.Size = new Size(260, 18);
			this.lb_welcome.TabIndex = 0;
			this.lb_welcome.Text = "你好，欢迎登录ZKAccess.Net3.0门禁管理系统";
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Dock = DockStyle.Top;
			this.panelEx1.Location = new Point(0, 0);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(940, 20);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 28;
			this.panelEx1.Text = "导航";
			this.label9.Dock = DockStyle.Fill;
			this.label9.Location = new Point(0, 0);
			this.label9.Name = "label9";
			this.label9.Size = new Size(170, 20);
			this.label9.TabIndex = 144;
			this.label9.Text = "报表";
			this.label9.TextAlign = ContentAlignment.TopCenter;
			this.label8.Dock = DockStyle.Fill;
			this.label8.Location = new Point(0, 0);
			this.label8.Name = "label8";
			this.label8.Size = new Size(170, 20);
			this.label8.TabIndex = 143;
			this.label8.Text = "实时监控";
			this.label8.TextAlign = ContentAlignment.TopCenter;
			this.label6.Dock = DockStyle.Fill;
			this.label6.Location = new Point(0, 0);
			this.label6.Name = "label6";
			this.label6.Size = new Size(200, 20);
			this.label6.TabIndex = 142;
			this.label6.Text = "门禁权限组";
			this.label6.TextAlign = ContentAlignment.TopCenter;
			this.label5.Dock = DockStyle.Fill;
			this.label5.Location = new Point(0, 0);
			this.label5.Name = "label5";
			this.label5.Size = new Size(200, 20);
			this.label5.TabIndex = 141;
			this.label5.Text = "门禁时间段";
			this.label5.TextAlign = ContentAlignment.TopCenter;
			this.label4.Dock = DockStyle.Fill;
			this.label4.Location = new Point(0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new Size(200, 20);
			this.label4.TabIndex = 140;
			this.label4.Text = "门禁节假日";
			this.label4.TextAlign = ContentAlignment.TopCenter;
			this.pic_reports.Image = (Image)componentResourceManager.GetObject("pic_reports.Image");
			this.pic_reports.Location = new Point(703, 282);
			this.pic_reports.Name = "pic_reports";
			this.pic_reports.Size = new Size(100, 82);
			this.pic_reports.TabIndex = 139;
			this.pic_reports.TabStop = false;
			this.pic_reports.Click += this.pic_reports_Click;
			this.pic_reports.MouseEnter += this.pic_reports_MouseEnter;
			this.pic_reports.MouseLeave += this.pic_reports_MouseLeave;
			this.pic_monitoring.Image = (Image)componentResourceManager.GetObject("pic_monitoring.Image");
			this.pic_monitoring.Location = new Point(703, 173);
			this.pic_monitoring.Name = "pic_monitoring";
			this.pic_monitoring.Size = new Size(100, 80);
			this.pic_monitoring.TabIndex = 138;
			this.pic_monitoring.TabStop = false;
			this.pic_monitoring.Click += this.pic_monitoring_Click;
			this.pic_monitoring.MouseEnter += this.pic_monitoring_MouseEnter;
			this.pic_monitoring.MouseLeave += this.pic_monitoring_MouseLeave;
			this.pictureBox10.Image = Resource.thread;
			this.pictureBox10.Location = new Point(609, 252);
			this.pictureBox10.Name = "pictureBox10";
			this.pictureBox10.Size = new Size(54, 41);
			this.pictureBox10.TabIndex = 137;
			this.pictureBox10.TabStop = false;
			this.pic_level.Image = (Image)componentResourceManager.GetObject("pic_level.Image");
			this.pic_level.Location = new Point(461, 391);
			this.pic_level.Name = "pic_level";
			this.pic_level.Size = new Size(100, 82);
			this.pic_level.TabIndex = 136;
			this.pic_level.TabStop = false;
			this.pic_level.Click += this.pic_level_Click;
			this.pic_level.MouseEnter += this.pic_level_MouseEnter;
			this.pic_level.MouseLeave += this.pic_level_MouseLeave;
			this.pic_holiday.Image = (Image)componentResourceManager.GetObject("pic_holiday.Image");
			this.pic_holiday.Location = new Point(461, 171);
			this.pic_holiday.Name = "pic_holiday";
			this.pic_holiday.Size = new Size(100, 82);
			this.pic_holiday.TabIndex = 135;
			this.pic_holiday.TabStop = false;
			this.pic_holiday.Click += this.pic_holiday_Click;
			this.pic_holiday.MouseEnter += this.pic_holiday_MouseEnter;
			this.pic_holiday.MouseLeave += this.pic_holiday_MouseLeave;
			this.pic_time.Image = (Image)componentResourceManager.GetObject("pic_time.Image");
			this.pic_time.Location = new Point(461, 61);
			this.pic_time.Name = "pic_time";
			this.pic_time.Size = new Size(100, 82);
			this.pic_time.TabIndex = 134;
			this.pic_time.TabStop = false;
			this.pic_time.Click += this.pic_time_Click;
			this.pic_time.MouseEnter += this.pic_time_MouseEnter;
			this.pic_time.MouseLeave += this.pic_time_MouseLeave;
			this.label3.Dock = DockStyle.Fill;
			this.label3.Location = new Point(0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new Size(200, 20);
			this.label3.TabIndex = 133;
			this.label3.Text = "门设置";
			this.label3.TextAlign = ContentAlignment.TopCenter;
			this.label2.Dock = DockStyle.Fill;
			this.label2.Location = new Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new Size(170, 20);
			this.label2.TabIndex = 132;
			this.label2.Text = "人员";
			this.label2.TextAlign = ContentAlignment.TopCenter;
			this.label1.Dock = DockStyle.Fill;
			this.label1.Location = new Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new Size(170, 20);
			this.label1.TabIndex = 131;
			this.label1.Text = "设备";
			this.label1.TextAlign = ContentAlignment.TopCenter;
			this.pictureBox5.Image = Resource.thread;
			this.pictureBox5.Location = new Point(357, 250);
			this.pictureBox5.Name = "pictureBox5";
			this.pictureBox5.Size = new Size(56, 41);
			this.pictureBox5.TabIndex = 130;
			this.pictureBox5.TabStop = false;
			this.pic_personnel.Image = Resource.personnel;
			this.pic_personnel.Location = new Point(232, 229);
			this.pic_personnel.Name = "pic_personnel";
			this.pic_personnel.Size = new Size(100, 82);
			this.pic_personnel.TabIndex = 129;
			this.pic_personnel.TabStop = false;
			this.pic_personnel.Click += this.pic_personnel_Click;
			this.pic_personnel.MouseEnter += this.pic_personnel_MouseEnter;
			this.pic_personnel.MouseLeave += this.pic_personnel_MouseLeave;
			this.pic_door.Image = (Image)componentResourceManager.GetObject("pic_door.Image");
			this.pic_door.Location = new Point(461, 282);
			this.pic_door.Name = "pic_door";
			this.pic_door.Size = new Size(100, 82);
			this.pic_door.TabIndex = 128;
			this.pic_door.TabStop = false;
			this.pic_door.Click += this.pic_door_Click;
			this.pic_door.MouseEnter += this.pic_door_MouseEnter;
			this.pic_door.MouseLeave += this.pic_door_MouseLeave;
			this.pictureBox2.Image = Resource.thread;
			this.pictureBox2.Location = new Point(160, 250);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new Size(48, 41);
			this.pictureBox2.TabIndex = 127;
			this.pictureBox2.TabStop = false;
			this.pic_device.Image = Resource.device;
			this.pic_device.InitialImage = null;
			this.pic_device.Location = new Point(41, 229);
			this.pic_device.Name = "pic_device";
			this.pic_device.Size = new Size(100, 82);
			this.pic_device.TabIndex = 126;
			this.pic_device.TabStop = false;
			this.pic_device.Click += this.pictureBox1_Click;
			this.pic_device.MouseEnter += this.pic_device_MouseEnter;
			this.pic_device.MouseLeave += this.pic_device_MouseLeave;
			this.Person_Pic_Log.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.Person_Pic_Log.BackgroundImageLayout = ImageLayout.Stretch;
			this.Person_Pic_Log.ImageLocation = "";
			this.Person_Pic_Log.InitialImage = null;
			this.Person_Pic_Log.Location = new Point(907, 0);
			this.Person_Pic_Log.Name = "Person_Pic_Log";
			this.Person_Pic_Log.Size = new Size(16, 16);
			this.Person_Pic_Log.SizeMode = PictureBoxSizeMode.AutoSize;
			this.Person_Pic_Log.TabIndex = 146;
			this.Person_Pic_Log.TabStop = false;
			this.Person_Pic_Log.WaitOnLoad = true;
			this.panel1.Controls.Add(this.label5);
			this.panel1.Location = new Point(411, 146);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(200, 20);
			this.panel1.TabIndex = 147;
			this.panel2.Controls.Add(this.label1);
			this.panel2.Location = new Point(6, 314);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(170, 20);
			this.panel2.TabIndex = 148;
			this.panel3.Controls.Add(this.label3);
			this.panel3.Location = new Point(411, 367);
			this.panel3.Name = "panel3";
			this.panel3.Size = new Size(200, 20);
			this.panel3.TabIndex = 149;
			this.panel4.Controls.Add(this.label2);
			this.panel4.Location = new Point(197, 314);
			this.panel4.Name = "panel4";
			this.panel4.Size = new Size(170, 20);
			this.panel4.TabIndex = 149;
			this.panel5.Controls.Add(this.label6);
			this.panel5.Location = new Point(411, 475);
			this.panel5.Name = "panel5";
			this.panel5.Size = new Size(200, 20);
			this.panel5.TabIndex = 149;
			this.panel6.Controls.Add(this.label8);
			this.panel6.Location = new Point(668, 256);
			this.panel6.Name = "panel6";
			this.panel6.Size = new Size(170, 20);
			this.panel6.TabIndex = 149;
			this.panel7.Controls.Add(this.label4);
			this.panel7.Location = new Point(411, 256);
			this.panel7.Name = "panel7";
			this.panel7.Size = new Size(200, 20);
			this.panel7.TabIndex = 149;
			this.panel8.Controls.Add(this.label9);
			this.panel8.Location = new Point(668, 367);
			this.panel8.Name = "panel8";
			this.panel8.Size = new Size(170, 20);
			this.panel8.TabIndex = 150;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoScroll = true;
			this.AutoSize = true;
			this.BackColor = Color.Transparent;
			this.BackgroundImage = (Image)componentResourceManager.GetObject("$this.BackgroundImage");
			this.BackgroundImageLayout = ImageLayout.Stretch;
			base.Controls.Add(this.Person_Pic_Log);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.pictureBox10);
			base.Controls.Add(this.pic_reports);
			base.Controls.Add(this.pic_monitoring);
			base.Controls.Add(this.pic_level);
			base.Controls.Add(this.pic_holiday);
			base.Controls.Add(this.pic_time);
			base.Controls.Add(this.pictureBox5);
			base.Controls.Add(this.pic_personnel);
			base.Controls.Add(this.pic_door);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.pic_device);
			base.Controls.Add(this.MenuPanelEx);
			base.Controls.Add(this.panel7);
			base.Controls.Add(this.panel2);
			base.Controls.Add(this.panel4);
			base.Controls.Add(this.panel3);
			base.Controls.Add(this.panel5);
			base.Controls.Add(this.panel8);
			base.Controls.Add(this.panel6);
			base.Controls.Add(this.panelEx1);
			base.Name = "NavigationUserControl";
			base.Size = new Size(940, 498);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			((ISupportInitialize)this.pic_reports).EndInit();
			((ISupportInitialize)this.pic_monitoring).EndInit();
			((ISupportInitialize)this.pictureBox10).EndInit();
			((ISupportInitialize)this.pic_level).EndInit();
			((ISupportInitialize)this.pic_holiday).EndInit();
			((ISupportInitialize)this.pic_time).EndInit();
			((ISupportInitialize)this.pictureBox5).EndInit();
			((ISupportInitialize)this.pic_personnel).EndInit();
			((ISupportInitialize)this.pic_door).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			((ISupportInitialize)this.pic_device).EndInit();
			((ISupportInitialize)this.Person_Pic_Log).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
