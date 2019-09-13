using System;
using System.Threading.Tasks;
using JsonDiffPatchDotNet;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDocumentVersioning.Models;
using MongoDocumentVersioning.Models.Persons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MongoDocumentVersioning.Repositories
{
    public class VersionedDocumentRepository<T> where T : class
    {
        private readonly MongoDbContext _dbContext;
        public IMongoCollection<VersionedDocument<T>> Collection { get; private set; }
        private readonly JsonDiffPatch _jdp = new JsonDiffPatch();

        public VersionedDocumentRepository(MongoDbContext dbContext)
        {
            _dbContext = dbContext;
            Collection = _dbContext.MongoDbCollection<VersionedDocument<T>>();
        }

        public async Task<VersionedDocument<T>> InsertDocumentAsync(
            ObjectId objectId, 
            T doc)
        {
            VersionedDocument<T> currentDocument = null;
            if (objectId != ObjectId.Empty)
            {
                currentDocument = await (await Collection.FindAsync(x => x.Id == objectId)).FirstOrDefaultAsync();
            }

            if (currentDocument == null)
            {
                var versionedDoc = new VersionedDocument<T>(doc);
                Collection.InsertOne(versionedDoc);
                return versionedDoc;
            }

            var previousVersionNumber = currentDocument.Version;
            JToken jtokenCurrentDoc = currentDocument.Current == null
                ? JToken.Parse("{}")
                : JToken.FromObject(currentDocument.Current);
            JToken diff = _jdp.Diff(jtokenCurrentDoc, JToken.FromObject(doc));
            if (diff == null) return null; // no change

            currentDocument.Current = doc;
            VersionHistory versionHistory = new VersionHistory(diff.ToString(Formatting.None))
            {
                Version = previousVersionNumber,
                UtcDate = currentDocument.UtcDate,
                Type = currentDocument.Type
            };
            currentDocument.UtcDate = DateTime.UtcNow;
            currentDocument.Prev.Add(versionHistory);
            currentDocument.Version = previousVersionNumber + 1;
            currentDocument.IsDeleted = false;
            currentDocument.Type = doc.GetType().ToString();
            var result = Collection.ReplaceOne(item => item.Id == objectId, currentDocument);

            if (result.ModifiedCount != 1) // the number of modified documents
            {
                // print("Someone must have gotten there first, re-fetch the new document, try again");
                // todo - är det här något som kan uppstå?
            }

            return null;
        }


        public async Task DeleteDocumentAsync(
            ObjectId objectId)
        {
            // todo - check if we delete an already deleted document, then return
            var currentDocument = (await Collection.FindAsync(x => x.Id == objectId)).First();
            var previousVersionNumber = currentDocument.Version;
            JToken diff = _jdp.Diff(JToken.FromObject(currentDocument.Current), JToken.Parse("{}"));
            currentDocument.Current = null;
            VersionHistory versionHistory = new VersionHistory(diff.ToString(Formatting.None))
            {
                Version = previousVersionNumber,
                UtcDate = currentDocument.UtcDate,
                IsDeleted = true,
                Type = currentDocument.Type
            };
            currentDocument.UtcDate = DateTime.UtcNow;
            currentDocument.Prev.Add(versionHistory);
            currentDocument.Version = previousVersionNumber + 1;
            currentDocument.IsDeleted = true;
            Collection.ReplaceOne(item => item.Id == objectId, currentDocument);
        }


        public async Task<T> RestoreDocumentAsync(
            ObjectId id, 
            int version)
        {
            if (version < 1) throw new ArgumentException("Version must be >= 1");
            var jdp = new JsonDiffPatch();
            var findAsync = await Collection.FindAsync(x => x.Id == id);
            var currentDocument = findAsync.First();

            if (version > currentDocument.Version) throw new ArgumentException($"version {version} doesn't exist");
            if (version == currentDocument.Version) return currentDocument.Current;

            int versionCounter = currentDocument.Version;
            JToken jtokenCurrentDoc = currentDocument.Current == null
                ? JToken.Parse("{}")
                : JToken.FromObject(currentDocument.Current);

            JToken restoredDocument = jdp.Unpatch(
                jtokenCurrentDoc,
                JToken.Parse(currentDocument.Prev[versionCounter - 2].Diff));
            string strType = currentDocument.Prev[versionCounter - 2].Type;
            versionCounter--;
            
            while (versionCounter > version)
            {
                restoredDocument = jdp.Unpatch(
                    restoredDocument,
                    JToken.Parse(currentDocument.Prev[versionCounter - 2].Diff));
                strType = currentDocument.Prev[versionCounter - 2].Type;
                versionCounter--;
            }

            // Can use this to know if a document is deleted.
            if (!restoredDocument.HasValues)
            {
                return null;
            }

            // Or you can use this to know if a document is deleted.
            //if (versionCounter >= 2 && currentDocument.Prev[versionCounter - 2].IsDeleted)
            //{
            //    return null;
            //}
            Type type = Type.GetType(strType);
            T restoredObject = restoredDocument.ToObject(type) as T;
            return restoredObject;
        }

    }
}
