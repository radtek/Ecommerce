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
using ZK.Utils;

namespace ZK.Access.door
{
	public class InterlockEdit : Office2007Form
	{
		private int m_id = 0;

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private Dictionary<int, int> indexidlist = new Dictionary<int, int>();

		private IContainer components = null;

		private ComboBox cmb_dev;

		private Label lb_dev;

		private Label lb_type;

		private ComboBox cmb_type;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label label2;

		private Label label1;

		public event EventHandler RefreshDataEvent = null;

		public InterlockEdit(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.InitInterlockType();
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

		private void InitInterlockType()
		{
			this.m_typeDic = initLang.GetInterlockComboxInfo();
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
				List<Machines> list = null;
				list = this.mbll.GetModelList(" id not in(select device_id from acc_interlock ) and door_count<>1 ");
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if ((list[i].aux_in_count != 0 || list[i].aux_out_count != 0) && list[i].door_count != 0 && !this.mlist.ContainsKey(list[i].ID))
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

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccInterlock accInterlock = null;
					AccInterlockBll accInterlockBll = new AccInterlockBll(MainForm._ia);
					accInterlock = accInterlockBll.GetModel(this.m_id);
					if (accInterlock != null)
					{
						if (this.mlist.ContainsKey(accInterlock.device_id))
						{
							this.cmb_dev.Text = this.mlist[accInterlock.device_id].MachineAlias;
						}
						else
						{
							Machines model = this.mbll.GetModel(accInterlock.device_id);
							if (model != null)
							{
								this.mlist.Add(model.ID, model);
								this.cmb_dev.Items.Add(model.MachineAlias);
								this.cmb_dev.Text = model.MachineAlias;
								this.indexidlist.Add(this.cmb_dev.Items.Count - 1, model.ID);
							}
							else
							{
								this.cmb_dev.Items.Add("-----");
								this.cmb_dev.SelectedIndex = 0;
								this.indexidlist.Add(this.cmb_dev.Items.Count - 1, 0);
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoDataEdit", "没有编辑的数据"));
							}
						}
						this.cmb_dev_SelectedIndexChanged(null, null);
						this.cmb_dev.Enabled = false;
						if (this.mlist.ContainsKey(accInterlock.device_id))
						{
							Machines machines = this.mlist[accInterlock.device_id];
							if (this.m_typeDic != null)
							{
								Dictionary<string, Dictionary<string, string>> typeDic = this.m_typeDic;
								int num = machines.acpanel_type;
								if (typeDic.ContainsKey(num.ToString()))
								{
									Dictionary<string, Dictionary<string, string>> typeDic2 = this.m_typeDic;
									num = machines.acpanel_type;
									Dictionary<string, string> dictionary = typeDic2[num.ToString()];
									foreach (KeyValuePair<string, string> item in dictionary)
									{
										string key = item.Key;
										num = accInterlock.InterlockType;
										if (key == num.ToString())
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

		private bool BindModel(AccInterlock interlock)
		{
			try
			{
				if (this.check())
				{
					interlock.device_id = this.indexidlist[this.cmb_dev.SelectedIndex];
					if (this.mlist.ContainsKey(interlock.device_id))
					{
						Machines machines = this.mlist[interlock.device_id];
						if (this.m_typeDic != null)
						{
							Dictionary<string, Dictionary<string, string>> typeDic = this.m_typeDic;
							int acpanel_type = machines.acpanel_type;
							if (typeDic.ContainsKey(acpanel_type.ToString()))
							{
								Dictionary<string, Dictionary<string, string>> typeDic2 = this.m_typeDic;
								acpanel_type = machines.acpanel_type;
								Dictionary<string, string> dictionary = typeDic2[acpanel_type.ToString()];
								foreach (KeyValuePair<string, string> item in dictionary)
								{
									if (item.Value == this.cmb_type.Text)
									{
										interlock.InterlockType = int.Parse(item.Key);
										break;
									}
								}
							}
						}
					}
					return true;
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
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectType", "请选择类型"));
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
				AccInterlock accInterlock = null;
				AccInterlockBll accInterlockBll = new AccInterlockBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accInterlock = accInterlockBll.GetModel(this.m_id);
				}
				if (accInterlock == null)
				{
					accInterlock = new AccInterlock();
				}
				AccInterlock accInterlock2 = new AccInterlock();
				accInterlock2.device_id = accInterlock.device_id;
				accInterlock2.InterlockType = accInterlock.InterlockType;
				if (this.BindModel(accInterlock))
				{
					if (this.m_id > 0)
					{
						if (accInterlock.device_id == accInterlock2.device_id && accInterlock.InterlockType == accInterlock2.InterlockType)
						{
							return true;
						}
						if (accInterlockBll.Update(accInterlock))
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
							CommandServer.SetInterLock(accInterlock.InterlockType, accInterlock.device_id);
							Application.DoEvents();
							FrmShowUpdata.Instance.ShowEx();
							return true;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
					else
					{
						try
						{
							if (accInterlockBll.Add(accInterlock) > 0)
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
								CommandServer.SetInterLock(accInterlock.InterlockType, accInterlock.device_id);
								Application.DoEvents();
								FrmShowUpdata.Instance.ShowEx();
								return true;
							}
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						}
						catch
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						}
					}
				}
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
			}
			return false;
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
					this.cmb_type.Text = "";
					try
					{
						int key = this.indexidlist[this.cmb_dev.SelectedIndex];
						if (this.mlist.ContainsKey(key))
						{
							Machines machines = this.mlist[key];
							if (this.m_typeDic != null)
							{
								Dictionary<string, Dictionary<string, string>> typeDic = this.m_typeDic;
								int acpanel_type = machines.acpanel_type;
								if (typeDic.ContainsKey(acpanel_type.ToString()))
								{
									Dictionary<string, Dictionary<string, string>> typeDic2 = this.m_typeDic;
									acpanel_type = machines.acpanel_type;
									Dictionary<string, string> dictionary = typeDic2[acpanel_type.ToString()];
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
					this.cmb_type.Text = "";
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(InterlockEdit));
			this.cmb_dev = new ComboBox();
			this.lb_dev = new Label();
			this.lb_type = new Label();
			this.cmb_type = new ComboBox();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.label2 = new Label();
			this.label1 = new Label();
			base.SuspendLayout();
			this.cmb_dev.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cmb_dev.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_dev.FormattingEnabled = true;
			this.cmb_dev.Location = new Point(174, 16);
			this.cmb_dev.Name = "cmb_dev";
			this.cmb_dev.Size = new Size(158, 20);
			this.cmb_dev.TabIndex = 0;
			this.cmb_dev.SelectedIndexChanged += this.cmb_dev_SelectedIndexChanged;
			this.lb_dev.Location = new Point(12, 20);
			this.lb_dev.Name = "lb_dev";
			this.lb_dev.Size = new Size(103, 12);
			this.lb_dev.TabIndex = 12;
			this.lb_dev.Text = "设备";
			this.lb_dev.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_type.Location = new Point(12, 54);
			this.lb_type.Name = "lb_type";
			this.lb_type.Size = new Size(103, 12);
			this.lb_type.TabIndex = 14;
			this.lb_type.Text = "互锁规则";
			this.lb_type.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_type.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cmb_type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_type.FormattingEnabled = true;
			this.cmb_type.Location = new Point(174, 51);
			this.cmb_type.Name = "cmb_type";
			this.cmb_type.Size = new Size(158, 20);
			this.cmb_type.TabIndex = 1;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(266, 92);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(168, 92);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 2;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.label2.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(338, 20);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 50;
			this.label2.Text = "*";
			this.label1.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label1.AutoSize = true;
			this.label1.BackColor = Color.Transparent;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(338, 54);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 51;
			this.label1.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(359, 128);
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
			base.Name = "InterlockEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
