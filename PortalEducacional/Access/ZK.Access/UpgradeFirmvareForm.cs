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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class UpgradeFirmvareForm : Office2007Form
	{
		public delegate void ShowInfo(string info);

		private List<int> m_deviceSelectedTable = null;

		private bool Updating = false;

		private Dictionary<int, Machines> dicId_Machine;

		private IContainer components = null;

		private TextBox txt_FirmwarePath;

		private Label blb_FirmwarePath;

		private OpenFileDialog odlg_File;

		private ButtonX buttonX1;

		private ButtonX btn_Update;

		private ButtonX btn_Browse;

		private Label lb_show;

		private TextBox txt_UpLoadInfo;

		public event EventHandler refreshDataEvent = null;

		public UpgradeFirmvareForm(List<int> machineSelectedInfo)
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.m_deviceSelectedTable = machineSelectedInfo;
			this.btn_Update.Enabled = false;
			this.LoadMachines();
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			base.Close();
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
					this.txt_UpLoadInfo.AppendText(UpLoadinfoStr + Environment.NewLine);
					this.Refresh();
				}
			}
		}

		private string readFirmwareVersion(string path)
		{
			StreamReader streamReader = new StreamReader(path);
			string text = "";
			string text2 = "";
			string result = "";
			while (text != null)
			{
				text = streamReader.ReadLine();
				if (text != null && !text.Equals(""))
				{
					string[] array = text.Split('=');
					if (array != null && array.Length >= 2)
					{
						text2 = array[0];
						result = array[1];
						if (text2.ToLower().IndexOf("firmwareversion") >= 0)
						{
							break;
						}
					}
				}
			}
			streamReader.Close();
			return result;
		}

		private void btn_Update_Click(object sender, EventArgs e)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < this.m_deviceSelectedTable.Count; i++)
			{
				if (this.dicId_Machine.ContainsKey(this.m_deviceSelectedTable[i]))
				{
					Machines machines = this.dicId_Machine[this.m_deviceSelectedTable[i]];
					SDKType devSDKType = machines.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						list2.Add(machines.ID);
					}
					else
					{
						list.Add(machines.ID);
					}
					if (list2.Count > 0 && list.Count > 0)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("DeviceProtocolError", "不能同时对两种通讯协议不同的设备升级固件"));
						return;
					}
				}
			}
			if (list2.Count > 0)
			{
				this.odlg_File.Filter = "Firmware file(*.cfg,*.bin)|*.cfg;*.bin";
			}
			this.SetButtonState(false);
			try
			{
				if (string.IsNullOrEmpty(this.txt_FirmwarePath.Text) || !File.Exists(this.txt_FirmwarePath.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSlectFirmwarePath", "请选择固件路径"));
					this.SetButtonState(true);
				}
				else
				{
					string fileName = Path.GetFileName(this.txt_FirmwarePath.Text);
					if (fileName.ToLower() != "emfw.cfg" && !fileName.ToLower().EndsWith(".bin"))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SWrongFileName", "目标文件名错误"));
						this.SetButtonState(true);
					}
					else
					{
						FileStream fileStream = File.OpenRead(this.txt_FirmwarePath.Text);
						byte[] array = SysFile.Read(fileStream);
						if (array == null || array.Length < 1000)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFirmwareFileFailed", "固件文件被损坏或者无效"));
							this.SetButtonState(true);
						}
						else
						{
							int num = array.Length;
							fileStream.Close();
							fileStream = null;
							array = null;
							Thread thread = new Thread(this.UpdateFirmwareToMachine);
							thread.Start(this.txt_FirmwarePath.Text);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ":" + ex.Message);
				this.Cursor = Cursors.Default;
			}
		}

		private void btn_Browse_Click(object sender, EventArgs e)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < this.m_deviceSelectedTable.Count; i++)
			{
				if (this.dicId_Machine.ContainsKey(this.m_deviceSelectedTable[i]))
				{
					Machines machines = this.dicId_Machine[this.m_deviceSelectedTable[i]];
					SDKType devSDKType = machines.DevSDKType;
					if (devSDKType == SDKType.StandaloneSDK)
					{
						list2.Add(machines.ID);
					}
					else
					{
						list.Add(machines.ID);
					}
				}
			}
			if (list2.Count > 0)
			{
				this.odlg_File.Filter = "Firmware file(*.cfg,*.bin)|*.cfg;*.bin";
			}
			if (this.odlg_File.ShowDialog() == DialogResult.OK)
			{
				this.txt_FirmwarePath.Text = this.odlg_File.FileName;
			}
		}

		private void txt_FirmwarePath_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_FirmwarePath.Text) && File.Exists(this.txt_FirmwarePath.Text))
			{
				this.btn_Update.Enabled = true;
			}
			else
			{
				this.btn_Update.Enabled = false;
			}
		}

		private void UpdateFirmwareToMachine(object objFileName)
		{
			int num = -1;
			string options = "";
			string fileName = "emfw.cfg";
			try
			{
				this.Updating = true;
				string text = objFileName as string;
				FileStream fileStream = File.OpenRead(text);
				byte[] array = SysFile.Read(fileStream);
				int bufferSize = array.Length;
				fileStream.Close();
				fileStream = null;
				if (this.m_deviceSelectedTable == null || this.m_deviceSelectedTable.Count <= 0)
				{
					this.UpdateFinished();
					this.Updating = false;
					return;
				}
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				for (int i = 0; i < this.m_deviceSelectedTable.Count; i++)
				{
					Machines model = machinesBll.GetModel(this.m_deviceSelectedTable[i]);
					if (text.ToLower().EndsWith(".cfg"))
					{
						model.FirmwareVersion = this.readFirmwareVersion(text);
					}
					if (model != null)
					{
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
						if (deviceServer != null)
						{
							if (!deviceServer.IsConnected)
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SConnectingDevice", "正在连接设备") + "(" + model.MachineAlias + ")");
								if (deviceServer.Connect(3000) < 0)
								{
									this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SConnectFailed", "设备连接失败") + "(" + model.MachineAlias + "):" + PullSDkErrorInfos.GetInfo(num));
									continue;
								}
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SConnectSuccess", "设备连接成功") + "(" + model.MachineAlias + ")");
							}
							this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SUploadingFirmvare", "升级固件，请稍候...") + "(" + model.MachineAlias + ")");
							try
							{
								num = ((deviceServer.DevInfo.DevSDKType != SDKType.StandaloneSDK) ? deviceServer.SetDeviceFileData(fileName, ref array[0], bufferSize, options) : ((!text.ToLower().EndsWith(".cfg")) ? deviceServer.STD_SendFile(text, 600) : deviceServer.UpdateFirmware(text)));
								if (num < 0)
								{
									if (deviceServer.IsConnected)
									{
										deviceServer.Disconnect();
									}
									this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + "(" + model.MachineAlias + "):" + PullSDkErrorInfos.GetInfo(num));
								}
								else
								{
									OperationLog.SaveOperationLog(model.MachineAlias + ShowMsgInfos.GetInfo("SUpgradeFirmvare", "升级固件"), 4, "device object");
									this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "，" + ShowMsgInfos.GetInfo("SRebootDevice", "正在重启设备") + "(" + model.MachineAlias + ")");
									deviceServer.RebootDevice();
									if (text.ToLower().EndsWith(".cfg"))
									{
										model.FirmwareVersion = this.readFirmwareVersion(text);
									}
									else if (text.ToLower().EndsWith(".bin") && model.DevSDKType == SDKType.StandaloneSDK)
									{
										deviceServer.Disconnect();
										num = deviceServer.Connect(3000);
										if (num >= 0)
										{
											num = deviceServer.STD_GetFirmwareVersion(out string firmwareVersion);
											if (num >= 0)
											{
												model.FirmwareVersion = firmwareVersion;
											}
										}
									}
									machinesBll.Update(model);
									if (deviceServer.IsConnected)
									{
										deviceServer.Disconnect();
									}
								}
							}
							catch (Exception ex)
							{
								this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + "(" + model.MachineAlias + "):" + ex.Message);
							}
						}
					}
				}
			}
			catch (Exception ex2)
			{
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ":" + ex2.Message);
			}
			this.UpdateFinished();
			this.Updating = false;
		}

		private void SetButtonState(bool state)
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.SetButtonState(state);
				});
			}
			else
			{
				this.btn_Update.Enabled = state;
				this.btn_Browse.Enabled = state;
				this.buttonX1.Enabled = state;
			}
		}

		private void UpdateFinished()
		{
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.UpdateFinished();
				});
			}
			else
			{
				if (this.refreshDataEvent != null)
				{
					this.refreshDataEvent(this, null);
				}
				this.SetButtonState(true);
			}
		}

		private void UpgradeFirmvareForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.Updating)
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("FirmvareUpdating", "正在升级固件"));
				e.Cancel = true;
			}
		}

		private void LoadMachines()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> modelList = machinesBll.GetModelList("");
			this.dicId_Machine = new Dictionary<int, Machines>();
			if (modelList != null)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!this.dicId_Machine.ContainsKey(modelList[i].ID))
					{
						this.dicId_Machine.Add(modelList[i].ID, modelList[i]);
					}
				}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UpgradeFirmvareForm));
			this.txt_FirmwarePath = new TextBox();
			this.blb_FirmwarePath = new Label();
			this.odlg_File = new OpenFileDialog();
			this.buttonX1 = new ButtonX();
			this.btn_Update = new ButtonX();
			this.btn_Browse = new ButtonX();
			this.lb_show = new Label();
			this.txt_UpLoadInfo = new TextBox();
			base.SuspendLayout();
			this.txt_FirmwarePath.BackColor = SystemColors.Window;
			this.txt_FirmwarePath.Location = new Point(12, 34);
			this.txt_FirmwarePath.Name = "txt_FirmwarePath";
			this.txt_FirmwarePath.ReadOnly = true;
			this.txt_FirmwarePath.Size = new Size(271, 20);
			this.txt_FirmwarePath.TabIndex = 0;
			this.txt_FirmwarePath.TextChanged += this.txt_FirmwarePath_TextChanged;
			this.blb_FirmwarePath.Location = new Point(12, 14);
			this.blb_FirmwarePath.Name = "blb_FirmwarePath";
			this.blb_FirmwarePath.Size = new Size(259, 13);
			this.blb_FirmwarePath.TabIndex = 19;
			this.blb_FirmwarePath.Text = "目标文件";
			this.blb_FirmwarePath.TextAlign = ContentAlignment.MiddleLeft;
			this.odlg_File.FileName = "emfw";
			this.odlg_File.Filter = "Firmware file(*.cfg)|*.cfg";
			this.buttonX1.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX1.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX1.Location = new Point(234, 187);
			this.buttonX1.Name = "buttonX1";
			this.buttonX1.Size = new Size(142, 25);
			this.buttonX1.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX1.TabIndex = 3;
			this.buttonX1.Text = "Retornar";
			this.buttonX1.Click += this.buttonX1_Click;
			this.btn_Update.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Update.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Update.Location = new Point(73, 187);
			this.btn_Update.Name = "btn_Update";
			this.btn_Update.Size = new Size(142, 25);
			this.btn_Update.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Update.TabIndex = 2;
			this.btn_Update.Text = "Atualizar";
			this.btn_Update.Click += this.btn_Update_Click;
			this.btn_Browse.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Browse.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Browse.Location = new Point(289, 34);
			this.btn_Browse.Name = "btn_Browse";
			this.btn_Browse.Size = new Size(87, 25);
			this.btn_Browse.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Browse.TabIndex = 1;
			this.btn_Browse.Text = "浏览";
			this.btn_Browse.Click += this.btn_Browse_Click;
			this.lb_show.Location = new Point(12, 64);
			this.lb_show.Name = "lb_show";
			this.lb_show.Size = new Size(255, 13);
			this.lb_show.TabIndex = 20;
			this.lb_show.Text = "升级提示";
			this.lb_show.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_UpLoadInfo.Location = new Point(12, 80);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(364, 87);
			this.txt_UpLoadInfo.TabIndex = 21;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(389, 225);
			base.Controls.Add(this.txt_UpLoadInfo);
			base.Controls.Add(this.lb_show);
			base.Controls.Add(this.btn_Browse);
			base.Controls.Add(this.buttonX1);
			base.Controls.Add(this.btn_Update);
			base.Controls.Add(this.txt_FirmwarePath);
			base.Controls.Add(this.blb_FirmwarePath);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UpgradeFirmvareForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "升级固件";
			base.FormClosing += this.UpgradeFirmvareForm_FormClosing;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
