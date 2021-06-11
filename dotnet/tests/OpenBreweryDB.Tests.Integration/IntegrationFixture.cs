// using System;
// using System.Data.SqlClient;
// using System.Diagnostics;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using OpenBreweryDB.API.Extensions;
// using OpenBreweryDB.Data;
// using ThrowawayDb;

// namespace OpenBreweryDB.Tests.Integration
// {
//     public class IntegrationFixture : IDisposable
//     {
//         private readonly IServiceCollection _serviceCollection;
//         private ISchema _graphQLSchema = null;
//         private readonly IRequestExecutorBuilder _requestExecutorBuilder;
//         private IRequestExecutor _graphQLRequestExecutor = null;

//         public IntegrationFixture()
//         {
//             Database = CreateDatabase("Server=.,9433;User Id=sa;Password=passw0rd!;");

//             _serviceCollection = new ServiceCollection()
//                 .AddOpenBreweryServices()
//                 // .AddAutoMapper(typeof(BreweryProfile), typeof(BreweryMappingProfile))
//                 .AddDbContext<BreweryDbContext>(options => options.UseSqlServer(Database.ConnectionString));

//             // _requestExecutorBuilder = _serviceCollection
//             //     .AddOpenBreweryGraphQLServer();

//             ServiceProvider = _serviceCollection.BuildServiceProvider();

//             _ = ServiceProvider.GetService<BreweryDbContext>().Database.EnsureCreated();
//         }

//         static ThrowawayDatabase CreateDatabase(string connectionString)
//         {
//             for (var i = 0; i < 5; i++)
//             {
//                 try
//                 {
//                     Thread.Sleep(i * 2000);
//                     return ThrowawayDatabase.Create(connectionString);
//                 }
//                 catch (Exception ex)
//                 {
//                     // If the exception was a timeout error (-2), continue
//                     if (ex is SqlException sqlException && sqlException.Number == -2)
//                     {
//                         Console.WriteLine($"SQL Timeout. Will retry in {(i + 1) * 2000} seconds. connectionstring='{connectionString}'");
//                         continue;
//                     }
//                 }
//             }

//             throw new Exception("Could not connect to database.");
//         }

//         public void Dispose() => Database.Dispose();

//         public IServiceProvider ServiceProvider { get; }

//         public ThrowawayDatabase Database { get; }

//         // public async Task<ISchema> GetSchemaAsync()
//         // {
//         //     if (_graphQLSchema == null)
//         //     {
//         //         _graphQLSchema = await _requestExecutorBuilder.BuildSchemaAsync();
//         //     }

//         //     return _graphQLSchema;
//         // }

//         // public async Task<IRequestExecutor> GetRequestExecutorAsync()
//         // {
//         //     if (_graphQLRequestExecutor == null)
//         //     {
//         //         _graphQLRequestExecutor = await _requestExecutorBuilder.BuildRequestExecutorAsync();
//         //     }

//         //     return _graphQLRequestExecutor;
//         // }

//         public async Task<long> PerformanceTest(Func<Task> performanceActionTest, int iterations = 10)
//         {
//             var timer = new Stopwatch();

//             timer.Start();
//             for (var i = 0; i < iterations; i++)
//             {
//                 await performanceActionTest();
//             }
//             timer.Stop();

//             return timer.ElapsedMilliseconds / iterations;
//         }

//         #region Public Methods

