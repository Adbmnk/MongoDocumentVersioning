using System;

namespace MongoDocumentVersioning.Models.Observations
{
    public class SpeciesObservation : ICloneable, IObservationKey
    {
        public string CatalogNumber { get; set; }
        public int DataProviderId { get; set; }
        public string RecordedBy { get; set; }
        //[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        //public DateTime ReportedDate { get; set; }
        public int DyntaxaTaxonId { get; set; }
        public Double CoordinateX { get; set; }
        public Double CoordinateY { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
