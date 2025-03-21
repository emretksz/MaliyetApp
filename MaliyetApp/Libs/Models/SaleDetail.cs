using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaliyetApp.Libs.AppSettings.AppConst;

namespace MaliyetApp.Libs.Models
{
    public class SaleDetail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        public int SaleId { get; set; }
        //[Ignore]
        //public Sale Sale { get; set; }

        public int MaterialId { get; set; }

        [Ignore]
        public Material Material { get; set; }

        /// <summary>
        /// BİRİM
        /// </summary>
        public float Unit { get; set; }

        /// <summary>
        /// BİRİM FİYAT
        /// </summary>
        public float UnitePrice { get; set; }

        /// <summary>
        /// KUR
        /// </summary>
        public float MarketRate { get; set; }

        /// <summary>
        /// Alınan Fiyat
        /// </summary>
        public float Price { get; set; }

        /// <summary>
        /// KUR TİPİ
        /// </summary>
        public string? MarketRateType { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime? LastModificationTime { get; set; } = DateTime.Now;
        public int SqliteId { get; set; }


    }
}
