/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System.Collections.Generic;
using System.Data;

namespace ZK.Access.data
{
	public class DataConfig
	{
		private ImportDataHelper _ImportDataHelper;

		private DataType m_dataType = DataType.None;

		private string m_dataSourceUrl = string.Empty;

		private string m_dataSourceUser = string.Empty;

		private string m_dataSourcePwd = string.Empty;

		private string m_dataSplit = ",";

		private string m_dataTableName = string.Empty;

		private bool m_check = true;

		private string m_checkErrorInfo = string.Empty;

		private bool m_isOk = false;

		private bool m_isUp = false;

		private List<string> m_sourceColumns = new List<string>();

		private List<string> m_selectColumns = new List<string>();

		private List<string> m_importColumns = new List<string>();

		private Dictionary<string, string> m_ColumnsToColumnsDic = new Dictionary<string, string>();

		private Dictionary<string, string> m_ColumnsToModelDic = new Dictionary<string, string>();

		public ImportDataHelper ImportDataHelper => this._ImportDataHelper;

		public DataType DataType
		{
			get
			{
				return this.m_dataType;
			}
			set
			{
				this.m_dataType = value;
			}
		}

		public string DataSourceUrl
		{
			get
			{
				return this.m_dataSourceUrl;
			}
			set
			{
				this.m_dataSourceUrl = value;
			}
		}

		public string DataSourceUser
		{
			get
			{
				return this.m_dataSourceUser;
			}
			set
			{
				this.m_dataSourceUser = value;
			}
		}

		public string DataSourcePwd
		{
			get
			{
				return this.m_dataSourcePwd;
			}
			set
			{
				this.m_dataSourcePwd = value;
			}
		}

		public string DataSplit
		{
			get
			{
				return this.m_dataSplit;
			}
			set
			{
				this.m_dataSplit = value;
			}
		}

		public string DataTableName
		{
			get
			{
				return this.m_dataTableName;
			}
			set
			{
				this.m_dataTableName = value;
			}
		}

		public bool Check
		{
			get
			{
				return this.m_check;
			}
			set
			{
				this.m_check = value;
			}
		}

		public string CheckErrorInfo
		{
			get
			{
				return this.m_checkErrorInfo;
			}
			set
			{
				this.m_checkErrorInfo = value;
			}
		}

		public bool IsOk
		{
			get
			{
				return this.m_isOk;
			}
			set
			{
				this.m_isOk = value;
			}
		}

		public bool IsUp
		{
			get
			{
				return this.m_isUp;
			}
			set
			{
				this.m_isUp = value;
			}
		}

		public List<string> SourceColumns => this.m_sourceColumns;

		public List<string> SelectColumns => this.m_selectColumns;

		public List<string> ImportColumns => this.m_importColumns;

		public Dictionary<string, string> ColumnsToColumnsDic => this.m_ColumnsToColumnsDic;

		public DataTable DataSource
		{
			get;
			set;
		}

		public DataTable SelectDataSource
		{
			get;
			set;
		}

		public object RowModel
		{
			get;
			set;
		}

		public Dictionary<string, string> ColumnsToModelDic => this.m_ColumnsToModelDic;

		public event ImportDataHelper.DataLoadedHandler DataLoaded
		{
			add
			{
				this._ImportDataHelper.DataLoaded += value;
			}
			remove
			{
				this._ImportDataHelper.DataLoaded -= value;
			}
		}

		public DataConfig(ImportDataHelper helper)
		{
			this._ImportDataHelper = helper;
		}

		public void OnDataLoaded(DataTable dt)
		{
			this._ImportDataHelper.OnDataLoaded(dt);
		}
	}
}
