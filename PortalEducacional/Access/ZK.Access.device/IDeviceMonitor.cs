/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Collections.Generic;
using ZK.Access.door;
using ZK.Data.BLL.PullSDK;

namespace ZK.Access.device
{
	public interface IDeviceMonitor : IDisposable
	{
		bool IsMonitoring
		{
			get;
		}

		int SerialPort
		{
			get;
		}

		int ServerCount
		{
			get;
		}

		int MonitorInterval
		{
			get;
			set;
		}

		int SYNCTimeInterval
		{
			get;
			set;
		}

		void ProcessOfflineEvents();

		void ProcessOfflineEvents(DeviceServerBll devServer);

		void StartMonitor();

		void StopMonitor();

		void AddDeviceServer(DeviceServerBll devServer);

		void RemoveDeviceServer(DeviceServerBll devServer);

		void ClearDeviceServer();

		void ClearOldRTEvent();

		bool Contains(DeviceServerBll devServer);

		void DisConnectAll();

		void OpenDoors(List<DevControl> lstDoorDev, OpenDoorSet frmOpenDoor, WaitForm frmWait, out int SucceedCount, out int FailedCount);

		void CloseDoor(List<DevControl> lstDoorDev, CloseDoorSet frmCloseDoor, WaitForm frmWait, out int SucceedCount, out int FailedCount);

		void CancelAlarm(List<DevControl> lstDoorDev, WaitForm frmWait, out int SucceedCount, out int FailedCount);
	}
}
