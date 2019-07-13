
using System;
using System.Collections.Generic;

namespace nhibernate_console.Entities {
    public class Customer : Entity {
        public Customer() {
            Initialize();
        }

        public virtual string Name { get; set; }

        public virtual string LegalName { get; set; }

        public virtual string VATCode { get; set; }

        public virtual ICollection<Site> Sites { get; set; }

        public virtual DateTime Created { get; set; } = DateTime.UtcNow;

        #region Private methods

        private void Initialize() {
            Sites = new List<Site>();
        }

        #endregion
    }
}
