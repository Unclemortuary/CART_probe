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
        public  List<Atribute> b;


        public Rule(List<Atribute> someAtr)
        {
            b = someAtr;
            rule = WriteRule();
        }

        public string WriteRule()
        {
            string str;
            str = b[0].atrName;
            if (b[0].textAtr != null)
                str += " is ";
            else
                str += " <= ";
            for (int i = 0; i < b.Count; i++)
            {
                    if (b[i].textAtr != null)
                        str += b[i].textAtr + " ";
                    else
                        str += b[i].intAtr.ToString() + " ";
            }
            str += "?";
            return str;
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
