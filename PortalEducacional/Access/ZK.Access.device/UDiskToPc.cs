/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.UDisk;
using ZK.Data.Model;
using ZK.MachinesManager.UDisk;
using ZK.Utils;

namespace ZK.Access.device
{
	public class UDiskToPc : Office2007Form
	{
		private int defaultDeptId = 0;

		private int FpVersionImported;

		private bool ThreadRunning;

		private WaitForm frmWait = WaitForm.Instance;

		private DataTable dtUserInfo;

		private Dictionary<string, UserInfo> dicPin_User;

		private Dictionary<int, string> dicUserId_Pin;

		private Dictionary<string, string> dicCardNo_Pin;

		private Dictionary<int, Dictionary<int, int>> dicUserIdFingerId_TemplateId;

		private Dictionary<int, Dictionary<int, int>> STDdicUserId_FaceIdDic;

		private Dictionary<int, Dictionary<int, int>> PullDicUserId_FaceIdDic;

		private Dictionary<string, Dictionary<int, int>> dicPinFingerId_FVId;

		private Dictionary<string, Dictionary<int, Template>> dicPin_dicFingerId_FP;

		private Dictionary<string, Dictionary<int, FaceTemp>> STDdicPin_dicFaceId_Face;

		private Dictionary<string, Dictionary<int, FaceTemp>> PullDicPin_dicFaceId_Face;

		private Dictionary<string, Dictionary<int, List<FingerVein>>> dicPin_dicFingerId_lstFV;

		private Dictionary<int, string> dicUID_Pin_Std;

		private Dictionary<string, UserInfo> dicPin_User_Imported;

		private IContainer components = null;

		private GridControl gridControl;

		private GridView grdview_UserInfo;

		private Panel panel2;

		private CheckBox ckb_FaceTemp;

		private CheckBox ckb_User;

		private CheckBox ckb_FP;

		private CheckBox ckb_FV;

		private GridColumn colCheck;

		private GridColumn colPin;

		private GridColumn colName;

		private GridColumn colCardNo;

		private GridColumn colFPCount;

		private GridColumn colFVCount;

		private GridColumn colFaceCount;

		private GroupBox gpbUserType;

		private GroupBox gpbFpVersion;

		private RadioButton rdbTFT;

		private RadioButton rdbPin9;

		private RadioButton rdbPin5;

		private RadioButton rdbFP9;

		private RadioButton rdbFP10;

		private ButtonX btnSave;

		private ButtonX btnImport;

		private ButtonX btnExit;

		private Panel panel3;

		private GroupBox groupBox1;

		private GridColumn colStdFaceCount;

		public UDiskToPc()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			DevExpressHelper.InitImageList(this.grdview_UserInfo, "colCheck");
			this.InitializeDataSource();
			this.LoadUserInDB();
			this.LoadDefaultID();
			this.LoadTemplateInDB();
			this.LoadFaceInDB();
			this.LoadFvInDB();
		}

		private void InitializeDataSource()
		{
			this.dtUserInfo = new DataTable();
			this.dtUserInfo.Columns.Add("Check", typeof(bool));
			this.dtUserInfo.Columns.Add("UID", typeof(int));
			this.dtUserInfo.Columns.Add("Pin", typeof(uint));
			this.dtUserInfo.Columns.Add("Name", typeof(string));
			this.dtUserInfo.Columns.Add("CardNo", typeof(string));
			this.dtUserInfo.Columns.Add("FPCount", typeof(int));
			this.dtUserInfo.Columns.Add("FVCount", typeof(int));
			this.dtUserInfo.Columns.Add("FaceCount", typeof(int));
			this.dtUserInfo.Columns.Add("StdFaceCount", typeof(int));
			this.dtUserInfo.Columns.Add("Password", typeof(string));
			this.dtUserInfo.Columns.Add("Privilege", typeof(int));
			this.dtUserInfo.Columns.Add("Group", typeof(int));
			this.dtUserInfo.Columns.Add("AccStartDate", typeof(string));
			this.dtUserInfo.Columns.Add("AccEndDate", typeof(string));
			this.colCardNo.FieldName = "CardNo";
			this.colCheck.FieldName = "Check";
			this.colFaceCount.FieldName = "FaceCount";
			this.colStdFaceCount.FieldName = "StdFaceCount";
			this.colFPCount.FieldName = "FPCount";
			this.colFVCount.FieldName = "FVCount";
			this.colName.FieldName = "Name";
			this.colPin.FieldName = "Pin";
			this.grdview_UserInfo.OptionsView.ShowFooter = true;
			this.colFPCount.SummaryItem.FieldName = "FPCount";
			this.colFPCount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.colFVCount.SummaryItem.FieldName = "FVCount";
			this.colFVCount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.colFaceCount.SummaryItem.FieldName = "FaceCount";
			this.colFaceCount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.colStdFaceCount.SummaryItem.FieldName = "StdFaceCount";
			this.colStdFaceCount.SummaryItem.SummaryType = SummaryItemType.Sum;
			this.gridControl.DataSource = this.dtUserInfo;
		}

