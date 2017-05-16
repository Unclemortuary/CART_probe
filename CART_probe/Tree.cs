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
        private double g;

        public Tree(int[] data)
        {
            indexesOfData = data;
        }

        public Tree Cut(string nameOfClass)
        {
            bool stop = false;
            do
            {
                double minG = Double.MaxValue;
                CalculateG(this, indexesOfData.Length, ref minG);
                stop = Cutting(minG, g);
            } while (!stop);
            return this;
        }
        
        private void CalculateG(Tree tree, int count, ref double minG)
        {
            double singleG = 0;
            double rT = 0, rTt = 0, t1t = 0;
            rT = CalculateError(tree, count);
            rTt = CalculateErrorOfBranch(tree, count);
            t1t = FindCountOfLeafs(tree);
            singleG = (rT - rTt) / (t1t - 1);
            if(!tree.isTerminate)
            {
                if (singleG > minG)
                    minG = singleG;
            }
            tree.g = singleG;

            if (tree.leftChild != null)
                CalculateG(tree.leftChild, count, ref minG);
            if (tree.rightChild != null)
                CalculateG(tree.rightChild, count, ref minG);
        }

        private double CalculateError(Tree tree, int count)
        {
            
            int k = 0;
            var prevailClass = LearningData.Instance.FindPrevailClass(indexesOfData);
            for (int i = 0; i < indexesOfData.Length; i++)
            {
                if (!LearningData.Instance.instances[indexesOfData[i]].instanceClass.Equals(prevailClass))
                    k++;
            }

            return (double) k/count;
        }

        private double CalculateErrorOfBranch(Tree tree, int count)
        {
            double totalError = 0;

            if (tree.isTerminate)
                totalError += CalculateError(tree, count);
            else
            {
                totalError += CalculateErrorOfBranch(tree.leftChild, count);
                totalError += CalculateErrorOfBranch(tree.rightChild, count);
            }

            return totalError;
        }

        private int FindCountOfLeafs(Tree tree)
        {
            var finaleCount = 0;

            if (isTerminate)
                return 1;
            else
            {
                finaleCount += FindCountOfLeafs(leftChild);
                finaleCount += FindCountOfLeafs(rightChild);
                return finaleCount;
            }
        }

        private bool Cutting(double minG, double rootG)
        {
            if (g == rootG)
                return true;
            else
            {
                if (g == minG)
                {
                    leftChild = null;
                    rightChild = null;
                    isTerminate = true;
                }
                else
                {
                    if (leftChild != null && !leftChild.isTerminate)
                        leftChild.Cutting(minG, rootG);
                    if (rightChild != null && !rightChild.isTerminate)
                        rightChild.Cutting(minG, rootG);
                }
                return false;
            }
        }
    }
}
