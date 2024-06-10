using Evarosa.Models;

namespace Evarosa.Services
{
    public interface IAppService
    {
        public ConfigSite Config { get; set; }
        public void ReloadConfig();
    }
}
