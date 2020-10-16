using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
namespace QuickStart
{
    class Program
    {
        //Default program command line arguments
        public class Options
        {
            [Option('o', "output", Required = false, HelpText = "Outputs a pseudo random sequence.")]
            public bool Output { get; set; }
            [Option('e', "encrypt", Required = false, HelpText = "Encrypts string using specified key.")]
            public bool Encrypt { get; set; }
            [Option('d', "decrypt", Required = false, HelpText = "decrypts string using specified key.")]
            public bool Decrypt { get; set; }
            [Option('b', "bruteforce", Required = false, HelpText = "decrypts string iterating through key.")]
            public bool Bruteforce { get; set; }


            [Option('k', "key", Required = false, HelpText = "Key to use. Defaults to 123.")]
            public string Key { get; set; } = "123";
            [Option('n', "num", Required = false, HelpText = "Sequence Length. Defaults to 256.")]
            public int SequenceLen { get; set; } = 256;
            [Option('s', "string", Required = false, HelpText = "String to encrypt/decrypt. Defaults to 'Hello World'")]
            public string String { get; set; } = "Hello World";

        }


        //TODO
        /*
        static void identifyRepeats(string PRNGString)
        {
            List<string> sections = new List<string>();
            int iterations = (PRNGString.Length % 8)-1;

            for (int i = 0; i < iterations; i++)
            {
                sections.Add(PRNGString.Substring(i, 1));
            }

            var duplicateKeys = sections.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key);
            foreach (string k in duplicateKeys)
            {
                Console.WriteLine($"Found Repeating Sequence: {k}");
            }
        }*/


        
        /// <summary>
        /// Shifts ASCII characters.
        /// </summary>
        /// <param name="value">ASCII decimal value</param>
        /// <param name="shift">Amount of characters to offset value</param>
        /// <param name="backwards">Is the shift is a subtract instead of an addition</param>
        /// <returns></returns>
        static int shiftASCII(int value, int shift, bool backwards)
        {
            if (value ==32) { return 32; } //Keep spaces
            // ASCII 32 -> 126
            int ASCIIStart = 32;
            int ASCIIEnd = 126;
            int newValue;
            if (backwards)
            {
                newValue = value - shift;
            }
            else
            {
                newValue = value + shift;
            }

            if (newValue > ASCIIEnd){newValue = ASCIIStart + (newValue - ASCIIEnd);}
            if (newValue < ASCIIStart) { newValue = ASCIIStart + newValue; }

            return newValue;
        }

        /// <summary>
        /// Creates a randomly generated number string for use offseting values
        /// </summary>
        /// <param name="key">starting value for function. Must be 2 digits.</param>
        /// <param name="n"></param>
        /// <returns></returns>
        static string Create(string key, int n)
        {
            if (key.Length < 2)
            {
                Console.WriteLine("Key must be at least 2 digits. Using default key.");
                key = "123";
            }
            string PRNGString = key;
            n = n - key.Length;
            for (int i = 0; i <= n; i++)
            {
                int c1 = Convert.ToInt32(PRNGString.Substring(i, 1));
                int c2 = Convert.ToInt32(PRNGString.Substring(i+1, 1));
                int result = (c1+c2) % 10;
                PRNGString += result.ToString();
            }
            return PRNGString;
        }

        static void Encrypt(string key, string txt)
        {
            string PRNGString = Create(key,txt.Length);
            string outputString = "";
            int curStringIndex = 0;
            foreach (char c in txt)
            {
                int shift = Convert.ToInt32(PRNGString.Substring(curStringIndex, 1));
                int asciiShift = shiftASCII(Convert.ToInt32(c), shift,false);
                outputString += Convert.ToChar(asciiShift);
                curStringIndex++;
            }
            Console.WriteLine(PRNGString);
            Console.WriteLine(outputString);
        }

        static string Decrypt(string key, string txt)
        {
            string PRNGString = Create(key, txt.Length);
            string outputString = "";
            int curStringIndex = 0;
            foreach (char c in txt)
            {
                int shift = Convert.ToInt32(PRNGString.Substring(curStringIndex, 1));
                int asciiShift = shiftASCII(Convert.ToInt32(c), shift, true);
                outputString += Convert.ToChar(asciiShift);
                curStringIndex++;
            }
            return outputString;
        }

        static void BruteForce(string txt)
        {
            bool foundMatch = false;
            int key = 10;
            List<string> validWords = LoadDictionary();


            while (!foundMatch)
            {
                string result = Decrypt(key.ToString(), txt);
                string[] wordsToCheck = result.Split(' ');
                if (wordsToCheck.Length > 1)
                {
                    int matchedWords = 0;
                    foreach (string s in wordsToCheck)
                    {
                        if (validWords.Contains(s.ToLower()))
                        {
                            matchedWords++;
                            Console.WriteLine($"{matchedWords} | {key}");
                        }
                    }
                    if (matchedWords == wordsToCheck.Length)
                    {
                        foundMatch = true;
                        Console.WriteLine($"Key is {key}");
                        Console.WriteLine(result);
                    }
                } else
                {
                    Console.WriteLine("You can't bruteforce single words currently.");
                    foundMatch = true;
                }
                
                key++;
            }
        }

        static List<string> LoadDictionary()
        {
            List<string> formattedWords = new List<string>();
            string line;
            int count = 0;
            StreamReader file = new StreamReader(File.OpenRead(System.AppDomain.CurrentDomain.BaseDirectory + @"\dictionary.txt"));
            while((line = file.ReadLine()) != null)  
            {
                formattedWords.Add(line.ToLower().Trim());
                count++;
            }
            Console.WriteLine($"Loaded Dictionary with {count} words!");
            return formattedWords;
        }


        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (o.Output)
                       {
                           Console.WriteLine($"Using key: {o.Key}");
                           Console.WriteLine(Create(o.Key.ToString(), o.SequenceLen));
                       }
                       else if (o.Encrypt)
                       {
                           Console.WriteLine($"Using key: {o.Key}");
                           Console.WriteLine($"String: {o.String}");
                           Encrypt(o.Key.ToString(), o.String);
                       }
                       else if (o.Decrypt)
                       {
                           Console.WriteLine($"Using key: {o.Key}");
                           Console.WriteLine($"String: {o.String}");
                           Console.WriteLine(Decrypt(o.Key.ToString(), o.String));
                       }
                       else if (o.Bruteforce)
                       {
                           Console.WriteLine($"Starting Bruteforce on string: {o.String}");
                           BruteForce(o.String);
                       }
                   });
        }
    }
}