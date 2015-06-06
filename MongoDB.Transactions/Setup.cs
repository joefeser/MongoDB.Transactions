using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Transactions {
    public static class Setup {

        public static void Initialize() {

            BsonClassMap.RegisterClassMap<Account>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                //cm.GetMemberMap(c => c.AccountId).SetElementName("accountId");
                cm.GetMemberMap(c => c.Balance).SetElementName("balance").SetSerializer(new DecimalSerializer());
                cm.GetMemberMap(c => c.PendingTransactions).SetElementName("pendingTransactions");
                cm.SetIdMember(cm.GetMemberMap(c => c.AccountId));
            });

            BsonClassMap.RegisterClassMap<Transaction>(cm => {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.GetMemberMap(c => c.Destination).SetElementName("destination");
                cm.GetMemberMap(c => c.LastModified).SetElementName("ts");
                cm.GetMemberMap(c => c.Source).SetElementName("source");
                cm.GetMemberMap(c => c.State).SetElementName("state");
                cm.GetMemberMap(c => c.Value).SetElementName("value");
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            });

        }

    }
}
