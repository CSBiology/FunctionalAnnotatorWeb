using System.Collections.Generic;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{
    public sealed class GeneModelList : TableView<GeneModel>
    {

        public GeneModelList(IEnumerable<GeneModel> rows)
            : base(rows)
        {
            Caption = "Gene Model Sources";
            AddField("Source Organism", x => x.GeneModelSource.Organism.Name, x => x.GeneModelSource.Organism.ID);
            AddField("Taxonomy", x => x.GeneModelSource.Organism.Ontology.Name, x => x.GeneModelSource.Organism.Ontology.ID);
            AddField("Source", x => x.GeneModelSource.Name);
            AddField("Transcript ID", x => x.TranscriptID);
        }

    }
}