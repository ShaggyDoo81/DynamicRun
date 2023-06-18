using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shg.dynRunner.Application.Extensions
{
    internal static class ITypeExtensions
    {
        internal static Type GetType(this ITypeSymbol symbol) => Type.GetType(symbol.ToDisplayString());
    }
}
