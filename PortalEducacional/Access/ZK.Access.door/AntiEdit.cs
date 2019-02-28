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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class AntiEdit : Office2007Form
	{
		private int m_id = 0;

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private Dictionary<int, int> indexidlist = new Dictionary<int, int>();

		private Machines machineOld;

		private AccDoor DoorOld;

		private DataTable dtInOutState;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_type;

		private ComboBox cmb_type;

		private Label lb_dev;

		private ComboBox cmb_dev;

		private Label label2;

		private Label label1;

		private Panel pnlMasterState;

		private ComboBox cbbMasterState;

		private Label label3;

		private Label lblMasterState;

		public event EventHandler RefreshDataEvent = null;

		public AntiEdit(int id)
		{
			this.InitializeComponent();
			this.pnlMasterState.Visible = false;
			base.Height = 161;
			this.m_id = id;
			try
			{
				this.InitInOutState();
				this.InitAntiType();
				this.InitMachines();
				this.InitCmbType();
				initLang.LocaleForm(this, base.Name);
				this.BindData();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void InitAntiType()
		{
			this.m_typeDic = initLang.GetAntiComboxInfo();
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				this.indexidlist.Clear();
				this.cmb_dev.Items.Clear();
				this.cmb_dev.Items.Add("-----");
				this.indexidlist.Add(0, 0);
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				List<Machines> list = null;
				list = machinesBll.GetModelList(" id not in(select device_id from acc_antiback ) ");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].door_count != 0 && !this.mlist.ContainsKey(list[i].ID))
						{
							this.indexidlist.Add(this.cmb_dev.Items.Count, list[i].ID);
							this.mlist.Add(list[i].ID, list[i]);
							this.cmb_dev.Items.Add(list[i].MachineAlias);
						}
					}
				}
				if (this.cmb_dev.Items.Count > 0)
				{
					this.cmb_dev.SelectedIndex = 0;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitCmbType()
		{
			this.cmb_dev_SelectedIndexChanged(this.cmb_dev, null);
		}

		private void InitInOutState()
		{
			this.dtInOutState = new DataTable();
			this.dtInOutState.Columns.Add("Value", typeof(int));
			this.dtInOutState.Columns.Add("Text", typeof(string));
			DataRow dataRow = this.dtInOutState.NewRow();
			dataRow["Value"] = -1;
			dataRow["Text"] = ShowMsgInfos.GetInfo("MasterStateNone", "无");
			this.dtInOutState.Rows.Add(dataRow);
			dataRow = this.dtInOutState.NewRow();
			dataRow["Value"] = 0;
			dataRow["Text"] = ShowMsgInfos.GetInfo("MasterStateOut", "出");
			this.dtInOutState.Rows.Add(dataRow);
			dataRow = this.dtInOutState.NewRow();
			dataRow["Value"] = 1;
			dataRow["Text"] = ShowMsgInfos.GetInfo("MasterStateIn", "入");
			this.dtInOutState.Rows.Add(dataRow);
			this.cbbMasterState.DataSource = this.dtInOutState;
			this.cbbMasterState.DisplayMember = "Text";
			this.cbbMasterState.ValueMember = "Value";
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccAntiback accAntiback = null;
					AccAntibackBll accAntibackBll = new AccAntibackBll(MainForm._ia);
					accAntiback = accAntibackBll.GetModel(this.m_id);
					if (accAntiback != null)
					{
						if (this.mlist.ContainsKey(accAntiback.device_id))
						{
							this.cmb_dev.Text = this.mlist[accAntiback.device_id].MachineAlias;
						}
						else
						{
							MachinesBll machinesBll = new MachinesBll(MainForm._ia);
							Machines model = machinesBll.GetModel(accAntiback.device_id);
							if (model != null)
							{
								this.mlist.Add(model.ID, model);
								this.cmb_dev.Items.Add(model.MachineAlias);
								this.indexidlist.Add(this.cmb_dev.Items.Count - 1, model.ID);
								this.cmb_dev.Text = model.MachineAlias;
							}
							else
							{
								this.cmb_dev.Items.Add("-----");
								this.cmb_dev.SelectedIndex = 0;
								this.indexidlist.Add(this.cmb_dev.Items.Count - 1, 0);
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDataEdit", "没有编辑的数据"));
							}
						}
						this.cmb_dev.Enabled = false;
						this.cmb_dev_SelectedIndexChanged(null, null);
						if (this.mlist.ContainsKey(accAntiback.device_id))
						{
							Machines machines = this.mlist[accAntiback.device_id];
							if (this.m_typeDic != null)
							{
								int num = machines.acpanel_type;
								if (machines.acpanel_type == 2 && machines.reader_count == 4)
								{
									num = 3;
								}
								if (machines.acpanel_type == 4 && machines.device_type == 10)
								{
									num = 5;
								}
								if (machines.DevSDKType == SDKType.StandaloneSDK)
								{
									num = 1000;
								}
								else if (machines.CompatOldFirmware == "0")
								{
									num = 1000;
								}
								if (this.m_typeDic.ContainsKey(num.ToString()))
								{
									Dictionary<string, string> dictionary = this.m_typeDic[num.ToString()];
									foreach (KeyValuePair<string, string> item in dictionary)
									{
										if (item.Key == accAntiback.AntibackType.ToString())
										{
											this.cmb_type.Text = item.Value;
										}
									}
								}
							}
						}
					}
				}
				else
				{
					this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool BindModel(AccAntiback anti)
		{
			try
			{
				if (this.check())
				{
					try
					{
						anti.device_id = this.indexidlist[this.cmb_dev.SelectedIndex];
						if (this.mlist.ContainsKey(anti.device_id))
						{
							Machines machines = this.mlist[anti.device_id];
							if (this.m_typeDic != null)
							{
								int num = machines.acpanel_type;
								if (machines.acpanel_type == 2 && machines.reader_count == 4)
								{
									num = 3;
								}
								if (machines.acpanel_type == 4 && machines.device_type == 10)
								{
									num = 5;
								}
								if (machines.DevSDKType == SDKType.StandaloneSDK)
								{
									num = 1000;
								}
								else if (machines.CompatOldFirmware == "0")
								{
									num = 1000;
								}
								if (this.m_typeDic.ContainsKey(num.ToString()))
								{
									Dictionary<string, string> dictionary = this.m_typeDic[num.ToString()];
									foreach (KeyValuePair<string, string> item in dictionary)
									{
										if (item.Value == this.cmb_type.Text)
										{
											anti.AntibackType = int.Parse(item.Key);
											return true;
										}
									}
								}
							}
						}
						return false;
					}
					catch
					{
						return false;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool check()
		{
			try
			{
				if (this.cmb_dev.SelectedIndex >= 0 && !string.IsNullOrEmpty(this.cmb_dev.Text) && this.cmb_dev.Text.IndexOf("----") == -1 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex))
				{
					if (this.cmb_type.SelectedIndex >= 0)
					{
						return true;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectType", "请选择规则"));
					this.cmb_type.Focus();
					return false;
				}
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDevice", "请选择设备"));
				this.cmb_dev.Focus();
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool Save()
		{
			try
			{
				AccAntiback accAntiback = null;
				AccAntibackBll accAntibackBll = new AccAntibackBll(MainForm._ia);
				int.TryParse((this.cbbMasterState.SelectedValue ?? "").ToString(), out int num);
				if (this.m_id > 0)
				{
					accAntiback = accAntibackBll.GetModel(this.m_id);
				}
				if (accAntiback == null)
				{
					accAntiback = new AccAntiback();
				}
				AccAntiback accAntiback2 = new AccAntiback();
				accAntiback2.AntibackType = accAntiback.AntibackType;
				accAntiback2.device_id = accAntiback.device_id;
				if (this.BindModel(accAntiback))
				{
					if (this.m_id > 0)
					{
						if (accAntiback.device_id == accAntiback2.device_id && accAntiback.AntibackType == accAntiback2.AntibackType)
						{
							if (this.DoorOld == null)
							{
								return true;
							}
							if (this.DoorOld.readerIOState == num)
							{
								return true;
							}
						}
						if (accAntibackBll.Update(accAntiback))
						{
							try
							{
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
							}
							catch
							{
							}
							CommandServer.SetAntiPassback(accAntiback.AntibackType, accAntiback.device_id);
							FrmShowUpdata.Instance.ShowEx();
							goto IL_021f;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						return false;
					}
					try
					{
						if (accAntibackBll.Add(accAntiback) > 0)
						{
							try
							{
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
							}
							catch
							{
							}
							CommandServer.SetAntiPassback(accAntiback.AntibackType, accAntiback.device_id);
							FrmShowUpdata.Instance.ShowEx();
							goto end_IL_0184;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						return false;
						end_IL_0184:;
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						return false;
					}
					goto IL_021f;
				}
				goto end_IL_0001;
				IL_021f:
				if (this.machineOld != null && (this.machineOld.DevSDKType == SDKType.StandaloneSDK || this.machineOld.CompatOldFirmware == "0") && this.DoorOld != null)
				{
					try
					{
						this.DoorOld.readerIOState = num;
						AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
						accDoorBll.Update(this.DoorOld);
						CommandServer.SetDoorParam(this.DoorOld);
					}
					catch (Exception ex)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveMasterStateFailed", "保存主机状态失败: ") + ex.Message);
						return false;
					}
				}
				end_IL_0001:;
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
				return false;
			}
			return true;
		}

		private void btn_saveAndContinue_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				this.cmb_dev.Items.RemoveAt(this.cmb_dev.SelectedIndex);
				if (this.cmb_dev.Items.Count > 0)
				{
					this.cmb_dev.SelectedIndex = 0;
				}
				else
				{
					base.Close();
				}
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void cmb_dev_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.cmb_dev.SelectedIndex >= 0 && !string.IsNullOrEmpty(this.cmb_dev.Text) && this.cmb_dev.Text.IndexOf("----") == -1 && this.indexidlist.ContainsKey(this.cmb_dev.SelectedIndex))
				{
					this.cmb_type.Items.Clear();
					try
					{
						int num = this.indexidlist[this.cmb_dev.SelectedIndex];
						if (this.mlist.ContainsKey(num))
						{
							Machines machines = this.mlist[num];
							if (this.m_typeDic != null)
							{
								int num2 = machines.acpanel_type;
								if (machines.acpanel_type == 2 && machines.reader_count == 4)
								{
									num2 = 3;
								}
								if (machines.acpanel_type == 4 && machines.device_type == 10)
								{
									num2 = 5;
								}
								if (machines.DevSDKType == SDKType.StandaloneSDK)
								{
									num2 = 1000;
									this.machineOld = machines;
									AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
									List<AccDoor> modelList = accDoorBll.GetModelList("device_id=" + num);
									if (modelList != null && modelList.Count > 0)
									{
										this.DoorOld = modelList[0];
										this.cbbMasterState.SelectedValue = this.DoorOld.readerIOState;
									}
								}
								else if (machines.CompatOldFirmware == "0")
								{
									num2 = 1000;
									this.machineOld = machines;
									AccDoorBll accDoorBll2 = new AccDoorBll(MainForm._ia);
									List<AccDoor> modelList2 = accDoorBll2.GetModelList("device_id=" + num);
									if (modelList2 != null && modelList2.Count > 0)
									{
										this.DoorOld = modelList2[0];
										this.cbbMasterState.SelectedValue = this.DoorOld.readerIOState;
									}
								}
								base.Height = ((num2 == 1000) ? 205 : 161);
								this.pnlMasterState.Visible = (num2 == 1000);
								if (this.m_typeDic.ContainsKey(num2.ToString()))
								{
									Dictionary<string, string> dictionary = this.m_typeDic[num2.ToString()];
									foreach (KeyValuePair<string, string> item in dictionary)
									{
										this.cmb_type.Items.Add(item.Value);
									}
								}
							}
						}
					}
					catch
					{
					}
				}
				else
				{
					this.cmb_type.Items.Clear();
					base.Height = 161;
					this.pnlMasterState.Visible = false;
				}
				if (this.cmb_type.Items.Count > 0)
				{
					this.cmb_type.SelectedIndex = 0;
				}
				else
				{
					this.cmb_type.Text = "";
				}
				initLang.ComboBoxAutoSize(this.cmb_type, this);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AntiEdit));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_type = new Label();
			this.cmb_type = new ComboBox();
			this.lb_dev = new Label();
			this.cmb_dev = new ComboBox();
			this.label2 = new Label();
			this.label1 = new Label();
			this.pnlMasterState = new Panel();
			this.cbbMasterState = new ComboBox();
			this.label3 = new Label();
			this.lblMasterState = new Label();
			this.pnlMasterState.SuspendLayout();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(266, 137);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(164, 137);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 2;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_type.Location = new Point(12, 56);
			this.lb_type.Name = "lb_type";
			this.lb_type.Size = new Size(125, 12);
			this.lb_type.TabIndex = 26;
			this.lb_type.Text = "反潜规则";
			this.lb_type.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_type.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cmb_type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_type.FormattingEnabled = true;
			this.cmb_type.Location = new Point(178, 51);
			this.cmb_type.Name = "cmb_type";
			this.cmb_type.Size = new Size(154, 20);
			this.cmb_type.TabIndex = 1;
			this.lb_dev.Location = new Point(12, 20);
			this.lb_dev.Name = "lb_dev";
			this.lb_dev.Size = new Size(125, 12);
			this.lb_dev.TabIndex = 24;
			this.lb_dev.Text = "设备";
			this.lb_dev.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_dev.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cmb_dev.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_dev.FormattingEnabled = true;
			this.cmb_dev.Location = new Point(178, 16);
			this.cmb_dev.Name = "cmb_dev";
			this.cmb_dev.Size = new Size(154, 20);
			this.cmb_dev.TabIndex = 0;
			this.cmb_dev.SelectedIndexChanged += this.cmb_dev_SelectedIndexChanged;
			this.label2.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(338, 21);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 49;
			this.label2.Text = "*";
			this.label1.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(338, 56);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 50;
			this.label1.Text = "*";
			this.pnlMasterState.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.pnlMasterState.Controls.Add(this.cbbMasterState);
			this.pnlMasterState.Controls.Add(this.label3);
			this.pnlMasterState.Controls.Add(this.lblMasterState);
			this.pnlMasterState.Location = new Point(-3, 77);
			this.pnlMasterState.Name = "pnlMasterState";
			this.pnlMasterState.Size = new Size(365, 45);
			this.pnlMasterState.TabIndex = 51;
			this.cbbMasterState.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cbbMasterState.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbbMasterState.FormattingEnabled = true;
			this.cbbMasterState.Location = new Point(181, 13);
			this.cbbMasterState.Name = "cbbMasterState";
			this.cbbMasterState.Size = new Size(154, 20);
			this.cbbMasterState.TabIndex = 51;
			this.label3.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label3.AutoSize = true;
			this.label3.BackColor = Color.Transparent;
			this.label3.ForeColor = Color.Red;
			this.label3.Location = new Point(341, 18);
			this.label3.Name = "label3";
			this.label3.Size = new Size(11, 12);
			this.label3.TabIndex = 53;
			this.label3.Text = "*";
			this.lblMasterState.Location = new Point(15, 18);
			this.lblMasterState.Name = "lblMasterState";
			this.lblMasterState.Size = new Size(125, 12);
			this.lblMasterState.TabIndex = 52;
			this.lblMasterState.Text = "本机状态";
			this.lblMasterState.TextAlign = ContentAlignment.MiddleLeft;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(360, 173);
			base.Controls.Add(this.pnlMasterState);
			base.Controls.Add(this.cmb_type);
			base.Controls.Add(this.cmb_dev);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_type);
			base.Controls.Add(this.lb_dev);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AntiEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			this.pnlMasterState.ResumeLayout(false);
			this.pnlMasterState.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
