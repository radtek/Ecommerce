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
using System.IO;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.system
{
	public class Login : Office2007Form
	{
		private IContainer components = null;

		private Label lb_info;

		private Label lb_info1;

		private ButtonX Btn_Cancel;

		private ButtonX btn_OK;

		private TextBox txt_password;

		private TextBox txt_userName;

		private Label lab_newPassword;

		private Label lab_oldPassword;

		private ButtonX btn_close;

		public Login()
		{
			this.InitializeComponent();
			if (initLang.Lang != "chs")
			{
				if (SysInfos.IsZkTitle)
				{
					this.BackgroundImage = Resource.LoginEn_ZK_System;
				}
				else
				{
					this.BackgroundImage = Resource.LoginEn_System;
				}
			}
			else if (SysInfos.IsZkTitle)
			{
				this.BackgroundImage = Resource.LoginChs_ZK;
			}
			else
			{
				this.BackgroundImage = Resource.LoginChs;
			}
			if (File.Exists(Program.ApplicationFolder + "\\img\\login-back.png"))
			{
				Image image2 = this.BackgroundImage = new Bitmap(Program.ApplicationFolder + "\\img\\login-back.png");
			}
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_password.Text) && !string.IsNullOrEmpty(this.txt_userName.Text))
			{
				try
				{
					this.Cursor = Cursors.WaitCursor;
					AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
					AuthUser model = authUserBll.GetModel("admin");
					if (model != null)
					{
						SysInfos.AdminID = model.id;
						SysInfos.AdminRoleID = model.RoleID;
						AuthGroupBll authGroupBll = new AuthGroupBll(MainForm._ia);
						AuthGroup model2 = authGroupBll.GetModel(model.RoleID);
						if (model2 == null)
						{
							List<AuthGroup> modelList = authGroupBll.GetModelList(" name='administrator' ");
							if (modelList == null || modelList.Count == 0)
							{
								AuthGroup authGroup = new AuthGroup();
								authGroup.name = "administrator";
								authGroup.Permission = "333333333333333333333333333333333333333333333333333333333";
								authGroup.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
								authGroupBll.Add(authGroup);
								modelList = authGroupBll.GetModelList(" name='administrator' ");
							}
							if (modelList != null && modelList.Count > 0)
							{
								model.RoleID = modelList[0].id;
								authUserBll.Update(model);
								SysInfos.AdminRoleID = model.RoleID;
							}
						}
					}
					AuthUser authUser = null;
					authUser = ((!(this.txt_userName.Text == "admin")) ? authUserBll.GetModel(this.txt_userName.Text) : model);
					if (authUser != null)
					{
						if (authUser.password == this.txt_password.Text)
						{
							authUser.last_login = DateTime.Now;
							authUserBll.Update(authUser);
							if (authUser.Status >= 0 || authUser.username.ToString() == "admin")
							{
								if (authUser.username.ToString() != "admin")
								{
									AreaAdminBll areaAdminBll = new AreaAdminBll(MainForm._ia);
									List<AreaAdmin> modelList2 = areaAdminBll.GetModelList("user_id=" + authUser.id);
									if (modelList2 != null && modelList2.Count > 0)
									{
										string text = "(" + modelList2[0].area_id;
										for (int i = 1; i < modelList2.Count; i++)
										{
											text = text + "," + modelList2[i].area_id;
										}
										text = (SysInfos.Areas = text + ")");
									}
									DeptAdminBll deptAdminBll = new DeptAdminBll(MainForm._ia);
									List<DeptAdmin> modelList3 = deptAdminBll.GetModelList("user_id=" + authUser.id);
									if (modelList3 != null && modelList3.Count > 0)
									{
										string text3 = "(" + modelList3[0].dept_id;
										for (int j = 1; j < modelList3.Count; j++)
										{
											text3 = text3 + "," + modelList3[j].dept_id;
										}
										text3 = (SysInfos.Depts = text3 + ")");
									}
								}
								SysInfos.SysUserInfo = authUser;
								SysInfos.SysUserInfo.username = this.txt_userName.Text.ToLower();
								if (authUser.RoleID == SysInfos.AdminRoleID)
								{
									SysInfos.UserPermission = string.Empty;
								}
								else
								{
									AuthGroupBll authGroupBll2 = new AuthGroupBll(MainForm._ia);
									AuthGroup authGroup2 = null;
									authGroup2 = authGroupBll2.GetModel(authUser.RoleID);
									if (authGroup2 != null)
									{
										SysInfos.UserPermission = authGroup2.Permission;
									}
									else
									{
										SysInfos.UserPermission = string.Empty;
									}
								}
								DevCmds.CreateOperatorLogin = SysInfos.SysUserInfo.username;
								base.Close();
							}
							else
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserDisEnabled", "用户名已经被禁用,请联系管理员"));
							}
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdWrong", "用户名或者密码错误"));
						}
					}
					else
					{
						string text5 = AppSite.Instance.GetNodeValueByName("password");
						if (string.IsNullOrEmpty(text5))
						{
							text5 = "admin";
							AppSite.Instance.SetNodeValue("password", text5);
						}
						if (this.txt_userName.Text.Trim().ToLower() == "admin" && this.txt_password.Text == text5)
						{
							authUser = new AuthUser();
							authUser.username = "admin";
							authUser.password = text5;
							authUser.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
							authUser.last_login = DateTime.Now;
							AuthGroupBll authGroupBll3 = new AuthGroupBll(MainForm._ia);
							List<AuthGroup> modelList4 = authGroupBll3.GetModelList(" name='administrator' ");
							if (modelList4 == null || modelList4.Count == 0)
							{
								AuthGroup authGroup3 = new AuthGroup();
								authGroup3.name = "administrator";
								authGroup3.Permission = "333333333333333333333333333333333333333333333333333333333";
								authGroup3.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
								authGroupBll3.Add(authGroup3);
								modelList4 = authGroupBll3.GetModelList(" name='administrator' ");
							}
							if (modelList4 != null && modelList4.Count > 0)
							{
								authUser.RoleID = modelList4[0].id;
							}
							authUserBll.Add(authUser);
							authUser.id = authUserBll.GetMaxId() - 1;
							SysInfos.AdminID = authUser.id;
							SysInfos.SysUserInfo = authUser;
							SysInfos.UserPermission = string.Empty;
							base.Close();
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdWrong", "用户名或者密码错误"));
						}
					}
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdNull", "用户名或密码不能为空"));
			}
			this.Cursor = Cursors.Default;
		}

		private void Btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void Login_Load(object sender, EventArgs e)
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_close.Image = Resource.close;
				this.btn_close.HoverImage = Resource.closeMove;
			}
		}

		private void txt_userName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 50);
		}

		private void txt_password_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 18);
			if (e.KeyChar == '\r')
			{
				this.btn_OK_Click(null, null);
			}
		}

		private void btn_close_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_close_MouseEnter(object sender, EventArgs e)
		{
			this.btn_close.Image = Resource.closeMove;
		}

		private void btn_close_MouseLeave(object sender, EventArgs e)
		{
			this.btn_close.Image = Resource.close;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Login));
			this.lb_info = new Label();
			this.lb_info1 = new Label();
			this.Btn_Cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.txt_password = new TextBox();
			this.txt_userName = new TextBox();
			this.lab_newPassword = new Label();
			this.lab_oldPassword = new Label();
			this.btn_close = new ButtonX();
			base.SuspendLayout();
			this.lb_info.AutoSize = true;
			this.lb_info.BackColor = Color.Transparent;
			this.lb_info.ForeColor = Color.Red;
			this.lb_info.Location = new Point(302, 106);
			this.lb_info.Name = "lb_info";
			this.lb_info.Size = new Size(11, 13);
			this.lb_info.TabIndex = 37;
			this.lb_info.Text = "*";
			this.lb_info1.AutoSize = true;
			this.lb_info1.BackColor = Color.Transparent;
			this.lb_info1.ForeColor = Color.Red;
			this.lb_info1.Location = new Point(302, 142);
			this.lb_info1.Name = "lb_info1";
			this.lb_info1.Size = new Size(11, 13);
			this.lb_info1.TabIndex = 36;
			this.lb_info1.Text = "*";
			this.Btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.Btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.Btn_Cancel.Location = new Point(255, 174);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new Size(82, 25);
			this.Btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.Btn_Cancel.TabIndex = 4;
			this.Btn_Cancel.Text = "Cancelar";
			this.Btn_Cancel.Click += this.Btn_Cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(153, 174);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 25);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "OK";
			this.btn_OK.Click += this.btn_OK_Click;
			this.txt_password.Location = new Point(175, 138);
			this.txt_password.Name = "txt_password";
			this.txt_password.PasswordChar = '*';
			this.txt_password.Size = new Size(123, 20);
			this.txt_password.TabIndex = 2;
			this.txt_password.KeyPress += this.txt_password_KeyPress;
			this.txt_userName.Location = new Point(175, 102);
			this.txt_userName.Name = "txt_userName";
			this.txt_userName.Size = new Size(123, 20);
			this.txt_userName.TabIndex = 1;
			this.txt_userName.KeyPress += this.txt_userName_KeyPress;
			this.lab_newPassword.AutoSize = true;
			this.lab_newPassword.BackColor = Color.Transparent;
			this.lab_newPassword.Location = new Point(54, 142);
			this.lab_newPassword.Name = "lab_newPassword";
			this.lab_newPassword.Size = new Size(38, 13);
			this.lab_newPassword.TabIndex = 30;
			this.lab_newPassword.Text = "Senha";
			this.lab_newPassword.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_oldPassword.AutoSize = true;
			this.lab_oldPassword.BackColor = Color.Transparent;
			this.lab_oldPassword.Location = new Point(54, 106);
			this.lab_oldPassword.Name = "lab_oldPassword";
			this.lab_oldPassword.Size = new Size(51, 13);
			this.lab_oldPassword.TabIndex = 28;
			this.lab_oldPassword.Text = "Operador";
			this.lab_oldPassword.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_close.AccessibleRole = AccessibleRole.PushButton;
			this.btn_close.BackColor = Color.Transparent;
			this.btn_close.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_close.Image = Resource.close;
			this.btn_close.Location = new Point(333, 2);
			this.btn_close.Name = "btn_close";
			this.btn_close.Size = new Size(22, 21);
			this.btn_close.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_close.TabIndex = 38;
			this.btn_close.Click += this.btn_close_Click;
			this.btn_close.MouseEnter += this.btn_close_MouseEnter;
			this.btn_close.MouseLeave += this.btn_close_MouseLeave;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackgroundImage = Resource.LoginChs_ZK;
			this.BackgroundImageLayout = ImageLayout.Stretch;
			base.ClientSize = new Size(361, 223);
			base.Controls.Add(this.txt_password);
			base.Controls.Add(this.txt_userName);
			base.Controls.Add(this.btn_close);
			base.Controls.Add(this.lb_info);
			base.Controls.Add(this.lb_info1);
			base.Controls.Add(this.Btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_newPassword);
			base.Controls.Add(this.lab_oldPassword);
			base.FormBorderStyle = FormBorderStyle.None;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Login";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "用户登录";
			base.Load += this.Login_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
