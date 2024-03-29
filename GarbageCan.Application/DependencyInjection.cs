﻿using FluentValidation;
using GarbageCan.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using GarbageCan.Application.Common.Interfaces;
using GarbageCan.Application.XP.Services;

namespace GarbageCan.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, Assembly mediatorAssembly = null)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            if (mediatorAssembly != null)
            {
                services.AddMediatR(mediatorAssembly);
            }

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));

            services.AddTransient<IXpCalculatorService, XpCalculatorService>();

            return services;
        }
    }
}