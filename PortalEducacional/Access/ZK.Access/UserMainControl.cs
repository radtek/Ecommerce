/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.AdvTree;
using DevComponents.DotNetBar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ZK.Access.door;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class UserMainControl : UserControl
	{
		public enum PageIdEnum
		{
			Navigation,
			ReportToday,
			ReportLastThreeDay,
			ReportLastSevenDay,
			ReportLastWeek,
			ReportException,
			Department = 30,
			UserList,
			IssueCardList,
			Area = 50,
			Device,
			SearchDevice,
			TimeZone = 70,
			Holiday,
			DoorSetting,
			AccessLevel,
			WiegandFormat,
			InterLock,
			AntiBack,
			Linkage,
			FirstCard,
			MultiCard,
			RealTimeMonitor,
			VirtueMap,
			ReaderSetting,
			AuxiliarySetting
		}

		public Thread threadDownLoadRecords = null;

		private bool Down = false;

		private ToolStripMenuItem mnu_AttReport;

		private Node nodeAttReport;

		public static AttParamBll attParamBll = new AttParamBll(MainForm._ia);

		public static AttParam attParam = null;

		private DateTime m_lastCollect = DateTime.Now;

		private UserControl m_lastfrm = null;

		private System.Windows.Forms.Timer mTime = new System.Windows.Forms.Timer();

		private ImagesForm imagesForm = new ImagesForm();

		private object DownLoadLock = new object();

		private DateTime? LastDownloadTime = null;

		private System.Timers.Timer tmrDownLoadRecord;

		private Node Node_Navigation;

		private Node Node_AttReport;

		private IContainer components = null;

		private ButtonX btn_Operation;

		private PanelEx panelEx3;

		private ToolStripPanel top_StripPanel;

		private ElementStyle elementStyle1;

		private ExplorerBar LeftExplorerBar;

		private AdvTree advTree1;

		private Node node_Personnel;

		private Node Node_dept;

		private Node Node_Employee;

		private Node Node_IssueCard;

		private Node node_Device;

		private Node Node_AreaSetting;

		private Node Node_Device2;

		private Node Node_SearchAccess;

		private Node node_AccessControl;

		private Node Node_TimeSlot;

		private Node Node_Holidays;

		private Node Node_doorsetting;

		private Node Node_anti;

		private Node Node_Linkage;

		private Node Node_Interlock;

		private Node Node_NormalOpen;

		private Node Node_MultiCardOpen;

		private Node Node_Monitoring;

		private NodeConnector nodeConnector1;

		private ExpandableSplitter expandableSplitter1;

		private ToolStripStatusLabel statusLabel1;

		private ToolStripStatusLabel lb_Opstatus;

		private Node node_doorLevels;

		private Node node_map;

		private Node node_RecordsReport;

		private Node node_recordsOfLastWeek;

		private Node node_ExptRecords;

		private Node node_RecordesToday;

		private Node node_RecordsOfHalfWeek;

		private Node node_recordsOfWeek;

		private System.Windows.Forms.Timer tmr_timeShow;

		public PanelEx pnl_type;

		public PanelEx MenuPanelEx;

		private PanelEx MainPanelEx;

		private Node node1;

		private Node node_wiegand;

		private Node node_reader;

		private Node node_auxiliary;

		private Node nodeCustomReport;

		private Node node_VisitoM;

		private Node node_visitors;

		private ToolStripMenuItem menuItem_sysset;

		private ToolStripMenuItem menuItem_role;

		private ToolStripMenuItem menuItem_sysuer;

		private ToolStripMenuItem menu_modifyPwd;

		private ToolStripMenuItem menu_initSystem;

		private ToolStripMenuItem menu_dbManage;

		private ToolStripMenuItem menu_DatabaseSet;

		private ToolStripMenuItem menu_dbBackup;

		private ToolStripMenuItem menu_dbRestore;

		private ToolStripMenuItem menu_DBBakPath;

		private ToolStripMenuItem menu_sysParamSet;

		private ToolStripMenuItem menu_navigation;

		private ToolStripMenuItem menu_selectlan;

		private ToolStripMenuItem menu_exit;

		private ToolStripMenuItem mnu_optmodules;

		private ToolStripMenuItem menuItem_sysuser;

		private ToolStripMenuItem menuItem_dept;

		private ToolStripMenuItem menuItem_user;

		private ToolStripMenuItem menuItem_issueCard;

		private ToolStripMenuItem menuItem_ToolDvice;

		private ToolStripMenuItem menuItem_Area;

		private ToolStripMenuItem menuItem_device;

		private ToolStripMenuItem menuItem_SearchAccess;

		private ToolStripMenuItem menuItem_AccessControl;

		private ToolStripMenuItem menuItem_TimeSlot;

		private ToolStripMenuItem menuItem_Holidays;

		private ToolStripMenuItem menuItem_doorsetting;

		private ToolStripMenuItem menuItem_Levels;

		private ToolStripMenuItem menuItem_wiegand;

		private ToolStripMenuItem menuItem_Interlock;

		private ToolStripMenuItem menuItem_anti;

		private ToolStripMenuItem menuItem_Linkage;

		private ToolStripMenuItem menuItem_NormalOpen;

		private ToolStripMenuItem menuItem_MultiCardOpen;

		private ToolStripMenuItem menuItem_Monitoring;

		private ToolStripMenuItem menuItem_map;

		private ToolStripMenuItem mnu_reader;

		private ToolStripMenuItem mnu_auxiliary;

		private ToolStripMenuItem menuItem_RecordsReport;

		private ToolStripMenuItem menuItem_RecordesToday;

		private ToolStripMenuItem menuItem_RecordesOfThreeDays;

		private ToolStripMenuItem menuItem_RecordsOfWeek;

		private ToolStripMenuItem menuItem_recordsOfLastWeek;

		private ToolStripMenuItem menuItem_ExptRecords;

		private ToolStripMenuItem mnuCustomReport;

		private ToolStripMenuItem menuItem_Help;

		private ToolStripMenuItem Menu_help;

		private ToolStripMenuItem menuItem_About;

		private ToolStripMenuItem menu_GenerateLangFile;

		private MenuStrip menu_Main;

		public UserMainControl()
		{
			this.InitializeComponent();
			try
			{
				UserMainControl.checkReg();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			this.node_visitors.Visible = MainForm._visitorEnabled;
			this.mnu_AttReport = new ToolStripMenuItem();
			this.mnu_AttReport.Image = Resources.deptSetting;
			this.mnu_AttReport.Name = "mnu_AttReport";
			this.mnu_AttReport.Size = new Size(81, 20);
			this.mnu_AttReport.Text = "Relatório de presença";
			this.mnu_AttReport.Click += this.mnu_AttReport_Click;
			this.menu_Main.Items.Insert(this.menu_Main.Items.Count - 1, this.mnu_AttReport);
			this.nodeAttReport = new Node();
			this.nodeAttReport.Expanded = true;
			this.nodeAttReport.Image = Resources.deptSetting;
			this.nodeAttReport.Name = "nodeAttReport";
			this.nodeAttReport.Text = "Relatório de prensença";
			this.nodeAttReport.NodeClick += this.nodeAttReport_NodeClick;
			this.advTree1.Nodes.Add(this.nodeAttReport);
			try
			{
				initLang.LocaleForm(this, base.Name);
			}
			catch (Exception ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
			}
			try
			{
				this.MainPanelEx.Controls.Clear();
				this.MenuPanelEx.Controls.Clear();
				this.ECardTongRegis();
				switch (SkinParameters.SkinOption)
				{
				case 0:
					this.InitApplication();
					if (SkinParameters.SkinOption != 1)
					{
						string nodeValueByName = AppSite.Instance.GetNodeValueByName("Language");
						List<string> list = null;
						list = AppSite.Instance.GetNodeListValueByName(nodeValueByName);
						this.menu_selectlan.DropDownItems.Clear();
						for (int i = 0; i < list.Count; i++)
						{
							this.AddContextMenu(list[i], this.menu_selectlan.DropDownItems, this.MenuLangClicked);
						}
					}
					break;
				case 1:
					this.menu_Main.Visible = false;
					this.node_map.Visible = false;
					this.Node_SearchAccess.Visible = false;
					this.node_map.Visible = false;
					this.menuItem_Area.Visible = false;
					this.Node_AreaSetting.Visible = false;
					this.ChangeSkin();
					this.AddAttNode();
					break;
				}
				this.menu_navigation_Click(null, null);
			}
			catch (Exception ex3)
			{
				SysDialogs.ShowWarningMessage(ex3.Message);
			}
		}

		public static void checkReg()
		{
			MainForm._visitorEnabled = true;
			MainForm._elevatorEnabled = false;
		}

		private void ChangeSkin()
		{
			this.MenuPanelEx.Style.BackColor1.Color = SkinParameters.ToolBarBackColor1;
			this.MenuPanelEx.Style.BackColor2.Color = SkinParameters.ToolBarBackColor2;
			this.advTree1.BackColor = SkinParameters.TreeMenuBackColor;
			this.expandableSplitter1.BackColor = SkinParameters.FormBackColor;
			this.advTree1.NodeStyle.TextColor = SkinParameters.Tree_Grid_ForeColor;
			this.node_Personnel.Image = ResourceIPC.Personnel1;
			this.Node_dept.Image = ResourceIPC.dept;
			this.Node_Employee.Image = ResourceIPC.personnels;
			this.Node_IssueCard.Image = ResourceIPC.Issue_Card;
			this.node_AccessControl.Image = ResourceIPC.door_m;
			this.Node_TimeSlot.Image = ResourceIPC.time_Zones;
			this.Node_Holidays.Image = ResourceIPC.holiday;
			this.Node_doorsetting.Image = ResourceIPC.doormanagement;
			this.node_doorLevels.Image = ResourceIPC.permission_group;
			this.node_wiegand.Image = ResourceIPC.Real_Time;
			this.Node_Interlock.Image = ResourceIPC.Interlock;
			this.Node_anti.Image = ResourceIPC.passback;
			this.Node_NormalOpen.Image = ResourceIPC.First_Card;
			this.Node_MultiCardOpen.Image = ResourceIPC.batchIssueCard;
			this.Node_Monitoring.Image = ResourceIPC.Monitoring1;
			this.node_RecordsReport.Image = ResourceIPC.reports1;
			this.node_RecordesToday.Image = ResourceIPC.one;
			this.node_RecordsOfHalfWeek.Image = ResourceIPC.three;
			this.node_recordsOfWeek.Image = ResourceIPC.seven;
			this.node_recordsOfLastWeek.Image = ResourceIPC.allDoorEvent_ico;
			this.node_ExptRecords.Image = ResourceIPC.exception_Events;
		}

		private void AddAttNode()
		{
			this.Node_Navigation = new Node();
			this.Node_Navigation.Text = ShowMsgInfos.GetInfo("SwitchToNavigation", "Navegação");
			this.Node_Navigation.NodeClick += this.Navigation_NodeClick;
			this.Node_Navigation.Image = Resources.navigation;
			this.advTree1.Nodes.Insert(0, this.Node_Navigation);
			this.Node_AttReport = new Node();
			this.Node_AttReport.Text = ShowMsgInfos.GetInfo("SwitchToAtt", "Relatório de presença");
			this.Node_AttReport.NodeClick += this.AttNode_NodeClick;
			this.Node_AttReport.Image = ResourceIPC.time_Zones;
			this.advTree1.Nodes.Add(this.Node_AttReport);
		}

		private void ECardTongRegis()
		{
			if (AccCommon.IsECardTong > 0)
			{
				this.Text = "[" + AccCommon.RegistCompany + "] " + this.Text;
			}
		}

		private void InitApplication()
		{
			this.InitContent();
			this.InitMenu();
		}

		private void InitContent()
		{
		}

		private void InitMenu()
		{
			this.SetMenuEnable(this.Node_dept, SysInfos.Personnel);
			this.SetMenuEnable(this.Node_Employee, SysInfos.Personnel);
			this.SetMenuEnable(this.Node_IssueCard, SysInfos.Personnel);
			this.SetMenuEnable(this.Node_AreaSetting, SysInfos.Device);
			this.SetMenuEnable(this.Node_Device2, SysInfos.Device);
			this.SetMenuEnable(this.Node_doorsetting, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_SearchAccess, SysInfos.Device);
			this.SetMenuEnable(this.Node_TimeSlot, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_Holidays, SysInfos.AccessLevel);
			this.SetMenuEnable(this.node_doorLevels, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_anti, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_Linkage, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_Interlock, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_NormalOpen, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_MultiCardOpen, SysInfos.AccessLevel);
			this.SetMenuEnable(this.Node_Monitoring, SysInfos.Monitoring);
			this.SetMenuEnable(this.node_map, SysInfos.Monitoring);
			this.SetMenuEnable(this.node_recordsOfLastWeek, SysInfos.Report);
			this.SetMenuEnable(this.node_RecordsReport, SysInfos.Report);
			this.SetMenuEnable(this.node_ExptRecords, SysInfos.Report);
			this.SetMenuEnable(this.node_RecordesToday, SysInfos.Report);
			this.SetMenuEnable(this.node_RecordsOfHalfWeek, SysInfos.Report);
			this.SetMenuEnable(this.node_recordsOfWeek, SysInfos.Report);
			this.SetMenuEnable(this.nodeCustomReport, SysInfos.Report);
			this.SetMenuEnable(this.menuItem_role, SysInfos.SysUser);
			this.SetMenuEnable(this.menuItem_sysuer, SysInfos.SysUser);
			this.SetMenuEnable(this.menuItem_dept, SysInfos.Personnel);
			this.SetMenuEnable(this.menuItem_user, SysInfos.Personnel);
			this.SetMenuEnable(this.menuItem_issueCard, SysInfos.Personnel);
			this.SetMenuEnable(this.menuItem_Area, SysInfos.Device);
			this.SetMenuEnable(this.menuItem_device, SysInfos.Device);
			this.SetMenuEnable(this.menuItem_SearchAccess, SysInfos.Device);
			this.SetMenuEnable(this.menuItem_TimeSlot, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_Holidays, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_doorsetting, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_Levels, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_Interlock, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_anti, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_Linkage, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_NormalOpen, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_MultiCardOpen, SysInfos.AccessLevel);
			this.SetMenuEnable(this.menuItem_Monitoring, SysInfos.Monitoring);
			this.SetMenuEnable(this.menuItem_map, SysInfos.Monitoring);
			this.SetMenuEnable(this.menu_initSystem, SysInfos.SysSet);
			this.SetMenuEnable(this.menu_dbBackup, SysInfos.SysSet);
			this.SetMenuEnable(this.menu_dbRestore, SysInfos.SysSet);
			this.SetMenuEnable(this.menu_sysParamSet, SysInfos.SysSet);
			this.SetMenuEnable(this.menu_dbManage, SysInfos.SysSet);
			this.SetMenuEnable(this.menuItem_ExptRecords, SysInfos.Report);
			this.SetMenuEnable(this.menuItem_RecordesToday, SysInfos.Report);
			this.SetMenuEnable(this.menuItem_RecordesOfThreeDays, SysInfos.Report);
			this.SetMenuEnable(this.menuItem_RecordsOfWeek, SysInfos.Report);
			this.SetMenuEnable(this.menuItem_recordsOfLastWeek, SysInfos.Report);
			this.SetMenuEnable(this.mnuCustomReport, SysInfos.Report);
			if (!SysInfos.IsOwerControlPermission(SysInfos.SysSet))
			{
				this.menu_sysParamSet.Enabled = false;
				this.menu_dbManage.Enabled = false;
				this.menu_dbRestore.Enabled = false;
				this.menu_dbBackup.Enabled = false;
				this.menu_initSystem.Enabled = false;
			}
			if (!this.Node_dept.Visible && !this.Node_Employee.Visible && !this.Node_IssueCard.Visible)
			{
				this.node_Personnel.Visible = false;
			}
			if (!this.Node_AreaSetting.Visible && !this.Node_Device2.Visible && !this.Node_SearchAccess.Visible)
			{
				this.node_Device.Visible = false;
			}
			if (!this.Node_TimeSlot.Visible && !this.Node_Holidays.Visible && !this.Node_doorsetting.Visible && !this.node_doorLevels.Visible && !this.Node_Interlock.Visible && !this.Node_anti.Visible && !this.Node_Linkage.Visible && !this.Node_NormalOpen.Visible && !this.Node_MultiCardOpen.Visible && !this.Node_Monitoring.Visible && !this.node_map.Visible)
			{
				this.node_AccessControl.Visible = false;
			}
			this.menu_GenerateLangFile.Visible = ("Access.vshost" == Process.GetCurrentProcess().ProcessName);
			this.menuItem_RecordesOfThreeDays.Visible = false;
			this.menuItem_RecordsOfWeek.Visible = false;
			this.menuItem_recordsOfLastWeek.Visible = false;
			this.node_RecordsOfHalfWeek.Visible = false;
			this.node_recordsOfLastWeek.Visible = false;
			this.node_recordsOfWeek.Visible = false;
			this.SetMenuEnable(this.mnu_AttReport, SysInfos.Report);
			this.SetMenuEnable(this.nodeAttReport, SysInfos.Report);
		}

		private void SetMenuEnable(Node node, string permissionName)
		{
			if (SysInfos.IsOwerShowPermission(permissionName))
			{
				node.Visible = true;
			}
			else
			{
				node.Visible = false;
			}
		}

		private void SetMenuEnable(ToolStripMenuItem menu, string permissionName)
		{
			if (SysInfos.IsOwerShowPermission(permissionName))
			{
				menu.Enabled = true;
			}
			else
			{
				menu.Enabled = false;
			}
			if (menu == this.menu_initSystem && AccCommon.IsECardTong > 0)
			{
				menu.Enabled = false;
			}
		}

		private void DisposeEx(Control ctl)
		{
			if (ctl != null)
			{
				if (ctl.Controls.Count > 0)
				{
					for (int i = 0; i < ctl.Controls.Count; i++)
					{
						this.DisposeEx(ctl.Controls[i]);
					}
				}
				ctl.Controls.Clear();
				ctl.Dispose();
				ctl = null;
			}
		}

		private void switchover(UserControl frm)
		{
			if (this.m_lastfrm != null)
			{
				try
				{
					MapUC mapUC = this.m_lastfrm as MapUC;
					if (mapUC != null && !(frm is MapUC) && !(frm is MonitoringUserControl))
					{
						mapUC.Free();
					}
					else
					{
						MonitoringUserControl monitoringUserControl = this.m_lastfrm as MonitoringUserControl;
						if (monitoringUserControl != null && !(frm is MapUC) && !(frm is MonitoringUserControl))
						{
							GLOBAL.monitorKeepAliveEnabled = false;
							GLOBAL.IsMonitorOwner = false;
							GLOBAL.resetMonitorKeepAlive();
							monitoringUserControl.Free();
						}
					}
					try
					{
						FieldInfo[] fields = this.m_lastfrm.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						FieldInfo[] array = fields;
						foreach (FieldInfo fieldInfo in array)
						{
							if (fieldInfo.FieldType == typeof(Control))
							{
								Control ctl = (Control)fieldInfo.GetValue(this.m_lastfrm);
								this.DisposeEx(ctl);
							}
							else if (fieldInfo.FieldType == typeof(PanelEx))
							{
								PanelEx panelEx = (PanelEx)fieldInfo.GetValue(this.m_lastfrm);
								panelEx.Dispose();
							}
							else if (fieldInfo.FieldType == typeof(ToolStrip))
							{
								ToolStrip toolStrip = (ToolStrip)fieldInfo.GetValue(this.m_lastfrm);
								toolStrip.Dispose();
							}
						}
					}
					catch
					{
					}
					this.m_lastfrm.Parent = null;
					this.m_lastfrm.Dispose();
				}
				catch
				{
				}
				this.m_lastfrm = null;
			}
			if (DateTime.Now > this.m_lastCollect.AddMinutes(1.0))
			{
				this.m_lastCollect = DateTime.Now;
				try
				{
					GC.Collect();
				}
				catch
				{
				}
			}
			this.MainPanelEx.Controls.Clear();
			this.MenuPanelEx.Controls.Clear();
			this.pnl_type.Controls.Clear();
			this.m_lastfrm = frm;
			this.m_lastfrm.Parent = this.MainPanelEx;
			this.m_lastfrm.Dock = DockStyle.Fill;
			this.m_lastfrm.Show();
		}

		private void Operation_buttonX_Click(object sender, EventArgs e)
		{
			this.advTree1.Visible = true;
		}

		private void menu_navigation_Click(object sender, EventArgs e)
		{
			this.MenuPanelEx.Controls.Clear();
			this.MainPanelEx.Controls.Clear();
			this.pnl_type.Controls.Clear();
			NavigationUserControl navigationUserControl = new NavigationUserControl();
			navigationUserControl.ClickEvent += this.navigationUserControl_ClickEvent;
			navigationUserControl.MenuPanelEx.Visible = false;
			navigationUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			navigationUserControl.panelEx1.Parent = this.pnl_type;
			navigationUserControl.MenuPanelEx.Dock = DockStyle.Fill;
			navigationUserControl.MenuPanelEx.Show();
			navigationUserControl.Parent = this.MainPanelEx;
			navigationUserControl.Dock = DockStyle.Fill;
			navigationUserControl.Show();
		}

		private void navigationUserControl_ClickEvent(int id)
		{
			switch (id)
			{
			case 1:
				this.Device_buttonItem_Click(null, null);
				break;
			case 2:
				this.personnel_buttonItem_Click(null, null);
				break;
			case 3:
				this.TimeSlot_buttonItem_Click(null, null);
				break;
			case 4:
				this.Holidays_buttonItem_Click(null, null);
				break;
			case 5:
				this.doorsettinbtn_Click(null, null);
				break;
			case 6:
				this.Level_buttonItem_Click(null, null);
				break;
			case 7:
				this.Monitoring_buttonItem_Click(null, null);
				break;
			case 8:
				this.node_AllRecords_NodeClick(null, null);
				break;
			}
		}

		private void Level_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			LevelUserControl levelUserControl = new LevelUserControl();
			this.switchover(levelUserControl);
			levelUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			levelUserControl.pnl_userLevel.Parent = this.pnl_type;
			levelUserControl.Show();
			this.enbaled(sender, true);
		}

		private void DST_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			DSTUserControl dSTUserControl = new DSTUserControl();
			this.switchover(dSTUserControl);
			dSTUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			dSTUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void Reports_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			ReportsUserControl reportsUserControl = new ReportsUserControl();
			this.switchover(reportsUserControl);
			reportsUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void Menu_ModifyPassword_Click(object sender, EventArgs e)
		{
			ModifyPasswordForm modifyPasswordForm = new ModifyPasswordForm();
			modifyPasswordForm.ShowDialog();
		}

		private Hashtable GetManagerInfo()
		{
			try
			{
				Hashtable hashtable = new Hashtable();
				AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
				DataSet allList = authUserBll.GetAllList();
				if (allList != null)
				{
					if (allList.Tables.Count > 0)
					{
						DataTable dataTable = allList.Tables[0];
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							hashtable.Add(dataTable.Rows[i]["id"].ToString(), dataTable.Rows[i]["username"].ToString());
						}
					}
					return hashtable;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			this.StartDownLoadTimer();
			this.mTime.Tick += this.mTime_Tick;
			this.mTime.Interval = 10000;
			this.mTime.Enabled = true;
		}

		private void mTime_Tick(object sender, EventArgs e)
		{
			if (MainForm._ia != null && MainForm._ia.MainFrom != null && !MainForm._ia.MainFrom.IsDisposed)
			{
				this.mTime.Enabled = false;
				FrmShowUpdata.Instance.ShowEx();
			}
		}

		private int GetlatestTime()
		{
			int num = 0;
			int num2 = 0;
			string[] array = null;
			DateTime now = DateTime.Now;
			int year = now.Year;
			now = DateTime.Now;
			int month = now.Month;
			now = DateTime.Now;
			int day = now.Day;
			try
			{
				ArrayList arrayList = new ArrayList();
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("TimeDownload");
				if (nodeValueByName == "")
				{
					return 0;
				}
				string[] array2 = nodeValueByName.Split(';');
				arrayList.Clear();
				for (num = 0; num < array2.Length; num++)
				{
					array = array2[num].Split(':');
					DateTime dateTime = new DateTime(year, month, day, int.Parse(array[0]), int.Parse(array[1]), 0);
					if (DateTime.Compare(dateTime, DateTime.Now) > 0)
					{
						arrayList.Add((int)(dateTime - DateTime.Now).TotalMilliseconds);
					}
				}
				return this.GetMin(arrayList);
			}
			catch
			{
				return 0;
			}
		}

		private void DownLoadRecordsOnTime()
		{
			this.Down = true;
			this.threadDownLoadRecords = new Thread(this.DownLoadRecords);
			this.threadDownLoadRecords.Start();
		}

		private void DownLoadRecords()
		{
			try
			{
				while (this.Down)
				{
					int num = this.GetlatestTime();
					if (num == 0)
					{
						this.Down = false;
						break;
					}
					int num2 = num / 1000 + 1;
					for (int i = 0; i < num2; i++)
					{
						if (!this.Down)
						{
							break;
						}
						Thread.Sleep(1000);
					}
					if (!this.Down)
					{
						break;
					}
					DownLoadRecordForm downLoadRecordForm = new DownLoadRecordForm(true);
					downLoadRecordForm.ShowDialog();
				}
				this.threadDownLoadRecords = null;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private int GetMin(ArrayList list)
		{
			if (list == null || list.Count <= 0)
			{
				return 0;
			}
			int num = int.Parse(list[0].ToString());
			for (int i = 1; i < list.Count; i++)
			{
				if (double.Parse(list[i].ToString()) < (double)num)
				{
					num = int.Parse(list[i].ToString());
				}
			}
			return num;
		}

		private void menu_modifyPwd_Click(object sender, EventArgs e)
		{
			ModifyPasswordForm modifyPasswordForm = new ModifyPasswordForm();
			modifyPasswordForm.ShowDialog();
		}

		private void menuItem_dept_Click(object sender, EventArgs e)
		{
			this.Department_buttonItem_Click(null, null);
		}

		private void menuItem_About_Click(object sender, EventArgs e)
		{
			string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\about.txt";
			if (File.Exists(path))
			{
				AboutForm2 aboutForm = new AboutForm2();
				aboutForm.ShowDialog();
			}
			else
			{
				AboutForm aboutForm2 = new AboutForm();
				aboutForm2.ShowDialog();
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!Program.IsRestart && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SExitSystem", "Deseja sair do sistema?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}

		private void ReStart()
		{
			Program.IsRestart = true;
			MainForm._ia.MainFrom.Close();
		}

		private void menu_DBBakPath_Click(object sender, EventArgs e)
		{
			DataPackForm dataPackForm = new DataPackForm();
			dataPackForm.ShowDialog();
		}

		private void menu_exit_Click(object sender, EventArgs e)
		{
			MainForm._ia.MainFrom.Close();
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void enbaled(object sender, bool b)
		{
			if (sender is Node)
			{
				(sender as Node).Selectable = b;
				(sender as Node).Enabled = b;
			}
		}

		private void menu_selectlan_Click(object sender, EventArgs e)
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("Language");
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
		}

		private void menuItem_sysset_Click(object sender, EventArgs e)
		{
		}

		private ToolStripMenuItem AddContextMenu(string text, ToolStripItemCollection cms, EventHandler callback)
		{
			if (text == "-")
			{
				ToolStripSeparator value = new ToolStripSeparator();
				cms.Add(value);
				return null;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split('_');
				if (array.Length == 2)
				{
					ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem(array[1]);
					toolStripMenuItem.Image = Resources.log;
					toolStripMenuItem.Tag = array[0];
					toolStripMenuItem.Name = array[1];
					if (callback != null)
					{
						toolStripMenuItem.Click += callback;
					}
					cms.Add(toolStripMenuItem);
					return toolStripMenuItem;
				}
				ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem(array[0]);
				toolStripMenuItem2.Image = Resources.setting;
				toolStripMenuItem2.Tag = 50;
				toolStripMenuItem2.Name = array[0];
				if (callback != null)
				{
					toolStripMenuItem2.Click += callback;
				}
				if (!cms.ContainsKey(toolStripMenuItem2.Name))
				{
					cms.Add(toolStripMenuItem2);
					return toolStripMenuItem2;
				}
			}
			return null;
		}

		private void MenuLangClicked(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
			AppSite.Instance.SetNodeValue("Language", $"{toolStripMenuItem.Tag}");
			if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "Sucesso na operação. Deseja reiniciar o software para torna-la efetiva?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				Program.IsRestart = true;
			}
			MainForm._ia.MainFrom.Close();
		}

		private void menuItem_wiegand_Click(object sender, EventArgs e)
		{
			this.doorsettinbtn_Click(sender, e);
			WiegandSeting wiegandSeting = new WiegandSeting(0, 0);
			wiegandSeting.ShowDialog();
		}

		private void MenuAccessRepairDBClicked(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
			if (this.RepairAccessDB())
			{
				if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "Sucesso na operação. Deseja reiniciar o software para torna-la efetiva?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					Program.IsRestart = true;
				}
				MainForm._ia.MainFrom.Close();
			}
		}

		private bool RepairAccessDB()
		{
			bool flag = false;
			try
			{
				this.imagesForm.TopMost = true;
				this.imagesForm.Show();
				Application.DoEvents();
				flag = DbHelperOleDb.CompactAccessDB(true);
			}
			catch (Exception)
			{
				flag = false;
			}
			this.imagesForm.Hide();
			Application.DoEvents();
			return flag;
		}

		private void doorsettinbtn_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			DoorUC doorUC = new DoorUC();
			this.switchover(doorUC);
			doorUC.MenuPanelEx.Parent = this.MenuPanelEx;
			doorUC.panelEx2.Parent = this.pnl_type;
			doorUC.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void personnel_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			PersonnelForm personnelForm = new PersonnelForm();
			this.switchover(personnelForm);
			personnelForm.MenuPanelEx.Parent = this.MenuPanelEx;
			personnelForm.pnl_personnel.Parent = this.pnl_type;
			personnelForm.MenuPanelEx.Dock = DockStyle.Fill;
			personnelForm.Show();
			this.enbaled(sender, true);
		}

		private void Device_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			DeviceUserControl deviceUserControl = new DeviceUserControl();
			this.switchover(deviceUserControl);
			deviceUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			deviceUserControl.panelEx1.Parent = this.pnl_type;
			deviceUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void TimeSlot_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			TimeSlotUserControl timeSlotUserControl = new TimeSlotUserControl();
			this.switchover(timeSlotUserControl);
			timeSlotUserControl.panelEx1.Parent = this.MenuPanelEx;
			timeSlotUserControl.pnl_timeZones.Parent = this.pnl_type;
			timeSlotUserControl.Show();
			this.enbaled(sender, true);
		}

		private void Holidays_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			HolidaysUserControl holidaysUserControl = new HolidaysUserControl();
			this.switchover(holidaysUserControl);
			holidaysUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			holidaysUserControl.pnl_holiday.Parent = this.pnl_type;
			holidaysUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void Monitoring_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			GLOBAL.IsMonitorOwner = false;
			if (GLOBAL.IsMonitorActive())
			{
				this.enbaled(sender, true);
				SysDialogs.ShowInfoMessage("Já existe uma instância do monitor de eventos em execução.");
			}
			else
			{
				MonitoringUserControl monitoringUserControl = new MonitoringUserControl(0);
				this.switchover(monitoringUserControl);
				monitoringUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
				monitoringUserControl.pnl_doorStatus.Parent = this.pnl_type;
				monitoringUserControl.MenuPanelEx.Show();
				Application.DoEvents();
				this.enbaled(sender, true);
			}
		}

		private void Role_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			RoleUserControl roleUserControl = new RoleUserControl();
			this.switchover(roleUserControl);
			roleUserControl.MenupanelEx.Parent = this.MenuPanelEx;
			roleUserControl.pnl_role.Parent = this.pnl_type;
			roleUserControl.MenupanelEx.Show();
			this.enbaled(sender, true);
		}

		private void userBtn_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			UserControl2 userControl = new UserControl2();
			this.switchover(userControl);
			userControl.MenupanelEx.Parent = this.MenuPanelEx;
			userControl.pnl_user.Parent = this.pnl_type;
			userControl.MenupanelEx.Show();
			this.enbaled(sender, true);
		}

		private void menu_initSystem_Click(object sender, EventArgs e)
		{
			InitDatabaseForm initDatabaseForm = new InitDatabaseForm();
			initDatabaseForm.ShowDialog();
			if (Program.IsRestart)
			{
				base.Hide();
				MainForm._ia.MainFrom.Close();
			}
		}

		private void menu_dbManage_MouseHover(object sender, EventArgs e)
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("data");
			if (nodeValueByName == "Access")
			{
				this.AddContextMenu(ShowMsgInfos.GetInfo("SRepairAccessDb", "Comprimir banco de dados Access"), this.menu_dbManage.DropDownItems, this.MenuAccessRepairDBClicked);
			}
		}

		private void menu_DatabaseSet_Click(object sender, EventArgs e)
		{
			try
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				bool flag = false;
				DataType dataType = AppSite.Instance.DataType;
				switch (dataType)
				{
				case DataType.Access:
					DbHelperOleDb.GetDatabaseAddress(DbHelperOleDb.connectionString, ref text3, ref text);
					break;
				case DataType.SqlServer:
					DbHelperSQL.GetDatabaseAddress(DbHelperSQL.connectionString, out flag, out text4, out text3, out text2, out text);
					break;
				}
				DataConfigSet dataConfigSet = new DataConfigSet();
				dataConfigSet.ShowDialog();
				if (dataConfigSet.DataType == 1)
				{
					AccessSet accessSet = new AccessSet();
					accessSet.ShowDialog();
					if (accessSet.IsOk && Program.IsRestart)
					{
						if (dataType != 0)
						{
							AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
						}
						else
						{
							string text5 = "";
							string text6 = "";
							DbHelperOleDb.GetDatabaseAddress(AppSite.Instance.GetNodeValueByName("ConnectionString"), ref text5, ref text6);
							if (text5.ToLower() != text3.ToLower())
							{
								AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
							}
						}
						MainForm._ia.MainFrom.Close();
					}
				}
				else if (dataConfigSet.DataType == 2)
				{
					SqlServerSet sqlServerSet = new SqlServerSet();
					sqlServerSet.ShowDialog();
					if (sqlServerSet.IsOk && Program.IsRestart)
					{
						if (dataType != DataType.SqlServer)
						{
							AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
						}
						else
						{
							string text7 = "";
							string text8 = "";
							string text9 = "";
							string text10 = "";
							bool flag2 = false;
							DbHelperSQL.GetDatabaseAddress(AppSite.Instance.GetNodeValueByName("ConnectionString"), out flag2, out text10, out text8, out text9, out text7);
							if (text8.ToLower() != text3.ToLower() || text10.ToLower() != text4.ToLower())
							{
								AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
							}
						}
						MainForm._ia.MainFrom.Close();
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void menu_dbBackup_Click(object sender, EventArgs e)
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("BakDBPath");
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = ShowMsgInfos.GetInfo("BackupDataBase", "Fazer backup do banco de dados");
			saveFileDialog.Filter = ((AppSite.Instance.DataType == DataType.Access) ? "MDB File(*.mdb)|*.mdb" : "Backup File(*.bak)|*.bak");
			if (nodeValueByName != null && Directory.Exists(nodeValueByName))
			{
				saveFileDialog.InitialDirectory = nodeValueByName;
			}
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					switch (AppSite.Instance.DataType)
					{
					case DataType.Access:
						DbHelperOleDb.BackupDataBase(saveFileDialog.FileName);
						break;
					case DataType.SqlServer:
						DbHelperSQL.BackupDataBase(saveFileDialog.FileName, 240);
						break;
					}
					if (File.Exists(saveFileDialog.FileName))
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "Operação bem sucedida"));
					}
					else
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationFailed", "Operação falhou"));
					}
				}
				catch (Exception ex)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("BackupDBFailed", "O Backup do banco de dados falhou") + ": " + ex.Message);
				}
			}
		}

		private void menu_dbRestore_Click(object sender, EventArgs e)
		{
			try
			{
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("data");
				OpenFileDialog openFileDialog = new OpenFileDialog();
				if (nodeValueByName.ToLower() == "access")
				{
					openFileDialog.Filter = "MDB files (*.mdb)|*.mdb";
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						string fileName = openFileDialog.FileName;
						string empty = string.Empty;
						string connectionString = MainForm._ia.ConnectionString;
						if (!(connectionString == ""))
						{
							DataHelper.GetAccessConnectStringInfo(connectionString, ref empty);
							if (fileName == empty)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRestoreDBFileWrong", "Restaurar erro de seleção de arquivo de banco de dados,em uso, não pode ser restaurado"));
								goto end_IL_0001;
							}
						}
						AppSite.Instance.SetNodeValue("IsRestoreDB", "1");
						AppSite.Instance.SetNodeValue("RestoreDBPath", fileName);
						OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SRestoreDB", "Restauração de Banco de Dados"), 5, "system");
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "Operação bem sucedida. Deseja reiniciar o software para torná-lo efetivo?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							this.ReStart();
						}
					}
				}
				if (nodeValueByName.ToLower() == "sqlserver")
				{
					openFileDialog.Filter = "MDB files (*.bak)|*.bak";
					if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
						string fileName = openFileDialog.FileName;
						AppSite.Instance.SetNodeValue("IsRestoreDB", "1");
						AppSite.Instance.SetNodeValue("RestoreDBPath", fileName);
						OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SRestoreDB", "Banco de dados restaurado"), 5, "system");
						if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "Operação bem sucedida. Deseja reiniciar o software para torná-lo efetivo?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							this.ReStart();
						}
					}
				}
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void menu_sysParamSet_Click(object sender, EventArgs e)
		{
			Program.IsRestart = false;
			SetParameterForm setParameterForm = new SetParameterForm();
			setParameterForm.ShowDialog();
			if (Program.IsRestart)
			{
				MainForm._ia.MainFrom.Close();
			}
		}

		private void AreaSetting_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			AreaSettingUserControl areaSettingUserControl = new AreaSettingUserControl();
			this.switchover(areaSettingUserControl);
			areaSettingUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			areaSettingUserControl.panelEx1.Parent = this.pnl_type;
			areaSettingUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void SearchAccessNode_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			this.Device_buttonItem_Click(null, null);
			SearchAccessForm searchAccessForm = new SearchAccessForm();
			searchAccessForm.ShowDialog();
			this.Device_buttonItem_Click(null, null);
			this.enbaled(sender, true);
		}

		private void node_doorLevels_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			LevelUserControl levelUserControl = new LevelUserControl();
			this.switchover(levelUserControl);
			levelUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			levelUserControl.pnl_userLevel.Parent = this.pnl_type;
			levelUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void InterlockNode_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			InterlockUC interlockUC = new InterlockUC();
			this.switchover(interlockUC);
			interlockUC.MenuPanelEx.Parent = this.MenuPanelEx;
			interlockUC.pnl_Interlock.Parent = this.pnl_type;
			interlockUC.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void antiNode_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			AntiUC antiUC = new AntiUC();
			this.switchover(antiUC);
			antiUC.MenuPanelEx.Parent = this.MenuPanelEx;
			antiUC.panelEx2.Parent = this.pnl_type;
			antiUC.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void LinkageNode_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			LinkageUC linkageUC = new LinkageUC();
			this.switchover(linkageUC);
			linkageUC.MenuPanelEx.Parent = this.MenuPanelEx;
			linkageUC.pnl_linkage.Parent = this.pnl_type;
			linkageUC.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void NormalOpenNode_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			NormalOpenUC normalOpenUC = new NormalOpenUC();
			this.switchover(normalOpenUC);
			normalOpenUC.MenupanelEx.Parent = this.MenuPanelEx;
			normalOpenUC.pnl_normalOpen.Parent = this.pnl_type;
			normalOpenUC.MenupanelEx.Show();
			this.enbaled(sender, true);
		}

		private void MenupanelExNode_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			Application.DoEvents();
			this.enbaled(sender, false);
			MultiCardOpenUC multiCardOpenUC = new MultiCardOpenUC();
			this.switchover(multiCardOpenUC);
			multiCardOpenUC.MenupanelEx.Parent = this.MenuPanelEx;
			multiCardOpenUC.pnl_multiCard.Parent = this.pnl_type;
			multiCardOpenUC.MenupanelEx.Show();
			this.enbaled(sender, true);
		}

		private void node_map_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			MapUC mapUC = new MapUC();
			this.switchover(mapUC);
			mapUC.MenuPanelEx.Parent = this.MenuPanelEx;
			mapUC.pnl_doorStatus.Parent = this.pnl_type;
			mapUC.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void node_RecordesToday_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			ReportsUserControlAll reportsUserControlAll = new ReportsUserControlAll("day");
			this.switchover(reportsUserControlAll);
			reportsUserControlAll.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControlAll.pnl_reports.Parent = this.pnl_type;
			reportsUserControlAll.MenuPanelEx.Show();
		}

		private void node_RecordsOfHalfWeek_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			Program.IsRegistZKECardTong();
			ReportsUserControlAll reportsUserControlAll = new ReportsUserControlAll("halfweek");
			this.switchover(reportsUserControlAll);
			reportsUserControlAll.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControlAll.pnl_reports.Parent = this.pnl_type;
			reportsUserControlAll.MenuPanelEx.Show();
		}

		private void node_recordsOfWeek_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			ReportsUserControlAll reportsUserControlAll = new ReportsUserControlAll("week");
			this.switchover(reportsUserControlAll);
			reportsUserControlAll.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControlAll.pnl_reports.Parent = this.pnl_type;
			reportsUserControlAll.MenuPanelEx.Show();
		}

		private void node_recordsOfLastWeek_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			ReportsUserControlAll reportsUserControlAll = new ReportsUserControlAll("lastweek");
			this.switchover(reportsUserControlAll);
			reportsUserControlAll.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControlAll.pnl_reports.Parent = this.pnl_type;
			reportsUserControlAll.MenuPanelEx.Show();
		}

		private void node_ExptRecords_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			Program.IsRegistZKECardTong();
			ReportsUserControlAll reportsUserControlAll = new ReportsUserControlAll("exception");
			this.switchover(reportsUserControlAll);
			reportsUserControlAll.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControlAll.pnl_reports.Parent = this.pnl_type;
			reportsUserControlAll.MenuPanelEx.Show();
		}

		private void Menu_help_Click(object sender, EventArgs e)
		{
			try
			{
				string text = (!(initLang.Lang == "chs")) ? ((!(initLang.Lang == "pt")) ? ((!(initLang.Lang == "pt_br")) ? (Path.GetDirectoryName(Application.ExecutablePath) + "\\Help_en.chm") : (Path.GetDirectoryName(Application.ExecutablePath) + "\\Help_pt.chm")) : (Path.GetDirectoryName(Application.ExecutablePath) + "\\Help_pt.chm")) : (Path.GetDirectoryName(Application.ExecutablePath) + "\\Help.chm");
				string text2 = Path.GetDirectoryName(Application.ExecutablePath) + "\\help\\help.html";
				if (File.Exists(text2))
				{
					text = text2;
				}
				if (File.Exists(text))
				{
					Process.Start(text);
				}
			}
			catch
			{
			}
		}

		private void issueCard_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			IssueCardUserControl issueCardUserControl = new IssueCardUserControl();
			this.switchover(issueCardUserControl);
			issueCardUserControl.MenuPanelEx.Parent = this.MenuPanelEx;
			issueCardUserControl.pnl_issueCard.Parent = this.pnl_type;
			issueCardUserControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void Department_buttonItem_Click(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			DepartmentForm departmentForm = new DepartmentForm();
			this.switchover(departmentForm);
			departmentForm.MenuPanelEx.Parent = this.MenuPanelEx;
			departmentForm.pnl_dept.Parent = this.pnl_type;
			departmentForm.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void node_AllRecords_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			ReportsUserControlAll reportsUserControlAll = new ReportsUserControlAll("day");
			this.switchover(reportsUserControlAll);
			reportsUserControlAll.MenuPanelEx.Parent = this.MenuPanelEx;
			reportsUserControlAll.pnl_reports.Parent = this.pnl_type;
			reportsUserControlAll.MenuPanelEx.Show();
		}

		private void menu_GenerateLangFile_Click(object sender, EventArgs e)
		{
			FileStream fileStream = null;
			StreamWriter streamWriter = null;
			try
			{
				InAddressInfo.Load();
				InOutStateInfo.Load();
				LinkAgeIOInfo.Load();
				OutAddressInfo.Load();
				PullSDkErrorInfos.Load();
				PullSDKEventInfos.Load();
				PullSDKVerifyTypeInfos.Load();
				ShowMsgInfos.Load();
				STD_CardMode.Load();
				STD_DateFormat.Load();
				STD_FpVersion.Load();
				STD_FreeType.Load();
				STD_Language.Load();
				STD_WiegandInOutContent.Load();
				STDWiegandFmtOut.Load();
				new XtraEditors_CN().InitialLocalString();
				new XtraGrid_CN().InitialLocalString();
				new XtraPrinting_CN().InitialLocalString();
				initLang.GenerateLanguageFile(Assembly.GetExecutingAssembly().GetTypes(), "..\\.", out Dictionary<string, ConstructorInfo> dictionary);
				if (File.Exists("0GLF_Ignored.txt"))
				{
					File.Delete("0GLF_Ignored.txt");
				}
				fileStream = File.Open("0GLF_Ignored.txt", FileMode.CreateNew);
				streamWriter = new StreamWriter(fileStream);
				foreach (KeyValuePair<string, ConstructorInfo> item in dictionary)
				{
					streamWriter.WriteLine(item.Key + "\t" + item.Value.ToString());
				}
				streamWriter.Flush();
				fileStream.Flush();
				streamWriter.Close();
				fileStream.Close();
				streamWriter = null;
				fileStream = null;
			}
			catch (Exception ex)
			{
				if (streamWriter != null)
				{
					streamWriter.Flush();
					streamWriter.Close();
					streamWriter = null;
				}
				if (fileStream != null)
				{
					fileStream.Flush();
					fileStream.Close();
					fileStream = null;
				}
				SysDialogs.ShowErrorMessage(ex.Message);
			}
			SysDialogs.ShowInfoMessage("Concluído");
		}

		private void node_reader_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			ReaderControl readerControl = new ReaderControl();
			this.switchover(readerControl);
			readerControl.MenuPanelEx.Parent = this.MenuPanelEx;
			readerControl.panelEx2.Parent = this.pnl_type;
			readerControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void node_auxiliary_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			AuxInOutControl auxInOutControl = new AuxInOutControl();
			this.switchover(auxInOutControl);
			auxInOutControl.MenuPanelEx.Parent = this.MenuPanelEx;
			auxInOutControl.panelEx2.Parent = this.pnl_type;
			auxInOutControl.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void mnu_reader_Click(object sender, EventArgs e)
		{
			this.node_reader_NodeClick(sender, e);
		}

		private void mnu_auxiliary_Click(object sender, EventArgs e)
		{
			this.node_auxiliary_NodeClick(sender, e);
		}

		private void StartDownLoadTimer()
		{
			if (this.tmrDownLoadRecord != null)
			{
				this.tmrDownLoadRecord.Enabled = false;
				this.tmrDownLoadRecord.Dispose();
			}
			this.tmrDownLoadRecord = new System.Timers.Timer();
			this.tmrDownLoadRecord.AutoReset = true;
			this.tmrDownLoadRecord.Interval = 10000.0;
			this.tmrDownLoadRecord.SynchronizingObject = this;
			this.tmrDownLoadRecord.Elapsed += this.tmrDownLoadRecord_Elapsed;
			this.tmrDownLoadRecord.Enabled = true;
		}

		private void tmrDownLoadRecord_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (Monitor.TryEnter(this.DownLoadLock, 1))
			{
				try
				{
					DateTime now = DateTime.Now;
					string nodeValueByName = AppSite.Instance.GetNodeValueByName("TimeDownload");
					if (nodeValueByName != null && "" != nodeValueByName.Trim() && DateTime.TryParse(now.ToString("yyyy-MM-dd") + " " + nodeValueByName, out DateTime dateTime))
					{
						if (!this.LastDownloadTime.HasValue)
						{
							if (now.ToString("yyyy-MM-dd HH:mm") == dateTime.ToString("yyyy-MM-dd HH:mm"))
							{
								this.LastDownloadTime = now;
								this.DownloadRecord();
							}
						}
						else if (this.LastDownloadTime.Value.ToString("yyyy-MM-dd") != now.ToString("yyyy-MM-dd") && now.ToString("yyyy-MM-dd HH:mm") == dateTime.ToString("yyyy-MM-dd HH:mm"))
						{
							this.LastDownloadTime = now;
							this.DownloadRecord();
						}
						else if (now.ToString("yyyy-MM-dd HH:mm") != dateTime.ToString("yyyy-MM-dd HH:mm"))
						{
							this.LastDownloadTime = null;
						}
					}
				}
				catch (Exception ex)
				{
					SysLogServer.WriteLog("Exception on download record on timing: " + ex.Message);
				}
				finally
				{
					Monitor.Exit(this.DownLoadLock);
				}
			}
		}

		private void DownloadRecord()
		{
			if (!(this?.IsDisposed ?? true))
			{
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.DownloadRecord();
					});
				}
				else
				{
					DownLoadRecordForm downLoadRecordForm = new DownLoadRecordForm(true);
					downLoadRecordForm.ShowDialog();
				}
			}
		}

		public void SwitchPage(PageIdEnum id)
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.SwitchPage(id);
				});
			}
			else
			{
				switch (id)
				{
				case PageIdEnum.AccessLevel:
					this.node_doorLevels_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_doorLevels, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.AntiBack:
					this.antiNode_NodeClick(null, null);
					this.advTree1.SelectNode(this.Node_anti, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.Area:
					this.AreaSetting_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_AreaSetting, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.AuxiliarySetting:
					this.node_auxiliary_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_auxiliary, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.Department:
					this.Department_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_dept, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.Device:
					this.menuItem_device.PerformClick();
					this.advTree1.SelectNode(this.Node_Device2, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.DoorSetting:
					this.doorsettinbtn_Click(null, null);
					this.advTree1.SelectNode(this.Node_doorsetting, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.FirstCard:
					this.NormalOpenNode_NodeClick(null, null);
					this.advTree1.SelectNode(this.Node_NormalOpen, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.Holiday:
					this.Holidays_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_Holidays, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.InterLock:
					this.InterlockNode_NodeClick(null, null);
					this.advTree1.SelectNode(this.Node_Interlock, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.IssueCardList:
					this.issueCard_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_IssueCard, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.Linkage:
					this.LinkageNode_NodeClick(null, null);
					this.advTree1.SelectNode(this.Node_Linkage, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.MultiCard:
					this.MenupanelExNode_NodeClick(null, null);
					this.advTree1.SelectNode(this.Node_MultiCardOpen, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.Navigation:
					this.menu_navigation_Click(null, null);
					this.advTree1.SelectNode(this.Node_Navigation, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.ReaderSetting:
					this.node_reader_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_reader, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.RealTimeMonitor:
					this.Monitoring_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_Monitoring, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.ReportException:
					this.node_ExptRecords_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_ExptRecords, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.ReportLastSevenDay:
					this.node_recordsOfWeek_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_recordsOfWeek, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.ReportLastThreeDay:
					this.node_RecordsOfHalfWeek_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_RecordsOfHalfWeek, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.ReportLastWeek:
					this.node_recordsOfLastWeek_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_recordsOfLastWeek, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.ReportToday:
					this.node_RecordesToday_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_RecordesToday, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.SearchDevice:
					this.SearchAccessNode_NodeClick(null, null);
					this.advTree1.SelectNode(this.Node_SearchAccess, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.TimeZone:
					this.TimeSlot_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_TimeSlot, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.UserList:
					this.personnel_buttonItem_Click(null, null);
					this.advTree1.SelectNode(this.Node_Employee, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.VirtueMap:
					this.node_map_NodeClick(null, null);
					this.advTree1.SelectNode(this.node_map, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				case PageIdEnum.WiegandFormat:
					this.menuItem_wiegand_Click(null, null);
					this.advTree1.SelectNode(this.node_wiegand, eTreeAction.Mouse);
					this.advTree1.Focus();
					break;
				}
			}
		}

		private void Navigation_NodeClick(object sender, EventArgs e)
		{
			this.SwitchPage(PageIdEnum.Navigation);
		}

		private void AttNode_NodeClick(object sender, EventArgs e)
		{
			Program.SendMessage(10000u, 1, 0);
		}

		private void mnu_AttReport_Click(object sender, EventArgs e)
		{
			this.nodeAttReport_NodeClick(null, null);
		}

		private void nodeAttReport_NodeClick(object sender, EventArgs e)
		{
			string empty = string.Empty;
			empty = Application.StartupPath + "\\ZKTimeNet.UI.exe";
			if (File.Exists(empty))
			{
				Process process = new Process();
				process.StartInfo.FileName = empty;
				process.Start();
			}
		}

		private void nodeCustomReport_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			CustomReportList customReportList = new CustomReportList();
			this.switchover(customReportList);
			customReportList.MenuPanelEx.Parent = this.MenuPanelEx;
			customReportList.pnl_CustomReport.Parent = this.pnl_type;
			customReportList.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void mnuCustomReport_Click(object sender, EventArgs e)
		{
			this.nodeCustomReport_NodeClick(null, null);
		}

		private void node_VisitoM_NodeClick(object sender, EventArgs e)
		{
			if (sender is Node && !(sender as Node).Enabled)
			{
				return;
			}
			this.enbaled(sender, false);
			Program.IsRegistZKECardTong();
			Visitor2Form visitor2Form = new Visitor2Form();
			this.switchover(visitor2Form);
			visitor2Form.MenuPanelEx.Parent = this.MenuPanelEx;
			visitor2Form.MenuPanelEx.Show();
			this.enbaled(sender, true);
		}

		private void menu_Main_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
		}

		private void mnu_optmodules_Click(object sender, EventArgs e)
		{
			FrmOptionalModule frmOptionalModule = new FrmOptionalModule();
			frmOptionalModule.ShowDialog();
		}

		private void mnu_optmodules_Click_1(object sender, EventArgs e)
		{
			FrmOptionalModule frmOptionalModule = new FrmOptionalModule();
			frmOptionalModule.ShowDialog();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserMainControl));
			this.statusLabel1 = new ToolStripStatusLabel();
			this.lb_Opstatus = new ToolStripStatusLabel();
			this.btn_Operation = new ButtonX();
			this.panelEx3 = new PanelEx();
			this.top_StripPanel = new ToolStripPanel();
			this.elementStyle1 = new ElementStyle();
			this.LeftExplorerBar = new ExplorerBar();
			this.advTree1 = new AdvTree();
			this.node_Personnel = new Node();
			this.Node_dept = new Node();
			this.Node_Employee = new Node();
			this.Node_IssueCard = new Node();
			this.node_visitors = new Node();
			this.node_Device = new Node();
			this.Node_AreaSetting = new Node();
			this.Node_Device2 = new Node();
			this.Node_SearchAccess = new Node();
			this.node_AccessControl = new Node();
			this.Node_TimeSlot = new Node();
			this.Node_Holidays = new Node();
			this.Node_doorsetting = new Node();
			this.node_doorLevels = new Node();
			this.node_wiegand = new Node();
			this.Node_Interlock = new Node();
			this.Node_anti = new Node();
			this.Node_Linkage = new Node();
			this.Node_NormalOpen = new Node();
			this.Node_MultiCardOpen = new Node();
			this.Node_Monitoring = new Node();
			this.node_map = new Node();
			this.node_reader = new Node();
			this.node_auxiliary = new Node();
			this.node_VisitoM = new Node();
			this.node_RecordsReport = new Node();
			this.node_RecordesToday = new Node();
			this.node_RecordsOfHalfWeek = new Node();
			this.node_recordsOfWeek = new Node();
			this.node_recordsOfLastWeek = new Node();
			this.node_ExptRecords = new Node();
			this.nodeCustomReport = new Node();
			this.nodeConnector1 = new NodeConnector();
			this.expandableSplitter1 = new ExpandableSplitter();
			this.tmr_timeShow = new System.Windows.Forms.Timer(this.components);
			this.pnl_type = new PanelEx();
			this.MenuPanelEx = new PanelEx();
			this.MainPanelEx = new PanelEx();
			this.node1 = new Node();
			this.menuItem_sysset = new ToolStripMenuItem();
			this.menuItem_role = new ToolStripMenuItem();
			this.menuItem_sysuer = new ToolStripMenuItem();
			this.menu_modifyPwd = new ToolStripMenuItem();
			this.menu_initSystem = new ToolStripMenuItem();
			this.menu_dbManage = new ToolStripMenuItem();
			this.menu_DatabaseSet = new ToolStripMenuItem();
			this.menu_dbBackup = new ToolStripMenuItem();
			this.menu_dbRestore = new ToolStripMenuItem();
			this.menu_DBBakPath = new ToolStripMenuItem();
			this.menu_sysParamSet = new ToolStripMenuItem();
			this.menu_navigation = new ToolStripMenuItem();
			this.menu_selectlan = new ToolStripMenuItem();
			this.mnu_optmodules = new ToolStripMenuItem();
			this.menu_exit = new ToolStripMenuItem();
			this.menuItem_sysuser = new ToolStripMenuItem();
			this.menuItem_dept = new ToolStripMenuItem();
			this.menuItem_user = new ToolStripMenuItem();
			this.menuItem_issueCard = new ToolStripMenuItem();
			this.menuItem_ToolDvice = new ToolStripMenuItem();
			this.menuItem_Area = new ToolStripMenuItem();
			this.menuItem_device = new ToolStripMenuItem();
			this.menuItem_SearchAccess = new ToolStripMenuItem();
			this.menuItem_AccessControl = new ToolStripMenuItem();
			this.menuItem_TimeSlot = new ToolStripMenuItem();
			this.menuItem_Holidays = new ToolStripMenuItem();
			this.menuItem_doorsetting = new ToolStripMenuItem();
			this.menuItem_Levels = new ToolStripMenuItem();
			this.menuItem_wiegand = new ToolStripMenuItem();
			this.menuItem_Interlock = new ToolStripMenuItem();
			this.menuItem_anti = new ToolStripMenuItem();
			this.menuItem_Linkage = new ToolStripMenuItem();
			this.menuItem_NormalOpen = new ToolStripMenuItem();
			this.menuItem_MultiCardOpen = new ToolStripMenuItem();
			this.menuItem_Monitoring = new ToolStripMenuItem();
			this.menuItem_map = new ToolStripMenuItem();
			this.mnu_reader = new ToolStripMenuItem();
			this.mnu_auxiliary = new ToolStripMenuItem();
			this.menuItem_RecordsReport = new ToolStripMenuItem();
			this.menuItem_RecordesToday = new ToolStripMenuItem();
			this.menuItem_RecordesOfThreeDays = new ToolStripMenuItem();
			this.menuItem_RecordsOfWeek = new ToolStripMenuItem();
			this.menuItem_recordsOfLastWeek = new ToolStripMenuItem();
			this.menuItem_ExptRecords = new ToolStripMenuItem();
			this.mnuCustomReport = new ToolStripMenuItem();
			this.menuItem_Help = new ToolStripMenuItem();
			this.Menu_help = new ToolStripMenuItem();
			this.menuItem_About = new ToolStripMenuItem();
			this.menu_GenerateLangFile = new ToolStripMenuItem();
			this.menu_Main = new MenuStrip();
			this.panelEx3.SuspendLayout();
			((ISupportInitialize)this.LeftExplorerBar).BeginInit();
			this.LeftExplorerBar.SuspendLayout();
			((ISupportInitialize)this.advTree1).BeginInit();
			this.menu_Main.SuspendLayout();
			base.SuspendLayout();
			this.statusLabel1.Name = "statusLabel1";
			this.statusLabel1.Size = new Size(41, 17);
			this.statusLabel1.Text = "状态：";
			this.lb_Opstatus.Name = "lb_Opstatus";
			this.lb_Opstatus.Size = new Size(11, 17);
			this.lb_Opstatus.Text = " ";
			this.btn_Operation.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Operation.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.btn_Operation.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Operation.Location = new Point(8, 3);
			this.btn_Operation.Name = "btn_Operation";
			this.btn_Operation.Size = new Size(75, 25);
			this.btn_Operation.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Operation.TabIndex = 0;
			this.btn_Operation.Text = "操作";
			this.btn_Operation.Click += this.Operation_buttonX_Click;
			this.panelEx3.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.panelEx3.CanvasColor = SystemColors.Control;
			this.panelEx3.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx3.Controls.Add(this.btn_Operation);
			this.panelEx3.Location = new Point(-3, 523);
			this.panelEx3.Name = "panelEx3";
			this.panelEx3.Size = new Size(175, 31);
			this.panelEx3.Style.Alignment = StringAlignment.Center;
			this.panelEx3.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx3.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx3.Style.Border = eBorderType.SingleLine;
			this.panelEx3.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx3.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx3.Style.GradientAngle = 90;
			this.panelEx3.TabIndex = 21;
			this.top_StripPanel.Dock = DockStyle.Top;
			this.top_StripPanel.Location = new Point(0, 24);
			this.top_StripPanel.Name = "top_StripPanel";
			this.top_StripPanel.Orientation = Orientation.Horizontal;
			this.top_StripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.top_StripPanel.Size = new Size(838, 0);
			this.elementStyle1.Class = "";
			this.elementStyle1.Name = "elementStyle1";
			this.elementStyle1.TextColor = SystemColors.ControlText;
			this.LeftExplorerBar.AccessibleRole = AccessibleRole.ToolBar;
			this.LeftExplorerBar.BackColor = SystemColors.Control;
			this.LeftExplorerBar.BackStyle.BackColor2SchemePart = eColorSchemePart.ExplorerBarBackground2;
			this.LeftExplorerBar.BackStyle.BackColorGradientAngle = 90;
			this.LeftExplorerBar.BackStyle.BackColorSchemePart = eColorSchemePart.ExplorerBarBackground;
			this.LeftExplorerBar.BackStyle.Class = "";
			this.LeftExplorerBar.Controls.Add(this.advTree1);
			this.LeftExplorerBar.Cursor = Cursors.Default;
			this.LeftExplorerBar.Dock = DockStyle.Left;
			this.LeftExplorerBar.GroupImages = null;
			this.LeftExplorerBar.Images = null;
			this.LeftExplorerBar.Location = new Point(0, 24);
			this.LeftExplorerBar.Name = "LeftExplorerBar";
			this.LeftExplorerBar.Size = new Size(175, 556);
			this.LeftExplorerBar.StockStyle = eExplorerBarStockStyle.SystemColors;
			this.LeftExplorerBar.TabIndex = 27;
			this.LeftExplorerBar.Text = "explorerBar2";
			this.LeftExplorerBar.ThemeAware = true;
			this.advTree1.AccessibleRole = AccessibleRole.Outline;
			this.advTree1.AllowDrop = true;
			this.advTree1.BackColor = Color.White;
			this.advTree1.BackgroundStyle.Class = "TreeBorderKey";
			this.advTree1.Dock = DockStyle.Fill;
			this.advTree1.DragDropEnabled = false;
			this.advTree1.ForeColor = Color.FromArgb(175, 210, 255);
			this.advTree1.Location = new Point(0, 0);
			this.advTree1.Name = "advTree1";
			this.advTree1.Nodes.AddRange(new Node[4]
			{
				this.node_Personnel,
				this.node_Device,
				this.node_AccessControl,
				this.node_RecordsReport
			});
			this.advTree1.NodesConnector = this.nodeConnector1;
			this.advTree1.NodeStyle = this.elementStyle1;
			this.advTree1.PathSeparator = ";";
			this.advTree1.Size = new Size(175, 556);
			this.advTree1.TabIndex = 0;
			this.advTree1.Text = "advTree1";
			this.node_Personnel.Expanded = true;
			this.node_Personnel.Image = (Image)componentResourceManager.GetObject("node_Personnel.Image");
			this.node_Personnel.Name = "node_Personnel";
			this.node_Personnel.Nodes.AddRange(new Node[4]
			{
				this.Node_dept,
				this.Node_Employee,
				this.Node_IssueCard,
				this.node_visitors
			});
			this.node_Personnel.Text = "人事";
			this.node_Personnel.NodeClick += this.personnel_buttonItem_Click;
			this.Node_dept.Expanded = true;
			this.Node_dept.Image = (Image)componentResourceManager.GetObject("Node_dept.Image");
			this.Node_dept.Name = "Node_dept";
			this.Node_dept.Text = "部门";
			this.Node_dept.NodeClick += this.Department_buttonItem_Click;
			this.Node_Employee.Image = (Image)componentResourceManager.GetObject("Node_Employee.Image");
			this.Node_Employee.Name = "Node_Employee";
			this.Node_Employee.Text = "人员";
			this.Node_Employee.NodeClick += this.personnel_buttonItem_Click;
			this.Node_IssueCard.Image = (Image)componentResourceManager.GetObject("Node_IssueCard.Image");
			this.Node_IssueCard.Name = "Node_IssueCard";
			this.Node_IssueCard.Text = "人员发卡";
			this.Node_IssueCard.NodeClick += this.issueCard_buttonItem_Click;
			this.node_visitors.Expanded = true;
			this.node_visitors.Image = Resources.Adjust_Department;
			this.node_visitors.Name = "node_visitors";
			this.node_visitors.Text = "Visitantes";
			this.node_visitors.NodeClick += this.node_VisitoM_NodeClick;
			this.node_Device.Expanded = true;
			this.node_Device.Image = (Image)componentResourceManager.GetObject("node_Device.Image");
			this.node_Device.Name = "node_Device";
			this.node_Device.Nodes.AddRange(new Node[3]
			{
				this.Node_AreaSetting,
				this.Node_Device2,
				this.Node_SearchAccess
			});
			this.node_Device.Text = "设备";
			this.node_Device.NodeClick += this.Device_buttonItem_Click;
			this.Node_AreaSetting.Image = (Image)componentResourceManager.GetObject("Node_AreaSetting.Image");
			this.Node_AreaSetting.Name = "Node_AreaSetting";
			this.Node_AreaSetting.Text = "区域";
			this.Node_AreaSetting.NodeClick += this.AreaSetting_buttonItem_Click;
			this.Node_Device2.Expanded = true;
			this.Node_Device2.Image = (Image)componentResourceManager.GetObject("Node_Device2.Image");
			this.Node_Device2.Name = "Node_Device2";
			this.Node_Device2.Text = "设备";
			this.Node_Device2.NodeClick += this.Device_buttonItem_Click;
			this.Node_SearchAccess.Expanded = true;
			this.Node_SearchAccess.Image = (Image)componentResourceManager.GetObject("Node_SearchAccess.Image");
			this.Node_SearchAccess.Name = "Node_SearchAccess";
			this.Node_SearchAccess.Text = "搜索门禁控制器";
			this.Node_SearchAccess.NodeClick += this.SearchAccessNode_NodeClick;
			this.node_AccessControl.Expanded = true;
			this.node_AccessControl.Image = (Image)componentResourceManager.GetObject("node_AccessControl.Image");
			this.node_AccessControl.Name = "node_AccessControl";
			this.node_AccessControl.Nodes.AddRange(new Node[15]
			{
				this.Node_TimeSlot,
				this.Node_Holidays,
				this.Node_doorsetting,
				this.node_doorLevels,
				this.node_wiegand,
				this.Node_Interlock,
				this.Node_anti,
				this.Node_Linkage,
				this.Node_NormalOpen,
				this.Node_MultiCardOpen,
				this.Node_Monitoring,
				this.node_map,
				this.node_reader,
				this.node_auxiliary,
				this.node_VisitoM
			});
			this.node_AccessControl.Text = "门禁";
			this.node_AccessControl.NodeClick += this.Monitoring_buttonItem_Click;
			this.Node_TimeSlot.Expanded = true;
			this.Node_TimeSlot.Image = (Image)componentResourceManager.GetObject("Node_TimeSlot.Image");
			this.Node_TimeSlot.Name = "Node_TimeSlot";
			this.Node_TimeSlot.Text = "门禁时间段";
			this.Node_TimeSlot.NodeClick += this.TimeSlot_buttonItem_Click;
			this.Node_Holidays.Expanded = true;
			this.Node_Holidays.Image = (Image)componentResourceManager.GetObject("Node_Holidays.Image");
			this.Node_Holidays.Name = "Node_Holidays";
			this.Node_Holidays.Text = "门禁节假日";
			this.Node_Holidays.NodeClick += this.Holidays_buttonItem_Click;
			this.Node_doorsetting.Expanded = true;
			this.Node_doorsetting.Image = (Image)componentResourceManager.GetObject("Node_doorsetting.Image");
			this.Node_doorsetting.Name = "Node_doorsetting";
			this.Node_doorsetting.Text = "门设置";
			this.Node_doorsetting.NodeClick += this.doorsettinbtn_Click;
			this.node_doorLevels.Expanded = true;
			this.node_doorLevels.Image = (Image)componentResourceManager.GetObject("node_doorLevels.Image");
			this.node_doorLevels.Name = "node_doorLevels";
			this.node_doorLevels.Text = "门禁权限组";
			this.node_doorLevels.NodeClick += this.node_doorLevels_NodeClick;
			this.node_wiegand.Image = Resources.Real_Time;
			this.node_wiegand.Name = "node_wiegand";
			this.node_wiegand.Text = "韦根格式";
			this.node_wiegand.NodeClick += this.menuItem_wiegand_Click;
			this.Node_Interlock.Expanded = true;
			this.Node_Interlock.Image = (Image)componentResourceManager.GetObject("Node_Interlock.Image");
			this.Node_Interlock.Name = "Node_Interlock";
			this.Node_Interlock.Text = "互锁";
			this.Node_Interlock.NodeClick += this.InterlockNode_NodeClick;
			this.Node_anti.Expanded = true;
			this.Node_anti.Image = (Image)componentResourceManager.GetObject("Node_anti.Image");
			this.Node_anti.Name = "Node_anti";
			this.Node_anti.Text = "反潜";
			this.Node_anti.NodeClick += this.antiNode_NodeClick;
			this.Node_Linkage.Expanded = true;
			this.Node_Linkage.Image = (Image)componentResourceManager.GetObject("Node_Linkage.Image");
			this.Node_Linkage.Name = "Node_Linkage";
			this.Node_Linkage.Text = "联动";
			this.Node_Linkage.NodeClick += this.LinkageNode_NodeClick;
			this.Node_NormalOpen.Expanded = true;
			this.Node_NormalOpen.Image = (Image)componentResourceManager.GetObject("Node_NormalOpen.Image");
			this.Node_NormalOpen.Name = "Node_NormalOpen";
			this.Node_NormalOpen.Text = "首卡常开";
			this.Node_NormalOpen.NodeClick += this.NormalOpenNode_NodeClick;
			this.Node_MultiCardOpen.Expanded = true;
			this.Node_MultiCardOpen.Image = (Image)componentResourceManager.GetObject("Node_MultiCardOpen.Image");
			this.Node_MultiCardOpen.Name = "Node_MultiCardOpen";
			this.Node_MultiCardOpen.Text = "多卡开门";
			this.Node_MultiCardOpen.NodeClick += this.MenupanelExNode_NodeClick;
			this.Node_Monitoring.Expanded = true;
			this.Node_Monitoring.Image = (Image)componentResourceManager.GetObject("Node_Monitoring.Image");
			this.Node_Monitoring.Name = "Node_Monitoring";
			this.Node_Monitoring.Text = "实时监控";
			this.Node_Monitoring.NodeClick += this.Monitoring_buttonItem_Click;
			this.node_map.Expanded = true;
			this.node_map.Image = (Image)componentResourceManager.GetObject("node_map.Image");
			this.node_map.Name = "node_map";
			this.node_map.Text = "电子地图";
			this.node_map.NodeClick += this.node_map_NodeClick;
			this.node_reader.Expanded = true;
			this.node_reader.Image = Resources.Reader;
			this.node_reader.Name = "node_reader";
			this.node_reader.Text = "读头设置";
			this.node_reader.NodeClick += this.node_reader_NodeClick;
			this.node_auxiliary.Expanded = true;
			this.node_auxiliary.Image = Resources.Auxiliary;
			this.node_auxiliary.Name = "node_auxiliary";
			this.node_auxiliary.Text = "辅助输入输出";
			this.node_auxiliary.NodeClick += this.node_auxiliary_NodeClick;
			this.node_VisitoM.Image = Resources.Import;
			this.node_VisitoM.Name = "node_VisitoM";
			this.node_VisitoM.Text = "访客";
			this.node_VisitoM.Visible = false;
			this.node_VisitoM.NodeClick += this.node_VisitoM_NodeClick;
			this.node_RecordsReport.Expanded = true;
			this.node_RecordsReport.Image = (Image)componentResourceManager.GetObject("node_RecordsReport.Image");
			this.node_RecordsReport.Name = "node_RecordsReport";
			this.node_RecordsReport.Nodes.AddRange(new Node[6]
			{
				this.node_RecordesToday,
				this.node_RecordsOfHalfWeek,
				this.node_recordsOfWeek,
				this.node_recordsOfLastWeek,
				this.node_ExptRecords,
				this.nodeCustomReport
			});
			this.node_RecordsReport.Text = "报表";
			this.node_RecordsReport.NodeClick += this.node_AllRecords_NodeClick;
			this.node_RecordesToday.Expanded = true;
			this.node_RecordesToday.Image = (Image)componentResourceManager.GetObject("node_RecordesToday.Image");
			this.node_RecordesToday.Name = "node_RecordesToday";
			this.node_RecordesToday.Text = "当天门禁事件";
			this.node_RecordesToday.NodeClick += this.node_RecordesToday_NodeClick;
			this.node_RecordsOfHalfWeek.Image = (Image)componentResourceManager.GetObject("node_RecordsOfHalfWeek.Image");
			this.node_RecordsOfHalfWeek.Name = "node_RecordsOfHalfWeek";
			this.node_RecordsOfHalfWeek.Text = "最近三天门禁事件";
			this.node_RecordsOfHalfWeek.NodeClick += this.node_RecordsOfHalfWeek_NodeClick;
			this.node_recordsOfWeek.Image = (Image)componentResourceManager.GetObject("node_recordsOfWeek.Image");
			this.node_recordsOfWeek.Name = "node_recordsOfWeek";
			this.node_recordsOfWeek.Text = "最近一周门禁事件";
			this.node_recordsOfWeek.NodeClick += this.node_recordsOfWeek_NodeClick;
			this.node_recordsOfLastWeek.Expanded = true;
			this.node_recordsOfLastWeek.Image = (Image)componentResourceManager.GetObject("node_recordsOfLastWeek.Image");
			this.node_recordsOfLastWeek.Name = "node_recordsOfLastWeek";
			this.node_recordsOfLastWeek.Text = "上周门禁事件";
			this.node_recordsOfLastWeek.NodeClick += this.node_recordsOfLastWeek_NodeClick;
			this.node_ExptRecords.Expanded = true;
			this.node_ExptRecords.Image = (Image)componentResourceManager.GetObject("node_ExptRecords.Image");
			this.node_ExptRecords.Name = "node_ExptRecords";
			this.node_ExptRecords.Text = "门禁异常事件";
			this.node_ExptRecords.NodeClick += this.node_ExptRecords_NodeClick;
			this.nodeCustomReport.Expanded = true;
			this.nodeCustomReport.Image = Resources.reports;
			this.nodeCustomReport.Name = "nodeCustomReport";
			this.nodeCustomReport.Text = "自定义报表";
			this.nodeCustomReport.NodeClick += this.nodeCustomReport_NodeClick;
			this.nodeConnector1.LineColor = SystemColors.ControlText;
			this.expandableSplitter1.BackColor2 = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.BackColor2SchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.BackColorSchemePart = eColorSchemePart.PanelBackground;
			this.expandableSplitter1.ExpandFillColor = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.ExpandFillColorSchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.ExpandLineColor = Color.FromArgb(0, 0, 0);
			this.expandableSplitter1.ExpandLineColorSchemePart = eColorSchemePart.ItemText;
			this.expandableSplitter1.GripDarkColor = Color.FromArgb(0, 0, 0);
			this.expandableSplitter1.GripDarkColorSchemePart = eColorSchemePart.ItemText;
			this.expandableSplitter1.GripLightColor = Color.FromArgb(227, 239, 255);
			this.expandableSplitter1.GripLightColorSchemePart = eColorSchemePart.BarBackground;
			this.expandableSplitter1.HotBackColor = Color.FromArgb(252, 151, 61);
			this.expandableSplitter1.HotBackColor2 = Color.FromArgb(255, 184, 94);
			this.expandableSplitter1.HotBackColor2SchemePart = eColorSchemePart.ItemPressedBackground2;
			this.expandableSplitter1.HotBackColorSchemePart = eColorSchemePart.ItemPressedBackground;
			this.expandableSplitter1.HotExpandFillColor = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.HotExpandFillColorSchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.HotExpandLineColor = Color.FromArgb(0, 0, 0);
			this.expandableSplitter1.HotExpandLineColorSchemePart = eColorSchemePart.ItemText;
			this.expandableSplitter1.HotGripDarkColor = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.HotGripDarkColorSchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.HotGripLightColor = Color.FromArgb(227, 239, 255);
			this.expandableSplitter1.HotGripLightColorSchemePart = eColorSchemePart.BarBackground;
			this.expandableSplitter1.Location = new Point(175, 24);
			this.expandableSplitter1.MinExtra = 900;
			this.expandableSplitter1.Name = "expandableSplitter1";
			this.expandableSplitter1.Size = new Size(6, 556);
			this.expandableSplitter1.Style = eSplitterStyle.Office2007;
			this.expandableSplitter1.TabIndex = 28;
			this.expandableSplitter1.TabStop = false;
			this.pnl_type.CanvasColor = SystemColors.Control;
			this.pnl_type.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_type.Dock = DockStyle.Top;
			this.pnl_type.Location = new Point(181, 24);
			this.pnl_type.Name = "pnl_type";
			this.pnl_type.Size = new Size(657, 25);
			this.pnl_type.Style.Alignment = StringAlignment.Center;
			this.pnl_type.Style.BackColor1.Color = Color.FromArgb(227, 239, 255);
			this.pnl_type.Style.BackColor2.Color = Color.FromArgb(175, 210, 255);
			this.pnl_type.Style.Border = eBorderType.SingleLine;
			this.pnl_type.Style.BorderColor.Color = Color.FromArgb(227, 239, 255);
			this.pnl_type.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_type.Style.GradientAngle = 90;
			this.pnl_type.TabIndex = 34;
			this.MenuPanelEx.CanvasColor = SystemColors.Control;
			this.MenuPanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MenuPanelEx.Dock = DockStyle.Top;
			this.MenuPanelEx.Location = new Point(181, 49);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(657, 41);
			this.MenuPanelEx.Style.Alignment = StringAlignment.Center;
			this.MenuPanelEx.Style.BackColor1.Color = Color.FromArgb(227, 239, 255);
			this.MenuPanelEx.Style.BackColor2.Color = Color.FromArgb(175, 210, 255);
			this.MenuPanelEx.Style.Border = eBorderType.SingleLine;
			this.MenuPanelEx.Style.BorderColor.Color = Color.FromArgb(227, 239, 255);
			this.MenuPanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenuPanelEx.Style.GradientAngle = 90;
			this.MenuPanelEx.TabIndex = 35;
			this.MainPanelEx.CanvasColor = SystemColors.Control;
			this.MainPanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MainPanelEx.Dock = DockStyle.Fill;
			this.MainPanelEx.Location = new Point(181, 90);
			this.MainPanelEx.Name = "MainPanelEx";
			this.MainPanelEx.Size = new Size(657, 490);
			this.MainPanelEx.Style.Alignment = StringAlignment.Center;
			this.MainPanelEx.Style.BackColor1.Color = Color.FromArgb(227, 239, 255);
			this.MainPanelEx.Style.BackColor2.Color = Color.FromArgb(175, 210, 255);
			this.MainPanelEx.Style.Border = eBorderType.SingleLine;
			this.MainPanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MainPanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MainPanelEx.Style.GradientAngle = 90;
			this.MainPanelEx.TabIndex = 36;
			this.node1.Name = "node1";
			this.menuItem_sysset.DropDownItems.AddRange(new ToolStripItem[10]
			{
				this.menuItem_role,
				this.menuItem_sysuer,
				this.menu_modifyPwd,
				this.menu_initSystem,
				this.menu_dbManage,
				this.menu_sysParamSet,
				this.menu_navigation,
				this.menu_selectlan,
				this.mnu_optmodules,
				this.menu_exit
			});
			this.menuItem_sysset.Image = (Image)componentResourceManager.GetObject("menuItem_sysset.Image");
			this.menuItem_sysset.Name = "menuItem_sysset";
			this.menuItem_sysset.Size = new Size(59, 20);
			this.menuItem_sysset.Text = "系统";
			this.menuItem_sysset.Click += this.menuItem_sysset_Click;
			this.menuItem_role.Image = (Image)componentResourceManager.GetObject("menuItem_role.Image");
			this.menuItem_role.Name = "menuItem_role";
			this.menuItem_role.Size = new Size(177, 22);
			this.menuItem_role.Text = "角色";
			this.menuItem_role.Click += this.Role_buttonItem_Click;
			this.menuItem_sysuer.Image = (Image)componentResourceManager.GetObject("menuItem_sysuer.Image");
			this.menuItem_sysuer.Name = "menuItem_sysuer";
			this.menuItem_sysuer.Size = new Size(177, 22);
			this.menuItem_sysuer.Text = "用户";
			this.menuItem_sysuer.Click += this.userBtn_Click;
			this.menu_modifyPwd.Image = (Image)componentResourceManager.GetObject("menu_modifyPwd.Image");
			this.menu_modifyPwd.Name = "menu_modifyPwd";
			this.menu_modifyPwd.Size = new Size(177, 22);
			this.menu_modifyPwd.Text = "修改密码";
			this.menu_modifyPwd.Click += this.menu_modifyPwd_Click;
			this.menu_initSystem.Image = (Image)componentResourceManager.GetObject("menu_initSystem.Image");
			this.menu_initSystem.Name = "menu_initSystem";
			this.menu_initSystem.Size = new Size(177, 22);
			this.menu_initSystem.Text = "系统初始化";
			this.menu_initSystem.Click += this.menu_initSystem_Click;
			this.menu_dbManage.DropDownItems.AddRange(new ToolStripItem[4]
			{
				this.menu_DatabaseSet,
				this.menu_dbBackup,
				this.menu_dbRestore,
				this.menu_DBBakPath
			});
			this.menu_dbManage.Image = (Image)componentResourceManager.GetObject("menu_dbManage.Image");
			this.menu_dbManage.Name = "menu_dbManage";
			this.menu_dbManage.Size = new Size(177, 22);
			this.menu_dbManage.Text = "数据库管理";
			this.menu_dbManage.MouseHover += this.menu_dbManage_MouseHover;
			this.menu_DatabaseSet.Image = Resources.set_database;
			this.menu_DatabaseSet.Name = "menu_DatabaseSet";
			this.menu_DatabaseSet.Size = new Size(182, 22);
			this.menu_DatabaseSet.Text = "设置数据库";
			this.menu_DatabaseSet.Click += this.menu_DatabaseSet_Click;
			this.menu_dbBackup.Image = (Image)componentResourceManager.GetObject("menu_dbBackup.Image");
			this.menu_dbBackup.Name = "menu_dbBackup";
			this.menu_dbBackup.Size = new Size(182, 22);
			this.menu_dbBackup.Text = "数据库备份";
			this.menu_dbBackup.Click += this.menu_dbBackup_Click;
			this.menu_dbRestore.Image = (Image)componentResourceManager.GetObject("menu_dbRestore.Image");
			this.menu_dbRestore.Name = "menu_dbRestore";
			this.menu_dbRestore.Size = new Size(182, 22);
			this.menu_dbRestore.Text = "数据库还原";
			this.menu_dbRestore.Click += this.menu_dbRestore_Click;
			this.menu_DBBakPath.Image = Resources.path;
			this.menu_DBBakPath.Name = "menu_DBBakPath";
			this.menu_DBBakPath.Size = new Size(182, 22);
			this.menu_DBBakPath.Text = "设置数据库备份路径";
			this.menu_DBBakPath.Click += this.menu_DBBakPath_Click;
			this.menu_sysParamSet.Image = (Image)componentResourceManager.GetObject("menu_sysParamSet.Image");
			this.menu_sysParamSet.Name = "menu_sysParamSet";
			this.menu_sysParamSet.Size = new Size(177, 22);
			this.menu_sysParamSet.Text = "系统参数设置";
			this.menu_sysParamSet.Click += this.menu_sysParamSet_Click;
			this.menu_navigation.Image = (Image)componentResourceManager.GetObject("menu_navigation.Image");
			this.menu_navigation.Name = "menu_navigation";
			this.menu_navigation.Size = new Size(177, 22);
			this.menu_navigation.Text = "导航";
			this.menu_navigation.Click += this.menu_navigation_Click;
			this.menu_selectlan.Image = Resources.log;
			this.menu_selectlan.Name = "menu_selectlan";
			this.menu_selectlan.Size = new Size(177, 22);
			this.menu_selectlan.Text = "语言选择";
			this.menu_selectlan.Click += this.menu_selectlan_Click;
			this.mnu_optmodules.Image = Resources.setting;
			this.mnu_optmodules.Name = "mnu_optmodules";
			this.mnu_optmodules.Size = new Size(177, 22);
			this.mnu_optmodules.Text = "Módulos Opcionais";
			this.mnu_optmodules.Click += this.mnu_optmodules_Click_1;
			this.menu_exit.Image = Resource.exit;
			this.menu_exit.Name = "menu_exit";
			this.menu_exit.Size = new Size(177, 22);
			this.menu_exit.Text = "退出";
			this.menu_exit.Click += this.menu_exit_Click;
			this.menuItem_sysuser.DropDownItems.AddRange(new ToolStripItem[3]
			{
				this.menuItem_dept,
				this.menuItem_user,
				this.menuItem_issueCard
			});
			this.menuItem_sysuser.Image = (Image)componentResourceManager.GetObject("menuItem_sysuser.Image");
			this.menuItem_sysuser.ImageScaling = ToolStripItemImageScaling.None;
			this.menuItem_sysuser.Name = "menuItem_sysuser";
			this.menuItem_sysuser.Size = new Size(80, 20);
			this.menuItem_sysuser.Text = "Usuários";
			this.menuItem_sysuser.ToolTipText = "人事";
			this.menuItem_dept.Image = (Image)componentResourceManager.GetObject("menuItem_dept.Image");
			this.menuItem_dept.ImageScaling = ToolStripItemImageScaling.None;
			this.menuItem_dept.Name = "menuItem_dept";
			this.menuItem_dept.Size = new Size(122, 22);
			this.menuItem_dept.Text = "部门";
			this.menuItem_dept.Click += this.menuItem_dept_Click;
			this.menuItem_user.Image = (Image)componentResourceManager.GetObject("menuItem_user.Image");
			this.menuItem_user.Name = "menuItem_user";
			this.menuItem_user.Size = new Size(122, 22);
			this.menuItem_user.Text = "Usuários";
			this.menuItem_user.Click += this.personnel_buttonItem_Click;
			this.menuItem_issueCard.Image = Resources.Issue_Card;
			this.menuItem_issueCard.Name = "menuItem_issueCard";
			this.menuItem_issueCard.Size = new Size(122, 22);
			this.menuItem_issueCard.Text = "人员发卡";
			this.menuItem_issueCard.Click += this.issueCard_buttonItem_Click;
			this.menuItem_ToolDvice.DropDownItems.AddRange(new ToolStripItem[3]
			{
				this.menuItem_Area,
				this.menuItem_device,
				this.menuItem_SearchAccess
			});
			this.menuItem_ToolDvice.Image = (Image)componentResourceManager.GetObject("menuItem_ToolDvice.Image");
			this.menuItem_ToolDvice.Name = "menuItem_ToolDvice";
			this.menuItem_ToolDvice.Size = new Size(59, 20);
			this.menuItem_ToolDvice.Text = "设备";
			this.menuItem_Area.Image = Resources.area;
			this.menuItem_Area.Name = "menuItem_Area";
			this.menuItem_Area.Size = new Size(158, 22);
			this.menuItem_Area.Text = "区域";
			this.menuItem_Area.Click += this.AreaSetting_buttonItem_Click;
			this.menuItem_device.Image = (Image)componentResourceManager.GetObject("menuItem_device.Image");
			this.menuItem_device.Name = "menuItem_device";
			this.menuItem_device.Size = new Size(158, 22);
			this.menuItem_device.Text = "Dispositivo";
			this.menuItem_device.Click += this.Device_buttonItem_Click;
			this.menuItem_SearchAccess.Image = (Image)componentResourceManager.GetObject("menuItem_SearchAccess.Image");
			this.menuItem_SearchAccess.ImageScaling = ToolStripItemImageScaling.None;
			this.menuItem_SearchAccess.Name = "menuItem_SearchAccess";
			this.menuItem_SearchAccess.Size = new Size(158, 22);
			this.menuItem_SearchAccess.Text = "搜索门禁控制器";
			this.menuItem_SearchAccess.Click += this.SearchAccessNode_NodeClick;
			this.menuItem_AccessControl.DropDownItems.AddRange(new ToolStripItem[14]
			{
				this.menuItem_TimeSlot,
				this.menuItem_Holidays,
				this.menuItem_doorsetting,
				this.menuItem_Levels,
				this.menuItem_wiegand,
				this.menuItem_Interlock,
				this.menuItem_anti,
				this.menuItem_Linkage,
				this.menuItem_NormalOpen,
				this.menuItem_MultiCardOpen,
				this.menuItem_Monitoring,
				this.menuItem_map,
				this.mnu_reader,
				this.mnu_auxiliary
			});
			this.menuItem_AccessControl.Image = (Image)componentResourceManager.GetObject("menuItem_AccessControl.Image");
			this.menuItem_AccessControl.Name = "menuItem_AccessControl";
			this.menuItem_AccessControl.Size = new Size(137, 20);
			this.menuItem_AccessControl.Text = "Controle de Acesso";
			this.menuItem_TimeSlot.Image = (Image)componentResourceManager.GetObject("menuItem_TimeSlot.Image");
			this.menuItem_TimeSlot.Name = "menuItem_TimeSlot";
			this.menuItem_TimeSlot.Size = new Size(198, 22);
			this.menuItem_TimeSlot.Text = "Horários";
			this.menuItem_TimeSlot.Click += this.TimeSlot_buttonItem_Click;
			this.menuItem_Holidays.Image = (Image)componentResourceManager.GetObject("menuItem_Holidays.Image");
			this.menuItem_Holidays.Name = "menuItem_Holidays";
			this.menuItem_Holidays.Size = new Size(198, 22);
			this.menuItem_Holidays.Text = "Feriados";
			this.menuItem_Holidays.Click += this.Holidays_buttonItem_Click;
			this.menuItem_doorsetting.Image = (Image)componentResourceManager.GetObject("menuItem_doorsetting.Image");
			this.menuItem_doorsetting.Name = "menuItem_doorsetting";
			this.menuItem_doorsetting.Size = new Size(198, 22);
			this.menuItem_doorsetting.Text = "Configuração de Portas";
			this.menuItem_doorsetting.Click += this.doorsettinbtn_Click;
			this.menuItem_Levels.Image = (Image)componentResourceManager.GetObject("menuItem_Levels.Image");
			this.menuItem_Levels.Name = "menuItem_Levels";
			this.menuItem_Levels.Size = new Size(198, 22);
			this.menuItem_Levels.Text = "Perfil de Acesso";
			this.menuItem_Levels.Click += this.node_doorLevels_NodeClick;
			this.menuItem_wiegand.Image = Resources.Real_Time;
			this.menuItem_wiegand.Name = "menuItem_wiegand";
			this.menuItem_wiegand.Size = new Size(198, 22);
			this.menuItem_wiegand.Text = "韦根格式";
			this.menuItem_wiegand.Click += this.menuItem_wiegand_Click;
			this.menuItem_Interlock.Image = (Image)componentResourceManager.GetObject("menuItem_Interlock.Image");
			this.menuItem_Interlock.Name = "menuItem_Interlock";
			this.menuItem_Interlock.Size = new Size(198, 22);
			this.menuItem_Interlock.Text = "互锁";
			this.menuItem_Interlock.Click += this.InterlockNode_NodeClick;
			this.menuItem_anti.Image = (Image)componentResourceManager.GetObject("menuItem_anti.Image");
			this.menuItem_anti.Name = "menuItem_anti";
			this.menuItem_anti.Size = new Size(198, 22);
			this.menuItem_anti.Text = "反潜";
			this.menuItem_anti.Click += this.antiNode_NodeClick;
			this.menuItem_Linkage.Image = (Image)componentResourceManager.GetObject("menuItem_Linkage.Image");
			this.menuItem_Linkage.Name = "menuItem_Linkage";
			this.menuItem_Linkage.Size = new Size(198, 22);
			this.menuItem_Linkage.Text = "联动";
			this.menuItem_Linkage.Click += this.LinkageNode_NodeClick;
			this.menuItem_NormalOpen.Image = (Image)componentResourceManager.GetObject("menuItem_NormalOpen.Image");
			this.menuItem_NormalOpen.Name = "menuItem_NormalOpen";
			this.menuItem_NormalOpen.Size = new Size(198, 22);
			this.menuItem_NormalOpen.Text = "首卡常开";
			this.menuItem_NormalOpen.Click += this.NormalOpenNode_NodeClick;
			this.menuItem_MultiCardOpen.Image = (Image)componentResourceManager.GetObject("menuItem_MultiCardOpen.Image");
			this.menuItem_MultiCardOpen.Name = "menuItem_MultiCardOpen";
			this.menuItem_MultiCardOpen.Size = new Size(198, 22);
			this.menuItem_MultiCardOpen.Text = "多卡开门";
			this.menuItem_MultiCardOpen.Click += this.MenupanelExNode_NodeClick;
			this.menuItem_Monitoring.Image = (Image)componentResourceManager.GetObject("menuItem_Monitoring.Image");
			this.menuItem_Monitoring.ImageScaling = ToolStripItemImageScaling.None;
			this.menuItem_Monitoring.Name = "menuItem_Monitoring";
			this.menuItem_Monitoring.Size = new Size(198, 22);
			this.menuItem_Monitoring.Text = "Monitoramento";
			this.menuItem_Monitoring.Click += this.Monitoring_buttonItem_Click;
			this.menuItem_map.Image = (Image)componentResourceManager.GetObject("menuItem_map.Image");
			this.menuItem_map.Name = "menuItem_map";
			this.menuItem_map.Size = new Size(198, 22);
			this.menuItem_map.Text = "电子地图";
			this.menuItem_map.Click += this.node_map_NodeClick;
			this.mnu_reader.Image = Resources.Reader;
			this.mnu_reader.Name = "mnu_reader";
			this.mnu_reader.Size = new Size(198, 22);
			this.mnu_reader.Text = "读头设置";
			this.mnu_reader.Click += this.mnu_reader_Click;
			this.mnu_auxiliary.Image = Resources.Auxiliary;
			this.mnu_auxiliary.Name = "mnu_auxiliary";
			this.mnu_auxiliary.Size = new Size(198, 22);
			this.mnu_auxiliary.Text = "辅助输入输出设置";
			this.mnu_auxiliary.Click += this.mnu_auxiliary_Click;
			this.menuItem_RecordsReport.DropDownItems.AddRange(new ToolStripItem[6]
			{
				this.menuItem_RecordesToday,
				this.menuItem_RecordesOfThreeDays,
				this.menuItem_RecordsOfWeek,
				this.menuItem_recordsOfLastWeek,
				this.menuItem_ExptRecords,
				this.mnuCustomReport
			});
			this.menuItem_RecordsReport.Image = (Image)componentResourceManager.GetObject("menuItem_RecordsReport.Image");
			this.menuItem_RecordsReport.Name = "menuItem_RecordsReport";
			this.menuItem_RecordsReport.Size = new Size(59, 20);
			this.menuItem_RecordsReport.Text = "报表";
			this.menuItem_RecordesToday.Image = (Image)componentResourceManager.GetObject("menuItem_RecordesToday.Image");
			this.menuItem_RecordesToday.Name = "menuItem_RecordesToday";
			this.menuItem_RecordesToday.Size = new Size(170, 22);
			this.menuItem_RecordesToday.Text = "当天门禁事件";
			this.menuItem_RecordesToday.Click += this.node_RecordesToday_NodeClick;
			this.menuItem_RecordesOfThreeDays.Image = (Image)componentResourceManager.GetObject("menuItem_RecordesOfThreeDays.Image");
			this.menuItem_RecordesOfThreeDays.Name = "menuItem_RecordesOfThreeDays";
			this.menuItem_RecordesOfThreeDays.Size = new Size(170, 22);
			this.menuItem_RecordesOfThreeDays.Text = "最近三天门禁事件";
			this.menuItem_RecordesOfThreeDays.Click += this.node_RecordsOfHalfWeek_NodeClick;
			this.menuItem_RecordsOfWeek.Image = (Image)componentResourceManager.GetObject("menuItem_RecordsOfWeek.Image");
			this.menuItem_RecordsOfWeek.Name = "menuItem_RecordsOfWeek";
			this.menuItem_RecordsOfWeek.Size = new Size(170, 22);
			this.menuItem_RecordsOfWeek.Text = "最近一周门禁事件";
			this.menuItem_RecordsOfWeek.Click += this.node_recordsOfWeek_NodeClick;
			this.menuItem_recordsOfLastWeek.Image = (Image)componentResourceManager.GetObject("menuItem_recordsOfLastWeek.Image");
			this.menuItem_recordsOfLastWeek.Name = "menuItem_recordsOfLastWeek";
			this.menuItem_recordsOfLastWeek.Size = new Size(170, 22);
			this.menuItem_recordsOfLastWeek.Text = "上周门禁事件";
			this.menuItem_recordsOfLastWeek.Click += this.node_recordsOfLastWeek_NodeClick;
			this.menuItem_ExptRecords.Image = (Image)componentResourceManager.GetObject("menuItem_ExptRecords.Image");
			this.menuItem_ExptRecords.Name = "menuItem_ExptRecords";
			this.menuItem_ExptRecords.Size = new Size(170, 22);
			this.menuItem_ExptRecords.Text = "门禁异常事件";
			this.menuItem_ExptRecords.Click += this.node_ExptRecords_NodeClick;
			this.mnuCustomReport.Image = Resources.reports;
			this.mnuCustomReport.Name = "mnuCustomReport";
			this.mnuCustomReport.Size = new Size(170, 22);
			this.mnuCustomReport.Text = "自定义报表";
			this.mnuCustomReport.Click += this.mnuCustomReport_Click;
			this.menuItem_Help.DropDownItems.AddRange(new ToolStripItem[3]
			{
				this.Menu_help,
				this.menuItem_About,
				this.menu_GenerateLangFile
			});
			this.menuItem_Help.Image = (Image)componentResourceManager.GetObject("menuItem_Help.Image");
			this.menuItem_Help.Name = "menuItem_Help";
			this.menuItem_Help.Size = new Size(59, 20);
			this.menuItem_Help.Text = "帮助";
			this.Menu_help.Image = (Image)componentResourceManager.GetObject("Menu_help.Image");
			this.Menu_help.Name = "Menu_help";
			this.Menu_help.Size = new Size(170, 22);
			this.Menu_help.Text = "Ajuda";
			this.Menu_help.Click += this.Menu_help_Click;
			this.menuItem_About.Image = (Image)componentResourceManager.GetObject("menuItem_About.Image");
			this.menuItem_About.Name = "menuItem_About";
			this.menuItem_About.Size = new Size(170, 22);
			this.menuItem_About.Text = "Sobre";
			this.menuItem_About.Click += this.menuItem_About_Click;
			this.menu_GenerateLangFile.Name = "menu_GenerateLangFile";
			this.menu_GenerateLangFile.Size = new Size(170, 22);
			this.menu_GenerateLangFile.Text = "生成语言脚本工具";
			this.menu_GenerateLangFile.Click += this.menu_GenerateLangFile_Click;
			this.menu_Main.ImeMode = ImeMode.NoControl;
			this.menu_Main.Items.AddRange(new ToolStripItem[6]
			{
				this.menuItem_sysset,
				this.menuItem_sysuser,
				this.menuItem_ToolDvice,
				this.menuItem_AccessControl,
				this.menuItem_RecordsReport,
				this.menuItem_Help
			});
			this.menu_Main.Location = new Point(0, 0);
			this.menu_Main.Name = "menu_Main";
			this.menu_Main.Size = new Size(838, 24);
			this.menu_Main.TabIndex = 23;
			this.menu_Main.ItemClicked += this.menu_Main_ItemClicked;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.CornflowerBlue;
			base.Controls.Add(this.MainPanelEx);
			base.Controls.Add(this.MenuPanelEx);
			base.Controls.Add(this.pnl_type);
			base.Controls.Add(this.expandableSplitter1);
			base.Controls.Add(this.LeftExplorerBar);
			base.Controls.Add(this.top_StripPanel);
			base.Controls.Add(this.panelEx3);
			base.Controls.Add(this.menu_Main);
			base.Name = "UserMainControl";
			base.Size = new Size(838, 580);
			base.Load += this.MainForm_Load;
			this.panelEx3.ResumeLayout(false);
			((ISupportInitialize)this.LeftExplorerBar).EndInit();
			this.LeftExplorerBar.ResumeLayout(false);
			((ISupportInitialize)this.advTree1).EndInit();
			this.menu_Main.ResumeLayout(false);
			this.menu_Main.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}