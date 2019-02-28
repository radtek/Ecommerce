/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZK.ConfigManager;
using ZK.Data.Model;
using ZK.Utils;

namespace ZK.Access
{
	public class DevExpressHelper
	{
		private static SolidBrush _fontbrush = null;

		public static SolidBrush FontBrush
		{
			get
			{
				if (DevExpressHelper._fontbrush == null)
				{
					DevExpressHelper._fontbrush = new SolidBrush(Color.White);
				}
				return DevExpressHelper._fontbrush;
			}
			set
			{
				DevExpressHelper._fontbrush = value;
			}
		}

		public static void CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e, string columnName)
		{
			GridView gridView = sender as GridView;
			if (gridView != null && e.Column != null && e.Column.Name.ToLower() == columnName.ToLower())
			{
				e.Column.Caption = string.Empty;
				e.Info.Caption = string.Empty;
				e.Column.ImageAlignment = StringAlignment.Center;
				e.Column.AppearanceHeader.Options.UseTextOptions = false;
				if (e.Column.Width != 55)
				{
					e.Column.Width = 55;
				}
				if (gridView.RowCount == 0)
				{
					gridView.Tag = false;
					int num = 0;
					while (true)
					{
						if (num < gridView.Columns.Count)
						{
							if (!(gridView.Columns[num].FieldName == "check"))
							{
								num++;
								continue;
							}
							break;
						}
						return;
					}
					if (gridView.Columns[num].ImageIndex != 1)
					{
						gridView.Columns[num].ImageIndex = 1;
						gridView.RefreshData();
					}
				}
			}
		}

		public static void InitImageList(object sender, string columnName)
		{
			GridView gridView = sender as GridView;
			gridView.OptionsView.ShowGroupPanel = false;
			if (gridView != null)
			{
				ImageList imageList = null;
				if (gridView.Images == null)
				{
					imageList = (ImageList)(gridView.Images = new ImageList());
				}
				else
				{
					imageList = (gridView.Images as ImageList);
					if (imageList == null)
					{
						imageList = (ImageList)(gridView.Images = new ImageList());
					}
				}
				gridView.OptionsBehavior.Editable = false;
				gridView.OptionsSelection.MultiSelect = false;
				if (imageList.Images.Count == 0)
				{
					imageList.Images.Add(Resource._checked);
					imageList.Images.Add(Resource.uncheck);
					int num = 0;
					while (true)
					{
						if (num < gridView.Columns.Count)
						{
							Application.DoEvents();
							if (!(gridView.Columns[num].Name.ToLower() == columnName.ToLower()))
							{
								num++;
								continue;
							}
							break;
						}
						return;
					}
					gridView.Columns[num].ImageIndex = 1;
					gridView.Columns[num].ImageAlignment = StringAlignment.Center;
				}
			}
		}

		public static void CustomDrawCell(object sender, RowCellCustomDrawEventArgs e, string columnName)
		{
			GridView gridView = sender as GridView;
			if (gridView != null && e != null && e.Column != null && e.Column.Name.ToLower() == columnName.ToLower() && e.RowHandle >= 0 && e.RowHandle < gridView.RowCount)
			{
				Graphics graphics = e.Graphics;
				Rectangle bounds = e.Bounds;
				Rectangle destRect = default(Rectangle);
				destRect.Y = bounds.Top + 1;
				destRect.Height = bounds.Height - 2;
				destRect.Width = bounds.Width - 2;
				if (destRect.Width > destRect.Height)
				{
					destRect.Width = destRect.Height;
				}
				destRect.X = 1 + (bounds.Left + bounds.Right) / 2 - destRect.Width / 2;
				string text = e.CellValue.ToString();
				if (text.ToLower() == "true")
				{
					graphics.DrawImage(Resource._checked, destRect, new Rectangle(0, 0, 13, 13), GraphicsUnit.Pixel);
				}
				else
				{
					graphics.DrawImage(Resource.uncheck, destRect, new Rectangle(0, 0, 13, 13), GraphicsUnit.Pixel);
				}
				e.Handled = true;
			}
		}

