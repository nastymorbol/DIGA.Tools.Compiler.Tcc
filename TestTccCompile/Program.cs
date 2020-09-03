using System;
using System.Runtime.InteropServices;
using DIGA.Tools.Compiler;
using DIGA.Tools.Compiler.Tcc;

namespace TestTccCompile
{
   
    public class DelegatePtr : IDisposable
    {
        public IntPtr Handle { get; }
       
        private GCHandle gch;
        public bool IsValid => this.Handle != IntPtr.Zero;
        public DelegatePtr(Delegate dlg)
        {
            gch = GCHandle.Alloc(dlg);
            this.Handle = Marshal.GetFunctionPointerForDelegate(dlg);
        }

        public static implicit operator IntPtr(DelegatePtr input)
        {
            return input.Handle;
        }

        public static implicit operator DelegatePtr(Delegate input)
        {
            return new DelegatePtr(input);
        }
        public void Dispose()
        {
            if (gch.IsAllocated)
                gch.Free();
        }
    }

    public class DelegatePtr<T> where T : Delegate
    {
        public T Delegate{get;}
        private IntPtr Handle{get;}
        public bool IsValid => this.Handle != IntPtr.Zero;
        public DelegatePtr(IntPtr funcPtr)
        {
            this.Handle = funcPtr;
            this.Delegate = Marshal.GetDelegateForFunctionPointer<T>(funcPtr);
        }

        public static implicit operator IntPtr(DelegatePtr<T> input)
        {
            return input.Handle;
        }

        public static implicit operator T(DelegatePtr<T> input)
        {
            return input.Delegate;
        }

        public static implicit operator DelegatePtr<T>(IntPtr funcPtr)
        {
            return new DelegatePtr<T>(funcPtr);
        }
    }
    public class HGlobalAnsiStringPtr : IDisposable
    {
        public IntPtr Handle { get; }
        public bool IsValid => this.Handle != IntPtr.Zero;
        public HGlobalAnsiStringPtr(string initString)
        {
            this.Handle = Marshal.StringToHGlobalAnsi(initString);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(this.Handle);
        }

        public static implicit operator HGlobalAnsiStringPtr(string input)
        {
            return new HGlobalAnsiStringPtr(input);

        }

        public static implicit operator IntPtr(HGlobalAnsiStringPtr input)
        {
            return input.Handle;
        }
    }
    class Program
    {
        //like in the c definiton _cdecl the CallingConvention must be set to Cdel
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int Add(int a, int b);

        //like in the c definiton _cdecl the CallingConvention must be set to Cdel
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public delegate int Foo(int n, string txt);

        static void Main(string[] args)
        {
            string code = "#include <tcclib.h>\n" + /* include the "Simple libc header for TCC" */
                          "#define _cdecl __attribute__((__cdecl__))\n" + /*define cdelc calling convention*/
                          "#define _import __attribute__((dllimport))\n" + /*define dllimport for symbol*/
                          "_cdecl _import extern int add(int a, int b);\n" +
                          "_import extern const char hello[];\n" +
                          "_cdecl int fib(int n)\n" +
                          "{\n" +
                          "    if (n <= 2)\n" +
                          "        return 1;\n" +
                          "    else\n" +
                          "        return fib(n-1) + fib(n-2);\n" +
                          "}\n" +
                          "\n" +
                          "_cdecl int foo(int n, const char* txt)\n" +
                          "{\n" +
                          "    printf(\"%s\\n\", hello);\n" +
                          "    printf(\"fib(%d) = %d\\n\", n, fib(n));\n" +
                          "    printf(\"add(%d, %d) = %d\\n\", n, 2 * n, add(n, 2 * n));\n" +
                          "    printf(\"param Text=%s\\n\",txt);\n" +
                          "    return 0;\n" +
                          "}\n";


            //Create a Compiler Object please use always USING
            //The Object must be disposed!!!
            using (var compiler = new TccCompiler())
            {
                compiler.SetOutPutType(TccOutputType.Memory);
                if (compiler.CompileString(code) == -1)
                {
                    Console.WriteLine("Could not comple");
                    return;
                }

            
                Add addDel = AddImp;
                DelegatePtr addPtr = addDel;
                int retVal = compiler.AddSymbol("add", addPtr);
                if (retVal != 0)
                {
                    Console.WriteLine("Cannot add Smbol add!");
                    addPtr.Dispose();
                    return;
                }


                string hello = "hallo Welt";
                //IntPtr halloPtr = Marshal.StringToHGlobalAnsi(hello);
                HGlobalAnsiStringPtr halloPtr = hello;
                compiler.AddSymbol("hello", halloPtr);

                if (compiler.Reallocate(TccRealocateConst.TCC_RELOCATE_AUTO) < 0)
                {
                    Console.WriteLine("Could not reloacate the intern pointers");
                    addPtr.Dispose();
                    halloPtr.Dispose();
                    return;
                }

                DelegatePtr<Foo> fooPtr = compiler.GetSymbol("foo");
                if (!fooPtr.IsValid)
                {
                    Console.WriteLine("cannot get foo Symbol!");
                    addPtr.Dispose();
                    halloPtr.Dispose();
                    return;
                }

                fooPtr.Delegate(32, "Eingabe Text1");
                fooPtr.Delegate(23, "Eingabe Text2");

                halloPtr.Dispose();
                addPtr.Dispose();
                

            }
        }


        private static int AddImp(int a, int b)
        {
            return a + b;
        }
    }
}
