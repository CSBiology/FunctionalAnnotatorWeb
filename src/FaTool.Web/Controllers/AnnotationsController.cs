using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FaTool.Db;
using FaTool.Web.Membership.Identity;
using FaTool.Web.Models.Entities;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Controllers
{

    public class AnnotationsController : FaToolDbControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Get(Guid id, AnnotationNavId navId = AnnotationNavId.Term)
        {

            var annotation = await FaToolDbEntities.Find<Annotation>(id);

            if (annotation == null)
                return HttpNotFound("Entity not found in database.");

            await FaToolDbEntities.LoadReference(annotation, x => x.Term, "Ontology");

            ViewBag.Title = string.Format("Annotation '{0}' - [{1}]", annotation.Term.Name, navId.ToString());

            var model = new EntityView<Annotation, AnnotationNavId>();
            model.NavTabs = Url.CreateNavTabs<Guid?, AnnotationNavId>(id);
            model.ActiveTab = model.NavTabs.ToggleActive(navId);

            model.Properties = new AnnotationRecordView(annotation);
            model.Properties.Actions.Add(Url.EntityActionLink("Edit", "Edit", id, User.IsAdminRole() == false));
            model.Properties.Actions.Add(Url.EntityGetActionLink("Back to Protein", "Proteins", annotation.FK_Protein, ProteinNavId.Functions));

            switch (navId)
            {
                case AnnotationNavId.Term:
                    await FaToolDbEntities.LoadReference(annotation, x => x.Term, "Ontology");
                    model.TabContent = new TermRecordView(annotation.Term);
                    break;
                case AnnotationNavId.References:
                    await FaToolDbEntities.LoadCollection(annotation, x => x.References);
                    model.TabContent = new ParamList(annotation.Params);
                    break;
                case AnnotationNavId.Description:
                    await FaToolDbEntities.LoadCollection(annotation, x => x.Params, "Term.Ontology", "Unit.Ontology");
                    model.TabContent = new PubReferenceList(annotation.References);
                    break;
                default:
                    break;
            }

            return View("EntityDefault", model);
        }

        [HttpGet]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        public async Task<ActionResult> Create(Guid proteinId)
        {

            var annotation = FaToolDbEntities.Init<Annotation, Guid?>(Guid.NewGuid());

            annotation.FK_Protein = proteinId;
            annotation.EntryDate = DateTimeOffset.Now;
            annotation.EvidenceCode = GOEvidences.Default.Code;

            var editArgs = new AnnotationEditArgs();

            editArgs.SaveAction = Url.Action("Insert");
            editArgs.EndEditAction = Url.EntityGetAction("Proteins", proteinId, ProteinNavId.Functions);

            return await Edit(annotation, editArgs);
        }

        [HttpGet]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        public async Task<ActionResult> Edit(Guid id)
        {

            var annotation = await FaToolDbEntities.Find<Annotation>(id);

            if (annotation == null)
                return HttpNotFound("Entity not found in database.");

            var editArgs = new AnnotationEditArgs();
            editArgs.SaveAction = Url.Action("Update");
            editArgs.EndEditAction = Url.EntityGetAction(annotation.ID);

            return await Edit(annotation, editArgs);
        }

        [HttpPost]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Prefix = "Args")] AnnotationEditArgs editArgs)
        {
            if (!ModelState.IsValid)
                return HttpStatusCodeResults.HttpBadRequest(ModelState);

            var annotation = Restore<Annotation>("Entity", true);
            return await Edit(annotation, editArgs);
        }

        [HttpPost]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeTerm(
            [Bind(Prefix = "Args")] AnnotationEditArgs editArgs)
        {
            if (!ModelState.IsValid)
                return HttpStatusCodeResults.HttpBadRequest(ModelState);

            var annotation = Restore<Annotation>("Entity", true);
            return await ChangeTerm(annotation, new TermQuery(), editArgs);
        }

        [HttpPost]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectTerm(
            [Bind(Prefix = "Args")] AnnotationEditArgs editArgs)
        {
            if (!ModelState.IsValid)
                return HttpStatusCodeResults.HttpBadRequest(ModelState);

            var termQuery = new TermQuery();

            if (TryUpdateModel(termQuery, "TermQuery"))
            {
                var annotation = Restore<Annotation>("Entity", true);
                return await SelectTerm(annotation, termQuery, editArgs);
            }
            else
            {
                var annotation = Restore<Annotation>("Entity", true);
                return await ChangeTerm(annotation, termQuery, editArgs);
            }

        }

        [HttpPost]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetTerm(
            [Bind(Prefix = "Args")] AnnotationEditArgs editArgs)
        {
            if (!ModelState.IsValid)
                return HttpStatusCodeResults.HttpBadRequest(ModelState);

            var annotation = Restore<Annotation>("Entity", true);
            var termQuery = new TermQuery();
            TryUpdateModel(termQuery, "TermQuery");
            ModelState.Clear();

            if (string.IsNullOrWhiteSpace(editArgs.SelectedTermId) == false)
            {
                if (await FaToolDbEntities.Exists<Annotation>(x => x.FK_Protein == annotation.FK_Protein && x.FK_Term == editArgs.SelectedTermId))
                {
                    ModelState.AddModelError("Args.SelectedTermId", "Selected function term already mapped to protein.");
                    return await SelectTerm(annotation, termQuery, editArgs);
                }
                else
                {
                    annotation.FK_Term = editArgs.SelectedTermId;
                }
            }
            else
            {
                ModelState.AddModelError("Args.SelectedTermId", "A term must be selected.");
                return await SelectTerm(annotation, termQuery, editArgs);
            }

            return await Edit(annotation, editArgs);
        }

        [HttpPost]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(
            [Bind(Prefix = "Entity.ID")] Guid id,
            [Bind(Prefix = "Entity.RowVersion")] byte[] rowVersion,
            [Bind(Prefix = "Args")] AnnotationEditArgs editArgs)
        {
            if (!ModelState.IsValid)
                return HttpStatusCodeResults.HttpBadRequest(ModelState);

            var annotation = await Find<Annotation, Guid?>(id, "Entity", false);

            if (annotation == null)
                return HttpNotFound("Entity not found in database.");

            if (ModelState.IsValid)
            {
                return await DbUpdateResult(
                    annotation,
                    rowVersion,
                    editArgs.EndEditAction);
            }
            else
            {
                return await Edit(annotation, editArgs);
            }
        }

        [HttpPost]
        [Authorize(Roles = FaToolRoles.ANNOTATOR)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insert(
            [Bind(Prefix = "Args")] AnnotationEditArgs editArgs)
        {

            if (!ModelState.IsValid)
                return HttpStatusCodeResults.HttpBadRequest(ModelState);

            var annotation = Restore<Annotation>("Entity", false);

            if (ModelState.IsValid)
            {
                return await DbInsertResult(
                    annotation,
                    editArgs.EndEditAction);
            }
            else
            {
                return await Edit(annotation, editArgs);
            }
        }

        #region helpers

        private async Task<ActionResult> Edit(Annotation annotation, AnnotationEditArgs editArgs)
        {
            ViewBag.Title = "Edit Annotation";

            var model = new AnnotationEditViewModel();
            model.Args = editArgs;
            model.Entity = annotation;
            model.GoEvidenceOptions = EnumSources.GetGoEvidenceOptions(annotation.EvidenceCode);
            model.TermOptions = await FaToolDbEntities.GetSelectedTerm(annotation.FK_Term);

            return View("Edit", model);
        }

        private async Task<ActionResult> ChangeTerm(Annotation annotation, TermQuery termQuery, AnnotationEditArgs editArgs)
        {
            ViewBag.Title = "Change Term";

            var model = new AnnotationEditViewModel();
            model.Args = editArgs;
            model.Entity = annotation;
            model.TermQuery = termQuery;
            model.OntologyOptions = await FaToolDbEntities.GetOntologyOptions(model.TermQuery.OntologyId);

            return View("ChangeTerm", model);
        }

        private async Task<ActionResult> SelectTerm(Annotation annotation, TermQuery termQuery, AnnotationEditArgs editArgs)
        {
            ViewBag.Title = "Select Term";

            var model = new AnnotationEditViewModel();
            model.Args = editArgs;
            model.Entity = annotation;
            model.TermQuery = termQuery;
            model.OntologyOptions = await FaToolDbEntities.GetOntologyOptions(model.TermQuery.OntologyId);
            model.TermOptions = await FaToolDbEntities.GetTermOptions(termQuery, annotation.FK_Term);

            if (model.TermOptions.Any())
            {
                return View("SelectTerm", model);
            }
            else
            {
                ModelState.AddModelError("TermQuery.SearchValue", "No Terms found.");
                return View("ChangeTerm", model);
            }
        }

        #endregion
    }


}