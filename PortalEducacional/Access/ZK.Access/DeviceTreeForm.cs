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
	public class DeviceTreeForm : Office2007Form
	{
		private bool m_ischecking = false;

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private bool m_isOnlyOne = false;

		public static bool is34BitsKeys = false;

		public static bool is26BitsKeys = false;

		public static bool showBitsKey = false;

		private string DeviceFilter;

		private List<int> m_SelectDoors = null;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private TreeView TView_device;

		public Label lb_info;

		private CheckBox checkBoxIsCard26Bits;

		private CheckBox checkBoxIsCard34Bits;

		public event EventHandler SelectDeviceEvent;

		public DeviceTreeForm()
		{
			this.InitializeComponent();
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		public DeviceTreeForm(bool isOnlyOne, string strDeviceFilter = "")
		{
			this.InitializeComponent();
			this.DeviceFilter = strDeviceFilter;
			this.BindData();
			initLang.LocaleForm(this, base.Name);
			this.m_isOnlyOne = isOnlyOne;
		}

		public DeviceTreeForm(List<int> selectDoors)
		{
			this.InitializeComponent();
			this.m_SelectDoors = selectDoors;
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			try
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> list = null;
				list = machinesBll.GetModelList(string.IsNullOrEmpty(this.DeviceFilter) ? "" : this.DeviceFilter);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (list != null && list.Count > 0)
				{
					TreeNode treeNode = new TreeNode();
					treeNode.Text = ShowMsgInfos.GetInfo("SAllSelect", "全选");
					treeNode.Tag = "";
					this.TView_device.Nodes.Add(treeNode);
					for (int i = 0; i < list.Count; i++)
					{
						TreeNode treeNode2 = new TreeNode();
						treeNode2.Text = list[i].MachineAlias;
						TreeNode treeNode3 = treeNode2;
						int num = list[i].ID;
						treeNode3.Tag = num.ToString();
						bool flag = false;
						if (modelList != null)
						{
							for (int j = 0; j < modelList.Count; j++)
							{
								if (i == 0 && !this.dlist.ContainsKey(modelList[j].id))
								{
									this.dlist.Add(modelList[j].id, modelList[j]);
								}
								if (list[i].ID == modelList[j].device_id)
								{
									bool flag2 = false;
									if (this.m_SelectDoors != null && this.m_SelectDoors.Count > 0)
									{
										int num2 = 0;
										while (num2 < this.m_SelectDoors.Count)
										{
											if (this.m_SelectDoors[num2] != modelList[j].id)
											{
												num2++;
												continue;
											}
											flag2 = true;
											break;
										}
									}
									if (!flag2)
									{
										TreeNode treeNode4 = new TreeNode();
										TreeNode treeNode5 = treeNode4;
										num = modelList[j].id;
										treeNode5.Tag = num.ToString();
										treeNode4.Text = modelList[j].door_name;
										treeNode2.Nodes.Add(treeNode4);
										flag = true;
									}
								}
							}
						}
						if (flag)
						{
							treeNode.Nodes.Add(treeNode2);
						}
					}
					if (treeNode.Nodes.Count == 0)
					{
						this.TView_device.Nodes.Remove(treeNode);
					}
				}
				if (this.TView_device.Nodes.Count > 0)
				{
					this.lb_info.Visible = false;
					this.TView_device.ExpandAll();
				}
				else
				{
					this.lb_info.Visible = true;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.checks26or34Bits();
			base.Close();
		}

		private void CheckUp(TreeNode node)
		{
			if (node.Parent != null && node.Checked && !node.Parent.Checked)
			{
				node.Parent.Checked = node.Checked;
				this.CheckUp(node.Parent);
			}
			else if (node.Parent != null && !node.Checked)
			{
				bool flag = false;
				for (int i = 0; i < node.Parent.Nodes.Count; i++)
				{
					if (node.Parent.Nodes[i].Checked)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					node.Parent.Checked = false;
					this.CheckUp(node.Parent);
				}
			}
		}

		private void CheckDown(TreeNode node)
		{
			if (node.Nodes.Count > 0)
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

		private void btn_OK_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.TView_device.Nodes.Count > 0)
				{
					List<AccDoor> list = new List<AccDoor>();
					int num = 0;
					for (int i = 0; i < this.TView_device.Nodes[0].Nodes.Count; i++)
					{
						if (this.TView_device.Nodes[0].Nodes[i].Checked && this.TView_device.Nodes[0].Nodes[i].Nodes.Count > 0)
						{
							num++;
							TreeNode treeNode = this.TView_device.Nodes[0].Nodes[i];
							for (int j = 0; j < treeNode.Nodes.Count; j++)
							{
								if (treeNode.Nodes[j].Checked && treeNode.Nodes[j].Tag != null)
								{
									int key = int.Parse(treeNode.Nodes[j].Tag.ToString());
									if (this.dlist.ContainsKey(key))
									{
										list.Add(this.dlist[key]);
									}
								}
							}
						}
					}
					if (this.m_isOnlyOne)
					{
						if (num > 1)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySeleteOneDevice", "只能选择一个设备！"));
						}
						else
						{
							if (this.SelectDeviceEvent != null)
							{
								this.SelectDeviceEvent(list, null);
								this.checks26or34Bits();
							}
							base.Close();
						}
					}
					else
					{
						if (this.SelectDeviceEvent != null)
						{
							this.SelectDeviceEvent(list, null);
							this.checks26or34Bits();
						}
						base.Close();
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public void checks26or34Bits()
		{
			DeviceTreeForm.is34BitsKeys = this.checkBoxIsCard34Bits.Checked;
			DeviceTreeForm.is26BitsKeys = this.checkBoxIsCard26Bits.Checked;
		}

		private void TView_device_AfterCheck(object sender, TreeViewEventArgs e)
		{
			try
			{
				this.btn_OK.Enabled = false;
				if (!this.m_ischecking)
				{
					this.m_ischecking = true;
					this.TView_device.AfterCheck -= this.TView_device_AfterCheck;
					if (e != null && e.Node != null)
					{
						this.CheckUp(e.Node);
						this.CheckDown(e.Node);
					}
					this.TView_device.AfterCheck += this.TView_device_AfterCheck;
					this.m_ischecking = false;
				}
				if (this.TView_device.Nodes[0].Nodes.Count > 0)
				{
					for (int i = 0; i < this.TView_device.Nodes[0].Nodes.Count; i++)
					{
						if (this.TView_device.Nodes[0].Nodes[i].Checked && this.TView_device.Nodes[0].Nodes[i].Nodes.Count > 0)
						{
							TreeNode treeNode = this.TView_device.Nodes[0].Nodes[i];
							int num = 0;
							while (num < treeNode.Nodes.Count)
							{
								if (!treeNode.Nodes[num].Checked || treeNode.Nodes[num].Tag == null)
								{
									num++;
									continue;
								}
								this.btn_OK.Enabled = true;
								break;
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void checkBoxIsCard26Bits_MouseCaptureChanged(object sender, EventArgs e)
		{
			this.checkBoxIsCard34Bits.Checked = false;
		}

		private void checkBoxIsCard34Bits_MouseCaptureChanged(object sender, EventArgs e)
		{
			this.checkBoxIsCard26Bits.Checked = false;
		}

		private void DeviceTreeForm_Load(object sender, EventArgs e)
		{
			this.checks26or34Bits();
		}

		private void DeviceTreeForm_Shown(object sender, EventArgs e)
		{
			this.checkBoxIsCard26Bits.Visible = DeviceTreeForm.showBitsKey;
			this.checkBoxIsCard34Bits.Visible = DeviceTreeForm.showBitsKey;
		}

		private void DeviceTreeForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			DeviceTreeForm.showBitsKey = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DeviceTreeForm));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.TView_device = new TreeView();
			this.lb_info = new Label();
			this.checkBoxIsCard26Bits = new CheckBox();
			this.checkBoxIsCard34Bits = new CheckBox();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(308, 338);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 2;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Enabled = false;
			this.btn_OK.Location = new Point(220, 338);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 25);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 1;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.TView_device.CheckBoxes = true;
			this.TView_device.ItemHeight = 18;
			this.TView_device.Location = new Point(6, 2);
			this.TView_device.Name = "TView_device";
			this.TView_device.Size = new Size(397, 262);
			this.TView_device.TabIndex = 5;
			this.TView_device.AfterCheck += this.TView_device_AfterCheck;
			this.lb_info.ForeColor = Color.Red;
			this.lb_info.Location = new Point(3, 322);
			this.lb_info.Name = "lb_info";
			this.lb_info.Size = new Size(387, 13);
			this.lb_info.TabIndex = 6;
			this.lb_info.Text = "没有可选设备";
			this.lb_info.Visible = false;
			this.checkBoxIsCard26Bits.AutoSize = true;
			this.checkBoxIsCard26Bits.Location = new Point(6, 270);
			this.checkBoxIsCard26Bits.Name = "checkBoxIsCard26Bits";
			this.checkBoxIsCard26Bits.Size = new Size(101, 17);
			this.checkBoxIsCard26Bits.TabIndex = 7;
			this.checkBoxIsCard26Bits.Text = "É cartão 26 Bits";
			this.checkBoxIsCard26Bits.UseVisualStyleBackColor = true;
			this.checkBoxIsCard26Bits.MouseCaptureChanged += this.checkBoxIsCard26Bits_MouseCaptureChanged;
			this.checkBoxIsCard34Bits.AutoSize = true;
			this.checkBoxIsCard34Bits.Location = new Point(6, 293);
			this.checkBoxIsCard34Bits.Name = "checkBoxIsCard34Bits";
			this.checkBoxIsCard34Bits.Size = new Size(101, 17);
			this.checkBoxIsCard34Bits.TabIndex = 8;
			this.checkBoxIsCard34Bits.Text = "É cartão 34 Bits";
			this.checkBoxIsCard34Bits.UseVisualStyleBackColor = true;
			this.checkBoxIsCard34Bits.MouseCaptureChanged += this.checkBoxIsCard34Bits_MouseCaptureChanged;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(402, 375);
			base.Controls.Add(this.checkBoxIsCard34Bits);
			base.Controls.Add(this.checkBoxIsCard26Bits);
			base.Controls.Add(this.lb_info);
			base.Controls.Add(this.TView_device);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DeviceTreeForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "选择门";
			base.FormClosing += this.DeviceTreeForm_FormClosing;
			base.Load += this.DeviceTreeForm_Load;
			base.Shown += this.DeviceTreeForm_Shown;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
