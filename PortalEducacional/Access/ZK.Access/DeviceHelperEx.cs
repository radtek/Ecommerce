/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System.Collections.Generic;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;

namespace ZK.Access
{
	public class DeviceHelperEx
	{
		public delegate void ShowInfoHandle(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private bool m_isStop = false;

		public event ShowInfoHandle ShowInfoEvent;

		public event ShowProgressHandle ShowProgressEvent;

		public void Stop()
		{
			this.m_isStop = true;
		}

		private void ShowUpLoadInfo(string UpLoadinfoStr)
		{
			if (this.ShowInfoEvent != null)
			{
				this.ShowInfoEvent(UpLoadinfoStr);
			}
		}

		private void ShowProgress(int prg)
		{
			if (this.ShowProgressEvent != null)
			{
				this.ShowProgressEvent(prg);
			}
		}

		private bool UpUserBaseInfo(UserBll devUserBll, List<ObjUser> tempList)
		{
			if (!this.m_isStop && devUserBll != null && tempList != null && tempList.Count > 0)
			{
				int num = devUserBll.Add(tempList);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoOk", "从PC同步人员信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + tempList.Count);
					return true;
				}
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoFalse", "从PC同步人员信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + tempList.Count);
			}
			return false;
		}

		private bool UpUserTemplate(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			if (!this.m_isStop && devServerBll != null && devUsers != null && devUsers.Count > 0)
			{
				int num = DeviceHelper.SetUserTemplate(devServerBll, devUsers);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateOk", "从PC同步指纹信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + devUsers.Count + " " + ShowMsgInfos.GetInfo("templateCount", "指纹数") + ":" + num);
					return true;
				}
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + devUsers.Count);
			}
			return false;
		}

		private bool UpUserFaceTemplate(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			if (!this.m_isStop && devServerBll != null && devUsers != null && devUsers.Count > 0)
			{
				int num = DeviceHelper.SetUserFaceTemplate(devServerBll, devUsers);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateOk", "从PC同步面部信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + devUsers.Count + " " + ShowMsgInfos.GetInfo("FacetemplateCount", "面部模版数") + ":" + num / 12);
					return true;
				}
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFaceTemplateFalse", "从PC同步面部信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + devUsers.Count);
			}
			return false;
		}

		private bool UpUserFingerVein(DeviceServerBll devServerBll, List<ObjUser> devUsers)
		{
			if (!this.m_isStop && devServerBll != null && devUsers != null && devUsers.Count > 0)
			{
				int num = DeviceHelper.SetUserFingerVein(devServerBll, devUsers);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFingerVeinOk", "从PC同步指静脉信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + devUsers.Count + " " + ShowMsgInfos.GetInfo("FingerVeinCount", "指静脉数") + ":" + num);
					return true;
				}
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFingerVeinFalse", "从PC同步指静脉信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + " " + ShowMsgInfos.GetInfo("UpUserInfoCount", "人员数") + ":" + devUsers.Count);
			}
			return false;
		}

		private bool UpUserAuthorizeInfo(UserAuthorizeBll devUserAuthorizeBll, List<ObjUserAuthorize> list)
		{
			if (!this.m_isStop && devUserAuthorizeBll != null && list != null && list.Count > 0)
			{
				int num = devUserAuthorizeBll.Add(list);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetLevelsetOk", "从PC同步人员权限信息到设备成功") + " " + ShowMsgInfos.GetInfo("UpUserAuthorizeInfoCount", "权限记录数") + ":" + list.Count);
					return true;
				}
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTemplateFalse", "从PC同步指纹信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num) + " " + ShowMsgInfos.GetInfo("UpUserAuthorizeInfoCount", "权限记录数") + ":" + list.Count);
			}
			return false;
		}

		private int SetUserAuthorizeInfoEx(DeviceServerBll devServerBll, int Option)
		{
			int result = -1002;
			if (devServerBll != null)
			{
				result = 0;
				AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
				List<AccLevelset> modelList = accLevelsetBll.GetModelList("");
				result = 0;
				if (modelList != null && modelList.Count > 0)
				{
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					List<UserInfo> modelList2 = userInfoBll.GetModelList("");
					AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
					AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
					List<ObjUserAuthorize> list = new List<ObjUserAuthorize>();
					Dictionary<int, UserInfo> dictionary = new Dictionary<int, UserInfo>();
					List<ObjUser> list2 = new List<ObjUser>();
					for (int i = 0; i < modelList.Count; i++)
					{
						int num = 20 * i / modelList.Count;
						this.ShowProgress(num);
						List<AccDoor> modelList3 = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group where acclevelset_id=" + modelList[i].id + " ) and device_id=" + devServerBll.DevInfo.ID);
						if (modelList3 != null && modelList3.Count > 0)
						{
							int num2 = 0;
							for (int j = 0; j < modelList3.Count; j++)
							{
								num2 += 1 << modelList3[j].door_no - 1;
							}
							int level_timeseg_id = modelList[i].level_timeseg_id;
							List<AccLevelsetEmp> modelList4 = accLevelsetEmpBll.GetModelList("acclevelset_id=" + modelList[i].id);
							if (modelList4 != null && modelList4.Count > 0)
							{
								int num3 = num;
								for (int k = 0; k < modelList4.Count; k++)
								{
									if (num3 != num + 20 * k / (modelList4.Count * modelList.Count))
									{
										num3 = num + 20 * k / (modelList4.Count * modelList.Count);
										this.ShowProgress(num3);
									}
									UserInfo userInfo = null;
									if (dictionary.ContainsKey(modelList4[k].employee_id))
									{
										userInfo = dictionary[modelList4[k].employee_id];
									}
									else
									{
										userInfo = userInfoBll.GetModel(modelList4[k].employee_id);
										if (userInfo != null)
										{
											dictionary.Add(modelList4[k].employee_id, userInfo);
											ObjUser item = DeviceHelper.UserDataConvert(userInfo);
											list2.Add(item);
										}
									}
									if (userInfo != null)
									{
										ObjUserAuthorize objUserAuthorize = new ObjUserAuthorize();
										objUserAuthorize.AuthorizeDoorId = num2.ToString();
										objUserAuthorize.Pin = userInfo.BadgeNumber;
										objUserAuthorize.AuthorizeTimezoneId = level_timeseg_id.ToString();
										if (DeviceHelper.TimeSeg.id == level_timeseg_id)
										{
											objUserAuthorize.AuthorizeTimezoneId = "1";
										}
										list.Add(objUserAuthorize);
									}
								}
							}
						}
					}
					this.ShowProgress(20);
					UserBll devUserBll = new UserBll(devServerBll.Application);
					UserAuthorizeBll devUserAuthorizeBll = new UserAuthorizeBll(devServerBll.Application);
					if (list2.Count > 0)
					{
						if (list2.Count < 100)
						{
							this.UpUserBaseInfo(devUserBll, list2);
							this.ShowProgress(30);
							switch (Option)
							{
							case 1:
								this.UpUserTemplate(devServerBll, list2);
								break;
							case 2:
								this.UpUserFingerVein(devServerBll, list2);
								break;
							case 3:
								this.UpUserTemplate(devServerBll, list2);
								this.UpUserFaceTemplate(devServerBll, list2);
								break;
							}
							this.ShowProgress(50);
						}
						else
						{
							List<ObjUser> list3 = new List<ObjUser>();
							for (int l = 0; l < list2.Count; l++)
							{
								list3.Add(list2[l]);
								int num4 = 500;
								if (l > 0 && l % num4 == 0)
								{
									this.UpUserBaseInfo(devUserBll, list3);
									switch (Option)
									{
									case 1:
										this.UpUserTemplate(devServerBll, list3);
										break;
									case 2:
										this.UpUserFingerVein(devServerBll, list3);
										break;
									case 3:
										this.UpUserTemplate(devServerBll, list3);
										this.UpUserFaceTemplate(devServerBll, list3);
										break;
									}
									this.ShowProgress(20 + 30 * l / list2.Count);
									list3.Clear();
								}
							}
							this.UpUserBaseInfo(devUserBll, list3);
							switch (Option)
							{
							case 1:
								this.UpUserTemplate(devServerBll, list3);
								break;
							case 2:
								this.UpUserFingerVein(devServerBll, list3);
								break;
							case 3:
								this.UpUserTemplate(devServerBll, list3);
								this.UpUserFaceTemplate(devServerBll, list3);
								break;
							}
							this.ShowProgress(50);
						}
					}
					if (list.Count > 0)
					{
						if (list.Count < 100)
						{
							this.UpUserAuthorizeInfo(devUserAuthorizeBll, list);
						}
						else
						{
							List<ObjUserAuthorize> list4 = new List<ObjUserAuthorize>();
							for (int m = 0; m < list.Count; m++)
							{
								list4.Add(list[m]);
								if (m > 0 && m % 200 == 0)
								{
									this.UpUserAuthorizeInfo(devUserAuthorizeBll, list4);
									this.ShowProgress(50 + 10 * m / list.Count);
									list4.Clear();
								}
							}
							this.UpUserAuthorizeInfo(devUserAuthorizeBll, list4);
						}
					}
					this.ShowProgress(60);
				}
			}
			return result;
		}

		public void SetUserAuthorizeInfo(DeviceServerBll devServerBll, int Option)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.DeleteUserAuthorize(devServerBll);
				if (num >= 0)
				{
					num = DeviceHelper.DeleteUserInfo(devServerBll);
					if (num >= 0)
					{
						switch (Option)
						{
						case 1:
							num = DeviceHelper.DeleteTemplate(devServerBll);
							if (num < 0)
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteTemplateFalse", "重置用户指纹信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							break;
						case 2:
							num = DeviceHelper.DeleteUserFingerVein(devServerBll);
							if (num < 0)
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteFVFalse", "重置用户指静脉信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							break;
						case 3:
							num = DeviceHelper.DeleteTemplate(devServerBll);
							if (num < 0)
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteTemplateFalse", "重置用户指纹信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							num = DeviceHelper.DeleteFaceTemplate(devServerBll);
							if (num < 0)
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DeleteFaceTemplateFalse", "重置面部识别信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
							}
							break;
						}
						num = this.SetUserAuthorizeInfoEx(devServerBll, Option);
						if (num >= 0)
						{
							AccLevelsetBll.IsUpdate = false;
							UserInfoBll.IsUpdate = false;
						}
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetUserInfoAndLevelsetFinish", "从PC同步人员及权限信息到设备完成"));
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelUserInfoFalse", "重置用户信息失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelLevelsetFalse", "重置人员权限失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void SetNormalOpenInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.DeleteFirstCard(devServerBll);
				if (num >= 0)
				{
					DeviceHelper.SetNormalOpenInfo(devServerBll);
					if (num >= 0)
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFirstCardOk", "从PC同步首卡开门信息到设备成功"));
						AccFirstOpenBll.IsUpdate = false;
						AccFirstOpenEmpBll.IsUpdate = false;
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetFirstCardFalse", "从PC同步首卡开门信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelFirstCardFalse", "首卡开门重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void SetMultimCardInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.DeleteMultimCard(devServerBll);
				if (num >= 0)
				{
					num = DeviceHelper.SetMultimCardInfo(devServerBll);
					if (num >= 0)
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetMultimCardOk", "从PC同步多卡开门信息到设备成功"));
						AccMorecardsetBll.IsUpdate = false;
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetMultimCardFalse", "从PC同步多卡开门信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelMultimCardFalse", "多卡开门重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void SetHolidayInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.DeleteHoliday(devServerBll);
				if (num >= 0)
				{
					num = DeviceHelper.SetHolidayInfo(devServerBll);
					if (num >= 0)
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayOk", "从PC同步节假日信息到设备成功"));
						AccHolidaysBll.IsUpdate = false;
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetHolidayFalse", "从PC同步节假日信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelHolidayFalse", "节假日重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void SetTZInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.DeleteTimeZone(devServerBll);
				if (num >= 0)
				{
					num = DeviceHelper.SetTZInfo(devServerBll);
					if (num >= 0)
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneOk", "从PC同步门禁时间段信息到设备成功"));
						AccTimesegBll.IsUpdate = false;
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetTimeZoneFalse", "从PC同步门禁时间段信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelTimeZoneFalse", "门禁时间段重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void UpdateDoorSetings(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + devServerBll.DevInfo.ID);
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						int num = DeviceHelper.UpdateDoorSetings(devServerBll, modelList[i]);
						if (num >= 0)
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDoorInfoOk", "从PC同步门设置信息到设备成功") + ":" + modelList[i].door_name);
						}
						else
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetDoorInfoFalse", "从PC同步门设置信息到设备失败") + "-" + modelList[i].door_name + ":" + PullSDkErrorInfos.GetInfo(num));
						}
						if (this.SetWiegandInfo(devServerBll))
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadWiegandfmt", "从PC同步韦根设置信息到设备成功"));
						}
						if (this.Set485MasterSalveInfo(devServerBll))
						{
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadMasterSlave", "PC同步韦根485主从机信息到设备成功"));
						}
					}
				}
			}
		}

		public void SetInOutFunInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.DeleteInOutFun(devServerBll);
				if (num >= 0)
				{
					num = DeviceHelper.SetInOutFunInfo(devServerBll);
					if (num >= 0)
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInOutFunOk", "从PC同步联动信息到设备成功"));
						AccLinkAgeIoBll.IsUpdate = false;
					}
					else
					{
						this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInOutFunFalse", "从PC同步联动信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
					}
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("DelInOutFunFalse", "联动重置失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void SetInterLockInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.SetInterLockInfo(devServerBll);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInterlockTypeOk", "从PC同步互锁设置信息到设备成功"));
					AccInterlockBll.IsUpdate = false;
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetInterlockTypeFalse", "从PC同步互锁设置信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public void SetAntiPassbackInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.SetAntiPassbackInfo(devServerBll);
				if (num >= 0)
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackTypeOk", "从PC同步反潜设置信息到设备成功"));
					AccAntibackBll.IsUpdate = false;
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SetAntibackTypeFalse", "从PC同步反潜设置信息到设备失败") + ":" + PullSDkErrorInfos.GetInfo(num));
				}
			}
		}

		public bool SetWiegandInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.SetWiegand(devServerBll);
				if (num >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool Set485MasterSalveInfo(DeviceServerBll devServerBll)
		{
			if (!this.m_isStop && devServerBll != null)
			{
				int num = DeviceHelper.SetMasterSlave(devServerBll);
				if (num >= 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
