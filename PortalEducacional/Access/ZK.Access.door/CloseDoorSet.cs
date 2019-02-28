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

namespace ZK.Access.door
{
	public class CloseDoorSet : Office2007Form
	{
		private int m_second = 0;

		private IContainer components = null;

		private RadioButton rdb_close;

		private RadioButton rdb_stop;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

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

		public CloseDoorSet()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			if (this.rdb_close.Checked)
			{
				this.m_second = 1;
			}
			else
			{
				this.m_second = 2;
			}
			base.Close();
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.m_second = -1;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CloseDoorSet));
			this.rdb_close = new RadioButton();
			this.rdb_stop = new RadioButton();
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			base.SuspendLayout();
			this.rdb_close.AutoSize = true;
			this.rdb_close.Checked = true;
			this.rdb_close.Location = new Point(17, 21);
			this.rdb_close.Name = "rdb_close";
			this.rdb_close.Size = new Size(47, 16);
			this.rdb_close.TabIndex = 0;
			this.rdb_close.TabStop = true;
			this.rdb_close.Text = "关门";
			this.rdb_close.UseVisualStyleBackColor = true;
			this.rdb_stop.AutoSize = true;
			this.rdb_stop.Location = new Point(17, 50);
			this.rdb_stop.Name = "rdb_stop";
			this.rdb_stop.Size = new Size(131, 16);
			this.rdb_stop.TabIndex = 1;
			this.rdb_stop.Text = "禁用当天常开时间段";
			this.rdb_stop.UseVisualStyleBackColor = true;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(230, 97);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 3;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(128, 97);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 2;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(324, 132);
			base.Controls.Add(this.rdb_close);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.rdb_stop);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CloseDoorSet";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "远程关门";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
