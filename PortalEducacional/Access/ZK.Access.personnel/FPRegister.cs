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
using System.Threading;
using System.Windows.Forms;
using ZK.Access.mensage;
using ZK.ConfigManager;
using ZK.Data.BLL.PullSDK;
using ZK.Data.Model;
using ZK.Data.OleDbDAL;
using ZK.Utils;

namespace ZK.Access.personnel
{
	public class FPRegister : Office2007Form
	{
		private Dictionary<string, List<Template>> dicPin_lstFp = new Dictionary<string, List<Template>>();

		private Thread thread = null;

		private bool pooling = false;

		private bool ValidEvent = false;

		public static int IDUser = 0;

		private int priorFinger = -1;

		private int Pin;

		private int _RemainTimes;

		private readonly string strRemainTimes;

		private DateTime LastFeatureTime = DateTime.Now;

		private DeviceServerBll devServer;

		private IContainer components = null;

		private CheckBox ckb;

		private ButtonX btnCancel;

		private ButtonX btnOK;

		private Label lblTimes;

		private Label lblMsg;

		private FlowLayoutPanel flowLayoutPanel1;

		private Palms palms1;

		private int RemainTimes
		{
			get
			{
				return this._RemainTimes;
			}
			set
			{
				this._RemainTimes = value;
				if (this._RemainTimes < 0)
				{
					this.SetLableText(this.lblMsg, ShowMsgInfos.GetInfo("SelectFingerToRegister", "请选择手指登记指纹"));
					this.SetLableText(this.lblTimes, "");
				}
				else
				{
					this.SetLableText(this.lblMsg, ShowMsgInfos.GetInfo("PressFinger", "请水平按压手指"));
					this.SetLableText(this.lblTimes, $"{this.strRemainTimes}: {this._RemainTimes}");
				}
			}
		}