		public static int[] GetCheckedRows(object sender, string fieldName)
		{
			List<int> list = new List<int>();
			GridView gridView = sender as GridView;
			if (gridView != null)
			{
				DataTable dataTable = gridView.GridControl.DataSource as DataTable;
				if (dataTable != null)
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.Rows[i];
						if (dataRow[fieldName] != null)
						{
							string text = dataRow[fieldName].ToString();
							if (text.ToLower() == "true")
							{
								list.Add(i);
							}
						}
					}
				}
			}
			if (list.Count > 0)
			{
				int[] array = new int[list.Count];
				for (int j = 0; j < list.Count; j++)
				{
					array[j] = list[j];
				}
				return array;
			}
			return null;
		}

		public static void CheckStatus(GridView gridView, string fieldName)
		{
			DataTable dataTable = gridView.GridControl.DataSource as DataTable;
			if (dataTable != null)
			{
				bool flag = true;
				DataRow[] array = dataTable.Select(gridView.RowFilter ?? "");
				int num = 0;
				while (num < array.Length)
				{
					if (!(array[num][fieldName].ToString().ToLower() == "false"))
					{
						num++;
						continue;
					}
					flag = false;
					break;
				}
				int num2 = 0;
				while (num2 < gridView.Columns.Count)
				{
					if (!(gridView.Columns[num2].FieldName.ToLower() == fieldName.ToLower()))
					{
						num2++;
						continue;
					}
					gridView.Columns[num2].ImageIndex = ((!flag) ? 1 : 0);
					gridView.RefreshData();
					break;
				}
				gridView.Tag = flag;
			}
		}

		public static bool ClickGridCheckBox(object sender, string fieldName)
		{
			GridView gridView = sender as GridView;
			bool result = false;
			if (gridView != null)
			{
				gridView.PostEditor();
				Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
				GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
				if (gridHitInfo != null && gridHitInfo.InColumn && gridHitInfo.Column != null && gridHitInfo.Column.FieldName == fieldName)
				{
					DevExpressHelper.InitImageList(sender, gridHitInfo.Column.Name);
					DataTable dataTable = gridView.GridControl.DataSource as DataTable;
					string rowFilter = gridView.RowFilter;
					DataRow[] array;
					if (rowFilter != "")
					{
						array = dataTable.Select(rowFilter);
					}
					else
					{
						array = new DataRow[dataTable.Rows.Count];
						dataTable.Rows.CopyTo(array, 0);
					}
					if (dataTable != null && gridHitInfo.RowHandle < 0)
					{
						bool flag = false;
						if (gridView.Tag != null)
						{
							flag = (gridView.Tag.ToString().ToLower() == "true" && true);
						}
						DataRow[] array2 = array;
						foreach (DataRow dataRow in array2)
						{
							dataRow[fieldName] = !flag;
						}
						gridView.Tag = !flag;
						if (!flag)
						{
							gridHitInfo.Column.ImageIndex = 0;
						}
						else
						{
							gridHitInfo.Column.ImageIndex = 1;
						}
						result = true;
					}
				}
				else if (gridHitInfo != null && gridHitInfo.InRowCell && gridHitInfo.Column != null && gridHitInfo.RowHandle >= 0)
				{
					DataTable dataTable2 = gridView.GridControl.DataSource as DataTable;
					if (dataTable2 != null)
					{
						DataRow dataRow2 = gridView.GetDataRow(gridHitInfo.RowHandle);
						if (dataRow2 != null)
						{
							object obj = dataRow2[fieldName];
							if (obj != null)
							{
								if (obj.ToString().ToLower() == "true")
								{
									dataRow2[fieldName] = "false";
								}
								else
								{
									dataRow2[fieldName] = "true";
								}
							}
						}
						DevExpressHelper.CheckStatus(gridView, fieldName);
					}
				}
			}
			return result;
		}

		public static int[] GetDataSourceRowIndexs(GridView gv, int[] rows)
		{
			if (rows != null && rows.Length != 0)
			{
				int[] array = new int[rows.Length];
				for (int i = 0; i < rows.Length; i++)
				{
					int dataSourceRowIndex = gv.GetDataSourceRowIndex(rows[i]);
					if (dataSourceRowIndex >= 0)
					{
						array[i] = dataSourceRowIndex;
					}
					else
					{
						array[i] = 0;
					}
				}
				return array;
			}
			return null;
		}

		public static void OutData(GridView gv, string fileName)
		{
			if (gv != null)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.FilterIndex = 0;
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.CreatePrompt = true;
				saveFileDialog.FileName = fileName;
				saveFileDialog.Title = ShowMsgInfos.GetInfo("SExportFilePath", "导出文件保存路径");
				saveFileDialog.Filter = "Excel files (*.xls)|*.xls|PDF files (*.pdf)|*.pdf|TXT files (*.txt)|*.txt";
				if (AccCommon.CodeVersion == CodeVersionType.JapanAF)
				{
					saveFileDialog.Filter = "Excel files (*.xls)|*.xls|TXT files (*.txt)|*.txt";
				}
				saveFileDialog.ShowDialog();
				string fileName2 = saveFileDialog.FileName;
				if (!string.IsNullOrEmpty(fileName2))
				{
					FileInfo fileInfo = new FileInfo(fileName2);
					string extension = fileInfo.Extension;
					int num = 0;
					while (num < gv.Columns.Count)
					{
						if (!(gv.Columns[num].FieldName == "check"))
						{
							num++;
							continue;
						}
						gv.Columns[num].Visible = false;
						break;
					}
					switch (extension)
					{
					case ".xls":
						try
						{
							XlsExportOptions xlsExportOptions = new XlsExportOptions();
							xlsExportOptions.ShowGridLines = true;
							xlsExportOptions.SheetName = "data";
							gv.ExportToXls(fileName2, xlsExportOptions);
						}
						catch (Exception ex3)
						{
							SysDialogs.ShowWarningMessage(ex3.Message);
							break;
						}
						if (!File.Exists(fileName2))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
						}
						else
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						}
						break;
					case ".pdf":
						try
						{
							gv.ExportToPdf(fileName2);
						}
						catch (Exception ex2)
						{
							SysDialogs.ShowWarningMessage(ex2.Message);
							break;
						}
						if (!File.Exists(fileName2))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
						}
						else
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						}
						break;
					case ".txt":
						try
						{
							gv.ExportToText(fileName2);
						}
						catch (Exception ex)
						{
							SysDialogs.ShowWarningMessage(ex.Message);
							break;
						}
						if (!File.Exists(fileName2))
						{
							SysDialogs.ShowWarningMessage(ShowMsgInfos.GetInfo("SOperationFailed", "操作失败"));
						}
						else
						{
							SysDialogs.ShowInfoMessage(ShowMsgInfos.GetInfo("SOperationSuccess", "操作成功"));
						}
						break;
					}
					int num2 = 0;
					while (true)
					{
						if (num2 < gv.Columns.Count)
						{
							if (!(gv.Columns[num2].FieldName == "check"))
							{
								num2++;
								continue;
							}
							break;
						}
						return;
					}
					gv.Columns[num2].Visible = true;
				}
			}
		}

		public static void InitTranslate()
		{
			Localizer.Active = new XtraEditors_CN();
			GridLocalizer.Active = new XtraGrid_CN();
			PreviewLocalizer.Active = new XtraPrinting_CN();
		}

		public static bool GridViewOutToCSV(GridView lvGV, string fileName)
		{
			try
			{
				StreamWriter streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < lvGV.Columns.Count; i++)
				{
					if (lvGV.Columns[i].FieldName != "check" && lvGV.Columns[i].Visible)
					{
						stringBuilder.Append(lvGV.Columns[i].Caption + ",");
					}
				}
				stringBuilder.Append("\r\n");
				if (lvGV != null && lvGV.RowCount > 0)
				{
					DataTable dataTable = lvGV.GridControl.DataSource as DataTable;
					if (dataTable != null)
					{
						for (int j = 0; j < dataTable.Rows.Count; j++)
						{
							DataRow dataRow = dataTable.Rows[j];
							if (dataRow != null)
							{
								for (int k = 0; k < lvGV.Columns.Count; k++)
								{
									if (lvGV.Columns[k].FieldName != "check" && lvGV.Columns[k].Visible)
									{
										if (dataRow[lvGV.Columns[k].FieldName] != null)
										{
											stringBuilder.Append(dataRow[lvGV.Columns[k].FieldName].ToString().Replace(",", "-") + ",");
										}
										else
										{
											stringBuilder.Append(" ,");
										}
									}
								}
							}
							stringBuilder.Append(" \r\n");
							if (j % 100 == 0)
							{
								streamWriter.Write(stringBuilder.ToString());
								stringBuilder = new StringBuilder();
							}
						}
						streamWriter.Write(stringBuilder.ToString());
					}
				}
				streamWriter.Flush();
				streamWriter.Close();
				streamWriter = null;
			}
			catch (Exception ex)
			{
				SysDialogs.ShowWarningMessage(ex.Message);
				return false;
			}
			return true;
		}
	}
}
