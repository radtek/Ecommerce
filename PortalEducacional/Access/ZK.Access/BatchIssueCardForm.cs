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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class BatchIssueCardForm : Office2007Form
	{
		private int btnRead = 0;

		private int countRow = 0;

		private int count = 0;

		private int countIssue = 0;

		private string selectcountValue;

		private string cardcountValue = "0";

		private DeviceServers dserver = DeviceServers.Instance;

		private ObjDevice modelComm = new ObjDevice();

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private DataTable m_dataTable = new DataTable();

		private DataTable m_browDataTable = new DataTable();

		private IDeviceMonitor DevMonitor;

		private bool m_islock = false;

		private WaitForm m_wait = WaitForm.Instance;

		private List<AccDoor> m_selectedDoor = null;

		private IContainer components = null;

		private Label lbl_selectCount;

		private Label lbl_cardcount;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		private GridControl grd_view;

		private GridView gridView1;

		private GridColumn column_Badgenumber;

		private GridColumn column_name;

		private GridColumn column_deptName;

		private GridControl grd_brow;

		private GridView grd_mainView;

		private GridColumn column2_Badgenumber;

		private GridColumn column2_name;

		private GridColumn column2_deptName;

		private GridColumn column2_card;

		private TextBox txt_cardNO;

		private RadioButton RBtn_door;

		private Label lbl_cardNO;

		private RadioButton RBtn_card;

		private Label lbl_cardPosition;

		private ComboBoxEx cbo_cardPosition;

		private ButtonX btn_cardOk;

		private ButtonX btn_read;

		private PictureBox pic_Readding;

		private GridColumn column_lastName;

		private GridColumn column_lastnameex;

		private GridColumn column_gender;

		private GridColumn column2_gender;

		private Label lbl_start;

		private Label lbl_end;

		private TextBox txt_start;

		private TextBox txt_end;

		private ButtonX btn_produce;

		public event EventHandler RefreshDataEvent = null;

		public BatchIssueCardForm()
		{
			try
			{
				this.InitializeComponent();
				if (initLang.Lang == "chs")
				{
					this.column_lastName.Visible = false;
					this.column_lastnameex.Visible = false;
				}
				this.InitDataTableSet();
				this.InitBrowDataTableSet();
				this.GenderType();
				initLang.LocaleForm(this, base.Name);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void GenderType()
		{
			this.m_gender = initLang.GetComboxInfo("gender");
			if (this.m_gender == null || this.m_gender.Count == 0)
			{
				this.m_gender = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("M", "男");
				dictionary.Add("F", "女");
				this.m_gender.Add("0", dictionary);
				initLang.SetComboxInfo("gender", this.m_gender);
				initLang.Save();
			}
		}

		private void InitDataTableSet()
		{
			this.m_dataTable.Columns.Add("userid");
			this.m_dataTable.Columns.Add("Badgenumber").DataType = typeof(int);
			this.m_dataTable.Columns.Add("Name");
			this.m_dataTable.Columns.Add("lastname");
			this.m_dataTable.Columns.Add("Gender");
			this.m_dataTable.Columns.Add("DEPTNAME");
			this.column_Badgenumber.FieldName = "Badgenumber";
			this.column_name.FieldName = "Name";
			this.column_lastName.FieldName = "lastname";
			this.column_gender.FieldName = "Gender";
			this.column_deptName.FieldName = "DEPTNAME";
			this.grd_view.DataSource = this.m_dataTable;
		}

		private void InitBrowDataTableSet()
		{
			this.m_browDataTable.Columns.Add("userid");
			this.m_browDataTable.Columns.Add("Badgenumber");
			this.m_browDataTable.Columns.Add("Name");
			this.m_browDataTable.Columns.Add("lastname");
			this.m_browDataTable.Columns.Add("Gender");
			this.m_browDataTable.Columns.Add("DEPTNAME");
			this.m_browDataTable.Columns.Add("cardNO");
			this.column2_Badgenumber.FieldName = "Badgenumber";
			this.column2_name.FieldName = "Name";
			this.column_lastnameex.FieldName = "lastname";
			this.column2_gender.FieldName = "Gender";
			this.column2_deptName.FieldName = "DEPTNAME";
			this.column2_card.FieldName = "cardNO";
			this.grd_brow.DataSource = this.m_browDataTable;
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			if (this.dserver.Count > 0)
			{
				DeviceServerBll deviceServerBll = this.dserver[0];
				deviceServerBll.RTLogEvent -= this.server_RTLogEvent;
			}
			base.Close();
		}

		private void DataBind()
		{
			try
			{
				this.m_dataTable.Rows.Clear();
				this.countRow = 0;
				this.selectcountValue = "0";
				string value = string.Empty;
				string value2 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					value = dictionary["m"];
					value2 = dictionary["f"];
				}
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				string str = (!string.IsNullOrEmpty(this.txt_start.Text)) ? this.txt_start.Text : "0";
				string str2 = (!string.IsNullOrEmpty(this.txt_end.Text)) ? this.txt_end.Text : "0";
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("data");
				DataSet dataSet = null;
				if (nodeValueByName.ToLower() == "sqlserver")
				{
					dataSet = userInfoBll.GetList("USERID>0 and  ((CardNo='') or (CardNo is null)) and convert(int,Badgenumber)>=" + str + " and convert(int,Badgenumber)<=" + str2);
				}
				else if (nodeValueByName.ToLower() == "access")
				{
					dataSet = userInfoBll.GetList("USERID>0 and  ((CardNo='') or (CardNo is null)) and  clng(Badgenumber)>=" + str + " and clng(Badgenumber)<=" + str2);
				}
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_dataTable.NewRow();
						dataRow[0] = dataTable.Rows[i]["userid"].ToString();
						dataRow[1] = dataTable.Rows[i]["Badgenumber"].ToString();
						dataRow[2] = dataTable.Rows[i]["Name"].ToString();
						dataRow[3] = dataTable.Rows[i]["lastname"].ToString();
						if (dataTable.Rows[i]["Gender"].ToString() == "F")
						{
							dataRow[4] = value2;
						}
						else
						{
							dataRow[4] = value;
						}
						dataRow[5] = dataTable.Rows[i]["DEPTNAME"].ToString();
						this.m_dataTable.Rows.Add(dataRow);
					}
					this.countRow = this.m_dataTable.Rows.Count;
					this.count = this.m_dataTable.Rows.Count;
					this.selectcountValue = this.m_dataTable.Rows.Count.ToString();
					this.lbl_selectCount.Text = ShowMsgInfos.GetInfo("SselectCount", "未分配卡人员数量") + " " + this.selectcountValue;
					this.lbl_cardcount.Text = ShowMsgInfos.GetInfo("Scardcount", "已发卡数") + " " + this.cardcountValue;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_read_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.btnRead == 0)
				{
					if (!string.IsNullOrEmpty(this.cbo_cardPosition.Text))
					{
						this.btn_read.Text = ShowMsgInfos.GetInfo("SStopReadingCardNo", "停止读取");
						this.pic_Readding.Visible = true;
						this.btnRead = 1;
						this.cbo_cardPosition.Enabled = false;
						if (this.dserver.Count > 0)
						{
							bool flag = false;
							for (int i = 0; i < this.dserver.Count; i++)
							{
								DeviceServerBll deviceServerBll = this.dserver[i];
								if (this.m_selectedDoor != null && this.m_selectedDoor.Count > 0)
								{
									bool flag2 = false;
									int num = 0;
									while (num < this.m_selectedDoor.Count)
									{
										if (this.m_selectedDoor[num].device_id != deviceServerBll.DevInfo.ID)
										{
											num++;
											continue;
										}
										flag2 = true;
										break;
									}
									if (flag2)
									{
										if (deviceServerBll.IsConnected)
										{
											goto IL_0172;
										}
										int num2 = deviceServerBll.Connect(3000);
										if (num2 >= 0)
										{
											goto IL_0172;
										}
									}
								}
								continue;
								IL_0172:
								deviceServerBll.IsNeedListen = true;
								flag = true;
								SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ReadCardNoSwippingMemo", "对于Pull设备，刷卡时，仅读取设备中未注册的卡！对于脱机设备，会读取所有卡！"));
								this.RBtn_card.Enabled = false;
								if (deviceServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
								{
									deviceServerBll.SwippingCard += this.server_RTLogEvent;
									this.DevMonitor = new StdDeviceMonitor(0);
									this.DevMonitor.AddDeviceServer(deviceServerBll);
									this.DevMonitor.StartMonitor();
								}
								else
								{
									deviceServerBll.CardNoRegEvent += this.server_RTLogEvent;
									this.DevMonitor = new PullDeviceMonitor(0);
									this.DevMonitor.AddDeviceServer(deviceServerBll);
									this.DevMonitor.StartMonitor();
								}
							}
							if (!flag)
							{
								this.pic_Readding.Visible = false;
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败"));
								this.btn_read.Text = ShowMsgInfos.GetInfo("SStartReadingCardNo", "开始读取");
								this.cbo_cardPosition.Enabled = true;
								this.btnRead = 0;
							}
						}
						else
						{
							this.pic_Readding.Visible = false;
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败"));
							this.btn_read.Text = ShowMsgInfos.GetInfo("SStartReadingCardNo", "开始读取");
							this.cbo_cardPosition.Enabled = true;
							this.btnRead = 0;
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SPositionSwipingCard", "请选择刷卡位置!"));
						this.cbo_cardPosition.Focus();
					}
				}
				else
				{
					this.cbo_cardPosition.Enabled = true;
					this.StopServerRead();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void StopServerRead()
		{
			try
			{
				if (this.dserver.Count > 0)
				{
					for (int i = 0; i < this.dserver.Count; i++)
					{
						DeviceServerBll deviceServerBll = this.dserver[i];
						deviceServerBll.CardNoRegEvent -= this.server_RTLogEvent;
						deviceServerBll.SwippingCard -= this.server_RTLogEvent;
					}
				}
				if (this.DevMonitor != null)
				{
					this.DevMonitor.StopMonitor();
					this.DevMonitor.ClearDeviceServer();
					this.DevMonitor = null;
				}
				this.btn_read.Text = ShowMsgInfos.GetInfo("SStartReadingCardNo", "开始读取");
				this.pic_Readding.Visible = false;
				this.RBtn_card.Enabled = true;
				this.btnRead = 0;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void RTLogEvent(ObjRTLogInfo info)
		{
			try
			{
				if (!this.m_islock)
				{
					this.m_islock = true;
					bool flag = false;
					PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
					if (info != null && (info.EType == EventType.Type27 || info.EType == EventType.Type1027) && this.m_selectedDoor != null)
					{
						for (int i = 0; i < this.m_selectedDoor.Count; i++)
						{
							if (info.DoorID == this.m_selectedDoor[i].door_no.ToString() && info.DevID == this.m_selectedDoor[i].device_id && !string.IsNullOrEmpty(info.CardNo))
							{
								int num = 0;
								while (num < this.m_browDataTable.Rows.Count)
								{
									if (this.m_browDataTable.Rows[num][0] == null || !(info.CardNo == this.m_browDataTable.Rows[num][6].ToString()))
									{
										num++;
										continue;
									}
									flag = true;
									break;
								}
								if (!flag)
								{
									if (this.txt_cardNO.Text != "0")
									{
										if (this.m_dataTable.Rows.Count > 0)
										{
											DataRow dataRow = this.gridView1.GetDataRow(0);
											if (dataRow != null)
											{
												ulong.TryParse(info.CardNo, out ulong num2);
												DataRow dataRow2 = this.m_browDataTable.NewRow();
												if (!personnelIssuecardBll.ExistsCard(info.CardNo))
												{
													dataRow2[0] = dataRow[0];
													dataRow2[1] = dataRow[1];
													dataRow2[2] = dataRow[2];
													dataRow2[3] = dataRow[3];
													dataRow2[4] = dataRow[4];
													dataRow2[5] = dataRow[5];
													CodeVersionType codeVersion = AccCommon.CodeVersion;
													if (codeVersion == CodeVersionType.JapanAF)
													{
														dataRow2[6] = num2.ToString("X");
													}
													else
													{
														dataRow2[6] = info.CardNo;
													}
													this.m_browDataTable.Rows.Add(dataRow2);
													this.m_dataTable.Rows.Remove(dataRow);
													this.countIssue++;
													this.countRow--;
													this.selectcountValue = this.countRow.ToString();
													this.lbl_selectCount.Text = ShowMsgInfos.GetInfo("SselectCount", "未分配数") + " " + this.selectcountValue;
													this.cardcountValue = this.countIssue.ToString();
													this.lbl_cardcount.Text = ShowMsgInfos.GetInfo("Scardcount", "已发卡数") + " " + this.cardcountValue;
												}
												else
												{
													SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardRepeat", "卡号[{0}]已存在").Replace("{0}", info.CardNo));
												}
											}
											else
											{
												SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personsNull", "没有需要发卡的人员!"));
											}
										}
										else
										{
											SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personsNull", "没有需要发卡的人员!"));
										}
									}
									else
									{
										SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInvalidCard", "无效卡！"));
									}
								}
								else
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardRepeat", "卡号[{0}]已存在").Replace("{0}", info.CardNo));
								}
							}
						}
					}
					this.m_islock = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void server_RTLogEvent(ObjDevice sender, ObjRTLogInfo info)
		{
			if (!base.IsDisposed && base.Visible)
			{
				if (base.InvokeRequired)
				{
					base.BeginInvoke((MethodInvoker)delegate
					{
						this.server_RTLogEvent(sender, info);
					});
				}
				else
				{
					this.RTLogEvent(info);
				}
			}
		}

		private void btn_cardok_Click(object sender, EventArgs e)
		{
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				if (!string.IsNullOrEmpty(this.txt_cardNO.Text))
				{
					if (!personnelIssuecardBll.ExistsCard(this.txt_cardNO.Text))
					{
						bool flag = false;
						int num = 0;
						while (num < this.m_browDataTable.Rows.Count)
						{
							if (this.m_browDataTable.Rows[num][0] == null || !(this.txt_cardNO.Text == this.m_browDataTable.Rows[num][6].ToString()))
							{
								num++;
								continue;
							}
							flag = true;
							break;
						}
						if (!flag)
						{
							if (this.txt_cardNO.Text != "0")
							{
								if (this.gridView1.RowCount > 0)
								{
									DataRow dataRow = this.gridView1.GetDataRow(0);
									if (dataRow != null)
									{
										ulong.TryParse(this.txt_cardNO.Text, out ulong num2);
										DataRow dataRow2 = this.m_browDataTable.NewRow();
										dataRow2[0] = dataRow[0];
										dataRow2[1] = dataRow[1];
										dataRow2[2] = dataRow[2];
										dataRow2[3] = dataRow[3];
										dataRow2[4] = dataRow[4];
										dataRow2[5] = dataRow[5];
										CodeVersionType codeVersion = AccCommon.CodeVersion;
										if (codeVersion == CodeVersionType.JapanAF)
										{
											dataRow2[6] = num2.ToString("X");
										}
										else
										{
											dataRow2[6] = this.txt_cardNO.Text;
										}
										this.m_browDataTable.Rows.Add(dataRow2);
										this.m_dataTable.Rows.Remove(dataRow);
										this.countIssue++;
										this.countRow--;
										this.selectcountValue = this.countRow.ToString();
										this.lbl_selectCount.Text = ShowMsgInfos.GetInfo("SselectCount", "未分配数") + " " + this.selectcountValue;
										this.cardcountValue = this.countIssue.ToString();
										this.lbl_cardcount.Text = ShowMsgInfos.GetInfo("Scardcount", "已发卡数") + " " + this.cardcountValue;
										this.txt_cardNO.Text = "";
									}
									else
									{
										SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personsNull", "没有需要发卡的人员!"));
									}
								}
								else
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personsNull", "没有需要发卡的人员!"));
								}
							}
							else
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInvalidCard", "无效卡！"));
							}
						}
						else
						{
							string text = this.txt_cardNO.Text;
							this.txt_cardNO.Text = "";
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardRepeat", "卡号[{0}]已存在").Replace("{0}", text));
							this.txt_cardNO.Focus();
						}
					}
					else
					{
						string text2 = this.txt_cardNO.Text;
						this.txt_cardNO.Text = "";
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCardRepeat", "卡号[{0}]已存在").Replace("{0}", text2));
						this.txt_cardNO.Focus();
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("cardNull", "卡号不能为空!"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			bool flag = false;
			bool flag2 = false;
			try
			{
				if (this.m_browDataTable.Rows.Count > 0)
				{
					this.m_wait.ShowEx();
					PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					List<UserInfo> list = new List<UserInfo>();
					for (int i = 0; i < this.m_browDataTable.Rows.Count; i++)
					{
						this.m_wait.ShowProgress(i * 100 / this.m_browDataTable.Rows.Count);
						if (this.m_browDataTable.Rows[i][0] != null)
						{
							int userID_id = int.Parse(this.m_browDataTable.Rows[i][0].ToString());
							PersonnelIssuecard personnelIssuecard = new PersonnelIssuecard();
							personnelIssuecard.UserID_id = userID_id;
							personnelIssuecard.cardno = this.m_browDataTable.Rows[i][6].ToString();
							personnelIssuecard.create_time = DateTime.Now;
							personnelIssuecardBll.Add(personnelIssuecard);
							UserInfo model = userInfoBll.GetModel(personnelIssuecard.UserID_id);
							if (model != null)
							{
								model.CardNo = personnelIssuecard.cardno;
								userInfoBll.Update(model);
								if (CommandServer.AddCmd(model, false, ""))
								{
									flag = true;
								}
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + model.BadgeNumber + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
							}
						}
					}
					if (flag)
					{
						FrmShowUpdata.Instance.ShowEx();
					}
					if (this.RefreshDataEvent != null)
					{
						this.RefreshDataEvent(this, null);
					}
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
					base.Close();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("personsNull", "没有需要发卡的用户!"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void cbo_cardPosition_Click(object sender, EventArgs e)
		{
			this.cbo_cardPosition.Items.Clear();
			this.cbo_cardPosition.Text = "";
			this.btn_read.Enabled = false;
			DeviceTreeForm deviceTreeForm = new DeviceTreeForm(true, "");
			deviceTreeForm.lb_info.Visible = true;
			deviceTreeForm.lb_info.Text = ShowMsgInfos.GetInfo("SOnlyReadUnregistered", "刷卡时，仅读取设备中未注册的卡！");
			deviceTreeForm.SelectDeviceEvent += this.devic_Event;
			deviceTreeForm.ShowDialog();
			deviceTreeForm.SelectDeviceEvent -= this.devic_Event;
		}

		private void devic_Event(object sender, EventArgs e)
		{
			if (sender != null)
			{
				List<AccDoor> list = this.m_selectedDoor = (sender as List<AccDoor>);
				if (list != null)
				{
					this.cbo_cardPosition.Text = "";
					this.btn_read.Enabled = true;
					for (int i = 0; i < list.Count; i++)
					{
						this.cbo_cardPosition.Items.Add(list[i].door_name + " ");
						ComboBoxEx comboBoxEx = this.cbo_cardPosition;
						comboBoxEx.Text = comboBoxEx.Text + list[i].door_name + " ";
					}
				}
			}
		}

		private void RBtn_door_CheckedChanged(object sender, EventArgs e)
		{
			if (this.RBtn_door.Checked)
			{
				this.lbl_cardNO.Enabled = false;
				this.txt_cardNO.Text = "";
				this.txt_cardNO.Enabled = false;
				this.btn_cardOk.Enabled = false;
				this.lbl_cardPosition.Enabled = true;
				this.cbo_cardPosition.Enabled = true;
				this.btn_read.Enabled = false;
			}
		}

		private void RBtn_card_CheckedChanged(object sender, EventArgs e)
		{
			if (this.RBtn_card.Checked)
			{
				this.lbl_cardNO.Enabled = true;
				this.txt_cardNO.Enabled = true;
				this.btn_cardOk.Enabled = false;
				this.lbl_cardPosition.Enabled = false;
				this.cbo_cardPosition.Enabled = false;
				this.btn_read.Enabled = false;
				this.StopServerRead();
				this.cbo_cardPosition.Text = "";
			}
		}

		private void txt_cardNO_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 9999999999L);
			if (e.KeyChar == '\r')
			{
				this.btn_cardok_Click(null, null);
			}
		}

		private void BatchIssueCardForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.StopServerRead();
		}

		private void txt_cardNO_KeyUp(object sender, KeyEventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_cardNO.Text))
			{
				this.btn_cardOk.Enabled = true;
			}
			else
			{
				this.btn_cardOk.Enabled = false;
			}
		}

		private void btn_produce_Click(object sender, EventArgs e)
		{
			this.DataBind();
		}

		private void txt_start_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 999999999L);
		}

		private void txt_end_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 999999999L);
		}

		private void BatchIssueCardForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.dserver.Count > 0)
			{
				for (int i = 0; i < this.dserver.Count; i++)
				{
					DeviceServerBll deviceServerBll = this.dserver[i];
					deviceServerBll.CardNoRegEvent -= this.server_RTLogEvent;
					deviceServerBll.SwippingCard -= this.server_RTLogEvent;
				}
			}
			if (this.DevMonitor != null)
			{
				this.DevMonitor.StopMonitor();
				this.DevMonitor.ClearDeviceServer();
				this.DevMonitor = null;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BatchIssueCardForm));
			this.lbl_selectCount = new Label();
			this.lbl_cardcount = new Label();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.grd_view = new GridControl();
			this.gridView1 = new GridView();
			this.column_Badgenumber = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_gender = new GridColumn();
			this.column_deptName = new GridColumn();
			this.grd_brow = new GridControl();
			this.grd_mainView = new GridView();
			this.column2_Badgenumber = new GridColumn();
			this.column2_name = new GridColumn();
			this.column_lastnameex = new GridColumn();
			this.column2_gender = new GridColumn();
			this.column2_deptName = new GridColumn();
			this.column2_card = new GridColumn();
			this.txt_cardNO = new TextBox();
			this.RBtn_door = new RadioButton();
			this.lbl_cardNO = new Label();
			this.RBtn_card = new RadioButton();
			this.lbl_cardPosition = new Label();
			this.cbo_cardPosition = new ComboBoxEx();
			this.btn_cardOk = new ButtonX();
			this.btn_read = new ButtonX();
			this.pic_Readding = new PictureBox();
			this.lbl_start = new Label();
			this.lbl_end = new Label();
			this.txt_start = new TextBox();
			this.txt_end = new TextBox();
			this.btn_produce = new ButtonX();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			((ISupportInitialize)this.grd_brow).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			((ISupportInitialize)this.pic_Readding).BeginInit();
			base.SuspendLayout();
			this.lbl_selectCount.Location = new Point(12, 104);
			this.lbl_selectCount.Name = "lbl_selectCount";
			this.lbl_selectCount.Size = new Size(352, 12);
			this.lbl_selectCount.TabIndex = 17;
			this.lbl_selectCount.Text = "未发卡人员数:";
			this.lbl_selectCount.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_cardcount.Location = new Point(520, 106);
			this.lbl_cardcount.Name = "lbl_cardcount";
			this.lbl_cardcount.Size = new Size(354, 12);
			this.lbl_cardcount.TabIndex = 19;
			this.lbl_cardcount.Text = "已发卡数:";
			this.lbl_cardcount.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(810, 439);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 9;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(904, 439);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 10;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.grd_view.Location = new Point(12, 124);
			this.grd_view.MainView = this.gridView1;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(495, 300);
			this.grd_view.TabIndex = 58;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridView1
			});
			this.gridView1.Columns.AddRange(new GridColumn[5]
			{
				this.column_Badgenumber,
				this.column_name,
				this.column_lastName,
				this.column_gender,
				this.column_deptName
			});
			this.gridView1.GridControl = this.grd_view;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.column_Badgenumber.Caption = "人员编号";
			this.column_Badgenumber.Name = "column_Badgenumber";
			this.column_Badgenumber.Visible = true;
			this.column_Badgenumber.VisibleIndex = 0;
			this.column_Badgenumber.Width = 121;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 1;
			this.column_name.Width = 121;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 2;
			this.column_gender.Caption = "性别";
			this.column_gender.Name = "column_gender";
			this.column_gender.Visible = true;
			this.column_gender.VisibleIndex = 3;
			this.column_deptName.Caption = "部门名称";
			this.column_deptName.Name = "column_deptName";
			this.column_deptName.Visible = true;
			this.column_deptName.VisibleIndex = 4;
			this.column_deptName.Width = 121;
			this.grd_brow.Location = new Point(522, 124);
			this.grd_brow.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_brow.MainView = this.grd_mainView;
			this.grd_brow.Name = "grd_brow";
			this.grd_brow.Size = new Size(464, 300);
			this.grd_brow.TabIndex = 61;
			this.grd_brow.TabStop = false;
			this.grd_brow.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[6]
			{
				this.column2_Badgenumber,
				this.column2_name,
				this.column_lastnameex,
				this.column2_gender,
				this.column2_deptName,
				this.column2_card
			});
			this.grd_mainView.GridControl = this.grd_brow;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsBehavior.Editable = false;
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.column2_Badgenumber.Caption = "人员编号";
			this.column2_Badgenumber.Name = "column2_Badgenumber";
			this.column2_Badgenumber.Visible = true;
			this.column2_Badgenumber.VisibleIndex = 0;
			this.column2_name.Caption = "姓名";
			this.column2_name.Name = "column2_name";
			this.column2_name.Visible = true;
			this.column2_name.VisibleIndex = 1;
			this.column_lastnameex.Caption = "姓氏";
			this.column_lastnameex.Name = "column_lastnameex";
			this.column_lastnameex.Visible = true;
			this.column_lastnameex.VisibleIndex = 2;
			this.column2_gender.Caption = "性别";
			this.column2_gender.Name = "column2_gender";
			this.column2_gender.Visible = true;
			this.column2_gender.VisibleIndex = 3;
			this.column2_deptName.Caption = "部门名称";
			this.column2_deptName.Name = "column2_deptName";
			this.column2_deptName.Visible = true;
			this.column2_deptName.VisibleIndex = 4;
			this.column2_card.Caption = "卡号";
			this.column2_card.Name = "column2_card";
			this.column2_card.Visible = true;
			this.column2_card.VisibleIndex = 5;
			this.txt_cardNO.Enabled = false;
			this.txt_cardNO.Location = new Point(435, 42);
			this.txt_cardNO.Name = "txt_cardNO";
			this.txt_cardNO.Size = new Size(109, 21);
			this.txt_cardNO.TabIndex = 4;
			this.txt_cardNO.KeyPress += this.txt_cardNO_KeyPress;
			this.txt_cardNO.KeyUp += this.txt_cardNO_KeyUp;
			this.RBtn_door.AutoSize = true;
			this.RBtn_door.BackColor = Color.Transparent;
			this.RBtn_door.Checked = true;
			this.RBtn_door.Location = new Point(12, 15);
			this.RBtn_door.Name = "RBtn_door";
			this.RBtn_door.Size = new Size(107, 16);
			this.RBtn_door.TabIndex = 0;
			this.RBtn_door.TabStop = true;
			this.RBtn_door.Text = "门禁控制器发卡";
			this.RBtn_door.UseVisualStyleBackColor = false;
			this.RBtn_door.CheckedChanged += this.RBtn_door_CheckedChanged;
			this.lbl_cardNO.BackColor = Color.Transparent;
			this.lbl_cardNO.Enabled = false;
			this.lbl_cardNO.Location = new Point(181, 47);
			this.lbl_cardNO.Name = "lbl_cardNO";
			this.lbl_cardNO.Size = new Size(247, 12);
			this.lbl_cardNO.TabIndex = 66;
			this.lbl_cardNO.Text = "输入卡号";
			this.lbl_cardNO.TextAlign = ContentAlignment.MiddleRight;
			this.RBtn_card.AutoSize = true;
			this.RBtn_card.BackColor = Color.Transparent;
			this.RBtn_card.Location = new Point(12, 45);
			this.RBtn_card.Name = "RBtn_card";
			this.RBtn_card.Size = new Size(83, 16);
			this.RBtn_card.TabIndex = 3;
			this.RBtn_card.TabStop = true;
			this.RBtn_card.Text = "发卡器发卡";
			this.RBtn_card.UseVisualStyleBackColor = false;
			this.RBtn_card.CheckedChanged += this.RBtn_card_CheckedChanged;
			this.lbl_cardPosition.BackColor = Color.Transparent;
			this.lbl_cardPosition.Location = new Point(179, 17);
			this.lbl_cardPosition.Name = "lbl_cardPosition";
			this.lbl_cardPosition.Size = new Size(249, 12);
			this.lbl_cardPosition.TabIndex = 67;
			this.lbl_cardPosition.Text = "刷卡位置";
			this.lbl_cardPosition.TextAlign = ContentAlignment.MiddleRight;
			this.cbo_cardPosition.DisplayMember = "Text";
			this.cbo_cardPosition.DrawMode = DrawMode.OwnerDrawFixed;
			this.cbo_cardPosition.FormattingEnabled = true;
			this.cbo_cardPosition.ItemHeight = 15;
			this.cbo_cardPosition.Location = new Point(435, 11);
			this.cbo_cardPosition.Name = "cbo_cardPosition";
			this.cbo_cardPosition.Size = new Size(109, 21);
			this.cbo_cardPosition.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cbo_cardPosition.TabIndex = 1;
			this.cbo_cardPosition.Click += this.cbo_cardPosition_Click;
			this.btn_cardOk.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cardOk.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cardOk.Enabled = false;
			this.btn_cardOk.Location = new Point(559, 42);
			this.btn_cardOk.Name = "btn_cardOk";
			this.btn_cardOk.Size = new Size(164, 23);
			this.btn_cardOk.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cardOk.TabIndex = 5;
			this.btn_cardOk.Text = "确定";
			this.btn_cardOk.Click += this.btn_cardok_Click;
			this.btn_read.AccessibleRole = AccessibleRole.PushButton;
			this.btn_read.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_read.Enabled = false;
			this.btn_read.Location = new Point(559, 11);
			this.btn_read.Name = "btn_read";
			this.btn_read.Size = new Size(164, 23);
			this.btn_read.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_read.TabIndex = 2;
			this.btn_read.Text = "开始读取";
			this.btn_read.Click += this.btn_read_Click;
			this.pic_Readding.BackColor = Color.Transparent;
			this.pic_Readding.Image = Resource.loadpage;
			this.pic_Readding.Location = new Point(737, 13);
			this.pic_Readding.Name = "pic_Readding";
			this.pic_Readding.Size = new Size(22, 18);
			this.pic_Readding.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_Readding.TabIndex = 70;
			this.pic_Readding.TabStop = false;
			this.pic_Readding.Visible = false;
			this.lbl_start.Location = new Point(12, 77);
			this.lbl_start.Name = "lbl_start";
			this.lbl_start.Size = new Size(151, 12);
			this.lbl_start.TabIndex = 71;
			this.lbl_start.Text = "起始人员编号";
			this.lbl_end.Location = new Point(279, 77);
			this.lbl_end.Name = "lbl_end";
			this.lbl_end.Size = new Size(149, 12);
			this.lbl_end.TabIndex = 72;
			this.lbl_end.Text = "结束人员编号";
			this.lbl_end.TextAlign = ContentAlignment.MiddleRight;
			this.txt_start.Location = new Point(169, 74);
			this.txt_start.Name = "txt_start";
			this.txt_start.Size = new Size(100, 21);
			this.txt_start.TabIndex = 6;
			this.txt_start.KeyPress += this.txt_start_KeyPress;
			this.txt_end.Location = new Point(435, 74);
			this.txt_end.Name = "txt_end";
			this.txt_end.Size = new Size(109, 21);
			this.txt_end.TabIndex = 7;
			this.txt_end.KeyPress += this.txt_end_KeyPress;
			this.btn_produce.AccessibleRole = AccessibleRole.PushButton;
			this.btn_produce.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_produce.Location = new Point(559, 71);
			this.btn_produce.Name = "btn_produce";
			this.btn_produce.Size = new Size(164, 23);
			this.btn_produce.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_produce.TabIndex = 8;
			this.btn_produce.Text = "生成人员列表";
			this.btn_produce.Click += this.btn_produce_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(998, 477);
			base.Controls.Add(this.txt_end);
			base.Controls.Add(this.txt_cardNO);
			base.Controls.Add(this.cbo_cardPosition);
			base.Controls.Add(this.txt_start);
			base.Controls.Add(this.btn_produce);
			base.Controls.Add(this.lbl_end);
			base.Controls.Add(this.lbl_start);
			base.Controls.Add(this.pic_Readding);
			base.Controls.Add(this.RBtn_door);
			base.Controls.Add(this.lbl_cardNO);
			base.Controls.Add(this.RBtn_card);
			base.Controls.Add(this.lbl_cardPosition);
			base.Controls.Add(this.btn_cardOk);
			base.Controls.Add(this.btn_read);
			base.Controls.Add(this.grd_brow);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lbl_cardcount);
			base.Controls.Add(this.lbl_selectCount);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "BatchIssueCardForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "批量发卡";
			base.FormClosing += this.BatchIssueCardForm_FormClosing;
			base.FormClosed += this.BatchIssueCardForm_FormClosed;
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			((ISupportInitialize)this.grd_brow).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			((ISupportInitialize)this.pic_Readding).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
