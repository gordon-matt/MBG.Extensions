using System.Collections.Generic;
using System.Linq;

namespace MBG.Extensions.Data
{
    public sealed class ForeignKeyInfo
    {
        public string ForeignKeyTable { get; set; }
        public string ForeignKeyColumn { get; set; }
        public string PrimaryKeyTable { get; set; }
        public string PrimaryKeyColumn { get; set; }
        public string ConstraintName { get; set; }

        public ForeignKeyInfo() { }
        public ForeignKeyInfo(string fkTable, string fkColumn, string pkTable, string pkColumn, string constraintName)
        {
            ForeignKeyTable = fkTable;
            ForeignKeyColumn = fkColumn;
            PrimaryKeyTable = pkTable;
            PrimaryKeyColumn = pkColumn;
            ConstraintName = constraintName;
        }

        public override string ToString()
        {
            return ConstraintName;
        }
    }
    public sealed class ForeignKeyInfoCollection : List<ForeignKeyInfo>
    {
        public bool Contains(string fkColumnName)
        {
            foreach (string fkColumn in this.Select(x => x.ForeignKeyColumn))
            {
                if (fkColumn == fkColumnName)
                { return true; }
            }
            return false;
        }
    }
}