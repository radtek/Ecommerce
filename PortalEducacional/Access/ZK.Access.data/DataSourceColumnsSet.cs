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
	public class DataSourceColumnsSet : Office2007Form
	{
		private DataConfig m_cinfig = null;

		private IContainer components = null;

		private PanelEx panelEx1;

		private PanelEx panelEx2;

		private ListViewEx lv_selectCoumns;

		private ColumnHeader column_nameEx;

		private LabelX lb_selectColumns;

		private ListViewEx lv_sourceColumns;

		private ColumnHeader column_name;

		private LabelX lb_sourceColumns;

		private ButtonX btn_leftAll;

		private ButtonX btn_left;

		private ButtonX btn_right;

		private ButtonX btn_rightAll;

		private ButtonX btn_cancel;

		private ButtonX btn_Next;

		private ButtonX btn_up;

		public DataConfig Config => this.m_cinfig;

		public DataSourceColumnsSet(DataConfig config)
		{
			this.InitializeComponent();
			this.m_cinfig = config;
			if (this.m_cinfig != null && this.m_cinfig.SourceColumns.Count > 0)
			{
				for (int i = 0; i < this.m_cinfig.SourceColumns.Count; i++)
				{
					this.lv_sourceColumns.Items.Add(this.m_cinfig.SourceColumns[i]);
				}
			}
			else
			{
				this.btn_Next.Enabled = false;
				this.btn_leftAll.Enabled = false;
				this.btn_left.Enabled = false;
				this.btn_right.Enabled = false;
				this.btn_rightAll.Enabled = false;
			}
			initLang.LocaleForm(this, base.Name);
			this.CheckBtn();
		}

		private void btn_Next_Click(object sender, EventArgs e)
		{
			if (this.lv_selectCoumns.Items.Count > 0)
			{
				this.m_cinfig.SelectColumns.Clear();
				for (int i = 0; i < this.lv_selectCoumns.Items.Count; i++)
				{
					if (this.lv_selectCoumns.Items[i].Text != "check")
					{
						this.m_cinfig.SelectColumns.Add(this.lv_selectCoumns.Items[i].Text);
					}
				}
				this.m_cinfig.IsOk = true;
				base.Close();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectImportColumnsData", "请选择导入的数据列"));
			}
		}

		private void CheckBtn()
		{
			if (this.lv_sourceColumns.Items.Count > 0)
			{
				this.btn_right.Enabled = true;
				this.btn_rightAll.Enabled = true;
			}
			else
			{
				this.btn_right.Enabled = false;
				this.btn_rightAll.Enabled = false;
			}
			if (this.lv_selectCoumns.Items.Count > 0)
			{
				this.btn_left.Enabled = true;
				this.btn_leftAll.Enabled = true;
				this.btn_Next.Enabled = true;
			}
			else
			{
				this.btn_left.Enabled = false;
				this.btn_leftAll.Enabled = false;
				this.btn_Next.Enabled = false;
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

		private void btn_rightAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.lv_sourceColumns.Items.Count; i++)
			{
				this.lv_selectCoumns.Items.Add(this.lv_sourceColumns.Items[i].Text);
			}
			this.lv_sourceColumns.Items.Clear();
			this.CheckBtn();
		}

		private void btn_right_Click(object sender, EventArgs e)
		{
			if (this.lv_sourceColumns.SelectedItems != null && this.lv_sourceColumns.SelectedItems.Count > 0)
			{
				while (this.lv_sourceColumns.SelectedItems.Count > 0)
				{
					this.lv_selectCoumns.Items.Add(this.lv_sourceColumns.SelectedItems[0].Text);
					this.lv_sourceColumns.Items.Remove(this.lv_sourceColumns.SelectedItems[0]);
				}
				this.CheckBtn();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
			}
		}

		private void btn_left_Click(object sender, EventArgs e)
		{
			if (this.lv_selectCoumns.SelectedItems != null && this.lv_selectCoumns.SelectedItems.Count > 0)
			{
				while (this.lv_selectCoumns.SelectedItems.Count > 0)
				{
					this.lv_sourceColumns.Items.Add(this.lv_selectCoumns.SelectedItems[0].Text);
					this.lv_selectCoumns.Items.Remove(this.lv_selectCoumns.SelectedItems[0]);
				}
				this.CheckBtn();
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectOneData", "请选择数据"));
			}
		}

		private void btn_leftAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.lv_selectCoumns.Items.Count; i++)
			{
				this.lv_sourceColumns.Items.Add(this.lv_selectCoumns.Items[i].Text);
			}
			this.lv_selectCoumns.Items.Clear();
			this.CheckBtn();
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

		private void lv_sourceColumns_DoubleClick(object sender, EventArgs e)
		{
			if (this.lv_sourceColumns.SelectedItems != null && this.lv_sourceColumns.SelectedItems.Count > 0)
			{
				this.btn_right_Click(sender, e);
			}
		}

		private void lv_selectCoumns_DoubleClick(object sender, EventArgs e)
		{
			if (this.lv_selectCoumns.SelectedItems != null && this.lv_selectCoumns.SelectedItems.Count > 0)
			{
				this.btn_left_Click(sender, e);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DataSourceColumnsSet));
			this.panelEx1 = new PanelEx();
			this.btn_up = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.btn_Next = new ButtonX();
			this.panelEx2 = new PanelEx();
			this.btn_leftAll = new ButtonX();
			this.btn_left = new ButtonX();
			this.btn_right = new ButtonX();
			this.btn_rightAll = new ButtonX();
			this.lv_selectCoumns = new ListViewEx();
			this.column_nameEx = new ColumnHeader();
			this.lb_selectColumns = new LabelX();
			this.lv_sourceColumns = new ListViewEx();
			this.column_name = new ColumnHeader();
			this.lb_sourceColumns = new LabelX();
			this.panelEx1.SuspendLayout();
			this.panelEx2.SuspendLayout();
			base.SuspendLayout();
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Controls.Add(this.btn_up);
			this.panelEx1.Controls.Add(this.btn_cancel);
			this.panelEx1.Controls.Add(this.btn_Next);
			this.panelEx1.Dock = DockStyle.Bottom;
			this.panelEx1.Location = new Point(0, 291);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(518, 51);
			this.panelEx1.Style.Alignment = StringAlignment.Center;
			this.panelEx1.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx1.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx1.Style.Border = eBorderType.SingleLine;
			this.panelEx1.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx1.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx1.Style.GradientAngle = 90;
			this.panelEx1.TabIndex = 0;
			this.btn_up.AccessibleRole = AccessibleRole.PushButton;
			this.btn_up.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_up.Location = new Point(220, 16);
			this.btn_up.Name = "btn_up";
			this.btn_up.Size = new Size(82, 23);
			this.btn_up.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_up.TabIndex = 4;
			this.btn_up.Text = "上一步";
			this.btn_up.Click += this.btn_up_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(424, 16);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 6;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_Next.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Next.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Next.Location = new Point(322, 16);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(82, 23);
			this.btn_Next.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Next.TabIndex = 5;
			this.btn_Next.Text = "下一步";
			this.btn_Next.Click += this.btn_Next_Click;
			this.panelEx2.CanvasColor = SystemColors.Control;
			this.panelEx2.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx2.Controls.Add(this.btn_leftAll);
			this.panelEx2.Controls.Add(this.btn_left);
			this.panelEx2.Controls.Add(this.btn_right);
			this.panelEx2.Controls.Add(this.btn_rightAll);
			this.panelEx2.Controls.Add(this.lv_selectCoumns);
			this.panelEx2.Controls.Add(this.lb_selectColumns);
			this.panelEx2.Controls.Add(this.lv_sourceColumns);
			this.panelEx2.Controls.Add(this.lb_sourceColumns);
			this.panelEx2.Dock = DockStyle.Fill;
			this.panelEx2.Location = new Point(0, 0);
			this.panelEx2.Name = "panelEx2";
			this.panelEx2.Size = new Size(518, 291);
			this.panelEx2.Style.Alignment = StringAlignment.Center;
			this.panelEx2.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.panelEx2.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.panelEx2.Style.Border = eBorderType.SingleLine;
			this.panelEx2.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.panelEx2.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.panelEx2.Style.GradientAngle = 90;
			this.panelEx2.TabIndex = 1;
			this.btn_leftAll.AccessibleRole = AccessibleRole.PushButton;
			this.btn_leftAll.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_leftAll.Location = new Point(233, 219);
			this.btn_leftAll.Name = "btn_leftAll";
			this.btn_leftAll.Size = new Size(52, 23);
			this.btn_leftAll.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_leftAll.TabIndex = 3;
			this.btn_leftAll.Text = "<<";
			this.btn_leftAll.Click += this.btn_leftAll_Click;
			this.btn_left.AccessibleRole = AccessibleRole.PushButton;
			this.btn_left.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_left.Location = new Point(233, 175);
			this.btn_left.Name = "btn_left";
			this.btn_left.Size = new Size(52, 23);
			this.btn_left.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_left.TabIndex = 2;
			this.btn_left.Text = "<";
			this.btn_left.Click += this.btn_left_Click;
			this.btn_right.AccessibleRole = AccessibleRole.PushButton;
			this.btn_right.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_right.Location = new Point(233, 133);
			this.btn_right.Name = "btn_right";
			this.btn_right.Size = new Size(52, 23);
			this.btn_right.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_right.TabIndex = 1;
			this.btn_right.Text = ">";
			this.btn_right.Click += this.btn_right_Click;
			this.btn_rightAll.AccessibleRole = AccessibleRole.PushButton;
			this.btn_rightAll.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_rightAll.Location = new Point(233, 90);
			this.btn_rightAll.Name = "btn_rightAll";
			this.btn_rightAll.Size = new Size(52, 23);
			this.btn_rightAll.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_rightAll.TabIndex = 0;
			this.btn_rightAll.Text = ">>";
			this.btn_rightAll.Click += this.btn_rightAll_Click;
			this.lv_selectCoumns.Border.Class = "ListViewBorder";
			this.lv_selectCoumns.Columns.AddRange(new ColumnHeader[1]
			{
				this.column_nameEx
			});
			this.lv_selectCoumns.FullRowSelect = true;
			this.lv_selectCoumns.GridLines = true;
			this.lv_selectCoumns.Location = new Point(299, 34);
			this.lv_selectCoumns.Name = "lv_selectCoumns";
			this.lv_selectCoumns.ShowGroups = false;
			this.lv_selectCoumns.Size = new Size(208, 249);
			this.lv_selectCoumns.TabIndex = 3;
			this.lv_selectCoumns.TabStop = false;
			this.lv_selectCoumns.UseCompatibleStateImageBehavior = false;
			this.lv_selectCoumns.View = View.Details;
			this.lv_selectCoumns.DoubleClick += this.lv_selectCoumns_DoubleClick;
			this.column_nameEx.Text = "列名";
			this.column_nameEx.Width = 207;
			this.lb_selectColumns.BackgroundStyle.Class = "";
			this.lb_selectColumns.Location = new Point(299, 12);
			this.lb_selectColumns.Name = "lb_selectColumns";
			this.lb_selectColumns.Size = new Size(172, 23);
			this.lb_selectColumns.TabIndex = 2;
			this.lb_selectColumns.Text = "已选数据列";
			this.lv_sourceColumns.Border.Class = "ListViewBorder";
			this.lv_sourceColumns.Columns.AddRange(new ColumnHeader[1]
			{
				this.column_name
			});
			this.lv_sourceColumns.FullRowSelect = true;
			this.lv_sourceColumns.GridLines = true;
			this.lv_sourceColumns.Location = new Point(12, 34);
			this.lv_sourceColumns.Name = "lv_sourceColumns";
			this.lv_sourceColumns.ShowGroups = false;
			this.lv_sourceColumns.Size = new Size(208, 249);
			this.lv_sourceColumns.TabIndex = 1;
			this.lv_sourceColumns.TabStop = false;
			this.lv_sourceColumns.UseCompatibleStateImageBehavior = false;
			this.lv_sourceColumns.View = View.Details;
			this.lv_sourceColumns.DoubleClick += this.lv_sourceColumns_DoubleClick;
			this.column_name.Text = "列名";
			this.column_name.Width = 208;
			this.lb_sourceColumns.BackgroundStyle.Class = "";
			this.lb_sourceColumns.Location = new Point(12, 12);
			this.lb_sourceColumns.Name = "lb_sourceColumns";
			this.lb_sourceColumns.Size = new Size(170, 23);
			this.lb_sourceColumns.TabIndex = 0;
			this.lb_sourceColumns.Text = "备选数据列";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(518, 342);
			base.Controls.Add(this.panelEx2);
			base.Controls.Add(this.panelEx1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DataSourceColumnsSet";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "导入";
			this.panelEx1.ResumeLayout(false);
			this.panelEx2.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
