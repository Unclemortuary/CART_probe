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
        public static LearningData Instance;

        
        public List<Instance> instances;
        public RulesForData rulesForData;
        private List<int> usedRules = new List<int>();
        public string[] atributes;
        private string[] classes;
        


        public LearningData(string path, string[] clss, string[] atrs)
        {
            if (Instance == null)
                Instance = this;
            instances = new List<Instance>();            
            atributes = atrs;
            classes = clss;
            rulesForData = new RulesForData(atrs);
            InitializeData(path);
            rulesForData.MakePotentialsRules();
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
                    for (int i = 0, j = 0; i < arr.Length; i++) // Проходим по строчке, забирая атрибуты и название класса
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
                                rulesForData.AddNewAtr(j, atribute);
                                atribute = null;
                                j++;
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
        }

        public int GetCountOfRules()
        {
            return rulesForData.potentialRules.Count;
        }

        public void ColumnSorting(int index)
        {

        }

        public void MakeRuleForNumericAtr()
        {

        }

        public Tree CART(int[] indexes /*, List<int> usedRules*/) // indexes - массив индексов примеров в instances для данного узла
        {
            int kL = 0, kR = 0;
            int[] kjL = new int[classes.Count()];
            int[] kjR = new int[classes.Count()];
            double pL = 0, pR = 0;
            double[] pjL = new double[classes.Count()];
            double[] pjR = new double[classes.Count()];
            double[] giniTable = new double[rulesForData.potentialRules.Count];
            int indexOfCurrentAtribute = 0;
            List<int> indexesOfLeftInstances = new List<int>();
            List<int> indexesOfRightInstances = new List<int>();

            if (indexes == null)
            {
                indexes = new int[instances.Count];
                for (int q = 0; q < indexes.Length; q++)
                    indexes[q] = q;
            }

            Tree tree = new Tree(indexes);
            if (IsTerminate(indexes) /*|| usedRules.Count == rulesForData.potentialRules.Count*/) // Проверяем лист или нет
            {
                tree.isTerminate = true;
                var nameOfClass = instances[indexes[0]].instanceClass;
                // Если да, то создаем правило, которое содержит только название класса
                tree.rule = new Rule(new List<Atribute> { new Atribute(null, nameOfClass, 0) });
                // И выходим из функции
                return tree;
            }
            else
            {
                for (int i = 0; i < rulesForData.potentialRules.Count; i++)
                {
                    //if (usedRules.Contains(i))
                    //    continue;

                    for (int k = 0; k < atributes.Length; k++)
                    {
                        if (rulesForData.potentialRules[i].b[0].atrName.Equals(atributes[k]))
                            indexOfCurrentAtribute = k;
                    }

                    for (int j = 0; j < indexes.Length; j++)
                    {
                        if (rulesForData.potentialRules[i].b[0].textAtr == null) // Если параметр непрерывный, то сравниваем (НАДО ДОПИСАТЬ!)
                        {

                        }
                        else
                        {
                            bool isLeft = false;
                            for (int q = 0; q < rulesForData.potentialRules[i].b.Count; q++)
                            {
                                if (instances[indexes[j]].atributes[indexOfCurrentAtribute].Equals(rulesForData.potentialRules[i].b[q]))
                                {
                                    kL++;
                                    var indexOfClass = Array.IndexOf(classes, instances[indexes[j]].instanceClass); // Берем индекс класса соответсвтующего примера
                                    kjL[indexOfClass]++; // И прибавляем 1 в ячейке массива с такой же размерностью с соответсвующим индексом
                                    // Таким образом подсчитываем кол-во примеров каждого класса в левом потомке
                                    isLeft = true;
                                }                                
                            }
                            if (!isLeft)
                            {
                                var indexofclass = Array.IndexOf(classes, instances[indexes[j]].instanceClass);
                                kjR[indexofclass]++;
                            }
                        }
                    }
                    kR = instances.Count - kL;
                    pL = (double) kL / indexes.Length;
                    pR = 1 - pL;
                    for (int j = 0; j < pjL.Length; j++)
                    {
                        if (kL == 0)
                            pjL[j] = 0;
                        else
                            pjL[j] = (double) kjL[j] / kL;
                        if (kR == 0)
                            pjR[j] = 0;
                        else
                            pjR[j] = (double) kjR[j] / kR;
                    }
                    giniTable[i] = CalculateGini(pL, pR, pjL, pjR);
                    kL = 0; kR = 0; pL = 0; pR = 0;
                    Array.Clear(kjL, 0, kjL.Length);
                    Array.Clear(kjR, 0, kjR.Length);
                    Array.Clear(pjL, 0, pjL.Length);
                    Array.Clear(pjR, 0, pjR.Length);
                }
                var indexOfRule = FindMax(giniTable); // Берем индекс самого лучшего правила из табицы ГИНИ
                //usedRules.Add(indexOfRule);
                tree.rule = rulesForData.potentialRules[indexOfRule]; // и заносим правило с таким индексом в дерево
                // Подсчитываем количество элементов в правом  левом потомке, основываясь на текущем разбиении
                for (int i = 0, ind = Array.IndexOf(atributes, rulesForData.potentialRules[indexOfRule].b[0].atrName); i < indexes.Count(); i++)
                {
                    for (int j = 0; j < rulesForData.potentialRules[indexOfRule].b.Count; j++)
                    {
                        if (instances[indexes[i]].atributes[ind].Equals(rulesForData.potentialRules[indexOfRule].b[j]))
                            indexesOfLeftInstances.Add(indexes[i]);
                    }
                    //!!!!!намного оптимальнее поставить флаг
                    if(!indexesOfLeftInstances.Contains(indexes[i]))
                        indexesOfRightInstances.Add(indexes[i]);
                }


                if (indexesOfLeftInstances.Count == 0)
                {
                    tree.isTerminate = true;
                    List<Atribute> temp = new List<Atribute>(1)
                    {
                        new Atribute(null, FindPrevailClass(indexesOfRightInstances.ToArray()), 0)
                    };
                    tree.rule = new Rule(temp);
                    return tree;
                }
                else
                {
                    if (indexesOfRightInstances.Count == 0)
                    {
                        tree.isTerminate = true;
                        List<Atribute> temp = new List<Atribute>(1);
                        temp.Add(new Atribute(null, FindPrevailClass(indexesOfLeftInstances.ToArray()), 0));
                        tree.rule = new Rule(temp);
                        return tree;
                    }
                    else
                    {
                        tree.leftChild = CART(indexesOfLeftInstances.ToArray()/*, usedRules*/);
                        tree.rightChild = CART(indexesOfRightInstances.ToArray()/*, usedRules*/);
                        return tree;
                    }
                }
                //if (indexesOfLeftInstances.Count != 0)
                //    tree.leftChild = CART(indexesOfLeftInstances.ToArray(), usedRules);
                //if (indexesOfRightInstances.Count != 0)
                //    tree.rightChild = CART(indexesOfRightInstances.ToArray(), usedRules);
                //return tree;
            }
        }

        public double CalculateGini(double pl, double pr, double[] pjl, double[] pjr)
        {
            double summ = 0;

            for (int i = 0; i < pjl.Count(); i++)
                summ += Math.Abs((pjl[i] - pjr[i]));

            return (2 * pl * pr * summ);
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

        //Находит преобладающий класс
        public string FindPrevailClass(int[] indexes)
        {
            //!!!!надо при построении эти значения запоминать, а не занаво считать
            int[] classesIndexes = new int[classes.Length];
            for (int i = 0; i < indexes.Length; i++)
            {
                var index = Array.IndexOf(classes, instances[indexes[i]].instanceClass);
                classesIndexes[index]++;
            }
            var finalIndex = Array.IndexOf(classesIndexes ,classesIndexes.Max());
            return classes[finalIndex];
        }

        public string FindCountOfAllClasses(int[] indexes)
        {
            int[] result = new int[classes.Length];
            string resultString = null;
            for (int i = 0; i < indexes.Length; i++)
            {
                var classOfInstance = instances[indexes[i]].instanceClass;
                var index = Array.IndexOf(classes, classOfInstance);
                result[index]++;
            }

            for (int i = 0; i < result.Length; i++)
            {
                resultString += classes[i] + " is " + result[i].ToString() + " ";
            }

            return resultString;
        }

        // Возвращает порции примеров исходного множества
        public List<int[]> MakePortionsOfIndexes(int portionNumbers)
        {
            int step = instances.Count / portionNumbers;
            var listResult = new List<int[]>();
            //int[,] result = new int[portionNumbers, step + 1];

            for (int i = 0; i < portionNumbers; i++)
            {
                var result = new int[step + 1];
                for (int j = 0, index = i; index < instances.Count; j++, index += portionNumbers)
                {
                    result[j] = index;
                }
                listResult.Add(result);
            }
            return listResult;
        }
    }
}
