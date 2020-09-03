using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DIGA.Tools.Compiler.Tcc
{
    internal  static class TccNativeX64
    {
        private const string DllName = "native\\x64\\libtcc.dll";

        private const CallingConvention Callingconvention = CallingConvention.StdCall;
        /// <summary>
        ///  create a new TCC compilation context
        /// </summary>
        /// <returns>TCC compilation context</returns>
        [DllImport(DllName, EntryPoint = "tcc_new", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr CreateNew();

        /// <summary>
        /// free a TCC compilation context
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        [DllImport(DllName, EntryPoint = "tcc_delete", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern void Destroy([In] HandleRef s);

        /// <summary>
        /// set CONFIG_TCCDIR at runtime 
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="path"></param>
        [DllImport(DllName, EntryPoint = "tcc_set_lib_path", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern void SetLibPath([In] HandleRef s, [In] string path);

        /// <summary>
        /// set error/warning display callback 
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="errorOpaque">??</param>
        /// <param name="errorFunc">error_funcDelegate</param>
        [DllImport(DllName, EntryPoint = "tcc_set_error_func", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern void SetErrorFunc([In] HandleRef s, object errorOpaque, TccErrorFuncDelegate64 errorFunc);

        /// <summary>
        /// set options as from command line (multiple supported)
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="str">Option</param>
        [DllImport(DllName, EntryPoint = "tcc_set_options", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern void SetOptions([In] HandleRef s, [In] string str);

        /// <summary>
        /// add include path 
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="pathname">Path to include Folder</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_add_include_path", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddIncludePath([In] HandleRef s, [In] string pathname);


        /// <summary>
        /// add in system include path
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="pathname">Path to Symbols</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_add_sysinclude_path", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddSysIncludePath([In] HandleRef s, [In] string pathname);

        /// <summary>
        /// define preprocessor symbol 'sym'. Can put optional value 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="sym">Symbol Name</param>
        /// <param name="value">Value</param>
        [DllImport(DllName, EntryPoint = "tcc_define_symbol", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern void DefineSymbol([In] HandleRef s, [In] string sym, [In] string value);

        /// <summary>
        /// add a symbol to the compiled program
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="name">Name</param>
        /// <param name="val">Value</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_add_symbol", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddSymbol([In] HandleRef s, string name, StringBuilder val);

        /// <summary>
        /// undefine preprocess symbol 'sym'
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="sym">Symbol Name</param>
        [DllImport(DllName, EntryPoint = "tcc_undefine_symbol", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern void UnDefineSymbol([In] HandleRef s, [In] string sym);


        /// <summary>
        /// add a file (C file, dll, object, library, ld script). Return -1 if error. 
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="filename">File-Path</param>
        /// <returns>Return -1 if error</returns>
        [DllImport(DllName, EntryPoint = "tcc_add_file", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddFile([In] HandleRef s, [In] string filename);

        /// <summary>
        /// compile a string containing a C source  
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="buf">String to compile</param>
        /// <returns>Return -1 if error</returns>
        [DllImport(DllName, EntryPoint = "tcc_compile_string", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int CompileString([In] HandleRef s, [In] string buf);

        /// <summary>
        /// set output type. MUST BE CALLED before any compilation
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="outputType">see TccOutPutType</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_set_output_type", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int SetOutputType([In] HandleRef s, [In] int outputType);

        /// <summary>
        /// equivalent to -Lpath option
        /// add library path 'dir'
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="pathname"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_add_library_path", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddLibPath([In] HandleRef s, string pathname);

        /// <summary>
        /// the library name is the same as the argument of the '-l' option 
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="libraryname">Library name</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_add_library", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddLibrary([In] HandleRef s, [In] string libraryname);


        /// <summary>
        /// add a symbol to the compiled program
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="name">Name</param>
        /// <param name="val">Value</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_add_symbol", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int AddSymbol([In] HandleRef s, [In] string name, [In] IntPtr val);

        /// <summary>
        /// output an executable, library or object file. DO NOT call tcc_relocate() before.
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="filename">File-name</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_output_file", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int CreateOutputFile([In] HandleRef s, [In] string filename);


        /// <summary>
        /// link and run main() function and return its value. DO NOT call tcc_relocate() before.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_run", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int Run([In] HandleRef s, [In] int argc, [In] string[] argv);

        /// <summary>
        ///  do all relocations (needed before using tcc_get_symbol())
        /// possible values for 'ptr':
        /// - TCC_RELOCATE_AUTO : Allocate and manage memory internally
        /// - NULL              : return required memory size for the step below
        /// - memory address    : copy code to memory passed by the caller
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="ptr">See Summery</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_relocate", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern int ReLocate([In] HandleRef s, [In] IntPtr ptr);

        /// <summary>
        /// return symbol value or NULL if not found 
        /// </summary>
        /// <param name="s">TCC compilation context</param>
        /// <param name="name">Name of the Symbol</param>
        /// <returns></returns>
        [DllImport(DllName, EntryPoint = "tcc_get_symbol", CallingConvention = Callingconvention,
            CharSet = CharSet.Ansi)]
        public static extern IntPtr GetSymbol([In] HandleRef s, [In] string name);
    }
}
