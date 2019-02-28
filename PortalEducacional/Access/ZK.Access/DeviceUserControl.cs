/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
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

namespace ZK.Access
{
	public class DeviceUserControl : UserControl
	{
		private MachinesBll m_machinesBll = new MachinesBll(MainForm._ia);

		private string strCondtion;

		private const bool ENABLE = true;

		private const bool DISABLE = false;

		private Dictionary<int, string> m_areaTable = new Dictionary<int, string>();

		private int deviceCount = 0;

		private byte[] m_enableImg = null;

		private byte[] m_disableImg = null;

		private byte[] imgNet_On = null;

		private byte[] imgNet_Off = null;

		private Dictionary<int, Machines> dicMachineId_Machine;

		private WaitForm m_wait = WaitForm.Instance;

		private DataTable m_datatable = new DataTable();

		private bool isDouble = false;

		private List<Thread> lstThread = new List<Thread>();

		private IContainer components = null;

		public PanelEx panelEx1;

		private ContextMenuStrip contextMenuStrip2;

		private ToolStripMenuItem Menu_add;

		private ToolStripMenuItem Menu_edit;

		private ToolStripMenuItem Menu_delete;

		private ToolStripMenuItem Menu_Export;

		private ToolStripMenuItem Menu_GetEvent;

		private ToolStripMenuItem Menu_SyncToPCEx;

		private GridControl grd_view;

		private GridView grd_MachineView;

		private GridColumn column_MachineAlias;

		private GridColumn column_sn;

		private GridColumn column_ConnectType;

		private GridColumn column_IP;

		private GridColumn column_SerialPort;

		private GridColumn column_MachineNumber;

		private GridColumn column_area_id;

		private GridColumn column_Enabled;

		private GridColumn column_acpanel_type;

		private GridColumn column_FirmwareVersion;

		private GridColumn column_usercount;

		private GridColumn column_fingercount;

		private ToolStripButton btn_add;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_devToPC;

		private ToolStripButton btn_GetEvent;

		private ToolStripButton btn_SyncToPCEx;

		public ToolStrip MenuPanelEx;

		private RepositoryItemPictureEdit repositoryItemPictureEdit1;

		private GridColumn column_check;

		private ToolStripButton btn_delete;

		private ToolStripButton btn_search;

		private ToolStripMenuItem Menu_SyncToPC;

		private GridColumn column_fvcount;

		private ToolStripDropDownButton toolStripDropDownButton2;

		private ToolStripMenuItem btn_Disable;

		private ToolStripMenuItem btn_Enable;

		private ToolStripMenuItem btn_UpgradeFirmware;

		private ToolStripMenuItem btn_SynchronizeTime;

		private ToolStripMenuItem btn_ModifyIP;

		private ToolStripMenuItem btn_CloseAuxiliary;

		private ToolStripMenuItem btn_ModifyComPwd;

		private ToolStripMenuItem btn_GetUserInfo1;

		private ToolStripMenuItem btn_ChangeFPThreshold;

		private ToolStripMenuItem btn_disableTime;

		private ToolStripMenuItem btn_enableTime;

		private ToolStripMenuItem btn_modifyBaudrate;

		private ToolStripMenuItem btn_check;

		private ToolStripMenuItem btn_Log;

		private ToolStripMenuItem btn_SyncToPC;

		private ToolStripMenuItem btn_Export;

		private ToolStripMenuItem btn_Sd_Record;

		private ToolStripMenuItem btn_RS485_MasterSlave;

		private GridColumn colOnlineState;

		private ToolStripMenuItem btn_GetDataFromUdisk;

		private ToolStripMenuItem btn_Export2Udisk;

		private GridColumn colFaceCount;

		private ToolStripButton btn_GetUserInfo;

		private ToolStripMenuItem rebootToolStripMenuItem;

		private ToolStripMenuItem resetDeFábricaToolStripMenuItem;

