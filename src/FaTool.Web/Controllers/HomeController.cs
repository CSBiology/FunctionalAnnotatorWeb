#region license
// The MIT License (MIT)

// HomeController.cs

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

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using FaTool.Web.Models.Home;
using FaTool.Web.Models.ProteinSearch;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;
using FaTool.Web.Models.Entities;

namespace FaTool.Web.Controllers
{
    public class HomeController : FaToolDbControllerBase
    {
        public async Task<ActionResult> Index()
        {
            var model = await CreateModel();
            return View("Index", model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult OData()
        {
            return Redirect("/Services/FaToolDbDataService.svc/$metadata");
        }

        private async Task<HomeViewModel> CreateModel()
        {
            var funcSearch = await CreateFunctionSearch();
            var sourceOrgs = await GetOrganismSearchLinks();
            return new HomeViewModel(funcSearch, sourceOrgs);
        }

        private async Task<IEnumerable<ActionLink>> GetOrganismSearchLinks()
        {
            var query = FaToolDbEntities
                .GeneModelSources
                .AsNoTracking()
                .Select(x => x.Organism)
                .Include(x => x.Ontology)
                .Distinct()
                .OrderBy(x => x.Name);

            var orgs = await query.ToArrayAsync();

            return orgs.Select(x => CreateOrganismSearchLink(x));
        }

        private ActionLink CreateOrganismSearchLink(Term org)
        {
            string url = Url.Action("Index", "ProteinSearch", new { organismId = org.ID });
            string text = string.Format("{0} [{1}]", org.Name, org.Ontology.Name);
            return new ActionLink(text, url);
        }

        private async Task<FunctionSearchViewModel> CreateFunctionSearch()
        {
            var query = new ProteinSearchQuery();
            var sourceOrganisms = await FaToolDbEntities.GetSourceOrganismOptions(query.SourceOrganismID);
            query.SearchOption = ProteinSearchOption.Function;
            query.SourceOrganismID = sourceOrganisms.FirstOrDefault().Value;
            var model = new FunctionSearchViewModel(query, sourceOrganisms);
            return model;
        }
        
    }
}