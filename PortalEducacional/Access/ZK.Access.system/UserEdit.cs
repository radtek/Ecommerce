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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.system
{
	public class UserEdit : Office2007Form
	{
		private int m_id = 0;

		private List<AuthGroup> glist = null;

		private IContainer components = null;

		private TextBox txt_name;

		private Label lb_name;

		private Label lb_pwd;

		private Label lb_pwd2;

		private TextBox txt_pwd;

		private TextBox txt_pwd2;

		private Label lb_status;

		private Panel panel2;

		private RadioButton rdb_userok;

		private RadioButton rdb_userstop;

		private Label lb_role;

		private ComboBox cmb_role;

		private ButtonX btn_Cancel;

		private ButtonX btn_Ok;

		private TextBox txt_remrk;

		private Label lb_remark;

		private Label lbl_star1;

		private Label lbl_star2;

		private Label lbl_star3;

		private Label lbl_star4;

		public event EventHandler RefreshDataEvent = null;

		public UserEdit(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.LoadGroup();
				this.BindModel();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
			initLang.LocaleForm(this, base.Name);
		}

		private void LoadGroup()
		{
			this.cmb_role.Items.Clear();
			AuthGroupBll authGroupBll = new AuthGroupBll(MainForm._ia);
			this.glist = authGroupBll.GetModelList("");
			if (this.glist != null && this.glist.Count > 0)
			{
				for (int i = 0; i < this.glist.Count; i++)
				{
					this.cmb_role.Items.Add(this.glist[i].name);
				}
			}
			if (this.cmb_role.Items.Count > 0)
			{
				this.cmb_role.SelectedIndex = 0;
			}
			else
			{
				this.cmb_role.Items.Add("-----");
				this.cmb_role.SelectedIndex = 0;
			}
		}

		private void BindModel()
		{
			if (this.m_id > 0)
			{
				AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
				AuthUser model = authUserBll.GetModel(this.m_id);
				if (model != null)
				{
					this.txt_name.Text = model.username;
					this.txt_pwd.Text = model.password;
					this.txt_pwd2.Text = model.password;
					this.txt_name.ReadOnly = true;
					this.txt_remrk.Text = model.Remark;
					if (model.username.ToLower() == "admin")
					{
						this.rdb_userok.Checked = true;
						this.rdb_userstop.Checked = false;
						this.rdb_userstop.Enabled = false;
					}
					else if (model.Status >= 0)
					{
						this.rdb_userok.Checked = true;
						this.rdb_userstop.Checked = false;
					}
					else
					{
						this.rdb_userok.Checked = false;
						this.rdb_userstop.Checked = true;
					}
					if (this.glist != null && this.glist.Count > 0)
					{
						int num = 0;
						while (true)
						{
							if (num < this.glist.Count)
							{
								if (this.glist[num].id != model.RoleID)
								{
									num++;
									continue;
								}
								break;
							}
							return;
						}
						this.cmb_role.SelectedIndex = num;
					}
				}
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(this.txt_name.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
					this.txt_name.Focus();
				}
				else if (!string.IsNullOrEmpty(this.txt_pwd.Text.Trim()) && this.txt_pwd.Text != this.txt_pwd2.Text.Trim())
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SPasswdWrongOrNull", "密码不一致或为空"));
				}
				else if (this.txt_pwd.Text.Length < 4 || this.txt_pwd.Text.Length > 18)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserPasswordLength", "密码长度必须大于4位,小于18位"));
				}
				else
				{
					try
					{
						AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
						AuthUser authUser = null;
						if (this.m_id > 0)
						{
							authUser = authUserBll.GetModel(this.m_id);
							if (authUser != null && !string.IsNullOrEmpty(this.txt_pwd.Text.Trim()) && this.txt_pwd.Text.Trim() == this.txt_pwd2.Text.Trim())
							{
								authUser.password = this.txt_pwd.Text;
							}
						}
						if (authUser == null)
						{
							if (authUserBll.Exists(this.txt_name.Text.Trim()))
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserRoleExist", "账号已经存在"));
							}
							else
							{
								authUser = new AuthUser();
								if (!string.IsNullOrEmpty(this.txt_pwd.Text.Trim()) && this.txt_pwd.Text.Trim() == this.txt_pwd2.Text.Trim())
								{
									authUser.password = this.txt_pwd.Text.Trim();
									goto IL_0244;
								}
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SPasswdWrongOrNull", "密码不一致或为空"));
							}
							goto end_IL_00e5;
						}
						goto IL_0244;
						IL_0244:
						authUser.Remark = this.txt_remrk.Text;
						authUser.username = this.txt_name.Text.Trim();
						if (this.glist != null && this.glist.Count > this.cmb_role.SelectedIndex && this.cmb_role.SelectedIndex >= 0)
						{
							authUser.RoleID = this.glist[this.cmb_role.SelectedIndex].id;
						}
						if (this.rdb_userok.Checked)
						{
							authUser.Status = 1;
						}
						else
						{
							authUser.Status = -1;
						}
						if (this.m_id > 0)
						{
							if (authUserBll.Update(authUser))
							{
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
								base.Close();
							}
							else
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
							}
						}
						else
						{
							try
							{
								authUserBll.Add(authUser);
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
								base.Close();
							}
							catch (Exception)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
							}
						}
						end_IL_00e5:;
					}
					catch (Exception ex2)
					{
						SysDialogs.ShowWarningMessage(ex2.Message);
					}
				}
			}
			catch (Exception ex3)
			{
				SysDialogs.ShowWarningMessage(ex3.Message);
			}
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 50);
		}

		private void txt_remrk_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 25);
		}

		private void txt_pwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 18);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserEdit));
			this.txt_name = new TextBox();
			this.lb_name = new Label();
			this.lb_pwd = new Label();
			this.lb_pwd2 = new Label();
			this.txt_pwd = new TextBox();
			this.txt_pwd2 = new TextBox();
			this.lb_status = new Label();
			this.panel2 = new Panel();
			this.rdb_userok = new RadioButton();
			this.rdb_userstop = new RadioButton();
			this.lb_role = new Label();
			this.cmb_role = new ComboBox();
			this.btn_Cancel = new ButtonX();
			this.btn_Ok = new ButtonX();
			this.txt_remrk = new TextBox();
			this.lb_remark = new Label();
			this.lbl_star1 = new Label();
			this.lbl_star2 = new Label();
			this.lbl_star3 = new Label();
			this.lbl_star4 = new Label();
			this.panel2.SuspendLayout();
			base.SuspendLayout();
			this.txt_name.Location = new Point(139, 7);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(162, 21);
			this.txt_name.TabIndex = 1;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.lb_name.Location = new Point(12, 12);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(121, 12);
			this.lb_name.TabIndex = 3;
			this.lb_name.Text = "用户名";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_pwd.Location = new Point(12, 44);
			this.lb_pwd.Name = "lb_pwd";
			this.lb_pwd.Size = new Size(121, 12);
			this.lb_pwd.TabIndex = 5;
			this.lb_pwd.Text = "密码";
			this.lb_pwd.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_pwd2.Location = new Point(12, 75);
			this.lb_pwd2.Name = "lb_pwd2";
			this.lb_pwd2.Size = new Size(121, 12);
			this.lb_pwd2.TabIndex = 6;
			this.lb_pwd2.Text = "确认密码";
			this.lb_pwd2.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_pwd.Location = new Point(139, 39);
			this.txt_pwd.Name = "txt_pwd";
			this.txt_pwd.PasswordChar = '*';
			this.txt_pwd.Size = new Size(162, 21);
			this.txt_pwd.TabIndex = 2;
			this.txt_pwd.KeyPress += this.txt_pwd_KeyPress;
			this.txt_pwd2.Location = new Point(139, 70);
			this.txt_pwd2.Name = "txt_pwd2";
			this.txt_pwd2.PasswordChar = '*';
			this.txt_pwd2.Size = new Size(162, 21);
			this.txt_pwd2.TabIndex = 3;
			this.txt_pwd2.KeyPress += this.txt_pwd_KeyPress;
			this.lb_status.Location = new Point(12, 173);
			this.lb_status.Name = "lb_status";
			this.lb_status.Size = new Size(121, 12);
			this.lb_status.TabIndex = 21;
			this.lb_status.Text = "状态";
			this.lb_status.TextAlign = ContentAlignment.MiddleLeft;
			this.panel2.Controls.Add(this.rdb_userstop);
			this.panel2.Controls.Add(this.rdb_userok);
			this.panel2.Location = new Point(139, 166);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(176, 23);
			this.panel2.TabIndex = 6;
			this.rdb_userok.AutoSize = true;
			this.rdb_userok.Checked = true;
			this.rdb_userok.Location = new Point(3, 3);
			this.rdb_userok.Name = "rdb_userok";
			this.rdb_userok.Size = new Size(47, 16);
			this.rdb_userok.TabIndex = 7;
			this.rdb_userok.TabStop = true;
			this.rdb_userok.Text = "正常";
			this.rdb_userok.UseVisualStyleBackColor = true;
			this.rdb_userstop.AutoSize = true;
			this.rdb_userstop.Location = new Point(76, 3);
			this.rdb_userstop.Name = "rdb_userstop";
			this.rdb_userstop.Size = new Size(47, 16);
			this.rdb_userstop.TabIndex = 8;
			this.rdb_userstop.Text = "禁用";
			this.rdb_userstop.UseVisualStyleBackColor = true;
			this.lb_role.Location = new Point(12, 108);
			this.lb_role.Name = "lb_role";
			this.lb_role.Size = new Size(121, 12);
			this.lb_role.TabIndex = 25;
			this.lb_role.Text = "角色";
			this.lb_role.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_role.FormattingEnabled = true;
			this.cmb_role.Location = new Point(139, 103);
			this.cmb_role.Name = "cmb_role";
			this.cmb_role.Size = new Size(162, 20);
			this.cmb_role.TabIndex = 4;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.Location = new Point(233, 205);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(82, 23);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 10;
			this.btn_Cancel.Text = "取消";
			this.btn_Cancel.Click += this.btn_Cancel_Click;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(139, 205);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 9;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.txt_remrk.Location = new Point(139, 133);
			this.txt_remrk.Name = "txt_remrk";
			this.txt_remrk.Size = new Size(162, 21);
			this.txt_remrk.TabIndex = 5;
			this.txt_remrk.KeyPress += this.txt_remrk_KeyPress;
			this.lb_remark.Location = new Point(12, 138);
			this.lb_remark.Name = "lb_remark";
			this.lb_remark.Size = new Size(121, 12);
			this.lb_remark.TabIndex = 29;
			this.lb_remark.Text = "备注";
			this.lb_remark.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_star1.AutoSize = true;
			this.lbl_star1.ForeColor = Color.Red;
			this.lbl_star1.Location = new Point(304, 12);
			this.lbl_star1.Name = "lbl_star1";
			this.lbl_star1.Size = new Size(11, 12);
			this.lbl_star1.TabIndex = 37;
			this.lbl_star1.Text = "*";
			this.lbl_star2.AutoSize = true;
			this.lbl_star2.ForeColor = Color.Red;
			this.lbl_star2.Location = new Point(304, 44);
			this.lbl_star2.Name = "lbl_star2";
			this.lbl_star2.Size = new Size(11, 12);
			this.lbl_star2.TabIndex = 38;
			this.lbl_star2.Text = "*";
			this.lbl_star3.AutoSize = true;
			this.lbl_star3.ForeColor = Color.Red;
			this.lbl_star3.Location = new Point(304, 75);
			this.lbl_star3.Name = "lbl_star3";
			this.lbl_star3.Size = new Size(11, 12);
			this.lbl_star3.TabIndex = 39;
			this.lbl_star3.Text = "*";
			this.lbl_star4.AutoSize = true;
			this.lbl_star4.ForeColor = Color.Red;
			this.lbl_star4.Location = new Point(304, 108);
			this.lbl_star4.Name = "lbl_star4";
			this.lbl_star4.Size = new Size(11, 12);
			this.lbl_star4.TabIndex = 40;
			this.lbl_star4.Text = "*";
			base.AcceptButton = this.btn_Ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(323, 240);
			base.Controls.Add(this.txt_remrk);
			base.Controls.Add(this.cmb_role);
			base.Controls.Add(this.txt_pwd2);
			base.Controls.Add(this.txt_pwd);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.lbl_star4);
			base.Controls.Add(this.lbl_star3);
			base.Controls.Add(this.lbl_star2);
			base.Controls.Add(this.lbl_star1);
			base.Controls.Add(this.lb_remark);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.lb_role);
			base.Controls.Add(this.panel2);
			base.Controls.Add(this.lb_status);
			base.Controls.Add(this.lb_pwd2);
			base.Controls.Add(this.lb_pwd);
			base.Controls.Add(this.lb_name);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UserEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
