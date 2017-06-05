using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Fractions;


namespace CART_probe
{
    public partial class Form1 : Form
    {
        LearningData data;        
        string[] atributes1 = new string[] { "buing", "maint", "doors", "persons", "lug_boot", "safety" };
        string[] classes1;
        string path1 = @"car.txt";
        List<Fraction> alfaOrigin = new List<Fraction>();
        List<Fraction> errorTreeForAlfa = new List<Fraction>();
        List<int> CountOfLeafs = new List<int>();
        List<int> depth = new List<int>();
        Stopwatch stopwatch;

        List<Fraction> beta = new List<Fraction>();
        List<Fraction> errForBeta = new List<Fraction>();
        int minErrIndex;
        Tree finalTree;

        public Form1()
        {
            InitializeComponent();
            classes1 = new string[] { "unacc", "acc", "good", "vgood" };
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            stopwatch = new Stopwatch();   
            data = new LearningData(path1, classes1, atributes1);
            var used = new List<int>(data.GetCountOfRules());

            stopwatch.Start();
            finalTree = data.CART(null);
            stopwatch.Stop();
            Console.WriteLine("Time of building is " + stopwatch.Elapsed.Milliseconds);
            stopwatch.Reset();

            DisplayTree display_tree = new DisplayTree(finalTree, 0);
            stopwatch.Start();
            alfaOrigin = finalTree.FindAlfa(ref errorTreeForAlfa, ref CountOfLeafs, ref depth);
            stopwatch.Stop();
            Console.WriteLine("Time of cutting is " + stopwatch.Elapsed.Milliseconds);
            stopwatch.Reset();

            DisplayAlpha(alfaOrigin);
            display_tree.Graph(alfaOrigin, errorTreeForAlfa, CountOfLeafs);
            // нахождение бета

            stopwatch.Start();
            beta.Add(new Fraction(0));
            for (int i = 0; i < alfaOrigin.Count - 1; i++)
            {
                beta.Add(new Fraction(Math.Sqrt(alfaOrigin[i].Multiply(alfaOrigin[i + 1]).ToDouble())));
            }
            textBox1.SelectionStart = 0;

            //разбиение выбоки
            var test = data.MakePortionsOfIndexes(10);
            //проход по всем Gi
            for (int j = 0; j < test.Count; j++)
            {

                List<int> indexOfData = new List<int>();
                for (int i = 0; i < test.Count; i++)
                {
                    // составление списка без одной порции
                    if (i != j)
                    {
                        indexOfData.AddRange(test[i]);
                        if(indexOfData.Last() == 0)
                        {
                            indexOfData.RemoveAt(indexOfData.Count - 1);
                        }
                    }                    
                }
                //открытие веток и распределение выборки по дереву
                finalTree.OpenTreeAndFill(indexOfData);
                //ошибка классификации для заданного бета, и выборки
                var err = finalTree.FindErrForBeta(test[j], beta);
                if (errForBeta.Count == 0)
                {
                    errForBeta = err;
                }
                else
                {
                    for (int i = 0; i < errForBeta.Count; i++)
                    {
                        errForBeta[i] += err[i];
                    }
                }
            }
            //нахождение минимальной ошибки
            var minErr = errForBeta[0];
            minErrIndex = 0;
            for (int i = 1; i < errForBeta.Count; i++)
            {
                if (errForBeta[i] < minErr)
                {
                    minErr = errForBeta[i];
                    minErrIndex = i;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Time of cross-validation is " + stopwatch.Elapsed.Milliseconds);
            stopwatch.Reset();

            textBox1.SelectionStart = 0;
        }

        private void DisplayAlpha(List<Fraction> array)
        {
            textBox1.Text += "i       alfa          error          |T|       Глубина     Ca(T)\r\n";
            for (int i = 0, j = 0; i < array.Count; i++, j++)
            {
                var str = array[i].ToDouble().ToString();
                if (str.Length > 7)
                    str = str.Substring(0, 7);
                var str2 = errorTreeForAlfa[i].ToDouble().ToString();
                if (str2.Length > 7)
                    str2 = str2.Substring(0, 7);
                var C = errorTreeForAlfa[i].Add(array[i].Multiply(CountOfLeafs[i]));
                textBox1.Text += (i) + " - " + str + "     " + str2 + "     " + CountOfLeafs[i].ToString() + "     " + depth[i].ToString() + "         " + C.ToDouble().ToString() + "\r\n";

            }
                
            numericUpDown1.Maximum = array.Count - 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            finalTree.CutForAlfa(alfaOrigin[Decimal.ToInt32(numericUpDown1.Value)]);
            DisplayTree display_tree = new DisplayTree(finalTree, Decimal.ToInt32(numericUpDown1.Value));
        }
    }
}