		private void LoadUserInDB()
		{
			try
			{
				this.dicUserId_Pin = new Dictionary<int, string>();
				this.dicCardNo_Pin = new Dictionary<string, string>();
				this.dicPin_User = new Dictionary<string, UserInfo>();
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				List<UserInfo> modelList = userInfoBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dicPin_User.ContainsKey(modelList[i].BadgeNumber))
						{
							this.dicPin_User.Add(modelList[i].BadgeNumber, modelList[i]);
							this.dicUserId_Pin.Add(modelList[i].UserId, modelList[i].BadgeNumber);
							if (modelList[i].CardNo != null && modelList[i].CardNo.Trim() != "" && !this.dicCardNo_Pin.ContainsKey(modelList[i].CardNo))
							{
								this.dicCardNo_Pin.Add(modelList[i].CardNo, modelList[i].BadgeNumber);
							}
						}
						else
						{
							this.dicPin_User[modelList[i].BadgeNumber] = modelList[i];
							this.dicUserId_Pin.Add(modelList[i].UserId, modelList[i].BadgeNumber);
							if (modelList[i].CardNo != null && modelList[i].CardNo.Trim() != "" && !this.dicCardNo_Pin.ContainsKey(modelList[i].CardNo))
							{
								this.dicCardNo_Pin.Add(modelList[i].CardNo, modelList[i].BadgeNumber);
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

		private void LoadTemplateInDB()
		{
			try
			{
				this.dicUserIdFingerId_TemplateId = new Dictionary<int, Dictionary<int, int>>();
				TemplateBll templateBll = new TemplateBll(MainForm._ia);
				DataTable fields = templateBll.GetFields("USERID,FINGERID,TEMPLATEID", "");
				if (fields != null && fields.Rows.Count > 0)
				{
					for (int i = 0; i < fields.Rows.Count; i++)
					{
						if (int.TryParse(fields.Rows[i]["USERID"].ToString(), out int key) && int.TryParse(fields.Rows[i]["FINGERID"].ToString(), out int key2) && int.TryParse(fields.Rows[i]["TEMPLATEID"].ToString(), out int value))
						{
							if (this.dicUserIdFingerId_TemplateId.ContainsKey(key))
							{
								Dictionary<int, int> dictionary = this.dicUserIdFingerId_TemplateId[key];
								if (!dictionary.ContainsKey(key2))
								{
									dictionary.Add(key2, value);
								}
							}
							else
							{
								Dictionary<int, int> dictionary = new Dictionary<int, int>();
								dictionary.Add(key2, value);
								this.dicUserIdFingerId_TemplateId.Add(key, dictionary);
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

		private void LoadFaceInDB()
		{
			try
			{
				this.STDdicUserId_FaceIdDic = new Dictionary<int, Dictionary<int, int>>();
				this.PullDicUserId_FaceIdDic = new Dictionary<int, Dictionary<int, int>>();
				FaceTempBll faceTempBll = new FaceTempBll(MainForm._ia);
				DataTable fields = faceTempBll.GetFields("USERNO,FACEID,TEMPLATEID,FaceType", "");
				for (int i = 0; i < fields.Rows.Count; i++)
				{
					DataRow dataRow = fields.Rows[i];
					int.TryParse(dataRow["USERNO"].ToString(), out int key);
					int.TryParse(dataRow["FACEID"].ToString(), out int key2);
					int.TryParse(dataRow["TEMPLATEID"].ToString(), out int value);
					byte.TryParse(dataRow["FaceType"].ToString(), out byte b);
					Dictionary<int, Dictionary<int, int>> dictionary = (b == 1) ? this.STDdicUserId_FaceIdDic : this.PullDicUserId_FaceIdDic;
					if (!dictionary.ContainsKey(key))
					{
						Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
						dictionary2.Add(key2, value);
						dictionary.Add(key, dictionary2);
					}
					else
					{
						Dictionary<int, int> dictionary2 = dictionary[key];
						if (!dictionary2.ContainsKey(key2))
						{
							dictionary2.Add(key2, value);
						}
						else
						{
							dictionary2[key2] = value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private void LoadDefaultID()
		{
			if (this.defaultDeptId == 0)
			{
				DepartmentsBll departmentsBll = new DepartmentsBll(MainForm._ia);
				List<Departments> modelList = departmentsBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					this.defaultDeptId = modelList[0].DEPTID;
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
					this.defaultDeptId = modelList[num].DEPTID;
				}
			}
		}

		private void LoadFvInDB()
		{
			try
			{
				this.dicPinFingerId_FVId = new Dictionary<string, Dictionary<int, int>>();
				FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
				DataTable fields = fingerVeinBll.GetFields("UserID,FingerID,FVID", "");
				if (fields != null && fields.Rows.Count > 0)
				{
					for (int i = 0; i < fields.Rows.Count; i++)
					{
						int.TryParse(fields.Rows[i]["UserID"].ToString(), out int key);
						int.TryParse(fields.Rows[i]["FingerID"].ToString(), out int key2);
						int.TryParse(fields.Rows[i]["FVID"].ToString(), out int value);
						if (this.dicUserId_Pin.ContainsKey(key))
						{
							string key3 = this.dicUserId_Pin[key];
							if (!this.dicPinFingerId_FVId.ContainsKey(key3))
							{
								Dictionary<int, int> dictionary = new Dictionary<int, int>();
								dictionary.Add(key2, value);
								this.dicPinFingerId_FVId.Add(key3, dictionary);
							}
							else
							{
								Dictionary<int, int> dictionary = this.dicPinFingerId_FVId[key3];
								if (!dictionary.ContainsKey(key2))
								{
									dictionary.Add(key2, value);
								}
								else
								{
									dictionary[key2] = value;
								}
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

		private void grdview_UserInfo_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "Check");
		}

		private void grdview_UserInfo_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "Check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grdview_UserInfo_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "Check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void ckb_User_CheckedChanged(object sender, EventArgs e)
		{
			this.gpbUserType.Enabled = this.ckb_User.Checked;
		}

		private void ckb_FP_CheckedChanged(object sender, EventArgs e)
		{
			this.gpbFpVersion.Enabled = this.ckb_FP.Checked;
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void UDiskToPcEx_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.ThreadRunning)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("WaitForThreadExit", "请等待当前操作完成"));
				e.Cancel = true;
			}
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			if (!this.ckb_User.Checked && !this.ckb_FP.Checked && !this.ckb_FV.Checked && !this.ckb_FaceTemp.Checked)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ChooseDataNeedToDown", "请选择需要下载的数据"));
			}
			else if (RemovableDiskDataManager.GetRemovableDiskList().Count <= 0)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("NoUDisk", "没有U盘设备"));
			}
			else
			{
				Thread thread = new Thread(this.StartImport);
				this.frmWait.ShowEx();
				thread.Start();
			}
		}

		private void StartImport()
		{
			this.ThreadRunning = true;
			try
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.btnImport.Enabled = false;
					this.btnExit.Enabled = false;
					this.btnSave.Enabled = false;
				});
				bool @checked = this.rdbTFT.Checked;
				int pinWidth = this.rdbPin5.Checked ? 5 : 9;
				int fpVersion = this.rdbFP10.Checked ? 10 : 9;
				this.dicUID_Pin_Std = new Dictionary<int, string>();
				this.frmWait.ShowProgress(0);
				this.ImportUser(pinWidth, @checked, out this.dicPin_User_Imported);
				this.frmWait.ShowProgress(20);
				this.ImportTemplate(fpVersion, out this.dicPin_dicFingerId_FP);
				this.frmWait.ShowProgress(40);
				this.STDdicPin_dicFaceId_Face = new Dictionary<string, Dictionary<int, FaceTemp>>();
				this.PullDicPin_dicFaceId_Face = new Dictionary<string, Dictionary<int, FaceTemp>>();
				this.ImportFaceTemp(out this.STDdicPin_dicFaceId_Face);
				this.frmWait.ShowProgress(60);
				this.dicPin_dicFingerId_lstFV = new Dictionary<string, Dictionary<int, List<FingerVein>>>();
				this.ImportFingerVein(out this.dicPin_dicFingerId_lstFV);
				this.frmWait.ShowProgress(80);
				base.Invoke((MethodInvoker)delegate
				{
					this.gridControl.DataSource = null;
				});
				this.dtUserInfo.Rows.Clear();
				foreach (KeyValuePair<string, UserInfo> item in this.dicPin_User_Imported)
				{
					ulong.TryParse(item.Value.CardNo, out ulong num);
					DataRow dataRow = this.dtUserInfo.NewRow();
					dataRow["Check"] = true;
					dataRow["UID"] = item.Value.UserId;
					dataRow["Pin"] = item.Value.BadgeNumber;
					dataRow["Name"] = item.Value.Name;
					if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
					{
						dataRow["CardNo"] = num.ToString("X");
					}
					else
					{
						dataRow["CardNo"] = item.Value.CardNo;
					}
					dataRow["FPCount"] = (this.dicPin_dicFingerId_FP.ContainsKey(item.Value.BadgeNumber) ? this.dicPin_dicFingerId_FP[item.Value.BadgeNumber].Count : 0);
					dataRow["FVCount"] = (this.dicPin_dicFingerId_lstFV.ContainsKey(item.Value.BadgeNumber) ? this.dicPin_dicFingerId_lstFV[item.Value.BadgeNumber].Count : 0);
					dataRow["FaceCount"] = (this.PullDicPin_dicFaceId_Face.ContainsKey(item.Value.BadgeNumber) ? 1 : 0);
					dataRow["StdFaceCount"] = (this.STDdicPin_dicFaceId_Face.ContainsKey(item.Value.BadgeNumber) ? 1 : 0);
					dataRow["Password"] = item.Value.PassWord;
					dataRow["Privilege"] = item.Value.Privilege;
					dataRow["Group"] = item.Value.MorecardGroupId;
					DateTime value;
					if (!item.Value.AccStartDate.HasValue)
					{
						dataRow["AccStartDate"] = "";
					}
					else
					{
						DataRow dataRow2 = dataRow;
						value = item.Value.AccStartDate.Value;
						dataRow2["AccStartDate"] = value.ToString("yyyy-MM-dd HH:mm:ss");
					}
					if (!item.Value.AccStartDate.HasValue)
					{
						dataRow["AccEndDate"] = "";
					}
					else
					{
						DataRow dataRow3 = dataRow;
						value = item.Value.AccEndDate.Value;
						dataRow3["AccEndDate"] = value.ToString("yyyy-MM-dd HH:mm:ss");
					}
					this.dtUserInfo.Rows.Add(dataRow);
				}
			}
			catch (Exception)
			{
				this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("ImportUDataError", "导入数据出错，请检查选择的格式是否正确"));
			}
			this.ThreadRunning = false;
			this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("ImportDataFinished", "导入数据完成"));
			this.frmWait.ShowProgress(100);
			this.frmWait.HideEx(false);
			base.Invoke((MethodInvoker)delegate
			{
				this.gridControl.DataSource = this.dtUserInfo;
				this.btnImport.Enabled = true;
				this.btnExit.Enabled = true;
				this.btnSave.Enabled = true;
				this.FpVersionImported = (this.rdbFP10.Checked ? 10 : 9);
			});
		}

		private void ImportUser(int PinWidth, bool IsTFT, out Dictionary<string, UserInfo> dicUser)
		{
			dicUser = new Dictionary<string, UserInfo>();
			if (this.ckb_User.Checked)
			{
				this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("ImportingUserInfo", "正在导入用户数据..."));
				List<UserInfo> list = new List<UserInfo>();
				UDiskBll uDiskBll = new UDiskBll(SDKType.StandaloneSDK);
				uDiskBll.ImportUserInfo(out List<UserInfo> list2, PinWidth, IsTFT);
				list.AddRange(list2);
				for (int i = 0; i < list2.Count; i++)
				{
					UserInfo userInfo = list2[i];
					if (!this.dicUID_Pin_Std.ContainsKey(userInfo.UserId))
					{
						this.dicUID_Pin_Std.Add(userInfo.UserId, userInfo.BadgeNumber);
					}
				}
				this.frmWait.ShowProgress(5);
				uDiskBll = new UDiskBll(SDKType.PullSDK);
				uDiskBll.ImportUserInfo(out list2, PinWidth, IsTFT);
				list.AddRange(list2);
				this.frmWait.ShowProgress(10);
				for (int j = 0; j < list.Count; j++)
				{
					UserInfo userInfo = list[j];
					userInfo.MorecardGroupId = 0;
					if (dicUser.ContainsKey(userInfo.BadgeNumber))
					{
						dicUser[userInfo.BadgeNumber] = userInfo;
					}
					else
					{
						dicUser.Add(userInfo.BadgeNumber, userInfo);
					}
				}
			}
		}

		private void ImportTemplate(int FpVersion, out Dictionary<string, Dictionary<int, Template>> dicPin_Template)
		{
			dicPin_Template = new Dictionary<string, Dictionary<int, Template>>();
			if (this.ckb_FP.Checked)
			{
				this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("ImportingTemplate", "正在导入指纹数据..."));
				UDiskBll uDiskBll = new UDiskBll(SDKType.StandaloneSDK);
				uDiskBll.ImportFingerPrint(FpVersion, out List<Template> list);
				for (int i = 0; i < list.Count; i++)
				{
					Template template = list[i];
					string key = (!this.dicUID_Pin_Std.ContainsKey(template.USERID)) ? "0" : this.dicUID_Pin_Std[template.USERID];
					if (dicPin_Template.ContainsKey(key))
					{
						Dictionary<int, Template> dictionary = dicPin_Template[key];
						if (dictionary.ContainsKey(template.FINGERID))
						{
							dictionary[template.FINGERID] = template;
						}
						else
						{
							dictionary.Add(template.FINGERID, template);
						}
					}
					else
					{
						Dictionary<int, Template> dictionary = new Dictionary<int, Template>();
						dictionary.Add(template.FINGERID, template);
						dicPin_Template.Add(key, dictionary);
					}
				}
				this.frmWait.ShowProgress(25);
				uDiskBll = new UDiskBll(SDKType.PullSDK);
				uDiskBll.ImportFingerPrint(FpVersion, out list);
				this.frmWait.ShowProgress(30);
				for (int j = 0; j < list.Count; j++)
				{
					Template template = list[j];
					if (dicPin_Template.ContainsKey(template.Pin))
					{
						Dictionary<int, Template> dictionary = dicPin_Template[template.Pin];
						if (dictionary.ContainsKey(template.FINGERID))
						{
							dictionary[template.FINGERID] = template;
						}
						else
						{
							dictionary.Add(template.FINGERID, template);
						}
					}
					else
					{
						Dictionary<int, Template> dictionary = new Dictionary<int, Template>();
						dictionary.Add(template.FINGERID, template);
						dicPin_Template.Add(template.Pin, dictionary);
					}
				}
			}
		}

		private void ImportFaceTemp(out Dictionary<string, Dictionary<int, FaceTemp>> dicUID_FaceTemp)
		{
			List<FaceTemp> list = new List<FaceTemp>();
			dicUID_FaceTemp = new Dictionary<string, Dictionary<int, FaceTemp>>();
			if (this.ckb_FaceTemp.Checked)
			{
				this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("ImportingFace", "正在导入人脸数据..."));
				UDiskBll uDiskBll = new UDiskBll(SDKType.StandaloneSDK);
				uDiskBll.ImportFaceTemplate(out List<FaceTemp> list2);
				for (int i = 0; i < list2.Count; i++)
				{
					FaceTemp faceTemp = list2[i];
					int userId = faceTemp.UserId;
					string key = (!this.dicUID_Pin_Std.ContainsKey(userId)) ? "0" : this.dicUID_Pin_Std[userId];
					Dictionary<string, Dictionary<int, FaceTemp>> dictionary = (faceTemp.FaceType == 1) ? this.STDdicPin_dicFaceId_Face : this.PullDicPin_dicFaceId_Face;
					if (!dictionary.ContainsKey(key))
					{
						Dictionary<int, FaceTemp> dictionary2 = new Dictionary<int, FaceTemp>();
						dictionary2.Add(faceTemp.FaceId, faceTemp);
						dictionary.Add(key, dictionary2);
					}
					else
					{
						Dictionary<int, FaceTemp> dictionary2 = dictionary[key];
						if (!dictionary2.ContainsKey(faceTemp.FaceId))
						{
							dictionary2.Add(faceTemp.FaceId, faceTemp);
						}
						else
						{
							dictionary2[faceTemp.FaceId] = faceTemp;
						}
					}
				}
				list.AddRange(list2);
				this.frmWait.ShowProgress(25);
				uDiskBll = new UDiskBll(SDKType.PullSDK);
				uDiskBll.ImportFaceTemplate(out list2);
				list.AddRange(list2);
				this.frmWait.ShowProgress(30);
				for (int j = 0; j < list2.Count; j++)
				{
					FaceTemp faceTemp = list2[j];
					int userId = faceTemp.UserId;
					string key = faceTemp.Pin.ToString();
					Dictionary<string, Dictionary<int, FaceTemp>> dictionary = (faceTemp.FaceType == 1) ? this.STDdicPin_dicFaceId_Face : this.PullDicPin_dicFaceId_Face;
					if (!dictionary.ContainsKey(key))
					{
						Dictionary<int, FaceTemp> dictionary2 = new Dictionary<int, FaceTemp>();
						dictionary2.Add(faceTemp.FaceId, faceTemp);
						dictionary.Add(key, dictionary2);
					}
					else
					{
						Dictionary<int, FaceTemp> dictionary2 = dictionary[key];
						if (!dictionary2.ContainsKey(faceTemp.FaceId))
						{
							dictionary2.Add(faceTemp.FaceId, faceTemp);
						}
						else
						{
							dictionary2[faceTemp.FaceId] = faceTemp;
						}
					}
				}
			}
		}

		private void ImportFingerVein(out Dictionary<string, Dictionary<int, List<FingerVein>>> dicPinFingerId_lstFV)
		{
			List<FingerVein> list = new List<FingerVein>();
			dicPinFingerId_lstFV = new Dictionary<string, Dictionary<int, List<FingerVein>>>();
			if (this.ckb_FV.Checked)
			{
				this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("ImportingFingerVein", "正在导入指静脉数据..."));
				UDiskBll uDiskBll = new UDiskBll(SDKType.StandaloneSDK);
				uDiskBll.ImportFingerVein(out List<FingerVein> collection);
				list.AddRange(collection);
				this.frmWait.ShowProgress(65);
				uDiskBll = new UDiskBll(SDKType.PullSDK);
				uDiskBll.ImportFingerVein(out collection);
				list.AddRange(collection);
				this.frmWait.ShowProgress(70);
				for (int i = 0; i < list.Count; i++)
				{
					FingerVein fingerVein = list[i];
					if (fingerVein.Pin != null && !(fingerVein.Pin.Trim() == ""))
					{
						if (!dicPinFingerId_lstFV.ContainsKey(fingerVein.Pin))
						{
							List<FingerVein> list2 = new List<FingerVein>();
							Dictionary<int, List<FingerVein>> dictionary = new Dictionary<int, List<FingerVein>>();
							list2.Add(fingerVein);
							dictionary.Add(fingerVein.FingerID, list2);
							dicPinFingerId_lstFV.Add(fingerVein.Pin, dictionary);
						}
						else
						{
							Dictionary<int, List<FingerVein>> dictionary = dicPinFingerId_lstFV[fingerVein.Pin];
							if (!dictionary.ContainsKey(fingerVein.FingerID))
							{
								List<FingerVein> list2 = new List<FingerVein>();
								list2.Add(fingerVein);
								dictionary.Add(fingerVein.FingerID, list2);
							}
							else
							{
								bool flag = false;
								List<FingerVein> list2 = dictionary[fingerVein.FingerID];
								int num = 0;
								while (num < list2.Count)
								{
									if (list2[num].Fv_ID_Index != fingerVein.Fv_ID_Index)
									{
										num++;
										continue;
									}
									flag = true;
									break;
								}
								if (flag)
								{
									list2[num] = fingerVein;
								}
								else
								{
									list2.Add(fingerVein);
								}
							}
						}
					}
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			DataRow[] array = this.dtUserInfo.Select("Check=true");
			if (array == null || array.Length == 0)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("NoRowSelected", "请选择需要导入的记录"));
			}
			else
			{
				Thread thread = new Thread(this.Save);
				this.frmWait.ShowEx();
				thread.Start(array);
			}
		}

		private void Save(object SelectedRows)
		{
			this.ThreadRunning = true;
			base.Invoke((MethodInvoker)delegate
			{
				this.btnExit.Enabled = false;
				this.btnImport.Enabled = false;
				this.btnSave.Enabled = false;
			});
			try
			{
				DataRow[] array = SelectedRows as DataRow[];
				if (array == null || array.Length == 0)
				{
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("NoRowSelected", "请选择需要导入的记录"));
				}
				else
				{
					this.frmWait.ShowProgress(3);
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("SavingUserInfo", "正在保存用户数据..."));
					this.SaveUser(array);
					this.frmWait.ShowProgress(30);
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("SavingTemplate", "正在保存指纹数据..."));
					this.SaveTemplate(array);
					this.frmWait.ShowProgress(50);
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("SavingFace", "正在保存人脸数据..."));
					this.SaveFaceTemplate(array);
					this.frmWait.ShowProgress(70);
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("SavingFingerVein", "正在保存指静脉数据..."));
					this.SaveFingerVein(array);
					this.frmWait.ShowProgress(90);
				}
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
			}
			base.Invoke((MethodInvoker)delegate
			{
				this.btnExit.Enabled = true;
				this.btnImport.Enabled = true;
				this.btnSave.Enabled = true;
			});
			this.ThreadRunning = false;
			this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("SaveDataFinished", "保存数据完成"));
			this.frmWait.ShowProgress(100);
			this.frmWait.HideEx(false);
		}

		private void SaveUser(DataRow[] drSelected)
		{
			Dictionary<string, UserInfo> dictionary = new Dictionary<string, UserInfo>();
			try
			{
				if (drSelected != null || drSelected.Length != 0)
				{
					List<UserInfo> list = new List<UserInfo>();
					List<UserInfo> list2 = new List<UserInfo>();
					List<UserInfo> list3 = new List<UserInfo>();
					List<UserInfo> list4 = new List<UserInfo>();
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					for (int i = 0; i < drSelected.Length; i++)
					{
						string text = drSelected[i]["Pin"].ToString();
						string text2 = drSelected[i]["Name"].ToString();
						string text3 = drSelected[i]["CardNo"].ToString();
						string passWord = drSelected[i]["Password"].ToString();
						int.TryParse(drSelected[i]["Group"].ToString(), out int morecardGroupId);
						int.TryParse(drSelected[i]["Privilege"].ToString(), out int privilege);
						DateTime.TryParse(drSelected[i]["AccStartDate"].ToString(), out DateTime value);
						DateTime.TryParse(drSelected[i]["AccEndDate"].ToString(), out DateTime value2);
						if (this.dicPin_User.ContainsKey(text))
						{
							UserInfo userInfo = this.dicPin_User[text];
							if ((userInfo.Name == null || userInfo.Name.Trim() == "") && text2.Trim() != "")
							{
								userInfo.Name = text2;
								list3.Add(userInfo);
							}
							if (!string.IsNullOrEmpty(text3))
							{
								if (this.dicCardNo_Pin.ContainsKey(text3))
								{
									UserInfo userInfo2 = this.dicPin_User[this.dicCardNo_Pin[text3]];
									if (userInfo2.BadgeNumber != userInfo.BadgeNumber)
									{
										userInfo2.CardNo = "";
										if (!list3.Contains(userInfo2))
										{
											list3.Add(userInfo2);
										}
										if (!list4.Contains(userInfo2))
										{
											list4.Add(userInfo2);
										}
										this.dicCardNo_Pin.Remove(text3);
									}
								}
								if (userInfo.CardNo != text3)
								{
									if (userInfo.CardNo != null && this.dicCardNo_Pin.ContainsKey(userInfo.CardNo))
									{
										this.dicCardNo_Pin.Remove(userInfo.CardNo);
									}
									userInfo.CardNo = text3;
									if (this.dicCardNo_Pin.ContainsKey(text3))
									{
										this.dicCardNo_Pin[text3] = userInfo.BadgeNumber;
									}
									else
									{
										this.dicCardNo_Pin.Add(text3, userInfo.BadgeNumber);
									}
									list2.Add(userInfo);
									if (!list3.Contains(userInfo))
									{
										list3.Add(userInfo);
									}
								}
							}
						}
						else
						{
							UserInfo userInfo = new UserInfo();
							userInfo.DefaultDeptId = this.defaultDeptId;
							userInfo.BadgeNumber = text;
							userInfo.Name = text2;
							userInfo.CardNo = text3;
							userInfo.PassWord = passWord;
							userInfo.MorecardGroupId = morecardGroupId;
							userInfo.Privilege = privilege;
							if (drSelected[i]["AccStartDate"].ToString().Trim() != "")
							{
								userInfo.AccStartDate = value;
							}
							if (drSelected[i]["AccEndDate"].ToString().Trim() != "")
							{
								userInfo.AccEndDate = value2;
							}
							list.Add(userInfo);
							if (!string.IsNullOrEmpty(text3))
							{
								if (this.dicCardNo_Pin.ContainsKey(text3) && this.dicPin_User.ContainsKey(this.dicCardNo_Pin[text3]))
								{
									UserInfo userInfo3 = this.dicPin_User[this.dicCardNo_Pin[text3]];
									if (userInfo3.BadgeNumber != userInfo.BadgeNumber)
									{
										userInfo3.CardNo = "";
										if (!list3.Contains(userInfo3))
										{
											list3.Add(userInfo3);
										}
										if (!list4.Contains(userInfo3))
										{
											list4.Add(userInfo3);
										}
										this.dicCardNo_Pin.Remove(text3);
									}
								}
								list2.Add(userInfo);
								if (dictionary.ContainsKey(text3))
								{
									dictionary[text3].CardNo = "";
									if (list2.Contains(dictionary[text3]))
									{
										list2.Remove(dictionary[text3]);
									}
									dictionary.Remove(text3);
								}
								dictionary.Add(text3, userInfo);
							}
						}
					}
					this.frmWait.ShowProgress(5);
					userInfoBll.Add(list, null);
					userInfoBll.Update(list3);
					this.frmWait.ShowProgress(10);
					this.LoadUserInDB();
					this.RemoveCard(list4);
					this.SaveCard(list2);
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("UserCount", "用户数") + ": " + drSelected.Length);
				}
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
			}
		}

		private void RemoveCard(List<UserInfo> lstUser)
		{
			int num = 10000;
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < lstUser.Count; i++)
				{
					if (!string.IsNullOrEmpty(lstUser[i].CardNo) && this.dicPin_User.ContainsKey(lstUser[i].BadgeNumber))
					{
						UserInfo userInfo = this.dicPin_User[lstUser[i].BadgeNumber];
						stringBuilder.Append(lstUser[i].UserId + ",");
						if ((i + 1) % num == 0)
						{
							string text = stringBuilder.ToString();
							personnelIssuecardBll.DeleteListByUserIDs(text.Substring(0, text.Length - 1));
							stringBuilder = new StringBuilder();
						}
					}
				}
				if (stringBuilder.Length > 0)
				{
					string text = stringBuilder.ToString();
					personnelIssuecardBll.DeleteListByUserIDs(text.Substring(0, text.Length - 1));
					stringBuilder = new StringBuilder();
				}
				this.frmWait.ShowProgress(13);
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
			}
		}

		private void SaveCard(List<UserInfo> lstModel)
		{
			int num = 10000;
			try
			{
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				List<PersonnelIssuecard> list = new List<PersonnelIssuecard>();
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < lstModel.Count; i++)
				{
					if (!string.IsNullOrEmpty(lstModel[i].CardNo) && this.dicPin_User.ContainsKey(lstModel[i].BadgeNumber))
					{
						UserInfo userInfo = this.dicPin_User[lstModel[i].BadgeNumber];
						PersonnelIssuecard personnelIssuecard = new PersonnelIssuecard();
						personnelIssuecard.create_time = DateTime.Now;
						personnelIssuecard.UserID_id = userInfo.UserId;
						personnelIssuecard.cardno = lstModel[i].CardNo;
						list.Add(personnelIssuecard);
						stringBuilder.Append(lstModel[i].UserId + ",");
						if ((i + 1) % num == 0)
						{
							string text = stringBuilder.ToString();
							personnelIssuecardBll.DeleteListByUserIDs(text.Substring(0, text.Length - 1));
							stringBuilder = new StringBuilder();
						}
					}
				}
				this.frmWait.ShowProgress(15);
				if (stringBuilder.Length > 0)
				{
					string text = stringBuilder.ToString();
					personnelIssuecardBll.DeleteListByUserIDs(text.Substring(0, text.Length - 1));
					stringBuilder = new StringBuilder();
				}
				this.frmWait.ShowProgress(17);
				personnelIssuecardBll.AddEx(list);
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
			}
		}

		private void SaveTemplate(DataRow[] drSelected)
		{
			try
			{
				if (drSelected == null && drSelected.Length == 0)
				{
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("NoRowSelected", "请选择需要导入的记录"));
				}
				else
				{
					List<Template> list = new List<Template>();
					List<Template> list2 = new List<Template>();
					TemplateBll templateBll = new TemplateBll(MainForm._ia);
					for (int i = 0; i < drSelected.Length; i++)
					{
						string key = drSelected[i]["Pin"].ToString();
						int num = int.Parse(drSelected[i]["UID"].ToString());
						if (this.dicPin_User.ContainsKey(key))
						{
							UserInfo userInfo = this.dicPin_User[key];
							if (this.dicPin_dicFingerId_FP.ContainsKey(userInfo.BadgeNumber))
							{
								Dictionary<int, Template> dictionary = this.dicPin_dicFingerId_FP[userInfo.BadgeNumber];
								Dictionary<int, int> dictionary2 = (!this.dicUserIdFingerId_TemplateId.ContainsKey(userInfo.UserId)) ? new Dictionary<int, int>() : this.dicUserIdFingerId_TemplateId[userInfo.UserId];
								foreach (KeyValuePair<int, Template> item in dictionary)
								{
									item.Value.USERID = userInfo.UserId;
									if (dictionary2.ContainsKey(item.Value.FINGERID))
									{
										item.Value.TEMPLATEID = dictionary2[item.Value.FINGERID];
										list2.Add(item.Value);
									}
									else
									{
										list.Add(item.Value);
									}
								}
							}
						}
					}
					this.frmWait.ShowProgress(25);
					templateBll.Add(list);
					templateBll.Update(list2, this.FpVersionImported);
					this.frmWait.ShowProgress(30);
					this.LoadTemplateInDB();
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("FPCount", "指纹数") + ": " + (list.Count + list2.Count));
				}
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
			}
		}

		private void SaveFaceTemplate(DataRow[] drSelected)
		{
			try
			{
				if (drSelected == null && drSelected.Length == 0)
				{
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("NoRowSelected", "请选择需要导入的记录"));
				}
				else
				{
					int num = 0;
					List<FaceTemp> list = new List<FaceTemp>();
					FaceTempBll faceTempBll = new FaceTempBll(MainForm._ia);
					for (int i = 0; i < drSelected.Length; i++)
					{
						string key = drSelected[i]["Pin"].ToString();
						int num2 = int.Parse(drSelected[i]["UID"].ToString());
						if (this.dicPin_User.ContainsKey(key))
						{
							UserInfo userInfo = this.dicPin_User[key];
							int num3 = 1;
							while (num3 >= 0)
							{
								Dictionary<string, Dictionary<int, FaceTemp>> dictionary = (num3 == 1) ? this.STDdicPin_dicFaceId_Face : this.PullDicPin_dicFaceId_Face;
								Dictionary<int, Dictionary<int, int>> dictionary2 = (num3 == 1) ? this.STDdicUserId_FaceIdDic : this.PullDicUserId_FaceIdDic;
								num3--;
								if (dictionary.ContainsKey(key))
								{
									num++;
									Dictionary<int, FaceTemp> dictionary3 = dictionary[key];
									if (dictionary2.ContainsKey(userInfo.UserId))
									{
										Dictionary<int, int> dictionary4 = dictionary2[userInfo.UserId];
									}
									else
									{
										Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
									}
									foreach (KeyValuePair<int, FaceTemp> item in dictionary3)
									{
										item.Value.UserId = userInfo.UserId;
										item.Value.Pin = userInfo.BadgeNumber;
										list.Add(item.Value);
									}
								}
							}
						}
					}
					this.frmWait.ShowProgress(55);
					faceTempBll.Add(list);
					this.frmWait.ShowProgress(60);
					this.frmWait.ShowProgress(70);
					this.LoadFaceInDB();
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("FaceCount", "人脸数") + ": " + num);
				}
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
			}
		}

		private void SaveFingerVein(DataRow[] drSelected)
		{
			try
			{
				if (drSelected == null && drSelected.Length == 0)
				{
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("NoRowSelected", "请选择需要导入的记录"));
				}
				else
				{
					int num = 0;
					Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
					List<FingerVein> list = new List<FingerVein>();
					List<FingerVein> list2 = new List<FingerVein>();
					for (int i = 0; i < drSelected.Length; i++)
					{
						string key = drSelected[i]["Pin"].ToString();
						if (this.dicPin_User.ContainsKey(key) && this.dicPin_dicFingerId_lstFV.ContainsKey(key))
						{
							UserInfo userInfo = this.dicPin_User[key];
							Dictionary<int, List<FingerVein>> dictionary2 = this.dicPin_dicFingerId_lstFV[key];
							foreach (KeyValuePair<int, List<FingerVein>> item in dictionary2)
							{
								foreach (FingerVein item2 in item.Value)
								{
									item2.UserID = userInfo.UserId;
									list.Add(item2);
									if (!dictionary.ContainsKey(item2.UserID))
									{
										num++;
										List<int> list3 = new List<int>();
										list3.Add(item2.FingerID);
										dictionary.Add(item2.UserID, list3);
									}
									else
									{
										List<int> list3 = dictionary[item2.UserID];
										if (!list3.Contains(item2.FingerID))
										{
											num++;
											list3.Add(item2.FingerID);
										}
									}
								}
							}
						}
					}
					this.frmWait.ShowProgress(75);
					FingerVeinBll fingerVeinBll = new FingerVeinBll(MainForm._ia);
					fingerVeinBll.Add(list);
					this.frmWait.ShowProgress(80);
					fingerVeinBll.Update(list2);
					this.frmWait.ShowProgress(90);
					this.frmWait.ShowInfos(ShowMsgInfos.GetInfo("FVCount", "指静脉数") + ": " + num);
				}
			}
			catch (Exception ex)
			{
				this.frmWait.ShowInfos(ex.Message);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UDiskToPc));
			this.ckb_FaceTemp = new CheckBox();
			this.ckb_User = new CheckBox();
			this.ckb_FP = new CheckBox();
			this.ckb_FV = new CheckBox();
			this.gridControl = new GridControl();
			this.grdview_UserInfo = new GridView();
			this.colCheck = new GridColumn();
			this.colPin = new GridColumn();
			this.colName = new GridColumn();
			this.colCardNo = new GridColumn();
			this.colFPCount = new GridColumn();
			this.colFVCount = new GridColumn();
			this.colFaceCount = new GridColumn();
			this.colStdFaceCount = new GridColumn();
			this.panel2 = new Panel();
			this.groupBox1 = new GroupBox();
			this.btnExit = new ButtonX();
			this.btnSave = new ButtonX();
			this.btnImport = new ButtonX();
			this.gpbFpVersion = new GroupBox();
			this.rdbFP9 = new RadioButton();
			this.rdbFP10 = new RadioButton();
			this.gpbUserType = new GroupBox();
			this.rdbTFT = new RadioButton();
			this.rdbPin9 = new RadioButton();
			this.rdbPin5 = new RadioButton();
			this.panel3 = new Panel();
			((ISupportInitialize)this.gridControl).BeginInit();
			((ISupportInitialize)this.grdview_UserInfo).BeginInit();
			this.panel2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.gpbFpVersion.SuspendLayout();
			this.gpbUserType.SuspendLayout();
			this.panel3.SuspendLayout();
			base.SuspendLayout();
			this.ckb_FaceTemp.AutoSize = true;
			this.ckb_FaceTemp.Location = new Point(6, 64);
			this.ckb_FaceTemp.Name = "ckb_FaceTemp";
			this.ckb_FaceTemp.Size = new Size(72, 16);
			this.ckb_FaceTemp.TabIndex = 45;
			this.ckb_FaceTemp.Text = "面部数据";
			this.ckb_FaceTemp.UseVisualStyleBackColor = true;
			this.ckb_User.AutoSize = true;
			this.ckb_User.Checked = true;
			this.ckb_User.CheckState = CheckState.Checked;
			this.ckb_User.Enabled = false;
			this.ckb_User.Location = new Point(6, 20);
			this.ckb_User.Name = "ckb_User";
			this.ckb_User.Size = new Size(72, 16);
			this.ckb_User.TabIndex = 41;
			this.ckb_User.Text = "用户信息";
			this.ckb_User.UseVisualStyleBackColor = true;
			this.ckb_User.CheckedChanged += this.ckb_User_CheckedChanged;
			this.ckb_FP.AutoSize = true;
			this.ckb_FP.Checked = true;
			this.ckb_FP.CheckState = CheckState.Checked;
			this.ckb_FP.Location = new Point(6, 42);
			this.ckb_FP.Name = "ckb_FP";
			this.ckb_FP.Size = new Size(72, 16);
			this.ckb_FP.TabIndex = 42;
			this.ckb_FP.Text = "指纹信息";
			this.ckb_FP.UseVisualStyleBackColor = true;
			this.ckb_FP.CheckedChanged += this.ckb_FP_CheckedChanged;
			this.ckb_FV.AutoSize = true;
			this.ckb_FV.Location = new Point(6, 86);
			this.ckb_FV.Name = "ckb_FV";
			this.ckb_FV.Size = new Size(78, 16);
			this.ckb_FV.TabIndex = 44;
			this.ckb_FV.Text = "指静脉   ";
			this.ckb_FV.UseVisualStyleBackColor = true;
			this.gridControl.Dock = DockStyle.Fill;
			this.gridControl.Location = new Point(10, 10);
			this.gridControl.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.gridControl.MainView = this.grdview_UserInfo;
			this.gridControl.Name = "gridControl";
			this.gridControl.Size = new Size(704, 498);
			this.gridControl.TabIndex = 19;
			this.gridControl.ViewCollection.AddRange(new BaseView[1]
			{
				this.grdview_UserInfo
			});
			this.grdview_UserInfo.Columns.AddRange(new GridColumn[8]
			{
				this.colCheck,
				this.colPin,
				this.colName,
				this.colCardNo,
				this.colFPCount,
				this.colFVCount,
				this.colFaceCount,
				this.colStdFaceCount
			});
			this.grdview_UserInfo.GridControl = this.gridControl;
			this.grdview_UserInfo.Name = "grdview_UserInfo";
			this.grdview_UserInfo.OptionsView.ShowGroupPanel = false;
			this.grdview_UserInfo.SortInfo.AddRange(new GridColumnSortInfo[1]
			{
				new GridColumnSortInfo(this.colPin, ColumnSortOrder.Ascending)
			});
			this.grdview_UserInfo.CustomDrawColumnHeader += this.grdview_UserInfo_CustomDrawColumnHeader;
			this.grdview_UserInfo.CustomDrawCell += this.grdview_UserInfo_CustomDrawCell;
			this.grdview_UserInfo.Click += this.grdview_UserInfo_Click;
			this.colCheck.Caption = " ";
			this.colCheck.Name = "colCheck";
			this.colCheck.Visible = true;
			this.colCheck.VisibleIndex = 0;
			this.colPin.Caption = "工号";
			this.colPin.Name = "colPin";
			this.colPin.Visible = true;
			this.colPin.VisibleIndex = 1;
			this.colName.Caption = "姓名";
			this.colName.Name = "colName";
			this.colName.Visible = true;
			this.colName.VisibleIndex = 2;
			this.colCardNo.Caption = "卡号";
			this.colCardNo.Name = "colCardNo";
			this.colCardNo.Visible = true;
			this.colCardNo.VisibleIndex = 3;
			this.colFPCount.Caption = "指纹数";
			this.colFPCount.Name = "colFPCount";
			this.colFPCount.Visible = true;
			this.colFPCount.VisibleIndex = 4;
			this.colFVCount.Caption = "静脉数";
			this.colFVCount.Name = "colFVCount";
			this.colFVCount.Visible = true;
			this.colFVCount.VisibleIndex = 5;
			this.colFaceCount.Caption = "人脸数(Pull)";
			this.colFaceCount.Name = "colFaceCount";
			this.colFaceCount.Visible = true;
			this.colFaceCount.VisibleIndex = 6;
			this.colStdFaceCount.Caption = "人脸数(脱机)";
			this.colStdFaceCount.Name = "colStdFaceCount";
			this.colStdFaceCount.Visible = true;
			this.colStdFaceCount.VisibleIndex = 7;
			this.panel2.Controls.Add(this.groupBox1);
			this.panel2.Controls.Add(this.btnExit);
			this.panel2.Controls.Add(this.btnSave);
			this.panel2.Controls.Add(this.btnImport);
			this.panel2.Controls.Add(this.gpbFpVersion);
			this.panel2.Controls.Add(this.gpbUserType);
			this.panel2.Dock = DockStyle.Right;
			this.panel2.Location = new Point(714, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(180, 518);
			this.panel2.TabIndex = 1;
			this.groupBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.groupBox1.Controls.Add(this.ckb_FaceTemp);
			this.groupBox1.Controls.Add(this.ckb_User);
			this.groupBox1.Controls.Add(this.ckb_FP);
			this.groupBox1.Controls.Add(this.ckb_FV);
			this.groupBox1.Location = new Point(6, 7);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(171, 115);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "数据类型";
			this.btnExit.AccessibleRole = AccessibleRole.PushButton;
			this.btnExit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnExit.Location = new Point(20, 417);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new Size(140, 23);
			this.btnExit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnExit.TabIndex = 7;
			this.btnExit.Text = "退出";
			this.btnExit.Click += this.btnExit_Click;
			this.btnSave.AccessibleRole = AccessibleRole.PushButton;
			this.btnSave.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnSave.Location = new Point(20, 378);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new Size(140, 23);
			this.btnSave.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnSave.TabIndex = 6;
			this.btnSave.Text = "保存";
			this.btnSave.Click += this.btnSave_Click;
			this.btnImport.AccessibleRole = AccessibleRole.PushButton;
			this.btnImport.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnImport.Location = new Point(20, 340);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new Size(140, 23);
			this.btnImport.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnImport.TabIndex = 5;
			this.btnImport.Text = "获取";
			this.btnImport.Click += this.btnImport_Click;
			this.gpbFpVersion.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.gpbFpVersion.Controls.Add(this.rdbFP9);
			this.gpbFpVersion.Controls.Add(this.rdbFP10);
			this.gpbFpVersion.Location = new Point(6, 251);
			this.gpbFpVersion.Name = "gpbFpVersion";
			this.gpbFpVersion.Size = new Size(171, 66);
			this.gpbFpVersion.TabIndex = 1;
			this.gpbFpVersion.TabStop = false;
			this.gpbFpVersion.Text = "指纹版本";
			this.rdbFP9.AutoSize = true;
			this.rdbFP9.Location = new Point(6, 42);
			this.rdbFP9.Name = "rdbFP9";
			this.rdbFP9.Size = new Size(41, 16);
			this.rdbFP9.TabIndex = 40;
			this.rdbFP9.Text = "9.0";
			this.rdbFP9.UseVisualStyleBackColor = true;
			this.rdbFP10.AutoSize = true;
			this.rdbFP10.Checked = true;
			this.rdbFP10.Location = new Point(6, 20);
			this.rdbFP10.Name = "rdbFP10";
			this.rdbFP10.Size = new Size(47, 16);
			this.rdbFP10.TabIndex = 41;
			this.rdbFP10.TabStop = true;
			this.rdbFP10.Text = "10.0";
			this.rdbFP10.UseVisualStyleBackColor = true;
			this.gpbUserType.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.gpbUserType.Controls.Add(this.rdbTFT);
			this.gpbUserType.Controls.Add(this.rdbPin9);
			this.gpbUserType.Controls.Add(this.rdbPin5);
			this.gpbUserType.Location = new Point(6, 142);
			this.gpbUserType.Name = "gpbUserType";
			this.gpbUserType.Size = new Size(171, 88);
			this.gpbUserType.TabIndex = 0;
			this.gpbUserType.TabStop = false;
			this.gpbUserType.Text = "用户类型";
			this.rdbTFT.AutoSize = true;
			this.rdbTFT.Location = new Point(6, 64);
			this.rdbTFT.Name = "rdbTFT";
			this.rdbTFT.Size = new Size(47, 16);
			this.rdbTFT.TabIndex = 43;
			this.rdbTFT.Text = "彩屏";
			this.rdbTFT.UseVisualStyleBackColor = true;
			this.rdbPin9.AutoSize = true;
			this.rdbPin9.Checked = true;
			this.rdbPin9.Location = new Point(6, 20);
			this.rdbPin9.Name = "rdbPin9";
			this.rdbPin9.Size = new Size(65, 16);
			this.rdbPin9.TabIndex = 42;
			this.rdbPin9.TabStop = true;
			this.rdbPin9.Text = "9位工号";
			this.rdbPin9.UseVisualStyleBackColor = true;
			this.rdbPin5.AutoSize = true;
			this.rdbPin5.Location = new Point(6, 42);
			this.rdbPin5.Name = "rdbPin5";
			this.rdbPin5.Size = new Size(65, 16);
			this.rdbPin5.TabIndex = 41;
			this.rdbPin5.Text = "5位工号";
			this.rdbPin5.UseVisualStyleBackColor = true;
			this.panel3.Controls.Add(this.gridControl);
			this.panel3.Dock = DockStyle.Fill;
			this.panel3.Location = new Point(0, 0);
			this.panel3.Name = "panel3";
			this.panel3.Padding = new System.Windows.Forms.Padding(10, 10, 0, 10);
			this.panel3.Size = new Size(714, 518);
			this.panel3.TabIndex = 20;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(894, 518);
			base.Controls.Add(this.panel3);
			base.Controls.Add(this.panel2);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UDiskToPc";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "从U盘导入数据";
			base.FormClosing += this.UDiskToPcEx_FormClosing;
			((ISupportInitialize)this.gridControl).EndInit();
			((ISupportInitialize)this.grdview_UserInfo).EndInit();
			this.panel2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.gpbFpVersion.ResumeLayout(false);
			this.gpbFpVersion.PerformLayout();
			this.gpbUserType.ResumeLayout(false);
			this.gpbUserType.PerformLayout();
			this.panel3.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
