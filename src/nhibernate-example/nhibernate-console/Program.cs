using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using nhibernate_console.Mapping;

namespace nhibernate_console {
    class Program {
        public static async Task Main( string[] args ) {

            var builder = new HostBuilder()
                .ConfigureAppConfiguration( ( hostingContext, config ) => {
                    config.AddJsonFile( "appsettings.json", optional: false );
                    config.AddEnvironmentVariables();
                    if ( args != null ) {
                        config.AddCommandLine( args );
                    }
                } )
                .ConfigureServices( ( hostContext, services ) => {
                    services.AddOptions();
                    services.AddSingleton<IHostedService, NhibernateService>();

                    var config = new Configuration();
                    config.SetProperty( NHibernate.Cfg.Environment.ShowSql, "true" );
                    config.DataBaseIntegration( db => {
                        db.ConnectionString = hostContext.Configuration[ "ConnectionStrings:DefaultConnection" ];
                        db.Dialect<MsSql2012Dialect>();
                        db.Driver<SqlClientDriver>();
                    } );

                    var mapping = new ModelMapper();
                    mapping.AddMapping<CustomerConfiguration>();
                    mapping.AddMapping<SiteConfiguration>();
                    var hbmMapping = mapping.CompileMappingForAllExplicitlyAddedEntities();
                    config.AddDeserializedMapping( hbmMapping, "dbo" );

                    var dialect = Dialect.GetDialect( config.GetDerivedProperties() );
                    SchemaMetadataUpdater.QuoteTableAndColumns( config, dialect );
                    SchemaValidator sv = new SchemaValidator( config );
                    sv.Validate();

                    var sessionFactory = config.BuildSessionFactory();
                    services.TryAddSingleton<ISessionFactory>( sessionFactory );
                } )
                .ConfigureLogging( ( hostingContext, logging ) => {
                    logging.AddConfiguration( hostingContext.Configuration.GetSection( "Logging" ) );
                    logging.AddConsole();
                } );

            await builder.RunConsoleAsync();

        }
    }
}
