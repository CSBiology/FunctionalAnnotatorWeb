#region license
// The MIT License (MIT)

// DataAnnotatorController.cs

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

using System.Threading.Tasks;
using System.Web.Mvc;
using FaTool.Web.Models.Entities;
using System.Linq;
using FaTool.Web.Models.DataAnnotator;
using FaTool.Db;
using System.Data.Entity;

namespace FaTool.Web.Controllers
{
    public sealed class DataAnnotatorController : FaToolDbControllerBase
    {

        #region actions

        [HttpGet]
        public ActionResult RunApp()
        {
            return View("DataAnnotatorApp");
        }

        [HttpPost]
        public async Task<ActionResult> SearchFunctions(FunctionSearchQuery fsq)
        {
            if (ModelState.IsValid)
            {

                var query = from sp in FaToolDbEntities.GetProteinSearchValues(ProteinSearchOption.ProteinName, fsq.OrganismId)
                            join fp in FaToolDbEntities.GetProteinSearchValues(ProteinSearchOption.Function, fsq.OrganismId) on sp.ProteinID equals fp.ProteinID
                            where sp.Value == fsq.SearchName && fp.OntologyID == fsq.OntologyId
                            select new { id = fp.TermID, name = fp.TermName };

                var results = await query.ToArrayAsync();
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return HttpStatusCodeResults.HttpBadRequest(ModelState);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetOntologies()
        {
            var options = await FaToolDbEntities.GetOntologyOptions(null);
            var ontologies = options.Select(x => new { id = x.Value, name = x.Text }).ToArray();
            return Json(ontologies, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> GetSourceOrganisms()
        {
            var options = await FaToolDbEntities.GetSourceOrganismOptions(null);
            var orgs = options.Select(x => new { id = x.Value, name = x.Text }).ToArray();
            return Json(orgs, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region queries



        #endregion
    }
}