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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZK.Access.mensage;
using ZK.Access.Properties;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Data.Model.StdSDK;
using ZK.Utils;

namespace ZK.Access.door
{
	public class TimeSet : Office2007Form
	{
		private int m_id = 0;

		private bool m_isShow = false;

		private TimeInfo m_lastSelectTime = null;

		private bool isLock = false;

		private AccTimeseg OldTimeZone;

		private List<int> lstTimeZoneExists;

		private DataTable dtTimeZone1;

		private DataTable dtTimeZone2;

		private DataTable dtTimeZone3;

		private DataTable dtTimeZoneHoliday;

		private bool SelectedValueChanging = false;

		private IContainer components = null;

		private TextBox txt_remark;

		private TextBox txt_name;

		private Label lb_remark;

		private Label lb_name;

		private Label lb_hliday3;

		private Label lb_hliday2;

		private Label lb_hliday1;

		private Label lb_sunday;

		private Label lb_saturday;

		private Label lb_friday;

		private Label lb_thursday;

		private Label lb_wednesday;

		private Label lb_tuesday;

		private Label lb_monday;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_satrt;

		private Label lb_end;

		private Panel panel1;

		private TimeControl time_10;

		private TimeControl time_9;

		private TimeControl time_8;

		private TimeControl time_7;

		private TimeControl time_1;

		private TimeControl time_6;

		private TimeControl time_5;

		private TimeControl time_3;

		private TimeControl time_4;

		private ButtonX btn_help;

		private Label label2;

		private TimeControl time_2;

		private DateEdit txt_start;

		private DateEdit txt_end;

		private System.Windows.Forms.ComboBox cbbTZ1Id;

		private Label lblTZ1;

		private Label lblTZ2;

		private System.Windows.Forms.ComboBox cbbTZ2Id;

		private Label lblTZ3;

		private System.Windows.Forms.ComboBox cbbTZ3Id;

		private Label lblTZHoliday;

		private System.Windows.Forms.ComboBox cbbTZHolidayId;

		private GroupBox groupBox1;

		private Button btn_help_diferences;

		private EventHandler txt_name_TextChanged;

		public event EventHandler RefreshDataEvent = null;

		public TimeSet(int id)
		{
			this.InitializeComponent();
			this.isLock = true;
			DateEdit dateEdit = this.txt_end;
			DateTime dateTime = DateTime.Now;
			DateTime now = DateTime.Now;
			dateTime = dateTime.AddHours((double)(-now.Hour));
			now = DateTime.Now;
			dateTime = dateTime.AddMinutes((double)(-now.Minute));
			now = DateTime.Now;
			dateEdit.EditValue = dateTime.AddSeconds((double)(-now.Second));
			this.txt_start.EditValue = this.txt_end.EditValue;
			this.isLock = false;
			this.m_id = id;
			initLang.LocaleForm(this, base.Name);
			this.LoadAllTimeZone();
			this.GenerateDataSource();
			this.BindData();
			this.time_1.SelectIndex = 0;
			this.time_1_SelectIndexChangedEvent(this.time_1, null);
			this.txt_start.LostFocus += this.txt_start_LostFocus;
			this.txt_end.LostFocus += this.txt_start_LostFocus;
			this.lb_satrt_SizeChanged(null, null);
			if (this.txt_name.Text == ShowMsgInfos.GetInfo("SDefaultTZ", "24小时通行"))
			{
				this.btn_OK.Enabled = false;
				this.txt_name.Enabled = false;
			}
			this.cbbTZ1Id.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
			this.cbbTZ2Id.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
			this.cbbTZ3Id.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
			this.cbbTZHolidayId.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
		}

		private void txt_start_LostFocus(object sender, EventArgs e)
		{
			this.SetTime(sender, e);
		}

