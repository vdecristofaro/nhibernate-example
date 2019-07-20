
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Engine;
using nhibernate_console.Entities;

namespace nhibernate_console {
    public class NhibernateService : IHostedService, IDisposable {

        private readonly ILogger _logger;
        private Timer _timer;
        private static Random random = new Random();
        private static int _counter = 1;

        private readonly ISessionFactory _sessionFactory;

        public NhibernateService( ILogger<NhibernateService> logger, ISessionFactory sessionFactory ) {
            _logger = logger;
            _sessionFactory = sessionFactory;
        }

        public Task StartAsync( CancellationToken cancellationToken ) {
            _logger.LogInformation( "Starting" );

            _timer = new Timer( DoWork, null, TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ) );

            return Task.CompletedTask;
        }

        private void DoWork( object state ) {
            using ( var session = _sessionFactory.OpenSession() ) {
                using ( var tx = session.BeginTransaction() ) {
                    //insert
                    var customerToAdd = new Customer() {
                        Name = $"Customer 1",
                        LegalName = $"Customer 1 LLC",
                        VATCode = "xxxxxxxxx",
                        Created = DateTime.UtcNow
                    };
                    customerToAdd.Sites.Add( new Site() {
                        Address = $"Address Customer 1",
                        City = $"City Customer 1",
                        Country = $"Country Customer 1",
                        Customer = customerToAdd
                    } );
                    session.Save( customerToAdd );

                    //update
                    if ( _counter % 7 == 0 ) {
                        var customertoUpdate = session.Query<Customer>().OrderBy( x => x.EntityID ).FirstOrDefault();
                        if ( customertoUpdate != null ) {
                            customertoUpdate.Name = $"Modified at {DateTime.Now.ToString( "HH:mm:ss" )}";
                            _logger.LogInformation( $"Updating customer {customertoUpdate.EntityID}" );
                            session.Update( customertoUpdate );
                        }
                    }

                    //delete
                    if ( _counter % 11 == 0 ) {
                        var customertoDelete = session.Query<Customer>().OrderBy( x => x.EntityID ).FirstOrDefault();
                        if ( customertoDelete != null ) {
                            _logger.LogInformation( $"Deleting customer {customertoDelete.EntityID}" );
                            session.Delete( customertoDelete );
                        }
                    }

                    //dirty entities detection
                    var dirtyObjects = new List<object>();
                    var sessionImpl = session.GetSessionImplementation();
                    var persistenceContext = sessionImpl.PersistenceContext;
                    foreach ( EntityEntry entityEntry in persistenceContext.EntityEntries.Values ) {
                        var loadedState = entityEntry.LoadedState;
                        var theObject = persistenceContext.GetEntity( entityEntry.EntityKey );
                        var currentState = entityEntry.Persister.GetPropertyValues( theObject );
                        if ( entityEntry.Persister.FindDirty( currentState, loadedState, theObject, sessionImpl ) != null ) {
                            dirtyObjects.Add( entityEntry );
                        }
                    }
                    _logger.LogTrace( $"*** --- Dirty entities --- ***" );
                    foreach ( var entity in dirtyObjects ) {
                        _logger.LogTrace( $"Dirty Entity: {entity.ToString()}" );
                    }
                    _logger.LogTrace( $"*** ---------------------- ***" );

                    tx.Commit();
                    _counter++;
                }
            }
        }

        public Task StopAsync( CancellationToken cancellationToken ) {
            _logger.LogInformation( "Stopping." );

            _timer?.Change( Timeout.Infinite, 0 );

            return Task.CompletedTask;
        }

        public void Dispose() {
            _timer?.Dispose();
        }

    }
}
