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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ZK.ConfigManager;

namespace ZK.Access.door
{
	public class MapFullScreen : Office2007Form
	{
		private Control m_map = null;

		private Control m_mapParent = null;

		private int m_dbcount = 0;

		private IContainer components = null;

		private Panel p_main;

		private Timer timer1;

		public MapFullScreen(Control map)
		{
			this.InitializeComponent();
			if (map != null)
			{
				this.m_map = map;
				this.m_map.Height = this.p_main.Height - 2 * this.m_map.Top - 5;
				this.m_map.Width = this.p_main.Width - 2 * this.m_map.Left - 5;
				this.m_mapParent = map.Parent;
				map.Parent = this.p_main;
				map.DoubleClick -= this.map_DoubleClick;
				map.DoubleClick += this.map_DoubleClick;
				initLang.LocaleForm(this, base.Name);
				DevMapControl devMapControl = map as DevMapControl;
				devMapControl?.ReSet();
				base.MaximizeBox = true;
			}
		}

		private void MapFullScreen_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.m_map != null)
			{
				this.m_map.DoubleClick -= this.map_DoubleClick;
				this.m_map.Parent = this.m_mapParent;
				this.m_map.Width = this.m_mapParent.Width - 20;
				this.m_map.Height = this.m_mapParent.Height - 20;
			}
		}

		private void map_DoubleClick(object sender, EventArgs e)
		{
			this.m_dbcount++;
			if (this.m_dbcount == 2)
			{
				base.Close();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.timer1.Enabled = false;
			DevMapControl devMapControl = this.m_map as DevMapControl;
			devMapControl?.ReLayout();
		}

		private void p_main_SizeChanged(object sender, EventArgs e)
		{
			if (this.m_map != null)
			{
				if (this.m_map.Height < this.p_main.Height - 2 * this.m_map.Top - 5)
				{
					this.m_map.Height = this.p_main.Height - 2 * this.m_map.Top - 5;
				}
				if (this.m_map.Width < this.p_main.Width - 2 * this.m_map.Left - 5)
				{
					this.m_map.Width = this.p_main.Width - 2 * this.m_map.Left - 5;
				}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MapFullScreen));
			this.p_main = new Panel();
			this.timer1 = new Timer(this.components);
			base.SuspendLayout();
			this.p_main.AutoScroll = true;
			this.p_main.BackColor = Color.White;
			this.p_main.Dock = DockStyle.Fill;
			this.p_main.Location = new Point(0, 0);
			this.p_main.Name = "p_main";
			this.p_main.Size = new Size(1018, 756);
			this.p_main.TabIndex = 0;
			this.p_main.SizeChanged += this.p_main_SizeChanged;
			this.timer1.Enabled = true;
			this.timer1.Tick += this.timer1_Tick;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(1018, 756);
			base.Controls.Add(this.p_main);
			base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "MapFullScreen";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "电子地图";
			base.WindowState = FormWindowState.Maximized;
			base.FormClosing += this.MapFullScreen_FormClosing;
			base.ResumeLayout(false);
		}
	}
}
