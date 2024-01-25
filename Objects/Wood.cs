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
using System.Xml.Linq;

namespace EscapeFromTheWoods
{
    public class Wood
    {


        
        // private DBwriter db;
        private Random r = new Random(1);
        public string Id { get; set; }
        public List<Tree> Trees { get; set; }
        public List<Monkey> Monkeys { get; private set; }
        private Map map;

        private MongoDBwriter MongoDB;
        private WoodModel WoodModel;

        public Wood(string woodID, List<Tree> trees, Map map, MongoDBwriter mongoDB)
        {
            Id = woodID;
            Trees = trees;
            Monkeys = new List<Monkey>();
            this.map = map;
            //this.db = db;
            MongoDB = mongoDB;
            List<TreeModel> Treemodels = new List<TreeModel>();
            WoodModel = new WoodModel(woodID,map.Xmax, map.Ymax,FillTrees(trees));
            
        }

        public List<TreeModel> FillTrees(List<Tree> trees) 
        {
            List<TreeModel> treeModels = new List<TreeModel>();
            foreach (Tree tree in trees) 
            {
                TreeModel treeModel = new TreeModel(tree.Id,tree.X,tree.Y, Id);
                treeModels.Add(treeModel);
            }
            return treeModels;
        }
        public async Task PlaceMonkey(string monkeyName, string monkeyID)
        {
            MonkeyModel monkeyModel = new MonkeyModel(monkeyID, monkeyName);
            int treeNr;
            do
            {
                treeNr = r.Next(0, Trees.Count - 1);
            }
            while (Trees[treeNr].HasMonkey);
            Monkey m = new Monkey(monkeyID, monkeyName, Trees[treeNr]);
            Monkeys.Add(m);
            Trees[treeNr].HasMonkey = true;
        }
        public async Task Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in Monkeys)
            {
                routes.Add(await m.EscapeMonkey(Trees,map,MongoDB,Id));
            }
            map.WriteEscaperoutesToBitmap(routes,Trees,Id);
        }
        public async Task WriteWoodToDB()
        {
            
            try
            {
                await Console.Out.WriteLineAsync();
                List<Task> tasks = new List<Task>();
                List<TreeModel> treeModels = new List<TreeModel>();
                foreach (Tree t in Trees)
                {
                    tasks.Add(t.TreeToTreeModels(treeModels,WoodModel));
                }
                WoodModel woodModel = new WoodModel(IDgenerator.GetNewID(),map.Xmax, map.Ymax, treeModels);
                MongoDB.WriteWood(woodModel);
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                throw new ObjectException("WriteWoodToDB",ex);
            }
        }
    }
}
