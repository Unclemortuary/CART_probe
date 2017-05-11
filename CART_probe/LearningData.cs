using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CART_probe
{
    class LearningData
    {
        public static List<Rule> potentialRules; // Набор потенциальных рабиений

        private List<int> usedRules = new List<int>();
        private string[] atributes;
        private string[] classes;

        private List<Instance> instances;

        public LearningData(string path, string[] clss, string[] atrs)
        {
            instances = new List<Instance>();
            potentialRules = new List<Rule>();
            atributes = atrs;
            classes = clss;
            InitializeData(path);
            MakePotentialRules(instances);
        }

        public void InitializeData(string p)
        {
            try
            {
                StreamReader reader = new StreamReader(p);
                string line = reader.ReadLine();
                char[] arr = line.ToCharArray();
                int atributeCount = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == ',')
                        atributeCount++;
                }
                do
                {
                    var instance = new Instance(atributeCount);
                    string atribute = null;
                    char singleSymbol = '\0';
                    for (int i = 0, j = 0; i < arr.Length; i++)
                    {
                        singleSymbol = arr[i];
                        if (i == arr.Length - 1) // Последний атрибут в строке - класс
                        {
                            atribute += singleSymbol;
                            atribute.Replace(" ", "");
                            instance.instanceClass = atribute;
                            atribute = null;
                        }
                        else
                        {
                            if (singleSymbol == ',')
                            {
                                atribute.Replace(" ", "");
                                instance.atributes[j].textAtr = atribute;
                                instance.atributes[j].atrName = atributes[j];
                                j++;
                                atribute = null;
                            }
                            else
                                atribute += singleSymbol;
                        }
                    }
                    instances.Add(instance);
                    line = reader.ReadLine();
                    Array.Clear(arr, 0, arr.Length);
                    arr = line.ToCharArray();
                } while (!reader.EndOfStream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception was occure : {0}", e);
            }
            Console.WriteLine("Done!");
        }

        public void MakePotentialRules(List<Instance> list)
        {
            // Проверяем на числовые атрибуты, если имеются - сортируем и добавляем разбиения
            for (int i = 0; i < list[0].atributes.Count(); i++)
            {
                if (list[0].atributes[i].textAtr == null)
                {
                    ColumnSorting(i);
                    MakeRuleForNumericAtr();
                }
            }
            // Добавляем разбиения для строковых атрибутов
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list[i].atributes.Count(); j++)
                {
                    var someRule = new Rule(list[i].atributes[j]);
                    if (!potentialRules.Contains(someRule))
                        potentialRules.Add(someRule);
                }
            }
        }

        public int GetCountOfRules()
        {
            return potentialRules.Count;
        }

        public void ColumnSorting(int index)
        {

        }

        public void MakeRuleForNumericAtr()
        {

        }

        public Tree CART(int[] indexes /*, List<int> usedRules*/) // indexes - массив индексов примеров в instances для данного узла
        {
            double kL = 0f, kR = 0f;
            double[] kjL = new double[classes.Count()];
            double[] kjR = new double[classes.Count()];
            double[] giniTable = new double[potentialRules.Count];
            int indexOfCurrentAtribute = 0;
            List<int> indexesOfLeftInstances = new List<int>();
            List<int> indexesOfRightInstances = new List<int>();

            if (indexes == null)
            {
                indexes = new int[instances.Count];
                for (int q = 0; q < indexes.Count(); q++)
                    indexes[q] = q;
            }

            Tree tree = new Tree(indexes);
            if (IsTerminate(indexes) || usedRules.Count == potentialRules.Count) // Проверяем лист или нет
            {
                tree.isTerminate = true;
                var nameOfClass = instances[indexes[0]].instanceClass;
                // Если да, то создаем правило, которое содержит только название класса
                tree.rule = new Rule(new Atribute(null, nameOfClass, 0));
                // И выходим из функции
                return tree;
            }
            else
            {
                for (int i = 0; i < potentialRules.Count; i++)
                {
                    if (usedRules.Contains(i))
                        continue;

                    for (int k = 0; k < atributes.Count(); k++)
                    {
                        if (potentialRules[i].b.atrName.Equals(atributes[k]))
                            indexOfCurrentAtribute = k;
                    }

                    for (int j = 0; j < indexes.Count(); j++)
                    {
                        if (potentialRules[i].b.textAtr == null) // Если параметр непрерывный, то сравниваем (НАДО ДОПИСАТЬ!)
                        {

                        }
                        else
                        {
                            if (instances[indexes[j]].atributes[indexOfCurrentAtribute].textAtr.Equals(potentialRules[i].b.textAtr)) // Проверям примеры в левом потомке
                            {
                                kL++;
                                var indexOfClass = Array.IndexOf(classes, instances[indexes[j]].instanceClass); // Берем индекс класса соответсвтующего примера
                                kjL[indexOfClass]++; // И прибавляем 1 в ячейке массива с такой же размерностью с соответсвующим индексом
                                // Таким образом подсчитываем кол-во примеров каждого класса в левом потомке
                                //indexesOfLeftInstances.Add(indexes[j]); // Заносим индекс примера в массив индексов для левого потомка
                            }
                            else
                            {
                                var indexOfClass = Array.IndexOf(classes, instances[indexes[j]].instanceClass);
                                kjR[indexOfClass]++;
                                //indexesOfRightInstances.Add(indexes[j]); // Заносим индекс примера в массив индексов для правого потомка
                            }
                        }
                    }
                    kR = instances.Count - kL;
                    giniTable[i] = CalculateGini(kL, kR, kjL, kjR);
                    kL = 0; kR = 0;
                    Array.Clear(kjL, 0, kjL.Length);
                    Array.Clear(kjR, 0, kjR.Length);
                }
                var indexOfRule = FindMax(giniTable); // Берем индекс самого лучшего правила из табицы ГИНИ
                usedRules.Add(indexOfRule);
                tree.rule = potentialRules[indexOfRule]; // и заносим правило с таким индексом в дерево
                for (int i = 0, ind = Array.IndexOf(atributes, potentialRules[indexOfRule].b.atrName); i < indexes.Count(); i++)
                {
                    if (instances[indexes[i]].atributes[ind].textAtr.Equals(potentialRules[indexOfRule].b.textAtr))
                        indexesOfLeftInstances.Add(indexes[i]);
                    else
                        indexesOfRightInstances.Add(indexes[i]);
                }
                tree.leftChild = CART(indexesOfLeftInstances.ToArray()/*, usedRules*/);
                tree.rightChild = CART(indexesOfRightInstances.ToArray()/*, usedRules*/);
                return tree;
            }
        }



        public double CalculateGini(double kl, double kr, double[] kjl, double[] kjr)
        {
            double leftSumm = 0f;
            double rightSumm = 0f;
            var inverseKL = 1 / kl;
            var inverseKR = 1 / kr;

            foreach (var item in kjl)
                leftSumm += Math.Pow(item, 2);
            foreach (var item in kjr)
                rightSumm += Math.Pow(item, 2);

            var result = inverseKL * leftSumm + inverseKR * rightSumm;
            return result;
        }

        public int FindMax(double[] array)
        {
            var max = array.Max();
            return Array.IndexOf(array, max);
        }


        public bool IsTerminate(int[] indexes)
        {
            string someClass = instances[indexes[0]].instanceClass;
            string someClass2 = null;

            for (int i = 1; i < indexes.Count(); i++)
            {
                someClass2 = instances[indexes[i]].instanceClass;
                if (!someClass2.Equals(someClass))
                    return false;
            }
            return true;
        }

        
    }
}
