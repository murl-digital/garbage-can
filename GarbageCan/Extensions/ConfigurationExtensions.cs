﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GarbageCan.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void Configure<TI, TC>(this IServiceCollection services, IConfiguration configuration) where TC : class, TI
        {
            services.Configure<TC>(configuration);
            services.AddTransient(typeof(TI), provider => provider.GetRequiredService<IOptions<TC>>().Value);
        }
    }
}