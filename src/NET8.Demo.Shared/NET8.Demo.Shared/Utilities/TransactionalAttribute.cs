using Microsoft.AspNetCore.Mvc.Filters;
using System.Transactions;

namespace NET8.Demo.Shared;

public class TransactionalAttribute : Attribute, IAsyncActionFilter
{
    readonly IsolationLevel _isolationLevel;

    public TransactionalAttribute(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted) => _isolationLevel = isolationLevel;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        using var transactionScope = new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions()
            {
                IsolationLevel = _isolationLevel
            },
            TransactionScopeAsyncFlowOption.Enabled
        );

        var actionExecutedContext = await next();
        if (actionExecutedContext.Exception == null)
        {
            transactionScope.Complete();
        }
    }
}
