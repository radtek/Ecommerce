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
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.DBUtility;
using ZK.Utils;

namespace ZK.Access
{
	public class AccessSet : Office2007Form
	{
		private bool m_isok = false;

		private IContainer components = null;

		private PanelEx pnl_bottom;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		private PanelEx pnl_excel;

		private Label label1;

		private Label lb_dataSource;

		private ButtonX btn_open;

		private TextBox txt_dataSourceUrl;

		private TextBox txt_pwd;

		public bool IsOk => this.m_isok;

		public AccessSet()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.btn_ok.Enabled = false;
			this.m_isok = false;
			if (AppSite.Instance.DataType == DataType.Access)
			{
				string text = "";
				string text2 = "";
				DbHelperOleDb.GetDatabaseAddress(DbHelperOleDb.connectionString, ref text, ref text2);
				this.txt_dataSourceUrl.Text = text;
				this.txt_pwd.Text = text2;
			}
		}

		private void btn_open_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.FilterIndex = 0;
			openFileDialog.RestoreDirectory = true;
			openFileDialog.FileName = "";
			openFileDialog.Filter = "Access files (*.mdb)|*.mdb";
			openFileDialog.ShowDialog();
			string fileName = openFileDialog.FileName;
			if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
			{
				this.txt_dataSourceUrl.Text = fileName;
			}
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_dataSourceUrl.Text) && File.Exists(this.txt_dataSourceUrl.Text))
			{
				string text = $"Provider = Microsoft.Jet.OleDb.4.0;Jet OLEDB:Database Password={this.txt_pwd.Text}; Data Source ={this.txt_dataSourceUrl.Text};";
				try
				{
					using (OleDbConnection oleDbConnection = new OleDbConnection(text))
					{
						oleDbConnection.Open();
						if (oleDbConnection.State == ConnectionState.Open)
						{
							try
							{
								using (OleDbCommand oleDbCommand = new OleDbCommand(" select count(*) from auth_user ", oleDbConnection))
								{
									int num = oleDbCommand.ExecuteNonQuery();
									this.m_isok = true;
									AppSite.Instance.SetNodeValue("ConnectionString", text);
									AppSite.Instance.SetNodeValue("data", "access");
									if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "操作成功，是否重启软件以使之生效?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
									{
										Program.IsRestart = true;
									}
									base.Close();
								}
							}
							catch
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("sDataNoTables", "数据库里没有相关的表"));
							}
							finally
							{
								oleDbConnection.Close();
							}
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("OpenConnectionFalse", "数据库链接打开失败"));
						}
					}
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("OpenConnectionFalse", "数据库链接打开失败") + ":" + ex.Message);
				}
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "目标文件不存在"));
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.m_isok = false;
			base.Close();
		}

		private void txt_dataSourceUrl_TextChanged(object sender, EventArgs e)
		{
			this.btn_ok.Enabled = false;
			try
			{
				if (!string.IsNullOrEmpty(this.txt_dataSourceUrl.Text) && File.Exists(this.txt_dataSourceUrl.Text))
				{
					this.btn_ok.Enabled = true;
				}
			}
			catch
			{
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AccessSet));
			this.pnl_bottom = new PanelEx();
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			this.pnl_excel = new PanelEx();
			this.txt_pwd = new TextBox();
			this.label1 = new Label();
			this.lb_dataSource = new Label();
			this.btn_open = new ButtonX();
			this.txt_dataSourceUrl = new TextBox();
			this.pnl_bottom.SuspendLayout();
			this.pnl_excel.SuspendLayout();
			base.SuspendLayout();
			this.pnl_bottom.CanvasColor = SystemColors.Control;
			this.pnl_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_bottom.Controls.Add(this.btn_cancel);
			this.pnl_bottom.Controls.Add(this.btn_ok);
			this.pnl_bottom.Dock = DockStyle.Bottom;
			this.pnl_bottom.Location = new Point(0, 103);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(536, 53);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 2;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(442, 15);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(340, 15);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 3;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.pnl_excel.CanvasColor = SystemColors.Control;
			this.pnl_excel.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_excel.Controls.Add(this.txt_pwd);
			this.pnl_excel.Controls.Add(this.txt_dataSourceUrl);
			this.pnl_excel.Controls.Add(this.label1);
			this.pnl_excel.Controls.Add(this.lb_dataSource);
			this.pnl_excel.Controls.Add(this.btn_open);
			this.pnl_excel.Dock = DockStyle.Fill;
			this.pnl_excel.Location = new Point(0, 0);
			this.pnl_excel.Name = "pnl_excel";
			this.pnl_excel.Size = new Size(536, 103);
			this.pnl_excel.Style.Alignment = StringAlignment.Center;
			this.pnl_excel.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_excel.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_excel.Style.Border = eBorderType.SingleLine;
			this.pnl_excel.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_excel.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_excel.Style.GradientAngle = 90;
			this.pnl_excel.TabIndex = 3;
			this.txt_pwd.BackColor = SystemColors.Window;
			this.txt_pwd.Location = new Point(118, 63);
			this.txt_pwd.Name = "txt_pwd";
			this.txt_pwd.PasswordChar = '*';
			this.txt_pwd.Size = new Size(123, 21);
			this.txt_pwd.TabIndex = 2;
			this.label1.Location = new Point(12, 66);
			this.label1.Name = "label1";
			this.label1.Size = new Size(102, 12);
			this.label1.TabIndex = 11;
			this.label1.Text = "密码";
			this.lb_dataSource.Location = new Point(12, 28);
			this.lb_dataSource.Name = "lb_dataSource";
			this.lb_dataSource.Size = new Size(102, 12);
			this.lb_dataSource.TabIndex = 9;
			this.lb_dataSource.Text = "目标文件";
			this.btn_open.AccessibleRole = AccessibleRole.PushButton;
			this.btn_open.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_open.Location = new Point(445, 24);
			this.btn_open.Name = "btn_open";
			this.btn_open.Size = new Size(75, 23);
			this.btn_open.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_open.TabIndex = 1;
			this.btn_open.Text = "浏览";
			this.btn_open.Click += this.btn_open_Click;
			this.txt_dataSourceUrl.BackColor = SystemColors.Window;
			this.txt_dataSourceUrl.Location = new Point(118, 25);
			this.txt_dataSourceUrl.Name = "txt_dataSourceUrl";
			this.txt_dataSourceUrl.ReadOnly = true;
			this.txt_dataSourceUrl.Size = new Size(311, 21);
			this.txt_dataSourceUrl.TabIndex = 0;
			this.txt_dataSourceUrl.TextChanged += this.txt_dataSourceUrl_TextChanged;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(536, 156);
			base.Controls.Add(this.pnl_excel);
			base.Controls.Add(this.pnl_bottom);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AccessSet";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "数据库";
			this.pnl_bottom.ResumeLayout(false);
			this.pnl_excel.ResumeLayout(false);
			this.pnl_excel.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
