using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace FaTool.Web.Models.ProteinSearch
{

    public sealed class ProteinSearchViewModel
    {

        public ProteinSearchViewModel() { }

        public ProteinSearchQuery Query { get; set; }

        public bool HasResults { get { return Results != null ? Results.Any() : false; } }

        public IEnumerable<SelectListItem> SourceOrganisms { get; set; }

        public IEnumerable<SelectListItem> SearchOptions { get; set; }

        public IEnumerable<ProteinSearchResult> Results { get; set; }

        public string StatusDescription { get; set; }

        public string ResultsAction { get; set; }

        public string NextResultsAction { get; set; }

        public string PreviousResultsAction { get; set; }
    }
}