using FluentResults;
using Microsoft.Extensions.Hosting;
using shg.dynRunner.Application.Events;
using shg.dynRunner.Application.Interfaces;
using shg.dynRunner.Application.Models;

namespace shg.dynRunner.Infrastructure.Services
{
    public class DynRunnerHostedService : IHostedService, IDisposable
    {
        internal List<DynCompiledCode> _compiledCodes = new();
        private readonly IGetDynCodes _dynCodesGetter;

        public event EventHandler<InvalidCodeEventArgs>? InvalidCodeCompilationEvent;

        public DynRunnerHostedService(IGetDynCodes dynCodesGetter)
        {
            _dynCodesGetter = dynCodesGetter;
        }

        public void Dispose()
        {
            //GC.SuppressFinalize(_compiledCodes => _compiledCodes.Dispose());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var getCodes = await _dynCodesGetter.GetDynamicCodes();
            var taskList = new List<Task>();
            if (getCodes.IsSuccess)
            {
                var codes = getCodes.Value;
                codes.ForEach(code => taskList.Add(LoadCode(code)));
            }
            await Task.WhenAll(taskList);

            return;
        }

        private async Task LoadCode(DynCodeData code)
        {
            var existingCode = _compiledCodes.FirstOrDefault(x => x.Identifier == code.Identifier);
            if(existingCode != null) 
            {
                //update only if code is more recent
                if (existingCode.CompilationTime <= code.UpdatedTime)
                {
                    var getCompilationCode = CodeCompiler.CompileSourceCode(code.Code, code.Identifier, code.AdditionalTypes);
                    if (getCompilationCode.IsSuccess)
                    {
                        var compilationCode = getCompilationCode.Value;
                        existingCode.Code = compilationCode;
                        existingCode.CompilationTime = DateTime.Now;
                    }
                    else
                    {
                        //raise event
                        RaiseCompilationErrorsEvent(code.Identifier, code.Code, getCompilationCode.Errors.Select(x => x.Message).ToList());
                    }
                }
            }
            else
            {
                var compile = await GetCompiledCode(code);
                if (compile.IsSuccess)
                {
                    _compiledCodes.Add(compile.Value);
                }
                else
                {
                    //raise event here
                    RaiseCompilationErrorsEvent(code.Identifier, code.Code, compile.Errors.Select(x => x.Message).ToList());
                }
            }
        }

        private void RaiseCompilationErrorsEvent(string identifier, string code, List<string> errores)
        {
            InvalidCodeCompilationEvent?.Invoke(this, new InvalidCodeEventArgs(code, identifier, errores));
        }

        private async Task<Result<DynCompiledCode>> GetCompiledCode(DynCodeData code)
        {
            var getCompilationCode = CodeCompiler.CompileSourceCode(code.Code, code.Identifier, code.AdditionalTypes);
            if (getCompilationCode.IsSuccess)
            {
                var compilationCode = getCompilationCode.Value;
                return Result.Ok<DynCompiledCode>(new DynCompiledCode
                {
                    Identifier = code.Identifier,
                    Code = compilationCode,
                    CompilationTime = DateTime.Now,
                    LastExecutionTime = code.UpdatedTime,
                });
            }
            else
            {
                return Result.Fail(getCompilationCode.Errors);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
