using FluentResults;
using shg.dynRunner.Application.Models;

namespace shg.dynRunner.Application.Interfaces
{
    public interface IGetDynCodes
    {
        Task<Result<List<DynCodeData>>> GetDynamicCodes();
    }
}
