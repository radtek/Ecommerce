using System;
using System.Collections.Generic;
using ZK.Data.BLL.PullSDK;
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
        public static IApplication _ia = null;
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
