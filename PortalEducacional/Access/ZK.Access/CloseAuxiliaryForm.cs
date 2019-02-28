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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class CloseAuxiliaryForm : Office2007Form
	{
		private int machineID;

		private int doorCount = 0;

		private AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);

		private DataTable dtAuxOut;

		private Dictionary<int, string> SDKErrorList = new Dictionary<int, string>();

		private IContainer components = null;

		private Label lbl_selectPoint;

		private ButtonX btn_cancel;

		private ButtonX btn_ok;

		private Panel panel1;

		private Panel panel2;

		private GridControl gridControl1;

		private GridView gridView1;

		private GridColumn colSelector;

		private GridColumn colPrintName;

		private GridColumn colAuxName;

		private GridColumn colAuxNo;

		public CloseAuxiliaryForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			DevExpressHelper.InitImageList(this.gridView1, "colSelector");
		}

		public CloseAuxiliaryForm(int MachineID)
			: this()
		{
			try
			{
				this.machineID = MachineID;
				AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
				this.dtAuxOut = accAuxiliaryBll.GetList("aux_state=1 and device_id=" + MachineID).Tables[0];
				this.dtAuxOut.Columns.Add("Check");
				this.colSelector.FieldName = "Check";
				this.colPrintName.FieldName = "printer_number";
				this.colAuxName.FieldName = "aux_name";
				this.colAuxNo.FieldName = "aux_no";
				this.gridControl1.DataSource = this.dtAuxOut;
				this.btn_ok.Enabled = (this.dtAuxOut.Rows.Count > 0);
				this.chk_auxiliary1_CheckedChanged(null, null);
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void chk_auxiliaryAll_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_ok_Click(object sender, EventArgs e)
		{
			try
			{
				List<int> list = new List<int>();
				DataRow[] array = this.dtAuxOut.Select("Check='true'");
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				Machines model = machinesBll.GetModel(this.machineID);
				DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(model);
				if (array == null || array.Length == 0)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("NoAuxOutSelected", "请选择需要关闭的辅助输出点"));
				}
				else if (deviceServer == null)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
				}
				else
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!int.TryParse(array[i]["aux_no"].ToString(), out int item))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("AuxNoError", "输出点地址错误"));
							return;
						}
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
					for (int j = 0; j < list.Count; j++)
					{
						int num = deviceServer.CloseAuxiliary(list[j]);
						if (num < 0)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败") + ":" + PullSDkErrorInfos.GetInfo(num));
							return;
						}
					}
					SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void chk_auxiliary1_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void gridView1_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "Check");
		}

		private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "Check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void gridView1_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "Check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CloseAuxiliaryForm));
			this.lbl_selectPoint = new Label();
			this.btn_cancel = new ButtonX();
			this.btn_ok = new ButtonX();
			this.panel1 = new Panel();
			this.panel2 = new Panel();
			this.gridControl1 = new GridControl();
			this.gridView1 = new GridView();
			this.colSelector = new GridColumn();
			this.colPrintName = new GridColumn();
			this.colAuxName = new GridColumn();
			this.colAuxNo = new GridColumn();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((ISupportInitialize)this.gridControl1).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			base.SuspendLayout();
			this.lbl_selectPoint.Location = new Point(21, 19);
			this.lbl_selectPoint.Name = "lbl_selectPoint";
			this.lbl_selectPoint.Size = new Size(345, 12);
			this.lbl_selectPoint.TabIndex = 15;
			this.lbl_selectPoint.Text = "选择要关闭的辅助输出点";
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(500, 15);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 6;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(398, 15);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 5;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btn_ok_Click;
			this.panel1.Controls.Add(this.btn_cancel);
			this.panel1.Controls.Add(this.btn_ok);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(0, 318);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(594, 50);
			this.panel1.TabIndex = 16;
			this.panel2.Controls.Add(this.lbl_selectPoint);
			this.panel2.Dock = DockStyle.Top;
			this.panel2.Location = new Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(594, 50);
			this.panel2.TabIndex = 17;
			this.gridControl1.Dock = DockStyle.Fill;
			this.gridControl1.Location = new Point(0, 50);
			this.gridControl1.MainView = this.gridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.Size = new Size(594, 268);
			this.gridControl1.TabIndex = 18;
			this.gridControl1.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridView1
			});
			this.gridView1.Columns.AddRange(new GridColumn[4]
			{
				this.colSelector,
				this.colPrintName,
				this.colAuxName,
				this.colAuxNo
			});
			this.gridView1.GridControl = this.gridControl1;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.CustomDrawColumnHeader += this.gridView1_CustomDrawColumnHeader;
			this.gridView1.CustomDrawCell += this.gridView1_CustomDrawCell;
			this.gridView1.Click += this.gridView1_Click;
			this.colSelector.Caption = " ";
			this.colSelector.Name = "colSelector";
			this.colSelector.Visible = true;
			this.colSelector.VisibleIndex = 0;
			this.colPrintName.Caption = "丝印";
			this.colPrintName.Name = "colPrintName";
			this.colPrintName.Visible = true;
			this.colPrintName.VisibleIndex = 1;
			this.colAuxName.Caption = "名称";
			this.colAuxName.Name = "colAuxName";
			this.colAuxName.Visible = true;
			this.colAuxName.VisibleIndex = 2;
			this.colAuxNo.Caption = "编号";
			this.colAuxNo.Name = "colAuxNo";
			this.colAuxNo.Visible = true;
			this.colAuxNo.VisibleIndex = 3;
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(594, 368);
			base.Controls.Add(this.gridControl1);
			base.Controls.Add(this.panel2);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CloseAuxiliaryForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "关闭辅助输出";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((ISupportInitialize)this.gridControl1).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
