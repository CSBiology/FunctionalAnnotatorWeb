using System.ComponentModel.DataAnnotations;

namespace FaTool.Web.Models.DataAnnotator
{

    public sealed class FunctionSearchQuery
    {
        [Required(AllowEmptyStrings = false)]
        public string OntologyId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string OrganismId { get; set; }

        [Required(AllowEmptyStrings = false)]        
        public string SearchName { get; set; }
    }
}