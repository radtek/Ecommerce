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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.BLL.UDisk;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.MachinesManager;
using ZK.Utils;

namespace ZK.Access
{
	public class DownLoadRecordForm : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		public delegate void SaveSDRecordsHandle(string path);

		private bool DownLoadRecordsOnTime = false;

		private Hashtable DeviceSelectedTable = null;

		private DeviceHelperEx helper = new DeviceHelperEx();

		private Dictionary<string, string> userTable = new Dictionary<string, string>();

		private List<AccDoor> attDoors = new List<AccDoor>();

		private List<AccLinkAgeIo> LinkAgeIOs = new List<AccLinkAgeIo>();

		private Dictionary<int, string> inAddressList = new Dictionary<int, string>();

		private Dictionary<int, string> outAddressList = new Dictionary<int, string>();

		private Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_InAuxAddress;

		private Dictionary<int, Dictionary<int, AccAuxiliary>> Dev_OutAuxAddress;

		private Dictionary<int, Dictionary<int, AccDoor>> dicDev_Door;

		private Thread thread = null;

		private IContainer components = null;

		private ButtonX btn_exit;

		private ButtonX btn_DownLoad;

		public RadioButton rbtn_all;

		public RadioButton rbtn_new;

		private TextBox txt_UpLoadInfo;

		private ProgressBar progressBarUp;

		public TextBox txt_SDCardPath;

		public Label lbl_targetFile;

		public ButtonX btn_borwser;

		private OpenFileDialog odlg_SDCardPath;

		private System.Windows.Forms.Timer tmr_start;

		private Label lbl_progress;

		private ProgressBar progress_all;

		private Label lbl_progressAll;

		private ButtonX btn_show;

		public RadioButton rbtn_bySDCard;

		private Label lblProgressAll;

		private Label lblProgress;

		private CheckBox ckbClearAttLog;

