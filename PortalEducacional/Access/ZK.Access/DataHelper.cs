/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.DBUtility;
using ZK.Utils;

namespace ZK.Access
{
	internal class DataHelper
	{
		public static bool ISChinese(string strWord)
		{
			string pattern = "[\\u4e00-\\u9fa5]";
			return Regex.IsMatch(strWord, pattern);
		}

		public static void DbBack(bool prompt)
		{
			try
			{
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("BakDBPath");
				if (!string.IsNullOrEmpty(nodeValueByName))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(nodeValueByName);
					if (!directoryInfo.Exists)
					{
						try
						{
							directoryInfo.Create();
						}
						catch
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNotPath", "该路径不存在") + ":" + nodeValueByName);
							return;
						}
					}
					if (AppSite.Instance.DataType == DataType.Access)
					{
						DataHelper.AccessDB(prompt, directoryInfo);
					}
					else if (AppSite.Instance.DataType == DataType.SqlServer)
					{
						string nodeValueByName2 = AppSite.Instance.GetNodeValueByName("ConnectionString");
						if (!string.IsNullOrEmpty(nodeValueByName2))
						{
							DataHelper.SqlDB(prompt, directoryInfo, nodeValueByName2);
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("DBConnectionStringNull", "数据库连接参数不能为空"));
						}
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("dataBackupPathNull", "数据库备份路径不能为空"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public static void RestoreDataBase()
		{
			try
			{
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("IsRestoreDB");
				string nodeValueByName2 = AppSite.Instance.GetNodeValueByName("RestoreDBPath");
				string nodeValueByName3 = AppSite.Instance.GetNodeValueByName("RestoreDBPwd");
				string nodeValueByName4 = AppSite.Instance.GetNodeValueByName("ConnectionString");
				if (!string.IsNullOrEmpty(nodeValueByName) && nodeValueByName == "1")
				{
					if (!string.IsNullOrEmpty(nodeValueByName2))
					{
						if (File.Exists(nodeValueByName2))
						{
							if (AppSite.Instance.DataType == DataType.Access)
							{
								DataHelper.AccessDR(nodeValueByName2, nodeValueByName3);
								AppSite.Instance.Save();
							}
							else if (AppSite.Instance.DataType == DataType.SqlServer)
							{
								if (!string.IsNullOrEmpty(nodeValueByName4))
								{
									DataHelper.SqlDR(nodeValueByName2, nodeValueByName4);
									AppSite.Instance.Save();
								}
								else
								{
									SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("DBConnectionStringNull", "数据库连接参数不能为空"));
								}
							}
						}
						else
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("DBBackupFileNotExists", "数据库备份文件不存在"));
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("RestoreDBPathNull", "数据库还原路径不能为空"));
					}
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public static void AccessDB(bool prompt, DirectoryInfo dir)
		{
			try
			{
				if (dir?.Exists ?? false)
				{
					string connectionString = MainForm._ia.ConnectionString;
					string text = string.Empty;
					if (string.IsNullOrEmpty(connectionString))
					{
						text = Application.StartupPath + "\\Access.mdb";
					}
					else
					{
						DataHelper.GetAccessConnectStringInfo(connectionString, ref text);
					}
					if (!File.Exists(text))
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SDBFileNotExist", "原数据库不存在，无法备份"));
					}
					else
					{
						object[] obj = new object[9]
						{
							dir.FullName,
							"\\Access",
							null,
							null,
							null,
							null,
							null,
							null,
							null
						};
						DateTime now = DateTime.Now;
						obj[2] = now.Year;
						now = DateTime.Now;
						int num = now.Month;
						obj[3] = num.ToString("00");
						now = DateTime.Now;
						num = now.Day;
						obj[4] = num.ToString("00");
						now = DateTime.Now;
						num = now.Hour;
						obj[5] = num.ToString("00");
						now = DateTime.Now;
						num = now.Minute;
						obj[6] = num.ToString("00");
						now = DateTime.Now;
						obj[7] = now.Second;
						obj[8] = ".mdb";
						string destFileName = string.Concat(obj);
						File.Copy(text, destFileName, true);
						OperationLog.SaveOperationLog(ShowMsgInfos.GetInfo("SBackDB", "数据库备份"), 5, "system");
						if (prompt)
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "," + ShowMsgInfos.GetInfo("SBackPath", "备份路径为:") + dir.FullName);
						}
					}
				}
				else if (dir != null & prompt)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SNotPath", "该路径不存在") + ":" + dir.FullName);
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
			}
		}

