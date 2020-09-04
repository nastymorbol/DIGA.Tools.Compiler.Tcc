using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DIGA.Tools.Compiler.Tcc
{
    public class TccCompiler : IDisposable
    {
        private HandleRef _OpaqueRef;

        public readonly HandleRef Handle;

        /// <summary>
        /// Action invoked before Compile the source
        /// </summary>
        public Action BeforeCompileAction { get; set; }

        /// <summary>
        /// Action invoked on Compiler Error or Warning
        /// </summary>
        public Action<string> CompilerErrorAction { get; set; }

        /// <summary>
        /// Ctor of TccCompiler
        /// </summary>
        public TccCompiler()
        {
            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string dirPath = fi.DirectoryName;


            if (dirPath == null)
            {
                dirPath = string.Empty;
            }

            var defaultLibPath = Path.Combine(dirPath, "native\\lib");
            var defaultTccIncludePath = Path.Combine(dirPath, "native\\libtcc");
            var defaultIncludePath = Path.Combine(dirPath, "native\\include");
            var defaultIncludeWindows = Path.Combine(dirPath, "native\\include\\winapi");
            this.Handle = CreateNew();
            if (this.Handle.Handle == IntPtr.Zero)
                throw new Exception("Could not create TCC Instance!");
            this._OpaqueRef = new HandleRef(this, Marshal.AllocHGlobal(512));
            if (this.IsX64)
            {
                TccErrorFuncDelegate64 dllErr = InternalCompilerErrorFunc;
                this.SetErrorFunction64(dllErr);
            }
            else
            {
                TccErrorFuncDelegate32 dllErr = InternalCompilerErrorFunc;
                this.SetErrorFunction32(dllErr);
            }

            this.AddIncludePath(defaultIncludePath);
            this.AddIncludePath(defaultTccIncludePath);
            this.AddIncludePath(defaultIncludeWindows);
            this.AddLibraryPath(defaultLibPath);
        }

        private void InternalCompilerErrorFunc(IntPtr opaque, string msg)
        {
            OnCompilerError(msg);
        }
        /// <summary>
        /// Invoking Function of CompilerErrorActions
        /// </summary>
        /// <param name="message">Error/Warning message</param>
        protected virtual void OnCompilerError(string message)
        {
            this.CompilerErrorAction?.Invoke(message);
        }

        /// <summary>
        /// Invoking Function of BeforeCompilerAction
        /// </summary>
        protected virtual void OnBeforeCompile()
        {
            this.BeforeCompileAction?.Invoke();
        }

        /// <summary>
        /// Infomrs you if you are using the 64 bit Version
        /// If true the calling Application uses 64 bit
        /// If fals the calling Application uses 32 bit
        /// </summary>
        public bool IsX64
        {
            get => IntPtr.Size == 8;
        }

        /// <summary>
        /// Read Source from File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetSourceFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Cannot find File:" + filePath);


            string retString = File.ReadAllText(filePath);

            return retString;
        }

        public int CompileSourceToDll(string source, string destinationFile)
        {
            return CompileSourceTo(source, TccOutputType.Dll, destinationFile);
        }

        public int CompileSourceToExe(string source, string destinationFile)
        {
            return CompileSourceTo(source, TccOutputType.Exe, destinationFile);
        }

        public int CompileSourceToObj(string source, string destinationFile)
        {
            return CompileSourceTo(source, TccOutputType.Obj, destinationFile);
        }

        public int CompileFileToDll(string fileName, string destinationFile)
        {
            return CompleFileTo(fileName, TccOutputType.Dll, destinationFile);
        }

        public int CompileFileToExe(string fileName, string destinationFile)
        {
            return CompleFileTo(fileName, TccOutputType.Exe, destinationFile);
        }

        public int CompileFileToObj(string fileName, string destnationFile)
        {
            return CompleFileTo(fileName, TccOutputType.Obj, destnationFile);
        }

        public int CompleFileTo(string fileName, TccOutputType outputType, string destinationFile)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("The sourcefile does not exiest!=>" + fileName);

            string soruceCode = File.ReadAllText(fileName);
            return CompileSourceTo(soruceCode, outputType, destinationFile);
        }

        public int CompileSourceTo(string sourceCode, TccOutputType outputType, string destinationFile)
        {
            this.SetOutPutType(outputType);
            int compileResult = this.CompileString(sourceCode);
            if (compileResult == -1)
            {
                return -1;
            }

            return this.CreateOutputFile(destinationFile);
        }

        public int Run(string sourceFile, params string[] args)
        {
            this.SetOutPutType(TccOutputType.Memory);
            string soruce = GetSourceFromFile(sourceFile);

            int compileResult = this.CompileString(soruce);
            if (compileResult == -1)
            {
                return -1;
            }

            return this.Run(args);
        }

        private HandleRef CreateNew()
        {
            if (this.IsX64)
            {
                return new HandleRef(this, TccNativeX64.CreateNew());
            }
            else
            {
                return new HandleRef(this, TccNativeX86.CreateNew());
            }
        }

        private void Delete()
        {
            if (this.Handle.Handle == IntPtr.Zero) return;
            if (this.IsX64)
            {
                TccNativeX64.Destroy(this.Handle);
            }
            else
            {
                TccNativeX86.Destroy(this.Handle);
            }
        }

        /// <summary>
        /// Set the Lib-Path
        /// </summary>
        /// <param name="path"></param>
        public void SetLibPath(string path)
        {
            if (this.IsX64)
            {
                TccNativeX64.SetLibPath(this.Handle, path);
            }
            else
            {
                TccNativeX86.SetLibPath(this.Handle, path);
            }
        }

        /// <summary>
        /// set error/warning display callback 32 bit
        /// </summary>
        /// <remarks>
        /// Please ensure that the transferred function can be called natively and that it has the CDECL calling convention.
        /// To do this, use the "UnmanagedFunctionPointer" attribute. 
        /// </remarks>
        /// <param name="errorFunction">Function delegate</param>
        public void SetErrorFunction32(TccErrorFuncDelegate32 errorFunction)
        {
            if (this.IsX64)
            {
                throw new Exception(
                    "You are using the 64 bit version please call SetErrorFunction32: The delegate must have the calling conventions STDCALL");
            }
            else
            {
                TccNativeX86.SetErrorFunc(this.Handle, this._OpaqueRef.Handle, errorFunction);
            }
        }

        /// <summary>
        /// set error/warning display callback 64 bit
        /// </summary>
        /// <remarks>
        /// Please ensure that the transferred function can be called natively and that it has the STDCALL calling convention.
        /// To do this, use the "UnmanagedFunctionPointer" attribute. 
        /// </remarks>
        /// <param name="errorFunction">Function delegate</param>
        public void SetErrorFunction64(TccErrorFuncDelegate64 errorFunction)
        {
            if (this.IsX64)
            {
                TccNativeX64.SetErrorFunc(this.Handle, this._OpaqueRef.Handle, errorFunction);
            }
            else
            {
                throw new Exception(
                    "You are using the 32 bit version please call SEtErrorFunction32: The delegate must have the calling conventions CDECL");
            }
        }

        /// <summary>
        /// set options as from command line (multiple supported)
        /// </summary>
        /// <param name="option"></param>
        public void SetOptions(string option)
        {
            if (this.IsX64)
            {
                TccNativeX64.SetOptions(this.Handle, option);
            }
            else
            {
                TccNativeX86.SetOptions(this.Handle, option);
            }
        }

        /// <summary>
        /// add include path 
        /// </summary>
        /// <param name="path">include path</param>
        /// <returns></returns>
        public int AddIncludePath(string path)
        {
            if (this.IsX64)
            {
                return TccNativeX64.AddIncludePath(this.Handle, path);
            }
            else
            {
                return TccNativeX86.AddIncludePath(this.Handle, path);
            }
        }

        /// <summary>
        /// add system include path
        /// </summary>
        /// <param name="path">include path</param>
        /// <returns></returns>
        public int AddSysIncludePath(string path)
        {
            if (this.IsX64)
            {
                return TccNativeX64.AddSysIncludePath(this.Handle, path);
            }
            else
            {
                return TccNativeX86.AddSysIncludePath(this.Handle, path);
            }
        }

        /// <summary>
        /// define preprocessor symbol 'sym'. Can put optional value 
        /// </summary>
        /// <param name="sym">symbol Name</param>
        /// <param name="value">optional value</param>
        public void DefineSymbol(string sym, string value)
        {
            if (this.IsX64)
            {
                TccNativeX64.DefineSymbol(this.Handle, sym, value);
            }
            else
            {
                TccNativeX86.DefineSymbol(this.Handle, sym, value);
            }
        }

        /// <summary>
        ///  undefine preprocess symbol 'sym'
        /// </summary>
        /// <param name="sym">symbol Name</param>
        public void UnDefineSymbol(string sym)
        {
            if (this.IsX64)
            {
                TccNativeX64.UnDefineSymbol(this.Handle, sym);
            }
            else
            {
                TccNativeX86.UnDefineSymbol(this.Handle, sym);
            }
        }

        /// <summary>
        ///  add a file (C file, dll, object, library, ld script). 
        /// </summary>
        /// <param name="fileName">File Path</param>
        /// <returns>Return -1 if error</returns>
        public int AddFile(string fileName)
        {
            if (this.IsX64)
            {
                return TccNativeX64.AddFile(this.Handle, fileName);
            }
            else
            {
                return TccNativeX86.AddFile(this.Handle, fileName);
            }
        }

        /// <summary>
        /// compile a string containing a C source 
        /// </summary>
        /// <param name="buffer">String to compile</param>
        /// <returns>Return -1 if error</returns>
        public int CompileString(string buffer)
        {
            OnBeforeCompile();
            if (this.IsX64)
            {
                return TccNativeX64.CompileString(this.Handle, buffer);
            }
            else
            {
                return TccNativeX86.CompileString(this.Handle, buffer);
            }
        }

        /// <summary>
        /// set output type. MUST BE CALLED before any compilation
        /// </summary>
        /// <param name="ouPutType">Output Type</param>
        /// <returns></returns>
        public int SetOutPutType(TccOutputType ouPutType)
        {
            if (this.IsX64)
            {
                return TccNativeX64.SetOutputType(this.Handle, (int) ouPutType);
            }
            else
            {
                return TccNativeX86.SetOutputType(this.Handle, (int) ouPutType);
            }
        }

        /// <summary>
        /// equivalent to -Lpath option of TCC
        /// add library path 'dir'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int AddLibraryPath(string path)
        {
            if (this.IsX64)
            {
                return TccNativeX64.AddLibPath(this.Handle, path);
            }
            else
            {
                return TccNativeX86.AddLibPath(this.Handle, path);
            }
        }

        /// <summary>
        /// the library name is the same as the argument of the '-l' option of TCC
        /// </summary>
        /// <param name="libName">Library name</param>
        /// <returns></returns>
        public int AddLibrary(string libName)
        {
            if (this.IsX64)
            {
                return TccNativeX64.AddLibrary(this.Handle, libName);
            }
            else
            {
                return TccNativeX86.AddLibrary(this.Handle, libName);
            }
        }

        /// <summary>
        /// add a symbol to the compiled program
        /// this can be a functionpointer or a variablepointer
        /// </summary>
        /// <remarks>
        /// If you give a funcitonpointer
        /// Please ensure that the transferred function can be called natively and that it has the CDECL(if 32 bit) or STDCALL(if 64 bit) calling convention.
        /// To do this, use the "UnmanagedFunctionPointer" attribute
        /// You can find out if the calling Application runs in 64 bit with the Property Is64Bit.
        /// If 64 Bit the underlaying DLL has STDCALL convention
        /// If 32 Bit the underlaying DLL has CDECL convention
        /// If you do not set the right CallingConvention the program will crash.
        /// Functionpointer:
        /// Use Marshal.GetFunctionPointerForDelegate to get the Functionpointer for the Delegate!!
        /// </remarks>
        /// <example>
        /// // in this case a 32 bit version is called
        /// [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        /// public dlegate int fool(int value);
        /// // in this case a 64 bit verison is called
        /// [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        /// public delegate int fool(int value);
        /// </example>
        /// <param name="name">Name of the Symbol</param>
        /// <param name="value">Pointer to the symbol</param>
        /// <returns></returns>
        public int AddSymbol(string name, IntPtr value)
        {
            if (this.IsX64)
            {
                return TccNativeX64.AddSymbol(this.Handle, name, value);
            }
            else
            {
                return TccNativeX86.AddSymbol(this.Handle, name, value);
            }
        }


        public int AddSymbol(string name, Delegate del)
        {
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(del);
            return AddSymbol(name, ptr);
        }


        /// <summary>
        /// output an executable, library or object file. DO NOT call Reallocate() before.
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <returns></returns>
        public int CreateOutputFile(string fileName)
        {
            if (this.IsX64)
            {
                return TccNativeX64.CreateOutputFile(this.Handle, fileName);
            }
            else
            {
                return TccNativeX86.CreateOutputFile(this.Handle, fileName);
            }
        }

        /// <summary>
        /// link and run main() function and return its value. DO NOT call Reallocate() before.
        /// </summary>
        /// <param name="args">arguments</param>
        /// <returns>return value of the executable</returns>
        public int Run(string[] args)
        {
            int argc = args.Length;
            if (this.IsX64)
            {
                return TccNativeX64.Run(this.Handle, argc, args);
            }
            else
            {
                return TccNativeX86.Run(this.Handle, argc, args);
            }
        }

        /// <summary>
        ///  do all relocations (needed before using GetSymbol())
        /// possible values for 'ptr':
        /// - TCC_RELOCATE_AUTO : Allocate and manage memory internally
        /// - NULL              : return required memory size for the step below
        /// - memory address    : copy code to memory passed by the caller
        /// </summary>
        /// <param name="ptr">See summery</param>
        /// <returns></returns>
        public int Reallocate(IntPtr ptr)
        {
            if (this.IsX64)
            {
                return TccNativeX64.ReLocate(this.Handle, ptr);
            }
            else
            {
                return TccNativeX86.ReLocate(this.Handle, ptr);
            }
        }

        /// <summary>
        /// return symbol pointer or NULL (IntPtr.Zero) if not found 
        /// </summary>
        /// <param name="name">Name of the symbol</param>
        /// <remarks>
        /// If you try to get a Delegate (function Ptr) please ensure the right CallingConvention!
        /// see also AddSybol Remarks.
        /// </remarks>
        /// <returns>return symbol value or NULL if not found </returns>
        public IntPtr GetSymbol(string name)
        {
            if (this.IsX64)
            {
                return TccNativeX64.GetSymbol(this.Handle, name);
            }
            else
            {
                return TccNativeX86.GetSymbol(this.Handle, name);
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(this._OpaqueRef.Handle);
            this.Delete();
        }
    }
}
