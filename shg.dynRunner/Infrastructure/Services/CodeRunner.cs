using FluentResults;
using shg.dynRunner.Application.AssemblyLoadContexts;
using System.Runtime.CompilerServices;

namespace shg.dynRunner.Infrastructure.Services
{
    internal static class CodeRunner
    {
        public static void Execute(byte[] compiledAssembly, string[] args)
        {
            var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly, args);

            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
        }

        public static Result<T> Execute<T>(byte[] compiledAssembly, string className, string methodName, object[]? methodParams)
        {
            try
            {
                var result = LoadAndExecute<T>(compiledAssembly, className, methodName, methodParams);
                var (assemblyLoadContextWeakRef, value) = result;
                for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                return Result.Ok<T>(value);
            }
            catch(Exception ex)
            { 
                return Result.Fail<T>($"{ex}");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] compiledAssembly, string[] args)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                var assembly = assemblyLoadContext.LoadFromStream(asm);

                var entry = assembly.EntryPoint;

                _ = entry != null && entry.GetParameters().Length > 0
                    ? entry.Invoke(null, new object[] { args })
                    : entry?.Invoke(null, null);

                assemblyLoadContext.Unload();

                return new WeakReference(assemblyLoadContext);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (WeakReference, T) LoadAndExecute<T>(byte[] compiledAssembly, string className, string methodName, object[]? methodParams)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                var assembly = assemblyLoadContext.LoadFromStream(asm);

                var type = assembly.GetType(className);

                var instance = Activator.CreateInstance(type);

                var method = type.GetMethod(methodName);

                var result = method != null && method.GetParameters().Length > 0
                        ? method.Invoke(instance, methodParams)
                        : method?.Invoke(instance, null);

                assemblyLoadContext.Unload();

                if (result is T typedResult)
                {
                    return (new WeakReference(assemblyLoadContext), typedResult);
                }
                else if (result is Task<T> taskTypedResult)
                {
                    return (new WeakReference(assemblyLoadContext), taskTypedResult.Result);
                }
                else
                {
                    throw new InvalidCastException($"The result could not be cast to type {typeof(T).Name}.");
                }
            }
        }
    }
}
