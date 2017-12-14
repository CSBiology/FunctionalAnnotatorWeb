using System.Configuration;

namespace FaTool.Web.Membership.ConfigFile
{
    public sealed class FaToolMembershipSection : ConfigurationSection
    {

        private static bool readOnly = false;
        private readonly static ConfigurationPropertyCollection properties =
            new ConfigurationPropertyCollection();

        public FaToolMembershipSection()
        {
            properties.Add(
                new ConfigurationProperty(
                    "loginPath",
                    typeof(string), null,
                    ConfigurationPropertyOptions.None));
        }

        public string LoginPath
        {
            get
            {
                return (string)this["loginPath"];
            }
            set
            {
                ThrowIfReadOnly("LoginPath");
                this["loginPath"] = value;
            }
        }

        protected override object GetRuntimeObject()
        {
            readOnly = true;
            return base.GetRuntimeObject();
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }

        private new bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        private void ThrowIfReadOnly(string propertyName)
        {
            if (IsReadOnly)
                throw new ConfigurationErrorsException(
                    "The property " + propertyName + " is read only.");
        }

    }
}
