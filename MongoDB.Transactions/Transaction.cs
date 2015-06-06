using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Transactions {

    public enum TransactionState {
        Initial,
        Pending,
        Applied,
        Done,
        Cancelling,
        Cancelled
    }

    public class Transaction {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public decimal Value { get; set; }
        public TransactionState State { get; set; }
        public DateTime LastModified { get; set; }
    }
}
