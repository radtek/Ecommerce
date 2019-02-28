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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.Access.door;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class MonitoringUserControl : UserControl
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private int m_mapid = 0;

		private ImagesForm imagesForm = new ImagesForm();

		private FrmSimpleProgress fPb;

		private WaitForm m_waitForm = WaitForm.Instance;

		private Dictionary<int, PersonnelArea> alist = new Dictionary<int, PersonnelArea>();

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private Timer m_timer = new Timer();

		private IContainer components = null;

		private Panel pnl_main;

		private Panel pnl_title;

		private Label lab_area;

		private ComboBox cmb_door;

		private Label lab_control;

		private ComboBox cmb_control;

		private Label lab_door;

		private ComboBox cmb_area;

		public PanelEx pnl_doorStatus;

		private ToolStripButton btn_openDoor;

		private ToolStripButton btn_closeDoor;

		public ToolStrip MenuPanelEx;

		private MapControl map_main;

		private ToolStripButton btn_openSelectedDoor;

		private ToolStripButton btn_closeSelectedDoor;

		private ToolStripButton btn_openListen;

		public MonitoringUserControl(int mapid)
		{
			try
			{
				GLOBAL.monitorKeepAliveEnabled = true;
				GLOBAL.IsMonitorOwner = true;
				this.imagesForm.TopMost = true;
				this.imagesForm.Show();
				Application.DoEvents();
				this.InitializeComponent();
				MonitorWatchdog.OnOfflineLog += this.MonitorWatchdog_OnOfflineLog;
				MonitorWatchdog.OnOfflineLogCount += this.MonitorWatchdog_OnOfflineLogCount;
				MonitorWatchdog.OnOfflineLogEnd += this.MonitorWatchdog_OnOfflineLogEnd;
				DevLogServer.ReConnect();
				this.m_mapid = mapid;
				this.map_main.Load();
				this.map_main.InitMap(this.m_mapid);
				this.map_main.Map.LoadFinishEvent += this.Map_LoadFinishEvent;
				this.m_timer.Interval = 3000;
				this.m_timer.Enabled = false;
				SysLogServer.WriteLog("main m_timer...", true);
				this.m_timer.Tick += this.m_timer_Tick;
				initLang.LocaleForm(this, base.Name);
				if (AccCommon.CodeVersion == CodeVersionType.OpenDoorWarning)
				{
					ToolStripItem toolStripItem = this.MenuPanelEx.Items.Add(ShowMsgInfos.GetInfo("CancelAllAlarm", "取消所有报警"), this.btn_closeDoor.Image, this.CancelAllAlarm);
					toolStripItem.ImageScaling = ToolStripItemImageScaling.None;
				}
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					ToolStripItem toolStripItem2 = this.MenuPanelEx.Items.Add(ShowMsgInfos.GetInfo("UnLockDoor", "解锁"), Resource.UnLock, this.UnlockSelectedDoor);
					toolStripItem2.ImageScaling = ToolStripItemImageScaling.None;
					ToolStripItem toolStripItem3 = this.MenuPanelEx.Items.Add(ShowMsgInfos.GetInfo("UnLockAllDoor", "解锁所有门"), Resource.UnLockAll, this.UnlockAllDoor);
					toolStripItem3.ImageScaling = ToolStripItemImageScaling.None;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
				MessageBox.Show(ex.Message);
				MessageBox.Show("MonitoringUserControl");
			}
			this.imagesForm.Hide();
			this.ChangeSkin();
		}

		private void MonitorWatchdog_OnOfflineLogEnd()
		{
			try
			{
				if (this.fPb != null)
				{
					this.fPb.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("MonitorWatchdog_OnOfflineLogEnd");
			}
		}

		private void MonitorWatchdog_OnOfflineLogCount()
		{
			try
			{
				if (this.fPb != null)
				{
					this.fPb.stepProgressBar();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("MonitorWatchdog_OnOfflineLogCount");
			}
		}

		private void MonitorWatchdog_OnOfflineLog(DeviceServerBll sender, OnOfflineLogEventArgs e)
		{
			try
			{
				this.fPb = new FrmSimpleProgress();
				this.fPb.setTitulo("Baixando logs...");
				this.fPb.setProgressBar(0, e.count);
				this.fPb.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("MonitorWatchdog_OnOfflineLog");
			}
		}

		private void ChangeSkin()
		{
			try
			{
				int skinOption = SkinParameters.SkinOption;
				if (skinOption == 1)
				{
					this.btn_closeDoor.Image = ResourceIPC.closeAllDoor;
					this.btn_closeSelectedDoor.Image = ResourceIPC.closeDoor;
					this.btn_openDoor.Image = ResourceIPC.openAllDoor;
					this.btn_openSelectedDoor.Image = ResourceIPC.openDoor;
					this.btn_openListen.Image = ResourceIPC.Monitoring11;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("ChangeSkin");
			}
		}

		private void UnlockSelectedDoor(object sender, EventArgs e)
		{
			try
			{
				if (!this.IsHaveVisible())
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SNodoors", "没有门"));
				}
				else
				{
					int num = 0;
					List<DevControl> selecteds = this.map_main.Map.GetSelecteds();
					if (selecteds == null || selecteds.Count <= 0)
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SSelectdoor", "请先选择门"));
					}
					else
					{
						this.m_waitForm.ShowEx();
						Application.DoEvents();
						for (int i = 0; i < selecteds.Count; i++)
						{
							int num2 = selecteds[i].UnlockDoor();
							Application.DoEvents();
							if (num2 < 0)
							{
								this.m_waitForm.ShowInfos(selecteds[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num2));
								num++;
							}
							else
							{
								this.m_waitForm.ShowInfos(selecteds[i].DoorName + ":" + ShowMsgInfos.GetInfo("Suceed", "成功"));
							}
							this.m_waitForm.ShowProgress((int)(((decimal)i + decimal.One) / (decimal)selecteds.Count * 100m));
						}
						this.m_waitForm.ShowProgress(100);
						this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("Suceed", "成功") + ":" + (selecteds.Count - num).ToString() + " " + ShowMsgInfos.GetInfo("Failed", "失败") + ":" + num.ToString());
						this.m_waitForm.HideEx(false);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("UnlockSelectedDoor");
			}
		}

		private void UnlockAllDoor(object sender, EventArgs e)
		{
			try
			{
				if (!this.IsHaveVisible())
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SNodoors", "没有门"));
				}
				else
				{
					this.m_waitForm.ShowEx();
					Application.DoEvents();
					int num = 0;
					List<DevControl> visibles = this.GetVisibles();
					for (int i = 0; i < visibles.Count; i++)
					{
						int num2 = visibles[i].UnlockDoor();
						Application.DoEvents();
						if (num2 < 0)
						{
							this.m_waitForm.ShowInfos(visibles[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num2));
							num++;
						}
						else
						{
							this.m_waitForm.ShowInfos(visibles[i].DoorName + ":" + ShowMsgInfos.GetInfo("Suceed", "成功"));
						}
						this.m_waitForm.ShowProgress((int)(((decimal)i + decimal.One) / (decimal)visibles.Count * 100m));
					}
					this.m_waitForm.ShowProgress(100);
					this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("Suceed", "成功") + ":" + (visibles.Count - num).ToString() + " " + ShowMsgInfos.GetInfo("Failed", "失败") + ":" + num.ToString());
					this.m_waitForm.HideEx(false);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("UnlockAllDoor");
			}
		}

		private void CancelAllAlarm(object sender, EventArgs e)
		{
			try
			{
				if (!this.IsHaveVisible())
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SNodoors", "没有门"));
				}
				else
				{
					if (AccCommon.CodeVersion == CodeVersionType.OpenDoorWarning)
					{
						DevControl.StopWarning = true;
					}
					this.m_waitForm.ShowEx();
					Application.DoEvents();
					int num = 0;
					List<DevControl> visibles = this.GetVisibles();
					for (int i = 0; i < visibles.Count; i++)
					{
						int num2 = visibles[i].CancelAlarm();
						Application.DoEvents();
						if (num2 < 0)
						{
							this.m_waitForm.ShowInfos(visibles[i].DoorName + ":" + PullSDkErrorInfos.GetInfo(num2));
							num++;
						}
						else
						{
							this.m_waitForm.ShowInfos(visibles[i].DoorName + ":" + ShowMsgInfos.GetInfo("Suceed", "成功"));
						}
						this.m_waitForm.ShowProgress((int)(((decimal)i + decimal.One) / (decimal)visibles.Count * 100m));
					}
					this.m_waitForm.ShowProgress(100);
					this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("Suceed", "成功") + ":" + (visibles.Count - num).ToString() + " " + ShowMsgInfos.GetInfo("Failed", "失败") + ":" + num.ToString());
					this.m_waitForm.HideEx(false);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("CancelAllAlarm");
			}
		}

		private void Map_LoadFinishEvent(object sender, EventArgs e)
		{
			try
			{
				this.InitMachines();
				this.LoadDoor();
				this.InitArea();
				if (this.m_mapid > 0)
				{
					this.pnl_title.Visible = false;
					List<DevControl> doorControls = this.map_main.DoorControls;
					for (int i = 0; i < doorControls.Count; i++)
					{
						doorControls[i].IsMove = true;
					}
				}
				this.SetListenBtn();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
				MessageBox.Show(ex.Message);
				MessageBox.Show("Map_LoadFinishEvent");
			}
			try
			{
				this.CheckPermission();
			}
			catch (Exception ex2)
			{
				MessageBox.Show(ex2.Message);
				MessageBox.Show("Map_LoadFinishEvent");
			}
		}

		private void CheckPermission()
		{
			try
			{
				if (!SysInfos.IsOwerControlPermission(SysInfos.Monitoring))
				{
					this.SetBtnEable(false);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("CheckPermission");
			}
		}

		private void ShowInfos(string infoStr)
		{
			try
			{
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new ShowInfo(this.ShowInfos), infoStr);
					}
					else
					{
						this.m_waitForm.ShowInfos(infoStr);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("ShowInfos");
			}
		}

		private void ShowProgress(int prg)
		{
			try
			{
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new ShowProgressHandle(this.ShowProgress), prg);
					}
					else
					{
						this.m_waitForm.ShowProgress(prg);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("ShowProgress");
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
					this.btn_openListen.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.disableMonitoring : Resources.disableMonitoring);
					this.btn_openListen.Text = ShowMsgInfos.GetInfo("StopListen", "停止监控");
					RTEventProcessor.LoadDictionary(true);
					MonitorWatchdog.InitialMonitors();
					MonitorWatchdog.StartWatchdog();
					if (this.IsHaveVisible())
					{
						this.btn_closeDoor.Enabled = true;
						this.btn_closeSelectedDoor.Enabled = true;
						this.btn_openDoor.Enabled = true;
						this.btn_openSelectedDoor.Enabled = true;
					}
					else
					{
						this.btn_closeDoor.Enabled = false;
						this.btn_closeSelectedDoor.Enabled = false;
						this.btn_openDoor.Enabled = false;
						this.btn_openSelectedDoor.Enabled = false;
					}
				}
				else
				{
					this.btn_closeDoor.Enabled = false;
					this.btn_closeSelectedDoor.Enabled = false;
					this.btn_openDoor.Enabled = false;
					this.btn_openSelectedDoor.Enabled = false;
					this.btn_openListen.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.Monitoring11 : Resources.Monitoring);
					this.btn_openListen.Text = ShowMsgInfos.GetInfo("OpenListen", "启动监控");
				}
				this.CheckPermission();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage("SetListenBtn error:" + ex.Message);
				MessageBox.Show(ex.Message);
				MessageBox.Show("SetListenBtn");
			}
		}

		private void InitArea()
		{
			try
			{
				SysLogServer.WriteLog("InitArea...1", true);
				this.alist.Clear();
				SysLogServer.WriteLog("InitArea...2", true);
				this.cmb_area.Items.Clear();
				SysLogServer.WriteLog("InitArea...3", true);
				this.cmb_area.Items.Add(ShowMsgInfos.GetInfo("SMonitorAll", "全部"));
				SysLogServer.WriteLog("InitArea...4", true);
				PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
				SysLogServer.WriteLog("InitArea...5", true);
				List<PersonnelArea> modelList = personnelAreaBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						SysLogServer.WriteLog("InitArea...6", true);
						this.alist.Add(modelList[i].id, modelList[i]);
						SysLogServer.WriteLog("InitArea...7", true);
						this.cmb_area.Items.Add(modelList[i].areaname);
						SysLogServer.WriteLog("InitArea...8", true);
					}
				}
				SysLogServer.WriteLog("InitArea...9...", true);
				if (this.cmb_area.Items.Count > 0)
				{
					int num = this.cmb_area.Items.Count;
					SysLogServer.WriteLog("InitArea...10...cmb_area.Items.Count=" + num.ToString(), true);
					num = this.cmb_area.SelectedIndex;
					SysLogServer.WriteLog("InitArea...10...cmb_area.SelectedIndex=" + num.ToString(), true);
					try
					{
						this.cmb_area.SelectedIndex = 0;
					}
					catch
					{
						this.cmb_area.SelectedIndex = 0;
					}
					SysLogServer.WriteLog("InitArea...11", true);
				}
				SysLogServer.WriteLog("InitArea...12", true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("InitArea");
			}
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> list = null;
				list = machinesBll.GetModelList("");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.mlist.ContainsKey(list[i].ID))
						{
							this.mlist.Add(list[i].ID, list[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("InitMachines");
			}
		}

		private void LoadDoor()
		{
			try
			{
				this.dlist.Clear();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dlist.ContainsKey(modelList[i].id) && this.mlist.ContainsKey(modelList[i].device_id))
						{
							this.dlist.Add(modelList[i].id, modelList[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("LoadDoor");
			}
		}

		public object CopyDictionaryData(object obj)
		{
			object result = null;
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
				MemoryStream memoryStream = new MemoryStream();
				binaryFormatter.Serialize(memoryStream, obj);
				memoryStream.Position = 0L;
				result = binaryFormatter.Deserialize(memoryStream);
				memoryStream.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("CopyDictionaryData");
			}
			return result;
		}

		private void cmb_area_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.cmb_control.Items.Clear();
				this.cmb_control.Items.Add("-----");
				Dictionary<int, Machines> dictionary = new Dictionary<int, Machines>();
				object obj = null;
				obj = this.CopyDictionaryData(this.mlist);
				dictionary = (Dictionary<int, Machines>)obj;
				if (this.cmb_area.SelectedIndex > 0)
				{
					Dictionary<int, PersonnelArea> dictionary2 = new Dictionary<int, PersonnelArea>();
					obj = null;
					obj = this.CopyDictionaryData(this.alist);
					dictionary2 = (Dictionary<int, PersonnelArea>)obj;
					foreach (KeyValuePair<int, PersonnelArea> item in dictionary2)
					{
						if (item.Value.areaname == this.cmb_area.Text)
						{
							foreach (KeyValuePair<int, Machines> item2 in dictionary)
							{
								if (item2.Value.area_id == item.Value.id)
								{
									this.cmb_control.Items.Add(item2.Value.MachineAlias);
								}
							}
							break;
						}
					}
				}
				else
				{
					foreach (KeyValuePair<int, Machines> item3 in dictionary)
					{
						this.cmb_control.Items.Add(item3.Value.MachineAlias);
					}
				}
				if (this.cmb_control.Items.Count > 0)
				{
					this.cmb_control.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("cmb_area_SelectedIndexChanged");
			}
		}

		private void cmb_control_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.cmb_door.Items.Clear();
				this.cmb_door.Items.Add("-----");
				if (this.cmb_control.SelectedIndex > 0)
				{
					foreach (KeyValuePair<int, Machines> item in this.mlist)
					{
						if (item.Value.MachineAlias == this.cmb_control.Text)
						{
							foreach (KeyValuePair<int, AccDoor> item2 in this.dlist)
							{
								if (item2.Value.device_id == item.Value.ID)
								{
									this.cmb_door.Items.Add(item2.Value.door_name);
								}
							}
							break;
						}
					}
				}
				else if (this.cmb_control.Items.Count > 1)
				{
					for (int i = 1; i < this.cmb_control.Items.Count; i++)
					{
						foreach (KeyValuePair<int, Machines> item3 in this.mlist)
						{
							if (item3.Value.MachineAlias == this.cmb_control.Items[i].ToString())
							{
								foreach (KeyValuePair<int, AccDoor> item4 in this.dlist)
								{
									if (item4.Value.device_id == item3.Value.ID)
									{
										this.cmb_door.Items.Add(item4.Value.door_name);
									}
								}
								break;
							}
						}
					}
				}
				if (this.cmb_door.Items.Count > 0)
				{
					this.cmb_door.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("cmb_control_SelectedIndexChanged");
			}
		}

		private void cmb_door_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.cmb_door.SelectedIndex > 0)
				{
					foreach (KeyValuePair<int, AccDoor> item in this.dlist)
					{
						if (item.Value.door_name == this.cmb_door.Text)
						{
							List<DevControl> doorControls = this.map_main.DoorControls;
							for (int i = 0; i < doorControls.Count; i++)
							{
								if (doorControls[i].AccDoorInfo.id == item.Key)
								{
									doorControls[i].Visible = true;
								}
								else
								{
									doorControls[i].Visible = false;
								}
							}
							break;
						}
					}
				}
				else
				{
					List<DevControl> doorControls2 = this.map_main.DoorControls;
					for (int j = 0; j < doorControls2.Count; j++)
					{
						bool flag = false;
						for (int k = 0; k < this.cmb_door.Items.Count; k++)
						{
							string b = this.cmb_door.Items[k].ToString();
							if (doorControls2[j].AccDoorInfo.door_name == b)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							doorControls2[j].Visible = true;
						}
						else
						{
							doorControls2[j].Visible = false;
						}
					}
				}
				if (!this.IsHaveVisible())
				{
					this.SetBtnEable(false);
				}
				else
				{
					this.ModifyXY();
					this.SetBtnEable(true);
					this.CheckPermission();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("cmb_door_SelectedIndexChanged");
			}
		}

		private void ModifyXY()
		{
			try
			{
				List<DevControl> visibles = this.GetVisibles();
				if (visibles != null && visibles.Count > 0)
				{
					int num = 3;
					int num2 = 3;
					for (int i = 0; i < visibles.Count; i++)
					{
						visibles[i].Left = num;
						visibles[i].Top = num2;
						num += visibles[i].Width + 5;
						if (num + visibles[i].Width + 5 >= base.Width)
						{
							num = 3;
							num2 = num2 + visibles[i].Height + 5;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("ModifyXY");
			}
		}

		private List<DevControl> GetVisibles()
		{
			List<DevControl> list = new List<DevControl>();
			try
			{
				List<DevControl> doorControls = this.map_main.DoorControls;
				for (int i = 0; i < doorControls.Count; i++)
				{
					if (doorControls[i].Visible)
					{
						list.Add(doorControls[i]);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("GetVisibles");
			}
			return list;
		}

		private void btn_selectall_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.cmb_area.Items.Count > 0)
				{
					this.cmb_area.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_selectall_Click");
			}
		}

		private void btn_remoteOpen_Click(object sender, EventArgs e)
		{
			try
			{
				List<DevControl> list = null;
				OpenDoorSet openDoorSet = new OpenDoorSet();
				openDoorSet.ShowDialog();
				if (openDoorSet.Second != 0)
				{
					list = this.map_main.DoorControls;
					if (list != null && list.Count > 0)
					{
						for (int num = list.Count - 1; num >= 0; num--)
						{
							if (!list[num].Visible)
							{
								list.RemoveAt(num);
							}
						}
						this.m_waitForm.HideEx(false);
						MonitorWatchdog.OpenDoors(list, openDoorSet, this.m_waitForm);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_remoteOpen_Click");
			}
		}

		private void m_timer_Tick(object sender, EventArgs e)
		{
			try
			{
				SysLogServer.WriteLog("main m_timer_Tick...1", true);
				if (DevLogServer.IsOpenOrCloseFinish && DevLogServer.IsCmdFinish)
				{
					this.DevLogServer_FinishEvent(sender, e);
					this.m_timer.Enabled = false;
					SysLogServer.WriteLog("main m_timer_Tick...2", true);
				}
				SysLogServer.WriteLog("main m_timer_Tick...3", true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("m_timer_Tick");
			}
		}

		private void DevLogServer_FinishEvent(object sender, EventArgs e)
		{
			try
			{
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new EventHandler(this.DevLogServer_FinishEvent), sender, e);
					}
					else
					{
						SysLogServer.WriteLog("main DevLogServer_FinishEvent...1", true);
						this.m_timer.Enabled = false;
						DevLogServer.FinishEvent -= this.DevLogServer_FinishEvent;
						this.m_waitForm.ShowProgress(100);
						int rightCount = DevLogServer.RightCount;
						int num = DevLogServer.AllCount - rightCount;
						this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + rightCount + " " + ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + num + "\r\n");
						DevLogServer.ShowInfoEvent -= this.ShowInfos;
						DevLogServer.ShowProgressEvent -= this.ShowProgress;
						this.m_waitForm.HideEx(false);
						SysLogServer.WriteLog("main DevLogServer_FinishEvent...2", true);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("DevLogServer_FinishEvent");
			}
		}

		private bool IsHaveVisible()
		{
			bool result = false;
			try
			{
				List<DevControl> doorControls = this.map_main.DoorControls;
				if (doorControls != null)
				{
					int num = 0;
					while (num < doorControls.Count)
					{
						if (!doorControls[num].Visible)
						{
							num++;
							continue;
						}
						result = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("IsHaveVisible");
			}
			return result;
		}

		private void btn_remoteclose_Click(object sender, EventArgs e)
		{
			try
			{
				List<DevControl> list = null;
				CloseDoorSet closeDoorSet = new CloseDoorSet();
				closeDoorSet.ShowDialog();
				if (closeDoorSet.Second != -1)
				{
					list = this.map_main.DoorControls;
					if (list != null && list.Count > 0)
					{
						for (int num = list.Count - 1; num >= 0; num--)
						{
							if (!list[num].Visible)
							{
								list.RemoveAt(num);
							}
						}
						this.m_waitForm.HideEx(false);
						MonitorWatchdog.CloseDoor(list, closeDoorSet, this.m_waitForm);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_remoteclose_Click");
			}
		}

		private void btn_openSelectedDoor_Click(object sender, EventArgs e)
		{
			try
			{
				List<DevControl> list = null;
				OpenDoorSet openDoorSet = new OpenDoorSet();
				openDoorSet.ShowDialog();
				if (openDoorSet.Second != 0)
				{
					list = this.map_main.Map.GetSelecteds();
					if (list != null && list.Count > 0)
					{
						this.m_waitForm.HideEx(false);
						MonitorWatchdog.OpenDoors(list, openDoorSet, this.m_waitForm);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_openSelectedDoor_Click");
			}
		}

		private bool IsHaveSelected()
		{
			bool result = false;
			try
			{
				List<DevControl> doorControls = this.map_main.DoorControls;
				int num = 0;
				while (num < doorControls.Count)
				{
					if (!doorControls[num].Visible || !doorControls[num].IsSelected)
					{
						num++;
						continue;
					}
					result = true;
					break;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("IsHaveSelected");
			}
			return result;
		}

		private void btn_closeSelectedDoor_Click(object sender, EventArgs e)
		{
			try
			{
				List<DevControl> list = null;
				CloseDoorSet closeDoorSet = new CloseDoorSet();
				closeDoorSet.ShowDialog();
				if (closeDoorSet.Second != -1)
				{
					list = this.map_main.Map.GetSelecteds();
					if (list != null && list.Count > 0)
					{
						this.m_waitForm.HideEx(false);
						MonitorWatchdog.CloseDoor(list, closeDoorSet, this.m_waitForm);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_closeSelectedDoor_Click");
			}
		}

		private void btn_openListen_Click(object sender, EventArgs e)
		{
			try
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
								deviceServerBll.Disconnect();
							}
						}
					}
				}
				if (this.btn_openListen.Text == ShowMsgInfos.GetInfo("OpenListen", "启动监控"))
				{
					RTEventProcessor.LoadDictionary(true);
					MonitorWatchdog.InitialMonitors();
					MonitorWatchdog.StartWatchdog();
					this.btn_openListen.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.disableMonitoring : Resources.disableMonitoring);
					this.btn_openListen.Text = ShowMsgInfos.GetInfo("StopListen", "停止监控");
					AppSite.Instance.SetNodeValue("IsListen", "true");
					this.btn_closeDoor.Enabled = true;
					this.btn_closeSelectedDoor.Enabled = true;
					this.btn_openDoor.Enabled = true;
					this.btn_openSelectedDoor.Enabled = true;
				}
				else
				{
					MonitorWatchdog.StopWatchdog();
					MonitorWatchdog.DisConnectAll();
					this.btn_closeDoor.Enabled = false;
					this.btn_closeSelectedDoor.Enabled = false;
					this.btn_openDoor.Enabled = false;
					this.btn_openSelectedDoor.Enabled = false;
					this.btn_openListen.Image = ((SkinParameters.SkinOption == 1) ? ResourceIPC.Monitoring11 : Resources.Monitoring);
					this.btn_openListen.Text = ShowMsgInfos.GetInfo("OpenListen", "启动监控");
					AppSite.Instance.SetNodeValue("IsListen", "false");
				}
				this.CheckPermission();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_openListen_Click");
			}
		}

		private void SetBtnEable(bool iseable)
		{
			try
			{
				if (!iseable)
				{
					this.btn_closeDoor.Enabled = iseable;
					this.btn_closeSelectedDoor.Enabled = iseable;
					this.btn_openDoor.Enabled = iseable;
					this.btn_openSelectedDoor.Enabled = iseable;
					this.btn_openListen.Enabled = iseable;
				}
				else if (this.btn_openListen.Text == ShowMsgInfos.GetInfo("StopListen", "停止监控"))
				{
					this.btn_closeDoor.Enabled = iseable;
					this.btn_closeSelectedDoor.Enabled = iseable;
					this.btn_openDoor.Enabled = iseable;
					this.btn_openSelectedDoor.Enabled = iseable;
					this.btn_openListen.Enabled = iseable;
				}
				else
				{
					this.btn_closeDoor.Enabled = false;
					this.btn_closeSelectedDoor.Enabled = false;
					this.btn_openDoor.Enabled = false;
					this.btn_openSelectedDoor.Enabled = false;
					this.btn_openListen.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("SetBtnEable");
			}
		}

		private void MenuPanelEx_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("MenuPanelEx_MouseEnter");
			}
		}

		private void btn_openDoor_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_openDoor_MouseEnter");
			}
		}

		private void btn_closeDoor_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_closeDoor_MouseEnter");
			}
		}

		private void btn_openSelectedDoor_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_openSelectedDoor_MouseEnter");
			}
		}

		private void btn_closeSelectedDoor_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_closeSelectedDoor_MouseEnter");
			}
		}

		private void btn_openListen_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("btn_openListen_MouseEnter");
			}
		}

		private void pnl_title_MouseEnter(object sender, EventArgs e)
		{
			try
			{
				DevShowInfo.Instance.Hide();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("pnl_title_MouseEnter");
			}
		}

		public void Free()
		{
			try
			{
				if (this.map_main != null)
				{
					this.map_main.Free();
					this.map_main.Dispose();
					this.map_main = null;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				MessageBox.Show("Free");
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MonitoringUserControl));
			this.pnl_main = new Panel();
			this.map_main = new MapControl();
			this.pnl_title = new Panel();
			this.lab_area = new Label();
			this.cmb_door = new ComboBox();
			this.lab_control = new Label();
			this.cmb_control = new ComboBox();
			this.lab_door = new Label();
			this.cmb_area = new ComboBox();
			this.pnl_doorStatus = new PanelEx();
			this.MenuPanelEx = new ToolStrip();
			this.btn_openDoor = new ToolStripButton();
			this.btn_closeDoor = new ToolStripButton();
			this.btn_openSelectedDoor = new ToolStripButton();
			this.btn_closeSelectedDoor = new ToolStripButton();
			this.btn_openListen = new ToolStripButton();
			this.pnl_main.SuspendLayout();
			this.pnl_title.SuspendLayout();
			this.MenuPanelEx.SuspendLayout();
			base.SuspendLayout();
			this.pnl_main.BackColor = Color.White;
			this.pnl_main.Controls.Add(this.map_main);
			this.pnl_main.Controls.Add(this.pnl_title);
			this.pnl_main.Controls.Add(this.pnl_doorStatus);
			this.pnl_main.Controls.Add(this.MenuPanelEx);
			this.pnl_main.Dock = DockStyle.Fill;
			this.pnl_main.Location = new Point(0, 0);
			this.pnl_main.Name = "pnl_main";
			this.pnl_main.Size = new Size(747, 663);
			this.pnl_main.TabIndex = 1;
			this.map_main.DevTabItem = null;
			this.map_main.Dock = DockStyle.Fill;
			this.map_main.Location = new Point(0, 109);
			this.map_main.Name = "map_main";
			this.map_main.Size = new Size(747, 554);
			this.map_main.TabIndex = 15;
			this.pnl_title.BorderStyle = BorderStyle.FixedSingle;
			this.pnl_title.Controls.Add(this.lab_area);
			this.pnl_title.Controls.Add(this.cmb_door);
			this.pnl_title.Controls.Add(this.lab_control);
			this.pnl_title.Controls.Add(this.cmb_control);
			this.pnl_title.Controls.Add(this.lab_door);
			this.pnl_title.Controls.Add(this.cmb_area);
			this.pnl_title.Dock = DockStyle.Top;
			this.pnl_title.Location = new Point(0, 66);
			this.pnl_title.Name = "pnl_title";
			this.pnl_title.Size = new Size(747, 43);
			this.pnl_title.TabIndex = 14;
			this.pnl_title.MouseEnter += this.pnl_title_MouseEnter;
			this.lab_area.Location = new Point(18, 17);
			this.lab_area.Name = "lab_area";
			this.lab_area.Size = new Size(29, 13);
			this.lab_area.TabIndex = 2;
			this.lab_area.Text = "区域";
			this.cmb_door.FormattingEnabled = true;
			this.cmb_door.Location = new Point(477, 12);
			this.cmb_door.Name = "cmb_door";
			this.cmb_door.Size = new Size(121, 21);
			this.cmb_door.TabIndex = 7;
			this.cmb_door.SelectedIndexChanged += this.cmb_door_SelectedIndexChanged;
			this.lab_control.Location = new Point(201, 16);
			this.lab_control.Name = "lab_control";
			this.lab_control.Size = new Size(96, 13);
			this.lab_control.TabIndex = 3;
			this.lab_control.Text = "控制器";
			this.cmb_control.FormattingEnabled = true;
			this.cmb_control.Location = new Point(305, 11);
			this.cmb_control.Name = "cmb_control";
			this.cmb_control.Size = new Size(121, 21);
			this.cmb_control.TabIndex = 6;
			this.cmb_control.SelectedIndexChanged += this.cmb_control_SelectedIndexChanged;
			this.lab_door.Location = new Point(442, 16);
			this.lab_door.Name = "lab_door";
			this.lab_door.Size = new Size(31, 13);
			this.lab_door.TabIndex = 4;
			this.lab_door.Text = "门";
			this.cmb_area.FormattingEnabled = true;
			this.cmb_area.Location = new Point(55, 12);
			this.cmb_area.Name = "cmb_area";
			this.cmb_area.Size = new Size(121, 21);
			this.cmb_area.TabIndex = 5;
			this.cmb_area.SelectedIndexChanged += this.cmb_area_SelectedIndexChanged;
			this.pnl_doorStatus.CanvasColor = SystemColors.Control;
			this.pnl_doorStatus.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_doorStatus.Dock = DockStyle.Top;
			this.pnl_doorStatus.Location = new Point(0, 41);
			this.pnl_doorStatus.Name = "pnl_doorStatus";
			this.pnl_doorStatus.Size = new Size(747, 25);
			this.pnl_doorStatus.Style.Alignment = StringAlignment.Center;
			this.pnl_doorStatus.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_doorStatus.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_doorStatus.Style.Border = eBorderType.SingleLine;
			this.pnl_doorStatus.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_doorStatus.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_doorStatus.Style.GradientAngle = 90;
			this.pnl_doorStatus.TabIndex = 13;
			this.pnl_doorStatus.Text = "实时监控";
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[5]
			{
				this.btn_openDoor,
				this.btn_closeDoor,
				this.btn_openSelectedDoor,
				this.btn_closeSelectedDoor,
				this.btn_openListen
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(747, 41);
			this.MenuPanelEx.TabIndex = 12;
			this.MenuPanelEx.Text = "toolStrip1";
			this.MenuPanelEx.MouseEnter += this.MenuPanelEx_MouseEnter;
			this.btn_openDoor.Image = (Image)componentResourceManager.GetObject("btn_openDoor.Image");
			this.btn_openDoor.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_openDoor.ImageTransparentColor = Color.Magenta;
			this.btn_openDoor.Name = "btn_openDoor";
			this.btn_openDoor.Size = new Size(115, 38);
			this.btn_openDoor.Text = "开当前所有门";
			this.btn_openDoor.Click += this.btn_remoteOpen_Click;
			this.btn_openDoor.MouseEnter += this.btn_openDoor_MouseEnter;
			this.btn_closeDoor.Image = (Image)componentResourceManager.GetObject("btn_closeDoor.Image");
			this.btn_closeDoor.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_closeDoor.ImageTransparentColor = Color.Magenta;
			this.btn_closeDoor.Name = "btn_closeDoor";
			this.btn_closeDoor.Size = new Size(115, 38);
			this.btn_closeDoor.Text = "关当前所有门";
			this.btn_closeDoor.Click += this.btn_remoteclose_Click;
			this.btn_closeDoor.MouseEnter += this.btn_closeDoor_MouseEnter;
			this.btn_openSelectedDoor.Image = (Image)componentResourceManager.GetObject("btn_openSelectedDoor.Image");
			this.btn_openSelectedDoor.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_openSelectedDoor.ImageTransparentColor = Color.Magenta;
			this.btn_openSelectedDoor.Name = "btn_openSelectedDoor";
			this.btn_openSelectedDoor.Size = new Size(115, 38);
			this.btn_openSelectedDoor.Text = "开当前选中门";
			this.btn_openSelectedDoor.Click += this.btn_openSelectedDoor_Click;
			this.btn_openSelectedDoor.MouseEnter += this.btn_openSelectedDoor_MouseEnter;
			this.btn_closeSelectedDoor.Image = (Image)componentResourceManager.GetObject("btn_closeSelectedDoor.Image");
			this.btn_closeSelectedDoor.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_closeSelectedDoor.ImageTransparentColor = Color.Magenta;
			this.btn_closeSelectedDoor.Name = "btn_closeSelectedDoor";
			this.btn_closeSelectedDoor.Size = new Size(115, 38);
			this.btn_closeSelectedDoor.Text = "关当前选中门";
			this.btn_closeSelectedDoor.Click += this.btn_closeSelectedDoor_Click;
			this.btn_closeSelectedDoor.MouseEnter += this.btn_closeSelectedDoor_MouseEnter;
			this.btn_openListen.Image = Resources.Monitoring;
			this.btn_openListen.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_openListen.ImageTransparentColor = Color.Magenta;
			this.btn_openListen.Name = "btn_openListen";
			this.btn_openListen.Size = new Size(91, 38);
			this.btn_openListen.Text = "启动监控";
			this.btn_openListen.Click += this.btn_openListen_Click;
			this.btn_openListen.MouseEnter += this.btn_openListen_MouseEnter;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.pnl_main);
			base.Name = "MonitoringUserControl";
			base.Size = new Size(747, 663);
			this.pnl_main.ResumeLayout(false);
			this.pnl_title.ResumeLayout(false);
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
