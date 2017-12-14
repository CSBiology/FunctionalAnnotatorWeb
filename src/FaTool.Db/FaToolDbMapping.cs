using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FaTool.Db
{
    internal static class FaToolDbMapping
    {
        
        public static void MapSchema(this DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            
            modelBuilder
                .MapHomologyGroup()
                .MapProteinHomologyGroup()
                .MapPubReference()
                .MapOntology()
                .MapTerm()
                .MapProtein()
                .MapAnnotation()
                .MapGeneModel()
                .MapGeneModelSource()
                .MapSynonym();
            
        }

        #region entity mapping

        private static DbModelBuilder MapHomologyGroup(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapParamContainer<FaToolDbEntities, HomologyGroup, HomologyGroupParam>(
                x => x.HomologyGroups, y => y.HomologyGroupParams);
            cfg.Property(x => x.Name).HasColumnName("Name").IsRequired();
            cfg.Property(x => x.HomologyType).HasColumnName("HomologyType").IsRequired();
            return modelBuilder;
        }

        private static DbModelBuilder MapProteinHomologyGroup(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapParamContainer<FaToolDbEntities, ProteinHomologyGroup, ProteinHomologyGroupParam>(
                x => x.ProteinHomologyGroups, y => y.ProteinHomologyGroupParams);
            cfg.HasRequired(x => x.Protein).WithMany(x => x.ProteinHomologyGroups).HasForeignKey(x => x.FK_Protein);
            cfg.HasRequired(x => x.Group).WithMany(x => x.ProteinHomologyGroups).HasForeignKey(x => x.FK_Group);
            return modelBuilder;
        }

        private static DbModelBuilder MapPubReference(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapGuidKeyEntitySet<FaToolDbEntities, PubReference>(x => x.PubReferences);

            cfg.Property(x => x.Name).HasColumnName("Name").IsRequired();
            cfg.Property(x => x.DOI).HasColumnName("DOI").IsRequired();

            return modelBuilder;
        }

        private static DbModelBuilder MapAnnotation(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapParamContainer<FaToolDbEntities, Annotation, AnnotationParam>(
                x => x.Annotations, y => y.AnnotationParams);

            cfg.Property(x => x.EvidenceCode).HasColumnName("EvidenceCode").IsRequired();
            cfg.Property(x => x.EntryDate).HasColumnName("EntryDate").IsRequired();
            cfg.HasRequired(x => x.Protein).WithMany(x => x.Annotations).HasForeignKey(x => x.FK_Protein);
            cfg.HasRequired(x => x.Term).WithMany().HasForeignKey(x => x.FK_Term);
            cfg.HasMany(x => x.References).WithMany()
                .MapManyToMany("FK_Annotation", "FK_PubReference", "AnnotationPubReference");

            return modelBuilder;
        }

        private static DbModelBuilder MapOntology(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder.MapStringKeyEntitySet<FaToolDbEntities, Ontology>(x => x.Ontologies);
            cfg.Property(x => x.Name).HasColumnName("Name").IsRequired();
            return modelBuilder;
        }

        private static DbModelBuilder MapTerm(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder.MapStringKeyEntitySet<FaToolDbEntities, Term>(x => x.Terms);
            cfg.Property(x => x.Name).HasColumnName("Name").IsRequired();
            cfg.HasRequired(x => x.Ontology).WithMany().HasForeignKey(x => x.FK_Ontology);
            return modelBuilder;
        }

        private static DbModelBuilder MapProtein(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapParamContainer<FaToolDbEntities, Protein, ProteinParam>(
                x => x.Proteins, y => y.ProteinParams);

            cfg.Property(x => x.Name).HasColumnName("Name").IsRequired();
            cfg.HasMany(x => x.Synonyms).WithRequired(x => x.Protein);
            cfg.HasMany(x => x.Annotations).WithRequired(x => x.Protein);
            cfg.HasMany(x => x.References).WithMany()
                .MapManyToMany("FK_Protein", "FK_PubReference", "ProteinPubReference");
            cfg.HasMany(x => x.GeneModels).WithRequired(x => x.Protein);

            return modelBuilder;
        }

        private static DbModelBuilder MapSynonym(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapParamContainer<FaToolDbEntities, Synonym, SynonymParam>(
                x => x.Synonyms, y => y.SynonymParams);

            cfg.Property(x => x.SynonymValue).HasColumnName("SynonymValue").IsRequired();
            cfg.HasRequired(x => x.Protein).WithMany(x => x.Synonyms).HasForeignKey(x => x.FK_Protein);
            cfg.HasRequired(x => x.SynonymType).WithMany().HasForeignKey(x => x.FK_SynonymType);

            return modelBuilder;
        }

        private static DbModelBuilder MapGeneModel(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder
                .MapParamContainer<FaToolDbEntities, GeneModel, GeneModelParam>(
                x => x.GeneModels, y => y.GeneModelParams);

            cfg.Property(x => x.TranscriptID).HasColumnName("TranscriptID").IsRequired();
            cfg.HasRequired(x => x.Protein).WithMany(x => x.GeneModels).HasForeignKey(x => x.FK_Protein);
            cfg.HasRequired(x => x.GeneModelSource).WithMany(x => x.GeneModels).HasForeignKey(x => x.FK_GeneModelSource);

            return modelBuilder;
        }

        private static DbModelBuilder MapGeneModelSource(this DbModelBuilder modelBuilder)
        {
            var cfg = modelBuilder.MapGuidKeyEntitySet<FaToolDbEntities, GeneModelSource>(x => x.GeneModelSources);

            cfg.Property(x => x.Name).HasColumnName("Name").IsRequired();
            cfg.HasRequired(x => x.Organism).WithMany().HasForeignKey(x => x.FK_Organism);
            cfg.HasMany(x => x.GeneModels).WithRequired(x => x.GeneModelSource);

            return modelBuilder;
        }

        private static EntityTypeConfiguration<TParamContainer> MapParamContainer<TContext, TParamContainer, TParam>(
            this DbModelBuilder modelBuilder,
            Expression<Func<TContext, IQueryable<TParamContainer>>> pcProperty,
            Expression<Func<TContext, IQueryable<TParam>>> paramProperty)
            where TParamContainer : ParamContainer<TParam>
            where TParam : ParamBase<TParamContainer>
        {
            var pc = modelBuilder.MapGuidKeyEntitySet<TContext, TParamContainer>(pcProperty);

            pc.HasMany(x => x.Params).WithRequired(x => x.ParamContainer);

            var param = modelBuilder.MapGuidKeyEntitySet<TContext, TParam>(paramProperty);

            param.Property(x => x.Value).HasColumnName("Value").IsRequired();
            
            param.HasRequired(x => x.Term).WithMany().HasForeignKey(x => x.FK_Term);
            param.Property(x => x.FK_Unit).IsOptional();
            param.HasOptional(x => x.Unit).WithMany().HasForeignKey(x => x.FK_Unit);
            param.HasRequired(x => x.ParamContainer).WithMany(x => x.Params).HasForeignKey(x => x.FK_ParamContainer);

            return pc;
        }

        private static EntityTypeConfiguration<TEntity> MapGuidKeyEntitySet<TContext, TEntity>(
            this DbModelBuilder builder,
            Expression<Func<TContext, IQueryable<TEntity>>> property)
            where TEntity : EntityBase<Guid?>
        {
            var cfg = builder
                .MapEntitySet<TContext, TEntity>(property);

            cfg.HasKey(x => x.ID)
                .Property(x => x.ID)
                .HasColumnName("ID")
                .IsRequired();

            return cfg;
        }

        private static EntityTypeConfiguration<TEntity> MapStringKeyEntitySet<TContext, TEntity>(
            this DbModelBuilder builder,
            Expression<Func<TContext, IQueryable<TEntity>>> property)
            where TEntity : EntityBase<string>
        {
            var cfg = builder
                .MapEntitySet<TContext, TEntity>(property);

            cfg.HasKey(x => x.ID)
                .Property(x => x.ID)
                .HasColumnName("ID")
                .IsRequired();

            return cfg;
        }

        private static EntityTypeConfiguration<TEntity> MapEntitySet<TContext, TEntity>(
            this DbModelBuilder builder,
            Expression<Func<TContext, IQueryable<TEntity>>> property)
            where TEntity : class, IHasRowVersion
        {
            string entitySetName = GetPropertyName(property);
            var cfg = builder
                .Entity<TEntity>()
                .HasEntitySetName(entitySetName)
                .ToTable(typeof(TEntity).Name);

            cfg.Property(x => x.RowVersion)
                .IsRowVersion()
                .HasColumnName("RowVersion")
                .IsRequired();

            return cfg;
        }

        private static ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> MapManyToMany
            <TEntityType, TTargetEntityType>(
            this ManyToManyNavigationPropertyConfiguration<TEntityType, TTargetEntityType> cfg,
            string leftKey,
            string rightKey,
            string table)
            where TEntityType : class
            where TTargetEntityType : class
        {
            return cfg.Map(m => m.MapLeftKey(leftKey).MapRightKey(rightKey).ToTable(table));
        }

        #endregion

        #region helpers

        private static string GetPropertyName<T, TProperty>(
            Expression<Func<T, TProperty>> property)
        {
            MemberExpression member = property.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression body '{0}' is not a member expression.",
                    property.Body.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    property.ToString()));
            return propInfo.Name;
        }

        #endregion
    }

}