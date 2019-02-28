/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class LevelUsers : Office2007Form
	{
		public Dictionary<string, Dictionary<string, string>> m_gender = null;

		private DataTable dtGender;

		private DataTable m_datatable = new DataTable();

		private Dictionary<int, List<int>> glist = new Dictionary<int, List<int>>();

		private DataTable dtAccLevel_DoorNames;

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private Dictionary<int, AccTimeseg> timelist = new Dictionary<int, AccTimeseg>();

		private WaitForm m_wait = WaitForm.Instance;

		private IContainer components = null;

		private PanelEx pnl_bottom;

		private GridControl grd_view;

		private GridView grd_mainView;

		private GridColumn column_levelName;

		private GridColumn column_doorName;

		private GridColumn column_name;

		private GridColumn column_card;

		private GridColumn column_timesegName;

		private ButtonX btn_exit;

		private GridColumn column_check;

		private RepositoryItemCheckEdit repositoryItemCheckEdit1;

		private ButtonX btn_del;

		private GridColumn column_lastName;

		private GridColumn column_number;

		private Timer time_load;

		private GridColumn column_gender;

		private RepositoryItemLookUpEdit lueDoorNames;

		private RepositoryItemLookUpEdit lueGender;

		public event EventHandler RefreshDataEvent = null;

		public LevelUsers()
		{
			try
			{
				this.InitializeComponent();
				if (initLang.Lang == "chs")
				{
					this.column_lastName.Visible = false;
				}
				this.GenderType();
				this.LoadGender();
				this.InitDataTableSet();
				this.LoadDoor();
				this.LoadDoorGroup();
				DevExpressHelper.InitImageList(this.grd_mainView, "column_check");
				initLang.LocaleForm(this, base.Name);
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
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

		private void LoadGender()
		{
			if (this.dtGender == null)
			{
				this.dtGender = new DataTable();
				this.dtGender.Columns.Add("Gender");
				this.dtGender.Columns.Add("GenderText");
				this.lueGender.DataSource = this.dtGender;
				this.lueGender.DisplayMember = "GenderText";
				this.lueGender.ValueMember = "Gender";
			}
			string text = string.Empty;
			string text2 = string.Empty;
			if (this.m_gender != null && this.m_gender.ContainsKey("0"))
			{
				Dictionary<string, string> dictionary = this.m_gender["0"];
				text = dictionary["m"];
				text2 = dictionary["f"];
			}
			this.dtGender.Rows.Clear();
			this.dtGender.Rows.Add("F", text2);
			this.dtGender.Rows.Add("M", text);
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id");
			this.m_datatable.Columns.Add("levelName");
			this.m_datatable.Columns.Add("doorName");
			this.m_datatable.Columns.Add("name");
			this.m_datatable.Columns.Add("lastname");
			this.m_datatable.Columns.Add("Gender");
			this.m_datatable.Columns.Add("cardNO");
			this.m_datatable.Columns.Add("timesegName");
			this.m_datatable.Columns.Add("check");
			this.m_datatable.Columns.Add("Badgenumber", typeof(int));
			this.column_levelName.FieldName = "levelName";
			this.column_doorName.FieldName = "doorName";
			this.column_name.FieldName = "name";
			this.column_lastName.FieldName = "lastname";
			this.column_gender.FieldName = "Gender";
			this.column_card.FieldName = "cardNO";
			this.column_timesegName.FieldName = "timesegName";
			this.column_check.FieldName = "check";
			this.column_number.FieldName = "Badgenumber";
			this.grd_view.DataSource = this.m_datatable;
		}

		private void LoadDoorGroup()
		{
			try
			{
				if (this.dtAccLevel_DoorNames == null)
				{
					this.dtAccLevel_DoorNames = new DataTable();
					this.dtAccLevel_DoorNames.Columns.Add("AccLevelId", typeof(int));
					this.dtAccLevel_DoorNames.Columns.Add("AccLevelDoorNames", typeof(string));
					this.lueDoorNames.DataSource = this.dtAccLevel_DoorNames;
					this.lueDoorNames.DisplayMember = "AccLevelDoorNames";
					this.lueDoorNames.ValueMember = "AccLevelId";
				}
				this.glist.Clear();
				this.dtAccLevel_DoorNames.Rows.Clear();
				AccLevelsetDoorGroupBll accLevelsetDoorGroupBll = new AccLevelsetDoorGroupBll(MainForm._ia);
				List<AccLevelsetDoorGroup> modelList = accLevelsetDoorGroupBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (this.glist.ContainsKey(modelList[i].acclevelset_id))
						{
							if (!this.glist[modelList[i].acclevelset_id].Contains(modelList[i].accdoor_id))
							{
								this.glist[modelList[i].acclevelset_id].Add(modelList[i].accdoor_id);
							}
						}
						else
						{
							List<int> list = new List<int>();
							list.Add(modelList[i].accdoor_id);
							this.glist.Add(modelList[i].acclevelset_id, list);
						}
					}
					foreach (int key in this.glist.Keys)
					{
						string text = "";
						List<int> list2 = this.glist[key];
						foreach (int item in list2)
						{
							if (this.dlist.ContainsKey(item))
							{
								text = text + this.dlist[item].door_name + ";";
							}
						}
						this.dtAccLevel_DoorNames.Rows.Add(key, text);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void LoadDoor()
		{
			try
			{
				this.dlist.Clear();
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				List<AccDoor> modelList = accDoorBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.dlist.ContainsKey(modelList[i].id))
						{
							this.dlist.Add(modelList[i].id, modelList[i]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void DataBind()
		{
			try
			{
				this.m_datatable.Rows.Clear();
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (this.m_gender != null && this.m_gender.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_gender["0"];
					empty = dictionary["m"];
					empty2 = dictionary["f"];
				}
				LevelDataClassBll levelDataClassBll = new LevelDataClassBll(MainForm._ia);
				DataSet list = levelDataClassBll.GetList();
				if (list != null && list.Tables.Count > 0)
				{
					this.m_datatable = list.Tables[0];
					this.m_datatable.Columns.Add("check");
					this.column_levelName.FieldName = "level_name";
					this.column_doorName.FieldName = "acclevelset_id";
					this.column_name.FieldName = "Name";
					this.column_lastName.FieldName = "lastname";
					this.column_gender.FieldName = "Gender";
					this.column_card.FieldName = "CardNo";
					this.column_timesegName.FieldName = "timeseg_name";
					this.column_check.FieldName = "check";
					this.column_number.FieldName = "Badgenumber";
					this.grd_view.DataSource = this.m_datatable;
				}
				this.grd_view.DataSource = this.m_datatable;
				this.column_check.ImageIndex = 1;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void m_gridView_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawColumnHeader(sender, e, e.Column.Name);
			}
		}

		private void m_gridView_Click(object sender, EventArgs e)
		{
			DevExpressHelper.ClickGridCheckBox(sender, "check");
		}

		private void grd_mainView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
		{
			if (e.Column != null && e.Column.FieldName == "check")
			{
				DevExpressHelper.CustomDrawCell(sender, e, e.Column.Name);
			}
		}

		private void btn_del_Click(object sender, EventArgs e)
		{
			try
			{
				int[] checkedRows = DevExpressHelper.GetCheckedRows(this.grd_mainView, "check");
				if (checkedRows != null && checkedRows.Length != 0 && checkedRows[0] >= 0 && checkedRows[0] < this.m_datatable.Rows.Count)
				{
					AccLevelsetBll accLevelsetBll = new AccLevelsetBll(MainForm._ia);
					AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
					if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteLevel", "确定要删除所属权限组?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
					{
						this.m_wait.ShowEx();
						if (checkedRows.Length < 50)
						{
							for (int i = 0; i < checkedRows.Length; i++)
							{
								this.m_wait.ShowProgress(100 * i / checkedRows.Length);
								if (checkedRows[i] < 0 || checkedRows[i] >= this.m_datatable.Rows.Count)
								{
									break;
								}
								AccLevelsetEmp model = accLevelsetEmpBll.GetModel(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								if (model != null)
								{
									AccLevelset model2 = accLevelsetBll.GetModel(model.acclevelset_id);
									if (model2 != null)
									{
										List<AccLevelsetEmp> list = new List<AccLevelsetEmp>();
										list.Add(model);
										CommandServer.UserCmd(model2, model2.level_timeseg_id, false, list, true);
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + this.m_datatable.Rows[checkedRows[i]]["Badgenumber"].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
									}
									accLevelsetEmpBll.Delete(int.Parse(this.m_datatable.Rows[checkedRows[i]][0].ToString()));
								}
							}
						}
						else
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append(this.m_datatable.Rows[checkedRows[0]][0].ToString());
							for (int j = 0; j < checkedRows.Length; j++)
							{
								this.m_wait.ShowProgress(100 * j / checkedRows.Length);
								if (checkedRows[j] < 0 || checkedRows[j] >= this.m_datatable.Rows.Count)
								{
									break;
								}
								AccLevelsetEmp model3 = accLevelsetEmpBll.GetModel(int.Parse(this.m_datatable.Rows[j][0].ToString()));
								if (model3 != null)
								{
									AccLevelset model4 = accLevelsetBll.GetModel(model3.acclevelset_id);
									if (model4 != null)
									{
										List<AccLevelsetEmp> list2 = new List<AccLevelsetEmp>();
										list2.Add(model3);
										CommandServer.UserCmd(model4, model4.level_timeseg_id, false, list2, true);
										this.m_wait.ShowInfos(ShowMsgInfos.GetInfo("SBadgeNumber", "人员编号") + this.m_datatable.Rows[checkedRows[j]]["Badgenumber"].ToString() + ":" + ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
									}
									stringBuilder.Append("," + this.m_datatable.Rows[checkedRows[j]][0].ToString());
									if (j % 1000 == 0)
									{
										accLevelsetEmpBll.DeleteList(stringBuilder.ToString());
										stringBuilder = new StringBuilder();
										if (checkedRows.Length > j + 1)
										{
											stringBuilder.Append(this.m_datatable.Rows[checkedRows[j + 1]][0].ToString());
										}
									}
								}
							}
							if (!string.IsNullOrEmpty(stringBuilder.ToString()))
							{
								accLevelsetEmpBll.DeleteList(stringBuilder.ToString());
							}
						}
						this.DataBind();
						this.m_wait.ShowProgress(100);
						this.m_wait.HideEx(false);
						FrmShowUpdata.Instance.ShowEx();
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectDeleteData", "请选择要删除的对象"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			AddLevelForm addLevelForm = new AddLevelForm(0);
			addLevelForm.refreshDataEvent += this.addLevel_RefreshDataEvent;
			addLevelForm.ShowDialog();
			addLevelForm.refreshDataEvent -= this.addLevel_RefreshDataEvent;
		}

		private void addLevel_RefreshDataEvent(object sender, EventArgs e)
		{
			this.DataBind();
			if (this.RefreshDataEvent != null)
			{
				this.RefreshDataEvent(sender, e);
			}
		}

		private void time_load_Tick(object sender, EventArgs e)
		{
			this.DataBind();
			this.time_load.Enabled = false;
			this.Cursor = Cursors.Default;
		}

		private void LevelUsers_Load(object sender, EventArgs e)
		{
			this.time_load.Enabled = true;
			this.Cursor = Cursors.WaitCursor;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(LevelUsers));
			this.pnl_bottom = new PanelEx();
			this.btn_del = new ButtonX();
			this.btn_exit = new ButtonX();
			this.grd_view = new GridControl();
			this.grd_mainView = new GridView();
			this.column_check = new GridColumn();
			this.column_number = new GridColumn();
			this.column_name = new GridColumn();
			this.column_lastName = new GridColumn();
			this.column_gender = new GridColumn();
			this.lueGender = new RepositoryItemLookUpEdit();
			this.column_levelName = new GridColumn();
			this.column_doorName = new GridColumn();
			this.lueDoorNames = new RepositoryItemLookUpEdit();
			this.column_card = new GridColumn();
			this.column_timesegName = new GridColumn();
			this.repositoryItemCheckEdit1 = new RepositoryItemCheckEdit();
			this.time_load = new Timer(this.components);
			this.pnl_bottom.SuspendLayout();
			((ISupportInitialize)this.grd_view).BeginInit();
			((ISupportInitialize)this.grd_mainView).BeginInit();
			((ISupportInitialize)this.lueGender).BeginInit();
			((ISupportInitialize)this.lueDoorNames).BeginInit();
			((ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
			base.SuspendLayout();
			this.pnl_bottom.CanvasColor = SystemColors.Control;
			this.pnl_bottom.ColorSchemeStyle = eDotNetBarStyle.StyleManagerControlled;
			this.pnl_bottom.Controls.Add(this.btn_del);
			this.pnl_bottom.Controls.Add(this.btn_exit);
			this.pnl_bottom.Dock = DockStyle.Bottom;
			this.pnl_bottom.Location = new Point(0, 383);
			this.pnl_bottom.Name = "pnl_bottom";
			this.pnl_bottom.Size = new Size(1012, 49);
			this.pnl_bottom.Style.Alignment = StringAlignment.Center;
			this.pnl_bottom.Style.BackColor1.ColorSchemePart = eColorSchemePart.PanelBackground;
			this.pnl_bottom.Style.BackColor2.ColorSchemePart = eColorSchemePart.PanelBackground2;
			this.pnl_bottom.Style.Border = eBorderType.SingleLine;
			this.pnl_bottom.Style.BorderColor.ColorSchemePart = eColorSchemePart.PanelBorder;
			this.pnl_bottom.Style.ForeColor.ColorSchemePart = eColorSchemePart.PanelText;
			this.pnl_bottom.Style.GradientAngle = 90;
			this.pnl_bottom.TabIndex = 20;
			this.btn_del.AccessibleRole = AccessibleRole.PushButton;
			this.btn_del.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_del.Location = new Point(486, 14);
			this.btn_del.Name = "btn_del";
			this.btn_del.Size = new Size(247, 23);
			this.btn_del.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_del.TabIndex = 0;
			this.btn_del.Text = "删除人员所属权限组";
			this.btn_del.Click += this.btn_del_Click;
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(753, 14);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(247, 23);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 1;
			this.btn_exit.Text = "返回";
			this.btn_exit.Click += this.btn_exit_Click;
			this.grd_view.Dock = DockStyle.Fill;
			this.grd_view.Location = new Point(0, 0);
			this.grd_view.LookAndFeel.SkinName = "DevExpress Dark Style";
			this.grd_view.MainView = this.grd_mainView;
			this.grd_view.Name = "grd_view";
			this.grd_view.RepositoryItems.AddRange(new RepositoryItem[3]
			{
				this.repositoryItemCheckEdit1,
				this.lueDoorNames,
				this.lueGender
			});
			this.grd_view.Size = new Size(1012, 383);
			this.grd_view.TabIndex = 21;
			this.grd_view.TabStop = false;
			this.grd_view.ViewCollection.AddRange(new BaseView[1]
			{
				this.grd_mainView
			});
			this.grd_mainView.Columns.AddRange(new GridColumn[9]
			{
				this.column_check,
				this.column_number,
				this.column_name,
				this.column_lastName,
				this.column_gender,
				this.column_levelName,
				this.column_doorName,
				this.column_card,
				this.column_timesegName
			});
			this.grd_mainView.GridControl = this.grd_view;
			this.grd_mainView.IndicatorWidth = 35;
			this.grd_mainView.Name = "grd_mainView";
			this.grd_mainView.OptionsView.ShowGroupPanel = false;
			this.grd_mainView.CustomDrawColumnHeader += this.m_gridView_CustomDrawColumnHeader;
			this.grd_mainView.CustomDrawCell += this.grd_mainView_CustomDrawCell;
			this.grd_mainView.Click += this.m_gridView_Click;
			this.column_check.Name = "column_check";
			this.column_check.Visible = true;
			this.column_check.VisibleIndex = 0;
			this.column_check.Width = 49;
			this.column_number.Caption = "人员编号";
			this.column_number.Name = "column_number";
			this.column_number.Visible = true;
			this.column_number.VisibleIndex = 1;
			this.column_number.Width = 91;
			this.column_name.Caption = "姓名";
			this.column_name.Name = "column_name";
			this.column_name.Visible = true;
			this.column_name.VisibleIndex = 2;
			this.column_name.Width = 80;
			this.column_lastName.Caption = "姓氏";
			this.column_lastName.Name = "column_lastName";
			this.column_lastName.Visible = true;
			this.column_lastName.VisibleIndex = 3;
			this.column_lastName.Width = 97;
			this.column_gender.Caption = "性别";
			this.column_gender.ColumnEdit = this.lueGender;
			this.column_gender.Name = "column_gender";
			this.column_gender.Visible = true;
			this.column_gender.VisibleIndex = 4;
			this.column_gender.Width = 97;
			this.lueGender.AutoHeight = false;
			this.lueGender.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueGender.Name = "lueGender";
			this.lueGender.NullText = "";
			this.column_levelName.Caption = "权限组名称";
			this.column_levelName.Name = "column_levelName";
			this.column_levelName.Visible = true;
			this.column_levelName.VisibleIndex = 6;
			this.column_levelName.Width = 139;
			this.column_doorName.Caption = "门名称";
			this.column_doorName.ColumnEdit = this.lueDoorNames;
			this.column_doorName.Name = "column_doorName";
			this.column_doorName.Visible = true;
			this.column_doorName.VisibleIndex = 7;
			this.column_doorName.Width = 139;
			this.lueDoorNames.AutoHeight = false;
			this.lueDoorNames.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.lueDoorNames.Name = "lueDoorNames";
			this.lueDoorNames.NullText = "";
			this.column_card.Caption = "卡号";
			this.column_card.Name = "column_card";
			this.column_card.Visible = true;
			this.column_card.VisibleIndex = 5;
			this.column_card.Width = 139;
			this.column_timesegName.Caption = "门禁时间段";
			this.column_timesegName.Name = "column_timesegName";
			this.column_timesegName.Visible = true;
			this.column_timesegName.VisibleIndex = 8;
			this.column_timesegName.Width = 144;
			this.repositoryItemCheckEdit1.AutoHeight = false;
			this.repositoryItemCheckEdit1.LookAndFeel.UseWindowsXPTheme = true;
			this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
			this.time_load.Tick += this.time_load_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = SystemColors.GradientInactiveCaption;
			base.ClientSize = new Size(1012, 432);
			base.Controls.Add(this.grd_view);
			base.Controls.Add(this.pnl_bottom);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "LevelUsers";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "人员门禁权限设置";
			base.Load += this.LevelUsers_Load;
			this.pnl_bottom.ResumeLayout(false);
			((ISupportInitialize)this.grd_view).EndInit();
			((ISupportInitialize)this.grd_mainView).EndInit();
			((ISupportInitialize)this.lueGender).EndInit();
			((ISupportInitialize)this.lueDoorNames).EndInit();
			((ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
