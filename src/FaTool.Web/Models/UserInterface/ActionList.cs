using System;
using System.Collections.Generic;

namespace FaTool.Web.Models.UserInterface
{
    public sealed class ActionList : IEnumerable<ActionLink>
    {

        private readonly string caption;
        private readonly IList<ActionLink> links = new List<ActionLink>();

        public ActionList(string caption)
        {
            if (caption == null)
                throw new ArgumentNullException("caption");
            this.caption = caption;
        }

        public string Caption { get { return caption; } }

        public void Add(ActionLink link)
        {
            links.Add(link);
        }

        public int Count { get { return links.Count; } }

        #region IEnumerable<ActionLink> Members

        public IEnumerator<ActionLink> GetEnumerator()
        {
            return links.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return links.GetEnumerator();
        }

        #endregion
    }
}