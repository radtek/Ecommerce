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
using DevExpress.XtraEditors.Mask;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Utils;

namespace ZK.Access.door
{
	public class ClearEventOption : Office2007Form
	{
		private IContainer components = null;

		private RadioButton rdbClearAll;

		private RadioButton rdbClearByDate;

		private Label lbStartDate;

		private Label lbEndDate;

		private DateEdit deStart;

		private DateEdit deEnd;

		private Panel panel1;

		private ButtonX btnCancel;

		private ButtonX btnOk;

		public ClearEventOption()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			DateTime dateTime = DateTime.Now.AddMonths(-3);
			this.deStart.EditValue = new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
			this.deEnd.EditValue = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month), 23, 59, 59);
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			string where = "";
			try
			{
				if (this.rdbClearByDate.Checked)
				{
					DateTime dateTime = (DateTime)this.deStart.EditValue;
					DateTime dateTime2 = (DateTime)this.deEnd.EditValue;
					string arg = (AppSite.Instance.DataType == DataType.Access) ? "#" : "'";
					where = string.Format("[time] >= {0}{1}{0} and [time] <= {0}{2}{0}", arg, dateTime.ToString("yyyy-MM-dd HH:mm:ss"), dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
				}
				AccMonitorLogBll accMonitorLogBll = new AccMonitorLogBll(MainForm._ia);
				accMonitorLogBll.Delete(where);
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("ClearAllFinished", "清除完成"));
				base.Close();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void rdbClearByDate_CheckedChanged(object sender, EventArgs e)
		{
			this.panel1.Enabled = this.rdbClearByDate.Checked;
		}

		private void rdbClearAll_CheckedChanged(object sender, EventArgs e)
		{
			this.rdbClearByDate_CheckedChanged(null, null);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ClearEventOption));
			this.rdbClearAll = new RadioButton();
			this.rdbClearByDate = new RadioButton();
			this.lbStartDate = new Label();
			this.lbEndDate = new Label();
			this.deStart = new DateEdit();
			this.deEnd = new DateEdit();
			this.panel1 = new Panel();
			this.btnCancel = new ButtonX();
			this.btnOk = new ButtonX();
			((ISupportInitialize)this.deStart.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.deStart.Properties).BeginInit();
			((ISupportInitialize)this.deEnd.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.deEnd.Properties).BeginInit();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.rdbClearAll.AutoSize = true;
			this.rdbClearAll.Location = new Point(25, 21);
			this.rdbClearAll.Name = "rdbClearAll";
			this.rdbClearAll.Size = new Size(71, 16);
			this.rdbClearAll.TabIndex = 0;
			this.rdbClearAll.Text = "清除所有";
			this.rdbClearAll.UseVisualStyleBackColor = true;
			this.rdbClearAll.CheckedChanged += this.rdbClearAll_CheckedChanged;
			this.rdbClearByDate.AutoSize = true;
			this.rdbClearByDate.Checked = true;
			this.rdbClearByDate.Location = new Point(25, 59);
			this.rdbClearByDate.Name = "rdbClearByDate";
			this.rdbClearByDate.Size = new Size(83, 16);
			this.rdbClearByDate.TabIndex = 1;
			this.rdbClearByDate.TabStop = true;
			this.rdbClearByDate.Text = "按日期清除";
			this.rdbClearByDate.UseVisualStyleBackColor = true;
			this.rdbClearByDate.CheckedChanged += this.rdbClearByDate_CheckedChanged;
			this.lbStartDate.AutoSize = true;
			this.lbStartDate.Location = new Point(11, 15);
			this.lbStartDate.Name = "lbStartDate";
			this.lbStartDate.Size = new Size(53, 12);
			this.lbStartDate.TabIndex = 2;
			this.lbStartDate.Text = "开始日期";
			this.lbEndDate.AutoSize = true;
			this.lbEndDate.Location = new Point(11, 50);
			this.lbEndDate.Name = "lbEndDate";
			this.lbEndDate.Size = new Size(53, 12);
			this.lbEndDate.TabIndex = 3;
			this.lbEndDate.Text = "结束日期";
			this.deStart.EditValue = null;
			this.deStart.Location = new Point(143, 12);
			this.deStart.Name = "deStart";
			this.deStart.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.deStart.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.deStart.Properties.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
			this.deStart.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			this.deStart.Properties.Mask.EditMask = "yyyy-MM-dd HH:mm:ss";
			this.deStart.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
			this.deStart.Size = new Size(142, 20);
			this.deStart.TabIndex = 4;
			this.deEnd.EditValue = null;
			this.deEnd.Location = new Point(143, 47);
			this.deEnd.Name = "deEnd";
			this.deEnd.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.deEnd.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.deEnd.Properties.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
			this.deEnd.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			this.deEnd.Properties.Mask.EditMask = "yyyy-MM-dd HH:mm:ss";
			this.deEnd.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
			this.deEnd.Size = new Size(142, 20);
			this.deEnd.TabIndex = 5;
			this.panel1.Controls.Add(this.lbStartDate);
			this.panel1.Controls.Add(this.deEnd);
			this.panel1.Controls.Add(this.lbEndDate);
			this.panel1.Controls.Add(this.deStart);
			this.panel1.Location = new Point(12, 85);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(291, 83);
			this.panel1.TabIndex = 6;
			this.btnCancel.AccessibleRole = AccessibleRole.PushButton;
			this.btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btnCancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.Location = new Point(221, 184);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(82, 23);
			this.btnCancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += this.btn_cancel_Click;
			this.btnOk.AccessibleRole = AccessibleRole.PushButton;
			this.btnOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btnOk.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnOk.Location = new Point(119, 184);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(82, 23);
			this.btnOk.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnOk.TabIndex = 7;
			this.btnOk.Text = "确定";
			this.btnOk.Click += this.btn_OK_Click;
			base.AcceptButton = this.btnOk;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new Size(315, 219);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.rdbClearByDate);
			base.Controls.Add(this.rdbClearAll);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ClearEventOption";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "清除记录";
			((ISupportInitialize)this.deStart.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.deStart.Properties).EndInit();
			((ISupportInitialize)this.deEnd.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.deEnd.Properties).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
