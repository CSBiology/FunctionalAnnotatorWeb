using System;
using System.Collections.Generic;
using FaTool.Web.Models.UserInterface;

namespace FaTool.Web.Models.Home
{
    public sealed class HomeViewModel
    {

        private readonly FunctionSearchViewModel functionSearch;
        private readonly IEnumerable<ActionLink> organismSearchLinks;

        public HomeViewModel(
            FunctionSearchViewModel functionSearch,
            IEnumerable<ActionLink> organismSearchLinks) 
        {
            if (functionSearch == null)
                throw new ArgumentNullException("functionSearch");
            if (organismSearchLinks == null)
                throw new ArgumentNullException("organismSearchLinks");

            this.functionSearch = functionSearch;
            this.organismSearchLinks = organismSearchLinks;
        }

        public FunctionSearchViewModel FunctionSearch { get { return functionSearch; } }

        public IEnumerable<ActionLink> OrganismSearchLinks { get { return organismSearchLinks; } }
    }
}