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
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class wiegand_add : Office2007Form
	{
		private int m_id = 0;

		private IContainer components = null;

		private TextBox txt_wiegan_name;

		private Label lbl_wiegan_name;

		private TextBox txt_wiegand_count;

		private Label lbl_wiegand_count;

		private TextBox txt_odd_start;

		private Label lbl_odd_start;

		private TextBox txt_odd_count;

		private Label lbl_odd_count;

		private TextBox txt_even_count;

		private Label lbl_even_cout;

		private TextBox txt_even_start;

		private Label lbl_even_start;

		private TextBox txt_cid_count;

		private Label lbl_cid_count;

		private TextBox txt_cid_start;

		private Label lbl_cid_start;

		private TextBox txt_company_count;

		private Label lbl_company_count;

		private TextBox txt_company_start;

		private Label lbl_company_start;

		private ButtonX btn_wiegand_add_canncel;

		private ButtonX btn_wiegand_add_ok;

		private Label label1;

		private Label label2;

		private Label label5;

		private Label label9;

		public event EventHandler refreshDataEvent = null;

		public wiegand_add()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
		}

		public wiegand_add(int id)
			: this()
		{
			this.m_id = id;
			this.BindData();
		}

		private void BindData()
		{
			if (this.m_id > 0)
			{
				AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
				List<AccWiegandfmt> list = null;
				list = accWiegandfmtBll.GetModelList(" status =0 and id = " + this.m_id);
				if (list != null && list.Count > 0)
				{
					this.Mode2Text(list[0]);
				}
				this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
			}
			else
			{
				this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
			}
		}

		private void Mode2Text(AccWiegandfmt model)
		{
			if (model != null)
			{
				this.txt_wiegan_name.Text = model.wiegand_name;
				int num;
				if (model.wiegand_count > 0)
				{
					TextBox textBox = this.txt_wiegand_count;
					num = model.wiegand_count;
					textBox.Text = num.ToString();
				}
				if (model.odd_start > 0)
				{
					TextBox textBox2 = this.txt_odd_start;
					num = model.odd_start;
					textBox2.Text = num.ToString();
				}
				if (model.odd_count > 0)
				{
					TextBox textBox3 = this.txt_odd_count;
					num = model.odd_count;
					textBox3.Text = num.ToString();
				}
				if (model.even_start > 0)
				{
					TextBox textBox4 = this.txt_even_start;
					num = model.even_start;
					textBox4.Text = num.ToString();
				}
				if (model.even_count > 0)
				{
					TextBox textBox5 = this.txt_even_count;
					num = model.even_count;
					textBox5.Text = num.ToString();
				}
				if (model.cid_start > 0)
				{
					TextBox textBox6 = this.txt_cid_start;
					num = model.cid_start;
					textBox6.Text = num.ToString();
				}
				if (model.cid_count > 0)
				{
					TextBox textBox7 = this.txt_cid_count;
					num = model.cid_count;
					textBox7.Text = num.ToString();
				}
				if (model.comp_start > 0)
				{
					TextBox textBox8 = this.txt_company_start;
					num = model.comp_start;
					textBox8.Text = num.ToString();
				}
				if (model.comp_count > 0)
				{
					TextBox textBox9 = this.txt_company_count;
					num = model.comp_count;
					textBox9.Text = num.ToString();
				}
			}
		}

		private AccWiegandfmt Text2Model()
		{
			AccWiegandfmt accWiegandfmt = new AccWiegandfmt();
			accWiegandfmt.id = this.m_id;
			accWiegandfmt.wiegand_name = this.txt_wiegan_name.Text;
			if (!string.IsNullOrEmpty(this.txt_wiegand_count.Text))
			{
				accWiegandfmt.wiegand_count = int.Parse(this.txt_wiegand_count.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_odd_start.Text))
			{
				accWiegandfmt.odd_start = int.Parse(this.txt_odd_start.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_odd_count.Text))
			{
				accWiegandfmt.odd_count = int.Parse(this.txt_odd_count.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_even_start.Text))
			{
				accWiegandfmt.even_start = int.Parse(this.txt_even_start.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_even_count.Text))
			{
				accWiegandfmt.even_count = int.Parse(this.txt_even_count.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_cid_start.Text))
			{
				accWiegandfmt.cid_start = int.Parse(this.txt_cid_start.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_cid_count.Text))
			{
				accWiegandfmt.cid_count = int.Parse(this.txt_cid_count.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_company_start.Text))
			{
				accWiegandfmt.comp_start = int.Parse(this.txt_company_start.Text);
			}
			if (!string.IsNullOrEmpty(this.txt_company_count.Text))
			{
				accWiegandfmt.comp_count = int.Parse(this.txt_company_count.Text);
			}
			return accWiegandfmt;
		}

		private bool check()
		{
			if (this.txt_wiegan_name.Text.Trim() == "")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandNameIsNull", "韦根名称不能空"));
				this.txt_wiegan_name.Focus();
				return false;
			}
			AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
			List<AccWiegandfmt> modelList = accWiegandfmtBll.GetModelList($"id<>{this.m_id} and wiegand_name='{this.txt_wiegan_name.Text.Trim()}'");
			if (modelList != null && modelList.Count > 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandNameExists", "韦根名称已存在"));
				this.txt_wiegan_name.Focus();
				return false;
			}
			if (this.txt_wiegand_count.Text.Trim() == "")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandTotalIsNull", "总长度不能空"));
				this.txt_wiegand_count.Focus();
				return false;
			}
			if (this.txt_odd_count.Text.Trim() != "" && this.txt_even_count.Text.Trim() != "" && int.Parse(this.txt_odd_count.Text.Trim()) + int.Parse(this.txt_even_count.Text.Trim()) > int.Parse(this.txt_wiegand_count.Text.Trim()))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandOdd_EvenMorethen", "奇偶校验位长度之和不能大于总长度"));
				this.txt_wiegand_count.Focus();
				return false;
			}
			if (this.txt_cid_start.Text.Trim() == "")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandCidStartIsNull", "卡号开始位不能空"));
				this.txt_cid_start.Focus();
				return false;
			}
			if (this.txt_cid_count.Text.Trim() == "")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandCidCountIsNull", "卡号位长度不能空"));
				this.txt_cid_count.Focus();
				return false;
			}
			if (this.txt_cid_count.Text.Trim() != "" && this.txt_company_count.Text.Trim() != "" && int.Parse(this.txt_cid_count.Text.Trim()) + int.Parse(this.txt_company_count.Text.Trim()) > int.Parse(this.txt_wiegand_count.Text.Trim()))
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandCid_CompanyMorethen", "卡号位公司码长度之和不能大于总长度"));
				this.txt_wiegand_count.Focus();
				return false;
			}
			return true;
		}

		private void btn_wiegand_add_ok_Click_1(object sender, EventArgs e)
		{
			if (this.check())
			{
				AccWiegandfmt accWiegandfmt = null;
				accWiegandfmt = this.Text2Model();
				if (this.Save(accWiegandfmt))
				{
					base.Close();
				}
			}
		}

		private void txt_wiegand_count_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 80L);
		}

		private void txt_odd_start_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 1, 999999999L);
		}

		private void btn_wiegand_add_canncel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private bool Save(AccWiegandfmt model)
		{
			try
			{
				if (model != null)
				{
					AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
					if (this.m_id > 0)
					{
						if (accWiegandfmtBll.Update(model) && this.refreshDataEvent != null)
						{
							this.refreshDataEvent(model, null);
						}
					}
					else if (accWiegandfmtBll.Add(model) > 0 && this.refreshDataEvent != null)
					{
						this.refreshDataEvent(model, null);
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		private void txt_odd_count_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (this.txt_wiegand_count.Text.Trim() == "")
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SwiegandTotalIsNull", "总长度不能空"));
				this.txt_wiegand_count.Focus();
				e.Handled = true;
			}
			else
			{
				CheckInfo.NumberKeyPress(sender, e, 1, int.Parse(this.txt_wiegand_count.Text));
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
			this.txt_wiegan_name = new TextBox();
			this.lbl_wiegan_name = new Label();
			this.txt_wiegand_count = new TextBox();
			this.lbl_wiegand_count = new Label();
			this.txt_odd_start = new TextBox();
			this.lbl_odd_start = new Label();
			this.txt_odd_count = new TextBox();
			this.lbl_odd_count = new Label();
			this.txt_even_count = new TextBox();
			this.lbl_even_cout = new Label();
			this.txt_even_start = new TextBox();
			this.lbl_even_start = new Label();
			this.txt_cid_count = new TextBox();
			this.lbl_cid_count = new Label();
			this.txt_cid_start = new TextBox();
			this.lbl_cid_start = new Label();
			this.txt_company_count = new TextBox();
			this.lbl_company_count = new Label();
			this.txt_company_start = new TextBox();
			this.lbl_company_start = new Label();
			this.btn_wiegand_add_canncel = new ButtonX();
			this.btn_wiegand_add_ok = new ButtonX();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label5 = new Label();
			this.label9 = new Label();
			base.SuspendLayout();
			this.txt_wiegan_name.Location = new Point(194, 13);
			this.txt_wiegan_name.Name = "txt_wiegan_name";
			this.txt_wiegan_name.Size = new Size(221, 21);
			this.txt_wiegan_name.TabIndex = 46;
			this.lbl_wiegan_name.BackColor = Color.Transparent;
			this.lbl_wiegan_name.Location = new Point(10, 16);
			this.lbl_wiegan_name.Name = "lbl_wiegan_name";
			this.lbl_wiegan_name.RightToLeft = RightToLeft.Yes;
			this.lbl_wiegan_name.Size = new Size(99, 12);
			this.lbl_wiegan_name.TabIndex = 45;
			this.lbl_wiegan_name.Text = "名称";
			this.lbl_wiegan_name.TextAlign = ContentAlignment.TopRight;
			this.txt_wiegand_count.Location = new Point(194, 39);
			this.txt_wiegand_count.Name = "txt_wiegand_count";
			this.txt_wiegand_count.Size = new Size(54, 21);
			this.txt_wiegand_count.TabIndex = 48;
			this.txt_wiegand_count.KeyPress += this.txt_wiegand_count_KeyPress;
			this.lbl_wiegand_count.BackColor = Color.Transparent;
			this.lbl_wiegand_count.Location = new Point(9, 43);
			this.lbl_wiegand_count.Name = "lbl_wiegand_count";
			this.lbl_wiegand_count.RightToLeft = RightToLeft.Yes;
			this.lbl_wiegand_count.Size = new Size(179, 12);
			this.lbl_wiegand_count.TabIndex = 47;
			this.lbl_wiegand_count.Text = "总长度";
			this.lbl_wiegand_count.TextAlign = ContentAlignment.TopRight;
			this.txt_odd_start.Location = new Point(194, 66);
			this.txt_odd_start.Name = "txt_odd_start";
			this.txt_odd_start.Size = new Size(54, 21);
			this.txt_odd_start.TabIndex = 50;
			this.txt_odd_start.KeyPress += this.txt_odd_start_KeyPress;
			this.lbl_odd_start.BackColor = Color.Transparent;
			this.lbl_odd_start.Location = new Point(10, 69);
			this.lbl_odd_start.Name = "lbl_odd_start";
			this.lbl_odd_start.RightToLeft = RightToLeft.Yes;
			this.lbl_odd_start.Size = new Size(178, 12);
			this.lbl_odd_start.TabIndex = 49;
			this.lbl_odd_start.Text = "奇校验开始位";
			this.lbl_odd_start.TextAlign = ContentAlignment.TopRight;
			this.txt_odd_count.Location = new Point(374, 66);
			this.txt_odd_count.Name = "txt_odd_count";
			this.txt_odd_count.Size = new Size(41, 21);
			this.txt_odd_count.TabIndex = 52;
			this.txt_odd_count.KeyPress += this.txt_odd_count_KeyPress;
			this.lbl_odd_count.BackColor = Color.Transparent;
			this.lbl_odd_count.Location = new Point(299, 69);
			this.lbl_odd_count.Name = "lbl_odd_count";
			this.lbl_odd_count.RightToLeft = RightToLeft.Yes;
			this.lbl_odd_count.Size = new Size(69, 12);
			this.lbl_odd_count.TabIndex = 51;
			this.lbl_odd_count.Text = "长度";
			this.lbl_odd_count.TextAlign = ContentAlignment.TopRight;
			this.txt_even_count.Location = new Point(374, 93);
			this.txt_even_count.Name = "txt_even_count";
			this.txt_even_count.Size = new Size(41, 21);
			this.txt_even_count.TabIndex = 56;
			this.txt_even_count.KeyPress += this.txt_odd_count_KeyPress;
			this.lbl_even_cout.BackColor = Color.Transparent;
			this.lbl_even_cout.Location = new Point(299, 96);
			this.lbl_even_cout.Name = "lbl_even_cout";
			this.lbl_even_cout.RightToLeft = RightToLeft.Yes;
			this.lbl_even_cout.Size = new Size(69, 12);
			this.lbl_even_cout.TabIndex = 55;
			this.lbl_even_cout.Text = "长度";
			this.lbl_even_cout.TextAlign = ContentAlignment.TopRight;
			this.txt_even_start.Location = new Point(194, 93);
			this.txt_even_start.Name = "txt_even_start";
			this.txt_even_start.Size = new Size(54, 21);
			this.txt_even_start.TabIndex = 54;
			this.txt_even_start.KeyPress += this.txt_odd_start_KeyPress;
			this.lbl_even_start.BackColor = Color.Transparent;
			this.lbl_even_start.Location = new Point(10, 96);
			this.lbl_even_start.Name = "lbl_even_start";
			this.lbl_even_start.RightToLeft = RightToLeft.Yes;
			this.lbl_even_start.Size = new Size(178, 12);
			this.lbl_even_start.TabIndex = 53;
			this.lbl_even_start.Text = "偶校验开始位";
			this.lbl_even_start.TextAlign = ContentAlignment.TopRight;
			this.txt_cid_count.Location = new Point(374, 120);
			this.txt_cid_count.Name = "txt_cid_count";
			this.txt_cid_count.Size = new Size(41, 21);
			this.txt_cid_count.TabIndex = 60;
			this.txt_cid_count.KeyPress += this.txt_odd_count_KeyPress;
			this.lbl_cid_count.BackColor = Color.Transparent;
			this.lbl_cid_count.Location = new Point(299, 123);
			this.lbl_cid_count.Name = "lbl_cid_count";
			this.lbl_cid_count.RightToLeft = RightToLeft.Yes;
			this.lbl_cid_count.Size = new Size(69, 12);
			this.lbl_cid_count.TabIndex = 59;
			this.lbl_cid_count.Text = "长度";
			this.lbl_cid_count.TextAlign = ContentAlignment.TopRight;
			this.txt_cid_start.Location = new Point(194, 120);
			this.txt_cid_start.Name = "txt_cid_start";
			this.txt_cid_start.Size = new Size(54, 21);
			this.txt_cid_start.TabIndex = 58;
			this.txt_cid_start.KeyPress += this.txt_odd_start_KeyPress;
			this.lbl_cid_start.BackColor = Color.Transparent;
			this.lbl_cid_start.Location = new Point(10, 123);
			this.lbl_cid_start.Name = "lbl_cid_start";
			this.lbl_cid_start.RightToLeft = RightToLeft.Yes;
			this.lbl_cid_start.Size = new Size(178, 12);
			this.lbl_cid_start.TabIndex = 57;
			this.lbl_cid_start.Text = "卡号开始位";
			this.lbl_cid_start.TextAlign = ContentAlignment.TopRight;
			this.txt_company_count.Location = new Point(374, 147);
			this.txt_company_count.Name = "txt_company_count";
			this.txt_company_count.Size = new Size(41, 21);
			this.txt_company_count.TabIndex = 64;
			this.txt_company_count.KeyPress += this.txt_odd_count_KeyPress;
			this.lbl_company_count.BackColor = Color.Transparent;
			this.lbl_company_count.Location = new Point(299, 150);
			this.lbl_company_count.Name = "lbl_company_count";
			this.lbl_company_count.RightToLeft = RightToLeft.Yes;
			this.lbl_company_count.Size = new Size(69, 12);
			this.lbl_company_count.TabIndex = 63;
			this.lbl_company_count.Text = "长度";
			this.lbl_company_count.TextAlign = ContentAlignment.TopRight;
			this.txt_company_start.Location = new Point(194, 147);
			this.txt_company_start.Name = "txt_company_start";
			this.txt_company_start.Size = new Size(54, 21);
			this.txt_company_start.TabIndex = 62;
			this.txt_company_start.KeyPress += this.txt_odd_start_KeyPress;
			this.lbl_company_start.BackColor = Color.Transparent;
			this.lbl_company_start.Location = new Point(10, 150);
			this.lbl_company_start.Name = "lbl_company_start";
			this.lbl_company_start.RightToLeft = RightToLeft.Yes;
			this.lbl_company_start.Size = new Size(178, 12);
			this.lbl_company_start.TabIndex = 61;
			this.lbl_company_start.Text = "公司码开始位";
			this.lbl_company_start.TextAlign = ContentAlignment.TopRight;
			this.btn_wiegand_add_canncel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_wiegand_add_canncel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_wiegand_add_canncel.Location = new Point(332, 181);
			this.btn_wiegand_add_canncel.Name = "btn_wiegand_add_canncel";
			this.btn_wiegand_add_canncel.Size = new Size(82, 23);
			this.btn_wiegand_add_canncel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_wiegand_add_canncel.TabIndex = 66;
			this.btn_wiegand_add_canncel.Text = "取消";
			this.btn_wiegand_add_canncel.Click += this.btn_wiegand_add_canncel_Click;
			this.btn_wiegand_add_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_wiegand_add_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_wiegand_add_ok.Location = new Point(230, 181);
			this.btn_wiegand_add_ok.Name = "btn_wiegand_add_ok";
			this.btn_wiegand_add_ok.Size = new Size(82, 23);
			this.btn_wiegand_add_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_wiegand_add_ok.TabIndex = 65;
			this.btn_wiegand_add_ok.Text = "确定";
			this.btn_wiegand_add_ok.Click += this.btn_wiegand_add_ok_Click_1;
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(420, 16);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 67;
			this.label1.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(253, 42);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 68;
			this.label2.Text = "*";
			this.label5.AutoSize = true;
			this.label5.ForeColor = Color.Red;
			this.label5.Location = new Point(253, 123);
			this.label5.Name = "label5";
			this.label5.Size = new Size(11, 12);
			this.label5.TabIndex = 71;
			this.label5.Text = "*";
			this.label9.AutoSize = true;
			this.label9.ForeColor = Color.Red;
			this.label9.Location = new Point(421, 123);
			this.label9.Name = "label9";
			this.label9.Size = new Size(11, 12);
			this.label9.TabIndex = 75;
			this.label9.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(441, 216);
			base.Controls.Add(this.txt_company_count);
			base.Controls.Add(this.txt_cid_count);
			base.Controls.Add(this.txt_even_count);
			base.Controls.Add(this.txt_odd_count);
			base.Controls.Add(this.txt_company_start);
			base.Controls.Add(this.txt_cid_start);
			base.Controls.Add(this.txt_even_start);
			base.Controls.Add(this.txt_odd_start);
			base.Controls.Add(this.txt_wiegand_count);
			base.Controls.Add(this.txt_wiegan_name);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.btn_wiegand_add_canncel);
			base.Controls.Add(this.btn_wiegand_add_ok);
			base.Controls.Add(this.lbl_company_count);
			base.Controls.Add(this.lbl_company_start);
			base.Controls.Add(this.lbl_cid_count);
			base.Controls.Add(this.lbl_cid_start);
			base.Controls.Add(this.lbl_even_cout);
			base.Controls.Add(this.lbl_even_start);
			base.Controls.Add(this.lbl_odd_count);
			base.Controls.Add(this.lbl_odd_start);
			base.Controls.Add(this.lbl_wiegand_count);
			base.Controls.Add(this.lbl_wiegan_name);
			base.Name = "wiegand_add";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "wiegand_add";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
