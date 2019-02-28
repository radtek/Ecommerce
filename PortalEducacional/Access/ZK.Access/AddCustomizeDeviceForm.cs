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
	public class AddCustomizeDeviceForm : Office2007Form
	{
		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		public delegate void FinishHandle(bool isSuccess);

		private string m_ip = string.Empty;

		private Dictionary<int, PersonnelArea> areaDicList = new Dictionary<int, PersonnelArea>();

		public bool devHasAdd = false;

		private WaitForm m_waitForm = WaitForm.Instance;

		private DateTime m_finishDate = DateTime.Now;

		private Thread m_thread = null;

		private IContainer components = null;

		private Label label1;

		private Label label2;

		private Label label3;

		private TextBox txt_deviceName;

		private TextBox txt_password;

		private CheckBox chk_deleteData;

		private ButtonX btn_cancel;

		private Label label4;

		private Label lb_area;

		private ComboBox cmb_area;

		private Label lbl_syncTime;

		private CheckBox chk_syncTime;

		private Label label5;

		private ButtonX btn_Ok;

		private System.Windows.Forms.Timer time_close;

		public AddCustomizeDeviceForm(string IP)
		{
			this.InitializeComponent();
			try
			{
				this.devHasAdd = false;
				this.txt_deviceName.Text = IP;
				this.m_ip = IP;
				this.LoadAreaInfo();
				initLang.LocaleForm(this, base.Name);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void ShowInfos(string infoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.ShowInfos), infoStr);
				}
				else
				{
					this.m_waitForm.ShowInfos(infoStr);
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
					this.m_waitForm.ShowProgress(prg);
				}
			}
		}

		private void OnFinish(bool isSuccess)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new FinishHandle(this.OnFinish), isSuccess);
				}
				else
				{
					this.Cursor = Cursors.Default;
					this.time_close.Tag = isSuccess;
					this.m_waitForm.Stop();
					this.devHasAdd = isSuccess;
					this.m_finishDate = DateTime.Now;
					this.time_close.Enabled = true;
					this.btn_Ok.Enabled = true;
					this.btn_cancel.Enabled = true;
					this.Cursor = Cursors.Default;
				}
			}
		}

		private void LoadAreaInfo()
		{
			this.cmb_area.Items.Clear();
			this.areaDicList.Clear();
			this.cmb_area.Items.Add("-----");
			PersonnelAreaBll personnelAreaBll = new PersonnelAreaBll(MainForm._ia);
			List<PersonnelArea> modelList = personnelAreaBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					this.areaDicList.Add(i, modelList[i]);
					this.cmb_area.Items.Add(modelList[i].areaname);
				}
			}
			if (this.cmb_area.Items.Count > 0)
			{
				this.cmb_area.SelectedIndex = 0;
			}
		}

		private bool SyncDeviceTime(Machines model, DeviceServerBll devServerBll)
		{
			try
			{
				if (this.chk_syncTime.Checked)
				{
					int num = DeviceHelper.SyncDeviceTime(devServerBll);
					if (num < 0)
					{
						this.ShowInfos(ShowMsgInfos.GetInfo("SSyncDevTimeFailed", "同步时间失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				this.ShowInfos(ex.Message);
				return false;
			}
		}

		private bool DeleteDeviceData(Machines model, DeviceServerBll devServerBll)
		{
			try
			{
				if (this.chk_deleteData.Checked)
				{
					int num = 0;
					num = ((model.IsOnlyRFMachine != 0) ? DeviceHelper.DeleteDeviceData(devServerBll, false) : DeviceHelper.DeleteDeviceData(devServerBll, true));
					if (num < 0)
					{
						this.ShowInfos(ShowMsgInfos.GetInfo("SDelDevDataFailed", "删除设备数据失败") + ":" + PullSDkErrorInfos.GetInfo(num));
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				this.ShowInfos(ex.Message);
				return false;
			}
		}

		private bool Check()
		{
			try
			{
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				if (string.IsNullOrEmpty(this.txt_deviceName.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputDeviceName", "请输入设备名称!"));
					this.txt_deviceName.Focus();
					return false;
				}
				if (machinesBll.ExistsDevice(this.txt_deviceName.Text.Trim(), ""))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDeviceNameRepeat", "设备名称重复!"));
					this.txt_deviceName.Focus();
					return false;
				}
				if (this.cmb_area.SelectedIndex < 1 || string.IsNullOrEmpty(this.cmb_area.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeviceArea", "请选择设备区域"));
					this.cmb_area.Focus();
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool BindModel(Machines model)
		{
			try
			{
				model.MachineAlias = this.txt_deviceName.Text;
				model.CommPassword = this.txt_password.Text;
				model.IP = this.m_ip;
				model.ConnectType = 1;
				model.Port = 4370;
				if (this.areaDicList.ContainsKey(this.cmb_area.SelectedIndex - 1))
				{
					model.area_id = this.areaDicList[this.cmb_area.SelectedIndex - 1].id;
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			this.devHasAdd = false;
			this.Cursor = Cursors.WaitCursor;
			try
			{
				if (this.Check())
				{
					Machines machines = new Machines();
					this.BindModel(machines);
					this.time_close.Enabled = false;
					this.btn_Ok.Enabled = false;
					this.btn_cancel.Enabled = false;
					this.Cursor = Cursors.WaitCursor;
					this.m_waitForm.ShowEx();
					if (this.m_thread == null)
					{
						this.m_thread = new Thread(this.Start);
						this.m_thread.Start(machines);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.Cursor = Cursors.Default;
		}

		private bool Add(Machines model)
		{
			try
			{
				int id = 1002;
				DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
				if (deviceServer != null)
				{
					this.ShowProgress(10);
					this.ShowInfos(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + model.MachineAlias + ")");
					if (deviceServer.IsConnected)
					{
						deviceServer.Disconnect();
					}
					id = deviceServer.Connect(3000);
					if (id >= 0)
					{
						this.ShowProgress(20);
						this.ShowInfos(ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功") + "(" + model.MachineAlias + ")");
						id = DeviceHelper.GetDeviceParams(model);
						if (id >= 0)
						{
							this.ShowProgress(30);
							this.SyncDeviceTime(model, deviceServer);
							this.ShowProgress(40);
							this.DeleteDeviceData(model, deviceServer);
							this.ShowProgress(60);
							MachinesBll machinesBll = new MachinesBll(MainForm._ia);
							if (machinesBll.Add(model) > 0)
							{
								model.ID = machinesBll.GetMaxId() - 1;
								deviceServer.DevInfo.ID = model.ID;
								this.ShowProgress(70);
								this.ShowInfos(DeviceHelper.SaveDoorInfo(model, true));
								this.ShowProgress(95);
								this.devHasAdd = true;
								return true;
							}
							this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "添加设备失败"));
						}
						else
						{
							this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "添加设备失败") + ":" + PullSDkErrorInfos.GetInfo(id));
						}
					}
					else
					{
						this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "添加设备失败") + ":" + PullSDkErrorInfos.GetInfo(id));
					}
				}
				else
				{
					this.ShowInfos(ShowMsgInfos.GetInfo("SAddDeviceFailed", "添加设备失败") + ":" + PullSDkErrorInfos.GetInfo(id));
				}
				return false;
			}
			catch (Exception ex)
			{
				this.ShowInfos(ex.Message);
				return false;
			}
		}

		private void Start(object obj)
		{
			if (obj != null)
			{
				Machines machines = obj as Machines;
				if (machines != null)
				{
					if (this.Add(machines))
					{
						this.OnFinish(true);
					}
					else
					{
						this.OnFinish(false);
					}
				}
				else
				{
					this.OnFinish(false);
				}
			}
			else
			{
				this.OnFinish(false);
			}
			this.m_thread = null;
		}

		private void txt_password_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e);
		}

		private void time_close_Tick(object sender, EventArgs e)
		{
			if (this.m_thread == null)
			{
				if (this.time_close.Tag != null && this.time_close.Tag.ToString().ToLower() == "true")
				{
					this.time_close.Enabled = false;
					this.m_waitForm.HideEx(false);
					base.DialogResult = DialogResult.OK;
					base.Close();
				}
				else if (DateTime.Now.AddSeconds(-10.0) > this.m_finishDate)
				{
					this.time_close.Enabled = false;
					this.m_waitForm.HideEx(false);
				}
			}
		}

		private void AddCustomizeDeviceForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.m_thread != null)
			{
				e.Cancel = true;
			}
		}

		private void AddCustomizeDeviceForm_Click(object sender, EventArgs e)
		{
			if (this.m_thread == null)
			{
				this.m_waitForm.HideEx(false);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddCustomizeDeviceForm));
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.txt_deviceName = new TextBox();
			this.txt_password = new TextBox();
			this.chk_deleteData = new CheckBox();
			this.btn_cancel = new ButtonX();
			this.label4 = new Label();
			this.lb_area = new Label();
			this.cmb_area = new ComboBox();
			this.lbl_syncTime = new Label();
			this.chk_syncTime = new CheckBox();
			this.label5 = new Label();
			this.btn_Ok = new ButtonX();
			this.time_close = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			this.label1.Location = new Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new Size(169, 12);
			this.label1.TabIndex = 10;
			this.label1.Text = "设备名称";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.Location = new Point(12, 42);
			this.label2.Name = "label2";
			this.label2.Size = new Size(169, 16);
			this.label2.TabIndex = 11;
			this.label2.Text = "通讯密码";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.label3.Location = new Point(12, 104);
			this.label3.Name = "label3";
			this.label3.Size = new Size(298, 12);
			this.label3.TabIndex = 13;
			this.label3.Text = "新增时删除设备中数据";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_deviceName.Location = new Point(208, 12);
			this.txt_deviceName.Name = "txt_deviceName";
			this.txt_deviceName.Size = new Size(127, 21);
			this.txt_deviceName.TabIndex = 0;
			this.txt_password.Location = new Point(208, 41);
			this.txt_password.Name = "txt_password";
			this.txt_password.PasswordChar = '*';
			this.txt_password.Size = new Size(127, 21);
			this.txt_password.TabIndex = 1;
			this.txt_password.KeyPress += this.txt_password_KeyPress;
			this.chk_deleteData.AutoSize = true;
			this.chk_deleteData.Checked = true;
			this.chk_deleteData.CheckState = CheckState.Checked;
			this.chk_deleteData.Location = new Point(320, 104);
			this.chk_deleteData.Name = "chk_deleteData";
			this.chk_deleteData.Size = new Size(15, 14);
			this.chk_deleteData.TabIndex = 3;
			this.chk_deleteData.UseVisualStyleBackColor = true;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(268, 179);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 6;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(339, 16);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 24;
			this.label4.Text = "*";
			this.lb_area.Location = new Point(12, 72);
			this.lb_area.Name = "lb_area";
			this.lb_area.Size = new Size(169, 16);
			this.lb_area.TabIndex = 12;
			this.lb_area.Text = "所属区域";
			this.lb_area.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_area.FormattingEnabled = true;
			this.cmb_area.Location = new Point(208, 72);
			this.cmb_area.Name = "cmb_area";
			this.cmb_area.Size = new Size(127, 20);
			this.cmb_area.TabIndex = 2;
			this.lbl_syncTime.Location = new Point(12, 131);
			this.lbl_syncTime.Name = "lbl_syncTime";
			this.lbl_syncTime.Size = new Size(298, 12);
			this.lbl_syncTime.TabIndex = 14;
			this.lbl_syncTime.Text = "连接时同步设备时间";
			this.lbl_syncTime.TextAlign = ContentAlignment.MiddleLeft;
			this.chk_syncTime.AutoSize = true;
			this.chk_syncTime.Checked = true;
			this.chk_syncTime.CheckState = CheckState.Checked;
			this.chk_syncTime.Location = new Point(320, 130);
			this.chk_syncTime.Name = "chk_syncTime";
			this.chk_syncTime.Size = new Size(15, 14);
			this.chk_syncTime.TabIndex = 4;
			this.chk_syncTime.UseVisualStyleBackColor = true;
			this.label5.AutoSize = true;
			this.label5.ForeColor = Color.Red;
			this.label5.Location = new Point(339, 77);
			this.label5.Name = "label5";
			this.label5.Size = new Size(11, 12);
			this.label5.TabIndex = 30;
			this.label5.Text = "*";
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(163, 179);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 5;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.time_close.Interval = 1000;
			this.time_close.Tick += this.time_close_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(362, 214);
			base.Controls.Add(this.chk_syncTime);
			base.Controls.Add(this.chk_deleteData);
			base.Controls.Add(this.cmb_area);
			base.Controls.Add(this.txt_password);
			base.Controls.Add(this.txt_deviceName);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.lbl_syncTime);
			base.Controls.Add(this.lb_area);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddCustomizeDeviceForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "添加设备";
			base.FormClosing += this.AddCustomizeDeviceForm_FormClosing;
			base.Click += this.AddCustomizeDeviceForm_Click;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
