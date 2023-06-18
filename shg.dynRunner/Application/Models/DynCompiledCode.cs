namespace shg.dynRunner.Application.Models
{
    internal class DynCompiledCode
    {
        internal string? Identifier { get; set; }
        internal string? ClassName { get; set; }
        internal DateTime? CompilationTime { get; set; }
        internal DateTime? LastExecutionTime { get; set; }
        internal byte[]? CompiledCode { get; set; }
        internal List<DynClass> Classes { get; set; }
    }
}
