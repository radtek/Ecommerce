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
	public class AreaTreeForm : Office2007Form
	{
		private List<int> areaIDList = new List<int>();

		private Dictionary<int, PersonnelArea> dicAreaModel;

		private PersonnelAreaBll bll = new PersonnelAreaBll(MainForm._ia);

		private IContainer components = null;

		public TreeView tview_deviceArea;

		private ButtonX btn_add;

		private ButtonX btn_edit;

		private ButtonX btn_delete;

		private ButtonX btn_cancel;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_delete;

		public event EventHandler refreshDataEvent = null;

		public AreaTreeForm()
		{
			this.InitializeComponent();
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			try
			{
				this.tview_deviceArea.Nodes.Clear();
				List<PersonnelArea> list = null;
				list = this.bll.GetModelList("");
				if (list != null)
				{
					this.dicAreaModel = new Dictionary<int, PersonnelArea>();
					NodeManager nodeManager = new NodeManager();
					for (int i = 0; i < list.Count; i++)
					{
						NodeBase nodeBase = new NodeBase();
						NodeBase nodeBase2 = nodeBase;
						int num = list[i].id;
						nodeBase2.ID = num.ToString();
						if (string.IsNullOrEmpty(list[i].areaid))
						{
							list[i].areaid = nodeBase.ID;
						}
						nodeBase.Name = list[i].areaid + "-" + list[i].areaname;
						nodeBase.Tag = list[i].id;
						NodeBase nodeBase3 = nodeBase;
						num = list[i].parent_id;
						nodeBase3.ParentNodeID = num.ToString();
						nodeManager.Datasouce.Add(nodeBase);
						this.dicAreaModel.Add(list[i].id, list[i]);
					}
					if (nodeManager.Bind())
					{
						nodeManager.ConvertToTree(this.tview_deviceArea);
						this.tview_deviceArea.ExpandAll();
					}
				}
				if (this.tview_deviceArea.Nodes != null && this.tview_deviceArea.Nodes.Count > 0)
				{
					this.tview_deviceArea.SelectedNode = this.tview_deviceArea.Nodes[0];
					this.tview_deviceArea.Nodes[0].Checked = true;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool IsHaveMachines(TreeNode node)
		{
			if (node == null)
			{
				return false;
			}
			if (this.Exist(node.Tag))
			{
				return true;
			}
			if (node.Nodes.Count > 0)
			{
				for (int i = 0; i < node.Nodes.Count; i++)
				{
					if (this.IsHaveMachines(node.Nodes[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private bool Exist(object id)
		{
			try
			{
				if (id != null)
				{
					this.areaIDList.Add(int.Parse(id.ToString()));
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					if (machinesBll.ExistsArea(id.ToString()))
					{
						this.areaIDList.Clear();
						return true;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void cancelbtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void AddBtn_Click(object sender, EventArgs e)
		{
		}

		private void addArea_RefreshDataEvent(object sender, EventArgs e)
		{
			this.BindData();
			if (this.refreshDataEvent != null)
			{
				this.refreshDataEvent(this, null);
			}
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tview_deviceArea.SelectedNode == null)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectArea", "请选择需要删除的区域!"));
				}
				else if (this.tview_deviceArea.SelectedNode.Tag != null)
				{
					string s = this.tview_deviceArea.SelectedNode.Tag.ToString();
					AddAreaForm addAreaForm = new AddAreaForm(int.Parse(s), 0);
					addAreaForm.RefreshDataEvent += this.addArea_RefreshDataEvent;
					addAreaForm.ShowDialog();
					addAreaForm.RefreshDataEvent -= this.addArea_RefreshDataEvent;
				}
				else
				{
					AddAreaForm addAreaForm2 = new AddAreaForm(0, 0);
					addAreaForm2.RefreshDataEvent += this.addArea_RefreshDataEvent;
					addAreaForm2.ShowDialog();
					addAreaForm2.RefreshDataEvent -= this.addArea_RefreshDataEvent;
				}
				this.SetFocus();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetFocus()
		{
			if (this.tview_deviceArea.SelectedNode == null)
			{
				if (this.tview_deviceArea.Nodes != null && this.tview_deviceArea.Nodes.Count > 0)
				{
					this.tview_deviceArea.SelectedNode = this.tview_deviceArea.Nodes[0];
					this.tview_deviceArea.Nodes[0].Checked = true;
					this.tview_deviceArea.Focus();
				}
			}
			else
			{
				this.tview_deviceArea.Focus();
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tview_deviceArea.SelectedNode == null)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectArea", "请选择需要删除的区域!"));
				}
				else
				{
					if (this.tview_deviceArea.SelectedNode == this.tview_deviceArea.Nodes[0])
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotDelRootArea", "根区域不能删除"));
						goto end_IL_0001;
					}
					if (this.tview_deviceArea.SelectedNode.Nodes.Count > 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDelLowerArea", "请先删除该区域的下级区域!"));
					}
					else if (this.tview_deviceArea.SelectedNode.Tag != null)
					{
						if (this.IsHaveMachines(this.tview_deviceArea.SelectedNode))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAreaHasDevice", "该区域还有设备，不能删除!"));
							goto end_IL_0001;
						}
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "是否删除该数据"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							foreach (int areaID in this.areaIDList)
							{
								this.bll.DeleteByAreaId(this.dicAreaModel[areaID].areaid);
							}
							this.tview_deviceArea.Nodes.Remove(this.tview_deviceArea.SelectedNode);
						}
					}
				}
				if (this.refreshDataEvent != null)
				{
					this.refreshDataEvent(this, null);
				}
				this.SetFocus();
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tview_deviceArea.SelectedNode == null)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectSupArea", "请选择上级区域!"));
				}
				else if (this.tview_deviceArea.SelectedNode.Tag != null)
				{
					string s = this.tview_deviceArea.SelectedNode.Tag.ToString();
					AddAreaForm addAreaForm = new AddAreaForm(0, int.Parse(s));
					addAreaForm.RefreshDataEvent += this.addArea_RefreshDataEvent;
					addAreaForm.ShowDialog();
					addAreaForm.RefreshDataEvent -= this.addArea_RefreshDataEvent;
					addAreaForm.Close();
				}
				else
				{
					AddAreaForm addAreaForm2 = new AddAreaForm(0, 0);
					addAreaForm2.RefreshDataEvent += this.addArea_RefreshDataEvent;
					addAreaForm2.ShowDialog();
					addAreaForm2.RefreshDataEvent -= this.addArea_RefreshDataEvent;
					addAreaForm2.Close();
				}
				this.SetFocus();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AreaTreeForm));
			this.tview_deviceArea = new TreeView();
			this.btn_add = new ButtonX();
			this.btn_edit = new ButtonX();
			this.btn_delete = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_delete = new ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.tview_deviceArea.FullRowSelect = true;
			this.tview_deviceArea.HideSelection = false;
			this.tview_deviceArea.Location = new Point(1, 3);
			this.tview_deviceArea.Name = "tview_deviceArea";
			this.tview_deviceArea.Size = new Size(406, 290);
			this.tview_deviceArea.TabIndex = 28;
			this.btn_add.AccessibleRole = AccessibleRole.PushButton;
			this.btn_add.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_add.Location = new Point(8, 299);
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(82, 23);
			this.btn_add.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_add.TabIndex = 29;
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_edit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_edit.Location = new Point(110, 299);
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(82, 23);
			this.btn_edit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_edit.TabIndex = 30;
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.AccessibleRole = AccessibleRole.PushButton;
			this.btn_delete.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_delete.Location = new Point(212, 299);
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(82, 23);
			this.btn_delete.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_delete.TabIndex = 31;
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(314, 299);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 32;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[3]
			{
				this.menu_add,
				this.menu_edit,
				this.menu_delete
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(95, 70);
			this.menu_add.Image = Resources.add;
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(94, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.btn_add_Click;
			this.menu_edit.Image = Resources.edit;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(94, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_delete.Image = Resources.delete;
			this.menu_delete.Name = "menu_delete";
			this.menu_delete.Size = new Size(94, 22);
			this.menu_delete.Text = "删除";
			this.menu_delete.Click += this.btn_delete_Click;
			base.AcceptButton = this.btn_cancel;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(408, 329);
			this.ContextMenuStrip = this.contextMenuStrip1;
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_delete);
			base.Controls.Add(this.btn_edit);
			base.Controls.Add(this.btn_add);
			base.Controls.Add(this.tview_deviceArea);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AreaTreeForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "区域设置";
			this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
