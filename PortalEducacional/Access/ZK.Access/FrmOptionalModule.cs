/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.Model;

namespace ZK.Access
{
	public class FrmOptionalModule : Office2007Form
	{
		public static AttParamBll attParamBll = new AttParamBll(MainForm._ia);

		public static AttParam attParam = null;

		private IContainer components = null;

		private ButtonX Btn_Cancel;

		private Label label1;

		private TextBox textBox1;

		private Label label2;

		private TextBox textBox2;

		private ButtonX buttonX1;

		public FrmOptionalModule()
		{
			this.InitializeComponent();
			initLang.LocaleForm(this, base.Name);
			this.textBox1.Text = FrmOptionalModule.getUID();
			this.textBox2.Text = FrmOptionalModule.GetRegKey();
		}

		private void AboutForm_Load(object sender, EventArgs e)
		{
		}

		private void Btn_Cancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		public static string getUID()
		{
			byte[] volumeSerial = FrmOptionalModule.getVolumeSerial("");
			byte[] macAddress = FrmOptionalModule.GetMacAddress();
			byte[] array = new byte[8];
			byte[] array2 = new byte[8];
			byte[] array3 = new byte[8];
			Buffer.BlockCopy(volumeSerial, 0, array, 0, volumeSerial.Length);
			Buffer.BlockCopy(macAddress, 0, array2, 0, macAddress.Length);
			ulong value = BitConverter.ToUInt64(array, 0);
			ulong value2 = BitConverter.ToUInt64(array2, 0);
			decimal d = value;
			d += (decimal)value2;
			ulong value3 = (ulong)d;
			byte[] bytes = BitConverter.GetBytes(value3);
			Buffer.BlockCopy(bytes, 0, array3, 0, 8);
			return FrmOptionalModule.ByteArrayToString(array3).ToUpper();
		}

		public static byte[] getVolumeSerial(string drive)
		{
			if (drive == string.Empty)
			{
				DriveInfo[] drives = DriveInfo.GetDrives();
				foreach (DriveInfo driveInfo in drives)
				{
					if (driveInfo.IsReady)
					{
						drive = driveInfo.RootDirectory.ToString();
						break;
					}
				}
			}
			ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive.Substring(0, 1) + ":\"");
			managementObject.Get();
			string hex = ((ManagementBaseObject)managementObject)["VolumeSerialNumber"].ToString();
			managementObject.Dispose();
			return FrmOptionalModule.StringToByteArray(hex);
		}

		public static byte[] getHDSerial()
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
			foreach (ManagementObject item in managementObjectSearcher.Get())
			{
				if (((ManagementBaseObject)item)["SerialNumber"] != null)
				{
					return FrmOptionalModule.StringToByteArray(((ManagementBaseObject)item)["SerialNumber"].ToString());
				}
			}
			return null;
		}

		public static string getCPUID()
		{
			string text = "";
			ManagementClass managementClass = new ManagementClass("win32_processor");
			ManagementObjectCollection instances = managementClass.GetInstances();
			foreach (ManagementObject item in instances)
			{
				if (text == "")
				{
					text = item.Properties["processorID"].Value.ToString();
					break;
				}
			}
			return text;
		}

		public static byte[] GetMacAddress()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
				{
					return networkInterface.GetPhysicalAddress().GetAddressBytes();
				}
			}
			return null;
		}

		public static byte[] StringToByteArray(string hex)
		{
			int length = hex.Length;
			byte[] array = new byte[length / 2];
			for (int i = 0; i < length; i += 2)
			{
				array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return array;
		}

		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		private void buttonX1_Click(object sender, EventArgs e)
		{
			FrmOptionalModule.SetRegKey(this.textBox2.Text);
			base.Close();
		}

		public static void SetRegKey(string key)
		{
			RegistryKey key2 = WindowsRegistry.keyExists("SOFTWARE\\ZKTeco\\ZKAccess3.5") ? WindowsRegistry.openKey("SOFTWARE\\ZKTeco\\ZKAccess3.5") : WindowsRegistry.createKey("SOFTWARE\\ZKTeco\\ZKAccess3.5");
			WindowsRegistry.setValue(key2, "RegistrationCode", key);
		}

		public static string GetRegKey()
		{
			if (!WindowsRegistry.keyExists("SOFTWARE\\ZKTeco\\ZKAccess3.5"))
			{
				return "";
			}
			RegistryKey key = WindowsRegistry.openKey("SOFTWARE\\ZKTeco\\ZKAccess3.5");
			string value = WindowsRegistry.getValue(key, "RegistrationCode");
			if (value == null)
			{
				return "";
			}
			return value;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FrmOptionalModule));
			this.Btn_Cancel = new ButtonX();
			this.label1 = new Label();
			this.textBox1 = new TextBox();
			this.label2 = new Label();
			this.textBox2 = new TextBox();
			this.buttonX1 = new ButtonX();
			base.SuspendLayout();
			this.Btn_Cancel.AccessibleRole = AccessibleRole.PushButton;
			this.Btn_Cancel.ColorTable = eButtonColor.OrangeWithBackground;
			this.Btn_Cancel.Location = new Point(315, 133);
			this.Btn_Cancel.Name = "Btn_Cancel";
			this.Btn_Cancel.Size = new Size(82, 25);
			this.Btn_Cancel.Style = eDotNetBarStyle.StyleManagerControlled;
			this.Btn_Cancel.TabIndex = 43;
			this.Btn_Cancel.Text = "Fechar";
			this.Btn_Cancel.Click += this.Btn_Cancel_Click;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new Size(117, 13);
			this.label1.TabIndex = 44;
			this.label1.Text = "Chave de Identificação";
			this.textBox1.Location = new Point(15, 25);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new Size(382, 20);
			this.textBox1.TabIndex = 45;
			this.label2.AutoSize = true;
			this.label2.Location = new Point(12, 61);
			this.label2.Name = "label2";
			this.label2.Size = new Size(95, 13);
			this.label2.TabIndex = 46;
			this.label2.Text = "Chave de Registro";
			this.textBox2.Location = new Point(15, 77);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new Size(382, 20);
			this.textBox2.TabIndex = 47;
			this.buttonX1.AccessibleRole = AccessibleRole.PushButton;
			this.buttonX1.ColorTable = eButtonColor.OrangeWithBackground;
			this.buttonX1.Location = new Point(227, 134);
			this.buttonX1.Name = "buttonX1";
			this.buttonX1.Size = new Size(82, 25);
			this.buttonX1.Style = eDotNetBarStyle.StyleManagerControlled;
			this.buttonX1.TabIndex = 48;
			this.buttonX1.Text = "Registrar";
			this.buttonX1.Click += this.buttonX1_Click;
			base.AcceptButton = this.Btn_Cancel;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(409, 171);
			base.Controls.Add(this.buttonX1);
			base.Controls.Add(this.textBox2);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.textBox1);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.Btn_Cancel);
			base.FormBorderStyle = FormBorderStyle.FixedSingle;
			base.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FrmOptionalModule";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "ZK TECO";
			base.Load += this.AboutForm_Load;
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
