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
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class InitDatabaseForm : Office2007Form
	{
		private bool m_ischecking = false;

		private IContainer components = null;

		private TreeView TView_Options;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		public event EventHandler SelectDeviceEvent;

		public InitDatabaseForm()
		{
			this.InitializeComponent();
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			this.TView_Options.Nodes.Clear();
			this.TView_Options.Nodes.Add(ShowMsgInfos.GetInfo("SAllInit", "全部"));
			this.TView_Options.Nodes[0].Nodes.Add(ShowMsgInfos.GetInfo("SInitPerson", "人事"));
			this.TView_Options.Nodes[0].Nodes.Add(ShowMsgInfos.GetInfo("SInitReport", "报表"));
			this.TView_Options.Nodes[0].Nodes.Add(ShowMsgInfos.GetInfo("SInitDevice", "设备"));
			this.TView_Options.Nodes[0].Nodes.Add(ShowMsgInfos.GetInfo("SInitAccess", "门禁"));
			this.TView_Options.Nodes[0].Nodes.Add(ShowMsgInfos.GetInfo("SInitSystem", "系统"));
			this.TView_Options.ExpandAll();
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

		private void TView_device_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (!this.m_ischecking)
			{
				this.m_ischecking = true;
				this.TView_Options.AfterCheck -= this.TView_device_AfterCheck;
				if (e != null && e.Node != null)
				{
					this.CheckUp(e.Node);
					this.CheckDown(e.Node);
				}
				this.TView_Options.AfterCheck += this.TView_device_AfterCheck;
				this.m_ischecking = false;
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			List<TreeNode> checkedNodes = this.GetCheckedNodes(this.TView_Options.Nodes[0].Nodes);
			if (checkedNodes.Count <= 0)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("NoChoiceChecked", "请选择需要初始化的数据项"));
			}
			else if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SSystemInit", "初始化系统时, 将清除所选表的数据, 要继续吗?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				try
				{
					for (int i = 0; i < checkedNodes.Count; i++)
					{
						if (checkedNodes[i].Text == ShowMsgInfos.GetInfo("SInitPerson", "人事") && checkedNodes[i].Checked)
						{
							AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
							accMorecardempGroupBll.InitTable();
							AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
							accMorecardGroupBll.InitTable();
							AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
							accMorecardsetBll.InitTable();
							AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
							accLevelsetBll.InitTable();
							AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
							accLevelsetDoorGroupBll.InitTable();
							AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
							accLevelsetEmpBll.InitTable();
							AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
							accFirstOpenBll.InitTable();
							AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
							accFirstOpenEmpBll.InitTable();
							PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
							personnelIssuecardBll.InitTable();
							UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
							userInfoBll.InitTable();
							TemplateBll templateBll = new TemplateBll(MainForm._ia);
							templateBll.InitTable();
							FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
							fingerVeinBll.InitTable();
							DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
							departmentsBll.InitTable();
							FaceTempBll faceTempBll = new FaceTempBll(MainForm._ia);
							faceTempBll.InitTable();
						}
						if (checkedNodes[i].Text == ShowMsgInfos.GetInfo("SInitReport", "报表") && checkedNodes[i].Checked)
						{
							AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
							accMonitorLogBll.InitTable();
							CheckInOutBll checkInOutBll = new CheckInOutBll(MainForm._ia);
							checkInOutBll.InitTable();
						}
						if (checkedNodes[i].Text == ShowMsgInfos.GetInfo("SInitDevice", "设备") && checkedNodes[i].Checked)
						{
							MachinesBll machinesBll = new MachinesBll(MainForm._ia);
							machinesBll.InitTable();
							AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
							accDoorBll.InitTable();
							PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
							personnelAreaBll.InitTable();
							AccInterlockBll accInterlockBll = new AccInterlockBll(MainForm._ia);
							accInterlockBll.InitTable();
							AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
							accLinkAgeIoBll.InitTable();
							AccMapBll accMapBll = new AccMapBll(MainForm._ia);
							accMapBll.InitTable();
							AccMapdoorposBll accMapdoorposBll = new AccMapdoorposBll(MainForm._ia);
							accMapdoorposBll.InitTable();
							AccAntibackBll accAntibackBll = new AccAntibackBll(MainForm._ia);
							accAntibackBll.InitTable();
							AccMorecardempGroupBll accMorecardempGroupBll2 = new AccMorecardempGroupBll(MainForm._ia);
							accMorecardempGroupBll2.InitTable();
							AccMorecardGroupBll accMorecardGroupBll2 = new AccMorecardGroupBll(MainForm._ia);
							accMorecardGroupBll2.InitTable();
							AccMorecardsetBll accMorecardsetBll2 = new AccMorecardsetBll(MainForm._ia);
							accMorecardsetBll2.InitTable();
							AccLevelsetBll accLevelsetBll2 = new AccLevelsetBll(MainForm._ia);
							accLevelsetBll2.InitTable();
							AccLevelsetDoorGroupBll accLevelsetDoorGroupBll2 = new AccLevelsetDoorGroupBll(MainForm._ia);
							accLevelsetDoorGroupBll2.InitTable();
							AccLevelsetEmpBll accLevelsetEmpBll2 = new AccLevelsetEmpBll(MainForm._ia);
							accLevelsetEmpBll2.InitTable();
							AccFirstOpenBll accFirstOpenBll2 = new AccFirstOpenBll(MainForm._ia);
							accFirstOpenBll2.InitTable();
							AccFirstOpenEmpBll accFirstOpenEmpBll2 = new AccFirstOpenEmpBll(MainForm._ia);
							accFirstOpenEmpBll2.InitTable();
							AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
							accAuxiliaryBll.InitTable();
							AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
							accReaderBll.InitTable();
						}
						if (checkedNodes[i].Text == ShowMsgInfos.GetInfo("SInitAccess", "门禁") && checkedNodes[i].Checked)
						{
							AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
							accHolidaysBll.InitTable();
							AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
							accTimesegBll.InitTable();
							DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
							devCmdsBll.InitTable();
							AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
							accWiegandfmtBll.InitTable();
							AttParamBll attParamBll = new AttParamBll(MainForm._ia);
							AttParam model = attParamBll.GetModel("DBVersionAccess");
							if (model == null)
							{
								model = new AttParam();
								model.PARANAME = "DBVersionAccess";
								model.PARATYPE = null;
								attParamBll.Add(model);
							}
							else
							{
								attParamBll.Update(model);
							}
						}
						if (checkedNodes[i].Text == ShowMsgInfos.GetInfo("SInitSystem", "系统") && checkedNodes[i].Checked)
						{
							AuthGroupBll authGroupBll = new AuthGroupBll(MainForm._ia);
							authGroupBll.InitTable();
							AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
							authUserBll.InitTable();
							DbBackUpLogBll dbBackUpLogBll = new DbBackUpLogBll(MainForm._ia);
							dbBackUpLogBll.InitTable();
							IClockDsTimeBll clockDsTimeBll = new IClockDsTimeBll(MainForm._ia);
							clockDsTimeBll.InitTable();
							ActionLogBll actionLogBll = new ActionLogBll(MainForm._ia);
							actionLogBll.InitTable();
						}
					}
					if (AppSite.Instance.DataType == DataType.Access)
					{
						DbHelperOleDb.CompactAccessDB(true);
					}
					OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SSystemInit1", "系统初始化"), 5, "system");
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SInitOK", "系统初始化完成,请重启软件以使之生效"));
					Program.IsRestart = true;
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
				base.Close();
			}
		}

		private List<TreeNode> GetCheckedNodes(TreeNodeCollection nodes)
		{
			List<TreeNode> list = new List<TreeNode>();
			if (nodes != null && nodes.Count > 0)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					if (nodes[i].Checked)
					{
						list.Add(nodes[i]);
					}
				}
			}
			return list;
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
			TreeNode treeNode = new TreeNode("人事");
			TreeNode treeNode2 = new TreeNode("设备");
			TreeNode treeNode3 = new TreeNode("门禁");
			TreeNode treeNode4 = new TreeNode("系统");
			TreeNode treeNode5 = new TreeNode("报表");
			TreeNode treeNode6 = new TreeNode("全部", new TreeNode[5]
			{
				treeNode,
				treeNode2,
				treeNode3,
				treeNode4,
				treeNode5
			});
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(InitDatabaseForm));
			this.TView_Options = new TreeView();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			base.SuspendLayout();
			this.TView_Options.CheckBoxes = true;
			this.TView_Options.ItemHeight = 18;
			this.TView_Options.Location = new Point(1, 1);
			this.TView_Options.Name = "TView_Options";
			treeNode.Checked = true;
			treeNode.Name = "nodeEmp";
			treeNode.Text = "人事";
			treeNode2.Checked = true;
			treeNode2.Name = "nodeDevice";
			treeNode2.Text = "设备";
			treeNode3.Checked = true;
			treeNode3.Name = "nodeAccess";
			treeNode3.Text = "门禁";
			treeNode4.Checked = true;
			treeNode4.Name = "nodeSys";
			treeNode4.Text = "系统";
			treeNode5.Checked = true;
			treeNode5.Name = "nodeReport";
			treeNode5.Text = "报表";
			treeNode6.Checked = true;
			treeNode6.Name = "nodeAll";
			treeNode6.Text = "全部";
			this.TView_Options.Nodes.AddRange(new TreeNode[1]
			{
				treeNode6
			});
			this.TView_Options.Size = new Size(304, 229);
			this.TView_Options.TabIndex = 4;
			this.TView_Options.TabStop = false;
			this.TView_Options.AfterCheck += this.TView_device_AfterCheck;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(216, 239);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 1;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(122, 239);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 0;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(310, 274);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.TView_Options);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "InitDatabaseForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "系统初始化";
			base.ResumeLayout(false);
		}
	}
}
