using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods
{
    public class DBMonkeyRecord
    {
        public DBMonkeyRecord(int monkeyID,int seqNr, int treeID)
        {
            this.monkeyID = monkeyID;
            this.seqNr = seqNr;
            this.treeID = treeID;
        }

        public DBMonkeyRecord(int recordID, int monkeyID, int seqNr, int treeID) : this(recordID, monkeyID, seqNr)
        {
            this.treeID = treeID;
        }

        public int recordID { get; set; }
        public int monkeyID { get; set; }
        public int seqNr { get; set; }
        public int treeID { get; set; }
    }
}
