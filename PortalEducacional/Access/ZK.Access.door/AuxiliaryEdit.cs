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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class AuxiliaryEdit : Office2007Form
	{
		public int AuxId;

		public string MachineName;

		private DataTable dtIOState;

		private AccAuxiliary accAuxiliary;

		private IContainer components = null;

		private ComboBoxEx cbbIOState;

		private Label lblIOState;

		private Label lblAuxName;

		private TextBox txtAuxName;

		private TextBox txtAuxNo;

		private Label lblAuxNo;

		private TextBox txtMachineName;

		private Label lblMachineName;

		private ButtonX btnCancel;

		private ButtonX btnOk;

		private Label lblPrinterName;

		private TextBox txtPrinterName;

		public event EventHandler RefreshData;

		public AuxiliaryEdit(int id, string mName)
		{
			this.InitializeComponent();
			this.AuxId = id;
			this.MachineName = mName;
			initLang.LocaleForm(this, base.Name);
			this.LoadData();
			this.CheckPermmition();
		}

		private void CheckPermmition()
		{
			bool enabled = SysInfos.IsOwerControlPermission(SysInfos.AccessLevel);
			this.btnOk.Enabled = enabled;
		}

		private void InitialIOState()
		{
			if (this.dtIOState == null)
			{
				this.dtIOState = new DataTable();
				this.dtIOState.Columns.Add("State", typeof(int));
				this.dtIOState.Columns.Add("ShowText", typeof(string));
				DataRow dataRow = this.dtIOState.NewRow();
				dataRow["State"] = 0;
				dataRow["ShowText"] = ShowMsgInfos.GetInfo("AuxStateIn", "辅助输入");
				this.dtIOState.Rows.Add(dataRow);
				dataRow = this.dtIOState.NewRow();
				dataRow["State"] = 1;
				dataRow["ShowText"] = ShowMsgInfos.GetInfo("AuxStateOut", "辅助输出");
				this.dtIOState.Rows.Add(dataRow);
			}
			this.cbbIOState.DataSource = this.dtIOState;
			this.cbbIOState.ValueMember = "State";
			this.cbbIOState.DisplayMember = "ShowText";
		}

		private void LoadData()
		{
			try
			{
				this.InitialIOState();
				AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
				this.accAuxiliary = accAuxiliaryBll.GetModel(this.AuxId);
				if (this.accAuxiliary != null)
				{
					this.txtMachineName.Text = this.MachineName;
					this.txtPrinterName.Text = this.accAuxiliary.PrinterNumber;
					this.txtAuxNo.Text = this.accAuxiliary.AuxNo.ToString();
					this.txtAuxName.Text = this.accAuxiliary.AuxName;
					this.cbbIOState.SelectedValue = this.accAuxiliary.AuxState;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.CheckData())
				{
					this.BindModel(this.accAuxiliary);
					AccAuxiliaryBll accAuxiliaryBll = new AccAuxiliaryBll(MainForm._ia);
					if (this.AuxId > 0 && !accAuxiliaryBll.Update(this.accAuxiliary))
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("UpdateFailed", "更新失败"));
					}
					else
					{
						base.Close();
						if (this.RefreshData != null)
						{
							this.RefreshData(this, new EventArgs());
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowInfoMessage(ex.Message);
			}
		}

		private bool CheckData()
		{
			if (string.IsNullOrEmpty(this.txtAuxName.Text.Trim()))
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("EmptyAuxName", "请输入名称"));
				this.txtAuxName.Focus();
				return false;
			}
			return true;
		}

		private void BindModel(AccAuxiliary Model)
		{
			Model.AuxName = this.txtAuxName.Text.Trim();
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
			this.cbbIOState = new ComboBoxEx();
			this.lblIOState = new Label();
			this.lblAuxName = new Label();
			this.txtAuxName = new TextBox();
			this.txtAuxNo = new TextBox();
			this.lblAuxNo = new Label();
			this.txtMachineName = new TextBox();
			this.lblMachineName = new Label();
			this.btnCancel = new ButtonX();
			this.btnOk = new ButtonX();
			this.lblPrinterName = new Label();
			this.txtPrinterName = new TextBox();
			base.SuspendLayout();
			this.cbbIOState.DisplayMember = "Text";
			this.cbbIOState.DrawMode = DrawMode.OwnerDrawFixed;
			this.cbbIOState.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbbIOState.Enabled = false;
			this.cbbIOState.FormattingEnabled = true;
			this.cbbIOState.ItemHeight = 15;
			this.cbbIOState.Location = new Point(209, 150);
			this.cbbIOState.Name = "cbbIOState";
			this.cbbIOState.Size = new Size(160, 21);
			this.cbbIOState.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cbbIOState.TabIndex = 15;
			this.lblIOState.AutoSize = true;
			this.lblIOState.Location = new Point(46, 154);
			this.lblIOState.Name = "lblIOState";
			this.lblIOState.Size = new Size(77, 12);
			this.lblIOState.TabIndex = 16;
			this.lblIOState.Text = "输入输出状态";
			this.lblAuxName.AutoSize = true;
			this.lblAuxName.Location = new Point(46, 122);
			this.lblAuxName.Name = "lblAuxName";
			this.lblAuxName.Size = new Size(29, 12);
			this.lblAuxName.TabIndex = 17;
			this.lblAuxName.Text = "名称";
			this.txtAuxName.Location = new Point(209, 118);
			this.txtAuxName.Name = "txtAuxName";
			this.txtAuxName.Size = new Size(160, 21);
			this.txtAuxName.TabIndex = 18;
			this.txtAuxNo.Enabled = false;
			this.txtAuxNo.Location = new Point(209, 86);
			this.txtAuxNo.Name = "txtAuxNo";
			this.txtAuxNo.Size = new Size(160, 21);
			this.txtAuxNo.TabIndex = 20;
			this.lblAuxNo.AutoSize = true;
			this.lblAuxNo.Location = new Point(46, 90);
			this.lblAuxNo.Name = "lblAuxNo";
			this.lblAuxNo.Size = new Size(29, 12);
			this.lblAuxNo.TabIndex = 19;
			this.lblAuxNo.Text = "编号";
			this.txtMachineName.Enabled = false;
			this.txtMachineName.Location = new Point(209, 22);
			this.txtMachineName.Name = "txtMachineName";
			this.txtMachineName.Size = new Size(160, 21);
			this.txtMachineName.TabIndex = 22;
			this.lblMachineName.AutoSize = true;
			this.lblMachineName.Location = new Point(46, 26);
			this.lblMachineName.Name = "lblMachineName";
			this.lblMachineName.Size = new Size(53, 12);
			this.lblMachineName.TabIndex = 21;
			this.lblMachineName.Text = "设备名称";
			this.btnCancel.AccessibleRole = AccessibleRole.PushButton;
			this.btnCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btnCancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnCancel.Location = new Point(331, 200);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(82, 23);
			this.btnCancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 26;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += this.btnCancel_Click;
			this.btnOk.AccessibleRole = AccessibleRole.PushButton;
			this.btnOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btnOk.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnOk.Location = new Point(229, 200);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new Size(82, 23);
			this.btnOk.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnOk.TabIndex = 25;
			this.btnOk.Text = "确定";
			this.btnOk.Click += this.btnOk_Click;
			this.lblPrinterName.AutoSize = true;
			this.lblPrinterName.Location = new Point(46, 58);
			this.lblPrinterName.Name = "lblPrinterName";
			this.lblPrinterName.Size = new Size(29, 12);
			this.lblPrinterName.TabIndex = 23;
			this.lblPrinterName.Text = "丝印";
			this.txtPrinterName.Enabled = false;
			this.txtPrinterName.Location = new Point(209, 54);
			this.txtPrinterName.Name = "txtPrinterName";
			this.txtPrinterName.Size = new Size(160, 21);
			this.txtPrinterName.TabIndex = 24;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(434, 235);
			base.Controls.Add(this.cbbIOState);
			base.Controls.Add(this.txtAuxName);
			base.Controls.Add(this.txtAuxNo);
			base.Controls.Add(this.txtPrinterName);
			base.Controls.Add(this.txtMachineName);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.lblPrinterName);
			base.Controls.Add(this.lblMachineName);
			base.Controls.Add(this.lblAuxNo);
			base.Controls.Add(this.lblAuxName);
			base.Controls.Add(this.lblIOState);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AuxiliaryEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "编辑";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
