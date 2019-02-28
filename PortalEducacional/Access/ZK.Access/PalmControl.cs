/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ZK.Access
{
	public class PalmControl : UserControl
	{
		private int _LineWidth;

		private int _PalmTopWidth;

		private int _PalmBottomWidth;

		private int _PalmHeight;

		private string _Text;

		private Pen _LinePen;

		private Color _lineColor;

		private Color _TextColor;

		private SolidBrush _TextBrush;

		private GraphicsPath _PalmPath;

		private IContainer components = null;

		[Category("Custom")]
		[Description("绘制手掌的线条颜色")]
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
				this.Refresh();
			}
		}

		[Category("Custom")]
		[Description("绘制手掌的线条大小")]
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
				this.Refresh();
			}
		}

		[Category("Custom")]
		[Description("绘制手掌的顶部宽度")]
		[DefaultValue(100)]
		public int PalmTopWidth
		{
			get
			{
				return this._PalmTopWidth;
			}
			set
			{
				this._PalmTopWidth = ((value < this._PalmBottomWidth) ? this._PalmBottomWidth : value);
				this.OnGeneratePath();
				this.Refresh();
			}
		}

		[Category("Custom")]
		[Description("绘制手掌的底部宽度")]
		[DefaultValue(80)]
		public int PalmBottomWidth
		{
			get
			{
				return this._PalmBottomWidth;
			}
			set
			{
				this._PalmBottomWidth = ((value > this._PalmTopWidth) ? this._PalmTopWidth : value);
				this.OnGeneratePath();
				this.Refresh();
			}
		}

		[Category("Custom")]
		[Description("绘制手掌的高度")]
		[DefaultValue(100)]
		public int PalmHeight
		{
			get
			{
				return this._PalmHeight;
			}
			set
			{
				this._PalmHeight = value;
				this.OnGeneratePath();
				this.Refresh();
			}
		}

		[Category("Custom")]
		[Description("显示在掌心的文字")]
		[DefaultValue("Palm")]
		[Browsable(true)]
		public new string Text
		{
			get
			{
				return (this._Text == null) ? "" : this._Text;
			}
			set
			{
				this._Text = ((value == null) ? "" : value);
				this.Refresh();
			}
		}

		[Category("Custom")]
		[Description("掌心的文字的颜色")]
		[DefaultValue(typeof(Color), "65, 98, 149")]
		public Color TextColor
		{
			get
			{
				return this._TextColor;
			}
			set
			{
				this._TextColor = value;
				this._TextBrush = new SolidBrush(this._TextColor);
				this.Refresh();
			}
		}

		public PalmControl()
		{
			this.InitializeComponent();
			this.BackColor = Color.Transparent;
			this._LineWidth = 1;
			this._lineColor = Color.FromArgb(220, 220, 220);
			this._TextColor = Color.FromArgb(65, 98, 149);
			this._LinePen = new Pen(this._lineColor, (float)this._LineWidth);
			this._TextBrush = new SolidBrush(this._TextColor);
			this._PalmTopWidth = 100;
			this._PalmBottomWidth = 80;
			this._PalmHeight = 100;
			this._Text = "Palm";
			this.OnGeneratePath();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			base.Region = new Region(this._PalmPath);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			e.Graphics.DrawPath(this._LinePen, this._PalmPath);
			e.Graphics.DrawPath(this._LinePen, this._PalmPath);
			if (this._Text != null && this._Text.Trim() != "")
			{
				int num = this._PalmTopWidth / 2;
				int num2 = this._PalmHeight / 2;
				int maxTextLength = this.GetMaxTextLength();
				PointF point = new PointF((float)(num - maxTextLength / 2), (float)num2);
				e.Graphics.DrawString(this._Text, this.Font, this._TextBrush, point);
			}
		}

		private int GetMaxTextLength()
		{
			return (int)base.CreateGraphics().MeasureString(this._Text, this.Font).Width;
		}

		private void OnGeneratePath()
		{
			this._PalmPath = new GraphicsPath();
			int num = this._PalmTopWidth / 3;
			int num2 = (this._PalmTopWidth - this._PalmBottomWidth) / 2;
			Point pt = new Point(0, num);
			Point pt2 = new Point(this._PalmTopWidth, num);
			Point point = new Point(num2, this._PalmHeight + num);
			Point point2 = new Point(num2 + this._PalmBottomWidth, this._PalmHeight + num);
			this._PalmPath.AddLine(point, pt);
			this._PalmPath.AddArc(new Rectangle(new Point(0, 0), new Size(this._PalmTopWidth, num)), -180f, 180f);
			this._PalmPath.AddLine(pt2, point2);
			this._PalmPath.AddLine(point2, point);
			this._PalmPath.CloseAllFigures();
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
