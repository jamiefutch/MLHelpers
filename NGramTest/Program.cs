using System;
using MLHelpers.Text;

namespace NGramTest
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            const string kText =
                "this is a test. this is only a test.  if this had been an actual program it would not have been so dumb";
            NGrams ngs = new NGrams(3);

            Console.WriteLine("N-Grams from 'raw' text...");
            var ngrams = ngs.GenerateNGramsStrings(kText);

            foreach (var nGram in ngrams)
            {
                Console.WriteLine(nGram.ToString());
            }

            Console.WriteLine("========================================");

            Console.WriteLine("N-Grams from 'normalize and stop words removed' text...");
            var normalizer = new TextNormalizer();
            var normalizedText = normalizer.NormalizeText(kText);
            var stopWordsRemover = new StopWordsRemover();
            var stopWordsRemoved = stopWordsRemover.RemoveStopWords(normalizedText);
            
            Console.WriteLine("Normalized and Stop words removed text:");
            Console.WriteLine(stopWordsRemoved);
            Console.WriteLine("\n\n\n");

            var normalizedWithoutStopWordsRemoved = normalizer.NormalizeText(kText);
            Console.WriteLine("Normalized and Stop words NOT removed text:");
            Console.WriteLine(normalizedWithoutStopWordsRemoved);
            Console.WriteLine("\n\n\n");

            var ngrams2 = ngs.GenerateNGrams(stopWordsRemoved);

            Console.WriteLine("N-Grams from 'normalized' text...");
            foreach (var nGram in ngrams2)
            {
                Console.WriteLine(nGram.ToString());
            }

            PrintEnd();
        }

        static void PrintEnd()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("done.... press any key to continue...");
            Console.ReadKey();
        }
    }
}