		public static void GetAccessConnectStringInfo(string dbConnectionString, ref string dataBaseName)
		{
			if (!string.IsNullOrEmpty(dbConnectionString))
			{
				string[] array = dbConnectionString.Split(';');
				if (array != null)
				{
					string[] array2 = array;
					int num = 0;
					string[] array3;
					while (true)
					{
						if (num < array2.Length)
						{
							string text = array2[num];
							if (!string.IsNullOrEmpty(text))
							{
								array3 = text.Split('=');
								if (array3 != null && array3.Length == 2 && array3[0].ToLower().IndexOf("source") >= 0)
								{
									break;
								}
							}
							num++;
							continue;
						}
						return;
					}
					dataBaseName = array3[1];
				}
			}
		}

		public static void GetDataConnectStringInfo(string dbConnectionString, ref string user, ref string pwd, ref string dataBaseName, ref string dataBaseHost)
		{
			if (!string.IsNullOrEmpty(dbConnectionString))
			{
				string[] array = dbConnectionString.Split(';');
				if (array != null)
				{
					string[] array2 = array;
					foreach (string text in array2)
					{
						if (!string.IsNullOrEmpty(text))
						{
							string[] array3 = text.Split('=');
							if (array3 != null && array3.Length == 2)
							{
								if (array3[0].ToLower().IndexOf("id") >= 0)
								{
									user = array3[1];
								}
								if (array3[0].ToLower().IndexOf("password") >= 0)
								{
									pwd = array3[1];
								}
								if (array3[0].ToLower().IndexOf("catalog") >= 0)
								{
									dataBaseName = array3[1];
								}
								if (array3[0].ToLower().IndexOf("database") >= 0)
								{
									dataBaseName = array3[1];
								}
								if (array3[0].ToLower().IndexOf("source") >= 0)
								{
									dataBaseHost = array3[1];
								}
							}
						}
					}
				}
			}
		}

