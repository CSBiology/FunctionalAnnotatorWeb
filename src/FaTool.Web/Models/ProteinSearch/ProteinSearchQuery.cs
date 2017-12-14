using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FaTool.Db;

namespace FaTool.Web.Models.ProteinSearch
{

    [Bind(Prefix = "Query")]
    public sealed class ProteinSearchQuery
    {

        public ProteinSearchQuery() { }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Source Organism")]
        public string SourceOrganismID { get; set; }

        [Required()]
        [Display(Name = "Search in")]
        public ProteinSearchOption SearchOption { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256, MinimumLength = 3)]
        [Display(Name = "Search for")]
        public string SearchValue { get; set; }
    }
}