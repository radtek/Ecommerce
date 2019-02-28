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
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.device
{
	public class DeviceToPC : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private List<int> m_deviceSelectedTable = null;

		private DeviceHelperEx helper = new DeviceHelperEx();

		private Dictionary<string, UserInfo> dicPin_User = new Dictionary<string, UserInfo>();

		private Dictionary<int, UserInfo> dicUserId_User = new Dictionary<int, UserInfo>();

		private Dictionary<string, List<Template>> dicPin_lstFp = new Dictionary<string, List<Template>>();

		private Dictionary<string, List<FingerVein>> dicPin_lstFv = new Dictionary<string, List<FingerVein>>();

		private Dictionary<int, Dictionary<int, Dictionary<int, int>>> dicPinFaceIdFaceType_TempId = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

		private int cuur_prg = 0;

		private Thread m_thead = null;

		private bool m_isStop = false;

		private int defaultID = 0;

		private Dictionary<string, int> m_levelDic = new Dictionary<string, int>();

		private Dictionary<int, int> m_timeiddic = new Dictionary<int, int>();

		private IContainer components = null;

		private PanelEx pnl_bottom;

		private LabelX lb_progress;

		private CheckBox chk_userPrivilege;

		private CheckBox chk_User;

		private ButtonX btn_exit;

		private ButtonX btn_down;

		private ProgressBar progressBarUp;

		private TextBox txt_UpLoadInfo;

		private CheckBox chk_FP;

		private LabelX lb_title;

		private CheckBox chk_timeZone;

		private System.Windows.Forms.Timer timer1;

		private ProgressBar progress_all;

		private LabelX lb_progressAll;

		private ButtonX btn_show;

		private CheckBox chk_FV;

		private CheckBox ckb_FaceTemp;

		private Label lblProgressAll;

		private Label lblProgress;

		public DeviceToPC(List<int> machineSelectedInfo)
		{
			this.InitializeComponent();
			this.m_deviceSelectedTable = machineSelectedInfo;
			initLang.LocaleForm(this, base.Name);
			this.helper.ShowInfoEvent += this.ShowUpLoadInfo;
			this.helper.ShowProgressEvent += this.ShowProgress;
			this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
			this.btn_show_Click(null, null);
		}

		private void LoadFaceTempDic()
		{
			try
			{
				ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
				DataTable fields = faceTempBll.GetFields("TemplateId,Pin,FaceId,FaceType", "");
				for (int i = 0; i < fields.Rows.Count; i++)
				{
					DataRow dataRow = fields.Rows[i];
					int.TryParse(dataRow["TemplateId"].ToString(), out int value);
					int.TryParse(dataRow["Pin"].ToString(), out int key);
					int.TryParse(dataRow["FaceId"].ToString(), out int key2);
					int.TryParse(dataRow["FaceType"].ToString(), out int num);
					if (!this.dicPinFaceIdFaceType_TempId.ContainsKey(key))
					{
						Dictionary<int, int> dictionary = new Dictionary<int, int>();
						Dictionary<int, Dictionary<int, int>> dictionary2 = new Dictionary<int, Dictionary<int, int>>();
						dictionary.Add((num == 1) ? 1 : 0, value);
						dictionary2.Add(key2, dictionary);
						this.dicPinFaceIdFaceType_TempId.Add(key, dictionary2);
					}
					else
					{
						Dictionary<int, Dictionary<int, int>> dictionary2 = this.dicPinFaceIdFaceType_TempId[key];
						if (!dictionary2.ContainsKey(key2))
						{
							Dictionary<int, int> dictionary = new Dictionary<int, int>();
							dictionary.Add((num == 1) ? 1 : 0, value);
							dictionary2.Add(key2, dictionary);
						}
						else
						{
							Dictionary<int, int> dictionary = dictionary2[key2];
							if (!dictionary.ContainsKey(num))
							{
								dictionary.Add((num == 1) ? 1 : 0, value);
							}
							else
							{
								dictionary[num] = value;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void Loadusers()
		{
			try
			{
				this.dicPin_User.Clear();
				this.dicUserId_User.Clear();
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				List<UserInfo> modelList = userInfoBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dicPin_User.ContainsKey(modelList[i].BadgeNumber))
						{
							this.dicPin_User.Add(modelList[i].BadgeNumber, modelList[i]);
							this.dicUserId_User.Add(modelList[i].UserId, modelList[i]);
						}
					}
				}
				this.dicPin_lstFp.Clear();
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				List<Template> modelList2 = templateBll.GetModelList("");
				if (modelList2 != null)
				{
					for (int j = 0; j < modelList2.Count; j++)
					{
						if (this.dicUserId_User.ContainsKey(modelList2[j].USERID))
						{
							modelList2[j].Pin = this.dicUserId_User[modelList2[j].USERID].BadgeNumber;
							if (this.dicPin_lstFp.ContainsKey(modelList2[j].Pin))
							{
								this.dicPin_lstFp[modelList2[j].Pin].Add(modelList2[j]);
							}
							else
							{
								List<Template> list = new List<Template>();
								list.Add(modelList2[j]);
								this.dicPin_lstFp.Add(modelList2[j].Pin, list);
							}
						}
					}
				}
				this.dicPin_lstFv.Clear();
				FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
				List<FingerVein> modelList3 = fingerVeinBll.GetModelList("");
				if (modelList3 != null && modelList3.Count > 0)
				{
					for (int k = 0; k < modelList3.Count; k++)
					{
						if (this.dicUserId_User.ContainsKey(modelList3[k].UserID))
						{
							modelList3[k].Pin = this.dicUserId_User[modelList3[k].UserID].BadgeNumber;
							if (this.dicPin_lstFv.ContainsKey(modelList3[k].Pin))
							{
								this.dicPin_lstFv[modelList3[k].Pin].Add(modelList3[k]);
							}
							else
							{
								List<FingerVein> list2 = new List<FingerVein>();
								list2.Add(modelList3[k]);
								this.dicPin_lstFv.Add(modelList3[k].Pin, list2);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void ShowUpLoadInfo(string UpLoadinfoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.ShowUpLoadInfo), UpLoadinfoStr);
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
					this.cuur_prg = prg;
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
					if (prg == 0)
					{
						prg = 1;
					}
					this.progress_all.Value = prg;
					this.lblProgressAll.Text = prg.ToString() + "%";
					this.cuur_prg = prg;
					this.Refresh();
				}
			}
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
					this.btn_down.Enabled = true;
					this.timer1.Enabled = false;
					this.Cursor = Cursors.Default;
					if (!this.m_isStop)
					{
						this.Refresh();
					}
				}
			}
		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void Start()
		{
			try
			{
				if (this.m_deviceSelectedTable != null && this.m_deviceSelectedTable.Count > 0)
				{
					this.Loadusers();
					this.LoadDefaultID();
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					for (int i = 0; i < this.m_deviceSelectedTable.Count; i++)
					{
						this.ShowProgressAll(i * 100 / this.m_deviceSelectedTable.Count);
						if (this.m_isStop)
						{
							break;
						}
						Machines model = machinesBll.GetModel(this.m_deviceSelectedTable[i]);
						if (model != null)
						{
							DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
							if (deviceServer != null)
							{
								this.ShowProgress(0);
								if (deviceServer.IsConnected)
								{
									deviceServer.Disconnect();
								}
								this.ShowUpLoadInfo(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + model.MachineAlias);
								if (deviceServer.Connect(3000) < 0)
								{
									this.ShowUpLoadInfo(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败"));
									this.ShowDetailInfo();
									continue;
								}
								this.ShowUpLoadInfo(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功"));
								this.ShowUpLoadInfo(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SGetingData", "正在获取信息，请稍候..."));
								if (model.IsOnlyRFMachine == 1 && this.chk_FP.Checked)
								{
									this.ShowUpLoadInfo(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SDeviceNotSupportFP", "该设备不支持指纹"));
								}
								if (this.chk_User.Checked)
								{
									this.CheckUser(deviceServer);
									this.Loadusers();
								}
								if (this.chk_FP.Checked && model.SupportFingerprint)
								{
									this.CheckTemplate(deviceServer);
									this.Loadusers();
								}
								if (this.chk_FV.Checked && model.FvFunOn == 1)
								{
									this.CheckFingerVein(deviceServer);
									this.Loadusers();
								}
								if (this.chk_timeZone.Checked)
								{
									this.CheckTime(deviceServer);
								}
								if (this.chk_userPrivilege.Checked)
								{
									this.CheckUserAuthorize(deviceServer);
								}
								if (this.ckb_FaceTemp.Checked && (model.device_type == 103 || model.FaceFunOn == 1))
								{
									this.CheckFaceTemplate(deviceServer);
								}
								this.ShowUpLoadInfo(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SCheckingDataSuccess", "获取设备信息完成"));
								this.ShowProgress(100);
								DeviceHelper.UpdateDataCount(model, null);
								machinesBll.Update(model);
							}
							if (deviceServer.IsConnected)
							{
								deviceServer.Disconnect();
							}
						}
						this.ShowProgress(100);
					}
					this.ShowProgress(100);
					this.ShowProgressAll(100);
				}
				else
				{
					this.ShowProgress(100);
					this.ShowProgressAll(100);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("NoDev", "没有设备"));
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.ShowDetailInfo();
			}
			this.OnFinish(this, null);
			this.m_thead = null;
		}

		private void btn_down_Click(object sender, EventArgs e)
		{
			try
			{
				this.ShowProgress(0);
				this.ShowProgressAll(0);
				this.btn_down.Enabled = false;
				this.Cursor = Cursors.WaitCursor;
				this.progressBarUp.Value = 0;
				if (this.m_thead == null)
				{
					this.m_isStop = false;
					this.timer1.Enabled = true;
					this.Cursor = Cursors.WaitCursor;
					this.m_thead = new Thread(this.Start);
					this.m_thead.Start();
				}
				else
				{
					this.Cursor = Cursors.Default;
					this.btn_down.Enabled = true;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
				this.btn_down.Enabled = true;
			}
		}

		private void LoadDefaultID()
		{
			if (this.defaultID == 0)
			{
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				List<Departments> modelList = departmentsBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					this.defaultID = modelList[0].DEPTID;
					int num = 0;
					while (true)
					{
						if (num < modelList.Count)
						{
							if (modelList[num].SUPDEPTID != 0 && modelList[num].SUPDEPTID != -1)
							{
								num++;
								continue;
							}
							break;
						}
						return;
					}
					this.defaultID = modelList[num].DEPTID;
				}
			}
		}

		private void SaveCard(List<UserInfo> model)
		{
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				List<PersonnelIssuecard> list = new List<PersonnelIssuecard>();
				for (int i = 0; i < model.Count; i++)
				{
					if (!string.IsNullOrEmpty(model[i].CardNo) && this.dicPin_User.ContainsKey(model[i].BadgeNumber))
					{
						PersonnelIssuecard personnelIssuecard = new PersonnelIssuecard();
						personnelIssuecard.create_time = DateTime.Now;
						personnelIssuecard.UserID_id = this.dicPin_User[model[i].BadgeNumber].UserId;
						personnelIssuecard.cardno = model[i].CardNo;
						list.Add(personnelIssuecard);
					}
				}
				personnelIssuecardBll.Add(list);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void CheckUser(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserInfoStart", "人员信息获取开始，请稍候..."));
					if (devServerBll != null)
					{
						int num = 0;
						int num2 = 0;
						List<UserInfo> list = new List<UserInfo>();
						num = ((devServerBll.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.GetUserInfo(devServerBll, list) : devServerBll.STD_GetAllUserInfo(out list));
						if (list != null && list.Count > 0)
						{
							this.ShowProgress(10);
							UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
							List<UserInfo> list2 = new List<UserInfo>();
							List<UserInfo> list3 = new List<UserInfo>();
							List<UserInfo> list4 = new List<UserInfo>();
							for (int i = 0; i < list.Count; i++)
							{
								bool flag = false;
								if (this.m_isStop)
								{
									return;
								}
								if (int.TryParse(list[i].BadgeNumber, out int _))
								{
									if (!this.dicPin_User.ContainsKey(list[i].BadgeNumber))
									{
										list[i].DefaultDeptId = this.defaultID;
										list[i].MorecardGroupId = 0;
										list2.Add(list[i]);
										if (!string.IsNullOrEmpty(list[i].CardNo))
										{
											foreach (KeyValuePair<string, UserInfo> item in this.dicPin_User)
											{
												if (item.Value.CardNo == list[i].CardNo)
												{
													this.dicPin_User[item.Key].CardNo = "";
													list3.Add(this.dicPin_User[item.Key]);
												}
											}
											list4.Add(list[i]);
										}
									}
									else
									{
										UserInfo userInfo = this.dicPin_User[list[i].BadgeNumber];
										if (!string.IsNullOrEmpty(list[i].Name) && string.IsNullOrEmpty(userInfo.Name))
										{
											userInfo.Name = list[i].Name;
											flag = true;
										}
										if (!string.IsNullOrEmpty(list[i].CardNo) && list[i].CardNo != userInfo.CardNo)
										{
											userInfo.CardNo = list[i].CardNo;
											list4.Add(userInfo);
											flag = true;
										}
										if (flag)
										{
											list3.Add(userInfo);
										}
									}
									num2++;
								}
								else
								{
									SysLogServer.WriteLog($"PIN '{list[i].BadgeNumber}' can not convert to a number.", true);
								}
								if (i != 0 && i % 100 == 0)
								{
									this.ShowProgress(10 + 80 * i / list.Count);
								}
							}
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("UserCount", "用户数") + ":" + num2);
							userInfoBll.Add(list2, null);
							userInfoBll.Update(list3);
							this.Loadusers();
							this.SaveCard(list4);
							this.ShowProgress(100);
						}
						else if (num < 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SGetUserInfoFalse", "人员信息获取失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
				}
				this.ShowProgress(100);
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserInfoFinish", "人员信息获取完成"));
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.ShowDetailInfo();
			}
		}

		private void CheckTemplate(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserTemplateStart", "指纹信息获取开始，请稍候..."));
					if (devServerBll != null)
					{
						int num = 0;
						List<Template> list = new List<Template>();
						this.ShowProgress(1);
						num = ((devServerBll.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.GetUserTemplate(devServerBll, list, null) : devServerBll.STD_GetAllUserFPTemplate(out list));
						if (list != null && list.Count > 0)
						{
							this.ShowProgress(20);
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("FPCount", "指纹数") + ":" + list.Count);
							UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
							TemplateBll templateBll = new TemplateBll(MainForm._ia);
							List<Template> list2 = new List<Template>();
							List<Template> list3 = new List<Template>();
							for (int i = 0; i < list.Count; i++)
							{
								if (this.dicPin_User.ContainsKey(list[i].Pin))
								{
									list[i].USERID = this.dicPin_User[list[i].Pin].UserId;
									List<Template> list4 = null;
									if (this.dicPin_lstFp.ContainsKey(list[i].Pin))
									{
										list4 = this.dicPin_lstFp[list[i].Pin];
									}
									bool flag = false;
									if (list4 != null && list4.Count > 0)
									{
										for (int j = 0; j < list4.Count; j++)
										{
											if (list[i].Flag == 3)
											{
												int fINGERID = list[i].FINGERID;
												list[i].FINGERID = (fINGERID & 0xF);
											}
											if (list4[j].FINGERID == list[i].FINGERID)
											{
												list[i].TEMPLATEID = list4[j].TEMPLATEID;
												flag = true;
												break;
											}
										}
										if (flag)
										{
											list3.Add(list[i]);
										}
										else
										{
											list2.Add(list[i]);
										}
									}
									else
									{
										if (list[i].Flag == 3)
										{
											int fINGERID2 = list[i].FINGERID;
											list[i].FINGERID = (fINGERID2 & 0xF);
										}
										list2.Add(list[i]);
									}
								}
								if (i != 0 && i % 50 == 0)
								{
									this.ShowProgress(20 + 60 * i / list.Count);
								}
							}
							templateBll.Add(list2);
							templateBll.Update(list3, devServerBll.DevInfo.FPVersion);
							userInfoBll.Load();
						}
						else if (num < 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserTemplateFalse", "指纹信息获取失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
				}
				this.ShowProgress(100);
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserTemplateFinish", "指纹信息获取完成"));
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.ShowDetailInfo();
			}
		}

		private void CheckFingerVein(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserFVStart", "指静脉信息获取开始，请稍候..."));
					if (devServerBll != null)
					{
						int num = 0;
						List<FingerVein> list = new List<FingerVein>();
						this.ShowProgress(1);
						num = ((devServerBll.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.GetUserFingerVein(devServerBll, list, null) : 0);
						if (list != null && list.Count > 0)
						{
							this.ShowProgress(20);
							int num2 = 0;
							Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].Pin != null && !("" == list[i].Pin.Trim()))
								{
									if (!dictionary.ContainsKey(list[i].Pin))
									{
										List<int> list2 = new List<int>();
										list2.Add(list[i].FingerID);
										dictionary.Add(list[i].Pin, list2);
										num2++;
									}
									else
									{
										List<int> list2 = dictionary[list[i].Pin];
										if (!list2.Contains(list[i].FingerID))
										{
											list2.Add(list[i].FingerID);
											num2++;
										}
									}
								}
							}
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("FVCount", "指静脉数") + ":" + num2);
							UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
							FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
							for (int j = 0; j < list.Count; j++)
							{
								if (this.dicPin_User.ContainsKey(list[j].Pin))
								{
									list[j].UserID = this.dicPin_User[list[j].Pin].UserId;
								}
								if (j != 0 && j % 50 == 0)
								{
									this.ShowProgress(20 + 60 * j / list.Count);
								}
							}
							fingerVeinBll.Add(list);
							userInfoBll.Load();
							this.ShowProgress(100);
						}
						else if (num < 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserFVFalse", "指静脉信息获取失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
				}
				this.ShowProgress(100);
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserFVFinish", "指静脉信息获取完成"));
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.ShowDetailInfo();
			}
		}

		private void CheckFaceTemplate(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserFaceTempStart", "面部数据获取开始，请稍候..."));
					this.LoadFaceTempDic();
					if (devServerBll != null)
					{
						int num = 0;
						List<FaceTemp> list = new List<FaceTemp>();
						this.ShowProgress(1);
						num = ((devServerBll.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.GetFaceTemplate(devServerBll, list, null) : devServerBll.STD_GetAllUserFaceTemplate(out list));
						if (list != null && list.Count > 0)
						{
							this.ShowProgress(20);
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("FaceCount", "人脸数") + ":" + list.Count / ((devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK) ? 1 : 12));
							UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
							ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
							List<FaceTemp> list2 = new List<FaceTemp>();
							for (int i = 0; i < list.Count; i++)
							{
								bool flag = false;
								if (this.dicPin_User.ContainsKey(list[i].Pin.ToString()))
								{
									list[i].UserId = this.dicPin_User[list[i].Pin].UserId;
									list2.Add(list[i]);
								}
								if (i > 0 && (i + 1) % 20 == 0)
								{
									this.ShowProgress(20 + 60 * i / list.Count);
								}
							}
							faceTempBll.Add(list2);
							userInfoBll.Load();
							this.ShowProgress(100);
						}
						else if (num < 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserFaceTempFalse", "面部信息获取失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
				}
				this.ShowProgress(100);
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserFaceTempFinish", "面部信息获取完成"));
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
			}
		}

		private bool GetAythorizeID(DeviceServerBll devServerBll, List<AccLevelset> accLevelsetList, AccDoorBll accDoorBll, int timesetid, int authorizeDoorId, ref int authorizeid)
		{
			authorizeid = 0;
			if (this.m_levelDic.ContainsKey(timesetid + "-" + devServerBll.DevInfo.ID + "-" + authorizeDoorId))
			{
				authorizeid = this.m_levelDic[timesetid + "-" + devServerBll.DevInfo.ID + "-" + authorizeDoorId];
				return true;
			}
			try
			{
				int num = 0;
				for (num = 0; num < accLevelsetList.Count; num++)
				{
					if (accLevelsetList[num].level_timeseg_id == timesetid)
					{
						List<AccDoor> modelList = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group where acclevelset_id=" + accLevelsetList[num].id + " ) and device_id=" + devServerBll.DevInfo.ID);
						if (modelList != null && modelList.Count > 0)
						{
							int num2 = 0;
							for (int i = 0; i < modelList.Count; i++)
							{
								num2 += 1 << modelList[i].door_no - 1;
							}
							if (num2 == authorizeDoorId)
							{
								authorizeid = accLevelsetList[num].id;
								this.m_levelDic.Add(timesetid + "-" + devServerBll.DevInfo.ID + "-" + authorizeDoorId, authorizeid);
								return true;
							}
						}
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				return false;
			}
		}

		private void CheckUserAuthorize(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserAuthorizeStart", "人员权限获取开始，请稍候..."));
					if (devServerBll != null)
					{
						int num = 0;
						List<ObjUserAuthorize> list = new List<ObjUserAuthorize>();
						UserAuthorizeBll userAuthorizeBll = new UserAuthorizeBll(devServerBll.Application);
						if (devServerBll.DevInfo.DevSDKType == SDKType.StandaloneSDK)
						{
							num = 0;
						}
						else
						{
							list = userAuthorizeBll.GetList(ref num);
						}
						if (list != null && list.Count > 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":" + list.Count);
							UserAuthorizeBll userAuthorizeBll2 = new UserAuthorizeBll(devServerBll.Application);
							AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
							AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
							AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
							AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
							AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
							List<AccLevelset> modelList = accLevelsetBll.GetModelList("");
							List<AccDoor> modelList2 = accDoorBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
							List<AccLevelsetEmp> list2 = new List<AccLevelsetEmp>();
							for (int i = 0; i < list.Count; i++)
							{
								if (this.dicPin_User.ContainsKey(list[i].Pin))
								{
									int acclevelset_id = 0;
									if (list[i].AuthorizeTimezoneId == "1")
									{
										list[i].AuthorizeTimezoneId = DeviceHelper.TimeSeg.id.ToString();
									}
									if (this.m_timeiddic.ContainsKey(int.Parse(list[i].AuthorizeTimezoneId)))
									{
										int num2 = this.m_timeiddic[int.Parse(list[i].AuthorizeTimezoneId)];
										list[i].AuthorizeTimezoneId = num2.ToString();
									}
									if (this.GetAythorizeID(devServerBll, modelList, accDoorBll, int.Parse(list[i].AuthorizeTimezoneId), int.Parse(list[i].AuthorizeDoorId), ref acclevelset_id))
									{
										AccLevelsetEmp accLevelsetEmp = new AccLevelsetEmp();
										accLevelsetEmp.acclevelset_id = acclevelset_id;
										accLevelsetEmp.employee_id = this.dicPin_User[list[i].Pin].UserId;
										list2.Add(accLevelsetEmp);
									}
									else
									{
										int num3 = 0;
										if (this.m_timeiddic.ContainsKey(int.Parse(list[i].AuthorizeTimezoneId)))
										{
											num3 = this.m_timeiddic[int.Parse(list[i].AuthorizeTimezoneId)];
										}
										else if (accTimesegBll.Exists(int.Parse(list[i].AuthorizeTimezoneId)))
										{
											num3 = int.Parse(list[i].AuthorizeTimezoneId);
										}
										else
										{
											num3 = accTimesegBll.GetdefaultTime().id;
											if (!this.m_timeiddic.ContainsKey(int.Parse(list[i].AuthorizeTimezoneId)))
											{
												this.m_timeiddic.Add(int.Parse(list[i].AuthorizeTimezoneId), num3);
											}
										}
										if (!this.m_timeiddic.ContainsKey(num3))
										{
											this.m_timeiddic.Add(num3, num3);
										}
										AccLevelset accLevelset = new AccLevelset();
										accLevelset.level_timeseg_id = num3;
										accLevelset.level_name = "D-" + devServerBll.DevInfo.ID + "-B-" + list[i].AuthorizeDoorId.ToString();
										accLevelsetBll.Add(accLevelset);
										accLevelset.id = accLevelsetBll.GetMaxId() - 1;
										modelList.Add(accLevelset);
										int num4 = int.Parse(list[i].AuthorizeDoorId);
										List<int> doors = this.GetDoors(num4);
										if (doors != null && doors.Count > 0)
										{
											AccLevelsetDoorGroup accLevelsetDoorGroup = new AccLevelsetDoorGroup();
											for (int j = 0; j < modelList2.Count; j++)
											{
												int num5 = 0;
												while (num5 < doors.Count)
												{
													if (modelList2[j].door_no != doors[num5])
													{
														num5++;
														continue;
													}
													accLevelsetDoorGroup.accdoor_id = modelList2[j].id;
													accLevelsetDoorGroup.acclevelset_id = accLevelset.id;
													accLevelsetDoorGroup.accdoor_no_exp = num4;
													accLevelsetDoorGroup.accdoor_device_id = modelList2[j].device_id;
													accLevelsetDoorGroup.level_timeseg_id = accLevelset.level_timeseg_id;
													accLevelsetDoorGroupBll.Add(accLevelsetDoorGroup);
													break;
												}
											}
										}
										this.m_levelDic.Add(num3 + "-" + devServerBll.DevInfo.ID + "-" + num4, accLevelset.id);
										AccLevelsetEmp accLevelsetEmp2 = new AccLevelsetEmp();
										accLevelsetEmp2.acclevelset_id = accLevelset.id;
										accLevelsetEmp2.employee_id = this.dicPin_User[list[i].Pin].UserId;
										list2.Add(accLevelsetEmp2);
									}
								}
								this.ShowProgress(90 * i / list.Count);
							}
							accLevelsetEmpBll.Add(list2);
							this.ShowProgress(100);
						}
						else if (num < 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserAuthorizeFalse", "人员权限获取失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
					this.ShowProgress(100);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetUserAuthorizeFinish", "人员权限获取完成"));
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.ShowDetailInfo();
			}
		}

		private void CheckTime(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.m_timeiddic.Clear();
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetTimeZoneStart", "门禁时间段获取开始，请稍候..."));
					if (devServerBll != null)
					{
						int num = 0;
						List<AccTimeseg> list = new List<AccTimeseg>();
						num = ((devServerBll.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? DeviceHelper.GetTimeInfo(devServerBll, list) : 0);
						if (list != null && list.Count > 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":" + list.Count);
							AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
							for (int i = 0; i < list.Count; i++)
							{
								if (!accTimesegBll.Exists(list[i].id))
								{
									int id = list[i].id;
									list[i].timeseg_name = list[i].id.ToString();
									accTimesegBll.Add(list[i]);
									if (!this.m_timeiddic.ContainsKey(id))
									{
										this.m_timeiddic.Add(id, list[i].id);
									}
									this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetTimeZone", "获取门禁时间段成功") + ":" + list[i].timeseg_name);
								}
								else if (!this.m_timeiddic.ContainsKey(list[i].id))
								{
									this.m_timeiddic.Add(list[i].id, list[i].id);
								}
								this.ShowProgress(100 * i / list.Count);
							}
						}
						else if (num < 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetTimeZoneFalse", "门禁时间段获取失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
					this.ShowProgress(100);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("GetTimeZoneFinish", "门禁时间段获取完成"));
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.ShowDetailInfo();
			}
		}

		private List<int> GetDoors(int authorizeDoorId)
		{
			List<int> list = new List<int>();
			if (authorizeDoorId >> 3 == 1)
			{
				list.Add(4);
			}
			if ((authorizeDoorId >> 2 & 1) == 1)
			{
				list.Add(3);
			}
			if ((authorizeDoorId >> 1 & 1) == 1)
			{
				list.Add(2);
			}
			if ((authorizeDoorId & 1) == 1)
			{
				list.Add(1);
			}
			return list;
		}

		private void chk_userPrivilege_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chk_userPrivilege.Checked)
			{
				this.chk_timeZone.Checked = true;
			}
		}

		private void DeviceToPC_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.m_isStop = true;
			if (this.m_thead != null)
			{
				e.Cancel = true;
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (this.m_isStop)
			{
				this.timer1.Enabled = false;
			}
			else
			{
				this.cuur_prg++;
				this.ShowProgress(this.cuur_prg);
				if (this.cuur_prg >= 100)
				{
					this.cuur_prg = 0;
				}
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

		private void ShowDetailInfo()
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.ShowDetailInfo();
				});
			}
			else if (this.btn_show.Text == ShowMsgInfos.GetInfo("SShowInfo", "详细信息"))
			{
				this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
				this.txt_UpLoadInfo.Visible = true;
				base.Height = this.txt_UpLoadInfo.Top + this.txt_UpLoadInfo.Height + 38;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DeviceToPC));
			this.pnl_bottom = new PanelEx();
			this.ckb_FaceTemp = new CheckBox();
			this.chk_FV = new CheckBox();
			this.chk_FP = new CheckBox();
			this.lblProgressAll = new Label();
			this.lblProgress = new Label();
			this.btn_show = new ButtonX();
			this.lb_progressAll = new LabelX();
			this.progress_all = new ProgressBar();
			this.chk_timeZone = new CheckBox();
			this.lb_title = new LabelX();
			this.lb_progress = new LabelX();
			this.chk_userPrivilege = new CheckBox();
			this.chk_User = new CheckBox();
			this.btn_exit = new ButtonX();
			this.btn_down = new ButtonX();
			this.progressBarUp = new ProgressBar();
			this.txt_UpLoadInfo = new TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.pnl_bottom.SuspendLayout();
			base.SuspendLayout();
			this.pnl_bottom.CanvasColor = SystemColors.Control;
			this.pnl_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_bottom.Controls.Add(this.ckb_FaceTemp);
			this.pnl_bottom.Controls.Add(this.chk_FV);
			this.pnl_bottom.Controls.Add(this.chk_FP);
			this.pnl_bottom.Controls.Add(this.lblProgressAll);
			this.pnl_bottom.Controls.Add(this.lblProgress);
			this.pnl_bottom.Controls.Add(this.btn_show);
			this.pnl_bottom.Controls.Add(this.lb_progressAll);
			this.pnl_bottom.Controls.Add(this.progress_all);
			this.pnl_bottom.Controls.Add(this.chk_timeZone);
			this.pnl_bottom.Controls.Add(this.lb_title);
			this.pnl_bottom.Controls.Add(this.lb_progress);
			this.pnl_bottom.Controls.Add(this.chk_userPrivilege);
			this.pnl_bottom.Controls.Add(this.chk_User);
			this.pnl_bottom.Controls.Add(this.btn_exit);
			this.pnl_bottom.Controls.Add(this.btn_down);
			this.pnl_bottom.Controls.Add(this.progressBarUp);
			this.pnl_bottom.Controls.Add(this.txt_UpLoadInfo);
			this.pnl_bottom.Dock = DockStyle.Fill;
			this.pnl_bottom.Location = new Point(0, 0);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(718, 454);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 13;
			this.ckb_FaceTemp.AutoSize = true;
			this.ckb_FaceTemp.Location = new Point(529, 41);
			this.ckb_FaceTemp.Name = "ckb_FaceTemp";
			this.ckb_FaceTemp.Size = new Size(74, 17);
			this.ckb_FaceTemp.TabIndex = 35;
			this.ckb_FaceTemp.Text = "面部数据";
			this.ckb_FaceTemp.UseVisualStyleBackColor = true;
			this.chk_FV.AutoSize = true;
			this.chk_FV.Location = new Point(375, 41);
			this.chk_FV.Name = "chk_FV";
			this.chk_FV.Size = new Size(71, 17);
			this.chk_FV.TabIndex = 34;
			this.chk_FV.Text = "指静脉   ";
			this.chk_FV.UseVisualStyleBackColor = true;
			this.chk_FP.AutoSize = true;
			this.chk_FP.Checked = true;
			this.chk_FP.CheckState = CheckState.Checked;
			this.chk_FP.Location = new Point(208, 41);
			this.chk_FP.Name = "chk_FP";
			this.chk_FP.Size = new Size(74, 17);
			this.chk_FP.TabIndex = 1;
			this.chk_FP.Text = "指纹信息";
			this.chk_FP.UseVisualStyleBackColor = true;
			this.lblProgressAll.AutoSize = true;
			this.lblProgressAll.BackColor = Color.Transparent;
			this.lblProgressAll.Location = new Point(346, 205);
			this.lblProgressAll.Name = "lblProgressAll";
			this.lblProgressAll.Size = new Size(21, 13);
			this.lblProgressAll.TabIndex = 37;
			this.lblProgressAll.Text = "0%";
			this.lblProgress.AutoSize = true;
			this.lblProgress.BackColor = Color.Transparent;
			this.lblProgress.Location = new Point(346, 152);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new Size(21, 13);
			this.lblProgress.TabIndex = 36;
			this.lblProgress.Text = "0%";
			this.btn_show.AccessibleRole = AccessibleRole.PushButton;
			this.btn_show.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_show.Location = new Point(12, 234);
			this.btn_show.Name = "btn_show";
			this.btn_show.Size = new Size(230, 25);
			this.btn_show.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_show.TabIndex = 33;
			this.btn_show.Text = "隐藏信息";
			this.btn_show.Click += this.btn_show_Click;
			this.lb_progressAll.BackgroundStyle.Class = "";
			this.lb_progressAll.Location = new Point(12, 174);
			this.lb_progressAll.Name = "lb_progressAll";
			this.lb_progressAll.Size = new Size(209, 20);
			this.lb_progressAll.TabIndex = 32;
			this.lb_progressAll.Text = "总体进度";
			this.progress_all.Location = new Point(12, 198);
			this.progress_all.Name = "progress_all";
			this.progress_all.Size = new Size(692, 25);
			this.progress_all.TabIndex = 31;
			this.chk_timeZone.AutoSize = true;
			this.chk_timeZone.Location = new Point(41, 77);
			this.chk_timeZone.Name = "chk_timeZone";
			this.chk_timeZone.Size = new Size(86, 17);
			this.chk_timeZone.TabIndex = 3;
			this.chk_timeZone.Text = "门禁时间段";
			this.chk_timeZone.UseVisualStyleBackColor = true;
			this.chk_timeZone.Visible = false;
			this.lb_title.AutoSize = true;
			this.lb_title.BackgroundStyle.Class = "";
			this.lb_title.Location = new Point(12, 15);
			this.lb_title.Name = "lb_title";
			this.lb_title.Size = new Size(111, 15);
			this.lb_title.TabIndex = 10;
			this.lb_title.Text = "从设备获取以下数据";
			this.lb_progress.BackgroundStyle.Class = "";
			this.lb_progress.Location = new Point(12, 121);
			this.lb_progress.Name = "lb_progress";
			this.lb_progress.Size = new Size(209, 20);
			this.lb_progress.TabIndex = 9;
			this.lb_progress.Text = "当前设备进度";
			this.chk_userPrivilege.AutoSize = true;
			this.chk_userPrivilege.Location = new Point(375, 41);
			this.chk_userPrivilege.Name = "chk_userPrivilege";
			this.chk_userPrivilege.Size = new Size(74, 17);
			this.chk_userPrivilege.TabIndex = 2;
			this.chk_userPrivilege.Text = "门禁权限";
			this.chk_userPrivilege.UseVisualStyleBackColor = true;
			this.chk_userPrivilege.Visible = false;
			this.chk_userPrivilege.CheckedChanged += this.chk_userPrivilege_CheckedChanged;
			this.chk_User.AutoSize = true;
			this.chk_User.Checked = true;
			this.chk_User.CheckState = CheckState.Checked;
			this.chk_User.Location = new Point(41, 41);
			this.chk_User.Name = "chk_User";
			this.chk_User.Size = new Size(74, 17);
			this.chk_User.TabIndex = 0;
			this.chk_User.Text = "用户信息";
			this.chk_User.UseVisualStyleBackColor = true;
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(562, 234);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(142, 25);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 5;
			this.btn_exit.Text = "返回";
			this.btn_exit.Click += this.btn_exit_Click;
			this.btn_down.AccessibleRole = AccessibleRole.PushButton;
			this.btn_down.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_down.Location = new Point(400, 234);
			this.btn_down.Name = "btn_down";
			this.btn_down.Size = new Size(142, 25);
			this.btn_down.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_down.TabIndex = 4;
			this.btn_down.Text = "Importar";
			this.btn_down.Click += this.btn_down_Click;
			this.progressBarUp.Location = new Point(12, 146);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(692, 25);
			this.progressBarUp.TabIndex = 7;
			this.txt_UpLoadInfo.Location = new Point(12, 270);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(692, 171);
			this.txt_UpLoadInfo.TabIndex = 8;
			this.timer1.Interval = 1000;
			this.timer1.Tick += this.timer1_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(718, 454);
			base.Controls.Add(this.pnl_bottom);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DeviceToPC";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "从设备获取数据";
			base.FormClosing += this.DeviceToPC_FormClosing;
			this.pnl_bottom.ResumeLayout(false);
			this.pnl_bottom.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
