using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FaTool.Db;
using FaTool.Web.Models.Entities;
using FaTool.Web.Models.ProteinSearch;
using System.Data.Entity;

namespace FaTool.Web.Controllers
{


    public sealed class ProteinSearchController : FaToolDbControllerBase
    {

        #region actions

        [HttpGet]
        public async Task<ActionResult> Index(string organismId = null)
        {
            var query = new ProteinSearchQuery();
            query.SearchOption = ProteinSearchOption.Function;
            query.SourceOrganismID = organismId;
            query.SearchValue = string.Empty;

            var model = await CreateModel(query, 0);
            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Results(ProteinSearchQuery query, int pageIndex = 0)
        {
            if (ModelState.IsValid)
            {
                var model = await Search(query, pageIndex);
                return View("Index", model);
            }
            else
            {
                var model = await CreateModel(query, pageIndex);
                return View("Index", model);
            }
        }

        #endregion

        #region helper

        private async Task<ProteinSearchViewModel> CreateModel(ProteinSearchQuery psq, int pageIndex)
        {
            var model = new ProteinSearchViewModel();
            model.Query = psq;
            model.SourceOrganisms = await FaToolDbEntities.GetSourceOrganismOptions(psq.SourceOrganismID);
            model.SearchOptions = EnumSources.GetProteinSearchOptions(psq.SearchOption);
            model.StatusDescription = string.Empty;
            model.Results = Enumerable.Empty<ProteinSearchResult>();
            model.ResultsAction = Url.Action("Results", new { pageIndex = 0 });
            model.NextResultsAction = Url.Action("Results", new { pageIndex = pageIndex + 1 });
            model.PreviousResultsAction = Url.Action("Results", new { pageIndex = pageIndex - 1 });

            return model;
        }

        private async Task<ProteinSearchViewModel> Search(ProteinSearchQuery psq, int pageIndex)
        {

            var model = await CreateModel(psq, pageIndex);

            var query = FaToolDbEntities
                .GetProteinSearchValues(psq.SearchOption, psq.SourceOrganismID)
                .Where(pv => pv.Value.Contains(psq.SearchValue));

            int numResults = await query.CountAsync();

            if (numResults == 0)
            {
                model.StatusDescription = "Query does not return any results.";
            }
            else
            {
                int numPages = DataPaging.CalculatePageCount(numResults);
                pageIndex = DataPaging.FixPageIndex(numPages, pageIndex);
                int skip = DataPaging.CalculateSkipCount(numResults, pageIndex);

                var proteinValues = await query
                    .OrderBy(x => x.Value)
                    .ThenBy(x => x.TermName)
                    .ThenBy(x => x.ProteinName)
                    .Skip(skip)
                    .Take(DataPaging.PageSize)
                    .ToArrayAsync();

                model.Results = proteinValues
                    .Select(CreateItem)
                    .ToArray();

                model.StatusDescription = CreateDescription(numResults, numPages, pageIndex);
            }

            return model;
        }

        private ProteinSearchResult CreateItem(ProteinSearchValue pv)
        {
            var action = Url.EntityGetActionLink(pv.ProteinName, "Proteins", pv.ProteinID);
            return new ProteinSearchResult(pv, action);
        }

        private string CreateDescription(int numResults, int numPages, int pageIndex)
        {
            pageIndex = DataPaging.FixPageIndex(numPages, pageIndex);

            return string.Format(
                    "Total number of results found: {0}, Page {1} of {2}.",
                    numResults,
                    pageIndex + 1,
                    numPages);
        }

        #endregion

    }
}