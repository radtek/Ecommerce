/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Drawing;

namespace ZK.Access
{
	public class TimeInfo
	{
		private TimeControl m_time = null;

		private int m_x = 0;

		private int m_y = 0;

		private int m_height = 0;

		private int m_width = 0;

		private int m_id = 0;

		private int m_starttime = 0;

		private int m_endtime = 0;

		private int m_starttime_temp = 0;

		private int m_endtime_temp = 0;

		private Rectangle rect = default(Rectangle);

		public TimeControl Time => this.m_time;

		public int ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		public int StartTime
		{
			get
			{
				if (this.m_starttime > 1438)
				{
					this.m_starttime = 1438;
				}
				return this.m_starttime;
			}
		}

		public int EndTime
		{
			get
			{
				if (this.m_endtime > 1439)
				{
					this.m_endtime = 1439;
				}
				return this.m_endtime;
			}
		}

		public int X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				if (value > 0)
				{
					this.m_x = value;
					this.GetRect();
				}
			}
		}

		public int Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				if (value > 0)
				{
					this.m_y = value;
					this.GetRect();
				}
			}
		}

		public int Height
		{
			get
			{
				return this.m_height;
			}
			set
			{
				if (value > 0)
				{
					this.m_height = value;
					this.GetRect();
				}
			}
		}

		public int Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				if (value > 0)
				{
					this.m_width = value;
					this.GetRect();
				}
			}
		}

		public Rectangle Rect => this.rect;

		public event EventHandler TimeValueChangedEvent;

		public TimeInfo(int x, int y, int width, int height, TimeControl time)
		{
			this.m_time = time;
			this.m_x = x;
			this.m_y = y;
			this.m_width = width;
			this.m_height = height;
			this.rect.X = -1;
			this.rect.Y = -1;
			this.rect.Width = 1;
			this.rect.Height = 1;
		}

		public void SetTime(int starttime, int endtime)
		{
			if (endtime >= 1440)
			{
				endtime = 1439;
			}
			if (starttime >= 1440)
			{
				starttime = 1439;
			}
			if (starttime >= 0 && starttime < 1440 && endtime >= 0 && endtime < 1440 && endtime != starttime)
			{
				if (endtime < starttime)
				{
					this.m_starttime = endtime;
					this.m_endtime = starttime;
				}
				else
				{
					this.m_endtime = endtime;
					this.m_starttime = starttime;
				}
				if (this.TimeValueChangedEvent != null)
				{
					this.TimeValueChangedEvent(this, null);
				}
				this.GetRect();
			}
		}

		public void SetTimeEx(int starttime, int endtime)
		{
			if (starttime >= 0 && starttime < 1440 && endtime >= 0 && endtime < 1440 && endtime != starttime)
			{
				if (endtime < starttime)
				{
					int num = endtime;
					endtime = starttime;
					starttime = num;
				}
				int num2 = -1;
				int num3 = -1;
				if (this.m_time != null)
				{
					num2 = this.m_time.GetSelectIndexEx(starttime, this);
					if (num2 >= 0)
					{
						TimeInfo timeInfo = this.m_time.GetTimeInfo(num2);
						if (timeInfo != null && timeInfo != this)
						{
							starttime = timeInfo.EndTime + 1;
						}
					}
					num3 = this.m_time.GetSelectIndexEx(endtime, this);
					if (num3 >= 0)
					{
						TimeInfo timeInfo2 = this.m_time.GetTimeInfo(num3);
						if (timeInfo2 != null && timeInfo2 != this)
						{
							endtime = timeInfo2.StartTime - 1;
						}
					}
				}
				if (num2 >= 0)
				{
					this.m_starttime = starttime;
				}
				if (num3 >= 0)
				{
					this.m_endtime = endtime;
				}
				if (this.TimeValueChangedEvent != null)
				{
					this.TimeValueChangedEvent(this, null);
				}
				this.GetRect();
			}
		}

		private int GetNWidth()
		{
			int result = 0;
			if (this.Width > 0)
			{
				result = Math.Abs(this.Width * (this.m_endtime - this.m_starttime)) / 1440;
			}
			return result;
		}

		private int GetStartX()
		{
			if (this.m_starttime < this.m_endtime)
			{
				return this.X + this.Width * this.m_starttime / 1440;
			}
			return this.X + this.Width * this.m_endtime / 1440;
		}

		public int GetTime(int x)
		{
			return (x - this.X) * 1440 / this.Width;
		}

		private Rectangle GetRect()
		{
			int nWidth = this.GetNWidth();
			if (nWidth > 0)
			{
				this.rect.X = this.GetStartX();
				this.rect.Y = this.Y + 1;
				this.rect.Width = nWidth;
				this.rect.Height = this.Height - 2;
				return this.rect;
			}
			this.rect.X = -1;
			this.rect.Y = -1;
			this.rect.Width = 1;
			this.rect.Height = 1;
			return this.rect;
		}
	}
}
