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
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DoorSearchForm : Office2007Form
	{
		public string ConditionStr;

		private Dictionary<int, string> deviceList = new Dictionary<int, string>();

		private Dictionary<int, int> indexidlist = new Dictionary<int, int>();

		private IContainer components = null;

		private Label lbl_doorName;

		private TextBox txt_doorName;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		private Label lbl_deviceName;

		private ComboBox cmb_devName;

		public DoorSearchForm()
		{
			this.InitializeComponent();
			this.LoadDevInfo();
			initLang.LocaleForm(this, base.Name);
		}

		private void LoadDevInfo()
		{
			this.deviceList.Clear();
			this.indexidlist.Clear();
			this.cmb_devName.Items.Add("-----");
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.deviceList.Add(list[i].ID, list[i].MachineAlias);
					this.indexidlist.Add(this.cmb_devName.Items.Count, list[i].ID);
					this.cmb_devName.Items.Add(list[i].MachineAlias);
				}
			}
			initLang.ComboBoxAutoSize(this.cmb_devName, this);
			this.cmb_devName.SelectedIndex = 0;
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("id>0 ");
			if (!string.IsNullOrEmpty(this.txt_doorName.Text))
			{
				stringBuilder.Append(" and door_name like'%" + this.txt_doorName.Text + "%'");
			}
			if (this.cmb_devName.SelectedIndex != 0 && this.indexidlist.ContainsKey(this.cmb_devName.SelectedIndex))
			{
				stringBuilder.Append(" and device_id=" + this.indexidlist[this.cmb_devName.SelectedIndex]);
			}
			this.ConditionStr = stringBuilder.ToString();
			base.DialogResult = DialogResult.OK;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_doorName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 40);
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
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
			this.lbl_doorName = new Label();
			this.txt_doorName = new TextBox();
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.lbl_deviceName = new Label();
			this.cmb_devName = new ComboBox();
			base.SuspendLayout();
			this.lbl_doorName.Location = new Point(12, 25);
			this.lbl_doorName.Name = "lbl_doorName";
			this.lbl_doorName.Size = new Size(88, 12);
			this.lbl_doorName.TabIndex = 0;
			this.lbl_doorName.Text = "门名称";
			this.txt_doorName.Location = new Point(106, 21);
			this.txt_doorName.Name = "txt_doorName";
			this.txt_doorName.Size = new Size(181, 21);
			this.txt_doorName.TabIndex = 1;
			this.txt_doorName.KeyPress += this.txt_doorName_KeyPress;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(106, 95);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 3;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(205, 95);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.lbl_deviceName.Location = new Point(12, 59);
			this.lbl_deviceName.Name = "lbl_deviceName";
			this.lbl_deviceName.Size = new Size(88, 12);
			this.lbl_deviceName.TabIndex = 4;
			this.lbl_deviceName.Text = "设备名称";
			this.cmb_devName.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_devName.FormattingEnabled = true;
			this.cmb_devName.Location = new Point(106, 56);
			this.cmb_devName.Name = "cmb_devName";
			this.cmb_devName.Size = new Size(181, 20);
			this.cmb_devName.TabIndex = 2;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(299, 130);
			base.Controls.Add(this.cmb_devName);
			base.Controls.Add(this.txt_doorName);
			base.Controls.Add(this.lbl_deviceName);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lbl_doorName);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DoorSearchForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "查找";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
