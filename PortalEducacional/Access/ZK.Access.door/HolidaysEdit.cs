/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
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

namespace ZK.Access.door
{
	public class HolidaysEdit : Office2007Form
	{
		private int m_id = 0;

		private AccHolidays oldHoilday = null;

		private const int MAX_HOLIDAY_COUNT_PER_TYPE = 32;

		private Dictionary<string, Dictionary<string, string>> m_typeDic = null;

		private DataTable dtHolidayNo;

		private DataTable dtHolidayTZ;

		private AccHolidays Holiday;

		private List<int> lstExistsNo;

		private List<int> lstExistsTZ;

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private TextBox txt_Remarks;

		private TextBox txt_name;

		private Label lal_Remarks;

		private Label lb_type;

		private Label lb_name;

		private System.Windows.Forms.ComboBox cmb_type;

		private Label lb_startdate;

		private Label lb_enddate;

		private Label lb_loop;

		private CheckBox ch_loop;

		private Label label5;

		private Label label6;

		private Label label7;

		private Label label8;

		private DateEdit dt_start;

		private DateEdit dt_end;

		private Label lblHolidayNo;

		private System.Windows.Forms.ComboBox cbbHolidayNo;

		private System.Windows.Forms.ComboBox cbbHolidayTZ;

		private Label lblHolidayTZ;

		private GroupBox groupBox1;

		public event EventHandler RefreshDataEvent = null;

		public HolidaysEdit(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			try
			{
				this.LoadHolidaysType();
				if (this.cmb_type.Items.Count > 0)
				{
					this.cmb_type.SelectedIndex = 0;
				}
				initLang.LocaleForm(this, base.Name);
				this.LoadAllHolidayNo();
				this.LoadAllTimeZoneNo();
				this.GenerateDataSource();
				this.BindData();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataLoadFailed", "数据加载失败") + ":" + ex.Message);
			}
		}

