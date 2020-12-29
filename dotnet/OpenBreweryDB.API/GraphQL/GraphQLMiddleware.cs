using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OpenBreweryDB.API.GraphQL
{
    public class GraphQLMiddleware
    {
        private readonly IDocumentExecuter _executer;
        private readonly RequestDelegate _next;
        private readonly GraphQLOptions _options;
        private readonly IDocumentWriter _writer;

        public GraphQLMiddleware(
            RequestDelegate next,
            IDocumentWriter writer,
            IDocumentExecuter executer,
            IOptions<GraphQLOptions> options)
        {
            _next = next;
            _options = options.Value;
            _executer = executer;
            _writer = writer;
        }

        public async Task Invoke(HttpContext context, ISchema schema, IServiceProvider serviceProvider)
        {
            if (!IsGraphQLRequest(context))
            {
                await _next(context);
                return;
            }

            await ExecuteAsync(context, schema, serviceProvider);
        }

        private bool IsGraphQLRequest(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments(_options.Path)
                && string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase);
        }

        private async Task ExecuteAsync(HttpContext context, ISchema schema, IServiceProvider serviceProvider)
        {
            var request = await Deserialize(context);

            var isDevelopment = serviceProvider.GetRequiredService<IWebHostEnvironment>()?.IsDevelopment() ?? true;

            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = request?.Query;
                _.OperationName = request?.OperationName;
                _.RequestServices = context.RequestServices;
                _.Inputs = request?.Variables.ToInputs();
                _.Listeners.Add(serviceProvider.GetRequiredService<DataLoaderDocumentListener>());
                _.UserContext = _options.BuildUserContext?.Invoke(context);
                _.ThrowOnUnhandledException = isDevelopment;
                _.EnableMetrics = isDevelopment;

                if (isDevelopment)
                {
                    _.UnhandledExceptionDelegate = context =>
                    {
                        try
                        {
                            var log = serviceProvider.GetRequiredService<ILogger<GraphQLMiddleware>>();
                            log.LogError(context.Exception, "Unhandled GraphQL Exception");
                        }
                        catch
                        {
                        }
                    };

                    _.FieldMiddleware.Use<InstrumentFieldsMiddleware>();
                }
            });

            await WriteResponseAsync(context, result);
        }

        private async Task WriteResponseAsync(HttpContext context, ExecutionResult result)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.Errors?.Any() == true ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;

            await _writer.WriteAsync(context.Response.Body, result);
        }

        public static async Task<GraphQLRequest> Deserialize(HttpContext httpContext) => await JsonSerializer
            .DeserializeAsync<GraphQLRequest>(
                httpContext.Request.Body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
    }
}
