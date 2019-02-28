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
	public class UserDeptManager : Office2007Form
	{
		private int m_id = 0;

		private bool m_ischecking = false;

		private bool m_islock = false;

		private IContainer components = null;

		private PanelEx pn_title;

		private PanelEx pn_bottom;

		private ButtonX btn_Cancel;

		private ButtonX btn_Ok;

		private TreeView tv_dept;

		public UserDeptManager(int userID)
		{
			this.InitializeComponent();
			this.m_id = userID;
			try
			{
				this.BindData();
				this.CheckData();
				initLang.LocaleForm(this, base.Name);
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
				this.tv_dept.Nodes.Clear();
				List<Departments> list = null;
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				list = departmentsBll.GetModelList("");
				if (list != null)
				{
					NodeManager nodeManager = new NodeManager();
					for (int i = 0; i < list.Count; i++)
					{
						NodeBase nodeBase = new NodeBase();
						NodeBase nodeBase2 = nodeBase;
						int num = list[i].DEPTID;
						nodeBase2.ID = num.ToString();
						if (string.IsNullOrEmpty(list[i].code))
						{
							list[i].code = nodeBase.ID;
						}
						nodeBase.Name = list[i].code + "-" + list[i].DEPTNAME;
						NodeBase nodeBase3 = nodeBase;
						num = list[i].DEPTID;
						nodeBase3.Tag = num.ToString();
						NodeBase nodeBase4 = nodeBase;
						num = list[i].SUPDEPTID;
						nodeBase4.ParentNodeID = num.ToString();
						nodeManager.Datasouce.Add(nodeBase);
					}
					if (nodeManager.Bind())
					{
						nodeManager.ConvertToTree(this.tv_dept);
						this.tv_dept.ExpandAll();
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private TreeNode FindNode(TreeNode node, string id)
		{
			try
			{
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					if (node.Nodes[i].Tag != null && node.Nodes[i].Tag.ToString() == id)
					{
						return node.Nodes[i];
					}
					TreeNode treeNode = this.FindNode(node.Nodes[i], id);
					if (treeNode != null)
					{
						return treeNode;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			return null;
		}

		private TreeNode FindNode(string id)
		{
			try
			{
				for (int i = 0; i < this.tv_dept.Nodes.Count; i++)
				{
					if (this.tv_dept.Nodes[i].Tag != null && this.tv_dept.Nodes[i].Tag.ToString() == id)
					{
						return this.tv_dept.Nodes[i];
					}
					TreeNode treeNode = this.FindNode(this.tv_dept.Nodes[i], id);
					if (treeNode != null)
					{
						return treeNode;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			return null;
		}

		private void CheckData()
		{
			this.m_islock = true;
			try
			{
				if (this.tv_dept.Nodes.Count > 0 && this.m_id > 0)
				{
					DeptAdminBll deptAdminBll = new DeptAdminBll(MainForm._ia);
					List<DeptAdmin> modelList = deptAdminBll.GetModelList("user_id=" + this.m_id);
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							TreeNode treeNode = this.FindNode(modelList[i].dept_id.ToString());
							if (treeNode != null)
							{
								treeNode.Checked = true;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.m_islock = false;
		}

		private void CheckUp(TreeNode node)
		{
			if (node.Parent != null && node.Checked && !node.Parent.Checked)
			{
				node.Parent.Checked = node.Checked;
				this.CheckUp(node.Parent);
			}
		}

		private void CheckDown(TreeNode node)
		{
			if (!this.m_islock && node.Nodes.Count > 0)
			{
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					node.Nodes[i].Checked = node.Checked;
					if (node.Nodes[i].Nodes.Count > 0)
					{
						this.CheckDown(node.Nodes[i]);
					}
				}
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private List<int> GetNodes(TreeNode node)
		{
			List<int> list = new List<int>();
			try
			{
				if (node.Nodes.Count > 0)
				{
					for (int i = 0; i < node.Nodes.Count; i++)
					{
						if (node.Nodes[i].Checked && node.Nodes[i].Tag != null)
						{
							List<int> nodes = this.GetNodes(node.Nodes[i]);
							if (nodes.Count > 0)
							{
								list.AddRange(nodes);
							}
						}
					}
				}
				else if (node.Tag != null)
				{
					int item = int.Parse(node.Tag.ToString());
					list.Add(item);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			return list;
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tv_dept.Nodes.Count > 0 && this.m_id > 0)
				{
					List<int> list = new List<int>();
					for (int i = 0; i < this.tv_dept.Nodes.Count; i++)
					{
						if (this.tv_dept.Nodes[i].Checked)
						{
							if (this.tv_dept.Nodes[i].Tag != null)
							{
								list.Add(int.Parse(this.tv_dept.Nodes[i].Tag.ToString()));
							}
							if (this.tv_dept.Nodes[i].Nodes.Count > 0)
							{
								TreeNode node = this.tv_dept.Nodes[i];
								List<int> nodes = this.GetNodes(node);
								if (nodes.Count > 0)
								{
									list.AddRange(nodes);
								}
							}
						}
					}
					DeptAdminBll deptAdminBll = new DeptAdminBll(MainForm._ia);
					deptAdminBll.DeleteByUserID(this.m_id);
					if (list.Count > 0)
					{
						for (int j = 0; j < list.Count; j++)
						{
							DeptAdmin deptAdmin = new DeptAdmin();
							deptAdmin.dept_id = list[j];
							deptAdmin.user_id = this.m_id;
							deptAdminBll.Add(deptAdmin);
						}
					}
					base.Close();
				}
				else
				{
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void tv_dept_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (!this.m_ischecking)
			{
				this.m_ischecking = true;
				this.tv_dept.AfterCheck -= this.tv_dept_AfterCheck;
				if (e != null && e.Node != null)
				{
					this.CheckUp(e.Node);
					this.CheckDown(e.Node);
				}
				this.tv_dept.AfterCheck += this.tv_dept_AfterCheck;
				this.m_ischecking = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserDeptManager));
			this.pn_title = new PanelEx();
			this.pn_bottom = new PanelEx();
			this.btn_Cancel = new ButtonX();
			this.btn_Ok = new ButtonX();
			this.tv_dept = new TreeView();
			this.pn_bottom.SuspendLayout();
			base.SuspendLayout();
			this.pn_title.CanvasColor = SystemColors.Control;
			this.pn_title.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pn_title.Dock = DockStyle.Top;
			this.pn_title.Location = new Point(0, 0);
			this.pn_title.Name = "pn_title";
			this.pn_title.Size = new Size(571, 47);
			this.pn_title.Style.Alignment = StringAlignment.Center;
			this.pn_title.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pn_title.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pn_title.Style.Border = eBorderType.SingleLine;
			this.pn_title.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pn_title.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pn_title.Style.GradientAngle = 90;
			this.pn_title.TabIndex = 0;
			this.pn_title.Text = "选择用户控制的部门，不选默认为全部";
			this.pn_bottom.CanvasColor = SystemColors.Control;
			this.pn_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pn_bottom.Controls.Add(this.btn_Cancel);
			this.pn_bottom.Controls.Add(this.btn_Ok);
			this.pn_bottom.Dock = DockStyle.Bottom;
			this.pn_bottom.Location = new Point(0, 319);
			this.pn_bottom.Name = "pn_bottom";
			this.pn_bottom.Size = new Size(571, 48);
			this.pn_bottom.Style.Alignment = StringAlignment.Center;
			this.pn_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pn_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pn_bottom.Style.Border = eBorderType.SingleLine;
			this.pn_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pn_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pn_bottom.Style.GradientAngle = 90;
			this.pn_bottom.TabIndex = 2;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.Location = new Point(477, 13);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(82, 23);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 1;
			this.btn_Cancel.Text = "取消";
			this.btn_Cancel.Click += this.btn_Cancel_Click;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(375, 13);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 0;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.tv_dept.CheckBoxes = true;
			this.tv_dept.Dock = DockStyle.Fill;
			this.tv_dept.ItemHeight = 18;
			this.tv_dept.Location = new Point(0, 47);
			this.tv_dept.Name = "tv_dept";
			this.tv_dept.Size = new Size(571, 272);
			this.tv_dept.TabIndex = 5;
			this.tv_dept.TabStop = false;
			this.tv_dept.AfterCheck += this.tv_dept_AfterCheck;
			base.AcceptButton = this.btn_Ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(571, 367);
			base.Controls.Add(this.tv_dept);
			base.Controls.Add(this.pn_bottom);
			base.Controls.Add(this.pn_title);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UserDeptManager";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "用户控制部门";
			this.pn_bottom.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
