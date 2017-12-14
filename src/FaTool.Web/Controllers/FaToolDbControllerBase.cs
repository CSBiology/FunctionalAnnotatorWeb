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