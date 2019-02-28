/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access.data
{
	public class DataView : Office2007Form
	{
		private DataConfig m_cinfig = null;

		private DataTable m_dataTable = null;

		private IContainer components = null;

		private PanelEx panelEx1;

		private ButtonX btn_cancel;

		private ButtonX btn_Next;

		private GridControl grd_view;

		private GridView grd_mainView;

		private ButtonX btn_up;

		private Timer timer1;

		public DataConfig Config => this.m_cinfig;

		public DataView(DataConfig config)
		{
			this.InitializeComponent();
			this.m_cinfig = config;
			if (this.m_cinfig != null)
			{
				try
				{
					this.InitDataTable();
					DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
					GridColumn gridColumn = this.grd_mainView.Columns[0];
					gridColumn.ImageIndex = 0;
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
			}
			else
			{
				this.btn_Next.Enabled = false;
			}
			initLang.LocaleForm(this, base.Name);
		}

		private void InitDataTable()
		{
			this.m_dataTable = new DataTable();
			if (this.m_cinfig != null && this.m_cinfig.SelectColumns.Count > 0)
			{
				this.m_dataTable.Columns.Add("check");
				GridColumn gridColumn = new GridColumn();
				this.grd_mainView.Columns.Add(gridColumn);
				gridColumn.FieldName = "check";
				gridColumn.Name = "column_check";
				gridColumn.Caption = "";
				gridColumn.Visible = true;
				gridColumn.VisibleIndex = 0;
				for (int i = 0; i < this.m_cinfig.SelectColumns.Count; i++)
				{
					this.m_dataTable.Columns.Add(this.m_cinfig.SelectColumns[i]);
					GridColumn gridColumn2 = new GridColumn();
					this.grd_mainView.Columns.Add(gridColumn2);
					gridColumn2.FieldName = this.m_cinfig.SelectColumns[i];
					gridColumn2.Name = "column_" + this.m_cinfig.SelectColumns[i];
					gridColumn2.Caption = gridColumn2.FieldName;
					gridColumn2.Visible = true;
					gridColumn2.VisibleIndex = 1 + i;
				}
				this.grd_view.DataSource = this.m_dataTable;
			}
		}

		private void LoadData()
		{
			if (this.m_cinfig != null && this.m_cinfig.SelectColumns.Count > 0 && this.m_dataTable != null && this.m_cinfig.DataSource != null)
			{
				DataTable dataSource = this.m_cinfig.DataSource;
				for (int i = 0; i < dataSource.Rows.Count; i++)
				{
					DataRow dataRow = this.m_dataTable.NewRow();
					dataRow[0] = true;
					for (int j = 0; j < this.m_cinfig.SelectColumns.Count; j++)
					{
						if (dataSource.Rows[i][this.m_cinfig.SelectColumns[j]] != null)
						{
							dataRow[this.m_cinfig.SelectColumns[j]] = dataSource.Rows[i][this.m_cinfig.SelectColumns[j]].ToString();
						}
					}
					this.m_dataTable.Rows.Add(dataRow);
				}
				this.grd_mainView.Tag = true;
				this.grd_view.DataSource = this.m_dataTable;
			}
			this.Config.OnDataLoaded(this.m_dataTable);
		}

		private void btn_Next_Click(object sender, EventArgs e)
		{
			if (this.m_cinfig != null && this.m_cinfig.SelectColumns != null && this.m_cinfig.SelectColumns.Count > 0)
			{
				this.m_cinfig.SelectDataSource = new DataTable();
				for (int i = 0; i < this.m_cinfig.SelectColumns.Count; i++)
				{
					this.m_cinfig.SelectDataSource.Columns.Add(this.m_cinfig.SelectColumns[i]);
				}
				for (int j = 0; j < this.m_dataTable.Rows.Count; j++)
				{
					string text = this.m_dataTable.Rows[j][0].ToString();
					if (text.ToLower() == "true")
					{
						DataRow dataRow = this.m_dataTable.Rows[j];
						DataRow dataRow2 = this.m_cinfig.SelectDataSource.NewRow();
						for (int k = 0; k < this.m_cinfig.SelectColumns.Count; k++)
						{
							if (dataRow[this.m_cinfig.SelectColumns[k]] != null)
							{
								dataRow2[this.m_cinfig.SelectColumns[k]] = dataRow[this.m_cinfig.SelectColumns[k]].ToString();
							}
						}
						this.m_cinfig.SelectDataSource.Rows.Add(dataRow2);
					}
				}
				if (this.m_cinfig.SelectDataSource.Rows.Count > 0)
				{
					this.m_cinfig.IsOk = true;
					base.Close();
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectImportData", "请选择需要导入的数据"));
				}
			}
			else
			{
				if (this.m_cinfig != null)
				{
					this.m_cinfig.IsOk = false;
				}
				base.Close();
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

		private void grd_mainView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
			int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
			if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_dataTable.Rows.Count)
			{
				this.btn_Next.Enabled = true;
			}
			else
			{
				this.btn_Next.Enabled = false;
			}
		}

		private void grd_mainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void grd_mainView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
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

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			this.Cursor = Cursors.WaitCursor;
			this.LoadData();
			GridColumn gridColumn = this.grd_mainView.Columns[0];
			gridColumn.ImageIndex = 0;
			this.Cursor = Cursors.Default;
		}

		private void DataView_Load(object sender, EventArgs e)
		{
			this.timer1.Enabled = true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(DataView));
			this.panelEx1 = new PanelEx();
			this.btn_up = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.btn_Next = new ButtonX();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.timer1 = new Timer(this.components);
			this.panelEx1.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			base.SuspendLayout();
			this.panelEx1.CanvasColor = SystemColors.Control;
			this.panelEx1.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.panelEx1.Controls.Add(this.btn_up);
			this.panelEx1.Controls.Add(this.btn_cancel);
			this.panelEx1.Controls.Add(this.btn_Next);
			this.panelEx1.Dock = DockStyle.Bottom;
			this.panelEx1.Location = new Point(0, 332);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new Size(717, 50);
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
			this.btn_up.Location = new Point(419, 15);
			this.btn_up.Name = "btn_up";
			this.btn_up.Size = new Size(82, 23);
			this.btn_up.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_up.TabIndex = 0;
			this.btn_up.Text = "上一步";
			this.btn_up.Click += this.btn_up_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(623, 15);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 2;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_Next.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Next.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Next.Location = new Point(521, 15);
			this.btn_Next.Name = "btn_Next";
			this.btn_Next.Size = new Size(82, 23);
			this.btn_Next.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Next.TabIndex = 1;
			this.btn_Next.Text = "下一步";
			this.btn_Next.Click += this.btn_Next_Click;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 0);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(717, 332);
			this.grd_view.TabIndex = 8;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsSelection.MultiSelect = true;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.grd_mainView.CustomDrawColumnHeader += this.grd_mainView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.grd_mainView_Click;
			this.timer1.Interval = 1000;
			this.timer1.Tick += this.timer1_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(717, 382);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.panelEx1);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "DataView";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "导入";
			base.Load += this.DataView_Load;
			this.panelEx1.ResumeLayout(false);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