//         /// <summary>
//         /// Resetsand truncates all tables in database
//         /// </summary>
//         public void ResetDatabase()
//         {
//             var sql = @"
//                 /* TRUNCATE ALL TABLES IN A DATABASE */
//                 DECLARE @dropAndCreateConstraintsTable TABLE
//                         (
//                         DropStmt VARCHAR(MAX)
//                         ,CreateStmt VARCHAR(MAX)
//                         )
//                 /* Gather information to drop and then recreate the current foreign key constraints  */
//                 INSERT  @dropAndCreateConstraintsTable
//                         SELECT  DropStmt = 'ALTER TABLE [' + ForeignKeys.ForeignTableSchema
//                                 + '].[' + ForeignKeys.ForeignTableName + '] DROP CONSTRAINT ['
//                                 + ForeignKeys.ForeignKeyName + ']; '
//                             ,CreateStmt = 'ALTER TABLE [' + ForeignKeys.ForeignTableSchema
//                                 + '].[' + ForeignKeys.ForeignTableName
//                                 + '] WITH CHECK ADD CONSTRAINT [' + ForeignKeys.ForeignKeyName
//                                 + '] FOREIGN KEY([' + ForeignKeys.ForeignTableColumn
//                                 + ']) REFERENCES [' + SCHEMA_NAME(sys.objects.schema_id)
//                                 + '].[' + sys.objects.[name] + ']([' + sys.columns.[name]
//                                 + ']); '
//                         FROM    sys.objects
//                         INNER JOIN sys.columns
//                                 ON ( sys.columns.[object_id] = sys.objects.[object_id] )
//                         INNER JOIN ( SELECT sys.foreign_keys.[name] AS ForeignKeyName
//                                         ,SCHEMA_NAME(sys.objects.schema_id) AS ForeignTableSchema
//                                         ,sys.objects.[name] AS ForeignTableName
//                                         ,sys.columns.[name] AS ForeignTableColumn
//                                         ,sys.foreign_keys.referenced_object_id AS referenced_object_id
//                                         ,sys.foreign_key_columns.referenced_column_id AS referenced_column_id
//                                     FROM   sys.foreign_keys
//                                     INNER JOIN sys.foreign_key_columns
//                                             ON ( sys.foreign_key_columns.constraint_object_id = sys.foreign_keys.[object_id] )
//                                     INNER JOIN sys.objects
//                                             ON ( sys.objects.[object_id] = sys.foreign_keys.parent_object_id )
//                                     INNER JOIN sys.columns
//                                             ON ( sys.columns.[object_id] = sys.objects.[object_id] )
//                                             AND ( sys.columns.column_id = sys.foreign_key_columns.parent_column_id )
//                                 ) ForeignKeys
//                                 ON ( ForeignKeys.referenced_object_id = sys.objects.[object_id] )
//                                 AND ( ForeignKeys.referenced_column_id = sys.columns.column_id )
//                         WHERE   ( sys.objects.[type] = 'U' )
//                                 AND ( sys.objects.[name] NOT IN ( 'sysdiagrams' ) )
//                 /* SELECT * FROM @dropAndCreateConstraintsTable AS DACCT  --Test statement*/
//                 DECLARE @DropStatement NVARCHAR(MAX)
//                 DECLARE @RecreateStatement NVARCHAR(MAX)
//                 /* Drop Constraints */
//                 DECLARE Cur1 CURSOR READ_ONLY
//                 FOR
//                         SELECT  DropStmt
//                         FROM    @dropAndCreateConstraintsTable
//                 OPEN Cur1
//                 FETCH NEXT FROM Cur1 INTO @DropStatement
//                 WHILE @@FETCH_STATUS = 0
//                     BEGIN
//                             PRINT 'Executing ' + @DropStatement
//                             EXECUTE sp_executesql @DropStatement
//                             FETCH NEXT FROM Cur1 INTO @DropStatement
//                     END
//                 CLOSE Cur1
//                 DEALLOCATE Cur1
//                 /* Truncate all tables in the database in the dbo schema */
//                 DECLARE @DeleteTableStatement NVARCHAR(MAX)
//                 DECLARE Cur2 CURSOR READ_ONLY
//                 FOR
//                         SELECT  'TRUNCATE TABLE [dbo].[' + TABLE_NAME + ']'
//                         FROM    INFORMATION_SCHEMA.TABLES
//                         WHERE   TABLE_SCHEMA = 'dbo'
//                                 AND TABLE_TYPE = 'BASE TABLE'
//                 /* Change your schema appropriately if you don't want to use dbo */
//                 OPEN Cur2
//                 FETCH NEXT FROM Cur2 INTO @DeleteTableStatement
//                 WHILE @@FETCH_STATUS = 0
//                     BEGIN
//                             PRINT 'Executing ' + @DeleteTableStatement
//                             EXECUTE sp_executesql @DeleteTableStatement
//                             FETCH NEXT FROM Cur2 INTO @DeleteTableStatement
//                     END
//                 CLOSE Cur2
//                 DEALLOCATE Cur2
//                 /* Recreate foreign key constraints  */
//                 DECLARE Cur3 CURSOR READ_ONLY
//                 FOR
//                         SELECT  CreateStmt
//                         FROM    @dropAndCreateConstraintsTable
//                 OPEN Cur3
//                 FETCH NEXT FROM Cur3 INTO @RecreateStatement
//                 WHILE @@FETCH_STATUS = 0
//                     BEGIN
//                             PRINT 'Executing ' + @RecreateStatement
//                             EXECUTE sp_executesql @RecreateStatement
//                             FETCH NEXT FROM Cur3 INTO @RecreateStatement
//                     END
//                 CLOSE Cur3
//                 DEALLOCATE Cur3";

//             using (var sqlConnection = new SqlConnection(Database.ConnectionString))
//             {
//                 sqlConnection.Open();

//                 using (var command = new SqlCommand(sql, sqlConnection))
//                 {
//                     command.ExecuteNonQuery();
//                 }
//             }
//         }

//         #endregion
//     }
// }
