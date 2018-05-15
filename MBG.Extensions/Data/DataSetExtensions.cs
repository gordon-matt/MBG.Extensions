using System.Data;
using System.IO;

namespace MBG.Extensions.Data
{
    public static class DataSetExtensions
    {
        public static void ToCsv(this DataSet dataSet, string directoryPath)
        {
            dataSet.ToCsv(directoryPath, true);
        }
        public static void ToCsv(this DataSet dataSet, string directoryPath, bool outputColumnNames)
        {
            string tableName = string.Empty;
            int tableCount = 0;

            foreach (DataTable table in dataSet.Tables)
            {
                if (!string.IsNullOrEmpty(table.TableName))
                {
                    tableName = table.TableName;
                }
                else
                { tableName = string.Concat("Table", tableCount++); }

                table.ToCsv(Path.Combine(directoryPath, string.Concat(tableName, ".csv")), outputColumnNames);
            }
        }

        //public static bool SaveToCsv(this DataSet dataSet, string filePath, bool outputColumnNames)
        //{
        //    bool ok = false;
        //    StringBuilder sb = new StringBuilder(2000);
        //    if (dataSet.Tables.Count > 0)
        //    {
        //        foreach (DataTable table in dataSet.Tables)
        //        {
        //            #region Column Names
        //            if (outputColumnNames)
        //            {
        //                foreach (DataColumn column in table.Columns)
        //                {
        //                    sb.Append(column.ColumnName);
        //                    sb.Append(',');
        //                }
        //                sb.Remove(sb.Length - 1, 1);
        //                sb.Append(Environment.NewLine);
        //            }
        //            #endregion

        //            #region Rows (Data)
        //            foreach (DataRow row in table.Rows)
        //            {
        //                foreach (DataColumn column in table.Columns)
        //                {
        //                    sb.Append(row[column].ToString());
        //                    sb.Append(',');
        //                }
        //                //Remove Last ','
        //                sb.Remove(sb.Length - 1, 1);
        //                sb.Append(Environment.NewLine);
        //            }
        //            #endregion
        //        }

        //        ok = sb.ToString().ToFile(filePath);
        //    }

        //    return ok;
        //}
    }
}