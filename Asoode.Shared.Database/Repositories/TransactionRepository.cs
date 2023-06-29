using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Dtos.Transaction;
using Asoode.Shared.Abstraction.Types;
using Asoode.Shared.Database.Contracts;

namespace Asoode.Shared.Database.Repositories;

internal class TransactionRepository : ITransactionRepository
{
    private readonly ILoggerService _loggerService;

    public TransactionRepository(ILoggerService loggerService)
    {
        _loggerService = loggerService;
    }
    public async Task<OperationResult<GridResult<TransactionDto>>> List(Guid userId, GridFilter model)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "TransactionRepository.List", e);
            return OperationResult<GridResult<TransactionDto>>.Failed();
        }
    }
}