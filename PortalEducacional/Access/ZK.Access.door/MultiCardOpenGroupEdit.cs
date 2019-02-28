/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Data.Model.StdSDK;
using ZK.Utils;

namespace ZK.Access.door
{
	public class MultiCardOpenGroupEdit : Office2007Form
	{
		private int m_id = 0;

		private DataTable dtGroupNo;

		private DataTable dtVerifyType;

		private DataTable dtGroupTZ;

		private AccMorecardempGroup oldGroup;

		private List<int> lstGroupNoExists;

		private List<AccTimeseg> lstTimeZone;

		private List<Machines> lstMachine;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_remark;

		private Label lb_name;

		private TextBox txt_name;

		private TextBox txt_remark;

		private Label lbl_star1;

		private Label lblStdGroupNo;

		private ComboBox cbbStdGroupNo;

		private GroupBox groupBox1;

		private ComboBox cbbVerifyType;

		private Label lblVerifyType;

		private ComboBox cbbGroupTZ;

		private Label label1;

		private CheckBox ckbValidOnHoliday;

		public event EventHandler RefreshDataEvent = null;

		public MultiCardOpenGroupEdit(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				initLang.LocaleForm(this, base.Name);
				this.LoadAllMachine();
				this.LoadAllGroupNo();
				this.LoadAllTimeZone();
				this.GenerateDataSource();
				this.BindData();
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败"));
			}
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccMorecardempGroup accMorecardempGroup = null;
					AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
					accMorecardempGroup = accMorecardempGroupBll.GetModel(this.m_id);
					if (accMorecardempGroup != null)
					{
						this.txt_name.Text = accMorecardempGroup.group_name;
						this.txt_remark.Text = accMorecardempGroup.memo;
					}
					this.cbbStdGroupNo.SelectedValue = accMorecardempGroup.StdGroupNo;
					this.cbbGroupTZ.SelectedValue = accMorecardempGroup.StdGroupTz;
					this.cbbVerifyType.SelectedValue = accMorecardempGroup.StdGroupVT;
					this.ckbValidOnHoliday.Checked = accMorecardempGroup.StdValidOnHoliday;
				}
				else
				{
					this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool BindModel(AccMorecardempGroup fopen)
		{
			if (this.check())
			{
				fopen.group_name = this.txt_name.Text;
				fopen.memo = this.txt_remark.Text;
				int.TryParse((this.cbbStdGroupNo.SelectedValue ?? ((object)0)).ToString(), out int num);
				fopen.StdGroupNo = num;
				int.TryParse((this.cbbGroupTZ.SelectedValue ?? ((object)0)).ToString(), out num);
				fopen.StdGroupTz = num;
				int.TryParse((this.cbbVerifyType.SelectedValue ?? ((object)0)).ToString(), out num);
				fopen.StdGroupVT = num;
				fopen.StdValidOnHoliday = this.ckbValidOnHoliday.Checked;
				return true;
			}
			return false;
		}

		private bool check()
		{
			if (!string.IsNullOrEmpty(this.txt_name.Text.Trim()))
			{
				return true;
			}
			SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
			this.txt_name.Focus();
			return false;
		}

		private bool Save()
		{
			try
			{
				AccMorecardempGroup accMorecardempGroup = null;
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accMorecardempGroup = accMorecardempGroupBll.GetModel(this.m_id);
				}
				if (accMorecardempGroup == null)
				{
					accMorecardempGroup = new AccMorecardempGroup();
					if (accMorecardempGroupBll.Exists(this.txt_name.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.txt_name.Focus();
						return false;
					}
				}
				else if (accMorecardempGroup.group_name != this.txt_name.Text && accMorecardempGroupBll.Exists(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
					this.txt_name.Focus();
					return false;
				}
				if (this.BindModel(accMorecardempGroup))
				{
					if (this.m_id > 0)
					{
						if (accMorecardempGroupBll.Update(accMorecardempGroup))
						{
							if (this.IsChanged(this.oldGroup, accMorecardempGroup))
							{
								MachinesBll machinesBll = new MachinesBll(MainForm._ia);
								List<Machines> modelList = machinesBll.GetModelList("DevSDKType = 2");
								modelList = ((modelList == null) ? new List<Machines>() : modelList);
								if (accMorecardempGroup.StdGroupTz > 0 && accMorecardempGroup.StdGroupTz != this.oldGroup.StdGroupTz)
								{
									AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
									List<AccTimeseg> modelList2 = accTimesegBll.GetModelList(string.Format("TimeZone1Id = {0} or TimeZone2Id = {0} or TimeZone3Id = {0} or TimeZoneHolidayId = {0}", accMorecardempGroup.StdGroupTz));
									if (modelList2 != null && modelList2.Count > 0)
									{
										for (int i = 0; i < modelList.Count; i++)
										{
											CommandServer.AddTimeZone(modelList[i], modelList2);
										}
									}
								}
								for (int j = 0; j < modelList.Count; j++)
								{
									Group model = CommandServer.ConvertGroup(modelList[j], accMorecardempGroup);
									CommandServer.AddGroup(modelList[j], model);
									FrmShowUpdata.Instance.ShowEx();
								}
								if (this.oldGroup.StdGroupNo != accMorecardempGroup.StdGroupNo)
								{
									UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
									List<UserInfo> modelList3 = userInfoBll.GetModelList("morecard_group_id = " + this.oldGroup.id);
									modelList3 = ((modelList3 == null) ? new List<UserInfo>() : modelList3);
									Dictionary<int, AccMorecardempGroup> dictionary = new Dictionary<int, AccMorecardempGroup>();
									dictionary.Add(this.oldGroup.id, accMorecardempGroup);
									CommandServer.SetUserGroup(modelList3, modelList, dictionary);
								}
							}
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							return true;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
					else
					{
						try
						{
							accMorecardempGroup.create_operator = SysInfos.SysUserInfo.id.ToString();
							if (accMorecardempGroupBll.Add(accMorecardempGroup) > 0)
							{
								MachinesBll machinesBll = new MachinesBll(MainForm._ia);
								List<Machines> modelList = machinesBll.GetModelList("DevSDKType = 2");
								modelList = ((modelList == null) ? new List<Machines>() : modelList);
								if (accMorecardempGroup.StdGroupTz > 0)
								{
									AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
									List<AccTimeseg> modelList2 = accTimesegBll.GetModelList(string.Format("TimeZone1Id = {0} or TimeZone2Id = {0} or TimeZone3Id = {0} or TimeZoneHolidayId = {0}", accMorecardempGroup.StdGroupTz));
									if (modelList2 != null && modelList2.Count > 0)
									{
										for (int k = 0; k < modelList.Count; k++)
										{
											CommandServer.AddTimeZone(modelList[k], modelList2);
										}
									}
								}
								if (this.IsChanged(this.oldGroup, accMorecardempGroup))
								{
									for (int l = 0; l < modelList.Count; l++)
									{
										Group model2 = CommandServer.ConvertGroup(modelList[l], accMorecardempGroup);
										CommandServer.AddGroup(modelList[l], model2);
										FrmShowUpdata.Instance.ShowEx();
									}
								}
								if (this.RefreshDataEvent != null)
								{
									this.RefreshDataEvent(this, null);
								}
								return true;
							}
						}
						catch
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
						}
					}
				}
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
			}
			return false;
		}

		private void btn_saveAndContinue_Click(object sender, EventArgs e)
		{
			if (this.check())
			{
				this.Save();
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_remark_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 25);
		}

		private void txt_remark_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.btn_OK_Click(null, null);
			}
		}

		private void LoadAllMachine()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			this.lstMachine = machinesBll.GetModelList("");
			if (this.lstMachine == null)
			{
				this.lstMachine = new List<Machines>();
			}
		}

