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
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class SelectedPersonnelForm : Office2007Form
	{
		private DataTable m_datatable = new DataTable();

		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private IContainer components = null;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		private GridControl grd_view;

		private GridView gridView1;

		private GridColumn column_personNo;

		private GridColumn column_name;

		private GridColumn column_dept;

		private GridColumn column_cardNo;

		private GridColumn column_lastName;

		private GridColumn column_gender;

		public event EventHandler SelectUserEvent;

		public SelectedPersonnelForm()
		{
			this.InitializeComponent();
			if (initLang.Lang == "chs")
			{
				this.column_lastName.Visible = false;
			}
			try
			{
				this.InitDataTableSet();
				this.GenderType();
				initLang.LocaleForm(this, base.Name);
				this.DataBind();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("dataFailedToLoad", "数据加载失败"));
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("userid");
			this.m_datatable.Columns.Add("Badgenumber").DataType = typeof(int);
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("lastname");
			this.m_datatable.Columns.Add("Gender");
			this.m_datatable.Columns.Add("deptname");
			this.m_datatable.Columns.Add("cardno");
			this.column_personNo.FieldName = "Badgenumber";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_gender.FieldName = "Gender";
			this.column_dept.FieldName = "deptname";
			this.column_cardNo.FieldName = "cardno";
		}

		private void GenderType()
		{
			this.m_gender = initLang.GetComboxInfo("gender");
			if (this.m_gender == null || this.m_gender.Count == 0)
			{
				this.m_gender = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("M", "男");
				dictionary.Add("F", "女");
				this.m_gender.Add("0", dictionary);
				initLang.SetComboxInfo("gender", this.m_gender);
				initLang.Save();
			}
		}

		private void DataBind()
		{
			try
			{
				string value = string.Empty;
				string value2 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					value = dictionary["m"];
					value2 = dictionary["f"];
				}
				this.m_datatable.Rows.Clear();
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
				DataSet list = userInfoBll.GetList(" ((CardNo is null) or (CardNo='')) ");
				if (list != null && list.Tables.Count > 0)
				{
					DataTable dataTable = list.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = dataTable.Rows[i]["userid"].ToString();
						dataRow[1] = dataTable.Rows[i]["Badgenumber"].ToString();
						dataRow[2] = dataTable.Rows[i]["Name"].ToString();
						dataRow[3] = dataTable.Rows[i]["lastname"].ToString();
						if (dataTable.Rows[i]["Gender"].ToString() == "F")
						{
							dataRow[4] = value2;
						}
						else
						{
							dataRow[4] = value;
						}
						dataRow[5] = dataTable.Rows[i]["DEPTNAME"].ToString();
						dataRow[6] = dataTable.Rows[i]["CardNo"].ToString();
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.grd_view.DataSource = this.m_datatable;
				if (this.m_datatable.Rows.Count > 0)
				{
					this.btn_OK.Enabled = true;
				}
				else
				{
					this.btn_OK.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void dataChoose()
		{
			try
			{
				int[] selectedRows = this.gridView1.GetSelectedRows();
				selectedRows = DevExpressHelper.GetDataSourceRowIndexs(this.gridView1, selectedRows);
				UserInfo userInfo = new UserInfo();
				if (selectedRows == null || selectedRows.Length == 0 || selectedRows[0] < 0 || selectedRows[0] >= this.m_datatable.Rows.Count)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SChoosePerson", "请选择人员!"));
				}
				else
				{
					int num = int.Parse(this.m_datatable.Rows[selectedRows[0]][1].ToString());
					string name = this.m_datatable.Rows[selectedRows[0]][2].ToString();
					userInfo.BadgeNumber = num.ToString();
					userInfo.Name = name;
					userInfo.UserId = int.Parse(this.m_datatable.Rows[selectedRows[0]][0].ToString());
					if (this.SelectUserEvent != null)
					{
						this.SelectUserEvent(userInfo, null);
					}
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			this.dataChoose();
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void grd_mainView_DoubleClick(object sender, EventArgs e)
		{
			this.btn_OK_Click(null, null);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SelectedPersonnelForm));
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.grd_view = new GridControl();
			this.gridView1 = new GridView();
			this.column_personNo = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_gender = new GridColumn();
			this.column_dept = new GridColumn();
			this.column_cardNo = new GridColumn();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.gridView1).BeginInit();
			base.SuspendLayout();
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(375, 308);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 0;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(469, 308);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 1;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.grd_view.Location = new Point(2, 2);
			this.grd_view.MainView = this.gridView1;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(560, 296);
			this.grd_view.TabIndex = 7;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.gridView1
			});
			this.grd_view.DoubleClick += this.grd_mainView_DoubleClick;
			this.gridView1.Columns.AddRange(new GridColumn[6]
			{
				this.column_personNo,
				this.column_name,
				this.column_lastName,
				this.column_gender,
				this.column_dept,
				this.column_cardNo
			});
			this.gridView1.GridControl = this.grd_view;
			this.gridView1.Name = "gridView1";
			this.column_personNo.Caption = "人员编号";
			this.column_personNo.Name = "column_personNo";
			this.column_personNo.Visible = true;
			this.column_personNo.VisibleIndex = 0;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 1;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 2;
			this.column_gender.Caption = "性别";
			this.column_gender.Name = "column_gender";
			this.column_gender.Visible = true;
			this.column_gender.VisibleIndex = 3;
			this.column_dept.Caption = "部门名称";
			this.column_dept.Name = "column_dept";
			this.column_dept.Visible = true;
			this.column_dept.VisibleIndex = 5;
			this.column_cardNo.Caption = "卡号";
			this.column_cardNo.Name = "column_cardNo";
			this.column_cardNo.Visible = true;
			this.column_cardNo.VisibleIndex = 4;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(563, 343);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SelectedPersonnelForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "选择人员";
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.gridView1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
