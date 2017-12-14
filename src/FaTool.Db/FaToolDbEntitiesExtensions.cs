#region license
// The MIT License (MIT)

// FaToolDbEntitiesExtensions.cs

// Copyright (c) 2016 Alexander Lüdemann
// alexander.luedemann@outlook.com
// luedeman@rhrk.uni-kl.de

// Computational Systems Biology, Technical University of Kaiserslautern, Germany
 

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

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