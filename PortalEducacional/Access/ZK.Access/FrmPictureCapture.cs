/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using AForge.Video;
using AForge.Video.DirectShow;
using DevComponents.DotNetBar;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ZK.Access
{
	public class FrmPictureCapture : Office2007Form
	{
		public delegate void OnCaptureEvent(Image img);

		private VideoCaptureDevice videoSource;

		private bool _hasCam = false;

		private IContainer components = null;

		private PictureBox pictureBox1;

		private ButtonX btn_cancel;

		private ButtonX buttonX1;

		public event OnCaptureEvent OnCapture;

		public FrmPictureCapture()
		{
			this.InitializeComponent();
			FilterInfoCollection filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			this._hasCam = false;
			if (filterInfoCollection == null)
			{
				MessageBox.Show("Não é possível iniciar a webcam.");
			}
			else if (filterInfoCollection.Count <= 0)
			{
				MessageBox.Show("Não é possível iniciar a webcam.");
			}
			else
			{
				this._hasCam = true;
				this.videoSource = new VideoCaptureDevice(filterInfoCollection[0].MonikerString);
				this.videoSource.NewFrame += this.videoSource_NewFrame;
				this.videoSource.Start();
			}
		}

		private void videoSource_NewFrame(object sender, NewFrameEventArgs e)
		{
			this.pictureBox1.Image = (Bitmap)e.Frame.Clone();
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			if (this._hasCam)
			{
				if (this.OnCapture != null)
				{
					this.OnCapture(this.pictureBox1.Image);
				}
				base.Close();
			}
		}

		private void FrmPictureCapture_FormClosed(object sender, FormClosedEventArgs e)
		{
			base.OnClosed(e);
			if (this.videoSource != null && this.videoSource.IsRunning)
			{
				this.videoSource.SignalToStop();
				this.videoSource = null;
			}
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			base.Close();
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
			this.pictureBox1 = new PictureBox();
			this.btn_cancel = new ButtonX();
			this.buttonX1 = new ButtonX();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.pictureBox1.Location = new Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new Size(329, 218);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(162, 235);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(85, 25);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 10;
			this.btn_cancel.Text = "OK";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.buttonX1.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX1.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX1.Location = new Point(253, 235);
			this.buttonX1.Name = "buttonX1";
			this.buttonX1.Size = new Size(88, 25);
			this.buttonX1.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX1.TabIndex = 11;
			this.buttonX1.Text = "Cancelar";
			this.buttonX1.Click += this.buttonX1_Click;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(353, 272);
			base.Controls.Add(this.buttonX1);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.pictureBox1);
			base.Name = "FrmPictureCapture";
			base.StartPosition = FormStartPosition.CenterScreen;
			base.FormClosed += this.FrmPictureCapture_FormClosed;
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
		}
	}
}
