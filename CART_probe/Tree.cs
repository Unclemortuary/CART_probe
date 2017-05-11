using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CART_probe
{
    class Tree
    {
        public Rule rule;

        public Tree leftChild;
        public Tree rightChild;
        public bool isTerminate;

        private int[] indexesOfData;

        public Tree(int[] data)
        {
            indexesOfData = data;
        }
    }
}
