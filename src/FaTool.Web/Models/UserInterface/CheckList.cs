using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FaTool.Web.Models.UserInterface
{

    public interface ICheckList
    {
        string PropertyName { get; }
        IEnumerable<SelectListItem> Items { get; }
    }

    public sealed class CheckList : ICheckList
    {

        public CheckList(string propertyName, IEnumerable<SelectListItem> items)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException("propertyName");
            if (items == null)
                throw new ArgumentNullException("items");

            this.PropertyName = propertyName;
            this.Items = items.ToArray();
        }

        public string PropertyName { get; private set; }

        public IEnumerable<SelectListItem> Items { get; private set; }
    }
}