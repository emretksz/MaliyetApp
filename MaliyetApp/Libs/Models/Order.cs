using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models
{

    /// <summary>
    /// ilk oluşturulacak daha sonra sale eklenecek
    /// </summary>
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Creationtime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        [Ignore]
        public List<OrderDetails> OrderDetails { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModificationTime { get; set; } = DateTime.Now;
        public int SqliteId { get; set; }

    }
}
