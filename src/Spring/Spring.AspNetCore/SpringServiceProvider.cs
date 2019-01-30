using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;

namespace Spring.AspNetCore
{
    public class SpringServiceProvider : IServiceProvider
    {
        private readonly IApplicationContext _context;
        private readonly IServiceProvider _aspnetProvider;
        private readonly MethodInfo _contextGetMethod;

        public SpringServiceProvider(IServiceCollection aspnetProvider, IApplicationContext context)
        {
            _context = context;
            _aspnetProvider = aspnetProvider.BuildServiceProvider(true);
            _contextGetMethod =_context.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(m => m.IsGenericMethod && !m.GetParameters().Any() && m.Name == nameof(IApplicationContext.GetObject));
        }

        public object GetService(Type serviceType)
        {
            return _aspnetProvider.GetService(serviceType) ??
                   _contextGetMethod.MakeGenericMethod(serviceType).Invoke(_context, new object[0]);
        }
    }
}