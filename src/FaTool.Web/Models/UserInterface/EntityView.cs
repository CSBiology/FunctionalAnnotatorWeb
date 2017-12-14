#region license
// The MIT License (MIT)

// EntityView.cs

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