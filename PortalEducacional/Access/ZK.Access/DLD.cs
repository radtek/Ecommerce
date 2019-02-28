/// Exception while reading XmlDoc: System.Xml.XmlException: ' ' é um símbolo inesperado. O símbolo esperado é '<!--' ou '<[CDATA['. Linha 6423, posição 11.
///   em System.Xml.XmlTextReaderImpl.Throw(Exception e)
///   em System.Xml.XmlTextReaderImpl.ParseElementContent()
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadMembersSection(XmlTextReader reader, LinePositionMapper linePosMapper, List`1 indexList)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider.ReadXmlDoc(XmlTextReader reader)
///   em ICSharpCode.Decompiler.Documentation.XmlDocumentationProvider..ctor(String fileName)
///   em ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation(ModuleDefinition module)
///   em ICSharpCode.Decompiler.CSharp.Transforms.AddXmlDocumentationTransform.Run(AstNode rootNode, TransformContext context)
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace ZK.Access
{
	internal class DLD
	{
		public enum ModePass
		{
			ByValue = 1,
			ByRef
		}

		private IntPtr hModule = IntPtr.Zero;

		private IntPtr farProc = IntPtr.Zero;

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		public void LoadDll(string lpFileName)
		{
			this.hModule = DLD.LoadLibrary(lpFileName);
			if (!(this.hModule == IntPtr.Zero))
			{
				return;
			}
			throw new Exception(" 没有找到 :" + lpFileName + ".");
		}

		public void LoadDll(IntPtr HMODULE)
		{
			if (HMODULE == IntPtr.Zero)
			{
				throw new Exception(" 所传入的函数库模块的句柄 HMODULE 为空 .");
			}
			this.hModule = HMODULE;
		}

		public void LoadFun(string lpProcName)
		{
			if (this.hModule == IntPtr.Zero)
			{
				throw new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !");
			}
			this.farProc = DLD.GetProcAddress(this.hModule, lpProcName);
			if (!(this.farProc == IntPtr.Zero))
			{
				return;
			}
			throw new Exception(" 没有找到 :" + lpProcName + " 这个函数的入口点 ");
		}

		public void LoadFun(string lpFileName, string lpProcName)
		{
			this.hModule = DLD.LoadLibrary(lpFileName);
			if (this.hModule == IntPtr.Zero)
			{
				throw new Exception(" 没有找到 :" + lpFileName + ".");
			}
			this.farProc = DLD.GetProcAddress(this.hModule, lpProcName);
			if (!(this.farProc == IntPtr.Zero))
			{
				return;
			}
			throw new Exception(" 没有找到 :" + lpProcName + " 这个函数的入口点 ");
		}

		public void UnLoadDll()
		{
			DLD.FreeLibrary(this.hModule);
			this.hModule = IntPtr.Zero;
			this.farProc = IntPtr.Zero;
		}

		public object Invoke(object[] ObjArray_Parameter, Type[] TypeArray_ParameterType, ModePass[] ModePassArray_Parameter, Type Type_Return)
		{
			if (this.hModule == IntPtr.Zero)
			{
				throw new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !");
			}
			if (this.farProc == IntPtr.Zero)
			{
				throw new Exception(" 函数指针为空 , 请确保已进行 LoadFun 操作 !");
			}
			if (ObjArray_Parameter.Length != ModePassArray_Parameter.Length)
			{
				throw new Exception(" 参数个数及其传递方式的个数不匹配 .");
			}
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "InvokeFun";
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("InvokeDll");
			MethodBuilder methodBuilder = moduleBuilder.DefineGlobalMethod("dld", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, Type_Return, TypeArray_ParameterType);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			for (int i = 0; i < ObjArray_Parameter.Length; i++)
			{
				switch (ModePassArray_Parameter[i])
				{
				case ModePass.ByValue:
					iLGenerator.Emit(OpCodes.Ldarg, i);
					break;
				case ModePass.ByRef:
					iLGenerator.Emit(OpCodes.Ldarga, i);
					break;
				default:
					throw new Exception(" 第 " + (i + 1).ToString() + " 个参数没有给定正确的传递方式 .");
				}
			}
			if (IntPtr.Size == 4)
			{
				iLGenerator.Emit(OpCodes.Ldc_I4, this.farProc.ToInt32());
				goto IL_0173;
			}
			if (IntPtr.Size == 8)
			{
				iLGenerator.Emit(OpCodes.Ldc_I8, this.farProc.ToInt64());
				goto IL_0173;
			}
			throw new PlatformNotSupportedException();
			IL_0173:
			iLGenerator.EmitCalli(OpCodes.Calli, CallingConvention.StdCall, Type_Return, TypeArray_ParameterType);
			iLGenerator.Emit(OpCodes.Ret);
			moduleBuilder.CreateGlobalFunctions();
			MethodInfo method = moduleBuilder.GetMethod("dld");
			return method.Invoke(null, ObjArray_Parameter);
		}

		public object Invoke(IntPtr IntPtr_Function, object[] ObjArray_Parameter, Type[] TypeArray_ParameterType, ModePass[] ModePassArray_Parameter, Type Type_Return)
		{
			if (this.hModule == IntPtr.Zero)
			{
				throw new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !");
			}
			if (IntPtr_Function == IntPtr.Zero)
			{
				throw new Exception(" 函数指针 IntPtr_Function 为空 !");
			}
			this.farProc = IntPtr_Function;
			return this.Invoke(ObjArray_Parameter, TypeArray_ParameterType, ModePassArray_Parameter, Type_Return);
		}
	}
}