		public static void SqlDB(bool prompt, DirectoryInfo dir, string dbConnectionString)
		{
			try
			{
				if (!string.IsNullOrEmpty(dbConnectionString) && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SSqlServerDataback", "Microsoft SQL Server 备份只能备份本机数据库，确认继续吗?"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					string empty = string.Empty;
					string empty2 = string.Empty;
					string empty3 = string.Empty;
					string empty4 = string.Empty;
					if (!Directory.Exists(dir.FullName))
					{
						Directory.CreateDirectory(dir.FullName);
					}
					DataHelper.GetDataConnectStringInfo(dbConnectionString, ref empty, ref empty2, ref empty3, ref empty4);
					if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty3) && !string.IsNullOrEmpty(empty2))
					{
						try
						{
							object[] obj = new object[9]
							{
								dir.FullName,
								"\\Access",
								null,
								null,
								null,
								null,
								null,
								null,
								null
							};
							DateTime now = DateTime.Now;
							obj[2] = now.Year;
							now = DateTime.Now;
							int num = now.Month;
							obj[3] = num.ToString("00");
							now = DateTime.Now;
							num = now.Day;
							obj[4] = num.ToString("00");
							now = DateTime.Now;
							num = now.Hour;
							obj[5] = num.ToString("00");
							now = DateTime.Now;
							num = now.Minute;
							obj[6] = num.ToString("00");
							now = DateTime.Now;
							obj[7] = now.Second;
							obj[8] = ".bak";
							string text = string.Concat(obj);
							string text2 = $"backup database [{empty3}] to disk = '{text}' with checksum";
							DbHelperSQL.BackupDataBase(text, 240);
							if (File.Exists(text))
							{
								if (prompt)
								{
									SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功") + "," + ShowMsgInfos.GetInfo("SBackPath", "备份路径为:") + dir.FullName);
								}
							}
							else
							{
								SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SOperationSqlFailed", "对不起，操作失败，可能原因:数据库不是本机数据库"));
							}
						}
						catch (Exception ex)
						{
							if (prompt)
							{
								SysDialogs.ShowErrorMessage(ex.ToString());
							}
						}
					}
					else if (prompt)
					{
						SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SDataError", "数据库参数错误"));
					}
				}
			}
			catch (Exception ex2)
			{
				if (prompt)
				{
					SysDialogs.ShowWarningMessage(ex2.Message);
				}
			}
		}

		public static void AccessDR(string fileName, string pwd)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
				{
					if (!Directory.Exists(Application.StartupPath + "\\dbbak"))
					{
						Directory.CreateDirectory(Application.StartupPath + "\\dbbak");
					}
					string nodeValueByName = AppSite.Instance.GetNodeValueByName("ConnectionString");
					string text = string.Empty;
					if (string.IsNullOrEmpty(nodeValueByName))
					{
						text = Application.StartupPath + "\\Access.mdb";
					}
					else
					{
						DataHelper.GetAccessConnectStringInfo(nodeValueByName, ref text);
						if (string.IsNullOrEmpty(text))
						{
							text = Application.StartupPath + "\\Access.mdb";
						}
					}
					if (File.Exists(text))
					{
						string sourceFileName = text;
						object[] obj = new object[8]
						{
							Application.StartupPath,
							"\\dbbak\\Access",
							null,
							null,
							null,
							null,
							null,
							null
						};
						DateTime now = DateTime.Now;
						obj[2] = now.Year;
						now = DateTime.Now;
						int num = now.Month;
						obj[3] = num.ToString("00");
						now = DateTime.Now;
						num = now.Day;
						obj[4] = num.ToString("00");
						now = DateTime.Now;
						num = now.Hour;
						obj[5] = num.ToString("00");
						now = DateTime.Now;
						num = now.Minute;
						obj[6] = num.ToString("00");
						obj[7] = ".mdb";
						File.Copy(sourceFileName, string.Concat(obj), true);
					}
					if (fileName.ToLower() != (Application.StartupPath + "\\Access.mdb").ToLower())
					{
						File.Copy(fileName, Application.StartupPath + "\\Access.mdb", true);
						nodeValueByName = "Provider = Microsoft.Jet.OleDb.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd; Data Source =" + Application.StartupPath + "\\Access.mdb;";
						if (!string.IsNullOrEmpty(pwd))
						{
							nodeValueByName = nodeValueByName.Replace("pwd", pwd);
						}
						AppSite.Instance.SetNodeValue("ConnectionString", nodeValueByName);
						AppSite.Instance.SetNodeValue("IsRestoreDB", "0");
						AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
					}
					else
					{
						nodeValueByName = "Provider = Microsoft.Jet.OleDb.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd; Data Source =" + Application.StartupPath + "\\Access.mdb;";
						if (!string.IsNullOrEmpty(pwd))
						{
							nodeValueByName = nodeValueByName.Replace("pwd", pwd);
						}
						AppSite.Instance.SetNodeValue("ConnectionString", nodeValueByName);
						AppSite.Instance.SetNodeValue("IsRestoreDB", "0");
						AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "目标文件不存在"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SErrorInformation", "对不起，操作失败，可能原因") + ":" + ex.Message);
			}
		}

		public static void SqlDR(string fileName, string connectionString)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName) && !string.IsNullOrEmpty(connectionString) && SysDialogs.ShowQueseMessage(ShowMsgInfos.GetInfo("SSqlServerDataRestore", "Microsoft SQL Server 还原只能还原本机数据库，确认继续吗"), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					string empty = string.Empty;
					string empty2 = string.Empty;
					string empty3 = string.Empty;
					string empty4 = string.Empty;
					bool isIntegratedSecurity = false;
					DbHelperSQL.GetDatabaseAddress(connectionString, out isIntegratedSecurity, out empty4, out empty3, out empty, out empty2);
					if (!string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(empty3) && !string.IsNullOrEmpty(empty2))
					{
						try
						{
							string text = "";
							string text2 = "";
							string text3 = "";
							string text4 = "";
							string cmdText = "";
							string connectionString2 = DbHelperSQL.CreateConnectionString(empty4, "master", empty, empty2, isIntegratedSecurity);
							using (SqlConnection connection = new SqlConnection(connectionString2))
							{
								SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
								SqlDataAdapter sda = new SqlDataAdapter(sqlCommand);
								DataHelper.GetBackFileInfo(fileName, ref text3, ref text4, sda);
								if (string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text4))
								{
									throw new Exception(ShowMsgInfos.GetInfo("SInvalidBackupFile", "无效的备份文件"));
								}
								DataHelper.GetDbInfo(empty3, ref text, ref text2, sda);
								if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
								{
									throw new Exception(ShowMsgInfos.GetInfo("SGetDbInfoFailed", "获取数据库信息失败"));
								}
								object[] args = new object[6]
								{
									empty3,
									fileName,
									text3,
									text,
									text4,
									text2
								};
								cmdText = (sqlCommand.CommandText = string.Format("restore database [{0}] from disk='{1}' with replace,move '{2}' to '{3}',move '{4}' to '{5}',replace", args));
								sqlCommand.Connection.Open();
								sqlCommand.ExecuteNonQuery();
								sqlCommand.Connection.Close();
								AppSite.Instance.SetNodeValue("NeedSync2ZKTime", "1");
							}
						}
						catch (Exception ex)
						{
							SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SErrorInformation", "对不起，操作失败，可能原因") + ":" + ex.ToString());
						}
						AppSite.Instance.SetNodeValue("IsRestoreDB", "0");
						AppSite.Instance.SetNodeValue("RestoreDBPath", "");
					}
					else
					{
						SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SDataError", "数据库参数错误"));
					}
				}
			}
			catch (Exception ex2)
			{
				SysDialogs.ShowErrorMessage(ShowMsgInfos.GetInfo("SErrorInformation", "对不起，操作失败，可能原因") + ":" + ex2.Message);
			}
		}

		private static void GetDbInfo(string strDBName, ref string strDataFileName, ref string strLogFileName, SqlDataAdapter sda)
		{
			DataTable dataTable = new DataTable();
			sda.SelectCommand.CommandText = $"Select * from [{strDBName}]..sysfiles";
			sda.Fill(dataTable);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (dataTable.Rows[i]["filename"].ToString().ToLower().EndsWith(".mdf"))
				{
					strDataFileName = dataTable.Rows[i]["filename"].ToString();
				}
				else if (dataTable.Rows[i]["filename"].ToString().ToLower().EndsWith(".ldf"))
				{
					strLogFileName = dataTable.Rows[i]["filename"].ToString();
				}
				if (!string.IsNullOrEmpty(strDataFileName) && !string.IsNullOrEmpty(strLogFileName))
				{
					break;
				}
			}
		}

		private static void GetBackFileInfo(string strFileName, ref string strLogicalDataName, ref string strLogicalLogName, SqlDataAdapter sda)
		{
			DataTable dataTable = new DataTable();
			sda.SelectCommand.CommandText = $"RESTORE FILELISTONLY FROM DISK = '{strFileName}'";
			sda.Fill(dataTable);
			for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
			{
				if (dataTable.Rows[num]["Type"].ToString().ToUpper() == "D")
				{
					strLogicalDataName = dataTable.Rows[num]["LogicalName"].ToString();
				}
				else if (dataTable.Rows[num]["Type"].ToString().ToUpper() == "L")
				{
					strLogicalLogName = dataTable.Rows[num]["LogicalName"].ToString();
				}
				if (!string.IsNullOrEmpty(strLogicalDataName) && !string.IsNullOrEmpty(strLogicalLogName))
				{
					break;
				}
			}
		}

		public static void SqlCreateDB()
		{
			if (File.Exists("access.sql"))
			{
				string nodeValueByName = AppSite.Instance.GetNodeValueByName("ConnectionString");
				DbHelperSQL.GetDatabaseAddress(nodeValueByName, out bool _, out string text, out string text2, out string text3, out string text4);
				if (text != null && !(text.Trim() == "") && text2 != null && !(text2.Trim() == "") && text3 != null && !(text3.Trim() == ""))
				{
					string format = "Data Source={0};Database={1};Password={2};User ID={3};";
					using (SqlConnection sqlConnection = new SqlConnection(string.Format(format, text, "master", text4, text3)))
					{
						try
						{
							sqlConnection.Open();
							using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
							{
								try
								{
									sqlCommand.CommandText = "drop database " + text2;
									sqlCommand.ExecuteNonQuery();
								}
								catch (Exception)
								{
								}
								sqlCommand.CommandText = "create database " + text2;
								sqlCommand.ExecuteNonQuery();
								sqlConnection.Close();
								sqlConnection.ConnectionString = nodeValueByName;
								sqlConnection.Open();
								StreamReader streamReader = new StreamReader("access.sql", Encoding.GetEncoding("GBK"));
								sqlCommand.CommandText = streamReader.ReadToEnd();
								sqlCommand.ExecuteNonQuery();
								AppSite.Instance.SetNodeValue("RebuildDataBase", "0");
							}
						}
						catch (Exception)
						{
						}
						finally
						{
							sqlConnection.Close();
						}
					}
				}
			}
		}
	}
}
