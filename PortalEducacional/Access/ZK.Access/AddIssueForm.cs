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
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class AddIssueForm : Office2007Form
	{
		private int m_id = 0;

		private int u_id = 0;

		private string cardNumber;

		private bool m_isopen = false;

		private IContainer components = null;

		private Label label1;

		private Label label2;

		private TextBox txt_card;

		private ButtonX btn_Ok;

		private ButtonX btn_cancel;

		private ComboBox cmb_user;

		private Label label3;

		private Label label4;

		public event EventHandler RefreshDataEvent = null;

		public AddIssueForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
		}

		public AddIssueForm(int id)
			: this()
		{
			this.m_id = id;
			initLang.LocaleForm(this, base.Name);
			this.BindModel();
		}

		public AddIssueForm(UserInfo user)
			: this()
		{
			initLang.LocaleForm(this, base.Name);
			if (user != null)
			{
				this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				this.u_id = user.UserId;
				if (!string.IsNullOrEmpty(user.Name))
				{
					this.cmb_user.Items.Add(user.BadgeNumber + "-" + user.Name);
					this.cmb_user.Text = user.BadgeNumber + "-" + user.Name;
				}
				else
				{
					this.cmb_user.Items.Add(user.BadgeNumber);
					this.cmb_user.Text = user.BadgeNumber;
				}
				this.cmb_user.Enabled = false;
			}
		}

		private void getNextPin()
		{
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void BindModel()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
					PersonnelIssuecard model = personnelIssuecardBll.GetModel(this.m_id);
					if (model != null)
					{
						UserInfo model2 = new UserInfoBll(MainForm._ia).GetModel(model.UserID_id);
						if (model2 != null)
						{
							if (!string.IsNullOrEmpty(model2.Name))
							{
								this.cmb_user.Items.Add(model2.BadgeNumber + "-" + model2.Name);
								this.cmb_user.Text = model2.BadgeNumber + "-" + model2.Name;
							}
							else
							{
								this.cmb_user.Items.Add(model2.BadgeNumber);
								this.cmb_user.Text = model2.BadgeNumber;
							}
							this.cmb_user.Enabled = false;
						}
						else
						{
							ComboBox.ObjectCollection items = this.cmb_user.Items;
							int userID_id = model.UserID_id;
							items.Add(userID_id.ToString());
							ComboBox comboBox = this.cmb_user;
							userID_id = model.UserID_id;
							comboBox.Text = userID_id.ToString();
						}
						this.u_id = model.UserID_id;
						this.txt_card.Text = model.cardno;
						this.cardNumber = model.cardno;
					}
				}
				else
				{
					this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void selectedpersonnelForm_SelectUserEvent(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					UserInfo userInfo = sender as UserInfo;
					if (userInfo != null)
					{
						this.u_id = userInfo.UserId;
						if (!string.IsNullOrEmpty(userInfo.Name))
						{
							this.cmb_user.Items.Add(userInfo.BadgeNumber + "-" + userInfo.Name);
							this.cmb_user.Text = userInfo.BadgeNumber + "-" + userInfo.Name;
						}
						else
						{
							this.cmb_user.Items.Add(userInfo.BadgeNumber);
							this.cmb_user.Text = userInfo.BadgeNumber;
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void txt_user_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void txt_user_Click(object sender, EventArgs e)
		{
			if (!this.m_isopen)
			{
				this.m_isopen = true;
				SelectedPersonnelForm selectedPersonnelForm = new SelectedPersonnelForm();
				selectedPersonnelForm.SelectUserEvent += this.selectedpersonnelForm_SelectUserEvent;
				selectedPersonnelForm.ShowDialog();
				selectedPersonnelForm.SelectUserEvent -= this.selectedpersonnelForm_SelectUserEvent;
				this.m_isopen = false;
			}
		}

		private bool CheckData()
		{
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				if (this.m_id > 0)
				{
					if (string.IsNullOrEmpty(this.txt_card.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardCanNotBeEmpty", "卡号不能为空!"));
						this.txt_card.Focus();
						return false;
					}
					if (this.cardNumber != this.txt_card.Text && personnelIssuecardBll.ExistsCard(this.txt_card.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardNumberRepeat", "卡号重复!"));
						this.txt_card.Focus();
						return false;
					}
				}
				else if (this.u_id > 0)
				{
					if (string.IsNullOrEmpty(this.cmb_user.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SPersonnelCanNotBeEmpty", "人员不能为空!"));
						this.cmb_user.Focus();
						return false;
					}
					if (string.IsNullOrEmpty(this.txt_card.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardCanNotBeEmpty", "卡号不能为空!"));
						this.txt_card.Focus();
						return false;
					}
					if (personnelIssuecardBll.ExistsCard(this.txt_card.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardNumberRepeat", "卡号重复!"));
						this.txt_card.Focus();
						return false;
					}
					if (personnelIssuecardBll.Search(this.u_id) > 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("cardHaveBeenIssued", "该人员已发卡!"));
						this.cmb_user.Focus();
						return false;
					}
				}
				else
				{
					if (string.IsNullOrEmpty(this.cmb_user.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SPersonnelCanNotBeEmpty", "人员不能为空!"));
						this.cmb_user.Focus();
						return false;
					}
					if (string.IsNullOrEmpty(this.txt_card.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardCanNotBeEmpty", "卡号不能为空!"));
						this.txt_card.Focus();
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			bool flag = false;
			try
			{
				if (this.CheckData())
				{
					PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					PersonnelIssuecard personnelIssuecard = null;
					UserInfo userInfo = null;
					if (this.m_id > 0)
					{
						personnelIssuecard = personnelIssuecardBll.GetModel(this.m_id);
						if (personnelIssuecard.UserID_id > 0 && personnelIssuecard.UserID_id != this.u_id)
						{
							userInfo = userInfoBll.GetModel(personnelIssuecard.UserID_id);
							if (userInfo != null)
							{
								userInfo.CardNo = "";
								userInfoBll.Update(userInfo);
								if (CommandServer.AddCmd(userInfo, false, ""))
								{
									FrmShowUpdata.Instance.ShowEx();
								}
							}
						}
					}
					if (personnelIssuecard == null)
					{
						personnelIssuecard = new PersonnelIssuecard();
						personnelIssuecard.isvalid = true;
						personnelIssuecard.create_time = DateTime.Now;
					}
					personnelIssuecard.UserID_id = this.u_id;
					personnelIssuecard.cardno = this.txt_card.Text;
					if (personnelIssuecard.id > 0)
					{
						personnelIssuecardBll.Update(personnelIssuecard);
					}
					else
					{
						personnelIssuecardBll.Add(personnelIssuecard);
					}
					userInfo = userInfoBll.GetModel(this.u_id);
					if (userInfo != null)
					{
						userInfo.CardNo = this.txt_card.Text;
						userInfoBll.Update(userInfo);
						if (CommandServer.AddCmd(userInfo, false, ""))
						{
							FrmShowUpdata.Instance.ShowEx();
						}
					}
					if (this.RefreshDataEvent != null)
					{
						this.RefreshDataEvent(personnelIssuecard, null);
					}
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void txt_card_KeyPress(object sender, KeyPressEventArgs e)
		{
			switch (AccCommon.CodeVersion)
			{
			case CodeVersionType.JapanAF:
				if (e.KeyChar != '\b' && e.KeyChar != '\r' && (e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'A' || e.KeyChar > 'F') && (e.KeyChar < 'a' || e.KeyChar > 'f'))
				{
					e.Handled = true;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SHexNumberOnly", "仅支持输入十六进制数"));
				}
				else if (e.KeyChar == '0' && this.txt_card.SelectionStart == 0 && this.txt_card.TextLength > this.txt_card.SelectionLength)
				{
					e.Handled = true;
				}
				else if (this.txt_card.Text == "0" && e.KeyChar != '\b' && e.KeyChar != '\r' && this.txt_card.SelectionLength == 0 && this.txt_card.SelectionStart == 1)
				{
					this.txt_card.Text = e.KeyChar.ToString();
					this.txt_card.SelectionStart = 1;
					e.Handled = true;
				}
				else if (this.txt_card.SelectionLength == 0 && this.txt_card.Text.Length >= 16 && e.KeyChar != '\b' && e.KeyChar != '\r')
				{
					e.Handled = true;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputIsOutLen", "输入超出有效长度"));
				}
				break;
			case CodeVersionType.Main:
				CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
				break;
			default:
				CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
				break;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddIssueForm));
			this.label1 = new Label();
			this.label2 = new Label();
			this.txt_card = new TextBox();
			this.btn_Ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.cmb_user = new ComboBox();
			this.label3 = new Label();
			this.label4 = new Label();
			base.SuspendLayout();
			this.label1.Location = new Point(12, 23);
			this.label1.Name = "label1";
			this.label1.Size = new Size(89, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "人员";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.Location = new Point(12, 59);
			this.label2.Name = "label2";
			this.label2.Size = new Size(89, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "卡号";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_card.Location = new Point(107, 56);
			this.txt_card.Name = "txt_card";
			this.txt_card.Size = new Size(158, 21);
			this.txt_card.TabIndex = 1;
			this.txt_card.KeyPress += this.txt_card_KeyPress;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(103, 98);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 2;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(197, 98);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.cmb_user.FormattingEnabled = true;
			this.cmb_user.Location = new Point(107, 20);
			this.cmb_user.Name = "cmb_user";
			this.cmb_user.Size = new Size(158, 20);
			this.cmb_user.TabIndex = 0;
			this.cmb_user.Click += this.txt_user_Click;
			this.cmb_user.KeyPress += this.txt_user_KeyPress;
			this.label3.AutoSize = true;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(268, 60);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "*";
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(268, 24);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 75;
			this.label4.Text = "*";
			base.AcceptButton = this.btn_Ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(295, 132);
			base.Controls.Add(this.txt_card);
			base.Controls.Add(this.cmb_user);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddIssueForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "新增";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
