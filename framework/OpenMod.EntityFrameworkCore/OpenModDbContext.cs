using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Configurator;
using System;
using System.Reflection;

namespace OpenMod.EntityFrameworkCore
{
    public abstract class OpenModDbContext<TSelf> : DbContext where TSelf : OpenModDbContext<TSelf>
    {
        internal readonly IServiceProvider ServiceProvider;
        private readonly IDbContextConfigurator? m_DbContextConfigurator;
        private readonly IConfiguration? m_OpenModConfiguration;

        protected OpenModDbContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            m_OpenModConfiguration = ServiceProvider.GetService<IOpenModHost>()?.LifetimeScope.Resolve<IConfiguration>();
        }

        protected OpenModDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider)
        {
            m_DbContextConfigurator = configurator;
            ServiceProvider = serviceProvider;
            m_OpenModConfiguration = ServiceProvider.GetService<IOpenModHost>()?.LifetimeScope.Resolve<IConfiguration>();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            m_DbContextConfigurator?.Configure(this, optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            m_DbContextConfigurator?.Configure(this, modelBuilder);
        }

        /// <summary>
        /// Gets the name of the migrations table for supporting providers.
        /// </summary>
        protected internal virtual string MigrationsTableName
        {
            get
            {
                var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>().Id;
                return "__" + componentId.Replace(".", "_") + "_MigrationsHistory".ToLower();
            }
        }

        /// <summary>
        /// Gets the prefix for the tables for supporting providers.
        /// </summary>
        protected internal virtual string? TablePrefix
        {
            get
            {
                var componentId = GetType().Assembly.GetCustomAttribute<PluginMetadataAttribute>()?.Id ??
                                  throw new InvalidOperationException("Could not find plugin metadata");

                return componentId.Replace(".", "_") + "_";
            }
        }

        /// <summary>
        /// Gets the name of the connection string used by supporting providers.
        /// </summary>
        protected internal virtual string GetConnectionStringName()
        {
            var defaultConnectionStringName = m_OpenModConfiguration?.GetValue("db:defaultConnectionStringName", ConnectionStrings.Default) ?? ConnectionStrings.Default;
            return typeof(TSelf).GetCustomAttribute<ConnectionStringAttribute>()?.Name ?? defaultConnectionStringName;
        }
    }
}