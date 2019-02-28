/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
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
using ZK.Access.personnel;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class MultiCardOpenGroupSet : Office2007Form
	{
		private List<Machines> lstMachine;

		private Dictionary<int, Machines> dicId_Machine;

		private Dictionary<int, AccMorecardempGroup> dicPullGroup_StdGroup;

		private Dictionary<int, List<int>> dicLvEmpId_lstLvId;

		private Dictionary<int, List<int>> dicLvId_lstDevId;

		private DataTable m_datatable = new DataTable();

		private DataTable m_browdatatable = new DataTable();

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private AccMorecardempGroupBll bll = new AccMorecardempGroupBll(MainForm._ia);

		private int selectid = -1;

		private WaitForm m_wait = WaitForm.Instance;

		private bool isDouble = false;

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem menu_add;

		private ToolStripMenuItem menu_edit;

		private ToolStripMenuItem menu_del;

		private ToolStripMenuItem menu_delall;

		private ToolStripMenuItem menu_addgroupuser;

		private ContextMenuStrip contextMenuStrip2;

		private ToolStripMenuItem menu_delex;

		private ToolStripMenuItem menu_delallex;

		private ButtonX btn_cancel;

		private GridControl grd_brow;

		private GridView grd_browview;

		private GridColumn column_no;

		private GridColumn column_name;

		private GridColumn column_cardno;

		private ButtonX btn_deletePerson;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_dev;

		private GridColumn column_remark;

		private BandedGridView bandedGridView1;

		private ButtonX btn_addPerson;

		private ButtonX btn_add;

		private ButtonX btn_delete;

		private ButtonX btn_edit;

		private LabelX lb_group;

		private LabelX lb_brow;

		private GridColumn column_usercheck;

		private GridColumn column_check;

		private GridColumn column_acount;

		private GridColumn column_lastName;

		private GridColumn column_gender;

		private GridColumn column_dept;

		public MultiCardOpenGroupSet()
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
			}
			try
			{
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				DevExpressHelper.InitImageList(this.grd_browview, "column_usercheck");
				this.GenderType();
				this.InitDataTableSet();
				this.InitBrowDataTableSet();
				this.LoadData();
				initLang.LocaleForm(this, base.Name);
				this.InitialLevelDictionary();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void InitialLevelDictionary()
		{
			this.dicLvId_lstDevId = new Dictionary<int, List<int>>();
			AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
			List<AccLevelsetDoorGroup> modelList = accLevelsetDoorGroupBll.GetModelList("");
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!this.dicLvId_lstDevId.ContainsKey(modelList[i].acclevelset_id))
					{
						List<int> list = new List<int>();
						list.Add(modelList[i].accdoor_device_id);
						this.dicLvId_lstDevId.Add(modelList[i].acclevelset_id, list);
					}
					else
					{
						List<int> list = this.dicLvId_lstDevId[modelList[i].acclevelset_id];
						if (!list.Contains(modelList[i].accdoor_device_id))
						{
							list.Add(modelList[i].accdoor_device_id);
						}
					}
				}
			}
			this.dicLvEmpId_lstLvId = new Dictionary<int, List<int>>();
			AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
			List<AccLevelsetEmp> modelList2 = accLevelsetEmpBll.GetModelList("");
			if (modelList2 != null)
			{
				for (int j = 0; j < modelList2.Count; j++)
				{
					if (!this.dicLvEmpId_lstLvId.ContainsKey(modelList2[j].employee_id))
					{
						List<int> list2 = new List<int>();
						list2.Add(modelList2[j].acclevelset_id);
						this.dicLvEmpId_lstLvId.Add(modelList2[j].employee_id, list2);
					}
					else
					{
						List<int> list2 = this.dicLvEmpId_lstLvId[modelList2[j].employee_id];
						if (!list2.Contains(modelList2[j].acclevelset_id))
						{
							list2.Add(modelList2[j].acclevelset_id);
						}
					}
				}
			}
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			this.lstMachine = machinesBll.GetModelList("DevSDKType = 2");
			this.dicId_Machine = new Dictionary<int, Machines>();
			this.lstMachine = ((this.lstMachine == null) ? new List<Machines>() : this.lstMachine);
			for (int k = 0; k < this.lstMachine.Count; k++)
			{
				if (!this.dicId_Machine.ContainsKey(this.lstMachine[k].ID))
				{
					this.dicId_Machine.Add(this.lstMachine[k].ID, this.lstMachine[k]);
				}
			}
			AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
			List<AccMorecardempGroup> modelList3 = accMorecardempGroupBll.GetModelList("");
			modelList3 = ((modelList3 == null) ? new List<AccMorecardempGroup>() : modelList3);
			this.dicPullGroup_StdGroup = new Dictionary<int, AccMorecardempGroup>();
			for (int l = 0; l < modelList3.Count; l++)
			{
				if (!this.dicPullGroup_StdGroup.ContainsKey(modelList3[l].id))
				{
					this.dicPullGroup_StdGroup.Add(modelList3[l].id, modelList3[l]);
				}
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("devicename");
			this.m_datatable.Columns.Add("InterlockType");
			this.m_datatable.Columns.Add("check");
			this.m_datatable.Columns.Add("acount");
			this.column_dev.FieldName = "devicename";
			this.column_remark.FieldName = "InterlockType";
			this.column_check.FieldName = "check";
			this.column_acount.FieldName = "acount";
		}

		private void InitBrowDataTableSet()
		{
			this.m_browdatatable.Columns.Add("id");
			this.m_browdatatable.Columns.Add("no", typeof(int));
			this.m_browdatatable.Columns.Add("name");
			this.m_browdatatable.Columns.Add("lastname");
			this.m_browdatatable.Columns.Add("Gender");
			this.m_browdatatable.Columns.Add("cardno");
			this.m_browdatatable.Columns.Add("DEPTNAME");
			this.m_browdatatable.Columns.Add("fid");
			this.m_browdatatable.Columns.Add("check");
			this.column_cardno.FieldName = "cardno";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_gender.FieldName = "Gender";
			this.column_dept.FieldName = "DEPTNAME";
			this.column_no.FieldName = "no";
			this.column_usercheck.FieldName = "check";
			this.grd_brow.DataSource = this.m_browdatatable;
		}

		private void GenderType()
		{
			try
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
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadData()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				List<AccMorecardempGroup> modelList = this.bll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = modelList[i].id;
						dataRow[1] = modelList[i].group_name;
						dataRow[2] = modelList[i].memo;
						dataRow[3] = false;
						dataRow[4] = modelList[i].Acount;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_delete.Enabled = true;
					this.btn_edit.Enabled = true;
					this.btn_addPerson.Enabled = true;
				}
				else
				{
					this.btn_delete.Enabled = false;
					this.btn_edit.Enabled = false;
					this.btn_addPerson.Enabled = false;
				}
				this.SelectionChanged(null, null);
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			MultiCardOpenGroupEdit multiCardOpenGroupEdit = new MultiCardOpenGroupEdit(0);
			multiCardOpenGroupEdit.RefreshDataEvent += this.fopen_RefreshDataEvent;
			multiCardOpenGroupEdit.ShowDialog();
			multiCardOpenGroupEdit.RefreshDataEvent -= this.fopen_RefreshDataEvent;
			this.InitialLevelDictionary();
		}

		private void fopen_RefreshDataEvent(object sender, EventArgs e)
		{
			this.LoadData();
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 0)
				{
					int[] array = null;
					if (this.isDouble)
					{
						array = this.grd_mainView.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.grd_mainView, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
					{
						if (array.Length == 1)
						{
							MultiCardOpenGroupEdit multiCardOpenGroupEdit = new MultiCardOpenGroupEdit(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							multiCardOpenGroupEdit.RefreshDataEvent += this.fopen_RefreshDataEvent;
							multiCardOpenGroupEdit.ShowDialog();
							multiCardOpenGroupEdit.RefreshDataEvent -= this.fopen_RefreshDataEvent;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一个对象"));
						}
					}
					else if (!this.isDouble)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
					}
				}
				else if (!this.isDouble)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
				this.InitialLevelDictionary();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count <= 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
				else
				{
					DataRow[] drSelected = this.m_datatable.Select("check=true");
					if (drSelected == null || drSelected.Length == 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的记录"));
					}
					else if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowProgress(0);
						this.m_wait.ShowEx();
						AccMorecardempGroupBll groupBll;
						List<AccMorecardempGroup> lstGroup;
						Dictionary<int, AccMorecardempGroup> dicGrouId_Group;
						UserInfoBll userBll;
						AccMorecardGroupBll mgBll;
						int GroupId;
						List<AccMorecardGroup> lstmcGroup;
						List<UserInfo> lstUser;
						StringBuilder filter;
						List<UserInfo> lstUserGroupChanged;
						Dictionary<int, UserInfo> dicUserId_UserSelected;
						MachinesBll machineBll;
						List<Machines> lstMachine;
						AccLevelsetEmpBll lvempBll;
						List<AccLevelsetEmp> lstLvEmp;
						ThreadPool.QueueUserWorkItem(delegate
						{
							try
							{
								groupBll = new AccMorecardempGroupBll(MainForm._ia);
								lstGroup = groupBll.GetModelList("");
								dicGrouId_Group = new Dictionary<int, AccMorecardempGroup>();
								for (int j = 0; j < lstGroup.Count; j++)
								{
									if (!dicGrouId_Group.ContainsKey(lstGroup[j].id))
									{
										dicGrouId_Group.Add(lstGroup[j].id, lstGroup[j]);
									}
								}
								userBll = new UserInfoBll(MainForm._ia);
								mgBll = new AccMorecardGroupBll(MainForm._ia);
								for (int k = 0; k < drSelected.Length; k++)
								{
									if (int.TryParse(drSelected[k]["id"].ToString(), out GroupId))
									{
										lstmcGroup = mgBll.GetModelList("group_id = " + GroupId);
										if (lstmcGroup != null && lstmcGroup.Count > 0)
										{
											this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("EmpGroup", "人员组") + ": " + drSelected[k][1].ToString() + " " + ShowMsgInfos.GetInfo("Used", "已使用") + ", " + ShowMsgInfos.GetInfo("DeleteMCSetFirst", "请先删除开门组合。"));
											this.m_wait.ShowProgress(100);
										}
										else
										{
											lstUser = userBll.GetModelList("morecard_group_id=" + GroupId);
											filter = new StringBuilder();
											lstUserGroupChanged = new List<UserInfo>();
											dicUserId_UserSelected = new Dictionary<int, UserInfo>();
											for (int l = 0; l < lstUser.Count; l++)
											{
												UserInfo userInfo = lstUser[l];
												if (!dicUserId_UserSelected.ContainsKey(userInfo.UserId))
												{
													dicUserId_UserSelected.Add(userInfo.UserId, userInfo);
													userInfo.MorecardGroupId = 0;
													if (!lstUserGroupChanged.Contains(userInfo))
													{
														lstUserGroupChanged.Add(userInfo);
														filter.AppendFormat("{0},", userInfo.UserId);
														if (lstUserGroupChanged.Count % 500 == 0)
														{
															filter.Remove(filter.Length - 1, 1);
															userBll.UpdateFields($"morecard_group_id={0}", $"UserId in ({filter.ToString()})");
															filter = new StringBuilder();
															this.m_wait.ShowProgress(40 * l / drSelected.Length);
														}
													}
												}
											}
											if (filter.Length > 0)
											{
												filter.Remove(filter.Length - 1, 1);
												userBll.UpdateFields($"morecard_group_id={0}", $"UserId in ({filter.ToString()})");
												filter = new StringBuilder();
											}
											if (base.InvokeRequired)
											{
												base.Invoke((MethodInvoker)delegate
												{
													this.m_wait.ShowProgress(40);
													this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
												});
											}
											else
											{
												this.m_wait.ShowProgress(40);
												this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
											}
											machineBll = new MachinesBll(MainForm._ia);
											lstMachine = machineBll.GetModelList("ID in (select accdoor_device_id from acc_levelset_door_group group by accdoor_device_id)");
											lvempBll = new AccLevelsetEmpBll(MainForm._ia);
											int i;
											for (i = 0; i < lstMachine.Count; i++)
											{
												lstLvEmp = lvempBll.GetModelList("acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id = " + lstMachine[i].ID + ")");
												if (lstLvEmp != null && lstLvEmp.Count > 0)
												{
													lstUser = new List<UserInfo>();
													for (int m = 0; m < lstLvEmp.Count; m++)
													{
														if (dicUserId_UserSelected.ContainsKey(lstLvEmp[m].employee_id))
														{
															UserInfo item = dicUserId_UserSelected[lstLvEmp[m].employee_id];
															if (!lstUser.Contains(item))
															{
																lstUser.Add(item);
															}
														}
													}
													if (lstUser.Count > 0)
													{
														if (lstMachine[i].DevSDKType == SDKType.StandaloneSDK)
														{
															List<Machines> list = new List<Machines>();
															list.Add(lstMachine[i]);
															CommandServer.SetUserGroup(lstUser, list, dicGrouId_Group);
														}
														else
														{
															CommandServer.AddUser(lstMachine[i], lstUser, dicGrouId_Group);
														}
													}
												}
												if (base.InvokeRequired)
												{
													base.Invoke((MethodInvoker)delegate
													{
														this.m_wait.ShowProgress(40 + 50 * i / lstMachine.Count);
													});
												}
												else
												{
													this.m_wait.ShowProgress(40 + 50 * i / lstMachine.Count);
												}
											}
											groupBll.Delete(GroupId);
											this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("EmpGroup", "人员组") + ": " + drSelected[k][1].ToString() + " " + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
										}
									}
								}
								if (base.InvokeRequired)
								{
									base.Invoke((MethodInvoker)delegate
									{
										this.grd_view.DataSource = null;
										this.LoadData();
										this.m_wait.ShowProgress(100);
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
									});
								}
								else
								{
									this.grd_view.DataSource = null;
									this.LoadData();
									this.m_wait.ShowProgress(100);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								}
							}
							catch (Exception ex2)
							{
								SysDialogs.ShowWarningMessage(ex2.Message);
							}
							finally
							{
								if (base.InvokeRequired)
								{
									base.Invoke((MethodInvoker)delegate
									{
										this.m_wait.HideEx(false);
									});
								}
								else
								{
									this.m_wait.HideEx(false);
								}
							}
						});
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			finally
			{
				this.InitialLevelDictionary();
			}
		}

		private void btn_addPerson_Click(object sender, EventArgs e)
		{
			try
			{
				this.selectid = -1;
				if (this.m_datatable.Rows.Count > 0)
				{
					int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
					if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
					{
						if (checkedRows.Length == 1)
						{
							this.selectid = int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString());
							SelectedMultiPersonnelForm selectedMultiPersonnelForm = new SelectedMultiPersonnelForm("morecard_group_id is null or morecard_group_id not in (select id from acc_morecardempgroup) ");
							selectedMultiPersonnelForm.SelectUserEvent += this.frmSelectUser_SelectUserEvent;
							selectedMultiPersonnelForm.Text = this.btn_addPerson.Text;
							selectedMultiPersonnelForm.ShowDialog(this);
							selectedMultiPersonnelForm.SelectUserEvent -= this.frmSelectUser_SelectUserEvent;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void frmSelectUser_SelectUserEvent(List<UserInfo> lstUserSelected)
		{
			try
			{
				if (this.selectid > 0 && lstUserSelected != null && lstUserSelected.Count > 0)
				{
					this.m_wait.ShowEx();
					AccMorecardempGroupBll groupBll;
					List<AccMorecardempGroup> lstGroup;
					Dictionary<int, AccMorecardempGroup> dicGrouId_Group;
					UserInfoBll userBll;
					List<UserInfo> lstUser;
					Dictionary<int, UserInfo> dicUserId_User;
					StringBuilder filter;
					List<UserInfo> lstUserGroupChanged;
					Dictionary<int, UserInfo> dicUserId_UserSelected;
					MachinesBll machineBll;
					List<Machines> lstMachine;
					AccLevelsetEmpBll lvempBll;
					int i;
					List<AccLevelsetEmp> lstLvEmp;
					ThreadPool.QueueUserWorkItem(delegate
					{
						try
						{
							groupBll = new AccMorecardempGroupBll(MainForm._ia);
							lstGroup = groupBll.GetModelList("");
							dicGrouId_Group = new Dictionary<int, AccMorecardempGroup>();
							for (int j = 0; j < lstGroup.Count; j++)
							{
								if (!dicGrouId_Group.ContainsKey(lstGroup[j].id))
								{
									dicGrouId_Group.Add(lstGroup[j].id, lstGroup[j]);
								}
							}
							userBll = new UserInfoBll(MainForm._ia);
							lstUser = userBll.GetModelList("");
							dicUserId_User = new Dictionary<int, UserInfo>();
							for (int k = 0; k < lstUser.Count; k++)
							{
								if (!dicUserId_User.ContainsKey(lstUser[k].UserId))
								{
									dicUserId_User.Add(lstUser[k].UserId, lstUser[k]);
								}
							}
							lstUser = null;
							filter = new StringBuilder();
							lstUserGroupChanged = new List<UserInfo>();
							dicUserId_UserSelected = new Dictionary<int, UserInfo>();
							for (int l = 0; l < lstUserSelected.Count; l++)
							{
								if (!dicUserId_UserSelected.ContainsKey(lstUserSelected[l].UserId) && dicUserId_User.ContainsKey(lstUserSelected[l].UserId))
								{
									UserInfo userInfo = dicUserId_User[lstUserSelected[l].UserId];
									dicUserId_UserSelected.Add(userInfo.UserId, userInfo);
									userInfo.MorecardGroupId = this.selectid;
									if (!lstUserGroupChanged.Contains(userInfo))
									{
										lstUserGroupChanged.Add(userInfo);
										filter.AppendFormat("{0},", userInfo.UserId);
										if (lstUserGroupChanged.Count % 500 == 0)
										{
											filter.Remove(filter.Length - 1, 1);
											userBll.UpdateFields($"morecard_group_id={this.selectid}", $"UserId in ({filter.ToString()})");
											filter = new StringBuilder();
											this.m_wait.ShowProgress(40 * l / lstUserSelected.Count);
										}
									}
								}
							}
							if (filter.Length > 0)
							{
								filter.Remove(filter.Length - 1, 1);
								userBll.UpdateFields($"morecard_group_id={this.selectid}", $"UserId in ({filter.ToString()})");
								filter = new StringBuilder();
							}
							dicUserId_User = null;
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.m_wait.ShowProgress(40);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
								});
							}
							else
							{
								this.m_wait.ShowProgress(40);
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
							}
							machineBll = new MachinesBll(MainForm._ia);
							lstMachine = machineBll.GetModelList("ID in (select accdoor_device_id from acc_levelset_door_group group by accdoor_device_id)");
							lvempBll = new AccLevelsetEmpBll(MainForm._ia);
							for (i = 0; i < lstMachine.Count; i++)
							{
								lstLvEmp = lvempBll.GetModelList("acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id = " + lstMachine[i].ID + ")");
								if (lstLvEmp != null && lstLvEmp.Count > 0)
								{
									lstUser = new List<UserInfo>();
									for (int m = 0; m < lstLvEmp.Count; m++)
									{
										if (dicUserId_UserSelected.ContainsKey(lstLvEmp[m].employee_id))
										{
											UserInfo item = dicUserId_UserSelected[lstLvEmp[m].employee_id];
											if (!lstUser.Contains(item))
											{
												lstUser.Add(item);
											}
										}
									}
									if (lstUser.Count > 0)
									{
										if (lstMachine[i].DevSDKType == SDKType.StandaloneSDK)
										{
											List<Machines> list = new List<Machines>();
											list.Add(lstMachine[i]);
											CommandServer.SetUserGroup(lstUser, list, dicGrouId_Group);
										}
										else
										{
											CommandServer.AddUser(lstMachine[i], lstUser, dicGrouId_Group);
										}
									}
								}
								if (base.InvokeRequired)
								{
									base.Invoke((MethodInvoker)delegate
									{
										this.m_wait.ShowProgress(40 + 50 * i / lstMachine.Count);
									});
								}
								else
								{
									this.m_wait.ShowProgress(40 + 50 * i / lstMachine.Count);
								}
							}
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.grd_view.DataSource = null;
									this.LoadData();
									this.m_wait.ShowProgress(100);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								});
							}
							else
							{
								this.grd_view.DataSource = null;
								this.LoadData();
								this.m_wait.ShowProgress(100);
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
							}
						}
						catch (Exception ex2)
						{
							SysDialogs.ShowWarningMessage(ex2.Message);
						}
						finally
						{
							if (base.InvokeRequired)
							{
								base.Invoke((MethodInvoker)delegate
								{
									this.m_wait.HideEx(false);
								});
							}
							else
							{
								this.m_wait.HideEx(false);
							}
						}
					});
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SelectionChanged(object sender, EventArgs e)
		{
			try
			{
				this.m_browdatatable.Rows.Clear();
				string value = string.Empty;
				string value2 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					value = dictionary["m"];
					value2 = dictionary["f"];
				}
				this.btn_deletePerson.Enabled = false;
				int[] selectedRows = this.grd_mainView.GetSelectedRows();
				selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.grd_mainView, selectedRows);
				if (selectedRows != null && selectedRows.Length != 0 && selectedRows[0] >= 0 && selectedRows[0] < this.m_datatable.Rows.Count)
				{
					int num = int.Parse(this.m_datatable.Rows[selectedRows[0]][0].ToString());
					UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
					List<UserInfo> modelList = userInfoBll.GetModelList(" USERINFO.morecard_group_id=" + num + " ");
					if (modelList != null && modelList.Count > 0)
					{
						for (int i = 0; i < modelList.Count; i++)
						{
							DataRow dataRow = this.m_browdatatable.NewRow();
							dataRow[0] = modelList[i].UserId.ToString();
							dataRow[1] = int.Parse(modelList[i].BadgeNumber);
							dataRow[2] = modelList[i].Name;
							dataRow[3] = modelList[i].LastName;
							if (!string.IsNullOrEmpty(modelList[i].Gender) && modelList[i].Gender == "F")
							{
								dataRow[4] = value2;
							}
							else
							{
								dataRow[4] = value;
							}
							dataRow[5] = modelList[i].CardNo;
							dataRow[6] = modelList[i].DeptName;
							dataRow[7] = num;
							dataRow[8] = false;
							this.m_browdatatable.Rows.Add(dataRow);
						}
						this.btn_deletePerson.Enabled = true;
					}
				}
				this.column_usercheck.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_deletePerson_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browdatatable.Rows.Count <= 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
				else
				{
					DataRow[] drSelected = this.m_browdatatable.Select("check=true");
					if (drSelected == null || drSelected.Length == 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的记录"));
					}
					else if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						AccMorecardempGroupBll groupBll;
						List<AccMorecardempGroup> lstGroup;
						Dictionary<int, AccMorecardempGroup> dicGrouId_Group;
						UserInfoBll userBll;
						List<UserInfo> lstUser;
						Dictionary<int, UserInfo> dicUserId_User;
						StringBuilder filter;
						List<UserInfo> lstUserGroupChanged;
						Dictionary<int, UserInfo> dicUserId_UserSelected;
						int UserId;
						MachinesBll machineBll;
						List<Machines> lstMachine;
						AccLevelsetEmpBll lvempBll;
						int i;
						List<AccLevelsetEmp> lstLvEmp;
						ThreadPool.QueueUserWorkItem(delegate
						{
							try
							{
								groupBll = new AccMorecardempGroupBll(MainForm._ia);
								lstGroup = groupBll.GetModelList("");
								dicGrouId_Group = new Dictionary<int, AccMorecardempGroup>();
								for (int j = 0; j < lstGroup.Count; j++)
								{
									if (!dicGrouId_Group.ContainsKey(lstGroup[j].id))
									{
										dicGrouId_Group.Add(lstGroup[j].id, lstGroup[j]);
									}
								}
								userBll = new UserInfoBll(MainForm._ia);
								lstUser = userBll.GetModelList("");
								dicUserId_User = new Dictionary<int, UserInfo>();
								for (int k = 0; k < lstUser.Count; k++)
								{
									if (!dicUserId_User.ContainsKey(lstUser[k].UserId))
									{
										dicUserId_User.Add(lstUser[k].UserId, lstUser[k]);
									}
								}
								lstUser = null;
								filter = new StringBuilder();
								lstUserGroupChanged = new List<UserInfo>();
								dicUserId_UserSelected = new Dictionary<int, UserInfo>();
								for (int l = 0; l < drSelected.Length; l++)
								{
									if (int.TryParse(drSelected[l]["id"].ToString(), out UserId) && !dicUserId_UserSelected.ContainsKey(UserId) && dicUserId_User.ContainsKey(UserId))
									{
										UserInfo userInfo = dicUserId_User[UserId];
										dicUserId_UserSelected.Add(userInfo.UserId, userInfo);
										userInfo.MorecardGroupId = 0;
										if (!lstUserGroupChanged.Contains(userInfo))
										{
											lstUserGroupChanged.Add(userInfo);
											filter.AppendFormat("{0},", userInfo.UserId);
											if (lstUserGroupChanged.Count % 500 == 0)
											{
												filter.Remove(filter.Length - 1, 1);
												userBll.UpdateFields($"morecard_group_id={0}", $"UserId in ({filter.ToString()})");
												filter = new StringBuilder();
												this.m_wait.ShowProgress(40 * l / drSelected.Length);
											}
										}
									}
								}
								if (filter.Length > 0)
								{
									filter.Remove(filter.Length - 1, 1);
									userBll.UpdateFields($"morecard_group_id={0}", $"UserId in ({filter.ToString()})");
									filter = new StringBuilder();
								}
								dicUserId_User = null;
								if (base.InvokeRequired)
								{
									base.Invoke((MethodInvoker)delegate
									{
										this.m_wait.ShowProgress(40);
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
									});
								}
								else
								{
									this.m_wait.ShowProgress(40);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
								}
								machineBll = new MachinesBll(MainForm._ia);
								lstMachine = machineBll.GetModelList("ID in (select accdoor_device_id from acc_levelset_door_group group by accdoor_device_id)");
								lvempBll = new AccLevelsetEmpBll(MainForm._ia);
								for (i = 0; i < lstMachine.Count; i++)
								{
									lstLvEmp = lvempBll.GetModelList("acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id = " + lstMachine[i].ID + ")");
									if (lstLvEmp != null && lstLvEmp.Count > 0)
									{
										lstUser = new List<UserInfo>();
										for (int m = 0; m < lstLvEmp.Count; m++)
										{
											if (dicUserId_UserSelected.ContainsKey(lstLvEmp[m].employee_id))
											{
												UserInfo item = dicUserId_UserSelected[lstLvEmp[m].employee_id];
												if (!lstUser.Contains(item))
												{
													lstUser.Add(item);
												}
											}
										}
										if (lstUser.Count > 0)
										{
											if (lstMachine[i].DevSDKType == SDKType.StandaloneSDK)
											{
												List<Machines> list = new List<Machines>();
												list.Add(lstMachine[i]);
												CommandServer.SetUserGroup(lstUser, list, dicGrouId_Group);
											}
											else
											{
												CommandServer.AddUser(lstMachine[i], lstUser, dicGrouId_Group);
											}
										}
									}
									if (base.InvokeRequired)
									{
										base.Invoke((MethodInvoker)delegate
										{
											this.m_wait.ShowProgress(40 + 50 * i / lstMachine.Count);
										});
									}
									else
									{
										this.m_wait.ShowProgress(40 + 50 * i / lstMachine.Count);
									}
								}
								if (base.InvokeRequired)
								{
									base.Invoke((MethodInvoker)delegate
									{
										this.grd_view.DataSource = null;
										this.LoadData();
										this.m_wait.ShowProgress(100);
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
									});
								}
								else
								{
									this.grd_view.DataSource = null;
									this.LoadData();
									this.m_wait.ShowProgress(100);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								}
							}
							catch (Exception ex2)
							{
								SysDialogs.ShowWarningMessage(ex2.Message);
							}
							finally
							{
								if (base.InvokeRequired)
								{
									base.Invoke((MethodInvoker)delegate
									{
										this.m_wait.HideEx(false);
									});
								}
								else
								{
									this.m_wait.HideEx(false);
								}
							}
						});
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void menu_delall_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_datatable.Rows.Count > 0)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						for (int i = 0; i < this.m_datatable.Rows.Count; i++)
						{
							this.m_wait.ShowProgress(100 * i / this.m_datatable.Rows.Count);
							if (this.m_datatable.Rows[i][0] == null)
							{
								break;
							}
							this.m_wait.ShowProgress(0);
							List<UserInfo> modelList = userInfoBll.GetModelList("UserInfo.morecard_group_id=" + int.Parse(this.m_datatable.Rows[i][0].ToString()));
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
							if (modelList != null && modelList.Count > 0)
							{
								for (int j = 0; j < modelList.Count; j++)
								{
									modelList[j].MorecardGroupId = 0;
									CommandServer.AddCmd(modelList[j], false, "");
									this.m_wait.ShowProgress(100 * j / modelList.Count);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + modelList[j].BadgeNumber + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								}
							}
							this.bll.Delete(int.Parse(this.m_datatable.Rows[i][0].ToString()));
							this.m_wait.ShowProgress(100);
							this.m_wait.ShowInfos(this.m_datatable.Rows[i][1].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						}
						this.m_datatable.Rows.Clear();
						this.LoadData();
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void menu_delallex_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.m_browdatatable.Rows.Count > 0)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
						if (this.m_browdatatable.Rows[0][0] != null)
						{
							this.m_wait.ShowProgress(0);
							int num = int.Parse(this.m_browdatatable.Rows[0][4].ToString());
							List<UserInfo> modelList = userInfoBll.GetModelList("UserInfo.morecard_group_id=" + num);
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SaveLevelInfoWaiting", "开始生成命令，时间可能比较长，请稍等..."));
							if (modelList != null && modelList.Count > 0)
							{
								for (int i = 0; i < modelList.Count; i++)
								{
									modelList[i].MorecardGroupId = 0;
									CommandServer.AddCmd(modelList[i], false, "");
									this.m_wait.ShowProgress(100 * i / modelList.Count);
									this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + modelList[i].BadgeNumber + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
								}
							}
							userInfoBll.DeleteGroupID(num);
							this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
							this.m_wait.ShowProgress(100);
						}
						this.m_browdatatable.Rows.Clear();
						this.btn_deletePerson.Enabled = false;
						this.m_wait.HideEx(false);
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(sender, e);
			this.isDouble = false;
		}

		private void grd_mainView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
			this.SelectionChanged(null, null);
		}

		private void grd_mainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_mainView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void grd_browview_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MultiCardOpenGroupSet));
			this.contextMenuStrip2 = new ContextMenuStrip(this.components);
			this.menu_delex = new ToolStripMenuItem();
			this.menu_delallex = new ToolStripMenuItem();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_add = new ToolStripMenuItem();
			this.menu_edit = new ToolStripMenuItem();
			this.menu_del = new ToolStripMenuItem();
			this.menu_delall = new ToolStripMenuItem();
			this.menu_addgroupuser = new ToolStripMenuItem();
			this.btn_cancel = new ButtonX();
			this.grd_brow = new GridControl();
			this.grd_browview = new GridView();
			this.column_usercheck = new GridColumn();
			this.column_no = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_gender = new GridColumn();
			this.column_cardno = new GridColumn();
			this.column_dept = new GridColumn();
			this.btn_deletePerson = new ButtonX();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_remark = new GridColumn();
			this.column_acount = new GridColumn();
			this.bandedGridView1 = new BandedGridView();
			this.btn_addPerson = new ButtonX();
			this.btn_add = new ButtonX();
			this.btn_delete = new ButtonX();
			this.btn_edit = new ButtonX();
			this.lb_group = new LabelX();
			this.lb_brow = new LabelX();
			this.contextMenuStrip2.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			((ISupportInitialize)this.grd_brow).BeginInit();
			((ISupportInitialize)this.grd_browview).BeginInit();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			((ISupportInitialize)this.bandedGridView1).BeginInit();
			base.SuspendLayout();
			this.contextMenuStrip2.Items.AddRange(new ToolStripItem[2]
			{
				this.menu_delex,
				this.menu_delallex
			});
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new Size(155, 48);
			this.menu_delex.Name = "menu_delex";
			this.menu_delex.Size = new Size(154, 22);
			this.menu_delex.Text = "删除组人员";
			this.menu_delex.Click += this.btn_deletePerson_Click;
			this.menu_delallex.Name = "menu_delallex";
			this.menu_delallex.Size = new Size(154, 22);
			this.menu_delallex.Text = "删除全部组人员";
			this.menu_delallex.Visible = false;
			this.menu_delallex.Click += this.menu_delallex_Click;
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[5]
			{
				this.menu_add,
				this.menu_edit,
				this.menu_del,
				this.menu_delall,
				this.menu_addgroupuser
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(153, 136);
			this.menu_add.Name = "menu_add";
			this.menu_add.Size = new Size(130, 22);
			this.menu_add.Text = "新增";
			this.menu_add.Click += this.btn_add_Click;
			this.menu_edit.Name = "menu_edit";
			this.menu_edit.Size = new Size(130, 22);
			this.menu_edit.Text = "编辑";
			this.menu_edit.Click += this.btn_edit_Click;
			this.menu_del.Name = "menu_del";
			this.menu_del.Size = new Size(130, 22);
			this.menu_del.Text = "删除";
			this.menu_del.Click += this.btn_delete_Click;
			this.menu_delall.Name = "menu_delall";
			this.menu_delall.Size = new Size(152, 22);
			this.menu_delall.Text = "全部删除";
			this.menu_delall.Visible = false;
			this.menu_delall.Click += this.menu_delall_Click;
			this.menu_addgroupuser.Name = "menu_addgroupuser";
			this.menu_addgroupuser.Size = new Size(130, 22);
			this.menu_addgroupuser.Text = "添加组人员";
			this.menu_addgroupuser.Click += this.btn_addPerson_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(944, 470);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(100, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.grd_brow.Location = new Point(538, 42);
			this.grd_brow.MainView = this.grd_browview;
			this.grd_brow.Name = "grd_brow";
			this.grd_brow.Size = new Size(506, 415);
			this.grd_brow.TabIndex = 11;
			this.grd_brow.TabStop = false;
			this.grd_brow.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_browview
			});
			this.grd_browview.Columns.AddRange(new GridColumn[7]
			{
				this.column_usercheck,
				this.column_no,
				this.column_name,
				this.column_lastName,
				this.column_gender,
				this.column_cardno,
				this.column_dept
			});
			this.grd_browview.GridControl = this.grd_brow;
			this.grd_browview.IndicatorWidth = 35;
			this.grd_browview.Name = "grd_browview";
			this.grd_browview.OptionsSelection.MultiSelect = true;
			this.grd_browview.PaintStyleName = "Office2003";
			this.grd_browview.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_browview.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_browview.Click += this.grd_browview_Click;
			this.column_usercheck.Name = "column_usercheck";
			this.column_usercheck.OptionsColumn.AllowEdit = false;
			this.column_usercheck.OptionsColumn.ReadOnly = true;
			this.column_usercheck.Visible = true;
			this.column_usercheck.VisibleIndex = 0;
			this.column_usercheck.Width = 40;
			this.column_no.AppearanceCell.Options.UseTextOptions = true;
			this.column_no.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Near;
			this.column_no.Caption = "人员编号";
			this.column_no.Name = "column_no";
			this.column_no.OptionsColumn.AllowEdit = false;
			this.column_no.OptionsColumn.ReadOnly = true;
			this.column_no.Visible = true;
			this.column_no.VisibleIndex = 1;
			this.column_no.Width = 83;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.OptionsColumn.AllowEdit = false;
			this.column_name.OptionsColumn.ReadOnly = true;
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 2;
			this.column_name.Width = 83;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_gender.Caption = "性别";
			this.column_gender.Name = "column_gender";
			this.column_gender.Visible = true;
			this.column_gender.VisibleIndex = 4;
			this.column_cardno.Caption = "卡号";
			this.column_cardno.Name = "column_cardno";
			this.column_cardno.OptionsColumn.AllowEdit = false;
			this.column_cardno.OptionsColumn.ReadOnly = true;
			this.column_cardno.Visible = true;
			this.column_cardno.VisibleIndex = 5;
			this.column_cardno.Width = 87;
			this.column_dept.Caption = "部门名称";
			this.column_dept.Name = "column_dept";
			this.column_dept.Visible = true;
			this.column_dept.VisibleIndex = 6;
			this.btn_deletePerson.AccessibleRole = AccessibleRole.PushButton;
			this.btn_deletePerson.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_deletePerson.Location = new Point(538, 470);
			this.btn_deletePerson.Name = "btn_deletePerson";
			this.btn_deletePerson.Size = new Size(188, 23);
			this.btn_deletePerson.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_deletePerson.TabIndex = 4;
			this.btn_deletePerson.Text = "删除人员";
			this.btn_deletePerson.Click += this.btn_deletePerson_Click;
			this.grd_view.Location = new Point(12, 42);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(506, 415);
			this.grd_view.TabIndex = 10;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[2]
			{
				this.grd_mainView,
				this.bandedGridView1
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[4]
			{
				this.column_check,
				this.column_dev,
				this.column_remark,
				this.column_acount
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsSelection.MultiSelect = true;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.grd_mainView.DoubleClick += this.grd_mainView_DoubleClick;
			this.column_check.Name = "column_check";
			this.column_check.OptionsColumn.AllowEdit = false;
			this.column_check.OptionsColumn.ReadOnly = true;
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 35;
			this.column_dev.Caption = "组名称";
			this.column_dev.Name = "column_dev";
			this.column_dev.OptionsColumn.AllowEdit = false;
			this.column_dev.OptionsColumn.ReadOnly = true;
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 1;
			this.column_dev.Width = 98;
			this.column_remark.Caption = "备注";
			this.column_remark.Name = "column_remark";
			this.column_remark.OptionsColumn.AllowEdit = false;
			this.column_remark.OptionsColumn.ReadOnly = true;
			this.column_remark.Visible = true;
			this.column_remark.VisibleIndex = 2;
			this.column_remark.Width = 99;
			this.column_acount.Caption = "人数";
			this.column_acount.Name = "column_acount";
			this.column_acount.Visible = true;
			this.column_acount.VisibleIndex = 3;
			this.column_acount.Width = 40;
			this.bandedGridView1.GridControl = this.grd_view;
			this.bandedGridView1.Name = "bandedGridView1";
			this.bandedGridView1.ShowButtonMode = ShowButtonModeEnum.ShowForFocusedRow;
			this.btn_addPerson.AccessibleRole = AccessibleRole.PushButton;
			this.btn_addPerson.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_addPerson.Location = new Point(330, 470);
			this.btn_addPerson.Name = "btn_addPerson";
			this.btn_addPerson.Size = new Size(188, 23);
			this.btn_addPerson.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_addPerson.TabIndex = 3;
			this.btn_addPerson.Text = "添加人员";
			this.btn_addPerson.Click += this.btn_addPerson_Click;
			this.btn_add.AccessibleRole = AccessibleRole.PushButton;
			this.btn_add.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_add.Location = new Point(12, 470);
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(90, 23);
			this.btn_add.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_add.TabIndex = 0;
			this.btn_add.Text = "新增";
			this.btn_add.Click += this.btn_add_Click;
			this.btn_delete.AccessibleRole = AccessibleRole.PushButton;
			this.btn_delete.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_delete.Location = new Point(224, 470);
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(90, 23);
			this.btn_delete.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_delete.TabIndex = 2;
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.btn_edit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_edit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_edit.Location = new Point(118, 470);
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(90, 23);
			this.btn_edit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_edit.TabIndex = 1;
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.lb_group.BackgroundStyle.Class = "";
			this.lb_group.Location = new Point(12, 19);
			this.lb_group.Name = "lb_group";
			this.lb_group.Size = new Size(429, 18);
			this.lb_group.TabIndex = 39;
			this.lb_group.Text = "多卡开门人员组列表";
			this.lb_brow.BackgroundStyle.Class = "";
			this.lb_brow.Location = new Point(541, 19);
			this.lb_brow.Name = "lb_brow";
			this.lb_brow.Size = new Size(389, 18);
			this.lb_brow.TabIndex = 40;
			this.lb_brow.Text = "浏览多卡开门组人员";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1056, 505);
			base.Controls.Add(this.lb_group);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.lb_brow);
			base.Controls.Add(this.btn_edit);
			base.Controls.Add(this.grd_brow);
			base.Controls.Add(this.btn_delete);
			base.Controls.Add(this.btn_add);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_deletePerson);
			base.Controls.Add(this.btn_addPerson);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MultiCardOpenGroupSet";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "多卡开门人员组设置";
			this.contextMenuStrip2.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			((ISupportInitialize)this.grd_brow).EndInit();
			((ISupportInitialize)this.grd_browview).EndInit();
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			((ISupportInitialize)this.bandedGridView1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
