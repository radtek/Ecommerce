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
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.IDAL;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.Framework;
using ZK.MachinesManager;

namespace ZK.Access
{
	public class DeviceServers : IDisposable
	{
		private Dictionary<string, DeviceServerBll> m_serverlist = new Dictionary<string, DeviceServerBll>();

		private static DeviceServers m_instance = null;

		public int Count => this.m_serverlist.Count;

		public DeviceServerBll this[int index]
		{
			get
			{
				try
				{
					if (index >= 0 && index < this.m_serverlist.Count)
					{
						DeviceServerBll[] array = new DeviceServerBll[this.m_serverlist.Count];
						this.m_serverlist.Values.CopyTo(array, 0);
						return array[index];
					}
					return null;
				}
				catch
				{
					return null;
				}
			}
		}

		public static DeviceServers Instance
		{
			get
			{
				if (DeviceServers.m_instance == null)
				{
					DeviceServers.m_instance = new DeviceServers();
				}
				return DeviceServers.m_instance;
			}
		}

		public event EventHandler DeleteServerEvent = null;

		public event EventHandler AddServerEvent = null;

		public DeviceServers()
		{
			DeviceServers.m_instance = this;
		}

		public static void CheckCardBox(ObjRTLogInfo info)
		{
			if (info.EType == EventType.Type0)
			{
				IVisitInfo dALObject = VisitInfoBll.getDALObject();
				IAccDoor dALObject2 = AccDoorBll.getDALObject();
				if (dALObject2.isCardCollector(info.DevID, int.Parse(info.DoorID)))
				{
					MachinesBll machinesBll = new MachinesBll(MainForm._ia);
					PersonnelIssuecardBll personnelIssuecardBll = new PersonnelIssuecardBll(MainForm._ia);
					int visitorIDByCard = dALObject.GetVisitorIDByCard(info.CardNo);
					List<UserInfo> list = new List<UserInfo>();
					VisitInfo model = dALObject.GetModel(visitorIDByCard);
					UserInfo userInfo = new UserInfo();
					if (model != null)
					{
						int num = info.Date.CompareTo(model.createDate);
						if (num >= 0)
						{
							userInfo.BadgeNumber = model.pin;
							userInfo.CardNo = model.card;
							list.Add(userInfo);
							List<Machines> list2 = null;
							list2 = machinesBll.GetModelList("id in(select  device_id from acc_door where id in(select  accdoor_id from acc_levelset_door_group where acclevelset_id in(select acclevelset_id from acc_levelset_emp where employee_id=" + visitorIDByCard + ")))");
							for (int i = 0; i < DeviceServers.m_instance.Count; i++)
							{
								DeviceServerBll deviceServerBll = DeviceServers.m_instance[i];
								int num2 = 0;
								while (num2 < list2.Count)
								{
									if (deviceServerBll.DevInfo.ID != list2[num2].ID)
									{
										num2++;
										continue;
									}
									int num3 = deviceServerBll.DeleteDeviceData("user", "Pin=" + info.Pin, "");
									if (num3 >= 0)
									{
										dALObject.EndVisit(visitorIDByCard);
										personnelIssuecardBll.DeleteByUserId(visitorIDByCard);
									}
									else
									{
										DevCmds.RunImmediatelyEnabled = true;
										string text;
										if (list2[num2].DevSDKType != SDKType.StandaloneSDK)
										{
											CommandServer.DeleteFace(list2[num2], list, true, out text);
											CommandServer.DeleteFingerVein(list2[num2], list, true, out text);
											CommandServer.DeleteFingerPrint(list2[num2], list, true, out text);
											CommandServer.DeleteUserAuthorize(list2[num2], list, true, out text);
										}
										CommandServer.DeleteUser(list2[num2], list, true, out text);
										DevCmds.RunImmediatelyEnabled = false;
									}
									break;
								}
							}
						}
					}
				}
			}
		}

