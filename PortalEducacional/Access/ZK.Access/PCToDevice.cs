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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class PCToDevice : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowInfoEx(int index, string info);

		public delegate void ShowProgressHandle(int currProgress);

		private DeviceHelperEx helper = new DeviceHelperEx();

		private List<int> m_deviceSelectedTable = null;

		private Thread m_thead = null;

		private bool m_iseable = true;

		private bool m_isStop = false;

		private IContainer components = null;

		private ButtonX btn_Update;

		private ProgressBar progressBarUp;

		private TextBox txt_UpLoadInfo;

		private ButtonX btn_exit;

		private LabelX lb_progress;

		private ProgressBar progress_all;

		private LabelX lb_progressAll;

		private Label label1;

		private DataGridViewX gv;

		private System.Windows.Forms.Timer timer1;

		private ButtonX btn_show;

		private DataGridViewTextBoxColumn Column_Inport;

		private DataGridViewTextBoxColumn Column_Source;

		private DataGridViewTextBoxColumn Column1;

		private DataGridViewTextBoxColumn Column2;

		private DataGridViewTextBoxColumn Column_RS485;

		private DataGridViewTextBoxColumn column_Status;

		private Label lblProgress;

		private Label lblProgressAll;

		public PCToDevice(List<int> machineSelectedInfo)
		{
			this.InitializeComponent();
			try
			{
				this.m_deviceSelectedTable = machineSelectedInfo;
				this.LoadDev();
				initLang.LocaleForm(this, base.Name);
				this.helper.ShowInfoEvent += this.ShowUpLoadInfo;
				this.helper.ShowProgressEvent += this.ShowProgress;
				this.btn_show.Text = ShowMsgInfos.GetInfo("SHideInfo", "隐藏信息");
				this.btn_show_Click(null, null);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadDev()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			this.ShowProgressAll(0);
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
				List<Machines> modelList = machinesBll.GetModelList("");
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

		private void ShowUpLoadInfo(string UpLoadinfoStr)
		{
			if (base.Visible && !base.IsDisposed && !string.IsNullOrEmpty(UpLoadinfoStr))
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.ShowUpLoadInfo), UpLoadinfoStr);
				}
				else
				{
					this.txt_UpLoadInfo.AppendText(DateTime.Now.ToString() + " " + UpLoadinfoStr.TrimEnd("\r\n".ToCharArray()) + Environment.NewLine);
					this.Refresh();
				}
			}
		}

		private void ShowUpLoadNullInfo(string UpLoadinfoStr)
		{
			if (base.Visible && !base.IsDisposed && !string.IsNullOrEmpty(UpLoadinfoStr))
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.ShowUpLoadNullInfo), UpLoadinfoStr);
				}
				else
				{
					this.txt_UpLoadInfo.AppendText(" " + UpLoadinfoStr.TrimEnd("\r\n".ToCharArray()) + Environment.NewLine);
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
					this.BtnEable(true);
					this.Cursor = Cursors.Default;
					if (!this.m_isStop)
					{
						this.Refresh();
					}
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

		private void StartUpdate()
		{
			try
			{
				if (this.m_deviceSelectedTable != null && this.m_deviceSelectedTable.Count > 0)
				{
					Application.DoEvents();
					FrmShowUpdata.Instance.sync_Info_PcToDeving = false;
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
					this.ShowProgressAll(0);
					for (int i = 0; i < this.m_deviceSelectedTable.Count; i++)
					{
						Application.DoEvents();
						this.ShowProgressAll(100 * i / this.m_deviceSelectedTable.Count);
						this.ShowProgress(0);
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
								if (deviceServer.IsConnected)
								{
									deviceServer.Disconnect();
								}
								if (!deviceServer.IsConnected)
								{
									this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + model.MachineAlias + ")");
									int num = deviceServer.Connect(3000);
									if (num < 0)
									{
										this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + "(" + model.MachineAlias + "):" + PullSDkErrorInfos.GetInfo(num));
										this.ShowProgress(100);
										this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsFail", "失败"));
										this.ShowErrorInfo(this, null);
										continue;
									}
									this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功") + "(" + model.MachineAlias + ")");
								}
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadingData", "正在从PC同步信息到设备，请稍候...") + "(" + model.MachineAlias + ")");
								bool flag = true;
								if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
								{
									flag = DeviceHelper.UpLoadDataToDevice(model, deviceServer, this.ShowUpLoadInfo, this.ShowProgress, this);
									if (flag)
									{
										this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
									}
									else
									{
										this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsFail", "失败"));
										this.ShowErrorInfo(this, null);
									}
								}
								else
								{
									DeviceHelper.PcToDevice(model, this.ShowUpLoadInfo, this.ShowProgress);
									this.gvShowInfo(i, ShowMsgInfos.GetInfo("SIsSuccess", "成功"));
								}
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadDataSuccess", "从PC同步信息到设备完成") + "(" + model.MachineAlias + ")");
								this.ShowProgress(100);
								if (flag)
								{
									int num = DeviceHelper.UpdateDataCount(model, null);
									if (num >= 0)
									{
										machinesBll.Update(model);
									}
									if (deviceServer.DevInfo.DevSDKType == SDKType.StandaloneSDK)
									{
										deviceServer.RebootDevice();
									}
								}
							}
							if (deviceServer.IsConnected)
							{
								deviceServer.Disconnect();
							}
						}
						this.ShowProgress(100);
						this.ShowUpLoadNullInfo(" ");
					}
					this.ShowProgressAll(100);
					FrmShowUpdata.Instance.sync_Info_PcToDeving = true;
				}
				else
				{
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("NoDev", "没有设备"));
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
			}
			this.OnFinish(this, null);
			this.m_thead = null;
		}

		private void BtnEable(bool iseable)
		{
			this.btn_Update.Enabled = iseable;
			this.btn_exit.Enabled = iseable;
			this.m_iseable = iseable;
		}

		private void btn_Update_Click(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			try
			{
				if (this.m_thead == null)
				{
					this.BtnEable(false);
					this.m_isStop = false;
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
				this.ShowUpLoadInfo(ex.Message);
				this.Cursor = Cursors.Default;
				this.btn_Update.Enabled = true;
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			this.btn_Update_Click(null, null);
		}

		[Obsolete]
		private void SetUserACPrivilege(DeviceServerBll devServerBll, int Option)
		{
			try
			{
				if (!this.m_isStop)
				{
					this.ShowProgress(1);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadUserAuthorizeInfo", "正在从PC同步人员门禁权限设置信息到设备..."));
					this.helper.SetUserAuthorizeInfo(devServerBll, Option);
					this.ShowProgress(60);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadNormalOpenInfo", "正在从PC同步首卡开门设置信息到设备..."));
					this.helper.SetNormalOpenInfo(devServerBll);
					this.ShowProgress(65);
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
			}
		}

		private void SetACOptions(DeviceServerBll devServerBll)
		{
			try
			{
				if (!this.m_isStop && devServerBll != null)
				{
					this.ShowProgress(55);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadTZInfo", "正在从PC同步时间段信息到设备..."));
					this.helper.SetTZInfo(devServerBll);
					this.ShowProgress(60);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadHolidayInfo", "正在从PC同步节假日信息到设备..."));
					this.helper.SetHolidayInfo(devServerBll);
					this.ShowProgress(65);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadInterLockInfo", "正在从PC同步互锁设置信息到设备..."));
					this.helper.SetInterLockInfo(devServerBll);
					this.ShowProgress(70);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadAntiPassbackInfo", "正在从PC同步反潜设置信息到设备..."));
					this.helper.SetAntiPassbackInfo(devServerBll);
					this.ShowProgress(75);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadInOutFunInfo", "正在从PC同步联动设置信息到设备..."));
					this.helper.SetInOutFunInfo(devServerBll);
					this.ShowProgress(80);
					this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadMultimCardInfo", "正在从PC同步多卡开门设置信息到设备..."));
					this.helper.SetMultimCardInfo(devServerBll);
					this.ShowProgress(85);
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
			}
		}

		private void grd_mainView_Click(object sender, EventArgs e)
		{
			if (this.m_iseable)
			{
				DevExpressHelper.ClickGridCheckBox(sender, "check");
			}
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

		private void btn_exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void PCToDevice_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.m_isStop = true;
			if (this.m_thead != null)
			{
				e.Cancel = true;
			}
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
				base.Height = this.txt_UpLoadInfo.Top + this.txt_UpLoadInfo.Height + 40;
			}
			else
			{
				this.btn_show.Text = ShowMsgInfos.GetInfo("SShowInfo", "详细信息");
				this.txt_UpLoadInfo.Visible = false;
				base.Height = this.txt_UpLoadInfo.Top + 35;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PCToDevice));
			this.btn_Update = new ButtonX();
			this.progressBarUp = new ProgressBar();
			this.txt_UpLoadInfo = new TextBox();
			this.btn_exit = new ButtonX();
			this.gv = new DataGridViewX();
			this.Column_Inport = new DataGridViewTextBoxColumn();
			this.Column_Source = new DataGridViewTextBoxColumn();
			this.Column1 = new DataGridViewTextBoxColumn();
			this.Column2 = new DataGridViewTextBoxColumn();
			this.Column_RS485 = new DataGridViewTextBoxColumn();
			this.column_Status = new DataGridViewTextBoxColumn();
			this.label1 = new Label();
			this.lb_progressAll = new LabelX();
			this.progress_all = new ProgressBar();
			this.lb_progress = new LabelX();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.btn_show = new ButtonX();
			this.lblProgress = new Label();
			this.lblProgressAll = new Label();
			((ISupportInitialize)this.gv).BeginInit();
			base.SuspendLayout();
			this.btn_Update.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Update.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Update.Location = new Point(260, 321);
			this.btn_Update.Name = "btn_Update";
			this.btn_Update.Size = new Size(142, 23);
			this.btn_Update.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Update.TabIndex = 9;
			this.btn_Update.Text = "同步";
			this.btn_Update.Click += this.btn_Update_Click;
			this.progressBarUp.Location = new Point(12, 239);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(550, 23);
			this.progressBarUp.TabIndex = 21;
			this.txt_UpLoadInfo.Location = new Point(12, 350);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(550, 110);
			this.txt_UpLoadInfo.TabIndex = 11;
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(420, 321);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(142, 23);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 10;
			this.btn_exit.Text = "返回";
			this.btn_exit.Click += this.btn_exit_Click;
			this.gv.AllowUserToAddRows = false;
			this.gv.AllowUserToDeleteRows = false;
			this.gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gv.Columns.AddRange(this.Column_Inport, this.Column_Source, this.Column1, this.Column2, this.Column_RS485, this.column_Status);
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("SimSun", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.gv.DefaultCellStyle = dataGridViewCellStyle;
			this.gv.GridColor = Color.FromArgb(208, 215, 229);
			this.gv.Location = new Point(12, 22);
			this.gv.Name = "gv";
			this.gv.RowTemplate.Height = 23;
			this.gv.Size = new Size(550, 193);
			this.gv.TabIndex = 27;
			this.Column_Inport.HeaderText = "设备名称";
			this.Column_Inport.Name = "Column_Inport";
			this.Column_Inport.ReadOnly = true;
			this.Column_Source.FillWeight = 75f;
			this.Column_Source.HeaderText = "通信方式";
			this.Column_Source.Name = "Column_Source";
			this.Column_Source.Resizable = DataGridViewTriState.True;
			this.Column_Source.SortMode = DataGridViewColumnSortMode.NotSortable;
			this.Column_Source.Width = 75;
			this.Column1.FillWeight = 90f;
			this.Column1.HeaderText = "IP地址";
			this.Column1.Name = "Column1";
			this.Column1.Width = 90;
			this.Column2.FillWeight = 70f;
			this.Column2.HeaderText = "串口号";
			this.Column2.Name = "Column2";
			this.Column2.Width = 70;
			this.Column_RS485.FillWeight = 90f;
			this.Column_RS485.HeaderText = "RS485地址";
			this.Column_RS485.Name = "Column_RS485";
			this.Column_RS485.Width = 90;
			this.column_Status.FillWeight = 80f;
			this.column_Status.HeaderText = "同步状态";
			this.column_Status.Name = "column_Status";
			this.column_Status.Width = 80;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = SystemColors.ControlText;
			this.label1.Location = new Point(9, 6);
			this.label1.Name = "label1";
			this.label1.Size = new Size(64, 12);
			this.label1.TabIndex = 26;
			this.label1.Text = "目标设备";
			this.label1.Visible = false;
			this.lb_progressAll.BackgroundStyle.Class = "";
			this.lb_progressAll.Location = new Point(12, 270);
			this.lb_progressAll.Name = "lb_progressAll";
			this.lb_progressAll.Size = new Size(214, 14);
			this.lb_progressAll.TabIndex = 24;
			this.lb_progressAll.Text = "总体进度";
			this.progress_all.Location = new Point(12, 288);
			this.progress_all.Name = "progress_all";
			this.progress_all.Size = new Size(550, 23);
			this.progress_all.TabIndex = 23;
			this.lb_progress.BackgroundStyle.Class = "";
			this.lb_progress.Location = new Point(12, 221);
			this.lb_progress.Name = "lb_progress";
			this.lb_progress.Size = new Size(220, 14);
			this.lb_progress.TabIndex = 22;
			this.lb_progress.Text = "当前设备进度";
			this.timer1.Tick += this.timer1_Tick;
			this.btn_show.AccessibleRole = AccessibleRole.PushButton;
			this.btn_show.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_show.Location = new Point(12, 321);
			this.btn_show.Name = "btn_show";
			this.btn_show.Size = new Size(230, 23);
			this.btn_show.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_show.TabIndex = 33;
			this.btn_show.Text = "隐藏信息";
			this.btn_show.Click += this.btn_show_Click;
			this.lblProgress.AutoSize = true;
			this.lblProgress.BackColor = Color.Transparent;
			this.lblProgress.Location = new Point(274, 245);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new Size(17, 12);
			this.lblProgress.TabIndex = 34;
			this.lblProgress.Text = "0%";
			this.lblProgressAll.AutoSize = true;
			this.lblProgressAll.BackColor = Color.Transparent;
			this.lblProgressAll.Location = new Point(274, 294);
			this.lblProgressAll.Name = "lblProgressAll";
			this.lblProgressAll.Size = new Size(17, 12);
			this.lblProgressAll.TabIndex = 35;
			this.lblProgressAll.Text = "0%";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(574, 469);
			base.Controls.Add(this.lblProgressAll);
			base.Controls.Add(this.lblProgress);
			base.Controls.Add(this.btn_show);
			base.Controls.Add(this.gv);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.lb_progressAll);
			base.Controls.Add(this.progress_all);
			base.Controls.Add(this.lb_progress);
			base.Controls.Add(this.btn_exit);
			base.Controls.Add(this.btn_Update);
			base.Controls.Add(this.progressBarUp);
			base.Controls.Add(this.txt_UpLoadInfo);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PCToDevice";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "同步所有数据到设备";
			base.FormClosing += this.PCToDevice_FormClosing;
			((ISupportInitialize)this.gv).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
