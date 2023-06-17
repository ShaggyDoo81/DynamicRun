using FluentResults;
using shg.dynRunner.Application.Interfaces;
using shg.dynRunner.Application.Models;
using shg.OtherLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicRun
{
    public class ExampleGetDynCodes : IGetDynCodes
    {
        public async Task<Result<List<DynCodeData>>> GetDynamicCodes()
        {
            var code = new DynCodeData(identifier: "ImplementClass",
                updatedTime: DateTime.Now,
                additionalTypes: new List<Type> {
                    typeof(IImplementation),
                    typeof(Task)
                },
                code: @"using shg.OtherLib;
                        using System.Threading.Tasks;

                        public class ImplementClass : IImplementation
                        {
                            public async Task<string> GetName()
                            {
                                return ""Jose"";
                            }
                        }");

            return Result.Ok<List<DynCodeData>>(new List<DynCodeData> { code });
        }
    }
}
