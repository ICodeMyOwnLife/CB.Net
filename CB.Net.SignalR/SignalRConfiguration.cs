using CB.Configuration.Common;


namespace CB.Net.SignalR
{
    public class SignalRConfiguration: ConfigurationBase<SignalRConfigSection>
    {
        #region  Constructors & Destructor
        public SignalRConfiguration(): this("signalRConfig") { }

        public SignalRConfiguration(string configSectionName): base(configSectionName) { }
        #endregion
    }
}