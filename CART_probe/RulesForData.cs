using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CART_probe
{
    class RulesForData
    {
        private List<Atribute> atr_values;
        public List<Rule> potentialRules; // Набор потенциальных рабиений
        public RulesForData()
        {
            atr_values = new List<Atribute>();
            potentialRules = new List<Rule>();
        }
        public void AddNewAtr(string atributes, string atribute)
        {
            //собираем все возможные значения категориальных атрибутов
            int a = 0;
            bool exist = false;
            while (a < atr_values.Count && !exist)
            {
                if (atr_values[a].atrName.Equals(atributes) && atr_values[a].textAtr.Equals(atribute))
                {
                    exist = true;
                }
                else
                {
                    a++;
                }
            }
            if (!exist || atr_values.Count == 0)
            {
                var art_val = new Atribute(atributes, atribute, 0);
                atr_values.Add(art_val);
            }
        }
        
    }
}
