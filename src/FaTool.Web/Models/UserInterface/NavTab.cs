#region license
// The MIT License (MIT)

// NavTab.cs

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