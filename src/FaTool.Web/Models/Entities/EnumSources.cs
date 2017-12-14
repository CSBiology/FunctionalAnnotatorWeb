#region license
// The MIT License (MIT)

// EnumSources.cs

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
using System.Web.Mvc;
using System.Linq;
using FaTool.Db;
using System.Threading.Tasks;
using System.Data.Entity;
using System;

namespace FaTool.Web.Models.Entities
{

    public static class EnumSources
    {

        public static IEnumerable<SelectListItem> GetProteinSearchOptions(ProteinSearchOption selected)
        {
            return Enum.GetNames(typeof(ProteinSearchOption))
                .Select(x => (ProteinSearchOption)Enum.Parse(typeof(ProteinSearchOption), x, true))
                .Select(x => new SelectListItem 
                { 
                    Text = x.ToString(),
                    Value = x.ToString(), 
                    Selected = x == selected 
                })
                .OrderBy(x => x.Text)
                .ToArray();
        }

        public static async Task<IEnumerable<SelectListItem>> GetSourceOrganismOptions(
            this FaToolDbEntities entities, string selected)
        {
            var query = from gms in entities.GeneModelSources.AsNoTracking()
                        select new SelectListItem()
                        {
                            Value = gms.Organism.ID,
                            Text = gms.Organism.Name,
                            Selected = gms.Organism.ID == selected
                        };

            var orgs = await query.Distinct().OrderBy(x => x.Text).ToArrayAsync();
            return orgs;
        }

        public static async Task<IEnumerable<SelectListItem>> GetOntologyOptions(
            this FaToolDbEntities entities,
            string selected)
        {
            return await entities
                .Ontologies
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem() 
                { 
                    Text = x.Name, 
                    Value = x.ID, 
                    Selected = x.ID == selected 
                })
                .ToArrayAsync();
        }

        public static async Task<IEnumerable<SelectListItem>> GetTermOptions(
            this FaToolDbEntities entities,
            TermQuery query,
            string selected)
        {
            return await entities
                    .Terms
                    .AsNoTracking()
                    .Where(x => x.Name.Contains(query.SearchValue) && x.FK_Ontology == query.OntologyId)
                    .OrderBy(x => x.Name)
                    .Select(x => new SelectListItem()
                    {
                        Value = x.ID,
                        Text = x.Name,
                        Selected = x.ID == selected
                    })
                    .ToArrayAsync();
        }

        public static async Task<IEnumerable<SelectListItem>> GetSelectedTerm(
            this FaToolDbEntities entities, string termId)
        {
            if (string.IsNullOrWhiteSpace(termId))
                return Enumerable.Empty<SelectListItem>();

            return await entities
                    .Terms
                    .AsNoTracking()
                    .Where(x => x.ID == termId)
                    .Select(x => new SelectListItem() { Value = x.ID, Text = x.Name, Selected = true })
                    .ToArrayAsync();
        }

        public static IEnumerable<SelectListItem> GetGoEvidenceOptions(string selected)
        {
            return GOEvidences.Values
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem() { Text = x.Name, Value = x.Code, Selected = x.Code == selected })
                .ToArray();
        }
    }
}