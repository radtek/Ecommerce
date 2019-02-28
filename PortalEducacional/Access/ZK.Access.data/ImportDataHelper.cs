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
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using ZK.ConfigManager;
using ZK.Data.Model.PullSDK;
using ZK.Utils;

namespace ZK.Access.data
{
	public class ImportDataHelper
	{
		public delegate void ImportHandle(DataConfig config);

		public delegate object GetRowModelHandle();

		public delegate bool SaveModelHandle(object model);

		public delegate void DataLoadedHandler(DataTable dt);

		public delegate void ShowInfoHandle(string info);

		public delegate void ShowProgressHandle(int currProgress);

		private DataConfig m_config = null;

		private static Thread m_thread = null;

		public DataConfig Config => this.m_config;

		public event ImportHandle SetImportColumnsEvent;

		public event ImportHandle SaveImportDataEvent;

		public event ImportHandle CheckDataEvent;

		public event GetRowModelHandle GetRowModelEvent;

		public event SaveModelHandle SaveModelEvent;

		public event EventHandler FinishEvent;

		public event EventHandler ShowEvent;

		public event DataLoadedHandler DataLoaded;

		public event ShowInfoHandle ShowInfoEvent = null;

		public event ShowProgressHandle ShowProgressEvent = null;

		public ImportDataHelper()
		{
			this.m_config = new DataConfig(this);
		}

		private bool Eql(DataRow row1, DataRow row2, DataConfig config)
		{
			int num = 0;
			bool result;
			while (true)
			{
				if (num < config.SelectColumns.Count)
				{
					if (row1[config.SelectColumns[num]] != null && row2[config.SelectColumns[num]] != null)
					{
						if (row1[config.SelectColumns[num]].ToString() != row2[config.SelectColumns[num]].ToString())
						{
							result = false;
							break;
						}
						num++;
						continue;
					}
					return true;
				}
				return true;
			}
			return result;
		}

		private void CheckAndResetDataSource(DataConfig config)
		{
			if (config.DataSource != null && config.DataSource.Rows.Count > 1 && config.DataSource.Rows.Count <= 1000)
			{
				for (int i = 1; i < config.DataSource.Rows.Count; i++)
				{
					for (int j = 0; j < i; j++)
					{
						if (this.Eql(config.DataSource.Rows[i], config.DataSource.Rows[j], config))
						{
							config.DataSource.Rows.RemoveAt(i);
							i--;
						}
					}
				}
			}
		}

		public bool Set(DataConfig config)
		{
			ImportDataSet importDataSet = new ImportDataSet(config);
			importDataSet.ShowDialog();
			if (config.IsOk)
			{
				config.IsOk = false;
				switch (config.DataType)
				{
				case DataType.Excel:
					config.IsOk = this.ImportExcel();
					break;
				case DataType.Csv:
					config.IsOk = this.ImportCsv();
					break;
				}
				return config.IsOk;
			}
			return false;
		}

		public bool ColumnSet(DataConfig config)
		{
			config.SourceColumns.Clear();
			config.SelectColumns.Clear();
			config.ImportColumns.Clear();
			if (config.IsOk && config.DataSource != null && config.DataSource.Rows.Count > 0)
			{
				config.IsOk = false;
				for (int i = 0; i < config.DataSource.Columns.Count; i++)
				{
					if (config.DataSource.Columns[i].ColumnName != "check")
					{
						config.DataSource.Columns[i].ColumnName = config.DataSource.Columns[i].ColumnName.Trim();
						config.SourceColumns.Add(config.DataSource.Columns[i].ColumnName);
					}
				}
				DataSourceColumnsSet dataSourceColumnsSet = new DataSourceColumnsSet(config);
				dataSourceColumnsSet.ShowDialog();
				if (config.IsOk)
				{
					return true;
				}
				if (config.IsUp)
				{
					config.IsUp = false;
					if (this.Set(config))
					{
						return this.ColumnSet(config);
					}
				}
			}
			return false;
		}

		public bool SelectData(DataConfig config)
		{
			if (config.IsOk && config.SelectColumns.Count > 0)
			{
				config.IsOk = false;
				config.SelectDataSource = null;
				this.CheckAndResetDataSource(config);
				DataView dataView = new DataView(config);
				dataView.ShowDialog();
				if (config.IsOk)
				{
					return true;
				}
				if (config.IsUp)
				{
					config.IsUp = false;
					config.IsOk = true;
					if (this.ColumnSet(config))
					{
						return this.SelectData(config);
					}
				}
			}
			return false;
		}

