using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(dotNetMVCLeagueApp.Areas.Identity.IdentityHostingStartup))]

namespace dotNetMVCLeagueApp.Areas.Identity {
    public class IdentityHostingStartup : IHostingStartup {
        public void Configure(IWebHostBuilder builder) {
            builder.ConfigureServices((context, services) => { });
        }
    }
}