		public DeviceServerBll GetDeviceServer(ObjDevice dev)
		{
			if (dev == null)
			{
				return null;
			}
			string text = dev.ToString();
			if (text == null || text.Trim() == "")
			{
				return null;
			}
			if (this.m_serverlist.ContainsKey(text))
			{
				if (this.m_serverlist[text].DevInfo.DevSDKType == dev.DevSDKType)
				{
					this.m_serverlist[text].DevInfo.Area_id = dev.Area_id;
					this.m_serverlist[text].DevInfo.BatchUpdate = dev.BatchUpdate;
					this.m_serverlist[text].DevInfo.Baudrate = dev.Baudrate;
					this.m_serverlist[text].DevInfo.CardFun = dev.CardFun;
					this.m_serverlist[text].DevInfo.CommPassword = dev.CommPassword;
					this.m_serverlist[text].DevInfo.CompatOldFirmware = dev.CompatOldFirmware;
					this.m_serverlist[text].DevInfo.ConnectType = dev.ConnectType;
					this.m_serverlist[text].DevInfo.DeviceName = dev.DeviceName;
					this.m_serverlist[text].DevInfo.DeviceType = dev.DeviceType;
					this.m_serverlist[text].DevInfo.DevSDKType = dev.DevSDKType;
					this.m_serverlist[text].DevInfo.DSTime_id = dev.DSTime_id;
					this.m_serverlist[text].DevInfo.Enabled = dev.Enabled;
					this.m_serverlist[text].DevInfo.FaceFunOn = dev.FaceFunOn;
					this.m_serverlist[text].DevInfo.FingerFunOn = dev.FingerFunOn;
					this.m_serverlist[text].DevInfo.FPVersion = dev.FPVersion;
					this.m_serverlist[text].DevInfo.FvFunOn = dev.FvFunOn;
					this.m_serverlist[text].DevInfo.ID = dev.ID;
					this.m_serverlist[text].DevInfo.IP = dev.IP;
					this.m_serverlist[text].DevInfo.IsTFTMachine = dev.IsTFTMachine;
					this.m_serverlist[text].DevInfo.MachineAlias = dev.MachineAlias;
					this.m_serverlist[text].DevInfo.MachineNumber = dev.MachineNumber;
					this.m_serverlist[text].DevInfo.Platform = dev.Platform;
					this.m_serverlist[text].DevInfo.Port = dev.Port;
					this.m_serverlist[text].DevInfo.SerialNumber = dev.SerialNumber;
					this.m_serverlist[text].DevInfo.SerialPort = dev.SerialPort;
					this.m_serverlist[text].DevInfo.simpleEventType = dev.simpleEventType;
					this.m_serverlist[text].DevInfo.UserExtFmt = dev.UserExtFmt;
					this.m_serverlist[text].DevInfo.ZKFPVersion10 = (dev.FPVersion == 10);
				}
				else
				{
					ServerApplication serverApplication = new ServerApplication();
					MachinesManagerPlug machinesManagerPlug = new MachinesManagerPlug(dev.DevSDKType);
					machinesManagerPlug.Application = serverApplication;
					machinesManagerPlug.Load();
					DeviceServerBll deviceServerBll = new DeviceServerBll(serverApplication);
					deviceServerBll.InitDevice(dev);
					this.m_serverlist[text] = deviceServerBll;
				}
				return this.m_serverlist[text];
			}
			ServerApplication serverApplication2 = new ServerApplication();
			MachinesManagerPlug machinesManagerPlug2 = new MachinesManagerPlug(dev.DevSDKType);
			machinesManagerPlug2.Application = serverApplication2;
			machinesManagerPlug2.Load();
			DeviceServerBll deviceServerBll2 = new DeviceServerBll(serverApplication2);
			deviceServerBll2.InitDevice(dev);
			this.m_serverlist.Add(text, deviceServerBll2);
			if (this.AddServerEvent != null)
			{
				this.AddServerEvent(deviceServerBll2, null);
			}
			return deviceServerBll2;
		}

		public DeviceServerBll GetDeviceServer(Machines dev)
		{
			if (dev?.Enabled ?? false)
			{
				return this.GetDeviceServer(dev.ToDeviceModel());
			}
			return null;
		}

		private DeviceServerBll GetDeviceServer(string key)
		{
			if (this.m_serverlist.ContainsKey(key))
			{
				return this.m_serverlist[key];
			}
			return null;
		}

		private void DelDeviceServer(string key)
		{
			if (!string.IsNullOrEmpty(key) && this.m_serverlist.ContainsKey(key))
			{
				this.m_serverlist[key].IsDel = true;
				this.m_serverlist[key].Disconnect();
				this.m_serverlist.Remove(key);
			}
		}

		public void DelDeviceServer(ObjDevice dev)
		{
			if (dev != null)
			{
				this.DelDeviceServer(dev.ToString());
			}
		}

		public void DelDeviceServer(Machines dev)
		{
			if (dev != null)
			{
				if (dev.ConnectType == 1)
				{
					this.DelDeviceServer(dev.IP.ToLower() + ":" + dev.Port);
				}
				else
				{
					int num = dev.SerialPort;
					string str = num.ToString();
					num = dev.MachineNumber;
					this.DelDeviceServer(str + "-" + num.ToString());
				}
			}
		}

		public void Dispose()
		{
			if (this.m_serverlist.Count > 0)
			{
				try
				{
					foreach (KeyValuePair<string, DeviceServerBll> item in this.m_serverlist)
					{
						if (item.Value != null)
						{
							item.Value.IsDel = true;
							item.Value.Disconnect();
						}
					}
					this.m_serverlist.Clear();
				}
				catch
				{
				}
			}
		}
	}
}
