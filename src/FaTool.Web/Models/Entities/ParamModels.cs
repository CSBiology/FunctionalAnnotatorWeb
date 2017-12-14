using System.Collections.Generic;
using FaTool.Db;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Entities
{
    public class ParamList : TableView<ParamBase>
    {
        public ParamList(IEnumerable<ParamBase> rows)
            : base(rows)
        {
            Caption = "Description Parameters";
            AddField("Term", x => x.Term.Name, x => x.Term.ID);
            AddField("Value", x => x.Value);
            AddField("Unit", x => GetUnitName(x), x => GetUnitID(x));
        }

        static string GetUnitName(ParamBase p)
        {
            if (p.Unit != null)
                return p.Unit.Name;
            else
                return string.Empty;
        }

        static string GetUnitID(ParamBase p)
        {
            if (p.Unit != null)
                return p.Unit.ID;
            else
                return string.Empty;
        }
    }
}