using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models
{
    public class OrderDetails
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SaleId { get; set; }
        [Ignore]
        public Sale Sale { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModificationTime { get; set; } = DateTime.Now;
        public int SqliteId { get; set; }

    }
}
