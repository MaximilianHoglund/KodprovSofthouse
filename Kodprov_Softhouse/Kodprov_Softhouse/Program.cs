using System;

namespace Kodprov_Softhouse
{
    class Program
    {
        static void Main(string[] args)
        {
            string rawText = TestStrings.TestInput;
            string xmlResult = Parser.TextToXml(TestStrings.TestInput);
            Console.Write(xmlResult); 
            Console.ReadLine();
        }
    }
}
