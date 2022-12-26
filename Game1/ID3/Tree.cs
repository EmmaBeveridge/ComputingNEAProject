using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;



namespace Game1.ID3
{
    public class Tree
    {
        public TreeNode Root;

         public Dictionary<string, Dictionary<int, int>> needDiscreteCategoryBoundaryValues = new Dictionary<string, Dictionary<int, int>>();

        public TreeNode Learn(DataTable data, string edgeName)
        {
            TreeNode root = GetRootNode(data, edgeName);

            Console.WriteLine(root.Name);


            foreach (var edge in root.Attribute.AttributeNames)
            {
                bool isLeaf = IsLeaf(root, data, edge);

                if (!isLeaf)
                {
                    DataTable reducedTable = ReduceTable(data, edge, root.TableIndex);
                    root.Children.Add(Learn(reducedTable, edge));
                }

            }

            return root;


        }

        private static bool IsLeaf(TreeNode root, DataTable data, string attributeToCheck)
        {
            bool isLeaf = true;
            List<string> allEndValues = new List<string>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                Console.WriteLine(i);
                if (data.Rows[i][root.TableIndex].ToString().Equals(attributeToCheck))
                {
                    allEndValues.Add(data.Rows[i]["SELECTED_NEED"].ToString()); 
                }

            }

            if (allEndValues.Count>0 && allEndValues.Any(x => x !=allEndValues[0])){ isLeaf = false; } //checks if all end values the same for given attribute -if not, attrbute is not a leaf

            if (isLeaf) { root.Children.Add(new TreeNode(true, allEndValues[0], attributeToCheck)); }
            return isLeaf;


        }

        private static DataTable ReduceTable(DataTable data, string edgeToNextNode, int rootTableIndex)
        {
            var reducedData = new DataTable();

            for (int i = 0; i < data.Columns.Count; i++)
            {
                reducedData.Columns.Add(data.Columns[i].ToString());
            }

            for (int i = 0; i < data.Rows.Count; i++)
            {
                if (data.Rows[i][rootTableIndex].ToString().Equals(edgeToNextNode))
                {
                    string[] row = new string[data.Columns.Count];

                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        row[j] = data.Rows[i][j].ToString();
                    }

                    reducedData.Rows.Add(row);

                }

            }

