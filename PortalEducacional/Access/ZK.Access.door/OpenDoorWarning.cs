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
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.door
{
	public class OpenDoorWarning : Office2007Form
	{
		private DataTable dtOpenedDoor;

		private ObjRTLogInfo LogInfo;

		private static OpenDoorWarning _Instance;

		private IContainer components = null;

		private GridControl grd_view;

		private GridView grd_deptView;

		private GridColumn colDateTime;

		private GridColumn colMachine;

		private GridColumn colDoor;

		public static OpenDoorWarning Instance
		{
			get
			{
				if (OpenDoorWarning._Instance == null)
				{
					OpenDoorWarning._Instance = new OpenDoorWarning();
				}
				return OpenDoorWarning._Instance;
			}
		}

		public OpenDoorWarning()
		{
			this.InitializeComponent();
			DevExpressHelper.InitImageList(this.grd_deptView, "");
			initLang.LocaleForm(this, base.Name);
			this.InitialDataTable();
		}

		private void InitialDataTable()
		{
			this.dtOpenedDoor = new DataTable();
			this.dtOpenedDoor.Columns.Add("CardNo");
			this.dtOpenedDoor.Columns.Add("Date");
			this.dtOpenedDoor.Columns.Add("DevID");
			this.dtOpenedDoor.Columns.Add("DevName");
			this.dtOpenedDoor.Columns.Add("DoorID");
			this.dtOpenedDoor.Columns.Add("DoorName");
			this.dtOpenedDoor.Columns.Add("DoorStatus");
			this.dtOpenedDoor.Columns.Add("EType");
			this.dtOpenedDoor.Columns.Add("InOutStatus");
			this.dtOpenedDoor.Columns.Add("IP");
			this.dtOpenedDoor.Columns.Add("Pin");
			this.dtOpenedDoor.Columns.Add("StatusInfo");
			this.dtOpenedDoor.Columns.Add("VerifyType");
			this.dtOpenedDoor.Columns.Add("WarningStatus");
			this.colDateTime.FieldName = "Date";
			this.colMachine.FieldName = "DevName";
			this.colDoor.FieldName = "DoorName";
			this.grd_view.DataSource = this.dtOpenedDoor;
		}

		public void Show(ObjRTLogInfo info, string MachineName, string DoorName)
		{
			try
			{
				this.LogInfo = info;
				if (this.LogInfo != null)
				{
					DataRow dataRow = this.dtOpenedDoor.NewRow();
					dataRow["CardNo"] = this.LogInfo.CardNo;
					dataRow["Date"] = this.LogInfo.Date;
					dataRow["DevID"] = this.LogInfo.DevID;
					dataRow["DoorID"] = this.LogInfo.DoorID;
					dataRow["DevName"] = MachineName;
					dataRow["DoorName"] = DoorName;
					dataRow["DoorStatus"] = this.LogInfo.DoorStatus;
					dataRow["EType"] = this.LogInfo.EType;
					dataRow["InOutStatus"] = this.LogInfo.InOutStatus;
					dataRow["IP"] = this.LogInfo.IP;
					dataRow["Pin"] = this.LogInfo.Pin;
					dataRow["StatusInfo"] = this.LogInfo.StatusInfo;
					dataRow["VerifyType"] = this.LogInfo.VerifyType;
					dataRow["WarningStatus"] = this.LogInfo.WarningStatus;
					this.dtOpenedDoor.Rows.InsertAt(dataRow, 0);
					if (this.dtOpenedDoor.Rows.Count > 500)
					{
						this.dtOpenedDoor.Rows.RemoveAt(this.dtOpenedDoor.Rows.Count - 1);
					}
					if (this.dtOpenedDoor.Rows.Count > 0)
					{
						this.grd_deptView.FocusedRowHandle = 0;
					}
					base.Show();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private void OpenDoorWarning_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.dtOpenedDoor.Rows.Clear();
			if (this.Equals(OpenDoorWarning.Instance) && e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				base.Hide();
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
			this.grd_view = new GridControl();
			this.grd_deptView = new GridView();
			this.colDateTime = new GridColumn();
			this.colMachine = new GridColumn();
			this.colDoor = new GridColumn();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_deptView).BeginInit();
			base.SuspendLayout();
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 0);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_deptView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(563, 369);
			this.grd_view.TabIndex = 19;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_deptView
			});
			this.grd_deptView.Columns.AddRange(new GridColumn[3]
			{
				this.colDateTime,
				this.colMachine,
				this.colDoor
			});
			this.grd_deptView.GridControl = this.grd_view;
			this.grd_deptView.Name = "grd_deptView";
			this.grd_deptView.OptionsBehavior.Editable = false;
			this.grd_deptView.OptionsView.ShowGroupPanel = false;
			this.colDateTime.Caption = "时间";
			this.colDateTime.Name = "colDateTime";
			this.colDateTime.Visible = true;
			this.colDateTime.VisibleIndex = 0;
			this.colMachine.Caption = "设备";
			this.colMachine.Name = "colMachine";
			this.colMachine.Visible = true;
			this.colMachine.VisibleIndex = 1;
			this.colDoor.Caption = "门";
			this.colDoor.Name = "colDoor";
			this.colDoor.Visible = true;
			this.colDoor.VisibleIndex = 2;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(563, 369);
			base.Controls.Add(this.grd_view);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.Name = "OpenDoorWarning";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "开门警告";
			base.FormClosing += this.OpenDoorWarning_FormClosing;
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_deptView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
