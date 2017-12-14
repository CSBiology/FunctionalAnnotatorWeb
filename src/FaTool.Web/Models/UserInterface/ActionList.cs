#region license
// The MIT License (MIT)

// ActionList.cs

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

namespace FaTool.Web.Models.UserInterface
{
    public sealed class ActionList : IEnumerable<ActionLink>
    {

        private readonly string caption;
        private readonly IList<ActionLink> links = new List<ActionLink>();

        public ActionList(string caption)
        {
            if (caption == null)
                throw new ArgumentNullException("caption");
            this.caption = caption;
        }

        public string Caption { get { return caption; } }

        public void Add(ActionLink link)
        {
            links.Add(link);
        }

        public int Count { get { return links.Count; } }

        #region IEnumerable<ActionLink> Members

        public IEnumerator<ActionLink> GetEnumerator()
        {
            return links.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return links.GetEnumerator();
        }

        #endregion
    }
}