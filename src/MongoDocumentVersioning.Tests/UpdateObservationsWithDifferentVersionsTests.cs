using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDocumentVersioning.Models.Observations;
using MongoDocumentVersioning.Repositories;
using MongoDocumentVersioning.Tests.TestRepositories;
using Xunit;

namespace MongoDocumentVersioning.Tests
{
    public class UpdateObservationsWithDifferentVersionsTests
    {
        private const string MongoUrl = "mongodb://localhost";
        private const string DatabaseName = "diff_sample";
        private const string CollectionName = "observations";

        [Fact]
        public async Task TestAddAndUpdateOneObservationCreatingDifferentVersions()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Database connection, etc.
            //-----------------------------------------------------------------------------------------------------------
            MongoDbContext dbContext = new MongoDbContext(MongoUrl, DatabaseName, CollectionName);
            await dbContext.Mongodb.DropCollectionAsync(CollectionName);
            var observationRepository = new VersionedDocumentRepositoryObservation<SpeciesObservation>(dbContext);

            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Observation versions. One original version and 3 different updated versions.
            //-----------------------------------------------------------------------------------------------------------
            SpeciesObservation originalObservation = SpeciesObservationTestRepository.CreateRandomObservation();
            var observationVersion2 = (SpeciesObservation)originalObservation.Clone();
            observationVersion2.RecordedBy = "Peter van Nostrand";
            var observationVersion3 = (SpeciesObservation)observationVersion2.Clone();
            observationVersion3.CoordinateX = 54.234;
            var observationVersion4 = (SpeciesObservation)observationVersion3.Clone();
            observationVersion4.RecordedBy = "Art Vandelay";

            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Variables
            //-----------------------------------------------------------------------------------------------------------
            int dataProviderId = originalObservation.DataProviderId;
            string catalogNumber = originalObservation.CatalogNumber;
            

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------                                    
            await observationRepository.InsertDocumentAsync(originalObservation); // Version 1, First insert
            await observationRepository.InsertDocumentAsync(observationVersion2); // Version 2, Change [RecordedBy]
            await observationRepository.InsertDocumentAsync(observationVersion3); // Version 3, Change [CoordinateX]
            await observationRepository.DeleteDocumentAsync(dataProviderId, catalogNumber); // Version 4, Delete document
            await observationRepository.InsertDocumentAsync(observationVersion4); // Version 5, Change [RecordedBy]
            await observationRepository.DeleteDocumentAsync(dataProviderId, catalogNumber); // Version 6, Delete document


            //-----------------------------------------------------------------------------------------------------------
            // Act - Restore versions
            //-----------------------------------------------------------------------------------------------------------
            var restoredVer6 = await observationRepository.RestoreDocumentAsync(dataProviderId, catalogNumber, 6);
            var restoredVer5 = await observationRepository.RestoreDocumentAsync(dataProviderId, catalogNumber, 5);
            var restoredVer4 = await observationRepository.RestoreDocumentAsync(dataProviderId, catalogNumber, 4);
            var restoredVer3 = await observationRepository.RestoreDocumentAsync(dataProviderId, catalogNumber, 3);
            var restoredVer2 = await observationRepository.RestoreDocumentAsync(dataProviderId, catalogNumber, 2);
            var restoredVer1 = await observationRepository.RestoreDocumentAsync(dataProviderId, catalogNumber, 1);



            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            restoredVer6.Should().BeNull("the observation was deleted in version 6");
            restoredVer5.Should().BeEquivalentTo(observationVersion4, "in version 5, the observationVersion4 was inserted");
            restoredVer4.Should().BeNull("the observation was deleted in version 4");
            restoredVer3.Should().BeEquivalentTo(observationVersion3, "in version 3, the observationVersion3 was inserted");
            restoredVer2.Should().BeEquivalentTo(observationVersion2, "in version 2, the observationVersion2 was inserted");
            restoredVer1.Should().BeEquivalentTo(originalObservation, "in the first version, the observationVersion1 was inserted");
        }


        [Fact]
        public async Task TestAddAndUpdateMultipleObservations()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Database connection, etc.
            //-----------------------------------------------------------------------------------------------------------
            MongoDbContext dbContext = new MongoDbContext(MongoUrl, DatabaseName, CollectionName);
            await dbContext.Mongodb.DropCollectionAsync(CollectionName);
            var observationRepository = new VersionedDocumentRepositoryObservation<SpeciesObservation>(dbContext);
            const int numberOfObservations = 10000;

            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Create <DataProviderId, CatalogNumber> index
            //-----------------------------------------------------------------------------------------------------------
            var indexDefinition = Builders<VersionedDocumentObservation<SpeciesObservation>>.IndexKeys.Combine(
                Builders<VersionedDocumentObservation<SpeciesObservation>>.IndexKeys.Ascending(f => f.DataProviderId),
                Builders<VersionedDocumentObservation<SpeciesObservation>>.IndexKeys.Ascending(f => f.CatalogNumber));
            CreateIndexOptions opts = new CreateIndexOptions {Unique = true};
            await observationRepository.Collection.Indexes.CreateOneAsync(new CreateIndexModel<VersionedDocumentObservation<SpeciesObservation>>(indexDefinition, opts));

            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Create <CompositeId> index
            //-----------------------------------------------------------------------------------------------------------
            //await observationRepository.Collection.Indexes.CreateOneAsync(Builders<VersionedDocumentObservation<SpeciesObservation>>.IndexKeys.Ascending(_ => _.CompositeId), opts);

            //-----------------------------------------------------------------------------------------------------------
            // Arrange - Create original observations
            //-----------------------------------------------------------------------------------------------------------
            var speciesObservations = SpeciesObservationTestRepository.CreateRandomObservations(numberOfObservations);


            //-----------------------------------------------------------------------------------------------------------
            // Act - Insert documents first version
            //-----------------------------------------------------------------------------------------------------------
            await observationRepository.InsertDocumentsAsync(speciesObservations);


            //-----------------------------------------------------------------------------------------------------------
            // Act - Update two observations and insert
            //-----------------------------------------------------------------------------------------------------------
            speciesObservations[0].RecordedBy = "Art Vandelay";
            speciesObservations[2].RecordedBy = "Peter Van Nostrand";

            // Insert again. The function make diff check on all observations and updates only those that have changed.
            await observationRepository.InsertDocumentsAsync(speciesObservations); 


            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            var obs0 = await observationRepository.GetDocumentAsync(speciesObservations[0].DataProviderId,  speciesObservations[0].CatalogNumber);
            var obs1 = await observationRepository.GetDocumentAsync(speciesObservations[1].DataProviderId, speciesObservations[1].CatalogNumber);
            var obs2 = await observationRepository.GetDocumentAsync(speciesObservations[2].DataProviderId, speciesObservations[2].CatalogNumber);

            obs0.Version.Should().Be(2, "This observation has been updated");
            obs1.Version.Should().Be(1, "This observation has not been updated");
            obs2.Version.Should().Be(2, "This observation has been updated");
        }

    }
}


// Date - test code
//observationVersion1.ReportedDate = new DateTime(2019,9,11, 13,45,0).ToUniversalTime(); // UTC = 11:45:00
//observationVersion1.ReportedDateOffset = new DateTimeOffset(new DateTime(2019, 9, 11, 13, 45, 0)); // UTC = 11:45:00
//observationVersion1.ReportedDate = observationVersion1.ReportedDateOffset.UtcDateTime;
//observationVersion3.ReportedDate = DateTime.UtcNow;