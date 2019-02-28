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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DoorNormalFormSeting : Office2007Form
	{
		private int m_second = 0;

		private bool m_normal_open = false;

		private bool m_enable_normal_open = false;

		private bool m_disnable_normal_open = false;

		private int m_id = 0;

		private IContainer components = null;

		private RadioButton rdb_open;

		private TextBox txt_time;

		private Label lb_second;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		private GroupBox groupBox1;

		private RadioButton rdb_stop;

		private RadioButton rdb_normal;

		private RadioButton rdb_openNormal;

		public int Second
		{
			get
			{
				return this.m_second;
			}
			set
			{
				this.m_second = value;
			}
		}

		public bool normal_open
		{
			get
			{
				return this.m_normal_open;
			}
			set
			{
				this.m_normal_open = value;
			}
		}

		public bool enable_normal_open
		{
			get
			{
				return this.m_enable_normal_open;
			}
			set
			{
				this.m_enable_normal_open = value;
			}
		}

		public bool disnable_normal_open
		{
			get
			{
				return this.m_disnable_normal_open;
			}
			set
			{
				this.m_disnable_normal_open = value;
			}
		}

		public DoorNormalFormSeting(int id)
		{
			this.m_id = id;
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.Text = ShowMsgInfos.GetInfo("SEditex", "编辑");
			this.BindData();
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					AccDoor accDoor = null;
					AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
					accDoor = accDoorBll.GetModel(this.m_id);
					if (accDoor != null)
					{
						if (accDoor.open_door_delay > 0)
						{
							this.rdb_open.Enabled = true;
							this.txt_time.Text = accDoor.open_door_delay.ToString();
						}
						this.rdb_normal.Checked = accDoor.door_normal_open;
						this.rdb_openNormal.Checked = accDoor.enable_normal_open;
						this.rdb_stop.Checked = accDoor.disenable_normal_open;
						this.Second = accDoor.open_door_delay;
						this.normal_open = accDoor.door_normal_open;
						this.enable_normal_open = accDoor.enable_normal_open;
						this.disnable_normal_open = accDoor.disenable_normal_open;
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private bool check()
		{
			if (!string.IsNullOrEmpty(this.txt_time.Text) && this.rdb_open.Enabled && (int.Parse(this.txt_time.Text) > 254 || int.Parse(this.txt_time.Text) < 1))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SdoorStatusError", "门磁延时范围为1-254秒"));
				this.txt_time.Focus();
				return false;
			}
			return true;
		}

		private bool Save()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				AccDoor accDoor = null;
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				accDoor = accDoorBll.GetModel(this.m_id);
				if (accDoor == null)
				{
					this.Cursor = Cursors.Default;
					return true;
				}
				if (this.BindModel(accDoor))
				{
					if (accDoorBll.Update(accDoor))
					{
						this.Second = accDoor.open_door_delay;
						this.normal_open = accDoor.door_normal_open;
						this.enable_normal_open = accDoor.enable_normal_open;
						this.disnable_normal_open = accDoor.disenable_normal_open;
						return true;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("UpdatesError", "更新失败"));
				}
				this.Cursor = Cursors.Default;
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				this.Cursor = Cursors.Default;
				return false;
			}
		}

		private bool BindModel(AccDoor door)
		{
			try
			{
				door.open_door_delay = int.Parse(this.txt_time.Text);
				door.door_normal_open = this.rdb_normal.Checked;
				door.enable_normal_open = this.rdb_openNormal.Checked;
				door.disenable_normal_open = this.rdb_stop.Checked;
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
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
			this.rdb_open = new RadioButton();
			this.txt_time = new TextBox();
			this.lb_second = new Label();
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.groupBox1 = new GroupBox();
			this.rdb_stop = new RadioButton();
			this.rdb_normal = new RadioButton();
			this.rdb_openNormal = new RadioButton();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.rdb_open.AutoSize = true;
			this.rdb_open.Checked = true;
			this.rdb_open.Location = new Point(25, 23);
			this.rdb_open.Name = "rdb_open";
			this.rdb_open.Size = new Size(71, 16);
			this.rdb_open.TabIndex = 17;
			this.rdb_open.TabStop = true;
			this.rdb_open.Text = "开门时长";
			this.rdb_open.UseVisualStyleBackColor = true;
			this.txt_time.Location = new Point(188, 21);
			this.txt_time.Name = "txt_time";
			this.txt_time.Size = new Size(32, 21);
			this.txt_time.TabIndex = 18;
			this.txt_time.Text = "15";
			this.lb_second.Location = new Point(226, 25);
			this.lb_second.Name = "lb_second";
			this.lb_second.Size = new Size(144, 12);
			this.lb_second.TabIndex = 19;
			this.lb_second.Text = "秒(1-254)";
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(143, 175);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 23;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(245, 175);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 24;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.groupBox1.Controls.Add(this.rdb_stop);
			this.groupBox1.Controls.Add(this.rdb_normal);
			this.groupBox1.Controls.Add(this.rdb_openNormal);
			this.groupBox1.Location = new Point(6, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(328, 116);
			this.groupBox1.TabIndex = 25;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "门状态";
			this.rdb_stop.AutoSize = true;
			this.rdb_stop.Location = new Point(20, 87);
			this.rdb_stop.Name = "rdb_stop";
			this.rdb_stop.Size = new Size(131, 16);
			this.rdb_stop.TabIndex = 25;
			this.rdb_stop.Text = "禁用当天常开时间段";
			this.rdb_stop.UseVisualStyleBackColor = true;
			this.rdb_normal.AutoSize = true;
			this.rdb_normal.Location = new Point(20, 24);
			this.rdb_normal.Name = "rdb_normal";
			this.rdb_normal.Size = new Size(47, 16);
			this.rdb_normal.TabIndex = 24;
			this.rdb_normal.Text = "常开";
			this.rdb_normal.UseVisualStyleBackColor = true;
			this.rdb_openNormal.AutoSize = true;
			this.rdb_openNormal.Location = new Point(20, 54);
			this.rdb_openNormal.Name = "rdb_openNormal";
			this.rdb_openNormal.Size = new Size(131, 16);
			this.rdb_openNormal.TabIndex = 23;
			this.rdb_openNormal.Text = "启用当天常开时间段";
			this.rdb_openNormal.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(343, 210);
			base.Controls.Add(this.txt_time);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.rdb_open);
			base.Controls.Add(this.lb_second);
			base.Name = "DoorNormalFormSeting";
			this.Text = "门设置";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