		public bool SetImportColumns(DataConfig config)
		{
			if (config.IsOk && config.SelectDataSource != null && config.SelectDataSource.Rows.Count > 0)
			{
				config.IsOk = false;
				if (this.SetImportColumnsEvent != null)
				{
					config.ImportColumns.Clear();
					config.ColumnsToColumnsDic.Clear();
					config.ColumnsToModelDic.Clear();
					this.SetImportColumnsEvent(config);
					if (config.ImportColumns.Count > 0)
					{
						if (config.ImportColumns.Count <= config.SelectColumns.Count)
						{
							bool flag = true;
							List<string> list = new List<string>();
							for (int i = 0; i < config.SelectColumns.Count; i++)
							{
								list.Add(config.SelectColumns[i].ToString().Trim());
							}
							int num = 0;
							while (num < config.ImportColumns.Count)
							{
								if (list.Contains(config.ImportColumns[num]))
								{
									num++;
									continue;
								}
								flag = false;
								break;
							}
							if (flag)
							{
								config.ColumnsToColumnsDic.Clear();
								for (int j = 0; j < config.ImportColumns.Count; j++)
								{
									config.ColumnsToColumnsDic.Add(config.ImportColumns[j], config.ImportColumns[j]);
								}
								config.IsOk = true;
								return true;
							}
						}
						ImportColumnSet importColumnSet = new ImportColumnSet(config);
						importColumnSet.ShowDialog();
						if (config.IsOk)
						{
							return true;
						}
						if (config.IsUp)
						{
							config.IsUp = false;
							config.IsOk = true;
							if (this.SelectData(config))
							{
								return this.SetImportColumns(config);
							}
						}
					}
				}
			}
			return false;
		}

		public void OnDataLoaded(DataTable dt)
		{
			if (this.DataLoaded != null)
			{
				this.DataLoaded(dt);
			}
		}

		public void ShowInfo(string msg)
		{
			if (this.ShowInfoEvent != null)
			{
				this.ShowInfoEvent(msg);
			}
		}

		public void ShowProgress(int prg)
		{
			if (this.ShowProgressEvent != null)
			{
				this.ShowProgressEvent(prg);
			}
		}

		private void OnShow(object sender, EventArgs e)
		{
			if (this.ShowEvent != null)
			{
				this.ShowEvent(null, null);
			}
		}

		private void OnSaveStart()
		{
			int count = this.m_config.SelectDataSource.Rows.Count;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			int i;
			for (i = 0; i < count; i++)
			{
				object obj = this.GetRowModelEvent();
				if (obj != null)
				{
					try
					{
						this.InitModel(obj, this.m_config.SelectDataSource.Rows[i]);
						if (this.SaveModelEvent(obj))
						{
							if (count < 1000)
							{
								this.ShowInfo(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + (i + 1));
							}
							else
							{
								stringBuilder.Append(ShowMsgInfos.GetInfo("SIsSuccess", "成功") + ":" + (i + 1) + "\r\n");
								if (stringBuilder.Length > 1000)
								{
									this.ShowInfo(stringBuilder.ToString());
									stringBuilder = new StringBuilder();
								}
							}
						}
						else
						{
							num++;
							if (!string.IsNullOrEmpty(this.m_config.CheckErrorInfo))
							{
								this.ShowInfo(ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + (i + 1) + "  " + this.m_config.CheckErrorInfo);
								this.m_config.CheckErrorInfo = string.Empty;
							}
							else
							{
								this.ShowInfo(ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + (i + 1));
							}
						}
					}
					catch (Exception ex)
					{
						num++;
						this.ShowInfo(ex.Message);
					}
				}
				else
				{
					num++;
					this.ShowInfo(ShowMsgInfos.GetInfo("SIsFail", "失败") + ":" + (i + 1));
				}
				this.ShowProgress((i + 1) * 100 / count);
			}
			num2 = i - num;
			if (stringBuilder.Length > 0)
			{
				this.ShowInfo(stringBuilder.ToString());
				stringBuilder = new StringBuilder();
			}
			this.ShowProgress(100);
			string msg = ShowMsgInfos.GetInfo("SIsSuccess", "成功") + "：" + num2 + ShowMsgInfos.GetInfo("SUnit", "条") + " " + ShowMsgInfos.GetInfo("SIsFail", "失败") + "：" + num + ShowMsgInfos.GetInfo("SUnit", "条");
			this.ShowInfo(msg);
			ImportDataHelper.m_thread = null;
			this.FinishEvent(null, null);
		}

