/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class WiegandSeting : Office2007Form
	{
		private class ComboxItem
		{
			private string _text = null;

			private object _value = null;

			public string Text
			{
				get
				{
					return this._text;
				}
				set
				{
					this._text = value;
				}
			}

			public object Value
			{
				get
				{
					return this._value;
				}
				set
				{
					this._value = value;
				}
			}

			public override string ToString()
			{
				return this._text;
			}
		}

		private int m_id = 0;

		private int dict = -1;

		private AccWiegandfmtBll bll = new AccWiegandfmtBll(MainForm._ia);

		private Dictionary<int, string> WiegandTypeFmtList = new Dictionary<int, string>();

		private Dictionary<int, string> WiegandAutoFitFmt = new Dictionary<int, string>();

		private Dictionary<int, string> m_dic = new Dictionary<int, string>();

		private AccDoorBll accBll = new AccDoorBll(MainForm._ia);

		private AccDoor accDoor = null;

		private MachinesBll mbll = new MachinesBll(MainForm._ia);

		private Machines accDevice = null;

		private DataTable m_datatable = new DataTable();

		private AccWiegandfmt wiegand_model = null;

		private IContainer components = null;

		private DevComponents.DotNetBar.TabControl tab_WiegandParameters;

		private TabControlPanel tabControlPanel1;

		private TabItem tabItem_Wiegand;

		private TabControlPanel tabControlPanel2;

		private TabItem tabItem_WiegandFormat;

		private GridControl gd_wigand;

		private BandedGridView gd_wigand_view;

		private BandedGridColumn col_total;

		private BandedGridColumn col_odd_start;

		private BandedGridColumn col_odd_count;

		private BandedGridColumn col_even_start;

		private BandedGridColumn col_even_count;

		private BandedGridView bandedGridView1;

		private GridBand gridBand1;

		private BandedGridColumn gridColumn1;

		private BandedGridColumn gridColumn2;

		private BandedGridColumn gridColumn3;

		private BandedGridColumn gridColumn4;

		private BandedGridColumn gridColumn5;

		private BandedGridColumn gridColumn6;

		private BandedGridColumn gridColumn7;

		private BandedGridColumn gridColumn8;

		private BandedGridColumn col_comany_start;

		private BandedGridColumn col_comany_count;

		private LinkLabel linklbl_wiegand_del;

		private LinkLabel linklbl_wiegand_add;

		private GridBand Band_wigand_total;

		private GridBand Band_wiegand_Odd;

		private GridBand Band_wiegand_even;

		private GridBand Band_wiegand_cid;

		private BandedGridColumn col_cid_start;

		private BandedGridColumn col_cid_count;

		private GridBand Band_wiegand_company;

		private ButtonX btn_wiegandfmt_canncel;

		private ButtonX btn_wiegandfmt_ok;

		private LinkLabel linkLabel_wiegand_edit;

		private ButtonX btn_Wiegand_cancel;

		private ButtonX btn_Wiegand_ok;

		private GroupBox group_wiegand_Out;

		private GroupBox group_wiegand_In;

		private ComboBoxEx cmb_wiegandInType;

		private Label lab_wiegandInType;

		private ComboBox cmb_cust_wiegandInfmt;

		private RadioButton rdb_cust_wiegandInfmt;

		private ComboBox cmb_WiegandIn_Standard;

		private RadioButton rdb_WiegandIn_Standard;

		private RadioButton rdb_weigandIn_AutoFit;

		private ComboBoxEx cmb_wiegandOutType;

		private Label lab_wiegandOutType;

		private ComboBox cmb_cust_wiegandOutfmt;

		private RadioButton rdb_cust_wiegandOutfmt;

		private ComboBox cmb_WiegandOut_Standard;

		private RadioButton rdb_wiegandOut_Standard;

		private FlowLayoutPanel flowLayoutPanel1;

		public event EventHandler refreshDataEvent = null;

		public WiegandSeting()
		{
			this.InitializeComponent();
			this.InitDataTableSet();
			initLang.LocaleForm(this, base.Name);
		}

		public WiegandSeting(int pid, int id)
			: this()
		{
			this.m_id = id;
			if (id > 0)
			{
				this.accDoor = this.accBll.GetModel(id);
				this.accDevice = this.mbll.GetModel(this.accDoor.device_id);
				this.InitWiegandFmtList();
				this.intWiegandFmt();
			}
			this.showTabItem(pid);
			this.intInOutWiegandRaido();
			this.BindData(pid);
			this.CheckPermission();
		}

		private void CheckPermission()
		{
			if (!SysInfos.IsOwerControlPermission(SysInfos.AccessLevel))
			{
				this.linkLabel_wiegand_edit.Enabled = false;
				this.linklbl_wiegand_add.Enabled = false;
				this.linklbl_wiegand_del.Enabled = false;
			}
		}

		private void intInOutWiegandRaido()
		{
			this.rdb_WiegandIn_Standard_CheckedChanged(null, null);
			this.rdb_cust_wiegandInfmt_CheckedChanged(null, null);
			this.rdb_wiegandOut_Standard_CheckedChanged(null, null);
			this.rdb_cust_wiegandOutfmt_CheckedChanged(null, null);
		}

		private void InitWiegandFmtList()
		{
			this.cmb_wiegandInType.Items.Clear();
			this.WiegandTypeFmtList.Clear();
			this.WiegandTypeFmtList = this.GetDic();
			foreach (KeyValuePair<int, string> wiegandTypeFmt in this.WiegandTypeFmtList)
			{
				this.cmb_wiegandInType.Items.Add(wiegandTypeFmt.Value);
				this.cmb_wiegandOutType.Items.Add(wiegandTypeFmt.Value);
			}
			this.cmb_wiegandInType.SelectedIndex = 1;
			this.cmb_wiegandOutType.SelectedIndex = 1;
		}

		private void load()
		{
			if (this.m_dic.Count == 0)
			{
				this.m_dic.Add(0, "工号");
				this.m_dic.Add(1, "卡号");
				XmlNode rootNode = initLang.GetRootNode("WiegandFmtType");
				if (rootNode != null)
				{
					Dictionary<int, string> dictionary = new Dictionary<int, string>();
					foreach (KeyValuePair<int, string> item in this.m_dic)
					{
						Application.DoEvents();
						int key = item.Key;
						string addNodeValueByName = initLang.GetAddNodeValueByName(key.ToString(), rootNode, false);
						if (!string.IsNullOrEmpty(addNodeValueByName))
						{
							dictionary.Add(item.Key, addNodeValueByName);
						}
						else
						{
							dictionary.Add(item.Key, item.Value);
							key = item.Key;
							initLang.SetAddNodeValue(key.ToString(), item.Value, rootNode, false);
						}
					}
					initLang.Save();
					foreach (KeyValuePair<int, string> item2 in dictionary)
					{
						this.m_dic[item2.Key] = item2.Value;
					}
				}
			}
		}

		private Dictionary<int, string> GetDic()
		{
			this.load();
			return this.m_dic;
		}

		private void intWiegandFmt()
		{
			List<AccWiegandfmt> list = null;
			list = this.bll.GetModelList(" status = 1 ");
			Application.DoEvents();
			if (list != null && list.Count > 0)
			{
				this.WiegandAutoFitFmt.Clear();
				this.WiegandAutoFitFmt.Add(list[0].id, list[0].wiegand_name);
			}
			list = this.bll.GetModelList("");
			Application.DoEvents();
			if (list != null && list.Count > 0)
			{
				this.cmb_WiegandIn_Standard.Items.Clear();
				this.cmb_WiegandOut_Standard.Items.Clear();
				for (int i = 0; i < list.Count; i++)
				{
					Application.DoEvents();
					ComboxItem comboxItem = new ComboxItem();
					comboxItem.Value = list[i].id;
					if (list[i].wiegand_name == "SRBOn")
					{
						if (this.accDevice.device_type == 101 || this.accDevice.device_type == 102 || this.accDevice.device_type == 103)
						{
							comboxItem.Text = list[i].wiegand_name;
							this.cmb_WiegandOut_Standard.Items.Add(comboxItem);
						}
					}
					else if (list[i].wiegand_name == "AutoMatchWiegandFmt")
					{
						string text = comboxItem.Text = ShowMsgInfos.GetInfo("AutoMatchWiegandFmt", "自动匹配");
						this.cmb_WiegandIn_Standard.Items.Add(comboxItem);
					}
					else
					{
						comboxItem.Text = list[i].wiegand_name;
						this.cmb_WiegandOut_Standard.Items.Add(comboxItem);
						this.cmb_WiegandIn_Standard.Items.Add(comboxItem);
					}
				}
			}
		}

		private void showTabItem(int pid)
		{
			switch (pid)
			{
			case 0:
				this.tab_WiegandParameters.TabIndex = 0;
				this.tab_WiegandParameters.SelectedTab = this.tabItem_WiegandFormat;
				this.tabItem_Wiegand.Visible = false;
				this.tabItem_WiegandFormat.Visible = true;
				break;
			case 1:
			{
				this.tab_WiegandParameters.TabIndex = 1;
				this.tab_WiegandParameters.SelectedTab = this.tabItem_Wiegand;
				this.tabItem_Wiegand.Visible = true;
				this.tabItem_WiegandFormat.Visible = false;
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				if (this.accDoor != null)
				{
					Machines model = machinesBll.GetModel(this.accDoor.device_id);
					if (model != null && model.device_type != 101 && model.device_type != 102 && model.device_type != 103 && model.DevSDKType != SDKType.StandaloneSDK)
					{
						this.group_wiegand_Out.Visible = false;
					}
				}
				break;
			}
			}
		}

		private void InitDataTableSet()
		{
			this.m_datatable.Columns.Add("id").DataType = typeof(int);
			this.m_datatable.Columns.Add("wiegand_name");
			this.m_datatable.Columns.Add("wiegand_count");
			this.m_datatable.Columns.Add("odd_start");
			this.m_datatable.Columns.Add("odd_count");
			this.m_datatable.Columns.Add("even_start");
			this.m_datatable.Columns.Add("even_count");
			this.m_datatable.Columns.Add("cid_start");
			this.m_datatable.Columns.Add("cid_count");
			this.m_datatable.Columns.Add("comp_start");
			this.m_datatable.Columns.Add("comp_count");
			this.m_datatable.Columns.Add("status");
			this.col_total.FieldName = "wiegand_count";
			this.col_odd_start.FieldName = "odd_start";
			this.col_odd_count.FieldName = "odd_count";
			this.col_even_start.FieldName = "even_start";
			this.col_even_count.FieldName = "even_count";
			this.col_cid_start.FieldName = "cid_start";
			this.col_cid_count.FieldName = "cid_count";
			this.col_comany_start.FieldName = "comp_start";
			this.col_comany_count.FieldName = "comp_count";
		}

		private void BindData(int pid)
		{
			this.m_datatable.Rows.Clear();
			string empty = string.Empty;
			switch (pid)
			{
			case 0:
			{
				empty = " status = 0 ";
				DataSet list = this.bll.GetList(empty);
				this.m_datatable.BeginLoadData();
				if (list != null)
				{
					DataTable dataTable = list.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = this.m_datatable.NewRow();
						dataRow[0] = dataTable.Rows[i]["id"].ToString();
						dataRow[1] = dataTable.Rows[i]["wiegand_name"].ToString();
						dataRow[2] = dataTable.Rows[i]["wiegand_count"].ToString();
						dataRow[3] = dataTable.Rows[i]["odd_start"].ToString();
						dataRow[4] = dataTable.Rows[i]["odd_count"].ToString();
						dataRow[5] = dataTable.Rows[i]["even_start"].ToString();
						dataRow[6] = dataTable.Rows[i]["even_count"].ToString();
						dataRow[7] = dataTable.Rows[i]["cid_start"].ToString();
						dataRow[8] = dataTable.Rows[i]["cid_count"].ToString();
						dataRow[9] = dataTable.Rows[i]["comp_start"].ToString();
						dataRow[10] = dataTable.Rows[i]["comp_count"].ToString();
						dataRow[11] = dataTable.Rows[i]["status"].ToString();
						this.m_datatable.Rows.Add(dataRow);
					}
				}
				this.m_datatable.EndLoadData();
				this.gd_wigand.BeginUpdate();
				this.gd_wigand.DataSource = this.m_datatable;
				this.gd_wigand.EndUpdate();
				if (this.accDoor != null)
				{
					this.IntWiegandInData();
					this.IntWiegandOutData();
				}
				break;
			}
			case 1:
				this.intWiegandFmt();
				this.IntWiegandInData();
				this.IntWiegandOutData();
				break;
			}
		}

		private void IntWiegandInData()
		{
			this.cmb_wiegandInType.SelectedIndex = 1;
			this.rdb_weigandIn_AutoFit.Checked = false;
			if (this.cmb_WiegandIn_Standard.Items.Count > 0)
			{
				this.cmb_WiegandIn_Standard.SelectedIndex = 0;
			}
			AccWiegandfmt accWiegandfmt = null;
			if (this.accDoor != null)
			{
				this.cmb_wiegandInType.SelectedIndex = this.accDoor.wiegandInType;
				accWiegandfmt = this.bll.GetModel(this.accDoor.wiegand_fmt_id);
				if (accWiegandfmt != null)
				{
					this.rdb_WiegandIn_Standard.Checked = true;
					ComboxItem comboxItem = new ComboxItem();
					comboxItem.Text = accWiegandfmt.wiegand_name;
					comboxItem.Value = accWiegandfmt.id;
					string empty = string.Empty;
					empty = ((!(comboxItem.Text == "AutoMatchWiegandFmt")) ? comboxItem.Text : ShowMsgInfos.GetInfo("AutoMatchWiegandFmt", "自动匹配"));
					this.cmb_WiegandIn_Standard.SelectedIndex = this.cmb_WiegandIn_Standard.FindString(empty);
					if (this.cmb_WiegandIn_Standard.SelectedIndex < 0)
					{
						this.cmb_WiegandIn_Standard.SelectedIndex = 0;
					}
				}
			}
		}

		private void IntWiegandOutData()
		{
			this.cmb_wiegandOutType.SelectedIndex = 1;
			this.rdb_wiegandOut_Standard.Checked = true;
			this.cmb_WiegandOut_Standard.SelectedIndex = this.cmb_WiegandOut_Standard.FindString("wiegand26");
			AccWiegandfmt accWiegandfmt = null;
			if (this.accDoor != null)
			{
				this.cmb_wiegandOutType.SelectedIndex = this.accDoor.wiegandOutType;
				accWiegandfmt = this.bll.GetModel(this.accDoor.wiegand_fmt_out_id);
				if (accWiegandfmt != null)
				{
					ComboxItem comboxItem = new ComboxItem();
					if (accWiegandfmt.status == 2)
					{
						this.rdb_wiegandOut_Standard.Checked = true;
						comboxItem.Value = accWiegandfmt.id;
						comboxItem.Text = accWiegandfmt.wiegand_name;
						this.cmb_WiegandOut_Standard.SelectedIndex = this.cmb_WiegandOut_Standard.FindString(comboxItem.Text);
					}
					else if (accWiegandfmt.status == 0)
					{
						this.rdb_wiegandOut_Standard.Checked = true;
						comboxItem.Value = accWiegandfmt.id;
						comboxItem.Text = accWiegandfmt.wiegand_name;
						this.cmb_WiegandOut_Standard.SelectedIndex = this.cmb_WiegandOut_Standard.FindString(comboxItem.Text);
					}
				}
			}
		}

		private void txt_FPThreadHold_KeyPress(object sender, KeyPressEventArgs e)
		{
			CheckInfo.NumberKeyPress(sender, e, 35, 70L);
		}

		private void wiegandForm_refreshDataEvent(object sender, EventArgs e)
		{
			if (sender != null)
			{
				this.wiegand_model = (sender as AccWiegandfmt);
				switch (this.dict)
				{
				case 0:
				{
					DataRow dataRow = this.m_datatable.NewRow();
					DataRow dataRow2 = dataRow;
					int num = this.wiegand_model.id;
					dataRow2[0] = num.ToString();
					dataRow[1] = this.wiegand_model.wiegand_name.ToString();
					DataRow dataRow3 = dataRow;
					num = this.wiegand_model.wiegand_count;
					dataRow3[2] = num.ToString();
					DataRow dataRow4 = dataRow;
					num = this.wiegand_model.odd_start;
					dataRow4[3] = num.ToString();
					DataRow dataRow5 = dataRow;
					num = this.wiegand_model.odd_count;
					dataRow5[4] = num.ToString();
					DataRow dataRow6 = dataRow;
					num = this.wiegand_model.even_start;
					dataRow6[5] = num.ToString();
					DataRow dataRow7 = dataRow;
					num = this.wiegand_model.even_count;
					dataRow7[6] = num.ToString();
					DataRow dataRow8 = dataRow;
					num = this.wiegand_model.cid_start;
					dataRow8[7] = num.ToString();
					DataRow dataRow9 = dataRow;
					num = this.wiegand_model.cid_count;
					dataRow9[8] = num.ToString();
					DataRow dataRow10 = dataRow;
					num = this.wiegand_model.comp_start;
					dataRow10[9] = num.ToString();
					DataRow dataRow11 = dataRow;
					num = this.wiegand_model.comp_count;
					dataRow11[10] = num.ToString();
					DataRow dataRow12 = dataRow;
					num = this.wiegand_model.status;
					dataRow12[11] = num.ToString();
					this.m_datatable.Rows.Add(dataRow);
					break;
				}
				case 1:
				{
					DataRow[] array = this.m_datatable.Select("id =" + this.wiegand_model.id);
					if (array.Length != 0)
					{
						array[0].BeginEdit();
						array[0]["wiegand_name"] = this.wiegand_model.wiegand_name;
						array[0]["wiegand_count"] = this.wiegand_model.wiegand_count;
						array[0]["odd_start"] = this.wiegand_model.odd_start;
						array[0]["odd_count"] = this.wiegand_model.odd_count;
						array[0]["even_start"] = this.wiegand_model.even_start;
						array[0]["even_count"] = this.wiegand_model.even_count;
						array[0]["cid_start"] = this.wiegand_model.cid_start;
						array[0]["cid_count"] = this.wiegand_model.cid_count;
						array[0]["comp_start"] = this.wiegand_model.comp_start;
						array[0]["comp_count"] = this.wiegand_model.comp_count;
						array[0].EndEdit();
					}
					break;
				}
				}
			}
		}

		private void linklbl_wiegand_add_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.dict = 0;
			wiegand_add wiegand_add = new wiegand_add(0);
			wiegand_add.refreshDataEvent += this.wiegandForm_refreshDataEvent;
			wiegand_add.ShowDialog();
			wiegand_add.refreshDataEvent -= this.wiegandForm_refreshDataEvent;
		}

		private void linkLabel_wiegand_edit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (this.m_datatable.Rows.Count > 0)
			{
				int[] array = null;
				array = this.gd_wigand_view.GetSelectedRows();
				array = DevExpressHelper.GetDataSourceRowIndexs(this.gd_wigand_view, array);
				if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count)
				{
					if (array.Length == 1)
					{
						int id = int.Parse(this.m_datatable.Rows[array[0]][0].ToString());
						this.dict = 1;
						wiegand_add wiegand_add = new wiegand_add(id);
						wiegand_add.refreshDataEvent += this.wiegandForm_refreshDataEvent;
						wiegand_add.ShowDialog();
						wiegand_add.refreshDataEvent -= this.wiegandForm_refreshDataEvent;
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOnlySelectOneData", "进行该操作只能选择一条记录!"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SSelectEditData", "请选择要编辑的记录"));
				}
			}
		}

		private void linklbl_wiegand_del_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AccWiegandfmtBll accWiegandfmtBll = new AccWiegandfmtBll(MainForm._ia);
			this.dict = 2;
			int[] array = null;
			array = this.gd_wigand_view.GetSelectedRows();
			array = DevExpressHelper.GetDataSourceRowIndexs(this.gd_wigand_view, array);
			if (array != null && array.Length != 0 && array[0] >= 0 && array[0] < this.m_datatable.Rows.Count && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SIsDeleteData", "确定要删除选定的记录?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes && accWiegandfmtBll.Delete(int.Parse(this.m_datatable.Rows[array[0]][0].ToString())))
			{
				this.BindData(0);
			}
		}

		private void tab_WiegandParameters_SelectedTabChanged(object sender, TabStripTabChangedEventArgs e)
		{
			if (this.tab_WiegandParameters.SelectedTab == this.tabItem_WiegandFormat)
			{
				this.BindData(0);
			}
			else if (this.tab_WiegandParameters.SelectedTab == this.tabItem_Wiegand)
			{
				this.BindData(1);
			}
		}

		private void rdb_WiegandIn_Standard_CheckedChanged(object sender, EventArgs e)
		{
			this.cmb_WiegandIn_Standard.Enabled = this.rdb_WiegandIn_Standard.Checked;
		}

		private void rdb_cust_wiegandInfmt_CheckedChanged(object sender, EventArgs e)
		{
			this.cmb_cust_wiegandInfmt.Enabled = this.rdb_cust_wiegandInfmt.Checked;
		}

		private void rdb_wiegandOut_Standard_CheckedChanged(object sender, EventArgs e)
		{
			this.cmb_WiegandOut_Standard.Enabled = this.rdb_wiegandOut_Standard.Checked;
		}

		private void rdb_cust_wiegandOutfmt_CheckedChanged(object sender, EventArgs e)
		{
			this.cmb_cust_wiegandOutfmt.Enabled = this.rdb_cust_wiegandOutfmt.Checked;
		}

		private void btn_Wiegand_cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void btn_Wiegand_ok_Click(object sender, EventArgs e)
		{
			if (this.m_id == 0 && this.tabItem_WiegandFormat.IsSelected)
			{
				base.Close();
			}
			else if (this.check() && this.Save())
			{
				base.Close();
			}
		}

		private bool Save()
		{
			AccDoor accDoor = new AccDoor();
			try
			{
				ComboxItem comboxItem = new ComboxItem();
				accDoor.wiegandInType = this.cmb_wiegandInType.SelectedIndex;
				if (this.rdb_weigandIn_AutoFit.Checked)
				{
					using (Dictionary<int, string>.Enumerator enumerator = this.WiegandAutoFitFmt.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							accDoor.wiegand_fmt_id = enumerator.Current.Key;
						}
					}
				}
				else if (this.rdb_WiegandIn_Standard.Checked)
				{
					comboxItem = (ComboxItem)this.cmb_WiegandIn_Standard.SelectedItem;
					accDoor.wiegand_fmt_id = (int)comboxItem.Value;
				}
				else if (this.rdb_cust_wiegandInfmt.Checked)
				{
					comboxItem = (ComboxItem)this.cmb_cust_wiegandInfmt.SelectedItem;
					accDoor.wiegand_fmt_id = (int)comboxItem.Value;
				}
				accDoor.wiegandOutType = this.cmb_wiegandOutType.SelectedIndex;
				if (this.rdb_wiegandOut_Standard.Checked)
				{
					comboxItem = (ComboxItem)this.cmb_WiegandOut_Standard.SelectedItem;
					accDoor.wiegand_fmt_out_id = (int)comboxItem.Value;
					accDoor.SRBOn = 0;
					if (comboxItem.Text == "SRBOn")
					{
						accDoor.SRBOn = 1;
					}
				}
				else if (this.rdb_cust_wiegandOutfmt.Checked)
				{
					comboxItem = (ComboxItem)this.cmb_cust_wiegandOutfmt.SelectedItem;
					accDoor.wiegand_fmt_out_id = (int)comboxItem.Value;
				}
				if (this.refreshDataEvent != null)
				{
					this.refreshDataEvent(accDoor, null);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
			return true;
		}

		private bool check()
		{
			if (this.rdb_WiegandIn_Standard.Checked && this.cmb_WiegandIn_Standard.SelectedIndex < 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoSelectWiegandInStandard", "请选择韦根输入格式"));
				this.cmb_WiegandIn_Standard.Focus();
				return false;
			}
			if (this.rdb_cust_wiegandInfmt.Checked && this.cmb_cust_wiegandInfmt.SelectedIndex < 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoSelectWiegandInCust", "请选择韦根自定义输入格式"));
				this.cmb_cust_wiegandInfmt.Focus();
				return false;
			}
			if (this.rdb_wiegandOut_Standard.Checked && this.cmb_WiegandOut_Standard.SelectedIndex < 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoSelectWiegandOutStandard", "请选择韦根输出格式"));
				this.cmb_WiegandOut_Standard.Focus();
				return false;
			}
			if (this.rdb_cust_wiegandOutfmt.Checked && this.cmb_cust_wiegandOutfmt.SelectedIndex < 0)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNoSelectWiegandOutCust", "请选择韦根自定义输出格式"));
				this.cmb_cust_wiegandOutfmt.Focus();
				return false;
			}
			return true;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(WiegandSeting));
			this.tab_WiegandParameters = new DevComponents.DotNetBar.TabControl();
			this.tabControlPanel2 = new TabControlPanel();
			this.flowLayoutPanel1 = new FlowLayoutPanel();
			this.linklbl_wiegand_add = new LinkLabel();
			this.linkLabel_wiegand_edit = new LinkLabel();
			this.linklbl_wiegand_del = new LinkLabel();
			this.gd_wigand = new GridControl();
			this.gd_wigand_view = new BandedGridView();
			this.Band_wigand_total = new GridBand();
			this.col_total = new BandedGridColumn();
			this.Band_wiegand_Odd = new GridBand();
			this.col_odd_start = new BandedGridColumn();
			this.col_odd_count = new BandedGridColumn();
			this.Band_wiegand_even = new GridBand();
			this.col_even_start = new BandedGridColumn();
			this.col_even_count = new BandedGridColumn();
			this.Band_wiegand_cid = new GridBand();
			this.col_cid_start = new BandedGridColumn();
			this.col_cid_count = new BandedGridColumn();
			this.Band_wiegand_company = new GridBand();
			this.col_comany_start = new BandedGridColumn();
			this.col_comany_count = new BandedGridColumn();
			this.bandedGridView1 = new BandedGridView();
			this.gridBand1 = new GridBand();
			this.gridColumn1 = new BandedGridColumn();
			this.gridColumn2 = new BandedGridColumn();
			this.gridColumn3 = new BandedGridColumn();
			this.gridColumn4 = new BandedGridColumn();
			this.gridColumn5 = new BandedGridColumn();
			this.gridColumn6 = new BandedGridColumn();
			this.gridColumn7 = new BandedGridColumn();
			this.gridColumn8 = new BandedGridColumn();
			this.btn_wiegandfmt_canncel = new ButtonX();
			this.btn_wiegandfmt_ok = new ButtonX();
			this.tabItem_WiegandFormat = new TabItem(this.components);
			this.tabControlPanel1 = new TabControlPanel();
			this.group_wiegand_In = new GroupBox();
			this.cmb_wiegandInType = new ComboBoxEx();
			this.lab_wiegandInType = new Label();
			this.cmb_cust_wiegandInfmt = new ComboBox();
			this.rdb_cust_wiegandInfmt = new RadioButton();
			this.cmb_WiegandIn_Standard = new ComboBox();
			this.rdb_WiegandIn_Standard = new RadioButton();
			this.rdb_weigandIn_AutoFit = new RadioButton();
			this.group_wiegand_Out = new GroupBox();
			this.cmb_wiegandOutType = new ComboBoxEx();
			this.lab_wiegandOutType = new Label();
			this.cmb_cust_wiegandOutfmt = new ComboBox();
			this.rdb_cust_wiegandOutfmt = new RadioButton();
			this.cmb_WiegandOut_Standard = new ComboBox();
			this.rdb_wiegandOut_Standard = new RadioButton();
			this.tabItem_Wiegand = new TabItem(this.components);
			this.btn_Wiegand_cancel = new ButtonX();
			this.btn_Wiegand_ok = new ButtonX();
			((ISupportInitialize)this.tab_WiegandParameters).BeginInit();
			this.tab_WiegandParameters.SuspendLayout();
			this.tabControlPanel2.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			((ISupportInitialize)this.gd_wigand).BeginInit();
			((ISupportInitialize)this.gd_wigand_view).BeginInit();
			((ISupportInitialize)this.bandedGridView1).BeginInit();
			this.tabControlPanel1.SuspendLayout();
			this.group_wiegand_In.SuspendLayout();
			this.group_wiegand_Out.SuspendLayout();
			base.SuspendLayout();
			this.tab_WiegandParameters.BackColor = Color.FromArgb(194, 217, 247);
			this.tab_WiegandParameters.CanReorderTabs = true;
			this.tab_WiegandParameters.Controls.Add(this.tabControlPanel2);
			this.tab_WiegandParameters.Controls.Add(this.tabControlPanel1);
			this.tab_WiegandParameters.Location = new Point(-1, 1);
			this.tab_WiegandParameters.Name = "tab_WiegandParameters";
			this.tab_WiegandParameters.SelectedTabFont = new Font("SimSun", 9f, FontStyle.Bold);
			this.tab_WiegandParameters.SelectedTabIndex = 0;
			this.tab_WiegandParameters.Size = new Size(585, 250);
			this.tab_WiegandParameters.TabIndex = 22;
			this.tab_WiegandParameters.TabLayoutType = eTabLayoutType.FixedWithNavigationBox;
			this.tab_WiegandParameters.Tabs.Add(this.tabItem_WiegandFormat);
			this.tab_WiegandParameters.Tabs.Add(this.tabItem_Wiegand);
			this.tab_WiegandParameters.Text = "tabControl1";
			this.tabControlPanel2.Controls.Add(this.flowLayoutPanel1);
			this.tabControlPanel2.Controls.Add(this.gd_wigand);
			this.tabControlPanel2.Controls.Add(this.btn_wiegandfmt_canncel);
			this.tabControlPanel2.Controls.Add(this.btn_wiegandfmt_ok);
			this.tabControlPanel2.Dock = DockStyle.Fill;
			this.tabControlPanel2.Location = new Point(0, 26);
			this.tabControlPanel2.Name = "tabControlPanel2";
			this.tabControlPanel2.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel2.Size = new Size(585, 224);
			this.tabControlPanel2.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel2.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel2.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel2.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel2.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel2.Style.GradientAngle = 90;
			this.tabControlPanel2.TabIndex = 0;
			this.tabControlPanel2.TabItem = this.tabItem_WiegandFormat;
			this.flowLayoutPanel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.flowLayoutPanel1.BackColor = Color.FromArgb(149, 184, 233);
			this.flowLayoutPanel1.Controls.Add(this.linklbl_wiegand_add);
			this.flowLayoutPanel1.Controls.Add(this.linkLabel_wiegand_edit);
			this.flowLayoutPanel1.Controls.Add(this.linklbl_wiegand_del);
			this.flowLayoutPanel1.Location = new Point(4, 8);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new Size(577, 20);
			this.flowLayoutPanel1.TabIndex = 50;
			this.linklbl_wiegand_add.AutoSize = true;
			this.linklbl_wiegand_add.BackColor = Color.Transparent;
			this.linklbl_wiegand_add.ForeColor = SystemColors.ActiveCaption;
			this.linklbl_wiegand_add.LinkColor = Color.Blue;
			this.linklbl_wiegand_add.Location = new Point(3, 0);
			this.linklbl_wiegand_add.Name = "linklbl_wiegand_add";
			this.linklbl_wiegand_add.Size = new Size(29, 12);
			this.linklbl_wiegand_add.TabIndex = 47;
			this.linklbl_wiegand_add.TabStop = true;
			this.linklbl_wiegand_add.Text = "新增";
			this.linklbl_wiegand_add.LinkClicked += this.linklbl_wiegand_add_LinkClicked;
			this.linkLabel_wiegand_edit.AutoSize = true;
			this.linkLabel_wiegand_edit.BackColor = Color.Transparent;
			this.linkLabel_wiegand_edit.ForeColor = SystemColors.ActiveCaption;
			this.linkLabel_wiegand_edit.LinkColor = Color.Blue;
			this.linkLabel_wiegand_edit.Location = new Point(38, 0);
			this.linkLabel_wiegand_edit.Name = "linkLabel_wiegand_edit";
			this.linkLabel_wiegand_edit.Size = new Size(29, 12);
			this.linkLabel_wiegand_edit.TabIndex = 49;
			this.linkLabel_wiegand_edit.TabStop = true;
			this.linkLabel_wiegand_edit.Text = "编辑";
			this.linkLabel_wiegand_edit.LinkClicked += this.linkLabel_wiegand_edit_LinkClicked;
			this.linklbl_wiegand_del.AutoSize = true;
			this.linklbl_wiegand_del.BackColor = Color.Transparent;
			this.linklbl_wiegand_del.ForeColor = SystemColors.ActiveCaption;
			this.linklbl_wiegand_del.LinkColor = Color.Blue;
			this.linklbl_wiegand_del.Location = new Point(73, 0);
			this.linklbl_wiegand_del.Name = "linklbl_wiegand_del";
			this.linklbl_wiegand_del.Size = new Size(29, 12);
			this.linklbl_wiegand_del.TabIndex = 48;
			this.linklbl_wiegand_del.TabStop = true;
			this.linklbl_wiegand_del.Text = "删除";
			this.linklbl_wiegand_del.LinkClicked += this.linklbl_wiegand_del_LinkClicked;
			this.gd_wigand.Location = new Point(0, 33);
			this.gd_wigand.MainView = this.gd_wigand_view;
			this.gd_wigand.Name = "gd_wigand";
			this.gd_wigand.Size = new Size(585, 194);
			this.gd_wigand.TabIndex = 0;
			this.gd_wigand.ViewCollection.AddRange(new BaseView[2]
			{
				this.gd_wigand_view,
				this.bandedGridView1
			});
			this.gd_wigand_view.Appearance.FixedLine.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.FixedLine.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.GroupRow.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.GroupRow.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.HeaderPanel.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.HeaderPanel.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.HorzLine.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.HorzLine.BackColor2 = Color.Red;
			this.gd_wigand_view.Appearance.HorzLine.BorderColor = Color.Green;
			this.gd_wigand_view.Appearance.HorzLine.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.HorzLine.Options.UseBorderColor = true;
			this.gd_wigand_view.Appearance.OddRow.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.OddRow.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.Preview.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.Preview.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.Row.BackColor = Color.White;
			this.gd_wigand_view.Appearance.Row.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.VertLine.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.VertLine.BackColor2 = Color.Red;
			this.gd_wigand_view.Appearance.VertLine.BorderColor = Color.Green;
			this.gd_wigand_view.Appearance.VertLine.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.VertLine.Options.UseBorderColor = true;
			this.gd_wigand_view.Appearance.ViewCaption.BackColor = Color.Blue;
			this.gd_wigand_view.Appearance.ViewCaption.Options.UseBackColor = true;
			this.gd_wigand_view.Appearance.ViewCaption.Options.UseTextOptions = true;
			this.gd_wigand_view.Appearance.ViewCaption.TextOptions.HAlignment = HorzAlignment.Center;
			this.gd_wigand_view.Appearance.ViewCaption.TextOptions.VAlignment = VertAlignment.Center;
			this.gd_wigand_view.AppearancePrint.Lines.BackColor = Color.White;
			this.gd_wigand_view.AppearancePrint.Lines.BackColor2 = Color.White;
			this.gd_wigand_view.AppearancePrint.Lines.BorderColor = Color.White;
			this.gd_wigand_view.AppearancePrint.Lines.Options.UseBackColor = true;
			this.gd_wigand_view.AppearancePrint.Lines.Options.UseBorderColor = true;
			this.gd_wigand_view.AppearancePrint.Preview.BackColor = Color.White;
			this.gd_wigand_view.AppearancePrint.Preview.BackColor2 = Color.White;
			this.gd_wigand_view.AppearancePrint.Preview.BorderColor = Color.White;
			this.gd_wigand_view.AppearancePrint.Preview.Options.UseBackColor = true;
			this.gd_wigand_view.AppearancePrint.Preview.Options.UseBorderColor = true;
			this.gd_wigand_view.Bands.AddRange(new GridBand[5]
			{
				this.Band_wigand_total,
				this.Band_wiegand_Odd,
				this.Band_wiegand_even,
				this.Band_wiegand_cid,
				this.Band_wiegand_company
			});
			this.gd_wigand_view.Columns.AddRange(new BandedGridColumn[9]
			{
				this.col_total,
				this.col_odd_count,
				this.col_odd_start,
				this.col_even_count,
				this.col_even_start,
				this.col_cid_start,
				this.col_cid_count,
				this.col_comany_start,
				this.col_comany_count
			});
			this.gd_wigand_view.FixedLineWidth = 5;
			this.gd_wigand_view.GridControl = this.gd_wigand;
			this.gd_wigand_view.Name = "gd_wigand_view";
			this.gd_wigand_view.OptionsView.ShowGroupPanel = false;
			this.gd_wigand_view.PaintStyleName = "Office2003";
			this.Band_wigand_total.AppearanceHeader.BackColor = Color.LightBlue;
			this.Band_wigand_total.AppearanceHeader.BackColor2 = Color.LightBlue;
			this.Band_wigand_total.AppearanceHeader.BorderColor = Color.LightBlue;
			this.Band_wigand_total.AppearanceHeader.Options.UseBackColor = true;
			this.Band_wigand_total.AppearanceHeader.Options.UseBorderColor = true;
			this.Band_wigand_total.AppearanceHeader.Options.UseTextOptions = true;
			this.Band_wigand_total.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
			this.Band_wigand_total.Caption = "韦根长度";
			this.Band_wigand_total.Columns.Add(this.col_total);
			this.Band_wigand_total.MinWidth = 20;
			this.Band_wigand_total.Name = "Band_wigand_total";
			this.Band_wigand_total.Width = 66;
			this.col_total.Caption = "总长度";
			this.col_total.Name = "col_total";
			this.col_total.Visible = true;
			this.col_total.Width = 66;
			this.Band_wiegand_Odd.AppearanceHeader.Options.UseTextOptions = true;
			this.Band_wiegand_Odd.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
			this.Band_wiegand_Odd.Caption = "奇校验";
			this.Band_wiegand_Odd.Columns.Add(this.col_odd_start);
			this.Band_wiegand_Odd.Columns.Add(this.col_odd_count);
			this.Band_wiegand_Odd.Name = "Band_wiegand_Odd";
			this.Band_wiegand_Odd.Width = 129;
			this.col_odd_start.Caption = "开始位";
			this.col_odd_start.Name = "col_odd_start";
			this.col_odd_start.Visible = true;
			this.col_odd_start.Width = 55;
			this.col_odd_count.Caption = "长度";
			this.col_odd_count.Name = "col_odd_count";
			this.col_odd_count.Visible = true;
			this.col_odd_count.Width = 74;
			this.Band_wiegand_even.AppearanceHeader.Options.UseTextOptions = true;
			this.Band_wiegand_even.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
			this.Band_wiegand_even.Caption = "偶校验";
			this.Band_wiegand_even.Columns.Add(this.col_even_start);
			this.Band_wiegand_even.Columns.Add(this.col_even_count);
			this.Band_wiegand_even.Name = "Band_wiegand_even";
			this.Band_wiegand_even.Width = 113;
			this.col_even_start.Caption = "开始位";
			this.col_even_start.Name = "col_even_start";
			this.col_even_start.Visible = true;
			this.col_even_start.Width = 56;
			this.col_even_count.Caption = "长度";
			this.col_even_count.Name = "col_even_count";
			this.col_even_count.Visible = true;
			this.col_even_count.Width = 57;
			this.Band_wiegand_cid.AppearanceHeader.Options.UseTextOptions = true;
			this.Band_wiegand_cid.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
			this.Band_wiegand_cid.Caption = "卡号码";
			this.Band_wiegand_cid.Columns.Add(this.col_cid_start);
			this.Band_wiegand_cid.Columns.Add(this.col_cid_count);
			this.Band_wiegand_cid.Name = "Band_wiegand_cid";
			this.Band_wiegand_cid.Width = 125;
			this.col_cid_start.Caption = "开始位";
			this.col_cid_start.Name = "col_cid_start";
			this.col_cid_start.Visible = true;
			this.col_cid_start.Width = 55;
			this.col_cid_count.Caption = "长度";
			this.col_cid_count.Name = "col_cid_count";
			this.col_cid_count.Visible = true;
			this.col_cid_count.Width = 70;
			this.Band_wiegand_company.AppearanceHeader.Options.UseTextOptions = true;
			this.Band_wiegand_company.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
			this.Band_wiegand_company.Caption = "公司码";
			this.Band_wiegand_company.Columns.Add(this.col_comany_start);
			this.Band_wiegand_company.Columns.Add(this.col_comany_count);
			this.Band_wiegand_company.Name = "Band_wiegand_company";
			this.Band_wiegand_company.Width = 135;
			this.col_comany_start.Caption = "开始位";
			this.col_comany_start.Name = "col_comany_start";
			this.col_comany_start.Visible = true;
			this.col_comany_start.Width = 57;
			this.col_comany_count.Caption = "长度";
			this.col_comany_count.Name = "col_comany_count";
			this.col_comany_count.Visible = true;
			this.col_comany_count.Width = 78;
			this.bandedGridView1.Bands.AddRange(new GridBand[1]
			{
				this.gridBand1
			});
			this.bandedGridView1.Columns.AddRange(new BandedGridColumn[8]
			{
				this.gridColumn1,
				this.gridColumn2,
				this.gridColumn3,
				this.gridColumn4,
				this.gridColumn5,
				this.gridColumn6,
				this.gridColumn7,
				this.gridColumn8
			});
			this.bandedGridView1.GridControl = this.gd_wigand;
			this.bandedGridView1.Name = "bandedGridView1";
			this.gridBand1.Caption = "gridBand1";
			this.gridBand1.Columns.Add(this.gridColumn1);
			this.gridBand1.Columns.Add(this.gridColumn2);
			this.gridBand1.Columns.Add(this.gridColumn3);
			this.gridBand1.Columns.Add(this.gridColumn4);
			this.gridBand1.Columns.Add(this.gridColumn5);
			this.gridBand1.Columns.Add(this.gridColumn6);
			this.gridBand1.Columns.Add(this.gridColumn7);
			this.gridBand1.Columns.Add(this.gridColumn8);
			this.gridBand1.Name = "gridBand1";
			this.gridBand1.Width = 600;
			this.gridColumn1.Caption = "gridColumn1";
			this.gridColumn1.Name = "gridColumn1";
			this.gridColumn1.Visible = true;
			this.gridColumn2.Caption = "gridColumn2";
			this.gridColumn2.Name = "gridColumn2";
			this.gridColumn2.Visible = true;
			this.gridColumn3.Caption = "gridColumn3";
			this.gridColumn3.Name = "gridColumn3";
			this.gridColumn3.Visible = true;
			this.gridColumn4.Caption = "gridColumn4";
			this.gridColumn4.Name = "gridColumn4";
			this.gridColumn4.Visible = true;
			this.gridColumn5.Caption = "gridColumn5";
			this.gridColumn5.Name = "gridColumn5";
			this.gridColumn5.Visible = true;
			this.gridColumn6.Caption = "gridColumn6";
			this.gridColumn6.Name = "gridColumn6";
			this.gridColumn6.Visible = true;
			this.gridColumn7.Caption = "gridColumn7";
			this.gridColumn7.Name = "gridColumn7";
			this.gridColumn7.Visible = true;
			this.gridColumn8.Caption = "gridColumn8";
			this.gridColumn8.Name = "gridColumn8";
			this.gridColumn8.Visible = true;
			this.btn_wiegandfmt_canncel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_wiegandfmt_canncel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_wiegandfmt_canncel.Location = new Point(484, 268);
			this.btn_wiegandfmt_canncel.Name = "btn_wiegandfmt_canncel";
			this.btn_wiegandfmt_canncel.Size = new Size(82, 23);
			this.btn_wiegandfmt_canncel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_wiegandfmt_canncel.TabIndex = 46;
			this.btn_wiegandfmt_canncel.Text = "取消";
			this.btn_wiegandfmt_canncel.Visible = false;
			this.btn_wiegandfmt_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_wiegandfmt_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_wiegandfmt_ok.Location = new Point(382, 268);
			this.btn_wiegandfmt_ok.Name = "btn_wiegandfmt_ok";
			this.btn_wiegandfmt_ok.Size = new Size(82, 23);
			this.btn_wiegandfmt_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_wiegandfmt_ok.TabIndex = 45;
			this.btn_wiegandfmt_ok.Text = "确定";
			this.btn_wiegandfmt_ok.Visible = false;
			this.tabItem_WiegandFormat.AttachedControl = this.tabControlPanel2;
			this.tabItem_WiegandFormat.Name = "tabItem_WiegandFormat";
			this.tabItem_WiegandFormat.Text = "自定义韦根格式表";
			this.tabControlPanel1.Controls.Add(this.group_wiegand_In);
			this.tabControlPanel1.Controls.Add(this.group_wiegand_Out);
			this.tabControlPanel1.Dock = DockStyle.Fill;
			this.tabControlPanel1.Location = new Point(0, 26);
			this.tabControlPanel1.Name = "tabControlPanel1";
			this.tabControlPanel1.Padding = new System.Windows.Forms.Padding(1);
			this.tabControlPanel1.Size = new Size(585, 224);
			this.tabControlPanel1.Style.BackColor1.Color = Color.FromArgb(142, 179, 231);
			this.tabControlPanel1.Style.BackColor2.Color = Color.FromArgb(223, 237, 254);
			this.tabControlPanel1.Style.Border = eBorderType.SingleLine;
			this.tabControlPanel1.Style.BorderColor.Color = Color.FromArgb(59, 97, 156);
			this.tabControlPanel1.Style.BorderSide = (eBorderSide.Left | eBorderSide.Right | eBorderSide.Bottom);
			this.tabControlPanel1.Style.GradientAngle = 90;
			this.tabControlPanel1.TabIndex = 1;
			this.tabControlPanel1.TabItem = this.tabItem_Wiegand;
			this.group_wiegand_In.BackColor = Color.FromArgb(194, 217, 247);
			this.group_wiegand_In.Controls.Add(this.cmb_wiegandInType);
			this.group_wiegand_In.Controls.Add(this.lab_wiegandInType);
			this.group_wiegand_In.Controls.Add(this.cmb_cust_wiegandInfmt);
			this.group_wiegand_In.Controls.Add(this.rdb_cust_wiegandInfmt);
			this.group_wiegand_In.Controls.Add(this.cmb_WiegandIn_Standard);
			this.group_wiegand_In.Controls.Add(this.rdb_WiegandIn_Standard);
			this.group_wiegand_In.Controls.Add(this.rdb_weigandIn_AutoFit);
			this.group_wiegand_In.Location = new Point(11, 4);
			this.group_wiegand_In.Name = "group_wiegand_In";
			this.group_wiegand_In.Size = new Size(560, 113);
			this.group_wiegand_In.TabIndex = 69;
			this.group_wiegand_In.TabStop = false;
			this.group_wiegand_In.Text = "韦根输入";
			this.cmb_wiegandInType.DisplayMember = "Text";
			this.cmb_wiegandInType.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_wiegandInType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_wiegandInType.Enabled = false;
			this.cmb_wiegandInType.FormattingEnabled = true;
			this.cmb_wiegandInType.ItemHeight = 15;
			this.cmb_wiegandInType.Location = new Point(256, 113);
			this.cmb_wiegandInType.Name = "cmb_wiegandInType";
			this.cmb_wiegandInType.Size = new Size(158, 21);
			this.cmb_wiegandInType.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_wiegandInType.TabIndex = 59;
			this.cmb_wiegandInType.Visible = false;
			this.lab_wiegandInType.BackColor = Color.Transparent;
			this.lab_wiegandInType.Location = new Point(21, 115);
			this.lab_wiegandInType.Name = "lab_wiegandInType";
			this.lab_wiegandInType.Size = new Size(178, 12);
			this.lab_wiegandInType.TabIndex = 60;
			this.lab_wiegandInType.Text = "韦根输入类型";
			this.lab_wiegandInType.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_wiegandInType.Visible = false;
			this.cmb_cust_wiegandInfmt.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_cust_wiegandInfmt.FormattingEnabled = true;
			this.cmb_cust_wiegandInfmt.Location = new Point(256, 80);
			this.cmb_cust_wiegandInfmt.Name = "cmb_cust_wiegandInfmt";
			this.cmb_cust_wiegandInfmt.Size = new Size(279, 20);
			this.cmb_cust_wiegandInfmt.TabIndex = 58;
			this.cmb_cust_wiegandInfmt.Visible = false;
			this.rdb_cust_wiegandInfmt.BackColor = Color.FromArgb(194, 217, 247);
			this.rdb_cust_wiegandInfmt.FlatStyle = FlatStyle.System;
			this.rdb_cust_wiegandInfmt.Location = new Point(18, 84);
			this.rdb_cust_wiegandInfmt.Name = "rdb_cust_wiegandInfmt";
			this.rdb_cust_wiegandInfmt.Size = new Size(158, 16);
			this.rdb_cust_wiegandInfmt.TabIndex = 57;
			this.rdb_cust_wiegandInfmt.Text = "自定义格式";
			this.rdb_cust_wiegandInfmt.TextImageRelation = TextImageRelation.TextAboveImage;
			this.rdb_cust_wiegandInfmt.UseVisualStyleBackColor = true;
			this.rdb_cust_wiegandInfmt.Visible = false;
			this.rdb_cust_wiegandInfmt.CheckedChanged += this.rdb_cust_wiegandInfmt_CheckedChanged;
			this.cmb_WiegandIn_Standard.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_WiegandIn_Standard.FormattingEnabled = true;
			this.cmb_WiegandIn_Standard.Location = new Point(256, 27);
			this.cmb_WiegandIn_Standard.Name = "cmb_WiegandIn_Standard";
			this.cmb_WiegandIn_Standard.Size = new Size(279, 20);
			this.cmb_WiegandIn_Standard.TabIndex = 56;
			this.rdb_WiegandIn_Standard.BackColor = Color.FromArgb(194, 217, 247);
			this.rdb_WiegandIn_Standard.Checked = true;
			this.rdb_WiegandIn_Standard.FlatStyle = FlatStyle.System;
			this.rdb_WiegandIn_Standard.Location = new Point(18, 33);
			this.rdb_WiegandIn_Standard.Name = "rdb_WiegandIn_Standard";
			this.rdb_WiegandIn_Standard.Size = new Size(158, 16);
			this.rdb_WiegandIn_Standard.TabIndex = 55;
			this.rdb_WiegandIn_Standard.TabStop = true;
			this.rdb_WiegandIn_Standard.Text = "韦根格式";
			this.rdb_WiegandIn_Standard.TextImageRelation = TextImageRelation.TextAboveImage;
			this.rdb_WiegandIn_Standard.UseVisualStyleBackColor = true;
			this.rdb_WiegandIn_Standard.CheckedChanged += this.rdb_WiegandIn_Standard_CheckedChanged;
			this.rdb_weigandIn_AutoFit.BackColor = Color.FromArgb(194, 217, 247);
			this.rdb_weigandIn_AutoFit.FlatStyle = FlatStyle.System;
			this.rdb_weigandIn_AutoFit.Location = new Point(18, 57);
			this.rdb_weigandIn_AutoFit.Name = "rdb_weigandIn_AutoFit";
			this.rdb_weigandIn_AutoFit.Size = new Size(158, 16);
			this.rdb_weigandIn_AutoFit.TabIndex = 54;
			this.rdb_weigandIn_AutoFit.Text = "自动匹配韦根格式";
			this.rdb_weigandIn_AutoFit.TextImageRelation = TextImageRelation.TextAboveImage;
			this.rdb_weigandIn_AutoFit.UseVisualStyleBackColor = true;
			this.rdb_weigandIn_AutoFit.Visible = false;
			this.group_wiegand_Out.BackColor = Color.FromArgb(194, 217, 247);
			this.group_wiegand_Out.Controls.Add(this.cmb_wiegandOutType);
			this.group_wiegand_Out.Controls.Add(this.lab_wiegandOutType);
			this.group_wiegand_Out.Controls.Add(this.cmb_cust_wiegandOutfmt);
			this.group_wiegand_Out.Controls.Add(this.rdb_cust_wiegandOutfmt);
			this.group_wiegand_Out.Controls.Add(this.cmb_WiegandOut_Standard);
			this.group_wiegand_Out.Controls.Add(this.rdb_wiegandOut_Standard);
			this.group_wiegand_Out.Location = new Point(11, 126);
			this.group_wiegand_Out.Name = "group_wiegand_Out";
			this.group_wiegand_Out.Size = new Size(559, 88);
			this.group_wiegand_Out.TabIndex = 68;
			this.group_wiegand_Out.TabStop = false;
			this.group_wiegand_Out.Text = "韦根输出";
			this.cmb_wiegandOutType.DisplayMember = "Text";
			this.cmb_wiegandOutType.DrawMode = DrawMode.OwnerDrawFixed;
			this.cmb_wiegandOutType.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_wiegandOutType.Enabled = false;
			this.cmb_wiegandOutType.FormattingEnabled = true;
			this.cmb_wiegandOutType.ItemHeight = 15;
			this.cmb_wiegandOutType.Location = new Point(256, 80);
			this.cmb_wiegandOutType.Name = "cmb_wiegandOutType";
			this.cmb_wiegandOutType.Size = new Size(158, 21);
			this.cmb_wiegandOutType.Style = eDotNetBarStyle.StyleManagerControlled;
			this.cmb_wiegandOutType.TabIndex = 72;
			this.cmb_wiegandOutType.Visible = false;
			this.lab_wiegandOutType.BackColor = Color.Transparent;
			this.lab_wiegandOutType.Location = new Point(33, 87);
			this.lab_wiegandOutType.Name = "lab_wiegandOutType";
			this.lab_wiegandOutType.Size = new Size(178, 12);
			this.lab_wiegandOutType.TabIndex = 73;
			this.lab_wiegandOutType.Text = "韦根输出类型";
			this.lab_wiegandOutType.TextAlign = ContentAlignment.MiddleLeft;
			this.lab_wiegandOutType.Visible = false;
			this.cmb_cust_wiegandOutfmt.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_cust_wiegandOutfmt.FormattingEnabled = true;
			this.cmb_cust_wiegandOutfmt.Location = new Point(256, 57);
			this.cmb_cust_wiegandOutfmt.Name = "cmb_cust_wiegandOutfmt";
			this.cmb_cust_wiegandOutfmt.Size = new Size(279, 20);
			this.cmb_cust_wiegandOutfmt.TabIndex = 71;
			this.cmb_cust_wiegandOutfmt.Visible = false;
			this.rdb_cust_wiegandOutfmt.BackColor = Color.FromArgb(194, 217, 247);
			this.rdb_cust_wiegandOutfmt.FlatStyle = FlatStyle.System;
			this.rdb_cust_wiegandOutfmt.Location = new Point(19, 61);
			this.rdb_cust_wiegandOutfmt.Name = "rdb_cust_wiegandOutfmt";
			this.rdb_cust_wiegandOutfmt.Size = new Size(158, 16);
			this.rdb_cust_wiegandOutfmt.TabIndex = 70;
			this.rdb_cust_wiegandOutfmt.Text = "自定义格式";
			this.rdb_cust_wiegandOutfmt.TextImageRelation = TextImageRelation.TextAboveImage;
			this.rdb_cust_wiegandOutfmt.UseVisualStyleBackColor = true;
			this.rdb_cust_wiegandOutfmt.Visible = false;
			this.rdb_cust_wiegandOutfmt.CheckedChanged += this.rdb_cust_wiegandOutfmt_CheckedChanged;
			this.cmb_WiegandOut_Standard.DropDownStyle = ComboBoxStyle.DropDownList;
			this.cmb_WiegandOut_Standard.FormattingEnabled = true;
			this.cmb_WiegandOut_Standard.Location = new Point(256, 27);
			this.cmb_WiegandOut_Standard.Name = "cmb_WiegandOut_Standard";
			this.cmb_WiegandOut_Standard.Size = new Size(279, 20);
			this.cmb_WiegandOut_Standard.TabIndex = 69;
			this.rdb_wiegandOut_Standard.BackColor = Color.FromArgb(194, 217, 247);
			this.rdb_wiegandOut_Standard.Checked = true;
			this.rdb_wiegandOut_Standard.FlatStyle = FlatStyle.System;
			this.rdb_wiegandOut_Standard.Location = new Point(19, 31);
			this.rdb_wiegandOut_Standard.Name = "rdb_wiegandOut_Standard";
			this.rdb_wiegandOut_Standard.Size = new Size(158, 16);
			this.rdb_wiegandOut_Standard.TabIndex = 68;
			this.rdb_wiegandOut_Standard.TabStop = true;
			this.rdb_wiegandOut_Standard.Text = "韦根格式";
			this.rdb_wiegandOut_Standard.TextImageRelation = TextImageRelation.TextAboveImage;
			this.rdb_wiegandOut_Standard.UseVisualStyleBackColor = true;
			this.rdb_wiegandOut_Standard.CheckedChanged += this.rdb_wiegandOut_Standard_CheckedChanged;
			this.tabItem_Wiegand.AttachedControl = this.tabControlPanel1;
			this.tabItem_Wiegand.Name = "tabItem_Wiegand";
			this.tabItem_Wiegand.Text = "韦根设置";
			this.btn_Wiegand_cancel.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Wiegand_cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Wiegand_cancel.Location = new Point(479, 276);
			this.btn_Wiegand_cancel.Name = "btn_Wiegand_cancel";
			this.btn_Wiegand_cancel.Size = new Size(82, 23);
			this.btn_Wiegand_cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Wiegand_cancel.TabIndex = 25;
			this.btn_Wiegand_cancel.Text = "取消";
			this.btn_Wiegand_cancel.Click += this.btn_Wiegand_cancel_Click;
			this.btn_Wiegand_ok.AccessibleRole = AccessibleRole.PushButton;
			this.btn_Wiegand_ok.ColorTable = eButtonColor.OrangeWithBackground;
			this.btn_Wiegand_ok.Location = new Point(377, 276);
			this.btn_Wiegand_ok.Name = "btn_Wiegand_ok";
			this.btn_Wiegand_ok.Size = new Size(82, 23);
			this.btn_Wiegand_ok.Style = eDotNetBarStyle.StyleManagerControlled;
			this.btn_Wiegand_ok.TabIndex = 24;
			this.btn_Wiegand_ok.Text = "确定";
			this.btn_Wiegand_ok.Click += this.btn_Wiegand_ok_Click;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(580, 309);
			base.Controls.Add(this.btn_Wiegand_cancel);
			base.Controls.Add(this.btn_Wiegand_ok);
			base.Controls.Add(this.tab_WiegandParameters);
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "WiegandSeting";
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "韦根";
			((ISupportInitialize)this.tab_WiegandParameters).EndInit();
			this.tab_WiegandParameters.ResumeLayout(false);
			this.tabControlPanel2.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			((ISupportInitialize)this.gd_wigand).EndInit();
			((ISupportInitialize)this.gd_wigand_view).EndInit();
			((ISupportInitialize)this.bandedGridView1).EndInit();
			this.tabControlPanel1.ResumeLayout(false);
			this.group_wiegand_In.ResumeLayout(false);
			this.group_wiegand_Out.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
