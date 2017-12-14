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