		private bool IsClearAttLog
		{
			get
			{
				bool IsChecked = false;
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						IsChecked = this.ckbClearAttLog.Checked;
					});
				}
				else
				{
					IsChecked = this.ckbClearAttLog.Checked;
				}
				return IsChecked;
			}
		}

		public event EventHandler RefreshDataEvent = null;

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

		private void LoadDoorInfo()
		{
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			this.dicDev_Door = new Dictionary<int, Dictionary<int, AccDoor>>();
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (this.dicDev_Door.ContainsKey(modelList[i].device_id))
					{
						Dictionary<int, AccDoor> dictionary = this.dicDev_Door[modelList[i].device_id];
						if (!dictionary.ContainsKey(modelList[i].door_no))
						{
							dictionary.Add(modelList[i].door_no, modelList[i]);
						}
					}
					else
					{
						Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
						dictionary.Add(modelList[i].door_no, modelList[i]);
						this.dicDev_Door.Add(modelList[i].device_id, dictionary);
					}
				}
			}
		}

		public DownLoadRecordForm()
		{
			this.InitializeComponent();
			this.helper.ShowInfoEvent += this.showUpLoadInfo;
			this.helper.ShowProgressEvent += this.ShowProgress;
			this.tmr_start.Enabled = false;
			try
			{
				this.GetUserInfo();
				this.GetAttDoors();
				this.GetLinkAgeIOInfo();
				this.LoadInAddressInfo();
				this.LoadOutAddressInfo();
				this.LoadAuxAddressInfo();
				this.LoadDoorInfo();
				initLang.LocaleForm(this, base.Name);
				this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
				this.btn_show_Click(null, null);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public DownLoadRecordForm(Hashtable MachineSelectedInfo)
			: this()
		{
			this.DeviceSelectedTable = MachineSelectedInfo;
		}

		public DownLoadRecordForm(bool isDownLoad)
			: this()
		{
			this.DownLoadRecordsOnTime = true;
			this.tmr_start.Enabled = true;
			this.rbtn_bySDCard.Visible = false;
			this.lbl_targetFile.Visible = false;
			this.txt_SDCardPath.Visible = false;
			this.btn_borwser.Visible = false;
		}

		private void tmr_start_Tick(object sender, EventArgs e)
		{
			this.tmr_start.Enabled = false;
			this.btn_DownLoad_Click(null, null);
		}

		private void GetUserInfo()
		{
			try
			{
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				List<UserInfo> modelList = userInfoBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.userTable.ContainsKey(modelList[i].BadgeNumber.ToString()))
						{
							this.userTable.Add(modelList[i].BadgeNumber.ToString(), modelList[i].UserId.ToString());
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

		private void GetAttDoors()
		{
			this.attDoors.Clear();
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (modelList[i].is_att)
					{
						this.attDoors.Add(modelList[i]);
					}
				}
			}
		}

		private void GetLinkAgeIOInfo()
		{
			this.LinkAgeIOs.Clear();
			AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
			List<AccLinkAgeIo> modelList = accLinkAgeIoBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				this.LinkAgeIOs = modelList;
			}
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

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void showUpLoadInfo(string UpLoadinfoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.showUpLoadInfo), UpLoadinfoStr);
				}
				else
				{
					this.txt_UpLoadInfo.AppendText(DateTime.Now.ToString() + " " + UpLoadinfoStr + Environment.NewLine);
					this.Refresh();
				}
			}
		}

		private void ShowProgress(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.ShowProgress), prg);
				}
				else
				{
					if (prg >= 100)
					{
						prg = 100;
					}
					if (prg == 0)
					{
						prg = 1;
					}
					this.progressBarUp.Value = prg;
					this.lblProgress.Text = prg.ToString() + "%";
					this.Refresh();
				}
			}
		}

		private void ShowProgressAll(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.ShowProgressAll), prg);
				}
				else
				{
					if (prg >= 100)
					{
						prg = 100;
					}
					if (prg <= 0)
					{
						prg = 0;
					}
					this.progress_all.Value = prg;
					this.lblProgressAll.Text = prg.ToString() + "%";
					this.Refresh();
				}
			}
		}

		private void ProgressReset()
		{
			this.progressBarUp.Value = 0;
		}

		private string GetDataString(string fileName)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
				{
					FileInfo fileInfo = new FileInfo(fileName);
					if (fileInfo.Length > 50)
					{
						int num = 0;
						FileStream fileStream = new FileStream(fileName, FileMode.Open);
						byte[] array = SysFile.Read(fileStream);
						fileStream.Close();
						num = array.Length;
						byte[] array2 = new byte[num * 4];
						int num2 = DownLoadRecordForm.ProcessBackupData(array, num, ref array2[0], array2.Length);
						if (num2 >= 0)
						{
							array2 = DataConvert.GetDataBuffer(array2);
							return Encoding.Default.GetString(array2);
						}
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFormatError", "数据格式不对") + "," + ShowMsgInfos.GetInfo("SCheckSDCardFile", "请检查该文件是否为SD卡备份的记录文件"));
					}
					else
					{
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFormatError", "数据格式不对") + "," + ShowMsgInfos.GetInfo("SCheckSDCardFile", "请检查该文件是否为SD卡备份的记录文件"));
					}
				}
				else
				{
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SSDFilePath", "请选择SD卡备份的记录文件路径"));
				}
			}
			catch (Exception ex)
			{
				this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ":" + ex.Message);
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ":" + ex.Message + " " + ShowMsgInfos.GetInfo("SCheckSDCardFile", "请检查该文件是否为SD卡备份的记录文件"));
			}
			return string.Empty;
		}

		[DllImport("plcommpro.dll")]
		public static extern int ProcessBackupData(byte[] data, int fileLen, ref byte Buffer, int BufferSize);

		private void ResolveBackupData(string filePath)
		{
			try
			{
				int num = filePath.LastIndexOf('\\');
				int num2 = filePath.LastIndexOf("_attlog.dat");
				List<ObjTransAction> list;
				if (num >= 0 && num2 >= 0)
				{
					string text = filePath.Substring(num + 1, num2 - num - 1);
					if (text != null && text.Trim() != "")
					{
						string strWhere = $"sn='{text}'";
						int num3 = default(int);
						if (text.Length <= 3 && int.TryParse(text, out num3) && num3 <= 255)
						{
							strWhere = $"MachineNumber={num3}";
						}
						MachinesBll machinesBll = new MachinesBll(MainForm._ia);
						List<Machines> modelList = machinesBll.GetModelList(strWhere);
						if (modelList != null && modelList.Count > 0)
						{
							Machines machines = modelList[0];
							UDiskBll uDiskBll = new UDiskBll(SDKType.StandaloneSDK);
							uDiskBll.ImportTransaction(filePath, out list, machines);
							if (list != null && list.Count > 0)
							{
								this.SaveRecords(list, machines);
							}
							this.ShowProgressAll(100);
						}
						else
						{
							this.showUpLoadInfo(ShowMsgInfos.GetInfo("SSDCardDeviceNotExist", "系统当前没有该设备或记录文件有误"));
						}
					}
					else
					{
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SSDCardDeviceNotExist", "系统当前没有该设备或记录文件有误"));
					}
				}
				else
				{
					string columns = "Cardno,Pin,Verified,DoorID,EventType,InOutState,Time_second";
					string dataString = this.GetDataString(filePath);
					if (!string.IsNullOrEmpty(dataString))
					{
						string[] array = dataString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						if (array != null && array.Length > 1)
						{
							string text2 = array[0];
							int num4 = text2.IndexOf(',');
							if (num4 > 0)
							{
								text2 = text2.Substring(0, num4);
								MachinesBll machinesBll = new MachinesBll(MainForm._ia);
								List<Machines> modelList = machinesBll.GetModelList("sn='" + text2 + "'");
								if (modelList != null && modelList.Count > 0)
								{
									Machines machines = modelList[0];
									list = new List<ObjTransAction>();
									for (int i = 1; i < array.Length; i++)
									{
										ObjTransAction objTransAction = new ObjTransAction();
										DataConvert.InitModel(objTransAction, columns, array[i]);
										objTransAction.Time_second = this.DecodeZKTime(uint.Parse(objTransAction.Time_second)).ToString("yyyy-MM-dd HH:mm:ss");
										list.Add(objTransAction);
									}
									this.SaveRecords(list, machines);
									this.ShowProgressAll(100);
								}
								else
								{
									this.showUpLoadInfo(ShowMsgInfos.GetInfo("SSDCardDeviceNotExist", "系统当前没有该设备或记录文件有误") + "," + ShowMsgInfos.GetInfo("SCheckSDCardFile", "请检查该文件是否为SD卡备份的记录文件"));
								}
							}
							else
							{
								this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFormatError", "数据格式不对"));
							}
						}
						else
						{
							this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFormatError", "数据格式不对"));
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ":" + ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void SaveRecords(List<ObjTransAction> devTransActionList, Machines dev)
		{
			bool flag = false;
			AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
			CheckInOutBll checkInOutBll = new CheckInOutBll(MainForm._ia);
			if (devTransActionList != null && devTransActionList.Count > 0 && dev != null)
			{
				List<AccMonitorLog> list = new List<AccMonitorLog>();
				List<CheckInOut> list2 = new List<CheckInOut>();
				for (int i = 0; i < devTransActionList.Count; i++)
				{
					flag = false;
					if (i + 1 == devTransActionList.Count)
					{
						this.ShowProgress(99);
					}
					else if ((i + 1) % 500 == 0)
					{
						this.ShowProgress((i + 1) * 100 / devTransActionList.Count);
					}
					try
					{
						int num = (int)devTransActionList[i].EventType;
						if (dev.simpleEventType == 1 && PullSDKEventInfos.GetDic().ContainsKey(num + 1000))
						{
							num += 1000;
						}
						AccMonitorLog accMonitorLog = new AccMonitorLog();
						accMonitorLog.device_id = dev.ID;
						accMonitorLog.device_name = dev.MachineAlias;
						accMonitorLog.device_sn = dev.sn;
						accMonitorLog.card_no = devTransActionList[i].Cardno;
						accMonitorLog.pin = devTransActionList[i].Pin;
						accMonitorLog.verified = (int)devTransActionList[i].Verified;
						accMonitorLog.event_type = num;
						if (!string.IsNullOrEmpty(devTransActionList[i].InOutState))
						{
							accMonitorLog.state = (int.Parse(devTransActionList[i].InOutState) & 0xF);
						}
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						int userID = userInfoBll.GetUserID(devTransActionList[i].Pin);
						UserInfo model = userInfoBll.GetModel(userID);
						if (model != null)
						{
							accMonitorLog.username = model.Name;
							accMonitorLog.userlastname = model.LastName;
						}
						accMonitorLog.time = DateTime.Parse(devTransActionList[i].Time_second);
						if (accMonitorLog.event_type == 220 || accMonitorLog.event_type == 221)
						{
							accMonitorLog.event_point_type = 1;
							accMonitorLog.event_point_id = int.Parse(devTransActionList[i].DoorID);
						}
						else if (accMonitorLog.event_type == 12 || accMonitorLog.event_type == 13)
						{
							accMonitorLog.event_point_type = 2;
							accMonitorLog.event_point_id = int.Parse(devTransActionList[i].DoorID);
						}
						else if (accMonitorLog.event_type == 6)
						{
							string text = PullSDKEventInfos.GetInfo(devTransActionList[i].EventType);
							string cardno = devTransActionList[i].Cardno;
							if (!string.IsNullOrEmpty(cardno))
							{
								int num2 = int.Parse(cardno);
								int num3 = 0;
								while (num3 < this.LinkAgeIOs.Count)
								{
									if (this.LinkAgeIOs[num3].id != num2)
									{
										num3++;
										continue;
									}
									text = text + "[" + PullSDKEventInfos.GetInfo((EventType)devTransActionList[i].Verified) + "]";
									text = text + "(" + this.LinkAgeIOs[num3].linkage_name + ")";
									break;
								}
							}
							accMonitorLog.description = text;
							accMonitorLog.event_point_type = 0;
							accMonitorLog.event_point_id = int.Parse(devTransActionList[i].DoorID);
							if (accMonitorLog.verified == 220 || accMonitorLog.verified == 221)
							{
								accMonitorLog.event_point_type = 1;
								accMonitorLog.event_point_id = int.Parse(devTransActionList[i].DoorID);
							}
							accMonitorLog.verified = 200;
							accMonitorLog.card_no = string.Empty;
						}
						else if (int.Parse(devTransActionList[i].DoorID) > 0)
						{
							accMonitorLog.event_point_type = 0;
							accMonitorLog.event_point_id = int.Parse(devTransActionList[i].DoorID);
						}
						accMonitorLog.event_point_name = accMonitorLog.device_name + "-" + accMonitorLog.event_point_id;
						switch (accMonitorLog.event_point_type)
						{
						case 1:
							if (dev != null && this.Dev_InAuxAddress.ContainsKey(dev.ID))
							{
								Dictionary<int, AccAuxiliary> dictionary2 = this.Dev_InAuxAddress[dev.ID];
								if (dictionary2.ContainsKey(accMonitorLog.event_point_id))
								{
									accMonitorLog.event_point_name = dictionary2[accMonitorLog.event_point_id].AuxName;
								}
							}
							break;
						case 2:
							if (dev != null && this.Dev_OutAuxAddress.ContainsKey(dev.ID))
							{
								Dictionary<int, AccAuxiliary> dictionary2 = this.Dev_OutAuxAddress[dev.ID];
								if (dictionary2.ContainsKey(accMonitorLog.event_point_id))
								{
									accMonitorLog.event_point_name = dictionary2[accMonitorLog.event_point_id].AuxName;
								}
							}
							break;
						default:
							if (dev != null && this.dicDev_Door.ContainsKey(dev.ID))
							{
								Dictionary<int, AccDoor> dictionary = this.dicDev_Door[dev.ID];
								if (dictionary.ContainsKey(accMonitorLog.event_point_id))
								{
									accMonitorLog.event_point_name = dictionary[accMonitorLog.event_point_id].door_name;
								}
							}
							break;
						}
						list.Add(accMonitorLog);
						int num4 = 0;
						while (num4 < this.attDoors.Count)
						{
							if (this.attDoors[num4].device_id != dev.ID || this.attDoors[num4].door_no != int.Parse(devTransActionList[i].DoorID))
							{
								num4++;
								continue;
							}
							flag = true;
							break;
						}
						CheckInOut checkInOut;
						int num6;
						if (flag)
						{
							checkInOut = new CheckInOut();
							if (!string.IsNullOrEmpty(accMonitorLog.pin) && this.userTable.ContainsKey(accMonitorLog.pin))
							{
								int.TryParse(accMonitorLog.pin, out int pin);
								checkInOut.Pin = pin;
								checkInOut.USERID = int.Parse(this.userTable[accMonitorLog.pin]);
							}
							else
							{
								checkInOut.Pin = 0;
								checkInOut.USERID = 0;
							}
							checkInOut.VERIFYCODE = accMonitorLog.verified;
							checkInOut.CHECKTIME = accMonitorLog.time;
							checkInOut.SENSORID = dev.MachineNumber.ToString();
							checkInOut.CHECKTYPE = "I";
							byte b = (byte)((int.Parse(devTransActionList[i].InOutState) & 0xF0) >> 4);
							if (dev.DevSDKType != SDKType.StandaloneSDK)
							{
								b = (byte)(b - 1);
							}
							string[] array = new string[6]
							{
								"I",
								"O",
								"0",
								"1",
								"i",
								"o"
							};
							if (b >= 0 && b < 6)
							{
								checkInOut.CHECKTYPE = array[b];
							}
							int num5 = accMonitorLog.event_type;
							if (num5 >= 1000)
							{
								num5 -= 1000;
							}
							if ((num5 < 0 || num5 > 3) && (num5 < 14 || num5 > 19) && (num5 < 21 || num5 > 23) && num5 != 26 && num5 != 32 && num5 != 35 && num5 != 203 && num5 != 207 && num5 != 208)
							{
								num6 = ((num5 == 214) ? 1 : 0);
								goto IL_0825;
							}
							num6 = 1;
							goto IL_0825;
						}
						goto end_IL_0099;
						IL_0825:
						if (num6 != 0 && checkInOut.USERID > 0)
						{
							list2.Add(checkInOut);
						}
						end_IL_0099:;
					}
					catch (Exception ex)
					{
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFormatError", "数据格式不对") + " index=" + i + " :" + ex.Message);
					}
				}
				this.showUpLoadInfo(ShowMsgInfos.GetInfo("SSavingData", "正在保存数据，请稍候...") + "(" + dev.MachineAlias + ")");
				try
				{
					int num7 = accMonitorLogBll.Add(list);
					checkInOutBll.Add(list2);
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SDownloadLogCount", "从设备获取记录条数:") + list.Count + "(" + dev.MachineAlias + ")");
					this.ShowProgress(100);
				}
				catch (Exception ex2)
				{
					this.showUpLoadInfo(ex2.Message);
				}
			}
		}

		private void SaveSDRecords(string path)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new SaveSDRecordsHandle(this.SaveSDRecords), path);
				}
				else if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SResolveSDFile", "正在解析"));
					this.ResolveBackupData(path);
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SResolveSuccess", "解析完成"));
				}
				else
				{
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SSDFilePath", "请选择SD卡备份的记录文件路径"));
				}
			}
		}

		private void SaveDevRecords(Machines dev)
		{
			if (dev != null)
			{
				DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(dev);
				if (deviceServer != null)
				{
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + dev.MachineAlias + ")");
					int num = deviceServer.Connect(3000);
					if (num < 0 && dev.ConnectType == 0)
					{
						deviceServer.Disconnect();
						Program.KillPlrscagent();
						num = deviceServer.Connect(3000);
					}
					if (num >= 0)
					{
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功") + "(" + dev.MachineAlias + ")");
						TransActionBll transActionBll = new TransActionBll(deviceServer.Application);
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SDownLoadingLogs", "正在从设备获取记录，请稍后...") + "(" + dev.MachineAlias + ")");
						List<ObjTransAction> list = null;
						if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
						{
							deviceServer.STD_GetAllTransaction(out list);
						}
						else
						{
							if (this.rbtn_new.Checked)
							{
								list = transActionBll.GetList("*", "NewRecord", ref num);
							}
							else if (this.rbtn_all.Checked)
							{
								num = 0;
								list = transActionBll.GetList(ref num);
							}
							if (list != null)
							{
								for (int i = 0; i < list.Count; i++)
								{
									list[i].Time_second = this.DecodeZKTime(uint.Parse(list[i].Time_second)).ToString("yyyy-MM-dd HH:mm:ss");
								}
							}
						}
						if (list != null && list.Count > 0)
						{
							this.SaveRecords(list, dev);
							if (this.IsClearAttLog)
							{
								num = deviceServer.STD_ClearGLog();
								if (num >= 0)
								{
									this.showUpLoadInfo(ShowMsgInfos.GetInfo("ClearAttLogSucceed", "从设备清除事件记录完成") + "(" + dev.MachineAlias + ")");
								}
								else
								{
									this.showUpLoadInfo(ShowMsgInfos.GetInfo("ClearAttLogFailed", "从设备清除事件记录失败") + "(" + dev.MachineAlias + "):" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							this.showUpLoadInfo(ShowMsgInfos.GetInfo("SDownLoadLogsSuccess", "从设备获取记录完成") + "(" + dev.MachineAlias + ")");
						}
						else
						{
							this.showUpLoadInfo(ShowMsgInfos.GetInfo("SDownloadLogCount", "从设备获取记录条数:") + "0(" + dev.MachineAlias + ")");
						}
						this.ShowProgress(100);
					}
					else
					{
						this.showUpLoadInfo(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + "(" + dev.MachineAlias + "):" + PullSDkErrorInfos.GetInfo(num));
					}
					deviceServer.Disconnect();
				}
			}
		}

		private void btn_DownLoad_Click(object sender, EventArgs e)
		{
			try
			{
				this.ShowProgress(0);
				this.ShowProgressAll(0);
				this.Cursor = Cursors.WaitCursor;
				this.btn_DownLoad.Enabled = false;
				this.btn_exit.Enabled = false;
				this.thread = new Thread(this.StartDownLog);
				this.thread.Start();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
				this.btn_DownLoad.Enabled = true;
				this.thread = null;
			}
		}

		private void StartDownLog()
		{
			int num = 0;
			try
			{
				if (this.rbtn_bySDCard.Checked)
				{
					this.SaveSDRecords(this.txt_SDCardPath.Text);
				}
				else
				{
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					List<Machines> modelList = machinesBll.GetModelList("");
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							if (this.DownLoadRecordsOnTime)
							{
								this.ShowProgressAll(i * 100 / modelList.Count);
								this.SaveDevRecords(modelList[i]);
							}
							else if (this.DeviceSelectedTable != null && this.DeviceSelectedTable.ContainsKey(modelList[i].ID.ToString()))
							{
								num++;
								this.SaveDevRecords(modelList[i]);
								this.ShowProgressAll(num * 100 / this.DeviceSelectedTable.Count);
							}
							Application.DoEvents();
						}
						this.ShowProgressAll(100);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.OnFinish(this, null);
			this.thread = null;
		}

		private void OnFinish(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new EventHandler(this.OnFinish), sender, e);
				}
				else
				{
					this.btn_DownLoad.Enabled = true;
					this.btn_exit.Enabled = true;
					this.Cursor = Cursors.Default;
					this.showUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFinish", "操作完成"));
					if (this.DownLoadRecordsOnTime)
					{
						System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
						timer.Interval = 5000;
						timer.Tick += this.timer_Tick;
						timer.Enabled = true;
					}
				}
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				base.Close();
			}
		}

		private DateTime DecodeZKTime(uint t)
		{
			uint second = t % 60u;
			t /= 60u;
			uint minute = t % 60u;
			t /= 60u;
			uint hour = t % 24u;
			t /= 24u;
			uint day = t % 31u + 1;
			t /= 31u;
			uint month = t % 12u + 1;
			t /= 12u;
			uint year = t + 2000;
			DateTime result;
			try
			{
				return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
			}
			catch (Exception)
			{
				result = new DateTime(1970, 1, 1, 1, 1, 1);
			}
			return result;
		}

		private int EncodeZKTime(DateTime t)
		{
			return t.Year % 100 * 12 * 31 + (t.Month - 1) * 31 + (t.Day - 1) * 24 * 60 * 60 + (t.Hour * 60 + t.Minute) * 60 + t.Second;
		}

		public void rbtn_bySDCard_Click(object sender, EventArgs e)
		{
			this.lbl_targetFile.Enabled = true;
			this.txt_SDCardPath.Enabled = true;
			this.btn_borwser.Enabled = true;
			this.rbtn_new.Visible = false;
			this.rbtn_all.Visible = false;
			this.ckbClearAttLog.Checked = false;
			this.ckbClearAttLog.Visible = false;
		}

		private void btn_borwser_Click(object sender, EventArgs e)
		{
			if (this.odlg_SDCardPath.ShowDialog() == DialogResult.OK)
			{
				this.txt_SDCardPath.Text = this.odlg_SDCardPath.FileName;
			}
		}

		private void rbtn_all_Click(object sender, EventArgs e)
		{
			this.lbl_targetFile.Enabled = false;
			this.txt_SDCardPath.Enabled = false;
			this.btn_borwser.Enabled = false;
			this.rbtn_new.Visible = true;
			this.rbtn_all.Visible = true;
		}

		private void rbtn_new_Click(object sender, EventArgs e)
		{
			this.lbl_targetFile.Enabled = false;
			this.txt_SDCardPath.Enabled = false;
			this.btn_borwser.Enabled = false;
			this.rbtn_new.Visible = true;
			this.rbtn_all.Visible = true;
		}

		private void DownLoadRecordForm_Load(object sender, EventArgs e)
		{
			if (this.DownLoadRecordsOnTime)
			{
				this.rbtn_new.Checked = true;
				this.tmr_start.Interval = 3000;
				this.tmr_start.Enabled = true;
			}
		}

		private void btn_show_Click(object sender, EventArgs e)
		{
			if (this.btn_show.Text == ShowMsgInfos.GetInfo("SShowInfo", "详细信息"))
			{
				this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
				this.txt_UpLoadInfo.Visible = true;
				base.Height = this.txt_UpLoadInfo.Top + this.txt_UpLoadInfo.Height + 38;
			}
			else
			{
				this.btn_show.Text = ShowMsgInfos.GetInfo("SShowInfo", "详细信息");
				this.txt_UpLoadInfo.Visible = false;
				base.Height = this.txt_UpLoadInfo.Top + 33;
			}
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
				this.txt_UpLoadInfo.Visible = true;
				base.Height = this.txt_UpLoadInfo.Top + this.txt_UpLoadInfo.Height + 38;
				this.btn_show.Visible = false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DownLoadRecordForm));
			this.btn_exit = new ButtonX();
			this.btn_DownLoad = new ButtonX();
			this.rbtn_all = new RadioButton();
			this.rbtn_new = new RadioButton();
			this.txt_UpLoadInfo = new TextBox();
			this.progressBarUp = new ProgressBar();
			this.txt_SDCardPath = new TextBox();
			this.lbl_targetFile = new Label();
			this.btn_borwser = new ButtonX();
			this.odlg_SDCardPath = new OpenFileDialog();
			this.tmr_start = new System.Windows.Forms.Timer(this.components);
			this.lbl_progress = new Label();
			this.progress_all = new ProgressBar();
			this.lbl_progressAll = new Label();
			this.btn_show = new ButtonX();
			this.rbtn_bySDCard = new RadioButton();
			this.lblProgressAll = new Label();
			this.lblProgress = new Label();
			this.ckbClearAttLog = new CheckBox();
			base.SuspendLayout();
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(473, 168);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(142, 23);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 6;
			this.btn_exit.Text = "返回";
			this.btn_exit.Click += this.btn_Cancel_Click;
			this.btn_DownLoad.AccessibleRole = AccessibleRole.PushButton;
			this.btn_DownLoad.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_DownLoad.Location = new Point(310, 168);
			this.btn_DownLoad.Name = "btn_DownLoad";
			this.btn_DownLoad.Size = new Size(142, 23);
			this.btn_DownLoad.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_DownLoad.TabIndex = 5;
			this.btn_DownLoad.Text = "获取";
			this.btn_DownLoad.Click += this.btn_DownLoad_Click;
			this.rbtn_all.AutoSize = true;
			this.rbtn_all.Location = new Point(211, 26);
			this.rbtn_all.Name = "rbtn_all";
			this.rbtn_all.Size = new Size(95, 16);
			this.rbtn_all.TabIndex = 1;
			this.rbtn_all.Text = "获取所有记录";
			this.rbtn_all.UseVisualStyleBackColor = true;
			this.rbtn_all.Click += this.rbtn_all_Click;
			this.rbtn_new.AutoSize = true;
			this.rbtn_new.Checked = true;
			this.rbtn_new.Location = new Point(16, 26);
			this.rbtn_new.Name = "rbtn_new";
			this.rbtn_new.Size = new Size(83, 16);
			this.rbtn_new.TabIndex = 0;
			this.rbtn_new.TabStop = true;
			this.rbtn_new.Text = "获取新记录";
			this.rbtn_new.UseVisualStyleBackColor = true;
			this.rbtn_new.Click += this.rbtn_new_Click;
			this.txt_UpLoadInfo.Location = new Point(12, 201);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(603, 150);
			this.txt_UpLoadInfo.TabIndex = 10;
			this.progressBarUp.Location = new Point(12, 91);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(603, 23);
			this.progressBarUp.TabIndex = 23;
			this.txt_SDCardPath.Location = new Point(138, 44);
			this.txt_SDCardPath.Name = "txt_SDCardPath";
			this.txt_SDCardPath.Size = new Size(384, 21);
			this.txt_SDCardPath.TabIndex = 3;
			this.lbl_targetFile.Location = new Point(10, 47);
			this.lbl_targetFile.Name = "lbl_targetFile";
			this.lbl_targetFile.Size = new Size(122, 12);
			this.lbl_targetFile.TabIndex = 26;
			this.lbl_targetFile.Text = "目标文件";
			this.btn_borwser.AccessibleRole = AccessibleRole.PushButton;
			this.btn_borwser.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_borwser.Location = new Point(528, 42);
			this.btn_borwser.Name = "btn_borwser";
			this.btn_borwser.Size = new Size(87, 23);
			this.btn_borwser.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_borwser.TabIndex = 4;
			this.btn_borwser.Text = "浏览";
			this.btn_borwser.Click += this.btn_borwser_Click;
			this.odlg_SDCardPath.Filter = ".dat File(*.dat)|*.dat";
			this.tmr_start.Tick += this.tmr_start_Tick;
			this.lbl_progress.Location = new Point(12, 76);
			this.lbl_progress.Name = "lbl_progress";
			this.lbl_progress.Size = new Size(267, 12);
			this.lbl_progress.TabIndex = 29;
			this.lbl_progress.Text = "当前设备进度";
			this.progress_all.Location = new Point(12, 136);
			this.progress_all.Name = "progress_all";
			this.progress_all.Size = new Size(603, 23);
			this.progress_all.TabIndex = 30;
			this.lbl_progressAll.Location = new Point(12, 121);
			this.lbl_progressAll.Name = "lbl_progressAll";
			this.lbl_progressAll.Size = new Size(267, 12);
			this.lbl_progressAll.TabIndex = 31;
			this.lbl_progressAll.Text = "总体进度";
			this.btn_show.AccessibleRole = AccessibleRole.PushButton;
			this.btn_show.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_show.Location = new Point(12, 168);
			this.btn_show.Name = "btn_show";
			this.btn_show.Size = new Size(230, 23);
			this.btn_show.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_show.TabIndex = 34;
			this.btn_show.Text = "隐藏信息";
			this.btn_show.Click += this.btn_show_Click;
			this.rbtn_bySDCard.AutoSize = true;
			this.rbtn_bySDCard.Location = new Point(16, 15);
			this.rbtn_bySDCard.Name = "rbtn_bySDCard";
			this.rbtn_bySDCard.Size = new Size(119, 16);
			this.rbtn_bySDCard.TabIndex = 36;
			this.rbtn_bySDCard.Text = "通过SD卡获取记录";
			this.rbtn_bySDCard.UseVisualStyleBackColor = true;
			this.lblProgressAll.AutoSize = true;
			this.lblProgressAll.BackColor = Color.Transparent;
			this.lblProgressAll.Location = new Point(302, 142);
			this.lblProgressAll.Name = "lblProgressAll";
			this.lblProgressAll.Size = new Size(17, 12);
			this.lblProgressAll.TabIndex = 39;
			this.lblProgressAll.Text = "0%";
			this.lblProgress.AutoSize = true;
			this.lblProgress.BackColor = Color.Transparent;
			this.lblProgress.Location = new Point(302, 96);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new Size(17, 12);
			this.lblProgress.TabIndex = 38;
			this.lblProgress.Text = "0%";
			this.ckbClearAttLog.AutoSize = true;
			this.ckbClearAttLog.Location = new Point(383, 26);
			this.ckbClearAttLog.Name = "ckbClearAttLog";
			this.ckbClearAttLog.Size = new Size(132, 16);
			this.ckbClearAttLog.TabIndex = 40;
			this.ckbClearAttLog.Text = "下载完成后清除记录";
			this.ckbClearAttLog.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(627, 364);
			base.Controls.Add(this.ckbClearAttLog);
			base.Controls.Add(this.txt_SDCardPath);
			base.Controls.Add(this.lblProgressAll);
			base.Controls.Add(this.lblProgress);
			base.Controls.Add(this.btn_show);
			base.Controls.Add(this.lbl_progressAll);
			base.Controls.Add(this.progress_all);
			base.Controls.Add(this.lbl_progress);
			base.Controls.Add(this.btn_borwser);
			base.Controls.Add(this.rbtn_all);
			base.Controls.Add(this.lbl_targetFile);
			base.Controls.Add(this.rbtn_new);
			base.Controls.Add(this.txt_UpLoadInfo);
			base.Controls.Add(this.progressBarUp);
			base.Controls.Add(this.btn_exit);
			base.Controls.Add(this.btn_DownLoad);
			base.Controls.Add(this.rbtn_bySDCard);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DownLoadRecordForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "获取事件记录";
			base.Load += this.DownLoadRecordForm_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
