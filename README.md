# DIGA.Tools.Compiler a LIBTCC Wrapper

This Dll wraps the native LIBTCC.dll.

The TCC-Comiler in a DLL.:

Sometimes it is necessary to create a native DLL or call native code. This component helps here. You can create the C code in your applications and then create a DLL from it, which you can then integrate via DLLImport. Or you can run the C code in memory. You can transfer symbols (variables and functions) to the running C code. Or call functions in the running C code.

Please make sure that you define the correct calling convention for your delegates. All delegates should be provided with the UnmanagedFunctionPointer attribute.

Note for Windows users:
If you do not specify any calling conventions in C, cdel is used for 32 bit and stdcall for 64 bit.

You can compile into a native EXE, DLL or you can Run in memory.

```c#

//like in the c definiton _cdecl the CallingConvention must be set to Cdel
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
public delegate int Foo(int n, string txt);

string code = "sourcecode!!"
 using (var compiler = new TccCompiler())
 {
     compiler.SetOutPutType(TccOutputType.Memory);
     if (compiler.CompileString(code) == -1)
      {
         Console.WriteLine("Could not comple");
         return;
      }
      string inputString = "hallo Welt";
      IntPtr symbolPointer = Marshal.StringToHGlobalAnsi(initString);
      compiler.AddSymbol("SymbolName",symbolPointer);
      ...
      ...
      if (compiler.Reallocate(TccRealocateConst.TCC_RELOCATE_AUTO) < 0)
      {
         Console.WriteLine("Could not reloacate the intern pointers");
         Marshal.FreeHGlobal(symbolPointer);
         return;
      ]
      
      IntPtr funcPtr = compiler.GetSymbol("myfuc");
      Foo foo = Marshal.GetDelegateForFunctionPointer<T>(funcPtr);
      foo(1,"Ein Parameter");
      
      Marshal.FreeHGlobal(symbolPointer);
      
      
 }
```

Offical Web:
https://bellard.org/tcc/

You can find the sources at:
https://github.com/TinyCC/tinycc

Or at:
https://repo.or.cz/w/tinycc.git

If you want to Compile the LIBTCC.dll from source use CL and compile the 32 bit and the 64 bit Verison separate.

eg.

<TCC-SOURCE_64>\build-tcc.bat -c cl -t 64

<TCC-SOURCE_32>\build-tcc.bat -c cl -t 32

if you use TCC to compile some Functions are not exportet.

