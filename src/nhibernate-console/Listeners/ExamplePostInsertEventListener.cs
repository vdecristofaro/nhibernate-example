

using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate.Event;

namespace nhibernate_console.Listeners {

    public class ExamplePostInsertEventListener : IPostInsertEventListener {

        private JsonSerializerSettings settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public ExamplePostInsertEventListener() {
        }

        public void OnPostInsert( PostInsertEvent @event ) {
            Console.WriteLine( $"Received PostInsert event for entity {JsonConvert.SerializeObject( @event.Entity, Formatting.None, settings )}" );
        }

        public async Task OnPostInsertAsync( PostInsertEvent @event, CancellationToken cancellationToken ) {
            Console.WriteLine( $"Received PostInsert event for entity {JsonConvert.SerializeObject( @event.Entity, Formatting.None, settings )}" );
            await Task.CompletedTask;
        }
    }

}
