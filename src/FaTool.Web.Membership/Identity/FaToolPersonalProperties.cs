#region license
// The MIT License (MIT)

// FaToolPersonalProperties.cs

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
    public interface IFaToolPersonalProperties
    {
        string Email { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string Company { get; set; }
        string Country { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string State { get; set; }
        string ZipCode { get; set; }
    }

    public class FaToolPersonalProperties : IFaToolPersonalProperties
    {
        public FaToolPersonalProperties() { }

        public FaToolPersonalProperties(IFaToolPersonalProperties properties)
        {
            this.SetProperties(properties);
        }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(150)]
        [Display(Name = "Company/Institution")]
        public string Company { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        [Display(Name = "Street Adress")]
        public string Address { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        [Display(Name = "Country")]
        public string Country { get; set; }
    }

    public static class PersonalPropertiesExtensions
    {
        public static void SetProperties(this IFaToolPersonalProperties target, IFaToolPersonalProperties source)
        {
            target.Email = source.Email;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.Company = source.Company;
            target.Address = source.Address;
            target.City = source.City;
            target.ZipCode = source.ZipCode;
            target.State = source.State;
            target.Country = source.Country;
        }
    }
}
