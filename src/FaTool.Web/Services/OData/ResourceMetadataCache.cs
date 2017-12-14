#region license
// The MIT License (MIT)

// ResourceMetadataCache.cs

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
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Services;
using System.Data.Services.Providers;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using Microsoft.Data.Edm;

namespace FaTool.Web.Services.OData
{

    /// <summary>
    /// Provides caching of resource metadata from entity framework metadata workspaces.
    /// </summary>
    /// <typeparam name="TService">The type of the service to reflect service operations.</typeparam>
    /// <typeparam name="TContext">The type of data context to reflect container namespace.</typeparam>
    public sealed class ResourceMetadataCache<TService, TContext>
        where TContext : IObjectContextAdapter
        where TService : DataService<TContext>
    {
        private readonly object mutex = new object();
        private bool hasMetadataInitialized = false;
        private readonly IDictionary<string, ResourceType> resourceTypes =
            new Dictionary<string, ResourceType>();
        private readonly IDictionary<string, ResourceSet> resourceSets =
            new Dictionary<string, ResourceSet>();
        private readonly IDictionary<string, ResourceAssociationSet> associationSets =
            new Dictionary<string, ResourceAssociationSet>();
        private readonly IDictionary<string, ServiceOperation> serviceOperations =
            new Dictionary<string, ServiceOperation>();

        public ResourceMetadataCache() { }

        /// <summary>
        /// Get the object context default container name.
        /// </summary>        
        public string GetContainerName(TContext adapter)
        {
            return adapter.ObjectContext.DefaultContainerName;
        }

        /// <summary>
        /// Reflects the container namespace from context type.
        /// </summary>        
        public string GetContainerNamespace()
        {
            return typeof(TContext).Namespace;
        }

        /// <summary>
        /// Lazy create all resource types from entity types and complex types 
        /// declared in metadata workspace model. 
        /// </summary> 
        public IEnumerable<ResourceType> LazyGetResourceTypes(TContext adapter)
        {
            Initialize(adapter);
            return resourceTypes.Values;
        }

        /// <summary>
        /// Lazy create all resource sets from entity sets 
        /// declared in metadata workspace model. 
        /// </summary>        
        public IEnumerable<ResourceSet> LazyGetResourceSets(TContext adapter)
        {
            Initialize(adapter);
            return resourceSets.Values;
        }

        /// <summary>
        /// Lazy reflects all public service operations 
        /// declared on specified service type.
        /// </summary>        
        public IEnumerable<ServiceOperation> LazyGetServiceOperations(TContext adapter)
        {
            Initialize(adapter);
            return serviceOperations.Values;
        }

        /// <summary>
        /// Lazy try get the resource set for specified entity set name.
        /// </summary>        
        public bool LazyTryGetResourceSet(
            TContext adapter,
            string name,
            out ResourceSet resourceSet)
        {
            Initialize(adapter);
            return resourceSets.TryGetValue(name, out resourceSet);
        }

        /// <summary>
        /// Lazy try get the resource type for specified 
        /// entity or complex type name.
        /// </summary> 
        public bool LazyTryGetResourceType(
            TContext adapter,
            string name,
            out ResourceType resourceType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            Initialize(adapter);
            return resourceTypes.TryGetValue(name, out resourceType);
        }

        /// <summary>
        /// Lazy get the resource type for specified 
        /// entity or complex clr type.
        /// </summary> 
        public ResourceType LazyGetResourceType(
            TContext adapter,
            Type clrType)
        {
            if (clrType == null)
                throw new ArgumentNullException("clrType");
            Initialize(adapter);

            ResourceType rt;

            if (resourceTypes.TryGetValue(clrType.FullName, out rt))
            {
                return rt;
            }

            if (adapter.ObjectContext.ContextOptions.ProxyCreationEnabled)
            {
                if (resourceTypes.TryGetValue(ObjectContext.GetObjectType(clrType).FullName, out rt))
                    return rt;
            }

            throw new InvalidOperationException(
                string.Format("No resource type found for clr type '{0}'.",
                clrType.FullName));
        }

        /// <summary>
        /// Lazy try get the service operation of specified 
        /// name declared on the service type.
        /// </summary> 
        public bool LazyTryGetServiceOperation(
            TContext adapter,
            string name,
            out ServiceOperation serviceOperation)
        {
            Initialize(adapter);
            return serviceOperations.TryGetValue(name, out serviceOperation);
        }

        /// <summary>
        /// Lazy get the association set for specified navigation property.
        /// </summary>        
        public ResourceAssociationSet LazyGetResourceAssociationSet(
            TContext adapter,
            ResourceSet resourceSet,
            ResourceType resourceType,
            ResourceProperty resourceProperty)
        {

            if (IsNavigationProperty(resourceProperty) == false)
                throw new ArgumentException(
                    string.Format("Property '{0}' is not a navigation property.",
                    resourceProperty.Name),
                        "resourceProperty");

            Initialize(adapter);

            ResourceAssociationSet resourceAssocSet =
                associationSets.Values
                .Where(x => HasAssociationEndOf(x, resourceSet, resourceType, resourceProperty))
                .First();

            return resourceAssocSet;
        }

        private void Initialize(TContext adapter)
        {

            lock (mutex)
            {

                if (hasMetadataInitialized || adapter == null)
                    return;

                // (1.) Create all types

                MetadataWorkspace workspace = adapter.ObjectContext.MetadataWorkspace;
                IEnumerable<EdmType> edmTypes = workspace
                    .GetItems<EdmType>(DataSpace.CSpace)
                    .Where(x => x.BuiltInTypeKind == BuiltInTypeKind.ComplexType ||
                        x.BuiltInTypeKind == BuiltInTypeKind.EntityType);

                foreach (ComplexType edmType in edmTypes.OfType<ComplexType>())
                    GetCreateComplexResourceType(workspace, edmType);
                foreach (EntityType edmType in edmTypes.OfType<EntityType>())
                    GetCreateEntityResourceType(workspace, edmType);

                // (2.) Create all entity sets

                string containerName = GetContainerName(adapter);
                IEnumerable<EntitySet> entitySets = workspace
                    .GetEntityContainer(containerName, DataSpace.CSpace)
                    .EntitySets;

                foreach (EntitySet entitySet in entitySets)
                    GetCreateResourceSet(workspace, entitySet);

                // (3.) Create all associations

                IEnumerable<AssociationSet> assocSets = workspace
                    .GetEntityContainer(GetContainerName(adapter), DataSpace.CSpace)
                    .AssociationSets;

                foreach (AssociationSet assocSet in assocSets.OrderBy(x => x.Name))
                    GetCreateAssociationSet(workspace, assocSet);

                //foreach (AssociationSet assocSet in assocSets)
                //    CreateConstraints(workspace, assocSet);

                // (4.) Create all service operations

                IEnumerable<MethodInfo> getOps = ReflectionHelper.GetDeclaredMethods<TService, WebGetAttribute>();

                foreach (MethodInfo mi in getOps)
                    CreateServiceOperation(mi, "GET");

                IEnumerable<MethodInfo> postOps = ReflectionHelper.GetDeclaredMethods<TService, WebInvokeAttribute>();

                foreach (MethodInfo mi in postOps)
                    CreateServiceOperation(mi, "POST");

                // (5.) Seal metadata types

                foreach (var rt in resourceTypes.Values)
                    rt.SetReadOnly();

                foreach (var rs in resourceSets.Values)
                    rs.SetReadOnly();

                foreach (var o in serviceOperations.Values)
                    o.SetReadOnly();

                hasMetadataInitialized = true;
            }
        }

        private ResourceSet GetCreateResourceSet(
            MetadataWorkspace workspace,
            EntitySet entitySet)
        {
            ResourceSet rs = null;

            if (resourceSets.TryGetValue(entitySet.Name, out rs) == false)
            {
                ResourceType elementType = GetCreateEntityResourceType(workspace, entitySet.ElementType);
                rs = new ResourceSet(entitySet.Name, elementType);
                resourceSets.Add(entitySet.Name, rs);
            }

            return rs;
        }

        private ResourceSet GetResourceSet(ResourceType type)
        {

            ResourceType resourceType = type;
            ResourceSet set = null;

            while (resourceType != null)
            {
                set = resourceSets
                    .Values
                    .FirstOrDefault(x => x.ResourceType == resourceType);

                if (set != null)
                    return set;
                else
                    resourceType = resourceType.BaseType;
            }

            if (set == null)
                throw new InvalidOperationException(
                    string.Format("No resource set found for resource type '{0}'.",
                    type.FullName));

            return set;
        }

        private ResourceType GetCreateEntityResourceType(
            MetadataWorkspace workspace,
            EntityType edmType)
        {

            if (edmType == null)
            {
                return null;
            }
            else
            {
                ResourceType resourceType = null;

                if (resourceTypes.TryGetValue(edmType.FullName, out resourceType) == false)
                {
                    ResourceType baseType = GetCreateEntityResourceType(workspace, edmType.BaseType as EntityType);
                    Type clrType = GetClrType(workspace, edmType);

                    resourceType = new ResourceType(
                        clrType,
                        ResourceTypeKind.EntityType,
                        baseType,
                        edmType.NamespaceName,
                        edmType.Name,
                        edmType.Abstract);

                    resourceTypes.Add(edmType.FullName, resourceType);
                    AddProperties(workspace, edmType, resourceType);
                }

                return resourceType;
            }
        }

        private ResourceType GetCreateComplexResourceType(
            MetadataWorkspace workspace,
            ComplexType edmType)
        {
            if (edmType == null)
            {
                return null;
            }
            else
            {
                ResourceType resourceType = null;

                if (resourceTypes.TryGetValue(edmType.FullName, out resourceType) == false)
                {
                    Type clrType = GetClrType(workspace, edmType);

                    resourceType = new ResourceType(
                        clrType,
                        ResourceTypeKind.ComplexType,
                        null,
                        edmType.NamespaceName,
                        edmType.Name,
                        edmType.Abstract);

                    resourceTypes.Add(edmType.FullName, resourceType);
                    AddProperties(workspace, edmType, resourceType);
                }

                return resourceType;
            }
        }

        private ResourceType GetCreateComplexResourceType(
            Type clrType)
        {

            if (ReflectionHelper.HasAttribute<DataContractAttribute>(clrType) == false)
                throw new InvalidOperationException(
                    string.Format(
                    "Class must be marked with <DataContractAttribute>: '{0}'.",
                    clrType.FullName));

            ResourceType resourceType;

            if (resourceTypes.TryGetValue(clrType.FullName, out resourceType) == false)
            {

                ResourceType baseType = null;

                if (clrType.BaseType != null &&
                    ReflectionHelper.HasAttribute<DataContractAttribute>(clrType.BaseType))
                {
                    baseType = GetCreateComplexResourceType(clrType.BaseType);
                }

                resourceType = new ResourceType(
                    clrType,
                    ResourceTypeKind.ComplexType,
                    baseType,
                    clrType.Namespace,
                    clrType.Name,
                    clrType.IsAbstract);

                resourceTypes.Add(clrType.FullName, resourceType);

                var props = ReflectionHelper.GetDeclaredProperties<DataMemberAttribute>(clrType);

                foreach (var p in props)
                {
                    if (IsSupportedPrimitiveResourceType(p.PropertyType))
                    {
                        ResourceProperty rp = new ResourceProperty(
                            p.Name,
                            ResourcePropertyKind.Primitive,
                            GetPrimitiveResourceType(p));

                        resourceType.AddProperty(rp);
                    }
                    else if (ReflectionHelper.IsGenericEnumerableType(p.PropertyType))
                    {

                        throw new NotSupportedException("Collection properties on complex types not supported ((c) by Microsoft Odata lib).");
                    }
                }
            }

            return resourceType;
        }

        private static ResourceType GetPrimitiveResourceType(
            PrimitiveType primitiveType)
        {
            if (IsSupportedPrimitiveResourceType(primitiveType.ClrEquivalentType) == false)
                throw new NotSupportedException(
                    string.Format("Unsupported edm primitive type '{0}'.",
                    primitiveType.FullName));

            return ResourceType.GetPrimitiveResourceType(primitiveType.ClrEquivalentType);
        }

        private static ResourceType GetParameterResourceType(
            ParameterInfo pInfo)
        {
            if (IsSupportedPrimitiveResourceType(pInfo.ParameterType) == false)
                throw new NotSupportedException(
                    string.Format("Unsupported primitive type '{0}' in operation parameter '{1}'.",
                    pInfo.ParameterType.FullName,
                    pInfo.Name));

            return ResourceType.GetPrimitiveResourceType(pInfo.ParameterType);
        }

        private static ResourceType GetPrimitiveResourceType(
            PropertyInfo pInfo)
        {
            if (IsSupportedPrimitiveResourceType(pInfo.PropertyType) == false)
                throw new NotSupportedException(
                    string.Format("Unsupported primitive type '{0}' in property '{1}'.",
                    pInfo.PropertyType.FullName,
                    pInfo.Name));

            return ResourceType.GetPrimitiveResourceType(pInfo.PropertyType);
        }

        private static bool IsSupportedPrimitiveResourceType(
            Type clrType)
        {
            return ResourceType.GetPrimitiveResourceType(clrType) != null;
        }

        private static Type GetClrType(
            MetadataWorkspace workspace,
            StructuralType edmType)
        {
            StructuralType oSpaceType = workspace
                .GetItem<StructuralType>(edmType.FullName, DataSpace.OSpace);

            ObjectItemCollection objectItems =
                workspace.GetItemCollection(DataSpace.OSpace) as ObjectItemCollection;

            return objectItems.GetClrType(oSpaceType);
        }

        private static void SetNullable(EdmProperty edmp, ResourceProperty resp)
        {
            if (edmp.Nullable == false)
                resp.AddCustomAnnotation("", "Nullable", "false");
            else
                resp.AddCustomAnnotation("", "Nullable", "true");
        }

        /// <summary>
        /// Check if specified property represents a db foreign key column.
        /// </summary>        
        private static bool IsNavigationDependentProperty(
            EntityType entityType,
            EdmProperty edmProperty)
        {
            return entityType
                .DeclaredNavigationProperties
                .SelectMany(x => x.GetDependentProperties())
                .Any(x => x.Name == edmProperty.Name);
            
            //return edmProperty.Name.StartsWith(
            //    "FK_",
            //    StringComparison.InvariantCultureIgnoreCase);
        }

        private void AddProperties(
            MetadataWorkspace workspace,
            ComplexType sourceType,
            ResourceType targetType)
        {
            foreach (var edmp in sourceType.Properties.Where(x => x.IsPrimitiveType))
            {
                ResourceProperty resourceProperty = new ResourceProperty(
                        edmp.Name,
                        ResourcePropertyKind.Primitive,
                        GetPrimitiveResourceType(edmp.PrimitiveType));

                SetNullable(edmp, resourceProperty);

                targetType.AddProperty(resourceProperty);
            }
        }

        private void AddProperties(
            MetadataWorkspace workspace,
            EntityType sourceType,
            ResourceType targetType)
        {
            foreach (var edmp in sourceType.DeclaredProperties)
            {
                // filter out navigation dependent properties (db foreign key columns)
                if (edmp.IsPrimitiveType && IsNavigationDependentProperty(sourceType, edmp) == false)
                {
                    ResourcePropertyKind pKind = ResourcePropertyKind.Primitive;

                    bool isKey = sourceType.KeyProperties.Any(x => x.Name == edmp.Name);

                    if (isKey)
                        pKind = pKind | ResourcePropertyKind.Key;

                    if (edmp.ConcurrencyMode == ConcurrencyMode.Fixed)
                        pKind = pKind | ResourcePropertyKind.ETag;

                    ResourceProperty resourceProperty = new ResourceProperty(
                        edmp.Name,
                        pKind,
                        GetPrimitiveResourceType(edmp.PrimitiveType));

                    SetNullable(edmp, resourceProperty);

                    targetType.AddProperty(resourceProperty);
                }
                else if (edmp.IsComplexType)
                {
                    ResourceProperty resourceProperty = new ResourceProperty(
                        edmp.Name,
                        ResourcePropertyKind.ComplexType,
                        GetCreateComplexResourceType(workspace, edmp.ComplexType));

                    SetNullable(edmp, resourceProperty);

                    targetType.AddProperty(resourceProperty);
                }
            }

            foreach (var navp in sourceType.DeclaredNavigationProperties)
            {
                ResourcePropertyKind pKind = ResourcePropertyKind.ResourceReference;

                if (navp.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                    pKind = ResourcePropertyKind.ResourceSetReference;

                ResourceType toEndType = GetCreateEntityResourceType(workspace, navp.ToEndMember.GetEntityType());

                ResourceProperty resourceProperty = new ResourceProperty(
                    navp.Name,
                    pKind,
                    toEndType);

                targetType.AddProperty(resourceProperty);
            }
        }

        private ResourceAssociationSet GetCreateAssociationSet(
            MetadataWorkspace workspace,
            AssociationSet edmAssocSet)
        {
            ResourceAssociationSet resourceAssocSet;

            if (associationSets.TryGetValue(edmAssocSet.Name, out resourceAssocSet) == false)
            {
                ResourceAssociationSetEnd rse1 = CreateResourceAssociationSetEnd(
                    workspace, edmAssocSet.AssociationSetEnds[0]);
                ResourceAssociationSetEnd rse2 = CreateResourceAssociationSetEnd(
                    workspace, edmAssocSet.AssociationSetEnds[1]);

                resourceAssocSet = new ResourceAssociationSet(edmAssocSet.Name, rse1, rse2);

                associationSets.Add(edmAssocSet.Name, resourceAssocSet);

                ResourceAssociationTypeEnd rst1 = CreateResourceAssociationTypeEnd(
                    workspace, edmAssocSet.AssociationSetEnds[0]);
                ResourceAssociationTypeEnd rst2 = CreateResourceAssociationTypeEnd(
                    workspace, edmAssocSet.AssociationSetEnds[1]);

                ResourceAssociationType resourceAssocType = new ResourceAssociationType(
                    edmAssocSet.ElementType.Name,
                    edmAssocSet.ElementType.NamespaceName, rst1, rst2);

                resourceAssocSet.ResourceAssociationType = resourceAssocType;
            }

            return resourceAssocSet;
        }

        private ResourceAssociationSetEnd CreateResourceAssociationSetEnd(
            MetadataWorkspace workspace,
            AssociationSetEnd ase)
        {
            EntityType entityType = ase.CorrespondingAssociationEndMember.GetEntityType();
            ResourceType resourceType = GetCreateEntityResourceType(workspace, entityType);
            ResourceSet resourceSet = GetCreateResourceSet(workspace, ase.EntitySet);

            NavigationProperty navp = entityType
                .NavigationProperties
                .FirstOrDefault(x => x.RelationshipType == ase.ParentAssociationSet.ElementType);

            ResourceProperty resourceProp = null;

            if (navp != null)
            {
                resourceProp = resourceType
                    .Properties
                    .SingleOrDefault(x => x.Name == navp.Name);
            }

            return new ResourceAssociationSetEnd(resourceSet, resourceType, resourceProp);
        }

        private ResourceAssociationTypeEnd CreateResourceAssociationTypeEnd(
            MetadataWorkspace workspace,
            AssociationSetEnd ase)
        {
            EntityType entityType = ase.CorrespondingAssociationEndMember.GetEntityType();
            ResourceType resourceType = GetCreateEntityResourceType(workspace, entityType);

            NavigationProperty navp = entityType
                .NavigationProperties
                .FirstOrDefault(x => x.RelationshipType == ase.ParentAssociationSet.ElementType);

            ResourceProperty resourceProp = null;

            if (navp != null)
            {
                resourceProp = resourceType
                    .Properties
                    .SingleOrDefault(x => x.Name == navp.Name);
            }

            string multiplicity = "";

            switch (ase.CorrespondingAssociationEndMember.RelationshipMultiplicity)
            {
                case RelationshipMultiplicity.Many:
                    multiplicity = "*";
                    break;
                case RelationshipMultiplicity.One:
                    multiplicity = "1";
                    break;
                default:
                    multiplicity = "0..1";
                    break;
            }

            return new ResourceAssociationTypeEnd(
                ase.CorrespondingAssociationEndMember.Name,
                resourceType,
                resourceProp,
                multiplicity,
                EdmOnDeleteAction.None);
        }

        private static bool IsNavigationProperty(
            ResourceProperty resourceProperty)
        {
            return (resourceProperty.Kind == ResourcePropertyKind.ResourceReference ||
                resourceProperty.Kind == ResourcePropertyKind.ResourceSetReference);
        }

        private static bool HasAssociationEndOf(
            ResourceAssociationSet set,
            ResourceSet resourceSet,
            ResourceType resourceType,
            ResourceProperty resourceProperty)
        {
            return (set.End1.ResourceSet == resourceSet &&
                set.End1.ResourceType == resourceType &&
                set.End1.ResourceProperty == resourceProperty) ||
                (set.End2.ResourceSet == resourceSet &&
                set.End2.ResourceType == resourceType &&
                set.End2.ResourceProperty == resourceProperty);
        }

        //private void CreateConstraints(
        //    MetadataWorkspace workspace,
        //    AssociationSet edmAssocSet)
        //{            
        //    if (edmAssocSet.ElementType.IsForeignKey)
        //    {
        //        ResourceAssociationSet resourceAssocSet = associationSets[edmAssocSet.Name];
        //        ReferentialConstraint edmConstraint = edmAssocSet.ElementType.Constraint;

        //        AssociationEndMember edmRelEndMember = edmAssocSet
        //            .ElementType
        //            .AssociationEndMembers
        //            .Where(x => x.RelationshipMultiplicity != RelationshipMultiplicity.Many).First();

        //        ResourceAssociationTypeEnd assocTypeEnd = resourceAssocSet.ResourceAssociationType.GetEnd(edmRelEndMember.Name);

        //        EdmProperty edmProperty = edmConstraint.FromProperties.First();
        //        EntityType edmType = edmConstraint.FromRole.GetEntityType();

        //        ResourceType resourceType = resourceTypes[edmType.FullName];
        //        IEnumerable<ResourceProperty> resourceProperty = resourceType.Properties.Where(x => x.Name == edmProperty.Name);
        //        resourceAssocSet.ResourceAssociationType.ReferentialConstraint = new ResourceReferentialConstraint(assocTypeEnd, resourceProperty);
        //    }
        //}

        private void CreateServiceOperation(
            MethodInfo methodInfo,
            string httpMethod)
        {

            ServiceOperationResultKind resultKind;
            Type resultClrType = null;
            ResourceSet resourceSet = null;

            if (methodInfo.ReturnType == null)
            {
                resultKind = ServiceOperationResultKind.Void;
            }
            else if (ReflectionHelper.IsGenericQueryableType(methodInfo.ReturnType))
            {
                bool isSingleResult = ReflectionHelper.HasAttribute<SingleResultAttribute>(methodInfo);

                if (isSingleResult)
                    resultKind = ServiceOperationResultKind.QueryWithSingleResult;
                else
                    resultKind = ServiceOperationResultKind.QueryWithMultipleResults;

                resultClrType = methodInfo.ReturnType.GenericTypeArguments[0];
            }
            else if (ReflectionHelper.IsGenericEnumerableType(methodInfo.ReturnType))
            {
                resultKind = ServiceOperationResultKind.Enumeration;
                resultClrType = methodInfo.ReturnType.GenericTypeArguments[0];
            }
            else
            {
                resultKind = ServiceOperationResultKind.DirectValue;
                resultClrType = methodInfo.ReturnType;
            }

            ResourceType resultType = null;

            if (resultClrType != null)
            {
                if (IsSupportedPrimitiveResourceType(resultClrType))
                {
                    resultType = ResourceType.GetPrimitiveResourceType(resultClrType);
                }
                else
                {
                    if (resourceTypes.TryGetValue(resultClrType.FullName, out resultType) == false)
                        resultType = GetCreateComplexResourceType(resultClrType);
                }

                if (resultType.ResourceTypeKind == ResourceTypeKind.EntityType)
                    resourceSet = GetResourceSet(resultType);
            }

            IEnumerable<ServiceOperationParameter> pars = methodInfo
                .GetParameters()
                .Select(x => new ServiceOperationParameter(x.Name, GetParameterResourceType(x)))
                .ToArray();

            ServiceOperation op = new ServiceOperation(
                methodInfo.Name,
                resultKind,
                resultType,
                resourceSet,
                httpMethod,
                pars);

            serviceOperations.Add(op.Name, op);
        }
    }
}