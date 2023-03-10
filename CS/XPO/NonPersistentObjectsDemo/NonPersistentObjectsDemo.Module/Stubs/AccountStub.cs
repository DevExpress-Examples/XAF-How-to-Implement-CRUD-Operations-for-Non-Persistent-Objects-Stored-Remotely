using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonPersistentObjectsDemo.Module.Stubs {
    [DebuggerDisplay("{MyName}")]
    public class AccountStub {
        public string MyKey { get; set; }
        public string MyName { get; set; }
    }
}
