using System.Collections.Generic;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{
    public sealed class PubReferenceList : TableView<PubReference>
    {
        public PubReferenceList(IEnumerable<PubReference> rows)
            : base(rows)
        {
            Caption = "Publication References";
            AddField("Name", x => x.Name);
            AddField("DOI", x => x.DOI);
        }

    }
}