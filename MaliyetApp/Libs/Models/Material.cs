using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models
{
    public class Material
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// METRE
        /// GRAM
        /// ADET
        /// </summary>
        public string Type { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? LastModificationTime { get; set; } = DateTime.Now;

        public int SqliteId { get; set; }

    }
}
