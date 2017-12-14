using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FaTool.Web.Models.ProteinSearch;

namespace FaTool.Web.Models.Home
{
    public sealed class FunctionSearchViewModel
    {

        private readonly ProteinSearchQuery query;
        private readonly IEnumerable<SelectListItem> sourceOrganisms;

        public FunctionSearchViewModel(
            ProteinSearchQuery query,
            IEnumerable<SelectListItem> sourceOrganisms)
        {
            if (sourceOrganisms == null)
                throw new ArgumentNullException("sourceOrganisms");
            if (query == null)
                throw new ArgumentNullException("query");

            this.query = query;
            this.sourceOrganisms = sourceOrganisms;
        }

        public ProteinSearchQuery Query { get { return query; } }

        public IEnumerable<SelectListItem> SourceOrganisms { get { return sourceOrganisms; } }
    }
}