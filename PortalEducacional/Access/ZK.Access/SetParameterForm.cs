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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Utils;

namespace ZK.Access
{
	public class SetParameterForm : Office2007Form
	{
		public string sysLang;

		private string FOriDownTime;

		private int FOriLangIndex;

		private string FOriReMonitorTime = string.Empty;

		private Font CurrentFont;

		private Dictionary<string, Dictionary<string, string>> m_Lang = new Dictionary<string, Dictionary<string, string>>();

		private IContainer components = null;

		private ButtonX btn_exit;

		private ButtonX btn_save;

		private TextBox txt_downTime1;

		private Label lbl_timeFormat;

		private ComboBox cbo_sysLang;

		private Label lbl_SysLang;

		private Label lbl_downRecordsTime;

		private FolderBrowserDialog folderBrowserDialog1;

		private Label lbl_reMonitorTime;

		private TextBox txt_reMonitorTime;

		private Label lbl_reMonitorTimeFormat;

		private Label label1_fiekie;

		private MaskedTextBox txt_downTime;

		private Label lblSysFont;

		private LinkLabel lnkSetFont;

		private Label label1;

		private NumericUpDown numPhotoDelayTime;

		private Label label2;

		public event EventHandler refreshDataEvent = null;

		public SetParameterForm()
		{
			this.InitializeComponent();
			this.ParameterForm_Load();
			initLang.LocaleForm(this, base.Name);
		}

