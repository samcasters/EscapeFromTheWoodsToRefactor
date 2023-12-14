using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EscapeFromTheWoods
{
    public class DoStuffAsync
    {
        private void simulateCPUOperation()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Math.Sqrt(Math.Cos(i) + Math.Sin(i));
            }
        }
        private void simulateIOOperation()
        {
            Thread.Sleep(500);
        }
        public async Task process(int n, char token,bool IO,bool CPU)
        {
            for (int i = 0; i < n; i++)
            {
                if (CPU) simulateCPUOperation();
                if (IO) simulateIOOperation();
                Console.WriteLine(token);
            }
        }
    }
}
