/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access.data
{
	public class ImportColumnSet : Office2007Form
	{
		private DataConfig m_cinfig = null;

		private IContainer components = null;

		private PanelEx panelEx1;

		private ButtonX btn_cancel;

		private ButtonX btn_Next;

		private PanelEx panelEx2;

		private DataGridViewX gv;

		private DataGridViewTextBoxColumn Column_Inport;

		private DataGridViewComboBoxColumn Column_Source;

		private ButtonX btn_up;

		public DataConfig Config => this.m_cinfig;

		public ImportColumnSet(DataConfig config)
		{
			this.InitializeComponent();
			this.m_cinfig = config;
			if (this.m_cinfig != null && this.m_cinfig.ImportColumns.Count > 0)
			{
				for (int i = 0; i < this.m_cinfig.ImportColumns.Count; i++)
				{
					int index = this.gv.Rows.Add();
					this.gv.Rows[index].Cells[0].Value = this.m_cinfig.ImportColumns[i];
					if (this.m_cinfig.SelectColumns.Count > 0)
					{
						DataGridViewComboBoxCell dataGridViewComboBoxCell = this.gv.Rows[index].Cells[1] as DataGridViewComboBoxCell;
						if (dataGridViewComboBoxCell != null)
						{
							dataGridViewComboBoxCell.Items.Add("-----");
							for (int j = 0; j < this.m_cinfig.SelectColumns.Count; j++)
							{
								dataGridViewComboBoxCell.Items.Add(this.m_cinfig.SelectColumns[j]);
							}
							if (this.m_cinfig.SelectColumns.Contains(this.m_cinfig.ImportColumns[i]))
							{
								dataGridViewComboBoxCell.Value = this.m_cinfig.ImportColumns[i];
							}
							else if (i < this.m_cinfig.SelectColumns.Count)
							{
								dataGridViewComboBoxCell.Value = this.m_cinfig.SelectColumns[i];
							}
							else
							{
								dataGridViewComboBoxCell.Value = "-----";
							}
						}
					}
				}
			}
			else
			{
				this.btn_Next.Enabled = false;
			}
			initLang.LocaleForm(this, base.Name);
		}

		private void btn_Next_Click(object sender, EventArgs e)
		{
			if (this.gv.Rows.Count > 0 && this.m_cinfig != null)
			{
				this.m_cinfig.ColumnsToColumnsDic.Clear();
				for (int i = 0; i < this.gv.Rows.Count; i++)
				{
					if (!this.m_cinfig.ColumnsToColumnsDic.ContainsKey(this.gv.Rows[i].Cells[0].Value.ToString()) && this.gv.Rows[i].Cells[1] != null && this.gv.Rows[i].Cells[1].Value != null && this.gv.Rows[i].Cells[1].Value.ToString() != "-----")
					{
						this.m_cinfig.ColumnsToColumnsDic.Add(this.gv.Rows[i].Cells[0].Value.ToString(), this.gv.Rows[i].Cells[1].Value.ToString());
					}
				}
				this.m_cinfig.IsOk = true;
				base.Close();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSetImportColumnSet", "请设置好数据导入关系"));
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			if (this.m_cinfig != null)
			{
				this.m_cinfig.IsOk = false;
			}
			base.Close();
		}

		private void btn_up_Click(object sender, EventArgs e)
		{
			if (this.m_cinfig != null)
			{
				this.m_cinfig.IsOk = false;
				this.m_cinfig.IsUp = true;
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
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ImportColumnSet));
			this.panelEx1 = new PanelEx();
			this.btn_up = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.btn_Next = new ButtonX();
			this.panelEx2 = new PanelEx();
			this.gv = new DataGridViewX();
			this.Column_Inport = new DataGridViewTextBoxColumn();
			this.Column_Source = new DataGridViewComboBoxColumn();
			this.panelEx1.SuspendLayout();
			this.panelEx2.SuspendLayout();
			((ISupportInitialize)this.gv).BeginInit();
			base.SuspendLayout();
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Controls.Add(this.btn_up);
			this.panelEx1.Controls.Add(this.btn_cancel);
			this.panelEx1.Controls.Add(this.btn_Next);
			this.panelEx1.Dock = DockStyle.Bottom;
			this.panelEx1.Location = new Point(0, 294);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(437, 49);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 1;
			this.btn_up.AccessibleRole = AccessibleRole.PushButton;
			this.btn_up.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_up.Location = new Point(139, 14);
			this.btn_up.Name = "btn_up";
			this.btn_up.Size = new Size(82, 23);
			this.btn_up.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_up.TabIndex = 0;
			this.btn_up.Text = "上一步";
			this.btn_up.Click += this.btn_up_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(343, 14);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 2;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_Next.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Next.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Next.Location = new Point(241, 14);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(82, 23);
			this.btn_Next.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Next.TabIndex = 1;
			this.btn_Next.Text = "下一步";
			this.btn_Next.Click += this.btn_Next_Click;
			this.panelEx2.CanvasColor = SystemColors.Control;
			this.panelEx2.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx2.Controls.Add(this.gv);
			this.panelEx2.Dock = DockStyle.Fill;
			this.panelEx2.Location = new Point(0, 0);
			this.panelEx2.Name = "panelEx2";
			this.panelEx2.Size = new Size(437, 294);
			this.panelEx2.Style.Alignment = StringAlignment.Center;
			this.panelEx2.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx2.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx2.Style.Border = eBorderType.SingleLine;
			this.panelEx2.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx2.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx2.Style.GradientAngle = 90;
			this.panelEx2.TabIndex = 2;
			this.gv.AllowUserToAddRows = false;
			this.gv.AllowUserToDeleteRows = false;
			this.gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gv.Columns.AddRange(this.Column_Inport, this.Column_Source);
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.gv.DefaultCellStyle = dataGridViewCellStyle;
			this.gv.Dock = DockStyle.Fill;
			this.gv.GridColor = Color.FromArgb(208, 215, 229);
			this.gv.Location = new Point(0, 0);
			this.gv.Name = "gv";
			this.gv.RowTemplate.Height = 23;
			this.gv.Size = new Size(437, 294);
			this.gv.TabIndex = 3;
			this.gv.TabStop = false;
			this.Column_Inport.FillWeight = 200f;
			this.Column_Inport.HeaderText = "导入数据字段";
			this.Column_Inport.Name = "Column_Inport";
			this.Column_Inport.ReadOnly = true;
			this.Column_Inport.Width = 200;
			this.Column_Source.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.Column_Source.FillWeight = 200f;
			this.Column_Source.HeaderText = "数据源字段";
			this.Column_Source.Name = "Column_Source";
			base.AcceptButton = this.btn_Next;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(437, 343);
			base.Controls.Add(this.panelEx2);
			base.Controls.Add(this.panelEx1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ImportColumnSet";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "导入";
			this.panelEx1.ResumeLayout(false);
			this.panelEx2.ResumeLayout(false);
			((ISupportInitialize)this.gv).EndInit();
			base.ResumeLayout(false);
		}
	}
}
