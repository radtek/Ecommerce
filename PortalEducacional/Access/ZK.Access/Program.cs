/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ZK.Access.device;
using ZK.Access.system;
using ZK.ConfigManager;
using ZK.Data.BLL;
using ZK.Data.BLL.PullSDK;
using ZK.Data.DBUtility;
using ZK.Data.Model;
using ZK.Data.OleDbDAL;
using ZK.Data.SqlDbDAL;
using ZK.Framework;
using ZK.MachinesManager;
using ZK.Utils;

namespace ZK.Access
{
	public static class Program
	{
		public static readonly string ApplicationFolder = Application.StartupPath;

		public static bool IsRestart = false;

		private static DLD dld = new DLD();

		private static MainForm uMain;

		private static IntPtr ptr_iVision = IntPtr.Zero;

		private static UserMainControl mainUserControl;

		[STAThread]
		private static void Main()
		{
			Program.IsRestart = false;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Program.InitSys();
				DatabaseUpdate.CheckAndUpdateDB();
				Program.CheckIsECardTongRegister();
				string str;
				switch (initLang.Lang.ToLower())
				{
				case "ru":
					str = "ru_RU";
					break;
				case "ara":
					str = "ar_AR";
					break;
				case "spa":
					str = "es_ES";
					break;
				default:
					str = "en_US";
					break;
				}
				string value = "[Startup Options]\r\nLanguage=" + str;
				string path = Application.StartupPath + "\\cfg.ini";
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				FileStream fileStream = new FileStream(path, FileMode.Create);
				StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.ASCII);
				streamWriter.Write(value);
				streamWriter.Close();
				fileStream.Close();
				streamWriter = null;
				fileStream = null;
				Login login = new Login();
				login.ShowDialog();
				if (SysInfos.SysUserInfo != null && SysInfos.SysUserInfo.id > 0)
				{
					Program.LoadMachines();
					Program.SetTheadCount();
					Program.SetAlarmFilePath();
					Application.Run(new MainForm());
					MonitorWatchdog.StopWatchdog();
					DeviceServers.Instance.Dispose();
					Program.Finish();
				}
			}
			catch (FileNotFoundException ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				SysDialogs.ShowWarningMessage(ex.StackTrace);
				Program.IsRestart = true;
			}
			catch (IOException ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
				SysDialogs.ShowWarningMessage(ex2.StackTrace);
				Program.IsRestart = true;
			}
			catch (Exception ex3)
			{
				SysDialogs.ShowWarningMessage(ex3.Message);
				SysDialogs.ShowWarningMessage(ex3.StackTrace);
				Program.IsRestart = true;
			}
			SysLogServer.WriteLog("system stop ", true);
			AppSite.Instance.Save();
			if (Program.IsRestart)
			{
				Application.Restart();
			}
			Program.KillPlrscagent();
		}

		private static void SetZKTimeDbPath()
		{
		}

		public static void KillPlrscagent()
		{
			Process[] processes = Process.GetProcesses();
			if (processes != null)
			{
				int num = 0;
				Process process;
				while (true)
				{
					if (num < processes.Length)
					{
						process = processes[num];
						if (!(process.ProcessName.ToLower() == "plrscagent.dll".ToLower()))
						{
							num++;
							continue;
						}
						break;
					}
					return;
				}
				try
				{
					process.Kill();
				}
				catch
				{
				}
			}
		}

		private static void SetTheadCount()
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("TheadListenDevCount");
			string nodeValueByName2 = AppSite.Instance.GetNodeValueByName("ReMonitorTime");
			if (!string.IsNullOrEmpty(nodeValueByName))
			{
				try
				{
					DevLogServer.TheadCount = int.Parse(nodeValueByName);
				}
				catch
				{
				}
			}
			if (!string.IsNullOrEmpty(nodeValueByName2))
			{
				try
				{
					if (int.Parse(nodeValueByName2) > 2)
					{
						DevLogServer.ReMonitorTime = int.Parse(nodeValueByName2);
					}
				}
				catch
				{
				}
			}
			if (DevLogServer.TheadCount < 0)
			{
				DevLogServer.TheadCount = 1;
			}
			AppSite.Instance.SetNodeValue("TheadListenDevCount", DevLogServer.TheadCount.ToString());
			AppSite.Instance.SetNodeValue("ReMonitorTime", DevLogServer.ReMonitorTime.ToString());
		}

		private static void SetAlarmFilePath()
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("AlarmFilePath");
			if (nodeValueByName != null && nodeValueByName.Trim() != "")
			{
				if (File.Exists(nodeValueByName))
				{
					MonitorWatchdog.AlarmFile = nodeValueByName;
				}
				else
				{
					MonitorWatchdog.AlarmFile = "";
				}
			}
			else
			{
				MonitorWatchdog.AlarmFile = Application.StartupPath + "\\sound\\alarm.mid";
				if (!File.Exists(MonitorWatchdog.AlarmFile))
				{
					MonitorWatchdog.AlarmFile = "";
				}
			}
			AppSite.Instance.SetNodeValue("AlarmFilePath", MonitorWatchdog.AlarmFile);
		}

		private static void Finish()
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("IsBackDB");
			if (!string.IsNullOrEmpty(nodeValueByName) && nodeValueByName == "1")
			{
				DataHelper.DbBack(false);
			}
		}

		private static void LoadMachines()
		{
			MachinesBll machinesBll = new MachinesBll(MainForm._ia);
			List<Machines> list = null;
			list = machinesBll.GetModelList("");
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Enabled)
					{
						DeviceServerBll deviceServer = DeviceServers.Instance.GetDeviceServer(list[i]);
						if (deviceServer != null)
						{
							deviceServer.IsNeedListen = false;
							if ((list[i].door_count == 0 || list[i].reader_count == 0) && deviceServer.DevInfo.DevSDKType != SDKType.StandaloneSDK)
							{
								deviceServer.IsAddOk = false;
							}
						}
					}
				}
			}
		}

		private static void InitSys()
		{
			bool isZkTitle = false;
			if (!bool.TryParse(AppSite.Instance.GetNodeValueByName("IsZkTitle"), out isZkTitle))
			{
				isZkTitle = true;
			}
			SysInfos.IsZkTitle = isZkTitle;
			AppSite.Instance.load();
			PullSDkErrorInfos.Load();
			PullSDKEventInfos.Load();
			SysDialogs.ErrorCaption = ShowMsgInfos.GetInfo("ErrorCaption", "错误");
			SysDialogs.InfoCaption = ShowMsgInfos.GetInfo("InfoCaption", "提示");
			SysDialogs.QueseCaption = ShowMsgInfos.GetInfo("QueseCaption", "咨询");
			SysDialogs.WarningCaption = ShowMsgInfos.GetInfo("WarningCaption", "警告");
			Program.AdjustAppSite();
			if (AppSite.Instance.DataType == DataType.Access)
			{
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				text3 = AppSite.Instance.GetNodeValueByName("ConnectionString");
				DbHelperOleDb.GetDatabaseAddress(text3, ref text, ref text2);
			}
			else if (AppSite.Instance.GetNodeValueByName("RebuildDataBase") == "1")
			{
				DataHelper.SqlCreateDB();
			}
			DataHelper.RestoreDataBase();
			SysInfos.DefaultFont = AppSite.Instance.GetFont();
			if (int.TryParse(AppSite.Instance.GetNodeValueByName("SkinOption"), out int skinOption))
			{
				SkinParameters.SkinOption = skinOption;
			}
			Program.InitSkinParameter();
			ServerApplication serverApplication = (ServerApplication)(MainForm._ia = new ServerApplication());
			Program.InitPlug(MainForm._ia);
			if (AppSite.Instance.GetNodeValueByName("Language") != "en")
			{
				DevExpressHelper.InitTranslate();
			}
			if (AppSite.Instance.GetNodeValueByName("Language") == "chs" || AppSite.Instance.GetNodeValueByName("Language") == "")
			{
				Font defaultFont = AppearanceObject.DefaultFont;
				try
				{
					AppearanceObject.DefaultFont = new Font("宋体", 10f);
				}
				catch
				{
					AppearanceObject.DefaultFont = defaultFont;
				}
			}
		}

		private static void InitPlug(IApplication ia)
		{
			ia.ConnectionString = AppSite.Instance.GetNodeValueByName("ConnectionString");
			if (AppSite.Instance.DataType == DataType.SqlServer)
			{
				SqlDalPlug sqlDalPlug = new SqlDalPlug();
				sqlDalPlug.Application = ia;
				sqlDalPlug.Load();
			}
			else
			{
				if (string.IsNullOrEmpty(ia.ConnectionString) || ia.ConnectionString.ToLower().IndexOf("microsoft.jet.oledb") == -1)
				{
					string text = Application.StartupPath + (Application.StartupPath.EndsWith("\\") ? "" : "\\") + "access.mdb";
					if (!File.Exists(text))
					{
						text = Application.StartupPath + (Application.StartupPath.EndsWith("\\") ? "" : "\\") + "access.mdb";
					}
					if (File.Exists(text))
					{
						ia.ConnectionString = "Provider = Microsoft.Jet.OleDb.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd; Data Source =" + text;
						AppSite.Instance.SetNodeValue("ConnectionString", ia.ConnectionString);
					}
				}
				AppSite.Instance.DataType = DataType.Access;
				OleDalPlug oleDalPlug = new OleDalPlug();
				oleDalPlug.Application = ia;
				oleDalPlug.Load();
			}
			MachinesManagerPlug machinesManagerPlug = new MachinesManagerPlug(SDKType.PullSDK);
			machinesManagerPlug.Application = ia;
			machinesManagerPlug.Load();
		}

		private static void AdjustAppSite()
		{
			string text = Path.GetDirectoryName(Application.ExecutablePath) + "\\appconfig.ini";
			if (File.Exists(text))
			{
				INIClass iNIClass = new INIClass(text);
				string text2 = iNIClass.IniReadValue("System", "data");
				string nodeValue = iNIClass.IniReadValue("System", "ConnectionString");
				string a = iNIClass.IniReadValue("System", "FirstRun");
				string text3 = iNIClass.IniReadValue("System", "Language");
				string text4 = iNIClass.IniReadValue("System", "BakDBPath");
				if (a == "1")
				{
					if (!string.IsNullOrEmpty(text4))
					{
						AppSite.Instance.SetNodeValue("BakDBPath", text4);
					}
					else
					{
						DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\bak");
						if (!directoryInfo.Exists)
						{
							directoryInfo.Create();
						}
						AppSite.Instance.SetNodeValue("BakDBPath", directoryInfo.FullName);
					}
					if (!string.IsNullOrEmpty(text3))
					{
						AppSite.Instance.SetNodeValue("Language", text3);
					}
					if (text2.ToLower() == "sqlserver")
					{
						AppSite.Instance.SetNodeValue("FirstRun", "0");
						AppSite.Instance.SetNodeValue("ConnectionString", nodeValue);
						AppSite.Instance.DataType = DataType.SqlServer;
					}
				}
				File.Delete(text);
			}
		}

		private static bool CheckMachineCount()
		{
			try
			{
				MachinesBll machinesBll = new MachinesBll(MainForm._ia);
				int num = 0;
				num = ((AccCommon.IsECardTong != 1) ? machinesBll.MachineCount("") : machinesBll.MachineCount(" AccFun = 255 "));
				if (AccCommon.IsECardTong == 1)
				{
					if (num > 30)
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMachineCountOverflow", "最大设备数量不能超过") + 30);
						return false;
					}
				}
				else if (num > 100)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SMachineCountOverflow", "最大设备数量不能超过") + 100);
					return false;
				}
				return true;
			}
			catch
			{
				return true;
			}
		}

		private static bool checkAuthorityLicLimits()
		{
			int num = 0;
			bool result = true;
			string empty = string.Empty;
			string empty2 = string.Empty;
			if (!Program.Check(ref num, ref empty, ref empty2) && (AppSite.Instance.GetNodeValueByName("ShowRegisterInfo") != "0" || num <= 0))
			{
				FrmRegister frmRegister = new FrmRegister(num, empty, empty2);
				frmRegister.ShowDialog();
				if (!frmRegister.IsRegister && num <= 0)
				{
					result = false;
				}
			}
			return result;
		}

		private static void CreateAppFile(byte[] buffer64, string fileName)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
				}
				byte[] array = new byte[buffer64.Length + buffer64.Length / 4 + 1];
				int num = 0;
				for (int i = 0; i < buffer64.Length; i++)
				{
					array[num] = buffer64[i];
					num++;
					if ((i + 1) % 4 == 0)
					{
						array[num] = 0;
						num++;
					}
				}
				FileStream fileStream = fileInfo.Create();
				fileStream.Write(array, 0, num);
				fileStream.Flush();
				fileStream.Close();
				fileStream = null;
			}
			catch
			{
			}
		}

		private static string GetAppFile(string fileName)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					string result = string.Empty;
					FileStream fileStream = File.OpenRead(fileName);
					byte[] array = SysFile.Read(fileStream);
					if (array != null && array.Length != 0)
					{
						byte[] array2 = new byte[array.Length];
						int num = 0;
						for (int i = 0; i < array.Length; i++)
						{
							if ((i + 1) % 5 != 0)
							{
								array2[num] = array[i];
								num++;
							}
						}
						result = Convert.ToBase64String(array2, 0, num);
					}
					fileStream.Close();
					fileStream = null;
					return result;
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		private static string GetCumputerID()
		{
			string text = string.Empty;
			try
			{
				ComputerInfos computerInfos = new ComputerInfos();
				text = computerInfos.GetCpuID();
				if (string.IsNullOrEmpty(text) || text == "unknow")
				{
					text = computerInfos.GetMacAddress();
				}
			}
			catch
			{
			}
			if (string.IsNullOrEmpty(text) || text == "unknow")
			{
				string text2 = Path.GetDirectoryName(Application.ExecutablePath) + "\\AppSite.dll";
				FileInfo fileInfo = new FileInfo(text2);
				DateTime dateTime;
				if (fileInfo.Exists)
				{
					FileStream fileStream = File.OpenRead(text2);
					byte[] array = SysFile.Read(fileStream);
					if (array != null && array.Length != 0)
					{
						string @string = Encoding.UTF8.GetString(array);
						try
						{
							dateTime = DateTime.Parse(@string);
							text = dateTime.ToString();
						}
						catch
						{
							dateTime = fileInfo.CreationTime;
							text = dateTime.ToString();
						}
					}
					else
					{
						dateTime = DateTime.Now;
						text = dateTime.ToString();
					}
					fileStream.Close();
					fileStream = null;
				}
				else
				{
					dateTime = DateTime.Now;
					text = dateTime.ToString();
					FileStream fileStream2 = fileInfo.Create();
					byte[] bytes = Encoding.UTF8.GetBytes(text);
					fileStream2.Write(bytes, 0, bytes.Length);
					fileStream2.Flush();
					fileStream2.Close();
					fileStream2 = null;
				}
			}
			return Rijndael.Instatnce.Encrypt(text);
		}

		private static DateTime GetCreateTime()
		{
			string empty = string.Empty;
			DateTime result = DateTime.Now;
			try
			{
				EditRegistryServer editRegistryServer = new EditRegistryServer();
				DateTime dateTime = DateTime.Now;
				string text = dateTime.ToString();
				empty = editRegistryServer.GetKeyValue("SOFTWARE", "date", text);
				if (string.IsNullOrEmpty(empty) || empty == text)
				{
					if (editRegistryServer.CreateKey("SOFTWARE", "date", text))
					{
						empty = text;
					}
					else
					{
						string text2 = Path.GetDirectoryName(Application.ExecutablePath) + "\\AppSite.dll";
						FileInfo fileInfo = new FileInfo(text2);
						if (fileInfo.Exists)
						{
							FileStream fileStream = File.OpenRead(text2);
							byte[] array = SysFile.Read(fileStream);
							if (array != null && array.Length != 0)
							{
								empty = Encoding.UTF8.GetString(array);
								try
								{
									result = DateTime.Parse(empty);
								}
								catch
								{
									dateTime = fileInfo.CreationTime;
									empty = dateTime.ToString();
								}
							}
							fileStream.Close();
							fileStream = null;
						}
						else
						{
							FileStream fileStream2 = fileInfo.Create();
							Encoding uTF = Encoding.UTF8;
							dateTime = DateTime.Now;
							byte[] bytes = uTF.GetBytes(dateTime.ToString());
							fileStream2.Write(bytes, 0, bytes.Length);
							fileStream2.Flush();
							fileStream2.Close();
							fileStream2 = null;
						}
					}
				}
				result = DateTime.Parse(empty);
			}
			catch
			{
			}
			return result;
		}

		private static string GetSN(string id)
		{
			try
			{
				byte[] array = Convert.FromBase64String(id);
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				for (num = 0; num < array.Length; num++)
				{
					int num2;
					if (num % 2 == 1)
					{
						if (array[num] > 192)
						{
							array[num] = (byte)(array[num] - 128);
						}
						else if (array[num] > 128)
						{
							array[num] = (byte)(array[num] - 64);
						}
						if (array[num] >= 65 && array[num] <= 90)
						{
							goto IL_0098;
						}
						if (array[num] >= 97 && array[num] <= 122)
						{
							goto IL_0098;
						}
						num2 = ((array[num] >= 48 && array[num] <= 57) ? 1 : 0);
						goto IL_0099;
					}
					continue;
					IL_0099:
					if (num2 != 0)
					{
						char value = (char)array[num];
						stringBuilder.Append(value);
						if (stringBuilder.Length == 25)
						{
							break;
						}
					}
					continue;
					IL_0098:
					num2 = 1;
					goto IL_0099;
				}
				while (stringBuilder.Length < 25)
				{
					for (num = 0; num < array.Length; num++)
					{
						int num3;
						if (num > 0 && num % 3 == 0)
						{
							array[num] = (byte)(array[num] + num);
							if (array[num] > 192)
							{
								array[num] = (byte)(array[num] - 128);
							}
							else if (array[num] > 128)
							{
								array[num] = (byte)(array[num] - 64);
							}
							if (array[num] >= 65 && array[num] <= 90)
							{
								goto IL_0174;
							}
							if (array[num] >= 97 && array[num] <= 122)
							{
								goto IL_0174;
							}
							num3 = ((array[num] >= 48 && array[num] <= 57) ? 1 : 0);
							goto IL_0175;
						}
						continue;
						IL_0175:
						if (num3 != 0)
						{
							char value2 = (char)array[num];
							stringBuilder.Append(value2);
							if (stringBuilder.Length == 25)
							{
								break;
							}
						}
						continue;
						IL_0174:
						num3 = 1;
						goto IL_0175;
					}
				}
				string text = stringBuilder.ToString().Substring(0, 5) + "-" + stringBuilder.ToString().Substring(5, 5) + "-" + stringBuilder.ToString().Substring(10, 5) + "-" + stringBuilder.ToString().Substring(15, 5) + "-" + stringBuilder.ToString().Substring(20, 5);
				return text.ToUpper();
			}
			catch
			{
				return string.Empty;
			}
		}

		private static bool Check(ref int days, ref string sid, ref string snid)
		{
			string nodeValueByName = AppSite.Instance.GetNodeValueByName("SN");
			string cumputerID = Program.GetCumputerID();
			string fileName = Path.GetDirectoryName(Application.ExecutablePath) + "\\App.dll";
			FileInfo fileInfo = new FileInfo(fileName);
			if (fileInfo.Exists)
			{
				string appFile = Program.GetAppFile(fileName);
				if (cumputerID != appFile)
				{
					byte[] buffer = Convert.FromBase64String(cumputerID);
					Program.CreateAppFile(buffer, fileName);
				}
			}
			else
			{
				byte[] buffer2 = Convert.FromBase64String(cumputerID);
				Program.CreateAppFile(buffer2, fileName);
			}
			sid = cumputerID;
			if (nodeValueByName != (snid = Program.GetSN(cumputerID)))
			{
				DateTime dateTime = fileInfo.CreationTime;
				DateTime createTime = Program.GetCreateTime();
				if (dateTime > createTime)
				{
					dateTime = createTime;
				}
				days = 30 - (DateTime.Now - dateTime).Days;
				if (days > 30)
				{
					days = 0;
				}
				if (days < 0)
				{
					days = 0;
				}
				return false;
			}
			return true;
		}

		private static void CheckIsECardTongRegister()
		{
			if (AccCommon.IsECardTong > 0)
			{
				string currentDirectory = Directory.GetCurrentDirectory();
				Program.dld.LoadDll("..\\ECardTongRegister.dll");
				object[] objArray_Parameter = new object[1]
				{
					0
				};
				Type[] typeArray_ParameterType = new Type[1]
				{
					typeof(int)
				};
				DLD.ModePass[] modePassArray_Parameter = new DLD.ModePass[1]
				{
					DLD.ModePass.ByValue
				};
				Type typeFromHandle = typeFromHandle = typeof(int);
				Program.dld.LoadFun("ECardTongRegisted");
				int num = int.Parse(Program.dld.Invoke(objArray_Parameter, typeArray_ParameterType, modePassArray_Parameter, typeFromHandle).ToString());
				Program.dld.LoadFun("ECardTongExpired");
				typeFromHandle = typeof(int);
				int num2 = int.Parse(Program.dld.Invoke(objArray_Parameter, typeArray_ParameterType, modePassArray_Parameter, typeFromHandle).ToString());
				if (num == 0 && num2 != 0)
				{
					SysDialogs.ShowWarningMessage("软件未注册，请联系供应商");
				}
				typeFromHandle = typeof(string);
				Program.dld.LoadFun("GetRegistedDeptName");
				string registCompany = Program.dld.Invoke(objArray_Parameter, typeArray_ParameterType, modePassArray_Parameter, typeFromHandle).ToString();
				AccCommon.ECardTongIsRegisted = num;
				AccCommon.ECardTongIsExpired = num2;
				if (num <= 0)
				{
					registCompany = "软件未注册，请联系供应商";
				}
				AccCommon.RegistCompany = registCompany;
				Program.dld.UnLoadDll();
			}
		}

		public static void IsRegistZKECardTong()
		{
			if (AccCommon.IsECardTong > 0 && AccCommon.ECardTongIsRegisted == 0 && AccCommon.ECardTongIsExpired != 0)
			{
				MessageBox.Show("软件未注册，请联系供应商", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private static void InitSkinParameter()
		{
			SkinParameters.EditBoxBackColor = Color.FromArgb(139, 175, 209);
			SkinParameters.FormBackColor = Color.FromArgb(64, 92, 116);
			SkinParameters.GridBackColor = Color.FromArgb(104, 145, 183);
			SkinParameters.GridLineColor = Color.FromArgb(224, 224, 224);
			SkinParameters.GridRowBackColor = Color.FromArgb(139, 200, 253);
			SkinParameters.MainPanelBackColor = Color.FromArgb(104, 145, 183);
			SkinParameters.TextColor = Color.FromArgb(139, 200, 253);
			SkinParameters.TitlePanelBackColor1 = Color.FromArgb(82, 124, 163);
			SkinParameters.TitlePanelBackColor2 = Color.FromArgb(64, 92, 116);
			SkinParameters.ToolBarBackColor1 = Color.FromArgb(82, 124, 163);
			SkinParameters.ToolBarBackColor2 = Color.FromArgb(64, 92, 116);
			SkinParameters.Tree_Grid_ForeColor = Color.FromArgb(0, 0, 0);
			SkinParameters.TreeMenuBackColor = Color.FromArgb(104, 145, 183);
		}

		public static void InitInfo()
		{
			Program.IsRestart = false;
			Application.EnableVisualStyles();
			try
			{
				Program.InitSys();
				SkinParameters.SkinOption = 1;
				DatabaseUpdate.CheckAndUpdateDB();
				Program.CheckIsECardTongRegister();
			}
			catch (FileNotFoundException ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				Program.IsRestart = true;
			}
			catch (IOException ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
				Program.IsRestart = true;
			}
			catch (Exception ex3)
			{
				SysDialogs.ShowWarningMessage(ex3.Message);
				Program.IsRestart = true;
			}
		}

		public static bool InitialByCpp(string UserId, string Password)
		{
			try
			{
				if (!LoginHelper.LoginIPC(UserId, Password))
				{
					return false;
				}
				return true;
			}
			catch (FileNotFoundException ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
			catch (IOException ex2)
			{
				SysDialogs.ShowWarningMessage(ex2.Message);
				return false;
			}
			catch (Exception ex3)
			{
				SysDialogs.ShowWarningMessage(ex3.Message);
				return false;
			}
		}

		public static void InitUserSys()
		{
			if (SysInfos.SysUserInfo != null && SysInfos.SysUserInfo.id > 0)
			{
				Program.LoadMachines();
				Program.SetTheadCount();
				Program.SetAlarmFilePath();
			}
		}

		public static void LoadMainFrom()
		{
			DevLogServer.OnStart();
			Program.uMain = new MainForm();
		}

		public static void CloseMainFrom()
		{
			Program.uMain.Close();
			DevLogServer.OnStop();
			DevLogServer.IsStop = false;
			DevLogServer.UnLockEx();
			DeviceServers.Instance.Dispose();
			Program.Finish();
			AppSite.Instance.Save();
			Program.KillPlrscagent();
		}

		public static int UpdateMachineParameters(int MachineId)
		{
			return DeviceHelper.UpdateMachineParameter(MachineId);
		}

		public static void SetiVisionHandle(int handle)
		{
			Program.ptr_iVision = new IntPtr(handle);
		}

		public static void SetMainControl(UserMainControl uc)
		{
			Program.mainUserControl = uc;
		}

		public static void SwitchPage(int PageId)
		{
			if (Program.mainUserControl != null && Enum.IsDefined(typeof(UserMainControl.PageIdEnum), PageId))
			{
				Program.mainUserControl.SwitchPage((UserMainControl.PageIdEnum)PageId);
			}
		}

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

		public static IntPtr SendMessage(uint Msg, int wParam, int lParam)
		{
			if (IntPtr.Zero == Program.ptr_iVision)
			{
				return IntPtr.Zero;
			}
			return Program.SendMessage(Program.ptr_iVision, Msg, wParam, lParam);
		}
	}
}
