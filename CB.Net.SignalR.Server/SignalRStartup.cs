using Microsoft.Owin.Cors;
using Owin;


namespace CB.Net.SignalR.Server
{
    public class SignalRStartup
    {
        #region Methods
        public virtual void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
        #endregion
    }
}