using System;
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
        List<Fraction> alfa = new List<Fraction>();

        public Form1()
        {
            InitializeComponent();
            classes1 = new string[] { "unacc", "acc", "good", "vgood" };
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            data = new LearningData(path1, classes1, atributes1);
            var used = new List<int>(data.GetCountOfRules());
            Tree finalTree = data.CART(null/*, used*/);
            DisplayTree display_tree = new DisplayTree(finalTree);
            alfa = finalTree.FindAlfa();
            textBox1.SelectionStart = 0;
        }
    }
}
