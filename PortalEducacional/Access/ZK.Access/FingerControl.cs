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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ZK.Access
{
	public class FingerControl : UserControl
	{
		private struct OneLine
		{
			public Point pt1;

			public Point pt2;

			public OneLine(Point p1, Point p2)
			{
				this.pt1 = p1;
				this.pt2 = p2;
			}
		}

		private int _LineWidth;

		private int _FingerTopWidth;

		private int _FingerBottomWidth;

		private int _FingerHeight;

		private int _Angle;

		private int _ShiftRight;

		private int _ShiftBottom;

		private Pen _LinePen;

		private Color _lineColor;

		private Color _FingerColor;

		private GraphicsPath FpPath;

		private SolidBrush FingerBrush;

		private List<OneLine> Lines;

		private IContainer components = null;

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
				this._LinePen = new Pen(this._lineColor, (float)this._LineWidth);
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
				this._LinePen = new Pen(this._lineColor, (float)this._LineWidth);
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
		[Description("手指的填充颜色")]
		[DefaultValue(typeof(Color), "176, 196, 222")]
		public Color FingerColor
		{
			get
			{
				return this._FingerColor;
			}
			set
			{
				this._FingerColor = value;
				this.FingerBrush = new SolidBrush(this._FingerColor);
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
		[Description("手指尖的大小")]
		[DefaultValue(17)]
		public int FingerTopWidth
		{
			get
			{
				return this._FingerTopWidth;
			}
			set
			{
				this._FingerTopWidth = ((value > this._FingerBottomWidth) ? this._FingerBottomWidth : value);
				this.OnGenerateFinger();
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
		[Description("手指底部的大小")]
		[DefaultValue(22)]
		public int FingerBottomWidth
		{
			get
			{
				return this._FingerBottomWidth;
			}
			set
			{
				this._FingerBottomWidth = ((value < this._FingerTopWidth) ? this._FingerTopWidth : value);
				this.OnGenerateFinger();
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
		[Description("手指高度")]
		[DefaultValue(60)]
		public int FingerHeight
		{
			get
			{
				return this._FingerHeight;
			}
			set
			{
				int num = (this._FingerBottomWidth + this._FingerTopWidth) / 2;
				this._FingerHeight = ((value < num) ? num : value);
				this.OnGenerateFinger();
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
		[Description("手指倾斜角度")]
		[DefaultValue(0)]
		public int Angle
		{
			get
			{
				return this._Angle;
			}
			set
			{
				this._Angle = value;
				this.OnGenerateFinger();
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
		[Description("手指右平移像素")]
		[DefaultValue(0)]
		public int ShiftRight
		{
			get
			{
				return this._ShiftRight;
			}
			set
			{
				this._ShiftRight = value;
				this.OnGenerateFinger();
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
		[Description("手指下平移像素")]
		[DefaultValue(0)]
		public int ShiftBottom
		{
			get
			{
				return this._ShiftBottom;
			}
			set
			{
				this._ShiftBottom = value;
				this.OnGenerateFinger();
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

		public FingerControl()
		{
			this.InitializeComponent();
			this.BackColor = Color.Transparent;
			this._Angle = 0;
			this._LineWidth = 1;
			this._FingerColor = this.BackColor;
			this._lineColor = Color.FromArgb(220, 220, 220);
			this._LinePen = new Pen(this._lineColor, (float)this._LineWidth);
			this.FingerBrush = new SolidBrush(this._FingerColor);
			this._FingerTopWidth = 17;
			this._FingerBottomWidth = 22;
			this._FingerHeight = 60;
			this._ShiftRight = 0;
			this._ShiftBottom = 0;
			this.OnGenerateFinger();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			base.Region = new Region(this.FpPath);
			e.Graphics.FillPath(this.FingerBrush, this.FpPath);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			e.Graphics.DrawPath(this._LinePen, this.FpPath);
			e.Graphics.DrawPath(this._LinePen, this.FpPath);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
		}

		protected virtual void OnGenerateFinger()
		{
			this.FpPath = new GraphicsPath();
			this.Lines = new List<OneLine>();
			int num = (this._FingerBottomWidth - this._FingerTopWidth) / 2;
			int x = this._FingerBottomWidth - num;
			int x2 = 0;
			int fingerBottomWidth = this._FingerBottomWidth;
			int num2 = this._FingerTopWidth / 2;
			int y = this._FingerHeight + num2 - this._FingerBottomWidth / 2;
			Point center = new Point(this._FingerBottomWidth / 2, this._FingerHeight + num2);
			Point point = this.PointRotate(center, new Point(num, num2), this._Angle);
			Point point2 = this.PointRotate(center, new Point(x2, y), this._Angle);
			Point point3 = this.PointRotate(center, new Point(x, num2), this._Angle);
			Point point4 = this.PointRotate(center, new Point(fingerBottomWidth, y), this._Angle);
			int num3 = this.CaclDistance(point, point3);
			int num4 = this.CaclDistance(point2, point4);
			this.FpPath.AddLine(point2, point);
			this.FpPath.AddArc(this.GetArcRectangle(point, point3), (float)(-180 + this._Angle), (float)(180 - Math.Abs(this._Angle)));
			this.FpPath.AddLine(point3, point4);
			this.FpPath.AddArc(this.GetArcRectangle(point2, point4), (float)this._Angle, (float)(180 - Math.Abs(this._Angle)));
			this.FpPath.CloseAllFigures();
			this.Lines.Add(new OneLine(this.PointRotate(center, point2, this._Angle), this.PointRotate(center, point, this._Angle)));
			this.Lines.Add(new OneLine(this.PointRotate(center, point3, this._Angle), this.PointRotate(center, point4, this._Angle)));
		}

		private Point PointRotate(Point center, Point p1, int angle)
		{
			double num = (double)angle * 3.1415926535897931 / 180.0;
			double num2 = (double)(p1.X - center.X) * Math.Cos(num) - (double)(p1.Y - center.Y) * Math.Sin(num) + (double)center.X + (double)this._ShiftRight;
			double num3 = (double)(p1.Y - center.Y) * Math.Cos(num) + (double)(p1.X - center.X) * Math.Sin(num) + (double)center.Y + (double)this._ShiftBottom;
			return new Point((int)num2, (int)num3);
		}

		private int CaclDistance(Point p1, Point p2)
		{
			double x = (double)(p2.X - p1.X);
			double x2 = (double)(p2.Y - p1.Y);
			double num = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(x2, 2.0));
			return (int)num;
		}

		private Rectangle GetArcRectangle(Point ptLeft, Point ptRight)
		{
			int num = (ptLeft.X + ptRight.X) / 2;
			int num2 = (ptLeft.Y + ptRight.Y) / 2;
			int num3 = this.CaclDistance(ptLeft, ptRight);
			Point location = new Point(ptLeft.X, num2 - num3 / 2);
			return new Rectangle(location, new Size(num3, num3));
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
			base.AutoScaleMode = AutoScaleMode.Font;
		}
	}
}
