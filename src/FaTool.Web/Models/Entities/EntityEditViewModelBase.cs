using System.ComponentModel.DataAnnotations;
using FaTool.Db;

namespace FaTool.Web.Models.Entities
{
    public abstract class EntityEditViewModelBase<TEntity, TArgs>
        where TEntity : class, IEntity
        where TArgs : EntityEditArgs
    {

        protected EntityEditViewModelBase() { }

        public TEntity Entity { get; set; }

        public TArgs Args { get; set; }
    }

    public abstract class EntityEditArgs
    {

        protected EntityEditArgs() { }

        [Required(AllowEmptyStrings = false)]
        public string SaveAction { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string EndEditAction { get; set; }
    }
}