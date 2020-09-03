using System.Runtime.InteropServices;

namespace DIGA.Tools.Compiler
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    public delegate void TccErrorFuncDelegate64(object opaque, string msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public delegate void TccErrorFuncDelegate32(object opaque, string msg);


   

}