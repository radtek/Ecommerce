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
	public class CboDeptTreeForm : Office2007Form
	{
		private DepartmentsBll bll = new DepartmentsBll(MainForm._ia);

		private IContainer components = null;

		private TreeView TView_dept;

		private ButtonX btn_OK;

		private ButtonX btn_cancel;

		public event EventHandler SelectDeptEvent;

		public CboDeptTreeForm()
		{
			this.InitializeComponent();
			this.BindData();
			initLang.LocaleForm(this, base.Name);
		}

		private void BindData()
		{
			try
			{
				List<Departments> list = null;
				list = this.bll.GetModelList("");
				if (list != null)
				{
					NodeManager nodeManager = new NodeManager();
					for (int i = 0; i < list.Count; i++)
					{
						NodeBase nodeBase = new NodeBase();
						NodeBase nodeBase2 = nodeBase;
						int num = list[i].DEPTID;
						nodeBase2.ID = num.ToString();
						if (string.IsNullOrEmpty(list[i].code))
						{
							list[i].code = nodeBase.ID;
						}
						nodeBase.Name = list[i].DEPTNAME;
						NodeBase nodeBase3 = nodeBase;
						num = list[i].DEPTID;
						nodeBase3.Tag = num.ToString();
						NodeBase nodeBase4 = nodeBase;
						num = list[i].SUPDEPTID;
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

		private void dataChoose()
		{
			try
			{
				if (this.TView_dept.SelectedNode == null)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SchooseDept", "请选择部门"));
				}
				else if (this.TView_dept.SelectedNode.Tag != null)
				{
					Departments departments = new Departments();
					departments.DEPTID = int.Parse(this.TView_dept.SelectedNode.Tag.ToString());
					departments.DEPTNAME = this.TView_dept.SelectedNode.Text.ToString();
					if (this.SelectDeptEvent != null)
					{
						this.SelectDeptEvent(departments, null);
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
			this.dataChoose();
			base.Close();
		}

		private void btn_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void TView_dept_DoubleClick(object sender, EventArgs e)
		{
			this.dataChoose();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(CboDeptTreeForm));
			this.TView_dept = new TreeView();
			this.btn_OK = new ButtonX();
			this.btn_cancel = new ButtonX();
			base.SuspendLayout();
			this.TView_dept.ItemHeight = 18;
			this.TView_dept.Location = new Point(3, 2);
			this.TView_dept.Name = "TView_dept";
			this.TView_dept.Size = new Size(304, 229);
			this.TView_dept.TabIndex = 0;
			this.TView_dept.DoubleClick += this.TView_dept_DoubleClick;
			this.btn_OK.AccessibleRole = AccessibleRole.PushButton;
			this.btn_OK.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_OK.Location = new Point(121, 245);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new Size(82, 23);
			this.btn_OK.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_OK.TabIndex = 1;
			this.btn_OK.Text = "确定";
			this.btn_OK.Click += this.btn_OK_Click;
			this.btn_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_cancel.Location = new Point(215, 244);
			this.btn_cancel.Name = "btn_cancel";
			this.btn_cancel.Size = new Size(82, 23);
			this.btn_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_cancel.TabIndex = 2;
			this.btn_cancel.Text = "取消";
			this.btn_cancel.Click += this.btn_cancel_Click;
			base.AcceptButton = this.btn_OK;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(309, 279);
			base.Controls.Add(this.btn_cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.TView_dept);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CboDeptTreeForm";
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "选择部门";
			base.ResumeLayout(false);
		}
	}
}
