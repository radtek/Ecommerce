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
using ZK.Utils;

namespace ZK.Access.door
{
	public class STD_WiegandSetting : Office2007Form
	{
		private AccDoor door;

		private Machines Machine;

		private STD_WiegandFmt wiegandFmt;

		private STD_WiegandFmt oldwiegandFmt;

		private int DoorId;

		private DataTable dtOutFmt;

		private DataTable dtInContent;

		private DataTable dtOutContent;

		private Dictionary<string, string> dicOutFmt;

		private Dictionary<string, string> dicName_Fmt;

		private IContainer components = null;

		private GroupBox groupBox1;

		private ComboBox cbbOutType;

		private Label label6;

		private Label label4;

		private NumericUpDown numPulseIntervalOut;

		private Label label5;

		private Label label3;

		private NumericUpDown numPulseWidthOut;

		private Label label2;

		private NumericUpDown numAreaCode;

		private NumericUpDown numFailureId;

		private CheckBox ckbAreaCode;

		private CheckBox ckbFailureId;

		private Label label1;

		private ComboBox cbbWiegandFmtOut;

		private GroupBox groupBox2;

		private ComboBox cbbInType;

		private Label label8;

		private Label label10;

		private NumericUpDown numPulseIntervalIn;

		private Label label11;

		private Label label12;

		private NumericUpDown numPulseWidthIn;

		private Label label13;

		private NumericUpDown numBitNumber;

		private Label label9;

		private TextBox txtWiegandFmtIn;

		private Label label7;

		private Panel panel1;

		private ButtonX btnCancel;

		private ButtonX btnOK;

		public STD_WiegandSetting(int doorId)
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.DoorId = doorId;
			this.LoadDoorMachine();
			this.InitialDataSource();
			this.BindData();
		}

		private void LoadDoorMachine()
		{
			if (this.DoorId > 0)
			{
				AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
				this.door = accDoorBll.GetModel(this.DoorId);
				if (this.door != null)
				{
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					this.Machine = machinesBll.GetModel(this.door.device_id);
				}
			}
		}

		private void ckbFailureId_CheckedChanged(object sender, EventArgs e)
		{
			this.numFailureId.Enabled = this.ckbFailureId.Checked;
		}

