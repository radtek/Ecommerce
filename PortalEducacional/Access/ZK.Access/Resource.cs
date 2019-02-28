/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ZK.Access
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resource
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resource.resourceMan == null)
				{
					ResourceManager resourceManager = Resource.resourceMan = new ResourceManager("ZK.Access.Resource", typeof(Resource).Assembly);
				}
				return Resource.resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resource.resourceCulture;
			}
			set
			{
				Resource.resourceCulture = value;
			}
		}

		internal static Bitmap _checked
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("_checked", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap background
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("background", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap clear_antyback
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("clear_antyback", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap close
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("close", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap close1
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("close1", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap closeMove
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("closeMove", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap device
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("device", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap device_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("device_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap device1
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("device1", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_alarm
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_alarm", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_closed
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_closed", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_default
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_default", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_disabled
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_disabled", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_locked
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_locked", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_nosensor
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_nosensor", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_offline
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_offline", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_open_timeout
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_open_timeout", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_opened
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_opened", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap door_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("door_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap DoorClosed_Pull
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("DoorClosed_Pull", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap DoorOpened_Pull
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("DoorOpened_Pull", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap error
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("error", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap error1
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("error1", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap exit
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("exit", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap holidays
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("holidays", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap holidays_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("holidays_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap IDCard
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("IDCard", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap level
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("level", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap level_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("level_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap load
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("load", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap loadpage
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("loadpage", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap LoginChs
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("LoginChs", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap LoginChs_ZK
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("LoginChs_ZK", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap LoginEn
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("LoginEn", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap LoginEn_System
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("LoginEn_System", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap LoginEn_ZK
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("LoginEn_ZK", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap LoginEn_ZK_System
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("LoginEn_ZK_System", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap monitoring
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("monitoring", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap monitoring_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("monitoring_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap personnel
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("personnel", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap personnel_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("personnel_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap punchCard
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("punchCard", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap Reader
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("Reader", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap Relays_open
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("Relays_open", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap RelaysClosed
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("RelaysClosed", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap RelaysOpend
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("RelaysOpend", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap reports
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("reports", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap reports_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("reports_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap StopPunchCard
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("StopPunchCard", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap thread
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("thread", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap time
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("time", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap time_s
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("time_s", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap uncheck
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("uncheck", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap UnLock
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("UnLock", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap UnLockAll
		{
			get
			{
				object @object = Resource.ResourceManager.GetObject("UnLockAll", Resource.resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resource()
		{
		}
	}
}
