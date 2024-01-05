using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    public class WoodBuilder
    {
        public async Task<Wood> GetWood(int size,Map map,string path,DBwriter db)
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

            Wood w = new Wood(IDgenerator.GetWoodID(),trees,map,path,db);
            return w;
        }
        private async Task AddTree(List<Tree> trees, Map map,Random r)
        {
            Tree tree = new Tree(IDgenerator.GetTreeID(), r.Next(map.xmin, map.xmax), r.Next(map.ymin, map.ymax));
            if (!trees.Contains(tree))
            {
                trees.Add(tree);
            }
        }
    }
}
