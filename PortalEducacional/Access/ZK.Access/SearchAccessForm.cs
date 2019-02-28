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
using System.Threading;
using System.Windows.Forms;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access
{
	public class SearchAccessForm : Office2007Form
	{
		public delegate void DealSearchInfo(List<ObjMachine> list);

		private List<string> machinesList = new List<string>();

		private List<string> macList = new List<string>();

		private string exist;

		private bool Presence;

		private int countControl = 0;

		private Thread thread = null;

		private ImagesForm imagesForm = new ImagesForm();

		private DataTable m_485Table = new DataTable();

		private bool m_stopex = false;

		private Thread m_threadex = null;

		private int m_from = 1;

		private int m_to = 1;

		private int m_baudRate = 38400;

		private int m_serialPort = 1;

		private IContainer components = null;

		private DataGridViewX dgrd_AccessControl;

		private ButtonX btn_search;

		private ButtonX btn_modifyIP;

		private ButtonX btn_addDevice;

		private Label lab_doorTotal;

		private ButtonX btn_cancel;

		private ContextMenuStrip menuScript_devMan;

		private ToolStripMenuItem Menu_addDev;

		private ToolStripMenuItem Menu_modifyIP;

		private DevComponents.DotNetBar.TabControl tab_devParameters;

		private TabControlPanel tabControlPanel1;

		private TabItem tabItem_IP;

		private TabControlPanel tabControlPanel3;

		private TabItem tabItem_rs485;

		private DataGridViewX grid_485;

		private ComboBox cbo_serialNo;

		private Label lbl_SerialNo;

		private TextBox txt_RS485;

		private Label lb_485addr;

		private ComboBox cbo_baudRate;

		private Label lbl_BaudRate;

		private ContextMenuStrip contextMenuStrip1;

		private ButtonX btn_startex;

		private ButtonX buttonX2;

		private ButtonX btn_addex;

		private TextBox txt_Rs485end;

		private Label lb_to;

		private ToolStripMenuItem Menu_addDevEx;

		private DataGridViewTextBoxColumn Column_com;

		private DataGridViewTextBoxColumn Column_rs485addr;

		private DataGridViewTextBoxColumn Column_botelv;

		private DataGridViewTextBoxColumn Column_isexcits;

		private DataGridViewTextBoxColumn Column_describe;

		private DataGridViewTextBoxColumn Column_isaddex;

		private DataGridViewTextBoxColumn Column2;

		private DataGridViewTextBoxColumn Column1;

		private DataGridViewTextBoxColumn Column3;

		private DataGridViewTextBoxColumn Column4;

		private DataGridViewTextBoxColumn Column5;

		private DataGridViewTextBoxColumn Column6;

		private DataGridViewTextBoxColumn Column7;

		public SearchAccessForm()
		{
			this.InitializeComponent();
			DevLogServer.LockEx();
			try
			{
				this.btn_modifyIP.Enabled = false;
				this.btn_addDevice.Enabled = false;
				this.Menu_addDev.Enabled = false;
				this.Menu_modifyIP.Enabled = false;
				this.btn_addex.Enabled = false;
				this.Menu_addDevEx.Enabled = false;
				this.InitDefaultsValue();
				this.LoadMachines();
				initLang.LocaleForm(this, base.Name);
				this.lbl_SerialNo_SizeChanged(null, null);
				this.CheckPermission();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void LoadMachines()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.machinesList.Add(list[i].IP);
				}
			}
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.Device))
			{
				this.btn_addDevice.Enabled = false;
				this.btn_modifyIP.Enabled = false;
				this.btn_search.Enabled = false;
				this.Menu_addDev.Enabled = false;
				this.Menu_modifyIP.Enabled = false;
				this.btn_addex.Enabled = false;
				this.btn_startex.Enabled = false;
			}
		}

		private void ModifyIPBtn_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.dgrd_AccessControl.CurrentRow.Cells[6].Value.ToString() == ShowMsgInfos.GetInfo("SSearchDevExist", "是"))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceExistCantNotModifyIP", "该设备已添加，不能修改IP"));
				}
				else if (this.dgrd_AccessControl.CurrentRow.Tag != null)
				{
					string ip = (this.dgrd_AccessControl.CurrentRow.Cells[1].Value == null) ? "" : this.dgrd_AccessControl.CurrentRow.Cells[1].Value.ToString();
					string mac = (this.dgrd_AccessControl.CurrentRow.Cells[0].Value == null) ? "" : ((string)this.dgrd_AccessControl.CurrentRow.Cells[0].Value);
					string netMask = (this.dgrd_AccessControl.CurrentRow.Cells[2].Value == null) ? "" : this.dgrd_AccessControl.CurrentRow.Cells[2].Value.ToString();
					string gateIPAddress = (this.dgrd_AccessControl.CurrentRow.Cells[3].Value == null) ? "" : this.dgrd_AccessControl.CurrentRow.Cells[3].Value.ToString();
					ModifyIPForm modifyIPForm = new ModifyIPForm(ip, mac, netMask, gateIPAddress);
					modifyIPForm.ShowDialog();
					if (modifyIPForm.ModifyIPSuccess)
					{
						if (!string.IsNullOrEmpty(modifyIPForm.newIP))
						{
							this.dgrd_AccessControl.CurrentRow.Cells[1].Value = modifyIPForm.newIP;
							this.dgrd_AccessControl.CurrentRow.Tag = modifyIPForm.newIP;
						}
						if (!string.IsNullOrEmpty(modifyIPForm.newNetMask))
						{
							this.dgrd_AccessControl.CurrentRow.Cells[2].Value = modifyIPForm.newNetMask;
						}
						if (!string.IsNullOrEmpty(modifyIPForm.newGateway))
						{
							this.dgrd_AccessControl.CurrentRow.Cells[3].Value = modifyIPForm.newGateway;
						}
						this.Refresh();
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNotExist", "当前没有设备"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void AddDeviceBtn_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.dgrd_AccessControl.CurrentRow != null && this.dgrd_AccessControl.CurrentRow.Tag != null)
				{
					string ip = this.dgrd_AccessControl.CurrentRow.Tag.ToString();
					string a = this.dgrd_AccessControl.CurrentRow.Cells[6].Value.ToString();
					if (a == ShowMsgInfos.GetInfo("SSearchDevNotExist", "否"))
					{
						AddDeviceForm addDeviceForm = new AddDeviceForm(ip);
						addDeviceForm.btn_saveContinue.Enabled = false;
						addDeviceForm.btn_saveContinue.Visible = false;
						addDeviceForm.ShowDialog();
						if (addDeviceForm.devHasAdd)
						{
							this.dgrd_AccessControl.CurrentRow.Cells[6].Value = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceHasAdd", "该设备已经添加"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNotExist", "当前没有设备"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			DevLogServer.LockEx();
			this.Cursor = Cursors.Default;
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.thread == null && this.thread == null)
				{
					this.SetEnble(false);
					if (this.macList != null)
					{
						this.macList.Clear();
					}
					this.Cursor = Cursors.WaitCursor;
					this.dgrd_AccessControl.Rows.Clear();
					this.lab_doorTotal.Text = ShowMsgInfos.GetInfo("SSearchDoorTotal", "当前共搜索到的门禁控制器总数为:") + "0";
					this.imagesForm.TopMost = true;
					this.imagesForm.Show();
					this.thread = new Thread(this.Start);
					this.thread.Start();
				}
				else
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SWaiting", "请稍等，还有任务没有执行完毕"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
				this.btn_search.Enabled = true;
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.thread != null)
				{
					try
					{
						this.thread.Abort();
					}
					catch
					{
					}
				}
				this.thread = null;
				if (this.m_threadex != null)
				{
					try
					{
						this.m_threadex.Abort();
					}
					catch
					{
					}
				}
				this.imagesForm.Hide();
				this.m_threadex = null;
				this.Cursor = Cursors.Default;
				base.Close();
			}
			catch
			{
				base.Close();
			}
		}

		private static int SortByIP(ObjMachine m1, ObjMachine m2)
		{
			try
			{
				string[] array = m1.IP.Split('.');
				string[] array2 = m2.IP.Split('.');
				int num;
				if (!string.IsNullOrEmpty(array[0]) && !string.IsNullOrEmpty(array2[0]) && array[0] != array2[0])
				{
					num = int.Parse(array[0]);
					return num.CompareTo(int.Parse(array2[0]));
				}
				if (!string.IsNullOrEmpty(array[1]) && !string.IsNullOrEmpty(array2[1]) && array[1] != array2[1])
				{
					num = int.Parse(array[1]);
					return num.CompareTo(int.Parse(array2[1]));
				}
				if (!string.IsNullOrEmpty(array[2]) && !string.IsNullOrEmpty(array2[2]) && array[2] != array2[2])
				{
					num = int.Parse(array[2]);
					return num.CompareTo(int.Parse(array2[2]));
				}
				if (!string.IsNullOrEmpty(array[3]) && !string.IsNullOrEmpty(array2[3]) && array[3] != array2[3])
				{
					num = int.Parse(array[3]);
					return num.CompareTo(int.Parse(array2[3]));
				}
				return 0;
			}
			catch
			{
				return 0;
			}
		}

		private void DealInfo(List<ObjMachine> list)
		{
			try
			{
				bool flag = false;
				if (base.Visible && !base.IsDisposed)
				{
					if (base.InvokeRequired)
					{
						base.Invoke(new DealSearchInfo(this.DealInfo), list);
					}
					else
					{
						this.LoadMachines();
						if (list != null && list.Count > 0)
						{
							list.Sort(SearchAccessForm.SortByIP);
							for (int i = 0; i < list.Count; i++)
							{
								flag = false;
								if (this.macList != null && this.macList.Count > 0)
								{
									int num = 0;
									while (num < this.macList.Count)
									{
										if (!(list[i].MAC == this.macList[num]))
										{
											num++;
											continue;
										}
										flag = true;
										break;
									}
									if (!flag)
									{
										goto IL_0105;
									}
									continue;
								}
								goto IL_0105;
								IL_0105:
								this.exist = ShowMsgInfos.GetInfo("SSearchDevNotExist", "否");
								this.Presence = false;
								foreach (string machines in this.machinesList)
								{
									if (list[i].IP == machines)
									{
										this.Presence = true;
										break;
									}
								}
								if (this.Presence)
								{
									this.exist = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
								}
								if (!string.IsNullOrEmpty(list[i].IP))
								{
									DataGridViewRow dataGridViewRow = new DataGridViewRow();
									dataGridViewRow.Tag = list[i].IP;
									DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = list[i].MAC;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = list[i].IP;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = list[i].NetMask;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = list[i].GATEIPAddress;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = list[i].SN;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = list[i].Device;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									dataGridViewCell = new DataGridViewTextBoxCell();
									dataGridViewCell.Value = this.exist;
									dataGridViewRow.Cells.Add(dataGridViewCell);
									this.dgrd_AccessControl.Rows.Add(dataGridViewRow);
									this.macList.Add(list[i].MAC);
								}
							}
						}
						this.lab_doorTotal.Text = ShowMsgInfos.GetInfo("SSearchDoorTotal", "当前共搜索到的门禁控制器总数为:") + this.macList.Count;
						this.countControl = int.Parse(list.Count.ToString());
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void Start()
		{
			try
			{
				DeviceServerBll deviceServerBll = new DeviceServerBll(MainForm._ia);
				string commType = "UDP";
				string address = "255.255.255.255";
				List<ObjMachine> list = deviceServerBll.SearchDeviceEx(commType, address);
				if (list.Count < 3)
				{
					list = deviceServerBll.SearchDeviceEx(commType, address);
				}
				this.DealInfo(list);
			}
			catch
			{
			}
			this.thread = null;
			this.Finish(null, null);
		}

		private void Finish(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new EventHandler(this.Finish), sender, e);
				}
				else
				{
					if (this.countControl == 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSearchDeviceFailed", "搜索不到任何控制器!"));
					}
					this.Cursor = Cursors.Default;
					this.imagesForm.Hide();
					this.SetEnble(true);
				}
			}
		}

		private void Menu_addDev_Click(object sender, EventArgs e)
		{
			if (this.btn_addDevice.Enabled)
			{
				this.AddDeviceBtn_Click(null, null);
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNotExist", "当前没有设备"));
			}
		}

		private void Menu_modifyIP_Click(object sender, EventArgs e)
		{
			if (this.btn_modifyIP.Enabled)
			{
				this.ModifyIPBtn_Click(null, null);
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNotExist", "当前没有设备"));
			}
		}

		private void dgrd_AccessControl_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				if (this.dgrd_AccessControl.CurrentRow != null && this.dgrd_AccessControl.CurrentRow.Tag != null)
				{
					string ip = this.dgrd_AccessControl.CurrentRow.Tag.ToString();
					string a = this.dgrd_AccessControl.CurrentRow.Cells[6].Value.ToString();
					if (a == ShowMsgInfos.GetInfo("SSearchDevNotExist", "否"))
					{
						AddDeviceForm addDeviceForm = new AddDeviceForm(ip);
						addDeviceForm.ShowDialog();
						if (addDeviceForm.devHasAdd)
						{
							this.dgrd_AccessControl.CurrentRow.Cells[6].Value = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
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

		private void InitDefaultsValue()
		{
			this.cbo_serialNo.Items.Clear();
			for (int i = 1; i < 255; i++)
			{
				this.cbo_serialNo.Items.Add("COM" + i.ToString());
			}
			if (this.cbo_serialNo.Items.Count > 0)
			{
				this.cbo_serialNo.SelectedIndex = 0;
			}
			if (this.cbo_baudRate.Items.Count > 2)
			{
				this.cbo_baudRate.SelectedIndex = 2;
			}
		}

		private void InitColumns()
		{
			if (this.m_485Table.Columns.Count == 0)
			{
				this.m_485Table.Columns.Add("com");
				this.m_485Table.Columns.Add("machinenumber");
				this.m_485Table.Columns.Add("baudrate");
				this.m_485Table.Columns.Add("success");
				this.m_485Table.Columns.Add("describe");
				this.m_485Table.Columns.Add("isadd");
			}
		}

		private void StartEx()
		{
			try
			{
				this.InitColumns();
				this.m_stopex = false;
				this.m_485Table.Clear();
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> list = null;
				list = machinesBll.GetModelList("");
				if (this.m_to >= this.m_from)
				{
					for (int i = this.m_from; i <= this.m_to; i++)
					{
						if (this.m_stopex)
						{
							break;
						}
						ObjDevice objDevice = new ObjDevice();
						objDevice.ConnectType = 0;
						objDevice.IP = string.Empty;
						objDevice.Port = 4730;
						objDevice.MachineNumber = i;
						objDevice.Baudrate = this.m_baudRate;
						objDevice.SerialPort = this.m_serialPort;
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(objDevice);
						DataRow dataRow = this.m_485Table.NewRow();
						dataRow[0] = "COM" + this.m_serialPort;
						dataRow[1] = objDevice.MachineNumber;
						dataRow[2] = objDevice.Baudrate;
						if (deviceServer != null)
						{
							int num = deviceServer.Connect(3000);
							if (num >= 0)
							{
								dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
								dataRow[4] = ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功");
								deviceServer.Disconnect();
								Thread.Sleep(1000);
							}
							else
							{
								switch (num)
								{
								case -14:
									dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
									dataRow[4] = PullSDkErrorInfos.GetInfo(num);
									break;
								case -206:
									Thread.Sleep(2000);
									num = deviceServer.Connect(3000);
									if (num >= 0)
									{
										dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
										dataRow[4] = ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功");
										deviceServer.Disconnect();
										Thread.Sleep(1000);
									}
									else if (num == -14)
									{
										dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
										dataRow[4] = PullSDkErrorInfos.GetInfo(num);
									}
									else
									{
										dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevNotExist", "否");
										dataRow[4] = PullSDkErrorInfos.GetInfo(num);
									}
									break;
								default:
									dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevNotExist", "否");
									dataRow[4] = PullSDkErrorInfos.GetInfo(num);
									break;
								}
							}
						}
						else
						{
							dataRow[3] = ShowMsgInfos.GetInfo("SSearchDevNotExist", "否");
							dataRow[4] = PullSDkErrorInfos.GetInfo(-1002);
						}
						string info = ShowMsgInfos.GetInfo("SSearchDevNotExist", "否");
						bool flag = false;
						int num2 = 0;
						while (num2 < list.Count)
						{
							if (list[num2].ConnectType != 0 || list[num2].MachineNumber != objDevice.MachineNumber || objDevice.SerialPort != list[num2].SerialPort)
							{
								num2++;
								continue;
							}
							flag = true;
							break;
						}
						if (flag)
						{
							info = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
						}
						dataRow[5] = info;
						this.m_485Table.Rows.Add(dataRow);
					}
				}
			}
			catch
			{
			}
			this.m_threadex = null;
			this.OnFinish(this, null);
		}

		private void btn_addex_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.grid_485.CurrentRow != null)
				{
					string a = this.grid_485.CurrentRow.Cells[5].Value.ToString();
					if (a == ShowMsgInfos.GetInfo("SSearchDevNotExist", "否"))
					{
						int com = int.Parse(this.grid_485.CurrentRow.Cells[0].Value.ToString().Substring(3));
						int number = int.Parse(this.grid_485.CurrentRow.Cells[1].Value.ToString());
						int baudRate = int.Parse(this.grid_485.CurrentRow.Cells[2].Value.ToString());
						AddDeviceForm addDeviceForm = new AddDeviceForm(com, number, baudRate);
						addDeviceForm.ShowDialog();
						if (addDeviceForm.devHasAdd)
						{
							this.grid_485.CurrentRow.Cells[5].Value = ShowMsgInfos.GetInfo("SSearchDevExist", "是");
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceHasAdd", "该设备已经添加"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNotExist", "当前没有设备"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.Cursor = Cursors.Default;
			DevLogServer.LockEx();
		}

		private bool Check()
		{
			if (this.thread != null || this.m_threadex != null)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SWaiting", "请稍等，还有任务没有执行完毕"));
				return false;
			}
			if (string.IsNullOrEmpty(this.txt_RS485.Text))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputRS485Address", "请输入RS485地址"));
				this.txt_RS485.Focus();
				return false;
			}
			if (int.Parse(this.txt_RS485.Text) > 255)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressToobig", "RS485地址不能大于255"));
				this.txt_RS485.Focus();
				return false;
			}
			if (string.IsNullOrEmpty(this.txt_Rs485end.Text))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputRS485Address", "请输入RS485地址"));
				this.txt_Rs485end.Focus();
				return false;
			}
			if (int.Parse(this.txt_Rs485end.Text) > 255)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressToobig", "RS485地址不能大于255"));
				this.txt_Rs485end.Focus();
				return false;
			}
			int num = int.Parse(this.txt_RS485.Text);
			int num2 = int.Parse(this.txt_Rs485end.Text);
			if (num < 1)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressFromIsZero", "RS485起始地址不能小于1"));
				this.txt_RS485.Focus();
				return false;
			}
			if (num > num2)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SRS485AddressFromIsOverTo", "RS485起始地址不能大于结束地址"));
				this.txt_RS485.Focus();
				return false;
			}
			return true;
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
					this.grid_485.Rows.Clear();
					for (int i = 0; i < this.m_485Table.Rows.Count; i++)
					{
						DataGridViewRow dataGridViewRow = new DataGridViewRow();
						DataGridViewCell dataGridViewCell = new DataGridViewTextBoxCell();
						dataGridViewCell.Value = this.m_485Table.Rows[i][0].ToString();
						dataGridViewRow.Cells.Add(dataGridViewCell);
						dataGridViewCell = new DataGridViewTextBoxCell();
						dataGridViewCell.Value = this.m_485Table.Rows[i][1].ToString();
						dataGridViewRow.Cells.Add(dataGridViewCell);
						dataGridViewCell = new DataGridViewTextBoxCell();
						dataGridViewCell.Value = this.m_485Table.Rows[i][2].ToString();
						dataGridViewRow.Cells.Add(dataGridViewCell);
						dataGridViewCell = new DataGridViewTextBoxCell();
						dataGridViewCell.Value = this.m_485Table.Rows[i][3].ToString();
						dataGridViewRow.Cells.Add(dataGridViewCell);
						dataGridViewCell = new DataGridViewTextBoxCell();
						dataGridViewCell.Value = this.m_485Table.Rows[i][4].ToString();
						dataGridViewRow.Cells.Add(dataGridViewCell);
						dataGridViewCell = new DataGridViewTextBoxCell();
						dataGridViewCell.Value = this.m_485Table.Rows[i][5].ToString();
						dataGridViewRow.Cells.Add(dataGridViewCell);
						this.grid_485.Rows.Add(dataGridViewRow);
					}
					this.SetEnble(true);
					this.imagesForm.Hide();
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void btn_startex_Click(object sender, EventArgs e)
		{
			if (this.Check())
			{
				this.SetEnble(false);
				if (this.m_threadex == null && this.thread == null)
				{
					this.Cursor = Cursors.WaitCursor;
					this.imagesForm.TopMost = true;
					this.imagesForm.Show();
					this.grid_485.Rows.Clear();
					this.m_from = int.Parse(this.txt_RS485.Text);
					this.m_to = int.Parse(this.txt_Rs485end.Text);
					this.m_baudRate = int.Parse(this.cbo_baudRate.Text);
					this.m_serialPort = int.Parse(this.cbo_serialNo.Text.Substring(3, this.cbo_serialNo.Text.Length - 3));
					Program.KillPlrscagent();
					this.m_threadex = new Thread(this.StartEx);
					this.m_threadex.Start();
				}
				else
				{
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SWaiting", "请稍等，还有任务没有执行完毕"));
				}
			}
		}

		private void SetEnble(bool isenble)
		{
			this.btn_startex.Enabled = isenble;
			if (isenble)
			{
				if (this.grid_485.Rows.Count > 0)
				{
					this.btn_addex.Enabled = isenble;
					this.Menu_addDevEx.Enabled = isenble;
				}
				else
				{
					this.btn_addex.Enabled = false;
					this.Menu_addDevEx.Enabled = false;
				}
			}
			else
			{
				this.Menu_addDevEx.Enabled = isenble;
				this.btn_addex.Enabled = isenble;
			}
			this.lbl_SerialNo.Enabled = isenble;
			this.cbo_serialNo.Enabled = isenble;
			this.lb_485addr.Enabled = isenble;
			this.txt_RS485.Enabled = isenble;
			this.lb_to.Enabled = isenble;
			this.txt_Rs485end.Enabled = isenble;
			this.lbl_BaudRate.Enabled = isenble;
			this.cbo_baudRate.Enabled = isenble;
			if (isenble)
			{
				if (this.dgrd_AccessControl.Rows.Count > 0)
				{
					this.btn_addDevice.Enabled = isenble;
					this.btn_modifyIP.Enabled = isenble;
					this.Menu_addDev.Enabled = isenble;
					this.Menu_modifyIP.Enabled = isenble;
				}
				else
				{
					this.btn_addDevice.Enabled = false;
					this.btn_modifyIP.Enabled = false;
					this.Menu_addDev.Enabled = false;
					this.Menu_modifyIP.Enabled = false;
				}
			}
			else
			{
				this.btn_addDevice.Enabled = isenble;
				this.btn_modifyIP.Enabled = isenble;
				this.Menu_addDev.Enabled = isenble;
				this.Menu_modifyIP.Enabled = isenble;
			}
			this.btn_search.Enabled = isenble;
		}

		private void buttonX3_Click(object sender, EventArgs e)
		{
			this.btn_cancel_Click(null, null);
		}

		private void txt_RS485_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 253L);
		}

		private void txt_Rs485end_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 255L);
		}

		private void lbl_SerialNo_SizeChanged(object sender, EventArgs e)
		{
			this.cbo_serialNo.Left = this.lbl_SerialNo.Left + this.lbl_SerialNo.Width + 5;
			this.lb_485addr.Left = this.cbo_serialNo.Left + this.cbo_serialNo.Width + 5;
			this.txt_RS485.Left = this.lb_485addr.Left + this.lb_485addr.Width + 5;
			this.lb_to.Left = this.txt_RS485.Left + this.txt_RS485.Width + 5;
			this.txt_Rs485end.Left = this.lb_to.Left + this.lb_to.Width + 5;
			this.lbl_BaudRate.Left = this.txt_Rs485end.Left + this.txt_Rs485end.Width + 5;
			this.cbo_baudRate.Left = this.lbl_BaudRate.Left + this.lbl_BaudRate.Width + 5;
		}

		private void SearchAccessForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.m_threadex != null || this.thread != null)
			{
				e.Cancel = true;
			}
		}

		private void Menu_addDevEx_Click(object sender, EventArgs e)
		{
			this.btn_addex_Click(null, null);
		}

		private void grid_485_DoubleClick(object sender, EventArgs e)
		{
			if (this.grid_485.CurrentRow != null)
			{
				this.btn_addex_Click(null, null);
			}
		}

		private void SearchAccessForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.imagesForm.Hide();
			DevLogServer.UnLockEx();
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
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SearchAccessForm));
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			this.dgrd_AccessControl = new DataGridViewX();
			this.menuScript_devMan = new ContextMenuStrip(this.components);
			this.Menu_addDev = new ToolStripMenuItem();
			this.Menu_modifyIP = new ToolStripMenuItem();
			this.btn_search = new ButtonX();
			this.btn_modifyIP = new ButtonX();
			this.btn_addDevice = new ButtonX();
			this.lab_doorTotal = new Label();
			this.btn_cancel = new ButtonX();
			this.tab_devParameters = new DevComponents.DotNetBar.TabControl();
			this.tabControlPanel1 = new TabControlPanel();
			this.tabItem_IP = new TabItem(this.components);
			this.tabControlPanel3 = new TabControlPanel();
			this.cbo_baudRate = new ComboBox();
			this.txt_Rs485end = new TextBox();
			this.txt_RS485 = new TextBox();
			this.cbo_serialNo = new ComboBox();
			this.lb_to = new Label();
			this.btn_startex = new ButtonX();
			this.buttonX2 = new ButtonX();
			this.btn_addex = new ButtonX();
			this.lbl_BaudRate = new Label();
			this.lb_485addr = new Label();
			this.lbl_SerialNo = new Label();
			this.grid_485 = new DataGridViewX();
			this.Column_com = new DataGridViewTextBoxColumn();
			this.Column_rs485addr = new DataGridViewTextBoxColumn();
			this.Column_botelv = new DataGridViewTextBoxColumn();
			this.Column_isexcits = new DataGridViewTextBoxColumn();
			this.Column_describe = new DataGridViewTextBoxColumn();
			this.Column_isaddex = new DataGridViewTextBoxColumn();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.Menu_addDevEx = new ToolStripMenuItem();
			this.tabItem_rs485 = new TabItem(this.components);
			this.Column2 = new DataGridViewTextBoxColumn();
			this.Column1 = new DataGridViewTextBoxColumn();
			this.Column3 = new DataGridViewTextBoxColumn();
			this.Column4 = new DataGridViewTextBoxColumn();
			this.Column5 = new DataGridViewTextBoxColumn();
			this.Column6 = new DataGridViewTextBoxColumn();
			this.Column7 = new DataGridViewTextBoxColumn();
			((ISupportInitialize)this.dgrd_AccessControl).BeginInit();
			this.menuScript_devMan.SuspendLayout();
			((ISupportInitialize)this.tab_devParameters).BeginInit();
			this.tab_devParameters.SuspendLayout();
			this.tabControlPanel1.SuspendLayout();
			this.tabControlPanel3.SuspendLayout();
			((ISupportInitialize)this.grid_485).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.dgrd_AccessControl.AllowUserToAddRows = false;
			this.dgrd_AccessControl.AllowUserToDeleteRows = false;
			this.dgrd_AccessControl.AllowUserToResizeColumns = false;
			this.dgrd_AccessControl.AllowUserToResizeRows = false;
			this.dgrd_AccessControl.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.dgrd_AccessControl.BackgroundColor = Color.White;
			this.dgrd_AccessControl.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgrd_AccessControl.Columns.AddRange(this.Column2, this.Column1, this.Column3, this.Column4, this.Column5, this.Column6, this.Column7);
			this.dgrd_AccessControl.ContextMenuStrip = this.menuScript_devMan;
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("SimSun", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.dgrd_AccessControl.DefaultCellStyle = dataGridViewCellStyle;
			this.dgrd_AccessControl.GridColor = Color.FromArgb(208, 215, 229);
			this.dgrd_AccessControl.ImeMode = ImeMode.NoControl;
			this.dgrd_AccessControl.Location = new Point(12, 8);
			this.dgrd_AccessControl.Name = "dgrd_AccessControl";
			this.dgrd_AccessControl.RowTemplate.Height = 23;
			this.dgrd_AccessControl.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgrd_AccessControl.Size = new Size(1026, 361);
			this.dgrd_AccessControl.TabIndex = 0;
			this.dgrd_AccessControl.DoubleClick += this.dgrd_AccessControl_DoubleClick;
			this.menuScript_devMan.Items.AddRange(new ToolStripItem[2]
			{
				this.Menu_addDev,
				this.Menu_modifyIP
			});
			this.menuScript_devMan.Name = "contextMenuStrip1";
			this.menuScript_devMan.Size = new Size(136, 48);
			this.Menu_addDev.Image = Resources.add;
			this.Menu_addDev.Name = "Menu_addDev";
			this.Menu_addDev.Size = new Size(135, 22);
			this.Menu_addDev.Text = "添加设备";
			this.Menu_addDev.Click += this.Menu_addDev_Click;
			this.Menu_modifyIP.Image = (Image)componentResourceManager.GetObject("Menu_modifyIP.Image");
			this.Menu_modifyIP.Name = "Menu_modifyIP";
			this.Menu_modifyIP.Size = new Size(135, 22);
			this.Menu_modifyIP.Text = "修改IP地址";
			this.Menu_modifyIP.Click += this.Menu_modifyIP_Click;
			this.btn_search.AccessibleRole = AccessibleRole.PushButton;
			this.btn_search.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_search.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_search.Location = new Point(770, 384);
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(130, 23);
			this.btn_search.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_search.TabIndex = 4;
			this.btn_search.Text = "开始搜索";
			this.btn_search.Click += this.btn_search_Click;
			this.btn_modifyIP.AccessibleRole = AccessibleRole.PushButton;
			this.btn_modifyIP.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_modifyIP.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_modifyIP.Location = new Point(494, 384);
			this.btn_modifyIP.Name = "btn_modifyIP";
			this.btn_modifyIP.Size = new Size(130, 23);
			this.btn_modifyIP.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_modifyIP.TabIndex = 2;
			this.btn_modifyIP.Text = "修改IP地址";
			this.btn_modifyIP.Click += this.ModifyIPBtn_Click;
			this.btn_addDevice.AccessibleRole = AccessibleRole.PushButton;
			this.btn_addDevice.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_addDevice.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_addDevice.Location = new Point(632, 384);
			this.btn_addDevice.Name = "btn_addDevice";
			this.btn_addDevice.Size = new Size(130, 23);
			this.btn_addDevice.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_addDevice.TabIndex = 3;
			this.btn_addDevice.Text = "添加设备";
			this.btn_addDevice.Click += this.AddDeviceBtn_Click;
			this.lab_doorTotal.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
			this.lab_doorTotal.BackColor = Color.Transparent;
			this.lab_doorTotal.Location = new Point(12, 390);
			this.lab_doorTotal.Name = "lab_doorTotal";
			this.lab_doorTotal.Size = new Size(436, 12);
			this.lab_doorTotal.TabIndex = 4;
			this.lab_doorTotal.Text = "当前共搜索到的门禁控制器总数为:0";
			this.lab_doorTotal.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(908, 384);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(130, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.tab_devParameters.BackColor = Color.FromArgb(194, 217, 247);
			this.tab_devParameters.CanReorderTabs = true;
			this.tab_devParameters.Controls.Add(this.tabControlPanel1);
			this.tab_devParameters.Controls.Add(this.tabControlPanel3);
			this.tab_devParameters.Dock = DockStyle.Fill;
			this.tab_devParameters.Location = new Point(0, 0);
			this.tab_devParameters.Name = "tab_devParameters";
			this.tab_devParameters.SelectedTabFont = new Font("SimSun", 9f, FontStyle.Bold);
			this.tab_devParameters.SelectedTabIndex = 0;
			this.tab_devParameters.Size = new Size(1053, 448);
			this.tab_devParameters.TabIndex = 8;
			this.tab_devParameters.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
			this.tab_devParameters.Tabs.Add(this.tabItem_IP);
			this.tab_devParameters.Tabs.Add(this.tabItem_rs485);
			this.tab_devParameters.Text = "tabControl1";
			this.tabControlPanel1.Controls.Add(this.btn_modifyIP);
			this.tabControlPanel1.Controls.Add(this.dgrd_AccessControl);
			this.tabControlPanel1.Controls.Add(this.lab_doorTotal);
			this.tabControlPanel1.Controls.Add(this.btn_search);
			this.tabControlPanel1.Controls.Add(this.btn_cancel);
			this.tabControlPanel1.Controls.Add(this.btn_addDevice);
			this.tabControlPanel1.Dock = DockStyle.Fill;
			this.tabControlPanel1.Location = new Point(0, 26);
			this.tabControlPanel1.Name = "tabControlPanel1";
			this.tabControlPanel1.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel1.Size = new Size(1053, 422);
			this.tabControlPanel1.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel1.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel1.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel1.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel1.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel1.Style.GradientAngle = 90;
			this.tabControlPanel1.TabIndex = 1;
			this.tabControlPanel1.TabItem = this.tabItem_IP;
			this.tabItem_IP.AttachedControl = this.tabControlPanel1;
			this.tabItem_IP.Name = "tabItem_IP";
			this.tabItem_IP.Text = "以太网搜索";
			this.tabControlPanel3.Controls.Add(this.cbo_baudRate);
			this.tabControlPanel3.Controls.Add(this.txt_Rs485end);
			this.tabControlPanel3.Controls.Add(this.txt_RS485);
			this.tabControlPanel3.Controls.Add(this.cbo_serialNo);
			this.tabControlPanel3.Controls.Add(this.lb_to);
			this.tabControlPanel3.Controls.Add(this.btn_startex);
			this.tabControlPanel3.Controls.Add(this.buttonX2);
			this.tabControlPanel3.Controls.Add(this.btn_addex);
			this.tabControlPanel3.Controls.Add(this.lbl_BaudRate);
			this.tabControlPanel3.Controls.Add(this.lb_485addr);
			this.tabControlPanel3.Controls.Add(this.lbl_SerialNo);
			this.tabControlPanel3.Controls.Add(this.grid_485);
			this.tabControlPanel3.Dock = DockStyle.Fill;
			this.tabControlPanel3.Location = new Point(0, 26);
			this.tabControlPanel3.Name = "tabControlPanel3";
			this.tabControlPanel3.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel3.Size = new Size(1053, 422);
			this.tabControlPanel3.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel3.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel3.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel3.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel3.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel3.Style.GradientAngle = 90;
			this.tabControlPanel3.TabIndex = 0;
			this.tabControlPanel3.TabItem = this.tabItem_rs485;
			this.cbo_baudRate.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_baudRate.FormattingEnabled = true;
			this.cbo_baudRate.Items.AddRange(new object[5]
			{
				"9600",
				"19200 ",
				"38400 ",
				"57600 ",
				"115200"
			});
			this.cbo_baudRate.Location = new Point(384, 387);
			this.cbo_baudRate.Name = "cbo_baudRate";
			this.cbo_baudRate.Size = new Size(69, 20);
			this.cbo_baudRate.TabIndex = 9;
			this.txt_Rs485end.Location = new Point(279, 387);
			this.txt_Rs485end.MaxLength = 3;
			this.txt_Rs485end.Name = "txt_Rs485end";
			this.txt_Rs485end.Size = new Size(52, 21);
			this.txt_Rs485end.TabIndex = 8;
			this.txt_Rs485end.Text = "5";
			this.txt_Rs485end.KeyPress += this.txt_Rs485end_KeyPress;
			this.txt_RS485.Location = new Point(200, 387);
			this.txt_RS485.MaxLength = 3;
			this.txt_RS485.Name = "txt_RS485";
			this.txt_RS485.Size = new Size(50, 21);
			this.txt_RS485.TabIndex = 7;
			this.txt_RS485.Text = "1";
			this.txt_RS485.KeyPress += this.txt_RS485_KeyPress;
			this.cbo_serialNo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_serialNo.FormattingEnabled = true;
			this.cbo_serialNo.Items.AddRange(new object[15]
			{
				"COM1",
				"COM2",
				"COM3",
				"COM4",
				"COM5",
				"COM6",
				"COM7",
				"COM8",
				"COM9",
				"COM10",
				"COM11",
				"COM12",
				"COM13",
				"COM14",
				"COM15"
			});
			this.cbo_serialNo.Location = new Point(59, 387);
			this.cbo_serialNo.Name = "cbo_serialNo";
			this.cbo_serialNo.Size = new Size(70, 20);
			this.cbo_serialNo.TabIndex = 6;
			this.lb_to.AutoSize = true;
			this.lb_to.BackColor = Color.Transparent;
			this.lb_to.Location = new Point(256, 390);
			this.lb_to.Name = "lb_to";
			this.lb_to.Size = new Size(17, 12);
			this.lb_to.TabIndex = 48;
			this.lb_to.Text = "到";
			this.lb_to.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_to.SizeChanged += this.lbl_SerialNo_SizeChanged;
			this.btn_startex.AccessibleRole = AccessibleRole.PushButton;
			this.btn_startex.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_startex.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_startex.Location = new Point(771, 384);
			this.btn_startex.Name = "btn_startex";
			this.btn_startex.Size = new Size(130, 23);
			this.btn_startex.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_startex.TabIndex = 11;
			this.btn_startex.Text = "开始搜索";
			this.btn_startex.Click += this.btn_startex_Click;
			this.buttonX2.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX2.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.buttonX2.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX2.Location = new Point(909, 384);
			this.buttonX2.Name = "buttonX2";
			this.buttonX2.Size = new Size(130, 23);
			this.buttonX2.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX2.TabIndex = 12;
			this.buttonX2.Text = "返回";
			this.buttonX2.Click += this.buttonX3_Click;
			this.btn_addex.AccessibleRole = AccessibleRole.PushButton;
			this.btn_addex.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_addex.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_addex.Location = new Point(633, 384);
			this.btn_addex.Name = "btn_addex";
			this.btn_addex.Size = new Size(130, 23);
			this.btn_addex.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_addex.TabIndex = 10;
			this.btn_addex.Text = "添加设备";
			this.btn_addex.Click += this.btn_addex_Click;
			this.lbl_BaudRate.AutoSize = true;
			this.lbl_BaudRate.BackColor = Color.Transparent;
			this.lbl_BaudRate.Location = new Point(337, 390);
			this.lbl_BaudRate.Name = "lbl_BaudRate";
			this.lbl_BaudRate.Size = new Size(41, 12);
			this.lbl_BaudRate.TabIndex = 44;
			this.lbl_BaudRate.Text = "波特率";
			this.lbl_BaudRate.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_BaudRate.SizeChanged += this.lbl_SerialNo_SizeChanged;
			this.lb_485addr.AutoSize = true;
			this.lb_485addr.BackColor = Color.Transparent;
			this.lb_485addr.Location = new Point(135, 390);
			this.lb_485addr.Name = "lb_485addr";
			this.lb_485addr.Size = new Size(59, 12);
			this.lb_485addr.TabIndex = 42;
			this.lb_485addr.Text = "RS485地址";
			this.lb_485addr.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_485addr.SizeChanged += this.lbl_SerialNo_SizeChanged;
			this.lbl_SerialNo.AutoSize = true;
			this.lbl_SerialNo.BackColor = Color.Transparent;
			this.lbl_SerialNo.Location = new Point(12, 390);
			this.lbl_SerialNo.Name = "lbl_SerialNo";
			this.lbl_SerialNo.Size = new Size(41, 12);
			this.lbl_SerialNo.TabIndex = 40;
			this.lbl_SerialNo.Text = "串口号";
			this.lbl_SerialNo.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_SerialNo.SizeChanged += this.lbl_SerialNo_SizeChanged;
			this.grid_485.AllowUserToAddRows = false;
			this.grid_485.AllowUserToDeleteRows = false;
			this.grid_485.AllowUserToResizeColumns = false;
			this.grid_485.AllowUserToResizeRows = false;
			this.grid_485.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.grid_485.BackgroundColor = Color.White;
			this.grid_485.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grid_485.Columns.AddRange(this.Column_com, this.Column_rs485addr, this.Column_botelv, this.Column_isexcits, this.Column_describe, this.Column_isaddex);
			this.grid_485.ContextMenuStrip = this.contextMenuStrip1;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = SystemColors.Window;
			dataGridViewCellStyle2.Font = new Font("SimSun", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
			this.grid_485.DefaultCellStyle = dataGridViewCellStyle2;
			this.grid_485.GridColor = Color.FromArgb(208, 215, 229);
			this.grid_485.ImeMode = ImeMode.NoControl;
			this.grid_485.Location = new Point(12, 8);
			this.grid_485.Name = "grid_485";
			this.grid_485.RowTemplate.Height = 23;
			this.grid_485.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.grid_485.Size = new Size(1026, 361);
			this.grid_485.TabIndex = 1;
			this.grid_485.DoubleClick += this.grid_485_DoubleClick;
			this.Column_com.HeaderText = "串口号";
			this.Column_com.Name = "Column_com";
			this.Column_com.ReadOnly = true;
			this.Column_com.Width = 150;
			this.Column_rs485addr.HeaderText = "RS485地址";
			this.Column_rs485addr.Name = "Column_rs485addr";
			this.Column_rs485addr.ReadOnly = true;
			this.Column_rs485addr.Width = 150;
			this.Column_botelv.HeaderText = "波特率";
			this.Column_botelv.Name = "Column_botelv";
			this.Column_botelv.ReadOnly = true;
			this.Column_botelv.Width = 150;
			this.Column_isexcits.HeaderText = "设备是否存在";
			this.Column_isexcits.Name = "Column_isexcits";
			this.Column_isexcits.ReadOnly = true;
			this.Column_isexcits.Width = 150;
			this.Column_describe.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.Column_describe.HeaderText = "描述";
			this.Column_describe.Name = "Column_describe";
			this.Column_describe.ReadOnly = true;
			this.Column_isaddex.HeaderText = "已添加";
			this.Column_isaddex.Name = "Column_isaddex";
			this.Column_isaddex.ReadOnly = true;
			this.Column_isaddex.Width = 150;
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[1]
			{
				this.Menu_addDevEx
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(125, 26);
			this.Menu_addDevEx.Image = (Image)componentResourceManager.GetObject("Menu_addDevEx.Image");
			this.Menu_addDevEx.Name = "Menu_addDevEx";
			this.Menu_addDevEx.Size = new Size(124, 22);
			this.Menu_addDevEx.Text = "添加设备";
			this.Menu_addDevEx.Click += this.Menu_addDevEx_Click;
			this.tabItem_rs485.AttachedControl = this.tabControlPanel3;
			this.tabItem_rs485.Name = "tabItem_rs485";
			this.tabItem_rs485.Text = "RS485搜索";
			this.Column2.HeaderText = "MAC地址";
			this.Column2.Name = "Column2";
			this.Column2.ReadOnly = true;
			this.Column2.Width = 120;
			this.Column1.HeaderText = "IP地址";
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			this.Column1.Width = 120;
			this.Column3.HeaderText = "子网掩码";
			this.Column3.Name = "Column3";
			this.Column3.ReadOnly = true;
			this.Column3.Width = 120;
			this.Column4.HeaderText = "网关";
			this.Column4.Name = "Column4";
			this.Column4.ReadOnly = true;
			this.Column4.Width = 120;
			this.Column5.HeaderText = "序列号";
			this.Column5.Name = "Column5";
			this.Column5.ReadOnly = true;
			this.Column5.Width = 120;
			this.Column6.HeaderText = "设备类型";
			this.Column6.Name = "Column6";
			this.Column6.ReadOnly = true;
			this.Column6.Width = 120;
			this.Column7.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.Column7.HeaderText = "已添加";
			this.Column7.Name = "Column7";
			this.Column7.ReadOnly = true;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1053, 448);
			base.Controls.Add(this.tab_devParameters);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SearchAccessForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "搜索门禁控制器";
			base.FormClosing += this.SearchAccessForm_FormClosing;
			base.FormClosed += this.SearchAccessForm_FormClosed;
			((ISupportInitialize)this.dgrd_AccessControl).EndInit();
			this.menuScript_devMan.ResumeLayout(false);
			((ISupportInitialize)this.tab_devParameters).EndInit();
			this.tab_devParameters.ResumeLayout(false);
			this.tabControlPanel1.ResumeLayout(false);
			this.tabControlPanel3.ResumeLayout(false);
			this.tabControlPanel3.PerformLayout();
			((ISupportInitialize)this.grid_485).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
