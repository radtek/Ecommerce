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
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class SettingDepartmentForm : Office2007Form
	{
		private List<int> deptIdNO = new List<int>();

		private DepartmentsBll bll = new DepartmentsBll(MainForm._ia);

		private IContainer components = null;

		private ButtonX ok_Btn;

		public TreeView tview_department;

		private ButtonX btn_cancel;

		private ButtonX btn_edit;

		private ButtonX btn_add;

		private ButtonX btn_delete;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_deit;

		private ToolStripMenuItem Menu_delete;

		public event EventHandler RefreshDataEvent = null;

		public SettingDepartmentForm()
		{
			this.InitializeComponent();
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			try
			{
				this.tview_department.Nodes.Clear();
				List<Departments> list = null;
				list = this.bll.GetModelList("");
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
						nodeManager.ConvertToTree(this.tview_department);
						this.tview_department.ExpandAll();
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void cancelbtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void addDept_RefreshDataEvent(object sender, EventArgs e)
		{
			this.BindData();
			if (this.RefreshDataEvent != null)
			{
				this.RefreshDataEvent(this, null);
			}
		}

		private bool Excit(object id)
		{
			if (id != null)
			{
				this.deptIdNO.Add(int.Parse(id.ToString()));
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				if (userInfoBll.ExistsDept(int.Parse(id.ToString())))
				{
					this.deptIdNO.Clear();
					return true;
				}
			}
			return false;
		}

		private bool IsHaveUser(TreeNode node)
		{
			if (this.Excit(node.Tag))
			{
				return true;
			}
			if (node.Nodes.Count > 0)
			{
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					if (this.IsHaveUser(node.Nodes[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			if (this.tview_department.SelectedNode != null && this.tview_department.SelectedNode.Tag != null)
			{
				string s = this.tview_department.SelectedNode.Tag.ToString();
				AddDept addDept = new AddDept(0, int.Parse(s));
				addDept.refreshDataEvent += this.addDept_RefreshDataEvent;
				addDept.ShowDialog();
				addDept.refreshDataEvent -= this.addDept_RefreshDataEvent;
			}
			else
			{
				AddDept addDept2 = new AddDept(0, 0);
				addDept2.refreshDataEvent += this.addDept_RefreshDataEvent;
				addDept2.ShowDialog();
				addDept2.refreshDataEvent -= this.addDept_RefreshDataEvent;
			}
			this.SetFoucs();
		}

		private void SetFoucs()
		{
			if (this.tview_department.SelectedNode == null)
			{
				if (this.tview_department.Nodes != null && this.tview_department.Nodes.Count > 0)
				{
					this.tview_department.SelectedNode = this.tview_department.Nodes[0];
					this.tview_department.Nodes[0].Checked = true;
					this.tview_department.Focus();
				}
			}
			else
			{
				this.tview_department.Focus();
			}
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			if (this.tview_department.SelectedNode == null)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("ChooseEditDept", "请选择需要编辑的部门!"));
			}
			else if (this.tview_department.SelectedNode.Tag != null)
			{
				string s = this.tview_department.SelectedNode.Tag.ToString();
				AddDept addDept = new AddDept(int.Parse(s), 0);
				addDept.refreshDataEvent += this.addDept_RefreshDataEvent;
				addDept.ShowDialog();
				addDept.refreshDataEvent -= this.addDept_RefreshDataEvent;
			}
			else
			{
				AddDept addDept2 = new AddDept(0, 0);
				addDept2.refreshDataEvent += this.addDept_RefreshDataEvent;
				addDept2.ShowDialog();
				addDept2.refreshDataEvent -= this.addDept_RefreshDataEvent;
			}
			this.SetFoucs();
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tview_department.Nodes.Count > 0)
				{
					if (this.tview_department.SelectedNode == null)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("ChooseDeleteDept", "请选择需要删除的部门!"));
					}
					else if (this.tview_department.SelectedNode == this.tview_department.Nodes[0])
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("rootNOdelete", "根部门不能删除!"));
					}
					else if (this.IsHaveUser(this.tview_department.SelectedNode))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("presencePersonnel", "该部门还有人员，不能删除!"));
					}
					else if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的对象?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						foreach (int item in this.deptIdNO)
						{
							if (this.bll.Delete(item))
							{
								TreeNode treeNode = this.FindTreeNodeByTag(item.ToString(), this.tview_department.Nodes);
								if (treeNode != null)
								{
									this.tview_department.Nodes.Remove(treeNode);
								}
							}
						}
					}
				}
				if (this.RefreshDataEvent != null)
				{
					this.RefreshDataEvent(this, null);
					if (this.tview_department.Nodes.Count > 0)
					{
						this.tview_department.SelectedNode = this.tview_department.Nodes[0];
						this.tview_department.Nodes[0].Checked = true;
					}
				}
				this.SetFoucs();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private TreeNode FindTreeNodeByTag(string tag, TreeNodeCollection NodeCollection)
		{
			TreeNode treeNode = null;
			foreach (TreeNode item in NodeCollection)
			{
				if (item.Tag != null)
				{
					if (item.Tag.ToString() == tag)
					{
						treeNode = item;
						break;
					}
					treeNode = this.FindTreeNodeByTag(tag, item.Nodes);
					if (treeNode != null)
					{
						break;
					}
				}
			}
			return treeNode;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SettingDepartmentForm));
			this.ok_Btn = new ButtonX();
			this.tview_department = new TreeView();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_deit = new ToolStripMenuItem();
			this.Menu_delete = new ToolStripMenuItem();
			this.btn_cancel = new ButtonX();
			this.btn_edit = new ButtonX();
			this.btn_add = new ButtonX();
			this.btn_delete = new ButtonX();
			this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.ok_Btn.AccessibleRole = AccessibleRole.PushButton;
			this.ok_Btn.Location = new Point(0, 0);
			this.ok_Btn.Name = "ok_Btn";
			this.ok_Btn.Size = new Size(0, 0);
			this.ok_Btn.TabIndex = 28;
			this.tview_department.ContextMenuStrip = this.contextMenuStrip1;
			this.tview_department.FullRowSelect = true;
			this.tview_department.HideSelection = false;
			this.tview_department.Location = new Point(0, 1);
			this.tview_department.Name = "tview_department";
			this.tview_department.Size = new Size(415, 253);
			this.tview_department.TabIndex = 5;
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[3]
			{
				this.Menu_add,
				this.Menu_deit,
				this.Menu_delete
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(153, 92);
			this.Menu_add.Image = Resources.add;
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(152, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.btn_add_Click;
			this.Menu_deit.Image = Resources.edit;
			this.Menu_deit.Name = "Menu_deit";
			this.Menu_deit.Size = new Size(152, 22);
			this.Menu_deit.Text = "编辑";
			this.Menu_deit.Click += this.btn_edit_Click;
			this.Menu_delete.Image = Resources.delete;
			this.Menu_delete.Name = "Menu_delete";
			this.Menu_delete.Size = new Size(152, 22);
			this.Menu_delete.Text = "删除";
			this.Menu_delete.Click += this.btn_delete_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(321, 266);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_edit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_edit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_edit.Location = new Point(133, 266);
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(82, 23);
			this.btn_edit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_edit.TabIndex = 1;
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_add.AccessibleRole = AccessibleRole.PushButton;
			this.btn_add.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_add.Location = new Point(39, 266);
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(82, 23);
			this.btn_add.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_add.TabIndex = 0;
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_delete.AccessibleRole = AccessibleRole.PushButton;
			this.btn_delete.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_delete.Location = new Point(227, 266);
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(82, 23);
			this.btn_delete.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_delete.TabIndex = 2;
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			base.AcceptButton = this.btn_cancel;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(415, 301);
			base.Controls.Add(this.tview_department);
			base.Controls.Add(this.ok_Btn);
			base.Controls.Add(this.btn_edit);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_add);
			base.Controls.Add(this.btn_delete);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SettingDepartmentForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "树型显示";
			this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
