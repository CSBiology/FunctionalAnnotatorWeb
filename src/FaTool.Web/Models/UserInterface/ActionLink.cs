#region license
// The MIT License (MIT)

// ActionLink.cs

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
using System.Web.Mvc;

namespace FaTool.Web.Models.UserInterface
{
    public class ActionLink
    {

        private readonly string linkText;
        private readonly string url;
        private readonly bool disabled;

        public ActionLink(
            string linkText,
            string url,
            bool disabled = false)
        {
            if (linkText == null)
                throw new ArgumentNullException("linkText");
            if (url == null)
                throw new ArgumentNullException("url");

            this.linkText = linkText;
            this.url = url;
            this.disabled = disabled;
        }

        public string LinkText { get { return linkText; } }
        public string Url { get { return url; } }
        public bool Disabled { get { return disabled; } }
    }

    public static class ActionLinks
    {

        public static ActionLink ActionLink(
            this UrlHelper urlHelper, 
            string linkText, 
            string actionName, 
            string controllerName,
            object routeValues, 
            bool disabled = false)
        {
            var url = urlHelper.Action(actionName, controllerName, routeValues);
            return new ActionLink(linkText, url, disabled);
        }

        public static ActionLink ActionLink(
            this UrlHelper urlHelper,
            string linkText,
            string actionName,
            object routeValues, 
            bool disabled = false)
        {
            var url = urlHelper.Action(actionName, routeValues);
            return new ActionLink(linkText, url, disabled);
        }
        
    }
}