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
	public class AddAreaForm : Office2007Form
	{
		private int m_id = 0;

		private int m_parentID = 0;

		private PersonnelAreaBll areaBll = new PersonnelAreaBll(MainForm._ia);

		private List<PersonnelArea> m_arealist = null;

		private IContainer components = null;

		private Label lal_AreaCode;

		private Label lal_AreaName;

		private Label lal_Remarks;

		private TextBox txt_AreaCode;

		private TextBox txt_AreaName;

		private TextBox txt_Remarks;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		private Label label1;

		private Label label2;

		private Label label3;

		private ComboBox cmb_parent;

		private Label label4;

		public event EventHandler RefreshDataEvent = null;

		public AddAreaForm(int id, int parentid)
		{
			this.InitializeComponent();
			try
			{
				this.m_id = id;
				this.m_parentID = parentid;
				initLang.LocaleForm(this, base.Name);
				this.BindData();
				this.loadArea();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void loadArea()
		{
			this.m_arealist = this.areaBll.GetModelList("");
			if (this.m_arealist != null && this.m_arealist.Count > 0)
			{
				if (this.m_id > 0)
				{
					NodeManager nodeManager = new NodeManager();
					int num = 0;
					int num2;
					for (num = 0; num < this.m_arealist.Count; num++)
					{
						NodeBase nodeBase = new NodeBase();
						NodeBase nodeBase2 = nodeBase;
						num2 = this.m_arealist[num].id;
						nodeBase2.ID = num2.ToString();
						nodeBase.Name = nodeBase.ID;
						nodeBase.Tag = nodeBase.ID;
						NodeBase nodeBase3 = nodeBase;
						num2 = this.m_arealist[num].parent_id;
						nodeBase3.ParentNodeID = num2.ToString();
						nodeManager.Datasouce.Add(nodeBase);
					}
					if (nodeManager.Bind())
					{
						INode node = null;
						if (nodeManager.NTree != null)
						{
							node = nodeManager.NTree.FindNode(this.m_id.ToString());
						}
						for (num = 0; num < this.m_arealist.Count; num++)
						{
							if (this.m_arealist[num].id != this.m_id && this.m_arealist[num].parent_id != this.m_id)
							{
								int num3;
								if (node != null)
								{
									Tree nTree = nodeManager.NTree;
									INode node2 = node;
									num2 = this.m_arealist[num].id;
									num3 = ((!nTree.Exists(node2, num2.ToString())) ? 1 : 0);
								}
								else
								{
									num3 = 1;
								}
								if (num3 != 0)
								{
									this.cmb_parent.Items.Add(this.m_arealist[num].areaname);
									if (this.m_arealist[num].id == this.m_parentID)
									{
										this.cmb_parent.SelectedIndex = this.cmb_parent.Items.Count - 1;
									}
								}
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < this.m_arealist.Count; i++)
					{
						this.cmb_parent.Items.Add(this.m_arealist[i].areaname);
						if (this.m_arealist[i].id == this.m_parentID)
						{
							this.cmb_parent.SelectedIndex = this.cmb_parent.Items.Count - 1;
						}
					}
				}
			}
			if (this.cmb_parent.Items.Count == 0)
			{
				this.cmb_parent.Items.Add("-----");
				this.cmb_parent.SelectedIndex = 0;
			}
			else if (this.cmb_parent.SelectedIndex < 0)
			{
				this.cmb_parent.SelectedIndex = 0;
			}
		}

		private void BindData()
		{
			if (this.m_id > 0)
			{
				PersonnelArea model = this.areaBll.GetModel(this.m_id);
				if (model != null)
				{
					this.txt_AreaCode.Text = model.areaid;
					this.txt_AreaCode.Tag = model.areaid;
					this.txt_AreaName.Text = model.areaname;
					this.txt_AreaName.Tag = model.areaname;
					this.m_parentID = model.parent_id;
					if (model.remark != null)
					{
						this.txt_Remarks.Text = model.remark;
					}
					this.Text = ShowMsgInfos.GetInfo("SEdit", "编辑");
				}
			}
			else
			{
				this.Text = ShowMsgInfos.GetInfo("SAdd", "新增");
			}
		}

		private bool check()
		{
			try
			{
				if (string.IsNullOrEmpty(this.txt_AreaCode.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputAreaNO", "请输入区域编号"));
					this.txt_AreaCode.Focus();
					return false;
				}
				if (string.IsNullOrEmpty(this.txt_AreaName.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SInputName", "请输入名称"));
					this.txt_AreaName.Focus();
					return false;
				}
				if ((this.m_id == 0 || (this.txt_AreaCode.Tag != null && this.txt_AreaCode.Tag.ToString() != this.txt_AreaCode.Text)) && this.areaBll.ExistsAreaID(this.txt_AreaCode.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAreaIDRepeat", "区域编号重复"));
					return false;
				}
				if ((this.m_id == 0 || (this.txt_AreaName.Tag != null && this.txt_AreaName.Tag.ToString() != this.txt_AreaName.Text)) && this.areaBll.ExistsAreaName(this.txt_AreaName.Text.Trim()))
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SAreaNameRepeat", "区域名称重复"));
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
		}

		private bool Save()
		{
			if (this.check())
			{
				PersonnelArea personnelArea = null;
				if (this.m_id > 0)
				{
					personnelArea = this.areaBll.GetModel(this.m_id);
				}
				if (personnelArea == null)
				{
					personnelArea = new PersonnelArea();
				}
				if (this.m_arealist != null && this.m_arealist.Count > 0)
				{
					int num = 0;
					while (num < this.m_arealist.Count)
					{
						if (!(this.m_arealist[num].areaname == this.cmb_parent.Text))
						{
							num++;
							continue;
						}
						personnelArea.parent_id = this.m_arealist[num].id;
						break;
					}
				}
				else
				{
					personnelArea.parent_id = 0;
				}
				personnelArea.areaid = this.txt_AreaCode.Text;
				personnelArea.areaname = this.txt_AreaName.Text;
				personnelArea.remark = this.txt_Remarks.Text;
				if (personnelArea.id > 0)
				{
					try
					{
						if (this.areaBll.Update(personnelArea))
						{
							if (this.RefreshDataEvent != null)
							{
								this.RefreshDataEvent(personnelArea, null);
							}
							return true;
						}
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
					catch
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SaveDataFailure", "保存数据失败，请检查数据是否有效或者超出了数据限制长度"));
					}
				}
				else
				{
					try
					{
						this.areaBll.Add(personnelArea);
						this.txt_AreaName.Text = "";
						this.txt_AreaCode.Text = "";
						if (this.RefreshDataEvent != null)
						{
							this.RefreshDataEvent(personnelArea, null);
						}
						return true;
					}
					catch (Exception ex)
					{
						SysDialogs.ShowErrorMessage(ex.Message);
					}
				}
			}
			return false;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.Save())
			{
				base.Close();
			}
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void txt_AreaCode_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e);
		}

		private void txt_AreaName_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.KeyPress(sender, e);
		}

		private void txt_Remarks_KeyPress(object sender, KeyPressEventArgs e)
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(AddAreaForm));
			this.lal_AreaCode = new Label();
			this.lal_AreaName = new Label();
			this.lal_Remarks = new Label();
			this.txt_AreaCode = new TextBox();
			this.txt_AreaName = new TextBox();
			this.txt_Remarks = new TextBox();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.cmb_parent = new ComboBox();
			this.label4 = new Label();
			base.SuspendLayout();
			this.lal_AreaCode.Location = new Point(17, 49);
			this.lal_AreaCode.Name = "lal_AreaCode";
			this.lal_AreaCode.Size = new Size(87, 12);
			this.lal_AreaCode.TabIndex = 10;
			this.lal_AreaCode.Text = "区域编号";
			this.lal_AreaCode.TextAlign = ContentAlignment.MiddleLeft;
			this.lal_AreaName.Location = new Point(17, 16);
			this.lal_AreaName.Name = "lal_AreaName";
			this.lal_AreaName.Size = new Size(87, 12);
			this.lal_AreaName.TabIndex = 11;
			this.lal_AreaName.Text = "区域名称";
			this.lal_AreaName.TextAlign = ContentAlignment.MiddleLeft;
			this.lal_Remarks.Location = new Point(17, 111);
			this.lal_Remarks.Name = "lal_Remarks";
			this.lal_Remarks.Size = new Size(87, 12);
			this.lal_Remarks.TabIndex = 13;
			this.lal_Remarks.Text = "备注";
			this.lal_Remarks.TextAlign = ContentAlignment.MiddleLeft;
			this.txt_AreaCode.Location = new Point(109, 45);
			this.txt_AreaCode.Name = "txt_AreaCode";
			this.txt_AreaCode.Size = new Size(151, 21);
			this.txt_AreaCode.TabIndex = 1;
			this.txt_AreaCode.KeyPress += this.txt_AreaCode_KeyPress;
			this.txt_AreaName.Location = new Point(109, 12);
			this.txt_AreaName.Name = "txt_AreaName";
			this.txt_AreaName.Size = new Size(151, 21);
			this.txt_AreaName.TabIndex = 0;
			this.txt_AreaName.KeyPress += this.txt_AreaName_KeyPress;
			this.txt_Remarks.Location = new Point(109, 107);
			this.txt_Remarks.Name = "txt_Remarks";
			this.txt_Remarks.Size = new Size(151, 21);
			this.txt_Remarks.TabIndex = 3;
			this.txt_Remarks.KeyPress += this.txt_Remarks_KeyPress;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(88, 149);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 4;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(192, 149);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 5;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			this.label1.AutoSize = true;
			this.label1.ForeColor = Color.Red;
			this.label1.Location = new Point(270, 49);
			this.label1.Name = "label1";
			this.label1.Size = new Size(11, 12);
			this.label1.TabIndex = 24;
			this.label1.Text = "*";
			this.label2.AutoSize = true;
			this.label2.ForeColor = Color.Red;
			this.label2.Location = new Point(270, 16);
			this.label2.Name = "label2";
			this.label2.Size = new Size(11, 12);
			this.label2.TabIndex = 25;
			this.label2.Text = "*";
			this.label3.Location = new Point(17, 81);
			this.label3.Name = "label3";
			this.label3.Size = new Size(87, 12);
			this.label3.TabIndex = 26;
			this.label3.Text = "上级区域";
			this.label3.TextAlign = ContentAlignment.MiddleLeft;
			this.cmb_parent.FormattingEnabled = true;
			this.cmb_parent.Location = new Point(109, 77);
			this.cmb_parent.Name = "cmb_parent";
			this.cmb_parent.Size = new Size(151, 20);
			this.cmb_parent.TabIndex = 2;
			this.label4.AutoSize = true;
			this.label4.ForeColor = Color.Red;
			this.label4.Location = new Point(270, 81);
			this.label4.Name = "label4";
			this.label4.Size = new Size(11, 12);
			this.label4.TabIndex = 28;
			this.label4.Text = "*";
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(286, 184);
			base.Controls.Add(this.txt_Remarks);
			base.Controls.Add(this.cmb_parent);
			base.Controls.Add(this.txt_AreaCode);
			base.Controls.Add(this.txt_AreaName);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lal_Remarks);
			base.Controls.Add(this.lal_AreaName);
			base.Controls.Add(this.lal_AreaCode);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "AddAreaForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "新增";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
