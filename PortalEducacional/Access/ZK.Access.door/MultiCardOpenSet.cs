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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class MultiCardOpenSet : Office2007Form
	{
		private int m_id = 0;

		private AccMorecardset oldOpenSet;

		private AccDoorBll mbll = new AccDoorBll(MainForm._ia);

		private Dictionary<int, AccDoor> dicDoorId_Door = new Dictionary<int, AccDoor>();

		private Dictionary<int, int> indexidlist = new Dictionary<int, int>();

		private Dictionary<int, Machines> dicMachineId_Machine;

		private Dictionary<string, AccMorecardempGroup> dicGroupName_Group;

		private Dictionary<string, int> groupnameidlist = new Dictionary<string, int>();

		private Dictionary<int, List<AccMorecardGroup>> glist = new Dictionary<int, List<AccMorecardGroup>>();

		private DataTable AllGroup;

		private DataTable StdGroup;

		private DataTable dtGroup1;

		private DataTable dtGroup2;

		private DataTable dtGroup3;

		private DataTable dtGroup4;

		private DataTable dtGroup5;

		private DataTable dtCombNo;

		private List<int> lstExistsCombNO;

		private AccMorecardset MCSet;

		private IContainer components = null;

		private TextBox txt_name;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_groupName;

		private Label lb_door;

		private ComboBox cmb_door;

		private Label lb_group5_num;

		private ComboBox cmb_group_num5;

		private ComboBox cmb_group5;

		private Label lb_group5;

		private Label lb_group4_num;

		private ComboBox cmb_group_num4;

		private ComboBox cmb_group4;

		private Label lb_grpup4;

		private Label lb_group3_num;

		private ComboBox cmb_group_num3;

		private ComboBox cmb_group3;

		private Label lb_group3;

		private Label lb_group2_num;

		private ComboBox cmb_group_num2;

		private ComboBox cmb_group2;

		private Label lb_group2;

		private Label lb_group1_num;

		private ComboBox cmb_group_num1;

		private ComboBox cmb_group1;

		private Label lb_group1;

		private PanelEx panelEx1;

		private Label lb_groupSet;

		private Label label2;

		private Label label1;

		private ComboBox cbbUnlockGroupId;

		private Label lblUnlockGroupId;

		private GroupBox groupBox1;

		public event EventHandler RefreshDataEvent = null;

		public MultiCardOpenSet(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.LoadMachine();
				this.InitDoors();
				this.InitGroupCmb();
				this.InitGroup();
				this.InitialEmpGroupDataTable();
				initLang.LocaleForm(this, base.Name);
				this.BindData();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void InitDoors()
		{
			try
			{
				this.dicDoorId_Door.Clear();
				this.cmb_door.Items.Clear();
				this.indexidlist.Clear();
				List<AccDoor> modelList = this.mbll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.dicMachineId_Machine.ContainsKey(modelList[i].device_id) && !this.dicDoorId_Door.ContainsKey(modelList[i].id))
						{
							this.indexidlist.Add(this.cmb_door.Items.Count, modelList[i].id);
							this.dicDoorId_Door.Add(modelList[i].id, modelList[i]);
							this.cmb_door.Items.Add(modelList[i].door_name);
						}
					}
				}
				if (this.cmb_door.Items.Count > 0)
				{
					this.cmb_door.SelectedIndex = 0;
				}
				else if (this.m_id < 1)
				{
					this.cmb_door.Items.Add("-----");
					this.cmb_door.SelectedIndex = 0;
					this.indexidlist.Add(0, 0);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void LoadMachine()
		{
			try
			{
				this.dicMachineId_Machine = new Dictionary<int, Machines>();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> modelList = machinesBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						Machines machines = modelList[i];
						if (!this.dicMachineId_Machine.ContainsKey(machines.ID))
						{
							this.dicMachineId_Machine.Add(machines.ID, machines);
						}
						else
						{
							this.dicMachineId_Machine[machines.ID] = machines;
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void InitGroupCmb()
		{
			try
			{
				this.cmb_group1.Items.Clear();
				this.groupnameidlist.Clear();
				this.cmb_group1.Items.Add("-----");
				this.cmb_group2.Items.Add("-----");
				this.cmb_group3.Items.Add("-----");
				this.cmb_group4.Items.Add("-----");
				this.cmb_group5.Items.Add("-----");
				this.groupnameidlist.Add("-----", 0);
				this.dicGroupName_Group = new Dictionary<string, AccMorecardempGroup>();
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				List<AccMorecardempGroup> modelList = accMorecardempGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].Acount > 0 && !this.groupnameidlist.ContainsKey(modelList[i].group_name))
						{
							this.groupnameidlist.Add(modelList[i].group_name, modelList[i].id);
							this.cmb_group1.Items.Add(modelList[i].group_name);
							this.cmb_group2.Items.Add(modelList[i].group_name);
							this.cmb_group3.Items.Add(modelList[i].group_name);
							this.cmb_group4.Items.Add(modelList[i].group_name);
							this.cmb_group5.Items.Add(modelList[i].group_name);
						}
						if (!this.dicGroupName_Group.ContainsKey(modelList[i].group_name))
						{
							this.dicGroupName_Group.Add(modelList[i].group_name, modelList[i]);
						}
						else
						{
							this.dicGroupName_Group[modelList[i].group_name] = modelList[i];
						}
					}
				}
				if (this.cmb_group1.Items.Count == 1)
				{
					this.btn_OK.Enabled = false;
				}
				this.cmb_group1.SelectedIndex = 0;
				this.cmb_group2.SelectedIndex = 0;
				this.cmb_group3.SelectedIndex = 0;
				this.cmb_group4.SelectedIndex = 0;
				this.cmb_group5.SelectedIndex = 0;
				for (int j = 0; j < 6; j++)
				{
					this.cmb_group_num1.Items.Add(j);
					this.cmb_group_num2.Items.Add(j);
					this.cmb_group_num3.Items.Add(j);
					this.cmb_group_num4.Items.Add(j);
					this.cmb_group_num5.Items.Add(j);
				}
				this.cmb_group_num1.SelectedIndex = 0;
				this.cmb_group_num2.SelectedIndex = 0;
				this.cmb_group_num3.SelectedIndex = 0;
				this.cmb_group_num4.SelectedIndex = 0;
				this.cmb_group_num5.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void InitGroup()
		{
			try
			{
				AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
				List<AccMorecardGroup> modelList = accMorecardGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.glist.ContainsKey(modelList[i].comb_id))
						{
							this.glist[modelList[i].comb_id].Add(modelList[i]);
						}
						else
						{
							List<AccMorecardGroup> list = new List<AccMorecardGroup>();
							list.Add(modelList[i]);
							this.glist.Add(modelList[i].comb_id, list);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void SelectCombox(ComboBox cmb, int value)
		{
			int num = 0;
			while (true)
			{
				if (num < cmb.Items.Count)
				{
					if (this.groupnameidlist.ContainsKey(cmb.Items[num].ToString()) && this.groupnameidlist[cmb.Items[num].ToString()] == value)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			cmb.SelectedIndex = num;
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccMorecardset accMorecardset = null;
					AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
					accMorecardset = accMorecardsetBll.GetModel(this.m_id);
					if (accMorecardset != null)
					{
						this.oldOpenSet = accMorecardset.Copy();
						this.txt_name.Text = accMorecardset.comb_name;
						if (this.dicDoorId_Door.ContainsKey(accMorecardset.door_id))
						{
							this.cmb_door.Text = this.dicDoorId_Door[accMorecardset.door_id].door_name;
						}
						else
						{
							AccDoor model = this.mbll.GetModel(accMorecardset.door_id);
							if (model != null)
							{
								this.dicDoorId_Door.Add(model.id, model);
								this.cmb_door.Items.Add(model.door_name);
								this.cmb_door.Text = model.door_name;
								this.indexidlist.Add(this.cmb_door.Items.Count - 1, model.id);
							}
							else
							{
								this.cmb_door.Items.Add("-----");
								this.cmb_door.SelectedIndex = 0;
								this.indexidlist.Add(this.cmb_door.Items.Count - 1, 0);
							}
						}
						this.InitialCombDataTable();
						this.cbbUnlockGroupId.SelectedValue = accMorecardset.CombNo;
					}
				}
				else
				{
					this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				}
				this.cmb_door_SelectedIndexChanged(null, null);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private bool BindModel(AccMorecardset model)
		{
			if (this.check())
			{
				model.comb_name = this.txt_name.Text;
				model.door_id = this.indexidlist[this.cmb_door.SelectedIndex];
				int.TryParse((this.cbbUnlockGroupId.SelectedValue ?? "").ToString(), out int combNo);
				model.CombNo = combNo;
				return true;
			}
			return false;
		}

		private bool check()
		{
			try
			{
				Machines machines = null;
				int key = this.indexidlist[this.cmb_door.SelectedIndex];
				if (this.dicDoorId_Door.ContainsKey(key))
				{
					AccDoor accDoor = this.dicDoorId_Door[key];
					if (this.dicMachineId_Machine.ContainsKey(accDoor.device_id))
					{
						machines = this.dicMachineId_Machine[accDoor.device_id];
					}
				}
				if (string.IsNullOrEmpty(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
					this.txt_name.Focus();
					return false;
				}
				if (this.cmb_door.SelectedIndex < 0 || string.IsNullOrEmpty(this.cmb_door.Text) || this.cmb_door.Text.IndexOf("----") >= 0 || !this.indexidlist.ContainsKey(this.cmb_door.SelectedIndex))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDoor", "请选择门"));
					this.cmb_door.Focus();
					return false;
				}
				if (this.cmb_group1.SelectedIndex < 1 && this.cmb_group2.SelectedIndex < 1 && this.cmb_group3.SelectedIndex < 1 && this.cmb_group4.SelectedIndex < 1 && this.cmb_group5.SelectedIndex < 1)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFirstSelectGroup", "请选择开门组"));
					return false;
				}
				int num = 0;
				if (this.cmb_group1.SelectedIndex > 0)
				{
					num += this.cmb_group_num1.SelectedIndex;
				}
				if (this.cmb_group2.SelectedIndex > 0)
				{
					num += this.cmb_group_num2.SelectedIndex;
				}
				if (this.cmb_group3.SelectedIndex > 0)
				{
					num += this.cmb_group_num3.SelectedIndex;
				}
				if (this.cmb_group4.SelectedIndex > 0)
				{
					num += this.cmb_group_num4.SelectedIndex;
				}
				if (this.cmb_group5.SelectedIndex > 0)
				{
					num += this.cmb_group_num5.SelectedIndex;
				}
				if ((machines == null || machines.DevSDKType != SDKType.StandaloneSDK) && num < 2)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAtleastTwoPersons", "至少两人同时开门!"));
					return false;
				}
				if (num > 5)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAtMostFivePersons", "最多五人同时开门!"));
					return false;
				}
				int num2;
				if (this.cmb_group1.SelectedIndex > 0 && this.cmb_group_num1.SelectedIndex > 0)
				{
					if (this.cmb_group1.SelectedIndex == this.cmb_group2.SelectedIndex && this.cmb_group_num2.SelectedIndex > 0)
					{
						goto IL_0384;
					}
					if (this.cmb_group1.SelectedIndex == this.cmb_group3.SelectedIndex && this.cmb_group_num3.SelectedIndex > 0)
					{
						goto IL_0384;
					}
					if (this.cmb_group1.SelectedIndex == this.cmb_group4.SelectedIndex && this.cmb_group_num4.SelectedIndex > 0)
					{
						goto IL_0384;
					}
					num2 = ((this.cmb_group1.SelectedIndex == this.cmb_group5.SelectedIndex && this.cmb_group_num5.SelectedIndex > 0) ? 1 : 0);
				}
				else
				{
					num2 = 0;
				}
				goto IL_0388;
				IL_0442:
				int num3 = 1;
				goto IL_0446;
				IL_0446:
				if (num3 != 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOpenGroupNoSame", "选择开门组不能相同"));
					return false;
				}
				if (this.cmb_group3.SelectedIndex > 0 && this.cmb_group_num3.SelectedIndex > 0 && ((this.cmb_group3.SelectedIndex == this.cmb_group4.SelectedIndex && this.cmb_group_num4.SelectedIndex > 0) || (this.cmb_group3.SelectedIndex == this.cmb_group5.SelectedIndex && this.cmb_group_num5.SelectedIndex > 0)))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOpenGroupNoSame", "选择开门组不能相同"));
					return false;
				}
				if (this.cmb_group4.SelectedIndex > 0 && this.cmb_group_num4.SelectedIndex > 0 && this.cmb_group4.SelectedIndex == this.cmb_group5.SelectedIndex && this.cmb_group_num5.SelectedIndex > 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOpenGroupNoSame", "选择开门组不能相同"));
					return false;
				}
				if (this.dicGroupName_Group.ContainsKey(this.cmb_group1.Text))
				{
					AccMorecardempGroup accMorecardempGroup = this.dicGroupName_Group[this.cmb_group1.Text];
					if (this.cmb_group_num1.SelectedIndex > accMorecardempGroup.Acount)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UserCountMoreThanGroupUserCount", "开门人数不能大于组内人数"));
						this.cmb_group_num1.Focus();
						return false;
					}
				}
				if (this.dicGroupName_Group.ContainsKey(this.cmb_group2.Text))
				{
					AccMorecardempGroup accMorecardempGroup = this.dicGroupName_Group[this.cmb_group2.Text];
					if (this.cmb_group_num2.SelectedIndex > accMorecardempGroup.Acount)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UserCountMoreThanGroupUserCount", "开门人数不能大于组内人数"));
						this.cmb_group_num2.Focus();
						return false;
					}
				}
				if (this.dicGroupName_Group.ContainsKey(this.cmb_group3.Text))
				{
					AccMorecardempGroup accMorecardempGroup = this.dicGroupName_Group[this.cmb_group3.Text];
					if (this.cmb_group_num3.SelectedIndex > accMorecardempGroup.Acount)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UserCountMoreThanGroupUserCount", "开门人数不能大于组内人数"));
						this.cmb_group_num3.Focus();
						return false;
					}
				}
				if (this.dicGroupName_Group.ContainsKey(this.cmb_group4.Text))
				{
					AccMorecardempGroup accMorecardempGroup = this.dicGroupName_Group[this.cmb_group4.Text];
					if (this.cmb_group_num4.SelectedIndex > accMorecardempGroup.Acount)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UserCountMoreThanGroupUserCount", "开门人数不能大于组内人数"));
						this.cmb_group_num4.Focus();
						return false;
					}
				}
				if (this.dicGroupName_Group.ContainsKey(this.cmb_group5.Text))
				{
					AccMorecardempGroup accMorecardempGroup = this.dicGroupName_Group[this.cmb_group5.Text];
					if (this.cmb_group_num5.SelectedIndex > accMorecardempGroup.Acount)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UserCountMoreThanGroupUserCount", "开门人数不能大于组内人数"));
						this.cmb_group_num5.Focus();
						return false;
					}
				}
				return true;
				IL_0384:
				num2 = 1;
				goto IL_0388;
				IL_0388:
				if (num2 != 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOpenGroupNoSame", "选择开门组不能相同"));
					return false;
				}
				if (this.cmb_group2.SelectedIndex > 0 && this.cmb_group_num2.SelectedIndex > 0)
				{
					if (this.cmb_group2.SelectedIndex == this.cmb_group3.SelectedIndex && this.cmb_group_num3.SelectedIndex > 0)
					{
						goto IL_0442;
					}
					if (this.cmb_group2.SelectedIndex == this.cmb_group4.SelectedIndex && this.cmb_group_num4.SelectedIndex > 0)
					{
						goto IL_0442;
					}
					num3 = ((this.cmb_group2.SelectedIndex == this.cmb_group5.SelectedIndex && this.cmb_group_num5.SelectedIndex > 0) ? 1 : 0);
				}
				else
				{
					num3 = 0;
				}
				goto IL_0446;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void SaveGroup(int cmbid)
		{
			try
			{
				int num = 0;
				AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
				if (this.cmb_group1.SelectedIndex > 0 && this.cmb_group_num1.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group1.Text))
				{
					AccMorecardGroup accMorecardGroup = new AccMorecardGroup();
					accMorecardGroup.group_id = this.groupnameidlist[this.cmb_group1.Text];
					accMorecardGroup.opener_number = this.cmb_group_num1.SelectedIndex;
					accMorecardGroup.comb_id = cmbid;
					accMorecardGroupBll.Add(accMorecardGroup);
					num = accMorecardGroup.opener_number;
				}
				if (num < 5 && this.cmb_group2.SelectedIndex > 0 && this.cmb_group_num2.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group2.Text))
				{
					AccMorecardGroup accMorecardGroup2 = new AccMorecardGroup();
					accMorecardGroup2.group_id = this.groupnameidlist[this.cmb_group2.Text];
					accMorecardGroup2.opener_number = this.cmb_group_num2.SelectedIndex;
					accMorecardGroup2.comb_id = cmbid;
					if (accMorecardGroup2.opener_number > 5 - num)
					{
						accMorecardGroup2.opener_number = 5 - num;
					}
					accMorecardGroupBll.Add(accMorecardGroup2);
					num += accMorecardGroup2.opener_number;
				}
				if (num < 5 && this.cmb_group3.SelectedIndex > 0 && this.cmb_group_num3.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group3.Text))
				{
					AccMorecardGroup accMorecardGroup3 = new AccMorecardGroup();
					accMorecardGroup3.group_id = this.groupnameidlist[this.cmb_group3.Text];
					accMorecardGroup3.opener_number = this.cmb_group_num3.SelectedIndex;
					accMorecardGroup3.comb_id = cmbid;
					if (accMorecardGroup3.opener_number > 5 - num)
					{
						accMorecardGroup3.opener_number = 5 - num;
					}
					accMorecardGroupBll.Add(accMorecardGroup3);
					num += accMorecardGroup3.opener_number;
				}
				if (num < 5 && this.cmb_group4.SelectedIndex > 0 && this.cmb_group_num4.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group4.Text))
				{
					AccMorecardGroup accMorecardGroup4 = new AccMorecardGroup();
					accMorecardGroup4.group_id = this.groupnameidlist[this.cmb_group4.Text];
					accMorecardGroup4.opener_number = this.cmb_group_num4.SelectedIndex;
					accMorecardGroup4.comb_id = cmbid;
					if (accMorecardGroup4.opener_number > 5 - num)
					{
						accMorecardGroup4.opener_number = 5 - num;
					}
					accMorecardGroupBll.Add(accMorecardGroup4);
					num += accMorecardGroup4.opener_number;
				}
				if (num < 5 && this.cmb_group5.SelectedIndex > 0 && this.cmb_group_num5.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group5.Text))
				{
					AccMorecardGroup accMorecardGroup5 = new AccMorecardGroup();
					accMorecardGroup5.group_id = this.groupnameidlist[this.cmb_group5.Text];
					accMorecardGroup5.opener_number = this.cmb_group_num5.SelectedIndex;
					accMorecardGroup5.comb_id = cmbid;
					if (accMorecardGroup5.opener_number > 5 - num)
					{
						accMorecardGroup5.opener_number = 5 - num;
					}
					accMorecardGroupBll.Add(accMorecardGroup5);
					num += accMorecardGroup5.opener_number;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool Save()
		{
			int num = -1;
			try
			{
				Machines machines = null;
				AccMorecardset accMorecardset = null;
				AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accMorecardset = accMorecardsetBll.GetModel(this.m_id);
					num = accMorecardset.door_id;
					if (accMorecardset != null)
					{
						if (this.dicDoorId_Door.ContainsKey(accMorecardset.door_id))
						{
							AccDoor accDoor = this.dicDoorId_Door[accMorecardset.door_id];
							if (this.dicMachineId_Machine.ContainsKey(accDoor.device_id))
							{
								machines = this.dicMachineId_Machine[accDoor.device_id];
							}
						}
						if (machines == null || machines.DevSDKType != SDKType.StandaloneSDK)
						{
							CommandServer.DelCmd(accMorecardset);
						}
					}
				}
				if (accMorecardset == null)
				{
					accMorecardset = new AccMorecardset();
					if (accMorecardsetBll.Exists(this.txt_name.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.txt_name.Focus();
						return false;
					}
				}
				else if (accMorecardset.comb_name != this.txt_name.Text && accMorecardsetBll.Exists(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
					this.txt_name.Focus();
					return false;
				}
				if (this.BindModel(accMorecardset))
				{
					if (this.m_id > 0)
					{
						if (this.dicDoorId_Door.ContainsKey(accMorecardset.door_id))
						{
							AccDoor accDoor = this.dicDoorId_Door[accMorecardset.door_id];
							if (this.dicMachineId_Machine.ContainsKey(accDoor.device_id))
							{
								machines = this.dicMachineId_Machine[accDoor.device_id];
								if (machines.DevSDKType == SDKType.StandaloneSDK && !this.CheckGroupCount(accDoor))
								{
									return false;
								}
							}
						}
						if (accMorecardsetBll.Update(accMorecardset))
						{
							AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
							accMorecardGroupBll.DeleteByCmbID(this.m_id);
							this.SaveGroup(this.m_id);
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							if (machines == null || machines.DevSDKType != SDKType.StandaloneSDK)
							{
								CommandServer.AddCmd(accMorecardset);
							}
							else
							{
								CommandServer.AddAllUnlockGroupCmd(machines);
							}
							if (num == accMorecardset.door_id)
							{
								goto IL_02c4;
							}
							goto IL_02c4;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
					else
					{
						try
						{
							if (this.dicDoorId_Door.ContainsKey(accMorecardset.door_id))
							{
								AccDoor accDoor = this.dicDoorId_Door[accMorecardset.door_id];
								if (this.dicMachineId_Machine.ContainsKey(accDoor.device_id))
								{
									machines = this.dicMachineId_Machine[accDoor.device_id];
									if (machines.DevSDKType == SDKType.StandaloneSDK)
									{
										List<AccMorecardset> modelList = accMorecardsetBll.GetModelList("door_id=" + accDoor.id);
										if (modelList != null && modelList.Count >= 9)
										{
											SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("STDMachineCombiCountError", "脱机设备最多定义9个开门组合"));
											return false;
										}
										if (!this.CheckGroupCount(accDoor))
										{
											return false;
										}
									}
								}
							}
							accMorecardset.create_operator = SysInfos.SysUserInfo.id.ToString();
							accMorecardsetBll.Add(accMorecardset);
							int cmbid = accMorecardsetBll.GetMaxId() - 1;
							this.SaveGroup(cmbid);
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							if (machines == null || machines.DevSDKType != SDKType.StandaloneSDK)
							{
								CommandServer.AddCmd(accMorecardset);
							}
							else
							{
								CommandServer.AddAllUnlockGroupCmd(machines);
							}
							FrmShowUpdata.Instance.ShowEx();
							return true;
						}
						catch (Exception ex)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("ExceptionOnSaveDataFailure", "保存数据失败") + ":" + ex.Message);
						}
					}
				}
				goto end_IL_0003;
				IL_02c4:
				return true;
				end_IL_0003:;
			}
			catch (Exception ex2)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("ExceptionOnSaveDataFailure", "保存数据失败:" + ex2.Message));
			}
			return false;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				FrmShowUpdata.Instance.ShowEx();
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void cmb_group1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_group1.SelectedIndex > 0)
			{
				this.cmb_group_num1.Enabled = true;
			}
			else
			{
				this.cmb_group1.SelectedValue = 0;
				this.cmb_group_num1.Enabled = false;
			}
			if (this.cmb_group1.DataSource != null)
			{
				this.cmb_group1.BindingContext[this.cmb_group1.DataSource].EndCurrentEdit();
			}
		}

		private void cmb_group2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_group2.SelectedIndex > 0)
			{
				this.cmb_group_num2.Enabled = true;
			}
			else
			{
				this.cmb_group2.SelectedValue = 0;
				this.cmb_group_num2.Enabled = false;
			}
			if (this.cmb_group2.DataSource != null)
			{
				this.cmb_group2.BindingContext[this.cmb_group2.DataSource].EndCurrentEdit();
			}
		}

		private void cmb_group3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_group3.SelectedIndex > 0)
			{
				this.cmb_group_num3.Enabled = true;
			}
			else
			{
				this.cmb_group3.SelectedValue = 0;
				this.cmb_group_num3.Enabled = false;
			}
			if (this.cmb_group3.DataSource != null)
			{
				this.cmb_group3.BindingContext[this.cmb_group3.DataSource].EndCurrentEdit();
			}
		}

		private void cmb_group4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_group4.SelectedIndex > 0)
			{
				this.cmb_group_num4.Enabled = true;
			}
			else
			{
				this.cmb_group4.SelectedValue = 0;
				this.cmb_group_num4.Enabled = false;
			}
			if (this.cmb_group4.DataSource != null)
			{
				this.cmb_group4.BindingContext[this.cmb_group4.DataSource].EndCurrentEdit();
			}
		}

		private void cmb_group5_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_group5.SelectedIndex > 0)
			{
				this.cmb_group_num5.Enabled = true;
			}
			else
			{
				this.cmb_group5.SelectedValue = 0;
				this.cmb_group_num5.Enabled = false;
			}
			if (this.cmb_group5.DataSource != null)
			{
				this.cmb_group5.BindingContext[this.cmb_group5.DataSource].EndCurrentEdit();
			}
		}

		private bool CheckGroupCount(AccDoor Door)
		{
			AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
			DataTable fields = accMorecardGroupBll.GetFields("max(group_id)", "comb_id in(select id from acc_morecardset where door_id=" + Door.id + ") group by group_id");
			List<int> list = new List<int>();
			int item;
			for (int i = 0; i < fields.Rows.Count; i++)
			{
				if (int.TryParse(fields.Rows[i][0].ToString(), out item))
				{
					list.Add(item);
				}
			}
			bool flag = false;
			if (this.cmb_group1.SelectedIndex > 0 && this.cmb_group_num1.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group1.Text))
			{
				item = this.groupnameidlist[this.cmb_group1.Text];
				if (!list.Contains(item))
				{
					flag = true;
				}
			}
			if (this.cmb_group2.SelectedIndex > 0 && this.cmb_group_num2.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group2.Text))
			{
				item = this.groupnameidlist[this.cmb_group2.Text];
				if (!list.Contains(item))
				{
					flag = true;
				}
			}
			if (this.cmb_group3.SelectedIndex > 0 && this.cmb_group_num3.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group3.Text))
			{
				item = this.groupnameidlist[this.cmb_group3.Text];
				if (!list.Contains(item))
				{
					flag = true;
				}
			}
			if (this.cmb_group4.SelectedIndex > 0 && this.cmb_group_num4.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group4.Text))
			{
				item = this.groupnameidlist[this.cmb_group4.Text];
				if (!list.Contains(item))
				{
					flag = true;
				}
			}
			if (this.cmb_group5.SelectedIndex > 0 && this.cmb_group_num5.SelectedIndex > 0 && this.groupnameidlist.ContainsKey(this.cmb_group5.Text))
			{
				item = this.groupnameidlist[this.cmb_group5.Text];
				if (!list.Contains(item))
				{
					flag = true;
				}
			}
			if (flag && list.Count >= 4)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("STDMachineGroupCountError", "脱机设备最多指定4个组"));
				return false;
			}
			return true;
		}

		private void cmb_door_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cbbUnlockGroupId.DataSource != null)
			{
				this.BindingContext[this.cbbUnlockGroupId.DataSource].EndCurrentEdit();
			}
			this.cmb_group5.Enabled = true;
			int key = this.indexidlist[this.cmb_door.SelectedIndex];
			if (this.dicDoorId_Door.ContainsKey(key))
			{
				AccDoor accDoor = this.dicDoorId_Door[key];
				if (this.dicMachineId_Machine.ContainsKey(accDoor.device_id))
				{
					Machines machines = this.dicMachineId_Machine[accDoor.device_id];
					this.LoadMachineComb(machines);
					this.InitialCombDataTable();
					int.TryParse((this.cmb_group1.SelectedValue ?? ((object)0)).ToString(), out int num);
					int.TryParse((this.cmb_group2.SelectedValue ?? ((object)0)).ToString(), out int num2);
					int.TryParse((this.cmb_group3.SelectedValue ?? ((object)0)).ToString(), out int num3);
					int.TryParse((this.cmb_group4.SelectedValue ?? ((object)0)).ToString(), out int num4);
					int.TryParse((this.cmb_group5.SelectedValue ?? ((object)0)).ToString(), out int num5);
					this.cbbUnlockGroupId.Enabled = (machines.DevSDKType == SDKType.StandaloneSDK);
					if (machines.DevSDKType == SDKType.StandaloneSDK)
					{
						if (this.StdGroup != null)
						{
							this.dtGroup1 = this.StdGroup.Copy();
							this.dtGroup2 = this.StdGroup.Copy();
							this.dtGroup3 = this.StdGroup.Copy();
							this.dtGroup4 = this.StdGroup.Copy();
							this.dtGroup5 = this.StdGroup.Copy();
						}
					}
					else if (this.AllGroup != null)
					{
						this.dtGroup1 = this.AllGroup.Copy();
						this.dtGroup2 = this.AllGroup.Copy();
						this.dtGroup3 = this.AllGroup.Copy();
						this.dtGroup4 = this.AllGroup.Copy();
						this.dtGroup5 = this.AllGroup.Copy();
					}
					this.cmb_group1.DataSource = this.dtGroup1;
					this.cmb_group1.ValueMember = "Value";
					this.cmb_group1.DisplayMember = "Text";
					this.cmb_group1.SelectedValue = num;
					this.cmb_group2.DataSource = this.dtGroup2;
					this.cmb_group2.ValueMember = "Value";
					this.cmb_group2.DisplayMember = "Text";
					this.cmb_group2.SelectedValue = num2;
					this.cmb_group3.DataSource = this.dtGroup3;
					this.cmb_group3.ValueMember = "Value";
					this.cmb_group3.DisplayMember = "Text";
					this.cmb_group3.SelectedValue = num3;
					this.cmb_group4.DataSource = this.dtGroup4;
					this.cmb_group4.ValueMember = "Value";
					this.cmb_group4.DisplayMember = "Text";
					this.cmb_group4.SelectedValue = num4;
					this.cmb_group5.DataSource = this.dtGroup5;
					this.cmb_group5.ValueMember = "Value";
					this.cmb_group5.DisplayMember = "Text";
					this.cmb_group5.SelectedValue = num5;
					this.BindGroupSet();
				}
			}
		}

		private void InitialEmpGroupDataTable()
		{
			this.AllGroup = new DataTable();
			this.AllGroup.Columns.Add("Value", typeof(int));
			this.AllGroup.Columns.Add("Text", typeof(string));
			DataRow dataRow = this.AllGroup.NewRow();
			dataRow["Value"] = 0;
			dataRow["Text"] = "-----";
			this.AllGroup.Rows.Add(dataRow);
			this.StdGroup = this.AllGroup.Copy();
			foreach (KeyValuePair<string, AccMorecardempGroup> item in this.dicGroupName_Group)
			{
				dataRow = this.AllGroup.NewRow();
				dataRow["Value"] = item.Value.id;
				dataRow["Text"] = item.Value.group_name;
				this.AllGroup.Rows.Add(dataRow);
				if (item.Value.StdGroupNo > 0)
				{
					this.StdGroup.ImportRow(dataRow);
				}
			}
		}

		private void LoadMachineComb(Machines machine)
		{
			this.lstExistsCombNO = new List<int>();
			AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
			List<AccMorecardset> modelList = accMorecardsetBll.GetModelList("door_id in (select id from acc_door where device_id in (select id from Machines where id=" + machine.ID + "))");
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (modelList[i].id == this.m_id)
					{
						this.MCSet = modelList[i];
					}
					if (modelList[i].CombNo > 0 && !this.lstExistsCombNO.Contains(modelList[i].CombNo))
					{
						this.lstExistsCombNO.Add(modelList[i].CombNo);
					}
				}
			}
		}

		private void InitialCombDataTable()
		{
			this.dtCombNo = new DataTable();
			this.dtCombNo.Columns.Add("Value", typeof(int));
			this.dtCombNo.Columns.Add("Text", typeof(string));
			DataRow dataRow = this.dtCombNo.NewRow();
			dataRow["Value"] = 0;
			dataRow["Text"] = "--------------------------";
			this.dtCombNo.Rows.Add(dataRow);
			for (int i = 2; i <= 10; i++)
			{
				if (this.MCSet != null && this.MCSet.CombNo == i)
				{
					dataRow = this.dtCombNo.NewRow();
					dataRow["Value"] = i;
					dataRow["Text"] = i.ToString();
					this.dtCombNo.Rows.Add(dataRow);
				}
				else if (!this.lstExistsCombNO.Contains(i))
				{
					dataRow = this.dtCombNo.NewRow();
					dataRow["Value"] = i;
					dataRow["Text"] = i.ToString();
					this.dtCombNo.Rows.Add(dataRow);
				}
			}
			object selectedValue = this.cbbUnlockGroupId.SelectedValue;
			this.cbbUnlockGroupId.DataSource = this.dtCombNo;
			this.cbbUnlockGroupId.DisplayMember = "Text";
			this.cbbUnlockGroupId.ValueMember = "Value";
			if (selectedValue != null)
			{
				this.cbbUnlockGroupId.SelectedValue = selectedValue;
			}
		}

		private void BindGroupSet()
		{
			if (this.glist.ContainsKey(this.m_id))
			{
				List<AccMorecardGroup> list = this.glist[this.m_id];
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						switch (i)
						{
						case 0:
							this.cmb_group_num1.SelectedIndex = list[i].opener_number;
							this.cmb_group1.SelectedValue = list[i].group_id;
							break;
						case 1:
							this.cmb_group_num2.SelectedIndex = list[i].opener_number;
							this.cmb_group2.SelectedValue = list[i].group_id;
							break;
						case 2:
							this.cmb_group_num3.SelectedIndex = list[i].opener_number;
							this.cmb_group3.SelectedValue = list[i].group_id;
							break;
						case 3:
							this.cmb_group_num4.SelectedIndex = list[i].opener_number;
							this.cmb_group4.SelectedValue = list[i].group_id;
							break;
						case 4:
							this.cmb_group_num5.SelectedIndex = list[i].opener_number;
							this.cmb_group5.SelectedValue = list[i].group_id;
							break;
						}
					}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MultiCardOpenSet));
			this.txt_name = new TextBox();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_groupName = new Label();
			this.lb_door = new Label();
			this.cmb_door = new ComboBox();
			this.lb_group5_num = new Label();
			this.cmb_group_num5 = new ComboBox();
			this.cmb_group5 = new ComboBox();
			this.lb_group5 = new Label();
			this.lb_group4_num = new Label();
			this.cmb_group_num4 = new ComboBox();
			this.cmb_group4 = new ComboBox();
			this.lb_grpup4 = new Label();
			this.lb_group3_num = new Label();
			this.cmb_group_num3 = new ComboBox();
			this.cmb_group3 = new ComboBox();
			this.lb_group3 = new Label();
			this.lb_group2_num = new Label();
			this.cmb_group_num2 = new ComboBox();
			this.cmb_group2 = new ComboBox();
			this.lb_group2 = new Label();
			this.lb_group1_num = new Label();
			this.cmb_group_num1 = new ComboBox();
			this.cmb_group1 = new ComboBox();
			this.lb_group1 = new Label();
			this.panelEx1 = new PanelEx();
			this.lb_groupSet = new Label();
			this.label2 = new Label();
			this.label1 = new Label();
			this.cbbUnlockGroupId = new ComboBox();
			this.lblUnlockGroupId = new Label();
			this.groupBox1 = new GroupBox();
			this.panelEx1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.txt_name.Location = new Point(142, 44);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(208, 21);
			this.txt_name.TabIndex = 1;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(335, 320);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 13;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(233, 320);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 12;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_groupName.Location = new Point(15, 46);
			this.lb_groupName.Name = "lb_groupName";
			this.lb_groupName.Size = new Size(121, 15);
			this.lb_groupName.TabIndex = 47;
			this.lb_groupName.Text = "组合名称";
			this.lb_groupName.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_door.Location = new Point(15, 14);
			this.lb_door.Name = "lb_door";
			this.lb_door.Size = new Size(121, 12);
			this.lb_door.TabIndex = 46;
			this.lb_door.Text = "门选择";
			this.lb_door.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_door.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_door.FormattingEnabled = true;
			this.cmb_door.Location = new Point(142, 11);
			this.cmb_door.Name = "cmb_door";
			this.cmb_door.Size = new Size(208, 20);
			this.cmb_door.TabIndex = 0;
			this.cmb_door.SelectedIndexChanged += this.cmb_door_SelectedIndexChanged;
			this.lb_group5_num.AutoSize = true;
			this.lb_group5_num.Location = new Point(340, 126);
			this.lb_group5_num.Name = "lb_group5_num";
			this.lb_group5_num.Size = new Size(17, 12);
			this.lb_group5_num.TabIndex = 60;
			this.lb_group5_num.Text = "人";
			this.cmb_group_num5.Enabled = false;
			this.cmb_group_num5.FormattingEnabled = true;
			this.cmb_group_num5.Location = new Point(274, 121);
			this.cmb_group_num5.Name = "cmb_group_num5";
			this.cmb_group_num5.Size = new Size(61, 20);
			this.cmb_group_num5.TabIndex = 9;
			this.cmb_group5.FormattingEnabled = true;
			this.cmb_group5.Location = new Point(127, 121);
			this.cmb_group5.Name = "cmb_group5";
			this.cmb_group5.Size = new Size(140, 20);
			this.cmb_group5.TabIndex = 8;
			this.cmb_group5.SelectedIndexChanged += this.cmb_group5_SelectedIndexChanged;
			this.lb_group5.Location = new Point(12, 124);
			this.lb_group5.Name = "lb_group5";
			this.lb_group5.Size = new Size(109, 14);
			this.lb_group5.TabIndex = 55;
			this.lb_group5.Text = "组5";
			this.lb_group5.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_group4_num.AutoSize = true;
			this.lb_group4_num.Location = new Point(340, 100);
			this.lb_group4_num.Name = "lb_group4_num";
			this.lb_group4_num.Size = new Size(17, 12);
			this.lb_group4_num.TabIndex = 59;
			this.lb_group4_num.Text = "人";
			this.cmb_group_num4.Enabled = false;
			this.cmb_group_num4.FormattingEnabled = true;
			this.cmb_group_num4.Location = new Point(274, 95);
			this.cmb_group_num4.Name = "cmb_group_num4";
			this.cmb_group_num4.Size = new Size(61, 20);
			this.cmb_group_num4.TabIndex = 7;
			this.cmb_group4.FormattingEnabled = true;
			this.cmb_group4.Location = new Point(127, 95);
			this.cmb_group4.Name = "cmb_group4";
			this.cmb_group4.Size = new Size(140, 20);
			this.cmb_group4.TabIndex = 6;
			this.cmb_group4.SelectedIndexChanged += this.cmb_group4_SelectedIndexChanged;
			this.lb_grpup4.Location = new Point(12, 99);
			this.lb_grpup4.Name = "lb_grpup4";
			this.lb_grpup4.Size = new Size(109, 14);
			this.lb_grpup4.TabIndex = 54;
			this.lb_grpup4.Text = "组4";
			this.lb_grpup4.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_group3_num.AutoSize = true;
			this.lb_group3_num.Location = new Point(340, 74);
			this.lb_group3_num.Name = "lb_group3_num";
			this.lb_group3_num.Size = new Size(17, 12);
			this.lb_group3_num.TabIndex = 58;
			this.lb_group3_num.Text = "人";
			this.cmb_group_num3.Enabled = false;
			this.cmb_group_num3.FormattingEnabled = true;
			this.cmb_group_num3.Location = new Point(274, 69);
			this.cmb_group_num3.Name = "cmb_group_num3";
			this.cmb_group_num3.Size = new Size(61, 20);
			this.cmb_group_num3.TabIndex = 5;
			this.cmb_group3.FormattingEnabled = true;
			this.cmb_group3.Location = new Point(127, 69);
			this.cmb_group3.Name = "cmb_group3";
			this.cmb_group3.Size = new Size(140, 20);
			this.cmb_group3.TabIndex = 4;
			this.cmb_group3.SelectedIndexChanged += this.cmb_group3_SelectedIndexChanged;
			this.lb_group3.Location = new Point(12, 73);
			this.lb_group3.Name = "lb_group3";
			this.lb_group3.Size = new Size(109, 12);
			this.lb_group3.TabIndex = 53;
			this.lb_group3.Text = "组3";
			this.lb_group3.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_group2_num.AutoSize = true;
			this.lb_group2_num.Location = new Point(340, 48);
			this.lb_group2_num.Name = "lb_group2_num";
			this.lb_group2_num.Size = new Size(17, 12);
			this.lb_group2_num.TabIndex = 57;
			this.lb_group2_num.Text = "人";
			this.cmb_group_num2.Enabled = false;
			this.cmb_group_num2.FormattingEnabled = true;
			this.cmb_group_num2.Location = new Point(274, 43);
			this.cmb_group_num2.Name = "cmb_group_num2";
			this.cmb_group_num2.Size = new Size(61, 20);
			this.cmb_group_num2.TabIndex = 3;
			this.cmb_group2.FormattingEnabled = true;
			this.cmb_group2.Location = new Point(127, 43);
			this.cmb_group2.Name = "cmb_group2";
			this.cmb_group2.Size = new Size(140, 20);
			this.cmb_group2.TabIndex = 2;
			this.cmb_group2.SelectedIndexChanged += this.cmb_group2_SelectedIndexChanged;
			this.lb_group2.Location = new Point(12, 47);
			this.lb_group2.Name = "lb_group2";
			this.lb_group2.Size = new Size(109, 12);
			this.lb_group2.TabIndex = 52;
			this.lb_group2.Text = "组2";
			this.lb_group2.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_group1_num.AutoSize = true;
			this.lb_group1_num.Location = new Point(340, 20);
			this.lb_group1_num.Name = "lb_group1_num";
			this.lb_group1_num.Size = new Size(17, 12);
			this.lb_group1_num.TabIndex = 56;
			this.lb_group1_num.Text = "人";
			this.cmb_group_num1.Enabled = false;
			this.cmb_group_num1.FormattingEnabled = true;
			this.cmb_group_num1.Location = new Point(274, 15);
			this.cmb_group_num1.Name = "cmb_group_num1";
			this.cmb_group_num1.Size = new Size(61, 20);
			this.cmb_group_num1.TabIndex = 1;
			this.cmb_group1.FormattingEnabled = true;
			this.cmb_group1.Location = new Point(127, 15);
			this.cmb_group1.Name = "cmb_group1";
			this.cmb_group1.Size = new Size(140, 20);
			this.cmb_group1.TabIndex = 0;
			this.cmb_group1.SelectedIndexChanged += this.cmb_group1_SelectedIndexChanged;
			this.lb_group1.Location = new Point(12, 20);
			this.lb_group1.Name = "lb_group1";
			this.lb_group1.Size = new Size(109, 12);
			this.lb_group1.TabIndex = 51;
			this.lb_group1.Text = "组1";
			this.lb_group1.TextAlign = ContentAlignment.MiddleLeft;
			this.panelEx1.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Controls.Add(this.cmb_group5);
			this.panelEx1.Controls.Add(this.cmb_group4);
			this.panelEx1.Controls.Add(this.cmb_group3);
			this.panelEx1.Controls.Add(this.cmb_group2);
			this.panelEx1.Controls.Add(this.cmb_group1);
			this.panelEx1.Controls.Add(this.lb_group5_num);
			this.panelEx1.Controls.Add(this.cmb_group_num4);
			this.panelEx1.Controls.Add(this.lb_group3);
			this.panelEx1.Controls.Add(this.cmb_group_num5);
			this.panelEx1.Controls.Add(this.cmb_group_num3);
			this.panelEx1.Controls.Add(this.lb_group1);
			this.panelEx1.Controls.Add(this.lb_group2_num);
			this.panelEx1.Controls.Add(this.lb_group3_num);
			this.panelEx1.Controls.Add(this.cmb_group_num2);
			this.panelEx1.Controls.Add(this.lb_group5);
			this.panelEx1.Controls.Add(this.lb_grpup4);
			this.panelEx1.Controls.Add(this.cmb_group_num1);
			this.panelEx1.Controls.Add(this.lb_group4_num);
			this.panelEx1.Controls.Add(this.lb_group1_num);
			this.panelEx1.Controls.Add(this.lb_group2);
			this.panelEx1.Location = new Point(15, 146);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(402, 155);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 2;
			this.panelEx1.TabStop = true;
			this.lb_groupSet.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.lb_groupSet.AutoSize = true;
			this.lb_groupSet.Location = new Point(15, 129);
			this.lb_groupSet.Name = "lb_groupSet";
			this.lb_groupSet.Size = new Size(77, 12);
			this.lb_groupSet.TabIndex = 49;
			this.lb_groupSet.Text = "各组开门人数";
			this.lb_groupSet.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(353, 49);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 50;
			this.label2.Text = "*";
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(354, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 51;
			this.label1.Text = "*";
			this.cbbUnlockGroupId.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbbUnlockGroupId.FormattingEnabled = true;
			this.cbbUnlockGroupId.Location = new Point(130, 20);
			this.cbbUnlockGroupId.Name = "cbbUnlockGroupId";
			this.cbbUnlockGroupId.Size = new Size(208, 20);
			this.cbbUnlockGroupId.TabIndex = 52;
			this.lblUnlockGroupId.Location = new Point(3, 23);
			this.lblUnlockGroupId.Name = "lblUnlockGroupId";
			this.lblUnlockGroupId.Size = new Size(121, 12);
			this.lblUnlockGroupId.TabIndex = 53;
			this.lblUnlockGroupId.Text = "组合编号";
			this.lblUnlockGroupId.TextAlign = ContentAlignment.MiddleLeft;
			this.groupBox1.Controls.Add(this.cbbUnlockGroupId);
			this.groupBox1.Controls.Add(this.lblUnlockGroupId);
			this.groupBox1.Location = new Point(12, 71);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(405, 49);
			this.groupBox1.TabIndex = 54;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "脱机参数";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(432, 357);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.cmb_door);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.lb_groupSet);
			base.Controls.Add(this.panelEx1);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_groupName);
			base.Controls.Add(this.lb_door);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MultiCardOpenSet";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "多卡开门设置";
			this.panelEx1.ResumeLayout(false);
			this.panelEx1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
