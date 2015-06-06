using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Transactions {

    public static class Mongo {
        public static string connectionString = ConfigurationManager.AppSettings["MongoServer"];
        public static MongoClient Client = new MongoClient(connectionString);
        public static IMongoDatabase MainDatabase = Client.GetDatabase("bank");
        public static IMongoCollection<Account> AccountCollection = MainDatabase.GetCollection<Account>("account");
        public static IMongoCollection<Transaction> TransactionCollection = MainDatabase.GetCollection<Transaction>("transaction");
    }
}
