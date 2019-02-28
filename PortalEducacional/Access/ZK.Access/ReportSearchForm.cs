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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class ReportSearchForm : Office2007Form
	{
		public string sqlConditionStr;

		private Dictionary<int, string> deviceList = new Dictionary<int, string>();

		private Dictionary<int, string> eventTypeList = new Dictionary<int, string>();

		private Dictionary<int, string> stateList = new Dictionary<int, string>();

		private Dictionary<int, string> verifiedList = new Dictionary<int, string>();

		private IContainer components = null;

		private Label lbl_dateTime;

		private Label label2;

		private Label lbl_userPin;

		private TextBox txt_userPin;

		private Label lbl_cardNo;

		private TextBox txt_cardNo;

		private Label lbl_deviceName;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private System.Windows.Forms.ComboBox cmb_devName;

		private Label lbl_verified;

		private System.Windows.Forms.ComboBox cmb_verified;

		private System.Windows.Forms.ComboBox cmb_inoutState;

		private Label lbl_inoutState;

		private System.Windows.Forms.ComboBox cmb_eventType;

		private Label lbl_evnetType;

		private DateEdit dtBeginTime;

		private DateEdit dtBeginDate;

		private DateEdit dtEndTime;

		private DateEdit dtEndDate;

		public ReportSearchForm()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.LoadDevInfo();
			this.LoadEventType();
			this.LoadStateInfo();
			this.LoadverifiedInfo();
		}

		private void LoadDevInfo()
		{
			this.deviceList.Clear();
			this.cmb_devName.Items.Clear();
			this.cmb_devName.Items.Add("-----");
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.deviceList.Add(list[i].ID, list[i].MachineAlias);
					this.cmb_devName.Items.Add(list[i].MachineAlias);
				}
			}
			initLang.ComboBoxAutoSize(this.cmb_devName, this);
			this.cmb_devName.SelectedIndex = 0;
		}

		private void LoadEventType()
		{
			this.eventTypeList.Clear();
			this.cmb_eventType.Items.Clear();
			this.cmb_eventType.Items.Add("-----");
			this.eventTypeList = PullSDKEventInfos.GetDic();
			foreach (int key in this.eventTypeList.Keys)
			{
				this.cmb_eventType.Items.Add(this.eventTypeList[key]);
			}
			initLang.ComboBoxAutoSize(this.cmb_eventType, this);
			this.cmb_eventType.SelectedIndex = 0;
		}

		private void LoadverifiedInfo()
		{
			this.verifiedList.Clear();
			this.cmb_verified.Items.Clear();
			this.cmb_verified.Items.Add("-----");
			this.verifiedList = PullSDKVerifyTypeInfos.GetDic();
			foreach (int key in this.verifiedList.Keys)
			{
				this.cmb_verified.Items.Add(this.verifiedList[key]);
			}
			initLang.ComboBoxAutoSize(this.cmb_verified, this);
			this.cmb_verified.SelectedIndex = 0;
		}

		private void LoadStateInfo()
		{
			this.stateList.Clear();
			this.cmb_inoutState.Items.Clear();
			this.cmb_inoutState.Items.Add("-----");
			this.stateList = InOutStateInfo.GetDic();
			foreach (int key in this.stateList.Keys)
			{
				this.cmb_inoutState.Items.Add(this.stateList[key]);
			}
			initLang.ComboBoxAutoSize(this.cmb_inoutState, this);
			this.cmb_inoutState.SelectedIndex = 0;
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_userPin_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e);
		}

		private void txt_cardNo_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e);
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DateTime dateTime = (DateTime)this.dtBeginDate.EditValue;
			string str = dateTime.ToString("yyyy-MM-dd");
			dateTime = (DateTime)this.dtBeginTime.EditValue;
			DateTime dateTime2 = DateTime.Parse(str + " " + dateTime.ToString("HH:mm:ss"));
			dateTime = (DateTime)this.dtEndDate.EditValue;
			string str2 = dateTime.ToString("yyyy-MM-dd");
			dateTime = (DateTime)this.dtEndTime.EditValue;
			DateTime dateTime3 = DateTime.Parse(str2 + " " + dateTime.ToString("HH:mm:ss"));
			if (AppSite.Instance.DataType == DataType.SqlServer)
			{
				stringBuilder.Append("time>='" + dateTime2.ToString("yyyy-MM-dd HH:mm:ss") + "' and time<='" + dateTime3.ToString("yyyy-MM-dd HH:mm:ss") + "'");
			}
			else
			{
				stringBuilder.Append("time>=#" + dateTime2.ToString("yyyy-MM-dd HH:mm:ss") + "# and time<=#" + dateTime3.ToString("yyyy-MM-dd HH:mm:ss") + "#");
			}
			if (!string.IsNullOrEmpty(this.txt_cardNo.Text))
			{
				stringBuilder.Append(" and card_no ='" + this.txt_cardNo.Text + "'");
			}
			if (!string.IsNullOrEmpty(this.txt_userPin.Text))
			{
				stringBuilder.Append(" and pin='" + this.txt_userPin.Text + "'");
			}
			if (!string.IsNullOrEmpty(this.cmb_devName.Text) && this.cmb_devName.Text != "-----")
			{
				stringBuilder.Append(" and device_name='" + this.cmb_devName.Text + "'");
			}
			if (this.cmb_verified.SelectedIndex != 0 && this.verifiedList != null)
			{
				foreach (KeyValuePair<int, string> verified in this.verifiedList)
				{
					if (verified.Value == this.cmb_verified.Text)
					{
						stringBuilder.Append(" and verified=" + verified.Key);
						break;
					}
				}
			}
			if (this.cmb_inoutState.SelectedIndex != 0 && this.stateList != null)
			{
				foreach (KeyValuePair<int, string> state in this.stateList)
				{
					if (state.Value == this.cmb_inoutState.Text)
					{
						stringBuilder.Append(" and state=" + state.Key);
						break;
					}
				}
			}
			if (this.cmb_eventType.SelectedIndex != 0 && this.eventTypeList != null)
			{
				foreach (KeyValuePair<int, string> eventType in this.eventTypeList)
				{
					if (eventType.Value == this.cmb_eventType.Text)
					{
						stringBuilder.Append(" and event_type=" + eventType.Key);
						break;
					}
				}
			}
			this.sqlConditionStr = stringBuilder.ToString();
			base.DialogResult = DialogResult.OK;
		}

		private void ReportSearchForm_Load(object sender, EventArgs e)
		{
			this.dtBeginDate.EditValue = DateTime.Now.AddDays(-7.0);
			this.dtBeginTime.EditValue = new DateTime(2000, 1, 1, 0, 0, 0);
			this.dtEndDate.EditValue = DateTime.Now;
			this.dtEndTime.EditValue = new DateTime(2000, 1, 1, 23, 59, 59);
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
			SerializableAppearanceObject appearance = new SerializableAppearanceObject();
			SerializableAppearanceObject appearance2 = new SerializableAppearanceObject();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ReportSearchForm));
			this.lbl_dateTime = new Label();
			this.label2 = new Label();
			this.lbl_userPin = new Label();
			this.txt_userPin = new TextBox();
			this.lbl_cardNo = new Label();
			this.txt_cardNo = new TextBox();
			this.lbl_deviceName = new Label();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.cmb_devName = new System.Windows.Forms.ComboBox();
			this.lbl_verified = new Label();
			this.cmb_verified = new System.Windows.Forms.ComboBox();
			this.cmb_inoutState = new System.Windows.Forms.ComboBox();
			this.lbl_inoutState = new Label();
			this.cmb_eventType = new System.Windows.Forms.ComboBox();
			this.lbl_evnetType = new Label();
			this.dtBeginTime = new DateEdit();
			this.dtBeginDate = new DateEdit();
			this.dtEndTime = new DateEdit();
			this.dtEndDate = new DateEdit();
			((ISupportInitialize)this.dtBeginTime.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.dtBeginTime.Properties).BeginInit();
			((ISupportInitialize)this.dtBeginDate.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.dtBeginDate.Properties).BeginInit();
			((ISupportInitialize)this.dtEndTime.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.dtEndTime.Properties).BeginInit();
			((ISupportInitialize)this.dtEndDate.Properties.VistaTimeProperties).BeginInit();
			((ISupportInitialize)this.dtEndDate.Properties).BeginInit();
			base.SuspendLayout();
			this.lbl_dateTime.Location = new Point(12, 27);
			this.lbl_dateTime.Name = "lbl_dateTime";
			this.lbl_dateTime.Size = new Size(101, 12);
			this.lbl_dateTime.TabIndex = 0;
			this.lbl_dateTime.Text = "起止时间";
			this.lbl_dateTime.TextAlign = ContentAlignment.MiddleLeft;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(313, 27);
			this.label2.Name = "label2";
			this.label2.Size = new Size(23, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "---";
			this.lbl_userPin.Location = new Point(12, 70);
			this.lbl_userPin.Name = "lbl_userPin";
			this.lbl_userPin.Size = new Size(101, 12);
			this.lbl_userPin.TabIndex = 7;
			this.lbl_userPin.Text = "人员编号";
			this.lbl_userPin.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_userPin.Location = new Point(123, 67);
			this.txt_userPin.Name = "txt_userPin";
			this.txt_userPin.Size = new Size(121, 21);
			this.txt_userPin.TabIndex = 5;
			this.txt_userPin.KeyPress += this.txt_userPin_KeyPress;
			this.lbl_cardNo.Location = new Point(309, 71);
			this.lbl_cardNo.Name = "lbl_cardNo";
			this.lbl_cardNo.Size = new Size(92, 12);
			this.lbl_cardNo.TabIndex = 9;
			this.lbl_cardNo.Text = "卡号";
			this.lbl_cardNo.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_cardNo.Location = new Point(407, 67);
			this.txt_cardNo.Name = "txt_cardNo";
			this.txt_cardNo.Size = new Size(121, 21);
			this.txt_cardNo.TabIndex = 6;
			this.txt_cardNo.KeyPress += this.txt_cardNo_KeyPress;
			this.lbl_deviceName.Location = new Point(12, 114);
			this.lbl_deviceName.Name = "lbl_deviceName";
			this.lbl_deviceName.Size = new Size(101, 12);
			this.lbl_deviceName.TabIndex = 11;
			this.lbl_deviceName.Text = "设备名称";
			this.lbl_deviceName.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(446, 204);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 12;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(344, 204);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 11;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.cmb_devName.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_devName.FormattingEnabled = true;
			this.cmb_devName.Location = new Point(123, 111);
			this.cmb_devName.Name = "cmb_devName";
			this.cmb_devName.Size = new Size(121, 20);
			this.cmb_devName.TabIndex = 7;
			this.lbl_verified.Location = new Point(309, 114);
			this.lbl_verified.Name = "lbl_verified";
			this.lbl_verified.Size = new Size(92, 12);
			this.lbl_verified.TabIndex = 20;
			this.lbl_verified.Text = "验证方式";
			this.lbl_verified.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_verified.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_verified.FormattingEnabled = true;
			this.cmb_verified.Location = new Point(407, 110);
			this.cmb_verified.Name = "cmb_verified";
			this.cmb_verified.Size = new Size(121, 20);
			this.cmb_verified.TabIndex = 8;
			this.cmb_inoutState.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_inoutState.FormattingEnabled = true;
			this.cmb_inoutState.Location = new Point(123, 154);
			this.cmb_inoutState.Name = "cmb_inoutState";
			this.cmb_inoutState.Size = new Size(121, 20);
			this.cmb_inoutState.TabIndex = 9;
			this.lbl_inoutState.Location = new Point(12, 157);
			this.lbl_inoutState.Name = "lbl_inoutState";
			this.lbl_inoutState.Size = new Size(101, 12);
			this.lbl_inoutState.TabIndex = 22;
			this.lbl_inoutState.Text = "出入状态";
			this.lbl_inoutState.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_eventType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_eventType.FormattingEnabled = true;
			this.cmb_eventType.Location = new Point(407, 154);
			this.cmb_eventType.Name = "cmb_eventType";
			this.cmb_eventType.Size = new Size(121, 20);
			this.cmb_eventType.TabIndex = 10;
			this.lbl_evnetType.Location = new Point(309, 158);
			this.lbl_evnetType.Name = "lbl_evnetType";
			this.lbl_evnetType.Size = new Size(92, 12);
			this.lbl_evnetType.TabIndex = 24;
			this.lbl_evnetType.Text = "事件描述";
			this.lbl_evnetType.TextAlign = ContentAlignment.MiddleLeft;
			this.dtBeginTime.EditValue = null;
			this.dtBeginTime.Location = new Point(232, 23);
			this.dtBeginTime.Name = "dtBeginTime";
			this.dtBeginTime.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance, "", null, null, true)
			});
			this.dtBeginTime.Properties.Mask.EditMask = "HH:mm";
			this.dtBeginTime.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtBeginTime.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtBeginTime.Size = new Size(71, 21);
			this.dtBeginTime.TabIndex = 26;
			this.dtBeginDate.EditValue = null;
			this.dtBeginDate.Location = new Point(123, 23);
			this.dtBeginDate.Name = "dtBeginDate";
			this.dtBeginDate.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dtBeginDate.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.dtBeginDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtBeginDate.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtBeginDate.Size = new Size(105, 21);
			this.dtBeginDate.TabIndex = 25;
			this.dtEndTime.EditValue = null;
			this.dtEndTime.Location = new Point(454, 23);
			this.dtEndTime.Name = "dtEndTime";
			this.dtEndTime.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo, "", -1, true, false, false, ImageLocation.MiddleCenter, null, new KeyShortcut(Keys.None), appearance2, "", null, null, true)
			});
			this.dtEndTime.Properties.Mask.EditMask = "HH:mm";
			this.dtEndTime.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtEndTime.Size = new Size(74, 21);
			this.dtEndTime.TabIndex = 28;
			this.dtEndDate.EditValue = null;
			this.dtEndDate.Location = new Point(346, 23);
			this.dtEndDate.Name = "dtEndDate";
			this.dtEndDate.Properties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton(ButtonPredefines.Combo)
			});
			this.dtEndDate.Properties.Mask.EditMask = "yyyy-MM-dd";
			this.dtEndDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			this.dtEndDate.Properties.VistaTimeProperties.Buttons.AddRange(new EditorButton[1]
			{
				new EditorButton()
			});
			this.dtEndDate.Size = new Size(105, 21);
			this.dtEndDate.TabIndex = 27;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(541, 239);
			base.Controls.Add(this.dtEndTime);
			base.Controls.Add(this.dtEndDate);
			base.Controls.Add(this.dtBeginTime);
			base.Controls.Add(this.dtBeginDate);
			base.Controls.Add(this.cmb_eventType);
			base.Controls.Add(this.cmb_verified);
			base.Controls.Add(this.txt_cardNo);
			base.Controls.Add(this.cmb_inoutState);
			base.Controls.Add(this.cmb_devName);
			base.Controls.Add(this.txt_userPin);
			base.Controls.Add(this.lbl_evnetType);
			base.Controls.Add(this.lbl_inoutState);
			base.Controls.Add(this.lbl_verified);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lbl_deviceName);
			base.Controls.Add(this.lbl_cardNo);
			base.Controls.Add(this.lbl_userPin);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.lbl_dateTime);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ReportSearchForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "查找";
			base.Load += this.ReportSearchForm_Load;
			((ISupportInitialize)this.dtBeginTime.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.dtBeginTime.Properties).EndInit();
			((ISupportInitialize)this.dtBeginDate.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.dtBeginDate.Properties).EndInit();
			((ISupportInitialize)this.dtEndTime.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.dtEndTime.Properties).EndInit();
			((ISupportInitialize)this.dtEndDate.Properties.VistaTimeProperties).EndInit();
			((ISupportInitialize)this.dtEndDate.Properties).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