		private int FpFlag
		{
			get
			{
				int tmp = 1;
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						tmp = ((!this.ckb.Checked) ? 1 : 3);
					});
				}
				else
				{
					tmp = ((!this.ckb.Checked) ? 1 : 3);
				}
				return tmp;
			}
		}

		public int SelectedFingerId
		{
			get
			{
				int result = -1;
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
					});
				}
				return result;
			}
		}

		public Dictionary<int, Template> RegisteredFinger
		{
			get
			{
				if (this.palms1 == null)
				{
					return null;
				}
				return this.palms1.RegisteredFinger;
			}
		}

		public FPRegister(DeviceServerBll devServerBll, int _Pin, Dictionary<int, Template> dic)
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.Pin = _Pin;
			this.strRemainTimes = ShowMsgInfos.GetInfo("RemainTimes", "剩余次数");
			this.devServer = devServerBll;
			this.devServer.FingerFeature += this.devServer_FingerFeature;
			this.devServer.OnEnrollFinger += this.devServer_OnEnrollFinger;
			this.palms1.FingerSelected += this.palms1_FingerSelected;
			this.palms1.SelectedFingerClicked += this.palms1_SelectedFingerClicked;
			Palms palms = this.palms1;
			palms.GetFingerPrintData = (Palms.GetFingerPrintHandler)Delegate.Combine(palms.GetFingerPrintData, new Palms.GetFingerPrintHandler(this.devServer_GetUserFPData));
			this.palms1.SelectedFingerIndexChanged += this.palms1_SelectedFingerIndexChanged;
			this.palms1.FingerPrintDeleting += this.palms1_FingerPrintDeleting;
			this.palms1.SelectedFingerIndex = -1;
			this.palms1.RegisteredFinger.Clear();
			if (dic != null && dic.Count > 0)
			{
				foreach (KeyValuePair<int, Template> item in dic)
				{
					byte[] array = (this.devServer.DevInfo.FPVersion == 10) ? item.Value.TEMPLATE4 : item.Value.TEMPLATE3;
					if (!this.palms1.RegisteredFinger.ContainsKey(item.Key) && array != null)
					{
						this.palms1.RegisteredFinger.Add(item.Key, item.Value);
					}
				}
			}
			this.palms1.RefreshFingerColor();
		}

		private void palms1_SelectedFingerClicked(object sender, EventArgs e)
		{
			this.palms1_FingerSelected(sender, e);
		}

		private void palms1_FingerSelected(object sender, EventArgs e)
		{
			int num = this.CancleOperation();
			if (this.priorFinger != -1)
			{
				this.waitEndOperation(3);
			}
			else
			{
				this.priorFinger = 0;
			}
			FingerControl fingerControl = sender as FingerControl;
			if (fingerControl != null && fingerControl.Tag != null && int.TryParse(fingerControl.Tag.ToString(), out int num2))
			{
				this.ckb.Enabled = false;
				if (!this.palms1.RegisteredFinger.ContainsKey(num2))
				{
					this.devServer.STD_GetUserFpTemplate(this.Pin, num2, out Template template);
					if (template != null)
					{
						if (!this.palms1.RegisteredFinger.ContainsKey(num2))
						{
							this.palms1.RegisteredFinger.Add(num2, template);
						}
						else
						{
							this.palms1.RegisteredFinger[num2] = template;
						}
					}
					else if (num < 0)
					{
						this.palms1.SelectedFingerIndex = -1;
						SysDialogs.ShowInfoMessage(PullSDkErrorInfos.GetInfo(num));
					}
					else
					{
						int flag = 1;
						if (this.ckb.Checked)
						{
							flag = 3;
						}
						num = this.devServer.STD_StartEnroll(this.Pin.ToString(), num2, flag);
						this.startEventPooling(this.Pin, num2);
						if (num < 0)
						{
							this.palms1.SelectedFingerIndex = -1;
							SysDialogs.ShowInfoMessage(PullSDkErrorInfos.GetInfo(num));
						}
						else
						{
							this.RemainTimes = 3;
						}
					}
				}
			}
		}

		private void palms1_SelectedFingerIndexChanged(object sender, EventArgs e)
		{
			this.CancleOperation();
			if (this.palms1.SelectedFingerIndex < 0 || this.palms1.SelectedFingerIndex > 9)
			{
				this.CancleOperation();
			}
			else
			{
				try
				{
					this.ckb.Enabled = false;
				}
				catch (Exception ex)
				{
					string message = ex.Message;
				}
				if (this.palms1.RegisteredFinger.ContainsKey(this.palms1.SelectedFingerIndex))
				{
					this.ckb.Checked = (this.palms1.RegisteredFinger[this.palms1.SelectedFingerIndex].Flag == 3);
				}
			}
		}

		private void palms1_FingerPrintDeleting(object sender, CancelEventArgs e)
		{
			this.CancleOperation();
			FingerControl fingerControl = sender as FingerControl;
			if (fingerControl != null && fingerControl.Tag != null)
			{
				if (SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("DeleteFPConfirm", "确定删除选定的指纹吗?"), MessageBoxButtons.OKCancel, MessageBoxDefaultButton.Button1) != DialogResult.OK)
				{
					e.Cancel = true;
				}
				else
				{
					int fINGERID = (int)fingerControl.Tag;
					List<Template> list = new List<Template>();
					Template template = new Template();
					template.Pin = this.Pin.ToString();
					template.FINGERID = fINGERID;
					list.Add(template);
					int num = this.devServer.STD_DeleteUserFPTemplate(list);
					if (num < 0)
					{
						e.Cancel = true;
						SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SErrDeleteEmTmp", "删除指纹机上的指纹失败"));
					}
				}
			}
		}

		private void devServer_FingerFeature(int Score)
		{
			DateTime now = DateTime.Now;
			TimeSpan timeSpan = now - this.LastFeatureTime;
			if (this._RemainTimes > 0 && Score > 0 && timeSpan.Ticks > 5000000)
			{
				this.LastFeatureTime = now;
				this._RemainTimes--;
				this.SetLableText(this.lblTimes, string.Format("{0}: {1}", ShowMsgInfos.GetInfo("RemainTimes", "剩余次数"), this._RemainTimes));
			}
		}

		private void devServer_OnEnrollFinger(string EnrollNumber, int FingerIndex, int ActionResult, int TemplateLength)
		{
			this.CancleOperation();
			this.stopEventPooling();
			if (ActionResult != 0)
			{
				this.SetLableText(this.lblMsg, ShowMsgInfos.GetInfo("EnrollFailed", "Registro falhou"));
				this.palms1.SelectedFingerIndex = -1;
			}
			else
			{
				this.RegisterOneFinished(int.Parse(EnrollNumber), this.palms1.SelectedFingerIndex, (!this.ckb.Checked) ? 1 : 3);
			}
		}

		private void RegisterOneFinished(int Pin, int newFingerId, int Flag)
		{
			this.CancleOperation();
			this.devServer.STD_StartIdentify();
			Template template = this.devServer_GetUserFPData(newFingerId);
			if (template == null)
			{
				this.SetLableText(this.lblTimes, "");
				this.SetLableText(this.lblMsg, ShowMsgInfos.GetInfo("RegistrationFailed", "登记失败"));
				this.palms1.SelectedFingerIndex = -1;
			}
			else
			{
				template.Flag = (short)Flag;
				if (!this.palms1.RegisteredFinger.ContainsKey(newFingerId))
				{
					this.palms1.RegisteredFinger.Add(newFingerId, template);
				}
				else
				{
					this.palms1.RegisteredFinger[newFingerId] = template;
				}
				this.palms1.RefreshFingerColor();
				this.insertFPDataBase(Pin, newFingerId, Flag, template);
				this.SetLableText(this.lblTimes, "");
				this.SetLableText(this.lblMsg, ShowMsgInfos.GetInfo("RegistrationSucceed", "登记成功"));
				this.palms1.SelectedFingerIndex = -1;
			}
		}

		private void insertFPDataBase(int Pin, int newFingerId, int Flag, Template Fp)
		{
			TemplateDal templateDal = new TemplateDal();
			Fp.Flag = (short)Flag;
			Fp.FINGERID = newFingerId;
			Fp.USERID = FPRegister.IDUser;
			this.priorFinger = -1;
			templateDal.Add(Fp);
		}

		private void SetLableText(Label lb, string Text)
		{
			if (!base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.SetLableText(lb, Text);
					});
				}
				else
				{
					lb.Text = Text;
				}
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.stopEventPooling();
			this.CancleOperation();
			base.DialogResult = DialogResult.Cancel;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.stopEventPooling();
			this.CancleOperation();
			base.DialogResult = DialogResult.OK;
		}

		public Template devServer_GetUserFPData(int FingerId)
		{
			this.devServer.STD_GetUserFpTemplate(this.Pin, FingerId, out Template result);
			return result;
		}

		private void FPRegister_Click(object sender, EventArgs e)
		{
			this.palms1.SelectedFingerIndex = -1;
		}

		private void flowLayoutPanel1_Click(object sender, EventArgs e)
		{
			this.palms1.SelectedFingerIndex = -1;
		}

		protected override void OnClosed(EventArgs e)
		{
			this.stopEventPooling();
			base.OnClosed(e);
			if (this.devServer != null)
			{
				this.devServer.FingerFeature -= this.devServer_FingerFeature;
				this.devServer.OnEnrollFinger -= this.devServer_OnEnrollFinger;
			}
		}

		private int CancleOperation()
		{
			this.RemainTimes = 0;
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.ckb.Enabled = true;
				});
			}
			else
			{
				this.ckb.Enabled = true;
			}
			int num = this.devServer.STD_CancelOperation();
			return this.devServer.STD_StartIdentify();
		}

		private int StartEnroll(string Pin, int FingerIndex, int FPFlag)
		{
			int num = this.CancleOperation();
			if (num < 0)
			{
				return num;
			}
			this.RemainTimes = 3;
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.ckb.Enabled = false;
				});
			}
			else
			{
				this.ckb.Enabled = false;
			}
			this.ValidEvent = true;
			num = this.devServer.STD_StartEnroll(Pin, FingerIndex, (!this.ckb.Checked) ? 1 : 3);
			this.startEventPooling(int.Parse(Pin), FingerIndex);
			return num;
		}

		private void startEventPooling(int Pin, int FingerIndex)
		{
			int num = 0;
			while (this.pooling)
			{
				this.devServer.GetRTLogs(ref num);
				Thread.Sleep(500);
			}
		}

		private void stopEventPooling()
		{
			this.pooling = false;
		}

		private void waitEndOperation(int wait)
		{
			string msg = "Aguarde até o dispositivo cancelar a operação anterior.";
			Mensage mensage = new Mensage(680, 160, "Mensagem", msg, false, true, 3);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FPRegister));
			this.ckb = new CheckBox();
			this.btnCancel = new ButtonX();
			this.btnOK = new ButtonX();
			this.lblTimes = new Label();
			this.lblMsg = new Label();
			this.flowLayoutPanel1 = new FlowLayoutPanel();
			this.palms1 = new Palms();
			this.flowLayoutPanel1.SuspendLayout();
			base.SuspendLayout();
			this.ckb.AutoSize = true;
			this.ckb.Location = new Point(384, 187);
			this.ckb.Name = "ckb";
			this.ckb.Size = new Size(74, 17);
			this.ckb.TabIndex = 3;
			this.ckb.Text = "胁迫指纹";
			this.ckb.UseVisualStyleBackColor = true;
			this.btnCancel.AccessibleRole = AccessibleRole.PushButton;
			this.btnCancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnCancel.Location = new Point(384, 281);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new Size(82, 25);
			this.btnCancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "取消";
			this.btnCancel.Click += this.btnCancel_Click;
			this.btnOK.AccessibleRole = AccessibleRole.PushButton;
			this.btnOK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btnOK.Location = new Point(384, 239);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new Size(82, 25);
			this.btnOK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "确定";
			this.btnOK.Click += this.btnOK_Click;
			this.lblTimes.AutoSize = true;
			this.lblTimes.Location = new Point(483, 46);
			this.lblTimes.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
			this.lblTimes.Name = "lblTimes";
			this.lblTimes.Size = new Size(108, 13);
			this.lblTimes.TabIndex = 1;
			this.lblTimes.Text = "Tentativas Restantes";
			this.lblMsg.AutoSize = true;
			this.lblMsg.Location = new Point(501, 22);
			this.lblMsg.Name = "lblMsg";
			this.lblMsg.Size = new Size(90, 13);
			this.lblMsg.TabIndex = 0;
			this.lblMsg.Text = "Selecione o dedo";
			this.lblMsg.TextAlign = ContentAlignment.MiddleRight;
			this.flowLayoutPanel1.Controls.Add(this.lblMsg);
			this.flowLayoutPanel1.Controls.Add(this.lblTimes);
			this.flowLayoutPanel1.Dock = DockStyle.Top;
			this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
			this.flowLayoutPanel1.RightToLeft = RightToLeft.Yes;
			this.flowLayoutPanel1.Size = new Size(594, 87);
			this.flowLayoutPanel1.TabIndex = 1;
			this.flowLayoutPanel1.Click += this.flowLayoutPanel1_Click;
			this.palms1.AutoValidate = AutoValidate.EnableAllowFocusChange;
			this.palms1.BackColor = Color.SteelBlue;
			this.palms1.LineColor = Color.White;
			this.palms1.LineWidth = 2;
			this.palms1.Location = new Point(12, 145);
			this.palms1.Name = "palms1";
			this.palms1.RegisteredFingerColor = Color.Green;
			this.palms1.SelectedFingerColor = Color.Yellow;
			this.palms1.SelectedFingerIndex = 0;
			this.palms1.SelectedRegisteredFingerColor = Color.Chartreuse;
			this.palms1.Size = new Size(343, 219);
			this.palms1.TabIndex = 7;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(594, 377);
			base.Controls.Add(this.palms1);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.ckb);
			base.Controls.Add(this.flowLayoutPanel1);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FPRegister";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "指纹登记";
			base.Click += this.FPRegister_Click;
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
