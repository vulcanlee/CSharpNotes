using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BadUrl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            var fooResult = await client.GetStringAsync("http://vulcanwebapi.azurewebsites.net/api/XXX/777");
            Console.WriteLine($"{fooResult}");
            Console.WriteLine($"Press any key to Exist...{Environment.NewLine}");
            Console.ReadKey();
        }
    }
}
