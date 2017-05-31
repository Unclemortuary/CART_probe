﻿using System;
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


        public DisplayTree(Tree tr, int nm)
        {
            NumberTree = 1;
            contacts = new List<string>();
            tree = tr;
            PrintTGF(tree, nm);
        }

        private void PrintTGF(Tree tr, int nm)
        {
            try
            {
                writer = new StreamWriter("graph_" + nm + ".tgf");
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
            if (tree.rightChild != null && !tree.isTerminate)
            {
                trR = NumberTree + 1;
                NumberTree += 1;
                contacts.Add(nmb.ToString() + " " + trR.ToString() + " no");
            }
            else
            {
                trR = 0;
            }
            if (tree.leftChild != null && !tree.isTerminate)
            {
                trL = NumberTree + 1;
                NumberTree += 1;
                contacts.Add(nmb.ToString() + " " + trL.ToString() + " yes");
            }
            else
            {
                trL = 0;
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
