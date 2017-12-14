using System.Linq;
using System;

namespace FaTool.Web.Models.UserInterface
{
    public interface IEntityView
    {
        IObjectView Properties { get; }
        bool HasNavTabs { get; }
        INavTabViewLayout NavTabLayout { get; }
        IRecordView TabContent { get; }
    }

    public class EntityView<TEntity> : IEntityView
    {

        public EntityView() { }

        #region IEntityView Members

        public bool HasNavTabs
        {
            get { return false; }
        }

        public IObjectView<TEntity> Properties { get; set; }

        IObjectView IEntityView.Properties { get { return Properties; } }

        public IRecordView TabContent { get { throw new NotSupportedException(); } }

        INavTabViewLayout IEntityView.NavTabLayout { get { throw new NotSupportedException(); } }

        #endregion

    }

    public class EntityView<TEntity, TNavId>
        : NavTabViewLayoutBase<TNavId>, IEntityView, INavTabViewLayout<TNavId>
        where TNavId : struct
    {

        public EntityView() { }

        #region IEntityView Members

        public bool HasNavTabs
        {
            get { return NavTabs != null && NavTabs.Any(); }
        }

        public IObjectView<TEntity> Properties { get; set; }

        IObjectView IEntityView.Properties { get { return Properties; } }

        public IRecordView TabContent { get; set; }

        INavTabViewLayout IEntityView.NavTabLayout { get { return this; } }

        #endregion

    }

}