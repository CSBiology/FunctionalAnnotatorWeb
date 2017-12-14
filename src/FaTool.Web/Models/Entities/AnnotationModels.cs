using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{

    public enum AnnotationNavId
    {
        Term,
        Description,
        References
    }

    public sealed class AnnotationEditViewModel : EntityEditViewModelBase<Annotation, AnnotationEditArgs>
    {
        public AnnotationEditViewModel()
            : base()
        {
            this.Args = new AnnotationEditArgs();
            this.TermQuery = new TermQuery();
            this.OntologyOptions = Enumerable.Empty<SelectListItem>();
            this.GoEvidenceOptions = EnumSources.GetGoEvidenceOptions(null);
            this.TermOptions = Enumerable.Empty<SelectListItem>();
        }

        public IEnumerable<SelectListItem> GoEvidenceOptions { get; set; }
        public IEnumerable<SelectListItem> TermOptions { get; set; }
        public IEnumerable<SelectListItem> OntologyOptions { get; set; }
        public TermQuery TermQuery { get; set; }        
    }
    
    public sealed class AnnotationEditArgs : EntityEditArgs
    {

        public AnnotationEditArgs() { }

        [DisplayName("Select Term")]
        public string SelectedTermId { get; set; }

    }

    public sealed class AnnotationRecordView : ObjectView<Annotation>
    {
        public AnnotationRecordView(Annotation annotation)
            : base(annotation)
        {
            AddField("Entry Date", x => x.EntryDate);
            AddField("Evidence Code", x => GOEvidences.ResolveName(x.EvidenceCode), x => x.EvidenceCode);
            AddField("Term", x => x.Term.Name, x => x.Term.ID);
            AddField("Ontology", x => x.Term.Ontology.Name, x => x.Term.Ontology.ID);
        }
    }

    public sealed class AnnotationList : TableView<Annotation>
    {
        public AnnotationList(IEnumerable<Annotation> rows)
            : base(rows)
        {
            Caption = "Protein Functions";
            AddField("Term", x => x.Term.Name, x => x.Term.ID);
            AddField("Ontology", x => x.Term.Ontology.Name, x => x.Term.Ontology.ID);
            AddField("GO Evidence", x => GOEvidences.ResolveName(x.EvidenceCode), x => x.EvidenceCode);
        }
    }
}