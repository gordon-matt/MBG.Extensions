using System;
using System.Data;

namespace MBG.Extensions.Data
{
    [Serializable]
    public sealed class ColumnInfo
    {
        public string Name { get; set; }
        public SqlDbType Type { get; set; }
        public bool IsForeignKeyColumn { get; set; }
        public bool IsRequired { get; set; }
        public int MaxLength { get; set; }
        public string DefaultValue { get; set; }
    }
}