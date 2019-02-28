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
	public class CboAreaTreeForm : Office2007Form
	{
		private PersonnelAreaBll bll = new PersonnelAreaBll(MainForm._ia);

		private IContainer components = null;

		private ButtonX btn_cancel;

		private ButtonX btn_OK;

		private TreeView TView_dept;

		public event EventHandler SelectAreaEvent;

		public CboAreaTreeForm()
		{
			this.InitializeComponent();
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			try
			{
				List<PersonnelArea> list = null;
				list = this.bll.GetModelList("");
				if (list != null)
				{
					NodeManager nodeManager = new NodeManager();
					for (int i = 0; i < list.Count; i++)
					{
						NodeBase nodeBase = new NodeBase();
						NodeBase nodeBase2 = nodeBase;
						int num = list[i].id;
						nodeBase2.ID = num.ToString();
						if (string.IsNullOrEmpty(list[i].areaid))
						{
							list[i].areaid = nodeBase.ID;
						}
						nodeBase.Name = list[i].areaid + "-" + list[i].areaname;
						NodeBase nodeBase3 = nodeBase;
						num = list[i].id;
						nodeBase3.Tag = num.ToString();
						NodeBase nodeBase4 = nodeBase;
						num = list[i].parent_id;
						nodeBase4.ParentNodeID = num.ToString();
						nodeManager.Datasouce.Add(nodeBase);
					}
					if (nodeManager.Bind())
					{
						nodeManager.ConvertToTree(this.TView_dept);
						this.TView_dept.ExpandAll();
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			try
			{
				if (this.TView_dept.SelectedNode == null)
				{
					SysDialogs.ShowWarningMessage("请选择需要编辑的区域!");
				}
				else if (this.TView_dept.SelectedNode.Tag != null)
				{
					PersonnelArea personnelArea = new PersonnelArea();
					personnelArea.id = int.Parse(this.TView_dept.SelectedNode.Tag.ToString());
					personnelArea.areaname = this.TView_dept.SelectedNode.Text.ToString();
					if (this.SelectAreaEvent != null)
					{
						this.SelectAreaEvent(personnelArea, null);
					}
				}
				base.Close();
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CboAreaTreeForm));
			this.btn_cancel = new ButtonX();
			this.btn_OK = new ButtonX();
			this.TView_dept = new TreeView();
			base.SuspendLayout();
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(216, 239);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 1;
			this.btn_cancel.Text = "取消";
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(114, 239);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 0;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.TView_dept.Location = new Point(3, 2);
			this.TView_dept.Name = "TView_dept";
			this.TView_dept.Size = new Size(304, 229);
			this.TView_dept.TabIndex = 3;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(310, 274);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.TView_dept);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CboAreaTreeForm";
			base.ShowInTaskbar = false;
			this.Text = "选择区域";
			base.ResumeLayout(false);
		}
	}
}
