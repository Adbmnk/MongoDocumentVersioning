using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDocumentVersioning.Models.Persons;
using MongoDocumentVersioning.Repositories;
using Xunit;

namespace MongoDocumentVersioning.Tests
{
    public class UpdatePersonWithDifferentVersionsTests
    {
        private const string MongoUrl = "mongodb://localhost";
        private const string DatabaseName = "diff_sample";
        private const string CollectionName = "persons";

        [Fact]
        public async Task TestAddAndUpdateOnePersonCreatingDifferentVersions()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Database connection, etc.
            //-----------------------------------------------------------------------------------------------------------
            MongoDbContext dbContext = new MongoDbContext(MongoUrl, DatabaseName, CollectionName);
            await dbContext.Mongodb.DropCollectionAsync(CollectionName);
            var personRepository = new VersionedDocumentRepository<IPerson>(dbContext);


            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Person versions. One original version, and 3 updated versions.
            //-----------------------------------------------------------------------------------------------------------
            var originalPerson = new PersonVer1(firstName: "Martin", lastName: "Van Nostrand");
            var personVersion2 = new PersonVer2(firstName: "Peter", lastName: "Van Nostrand", age: 45);
            var personVersion3 = new PersonVer1(firstName: "Kel", lastName: "Varnsen");
            var personVersion4 = new PersonVer2(firstName: "Art", lastName: "Vandelay", age: 37);
            ObjectId objectId = ObjectId.GenerateNewId();

            //-----------------------------------------------------------------------------------------------------------
            // Act - Insert and Delete documents
            //-----------------------------------------------------------------------------------------------------------                                    
            var currentDocument = await personRepository.InsertDocumentAsync(objectId, originalPerson); // Version 1, First insert
            objectId = currentDocument.Id;
            await personRepository.InsertDocumentAsync(objectId, personVersion2); // Version 2, Change [last name, person type].
            await personRepository.InsertDocumentAsync(objectId, personVersion3); // Version 3, Change [first name, last name, person type].
            await personRepository.DeleteDocumentAsync(objectId); // Version 4, Delete document
            await personRepository.InsertDocumentAsync(objectId, personVersion4); // Version 5, Change [first name, last name]
            await personRepository.DeleteDocumentAsync(objectId); // Version 6, Delete document

            //-----------------------------------------------------------------------------------------------------------
            // Act - Restore versions
            //-----------------------------------------------------------------------------------------------------------
            var restoredVer6 = await personRepository.RestoreDocumentAsync(objectId, 6);
            var restoredVer5 = await personRepository.RestoreDocumentAsync(objectId, 5);
            var restoredVer4 = await personRepository.RestoreDocumentAsync(objectId, 4);
            var restoredVer3 = await personRepository.RestoreDocumentAsync(objectId, 3);
            var restoredVer2 = await personRepository.RestoreDocumentAsync(objectId, 2);
            var restoredVer1 = await personRepository.RestoreDocumentAsync(objectId, 1);

            //-----------------------------------------------------------------------------------------------------------
            // Assert - Restored versions
            //-----------------------------------------------------------------------------------------------------------
            restoredVer6.Should().BeNull("the observation was deleted in version 6");
            restoredVer5.Should().BeEquivalentTo(personVersion4, "in version 5, the personVersion4 was inserted");
            restoredVer4.Should().BeNull("the observation was deleted in version 4");
            restoredVer3.Should().BeEquivalentTo(personVersion3, "in version 3, the personVersion3 was inserted");
            restoredVer2.Should().BeEquivalentTo(personVersion2, "in version 2, the personVersion2 was inserted");
            restoredVer1.Should().BeEquivalentTo(originalPerson, "in version 1, the originalPerson was inserted");
        }

    }
}