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
    public class Tree
    {
        public Tree(string treeID, int x, int y)
        {
            Id = treeID;
            X = x;
            Y = y;
            HasMonkey = false;
        }
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool HasMonkey { get; set; }

        public async Task TreeToTreeModels(List<TreeModel> treeModels, WoodModel wood)
        {
            try
            {
                TreeModel treeModel = new TreeModel(Id, X, Y, wood.Id);
                treeModels.Add(treeModel);
            }
            catch (Exception ex)
            {
                throw new ObjectException($"TreeToTreeModels  id:{Id}", ex);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Tree tree &&
                   X == tree.X &&
                   Y == tree.Y;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        public override string ToString()
        {
            return $"{Id},{X},{Y}";
        }
    }
}
