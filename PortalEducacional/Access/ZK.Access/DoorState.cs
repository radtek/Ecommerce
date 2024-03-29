/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
namespace ZK.Access
{
	public enum DoorState
	{
		Disabled = -1,
		None,
		Connected,
		NoConnected,
		Alarm,
		OutTimeAlarm,
		NoDoorSensor,
		Open,
		Closed = 9,
		DoorCloseRelayOpen,
		DoorOpenRelayOpen,
		NoDoorSensorRelayOpen,
		NoDoorSensorRelayClose,
		DoorLocked = 51,
		NormalOpenOrRemoteOpen = 14,
		DoorClosedOnNormalOpenTimeOrRemoteOpen
	}
}
