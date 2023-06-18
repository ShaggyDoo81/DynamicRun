using FluentResults;
using shg.dynRunner.Application.Interfaces;
using shg.dynRunner.Application.Models;
using shg.OtherLib;

namespace BlazorServerDemo.Services
{
    public class ExampleGetDynCodes : IGetDynCodes
    {
        public async Task<Result<List<DynCodeData>>> GetDynamicCodes()
        {
            var code = new DynCodeData(identifier: "ImplementClass",
                className: "ImplementClass",
                updatedTime: DateTime.Now,
                additionalTypes: new List<Type> {
                    typeof(IImplementation),
                    typeof(Task)
                },
                code: @"using shg.OtherLib;
                        using System.Threading.Tasks;
                        namespace Custom.ImplementClass
                        {
                            public class ImplementClass : IImplementation
                            {
                                public async Task<string> GetName()
                                {
                                    return ""Jose"";
                                }
                            }
                        }");
            var code2 = new DynCodeData(identifier: "ImplementClassWithoutInterface",
                className: "ImplementClassWithoutInterface",
                updatedTime: DateTime.Now,
                additionalTypes: new List<Type> {
                    typeof(Task)
                },
                code: @"using shg.OtherLib;
                        using System.Threading.Tasks;
                        namespace Custom.ImplementClassWithoutInterface;

                        public class ImplementClassWithoutInterface
                        {
                            public async Task<string> GetName(string name, string surname)
                            {
                                return $""{name} {surname}"";
                            }
                        }");

            return Result.Ok<List<DynCodeData>>(new List<DynCodeData> { code, code2 });
        }
    }
}
