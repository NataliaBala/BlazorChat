using ApplicationCore.Interfaces.Criteria;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF;

public class EfSpecificationEvaluator<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
    {
        var query = inputQuery;
        query = query.Where(spec.Criteria);
        query = query.OrderBy(spec.OrderBy);
        query = query.OrderByDescending(spec.OrderByDescending);
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        return query;
    }
}