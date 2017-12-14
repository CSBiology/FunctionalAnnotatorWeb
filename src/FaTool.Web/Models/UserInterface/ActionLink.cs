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