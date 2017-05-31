﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            
            data = new LearningData(path1, classes1, atributes1);
            var used = new List<int>(data.GetCountOfRules());
            finalTree = data.CART(null);
            DisplayTree display_tree = new DisplayTree(finalTree, 0);
            alfaOrigin = finalTree.FindAlfa();
            DisplayAlpha(alfaOrigin);
            beta.Add(new Fraction(0));
            for (int i = 0; i < alfaOrigin.Count - 1; i++)
            {
                beta.Add(new Fraction(Math.Sqrt(alfaOrigin[i].Multiply(alfaOrigin[i + 1]).ToDouble())));
            }
            textBox1.SelectionStart = 0;
            var test = data.MakePortionsOfIndexes(10);
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
                finalTree.OpenTreeAndFill(indexOfData);
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

            textBox1.SelectionStart = 0;
        }

        private void DisplayAlpha(List<Fraction> array)
        {
            for (int i = 0, j = 0; i < array.Count; i++, j++)
            {
                textBox1.Text += (i + 1) + " - " + array[i].ToString() + "     ";

            }
                
            numericUpDown1.Maximum = array.Count;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            finalTree.CutForAlfa(alfaOrigin[Decimal.ToInt32(numericUpDown1.Value)]);
            DisplayTree display_tree = new DisplayTree(finalTree, Decimal.ToInt32(numericUpDown1.Value));
        }
    }
}
