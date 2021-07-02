namespace GraphQL.AspNetCore.Middleware
{
    public class GraphQLRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public Inputs Variables { get; set; }
    }
}
