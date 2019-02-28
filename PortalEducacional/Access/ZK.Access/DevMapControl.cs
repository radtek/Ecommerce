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
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DevMapControl : UserControl
	{
		public TabItem DevTabItem;

		private int m_id = 0;

		private Dictionary<int, Machines> mlist = new Dictionary<int, Machines>();

		private int m_lastx = 10;

		private int m_lasty = 10;

		private int m_nwidth = 5;

		private int m_nheight = 5;

		private List<DevControl> m_doorControls = new List<DevControl>();

		private Dictionary<int, AccDoor> dlist = new Dictionary<int, AccDoor>();

		private Timer m_time = new Timer();

		private bool m_isMutilSelected = false;

		private bool m_isDown = false;

		private bool m_ismoveSelecting = false;

		private int m_startX = 0;

		private int m_starty = 0;

		private int m_lastX = 0;

		private int m_lastY = 0;

		private static string[] Fonts = new string[5]
		{
			"Arial",
			"宋体",
			"Impact",
			"Courier New",
			"Tahoma"
		};

		private static Color[] Colors = new Color[8]
		{
			Color.White,
			Color.Black,
			Color.FromArgb(127, 143, 154),
			Color.LightGreen,
			Color.LightBlue,
			Color.FromArgb(90, 34, 130, 255),
			Color.Green,
			Color.Blue
		};

		private SolidBrush _brush = null;

		private SolidBrush _forebrush = null;

		private SolidBrush _fontbrush = null;

		private SolidBrush _selectbrush = null;

		private Pen _selectPen = null;

		private Pen _borderPen = null;

		private int _fontSize = 9;

		private Font _font = null;

		private Bitmap m_bgimg = null;

		private bool m_isPaiting = false;

		private bool m_lock = false;

		private int m_startLeft = 0;

		private int m_startTop = 0;

		private IContainer components = null;

		public int MapID => this.m_id;

		public List<DevControl> DoorControls
		{
			get
			{
				if (this.m_doorControls == null)
				{
					this.m_doorControls = new List<DevControl>();
				}
				return this.m_doorControls;
			}
		}

		public bool IsMutilSelected => this.m_isMutilSelected;

		public SolidBrush BackBrush
		{
			get
			{
				if (this._brush == null)
				{
					this._brush = new SolidBrush(this.BackColor);
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
					this._forebrush = new SolidBrush(DevMapControl.Colors[3]);
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
					this._fontbrush = new SolidBrush(DevMapControl.Colors[6]);
				}
				return this._fontbrush;
			}
			set
			{
				this._fontbrush = value;
			}
		}

		public SolidBrush SelectBrush
		{
			get
			{
				if (this._selectbrush == null)
				{
					this._selectbrush = new SolidBrush(DevMapControl.Colors[5]);
				}
				return this._selectbrush;
			}
			set
			{
				this._selectbrush = value;
			}
		}

		public Pen SelectPen
		{
			get
			{
				if (this._selectPen == null)
				{
					this._selectPen = new Pen(DevMapControl.Colors[4]);
				}
				return this._selectPen;
			}
			set
			{
				this._selectPen = value;
			}
		}

		public Pen BorderPen
		{
			get
			{
				if (this._borderPen == null)
				{
					this._borderPen = new Pen(DevMapControl.Colors[1]);
				}
				return this._borderPen;
			}
			set
			{
				this._borderPen = value;
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
					this._font = new Font(DevMapControl.Fonts[1], (float)this._fontSize, FontStyle.Regular);
				}
				return this._font;
			}
			set
			{
				this._font = value;
			}
		}

		public Image BackImg
		{
			get
			{
				return this.m_bgimg;
			}
			set
			{
				if (value != null)
				{
					Bitmap bitmap = new Bitmap(value.Width, value.Height);
					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(value, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					this.m_bgimg = bitmap;
					graphics.Dispose();
					this.SetBakImg();
				}
			}
		}

		public bool IsLock
		{
			get
			{
				return this.m_lock;
			}
			set
			{
				this.m_lock = value;
			}
		}

		public int StartLeft => this.m_startLeft;

		public int StartTop => this.m_startTop;

		public int ImgHeight
		{
			get
			{
				if (this.m_bgimg == null)
				{
					return base.Height;
				}
				if (this.m_bgimg.Height > base.Height)
				{
					return this.m_bgimg.Height;
				}
				return base.Height;
			}
		}

		public int ImgWidth
		{
			get
			{
				if (this.m_bgimg == null)
				{
					return base.Width;
				}
				if (this.m_bgimg.Width > base.Width)
				{
					return this.m_bgimg.Width;
				}
				return base.Width;
			}
		}

		public event EventHandler LoadFinishEvent = null;

		public DevMapControl()
		{
			this.InitializeComponent();
			base.ParentChanged -= this.DevMapControl_ParentChanged;
			base.ParentChanged += this.DevMapControl_ParentChanged;
			try
			{
				base.SetStyle(ControlStyles.UserPaint, true);
				base.SetStyle(ControlStyles.DoubleBuffer, true);
				base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			}
			catch
			{
			}
		}

		private void InitMachines()
		{
			this.mlist.Clear();
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (!this.mlist.ContainsKey(list[i].ID))
					{
						this.mlist.Add(list[i].ID, list[i]);
					}
				}
			}
			machinesBll = null;
			list.Clear();
			list = null;
		}

		private void LoadDoor()
		{
			this.dlist.Clear();
			AccDoorBll accDoorBll = new AccDoorBll(MainForm._ia);
			List<AccDoor> modelList = accDoorBll.GetModelList("");
			if (modelList != null && modelList.Count > 0)
			{
				for (int i = 0; i < modelList.Count; i++)
				{
					if (!this.dlist.ContainsKey(modelList[i].id) && this.mlist.ContainsKey(modelList[i].device_id))
					{
						this.dlist.Add(modelList[i].id, modelList[i]);
					}
				}
			}
			modelList.Clear();
			modelList = null;
			accDoorBll = null;
		}

		public void LoadMap(int mapid)
		{
			SysLogServer.WriteLog("LoadMap...1", true);
			DevLogServer.DeleteDoorEvent -= this.DevLogServer_DeleteDoorEvent;
			DevLogServer.DeleteDoorEvent += this.DevLogServer_DeleteDoorEvent;
			SysLogServer.WriteLog("LoadMap...2", true);
			base.SetStyle(ControlStyles.UserPaint, true);
			base.SetStyle(ControlStyles.DoubleBuffer, true);
			base.MouseWheel -= this.DevMapControl_MouseWheel;
			base.MouseWheel += this.DevMapControl_MouseWheel;
			SysLogServer.WriteLog("LoadMap...3", true);
			this.m_id = mapid;
			this.m_doorControls.Clear();
			base.Controls.Clear();
			this.InitMachines();
			this.LoadDoor();
			if (this.m_time != null)
			{
				SysLogServer.WriteLog("LoadMap...4", true);
				this.m_time.Interval = 50;
				this.m_time.Tick -= this.m_time_Tick;
				this.m_time.Tick += this.m_time_Tick;
				this.m_time.Enabled = true;
				SysLogServer.WriteLog("LoadMap...5", true);
			}
		}

		private void DevLogServer_DeleteDoorEvent(object sender, EventArgs e)
		{
			if (base.Visible && !base.IsDisposed)
			{
				if (base.InvokeRequired)
				{
					object[] args = new object[2]
					{
						sender,
						e
					};
					base.BeginInvoke(new EventHandler(this.DevLogServer_DeleteDoorEvent), args);
				}
				else
				{
					this.LoadMap(this.m_id);
				}
			}
		}

		public object CopyDictionaryData(object obj)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, obj);
			memoryStream.Position = 0L;
			object result = binaryFormatter.Deserialize(memoryStream);
			memoryStream.Close();
			return result;
		}

		private void m_time_Tick(object sender, EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;
			SysLogServer.WriteLog("m_time_Tick...1", true);
			this.m_time.Enabled = false;
			AccMapdoorposBll accMapdoorposBll = new AccMapdoorposBll(MainForm._ia);
			SysLogServer.WriteLog("m_time_Tick...2", true);
			List<AccMapdoorpos> list = null;
			if (this.m_id > 0)
			{
				SysLogServer.WriteLog("m_time_Tick...3", true);
				list = accMapdoorposBll.GetModelList("map_id=" + this.m_id);
				SysLogServer.WriteLog("m_time_Tick...4", true);
				if (list != null && list.Count > 0)
				{
					int num = 0;
					int num2 = 0;
					for (int i = 0; i < list.Count; i++)
					{
						SysLogServer.WriteLog("m_time_Tick...5", true);
						if (this.dlist.ContainsKey(list[i].map_door_id))
						{
							SysLogServer.WriteLog("m_time_Tick...6", true);
							if (list[i].top < num)
							{
								num = list[i].top;
							}
							if (list[i].left < num2)
							{
								num2 = list[i].left;
							}
						}
					}
					num -= 5;
					num2 -= 5;
					for (int j = 0; j < list.Count; j++)
					{
						SysLogServer.WriteLog("m_time_Tick...7", true);
						if (this.dlist.ContainsKey(list[j].map_door_id))
						{
							SysLogServer.WriteLog("m_time_Tick...8", true);
							AccDoor door = this.dlist[list[j].map_door_id];
							this.AddDoor(door, list[j].left - num2, list[j].top - num);
						}
					}
					SysLogServer.WriteLog("m_time_Tick...9", true);
					if (this.m_doorControls != null)
					{
						for (int k = 0; k < this.m_doorControls.Count; k++)
						{
							this.m_doorControls[k].IsLoadFinish = true;
							this.m_doorControls[k].IsMove = true;
						}
					}
					SysLogServer.WriteLog("m_time_Tick...10", true);
				}
			}
			else
			{
				SysLogServer.WriteLog("m_time_Tick...11", true);
				int num3 = this.dlist.Count;
				SysLogServer.WriteLog("m_time_Tick...dlist = " + num3.ToString(), true);
				try
				{
					if (this.dlist != null)
					{
						Dictionary<int, AccDoor> dictionary = new Dictionary<int, AccDoor>();
						dictionary = (Dictionary<int, AccDoor>)this.CopyDictionaryData(this.dlist);
						foreach (KeyValuePair<int, AccDoor> item in dictionary)
						{
							num3 = item.Value.device_id;
							SysLogServer.WriteLog("m_time_Tick...key.Value.device_id = " + num3.ToString(), true);
							this.AddDoor(item.Value);
							SysLogServer.WriteLog("m_time_Tick...666 ", true);
						}
					}
				}
				catch (Exception ex)
				{
					SysLogServer.WriteLog("m_time_Tick...AddDoor ...ex.message=" + ex.Message, true);
				}
				SysLogServer.WriteLog("m_time_Tick...12", true);
				if (this.m_doorControls != null)
				{
					for (int l = 0; l < this.m_doorControls.Count; l++)
					{
						this.m_doorControls[l].IsLoadFinish = true;
					}
				}
				SysLogServer.WriteLog("m_time_Tick...13", true);
			}
			SysLogServer.WriteLog("m_time_Tick...14", true);
			if (this.LoadFinishEvent != null)
			{
				SysLogServer.WriteLog("m_time_Tick...15", true);
				this.LoadFinishEvent(this, null);
			}
			SysLogServer.WriteLog("m_time_Tick...16", true);
			this.Cursor = Cursors.Default;
		}

		public void AddDoor(AccDoor door)
		{
			SysLogServer.WriteLog("1->AddDoor...m_lastx = " + this.m_lastx.ToString() + ",m_lasty= " + this.m_lasty.ToString(), true);
			this.AddDoor(door, this.m_lastx, this.m_lasty);
		}

		public void AddDoor(AccDoor door, int x, int y)
		{
			if (door != null)
			{
				Application.DoEvents();
				SysLogServer.WriteLog("AddDoor...1 ", true);
				try
				{
					DevControl devControl = null;
					if (this.m_doorControls != null)
					{
						for (int i = 0; i < this.m_doorControls.Count; i++)
						{
							SysLogServer.WriteLog("AddDoor...111 ", true);
							if (this.m_doorControls[i].AccDoorInfo != null && this.m_doorControls[i].AccDoorInfo.id == door.id)
							{
								SysLogServer.WriteLog("AddDoor...222 ", true);
								devControl = this.m_doorControls[i];
								SysLogServer.WriteLog("AddDoor...333 ", true);
								break;
							}
							SysLogServer.WriteLog("AddDoor...444 ", true);
						}
						SysLogServer.WriteLog("AddDoor...2 ", true);
						if (devControl == null)
						{
							SysLogServer.WriteLog("AddDoor...3 ", true);
							devControl = new DevControl();
							SysLogServer.WriteLog("AddDoor...4 ", true);
							devControl.SelectChangedEvent += this.doordev_SelectChangedEvent;
							SysLogServer.WriteLog("AddDoor...5 ", true);
							devControl.MutilSelectChangedEvent += this.doordev_MutilSelectChangedEvent;
							SysLogServer.WriteLog("AddDoor...6 ", true);
							devControl.DoorStateChanged += this.doordev_DoorStateChanged;
							devControl.MapControl = this;
							SysLogServer.WriteLog("AddDoor...7 ", true);
							this.m_doorControls.Add(devControl);
							SysLogServer.WriteLog("AddDoor...8 ", true);
							base.Controls.Add(devControl);
							SysLogServer.WriteLog("AddDoor...9 ", true);
							devControl.AccDoorInfo = door;
							if (x < 10)
							{
								x = 10;
							}
							if (y < 10)
							{
								y = 10;
							}
							devControl.Left = x;
							devControl.Top = y;
							if (x + devControl.Width * 2 + 2 * this.m_nwidth < base.Width)
							{
								this.m_lastx += devControl.Width + this.m_nwidth;
							}
							else
							{
								this.m_lastx = 10;
								this.m_lasty = this.m_lasty + devControl.Height + this.m_nheight;
							}
							SysLogServer.WriteLog("AddDoor...10 ", true);
						}
						else
						{
							SysLogServer.WriteLog("AddDoor...11 ", true);
							devControl.Visible = true;
							SysLogServer.WriteLog("AddDoor...12 ", true);
						}
					}
				}
				catch (Exception ex)
				{
					SysLogServer.WriteLog("AddDoore...e.Message=" + ex.Message, true);
				}
			}
		}

		private void doordev_DoorStateChanged(object sender)
		{
			if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
			{
				bool flag = false;
				DevControl devControl = sender as DevControl;
				if (this.DevTabItem != null && devControl != null)
				{
					DoorState status = devControl.Status;
					if ((uint)(status - 3) <= 1u)
					{
						flag = true;
					}
					else if (devControl.MapControl != null)
					{
						foreach (DevControl doorControl in devControl.MapControl.DoorControls)
						{
							DoorState status2 = doorControl.Status;
							if ((uint)(status2 - 3) <= 1u)
							{
								flag = true;
							}
							if (flag)
							{
								break;
							}
						}
					}
					if (flag)
					{
						Graphics graphics = base.CreateGraphics();
						Font defaultFont = SysInfos.DefaultFont;
						SolidBrush brush = new SolidBrush(Color.Black);
						SizeF sizeF = graphics.MeasureString(this.DevTabItem.Tooltip, defaultFont);
						graphics.Dispose();
						graphics = null;
						SolidBrush brush2 = new SolidBrush(Color.Red);
						Bitmap bitmap = new Bitmap((int)Math.Ceiling((double)(sizeF.Width + 4f)), (int)Math.Ceiling((double)(sizeF.Height + 2f)));
						graphics = Graphics.FromImage(bitmap);
						graphics.FillRectangle(brush2, 0, 0, bitmap.Width, bitmap.Height);
						PointF point = new PointF((float)(bitmap.Width / 2) - sizeF.Width / 2f, (float)(bitmap.Height / 2) - sizeF.Height / 2f);
						graphics.DrawString(this.DevTabItem.Tooltip, defaultFont, brush, point);
						graphics.Dispose();
						graphics = null;
						this.DevTabItem.Image = bitmap;
						this.DevTabItem.Text = "";
					}
					else
					{
						this.DevTabItem.Image = null;
						this.DevTabItem.Text = this.DevTabItem.Tooltip;
					}
				}
			}
		}

		private void doordev_MutilSelectChangedEvent(object sender, EventArgs e)
		{
			if (sender != null)
			{
				SysLogServer.WriteLog("doordev_MutilSelectChangedEvent...1 ", true);
				DevControl devControl = sender as DevControl;
				SysLogServer.WriteLog("doordev_MutilSelectChangedEvent...2 ", true);
				if (devControl != null)
				{
					if (devControl.IsMutilSelected)
					{
						SysLogServer.WriteLog("doordev_MutilSelectChangedEvent...3 ", true);
						this.m_isMutilSelected = true;
						SysLogServer.WriteLog("doordev_MutilSelectChangedEvent...4 ", true);
					}
					else
					{
						SysLogServer.WriteLog("doordev_MutilSelectChangedEvent...5 ", true);
						this.m_isMutilSelected = false;
						SysLogServer.WriteLog("doordev_MutilSelectChangedEvent...6 ", true);
					}
				}
			}
		}

		public DevControl GetDoor(int id)
		{
			if (this.m_doorControls != null)
			{
				if (this.m_doorControls.Count > 0)
				{
					for (int i = 0; i < this.m_doorControls.Count; i++)
					{
						if (this.m_doorControls[i].AccDoorInfo.id == id)
						{
							return this.m_doorControls[i];
						}
					}
				}
				if (this.dlist.ContainsKey(id))
				{
					AccDoor door = this.dlist[id];
					this.AddDoor(door);
					return this.m_doorControls[this.m_doorControls.Count - 1];
				}
			}
			return null;
		}

		private void DevMapControl_Click(object sender, EventArgs e)
		{
			if (!this.m_ismoveSelecting)
			{
				if (this.m_doorControls != null && this.m_doorControls.Count > 0)
				{
					for (int i = 0; i < this.m_doorControls.Count; i++)
					{
						this.m_doorControls[i].IsSelected = false;
					}
				}
			}
			else
			{
				this.m_ismoveSelecting = false;
			}
		}

		private void doordev_SelectChangedEvent(object sender, EventArgs e)
		{
			if (!this.m_isMutilSelected)
			{
				SysLogServer.WriteLog("doordev_SelectChangedEvent...1 ", true);
				if (sender != null)
				{
					DevControl devControl = sender as DevControl;
					if (devControl != null && devControl.IsSelected)
					{
						SysLogServer.WriteLog("doordev_SelectChangedEvent...2 ", true);
						if (this.m_doorControls != null)
						{
							for (int i = 0; i < this.m_doorControls.Count; i++)
							{
								if (devControl != this.m_doorControls[i] && this.m_doorControls[i].IsSelected)
								{
									SysLogServer.WriteLog("doordev_SelectChangedEvent...3 ", true);
									this.m_doorControls[i].IsSelected = false;
								}
							}
						}
						SysLogServer.WriteLog("doordev_SelectChangedEvent...4 ", true);
					}
				}
			}
		}

		private void DevMapControl_KeyUp(object sender, KeyEventArgs e)
		{
			this.m_isMutilSelected = false;
		}

		private void DevMapControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Control || e.KeyCode == Keys.ControlKey)
			{
				this.m_isMutilSelected = true;
			}
		}

		private void SetBakImg()
		{
			try
			{
				if (base.Width > 50 && base.Height > 50)
				{
					if (this.m_isDown || this.m_bgimg != null)
					{
						if (base.VerticalScroll != null)
						{
							this.m_startTop = base.VerticalScroll.Value;
						}
						if (base.HorizontalScroll != null)
						{
							this.m_startLeft = base.HorizontalScroll.Value;
						}
						Bitmap bitmap = new Bitmap(base.Width, base.Height);
						Graphics graphics = Graphics.FromImage(bitmap);
						graphics.Clear(Color.FromArgb(0, 255, 255, 255));
						if (this.m_bgimg != null)
						{
							int num = this.m_bgimg.Width - this.m_startLeft;
							int num2 = this.m_bgimg.Height - this.m_startTop;
							int width = base.Width;
							int height = base.Height;
							if (num > width)
							{
								num = width;
							}
							if (num2 > height)
							{
								num2 = height;
							}
							if (num2 > 0 && num > 0)
							{
								graphics.DrawImage(this.m_bgimg, new Rectangle(0, 0, num, num2), new Rectangle(this.m_startLeft, this.m_startTop, num, num2), GraphicsUnit.Pixel);
							}
						}
						if (this.m_isDown)
						{
							if (this.m_startX < this.m_lastX && this.m_starty < this.m_lastY)
							{
								graphics.DrawRectangle(this.BorderPen, this.m_startX, this.m_starty, this.m_lastX - this.m_startX, this.m_lastY - this.m_starty);
							}
							else if (this.m_startX > this.m_lastX && this.m_starty < this.m_lastY)
							{
								graphics.DrawRectangle(this.BorderPen, this.m_lastX, this.m_starty, this.m_startX - this.m_lastX, this.m_lastY - this.m_starty);
							}
							else if (this.m_startX > this.m_lastX && this.m_starty > this.m_lastY)
							{
								graphics.DrawRectangle(this.BorderPen, this.m_lastX, this.m_lastY, this.m_startX - this.m_lastX, this.m_starty - this.m_lastY);
							}
							else if (this.m_startX < this.m_lastX && this.m_starty > this.m_lastY)
							{
								graphics.DrawRectangle(this.BorderPen, this.m_startX, this.m_lastY, this.m_lastX - this.m_startX, this.m_starty - this.m_lastY);
							}
						}
						try
						{
							if (this.BackgroundImage != null)
							{
								this.BackgroundImage.Dispose();
								this.BackgroundImage = null;
							}
						}
						catch
						{
						}
						this.BackgroundImage = bitmap;
						graphics.Dispose();
						graphics = null;
					}
					else
					{
						try
						{
							if (this.BackgroundImage != null)
							{
								this.BackgroundImage.Dispose();
								this.BackgroundImage = null;
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}

		public void ReSet()
		{
			this.m_isDown = false;
			this.m_startX = 0;
			this.m_starty = 0;
			this.m_lastX = 0;
			this.m_lastY = 0;
			this.m_isMutilSelected = false;
			this.m_ismoveSelecting = false;
			this.SetBakImg();
		}

		public bool IsHaveSelected()
		{
			bool result = false;
			if (this.m_doorControls != null)
			{
				int num = 0;
				while (num < this.m_doorControls.Count)
				{
					if (!this.m_doorControls[num].Visible || !this.m_doorControls[num].IsSelected)
					{
						num++;
						continue;
					}
					result = true;
					break;
				}
			}
			return result;
		}

		public List<DevControl> GetSelecteds()
		{
			List<DevControl> list = new List<DevControl>();
			if (this.m_doorControls != null)
			{
				for (int i = 0; i < this.m_doorControls.Count; i++)
				{
					if (this.m_doorControls[i].Visible && this.m_doorControls[i].IsSelected)
					{
						list.Add(this.m_doorControls[i]);
					}
				}
			}
			return list;
		}

		private void DevMapControl_MouseDown(object sender, MouseEventArgs e)
		{
			this.m_isDown = true;
			this.m_startX = e.X;
			this.m_starty = e.Y;
			this.m_lastX = e.X;
			this.m_lastY = e.Y;
			this.m_isMutilSelected = true;
		}

		private bool IsInRect(DevControl doordev, int left, int top, int bottom, int right)
		{
			bool result = false;
			if (doordev.Left > left && doordev.Left < right && doordev.Top > top && doordev.Top < bottom)
			{
				result = true;
			}
			else if (doordev.Right > left && doordev.Right < right && doordev.Top > top && doordev.Top < bottom)
			{
				result = true;
			}
			else if (doordev.Right > left && doordev.Right < right && doordev.Bottom > top && doordev.Bottom < bottom)
			{
				result = true;
			}
			else if (doordev.Left > left && doordev.Left < right && doordev.Bottom > top && doordev.Bottom < bottom)
			{
				result = true;
			}
			return result;
		}

		private void DevMapControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.m_isDown)
			{
				this.m_ismoveSelecting = true;
				if (Math.Abs(this.m_lastX - e.X) > 5 || Math.Abs(this.m_lastY - e.Y) > 5)
				{
					this.m_lastX = e.X;
					this.m_lastY = e.Y;
					int num = this.m_startX;
					int num2 = this.m_lastX;
					int num3 = this.m_starty;
					int num4 = this.m_lastY;
					if (num > num2)
					{
						num = num2;
						num2 = this.m_startX;
					}
					if (num3 > num4)
					{
						num3 = num4;
						num4 = this.m_starty;
					}
					if (this.m_doorControls != null && this.m_doorControls.Count > 0)
					{
						for (int i = 0; i < this.m_doorControls.Count; i++)
						{
							if (this.IsInRect(this.m_doorControls[i], num, num3, num4, num2))
							{
								this.m_doorControls[i].IsSelected = true;
							}
							else
							{
								this.m_doorControls[i].IsSelected = false;
							}
						}
					}
					this.SetBakImg();
				}
			}
			else
			{
				this.m_isMutilSelected = false;
			}
		}

		private void DevMapControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.m_isDown)
			{
				this.ReSet();
			}
		}

		private void DevMapControl_MouseLeave(object sender, EventArgs e)
		{
			if (this.m_isDown)
			{
				this.ReSet();
			}
		}

		public void ReLayout()
		{
			this.DevMapControl_SizeChanged(this, null);
		}

		private void DevMapControl_SizeChanged(object sender, EventArgs e)
		{
			if (!this.IsLock && this.m_id == 0 && this.m_doorControls != null)
			{
				List<DevControl> doorControls = this.m_doorControls;
				if (doorControls != null && doorControls.Count > 0)
				{
					int num = 3;
					int num2 = 3;
					for (int i = 0; i < doorControls.Count; i++)
					{
						if (doorControls[i].Visible)
						{
							doorControls[i].Left = num;
							doorControls[i].Top = num2;
							num += doorControls[i].Width + 5;
							if (num + doorControls[i].Width + 5 >= base.Width)
							{
								num = 3;
								num2 = num2 + doorControls[i].Height + 5;
							}
						}
					}
				}
			}
			this.SetBakImg();
		}

		private void DevMapControl_ParentChanged(object sender, EventArgs e)
		{
			this.SetBakImg();
		}

		private void DisposeEx(Control ctl)
		{
			if (ctl != null)
			{
				if (ctl.Controls.Count > 0)
				{
					for (int i = 0; i < ctl.Controls.Count; i++)
					{
						this.DisposeEx(ctl.Controls[i]);
					}
				}
				ctl.Controls.Clear();
				ctl.Dispose();
				ctl = null;
			}
		}

		public void Free()
		{
			try
			{
				if (this.m_time != null)
				{
					this.m_time.Dispose();
					this.m_time = null;
				}
			}
			catch
			{
			}
			try
			{
				if (this.m_bgimg != null)
				{
					this.m_bgimg.Dispose();
					this.m_bgimg = null;
				}
			}
			catch
			{
			}
			try
			{
				this.BackgroundImage = null;
				List<DevControl> doorControls = this.m_doorControls;
				if (doorControls != null && doorControls.Count > 0)
				{
					for (int i = 0; i < doorControls.Count; i++)
					{
						doorControls[i].Free();
						doorControls[i].Dispose();
					}
				}
				this.m_doorControls.Clear();
				this.m_doorControls = null;
			}
			catch
			{
			}
			try
			{
				if (base.Controls.Count > 0)
				{
					for (int j = 0; j < base.Controls.Count; j++)
					{
						if (base.Controls[j] != null)
						{
							this.DisposeEx(base.Controls[j]);
						}
					}
					base.Controls.Clear();
				}
			}
			catch
			{
			}
			this.mlist.Clear();
			this.mlist = null;
			this.dlist.Clear();
			this.dlist = null;
			this._font = null;
			this._borderPen = null;
			this._selectPen = null;
			this._selectbrush = null;
			this._fontbrush = null;
			this._forebrush = null;
			this._brush = null;
			Application.DoEvents();
		}

		private void DevMapControl_Scroll(object sender, ScrollEventArgs e)
		{
			this.RetSetBg();
		}

		private void RetSetBg()
		{
			this.SetBakImg();
		}

		private void DevMapControl_MouseWheel(object sender, MouseEventArgs e)
		{
			this.RetSetBg();
		}

		protected override void Dispose(bool disposing)
		{
			if (this.BackgroundImage != null)
			{
				this.BackgroundImage = null;
			}
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoScroll = true;
			base.AutoScrollMargin = new Size(5, 5);
			base.AutoScrollMinSize = new Size(10, 10);
			this.BackgroundImageLayout = ImageLayout.None;
			base.Name = "DevMapControl";
			base.Scroll += this.DevMapControl_Scroll;
			base.SizeChanged += this.DevMapControl_SizeChanged;
			base.Click += this.DevMapControl_Click;
			base.KeyDown += this.DevMapControl_KeyDown;
			base.KeyUp += this.DevMapControl_KeyUp;
			base.MouseDown += this.DevMapControl_MouseDown;
			base.MouseLeave += this.DevMapControl_MouseLeave;
			base.MouseMove += this.DevMapControl_MouseMove;
			base.MouseUp += this.DevMapControl_MouseUp;
			base.ResumeLayout(false);
		}
	}
}
