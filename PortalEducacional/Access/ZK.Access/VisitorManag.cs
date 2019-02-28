/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.Access.Properties;

namespace ZK.Access
{
	public class VisitorManag : UserControl
	{
		private IContainer components = null;

		private GridControl grd_view;

		public GridView grd_mainView;

		private GridColumn column_check;

		private GridColumn column_UserName;

		private GridColumn column_SSNNo;

		private GridColumn column_dev;

		private GridColumn column_LockActiveID;

		private GridColumn column_longOpenID;

		private GridColumn column_lockDelay;

		private GridColumn column_sensorDelay;

		private GridColumn column_openDoorType;

		private GridColumn column_doorSensorStatus;

		public ToolStrip MenuPanelEx;

		private ToolStripButton btn_edit;

		private ToolStripButton btn_search;

		private ToolStripButton toolStripButton1;

		private ToolStripButton toolStripButton3;

		public VisitorManag()
		{
			this.InitializeComponent();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
		}

		private void btn_edit_Click(object sender, EventArgs e)
		{
		}

		private void btn_search_Click(object sender, EventArgs e)
		{
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
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_UserName = new GridColumn();
			this.column_SSNNo = new GridColumn();
			this.column_dev = new GridColumn();
			this.column_LockActiveID = new GridColumn();
			this.column_longOpenID = new GridColumn();
			this.column_lockDelay = new GridColumn();
			this.column_sensorDelay = new GridColumn();
			this.column_openDoorType = new GridColumn();
			this.column_doorSensorStatus = new GridColumn();
			this.MenuPanelEx = new ToolStrip();
			this.toolStripButton1 = new ToolStripButton();
			this.toolStripButton3 = new ToolStripButton();
			this.btn_edit = new ToolStripButton();
			this.btn_search = new ToolStripButton();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			this.MenuPanelEx.SuspendLayout();
			base.SuspendLayout();
			this.grd_view.Cursor = Cursors.Default;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 41);
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(706, 354);
			this.grd_view.TabIndex = 11;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[10]
			{
				this.column_check,
				this.column_UserName,
				this.column_SSNNo,
				this.column_dev,
				this.column_LockActiveID,
				this.column_longOpenID,
				this.column_lockDelay,
				this.column_sensorDelay,
				this.column_openDoorType,
				this.column_doorSensorStatus
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.PaintStyleName = "Office2003";
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 35;
			this.column_UserName.Caption = "姓名";
			this.column_UserName.Name = "column_UserName";
			this.column_UserName.Visible = true;
			this.column_UserName.VisibleIndex = 1;
			this.column_UserName.Width = 80;
			this.column_SSNNo.Caption = "证件号";
			this.column_SSNNo.Name = "column_SSNNo";
			this.column_SSNNo.Visible = true;
			this.column_SSNNo.VisibleIndex = 2;
			this.column_SSNNo.Width = 80;
			this.column_dev.Caption = "手机号";
			this.column_dev.Name = "column_dev";
			this.column_dev.Visible = true;
			this.column_dev.VisibleIndex = 3;
			this.column_dev.Width = 80;
			this.column_LockActiveID.Caption = "公司名";
			this.column_LockActiveID.Name = "column_LockActiveID";
			this.column_LockActiveID.Visible = true;
			this.column_LockActiveID.VisibleIndex = 4;
			this.column_LockActiveID.Width = 72;
			this.column_longOpenID.Caption = "性别";
			this.column_longOpenID.Name = "column_longOpenID";
			this.column_longOpenID.Visible = true;
			this.column_longOpenID.VisibleIndex = 5;
			this.column_longOpenID.Width = 83;
			this.column_lockDelay.Caption = "邮件地址";
			this.column_lockDelay.Name = "column_lockDelay";
			this.column_lockDelay.Visible = true;
			this.column_lockDelay.VisibleIndex = 6;
			this.column_lockDelay.Width = 72;
			this.column_sensorDelay.Caption = "其它1";
			this.column_sensorDelay.Name = "column_sensorDelay";
			this.column_sensorDelay.Visible = true;
			this.column_sensorDelay.VisibleIndex = 7;
			this.column_sensorDelay.Width = 72;
			this.column_openDoorType.Caption = "其它2";
			this.column_openDoorType.Name = "column_openDoorType";
			this.column_openDoorType.Visible = true;
			this.column_openDoorType.VisibleIndex = 8;
			this.column_openDoorType.Width = 72;
			this.column_doorSensorStatus.Caption = "其它3";
			this.column_doorSensorStatus.Name = "column_doorSensorStatus";
			this.column_doorSensorStatus.Visible = true;
			this.column_doorSensorStatus.VisibleIndex = 9;
			this.column_doorSensorStatus.Width = 60;
			this.MenuPanelEx.AutoSize = false;
			this.MenuPanelEx.Items.AddRange(new ToolStripItem[4]
			{
				this.toolStripButton1,
				this.toolStripButton3,
				this.btn_edit,
				this.btn_search
			});
			this.MenuPanelEx.Location = new Point(0, 0);
			this.MenuPanelEx.Name = "MenuPanelEx";
			this.MenuPanelEx.Size = new Size(706, 41);
			this.MenuPanelEx.TabIndex = 10;
			this.MenuPanelEx.Text = "toolStrip1";
			this.toolStripButton1.Image = Resources.add;
			this.toolStripButton1.ImageTransparentColor = Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new Size(78, 38);
			this.toolStripButton1.Text = "Adicionar";
			this.toolStripButton1.Click += this.toolStripButton1_Click;
			this.toolStripButton3.Image = Resources.edit;
			this.toolStripButton3.ImageScaling = ToolStripItemImageScaling.None;
			this.toolStripButton3.ImageTransparentColor = Color.Magenta;
			this.toolStripButton3.Name = "toolStripButton3";
			this.toolStripButton3.Size = new Size(53, 38);
			this.toolStripButton3.Text = "Edita";
			this.toolStripButton3.Click += this.toolStripButton3_Click;
			this.btn_edit.Image = Resources.delete;
			this.btn_edit.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_edit.ImageTransparentColor = Color.Magenta;
			this.btn_edit.Name = "btn_edit";
			this.btn_edit.Size = new Size(61, 38);
			this.btn_edit.Text = "Apaga";
			this.btn_edit.Click += this.btn_edit_Click;
			this.btn_search.Image = Resources.search;
			this.btn_search.ImageScaling = ToolStripItemImageScaling.None;
			this.btn_search.ImageTransparentColor = Color.Magenta;
			this.btn_search.Name = "btn_search";
			this.btn_search.Size = new Size(68, 38);
			this.btn_search.Text = "Procura";
			this.btn_search.Click += this.btn_search_Click;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.MenuPanelEx);
			base.Name = "VisitorManag";
			base.Size = new Size(706, 395);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			this.MenuPanelEx.ResumeLayout(false);
			this.MenuPanelEx.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
