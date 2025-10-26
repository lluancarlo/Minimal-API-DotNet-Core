using Minimal.Application.Common.DTOs;
using Minimal.Application.Common.Results;

namespace Minimal.Application.Common.Interfaces;

public interface IVerbImporterService
{
    Task<ServiceResult<VerbConjugationData?>> GetVerbConjugation(string verb, CancellationToken ct);
}
