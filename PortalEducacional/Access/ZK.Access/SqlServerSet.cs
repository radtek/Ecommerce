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
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.DBUtility;
using ZK.Utils;

namespace ZK.Access
{
	public class SqlServerSet : Office2007Form
	{
		private bool m_isok = false;

		private IContainer components = null;

		private PanelEx pnl_excel;

		private TextBox txt_data;

		private Label label2;

		private TextBox txt_pwd;

		private TextBox txt_user;

		private Label label1;

		private Label lb_split;

		private Label lb_dataSource;

		private TextBox txt_server;

		private PanelEx pnl_bottom;

		private ButtonX btn_cancel;

		private ButtonX btn_Next;

		private Label label6;

		private Label label5;

		private Label label4;

		private Label label3;

		public bool IsOk => this.m_isok;

		public SqlServerSet()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.btn_Next.Enabled = false;
			this.txt_server_TextChanged(null, null);
			if (AppSite.Instance.DataType == DataType.SqlServer)
			{
				DbHelperSQL.GetDatabaseAddress(DbHelperSQL.connectionString, out bool _, out string text, out string text2, out string text3, out string text4);
				this.txt_data.Text = text2;
				this.txt_pwd.Text = text4;
				this.txt_server.Text = text;
				this.txt_user.Text = text3;
			}
		}

		private void txt_server_TextChanged(object sender, EventArgs e)
		{
			this.btn_Next.Enabled = false;
			if (!string.IsNullOrEmpty(this.txt_data.Text) && !string.IsNullOrEmpty(this.txt_server.Text) && !string.IsNullOrEmpty(this.txt_user.Text) && !string.IsNullOrEmpty(this.txt_pwd.Text))
			{
				this.btn_Next.Enabled = true;
			}
		}

