/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevExpress.XtraEditors.Controls;
using System;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class XtraEditors_CN : Localizer
	{
		public string GetInfo(StringId id)
		{
			switch (id)
			{
			case StringId.TextEditMenuCopy:
				return "复制(&C)";
			case StringId.TextEditMenuCut:
				return "剪切(&T)";
			case StringId.TextEditMenuDelete:
				return "删除(&D)";
			case StringId.TextEditMenuPaste:
				return "粘贴(&P)";
			case StringId.TextEditMenuSelectAll:
				return "全选(&A)";
			case StringId.TextEditMenuUndo:
				return "撤消(&U)";
			case StringId.UnknownPictureFormat:
				return "未知图片格式";
			case StringId.DateEditToday:
				return "今天";
			case StringId.DateEditClear:
				return "清空";
			case StringId.DataEmpty:
				return "无图像";
			case StringId.ColorTabWeb:
				return "网页";
			case StringId.ColorTabSystem:
				return "系统";
			case StringId.ColorTabCustom:
				return "自定义";
			case StringId.CheckUnchecked:
				return "未选择";
			case StringId.CheckIndeterminate:
				return "不确定";
			case StringId.CheckChecked:
				return "已选择";
			case StringId.CaptionError:
				return "标题错误";
			case StringId.Cancel:
				return "取消";
			case StringId.CalcError:
				return "计算错误";
			case StringId.OK:
				return "确定";
			case StringId.PictureEditMenuCopy:
				return "复制(&C)";
			case StringId.PictureEditMenuCut:
				return "剪切(&T)";
			case StringId.PictureEditMenuDelete:
				return "删除(&D)";
			case StringId.PictureEditMenuLoad:
				return "加载(&L)";
			case StringId.PictureEditMenuPaste:
				return "粘贴(&P)";
			case StringId.PictureEditMenuSave:
				return "保存(&S)";
			case StringId.PictureEditOpenFileError:
				return "错误图片格式";
			case StringId.PictureEditOpenFileErrorCaption:
				return "打开错误";
			case StringId.PictureEditOpenFileFilter:
				return "位图文件(*.bmp)|*.bmp|GIF动画 (*.gif)|*.gif|JPEG(*.jpg;*.jpeg)|*.jpg;*.jpeg|ICO(*.ico)|*.ico|所有图片文件|*.bmp;*.gif;*.jpeg;*.jpg;*.ico|所有文件(*.*)|*.*";
			case StringId.PictureEditOpenFileTitle:
				return "打开";
			case StringId.PictureEditSaveFileFilter:
				return "位图文件(*.bmp)|*.bmp|GIF动画(*.gif)|*.gif|JPEG(*.jpg)|*.jpg";
			case StringId.PictureEditSaveFileTitle:
				return "保存为";
			case StringId.TabHeaderButtonClose:
				return "关闭";
			case StringId.TabHeaderButtonNext:
				return "下一页";
			case StringId.TabHeaderButtonPrev:
				return "上一页";
			case StringId.XtraMessageBoxAbortButtonText:
				return "中断(&A)";
			case StringId.XtraMessageBoxCancelButtonText:
				return "取消(&C)";
			case StringId.XtraMessageBoxIgnoreButtonText:
				return "忽略(&I)";
			case StringId.XtraMessageBoxNoButtonText:
				return "否(&N)";
			case StringId.XtraMessageBoxOkButtonText:
				return "确定(&O)";
			case StringId.XtraMessageBoxRetryButtonText:
				return "重试(&R)";
			case StringId.XtraMessageBoxYesButtonText:
				return "是(&Y)";
			case StringId.ImagePopupEmpty:
				return "(空)";
			case StringId.ImagePopupPicture:
				return "(图片)";
			case StringId.InvalidValueText:
				return "无效的值";
			case StringId.LookUpEditValueIsNull:
				return "[无数据]";
			case StringId.LookUpInvalidEditValueType:
				return "无效的数据类型";
			case StringId.MaskBoxValidateError:
				return "输入数值不完整. 是否须要修改? 是 - 回到编辑模式以修改数值. 否 - 保持原来数值. 取消 - 回复原来数值. ";
			case StringId.NavigatorAppendButtonHint:
				return "添加";
			case StringId.NavigatorCancelEditButtonHint:
				return "取消编辑";
			case StringId.NavigatorEditButtonHint:
				return "编辑";
			case StringId.NavigatorEndEditButtonHint:
				return "结束编辑";
			case StringId.NavigatorFirstButtonHint:
				return "第一条";
			case StringId.NavigatorLastButtonHint:
				return "最后一条";
			case StringId.NavigatorNextButtonHint:
				return "下一条";
			case StringId.NavigatorNextPageButtonHint:
				return "下一页";
			case StringId.NavigatorPreviousButtonHint:
				return "上一条";
			case StringId.NavigatorPreviousPageButtonHint:
				return "上一页";
			case StringId.NavigatorRemoveButtonHint:
				return "删除";
			case StringId.NavigatorTextStringFormat:
				return "记录{0}/{1}";
			case StringId.None:
				return "";
			case StringId.NotValidArrayLength:
				return "无效的数组长度.";
			default:
				return string.Empty;
			}
		}

		public override string GetLocalizedString(StringId id)
		{
			string text = null;
			if (initLang.Lang.ToLower() != "en")
			{
				text = this.GetInfo(id);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = base.GetLocalizedString(id);
			}
			return XtraEditorsLocalizeInfo.GetInfo((int)id, text);
		}

		public void InitialLocalString()
		{
			foreach (int value in Enum.GetValues(typeof(StringId)))
			{
				this.GetLocalizedString((StringId)value);
			}
		}
	}
}
