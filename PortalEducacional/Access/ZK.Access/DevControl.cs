/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.Access.door;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class DevControl : UserControl
	{
		public delegate void DoorStateChangedHandler(object sender);

		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private DoorState m_status = DoorState.None;

		private AccDoor m_accdoor = null;

		private Machines m_machines = null;

		private MachinesBll m_devBll = new MachinesBll(MainForm._ia);

		private AccDoorBll m_doorBll = new AccDoorBll(MainForm._ia);

		private ObjDevice m_dev = null;

		private DoorType m_doorType = DoorType.Door1;

		private ObjDoorInfo m_door = null;

		private DeviceParamBll m_paramBll = null;

		private DeviceServerBll m_devServer = null;

		private int m_paintStartX = 3;

		private int m_paintStarty = 2;

		private int m_PaintTxtHeight = 24;

		private string m_doorName = string.Empty;

		private bool m_ispaint = false;

		private bool m_ismovedown = false;

		private int m_lastx = 0;

		private int m_lasty = 0;

		private int m_startx = 0;

		private int m_starty = 0;

		private int n_scrolltop = 0;

		private int n_scrollleft = 0;

		private bool n_isMove = false;

		private DevMapControl m_mapControl;

		private bool m_isShow = false;

		private string m_statusInfo = string.Empty;

		private bool m_isMutilSelected = false;

		private bool m_isSelected = false;

		private ObjRTLogInfo m_lastInfo = null;

		private int m_TextWidth = 0;

		private bool m_isAlarm = false;

		public static bool StopWarning = true;

		private static Thread threadPlaySound;

		private FrmSimpleProgress fPb;

		private static string[] Fonts = new string[5]
		{
			"Arial",
			"宋体",
			"Impact",
			"Courier New",
			"Tahoma"
		};

		private static Color[] Colors = new Color[8]
		{
			Color.White,
			Color.Black,
			Color.FromArgb(127, 143, 154),
			Color.LightGreen,
			Color.LightBlue,
			Color.FromArgb(90, 34, 130, 255),
			Color.Green,
			Color.Blue
		};

		private SolidBrush _brush = null;

		private SolidBrush _forebrush = null;

		private SolidBrush _fontbrush = null;

		private SolidBrush _selectbrush = null;

		private Pen _selectPen = null;

		private Pen _borderPen = null;

		private int _fontSize = 9;

		private Font _font = null;

		private bool m_isLoadFinish = false;

		private bool m_isMoving = false;

		private WaitForm m_waitForm = WaitForm.Instance;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem menu_opendoor;

		private ToolStripMenuItem menu_closedoor;

		private ToolStripMenuItem menu_cancelAlarm;

		private System.Windows.Forms.Timer timer1;

		private ToolStripMenuItem menu_door_seting;

		public DeviceServerBll DevServer => this.m_devServer;

		public int ScrollTop
		{
			get
			{
				return this.n_scrolltop;
			}
			set
			{
				this.n_scrolltop = value;
			}
		}

		public int ScrollLeft
		{
			get
			{
				return this.n_scrollleft;
			}
			set
			{
				this.n_scrollleft = value;
			}
		}

		public bool IsMove
		{
			get
			{
				return this.n_isMove;
			}
			set
			{
				this.n_isMove = value;
			}
		}

		public DevMapControl MapControl
		{
			get
			{
				return this.m_mapControl;
			}
			set
			{
				this.m_mapControl = value;
			}
		}

		public bool IsMutilSelected => this.m_isMutilSelected;

		public bool IsSelected
		{
			get
			{
				return this.m_isSelected;
			}
			set
			{
				if (this.m_isSelected != value)
				{
					this.m_isSelected = value;
					if (this.SelectChangedEvent != null)
					{
						this.SelectChangedEvent(this, null);
					}
					this.SetBakImg();
				}
			}
		}

		public string DoorName
		{
			get
			{
				return this.m_doorName;
			}
			set
			{
				this.m_doorName = value;
				string text = this.m_doorName;
				if (string.IsNullOrEmpty(text))
				{
					text = base.Name;
				}
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				int num = bytes.Length - text.Length;
				if (num > 0)
				{
					num /= 2;
					this.m_TextWidth = (text.Length - num) * (int)this.IFont.Size * 3 / 4;
					this.m_TextWidth += num * (int)this.IFont.Size;
					this.m_TextWidth += num / 2 * (int)this.IFont.Size;
				}
				else
				{
					this.m_TextWidth = text.Length * (int)this.IFont.Size * 3 / 4;
				}
			}
		}

		public AccDoor AccDoorInfo
		{
			get
			{
				SysLogServer.WriteLog("AccDoorInfo...1 ", true);
				return this.m_accdoor;
			}
			set
			{
				if (value != null)
				{
					SysLogServer.WriteLog("AccDoorInfo...2 ", true);
					this.m_accdoor = value;
					switch (this.m_accdoor.door_no)
					{
					case 1:
						SysLogServer.WriteLog("AccDoorInfo...3 ", true);
						this.m_doorType = DoorType.Door1;
						break;
					case 2:
						SysLogServer.WriteLog("AccDoorInfo...4 ", true);
						this.m_doorType = DoorType.Door2;
						break;
					case 3:
						SysLogServer.WriteLog("AccDoorInfo...5 ", true);
						this.m_doorType = DoorType.Door3;
						break;
					case 4:
						SysLogServer.WriteLog("AccDoorInfo...6 ", true);
						this.m_doorType = DoorType.Door4;
						break;
					default:
						SysLogServer.WriteLog("AccDoorInfo...9 ", true);
						this.m_doorType = DoorType.Door1;
						break;
					}
					this.DoorName = this.m_accdoor.door_name;
					SysLogServer.WriteLog("AccDoorInfo...10 ", true);
					this.SetBakImg();
					SysLogServer.WriteLog("AccDoorInfo...11 ", true);
					Machines model = this.m_devBll.GetModel(this.m_accdoor.device_id);
					if (model != null)
					{
						SysLogServer.WriteLog("AccDoorInfo...12 ", true);
						DevShowInfo.Instance.SetInfo(model.MachineAlias, this.m_accdoor.door_no.ToString(), this.m_doorName, model.IP, this.m_statusInfo);
						this.m_machines = model;
						if (this.m_devServer != null)
						{
							SysLogServer.WriteLog("AccDoorInfo...13 ", true);
							this.m_devServer.DoorStateEvent -= this.server_DoorStateEvent;
							this.m_devServer.ListenChangedEvent -= this.m_devServer_ListenChangedEvent;
						}
						SysLogServer.WriteLog("AccDoorInfo...14 ", true);
						this.m_devServer = DeviceServers.Instance.GetDeviceServer(this.m_machines);
						SysLogServer.WriteLog("AccDoorInfo...15 ", true);
						if (this.m_devServer != null)
						{
							SysLogServer.WriteLog("AccDoorInfo...16 ", true);
							this.m_dev = this.m_devServer.DevInfo;
							this.m_devServer.DoorStateEvent += this.server_DoorStateEvent;
							this.m_devServer.ListenChangedEvent += this.m_devServer_ListenChangedEvent;
							this.m_paramBll = new DeviceParamBll(this.m_devServer.Application);
							if (!this.m_devServer.IsAddOk)
							{
								this.Status = DoorState.NoConnected;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SNoConnected", "未连接");
								SysLogServer.WriteLog("AccDoorInfo...17 ", true);
							}
							else
							{
								SysLogServer.WriteLog("AccDoorInfo...18 ", true);
								if (this.m_devServer.IsNeedListen)
								{
									SysLogServer.WriteLog("AccDoorInfo...19 ", true);
									if (this.m_devServer.IsConnected)
									{
										SysLogServer.WriteLog("AccDoorInfo...20 ", true);
										this.Status = DoorState.Connected;
										this.MenuEable(true);
									}
									else
									{
										SysLogServer.WriteLog("AccDoorInfo...21 ", true);
										this.Status = DoorState.NoConnected;
										this.m_statusInfo = ShowMsgInfos.GetInfo("SNoConnected", "未连接");
									}
								}
								else
								{
									SysLogServer.WriteLog("AccDoorInfo...22 ", true);
									this.Status = DoorState.None;
									this.m_statusInfo = string.Empty;
								}
							}
						}
						else
						{
							SysLogServer.WriteLog("AccDoorInfo...23 ", true);
							this.m_statusInfo = ShowMsgInfos.GetInfo("SDisabled", "禁用");
							this.Status = DoorState.Disabled;
						}
					}
					else
					{
						SysLogServer.WriteLog("AccDoorInfo...24 ", true);
						base.Visible = false;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SDisabled", "禁用");
						this.Status = DoorState.Disabled;
					}
					initLang.LocaleForm(this, base.Name);
				}
			}
		}

		public ObjDoorInfo Door => this.m_door;

		public DoorState Status
		{
			get
			{
				return this.m_status;
			}
			set
			{
				if (value != this.m_status)
				{
					this.m_status = value;
					this.SetBakImg();
					this.OnDoorStateChanged();
				}
			}
		}

		public Machines MachineInfo => this.m_machines;

		public ObjDevice DevInfo
		{
			get
			{
				if (this.m_dev == null && this.m_devServer != null)
				{
					this.m_dev = this.m_devServer.DevInfo;
				}
				return this.m_dev;
			}
		}

		public DoorType DoorType => this.m_doorType;

		public SolidBrush BackBrush
		{
			get
			{
				if (this._brush == null)
				{
					this._brush = new SolidBrush(this.BackColor);
				}
				return this._brush;
			}
			set
			{
				this._brush = value;
			}
		}

		public SolidBrush ForeBrush
		{
			get
			{
				if (this._forebrush == null)
				{
					this._forebrush = new SolidBrush(DevControl.Colors[3]);
				}
				return this._forebrush;
			}
			set
			{
				this._forebrush = value;
			}
		}

		public SolidBrush FontBrush
		{
			get
			{
				if (this._fontbrush == null)
				{
					this._fontbrush = new SolidBrush(DevControl.Colors[6]);
				}
				return this._fontbrush;
			}
			set
			{
				this._fontbrush = value;
			}
		}

		public SolidBrush SelectBrush
		{
			get
			{
				if (this._selectbrush == null)
				{
					this._selectbrush = new SolidBrush(DevControl.Colors[5]);
				}
				return this._selectbrush;
			}
			set
			{
				this._selectbrush = value;
			}
		}

		public Pen SelectPen
		{
			get
			{
				if (this._selectPen == null)
				{
					this._selectPen = new Pen(DevControl.Colors[4]);
				}
				return this._selectPen;
			}
			set
			{
				this._selectPen = value;
			}
		}

		public Pen BorderPen
		{
			get
			{
				if (this._borderPen == null)
				{
					this._borderPen = new Pen(DevControl.Colors[1]);
				}
				return this._borderPen;
			}
			set
			{
				this._borderPen = value;
			}
		}

		public int FontSize
		{
			get
			{
				return this._fontSize;
			}
			set
			{
				this._fontSize = value;
			}
		}

		public Font IFont
		{
			get
			{
				if (this._font == null)
				{
					this._font = new Font(DevControl.Fonts[1], (float)this._fontSize, FontStyle.Regular);
				}
				return this._font;
			}
			set
			{
				this._font = value;
			}
		}

		public bool IsLoadFinish
		{
			get
			{
				return this.m_isLoadFinish;
			}
			set
			{
				this.m_isLoadFinish = value;
			}
		}

		public event DoorStateChangedHandler DoorStateChanged;

		public event EventHandler SelectChangedEvent;

		public event EventHandler MutilSelectChangedEvent;

		protected void OnDoorStateChanged()
		{
			if (this.DoorStateChanged != null)
			{
				this.DoorStateChanged(this);
			}
		}

		public DevControl()
		{
			SysLogServer.WriteLog("DevControl...1 ", true);
			this.InitializeComponent();
			SysLogServer.WriteLog("DevControl...2 ", true);
			this.BackColor = Color.FromArgb(0, 255, 255, 255);
			SysLogServer.WriteLog("DevControl...3 ", true);
			this.MenuEable(false);
			SysLogServer.WriteLog("DevControl...4 ", true);
			try
			{
				SysLogServer.WriteLog("DevControl...5 ", true);
				base.SetStyle(ControlStyles.UserPaint, true);
				SysLogServer.WriteLog("DevControl...6 ", true);
				base.SetStyle(ControlStyles.DoubleBuffer, true);
				SysLogServer.WriteLog("DevControl...7 ", true);
				base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				SysLogServer.WriteLog("DevControl...8 ", true);
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("DevControl...9 :" + ex.Message, true);
			}
		}

		public void MenuEable(bool isEable)
		{
			if (!base.IsDisposed && base.Visible && !base.InvokeRequired)
			{
				this.menu_closedoor.Enabled = isEable;
				this.menu_opendoor.Enabled = isEable;
				this.menu_cancelAlarm.Enabled = isEable;
				if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel) && !SysInfos.IsOwerControlPermission(SysInfos.Monitoring))
				{
					this.menu_closedoor.Enabled = false;
					this.menu_opendoor.Enabled = false;
					this.menu_cancelAlarm.Enabled = false;
				}
			}
		}

		private void m_devServer_ListenChangedEvent(object sender, EventArgs e)
		{
			if (this.m_devServer != null && !this.m_devServer.IsNeedListen)
			{
				this.Status = DoorState.None;
				this.m_lastInfo = null;
				this.MenuEable(false);
			}
		}

		private void server_DoorStateEvent(ObjDevice sender, ObjRTLogInfo info)
		{
			if (!base.IsDisposed && base.Visible && info != null)
			{
				string doorID = info.DoorID;
				int doorType = (int)this.m_doorType;
				if (doorID == doorType.ToString() || info.DoorID == "0" || info.EType == EventType.Type302 || info.EType == EventType.Type300 || info.EType == EventType.Type301)
				{
					if (base.InvokeRequired)
					{
						object[] array = new object[1]
						{
							info
						};
						try
						{
							base.BeginInvoke((MethodInvoker)delegate
							{
								this.server_DoorStateEvent(sender, info);
							});
						}
						catch (Exception ex)
						{
							SysLogServer.WriteLog("DevControl.server_DoorStateEvent_this.BeginInvoke Exception: " + ex.Message, true);
						}
					}
					else if (this.m_devServer != null && this.m_devServer.IsNeedListen)
					{
						this.DoorStateEvent(info);
					}
				}
			}
		}

		private void DoorStateEvent(ObjRTLogInfo info)
		{
			if (AccCommon.CodeVersion == CodeVersionType.OpenDoorWarning)
			{
				DevControl.OpenDoorWarning(info, this.MachineInfo.MachineAlias, this.DoorName);
			}
			if (!this.m_isAlarm)
			{
				if (this.m_lastInfo != null && this.m_lastInfo.EType == info.EType)
				{
					if (info.EType != EventType.Type255)
					{
						if (this.MachineInfo.DevSDKType == SDKType.StandaloneSDK)
						{
							this.m_statusInfo = info.StatusInfo;
						}
						return;
					}
					if (info.WarningStatus == this.m_lastInfo.WarningStatus && info.DoorStatus == this.m_lastInfo.DoorStatus)
					{
						if (this.MachineInfo.DevSDKType == SDKType.StandaloneSDK)
						{
							this.m_statusInfo = info.StatusInfo;
						}
						return;
					}
				}
				this.m_lastInfo = info;
				this.m_statusInfo = info.StatusInfo;
				if (string.IsNullOrEmpty(this.m_statusInfo))
				{
					int eType = (int)info.EType;
					if (eType < 255)
					{
						this.m_statusInfo = PullSDKEventInfos.GetInfo(info.EType);
					}
				}
				this.MenuEable(true);
				switch (info.EType)
				{
				case EventType.Type51:
					if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
					{
						this.Status = DoorState.DoorLocked;
						if (info.WarningStatus == "1")
						{
							this.m_isAlarm = true;
						}
					}
					break;
				case EventType.Type101:
				case EventType.DoorUnexpectedOpened:
				case EventType.DuressAlarm:
				case EventType.AntibackAlarm:
				case EventType.DeviceBroken:
				case EventType.VerifyTimesOut:
				case EventType.LinkageAlarm:
					this.Status = DoorState.Alarm;
					this.m_isAlarm = true;
					break;
				case EventType.Type102:
					this.Status = DoorState.Alarm;
					this.m_isAlarm = true;
					break;
				case EventType.Type103:
					this.Status = DoorState.Alarm;
					this.m_isAlarm = true;
					break;
				case EventType.Type200:
				case EventType.DoorNotClosedOrOpened:
				case EventType.DoorOpenedByButton:
					this.Status = DoorState.Open;
					break;
				case EventType.Type201:
				case EventType.DoorClosed:
					this.Status = DoorState.Closed;
					break;
				case EventType.Type300:
					this.Status = DoorState.NoConnected;
					this.MenuEable(false);
					break;
				case EventType.Type301:
					this.Status = DoorState.Connected;
					break;
				case EventType.Type302:
					this.Status = DoorState.None;
					this.MenuEable(false);
					break;
				case EventType.Type255:
					if (info.WarningStatus == "1")
					{
						this.Status = DoorState.Alarm;
						this.m_devServer.IsAlarm[(int)(this.m_doorType - 1)] = true;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SIsAlarm", "报警");
					}
					else if (info.WarningStatus == "2")
					{
						this.Status = DoorState.OutTimeAlarm;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SIsOutTimeAlarm", "门开超时");
					}
					else if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
					{
						int.TryParse(info.DoorStatus, out int num);
						int num2 = num >> 4;
						switch (num & 0xF)
						{
						case 0:
							this.Status = DoorState.NoDoorSensor;
							this.m_statusInfo = ShowMsgInfos.GetInfo("SIsNoDoorSensor", "无门磁");
							break;
						case 1:
							switch (num2)
							{
							case 0:
								this.Status = DoorState.Closed;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsOpen", "门打开");
								break;
							case 1:
								this.Status = DoorState.DoorClosedOnNormalOpenTimeOrRemoteOpen;
								this.m_statusInfo = ShowMsgInfos.GetInfo("DoorClosedOnNormalOpenTime", "常开时段门关闭");
								break;
							case 2:
								this.Status = DoorState.DoorClosedOnNormalOpenTimeOrRemoteOpen;
								this.m_statusInfo = ShowMsgInfos.GetInfo("DoorClosedOnRemoteOpen", "远程开门时门关闭");
								break;
							default:
								this.Status = DoorState.Open;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsOpen", "门打开");
								break;
							}
							this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsClosed", "门关闭");
							break;
						case 2:
							switch (num2)
							{
							case 0:
								this.Status = DoorState.Open;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsOpen", "门打开");
								break;
							case 1:
								this.Status = DoorState.NormalOpenOrRemoteOpen;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsNormalOpen", "常开时段门打开");
								break;
							case 2:
								this.Status = DoorState.NormalOpenOrRemoteOpen;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsRemoteOpen", "远程开门");
								break;
							default:
								this.Status = DoorState.Open;
								this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsOpen", "门打开");
								break;
							}
							break;
						}
					}
					else if (info.DoorStatus == "0")
					{
						this.Status = DoorState.NoDoorSensor;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SIsNoDoorSensor", "无门磁");
					}
					else if (info.DoorStatus == "1")
					{
						this.Status = DoorState.Closed;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsClosed", "门关闭");
					}
					else if (info.DoorStatus == "2")
					{
						this.Status = DoorState.Open;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SDoorIsOpen", "门打开");
					}
					else
					{
						this.Status = DoorState.NoDoorSensor;
						this.m_statusInfo = ShowMsgInfos.GetInfo("SIsNoDoorSensor", "无门磁");
					}
					break;
				}
			}
		}

		public static void OpenDoorWarning(ObjRTLogInfo info, string devName, string doorName)
		{
			try
			{
				if (info.DoorStatus == "2" || (info.EType == EventType.Type255 && info.VerifyType == "1"))
				{
					if (!string.IsNullOrEmpty(DevLogServer.AlarmFilePath))
					{
						if (DevControl.threadPlaySound == null || DevControl.threadPlaySound.ThreadState == System.Threading.ThreadState.Stopped)
						{
							DevControl.threadPlaySound = new Thread(DevControl.PlaySound);
						}
						DevControl.StopWarning = false;
						if (DevControl.threadPlaySound.ThreadState != 0)
						{
							DevControl.threadPlaySound.Start();
						}
					}
					OpenDoorWarning instance = ZK.Access.door.OpenDoorWarning.Instance;
					instance.Show(info, devName, doorName);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private static void PlaySound()
		{
			try
			{
				SoundPlayer soundPlayer = new SoundPlayer(DevLogServer.AlarmFilePath);
				while (!DevControl.StopWarning)
				{
					soundPlayer.PlaySync();
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog(ex.Message);
			}
		}

		public int UnlockDoor()
		{
			int result = -13;
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				result = ((this.m_devServer != null) ? this.m_devServer.ControlDevice(1, (int)this.DoorType, 3, 0, 0, "") : (-1002));
			}
			return result;
		}

		public int Reboot()
		{
			int result = -13;
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				result = ((this.m_devServer != null) ? this.m_devServer.ControlDevice(3, 0, 0, 0, 0, "") : (-1002));
			}
			return result;
		}

		private void _getOfflineEvents(DoorState state)
		{
			try
			{
				if (this.m_devServer.Monitor != null)
				{
					SDKType devSDKType = this.m_devServer.DevInfo.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						StdDeviceMonitor stdDeviceMonitor = (StdDeviceMonitor)this.m_devServer.Monitor;
						stdDeviceMonitor.OnOfflineLog += this.MonitorWatchdog_OnOfflineLog;
						stdDeviceMonitor.OnOfflineLogCount += this.MonitorWatchdog_OnOfflineLogCount;
						stdDeviceMonitor.OnOfflineLogEnd += this.MonitorWatchdog_OnOfflineLogEnd;
						stdDeviceMonitor.ProcessOfflineEvents(this.m_devServer);
						stdDeviceMonitor.OnOfflineLog -= this.MonitorWatchdog_OnOfflineLog;
						stdDeviceMonitor.OnOfflineLogCount -= this.MonitorWatchdog_OnOfflineLogCount;
						stdDeviceMonitor.OnOfflineLogEnd -= this.MonitorWatchdog_OnOfflineLogEnd;
					}
					else
					{
						PullDeviceMonitor pullDeviceMonitor = (PullDeviceMonitor)this.m_devServer.Monitor;
						pullDeviceMonitor.OnOfflineLog += this.MonitorWatchdog_OnOfflineLog;
						pullDeviceMonitor.OnOfflineLogCount += this.MonitorWatchdog_OnOfflineLogCount;
						pullDeviceMonitor.OnOfflineLogEnd += this.MonitorWatchdog_OnOfflineLogEnd;
						pullDeviceMonitor.ProcessOfflineEvents(this.m_devServer);
						pullDeviceMonitor.OnOfflineLog -= this.MonitorWatchdog_OnOfflineLog;
						pullDeviceMonitor.OnOfflineLogCount -= this.MonitorWatchdog_OnOfflineLogCount;
						pullDeviceMonitor.OnOfflineLogEnd -= this.MonitorWatchdog_OnOfflineLogEnd;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void MonitorWatchdog_OnOfflineLogEnd()
		{
			if (this.fPb != null)
			{
				this.fPb.Close();
				this.fPb = null;
			}
		}

		private void MonitorWatchdog_OnOfflineLogCount()
		{
			if (this.fPb != null)
			{
				this.fPb.stepProgressBar();
			}
		}

		private void MonitorWatchdog_OnOfflineLog(DeviceServerBll sender, OnOfflineLogEventArgs e)
		{
			this.fPb = new FrmSimpleProgress();
			this.fPb.setTitulo("Baixando logs... (" + e.devName + ")");
			this.fPb.setProgressBar(0, e.count);
			this.fPb.Show();
		}

		private void SetBakImg()
		{
			SysLogServer.WriteLog("\n\nSetBakImg START: " + this.m_doorName, true);
			try
			{
				SysLogServer.WriteLog("SetBakImg...1 ", true);
				Bitmap bitmap = new Bitmap(base.Width, base.Height);
				SysLogServer.WriteLog("SetBakImg...2 ", true);
				Graphics graphics = Graphics.FromImage(bitmap);
				SysLogServer.WriteLog("SetBakImg...3 ", true);
				graphics.Clear(Color.FromArgb(0, 255, 255, 255));
				SysLogServer.WriteLog("SetBakImg...4 ", true);
				int num = base.Height - this.m_PaintTxtHeight - this.m_paintStarty - 2;
				int num2 = base.Width - this.m_paintStartX - 2;
				int num3 = 0;
				if (num2 > 40)
				{
					num3 = (num2 - 40) / 2;
					num2 = 40;
				}
				int num4 = 0;
				if (num > 40)
				{
					num4 = (num - 40) / 2;
					num = 40;
				}
				switch (this.m_status)
				{
				case DoorState.None:
					SysLogServer.WriteLog("SetBakImg...5 ", true);
					graphics.DrawImage(Resource.door_default, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.Disabled:
					SysLogServer.WriteLog("SetBakImg...6 ", true);
					graphics.DrawImage(Resource.door_disabled, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.NoConnected:
					SysLogServer.WriteLog("SetBakImg...7 ", true);
					graphics.DrawImage(Resource.door_offline, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.Connected:
					SysLogServer.WriteLog("SetBakImg...8 ", true);
					graphics.DrawImage(Resource.door_default, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.NoDoorSensor:
					SysLogServer.WriteLog("SetBakImg...9 ", true);
					graphics.DrawImage(Resource.door_nosensor, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.Alarm:
					SysLogServer.WriteLog("SetBakImg...10 ", true);
					graphics.DrawImage(Resource.door_alarm, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.OutTimeAlarm:
					SysLogServer.WriteLog("SetBakImg...11 ", true);
					graphics.DrawImage(Resource.door_open_timeout, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.Open:
				{
					SysLogServer.WriteLog("SetBakImg...12 ", true);
					Bitmap relays_open = Resource.door_opened;
					graphics.DrawImage(relays_open, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				}
				case DoorState.Closed:
				{
					SysLogServer.WriteLog("SetBakImg...13 ", true);
					Bitmap relays_open = Resource.door_closed;
					graphics.DrawImage(relays_open, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				}
				case DoorState.DoorLocked:
					SysLogServer.WriteLog("SetBakImg...JapanAF ", true);
					graphics.DrawImage(Resource.door_locked, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				case DoorState.NormalOpenOrRemoteOpen:
				{
					Bitmap relays_open = Resource.DoorOpened_Pull;
					graphics.DrawImage(relays_open, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				}
				case DoorState.DoorClosedOnNormalOpenTimeOrRemoteOpen:
				{
					Bitmap relays_open = Resource.Relays_open;
					graphics.DrawImage(relays_open, new Rectangle(this.m_paintStartX + num3, this.m_paintStarty + num4, num2, num), new Rectangle(0, 0, 40, 40), GraphicsUnit.Pixel);
					break;
				}
				}
				if (this.m_isSelected)
				{
					SysLogServer.WriteLog("SetBakImg...14 ", true);
					graphics.FillRectangle(this.SelectBrush, this.m_paintStartX + num3 - 2, this.m_paintStarty + num4 - 2, num2 + 4, num + 4);
				}
				int num5 = base.Height - this.m_PaintTxtHeight + 4;
				if (this.m_isSelected)
				{
					SysLogServer.WriteLog("SetBakImg...15 ", true);
					graphics.FillRectangle(this.SelectBrush, 1, num5, base.Width - 2, this.m_PaintTxtHeight);
				}
				string text = this.m_doorName;
				if (string.IsNullOrEmpty(text))
				{
					text = base.Name;
				}
				int num6 = 0;
				if (base.Width > this.m_TextWidth)
				{
					num6 = 2 + (base.Width - this.m_TextWidth) / 2;
				}
				else
				{
					num6 = 0;
					int num7 = base.Width * text.Length / this.m_TextWidth - 1;
					if (text.Length > num7)
					{
						text = text.Substring(0, num7) + "..";
					}
				}
				SysLogServer.WriteLog("SetBakImg...16 ", true);
				graphics.DrawString(text, this.IFont, this.FontBrush, (float)num6, (float)num5);
				try
				{
					if (this.BackgroundImage != null)
					{
						SysLogServer.WriteLog("SetBakImg...17 ", true);
						this.BackgroundImage.Dispose();
						this.BackgroundImage = null;
					}
				}
				catch
				{
					SysLogServer.WriteLog("SetBakImg...18 ", true);
				}
				SysLogServer.WriteLog("SetBakImg...19 ", true);
				this.BackgroundImage = bitmap;
				SysLogServer.WriteLog("SetBakImg...20 ", true);
				graphics.Dispose();
				graphics = null;
				this.Refresh();
			}
			catch
			{
				SysLogServer.WriteLog("SetBakImg...21 ", true);
			}
		}

		private void DevControl_SizeChanged(object sender, EventArgs e)
		{
			if (!this.m_ispaint)
			{
				base.SizeChanged -= this.DevControl_SizeChanged;
				if (base.Height < 70)
				{
					base.Height = 70;
				}
				if (base.Width < 80)
				{
					base.Width = 80;
				}
				base.SizeChanged += this.DevControl_SizeChanged;
			}
		}

		private Point GetOffset(Control ctl)
		{
			if (ctl.Parent != null)
			{
				Point result = default(Point);
				result.X = ctl.Left;
				result.Y = ctl.Top;
				Point offset = this.GetOffset(ctl.Parent);
				result.X += offset.X;
				result.Y += offset.Y;
				return result;
			}
			if (base.ParentForm != null)
			{
				Point result2 = default(Point);
				result2.X = ctl.Left + base.ParentForm.Left;
				result2.Y = ctl.Top + base.ParentForm.Top;
				return result2;
			}
			Point result3 = default(Point);
			result3.X = ctl.Left;
			result3.Y = ctl.Top;
			return result3;
		}

		private void GetXY(ref int x, ref int y)
		{
			Point p = base.PointToScreen(Control.MousePosition);
			p = base.PointToClient(p);
			x = p.X;
			y = p.Y;
			x += 3;
			y += 3;
			if (base.Parent != null)
			{
				if (base.Right + DevShowInfo.Instance.Width > base.Parent.Width)
				{
					x = x - DevShowInfo.Instance.Width - 6;
				}
				if (base.Bottom + DevShowInfo.Instance.Height > base.Parent.Height)
				{
					y = y - DevShowInfo.Instance.Height - 6;
				}
			}
		}

		private void DevControl_MouseEnter(object sender, EventArgs e)
		{
			if (!this.m_isShow && !this.m_ismovedown && this.m_machines != null)
			{
				Application.DoEvents();
				DevShowInfo.Instance.SetInfo(this.m_machines.MachineAlias, this.m_accdoor.door_no.ToString(), this.m_doorName, this.m_machines.IP, this.m_statusInfo);
				DevShowInfo.Instance.ShowEx();
				int left = 0;
				int top = 0;
				this.GetXY(ref left, ref top);
				DevShowInfo.Instance.Left = left;
				DevShowInfo.Instance.Top = top;
				this.m_isShow = true;
			}
		}

		private void DevControl_MouseLeave(object sender, EventArgs e)
		{
			Application.DoEvents();
			DevShowInfo.Instance.hide();
			this.m_isShow = false;
		}

		private void DevControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.n_isMove)
			{
				Application.DoEvents();
				DevShowInfo.Instance.HideEx();
				this.m_isShow = false;
				this.m_ismovedown = true;
				this.m_lastx = e.X;
				this.m_lasty = e.Y;
				this.m_startx = e.X;
				this.m_starty = e.Y;
				this.Cursor = Cursors.SizeAll;
			}
			if (!this.m_isSelected)
			{
				this.IsSelected = true;
			}
		}

		private void DevControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.m_isShow)
			{
				int left = 0;
				int top = 0;
				this.GetXY(ref left, ref top);
				DevShowInfo.Instance.Left = left;
				DevShowInfo.Instance.Top = top;
			}
			else if (this.n_isMove && this.m_ismovedown)
			{
				this.m_isMoving = true;
				if (Math.Abs(e.X - this.m_lastx) > 3 || Math.Abs(e.Y - this.m_lasty) > 3)
				{
					if (Math.Abs(e.X - this.m_lastx) > 3)
					{
						int num = base.Left + e.X - this.m_startx;
						if (num < 10)
						{
							base.Left = 10;
							this.DevControl_MouseUp(this, e);
						}
						else if (this.m_mapControl.VerticalScroll != null && this.m_mapControl.VerticalScroll.Visible)
						{
							if (num + base.Width + 40 > this.m_mapControl.Width)
							{
								base.Left = this.m_mapControl.Width - base.Width - 40;
								this.DevControl_MouseUp(this, e);
							}
							else
							{
								base.Left = num;
							}
						}
						else if (num + base.Width + 10 > this.m_mapControl.Width)
						{
							base.Left = this.m_mapControl.Width - base.Width - 10;
							this.DevControl_MouseUp(this, e);
						}
						else
						{
							base.Left = num;
						}
					}
					if (Math.Abs(e.Y - this.m_lasty) > 3)
					{
						int num2 = base.Top + e.Y - this.m_starty;
						if (base.Top < 10)
						{
							base.Top = 10;
							this.DevControl_MouseUp(this, e);
						}
						else if (this.m_mapControl.HorizontalScroll != null && this.m_mapControl.HorizontalScroll.Visible)
						{
							if (num2 + base.Height + 40 > this.m_mapControl.Height)
							{
								base.Top = this.m_mapControl.Height - base.Height - 40;
								this.DevControl_MouseUp(this, e);
							}
							else
							{
								base.Top = num2;
							}
						}
						else if (num2 + base.Height + 10 > this.m_mapControl.Height)
						{
							base.Top = this.m_mapControl.Height - base.Height - 10;
							this.DevControl_MouseUp(this, e);
						}
						else
						{
							base.Top = num2;
						}
					}
					this.m_lastx = e.X;
					this.m_lasty = e.Y;
				}
				this.m_isMoving = false;
			}
		}

		private void DevControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.n_isMove)
			{
				this.m_ismovedown = false;
				this.m_startx = 0;
				this.m_starty = 0;
				this.Cursor = Cursors.Default;
			}
		}

		private void ShowInfos(string infoStr)
		{
			if (base.Visible && !base.IsDisposed && this.m_waitForm != null)
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

		private void ShowProgress(int prg)
		{
			if (base.Visible && !base.IsDisposed && this.m_waitForm != null)
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

		private void menu_door_seting_Click(object sender, EventArgs e)
		{
			if (this.m_devServer != null && this.m_devServer.IsAddOk)
			{
				this.m_devServer.IsLock = true;
				if (DevLogServer.IsCmdFinish)
				{
					this.m_waitForm.HideEx(false);
					List<DevControl> list = null;
					if (this.m_mapControl != null)
					{
						list = this.m_mapControl.GetSelecteds();
					}
					if (list != null && list.Count > 1)
					{
						DoorNormalFormSeting doorNormalFormSeting = new DoorNormalFormSeting(this.m_accdoor.id);
						doorNormalFormSeting.ShowDialog();
						DevLogServer.ReSet();
						if (doorNormalFormSeting.normal_open)
						{
							DevLogServer.OpenType = -255;
							for (int i = 0; i < list.Count; i++)
							{
								DevLogServer.AddOpenDevControl(list[i]);
								DevLogServer.AllCount++;
							}
						}
						if (doorNormalFormSeting.enable_normal_open)
						{
							DevLogServer.OpenType = -100;
							for (int j = 0; j < list.Count; j++)
							{
								DevLogServer.AddOpenDevControl(list[j]);
								DevLogServer.AllCount++;
							}
						}
						if (doorNormalFormSeting.disnable_normal_open)
						{
							DevLogServer.CloseType = 2;
							for (int k = 0; k < list.Count; k++)
							{
								DevLogServer.AddCloseDevControl(list[k]);
								DevLogServer.AllCount++;
							}
						}
						if (!DevLogServer.IsCmdFinish)
						{
							DevLogServer.FinishEvent += this.DevLogServer_FinishEvent;
							DevLogServer.ShowInfoEvent += this.ShowInfos;
							DevLogServer.ShowProgressEvent += this.ShowProgress;
							this.m_waitForm.ShowEx();
							this.timer1.Enabled = true;
							DevLogServer.SetFinish = true;
						}
						doorNormalFormSeting.Dispose();
						doorNormalFormSeting = null;
					}
					else
					{
						DoorNormalFormSeting doorNormalFormSeting2 = new DoorNormalFormSeting(this.m_accdoor.id);
						doorNormalFormSeting2.ShowDialog();
						int num = 999;
						if (doorNormalFormSeting2.normal_open)
						{
							num = this.OpenDoor();
						}
						if (doorNormalFormSeting2.enable_normal_open)
						{
							num = this.NormalOpen(true);
						}
						if (doorNormalFormSeting2.disnable_normal_open)
						{
							num = this.NormalOpen(false);
						}
						if (num < 0)
						{
							if (-28 != num)
							{
								SysDialogs.ShowInfoMessage(PullSDkErrorInfos.GetInfo(num));
							}
						}
						else if (num != 999)
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						}
					}
				}
			}
		}

		private void menu_opendoor_Click(object sender, EventArgs e)
		{
			List<DevControl> list = null;
			OpenDoorSet openDoorSet = new OpenDoorSet();
			openDoorSet.ShowDialog();
			if (openDoorSet.Second != 0)
			{
				if (this.m_devServer == null || !this.m_devServer.IsAddOk)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
				}
				else if (this.m_mapControl != null)
				{
					list = this.m_mapControl.GetSelecteds();
					if (list != null && list.Count > 0)
					{
						this.m_waitForm.HideEx(false);
						MonitorWatchdog.OpenDoors(list, openDoorSet, this.m_waitForm);
					}
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (DevLogServer.IsOpenOrCloseFinish && DevLogServer.IsCmdFinish)
			{
				this.DevLogServer_FinishEvent(sender, e);
				this.timer1.Enabled = false;
			}
		}

		private void DevLogServer_FinishEvent(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new EventHandler(this.DevLogServer_FinishEvent), sender, e);
				}
				else
				{
					this.timer1.Enabled = false;
					DevLogServer.FinishEvent -= this.DevLogServer_FinishEvent;
					this.m_waitForm.ShowProgress(100);
					DevLogServer.ShowInfoEvent -= this.ShowInfos;
					DevLogServer.ShowProgressEvent -= this.ShowProgress;
					int rightCount = DevLogServer.RightCount;
					int num = DevLogServer.AllCount - rightCount;
					this.m_waitForm.ShowInfos(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + rightCount + " " + ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + num + "\r\n");
					this.m_waitForm.HideEx(false);
				}
			}
		}

		[Conditional("CustomRemoteOpenCloseDoor")]
		private void CustomRemoteSingleOpenDoor()
		{
			int num = 999;
			this.Status = DoorState.Open;
			num = this.OpenDoor();
			if (num < 0)
			{
				if (-28 == num)
				{
					this.NormalOpen(false);
					this.OpenDoor();
				}
			}
			else if (num == 999)
			{
				return;
			}
		}

		[Conditional("CustomRemoteOpenCloseDoor")]
		private void CustomRemoteMultiOpenDoor(OpenDoorSet openDoorSet, List<DevControl> m_selects)
		{
			if (openDoorSet.Second != 0)
			{
				DevLogServer.ReSet();
				DevLogServer.OpenType = openDoorSet.Second;
				for (int i = 0; i < m_selects.Count; i++)
				{
					DevLogServer.AddOpenDevControl(m_selects[i]);
					DevLogServer.AllCount++;
					m_selects[i].Status = DoorState.Open;
				}
				if (!DevLogServer.IsCmdFinish)
				{
					DevLogServer.FinishEvent += this.DevLogServer_FinishEvent;
					DevLogServer.ShowInfoEvent += this.ShowInfos;
					DevLogServer.ShowProgressEvent += this.ShowProgress;
					this.m_waitForm.ShowEx();
					this.timer1.Enabled = true;
					DevLogServer.SetFinish = true;
				}
			}
		}

		[Conditional("CustomRemoteOpenCloseDoor")]
		private void EnableMultiOpenDoor(OpenDoorSet openDoorSet, List<DevControl> m_selects)
		{
			openDoorSet.Second = -255;
		}

		[Conditional("CustomRemoteOpenCloseDoor")]
		private void CustomRemoteSingleCloseDoor()
		{
			int num = 999;
			this.Status = DoorState.Closed;
			num = this.CloseDoor();
			if (num < 0)
			{
				if (-28 == num)
				{
					this.NormalOpen(false);
					this.CloseDoor();
				}
			}
			else if (num == 999)
			{
				return;
			}
		}

		[Conditional("CustomRemoteOpenCloseDoor")]
		private void EnableMultiNormallyOpenDoor(CloseDoorSet closeDoorSet, List<DevControl> m_selects)
		{
			closeDoorSet.Second = 1;
		}

		[Conditional("CustomRemoteOpenCloseDoor")]
		private void CustomRemoteMultiCloseDoor(CloseDoorSet closeDoorSet, List<DevControl> m_selects)
		{
			if (closeDoorSet.Second > 0)
			{
				DevLogServer.ReSet();
				DevLogServer.CloseType = closeDoorSet.Second;
				for (int i = 0; i < m_selects.Count; i++)
				{
					DevLogServer.AddCloseDevControl(m_selects[i]);
					DevLogServer.AllCount++;
					m_selects[i].Status = DoorState.Closed;
				}
				if (!DevLogServer.IsCmdFinish)
				{
					DevLogServer.FinishEvent += this.DevLogServer_FinishEvent;
					DevLogServer.ShowInfoEvent += this.ShowInfos;
					DevLogServer.ShowProgressEvent += this.ShowProgress;
					this.m_waitForm.ShowProgress(0);
					this.m_waitForm.ShowEx();
					this.timer1.Enabled = true;
					DevLogServer.SetFinish = true;
				}
			}
		}

		[Conditional("RemoteOpenCloseDoor")]
		private void RemoteSingleOpenDoor(OpenDoorSet openDoorSet)
		{
			openDoorSet.ShowDialog();
			if (openDoorSet.Second != 0)
			{
				int num = 999;
				if (openDoorSet.Second == -255)
				{
					num = this.OpenDoor();
				}
				else if (openDoorSet.Second == -100)
				{
					num = this.NormalOpen(true);
				}
				else if (openDoorSet.Second > 0 && openDoorSet.Second < 255)
				{
					num = this.OpenDoor(openDoorSet.Second);
				}
				if (num < 0)
				{
					SysDialogs.ShowInfoMessage(PullSDkErrorInfos.GetInfo(num));
				}
				else if (num != 999)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
				}
			}
		}

		[Conditional("RemoteOpenCloseDoor")]
		private void RemoteMultiOpenDoor(OpenDoorSet openDoorSet, List<DevControl> m_selects)
		{
			openDoorSet.ShowDialog();
			if (openDoorSet.Second != 0)
			{
				DevLogServer.ReSet();
				DevLogServer.OpenType = openDoorSet.Second;
				for (int i = 0; i < m_selects.Count; i++)
				{
					DevLogServer.AddOpenDevControl(m_selects[i]);
					DevLogServer.AllCount++;
				}
				if (!DevLogServer.IsCmdFinish)
				{
					DevLogServer.FinishEvent += this.DevLogServer_FinishEvent;
					DevLogServer.ShowInfoEvent += this.ShowInfos;
					DevLogServer.ShowProgressEvent += this.ShowProgress;
					this.m_waitForm.ShowEx();
					this.timer1.Enabled = true;
					DevLogServer.SetFinish = true;
				}
			}
		}

		[Conditional("RemoteOpenCloseDoor")]
		private void RemoteSingleCloseDoor(CloseDoorSet closeDoorSet)
		{
			closeDoorSet.ShowDialog();
			if (closeDoorSet.Second > 0)
			{
				int num = 999;
				num = ((closeDoorSet.Second != 1) ? this.NormalOpen(false) : this.CloseDoor());
				if (num < 0)
				{
					SysDialogs.ShowInfoMessage(PullSDkErrorInfos.GetInfo(num));
				}
				else if (num != 999)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
				}
			}
		}

		[Conditional("RemoteOpenCloseDoor")]
		private void RemoteMultiCloseDoor(CloseDoorSet closeDoorSet, List<DevControl> m_selects)
		{
			closeDoorSet.ShowDialog();
			if (closeDoorSet.Second > 0)
			{
				DevLogServer.ReSet();
				DevLogServer.CloseType = closeDoorSet.Second;
				for (int i = 0; i < m_selects.Count; i++)
				{
					DevLogServer.AddCloseDevControl(m_selects[i]);
					DevLogServer.AllCount++;
				}
				if (!DevLogServer.IsCmdFinish)
				{
					DevLogServer.FinishEvent += this.DevLogServer_FinishEvent;
					DevLogServer.ShowInfoEvent += this.ShowInfos;
					DevLogServer.ShowProgressEvent += this.ShowProgress;
					this.m_waitForm.ShowProgress(0);
					this.m_waitForm.ShowEx();
					this.timer1.Enabled = true;
					DevLogServer.SetFinish = true;
				}
			}
		}

		private void menu_closedoor_Click(object sender, EventArgs e)
		{
			List<DevControl> list = null;
			CloseDoorSet closeDoorSet = new CloseDoorSet();
			closeDoorSet.ShowDialog();
			if (closeDoorSet.Second != -1)
			{
				if (this.m_devServer == null || !this.m_devServer.IsAddOk)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
				}
				else if (this.m_mapControl != null)
				{
					list = this.m_mapControl.GetSelecteds();
					if (list != null && list.Count > 0)
					{
						this.m_waitForm.HideEx(false);
						MonitorWatchdog.CloseDoor(list, closeDoorSet, this.m_waitForm);
					}
				}
			}
		}

		public int OpenDoor()
		{
			if (this.m_devServer != null && this.m_devServer.IsAddOk)
			{
				this.m_devServer.IsLock = true;
				if (this.m_devServer.IsConnected)
				{
					int result = this.m_devServer.OpenDoor(this.m_doorType);
					this.m_devServer.IsLock = false;
					return result;
				}
				this.m_devServer.IsLock = false;
				return -1001;
			}
			return -1002;
		}

		public int OpenDoor(int second)
		{
			if (this.m_devServer != null && this.m_devServer.IsAddOk)
			{
				this.m_devServer.IsLock = true;
				if (this.m_devServer.IsConnected)
				{
					int result = this.m_devServer.OpenDoor(this.m_doorType, second);
					this.m_devServer.IsLock = false;
					return result;
				}
				this.m_devServer.IsLock = false;
				return -1001;
			}
			return -1002;
		}

		public int CloseDoor()
		{
			if (this.m_devServer != null && this.m_devServer.IsAddOk)
			{
				this.m_devServer.IsLock = true;
				if (this.m_devServer.IsConnected)
				{
					int result = this.m_devServer.CloseDoor(this.m_doorType);
					this.m_devServer.IsLock = false;
					return result;
				}
				this.m_devServer.IsLock = false;
				return -1001;
			}
			return -1002;
		}

		public int NormalOpen(bool state)
		{
			if (this.m_devServer != null && this.m_devServer.IsAddOk)
			{
				this.m_devServer.IsLock = true;
				if (this.m_devServer.IsConnected)
				{
					int result = this.m_devServer.NormalOpenDoor(this.m_doorType, state);
					this.m_devServer.IsLock = false;
					return result;
				}
				this.m_devServer.IsLock = false;
				return -1001;
			}
			return -1002;
		}

		public int CancelAlarm()
		{
			if (this.m_isAlarm)
			{
				this.m_isAlarm = false;
				this.m_lastInfo = null;
				this.Status = DoorState.Connected;
			}
			if (this.m_devServer != null && this.m_devServer.IsAddOk)
			{
				this.m_devServer.IsLock = true;
				if (this.m_devServer.IsConnected)
				{
					int num = this.m_devServer.CancelAlarm();
					this.m_devServer.IsLock = false;
					if (num >= 0)
					{
						this.m_devServer.IsAlarm[(int)(this.DoorType - 1)] = false;
					}
					return num;
				}
				this.m_devServer.IsLock = false;
				return -1001;
			}
			return -1002;
		}

		private void btn_cancelAlarm_Click(object sender, EventArgs e)
		{
			DevControl.StopWarning = true;
			List<DevControl> list = null;
			if (this.m_devServer == null || !this.m_devServer.IsAddOk)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
			}
			else if (this.m_mapControl != null)
			{
				list = this.m_mapControl.GetSelecteds();
				if (list != null && list.Count > 0)
				{
					this.m_waitForm.HideEx(false);
					MonitorWatchdog.CancelAlarm(list, this.m_waitForm);
				}
			}
		}

		private void DevControl_Click(object sender, EventArgs e)
		{
			if (!this.m_isSelected)
			{
				this.IsSelected = true;
			}
		}

		private void DevControl_MouseClick(object sender, MouseEventArgs e)
		{
			if (!this.m_isSelected)
			{
				this.IsSelected = true;
			}
		}

		private void DevControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Control || e.KeyCode == Keys.ControlKey)
			{
				this.m_isMutilSelected = true;
				if (this.MutilSelectChangedEvent != null)
				{
					this.MutilSelectChangedEvent(this, e);
				}
			}
		}

		private void DevControl_KeyUp(object sender, KeyEventArgs e)
		{
			this.m_isMutilSelected = false;
			if (this.MutilSelectChangedEvent != null)
			{
				this.MutilSelectChangedEvent(this, e);
			}
		}

		private void DevControl_LocationChanged(object sender, EventArgs e)
		{
			if (this.n_isMove && this.m_ismovedown)
			{
				if (base.Left < 2)
				{
					base.Left = 2;
				}
				if (base.Top < 2)
				{
					base.Top = 2;
				}
				if (base.Parent != null)
				{
					if (base.Left + base.Width + 10 > base.Parent.Width)
					{
						base.Left = base.Parent.Width - base.Width - 10;
					}
					if (base.Top + base.Height + 10 > base.Parent.Height)
					{
						base.Top = base.Parent.Height - base.Height - 10;
					}
				}
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

		public void Free()
		{
			try
			{
				this.timer1.Dispose();
				this.timer1 = null;
			}
			catch
			{
			}
			try
			{
				if (base.Controls.Count > 0)
				{
					for (int i = 0; i < base.Controls.Count; i++)
					{
						if (base.Controls[i] != null)
						{
							this.DisposeEx(base.Controls[i]);
						}
					}
					base.Controls.Clear();
				}
			}
			catch
			{
			}
			try
			{
				if (this.m_devServer != null)
				{
					this.m_devServer.DoorStateEvent -= this.server_DoorStateEvent;
					this.m_devServer.ListenChangedEvent -= this.m_devServer_ListenChangedEvent;
					this.m_devServer = null;
				}
			}
			catch
			{
			}
			this.m_waitForm = null;
			this._font = null;
			this._borderPen = null;
			this._selectPen = null;
			this._selectbrush = null;
			this._fontbrush = null;
			this._forebrush = null;
			this._brush = null;
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
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_opendoor = new ToolStripMenuItem();
			this.menu_closedoor = new ToolStripMenuItem();
			this.menu_cancelAlarm = new ToolStripMenuItem();
			this.menu_door_seting = new ToolStripMenuItem();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[4]
			{
				this.menu_opendoor,
				this.menu_closedoor,
				this.menu_cancelAlarm,
				this.menu_door_seting
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(153, 114);
			this.menu_opendoor.Image = Resources.remoteOpening;
			this.menu_opendoor.Name = "menu_opendoor";
			this.menu_opendoor.Size = new Size(152, 22);
			this.menu_opendoor.Text = "远程开门";
			this.menu_opendoor.Click += this.menu_opendoor_Click;
			this.menu_closedoor.Image = Resources.remoteClosing;
			this.menu_closedoor.Name = "menu_closedoor";
			this.menu_closedoor.Size = new Size(152, 22);
			this.menu_closedoor.Text = "远程关门";
			this.menu_closedoor.Click += this.menu_closedoor_Click;
			this.menu_cancelAlarm.Image = Resources.cancelAlarm;
			this.menu_cancelAlarm.Name = "menu_cancelAlarm";
			this.menu_cancelAlarm.Size = new Size(152, 22);
			this.menu_cancelAlarm.Text = "取消报警";
			this.menu_cancelAlarm.Click += this.btn_cancelAlarm_Click;
			this.menu_door_seting.Image = Resources.setting;
			this.menu_door_seting.Name = "menu_door_seting";
			this.menu_door_seting.Size = new Size(152, 22);
			this.menu_door_seting.Text = "开门设置";
			this.menu_door_seting.Visible = false;
			this.menu_door_seting.Click += this.menu_door_seting_Click;
			this.timer1.Interval = 3000;
			this.timer1.Tick += this.timer1_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.ContextMenuStrip = this.contextMenuStrip1;
			base.Name = "DevControl";
			base.Size = new Size(80, 71);
			base.LocationChanged += this.DevControl_LocationChanged;
			base.SizeChanged += this.DevControl_SizeChanged;
			base.Click += this.DevControl_Click;
			base.KeyDown += this.DevControl_KeyDown;
			base.KeyUp += this.DevControl_KeyUp;
			base.MouseClick += this.DevControl_MouseClick;
			base.MouseDown += this.DevControl_MouseDown;
			base.MouseEnter += this.DevControl_MouseEnter;
			base.MouseLeave += this.DevControl_MouseLeave;
			base.MouseMove += this.DevControl_MouseMove;
			base.MouseUp += this.DevControl_MouseUp;
			this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
