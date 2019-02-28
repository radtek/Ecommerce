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
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.door
{
	public class FirstOpenEidt : Office2007Form
	{
		private int m_id = 0;

		private AccDoorBll mbll = new AccDoorBll(MainForm._ia);

		private Dictionary<int, AccDoor> mlist = new Dictionary<int, AccDoor>();

		private Dictionary<int, int> indexidlist = new Dictionary<int, int>();

		private Dictionary<int, int> timeindexidlist = new Dictionary<int, int>();

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_time;

		private ComboBox cmb_type;

		private Label lal_door;

		private ComboBox cmb_door;

		private Label label2;

		private Label label1;

		public event EventHandler RefreshDataEvent = null;

		public FirstOpenEidt(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.InitMachines();
				this.InitTimeseg();
				initLang.LocaleForm(this, base.Name);
				this.BindData();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void InitMachines()
		{
			try
			{
				this.mlist.Clear();
				this.cmb_door.Items.Clear();
				this.indexidlist.Clear();
				List<AccDoor> list = null;
				list = this.mbll.GetModelList("device_id in (select ID from Machines where DevSDKType is null or DevSDKType<>2)");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (!this.mlist.ContainsKey(list[i].id))
						{
							this.mlist.Add(list[i].id, list[i]);
							this.indexidlist.Add(this.cmb_door.Items.Count, list[i].id);
							this.cmb_door.Items.Add(list[i].door_name);
						}
					}
				}
				if (this.cmb_door.Items.Count > 0)
				{
					this.cmb_door.SelectedIndex = 0;
				}
				else if (this.m_id < 1)
				{
					this.cmb_door.Items.Add("-----");
					this.cmb_door.SelectedIndex = 0;
					this.indexidlist.Add(0, 0);
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDataEdit", "没有编辑的数据"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void InitTimeseg()
		{
			try
			{
				this.cmb_type.Items.Clear();
				this.timeindexidlist.Clear();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						this.timeindexidlist.Add(i, modelList[i].id);
						this.cmb_type.Items.Add(modelList[i].timeseg_name);
					}
				}
				if (this.cmb_type.Items.Count > 0)
				{
					this.cmb_type.SelectedIndex = 0;
				}
				else
				{
					this.cmb_type.Items.Add("-----");
					this.cmb_type.SelectedIndex = 0;
					this.timeindexidlist.Add(0, 0);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccFirstOpen accFirstOpen = null;
					AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
					accFirstOpen = accFirstOpenBll.GetModel(this.m_id);
					if (accFirstOpen != null)
					{
						if (this.mlist.ContainsKey(accFirstOpen.door_id))
						{
							this.cmb_door.Text = this.mlist[accFirstOpen.door_id].door_name;
						}
						else
						{
							AccDoor model = this.mbll.GetModel(accFirstOpen.door_id);
							if (model != null)
							{
								this.mlist.Add(model.id, model);
								this.cmb_door.Items.Add(model.door_name);
								this.cmb_door.Text = model.door_name;
								this.indexidlist.Add(this.cmb_door.Items.Count - 1, accFirstOpen.door_id);
							}
							else
							{
								this.cmb_door.Items.Add("-----");
								this.cmb_door.SelectedIndex = 0;
								this.indexidlist.Add(this.cmb_door.Items.Count - 1, 0);
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDataEdit", "没有编辑的数据"));
							}
						}
						int num = 0;
						while (true)
						{
							if (num < this.cmb_type.Items.Count)
							{
								if (!this.timeindexidlist.ContainsKey(num) || this.timeindexidlist[num] != accFirstOpen.timeseg_id)
								{
									num++;
									continue;
								}
								break;
							}
							return;
						}
						this.cmb_type.SelectedIndex = num;
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

		private bool BindModel(AccFirstOpen fopen)
		{
			if (this.check())
			{
				fopen.door_id = this.indexidlist[this.cmb_door.SelectedIndex];
				fopen.timeseg_id = this.timeindexidlist[this.cmb_type.SelectedIndex];
				return true;
			}
			return false;
		}

		private bool check()
		{
			if (this.cmb_door.SelectedIndex >= 0 && !string.IsNullOrEmpty(this.cmb_door.Text) && this.cmb_door.Text.IndexOf("----") == -1 && this.indexidlist.ContainsKey(this.cmb_door.SelectedIndex))
			{
				if (this.cmb_type.SelectedIndex >= 0 && !string.IsNullOrEmpty(this.cmb_type.Text) && this.cmb_type.Text.IndexOf("----") == -1 && this.timeindexidlist.ContainsKey(this.cmb_type.SelectedIndex))
				{
					return true;
				}
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectTomeZone", "请选择时段"));
				this.cmb_type.Focus();
				return false;
			}
			SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDevice", "请选择设备"));
			this.cmb_door.Focus();
			return false;
		}

		private bool CheckCF(AccFirstOpen fopen)
		{
			try
			{
				AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
				List<AccFirstOpen> modelList = accFirstOpenBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].id != this.m_id && modelList[i].timeseg_id == fopen.timeseg_id && modelList[i].door_id == fopen.door_id)
						{
							return false;
						}
					}
				}
				return true;
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
				AccFirstOpen accFirstOpen = null;
				AccFirstOpen accFirstOpen2 = new AccFirstOpen();
				AccFirstOpenBll accFirstOpenBll = new AccFirstOpenBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accFirstOpen = accFirstOpenBll.GetModel(this.m_id);
					if (accFirstOpen != null)
					{
						accFirstOpen2.door_id = accFirstOpen.door_id;
						accFirstOpen2.timeseg_id = accFirstOpen.timeseg_id;
					}
				}
				if (accFirstOpen == null)
				{
					accFirstOpen = new AccFirstOpen();
				}
				if (this.BindModel(accFirstOpen))
				{
					if (!this.CheckCF(accFirstOpen))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDoorAndTimeZoneRepeat", "当前用户或其他用户已添加过该门在当前时间段内的首卡常开设置"));
						return false;
					}
					if (this.m_id > 0)
					{
						if (accFirstOpen2.timeseg_id != accFirstOpen.timeseg_id || accFirstOpen2.door_id != accFirstOpen.door_id)
						{
							if (accFirstOpenBll.Update(accFirstOpen))
							{
								CommandServer.DelCmd(accFirstOpen2);
								List<ObjFirstCard> firstCardInfo = DeviceHelper.GetFirstCardInfo(accFirstOpen);
								CommandServer.AddCmd(accFirstOpen, firstCardInfo);
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
								return true;
							}
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
							goto end_IL_0001;
						}
						return true;
					}
					try
					{
						if (accFirstOpenBll.Add(accFirstOpen) > 0)
						{
							List<ObjFirstCard> firslist = new List<ObjFirstCard>();
							CommandServer.AddCmd(accFirstOpen, firslist);
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							return true;
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
				}
				end_IL_0001:;
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
			}
			return false;
		}

		private void btn_saveAndContinue_Click(object sender, EventArgs e)
		{
			if (this.check())
			{
				this.Save();
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				FrmShowUpdata.Instance.ShowEx();
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FirstOpenEidt));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_time = new Label();
			this.cmb_type = new ComboBox();
			this.lal_door = new Label();
			this.cmb_door = new ComboBox();
			this.label2 = new Label();
			this.label1 = new Label();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(268, 100);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(166, 100);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 2;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_time.Location = new Point(12, 59);
			this.lb_time.Name = "lb_time";
			this.lb_time.Size = new Size(153, 12);
			this.lb_time.TabIndex = 33;
			this.lb_time.Text = "门禁时间段";
			this.lb_time.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_type.FormattingEnabled = true;
			this.cmb_type.Location = new Point(178, 55);
			this.cmb_type.Name = "cmb_type";
			this.cmb_type.Size = new Size(154, 20);
			this.cmb_type.TabIndex = 1;
			this.lal_door.Location = new Point(12, 24);
			this.lal_door.Name = "lal_door";
			this.lal_door.Size = new Size(153, 12);
			this.lal_door.TabIndex = 31;
			this.lal_door.Text = "当前门";
			this.lal_door.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_door.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_door.FormattingEnabled = true;
			this.cmb_door.Location = new Point(178, 21);
			this.cmb_door.Name = "cmb_door";
			this.cmb_door.Size = new Size(154, 20);
			this.cmb_door.TabIndex = 0;
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(338, 24);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 49;
			this.label2.Text = "*";
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(338, 59);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 50;
			this.label1.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(361, 135);
			base.Controls.Add(this.cmb_type);
			base.Controls.Add(this.cmb_door);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_time);
			base.Controls.Add(this.lal_door);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FirstOpenEidt";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "首卡常开";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
