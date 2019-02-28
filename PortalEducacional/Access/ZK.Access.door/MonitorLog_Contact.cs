/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevExpress.Xpo;
using System;
using ZK.ConfigManager;
using ZK.Data.Model;

namespace ZK.Access.door
{
	[Persistent("acc_monitor_log")]
	public class MonitorLog_Contact : XPLiteObject
	{
		[Key]
		[DisplayName("id")]
		public long id;

		[DisplayName("time")]
		public DateTime time;

		[DisplayName("pin")]
		public string pin;

		[DisplayName("card_no")]
		public string card_no;

		[DisplayName("device_id")]
		public int device_id;

		[DisplayName("设备名称")]
		public string device_name;

		[DisplayName("verified")]
		public int verified;

		[DisplayName("state")]
		public int state;

		[DisplayName("event_point_type")]
		public int event_point_type;

		[DisplayName("event_point_id")]
		public int event_point_id;

		[DisplayName("事件点")]
		public string event_point_name;

		[DisplayName("event_type")]
		public int event_type;

		[DisplayName("备注")]
		public string description;

		[DisplayName("时间")]
		public string CheckTime
		{
			get
			{
				return this.time.ToString();
			}
		}

		[DisplayName("人员编号")]
		public string UserPin
		{
			get
			{
				return (this.pin == "0") ? "" : this.pin;
			}
		}

		[DisplayName("卡号")]
		public string CardNo
		{
			get
			{
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					if (ulong.TryParse(this.card_no, out ulong num))
					{
						return (num == 0L) ? "" : num.ToString("X");
					}
					return "";
				}
				return (this.card_no == "0") ? "" : this.card_no;
			}
		}

		[DisplayName("验证方式")]
		public string VerifyType
		{
			get
			{
				if (PullSDKVerifyTypeInfos.GetDic().ContainsKey(this.verified))
				{
					return PullSDKVerifyTypeInfos.GetDic()[this.verified];
				}
				return "";
			}
		}

		[DisplayName("出入状态")]
		public string InOutState
		{
			get
			{
				if (InOutStateInfo.GetDic().ContainsKey(this.state))
				{
					return InOutStateInfo.GetDic()[this.state];
				}
				return "";
			}
		}

		[DisplayName("事件类型")]
		public string EventType
		{
			get
			{
				if (PullSDKEventInfos.GetDic().ContainsKey(this.event_type))
				{
					return PullSDKEventInfos.GetDic()[this.event_type];
				}
				return "";
			}
		}

		public MonitorLog_Contact(Session session)
			: base(session)
		{
		}
	}
}
