using System.Reflection;
using System.Runtime.Loader;

namespace shg.dynRunner.Application.AssemblyLoadContexts
{
    internal class SimpleUnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        public SimpleUnloadableAssemblyLoadContext()
            : base(true)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
