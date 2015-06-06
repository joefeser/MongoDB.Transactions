using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Transactions {
    public class DecimalSerializer : IBsonSerializer<decimal> {

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            return ((decimal)context.Reader.ReadInt64()) / 10000;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) {
            if (value is decimal) {
                context.Writer.WriteInt64((long)((decimal)value * 10000));
            }
        }

        public Type ValueType {
            get { return typeof(decimal); }
        }

        decimal IBsonSerializer<decimal>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
            return ((decimal)context.Reader.ReadInt64()) / 10000;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, decimal value) {
            context.Writer.WriteInt64((long)((decimal)value * 10000));
        }
    }
}
