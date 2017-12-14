#region license
// The MIT License (MIT)

// EntityUrlExtensions.cs

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
using FaTool.Web.Models.UserInterface;
using System.Linq;

namespace FaTool.Web.Models.Entities
{
    public static class EntityUrlExtensions
    {

        public static ActionLink EntityGetActionLink<TKey>(
            this UrlHelper url,
            string linkText,
            string controllerName,
            TKey id)
        {
            return new ActionLink(linkText, url.EntityGetAction(controllerName, id));
        }

        public static ActionLink EntityGetActionLink<TKey>(
            this UrlHelper url,
            string linkText,
            TKey id)
        {
            return new ActionLink(linkText, url.EntityGetAction(id));
        }

        public static ActionLink EntityGetActionLink<TKey, TNavId>(
            this UrlHelper url,
            string linkText,
            TKey id,
            TNavId navId)
            where TNavId : struct
        {
            return new ActionLink(linkText, url.EntityGetAction(id, navId));
        }

        public static ActionLink EntityGetActionLink<TKey, TNavId>(
            this UrlHelper url,
            string linkText,
            string controllerName,
            TKey id,
            TNavId navId)
            where TNavId : struct
        {
            return new ActionLink(linkText, url.EntityGetAction(controllerName, id, navId));
        }

        public static ActionLink EntityActionLink<TKey>(
            this UrlHelper url,
            string linkText,
            string actionName,
            TKey id,
            bool disabled = false)
        {
            return new ActionLink(linkText, url.EntityAction(actionName, id), disabled);
        }

        public static ActionLink EntityActionLink<TKey>(
            this UrlHelper url,
            string linkText,
            string actionName,
            string controllerName,
            TKey id,
            bool disabled = false)
        {
            return new ActionLink(linkText, url.EntityAction(actionName, controllerName, id), disabled);
        }

        public static string EntityGetAction<TKey>(
            this UrlHelper url,
            string controllerName,
            TKey id)
        {
            return url.Action("Get", controllerName, new { id = id });
        }

        public static string EntityGetAction<TKey>(
            this UrlHelper url,
            TKey id)
        {
            return url.Action("Get", new { id = id });
        }

        public static string EntityGetAction<TKey, TNavId>(
            this UrlHelper url,
            TKey id,
            TNavId navId)
            where TNavId : struct
        {
            return url.Action("Get", new { id = id, navId = navId });
        }

        public static string EntityGetAction<TKey, TNavId>(
            this UrlHelper url,
            string controllerName,
            TKey id,
            TNavId navId)
            where TNavId : struct
        {
            return url.Action("Get", controllerName, new { id = id, navId = navId });
        }

        public static string EntityAction<TKey>(
            this UrlHelper url,
            string actionName,
            TKey id)
        {
            return url.Action(actionName, new { id = id });
        }

        public static string EntityAction<TKey>(
            this UrlHelper url,
            string actionName,
            string controllerName,
            TKey id)
        {
            return url.Action(actionName, controllerName, new { id = id });
        }

        public static IEnumerable<NavTab<TNavId>> CreateNavTabs<TKey, TNavId>(
            this UrlHelper url,
            TKey id,
            bool disabled = false)
            where TNavId : struct
        {
            if (!typeof(TNavId).IsEnum)
                throw new InvalidOperationException("Enum type expected.");

            return Enum.GetValues(typeof(TNavId))
                .OfType<TNavId>()
                .Select(x => url.CreateNavTab(id, x))
                .ToArray();
        }

        public static NavTab<TNavId> CreateNavTab<TKey, TNavId>(
            this UrlHelper url,
            TKey id,
            TNavId navId,
            bool disabled = false)
            where TNavId : struct
        {
            // TODO get link text from attribute
            var linkText = navId.ToString();
            var link = url.EntityGetActionLink(linkText, id, navId);
            return new NavTab<TNavId>(navId, link, disabled);
        }
    }
}