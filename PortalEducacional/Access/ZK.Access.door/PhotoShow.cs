/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ZK.Access.door
{
	public class PhotoShow : Form
	{
		private DateTime m_date = DateTime.Now;

		private IContainer components = null;

		private PictureBox pic_photo;

		private Timer timer_hide;

		private PictureBox pic_close;

		public PhotoShow()
		{
			this.InitializeComponent();
			this.timer_hide.Enabled = false;
		}

		public void SetPhoto(Image photo)
		{
			this.pic_photo.Image = photo;
			this.timer_hide.Enabled = true;
			this.m_date = DateTime.Now;
		}

		private void timer_hide_Tick(object sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			if (this.m_date.AddSeconds(2.0) < now)
			{
				base.Hide();
				this.timer_hide.Enabled = false;
			}
		}

		private void PhotoShow_MouseEnter(object sender, EventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void PhotoShow_MouseMove(object sender, MouseEventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void pic_close_Click(object sender, EventArgs e)
		{
			base.Hide();
		}

		private void pic_photo_MouseHover(object sender, EventArgs e)
		{
			this.m_date = DateTime.Now;
		}

		private void PhotoShow_SizeChanged(object sender, EventArgs e)
		{
			this.pic_close.Location = new Point(base.Size.Width - this.pic_close.Width, 0);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PhotoShow));
			this.pic_photo = new PictureBox();
			this.timer_hide = new Timer(this.components);
			this.pic_close = new PictureBox();
			((ISupportInitialize)this.pic_photo).BeginInit();
			((ISupportInitialize)this.pic_close).BeginInit();
			base.SuspendLayout();
			this.pic_photo.Dock = DockStyle.Fill;
			this.pic_photo.Location = new Point(0, 0);
			this.pic_photo.Name = "pic_photo";
			this.pic_photo.Size = new Size(125, 135);
			this.pic_photo.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_photo.TabIndex = 0;
			this.pic_photo.TabStop = false;
			this.pic_photo.MouseEnter += this.PhotoShow_MouseEnter;
			this.pic_photo.MouseHover += this.pic_photo_MouseHover;
			this.pic_photo.MouseMove += this.PhotoShow_MouseMove;
			this.timer_hide.Interval = 2000;
			this.timer_hide.Tick += this.timer_hide_Tick;
			this.pic_close.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			this.pic_close.Image = (Image)componentResourceManager.GetObject("pic_close.Image");
			this.pic_close.Location = new Point(105, 0);
			this.pic_close.Name = "pic_close";
			this.pic_close.Size = new Size(20, 20);
			this.pic_close.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pic_close.TabIndex = 1;
			this.pic_close.TabStop = false;
			this.pic_close.Click += this.pic_close_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(194, 217, 247);
			base.ClientSize = new Size(125, 135);
			base.ControlBox = false;
			base.Controls.Add(this.pic_close);
			base.Controls.Add(this.pic_photo);
			base.FormBorderStyle = FormBorderStyle.None;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PhotoShow";
			base.ShowInTaskbar = false;
			this.Text = "人员相片";
			base.TopMost = true;
			base.SizeChanged += this.PhotoShow_SizeChanged;
			base.MouseEnter += this.PhotoShow_MouseEnter;
			base.MouseMove += this.PhotoShow_MouseMove;
			((ISupportInitialize)this.pic_photo).EndInit();
			((ISupportInitialize)this.pic_close).EndInit();
			base.ResumeLayout(false);
		}
	}
}
