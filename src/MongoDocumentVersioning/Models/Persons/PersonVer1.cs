namespace MongoDocumentVersioning.Models.Persons
{
    public class PersonVer1 : IPerson
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public PersonVer1(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public PersonVer1()
        {
        }

        public override string ToString()
        {
            return $"{nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}";
        }
    }
}