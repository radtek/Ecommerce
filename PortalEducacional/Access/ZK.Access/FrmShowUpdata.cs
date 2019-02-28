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
using ZK.Access.device;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class FrmShowUpdata : Office2007Form
	{
		private bool RunImmediateCommands = false;

		private bool RunOnlyLoggedUserCommands = false;

		private bool UseUpdateSmallVersion = false;

		public DateTime m_date = DateTime.Now;

		private static bool m_isShow = true;

		private static FrmShowUpdata m_instance = null;

		public bool sync_Info_PcToDeving = true;

		public DateTime m_hidedate = DateTime.Now;

		private IContainer components = null;

		private Label label1;

		private ButtonX btn_addPerson;

		private ButtonX buttonX1;

		private Timer timer1;

		private Timer timer2;

		public static bool IsShow => FrmShowUpdata.m_isShow;

		public static FrmShowUpdata Instance
		{
			get
			{
				if (FrmShowUpdata.m_instance == null)
				{
					FrmShowUpdata.m_instance = new FrmShowUpdata();
				}
				if (MainForm._ia == null || MainForm._ia.MainFrom == null || MainForm._ia.MainFrom.IsDisposed)
				{
					if (Application.OpenForms != null && Application.OpenForms.Count > 0)
					{
						FrmShowUpdata.m_instance.Owner = Application.OpenForms[0];
					}
					return FrmShowUpdata.m_instance;
				}
				FrmShowUpdata.m_instance.Owner = MainForm._ia.MainFrom;
				return FrmShowUpdata.m_instance;
			}
		}

		public FrmShowUpdata()
		{
			this.InitializeComponent();
			FrmShowUpdata.m_instance = this;
			initLang.LocaleForm(this, base.Name);
		}

		private void ch_noshow_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void procListen(bool islisten)
		{
			if (DeviceServers.Instance.Count > 0)
			{
				int num = 0;
				for (num = 0; num < DeviceServers.Instance.Count; num++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[num];
					if (deviceServerBll != null)
					{
						deviceServerBll.IsNeedListen = islisten;
						Application.DoEvents();
					}
				}
			}
		}

		private void btn_addPerson_Click(object sender, EventArgs e)
		{
			try
			{
				this.sync_Info_PcToDeving = false;
				PCToDeviceEx pCToDeviceEx = new PCToDeviceEx();
				pCToDeviceEx.setRunImmediateCommands(this.RunImmediateCommands);
				pCToDeviceEx.setRunOnlyLoggedUserCommands(this.RunOnlyLoggedUserCommands);
				base.Hide();
				this.procListen(false);
				pCToDeviceEx.ShowDialog();
				this.procListen(true);
				this.sync_Info_PcToDeving = true;
				this.Start();
			}
			catch (Exception ex)
			{
				LogServer.Log("Exception in FrmShowUpdata.btn_addPerson_Click: " + ex.Message, true);
			}
		}

		public void ShowEx()
		{
			this.ShowEx(false, false, false);
		}

		public void ShowEx(bool immediate, bool loggedOnly, bool small)
		{
			try
			{
				this.RunImmediateCommands = immediate;
				this.RunOnlyLoggedUserCommands = loggedOnly;
				this.UseUpdateSmallVersion = small;
				if (!GLOBAL.IsMonitorActive() || !GLOBAL.IsMonitorOwner)
				{
					Application.DoEvents();
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					long count = devCmdsBll.GetCount("status=0 and SN_id in (Select distinct id from Machines)");
					if (count > 0 && MainForm._ia != null && MainForm._ia.MainFrom != null && !MainForm._ia.MainFrom.IsDisposed)
					{
						base.Show();
						for (int i = 0; i < 10; i++)
						{
							if (base.Visible)
							{
								break;
							}
							Application.DoEvents();
							base.Show();
						}
						base.Left = MainForm._ia.MainFrom.Left + 5;
						base.Top = MainForm._ia.MainFrom.Top + MainForm._ia.MainFrom.Height - base.Height - 5;
						this.timer1.Interval = 5000;
						this.timer1.Enabled = true;
						this.m_date = DateTime.Now;
						this.timer2.Enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				LogServer.Log("Exception in FrmShowUpdata.ShowEx: " + ex.Message, true);
			}
		}

		public void HideEx()
		{
			base.Hide();
			this.m_hidedate = DateTime.Now;
			this.Start();
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			base.Hide();
			this.m_hidedate = DateTime.Now;
			this.Start();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Application.DoEvents();
			DateTime now = DateTime.Now;
			if (this.m_date.AddMinutes(5.0) < now)
			{
				this.timer1.Enabled = false;
				base.Hide();
				this.m_hidedate = DateTime.Now;
				this.Start();
			}
		}

		private void FrmShowUpdata_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			base.Hide();
			this.m_hidedate = DateTime.Now;
			this.Start();
		}

		private void Start()
		{
			if (this.sync_Info_PcToDeving)
			{
				this.m_hidedate = DateTime.Now;
				this.timer2.Interval = 5000;
				this.timer2.Enabled = true;
			}
		}

		private void FrmShowUpdata_MouseEnter(object sender, EventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			if (this.m_hidedate.AddMinutes(10.0) < now && this.sync_Info_PcToDeving)
			{
				this.ShowEx();
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
			this.components = new Container();
			this.label1 = new Label();
			this.btn_addPerson = new ButtonX();
			this.buttonX1 = new ButtonX();
			this.timer1 = new Timer(this.components);
			this.timer2 = new Timer(this.components);
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(5, 11);
			this.label1.Name = "label1";
			this.label1.Size = new Size(227, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "有变动的数据没有同步到设备,请及时同步";
			this.btn_addPerson.AccessibleRole = AccessibleRole.PushButton;
			this.btn_addPerson.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_addPerson.Location = new Point(14, 51);
			this.btn_addPerson.Name = "btn_addPerson";
			this.btn_addPerson.Size = new Size(152, 23);
			this.btn_addPerson.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_addPerson.TabIndex = 4;
			this.btn_addPerson.Text = "立即同步";
			this.btn_addPerson.Click += this.btn_addPerson_Click;
			this.buttonX1.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX1.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX1.Location = new Point(183, 51);
			this.buttonX1.Name = "buttonX1";
			this.buttonX1.Size = new Size(152, 23);
			this.buttonX1.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX1.TabIndex = 5;
			this.buttonX1.Text = "稍后提示";
			this.buttonX1.Click += this.buttonX1_Click;
			this.timer1.Tick += this.timer1_Tick;
			this.timer2.Tick += this.timer2_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(349, 87);
			base.Controls.Add(this.buttonX1);
			base.Controls.Add(this.btn_addPerson);
			base.Controls.Add(this.label1);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FrmShowUpdata";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.Manual;
			this.Text = "同步提示";
			base.FormClosing += this.FrmShowUpdata_FormClosing;
			base.MouseEnter += this.FrmShowUpdata_MouseEnter;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