            reducedData.Columns.Remove(reducedData.Columns[rootTableIndex]);
            return reducedData;


        }

        private TreeNode GetRootNode(DataTable data, string edgeName)
        {
            List<NodeAttribute> attributes = new List<NodeAttribute>();
            var maximumInformationGainIndex = -1;
            double maximumInformationGain = -1;

            for (int i = 1; i < data.Columns.Count-1; i++) //don't want to use ID column so start at index 1
            {
                List<string> uniqueAttributeNames = NodeAttribute.GetDifferentAttributeNamesInColumn(data, i);
                attributes.Add(new NodeAttribute(data.Columns[i].ColumnName, uniqueAttributeNames));
            }

            for (int i = 0; i < attributes.Count; i++)
            {
                attributes[i].InformationGain = CalculateInformationGain(data, attributes[i].Name).InformationGain;
                if (attributes[i].InformationGain > maximumInformationGain)
                {
                    maximumInformationGain = attributes[i].InformationGain;
                    maximumInformationGainIndex = i;
                }




            }


            return new TreeNode(attributes[maximumInformationGainIndex].Name, maximumInformationGainIndex, attributes[maximumInformationGainIndex], edgeName);







        }

        public DataTable DiscretiseData(DataTable data) //NEED TO PUT INDEX COLUMN IN DATA
        {
            DataTable discreteTable = data.Clone();


            //for (int i = 0; i < data.Rows.Count; i++)
            //{
            //    discreteTable.Rows.Add(i);
            //}


            List<DataTable> ColumnResultData = MakeColumnDataTables(data);

            Dictionary<string, DataTable> discreteColumnsDict = new Dictionary<string, DataTable>();

            foreach (DataTable ColumnResultTable in ColumnResultData)
            {

                Dictionary<int, int> columnDiscreteBoundaries = new Dictionary<int, int>();

                DataTable discreteColumn = DiscretiseColumn(ColumnResultTable, ref columnDiscreteBoundaries);

                discreteColumn.Columns.RemoveAt(2); //removes results column

                discreteColumnsDict.Add(ColumnResultTable.TableName, discreteColumn);

                needDiscreteCategoryBoundaryValues.Add(ColumnResultTable.TableName, columnDiscreteBoundaries);
                


                
            }

            Regex regex = new Regex(@"\S+_PRIORITISED");


            foreach (DataRow row in data.Rows)
            {
                DataRow discreteRow = discreteTable.NewRow();
                //discreteRow[0] = row[0]; //sets id 

                foreach (DataColumn column in discreteTable.Columns)
                {

                    if (regex.IsMatch(column.ColumnName) || column.ColumnName == "ID" || column.ColumnName == "SELECTED_NEED")
                    {
                        discreteRow[column] = row[column.ColumnName]; //data either id or prioirty  or result therefore already discrete
                    }
                    else
                    {
                        string selectExpression = $"ID = {row[0]}";


                        int columnValue = (int)discreteColumnsDict[column.ColumnName].Select(selectExpression)[0][1];

                        discreteRow[column.ColumnName] = columnValue;


                    }       




                }


                discreteTable.Rows.Add(discreteRow);


            }

            return discreteTable;



        }



        private DataTable DiscretiseColumn (DataTable columnData, ref Dictionary<int, int> discreteBoundaries)
        {
            
            columnData.DefaultView.Sort = columnData.Columns[1].ColumnName;
            columnData = columnData.DefaultView.ToTable();

            columnData.Columns.Add("ORDERED_ID", typeof(int));

            int count = 0;

            foreach (DataRow row in columnData.Rows)
            {
                row["ORDERED_ID"] = count;
                count++;
            }


            
            //int uniqueResults = columnData.DefaultView.ToTable(true, columnData.Columns[2].ColumnName).Rows.Count;

            return CutColumn(columnData, 0, ref discreteBoundaries);


            



        }

        private DataTable CutColumn(DataTable partition, int cutCount, ref Dictionary<int, int> discreteBoundaries)
        {
           Cut bestCut = FindBestCut(partition);
            DataTable mergedTable = new DataTable();

            Console.WriteLine("Cut count:"+cutCount);

            if (bestCut!= null && ShouldCut(bestCut))
            {
                List<DataTable> partitionTables = partition.AsEnumerable().GroupBy(r => r.Field<int>(partition.Columns["ORDERED_ID"]) <= bestCut.CutIndex).Select(x => x.CopyToDataTable()).ToList();



                int newCutCount = new int();
                newCutCount = 2*cutCount;   //uses binary tree indexing to ensure unique cut count key for each recursive call - at most 2 further calls made each time, these calls have cutcount of index of left and right child nodes in binary tree
                discreteBoundaries.Add(cutCount, bestCut.splitValue);
                foreach (DataTable subPartition in partitionTables)
                {
                    newCutCount++;
                    
                    mergedTable.Merge(CutColumn(subPartition, newCutCount, ref discreteBoundaries)); //need to fix cut count when collapses back down

                    Console.WriteLine($"New Cut count:{newCutCount} Cut COunt:{cutCount}");

                }

                return mergedTable;

            }
            else
            {
                
                
                
                foreach (DataRow row in partition.Rows)
                {
                    row[1] = cutCount;//sets all rows to categoric value

                }

                return partition;
            }





        }



        private bool ShouldCut(Cut bestCut)
        {
            //uses https://www.ijcai.org/Proceedings/93-2/Papers/022.pdf pg5

            double weightedChildEntropySum = 0;
            for (int i = 0; i < bestCut.ChildrenEntropy.Count; i++)
            {
                weightedChildEntropySum += bestCut.ChildrenEntropy[i] * bestCut.ChildrenResultClassesCount[i];
            }

            double deltaATS = Math.Log(Math.Pow(3, bestCut.ResultClassesCount) - 2, 2) - (bestCut.ResultClassesCount * bestCut.ParentEntropy - weightedChildEntropySum);
            double RHS = (1 / bestCut.ResultClassesCount) * (Math.Log(bestCut.ResultClassesCount - 1) + deltaATS);

            if (bestCut.InformationGain>RHS) { return true; }
            else { return false; }




        }


        private Cut FindBestCut(DataTable partition)
        {
            List<int> cutIndexes = FindBoundaryIndices(partition);

            double maxInformationGain = -1;
            Cut bestCut = null;

            
                Console.WriteLine("Finding Best Cut for"+partition.TableName);



            foreach (int index in cutIndexes)
            {


                int splitValue = 0;

                Console.WriteLine("Started"+index);
                DataTable binaryPartition = MakeBinaryTable(partition.Copy(), index, ref splitValue);
                //Console.WriteLine("Done:" + index);
                Cut cut = CalculateInformationGain(binaryPartition, "DISCRETISED");
                cut.CutIndex = index;
                cut.splitValue = splitValue;

                if (cut.InformationGain > maxInformationGain)
                {
                    maxInformationGain = cut.InformationGain;
                    bestCut = cut;
                }

            }

            if(bestCut == null)
            {
                
                Console.WriteLine();
            }

            return bestCut;



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="splitColumnName">Name of column to split on / to calculate info gain for</param>
        /// <returns></returns>

        private Cut CalculateInformationGain(DataTable partition, string splitColumnName) 
        {
            Cut cut = new Cut();
            cut.SizeOfParent = partition.Rows.Count;
            cut.ParentEntropy = FindEntropy(partition, ref cut.ResultClassesCount);
            
            DataTable uniqueSplitValues = partition.DefaultView.ToTable(true, splitColumnName);

            double childEntropySum = 0;

            foreach (DataRow uniqueSplitRow in uniqueSplitValues.Rows)
            {
                string uniqueSplitValue = uniqueSplitRow[splitColumnName].ToString();
                DataTable childTable = partition.Select($"{splitColumnName} = '{uniqueSplitValue}' ").CopyToDataTable();
                int childResultSize = 0;
                double childEntropy = FindEntropy(childTable, ref childResultSize);
                cut.ChildrenEntropy.Add(childEntropy);
                cut.ChildrenResultClassesCount.Add(childResultSize);

                childEntropySum += ((float)childTable.Rows.Count / cut.SizeOfParent) * childEntropy;


            }


            cut.InformationGain = cut.ParentEntropy - childEntropySum;

            return cut;

        }


        private DataTable MakeBinaryTable(DataTable partition, int cutIndex, ref int splitValue)
        {
            splitValue = (int)((int.Parse((partition.Select($"ORDERED_ID = {cutIndex}")[0].ItemArray[1]).ToString()) + int.Parse(partition.Select($"ORDERED_ID = {cutIndex+1}")[0].ItemArray[1].ToString()))/2);

            partition.Columns.Add("DISCRETISED", typeof(int), $"IIF ({partition.Columns[1].ColumnName} >= {splitValue} , 1, 0)");
            //partition.Columns.Remove(partition.Columns[1]);

            return partition;


        }


        private double FindEntropy(DataTable partition, ref int classCount)
        {

            //string resultColumnName = partition.Columns[partition.Columns.Count - 1].ColumnName;

            string resultColumnName = "SELECTED_NEED";


            DataTable uniqueResults = partition.DefaultView.ToTable(true, resultColumnName); //finds unique result values in partition
            classCount = uniqueResults.Rows.Count;
            Dictionary<string, float> resultCount = new Dictionary<string, float>();

            foreach (DataRow uniqueRow in uniqueResults.Rows)
            {
                string result = uniqueRow.ItemArray[uniqueRow.ItemArray.Length - 1].ToString();

                resultCount.Add(result, partition.Select($"{resultColumnName} = '{result}'").Length);

            }

            int numRows = partition.Rows.Count;

            List<float> probabilities = (from count in resultCount.Values select count / numRows).ToList();

            double entropy = probabilities.Sum(p => (-p * Math.Log(p, 2)));

            return entropy;


        }


        private List<int> FindBoundaryIndices(DataTable partition)
        {
            //finds smaller indexes where value in result column changes between rows
            List<int> indices = new List<int>();
            //int resultIndex = partition.Rows[0].ItemArray.Count() - 1;



            //string currentResult = partition.Rows[0].ItemArray[resultIndex].ToString();

            string currentResult = partition.Rows[0]["SELECTED_NEED"].ToString();
            for (int i = 0; i < partition.Rows.Count; i++)
            {
                if (partition.Rows[i]["SELECTED_NEED"] != currentResult)
                {
                    currentResult = partition.Rows[i]["SELECTED_NEED"].ToString();
                    indices.Add((int)partition.Rows[i - 1]["ORDERED_ID"]);


                }
            }

            return indices;

        }

        /// <summary>
        /// returns list of smaller data tables split by need with id column, continuous need value column, and need name result column
        /// </summary>
        /// <param name="data">Continuous full need data table</param>
        /// <returns></returns>

        private List<DataTable> MakeColumnDataTables(DataTable data)
        {
            List<DataTable> ColumnDataTables = new List<DataTable>();

            Regex regex = new Regex(@"\S+_PRIORITISED");

            
            //DataColumn resultData = new DataColumn();
            //resultData.DataType = typeof(string);
            //resultData.ColumnName = data.Columns[data.Columns.Count - 1].ColumnName;

            //DataColumn ID = new DataColumn();
            //ID.DataType = typeof(int);
            //ID.Unique = true;
            //ID.ColumnName = "ID";



            foreach (DataColumn column in data.Columns)
            {
                if (regex.IsMatch(column.ColumnName) || column.ColumnName=="ID" || column.ColumnName == "SELECTED_NEED")
                {
                    continue;
                    //data.Columns.Remove(column);
                }
                else
                {
                    DataTable columnTable = new DataTable();
                    columnTable.TableName = column.ColumnName;
                    columnTable.Columns.Add(new DataColumn("ID", typeof(int)));
                    columnTable.Columns.Add(column.ColumnName, typeof(int));
                    columnTable.Columns.Add(new DataColumn("SELECTED_NEED", typeof(string)));

                    ColumnDataTables.Add(columnTable);


                }
            }

            
           
            foreach (DataRow row in data.Rows)
            {


                foreach (DataTable columnTable in ColumnDataTables)
                {
                    DataRow rowToAdd = columnTable.NewRow();
                    rowToAdd[0] = row["ID"]; //sets id
                    rowToAdd[1] = row[columnTable.TableName]; //add value for need to row
                    rowToAdd[2] = row["SELECTED_NEED"]; //add result to row
                    columnTable.Rows.Add(rowToAdd);
                }



            }

            return ColumnDataTables;




           
        }




    }
}
