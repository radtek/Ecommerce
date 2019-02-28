/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Utils;

namespace ZK.Access
{
	public class LogsInfoForm : Office2007Form
	{
		private Hashtable managerTable;

		private string logObject;

		private DataTable m_dataTable = new DataTable();

		private IContainer components = null;

		private GridControl grd_view;

		private GridView grd_userOperationView;

		private GridColumn column_userID;

		private GridColumn column_time;

		private GridColumn column_actionObject;

		private GridColumn column_actionFlag;

		private GridColumn column_changeMessage;

		private ButtonX btn_cancel;

		public LogsInfoForm()
		{
			this.InitializeComponent();
		}

		public LogsInfoForm(string obectTable)
			: this()
		{
			try
			{
				this.logObject = obectTable;
				this.InitDataTableSet();
				this.GetManagerInfo();
				this.DataBind(this.logObject);
				initLang.LocaleForm(this, base.Name);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void InitDataTableSet()
		{
			this.m_dataTable.Columns.Add("user_id");
			this.m_dataTable.Columns.Add("action_time");
			this.m_dataTable.Columns.Add("object_repr");
			this.m_dataTable.Columns.Add("action_flag");
			this.m_dataTable.Columns.Add("change_message");
			this.column_userID.FieldName = "user_id";
			this.column_time.FieldName = "action_time";
			this.column_actionObject.FieldName = "object_repr";
			this.column_actionFlag.FieldName = "action_flag";
			this.column_changeMessage.FieldName = "change_message";
		}

		private void GetManagerInfo()
		{
			this.managerTable = new Hashtable();
			AuthUserBll authUserBll = new AuthUserBll(MainForm._ia);
			DataSet allList = authUserBll.GetAllList();
			if (allList != null && allList.Tables.Count > 0)
			{
				DataTable dataTable = allList.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					this.managerTable.Add(dataTable.Rows[i]["id"].ToString(), dataTable.Rows[i]["username"].ToString());
				}
			}
		}

		private void DataBind(string logTable)
		{
			try
			{
				this.m_dataTable.Rows.Clear();
				ActionLogBll actionLogBll = new ActionLogBll(MainForm._ia);
				DataSet dataSet = null;
				dataSet = (string.IsNullOrEmpty(logTable) ? actionLogBll.GetList("") : ((!(logTable == "Machines")) ? actionLogBll.GetList("object_repr='" + logTable + "'") : actionLogBll.GetList("object_repr='" + logTable + "' or object_repr='device object'")));
				if (dataSet != null && dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_dataTable.NewRow();
						if (this.managerTable.Contains(dataTable.Rows[i]["user_id"].ToString()))
						{
							dataRow[0] = (string)this.managerTable[dataTable.Rows[i]["user_id"].ToString()];
						}
						dataRow[1] = dataTable.Rows[i]["action_time"].ToString();
						dataRow[2] = dataTable.Rows[i]["object_repr"].ToString();
						if (dataTable.Rows[i]["action_flag"].ToString() == "1")
						{
							dataRow[3] = ShowMsgInfos.GetInfo("SLogAdd", "新增");
						}
						else if (dataTable.Rows[i]["action_flag"].ToString() == "2")
						{
							dataRow[3] = ShowMsgInfos.GetInfo("SLogDelete", "删除");
						}
						else if (dataTable.Rows[i]["action_flag"].ToString() == "3")
						{
							dataRow[3] = ShowMsgInfos.GetInfo("SLogModify", "修改");
						}
						else if (dataTable.Rows[i]["action_flag"].ToString() == "4")
						{
							dataRow[3] = ShowMsgInfos.GetInfo("SLogOperateDevice", "操作");
						}
						else
						{
							dataRow[3] = ShowMsgInfos.GetInfo("SLogOther", "其他");
						}
						dataRow[4] = dataTable.Rows[i]["change_message"].ToString();
						this.m_dataTable.Rows.Add(dataRow);
					}
				}
				this.grd_view.DataSource = this.m_dataTable;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
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
			GridLevelNode gridLevelNode = new GridLevelNode();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LogsInfoForm));
			this.grd_view = new GridControl();
			this.grd_userOperationView = new GridView();
			this.column_userID = new GridColumn();
			this.column_time = new GridColumn();
			this.column_actionObject = new GridColumn();
			this.column_actionFlag = new GridColumn();
			this.column_changeMessage = new GridColumn();
			this.btn_cancel = new ButtonX();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_userOperationView).BeginInit();
			base.SuspendLayout();
			gridLevelNode.RelationName = "Level1";
			this.grd_view.LevelTree.Nodes.AddRange(new GridLevelNode[1]
			{
				gridLevelNode
			});
			this.grd_view.Location = new Point(0, 0);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_userOperationView;
			this.grd_view.Name = "grd_view";
			this.grd_view.Size = new Size(774, 357);
			this.grd_view.TabIndex = 16;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_userOperationView
			});
			this.grd_userOperationView.Appearance.FooterPanel.Options.UseTextOptions = true;
			this.grd_userOperationView.Appearance.FooterPanel.TextOptions.HAlignment = HorzAlignment.Near;
			this.grd_userOperationView.Columns.AddRange(new GridColumn[5]
			{
				this.column_userID,
				this.column_time,
				this.column_actionObject,
				this.column_actionFlag,
				this.column_changeMessage
			});
			this.grd_userOperationView.GridControl = this.grd_view;
			this.grd_userOperationView.Name = "grd_userOperationView";
			this.grd_userOperationView.OptionsBehavior.Editable = false;
			this.column_userID.Caption = "用户";
			this.column_userID.Name = "column_userID";
			this.column_userID.Visible = true;
			this.column_userID.VisibleIndex = 0;
			this.column_userID.Width = 70;
			this.column_time.Caption = "操作时间";
			this.column_time.Name = "column_time";
			this.column_time.Visible = true;
			this.column_time.VisibleIndex = 1;
			this.column_time.Width = 147;
			this.column_actionObject.Caption = "对象类型";
			this.column_actionObject.Name = "column_actionObject";
			this.column_actionObject.Visible = true;
			this.column_actionObject.VisibleIndex = 2;
			this.column_actionObject.Width = 120;
			this.column_actionFlag.Caption = "动作标识";
			this.column_actionFlag.Name = "column_actionFlag";
			this.column_actionFlag.Visible = true;
			this.column_actionFlag.VisibleIndex = 3;
			this.column_actionFlag.Width = 124;
			this.column_changeMessage.Caption = "改变消息 ";
			this.column_changeMessage.Name = "column_changeMessage";
			this.column_changeMessage.Visible = true;
			this.column_changeMessage.VisibleIndex = 4;
			this.column_changeMessage.Width = 295;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(678, 370);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 17;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.btn_cancel_Click;
			base.AcceptButton = this.btn_cancel;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(772, 405);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.grd_view);
			this.DoubleBuffered = true;
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LogsInfoForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "日志记录";
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_userOperationView).EndInit();
			base.ResumeLayout(false);
		}
	}
}
