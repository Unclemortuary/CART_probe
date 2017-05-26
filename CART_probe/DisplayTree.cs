using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CART_probe
{
    class DisplayTree
    {
        StreamWriter writer;
        int NumberTree;
        private List<string> contacts;
        private Tree tree;


        public DisplayTree(Tree tr)
        {
            NumberTree = 1;
            contacts = new List<string>();
            tree = tr;
            PrintTGF(tree);
        }

        private void PrintTGF(Tree tr)
        {
            try
            {
                writer = new StreamWriter("graph.tgf");
                Walk(tr, NumberTree);
                writer.WriteLine("#");
                for (int i = 0; i < contacts.Count; i++)
                {
                    writer.WriteLine(contacts[i]);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception was occure : {0}", e);
            }
            Console.WriteLine("Done!");
        }
        private void Walk(Tree tree, int nmb)
        {
            int trL, trR;
            if (tree.leftChild != null)
            {
                trL = NumberTree + 1;
                NumberTree += 1;
                contacts.Add(nmb.ToString() + " " + trL.ToString());
            }
            else
            {
                trL = 0;
            }
            if (tree.rightChild != null)
            {
                trR = NumberTree + 1;
                NumberTree += 1;
                contacts.Add(nmb.ToString() + " " + trR.ToString());
            }
            else
            {
                trR = 0;
            }
            writer.WriteLine(nmb.ToString() + " " + tree.rule.rule);
            if(trL != 0)
            {
                Walk(tree.leftChild, trL);
            }
            if (trR != 0)
            {
                Walk(tree.rightChild, trR);
            }
        }
    }


}
