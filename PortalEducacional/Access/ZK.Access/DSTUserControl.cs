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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class DSTUserControl : UserControl
	{
		private IContainer components = null;

		private Button btn_Log;

		private Button btn_export;

		private Button btn_search;

		private Button btn_add;

		private PanelEx pnl_DST;

		private DataGridViewX dgrd_DST;

		private DataGridViewTextBoxColumn Column1;

		private DataGridViewTextBoxColumn Column2;

		private DataGridViewTextBoxColumn Column3;

		private DataGridViewTextBoxColumn Column4;

		public PanelEx MenuPanelEx;

		private Button btn_settingDST;

		private Button btn_edit;

		private Button btn_delete;

		private ContextMenuStrip contextMenuStrip1;

		public DSTUserControl()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
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
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DSTUserControl));
			DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
			this.MenuPanelEx = new PanelEx();
			this.btn_settingDST = new Button();
			this.btn_edit = new Button();
			this.btn_delete = new Button();
			this.btn_Log = new Button();
			this.btn_export = new Button();
			this.btn_search = new Button();
			this.btn_add = new Button();
			this.pnl_DST = new PanelEx();
			this.dgrd_DST = new DataGridViewX();
			this.Column1 = new DataGridViewTextBoxColumn();
			this.Column2 = new DataGridViewTextBoxColumn();
			this.Column3 = new DataGridViewTextBoxColumn();
			this.Column4 = new DataGridViewTextBoxColumn();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.MenuPanelEx.SuspendLayout();
			((ISupportInitialize)this.dgrd_DST).BeginInit();
			base.SuspendLayout();
			this.MenuPanelEx.CanvasColor = SystemColors.Control;
			this.MenuPanelEx.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.MenuPanelEx.Controls.Add(this.btn_settingDST);
			this.MenuPanelEx.Controls.Add(this.btn_edit);
			this.MenuPanelEx.Controls.Add(this.btn_delete);
			this.MenuPanelEx.Controls.Add(this.btn_Log);
			this.MenuPanelEx.Controls.Add(this.btn_export);
			this.MenuPanelEx.Controls.Add(this.btn_search);
			this.MenuPanelEx.Controls.Add(this.btn_add);
			this.MenuPanelEx.Dock = DockStyle.Top;
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(684, 53);
			this.MenuPanelEx.Style.Alignment = StringAlignment.Center;
			this.MenuPanelEx.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.MenuPanelEx.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.MenuPanelEx.Style.Border = eBorderType.SingleLine;
			this.MenuPanelEx.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.MenuPanelEx.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.MenuPanelEx.Style.GradientAngle = 90;
			this.MenuPanelEx.TabIndex = 0;
			this.btn_settingDST.BackgroundImage = (Image)componentResourceManager.GetObject("btn_settingDST.BackgroundImage");
			this.btn_settingDST.BackgroundImageLayout = ImageLayout.Center;
			this.btn_settingDST.FlatAppearance.BorderSize = 0;
			this.btn_settingDST.FlatStyle = FlatStyle.Flat;
			this.btn_settingDST.Location = new Point(409, 3);
			this.btn_settingDST.Name = "btn_settingDST";
			this.btn_settingDST.Size = new Size(77, 47);
			this.btn_settingDST.TabIndex = 6;
			this.btn_settingDST.Text = "设置夏令时";
			this.btn_settingDST.TextAlign = ContentAlignment.BottomCenter;
			this.btn_settingDST.UseVisualStyleBackColor = true;
			this.btn_edit.BackgroundImage = (Image)componentResourceManager.GetObject("btn_edit.BackgroundImage");
			this.btn_edit.BackgroundImageLayout = ImageLayout.Center;
			this.btn_edit.FlatAppearance.BorderSize = 0;
			this.btn_edit.FlatStyle = FlatStyle.Flat;
			this.btn_edit.Location = new Point(137, 3);
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(51, 47);
			this.btn_edit.TabIndex = 2;
			this.btn_edit.Text = "编辑";
			this.btn_edit.TextAlign = ContentAlignment.BottomCenter;
			this.btn_edit.UseVisualStyleBackColor = true;
			this.btn_delete.BackgroundImage = (Image)componentResourceManager.GetObject("btn_delete.BackgroundImage");
			this.btn_delete.BackgroundImageLayout = ImageLayout.Center;
			this.btn_delete.FlatAppearance.BorderSize = 0;
			this.btn_delete.FlatStyle = FlatStyle.Flat;
			this.btn_delete.Location = new Point(72, 3);
			this.btn_delete.Name = "btn_delete";
			this.btn_delete.Size = new Size(51, 47);
			this.btn_delete.TabIndex = 1;
			this.btn_delete.Text = "删除";
			this.btn_delete.TextAlign = ContentAlignment.BottomCenter;
			this.btn_delete.UseVisualStyleBackColor = true;
			this.btn_Log.BackgroundImage = (Image)componentResourceManager.GetObject("btn_Log.BackgroundImage");
			this.btn_Log.BackgroundImageLayout = ImageLayout.Center;
			this.btn_Log.FlatAppearance.BorderSize = 0;
			this.btn_Log.FlatStyle = FlatStyle.Flat;
			this.btn_Log.Location = new Point(332, 3);
			this.btn_Log.Name = "btn_Log";
			this.btn_Log.Size = new Size(63, 47);
			this.btn_Log.TabIndex = 5;
			this.btn_Log.Text = "日志记录";
			this.btn_Log.TextAlign = ContentAlignment.BottomCenter;
			this.btn_Log.UseVisualStyleBackColor = true;
			this.btn_export.BackgroundImage = (Image)componentResourceManager.GetObject("btn_export.BackgroundImage");
			this.btn_export.BackgroundImageLayout = ImageLayout.Center;
			this.btn_export.FlatAppearance.BorderSize = 0;
			this.btn_export.FlatStyle = FlatStyle.Flat;
			this.btn_export.Location = new Point(267, 3);
			this.btn_export.Name = "btn_export";
			this.btn_export.Size = new Size(51, 47);
			this.btn_export.TabIndex = 4;
			this.btn_export.Text = "导出";
			this.btn_export.TextAlign = ContentAlignment.BottomCenter;
			this.btn_export.UseVisualStyleBackColor = true;
			this.btn_search.BackgroundImage = (Image)componentResourceManager.GetObject("btn_search.BackgroundImage");
			this.btn_search.BackgroundImageLayout = ImageLayout.Center;
			this.btn_search.FlatAppearance.BorderSize = 0;
			this.btn_search.FlatStyle = FlatStyle.Flat;
			this.btn_search.Location = new Point(202, 3);
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(51, 47);
			this.btn_search.TabIndex = 3;
			this.btn_search.Text = "查找";
			this.btn_search.TextAlign = ContentAlignment.BottomCenter;
			this.btn_search.UseVisualStyleBackColor = true;
			this.btn_add.BackgroundImage = (Image)componentResourceManager.GetObject("btn_add.BackgroundImage");
			this.btn_add.BackgroundImageLayout = ImageLayout.Center;
			this.btn_add.FlatAppearance.BorderSize = 0;
			this.btn_add.FlatStyle = FlatStyle.Flat;
			this.btn_add.Location = new Point(7, 3);
			this.btn_add.Name = "btn_add";
			this.btn_add.Size = new Size(51, 47);
			this.btn_add.TabIndex = 0;
			this.btn_add.Text = "新增";
			this.btn_add.TextAlign = ContentAlignment.BottomCenter;
			this.btn_add.UseVisualStyleBackColor = true;
			this.pnl_DST.CanvasColor = SystemColors.Control;
			this.pnl_DST.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_DST.Dock = DockStyle.Top;
			this.pnl_DST.Location = new Point(0, 53);
			this.pnl_DST.Name = "pnl_DST";
			this.pnl_DST.Size = new Size(684, 28);
			this.pnl_DST.Style.Alignment = StringAlignment.Center;
			this.pnl_DST.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_DST.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_DST.Style.Border = eBorderType.SingleLine;
			this.pnl_DST.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_DST.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_DST.Style.GradientAngle = 90;
			this.pnl_DST.TabIndex = 2;
			this.pnl_DST.Text = "夏令时";
			this.dgrd_DST.AllowUserToAddRows = false;
			this.dgrd_DST.AllowUserToDeleteRows = false;
			this.dgrd_DST.BackgroundColor = Color.White;
			this.dgrd_DST.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgrd_DST.Columns.AddRange(this.Column1, this.Column2, this.Column3, this.Column4);
			dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = SystemColors.Window;
			dataGridViewCellStyle.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 134);
			dataGridViewCellStyle.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = SystemColors.ControlText;
			dataGridViewCellStyle.WrapMode = DataGridViewTriState.False;
			this.dgrd_DST.DefaultCellStyle = dataGridViewCellStyle;
			this.dgrd_DST.Dock = DockStyle.Fill;
			this.dgrd_DST.GridColor = Color.FromArgb(208, 215, 229);
			this.dgrd_DST.Location = new Point(0, 81);
			this.dgrd_DST.Name = "dgrd_DST";
			this.dgrd_DST.RowTemplate.Height = 23;
			this.dgrd_DST.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.dgrd_DST.Size = new Size(684, 418);
			this.dgrd_DST.TabIndex = 3;
			this.Column1.HeaderText = "夏令时名称";
			this.Column1.Name = "Column1";
			this.Column1.Width = 180;
			this.Column2.HeaderText = "模式";
			this.Column2.Name = "Column2";
			this.Column2.Width = 150;
			this.Column3.HeaderText = "开始时间";
			this.Column3.Name = "Column3";
			this.Column3.Width = 150;
			this.Column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
			this.Column4.HeaderText = "结束时间";
			this.Column4.Name = "Column4";
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(61, 4);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.Controls.Add(this.dgrd_DST);
			base.Controls.Add(this.pnl_DST);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "DSTUserControl";
			base.Size = new Size(684, 499);
			this.MenuPanelEx.ResumeLayout(false);
			((ISupportInitialize)this.dgrd_DST).EndInit();
			base.ResumeLayout(false);
		}
	}
}
