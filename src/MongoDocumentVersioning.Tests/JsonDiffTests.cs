using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using JsonDiffPatchDotNet;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MongoDocumentVersioning.Tests
{
    public class JsonDiffTests
    {
        [Fact]
        public void TestDiffDocuments()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var jdp = new JsonDiffPatch();

            var person1 = new Person("Tord", "Yvel");
            var person1Jtoken = JToken.FromObject(person1);

            var person2 = new Person("Tord", "Yvels");
            var person2Jtoken = JToken.FromObject(person2);

            var person3 = new Person("Romeo", "Olsson");
            var person3Jtoken = JToken.FromObject(person3);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            JToken patch1 = jdp.Diff(person1Jtoken, person2Jtoken);
            JToken patch2 = jdp.Diff(person2Jtoken, person3Jtoken);
            var unpatch2 = jdp.Unpatch(person3Jtoken, patch2);
            var restoredVer2 = unpatch2.ToObject(typeof(Person));
            var unpatch1 = jdp.Unpatch(unpatch2, patch1);
            var restoredVer1 = unpatch1.ToObject(typeof(Person));
            
            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            restoredVer1.Should().BeEquivalentTo(person1);
            restoredVer2.Should().BeEquivalentTo(person2);
        }


        public class Person
        {
            public ObjectId Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }
        }
    }
}