		private void SetLangInfo()
		{
			try
			{
				this.m_Lang.Clear();
				this.m_Lang = initLang.GetComboxInfo("SysLang");
				if (this.m_Lang == null || this.m_Lang.Count == 0)
				{
					this.m_Lang = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("0", "简体中文");
					dictionary.Add("1", "English");
					this.m_Lang.Add("0", dictionary);
					initLang.SetComboxInfo("SysLang", this.m_Lang);
					initLang.Save();
				}
				this.cbo_sysLang.Items.Clear();
				if (this.m_Lang != null && this.m_Lang.ContainsKey("0"))
				{
					Dictionary<string, string> dictionary2 = this.m_Lang["0"];
					foreach (KeyValuePair<string, string> item in dictionary2)
					{
						this.cbo_sysLang.Items.Add(item.Value);
					}
				}
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

		private void ParameterForm_Load()
		{
			try
			{
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("TimeDownload");
				this.txt_downTime1.Text = nodeValueByName;
				this.txt_downTime.Text = nodeValueByName;
				this.FOriDownTime = nodeValueByName;
				this.FOriReMonitorTime = AppSite.Instance.GetNodeValueByName("ReMonitorTime");
				this.txt_reMonitorTime.Text = this.FOriReMonitorTime;
				this.sysLang = AppSite.Instance.GetNodeValueByName("Language");
				if (this.sysLang == "chs")
				{
					this.cbo_sysLang.SelectedIndex = 0;
				}
				else
				{
					this.cbo_sysLang.SelectedIndex = 1;
				}
				this.FOriLangIndex = this.cbo_sysLang.SelectedIndex;
				this.CurrentFont = SysInfos.DefaultFont;
				if (!int.TryParse(AppSite.Instance.GetNodeValueByName("PhotoDelayTimeOnMonitoring"), out int value))
				{
					this.numPhotoDelayTime.Value = 15m;
				}
				else
				{
					this.numPhotoDelayTime.Value = value;
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool DownTimeCheck()
		{
			try
			{
				ArrayList arrayList = new ArrayList();
				string text = this.txt_downTime.Text;
				int num = 0;
				int num2;
				for (num2 = text.IndexOf(";"); num2 > 0; num2 = text.IndexOf(";"))
				{
					arrayList.Add(text.Substring(0, num2));
					text = text.Remove(0, num2 + 1);
				}
				if (num2 <= 0)
				{
					arrayList.Add(text);
				}
				for (int i = 0; i < arrayList.Count; i++)
				{
					try
					{
						string text2 = (string)arrayList[i];
						num = text2.IndexOf(":");
						string s = text2.Substring(0, num);
						text2 = text2.Remove(0, num + 1);
						string s2 = text2;
						if (int.Parse(s) > 23 || int.Parse(s2) > 59)
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("STimeError", "时间格式不对"));
							this.txt_downTime1.Focus();
							return false;
						}
					}
					catch
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("STimeError", "时间格式不对"));
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private string DownTimeCheckEx()
		{
			string text = this.txt_downTime.Text;
			string[] array = text.Split(';');
			string text2 = "";
			string[] array2 = array;
			foreach (string text3 in array2)
			{
				int num2;
				if (text3.Trim() != ":")
				{
					int num = text3.IndexOf(":");
					string text4 = text3.Substring(0, num).Trim();
					string text5 = text3.Remove(0, num + 1).Trim();
					string text6 = text5;
					if (!string.IsNullOrEmpty(text4 + text6) && (string.IsNullOrEmpty(text4) || string.IsNullOrEmpty(text6)))
					{
						goto IL_00d7;
					}
					if (!string.IsNullOrEmpty(text4) && int.Parse(text4) > 23)
					{
						goto IL_00d7;
					}
					num2 = ((!string.IsNullOrEmpty(text6) && int.Parse(text6) > 59) ? 1 : 0);
					goto IL_00d8;
				}
				continue;
				IL_00d8:
				if (num2 != 0)
				{
					this.txt_downTime.Focus();
					throw new Exception(ShowMsgInfos.GetInfo("STimeError", "时间格式不对"));
				}
				text2 += text3;
				continue;
				IL_00d7:
				num2 = 1;
				goto IL_00d8;
			}
			return text2;
		}

		private void btn_save_Click(object sender, EventArgs e)
		{
			try
			{
				bool flag = false;
				if (this.FOriDownTime != this.txt_downTime.Text)
				{
					string nodeValue = this.DownTimeCheckEx();
					AppSite.Instance.SetNodeValue("TimeDownload", nodeValue);
				}
				if (this.FOriReMonitorTime != this.txt_reMonitorTime.Text)
				{
					try
					{
						int num = int.Parse(this.txt_reMonitorTime.Text);
						AppSite.Instance.SetNodeValue("ReMonitorTime", num.ToString());
						flag = true;
					}
					catch
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("STimeError", "时间格式不对"));
						this.txt_reMonitorTime.Focus();
						return;
					}
				}
				if (this.CurrentFont != SysInfos.DefaultFont)
				{
					try
					{
						AppSite.Instance.SetFont(this.CurrentFont);
						flag = true;
					}
					catch (Exception ex)
					{
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SaveFontError", "保存字体参数时出错") + ": " + ex.Message);
						this.lnkSetFont.Focus();
						return;
					}
				}
				AppSite.Instance.SetNodeValue("PhotoDelayTimeOnMonitoring", this.numPhotoDelayTime.Value.ToString());
				if (flag && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SOperationSuccessRestart", "操作成功，是否重启软件以使之生效?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					Program.IsRestart = true;
				}
				base.Close();
			}
			catch (Exception ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
			}
		}

		private void txt_downTime1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar < '.' || e.KeyChar > ';') && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		public static bool ISChinese(string strWord)
		{
			string pattern = "[\\u4e00-\\u9fa5]";
			return Regex.IsMatch(strWord, pattern);
		}

		private void txt_reMonitorTime_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 3);
		}

