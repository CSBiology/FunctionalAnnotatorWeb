using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{
    public sealed class TermRecordView : ObjectView<Term>
    {
        public TermRecordView(Term term)
            : base(term)
        {
            AddField("Name", x => x.Name);
            AddField("ID", x => x.ID);
            AddField("Ontology Name", x => x.Ontology.Name);
            AddField("Ontology ID", x => x.Ontology.ID);
        }        
    }

    public sealed class TermQuery
    {
        [Required(AllowEmptyStrings = false)]
        [DisplayName("Select Ontology")]
        public string OntologyId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(64, MinimumLength = 3)]
        [DisplayName("Search Value")]
        public string SearchValue { get; set; }
    }
}