/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevComponents.DotNetBar;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Management;
using System.Windows.Forms;

namespace ZK.Access
{
	public class FrmModuleRegister : Office2007Form
	{
		private IContainer components = null;

		public FrmModuleRegister()
		{
			this.InitializeComponent();
		}

		private string getUniqueID(string drive)
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
			if (drive.EndsWith(":\\"))
			{
				drive = drive.Substring(0, drive.Length - 2);
			}
			string volumeSerial = this.getVolumeSerial(drive);
			string cPUID = this.getCPUID();
			return cPUID.Substring(13) + cPUID.Substring(1, 4) + volumeSerial + cPUID.Substring(4, 4);
		}

		private string getVolumeSerial(string drive)
		{
			ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"" + drive + ":\"");
			managementObject.Get();
			string result = ((ManagementBaseObject)managementObject)["VolumeSerialNumber"].ToString();
			managementObject.Dispose();
			return result;
		}

		private string getCPUID()
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

		private void FrmModuleRegister_Load(object sender, EventArgs e)
		{
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
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(585, 231);
			base.Name = "FrmModuleRegister";
			this.Text = "ZK Teco";
			base.Load += this.FrmModuleRegister_Load;
			base.ResumeLayout(false);
		}
	}
}
