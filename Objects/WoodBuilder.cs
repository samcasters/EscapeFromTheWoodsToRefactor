using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    public static class WoodBuilder
    {
        public static Wood GetWood(int size,Map map,string path,DBwriter db)
        {
            
            Random r = new Random(100);
            List<Tree> trees = new List<Tree>();
            int n = 0;
            while (n < size)
            {
                    Tree tree = new Tree(IDgenerator.GetTreeID(), r.Next(map.xmin, map.xmax), r.Next(map.ymin, map.ymax));
                    if (!trees.Contains(tree))
                    {
                        trees.Add(tree);
                    }
                n++;
            }
            

            Wood w = new Wood(IDgenerator.GetWoodID(),trees,map,path,db);
            return w;
        }
    }
}
