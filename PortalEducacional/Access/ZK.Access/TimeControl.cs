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
using ZK.Access.Properties;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class TimeControl : UserControl
	{
		public delegate void KeyUpHandler(object sender, KeyEventArgs e);

		private int DegreeTimeUnit = 30;

		private int XDown = 0;

		private bool isdown = false;

		private int startx = 0;

		private int starty = 0;

		private int lastx = 0;

		private int moveType = 0;

		private int m_selectindex = -1;

		private bool is_paint = false;

		private int moveTime = -1;

		private int moveX = 0;

		private int moveY = 0;

		private int m_keyMove = 1;

		private string[] Fonts = new string[5]
		{
			"Arial",
			"宋体",
			"Impact",
			"Courier New",
			"Tahoma"
		};

		private Color[] Colors = new Color[7]
		{
			Color.White,
			Color.Black,
			Color.FromArgb(127, 143, 154),
			Color.LightGreen,
			Color.Red,
			Color.LightSkyBlue,
			Color.Green
		};

		private SolidBrush _brush = null;

		private SolidBrush _forebrush = null;

		private SolidBrush _fontbrush = null;

		private SolidBrush _selectbrush = null;

		private Pen _borderPen = null;

		private int m_widthex = 20;

		private int m_heightex = 10;

		private bool m_isCreateSize = true;

		private int _fontSize = 9;

		private Font _font = null;

		private DateTime lastPaintTime = DateTime.Now;

		private List<TimeInfo> m_times = new List<TimeInfo>();

		private Rectangle currRect = default(Rectangle);

		private IContainer components = null;

		private ContextMenuStrip contextMenuStrip1;

		private ToolStripMenuItem menu_modifyTime;

		private ToolStripMenuItem menu_delTime;

		public SolidBrush BackBrush
		{
			get
			{
				if (this._brush == null)
				{
					this._brush = new SolidBrush(Color.WhiteSmoke);
				}
				return this._brush;
			}
			set
			{
				this._brush = value;
			}
		}

		public SolidBrush ForeBrush
		{
			get
			{
				if (this._forebrush == null)
				{
					this._forebrush = new SolidBrush(this.Colors[3]);
				}
				return this._forebrush;
			}
			set
			{
				this._forebrush = value;
			}
		}

		public SolidBrush FontBrush
		{
			get
			{
				if (this._fontbrush == null)
				{
					this._fontbrush = new SolidBrush(this.Colors[6]);
				}
				return this._fontbrush;
			}
			set
			{
				this._fontbrush = value;
			}
		}

		public SolidBrush SelecrBrush
		{
			get
			{
				if (this._selectbrush == null)
				{
					this._selectbrush = new SolidBrush(this.Colors[5]);
				}
				return this._selectbrush;
			}
			set
			{
				this._selectbrush = value;
			}
		}

		public Pen BorderPen
		{
			get
			{
				if (this._borderPen == null)
				{
					this._borderPen = new Pen(this.Colors[1]);
				}
				return this._borderPen;
			}
			set
			{
				this._borderPen = value;
			}
		}

		public int WidthEx
		{
			get
			{
				return this.m_widthex;
			}
			set
			{
				if (value > 1)
				{
					this.m_widthex = value;
				}
			}
		}

		public int KeyMove
		{
			get
			{
				return this.m_keyMove;
			}
			set
			{
				if (value > 0)
				{
					this.m_keyMove = value;
				}
			}
		}

		public int HeightEx
		{
			get
			{
				return this.m_heightex;
			}
			set
			{
				if (value > 1)
				{
					this.m_heightex = value;
				}
			}
		}

		public bool IsCreateSize
		{
			get
			{
				return this.m_isCreateSize;
			}
			set
			{
				this.m_isCreateSize = value;
				if (this.m_isCreateSize)
				{
					if (this.m_heightex < this._fontSize + 1)
					{
						this.m_heightex = this._fontSize + 1;
					}
					if (this.m_widthex < 2 * this._fontSize + 2)
					{
						this.m_widthex = 2 * this._fontSize + 2;
					}
				}
			}
		}

		public int FontSize
		{
			get
			{
				return this._fontSize;
			}
			set
			{
				this._fontSize = value;
			}
		}

		public Font IFont
		{
			get
			{
				if (this._font == null)
				{
					this._font = new Font(this.Fonts[1], (float)this._fontSize, FontStyle.Regular);
				}
				return this._font;
			}
			set
			{
				this._font = value;
			}
		}

		public List<TimeInfo> Times
		{
			get
			{
				List<TimeInfo> list = new List<TimeInfo>();
				for (int i = 0; i < this.m_times.Count; i++)
				{
					list.Add(this.m_times[i]);
				}
				for (int j = 0; j < list.Count; j++)
				{
					int num = j;
					for (int k = j + 1; k < list.Count; k++)
					{
						if (list[k].StartTime < list[num].StartTime)
						{
							num = k;
						}
					}
					if (num != j)
					{
						TimeInfo value = list[j];
						list[j] = list[num];
						list[num] = value;
					}
				}
				return list;
			}
		}

		public int SelectIndex
		{
			get
			{
				return this.m_selectindex;
			}
			set
			{
				if (value < this.m_times.Count && value != this.m_selectindex)
				{
					this.m_selectindex = value;
					this.Refresh();
					if (this.m_selectindex >= 0)
					{
						this.SetMenuEnabled(true);
					}
					else
					{
						this.SetMenuEnabled(false);
					}
				}
			}
		}

		public TimeInfo SelectTime
		{
			get
			{
				if (this.m_selectindex >= 0 && this.m_selectindex < this.m_times.Count)
				{
					return this.m_times[this.m_selectindex];
				}
				TimeInfo timeInfo = new TimeInfo(this.m_widthex, this.m_heightex * 2, base.Width - 2 * this.m_widthex, base.Height - 4 * this.m_heightex, this);
				int time = timeInfo.GetTime(this.currRect.X);
				int time2 = timeInfo.GetTime(this.currRect.Right);
				if (Math.Abs(time2 - time) >= 30)
				{
					timeInfo.SetTime(time, time2);
					return timeInfo;
				}
				return null;
			}
		}

		public event EventHandler SelectIndexChangedEvent;

		public event EventHandler TimeValueChangedEvent;

		public event KeyUpHandler KeyUpEvent;

		public TimeControl()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			base.Height = 50;
			base.Width = 490;
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
		}

		private void PaintBorder(PaintEventArgs e, int twidth)
		{
			if (this.m_isCreateSize)
			{
				e.Graphics.DrawLine(this.BorderPen, this.m_widthex, this.m_heightex * 3 / 2, this.m_widthex, base.Height - this.m_heightex * 3 / 2);
				e.Graphics.DrawLine(this.BorderPen, base.Width - this.m_widthex, this.m_heightex * 3 / 2, base.Width - this.m_widthex, base.Height - this.m_heightex * 3 / 2);
				e.Graphics.DrawLine(this.BorderPen, this.m_widthex, 2 * this.m_heightex, base.Width - this.m_widthex, 2 * this.m_heightex);
				e.Graphics.DrawLine(this.BorderPen, this.m_widthex, base.Height - 2 * this.m_heightex, base.Width - this.m_widthex, base.Height - 2 * this.m_heightex);
			}
			else
			{
				e.Graphics.DrawLine(this.BorderPen, this.m_widthex, this.m_heightex, this.m_widthex, base.Height - this.m_heightex);
				e.Graphics.DrawLine(this.BorderPen, base.Width - this.m_widthex, this.m_heightex, base.Width - this.m_widthex, base.Height - this.m_heightex);
				e.Graphics.DrawLine(this.BorderPen, this.m_widthex, 2 * this.m_heightex, base.Width - this.m_widthex, 2 * this.m_heightex);
				e.Graphics.DrawLine(this.BorderPen, this.m_widthex, base.Height - 2 * this.m_heightex, base.Width - this.m_widthex, base.Height - 2 * this.m_heightex);
			}
			if (this.m_isCreateSize)
			{
				e.Graphics.DrawString("0", this.IFont, this.FontBrush, (float)(this.m_widthex - this._fontSize / 2), (float)(base.Height - this.m_heightex * 3 / 2));
				e.Graphics.DrawString("24", this.IFont, this.FontBrush, (float)(base.Width - this.m_widthex - this._fontSize), (float)(base.Height - this.m_heightex * 3 / 2));
			}
			for (int i = 1; i < 48; i++)
			{
				if (i % 6 == 0)
				{
					if (this.m_isCreateSize)
					{
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, this.m_heightex * 3 / 2, this.m_widthex + i * twidth, 2 * this.m_heightex);
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, base.Height - 2 * this.m_heightex, this.m_widthex + i * twidth, base.Height - this.m_heightex * 3 / 2);
						int num = i / 2;
						if (num > 10)
						{
							e.Graphics.DrawString(num.ToString() + ":00", this.IFont, this.FontBrush, (float)(i * twidth + this.m_widthex - this._fontSize * 2), (float)(base.Height - this.m_heightex * 3 / 2));
						}
						else
						{
							e.Graphics.DrawString(num.ToString() + ":00", this.IFont, this.FontBrush, (float)(i * twidth + this.m_widthex - this._fontSize), (float)(base.Height - this.m_heightex * 3 / 2));
						}
					}
					else
					{
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, this.m_heightex, this.m_widthex + i * twidth, 2 * this.m_heightex);
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, base.Height - 2 * this.m_heightex, this.m_widthex + i * twidth, base.Height - this.m_heightex);
					}
				}
				else if (i % 2 == 0)
				{
					if (this.m_isCreateSize)
					{
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, 7 * this.m_heightex / 4, this.m_widthex + i * twidth, 2 * this.m_heightex);
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, base.Height - 2 * this.m_heightex, this.m_widthex + i * twidth, base.Height - 7 * this.m_heightex / 4);
					}
					else
					{
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, 3 * this.m_heightex / 2, this.m_widthex + i * twidth, 2 * this.m_heightex);
						e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, base.Height - 2 * this.m_heightex, this.m_widthex + i * twidth, base.Height - 3 * this.m_heightex / 2);
					}
				}
				else if (this.m_isCreateSize)
				{
					e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, 15 * this.m_heightex / 8, this.m_widthex + i * twidth, 2 * this.m_heightex);
					e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, base.Height - 2 * this.m_heightex, this.m_widthex + i * twidth, base.Height - 15 * this.m_heightex / 8);
				}
				else
				{
					e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, 7 * this.m_heightex / 4, this.m_widthex + i * twidth, 2 * this.m_heightex);
					e.Graphics.DrawLine(this.BorderPen, this.m_widthex + i * twidth, base.Height - 2 * this.m_heightex, this.m_widthex + i * twidth, base.Height - 7 * this.m_heightex / 4);
				}
			}
			e.Graphics.FillRectangle(this.BackBrush, this.m_widthex + 1, 2 * this.m_heightex + 1, base.Width - 2 * this.m_widthex - 2, base.Height - 4 * this.m_heightex - 2);
		}

		private void RePaint()
		{
			DateTime now = DateTime.Now;
			if (!(this.lastPaintTime.AddMilliseconds(50.0) > now))
			{
				this.lastPaintTime = now;
				this.Refresh();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.SuspendLayout();
			this.is_paint = true;
			base.OnPaint(e);
			int num = base.Width - 2 * this.m_widthex;
			int num2 = num / 48;
			base.Width = num2 * 48 + 2 * this.m_widthex;
			this.PaintBorder(e, num2);
			if (this.m_times.Count > 0)
			{
				for (int i = 0; i < this.m_times.Count; i++)
				{
					Rectangle rect = this.m_times[i].Rect;
					if (i == this.m_selectindex)
					{
						e.Graphics.FillRectangle(this.SelecrBrush, rect);
					}
					else
					{
						e.Graphics.FillRectangle(this.ForeBrush, rect);
					}
					e.Graphics.DrawLine(this.BorderPen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
					e.Graphics.DrawLine(this.BorderPen, rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
				}
			}
			if (this.isdown && this.m_selectindex == -1)
			{
				e.Graphics.FillRectangle(this.SelecrBrush, this.currRect);
				e.Graphics.DrawLine(this.BorderPen, this.currRect.X, this.currRect.Y, this.currRect.X, this.currRect.Y + this.currRect.Height);
				e.Graphics.DrawLine(this.BorderPen, this.currRect.X + this.currRect.Width, this.currRect.Y, this.currRect.X + this.currRect.Width, this.currRect.Y + this.currRect.Height);
			}
			if (!this.isdown && this.moveTime >= 0)
			{
				string s = this.moveTime / 60 + ":" + this.moveTime % 60;
				e.Graphics.DrawString(s, this.IFont, this.FontBrush, (float)this.moveX, (float)this.moveY);
			}
			this.is_paint = false;
			base.ResumeLayout();
		}

		private bool IsInRect(int x, int y)
		{
			if (x > this.m_widthex && x < base.Width - this.m_widthex && y > 2 * this.m_heightex && y < base.Height - 2 * this.m_heightex)
			{
				return true;
			}
			return false;
		}

		public bool AddTime(int starttime, int endtime)
		{
			if (this.m_times.Count < 3)
			{
				TimeInfo timeInfo = new TimeInfo(this.m_widthex, this.m_heightex * 2, base.Width - 2 * this.m_widthex, base.Height - 4 * this.m_heightex, this);
				timeInfo.TimeValueChangedEvent += this.time_TimeValueChangedEvent;
				timeInfo.SetTime(starttime, endtime);
				this.m_times.Add(timeInfo);
				return true;
			}
			return false;
		}

		public bool RemveTimeAt(int index)
		{
			if (this.m_times.Count > 0 && index >= 0 && index < this.m_times.Count)
			{
				this.m_times.RemoveAt(index);
				return true;
			}
			return false;
		}

		public int GetSelectIndex(int x)
		{
			if (this.m_times != null && this.m_times.Count > 0)
			{
				for (int i = 0; i < this.m_times.Count; i++)
				{
					Rectangle rect = this.m_times[i].Rect;
					if (rect.X <= x && x <= rect.X + rect.Width)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int GetSelectIndexEx(int time, TimeInfo tinfo)
		{
			int result = -1;
			if (this.m_times != null && this.m_times.Count > 0)
			{
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				for (int i = 0; i < this.m_times.Count; i++)
				{
					if (this.m_times[i].StartTime <= time && time <= this.m_times[i].EndTime)
					{
						return i;
					}
					list.Add(this.m_times[i].StartTime);
					list2.Add(this.m_times[i].EndTime);
				}
				list.Sort();
				list2.Sort();
				if (time <= list[0] && tinfo.EndTime == list2[0])
				{
					result = 0;
				}
				if (time >= list2[list2.Count - 1] && tinfo.StartTime == list[list2.Count - 1])
				{
					result = list2.Count - 1;
				}
				if (time >= tinfo.EndTime)
				{
					int num = 0;
					while (num < list.Count)
					{
						if (time > list[num])
						{
							num++;
							continue;
						}
						result = num - 1;
						break;
					}
				}
				if (time <= tinfo.StartTime)
				{
					for (int j = 0; j < list2.Count; j++)
					{
						if (j < list2.Count - 1)
						{
							if (time >= list2[j] && time < list2[j + 1])
							{
								result = j + 1;
								break;
							}
						}
						else if (time >= list2[j])
						{
							result = j + 1;
							break;
						}
					}
				}
			}
			return result;
		}

		public TimeInfo GetTimeInfo(int index)
		{
			if (this.m_times.Count > index && index >= 0)
			{
				return this.m_times[index];
			}
			return null;
		}

		private void SetSelect(MouseEventArgs e)
		{
			if (this.m_times.Count > this.m_selectindex && this.m_selectindex >= 0)
			{
				TimeInfo timeInfo = this.m_times[this.m_selectindex];
				Rectangle rect = timeInfo.Rect;
				if (rect.X + 10 > e.X)
				{
					this.moveType = 1;
					this.Cursor = Cursors.SizeWE;
				}
				else
				{
					rect = timeInfo.Rect;
					if (rect.Right < e.X + 10)
					{
						this.moveType = 2;
						this.Cursor = Cursors.SizeWE;
					}
					else
					{
						this.moveType = 0;
						this.Cursor = Cursors.SizeAll;
					}
				}
				this.isdown = true;
			}
		}

		private void ReSet()
		{
			this.lastx = 0;
			this.moveType = 0;
			this.startx = 0;
			this.starty = 0;
			this.isdown = false;
			this.moveTime = -1;
			this.moveX = -1;
			this.moveY = -1;
			this.Cursor = Cursors.Default;
			this.currRect = default(Rectangle);
			if (this.SelectIndexChangedEvent != null)
			{
				this.SelectIndexChangedEvent(this, null);
			}
			this.RePaint();
		}

		private void TimeControl_MouseDown(object sender, MouseEventArgs e)
		{
			this.XDown = e.X;
			this.SetMenuEnabled(false);
			this.ReSet();
			this.lastx = e.X;
			if (this.m_times.Count < 3)
			{
				if (this.IsInRect(e.X, e.Y))
				{
					this.m_selectindex = this.GetSelectIndex(e.X);
					if (this.m_selectindex == -1)
					{
						this.Cursor = Cursors.Hand;
						this.startx = e.X;
						this.starty = 2 * this.m_heightex + 1;
						this.isdown = true;
						this.currRect = new Rectangle(this.startx, this.starty, 1, base.Height - 4 * this.m_heightex - 2);
					}
					else
					{
						this.SetMenuEnabled(true);
						this.SetSelect(e);
						if (this.SelectIndexChangedEvent != null)
						{
							this.SelectIndexChangedEvent(this, null);
						}
						this.Refresh();
					}
				}
			}
			else if (this.IsInRect(e.X, e.Y))
			{
				this.m_selectindex = this.GetSelectIndex(e.X);
				if (this.m_selectindex != -1)
				{
					this.SetMenuEnabled(true);
					this.SetSelect(e);
				}
				this.Refresh();
			}
		}

		private void moveEx(TimeInfo rct, int nwidth, MouseEventArgs e = null)
		{
			if (this.moveType == 0)
			{
				int num = rct.StartTime + rct.GetTime(rct.X + nwidth);
				int num2 = rct.EndTime + rct.GetTime(rct.X + nwidth);
				if (Math.Abs(num2 - num) >= 30)
				{
					rct.SetTime(num, num2);
				}
				else
				{
					this.m_times.Remove(rct);
					this.ReSet();
				}
			}
			else if (this.moveType == 1)
			{
				int num3 = rct.StartTime + rct.GetTime(rct.X + nwidth);
				if (Math.Abs(rct.EndTime - num3) >= 30)
				{
					rct.SetTime(num3, rct.EndTime);
				}
				else
				{
					this.m_times.Remove(rct);
					this.ReSet();
				}
			}
			else
			{
				int num4 = rct.EndTime + rct.GetTime(rct.X + nwidth);
				if (Math.Abs(num4 - rct.StartTime) >= 30)
				{
					rct.SetTime(rct.StartTime, num4);
				}
				else
				{
					this.m_times.Remove(rct);
					this.ReSet();
				}
			}
		}

		private void TimeControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.isdown)
			{
				if (this.IsInRect(e.X, e.Y))
				{
					if (Math.Abs(e.X - this.lastx) > 1)
					{
						if (e.X != this.startx)
						{
							if (this.m_selectindex == -1)
							{
								int selectIndex = this.GetSelectIndex(e.X);
								if (selectIndex == -1)
								{
									if (e.X > this.startx)
									{
										this.currRect.X = this.startx;
										this.currRect.Width = e.X - this.startx;
									}
									else
									{
										this.currRect.X = e.X;
										this.currRect.Width = this.startx - e.X;
									}
									if (this.SelectIndexChangedEvent != null)
									{
										this.SelectIndexChangedEvent(this, null);
									}
								}
							}
							else
							{
								int num = e.X - this.lastx;
								TimeInfo timeInfo = this.m_times[this.m_selectindex];
								int num2 = -1;
								Rectangle rect;
								if (num < 0)
								{
									rect = timeInfo.Rect;
									num2 = this.GetSelectIndex(rect.X + num);
								}
								else
								{
									rect = timeInfo.Rect;
									num2 = this.GetSelectIndex(rect.Right + num);
								}
								if (num2 == -1 || num2 == this.m_selectindex)
								{
									this.moveEx(timeInfo, num, e);
								}
								else
								{
									TimeInfo timeInfo2 = this.m_times[num2];
									if (num > 0)
									{
										int startTime = timeInfo.StartTime;
										int endTime = timeInfo2.EndTime;
										timeInfo.SetTime(startTime, endTime);
									}
									else
									{
										int startTime2 = timeInfo2.StartTime;
										int endTime2 = timeInfo.EndTime;
										timeInfo.SetTime(startTime2, endTime2);
									}
									this.moveType = 0;
									this.m_times.RemoveAt(num2);
									if (num2 < this.m_selectindex)
									{
										this.m_selectindex--;
									}
								}
								if (this.SelectIndexChangedEvent != null)
								{
									this.SelectIndexChangedEvent(this, null);
								}
							}
							this.RePaint();
						}
						this.lastx = e.X;
					}
				}
				else
				{
					this.CreateTime();
					this.ReSet();
				}
			}
			else if (this.IsInRect(e.X, e.Y))
			{
				if (Math.Abs(e.X - this.lastx) > 2)
				{
					TimeInfo timeInfo3 = new TimeInfo(this.m_widthex, this.m_heightex * 2, base.Width - 2 * this.m_widthex, base.Height - 4 * this.m_heightex, this);
					this.moveTime = timeInfo3.GetTime(e.X);
					this.moveX = e.X - this._fontSize;
					this.lastx = e.X;
					this.moveY = e.Y - this._fontSize;
					if (this.moveY > base.Height - 2 * this.m_heightex)
					{
						this.moveY = base.Height - 2 * this.m_heightex;
					}
					else if (this.moveY < 2 * this.m_heightex - 5)
					{
						this.moveY = 2 * this.m_heightex;
					}
					timeInfo3.TimeValueChangedEvent += this.time_TimeValueChangedEvent;
					this.Cursor = Cursors.Cross;
					this.RePaint();
				}
			}
			else
			{
				this.moveTime = -1;
			}
		}

		private void CreateTime()
		{
			TimeInfo timeInfo = null;
			if (this.m_selectindex == -1)
			{
				if (this.m_times.Count < 3)
				{
					timeInfo = new TimeInfo(this.m_widthex, this.m_heightex * 2, base.Width - 2 * this.m_widthex, base.Height - 4 * this.m_heightex, this);
					int time = timeInfo.GetTime(this.currRect.X);
					int time2 = timeInfo.GetTime(this.currRect.Right);
					if (Math.Abs(time2 - time) >= 30)
					{
						timeInfo.SetTime(time, time2);
						this.m_times.Add(timeInfo);
						this.AdjustTimeInfo(timeInfo);
					}
					timeInfo.TimeValueChangedEvent += this.time_TimeValueChangedEvent;
				}
			}
			else if (Math.Abs(this.lastx - this.XDown) >= 2)
			{
				this.AdjustTimeInfo(this.m_times[this.m_selectindex]);
			}
		}

		private void time_TimeValueChangedEvent(object sender, EventArgs e)
		{
			if (this.TimeValueChangedEvent != null)
			{
				this.TimeValueChangedEvent(sender, e);
			}
		}

		private void TimeControl_MouseUp(object sender, MouseEventArgs e)
		{
			this.moveX = 0;
			this.moveTime = -1;
			if (this.isdown)
			{
				this.CreateTime();
				this.ReSet();
				this.XDown = 0;
			}
		}

		private void TimeControl_SizeChanged(object sender, EventArgs e)
		{
			if (!this.is_paint)
			{
				base.SizeChanged -= this.TimeControl_SizeChanged;
				if (base.Height < 40)
				{
					base.Height = 40;
				}
				int num = base.Height - 4 * this.m_heightex;
				if (num < 20)
				{
					base.Height = 20 + 4 * this.m_heightex;
				}
				if (base.Width < 240)
				{
					base.Width = 240;
				}
				int num2 = base.Width - 2 * this.m_widthex;
				if (num2 < 240)
				{
					base.Width = 240 + 2 * this.m_widthex;
				}
				base.SizeChanged += this.TimeControl_SizeChanged;
				if (this.m_times.Count > 0)
				{
					List<TimeInfo> list = new List<TimeInfo>();
					for (int i = 0; i < this.m_times.Count; i++)
					{
						list.Add(this.m_times[i]);
					}
					this.m_times.Clear();
					for (int j = 0; j < list.Count; j++)
					{
						this.AddTime(list[j].StartTime, list[j].EndTime);
					}
				}
			}
		}

		private void TimeControl_MouseLeave(object sender, EventArgs e)
		{
			this.moveTime = -1;
			this.Refresh();
		}

		public void OnKeyUP(object sender, KeyEventArgs e)
		{
			if (this.m_selectindex != -1)
			{
				TimeControl timeControl = sender as TimeControl;
				if (timeControl != null && timeControl.Name != base.Name)
				{
					this.TimeControl_KeyUP(sender, e);
				}
			}
		}

		private void TimeControl_KeyUP(object sender, KeyEventArgs e)
		{
			if (this.m_selectindex != -1)
			{
				TimeInfo timeInfo = this.m_times[this.m_selectindex];
				if (timeInfo != null)
				{
					if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
					{
						int starttime = timeInfo.StartTime - this.m_keyMove;
						int endtime = timeInfo.EndTime - this.m_keyMove;
						timeInfo.SetTimeEx(starttime, endtime);
						this.RePaint();
					}
					else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
					{
						int starttime2 = timeInfo.StartTime + this.m_keyMove;
						int endtime2 = timeInfo.EndTime + this.m_keyMove;
						timeInfo.SetTimeEx(starttime2, endtime2);
						this.RePaint();
					}
					else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
					{
						int startTime = timeInfo.StartTime;
						int endtime3 = timeInfo.EndTime + this.m_keyMove;
						timeInfo.SetTimeEx(startTime, endtime3);
						this.RePaint();
					}
					else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Z)
					{
						int startTime2 = timeInfo.StartTime;
						int endtime4 = timeInfo.EndTime - this.m_keyMove;
						timeInfo.SetTimeEx(startTime2, endtime4);
						this.RePaint();
					}
				}
			}
			else
			{
				TimeControl timeControl = sender as TimeControl;
				if (timeControl != null && timeControl.Name == base.Name && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && this.KeyUpEvent != null)
				{
					this.KeyUpEvent(this, e);
				}
			}
		}

		private void TimeControl_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_selectindex != -1)
			{
				TimeInfo timeInfo = this.m_times[this.m_selectindex];
				if (timeInfo != null)
				{
					TimeInfoModify timeInfoModify = new TimeInfoModify(timeInfo);
					timeInfoModify.ShowDialog();
				}
			}
		}

		private void TimeControl_Click(object sender, EventArgs e)
		{
			if (this.m_selectindex != -1)
			{
				this.SetMenuEnabled(true);
				if (this.SelectIndexChangedEvent != null)
				{
					this.SelectIndexChangedEvent(this, e);
				}
			}
			else
			{
				this.SetMenuEnabled(false);
			}
		}

		private void SetMenuEnabled(bool isEnabled)
		{
			this.menu_delTime.Enabled = isEnabled;
			this.menu_modifyTime.Enabled = isEnabled;
		}

		private void menu_modifyTime_Click(object sender, EventArgs e)
		{
			if (this.m_selectindex != -1)
			{
				TimeInfo timeInfo = this.m_times[this.m_selectindex];
				if (timeInfo != null)
				{
					TimeInfoModify timeInfoModify = new TimeInfoModify(timeInfo);
					timeInfoModify.ShowDialog();
				}
				this.Refresh();
				base.Focus();
				this.Refresh();
				Application.DoEvents();
			}
		}

		private void menu_delTime_Click(object sender, EventArgs e)
		{
			if (this.m_selectindex != -1)
			{
				this.m_times.RemoveAt(this.m_selectindex);
				this.m_selectindex = -1;
				this.Refresh();
				this.SetMenuEnabled(false);
			}
		}

		private int GetDegreeTime(int time)
		{
			int num = time / this.DegreeTimeUnit * this.DegreeTimeUnit;
			return (time > num + this.DegreeTimeUnit / 2) ? (num + this.DegreeTimeUnit) : num;
		}

		private void AdjustTimeInfo(TimeInfo time)
		{
			int degreeTime = this.GetDegreeTime(time.StartTime);
			int degreeTime2 = this.GetDegreeTime(time.EndTime);
			if (Math.Abs(degreeTime2 - degreeTime) >= 30)
			{
				time.SetTime(degreeTime, degreeTime2);
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
			this.components = new Container();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.menu_modifyTime = new ToolStripMenuItem();
			this.menu_delTime = new ToolStripMenuItem();
			this.contextMenuStrip1.SuspendLayout();
			base.SuspendLayout();
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[2]
			{
				this.menu_modifyTime,
				this.menu_delTime
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(153, 70);
			this.menu_modifyTime.Image = Resources.edit;
			this.menu_modifyTime.Name = "menu_modifyTime";
			this.menu_modifyTime.Size = new Size(152, 22);
			this.menu_modifyTime.Text = "修改时间区间";
			this.menu_modifyTime.Click += this.menu_modifyTime_Click;
			this.menu_delTime.Image = Resources.delete;
			this.menu_delTime.Name = "menu_delTime";
			this.menu_delTime.Size = new Size(152, 22);
			this.menu_delTime.Text = "删除时间区间";
			this.menu_delTime.Click += this.menu_delTime_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.ContextMenuStrip = this.contextMenuStrip1;
			base.Name = "TimeControl";
			base.Size = new Size(222, 40);
			base.SizeChanged += this.TimeControl_SizeChanged;
			base.Click += this.TimeControl_Click;
			base.DoubleClick += this.TimeControl_DoubleClick;
			base.KeyUp += this.TimeControl_KeyUP;
			base.MouseDown += this.TimeControl_MouseDown;
			base.MouseLeave += this.TimeControl_MouseLeave;
			base.MouseMove += this.TimeControl_MouseMove;
			base.MouseUp += this.TimeControl_MouseUp;
			this.contextMenuStrip1.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
