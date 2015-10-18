using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Collections;

namespace RecursiveAdd
{
    class Program
    {

        static void PrintList(List<List<String>> inputListList)
        {
            List<String> inputList;
            for (int i = 0; i < inputListList.Count(); i++)
            {
                inputList = inputListList.ElementAt(i);
                for (int j = 0; j < inputList.Count(); j++)
                    Console.Write(inputList.ElementAt(j) + " ");
                Console.WriteLine("\n\n");
            } 
        }

        static void ProcessComponent(List<List<String>> listlist, String kanji, String component)
        {
            List<String> list, addList = new List<String>();
            Boolean found;

            for (int i = 0; i < listlist.Count(); i++)
            {
                list = listlist.ElementAt(i);

                if (String.Compare(list.ElementAt(0), component) == 0)
                {
                    for (int j = 1; j < list.Count(); j++)
                    {
                        ProcessComponent(listlist,kanji,list.ElementAt(j));
                        addList.Add(list.ElementAt(j));
                    }
                }
            }

            for (int i = 0; i < listlist.Count(); i++)
            {
                list = listlist.ElementAt(i);

                if (String.Compare(list.ElementAt(0), kanji) == 0)
                {
                    for (int j = 0; j < addList.Count(); j++)
                    {
                        found = false;
                        for (int k = 1; k < list.Count(); k++){
                            if (String.Compare((String)list.ElementAt(k),(String)addList.ElementAt(j))==0){
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            list.Add(addList.ElementAt(j));
                            // This would be the sp call to insert data into the db
                    }
                }
            }
        }

        static void ProcessList(List<List<String>> listList)
        {
            List<String> list;
            for (int i = 0; i < listList.Count(); i++)
            {
                list = listList.ElementAt(i);
                for (int j = 1; j < list.Count(); j++)
                {
                    ProcessComponent(listList, list.ElementAt(0),list.ElementAt(j));
                }
            }
        }

        static void Main(string[] args)
        {
            // Need to supply connection information string
            // String connection = "Data Source=<server>;Initial Catalog=<db_name>;User ID=<user_id>;Password=<pw>";
            List<List<String>> list = new List<List<String>>();
            List<String> newList;
            Boolean headFound, charFound;
            String colChar;

            /* Populate data */
            using (SqlConnection con = new SqlConnection(connection)){
                con.Open();
                using (SqlCommand cmd = new SqlCommand("sp_get_information", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@in_table_name",SqlDbType.NVarChar).Value = "tblTestKanji";

                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        headFound = false;
                        colChar = (String) read.GetValue(0); // kanji

                        for (int i = 0; i < list.Count(); i++)
                        {
                            List<String> subList = list.ElementAt(i);

                            /* Found element */
                            if (String.Compare(subList.ElementAt(0),colChar)==0)
                            {
                                /* Check for existing element */
                                headFound = true;
                                charFound = false;
                                for (int j = 1; j < subList.Count(); j++)
                                {
                                    if (String.Compare(subList.ElementAt(j),(String)read.GetValue(1))==0)
                                    {
                                        charFound = true;
                                        break;
                                    }
                                }
                                if (!charFound)
                                {
                                    subList.Add((String)read.GetValue(1));
                                }
                            }
                        }// end for-loop

                        if (!headFound)
                        {
                            newList = new List<String>();
                            newList.Add((String)read.GetValue(0));
                            newList.Add((String)read.GetValue(1));
                            list.Add(newList);
                        }
                    }
                }
            }
            PrintList(list);
            ProcessList(list);
            Console.WriteLine("Recursively processed list:");
            PrintList(list);
            Console.WriteLine();
        }
    }
}
