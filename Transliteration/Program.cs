using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transliteration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var yolo = "geia sou ti kaneis";

            var result = LinguisticTools.GreeklishHelper.GuessSentense(yolo);
            Console.WriteLine(result);
            Console.WriteLine(LinguisticTools.GreeklishHelper.GetPlainGreekLetters(yolo));
            Console.ReadKey();
        }
    }
}
