using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FaTool.Web.Membership.Identity
{

    /// <summary>
    /// ISO-3166-1 Alpha-2 Codes country list.
    /// </summary>
    public static class Countries
    {

        private static CountryCollection values = null;

        private static void InitValues()
        {
            if (values == null)
            {

                var countries = new Country[] { 

                    new Country("AF", "Afghanistan"),
                    new Country("AX", "Åland Islands"),
                    new Country("AL", "Albania"),
                    new Country("DZ", "Algeria"),
                    new Country("AS", "American Samoa"),
                    new Country("AD", "Andorra"),
                    new Country("AO", "Angola"),
                    new Country("AI", "Anguilla"),
                    new Country("AQ", "Antarctica"),
                    new Country("AG", "Antigua and Barbuda"),
                    new Country("AR", "Argentina"),
                    new Country("AM", "Armenia"),
                    new Country("AW", "Aruba"),
                    new Country("AU", "Australia"),
                    new Country("AT", "Austria"),
                    new Country("AZ", "Azerbaijan"),
                    new Country("BS", "Bahamas"),
                    new Country("BH", "Bahrain"),
                    new Country("BD", "Bangladesh"),
                    new Country("BB", "Barbados"),
                    new Country("BY", "Belarus"),
                    new Country("BE", "Belgium"),
                    new Country("BZ", "Belize"),
                    new Country("BJ", "Benin"),
                    new Country("BM", "Bermuda"),
                    new Country("BT", "Bhutan"),
                    new Country("BO", "Bolivia, Plurinational State of"),
                    new Country("BQ", "Bonaire, Sint Eustatius and Saba"),
                    new Country("BA", "Bosnia and Herzegovina"),
                    new Country("BW", "Botswana"),
                    new Country("BV", "Bouvet Island"),
                    new Country("BR", "Brazil"),
                    new Country("IO", "British Indian Ocean Territory"),
                    new Country("BN", "Brunei Darussalam"),
                    new Country("BG", "Bulgaria"),
                    new Country("BF", "Burkina Faso"),
                    new Country("BI", "Burundi"),
                    new Country("KH", "Cambodia"),
                    new Country("CM", "Cameroon"),
                    new Country("CA", "Canada"),
                    new Country("CV", "Cape Verde"),
                    new Country("KY", "Cayman Islands"),
                    new Country("CF", "Central African Republic"),
                    new Country("TD", "Chad"),
                    new Country("CL", "Chile"),
                    new Country("CN", "China"),
                    new Country("CX", "Christmas Island"),
                    new Country("CC", "Cocos (Keeling) Islands"),
                    new Country("CO", "Colombia"),
                    new Country("KM", "Comoros"),
                    new Country("CG", "Congo"),
                    new Country("CD", "Congo, the Democratic Republic of the"),
                    new Country("CK", "Cook Islands"),
                    new Country("CR", "Costa Rica"),
                    new Country("CI", "Côte d'Ivoire"),
                    new Country("HR", "Croatia"),
                    new Country("CU", "Cuba"),
                    new Country("CW", "Curaçao"),
                    new Country("CY", "Cyprus"),
                    new Country("CZ", "Czech Republic"),
                    new Country("DK", "Denmark"),
                    new Country("DJ", "Djibouti"),
                    new Country("DM", "Dominica"),
                    new Country("DO", "Dominican Republic"),
                    new Country("EC", "Ecuador"),
                    new Country("EG", "Egypt"),
                    new Country("SV", "El Salvador"),
                    new Country("GQ", "Equatorial Guinea"),
                    new Country("ER", "Eritrea"),
                    new Country("EE", "Estonia"),
                    new Country("ET", "Ethiopia"),
                    new Country("FK", "Falkland Islands (Malvinas)"),
                    new Country("FO", "Faroe Islands"),
                    new Country("FJ", "Fiji"),
                    new Country("FI", "Finland"),
                    new Country("FR", "France"),
                    new Country("GF", "French Guiana"),
                    new Country("PF", "French Polynesia"),
                    new Country("TF", "French Southern Territories"),
                    new Country("GA", "Gabon"),
                    new Country("GM", "Gambia"),
                    new Country("GE", "Georgia"),
                    new Country("DE", "Germany"),
                    new Country("GH", "Ghana"),
                    new Country("GI", "Gibraltar"),
                    new Country("GR", "Greece"),
                    new Country("GL", "Greenland"),
                    new Country("GD", "Grenada"),
                    new Country("GP", "Guadeloupe"),
                    new Country("GU", "Guam"),
                    new Country("GT", "Guatemala"),
                    new Country("GG", "Guernsey"),
                    new Country("GN", "Guinea"),
                    new Country("GW", "Guinea-Bissau"),
                    new Country("GY", "Guyana"),
                    new Country("HT", "Haiti"),
                    new Country("HM", "Heard Island and McDonald Islands"),
                    new Country("VA", "Holy See (Vatican City State)"),
                    new Country("HN", "Honduras"),
                    new Country("HK", "Hong Kong"),
                    new Country("HU", "Hungary"),
                    new Country("IS", "Iceland"),
                    new Country("IN", "India"),
                    new Country("ID", "Indonesia"),
                    new Country("IR", "Iran, Islamic Republic of"),
                    new Country("IQ", "Iraq"),
                    new Country("IE", "Ireland"),
                    new Country("IM", "Isle of Man"),
                    new Country("IL", "Israel"),
                    new Country("IT", "Italy"),
                    new Country("JM", "Jamaica"),
                    new Country("JP", "Japan"),
                    new Country("JE", "Jersey"),
                    new Country("JO", "Jordan"),
                    new Country("KZ", "Kazakhstan"),
                    new Country("KE", "Kenya"),
                    new Country("KI", "Kiribati"),
                    new Country("KP", "Korea, Democratic People's Republic of"),
                    new Country("KR", "Korea, Republic of"),
                    new Country("KW", "Kuwait"),
                    new Country("KG", "Kyrgyzstan"),
                    new Country("LA", "Lao People's Democratic Republic"),
                    new Country("LV", "Latvia"),
                    new Country("LB", "Lebanon"),
                    new Country("LS", "Lesotho"),
                    new Country("LR", "Liberia"),
                    new Country("LY", "Libya"),
                    new Country("LI", "Liechtenstein"),
                    new Country("LT", "Lithuania"),
                    new Country("LU", "Luxembourg"),
                    new Country("MO", "Macao"),
                    new Country("MK", "Macedonia, the former Yugoslav Republic of"),
                    new Country("MG", "Madagascar"),
                    new Country("MW", "Malawi"),
                    new Country("MY", "Malaysia"),
                    new Country("MV", "Maldives"),
                    new Country("ML", "Mali"),
                    new Country("MT", "Malta"),
                    new Country("MH", "Marshall Islands"),
                    new Country("MQ", "Martinique"),
                    new Country("MR", "Mauritania"),
                    new Country("MU", "Mauritius"),
                    new Country("YT", "Mayotte"),
                    new Country("MX", "Mexico"),
                    new Country("FM", "Micronesia, Federated States of"),
                    new Country("MD", "Moldova, Republic of"),
                    new Country("MC", "Monaco"),
                    new Country("MN", "Mongolia"),
                    new Country("ME", "Montenegro"),
                    new Country("MS", "Montserrat"),
                    new Country("MA", "Morocco"),
                    new Country("MZ", "Mozambique"),
                    new Country("MM", "Myanmar"),
                    new Country("NA", "Namibia"),
                    new Country("NR", "Nauru"),
                    new Country("NP", "Nepal"),
                    new Country("NL", "Netherlands"),
                    new Country("NC", "New Caledonia"),
                    new Country("NZ", "New Zealand"),
                    new Country("NI", "Nicaragua"),
                    new Country("NE", "Niger"),
                    new Country("NG", "Nigeria"),
                    new Country("NU", "Niue"),
                    new Country("NF", "Norfolk Island"),
                    new Country("MP", "Northern Mariana Islands"),
                    new Country("NO", "Norway"),
                    new Country("OM", "Oman"),
                    new Country("PK", "Pakistan"),
                    new Country("PW", "Palau"),
                    new Country("PS", "Palestinian Territory, Occupied"),
                    new Country("PA", "Panama"),
                    new Country("PG", "Papua New Guinea"),
                    new Country("PY", "Paraguay"),
                    new Country("PE", "Peru"),
                    new Country("PH", "Philippines"),
                    new Country("PN", "Pitcairn"),
                    new Country("PL", "Poland"),
                    new Country("PT", "Portugal"),
                    new Country("PR", "Puerto Rico"),
                    new Country("QA", "Qatar"),
                    new Country("RE", "Réunion"),
                    new Country("RO", "Romania"),
                    new Country("RU", "Russian Federation"),
                    new Country("RW", "Rwanda"),
                    new Country("BL", "Saint Barthélemy"),
                    new Country("SH", "Saint Helena, Ascension and Tristan da Cunha"),
                    new Country("KN", "Saint Kitts and Nevis"),
                    new Country("LC", "Saint Lucia"),
                    new Country("MF", "Saint Martin (French part)"),
                    new Country("PM", "Saint Pierre and Miquelon"),
                    new Country("VC", "Saint Vincent and the Grenadines"),
                    new Country("WS", "Samoa"),
                    new Country("SM", "San Marino"),
                    new Country("ST", "Sao Tome and Principe"),
                    new Country("SA", "Saudi Arabia"),
                    new Country("SN", "Senegal"),
                    new Country("RS", "Serbia"),
                    new Country("SC", "Seychelles"),
                    new Country("SL", "Sierra Leone"),
                    new Country("SG", "Singapore"),
                    new Country("SX", "Sint Maarten (Dutch part)"),
                    new Country("SK", "Slovakia"),
                    new Country("SI", "Slovenia"),
                    new Country("SB", "Solomon Islands"),
                    new Country("SO", "Somalia"),
                    new Country("ZA", "South Africa"),
                    new Country("GS", "South Georgia and the South Sandwich Islands"),
                    new Country("SS", "South Sudan"),
                    new Country("ES", "Spain"),
                    new Country("LK", "Sri Lanka"),
                    new Country("SD", "Sudan"),
                    new Country("SR", "Suriname"),
                    new Country("SJ", "Svalbard and Jan Mayen"),
                    new Country("SZ", "Swaziland"),
                    new Country("SE", "Sweden"),
                    new Country("CH", "Switzerland"),
                    new Country("SY", "Syrian Arab Republic"),
                    new Country("TW", "Taiwan, Province of China"),
                    new Country("TJ", "Tajikistan"),
                    new Country("TZ", "Tanzania, United Republic of"),
                    new Country("TH", "Thailand"),
                    new Country("TL", "Timor-Leste"),
                    new Country("TG", "Togo"),
                    new Country("TK", "Tokelau"),
                    new Country("TO", "Tonga"),
                    new Country("TT", "Trinidad and Tobago"),
                    new Country("TN", "Tunisia"),
                    new Country("TR", "Turkey"),
                    new Country("TM", "Turkmenistan"),
                    new Country("TC", "Turks and Caicos Islands"),
                    new Country("TV", "Tuvalu"),
                    new Country("UG", "Uganda"),
                    new Country("UA", "Ukraine"),
                    new Country("AE", "United Arab Emirates"),
                    new Country("GB", "United Kingdom"),
                    new Country("US", "United States"),
                    new Country("UM", "United States Minor Outlying Islands"),
                    new Country("UY", "Uruguay"),
                    new Country("UZ", "Uzbekistan"),
                    new Country("VU", "Vanuatu"),
                    new Country("VE", "Venezuela, Bolivarian Republic of"),
                    new Country("VN", "Viet Nam"),
                    new Country("VG", "Virgin Islands, British"),
                    new Country("VI", "Virgin Islands, U.S."),
                    new Country("WF", "Wallis and Futuna"),
                    new Country("EH", "Western Sahara"),
                    new Country("YE", "Yemen"),
                    new Country("ZM", "Zambia"),
                    new Country("ZW", "Zimbabwe")
                };

                values = new CountryCollection(countries);
            }
        }

        public static Country Default { get { return Resolve("DE"); } }

        public static IEnumerable<Country> Values
        {
            get
            {
                InitValues();
                return values;
            }
        }

        public static Country Resolve(string code)
        {
            InitValues();

            if (values.Contains(code))
                return values[code];
            else
                throw new KeyNotFoundException(code);
        }

        public static string ResolveName(string code)
        {
            return Resolve(code).Name;
        }

        public static bool CanResolve(string code)
        {
            InitValues();
            return values.Contains(code);
        }

        private class CountryCollection : KeyedCollection<string, Country>
        {

            public CountryCollection(IEnumerable<Country> values)
                : base(StringComparer.InvariantCultureIgnoreCase)
            {
                foreach (var item in values)
                    Add(item);
            }

            protected override string GetKeyForItem(Country item)
            {
                return item.Code;
            }
        }

    }

    public sealed class Country
    {
        private readonly string code = "AF";
        private readonly string name = "Afghanistan";

        internal Country(string code, string name)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("code");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            this.code = code;
            this.name = name;
        }

        public string Code { get { return code; } }
        public string Name { get { return name; } }
    }
}