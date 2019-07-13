
using System;
namespace nhibernate_console.Entities {
    public class Site : Entity {
        public Site() {
        }

        public virtual string Address { get; set; }

        public virtual string City { get; set; }

        public virtual string Country { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
