namespace DIGA.Tools.Compiler.Tcc
{
    public static class TccOutPutTypeConst
    {
        public const int TCC_OUTPUT_MEMORY = 1; // output will be run in memory (default)
        public const int TCC_OUTPUT_EXE = 2; // executable file
        public const int TCC_OUTPUT_DLL = 3; // dynamic library
        public const int TCC_OUTPUT_OBJ = 4; // object file
        public const int TCC_OUTPUT_PREPROCESS = 5; // only preprocess (used internally)
    }
}