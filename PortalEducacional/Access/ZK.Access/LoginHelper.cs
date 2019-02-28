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
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class LoginHelper
	{
		public static bool Login(string UserId, string Password)
		{
			if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserId))
			{
				try
				{
					AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
					AuthUser model = authUserBll.GetModel("admin");
					if (model != null)
					{
						SysInfos.AdminID = model.id;
						SysInfos.AdminRoleID = model.RoleID;
						AuthGroupBll authGroupBll = new AuthGroupBll(MainForm._ia);
						AuthGroup model2 = authGroupBll.GetModel(model.RoleID);
						if (model2 == null)
						{
							List<AuthGroup> modelList = authGroupBll.GetModelList(" name='administrator' ");
							if (modelList == null || modelList.Count == 0)
							{
								AuthGroup authGroup = new AuthGroup();
								authGroup.name = "administrator";
								authGroup.Permission = "333333333333333333333333333333333333333333333333333333333";
								authGroup.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
								authGroupBll.Add(authGroup);
								modelList = authGroupBll.GetModelList(" name='administrator' ");
							}
							if (modelList != null && modelList.Count > 0)
							{
								model.RoleID = modelList[0].id;
								authUserBll.Update(model);
								SysInfos.AdminRoleID = model.RoleID;
							}
						}
					}
					AuthUser authUser = null;
					authUser = ((!(UserId == "admin")) ? authUserBll.GetModel(UserId) : model);
					if (authUser != null)
					{
						if (authUser.password == Password)
						{
							authUser.last_login = DateTime.Now;
							authUserBll.Update(authUser);
							if (authUser.Status >= 0 || authUser.username.ToString() == "admin")
							{
								if (authUser.username.ToString() != "admin")
								{
									AreaAdminBll areaAdminBll = new AreaAdminBll(MainForm._ia);
									List<AreaAdmin> modelList2 = areaAdminBll.GetModelList("user_id=" + authUser.id);
									if (modelList2 != null && modelList2.Count > 0)
									{
										string text = "(" + modelList2[0].area_id;
										for (int i = 1; i < modelList2.Count; i++)
										{
											text = text + "," + modelList2[i].area_id;
										}
										text = (SysInfos.Areas = text + ")");
									}
									DeptAdminBll deptAdminBll = new DeptAdminBll(MainForm._ia);
									List<DeptAdmin> modelList3 = deptAdminBll.GetModelList("user_id=" + authUser.id);
									if (modelList3 != null && modelList3.Count > 0)
									{
										string text3 = "(" + modelList3[0].dept_id;
										for (int j = 1; j < modelList3.Count; j++)
										{
											text3 = text3 + "," + modelList3[j].dept_id;
										}
										text3 = (SysInfos.Depts = text3 + ")");
									}
								}
								SysInfos.SysUserInfo = authUser;
								SysInfos.SysUserInfo.username = UserId.ToLower();
								if (authUser.RoleID == SysInfos.AdminRoleID)
								{
									SysInfos.UserPermission = string.Empty;
								}
								else
								{
									AuthGroupBll authGroupBll2 = new AuthGroupBll(MainForm._ia);
									AuthGroup authGroup2 = null;
									authGroup2 = authGroupBll2.GetModel(authUser.RoleID);
									if (authGroup2 != null)
									{
										SysInfos.UserPermission = authGroup2.Permission;
									}
									else
									{
										SysInfos.UserPermission = string.Empty;
									}
								}
								return true;
							}
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserDisEnabled", "用户名已经被禁用,请联系管理员"));
							return false;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdWrong", "用户名或者密码错误"));
						return false;
					}
					string text5 = AppSite.Instance.GetNodeValueByName("password");
					if (string.IsNullOrEmpty(text5))
					{
						text5 = "admin";
						AppSite.Instance.SetNodeValue("password", text5);
					}
					if (UserId.Trim().ToLower() == "admin" && Password == text5)
					{
						authUser = new AuthUser();
						authUser.username = "admin";
						authUser.password = text5;
						authUser.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
						authUser.last_login = DateTime.Now;
						AuthGroupBll authGroupBll3 = new AuthGroupBll(MainForm._ia);
						List<AuthGroup> modelList4 = authGroupBll3.GetModelList(" name='administrator' ");
						if (modelList4 == null || modelList4.Count == 0)
						{
							AuthGroup authGroup3 = new AuthGroup();
							authGroup3.name = "administrator";
							authGroup3.Permission = "333333333333333333333333333333333333333333333333333333333";
							authGroup3.Remark = ShowMsgInfos.GetInfo("administrator", "超级管理员");
							authGroupBll3.Add(authGroup3);
							modelList4 = authGroupBll3.GetModelList(" name='administrator' ");
						}
						if (modelList4 != null && modelList4.Count > 0)
						{
							authUser.RoleID = modelList4[0].id;
						}
						authUserBll.Add(authUser);
						authUser.id = authUserBll.GetMaxId() - 1;
						SysInfos.AdminID = authUser.id;
						SysInfos.SysUserInfo = authUser;
						SysInfos.UserPermission = string.Empty;
						return true;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdWrong", "用户名或者密码错误"));
					return false;
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
					return false;
				}
			}
			SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdNull", "用户名或密码不能为空"));
			return false;
		}

		public static bool LoginIPC(string UserId, string Password)
		{
			if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserId))
			{
				try
				{
					AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
					AuthUser model = authUserBll.GetModel("admin");
					if (model != null)
					{
						SysInfos.AdminID = model.id;
						SysInfos.AdminRoleID = model.RoleID;
					}
					AuthUser authUser = null;
					authUser = ((!(UserId == "admin")) ? authUserBll.GetModel(UserId) : model);
					if (authUser != null)
					{
						if (authUser.password == Password)
						{
							authUser.last_login = DateTime.Now;
							if (authUser.Status >= 0 || authUser.username.ToString() == "admin")
							{
								if (authUser.username.ToString() != "admin")
								{
									AreaAdminBll areaAdminBll = new AreaAdminBll(MainForm._ia);
									List<AreaAdmin> modelList = areaAdminBll.GetModelList("user_id=" + authUser.id);
									if (modelList != null && modelList.Count > 0)
									{
										string text = "(" + modelList[0].area_id;
										for (int i = 1; i < modelList.Count; i++)
										{
											text = text + "," + modelList[i].area_id;
										}
										text = (SysInfos.Areas = text + ")");
									}
									DeptAdminBll deptAdminBll = new DeptAdminBll(MainForm._ia);
									List<DeptAdmin> modelList2 = deptAdminBll.GetModelList("user_id=" + authUser.id);
									if (modelList2 != null && modelList2.Count > 0)
									{
										string text3 = "(" + modelList2[0].dept_id;
										for (int j = 1; j < modelList2.Count; j++)
										{
											text3 = text3 + "," + modelList2[j].dept_id;
										}
										text3 = (SysInfos.Depts = text3 + ")");
									}
								}
								SysInfos.SysUserInfo = authUser;
								SysInfos.SysUserInfo.username = UserId.ToLower();
								if (authUser.RoleID == SysInfos.AdminRoleID)
								{
									SysInfos.UserPermission = string.Empty;
								}
								else
								{
									AuthGroupBll authGroupBll = new AuthGroupBll(MainForm._ia);
									AuthGroup authGroup = null;
									authGroup = authGroupBll.GetModel(authUser.RoleID);
									if (authGroup != null)
									{
										SysInfos.UserPermission = authGroup.Permission;
									}
									else
									{
										SysInfos.UserPermission = string.Empty;
									}
								}
								return true;
							}
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserDisEnabled", "用户名已经被禁用,请联系管理员"));
							return false;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdWrong", "用户名或者密码错误"));
						return false;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdWrong", "用户名或者密码错误"));
					return false;
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
					return false;
				}
			}
			SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SUserNameOrPasswdNull", "用户名或密码不能为空"));
			return false;
		}
	}
}
