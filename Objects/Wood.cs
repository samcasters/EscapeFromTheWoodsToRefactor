using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    public class Wood
    {

        private const int drawingFactor = 8;
        private string path;
        private DBwriter db;
        private Random r = new Random(1);
        public int woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;
        public Wood(int woodID, List<Tree> trees, Map map, string path, DBwriter db)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            this.db = db;
        }
        public async Task PlaceMonkey(string monkeyName, int monkeyID)
        {
            int treeNr;
            do
            {
                treeNr = r.Next(0, trees.Count - 1);
            }
            while (trees[treeNr].hasMonkey);
            Monkey m = new Monkey(monkeyID, monkeyName, trees[treeNr]);
            monkeys.Add(m);
            trees[treeNr].hasMonkey = true;
        }
        public async Task Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
               routes.Add(await EscapeMonkey(m));
            }                
            WriteEscaperoutesToBitmap(routes);           
        }
        private async Task WriteRouteToDB(Monkey monkey, List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");

            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            List<Task> tasks = new List<Task>();

            for (int j = 0; j < route.Count; j++)
            {
                tasks.Add(CreateDBMonkeyRecordAsync(records, monkey, j, route[j]));
            }

            await Task.WhenAll(tasks);

            db.WriteMonkeyRecords(records);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }

        private async Task CreateDBMonkeyRecordAsync(List<DBMonkeyRecord> records, Monkey monkey, int index, Tree tree)
        {
            records.Add(new DBMonkeyRecord(monkey.monkeyID, monkey.name, woodID, index, tree.treeID, tree.x, tree.y));
        }
        public async void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);
            List<Task> tasks = new List<Task>();
            foreach (Tree t in trees)
            {
                g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
            }
            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                for (int i = 1; i < route.Count; i++)
                {
                    g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                colorN++;
            }
            bm.Save(Path.Combine(path, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }
        public async Task WriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}: write db wood {woodID} start");

            List<DBWoodRecord> records = new List<DBWoodRecord>();
            
            foreach (Tree t in trees)
            {
                CreateDBWoodRecordAsync(records, t);
            }

            db.WriteWoodRecords(records);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}: write db wood {woodID} end");
        }

        private void CreateDBWoodRecordAsync(List<DBWoodRecord> records, Tree tree)
        {
            records.Add(new DBWoodRecord(woodID, tree.treeID, tree.x, tree.y));
        }
        public async Task<List<Tree>> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");
            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            trees.ForEach(x => visited.Add(x.treeID, false));
            List<Tree> route = new List<Tree>() { monkey.tree };

            do
            {
                visited[monkey.tree.treeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                //zoek dichtste boom die nog niet is bezocht asynchronously
                List<Task> tasks = new List<Task>();
                foreach (Tree t in trees)
                {
                    if ((!visited[t.treeID]) && (!t.hasMonkey))
                    {
                        tasks.Add(ProcessTreeAsync(t, distanceToMonkey, monkey.tree));
                    }
                }

                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>(){ map.ymax - monkey.tree.y,
                map.xmax - monkey.tree.x,monkey.tree.y-map.ymin,monkey.tree.x-map.xmin }).Min();

                if (distanceToMonkey.Count == 0 || distanceToBorder < distanceToMonkey.First().Key)
                {
                    tasks.Add(WriteRouteToDBAsync(monkey, route));
                    await Task.WhenAll(tasks);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();

                await Task.WhenAll(tasks);
            }
            while (true);
        }

        private async Task ProcessTreeAsync(Tree tree, SortedList<double, List<Tree>> distanceToMonkey, Tree monkeyTree)
        {

            double d = Math.Sqrt(Math.Pow(tree.x - monkeyTree.x, 2) + Math.Pow(tree.y - monkeyTree.y, 2));

            // Ensure the distanceToMonkey list is initialized before adding elements
            if (distanceToMonkey.TryGetValue(d, out var treeList))
            {
                treeList.Add(tree);
            }
            else
            {
                distanceToMonkey.Add(d, new List<Tree>() { tree });
            }
        }

        private async Task WriteRouteToDBAsync(Monkey monkey, List<Tree> route)
        {
            Console.WriteLine($"{woodID}: Writing route to DB for {woodID},{monkey.name}");
            WriteRouteToDB(monkey, route);
        }
    }
}
