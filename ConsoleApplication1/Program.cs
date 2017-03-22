using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            Console.WriteLine("Test End!");
            Console.ReadLine();
        }

        static async void Test()
        {
            await Test1();
            Console.WriteLine("Test1 End!");
        }


        static Task Test1()
        {
            Thread.Sleep(1000);
            Console.WriteLine("create task in test1");

            return Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("Test1");
            });
        }
    }
}