		private void LoadHolidaysType()
		{
			try
			{
				this.cmb_type.Items.Clear();
				this.m_typeDic = initLang.GetComboxInfo("holidaysType");
				if (this.m_typeDic == null || this.m_typeDic.Count == 0)
				{
					this.m_typeDic = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("1", "假日类型1");
					dictionary.Add("2", "假日类型2");
					dictionary.Add("3", "假日类型3");
					this.m_typeDic.Add("0", dictionary);
					initLang.SetComboxInfo("holidaysType", this.m_typeDic);
					initLang.Save();
				}
				if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary2 = this.m_typeDic["0"];
					foreach (KeyValuePair<string, string> item in dictionary2)
					{
						this.cmb_type.Items.Add(item.Value);
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccHolidays accHolidays = null;
					AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
					accHolidays = accHolidaysBll.GetModel(this.m_id);
					if (accHolidays != null)
					{
						this.oldHoilday = accHolidays.Copy();
						this.txt_name.Text = accHolidays.holiday_name;
						this.txt_Remarks.Text = accHolidays.memo;
						this.dt_end.EditValue = accHolidays.end_date.Value;
						this.dt_start.EditValue = accHolidays.start_date.Value;
						if (accHolidays.loop_by_year == 1)
						{
							this.ch_loop.Checked = true;
						}
						else
						{
							this.ch_loop.Checked = false;
						}
						if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
						{
							Dictionary<string, string> dictionary = this.m_typeDic["0"];
							Dictionary<string, string> dictionary2 = dictionary;
							int holiday_type = accHolidays.holiday_type;
							if (dictionary2.ContainsKey(holiday_type.ToString()))
							{
								System.Windows.Forms.ComboBox comboBox = this.cmb_type;
								Dictionary<string, string> dictionary3 = dictionary;
								holiday_type = accHolidays.holiday_type;
								comboBox.Text = dictionary3[holiday_type.ToString()];
							}
						}
						this.cbbHolidayNo.SelectedValue = accHolidays.HolidayNo;
						this.cbbHolidayTZ.SelectedValue = accHolidays.HolidayTZ;
					}
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

		private void BindModel(AccHolidays hliday)
		{
			try
			{
				hliday.holiday_name = this.txt_name.Text;
				hliday.memo = this.txt_Remarks.Text;
				hliday.end_date = (DateTime?)this.dt_end.EditValue;
				hliday.start_date = (DateTime?)this.dt_start.EditValue;
				if (this.ch_loop.Checked)
				{
					hliday.loop_by_year = 1;
				}
				else
				{
					hliday.loop_by_year = 2;
				}
				if (this.m_typeDic != null && this.m_typeDic.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary = this.m_typeDic["0"];
					foreach (KeyValuePair<string, string> item in dictionary)
					{
						if (item.Value == this.cmb_type.Text)
						{
							hliday.holiday_type = int.Parse(item.Key);
							break;
						}
					}
				}
				hliday.HolidayNo = (int)(this.cbbHolidayNo.SelectedValue ?? ((object)0));
				hliday.HolidayTZ = (int)(this.cbbHolidayTZ.SelectedValue ?? ((object)0));
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool HolidayTypeCountCheck()
		{
			try
			{
				int num2;
				int num;
				int num3 = num2 = (num = 0);
				AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
				List<AccHolidays> modelList = accHolidaysBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].holiday_type == 1)
						{
							num3++;
						}
						else if (modelList[i].holiday_type == 2)
						{
							num2++;
						}
						else if (modelList[i].holiday_type == 3)
						{
							num++;
						}
					}
				}
				if (num3 > 32 || num2 > 32 || num > 32)
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool check()
		{
			try
			{
				DateTime dateTime = (DateTime)this.dt_start.EditValue;
				DateTime dateTime2 = (DateTime)this.dt_end.EditValue;
				int num = dateTime2.Year * 10000 + dateTime2.Month * 100 + dateTime2.Day;
				int num2 = dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
				int year = dateTime.Year;
				DateTime dateTime3 = DateTime.Now;
				if (year < dateTime3.Year)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SStartDateError", "开始日期设置错误"));
					this.dt_start.Focus();
					return false;
				}
				if (dateTime2.Year != dateTime.Year)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SHolidayTwoYear", "节假日日期不能跨年"));
					this.dt_end.Focus();
					return false;
				}
				if (num < num2)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SStartDateBiger", "开始日期不能大于结束日期"));
					this.dt_end.Focus();
					return false;
				}
				if (!this.HolidayTypeCountCheck())
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SHolidayTypeCount", "每种假日类型包含的节假日数量不能超过") + 32.ToString());
					this.cmb_type.Focus();
					return false;
				}
				AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
				List<AccHolidays> modelList = accHolidaysBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!(this.m_id.ToString() == modelList[i].id))
						{
							int num3 = 0;
							int num4 = 0;
							int num5 = 0;
							int num6 = 0;
							if (modelList[i].loop_by_year == 1 || this.ch_loop.Checked)
							{
								num3 = dateTime.Month * 100 + dateTime.Day;
								num4 = dateTime2.Month * 100 + dateTime2.Day;
								dateTime3 = modelList[i].start_date.Value;
								int num7 = dateTime3.Month * 100;
								dateTime3 = modelList[i].start_date.Value;
								num5 = num7 + dateTime3.Day;
								dateTime3 = modelList[i].end_date.Value;
								int num8 = dateTime3.Month * 100;
								dateTime3 = modelList[i].end_date.Value;
								num6 = num8 + dateTime3.Day;
							}
							else
							{
								num3 = dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
								num4 = dateTime2.Year * 10000 + dateTime2.Month * 100 + dateTime2.Day;
								dateTime3 = modelList[i].start_date.Value;
								int num9 = dateTime3.Year * 10000;
								dateTime3 = modelList[i].start_date.Value;
								int num10 = num9 + dateTime3.Month * 100;
								dateTime3 = modelList[i].start_date.Value;
								num5 = num10 + dateTime3.Day;
								dateTime3 = modelList[i].end_date.Value;
								int num11 = dateTime3.Year * 10000;
								dateTime3 = modelList[i].end_date.Value;
								int num12 = num11 + dateTime3.Month * 100;
								dateTime3 = modelList[i].end_date.Value;
								num6 = num12 + dateTime3.Day;
							}
							if (num3 >= num5 && num3 <= num6)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SData", "日期:") + dateTime.Month + "-" + dateTime.Day + ShowMsgInfos.GetInfo("SDateHasSet", "已经被设置为节假日，不能重复设置!"));
								this.dt_start.Focus();
								return false;
							}
							if (num4 >= num5 && num4 <= num6)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SData", "日期:") + dateTime2.Month + "-" + dateTime2.Day + ShowMsgInfos.GetInfo("SDateHasSet", "已经被设置为节假日，不能重复设置!"));
								this.dt_start.Focus();
								return false;
							}
							if (num3 <= num5 && num4 >= num6)
							{
								SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SHolidayRepeat", "开始日期和结束日期时段内已经被设置为节假日，不能重复设置!"));
								this.dt_start.Focus();
								return false;
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(this.txt_name.Text.Trim()))
				{
					return true;
				}
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
				this.txt_name.Focus();
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool Save()
		{
			try
			{
				AccHolidays accHolidays = null;
				AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accHolidays = accHolidaysBll.GetModel(this.m_id);
				}
				if (accHolidays == null)
				{
					accHolidays = new AccHolidays();
					if (accHolidaysBll.Exists(this.txt_name.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "门禁节假日名称设置重复"));
						this.txt_name.Focus();
						return false;
					}
				}
				else if (accHolidays.holiday_name != this.txt_name.Text && accHolidaysBll.Exists(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "门禁节假日名称设置重复"));
					this.txt_name.Focus();
					return false;
				}
				this.BindModel(accHolidays);
				if (this.m_id > 0)
				{
					if (accHolidaysBll.Update(accHolidays))
					{
						if (this.HolidayChanged(accHolidays, this.oldHoilday))
						{
							if (this.oldHoilday != null)
							{
								CommandServer.DelCmd(this.oldHoilday);
							}
							if (accHolidays.HolidayTZ > 0 && accHolidays.HolidayTZ != this.oldHoilday.HolidayTZ)
							{
								AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
								List<AccTimeseg> modelList = accTimesegBll.GetModelList(string.Format("TimeZone1Id = {0} or TimeZone2Id = {0} or TimeZone3Id = {0} or TimeZoneHolidayId = {0}", accHolidays.HolidayTZ));
								if (modelList != null && modelList.Count > 0)
								{
									MachinesBll machinesBll = new MachinesBll(MainForm._ia);
									List<Machines> modelList2 = machinesBll.GetModelList("DevSDKType = 2");
									if (modelList2 != null)
									{
										for (int i = 0; i < modelList2.Count; i++)
										{
											CommandServer.AddTimeZone(modelList2[i], modelList);
										}
									}
								}
							}
							CommandServer.AddCmd(accHolidays);
							FrmShowUpdata.Instance.ShowEx();
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
						if (accHolidaysBll.Add(accHolidays) > 0)
						{
							if (accHolidays.HolidayTZ > 0)
							{
								AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
								List<AccTimeseg> modelList = accTimesegBll.GetModelList(string.Format("TimeZone1Id = {0} or TimeZone2Id = {0} or TimeZone3Id = {0} or TimeZoneHolidayId = {0}", accHolidays.HolidayTZ));
								if (modelList != null && modelList.Count > 0)
								{
									MachinesBll machinesBll = new MachinesBll(MainForm._ia);
									List<Machines> modelList2 = machinesBll.GetModelList("DevSDKType = 2");
									if (modelList2 != null)
									{
										for (int j = 0; j < modelList2.Count; j++)
										{
											CommandServer.AddTimeZone(modelList2[j], modelList);
										}
									}
								}
							}
							CommandServer.AddCmd(accHolidays);
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							FrmShowUpdata.Instance.ShowEx();
							this.txt_name.Text = "";
							return true;
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDataSaveException", "保存数据出现异常") + ":" + ex.Message);
			}
			return false;
		}

		private bool HolidayChanged(AccHolidays newHoldiay, AccHolidays oldHoliday)
		{
			if (newHoldiay != null && oldHoliday != null && (newHoldiay.holiday_type != this.oldHoilday.holiday_type || newHoldiay.start_date != this.oldHoilday.start_date || newHoldiay.end_date != this.oldHoilday.end_date || newHoldiay.loop_by_year != this.oldHoilday.loop_by_year || newHoldiay.HolidayNo != this.oldHoilday.HolidayNo || newHoldiay.HolidayTZ != this.oldHoilday.HolidayTZ))
			{
				return true;
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

		private void txt_Remarks_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 25);
		}

		private void LoadAllHolidayNo()
		{
			try
			{
				this.lstExistsNo = new List<int>();
				AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
				List<AccHolidays> modelList = accHolidaysBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (!this.lstExistsNo.Contains(modelList[i].HolidayNo))
						{
							this.lstExistsNo.Add(modelList[i].HolidayNo);
						}
						if (modelList[i].id == this.m_id.ToString())
						{
							this.Holiday = modelList[i];
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void LoadAllTimeZoneNo()
		{
			try
			{
				this.lstExistsTZ = new List<int>();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null && modelList.Count > 0)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].TimeZone1Id > 0 && !this.lstExistsTZ.Contains(modelList[i].TimeZone1Id))
						{
							this.lstExistsTZ.Add(modelList[i].TimeZone1Id);
						}
						if (modelList[i].TimeZone2Id > 0 && !this.lstExistsTZ.Contains(modelList[i].TimeZone2Id))
						{
							this.lstExistsTZ.Add(modelList[i].TimeZone2Id);
						}
						if (modelList[i].TimeZone3Id > 0 && !this.lstExistsTZ.Contains(modelList[i].TimeZone3Id))
						{
							this.lstExistsTZ.Add(modelList[i].TimeZone3Id);
						}
						if (modelList[i].TimeZoneHolidayId > 0 && !this.lstExistsTZ.Contains(modelList[i].TimeZoneHolidayId))
						{
							this.lstExistsTZ.Add(modelList[i].TimeZoneHolidayId);
						}
					}
					this.lstExistsTZ.Sort();
				}
			}
			catch (Exception)
			{
			}
		}

		private void GenerateDataSource()
		{
			this.dtHolidayNo = new DataTable();
			this.dtHolidayNo.Columns.Add("Value", typeof(int));
			this.dtHolidayNo.Columns.Add("Text", typeof(string));
			DataRow dataRow = this.dtHolidayNo.NewRow();
			dataRow["Value"] = 0;
			dataRow["Text"] = "--------------";
			this.dtHolidayNo.Rows.Add(dataRow);
			this.dtHolidayTZ = this.dtHolidayNo.Copy();
			for (int i = 1; i <= 24; i++)
			{
				if (this.Holiday == null)
				{
					if (!this.lstExistsNo.Contains(i))
					{
						goto IL_00f5;
					}
				}
				else if (!this.lstExistsNo.Contains(i) || i == this.Holiday.HolidayNo)
				{
					goto IL_00f5;
				}
				continue;
				IL_00f5:
				dataRow = this.dtHolidayNo.NewRow();
				dataRow["Value"] = i;
				dataRow["Text"] = i.ToString();
				this.dtHolidayNo.Rows.Add(dataRow);
			}
			for (int j = 0; j < this.lstExistsTZ.Count; j++)
			{
				dataRow = this.dtHolidayTZ.NewRow();
				dataRow["Value"] = this.lstExistsTZ[j];
				dataRow["Text"] = this.lstExistsTZ[j].ToString();
				this.dtHolidayTZ.Rows.Add(dataRow);
			}
			this.cbbHolidayNo.DataSource = this.dtHolidayNo;
			this.cbbHolidayNo.ValueMember = "Value";
			this.cbbHolidayNo.DisplayMember = "Text";
			this.cbbHolidayTZ.DataSource = this.dtHolidayTZ;
			this.cbbHolidayTZ.ValueMember = "Value";
			this.cbbHolidayTZ.DisplayMember = "Text";
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(HolidaysEdit));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.txt_Remarks = new TextBox();
			this.txt_name = new TextBox();
			this.lal_Remarks = new Label();
			this.lb_type = new Label();
			this.lb_name = new Label();
			this.cmb_type = new System.Windows.Forms.ComboBox();
			this.lb_startdate = new Label();
			this.lb_enddate = new Label();
			this.lb_loop = new Label();
			this.ch_loop = new CheckBox();
			this.label5 = new Label();
			this.label6 = new Label();
			this.label7 = new Label();
			this.label8 = new Label();
			this.dt_start = new DateEdit();
			this.dt_end = new DateEdit();
			this.lblHolidayNo = new Label();
			this.cbbHolidayNo = new System.Windows.Forms.ComboBox();
			this.cbbHolidayTZ = new System.Windows.Forms.ComboBox();
			this.lblHolidayTZ = new Label();
			this.groupBox1 = new GroupBox();
			((ISupportInitialize)this.dt_start.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.dt_start.Properties).BeginInit();
			((ISupportInitialize)this.dt_end.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.dt_end.Properties).BeginInit();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(255, 302);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 7;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(153, 302);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 6;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.txt_Remarks.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.txt_Remarks.Location = new Point(169, 169);
			this.txt_Remarks.Name = "txt_Remarks";
			this.txt_Remarks.Size = new Size(151, 21);
			this.txt_Remarks.TabIndex = 5;
			this.txt_Remarks.KeyPress += this.txt_Remarks_KeyPress;
			this.txt_name.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.txt_name.Location = new Point(169, 16);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(151, 21);
			this.txt_name.TabIndex = 0;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.lal_Remarks.Location = new Point(12, 172);
			this.lal_Remarks.Name = "lal_Remarks";
			this.lal_Remarks.Size = new Size(91, 12);
			this.lal_Remarks.TabIndex = 13;
			this.lal_Remarks.Text = "备注";
			this.lal_Remarks.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_type.Location = new Point(12, 51);
			this.lb_type.Name = "lb_type";
			this.lb_type.Size = new Size(91, 12);
			this.lb_type.TabIndex = 12;
			this.lb_type.Text = "节假日类型";
			this.lb_type.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_name.Location = new Point(12, 20);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(91, 12);
			this.lb_name.TabIndex = 11;
			this.lb_name.Text = "节假日名称";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_type.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cmb_type.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_type.FormattingEnabled = true;
			this.cmb_type.Items.AddRange(new object[3]
			{
				"假日类型1",
				"假日类型2",
				"假日类型3"
			});
			this.cmb_type.Location = new Point(169, 47);
			this.cmb_type.Name = "cmb_type";
			this.cmb_type.Size = new Size(151, 20);
			this.cmb_type.TabIndex = 1;
			this.lb_startdate.Location = new Point(12, 83);
			this.lb_startdate.Name = "lb_startdate";
			this.lb_startdate.Size = new Size(91, 12);
			this.lb_startdate.TabIndex = 21;
			this.lb_startdate.Text = "开始日期";
			this.lb_startdate.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_enddate.Location = new Point(12, 114);
			this.lb_enddate.Name = "lb_enddate";
			this.lb_enddate.Size = new Size(91, 12);
			this.lb_enddate.TabIndex = 22;
			this.lb_enddate.Text = "结束日期";
			this.lb_enddate.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_loop.Location = new Point(12, 145);
			this.lb_loop.Name = "lb_loop";
			this.lb_loop.Size = new Size(91, 12);
			this.lb_loop.TabIndex = 25;
			this.lb_loop.Text = "按年循环";
			this.lb_loop.TextAlign = ContentAlignment.MiddleLeft;
			this.ch_loop.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.ch_loop.AutoSize = true;
			this.ch_loop.Location = new Point(169, 143);
			this.ch_loop.Name = "ch_loop";
			this.ch_loop.Size = new Size(15, 14);
			this.ch_loop.TabIndex = 4;
			this.ch_loop.UseVisualStyleBackColor = true;
			this.label5.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label5.AutoSize = true;
			this.label5.ForeColor = Color.Red;
			this.label5.Location = new Point(326, 114);
			this.label5.Name = "label5";
			this.label5.Size = new Size(11, 12);
			this.label5.TabIndex = 28;
			this.label5.Text = "*";
			this.label6.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label6.AutoSize = true;
			this.label6.ForeColor = Color.Red;
			this.label6.Location = new Point(326, 83);
			this.label6.Name = "label6";
			this.label6.Size = new Size(11, 12);
			this.label6.TabIndex = 29;
			this.label6.Text = "*";
			this.label7.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label7.AutoSize = true;
			this.label7.ForeColor = Color.Red;
			this.label7.Location = new Point(326, 51);
			this.label7.Name = "label7";
			this.label7.Size = new Size(11, 12);
			this.label7.TabIndex = 30;
			this.label7.Text = "*";
			this.label8.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.label8.AutoSize = true;
			this.label8.ForeColor = Color.Red;
			this.label8.Location = new Point(326, 20);
			this.label8.Name = "label8";
			this.label8.Size = new Size(11, 12);
			this.label8.TabIndex = 31;
			this.label8.Text = "*";
			this.dt_start.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.dt_start.EditValue = null;
			this.dt_start.Location = new Point(169, 78);
			this.dt_start.Name = "dt_start";
			this.dt_start.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dt_start.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.dt_start.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
			this.dt_start.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dt_start.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dt_start.Size = new Size(151, 21);
			this.dt_start.TabIndex = 2;
			this.dt_end.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.dt_end.EditValue = null;
			this.dt_end.Location = new Point(169, 109);
			this.dt_end.Name = "dt_end";
			this.dt_end.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dt_end.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.dt_end.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dt_end.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dt_end.Size = new Size(151, 21);
			this.dt_end.TabIndex = 3;
			this.lblHolidayNo.Location = new Point(2, 28);
			this.lblHolidayNo.Name = "lblHolidayNo";
			this.lblHolidayNo.Size = new Size(149, 12);
			this.lblHolidayNo.TabIndex = 33;
			this.lblHolidayNo.Text = "假日编号";
			this.lblHolidayNo.TextAlign = ContentAlignment.MiddleLeft;
			this.cbbHolidayNo.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cbbHolidayNo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbbHolidayNo.FormattingEnabled = true;
			this.cbbHolidayNo.Location = new Point(157, 24);
			this.cbbHolidayNo.Name = "cbbHolidayNo";
			this.cbbHolidayNo.Size = new Size(151, 20);
			this.cbbHolidayNo.TabIndex = 34;
			this.cbbHolidayTZ.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.cbbHolidayTZ.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbbHolidayTZ.FormattingEnabled = true;
			this.cbbHolidayTZ.Location = new Point(157, 54);
			this.cbbHolidayTZ.Name = "cbbHolidayTZ";
			this.cbbHolidayTZ.Size = new Size(151, 20);
			this.cbbHolidayTZ.TabIndex = 36;
			this.lblHolidayTZ.Location = new Point(2, 58);
			this.lblHolidayTZ.Name = "lblHolidayTZ";
			this.lblHolidayTZ.Size = new Size(149, 12);
			this.lblHolidayTZ.TabIndex = 35;
			this.lblHolidayTZ.Text = "假日时段";
			this.lblHolidayTZ.TextAlign = ContentAlignment.MiddleLeft;
			this.groupBox1.Controls.Add(this.lblHolidayNo);
			this.groupBox1.Controls.Add(this.cbbHolidayTZ);
			this.groupBox1.Controls.Add(this.cbbHolidayNo);
			this.groupBox1.Controls.Add(this.lblHolidayTZ);
			this.groupBox1.Location = new Point(12, 199);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(328, 87);
			this.groupBox1.TabIndex = 37;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "脱机参数";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(350, 337);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.dt_end);
			base.Controls.Add(this.dt_start);
			base.Controls.Add(this.txt_Remarks);
			base.Controls.Add(this.cmb_type);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.ch_loop);
			base.Controls.Add(this.lb_loop);
			base.Controls.Add(this.lb_enddate);
			base.Controls.Add(this.lb_startdate);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lal_Remarks);
			base.Controls.Add(this.lb_type);
			base.Controls.Add(this.lb_name);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "HolidaysEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			((ISupportInitialize)this.dt_start.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.dt_start.Properties).EndInit();
			((ISupportInitialize)this.dt_end.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.dt_end.Properties).EndInit();
			this.groupBox1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
