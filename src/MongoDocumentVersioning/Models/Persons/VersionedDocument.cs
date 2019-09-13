using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace MongoDocumentVersioning.Models.Persons
{
    public class VersionedDocument<T>
    {
        public ObjectId Id { get; set; }

        public T Current { get; set; }

        public DateTime UtcDate { get; set; }

        public int Version { get; set; }        

        public bool IsDeleted { get; set; }

        public string Type { get; set; }

        public List<VersionHistory> Prev { get; set; }

        public VersionedDocument(T current)
        {
            Current = current;
            UtcDate = DateTime.UtcNow;
            Version = 1;
            IsDeleted = false;
            Prev = new List<VersionHistory>();
            Type = current.GetType().ToString();
            //Type = current.GetType().AssemblyQualifiedName;
        }


        //public static VersionedDocument<T> CreateNew(T item)
        //{
        //    var doc = new VersionedDocument<T>(item);
            
        //    doc.State = "i";
        //}

        //[BsonElement("t")]
        //public DateTime Time { get; set; }

        ///// <summary>
        ///// "i" = insert
        ///// "u" = update
        ///// "r" = remove
        ///// </summary>
        //[BsonElement("s")]
        //public string State { get; set; }
        
        //[BsonElement("v")]
        //public int Version { get; set; }

        //[BsonElement("d")]
        //public IMongoDocument Document { get; set; }
    }

    //public interface IMongoDocument
    //{
    //    ObjectId Id { get; set; }
    //}
}
