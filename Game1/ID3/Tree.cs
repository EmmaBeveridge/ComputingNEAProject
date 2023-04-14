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

        /// <summary>
        /// Root node of ID3 decision tree. Contains node with attribute for which splitting data set on yields highest information gain. 
        /// </summary>
        public TreeNode Root;

        /// <summary>
        /// Dictionary of dictionaries containing data regarding conversion of continuous needs values to discrete categories for use in ID3. For the outer dictionary, the key is the name of the need for which the value dictionary stores category boundary values e.g. HUNGER. The dictionaries stored as values in the outer dictionary use the discrete category number (an arbitrary but unique number within particular need’s discrete categories) as a key and the highest continuous need value grouped in that category. Inner dictionaries are sorted ascending by value field. 
        /// </summary>
        public Dictionary<string, Dictionary<int, int>> needDiscreteCategoryBoundaryValues = new Dictionary<string, Dictionary<int, int>>();


        /// <summary>
        /// Dictionary to convert need name as string used in datatables to NeedNames enumeration values. 
        /// </summary>
        public Dictionary<string, NeedNames> NeedStringToEnum = new Dictionary<string, NeedNames>() { { "HUNGER", NeedNames.Hunger }, { "TOILET", NeedNames.Toilet }, { "SLEEP", NeedNames.Sleep }, { "FUN", NeedNames.Fun }, { "SOCIAL", NeedNames.Social }, { "HYGIENE", NeedNames.Hygiene } };


        /// <summary>
        /// Dictionary to convert NeedNames enumeration values to need name as string used in datatables. 
        /// </summary>
        public Dictionary<NeedNames, string> EnumToNeedString = new Dictionary<NeedNames, string>() { { NeedNames.Hunger, "HUNGER" }, { NeedNames.Toilet, "TOILET" }, { NeedNames.Sleep, "SLEEP" }, { NeedNames.Fun, "FUN" }, { NeedNames.Social, "SOCIAL" }, { NeedNames.Hygiene, "HYGIENE" } };

        /// <summary>
        /// Returns NeedNames enumeration value of the next need to be fulfilled given the current need status of the person. Uses MakeQueryDict method to create discrete query dictionary from query (need) data dictionary supplied as parameter. CalculateResult method is called and result is converted to a NeedNames enumeration value using NeedStringToEnum dictionary. This result is then returned. 
        /// Returns name (as NeedNames enum object) of the next need to be fulfilled given the current Need stats of the person
        /// </summary>
        /// <param name="QueryData"> DIctionary containing current Need values for the person </param>
        /// <returns></returns>
        public NeedNames GetResult(Dictionary<NeedNames, Need> QueryData, bool printTrace = false)
        {
            
            Dictionary<string, string> QueryDict = MakeQueryDict(QueryData);
            string result = CalculateResult(Root, QueryDict, "", QueryData, printTrace);

            if (result != "Attribute not found")
            {
                return NeedStringToEnum[result];
            }

            return NeedNames.Null;


        }


        /// <summary>
        /// Unfortunately, the data set on which the ID3 algorithm runs is not perfect and may occasionally lead to a need being selected that would not make sense in the context of the game. We can rectify this by checking if the suggested result by the ID3 algorithm is a good one before returning it. An acceptable difference in the score between the selected need and the least fulfilled need is calculated, if the selected need is prioritised by the character this acceptable difference is increased. We also posit that only certain needs can be fulfilled without addressing another need in between (e.g. fun and social actions make sense to repeat immediately, actions like using the toilet or showering do not). If the chosen need is already fulfilled and is not one that can be repeated immediately, the result is not good and the function will return false. If the value of the selected need differs from the value of the lowest scoring need by more than the calculated acceptable difference, the result is not good and the function returns false. Otherwise, the result is deemed good and the function returns true.
        /// CHecks if a sensible need has been selected by ID3. Necessary due to imperfect data set
        /// </summary>
        /// <param name="selectedNode">Node selected by ID3</param>
        /// <param name="QueryData"></param>
        /// <returns></returns>

        private bool IsResultGood (TreeNode selectedNode, Dictionary<NeedNames, Need> QueryData)
        {
            float acceptedDiffToMin = 20 + (QueryData[NeedStringToEnum[selectedNode.Name]].Prioritised ? 5 : 0);

            NeedNames[] CanRepeatImmediately = new NeedNames[] { NeedNames.Fun, NeedNames.Social };

            float valueOfSelectedNeed = QueryData[NeedStringToEnum[selectedNode.Name]].CurrentNeed;

            if (valueOfSelectedNeed == 100 && !CanRepeatImmediately.Contains(NeedStringToEnum[selectedNode.Name]) )
            {
                return false;

            }


            if (Math.Abs(valueOfSelectedNeed - QueryData.Min(kvp => kvp.Value.CurrentNeed)) < acceptedDiffToMin )
            {
                return true;

            }

            return false;




        }



        /// <summary>
        /// Recursive function used to query ID3 tree. Base case for if the root is a leaf: calls IsResultGood function with root node and query data arguments to determine if acceptable need selected from decision tree. If result node is acceptable, result node Name attribute is returned containing the name of the need to next be fulfilled. If the result is not acceptable, the name of the need with the lowest value is returned. If the node is not a leaf, as the data set is not perfect and there is not a record for every possible combination of discrete categories in the data set (especially as when the data set becomes larger, the discrete categories tend to continuous categories) the model tends to overfit which would result in it being unable to find a path from root to leaf through the tree. We therefore use a heuristic approach in the case where the current node does not contain an edge that exactly describes a feature of the query dictionary, we find the edge that is closest in accuracy to the query dictionary and follow that edge. We iterate over this list of child tree nodes in root.Children: if the edge of the node (I.e the value of the discrete category for the need represented by the root node) is equal to the discrete category value for the root node need in the query dictionary then the need data is removed from the query dictionary and a recursive call is made to CalculateResult sending updated query dictionary and child node (now root node) as arguments. If edge is not equal to query, the absolute difference is found between the estimated continuous value for the edge (found using dictionary stored in needDiscreteCategoryBoundaryValues with outer key as the name of the root node and inner key being the value of discrete category I.e. the edge) and the continuous need value for the query. If no exact match is found in any of the children, we then pick to follow the edge for which this difference is a minimum. The need data for the root node is removed from the query data and a recursive call is made to CalculateResult sending updated query dictionary and minimum edge difference child node (now root node) as arguments. The result is then returned from the function.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="QueryDict"></param>
        /// <param name="result"></param>
        /// <param name="QueryData"></param>
        /// <returns></returns>
        private string CalculateResult (TreeNode root, Dictionary<string, string> QueryDict, string result, Dictionary<NeedNames, Need> QueryData, bool printTrace = false)
        {



            if (printTrace)
            {
                Console.WriteLine(root.Name);

            }




            
            
            bool valueFound = false;
            //result += root.Name.ToUpper() + " -- ";



            if (root.IsLeaf)
            {

                //result = root.Edge.ToLower() + " --> " + root.Name.ToUpper();

                result = root.Name.ToUpper();





                if (IsResultGood(root, QueryData))
                {
                    valueFound = true;
                
                }

                else { result = EnumToNeedString[QueryData.OrderBy(kvp => kvp.Value.CurrentNeed).First().Key]; valueFound = true; } //just returns need with lowest value


                
            }

            else
            {

                string entry = QueryDict[root.Name];

                ///Console.WriteLine("\n\n\nEntry"+entry);


                //since don't have every possible combination of discrete categories in dataset, especially as data set gets large and discrete->continuous, must find a way to get closest edge from a given node

                TreeNode bestNode = null;


                //float closestEdgeDifference = Math.Abs(needDiscreteCategoryBoundaryValues[root.Name][int.Parse(root.Children[0].Edge)] - float.Parse(entry)); //fix this line



                float closestEdgeDifference = float.MaxValue;


                foreach (TreeNode childNode in root.Children)
                {
                    ///Console.WriteLine("Edge: "+childNode.Edge);

                    
                    if (childNode.Edge.ToUpper().Equals(entry.ToUpper()))
                    {
                        QueryDict.Remove(root.Name);

                        //return result + CalculateResult(childNode, QueryDict, $"{childNode.Edge.ToLower()} --> ");


                        if (printTrace) { Console.WriteLine($"\t|\n\t|{childNode.Edge}\n\t|\n\tV"); }



                        return CalculateResult(childNode, QueryDict, "", QueryData, printTrace);

                    }

                    else
                    {
                        float edgeDifference = Math.Abs(needDiscreteCategoryBoundaryValues[root.Name][int.Parse(childNode.Edge)] - QueryData[NeedStringToEnum[root.Name]].CurrentNeed); //edge refers to need value of root node name

                        if (edgeDifference < closestEdgeDifference)
                        {
                            bestNode = childNode;
                            closestEdgeDifference = edgeDifference;
                        }




                    }

                }

                //in case edge is not one of edges from root node


                QueryDict.Remove(root.Name);
                if (printTrace) { Console.WriteLine($"\t|\n\t|{bestNode.Edge}\n\t|\n\tV"); }

                return CalculateResult(bestNode, QueryDict, "", QueryData, printTrace);





                //foreach (TreeNode childNode in root.Children)
                //{

                //    foreach (KeyValuePair<string, string> entry in QueryDict)
                //    {
                //        if (childNode.Edge.ToUpper().Equals(entry.Value.ToUpper()) && root.Name.ToUpper().Equals(entry.Key.ToUpper()))
                //        {
                //            QueryDict.Remove(entry.Key);

                //            return result + CalculateResult(childNode, QueryDict, $"{childNode.Edge.ToLower()} --> ");
                //        }



                //    }

                //}
            }


            


            if (!valueFound)
            {
                result = "Attribute Not Found";
            }

            return result;
        }



        /// <summary>
        /// Creates a dictionary containing need strings as keys and discrete category value within which the continuous need lies. Iterates over a continuous dictionary with NeedNames enumeration keys (converts these to need strings values as used in tree nodes). Calls MakeQueryNeedDiscrete function to convert the continuous need value to a discrete category value. Adds key value pair to a query dictionary. Then checks if the need is prioritised and adds a key value pair object to query dictionary to describe the prioritised state. Returns the new discrete query dictionary. 
        /// </summary>
        /// <param name="QueryData"></param>
        /// <returns></returns>
        private Dictionary<string, string> MakeQueryDict(Dictionary<NeedNames, Need> QueryData)
        {

            Dictionary<string, string> QueryDict = new Dictionary<string, string>();

            foreach (KeyValuePair<NeedNames, Need> need in QueryData)
            {
                string NeedName = need.Key.ToString().ToUpper();
                int NeedDiscrete = MakeQueryNeedDiscrete(NeedName, (int)need.Value.CurrentNeed);

                QueryDict.Add(NeedName, NeedDiscrete.ToString());
               
                if (need.Key != NeedNames.Toilet) //no option to prioritise toilet need
                {
                    QueryDict.Add(NeedName + "_PRIORITISED", need.Value.Prioritised ? "1" : "0");


                }

            }


            return QueryDict;


        }


        /// <summary>
        /// Function to return the discrete category containing the continuous need value. Accesses discrete boundary dictionary for a particular need using needDiscreteCategoryBoundaryValues dictionary with key as the name of the need. The discrete categories are defined in the dictionary by the maximum continuous value contained within the category. The dictionary is ordered ascending by value and the first key for which the corresponding value is greater than the continuous value is returned. 
        /// </summary>
        /// <param name="NeedName"></param>
        /// <param name="NeedContValue"></param>
        /// <returns></returns>
        private int MakeQueryNeedDiscrete(string NeedName, int NeedContValue)
        {

            Dictionary<int, int> DiscreteBoundaries = needDiscreteCategoryBoundaryValues[NeedName];

            KeyValuePair<int, int> MaxBoundary = DiscreteBoundaries.OrderBy(key => key.Value).First(key => NeedContValue <= key.Value);  

            return MaxBoundary.Key;


        }

        /// <summary>
        /// Recursive method to construct ID3 tree. Uses GetRootNode method to return the node to be used as the root of the tree/subtree. We then iterate over each edge in root.Attribute.Attribute names and check if the attribute defines a result leaf node using IsLeaf() method. If the attribute does not define a result leaf node then further learning is required. We use ReduceTable function to return a smaller version of the dataset without the column set as the node. The Learn method is called recursively with the reduced dataset and current attribute name (current edge). The recursive call returns the root for the reduced dataset and is added to the list of children of parent node (root) in upper recursion layer. This process is repeated for each of the root nodes edges/attributes. Finially the root node is returned. This causes a tree to be constructed. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="edgeName"></param>
        /// <returns></returns>
        public TreeNode Learn(DataTable data, string edgeName)
        {
            TreeNode root = GetRootNode(data, edgeName);

            ///Console.WriteLine(root.Name);


            foreach (var edge in root.Attribute.AttributeNames)
            {
                bool isLeaf = IsLeaf(root, data, edge);

                if (!isLeaf)
                {
                    DataTable reducedTable = ReduceTable(data, edge, root.TableIndex);

                    var newChild = Learn(reducedTable, edge);
                    
                    root.Children.Add(newChild);
                }

            }

            return root;


        }


        /// <summary>
        /// Returns if the specified attribute should end with a leaf node is a leaf I.e. if all records in data set with this value for the particular attribute have the same result for the selected need to be fulfilled.  If these result values are all the same and the edge therefore defines a leaf a new TreeNode object is created with isLeaf value set to true, Name set to the name of result need shared by all records with attribute and edge set as attribute (discrete category represented by the edge). This new node is added to root.Children and function returns true. If the attribute does not end with a leaf, function returns false. 
        /// </summary>
        /// <param name="root">node from which edge extends</param>
        /// <param name="data">discrete data set</param>
        /// <param name="attributeToCheck">discrete category value for root node need represented by edge</param>
        /// <returns></returns>
        private static bool IsLeaf(TreeNode root, DataTable data, string attributeToCheck)
        {
            bool isLeaf = true;
            List<string> allEndValues = new List<string>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                //Console.WriteLine(i);
                if (data.Rows[i][root.TableIndex].ToString().Equals(attributeToCheck))
                {
                    allEndValues.Add(data.Rows[i]["SELECTED_NEED"].ToString());
                }

            }

            if (allEndValues.Count>0 && allEndValues.Any(x => x != allEndValues[0])){ isLeaf = false; } //checks if all end values the same for given attribute -if not, attrbute is not a leaf

            if (isLeaf) { root.Children.Add(new TreeNode(true, allEndValues[0], attributeToCheck)); }
            return isLeaf;


        }


        /// <summary>
        /// Returns a new version of the table with column with index rootTableIndex (dataset table index of the root node for current recursion level of Learn function passed as parameter) removed containing only records for which the value for the root attribute (e.g. HUNGER etc) is equal to the edgeToNextNode (discrete attribute category passed as parameter). As DataTable is a mutable reference type in C#, simply deleting the column in DataTable passed as parameter would delete the column too in the variable passed by the calling method (even though parameter is not passed with ref keyword). To preserve the state of the passed variable, we therefore must construct an entirely new data table by iterating over data table passed as parameter, selectively adding records that match our criteria. We can then delete the column at rootTableIndex for new data table with affecting original table and return new reduced data table.
        /// </summary>
        /// <param name="data">data table to reduce</param>
        /// <param name="edgeToNextNode">Discrete category for edge of tree following. Only records with this value for column identified by rootTableIndex will be added to reduced data table</param>
        /// <param name="rootTableIndex">Index of Column of root node in data table. Identifies column in reduced table to be deleted</param>
        /// <returns></returns>
        private static DataTable ReduceTable(DataTable data, string edgeToNextNode, int rootTableIndex)
        {
            //must make new data table as DataTable refernce type in C#: https://jonskeet.uk/csharp/parameters.html
            //changing data parameter would change data variable in Learn method.

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


        /// <summary>
        /// Returns the node to be used as root for subtree by determining the attribute in supplied data table with highest information gain. A list of data set attributes  of type NodeAttribute (attributes) is constructed by iterating over the columns in the data table (excluding the record ID column) and for each creating a list of the different values the attribute can take (the different names of the discrete categories for that particular attribute) using NodeAttribute class GetDifferentAttributeNamesInColumn static method supplied with data table and index of the attribute column in data table. A new NodeAttribute object is then created and added to attributes list with name of attribute (column name of attribute’s column) and unique attribute values supplied as parameters to constructor. The constructed attributes list is then iterated over and the InformationGain property of NodeAttribute object is assigned to return of CalculateInformationGain function supplied with data and name of attribute as parameter to return the information gain for the specified attribute. The attribute with the largest value for information gain is selected and a new TreeNode is constructed with the name of the attribute, information gain for attribute, NodeAttributeObject for attribute and edge value that connects new node to parent node as parameters. 
        /// </summary>
        /// <param name="data">(reduced) data table to find root node for</param>
        /// <param name="edgeName">Edge (discrete category value) connecting new node to parent root node. </param>
        /// <returns></returns>
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



        /// <summary>
        /// Takes data table containing continuous values in columns and returns a new data table with discrete values. Begins by creating a new discreteTable DataTable object cloned from original data (produces empty table with same schema as original continuous data table). Method calls MakeColumnDataTables sending continuous data as argument to return a list of smaller data tables for each of need value attributes (NB not columns describing if a need is prioritised as these are already Boolean (discrete) values) containing the columns with : record IDs, continuous need value data for a particular need (e.g. HUNGER, SLEEP etc), and result column containing the name of the need selected as need to be fulfilled in training data. For each of these smaller column result tables a new dictionary columnDiscreteBoundaries is created to contain the delimiting continuous values (highest continuous value mapped to discrete category) for each discrete category for given need attribute. The discrete column is created and columnDiscreteBoundaries populated using DiscretiseColumn method. Result column is removed from the discrete column returned and discrete column is added to a discrete column dictionary with name of need attribute as key and discrete column as value. The columnDiscreteBoundaries is added to needDiscreteCategoryBoundaryValues attribute with name of need attribute as its key. We then use the discretised columns to construct a fully discrete data table to be used in ID3 algorithm. Iterating over the rows in the continuous data table we create a new equivalent discrete row which, by iterating over the columns in the table, we populate. If the column is either the ID column (ID), a needs prioritised column ([NEEDNAME]_PRIORITISED) (0 or 1 identifying if particular need is prioritised by character) (identifies using a regular expression on the column name), or the results column (SELECTED_NEED), then the value in the column is already discrete (or for ID column should not be discretised) and as such the value for that column in the current row can be added to new discrete row. If the column formerly contained continuous data, value must be replaced with the new discrete value. A select expression is created using the row’s ID and is used to extract the discrete column value by first accessing the discrete column in the discrete columns dictionary using the column name as the key and selecting from the data table row with the same ID. The discrete column value from this row is then added to the created discrete row at the correct column. Once all value for all columns added to the discrete row, the discrete row is added to the discrete table. When all discrete rows have been constructed and added, the discrete table is returned. 
        /// </summary>
        /// <param name="data">Continuous data to be made discrete</param>
        /// <returns>Data table containing equivalent discrete data</returns>
        public DataTable DiscretiseData(DataTable data)
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


        /// <summary>
        /// Takes data table containing ID column, continuous value column, and result value column and returns a new data table where continuous values have been coded into discrete values, populating a dictionary of discrete category boundaries passed by reference. Column data is first sorted ascending by continuous need value column and a new ORDERED_ID column is added to data table and filled to give rows in sorted table consecutive ID numbers. Method then calls CutColumn method to begin cutting the continuous column into discrete values, passing the columnData data table and discrete category boundaries (by reference) as parameters. The parameters for cutCount and previousSplitValue are passed as 0 and 100 (the maximum need value) respectively. The return of CutColumn call is returned as discrete data table. 
        /// </summary>
        /// <param name="columnData">Continuous column data table with id, continuous need value, and result columns</param>
        /// <param name="discreteBoundaries">Dictionary for discrete category boundaries passed by reference</param>
        /// <returns></returns>
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

            return CutColumn(columnData, 0, 100, ref discreteBoundaries); //previous split value set to 100 as max value of needs

        }



        /// <summary>
        /// Columns are discretised by repeatedly splitting (cutting) column into groups cutting is no longer necessary (extending binary discretisation to divide continuous data into multiple categories). Each of these groups is assigned an arbitrary unique value which becomes the discrete value representing continuous values within the range of the group. Implements a recursive algorithm to select best cut point, cut column at that point and then apply a criterion to determine if further binary discretisation is required for a particular interval. We first create a new mergedTable data table that will contain the merged discrete data table for partition after undergoing binary discretisation if necessary. We then locate the best cut for the current partition of continuous column data using FindBestCut method, sending partition as argument, which returns a Cut object. If this bestCut Cut object is not null and applying cut criteria in ShouldCut method sent bestCut as argument determines we should cut column further, then we cut column. To cut column we create a list of partition tables using the ORDERED_ID field on sorted current partition data table to divide into two further partitions, one containing all rows with an ORDERED_ID less than or equal to the cutIndex of the bestCut and one with all the rows with an ORDERED_ID greater than the cutIndex of the bestCut. A new value for the discrete boundary is created using binary tree indexing to ensure a unique value for each recursive call as partition split into at most 2 partitions and thus at most 2 further calls made each time. This new value (newCutCount) is created by multiplying cutCount parameter by two and adding 1 (which would be the left child index of a parent node with index of cutCount). Sub-partitions may need to be swapped such that the left partition (containing rows with smaller ORDERED_ID values) is in index 0 of partitionTables array. We now call CutColumn method recursively for each sub-partition. Beginning with left partition, we call with parameters of left sup-partition, newCutCount, discreteBoundaries dictionary, and for the previousSplitValue we use the splitValue attribute of the bestCut object as this will be the largest continuous value found within the sub-partition and so will be needed to add to discreteBoundaries dictionary. The return of this recursive call (a fully discretised version of the left sub-partition likely after further recursive calls) is then merged into mergedTable. We increment newCutCount so points to right sub-partition index and repeat recursive call for right sub-partition but with previousSplitValue for new recursive call being the same as the previousSplitValue for current recursive call as this will still be largest continuous value found in right sub-partition. The return of this recursive call (a fully discretised version of the right sub-partition likely after further recursive calls) is then merged into mergedTable. The mergedTable is then returned. The recursion base case is that the FindBestCut method returns null, or it is determined that we should not further cut the column. In this case we must set all of the continuous values in the partition to the same discrete value, using cutCount as the discrete value as it is guaranteed to be unique (NB discrete value assigned to group is completely arbitrary so long as it is unique and matches value added to discreteBoundaries dictionary, we do not need to worry therefore about not consecutive cutCount values being used).  After setting all rows in partition to discrete value, we add the discrete value (cutCount) and previousSplitValue (largest continuous value that had been contained within the partition) to discreteBoundaries dictionary. We then return the discrete partition.  The final return of the function will be a fully merged discrete column and updates discreteBoundaries dictionary passed by reference. 
        /// </summary>
        /// <param name="partition">Current partition to cut (or not cut and instead discretise)</param>
        /// <param name="cutCount">Unique discrete value for the current partition</param>
        /// <param name="previousSplitValue">Largest continuous value contained within partition</param>
        /// <param name="discreteBoundaries">dictionary passed by reference containing greatest continous value mapped to each discrete value</param>
        /// <returns></returns>
        private DataTable CutColumn(DataTable partition, int cutCount, int previousSplitValue, ref Dictionary<int, int> discreteBoundaries)
        {
           Cut bestCut = FindBestCut(partition);
            DataTable mergedTable = new DataTable();

            //Console.WriteLine("Cut count:"+cutCount);

            if (bestCut!= null && ShouldCut(bestCut))
            {
                List<DataTable> partitionTables = partition.AsEnumerable().GroupBy(r => r.Field<int>(partition.Columns["ORDERED_ID"]) <= bestCut.CutIndex).Select(x => x.CopyToDataTable()).ToList();



                int newCutCount = new int();
                newCutCount = 2*cutCount+1;   //uses binary tree indexing to ensure unique cut count key for each recursive call - at most 2 further calls made each time, these calls have cutcount of index of left and right child nodes in binary tree
                
                
                
                
                if (int.Parse(partitionTables[0].Rows[0]["ORDERED_ID"].ToString()) > int.Parse( partitionTables[1].Rows[0]["ORDERED_ID"].ToString())) //if left partition, smaller orderedid, not in index 0 of partitionTables
                {
                    DataTable temp = partitionTables[0];
                    partitionTables[0] = partitionTables[1];
                    partitionTables[1] = temp;


                }

                mergedTable.Merge(CutColumn(partitionTables[0], newCutCount, bestCut.splitValue, ref discreteBoundaries));

                newCutCount++;

                mergedTable.Merge(CutColumn(partitionTables[1], newCutCount, previousSplitValue, ref discreteBoundaries));

                return mergedTable;

            }
            else
            {
                
                //not adding final recursive cut to dictionary do it 
                
                foreach (DataRow row in partition.Rows)
                {
                    row[1] = cutCount;//sets all rows to categoric value

                }

                discreteBoundaries.Add(cutCount, previousSplitValue);
                

                return partition;
            }





        }


        /// <summary>
        /// Applies MDLP criterion to determine if the specified cut should be applied, see proect writeup for formula derivations and explanantions etc. Theory: https://www.ijcai.org/Proceedings/93-2/Papers/022.pdf 
        /// </summary>
        /// <param name="bestCut"></param>
        /// <returns></returns>
        private bool ShouldCut(Cut bestCut)
        {
            

            double weightedChildEntropySum = 0;
            for (int i = 0; i < bestCut.ChildrenEntropy.Count; i++)
            {
                weightedChildEntropySum += bestCut.ChildrenEntropy[i] * bestCut.ChildrenResultClassesCount[i];
            }

            double deltaATS = Math.Log(Math.Pow(3, bestCut.ResultClassesCount) - 2, 2) - (bestCut.ResultClassesCount * bestCut.ParentEntropy - weightedChildEntropySum);
            double RHS = (1 / bestCut.ResultClassesCount) * (Math.Log(bestCut.ResultClassesCount - 1, 2) + deltaATS);

            if (bestCut.InformationGain>RHS) { return true; }
            else { return false; }




        }

        /// <summary>
        /// Given a partition returns Cut object representing optimal data sub-partition such that the information gain of the cut is greatest. We begin by obtaining a list of boundary indexes, I.e. the ORDERED_IDs of records in the ordered partition such that the subsequent record in the ordered partition has a different result class, using FindBoundaryIndexes method. It can be shown that a cut T of dataset S on an attribute A such that the weighted average child subset entropy Ent(A,T:S) is minimised will always be on one of these boundary indexes. This not only supports our use of an entropy minimisation heuristic in discretising the data as cuts that are illogical are never favoured by heuristic. By only examining the boundary indexes B as possible cut locations; for a dataset S for which the result classes present in S form set C : |C|-1 ≤ |B| ≤ |S|-1 . In most cases |C|<<|S| therefore we drastically improve the efficiency of our discretisation algorithm by only considering boundary indexes. For each of these boundary indexes within the range of the current partition, we consider the binary child partitions produced from applying cut. We obtain the binary partition data table using MakeBinaryTable method which allocates records in partition to sub-partitions using supplied cut index. For the resulting binary table we use the CalculateInformationGain method to return a Cut object. This process is repeated for all boundary (cut) indexes; the one yielding a cut with the greatest information gain returned as the best cut 
        /// </summary>
        /// <param name="partition">Partition to cut</param>
        /// <returns>Cut object with highest information gain</returns>
        private Cut FindBestCut(DataTable partition)
        {
            List<int> cutIndexes = FindBoundaryIndexes(partition);

            double maxInformationGain = -1;
            Cut bestCut = null;

            
                //Console.WriteLine("Finding Best Cut for"+partition.TableName);



            foreach (int index in cutIndexes)
            {


                int splitValue = 0;

                //Console.WriteLine("Started"+index);
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
                
                //Console.WriteLine();
            }

            return bestCut;



        }

        /// <summary>
        /// Returns new Cut object with InformationGain attribute assigned. The information gain of the cut is a measure of the reduction in the entropy of a data set by splitting it into two subsets according to the split value of a random variable/attribute of the data. We calculate the information gain resulting from cut value of T on attribute A of a dataset S to be: 
        /// Gain(A, T:S)=Ent(S)-Ent(A, T; S)
        /// Gain(A, T:S)=Ent(S)-|S_1 |/|S|*Ent(S_1 )-|S_2 |/|S|*Ent(S_2 )
        ///Method first creates a new cut object and assigns SizeOfParent attribute to the number of rows in the partition supplied as a parameter.The parent entropy is then calculated and assigned using FindEntropy method.In order to find the child entropy we must first split the full partition into the child partition.We split into child partitions based on the values in the specified split column (when called from FindBestCut() this will always be the DISCRETISED column in binary table sent as parameter, when called from GetRootNode() this will be the column name for the attribute currently being investigated as the attribute to be used in the root node). By identifying the possible values of the split column attribute and then grouping the partition data by this attribute we obtain our child partition tables.The FindEntropy method is used with child partition parameters to return entropy of child partition and number of unique result classes in partition – these values are added to the cut’s ChildrenEntropy and ChildrenResultClassesCount lists respectively. The weighted child entropy sum is incremented with these values according to the formula above.Once all child partitions have been processed we calculate cut information gain by subtracting the weighted child entropy sum from the parent entropy, assigning this value to the cut’s InformationGain attribute and returning the cut.
        /// </summary>
        /// <param name="partition">Partition to be cut</param>
        /// <param name="splitColumnName">Name of column to split on / to calculate info gain for</param>
        /// <returns>Cut object containing data for partition cut on split coulmn name</returns>
    private Cut CalculateInformationGain(DataTable partition, string splitColumnName) 
        {
            Cut cut = new Cut();
            cut.SizeOfParent = partition.Rows.Count;
            cut.ParentEntropy = FindEntropy(partition, ref cut.ResultClassesCount);
            
            DataTable uniqueSplitValues = partition.DefaultView.ToTable(true, splitColumnName); //will only contain tow values when split on DISCRETISED column, contains more when split on a different attribute column when determining new node;  information gain calculated same regardless of number of child classes cut divides partition into.

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


        /// <summary>
        /// Returns a binary table for partition supplied as parameter. The method is supplied with the ORDERED_ID of the record with attribute value immediately less than split value for the data. The continuous split value is calculated as the median value of the split attribute for the record between which the cut lies. Using this split value, we assign each partition record to one of two sub-partitions, where one contains all of the records with attribute value greater than or equal to split value (P1) and the other containing those with attribute value less than the split value (P2). We catalogue the sub-partition to which the record is assigned by adding a DISCRETISED column to our binary data table where records that are elements of P1 are assigned 1 and records that are elements of P2 are assigned 0. 
        /// </summary>
        /// <param name="partition">Dataset to partition into two binary subpartitions </param>
        /// <param name="cutIndex">IORDERED_ID of record with attribute immediately smaller than split value i.e. cut falls just below </param>
        /// <param name="splitValue">Continuous value that splits partition</param>
        /// <returns>Binary table for partition- allocation of records indicated by DISCRETISED column</returns>
        private DataTable MakeBinaryTable(DataTable partition, int cutIndex, ref int splitValue)
        {
            splitValue = (int)((int.Parse((partition.Select($"ORDERED_ID = {cutIndex}")[0].ItemArray[1]).ToString()) + int.Parse(partition.Select($"ORDERED_ID = {cutIndex+1}")[0].ItemArray[1].ToString()))/2);

            partition.Columns.Add("DISCRETISED", typeof(int), $"IIF ({partition.Columns[1].ColumnName} >= {splitValue} , 1, 0)");
            //partition.Columns.Remove(partition.Columns[1]);

            return partition;


        }


        /// <summary>
        /// Returns the entropy for a partition supplied as a parameter and updates the count variable of unique result classes for the specified partition passed by reference. We define class entropy to be, for a dataset S containing a class set C: 
        /// Ent(S)=-∑_(r= 1)^|C|▒〖P(C_r|S)*log_2⁡P(C_r| S) 〗
        /// We first determine the number of unique result values in the partition and then determine and store a count of the number of records in the partition with each unique result value.Using these counts, we calculate the probability of a record in the dataset having a particular class as its result.We find the Shannon information for the event as being -log_2⁡P(C_r "| " S), taking a logarithm in base 2 to make the units of our information measure in bits(number of bits required to represent the event). To determine entropy, for each possible result class we multiply the probability of the event that a record’s result is of that class by the information of the event; this is summed over each possible result class and returned as the entropy of the partition.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="classCount"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns list of boundary records ORDERED_ID field values I.e. the IDs of records in the ordered partition such that the subsequent record in the ordered partition has a different result class. Iterates over partition, comparing result class value with a last-seen value, adding the value of the ORDERED_ID field of the previous record to an index list if the result class value of the current record does not match the last-seen value. Once all rows of partition examined, index list returned. 
        /// </summary>
        /// <param name="partition">Dataset to find boundary indices for</param>
        /// <returns></returns>
        private List<int> FindBoundaryIndexes(DataTable partition)
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
        ///Returns a list of smaller data tables split by need. Fields on smaller data tables for the ID of a record, the continuous need value for each need as stored in the record, and the need name result column.
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
