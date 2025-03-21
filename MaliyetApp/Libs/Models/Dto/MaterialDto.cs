using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models.NewFolder
{
    public class MaterialDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Type { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
