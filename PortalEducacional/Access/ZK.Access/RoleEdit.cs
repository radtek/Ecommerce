/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class RoleEdit : Office2007Form
	{
		private int m_id = 0;

		private AuthGroupBll bll = new AuthGroupBll(MainForm._ia);

		private List<string> m_permissions = new List<string>();

		private IContainer components = null;

		private Label lb_name;

		private TextBox txt_name;

		private ButtonX btn_Cancel;

		private ButtonX btn_Ok;

		private Label lb_remark;

		private TextBox txt_remark;

		private PanelEx panelEx1;

		private DataGridViewX gv_role;

		private Label lbl_star1;

		private CheckBox ch_control;

		private CheckBox ch_show;

		private DataGridViewTextBoxColumn column_name;

		private DataGridViewCheckBoxColumn Column_Read;

		private DataGridViewCheckBoxColumn Column_Control;

		public event EventHandler RefreshDataEvent = null;

		public RoleEdit(int id)
		{
			this.InitializeComponent();
			try
			{
				this.gv_role.Controls.Add(this.ch_control);
				this.gv_role.Controls.Add(this.ch_show);
				this.m_id = id;
				this.LoadPermission();
				this.BindData();
				initLang.LocaleForm(this, base.Name);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadPermission()
		{
			this.m_permissions.Clear();
			this.m_permissions.AddRange(SysInfos.PermissionList);
		}

		public void LoadPermissionTable(string data)
		{
			try
			{
				this.gv_role.Rows.Clear();
				for (int i = 0; i < this.m_permissions.Count; i++)
				{
					int index = this.gv_role.Rows.Add();
					this.gv_role.Rows[index].Cells[0].Value = this.m_permissions[i];
					if (!string.IsNullOrEmpty(data))
					{
						if (data.Length > i)
						{
							string a = data.Substring(i, 1);
							if (a == "0")
							{
								this.gv_role.Rows[index].Cells[1].Value = false;
								this.gv_role.Rows[index].Cells[2].Value = false;
							}
							else if (a == "1")
							{
								this.gv_role.Rows[index].Cells[1].Value = true;
								this.gv_role.Rows[index].Cells[2].Value = false;
							}
							else if (a == "2")
							{
								this.gv_role.Rows[index].Cells[1].Value = false;
								this.gv_role.Rows[index].Cells[2].Value = true;
							}
							else
							{
								this.gv_role.Rows[index].Cells[1].Value = true;
								this.gv_role.Rows[index].Cells[2].Value = true;
							}
						}
						else
						{
							this.gv_role.Rows[index].Cells[1].Value = false;
							this.gv_role.Rows[index].Cells[2].Value = false;
						}
					}
					else
					{
						this.gv_role.Rows[index].Cells[1].Value = false;
						this.gv_role.Rows[index].Cells[2].Value = false;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					AuthGroup authGroup = null;
					authGroup = this.bll.GetModel(this.m_id);
					if (authGroup != null)
					{
						this.txt_name.Text = authGroup.name;
						this.txt_remark.Text = authGroup.Remark;
						if (!string.IsNullOrEmpty(authGroup.Permission))
						{
							this.LoadPermissionTable(authGroup.Permission);
						}
						else
						{
							this.LoadPermissionTable("000000000000000000000000000000000000000000000000000000000000000000000000000000");
						}
					}
					else
					{
						this.LoadPermissionTable("");
					}
				}
				else
				{
					this.LoadPermissionTable("");
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool check()
		{
			if (!string.IsNullOrEmpty(this.txt_name.Text.Trim()))
			{
				return true;
			}
			SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
			this.txt_name.Focus();
			return false;
		}

		private bool BindModel(AuthGroup group)
		{
			if (this.check())
			{
				try
				{
					group.name = this.txt_name.Text;
					group.Remark = this.txt_remark.Text;
					group.Permission = "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
					for (int i = 0; i < this.gv_role.Rows.Count && this.gv_role.Rows[i].Cells.Count >= 3; i++)
					{
						string a = this.gv_role.Rows[i].Cells[1].Value.ToString().ToLower();
						string a2 = this.gv_role.Rows[i].Cells[2].Value.ToString().ToLower();
						int num = 0;
						if (a == "true")
						{
							num++;
						}
						if (a2 == "true")
						{
							num += 2;
						}
						a2 = this.gv_role.Rows[i].Cells[0].Value.ToString().ToLower();
						int num2 = 0;
						while (num2 < this.m_permissions.Count)
						{
							if (!(this.m_permissions[num2].ToLower() == a2))
							{
								num2++;
								continue;
							}
							if (group.Permission.Length > num2)
							{
								group.Permission = group.Permission.Remove(num2, 1);
								group.Permission = group.Permission.Insert(num2, num.ToString());
							}
							else
							{
								while (group.Permission.Length < num2 + 1)
								{
									group.Permission += "00000";
								}
								group.Permission = group.Permission.Remove(num2, 1);
								group.Permission = group.Permission.Insert(num2, num.ToString());
							}
							break;
						}
					}
					return true;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		private bool Save()
		{
			try
			{
				AuthGroup authGroup = null;
				if (this.m_id > 0)
				{
					authGroup = this.bll.GetModel(this.m_id);
				}
				if (authGroup == null)
				{
					if (this.bll.Exists(this.txt_name.Text.Trim()))
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SRoleNameRepeat", "角色名称已经存在"));
						return false;
					}
					authGroup = new AuthGroup();
				}
				if (this.BindModel(authGroup))
				{
					if (this.m_id > 0)
					{
						if (this.bll.Update(authGroup))
						{
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							return true;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
					else
					{
						try
						{
							this.bll.Add(authGroup);
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							return true;
						}
						catch
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						}
					}
				}
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
			}
			return false;
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_all_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.gv_role.Rows.Count; i++)
			{
				if (this.gv_role.Rows[i].Cells.Count >= 3)
				{
					this.gv_role.Rows[i].Cells[1].Value = true;
					this.gv_role.Rows[i].Cells[2].Value = true;
				}
			}
		}

		private void btn_cancelAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.gv_role.Rows.Count; i++)
			{
				if (this.gv_role.Rows[i].Cells.Count >= 3)
				{
					this.gv_role.Rows[i].Cells[1].Value = false;
					this.gv_role.Rows[i].Cells[2].Value = false;
				}
			}
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_remark_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 25);
		}

		private void ch_show_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < this.gv_role.Rows.Count; i++)
			{
				if (this.gv_role.Rows[i].Cells.Count >= 3)
				{
					this.gv_role.Rows[i].Cells[1].Value = this.ch_show.Checked;
				}
			}
		}

		private void ch_control_CheckedChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < this.gv_role.Rows.Count; i++)
			{
				if (this.gv_role.Rows[i].Cells.Count >= 3)
				{
					this.gv_role.Rows[i].Cells[2].Value = this.ch_control.Checked;
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
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(RoleEdit));
			this.lb_name = new Label();
			this.txt_name = new TextBox();
			this.btn_Cancel = new ButtonX();
			this.btn_Ok = new ButtonX();
			this.lb_remark = new Label();
			this.txt_remark = new TextBox();
			this.panelEx1 = new PanelEx();
			this.ch_control = new CheckBox();
			this.ch_show = new CheckBox();
			this.gv_role = new DataGridViewX();
			this.column_name = new DataGridViewTextBoxColumn();
			this.Column_Read = new DataGridViewCheckBoxColumn();
			this.Column_Control = new DataGridViewCheckBoxColumn();
			this.lbl_star1 = new Label();
			this.panelEx1.SuspendLayout();
			((ISupportInitialize)this.gv_role).BeginInit();
			base.SuspendLayout();
			this.lb_name.Location = new Point(10, 17);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(105, 12);
			this.lb_name.TabIndex = 1;
			this.lb_name.Text = "角色名称";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_name.Location = new Point(121, 13);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(162, 21);
			this.txt_name.TabIndex = 0;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.Location = new Point(556, 373);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(82, 23);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 5;
			this.btn_Cancel.Text = "取消";
			this.btn_Cancel.Click += this.btn_Cancel_Click;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(462, 373);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 4;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.lb_remark.Location = new Point(374, 17);
			this.lb_remark.Name = "lb_remark";
			this.lb_remark.Size = new Size(96, 12);
			this.lb_remark.TabIndex = 26;
			this.lb_remark.Text = "备注";
			this.lb_remark.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_remark.Location = new Point(476, 13);
			this.txt_remark.Name = "txt_remark";
			this.txt_remark.Size = new Size(162, 21);
			this.txt_remark.TabIndex = 1;
			this.txt_remark.KeyPress += this.txt_remark_KeyPress;
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Controls.Add(this.ch_control);
			this.panelEx1.Controls.Add(this.ch_show);
			this.panelEx1.Controls.Add(this.gv_role);
			this.panelEx1.Location = new Point(12, 44);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(626, 315);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 27;
			this.ch_control.AutoSize = true;
			this.ch_control.Location = new Point(603, 3);
			this.ch_control.Name = "ch_control";
			this.ch_control.Size = new Size(15, 14);
			this.ch_control.TabIndex = 3;
			this.ch_control.UseVisualStyleBackColor = true;
			this.ch_control.CheckedChanged += this.ch_control_CheckedChanged;
			this.ch_show.AutoSize = true;
			this.ch_show.Location = new Point(449, 3);
			this.ch_show.Name = "ch_show";
			this.ch_show.Size = new Size(15, 14);
			this.ch_show.TabIndex = 2;
			this.ch_show.UseVisualStyleBackColor = true;
			this.ch_show.CheckedChanged += this.ch_show_CheckedChanged;
			this.gv_role.AllowUserToAddRows = false;
			this.gv_role.AllowUserToDeleteRows = false;
			this.gv_role.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gv_role.Columns.AddRange(this.column_name, this.Column_Read, this.Column_Control);
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("SimSun", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.gv_role.DefaultCellStyle = dataGridViewCellStyle;
			this.gv_role.Dock = DockStyle.Fill;
			this.gv_role.GridColor = Color.FromArgb(208, 215, 229);
			this.gv_role.Location = new Point(0, 0);
			this.gv_role.Name = "gv_role";
			this.gv_role.RowTemplate.Height = 23;
			this.gv_role.Size = new Size(626, 315);
			this.gv_role.TabIndex = 1;
			this.gv_role.TabStop = false;
			this.column_name.FillWeight = 250f;
			this.column_name.HeaderText = "权限名称";
			this.column_name.Name = "column_name";
			this.column_name.Width = 250;
			this.Column_Read.HeaderText = "浏览权限";
			this.Column_Read.Name = "Column_Read";
			this.Column_Read.Resizable = DataGridViewTriState.True;
			this.Column_Read.SortMode = DataGridViewColumnSortMode.Automatic;
			this.Column_Read.Width = 180;
			this.Column_Control.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.Column_Control.HeaderText = "控制权限";
			this.Column_Control.Name = "Column_Control";
			this.Column_Control.Resizable = DataGridViewTriState.True;
			this.Column_Control.SortMode = DataGridViewColumnSortMode.Automatic;
			this.lbl_star1.AutoSize = true;
			this.lbl_star1.ForeColor = Color.Red;
			this.lbl_star1.Location = new Point(286, 17);
			this.lbl_star1.Name = "lbl_star1";
			this.lbl_star1.Size = new Size(11, 12);
			this.lbl_star1.TabIndex = 36;
			this.lbl_star1.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(650, 408);
			base.Controls.Add(this.txt_remark);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.lbl_star1);
			base.Controls.Add(this.panelEx1);
			base.Controls.Add(this.lb_remark);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.lb_name);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "RoleEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "角色设置";
			this.panelEx1.ResumeLayout(false);
			this.panelEx1.PerformLayout();
			((ISupportInitialize)this.gv_role).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
