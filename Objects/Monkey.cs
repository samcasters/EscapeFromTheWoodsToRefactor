using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods
{
    public class Monkey
    {
        public string monkeyID { get; set; }
        public string name { get; set; }
        public Tree tree { get; set; }

        public Monkey(string monkeyID,string name,Tree tree)
        {
            this.monkeyID = monkeyID;
            this.tree = tree;
            this.name = name;
        }
    }
}
