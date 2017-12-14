using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FaTool.Db
{
    public static class FaToolDbEntitiesExtensions
    {

        public static async Task<bool> Exists<TEntity>(this FaToolDbEntities ctx, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity
        {
            return await ctx.Set<TEntity>().AnyAsync(predicate);
        }

        public static async Task<bool> Exists<TEntity>(this FaToolDbEntities ctx, Guid id)
            where TEntity : class, IEntity<Guid?>
        {
            return await ctx.Exists<TEntity>(x => x.ID == id);
        }

        public static async Task<bool> Exists<TEntity>(this FaToolDbEntities ctx, string id)
            where TEntity : class, IEntity<string>
        {
            return await ctx.Exists<TEntity>(x => x.ID == id);
        }

        public static Task<TEntity> Find<TEntity>(this FaToolDbEntities ctx, object id)
            where TEntity : class, IEntity
        {
            return ctx.Set<TEntity>().FindAsync(id);
        }

        public static TEntity Create<TEntity>(this FaToolDbEntities ctx)
            where TEntity : class, IEntity
        {
            return ctx.Core.CreateObject<TEntity>();
        }

        public static TEntity Init<TEntity, TKey>(this FaToolDbEntities ctx, TKey id)
            where TEntity : class, IEntity<TKey>
        {
            var entity = ctx.Create<TEntity>();
            entity.ID = id;
            entity.RowVersion = new byte[8];
            return entity;
        }

        public static DbEntityEntry<TEntity> SetRowVersion<TEntity>(this DbEntityEntry<TEntity> entry, byte[] rowVersion)
            where TEntity : class, IHasRowVersion
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (rowVersion == null)
                throw new ArgumentNullException("rowVersion");

            entry.OriginalValues["RowVersion"] = rowVersion;
            return entry;
        }

        public static Task LoadCollection<TEntity, TElement>(
            this FaToolDbEntities ctx,
            TEntity entity,
            Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
            params string[] includes)
            where TEntity : class, IEntity
            where TElement : class, IEntity
        {
            var query = ctx
                .Entry(entity)
                .Collection(navigationProperty)
                .Query();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.LoadAsync();
        }

        public static Task LoadReference<TEntity, TElement>(
            this FaToolDbEntities ctx,
            TEntity entity,
            Expression<Func<TEntity, TElement>> navigationProperty,
            params string[] includes)
            where TEntity : class, IEntity
            where TElement : class, IEntity
        {
            var query = ctx
                .Entry(entity)
                .Reference(navigationProperty)
                .Query();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.LoadAsync();
        }
    }
}