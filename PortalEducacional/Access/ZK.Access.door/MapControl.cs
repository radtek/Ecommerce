/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.door
{
	public class MapControl : UserControl
	{
		private UserInfoBll userBll;

		private int m_mapid = 0;

		private Dictionary<string, UserInfo> m_userList = new Dictionary<string, UserInfo>();

		private Dictionary<string, UserInfo> m_userListLastName = new Dictionary<string, UserInfo>();

		private bool m_loadUser = false;

		private Dictionary<int, string> stateList = new Dictionary<int, string>();

		private Dictionary<int, string> verifiedList = new Dictionary<int, string>();

		private Dictionary<int, string> inAddressList = new Dictionary<int, string>();

		private Dictionary<int, string> outAddressList = new Dictionary<int, string>();

		private Dictionary<int, string> linkAgeIOList = new Dictionary<int, string>();

		private Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_InAuxAddress;

		private Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_OutAuxAddress;

		private Dictionary<int, Dictionary<int, AccReader>> dicDoorIdInOutState_Reader;

		private Dictionary<int, Dictionary<int, AccDoor>> dicDevIdDoorNo_Door = new Dictionary<int, Dictionary<int, AccDoor>>();

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private static DataTable m_datatable = new DataTable();

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private ObjRTLogInfo m_lastInfo = null;

		private PhotoShow m_photoShow = new PhotoShow();

		private object objShowPhotoLock = new object();

		private object objPhotoQueueLock = new object();

		private List<UserInfo> lstUserToShowPhoto = new List<UserInfo>();

		private Dictionary<string, int> dicPin_UserIdHasPhoto;

		private Thread threadShowPhoto;

		private DateTime dt = DateTime.Now;

		private DateTime m_date = DateTime.Now;

		private IContainer components = null;

		private ContextMenuStrip menu_Strip;

		private ToolStripMenuItem menu_registerCardNo;

		private System.Windows.Forms.Timer timer_hide;

		private System.Windows.Forms.Timer time_load;

		private Panel pnl_show;

		private PictureBox pic_close;

		private PictureBox pic_photo;

		private PanelEx panelEx1;

		private Panel pnl_main;

		private DevMapControl map_main;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_dete;

		private GridColumn column_dev;

		private GridColumn column_doorno;

		private GridColumn column_event;

		private GridColumn column_cardno;

		private GridColumn column_no;

		private GridColumn column_inoutstate;

		private GridColumn column_vilify;

		private ExpandableSplitter expandableSplitter1;

		private Panel panel1;

		public TabItem DevTabItem
		{
			get
			{
				return this.map_main.DevTabItem;
			}
			set
			{
				this.map_main.DevTabItem = value;
			}
		}

		public DevMapControl Map
		{
			get
			{
				SysLogServer.WriteLog("MapControl...Map...1", true);
				return this.map_main;
			}
		}

		public List<DevControl> DoorControls => this.map_main.DoorControls;

		public MapControl()
		{
			SysLogServer.WriteLog("MapControl...1", true);
			this.InitializeComponent();
			SysLogServer.WriteLog("MapControl...2", true);
			try
			{
				this.userBll = new UserInfoBll(MainForm._ia);
			}
			catch (Exception)
			{
			}
		}

		private void InitUsers()
		{
			try
			{
				if (!this.m_loadUser)
				{
					this.m_loadUser = true;
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					List<UserInfo> modelList = userInfoBll.GetModelList(" DEFAULTDEPTID <> -1 ");
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							if (!this.m_userList.ContainsKey(modelList[i].BadgeNumber.ToString()))
							{
								this.m_userList.Add(modelList[i].BadgeNumber.ToString(), modelList[i]);
							}
						}
					}
					userInfoBll = null;
					modelList.Clear();
					modelList = null;
				}
			}
			catch (Exception)
			{
			}
		}

		private void LoadStateInfo()
		{
			this.stateList.Clear();
			this.stateList = InOutStateInfo.GetDic();
			this.menu_registerCardNo.Enabled = false;
			this.menu_registerCardNo.Visible = false;
		}

		private void LoadverifiedInfo()
		{
			this.verifiedList.Clear();
			this.verifiedList = PullSDKVerifyTypeInfos.GetDic();
		}

		private void LoadInAddressInfo()
		{
			this.inAddressList.Clear();
			this.inAddressList = InAddressInfo.GetDic();
		}

		private void LoadOutAddressInfo()
		{
			this.outAddressList.Clear();
			this.outAddressList = OutAddressInfo.GetDic();
		}

		private void LoadAuxAddressInfo()
		{
			this.Dev_InAuxAddress = new Dictionary<int, Dictionary<int, AccAuxiliary>>();
			this.Dev_OutAuxAddress = new Dictionary<int, Dictionary<int, AccAuxiliary>>();
			AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
			List<AccAuxiliary> modelList = accAuxiliaryBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					switch (modelList[i].AuxState)
					{
					case AccAuxiliary.AccAuxiliaryState.In:
						if (this.Dev_InAuxAddress.ContainsKey(modelList[i].DeviceId))
						{
							Dictionary<int, AccAuxiliary> dictionary = this.Dev_InAuxAddress[modelList[i].DeviceId];
							if (!dictionary.ContainsKey(modelList[i].AuxNo))
							{
								dictionary.Add(modelList[i].AuxNo, modelList[i]);
							}
						}
						else
						{
							Dictionary<int, AccAuxiliary> dictionary = new Dictionary<int, AccAuxiliary>();
							dictionary.Add(modelList[i].AuxNo, modelList[i]);
							this.Dev_InAuxAddress.Add(modelList[i].DeviceId, dictionary);
						}
						break;
					case AccAuxiliary.AccAuxiliaryState.Out:
						if (this.Dev_OutAuxAddress.ContainsKey(modelList[i].DeviceId))
						{
							Dictionary<int, AccAuxiliary> dictionary = this.Dev_OutAuxAddress[modelList[i].DeviceId];
							if (!dictionary.ContainsKey(modelList[i].AuxNo))
							{
								dictionary.Add(modelList[i].AuxNo, modelList[i]);
							}
						}
						else
						{
							Dictionary<int, AccAuxiliary> dictionary = new Dictionary<int, AccAuxiliary>();
							dictionary.Add(modelList[i].AuxNo, modelList[i]);
							this.Dev_OutAuxAddress.Add(modelList[i].DeviceId, dictionary);
						}
						break;
					}
				}
			}
		}

		private void LoadlinkAgeIOInfo()
		{
			this.linkAgeIOList.Clear();
			this.linkAgeIOList = LinkAgeIOInfo.GetDic();
		}

		private void LoadReader()
		{
			AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
			List<AccReader> modelList = accReaderBll.GetModelList("");
			if (modelList != null)
			{
				this.dicDoorIdInOutState_Reader = new Dictionary<int, Dictionary<int, AccReader>>();
				for (int i = 0; i < modelList.Count; i++)
				{
					AccReader accReader = modelList[i];
					if (!this.dicDoorIdInOutState_Reader.ContainsKey(accReader.DoorId))
					{
						Dictionary<int, AccReader> dictionary = new Dictionary<int, AccReader>();
						dictionary.Add((int)accReader.ReaderState, accReader);
						this.dicDoorIdInOutState_Reader.Add(accReader.DoorId, dictionary);
					}
					else
					{
						Dictionary<int, AccReader> dictionary = this.dicDoorIdInOutState_Reader[accReader.DoorId];
						if (!dictionary.ContainsKey((int)accReader.ReaderState))
						{
							dictionary.Add((int)accReader.ReaderState, accReader);
						}
						else
						{
							dictionary[(int)accReader.ReaderState] = accReader;
						}
					}
				}
			}
		}

		private void InitDoor()
		{
			this.dlist.Clear();
			this.dicDevIdDoorNo_Door.Clear();
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!this.dlist.ContainsKey(modelList[i].id))
					{
						this.dlist.Add(modelList[i].id, modelList[i]);
					}
					if (!this.dicDevIdDoorNo_Door.ContainsKey(modelList[i].device_id))
					{
						Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
						dictionary.Add(modelList[i].door_no, modelList[i]);
						this.dicDevIdDoorNo_Door.Add(modelList[i].device_id, dictionary);
					}
					else
					{
						Dictionary<int, AccDoor> dictionary = this.dicDevIdDoorNo_Door[modelList[i].device_id];
						if (!dictionary.ContainsKey(modelList[i].door_no))
						{
							dictionary.Add(modelList[i].door_no, modelList[i]);
						}
					}
				}
			}
			accDoorBll = null;
			modelList.Clear();
			modelList = null;
		}

		private void InitDataTableSet()
		{
			if (MapControl.m_datatable == null)
			{
				MapControl.m_datatable = new DataTable();
			}
			if (MapControl.m_datatable.Columns.Count <= 0)
			{
				MapControl.m_datatable.Columns.Clear();
				MapControl.m_datatable.Columns.Add("date");
				MapControl.m_datatable.Columns.Add("dev");
				MapControl.m_datatable.Columns.Add("doorno");
				MapControl.m_datatable.Columns.Add("event");
				MapControl.m_datatable.Columns.Add("cardno");
				MapControl.m_datatable.Columns.Add("no");
				MapControl.m_datatable.Columns.Add("inoutstate");
				MapControl.m_datatable.Columns.Add("vilify");
				MapControl.m_datatable.Columns.Add("eventtype");
			}
			this.column_dete.FieldName = "date";
			this.column_dev.FieldName = "dev";
			this.column_event.FieldName = "event";
			this.column_cardno.FieldName = "cardno";
			this.column_no.FieldName = "no";
			this.column_doorno.FieldName = "doorno";
			this.column_inoutstate.FieldName = "inoutstate";
			this.column_vilify.FieldName = "vilify";
			try
			{
				this.grd_view.DataSource = MapControl.m_datatable;
			}
			catch (Exception ex)
			{
				LogServer.Log("MapControl.InitDataTableSet.BindDataTable Exception: " + ex.Message);
			}
		}

		public void Load()
		{
			SysLogServer.WriteLog("MapControl...Load...1", true);
			this.InitDataTableSet();
			this.InitMachines();
			this.LoadStateInfo();
			this.LoadverifiedInfo();
			this.LoadInAddressInfo();
			this.LoadAuxAddressInfo();
			this.LoadReader();
			this.InitDoor();
			initLang.LocaleForm(this, "MapControl");
			SysLogServer.WriteLog("MapControl grd_mainView.Appearance...", true);
			this.grd_mainView.Appearance.HideSelectionRow.BackColor = Color.FromArgb(0, 0, 0, 0);
			this.grd_mainView.Appearance.HideSelectionRow.BackColor2 = Color.FromArgb(0, 0, 0, 0);
			this.grd_mainView.Appearance.SelectedRow.BackColor = Color.FromArgb(0, 0, 0, 0);
			this.grd_mainView.Appearance.SelectedRow.BackColor2 = Color.FromArgb(0, 0, 0, 0);
			this.grd_mainView.Appearance.FocusedRow.BackColor = Color.FromArgb(0, 0, 0, 0);
			this.grd_mainView.Appearance.FocusedRow.BackColor2 = Color.FromArgb(0, 0, 0, 0);
			SysLogServer.WriteLog("benging pnl_main_Size...", true);
			this.pnl_main_SizeChanged(this, null);
			SysLogServer.WriteLog("MapControl...Load...2", true);
		}

		public void InitMap(int mapid)
		{
			this.m_mapid = mapid;
			SysLogServer.WriteLog("InitMap...1", true);
			this.map_main.LoadFinishEvent -= this.map_main_LoadFinishEvent;
			this.map_main.LoadFinishEvent += this.map_main_LoadFinishEvent;
			SysLogServer.WriteLog("InitMap...2", true);
			this.map_main.LoadMap(this.m_mapid);
			SysLogServer.WriteLog("InitMap...3", true);
			this.grd_mainView_Click(null, null);
		}

		private void map_main_LoadFinishEvent(object sender, EventArgs e)
		{
			try
			{
				SysLogServer.WriteLog("map_main_LoadFinishEvent...1", true);
				this.LoadEvent();
				SysLogServer.WriteLog("map_main_LoadFinishEvent...2", true);
				if (this.m_mapid > 0)
				{
					SysLogServer.WriteLog("map_main_LoadFinishEvent...3", true);
					List<DevControl> doorControls = this.map_main.DoorControls;
					SysLogServer.WriteLog("map_main_LoadFinishEvent...4", true);
					if (doorControls != null)
					{
						SysLogServer.WriteLog("map_main_LoadFinishEvent...5", true);
						for (int i = 0; i < doorControls.Count; i++)
						{
							doorControls[i].IsMove = true;
						}
						SysLogServer.WriteLog("map_main_LoadFinishEvent...6", true);
					}
					this.pnl_main.Height = 400;
				}
				SysLogServer.WriteLog("map_main_LoadFinishEvent...7", true);
				this.map_main_SizeChanged(null, null);
				SysLogServer.WriteLog("map_main_LoadFinishEvent...8", true);
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("map_main_LoadFinishEvent...ex.message=" + ex.Message, true);
			}
		}

		private void InitMachines()
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
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(list[i]);
						if (deviceServer != null)
						{
							deviceServer.DevInfo.ID = list[i].ID;
							this.mlist.Add(list[i].ID, list[i]);
						}
					}
				}
			}
			machinesBll = null;
			list.Clear();
			list = null;
		}

		private void LoadEvent()
		{
			if (DeviceServers.Instance.Count > 0)
			{
				SysLogServer.WriteLog("LoadEvent...1", true);
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("IsListen");
				bool isNeedListen = true;
				if (string.IsNullOrEmpty(nodeValueByName))
				{
					AppSite.Instance.SetNodeValue("IsListen", "true");
				}
				else
				{
					isNeedListen = (nodeValueByName == "true" && true);
				}
				SysLogServer.WriteLog("LoadEvent...2", true);
				int num = 0;
				for (num = 0; num < DeviceServers.Instance.Count; num++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[num];
					if (deviceServerBll != null)
					{
						SysLogServer.WriteLog("LoadEvent...3", true);
						deviceServerBll.RTLogEvent -= this.devServer_RTLogEvent;
						deviceServerBll.IsNeedListen = isNeedListen;
						deviceServerBll.RTLogEvent += this.devServer_RTLogEvent;
					}
				}
			}
		}

		public void devServer_RTLogEvent(ObjDevice sender, ObjRTLogInfo info)
		{
			if (!base.IsDisposed && base.Visible)
			{
				if (base.InvokeRequired)
				{
					base.BeginInvoke((MethodInvoker)delegate
					{
						this.devServer_RTLogEvent(sender, info);
					});
				}
				else
				{
					this.RTLogEvent(info);
					MapControl._checkCardBoxEvent(info);
				}
			}
		}

		private static void _checkCardBoxEvent(ObjRTLogInfo info)
		{
			DeviceServers.CheckCardBox(info);
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
			Form form = base.FindForm();
			if (form != null)
			{
				x = form.Width - this.m_photoShow.Width - 25;
				y = form.Height - this.m_photoShow.Height - 30;
			}
			else
			{
				Point p = new Point(base.Width, base.Height);
				p = base.PointToClient(p);
				x = p.X - this.m_photoShow.Width - 20;
				y = p.Y - this.m_photoShow.Height - 30;
			}
		}

		public void RTLogEvent(ObjRTLogInfo info)
		{
			try
			{
				SysLogServer.WriteLog("RTLogEvent...1", true);
				if (info != null)
				{
					int num = -1;
					MapControl.m_datatable.BeginLoadData();
					if (info.EType != EventType.Type255)
					{
						int[] selectedRows = this.grd_mainView.GetSelectedRows();
						if (selectedRows != null && selectedRows.Length != 0)
						{
							this.grd_mainView.UnselectRow(selectedRows[0]);
						}
						List<DevControl> doorControls = this.map_main.DoorControls;
						bool flag = false;
						int num2 = 0;
						while (num2 < doorControls.Count)
						{
							if (doorControls[num2].AccDoorInfo.device_id != info.DevID || !doorControls[num2].Visible)
							{
								num2++;
								continue;
							}
							num = doorControls[num2].DevInfo.DeviceType;
							flag = true;
							break;
						}
						if (flag)
						{
							this.InitUsers();
							DataRow dataRow = MapControl.m_datatable.NewRow();
							dataRow[0] = info.Date;
							if (this.mlist.ContainsKey(info.DevID))
							{
								dataRow[1] = this.mlist[info.DevID].MachineAlias;
							}
							else
							{
								dataRow[1] = info.IP;
							}
							dataRow[2] = "";
							if (info.EType != EventType.Type255 && this.dicDevIdDoorNo_Door.ContainsKey(info.DevID))
							{
								Dictionary<int, AccDoor> dictionary = this.dicDevIdDoorNo_Door[info.DevID];
								if (dictionary.ContainsKey(int.Parse(info.DoorID)))
								{
									dataRow[2] = dictionary[int.Parse(info.DoorID)].door_name;
								}
							}
							if (info.EType == EventType.Type220 || info.EType == EventType.Type221)
							{
								string value = string.Empty;
								int key = int.Parse(info.DoorID);
								if (this.Dev_InAuxAddress.ContainsKey(info.DevID))
								{
									Dictionary<int, AccAuxiliary> dictionary2 = this.Dev_InAuxAddress[info.DevID];
									if (dictionary2.ContainsKey(key))
									{
										value = dictionary2[key].AuxName;
									}
								}
								dataRow[2] = value;
								dataRow[3] = PullSDKEventInfos.GetInfo(info.EType);
							}
							if (info.EType == EventType.Type6)
							{
								string text = PullSDKEventInfos.GetInfo(info.EType);
								string cardNo = info.CardNo;
								if (!string.IsNullOrEmpty(cardNo))
								{
									try
									{
										int id = int.Parse(cardNo);
										AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
										AccLinkAgeIo model = accLinkAgeIoBll.GetModel(id);
										if (model != null)
										{
											if (!string.IsNullOrEmpty(info.VerifyType))
											{
												text = text + "[" + PullSDKEventInfos.GetInfo((EventType)Enum.Parse(typeof(EventType), info.VerifyType)) + "]";
											}
											text = text + "(" + model.linkage_name + ")";
										}
									}
									catch
									{
									}
								}
								dataRow[3] = text;
								info.VerifyType = "200";
								info.CardNo = string.Empty;
							}
							else if (info.EType == EventType.Type220 || info.EType == EventType.Type221)
							{
								string value2 = string.Empty;
								int key2 = int.Parse(info.DoorID);
								if (this.Dev_InAuxAddress.ContainsKey(info.DevID))
								{
									Dictionary<int, AccAuxiliary> dictionary3 = this.Dev_InAuxAddress[info.DevID];
									if (dictionary3.ContainsKey(key2))
									{
										value2 = dictionary3[key2].AuxName;
									}
								}
								dataRow[2] = value2;
								dataRow[3] = PullSDKEventInfos.GetInfo(info.EType);
							}
							else
							{
								dataRow[3] = PullSDKEventInfos.GetInfo(info.EType);
							}
							if (!string.IsNullOrEmpty(info.CardNo) && info.CardNo != "0")
							{
								dataRow[4] = info.CardNo;
								if (AccCommon.CodeVersion == CodeVersionType.JapanAF && ulong.TryParse(info.CardNo, out ulong num3))
								{
									dataRow[4] = num3.ToString("X");
								}
							}
							else
							{
								dataRow[4] = "";
							}
							if (!string.IsNullOrEmpty(info.Pin) && info.Pin != "0")
							{
								if (this.m_userList.ContainsKey(info.Pin))
								{
									if (!string.IsNullOrEmpty(this.m_userList[info.Pin].Name))
									{
										if (AppSite.Instance.GetNodeValueByName("Language") != "chs")
										{
											dataRow[5] = info.Pin + "(" + this.m_userList[info.Pin].Name + " " + this.m_userList[info.Pin].LastName + ")";
										}
										else
										{
											dataRow[5] = info.Pin + "(" + this.m_userList[info.Pin].LastName + this.m_userList[info.Pin].Name + ")";
										}
									}
									else
									{
										dataRow[5] = info.Pin;
									}
									lock (this.objPhotoQueueLock)
									{
										if (this.dicPin_UserIdHasPhoto == null)
										{
											if (this.userBll == null)
											{
												this.userBll = new UserInfoBll(MainForm._ia);
											}
											this.dicPin_UserIdHasPhoto = this.userBll.GetBadgenumber_UserIdDic("[PHOTO] is not null");
										}
										if (this.dicPin_UserIdHasPhoto.ContainsKey(info.Pin))
										{
											this.lstUserToShowPhoto.Add(this.m_userList[info.Pin]);
										}
									}
									ThreadPool.QueueUserWorkItem(delegate
									{
										try
										{
											this.ShowUserPhoto();
										}
										catch (Exception ex2)
										{
											SysLogServer.WriteLog("Exception in MapControl.ShowUserPhoto: " + ex2.Message, true);
										}
									});
								}
								else
								{
									dataRow[5] = info.Pin;
								}
							}
							else
							{
								dataRow[5] = "";
							}
							if (!string.IsNullOrEmpty(info.InOutStatus))
							{
								int key3 = int.Parse(info.InOutStatus) & 0xF;
								bool flag2 = false;
								int key4 = 0;
								int.TryParse(info.DoorID, out int key5);
								if (this.dicDevIdDoorNo_Door.ContainsKey(info.DevID))
								{
									Dictionary<int, AccDoor> dictionary4 = this.dicDevIdDoorNo_Door[info.DevID];
									if (dictionary4.ContainsKey(key5))
									{
										key4 = dictionary4[key5].id;
									}
								}
								if (this.dicDoorIdInOutState_Reader != null && this.dicDoorIdInOutState_Reader.ContainsKey(key4))
								{
									Dictionary<int, AccReader> dictionary5 = this.dicDoorIdInOutState_Reader[key4];
									if (dictionary5.ContainsKey(key3))
									{
										dataRow[6] = dictionary5[key3].ReaderName;
										flag2 = true;
									}
								}
								if (!flag2)
								{
									if (this.stateList.ContainsKey(key3))
									{
										dataRow[6] = this.stateList[key3];
									}
									else
									{
										dataRow[6] = "";
									}
								}
							}
							else
							{
								dataRow[6] = "";
							}
							if (!string.IsNullOrEmpty(info.VerifyType) && this.verifiedList.ContainsKey(int.Parse(info.VerifyType)))
							{
								dataRow[7] = this.verifiedList[int.Parse(info.VerifyType)];
							}
							else
							{
								dataRow[7] = "";
							}
							dataRow[8] = (int)info.EType;
							MapControl.m_datatable.Rows.InsertAt(dataRow, 0);
							if (MapControl.m_datatable.Rows.Count > 100)
							{
								while (MapControl.m_datatable.Rows.Count > 50)
								{
									MapControl.m_datatable.Rows.RemoveAt(50);
								}
							}
							this.grd_view.DataSource = null;
							goto IL_0946;
						}
						goto end_IL_0001;
					}
					goto IL_0946;
				}
				goto IL_096a;
				IL_096a:
				if (AccCommon.CodeVersion == CodeVersionType.OpenDoorWarning)
				{
					string devName = info.IP;
					string doorName = info.DoorID;
					int.TryParse(info.DoorID, out int key6);
					if (this.mlist.ContainsKey(info.DevID))
					{
						devName = this.mlist[info.DevID].MachineAlias;
					}
					if (this.dlist.ContainsKey(key6))
					{
						doorName = this.dlist[key6].door_name;
					}
					DevControl.OpenDoorWarning(info, devName, doorName);
				}
				goto end_IL_0001;
				IL_0946:
				this.grd_view.DataSource = MapControl.m_datatable;
				MapControl.m_datatable.EndLoadData();
				this.m_lastInfo = info;
				goto IL_096a;
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				LogServer.Log("MapControl.RTLogEvent Exception: " + ex.Message, true);
			}
		}

		private void ShowUserPhoto()
		{
			UserInfo UserTmp = null;
			List<UserInfo> lstUserTmp = null;
			List<UserInfo> lstUserInQueue = null;
			if (Monitor.TryEnter(this.objShowPhotoLock))
			{
				try
				{
					ThreadState threadState = ThreadState.Unstarted;
					if (this.threadShowPhoto != null)
					{
						threadState = this.threadShowPhoto.ThreadState;
					}
					if (this.threadShowPhoto == null || (threadState != 0 && threadState != ThreadState.WaitSleepJoin))
					{
						if (this.threadShowPhoto != null)
						{
							LogServer.Log("threadShowPhoto's ThreadState is " + threadState.ToString(), true);
							try
							{
								this.threadShowPhoto.Abort();
							}
							catch (Exception ex)
							{
								LogServer.Log("Abort threadShowPhoto Exception: " + ex.Message, true);
							}
						}
						if (this.userBll == null)
						{
							this.userBll = new UserInfoBll(MainForm._ia);
						}
						this.threadShowPhoto = new Thread((ParameterizedThreadStart)delegate
						{
							LogServer.Log("Thread ShowUserPhoto Started.", true);
							while (!base.IsDisposed && !MonitorWatchdog.ShouldStop)
							{
								UserTmp = null;
								lock (this.objPhotoQueueLock)
								{
									if (this.lstUserToShowPhoto == null || this.lstUserToShowPhoto.Count <= 0)
									{
										lstUserInQueue = null;
									}
									else
									{
										lstUserInQueue = this.lstUserToShowPhoto;
										this.lstUserToShowPhoto = new List<UserInfo>();
									}
								}
								if (lstUserInQueue == null || lstUserInQueue.Count <= 0)
								{
									Thread.Sleep(500);
								}
								else
								{
									try
									{
										for (int num = lstUserInQueue.Count - 1; num >= 0; num--)
										{
											lstUserTmp = this.userBll.GetModelList("UserId=" + lstUserInQueue[num].UserId, true);
											if (lstUserTmp != null && lstUserTmp.Count > 0)
											{
												if (lstUserTmp[0].Photo != null)
												{
													UserTmp = lstUserTmp[0];
													break;
												}
												if (this.dicPin_UserIdHasPhoto.ContainsKey(lstUserTmp[0].BadgeNumber))
												{
													this.dicPin_UserIdHasPhoto.Remove(lstUserTmp[0].BadgeNumber);
												}
											}
										}
									}
									catch (Exception ex3)
									{
										LogServer.Log("Exception in MapControl.ShowUserPhoto.GetUser: " + ex3.Message, true);
									}
									if (UserTmp != null && UserTmp.Photo != null)
									{
										this.ShowPhoto(UserTmp.Photo);
									}
									Thread.Sleep(500);
								}
							}
							LogServer.Log("Thread ShowUserPhoto Stoped.", true);
						});
						this.threadShowPhoto.Start();
					}
				}
				catch (Exception ex2)
				{
					LogServer.Log("Exception in MapControl.ShowUserPhoto: " + ex2.Message, true);
				}
				finally
				{
					Monitor.Exit(this.objShowPhotoLock);
				}
			}
		}

		private void ShowPhoto(byte[] data)
		{
			if (data != null)
			{
				try
				{
					if (!base.IsDisposed && base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.ShowPhoto(data);
						});
					}
					else
					{
						MemoryStream memoryStream = new MemoryStream(data);
						Image image = Image.FromStream(memoryStream);
						this.pic_photo.Image = image;
						this.pic_photo.Visible = true;
						this.pnl_show.Visible = true;
						int.TryParse(AppSite.Instance.GetNodeValueByName("PhotoDelayTimeOnMonitoring"), out int num);
						if (num <= 0)
						{
							num = 15;
						}
						this.timer_hide.Interval = num * 1000;
						this.timer_hide.Enabled = true;
						this.m_date = DateTime.Now;
						memoryStream.Close();
						memoryStream = null;
					}
				}
				catch (Exception ex)
				{
					SysLogServer.WriteLog("Exception in MapControl.ShowPhoto: " + ex.Message, true);
				}
			}
		}

		private void grd_mainView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
		{
			initLang.CustomDrawRowIndicator(sender, e);
		}

		private void map_main_DoubleClick(object sender, EventArgs e)
		{
			if (this.dt.AddMilliseconds(500.0) < DateTime.Now)
			{
				this.map_main.DoubleClick -= this.map_main_DoubleClick;
				MapFullScreen mapFullScreen = new MapFullScreen(this.map_main);
				mapFullScreen.ShowDialog();
				this.dt = DateTime.Now;
				this.map_main.DoubleClick += this.map_main_DoubleClick;
			}
		}

		private void menu_registerCardNo_Click(object sender, EventArgs e)
		{
			int[] selectedRows = this.grd_mainView.GetSelectedRows();
			selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.grd_mainView, selectedRows);
			if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < MapControl.m_datatable.Rows.Count)
			{
				string a = this.grd_mainView.GetRowCellValue(selectedRows[0], this.column_event).ToString();
				if (a == PullSDKEventInfos.GetInfo(EventType.Type27))
				{
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					string text = this.grd_mainView.GetRowCellValue(selectedRows[0], this.column_cardno).ToString();
					if (!userInfoBll.ExistsCardNo(text))
					{
						PersonnelManagementForm personnelManagementForm = new PersonnelManagementForm(text);
						personnelManagementForm.ShowDialog();
						if (userInfoBll.ExistsCardNo(text))
						{
							this.menu_registerCardNo.Enabled = false;
						}
					}
					else
					{
						this.menu_registerCardNo.Enabled = false;
					}
				}
				else
				{
					this.menu_registerCardNo.Enabled = false;
				}
			}
			else
			{
				this.menu_registerCardNo.Enabled = false;
			}
		}

		private void grd_mainView_Click(object sender, EventArgs e)
		{
			DataRow focusedDataRow = this.grd_mainView.GetFocusedDataRow();
			int num;
			if (focusedDataRow == null)
			{
				this.menu_registerCardNo.Enabled = false;
				this.menu_registerCardNo.Visible = false;
			}
			else if (!int.TryParse(focusedDataRow[8].ToString(), out num))
			{
				this.menu_registerCardNo.Enabled = false;
				this.menu_registerCardNo.Visible = false;
			}
			else if (27 == num)
			{
				this.menu_registerCardNo.Enabled = true;
				this.menu_registerCardNo.Visible = true;
			}
			else
			{
				this.menu_registerCardNo.Enabled = false;
				this.menu_registerCardNo.Visible = false;
			}
		}

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			this.menu_registerCardNo_Click(sender, e);
		}

		private void grd_mainView_RowStyle(object sender, RowStyleEventArgs e)
		{
			GridView gridView = sender as GridView;
			int num;
			int num2;
			if (e.RowHandle >= 0)
			{
				DataRow dataRow = gridView.GetDataRow(e.RowHandle);
				if (dataRow != null && int.TryParse(dataRow[8].ToString(), out num))
				{
					if (num >= 1000 && num < 10000)
					{
						num -= 1000;
					}
					if ((num < 20 || num >= 100) && num != 230 && num != 100000)
					{
						num2 = ((num == 101001) ? 1 : 0);
						goto IL_00a8;
					}
					num2 = 1;
					goto IL_00a8;
				}
			}
			return;
			IL_0108:
			int num3;
			if (num3 != 0)
			{
				e.Appearance.ForeColor = Color.Red;
			}
			else
			{
				e.Appearance.ForeColor = Color.Green;
			}
			return;
			IL_00a8:
			if (num2 != 0)
			{
				e.Appearance.ForeColor = Color.Orange;
				return;
			}
			if ((num < 100 || num >= 200) && num != 28 && num != 100001 && num != 100032 && num != 100034 && num != 100055)
			{
				num3 = ((num == 100058) ? 1 : 0);
				goto IL_0108;
			}
			num3 = 1;
			goto IL_0108;
		}

		private void pic_close_Click(object sender, EventArgs e)
		{
			this.pnl_show.Visible = false;
		}

		private void timer_hide_Tick(object sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			if (this.m_date.AddSeconds(2.0) <= now)
			{
				this.pnl_show.Visible = false;
				this.timer_hide.Enabled = false;
			}
		}

		private void pic_photo_MouseEnter(object sender, EventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void pic_photo_MouseMove(object sender, MouseEventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void pic_photo_Click(object sender, EventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void pnl_main_SizeChanged(object sender, EventArgs e)
		{
			if (this.map_main.Height < this.pnl_main.Height - 2 * this.map_main.Top)
			{
				this.map_main.Height = this.pnl_main.Height - 2 * this.map_main.Top;
			}
			if (this.map_main.Width < this.pnl_main.Width - 2 * this.map_main.Left)
			{
				this.map_main.Width = this.pnl_main.Width - 2 * this.map_main.Left;
			}
		}

		private void time_load_Tick(object sender, EventArgs e)
		{
			this.InitUsers();
			this.time_load.Enabled = false;
		}

		private void map_main_SizeChanged(object sender, EventArgs e)
		{
			if (this.map_main != null && this.map_main.Parent == this.pnl_main)
			{
				if (this.map_main.Height > this.pnl_main.Height - 2 * this.map_main.Top - 5)
				{
					this.map_main.Height = this.pnl_main.Height - 2 * this.map_main.Top - 5;
				}
				if (this.map_main.Width > this.pnl_main.Width - 2 * this.map_main.Left - 5)
				{
					this.map_main.Width = this.pnl_main.Width - 2 * this.map_main.Left - 5;
				}
			}
		}

		private void map_main_ParentChanged(object sender, EventArgs e)
		{
			if (this.map_main.Parent == this.pnl_main)
			{
				if (this.map_main.Height > this.pnl_main.Height - 2 * this.map_main.Top - 5)
				{
					this.map_main.Height = this.pnl_main.Height - 2 * this.map_main.Top - 5;
				}
				if (this.map_main.Width > this.pnl_main.Width - 2 * this.map_main.Left - 5)
				{
					this.map_main.Width = this.pnl_main.Width - 2 * this.map_main.Left - 5;
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
			Application.DoEvents();
			try
			{
				MonitorWatchdog.StopWatchdog();
				MonitorWatchdog.DisConnectAll();
				MonitorWatchdog.ClearMonitor();
			}
			catch (Exception)
			{
			}
			try
			{
				this.m_photoShow.Dispose();
				this.m_photoShow = null;
				this.pic_photo.Image = null;
			}
			catch
			{
			}
			try
			{
				if (this.map_main != null)
				{
					this.map_main.Free();
					this.map_main.Dispose();
				}
			}
			catch
			{
			}
			try
			{
				int num = 0;
				for (num = 0; num < DeviceServers.Instance.Count; num++)
				{
					DeviceServerBll deviceServerBll = DeviceServers.Instance[num];
					if (deviceServerBll != null)
					{
						deviceServerBll.RTLogEvent -= this.devServer_RTLogEvent;
					}
				}
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
				this.m_userList.Clear();
				this.m_userList = null;
				this.m_userListLastName.Clear();
				this.m_userListLastName = null;
				this.mlist.Clear();
				this.mlist = null;
			}
			catch
			{
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MapControl));
			this.pnl_show = new Panel();
			this.pic_close = new PictureBox();
			this.pic_photo = new PictureBox();
			this.menu_Strip = new ContextMenuStrip(this.components);
			this.menu_registerCardNo = new ToolStripMenuItem();
			this.timer_hide = new System.Windows.Forms.Timer(this.components);
			this.time_load = new System.Windows.Forms.Timer(this.components);
			this.panelEx1 = new PanelEx();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_dete = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_doorno = new GridColumn();
			this.column_event = new GridColumn();
			this.column_cardno = new GridColumn();
			this.column_no = new GridColumn();
			this.column_inoutstate = new GridColumn();
			this.column_vilify = new GridColumn();
			this.pnl_main = new Panel();
			this.map_main = new DevMapControl();
			this.expandableSplitter1 = new ExpandableSplitter();
			this.panel1 = new Panel();
			this.pnl_show.SuspendLayout();
			((ISupportInitialize)this.pic_close).BeginInit();
			((ISupportInitialize)this.pic_photo).BeginInit();
			this.menu_Strip.SuspendLayout();
			this.panelEx1.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			this.pnl_main.SuspendLayout();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.pnl_show.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.pnl_show.BorderStyle = BorderStyle.FixedSingle;
			this.pnl_show.Controls.Add(this.pic_close);
			this.pnl_show.Controls.Add(this.pic_photo);
			this.pnl_show.Location = new Point(553, 126);
			this.pnl_show.Name = "pnl_show";
			this.pnl_show.Size = new Size(232, 232);
			this.pnl_show.TabIndex = 16;
			this.pnl_show.Visible = false;
			this.pic_close.Image = (Image)componentResourceManager.GetObject("pic_close.Image");
			this.pic_close.Location = new Point(210, 3);
			this.pic_close.Name = "pic_close";
			this.pic_close.Size = new Size(17, 18);
			this.pic_close.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_close.TabIndex = 14;
			this.pic_close.TabStop = false;
			this.pic_close.Click += this.pic_close_Click;
			this.pic_photo.Dock = DockStyle.Fill;
			this.pic_photo.Location = new Point(0, 0);
			this.pic_photo.Name = "pic_photo";
			this.pic_photo.Size = new Size(230, 230);
			this.pic_photo.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_photo.TabIndex = 13;
			this.pic_photo.TabStop = false;
			this.pic_photo.Visible = false;
			this.pic_photo.Click += this.pic_photo_Click;
			this.pic_photo.MouseEnter += this.pic_photo_MouseEnter;
			this.menu_Strip.Items.AddRange(new ToolStripItem[1]
			{
				this.menu_registerCardNo
			});
			this.menu_Strip.Name = "menu_Strip";
			this.menu_Strip.Size = new Size(114, 26);
			this.menu_registerCardNo.Image = Resources.add;
			this.menu_registerCardNo.Name = "menu_registerCardNo";
			this.menu_registerCardNo.Size = new Size(113, 22);
			this.menu_registerCardNo.Text = "新增卡";
			this.menu_registerCardNo.Click += this.menu_registerCardNo_Click;
			this.timer_hide.Interval = 1000;
			this.timer_hide.Tick += this.timer_hide_Tick;
			this.time_load.Enabled = true;
			this.time_load.Interval = 1000;
			this.time_load.Tick += this.time_load_Tick;
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Controls.Add(this.pnl_show);
			this.panelEx1.Controls.Add(this.grd_view);
			this.panelEx1.Dock = DockStyle.Fill;
			this.panelEx1.Location = new Point(0, 0);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(788, 361);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 17;
			this.grd_view.ContextMenuStrip = this.menu_Strip;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 0);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(788, 361);
			this.grd_view.TabIndex = 9;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Appearance.FocusedRow.BackColor = Color.White;
			this.grd_mainView.Appearance.FocusedRow.BackColor2 = Color.White;
			this.grd_mainView.Appearance.FocusedRow.Options.UseBackColor = true;
			this.grd_mainView.Appearance.HideSelectionRow.BackColor = Color.White;
			this.grd_mainView.Appearance.HideSelectionRow.BackColor2 = Color.White;
			this.grd_mainView.Appearance.HideSelectionRow.Options.UseBackColor = true;
			this.grd_mainView.Appearance.SelectedRow.BackColor = Color.White;
			this.grd_mainView.Appearance.SelectedRow.BackColor2 = Color.White;
			this.grd_mainView.Appearance.SelectedRow.Options.UseBackColor = true;
			this.grd_mainView.Columns.AddRange(new GridColumn[8]
			{
				this.column_dete,
				this.column_dev,
				this.column_doorno,
				this.column_event,
				this.column_cardno,
				this.column_no,
				this.column_inoutstate,
				this.column_vilify
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawRowIndicator += this.grd_mainView_CustomDrawRowIndicator;
			this.grd_mainView.RowStyle += this.grd_mainView_RowStyle;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_dete.Caption = "时间";
			this.column_dete.Name = "column_dete";
			this.column_dete.OptionsFilter.AllowFilter = false;
			this.column_dete.Visible = true;
			this.column_dete.VisibleIndex = 0;
			this.column_dete.Width = 111;
			this.column_dev.Caption = "设备";
			this.column_dev.Name = "column_dev";
			this.column_dev.OptionsFilter.AllowFilter = false;
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 90;
			this.column_doorno.Caption = "事件点";
			this.column_doorno.Name = "column_doorno";
			this.column_doorno.OptionsColumn.ReadOnly = true;
			this.column_doorno.OptionsFilter.AllowFilter = false;
			this.column_doorno.Visible = true;
			this.column_doorno.VisibleIndex = 2;
			this.column_doorno.Width = 90;
			this.column_event.Caption = "事件描述";
			this.column_event.Name = "column_event";
			this.column_event.OptionsFilter.AllowFilter = false;
			this.column_event.Visible = true;
			this.column_event.VisibleIndex = 3;
			this.column_event.Width = 146;
			this.column_cardno.Caption = "卡号";
			this.column_cardno.Name = "column_cardno";
			this.column_cardno.OptionsFilter.AllowFilter = false;
			this.column_cardno.Visible = true;
			this.column_cardno.VisibleIndex = 4;
			this.column_cardno.Width = 76;
			this.column_no.Caption = "人员编号(姓名)";
			this.column_no.Name = "column_no";
			this.column_no.OptionsFilter.AllowFilter = false;
			this.column_no.Visible = true;
			this.column_no.VisibleIndex = 5;
			this.column_no.Width = 87;
			this.column_inoutstate.Caption = "出入状态";
			this.column_inoutstate.Name = "column_inoutstate";
			this.column_inoutstate.OptionsFilter.AllowFilter = false;
			this.column_inoutstate.Visible = true;
			this.column_inoutstate.VisibleIndex = 6;
			this.column_inoutstate.Width = 71;
			this.column_vilify.Caption = "验证方式";
			this.column_vilify.Name = "column_vilify";
			this.column_vilify.OptionsFilter.AllowFilter = false;
			this.column_vilify.Visible = true;
			this.column_vilify.VisibleIndex = 7;
			this.column_vilify.Width = 80;
			this.pnl_main.BackColor = Color.White;
			this.pnl_main.BorderStyle = BorderStyle.FixedSingle;
			this.pnl_main.Controls.Add(this.map_main);
			this.pnl_main.Dock = DockStyle.Top;
			this.pnl_main.Location = new Point(0, 0);
			this.pnl_main.Name = "pnl_main";
			this.pnl_main.Size = new Size(788, 180);
			this.pnl_main.TabIndex = 19;
			this.pnl_main.SizeChanged += this.pnl_main_SizeChanged;
			this.map_main.AutoScroll = true;
			this.map_main.AutoScrollMargin = new Size(5, 5);
			this.map_main.AutoScrollMinSize = new Size(10, 10);
			this.map_main.BackgroundImageLayout = ImageLayout.None;
			this.map_main.BackImg = null;
			this.map_main.FontSize = 9;
			this.map_main.IFont = new Font("SimSun", 9f);
			this.map_main.IsLock = false;
			this.map_main.Location = new Point(6, 6);
			this.map_main.Name = "map_main";
			this.map_main.Size = new Size(688, 88);
			this.map_main.TabIndex = 0;
			this.map_main.SizeChanged += this.map_main_SizeChanged;
			this.map_main.DoubleClick += this.map_main_DoubleClick;
			this.map_main.ParentChanged += this.map_main_ParentChanged;
			this.expandableSplitter1.BackColor2 = Color.FromArgb(101, 147, 207);
			this.expandableSplitter1.BackColor2SchemePart = eColorSchemePart.PanelBorder;
			this.expandableSplitter1.BackColorSchemePart = eColorSchemePart.PanelBackground;
			this.expandableSplitter1.Cursor = Cursors.HSplit;
			this.expandableSplitter1.Dock = DockStyle.Top;
			this.expandableSplitter1.ExpandableControl = this.pnl_main;
			this.expandableSplitter1.ExpandActionClick = false;
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
			this.expandableSplitter1.Location = new Point(0, 180);
			this.expandableSplitter1.MinExtra = 85;
			this.expandableSplitter1.MinSize = 85;
			this.expandableSplitter1.Name = "expandableSplitter1";
			this.expandableSplitter1.Size = new Size(788, 10);
			this.expandableSplitter1.Style = eSplitterStyle.Office2007;
			this.expandableSplitter1.TabIndex = 21;
			this.expandableSplitter1.TabStop = false;
			this.panel1.Controls.Add(this.panelEx1);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 190);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(788, 361);
			this.panel1.TabIndex = 22;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.expandableSplitter1);
			base.Controls.Add(this.pnl_main);
			base.Name = "MapControl";
			base.Size = new Size(788, 551);
			this.pnl_show.ResumeLayout(false);
			((ISupportInitialize)this.pic_close).EndInit();
			((ISupportInitialize)this.pic_photo).EndInit();
			this.menu_Strip.ResumeLayout(false);
			this.panelEx1.ResumeLayout(false);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			this.pnl_main.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
