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
        /// <summary>
        /// String name of quantity represented by the attribute e.g. HUNGER, SLEEP_PRIORITISED
        /// </summary>
        public string Name;

        /// <summary>
        /// List of the possible values of the attribute I.e. the discrete need categories
        /// </summary>
        public List<string> AttributeNames;

        /// <summary>
        /// Information gain for splitting the dataset on this attribute
        /// </summary>
        public double InformationGain;

        /// <summary>
        /// Constructor for new NodeAttribute object.
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_attributeNames"></param>
        public NodeAttribute(string _name, List<string> _attributeNames)
        {
            Name = _name;
            AttributeNames = _attributeNames;
        }


        /// <summary>
        /// Static method to return a list of the different possible values for the attribute data in the specified column index for the dataset (dataset and column index passed as parameters to the method).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
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
