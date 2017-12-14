#region license
// The MIT License (MIT)

// ProteinSearchResults.cs

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
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.ProteinSearch
{

    public sealed class ProteinSearchResult
    {

        private readonly ProteinSearchValue value;
        private readonly ActionLink link;

        public ProteinSearchResult(
            ProteinSearchValue value,
            ActionLink link)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (link == null)
                throw new ArgumentNullException("link");

            this.value = value;
            this.link = link;
        }

        public ProteinSearchValue Value { get { return value; } }
        public ActionLink ProteinLink { get { return link; } }
    }

    public static class DataPaging
    {

        public const int PageSize = 16;

        public static int CalculatePageCount(int numResults)
        {
            return (int)Math.Ceiling((double)numResults / (double)PageSize);
        }

        public static int CalculateSkipCount(int numResults, int pageIndex)
        {
            int numPages = DataPaging.CalculatePageCount(numResults);

            pageIndex = DataPaging.FixPageIndex(numPages, pageIndex);

            return PageSize * pageIndex;
        }

        public static int FixPageIndex(int numPages, int pageIndex)
        {
            if (pageIndex < 0)
                return 0;
            else if (pageIndex >= numPages)
                return numPages - 1;
            else
                return pageIndex;
        }

    }
}