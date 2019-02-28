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
using System.IO;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access.door
{
	public class MapEdit : Office2007Form
	{
		private int m_id = 0;

		private IContainer components = null;

		private TextBox txt_mapPath;

		private TextBox txt_name;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private Label lb_path;

		private Label lb_name;

		private ButtonX btn_openUrl;

		private OpenFileDialog openFileDialog1;

		private Label label2;

		public event EventHandler RefreshDataEvent = null;

		public MapEdit(int id)
		{
			this.InitializeComponent();
			this.m_id = id;
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			if (this.m_id > 0)
			{
				AccMap accMap = null;
				AccMapBll accMapBll = new AccMapBll(MainForm._ia);
				accMap = accMapBll.GetModel(this.m_id);
				if (accMap != null)
				{
					this.txt_name.Text = accMap.map_name;
					this.txt_mapPath.Text = accMap.map_path;
				}
			}
		}

		private void BindModel(AccMap map)
		{
			try
			{
				map.map_name = this.txt_name.Text;
				if (string.IsNullOrEmpty(map.map_path) || map.map_path != this.txt_mapPath.Text)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\maps");
					if (!directoryInfo.Exists)
					{
						directoryInfo.Create();
					}
					if (!string.IsNullOrEmpty(map.map_path) && File.Exists(map.map_path))
					{
						try
						{
							File.Delete(map.map_path);
						}
						catch
						{
						}
					}
					FileInfo fileInfo = new FileInfo(this.txt_mapPath.Text);
					object[] obj2 = new object[10]
					{
						directoryInfo.FullName,
						"\\",
						null,
						null,
						null,
						null,
						null,
						null,
						null,
						null
					};
					DateTime now = DateTime.Now;
					int num = now.Year;
					obj2[2] = num.ToString();
					now = DateTime.Now;
					num = now.Month;
					obj2[3] = num.ToString();
					now = DateTime.Now;
					num = now.Day;
					obj2[4] = num.ToString();
					now = DateTime.Now;
					num = now.Hour;
					obj2[5] = num.ToString();
					now = DateTime.Now;
					num = now.Minute;
					obj2[6] = num.ToString();
					now = DateTime.Now;
					obj2[7] = now.Second;
					obj2[8] = ".";
					obj2[9] = fileInfo.Extension;
					map.map_path = string.Concat(obj2);
					File.Copy(this.txt_mapPath.Text, map.map_path);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private bool check()
		{
			if (!string.IsNullOrEmpty(this.txt_name.Text))
			{
				if (!string.IsNullOrEmpty(this.txt_mapPath.Text) && File.Exists(this.txt_mapPath.Text))
				{
					try
					{
						Image image = Image.FromFile(this.txt_mapPath.Text);
						if (image.Width > 1856 || image.Height > 1392)
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMapIsOverWidthOrHeight", "地图超出规定大小，目前只支持最大分辨率为1856*121392的地图，为保证效果，请先把地图进行压缩处理"));
							return false;
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "目标文件不存在或损坏"));
						return false;
					}
					return true;
				}
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "目标文件不存在或损坏"));
				this.txt_mapPath.Focus();
				return false;
			}
			SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
			this.txt_name.Focus();
			return false;
		}

		private bool Save()
		{
			try
			{
				AccMap accMap = null;
				AccMapBll accMapBll = new AccMapBll(MainForm._ia);
				if (this.m_id > 0)
				{
					accMap = accMapBll.GetModel(this.m_id);
				}
				if (accMap == null)
				{
					accMap = new AccMap();
					if (accMapBll.Exists(this.txt_name.Text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
						this.txt_name.Focus();
						return false;
					}
				}
				else if (accMap.map_name != this.txt_name.Text && accMapBll.Exists(this.txt_name.Text))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNameExist", "名称已经存在"));
					this.txt_name.Focus();
					return false;
				}
				this.BindModel(accMap);
				if (this.m_id > 0)
				{
					if (accMapBll.Update(accMap))
					{
						if (this.RefreshDataEvent != null)
						{
							this.RefreshDataEvent(accMap, null);
						}
						return true;
					}
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
				}
				else
				{
					try
					{
						if (accMapBll.Add(accMap) > 0)
						{
							accMap.id = accMapBll.GetMaxId() - 1;
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(accMap, null);
							}
							this.txt_name.Text = "";
							return true;
						}
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
				}
			}
			catch
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
			}
			return false;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_openUrl_Click(object sender, EventArgs e)
		{
			this.openFileDialog1.Filter = "*.jpg;*.bmp;*.png;*.gif|*.jpg;*.bmp;*.png;*.gif";
			this.openFileDialog1.FileName = "";
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				this.txt_mapPath.Text = this.openFileDialog1.FileName;
			}
		}

		private void txt_name_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MapEdit));
			this.txt_mapPath = new TextBox();
			this.txt_name = new TextBox();
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.lb_path = new Label();
			this.lb_name = new Label();
			this.btn_openUrl = new ButtonX();
			this.openFileDialog1 = new OpenFileDialog();
			this.label2 = new Label();
			base.SuspendLayout();
			this.txt_mapPath.BackColor = SystemColors.Window;
			this.txt_mapPath.Location = new Point(96, 51);
			this.txt_mapPath.Name = "txt_mapPath";
			this.txt_mapPath.ReadOnly = true;
			this.txt_mapPath.Size = new Size(232, 21);
			this.txt_mapPath.TabIndex = 1;
			this.txt_name.Location = new Point(96, 17);
			this.txt_name.Name = "txt_name";
			this.txt_name.Size = new Size(232, 21);
			this.txt_name.TabIndex = 0;
			this.txt_name.KeyPress += this.txt_name_KeyPress;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(340, 96);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 4;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(246, 96);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.lb_path.Location = new Point(12, 55);
			this.lb_path.Name = "lb_path";
			this.lb_path.Size = new Size(78, 12);
			this.lb_path.TabIndex = 47;
			this.lb_path.Text = "地图路径";
			this.lb_path.TextAlign = ContentAlignment.MiddleLeft;
			this.lb_name.Location = new Point(12, 20);
			this.lb_name.Name = "lb_name";
			this.lb_name.Size = new Size(78, 12);
			this.lb_name.TabIndex = 46;
			this.lb_name.Text = "地图名称";
			this.lb_name.TextAlign = ContentAlignment.MiddleLeft;
			this.btn_openUrl.AccessibleRole = AccessibleRole.PushButton;
			this.btn_openUrl.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_openUrl.Location = new Point(340, 51);
			this.btn_openUrl.Name = "btn_openUrl";
			this.btn_openUrl.Size = new Size(82, 23);
			this.btn_openUrl.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_openUrl.TabIndex = 2;
			this.btn_openUrl.Text = "浏览";
			this.btn_openUrl.Click += this.btn_openUrl_Click;
			this.openFileDialog1.FileName = "openFileDialog1";
			this.label2.AutoSize = true;
			this.label2.BackColor = Color.Transparent;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(335, 20);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 48;
			this.label2.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(434, 131);
			base.Controls.Add(this.txt_mapPath);
			base.Controls.Add(this.txt_name);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.btn_openUrl);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lb_path);
			base.Controls.Add(this.lb_name);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MapEdit";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "地图设置";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
