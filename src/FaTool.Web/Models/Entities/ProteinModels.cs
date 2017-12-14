using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{

    public enum ProteinNavId
    {
        Functions,
        Synonyms,
        Description,
        References,
        [NavTabLinkTextAttribute("Gene Models")]
        GeneModels,
        Homologs
    }
    
    public sealed class ProteinRecordView : ObjectView<Protein>
    {
        public ProteinRecordView(Protein protein)
            : base(protein)
        {
            AddField("Protein Name", x => x.Name);
        }        
    }    
}