		private void BindData()
		{
			try
			{
				if (this.m_id > 0)
				{
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
					AccTimeseg accTimeseg = null;
					AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
					accTimeseg = accTimesegBll.GetModel(this.m_id);
					this.OldTimeZone = accTimeseg.Copy();
					if (accTimeseg != null)
					{
						this.txt_name.Text = accTimeseg.timeseg_name;
						this.txt_remark.Text = accTimeseg.memo;
						int minite = this.GetMinite(accTimeseg.monday_start1.Value);
						int minite2 = this.GetMinite(accTimeseg.monday_end1.Value);
						if (minite2 != minite)
						{
							this.time_1.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.monday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.monday_end2.Value);
						if (minite2 != minite)
						{
							this.time_1.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.monday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.monday_end3.Value);
						if (minite2 != minite)
						{
							this.time_1.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.tuesday_start1.Value);
						minite2 = this.GetMinite(accTimeseg.tuesday_end1.Value);
						if (minite2 != minite)
						{
							this.time_2.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.tuesday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.tuesday_end2.Value);
						if (minite2 != minite)
						{
							this.time_2.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.tuesday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.tuesday_end3.Value);
						if (minite2 != minite)
						{
							this.time_2.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.wednesday_start1.Value);
						minite2 = this.GetMinite(accTimeseg.wednesday_end1.Value);
						if (minite2 != minite)
						{
							this.time_3.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.wednesday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.wednesday_end2.Value);
						if (minite2 != minite)
						{
							this.time_3.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.wednesday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.wednesday_end3.Value);
						if (minite2 != minite)
						{
							this.time_3.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.thursday_start1.Value);
						minite2 = this.GetMinite(accTimeseg.thursday_end1.Value);
						if (minite2 != minite)
						{
							this.time_4.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.thursday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.thursday_end2.Value);
						if (minite2 != minite)
						{
							this.time_4.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.thursday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.thursday_end3.Value);
						if (minite2 != minite)
						{
							this.time_4.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.friday_start1.Value);
						minite2 = this.GetMinite(accTimeseg.friday_end1.Value);
						if (minite2 != minite)
						{
							this.time_5.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.friday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.friday_end2.Value);
						if (minite2 != minite)
						{
							this.time_5.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.friday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.friday_end3.Value);
						if (minite2 != minite)
						{
							this.time_5.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.saturday_start1.Value);
						minite2 = this.GetMinite(accTimeseg.saturday_end1.Value);
						if (minite2 != minite)
						{
							this.time_6.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.saturday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.saturday_end2.Value);
						if (minite2 != minite)
						{
							this.time_6.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.saturday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.saturday_end3.Value);
						if (minite2 != minite)
						{
							this.time_6.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.sunday_start1.Value);
						minite2 = this.GetMinite(accTimeseg.sunday_end1.Value);
						if (minite2 != minite)
						{
							this.time_7.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.sunday_start2.Value);
						minite2 = this.GetMinite(accTimeseg.sunday_end2.Value);
						if (minite2 != minite)
						{
							this.time_7.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.sunday_start3.Value);
						minite2 = this.GetMinite(accTimeseg.sunday_end3.Value);
						if (minite2 != minite)
						{
							this.time_7.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype1_start1.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype1_end1.Value);
						if (minite2 != minite)
						{
							this.time_8.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype1_start2.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype1_end2.Value);
						if (minite2 != minite)
						{
							this.time_8.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype1_start3.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype1_end3.Value);
						if (minite2 != minite)
						{
							this.time_8.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype2_start1.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype2_end1.Value);
						if (minite2 != minite)
						{
							this.time_9.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype2_start2.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype2_end2.Value);
						if (minite2 != minite)
						{
							this.time_9.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype2_start3.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype2_end3.Value);
						if (minite2 != minite)
						{
							this.time_9.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype3_start1.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype3_end1.Value);
						if (minite2 != minite)
						{
							this.time_10.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype3_start2.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype3_end2.Value);
						if (minite2 != minite)
						{
							this.time_10.AddTime(minite, minite2);
						}
						minite = this.GetMinite(accTimeseg.holidaytype3_start3.Value);
						minite2 = this.GetMinite(accTimeseg.holidaytype3_end3.Value);
						if (minite2 != minite)
						{
							this.time_10.AddTime(minite, minite2);
						}
						this.cbbTZ1Id.SelectedValue = accTimeseg.TimeZone1Id;
						this.cbbTZ2Id.SelectedValue = accTimeseg.TimeZone2Id;
						this.cbbTZ3Id.SelectedValue = accTimeseg.TimeZone3Id;
						this.cbbTZHolidayId.SelectedValue = accTimeseg.TimeZoneHolidayId;
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

		public static DateTime GetDate(int minite)
		{
			return AccTimesegBll.GetDate(minite);
		}

		public int GetMinite(DateTime date)
		{
			return date.Hour * 60 + date.Minute;
		}

		private void BindModel(AccTimeseg time)
		{
			try
			{
				if (time != null)
				{
					time.timeseg_name = this.txt_name.Text;
					time.memo = this.txt_remark.Text;
					List<TimeInfo> times = this.time_1.Times;
					time.monday_start1 = TimeSet.GetDate(0);
					time.monday_end1 = TimeSet.GetDate(0);
					time.monday_start2 = TimeSet.GetDate(0);
					time.monday_end2 = TimeSet.GetDate(0);
					time.monday_start3 = TimeSet.GetDate(0);
					time.monday_end3 = TimeSet.GetDate(0);
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.monday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.monday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.monday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.monday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.monday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.monday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.monday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.monday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.monday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.monday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.monday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.monday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.tuesday_start1 = TimeSet.GetDate(0);
					time.tuesday_end1 = TimeSet.GetDate(0);
					time.tuesday_start2 = TimeSet.GetDate(0);
					time.tuesday_end2 = TimeSet.GetDate(0);
					time.tuesday_start3 = TimeSet.GetDate(0);
					time.tuesday_end3 = TimeSet.GetDate(0);
					times = this.time_2.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.tuesday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.tuesday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.tuesday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.tuesday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.tuesday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.tuesday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.tuesday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.tuesday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.tuesday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.tuesday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.tuesday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.tuesday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.wednesday_start1 = TimeSet.GetDate(0);
					time.wednesday_end1 = TimeSet.GetDate(0);
					time.wednesday_start2 = TimeSet.GetDate(0);
					time.wednesday_end2 = TimeSet.GetDate(0);
					time.wednesday_start3 = TimeSet.GetDate(0);
					time.wednesday_end3 = TimeSet.GetDate(0);
					times = this.time_3.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.wednesday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.wednesday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.wednesday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.wednesday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.wednesday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.wednesday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.wednesday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.wednesday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.wednesday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.wednesday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.wednesday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.wednesday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.thursday_start1 = TimeSet.GetDate(0);
					time.thursday_end1 = TimeSet.GetDate(0);
					time.thursday_start2 = TimeSet.GetDate(0);
					time.thursday_end2 = TimeSet.GetDate(0);
					time.thursday_start3 = TimeSet.GetDate(0);
					time.thursday_end3 = TimeSet.GetDate(0);
					times = this.time_4.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.thursday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.thursday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.thursday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.thursday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.thursday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.thursday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.thursday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.thursday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.thursday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.thursday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.thursday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.thursday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.friday_start1 = TimeSet.GetDate(0);
					time.friday_end1 = TimeSet.GetDate(0);
					time.friday_start2 = TimeSet.GetDate(0);
					time.friday_end2 = TimeSet.GetDate(0);
					time.friday_start3 = TimeSet.GetDate(0);
					time.friday_end3 = TimeSet.GetDate(0);
					times = this.time_5.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.friday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.friday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.friday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.friday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.friday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.friday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.friday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.friday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.friday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.friday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.friday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.friday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.saturday_start1 = TimeSet.GetDate(0);
					time.saturday_end1 = TimeSet.GetDate(0);
					time.saturday_start2 = TimeSet.GetDate(0);
					time.saturday_end2 = TimeSet.GetDate(0);
					time.saturday_start3 = TimeSet.GetDate(0);
					time.saturday_end3 = TimeSet.GetDate(0);
					times = this.time_6.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.saturday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.saturday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.saturday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.saturday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.saturday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.saturday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.saturday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.saturday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.saturday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.saturday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.saturday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.saturday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.sunday_start1 = TimeSet.GetDate(0);
					time.sunday_end1 = TimeSet.GetDate(0);
					time.sunday_start2 = TimeSet.GetDate(0);
					time.sunday_end2 = TimeSet.GetDate(0);
					time.sunday_start3 = TimeSet.GetDate(0);
					time.sunday_end3 = TimeSet.GetDate(0);
					times = this.time_7.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.sunday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.sunday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.sunday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.sunday_end2 = TimeSet.GetDate(times[1].EndTime);
							time.sunday_start3 = TimeSet.GetDate(times[2].StartTime);
							time.sunday_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.sunday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.sunday_end1 = TimeSet.GetDate(times[0].EndTime);
							time.sunday_start2 = TimeSet.GetDate(times[1].StartTime);
							time.sunday_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.sunday_start1 = TimeSet.GetDate(times[0].StartTime);
							time.sunday_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.holidaytype1_start1 = TimeSet.GetDate(0);
					time.holidaytype1_end1 = TimeSet.GetDate(0);
					time.holidaytype1_start2 = TimeSet.GetDate(0);
					time.holidaytype1_end2 = TimeSet.GetDate(0);
					time.holidaytype1_start3 = TimeSet.GetDate(0);
					time.holidaytype1_end3 = TimeSet.GetDate(0);
					times = this.time_8.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.holidaytype1_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype1_end1 = TimeSet.GetDate(times[0].EndTime);
							time.holidaytype1_start2 = TimeSet.GetDate(times[1].StartTime);
							time.holidaytype1_end2 = TimeSet.GetDate(times[1].EndTime);
							time.holidaytype1_start3 = TimeSet.GetDate(times[2].StartTime);
							time.holidaytype1_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.holidaytype1_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype1_end1 = TimeSet.GetDate(times[0].EndTime);
							time.holidaytype1_start2 = TimeSet.GetDate(times[1].StartTime);
							time.holidaytype1_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.holidaytype1_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype1_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.holidaytype2_start1 = TimeSet.GetDate(0);
					time.holidaytype2_end1 = TimeSet.GetDate(0);
					time.holidaytype2_start2 = TimeSet.GetDate(0);
					time.holidaytype2_end2 = TimeSet.GetDate(0);
					time.holidaytype2_start3 = TimeSet.GetDate(0);
					time.holidaytype2_end3 = TimeSet.GetDate(0);
					times = this.time_9.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.holidaytype2_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype2_end1 = TimeSet.GetDate(times[0].EndTime);
							time.holidaytype2_start2 = TimeSet.GetDate(times[1].StartTime);
							time.holidaytype2_end2 = TimeSet.GetDate(times[1].EndTime);
							time.holidaytype2_start3 = TimeSet.GetDate(times[2].StartTime);
							time.holidaytype2_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.holidaytype2_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype2_end1 = TimeSet.GetDate(times[0].EndTime);
							time.holidaytype2_start2 = TimeSet.GetDate(times[1].StartTime);
							time.holidaytype2_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.holidaytype2_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype2_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					time.holidaytype3_start1 = TimeSet.GetDate(0);
					time.holidaytype3_end1 = TimeSet.GetDate(0);
					time.holidaytype3_start2 = TimeSet.GetDate(0);
					time.holidaytype3_end2 = TimeSet.GetDate(0);
					time.holidaytype3_start3 = TimeSet.GetDate(0);
					time.holidaytype3_end3 = TimeSet.GetDate(0);
					times = this.time_10.Times;
					if (times != null && times.Count > 0)
					{
						if (times.Count == 3)
						{
							time.holidaytype3_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype3_end1 = TimeSet.GetDate(times[0].EndTime);
							time.holidaytype3_start2 = TimeSet.GetDate(times[1].StartTime);
							time.holidaytype3_end2 = TimeSet.GetDate(times[1].EndTime);
							time.holidaytype3_start3 = TimeSet.GetDate(times[2].StartTime);
							time.holidaytype3_end3 = TimeSet.GetDate(times[2].EndTime);
						}
						else if (times.Count == 2)
						{
							time.holidaytype3_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype3_end1 = TimeSet.GetDate(times[0].EndTime);
							time.holidaytype3_start2 = TimeSet.GetDate(times[1].StartTime);
							time.holidaytype3_end2 = TimeSet.GetDate(times[1].EndTime);
						}
						else
						{
							time.holidaytype3_start1 = TimeSet.GetDate(times[0].StartTime);
							time.holidaytype3_end1 = TimeSet.GetDate(times[0].EndTime);
						}
					}
					this.AutoSetStdTimeZoneId(this.cbbTZ1Id, time);
					this.AutoSetStdTimeZoneId(this.cbbTZ2Id, time);
					this.AutoSetStdTimeZoneId(this.cbbTZ3Id, time);
					this.AutoSetStdTimeZoneId(this.cbbTZHolidayId, time);
					int.TryParse((this.cbbTZ1Id.SelectedValue ?? ((object)0)).ToString(), out int num);
					time.TimeZone1Id = num;
					int.TryParse((this.cbbTZ2Id.SelectedValue ?? ((object)0)).ToString(), out num);
					time.TimeZone2Id = num;
					int.TryParse((this.cbbTZ3Id.SelectedValue ?? ((object)0)).ToString(), out num);
					time.TimeZone3Id = num;
					int.TryParse((this.cbbTZHolidayId.SelectedValue ?? ((object)0)).ToString(), out num);
					time.TimeZoneHolidayId = num;
					if (time.TimeZone1Id == 0 && time.TimeZone2Id == 0 && time.TimeZone3Id == 0 && time.TimeZoneHolidayId == 0)
					{
						DataTable dataTable = this.cbbTZ1Id.DataSource as DataTable;
						if (dataTable != null && dataTable.Rows.Count > 1)
						{
							this.cbbTZ1Id.SelectedValue = dataTable.Rows[1]["Value"];
							int.TryParse((this.cbbTZ1Id.SelectedValue ?? ((object)0)).ToString(), out num);
							time.TimeZone1Id = num;
						}
					}
				}
			}
			catch
			{
			}
		}

		private AccTimeseg GetOldMode(AccTimeseg model)
		{
			AccTimeseg accTimeseg = new AccTimeseg();
			return model.Copy();
		}

		private bool ModelIsChanged(AccTimeseg oldMode, AccTimeseg model)
		{
			if (oldMode.friday_end1 == model.friday_end1 && oldMode.friday_end2 == model.friday_end2 && oldMode.friday_end3 == model.friday_end3 && oldMode.friday_start1 == model.friday_start1 && oldMode.friday_start2 == model.friday_start2 && oldMode.friday_start3 == model.friday_start3 && oldMode.holidaytype1_end1 == model.holidaytype1_end1 && oldMode.holidaytype1_end2 == model.holidaytype1_end2 && oldMode.holidaytype1_end3 == model.holidaytype1_end3 && oldMode.holidaytype1_start1 == model.holidaytype1_start1 && oldMode.holidaytype1_start2 == model.holidaytype1_start2 && oldMode.holidaytype1_start3 == model.holidaytype1_start3 && oldMode.holidaytype2_end1 == model.holidaytype2_end1 && oldMode.holidaytype2_end2 == model.holidaytype2_end2 && oldMode.holidaytype2_end3 == model.holidaytype2_end3 && oldMode.holidaytype2_start1 == model.holidaytype2_start1 && oldMode.holidaytype2_start2 == model.holidaytype2_start2 && oldMode.holidaytype2_start3 == model.holidaytype2_start3 && oldMode.holidaytype3_end1 == model.holidaytype3_end1 && oldMode.holidaytype3_end2 == model.holidaytype3_end2 && oldMode.holidaytype3_end3 == model.holidaytype3_end3 && oldMode.holidaytype3_start1 == model.holidaytype3_start1 && oldMode.holidaytype3_start2 == model.holidaytype3_start2 && oldMode.holidaytype3_start3 == model.holidaytype3_start3 && oldMode.monday_end1 == model.monday_end1 && oldMode.monday_end2 == model.monday_end2 && oldMode.monday_end3 == model.monday_end3 && oldMode.monday_start1 == model.monday_start1 && oldMode.monday_start2 == model.monday_start2 && oldMode.monday_start3 == model.monday_start3 && oldMode.saturday_end1 == model.saturday_end1 && oldMode.saturday_end2 == model.saturday_end2 && oldMode.saturday_end3 == model.saturday_end3 && oldMode.saturday_start1 == model.saturday_start1 && oldMode.saturday_start2 == model.saturday_start2 && oldMode.saturday_start3 == model.saturday_start3 && oldMode.sunday_end1 == model.sunday_end1 && oldMode.sunday_end2 == model.sunday_end2 && oldMode.sunday_end3 == model.sunday_end3 && oldMode.sunday_start1 == model.sunday_start1 && oldMode.sunday_start2 == model.sunday_start2 && oldMode.sunday_start3 == model.sunday_start3 && oldMode.thursday_end1 == model.thursday_end1 && oldMode.thursday_end2 == model.thursday_end2 && oldMode.thursday_end3 == model.thursday_end3 && oldMode.thursday_start1 == model.thursday_start1 && oldMode.thursday_start2 == model.thursday_start2 && oldMode.thursday_start3 == model.thursday_start3 && oldMode.tuesday_end1 == model.tuesday_end1 && oldMode.tuesday_end2 == model.tuesday_end2 && oldMode.tuesday_end3 == model.tuesday_end3 && oldMode.tuesday_start1 == model.tuesday_start1 && oldMode.tuesday_start2 == model.tuesday_start2 && oldMode.tuesday_start3 == model.tuesday_start3 && oldMode.wednesday_end1 == model.wednesday_end1 && oldMode.wednesday_end2 == model.wednesday_end2 && oldMode.wednesday_end3 == model.wednesday_end3 && oldMode.wednesday_start1 == model.wednesday_start1 && oldMode.wednesday_start2 == model.wednesday_start2 && oldMode.wednesday_start3 == model.wednesday_start3 && oldMode.TimeZone1Id == model.TimeZone1Id && oldMode.TimeZone2Id == model.TimeZone1Id && oldMode.TimeZone3Id == model.TimeZone3Id && oldMode.TimeZoneHolidayId == model.TimeZoneHolidayId)
			{
				return false;
			}
			return true;
		}

		private bool Save()
		{
			try
			{
				bool flag = false;
				AccTimeseg accTimeseg = null;
				AccTimeseg accTimeseg2 = new AccTimeseg();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accTimeseg = accTimesegBll.GetModel(this.m_id);
					accTimeseg2 = accTimeseg.Copy();
				}
				if (accTimeseg == null)
				{
					accTimeseg = new AccTimeseg();
					if (accTimesegBll.Exists(this.txt_name.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.txt_name.Focus();
						return false;
					}
				}
				else if (accTimeseg.timeseg_name != this.txt_name.Text && accTimesegBll.Exists(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
					this.txt_name.Focus();
					return false;
				}
				this.BindModel(accTimeseg);
				flag = this.ModelIsChanged(accTimeseg2, accTimeseg);
				if (this.m_id > 0)
				{
					if (!flag && accTimesegBll.Update(accTimeseg))
					{
						if (this.RefreshDataEvent != null)
						{
							this.RefreshDataEvent(this, null);
						}
						return true;
					}
					if (accTimeseg != null)
					{
						CommandServer.DelCmd(accTimeseg2);
					}
					if (accTimesegBll.Update(accTimeseg))
					{
						CommandServer.AddCmd(accTimeseg);
						if (accTimeseg2.TimeZone1Id != accTimeseg.TimeZone1Id || accTimeseg2.TimeZone2Id != accTimeseg.TimeZone2Id || accTimeseg2.TimeZone3Id != accTimeseg.TimeZone3Id)
						{
							MachinesBll machinesBll = new MachinesBll(MainForm._ia);
							List<Machines> modelList = machinesBll.GetModelList("DevSDKType = 2 and ID in (select device_id from acc_door where id in (select accdoor_id from acc_levelset_door_group where acclevelset_id in (select id from acc_levelset where level_timeseg_id = " + accTimeseg.id + ")))");
							AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
							List<AccDoor> modelList2 = accDoorBll.GetModelList("id in (select accdoor_id from acc_levelset_door_group)");
							Dictionary<int, Dictionary<int, AccDoor>> dictionary = new Dictionary<int, Dictionary<int, AccDoor>>();
							if (modelList2 != null)
							{
								for (int i = 0; i < modelList2.Count; i++)
								{
									if (!dictionary.ContainsKey(modelList2[i].device_id))
									{
										Dictionary<int, AccDoor> dictionary2 = new Dictionary<int, AccDoor>();
										dictionary2.Add(modelList2[i].id, modelList2[i]);
										dictionary.Add(modelList2[i].device_id, dictionary2);
									}
									else
									{
										Dictionary<int, AccDoor> dictionary2 = dictionary[modelList2[i].device_id];
										if (!dictionary2.ContainsKey(modelList2[i].id))
										{
											dictionary2.Add(modelList2[i].id, modelList2[i]);
										}
									}
								}
							}
							if (modelList != null && modelList.Count > 0)
							{
								UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
								List<UserInfo> modelList3 = userInfoBll.GetModelList("USERID in (select employee_id from acc_levelset_emp)");
								Dictionary<int, UserInfo> dictionary3 = new Dictionary<int, UserInfo>();
								if (modelList3 != null && modelList3.Count > 0)
								{
									for (int j = 0; j < modelList3.Count; j++)
									{
										if (!dictionary3.ContainsKey(modelList3[j].UserId))
										{
											dictionary3.Add(modelList3[j].UserId, modelList3[j]);
										}
									}
								}
								AccLevelsetEmpBll accLevelsetEmpBll = new AccLevelsetEmpBll(MainForm._ia);
								for (int k = 0; k < modelList.Count; k++)
								{
									if (dictionary.ContainsKey(modelList[k].ID))
									{
										List<AccLevelsetEmp> modelList4 = accLevelsetEmpBll.GetModelList("acclevelset_id in (select acclevelset_id from acc_levelset_door_group where accdoor_device_id = " + modelList[k].ID + " and acclevelset_id in (select id from acc_levelset where level_timeseg_id = " + accTimeseg.id + "))");
										if (modelList4 != null && modelList4.Count > 0)
										{
											List<UserInfo> list = new List<UserInfo>();
											for (int l = 0; l < modelList4.Count; l++)
											{
												AccLevelsetEmp accLevelsetEmp = modelList4[l];
												if (dictionary3.ContainsKey(accLevelsetEmp.employee_id))
												{
													UserInfo item = dictionary3[modelList4[l].employee_id];
													if (!list.Contains(item))
													{
														list.Add(item);
													}
												}
											}
											CommandServer.AddUserAuthorize(modelList[k], accTimeseg, list, modelList2);
										}
									}
								}
								List<Group> list2 = new List<Group>();
								AccMorecardempGroupBll accMorecardempGroupBll = new AccMorecardempGroupBll(MainForm._ia);
								List<AccMorecardempGroup> modelList5 = accMorecardempGroupBll.GetModelList("");
								if (modelList5 != null)
								{
									for (int m = 0; m < modelList5.Count; m++)
									{
										if (modelList5[m].StdGroupNo > 0 && modelList5[m].StdGroupTz == accTimeseg.id)
										{
											Group group = new Group();
											group.GroupNo = modelList5[m].StdGroupNo;
											group.TimeZoneId1 = accTimeseg.TimeZone1Id;
											group.TimeZoneId2 = accTimeseg.TimeZone2Id;
											group.TimeZoneId3 = accTimeseg.TimeZone3Id;
											group.ValidOnHoliday = (modelList5[m].StdValidOnHoliday ? 1 : 0);
											group.VerifyMode = modelList5[m].StdGroupVT;
											list2.Add(group);
										}
									}
								}
								for (int n = 0; n < modelList.Count; n++)
								{
									if (dictionary.ContainsKey(modelList[n].ID))
									{
										foreach (KeyValuePair<int, AccDoor> item2 in dictionary[modelList[n].ID])
										{
											if (item2.Value.lock_active_id == accTimeseg.id)
											{
												Group group = new Group();
												group.GroupNo = 1;
												group.TimeZoneId1 = accTimeseg.TimeZone1Id;
												group.TimeZoneId2 = accTimeseg.TimeZone2Id;
												group.TimeZoneId3 = accTimeseg.TimeZone3Id;
												group.ValidOnHoliday = 1;
												group.VerifyMode = item2.Value.opendoor_type;
												list2.Add(group);
												break;
											}
										}
									}
									if (list2.Count > 0)
									{
										CommandServer.AddGroup(modelList[n], list2);
									}
								}
								AccHolidaysBll accHolidaysBll = new AccHolidaysBll(MainForm._ia);
								List<AccHolidays> modelList6 = accHolidaysBll.GetModelList("");
								if (modelList6 != null)
								{
									for (int num = 0; num < modelList6.Count; num++)
									{
										if (modelList6[num].HolidayNo > 0)
										{
											if (modelList6[num].HolidayTZ == accTimeseg2.TimeZone1Id && accTimeseg2.TimeZone1Id != accTimeseg.TimeZone1Id)
											{
												modelList6[num].HolidayTZ = accTimeseg.TimeZone1Id;
												accHolidaysBll.Update(modelList6[num]);
												CommandServer.AddCmd(modelList6[num]);
											}
											else if (modelList6[num].HolidayTZ == accTimeseg2.TimeZone2Id && accTimeseg2.TimeZone1Id != accTimeseg.TimeZone2Id)
											{
												modelList6[num].HolidayTZ = accTimeseg.TimeZone2Id;
												accHolidaysBll.Update(modelList6[num]);
												CommandServer.AddCmd(modelList6[num]);
											}
											else if (modelList6[num].HolidayTZ == accTimeseg2.TimeZone3Id && accTimeseg2.TimeZone1Id != accTimeseg.TimeZone3Id)
											{
												modelList6[num].HolidayTZ = accTimeseg.TimeZone3Id;
												accHolidaysBll.Update(modelList6[num]);
												CommandServer.AddCmd(modelList6[num]);
											}
											else if (modelList6[num].HolidayTZ == accTimeseg2.TimeZoneHolidayId && accTimeseg2.TimeZone1Id != accTimeseg.TimeZoneHolidayId)
											{
												modelList6[num].HolidayTZ = accTimeseg.TimeZoneHolidayId;
												accHolidaysBll.Update(modelList6[num]);
												CommandServer.AddCmd(modelList6[num]);
											}
										}
									}
								}
								for (int num2 = 0; num2 < modelList.Count; num2++)
								{
									if (modelList[num2].IsTFTMachine && dictionary.ContainsKey(modelList[num2].ID))
									{
										foreach (KeyValuePair<int, AccDoor> item3 in dictionary[modelList[num2].ID])
										{
											if (item3.Value.long_open_id == accTimeseg.id && accTimeseg2.TimeZone1Id != accTimeseg.TimeZone1Id)
											{
												CommandServer.SetDoorParam(item3.Value);
											}
										}
									}
								}
							}
						}
						FrmShowUpdata.Instance.ShowEx();
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
						if (accTimesegBll.Add(accTimeseg) > 0)
						{
							CommandServer.AddCmd(accTimeseg);
							FrmShowUpdata.Instance.ShowEx();
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(this, null);
							}
							this.txt_name.Text = "";
							return true;
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private void btn_saveAndContinue_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txt_name.Text))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
				this.txt_name.Focus();
			}
			else
			{
				this.Save();
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.txt_name.Text.Trim()))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
				this.txt_name.Focus();
			}
			else if (this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void time_1_SelectIndexChangedEvent(object sender, EventArgs e)
		{
			try
			{
				if (sender != null)
				{
					TimeControl timeControl = sender as TimeControl;
					if (timeControl != null && (this.m_lastSelectTime = timeControl.SelectTime) != null)
					{
						this.time_1_TimeValueChangedEvent(this.m_lastSelectTime, e);
						if (timeControl != this.time_1)
						{
							this.time_1.SelectIndex = -1;
						}
						if (timeControl != this.time_2)
						{
							this.time_2.SelectIndex = -1;
						}
						if (timeControl != this.time_3)
						{
							this.time_3.SelectIndex = -1;
						}
						if (timeControl != this.time_4)
						{
							this.time_4.SelectIndex = -1;
						}
						if (timeControl != this.time_5)
						{
							this.time_5.SelectIndex = -1;
						}
						if (timeControl != this.time_6)
						{
							this.time_6.SelectIndex = -1;
						}
						if (timeControl != this.time_7)
						{
							this.time_7.SelectIndex = -1;
						}
						if (timeControl != this.time_8)
						{
							this.time_8.SelectIndex = -1;
						}
						if (timeControl != this.time_9)
						{
							this.time_9.SelectIndex = -1;
						}
						if (timeControl != this.time_10)
						{
							this.time_10.SelectIndex = -1;
						}
					}
				}
			}
			catch
			{
			}
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_remark_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e, 70);
		}

		private void btn_help_Click(object sender, EventArgs e)
		{
			TimeHelp timeHelp = new TimeHelp();
			timeHelp.ShowDialog();
		}

		private void time_1_TimeValueChangedEvent(object sender, EventArgs e)
		{
			this.m_isShow = true;
			try
			{
				TimeInfo timeInfo = sender as TimeInfo;
				if (timeInfo != null)
				{
					int startTime = timeInfo.StartTime;
					DateEdit dateEdit = this.txt_start;
					DateTime dateTime = DateTime.Now;
					DateTime now = DateTime.Now;
					dateTime = dateTime.AddHours((double)(-now.Hour));
					now = DateTime.Now;
					dateTime = dateTime.AddMinutes((double)(-now.Minute));
					now = DateTime.Now;
					dateTime = dateTime.AddSeconds((double)(-now.Second));
					dateEdit.EditValue = dateTime.AddMinutes((double)startTime);
					int endTime = timeInfo.EndTime;
					DateEdit dateEdit2 = this.txt_end;
					dateTime = DateTime.Now;
					now = DateTime.Now;
					dateTime = dateTime.AddHours((double)(-now.Hour));
					now = DateTime.Now;
					dateTime = dateTime.AddMinutes((double)(-now.Minute));
					now = DateTime.Now;
					dateTime = dateTime.AddSeconds((double)(-now.Second));
					dateEdit2.EditValue = dateTime.AddMinutes((double)endTime);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
			this.m_isShow = false;
		}

		private void time_1_KeyUpEvent(object sender, KeyEventArgs e)
		{
			try
			{
				if (this.m_lastSelectTime != null)
				{
					this.time_1.OnKeyUP(sender, e);
					this.time_2.OnKeyUP(sender, e);
					this.time_3.OnKeyUP(sender, e);
					this.time_4.OnKeyUP(sender, e);
					this.time_5.OnKeyUP(sender, e);
					this.time_6.OnKeyUP(sender, e);
					this.time_7.OnKeyUP(sender, e);
					this.time_8.OnKeyUP(sender, e);
					this.time_9.OnKeyUP(sender, e);
					this.time_10.OnKeyUP(sender, e);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetTime(object sender, EventArgs e)
		{
			if (!this.m_isShow && !this.isLock)
			{
				this.isLock = true;
				try
				{
					if (this.m_lastSelectTime != null)
					{
						bool flag = false;
						try
						{
							DateTime dateTime = (DateTime)this.txt_start.EditValue;
							DateTime dateTime2 = (DateTime)this.txt_end.EditValue;
							int num = dateTime.Hour * 60 + dateTime.Minute;
							int num2 = dateTime2.Hour * 60 + dateTime2.Minute;
							if (num < num2 && num2 <= 1440)
							{
								this.m_lastSelectTime.SetTimeEx(num, num2);
								if (this.m_lastSelectTime.Time != null)
								{
									this.m_lastSelectTime.Time.Refresh();
								}
								flag = true;
							}
						}
						catch
						{
						}
						if (!flag)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("STimeInputIsError", "输入时间格式不正确或者开始时间大于结束时间"));
							int startTime = this.m_lastSelectTime.StartTime;
							DateEdit dateEdit = this.txt_start;
							DateTime dateTime3 = DateTime.Now;
							DateTime now = DateTime.Now;
							dateTime3 = dateTime3.AddHours((double)(-now.Hour));
							now = DateTime.Now;
							dateTime3 = dateTime3.AddMinutes((double)(-now.Minute));
							now = DateTime.Now;
							dateTime3 = dateTime3.AddSeconds((double)(-now.Second));
							dateEdit.EditValue = dateTime3.AddMinutes((double)startTime);
							int endTime = this.m_lastSelectTime.EndTime;
							DateEdit dateEdit2 = this.txt_end;
							dateTime3 = DateTime.Now;
							now = DateTime.Now;
							dateTime3 = dateTime3.AddHours((double)(-now.Hour));
							now = DateTime.Now;
							dateTime3 = dateTime3.AddMinutes((double)(-now.Minute));
							now = DateTime.Now;
							dateTime3 = dateTime3.AddSeconds((double)(-now.Second));
							dateEdit2.EditValue = dateTime3.AddMinutes((double)endTime);
						}
					}
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
				this.isLock = false;
			}
		}

		private void lb_satrt_SizeChanged(object sender, EventArgs e)
		{
			this.txt_start.Left = this.lb_satrt.Left + this.lb_satrt.Width + 5;
			this.lb_end.Left = this.txt_start.Left + this.txt_start.Width + 20;
			this.txt_end.Left = this.lb_end.Left + this.lb_end.Width + 5;
		}

		private void LoadAllTimeZone()
		{
			try
			{
				this.lstTimeZoneExists = new List<int>();
				AccTimesegBll accTimesegBll = new AccTimesegBll(MainForm._ia);
				List<AccTimeseg> modelList = accTimesegBll.GetModelList("");
				if (modelList != null)
				{
					for (int i = 0; i < modelList.Count; i++)
					{
						if (modelList[i].TimeZone1Id > 0 && !this.lstTimeZoneExists.Contains(modelList[i].TimeZone1Id))
						{
							this.lstTimeZoneExists.Add(modelList[i].TimeZone1Id);
						}
						if (modelList[i].TimeZone2Id > 0 && !this.lstTimeZoneExists.Contains(modelList[i].TimeZone2Id))
						{
							this.lstTimeZoneExists.Add(modelList[i].TimeZone2Id);
						}
						if (modelList[i].TimeZone3Id > 0 && !this.lstTimeZoneExists.Contains(modelList[i].TimeZone3Id))
						{
							this.lstTimeZoneExists.Add(modelList[i].TimeZone3Id);
						}
						if (modelList[i].TimeZoneHolidayId > 0 && !this.lstTimeZoneExists.Contains(modelList[i].TimeZoneHolidayId))
						{
							this.lstTimeZoneExists.Add(modelList[i].TimeZoneHolidayId);
						}
						if (modelList[i].id == this.m_id)
						{
							this.OldTimeZone = modelList[i];
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void GenerateDataSource()
		{
			this.dtTimeZone1 = new DataTable();
			this.dtTimeZone1.Columns.Add("Value", typeof(int));
			this.dtTimeZone1.Columns.Add("Text", typeof(string));
			DataRow dataRow = this.dtTimeZone1.NewRow();
			dataRow["Value"] = 0;
			dataRow["Text"] = "----------";
			this.dtTimeZone1.Rows.Add(dataRow);
			for (int i = 1; i <= 50; i++)
			{
				if (!this.lstTimeZoneExists.Contains(i))
				{
					dataRow = this.dtTimeZone1.NewRow();
					dataRow["Value"] = i;
					dataRow["Text"] = i.ToString();
					this.dtTimeZone1.Rows.Add(dataRow);
				}
			}
			this.dtTimeZone2 = this.dtTimeZone1.Copy();
			this.dtTimeZone3 = this.dtTimeZone1.Copy();
			this.dtTimeZoneHoliday = this.dtTimeZone1.Copy();
			if (this.m_id > 0 && this.OldTimeZone != null)
			{
				if (this.OldTimeZone.TimeZone1Id > 0)
				{
					this.InsertTimeZoneId(this.dtTimeZone1, this.OldTimeZone.TimeZone1Id);
				}
				if (this.OldTimeZone.TimeZone2Id > 0)
				{
					this.InsertTimeZoneId(this.dtTimeZone2, this.OldTimeZone.TimeZone2Id);
				}
				if (this.OldTimeZone.TimeZone3Id > 0)
				{
					this.InsertTimeZoneId(this.dtTimeZone3, this.OldTimeZone.TimeZone3Id);
				}
				if (this.OldTimeZone.TimeZoneHolidayId > 0)
				{
					this.InsertTimeZoneId(this.dtTimeZoneHoliday, this.OldTimeZone.TimeZoneHolidayId);
				}
			}
			this.cbbTZ1Id.DataSource = this.dtTimeZone1;
			this.cbbTZ1Id.ValueMember = "Value";
			this.cbbTZ1Id.DisplayMember = "Text";
			this.cbbTZ2Id.DataSource = this.dtTimeZone2;
			this.cbbTZ2Id.ValueMember = "Value";
			this.cbbTZ2Id.DisplayMember = "Text";
			this.cbbTZ3Id.DataSource = this.dtTimeZone3;
			this.cbbTZ3Id.ValueMember = "Value";
			this.cbbTZ3Id.DisplayMember = "Text";
			this.cbbTZHolidayId.DataSource = this.dtTimeZoneHoliday;
			this.cbbTZHolidayId.ValueMember = "Value";
			this.cbbTZHolidayId.DisplayMember = "Text";
		}

		private void InsertTimeZoneId(DataTable dt, int TimeZoneId)
		{
			bool flag = false;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dt.Rows[i];
				if (int.TryParse(dataRow["Value"].ToString(), out int num) && num == TimeZoneId)
				{
					if (num == TimeZoneId)
					{
						flag = true;
						break;
					}
					if (num > TimeZoneId)
					{
						DataRow dataRow2 = dt.NewRow();
						dataRow2["Value"] = TimeZoneId;
						dataRow2["Text"] = TimeZoneId.ToString();
						dt.Rows.InsertAt(dataRow2, i);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				DataRow dataRow2 = dt.NewRow();
				dataRow2["Value"] = TimeZoneId;
				dataRow2["Text"] = TimeZoneId.ToString();
				dt.Rows.Add(dataRow2);
			}
		}

		private void RemoveTimeZoneId(DataTable dt, int TimeZoneId)
		{
			if (TimeZoneId > 0)
			{
				int num = 0;
				DataRow dataRow;
				while (true)
				{
					if (num < dt.Rows.Count)
					{
						dataRow = dt.Rows[num];
						if (int.TryParse(dataRow["Value"].ToString(), out int num2) && num2 == TimeZoneId)
						{
							break;
						}
						num++;
						continue;
					}
					return;
				}
				dt.Rows.Remove(dataRow);
			}
		}

		private void CbbTZId_SelectedChanged(object sender, EventArgs e)
		{
			System.Windows.Forms.ComboBox comboBox = sender as System.Windows.Forms.ComboBox;
			if (comboBox != null)
			{
				DataTable dataTable = comboBox.DataSource as DataTable;
				int timeZoneId = default(int);
				if (dataTable != null && comboBox.SelectedValue != null && int.TryParse(comboBox.SelectedValue.ToString(), out timeZoneId))
				{
					lock (this)
					{
						if (!this.SelectedValueChanging)
						{
							this.SelectedValueChanging = true;
							this.cbbTZ1Id.SelectedIndexChanged -= this.CbbTZId_SelectedChanged;
							this.cbbTZ2Id.SelectedIndexChanged -= this.CbbTZId_SelectedChanged;
							this.cbbTZ3Id.SelectedIndexChanged -= this.CbbTZId_SelectedChanged;
							this.cbbTZHolidayId.SelectedIndexChanged -= this.CbbTZId_SelectedChanged;
							goto end_IL_0066;
						}
						return;
						end_IL_0066:;
					}
					int num;
					if (dataTable != this.dtTimeZone1)
					{
						int.TryParse((this.cbbTZ1Id.SelectedValue ?? ((object)0)).ToString(), out num);
						this.dtTimeZone1 = dataTable.Copy();
						this.cbbTZ1Id.DataSource = this.dtTimeZone1;
						this.InsertTimeZoneId(this.dtTimeZone1, num);
						this.cbbTZ1Id.SelectedValue = num;
						this.RemoveTimeZoneId(this.dtTimeZone1, timeZoneId);
					}
					if (dataTable != this.dtTimeZone2)
					{
						int.TryParse((this.cbbTZ2Id.SelectedValue ?? ((object)0)).ToString(), out num);
						this.dtTimeZone2 = dataTable.Copy();
						this.cbbTZ2Id.DataSource = this.dtTimeZone2;
						this.InsertTimeZoneId(this.dtTimeZone2, num);
						this.cbbTZ2Id.SelectedValue = num;
						this.RemoveTimeZoneId(this.dtTimeZone2, timeZoneId);
					}
					if (dataTable != this.dtTimeZone3)
					{
						int.TryParse((this.cbbTZ3Id.SelectedValue ?? ((object)0)).ToString(), out num);
						this.dtTimeZone3 = dataTable.Copy();
						this.cbbTZ3Id.DataSource = this.dtTimeZone3;
						this.InsertTimeZoneId(this.dtTimeZone3, num);
						this.cbbTZ3Id.SelectedValue = num;
						this.RemoveTimeZoneId(this.dtTimeZone3, timeZoneId);
					}
					if (dataTable != this.dtTimeZoneHoliday)
					{
						int.TryParse((this.cbbTZHolidayId.SelectedValue ?? ((object)0)).ToString(), out num);
						this.dtTimeZoneHoliday = dataTable.Copy();
						this.cbbTZHolidayId.DataSource = this.dtTimeZoneHoliday;
						this.InsertTimeZoneId(this.dtTimeZoneHoliday, num);
						this.cbbTZHolidayId.SelectedValue = num;
						this.RemoveTimeZoneId(this.dtTimeZoneHoliday, timeZoneId);
					}
					lock (this)
					{
						this.SelectedValueChanging = false;
						this.cbbTZ1Id.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
						this.cbbTZ2Id.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
						this.cbbTZ3Id.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
						this.cbbTZHolidayId.SelectedIndexChanged += this.CbbTZId_SelectedChanged;
					}
				}
			}
		}

		private void AutoSetStdTimeZoneId(System.Windows.Forms.ComboBox cbb, AccTimeseg time)
		{
			DataTable dataTable;
			int num;
			int num2;
			int num3;
			if (cbb != null)
			{
				dataTable = (cbb.DataSource as DataTable);
				if (dataTable != null && dataTable.Rows.Count > 1)
				{
					if (cbb == this.cbbTZ1Id)
					{
						if (time.sunday_start1.HasValue && time.sunday_end1.HasValue && time.sunday_end1.Value > time.sunday_start1.Value)
						{
							goto IL_0250;
						}
						if (time.monday_start1.HasValue && time.monday_end1.HasValue && time.monday_end1.Value > time.monday_start1.Value)
						{
							goto IL_0250;
						}
						if (time.tuesday_start1.HasValue && time.tuesday_end1.HasValue && time.tuesday_end1.Value > time.tuesday_start1.Value)
						{
							goto IL_0250;
						}
						if (time.wednesday_start1.HasValue && time.wednesday_end1.HasValue && time.wednesday_end1.Value > time.wednesday_start1.Value)
						{
							goto IL_0250;
						}
						if (time.thursday_start1.HasValue && time.thursday_end1.HasValue && time.thursday_end1.Value > time.thursday_start1.Value)
						{
							goto IL_0250;
						}
						if (time.friday_start1.HasValue && time.friday_end1.HasValue && time.friday_end1.Value > time.friday_start1.Value)
						{
							goto IL_0250;
						}
						num = ((time.saturday_start1.HasValue && time.saturday_end1.HasValue && time.saturday_end1.Value > time.saturday_start1.Value) ? 1 : 0);
						goto IL_0251;
					}
					if (cbb == this.cbbTZ2Id)
					{
						if (time.sunday_start2.HasValue && time.sunday_end2.HasValue && time.sunday_end2.Value > time.sunday_start2.Value)
						{
							goto IL_04ca;
						}
						if (time.monday_start2.HasValue && time.monday_end2.HasValue && time.monday_end2.Value > time.monday_start2.Value)
						{
							goto IL_04ca;
						}
						if (time.tuesday_start2.HasValue && time.tuesday_end2.HasValue && time.tuesday_end2.Value > time.tuesday_start2.Value)
						{
							goto IL_04ca;
						}
						if (time.wednesday_start2.HasValue && time.wednesday_end2.HasValue && time.wednesday_end2.Value > time.wednesday_start2.Value)
						{
							goto IL_04ca;
						}
						if (time.thursday_start2.HasValue && time.thursday_end2.HasValue && time.thursday_end2.Value > time.thursday_start2.Value)
						{
							goto IL_04ca;
						}
						if (time.friday_start2.HasValue && time.friday_end2.HasValue && time.friday_end2.Value > time.friday_start2.Value)
						{
							goto IL_04ca;
						}
						num2 = ((time.saturday_start2.HasValue && time.saturday_end2.HasValue && time.saturday_end2.Value > time.saturday_start2.Value) ? 1 : 0);
						goto IL_04cb;
					}
					if (cbb == this.cbbTZ3Id)
					{
						if (time.sunday_start3.HasValue && time.sunday_end3.HasValue && time.sunday_end3.Value > time.sunday_start3.Value)
						{
							goto IL_0744;
						}
						if (time.monday_start3.HasValue && time.monday_end3.HasValue && time.monday_end3.Value > time.monday_start3.Value)
						{
							goto IL_0744;
						}
						if (time.tuesday_start3.HasValue && time.tuesday_end3.HasValue && time.tuesday_end3.Value > time.tuesday_start3.Value)
						{
							goto IL_0744;
						}
						if (time.wednesday_start3.HasValue && time.wednesday_end3.HasValue && time.wednesday_end3.Value > time.wednesday_start3.Value)
						{
							goto IL_0744;
						}
						if (time.thursday_start3.HasValue && time.thursday_end3.HasValue && time.thursday_end3.Value > time.thursday_start3.Value)
						{
							goto IL_0744;
						}
						if (time.friday_start3.HasValue && time.friday_end3.HasValue && time.friday_end3.Value > time.friday_start3.Value)
						{
							goto IL_0744;
						}
						num3 = ((time.saturday_start3.HasValue && time.saturday_end3.HasValue && time.saturday_end3.Value > time.saturday_start3.Value) ? 1 : 0);
						goto IL_0745;
					}
					if (cbb == this.cbbTZHolidayId)
					{
						if (time.holidaytype1_start1.HasValue && time.holidaytype1_end1.HasValue && time.holidaytype1_end1.Value > time.holidaytype1_start1.Value)
						{
							if (cbb.SelectedValue == null || cbb.SelectedValue.ToString() == "0")
							{
								cbb.SelectedValue = dataTable.Rows[1]["Value"];
							}
						}
						else
						{
							cbb.SelectedValue = "0";
						}
					}
				}
			}
			return;
			IL_0744:
			num3 = 1;
			goto IL_0745;
			IL_0745:
			if (num3 != 0)
			{
				if (cbb.SelectedValue == null || cbb.SelectedValue.ToString() == "0")
				{
					cbb.SelectedValue = dataTable.Rows[1]["Value"];
				}
			}
			else
			{
				cbb.SelectedValue = "0";
			}
			return;
			IL_0251:
			if (num != 0)
			{
				if (cbb.SelectedValue == null || cbb.SelectedValue.ToString() == "0")
				{
					cbb.SelectedValue = dataTable.Rows[1]["Value"];
				}
			}
			else
			{
				cbb.SelectedValue = "0";
			}
			return;
			IL_0250:
			num = 1;
			goto IL_0251;
			IL_04ca:
			num2 = 1;
			goto IL_04cb;
			IL_04cb:
			if (num2 != 0)
			{
				if (cbb.SelectedValue == null || cbb.SelectedValue.ToString() == "0")
				{
					cbb.SelectedValue = dataTable.Rows[1]["Value"];
				}
			}
			else
			{
				cbb.SelectedValue = "0";
			}
		}

		private void btn_help_diferences_Click(object sender, EventArgs e)
		{
			string msg = "Importante: A função Zona de tempo, possibilita a criação de no máximo três intervalos de tempo para cada Zona criada. \nDispositivos STD SDK(Duo SS 210, Duo SS 220, NEO SS 410, NEO SS 420, Bio Inox Plus SS 311E, Bio Inox Plus SS \n311 MF, SS 420, SS 420 MF, Duo SS 230, Duo SS 230 MF, SS 610, SS 710, IBProx SS 120), permitem apenas uma zona\nde tempo com até três períodos por usuário. ";
			Mensage mensage = new Mensage(680, 130, "Mensagem", msg, true, false, 0);
			mensage.Show();
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
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			SerializableAppearanceObject appearance2 = new SerializableAppearanceObject();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TimeSet));
			this.txt_remark = new TextBox();
			this.txt_name = new TextBox();
			this.lb_remark = new Label();
			this.lb_name = new Label();
			this.lb_hliday3 = new Label();
			this.lb_hliday2 = new Label();
			this.lb_hliday1 = new Label();
			this.lb_sunday = new Label();
			this.lb_saturday = new Label();
			this.lb_friday = new Label();
			this.lb_thursday = new Label();
			this.lb_wednesday = new Label();
			this.lb_tuesday = new Label();
			this.lb_monday = new Label();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_satrt = new Label();
			this.lb_end = new Label();
			this.panel1 = new Panel();
			this.time_2 = new TimeControl();
			this.time_10 = new TimeControl();
			this.time_9 = new TimeControl();
			this.time_8 = new TimeControl();
			this.time_7 = new TimeControl();
			this.time_1 = new TimeControl();
			this.time_6 = new TimeControl();
			this.time_5 = new TimeControl();
			this.time_3 = new TimeControl();
			this.time_4 = new TimeControl();
			this.btn_help = new ButtonX();
			this.label2 = new Label();
			this.txt_start = new DateEdit();
			this.txt_end = new DateEdit();
			this.cbbTZ1Id = new System.Windows.Forms.ComboBox();
			this.lblTZ1 = new Label();
			this.lblTZ2 = new Label();
			this.cbbTZ2Id = new System.Windows.Forms.ComboBox();
			this.lblTZ3 = new Label();
			this.cbbTZ3Id = new System.Windows.Forms.ComboBox();
			this.lblTZHoliday = new Label();
			this.cbbTZHolidayId = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new GroupBox();
			this.btn_help_diferences = new Button();
			this.panel1.SuspendLayout();
			((ISupportInitialize)this.txt_start.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.txt_start.Properties).BeginInit();
			((ISupportInitialize)this.txt_end.Properties.CalendarTimeProperties).BeginInit();
			((ISupportInitialize)this.txt_end.Properties).BeginInit();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.txt_remark.Location = new Point(488, 14);
			this.txt_remark.Name = "txt_remark";
			this.txt_remark.Size = new Size(184, 20);
			this.txt_remark.TabIndex = 1;
			this.txt_remark.KeyPress += this.txt_remark_KeyPress;
			this.txt_name.Location = new Point(181, 14);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(168, 20);
			this.txt_name.TabIndex = 2;
			this.txt_name.Click += this.btn_help_diferences_Click;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.lb_remark.Location = new Point(391, 18);
			this.lb_remark.Name = "lb_remark";
			this.lb_remark.Size = new Size(91, 13);
			this.lb_remark.TabIndex = 155;
			this.lb_remark.Text = "备注";
			this.lb_remark.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_name.Location = new Point(18, 18);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(120, 13);
			this.lb_name.TabIndex = 154;
			this.lb_name.Text = "时间段名称";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_hliday3.Location = new Point(18, 581);
			this.lb_hliday3.Name = "lb_hliday3";
			this.lb_hliday3.Size = new Size(120, 13);
			this.lb_hliday3.TabIndex = 167;
			this.lb_hliday3.Text = "假日类型3";
			this.lb_hliday3.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_hliday2.Location = new Point(18, 524);
			this.lb_hliday2.Name = "lb_hliday2";
			this.lb_hliday2.Size = new Size(120, 13);
			this.lb_hliday2.TabIndex = 166;
			this.lb_hliday2.Text = "假日类型2";
			this.lb_hliday2.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_hliday1.Location = new Point(18, 476);
			this.lb_hliday1.Name = "lb_hliday1";
			this.lb_hliday1.Size = new Size(120, 13);
			this.lb_hliday1.TabIndex = 165;
			this.lb_hliday1.Text = "假日类型1";
			this.lb_hliday1.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_sunday.Location = new Point(18, 426);
			this.lb_sunday.Name = "lb_sunday";
			this.lb_sunday.Size = new Size(120, 13);
			this.lb_sunday.TabIndex = 164;
			this.lb_sunday.Text = "星期日";
			this.lb_sunday.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_saturday.Location = new Point(18, 376);
			this.lb_saturday.Name = "lb_saturday";
			this.lb_saturday.Size = new Size(120, 13);
			this.lb_saturday.TabIndex = 163;
			this.lb_saturday.Text = "星期六";
			this.lb_saturday.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_friday.Location = new Point(18, 326);
			this.lb_friday.Name = "lb_friday";
			this.lb_friday.Size = new Size(120, 13);
			this.lb_friday.TabIndex = 162;
			this.lb_friday.Text = "星期五";
			this.lb_friday.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_thursday.Location = new Point(18, 276);
			this.lb_thursday.Name = "lb_thursday";
			this.lb_thursday.Size = new Size(120, 13);
			this.lb_thursday.TabIndex = 161;
			this.lb_thursday.Text = "星期四";
			this.lb_thursday.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_wednesday.Location = new Point(18, 226);
			this.lb_wednesday.Name = "lb_wednesday";
			this.lb_wednesday.Size = new Size(120, 13);
			this.lb_wednesday.TabIndex = 160;
			this.lb_wednesday.Text = "星期三";
			this.lb_wednesday.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_tuesday.Location = new Point(18, 176);
			this.lb_tuesday.Name = "lb_tuesday";
			this.lb_tuesday.Size = new Size(120, 13);
			this.lb_tuesday.TabIndex = 159;
			this.lb_tuesday.Text = "星期二";
			this.lb_tuesday.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_monday.Location = new Point(18, 120);
			this.lb_monday.Name = "lb_monday";
			this.lb_monday.Size = new Size(120, 13);
			this.lb_monday.TabIndex = 158;
			this.lb_monday.Text = "星期一";
			this.lb_monday.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(723, 636);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 7;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(617, 636);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 25);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 6;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_satrt.AutoSize = true;
			this.lb_satrt.Location = new Point(172, 640);
			this.lb_satrt.Name = "lb_satrt";
			this.lb_satrt.Size = new Size(55, 13);
			this.lb_satrt.TabIndex = 180;
			this.lb_satrt.Text = "开始时间";
			this.lb_satrt.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_satrt.SizeChanged += this.lb_satrt_SizeChanged;
			this.lb_end.AutoSize = true;
			this.lb_end.Location = new Point(365, 640);
			this.lb_end.Name = "lb_end";
			this.lb_end.Size = new Size(55, 13);
			this.lb_end.TabIndex = 182;
			this.lb_end.Text = "结束时间";
			this.lb_end.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_end.SizeChanged += this.lb_satrt_SizeChanged;
			this.panel1.Controls.Add(this.time_2);
			this.panel1.Controls.Add(this.time_10);
			this.panel1.Controls.Add(this.time_9);
			this.panel1.Controls.Add(this.time_8);
			this.panel1.Controls.Add(this.time_7);
			this.panel1.Controls.Add(this.time_1);
			this.panel1.Controls.Add(this.time_6);
			this.panel1.Controls.Add(this.time_5);
			this.panel1.Controls.Add(this.time_3);
			this.panel1.Controls.Add(this.time_4);
			this.panel1.Location = new Point(157, 92);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(675, 527);
			this.panel1.TabIndex = 183;
			this.time_2.FontSize = 9;
			this.time_2.HeightEx = 5;
			this.time_2.IFont = new Font("SimSun", 9f);
			this.time_2.IsCreateSize = false;
			this.time_2.KeyMove = 2;
			this.time_2.Location = new Point(3, 68);
			this.time_2.Name = "time_2";
			this.time_2.SelectIndex = -1;
			this.time_2.Size = new Size(664, 43);
			this.time_2.TabIndex = 182;
			this.time_2.TabStop = false;
			this.time_2.WidthEx = 20;
			this.time_2.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_2.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_2.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_10.FontSize = 9;
			this.time_10.HeightEx = 10;
			this.time_10.IFont = new Font("SimSun", 9f);
			this.time_10.IsCreateSize = true;
			this.time_10.KeyMove = 2;
			this.time_10.Location = new Point(3, 462);
			this.time_10.Name = "time_10";
			this.time_10.SelectIndex = -1;
			this.time_10.Size = new Size(664, 65);
			this.time_10.TabIndex = 181;
			this.time_10.TabStop = false;
			this.time_10.WidthEx = 20;
			this.time_10.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_10.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_10.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_9.FontSize = 9;
			this.time_9.HeightEx = 5;
			this.time_9.IFont = new Font("SimSun", 9f);
			this.time_9.IsCreateSize = false;
			this.time_9.KeyMove = 2;
			this.time_9.Location = new Point(3, 417);
			this.time_9.Name = "time_9";
			this.time_9.SelectIndex = -1;
			this.time_9.Size = new Size(664, 43);
			this.time_9.TabIndex = 180;
			this.time_9.TabStop = false;
			this.time_9.WidthEx = 20;
			this.time_9.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_9.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_9.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_8.FontSize = 9;
			this.time_8.HeightEx = 5;
			this.time_8.IFont = new Font("SimSun", 9f);
			this.time_8.IsCreateSize = false;
			this.time_8.KeyMove = 2;
			this.time_8.Location = new Point(3, 367);
			this.time_8.Name = "time_8";
			this.time_8.SelectIndex = -1;
			this.time_8.Size = new Size(664, 43);
			this.time_8.TabIndex = 179;
			this.time_8.TabStop = false;
			this.time_8.WidthEx = 20;
			this.time_8.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_8.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_8.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_7.FontSize = 9;
			this.time_7.HeightEx = 5;
			this.time_7.IFont = new Font("SimSun", 9f);
			this.time_7.IsCreateSize = false;
			this.time_7.KeyMove = 2;
			this.time_7.Location = new Point(3, 317);
			this.time_7.Name = "time_7";
			this.time_7.SelectIndex = -1;
			this.time_7.Size = new Size(664, 43);
			this.time_7.TabIndex = 178;
			this.time_7.TabStop = false;
			this.time_7.WidthEx = 20;
			this.time_7.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_7.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_7.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_1.FontSize = 9;
			this.time_1.HeightEx = 10;
			this.time_1.IFont = new Font("SimSun", 9f);
			this.time_1.IsCreateSize = true;
			this.time_1.KeyMove = 2;
			this.time_1.Location = new Point(3, 3);
			this.time_1.Name = "time_1";
			this.time_1.SelectIndex = -1;
			this.time_1.Size = new Size(664, 65);
			this.time_1.TabIndex = 172;
			this.time_1.TabStop = false;
			this.time_1.WidthEx = 20;
			this.time_1.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_1.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_1.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_6.FontSize = 9;
			this.time_6.HeightEx = 5;
			this.time_6.IFont = new Font("SimSun", 9f);
			this.time_6.IsCreateSize = false;
			this.time_6.KeyMove = 2;
			this.time_6.Location = new Point(3, 268);
			this.time_6.Name = "time_6";
			this.time_6.SelectIndex = -1;
			this.time_6.Size = new Size(664, 43);
			this.time_6.TabIndex = 177;
			this.time_6.TabStop = false;
			this.time_6.WidthEx = 20;
			this.time_6.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_6.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_6.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_5.FontSize = 9;
			this.time_5.HeightEx = 5;
			this.time_5.IFont = new Font("SimSun", 9f);
			this.time_5.IsCreateSize = false;
			this.time_5.KeyMove = 2;
			this.time_5.Location = new Point(3, 218);
			this.time_5.Name = "time_5";
			this.time_5.SelectIndex = -1;
			this.time_5.Size = new Size(664, 43);
			this.time_5.TabIndex = 176;
			this.time_5.TabStop = false;
			this.time_5.WidthEx = 20;
			this.time_5.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_5.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_5.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_3.FontSize = 9;
			this.time_3.HeightEx = 5;
			this.time_3.IFont = new Font("SimSun", 9f);
			this.time_3.IsCreateSize = false;
			this.time_3.KeyMove = 2;
			this.time_3.Location = new Point(3, 118);
			this.time_3.Name = "time_3";
			this.time_3.SelectIndex = -1;
			this.time_3.Size = new Size(664, 43);
			this.time_3.TabIndex = 174;
			this.time_3.TabStop = false;
			this.time_3.WidthEx = 20;
			this.time_3.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_3.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_3.KeyUpEvent += this.time_1_KeyUpEvent;
			this.time_4.FontSize = 9;
			this.time_4.HeightEx = 5;
			this.time_4.IFont = new Font("SimSun", 9f);
			this.time_4.IsCreateSize = false;
			this.time_4.KeyMove = 2;
			this.time_4.Location = new Point(3, 168);
			this.time_4.Name = "time_4";
			this.time_4.SelectIndex = -1;
			this.time_4.Size = new Size(664, 43);
			this.time_4.TabIndex = 175;
			this.time_4.TabStop = false;
			this.time_4.WidthEx = 20;
			this.time_4.SelectIndexChangedEvent += this.time_1_SelectIndexChangedEvent;
			this.time_4.TimeValueChangedEvent += this.time_1_TimeValueChangedEvent;
			this.time_4.KeyUpEvent += this.time_1_KeyUpEvent;
			this.btn_help.AccessibleRole = AccessibleRole.PushButton;
			this.btn_help.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_help.Location = new Point(697, 13);
			this.btn_help.Name = "btn_help";
			this.btn_help.Size = new Size(68, 25);
			this.btn_help.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_help.TabIndex = 3;
			this.btn_help.Text = "帮助？";
			this.btn_help.Click += this.btn_help_Click;
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(359, 18);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 13);
			this.label2.TabIndex = 187;
			this.label2.Text = "*";
			this.txt_start.EditValue = null;
			this.txt_start.Location = new Point(275, 636);
			this.txt_start.Name = "txt_start";
			this.txt_start.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance, "", null, null, true)
			});
			this.txt_start.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.txt_start.Properties.Mask.EditMask = "HH:mm";
			this.txt_start.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.txt_start.Size = new Size(74, 20);
			this.txt_start.TabIndex = 188;
			this.txt_start.EditValueChanged += this.SetTime;
			this.txt_end.EditValue = null;
			this.txt_end.Location = new Point(462, 636);
			this.txt_end.Name = "txt_end";
			this.txt_end.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance2, "", null, null, true)
			});
			this.txt_end.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.txt_end.Properties.Mask.EditMask = "HH:mm";
			this.txt_end.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.txt_end.Size = new Size(74, 20);
			this.txt_end.TabIndex = 189;
			this.txt_end.EditValueChanged += this.SetTime;
			this.cbbTZ1Id.Enabled = false;
			this.cbbTZ1Id.FormattingEnabled = true;
			this.cbbTZ1Id.Location = new Point(169, 20);
			this.cbbTZ1Id.Name = "cbbTZ1Id";
			this.cbbTZ1Id.Size = new Size(44, 21);
			this.cbbTZ1Id.TabIndex = 190;
			this.lblTZ1.AutoSize = true;
			this.lblTZ1.Location = new Point(6, 25);
			this.lblTZ1.Name = "lblTZ1";
			this.lblTZ1.Size = new Size(61, 13);
			this.lblTZ1.TabIndex = 191;
			this.lblTZ1.Text = "时段1编号";
			this.lblTZ2.AutoSize = true;
			this.lblTZ2.Location = new Point(268, 25);
			this.lblTZ2.Name = "lblTZ2";
			this.lblTZ2.Size = new Size(61, 13);
			this.lblTZ2.TabIndex = 193;
			this.lblTZ2.Text = "时段2编号";
			this.cbbTZ2Id.Enabled = false;
			this.cbbTZ2Id.FormattingEnabled = true;
			this.cbbTZ2Id.Location = new Point(362, 20);
			this.cbbTZ2Id.Name = "cbbTZ2Id";
			this.cbbTZ2Id.Size = new Size(44, 21);
			this.cbbTZ2Id.TabIndex = 192;
			this.lblTZ3.AutoSize = true;
			this.lblTZ3.Location = new Point(458, 25);
			this.lblTZ3.Name = "lblTZ3";
			this.lblTZ3.Size = new Size(61, 13);
			this.lblTZ3.TabIndex = 195;
			this.lblTZ3.Text = "时段3编号";
			this.cbbTZ3Id.Enabled = false;
			this.cbbTZ3Id.FormattingEnabled = true;
			this.cbbTZ3Id.Location = new Point(552, 20);
			this.cbbTZ3Id.Name = "cbbTZ3Id";
			this.cbbTZ3Id.Size = new Size(44, 21);
			this.cbbTZ3Id.TabIndex = 194;
			this.lblTZHoliday.AutoSize = true;
			this.lblTZHoliday.Location = new Point(655, 25);
			this.lblTZHoliday.Name = "lblTZHoliday";
			this.lblTZHoliday.Size = new Size(79, 13);
			this.lblTZHoliday.TabIndex = 197;
			this.lblTZHoliday.Text = "假日时段编号";
			this.cbbTZHolidayId.Enabled = false;
			this.cbbTZHolidayId.FormattingEnabled = true;
			this.cbbTZHolidayId.Location = new Point(749, 20);
			this.cbbTZHolidayId.Name = "cbbTZHolidayId";
			this.cbbTZHolidayId.Size = new Size(44, 21);
			this.cbbTZHolidayId.TabIndex = 196;
			this.groupBox1.Controls.Add(this.cbbTZ1Id);
			this.groupBox1.Controls.Add(this.lblTZHoliday);
			this.groupBox1.Controls.Add(this.lblTZ1);
			this.groupBox1.Controls.Add(this.cbbTZHolidayId);
			this.groupBox1.Controls.Add(this.cbbTZ2Id);
			this.groupBox1.Controls.Add(this.lblTZ3);
			this.groupBox1.Controls.Add(this.lblTZ2);
			this.groupBox1.Controls.Add(this.cbbTZ3Id);
			this.groupBox1.Location = new Point(12, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(805, 49);
			this.groupBox1.TabIndex = 198;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "脱机设备参数";
			this.btn_help_diferences.FlatAppearance.BorderSize = 0;
			this.btn_help_diferences.Image = Resources.help_opera;
			this.btn_help_diferences.Location = new Point(779, 12);
			this.btn_help_diferences.Margin = new System.Windows.Forms.Padding(0);
			this.btn_help_diferences.Name = "btn_help_diferences";
			this.btn_help_diferences.Size = new Size(26, 23);
			this.btn_help_diferences.TabIndex = 199;
			this.btn_help_diferences.UseVisualStyleBackColor = true;
			this.btn_help_diferences.Click += this.btn_help_diferences_Click;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(829, 673);
			base.Controls.Add(this.btn_help_diferences);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.txt_end);
			base.Controls.Add(this.txt_start);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.btn_help);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.lb_end);
			base.Controls.Add(this.lb_satrt);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_hliday3);
			base.Controls.Add(this.lb_hliday2);
			base.Controls.Add(this.lb_hliday1);
			base.Controls.Add(this.lb_sunday);
			base.Controls.Add(this.lb_saturday);
			base.Controls.Add(this.lb_friday);
			base.Controls.Add(this.lb_thursday);
			base.Controls.Add(this.lb_wednesday);
			base.Controls.Add(this.lb_tuesday);
			base.Controls.Add(this.lb_monday);
			base.Controls.Add(this.txt_remark);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.lb_remark);
			base.Controls.Add(this.lb_name);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TimeSet";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "门禁时间段";
			this.panel1.ResumeLayout(false);
			((ISupportInitialize)this.txt_start.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.txt_start.Properties).EndInit();
			((ISupportInitialize)this.txt_end.Properties.CalendarTimeProperties).EndInit();
			((ISupportInitialize)this.txt_end.Properties).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
