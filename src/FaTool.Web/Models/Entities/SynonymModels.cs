using System.Collections.Generic;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{
    public sealed class SynonymList : TableView<Synonym>
    {
        public SynonymList(IEnumerable<Synonym> rows)
            : base(rows)
        {
            Caption = "Protein Synonymes";
            AddField("Term", x => x.SynonymType.Name, x => x.SynonymType.ID);
            AddField("Ontology", x => x.SynonymType.Ontology.Name, x => x.SynonymType.Ontology.ID);
            AddField("Value", x => x.SynonymValue);
        }
    }
}