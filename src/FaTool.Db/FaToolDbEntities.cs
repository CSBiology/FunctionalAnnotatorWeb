using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace FaTool.Db
{

    public sealed class FaToolDbEntities : DbContext
    {

        public FaToolDbEntities()
            : base("FaToolDbConnectionString")
        {
            base.Configuration.ProxyCreationEnabled = true;
            base.Configuration.LazyLoadingEnabled = false;
            base.Configuration.AutoDetectChangesEnabled = false;
            this.CreateDbSets();
        }

        internal ObjectContext Core
        {
            get
            {
                return ((IObjectContextAdapter)this).ObjectContext;
            }
        }

        #region DbContext override

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.MapSchema();
        }

        #endregion

        #region Db Sets

        private void CreateDbSets()
        {
            PubReferences = Set<PubReference>();
            Annotations = Set<Annotation>();
            AnnotationParams = Set<AnnotationParam>();
            GeneModelSources = Set<GeneModelSource>();
            GeneModels = Set<GeneModel>();
            GeneModelParams = Set<GeneModelParam>();
            Proteins = Set<Protein>();
            ProteinParams = Set<ProteinParam>();
            Synonyms = Set<Synonym>();
            SynonymParams = Set<SynonymParam>();
            Terms = Set<Term>();
            Ontologies = Set<Ontology>();
            HomologyGroups = Set<HomologyGroup>();
            HomologyGroupParams = Set<HomologyGroupParam>();
            ProteinHomologyGroups = Set<ProteinHomologyGroup>();
            ProteinHomologyGroupParams = Set<ProteinHomologyGroupParam>();
        }


        public DbSet<PubReference> PubReferences
        {
            get;
            private set;
        }

        public DbSet<Annotation> Annotations
        {
            get;
            private set;
        }

        public DbSet<AnnotationParam> AnnotationParams
        {
            get;
            private set;
        }

        public DbSet<GeneModelSource> GeneModelSources
        {
            get;
            private set;
        }

        public DbSet<GeneModel> GeneModels
        {
            get;
            private set;
        }

        public DbSet<GeneModelParam> GeneModelParams
        {
            get;
            private set;
        }

        public DbSet<Protein> Proteins
        {
            get;
            private set;
        }

        public DbSet<ProteinParam> ProteinParams
        {
            get;
            private set;
        }

        public DbSet<Synonym> Synonyms
        {
            get;
            private set;
        }

        public DbSet<SynonymParam> SynonymParams
        {
            get;
            private set;
        }

        public DbSet<Term> Terms
        {
            get;
            private set;
        }

        public DbSet<Ontology> Ontologies
        {
            get;
            private set;
        }

        public DbSet<HomologyGroup> HomologyGroups
        {
            get;
            private set;
        }

        public DbSet<HomologyGroupParam> HomologyGroupParams
        {
            get;
            private set;
        }

        public DbSet<ProteinHomologyGroup> ProteinHomologyGroups
        {
            get;
            private set;
        }

        public DbSet<ProteinHomologyGroupParam> ProteinHomologyGroupParams
        {
            get;
            private set;
        }

        #endregion
    }

    #region EntityBase

    public interface IHasRowVersion
    {
        byte[] RowVersion { get; set; }
    }

    public interface IIdentity
    {
        object ID { get; }
    }

    public interface IIdentity<TKey> : IIdentity
    {
        new TKey ID { get; set; }
    }

    public interface IEntity : IIdentity, IHasRowVersion { }
    public interface IEntity<TKey> : IEntity, IIdentity<TKey> { }

    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        protected internal EntityBase() { }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("FaTool Database ID")]
        public virtual TKey ID { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        public virtual byte[] RowVersion { get; set; }

        #region IIdentityBase Members

        object IIdentity.ID
        {
            get { return ID; }
        }

        #endregion
    }

    #endregion

    #region ParamBase/ParamContainer

    public abstract class ParamBase
        : EntityBase<Guid?>
    {

        protected internal ParamBase() { }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Value")]
        public virtual string Value { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Term")]
        public virtual string FK_Term { get; set; }

        public virtual Term Term { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Unit")]
        public virtual string FK_Unit { get; set; }

        public virtual Term Unit { get; set; }
    }

    public abstract class ParamBase<TParamContainer>
        : ParamBase
        where TParamContainer : ParamContainer
    {

        protected internal ParamBase() { }

        [Required]
        [DisplayName("Param Container")]
        public virtual Guid? FK_ParamContainer { get; set; }

        public virtual TParamContainer ParamContainer { get; set; }
    }

    public abstract class ParamContainer
        : EntityBase<Guid?>
    {
        protected internal ParamContainer() { }
    }

    public abstract class ParamContainer<TParam>
        : ParamContainer
        where TParam : ParamBase
    {
        protected internal ParamContainer() { }

        public virtual ICollection<TParam> Params { get; set; }
    }

    #endregion

    #region PubReference

    public class PubReference : EntityBase<Guid?>
    {

        public PubReference() : base() { }

        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public virtual string DOI { get; set; }

    }

    #endregion

    #region Ontology

    public class Ontology : EntityBase<string>
    {

        public Ontology() : base() { }

        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }
    }

    public class Term : EntityBase<string>
    {

        public Term() : base() { }

        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Ontology")]
        public virtual string FK_Ontology { get; set; }

        public virtual Ontology Ontology { get; set; }
    }

    #endregion

    #region Annotation

    public class Annotation : ParamContainer<AnnotationParam>
    {
        public Annotation() : base() { }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("GO Evidence Code")]
        public virtual string EvidenceCode { get; set; }

        [Required()]
        [DisplayName("Entry Date")]
        public virtual DateTimeOffset EntryDate { get; set; }

        [Required]
        [DisplayName("Protein")]
        public virtual Guid? FK_Protein { get; set; }

        public virtual Protein Protein { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Term")]
        public virtual string FK_Term { get; set; }

        public virtual Term Term { get; set; }

        public virtual ICollection<PubReference> References { get; set; }
    }

    public class AnnotationParam : ParamBase<Annotation>
    {
        public AnnotationParam() : base() { }
    }

    #endregion

    #region Protein

    public class Protein : ParamContainer<ProteinParam>
    {
        public Protein() : base() { }

        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }

        public virtual ICollection<Synonym> Synonyms { get; set; }

        public virtual ICollection<Annotation> Annotations { get; set; }

        public virtual ICollection<PubReference> References { get; set; }

        public virtual ICollection<GeneModel> GeneModels { get; set; }

        public virtual ICollection<ProteinHomologyGroup> ProteinHomologyGroups { get; set; }
    }

    public class ProteinParam : ParamBase<Protein>
    {
        public ProteinParam() : base() { }
    }

    #endregion

    #region Synonym

    public class Synonym : ParamContainer<SynonymParam>
    {

        public Synonym() : base() { }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Synonym Value")]
        public virtual string SynonymValue { get; set; }

        [Required()]
        [DisplayName("Protein")]
        public virtual Guid? FK_Protein { get; set; }
        public virtual Protein Protein { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Synonym Type")]
        public virtual string FK_SynonymType { get; set; }
        public virtual Term SynonymType { get; set; }

    }

    public class SynonymParam : ParamBase<Synonym>
    {
        public SynonymParam() : base() { }
    }

    #endregion

    #region GeneModel

    public class GeneModel : ParamContainer<GeneModelParam>
    {

        public GeneModel() : base() { }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Transcript ID")]
        public virtual string TranscriptID { get; set; }

        [Required()]
        [DisplayName("Gene Model Source")]
        public virtual Guid? FK_GeneModelSource { get; set; }
        public virtual GeneModelSource GeneModelSource { get; set; }

        [Required()]
        [DisplayName("Protein")]
        public virtual Guid? FK_Protein { get; set; }
        public virtual Protein Protein { get; set; }
    }

    public class GeneModelParam : ParamBase<GeneModel>
    {
        public GeneModelParam() : base() { }
    }

    public class GeneModelSource : EntityBase<Guid?>
    {

        public GeneModelSource() : base() { }

        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Source Organism")]
        public virtual string FK_Organism { get; set; }
        public virtual Term Organism { get; set; }

        public virtual ICollection<GeneModel> GeneModels { get; set; }

    }

    #endregion

    #region Homology

    public class HomologyGroup : ParamContainer<HomologyGroupParam>
    {
        public HomologyGroup() : base() { }

        [Required(AllowEmptyStrings = false)]
        public virtual string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DisplayName("Homology Type")]
        public virtual string HomologyType { get; set; }

        public virtual ICollection<ProteinHomologyGroup> ProteinHomologyGroups { get; set; }
    }

    public class HomologyGroupParam : ParamBase<HomologyGroup>
    {
        public HomologyGroupParam() : base() { }
    }

    public class ProteinHomologyGroup : ParamContainer<ProteinHomologyGroupParam>
    {
        public ProteinHomologyGroup() : base() { }

        [Required()]
        [DisplayName("Protein")]
        public virtual Guid? FK_Protein { get; set; }
        public virtual Protein Protein { get; set; }

        [Required()]
        [DisplayName("Homology Group")]
        public virtual Guid? FK_Group { get; set; }
        public virtual HomologyGroup Group { get; set; }
    }

    public class ProteinHomologyGroupParam : ParamBase<ProteinHomologyGroup>
    {
        public ProteinHomologyGroupParam() : base() { }
    }

    #endregion
}