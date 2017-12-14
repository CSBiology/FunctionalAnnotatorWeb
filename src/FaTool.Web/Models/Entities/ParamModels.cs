#region license
// The MIT License (MIT)

// ParamModels.cs

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