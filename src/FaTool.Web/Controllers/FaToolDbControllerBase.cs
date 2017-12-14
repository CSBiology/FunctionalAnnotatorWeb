#region license
// The MIT License (MIT)

// FaToolDbControllerBase.cs

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Web.Mvc;
using FaTool.Db;

namespace FaTool.Web.Controllers
{
    public abstract class FaToolDbControllerBase : Controller
    {

        private readonly FaToolDbEntities faToolDbEntities;

        public FaToolDbControllerBase()
        {
            faToolDbEntities = new FaToolDbEntities();
            faToolDbEntities.Configuration.AutoDetectChangesEnabled = false;
            faToolDbEntities.Configuration.LazyLoadingEnabled = false;
            faToolDbEntities.Configuration.ProxyCreationEnabled = true;
        }

        #region entity

        protected FaToolDbEntities FaToolDbEntities { get { return faToolDbEntities; } }

        protected T Update<T>(
            T model,
            string prefix,
            bool clearModelState = false)
            where T : class, IEntity
        {
            if (ValueProvider.ContainsPrefix(prefix))
            {
                bool ok = TryUpdateModel(model, prefix);

                if (ok == false && clearModelState)
                    ModelState.Clear();
            }
            else
            {
                throw new InvalidOperationException(
                    "Model could not be updated because prefix is missing in value converter: " + prefix);
            }

            return model;
        }

        protected async Task<TEntity> Find<TEntity, TKey>(
            TKey id,
            string prefix,
            bool clearModelState = false)
            where TEntity : class, IEntity<TKey>
        {
            var entity = await FaToolDbEntities.Find<TEntity>(id);

            if (entity == null)
                return null;

            entity = Update<TEntity>(entity, prefix, clearModelState);

            if (object.Equals(id, entity.ID))
                return entity;
            else
                return null;
        }

        protected T Restore<T>(
            string prefix,
            bool clearModelState = false)
            where T : class, IEntity
        {
            var model = FaToolDbEntities.Create<T>();
            return Update<T>(model, prefix, clearModelState);
        }

        protected async Task<ActionResult> DbUpdateResult<TEntity>(
            TEntity entity,
            byte[] rowVersion,
            string redirectUrl)
            where TEntity : class, IEntity
        {

            if (entity == null)
                throw new ArgumentNullException("entity");
            if (redirectUrl == null)
                throw new ArgumentNullException("redirectUrl");
            if (rowVersion == null)
                throw new ArgumentNullException("rowVersion");

            try
            {
                var entry = FaToolDbEntities.Entry(entity);
                entry.State = EntityState.Modified;
                entry.SetRowVersion(rowVersion);

                await FaToolDbEntities.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return HttpStatusCodeResults.HttpConflict(ex.Message);
            }
            catch (DbUpdateException ex1)
            {
                return HttpStatusCodeResults.HttpInternalServerError(ex1);
            }

            return Redirect(redirectUrl);
        }

        protected async Task<ActionResult> DbInsertResult<TEntity>(
            TEntity entity,
            string redirectUrl)
            where TEntity : class, IEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (redirectUrl == null)
                throw new ArgumentNullException("redirectUrl");

            try
            {
                var entry = FaToolDbEntities.Entry(entity);
                entry.State = EntityState.Added;

                await FaToolDbEntities.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return HttpStatusCodeResults.HttpConflict(ex.Message);
            }
            catch (DbUpdateException ex1)
            {
                return HttpStatusCodeResults.HttpInternalServerError(ex1);
            }

            return Redirect(redirectUrl);
        }

        #endregion

        #region controller override

        protected override void Dispose(bool disposing)
        {

            if (disposing && faToolDbEntities != null)
            {
                faToolDbEntities.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}