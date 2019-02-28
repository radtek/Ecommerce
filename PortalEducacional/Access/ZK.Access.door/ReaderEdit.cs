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
	public class ReaderEdit : Office2007Form
	{
		public int ReaderId;

		public string MachineName;

		public string DoorName;

		public string DoorNo;

		private DataTable dtIOState;

		private AccReader accReader;

		private IContainer components = null;

		private ComboBoxEx cbbIOState;

		private Label lblIOState;

		private Label lblReaderName;

		private TextBox txtReaderName;

		private TextBox txtReaderNo;

		private Label lblReaderNo;

		private TextBox txtMachineName;

		private Label lblMachineName;

		private TextBox txtDoorName;

		private Label lblDoorName;

		private ButtonX btnCancel;

		private ButtonX btnOk;

		public event EventHandler RefreshData;

		public ReaderEdit(int id, string mName, string dNo, string dName)
		{
			this.InitializeComponent();
			this.ReaderId = id;
			this.DoorNo = dNo;
			this.MachineName = mName;
			this.DoorName = dName;
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
				dataRow["ShowText"] = ShowMsgInfos.GetInfo("ReaderStateIn", "入");
				this.dtIOState.Rows.Add(dataRow);
				dataRow = this.dtIOState.NewRow();
				dataRow["State"] = 1;
				dataRow["ShowText"] = ShowMsgInfos.GetInfo("ReaderStateOut", "出");
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
				AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
				this.accReader = accReaderBll.GetModel(this.ReaderId);
				if (this.accReader != null)
				{
					this.txtMachineName.Text = this.MachineName;
					this.txtDoorName.Text = this.DoorName;
					this.txtReaderNo.Text = this.accReader.ReaderNo.ToString();
					this.txtReaderName.Text = this.accReader.ReaderName;
					this.cbbIOState.SelectedValue = this.accReader.ReaderState;
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
					this.BindModel(this.accReader);
					AccReaderBll accReaderBll = new AccReaderBll(MainForm._ia);
					if (this.ReaderId > 0 && !accReaderBll.Update(this.accReader))
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
			if (string.IsNullOrEmpty(this.txtReaderName.Text.Trim()))
			{
				SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("EmptyReaderName", "请输入读头名称"));
				this.txtReaderName.Focus();
				return false;
			}
			return true;
		}

		private void BindModel(AccReader Model)
		{
			Model.ReaderName = this.txtReaderName.Text.Trim();
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
			this.lblReaderName = new Label();
			this.txtReaderName = new TextBox();
			this.txtReaderNo = new TextBox();
			this.lblReaderNo = new Label();
			this.txtMachineName = new TextBox();
			this.lblMachineName = new Label();
			this.txtDoorName = new TextBox();
			this.lblDoorName = new Label();
			this.btnCancel = new ButtonX();
			this.btnOk = new ButtonX();
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
			this.lblIOState.Size = new Size(53, 12);
			this.lblIOState.TabIndex = 16;
			this.lblIOState.Text = "出入状态";
			this.lblReaderName.AutoSize = true;
			this.lblReaderName.Location = new Point(46, 122);
			this.lblReaderName.Name = "lblReaderName";
			this.lblReaderName.Size = new Size(53, 12);
			this.lblReaderName.TabIndex = 17;
			this.lblReaderName.Text = "读头名称";
			this.txtReaderName.Location = new Point(209, 118);
			this.txtReaderName.Name = "txtReaderName";
			this.txtReaderName.Size = new Size(160, 21);
			this.txtReaderName.TabIndex = 18;
			this.txtReaderNo.Enabled = false;
			this.txtReaderNo.Location = new Point(209, 86);
			this.txtReaderNo.Name = "txtReaderNo";
			this.txtReaderNo.Size = new Size(160, 21);
			this.txtReaderNo.TabIndex = 20;
			this.lblReaderNo.AutoSize = true;
			this.lblReaderNo.Location = new Point(46, 90);
			this.lblReaderNo.Name = "lblReaderNo";
			this.lblReaderNo.Size = new Size(53, 12);
			this.lblReaderNo.TabIndex = 19;
			this.lblReaderNo.Text = "读头编号";
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
			this.txtDoorName.Enabled = false;
			this.txtDoorName.Location = new Point(209, 54);
			this.txtDoorName.Name = "txtDoorName";
			this.txtDoorName.Size = new Size(160, 21);
			this.txtDoorName.TabIndex = 24;
			this.lblDoorName.AutoSize = true;
			this.lblDoorName.Location = new Point(46, 58);
			this.lblDoorName.Name = "lblDoorName";
			this.lblDoorName.Size = new Size(41, 12);
			this.lblDoorName.TabIndex = 23;
			this.lblDoorName.Text = "门名称";
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
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(434, 235);
			base.Controls.Add(this.cbbIOState);
			base.Controls.Add(this.txtReaderName);
			base.Controls.Add(this.txtReaderNo);
			base.Controls.Add(this.txtDoorName);
			base.Controls.Add(this.txtMachineName);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.lblDoorName);
			base.Controls.Add(this.lblMachineName);
			base.Controls.Add(this.lblReaderNo);
			base.Controls.Add(this.lblReaderName);
			base.Controls.Add(this.lblIOState);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ReaderEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "编辑";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