		public DeviceUserControl()
		{
			this.InitializeComponent();
			try
			{
				base.Disposed += this.DeviceUserControl_Disposed;
				DevExpressHelper.InitImageList(this.grd_MachineView, "column_check");
				this.InitDataTableSet();
				this.GetAreaInfo();
				this.m_enableImg = this.getImageByte(Application.StartupPath + "\\img\\devEnable.gif");
				this.m_disableImg = this.getImageByte(Application.StartupPath + "\\img\\devDisable.gif");
				this.imgNet_On = this.getImageByte(Application.StartupPath + "\\img\\Net_On.png");
				this.imgNet_Off = this.getImageByte(Application.StartupPath + "\\img\\Net_Off.png");
				if (AccCommon.IsECardTong == 1)
				{
					this.DataBind(" AccFun = 255 ");
				}
				else
				{
					this.DataBind("");
				}
				initLang.LocaleForm(this, base.Name);
				this.ChangeSking();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.CheckPermission();
		}

		private void ChangeSking()
		{
			int skinOption = SkinParameters.SkinOption;
			if (skinOption == 1)
			{
				this.btn_add.Visible = false;
				this.btn_delete.Visible = false;
				this.btn_edit.Visible = false;
				this.btn_disableTime.Visible = false;
				this.btn_enableTime.Visible = false;
				this.btn_add.Image = ResourceIPC.add;
				this.btn_delete.Image = ResourceIPC.delete;
				this.btn_edit.Image = ResourceIPC.edit;
				this.btn_search.Image = ResourceIPC.search;
				this.btn_ChangeFPThreshold.Image = ResourceIPC.editfinger;
				this.btn_CloseAuxiliary.Image = ResourceIPC.close_Auxiliary_Output;
				this.btn_devToPC.Image = ResourceIPC.GetDataFromDev;
				this.btn_Disable.Image = ResourceIPC.disable;
				this.btn_Enable.Image = ResourceIPC.enabled;
				this.btn_Export.Image = ResourceIPC.Export1;
				this.btn_Export2Udisk.Image = ResourceIPC.ExportUDisk;
				this.btn_GetDataFromUdisk.Image = ResourceIPC.ImportUDisk;
				this.btn_GetEvent.Image = ResourceIPC.Get_event_entries;
				this.btn_GetUserInfo.Image = ResourceIPC.Count;
				this.btn_GetUserInfo1.Image = ResourceIPC.get_information_of_personnel;
				this.btn_Log.Image = ResourceIPC.Log_Entries1;
				this.btn_modifyBaudrate.Image = ResourceIPC.edit_BaudRate;
				this.btn_ModifyComPwd.Image = ResourceIPC.editPws;
				this.btn_ModifyIP.Image = ResourceIPC.editIP;
				this.btn_RS485_MasterSlave.Image = ResourceIPC.path;
				this.btn_Sd_Record.Image = ResourceIPC.system_Initialization;
				this.btn_search.Image = ResourceIPC.search;
				this.btn_SynchronizeTime.Image = ResourceIPC.Real_Time1;
				this.btn_SyncToPC.Image = ResourceIPC.synchronize_the_data1;
				this.btn_SyncToPCEx.Image = ResourceIPC.synchronize_the_data;
				this.btn_UpgradeFirmware.Image = ResourceIPC.upgrade_firmware;
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Device))
			{
				this.btn_add.Enabled = false;
				this.btn_ChangeFPThreshold.Enabled = false;
				this.btn_CloseAuxiliary.Enabled = false;
				this.btn_delete.Enabled = false;
				this.btn_Disable.Enabled = false;
				this.btn_disableTime.Enabled = false;
				this.btn_edit.Enabled = false;
				this.btn_Enable.Enabled = false;
				this.btn_enableTime.Enabled = false;
				this.btn_GetEvent.Enabled = false;
				this.btn_GetUserInfo1.Enabled = false;
				this.btn_GetUserInfo.Enabled = false;
				this.btn_modifyBaudrate.Enabled = false;
				this.btn_ModifyComPwd.Enabled = false;
				this.btn_ModifyIP.Enabled = false;
				this.btn_SynchronizeTime.Enabled = false;
				this.btn_SyncToPCEx.Enabled = false;
				this.btn_UpgradeFirmware.Enabled = false;
				this.btn_devToPC.Enabled = false;
				this.btn_Log.Enabled = false;
				this.btn_check.Enabled = false;
				this.Menu_add.Enabled = false;
				this.Menu_delete.Enabled = false;
				this.Menu_edit.Enabled = false;
				this.Menu_Export.Enabled = false;
				this.Menu_GetEvent.Enabled = false;
				this.Menu_SyncToPCEx.Enabled = false;
				this.Menu_SyncToPC.Enabled = false;
				this.btn_SyncToPC.Enabled = false;
				this.btn_search.Enabled = false;
				this.btn_Sd_Record.Enabled = false;
				this.btn_Export.Enabled = false;
				this.btn_RS485_MasterSlave.Enabled = false;
				this.grd_view.DoubleClick -= this.grd_view_DoubleClick;
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("ID");
			this.m_datatable.Columns.Add("MachineAlias");
			this.m_datatable.Columns.Add("sn");
			this.m_datatable.Columns.Add("ConnectType");
			this.m_datatable.Columns.Add("IP");
			this.m_datatable.Columns.Add("SerialPort");
			this.m_datatable.Columns.Add("MachineNumber");
			this.m_datatable.Columns.Add("area_id");
			this.m_datatable.Columns.Add("Enabled", Type.GetType("System.Byte[]"));
			this.m_datatable.Columns.Add("device_name");
			this.m_datatable.Columns.Add("FirmwareVersion");
			this.m_datatable.Columns.Add("usercount");
			this.m_datatable.Columns.Add("fingercount");
			this.m_datatable.Columns.Add("fvcount");
			this.m_datatable.Columns.Add("check", Type.GetType("System.Boolean"));
			this.m_datatable.Columns.Add("OnlineState", typeof(byte[]));
			this.m_datatable.Columns.Add("SDKType", typeof(int));
			this.m_datatable.Columns.Add("FaceCount");
			this.column_MachineAlias.FieldName = "MachineAlias";
			this.column_sn.FieldName = "sn";
			this.column_ConnectType.FieldName = "ConnectType";
			this.column_IP.FieldName = "IP";
			this.column_SerialPort.FieldName = "SerialPort";
			this.column_MachineNumber.FieldName = "MachineNumber";
			this.column_area_id.FieldName = "area_id";
			this.column_Enabled.FieldName = "Enabled";
			this.column_acpanel_type.FieldName = "device_name";
			this.column_FirmwareVersion.FieldName = "FirmwareVersion";
			this.column_usercount.FieldName = "usercount";
			this.column_fingercount.FieldName = "fingercount";
			this.column_fvcount.FieldName = "fvcount";
			this.column_check.FieldName = "check";
			this.colOnlineState.FieldName = "OnlineState";
			this.colFaceCount.FieldName = "FaceCount";
		}

		private void frm_Event(object sender, EventArgs e)
		{
			if (AccCommon.IsECardTong == 1)
			{
				this.DataBind(" AccFun = 255 ");
			}
			else
			{
				this.DataBind("");
			}
		}

		private void refresh_Event(object sender, EventArgs e)
		{
			if (AccCommon.IsECardTong == 1)
			{
				this.DataBind(" AccFun = 255 ");
			}
			else
			{
				this.DataBind("");
			}
		}

		private void GetAreaInfo()
		{
			try
			{
				this.m_areaTable.Clear();
				PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
				List<PersonnelArea> modelList = personnelAreaBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.m_areaTable.Add(modelList[i].id, modelList[i].areaname);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			try
			{
				if (AccCommon.IsECardTong > 0 && this.deviceCount >= 30)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SCannotMoreThan", "系统最大支持门禁控制器台数:") + 30);
				}
				else
				{
					Program.IsRegistZKECardTong();
					AddDeviceForm addDeviceForm = new AddDeviceForm(0);
					addDeviceForm.Text = this.btn_add.Text;
					addDeviceForm.refreshDataEvent += this.frm_Event;
					addDeviceForm.ShowDialog();
					addDeviceForm.refreshDataEvent -= this.frm_Event;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
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
						array = this.grd_MachineView.GetSelectedRows();
						array = DevExpressHelper.GetDataSourceRowIndexs(this.grd_MachineView, array);
					}
					else
					{
						array = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
					}
					if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
					{
						if (array.Length == 1)
						{
							Program.IsRegistZKECardTong();
							AddDeviceForm addDeviceForm = new AddDeviceForm(int.Parse(this.m_datatable.Rows[array[0]][0].ToString()));
							addDeviceForm.Text = this.btn_edit.Text;
							addDeviceForm.btn_saveContinue.Enabled = false;
							addDeviceForm.btn_saveContinue.Visible = false;
							addDeviceForm.refreshDataEvent += this.frm_Event;
							addDeviceForm.ShowDialog();
							addDeviceForm.refreshDataEvent -= this.frm_Event;
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
					}
				}
				else if (!this.isDouble)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoData", "没有要处理的记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataBind(string dataType)
		{
			try
			{
				this.m_datatable.Rows.Clear();
				this.dicMachineId_Machine = new Dictionary<int, Machines>();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> list = null;
				list = ((!(dataType == "")) ? machinesBll.GetModelList(dataType) : machinesBll.GetModelList(""));
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(list[i]);
						if (this.imgNet_Off != null)
						{
							dataRow["OnlineState"] = this.imgNet_Off;
						}
						this.dicMachineId_Machine.Add(list[i].ID, list[i]);
						DataRow dataRow2 = dataRow;
						int num = list[i].ID;
						dataRow2[0] = num.ToString();
						dataRow[1] = list[i].MachineAlias;
						dataRow[2] = list[i].sn;
						if (list[i].ConnectType == 0)
						{
							dataRow[3] = "RS485";
							DataRow dataRow3 = dataRow;
							num = list[i].SerialPort;
							dataRow3[5] = "COM" + num.ToString();
							DataRow dataRow4 = dataRow;
							num = list[i].MachineNumber;
							dataRow4[6] = num.ToString();
						}
						else
						{
							dataRow[3] = "TCP/IP";
							dataRow[5] = "";
							dataRow[6] = "";
						}
						dataRow[4] = list[i].IP;
						if (this.m_areaTable.ContainsKey(list[i].area_id))
						{
							dataRow[7] = this.m_areaTable[list[i].area_id];
						}
						else
						{
							dataRow[7] = "";
						}
						if (list[i].Enabled && this.m_enableImg != null)
						{
							dataRow[8] = this.m_enableImg;
						}
						else if (!list[i].Enabled && this.m_disableImg != null)
						{
							dataRow[8] = this.m_disableImg;
						}
						dataRow[9] = list[i].device_name;
						dataRow[10] = list[i].FirmwareVersion;
						short num2 = list[i].usercount;
						if (int.Parse(num2.ToString()) <= 0)
						{
							dataRow[11] = "0";
						}
						else
						{
							DataRow dataRow5 = dataRow;
							num2 = list[i].usercount;
							dataRow5[11] = num2.ToString();
						}
						num2 = list[i].fingercount;
						if (int.Parse(num2.ToString()) <= 0)
						{
							dataRow[12] = "0";
						}
						else
						{
							DataRow dataRow6 = dataRow;
							num2 = list[i].fingercount;
							dataRow6[12] = num2.ToString();
						}
						num2 = list[i].fvcount;
						if (int.Parse(num2.ToString()) <= 0)
						{
							dataRow[13] = "0";
						}
						else
						{
							DataRow dataRow7 = dataRow;
							num2 = list[i].fvcount;
							dataRow7[13] = num2.ToString();
						}
						dataRow[14] = false;
						dataRow["SDKType"] = (int)list[i].DevSDKType;
						dataRow["FaceCount"] = list[i].FaceCount;
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				if (this.m_datatable.Rows.Count > 0)
				{
					this.BtnEnable(true);
					this.CheckPermission();
				}
				else
				{
					this.BtnEnable(false);
				}
				this.deviceCount = this.m_datatable.Rows.Count;
				this.grd_view.DataSource = this.m_datatable;
				this.column_check.ImageIndex = 1;
				this.CheckPermission();
				this.grd_MachineView.BestFitColumns();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void BtnEnable(bool isEnable)
		{
			this.btn_edit.Enabled = isEnable;
			this.btn_delete.Enabled = isEnable;
			this.btn_devToPC.Enabled = isEnable;
			this.btn_GetEvent.Enabled = isEnable;
			this.btn_SyncToPCEx.Enabled = isEnable;
			this.toolStripDropDownButton2.Enabled = isEnable;
			this.Menu_edit.Enabled = isEnable;
			this.Menu_delete.Enabled = isEnable;
			this.Menu_Export.Enabled = isEnable;
			this.Menu_GetEvent.Enabled = isEnable;
			this.Menu_SyncToPCEx.Enabled = isEnable;
			this.Menu_SyncToPC.Enabled = isEnable;
			this.btn_SyncToPC.Enabled = isEnable;
			this.btn_search.Enabled = isEnable;
			this.btn_GetUserInfo.Enabled = isEnable;
		}

		private byte[] getImageByte(string imagePath)
		{
			try
			{
				if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
				{
					FileStream fileStream = new FileStream(imagePath, FileMode.Open);
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
					fileStream.Close();
					return array;
				}
				return null;
			}
			catch
			{
				return null;
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				Program.IsRegistZKECardTong();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						for (int i = 0; i < checkedRows.Length; i++)
						{
							this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
							Machines model = machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model != null)
							{
								DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
								if (deviceServer != null)
								{
									DeviceServers.Instance.DelDeviceServer(model);
								}
								machinesBll.Delete(model);
								List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + model.ID);
								if (modelList != null && modelList.Count > 0)
								{
									for (int j = 0; j < modelList.Count; j++)
									{
										accDoorBll.Delete(modelList[j].id);
									}
								}
								this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
							}
						}
						this.m_wait.ShowProgress(100);
						if (AccCommon.IsECardTong == 1)
						{
							this.DataBind(" AccFun = 255 ");
						}
						else
						{
							this.DataBind("");
						}
						this.m_wait.HideEx(false);
					}
					this.Cursor = Cursors.Default;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的记录"));
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				this.Cursor = Cursors.Default;
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void Menu_SyncToPCEx_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				List<int> list = new List<int>();
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model?.Enabled ?? false)
							{
								list.Add(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							}
						}
					}
					if (list.Count > 0)
					{
						this.Cursor = Cursors.Default;
						PCToDeviceEx pCToDeviceEx = new PCToDeviceEx(list);
						pCToDeviceEx.ShowDialog();
						if (AccCommon.IsECardTong == 1)
						{
							this.DataBind(" AccFun = 255 ");
						}
						else
						{
							this.DataBind("");
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
						this.Cursor = Cursors.Default;
					}
				}
				else
				{
					this.Cursor = Cursors.Default;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_SyncToPC_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				List<int> list = new List<int>();
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model?.Enabled ?? false)
							{
								list.Add(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							}
						}
					}
					if (list.Count > 0)
					{
						this.Cursor = Cursors.Default;
						Program.IsRegistZKECardTong();
						PCToDevice pCToDevice = new PCToDevice(list);
						pCToDevice.ShowDialog();
						if (AccCommon.IsECardTong == 1)
						{
							this.DataBind(" AccFun = 255 ");
						}
						else
						{
							this.DataBind("");
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
						this.Cursor = Cursors.Default;
					}
				}
				else
				{
					this.Cursor = Cursors.Default;
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void enableDisable(bool type)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					this.m_wait.ShowEx();
					for (int i = 0; i < checkedRows.Length; i++)
					{
						this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
						Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
						if (model != null)
						{
							model.Enabled = type;
							this.m_machinesBll.Update(model);
							if (!type)
							{
								DeviceServers.Instance.DelDeviceServer(model);
							}
							if (type)
							{
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SEnableDevice", "启用设备") + " " + model.MachineAlias + " " + ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
								OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SEnableDevice", "启用设备") + " " + model.MachineAlias, 4, "device object");
							}
							else
							{
								this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SDisableDevice", "禁用设备") + " " + model.MachineAlias + " " + ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
								OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SDisableDevice", "禁用设备") + " " + model.MachineAlias, 4, "device object");
							}
						}
					}
					this.m_wait.ShowProgress(100);
					if (AccCommon.IsECardTong == 1)
					{
						this.DataBind(" AccFun = 255 ");
					}
					else
					{
						this.DataBind("");
					}
					this.m_wait.HideEx(false);
					this.Cursor = Cursors.Default;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_Enable_Click(object sender, EventArgs e)
		{
			this.enableDisable(true);
		}

		private void btn_Disable_Click(object sender, EventArgs e)
		{
			this.enableDisable(false);
		}

		private void btn_SynchronizeTime_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					this.m_wait.ShowEx();
					for (int i = 0; i < checkedRows.Length; i++)
					{
						this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model != null)
							{
								if (!model.Enabled)
								{
									this.m_wait.ShowInfos(model.MachineAlias + " " + ShowMsgInfos.GetInfo("SDevDisabled", "设备已被禁用") + "\r\n");
								}
								else
								{
									DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
									if (deviceServer != null)
									{
										DeviceParamBll deviceParamBll = new DeviceParamBll(deviceServer.Application);
										int num = deviceServer.Connect(3000);
										if (num >= 0)
										{
											num = ((deviceServer.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? deviceParamBll.SetDateTime(DateTime.Now) : deviceServer.STD_SetDeviceTime(null));
											if (num >= 0)
											{
												this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
												OperationLog.SaveOperationLog(model.MachineAlias + " " + ShowMsgInfos.GetInfo("SSyncDevTime", "同步时间"), 4, "device object");
											}
											else
											{
												this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SSyncDevTimeFailed", "同步时间失败") + ":" + PullSDkErrorInfos.GetInfo(num) + "\r\n");
											}
										}
										else
										{
											this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + ":" + PullSDkErrorInfos.GetInfo(num) + "\r\n");
										}
										deviceServer.Disconnect();
									}
								}
							}
						}
					}
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
					this.Cursor = Cursors.Default;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_ModifyIP_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length > 1)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
						this.Cursor = Cursors.Default;
						goto end_IL_0001;
					}
					Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString()));
					if (model != null)
					{
						if (!model.Enabled)
						{
							SysDialogs.ShowWarningMessage(model.MachineAlias + " " + ShowMsgInfos.GetInfo("SDevDisabled", "设备已被禁用"));
							this.Cursor = Cursors.Default;
							goto end_IL_0001;
						}
						if (model.ConnectType == 0)
						{
							SysDialogs.ShowWarningMessage(model.MachineAlias + " " + ShowMsgInfos.GetInfo("NotNetConnetType", "非TCP连接不能修改IP地址"));
							this.Cursor = Cursors.Default;
							goto end_IL_0001;
						}
						string commPassword = model.CommPassword;
						string iP = model.IP;
						ModifyIPForm modifyIPForm = new ModifyIPForm(model.ID, 1, iP, commPassword);
						modifyIPForm.refreshDataEvent += this.frm_Event;
						modifyIPForm.ShowDialog();
						modifyIPForm.refreshDataEvent -= this.frm_Event;
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
				this.Cursor = Cursors.Default;
				end_IL_0001:;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_UpgradeFirmware_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				List<int> list = new List<int>();
				Hashtable hashtable = new Hashtable();
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					List<int> list2 = new List<int>();
					List<int> list3 = new List<int>();
					List<int> list4 = new List<int>();
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model?.Enabled ?? false)
							{
								SDKType devSDKType = model.DevSDKType;
								if (devSDKType == SDKType.StandaloneSDK)
								{
									list3.Add(model.ID);
									if (model.ConnectType == 0)
									{
										list4.Add(model.ID);
									}
								}
								else
								{
									list2.Add(model.ID);
								}
								if (list3.Count > 0 && list2.Count > 0)
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SelectSDKTypeError", "不能同时选择两种背景颜色不同的记录"));
									return;
								}
								if (list4.Count > 0)
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UpgradeSTDOnComError", "脱机设备不能通过COM连接升级固件"));
									return;
								}
								list.Add(model.ID);
							}
						}
					}
					if (list.Count > 0)
					{
						UpgradeFirmvareForm upgradeFirmvareForm = new UpgradeFirmvareForm(list);
						upgradeFirmvareForm.refreshDataEvent += this.frm_Event;
						upgradeFirmvareForm.ShowDialog();
						upgradeFirmvareForm.refreshDataEvent -= this.frm_Event;
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoSelectDevice", "设备已被禁用"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_ModifyComPwd_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length > 1)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
					else
					{
						int num = int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString());
						Machines model = this.m_machinesBll.GetModel(num);
						if (model != null)
						{
							if (!model.Enabled)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
							}
							else
							{
								ModifyComPwdForm modifyComPwdForm = new ModifyComPwdForm(num);
								modifyComPwdForm.refreshDataEvent += this.frm_Event;
								modifyComPwdForm.ShowDialog();
								modifyComPwdForm.refreshDataEvent -= this.frm_Event;
							}
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_GetEvent_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				Hashtable hashtable = new Hashtable();
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model?.Enabled ?? false)
							{
								hashtable.Add(this.m_datatable.Rows[checkedRows[i]][0].ToString(), "1");
							}
						}
					}
					if (hashtable.Count > 0)
					{
						Program.IsRegistZKECardTong();
						DownLoadRecordForm downLoadRecordForm = new DownLoadRecordForm(hashtable);
						downLoadRecordForm.RefreshDataEvent += this.frm_Event;
						downLoadRecordForm.rbtn_new.Checked = true;
						downLoadRecordForm.rbtn_new.Visible = true;
						downLoadRecordForm.rbtn_all.Visible = true;
						downLoadRecordForm.rbtn_bySDCard.Visible = false;
						downLoadRecordForm.lbl_targetFile.Visible = false;
						downLoadRecordForm.txt_SDCardPath.Visible = false;
						downLoadRecordForm.btn_borwser.Visible = false;
						downLoadRecordForm.ShowDialog();
						downLoadRecordForm.RefreshDataEvent -= this.frm_Event;
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_GetUserInfo_Click(object sender, EventArgs e)
		{
			List<Machines> list = new List<Machines>();
			try
			{
				this.Cursor = Cursors.WaitCursor;
				DeviceParamBll deviceParamBll = new DeviceParamBll(MainForm._ia);
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					this.m_wait.ShowEx();
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < checkedRows.Length; i++)
					{
						this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (!model.Enabled)
							{
								this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SDevDisabled", "设备已被禁用") + "\r\n");
							}
							else
							{
								DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
								if (deviceServer != null)
								{
									if (deviceServer.IsConnected)
									{
										deviceServer.Disconnect();
									}
									int num = deviceServer.Connect(3000);
									if (num >= 0)
									{
										int num2 = 0;
										int num3 = 0;
										int num4 = 0;
										int num5 = 0;
										list.Add(model);
										Machines machines = model.Copy();
										num = DeviceHelper.UpdateDataCount(machines, this.m_wait.ShowInfos);
										if (num >= 0)
										{
											if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
											{
												num = deviceServer.STD_GetFirmwareVersion(out string firmwareVersion);
												machines.FirmwareVersion = firmwareVersion;
											}
											else
											{
												num = DeviceHelper.GetDeviceParams(machines);
											}
											model.FirmwareVersion = machines.FirmwareVersion;
											model.usercount = machines.usercount;
											model.fingercount = machines.fingercount;
											model.fvcount = machines.fvcount;
											model.FaceCount = machines.FaceCount;
											this.m_machinesBll.Update(model);
											this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
										}
										else
										{
											this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ": " + PullSDkErrorInfos.GetInfo(num));
										}
										if (deviceServer.IsConnected)
										{
											deviceServer.Disconnect();
										}
									}
									else
									{
										this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + "," + PullSDkErrorInfos.GetInfo(num) + "\r\n");
									}
								}
							}
						}
					}
					this.m_wait.ShowProgress(98);
					if (AccCommon.IsECardTong == 1)
					{
						this.DataBind(" AccFun = 255 ");
					}
					else
					{
						this.DataBind("");
					}
					this.SetOnline(list);
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
					this.Cursor = Cursors.Default;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void _Reboot()
		{
			List<Machines> online = new List<Machines>();
			try
			{
				this.Cursor = Cursors.WaitCursor;
				DeviceParamBll deviceParamBll = new DeviceParamBll(MainForm._ia);
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					this.m_wait.ShowEx();
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < checkedRows.Length; i++)
					{
						this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model.Enabled)
							{
								DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
								if (deviceServer != null)
								{
									if (deviceServer.IsConnected)
									{
										deviceServer.Disconnect();
									}
									int num = deviceServer.Connect(3000);
									if (num >= 0)
									{
										deviceServer.RebootDevice();
										this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
									}
								}
							}
						}
					}
					this.m_wait.ShowProgress(98);
					this.DataBind("");
					this.SetOnline(online);
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private AccTimeseg getTZ(int id)
		{
			AccTimeseg accTimeseg = new AccTimeseg();
			accTimeseg.id = id;
			accTimeseg.sunday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.sunday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.sunday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.sunday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.sunday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.sunday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.monday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.monday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.monday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.monday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.monday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.monday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.tuesday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.tuesday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.tuesday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.tuesday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.tuesday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.tuesday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.wednesday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.wednesday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.wednesday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.wednesday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.wednesday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.wednesday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.thursday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.thursday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.thursday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.thursday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.thursday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.thursday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.friday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.friday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.friday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.friday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.friday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.friday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.saturday_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.saturday_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.saturday_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.saturday_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.saturday_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.saturday_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype1_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype1_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype1_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype1_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype1_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype1_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype2_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype2_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype2_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype2_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype2_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype2_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype3_start1 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype3_start2 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype3_start3 = new DateTime(2000, 1, 1, 0, 0, 0);
			accTimeseg.holidaytype3_end1 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype3_end2 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.holidaytype3_end3 = new DateTime(2000, 1, 1, 23, 59, 59);
			accTimeseg.TimeZone1Id = 1;
			accTimeseg.TimeZone2Id = 0;
			accTimeseg.TimeZone3Id = 0;
			accTimeseg.TimeZoneHolidayId = 0;
			return accTimeseg;
		}

		private void _ResetFactoryDefaults(bool reset)
		{
			List<Machines> online = new List<Machines>();
			try
			{
				this.Cursor = Cursors.WaitCursor;
				DeviceParamBll deviceParamBll = new DeviceParamBll(MainForm._ia);
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					this.m_wait.ShowEx();
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < checkedRows.Length; i++)
					{
						this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model.Enabled)
							{
								DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
								if (deviceServer != null)
								{
									if (deviceServer.IsConnected)
									{
										deviceServer.Disconnect();
									}
									int num = deviceServer.Connect(3000);
									if (num >= 0)
									{
										if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
										{
											this.m_wait.ShowInfos("Resetando equipamento para configurações de fábrica...");
											deviceServer.STD_InitializeDeviceData(reset);
											this.m_wait.ShowInfos("Pronto! \r\n");
										}
										else
										{
											this.m_wait.ShowInfos("Resetando equipamento para configurações de fábrica...");
											num = DeviceHelper.DeletePullDeviceData(model, deviceServer, reset);
											this.m_wait.ShowInfos("Pronto! \r\n");
										}
										deviceServer.RebootDevice();
										this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
										AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
										List<AccDoor> list = null;
										list = accDoorBll.GetModelList("device_id=" + model.ID + " order by door_no");
										foreach (AccDoor item in list)
										{
											item.long_open_id = 0;
											accDoorBll.Update(item);
										}
									}
								}
							}
						}
					}
					this.m_wait.ShowProgress(98);
					this.DataBind("");
					this.SetOnline(online);
					this.m_wait.ShowProgress(100);
					this.m_wait.HideEx(false);
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_ChangeFPThreshold_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				int num = 0;
				string empty = string.Empty;
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					ChangeFPThresholdForm changeFPThresholdForm = new ChangeFPThresholdForm();
					changeFPThresholdForm.ShowDialog();
					num = changeFPThresholdForm.FPThreshold;
					if (num >= 35 && num <= 70)
					{
						this.m_wait.ShowEx();
						for (int i = 0; i < checkedRows.Length; i++)
						{
							this.m_wait.ShowProgress(i * 100 / checkedRows.Length);
							if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
							{
								Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									if (!model.Enabled)
									{
										this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SDevDisabled", "设备已被禁用") + "\r\n");
									}
									else if (model.IsOnlyRFMachine == 0)
									{
										DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
										if (deviceServer != null)
										{
											int num2 = deviceServer.Connect(3000);
											if (num2 >= 0)
											{
												empty = "MThreshold=" + num;
												if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
												{
													num2 = deviceServer.STD_SetDeviceInfo(DeviceInfoCode.Threshold12N, num - 20);
													num2 = deviceServer.STD_SetDeviceInfo(DeviceInfoCode.Threshold121, num - 20);
													num2 = deviceServer.STD_SetDeviceInfo(DeviceInfoCode.ThresholdRegister, num - 20);
												}
												else
												{
													num2 = deviceServer.SetDeviceParam(empty);
												}
												if (num2 >= 0)
												{
													model.fp_mthreshold = num;
													this.m_machinesBll.Update(model);
													this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "\r\n");
												}
												else
												{
													this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SChangeFPThresholdFailed", "修改指纹比对阀值失败") + "," + PullSDkErrorInfos.GetInfo(num2) + "\r\n");
												}
											}
											else
											{
												this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + "," + PullSDkErrorInfos.GetInfo(num2) + "\r\n");
											}
										}
									}
									else
									{
										this.m_wait.ShowInfos(model.MachineAlias + ":" + ShowMsgInfos.GetInfo("SDevNotSupportFP", "设备不支持指纹") + "\r\n");
									}
								}
							}
						}
						this.m_wait.ShowProgress(98);
						if (AccCommon.IsECardTong == 1)
						{
							this.DataBind(" AccFun = 255 ");
						}
						else
						{
							this.DataBind("");
						}
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
						this.Cursor = Cursors.Default;
					}
					this.Cursor = Cursors.Default;
					this.Cursor = Cursors.Default;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_CloseAuxiliary_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length > 1)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
					else
					{
						int num = int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString());
						Machines model = this.m_machinesBll.GetModel(num);
						if (!model.Enabled)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
						}
						else if (model.DevSDKType == SDKType.StandaloneSDK)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("OperationNotSuported", "该设备不支持此操作"));
						}
						else
						{
							CloseAuxiliaryForm closeAuxiliaryForm = new CloseAuxiliaryForm(num);
							closeAuxiliaryForm.ShowDialog();
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_modifyBaudrate_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length > 1)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
					else
					{
						int num = int.Parse(this.m_datatable.Rows[checkedRows[0]][0].ToString());
						Machines model = this.m_machinesBll.GetModel(num);
						if (!model.Enabled)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
						}
						else
						{
							ModifyBaudrateForm modifyBaudrateForm = new ModifyBaudrateForm(num);
							modifyBaudrateForm.refreshDataEvent += this.frm_Event;
							modifyBaudrateForm.ShowDialog();
							modifyBaudrateForm.refreshDataEvent -= this.frm_Event;
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_Export_Click(object sender, EventArgs e)
		{
			try
			{
				object[] obj = new object[8]
				{
					this.panelEx1.Text,
					"_",
					null,
					null,
					null,
					null,
					null,
					null
				};
				DateTime now = DateTime.Now;
				obj[2] = now.Year;
				now = DateTime.Now;
				int num = now.Month;
				obj[3] = num.ToString("00");
				now = DateTime.Now;
				num = now.Day;
				obj[4] = num.ToString("00");
				now = DateTime.Now;
				num = now.Hour;
				obj[5] = num.ToString("00");
				now = DateTime.Now;
				num = now.Minute;
				obj[6] = num.ToString("00");
				now = DateTime.Now;
				obj[7] = now.Second;
				string fileName = string.Concat(obj);
				DevExpressHelper.OutData(this.grd_MachineView, fileName);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_Log_Click(object sender, EventArgs e)
		{
			LogsInfoForm logsInfoForm = new LogsInfoForm("Machines");
			logsInfoForm.ShowDialog();
		}

		private void grd_view_DoubleClick(object sender, EventArgs e)
		{
			this.isDouble = true;
			this.btn_edit_Click(sender, e);
			this.isDouble = false;
		}

		private void grd_MachineView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void grd_MachineView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_MachineView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void btn_check_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				List<int> list = new List<int>();
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					for (int i = 0; i < checkedRows.Length; i++)
					{
						if (checkedRows[i] >= 0 && checkedRows[i] < this.m_datatable.Rows.Count)
						{
							Machines model = this.m_machinesBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							if (model?.Enabled ?? false)
							{
								list.Add(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
							}
						}
					}
					if (list.Count > 0)
					{
						Program.IsRegistZKECardTong();
						DeviceToPC deviceToPC = new DeviceToPC(list);
						deviceToPC.ShowDialog();
						if (AccCommon.IsECardTong == 1)
						{
							this.DataBind(" AccFun = 255 ");
						}
						else
						{
							this.DataBind("");
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void Menu_edit_Click(object sender, EventArgs e)
		{
			this.btn_edit_Click(sender, e);
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
			try
			{
				DeviceSearchForm deviceSearchForm = new DeviceSearchForm();
				deviceSearchForm.Text = this.btn_search.Text;
				deviceSearchForm.ShowDialog();
				if (deviceSearchForm.DialogResult == DialogResult.OK)
				{
					this.strCondtion = deviceSearchForm.ConditionStr;
					if (AccCommon.IsECardTong == 1)
					{
						this.DataBind(this.strCondtion + " and AccFun = 255 ");
					}
					else
					{
						this.DataBind(this.strCondtion);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_SdRecord_Click(object sender, EventArgs e)
		{
			Hashtable machineSelectedInfo = new Hashtable();
			DownLoadRecordForm downLoadRecordForm = new DownLoadRecordForm(machineSelectedInfo);
			downLoadRecordForm.RefreshDataEvent += this.frm_Event;
			downLoadRecordForm.rbtn_bySDCard.Checked = true;
			downLoadRecordForm.rbtn_bySDCard_Click(null, null);
			downLoadRecordForm.ShowDialog();
			downLoadRecordForm.RefreshDataEvent -= this.frm_Event;
		}

		private void btn_RS485_MasterSlave_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_MachineView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					if (checkedRows.Length < 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
					else
					{
						List<Machines> list = new List<Machines>();
						for (int i = 0; i < checkedRows.Length; i++)
						{
							int iD = int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString());
							Machines model = this.m_machinesBll.GetModel(iD);
							if (model.Enabled)
							{
								list.Add(model);
							}
						}
						OptionsForm optionsForm = new OptionsForm(0, list);
						optionsForm.refreshDataEvent += this.frm_Event;
						optionsForm.ShowDialog();
						optionsForm.refreshDataEvent -= this.frm_Event;
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void CheckAllOnlineState()
		{
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < this.m_datatable.Rows.Count; i++)
			{
				if (this.m_datatable.Rows[i][3].ToString().ToUpper() == "RS485")
				{
					list.Add(this.m_datatable.Rows[i]);
				}
				else
				{
					Thread thread = new Thread(this.CheckTcpDevState);
					this.lstThread.Add(thread);
					thread.Start(this.m_datatable.Rows[i]);
				}
			}
			Thread thread2 = new Thread(this.CheckCOMDevState);
			this.lstThread.Add(thread2);
			thread2.Start(list);
		}

		private void CheckTcpDevState(object objDataRow)
		{
			int key = 0;
			int num = -1002;
			DataRow dataRow = null;
			int num4;
			int num3;
			int num2;
			int num5 = num4 = (num3 = (num2 = 0));
			try
			{
				dataRow = (DataRow)objDataRow;
				int.TryParse(dataRow["ID"].ToString(), out key);
				num5 = (num4 = (num3 = 0));
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				if (this.dicMachineId_Machine.ContainsKey(key))
				{
					Machines machines = this.dicMachineId_Machine[key];
					DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machines);
					if (deviceServer != null)
					{
						lock (deviceServer)
						{
							if (deviceServer.IsConnected)
							{
								deviceServer.Disconnect();
								if (machines.ConnectType == 0)
								{
									Program.KillPlrscagent();
								}
							}
							num = deviceServer.Connect(3000);
							if (num >= 0)
							{
								if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
								{
									num = deviceServer.STD_GetRecordCount(MachineDataStatusCode.RegistedUserCount, out num5);
									num = deviceServer.STD_GetRecordCount(MachineDataStatusCode.TemplateCount, out num4);
									if (machines.FaceFunOn == 1)
									{
										deviceServer.STD_GetRecordCount(MachineDataStatusCode.FaceTemplateCount, out num2);
									}
								}
								else
								{
									num5 = deviceServer.GetDeviceDataCount("user", "", "");
									if (machines.device_type != 102)
									{
										if (machines.IsOnlyRFMachine == 0)
										{
											num4 = deviceServer.GetDeviceDataCount("templatev10", "", "");
										}
									}
									else
									{
										num3 = deviceServer.GetDeviceDataCount("fvtemplate", "", "");
									}
									if (machines.device_type == 103)
									{
										num2 = deviceServer.GetDeviceDataCount("ssrface7", "", "");
									}
								}
								num5 = ((num5 >= 0) ? num5 : 0);
								num4 = ((num4 >= 0) ? num4 : 0);
								num3 = ((num3 >= 0) ? num3 : 0);
								num2 = ((num2 >= 0) ? num2 : 0);
								num = deviceServer.Disconnect();
								machines.usercount = (short)num5;
								machines.fingercount = (short)num4;
								machines.fvcount = (short)num3;
								machines.FaceCount = num2;
								machinesBll.Update(machines);
								this.SetColunmValue(dataRow, "OnlineState", true.ToString());
								this.SetColunmValue(dataRow, "usercount", num5.ToString());
								this.SetColunmValue(dataRow, "fingercount", num4.ToString());
								this.SetColunmValue(dataRow, "fvcount", num3.ToString());
								this.SetColunmValue(dataRow, "FaceCount", num2.ToString());
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysLogServer.WriteLog("Exception on CheckTcpDevState\t" + ex.Message);
			}
		}

		private void CheckCOMDevState(object objListDataRow)
		{
			int num3;
			int num2;
			int num;
			int num4 = num3 = (num2 = (num = 0));
			List<DataRow> list = objListDataRow as List<DataRow>;
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						if (!base.Disposing && !base.IsDisposed)
						{
							DataRow dataRow = list[i];
							int.TryParse(dataRow[0].ToString(), out int key);
							if (this.dicMachineId_Machine.ContainsKey(key))
							{
								Machines machines = this.dicMachineId_Machine[key];
								MachinesBll machinesBll = new MachinesBll(MainForm._ia);
								DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(machines);
								if (deviceServer != null)
								{
									lock (deviceServer)
									{
										if (deviceServer.IsConnected)
										{
											deviceServer.Disconnect();
											if (machines.ConnectType == 0)
											{
												Program.KillPlrscagent();
											}
										}
										int num5 = deviceServer.Connect(3000);
										if (num5 >= 0)
										{
											if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
											{
												num5 = deviceServer.STD_GetRecordCount(MachineDataStatusCode.RegistedUserCount, out num4);
												num5 = deviceServer.STD_GetRecordCount(MachineDataStatusCode.TemplateCount, out num3);
												if (machines.FaceFunOn == 1)
												{
													num5 = deviceServer.STD_GetRecordCount(MachineDataStatusCode.FaceTemplateCount, out num);
												}
											}
											else
											{
												num4 = deviceServer.GetDeviceDataCount("user", "", "");
												if (machines.device_type != 102)
												{
													if (machines.IsOnlyRFMachine == 0)
													{
														num3 = deviceServer.GetDeviceDataCount("templatev10", "", "");
													}
												}
												else
												{
													num2 = deviceServer.GetDeviceDataCount("fvtemplate", "", "");
												}
												if (machines.device_type == 103)
												{
													num = deviceServer.GetDeviceDataCount("ssrface7", "", "");
												}
											}
											num5 = deviceServer.Disconnect();
											num4 = ((num4 >= 0) ? num4 : 0);
											num3 = ((num3 >= 0) ? num3 : 0);
											num2 = ((num2 >= 0) ? num2 : 0);
											num = ((num >= 0) ? num : 0);
											machines.usercount = (short)num4;
											machines.fingercount = (short)num3;
											machines.fvcount = (short)num2;
											machines.FaceCount = num;
											machinesBll.Update(machines);
											this.SetColunmValue(dataRow, "OnlineState", true.ToString());
											this.SetColunmValue(dataRow, "usercount", num4.ToString());
											this.SetColunmValue(dataRow, "fingercount", num3.ToString());
											this.SetColunmValue(dataRow, "fvcount", num2.ToString());
											this.SetColunmValue(dataRow, "FaceCount", num.ToString());
										}
									}
								}
							}
							goto end_IL_003c;
						}
						return;
						end_IL_003c:;
					}
					catch (Exception ex)
					{
						SysLogServer.WriteLog("Exception on CheckComDevState\t" + ex.Message);
					}
				}
			}
		}

		private void SetColunmValue(DataRow dr, string ColName, string ColValue)
		{
			if (!base.Disposing && !base.IsDisposed && this.m_datatable.Rows.Count > 0)
			{
				try
				{
					if (base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.SetColunmValue(dr, ColName, ColValue);
						});
					}
					else
					{
						lock (this.m_datatable)
						{
							if (ColName == "OnlineState")
							{
								dr[ColName] = (bool.Parse(ColValue) ? this.imgNet_On : this.imgNet_Off);
							}
							else
							{
								dr[ColName] = ColValue;
							}
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private void DeviceUserControl_Disposed(object sender, EventArgs e)
		{
			for (int i = 0; i < this.lstThread.Count; i++)
			{
				try
				{
					this.lstThread[i].Abort();
				}
				catch (Exception ex)
				{
					SysLogServer.WriteLog("Exception on Abort CheckOnlineStateThread: " + ex.Message);
				}
			}
		}

		private void SetOnline(List<Machines> lstMachine)
		{
			if (!base.Disposing && !base.IsDisposed && this.m_datatable.Rows.Count > 0)
			{
				try
				{
					if (base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.SetOnline(lstMachine);
						});
					}
					else
					{
						lock (this.m_datatable)
						{
							Dictionary<int, Machines> dictionary = new Dictionary<int, Machines>();
							for (int i = 0; i < lstMachine.Count; i++)
							{
								if (!dictionary.ContainsKey(lstMachine[i].ID))
								{
									dictionary.Add(lstMachine[i].ID, lstMachine[i]);
								}
							}
							for (int j = 0; j < this.m_datatable.Rows.Count; j++)
							{
								if (int.TryParse(this.m_datatable.Rows[j][0].ToString(), out int key) && dictionary.ContainsKey(key))
								{
									this.m_datatable.Rows[j]["OnlineState"] = this.imgNet_On;
								}
							}
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private void btn_GetDataFromUdisk_Click(object sender, EventArgs e)
		{
			UDiskToPc uDiskToPc = new UDiskToPc();
			uDiskToPc.ShowDialog();
			uDiskToPc.Dispose();
			uDiskToPc = null;
		}

		private void btn_Export2Udisk_Click(object sender, EventArgs e)
		{
			DataRow[] array = this.m_datatable.Select("check=true and SDKType<>2");
			DataRow[] array2 = this.m_datatable.Select("check=true and SDKType=2");
			if (array != null && array.Length != 0 && array2 != null && array2.Length != 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SelectSDKTypeError", "不能同时选择两种背景颜色不同的记录"));
			}
			else if (array2 != null && array2.Length > 1)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SelectRowCountError", "不能选择多个黄色背景的记录"));
			}
			else
			{
				SDKType sDKType = (array != null && array.Length != 0) ? SDKType.PullSDK : SDKType.StandaloneSDK;
				if ((array != null && array.Length != 0) || (array2 != null && array2.Length != 0))
				{
					List<int> list = new List<int>();
					if (sDKType == SDKType.StandaloneSDK)
					{
						for (int i = 0; i < array2.Length; i++)
						{
							list.Add(int.Parse(array2[i]["ID"].ToString()));
						}
					}
					else
					{
						for (int j = 0; j < array.Length; j++)
						{
							list.Add(int.Parse(array[j]["ID"].ToString()));
						}
					}
					PCToUDisk pCToUDisk = new PCToUDisk(list, sDKType);
					pCToUDisk.ShowDialog();
					pCToUDisk.Dispose();
					pCToUDisk = null;
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectlog", "请选择记录"));
				}
			}
		}

		private void grd_MachineView_RowStyle(object sender, RowStyleEventArgs e)
		{
			SDKType sDKType = SDKType.Undefined;
			GridView gridView = sender as GridView;
			if (e.RowHandle >= 0)
			{
				DataRow dataRow = gridView.GetDataRow(e.RowHandle);
				try
				{
					sDKType = (SDKType)(int)dataRow["SDKType"];
				}
				catch
				{
				}
				if (sDKType == SDKType.StandaloneSDK)
				{
					e.Appearance.BackColor = Color.Yellow;
					e.Appearance.BackColor2 = Color.Yellow;
				}
			}
		}

		private void rebootToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this._Reboot();
		}

		private void resetDeFábricaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FrmYesNo frmYesNo = new FrmYesNo("Deseja reconfigurar o endereço IP do equipamento?\nEssa operação poderá fazer com que o sistema\nperca a comunicação com o equipamento.\nCertifique-se de reconfigurar o sistema e\no equipamento para que seja possível a comunicação.");
			frmYesNo.ShowDialog();
			bool reset = frmYesNo.isAccepted();
			this._ResetFactoryDefaults(reset);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DeviceUserControl));
			this.panelEx1 = new PanelEx();
			this.contextMenuStrip2 = new ContextMenuStrip(this.components);
			this.Menu_add = new ToolStripMenuItem();
			this.Menu_edit = new ToolStripMenuItem();
			this.Menu_delete = new ToolStripMenuItem();
			this.Menu_Export = new ToolStripMenuItem();
			this.Menu_GetEvent = new ToolStripMenuItem();
			this.Menu_SyncToPCEx = new ToolStripMenuItem();
			this.Menu_SyncToPC = new ToolStripMenuItem();
			this.grd_view = new GridControl();
			this.grd_MachineView = new GridView();
			this.column_check = new GridColumn();
			this.column_MachineAlias = new GridColumn();
			this.column_sn = new GridColumn();
			this.column_ConnectType = new GridColumn();
			this.column_IP = new GridColumn();
			this.column_SerialPort = new GridColumn();
			this.column_MachineNumber = new GridColumn();
			this.column_area_id = new GridColumn();
			this.column_Enabled = new GridColumn();
			this.repositoryItemPictureEdit1 = new RepositoryItemPictureEdit();
			this.column_FirmwareVersion = new GridColumn();
			this.colOnlineState = new GridColumn();
			this.column_usercount = new GridColumn();
			this.column_fingercount = new GridColumn();
			this.column_fvcount = new GridColumn();
			this.colFaceCount = new GridColumn();
			this.column_acpanel_type = new GridColumn();
			this.MenuPanelEx = new ToolStrip();
			this.btn_add = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_delete = new ToolStripButton();
			this.btn_search = new ToolStripButton();
			this.btn_GetEvent = new ToolStripButton();
			this.btn_SyncToPCEx = new ToolStripButton();
			this.btn_devToPC = new ToolStripButton();
			this.btn_GetUserInfo = new ToolStripButton();
			this.toolStripDropDownButton2 = new ToolStripDropDownButton();
			this.btn_Disable = new ToolStripMenuItem();
			this.btn_Enable = new ToolStripMenuItem();
			this.btn_UpgradeFirmware = new ToolStripMenuItem();
			this.btn_SynchronizeTime = new ToolStripMenuItem();
			this.btn_ModifyIP = new ToolStripMenuItem();
			this.btn_CloseAuxiliary = new ToolStripMenuItem();
			this.btn_ModifyComPwd = new ToolStripMenuItem();
			this.btn_GetUserInfo1 = new ToolStripMenuItem();
			this.btn_ChangeFPThreshold = new ToolStripMenuItem();
			this.btn_disableTime = new ToolStripMenuItem();
			this.btn_enableTime = new ToolStripMenuItem();
			this.btn_modifyBaudrate = new ToolStripMenuItem();
			this.btn_check = new ToolStripMenuItem();
			this.btn_Log = new ToolStripMenuItem();
			this.btn_SyncToPC = new ToolStripMenuItem();
			this.btn_Export = new ToolStripMenuItem();
			this.btn_Sd_Record = new ToolStripMenuItem();
			this.btn_RS485_MasterSlave = new ToolStripMenuItem();
			this.btn_GetDataFromUdisk = new ToolStripMenuItem();
			this.btn_Export2Udisk = new ToolStripMenuItem();
			this.rebootToolStripMenuItem = new ToolStripMenuItem();
			this.resetDeFábricaToolStripMenuItem = new ToolStripMenuItem();
			this.contextMenuStrip2.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_MachineView).BeginInit();
			((ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
			this.MenuPanelEx.SuspendLayout();
			base.SuspendLayout();
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Dock = DockStyle.Top;
			this.panelEx1.Location = new Point(0, 41);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(833, 25);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 2;
			this.panelEx1.Text = "设备";
			this.contextMenuStrip2.Items.AddRange(new ToolStripItem[7]
			{
				this.Menu_add,
				this.Menu_edit,
				this.Menu_delete,
				this.Menu_Export,
				this.Menu_GetEvent,
				this.Menu_SyncToPCEx,
				this.Menu_SyncToPC
			});
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new Size(183, 158);
			this.Menu_add.Image = Resources.add;
			this.Menu_add.Name = "Menu_add";
			this.Menu_add.Size = new Size(182, 22);
			this.Menu_add.Text = "新增";
			this.Menu_add.Click += this.btn_add_Click;
			this.Menu_edit.Image = Resources.edit;
			this.Menu_edit.Name = "Menu_edit";
			this.Menu_edit.Size = new Size(182, 22);
			this.Menu_edit.Text = "编辑";
			this.Menu_edit.Click += this.Menu_edit_Click;
			this.Menu_delete.Image = Resources.delete;
			this.Menu_delete.Name = "Menu_delete";
			this.Menu_delete.Size = new Size(182, 22);
			this.Menu_delete.Text = "删除";
			this.Menu_delete.Click += this.btn_delete_Click;
			this.Menu_Export.Image = Resources.Export;
			this.Menu_Export.Name = "Menu_Export";
			this.Menu_Export.Size = new Size(182, 22);
			this.Menu_Export.Text = "导出";
			this.Menu_Export.Click += this.btn_Export_Click;
			this.Menu_GetEvent.Image = Resources.Get_event_entries;
			this.Menu_GetEvent.Name = "Menu_GetEvent";
			this.Menu_GetEvent.Size = new Size(182, 22);
			this.Menu_GetEvent.Text = "获取事件记录";
			this.Menu_GetEvent.Click += this.btn_GetEvent_Click;
			this.Menu_SyncToPCEx.Image = Resources.synchronize_the_data;
			this.Menu_SyncToPCEx.Name = "Menu_SyncToPCEx";
			this.Menu_SyncToPCEx.Size = new Size(182, 22);
			this.Menu_SyncToPCEx.Text = "同步变动数据到设备";
			this.Menu_SyncToPCEx.Click += this.Menu_SyncToPCEx_Click;
			this.Menu_SyncToPC.Image = Resources.synchronize_the_data;
			this.Menu_SyncToPC.Name = "Menu_SyncToPC";
			this.Menu_SyncToPC.Size = new Size(182, 22);
			this.Menu_SyncToPC.Text = "同步所有数据到设备";
			this.Menu_SyncToPC.Click += this.btn_SyncToPC_Click;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 66);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_MachineView;
			this.grd_view.Name = "grd_view";
			this.grd_view.RepositoryItems.AddRange(new RepositoryItem[1]
			{
				this.repositoryItemPictureEdit1
			});
			this.grd_view.Size = new Size(833, 420);
			this.grd_view.TabIndex = 15;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_MachineView
			});
			this.grd_view.DoubleClick += this.grd_view_DoubleClick;
			this.grd_MachineView.Columns.AddRange(new GridColumn[16]
			{
				this.column_check,
				this.column_MachineAlias,
				this.column_sn,
				this.column_ConnectType,
				this.column_IP,
				this.column_SerialPort,
				this.column_MachineNumber,
				this.column_area_id,
				this.column_Enabled,
				this.column_FirmwareVersion,
				this.colOnlineState,
				this.column_usercount,
				this.column_fingercount,
				this.column_fvcount,
				this.colFaceCount,
				this.column_acpanel_type
			});
			this.grd_MachineView.GridControl = this.grd_view;
			this.grd_MachineView.Name = "grd_MachineView";
			this.grd_MachineView.OptionsBehavior.Editable = false;
			this.grd_MachineView.OptionsView.ShowGroupPanel = false;
			this.grd_MachineView.CustomDrawColumnHeader += this.grd_MachineView_CustomDrawColumnHeader;
			this.grd_MachineView.CustomDrawCell += this.grd_MachineView_CustomDrawCell;
			this.grd_MachineView.RowStyle += this.grd_MachineView_RowStyle;
			this.grd_MachineView.Click += this.grd_MachineView_Click;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 25;
			this.column_MachineAlias.Caption = "设备名称";
			this.column_MachineAlias.Name = "column_MachineAlias";
			this.column_MachineAlias.Visible = true;
			this.column_MachineAlias.VisibleIndex = 1;
			this.column_MachineAlias.Width = 71;
			this.column_sn.Caption = "序列号";
			this.column_sn.Name = "column_sn";
			this.column_sn.Visible = true;
			this.column_sn.VisibleIndex = 2;
			this.column_sn.Width = 50;
			this.column_ConnectType.Caption = "通信方式";
			this.column_ConnectType.Name = "column_ConnectType";
			this.column_ConnectType.Visible = true;
			this.column_ConnectType.VisibleIndex = 3;
			this.column_ConnectType.Width = 70;
			this.column_IP.Caption = "IP地址";
			this.column_IP.Name = "column_IP";
			this.column_IP.Visible = true;
			this.column_IP.VisibleIndex = 4;
			this.column_IP.Width = 61;
			this.column_SerialPort.Caption = "串口号";
			this.column_SerialPort.Name = "column_SerialPort";
			this.column_SerialPort.Visible = true;
			this.column_SerialPort.VisibleIndex = 5;
			this.column_SerialPort.Width = 69;
			this.column_MachineNumber.Caption = "RS485地址";
			this.column_MachineNumber.Name = "column_MachineNumber";
			this.column_MachineNumber.Visible = true;
			this.column_MachineNumber.VisibleIndex = 6;
			this.column_MachineNumber.Width = 71;
			this.column_area_id.Caption = "区域名称";
			this.column_area_id.Name = "column_area_id";
			this.column_area_id.Visible = true;
			this.column_area_id.VisibleIndex = 15;
			this.column_area_id.Width = 73;
			this.column_Enabled.Caption = "启用";
			this.column_Enabled.ColumnEdit = this.repositoryItemPictureEdit1;
			this.column_Enabled.ImageAlignment = StringAlignment.Center;
			this.column_Enabled.Name = "column_Enabled";
			this.column_Enabled.Visible = true;
			this.column_Enabled.VisibleIndex = 7;
			this.column_Enabled.Width = 27;
			this.repositoryItemPictureEdit1.CustomHeight = 40;
			this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
			this.repositoryItemPictureEdit1.SizeMode = PictureSizeMode.Squeeze;
			this.column_FirmwareVersion.Caption = "固件版本";
			this.column_FirmwareVersion.Name = "column_FirmwareVersion";
			this.column_FirmwareVersion.Visible = true;
			this.column_FirmwareVersion.VisibleIndex = 13;
			this.column_FirmwareVersion.Width = 56;
			this.colOnlineState.Caption = "在线状态";
			this.colOnlineState.ColumnEdit = this.repositoryItemPictureEdit1;
			this.colOnlineState.Name = "colOnlineState";
			this.colOnlineState.Visible = true;
			this.colOnlineState.VisibleIndex = 14;
			this.column_usercount.Caption = "用户数";
			this.column_usercount.Name = "column_usercount";
			this.column_usercount.Visible = true;
			this.column_usercount.VisibleIndex = 8;
			this.column_usercount.Width = 57;
			this.column_fingercount.Caption = "指纹数";
			this.column_fingercount.Name = "column_fingercount";
			this.column_fingercount.Visible = true;
			this.column_fingercount.VisibleIndex = 9;
			this.column_fingercount.Width = 62;
			this.column_fvcount.Caption = "指静脉数";
			this.column_fvcount.Name = "column_fvcount";
			this.column_fvcount.Visible = true;
			this.column_fvcount.VisibleIndex = 10;
			this.column_fvcount.Width = 62;
			this.colFaceCount.Caption = "人脸数";
			this.colFaceCount.Name = "colFaceCount";
			this.colFaceCount.Visible = true;
			this.colFaceCount.VisibleIndex = 11;
			this.column_acpanel_type.Caption = "设备型号";
			this.column_acpanel_type.Name = "column_acpanel_type";
			this.column_acpanel_type.Visible = true;
			this.column_acpanel_type.VisibleIndex = 12;
			this.column_acpanel_type.Width = 61;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[9]
			{
				this.btn_add,
				this.btn_edit,
				this.btn_delete,
				this.btn_search,
				this.btn_GetEvent,
				this.btn_SyncToPCEx,
				this.btn_devToPC,
				this.btn_GetUserInfo,
				this.toolStripDropDownButton2
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(833, 41);
			this.MenuPanelEx.TabIndex = 16;
			this.MenuPanelEx.Text = "toolStrip1";
			this.btn_add.Image = (Image)componentResourceManager.GetObject("btn_add.Image");
			this.btn_add.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_add.ImageTransparentColor = Color.Magenta;
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(67, 38);
			this.btn_add.Text = "新增";
			this.btn_add.TextAlign = ContentAlignment.MiddleRight;
			this.btn_add.Click += this.btn_add_Click;
			this.btn_edit.Image = (Image)componentResourceManager.GetObject("btn_edit.Image");
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(67, 38);
			this.btn_edit.Text = "编辑";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_delete.Image = (Image)componentResourceManager.GetObject("btn_delete.Image");
			this.btn_delete.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_delete.ImageTransparentColor = Color.Magenta;
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(67, 38);
			this.btn_delete.Text = "删除";
			this.btn_delete.Click += this.btn_delete_Click;
			this.btn_search.Image = (Image)componentResourceManager.GetObject("btn_search.Image");
			this.btn_search.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_search.ImageTransparentColor = Color.Magenta;
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(67, 38);
			this.btn_search.Text = "查找";
			this.btn_search.Click += this.btn_search_Click;
			this.btn_GetEvent.Image = (Image)componentResourceManager.GetObject("btn_GetEvent.Image");
			this.btn_GetEvent.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_GetEvent.ImageTransparentColor = Color.Magenta;
			this.btn_GetEvent.Name = "btn_GetEvent";
			this.btn_GetEvent.Size = new Size(133, 38);
			this.btn_GetEvent.Text = "Importar Eventos";
			this.btn_GetEvent.Click += this.btn_GetEvent_Click;
			this.btn_SyncToPCEx.Image = (Image)componentResourceManager.GetObject("btn_SyncToPCEx.Image");
			this.btn_SyncToPCEx.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_SyncToPCEx.ImageTransparentColor = Color.Magenta;
			this.btn_SyncToPCEx.Name = "btn_SyncToPCEx";
			this.btn_SyncToPCEx.Size = new Size(151, 38);
			this.btn_SyncToPCEx.Text = "同步所有数据到设备";
			this.btn_SyncToPCEx.Click += this.btn_SyncToPC_Click;
			this.btn_devToPC.Image = Resources.devToPC;
			this.btn_devToPC.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_devToPC.ImageTransparentColor = Color.Magenta;
			this.btn_devToPC.Name = "btn_devToPC";
			this.btn_devToPC.Size = new Size(127, 38);
			this.btn_devToPC.Text = "从设备获取数据";
			this.btn_devToPC.Click += this.btn_check_Click;
			this.btn_GetUserInfo.Image = (Image)componentResourceManager.GetObject("btn_GetUserInfo.Image");
			this.btn_GetUserInfo.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_GetUserInfo.ImageTransparentColor = Color.Magenta;
			this.btn_GetUserInfo.Name = "btn_GetUserInfo";
			this.btn_GetUserInfo.Size = new Size(115, 38);
			this.btn_GetUserInfo.Text = "获取人员信息";
			this.btn_GetUserInfo.Click += this.btn_GetUserInfo_Click;
			this.toolStripDropDownButton2.DropDownItems.AddRange(new ToolStripItem[22]
			{
				this.btn_Disable,
				this.btn_Enable,
				this.btn_UpgradeFirmware,
				this.btn_SynchronizeTime,
				this.btn_ModifyIP,
				this.btn_CloseAuxiliary,
				this.btn_ModifyComPwd,
				this.btn_GetUserInfo1,
				this.btn_ChangeFPThreshold,
				this.btn_disableTime,
				this.btn_enableTime,
				this.btn_modifyBaudrate,
				this.btn_check,
				this.btn_Log,
				this.btn_SyncToPC,
				this.btn_Export,
				this.btn_Sd_Record,
				this.btn_RS485_MasterSlave,
				this.btn_GetDataFromUdisk,
				this.btn_Export2Udisk,
				this.rebootToolStripMenuItem,
				this.resetDeFábricaToolStripMenuItem
			});
			this.toolStripDropDownButton2.Image = (Image)componentResourceManager.GetObject("toolStripDropDownButton2.Image");
			this.toolStripDropDownButton2.ImageTransparentColor = Color.Magenta;
			this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
			this.toolStripDropDownButton2.Size = new Size(84, 20);
			this.toolStripDropDownButton2.Text = "更多操作";
			this.btn_Disable.Image = (Image)componentResourceManager.GetObject("btn_Disable.Image");
			this.btn_Disable.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_Disable.Name = "btn_Disable";
			this.btn_Disable.Size = new Size(182, 22);
			this.btn_Disable.Text = "禁用";
			this.btn_Disable.Click += this.btn_Disable_Click;
			this.btn_Enable.Image = Resources.enabled;
			this.btn_Enable.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_Enable.Name = "btn_Enable";
			this.btn_Enable.Size = new Size(182, 22);
			this.btn_Enable.Text = "启用";
			this.btn_Enable.Click += this.btn_Enable_Click;
			this.btn_UpgradeFirmware.Image = (Image)componentResourceManager.GetObject("btn_UpgradeFirmware.Image");
			this.btn_UpgradeFirmware.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_UpgradeFirmware.Name = "btn_UpgradeFirmware";
			this.btn_UpgradeFirmware.Size = new Size(182, 22);
			this.btn_UpgradeFirmware.Text = "Atualizar Firmware";
			this.btn_UpgradeFirmware.Click += this.btn_UpgradeFirmware_Click;
			this.btn_SynchronizeTime.Image = (Image)componentResourceManager.GetObject("btn_SynchronizeTime.Image");
			this.btn_SynchronizeTime.Name = "btn_SynchronizeTime";
			this.btn_SynchronizeTime.Size = new Size(182, 22);
			this.btn_SynchronizeTime.Text = "同步时间";
			this.btn_SynchronizeTime.Click += this.btn_SynchronizeTime_Click;
			this.btn_ModifyIP.Image = (Image)componentResourceManager.GetObject("btn_ModifyIP.Image");
			this.btn_ModifyIP.Name = "btn_ModifyIP";
			this.btn_ModifyIP.Size = new Size(182, 22);
			this.btn_ModifyIP.Text = "修改IP地址";
			this.btn_ModifyIP.Click += this.btn_ModifyIP_Click;
			this.btn_CloseAuxiliary.Image = (Image)componentResourceManager.GetObject("btn_CloseAuxiliary.Image");
			this.btn_CloseAuxiliary.Name = "btn_CloseAuxiliary";
			this.btn_CloseAuxiliary.Size = new Size(182, 22);
			this.btn_CloseAuxiliary.Text = "关闭辅助输出";
			this.btn_CloseAuxiliary.Click += this.btn_CloseAuxiliary_Click;
			this.btn_ModifyComPwd.Image = (Image)componentResourceManager.GetObject("btn_ModifyComPwd.Image");
			this.btn_ModifyComPwd.Name = "btn_ModifyComPwd";
			this.btn_ModifyComPwd.Size = new Size(182, 22);
			this.btn_ModifyComPwd.Text = "修改通信密码";
			this.btn_ModifyComPwd.Click += this.btn_ModifyComPwd_Click;
			this.btn_GetUserInfo1.Image = (Image)componentResourceManager.GetObject("btn_GetUserInfo1.Image");
			this.btn_GetUserInfo1.Name = "btn_GetUserInfo1";
			this.btn_GetUserInfo1.Size = new Size(182, 22);
			this.btn_GetUserInfo1.Text = "获取人员信息";
			this.btn_GetUserInfo1.Visible = false;
			this.btn_GetUserInfo1.Click += this.btn_GetUserInfo_Click;
			this.btn_ChangeFPThreshold.Image = (Image)componentResourceManager.GetObject("btn_ChangeFPThreshold.Image");
			this.btn_ChangeFPThreshold.Name = "btn_ChangeFPThreshold";
			this.btn_ChangeFPThreshold.Size = new Size(182, 22);
			this.btn_ChangeFPThreshold.Text = "修改指纹比对阈值";
			this.btn_ChangeFPThreshold.Click += this.btn_ChangeFPThreshold_Click;
			this.btn_disableTime.Image = (Image)componentResourceManager.GetObject("btn_disableTime.Image");
			this.btn_disableTime.Name = "btn_disableTime";
			this.btn_disableTime.Size = new Size(182, 22);
			this.btn_disableTime.Text = "禁用夏令时";
			this.btn_disableTime.Visible = false;
			this.btn_enableTime.Image = (Image)componentResourceManager.GetObject("btn_enableTime.Image");
			this.btn_enableTime.Name = "btn_enableTime";
			this.btn_enableTime.Size = new Size(182, 22);
			this.btn_enableTime.Text = "启用夏令时";
			this.btn_enableTime.Visible = false;
			this.btn_modifyBaudrate.Image = (Image)componentResourceManager.GetObject("btn_modifyBaudrate.Image");
			this.btn_modifyBaudrate.Name = "btn_modifyBaudrate";
			this.btn_modifyBaudrate.Size = new Size(182, 22);
			this.btn_modifyBaudrate.Text = "修改波特率";
			this.btn_modifyBaudrate.Click += this.btn_modifyBaudrate_Click;
			this.btn_check.Image = (Image)componentResourceManager.GetObject("btn_check.Image");
			this.btn_check.Name = "btn_check";
			this.btn_check.Size = new Size(182, 22);
			this.btn_check.Text = "从设备获取数据";
			this.btn_check.Visible = false;
			this.btn_check.Click += this.btn_check_Click;
			this.btn_Log.Image = Resources.Log_Entries;
			this.btn_Log.Name = "btn_Log";
			this.btn_Log.Size = new Size(182, 22);
			this.btn_Log.Text = "日志记录";
			this.btn_Log.Click += this.btn_Log_Click;
			this.btn_SyncToPC.Image = Resources.synchronize_the_data;
			this.btn_SyncToPC.Name = "btn_SyncToPC";
			this.btn_SyncToPC.Size = new Size(182, 22);
			this.btn_SyncToPC.Text = "同步变动数据到设备";
			this.btn_SyncToPC.Click += this.Menu_SyncToPCEx_Click;
			this.btn_Export.Image = Resources.Export;
			this.btn_Export.Name = "btn_Export";
			this.btn_Export.Size = new Size(182, 22);
			this.btn_Export.Text = "导出";
			this.btn_Export.Click += this.btn_Export_Click;
			this.btn_Sd_Record.Image = Resources.system_Initialization;
			this.btn_Sd_Record.Name = "btn_Sd_Record";
			this.btn_Sd_Record.Size = new Size(182, 22);
			this.btn_Sd_Record.Text = "获取SD记录";
			this.btn_Sd_Record.Click += this.btn_SdRecord_Click;
			this.btn_RS485_MasterSlave.Image = Resources.path;
			this.btn_RS485_MasterSlave.Name = "btn_RS485_MasterSlave";
			this.btn_RS485_MasterSlave.Size = new Size(182, 22);
			this.btn_RS485_MasterSlave.Text = "RS485主从机配置";
			this.btn_RS485_MasterSlave.Click += this.btn_RS485_MasterSlave_Click;
			this.btn_GetDataFromUdisk.Image = Resources.ImportUDisk;
			this.btn_GetDataFromUdisk.Name = "btn_GetDataFromUdisk";
			this.btn_GetDataFromUdisk.Size = new Size(182, 22);
			this.btn_GetDataFromUdisk.Text = "从U盘获取数据";
			this.btn_GetDataFromUdisk.Click += this.btn_GetDataFromUdisk_Click;
			this.btn_Export2Udisk.Image = Resources.ExportUDisk;
			this.btn_Export2Udisk.Name = "btn_Export2Udisk";
			this.btn_Export2Udisk.Size = new Size(182, 22);
			this.btn_Export2Udisk.Text = "导出数据到U盘";
			this.btn_Export2Udisk.Click += this.btn_Export2Udisk_Click;
			this.rebootToolStripMenuItem.Image = Resources.devDisable;
			this.rebootToolStripMenuItem.Name = "rebootToolStripMenuItem";
			this.rebootToolStripMenuItem.Size = new Size(182, 22);
			this.rebootToolStripMenuItem.Text = "Reboot";
			this.rebootToolStripMenuItem.Click += this.rebootToolStripMenuItem_Click;
			this.resetDeFábricaToolStripMenuItem.Image = Resources.clear_all_event;
			this.resetDeFábricaToolStripMenuItem.Name = "resetDeFábricaToolStripMenuItem";
			this.resetDeFábricaToolStripMenuItem.Size = new Size(182, 22);
			this.resetDeFábricaToolStripMenuItem.Text = "Reset de Fábrica";
			this.resetDeFábricaToolStripMenuItem.Click += this.resetDeFábricaToolStripMenuItem_Click;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			this.ContextMenuStrip = this.contextMenuStrip2;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.panelEx1);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "DeviceUserControl";
			base.Size = new Size(833, 486);
			this.contextMenuStrip2.ResumeLayout(false);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_MachineView).EndInit();
			((ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
