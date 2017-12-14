using System;
using System.Linq;

namespace FaTool.Db
{

    public enum ProteinSearchOption
    {
        Function,
        Description,
        ProteinName
    }

    internal sealed class SourceProtein
    {
        public string SourceOrganismID { get; set; }
        public Guid? ProteinID { get; set; }
    }

    public sealed class ProteinSearchValue
    {

        public ProteinSearchValue() { }

        public Guid? ProteinID { get; set; }

        public string ProteinName { get; set; }

        public string TermID { get; set; }

        public string TermName { get; set; }

        public string OntologyID { get; set; }

        public string OntologyName { get; set; }

        public string Value { get; set; }
    }

    public static class ProteinSearch
    {

        public static IQueryable<ProteinSearchValue> GetProteinSearchValues(
            this FaToolDbEntities entities,
            ProteinSearchOption option,
            string organismId)
        {
            return from p in entities.GetSourceProteins()
                   join pv in entities.GetProteinSearchValues(option) on p.ProteinID equals pv.ProteinID
                   where p.SourceOrganismID == organismId
                   select pv;
        }               

        public static IQueryable<ProteinSearchValue> GetProteinSearchValues(
            this FaToolDbEntities entities,
            ProteinSearchOption option)
        {
            switch (option)
            {
                case ProteinSearchOption.Function:
                    return GetFunctionValues(entities);
                case ProteinSearchOption.Description:
                    return GetDescriptionValues(entities);
                default:
                    return GetNameSearchValues(entities);
            }
        }

        private static IQueryable<SourceProtein> GetSourceProteins(this FaToolDbEntities entities)
        {
            var query = from gm in entities.GeneModels
                        select new SourceProtein()
                        {
                            SourceOrganismID = gm.GeneModelSource.Organism.ID,
                            ProteinID = gm.Protein.ID
                        };

            return query.Distinct();
        }

        private static IQueryable<ProteinSearchValue> GetNameSearchValues(this FaToolDbEntities entities)
        {
            return entities
                .GetProteinNameValues()
                .Union(entities.GetSynonymValues())
                .Union(entities.GetTranscriptIDValues());
        }

        private static IQueryable<ProteinSearchValue> GetFunctionValues(this FaToolDbEntities entities)
        {
            return from a in entities.Annotations
                   select new ProteinSearchValue()
                   {
                       ProteinID = a.Protein.ID,
                       ProteinName = a.Protein.Name,
                       TermID = a.Term.ID,
                       TermName = a.Term.Name,
                       OntologyID = a.Term.Ontology.ID,
                       OntologyName = a.Term.Ontology.Name,
                       Value = a.Term.Name
                   };
        }

        private static IQueryable<ProteinSearchValue> GetSynonymValues(this FaToolDbEntities entities)
        {
            return from s in entities.Synonyms
                   select new ProteinSearchValue()
                   {
                       ProteinID = s.Protein.ID,
                       ProteinName = s.Protein.Name,
                       TermID = s.SynonymType.ID,
                       TermName = s.SynonymType.Name,
                       OntologyID = s.SynonymType.Ontology.ID,
                       OntologyName = s.SynonymType.Ontology.Name,
                       Value = s.SynonymValue
                   };
        }

        private static IQueryable<ProteinSearchValue> GetDescriptionValues(this FaToolDbEntities entities)
        {
            return from param in entities.ProteinParams
                   select new ProteinSearchValue()
                   {
                       ProteinID = param.ParamContainer.ID,
                       ProteinName = param.ParamContainer.Name,
                       TermID = param.Term.ID,
                       TermName = param.Term.Name,
                       OntologyID = param.Term.Ontology.ID,
                       OntologyName = param.Term.Ontology.Name,
                       Value = param.Value
                   };
        }

        private static IQueryable<ProteinSearchValue> GetTranscriptIDValues(this FaToolDbEntities entities)
        {
            return from gm in entities.GeneModels
                   select new ProteinSearchValue()
                   {
                       ProteinID = gm.Protein.ID,
                       ProteinName = gm.Protein.Name,
                       TermID = null,
                       TermName = null,
                       OntologyID = null,
                       OntologyName = null,
                       Value = gm.TranscriptID
                   };
        }

        private static IQueryable<ProteinSearchValue> GetProteinNameValues(this FaToolDbEntities entities)
        {
            return from p in entities.Proteins
                   select new ProteinSearchValue()
                   {
                       ProteinID = p.ID,
                       ProteinName = p.Name,
                       TermID = null,
                       TermName = null,
                       OntologyID = null,
                       OntologyName = null,
                       Value = p.Name
                   };
        }

    }
}