/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)

using System;
using System.IO;
using ZK.ConfigManager;

namespace ZK.Access
{
	internal class Access
	{
		public void Create(string mdbPath)
		{
			if (File.Exists(mdbPath))
			{
				throw new Exception(ShowMsgInfos.GetInfo("STargetMdbIsExisted", "目标数据库已存在,无法创建"));
			}
			mdbPath = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd;Data Source=" + mdbPath;
			//CatalogClass catalogClass = new CatalogClass();
			//catalogClass.Create(mdbPath);
		}

		public void Compact(string mdbPath)
		{
			if (!File.Exists(mdbPath))
			{
				throw new Exception(ShowMsgInfos.GetInfo("STargetMdbDontNotExisted", "目标数据库不存在,无法压缩"));
			}
			DateTime now = DateTime.Now;
			int num = now.Year;
			string text = num.ToString();
			string str = text;
			now = DateTime.Now;
			num = now.Month;
			text = str + num.ToString();
			string str2 = text;
			now = DateTime.Now;
			num = now.Day;
			text = str2 + num.ToString();
			string str3 = text;
			now = DateTime.Now;
			num = now.Hour;
			text = str3 + num.ToString();
			string str4 = text;
			now = DateTime.Now;
			num = now.Minute;
			text = str4 + num.ToString();
			string str5 = text;
			now = DateTime.Now;
			num = now.Second;
			text = str5 + num.ToString() + ".bak";
			text = mdbPath.Substring(0, mdbPath.LastIndexOf("\\") + 1) + text;
			string destconnection = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd;Data Source=" + text;
			string sourceConnection = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd;Data Source=" + mdbPath;
			//JetEngineClass jetEngineClass = new JetEngineClass();
			//jetEngineClass.CompactDatabase(sourceConnection, destconnection);
			File.Copy(text, mdbPath, true);
			File.Delete(text);
		}

		public void Backup(string mdb1, string mdb2)
		{
			if (!File.Exists(mdb1))
			{
				throw new Exception(ShowMsgInfos.GetInfo("SSourceMdbIsNotExist", "源数据库不存在"));
			}
			try
			{
				File.Copy(mdb1, mdb2, true);
			}
			catch (IOException ex)
			{
				throw new Exception(ex.ToString());
			}
		}

		public void Recover(string mdb1, string mdb2)
		{
			if (!File.Exists(mdb1))
			{
				throw new Exception(ShowMsgInfos.GetInfo("SBakMdbIsNotExist", "备份数据库不存在"));
			}
			try
			{
				File.Copy(mdb1, mdb2, true);
			}
			catch (IOException ex)
			{
				throw new Exception(ex.ToString());
			}
		}
	}
}
