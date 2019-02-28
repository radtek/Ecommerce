/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.Data.Model;

namespace ZK.Access
{
	public class Palms : UserControl
	{
		public delegate Template GetFingerPrintHandler(int FingerId);

		private int _LineWidth;

		private int _SelectedFingerIndex;

		private Color _lineColor;

		private Color _SelectedFingerColor;

		private Color _RegisteredFingerColor;

		private Color _SelectedRegisteredFingerColor;

		private List<FingerControl> lstFinger;

		private Dictionary<int, Template> _RegisteredFinger;

		public GetFingerPrintHandler GetFingerPrintData;

		private IContainer components = null;

		private FingerControl fingerControl10;

		private FingerControl fingerControl9;

		private FingerControl fingerControl8;

		private FingerControl fingerControl7;

		private FingerControl fingerControl6;

		private FingerControl fingerControl5;

		private FingerControl fingerControl4;

		private FingerControl fingerControl3;

		private FingerControl fingerControl2;

		private FingerControl fingerControl1;

		private PalmControl palmControl2;

		private PalmControl palmControl1;

		[Category("Custom")]
		[Description("绘制手指的线条颜色")]
		[DefaultValue(typeof(Color), "220, 220, 220")]
		public Color LineColor
		{
			get
			{
				return this._lineColor;
			}
			set
			{
				this._lineColor = value;
				if (this.lstFinger != null)
				{
					this.palmControl1.LineColor = this.LineColor;
					this.palmControl2.LineColor = this.LineColor;
					foreach (FingerControl item in this.lstFinger)
					{
						item.LineColor = this.LineColor;
					}
					if (base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.Refresh();
						});
					}
					else
					{
						this.Refresh();
					}
				}
			}
		}

		[Category("Custom")]
		[Description("绘制手指的线条大小")]
		[DefaultValue(1)]
		public int LineWidth
		{
			get
			{
				return this._LineWidth;
			}
			set
			{
				this._LineWidth = value;
				if (this.lstFinger != null)
				{
					this.palmControl1.LineWidth = this.LineWidth;
					this.palmControl2.LineWidth = this.LineWidth;
					foreach (FingerControl item in this.lstFinger)
					{
						item.LineWidth = this.LineWidth;
					}
					if (base.InvokeRequired)
					{
						base.Invoke((MethodInvoker)delegate
						{
							this.Refresh();
						});
					}
					else
					{
						this.Refresh();
					}
				}
			}
		}

		[Category("Custom")]
		[Description("已选择手指的填充颜色")]
		[DefaultValue(typeof(Color), "0, 250, 250")]
		public Color SelectedFingerColor
		{
			get
			{
				return this._SelectedFingerColor;
			}
			set
			{
				this._SelectedFingerColor = value;
				this.SetFingerColor(-1);
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.Refresh();
					});
				}
				else
				{
					this.Refresh();
				}
			}
		}

		[Category("Custom")]
		[Description("已选择手指的填充颜色")]
		[DefaultValue(typeof(Color), "0, 250, 250")]
		public Color SelectedRegisteredFingerColor
		{
			get
			{
				return this._SelectedRegisteredFingerColor;
			}
			set
			{
				this._SelectedRegisteredFingerColor = value;
				this.SetFingerColor(-1);
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.Refresh();
					});
				}
				else
				{
					this.Refresh();
				}
			}
		}

		[Category("Custom")]
		[Description("已注册手指的填充颜色")]
		[DefaultValue(typeof(Color), "0, 250, 0")]
		public Color RegisteredFingerColor
		{
			get
			{
				return this._RegisteredFingerColor;
			}
			set
			{
				this._RegisteredFingerColor = value;
				this.SetFingerColor(-1);
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.Refresh();
					});
				}
				else
				{
					this.Refresh();
				}
			}
		}

		public int SelectedFingerIndex
		{
			get
			{
				return this._SelectedFingerIndex;
			}
			set
			{
				this._SelectedFingerIndex = value;
				this.SetFingerColor(-1);
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.Refresh();
					});
				}
				else
				{
					this.Refresh();
				}
				if (this.SelectedFingerIndexChanged != null)
				{
					this.SelectedFingerIndexChanged(this, null);
				}
			}
		}

		public Dictionary<int, Template> RegisteredFinger => this._RegisteredFinger;

		[Category("Custom")]
		[Description("左手掌掌心文字")]
		[DefaultValue("L")]
		public string LeftPalmText
		{
			get
			{
				return this.palmControl1.Text;
			}
			set
			{
				this.palmControl1.Text = (value ?? "");
			}
		}

		[Category("Custom")]
		[Description("右手掌掌心文字")]
		[DefaultValue("R")]
		public string RightPalmText
		{
			get
			{
				return this.palmControl2.Text;
			}
			set
			{
				this.palmControl2.Text = (value ?? "");
			}
		}

		public event EventHandler FingerSelected;

		public event EventHandler SelectedFingerClicked;

		public event EventHandler RegisteredFingerClicked;

		public event EventHandler SelectedFingerIndexChanged;

		public event EventHandler FingerPrintDeleted;

		public event CancelEventHandler FingerPrintDeleting;

		public Palms()
		{
			this.InitializeComponent();
			this.BackColor = Color.Transparent;
			this._LineWidth = 1;
			this._SelectedFingerIndex = -1;
			this._lineColor = Color.FromArgb(220, 220, 220);
			this._SelectedFingerColor = Color.Yellow;
			this._RegisteredFingerColor = Color.Green;
			this._SelectedRegisteredFingerColor = Color.LightGreen;
			this.palmControl1.Text = "L";
			this.palmControl2.Text = "R";
			this._RegisteredFinger = new Dictionary<int, Template>();
			this.InitialFingerControl();
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.Refresh();
				});
			}
			else
			{
				this.Refresh();
			}
		}

		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged(e);
			if (this.lstFinger != null)
			{
				this.palmControl1.BackColor = this.BackColor;
				this.palmControl2.BackColor = this.BackColor;
				foreach (FingerControl item in this.lstFinger)
				{
					int num = (int)item.Tag;
					if (this.IsFingerRegistered(num, false))
					{
						item.BackColor = this._RegisteredFingerColor;
					}
					else if (num == this._SelectedFingerIndex)
					{
						item.BackColor = this._SelectedFingerColor;
					}
					else
					{
						item.BackColor = Color.Transparent;
					}
				}
				if (base.InvokeRequired)
				{
					base.Invoke((MethodInvoker)delegate
					{
						this.Refresh();
					});
				}
				else
				{
					this.Refresh();
				}
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			this.SelectedFingerIndex = -1;
		}

		private void InitialFingerControl()
		{
			this.lstFinger = new List<FingerControl>();
			this.lstFinger.Add(this.fingerControl1);
			this.lstFinger.Add(this.fingerControl2);
			this.lstFinger.Add(this.fingerControl3);
			this.lstFinger.Add(this.fingerControl4);
			this.lstFinger.Add(this.fingerControl5);
			this.lstFinger.Add(this.fingerControl6);
			this.lstFinger.Add(this.fingerControl7);
			this.lstFinger.Add(this.fingerControl8);
			this.lstFinger.Add(this.fingerControl9);
			this.lstFinger.Add(this.fingerControl10);
			for (int i = 0; i < this.lstFinger.Count; i++)
			{
				FingerControl fingerControl = this.lstFinger[i];
				fingerControl.Tag = i;
				fingerControl.FingerColor = (this.IsFingerRegistered(i, false) ? this._RegisteredFingerColor : ((i == this._SelectedFingerIndex) ? this._SelectedFingerColor : Color.Transparent));
				fingerControl.Click += this.FingerControl_Click;
			}
			this.RegisteredFingerClicked += this.PalmsControl_RegisteredFingerClicked;
		}

		private void SetFingerColor(int Fid = -1)
		{
			for (int i = 0; i < this.lstFinger.Count; i++)
			{
				FingerControl fingerControl = this.lstFinger[i];
				int num = default(int);
				if (fingerControl.Tag == null || !int.TryParse(fingerControl.Tag.ToString(), out num))
				{
					num = i;
				}
				if (this.IsFingerRegistered(num, Fid == num))
				{
					if (num == this._SelectedFingerIndex)
					{
						fingerControl.FingerColor = this._SelectedRegisteredFingerColor;
					}
					else
					{
						fingerControl.FingerColor = ((this.RegisteredFinger[num].Flag == 3) ? Color.Red : this._RegisteredFingerColor);
					}
				}
				else if (num == this._SelectedFingerIndex)
				{
					fingerControl.FingerColor = this._SelectedFingerColor;
				}
				else
				{
					fingerControl.FingerColor = Color.Transparent;
				}
			}
		}

		private void FingerControl_Click(object sender, EventArgs e)
		{
			FingerControl fingerControl = sender as FingerControl;
			if (fingerControl != null && fingerControl.Tag != null && int.TryParse(fingerControl.Tag.ToString(), out int num))
			{
				if (num == this._SelectedFingerIndex)
				{
					if (this.RegisteredFinger.ContainsKey(num))
					{
						if (this.RegisteredFingerClicked != null)
						{
							this.RegisteredFingerClicked(sender, e);
						}
					}
					else if (this.SelectedFingerClicked != null)
					{
						this.SelectedFingerClicked(sender, e);
					}
				}
				else
				{
					this._SelectedFingerIndex = num;
					this.SetFingerColor(this._SelectedFingerIndex);
					fingerControl.Refresh();
					if (this.FingerSelected != null)
					{
						this.FingerSelected(sender, e);
					}
				}
			}
		}

		private void PalmsControl_RegisteredFingerClicked(object sender, EventArgs e)
		{
			FingerControl fingerControl = sender as FingerControl;
			if (fingerControl != null && fingerControl.Tag != null && this.RegisteredFinger.ContainsKey((int)fingerControl.Tag))
			{
				if (this.FingerPrintDeleting != null)
				{
					CancelEventArgs cancelEventArgs = new CancelEventArgs();
					cancelEventArgs.Cancel = false;
					this.FingerPrintDeleting(fingerControl, cancelEventArgs);
					if (cancelEventArgs.Cancel)
					{
						return;
					}
				}
				this.RegisteredFinger.Remove((int)fingerControl.Tag);
				this._SelectedFingerIndex = -1;
				this.SetFingerColor(-1);
				if (this.FingerPrintDeleted != null)
				{
					this.FingerPrintDeleted(fingerControl, e);
				}
			}
		}

		private bool IsFingerRegistered(int FingerId, bool CheckDeviceData = false)
		{
			Template template = null;
			if (this.RegisteredFinger.ContainsKey(FingerId) && this.RegisteredFinger[FingerId] != null)
			{
				return true;
			}
			if (CheckDeviceData)
			{
				template = ((this.GetFingerPrintData == null) ? null : this.GetFingerPrintData(FingerId));
			}
			if (template != null)
			{
				this.RegisteredFinger.Add(FingerId, template);
				return true;
			}
			return false;
		}

		public void RefreshFingerColor()
		{
			this.SetFingerColor(-1);
			if (base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate
				{
					this.Refresh();
				});
			}
			else
			{
				this.Refresh();
			}
		}

		private void palmControl2_Click(object sender, EventArgs e)
		{
			this.SelectedFingerIndex = -1;
		}

		private void palmControl1_Click(object sender, EventArgs e)
		{
			this.SelectedFingerIndex = -1;
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
			this.fingerControl10 = new FingerControl();
			this.fingerControl9 = new FingerControl();
			this.fingerControl8 = new FingerControl();
			this.fingerControl7 = new FingerControl();
			this.fingerControl6 = new FingerControl();
			this.fingerControl5 = new FingerControl();
			this.fingerControl4 = new FingerControl();
			this.fingerControl3 = new FingerControl();
			this.fingerControl2 = new FingerControl();
			this.fingerControl1 = new FingerControl();
			this.palmControl2 = new PalmControl();
			this.palmControl1 = new PalmControl();
			base.SuspendLayout();
			this.fingerControl10.Angle = 6;
			this.fingerControl10.BackColor = Color.Transparent;
			this.fingerControl10.FingerColor = Color.Transparent;
			this.fingerControl10.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl10.LineWidth = 2;
			this.fingerControl10.Location = new Point(301, 52);
			this.fingerControl10.Name = "fingerControl10";
			this.fingerControl10.Size = new Size(29, 81);
			this.fingerControl10.TabIndex = 37;
			this.fingerControl9.Angle = 3;
			this.fingerControl9.BackColor = Color.Transparent;
			this.fingerControl9.FingerColor = Color.Transparent;
			this.fingerControl9.FingerHeight = 70;
			this.fingerControl9.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl9.LineWidth = 2;
			this.fingerControl9.Location = new Point(274, 31);
			this.fingerControl9.Name = "fingerControl9";
			this.fingerControl9.ShiftBottom = 1;
			this.fingerControl9.Size = new Size(26, 86);
			this.fingerControl9.TabIndex = 36;
			this.fingerControl8.BackColor = Color.Transparent;
			this.fingerControl8.FingerColor = Color.Transparent;
			this.fingerControl8.FingerHeight = 90;
			this.fingerControl8.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl8.LineWidth = 2;
			this.fingerControl8.Location = new Point(245, 11);
			this.fingerControl8.Name = "fingerControl8";
			this.fingerControl8.Size = new Size(26, 106);
			this.fingerControl8.TabIndex = 35;
			this.fingerControl7.Angle = -3;
			this.fingerControl7.BackColor = Color.Transparent;
			this.fingerControl7.FingerColor = Color.Transparent;
			this.fingerControl7.FingerHeight = 70;
			this.fingerControl7.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl7.LineWidth = 2;
			this.fingerControl7.Location = new Point(214, 37);
			this.fingerControl7.Name = "fingerControl7";
			this.fingerControl7.ShiftBottom = 1;
			this.fingerControl7.ShiftRight = 2;
			this.fingerControl7.Size = new Size(26, 86);
			this.fingerControl7.TabIndex = 34;
			this.fingerControl6.Angle = -20;
			this.fingerControl6.BackColor = Color.Transparent;
			this.fingerControl6.FingerBottomWidth = 28;
			this.fingerControl6.FingerColor = Color.Transparent;
			this.fingerControl6.FingerTopWidth = 23;
			this.fingerControl6.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl6.LineWidth = 2;
			this.fingerControl6.Location = new Point(172, 73);
			this.fingerControl6.Name = "fingerControl6";
			this.fingerControl6.ShiftRight = 20;
			this.fingerControl6.Size = new Size(43, 86);
			this.fingerControl6.TabIndex = 33;
			this.fingerControl5.Angle = 20;
			this.fingerControl5.BackColor = Color.Transparent;
			this.fingerControl5.FingerBottomWidth = 28;
			this.fingerControl5.FingerColor = Color.Transparent;
			this.fingerControl5.FingerTopWidth = 23;
			this.fingerControl5.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl5.LineWidth = 2;
			this.fingerControl5.Location = new Point(122, 73);
			this.fingerControl5.Name = "fingerControl5";
			this.fingerControl5.Size = new Size(52, 86);
			this.fingerControl5.TabIndex = 32;
			this.fingerControl4.Angle = 4;
			this.fingerControl4.BackColor = Color.Transparent;
			this.fingerControl4.FingerColor = Color.Transparent;
			this.fingerControl4.FingerHeight = 70;
			this.fingerControl4.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl4.LineWidth = 2;
			this.fingerControl4.Location = new Point(102, 34);
			this.fingerControl4.Name = "fingerControl4";
			this.fingerControl4.ShiftBottom = 1;
			this.fingerControl4.Size = new Size(26, 86);
			this.fingerControl4.TabIndex = 31;
			this.fingerControl3.BackColor = Color.Transparent;
			this.fingerControl3.FingerColor = Color.Transparent;
			this.fingerControl3.FingerHeight = 90;
			this.fingerControl3.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl3.LineWidth = 2;
			this.fingerControl3.Location = new Point(73, 9);
			this.fingerControl3.Name = "fingerControl3";
			this.fingerControl3.Size = new Size(26, 106);
			this.fingerControl3.TabIndex = 30;
			this.fingerControl2.Angle = -3;
			this.fingerControl2.BackColor = Color.Transparent;
			this.fingerControl2.FingerColor = Color.Transparent;
			this.fingerControl2.FingerHeight = 70;
			this.fingerControl2.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl2.LineWidth = 2;
			this.fingerControl2.Location = new Point(44, 31);
			this.fingerControl2.Name = "fingerControl2";
			this.fingerControl2.ShiftBottom = 1;
			this.fingerControl2.ShiftRight = 2;
			this.fingerControl2.Size = new Size(26, 86);
			this.fingerControl2.TabIndex = 29;
			this.fingerControl1.Angle = -5;
			this.fingerControl1.BackColor = Color.Transparent;
			this.fingerControl1.FingerColor = Color.Transparent;
			this.fingerControl1.LineColor = Color.FromArgb(220, 220, 220);
			this.fingerControl1.LineWidth = 2;
			this.fingerControl1.Location = new Point(16, 48);
			this.fingerControl1.Name = "fingerControl1";
			this.fingerControl1.ShiftRight = 3;
			this.fingerControl1.Size = new Size(27, 81);
			this.fingerControl1.TabIndex = 28;
			this.palmControl2.BackColor = Color.Transparent;
			this.palmControl2.Font = new Font("Arial", 15f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.palmControl2.LineColor = Color.FromArgb(220, 220, 220);
			this.palmControl2.LineWidth = 2;
			this.palmControl2.Location = new Point(216, 110);
			this.palmControl2.Margin = new Padding(5, 5, 5, 5);
			this.palmControl2.Name = "palmControl2";
			this.palmControl2.PalmHeight = 50;
			this.palmControl2.Size = new Size(104, 87);
			this.palmControl2.TabIndex = 27;
			this.palmControl2.Click += this.palmControl2_Click;
			this.palmControl1.BackColor = Color.Transparent;
			this.palmControl1.Font = new Font("Arial", 15f, FontStyle.Regular, GraphicsUnit.Point, 134);
			this.palmControl1.LineColor = Color.FromArgb(220, 220, 220);
			this.palmControl1.LineWidth = 2;
			this.palmControl1.Location = new Point(26, 110);
			this.palmControl1.Margin = new Padding(5, 5, 5, 5);
			this.palmControl1.Name = "palmControl1";
			this.palmControl1.PalmHeight = 50;
			this.palmControl1.Size = new Size(103, 87);
			this.palmControl1.TabIndex = 26;
			this.palmControl1.Click += this.palmControl1_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.Transparent;
			base.Controls.Add(this.fingerControl10);
			base.Controls.Add(this.fingerControl9);
			base.Controls.Add(this.fingerControl8);
			base.Controls.Add(this.fingerControl7);
			base.Controls.Add(this.fingerControl6);
			base.Controls.Add(this.fingerControl5);
			base.Controls.Add(this.fingerControl4);
			base.Controls.Add(this.fingerControl3);
			base.Controls.Add(this.fingerControl2);
			base.Controls.Add(this.fingerControl1);
			base.Controls.Add(this.palmControl2);
			base.Controls.Add(this.palmControl1);
			base.Name = "Palms";
			base.Size = new Size(343, 202);
			base.ResumeLayout(false);
		}
	}
}
