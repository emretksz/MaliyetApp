using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models
{
    public class BaseLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Method { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;

    }

    
}