		private void OnFnish(object sender, EventArgs e)
		{
			if (this.FinishEvent != null)
			{
				this.FinishEvent(null, null);
			}
		}

		public void Start()
		{
			if (ImportDataHelper.m_thread != null)
			{
				string info = ShowMsgInfos.GetInfo("SWaiting", "请稍等，还有任务没有执行完毕");
				this.FinishEvent(info, null);
			}
			else
			{
				try
				{
					if (this.Set(this.m_config))
					{
						if (this.ColumnSet(this.m_config))
						{
							if (this.SelectData(this.m_config))
							{
								if (this.SetImportColumns(this.m_config))
								{
									if (this.m_config.IsOk && this.m_config.ColumnsToColumnsDic.Count > 0)
									{
										if (this.CheckDataEvent != null)
										{
											this.CheckDataEvent(this.m_config);
										}
										if (this.m_config.Check)
										{
											if (this.SaveImportDataEvent != null)
											{
												this.SaveImportDataEvent(this.m_config);
												this.FinishEvent(null, null);
											}
											else if (this.GetRowModelEvent != null && this.SaveModelEvent != null && this.m_config.ColumnsToModelDic.Count > 0 && this.m_config.SelectDataSource != null)
											{
												this.OnShow(this, null);
												if (this.m_config.SelectDataSource.Rows.Count > 1000)
												{
													ImportDataHelper.m_thread = new Thread(this.OnSaveStart);
													ImportDataHelper.m_thread.Start();
												}
												else
												{
													this.OnSaveStart();
												}
											}
										}
										else if (!string.IsNullOrEmpty(this.m_config.CheckErrorInfo))
										{
											SysDialogs.ShowWarningMessage(this.m_config.CheckErrorInfo);
											this.m_config.CheckErrorInfo = string.Empty;
											this.FinishEvent(null, null);
										}
										else
										{
											SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SDataCheckFail", "数据检查失败"));
											this.FinishEvent(null, null);
										}
									}
									else
									{
										this.FinishEvent(null, null);
									}
								}
								else
								{
									this.FinishEvent(null, null);
								}
							}
							else
							{
								this.FinishEvent(null, null);
							}
						}
						else
						{
							this.FinishEvent(null, null);
						}
					}
					else
					{
						this.FinishEvent(null, null);
					}
				}
				catch (Exception ex)
				{
					string sender = ShowMsgInfos.GetInfo("SIsFail", "失败") + "：" + ex.Message;
					this.FinishEvent(sender, null);
				}
			}
		}

