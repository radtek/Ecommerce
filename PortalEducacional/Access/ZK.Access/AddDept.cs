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

namespace ZK.Access
{
	public class AddDept : Office2007Form
	{
		private string deptCode = string.Empty;

		private string deptName = string.Empty;

		private int m_id = 0;

		private int m_parentID = 0;

		private List<Departments> m_deptlist = null;

		private IContainer components = null;

		private TextBox txt_code;

		private TextBox txt_dept;

		private ButtonX btn_Ok;

		private ButtonX btn_Cancel;

		private Label label3;

		private Label label4;

		private ComboBox cmb_parent;

		private Label label6;

		private Label label1;

		private Label label2;

		private Label label5;

		public event EventHandler refreshDataEvent = null;

		public AddDept(int id, int parentID)
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.m_id = id;
			this.m_parentID = parentID;
			this.BindData();
			this.loadDept();
		}

		private void loadDept()
		{
			try
			{
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				this.m_deptlist = departmentsBll.GetModelList("");
				if (this.m_deptlist != null && this.m_deptlist.Count > 0)
				{
					if (this.m_id > 0)
					{
						NodeManager nodeManager = new NodeManager();
						int num = 0;
						int num2;
						for (num = 0; num < this.m_deptlist.Count; num++)
						{
							NodeBase nodeBase = new NodeBase();
							NodeBase nodeBase2 = nodeBase;
							num2 = this.m_deptlist[num].DEPTID;
							nodeBase2.ID = num2.ToString();
							nodeBase.Name = nodeBase.ID;
							nodeBase.Tag = nodeBase.ID;
							NodeBase nodeBase3 = nodeBase;
							num2 = this.m_deptlist[num].SUPDEPTID;
							nodeBase3.ParentNodeID = num2.ToString();
							nodeManager.Datasouce.Add(nodeBase);
						}
						if (nodeManager.Bind())
						{
							INode node = null;
							if (nodeManager.NTree != null)
							{
								node = nodeManager.NTree.FindNode(this.m_id.ToString());
							}
							for (num = 0; num < this.m_deptlist.Count; num++)
							{
								if (this.m_deptlist[num].DEPTID != this.m_id && this.m_deptlist[num].SUPDEPTID != this.m_id)
								{
									int num3;
									if (node != null)
									{
										Tree nTree = nodeManager.NTree;
										INode node2 = node;
										num2 = this.m_deptlist[num].DEPTID;
										num3 = ((!nTree.Exists(node2, num2.ToString())) ? 1 : 0);
									}
									else
									{
										num3 = 1;
									}
									if (num3 != 0)
									{
										this.cmb_parent.Items.Add(this.m_deptlist[num].DEPTNAME);
										if (this.m_deptlist[num].DEPTID == this.m_parentID)
										{
											this.cmb_parent.SelectedIndex = this.cmb_parent.Items.Count - 1;
										}
									}
								}
							}
						}
					}
					else
					{
						for (int i = 0; i < this.m_deptlist.Count; i++)
						{
							this.cmb_parent.Items.Add(this.m_deptlist[i].DEPTNAME);
							if (this.m_deptlist[i].DEPTID == this.m_parentID)
							{
								this.cmb_parent.SelectedIndex = this.cmb_parent.Items.Count - 1;
							}
						}
					}
				}
				if (this.cmb_parent.Items.Count == 0)
				{
					this.cmb_parent.Items.Add("-----");
					this.cmb_parent.SelectedIndex = 0;
				}
				else if (this.cmb_parent.SelectedIndex < 0)
				{
					this.cmb_parent.SelectedIndex = 0;
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
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					Departments departments = null;
					DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
					departments = departmentsBll.GetModel(this.m_id);
					if (departments != null)
					{
						this.txt_dept.Text = departments.DEPTNAME;
						this.deptName = departments.DEPTNAME;
						this.txt_code.Text = departments.code;
						this.deptCode = departments.code;
						this.m_parentID = departments.SUPDEPTID;
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

		private bool check()
		{
			try
			{
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				if (string.IsNullOrEmpty(this.txt_code.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeptCodeNull", "部门编号不能为空!"));
					this.txt_code.Focus();
					return false;
				}
				if (string.IsNullOrEmpty(this.txt_dept.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeptNameNull", "部门名称不能为空!"));
					this.txt_dept.Focus();
					return false;
				}
				if (this.m_id > 0)
				{
					if (this.deptCode != this.txt_code.Text && departmentsBll.ExistsCode(this.txt_code.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeptCodeRepeat", "部门编号已经存在!"));
						this.txt_code.Focus();
						return false;
					}
					if (this.deptName != this.txt_dept.Text && departmentsBll.ExistsDeptName(this.txt_dept.Text.Trim()))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeptNameRepeat", "部门名称重复!"));
						this.txt_dept.Focus();
						return false;
					}
				}
				else
				{
					if (departmentsBll.ExistsCode(this.txt_code.Text.Trim()))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeptCodeRepeat", "部门编号已经存在!"));
						this.txt_code.Focus();
						return false;
					}
					if (this.deptName != this.txt_dept.Text && departmentsBll.ExistsDeptName(this.txt_dept.Text.Trim()))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeptNameRepeat", "部门名称重复!"));
						this.txt_dept.Focus();
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

		private void OkBtn_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.check())
				{
					Departments departments = null;
					DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
					if (this.m_id > 0)
					{
						departments = departmentsBll.GetModel(this.m_id);
					}
					if (departments == null)
					{
						departments = new Departments();
						departments.SUPDEPTID = this.m_parentID;
					}
					if (this.m_deptlist != null && this.m_deptlist.Count > 0)
					{
						int num = 0;
						while (num < this.m_deptlist.Count)
						{
							if (!(this.m_deptlist[num].DEPTNAME == this.cmb_parent.Text))
							{
								num++;
								continue;
							}
							departments.SUPDEPTID = this.m_deptlist[num].DEPTID;
							break;
						}
					}
					else
					{
						departments.SUPDEPTID = 0;
					}
					departments.DEPTNAME = this.txt_dept.Text;
					departments.code = this.txt_code.Text;
					if (departments.DEPTID > 0)
					{
						departmentsBll.Update(departments);
					}
					else
					{
						departmentsBll.Add(departments);
					}
					if (this.refreshDataEvent != null)
					{
						this.refreshDataEvent(this, null);
					}
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_code_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '.' || e.KeyChar == '\b')
			{
				e.Handled = false;
			}
			else if (e.KeyChar < '\0' || e.KeyChar > 'z')
			{
				e.Handled = true;
			}
			else
			{
				TextBox textBox = sender as TextBox;
				if (textBox.Text.Length >= 50)
				{
					e.Handled = true;
				}
			}
		}

		private void txt_dept_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddDept));
			this.txt_code = new TextBox();
			this.txt_dept = new TextBox();
			this.btn_Ok = new ButtonX();
			this.btn_Cancel = new ButtonX();
			this.label3 = new Label();
			this.label4 = new Label();
			this.cmb_parent = new ComboBox();
			this.label6 = new Label();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label5 = new Label();
			base.SuspendLayout();
			this.txt_code.Location = new Point(137, 53);
			this.txt_code.Name = "txt_code";
			this.txt_code.Size = new Size(157, 21);
			this.txt_code.TabIndex = 1;
			this.txt_code.KeyPress += this.txt_code_KeyPress;
			this.txt_dept.Location = new Point(137, 16);
			this.txt_dept.Name = "txt_dept";
			this.txt_dept.Size = new Size(157, 21);
			this.txt_dept.TabIndex = 0;
			this.txt_dept.KeyPress += this.txt_dept_KeyPress;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(132, 130);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 3;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.OkBtn_Click;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.Location = new Point(226, 130);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(82, 23);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 4;
			this.btn_Cancel.Text = "取消";
			this.btn_Cancel.Click += this.CancelBtn_Click;
			this.label3.AutoSize = true;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(297, 20);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "*";
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(297, 56);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 7;
			this.label4.Text = "*";
			this.cmb_parent.FormattingEnabled = true;
			this.cmb_parent.Location = new Point(137, 90);
			this.cmb_parent.Name = "cmb_parent";
			this.cmb_parent.Size = new Size(157, 20);
			this.cmb_parent.TabIndex = 2;
			this.label6.AutoSize = true;
			this.label6.ForeColor = Color.Red;
			this.label6.Location = new Point(297, 94);
			this.label6.Name = "label6";
			this.label6.Size = new Size(11, 12);
			this.label6.TabIndex = 11;
			this.label6.Text = "*";
			this.label1.Location = new Point(12, 56);
			this.label1.Name = "label1";
			this.label1.Size = new Size(112, 12);
			this.label1.TabIndex = 5;
			this.label1.Text = "部门编号";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.Location = new Point(12, 19);
			this.label2.Name = "label2";
			this.label2.Size = new Size(112, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "部门名称";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label5.Location = new Point(12, 93);
			this.label5.Name = "label5";
			this.label5.Size = new Size(112, 12);
			this.label5.TabIndex = 9;
			this.label5.Text = "上级部门";
			this.label5.TextAlign = ContentAlignment.MiddleLeft;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(319, 165);
			base.Controls.Add(this.cmb_parent);
			base.Controls.Add(this.txt_code);
			base.Controls.Add(this.txt_dept);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddDept";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "部门设置";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
