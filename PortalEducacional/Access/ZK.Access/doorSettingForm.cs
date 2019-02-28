/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.Access.door;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class doorSettingForm : Office2007Form
	{
		private int m_id = 0;

		private bool force;

		private bool urgent;

		private string forceValue;

		private string urgentValue;

		private string forceSetValue;

		private string urgentSetValue;

		private int setValue = 0;

		private int setUrgentValue = 0;

		private Machines machine;

		private List<AccDoor> allDevice = new List<AccDoor>();

		private AccDoor doorTemp = null;

		private AccDoor wiegand_door = null;

		private Dictionary<int, string> verifyTypeList = new Dictionary<int, string>();

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private Dictionary<int, int> timeindexidlist = new Dictionary<int, int>();

		private IContainer components = null;

		private Label lab_deviceName;

		private Label lab_doorNO;

		private Label lab_doorName;

		private Label lab_doorActive;

		private Label lab_normalOpen;

		private Label lab_lockDuration;

		private Label lab_punchInterval;

		private Label lab_doorSensor;

		private Label lab_verifyMode;

		private Label lab_duress;

		private Label lab_emergencyPassword;

		private ComboBoxEx cmb_doorActive;

		private ComboBoxEx cmb_normalOpen;

		private ComboBoxEx cmb_doorSensor;

		private ComboBoxEx cmb_verifyMode;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label6;

		private Label label7;

		private Label label8;

		private TextBox txt_name;

		private CheckBox ch_closeAndLock;

		private Label lbl_doorStatus;

		private TextBoxX txt_devName;

		private TextBoxX txt_doorNO;

		private LinkLabel btn_seetingPwd;

		private LinkLabel lbl_urgentPwd;

		private Panel panel1;

		private CheckBox chk_currentAccess;

		private CheckBox chk_allAccess;

		private ComboBoxEx cmb_slave;

		private Label lab_slave;

		private ComboBoxEx cmb_master;

		private Label lab_master;

		private ComboBoxEx cmb_reader1;

		private Label lab_reader1;

		private ComboBoxEx cmb_reader2;

		private Label lab_reader2;

		private CheckBox chk_isAtt;

		private LinkLabel btn_wiegand_set;

		private LinkLabel linkLabel_wiegand_fmt;

		private Label lbl_wigand;

		private Panel gop_Reader1IOState;

		private NumericUpDown numErrTimes;

		private Label label9;

		private Label label10;

		private NumericUpDown numSenserAlarmTime;

		private NumericUpDown numLockDriveTime;

		private NumericUpDown numPunchInterval;

		private NumericUpDown numSensorDelayTime;

		private CheckBox ckbSRBOn;

		private Panel pnlSensorType;

		private Panel pnlStdParam;

		private Label lblUnitLockDrivingTime;

		private Label lblUnitSensorDelay;

		private Label lblSensorAlarmTime;

		private Label lblPunchInterval;

		private FlowLayoutPanel flowLayoutPanel1;

		private CheckBox ckbTwoPunchCardClose;

		private CheckBox chk_cardbox;

		public event EventHandler refreshDataEvent = null;

		public doorSettingForm(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.machine = this.GetMachine(this.m_id);
				this.LoadTimeSeg();
				this.InitSensorType();
				this.InitVerifyType();
				this.InitMasterSlaveIOStatee();
				this.InitReaderIOStatee();
				initLang.LocaleForm(this, base.Name);
				this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
				this.BindData();
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF && this.machine != null && this.machine.DevSDKType != SDKType.StandaloneSDK)
				{
					this.ckbTwoPunchCardClose.Visible = true;
				}
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataError", "数据加载失败!"));
			}
		}

		private Machines GetMachine(int DoorId)
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList("ID in (Select device_id from acc_door where id = " + DoorId + ")");
			if (modelList != null && modelList.Count > 0)
			{
				return modelList[0];
			}
			return null;
		}

		private void InitVerifyType()
		{
			try
			{
				this.cmb_verifyMode.Items.Clear();
				this.verifyTypeList = PullSDKVerifyTypeInfos.GetDic();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DoorType()
		{
			this.m_typeDic = initLang.GetComboxInfo("doorType");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("0", "无");
				dictionary.Add("1", "常开");
				dictionary.Add("2", "常闭");
				this.m_typeDic.Add("0", dictionary);
				initLang.SetComboxInfo("doorType", this.m_typeDic);
				initLang.Save();
			}
		}

		private void InitSensorType()
		{
			this.cmb_doorSensor.Items.Clear();
			this.m_typeDic = initLang.GetComboxInfo("doorType");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("0", "无");
				dictionary.Add("1", "常开");
				dictionary.Add("2", "常闭");
				this.m_typeDic.Add("0", dictionary);
				initLang.SetComboxInfo("doorType", this.m_typeDic);
				initLang.Save();
			}
			if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary2 = this.m_typeDic["0"];
				foreach (KeyValuePair<string, string> item in dictionary2)
				{
					this.cmb_doorSensor.Items.Add(item.Value);
				}
			}
		}

		private void InitMasterSlaveIOStatee()
		{
			this.cmb_master.Items.Clear();
			this.m_typeDic = initLang.GetComboxInfo("MasterIOState");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("0", "入");
				dictionary.Add("1", "出");
				this.m_typeDic.Add("0", dictionary);
				initLang.SetComboxInfo("MasterIOState", this.m_typeDic);
				initLang.Save();
			}
			if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary2 = this.m_typeDic["0"];
				foreach (KeyValuePair<string, string> item in dictionary2)
				{
					this.cmb_master.Items.Add(item.Value);
				}
			}
			this.cmb_slave.Items.Clear();
			this.m_typeDic = initLang.GetComboxInfo("SlaveIOState");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				dictionary3.Add("0", "入");
				dictionary3.Add("1", "出");
				this.m_typeDic.Add("0", dictionary3);
				initLang.SetComboxInfo("SlaveIOState", this.m_typeDic);
				initLang.Save();
			}
			if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary4 = this.m_typeDic["0"];
				foreach (KeyValuePair<string, string> item2 in dictionary4)
				{
					this.cmb_slave.Items.Add(item2.Value);
				}
			}
		}

		private void InitReaderIOStatee()
		{
			this.cmb_reader1.Items.Clear();
			this.m_typeDic = initLang.GetComboxInfo("Reader1IOState");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("0", "出");
				dictionary.Add("1", "入");
				this.m_typeDic.Add("0", dictionary);
				initLang.SetComboxInfo("Reader1IOState", this.m_typeDic);
				initLang.Save();
			}
			if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary2 = this.m_typeDic["0"];
				foreach (KeyValuePair<string, string> item in dictionary2)
				{
					this.cmb_reader1.Items.Add(item.Value);
				}
			}
			this.cmb_reader2.Items.Clear();
			this.m_typeDic = initLang.GetComboxInfo("Reader2IOState");
			if (this.m_typeDic == null || this.m_typeDic.Count == 0)
			{
				this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				dictionary3.Add("0", "出");
				dictionary3.Add("1", "入");
				this.m_typeDic.Add("0", dictionary3);
				initLang.SetComboxInfo("Reader2IOState", this.m_typeDic);
				initLang.Save();
			}
			if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary4 = this.m_typeDic["0"];
				foreach (KeyValuePair<string, string> item2 in dictionary4)
				{
					this.cmb_reader2.Items.Add(item2.Value);
				}
			}
		}

		private void LoadTimeSeg()
		{
			try
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("TzId", typeof(int));
				dataTable.Columns.Add("TzName", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["TzId"] = 0;
				dataRow["TzName"] = "-----";
				dataTable.Rows.Add(dataRow);
				DataTable dataTable2 = dataTable.Copy();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				modelList = (modelList ?? new List<AccTimeseg>());
				for (int i = 0; i < modelList.Count; i++)
				{
					AccTimeseg accTimeseg = modelList[i];
					if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
					{
						if (accTimeseg.TimeZone1Id > 0 || accTimeseg.TimeZone2Id > 0 || accTimeseg.TimeZone3Id > 0)
						{
							dataRow = dataTable.NewRow();
							dataRow["TzId"] = accTimeseg.id;
							dataRow["TzName"] = accTimeseg.timeseg_name;
							dataTable.Rows.Add(dataRow);
						}
						if (accTimeseg.TimeZone1Id > 0)
						{
							dataRow = dataTable2.NewRow();
							dataRow["TzId"] = accTimeseg.TimeZone1Id;
							dataRow["TzName"] = accTimeseg.TimeZone1Id;
							dataTable2.Rows.Add(dataRow);
						}
						if (accTimeseg.TimeZone2Id > 0)
						{
							dataRow = dataTable2.NewRow();
							dataRow["TzId"] = accTimeseg.TimeZone2Id;
							dataRow["TzName"] = accTimeseg.TimeZone2Id;
							dataTable2.Rows.Add(dataRow);
						}
						if (accTimeseg.TimeZone3Id > 0)
						{
							dataRow = dataTable2.NewRow();
							dataRow["TzId"] = accTimeseg.TimeZone3Id;
							dataRow["TzName"] = accTimeseg.TimeZone3Id;
							dataTable2.Rows.Add(dataRow);
						}
						if (accTimeseg.TimeZoneHolidayId > 0)
						{
							dataRow = dataTable2.NewRow();
							dataRow["TzId"] = accTimeseg.TimeZoneHolidayId;
							dataRow["TzName"] = accTimeseg.TimeZoneHolidayId;
							dataTable2.Rows.Add(dataRow);
						}
					}
					else
					{
						dataRow = dataTable.NewRow();
						dataRow["TzId"] = accTimeseg.id;
						dataRow["TzName"] = accTimeseg.timeseg_name;
						dataTable.Rows.Add(dataRow);
						dataTable2.Rows.Add(dataRow.ItemArray);
					}
				}
				this.cmb_doorActive.DataSource = dataTable;
				this.cmb_doorActive.ValueMember = "TzId";
				this.cmb_doorActive.DisplayMember = "TzName";
				this.cmb_doorActive.SelectedValue = 0;
				this.cmb_normalOpen.DataSource = dataTable2;
				this.cmb_normalOpen.ValueMember = "TzId";
				this.cmb_normalOpen.DisplayMember = "TzName";
				this.cmb_normalOpen.SelectedValue = 0;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					AccDoor accDoor = null;
					AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
					accDoor = accDoorBll.GetModel(this.m_id);
					if (accDoor != null)
					{
						this.doorTemp = accDoor.Copy();
						if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
						{
							this.ckbTwoPunchCardClose.Checked = (accDoor.ManualCtlMode == 1);
						}
						MachinesBll machinesBll = new MachinesBll(MainForm._ia);
						Machines model = machinesBll.GetModel(accDoor.device_id);
						if (model != null)
						{
							this.txt_devName.Text = model.MachineAlias;
							this.pnlStdParam.Visible = (model.DevSDKType == SDKType.StandaloneSDK);
							if (model.DevSDKType == SDKType.StandaloneSDK)
							{
								this.numPunchInterval.Enabled = false;
								this.numLockDriveTime.Minimum = decimal.One;
								this.numLockDriveTime.Maximum = 10m;
								this.lblUnitLockDrivingTime.Text = ShowMsgInfos.GetInfo("Second", "秒") + $"({this.numLockDriveTime.Minimum}-{this.numLockDriveTime.Maximum})";
								this.machine = model;
								this.cmb_normalOpen.Enabled = model.IsTFTMachine;
								this.btn_seetingPwd.Enabled = false;
								this.lbl_urgentPwd.Enabled = false;
								this.gop_Reader1IOState.Visible = false;
								this.lab_slave.Visible = false;
								this.lab_reader2.Visible = false;
								this.cmb_reader2.Visible = false;
								this.cmb_slave.Visible = false;
								this.lab_doorActive.Text = ShowMsgInfos.GetInfo("DefaultTimeZone", "默认时间段");
								if (this.machine.IsTFTMachine)
								{
									this.numSensorDelayTime.Minimum = decimal.One;
									this.numSensorDelayTime.Maximum = 99m;
									this.lblUnitSensorDelay.Text = ShowMsgInfos.GetInfo("Second", "秒") + $"({this.numSensorDelayTime.Minimum}-{this.numSensorDelayTime.Maximum})";
								}
								if ((decimal)accDoor.ERRTimes >= this.numErrTimes.Minimum && (decimal)accDoor.ERRTimes <= this.numErrTimes.Maximum)
								{
									this.numErrTimes.Value = accDoor.ERRTimes;
								}
								else if ((decimal)accDoor.ERRTimes < this.numErrTimes.Minimum)
								{
									this.numErrTimes.Value = this.numErrTimes.Minimum;
								}
								else
								{
									this.numErrTimes.Value = this.numErrTimes.Maximum;
								}
								if ((decimal)accDoor.DoorSensorTimeout >= this.numSenserAlarmTime.Minimum && (decimal)accDoor.DoorSensorTimeout <= this.numSenserAlarmTime.Maximum)
								{
									this.numSenserAlarmTime.Value = accDoor.DoorSensorTimeout;
								}
								else if ((decimal)accDoor.DoorSensorTimeout < this.numSenserAlarmTime.Minimum)
								{
									this.numSenserAlarmTime.Value = this.numSenserAlarmTime.Minimum;
								}
								else
								{
									this.numSenserAlarmTime.Value = this.numSenserAlarmTime.Maximum;
								}
								this.ckbSRBOn.Checked = (accDoor.SRBOn == 1);
								this.ckbTwoPunchCardClose.Enabled = false;
							}
							else if (model.device_type == 12 || model.device_type == 101 || model.device_type == 102 || model.device_type == 103)
							{
								this.lab_reader1.Visible = false;
								this.lab_reader2.Visible = false;
								this.cmb_reader1.Visible = false;
								this.cmb_reader2.Visible = false;
								this.lab_master.BringToFront();
								this.lab_slave.BringToFront();
								this.cmb_master.BringToFront();
								this.cmb_slave.BringToFront();
								if (this.doorTemp.readerIOState == 0)
								{
									this.cmb_master.SelectedIndex = 1;
								}
								else
								{
									this.cmb_master.SelectedIndex = 0;
								}
								this.numLockDriveTime.Minimum = decimal.One;
								this.numLockDriveTime.Maximum = 10m;
								this.lblUnitLockDrivingTime.Text = ShowMsgInfos.GetInfo("Second", "秒") + $"({this.numLockDriveTime.Minimum}-{this.numLockDriveTime.Maximum})";
							}
							else
							{
								this.lab_reader1.BringToFront();
								this.lab_reader2.BringToFront();
								this.cmb_reader1.BringToFront();
								this.cmb_reader2.BringToFront();
								this.lab_master.Visible = false;
								this.lab_slave.Visible = false;
								this.cmb_master.Visible = false;
								this.cmb_slave.Visible = false;
								this.cmb_reader1.SelectedIndex = 0;
								this.cmb_reader2.SelectedIndex = 1;
								this.cmb_reader1.Enabled = false;
								this.cmb_reader2.Enabled = false;
								this.numLockDriveTime.Minimum = decimal.Zero;
								this.numLockDriveTime.Maximum = 254m;
								this.lblUnitLockDrivingTime.Text = ShowMsgInfos.GetInfo("Second", "秒") + $"({this.numLockDriveTime.Minimum}-{this.numLockDriveTime.Maximum})";
							}
							this.cmb_verifyMode.Items.Clear();
							if (this.verifyTypeList != null && this.verifyTypeList.Count > 0)
							{
								List<int> list = new List<int>();
								if (model.device_type <= 100)
								{
									switch (model.device_type)
									{
									case 1:
									case 2:
									case 4:
									case 7:
										list.AddRange(new int[4]
										{
											3,
											4,
											7,
											11
										});
										break;
									case 3:
									case 5:
									case 6:
									case 8:
									case 9:
									case 10:
										list.AddRange(new int[7]
										{
											1,
											3,
											4,
											6,
											7,
											10,
											11
										});
										break;
									default:
										list.AddRange(new int[25]
										{
											0,
											1,
											2,
											3,
											4,
											5,
											6,
											7,
											8,
											9,
											10,
											11,
											12,
											13,
											14,
											15,
											16,
											17,
											18,
											19,
											20,
											21,
											22,
											23,
											24
										});
										break;
									}
								}
								else
								{
									list.Add(0);
									if (model.SupportRFCard)
									{
										list.AddRange(new int[5]
										{
											2,
											3,
											4,
											7,
											11
										});
									}
									if (model.SupportFingerprint && !model.SupportFingerVein)
									{
										list.AddRange(new int[5]
										{
											1,
											5,
											8,
											9,
											13
										});
										if (model.SupportRFCard)
										{
											list.AddRange(new int[4]
											{
												6,
												10,
												12,
												14
											});
										}
									}
									if (model.SupportFace)
									{
										list.AddRange(new int[2]
										{
											15,
											17
										});
										if (model.SupportRFCard)
										{
											list.AddRange(new int[1]
											{
												18
											});
										}
										if (model.SupportFingerprint)
										{
											list.AddRange(new int[3]
											{
												16,
												19,
												20
											});
											if (model.SupportRFCard)
											{
												list.AddRange(new int[1]
												{
													19
												});
											}
										}
									}
									if (model.SupportFingerVein)
									{
										list.AddRange(new int[2]
										{
											21,
											22
										});
										if (model.SupportRFCard)
										{
											list.AddRange(new int[2]
											{
												23,
												24
											});
										}
									}
								}
								list.Sort();
								for (int i = 0; i < list.Count; i++)
								{
									if (this.verifyTypeList.ContainsKey(list[i]))
									{
										this.cmb_verifyMode.Items.Add(this.verifyTypeList[list[i]]);
									}
								}
							}
						}
						this.chk_cardbox.Checked = accDoor.cardBox;
						this.txt_name.Text = accDoor.door_name;
						if (accDoor.sensor_delay > 0)
						{
							if ((decimal)accDoor.sensor_delay >= this.numSensorDelayTime.Minimum && (decimal)accDoor.sensor_delay <= this.numSensorDelayTime.Maximum)
							{
								this.numSensorDelayTime.Value = accDoor.sensor_delay;
							}
							else if ((decimal)accDoor.sensor_delay < this.numSensorDelayTime.Minimum)
							{
								this.numSensorDelayTime.Value = this.numSensorDelayTime.Minimum;
							}
							else
							{
								this.numSensorDelayTime.Value = this.numSensorDelayTime.Maximum;
							}
						}
						this.ch_closeAndLock.Checked = accDoor.back_lock;
						if (!string.IsNullOrEmpty(accDoor.supper_pwd))
						{
							this.urgent = true;
							this.lbl_urgentPwd.Text = ShowMsgInfos.GetInfo("SSetPW", "重置");
						}
						if (!string.IsNullOrEmpty(accDoor.force_pwd))
						{
							this.force = true;
							this.btn_seetingPwd.Text = ShowMsgInfos.GetInfo("SSetPW", "重置");
						}
						this.urgentValue = accDoor.supper_pwd;
						this.forceValue = accDoor.force_pwd;
						decimal num = accDoor.lock_delay;
						if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK && this.machine.platform != null && (this.machine.platform.ToUpper().Contains("ZEM560") || this.machine.platform.ToUpper().Contains("ZEM500")))
						{
							num = num * 4m / 100m;
							this.numLockDriveTime.DecimalPlaces = 1;
						}
						if (num >= this.numLockDriveTime.Minimum && num <= this.numLockDriveTime.Maximum)
						{
							this.numLockDriveTime.Value = num;
						}
						else if (num < this.numLockDriveTime.Minimum)
						{
							this.numLockDriveTime.Value = this.numLockDriveTime.Minimum;
						}
						else
						{
							this.numLockDriveTime.Value = this.numLockDriveTime.Maximum;
						}
						this.numPunchInterval.Value = accDoor.card_intervaltime;
						this.chk_isAtt.Checked = accDoor.is_att;
						this.txt_doorNO.Text = accDoor.door_no.ToString();
						if (this.cmb_doorSensor.Items.Count > accDoor.door_sensor_status)
						{
							this.cmb_doorSensor.SelectedIndex = accDoor.door_sensor_status;
							if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
							{
								this.ch_closeAndLock.Enabled = false;
							}
							else if (this.cmb_doorSensor.SelectedIndex > 0)
							{
								this.ch_closeAndLock.Enabled = true;
								this.lbl_doorStatus.Enabled = true;
								this.pnlSensorType.Enabled = true;
							}
							else
							{
								this.ch_closeAndLock.Enabled = false;
								this.lbl_doorStatus.Enabled = false;
								this.pnlSensorType.Enabled = false;
							}
						}
						if (this.verifyTypeList != null)
						{
							foreach (KeyValuePair<int, string> verifyType in this.verifyTypeList)
							{
								if (verifyType.Key == accDoor.opendoor_type)
								{
									this.cmb_verifyMode.Text = verifyType.Value;
									break;
								}
							}
						}
						this.cmb_doorActive.SelectedValue = accDoor.lock_active_id;
						this.cmb_normalOpen.SelectedValue = accDoor.long_open_id;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.lblSensorAlarmTime.Text = ShowMsgInfos.GetInfo("Second", "秒") + $"({this.numSenserAlarmTime.Minimum}-{this.numSenserAlarmTime.Maximum})";
			initLang.ComboBoxAutoSize(this.cmb_verifyMode, this);
		}

		private void DoorDataModel(AccDoor door, AccDoor sourcedoor)
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			Machines model = machinesBll.GetModel(door.device_id);
			Machines model2 = machinesBll.GetModel(sourcedoor.device_id);
			door.back_lock = sourcedoor.back_lock;
			door.lock_delay = sourcedoor.lock_delay;
			door.card_intervaltime = sourcedoor.card_intervaltime;
			door.duration_apb = sourcedoor.duration_apb;
			door.sensor_delay = sourcedoor.sensor_delay;
			door.door_sensor_status = sourcedoor.door_sensor_status;
			if (model2.IsOnlyRFMachine != 0)
			{
				door.opendoor_type = sourcedoor.opendoor_type;
			}
			else if (model != null && model.IsOnlyRFMachine == 0)
			{
				door.opendoor_type = sourcedoor.opendoor_type;
			}
			door.lock_active_id = sourcedoor.lock_active_id;
			door.long_open_id = sourcedoor.long_open_id;
			door.supper_pwd = sourcedoor.supper_pwd;
			door.force_pwd = sourcedoor.force_pwd;
			door.is_att = sourcedoor.is_att;
			door.wiegand_fmt_id = sourcedoor.wiegand_fmt_id;
			door.wiegand_fmt_out_id = sourcedoor.wiegand_fmt_out_id;
			door.wiegandInType = sourcedoor.wiegandInType;
			door.wiegandOutType = sourcedoor.wiegandOutType;
			door.SRBOn = sourcedoor.SRBOn;
			door.ERRTimes = sourcedoor.ERRTimes;
			door.DoorSensorTimeout = sourcedoor.DoorSensorTimeout;
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				door.ManualCtlMode = sourcedoor.ManualCtlMode;
			}
		}

		private bool BindModel(AccDoor door)
		{
			try
			{
				if (this.check())
				{
					if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
					{
						door.ManualCtlMode = (this.ckbTwoPunchCardClose.Checked ? 1 : 0);
					}
					door.door_name = this.txt_name.Text;
					door.cardBox = this.chk_cardbox.Checked;
					door.back_lock = this.ch_closeAndLock.Checked;
					if (this.setUrgentValue == 1)
					{
						door.supper_pwd = this.urgentSetValue;
					}
					else
					{
						door.supper_pwd = this.urgentValue;
					}
					if (this.setValue == 1)
					{
						door.force_pwd = this.forceSetValue;
					}
					else
					{
						door.force_pwd = this.forceValue;
					}
					door.lock_delay = (int)this.numLockDriveTime.Value;
					if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK && this.machine.platform != null && (this.machine.platform.ToUpper().Contains("ZEM560") || this.machine.platform.ToUpper().Contains("ZEM500")))
					{
						door.lock_delay = (int)(this.numLockDriveTime.Value * 100m / 4m);
					}
					door.card_intervaltime = (int)this.numPunchInterval.Value;
					door.sensor_delay = (int)this.numSensorDelayTime.Value;
					door.door_sensor_status = this.cmb_doorSensor.SelectedIndex;
					if (this.verifyTypeList != null)
					{
						foreach (KeyValuePair<int, string> verifyType in this.verifyTypeList)
						{
							if (verifyType.Value == this.cmb_verifyMode.Text)
							{
								door.opendoor_type = verifyType.Key;
								break;
							}
						}
					}
					door.is_att = this.chk_isAtt.Checked;
					door.lock_active_id = (int)(this.cmb_doorActive.SelectedValue ?? ((object)0));
					door.long_open_id = (int)(this.cmb_normalOpen.SelectedValue ?? ((object)0));
					if (this.cmb_master.SelectedIndex == 0)
					{
						door.readerIOState = 1;
					}
					else if (this.cmb_master.SelectedIndex == 1)
					{
						door.readerIOState = 0;
					}
					if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
					{
						door.readerIOState = ((this.cmb_master.SelectedIndex != 2) ? ((this.cmb_master.SelectedIndex == 1) ? 1 : (-1)) : 0);
					}
					if (this.wiegand_door != null)
					{
						door.wiegand_fmt_id = this.wiegand_door.wiegand_fmt_id;
						door.wiegand_fmt_out_id = this.wiegand_door.wiegand_fmt_out_id;
						door.wiegandInType = this.wiegand_door.wiegandInType;
						door.wiegandOutType = this.wiegand_door.wiegandOutType;
						door.SRBOn = this.wiegand_door.SRBOn;
					}
					if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
					{
						door.SRBOn = (this.ckbSRBOn.Checked ? 1 : 0);
						door.ERRTimes = (int)this.numErrTimes.Value;
						door.DoorSensorTimeout = (int)this.numSenserAlarmTime.Value;
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
				if (string.IsNullOrEmpty(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDoorNameNotNull", "门名称不能为空"));
					this.txt_name.Focus();
					return false;
				}
				if (this.cmb_doorSensor.SelectedIndex > 0 && this.numSensorDelayTime.Value < this.numLockDriveTime.Value)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSensorDelayLongerThanDoorStatus", "门磁延时需大于锁驱动时长"));
					this.numSensorDelayTime.Focus();
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool dooSettingChanged(AccDoor newDoor, AccDoor oldDoor)
		{
			if (newDoor.back_lock != oldDoor.back_lock || newDoor.card_intervaltime != oldDoor.card_intervaltime || newDoor.door_sensor_status != oldDoor.door_sensor_status || newDoor.force_pwd != oldDoor.force_pwd || newDoor.lock_active_id != oldDoor.lock_active_id || newDoor.lock_delay != oldDoor.lock_delay || newDoor.long_open_id != oldDoor.long_open_id || newDoor.opendoor_type != oldDoor.opendoor_type || newDoor.sensor_delay != oldDoor.sensor_delay || newDoor.supper_pwd != oldDoor.supper_pwd || newDoor.readerIOState != oldDoor.readerIOState || this.chk_currentAccess.Checked || this.chk_allAccess.Checked || newDoor.wiegand_fmt_id != oldDoor.wiegand_fmt_id || newDoor.wiegand_fmt_out_id != oldDoor.wiegand_fmt_out_id || newDoor.wiegandInType != oldDoor.wiegandInType || newDoor.wiegandOutType != oldDoor.wiegandOutType || newDoor.ManualCtlMode != oldDoor.ManualCtlMode || newDoor.ERRTimes != oldDoor.ERRTimes || newDoor.SRBOn != oldDoor.SRBOn || newDoor.DoorSensorTimeout != oldDoor.DoorSensorTimeout)
			{
				return true;
			}
			return false;
		}

		private void SaveDoorSettingLog(AccDoor newDoor, AccDoor oldDoor)
		{
			if (newDoor.door_name != oldDoor.door_name)
			{
				OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SLogModifyDoorName", "修改门名称") + "：" + oldDoor.door_name + "->" + newDoor.door_name, 3, "door");
			}
			if (newDoor.lock_active_id != oldDoor.lock_active_id)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorActiveTime", "修改门有效时间段"), 3, "door");
			}
			if (newDoor.long_open_id != oldDoor.long_open_id)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorLongOpen", "修改门常开时间段"), 3, "door");
			}
			if (newDoor.lock_delay != oldDoor.lock_delay)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorLockDelay", "修改门锁驱动时常") + " " + newDoor.lock_delay, 3, "door");
			}
			if (newDoor.card_intervaltime != oldDoor.card_intervaltime)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorCardIntervalTime", "修改门刷卡间隔") + " " + newDoor.card_intervaltime, 3, "door");
			}
			if (newDoor.door_sensor_status != oldDoor.door_sensor_status)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorSensorStatus", "修改门磁类型") + " " + this.cmb_doorSensor.Text, 3, "door");
			}
			if (newDoor.sensor_delay != oldDoor.sensor_delay)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorSensorDelay", "修改门锁驱动时常") + newDoor.sensor_delay, 3, "door");
			}
			if (newDoor.back_lock != oldDoor.back_lock)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorBackLock", "修改门闭门回锁"), 3, "door");
			}
			if (newDoor.opendoor_type != oldDoor.opendoor_type)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorVerifyType", "修改门验证方式") + " " + this.cmb_verifyMode.Text, 3, "door");
			}
			if (newDoor.force_pwd != oldDoor.force_pwd)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorForcePwd", "修改门胁迫密码"), 3, "door");
			}
			if (newDoor.supper_pwd != oldDoor.supper_pwd)
			{
				OperationLog.SaveOperationLog(newDoor.door_name + ":" + ShowMsgInfos.GetInfo("SLogModifyDoorSupperPwd", "修改门紧急密码"), 3, "door");
			}
		}

		private bool Save()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				AccDoor accDoor = null;
				AccDoor accDoor2 = null;
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				accDoor = accDoorBll.GetModel(this.m_id);
				if (accDoor == null)
				{
					this.Cursor = Cursors.Default;
					return true;
				}
				accDoor2 = accDoor.Copy();
				if (this.BindModel(accDoor))
				{
					if (accDoorBll.Update(accDoor))
					{
						List<AccDoor> list = null;
						if (this.chk_allAccess.Checked)
						{
							list = accDoorBll.GetModelList("device_id in (Select id From Machines)");
						}
						else if (this.chk_currentAccess.Checked)
						{
							list = accDoorBll.GetModelList("device_id=" + accDoor.device_id);
							if (list == null)
							{
								list = new List<AccDoor>();
							}
							list.Add(accDoor);
						}
						if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
						{
							AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
							if (accDoor.lock_active_id != accDoor2.lock_active_id)
							{
								AccTimeseg model = accTimesegBll.GetModel(accDoor.lock_active_id);
								if (model != null)
								{
									CommandServer.AddTimeZone(this.machine, model);
								}
							}
							if (accDoor.long_open_id != accDoor2.long_open_id && accDoor.long_open_id != accDoor.lock_active_id)
							{
								List<AccTimeseg> modelList = accTimesegBll.GetModelList(string.Format("TimeZone1Id={0} or TimeZone2Id={0} or TimeZone3Id={0} or TimeZoneHolidayId={0}", accDoor.long_open_id));
								AccTimeseg model = (modelList != null && modelList.Count > 0) ? modelList[0] : null;
								if (model != null)
								{
									CommandServer.AddTimeZone(this.machine, model);
								}
							}
						}
						CommandServer.SetDoorParam(accDoor);
						if (this.machine != null && this.machine.DevSDKType != SDKType.StandaloneSDK)
						{
							CommandServer.SetWiegandParam(accDoor);
						}
						StringBuilder stringBuilder = new StringBuilder();
						if (list != null && list.Count > 0)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (this.m_id != list[i].id)
								{
									this.DoorDataModel(list[i], accDoor);
									if (!accDoorBll.Update(list[i]))
									{
										stringBuilder.Append(ShowMsgInfos.GetInfo("UpdatesError", "更新失败") + ":" + list[i].door_name + "\r\n");
									}
									CommandServer.SetDoorParam(list[i]);
									if (this.machine != null && this.machine.DevSDKType != SDKType.StandaloneSDK)
									{
										CommandServer.SetWiegandParam(list[i]);
									}
								}
							}
						}
						if (this.doorTemp != null)
						{
							this.SaveDoorSettingLog(accDoor, this.doorTemp);
						}
						if (this.dooSettingChanged(accDoor, this.doorTemp))
						{
							FrmShowUpdata.Instance.ShowEx();
						}
						if (this.refreshDataEvent != null)
						{
							this.refreshDataEvent(this, null);
						}
						if (!string.IsNullOrEmpty(stringBuilder.ToString()))
						{
							SysDialogs.ShowInfoMessage(stringBuilder.ToString());
							goto IL_03b6;
						}
						return true;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UpdatesError", "更新失败"));
				}
				goto IL_03b6;
				IL_03b6:
				this.Cursor = Cursors.Default;
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
				return false;
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private void cmb_dev_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e);
		}

		private void txt_LockDuration_KeyPress(object sender, KeyPressEventArgs e)
		{
		}

		private void txt_PunchInterval_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 0, 10L);
		}

		private void txt_Anti_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 1440L);
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void cmb_DoorSensor_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_doorSensor.SelectedIndex > 0)
			{
				this.ch_closeAndLock.Enabled = true;
				this.lbl_doorStatus.Enabled = true;
				this.pnlSensorType.Enabled = true;
				if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
				{
					this.ch_closeAndLock.Enabled = false;
				}
			}
			else
			{
				this.ch_closeAndLock.Enabled = false;
				this.lbl_doorStatus.Enabled = false;
				this.pnlSensorType.Enabled = false;
			}
		}

		private void txt_doorStatus_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK && this.machine.IsTFTMachine)
			{
				CheckInfo.NumberKeyPress(sender, e, 1, 99L);
			}
			else
			{
				CheckInfo.NumberKeyPress(sender, e, 1, 254L);
			}
		}

		private void duressPWForm_DataEvent(object sender, EventArgs e)
		{
			if (sender != null)
			{
				this.forceSetValue = sender.ToString();
				this.setValue = 1;
			}
		}

		private void txt_forcePwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 7);
		}

		private void txt_forceNewPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 7);
		}

		private void txt_forceNewPwdOk_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 7);
		}

		private void txt_UrgentPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 7);
		}

		private void txt_UrgentNewPwd_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 7);
		}

		private void txt_UrgentNewOk_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 7);
		}

		private void btn_seetingPW_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.force)
				{
					DuressPWForm duressPWForm = new DuressPWForm(this.force, this.forceValue);
					duressPWForm.refreshDataEvent += this.duressPWForm_DataEvent;
					duressPWForm.ShowDialog();
					duressPWForm.refreshDataEvent -= this.duressPWForm_DataEvent;
				}
				else
				{
					DuressPWForm duressPWForm2 = new DuressPWForm(this.force, this.forceValue);
					duressPWForm2.refreshDataEvent += this.duressPWForm_DataEvent;
					duressPWForm2.ShowDialog();
					duressPWForm2.refreshDataEvent -= this.duressPWForm_DataEvent;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void lbl_urgentPwd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				if (this.urgent)
				{
					UrgentPwdForm urgentPwdForm = new UrgentPwdForm(this.urgent, this.urgentValue);
					urgentPwdForm.refreshDataEvent += this.urgentPwdForm_refreshDataEvent;
					urgentPwdForm.ShowDialog();
					urgentPwdForm.refreshDataEvent -= this.urgentPwdForm_refreshDataEvent;
				}
				else
				{
					UrgentPwdForm urgentPwdForm2 = new UrgentPwdForm(this.urgent, this.urgentValue);
					urgentPwdForm2.refreshDataEvent += this.urgentPwdForm_refreshDataEvent;
					urgentPwdForm2.ShowDialog();
					urgentPwdForm2.refreshDataEvent -= this.urgentPwdForm_refreshDataEvent;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void urgentPwdForm_refreshDataEvent(object sender, EventArgs e)
		{
			if (sender != null)
			{
				this.urgentSetValue = sender.ToString();
				this.setUrgentValue = 1;
			}
		}

		private void cmb_master_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_master.SelectedIndex >= 0 && (this.machine == null || this.machine.DevSDKType != SDKType.StandaloneSDK))
			{
				if (this.cmb_master.SelectedIndex == 0)
				{
					this.cmb_slave.SelectedIndex = 1;
				}
				else
				{
					this.cmb_slave.SelectedIndex = 0;
				}
			}
		}

		private void cmb_slave_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.cmb_slave.SelectedIndex >= 0)
			{
				if (this.cmb_slave.SelectedIndex == 0)
				{
					this.cmb_master.SelectedIndex = 1;
				}
				else
				{
					this.cmb_master.SelectedIndex = 0;
				}
			}
		}

		private void linkLabel_wiegand_fmt_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			WiegandSeting wiegandSeting = new WiegandSeting(0, this.doorTemp.id);
			wiegandSeting.refreshDataEvent += this.refreshdataWiegandFmt;
			wiegandSeting.ShowDialog();
			wiegandSeting.refreshDataEvent -= this.refreshdataWiegandFmt;
		}

		private void btn_wiegandIn_set_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.machine != null && this.machine.DevSDKType == SDKType.StandaloneSDK)
			{
				STD_WiegandSetting sTD_WiegandSetting = new STD_WiegandSetting(this.doorTemp.id);
				sTD_WiegandSetting.ShowDialog();
			}
			else
			{
				WiegandSeting wiegandSeting = new WiegandSeting(1, this.doorTemp.id);
				wiegandSeting.refreshDataEvent += this.refreshdataWiegandFmt;
				wiegandSeting.ShowDialog();
				wiegandSeting.refreshDataEvent -= this.refreshdataWiegandFmt;
			}
		}

		private void refreshdataWiegandFmt(object sender, EventArgs e)
		{
			AccDoor accDoor = sender as AccDoor;
			if (accDoor != null)
			{
				this.wiegand_door = new AccDoor();
				this.wiegand_door.wiegand_fmt_id = accDoor.wiegand_fmt_id;
				this.wiegand_door.wiegandInType = accDoor.wiegandInType;
				this.wiegand_door.wiegandOutType = accDoor.wiegandOutType;
				this.wiegand_door.wiegand_fmt_out_id = accDoor.wiegand_fmt_out_id;
				this.wiegand_door.SRBOn = accDoor.SRBOn;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(doorSettingForm));
			this.lab_deviceName = new Label();
			this.lab_doorNO = new Label();
			this.lab_doorName = new Label();
			this.lab_doorActive = new Label();
			this.lab_normalOpen = new Label();
			this.lab_lockDuration = new Label();
			this.lab_punchInterval = new Label();
			this.lab_doorSensor = new Label();
			this.lab_verifyMode = new Label();
			this.lab_duress = new Label();
			this.lab_emergencyPassword = new Label();
			this.cmb_doorActive = new ComboBoxEx();
			this.cmb_normalOpen = new ComboBoxEx();
			this.cmb_doorSensor = new ComboBoxEx();
			this.cmb_verifyMode = new ComboBoxEx();
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.label6 = new Label();
			this.label7 = new Label();
			this.label8 = new Label();
			this.txt_name = new TextBox();
			this.ch_closeAndLock = new CheckBox();
			this.lbl_doorStatus = new Label();
			this.txt_devName = new TextBoxX();
			this.txt_doorNO = new TextBoxX();
			this.btn_seetingPwd = new LinkLabel();
			this.lbl_urgentPwd = new LinkLabel();
			this.panel1 = new Panel();
			this.chk_currentAccess = new CheckBox();
			this.chk_allAccess = new CheckBox();
			this.chk_isAtt = new CheckBox();
			this.cmb_slave = new ComboBoxEx();
			this.lab_slave = new Label();
			this.cmb_master = new ComboBoxEx();
			this.lab_master = new Label();
			this.cmb_reader1 = new ComboBoxEx();
			this.cmb_reader2 = new ComboBoxEx();
			this.lab_reader1 = new Label();
			this.lab_reader2 = new Label();
			this.btn_wiegand_set = new LinkLabel();
			this.linkLabel_wiegand_fmt = new LinkLabel();
			this.lbl_wigand = new Label();
			this.gop_Reader1IOState = new Panel();
			this.numErrTimes = new NumericUpDown();
			this.label9 = new Label();
			this.label10 = new Label();
			this.numSenserAlarmTime = new NumericUpDown();
			this.numLockDriveTime = new NumericUpDown();
			this.numPunchInterval = new NumericUpDown();
			this.numSensorDelayTime = new NumericUpDown();
			this.ckbSRBOn = new CheckBox();
			this.pnlSensorType = new Panel();
			this.lblUnitSensorDelay = new Label();
			this.pnlStdParam = new Panel();
			this.lblSensorAlarmTime = new Label();
			this.lblUnitLockDrivingTime = new Label();
			this.lblPunchInterval = new Label();
			this.flowLayoutPanel1 = new FlowLayoutPanel();
			this.ckbTwoPunchCardClose = new CheckBox();
			this.chk_cardbox = new CheckBox();
			this.gop_Reader1IOState.SuspendLayout();
			((ISupportInitialize)this.numErrTimes).BeginInit();
			((ISupportInitialize)this.numSenserAlarmTime).BeginInit();
			((ISupportInitialize)this.numLockDriveTime).BeginInit();
			((ISupportInitialize)this.numPunchInterval).BeginInit();
			((ISupportInitialize)this.numSensorDelayTime).BeginInit();
			this.pnlSensorType.SuspendLayout();
			this.pnlStdParam.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.lab_deviceName.BackColor = Color.Transparent;
			this.lab_deviceName.Location = new Point(16, 16);
			this.lab_deviceName.Name = "lab_deviceName";
			this.lab_deviceName.Size = new Size(170, 13);
			this.lab_deviceName.TabIndex = 0;
			this.lab_deviceName.Text = "设备名称";
			this.lab_deviceName.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_doorNO.BackColor = Color.Transparent;
			this.lab_doorNO.Location = new Point(16, 46);
			this.lab_doorNO.Name = "lab_doorNO";
			this.lab_doorNO.Size = new Size(170, 13);
			this.lab_doorNO.TabIndex = 1;
			this.lab_doorNO.Text = "门编号";
			this.lab_doorNO.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_doorName.BackColor = Color.Transparent;
			this.lab_doorName.Location = new Point(16, 75);
			this.lab_doorName.Name = "lab_doorName";
			this.lab_doorName.Size = new Size(170, 13);
			this.lab_doorName.TabIndex = 2;
			this.lab_doorName.Text = "门名称";
			this.lab_doorName.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_doorActive.BackColor = Color.Transparent;
			this.lab_doorActive.Location = new Point(16, 104);
			this.lab_doorActive.Name = "lab_doorActive";
			this.lab_doorActive.Size = new Size(170, 13);
			this.lab_doorActive.TabIndex = 3;
			this.lab_doorActive.Text = "门有效时间段";
			this.lab_doorActive.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_normalOpen.BackColor = Color.Transparent;
			this.lab_normalOpen.Location = new Point(16, 133);
			this.lab_normalOpen.Name = "lab_normalOpen";
			this.lab_normalOpen.Size = new Size(170, 13);
			this.lab_normalOpen.TabIndex = 4;
			this.lab_normalOpen.Text = "门常开时间段";
			this.lab_normalOpen.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_lockDuration.BackColor = Color.Transparent;
			this.lab_lockDuration.Location = new Point(395, 133);
			this.lab_lockDuration.Name = "lab_lockDuration";
			this.lab_lockDuration.Size = new Size(171, 13);
			this.lab_lockDuration.TabIndex = 5;
			this.lab_lockDuration.Text = "锁驱动时长";
			this.lab_lockDuration.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_punchInterval.BackColor = Color.Transparent;
			this.lab_punchInterval.Location = new Point(395, 163);
			this.lab_punchInterval.Name = "lab_punchInterval";
			this.lab_punchInterval.Size = new Size(171, 13);
			this.lab_punchInterval.TabIndex = 6;
			this.lab_punchInterval.Text = "刷卡间隔";
			this.lab_punchInterval.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_doorSensor.BackColor = Color.Transparent;
			this.lab_doorSensor.Location = new Point(395, 16);
			this.lab_doorSensor.Name = "lab_doorSensor";
			this.lab_doorSensor.Size = new Size(178, 13);
			this.lab_doorSensor.TabIndex = 8;
			this.lab_doorSensor.Text = "门磁类型";
			this.lab_doorSensor.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_verifyMode.BackColor = Color.Transparent;
			this.lab_verifyMode.Location = new Point(16, 163);
			this.lab_verifyMode.Name = "lab_verifyMode";
			this.lab_verifyMode.Size = new Size(170, 13);
			this.lab_verifyMode.TabIndex = 9;
			this.lab_verifyMode.Text = "验证方式";
			this.lab_verifyMode.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_duress.BackColor = Color.Transparent;
			this.lab_duress.Location = new Point(16, 257);
			this.lab_duress.Name = "lab_duress";
			this.lab_duress.Size = new Size(170, 13);
			this.lab_duress.TabIndex = 10;
			this.lab_duress.Text = "胁迫密码";
			this.lab_duress.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_emergencyPassword.BackColor = Color.Transparent;
			this.lab_emergencyPassword.Location = new Point(16, 286);
			this.lab_emergencyPassword.Name = "lab_emergencyPassword";
			this.lab_emergencyPassword.Size = new Size(170, 13);
			this.lab_emergencyPassword.TabIndex = 11;
			this.lab_emergencyPassword.Text = "紧急状态密码";
			this.lab_emergencyPassword.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_doorActive.DisplayMember = "Text";
			this.cmb_doorActive.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_doorActive.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_doorActive.FormattingEnabled = true;
			this.cmb_doorActive.ItemHeight = 15;
			this.cmb_doorActive.Location = new Point(193, 100);
			this.cmb_doorActive.Name = "cmb_doorActive";
			this.cmb_doorActive.Size = new Size(159, 21);
			this.cmb_doorActive.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_doorActive.TabIndex = 3;
			this.cmb_normalOpen.DisplayMember = "Text";
			this.cmb_normalOpen.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_normalOpen.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_normalOpen.FormattingEnabled = true;
			this.cmb_normalOpen.ItemHeight = 15;
			this.cmb_normalOpen.Location = new Point(193, 129);
			this.cmb_normalOpen.Name = "cmb_normalOpen";
			this.cmb_normalOpen.Size = new Size(159, 21);
			this.cmb_normalOpen.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_normalOpen.TabIndex = 4;
			this.cmb_doorSensor.DisplayMember = "Text";
			this.cmb_doorSensor.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_doorSensor.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_doorSensor.FormattingEnabled = true;
			this.cmb_doorSensor.ItemHeight = 15;
			this.cmb_doorSensor.Location = new Point(572, 12);
			this.cmb_doorSensor.Name = "cmb_doorSensor";
			this.cmb_doorSensor.Size = new Size(159, 21);
			this.cmb_doorSensor.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_doorSensor.TabIndex = 7;
			this.cmb_doorSensor.SelectedIndexChanged += this.cmb_DoorSensor_SelectedIndexChanged;
			this.cmb_verifyMode.DisplayMember = "Text";
			this.cmb_verifyMode.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_verifyMode.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_verifyMode.FormattingEnabled = true;
			this.cmb_verifyMode.ItemHeight = 15;
			this.cmb_verifyMode.Location = new Point(193, 158);
			this.cmb_verifyMode.Name = "cmb_verifyMode";
			this.cmb_verifyMode.Size = new Size(159, 21);
			this.cmb_verifyMode.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_verifyMode.TabIndex = 10;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(562, 397);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 25);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 18;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(664, 397);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 19;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(355, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 13);
			this.label1.TabIndex = 35;
			this.label1.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(355, 163);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 13);
			this.label2.TabIndex = 36;
			this.label2.Text = "*";
			this.label3.AutoSize = true;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(734, 16);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 13);
			this.label3.TabIndex = 37;
			this.label3.Text = "*";
			this.label6.AutoSize = true;
			this.label6.ForeColor = Color.Red;
			this.label6.Location = new Point(355, 104);
			this.label6.Name = "label6";
			this.label6.Size = new Size(11, 13);
			this.label6.TabIndex = 40;
			this.label6.Text = "*";
			this.label7.AutoSize = true;
			this.label7.ForeColor = Color.Red;
			this.label7.Location = new Point(355, 75);
			this.label7.Name = "label7";
			this.label7.Size = new Size(11, 13);
			this.label7.TabIndex = 41;
			this.label7.Text = "*";
			this.label8.AutoSize = true;
			this.label8.ForeColor = Color.Red;
			this.label8.Location = new Point(355, 46);
			this.label8.Name = "label8";
			this.label8.Size = new Size(11, 13);
			this.label8.TabIndex = 42;
			this.label8.Text = "*";
			this.txt_name.Location = new Point(193, 70);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(158, 20);
			this.txt_name.TabIndex = 2;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.ch_closeAndLock.BackColor = Color.Transparent;
			this.ch_closeAndLock.CheckAlign = ContentAlignment.MiddleRight;
			this.ch_closeAndLock.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 128, 0);
			this.ch_closeAndLock.Location = new Point(1, 33);
			this.ch_closeAndLock.Name = "ch_closeAndLock";
			this.ch_closeAndLock.Size = new Size(193, 20);
			this.ch_closeAndLock.TabIndex = 9;
			this.ch_closeAndLock.Text = "闭门回锁";
			this.ch_closeAndLock.UseVisualStyleBackColor = false;
			this.lbl_doorStatus.BackColor = Color.Transparent;
			this.lbl_doorStatus.Location = new Point(3, 8);
			this.lbl_doorStatus.Name = "lbl_doorStatus";
			this.lbl_doorStatus.Size = new Size(178, 13);
			this.lbl_doorStatus.TabIndex = 45;
			this.lbl_doorStatus.Text = "门磁延时";
			this.lbl_doorStatus.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_devName.Border.Class = "TextBoxBorder";
			this.txt_devName.Enabled = false;
			this.txt_devName.Location = new Point(193, 12);
			this.txt_devName.Name = "txt_devName";
			this.txt_devName.ReadOnly = true;
			this.txt_devName.Size = new Size(158, 20);
			this.txt_devName.TabIndex = 0;
			this.txt_doorNO.Border.Class = "TextBoxBorder";
			this.txt_doorNO.Enabled = false;
			this.txt_doorNO.Location = new Point(193, 41);
			this.txt_doorNO.Name = "txt_doorNO";
			this.txt_doorNO.ReadOnly = true;
			this.txt_doorNO.Size = new Size(158, 20);
			this.txt_doorNO.TabIndex = 1;
			this.btn_seetingPwd.AutoSize = true;
			this.btn_seetingPwd.Location = new Point(191, 257);
			this.btn_seetingPwd.Name = "btn_seetingPwd";
			this.btn_seetingPwd.Size = new Size(31, 13);
			this.btn_seetingPwd.TabIndex = 11;
			this.btn_seetingPwd.TabStop = true;
			this.btn_seetingPwd.Text = "设置";
			this.btn_seetingPwd.Click += this.btn_seetingPW_Click;
			this.lbl_urgentPwd.AutoSize = true;
			this.lbl_urgentPwd.Location = new Point(191, 286);
			this.lbl_urgentPwd.Name = "lbl_urgentPwd";
			this.lbl_urgentPwd.Size = new Size(31, 13);
			this.lbl_urgentPwd.TabIndex = 12;
			this.lbl_urgentPwd.TabStop = true;
			this.lbl_urgentPwd.Text = "设置";
			this.lbl_urgentPwd.LinkClicked += this.lbl_urgentPwd_LinkClicked;
			this.panel1.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
			this.panel1.BackColor = Color.Black;
			this.panel1.ForeColor = Color.Black;
			this.panel1.Location = new Point(16, 338);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(732, 1);
			this.panel1.TabIndex = 75;
			this.chk_currentAccess.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.chk_currentAccess.AutoSize = true;
			this.chk_currentAccess.CheckAlign = ContentAlignment.MiddleRight;
			this.chk_currentAccess.Location = new Point(11, 352);
			this.chk_currentAccess.Name = "chk_currentAccess";
			this.chk_currentAccess.Size = new Size(206, 17);
			this.chk_currentAccess.TabIndex = 16;
			this.chk_currentAccess.Text = "将该设置应用于当前控制器所有门";
			this.chk_currentAccess.UseVisualStyleBackColor = true;
			this.chk_allAccess.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.chk_allAccess.AutoSize = true;
			this.chk_allAccess.CheckAlign = ContentAlignment.MiddleRight;
			this.chk_allAccess.Location = new Point(11, 372);
			this.chk_allAccess.Name = "chk_allAccess";
			this.chk_allAccess.Size = new Size(206, 17);
			this.chk_allAccess.TabIndex = 17;
			this.chk_allAccess.Text = "将该设置应用于所有控制器所有门";
			this.chk_allAccess.UseVisualStyleBackColor = true;
			this.chk_isAtt.BackColor = Color.Transparent;
			this.chk_isAtt.CheckAlign = ContentAlignment.MiddleRight;
			this.chk_isAtt.FlatAppearance.BorderSize = 2;
			this.chk_isAtt.FlatAppearance.CheckedBackColor = Color.FromArgb(255, 128, 0);
			this.chk_isAtt.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 128, 0);
			this.chk_isAtt.Location = new Point(393, 101);
			this.chk_isAtt.Name = "chk_isAtt";
			this.chk_isAtt.Size = new Size(193, 20);
			this.chk_isAtt.TabIndex = 13;
			this.chk_isAtt.Text = "考勤";
			this.chk_isAtt.TextAlign = ContentAlignment.BottomLeft;
			this.chk_isAtt.UseVisualStyleBackColor = false;
			this.cmb_slave.DisplayMember = "Text";
			this.cmb_slave.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_slave.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_slave.FormattingEnabled = true;
			this.cmb_slave.ItemHeight = 15;
			this.cmb_slave.Location = new Point(184, 33);
			this.cmb_slave.Name = "cmb_slave";
			this.cmb_slave.Size = new Size(160, 21);
			this.cmb_slave.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_slave.TabIndex = 15;
			this.cmb_slave.SelectedIndexChanged += this.cmb_slave_SelectedIndexChanged;
			this.lab_slave.BackColor = Color.Transparent;
			this.lab_slave.Location = new Point(7, 37);
			this.lab_slave.Name = "lab_slave";
			this.lab_slave.Size = new Size(171, 13);
			this.lab_slave.TabIndex = 78;
			this.lab_slave.Text = "从机出入状态";
			this.lab_slave.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_master.DisplayMember = "Text";
			this.cmb_master.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_master.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_master.FormattingEnabled = true;
			this.cmb_master.ItemHeight = 15;
			this.cmb_master.Location = new Point(184, 3);
			this.cmb_master.Name = "cmb_master";
			this.cmb_master.Size = new Size(160, 21);
			this.cmb_master.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_master.TabIndex = 14;
			this.cmb_master.SelectedIndexChanged += this.cmb_master_SelectedIndexChanged;
			this.lab_master.BackColor = Color.Transparent;
			this.lab_master.Location = new Point(7, 8);
			this.lab_master.Name = "lab_master";
			this.lab_master.Size = new Size(171, 13);
			this.lab_master.TabIndex = 80;
			this.lab_master.Text = "主机出入状态";
			this.lab_master.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_reader1.DisplayMember = "Text";
			this.cmb_reader1.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_reader1.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_reader1.FormattingEnabled = true;
			this.cmb_reader1.ItemHeight = 15;
			this.cmb_reader1.Location = new Point(184, 3);
			this.cmb_reader1.Name = "cmb_reader1";
			this.cmb_reader1.Size = new Size(160, 21);
			this.cmb_reader1.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_reader1.TabIndex = 14;
			this.cmb_reader2.DisplayMember = "Text";
			this.cmb_reader2.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_reader2.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_reader2.FormattingEnabled = true;
			this.cmb_reader2.ItemHeight = 15;
			this.cmb_reader2.Location = new Point(184, 33);
			this.cmb_reader2.Name = "cmb_reader2";
			this.cmb_reader2.Size = new Size(160, 21);
			this.cmb_reader2.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_reader2.TabIndex = 15;
			this.lab_reader1.BackColor = Color.Transparent;
			this.lab_reader1.Location = new Point(7, 8);
			this.lab_reader1.Name = "lab_reader1";
			this.lab_reader1.Size = new Size(178, 13);
			this.lab_reader1.TabIndex = 84;
			this.lab_reader1.Text = "读头1出入状态";
			this.lab_reader1.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_reader2.BackColor = Color.Transparent;
			this.lab_reader2.Location = new Point(7, 37);
			this.lab_reader2.Name = "lab_reader2";
			this.lab_reader2.Size = new Size(178, 13);
			this.lab_reader2.TabIndex = 82;
			this.lab_reader2.Text = "读头2出入状态";
			this.lab_reader2.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_wiegand_set.AutoSize = true;
			this.btn_wiegand_set.Location = new Point(191, 313);
			this.btn_wiegand_set.Name = "btn_wiegand_set";
			this.btn_wiegand_set.Size = new Size(31, 13);
			this.btn_wiegand_set.TabIndex = 103;
			this.btn_wiegand_set.TabStop = true;
			this.btn_wiegand_set.Text = "设置";
			this.btn_wiegand_set.LinkClicked += this.btn_wiegandIn_set_LinkClicked;
			this.linkLabel_wiegand_fmt.AutoSize = true;
			this.linkLabel_wiegand_fmt.Location = new Point(296, 313);
			this.linkLabel_wiegand_fmt.Name = "linkLabel_wiegand_fmt";
			this.linkLabel_wiegand_fmt.Size = new Size(55, 13);
			this.linkLabel_wiegand_fmt.TabIndex = 105;
			this.linkLabel_wiegand_fmt.TabStop = true;
			this.linkLabel_wiegand_fmt.Text = "格式定义";
			this.linkLabel_wiegand_fmt.Visible = false;
			this.linkLabel_wiegand_fmt.LinkClicked += this.linkLabel_wiegand_fmt_LinkClicked;
			this.lbl_wigand.BackColor = Color.Transparent;
			this.lbl_wigand.Location = new Point(16, 313);
			this.lbl_wigand.Name = "lbl_wigand";
			this.lbl_wigand.Size = new Size(169, 13);
			this.lbl_wigand.TabIndex = 106;
			this.lbl_wigand.Text = "韦根";
			this.lbl_wigand.TextAlign = ContentAlignment.MiddleLeft;
			this.gop_Reader1IOState.Controls.Add(this.cmb_slave);
			this.gop_Reader1IOState.Controls.Add(this.cmb_master);
			this.gop_Reader1IOState.Controls.Add(this.cmb_reader1);
			this.gop_Reader1IOState.Controls.Add(this.lab_master);
			this.gop_Reader1IOState.Controls.Add(this.lab_reader1);
			this.gop_Reader1IOState.Controls.Add(this.lab_slave);
			this.gop_Reader1IOState.Controls.Add(this.cmb_reader2);
			this.gop_Reader1IOState.Controls.Add(this.lab_reader2);
			this.gop_Reader1IOState.Location = new Point(8, 184);
			this.gop_Reader1IOState.Name = "gop_Reader1IOState";
			this.gop_Reader1IOState.Size = new Size(358, 63);
			this.gop_Reader1IOState.TabIndex = 107;
			this.numErrTimes.Location = new Point(180, 3);
			this.numErrTimes.Maximum = new decimal(new int[4]
			{
				9,
				0,
				0,
				0
			});
			this.numErrTimes.Name = "numErrTimes";
			this.numErrTimes.Size = new Size(50, 20);
			this.numErrTimes.TabIndex = 108;
			this.label9.BackColor = Color.Transparent;
			this.label9.Location = new Point(3, 8);
			this.label9.Name = "label9";
			this.label9.Size = new Size(171, 13);
			this.label9.TabIndex = 109;
			this.label9.Text = "错按报警次数";
			this.label9.TextAlign = ContentAlignment.MiddleLeft;
			this.label10.BackColor = Color.Transparent;
			this.label10.Location = new Point(3, 37);
			this.label10.Name = "label10";
			this.label10.Size = new Size(171, 13);
			this.label10.TabIndex = 111;
			this.label10.Text = "门磁延时报警";
			this.label10.TextAlign = ContentAlignment.MiddleLeft;
			this.numSenserAlarmTime.Location = new Point(180, 33);
			this.numSenserAlarmTime.Maximum = new decimal(new int[4]
			{
				99,
				0,
				0,
				0
			});
			this.numSenserAlarmTime.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numSenserAlarmTime.Name = "numSenserAlarmTime";
			this.numSenserAlarmTime.Size = new Size(50, 20);
			this.numSenserAlarmTime.TabIndex = 110;
			this.numSenserAlarmTime.Value = new decimal(new int[4]
			{
				30,
				0,
				0,
				0
			});
			this.numLockDriveTime.Location = new Point(572, 129);
			this.numLockDriveTime.Maximum = new decimal(new int[4]
			{
				254,
				0,
				0,
				0
			});
			this.numLockDriveTime.Name = "numLockDriveTime";
			this.numLockDriveTime.Size = new Size(50, 20);
			this.numLockDriveTime.TabIndex = 112;
			this.numLockDriveTime.Value = new decimal(new int[4]
			{
				10,
				0,
				0,
				0
			});
			this.numPunchInterval.Location = new Point(572, 158);
			this.numPunchInterval.Maximum = new decimal(new int[4]
			{
				10,
				0,
				0,
				0
			});
			this.numPunchInterval.Name = "numPunchInterval";
			this.numPunchInterval.Size = new Size(50, 20);
			this.numPunchInterval.TabIndex = 113;
			this.numPunchInterval.Value = new decimal(new int[4]
			{
				2,
				0,
				0,
				0
			});
			this.numSensorDelayTime.Location = new Point(180, 3);
			this.numSensorDelayTime.Maximum = new decimal(new int[4]
			{
				254,
				0,
				0,
				0
			});
			this.numSensorDelayTime.Name = "numSensorDelayTime";
			this.numSensorDelayTime.Size = new Size(50, 20);
			this.numSensorDelayTime.TabIndex = 114;
			this.numSensorDelayTime.Value = new decimal(new int[4]
			{
				30,
				0,
				0,
				0
			});
			this.ckbSRBOn.BackColor = Color.Transparent;
			this.ckbSRBOn.CheckAlign = ContentAlignment.MiddleRight;
			this.ckbSRBOn.FlatAppearance.BorderSize = 2;
			this.ckbSRBOn.FlatAppearance.CheckedBackColor = Color.FromArgb(255, 128, 0);
			this.ckbSRBOn.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 128, 0);
			this.ckbSRBOn.Location = new Point(1, 62);
			this.ckbSRBOn.Name = "ckbSRBOn";
			this.ckbSRBOn.Size = new Size(193, 20);
			this.ckbSRBOn.TabIndex = 115;
			this.ckbSRBOn.Text = "启用SRB";
			this.ckbSRBOn.TextAlign = ContentAlignment.BottomLeft;
			this.ckbSRBOn.UseVisualStyleBackColor = false;
			this.pnlSensorType.Controls.Add(this.numSensorDelayTime);
			this.pnlSensorType.Controls.Add(this.lblUnitSensorDelay);
			this.pnlSensorType.Controls.Add(this.ch_closeAndLock);
			this.pnlSensorType.Controls.Add(this.lbl_doorStatus);
			this.pnlSensorType.Location = new Point(392, 38);
			this.pnlSensorType.Name = "pnlSensorType";
			this.pnlSensorType.Size = new Size(353, 56);
			this.pnlSensorType.TabIndex = 116;
			this.lblUnitSensorDelay.AutoSize = true;
			this.lblUnitSensorDelay.Location = new Point(236, 8);
			this.lblUnitSensorDelay.Name = "lblUnitSensorDelay";
			this.lblUnitSensorDelay.Size = new Size(52, 13);
			this.lblUnitSensorDelay.TabIndex = 119;
			this.lblUnitSensorDelay.Text = "秒(0-254)";
			this.pnlStdParam.Controls.Add(this.numSenserAlarmTime);
			this.pnlStdParam.Controls.Add(this.numErrTimes);
			this.pnlStdParam.Controls.Add(this.lblSensorAlarmTime);
			this.pnlStdParam.Controls.Add(this.label9);
			this.pnlStdParam.Controls.Add(this.ckbSRBOn);
			this.pnlStdParam.Controls.Add(this.label10);
			this.pnlStdParam.Location = new Point(3, 29);
			this.pnlStdParam.Name = "pnlStdParam";
			this.pnlStdParam.Size = new Size(353, 90);
			this.pnlStdParam.TabIndex = 117;
			this.lblSensorAlarmTime.AutoSize = true;
			this.lblSensorAlarmTime.Location = new Point(237, 37);
			this.lblSensorAlarmTime.Name = "lblSensorAlarmTime";
			this.lblSensorAlarmTime.Size = new Size(46, 13);
			this.lblSensorAlarmTime.TabIndex = 119;
			this.lblSensorAlarmTime.Text = "秒(1-99)";
			this.lblUnitLockDrivingTime.AutoSize = true;
			this.lblUnitLockDrivingTime.Location = new Point(628, 133);
			this.lblUnitLockDrivingTime.Name = "lblUnitLockDrivingTime";
			this.lblUnitLockDrivingTime.Size = new Size(52, 13);
			this.lblUnitLockDrivingTime.TabIndex = 118;
			this.lblUnitLockDrivingTime.Text = "秒(0-254)";
			this.lblPunchInterval.AutoSize = true;
			this.lblPunchInterval.Location = new Point(628, 163);
			this.lblPunchInterval.Name = "lblPunchInterval";
			this.lblPunchInterval.Size = new Size(46, 13);
			this.lblPunchInterval.TabIndex = 120;
			this.lblPunchInterval.Text = "秒(0-10)";
			this.flowLayoutPanel1.Controls.Add(this.ckbTwoPunchCardClose);
			this.flowLayoutPanel1.Controls.Add(this.pnlStdParam);
			this.flowLayoutPanel1.Location = new Point(389, 184);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new Size(359, 140);
			this.flowLayoutPanel1.TabIndex = 121;
			this.ckbTwoPunchCardClose.BackColor = Color.Transparent;
			this.ckbTwoPunchCardClose.CheckAlign = ContentAlignment.MiddleRight;
			this.ckbTwoPunchCardClose.FlatAppearance.BorderSize = 2;
			this.ckbTwoPunchCardClose.FlatAppearance.CheckedBackColor = Color.FromArgb(255, 128, 0);
			this.ckbTwoPunchCardClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 128, 0);
			this.ckbTwoPunchCardClose.Location = new Point(3, 3);
			this.ckbTwoPunchCardClose.Name = "ckbTwoPunchCardClose";
			this.ckbTwoPunchCardClose.Size = new Size(193, 20);
			this.ckbTwoPunchCardClose.TabIndex = 118;
			this.ckbTwoPunchCardClose.Text = "二次刷卡开/关门";
			this.ckbTwoPunchCardClose.TextAlign = ContentAlignment.BottomLeft;
			this.ckbTwoPunchCardClose.UseVisualStyleBackColor = false;
			this.ckbTwoPunchCardClose.Visible = false;
			this.chk_cardbox.AutoSize = true;
			this.chk_cardbox.Location = new Point(16, 395);
			this.chk_cardbox.Name = "chk_cardbox";
			this.chk_cardbox.RightToLeft = RightToLeft.Yes;
			this.chk_cardbox.Size = new Size(190, 17);
			this.chk_cardbox.TabIndex = 122;
			this.chk_cardbox.Text = "Funcionar como coletor de cartões";
			this.chk_cardbox.UseVisualStyleBackColor = true;
			this.chk_cardbox.Visible = false;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(194, 217, 247);
			base.ClientSize = new Size(758, 436);
			base.Controls.Add(this.chk_cardbox);
			base.Controls.Add(this.flowLayoutPanel1);
			base.Controls.Add(this.numPunchInterval);
			base.Controls.Add(this.numLockDriveTime);
			base.Controls.Add(this.cmb_doorSensor);
			base.Controls.Add(this.cmb_verifyMode);
			base.Controls.Add(this.cmb_normalOpen);
			base.Controls.Add(this.cmb_doorActive);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.lab_punchInterval);
			base.Controls.Add(this.txt_doorNO);
			base.Controls.Add(this.lblPunchInterval);
			base.Controls.Add(this.txt_devName);
			base.Controls.Add(this.lblUnitLockDrivingTime);
			base.Controls.Add(this.pnlSensorType);
			base.Controls.Add(this.gop_Reader1IOState);
			base.Controls.Add(this.chk_isAtt);
			base.Controls.Add(this.lbl_wigand);
			base.Controls.Add(this.linkLabel_wiegand_fmt);
			base.Controls.Add(this.btn_wiegand_set);
			base.Controls.Add(this.chk_allAccess);
			base.Controls.Add(this.chk_currentAccess);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.lbl_urgentPwd);
			base.Controls.Add(this.btn_seetingPwd);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lab_emergencyPassword);
			base.Controls.Add(this.lab_duress);
			base.Controls.Add(this.lab_verifyMode);
			base.Controls.Add(this.lab_doorSensor);
			base.Controls.Add(this.lab_lockDuration);
			base.Controls.Add(this.lab_normalOpen);
			base.Controls.Add(this.lab_doorActive);
			base.Controls.Add(this.lab_doorName);
			base.Controls.Add(this.lab_doorNO);
			base.Controls.Add(this.lab_deviceName);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "doorSettingForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "编辑";
			this.gop_Reader1IOState.ResumeLayout(false);
			((ISupportInitialize)this.numErrTimes).EndInit();
			((ISupportInitialize)this.numSenserAlarmTime).EndInit();
			((ISupportInitialize)this.numLockDriveTime).EndInit();
			((ISupportInitialize)this.numPunchInterval).EndInit();
			((ISupportInitialize)this.numSensorDelayTime).EndInit();
			this.pnlSensorType.ResumeLayout(false);
			this.pnlSensorType.PerformLayout();
			this.pnlStdParam.ResumeLayout(false);
			this.pnlStdParam.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
