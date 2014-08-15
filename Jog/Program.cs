using System.Linq;
using System.Threading;

namespace Jog
{
    using System;
    using Nancy.Hosting.Self;

    class Program
    {
        static void Main(string[] args)
        {
            var uri =
                new Uri("http://localhost:3579");

            var host = new NancyHost(uri);
            host.Start();

            Console.WriteLine("Your application is running on " + uri);

            if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                Console.ReadKey();
            }

            host.Stop();  // stop hosting
        }
    }
}