		private void LoadAllGroupNo()
		{
			try
			{
				this.lstGroupNoExists = new List<int>();
				AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
				List<AccMorecardempGroup> modelList = accMorecardempGroupBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						AccMorecardempGroup accMorecardempGroup = modelList[i];
						if (accMorecardempGroup.id == this.m_id)
						{
							this.oldGroup = accMorecardempGroup.Copy();
						}
						if (!this.lstGroupNoExists.Contains(accMorecardempGroup.StdGroupNo))
						{
							this.lstGroupNoExists.Add(accMorecardempGroup.StdGroupNo);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void LoadAllTimeZone()
		{
			AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
			this.lstTimeZone = accTimesegBll.GetModelList("");
		}

		private void GenerateDataSource()
		{
			this.dtGroupNo = new DataTable();
			this.dtGroupNo.Columns.Add("Value", typeof(int));
			this.dtGroupNo.Columns.Add("Text", typeof(string));
			DataRow dataRow = this.dtGroupNo.NewRow();
			dataRow["Value"] = 0;
			dataRow["Text"] = "----------";
			this.dtGroupNo.Rows.Add(dataRow);
			for (int i = 2; i <= 99; i++)
			{
				if (!this.lstGroupNoExists.Contains(i))
				{
					dataRow = this.dtGroupNo.NewRow();
					dataRow["Value"] = i;
					dataRow["Text"] = i.ToString();
					this.dtGroupNo.Rows.Add(dataRow);
				}
			}
			if (this.m_id > 0 && this.oldGroup != null)
			{
				this.InsertGroupNo(this.dtGroupNo, this.oldGroup.StdGroupNo);
			}
			this.cbbStdGroupNo.DataSource = this.dtGroupNo;
			this.cbbStdGroupNo.ValueMember = "Value";
			this.cbbStdGroupNo.DisplayMember = "Text";
			this.dtVerifyType = this.dtGroupNo.Clone();
			foreach (KeyValuePair<int, string> item in PullSDKVerifyTypeInfos.GetDic())
			{
				if (item.Key != 200 && item.Key != 22 && item.Key != 23 && item.Key != 24)
				{
					dataRow = this.dtVerifyType.NewRow();
					dataRow["Value"] = item.Key;
					dataRow["Text"] = item.Value;
					this.dtVerifyType.Rows.Add(dataRow);
				}
			}
			this.cbbVerifyType.DataSource = this.dtVerifyType;
			this.cbbVerifyType.ValueMember = "Value";
			this.cbbVerifyType.DisplayMember = "Text";
			this.dtGroupTZ = this.dtGroupNo.Clone();
			if (this.lstTimeZone != null)
			{
				for (int j = 0; j < this.lstTimeZone.Count; j++)
				{
					if (this.lstTimeZone[j].TimeZone1Id > 0 || this.lstTimeZone[j].TimeZone1Id > 0 || this.lstTimeZone[j].TimeZone1Id > 0 || this.lstTimeZone[j].TimeZoneHolidayId > 0)
					{
						dataRow = this.dtGroupTZ.NewRow();
						dataRow["Value"] = this.lstTimeZone[j].id;
						dataRow["Text"] = this.lstTimeZone[j].timeseg_name;
						this.dtGroupTZ.Rows.Add(dataRow);
					}
				}
			}
			this.cbbGroupTZ.DataSource = this.dtGroupTZ;
			this.cbbGroupTZ.ValueMember = "Value";
			this.cbbGroupTZ.DisplayMember = "Text";
		}

		private void InsertGroupNo(DataTable dt, int GroupNo)
		{
			DataRow dataRow = dt.Rows[dt.Rows.Count - 1];
			int num;
			DataRow dataRow2;
			if (int.TryParse(dataRow["Value"].ToString(), out num) && GroupNo == 99 && num != 99)
			{
				dataRow2 = dt.NewRow();
				dataRow2["Value"] = GroupNo;
				dataRow2["Text"] = GroupNo.ToString();
				dt.Rows.Add(dataRow2);
			}
			int num2 = 0;
			while (true)
			{
				if (num2 < dt.Rows.Count)
				{
					dataRow = dt.Rows[num2];
					if (int.TryParse(dataRow["Value"].ToString(), out num) && num >= GroupNo)
					{
						if (num == GroupNo)
						{
							return;
						}
						if (num > GroupNo)
						{
							break;
						}
					}
					num2++;
					continue;
				}
				return;
			}
			dataRow2 = dt.NewRow();
			dataRow2["Value"] = GroupNo;
			dataRow2["Text"] = GroupNo.ToString();
			dt.Rows.InsertAt(dataRow2, num2);
		}

		private bool IsChanged(AccMorecardempGroup oldModel, AccMorecardempGroup newModel)
		{
			if (oldModel == null || oldModel.StdGroupNo != newModel.StdGroupNo || oldModel.StdGroupTz != newModel.StdGroupTz || oldModel.StdGroupVT != newModel.StdGroupVT || this.oldGroup.StdValidOnHoliday != newModel.StdValidOnHoliday)
			{
				return true;
			}
			return false;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MultiCardOpenGroupEdit));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_remark = new Label();
			this.lb_name = new Label();
			this.txt_name = new TextBox();
			this.txt_remark = new TextBox();
			this.lbl_star1 = new Label();
			this.lblStdGroupNo = new Label();
			this.cbbStdGroupNo = new ComboBox();
			this.groupBox1 = new GroupBox();
			this.ckbValidOnHoliday = new CheckBox();
			this.cbbGroupTZ = new ComboBox();
			this.label1 = new Label();
			this.cbbVerifyType = new ComboBox();
			this.lblVerifyType = new Label();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(214, 268);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(112, 268);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_remark.Location = new Point(12, 55);
			this.lb_remark.Name = "lb_remark";
			this.lb_remark.Size = new Size(115, 12);
			this.lb_remark.TabIndex = 40;
			this.lb_remark.Text = "备注";
			this.lb_remark.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_name.Location = new Point(12, 20);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(110, 12);
			this.lb_name.TabIndex = 38;
			this.lb_name.Text = "组名称";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_name.Location = new Point(139, 17);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(140, 21);
			this.txt_name.TabIndex = 0;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.txt_remark.Location = new Point(139, 52);
			this.txt_remark.Name = "txt_remark";
			this.txt_remark.Size = new Size(140, 21);
			this.txt_remark.TabIndex = 1;
			this.txt_remark.KeyDown += this.txt_remark_KeyDown;
			this.txt_remark.KeyPress += this.txt_remark_KeyPress;
			this.lbl_star1.AutoSize = true;
			this.lbl_star1.BackColor = Color.Transparent;
			this.lbl_star1.ForeColor = Color.Red;
			this.lbl_star1.Location = new Point(285, 20);
			this.lbl_star1.Name = "lbl_star1";
			this.lbl_star1.Size = new Size(11, 12);
			this.lbl_star1.TabIndex = 51;
			this.lbl_star1.Text = "*";
			this.lblStdGroupNo.Location = new Point(4, 20);
			this.lblStdGroupNo.Name = "lblStdGroupNo";
			this.lblStdGroupNo.Size = new Size(115, 20);
			this.lblStdGroupNo.TabIndex = 52;
			this.lblStdGroupNo.Text = "组编号";
			this.lblStdGroupNo.TextAlign = ContentAlignment.MiddleLeft;
			this.cbbStdGroupNo.FormattingEnabled = true;
			this.cbbStdGroupNo.Location = new Point(131, 20);
			this.cbbStdGroupNo.Name = "cbbStdGroupNo";
			this.cbbStdGroupNo.Size = new Size(140, 20);
			this.cbbStdGroupNo.TabIndex = 53;
			this.groupBox1.Controls.Add(this.ckbValidOnHoliday);
			this.groupBox1.Controls.Add(this.cbbGroupTZ);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.cbbVerifyType);
			this.groupBox1.Controls.Add(this.lblVerifyType);
			this.groupBox1.Controls.Add(this.cbbStdGroupNo);
			this.groupBox1.Controls.Add(this.lblStdGroupNo);
			this.groupBox1.Location = new Point(8, 92);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(288, 157);
			this.groupBox1.TabIndex = 54;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "脱机设备参数";
			this.ckbValidOnHoliday.AutoSize = true;
			this.ckbValidOnHoliday.Location = new Point(131, 119);
			this.ckbValidOnHoliday.Name = "ckbValidOnHoliday";
			this.ckbValidOnHoliday.Size = new Size(84, 16);
			this.ckbValidOnHoliday.TabIndex = 59;
			this.ckbValidOnHoliday.Text = "节假日有效";
			this.ckbValidOnHoliday.UseVisualStyleBackColor = true;
			this.cbbGroupTZ.FormattingEnabled = true;
			this.cbbGroupTZ.Location = new Point(131, 84);
			this.cbbGroupTZ.Name = "cbbGroupTZ";
			this.cbbGroupTZ.Size = new Size(140, 20);
			this.cbbGroupTZ.TabIndex = 57;
			this.label1.Location = new Point(4, 84);
			this.label1.Name = "label1";
			this.label1.Size = new Size(115, 20);
			this.label1.TabIndex = 56;
			this.label1.Text = "组时间段";
			this.label1.TextAlign = ContentAlignment.MiddleLeft;
			this.cbbVerifyType.FormattingEnabled = true;
			this.cbbVerifyType.Location = new Point(131, 52);
			this.cbbVerifyType.Name = "cbbVerifyType";
			this.cbbVerifyType.Size = new Size(140, 20);
			this.cbbVerifyType.TabIndex = 55;
			this.lblVerifyType.Location = new Point(4, 52);
			this.lblVerifyType.Name = "lblVerifyType";
			this.lblVerifyType.Size = new Size(115, 20);
			this.lblVerifyType.TabIndex = 54;
			this.lblVerifyType.Text = "验证方式";
			this.lblVerifyType.TextAlign = ContentAlignment.MiddleLeft;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(308, 303);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.txt_remark);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.lbl_star1);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_remark);
			base.Controls.Add(this.lb_name);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MultiCardOpenGroupEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "多卡开门人员组设置";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
