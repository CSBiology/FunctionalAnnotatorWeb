#region license
// The MIT License (MIT)

// RegisterViewModel.cs

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
using System.Web.Mvc;
using FaTool.Web.Membership.Identity;
using System.Linq;

namespace FaTool.Web.Models.Account
{
    public sealed class RegisterViewModel : UserPropertyViewModelBase<FaToolRegisterProperties>
    {

        public RegisterViewModel(
            FaToolRegisterProperties properties,
            IEnumerable<SelectListItem> countryOptions)
            : base(properties)
        {
            if (countryOptions == null)
                throw new ArgumentNullException("countryOptions");

            CountryOptions = countryOptions.ToArray();
        }

        public RegisterViewModel(IEnumerable<SelectListItem> countryOptions)
            : this(new FaToolRegisterProperties(), countryOptions) { }

        public override string Caption
        {
            get { return "Sign Up to Functional Annotator"; }
        }

        public IEnumerable<SelectListItem> CountryOptions { get; private set; }
    }
}