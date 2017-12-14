#region license
// The MIT License (MIT)

// UserViewModelBase.cs

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
using FaTool.Web.Membership.Identity;

namespace FaTool.Web.Models.Account
{
    public abstract class UserPropertyViewModelBase<TProperties>
    {

        public UserPropertyViewModelBase(TProperties properties)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            Properties = properties;
        }

        public abstract string Caption { get; }

        public TProperties Properties { get; private set; }

    }    

    public abstract class UserViewModelBase<TProperties>
        : UserPropertyViewModelBase<TProperties>
    {

        public UserViewModelBase(FaToolUser user, TProperties properties)
            : base(properties)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            User = user;
        }

        public FaToolUser User { get; private set; }

        public string Id { get { return User.Id; } }
    }
}