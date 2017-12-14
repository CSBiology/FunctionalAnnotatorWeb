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