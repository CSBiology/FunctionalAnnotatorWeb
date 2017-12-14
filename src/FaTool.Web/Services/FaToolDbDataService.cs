using System;
using System.Data.Services;
using System.Data.Services.Common;
using FaTool.Web.Services.OData;
using FaTool.Db;

namespace FaTool.Web.Services
{

    public sealed class FaToolDbDataService
        : DataService<FaToolDbEntities>, IServiceProvider
    {

        private readonly ObjectContextProvider<FaToolDbDataService, FaToolDbEntities> provider;

        public FaToolDbDataService()
        {
            provider = new ObjectContextProvider<FaToolDbDataService, FaToolDbEntities>(this);
        }

        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.EnableTypeAccess("*");
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
            config.MaxResultsPerCollection = 1000;
            config.MaxExpandDepth = 10;
            config.MaxExpandCount = 50;
            //config.DataServiceBehavior.AcceptProjectionRequests = true;
        }

        protected override FaToolDbEntities CreateDataSource()
        {
            return new FaToolDbEntities();
        }
        
        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType.IsAssignableFrom(provider.GetType()))
                return provider;
            else
                return null;
        }

        #endregion
    }
}
