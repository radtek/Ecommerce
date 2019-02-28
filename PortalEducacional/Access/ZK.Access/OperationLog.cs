/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;

namespace ZK.Access
{
	public class OperationLog
	{
		public const int add = 1;

		public const int delete = 2;

		public const int modify = 3;

		public const int operateDevice = 4;

		public const int systemManager = 5;

		public static ActionLogBll actionLogBll = new ActionLogBll(MainForm._ia);

		public static ActionLog actionLog = null;

		public static void SaveOperationLog(string changeMessage, int actionFlag, string objectRepr)
		{
			OperationLog.actionLog = new ActionLog();
			int content_type_id = 3;
			OperationLog.actionLog.action_time = DateTime.Now;
			OperationLog.actionLog.user_id = SysInfos.SysUserInfo.id;
			OperationLog.actionLog.content_type_id = content_type_id;
			OperationLog.actionLog.change_message = changeMessage;
			OperationLog.actionLog.action_flag = actionFlag;
			OperationLog.actionLog.object_repr = objectRepr;
			OperationLog.actionLogBll.Add(OperationLog.actionLog);
		}
	}
}
