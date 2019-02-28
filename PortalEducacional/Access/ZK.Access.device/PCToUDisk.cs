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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.BLL.UDisk;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.MachinesManager.UDisk;
using ZK.Utils;

namespace ZK.Access.device
{
	public class PCToUDisk : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private List<int> lstSelectedMachineId = null;

		private SDKType SDKType;

		private DeviceHelperEx helper = new DeviceHelperEx();

		private Dictionary<int, Machines> dicMachineId_MachineInlvSet;

		private Dictionary<int, Machines> dicMachineId_MachineNotInLvSet;

		private int cuur_prg = 0;

		private bool m_isStop = false;

		private Thread m_thead = null;

		private List<UserInfo> lstUserInfo;

		private Dictionary<int, UserInfo> dicUserId_UserInfo = new Dictionary<int, UserInfo>();

		private Dictionary<int, AccLevelset> dicLvId_LvSetId;

		private Dictionary<int, List<AccLevelsetDoorGroup>> dicLvId_LvDoors;

		private List<AccHolidays> lstHoliday;

		private Dictionary<int, List<AccFirstOpenEmp>> dicOpenSetId_FirstOpenEmp;

		private List<AccFirstOpen> lstFirstOpenSet;

		private Dictionary<int, AccDoor> dicDoorId_AccDoor;

		private Dictionary<int, List<ObjMultimCard>> dicMachineID_lstMultilCardGroup;

		private Dictionary<int, List<AccMorecardGroup>> dicmcSetID_lstmcGroup;

		private int defaultID = 0;

		private Dictionary<int, int> m_timeiddic = new Dictionary<int, int>();

		private Dictionary<string, int> m_levelDic = new Dictionary<string, int>();

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

		private CheckBox ckb_timeZone;

		private System.Windows.Forms.Timer timer1;

		private ProgressBar progress_all;

		private LabelX lb_progressAll;

		private ButtonX btn_show;

		private CheckBox chk_FV;

		private CheckBox ckb_FaceTemp;

		private Label lblProgressAll;

		private Label lblProgress;

		private CheckBox ckb_MultiCard;

		private CheckBox ckb_InOutFun;

		private CheckBox ckb_FirstCard;

		private CheckBox ckb_Holiday;

		public PCToUDisk(List<int> lstMachineId, SDKType sdkType)
		{
			this.InitializeComponent();
			this.lstSelectedMachineId = lstMachineId;
			this.SDKType = sdkType;
			if (sdkType == SDKType.StandaloneSDK)
			{
				this.chk_FV.Enabled = false;
				this.chk_userPrivilege.Enabled = false;
				this.ckb_FirstCard.Enabled = false;
				this.ckb_Holiday.Enabled = false;
				this.ckb_InOutFun.Enabled = false;
				this.ckb_MultiCard.Enabled = false;
				this.ckb_timeZone.Enabled = false;
			}
			initLang.LocaleForm(this, base.Name);
			this.helper.ShowInfoEvent += this.ShowUpLoadInfo;
			this.helper.ShowProgressEvent += this.ShowProgress;
			this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
			this.btn_show_Click(null, null);
			CheckBox checkBox = this.ckb_MultiCard;
			Point location = this.chk_FP.Location;
			int x = location.X;
			location = this.ckb_MultiCard.Location;
			checkBox.Location = new Point(x, location.Y);
			CheckBox checkBox2 = this.ckb_InOutFun;
			location = this.ckb_FaceTemp.Location;
			int x2 = location.X;
			location = this.ckb_InOutFun.Location;
			checkBox2.Location = new Point(x2, location.Y);
		}

