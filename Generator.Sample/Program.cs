using System;
using System.Collections.Generic;

namespace Generator.Sample
{
    class Program
    {
        public class Args
        {
            public string Name { get; set; }
        }

        public static IEnumerable<IYield> DoStuff(Args args)
        {
            yield return Gen.Yield("hello", () => args.Name);
            yield return Gen.Yield(3);
        } 

        static void Main()
        {
            var args = new Args();
            var generator = Gen.Create(DoStuff(args));

            Console.WriteLine(generator.Send<string, string>("Orange"));
            Console.WriteLine(args.Name);
            Console.WriteLine(generator.Next<int>());

        }
    }
}
