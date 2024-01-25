using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Database.exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    public class WoodBuilder
    {
        public async Task<Wood> GetWood(int size,Map map,string path,MongoDBwriter db)
        {
            try
            {
                Random r = new Random(100);
                List<Tree> trees = new List<Tree>();
                int n = 0;
                List<Task> tasks = new List<Task>();
                while (n < size)
                {
                    tasks.Add(AddTree(trees, map, r));
                    n++;
                }
                await Task.WhenAll(tasks);

                Wood w = new Wood(IDgenerator.GetNewID(), trees, map, db);
                return w;
            }
            catch (Exception ex)
            {
                throw new ObjectException("GetWood", ex);
            }
        }
        private async Task AddTree(List<Tree> trees, Map map,Random r)
        {
            try
            {
                Tree tree = new Tree(IDgenerator.GetNewID(), r.Next(map.Xmin, map.Xmax), r.Next(map.Ymin, map.Ymax));
                if (!trees.Contains(tree) & !string.IsNullOrEmpty(tree.Id))
                {
                    trees.Add(tree);
                }
            }
            catch (Exception ex)
            {
                throw new ObjectException("AddTree",ex);
            }
        }
    }
}
