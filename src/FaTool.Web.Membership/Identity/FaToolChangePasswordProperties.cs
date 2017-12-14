#region license
// The MIT License (MIT)

// FaToolChangePasswordProperties.cs

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

using System.ComponentModel.DataAnnotations;

namespace FaTool.Web.Membership.Identity
{
    public sealed class FaToolChangePasswordProperties
    {
                
        [Display(Name = "Current Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        public string CurrentPassword { get; set; }
        
        [Display(Name = "New Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        public string NewPassword { get; set; }
        
        [Display(Name = "Confirm Password")]
        [CustomValidationAttribute(typeof(FaToolPasswordValidator), "ValidatePassword")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }  
    }
}