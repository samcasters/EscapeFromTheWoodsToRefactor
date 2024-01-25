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
    public class Monkey
    {
        public string MonkeyID { get; private set; }
        public string Name { get; private set; }
        public Tree Tree { get; private set; }
        private Random r = new Random(1);

        public Monkey(string monkeyID,string name,Tree tree)
        {
            MonkeyID = monkeyID;
            Name = name;
            Tree = tree;
        }

        public async Task<List<Tree>> EscapeMonkey(List<Tree> trees,Map map, MongoDBwriter mongoDB,string woodId)
        {
            Dictionary<string, bool> visited = new Dictionary<string, bool>();
            trees.ForEach(x => visited.Add(x.Id, false));
            List<Tree> route = new List<Tree>() { Tree };


            do
            {
                visited[Tree.Id] = true;
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                //zoek dichtste boom die nog niet is bezocht asynchronously
                List<Task> tasks = new List<Task>();
                foreach (Tree t in trees)
                {
                    if ((!visited[t.Id]) && (!t.HasMonkey))
                    {
                        visited[t.Id]=true;
                        tasks.Add(ProcessTreeAsync(t, distanceToMonkey, Tree));
                    }
                }
                await Task.WhenAll(tasks);
                //distance to border            
                //noord oost zuid west
                double distanceToBorder = (new List<double>(){ map.Ymax - Tree.Y,
                map.Xmax - Tree.X,Tree.Y-map.Ymin,Tree.X-map.Xmin }).Min();

                if (distanceToMonkey.Count == 0 || distanceToBorder < distanceToMonkey.First().Key)
                {
                    WriteRouteToDB(route,mongoDB,woodId);
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());

            }
            while (true);
        }
        private async Task ProcessTreeAsync(Tree tree, SortedList<double, List<Tree>> distanceToMonkey, Tree monkeyTree)
        {

            double d = Math.Sqrt(Math.Pow(tree.X - monkeyTree.X, 2) + Math.Pow(tree.Y - monkeyTree.Y, 2));

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
        private async void WriteRouteToDB(List<Tree> route, MongoDBwriter mongoDB,string woodId)
        {
            List<PathModel> paths = new List<PathModel>();

            List<Task> tasks = new List<Task>();
            for (int j = 0; j < route.Count; j++)
            {
                tasks.Add(CreateAndAddPathModelAsync(paths, j, route[j], woodId));
            }
            await Task.WhenAll(tasks);
            RouteModel routeModel = new RouteModel(IDgenerator.GetNewID(),paths);
            mongoDB.WriteRouteAsync(routeModel);
        }
        private async Task CreateAndAddPathModelAsync(List<PathModel> paths, int index, Tree tree,string woodId)
        {
            paths.Add(new PathModel(IDgenerator.GetNewID(), index, new MonkeyModel(Name), new TreeModel(tree.Id,tree.X,tree.Y,woodId)));
        }
        //private async void WritePathsToDB(List<PathModel> paths, MongoDBwriter mongoDB)
        //{
        //    List<Task> tasks = new List<Task>();
        //    foreach (PathModel pathModel in paths)
        //    {
        //        tasks.Add(mongoDB.WritePathAsync(pathModel));
        //    }
        //    await Task.WhenAll(tasks);
        //}

    }
}
