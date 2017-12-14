using System;
using System.Collections.Generic;
using System.Linq;

namespace FaTool.Web.Models.UserInterface
{
    public interface INavTab
    {
        ActionLink Link { get; }
        bool Active { get; }
    }

    public sealed class NavTab<TNavId> : INavTab
        where TNavId : struct
    {

        private readonly TNavId id;

        public NavTab(
            TNavId id,
            ActionLink link,
            bool disabled = false)
        {

            if (link == null)
                throw new ArgumentNullException("link");

            this.Link = link;
            this.id = id;
        }

        public TNavId NavId { get { return id; } }

        #region INavTab Members

        public ActionLink Link { get; private set; }

        public bool Active { get; internal set; }

        #endregion
    }

    public interface INavTabViewLayout : IEnumerable<INavTab>
    {
        IEnumerable<INavTab> NavTabs { get; }
        INavTab ActiveTab { get; }
    }

    public interface INavTabViewLayout<TNavId> : INavTabViewLayout
        where TNavId : struct
    {
        new IEnumerable<NavTab<TNavId>> NavTabs { get; }
        new NavTab<TNavId> ActiveTab { get; }
    }

    public static class NavTabs
    {
        public static NavTab<TNavId> ToggleActive<TNavId>(
            this IEnumerable<NavTab<TNavId>> navTabs,
            TNavId navId)
            where TNavId : struct
        {
            if (navTabs == null)
                throw new ArgumentNullException("navTabs");

            foreach (var tab in navTabs)
            {
                tab.Active = tab.NavId.Equals(navId);
            }

            return navTabs.Single(x => x.Active);
        }        
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class NavTabLinkTextAttribute : Attribute
    {

        private readonly string name = string.Empty;

        public NavTabLinkTextAttribute(string name)
        {
            this.name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
        }

        public string Name { get { return name; } }

    }

    public abstract class NavTabViewLayoutBase<TNavId> : INavTabViewLayout<TNavId>
        where TNavId : struct
    {

        protected NavTabViewLayoutBase() { }

        #region INavTabViewLayout Members

        IEnumerable<INavTab> INavTabViewLayout.NavTabs
        {
            get { return NavTabs; }
        }

        INavTab INavTabViewLayout.ActiveTab
        {
            get { return ActiveTab; }
        }

        #endregion

        #region INavTabViewLayout<TNavId> Members

        public IEnumerable<NavTab<TNavId>> NavTabs { get; set; }
        public NavTab<TNavId> ActiveTab { get; set; }

        #endregion

        #region IEnumerable<INavTab> Members

        public IEnumerator<INavTab> GetEnumerator()
        {
            return NavTabs.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return NavTabs.GetEnumerator();
        }

        #endregion
    }
}