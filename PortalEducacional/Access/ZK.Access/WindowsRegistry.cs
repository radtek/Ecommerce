/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using Microsoft.Win32;

namespace ZK.Access
{
	internal class WindowsRegistry
	{
		public static bool keyExists(string path)
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path, true);
			return registryKey != null;
		}

		public static RegistryKey createKey(string path)
		{
			return Registry.LocalMachine.CreateSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree);
		}

		public static RegistryKey openKey(string path)
		{
			return Registry.LocalMachine.OpenSubKey(path, true);
		}

		public static void setValue(RegistryKey key, string name, string value)
		{
			key.SetValue(name, value, RegistryValueKind.String);
		}

		public static string getValue(RegistryKey key, string name)
		{
			return key.GetValue(name).ToString();
		}
	}
}
