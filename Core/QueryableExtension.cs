using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core
{
    /// <summary>
    /// IQueryable extensions
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// Converts P-SQL (ADO.NET) string given an IQueryable&lt;TEntity&gt; and DbContext
        /// </summary>
        /// <param name="query"></param>
        /// <param name="dbCtx"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static string ToSql<TEntity>(this IQueryable<TEntity> query, DbContext dbCtx)
        {
            var modelGenerator = dbCtx.GetService<IQueryModelGenerator>();
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var databaseDependencies = dbCtx.GetService<DatabaseDependencies>();
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor) queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.FirstOrDefault()?.ToString();
            return sql;
        }
    }
}