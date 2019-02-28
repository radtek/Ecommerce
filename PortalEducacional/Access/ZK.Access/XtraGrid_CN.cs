/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using DevExpress.XtraGrid.Localization;
using System;
using ZK.ConfigManager;

namespace ZK.Access
{
	public class XtraGrid_CN : GridLocalizer
	{
		public string GetInfo(GridStringId id)
		{
			switch (id)
			{
			case GridStringId.CardViewNewCard:
				return "新卡片";
			case GridStringId.CardViewQuickCustomizationButton:
				return "自定义格式";
			case GridStringId.CardViewQuickCustomizationButtonFilter:
				return "筛选";
			case GridStringId.CardViewQuickCustomizationButtonSort:
				return "排序";
			case GridStringId.ColumnViewExceptionMessage:
				return "是否确定修改?";
			case GridStringId.CustomFilterDialog2FieldCheck:
				return "字段";
			case GridStringId.CustomFilterDialogCancelButton:
				return "取消";
			case GridStringId.CustomFilterDialogCaption:
				return "条件为:";
			case GridStringId.CustomFilterDialogFormCaption:
				return "清除筛选条件(&L)";
			case GridStringId.CustomFilterDialogOkButton:
				return "确定(&O)";
			case GridStringId.CustomFilterDialogRadioAnd:
				return "和(&A)";
			case GridStringId.CustomFilterDialogRadioOr:
				return "或者(&O)";
			case GridStringId.CustomizationBands:
				return "分区";
			case GridStringId.CustomizationCaption:
				return "自定义显示字段";
			case GridStringId.CustomizationColumns:
				return "列";
			case GridStringId.FileIsNotFoundError:
				return "文件{0}没找到!";
			case GridStringId.GridGroupPanelText:
				return "拖曳一列页眉在此进行排序";
			case GridStringId.GridNewRowText:
				return "单击这里新增一行";
			case GridStringId.GridOutlookIntervals:
				return "一个月以上;上个月;三周前;两周前;上周;;;;;;;;昨天;今天;明天; ;;;;;;;下周;两周后;三周后;下个月;一个月之后;";
			case GridStringId.MenuColumnBestFit:
				return "自动调整字段宽度";
			case GridStringId.MenuColumnBestFitAllColumns:
				return "自动调整所有字段宽度";
			case GridStringId.MenuColumnClearFilter:
				return "清除筛选条件";
			case GridStringId.MenuColumnColumnCustomization:
				return "显示/隐藏字段";
			case GridStringId.MenuColumnFilter:
				return "筛选";
			case GridStringId.MenuColumnGroup:
				return "按此列分组";
			case GridStringId.MenuColumnGroupBox:
				return "分组区";
			case GridStringId.MenuColumnSortAscending:
				return "升序排序";
			case GridStringId.MenuColumnSortDescending:
				return "降序排序";
			case GridStringId.MenuColumnUnGroup:
				return "取消分组";
			case GridStringId.MenuFooterAverage:
				return "平均";
			case GridStringId.MenuFooterAverageFormat:
				return "平均={0:#.##}";
			case GridStringId.MenuFooterCount:
				return "计数";
			case GridStringId.MenuFooterCountFormat:
				return "{0}";
			case GridStringId.MenuFooterMax:
				return "最大值";
			case GridStringId.MenuFooterMaxFormat:
				return "最大值={0}";
			case GridStringId.MenuFooterMin:
				return "最小";
			case GridStringId.MenuFooterMinFormat:
				return "最小值={0}";
			case GridStringId.MenuFooterNone:
				return "没有";
			case GridStringId.MenuFooterSum:
				return "合计";
			case GridStringId.MenuFooterSumFormat:
				return "求和={0:#.##}";
			case GridStringId.MenuGroupPanelClearGrouping:
				return "取消所有分组";
			case GridStringId.MenuGroupPanelFullCollapse:
				return "收缩全部分组";
			case GridStringId.MenuGroupPanelFullExpand:
				return "展开全部分组";
			case GridStringId.PopupFilterAll:
				return "(所有)";
			case GridStringId.PopupFilterBlanks:
				return "(空值)";
			case GridStringId.PopupFilterCustom:
				return "(自定义)";
			case GridStringId.PopupFilterNonBlanks:
				return "(非空值)";
			case GridStringId.PrintDesignerBandedView:
				return "打印设置(区域模式)";
			case GridStringId.PrintDesignerBandHeader:
				return "区标题";
			case GridStringId.PrintDesignerCardView:
				return "打印设置(卡片模式)";
			case GridStringId.PrintDesignerDescription:
				return "为当前视图设置不同的打印选项.";
			case GridStringId.PrintDesignerGridView:
				return "打印设置(表格模式)";
			case GridStringId.WindowErrorCaption:
				return "错误";
			case GridStringId.MenuGroupPanelHide:
				return "隐藏分组面板";
			case GridStringId.CustomFilterDialogEmptyValue:
				return "请输入一个值";
			case GridStringId.CustomFilterDialogEmptyOperator:
				return "请输入一个值";
			case GridStringId.CustomFilterDialogHint:
				return "请输入一个值";
			default:
				return string.Empty;
			}
		}

		public override string GetLocalizedString(GridStringId id)
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
			return XtraGridLocalizeInfo.GetInfo((int)id, text);
		}

		public void InitialLocalString()
		{
			foreach (int value in Enum.GetValues(typeof(GridStringId)))
			{
				this.GetLocalizedString((GridStringId)value);
			}
		}
	}
}
