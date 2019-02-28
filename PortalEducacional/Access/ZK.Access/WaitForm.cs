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
using System.Resources;
using System.Timers;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class WaitForm : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private bool m_isCanClose = false;

		private static WaitForm m_instance = null;

		private System.Timers.Timer m_time = new System.Timers.Timer();

		private int m_count = 0;

		private string m_lastInfo = string.Empty;

		private int curr_prg = 0;

		private IContainer components = null;

		private Panel panel1;

		private TextBox txt_info;

		private ProgressBar prg_info;

		private Label lb_closeInfo;

		private ButtonX btn_close;

		private ButtonX btn_stop;

		public static WaitForm Instance
		{
			get
			{
				if (WaitForm.m_instance == null)
				{
					WaitForm.m_instance = new WaitForm();
				}
				return WaitForm.m_instance;
			}
		}

		public string LastInfo => this.m_lastInfo;

		public WaitForm()
		{
			this.InitializeComponent();
			this.m_time.Enabled = false;
			try
			{
				ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(WaitForm));
				if (componentResourceManager != null)
				{
					base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
				}
			}
			catch (OutOfMemoryException)
			{
			}
			catch (ArgumentNullException)
			{
			}
			catch (MissingManifestResourceException)
			{
			}
			catch (IOException)
			{
			}
			initLang.LocaleForm(this, base.Name);
			this.m_isCanClose = false;
		}

		private void OnShow(object sender, EventArgs e)
		{
			try
			{
				if (!base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new EventHandler(this.OnShow), sender, e);
					}
					else
					{
						this.m_time.Elapsed -= this.m_time_Elapsed;
						this.m_time.Enabled = false;
						this.m_count = 0;
						this.m_lastInfo = string.Empty;
						this.txt_info.Text = ShowMsgInfos.GetInfo("SpleaseWaiting", "正在处理，请稍候...") + "\r\n";
						this.lb_closeInfo.Text = "";
						this.prg_info.Value = 0;
						this.m_isCanClose = false;
						this.btn_stop.Enabled = false;
						this.btn_close.Enabled = false;
						base.TopMost = true;
						base.TopLevel = true;
						base.Show();
					}
				}
			}
			catch
			{
			}
		}

		private void OnShowInfo(string info)
		{
			try
			{
				if (!base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new ShowInfo(this.OnShowInfo), info);
					}
					else
					{
						this.m_time.Elapsed -= this.m_time_Elapsed;
						this.m_time.Enabled = false;
						this.m_count = 0;
						this.m_lastInfo = string.Empty;
						this.txt_info.Text = info;
						this.lb_closeInfo.Text = "";
						this.prg_info.Value = 0;
						this.m_isCanClose = false;
						this.btn_stop.Enabled = false;
						this.btn_close.Enabled = false;
						base.TopMost = true;
						base.TopLevel = true;
						base.Show();
					}
				}
			}
			catch
			{
			}
		}

		public void ShowEx()
		{
			this.OnShow(this, null);
		}

		public void ShowEx(string info)
		{
			this.OnShowInfo(info);
		}

		public void Stop()
		{
			this.m_isCanClose = true;
		}

		private void OnHide(object sender, EventArgs e)
		{
			try
			{
				if (!base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new EventHandler(this.OnHide), sender, e);
					}
					else
					{
						this.m_count = 0;
						if (base.Visible)
						{
							this.m_time.Interval = 1000.0;
							this.m_time.Elapsed -= this.m_time_Elapsed;
							this.m_time.Elapsed += this.m_time_Elapsed;
							this.m_time.Enabled = true;
							this.m_isCanClose = true;
							this.btn_stop.Enabled = true;
							this.btn_close.Enabled = true;
							Application.DoEvents();
						}
					}
				}
			}
			catch
			{
			}
		}

		public void HideEx(bool b = false)
		{
			if (!b)
			{
				this.OnHide(this, null);
			}
			else
			{
				this.HideEx(this, null);
			}
		}

		public void HideEx(object sender, EventArgs e)
		{
			if (!base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new EventHandler(this.HideEx), sender, e);
				}
				else
				{
					base.Hide();
				}
			}
		}

		private void m_time_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				if (this.m_count < 5)
				{
					int num = 5 - this.m_count;
					this.ShowInfosEx(ShowMsgInfos.GetInfo("SWaitForm_FormClosing", "窗口将在{0}秒后自动关闭").Replace("{0}", num.ToString()));
					this.m_isCanClose = true;
					this.m_count++;
				}
				else
				{
					this.m_time.Enabled = false;
					this.HideEx(sender, null);
				}
			}
			catch
			{
				this.m_time.Enabled = false;
				this.HideEx(sender, null);
			}
		}

		public void ShowInfosEx(string infoStr)
		{
			try
			{
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new ShowInfo(this.ShowInfosEx), infoStr);
					}
					else
					{
						this.lb_closeInfo.Text = infoStr;
					}
				}
			}
			catch
			{
			}
		}

		public void ShowInfos(string infoStr)
		{
			try
			{
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new ShowInfo(this.ShowInfos), infoStr);
					}
					else
					{
						this.m_lastInfo = infoStr;
						infoStr = infoStr.TrimEnd("\r\n".ToCharArray());
						this.txt_info.AppendText(infoStr + "\r\n");
						this.Refresh();
					}
				}
			}
			catch
			{
			}
		}

		public void ShowProgress(int prg)
		{
			try
			{
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new ShowProgressHandle(this.ShowProgress), prg);
					}
					else
					{
						if (prg >= 100)
						{
							prg = 100;
						}
						if (prg < 0)
						{
							prg = 0;
						}
						this.prg_info.Value = prg;
						this.curr_prg = prg;
						this.Refresh();
					}
				}
			}
			catch
			{
			}
		}

		private void WaitForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				e.Cancel = true;
				if (this.m_isCanClose)
				{
					this.m_time.Enabled = false;
					this.m_count = 0;
					base.Hide();
				}
				else
				{
					this.HideEx(false);
				}
			}
			catch
			{
			}
		}

		private void btn_close_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_isCanClose)
				{
					this.m_time.Enabled = false;
					this.m_count = 0;
					this.btn_stop.Enabled = false;
					this.btn_close.Enabled = false;
					base.Hide();
				}
			}
			catch
			{
			}
		}

		private void btn_stop_Click(object sender, EventArgs e)
		{
			this.m_time.Elapsed -= this.m_time_Elapsed;
			this.m_time.Enabled = false;
			this.btn_stop.Enabled = false;
			this.lb_closeInfo.Text = "";
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
			this.panel1 = new Panel();
			this.btn_close = new ButtonX();
			this.btn_stop = new ButtonX();
			this.lb_closeInfo = new Label();
			this.txt_info = new TextBox();
			this.prg_info = new ProgressBar();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panel1.BorderStyle = BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.btn_close);
			this.panel1.Controls.Add(this.btn_stop);
			this.panel1.Controls.Add(this.lb_closeInfo);
			this.panel1.Controls.Add(this.txt_info);
			this.panel1.Controls.Add(this.prg_info);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(602, 257);
			this.panel1.TabIndex = 3;
			this.btn_close.AccessibleRole = AccessibleRole.PushButton;
			this.btn_close.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_close.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_close.Location = new Point(507, 224);
			this.btn_close.Name = "btn_close";
			this.btn_close.Size = new Size(82, 23);
			this.btn_close.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_close.TabIndex = 7;
			this.btn_close.Text = "关闭";
			this.btn_close.Click += this.btn_close_Click;
			this.btn_stop.AccessibleRole = AccessibleRole.PushButton;
			this.btn_stop.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_stop.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_stop.Location = new Point(351, 224);
			this.btn_stop.Name = "btn_stop";
			this.btn_stop.Size = new Size(140, 23);
			this.btn_stop.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_stop.TabIndex = 6;
			this.btn_stop.Text = "暂停关闭";
			this.btn_stop.Click += this.btn_stop_Click;
			this.lb_closeInfo.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.lb_closeInfo.AutoSize = true;
			this.lb_closeInfo.ForeColor = Color.Red;
			this.lb_closeInfo.Location = new Point(9, 227);
			this.lb_closeInfo.Name = "lb_closeInfo";
			this.lb_closeInfo.Size = new Size(11, 12);
			this.lb_closeInfo.TabIndex = 5;
			this.lb_closeInfo.Text = "w";
			this.txt_info.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.txt_info.Location = new Point(11, 40);
			this.txt_info.Multiline = true;
			this.txt_info.Name = "txt_info";
			this.txt_info.ScrollBars = ScrollBars.Vertical;
			this.txt_info.Size = new Size(578, 176);
			this.txt_info.TabIndex = 4;
			this.prg_info.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.prg_info.Location = new Point(11, 11);
			this.prg_info.Name = "prg_info";
			this.prg_info.Size = new Size(578, 23);
			this.prg_info.TabIndex = 3;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(194, 217, 247);
			base.ClientSize = new Size(602, 257);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "WaitForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "进度";
			base.TopMost = true;
			base.FormClosing += this.WaitForm_FormClosing;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
