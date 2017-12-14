#region license
// The MIT License (MIT)

// FaToolDbDataService.cs

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
