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
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access
{
	public class DeviceSearchForm : Office2007Form
	{
		public string ConditionStr;

		private IContainer components = null;

		private Label lbl_device;

		private Label lbl_serialNumber;

		private Label lbl_ip;

		private Label lbl_RS485;

		private TextBox txt_devcie;

		private TextBox txt_serialNumber;

		private TextBox txt_ip;

		private TextBox txt_RS485;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		public DeviceSearchForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ID>0 ");
			if (!string.IsNullOrEmpty(this.txt_devcie.Text))
			{
				stringBuilder.Append(" and MachineAlias like'%" + this.txt_devcie.Text + "%'");
			}
			if (!string.IsNullOrEmpty(this.txt_ip.Text))
			{
				stringBuilder.Append(" and ip like'%" + this.txt_ip.Text + "%'");
			}
			if (!string.IsNullOrEmpty(this.txt_RS485.Text))
			{
				stringBuilder.Append((" and MachineNumber =" + this.txt_RS485.Text) ?? "");
			}
			if (!string.IsNullOrEmpty(this.txt_serialNumber.Text))
			{
				stringBuilder.Append(" and sn ='" + this.txt_serialNumber.Text + "'");
			}
			this.ConditionStr = stringBuilder.ToString();
			base.DialogResult = DialogResult.OK;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_RS485_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
			CheckInfo.NumberKeyPress(sender, e, 1, 255L);
		}

		private void txt_devcie_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 40);
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
		}

		private void txt_serialNumber_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 40);
			if (e.KeyChar == '\r')
			{
				this.btn_ok_Click(null, null);
			}
		}

		private void txt_ip_KeyPress(object sender, KeyPressEventArgs e)
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
			this.lbl_device = new Label();
			this.lbl_serialNumber = new Label();
			this.lbl_ip = new Label();
			this.lbl_RS485 = new Label();
			this.txt_devcie = new TextBox();
			this.txt_serialNumber = new TextBox();
			this.txt_ip = new TextBox();
			this.txt_RS485 = new TextBox();
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			base.SuspendLayout();
			this.lbl_device.AutoSize = true;
			this.lbl_device.Location = new Point(48, 28);
			this.lbl_device.Name = "lbl_device";
			this.lbl_device.Size = new Size(53, 12);
			this.lbl_device.TabIndex = 0;
			this.lbl_device.Text = "设备名称";
			this.lbl_serialNumber.AutoSize = true;
			this.lbl_serialNumber.Location = new Point(48, 112);
			this.lbl_serialNumber.Name = "lbl_serialNumber";
			this.lbl_serialNumber.Size = new Size(41, 12);
			this.lbl_serialNumber.TabIndex = 4;
			this.lbl_serialNumber.Text = "序列号";
			this.lbl_ip.AutoSize = true;
			this.lbl_ip.Location = new Point(48, 70);
			this.lbl_ip.Name = "lbl_ip";
			this.lbl_ip.Size = new Size(41, 12);
			this.lbl_ip.TabIndex = 2;
			this.lbl_ip.Text = "IP地址";
			this.lbl_RS485.AutoSize = true;
			this.lbl_RS485.Location = new Point(48, 154);
			this.lbl_RS485.Name = "lbl_RS485";
			this.lbl_RS485.Size = new Size(59, 12);
			this.lbl_RS485.TabIndex = 6;
			this.lbl_RS485.Text = "RS485地址";
			this.txt_devcie.Location = new Point(225, 24);
			this.txt_devcie.Name = "txt_devcie";
			this.txt_devcie.Size = new Size(135, 21);
			this.txt_devcie.TabIndex = 1;
			this.txt_devcie.KeyPress += this.txt_devcie_KeyPress;
			this.txt_serialNumber.Location = new Point(225, 108);
			this.txt_serialNumber.Name = "txt_serialNumber";
			this.txt_serialNumber.Size = new Size(135, 21);
			this.txt_serialNumber.TabIndex = 5;
			this.txt_serialNumber.KeyPress += this.txt_serialNumber_KeyPress;
			this.txt_ip.Location = new Point(225, 66);
			this.txt_ip.Name = "txt_ip";
			this.txt_ip.Size = new Size(135, 21);
			this.txt_ip.TabIndex = 3;
			this.txt_ip.KeyPress += this.txt_ip_KeyPress;
			this.txt_RS485.Location = new Point(225, 150);
			this.txt_RS485.MaxLength = 255;
			this.txt_RS485.Name = "txt_RS485";
			this.txt_RS485.Size = new Size(135, 21);
			this.txt_RS485.TabIndex = 7;
			this.txt_RS485.KeyPress += this.txt_RS485_KeyPress;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(153, 210);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 8;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(253, 210);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 9;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(488, 245);
			base.Controls.Add(this.txt_RS485);
			base.Controls.Add(this.txt_serialNumber);
			base.Controls.Add(this.txt_ip);
			base.Controls.Add(this.txt_devcie);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.lbl_RS485);
			base.Controls.Add(this.lbl_ip);
			base.Controls.Add(this.lbl_serialNumber);
			base.Controls.Add(this.lbl_device);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DeviceSearchForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "查找";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
