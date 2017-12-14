﻿#region license
// The MIT License (MIT)

// GeneModelModels.cs

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