		private void ckbAreaCode_CheckedChanged(object sender, EventArgs e)
		{
			this.numAreaCode.Enabled = this.ckbAreaCode.Checked;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void InitialDataSource()
		{
			this.dtOutFmt = new DataTable();
			this.dtOutFmt.Columns.Add("Id", typeof(string));
			this.dtOutFmt.Columns.Add("Name", typeof(string));
			this.dicOutFmt = new Dictionary<string, string>();
			this.dicName_Fmt = new Dictionary<string, string>();
			this.dicOutFmt.Add("26", "Wiegand26 with Device ID(Site Code)");
			this.dicOutFmt.Add("PeeeeeeeeEEEEEEEEOOOOOOOOOOOOOOOOP", "Wiegand34 with Device ID(Site Code)");
			this.dicOutFmt.Add("PEEEEEEEEEEEEOOOOOOOOOOOOP", "Wiegand26");
			this.dicOutFmt.Add("PEEEEEEEEEEEEEEEEOOOOOOOOOOOOOOOOP", "Wiegand34");
			if (this.Machine != null)
			{
				this.dicOutFmt.Clear();
				if (this.Machine.IsTFTMachine)
				{
					this.dicOutFmt.Add("pssssssssccccccccccccccccp:eeeeeeeeeeeeeooooooooooooo", "Wiegand26 with Device ID(Site Code)");
					this.dicOutFmt.Add("pssssssssccccccccccccccccccccccccp:eeeeeeeeeeeeeeeeeooooooooooooooooo", "Wiegand34 with Device ID(Site Code)");
					this.dicOutFmt.Add("pccccccccccccccccccccccccp:eeeeeeeeeeeeeooooooooooooo", "Wiegand26");
					this.dicOutFmt.Add("pccccccccccccccccccccccccccccccccp:eeeeeeeeeeeeeeeeeooooooooooooooooo", "Wiegand34");
				}
				else
				{
					this.dicOutFmt.Add("PeeeeeeeeEEEEOOOOOOOOOOOOP", "Wiegand26 with Device ID(Site Code)");
					this.dicOutFmt.Add("PeeeeeeeeEEEEEEEEOOOOOOOOOOOOOOOOP", "Wiegand34 with Device ID(Site Code)");
					this.dicOutFmt.Add("PEEEEEEEEEEEEOOOOOOOOOOOOP", "Wiegand26");
					this.dicOutFmt.Add("PEEEEEEEEEEEEEEEEOOOOOOOOOOOOOOOOP", "Wiegand34");
				}
			}
			foreach (KeyValuePair<string, string> item in this.dicOutFmt)
			{
				if (!this.dicName_Fmt.ContainsKey(item.Value))
				{
					this.dicName_Fmt.Add(item.Value, item.Key);
				}
			}
			this.cbbWiegandFmtOut.Items.Clear();
			if (this.dicOutFmt != null)
			{
				foreach (KeyValuePair<string, string> item2 in this.dicOutFmt)
				{
					this.cbbWiegandFmtOut.Items.Add(item2.Value);
				}
			}
			this.dtInContent = new DataTable();
			this.dtInContent.Columns.Add("Id", typeof(int));
			this.dtInContent.Columns.Add("Name", typeof(string));
			Dictionary<int, string> dic = STD_WiegandInOutContent.GetDic();
			if (this.dicOutFmt != null)
			{
				foreach (KeyValuePair<int, string> item3 in dic)
				{
					DataRow dataRow = this.dtInContent.NewRow();
					dataRow["Id"] = item3.Key;
					dataRow["Name"] = item3.Value;
					this.dtInContent.Rows.Add(dataRow);
				}
			}
			this.cbbInType.ValueMember = "Id";
			this.cbbInType.DisplayMember = "Name";
			this.cbbInType.DataSource = this.dtInContent;
			this.dtOutContent = this.dtInContent.Copy();
			this.cbbOutType.ValueMember = "Id";
			this.cbbOutType.DisplayMember = "Name";
			this.cbbOutType.DataSource = this.dtOutContent;
		}

		private void BindData()
		{
			try
			{
				STD_WiegandFmtBll sTD_WiegandFmtBll = new STD_WiegandFmtBll(MainForm._ia);
				this.wiegandFmt = sTD_WiegandFmtBll.GetModelByDoorId(this.DoorId);
				this.oldwiegandFmt = sTD_WiegandFmtBll.GetModelByDoorId(this.DoorId);
				if (this.wiegandFmt != null)
				{
					if (this.dicOutFmt == null)
					{
						this.dicOutFmt = new Dictionary<string, string>();
					}
					if (!this.dicOutFmt.ContainsKey(this.wiegandFmt.OutWiegandFmt))
					{
						this.dicOutFmt.Add(this.wiegandFmt.OutWiegandFmt, this.wiegandFmt.OutWiegandFmt);
						if (!this.dicName_Fmt.ContainsKey(this.wiegandFmt.OutWiegandFmt))
						{
							this.dicName_Fmt.Add(this.wiegandFmt.OutWiegandFmt, this.wiegandFmt.OutWiegandFmt);
						}
						if (!this.cbbWiegandFmtOut.Items.Contains(this.wiegandFmt.OutWiegandFmt))
						{
							this.cbbWiegandFmtOut.Items.Add(this.wiegandFmt.OutWiegandFmt);
						}
						DataRow dataRow = this.dtOutFmt.NewRow();
						dataRow["Id"] = this.wiegandFmt.OutWiegandFmt;
						dataRow["Name"] = this.wiegandFmt.OutWiegandFmt;
						this.dtOutFmt.Rows.Add(dataRow);
					}
					this.cbbWiegandFmtOut.Text = this.dicOutFmt[this.wiegandFmt.OutWiegandFmt];
					this.ckbAreaCode.Checked = (this.wiegandFmt.OutAreaCode >= 0);
					this.numAreaCode.Enabled = (this.wiegandFmt.OutAreaCode >= 0);
					if (this.wiegandFmt.OutAreaCode >= 0)
					{
						this.numAreaCode.Value = this.wiegandFmt.OutAreaCode;
					}
					this.ckbFailureId.Checked = (this.wiegandFmt.OutFailureId >= 0);
					this.numFailureId.Enabled = (this.wiegandFmt.OutFailureId >= 0);
					if (this.wiegandFmt.OutFailureId >= 0)
					{
						this.numFailureId.Value = this.wiegandFmt.OutFailureId;
					}
					this.numPulseIntervalOut.Value = this.wiegandFmt.OutPulseInterval;
					this.numPulseWidthOut.Value = this.wiegandFmt.OutPulseWidth;
					this.cbbOutType.SelectedValue = this.wiegandFmt.OutContent;
					this.txtWiegandFmtIn.Text = this.wiegandFmt.InWiegandFmt;
					this.numBitNumber.Value = this.wiegandFmt.InBitCount;
					this.numPulseIntervalIn.Value = this.wiegandFmt.InPulseInterval;
					this.numPulseWidthIn.Value = this.wiegandFmt.InPulseWidth;
					this.cbbInType.SelectedValue = this.wiegandFmt.InContent;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private bool BindModel(STD_WiegandFmt WgFmt)
		{
			if (!this.Check())
			{
				return false;
			}
			WgFmt.InBitCount = (int)this.numBitNumber.Value;
			WgFmt.InContent = (int)this.cbbInType.SelectedValue;
			WgFmt.InPulseInterval = (int)this.numPulseIntervalIn.Value;
			WgFmt.InPulseWidth = (int)this.numPulseWidthIn.Value;
			WgFmt.InWiegandFmt = this.txtWiegandFmtIn.Text;
			WgFmt.OutAreaCode = (this.ckbAreaCode.Checked ? ((int)this.numAreaCode.Value) : (-1));
			WgFmt.OutContent = (int)this.cbbOutType.SelectedValue;
			WgFmt.OutFailureId = (this.ckbFailureId.Checked ? ((int)this.numFailureId.Value) : (-1));
			WgFmt.OutPulseInterval = (int)this.numPulseIntervalOut.Value;
			WgFmt.OutPulseWidth = (int)this.numPulseWidthOut.Value;
			if (this.cbbWiegandFmtOut.Text.Trim() != "" && this.dicName_Fmt.ContainsKey(this.cbbWiegandFmtOut.Text))
			{
				WgFmt.OutWiegandFmt = this.dicName_Fmt[this.cbbWiegandFmtOut.Text];
			}
			return true;
		}

		private bool Check()
		{
			return true;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.BindModel(this.wiegandFmt))
				{
					STD_WiegandFmtBll sTD_WiegandFmtBll = new STD_WiegandFmtBll(MainForm._ia);
					sTD_WiegandFmtBll.Update(this.wiegandFmt);
					if (this.IsModelChanged(this.oldwiegandFmt, this.wiegandFmt) && this.door != null)
					{
						if (this.Machine != null)
						{
							DevCmdsBll devCmdsBll = new DevCmdsBll(MainForm._ia);
							DevCmds devCmds = new DevCmds();
							devCmds.SN_id = this.door.device_id;
							devCmds.status = 0;
							devCmds.CmdContent = "setwiegand$" + this.wiegandFmt.ToPullCmdString(this.Machine);
							devCmds.CmdCommitTime = DateTime.Now;
							devCmds.CmdReturnContent = string.Empty;
							devCmdsBll.Add(devCmds);
						}
						FrmShowUpdata.Instance.ShowEx();
					}
					base.Close();
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowErrorMessage(ex.Message);
			}
		}

		private bool IsModelChanged(STD_WiegandFmt oldWGFmt, STD_WiegandFmt newWGFmt)
		{
			if (oldWGFmt.InBitCount != newWGFmt.InBitCount || oldWGFmt.InContent != newWGFmt.InContent || oldWGFmt.InPulseInterval != newWGFmt.InPulseInterval || oldWGFmt.InPulseWidth != newWGFmt.InPulseWidth || oldWGFmt.InWiegandFmt != newWGFmt.InWiegandFmt || oldWGFmt.OutAreaCode != newWGFmt.OutAreaCode || oldWGFmt.OutContent != newWGFmt.OutContent || oldWGFmt.OutFailureId != newWGFmt.OutFailureId || oldWGFmt.OutPulseInterval != newWGFmt.OutPulseInterval || oldWGFmt.OutPulseWidth != newWGFmt.OutPulseWidth || oldWGFmt.OutWiegandFmt != newWGFmt.OutWiegandFmt)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(STD_WiegandSetting));
			this.groupBox1 = new GroupBox();
			this.cbbOutType = new ComboBox();
			this.label6 = new Label();
			this.label4 = new Label();
			this.numPulseIntervalOut = new NumericUpDown();
			this.label5 = new Label();
			this.label3 = new Label();
			this.numPulseWidthOut = new NumericUpDown();
			this.label2 = new Label();
			this.numAreaCode = new NumericUpDown();
			this.numFailureId = new NumericUpDown();
			this.ckbAreaCode = new CheckBox();
			this.ckbFailureId = new CheckBox();
			this.label1 = new Label();
			this.cbbWiegandFmtOut = new ComboBox();
			this.groupBox2 = new GroupBox();
			this.cbbInType = new ComboBox();
			this.label8 = new Label();
			this.label10 = new Label();
			this.numPulseIntervalIn = new NumericUpDown();
			this.label11 = new Label();
			this.label12 = new Label();
			this.numPulseWidthIn = new NumericUpDown();
			this.label13 = new Label();
			this.numBitNumber = new NumericUpDown();
			this.label9 = new Label();
			this.txtWiegandFmtIn = new TextBox();
			this.label7 = new Label();
			this.panel1 = new Panel();
			this.btnCancel = new ButtonX();
			this.btnOK = new ButtonX();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.numPulseIntervalOut).BeginInit();
			((ISupportInitialize)this.numPulseWidthOut).BeginInit();
			((ISupportInitialize)this.numAreaCode).BeginInit();
			((ISupportInitialize)this.numFailureId).BeginInit();
			this.groupBox2.SuspendLayout();
			((ISupportInitialize)this.numPulseIntervalIn).BeginInit();
			((ISupportInitialize)this.numPulseWidthIn).BeginInit();
			((ISupportInitialize)this.numBitNumber).BeginInit();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.groupBox1.Controls.Add(this.cbbOutType);
			this.groupBox1.Controls.Add(this.numPulseIntervalOut);
			this.groupBox1.Controls.Add(this.numPulseWidthOut);
			this.groupBox1.Controls.Add(this.numAreaCode);
			this.groupBox1.Controls.Add(this.numFailureId);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.ckbAreaCode);
			this.groupBox1.Controls.Add(this.ckbFailureId);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.cbbWiegandFmtOut);
			this.groupBox1.Location = new Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(300, 278);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "输出配置";
			this.cbbOutType.FormattingEnabled = true;
			this.cbbOutType.Location = new Point(161, 241);
			this.cbbOutType.Name = "cbbOutType";
			this.cbbOutType.Size = new Size(133, 20);
			this.cbbOutType.TabIndex = 15;
			this.label6.AutoSize = true;
			this.label6.Location = new Point(6, 245);
			this.label6.Name = "label6";
			this.label6.Size = new Size(53, 12);
			this.label6.TabIndex = 14;
			this.label6.Text = "输出内容";
			this.label4.AutoSize = true;
			this.label4.Location = new Point(236, 207);
			this.label4.Name = "label4";
			this.label4.Size = new Size(29, 12);
			this.label4.TabIndex = 13;
			this.label4.Text = "微秒";
			this.numPulseIntervalOut.Location = new Point(161, 203);
			this.numPulseIntervalOut.Maximum = new decimal(new int[4]
			{
				20000,
				0,
				0,
				0
			});
			this.numPulseIntervalOut.Name = "numPulseIntervalOut";
			this.numPulseIntervalOut.Size = new Size(69, 21);
			this.numPulseIntervalOut.TabIndex = 12;
			this.numPulseIntervalOut.Value = new decimal(new int[4]
			{
				1000,
				0,
				0,
				0
			});
			this.label5.AutoSize = true;
			this.label5.Location = new Point(6, 207);
			this.label5.Name = "label5";
			this.label5.Size = new Size(53, 12);
			this.label5.TabIndex = 11;
			this.label5.Text = "脉冲间隔";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(236, 169);
			this.label3.Name = "label3";
			this.label3.Size = new Size(29, 12);
			this.label3.TabIndex = 10;
			this.label3.Text = "微秒";
			this.numPulseWidthOut.Location = new Point(161, 165);
			this.numPulseWidthOut.Maximum = new decimal(new int[4]
			{
				800,
				0,
				0,
				0
			});
			this.numPulseWidthOut.Name = "numPulseWidthOut";
			this.numPulseWidthOut.Size = new Size(69, 21);
			this.numPulseWidthOut.TabIndex = 9;
			this.numPulseWidthOut.Value = new decimal(new int[4]
			{
				100,
				0,
				0,
				0
			});
			this.label2.AutoSize = true;
			this.label2.Location = new Point(6, 169);
			this.label2.Name = "label2";
			this.label2.Size = new Size(53, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "脉冲宽度";
			this.numAreaCode.Location = new Point(161, 127);
			this.numAreaCode.Maximum = new decimal(new int[4]
			{
				65535,
				0,
				0,
				0
			});
			this.numAreaCode.Name = "numAreaCode";
			this.numAreaCode.Size = new Size(69, 21);
			this.numAreaCode.TabIndex = 7;
			this.numFailureId.Location = new Point(161, 89);
			this.numFailureId.Maximum = new decimal(new int[4]
			{
				65535,
				0,
				0,
				0
			});
			this.numFailureId.Name = "numFailureId";
			this.numFailureId.Size = new Size(69, 21);
			this.numFailureId.TabIndex = 6;
			this.ckbAreaCode.AutoSize = true;
			this.ckbAreaCode.Location = new Point(8, 129);
			this.ckbAreaCode.Name = "ckbAreaCode";
			this.ckbAreaCode.Size = new Size(60, 16);
			this.ckbAreaCode.TabIndex = 4;
			this.ckbAreaCode.Text = "区位码";
			this.ckbAreaCode.UseVisualStyleBackColor = true;
			this.ckbAreaCode.CheckedChanged += this.ckbAreaCode_CheckedChanged;
			this.ckbFailureId.AutoSize = true;
			this.ckbFailureId.Location = new Point(8, 91);
			this.ckbFailureId.Name = "ckbFailureId";
			this.ckbFailureId.Size = new Size(60, 16);
			this.ckbFailureId.TabIndex = 2;
			this.ckbFailureId.Text = "失败ID";
			this.ckbFailureId.UseVisualStyleBackColor = true;
			this.ckbFailureId.CheckedChanged += this.ckbFailureId_CheckedChanged;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(6, 23);
			this.label1.Name = "label1";
			this.label1.Size = new Size(65, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "已定义格式";
			this.cbbWiegandFmtOut.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbbWiegandFmtOut.FormattingEnabled = true;
			this.cbbWiegandFmtOut.Location = new Point(8, 38);
			this.cbbWiegandFmtOut.Name = "cbbWiegandFmtOut";
			this.cbbWiegandFmtOut.Size = new Size(286, 20);
			this.cbbWiegandFmtOut.TabIndex = 0;
			this.groupBox2.Controls.Add(this.cbbInType);
			this.groupBox2.Controls.Add(this.numPulseIntervalIn);
			this.groupBox2.Controls.Add(this.numPulseWidthIn);
			this.groupBox2.Controls.Add(this.numBitNumber);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.txtWiegandFmtIn);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new Point(329, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(300, 278);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "输入配置";
			this.cbbInType.FormattingEnabled = true;
			this.cbbInType.Location = new Point(161, 241);
			this.cbbInType.Name = "cbbInType";
			this.cbbInType.Size = new Size(133, 20);
			this.cbbInType.TabIndex = 23;
			this.label8.AutoSize = true;
			this.label8.Location = new Point(6, 245);
			this.label8.Name = "label8";
			this.label8.Size = new Size(53, 12);
			this.label8.TabIndex = 22;
			this.label8.Text = "输入内容";
			this.label10.AutoSize = true;
			this.label10.Location = new Point(236, 207);
			this.label10.Name = "label10";
			this.label10.Size = new Size(29, 12);
			this.label10.TabIndex = 21;
			this.label10.Text = "微秒";
			this.numPulseIntervalIn.Location = new Point(161, 203);
			this.numPulseIntervalIn.Maximum = new decimal(new int[4]
			{
				10000,
				0,
				0,
				0
			});
			this.numPulseIntervalIn.Name = "numPulseIntervalIn";
			this.numPulseIntervalIn.Size = new Size(69, 21);
			this.numPulseIntervalIn.TabIndex = 20;
			this.label11.AutoSize = true;
			this.label11.Location = new Point(6, 207);
			this.label11.Name = "label11";
			this.label11.Size = new Size(53, 12);
			this.label11.TabIndex = 19;
			this.label11.Text = "脉冲间隔";
			this.label12.AutoSize = true;
			this.label12.Location = new Point(236, 169);
			this.label12.Name = "label12";
			this.label12.Size = new Size(29, 12);
			this.label12.TabIndex = 18;
			this.label12.Text = "微秒";
			this.numPulseWidthIn.Location = new Point(161, 165);
			this.numPulseWidthIn.Maximum = new decimal(new int[4]
			{
				10000,
				0,
				0,
				0
			});
			this.numPulseWidthIn.Name = "numPulseWidthIn";
			this.numPulseWidthIn.Size = new Size(69, 21);
			this.numPulseWidthIn.TabIndex = 17;
			this.label13.AutoSize = true;
			this.label13.Location = new Point(6, 169);
			this.label13.Name = "label13";
			this.label13.Size = new Size(53, 12);
			this.label13.TabIndex = 16;
			this.label13.Text = "脉冲宽度";
			this.numBitNumber.Location = new Point(161, 107);
			this.numBitNumber.Name = "numBitNumber";
			this.numBitNumber.Size = new Size(69, 21);
			this.numBitNumber.TabIndex = 12;
			this.label9.AutoSize = true;
			this.label9.Location = new Point(6, 111);
			this.label9.Name = "label9";
			this.label9.Size = new Size(59, 12);
			this.label9.TabIndex = 11;
			this.label9.Text = "bit位总数";
			this.txtWiegandFmtIn.Location = new Point(8, 38);
			this.txtWiegandFmtIn.Name = "txtWiegandFmtIn";
			this.txtWiegandFmtIn.Size = new Size(286, 21);
			this.txtWiegandFmtIn.TabIndex = 3;
			this.label7.AutoSize = true;
			this.label7.Location = new Point(6, 23);
			this.label7.Name = "label7";
			this.label7.Size = new Size(53, 12);
			this.label7.TabIndex = 2;
			this.label7.Text = "输入格式";
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnOK);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(0, 296);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(641, 47);
			this.panel1.TabIndex = 2;
			this.btnCancel.AccessibleRole = AccessibleRole.PushButton;
			this.btnCancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnCancel.Location = new Point(541, 12);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(82, 23);
			this.btnCancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += this.btnCancel_Click;
			this.btnOK.AccessibleRole = AccessibleRole.PushButton;
			this.btnOK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnOK.Location = new Point(427, 12);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new Size(82, 23);
			this.btnOK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnOK.TabIndex = 19;
			this.btnOK.Text = "确定";
			this.btnOK.Click += this.btnOK_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(641, 343);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "STD_WiegandSetting";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "韦根格式设置";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((ISupportInitialize)this.numPulseIntervalOut).EndInit();
			((ISupportInitialize)this.numPulseWidthOut).EndInit();
			((ISupportInitialize)this.numAreaCode).EndInit();
			((ISupportInitialize)this.numFailureId).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((ISupportInitialize)this.numPulseIntervalIn).EndInit();
			((ISupportInitialize)this.numPulseWidthIn).EndInit();
			((ISupportInitialize)this.numBitNumber).EndInit();
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
