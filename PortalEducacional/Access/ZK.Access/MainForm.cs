/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.AdvTree;
using DevComponents.DotNetBar;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.DBUtility;
using ZK.Framework;
using ZK.Utils;

namespace ZK.Access
{
	public class MainForm : Office2007Form
	{
		public static IApplication _ia = null;

		public static bool _visitorEnabled = true;

		public static bool _elevatorEnabled = false;

		public Thread threadDownLoadRecords = null;

		private bool Down = false;

		private UserMainControl uMainControl;

		private bool Prompt = true;

		private IContainer components = null;

		private ToolStripPanel top_StripPanel;

		private ElementStyle elementStyle1;

		private ToolStripStatusLabel statusLabel1;

		private ToolStripStatusLabel lb_Opstatus;

		private System.Windows.Forms.Timer tmr_timeShow;

		private Node node1;

		public MainForm()
		{
			this.InitializeComponent();
			try
			{
				initLang.LocaleForm(this, base.Name);
				MainForm.SetZKText(this);
				this.InitApplication();
				try
				{
					this.uMainControl = new UserMainControl();
					base.Controls.Add(this.uMainControl);
					this.uMainControl.Dock = DockStyle.Fill;
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
			}
			catch (Exception ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
			}
			if (AppSite.Instance.GetNodeValueByName("NeedSync2ZKTime") == "1")
			{
				ImagesForm imagesForm = new ImagesForm();
				imagesForm.Show();
				Application.DoEvents();
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				CheckInOutBll checkInOutBll = new CheckInOutBll(MainForm._ia);
				ZKTimeNetLite.accessExce.ClearAllData();
				departmentsBll.Sync2ZKTime();
				userInfoBll.Sync2ZKTime();
				checkInOutBll.Sync2ZKTime();
				AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "0");
				AppSite.Instance.Save();
				imagesForm.Close();
			}
		}

		public static void SetZKText(Control ctl)
		{
		}

		private void InitApplication()
		{
			this.InitContent();
		}

		private void InitContent()
		{
			MainForm._ia.MainFrom = this;
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (SkinParameters.SkinOption != 1 && this.threadDownLoadRecords != null)
			{
				this.Down = false;
				Thread.Sleep(2000);
				if (this.threadDownLoadRecords != null)
				{
					try
					{
						this.threadDownLoadRecords.Abort();
					}
					catch (ThreadAbortException)
					{
					}
					catch (ThreadStateException)
					{
					}
					catch (SecurityException)
					{
					}
				}
			}
		}

		private void menu_Main_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			string text = Path.GetDirectoryName(Application.ExecutablePath) + "\\backup";
			string text2 = "backup-" + ((int)DateTime.Now.DayOfWeek).ToString() + ((AppSite.Instance.DataType == DataType.Access) ? ".mdb" : ".bak");
			string str = "temp-" + text2;
			WaitForm instance = WaitForm.Instance;
			instance.ShowProgress(0);
			instance.ShowEx();
			Application.DoEvents();
			try
			{
				instance.ShowInfos("Fazendo backup da base de dados...");
				instance.ShowProgress(0);
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				if (File.Exists(text2))
				{
					str = "temp-" + text2;
				}
				instance.ShowProgress(50);
				switch (AppSite.Instance.DataType)
				{
				case DataType.Access:
					DbHelperOleDb.BackupDataBase(text + "\\" + str);
					break;
				case DataType.SqlServer:
					DbHelperSQL.BackupDataBase(text + "\\" + str, 240);
					break;
				}
				if (File.Exists(text + "\\" + str))
				{
					if (File.Exists(text + "\\" + text2))
					{
						File.Delete(text + "\\" + text2);
					}
					File.Move(text + "\\" + str, text + "\\" + text2);
					instance.ShowProgress(100);
					goto end_IL_0079;
				}
				this.Prompt = true;
				throw new Exception("Não foi possível criar arquivo de backup.");
				end_IL_0079:;
			}
			catch (Exception ex)
			{
				this.Prompt = true;
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("BackupDBFailed", "O backup do banco de dados falhou") + ": " + ex.Message);
			}
			finally
			{
				instance.HideEx(false);
			}
			this.CloseThisForm(e, !Program.IsRestart && this.Prompt);
		}

		private void ClearOwnedForms()
		{
			if (base.OwnedForms != null && base.OwnedForms.Length != 0)
			{
				for (int num = base.OwnedForms.Length - 1; num >= 0; num--)
				{
					base.OwnedForms[num].Owner = null;
				}
			}
		}

		private void CloseThisForm(FormClosingEventArgs e, bool IsPrompt = false)
		{
			if (SkinParameters.SkinOption == 1)
			{
				if (base.OwnedForms != null && base.OwnedForms.Length != 0)
				{
					this.ClearOwnedForms();
					this.Prompt = false;
					base.Close();
				}
			}
			else if (IsPrompt && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SExitSystem", "Deseja sair do sistema?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				e.Cancel = true;
			}
			else if (base.OwnedForms != null && base.OwnedForms.Length != 0)
			{
				this.ClearOwnedForms();
				this.Prompt = false;
				base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
			this.statusLabel1 = new ToolStripStatusLabel();
			this.lb_Opstatus = new ToolStripStatusLabel();
			this.top_StripPanel = new ToolStripPanel();
			this.elementStyle1 = new ElementStyle();
			this.tmr_timeShow = new System.Windows.Forms.Timer(this.components);
			this.node1 = new Node();
			base.SuspendLayout();
			this.statusLabel1.Name = "statusLabel1";
			this.statusLabel1.Size = new Size(41, 17);
			this.statusLabel1.Text = "状态：";
			this.lb_Opstatus.Name = "lb_Opstatus";
			this.lb_Opstatus.Size = new Size(11, 17);
			this.lb_Opstatus.Text = " ";
			this.top_StripPanel.Dock = DockStyle.Top;
			this.top_StripPanel.Location = new Point(0, 0);
			this.top_StripPanel.Name = "top_StripPanel";
			this.top_StripPanel.Orientation = Orientation.Horizontal;
			this.top_StripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.top_StripPanel.Size = new Size(838, 0);
			this.elementStyle1.Class = "";
			this.elementStyle1.Name = "elementStyle1";
			this.elementStyle1.TextColor = SystemColors.ControlText;
			this.node1.Name = "node1";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.CornflowerBlue;
			base.ClientSize = new Size(838, 535);
			base.Controls.Add(this.top_StripPanel);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.IsMdiContainer = true;
			base.Name = "MainForm";
			this.Text = "ZKAccess3.5门禁管理系统";
			base.WindowState = FormWindowState.Maximized;
			base.FormClosing += this.MainForm_FormClosing;
			base.FormClosed += this.MainForm_FormClosed;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
