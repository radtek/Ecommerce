/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using ZK.Data.BLL;
using ZK.Data.Model;

namespace ZK.Access
{
	public class GLOBAL
	{
		public static bool IsMonitorOwner = false;

		public static bool monitorKeepAliveEnabled = false;

		public static bool monitorPerformingUpdate = false;

		public static bool IsMonitorActive()
		{
			AttParamBll attParamBll = new AttParamBll(MainForm._ia);
			AttParam attParam = new AttParam();
			attParam.PARANAME = "MONITOR_KEEP_ALIVE";
			AttParam attParam2 = attParam;
			DateTime dateTime = DateTime.Now;
			attParam2.PARAVALUE = dateTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString();
			if (!attParamBll.Exists("MONITOR_KEEP_ALIVE"))
			{
				attParamBll.Add(attParam);
				return false;
			}
			attParam = attParamBll.GetModel("MONITOR_KEEP_ALIVE");
			dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
			DateTime value = dateTime.AddSeconds(double.Parse(attParam.PARAVALUE)).AddSeconds(20.0);
			dateTime = DateTime.Now;
			return dateTime.CompareTo(value) <= 0;
		}

		public static void resetMonitorKeepAlive()
		{
			AttParamBll attParamBll = new AttParamBll(MainForm._ia);
			AttParam attParam = new AttParam();
			attParam.PARANAME = "MONITOR_KEEP_ALIVE";
			attParam.PARAVALUE = "0";
			if (!attParamBll.Exists("MONITOR_KEEP_ALIVE"))
			{
				attParamBll.Add(attParam);
			}
			else
			{
				attParamBll.Update(attParam);
			}
		}

		public static void setMonitorKeepAlive()
		{
			if (GLOBAL.monitorKeepAliveEnabled)
			{
				AttParamBll attParamBll = new AttParamBll(MainForm._ia);
				AttParam attParam = new AttParam();
				attParam.PARANAME = "MONITOR_KEEP_ALIVE";
				attParam.PARAVALUE = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString();
				if (!attParamBll.Exists("MONITOR_KEEP_ALIVE"))
				{
					attParamBll.Add(attParam);
				}
				else
				{
					attParamBll.Update(attParam);
				}
			}
		}
	}
}
