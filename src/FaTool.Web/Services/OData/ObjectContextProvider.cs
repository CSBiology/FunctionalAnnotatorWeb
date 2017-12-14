#region license
// The MIT License (MIT)

// ObjectContextProvider.cs

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
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Services;
using System.Data.Services.Providers;
using System.Linq;

namespace FaTool.Web.Services.OData
{
    public sealed class ObjectContextProvider<TService, TContext>
        : IDataServiceMetadataProvider,
        IDataServiceQueryProvider,
        IDataServiceUpdateProvider
        where TContext : IObjectContextAdapter
        where TService : DataService<TContext>, IServiceProvider
    {

        private readonly TService service;
        private readonly ResourceMetadataCache<TService, TContext> metadataCache =
            new ResourceMetadataCache<TService, TContext>();

        public ObjectContextProvider(TService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            this.service = service;
        }

        public TService Service { get { return service; } }

        public TContext CurrentDataSource { get; private set; }

        public ResourceMetadataCache<TService, TContext> MetadataCache { get { return metadataCache; } }

        #region IDataServiceMetadataProvider Members

        string IDataServiceMetadataProvider.ContainerName
        {
            get { return metadataCache.GetContainerName(CurrentDataSource); }
        }

        string IDataServiceMetadataProvider.ContainerNamespace
        {
            get { return metadataCache.GetContainerNamespace(); }
        }

        IEnumerable<ResourceType> IDataServiceMetadataProvider.GetDerivedTypes(ResourceType resourceType)
        {
            return metadataCache
                .LazyGetResourceTypes(CurrentDataSource)
                .Where(x => x.BaseType == resourceType);
        }

        ResourceAssociationSet IDataServiceMetadataProvider.GetResourceAssociationSet(
            ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            return metadataCache.LazyGetResourceAssociationSet(
                CurrentDataSource,
                resourceSet,
                resourceType,
                resourceProperty);
        }

        bool IDataServiceMetadataProvider.HasDerivedTypes(ResourceType resourceType)
        {
            return metadataCache
                .LazyGetResourceTypes(CurrentDataSource)
                .Any(x => x.BaseType == resourceType);
        }

        IEnumerable<ResourceSet> IDataServiceMetadataProvider.ResourceSets
        {
            get
            {
                return metadataCache.LazyGetResourceSets(CurrentDataSource);
            }
        }

        IEnumerable<ServiceOperation> IDataServiceMetadataProvider.ServiceOperations
        {
            get
            {
                return metadataCache.LazyGetServiceOperations(CurrentDataSource);
            }
        }

        IEnumerable<ResourceType> IDataServiceMetadataProvider.Types
        {
            get
            {
                return metadataCache.LazyGetResourceTypes(CurrentDataSource);
            }
        }

        bool IDataServiceMetadataProvider.TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            return metadataCache.LazyTryGetResourceSet(CurrentDataSource, name, out resourceSet);
        }

        bool IDataServiceMetadataProvider.TryResolveResourceType(string name, out ResourceType resourceType)
        {
            return metadataCache.LazyTryGetResourceType(CurrentDataSource, name, out resourceType);
        }

        bool IDataServiceMetadataProvider.TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            return metadataCache.LazyTryGetServiceOperation(CurrentDataSource, name, out serviceOperation);
        }

        #endregion

        #region IDataServiceQueryProvider Members

        object IDataServiceQueryProvider.CurrentDataSource
        {
            get
            {
                return CurrentDataSource;
            }
            set
            {
                CurrentDataSource = (TContext)value;
            }
        }

        object IDataServiceQueryProvider.GetOpenPropertyValue(object target, string propertyName)
        {
            throw new NotSupportedException();
        }

        IEnumerable<KeyValuePair<string, object>> IDataServiceQueryProvider.GetOpenPropertyValues(object target)
        {
            throw new NotSupportedException();
        }

        object IDataServiceQueryProvider.GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            throw new NotSupportedException();
        }

        IQueryable IDataServiceQueryProvider.GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            return oc.GetQuery(resourceSet.Name);
        }

        ResourceType IDataServiceQueryProvider.GetResourceType(object target)
        {
            return metadataCache.LazyGetResourceType(CurrentDataSource, target.GetType());
        }

        object IDataServiceQueryProvider.InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            return ReflectionHelper.Invoke(
                service,
                serviceOperation.Name,
                null,
                parameters);
        }

        bool IDataServiceQueryProvider.IsNullPropagationRequired
        {
            get { return true; }
        }

        #endregion

        #region IDataServiceUpdateProvider Members

        void IDataServiceUpdateProvider.SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            if (checkForEquality == null)
                throw new DataServiceException(500, "Cannot perform operation without ETag.");
            if (!checkForEquality.Value)
                throw new DataServiceException(500, "IfNoneMatch header not supported in update and delete.");

            ObjectStateEntry objectStateEntry = CurrentDataSource.ObjectContext.ObjectStateManager.GetObjectStateEntry(resourceCookie);
            OriginalValueRecord originalValues = objectStateEntry.GetUpdatableOriginalValues();

            foreach (KeyValuePair<string, object> etag in concurrencyValues)
            {
                int propertyOrdinal = originalValues.GetOrdinal(etag.Key);
                originalValues.SetValue(propertyOrdinal, etag.Value);
            }

        }

        #endregion

        #region IUpdatable Members

        void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            oc.AddReferenceToCollection(targetResource, propertyName, resourceToBeAdded);
        }

        void IUpdatable.ClearChanges()
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            oc.ClearChanges();
        }

        object IUpdatable.CreateResource(string containerName, string fullTypeName)
        {
            ResourceType resourceType;
            if (metadataCache.LazyTryGetResourceType(CurrentDataSource, fullTypeName, out resourceType) &&
                resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                object newResource = CurrentDataSource.ObjectContext.CreateObject(resourceType.InstanceType);
                CurrentDataSource.ObjectContext.AddObject(containerName, newResource);
                return newResource;
            }
            else
            {
                throw new DataServiceException(
                    400,
                    string.Format(
                    "Can't create resource type '{0}'.",
                    fullTypeName));
            }
        }

        void IUpdatable.DeleteResource(object targetResource)
        {
            CurrentDataSource.ObjectContext.DeleteObject(targetResource);
        }

        object IUpdatable.GetResource(IQueryable query, string fullTypeName)
        {
            foreach (var o in query)
                return o;
            return null;
        }

        object IUpdatable.GetValue(object targetResource, string propertyName)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            return oc.GetValue(targetResource, propertyName);
        }

        void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            oc.RemoveReferenceFromCollection(targetResource, propertyName, resourceToBeRemoved);
        }

        object IUpdatable.ResetResource(object resource)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            oc.ResetProperties(resource);
            return resource;
        }

        object IUpdatable.ResolveResource(object resource)
        {
            return resource;
        }

        void IUpdatable.SaveChanges()
        {
            CurrentDataSource.ObjectContext.SaveChanges();
        }

        void IUpdatable.SetReference(object targetResource, string propertyName, object propertyValue)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            oc.SetReference(targetResource, propertyName, propertyValue);
        }

        void IUpdatable.SetValue(object targetResource, string propertyName, object propertyValue)
        {
            ObjectContext oc = CurrentDataSource.ObjectContext;
            oc.SetValue(targetResource, propertyName, propertyValue);
        }

        #endregion


    }
}