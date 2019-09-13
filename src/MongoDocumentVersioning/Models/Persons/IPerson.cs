namespace MongoDocumentVersioning.Models.Persons
{
    public interface IPerson
    {
        string FirstName { get; set; }
        
        string LastName { get; set; }
    }
}