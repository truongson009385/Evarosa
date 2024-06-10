using Evarosa.Models;

namespace Evarosa.Services.Impl
{
    public class AppService : IAppService
    {
        private readonly IServiceProvider _serviceProvider;
        public ConfigSite Config { get; set; }

        public AppService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            ReloadConfig();
        }

        public void ReloadConfig()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var Db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Config = Db.ConfigSite.FirstOrDefault() ?? new ConfigSite();
            }
        }
    }
}
