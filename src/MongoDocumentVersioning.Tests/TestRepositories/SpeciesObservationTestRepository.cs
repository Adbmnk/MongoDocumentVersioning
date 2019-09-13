using System;
using System.Collections.Generic;
using MongoDocumentVersioning.Models.Observations;

namespace MongoDocumentVersioning.Tests.TestRepositories
{
    public static class SpeciesObservationTestRepository
    {
        private static Random random = new Random();
        private static int counter = 1;

        private static int[] TaxonIds =
        {
            103025, // Blåmes
            101656, // Trumgräshoppa
            100067, // Havsörn
            100024, // Varg
            220396 // Tussilago
        };

        public static SpeciesObservation CreateRandomObservation()
        {
            SpeciesObservation observation = new SpeciesObservation();
            observation.DataProviderId = random.Next(1, 5);
            observation.CatalogNumber = counter.ToString();
            counter++;
            var person = PersonTestRepository.GetRandomPerson();
            observation.RecordedBy = $"{person.FirstName} {person.LastName}";
            observation.CoordinateX = random.NextDouble() * 100;
            observation.CoordinateY = random.NextDouble() * 100;
            //observation.ReportedDate = new DateTime(random.Next(1970, 2019), random.Next(1, 13), random.Next(1, 29));
            observation.DyntaxaTaxonId = TaxonIds[random.Next(0, TaxonIds.Length)];
            return observation;
        }

        public static List<SpeciesObservation> CreateRandomObservations(int numberOfObservations)
        {
            List<SpeciesObservation> speciesObservations = new List<SpeciesObservation>(numberOfObservations);
            for(int i=0; i < numberOfObservations; i++)
            {
                speciesObservations.Add(CreateRandomObservation());
            }

            return speciesObservations;
        }
    }
}
