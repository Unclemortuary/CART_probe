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

        //private int[] indexesOfData;
        private List<int> indexesOfData;
        private Fraction g;

        public Tree(int[] data)
        {
            indexesOfData = new List<int>(data);
        }

        public List<Fraction> FindAlfa(ref List<Fraction> error, ref List<int> cntList, ref List<int> depth)
        {
            List<Fraction> alfa = new List<Fraction>();
            alfa.Add(0);
            error.Add(new Fraction(CalculateErrorOfBranch(), indexesOfData.Count));
            cntList.Add(FindCountOfLeafs());
            depth.Add(Depth());
            while (!isTerminate)
            {
                Fraction minG = new Fraction(Double.MaxValue);
                CalculateG(indexesOfData.Count, ref minG);
                Cutting(minG);
                error.Add(new Fraction(CalculateErrorOfBranch(), indexesOfData.Count));
                cntList.Add(FindCountOfLeafs());
                depth.Add(Depth());
                alfa.Add(minG);
            }
            return alfa;
        }
        public List<Fraction> FindErrForBeta(int[] indexes, List<Fraction> beta)
        {
            if (indexes.Last() == 0)
            {
                Array.Resize(ref indexes, indexes.Length - 1);
            }
            List<Fraction> alfa = new List<Fraction>();
            List<Fraction> errAlfa = new List<Fraction>();
            while (!isTerminate)
            {
                Fraction minG = new Fraction(Double.MaxValue);
                CalculateG(indexesOfData.Count, ref minG);
                if(alfa.Count == 0 && minG > 0)
                {
                    errAlfa.Add(new Fraction(0));
                }
                Cutting(minG);
                alfa.Add(minG);
                //!!!!!можно считать ошибку только для нужных нам деревьев
                var rt = ErrorGi(indexes);
                errAlfa.Add(new Fraction(rt, indexes.Length));
            }
            List<Fraction> errForBeta = new List<Fraction>();
            //int[] errForBeta = new int[beta.Count];
            int cnt = 0;
            for (int i = 0; i < beta.Count; i++)
            {
                if (beta[i] >= alfa.Last())
                {
                    errForBeta.Add(errAlfa.Last());
                }
                else
                {
                    while (!(beta[i] >= alfa[cnt] && beta[i] < alfa[cnt + 1]))
                    {
                        cnt++;
                    }
                    errForBeta.Add(errAlfa[cnt]);
                }
            }
            return errForBeta;
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
            var prevailClass = LearningData.Instance.FindPrevailClass(indexesOfData.ToArray());
            //!!!!!!!!!!!!! надо просто count - количество элементов выбранного класса
            for (int i = 0; i < indexesOfData.Count; i++)
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

        public void OpenTreeAndFill(List<int> indexes)
        {
            indexesOfData = indexes;
            if (leftChild == null && rightChild == null)
            {
                isTerminate = true;                
            }
            else
            {
                isTerminate = false;
                //разбиение примеров по правилу
                List<int> indexesOfLeftInstances = new List<int>();
                List<int> indexesOfRightInstances = new List<int>();
                for (int i = 0, ind = Array.IndexOf(LearningData.Instance.atributes, rule.b[0].atrName); i < indexes.Count(); i++)
                {
                    for (int j = 0; j < rule.b.Count; j++)
                    {
                        if (LearningData.Instance.instances[indexes[i]].atributes[ind].Equals(rule.b[j]))
                            indexesOfLeftInstances.Add(indexes[i]);
                    }
                    //!!!!!намного оптимальнее поставить флаг
                    if (!indexesOfLeftInstances.Contains(indexes[i]))
                        indexesOfRightInstances.Add(indexes[i]);
                }
                if (leftChild != null)
                {
                    leftChild.OpenTreeAndFill(indexesOfLeftInstances);
                }
                if(rightChild != null)
                {
                    rightChild.OpenTreeAndFill(indexesOfRightInstances);
                }
            }
        }
        //дерево обрезано и учебные примеры раскинуты. и считается ошибка для тестовой выборки.
        private int ErrorGi(int[] indexes)
        {
            int k = 0;
            if (isTerminate)
            {                
                var prevailClass = LearningData.Instance.FindPrevailClass(indexesOfData.ToArray());
                for (int i = 0; i < indexes.Length; i++)
                {
                    if (!LearningData.Instance.instances[indexes[i]].instanceClass.Equals(prevailClass))
                        k++;
                }                
            }
            else
            {
                List<int> indexesOfLeftInstances = new List<int>();
                List<int> indexesOfRightInstances = new List<int>();
                for (int i = 0, ind = Array.IndexOf(LearningData.Instance.atributes, rule.b[0].atrName); i < indexes.Count(); i++)
                {
                    for (int j = 0; j < rule.b.Count; j++)
                    {
                        if (LearningData.Instance.instances[indexes[i]].atributes[ind].Equals(rule.b[j]))
                            indexesOfLeftInstances.Add(indexes[i]);
                    }
                    //!!!!!намного оптимальнее поставить флаг
                    if (!indexesOfLeftInstances.Contains(indexes[i]))
                        indexesOfRightInstances.Add(indexes[i]);
                }
                k += leftChild.ErrorGi(indexesOfLeftInstances.ToArray());
                k += rightChild.ErrorGi(indexesOfRightInstances.ToArray());
            }
            return k;
        }

        public string ReturnCountOfAllClasses()
        {
            return LearningData.Instance.FindCountOfAllClasses(indexesOfData.ToArray());
        }

        //обрезка для выведения дерева
        public void CutForAlfa(Fraction alfa)
        {
            int[] indexes = new int[LearningData.Instance.instances.Count];
            for (int q = 0; q < indexes.Length; q++)
                indexes[q] = q;

            OpenTreeAndFill(indexes.ToList());
            var min = new Fraction();
            while (!isTerminate && min != alfa)
            {
                Fraction minG = new Fraction(Double.MaxValue);
                CalculateG(indexesOfData.Count, ref minG);
                Cutting(minG);
                min = minG;
            }
        }
        public int Depth()
        {
            if (isTerminate)
                return 1;
            else
            {
                var l = leftChild.Depth();
                var r = rightChild.Depth();
                if (l > r)
                    return 1 + l;
                else
                    return 1 + r;
            }
        }
        //public List<Fraction> FindErrorForALfaS(int[] indexes, List<Fraction> alfas)
        //{
        //    OpenTreeAndFill(indexes.ToList());
        //    List<Fraction> errorTreeForAlfa = new List<Fraction>();
        //    var err = FindErrForBeta(indexes, alfas);
        //    return errorTreeForAlfa;
        //}

    }
}
