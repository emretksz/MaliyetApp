using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// marka
        /// </summary>
        public string? Brand { get; set; }


        //ürün resmi
        //Model Numarası
        //Yaş/Size
        //Model Açıklaması
        //iç kumaş Adı
        //iç kumaş içeriği
        //iç kumaş tedariği
        //dış kumaş Adı
        //dış kumaş içeriği
        //dış kumaş tedariği


        public string? ModelNumber { get; set; }
        /// <summary>
        /// model açıklaması gelen değere göre bakarım
        /// </summary>
        public string? ModelDescription { get; set; }

        /// <summary>
        /// model açıklaması gelen değere göre bakarım
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Size = true beden
        /// Size = false yaş
        /// </summary>
        public bool Size { get; set; }
        public string? YasSize { get; set; }
        /// <summary>
        ///  İÇ KUMAŞ
        /// </summary>
        public string? InnerFabricName{ get; set; }
        public string? InnerFabricContent{ get; set; }
        public string ?InnerFabricSupply { get; set; }


        /// <summary>
        /// Dış kumaş
        /// </summary>
        public string ?OuterFabricName { get; set; }
        public string? OuterFabricContent { get; set; }
        public string ?OuterFabricSupply { get; set; }


        /// <summary>
        /// oluşturma zamanı
        /// </summary>
        public string Creationtime { get; set; } =  DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);


        /// <summary>
        /// RESİM
        /// </summary>
        public string ImageURL { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTime? LastModificationTime { get; set; } = DateTime.Now;
        public bool IsUpdate { get; set; } = false;
        public int SqliteId { get; set; }

    }
}