		private void LoadMachinesInLevelSet()
		{
			try
			{
				this.dicMachineId_MachineInlvSet = new Dictionary<int, Machines>();
				this.dicMachineId_MachineNotInLvSet = new Dictionary<int, Machines>();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> modelList = machinesBll.GetModelList("ID in (Select accdoor_device_id from acc_levelset_door_group)");
				for (int i = 0; i < modelList.Count; i++)
				{
					Machines machines = modelList[i];
					if (!this.dicMachineId_MachineInlvSet.ContainsKey(machines.ID))
					{
						this.dicMachineId_MachineInlvSet.Add(machines.ID, machines);
					}
				}
				modelList = machinesBll.GetModelList("");
				for (int j = 0; j < modelList.Count; j++)
				{
					Machines machines = modelList[j];
					if (!this.dicMachineId_MachineInlvSet.ContainsKey(machines.ID) && !this.dicMachineId_MachineNotInLvSet.ContainsKey(machines.ID))
					{
						this.dicMachineId_MachineNotInLvSet.Add(machines.ID, machines);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
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

		private void btn_down_Click(object sender, EventArgs e)
		{
			try
			{
				if (RemovableDiskDataManager.GetRemovableDiskList().Count <= 0)
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SNoneUDiskDetected", "未检测到U盘设备"));
				}
				else
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
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
				this.btn_down.Enabled = true;
			}
		}

		private void Start()
		{
			try
			{
				List<DriveInfo> removableDiskList = RemovableDiskDataManager.GetRemovableDiskList();
				if (removableDiskList == null || removableDiskList.Count <= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("NoUDisk", "没有U盘设备"));
					return;
				}
				DriveInfo uDataDrive = this.GetUDataDrive(removableDiskList);
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("UDisk", "可移动磁盘") + ":" + uDataDrive.Name + " " + ShowMsgInfos.GetInfo("FreeSpace", "可用空间") + ":" + Math.Round((decimal)uDataDrive.TotalFreeSpace / 1024m / 1024m, 2) + "M");
				if (!this.m_isStop)
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("BeginExportData", "开始导出数据，请稍候...") + "\r\n");
					this.LoadMachinesInLevelSet();
					this.LoadusersInLevelSet();
					if (this.lstSelectedMachineId != null && this.lstSelectedMachineId.Count > 0)
					{
						MachinesBll machinesBll = new MachinesBll(MainForm._ia);
						AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
						for (int i = 0; i < this.lstSelectedMachineId.Count; i++)
						{
							if (this.dicMachineId_MachineInlvSet.ContainsKey(this.lstSelectedMachineId[i]))
							{
								Machines machines = this.dicMachineId_MachineInlvSet[this.lstSelectedMachineId[i]];
								UDiskBll uDiskBll = new UDiskBll(machines.DevSDKType);
								DirectoryInfo directoryInfo;
								if (machines.DevSDKType == SDKType.StandaloneSDK)
								{
									directoryInfo = uDataDrive.RootDirectory;
									this.ShowUpLoadInfo(machines.MachineAlias + " " + ShowMsgInfos.GetInfo("PinWidth", "工号长度") + ":" + machines.PinWidth);
									this.ShowUpLoadInfo(machines.MachineAlias + " " + ShowMsgInfos.GetInfo("FpVersion", "指纹版本") + ":" + ((machines.FpVersion == 10) ? 10 : 9));
									this.ShowUpLoadInfo(machines.MachineAlias + " " + ShowMsgInfos.GetInfo("IsTFT", "是否为彩屏机") + ":" + (machines.IsTFTMachine ? "是" : "否") + "\r\n");
								}
								else
								{
									directoryInfo = new DirectoryInfo(uDataDrive.RootDirectory.FullName + machines.MachineNumber + "_udata");
								}
								if (uDiskBll.InitializeFiles(directoryInfo, machines))
								{
									machines.UDataTableDescription = RemovableDiskDataManager.GetTableDescriptionFileContent(directoryInfo);
									Dictionary<string, string> tableDescDictionary = RemovableDiskDataManager.GetTableDescDictionary(directoryInfo);
									List<AccLevelsetEmp> modelList = accLevelsetEmpBll.GetModelList("acclevelset_id in (select acclevelset_id from acc_levelset left join acc_levelset_door_group on acc_levelset.id=acc_levelset_door_group.acclevelset_id where accdoor_device_id=" + machines.ID + ")");
									Dictionary<int, int> dictionary = new Dictionary<int, int>();
									if (modelList != null && modelList.Count > 0)
									{
										for (int j = 0; j < modelList.Count; j++)
										{
											if (!dictionary.ContainsKey(modelList[j].employee_id))
											{
												dictionary.Add(modelList[j].employee_id, j + 1);
											}
										}
									}
									if (this.chk_User.Checked)
									{
										this.CheckUser(machines, directoryInfo, modelList, uDiskBll, tableDescDictionary, dictionary);
									}
									if (this.chk_FP.Checked)
									{
										this.CheckTemplate(machines, directoryInfo, modelList, uDiskBll, tableDescDictionary, dictionary);
									}
									if (this.chk_FV.Checked)
									{
										this.CheckFingerVein(machines, directoryInfo, modelList, uDiskBll, tableDescDictionary);
									}
									if (this.chk_userPrivilege.Checked)
									{
										this.CheckUserAuthorize(machines, directoryInfo, modelList, uDiskBll, tableDescDictionary);
									}
									if (this.ckb_FaceTemp.Checked)
									{
										this.CheckFaceTemplate(machines, directoryInfo, modelList, uDiskBll, tableDescDictionary, dictionary);
									}
									if (this.ckb_timeZone.Checked)
									{
										this.CheckTime(machines, directoryInfo, uDiskBll, tableDescDictionary);
									}
									if (this.ckb_Holiday.Checked)
									{
										this.CheckHoliday(machines, directoryInfo, uDiskBll, tableDescDictionary);
									}
									if (this.ckb_FirstCard.Checked)
									{
										this.CheckFirstCard(machines, directoryInfo, uDiskBll, tableDescDictionary);
									}
									if (this.ckb_MultiCard.Checked)
									{
										this.CheckMultiCard(machines, directoryInfo, uDiskBll, tableDescDictionary);
									}
									if (this.ckb_InOutFun.Checked)
									{
										this.CheckInOutFun(machines, directoryInfo, uDiskBll, tableDescDictionary);
									}
									machinesBll.Update(machines);
								}
								else
								{
									this.ShowUpLoadInfo(machines.MachineAlias + ":" + ShowMsgInfos.GetInfo("TableDescriptionNotFound", "未找到表描述文件，请先到设备上下载") + "\r\n");
								}
							}
							else if (this.dicMachineId_MachineNotInLvSet.ContainsKey(this.lstSelectedMachineId[i]))
							{
								Machines machines = this.dicMachineId_MachineNotInLvSet[this.lstSelectedMachineId[i]];
								this.ShowUpLoadInfo(machines.MachineAlias + ":" + ShowMsgInfos.GetInfo("MachineNotInLevelSet", "设备未加入权限组") + "\r\n");
							}
							else
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("MachineNotExists", "设备不存在") + ShowMsgInfos.GetInfo("MachineId", "设备编号") + ":" + this.lstSelectedMachineId[i] + "\r\n");
							}
							this.ShowProgressAll(100 * (i + 1) / this.lstSelectedMachineId.Count);
						}
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("NoMachinesSelected", "未选择任何设备"));
					}
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("EndExportData", "导出数据完成"));
					this.ShowProgress(100);
				}
				this.ShowProgressAll(100);
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
			}
			this.OnFinish(this, null);
			this.m_thead = null;
		}

		private void LoadusersInLevelSet()
		{
			try
			{
				this.dicUserId_UserInfo.Clear();
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				this.lstUserInfo = userInfoBll.GetModelList("USERID in (select employee_id from acc_levelset_emp)");
				if (this.lstUserInfo != null && this.lstUserInfo.Count > 0)
				{
					for (int i = 0; i < this.lstUserInfo.Count; i++)
					{
						if (!this.dicUserId_UserInfo.ContainsKey(this.lstUserInfo[i].UserId))
						{
							this.dicUserId_UserInfo.Add(this.lstUserInfo[i].UserId, this.lstUserInfo[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void CheckUser(Machines machine, DirectoryInfo MachineDir, List<AccLevelsetEmp> lstLvEmp, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc, Dictionary<int, int> dicUserId_NewUserId)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (machine.DevSDKType != SDKType.StandaloneSDK && !dicTableDesc.ContainsKey("user"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("UserTableDescriptionNotFound", "未找到用户表描述信息，请先到设备上下载"));
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportUser", "开始导出人员数据"));
						if (lstLvEmp != null && lstLvEmp.Count > 0)
						{
							List<UserInfo> list = new List<UserInfo>();
							for (int i = 0; i < lstLvEmp.Count; i++)
							{
								AccLevelsetEmp accLevelsetEmp = lstLvEmp[i];
								if (this.dicUserId_UserInfo.ContainsKey(accLevelsetEmp.employee_id))
								{
									UserInfo userInfo = this.dicUserId_UserInfo[accLevelsetEmp.employee_id];
									if (machine.DevSDKType == SDKType.StandaloneSDK && dicUserId_NewUserId.ContainsKey(userInfo.UserId))
									{
										userInfo = userInfo.Copy();
										userInfo.UserId = dicUserId_NewUserId[userInfo.UserId];
									}
									list.Add(userInfo);
								}
							}
							this.ShowProgress(20);
							try
							{
								int num = udiskBll.ExportUserInfo(list, MachineDir, machine);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserSucceed", "导出人员数据成功") + ":" + list.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserFailed", "导出人员数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserFailed", "导出人员数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
					this.ShowProgress(100);
					this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserInfoFinish", "导出人员信息完成") + "\r\n");
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private Dictionary<int, List<FaceTemp>> GetUserId_lstFaceTemplateDic()
		{
			return new Dictionary<int, List<FaceTemp>>();
		}

		private void CheckFaceTemplate(Machines machine, DirectoryInfo MachineDir, List<AccLevelsetEmp> lstLvEmp, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc, Dictionary<int, int> dicUserId_NewUserId)
		{
			if (!this.m_isStop)
			{
				if (machine.DevSDKType != SDKType.StandaloneSDK && !dicTableDesc.ContainsKey("ssrface7"))
				{
					this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("FaceTableDescriptionNotFound", "未找到人脸表描述信息，请先到设备上下载") + "\r\n");
				}
				else
				{
					this.ShowProgress(0);
					this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportFace", "开始导出人脸数据"));
					try
					{
						ZK.Data.BLL.FaceTempBll faceTempBll = new ZK.Data.BLL.FaceTempBll(MainForm._ia);
						List<FaceTemp> list = faceTempBll.GetModelList(((AppSite.Instance.DataType == DataType.Access) ? " clng(USERNO) " : " USERNO ") + "in(select employee_id from acc_levelset_emp where acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id=" + machine.ID + "))");
						if (list != null)
						{
							if (machine.DevSDKType == SDKType.StandaloneSDK)
							{
								for (int i = 0; i < list.Count; i++)
								{
									FaceTemp faceTemp = list[i];
									int userId = faceTemp.UserId;
									if (dicUserId_NewUserId.ContainsKey(userId))
									{
										faceTemp.UserId = dicUserId_NewUserId[userId];
									}
								}
							}
							else
							{
								list = FaceDataConverter.PullFace2DBFace(FaceDataConverter.DBFace2PullFace(list));
							}
						}
						UDiskBll uDiskBll = new UDiskBll(machine.DevSDKType);
						int num = uDiskBll.ExportFaceTemplate(list, MachineDir, machine);
						if (num >= 0)
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFaceSucceed", "导出人脸数据成功") + ":" + num);
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFaceFailed", "导出人脸数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						}
					}
					catch (Exception ex)
					{
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFaceFailed", "导出人脸数据失败") + ":" + ex.Message);
					}
					this.ShowProgress(100);
					this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFaceFinish", "导出人脸数据完成") + "\r\n");
				}
			}
		}

		private Dictionary<int, List<Template>> GetUserId_LstTemplateDic()
		{
			Dictionary<int, List<Template>> dictionary = new Dictionary<int, List<Template>>();
			TemplateBll templateBll = new TemplateBll(MainForm._ia);
			List<Template> modelList = templateBll.GetModelList("USERID in (select employee_id from acc_levelset_emp)");
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (this.dicUserId_UserInfo.ContainsKey(modelList[i].USERID))
					{
						modelList[i].Pin = this.dicUserId_UserInfo[modelList[i].USERID].BadgeNumber;
						if (dictionary.ContainsKey(modelList[i].USERID))
						{
							dictionary[modelList[i].USERID].Add(modelList[i]);
						}
						else
						{
							List<Template> list = new List<Template>();
							list.Add(modelList[i]);
							dictionary.Add(modelList[i].USERID, list);
						}
					}
				}
			}
			return dictionary;
		}

		private void CheckTemplate(Machines machine, DirectoryInfo MachineDir, List<AccLevelsetEmp> lstLvEmp, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc, Dictionary<int, int> dicUserId_NewUserId)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (machine.DevSDKType != SDKType.StandaloneSDK && !dicTableDesc.ContainsKey("templatev10"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("TemplateTableDescriptionNotFound", "未找到指纹表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportTemplate", "开始导出指纹数据"));
						Dictionary<int, List<Template>> userId_LstTemplateDic = this.GetUserId_LstTemplateDic();
						if (userId_LstTemplateDic != null && userId_LstTemplateDic.Count > 0 && lstLvEmp != null && lstLvEmp.Count > 0)
						{
							List<Template> list = new List<Template>();
							for (int i = 0; i < lstLvEmp.Count; i++)
							{
								AccLevelsetEmp accLevelsetEmp = lstLvEmp[i];
								if (this.dicUserId_UserInfo.ContainsKey(accLevelsetEmp.employee_id) && userId_LstTemplateDic.ContainsKey(accLevelsetEmp.employee_id))
								{
									UserInfo userInfo = this.dicUserId_UserInfo[accLevelsetEmp.employee_id];
									int num = userInfo.UserId;
									if (machine.DevSDKType == SDKType.StandaloneSDK && dicUserId_NewUserId.ContainsKey(num))
									{
										num = dicUserId_NewUserId[num];
									}
									List<Template> list2 = userId_LstTemplateDic[accLevelsetEmp.employee_id];
									for (int j = 0; j < list2.Count; j++)
									{
										Template template = list2[j].Copy();
										template.Pin = userInfo.BadgeNumber;
										template.USERID = num;
										list.Add(template);
									}
								}
							}
							this.ShowProgress(20);
							try
							{
								int num2 = udiskBll.ExportFingerPrint(list, MachineDir, machine);
								if (num2 >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTemplateSucceed", "导出指纹数据成功") + ":" + num2);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTemplateFailed", "导出指纹数据失败") + ":" + PullSDkErrorInfos.GetInfo(num2));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTemplateFailed", "导出指纹数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
					}
					this.ShowProgress(100);
					this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTemplateInfoFinish", "导出指纹信息完成") + "\r\n");
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private Dictionary<int, List<FingerVein>> GetUserId_lstFvTemplateDic()
		{
			Dictionary<int, List<FingerVein>> dictionary = new Dictionary<int, List<FingerVein>>();
			try
			{
				dictionary.Clear();
				FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
				List<FingerVein> modelList = fingerVeinBll.GetModelList("USERID in (select employee_id from acc_levelset_emp)");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.dicUserId_UserInfo.ContainsKey(modelList[i].UserID))
						{
							modelList[i].Pin = this.dicUserId_UserInfo[modelList[i].UserID].BadgeNumber;
							if (dictionary.ContainsKey(modelList[i].UserID))
							{
								dictionary[modelList[i].UserID].Add(modelList[i]);
							}
							else
							{
								List<FingerVein> list = new List<FingerVein>();
								list.Add(modelList[i]);
								dictionary.Add(modelList[i].UserID, list);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			return dictionary;
		}

		private void CheckFingerVein(Machines machine, DirectoryInfo MachineDir, List<AccLevelsetEmp> lstLvEmp, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("fvtemplate"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("FVTemplateTableDescriptionNotFound", "未找到指静脉表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportFVTemplate", "开始导出静脉指纹数据"));
						Dictionary<int, List<FingerVein>> userId_lstFvTemplateDic = this.GetUserId_lstFvTemplateDic();
						if (userId_lstFvTemplateDic != null && userId_lstFvTemplateDic.Count > 0 && lstLvEmp != null && lstLvEmp.Count > 0)
						{
							List<FingerVein> list = new List<FingerVein>();
							for (int i = 0; i < lstLvEmp.Count; i++)
							{
								AccLevelsetEmp accLevelsetEmp = lstLvEmp[i];
								if (this.dicUserId_UserInfo.ContainsKey(accLevelsetEmp.employee_id) && userId_lstFvTemplateDic.ContainsKey(accLevelsetEmp.employee_id))
								{
									list.AddRange(userId_lstFvTemplateDic[accLevelsetEmp.employee_id]);
								}
							}
							this.ShowProgress(80);
							try
							{
								int num = udiskBll.ExportFingerVein(list, MachineDir, machine);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFVTemplateSucceed", "导出静脉指纹数据成功") + ":" + num);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFVTemplateFailed", "导出静脉指纹数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFVTemplateFailed", "导出静脉指纹数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFVTemplateInfoFinish", "导出静脉指纹信息完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private void LoadLevelSetDic()
		{
			if (this.dicLvId_LvSetId == null)
			{
				this.dicLvId_LvSetId = new Dictionary<int, AccLevelset>();
				AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
				List<AccLevelset> modelList = accLevelsetBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.dicLvId_LvSetId.Add(modelList[i].id, modelList[i]);
					}
				}
			}
		}

		private void LoadLevelDoorsDic()
		{
			if (this.dicLvId_LvDoors == null)
			{
				this.dicLvId_LvDoors = new Dictionary<int, List<AccLevelsetDoorGroup>>();
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				List<AccLevelsetDoorGroup> modelList = accLevelsetDoorGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.dicLvId_LvDoors.ContainsKey(modelList[i].acclevelset_id))
						{
							List<AccLevelsetDoorGroup> list = this.dicLvId_LvDoors[modelList[i].acclevelset_id];
							list.Add(modelList[i]);
						}
						else
						{
							List<AccLevelsetDoorGroup> list = new List<AccLevelsetDoorGroup>();
							list.Add(modelList[i]);
							this.dicLvId_LvDoors.Add(modelList[i].acclevelset_id, list);
						}
					}
				}
			}
		}

		private void CheckUserAuthorize(Machines machine, DirectoryInfo MachineDir, List<AccLevelsetEmp> lstLvEmp, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("userauthorize"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("UserAuthorizeTableDescriptionNotFound", "未找到用户权限表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportUserAuthorize", "开始导出人员权限数据"));
						this.LoadDoors();
						this.LoadLevelSetDic();
						this.LoadLevelDoorsDic();
						if (lstLvEmp != null && lstLvEmp.Count > 0 && this.dicLvId_LvDoors != null && this.dicLvId_LvDoors.Count > 0)
						{
							DataTable dataTable = new DataTable();
							dataTable.Columns.Add("Pin", typeof(string));
							dataTable.Columns.Add("AuthorizeTimezoneId", typeof(string));
							dataTable.Columns.Add("AuthorizeDoorId", typeof(string));
							this.ShowProgress(20);
							for (int i = 0; i < lstLvEmp.Count; i++)
							{
								AccLevelsetEmp accLevelsetEmp = lstLvEmp[i];
								if (this.dicUserId_UserInfo.ContainsKey(accLevelsetEmp.employee_id))
								{
									UserInfo userInfo = this.dicUserId_UserInfo[accLevelsetEmp.employee_id];
									if (this.dicLvId_LvSetId.ContainsKey(accLevelsetEmp.acclevelset_id))
									{
										AccLevelset accLevelset = this.dicLvId_LvSetId[accLevelsetEmp.acclevelset_id];
										if (this.dicLvId_LvDoors.ContainsKey(accLevelsetEmp.acclevelset_id))
										{
											List<AccLevelsetDoorGroup> list = this.dicLvId_LvDoors[accLevelsetEmp.acclevelset_id];
											DataRow dataRow = dataTable.NewRow();
											dataRow["Pin"] = userInfo.BadgeNumber;
											dataRow["AuthorizeTimezoneId"] = accLevelset.level_timeseg_id;
											int num = 0;
											for (int j = 0; j < list.Count; j++)
											{
												if (this.dicDoorId_AccDoor.ContainsKey(list[j].accdoor_id))
												{
													num |= this.dicDoorId_AccDoor[list[j].accdoor_id].door_no;
												}
											}
											dataRow["AuthorizeDoorId"] = num;
											dataTable.Rows.Add(dataRow);
										}
									}
								}
							}
							this.ShowProgress(80);
							try
							{
								int num2 = RemovableDiskDataManager.ExportData(RemovableDiskDataManager.DiskDataType.UserAuthorize, dataTable, MachineDir);
								if (num2 >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserAuthorizeSucceed", "导出人员权限数据成功") + ":" + dataTable.Rows.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserAuthorizeFailed", "导出人员权限数据失败") + ":" + PullSDkErrorInfos.GetInfo(num2));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserAuthorizeFailed", "导出人员权限数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportUserAuthorizeFinish", "导出人员权限完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private List<AccTimeseg> GetTimeZoneList()
		{
			List<AccTimeseg> result = new List<AccTimeseg>();
			try
			{
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				result = accTimesegBll.GetModelList("");
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
			return result;
		}

		private void CheckTime(Machines machine, DirectoryInfo MachineDir, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("timezone"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("TimeZoneTableDescriptionNotFound", "未找到时间段表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportTimeZone", "开始导出时间段数据"));
						List<AccTimeseg> timeZoneList = this.GetTimeZoneList();
						if (timeZoneList != null && timeZoneList.Count > 0)
						{
							DataTable dataTable = new DataTable();
							dataTable.Columns.Add("TimezoneId", typeof(string));
							dataTable.Columns.Add("SunTime1", typeof(string));
							dataTable.Columns.Add("SunTime2", typeof(string));
							dataTable.Columns.Add("SunTime3", typeof(string));
							dataTable.Columns.Add("MonTime1", typeof(string));
							dataTable.Columns.Add("MonTime2", typeof(string));
							dataTable.Columns.Add("MonTime3", typeof(string));
							dataTable.Columns.Add("TueTime1", typeof(string));
							dataTable.Columns.Add("TueTime2", typeof(string));
							dataTable.Columns.Add("TueTime3", typeof(string));
							dataTable.Columns.Add("WedTime1", typeof(string));
							dataTable.Columns.Add("WedTime2", typeof(string));
							dataTable.Columns.Add("WedTime3", typeof(string));
							dataTable.Columns.Add("ThuTime1", typeof(string));
							dataTable.Columns.Add("ThuTime2", typeof(string));
							dataTable.Columns.Add("ThuTime3", typeof(string));
							dataTable.Columns.Add("FriTime1", typeof(string));
							dataTable.Columns.Add("FriTime2", typeof(string));
							dataTable.Columns.Add("FriTime3", typeof(string));
							dataTable.Columns.Add("SatTime1", typeof(string));
							dataTable.Columns.Add("SatTime2", typeof(string));
							dataTable.Columns.Add("SatTime3", typeof(string));
							dataTable.Columns.Add("Hol1Time1", typeof(string));
							dataTable.Columns.Add("Hol1Time2", typeof(string));
							dataTable.Columns.Add("Hol1Time3", typeof(string));
							dataTable.Columns.Add("Hol2Time1", typeof(string));
							dataTable.Columns.Add("Hol2Time2", typeof(string));
							dataTable.Columns.Add("Hol2Time3", typeof(string));
							dataTable.Columns.Add("Hol3Time1", typeof(string));
							dataTable.Columns.Add("Hol3Time2", typeof(string));
							dataTable.Columns.Add("Hol3Time3", typeof(string));
							this.ShowProgress(20);
							for (int i = 0; i < timeZoneList.Count; i++)
							{
								AccTimeseg accTimeseg = timeZoneList[i];
								DataRow dataRow = dataTable.NewRow();
								dataRow["TimezoneId"] = accTimeseg.id;
								dataRow["SunTime1"] = this.EncodeTimeZone(accTimeseg.sunday_start1, accTimeseg.sunday_end1);
								dataRow["SunTime2"] = this.EncodeTimeZone(accTimeseg.sunday_start2, accTimeseg.sunday_end2);
								dataRow["SunTime3"] = this.EncodeTimeZone(accTimeseg.sunday_start3, accTimeseg.sunday_end3);
								dataRow["MonTime1"] = this.EncodeTimeZone(accTimeseg.monday_start1, accTimeseg.monday_end1);
								dataRow["MonTime2"] = this.EncodeTimeZone(accTimeseg.monday_start2, accTimeseg.monday_end2);
								dataRow["MonTime3"] = this.EncodeTimeZone(accTimeseg.monday_start3, accTimeseg.monday_end3);
								dataRow["TueTime1"] = this.EncodeTimeZone(accTimeseg.tuesday_start1, accTimeseg.tuesday_end1);
								dataRow["TueTime2"] = this.EncodeTimeZone(accTimeseg.tuesday_start2, accTimeseg.tuesday_end2);
								dataRow["TueTime3"] = this.EncodeTimeZone(accTimeseg.tuesday_start3, accTimeseg.tuesday_end3);
								dataRow["WedTime1"] = this.EncodeTimeZone(accTimeseg.wednesday_start1, accTimeseg.wednesday_end1);
								dataRow["WedTime2"] = this.EncodeTimeZone(accTimeseg.wednesday_start2, accTimeseg.wednesday_end2);
								dataRow["WedTime3"] = this.EncodeTimeZone(accTimeseg.wednesday_start3, accTimeseg.wednesday_end3);
								dataRow["ThuTime1"] = this.EncodeTimeZone(accTimeseg.thursday_start1, accTimeseg.thursday_end1);
								dataRow["ThuTime2"] = this.EncodeTimeZone(accTimeseg.thursday_start2, accTimeseg.thursday_end2);
								dataRow["ThuTime3"] = this.EncodeTimeZone(accTimeseg.thursday_start3, accTimeseg.thursday_end3);
								dataRow["FriTime1"] = this.EncodeTimeZone(accTimeseg.friday_start1, accTimeseg.friday_end1);
								dataRow["FriTime2"] = this.EncodeTimeZone(accTimeseg.friday_start2, accTimeseg.friday_end2);
								dataRow["FriTime3"] = this.EncodeTimeZone(accTimeseg.friday_start3, accTimeseg.friday_end3);
								dataRow["SatTime1"] = this.EncodeTimeZone(accTimeseg.saturday_start1, accTimeseg.saturday_end1);
								dataRow["SatTime2"] = this.EncodeTimeZone(accTimeseg.saturday_start2, accTimeseg.saturday_end2);
								dataRow["SatTime3"] = this.EncodeTimeZone(accTimeseg.saturday_start3, accTimeseg.saturday_end3);
								dataRow["Hol1Time1"] = this.EncodeTimeZone(accTimeseg.holidaytype1_start1, accTimeseg.holidaytype1_end1);
								dataRow["Hol1Time2"] = this.EncodeTimeZone(accTimeseg.holidaytype1_start2, accTimeseg.holidaytype1_end2);
								dataRow["Hol1Time3"] = this.EncodeTimeZone(accTimeseg.holidaytype1_start3, accTimeseg.holidaytype1_end3);
								dataRow["Hol2Time1"] = this.EncodeTimeZone(accTimeseg.holidaytype2_start1, accTimeseg.holidaytype2_end1);
								dataRow["Hol2Time2"] = this.EncodeTimeZone(accTimeseg.holidaytype2_start2, accTimeseg.holidaytype2_end2);
								dataRow["Hol2Time3"] = this.EncodeTimeZone(accTimeseg.holidaytype2_start3, accTimeseg.holidaytype2_end3);
								dataRow["Hol3Time1"] = this.EncodeTimeZone(accTimeseg.holidaytype3_start1, accTimeseg.holidaytype3_end1);
								dataRow["Hol3Time2"] = this.EncodeTimeZone(accTimeseg.holidaytype3_start2, accTimeseg.holidaytype3_end2);
								dataRow["Hol3Time3"] = this.EncodeTimeZone(accTimeseg.holidaytype3_start3, accTimeseg.holidaytype3_end3);
								dataTable.Rows.Add(dataRow);
								this.ShowProgress(20 + 60 * i / timeZoneList.Count);
							}
							try
							{
								int num = RemovableDiskDataManager.ExportData(RemovableDiskDataManager.DiskDataType.TimeZone, dataTable, MachineDir);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTimeZoneSucceed", "导出时间段数据成功") + ":" + dataTable.Rows.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTimeZoneFailed", "导出时间段数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTimeZoneFailed", "导出时间段数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportTimeZoneFinish", "导出时间段数据完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private List<AccHolidays> GetHolidayList()
		{
			List<AccHolidays> result = new List<AccHolidays>();
			try
			{
				AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
				result = accHolidaysBll.GetModelList("");
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
			return result;
		}

		private void CheckHoliday(Machines machine, DirectoryInfo MachineDir, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("holiday"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("HolidayTableDescriptionNotFound", "未找到节假日表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportHoliday", "开始导出节假日数据"));
						List<AccHolidays> holidayList = this.GetHolidayList();
						if (holidayList != null && holidayList.Count > 0)
						{
							DataTable dataTable = new DataTable();
							dataTable.Columns.Add("Holiday", typeof(string));
							dataTable.Columns.Add("HolidayType", typeof(string));
							dataTable.Columns.Add("Loop", typeof(string));
							this.ShowProgress(20);
							for (int i = 0; i < holidayList.Count; i++)
							{
								AccHolidays accHolidays = holidayList[i];
								DateTime value = accHolidays.start_date.Value;
								DateTime value2 = accHolidays.end_date.Value;
								TimeSpan timeSpan = value2 - value;
								for (int j = 0; j < timeSpan.Days + 1; j++)
								{
									DataRow dataRow = dataTable.NewRow();
									dataRow["Holiday"] = value.AddDays((double)j).ToString("yyyyMMdd");
									dataRow["HolidayType"] = accHolidays.holiday_type;
									dataRow["Loop"] = accHolidays.loop_by_year;
									dataTable.Rows.Add(dataRow);
								}
								this.ShowProgress(20 + 60 * i / holidayList.Count);
							}
							try
							{
								int num = RemovableDiskDataManager.ExportData(RemovableDiskDataManager.DiskDataType.Holiday, dataTable, MachineDir);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportHolidaySucceed", "导出节假日数据成功") + ":" + dataTable.Rows.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportHolidayFailed", "导出节假日数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportHolidayFailed", "导出节假日数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportHolidayFinish", "导出节假日数据完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private void LoadFirstCard()
		{
			try
			{
				if (this.lstFirstOpenSet == null)
				{
					this.lstFirstOpenSet = new List<AccFirstOpen>();
					AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
					List<AccFirstOpen> modelList = accFirstOpenBll.GetModelList("");
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							AccFirstOpen item = modelList[i];
							this.lstFirstOpenSet.Add(item);
						}
					}
				}
				if (this.dicOpenSetId_FirstOpenEmp == null)
				{
					this.dicOpenSetId_FirstOpenEmp = new Dictionary<int, List<AccFirstOpenEmp>>();
					AccFirstOpenEmpBll accFirstOpenEmpBll = new AccFirstOpenEmpBll(MainForm._ia);
					List<AccFirstOpenEmp> modelList2 = accFirstOpenEmpBll.GetModelList("");
					if (modelList2 != null && modelList2.Count > 0)
					{
						for (int j = 0; j < modelList2.Count; j++)
						{
							AccFirstOpenEmp accFirstOpenEmp = modelList2[j];
							if (this.dicOpenSetId_FirstOpenEmp.ContainsKey(accFirstOpenEmp.accfirstopen_id))
							{
								List<AccFirstOpenEmp> list = this.dicOpenSetId_FirstOpenEmp[accFirstOpenEmp.accfirstopen_id];
								list.Add(accFirstOpenEmp);
							}
							else
							{
								List<AccFirstOpenEmp> list = new List<AccFirstOpenEmp>();
								list.Add(accFirstOpenEmp);
								this.dicOpenSetId_FirstOpenEmp.Add(accFirstOpenEmp.accfirstopen_id, list);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private void CheckFirstCard(Machines machine, DirectoryInfo MachineDir, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("firstcard"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("FirstCardTableDescriptionNotFound", "未找到首卡常开表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportFirstCard", "开始导出首卡常开数据"));
						this.LoadDoors();
						this.LoadFirstCard();
						if (this.dicDoorId_AccDoor != null && this.dicDoorId_AccDoor.Count > 0 && this.lstFirstOpenSet != null && this.lstFirstOpenSet.Count > 0)
						{
							DataTable dataTable = new DataTable();
							dataTable.Columns.Add("Pin", typeof(string));
							dataTable.Columns.Add("DoorID", typeof(string));
							dataTable.Columns.Add("TimezoneID", typeof(string));
							this.ShowProgress(20);
							for (int i = 0; i < this.lstFirstOpenSet.Count; i++)
							{
								AccFirstOpen accFirstOpen = this.lstFirstOpenSet[i];
								if (this.dicDoorId_AccDoor.ContainsKey(accFirstOpen.door_id))
								{
									AccDoor accDoor = this.dicDoorId_AccDoor[accFirstOpen.door_id];
									if (this.dicOpenSetId_FirstOpenEmp.ContainsKey(accFirstOpen.id))
									{
										List<AccFirstOpenEmp> list = this.dicOpenSetId_FirstOpenEmp[accFirstOpen.id];
										for (int j = 0; j < list.Count; j++)
										{
											AccFirstOpenEmp accFirstOpenEmp = list[j];
											if (this.dicUserId_UserInfo.ContainsKey(accFirstOpenEmp.employee_id))
											{
												UserInfo userInfo = this.dicUserId_UserInfo[accFirstOpenEmp.employee_id];
												DataRow dataRow = dataTable.NewRow();
												dataRow["Pin"] = userInfo.BadgeNumber;
												dataRow["DoorID"] = accDoor.door_no;
												dataRow["TimezoneID"] = accFirstOpen.timeseg_id;
												dataTable.Rows.Add(dataRow);
											}
										}
									}
								}
								this.ShowProgress(20 + 60 * i / this.lstFirstOpenSet.Count);
							}
							try
							{
								int num = RemovableDiskDataManager.ExportData(RemovableDiskDataManager.DiskDataType.FirstCard, dataTable, MachineDir);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFirstCardSucceed", "导出首卡常开数据成功") + ":" + dataTable.Rows.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFirstCardFailed", "导出首卡常开数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFirstCardFailed", "导出首卡常开数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportFirstCardFinish", "导出首卡常开数据完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private void LoadDoors()
		{
			if (this.dicDoorId_AccDoor == null)
			{
				this.dicDoorId_AccDoor = new Dictionary<int, AccDoor>();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						AccDoor accDoor = modelList[i];
						this.dicDoorId_AccDoor.Add(accDoor.id, accDoor);
					}
				}
			}
		}

		private void LoadMultiCard()
		{
			if (this.dicmcSetID_lstmcGroup == null)
			{
				this.dicmcSetID_lstmcGroup = new Dictionary<int, List<AccMorecardGroup>>();
				AccMorecardGroupBll accMorecardGroupBll = new AccMorecardGroupBll(MainForm._ia);
				List<AccMorecardGroup> modelList = accMorecardGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						AccMorecardGroup accMorecardGroup = modelList[i];
						if (this.dicmcSetID_lstmcGroup.ContainsKey(accMorecardGroup.comb_id))
						{
							List<AccMorecardGroup> list = this.dicmcSetID_lstmcGroup[accMorecardGroup.comb_id];
							list.Add(accMorecardGroup);
						}
						else
						{
							List<AccMorecardGroup> list = new List<AccMorecardGroup>();
							list.Add(accMorecardGroup);
							this.dicmcSetID_lstmcGroup.Add(accMorecardGroup.comb_id, list);
						}
					}
				}
			}
			if (this.dicMachineID_lstMultilCardGroup == null)
			{
				this.dicMachineID_lstMultilCardGroup = new Dictionary<int, List<ObjMultimCard>>();
				AccMorecardsetBll accMorecardsetBll = new AccMorecardsetBll(MainForm._ia);
				List<AccMorecardset> modelList2 = accMorecardsetBll.GetModelList("");
				if (modelList2 != null && modelList2.Count > 0)
				{
					for (int j = 0; j < modelList2.Count; j++)
					{
						AccMorecardset accMorecardset = modelList2[j];
						if (this.dicDoorId_AccDoor.ContainsKey(accMorecardset.door_id) && this.dicmcSetID_lstmcGroup.ContainsKey(accMorecardset.id))
						{
							int num = 0;
							int[] array = new int[5];
							List<AccMorecardGroup> list = this.dicmcSetID_lstmcGroup[accMorecardset.id];
							for (int k = 0; k < list.Count; k++)
							{
								int num2 = list[k].opener_number;
								if (num2 > 5)
								{
									num2 = 5;
								}
								if (num + num2 > 5)
								{
									num += num2;
									break;
								}
								for (int l = num; l < num + num2; l++)
								{
									array[l] = list[k].group_id;
								}
								num += num2;
							}
							if (num <= 5)
							{
								ObjMultimCard objMultimCard = new ObjMultimCard();
								ObjMultimCard objMultimCard2 = objMultimCard;
								int num3 = accMorecardset.id;
								objMultimCard2.Index = num3.ToString();
								ObjMultimCard objMultimCard3 = objMultimCard;
								num3 = this.dicDoorId_AccDoor[accMorecardset.door_id].door_no;
								objMultimCard3.DoorId = num3.ToString();
								objMultimCard.Group1 = array[0].ToString();
								objMultimCard.Group2 = array[1].ToString();
								objMultimCard.Group3 = array[2].ToString();
								objMultimCard.Group4 = array[3].ToString();
								objMultimCard.Group5 = array[4].ToString();
								if (this.dicMachineID_lstMultilCardGroup.ContainsKey(this.dicDoorId_AccDoor[accMorecardset.door_id].device_id))
								{
									List<ObjMultimCard> list2 = this.dicMachineID_lstMultilCardGroup[this.dicDoorId_AccDoor[accMorecardset.door_id].device_id];
									list2.Add(objMultimCard);
								}
								else
								{
									List<ObjMultimCard> list2 = new List<ObjMultimCard>();
									list2.Add(objMultimCard);
									this.dicMachineID_lstMultilCardGroup.Add(this.dicDoorId_AccDoor[accMorecardset.door_id].device_id, list2);
								}
							}
						}
					}
				}
			}
		}

		private void CheckMultiCard(Machines machine, DirectoryInfo MachineDir, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("multimcard"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("MultiCardTableDescriptionNotFound", "未找到多卡开门组表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportMultiCard", "开始导出多卡开门数据"));
						this.LoadDoors();
						this.LoadMultiCard();
						if (this.dicDoorId_AccDoor != null && this.dicDoorId_AccDoor.Count > 0 && this.dicMachineID_lstMultilCardGroup != null && this.dicMachineID_lstMultilCardGroup.Count > 0 && this.dicMachineID_lstMultilCardGroup.ContainsKey(machine.ID))
						{
							DataTable dataTable = new DataTable();
							dataTable.Columns.Add("Index", typeof(string));
							dataTable.Columns.Add("DoorId", typeof(string));
							dataTable.Columns.Add("Group1", typeof(string));
							dataTable.Columns.Add("Group2", typeof(string));
							dataTable.Columns.Add("Group3", typeof(string));
							dataTable.Columns.Add("Group4", typeof(string));
							dataTable.Columns.Add("Group5", typeof(string));
							this.ShowProgress(20);
							List<ObjMultimCard> list = this.dicMachineID_lstMultilCardGroup[machine.ID];
							for (int i = 0; i < list.Count; i++)
							{
								ObjMultimCard objMultimCard = list[i];
								DataRow dataRow = dataTable.NewRow();
								dataRow["Index"] = objMultimCard.Index;
								dataRow["DoorId"] = objMultimCard.DoorId;
								dataRow["Group1"] = objMultimCard.Group1;
								dataRow["Group2"] = objMultimCard.Group2;
								dataRow["Group3"] = objMultimCard.Group3;
								dataRow["Group4"] = objMultimCard.Group4;
								dataRow["Group5"] = objMultimCard.Group5;
								dataTable.Rows.Add(dataRow);
								this.ShowProgress(20 + 60 * i / list.Count);
							}
							try
							{
								int num = RemovableDiskDataManager.ExportData(RemovableDiskDataManager.DiskDataType.MultimCard, dataTable, MachineDir);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportMultiCardSucceed", "导出多卡开门数据成功") + ":" + dataTable.Rows.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportMultiCardFailed", "导出多卡开门数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportMultiCardFailed", "导出多卡开门数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportMultiCardFinish", "导出多卡开门数据完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private Dictionary<int, List<AccLinkAgeIo>> GetInOutFunDic()
		{
			Dictionary<int, List<AccLinkAgeIo>> dictionary = new Dictionary<int, List<AccLinkAgeIo>>();
			AccLinkAgeIoBll accLinkAgeIoBll = new AccLinkAgeIoBll(MainForm._ia);
			List<AccLinkAgeIo> modelList = accLinkAgeIoBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					AccLinkAgeIo accLinkAgeIo = modelList[i];
					if (dictionary.ContainsKey(accLinkAgeIo.device_id))
					{
						List<AccLinkAgeIo> list = dictionary[accLinkAgeIo.device_id];
						list.Add(accLinkAgeIo);
					}
					else
					{
						List<AccLinkAgeIo> list = new List<AccLinkAgeIo>();
						list.Add(accLinkAgeIo);
						dictionary.Add(accLinkAgeIo.device_id, list);
					}
				}
			}
			return dictionary;
		}

		private void CheckInOutFun(Machines machine, DirectoryInfo MachineDir, UDiskBll udiskBll, Dictionary<string, string> dicTableDesc)
		{
			try
			{
				if (!this.m_isStop)
				{
					if (!dicTableDesc.ContainsKey("inoutfun"))
					{
						this.ShowUpLoadInfo(machine.MachineAlias + ":" + ShowMsgInfos.GetInfo("InOutFunTableDescriptionNotFound", "未找到联动表描述信息，请先到设备上下载") + "\r\n");
					}
					else
					{
						this.ShowProgress(0);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("BeginExportInOutFun", "开始导出联动数据"));
						Dictionary<int, List<AccLinkAgeIo>> inOutFunDic = this.GetInOutFunDic();
						if (inOutFunDic != null && inOutFunDic.Count > 0 && inOutFunDic.ContainsKey(machine.ID))
						{
							DataTable dataTable = new DataTable();
							dataTable.Columns.Add("Index", typeof(string));
							dataTable.Columns.Add("EventType", typeof(string));
							dataTable.Columns.Add("InAddr", typeof(string));
							dataTable.Columns.Add("OutType", typeof(string));
							dataTable.Columns.Add("OutAddr", typeof(string));
							dataTable.Columns.Add("OutTime", typeof(string));
							dataTable.Columns.Add("Reserved", typeof(string));
							this.ShowProgress(20);
							List<AccLinkAgeIo> list = inOutFunDic[machine.ID];
							for (int i = 0; i < list.Count; i++)
							{
								AccLinkAgeIo accLinkAgeIo = list[i];
								DataRow dataRow = dataTable.NewRow();
								dataRow["Index"] = accLinkAgeIo.id;
								dataRow["EventType"] = accLinkAgeIo.trigger_opt;
								dataRow["InAddr"] = accLinkAgeIo.in_address;
								dataRow["OutType"] = accLinkAgeIo.out_type_hide;
								dataRow["OutAddr"] = accLinkAgeIo.out_address;
								dataRow["OutTime"] = accLinkAgeIo.delay_time;
								dataTable.Rows.Add(dataRow);
								this.ShowProgress(20 + 60 * i / list.Count);
							}
							try
							{
								int num = RemovableDiskDataManager.ExportData(RemovableDiskDataManager.DiskDataType.InOutFun, dataTable, MachineDir);
								if (num >= 0)
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportInOutFunSucceed", "导出联动数据成功") + ":" + dataTable.Rows.Count);
								}
								else
								{
									this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportInOutFunFailed", "导出联动数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportInOutFunFailed", "导出联动数据失败") + ":" + ex.Message);
							}
						}
						else
						{
							this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("Count", "记录数") + ":0 ");
						}
						this.ShowProgress(100);
						this.ShowUpLoadInfo(machine.MachineAlias + " " + ShowMsgInfos.GetInfo("ExportInOutFunFinish", "导出联动数据完成") + "\r\n");
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ex2.Message);
			}
		}

		private void chk_userPrivilege_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chk_userPrivilege.Checked)
			{
				this.ckb_timeZone.Checked = true;
			}
		}

		private void ckb_FirstCard_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ckb_FirstCard.Checked)
			{
				this.ckb_timeZone.Checked = true;
			}
		}

		private void ckb_MultiCard_CheckedChanged(object sender, EventArgs e)
		{
			if (this.ckb_MultiCard.Checked)
			{
				this.ckb_timeZone.Checked = true;
			}
		}

		private void PCToUDisk_FormClosing(object sender, FormClosingEventArgs e)
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
					if (!string.IsNullOrEmpty(model[i].CardNo))
					{
						PersonnelIssuecard personnelIssuecard = new PersonnelIssuecard();
						personnelIssuecard.create_time = DateTime.Now;
						personnelIssuecard.UserID_id = model[i].UserId;
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

		private DriveInfo GetUDataDrive(List<DriveInfo> lstDrive)
		{
			DriveInfo result = null;
			if (lstDrive == null || lstDrive.Count <= 0)
			{
				return result;
			}
			result = lstDrive[0];
			for (int i = 0; i < lstDrive.Count; i++)
			{
				if (RemovableDiskDataManager.GetOptionDic(lstDrive[i]).Count > 0)
				{
					if (lstDrive[i].TotalFreeSpace > result.TotalFreeSpace)
					{
						result = lstDrive[i];
					}
				}
				else if (RemovableDiskDataManager.GetOptionDic(result).Count <= 0 && lstDrive[i].TotalFreeSpace > result.TotalFreeSpace)
				{
					result = lstDrive[i];
				}
			}
			return result;
		}

		private int EncodeTimeZone(DateTime? StartTime, DateTime? EndTime)
		{
			if (!StartTime.HasValue || !EndTime.HasValue)
			{
				return 0;
			}
			DateTime value = StartTime.Value;
			DateTime value2 = EndTime.Value;
			int num = value.Hour * 100 + value.Minute;
			int num2 = value2.Hour * 100 + value2.Minute;
			return num << 16 | num2;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PCToUDisk));
			this.pnl_bottom = new PanelEx();
			this.ckb_InOutFun = new CheckBox();
			this.ckb_MultiCard = new CheckBox();
			this.ckb_Holiday = new CheckBox();
			this.chk_userPrivilege = new CheckBox();
			this.chk_FV = new CheckBox();
			this.ckb_FaceTemp = new CheckBox();
			this.chk_FP = new CheckBox();
			this.ckb_FirstCard = new CheckBox();
			this.lblProgressAll = new Label();
			this.lblProgress = new Label();
			this.btn_show = new ButtonX();
			this.lb_progressAll = new LabelX();
			this.progress_all = new ProgressBar();
			this.ckb_timeZone = new CheckBox();
			this.lb_title = new LabelX();
			this.lb_progress = new LabelX();
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
			this.pnl_bottom.Controls.Add(this.ckb_InOutFun);
			this.pnl_bottom.Controls.Add(this.ckb_MultiCard);
			this.pnl_bottom.Controls.Add(this.ckb_Holiday);
			this.pnl_bottom.Controls.Add(this.chk_userPrivilege);
			this.pnl_bottom.Controls.Add(this.chk_FV);
			this.pnl_bottom.Controls.Add(this.ckb_FaceTemp);
			this.pnl_bottom.Controls.Add(this.chk_FP);
			this.pnl_bottom.Controls.Add(this.ckb_FirstCard);
			this.pnl_bottom.Controls.Add(this.lblProgressAll);
			this.pnl_bottom.Controls.Add(this.lblProgress);
			this.pnl_bottom.Controls.Add(this.btn_show);
			this.pnl_bottom.Controls.Add(this.lb_progressAll);
			this.pnl_bottom.Controls.Add(this.progress_all);
			this.pnl_bottom.Controls.Add(this.ckb_timeZone);
			this.pnl_bottom.Controls.Add(this.lb_title);
			this.pnl_bottom.Controls.Add(this.lb_progress);
			this.pnl_bottom.Controls.Add(this.chk_User);
			this.pnl_bottom.Controls.Add(this.btn_exit);
			this.pnl_bottom.Controls.Add(this.btn_down);
			this.pnl_bottom.Controls.Add(this.progressBarUp);
			this.pnl_bottom.Controls.Add(this.txt_UpLoadInfo);
			this.pnl_bottom.Dock = DockStyle.Fill;
			this.pnl_bottom.Location = new Point(0, 0);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(714, 449);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 13;
			this.ckb_InOutFun.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.ckb_InOutFun.AutoSize = true;
			this.ckb_InOutFun.Location = new Point(375, 105);
			this.ckb_InOutFun.Name = "ckb_InOutFun";
			this.ckb_InOutFun.Size = new Size(48, 16);
			this.ckb_InOutFun.TabIndex = 40;
			this.ckb_InOutFun.Text = "联动";
			this.ckb_InOutFun.UseVisualStyleBackColor = true;
			this.ckb_MultiCard.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.ckb_MultiCard.AutoSize = true;
			this.ckb_MultiCard.Location = new Point(208, 105);
			this.ckb_MultiCard.Name = "ckb_MultiCard";
			this.ckb_MultiCard.Size = new Size(72, 16);
			this.ckb_MultiCard.TabIndex = 41;
			this.ckb_MultiCard.Text = "多卡开门";
			this.ckb_MultiCard.UseVisualStyleBackColor = true;
			this.ckb_MultiCard.CheckedChanged += this.ckb_MultiCard_CheckedChanged;
			this.ckb_Holiday.AutoSize = true;
			this.ckb_Holiday.Location = new Point(375, 71);
			this.ckb_Holiday.Name = "ckb_Holiday";
			this.ckb_Holiday.Size = new Size(60, 16);
			this.ckb_Holiday.TabIndex = 38;
			this.ckb_Holiday.Text = "节假日";
			this.ckb_Holiday.UseVisualStyleBackColor = true;
			this.chk_userPrivilege.AutoSize = true;
			this.chk_userPrivilege.Location = new Point(208, 71);
			this.chk_userPrivilege.Name = "chk_userPrivilege";
			this.chk_userPrivilege.Size = new Size(72, 16);
			this.chk_userPrivilege.TabIndex = 2;
			this.chk_userPrivilege.Text = "门禁权限";
			this.chk_userPrivilege.UseVisualStyleBackColor = true;
			this.chk_userPrivilege.CheckedChanged += this.chk_userPrivilege_CheckedChanged;
			this.chk_FV.AutoSize = true;
			this.chk_FV.Location = new Point(527, 38);
			this.chk_FV.Name = "chk_FV";
			this.chk_FV.Size = new Size(78, 16);
			this.chk_FV.TabIndex = 34;
			this.chk_FV.Text = "指静脉   ";
			this.chk_FV.UseVisualStyleBackColor = true;
			this.ckb_FaceTemp.AutoSize = true;
			this.ckb_FaceTemp.Location = new Point(375, 38);
			this.ckb_FaceTemp.Name = "ckb_FaceTemp";
			this.ckb_FaceTemp.Size = new Size(72, 16);
			this.ckb_FaceTemp.TabIndex = 35;
			this.ckb_FaceTemp.Text = "面部数据";
			this.ckb_FaceTemp.UseVisualStyleBackColor = true;
			this.chk_FP.AutoSize = true;
			this.chk_FP.Checked = true;
			this.chk_FP.CheckState = CheckState.Checked;
			this.chk_FP.Location = new Point(208, 38);
			this.chk_FP.Name = "chk_FP";
			this.chk_FP.Size = new Size(72, 16);
			this.chk_FP.TabIndex = 1;
			this.chk_FP.Text = "指纹信息";
			this.chk_FP.UseVisualStyleBackColor = true;
			this.ckb_FirstCard.AutoSize = true;
			this.ckb_FirstCard.Location = new Point(41, 105);
			this.ckb_FirstCard.Name = "ckb_FirstCard";
			this.ckb_FirstCard.Size = new Size(72, 16);
			this.ckb_FirstCard.TabIndex = 39;
			this.ckb_FirstCard.Text = "首卡常开";
			this.ckb_FirstCard.UseVisualStyleBackColor = true;
			this.ckb_FirstCard.CheckedChanged += this.ckb_FirstCard_CheckedChanged;
			this.lblProgressAll.AutoSize = true;
			this.lblProgressAll.BackColor = Color.Transparent;
			this.lblProgressAll.Location = new Point(346, 219);
			this.lblProgressAll.Name = "lblProgressAll";
			this.lblProgressAll.Size = new Size(17, 12);
			this.lblProgressAll.TabIndex = 37;
			this.lblProgressAll.Text = "0%";
			this.lblProgress.AutoSize = true;
			this.lblProgress.BackColor = Color.Transparent;
			this.lblProgress.Location = new Point(346, 170);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new Size(17, 12);
			this.lblProgress.TabIndex = 36;
			this.lblProgress.Text = "0%";
			this.btn_show.AccessibleRole = AccessibleRole.PushButton;
			this.btn_show.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_show.Location = new Point(12, 246);
			this.btn_show.Name = "btn_show";
			this.btn_show.Size = new Size(230, 23);
			this.btn_show.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_show.TabIndex = 33;
			this.btn_show.Text = "隐藏信息";
			this.btn_show.Click += this.btn_show_Click;
			this.lb_progressAll.BackgroundStyle.Class = "";
			this.lb_progressAll.Location = new Point(12, 191);
			this.lb_progressAll.Name = "lb_progressAll";
			this.lb_progressAll.Size = new Size(209, 18);
			this.lb_progressAll.TabIndex = 32;
			this.lb_progressAll.Text = "总体进度";
			this.progress_all.Location = new Point(12, 213);
			this.progress_all.Name = "progress_all";
			this.progress_all.Size = new Size(690, 23);
			this.progress_all.TabIndex = 31;
			this.ckb_timeZone.AutoSize = true;
			this.ckb_timeZone.Location = new Point(41, 71);
			this.ckb_timeZone.Name = "ckb_timeZone";
			this.ckb_timeZone.Size = new Size(84, 16);
			this.ckb_timeZone.TabIndex = 3;
			this.ckb_timeZone.Text = "门禁时间段";
			this.ckb_timeZone.UseVisualStyleBackColor = true;
			this.lb_title.AutoSize = true;
			this.lb_title.BackgroundStyle.Class = "";
			this.lb_title.Location = new Point(12, 14);
			this.lb_title.Name = "lb_title";
			this.lb_title.Size = new Size(112, 18);
			this.lb_title.TabIndex = 10;
			this.lb_title.Text = "导出以下数据到U盘";
			this.lb_progress.BackgroundStyle.Class = "";
			this.lb_progress.Location = new Point(12, 142);
			this.lb_progress.Name = "lb_progress";
			this.lb_progress.Size = new Size(209, 18);
			this.lb_progress.TabIndex = 9;
			this.lb_progress.Text = "当前设备进度";
			this.chk_User.AutoSize = true;
			this.chk_User.Checked = true;
			this.chk_User.CheckState = CheckState.Checked;
			this.chk_User.Location = new Point(41, 38);
			this.chk_User.Name = "chk_User";
			this.chk_User.Size = new Size(72, 16);
			this.chk_User.TabIndex = 0;
			this.chk_User.Text = "用户信息";
			this.chk_User.UseVisualStyleBackColor = true;
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(560, 246);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(142, 23);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 5;
			this.btn_exit.Text = "返回";
			this.btn_exit.Click += this.btn_exit_Click;
			this.btn_down.AccessibleRole = AccessibleRole.PushButton;
			this.btn_down.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_down.Location = new Point(398, 246);
			this.btn_down.Name = "btn_down";
			this.btn_down.Size = new Size(142, 23);
			this.btn_down.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_down.TabIndex = 4;
			this.btn_down.Text = "导出";
			this.btn_down.Click += this.btn_down_Click;
			this.progressBarUp.Location = new Point(12, 165);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(690, 23);
			this.progressBarUp.TabIndex = 7;
			this.txt_UpLoadInfo.Location = new Point(12, 279);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(690, 158);
			this.txt_UpLoadInfo.TabIndex = 8;
			this.timer1.Interval = 1000;
			this.timer1.Tick += this.timer1_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(714, 449);
			base.Controls.Add(this.pnl_bottom);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PCToUDisk";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "导出数据到U盘";
			base.FormClosing += this.PCToUDisk_FormClosing;
			this.pnl_bottom.ResumeLayout(false);
			this.pnl_bottom.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
