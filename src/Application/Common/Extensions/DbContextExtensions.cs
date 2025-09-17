using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Common.Extensions;

public static class Extensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken = default) where TDestination : class
        => PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize, cancellationToken);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration, CancellationToken cancellationToken = default) where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync(cancellationToken);

    public static async Task PopulateActivitiesAsync<T>(this IEnumerable<T> entities, IApplicationDbContext context) where T : class, IActivatableEntity
    {
        var entityList = entities.ToList();
        if (!entityList.Any()) return;

        var ids = entityList.Select(e => e.Id).ToList();
        var entityType = entityList.First().EntityType;

        var activities = await context.Activities.Where(c => c.EntityType == entityType && ids.Contains(c.EntityId)).ToListAsync();

        foreach (var entity in entityList)
        {
            entity.Activities = activities.Where(c => c.EntityId == entity.Id).ToList();
        }
    }

    public static async Task PopulateCommentsForMultipleEntitiesAsync(IApplicationDbContext context, params IEnumerable<IActivatableEntity>[] entityCollections)
    {
        var allEntities = entityCollections.SelectMany(e => e).ToList();
        if (!allEntities.Any()) return;

        var entitiesByType = allEntities.GroupBy(e => e.EntityType);

        foreach (var group in entitiesByType)
        {
            var ids = group.Select(e => e.Id).ToList();
            var entityType = group.Key;

            var activities = await context.Activities.Where(c => c.EntityType == entityType && ids.Contains(c.EntityId)).ToListAsync();

            foreach (var entity in group)
            {
                entity.Activities = activities.Where(c => c.EntityId == entity.Id).ToList();
            }
        }
    }
}