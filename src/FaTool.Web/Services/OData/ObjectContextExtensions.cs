﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;

namespace FaTool.Web.Services.OData
{
    internal static class ObjectContextExtensions
    {

        public static object CreateObject(
            this ObjectContext oc, 
            Type type)
        {
            if (oc.ContextOptions.ProxyCreationEnabled)
            {
                return ReflectionHelper.Invoke(
                    oc,
                    "CreateObject",
                    new Type[] { type });
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }

        public static IQueryable GetQuery(
            this ObjectContext oc,
            string entitySetName)
        {

            EntitySet entitySet = oc.MetadataWorkspace
                .GetEntityContainer(oc.DefaultContainerName, DataSpace.CSpace)
                .EntitySets[entitySetName];

            EntityType entityType = oc.MetadataWorkspace.GetItem<EntityType>(
                entitySet.ElementType.FullName,
                DataSpace.OSpace);

            ObjectItemCollection objectItems =
                oc.MetadataWorkspace.GetItemCollection(DataSpace.OSpace) as ObjectItemCollection;

            Type clrType = objectItems.GetClrType(entityType);

            return GetQuery(oc, clrType, entitySetName);
        }

        public static IQueryable GetQuery(
            this ObjectContext oc, 
            Type elementType, 
            string entitySetName)
        {
            return ReflectionHelper.Invoke(
                oc,
                "CreateObjectSet",
                new Type[] { elementType },
                entitySetName) as IQueryable;
        }

        public static void SetReference(
            this ObjectContext oc,
            object target,
            string propertyName,
            object propertyValue)
        {
            EntityType entityType = oc.MetadataWorkspace.GetItem<EntityType>(
                target.GetType().FullName,
                DataSpace.CSpace);
            NavigationProperty navProperty = entityType.NavigationProperties[propertyName];
            ObjectStateEntry entry = oc.ObjectStateManager.GetObjectStateEntry(target);
            EntityReference relatedEnd = (EntityReference)entry.RelationshipManager
                .GetRelatedEnd(navProperty.RelationshipType.Name, navProperty.ToEndMember.Name);

            if (entry.State != EntityState.Added && !relatedEnd.IsLoaded)
            {
                relatedEnd.Load();
            }

            if (propertyValue == null)
            {
                relatedEnd.EntityKey = null;
            }
            else
            {
                EqualityComparer<EntityKey> keycomp = EqualityComparer<EntityKey>.Default;
                EntityKey relatedKey = oc.ObjectStateManager.GetObjectStateEntry(target).EntityKey;
                if (!keycomp.Equals(relatedEnd.EntityKey, relatedKey))
                    ReflectionHelper.SetProperty(relatedEnd, "Value", propertyValue);
            }
        }

        public static void AddReferenceToCollection(
            this ObjectContext oc,
            object target,
            string propertyName,
            object resourceToBeAdded)
        {
            EntityType entityType = oc.MetadataWorkspace.GetItem<EntityType>(
                target.GetType().FullName,
                DataSpace.CSpace);
            NavigationProperty navProperty = entityType.NavigationProperties[propertyName];
            ObjectStateEntry entry = oc.ObjectStateManager.GetObjectStateEntry(target);
            IRelatedEnd relatedEnd = entry.RelationshipManager
                .GetRelatedEnd(navProperty.RelationshipType.Name, navProperty.ToEndMember.Name);

            relatedEnd.Add(resourceToBeAdded);
        }

        public static void RemoveReferenceFromCollection(
            this ObjectContext oc,
            object target,
            string propertyName,
            object toBeRemoved)
        {

            EntityType entityType = oc.MetadataWorkspace.GetItem<EntityType>(
                target.GetType().FullName,
                DataSpace.CSpace);
            NavigationProperty navProperty = entityType.NavigationProperties[propertyName];
            ObjectStateEntry entry = oc.ObjectStateManager.GetObjectStateEntry(target);
            IRelatedEnd relatedEnd = entry.RelationshipManager
                .GetRelatedEnd(navProperty.RelationshipType.Name, navProperty.ToEndMember.Name);

            relatedEnd.Attach(toBeRemoved);
            relatedEnd.Remove(toBeRemoved);
        }

        public static void SetValue(
            this ObjectContext oc,
            object target,
            string propertyName,
            object propertyValue)
        {
            ObjectStateEntry objectStateEntry = oc
                .ObjectStateManager
                .GetObjectStateEntry(target);

            CurrentValueRecord currentValues = objectStateEntry.CurrentValues;
            int propertyOrdinal = currentValues.GetOrdinal(propertyName);
            currentValues.SetValue(propertyOrdinal, propertyValue);
        }

        public static object GetValue(
            this ObjectContext oc,
            object target,
            string propertyName)
        {
            ObjectStateEntry objectStateEntry = oc
                .ObjectStateManager
                .GetObjectStateEntry(target);

            return objectStateEntry.CurrentValues[propertyName];
        }

        public static void ClearChanges(
            this ObjectContext oc)
        {

            IEnumerable<ObjectStateEntry> entries = oc.ObjectStateManager.GetObjectStateEntries(
                EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged);

            foreach (ObjectStateEntry entry in entries)
            {
                if (entry.State != EntityState.Detached && !entry.IsRelationship && entry.Entity != null)
                {
                    oc.Detach(entry.Entity);
                }
            }

        }

        public static void ResetProperties(
            this ObjectContext oc,
            object targetObject)
        {

            EntityType entityType = oc.MetadataWorkspace.GetItem<EntityType>(
                targetObject.GetType().FullName,
                DataSpace.CSpace);

            ObjectStateEntry objectStateEntry = oc
                .ObjectStateManager
                .GetObjectStateEntry(targetObject);

            CurrentValueRecord currentValues = objectStateEntry.CurrentValues;

            object dummy = Activator.CreateInstance(targetObject.GetType());

            foreach (EdmProperty p in entityType.Properties.Where(x => x.IsPrimitiveType || x.IsComplexType))
            {
                bool isKey = entityType.KeyProperties.Any(x => x.Name == p.Name);

                if (!isKey)
                {
                    object v = ReflectionHelper.GetProperty(dummy, p.Name);
                    int propertyOrdinal = currentValues.GetOrdinal(p.Name);
                    currentValues.SetValue(propertyOrdinal, v);
                }
            }

        }
    }
}