		private void btn_Next_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txt_data.Text) || string.IsNullOrEmpty(this.txt_server.Text) || string.IsNullOrEmpty(this.txt_user.Text) || string.IsNullOrEmpty(this.txt_pwd.Text))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("dataSetError", "数据库设置不正确"));
			}
			else
			{
				string format = "Data Source={0};Database={1};Password={2};User ID={3};";
				try
				{
					using (SqlConnection sqlConnection = new SqlConnection(string.Format(format, this.txt_server.Text, "master", this.txt_pwd.Text, this.txt_user.Text)))
					{
						sqlConnection.Open();
						if (sqlConnection.State != ConnectionState.Open)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("OpenConnectionFalse", "数据库链接打开失败"));
						}
						else
						{
							try
							{
								sqlConnection.Close();
								sqlConnection.ConnectionString = string.Format(format, this.txt_server.Text, this.txt_data.Text, this.txt_pwd.Text, this.txt_user.Text);
								sqlConnection.Open();
								using (SqlCommand sqlCommand = new SqlCommand(" select count(*) from auth_user ", sqlConnection))
								{
									int num = sqlCommand.ExecuteNonQuery();
									AppSite.Instance.SetNodeValue("ConnectionString", string.Format(format, this.txt_server.Text, this.txt_data.Text, this.txt_pwd.Text, this.txt_user.Text));
									AppSite.Instance.SetNodeValue("data", "sqlserver");
									if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "操作成功，是否重启软件以使之生效?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
									{
										Program.IsRestart = true;
									}
									this.m_isok = true;
									base.Close();
								}
							}
							catch (Exception)
							{
								DialogResult dialogResult = SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("RebuildDataBaseConfirm", "数据库不存在或不完整. 是否重建数据库?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
								if (dialogResult == DialogResult.Yes)
								{
									AppSite.Instance.SetNodeValue("ConnectionString", string.Format(format, this.txt_server.Text, this.txt_data.Text, this.txt_pwd.Text, this.txt_user.Text));
									AppSite.Instance.SetNodeValue("data", "sqlserver");
									AppSite.Instance.SetNodeValue("RebuildDataBase", "1");
									if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "操作成功，是否重启软件以使之生效?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
									{
										Program.IsRestart = true;
									}
									this.m_isok = true;
									base.Close();
								}
							}
							finally
							{
								sqlConnection.Close();
							}
						}
					}
				}
				catch (Exception ex2)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("OpenConnectionFalse", "数据库链接打开失败") + ":" + ex2.Message);
				}
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.m_isok = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SqlServerSet));
			this.pnl_excel = new PanelEx();
			this.label6 = new Label();
			this.label5 = new Label();
			this.label4 = new Label();
			this.label3 = new Label();
			this.txt_data = new TextBox();
			this.label2 = new Label();
			this.txt_pwd = new TextBox();
			this.txt_user = new TextBox();
			this.label1 = new Label();
			this.lb_split = new Label();
			this.lb_dataSource = new Label();
			this.txt_server = new TextBox();
			this.pnl_bottom = new PanelEx();
			this.btn_cancel = new ButtonX();
			this.btn_Next = new ButtonX();
			this.pnl_excel.SuspendLayout();
			this.pnl_bottom.SuspendLayout();
			base.SuspendLayout();
			this.pnl_excel.CanvasColor = SystemColors.Control;
			this.pnl_excel.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_excel.Controls.Add(this.txt_data);
			this.pnl_excel.Controls.Add(this.txt_pwd);
			this.pnl_excel.Controls.Add(this.txt_user);
			this.pnl_excel.Controls.Add(this.txt_server);
			this.pnl_excel.Controls.Add(this.label6);
			this.pnl_excel.Controls.Add(this.label5);
			this.pnl_excel.Controls.Add(this.label4);
			this.pnl_excel.Controls.Add(this.label3);
			this.pnl_excel.Controls.Add(this.label2);
			this.pnl_excel.Controls.Add(this.label1);
			this.pnl_excel.Controls.Add(this.lb_split);
			this.pnl_excel.Controls.Add(this.lb_dataSource);
			this.pnl_excel.Dock = DockStyle.Fill;
			this.pnl_excel.Location = new Point(0, 0);
			this.pnl_excel.Name = "pnl_excel";
			this.pnl_excel.Size = new Size(382, 175);
			this.pnl_excel.Style.Alignment = StringAlignment.Center;
			this.pnl_excel.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_excel.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_excel.Style.Border = eBorderType.SingleLine;
			this.pnl_excel.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_excel.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_excel.Style.GradientAngle = 90;
			this.pnl_excel.TabIndex = 5;
			this.label6.AutoSize = true;
			this.label6.BackColor = Color.Transparent;
			this.label6.ForeColor = Color.Red;
			this.label6.Location = new Point(360, 102);
			this.label6.Name = "label6";
			this.label6.Size = new Size(11, 12);
			this.label6.TabIndex = 30;
			this.label6.Text = "*";
			this.label5.AutoSize = true;
			this.label5.BackColor = Color.Transparent;
			this.label5.ForeColor = Color.Red;
			this.label5.Location = new Point(360, 138);
			this.label5.Name = "label5";
			this.label5.Size = new Size(11, 12);
			this.label5.TabIndex = 29;
			this.label5.Text = "*";
			this.label4.AutoSize = true;
			this.label4.BackColor = Color.Transparent;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(360, 66);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 28;
			this.label4.Text = "*";
			this.label3.AutoSize = true;
			this.label3.BackColor = Color.Transparent;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(360, 30);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 27;
			this.label3.Text = "*";
			this.txt_data.BackColor = SystemColors.Window;
			this.txt_data.Location = new Point(187, 133);
			this.txt_data.Name = "txt_data";
			this.txt_data.Size = new Size(172, 21);
			this.txt_data.TabIndex = 3;
			this.txt_data.Text = "access";
			this.txt_data.TextChanged += this.txt_server_TextChanged;
			this.label2.Location = new Point(12, 136);
			this.label2.Name = "label2";
			this.label2.Size = new Size(165, 12);
			this.label2.TabIndex = 14;
			this.label2.Text = "服务器上数据库";
			this.txt_pwd.BackColor = SystemColors.Window;
			this.txt_pwd.Location = new Point(187, 97);
			this.txt_pwd.Name = "txt_pwd";
			this.txt_pwd.PasswordChar = '*';
			this.txt_pwd.Size = new Size(172, 21);
			this.txt_pwd.TabIndex = 2;
			this.txt_pwd.TextChanged += this.txt_server_TextChanged;
			this.txt_user.BackColor = SystemColors.Window;
			this.txt_user.Location = new Point(187, 61);
			this.txt_user.Name = "txt_user";
			this.txt_user.Size = new Size(172, 21);
			this.txt_user.TabIndex = 1;
			this.txt_user.Text = "sa";
			this.txt_user.TextChanged += this.txt_server_TextChanged;
			this.label1.Location = new Point(12, 100);
			this.label1.Name = "label1";
			this.label1.Size = new Size(165, 12);
			this.label1.TabIndex = 11;
			this.label1.Text = "密码";
			this.lb_split.Location = new Point(12, 64);
			this.lb_split.Name = "lb_split";
			this.lb_split.Size = new Size(165, 12);
			this.lb_split.TabIndex = 10;
			this.lb_split.Text = "用户名称";
			this.lb_dataSource.Location = new Point(12, 28);
			this.lb_dataSource.Name = "lb_dataSource";
			this.lb_dataSource.Size = new Size(165, 12);
			this.lb_dataSource.TabIndex = 9;
			this.lb_dataSource.Text = "服务器名称";
			this.txt_server.BackColor = SystemColors.Window;
			this.txt_server.Location = new Point(187, 25);
			this.txt_server.Name = "txt_server";
			this.txt_server.Size = new Size(172, 21);
			this.txt_server.TabIndex = 0;
			this.txt_server.Text = "127.0.0.1";
			this.txt_server.TextChanged += this.txt_server_TextChanged;
			this.pnl_bottom.CanvasColor = SystemColors.Control;
			this.pnl_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_bottom.Controls.Add(this.btn_cancel);
			this.pnl_bottom.Controls.Add(this.btn_Next);
			this.pnl_bottom.Dock = DockStyle.Bottom;
			this.pnl_bottom.Location = new Point(0, 175);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(382, 48);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 4;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(289, 15);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_Next.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Next.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Next.Location = new Point(187, 15);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(82, 23);
			this.btn_Next.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Next.TabIndex = 4;
			this.btn_Next.Text = "确定";
			this.btn_Next.Click += this.btn_Next_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(382, 223);
			base.Controls.Add(this.pnl_excel);
			base.Controls.Add(this.pnl_bottom);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SqlServerSet";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "数据库";
			this.pnl_excel.ResumeLayout(false);
			this.pnl_excel.PerformLayout();
			this.pnl_bottom.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
