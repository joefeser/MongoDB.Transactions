using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Transactions {
    public class Account {
        public Account() {
            PendingTransactions = new List<string>();
        }
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
        public List<string> PendingTransactions { get; set; }
    }
}
