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
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class BatchAddPersonnelForm : Office2007Form
	{
		public delegate void FinishHandle(bool isSuccess);

		public delegate void ShowInfo(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private int addCount = 0;

		private int userID;

		private UserInfo user = null;

		private UserInfoBll bll = new UserInfoBll(MainForm._ia);

		private List<string> m_numbers = new List<string>();

		private Thread m_thread = null;

		private bool m_lock = false;

		private IContainer components = null;

		private ButtonX btn_ok;

		private ButtonX btn_cancel;

		private Label lbl_editNum;

		private GroupBox groupBox1;

		private TextBox txt_format;

		private Label lbl_width;

		private Label lbl_format;

		private NumericUpDown numUpDown_end;

		private NumericUpDown numUpDown_start;

		private Label lbl_end;

		private Label lbl_start;

		private GroupBox groupBox2;

		private CheckBox chk_multiCard;

		private CheckBox chk_officePhone;

		private CheckBox chk_type;

		private CheckBox chk_hire;

		private CheckBox chk_education;

		private CheckBox chk_gender;

		private CheckBox chk_dept;

		private NumericUpDown numUpDown_width;

		private ListView listView1;

		private ColumnHeader columnHeader1;

		private LabelX lb_progress;

		private ProgressBar progressBarUp;

		private TextBox txt_UpLoadInfo;

		private System.Windows.Forms.Timer time_close;

		private Label lbl_notice;

		public event EventHandler refreshDataEvent = null;

		public BatchAddPersonnelForm(int id)
		{
			this.InitializeComponent();
			this.userID = id;
			try
			{
				initLang.LocaleForm(this, base.Name);
				if (this.userID > 0)
				{
					this.user = this.bll.GetModel(this.userID);
					string text = "";
					if (this.user.Name != null)
					{
						text = this.user.Name.ToString() + " " + this.user.LastName;
					}
					this.lbl_editNum.Text = ShowMsgInfos.GetInfo("FromPersonnel", "从人员") + "  " + text + "(" + this.user.BadgeNumber.ToString() + ") " + ShowMsgInfos.GetInfo("CopyTheData", "复制数据");
				}
				else
				{
					this.groupBox2.Enabled = false;
					this.chk_dept.Enabled = false;
					this.chk_gender.Enabled = false;
					this.chk_officePhone.Enabled = false;
					this.chk_education.Enabled = false;
					this.chk_hire.Enabled = false;
					this.chk_type.Enabled = false;
					this.chk_multiCard.Enabled = false;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void SetModel(UserInfo model)
		{
			try
			{
				if (this.userID > 0)
				{
					this.user = this.bll.GetModel(this.userID);
					if (this.chk_dept.Checked)
					{
						model.DefaultDeptId = this.user.DefaultDeptId;
					}
					if (this.chk_gender.Checked)
					{
						model.Gender = this.user.Gender;
					}
					if (string.IsNullOrEmpty(model.Gender))
					{
						model.Gender = "M";
					}
					if (this.chk_officePhone.Checked)
					{
						model.OPhone = this.user.OPhone;
					}
					if (this.chk_education.Checked)
					{
						model.Education = this.user.Education;
					}
					if (this.chk_hire.Checked)
					{
						model.HireType = this.user.HireType;
					}
					if (this.chk_type.Checked)
					{
						model.EmpType = this.user.EmpType;
					}
					if (this.chk_multiCard.Checked)
					{
						model.MorecardGroupId = this.user.MorecardGroupId;
					}
					model.Photo = null;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void ShowUpLoadInfo(string UpLoadinfoStr)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowInfo(this.ShowUpLoadInfo), UpLoadinfoStr);
				}
				else
				{
					this.txt_UpLoadInfo.AppendText(UpLoadinfoStr + Environment.NewLine);
					this.Refresh();
				}
			}
		}

		private void ShowProgress(int prg)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new ShowProgressHandle(this.ShowProgress), prg);
				}
				else
				{
					if (prg >= 100)
					{
						prg = 100;
					}
					if (prg == 0)
					{
						prg = 0;
					}
					this.progressBarUp.Value = prg;
					this.Refresh();
				}
			}
		}

		private void OnFinish(bool isSuccess)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke(new FinishHandle(this.OnFinish), isSuccess);
				}
				else
				{
					if (this.refreshDataEvent != null)
					{
						this.refreshDataEvent(this, null);
					}
					this.Cursor = Cursors.Default;
					this.btn_ok.Enabled = true;
					this.btn_cancel.Enabled = true;
					this.time_close.Enabled = true;
				}
			}
		}

		private void Start()
		{
			try
			{
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SSavingData", "正在保存数据，请稍候..."));
				UserInfoBll userInfoBll = new UserInfoBll(MainForm._ia);
				string format = (AppSite.Instance.DataType == DataType.Access) ? "CLng(Badgenumber)>={0} And CLng(Badgenumber)<={1}" : "CAST(Badgenumber as int)>={0} And CAST(Badgenumber as int)<={1}";
				Dictionary<string, int> badgenumber_UserIdDic = userInfoBll.GetBadgenumber_UserIdDic(string.Format(format, this.m_numbers[0], this.m_numbers[this.m_numbers.Count - 1]));
				StringBuilder stringBuilder = new StringBuilder();
				List<UserInfo> list = new List<UserInfo>();
				for (int i = 0; i < this.m_numbers.Count; i++)
				{
					int num = 90 * (i + 1) / this.m_numbers.Count;
					if (num > this.progressBarUp.Value)
					{
						this.ShowProgress(num);
					}
					if (int.Parse(this.m_numbers[i]) > 0)
					{
						if (!badgenumber_UserIdDic.ContainsKey(this.m_numbers[i]))
						{
							UserInfo userInfo = new UserInfo();
							this.SetModel(userInfo);
							userInfo.Name = this.m_numbers[i];
							userInfo.BadgeNumber = int.Parse(this.m_numbers[i]).ToString();
							list.Add(userInfo);
						}
						else
						{
							stringBuilder.Append(this.m_numbers[i] + ":" + ShowMsgInfos.GetInfo("personnelNumberRepeat", "人员编号已存在") + "\r\n");
							if (i % 100 == 0 && stringBuilder.Length > 0)
							{
								this.ShowUpLoadInfo(stringBuilder.ToString());
								stringBuilder = new StringBuilder();
							}
						}
					}
				}
				if (stringBuilder.Length > 0)
				{
					this.ShowUpLoadInfo(stringBuilder.ToString());
					stringBuilder = new StringBuilder();
				}
				userInfoBll.Add(list, null);
				this.ShowUpLoadInfo(ShowMsgInfos.GetInfo("SSaveDataSuccess", "保存数据完成"));
				this.ShowProgress(100);
				this.m_thread = null;
				this.OnFinish(true);
			}
			catch (Exception ex)
			{
				this.ShowUpLoadInfo(ex.Message);
				this.m_thread = null;
				this.OnFinish(true);
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			this.DataEvent();
			this.ShowProgress(0);
			if (this.listView1.Items.Count > 0)
			{
				try
				{
					this.time_close.Enabled = false;
					this.btn_ok.Enabled = false;
					this.btn_cancel.Enabled = false;
					this.Cursor = Cursors.WaitCursor;
					this.m_numbers.Clear();
					if (this.m_thread == null)
					{
						for (int i = 0; i < this.listView1.Items.Count; i++)
						{
							this.m_numbers.Add(this.listView1.Items[i].Text);
						}
						this.m_thread = new Thread(this.Start);
						this.m_thread.Start();
					}
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ex.Message);
				}
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("NoSetUserNumber", "没有设置人员编号"));
			}
		}

		private void cancelBtn_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_format_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 11);
			int num = this.txt_format.Text.Length + int.Parse(this.numUpDown_width.Value.ToString());
			int num2;
			if ((num > 11 || this.txt_format.Text.Length >= 11) && this.txt_format.SelectionLength == 0 && !e.Handled && e.KeyChar != '\b')
			{
				num2 = ((e.KeyChar != '\r') ? 1 : 0);
				goto IL_007c;
			}
			num2 = 0;
			goto IL_007c;
			IL_007c:
			if (num2 != 0)
			{
				e.Handled = true;
			}
		}

		private void DataEvent()
		{
			try
			{
				decimal value = this.numUpDown_start.Value;
				int num = int.Parse(value.ToString());
				value = this.numUpDown_end.Value;
				if (num <= int.Parse(value.ToString()))
				{
					if (string.IsNullOrEmpty(this.txt_format.Text))
					{
						this.txt_format.Text = "(*)";
					}
					else if (this.txt_format.Text.IndexOf("(*)") == -1)
					{
						if (!CheckInfo.IsNumber(this.txt_format.Text))
						{
							goto end_IL_0001;
						}
						this.txt_format.Text = this.txt_format.Text + "(*)";
					}
					value = this.numUpDown_width.Value;
					int num2 = int.Parse(value.ToString());
					value = this.numUpDown_start.Value;
					int num3 = int.Parse(value.ToString());
					value = this.numUpDown_end.Value;
					int num4 = int.Parse(value.ToString());
					StringBuilder stringBuilder = new StringBuilder();
					if (this.txt_format.Text.IndexOf("(*)") > 0)
					{
						for (int i = 0; i < num2; i++)
						{
							stringBuilder.Append("0");
						}
					}
					else
					{
						stringBuilder.Append("0");
					}
					this.listView1.Items.Clear();
					this.listView1.BeginUpdate();
					for (int j = num3; j <= num4; j++)
					{
						int num5 = num2;
						string newValue = j.ToString(stringBuilder.ToString());
						newValue = this.txt_format.Text.Replace("(*)", newValue);
						this.listView1.Items.Add(newValue.Trim());
						Application.DoEvents();
					}
					this.listView1.EndUpdate();
				}
				end_IL_0001:;
			}
			catch
			{
			}
		}

		private void txt_format_TextChanged(object sender, EventArgs e)
		{
			this.numUpDown_width.Maximum = 12 - this.txt_format.TextLength - 1;
			if (this.numUpDown_width.Maximum > 8m)
			{
				this.numUpDown_width.Maximum = 8m;
			}
			else if (this.numUpDown_width.Maximum == decimal.Zero)
			{
				this.numUpDown_width.Maximum = decimal.One;
				this.numUpDown_width.Value = decimal.One;
			}
		}

		private void numUpDown_width_ValueChanged(object sender, EventArgs e)
		{
			int length = this.txt_format.Text.Length;
			decimal value = this.numUpDown_width.Value;
			int num = length + int.Parse(value.ToString());
			value = this.numUpDown_width.Value;
			int num2 = int.Parse(value.ToString());
			value = this.numUpDown_width.Value;
			switch (value.ToString())
			{
			case "8":
				this.numUpDown_end.Maximum = 99999999m;
				this.numUpDown_start.Maximum = 99999999m;
				break;
			case "7":
				this.numUpDown_end.Maximum = 9999999m;
				this.numUpDown_start.Maximum = 9999999m;
				break;
			case "6":
				this.numUpDown_end.Maximum = 999999m;
				this.numUpDown_start.Maximum = 999999m;
				break;
			case "5":
				this.numUpDown_end.Maximum = 99999m;
				this.numUpDown_start.Maximum = 99999m;
				break;
			case "4":
				this.numUpDown_end.Maximum = 9999m;
				this.numUpDown_start.Maximum = 9999m;
				break;
			case "3":
				this.numUpDown_end.Maximum = 999m;
				this.numUpDown_start.Maximum = 999m;
				break;
			case "2":
				this.numUpDown_end.Maximum = 99m;
				this.numUpDown_start.Maximum = 99m;
				break;
			case "1":
				this.numUpDown_end.Maximum = 9m;
				this.numUpDown_start.Maximum = 9m;
				break;
			default:
				this.numUpDown_end.Maximum = 1000m;
				this.numUpDown_start.Maximum = 1000m;
				break;
			}
			if (num > 12)
			{
				return;
			}
		}

		private void numUpDown_width_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 0, 8L);
			if (e.KeyChar >= '0' && e.KeyChar < '9' && !e.Handled)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(e.KeyChar);
				int num = this.txt_format.Text.Length + int.Parse(stringBuilder.ToString());
				if (num > 12)
				{
					e.Handled = true;
				}
			}
		}

		private void numUpDown_start_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 0, 99999999L);
			this.numUpDown_width_ValueChanged(null, null);
			if (this.txt_format.TextLength + this.numUpDown_start.Value.ToString().Length >= 11)
			{
				e.Handled = true;
			}
		}

		private void numUpDown_end_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 0, 999999999L);
			this.numUpDown_width_ValueChanged(null, null);
			if (this.txt_format.TextLength + this.numUpDown_end.Value.ToString().Length > 11)
			{
				e.Handled = true;
			}
		}

		private void numUpDown_start_ValueChanged(object sender, EventArgs e)
		{
			this.numUpDown_end_ValueChanged(null, null);
		}

		private void numUpDown_end_ValueChanged(object sender, EventArgs e)
		{
			if (this.numUpDown_end.Value - this.numUpDown_start.Value > 9999m)
			{
				this.numUpDown_end.Value = this.numUpDown_start.Value + 10000m - decimal.One;
			}
		}

		private void numUpDown_width_KeyUp(object sender, KeyEventArgs e)
		{
		}

		private void numUpDown_start_KeyUp(object sender, KeyEventArgs e)
		{
			if (!this.m_lock)
			{
				decimal value = this.numUpDown_end.Value;
				int num;
				if (!string.IsNullOrEmpty(value.ToString()))
				{
					value = this.numUpDown_start.Value;
					num = ((!string.IsNullOrEmpty(value.ToString())) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				if (num != 0)
				{
					this.m_lock = true;
					value = this.numUpDown_start.Value;
					int num2 = int.Parse(value.ToString());
					value = this.numUpDown_end.Value;
					if (num2 <= int.Parse(value.ToString()))
					{
						value = this.numUpDown_end.Value;
						int num3 = int.Parse(value.ToString());
						value = this.numUpDown_start.Value;
						if (num3 - int.Parse(value.ToString()) > 10000)
						{
							NumericUpDown numericUpDown = this.numUpDown_end;
							value = this.numUpDown_start.Value;
							numericUpDown.Value = int.Parse(value.ToString()) + 9999;
						}
					}
					else
					{
						this.listView1.Items.Clear();
					}
					this.m_lock = false;
				}
			}
		}

		private void numUpDown_end_KeyUp(object sender, KeyEventArgs e)
		{
			if (!this.m_lock)
			{
				decimal value = this.numUpDown_end.Value;
				int num;
				if (!string.IsNullOrEmpty(value.ToString()))
				{
					value = this.numUpDown_start.Value;
					num = ((!string.IsNullOrEmpty(value.ToString())) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				if (num != 0)
				{
					this.m_lock = true;
					value = this.numUpDown_start.Value;
					int num2 = int.Parse(value.ToString());
					value = this.numUpDown_end.Value;
					if (num2 <= int.Parse(value.ToString()))
					{
						value = this.numUpDown_end.Value;
						int num3 = int.Parse(value.ToString());
						value = this.numUpDown_start.Value;
						if (num3 - int.Parse(value.ToString()) > 10000)
						{
							NumericUpDown numericUpDown = this.numUpDown_start;
							value = this.numUpDown_end.Value;
							numericUpDown.Value = int.Parse(value.ToString()) - 9999;
						}
					}
					else
					{
						this.listView1.Items.Clear();
					}
					this.m_lock = false;
				}
			}
		}

		private void time_close_Tick(object sender, EventArgs e)
		{
			if (this.m_thread == null)
			{
				this.time_close.Enabled = false;
			}
		}

		private void BatchAddPersonnelForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.m_thread != null)
			{
				e.Cancel = true;
			}
		}

		private void numUpDown_start_Leave(object sender, EventArgs e)
		{
		}

		private void numUpDown_end_Leave(object sender, EventArgs e)
		{
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(BatchAddPersonnelForm));
			this.btn_ok = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.lbl_editNum = new Label();
			this.groupBox1 = new GroupBox();
			this.listView1 = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.numUpDown_width = new NumericUpDown();
			this.numUpDown_end = new NumericUpDown();
			this.numUpDown_start = new NumericUpDown();
			this.lbl_end = new Label();
			this.lbl_start = new Label();
			this.txt_format = new TextBox();
			this.lbl_width = new Label();
			this.lbl_format = new Label();
			this.groupBox2 = new GroupBox();
			this.chk_multiCard = new CheckBox();
			this.chk_officePhone = new CheckBox();
			this.chk_type = new CheckBox();
			this.chk_hire = new CheckBox();
			this.chk_education = new CheckBox();
			this.chk_gender = new CheckBox();
			this.chk_dept = new CheckBox();
			this.lb_progress = new LabelX();
			this.progressBarUp = new ProgressBar();
			this.txt_UpLoadInfo = new TextBox();
			this.time_close = new System.Windows.Forms.Timer(this.components);
			this.lbl_notice = new Label();
			this.groupBox1.SuspendLayout();
			((ISupportInitialize)this.numUpDown_width).BeginInit();
			((ISupportInitialize)this.numUpDown_end).BeginInit();
			((ISupportInitialize)this.numUpDown_start).BeginInit();
			this.groupBox2.SuspendLayout();
			base.SuspendLayout();
			this.btn_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_ok.Location = new Point(344, 475);
			this.btn_ok.Name = "btn_ok";
			this.btn_ok.Size = new Size(82, 23);
			this.btn_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_ok.TabIndex = 12;
			this.btn_ok.Text = "确定";
			this.btn_ok.Click += this.btnOk_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(438, 475);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 13;
			this.btn_cancel.Text = "返回";
			this.btn_cancel.Click += this.cancelBtn_Click;
			this.lbl_editNum.AutoSize = true;
			this.lbl_editNum.Location = new Point(13, 20);
			this.lbl_editNum.Name = "lbl_editNum";
			this.lbl_editNum.Size = new Size(0, 12);
			this.lbl_editNum.TabIndex = 56;
			this.groupBox1.Controls.Add(this.numUpDown_end);
			this.groupBox1.Controls.Add(this.numUpDown_start);
			this.groupBox1.Controls.Add(this.numUpDown_width);
			this.groupBox1.Controls.Add(this.txt_format);
			this.groupBox1.Controls.Add(this.listView1);
			this.groupBox1.Controls.Add(this.lbl_end);
			this.groupBox1.Controls.Add(this.lbl_start);
			this.groupBox1.Controls.Add(this.lbl_width);
			this.groupBox1.Controls.Add(this.lbl_format);
			this.groupBox1.Location = new Point(12, 49);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(256, 276);
			this.groupBox1.TabIndex = 57;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "人员编号";
			this.listView1.AccessibleName = "";
			this.listView1.Activation = ItemActivation.OneClick;
			this.listView1.Columns.AddRange(new ColumnHeader[1]
			{
				this.columnHeader1
			});
			this.listView1.GridLines = true;
			this.listView1.HoverSelection = true;
			this.listView1.Location = new Point(9, 119);
			this.listView1.Name = "listView1";
			this.listView1.Size = new Size(222, 153);
			this.listView1.TabIndex = 12;
			this.listView1.TabStop = false;
			this.listView1.Tag = "";
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = View.List;
			this.columnHeader1.Text = "人员编号";
			this.columnHeader1.Width = 80;
			this.numUpDown_width.Location = new Point(169, 57);
			this.numUpDown_width.Maximum = new decimal(new int[4]
			{
				8,
				0,
				0,
				0
			});
			this.numUpDown_width.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numUpDown_width.Name = "numUpDown_width";
			this.numUpDown_width.Size = new Size(62, 21);
			this.numUpDown_width.TabIndex = 2;
			this.numUpDown_width.Value = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numUpDown_width.ValueChanged += this.numUpDown_width_ValueChanged;
			this.numUpDown_width.KeyPress += this.numUpDown_width_KeyPress;
			this.numUpDown_width.KeyUp += this.numUpDown_width_KeyUp;
			this.numUpDown_end.Location = new Point(169, 90);
			this.numUpDown_end.Maximum = new decimal(new int[4]
			{
				99999999,
				0,
				0,
				0
			});
			this.numUpDown_end.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numUpDown_end.Name = "numUpDown_end";
			this.numUpDown_end.Size = new Size(62, 21);
			this.numUpDown_end.TabIndex = 4;
			this.numUpDown_end.Value = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numUpDown_end.ValueChanged += this.numUpDown_end_ValueChanged;
			this.numUpDown_end.KeyPress += this.numUpDown_end_KeyPress;
			this.numUpDown_end.KeyUp += this.numUpDown_end_KeyUp;
			this.numUpDown_end.Leave += this.numUpDown_end_Leave;
			this.numUpDown_start.Location = new Point(61, 90);
			this.numUpDown_start.Maximum = new decimal(new int[4]
			{
				9999999,
				0,
				0,
				0
			});
			this.numUpDown_start.Minimum = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numUpDown_start.Name = "numUpDown_start";
			this.numUpDown_start.Size = new Size(54, 21);
			this.numUpDown_start.TabIndex = 3;
			this.numUpDown_start.Value = new decimal(new int[4]
			{
				1,
				0,
				0,
				0
			});
			this.numUpDown_start.ValueChanged += this.numUpDown_start_ValueChanged;
			this.numUpDown_start.KeyPress += this.numUpDown_start_KeyPress;
			this.numUpDown_start.KeyUp += this.numUpDown_start_KeyUp;
			this.numUpDown_start.Leave += this.numUpDown_start_Leave;
			this.lbl_end.Location = new Point(127, 91);
			this.lbl_end.Name = "lbl_end";
			this.lbl_end.Size = new Size(44, 18);
			this.lbl_end.TabIndex = 7;
			this.lbl_end.Text = "到";
			this.lbl_end.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_start.Location = new Point(7, 91);
			this.lbl_start.Name = "lbl_start";
			this.lbl_start.Size = new Size(48, 18);
			this.lbl_start.TabIndex = 6;
			this.lbl_start.Text = "从";
			this.lbl_start.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_format.Location = new Point(129, 25);
			this.txt_format.Name = "txt_format";
			this.txt_format.Size = new Size(102, 21);
			this.txt_format.TabIndex = 1;
			this.txt_format.Text = "(*)";
			this.txt_format.TextChanged += this.txt_format_TextChanged;
			this.txt_format.KeyPress += this.txt_format_KeyPress;
			this.lbl_width.Location = new Point(7, 59);
			this.lbl_width.Name = "lbl_width";
			this.lbl_width.Size = new Size(156, 16);
			this.lbl_width.TabIndex = 1;
			this.lbl_width.Text = "通配符“(*)”宽度";
			this.lbl_width.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_format.Location = new Point(7, 27);
			this.lbl_format.Name = "lbl_format";
			this.lbl_format.Size = new Size(118, 16);
			this.lbl_format.TabIndex = 0;
			this.lbl_format.Text = "编号格式";
			this.lbl_format.TextAlign = ContentAlignment.MiddleLeft;
			this.groupBox2.Controls.Add(this.chk_multiCard);
			this.groupBox2.Controls.Add(this.chk_officePhone);
			this.groupBox2.Controls.Add(this.chk_type);
			this.groupBox2.Controls.Add(this.chk_hire);
			this.groupBox2.Controls.Add(this.chk_education);
			this.groupBox2.Controls.Add(this.chk_gender);
			this.groupBox2.Controls.Add(this.chk_dept);
			this.groupBox2.Location = new Point(285, 49);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(235, 276);
			this.groupBox2.TabIndex = 58;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "选择要复制的字段";
			this.chk_multiCard.AutoSize = true;
			this.chk_multiCard.BackColor = Color.Transparent;
			this.chk_multiCard.Location = new Point(10, 253);
			this.chk_multiCard.Name = "chk_multiCard";
			this.chk_multiCard.Size = new Size(108, 16);
			this.chk_multiCard.TabIndex = 11;
			this.chk_multiCard.Text = "多卡开门人员组";
			this.chk_multiCard.UseVisualStyleBackColor = false;
			this.chk_officePhone.AutoSize = true;
			this.chk_officePhone.BackColor = Color.Transparent;
			this.chk_officePhone.Location = new Point(10, 215);
			this.chk_officePhone.Name = "chk_officePhone";
			this.chk_officePhone.Size = new Size(72, 16);
			this.chk_officePhone.TabIndex = 10;
			this.chk_officePhone.Text = "办公电话";
			this.chk_officePhone.UseVisualStyleBackColor = false;
			this.chk_type.AutoSize = true;
			this.chk_type.BackColor = Color.Transparent;
			this.chk_type.Checked = true;
			this.chk_type.CheckState = CheckState.Checked;
			this.chk_type.Location = new Point(10, 177);
			this.chk_type.Name = "chk_type";
			this.chk_type.Size = new Size(72, 16);
			this.chk_type.TabIndex = 9;
			this.chk_type.Text = "员工类型";
			this.chk_type.UseVisualStyleBackColor = false;
			this.chk_hire.AutoSize = true;
			this.chk_hire.BackColor = Color.Transparent;
			this.chk_hire.Checked = true;
			this.chk_hire.CheckState = CheckState.Checked;
			this.chk_hire.Location = new Point(10, 139);
			this.chk_hire.Name = "chk_hire";
			this.chk_hire.Size = new Size(72, 16);
			this.chk_hire.TabIndex = 8;
			this.chk_hire.Text = "雇佣类型";
			this.chk_hire.UseVisualStyleBackColor = false;
			this.chk_education.AutoSize = true;
			this.chk_education.BackColor = Color.Transparent;
			this.chk_education.Location = new Point(10, 101);
			this.chk_education.Name = "chk_education";
			this.chk_education.Size = new Size(48, 16);
			this.chk_education.TabIndex = 7;
			this.chk_education.Text = "学历";
			this.chk_education.UseVisualStyleBackColor = false;
			this.chk_gender.AutoSize = true;
			this.chk_gender.BackColor = Color.Transparent;
			this.chk_gender.Location = new Point(10, 63);
			this.chk_gender.Name = "chk_gender";
			this.chk_gender.Size = new Size(48, 16);
			this.chk_gender.TabIndex = 6;
			this.chk_gender.Text = "性别";
			this.chk_gender.UseVisualStyleBackColor = false;
			this.chk_dept.AutoSize = true;
			this.chk_dept.BackColor = Color.Transparent;
			this.chk_dept.Checked = true;
			this.chk_dept.CheckState = CheckState.Checked;
			this.chk_dept.Location = new Point(10, 25);
			this.chk_dept.Name = "chk_dept";
			this.chk_dept.Size = new Size(48, 16);
			this.chk_dept.TabIndex = 5;
			this.chk_dept.Text = "部门";
			this.chk_dept.UseVisualStyleBackColor = false;
			this.lb_progress.AutoSize = true;
			this.lb_progress.BackgroundStyle.Class = "";
			this.lb_progress.Location = new Point(12, 329);
			this.lb_progress.Name = "lb_progress";
			this.lb_progress.Size = new Size(31, 18);
			this.lb_progress.TabIndex = 60;
			this.lb_progress.Text = "进度";
			this.progressBarUp.Location = new Point(12, 353);
			this.progressBarUp.Name = "progressBarUp";
			this.progressBarUp.Size = new Size(508, 23);
			this.progressBarUp.TabIndex = 59;
			this.txt_UpLoadInfo.Location = new Point(12, 382);
			this.txt_UpLoadInfo.Multiline = true;
			this.txt_UpLoadInfo.Name = "txt_UpLoadInfo";
			this.txt_UpLoadInfo.ScrollBars = ScrollBars.Vertical;
			this.txt_UpLoadInfo.Size = new Size(508, 82);
			this.txt_UpLoadInfo.TabIndex = 61;
			this.txt_UpLoadInfo.TabStop = false;
			this.time_close.Tick += this.time_close_Tick;
			this.lbl_notice.ForeColor = Color.Firebrick;
			this.lbl_notice.Location = new Point(16, 480);
			this.lbl_notice.Name = "lbl_notice";
			this.lbl_notice.Size = new Size(322, 12);
			this.lbl_notice.TabIndex = 62;
			this.lbl_notice.Text = "注:一次只能批量添加10000个人";
			base.AcceptButton = this.btn_ok;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(532, 510);
			base.Controls.Add(this.lbl_notice);
			base.Controls.Add(this.txt_UpLoadInfo);
			base.Controls.Add(this.lb_progress);
			base.Controls.Add(this.progressBarUp);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.lbl_editNum);
			base.Controls.Add(this.btn_ok);
			base.Controls.Add(this.btn_cancel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "BatchAddPersonnelForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "批量添加人员";
			base.FormClosing += this.BatchAddPersonnelForm_FormClosing;
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((ISupportInitialize)this.numUpDown_width).EndInit();
			((ISupportInitialize)this.numUpDown_end).EndInit();
			((ISupportInitialize)this.numUpDown_start).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