		private void InitModel(object model, DataRow row)
		{
			try
			{
				if (model != null)
				{
					Type type = model.GetType();
					PropertyInfo[] properties = type.GetProperties();
					if (properties != null && properties.Length != 0)
					{
						foreach (PropertyInfo propertyInfo in properties)
						{
							foreach (KeyValuePair<string, string> item in this.m_config.ColumnsToModelDic)
							{
								if (item.Value.ToLower() == propertyInfo.Name.ToLower())
								{
									if (this.m_config.ColumnsToColumnsDic.ContainsKey(item.Key) && row[this.m_config.ColumnsToColumnsDic[item.Key]] != null)
									{
										string text = row[this.m_config.ColumnsToColumnsDic[item.Key]].ToString();
										if (!string.IsNullOrEmpty(text))
										{
											this.SetKValue(model, text, propertyInfo);
										}
									}
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ShowInfo(ex.Message);
			}
		}

		private void SetKValue(object info, string pvalue, PropertyInfo pi)
		{
			if (pi.PropertyType == typeof(int))
			{
				try
				{
					int num = int.Parse(pvalue);
					pi.SetValue(info, num, null);
				}
				catch
				{
				}
			}
			else if (pi.PropertyType == typeof(string))
			{
				pi.SetValue(info, pvalue, null);
			}
			else if (pi.PropertyType == typeof(DateTime))
			{
				try
				{
					DateTime dateTime = DateTime.Parse(pvalue);
					pi.SetValue(info, dateTime, null);
				}
				catch
				{
				}
			}
			else if (pi.PropertyType == typeof(DateTime?))
			{
				try
				{
					DateTime? nullable = (pvalue != null && !(pvalue.Trim() == "")) ? new DateTime?(DateTime.Parse(pvalue)) : null;
					pi.SetValue(info, nullable, null);
				}
				catch
				{
				}
			}
			else if (pi.PropertyType == typeof(EventType))
			{
				try
				{
					EventType eventType = (EventType)Enum.Parse(typeof(EventType), pvalue);
					pi.SetValue(info, eventType, null);
				}
				catch
				{
					pi.SetValue(info, EventType.Type0, null);
				}
			}
			else if (pi.PropertyType == typeof(FingerType))
			{
				try
				{
					FingerType fingerType = (FingerType)Enum.Parse(typeof(FingerType), pvalue);
					pi.SetValue(info, fingerType, null);
				}
				catch
				{
					pi.SetValue(info, FingerType.Type0, null);
				}
			}
			else if (pi.PropertyType == typeof(HolidayType))
			{
				try
				{
					HolidayType holidayType = (HolidayType)Enum.Parse(typeof(HolidayType), pvalue);
					pi.SetValue(info, holidayType, null);
				}
				catch
				{
					pi.SetValue(info, HolidayType.Type1, null);
				}
			}
			else if (pi.PropertyType == typeof(InAddrType))
			{
				try
				{
					InAddrType inAddrType = (InAddrType)Enum.Parse(typeof(InAddrType), pvalue);
					pi.SetValue(info, inAddrType, null);
				}
				catch
				{
					pi.SetValue(info, InAddrType.Type1, null);
				}
			}
			else if (pi.PropertyType == typeof(LoopType))
			{
				try
				{
					LoopType loopType = (LoopType)Enum.Parse(typeof(LoopType), pvalue);
					pi.SetValue(info, loopType, null);
				}
				catch
				{
					pi.SetValue(info, LoopType.Type1, null);
				}
			}
			else if (pi.PropertyType == typeof(OutAddrType))
			{
				try
				{
					OutAddrType outAddrType = (OutAddrType)Enum.Parse(typeof(OutAddrType), pvalue);
					pi.SetValue(info, outAddrType, null);
				}
				catch
				{
					pi.SetValue(info, OutAddrType.Type1, null);
				}
			}
			else if (pi.PropertyType == typeof(OutType))
			{
				try
				{
					OutType outType = (OutType)Enum.Parse(typeof(OutType), pvalue);
					pi.SetValue(info, outType, null);
				}
				catch
				{
					pi.SetValue(info, OutType.Type1, null);
				}
			}
			else if (pi.PropertyType == typeof(ValidType))
			{
				try
				{
					ValidType validType = (ValidType)Enum.Parse(typeof(ValidType), pvalue);
					pi.SetValue(info, validType, null);
				}
				catch
				{
					pi.SetValue(info, ValidType.Type1, null);
				}
			}
			else if (pi.PropertyType == typeof(VerifiedType))
			{
				try
				{
					VerifiedType verifiedType = (VerifiedType)Enum.Parse(typeof(VerifiedType), pvalue);
					pi.SetValue(info, verifiedType, null);
				}
				catch
				{
					pi.SetValue(info, VerifiedType.Type0, null);
				}
			}
		}

		private bool ImportCsv()
		{
			if (File.Exists(this.m_config.DataSourceUrl))
			{
				try
				{
					Encoding encoding = ImportDataHelper.GetEncoding(this.m_config.DataSourceUrl, Encoding.Default);
					StreamReader streamReader = new StreamReader(this.m_config.DataSourceUrl, encoding);
					string text = streamReader.ReadLine();
					string[] array = text.Split(this.m_config.DataSplit.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (array != null && array.Length == 1)
					{
						if (this.m_config.DataSplit == " ")
						{
							if (text.IndexOf("\t") > 0)
							{
								this.m_config.DataSplit = "\t";
								array = text.Split(this.m_config.DataSplit.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
							}
						}
						else if (this.m_config.DataSplit == "\t" && text.IndexOf(" ") > 0)
						{
							this.m_config.DataSplit = " ";
							array = text.Split(this.m_config.DataSplit.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
						}
					}
					if (array != null && array.Length != 0)
					{
						this.m_config.DataSource = new DataTable();
						for (int i = 0; i < array.Length; i++)
						{
							this.m_config.DataSource.Columns.Add(array[i]);
						}
						while (!streamReader.EndOfStream)
						{
							string text2 = streamReader.ReadLine();
							string[] array2 = text2.Split(this.m_config.DataSplit.ToCharArray());
							if (array2 != null && array2.Length >= this.m_config.DataSource.Columns.Count)
							{
								DataRow dataRow = this.m_config.DataSource.NewRow();
								for (int j = 0; j < this.m_config.DataSource.Columns.Count; j++)
								{
									dataRow[j] = array2[j];
								}
								this.m_config.DataSource.Rows.Add(dataRow);
							}
						}
						streamReader.Close();
						streamReader = null;
						return true;
					}
					streamReader.Close();
					streamReader = null;
				}
				catch (Exception ex)
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIsFail", "失败") + "：" + ex.Message);
				}
			}
			else
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "文件不存在"));
			}
			return false;
		}

		private bool ImportExcel()
		{
			try
			{
				if (File.Exists(this.m_config.DataSourceUrl))
				{
					FileInfo fileInfo = new FileInfo(this.m_config.DataSourceUrl);
					if (fileInfo.Extension.ToLower() == ".xls")
					{
						string text = "Provider=Microsoft.Jet.OleDb.4.0;Persist Security Info=true;Jet OLEDB:Database Password=pwd;Data Source=" + this.m_config.DataSourceUrl + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1' ";
						using (OleDbConnection oleDbConnection = new OleDbConnection(text))
						{
							oleDbConnection.Open();
							DataTable oleDbSchemaTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[4]
							{
								null,
								null,
								null,
								"TABLE"
							});
							if (oleDbSchemaTable != null && oleDbSchemaTable.Rows.Count > 0)
							{
								string text2 = text2 = "select * from [" + oleDbSchemaTable.Rows[0][2].ToString() + "] ";
								using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(text2, text))
								{
									DataSet dataSet = new DataSet();
									oleDbDataAdapter.Fill(dataSet, "table1");
									if (dataSet != null && dataSet.Tables.Count > 0)
									{
										this.m_config.DataSource = dataSet.Tables[0];
										return true;
									}
								}
							}
						}
					}
					else
					{
						SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileExtensionIsWrong", "文件格式不正确"));
					}
				}
				else
				{
					SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SFileNoExists", "文件不存在"));
				}
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SIsFail", "失败") + "：" + ex.Message);
			}
			return false;
		}

		public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
		{
			FileStream fileStream = null;
			Encoding result = defaultEncoding;
			try
			{
				fileStream = new FileStream(fileName, FileMode.Open);
				result = ImportDataHelper.GetEncoding(fileStream, defaultEncoding);
			}
			catch
			{
			}
			fileStream?.Close();
			return result;
		}

		public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
		{
			Encoding result = defaultEncoding;
			if (stream != null && stream.Length >= 2)
			{
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				byte b4 = 0;
				long offset = stream.Seek(0L, SeekOrigin.Begin);
				stream.Seek(0L, SeekOrigin.Begin);
				int value = stream.ReadByte();
				b = Convert.ToByte(value);
				b2 = Convert.ToByte(stream.ReadByte());
				if (stream.Length >= 3)
				{
					b3 = Convert.ToByte(stream.ReadByte());
				}
				if (stream.Length >= 4)
				{
					b4 = Convert.ToByte(stream.ReadByte());
				}
				if (b == 254 && b2 == 255)
				{
					result = Encoding.BigEndianUnicode;
				}
				else if (b == 255 && b2 == 254 && b3 != 255)
				{
					result = Encoding.Unicode;
				}
				else if (b == 239 && b2 == 187 && b3 == 191)
				{
					result = Encoding.UTF8;
				}
				else if (b2 == 0)
				{
					result = Encoding.Unicode;
				}
				stream.Seek(offset, SeekOrigin.Begin);
			}
			return result;
		}
	}
}
