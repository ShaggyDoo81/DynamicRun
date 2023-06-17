namespace shg.dynRunner.Application.Models
{
    internal class DynCompiledCode
    {
        internal string? Identifier { get; set; }
        internal DateTime? CompilationTime { get; set; }
        internal DateTime? LastExecutionTime { get; set; }
        internal byte[]? Code { get; set; }
    }
}
