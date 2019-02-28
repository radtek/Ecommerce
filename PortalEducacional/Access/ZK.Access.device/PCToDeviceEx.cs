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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.device
{
	public class PCToDeviceEx : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowInfoEx(int index, string info);

		public delegate void ShowProgressHandle(int currProgress);

		private bool RunImmediateCommands = false;

		private bool RunOnlyLoggedUserCommands = false;

		private DeviceHelperEx helper = new DeviceHelperEx();

		private List<int> m_deviceSelectedTable = null;

		private Thread m_thead = null;

		private bool m_isStopEx = false;

		private static bool m_isShow = false;

		private IContainer components = null;

		private LabelX lb_progress;

		private ButtonX btn_exit;

		private ButtonX btn_Update;

		private ProgressBar progressBarUp;

		private TextBox txt_UpLoadInfo;

		private DataGridViewX gv;

		private Label label1;

		private System.Windows.Forms.Timer timer1;

		private LabelX lb_progressAll;

		private ProgressBar progress_all;

		private ButtonX btn_show;

		private DataGridViewTextBoxColumn Column_Inport;

		private DataGridViewTextBoxColumn Column_Source;

		private DataGridViewTextBoxColumn Column1;

		private DataGridViewTextBoxColumn Column2;

		private DataGridViewTextBoxColumn Column_RS485;

		private DataGridViewTextBoxColumn Column_Status;

		private Label lblProgressAll;

		private Label lblProgress;

		public static bool IsShow => PCToDeviceEx.m_isShow;

		public PCToDeviceEx()
		{
			this.RunImmediateCommands = false;
			this.RunOnlyLoggedUserCommands = false;
			this.InitializeComponent();
			this.LoadDev();
			initLang.LocaleForm(this, base.Name);
			this.helper.ShowInfoEvent += this.OnShowInfo;
			this.helper.ShowProgressEvent += this.OnShowProgress;
			this.btn_Update.Enabled = false;
			this.btn_exit.Enabled = false;
			this.timer1.Interval = 500;
			this.timer1.Enabled = true;
			this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
			this.btn_show_Click(null, null);
		}

		public PCToDeviceEx(List<int> machineSelectedInfo)
		{
			this.RunImmediateCommands = false;
			this.RunOnlyLoggedUserCommands = false;
			this.InitializeComponent();
			this.m_deviceSelectedTable = machineSelectedInfo;
			this.LoadDev();
			initLang.LocaleForm(this, base.Name);
			this.helper.ShowInfoEvent += this.OnShowInfo;
			this.helper.ShowProgressEvent += this.OnShowProgress;
			this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
			this.btn_show_Click(null, null);
		}

		public void setRunImmediateCommands(bool value)
		{
			this.RunImmediateCommands = value;
		}

		public void setRunOnlyLoggedUserCommands(bool value)
		{
			this.RunOnlyLoggedUserCommands = value;
		}

		private void LoadDev()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			int num;
			if (this.m_deviceSelectedTable != null && this.m_deviceSelectedTable.Count > 0)
			{
				for (int i = 0; i < this.m_deviceSelectedTable.Count; i++)
				{
					Machines model = machinesBll.GetModel(this.m_deviceSelectedTable[i]);
					if (model != null)
					{
						int index = this.gv.Rows.Add();
						this.gv.Rows[index].Cells[0].Value = model.MachineAlias;
						if (model.ConnectType == 0)
						{
							this.gv.Rows[index].Cells[1].Value = "RS485";
							this.gv.Rows[index].Cells[2].Value = "";
							DataGridViewCell dataGridViewCell = this.gv.Rows[index].Cells[3];
							num = model.SerialPort;
							dataGridViewCell.Value = "COM" + num.ToString();
							DataGridViewCell dataGridViewCell2 = this.gv.Rows[index].Cells[4];
							num = model.MachineNumber;
							dataGridViewCell2.Value = num.ToString();
							this.gv.Rows[index].Cells[5].Value = "";
						}
						else
						{
							this.gv.Rows[index].Cells[1].Value = "TCP/IP";
							this.gv.Rows[index].Cells[2].Value = model.IP;
							this.gv.Rows[index].Cells[3].Value = "";
							this.gv.Rows[index].Cells[4].Value = "";
							this.gv.Rows[index].Cells[5].Value = "";
						}
					}
				}
			}
			else
			{
				List<Machines> modelList = machinesBll.GetModelList("id in (select SN_id from devcmds where status=0)");
				this.m_deviceSelectedTable = new List<int>();
				if (modelList != null && modelList.Count > 0)
				{
					for (int j = 0; j < modelList.Count; j++)
					{
						Machines machines = modelList[j];
						if (machines != null)
						{
							this.m_deviceSelectedTable.Add(machines.ID);
							int index2 = this.gv.Rows.Add();
							this.gv.Rows[index2].Cells[0].Value = machines.MachineAlias;
							if (machines.ConnectType == 0)
							{
								this.gv.Rows[index2].Cells[1].Value = "RS485";
								this.gv.Rows[index2].Cells[2].Value = "";
								DataGridViewCell dataGridViewCell3 = this.gv.Rows[index2].Cells[3];
								num = machines.SerialPort;
								dataGridViewCell3.Value = "COM" + num.ToString();
								DataGridViewCell dataGridViewCell4 = this.gv.Rows[index2].Cells[4];
								num = machines.MachineNumber;
								dataGridViewCell4.Value = num.ToString();
								this.gv.Rows[index2].Cells[5].Value = "";
							}
							else
							{
								this.gv.Rows[index2].Cells[1].Value = "TCP/IP";
								this.gv.Rows[index2].Cells[2].Value = machines.IP;
								this.gv.Rows[index2].Cells[3].Value = "";
								this.gv.Rows[index2].Cells[4].Value = "";
								this.gv.Rows[index2].Cells[5].Value = "";
							}
						}
					}
				}
			}
		}

		private void OnShowInfo(string UpLoadinfoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.OnShowInfo), UpLoadinfoStr);
				}
				else
				{
					int num = 50000;
					string str = UpLoadinfoStr;
					if (UpLoadinfoStr.Length > num)
					{
						str = UpLoadinfoStr.Substring(0, num) + "......";
					}
					str = DateTime.Now.ToString() + " " + str;
					str = str.TrimEnd("\r\n".ToCharArray());
					this.txt_UpLoadInfo.AppendText(str + "\r\n");
					this.Refresh();
				}
			}
		}

		private void gvShowInfo(int index, string infoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfoEx(this.gvShowInfo), index, infoStr);
				}
				else
				{
					this.gv.Rows[index].Cells[5].Value = infoStr;
					this.Refresh();
				}
			}
		}

		private void OnShowProgress(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.OnShowProgress), prg);
				}
				else
				{
					if (prg > 100)
					{
						prg = 100;
					}
					if (prg < 0)
					{
						prg = 0;
					}
					this.progressBarUp.Value = prg;
					this.lblProgress.Text = prg.ToString() + "%";
					this.Refresh();
				}
			}
		}

		private void OnShowProgressAll(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.OnShowProgressAll), prg);
				}
				else
				{
					if (prg >= 100)
					{
						prg = 100;
					}
					if (prg <= 0)
					{
						prg = 0;
					}
					this.progress_all.Value = prg;
					this.lblProgressAll.Text = prg.ToString() + "%";
					this.Refresh();
				}
			}
		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			base.Close();
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
					this.Cursor = Cursors.Default;
					this.btn_Update.Enabled = true;
					this.btn_exit.Enabled = true;
					this.Refresh();
				}
			}
		}

		private void StartUpdate()
		{
			bool flag = true;
			bool flag2 = false;
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
			this.OnShowProgressAll(0);
			for (int i = 0; i < this.m_deviceSelectedTable.Count && !this.m_isStopEx; i++)
			{
				this.OnShowProgress(0);
				this.OnShowProgressAll(i * 100 / this.m_deviceSelectedTable.Count);
				Machines model = machinesBll.GetModel(this.m_deviceSelectedTable[i]);
				if (model != null)
				{
					DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
					if (deviceServer != null)
					{
						if (deviceServer.IsConnected)
						{
							deviceServer.Disconnect();
							if (deviceServer.DevInfo.ConnectType == 0)
							{
								Program.KillPlrscagent();
							}
						}
						int num;
						if (!deviceServer.IsConnected)
						{
							this.OnShowInfo(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + model.MachineAlias + ")");
							num = deviceServer.Connect(3000);
							if (num < 0)
							{
								this.OnShowInfo(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + "(" + model.MachineAlias + "):" + PullSDkErrorInfos.GetInfo(num));
								this.OnShowProgress(100);
								this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsFail", "失败"));
								this.ShowErrorInfo(this, null);
								continue;
							}
							this.OnShowInfo(ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功") + "(" + model.MachineAlias + ")");
						}
						this.OnShowInfo(ShowMsgInfos.GetInfo("SUploadingData", "正在从PC同步信息到设备，请稍候...") + "(" + model.MachineAlias + ")");
						if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
						{
							if (DeviceHelper.UpLoadDataToDeviceEx(model, deviceServer, (DeviceHelper.ShowInfoHandler)this.OnShowInfo, (DeviceHelper.ShowProgressHandler)this.OnShowProgress, (Form)this, out bool flag3))
							{
								this.gvShowInfo(i, ShowMsgInfos.GetInfo("FinishEx", "完成"));
							}
							else
							{
								this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsFail", "失败"));
								this.ShowErrorInfo(this, null);
							}
							if (flag3)
							{
								flag2 = true;
							}
						}
						else
						{
							DataSet list = devCmdsBll.GetList("status=0 and SN_id=" + model.ID + (this.RunOnlyLoggedUserCommands ? (" and create_operator = " + ((DevCmds.CreateOperatorLogin == null) ? "null" : ("'" + DevCmds.CreateOperatorLogin + "'"))) : "") + (this.RunImmediateCommands ? (" and CmdImmediately = " + ((AppSite.Instance.DataType == DataType.Access) ? "true" : "1")) : "") + " order by  CmdImmediately desc, id asc ");
							this.OnShowProgress(0);
							bool flag4 = false;
							if (list != null && list.Tables.Count > 0 && list.Tables[0].Rows.Count > 0)
							{
								list.Tables[0].BeginLoadData();
								DataRowCollection rows = list.Tables[0].Rows;
								for (int j = 0; j < rows.Count; j++)
								{
									string text = rows[j]["CmdContent"].ToString();
									if (!string.IsNullOrEmpty(text))
									{
										string input = text;
										input = Regex.Replace(input, "Password\\=.*\\t", "Password=*****\t");
										this.OnShowInfo(ShowMsgInfos.GetInfo("SUploadingDataStart", "开始同步命令") + ":" + input);
										int num2 = CommandServer.PcToDevice(deviceServer, text);
										DevCmds devCmds = new DevCmds();
										devCmds.id = int.Parse(rows[j]["id"].ToString());
										devCmds.CmdTransTime = DateTime.Now;
										if (num2 == 0)
										{
											devCmds.status = 1;
											devCmds.CmdReturnContent = "ok";
											this.OnShowInfo(ShowMsgInfos.GetInfo("SUploadingDataFinish", "同步命令结束"));
										}
										else
										{
											devCmds.status = 0;
											devCmds.CmdReturnContent = num2.ToString() + "->" + PullSDkErrorInfos.GetInfo(num2);
											this.OnShowInfo(ShowMsgInfos.GetInfo("SUploadingDataFail", "同步命令失败") + ": " + num2.ToString() + "->" + devCmds.CmdReturnContent);
										}
										devCmdsBll.Update(devCmds);
										if (devCmds.status == 0)
										{
											flag4 = true;
											break;
										}
									}
									this.OnShowProgress(j * 100 / list.Tables[0].Rows.Count);
									Application.DoEvents();
								}
								list.Tables[0].EndLoadData();
							}
							else
							{
								this.OnShowInfo(ShowMsgInfos.GetInfo("NoUploadData", "没有需要同步的信息") + "(" + model.MachineAlias + ")");
							}
							if (flag4)
							{
								this.ShowErrorInfo(this, null);
								this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsFail", "失败"));
							}
							else
							{
								this.OnShowProgress(100);
								this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
								this.OnShowInfo(ShowMsgInfos.GetInfo("SUploadDataSuccess", "从PC同步信息到设备完成") + "(" + model.MachineAlias + ")");
							}
						}
						num = DeviceHelper.UpdateDataCount(model, null);
						if (num >= 0)
						{
							machinesBll.Update(model);
							if (flag2)
							{
								deviceServer.RebootDevice();
							}
						}
						deviceServer.Disconnect();
					}
					else
					{
						this.OnShowInfo(ShowMsgInfos.GetInfo("SDisableDevice", "设备已被禁用") + "(" + model.MachineAlias + ")");
					}
				}
				this.OnShowProgress(100);
				Application.DoEvents();
			}
			this.OnShowProgress(100);
			this.OnShowProgressAll(100);
			SysLogServer.WriteLog("同步结束...", true);
			this.OnFinish(this, null);
			SysLogServer.WriteLog("结束线程...", true);
			this.m_thead = null;
			if (GLOBAL.IsMonitorActive() && GLOBAL.IsMonitorOwner)
			{
				GLOBAL.monitorPerformingUpdate = false;
				base.Invoke((MethodInvoker)delegate
				{
					this.btn_exit_Click(this, new EventArgs());
				});
			}
		}

		private void btn_Update_Click(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			this.btn_Update.Enabled = false;
			this.btn_exit.Enabled = false;
			try
			{
				if (this.m_thead == null)
				{
					this.Cursor = Cursors.WaitCursor;
					this.m_thead = new Thread(this.StartUpdate);
					this.m_thead.Start();
				}
				else
				{
					this.Cursor = Cursors.Default;
				}
			}
			catch (Exception ex)
			{
				this.OnShowInfo(ex.Message);
				this.Cursor = Cursors.Default;
				this.btn_Update.Enabled = true;
				this.btn_exit.Enabled = true;
			}
		}

		private void PCToDeviceEx_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.m_isStopEx)
			{
				SysLogServer.WriteLog("In PCToDeviceEx_FormClosing the m_isStopEx = True", true);
			}
			else
			{
				SysLogServer.WriteLog("In PCToDeviceEx_FormClosing the m_isStopEx = False", true);
			}
			this.m_isStopEx = true;
			if (this.m_thead != null)
			{
				e.Cancel = true;
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			this.btn_Update_Click(null, null);
		}

		private void ShowErrorInfo(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new EventHandler(this.ShowErrorInfo), sender, e);
				}
				else if (this.btn_show.Text == ShowMsgInfos.GetInfo("SShowInfo", "详细信息"))
				{
					this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
					this.txt_UpLoadInfo.Visible = true;
					base.Height = this.txt_UpLoadInfo.Top + this.txt_UpLoadInfo.Height + 38;
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

		private void PCToDeviceEx_Load(object sender, EventArgs e)
		{
			PCToDeviceEx.m_isShow = true;
		}

		private void PCToDeviceEx_FormClosed(object sender, FormClosedEventArgs e)
		{
			PCToDeviceEx.m_isShow = false;
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
			this.lb_progress = new LabelX();
			this.btn_exit = new ButtonX();
			this.btn_Update = new ButtonX();
			this.progressBarUp = new ProgressBar();
			this.txt_UpLoadInfo = new TextBox();
			this.gv = new DataGridViewX();
			this.Column_Inport = new DataGridViewTextBoxColumn();
			this.Column_Source = new DataGridViewTextBoxColumn();
			this.Column1 = new DataGridViewTextBoxColumn();
			this.Column2 = new DataGridViewTextBoxColumn();
			this.Column_RS485 = new DataGridViewTextBoxColumn();
			this.Column_Status = new DataGridViewTextBoxColumn();
			this.label1 = new Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.lb_progressAll = new LabelX();
			this.progress_all = new ProgressBar();
			this.btn_show = new ButtonX();
			this.lblProgressAll = new Label();
			this.lblProgress = new Label();
			((ISupportInitialize)this.gv).BeginInit();
			base.SuspendLayout();
			this.lb_progress.AutoSize = true;
			this.lb_progress.BackgroundStyle.Class = "";
			this.lb_progress.Location = new Point(12, 185);
			this.lb_progress.Name = "lb_progress";
			this.lb_progress.Size = new Size(81, 18);
			this.lb_progress.TabIndex = 27;
			this.lb_progress.Text = "当前设备进度";
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(432, 295);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(142, 23);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 24;
			this.btn_exit.Text = "返回";
			this.btn_exit.Click += this.btn_exit_Click;
			this.btn_Update.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Update.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Update.Location = new Point(275, 295);
			this.btn_Update.Name = "btn_Update";
			this.btn_Update.Size = new Size(142, 23);
			this.btn_Update.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Update.TabIndex = 23;
			this.btn_Update.Text = "同步";
			this.btn_Update.Click += this.btn_Update_Click;
			this.progressBarUp.Location = new Point(12, 209);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(563, 23);
			this.progressBarUp.TabIndex = 26;
			this.txt_UpLoadInfo.Location = new Point(12, 324);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(563, 139);
			this.txt_UpLoadInfo.TabIndex = 25;
			this.gv.AllowUserToAddRows = false;
			this.gv.AllowUserToDeleteRows = false;
			this.gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gv.Columns.AddRange(this.Column_Inport, this.Column_Source, this.Column1, this.Column2, this.Column_RS485, this.Column_Status);
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("SimSun", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.gv.DefaultCellStyle = dataGridViewCellStyle;
			this.gv.GridColor = Color.FromArgb(208, 215, 229);
			this.gv.Location = new Point(12, 19);
			this.gv.Name = "gv";
			this.gv.RowTemplate.Height = 23;
			this.gv.Size = new Size(564, 159);
			this.gv.TabIndex = 29;
			this.Column_Inport.HeaderText = "设备名称";
			this.Column_Inport.Name = "Column_Inport";
			this.Column_Inport.ReadOnly = true;
			this.Column_Source.FillWeight = 70f;
			this.Column_Source.HeaderText = "通信方式";
			this.Column_Source.Name = "Column_Source";
			this.Column_Source.Resizable = DataGridViewTriState.True;
			this.Column_Source.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.Column_Source.Width = 70;
			this.Column1.FillWeight = 95f;
			this.Column1.HeaderText = "IP地址";
			this.Column1.Name = "Column1";
			this.Column1.Width = 95;
			this.Column2.FillWeight = 70f;
			this.Column2.HeaderText = "串口号";
			this.Column2.Name = "Column2";
			this.Column2.Width = 70;
			this.Column_RS485.FillWeight = 90f;
			this.Column_RS485.HeaderText = "RS485地址";
			this.Column_RS485.Name = "Column_RS485";
			this.Column_RS485.Width = 90;
			this.Column_Status.FillWeight = 80f;
			this.Column_Status.HeaderText = "同步结果";
			this.Column_Status.Name = "Column_Status";
			this.Column_Status.Width = 95;
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = SystemColors.ControlText;
			this.label1.Location = new Point(10, 4);
			this.label1.Name = "label1";
			this.label1.Size = new Size(53, 12);
			this.label1.TabIndex = 28;
			this.label1.Text = "目标设备";
			this.timer1.Tick += this.timer1_Tick;
			this.lb_progressAll.AutoSize = true;
			this.lb_progressAll.BackgroundStyle.Class = "";
			this.lb_progressAll.Location = new Point(12, 238);
			this.lb_progressAll.Name = "lb_progressAll";
			this.lb_progressAll.Size = new Size(56, 18);
			this.lb_progressAll.TabIndex = 31;
			this.lb_progressAll.Text = "总体进度";
			this.progress_all.Location = new Point(12, 261);
			this.progress_all.Name = "progress_all";
			this.progress_all.Size = new Size(563, 23);
			this.progress_all.TabIndex = 30;
			this.btn_show.AccessibleRole = AccessibleRole.PushButton;
			this.btn_show.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_show.Location = new Point(12, 295);
			this.btn_show.Name = "btn_show";
			this.btn_show.Size = new Size(142, 23);
			this.btn_show.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_show.TabIndex = 32;
			this.btn_show.Text = "隐藏信息";
			this.btn_show.Click += this.btn_show_Click;
			this.lblProgressAll.AutoSize = true;
			this.lblProgressAll.BackColor = Color.Transparent;
			this.lblProgressAll.Location = new Point(285, 267);
			this.lblProgressAll.Name = "lblProgressAll";
			this.lblProgressAll.Size = new Size(17, 12);
			this.lblProgressAll.TabIndex = 37;
			this.lblProgressAll.Text = "0%";
			this.lblProgress.AutoSize = true;
			this.lblProgress.BackColor = Color.Transparent;
			this.lblProgress.Location = new Point(285, 215);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new Size(17, 12);
			this.lblProgress.TabIndex = 36;
			this.lblProgress.Text = "0%";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(588, 473);
			base.Controls.Add(this.lblProgressAll);
			base.Controls.Add(this.lblProgress);
			base.Controls.Add(this.btn_show);
			base.Controls.Add(this.lb_progressAll);
			base.Controls.Add(this.progress_all);
			base.Controls.Add(this.gv);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.lb_progress);
			base.Controls.Add(this.btn_exit);
			base.Controls.Add(this.btn_Update);
			base.Controls.Add(this.progressBarUp);
			base.Controls.Add(this.txt_UpLoadInfo);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PCToDeviceEx";
			this.Text = "同步变动数据到设备";
			base.FormClosing += this.PCToDeviceEx_FormClosing;
			base.FormClosed += this.PCToDeviceEx_FormClosed;
			base.Load += this.PCToDeviceEx_Load;
			((ISupportInitialize)this.gv).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
