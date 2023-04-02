using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ID3
{
    public class Cut
    {
        /// <summary>
        /// The information gain for a particular division of the parent dataset.
        /// </summary>
        public double InformationGain;

        /// <summary>
        /// Number of records in the parent dataset (|P|)
        /// </summary>
        public int SizeOfParent; //|P|

        /// <summary>
        /// Number of unique values in result column for records in parent dataset (|C|)
        /// </summary>
        public int ResultClassesCount;//|C|

        /// <summary>
        /// Value of parent dataset entropy (Ent(P))
        /// </summary>
        public double ParentEntropy;

        /// <summary>
        /// List of entropies for each of the child datasets resulting from the partition of the parent dataset (Ent(C_i))
        /// </summary>
        public List<double> ChildrenEntropy = new List<double>();

        /// <summary>
        /// List of number of unique values in result column for records in each child dataset resulting from the partition of the parent dataset. (|Ci|)
        /// </summary>
        public List<int> ChildrenResultClassesCount = new List<int>();

        #region Attributes for binary partition where cut splits parent partition into two child partitions and thus split value and cutIndex can be specified
        /// <summary>
        /// Used in the context of a binary partition where the cut splits the parent partition into two child partitions, creating a split value for which records can be split into child datasets by comparing their attribute value to the split value (T)
        /// </summary>
        public int splitValue;

        /// <summary>
        /// Used in the context of a binary partition where the cut splits the parent partition into two child partitions, creating a cut index for which records can be split into child datasets by comparing the ORDERED_ID field of the record to the CutIndex value.
        /// </summary>
        public int CutIndex;

        #endregion


        /// <summary>
        /// Constructor for new cut object
        /// </summary>
        public Cut() { }



    }
}
