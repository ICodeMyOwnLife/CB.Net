using System.Configuration;


namespace CB.Net.SignalR
{
    public class SignalRConfigSection: ConfigurationSection
    {
        #region Fields
        protected const string HUB_NAME = "hubName";
        protected const string URL = "url";
        #endregion


        #region  Properties & Indexers
        [ConfigurationProperty(HUB_NAME, IsRequired = false)]
        public virtual string HubName
        {
            get { return (string)this[HUB_NAME]; }
            set { this[HUB_NAME] = value; }
        }

        [ConfigurationProperty(URL, DefaultValue = SignalRDefaults.URL, IsRequired = false)]
        public virtual string Url
        {
            get { return (string)this[URL]; }
            set { this[URL] = value; }
        }
        #endregion
    }
}