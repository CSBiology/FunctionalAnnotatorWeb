#region license
// The MIT License (MIT)

// AnnotationModels.cs

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