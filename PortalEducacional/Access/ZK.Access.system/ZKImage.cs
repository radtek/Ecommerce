/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Drawing;

namespace ZK.Access.system
{
	public class ZKImage
	{
		public static Image ResizeImage(int maxWidth, int maxHeight, Image Image)
		{
			int width = Image.Width;
			int height = Image.Height;
			if (width > maxWidth || height > maxHeight)
			{
				Image.RotateFlip(RotateFlipType.Rotate180FlipX);
				Image.RotateFlip(RotateFlipType.Rotate180FlipX);
				float num = 0f;
				if (width > height)
				{
					num = (float)width / (float)height;
					width = maxWidth;
					height = Convert.ToInt32(Math.Round((double)((float)width / num)));
				}
				else
				{
					num = (float)height / (float)width;
					height = maxHeight;
					width = Convert.ToInt32(Math.Round((double)((float)height / num)));
				}
				return Image.GetThumbnailImage(width, height, null, IntPtr.Zero);
			}
			return Image;
		}
	}
}
