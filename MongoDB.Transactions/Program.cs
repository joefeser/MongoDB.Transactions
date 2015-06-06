using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Transactions {
    class Program {
        static void Main(string[] args) {
            Task t = MainAsync(args);
            t.Wait();
        }

        static async Task MainAsync(string[] args) {
            Setup.Initialize();
            await LoadAccounts();
            await CreateTransaction();
            var transaction = await GetInitialTransaction();
            transaction = await UpdateAccountWithPendingTransaction(transaction);
            await SetTransactionApplied(transaction);
            await RemovePendingTransaction(transaction);
            await SetTransactionComplete(transaction);
        }

        /*        
        var dateThreshold = new Date();
        dateThreshold.setMinutes(dateThreshold.getMinutes() - 30);

        var t = db.transactions.findOne( { state: "applied", lastModified: { $lt: dateThreshold } } );


        db.transactions.update(
           { _id: t._id, state: "pending" },
           {
             $set: { state: "canceling" },
             $currentDate: { lastModified: true }
           }
        )
        */

        private static async Task<Transaction> SetTransactionCancelling(Transaction transaction) {
            var query = Builders<Transaction>.Filter.Where(item => item.State == TransactionState.Pending);
            var update = Builders<Transaction>.Update.Set(item => item.State, TransactionState.Cancelling).Set(item => item.LastModified, DateTime.Now);
            var options = new FindOneAndUpdateOptions<Transaction, Transaction>();
            options.ReturnDocument = ReturnDocument.After;
            var data = await Mongo.TransactionCollection.FindOneAndUpdateAsync(query, update, options);
            return data;
        }

        private static async Task<Transaction> GetInitialTransaction() {
            var query = Builders<Transaction>.Filter.Where(item => item.State == TransactionState.Initial);
            var update = Builders<Transaction>.Update.Set(item => item.State, TransactionState.Pending).Set(item => item.LastModified, DateTime.Now);
            var options = new FindOneAndUpdateOptions<Transaction, Transaction>();
            options.ReturnDocument = ReturnDocument.After;
            var data = await Mongo.TransactionCollection.FindOneAndUpdateAsync(query, update, options);
            return data;
        }

        private static async Task<Transaction> SetTransactionApplied(Transaction transaction) {
            var query = Builders<Transaction>.Filter.Where(item => item.State == TransactionState.Pending);
            var update = Builders<Transaction>.Update.Set(item => item.State, TransactionState.Applied).Set(item => item.LastModified, DateTime.Now);
            var options = new FindOneAndUpdateOptions<Transaction, Transaction>();
            options.ReturnDocument = ReturnDocument.After;
            var data = await Mongo.TransactionCollection.FindOneAndUpdateAsync(query, update, options);
            return data;
        }

        private static async Task<Transaction> SetTransactionComplete(Transaction transaction) {
            var query = Builders<Transaction>.Filter.Where(item => item.State == TransactionState.Applied);
            var update = Builders<Transaction>.Update.Set(item => item.State, TransactionState.Done).Set(item => item.LastModified, DateTime.Now);
            var options = new FindOneAndUpdateOptions<Transaction, Transaction>();
            options.ReturnDocument = ReturnDocument.After;
            var data = await Mongo.TransactionCollection.FindOneAndUpdateAsync(query, update, options);
            return data;
        }

        private static async Task CreateTransaction() {
            var transaction = new Transaction() {
                Destination = "9876543210",
                Id = Guid.NewGuid().ToString(),
                LastModified = DateTime.Now,
                Source = "1234567890",
                State = TransactionState.Initial,
                Value = 2500
            };
            await Mongo.TransactionCollection.InsertOneAsync(transaction);
        }

        /*
        db.accounts.update(
           { _id: t.source, pendingTransactions: { $ne: t._id } },
           { $inc: { balance: -t.value }, $push: { pendingTransactions: t._id } }
        ) 
        */

        private static async Task<Transaction> UpdateAccountWithPendingTransaction(Transaction transaction) {
            var query = Builders<Account>.Filter.Where(item => item.AccountId == transaction.Source && !item.PendingTransactions.Contains(transaction.Id));
            var update = Builders<Account>.Update.Inc(item => item.Balance, -transaction.Value).Push(item => item.PendingTransactions, transaction.Id);
            var options = new FindOneAndUpdateOptions<Account, Account>();
            options.ReturnDocument = ReturnDocument.After;
            var data = await Mongo.AccountCollection.FindOneAndUpdateAsync(query, update, options);

            query = Builders<Account>.Filter.Where(item => item.AccountId == transaction.Destination && !item.PendingTransactions.Contains(transaction.Id));
            update = Builders<Account>.Update.Inc(item => item.Balance, transaction.Value).Push(item => item.PendingTransactions, transaction.Id);
            options.ReturnDocument = ReturnDocument.After;
            data = await Mongo.AccountCollection.FindOneAndUpdateAsync(query, update, options);

            return transaction;
        }

        private static async Task<Transaction> RemovePendingTransaction(Transaction transaction) {
            var query = Builders<Account>.Filter.Where(item => item.AccountId == transaction.Source );
            var update = Builders<Account>.Update.Pull(item => item.PendingTransactions, transaction.Id);
            var options = new FindOneAndUpdateOptions<Account, Account>();
            options.ReturnDocument = ReturnDocument.After;
            var data = await Mongo.AccountCollection.FindOneAndUpdateAsync(query, update, options);

            query = Builders<Account>.Filter.Where(item => item.AccountId == transaction.Destination );
            update = Builders<Account>.Update.Pull(item => item.PendingTransactions, transaction.Id);
            options.ReturnDocument = ReturnDocument.After;
            data = await Mongo.AccountCollection.FindOneAndUpdateAsync(query, update, options);

            return transaction;
        }

        private static async Task LoadAccounts() {
            var acc = new Account() { AccountId = "1234567890", Balance = 20000 };
            await Mongo.AccountCollection.InsertOneAsync(acc);
            acc = new Account() { AccountId = "9876543210", Balance = 20000 };
            await Mongo.AccountCollection.InsertOneAsync(acc);
        }
    }
}
