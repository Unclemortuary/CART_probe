using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fractions;

namespace CART_probe
{
    class Tree
    {
        public Rule rule;

        public Tree leftChild;
        public Tree rightChild;
        public bool isTerminate;

        private int[] indexesOfData;
        private Fraction g;

        public Tree(int[] data)
        {
            indexesOfData = data;
        }

        public List<Fraction> FindAlfa()
        {
            List<Fraction> alfa = new List<Fraction>();
            while (!isTerminate)
            {
                Fraction minG = new Fraction (Double.MaxValue);
                CalculateG(indexesOfData.Length, ref minG);
                Cutting(minG);
                alfa.Add(minG);
            } 
            return alfa;
        }
        
        private void CalculateG(int count, ref Fraction minG)
        {
            Fraction singleG;
            int rT, rTt, t1t;
            rT = CalculateError();
            rTt = CalculateErrorOfBranch();
            t1t = FindCountOfLeafs();
            singleG = new Fraction((rT - rTt), count);
            singleG = singleG.Divide(t1t - 1);
            if (singleG < minG)
            {
                minG = singleG;
            }                
            g = singleG;

            if (!leftChild.isTerminate)
                leftChild.CalculateG(count, ref minG);
            if (!rightChild.isTerminate)
                rightChild.CalculateG(count, ref minG);
        }

        private int CalculateError()
        {            
            int k = 0;
            var prevailClass = LearningData.Instance.FindPrevailClass(indexesOfData);
            //!!!!!!!!!!!!! надо просто count - количество элементов выбранного класса
            for (int i = 0; i < indexesOfData.Length; i++)
            {
                if (!LearningData.Instance.instances[indexesOfData[i]].instanceClass.Equals(prevailClass))
                    k++;
            }

            return k;
        }

        private int CalculateErrorOfBranch()
        {
            int totalError = 0;

            if (isTerminate)
                totalError += CalculateError();
            else
            {
                totalError += leftChild.CalculateErrorOfBranch();
                totalError += rightChild.CalculateErrorOfBranch();
            }

            return totalError;
        }

        private int FindCountOfLeafs()
        {
            var finaleCount = 0;

            if (isTerminate)
                return 1;
            else
            {
                finaleCount += leftChild.FindCountOfLeafs();
                finaleCount += rightChild.FindCountOfLeafs();
                return finaleCount;
            }
        }

        private void Cutting(Fraction minG)
        {
            if (g == minG)
            {
                isTerminate = true;
            }
            else
            {
                if (!leftChild.isTerminate)
                    leftChild.Cutting(minG);
                if (!rightChild.isTerminate)
                    rightChild.Cutting(minG);
            }
        }
    }
}
