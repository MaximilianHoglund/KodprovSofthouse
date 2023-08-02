using System;
using System.IO;
using System.Xml.Serialization;

namespace Kodprov_Softhouse
{
    static class Parser
    {
        public static string TextToXml(string text)
        {
            string[] lines = text.Split(Environment.NewLine);
            
            var peopleList = new People();
            Person currentPerson = null;
            Family currentFamily = null;
            
            foreach (string line in lines)
            {
                string[] segments = line.Split('|');
                switch (segments[0])
                {
                    case "P":
                        currentFamily = null;
                        currentPerson = HandlePersonEntry(peopleList, segments);
                        break;

                    case "T":
                        HandleTelephoneEntry(segments, currentPerson, currentFamily);
                        break;

                    case "A":
                        HandleAddressEntry(segments, currentPerson, currentFamily);
                        break;

                    case "F":
                        currentFamily = HandleFamilyEntry(currentPerson, segments);
                        break;

                    default:
                        throw new ArgumentException($"Unsupported prefix encountered. Prefix: '{segments[0]}'");
                }
            }

            XmlSerializer xs = new XmlSerializer(typeof(People));
            using (StringWriter writer = new StringWriter())
            {
                xs.Serialize(writer, peopleList);
                return writer.ToString();
            }
        }

        private static Person HandlePersonEntry(People toSerialize, string[] segments)
        {
            Person currentPerson;
            AssertSegmentsLength(segments, 3);
            var newPerson = new Person() { firstname = segments[1], lastname = segments[2] };

            currentPerson = newPerson;
            toSerialize.people.Add(newPerson);
            return currentPerson;
        }

        private static Family HandleFamilyEntry(Person currentPerson, string[] segments)
        {
            Family currentFamily;
            AssertSegmentsLength(segments, 3);
            if (currentPerson == null)
            {
                throw new ArgumentException("Encountered loose-hanging (F)amily entry, input requires a (P)erson entry first to connect the family entry to.");
            }

            var success = int.TryParse(segments[2], out int yearOfBirth);
            if (!success)
            {
                throw new ArgumentException($"Unable to parse {segments[2]} to a year of birth in {segments}");
            }

            var newFamily = new Family() { name = segments[1], born = yearOfBirth };

            currentPerson.family.Add(newFamily);
            currentFamily = newFamily;
            return currentFamily;
        }

        private static void HandleAddressEntry(string[] segments, Person currentPerson, Family currentFamily)
        {
            AssertSegmentsLength(segments, 4);
            var newAddress = new Address() { street = segments[1], city = segments[2], zip = segments[3] };

            if (currentFamily != null)
            {
                currentFamily.address = newAddress;
            }
            else if (currentPerson != null)
            {
                currentPerson.address = newAddress;
            }
            else
            {
                throw new ArgumentException("Encountered loose-hanging (A)ddress entry, input requires a (P)erson or (F)amily entry first to connect the address entry to.");
            }
        }

        private static void HandleTelephoneEntry(string[] segments, Person currentPerson, Family currentFamily)
        {
            AssertSegmentsLength(segments, 3);
            var newPhone = new Phone() { mobile = segments[1], landline = segments[2] };
            if (currentFamily != null)
            {
                currentFamily.phone = newPhone;
            }
            else if (currentPerson != null)
            {
                currentPerson.phone = newPhone;
            }
            else
            {
                throw new ArgumentException("Encountered loose-hanging (T)elephone entry, input requires a (P)erson or (F)amily entry first to connect the phone entry to.");
            }
        }

        private static void AssertSegmentsLength(string[] segments, int expectedLength)
        {
            if (segments.Length != expectedLength)
            {
                throw new ArgumentException($"Expected {expectedLength} segments for parameter type {segments[0]}, but it had {segments.Length}. Aborting.");
            }
        }
    }
}
