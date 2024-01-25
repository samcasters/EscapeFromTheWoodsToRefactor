using EscapeFromTheWoods.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;
using static System.Formats.Asn1.AsnWriter;

namespace EscapeFromTheWoods
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            //string connectionString = @"Data Source=DESKTOP-PJRLO8E\SQLEXPRESS;Initial Catalog=monkeys;Integrated Security=True"; //sql
            string connectionString = ConfigurationManager.ConnectionStrings["MongoDbBelgica"].ConnectionString;//mongodb
            MongoDBwriter db = new MongoDBwriter(connectionString);
            WoodBuilder woodBuilder = new WoodBuilder();
            string path = @"C:\Users\samca\Documents\programerenSpecialisatie\OpdrachtEscapeTheWoods\monkey";


            List<Task<Wood>> tasksGetWood = new List<Task<Wood>>();
            Map m1 = new Map(0, 500, 0, 500,path);
            tasksGetWood.Add(woodBuilder.GetWood(500, m1, path, db));

            Map m2 = new Map(0, 200, 0, 400, path);
            tasksGetWood.Add(woodBuilder.GetWood(2500, m2, path, db));

            Map m3 = new Map(0, 400, 0, 400, path);
            tasksGetWood.Add(woodBuilder.GetWood(2000, m3, path, db));
            Wood[] woods = await Task.WhenAll(tasksGetWood);
            Wood w1 = woods[0];
            Wood w2 = woods[1];
            Wood w3 = woods[2];


            Console.WriteLine("1: place monkeys");
            List<Task> tasksWrite1 = new List<Task>
            {
            w1.PlaceMonkey("Alice", IDgenerator.GetNewID()),
            w1.PlaceMonkey("Janice", IDgenerator.GetNewID()),
            w1.PlaceMonkey("Toby", IDgenerator.GetNewID()),
            w1.PlaceMonkey("Mindy", IDgenerator.GetNewID()),
            w1.PlaceMonkey("Jos", IDgenerator.GetNewID()),

            w2.PlaceMonkey("Tom", IDgenerator.GetNewID()),
            w2.PlaceMonkey("Jerry", IDgenerator.GetNewID()),
            w2.PlaceMonkey("Tiffany", IDgenerator.GetNewID()),
            w2.PlaceMonkey("Mozes", IDgenerator.GetNewID()),
            w2.PlaceMonkey("Jebus", IDgenerator.GetNewID()),

            w3.PlaceMonkey("Kelly", IDgenerator.GetNewID()),
            w3.PlaceMonkey("Kenji", IDgenerator.GetNewID()),
            w3.PlaceMonkey("Kobe", IDgenerator.GetNewID()),
            w3.PlaceMonkey("Kendra", IDgenerator.GetNewID()),
            };
            await Task.WhenAll(tasksWrite1);


            Console.WriteLine("2: write wood to database");
            List<Task> tasksWrite2 = new List<Task>
            {
                w1.WriteWoodToDB(),
                w2.WriteWoodToDB(),
                w3.WriteWoodToDB(),
                w1.Escape(),
                w2.Escape(),
                w3.Escape()
            };
            await Task.WhenAll(tasksWrite2);


            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.WriteLine("end");
        }
    }
}