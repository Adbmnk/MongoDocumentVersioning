namespace MongoDocumentVersioning.Tests
{
    public class MongoDbExampleCode
    {
        public void TestProjectionToBsonDocument()
        {
            //var projection = Builders<VersionedDocumentObservation<T>>.Projection
            //    .Include(x => x.DataProviderId)
            //    .Include(x => x.CatalogNumber)
            //    .Exclude("_id");  // _id is special and needs to be explicitly excluded if not needed
            //var options = new FindOptions<VersionedDocumentObservation<T>, BsonDocument> { Projection = projection };
            //var foundObservationsUsingProjection = (await Collection.FindAsync(filter, options)).ToList();
        }

        public void TestFilters()
        {
            //var filter2 = filterDef.In(x => (x.CatalogNumber, x.CatalogNumber), speciesObservations.Select(x => x.CatalogNumber));
            //var filt = Builders<VersionedDocumentObservation<T>>.Filter.ElemMatch(y => y.CatalogNumber, x => fruitNames.Contains(x.name));
            //var filterDef = new FilterDefinitionBuilder<VersionedDocumentObservation<T>>();
            //var filter = filterDef.In(x => x.CatalogNumber, speciesObservations.Select(x => x.CatalogNumber));
            //var filter2 = filterDef.In(x => (x.CatalogNumber, x.CatalogNumber), speciesObservations.Select(x => x.CatalogNumber));
            //var filter = filterDef.In(x => x.Id, new[] { ObjectId.Empty, ObjectId.Empty });
        }

        public void TestBulkWrite()
        {
            // the InsertManyAsync uses internally the BulkWriteAsync, so using InsertManyAsync it's the same as writing:
            // {
            //     List<BsonDocument> documents = ...
            //     collection.BulkWriteAsync(documents.Select(d => new InsertOneModel<BsonDocument>(d)));
            // }
            // Obviously, if all operations are Inserts, the InsertManyAsync should be used.


            // Example from SOS
            //var result = await db.GetCollection<Verbatim>(Constants.VerbatimCollectionName)
            //    .BulkWriteAsync(writeModel,
            //        new BulkWriteOptions()
            //            { IsOrdered = false },
            //        cancellationToken)
            //    .ConfigureAwait(false);
        }

        public void TestDifferentMultipleUpdates()
        {
            //Collection.DeleteManyAsync()
            //Collection.InsertManyAsync()
            //Collection.UpdateManyAsync()
            //Collection.BulkWriteAsync()
        }
    }
}


//class YourRepository<T> where T : IMongoIdentity
//{
//    IMongoCollection<T> collection;

//    public async Task UpdateManyAsync(IEnumerable<T> entities)
//    {
//        var updates = new List<WriteModel<T>>();
//        var filterBuilder = Builders<T>.Filter;

//        foreach (var doc in entities)
//        {
//            var filter = filterBuilder.Where(x => x.Id == doc.Id);
//            updates.Add(new ReplaceOneModel<T>(filter, doc));
//        }

//        await collection.BulkWriteAsync(updates);
//    }
//}

//public interface IMongoIdentity
//{
//    ObjectId Id { get; set; }
//}