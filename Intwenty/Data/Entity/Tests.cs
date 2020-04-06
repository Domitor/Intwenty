using Intwenty.Data.DBAccess.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Data.Entity
{
    [DbTableName("TestEntityOne")]
    public class TestEntity
    {
        [AutoIncrement]
        public int Id { get; set; }

        [DbColumnName("NameX")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

    }

    [DbTableIndex("TestEntitiy2_IDX_1", false, "NameX")]
    [DbTablePrimaryKey("Id")]
    [DbTableName("TestEntityTwo")]
    public class TestEntity2
    {
        public int Id { get; set; }

        [DbColumnName("NameX")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }

    }
}
