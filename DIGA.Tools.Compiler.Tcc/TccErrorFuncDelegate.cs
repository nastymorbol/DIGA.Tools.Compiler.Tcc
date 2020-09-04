using System;
using System.Runtime.InteropServices;

namespace DIGA.Tools.Compiler.Tcc
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    public delegate void TccErrorFuncDelegate64(IntPtr opaque, string msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void TccErrorFuncDelegate32(IntPtr opaque, string msg);


   

}