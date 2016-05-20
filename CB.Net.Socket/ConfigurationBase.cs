using System.Configuration;


namespace CB.Net.Socket
{
    public abstract class ConfigurationBase<TConfigurationSection>
        where TConfigurationSection: ConfigurationSection, new()
    {
        #region  Constructors & Destructor
        protected ConfigurationBase(string configSectionName)
        {
            ConfigurationSection = (TConfigurationSection)ConfigurationManager.GetSection(configSectionName) ??
                                   new TConfigurationSection();
        }
        #endregion


        #region  Properties & Indexers
        public TConfigurationSection ConfigurationSection { get; set; }
        #endregion
    }
}