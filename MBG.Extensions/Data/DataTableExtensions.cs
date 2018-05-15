using System;
using System.Data;
using System.Text;
using MBG.Extensions.Core;

namespace MBG.Extensions.Data
{
    public static class DataTableExtensions
    {
        public static bool ToCsv(this DataTable table, string filePath)
        {
            return table.ToCsv(filePath, true);
        }
        public static bool ToCsv(this DataTable table, string filePath, bool outputColumnNames)
        {
            bool ok = false;
            StringBuilder sb = new StringBuilder(2000);

            #region Column Names
            if (outputColumnNames)
            {
                foreach (DataColumn column in table.Columns)
                {
                    sb.Append(column.ColumnName);
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(Environment.NewLine);
            }
            #endregion

            #region Rows (Data)
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    sb.Append(row[column].ToString());
                    sb.Append(',');
                }
                //Remove Last ','
                sb.Remove(sb.Length - 1, 1);
                sb.Append(Environment.NewLine);
            }
            #endregion

            ok = sb.ToString().ToFile(filePath);

            return ok;
        }
    }
}