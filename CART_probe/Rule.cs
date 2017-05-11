using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CART_probe
{
    class Rule : IEquatable<Rule>
    {
        public string rule;
        public  Atribute b;

        //private List<int> indexesOfLeftChilds;
        //private List<int> indexesOfRightChilds;

        public Rule(Atribute someAtr)
        {
            b = someAtr;
            rule = WriteRule();
        }

        public string WriteRule()
        {
            if (b.atrName != null)
            {
                if (b.textAtr != null)
                    return b.atrName + " is " + b.textAtr + "?";
                else
                    return b.atrName + " <= " + b.intAtr.ToString() + "?";
            }
            else
                return b.textAtr;
        }

        public bool Equals(Rule other)
        {
            if (other.rule.Equals(rule))
                return true;
            else
                return false;
        }
    }
}
