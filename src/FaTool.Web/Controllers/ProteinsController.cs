﻿#region license
// The MIT License (MIT)

// ProteinsController.cs

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

using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using FaTool.Db;
using FaTool.Web.Models.Entities;
using FaTool.Web.Models.UserInterface;
using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Controllers
{

    public sealed class ProteinsController : FaToolDbControllerBase
    {

        [HttpGet]
        public async Task<ActionResult> Get(Guid id, ProteinNavId navId = ProteinNavId.Functions)
        {
            var protein = await FaToolDbEntities.Find<Protein>(id);
            
            if (protein == null)
                return HttpNotFound("Protein not found in database.");

            ViewBag.Title = string.Format("Protein '{0}' - [{1}]", protein.Name, navId.ToString());

            var model = new EntityView<Protein, ProteinNavId>();
            model.NavTabs = Url.CreateNavTabs<Guid?, ProteinNavId>(id);
            model.ActiveTab = model.NavTabs.ToggleActive(navId);

            model.Properties = new ProteinRecordView(protein);

            switch (navId)
            {
                case ProteinNavId.Functions:
                    await FaToolDbEntities.LoadCollection(protein, x => x.Annotations, "Term.Ontology");
                    var annotationList = new AnnotationList(protein.Annotations);
                    annotationList.AddRowAction(x => Url.EntityGetActionLink("Show", "Annotations", x.ID));
                    annotationList.Actions.Add(Url.ActionLink("Add", "Create", "Annotations", new { proteinId = id }, User.IsAdminRole() == false));
                    model.TabContent = annotationList;
                    break;
                case ProteinNavId.Synonyms:
                    await FaToolDbEntities.LoadCollection(protein, x => x.Synonyms, "SynonymType.Ontology");
                    model.TabContent = new SynonymList(protein.Synonyms);
                    break;
                case ProteinNavId.Description:
                    await FaToolDbEntities.LoadCollection(protein, x => x.Params, "Term.Ontology", "Unit.Ontology");
                    model.TabContent = new ParamList(protein.Params);
                    break;
                case ProteinNavId.References:
                    await FaToolDbEntities.LoadCollection(protein, x => x.References);
                    model.TabContent = new PubReferenceList(protein.References);
                    break;
                case ProteinNavId.GeneModels:
                    await FaToolDbEntities.LoadCollection(protein, x => x.GeneModels, "GeneModelSource.Organism.Ontology");
                    model.TabContent = new GeneModelList(protein.GeneModels);
                    break;
                case ProteinNavId.Homologs:
                    await FaToolDbEntities.LoadCollection(protein, x => x.ProteinHomologyGroups, "Group");
                    model.TabContent = new ProteinHomologyGroupList(protein.ProteinHomologyGroups);
                    break;
                default:
                    throw new InvalidOperationException("Invalid nav id.");
            }

            return View("EntityDefault", model);
        }
    }
}