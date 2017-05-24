using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CART_probe
{
    struct Atribute : IEquatable<Atribute>
    {
        public string atrName;
        public string textAtr;
        public int intAtr;

        public Atribute(string name, string a, int b)
        {
            textAtr = a;
            intAtr = b;
            atrName = name;
        }

        public bool Equals(Atribute a)
        {
            if (atrName.Equals(a.atrName) && textAtr.Equals(a.textAtr))
                return true;
            else
                return false;
        }
    }

    class Instance
    {
        public string instanceClass;
        public Atribute[] atributes;

        public Instance(int k)
        {
            atributes = new Atribute[k];
            for (int i = 0; i < k; i++)
                atributes[i].textAtr = null;
        }
    }
}
