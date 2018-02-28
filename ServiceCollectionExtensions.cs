using Microsoft.Extensions.DependencyInjection;

namespace RoboUI
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRoboUI(this IServiceCollection services)
        {
            services.AddSingleton<IStaticCacheManager, StaticCacheManager>();
            services.AddScoped<IScriptRegister, ScriptRegister>();
        }
    }
}
