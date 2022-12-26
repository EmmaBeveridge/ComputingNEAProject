using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.ID3
{
    public class Cut
    {
        public double InformationGain;

        public int SizeOfParent; //N
        public int ResultClassesCount;
        public double ParentEntropy;

        public List<double> ChildrenEntropy = new List<double>();
        public List<int> ChildrenResultClassesCount = new List<int>();


        public int splitValue;

        public int CutIndex;

        public Cut() { }



    }
}
