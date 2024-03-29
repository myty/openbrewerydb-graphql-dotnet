using System;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Language.AST;

namespace OpenBreweryDB.Schema
{
    public class OpenBreweryDocumentExecuter : DocumentExecuter
    {
        private static readonly IExecutionStrategy _serialExecutionStrategy = new SerialExecutionStrategy();
        private static readonly IExecutionStrategy _subscriptionExecutionStrategy = new SubscriptionExecutionStrategy();

        protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
        {
            return context.Operation.OperationType switch
            {
                OperationType.Query => _serialExecutionStrategy,
                OperationType.Mutation => _serialExecutionStrategy,
                OperationType.Subscription => _subscriptionExecutionStrategy,
                _ => throw new InvalidOperationException($"Unexpected OperationType {context.Operation.OperationType}"),
            };
        }
    }
}
