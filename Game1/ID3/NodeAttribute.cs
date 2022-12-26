using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ID3
{
    public class NodeAttribute
    {
        public string Name;
        public List<string> AttributeNames;
        public double InformationGain;


        public NodeAttribute(string _name, List<string> _attributeNames)
        {
            Name = _name;
            AttributeNames = _attributeNames;
        }

        public static List<string> GetDifferentAttributeNamesInColumn(DataTable data, int columnIndex)
        {
            List<string> differentAttributes = new List<string>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                bool exists = differentAttributes.Any(a => a.ToUpper() == data.Rows[i][columnIndex].ToString().ToUpper());
                if (!exists)
                {
                    differentAttributes.Add(data.Rows[i][columnIndex].ToString());
                }
            }

            return differentAttributes;
        }





    }
}