		private void lnkSetFont_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			FontDialog fontDialog = new FontDialog();
			fontDialog.Font = this.CurrentFont;
			if (fontDialog.ShowDialog() == DialogResult.OK)
			{
				this.CurrentFont = fontDialog.Font;
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SetParameterForm));
			this.btn_exit = new ButtonX();
			this.btn_save = new ButtonX();
			this.txt_downTime1 = new TextBox();
			this.lbl_timeFormat = new Label();
			this.cbo_sysLang = new ComboBox();
			this.lbl_SysLang = new Label();
			this.lbl_downRecordsTime = new Label();
			this.folderBrowserDialog1 = new FolderBrowserDialog();
			this.lbl_reMonitorTime = new Label();
			this.txt_reMonitorTime = new TextBox();
			this.lbl_reMonitorTimeFormat = new Label();
			this.label1_fiekie = new Label();
			this.txt_downTime = new MaskedTextBox();
			this.lblSysFont = new Label();
			this.lnkSetFont = new LinkLabel();
			this.label1 = new Label();
			this.numPhotoDelayTime = new NumericUpDown();
			this.label2 = new Label();
			((ISupportInitialize)this.numPhotoDelayTime).BeginInit();
			base.SuspendLayout();
			this.btn_exit.AccessibleRole = AccessibleRole.PushButton;
			this.btn_exit.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_exit.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_exit.Location = new Point(407, 219);
			this.btn_exit.Name = "btn_exit";
			this.btn_exit.Size = new Size(82, 25);
			this.btn_exit.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_exit.TabIndex = 3;
			this.btn_exit.Text = "取消";
			this.btn_exit.Click += this.btn_exit_Click;
			this.btn_save.AccessibleRole = AccessibleRole.PushButton;
			this.btn_save.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
			this.btn_save.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_save.Location = new Point(308, 219);
			this.btn_save.Name = "btn_save";
			this.btn_save.Size = new Size(82, 25);
			this.btn_save.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_save.TabIndex = 2;
			this.btn_save.Text = "确定";
			this.btn_save.Click += this.btn_save_Click;
			this.txt_downTime1.Location = new Point(203, 42);
			this.txt_downTime1.Name = "txt_downTime1";
			this.txt_downTime1.Size = new Size(318, 20);
			this.txt_downTime1.TabIndex = 0;
			this.txt_downTime1.Visible = false;
			this.txt_downTime1.KeyPress += this.txt_downTime1_KeyPress;
			this.lbl_timeFormat.Location = new Point(256, 46);
			this.lbl_timeFormat.Name = "lbl_timeFormat";
			this.lbl_timeFormat.Size = new Size(261, 16);
			this.lbl_timeFormat.TabIndex = 2;
			this.lbl_timeFormat.Text = "时间格式:21:31 (24小时制)";
			this.cbo_sysLang.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cbo_sysLang.FormattingEnabled = true;
			this.cbo_sysLang.Items.AddRange(new object[2]
			{
				"简体中文",
				"英语"
			});
			this.cbo_sysLang.Location = new Point(203, 4);
			this.cbo_sysLang.Name = "cbo_sysLang";
			this.cbo_sysLang.Size = new Size(293, 21);
			this.cbo_sysLang.TabIndex = 1;
			this.cbo_sysLang.Visible = false;
			this.lbl_SysLang.Location = new Point(8, 10);
			this.lbl_SysLang.Name = "lbl_SysLang";
			this.lbl_SysLang.Size = new Size(178, 18);
			this.lbl_SysLang.TabIndex = 10;
			this.lbl_SysLang.Text = "系统语言";
			this.lbl_SysLang.Visible = false;
			this.lbl_downRecordsTime.Location = new Point(8, 46);
			this.lbl_downRecordsTime.Name = "lbl_downRecordsTime";
			this.lbl_downRecordsTime.Size = new Size(189, 20);
			this.lbl_downRecordsTime.TabIndex = 9;
			this.lbl_downRecordsTime.Text = "定时获取记录时间";
			this.lbl_reMonitorTime.Location = new Point(8, 88);
			this.lbl_reMonitorTime.Name = "lbl_reMonitorTime";
			this.lbl_reMonitorTime.Size = new Size(178, 20);
			this.lbl_reMonitorTime.TabIndex = 12;
			this.lbl_reMonitorTime.Text = "监控失败再次重连时间";
			this.txt_reMonitorTime.Location = new Point(203, 81);
			this.txt_reMonitorTime.Name = "txt_reMonitorTime";
			this.txt_reMonitorTime.Size = new Size(201, 20);
			this.txt_reMonitorTime.TabIndex = 11;
			this.txt_reMonitorTime.KeyPress += this.txt_reMonitorTime_KeyPress;
			this.lbl_reMonitorTimeFormat.Location = new Point(409, 85);
			this.lbl_reMonitorTimeFormat.Name = "lbl_reMonitorTimeFormat";
			this.lbl_reMonitorTimeFormat.Size = new Size(114, 16);
			this.lbl_reMonitorTimeFormat.TabIndex = 13;
			this.lbl_reMonitorTimeFormat.Text = "分钟";
			this.label1_fiekie.AllowDrop = true;
			this.label1_fiekie.AutoEllipsis = true;
			this.label1_fiekie.Location = new Point(12, 145);
			this.label1_fiekie.Name = "label1_fiekie";
			this.label1_fiekie.Size = new Size(167, 13);
			this.label1_fiekie.TabIndex = 14;
			this.txt_downTime.Location = new Point(203, 42);
			this.txt_downTime.Mask = "90:00";
			this.txt_downTime.Name = "txt_downTime";
			this.txt_downTime.Size = new Size(44, 20);
			this.txt_downTime.TabIndex = 15;
			this.lblSysFont.Location = new Point(8, 168);
			this.lblSysFont.Name = "lblSysFont";
			this.lblSysFont.Size = new Size(178, 20);
			this.lblSysFont.TabIndex = 16;
			this.lblSysFont.Text = "系统字体";
			this.lnkSetFont.AutoSize = true;
			this.lnkSetFont.Location = new Point(201, 166);
			this.lnkSetFont.Name = "lnkSetFont";
			this.lnkSetFont.Size = new Size(31, 13);
			this.lnkSetFont.TabIndex = 17;
			this.lnkSetFont.TabStop = true;
			this.lnkSetFont.Text = "设置";
			this.lnkSetFont.LinkClicked += this.lnkSetFont_LinkClicked;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(8, 127);
			this.label1.Name = "label1";
			this.label1.Size = new Size(127, 13);
			this.label1.TabIndex = 18;
			this.label1.Text = "监控中的照片显示时间";
			this.numPhotoDelayTime.Location = new Point(203, 122);
			this.numPhotoDelayTime.Name = "numPhotoDelayTime";
			this.numPhotoDelayTime.Size = new Size(80, 20);
			this.numPhotoDelayTime.TabIndex = 19;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(290, 127);
			this.label2.Name = "label2";
			this.label2.Size = new Size(19, 13);
			this.label2.TabIndex = 20;
			this.label2.Text = "秒";
			base.AcceptButton = this.btn_save;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(536, 257);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.numPhotoDelayTime);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.txt_reMonitorTime);
			base.Controls.Add(this.txt_downTime);
			base.Controls.Add(this.cbo_sysLang);
			base.Controls.Add(this.lnkSetFont);
			base.Controls.Add(this.lblSysFont);
			base.Controls.Add(this.label1_fiekie);
			base.Controls.Add(this.lbl_reMonitorTimeFormat);
			base.Controls.Add(this.lbl_reMonitorTime);
			base.Controls.Add(this.lbl_SysLang);
			base.Controls.Add(this.lbl_timeFormat);
			base.Controls.Add(this.lbl_downRecordsTime);
			base.Controls.Add(this.btn_exit);
			base.Controls.Add(this.btn_save);
			base.Controls.Add(this.txt_downTime1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SetParameterForm";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "系统参数设置";
			((ISupportInitialize)this.numPhotoDelayTime).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
