using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Database.Model;
using EscapeFromTheWoods.Database.exceptions;

namespace EscapeFromTheWoods
{
    public class Wood
    {

        private const int drawingFactor = 8;
        private string path;
        // private DBwriter db;
        private Random r = new Random(1);
        public string woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;

        private MongoDBwriter mongoDB;
        private WoodModel WoodModel;

        public Wood(string woodID, List<Tree> trees, Map map, string path, MongoDBwriter mongoDB)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            //this.db = db;
            this.mongoDB = mongoDB;
            WoodModel = new WoodModel(woodID,map.xmax, map.ymax);
        }

        public async Task PlaceMonkey(string monkeyName, string monkeyID)
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

        public async void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);

            List<Task> ellipseTasks = new List<Task>();
            foreach (Tree t in trees)
            {
                ellipseTasks.Add(DrawEllipseAsync(g, p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor));
            }
            await Task.WhenAll(ellipseTasks);

            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);

                List<Task> lineTasks = new List<Task>();
                for (int i = 1; i < route.Count; i++)
                {
                    lineTasks.Add(DrawLineAsync(g, pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta));
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                await Task.WhenAll(lineTasks);

                colorN++;
            }
            bm.Save(Path.Combine(path, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
        }
        private async Task DrawEllipseAsync(Graphics graphics,Pen pen,int x,int y,int width,int height)
        {
            graphics.DrawEllipse(pen, x, y, width, height);
        }
        private async Task DrawLineAsync(Graphics graphics, Pen pen, int x1, int y1, int x2, int y2)
        {
            graphics.DrawLine(pen, x1, y1, x2, y2);
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

        public async Task Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(await EscapeMonkey(m));
            }
            WriteEscaperoutesToBitmap(routes);
        }
        public async Task<List<Tree>> EscapeMonkey(Monkey monkey)
        {
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
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
                await Task.WhenAll(tasks);
                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>(){ map.ymax - monkey.tree.y,
                map.xmax - monkey.tree.x,monkey.tree.y-map.ymin,monkey.tree.x-map.xmin }).Min();

                if (distanceToMonkey.Count == 0 || distanceToBorder < distanceToMonkey.First().Key)
                {
                    WriteRouteToDB(monkey, route);
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            }
            while (true);
        }
        private async void WriteRouteToDB(Monkey monkey, List<Tree> route)
        {
            List<PathModel> paths = new List<PathModel>();
            
            List<Task> tasks = new List<Task>();
            for (int j = 0; j < route.Count; j++)
            {
                tasks.Add(CreateAndAddPathModelAsync(paths, j, monkey, route[j]));
            }
            await Task.WhenAll(tasks);
            WritePathsToDB(paths);
        }
        private async Task CreateAndAddPathModelAsync(List<PathModel> paths, int index, Monkey monkey, Tree tree)
        {
            paths.Add(new PathModel(IDgenerator.GetNewID(),index, monkey.monkeyID, tree.treeID));
        }
        private async void WritePathsToDB(List<PathModel> paths)
        {
            List<Task> tasks = new List<Task>();
            foreach (PathModel pathModel in paths)
            {
                tasks.Add(mongoDB.WritePathAsync(pathModel));
            }
            await Task.WhenAll(tasks);
        }

        public async Task WriteWoodToDB()
        {
            try
            {
                await Console.Out.WriteLineAsync("write wood ping");
                List<Task> tasks = new List<Task>();
                List<TreeModel> treeModels = new List<TreeModel>();
                foreach (Tree t in trees)
                {
                    tasks.Add(TreeToTreeModels(treeModels, t));
                }
                mongoDB.WriteTrees(treeModels);
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw new ObjectException("WriteWoodToDB",ex);
            }
        }
        private async Task TreeToTreeModels(List<TreeModel> treeModels, Tree tree)
        {
            try
            {
                TreeModel treeModel = new TreeModel(tree.treeID,tree.x, tree.y, woodID);
                treeModels.Add(treeModel);
            }
            catch (Exception ex)
            {
                throw new ObjectException($"TreeToTreeModels  id:{tree.treeID}", ex);
            }
        }
    }
}
