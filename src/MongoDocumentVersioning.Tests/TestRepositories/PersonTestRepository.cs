using System;
using System.Collections.Generic;
using MongoDocumentVersioning.Models.Persons;

namespace MongoDocumentVersioning.Tests.TestRepositories
{
    public static class PersonTestRepository
    {
        private static Random random = new Random();

        private static string[] FirstNames =
        {
            "Romeo",
            "Tord",
            "Glenn",
            "Bill",
            "Bull"
        };

        private static string[] LastNames =
        {
            "Olsson",
            "Yvel",
            "Åkesson",
            "Hysén"
        };

        private static int[] Ages =
        {
            32,
            39,
            45,
            51
        };


        public static List<IPerson> GetRandomPersons(int numberOfPersons)
        {
            List<IPerson> persons = new List<IPerson>(numberOfPersons);
            for (int i = 0; i < numberOfPersons; i++)
            {
                persons.Add(GetRandomPerson());
            }

            return persons;
        }

        public static IPerson GetRandomPerson()
        {
            string firstName = FirstNames[random.Next(0, FirstNames.Length)];
            string lastName = LastNames[random.Next(0, LastNames.Length)];
            int age = Ages[random.Next(0, Ages.Length - 1)];
            IPerson person;
            if (random.Next(1, 3) == 1)
            {
                person = new PersonVer1(firstName, lastName);
            }
            else
            {
                person = new PersonVer2(firstName, lastName, age);
            }

            return person;
        }

        public static List<IPerson> UpdatePersons(
            List<IPerson> persons,
            int percentToUpdate)
        {
            List<IPerson> updatedPersons = new List<IPerson>(persons.Count);
            foreach (var person in persons)
            {
                if (random.Next(1, 101) <= percentToUpdate)
                {
                    updatedPersons.Add(GetRandomPerson());
                }
                else
                {
                    updatedPersons.Add(person);
                }
            }

            return updatedPersons;
        }

        public static void UpdatePersonsInList(
            List<IPerson> persons,
            int percentToUpdate)
        {
            for (var i = 0; i < persons.Count; i++)
            {
                if (random.Next(1, 101) <= percentToUpdate)
                {
                    persons[i] = GetRandomPerson();
                }
            }
        }


        private static string[] _names =
        {
            "H.E. Pennypacker",
            "Kel Varnsen",
            "Art Vandelay",
            "Dr. Martin Van Nostrand",
            "Dr. Peter Van Nostrand",
            "Paloma Pepper",
            "Wanda Pepper",
            "Dylan Murphy",
            "Eduardo Corrochio"
        };

    }
}
