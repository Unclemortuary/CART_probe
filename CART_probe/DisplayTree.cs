using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fractions;

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

            writer.WriteLine(nmb.ToString() + " " + "<html>" + tree.rule.rule + "<br>(" + tree.ReturnCountOfAllClasses() + ")</html>");
            if(trL != 0)
            {
                Walk(tree.leftChild, trL);
            }
            if (trR != 0)
            {
                Walk(tree.rightChild, trR);
            }
        }
        public void Graph(List<Fraction> alfaOrigin, List<Fraction> errorTreeForAlfa, List<int> CountOfLeafs)
        {
            try
            {
                StreamWriter writer = new StreamWriter("err_ot_alfa.txt");
                for(int i = 0; i < alfaOrigin.Count; i++)
                {
                    writer.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + errorTreeForAlfa[i].ToDouble().ToString());
                }
                writer.Close();
                writer = new StreamWriter("T_ot_alfa.txt");
                StreamWriter writer2 = new StreamWriter("T_normir_ot_alfa.txt");
                StreamWriter writerDifErr = new StreamWriter("dif_err.txt");
                StreamWriter writerDifList = new StreamWriter("dif_list.txt");
                for (int i = 0; i < alfaOrigin.Count; i++)
                {
                    writer.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + CountOfLeafs[i].ToString());
                    double x = (double)CountOfLeafs[i] / (double)CountOfLeafs[0];
                    
                    writer2.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + x.ToString());
                    if (i == 0)
                    {
                        writerDifErr.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + "0");
                        writerDifList.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + "0");
                    }
                    else
                    {
                        writerDifErr.WriteLine(alfaOrigin[i - 1].ToDouble().ToString() + " " + ((errorTreeForAlfa[i] - errorTreeForAlfa[i - 1]) / (alfaOrigin[i] - alfaOrigin[i - 1])).ToDouble().ToString());
                        writerDifErr.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + ((errorTreeForAlfa[i] - errorTreeForAlfa[i - 1]) / (alfaOrigin[i] - alfaOrigin[i - 1])).ToDouble().ToString());
                        double x_pred = (double)CountOfLeafs[i - 1] / (double)CountOfLeafs[0];
                        writerDifList.WriteLine(alfaOrigin[i - 1].ToDouble().ToString() + " " + ((x_pred-x) / (alfaOrigin[i] - alfaOrigin[i - 1]).ToDouble()).ToString());
                        writerDifList.WriteLine(alfaOrigin[i].ToDouble().ToString() + " " + ((x_pred-x) / (alfaOrigin[i] - alfaOrigin[i - 1]).ToDouble()).ToString());
                    }

                }
                writer.Close();
                writer2.Close();
                writerDifErr.Close();
                writerDifList.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception was occure : {0}", e);
            }
            Console.WriteLine("Done!");
        }
    }


}
