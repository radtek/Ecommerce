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
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class LinkageEdit : Office2007Form
	{
		private int m_id = 0;

		private Dictionary<string, Dictionary<string, string>> m_inAddrTypeDic = null;

		private Dictionary<string, Dictionary<string, string>> m_outAddrTypeDic = null;

		private DataTable dtInAddrAux;

		private DataTable dtOutAddrAux;

		private DataTable dtInAddrDoor;

		private DataTable dtOutAddrDoor;

		private Machines machineSelected;

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private Dictionary<int, int> indexidlist = new Dictionary<int, int>();

		private Dictionary<int, string> eventTypeList = new Dictionary<int, string>();

		private List<int> eventTypeListex = new List<int>();

		private Dictionary<int, int> eventTypeIndexList = new Dictionary<int, int>();

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_event;

		private ComboBox cmb_type;

		private Label lb_dev;

		private ComboBox cmb_dev;

		private Label lb_name;

		private TextBox txt_name;

		private Label lb_inAddr;

		private ComboBox cmb_inAddr;

		private Label lb_outAddr;

		private ComboBox cmb_outaddr;

		private Label lb_outType;

		private ComboBox cmb_outtype;

		private TextBox txt_dalay;

		private Label lb_delay;

		private Label lb_second;

		private Label lb_actionType;

		private ComboBox cmb_actionType;

		private Label label2;

		private Label label1;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		public event EventHandler RefreshDataEvent = null;

		public LinkageEdit(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.InitOutAddrType();
				this.InitOutType(-1);
				this.InitActionType();
				this.InitMachines();
				this.loadEventType();
				if (this.cmb_outtype.Items.Count > 0)
				{
					this.cmb_outtype.SelectedIndex = 0;
					this.cmb_outtype_SelectedIndexChanged(null, null);
				}
				if (this.cmb_actionType.Items.Count > 0)
				{
					this.cmb_actionType.SelectedIndex = 0;
					this.com_actionType_SelectedIndexChanged(null, null);
				}
				this.cmb_dev_SelectedIndexChanged(null, null);
				this.BindData();
				initLang.LocaleForm(this, base.Name);
				initLang.ComboBoxAutoSize(this.cmb_dev, this);
				initLang.ComboBoxAutoSize(this.cmb_type, this);
				initLang.ComboBoxAutoSize(this.cmb_inAddr, this);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInfoLoadError", "数据加载失败"));
			}
		}

		private void InitActionType()
		{
			try
			{
				XmlNode rootNode = initLang.GetRootNode("LinkAnctionType");
				this.cmb_actionType.Items.Clear();
				if (rootNode != null)
				{
					string addNodeValueByName = initLang.GetAddNodeValueByName("0", rootNode, false);
					if (!string.IsNullOrEmpty(addNodeValueByName))
					{
						this.cmb_actionType.Items.Add(addNodeValueByName);
					}
					else
					{
						addNodeValueByName = ShowMsgInfos.GetInfo("SLinkAnctionTypeClose", "关闭");
						this.cmb_actionType.Items.Add(addNodeValueByName);
						initLang.SetAddNodeValue("0", addNodeValueByName, rootNode, false);
					}
					addNodeValueByName = initLang.GetAddNodeValueByName("1", rootNode, false);
					if (!string.IsNullOrEmpty(addNodeValueByName))
					{
						this.cmb_actionType.Items.Add(addNodeValueByName);
					}
					else
					{
						addNodeValueByName = ShowMsgInfos.GetInfo("SLinkAnctionTypeOpen", "打开");
						this.cmb_actionType.Items.Add(addNodeValueByName);
						initLang.SetAddNodeValue("1", addNodeValueByName, rootNode, false);
						initLang.Save();
					}
					addNodeValueByName = initLang.GetAddNodeValueByName("2", rootNode, false);
					if (!string.IsNullOrEmpty(addNodeValueByName))
					{
						this.cmb_actionType.Items.Add(addNodeValueByName);
					}
					else
					{
						addNodeValueByName = ShowMsgInfos.GetInfo("SLinkAnctionTypeNormalOpen", "常开");
						this.cmb_actionType.Items.Add(addNodeValueByName);
						initLang.SetAddNodeValue("2", addNodeValueByName, rootNode, false);
						initLang.Save();
					}
					if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
					{
						addNodeValueByName = initLang.GetAddNodeValueByName("3", rootNode, false);
						if (!string.IsNullOrEmpty(addNodeValueByName))
						{
							this.cmb_actionType.Items.Add(addNodeValueByName);
						}
						else
						{
							addNodeValueByName = ShowMsgInfos.GetInfo("SLinkAnctionTypeLocked", "锁定");
							this.cmb_actionType.Items.Add(addNodeValueByName);
							initLang.SetAddNodeValue("3", addNodeValueByName, rootNode, false);
							initLang.Save();
						}
					}
				}
				if (this.cmb_actionType.Items.Count > 0)
				{
					this.cmb_actionType.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitOutType(int devType)
		{
			try
			{
				this.cmb_outtype.Items.Clear();
				XmlNode rootNode = initLang.GetRootNode("LinkOutType");
				if (rootNode != null)
				{
					string addNodeValueByName = initLang.GetAddNodeValueByName("0", rootNode, false);
					if (!string.IsNullOrEmpty(addNodeValueByName))
					{
						this.cmb_outtype.Items.Add(addNodeValueByName);
					}
					else
					{
						addNodeValueByName = "门锁";
						this.cmb_outtype.Items.Add(addNodeValueByName);
						initLang.SetAddNodeValue("0", addNodeValueByName, rootNode, false);
					}
					addNodeValueByName = initLang.GetAddNodeValueByName("1", rootNode, false);
					if (!string.IsNullOrEmpty(addNodeValueByName))
					{
						if (devType != 101 && devType != 102 && devType != 103)
						{
							this.cmb_outtype.Items.Add(addNodeValueByName);
						}
					}
					else
					{
						addNodeValueByName = "辅助输出";
						if (devType != 101 && devType != 102 && devType != 103)
						{
							this.cmb_outtype.Items.Add(addNodeValueByName);
							initLang.SetAddNodeValue("1", addNodeValueByName, rootNode, false);
							initLang.Save();
						}
					}
				}
				if (this.cmb_outtype.Items.Count > 0)
				{
					this.cmb_outtype.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitInOutAddrType()
		{
			if (this.dtInAddrAux == null)
			{
				this.dtInAddrAux = new DataTable();
				this.dtInAddrAux.Columns.Add("Key");
				this.dtInAddrAux.Columns.Add("Value");
			}
			if (this.dtOutAddrAux == null)
			{
				this.dtOutAddrAux = this.dtInAddrAux.Clone();
			}
			if (this.dtInAddrDoor == null)
			{
				this.dtInAddrDoor = this.dtInAddrAux.Clone();
			}
			if (this.dtOutAddrDoor == null)
			{
				this.dtOutAddrDoor = this.dtInAddrAux.Clone();
			}
			if (this.cmb_dev.SelectedIndex > 0 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex) && this.indexidlist[this.cmb_dev.SelectedIndex] > 0 && this.mlist.ContainsKey(this.indexidlist[this.cmb_dev.SelectedIndex]))
			{
				Machines machines = this.mlist[this.indexidlist[this.cmb_dev.SelectedIndex]];
				if (machines != this.machineSelected)
				{
					this.dtInAddrAux.Rows.Clear();
					this.dtOutAddrAux.Rows.Clear();
					this.dtInAddrDoor.Rows.Clear();
					this.dtOutAddrDoor.Rows.Clear();
					this.machineSelected = machines;
					DataRow dataRow = this.dtInAddrAux.NewRow();
					dataRow["Key"] = 0;
					dataRow["Value"] = ShowMsgInfos.GetInfo("Any", "任意");
					this.dtInAddrAux.Rows.Add(dataRow.ItemArray);
					this.dtInAddrDoor.Rows.Add(dataRow.ItemArray);
					AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
					List<AccAuxiliary> modelList = accAuxiliaryBll.GetModelList("device_id=" + machines.ID);
					int num;
					if (modelList != null)
					{
						foreach (AccAuxiliary item in modelList)
						{
							dataRow = this.dtInAddrAux.NewRow();
							DataRow dataRow2 = dataRow;
							num = item.AuxNo;
							dataRow2["Key"] = num.ToString();
							dataRow["Value"] = item.AuxName;
							if (item.AuxState == AccAuxiliary.AccAuxiliaryState.In)
							{
								this.dtInAddrAux.Rows.Add(dataRow.ItemArray);
							}
							else
							{
								this.dtOutAddrAux.Rows.Add(dataRow.ItemArray);
							}
						}
					}
					AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
					List<AccDoor> modelList2 = accDoorBll.GetModelList("device_id=" + machines.ID);
					if (modelList2 != null)
					{
						foreach (AccDoor item2 in modelList2)
						{
							dataRow = this.dtInAddrDoor.NewRow();
							DataRow dataRow3 = dataRow;
							num = item2.door_no;
							dataRow3["Key"] = num.ToString();
							dataRow["Value"] = item2.door_name;
							this.dtInAddrDoor.Rows.Add(dataRow.ItemArray);
							this.dtOutAddrDoor.Rows.Add(dataRow.ItemArray);
						}
					}
				}
			}
		}

		private void InitOutAddrType()
		{
			this.m_outAddrTypeDic = initLang.GetComboxInfo("OutAddrType");
			if (this.m_outAddrTypeDic == null || this.m_outAddrTypeDic.Count == 0)
			{
				this.m_inAddrTypeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("1", "门锁1");
				dictionary.Add("2", "门锁2");
				dictionary.Add("3", "门锁3");
				dictionary.Add("4", "门锁4");
				this.m_outAddrTypeDic.Add("0", dictionary);
				dictionary = new Dictionary<string, string>();
				dictionary.Add("1", "辅助输出1");
				dictionary.Add("2", "辅助输出2");
				dictionary.Add("3", "辅助输出3");
				dictionary.Add("4", "辅助输出4");
				dictionary.Add("5", "辅助输出5");
				dictionary.Add("6", "辅助输出6");
				this.m_outAddrTypeDic.Add("1", dictionary);
				initLang.SetComboxInfo("OutAddrType", this.m_outAddrTypeDic);
				initLang.Save();
			}
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				this.cmb_dev.Items.Clear();
				this.indexidlist.Clear();
				this.cmb_dev.Items.Add("-----");
				this.indexidlist.Add(0, 0);
				List<Machines> list = null;
				list = this.mbll.GetModelList("DevSDKType <> 2 and device_type < 101");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].DevSDKType != SDKType.StandaloneSDK && list[i].door_count != 0 && !this.mlist.ContainsKey(list[i].ID))
						{
							this.mlist.Add(list[i].ID, list[i]);
							this.indexidlist.Add(this.cmb_dev.Items.Count, list[i].ID);
							this.cmb_dev.Items.Add(list[i].MachineAlias);
						}
					}
				}
				if (this.cmb_dev.Items.Count > 0)
				{
					this.cmb_dev.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void loadEventType()
		{
			try
			{
				Dictionary<int, string> dic = LinkAgeIOInfo.GetDic();
				IOrderedEnumerable<KeyValuePair<int, string>> orderedEnumerable = from kv in dic
				orderby kv.Value
				select kv;
				this.eventTypeList.Clear();
				foreach (KeyValuePair<int, string> item in orderedEnumerable)
				{
					this.eventTypeList.Add(item.Key, item.Value);
				}
				this.eventTypeListex.Clear();
				this.cmb_type.Items.Clear();
				this.eventTypeIndexList.Clear();
				if (this.cmb_dev.SelectedIndex > 0 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex) && this.indexidlist[this.cmb_dev.SelectedIndex] > 0 && this.mlist.ContainsKey(this.indexidlist[this.cmb_dev.SelectedIndex]))
				{
					foreach (KeyValuePair<int, string> eventType in this.eventTypeList)
					{
						if (eventType.Key != -1)
						{
							if (this.mlist[this.indexidlist[this.cmb_dev.SelectedIndex]].acpanel_type == 1)
							{
								if ((this.mlist[this.indexidlist[this.cmb_dev.SelectedIndex]].aux_in_count != 0 || (eventType.Key != 220 && eventType.Key != 221)) && eventType.Key != 25 && eventType.Key < 100000)
								{
									this.eventTypeListex.Add(eventType.Key);
									this.cmb_type.Items.Add(eventType.Value);
									this.eventTypeIndexList.Add(eventType.Key, this.cmb_type.Items.Count - 1);
								}
							}
							else if (eventType.Key < 100000)
							{
								this.eventTypeListex.Add(eventType.Key);
								this.cmb_type.Items.Add(eventType.Value);
								this.eventTypeIndexList.Add(eventType.Key, this.cmb_type.Items.Count - 1);
							}
						}
					}
					if (this.cmb_type.Items.Count > 0)
					{
						this.cmb_type.SelectedIndex = 0;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadInAddr()
		{
			try
			{
				this.InitInOutAddrType();
				if (this.cmb_dev.SelectedIndex > 0 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex) && this.indexidlist[this.cmb_dev.SelectedIndex] > 0 && this.mlist.ContainsKey(this.indexidlist[this.cmb_dev.SelectedIndex]))
				{
					if (this.cmb_type.SelectedIndex >= 0 && (this.eventTypeListex[this.cmb_type.SelectedIndex] == 220 || this.eventTypeListex[this.cmb_type.SelectedIndex] == 221))
					{
						this.cmb_inAddr.DisplayMember = "Value";
						this.cmb_inAddr.ValueMember = "Key";
						this.cmb_inAddr.DataSource = this.dtInAddrAux;
					}
					else
					{
						this.cmb_inAddr.DisplayMember = "Value";
						this.cmb_inAddr.ValueMember = "Key";
						this.cmb_inAddr.DataSource = this.dtInAddrDoor;
					}
					if (this.cmb_inAddr.Items.Count > 0)
					{
						this.cmb_inAddr.SelectedIndex = 0;
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
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccLinkAgeIo accLinkAgeIo = null;
					AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
					accLinkAgeIo = accLinkAgeIoBll.GetModel(this.m_id);
					if (accLinkAgeIo != null)
					{
						if (this.mlist.ContainsKey(accLinkAgeIo.device_id))
						{
							this.cmb_dev.Text = this.mlist[accLinkAgeIo.device_id].MachineAlias;
						}
						else
						{
							Machines model = this.mbll.GetModel(accLinkAgeIo.device_id);
							if (model != null)
							{
								this.mlist.Add(model.ID, model);
								this.cmb_dev.Items.Add(model.MachineAlias);
								this.indexidlist.Add(this.cmb_dev.Items.Count - 1, model.ID);
								this.cmb_dev.Text = model.MachineAlias;
							}
							else
							{
								this.cmb_dev.Items.Add("-----");
								this.cmb_dev.SelectedIndex = 0;
								this.indexidlist.Add(this.cmb_dev.Items.Count - 1, 0);
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDataEdit", "没有编辑的数据"));
							}
						}
						if (this.eventTypeIndexList.ContainsKey(accLinkAgeIo.trigger_opt))
						{
							this.cmb_type.SelectedIndex = this.eventTypeIndexList[accLinkAgeIo.trigger_opt];
						}
						this.txt_name.Text = accLinkAgeIo.linkage_name;
						this.cmb_inAddr.SelectedValue = accLinkAgeIo.in_address;
						if (accLinkAgeIo.out_type_hide < this.cmb_outtype.Items.Count)
						{
							this.cmb_outtype.SelectedIndex = accLinkAgeIo.out_type_hide;
						}
						this.cmb_outtype_SelectedIndexChanged(null, null);
						this.cmb_outaddr.SelectedValue = accLinkAgeIo.out_address;
						if (this.cmb_actionType.Items.Count > accLinkAgeIo.action_type)
						{
							this.cmb_actionType.SelectedIndex = accLinkAgeIo.action_type;
						}
						if (this.txt_dalay.Enabled)
						{
							this.txt_dalay.Text = accLinkAgeIo.delay_time.ToString();
						}
						else
						{
							this.txt_dalay.Text = "";
						}
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

		private bool BindModel(AccLinkAgeIo link)
		{
			try
			{
				if (this.check())
				{
					link.linkage_name = this.txt_name.Text;
					link.device_id = this.indexidlist[this.cmb_dev.SelectedIndex];
					link.trigger_opt = this.eventTypeListex[this.cmb_type.SelectedIndex];
					link.in_address = int.Parse(this.cmb_inAddr.SelectedValue.ToString());
					link.out_type_hide = this.cmb_outtype.SelectedIndex;
					link.out_address = int.Parse(this.cmb_outaddr.SelectedValue.ToString());
					link.action_type = this.cmb_actionType.SelectedIndex;
					if (link.action_type == 0)
					{
						link.delay_time = 0;
					}
					else if (link.action_type == 2)
					{
						link.delay_time = 255;
					}
					else if (link.action_type == 1)
					{
						if (!string.IsNullOrEmpty(this.txt_dalay.Text))
						{
							link.delay_time = int.Parse(this.txt_dalay.Text);
						}
						else
						{
							link.delay_time = 20;
						}
					}
					else if (AccCommon.CodeVersion == CodeVersionType.JapanAF && link.action_type == 3)
					{
						link.delay_time = 0;
					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool check()
		{
			try
			{
				if (!string.IsNullOrEmpty(this.txt_name.Text))
				{
					if (this.cmb_dev.SelectedIndex >= 0 && !string.IsNullOrEmpty(this.cmb_dev.Text) && this.cmb_dev.Text.IndexOf("----") == -1 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex))
					{
						if (this.cmb_type.SelectedIndex >= 0)
						{
							if (this.cmb_outaddr.SelectedIndex >= 0)
							{
								if (this.cmb_outtype.SelectedIndex >= 0)
								{
									if (this.cmb_inAddr.SelectedIndex >= 0)
									{
										if (this.txt_dalay.Enabled && string.IsNullOrEmpty(this.txt_dalay.Text))
										{
											SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputDelayTime", "请输入输出延迟时间"));
											this.txt_dalay.Focus();
											return false;
										}
										return true;
									}
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectInAddress", "请选择输入点"));
									this.cmb_inAddr.Focus();
									return false;
								}
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOutputType", "请选择输出点类型"));
								this.cmb_outtype.Focus();
								return false;
							}
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOutputAddress", "请选择输出点"));
							this.cmb_outaddr.Focus();
							return false;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOutputType", "请选择输入点类型"));
						this.cmb_type.Focus();
						return false;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDevice", "请选择设备"));
					this.cmb_dev.Focus();
					return false;
				}
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
				this.txt_name.Focus();
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool LinkRepeatCheck(AccLinkAgeIo model)
		{
			try
			{
				AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
				List<AccLinkAgeIo> modelList = accLinkAgeIoBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].id != this.m_id)
						{
							if (modelList[i].in_address == 0)
							{
								if (modelList[i].device_id == model.device_id && modelList[i].trigger_opt == model.trigger_opt && model.in_address >= 0)
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SLinkExistSomeInAddress", "系统中已存在该设备在该触发条件下输入点为任意的联动设置"));
									this.cmb_inAddr.Focus();
									return false;
								}
							}
							else if (modelList[i].device_id == model.device_id && modelList[i].trigger_opt == model.trigger_opt && model.in_address == 0)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SLinkExistAnyInAddress", "系统中已存在该设备在该触发条件下输入点不为任意的联动设置"));
								this.cmb_inAddr.Focus();
								return false;
							}
							if (modelList[i].device_id == model.device_id && modelList[i].trigger_opt == model.trigger_opt && modelList[i].in_address == model.in_address && modelList[i].out_type_hide == model.out_type_hide && modelList[i].out_address == model.out_address)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SLinkInOutAddressRepeat", "系统中已存在该设备在该触发条件下输入点和输出点都相同的联动设置"));
								this.cmb_outaddr.Focus();
								return false;
							}
						}
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

		private bool Save()
		{
			try
			{
				AccLinkAgeIo accLinkAgeIo = new AccLinkAgeIo();
				AccLinkAgeIo accLinkAgeIo2 = null;
				AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accLinkAgeIo2 = accLinkAgeIoBll.GetModel(this.m_id);
					if (accLinkAgeIo2 != null)
					{
						accLinkAgeIo.linkage_name = accLinkAgeIo2.linkage_name;
						accLinkAgeIo.device_id = accLinkAgeIo2.device_id;
						accLinkAgeIo.trigger_opt = accLinkAgeIo2.trigger_opt;
						accLinkAgeIo.in_address = accLinkAgeIo2.in_address;
						accLinkAgeIo.out_type_hide = accLinkAgeIo2.out_type_hide;
						accLinkAgeIo.out_address = accLinkAgeIo2.out_address;
						accLinkAgeIo.delay_time = accLinkAgeIo2.delay_time;
						accLinkAgeIo.action_type = accLinkAgeIo2.action_type;
						accLinkAgeIo.id = accLinkAgeIo2.id;
					}
				}
				if (accLinkAgeIo2 == null)
				{
					accLinkAgeIo2 = new AccLinkAgeIo();
					if (accLinkAgeIoBll.Exists(this.txt_name.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.txt_name.Focus();
						return false;
					}
				}
				else if (accLinkAgeIo2.linkage_name != this.txt_name.Text && accLinkAgeIoBll.Exists(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
					this.txt_name.Focus();
					return false;
				}
				if (this.BindModel(accLinkAgeIo2))
				{
					if (!this.LinkRepeatCheck(accLinkAgeIo2))
					{
						return false;
					}
					if (this.m_id > 0)
					{
						if (accLinkAgeIo.linkage_name == accLinkAgeIo2.linkage_name && accLinkAgeIo.action_type == accLinkAgeIo2.action_type && accLinkAgeIo.device_id == accLinkAgeIo2.device_id && accLinkAgeIo.trigger_opt == accLinkAgeIo2.trigger_opt && accLinkAgeIo.in_address == accLinkAgeIo2.in_address && accLinkAgeIo.out_type_hide == accLinkAgeIo2.out_type_hide && accLinkAgeIo.out_address == accLinkAgeIo2.out_address && accLinkAgeIo.delay_time == accLinkAgeIo2.delay_time)
						{
							return true;
						}
						if (accLinkAgeIoBll.Update(accLinkAgeIo2))
						{
							if (accLinkAgeIo.action_type != accLinkAgeIo2.action_type || accLinkAgeIo.device_id != accLinkAgeIo2.device_id || accLinkAgeIo.trigger_opt != accLinkAgeIo2.trigger_opt || accLinkAgeIo.in_address != accLinkAgeIo2.in_address || accLinkAgeIo.out_type_hide != accLinkAgeIo2.out_type_hide || accLinkAgeIo.out_address != accLinkAgeIo2.out_address || accLinkAgeIo.delay_time != accLinkAgeIo2.delay_time)
							{
								CommandServer.DelCmd(accLinkAgeIo);
								CommandServer.AddCmd(accLinkAgeIo2);
								FrmShowUpdata.Instance.ShowEx();
							}
							try
							{
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
							}
							catch
							{
							}
							return true;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
					else
					{
						try
						{
							if (accLinkAgeIoBll.Add(accLinkAgeIo2) > 0)
							{
								CommandServer.AddCmd(accLinkAgeIo2);
								FrmShowUpdata.Instance.ShowEx();
								try
								{
									if (this.RefreshDataEvent != null)
									{
										this.RefreshDataEvent(this, null);
									}
								}
								catch
								{
								}
								return true;
							}
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

		private void btn_saveAndContinue_Click(object sender, EventArgs e)
		{
			if (this.check())
			{
				this.Save();
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void cmb_type_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.LoadInAddr();
		}

		private void cmb_outtype_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.InitInOutAddrType();
				if (this.cmb_dev.SelectedIndex > 0 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex) && this.indexidlist[this.cmb_dev.SelectedIndex] > 0 && this.mlist.ContainsKey(this.indexidlist[this.cmb_dev.SelectedIndex]))
				{
					if (this.cmb_outtype.SelectedIndex == 1)
					{
						this.cmb_outaddr.ValueMember = "Key";
						this.cmb_outaddr.DisplayMember = "Value";
						this.cmb_outaddr.DataSource = this.dtOutAddrAux;
					}
					else
					{
						this.cmb_outaddr.ValueMember = "Key";
						this.cmb_outaddr.DisplayMember = "Value";
						this.cmb_outaddr.DataSource = this.dtOutAddrDoor;
					}
					if (this.cmb_outaddr.Items.Count > 0)
					{
						this.cmb_outaddr.SelectedIndex = 0;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_dalay_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 254L);
		}

		private void com_actionType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_actionType.SelectedIndex == 1)
			{
				this.lb_delay.Enabled = true;
				this.txt_dalay.Enabled = true;
				this.lb_second.Enabled = true;
				if (string.IsNullOrEmpty(this.txt_dalay.Text) || this.txt_dalay.Text == "0")
				{
					this.txt_dalay.Text = "10";
				}
			}
			else
			{
				this.txt_dalay.Enabled = false;
				this.lb_delay.Enabled = false;
				this.lb_second.Enabled = false;
				this.txt_dalay.Text = "";
			}
		}

		private void EnableSet(bool isEnable)
		{
			this.cmb_type.Enabled = isEnable;
			this.lb_event.Enabled = isEnable;
			this.cmb_inAddr.Enabled = isEnable;
			this.lb_inAddr.Enabled = isEnable;
			this.cmb_outtype.Enabled = isEnable;
			this.lb_outType.Enabled = isEnable;
			this.cmb_outaddr.Enabled = isEnable;
			this.lb_outAddr.Enabled = isEnable;
			this.cmb_actionType.Enabled = isEnable;
			this.lb_actionType.Enabled = isEnable;
			this.btn_OK.Enabled = isEnable;
		}

		private void cmb_dev_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.cmb_dev.SelectedIndex > 0 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex) && this.indexidlist[this.cmb_dev.SelectedIndex] > 0)
				{
					this.loadEventType();
					this.LoadInAddr();
					this.InitOutType(this.mlist[this.indexidlist[this.cmb_dev.SelectedIndex]].device_type);
					this.EnableSet(true);
				}
				else
				{
					this.EnableSet(false);
				}
				initLang.ComboBoxAutoSize(this.cmb_type, this);
				initLang.ComboBoxAutoSize(this.cmb_inAddr, this);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LinkageEdit));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_event = new Label();
			this.cmb_type = new ComboBox();
			this.lb_dev = new Label();
			this.cmb_dev = new ComboBox();
			this.lb_name = new Label();
			this.txt_name = new TextBox();
			this.lb_inAddr = new Label();
			this.cmb_inAddr = new ComboBox();
			this.lb_outAddr = new Label();
			this.cmb_outaddr = new ComboBox();
			this.lb_outType = new Label();
			this.cmb_outtype = new ComboBox();
			this.txt_dalay = new TextBox();
			this.lb_delay = new Label();
			this.lb_second = new Label();
			this.lb_actionType = new Label();
			this.cmb_actionType = new ComboBox();
			this.label2 = new Label();
			this.label1 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.label5 = new Label();
			this.label6 = new Label();
			this.label7 = new Label();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(201, 293);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 9;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(99, 293);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 8;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_event.BackColor = Color.Transparent;
			this.lb_event.Location = new Point(20, 89);
			this.lb_event.Name = "lb_event";
			this.lb_event.Size = new Size(98, 12);
			this.lb_event.TabIndex = 33;
			this.lb_event.Text = "触发条件";
			this.lb_event.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_type.FormattingEnabled = true;
			this.cmb_type.Location = new Point(181, 85);
			this.cmb_type.Name = "cmb_type";
			this.cmb_type.Size = new Size(142, 20);
			this.cmb_type.TabIndex = 2;
			this.cmb_type.SelectedIndexChanged += this.cmb_type_SelectedIndexChanged;
			this.lb_dev.BackColor = Color.Transparent;
			this.lb_dev.Location = new Point(20, 56);
			this.lb_dev.Name = "lb_dev";
			this.lb_dev.Size = new Size(98, 12);
			this.lb_dev.TabIndex = 31;
			this.lb_dev.Text = "设备";
			this.lb_dev.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_dev.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_dev.FormattingEnabled = true;
			this.cmb_dev.Location = new Point(181, 52);
			this.cmb_dev.Name = "cmb_dev";
			this.cmb_dev.Size = new Size(142, 20);
			this.cmb_dev.TabIndex = 1;
			this.cmb_dev.SelectedIndexChanged += this.cmb_dev_SelectedIndexChanged;
			this.lb_name.BackColor = Color.Transparent;
			this.lb_name.Location = new Point(20, 23);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(98, 12);
			this.lb_name.TabIndex = 37;
			this.lb_name.Text = "联动名称";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_name.Location = new Point(181, 19);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(142, 21);
			this.txt_name.TabIndex = 0;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.lb_inAddr.BackColor = Color.Transparent;
			this.lb_inAddr.Location = new Point(20, 122);
			this.lb_inAddr.Name = "lb_inAddr";
			this.lb_inAddr.Size = new Size(98, 12);
			this.lb_inAddr.TabIndex = 40;
			this.lb_inAddr.Text = "输入点地址";
			this.lb_inAddr.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_inAddr.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_inAddr.FormattingEnabled = true;
			this.cmb_inAddr.Location = new Point(181, 118);
			this.cmb_inAddr.Name = "cmb_inAddr";
			this.cmb_inAddr.Size = new Size(142, 20);
			this.cmb_inAddr.TabIndex = 3;
			this.lb_outAddr.BackColor = Color.Transparent;
			this.lb_outAddr.Location = new Point(20, 188);
			this.lb_outAddr.Name = "lb_outAddr";
			this.lb_outAddr.Size = new Size(98, 12);
			this.lb_outAddr.TabIndex = 42;
			this.lb_outAddr.Text = "输出点地址";
			this.lb_outAddr.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_outaddr.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_outaddr.FormattingEnabled = true;
			this.cmb_outaddr.Location = new Point(181, 183);
			this.cmb_outaddr.Name = "cmb_outaddr";
			this.cmb_outaddr.Size = new Size(142, 20);
			this.cmb_outaddr.TabIndex = 5;
			this.lb_outType.BackColor = Color.Transparent;
			this.lb_outType.Location = new Point(20, 155);
			this.lb_outType.Name = "lb_outType";
			this.lb_outType.Size = new Size(98, 12);
			this.lb_outType.TabIndex = 44;
			this.lb_outType.Text = "输出点类型";
			this.lb_outType.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_outtype.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_outtype.FormattingEnabled = true;
			this.cmb_outtype.Items.AddRange(new object[2]
			{
				"门锁",
				"辅助输出"
			});
			this.cmb_outtype.Location = new Point(181, 151);
			this.cmb_outtype.Name = "cmb_outtype";
			this.cmb_outtype.Size = new Size(142, 20);
			this.cmb_outtype.TabIndex = 4;
			this.cmb_outtype.SelectedIndexChanged += this.cmb_outtype_SelectedIndexChanged;
			this.txt_dalay.Enabled = false;
			this.txt_dalay.Location = new Point(181, 250);
			this.txt_dalay.Name = "txt_dalay";
			this.txt_dalay.Size = new Size(65, 21);
			this.txt_dalay.TabIndex = 7;
			this.txt_dalay.KeyPress += this.txt_dalay_KeyPress;
			this.lb_delay.BackColor = Color.Transparent;
			this.lb_delay.Location = new Point(20, 254);
			this.lb_delay.Name = "lb_delay";
			this.lb_delay.Size = new Size(98, 12);
			this.lb_delay.TabIndex = 46;
			this.lb_delay.Text = "输出延时";
			this.lb_delay.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_second.BackColor = Color.Transparent;
			this.lb_second.Location = new Point(253, 254);
			this.lb_second.Name = "lb_second";
			this.lb_second.Size = new Size(79, 12);
			this.lb_second.TabIndex = 47;
			this.lb_second.Text = "秒(1-254)";
			this.lb_actionType.BackColor = Color.Transparent;
			this.lb_actionType.Location = new Point(20, 221);
			this.lb_actionType.Name = "lb_actionType";
			this.lb_actionType.Size = new Size(98, 12);
			this.lb_actionType.TabIndex = 48;
			this.lb_actionType.Text = "动作类型";
			this.lb_actionType.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_actionType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_actionType.FormattingEnabled = true;
			this.cmb_actionType.Items.AddRange(new object[3]
			{
				"关闭",
				"打开",
				"常开"
			});
			this.cmb_actionType.Location = new Point(181, 216);
			this.cmb_actionType.Name = "cmb_actionType";
			this.cmb_actionType.Size = new Size(142, 20);
			this.cmb_actionType.TabIndex = 6;
			this.cmb_actionType.SelectedIndexChanged += this.com_actionType_SelectedIndexChanged;
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(329, 22);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 50;
			this.label2.Text = "*";
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(329, 55);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 51;
			this.label1.Text = "*";
			this.label3.AutoSize = true;
			this.label3.BackColor = Color.Transparent;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(329, 88);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 52;
			this.label3.Text = "*";
			this.label4.AutoSize = true;
			this.label4.BackColor = Color.Transparent;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(329, 121);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 53;
			this.label4.Text = "*";
			this.label5.AutoSize = true;
			this.label5.BackColor = Color.Transparent;
			this.label5.ForeColor = Color.Red;
			this.label5.Location = new Point(329, 154);
			this.label5.Name = "label5";
			this.label5.Size = new Size(11, 12);
			this.label5.TabIndex = 54;
			this.label5.Text = "*";
			this.label6.AutoSize = true;
			this.label6.BackColor = Color.Transparent;
			this.label6.ForeColor = Color.Red;
			this.label6.Location = new Point(329, 186);
			this.label6.Name = "label6";
			this.label6.Size = new Size(11, 12);
			this.label6.TabIndex = 55;
			this.label6.Text = "*";
			this.label7.AutoSize = true;
			this.label7.BackColor = Color.Transparent;
			this.label7.ForeColor = Color.Red;
			this.label7.Location = new Point(329, 219);
			this.label7.Name = "label7";
			this.label7.Size = new Size(11, 12);
			this.label7.TabIndex = 56;
			this.label7.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(356, 328);
			base.Controls.Add(this.txt_dalay);
			base.Controls.Add(this.cmb_actionType);
			base.Controls.Add(this.cmb_outaddr);
			base.Controls.Add(this.cmb_outtype);
			base.Controls.Add(this.cmb_inAddr);
			base.Controls.Add(this.cmb_type);
			base.Controls.Add(this.cmb_dev);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.lb_actionType);
			base.Controls.Add(this.lb_second);
			base.Controls.Add(this.lb_delay);
			base.Controls.Add(this.lb_outType);
			base.Controls.Add(this.lb_outAddr);
			base.Controls.Add(this.lb_inAddr);
			base.Controls.Add(this.lb_name);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_event);
			base.Controls.Add(this.lb_dev);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LinkageEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
