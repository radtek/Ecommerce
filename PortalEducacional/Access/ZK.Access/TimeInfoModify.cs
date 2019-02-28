/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access
{
	public class TimeInfoModify : Office2007Form
	{
		private TimeInfo m_time = null;

		private IContainer components = null;

		private Label label1;

		private Label label2;

		private ButtonX btn_Cancel;

		private ButtonX btn_Ok;

		private DateEdit txt_start;

		private DateEdit txt_end;

		public TimeInfoModify(TimeInfo time)
		{
			this.InitializeComponent();
			this.m_time = time;
			int hour = 0;
			int hour2 = 0;
			int minute = 0;
			int minute2 = 0;
			if (this.m_time != null)
			{
				if (this.m_time.StartTime > 1439)
				{
					this.m_time.SetTime(1439, this.m_time.EndTime);
				}
				if (this.m_time.EndTime > 1439)
				{
					this.m_time.SetTime(this.m_time.StartTime, 1439);
				}
				hour = this.m_time.StartTime / 60;
				hour2 = this.m_time.EndTime / 60;
				minute = this.m_time.StartTime % 60;
				minute2 = this.m_time.EndTime % 60;
			}
			this.txt_start.EditValue = new DateTime(2000, 1, 1, hour, minute, 0);
			this.txt_end.EditValue = new DateTime(2000, 1, 1, hour2, minute2, 0);
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_Ok_Click(object sender, EventArgs e)
		{
			if (this.m_time != null)
			{
				DateTime t = (DateTime)this.txt_start.EditValue;
				DateTime t2 = (DateTime)this.txt_end.EditValue;
				if (t > t2)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("STimeInputIsError", "输入时间格式不正确或者开始时间大于结束时间"));
					return;
				}
				int starttime = t.Hour * 60 + t.Minute;
				int endtime = t2.Hour * 60 + t2.Minute;
				this.m_time.SetTime(starttime, endtime);
			}
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
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			SerializableAppearanceObject appearance2 = new SerializableAppearanceObject();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TimeInfoModify));
			this.label1 = new Label();
			this.label2 = new Label();
			this.btn_Cancel = new ButtonX();
			this.btn_Ok = new ButtonX();
			this.txt_start = new DateEdit();
			this.txt_end = new DateEdit();
			((ISupportInitialize)this.txt_start.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.txt_start.Properties).BeginInit();
			((ISupportInitialize)this.txt_end.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.txt_end.Properties).BeginInit();
			base.SuspendLayout();
			this.label1.Location = new Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new Size(151, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "开始时间";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.Location = new Point(12, 52);
			this.label2.Name = "label2";
			this.label2.Size = new Size(151, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "结束时间";
			this.label2.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Cancel.Location = new Point(213, 91);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new Size(82, 23);
			this.btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Cancel.TabIndex = 6;
			this.btn_Cancel.Text = "取消";
			this.btn_Cancel.Click += this.btn_Cancel_Click;
			this.btn_Ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Ok.Location = new Point(111, 91);
			this.btn_Ok.Name = "btn_Ok";
			this.btn_Ok.Size = new Size(82, 23);
			this.btn_Ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Ok.TabIndex = 5;
			this.btn_Ok.Text = "确定";
			this.btn_Ok.Click += this.btn_Ok_Click;
			this.txt_start.EditValue = null;
			this.txt_start.Location = new Point(182, 16);
			this.txt_start.Name = "txt_start";
			this.txt_start.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance, "", null, null, true)
			});
			this.txt_start.Properties.Mask.EditMask = "HH:mm";
			this.txt_start.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.txt_start.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.txt_start.Size = new Size(74, 21);
			this.txt_start.TabIndex = 10;
			this.txt_end.EditValue = null;
			this.txt_end.Location = new Point(182, 48);
			this.txt_end.Name = "txt_end";
			this.txt_end.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance2, "", null, null, true)
			});
			this.txt_end.Properties.Mask.EditMask = "HH:mm";
			this.txt_end.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.txt_end.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.txt_end.Size = new Size(74, 21);
			this.txt_end.TabIndex = 11;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(307, 126);
			base.Controls.Add(this.txt_end);
			base.Controls.Add(this.txt_start);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_Ok);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TimeInfoModify";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "修改时间区间";
			((ISupportInitialize)this.txt_start.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.txt_start.Properties).EndInit();
			((ISupportInitialize)this.txt_end.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.txt_end.Properties).EndInit();
			base.ResumeLayout(false);
		}
	}
}
