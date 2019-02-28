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
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class MapUC : UserControl
	{
		private bool _isEditing;

		private LinkLabel lnkMode;

		private Dictionary<int, AccMap> maps = new Dictionary<int, AccMap>();

		private Dictionary<int, MapControl> mapControls = new Dictionary<int, MapControl>();

		private Timer m_timerAdddoor = new Timer();

		private IContainer components = null;

		public PanelEx pnl_doorStatus;

		private DevComponents.DotNetBar.TabControl tab_map;

		private TabControlPanel tabpnl_first;

		private MapControl map_first;

		private TabItem tabItem_first;

		public PanelEx MenuPanelEx;

		private ToolStrip toolStrip1;

		private ToolStripButton btn_addMap;

		private ToolStripButton btn_editMap;

		private ToolStripButton btn_delMap;

		private ToolStripButton btn_addDoor;

		private ToolStripButton btn_delDoor;

		private ToolStripButton btn_saveInfo;

		private ToolStripButton btn_openListen;

		private bool IsEditing
		{
			get
			{
				return this._isEditing;
			}
			set
			{
				if (value)
				{
					this.BeginEditMode();
					this.lnkMode.Text = ShowMsgInfos.GetInfo("Back", "返回");
				}
				else
				{
					this.EndEditMode();
					this.lnkMode.Text = ShowMsgInfos.GetInfo("ToEdit", "编辑");
				}
				this._isEditing = value;
			}
		}

		public MapUC()
		{
			this.InitializeComponent();
			try
			{
				DevLogServer.ReConnect();
				this.CheckDoor();
				initLang.LocaleForm(this, base.Name);
				this.tabItem_first.Text = ShowMsgInfos.GetInfo("SNoLoadMap", "未添加地图");
				this.InitMaps();
				if (this.map_first != null)
				{
					this.map_first.Load();
				}
				this.SetListenBtn();
			}
			catch (Exception)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
			this.CheckPermission();
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				this.lnkMode = new LinkLabel();
				this.lnkMode.AutoSize = true;
				this.lnkMode.Name = "lnkMode";
				int x = 3;
				int num = (this.pnl_doorStatus.Height - this.lnkMode.Height) / 2;
				this.lnkMode.Location = new Point(x, (num > 0) ? num : 5);
				this.lnkMode.Click += this.lnkMode_Click;
				this.pnl_doorStatus.Controls.Add(this.lnkMode);
				this.IsEditing = false;
				this.toolStrip1.Items.Remove(this.btn_delMap);
				this.toolStrip1.Items.Insert(1, this.btn_delMap);
			}
		}

		private void lnkMode_Click(object sender, EventArgs e)
		{
			this.IsEditing = !this.IsEditing;
		}

		private void CheckDoor()
		{
			try
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					this.btn_openListen.Enabled = true;
					this.btn_openListen.Tag = "1";
				}
				else
				{
					this.btn_openListen.Tag = "0";
					this.btn_openListen.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Monitoring))
			{
				this.btn_openListen.Enabled = false;
				this.btn_addDoor.Enabled = false;
				this.btn_addMap.Enabled = false;
				this.btn_delDoor.Enabled = false;
				this.btn_delMap.Enabled = false;
				this.btn_editMap.Enabled = false;
				this.btn_saveInfo.Enabled = false;
			}
		}

		private void SetListenBtn()
		{
			try
			{
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("IsListen");
				bool flag = true;
				if (string.IsNullOrEmpty(nodeValueByName))
				{
					AppSite.Instance.SetNodeValue("IsListen", "true");
				}
				else
				{
					flag = (nodeValueByName == "true" && true);
				}
				if (flag)
				{
					this.btn_openListen.Image = Resources.disableMonitoring;
					this.btn_openListen.Text = ShowMsgInfos.GetInfo("StopListen", "停止监控");
					RTEventProcessor.LoadDictionary(true);
					MonitorWatchdog.InitialMonitors();
					MonitorWatchdog.StartWatchdog();
				}
				else
				{
					this.btn_openListen.Image = Resources.Monitoring;
					this.btn_openListen.Text = ShowMsgInfos.GetInfo("OpenListen", "启动监控");
				}
				this.CheckPermission();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitMaps()
		{
			try
			{
				AccMapBll accMapBll = new AccMapBll(MainForm._ia);
				List<AccMap> list = accMapBll.GetModelList("");
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					list = this.SortTabSequence(list);
				}
				if (list != null && list.Count > 0)
				{
					this.tab_map.Tabs.Clear();
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.maps.ContainsKey(list[i].id))
						{
							this.maps.Add(list[i].id, list[i]);
							this.InitMap(list[i]);
						}
					}
				}
				else
				{
					this.SetBtnEnabled(false);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetBtnEnabled(bool isEnable)
		{
			this.btn_delMap.Enabled = isEnable;
			this.btn_editMap.Enabled = isEnable;
			this.btn_addDoor.Enabled = isEnable;
			this.btn_saveInfo.Enabled = isEnable;
			this.btn_delDoor.Enabled = isEnable;
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				this.btn_delMap.Enabled = (isEnable && !this.IsEditing);
				this.btn_editMap.Enabled = (isEnable && !this.IsEditing);
				this.btn_addDoor.Enabled = (isEnable && this.IsEditing);
				this.btn_delDoor.Enabled = (isEnable && this.IsEditing);
				this.btn_saveInfo.Enabled = (isEnable && this.IsEditing);
			}
			this.CheckPermission();
		}

		private void InitMap(AccMap map)
		{
			try
			{
				if (!this.mapControls.ContainsKey(map.id))
				{
					TabItem tabItem = new TabItem(this.components);
					this.tab_map.Tabs.Insert(0, tabItem);
					tabItem.Tag = map.id;
					tabItem.Text = map.map_name;
					tabItem.Tooltip = map.map_name;
					TabControlPanel tabControlPanel = new TabControlPanel();
					this.tab_map.Controls.Add(tabControlPanel);
					tabItem.AttachedControl = tabControlPanel;
					tabControlPanel.Dock = DockStyle.Fill;
					MapControl mapControl = new MapControl();
					mapControl.Load();
					mapControl.Dock = DockStyle.Fill;
					tabControlPanel.Controls.Add(mapControl);
					mapControl.DevTabItem = tabItem;
					mapControl.InitMap(map.id);
					this.SetMapBack(mapControl, map.map_path);
					this.tab_map.SelectedTab = tabItem;
					this.mapControls.Add(map.id, mapControl);
					this.SetBtnEnabled(true);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetMapBack(MapControl map, string fileName)
		{
			if (File.Exists(fileName))
			{
				try
				{
					Image image = Image.FromFile(fileName);
					int top = map.Map.Height;
					int left = map.Map.Width;
					bool flag = false;
					if (image.Height > map.Map.Height)
					{
						top = image.Height + 20 + 2 * map.Map.Top;
						flag = true;
					}
					if (image.Width > map.Map.Width)
					{
						left = image.Width + 20 + 2 * map.Map.Left;
						flag = true;
					}
					if (flag)
					{
						Label label = new Label();
						label.Text = ".";
						map.Map.Controls.Add(label);
						label.Left = left;
						label.Top = top;
						label.AutoSize = false;
						label.Width = 5;
					}
					map.Map.BackImg = image;
					image.Dispose();
					image = null;
				}
				catch
				{
				}
			}
		}

		private void btn_addMap_Click(object sender, EventArgs e)
		{
			MapEdit mapEdit = new MapEdit(0);
			mapEdit.RefreshDataEvent += this.mapEdit_RefreshDataEvent;
			mapEdit.Text = this.btn_addMap.Text;
			mapEdit.ShowDialog();
			mapEdit.RefreshDataEvent -= this.mapEdit_RefreshDataEvent;
		}

		private void mapEdit_RefreshDataEvent(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					try
					{
						AccMap accMap = sender as AccMap;
						if (accMap != null)
						{
							if (!this.maps.ContainsKey(accMap.id))
							{
								this.maps.Add(accMap.id, accMap);
								if (this.tab_map.Tabs.Count == 1 && this.tab_map.Tabs[0].Tag == null)
								{
									this.tab_map.Tabs.Clear();
								}
								this.InitMap(accMap);
								this.SetBtnEnabled(true);
							}
							else
							{
								int num = 0;
								while (true)
								{
									if (num < this.tab_map.Tabs.Count)
									{
										if (this.tab_map.Tabs[num].Tag == null || !(this.tab_map.Tabs[num].Tag.ToString() == accMap.id.ToString()))
										{
											num++;
											continue;
										}
										break;
									}
									return;
								}
								this.tab_map.Tabs[num].Text = accMap.map_name;
								this.tab_map.Tabs[num].Tooltip = accMap.map_name;
								if (this.mapControls.ContainsKey(accMap.id))
								{
									this.SetMapBack(this.mapControls[accMap.id], accMap.map_path);
								}
								this.SetBtnEnabled(true);
							}
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_editMap_Click(object sender, EventArgs e)
		{
			if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
			{
				MapEdit mapEdit = new MapEdit(int.Parse(this.tab_map.SelectedTab.Tag.ToString()));
				mapEdit.RefreshDataEvent += this.mapEdit_RefreshDataEvent;
				mapEdit.Text = this.btn_editMap.Text;
				mapEdit.ShowDialog();
				mapEdit.RefreshDataEvent -= this.mapEdit_RefreshDataEvent;
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
			}
		}

		private void btn_delMap_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的对象?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						AccMapBll accMapBll = new AccMapBll(MainForm._ia);
						int num = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
						accMapBll.Delete(num);
						if (this.tab_map.SelectedTab.AttachedControl != null)
						{
							this.tab_map.Controls.Remove(this.tab_map.SelectedTab.AttachedControl);
						}
						if (this.mapControls.ContainsKey(num))
						{
							this.mapControls[num].Dispose();
							this.mapControls.Remove(num);
						}
						if (this.maps.ContainsKey(num))
						{
							this.maps.Remove(num);
						}
						this.tab_map.Tabs.Remove(this.tab_map.SelectedTab);
						if (this.tab_map.Tabs.Count == 0)
						{
							TabItem tabItem = new TabItem(this.components);
							this.tab_map.Tabs.Insert(0, tabItem);
							tabItem.Text = ShowMsgInfos.GetInfo("SNoLoadMap", "未添加地图");
							TabControlPanel tabControlPanel = new TabControlPanel();
							this.tab_map.Controls.Add(tabControlPanel);
							tabItem.AttachedControl = tabControlPanel;
							tabControlPanel.Dock = DockStyle.Fill;
							MapControl mapControl = new MapControl();
							mapControl.Load();
							mapControl.Dock = DockStyle.Fill;
							tabControlPanel.Controls.Add(mapControl);
							this.tab_map.SelectedTab = tabItem;
							this.SetBtnEnabled(false);
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_addDoor_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
				{
					int key = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
					List<int> list = new List<int>();
					if (this.mapControls.ContainsKey(key))
					{
						List<DevControl> doorControls = this.mapControls[key].Map.DoorControls;
						for (int i = 0; i < doorControls.Count; i++)
						{
							if (doorControls[i].Visible)
							{
								list.Add(doorControls[i].AccDoorInfo.id);
							}
						}
					}
					DeviceTreeForm deviceTreeForm = new DeviceTreeForm(list);
					deviceTreeForm.SelectDeviceEvent += this.doorSelect_SelectDeviceEvent;
					deviceTreeForm.Text = this.btn_addDoor.Text;
					deviceTreeForm.ShowDialog();
					deviceTreeForm.SelectDeviceEvent -= this.doorSelect_SelectDeviceEvent;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void doorSelect_SelectDeviceEvent(object sender, EventArgs e)
		{
			try
			{
				if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null && sender != null)
				{
					List<AccDoor> list = sender as List<AccDoor>;
					if (list != null && list.Count > 0)
					{
						int num = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
						if (this.mapControls.ContainsKey(num))
						{
							for (int i = 0; i < list.Count; i++)
							{
								this.mapControls[num].Map.AddDoor(list[i]);
							}
							this.m_timerAdddoor.Tick -= this.m_timerAdddoor_Tick;
							this.m_timerAdddoor.Tick += this.m_timerAdddoor_Tick;
							this.m_timerAdddoor.Tag = num;
							this.m_timerAdddoor.Enabled = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void m_timerAdddoor_Tick(object sender, EventArgs e)
		{
			this.m_timerAdddoor.Enabled = false;
			this.Cursor = Cursors.WaitCursor;
			if (this.m_timerAdddoor.Tag != null)
			{
				try
				{
					int num = int.Parse(this.m_timerAdddoor.Tag.ToString());
					if (this.mapControls.ContainsKey(num))
					{
						List<DevControl> doorControls = this.mapControls[num].Map.DoorControls;
						for (int i = 0; i < doorControls.Count; i++)
						{
							doorControls[i].IsMove = true;
						}
						this.btn_delDoor.Enabled = true;
						AccMapdoorposBll accMapdoorposBll = new AccMapdoorposBll(MainForm._ia);
						accMapdoorposBll.DeleteByMapID(num);
						doorControls = this.mapControls[num].Map.DoorControls;
						for (int j = 0; j < doorControls.Count; j++)
						{
							if (doorControls[j].Visible)
							{
								AccMapdoorpos accMapdoorpos = new AccMapdoorpos();
								accMapdoorpos.map_id = num;
								accMapdoorpos.map_door_id = doorControls[j].AccDoorInfo.id;
								accMapdoorpos.left = doorControls[j].Left + this.mapControls[num].Map.StartLeft;
								accMapdoorpos.top = doorControls[j].Top + this.mapControls[num].Map.StartTop;
								accMapdoorpos.width = doorControls[j].Width;
								accMapdoorposBll.Add(accMapdoorpos);
							}
						}
					}
				}
				catch
				{
				}
			}
			this.Cursor = Cursors.Default;
		}

		private void btn_delDoor_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
				{
					int num = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
					if (this.mapControls.ContainsKey(num))
					{
						List<DevControl> doorControls = this.mapControls[num].Map.DoorControls;
						bool flag = false;
						int num2 = 0;
						while (num2 < doorControls.Count)
						{
							if (!doorControls[num2].Visible || !doorControls[num2].IsSelected)
							{
								num2++;
								continue;
							}
							flag = true;
							break;
						}
						if (flag)
						{
							if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDelSelectedDoorInfo", "是否删除该地图上选中的门"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
							{
								for (int i = 0; i < doorControls.Count; i++)
								{
									if (doorControls[i].IsSelected)
									{
										doorControls[i].Visible = false;
									}
								}
								AccMapdoorposBll accMapdoorposBll = new AccMapdoorposBll(MainForm._ia);
								accMapdoorposBll.DeleteByMapID(num);
								doorControls = this.mapControls[num].Map.DoorControls;
								for (int j = 0; j < doorControls.Count; j++)
								{
									if (doorControls[j].Visible)
									{
										AccMapdoorpos accMapdoorpos = new AccMapdoorpos();
										accMapdoorpos.map_id = num;
										accMapdoorpos.map_door_id = doorControls[j].AccDoorInfo.id;
										accMapdoorpos.left = doorControls[j].Left + this.mapControls[num].Map.StartLeft;
										accMapdoorpos.top = doorControls[j].Top + this.mapControls[num].Map.StartTop;
										accMapdoorpos.width = doorControls[j].Width;
										accMapdoorposBll.Add(accMapdoorpos);
									}
								}
								if (this.IsHaveDoor())
								{
									this.btn_delDoor.Enabled = true;
								}
								else
								{
									this.btn_delDoor.Enabled = false;
								}
							}
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的数据"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的数据"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的数据"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool IsHaveDoor()
		{
			if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
			{
				int key = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
				if (this.mapControls.ContainsKey(key))
				{
					List<DevControl> doorControls = this.mapControls[key].Map.DoorControls;
					for (int i = 0; i < doorControls.Count; i++)
					{
						if (doorControls[i].Visible)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void btn_saveInfo_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
				{
					int num = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
					AccMapdoorposBll accMapdoorposBll = new AccMapdoorposBll(MainForm._ia);
					if (this.mapControls.ContainsKey(num))
					{
						accMapdoorposBll.DeleteByMapID(num);
						List<DevControl> doorControls = this.mapControls[num].Map.DoorControls;
						for (int i = 0; i < doorControls.Count; i++)
						{
							if (doorControls[i].Visible)
							{
								AccMapdoorpos accMapdoorpos = new AccMapdoorpos();
								accMapdoorpos.map_id = num;
								accMapdoorpos.map_door_id = doorControls[i].AccDoorInfo.id;
								accMapdoorpos.left = doorControls[i].Left + this.mapControls[num].Map.StartLeft;
								accMapdoorpos.top = doorControls[i].Top + this.mapControls[num].Map.StartTop;
								accMapdoorpos.width = doorControls[i].Width;
								accMapdoorposBll.Add(accMapdoorpos);
							}
						}
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_openListen_Click(object sender, EventArgs e)
		{
			if (DeviceServers.Instance.Count > 0)
			{
				int num = 0;
				for (num = 0; num < DeviceServers.Instance.Count; num++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[num];
					if (deviceServerBll != null)
					{
						if (this.btn_openListen.Text == ShowMsgInfos.GetInfo("OpenListen", "启动监控"))
						{
							deviceServerBll.IsNeedListen = true;
						}
						else
						{
							deviceServerBll.IsNeedListen = false;
						}
					}
				}
			}
			if (this.btn_openListen.Text == ShowMsgInfos.GetInfo("OpenListen", "启动监控"))
			{
				RTEventProcessor.LoadDictionary(true);
				MonitorWatchdog.InitialMonitors();
				MonitorWatchdog.StartWatchdog();
				this.btn_openListen.Image = Resources.disableMonitoring;
				this.btn_openListen.Text = ShowMsgInfos.GetInfo("StopListen", "停止监控");
				AppSite.Instance.SetNodeValue("IsListen", "true");
			}
			else
			{
				MonitorWatchdog.StopWatchdog();
				MonitorWatchdog.DisConnectAll();
				this.btn_openListen.Image = Resources.Monitoring;
				this.btn_openListen.Text = ShowMsgInfos.GetInfo("OpenListen", "启动监控");
				AppSite.Instance.SetNodeValue("IsListen", "false");
			}
		}

		private void tab_map_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
		{
			if (this.tab_map.SelectedTab != null && this.tab_map.SelectedTab.Tag != null)
			{
				int key = int.Parse(this.tab_map.SelectedTab.Tag.ToString());
				if (this.mapControls.ContainsKey(key))
				{
					this.SetBtnEnabled(true);
					if (!this.IsHaveDoor())
					{
						this.btn_delDoor.Enabled = false;
					}
				}
				else
				{
					this.SetBtnEnabled(false);
				}
			}
			else
			{
				this.SetBtnEnabled(false);
			}
		}

		private void btn_addDoor_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		private void btn_addMap_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		private void btn_delMap_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		private void btn_delDoor_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		private void btn_editMap_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		private void btn_openListen_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		private void btn_saveInfo_MouseEnter(object sender, EventArgs e)
		{
			DevShowInfo.Instance.Hide();
		}

		public void Free()
		{
			try
			{
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					this.SaveTabSequence();
				}
				if (this.mapControls != null && this.mapControls.Count > 0)
				{
					int[] array = new int[this.mapControls.Count];
					this.mapControls.Keys.CopyTo(array, 0);
					for (int i = 0; i < array.Length; i++)
					{
						if (this.mapControls.ContainsKey(array[i]))
						{
							this.mapControls[array[i]].Free();
							this.mapControls[array[i]].Dispose();
							this.mapControls.Remove(array[i]);
						}
					}
				}
			}
			catch
			{
			}
		}

		private void SaveTabSequence()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TabItem tab in this.tab_map.Tabs)
			{
				int num = default(int);
				if (tab.Tag != null && int.TryParse(tab.Tag.ToString(), out num))
				{
					stringBuilder.Append(num + ",");
				}
			}
			AppSite.Instance.SetNodeValue("TabSequence", stringBuilder.ToString());
		}

		private List<AccMap> SortTabSequence(List<AccMap> lstSource)
		{
			int num = 0;
			if (lstSource == null)
			{
				return null;
			}
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("TabSequence");
			if (nodeValueByName == null || nodeValueByName.Trim() == "")
			{
				return null;
			}
			string[] array = nodeValueByName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			Dictionary<int, AccMap> dictionary = new Dictionary<int, AccMap>();
			foreach (AccMap item in lstSource)
			{
				if (!dictionary.ContainsKey(item.id))
				{
					dictionary.Add(item.id, item);
				}
			}
			List<int> list = new List<int>();
			List<AccMap> list2 = new List<AccMap>();
			for (int num2 = array.Length - 1; num2 >= 0; num2--)
			{
				if (int.TryParse(array[num2], out num) && dictionary.ContainsKey(num) && !list.Contains(num))
				{
					list2.Add(dictionary[num]);
					list.Add(num);
					lstSource.Remove(dictionary[num]);
				}
			}
			foreach (AccMap item2 in lstSource)
			{
				list2.Add(item2);
			}
			return list2;
		}

		private void BeginEditMode()
		{
			this.btn_addMap.Enabled = false;
			this.btn_editMap.Enabled = false;
			this.btn_delMap.Enabled = false;
			this.btn_openListen.Enabled = false;
			this.btn_addDoor.Enabled = true;
			this.btn_delDoor.Enabled = this.IsHaveDoor();
			this.btn_saveInfo.Enabled = true;
			this.SetDoorMovable(true);
		}

		private void EndEditMode()
		{
			this.btn_addMap.Enabled = true;
			this.btn_editMap.Enabled = true;
			this.btn_delMap.Enabled = true;
			this.btn_openListen.Enabled = true;
			this.btn_addDoor.Enabled = false;
			this.btn_delDoor.Enabled = false;
			this.btn_saveInfo.Enabled = false;
			this.SetDoorMovable(false);
		}

		private void SetDoorMovable(bool CanMove)
		{
			foreach (KeyValuePair<int, MapControl> mapControl in this.mapControls)
			{
				foreach (DevControl doorControl in mapControl.Value.DoorControls)
				{
					doorControl.IsMove = CanMove;
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MapUC));
			this.pnl_doorStatus = new PanelEx();
			this.tab_map = new DevComponents.DotNetBar.TabControl();
			this.tabpnl_first = new TabControlPanel();
			this.map_first = new MapControl();
			this.tabItem_first = new TabItem(this.components);
			this.MenuPanelEx = new PanelEx();
			this.toolStrip1 = new ToolStrip();
			this.btn_addMap = new ToolStripButton();
			this.btn_editMap = new ToolStripButton();
			this.btn_delMap = new ToolStripButton();
			this.btn_addDoor = new ToolStripButton();
			this.btn_delDoor = new ToolStripButton();
			this.btn_saveInfo = new ToolStripButton();
			this.btn_openListen = new ToolStripButton();
			((ISupportInitialize)this.tab_map).BeginInit();
			this.tab_map.SuspendLayout();
			this.tabpnl_first.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			base.SuspendLayout();
			this.pnl_doorStatus.CanvasColor = SystemColors.Control;
			this.pnl_doorStatus.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_doorStatus.Dock = DockStyle.Top;
			this.pnl_doorStatus.Location = new Point(0, 35);
			this.pnl_doorStatus.Name = "pnl_doorStatus";
			this.pnl_doorStatus.Size = new Size(879, 23);
			this.pnl_doorStatus.Style.Alignment = StringAlignment.Center;
			this.pnl_doorStatus.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_doorStatus.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_doorStatus.Style.Border = eBorderType.SingleLine;
			this.pnl_doorStatus.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_doorStatus.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_doorStatus.Style.GradientAngle = 90;
			this.pnl_doorStatus.TabIndex = 2;
			this.pnl_doorStatus.Text = "电子地图";
			this.tab_map.CanReorderTabs = true;
			this.tab_map.Controls.Add(this.tabpnl_first);
			this.tab_map.Dock = DockStyle.Fill;
			this.tab_map.Location = new Point(0, 58);
			this.tab_map.Name = "tab_map";
			this.tab_map.SelectedTabFont = new Font("宋体", 9f, FontStyle.Bold);
			this.tab_map.SelectedTabIndex = 0;
			this.tab_map.Size = new Size(879, 490);
			this.tab_map.TabIndex = 3;
			this.tab_map.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
			this.tab_map.Tabs.Add(this.tabItem_first);
			this.tab_map.Text = "tabControl1";
			this.tab_map.SelectedTabChanged += this.tab_map_SelectedTabChanged;
			this.tabpnl_first.Controls.Add(this.map_first);
			this.tabpnl_first.Dock = DockStyle.Fill;
			this.tabpnl_first.Location = new Point(0, 26);
			this.tabpnl_first.Name = "tabpnl_first";
			this.tabpnl_first.Padding = new System.Windows.Forms.Padding(1);
			this.tabpnl_first.Size = new Size(879, 464);
			this.tabpnl_first.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabpnl_first.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabpnl_first.Style.Border = eBorderType.SingleLine;
			this.tabpnl_first.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabpnl_first.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabpnl_first.Style.GradientAngle = 90;
			this.tabpnl_first.TabIndex = 1;
			this.tabpnl_first.TabItem = this.tabItem_first;
			this.map_first.Dock = DockStyle.Fill;
			this.map_first.Location = new Point(1, 1);
			this.map_first.Name = "map_first";
			this.map_first.Size = new Size(877, 462);
			this.map_first.TabIndex = 0;
			this.tabItem_first.AttachedControl = this.tabpnl_first;
			this.tabItem_first.Name = "tabItem_first";
			this.tabItem_first.Text = "map";
			this.MenuPanelEx.CanvasColor = SystemColors.Control;
			this.MenuPanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MenuPanelEx.Controls.Add(this.toolStrip1);
			this.MenuPanelEx.Dock = DockStyle.Top;
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(879, 35);
			this.MenuPanelEx.Style.Alignment = StringAlignment.Center;
			this.MenuPanelEx.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.MenuPanelEx.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.MenuPanelEx.Style.Border = eBorderType.SingleLine;
			this.MenuPanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MenuPanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenuPanelEx.Style.GradientAngle = 90;
			this.MenuPanelEx.TabIndex = 1;
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Items.AddRange(new ToolStripItem[7]
			{
				this.btn_addMap,
				this.btn_editMap,
				this.btn_delMap,
				this.btn_addDoor,
				this.btn_delDoor,
				this.btn_saveInfo,
				this.btn_openListen
			});
			this.toolStrip1.Location = new Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new Size(879, 38);
			this.toolStrip1.TabIndex = 7;
			this.toolStrip1.Text = "toolStrip1";
			this.btn_addMap.Image = (Image)componentResourceManager.GetObject("btn_addMap.Image");
			this.btn_addMap.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_addMap.ImageTransparentColor = Color.Magenta;
			this.btn_addMap.Name = "btn_addMap";
			this.btn_addMap.Size = new Size(89, 35);
			this.btn_addMap.Text = "添加地图";
			this.btn_addMap.Click += this.btn_addMap_Click;
			this.btn_addMap.MouseEnter += this.btn_addMap_MouseEnter;
			this.btn_editMap.Image = (Image)componentResourceManager.GetObject("btn_editMap.Image");
			this.btn_editMap.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_editMap.ImageTransparentColor = Color.Magenta;
			this.btn_editMap.Name = "btn_editMap";
			this.btn_editMap.Size = new Size(89, 35);
			this.btn_editMap.Text = "编辑地图";
			this.btn_editMap.Click += this.btn_editMap_Click;
			this.btn_editMap.MouseEnter += this.btn_editMap_MouseEnter;
			this.btn_delMap.Image = (Image)componentResourceManager.GetObject("btn_delMap.Image");
			this.btn_delMap.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delMap.ImageTransparentColor = Color.Magenta;
			this.btn_delMap.Name = "btn_delMap";
			this.btn_delMap.Size = new Size(89, 35);
			this.btn_delMap.Text = "删除地图";
			this.btn_delMap.Click += this.btn_delMap_Click;
			this.btn_delMap.MouseEnter += this.btn_delMap_MouseEnter;
			this.btn_addDoor.Image = (Image)componentResourceManager.GetObject("btn_addDoor.Image");
			this.btn_addDoor.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_addDoor.ImageTransparentColor = Color.Magenta;
			this.btn_addDoor.Name = "btn_addDoor";
			this.btn_addDoor.Size = new Size(77, 35);
			this.btn_addDoor.Text = "添加门";
			this.btn_addDoor.Click += this.btn_addDoor_Click;
			this.btn_addDoor.MouseEnter += this.btn_addDoor_MouseEnter;
			this.btn_delDoor.Image = (Image)componentResourceManager.GetObject("btn_delDoor.Image");
			this.btn_delDoor.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delDoor.ImageTransparentColor = Color.Magenta;
			this.btn_delDoor.Name = "btn_delDoor";
			this.btn_delDoor.Size = new Size(77, 35);
			this.btn_delDoor.Text = "删除门";
			this.btn_delDoor.Click += this.btn_delDoor_Click;
			this.btn_delDoor.MouseEnter += this.btn_delDoor_MouseEnter;
			this.btn_saveInfo.Image = (Image)componentResourceManager.GetObject("btn_saveInfo.Image");
			this.btn_saveInfo.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_saveInfo.ImageTransparentColor = Color.Magenta;
			this.btn_saveInfo.Name = "btn_saveInfo";
			this.btn_saveInfo.Size = new Size(113, 35);
			this.btn_saveInfo.Text = "保存位置信息";
			this.btn_saveInfo.Click += this.btn_saveInfo_Click;
			this.btn_saveInfo.MouseEnter += this.btn_saveInfo_MouseEnter;
			this.btn_openListen.Image = (Image)componentResourceManager.GetObject("btn_openListen.Image");
			this.btn_openListen.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_openListen.ImageTransparentColor = Color.Magenta;
			this.btn_openListen.Name = "btn_openListen";
			this.btn_openListen.Size = new Size(89, 35);
			this.btn_openListen.Text = "启动监控";
			this.btn_openListen.Click += this.btn_openListen_Click;
			this.btn_openListen.MouseEnter += this.btn_openListen_MouseEnter;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tab_map);
			base.Controls.Add(this.pnl_doorStatus);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "MapUC";
			base.Size = new Size(879, 548);
			((ISupportInitialize)this.tab_map).EndInit();
			this.tab_map.ResumeLayout(false);
			this.tabpnl_first.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
