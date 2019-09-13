namespace MongoDocumentVersioning.Models
{
    public class ObservationIdTuple
    {
        public int DataProviderId { get; set; }
        public string CatalogNumber { get; set; }

        public ObservationIdTuple(int dataProviderId, string catalogNumber)
        {
            DataProviderId = dataProviderId;
            CatalogNumber = catalogNumber;
        }

        public bool Equals(ObservationIdTuple other)
        {
            return DataProviderId == other.DataProviderId && CatalogNumber == other.CatalogNumber;
        }

        public override bool Equals(object obj)
        {
            return obj is ObservationIdTuple other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DataProviderId * 397) ^ (CatalogNumber != null ? CatalogNumber.GetHashCode() : 0);
            }
        }
    }
}