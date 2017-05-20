using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CART_probe
{
    class RulesForData
    {
        private string[] atributes;
        private List<List<Atribute>> atr_values;
        public List<Rule> potentialRules; // Набор потенциальных рабиений
        public RulesForData(string[] atrs)
        {
            atr_values = new List<List<Atribute>>();
            potentialRules = new List<Rule>();
            atributes = atrs;
            for (int i = 0; i < atributes.Length; i++)
            {
                List<Atribute> x = new List<Atribute>();
                atr_values.Add(x);
            }
        }
        public void AddNewAtr(int j_atrs, string atribute)
        {
            //собираем все возможные значения категориальных атрибутов
            int i = 0;
            bool exist = false;
            while (i < atr_values[j_atrs].Count && !exist)
            {
                if (atr_values[j_atrs][i].textAtr.Equals(atribute))
                {
                    exist = true;
                }
                else
                {
                    i++;
                }
            }
            if (!exist || atr_values[j_atrs].Count == 0)
            {
                var art_val = new Atribute(atributes[j_atrs], atribute, 0);
                atr_values[j_atrs].Add(art_val);
            }
        }
        public void MakePotentialsRules()
        {
            for(int j = 0; j < atributes.Length; j++)
            {
                var bit_per = new BitArray(atr_values[j].Count);
                var oneBits = new BitArray(atr_values[j].Count);
                bit_per.Set(0, true);
                oneBits.Set(0, true);
                while (bit_per.Get(atr_values[j].Count - 1) == false)
                {
                    //делаем правило из набора атрибутов в соответствии с битовым массивом
                    var listAtr = new List<Atribute>();
                    for (int i = 0; i < atr_values[j].Count; i++)
                    {
                        if (bit_per.Get(i))
                        {
                            listAtr.Add(atr_values[j][i]);
                        }                            
                    }
                    potentialRules.Add(new Rule(listAtr));
                    bit_per = SumBit(bit_per, oneBits);
                } 
            }
            
        }
        // сложение 2 битовых массивов. предполагается для одинаковых размерностей
        public BitArray SumBit(BitArray bits1, BitArray bits2) 
        {            
            var bitsXor = new BitArray (bits1);
            var bitsAnd = new BitArray (bits1);
            bitsXor.Xor(bits2);
            bitsAnd.And(bits2);
            bool next = false;
            for (int i = bitsAnd.Count - 1; i > 0; i--)
            {
                bitsAnd[i] = bitsAnd[i - 1];
                if (bitsAnd[i] == true)
                    next = true;
            }
            bitsAnd[0] = false;
            if (next)
                return SumBit(bitsXor, bitsAnd);
            else
                return bitsXor;
        }
        
    }
}
