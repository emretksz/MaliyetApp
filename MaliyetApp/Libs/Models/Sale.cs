using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaliyetApp.Libs.AppSettings.AppConst;

namespace MaliyetApp.Libs.Models
{
    public class Sale
    {
        [PrimaryKey, AutoIncrement]

        public int Id { get; set; }
        public int ProductId { get; set; }
        [Ignore]
        public Product Product { get; set; }

        [Ignore]
        public ICollection<SaleDetail> SaleDetail { get; set; }
        public bool IsDeleted { get; set; } = false;

        public float GenelToplam { get; set; }
        public float SatisFiyati { get; set; }
        public float EldeEdilenKar { get; set; }
        public float KarOrani { get; set; }
        public string Creationtime { get; set; } =  DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        public DateTime? LastModificationTime { get; set; } = DateTime.Now;
        public int SqliteId { get; set; }


    }
}
