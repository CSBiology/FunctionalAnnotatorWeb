using System.Collections.Generic;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{

    public sealed class ProteinHomologyGroupList : TableView<ProteinHomologyGroup>
    {
        public ProteinHomologyGroupList(IEnumerable<ProteinHomologyGroup> rows)
            : base(rows)
        {
            AddField("Group Name", x => x.Group.Name);
            AddField("Homology Type", x => x.Group.HomologyType);
        }
    } 
}