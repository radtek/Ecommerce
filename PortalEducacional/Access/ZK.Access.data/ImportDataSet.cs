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
using System.IO;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access.data
{
	public class ImportDataSet : Office2007Form
	{
		private DataConfig m_cinfig = null;

		private IContainer components = null;

		private PanelEx pnl_bottom;

		private ButtonX btn_cancel;

		private ButtonX btn_Next;

		private PanelEx pnl_excel;

		private RadioButton rd_csv;

		private RadioButton rd_excel;

		private ButtonX btn_openExcel;

		private TextBox txt_dataSourceUrl;

		private TextBox txt_split;

		private Label lb_split;

		private Label lb_dataSource;

		private Label label1;

		private RadioButton rd_txt;

		public DataConfig Config => this.m_cinfig;

		public ImportDataSet(DataConfig config)
		{
			this.InitializeComponent();
			this.m_cinfig = config;
			this.rd_excel_CheckedChanged(null, null);
			if (!string.IsNullOrEmpty(this.m_cinfig.DataSourceUrl))
			{
				this.txt_split.Text = this.m_cinfig.DataSplit;
				if (this.m_cinfig.DataType == DataType.Excel)
				{
					this.rd_excel.Checked = true;
				}
				else if (this.m_cinfig.DataType == DataType.Csv)
				{
					this.rd_csv.Checked = true;
				}
				this.txt_dataSourceUrl.Text = this.m_cinfig.DataSourceUrl;
			}
			initLang.LocaleForm(this, base.Name);
			this.txt_dataSourceUrl_TextChanged(null, null);
		}

		private void btn_Next_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.txt_dataSourceUrl.Text))
			{
				this.m_cinfig.DataType = DataType.None;
				if (this.rd_excel.Checked)
				{
					this.m_cinfig.DataType = DataType.Excel;
				}
				else
				{
					this.m_cinfig.DataType = DataType.Csv;
				}
				this.m_cinfig.DataSourceUrl = this.txt_dataSourceUrl.Text;
				this.m_cinfig.DataSplit = this.txt_split.Text.Trim();
				if (string.IsNullOrEmpty(this.m_cinfig.DataSplit))
				{
					this.m_cinfig.DataSplit = "\t";
				}
				this.m_cinfig.IsOk = true;
				base.Close();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "目标文件不存在"));
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			this.m_cinfig.IsOk = false;
			this.m_cinfig.DataType = DataType.None;
			base.Close();
		}

		private void rd_excel_CheckedChanged(object sender, EventArgs e)
		{
			this.txt_dataSourceUrl.Text = string.Empty;
			this.lb_split.Enabled = false;
			this.txt_split.Enabled = false;
		}

		private void rd_csv_CheckedChanged(object sender, EventArgs e)
		{
			this.txt_dataSourceUrl.Text = string.Empty;
			this.lb_split.Enabled = true;
			this.txt_split.Enabled = true;
		}

		private void btn_openExcel_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.FilterIndex = 0;
			openFileDialog.RestoreDirectory = true;
			if (this.rd_excel.Checked)
			{
				openFileDialog.Filter = "Execl files (*.xls)|*.xls";
			}
			else if (this.rd_csv.Checked)
			{
				openFileDialog.Filter = "csv files (*.csv)|*.csv";
			}
			else
			{
				openFileDialog.Filter = "Txt files (*.txt)|*.txt";
			}
			openFileDialog.ShowDialog();
			string fileName = openFileDialog.FileName;
			if (!string.IsNullOrEmpty(fileName))
			{
				this.txt_dataSourceUrl.Text = fileName;
			}
		}

		private void txt_dataSourceUrl_TextChanged(object sender, EventArgs e)
		{
			this.btn_Next.Enabled = false;
			try
			{
				if (!string.IsNullOrEmpty(this.txt_dataSourceUrl.Text) && File.Exists(this.txt_dataSourceUrl.Text))
				{
					this.btn_Next.Enabled = true;
				}
			}
			catch
			{
			}
		}

		private void rd_txt_CheckedChanged(object sender, EventArgs e)
		{
			this.txt_dataSourceUrl.Text = string.Empty;
			this.lb_split.Enabled = true;
			this.txt_split.Enabled = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ImportDataSet));
			this.pnl_bottom = new PanelEx();
			this.btn_cancel = new ButtonX();
			this.btn_Next = new ButtonX();
			this.pnl_excel = new PanelEx();
			this.txt_split = new TextBox();
			this.txt_dataSourceUrl = new TextBox();
			this.rd_txt = new RadioButton();
			this.lb_split = new Label();
			this.lb_dataSource = new Label();
			this.label1 = new Label();
			this.rd_csv = new RadioButton();
			this.rd_excel = new RadioButton();
			this.btn_openExcel = new ButtonX();
			this.pnl_bottom.SuspendLayout();
			this.pnl_excel.SuspendLayout();
			base.SuspendLayout();
			this.pnl_bottom.CanvasColor = SystemColors.Control;
			this.pnl_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_bottom.Controls.Add(this.btn_cancel);
			this.pnl_bottom.Controls.Add(this.btn_Next);
			this.pnl_bottom.Dock = DockStyle.Bottom;
			this.pnl_bottom.Location = new Point(0, 134);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(556, 50);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 0;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(462, 15);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 8;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_Next.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Next.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Next.Location = new Point(360, 15);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(82, 23);
			this.btn_Next.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Next.TabIndex = 7;
			this.btn_Next.Text = "下一步";
			this.btn_Next.Click += this.btn_Next_Click;
			this.pnl_excel.CanvasColor = SystemColors.Control;
			this.pnl_excel.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_excel.Controls.Add(this.txt_split);
			this.pnl_excel.Controls.Add(this.txt_dataSourceUrl);
			this.pnl_excel.Controls.Add(this.rd_txt);
			this.pnl_excel.Controls.Add(this.lb_split);
			this.pnl_excel.Controls.Add(this.lb_dataSource);
			this.pnl_excel.Controls.Add(this.label1);
			this.pnl_excel.Controls.Add(this.rd_csv);
			this.pnl_excel.Controls.Add(this.rd_excel);
			this.pnl_excel.Controls.Add(this.btn_openExcel);
			this.pnl_excel.Dock = DockStyle.Fill;
			this.pnl_excel.Location = new Point(0, 0);
			this.pnl_excel.Name = "pnl_excel";
			this.pnl_excel.Size = new Size(556, 134);
			this.pnl_excel.Style.Alignment = StringAlignment.Center;
			this.pnl_excel.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_excel.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_excel.Style.Border = eBorderType.SingleLine;
			this.pnl_excel.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_excel.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_excel.Style.GradientAngle = 90;
			this.pnl_excel.TabIndex = 6;
			this.txt_split.Location = new Point(142, 89);
			this.txt_split.Name = "txt_split";
			this.txt_split.Size = new Size(36, 21);
			this.txt_split.TabIndex = 5;
			this.txt_dataSourceUrl.BackColor = SystemColors.Window;
			this.txt_dataSourceUrl.Location = new Point(142, 56);
			this.txt_dataSourceUrl.Name = "txt_dataSourceUrl";
			this.txt_dataSourceUrl.ReadOnly = true;
			this.txt_dataSourceUrl.Size = new Size(311, 21);
			this.txt_dataSourceUrl.TabIndex = 3;
			this.txt_dataSourceUrl.TextChanged += this.txt_dataSourceUrl_TextChanged;
			this.rd_txt.AutoSize = true;
			this.rd_txt.Location = new Point(309, 24);
			this.rd_txt.Name = "rd_txt";
			this.rd_txt.Size = new Size(41, 16);
			this.rd_txt.TabIndex = 2;
			this.rd_txt.TabStop = true;
			this.rd_txt.Text = "Txt";
			this.rd_txt.UseVisualStyleBackColor = true;
			this.rd_txt.CheckedChanged += this.rd_txt_CheckedChanged;
			this.lb_split.Location = new Point(12, 92);
			this.lb_split.Name = "lb_split";
			this.lb_split.Size = new Size(117, 12);
			this.lb_split.TabIndex = 10;
			this.lb_split.Text = "分隔符";
			this.lb_dataSource.Location = new Point(12, 59);
			this.lb_dataSource.Name = "lb_dataSource";
			this.lb_dataSource.Size = new Size(117, 12);
			this.lb_dataSource.TabIndex = 9;
			this.lb_dataSource.Text = "目标文件";
			this.label1.Location = new Point(12, 26);
			this.label1.Name = "label1";
			this.label1.Size = new Size(117, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "文件格式";
			this.rd_csv.AutoSize = true;
			this.rd_csv.Location = new Point(232, 24);
			this.rd_csv.Name = "rd_csv";
			this.rd_csv.Size = new Size(41, 16);
			this.rd_csv.TabIndex = 1;
			this.rd_csv.TabStop = true;
			this.rd_csv.Text = "Csv";
			this.rd_csv.UseVisualStyleBackColor = true;
			this.rd_csv.Visible = false;
			this.rd_csv.CheckedChanged += this.rd_csv_CheckedChanged;
			this.rd_excel.AutoSize = true;
			this.rd_excel.Checked = true;
			this.rd_excel.Location = new Point(142, 24);
			this.rd_excel.Name = "rd_excel";
			this.rd_excel.Size = new Size(53, 16);
			this.rd_excel.TabIndex = 0;
			this.rd_excel.TabStop = true;
			this.rd_excel.Text = "Excel";
			this.rd_excel.UseVisualStyleBackColor = true;
			this.rd_excel.CheckedChanged += this.rd_excel_CheckedChanged;
			this.btn_openExcel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_openExcel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_openExcel.Location = new Point(469, 54);
			this.btn_openExcel.Name = "btn_openExcel";
			this.btn_openExcel.Size = new Size(75, 23);
			this.btn_openExcel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_openExcel.TabIndex = 4;
			this.btn_openExcel.Text = "浏览";
			this.btn_openExcel.Click += this.btn_openExcel_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(556, 184);
			base.Controls.Add(this.pnl_excel);
			base.Controls.Add(this.pnl_bottom);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ImportDataSet";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "导入";
			this.pnl_bottom.ResumeLayout(false);
			this.pnl_excel.ResumeLayout(false);
			this.pnl_excel.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
