using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.Models.SQlitecontextDb
{
    internal class TempData
    {


        public  TempData()
        {
            //List<Material> material = new List<Material>()
            //{
            //    new Material()
            //    {
            //        Name = "Kumaş",
            //      Type = "M",
            //    },
            //    new Material()
            //    {
            //        Name = "Polar",
            //      Type = "M",
            //    },
            //      new Material()
            //    {
            //        Name = "Elyaf",
            //      Type = "M",
            //    },
            //        new Material()
            //    {
            //        Name = "Kol Astar",
            //      Type = "M",
            //    },
            //             new Material()
            //    {
            //        Name = "Dolum",
            //      Type = "GR",
            //    },
            //      new Material()
            //    {
            //        Name = "Çırt",
            //      Type = "M",
            //    },
            //       new Material()
            //    {
            //        Name = "Çıtçıt",
            //      Type = "Adet",
            //    },
            //          new Material()
            //    {
            //        Name = "Ribana",
            //      Type = "Adet",
            //    },
            //             new Material()
            //    {
            //        Name = "Kemer Başlık",
            //      Type = "Adet",
            //    },
            //    new Material()
            //    {
            //        Name = "Paket",
            //      Type = "Adet",
            //    },
            //      new Material()
            //    {
            //        Name = "Koli",
            //      Type = "Adet",
            //    },

            //      new Material()
            //    {
            //        Name = "İç cep fermuar",
            //      Type = "Adet",
            //    },
            //    new Material()
            //    {
            //        Name = "Jelatin",
            //      Type = "Adet",
            //    },
            //     new Material()
            //    {
            //        Name = "Etiket (Kart/Sticker/Yıkama)",
            //      Type = "Adet",
            //    },
            //      new Material()
            //    {
            //        Name = "Kart ipi",
            //      Type = "Adet",
            //    },
            //       new Material() {
            //         Name = "Ön fermuar",
            //      Type = "Adet",
            //    },

            // new Material() {
            //         Name = "Cep fermuar",
            //      Type = "Adet",
            //    },
            //     new Material() {
            //         Name = "8'lik prim",
            //      Type = "Adet",
            //    },
            //         new Material() {
            //         Name = "Kürk",
            //      Type = "Adet",
            //    },
            //             new Material() {
            //         Name = "Arma",
            //      Type = "Adet",
            //    },
            //                 new Material() {
            //         Name = "Nakış",
            //      Type = "Adet",
            //    },
            //                              new Material() {
            //         Name = "Baskı",
            //      Type = "Adet",
            //    },
            //          new Material() {
            //         Name = "Genel gider",
            //      Type = "Adet",
            //    },
            //                       new Material() {
            //         Name = "Depo gider",
            //      Type = "Adet",
            //    },
            //          new Material() {
            //         Name = "Fason İşçilik",
            //      Type = "Adet",
            //    },

            //};


            //foreach (var item in material)
            //{
            //    DatabaseService.CreateMetarial(item);
            //}

        }

        public static void InitializeMaterial()
        {
            List<Material> material = new List<Material>()
            {
                new Material()
                {
                    Name = "Kumaş",
                  Type = "M",
                },
                new Material()
                {
                    Name = "",
                  Type = "M",
                },
                  new Material()
                {
                    Name = "Elyaf",
                  Type = "M",
                },
                    new Material()
                {
                    Name = "Kol Astar",
                  Type = "M",
                },
                         new Material()
                {
                    Name = "Dolum",
                  Type = "GR",
                },
                  new Material()
                {
                    Name = "Çırt",
                  Type = "M",
                },
                   new Material()
                {
                    Name = "Çıtçıt",
                  Type = "Adet",
                },
                      new Material()
                {
                    Name = "Ribana",
                  Type = "Adet",
                },
                         new Material()
                {
                    Name = "Kemer Başlık",
                  Type = "Adet",
                },
                new Material()
                {
                    Name = "Paket",
                  Type = "Adet",
                },
                  new Material()
                {
                    Name = "Koli",
                  Type = "Adet",
                },

                  new Material()
                {
                    Name = "İç cep fermuar",
                  Type = "Adet",
                },
                new Material()
                {
                    Name = "Jelatin",
                  Type = "Adet",
                },
                 new Material()
                {
                    Name = "Etiket (Kart/Sticker/Yıkama)",
                  Type = "Adet",
                },
                  new Material()
                {
                    Name = "Kart ipi",
                  Type = "Adet",
                },
                   new Material() {
                     Name = "Ön fermuar",
                  Type = "Adet",
                },

             new Material() {
                     Name = "Cep fermuar",
                  Type = "Adet",
                },
                 new Material() {
                     Name = "8'lik prim",
                  Type = "Adet",
                },
                     new Material() {
                     Name = "Kürk",
                  Type = "Adet",
                },
                         new Material() {
                     Name = "Arma",
                  Type = "Adet",
                },
                             new Material() {
                     Name = "Nakış",
                  Type = "Adet",
                },
                                          new Material() {
                     Name = "Baskı",
                  Type = "Adet",
                },
                      new Material() {
                     Name = "Genel gider",
                  Type = "Adet",
                },
                                   new Material() {
                     Name = "Depo gider",
                  Type = "Adet",
                },
                      new Material() {
                     Name = "Fason İşçilik",
                  Type = "Adet",
                },

            };

            List<Material> materials = new List<Material>()
            {
                new Material() { Name = "Kumaş", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "İç kumaş", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Polar", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Şerpa", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Penye", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Elyaf", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Dolum", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Kapitone", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Kol astarı", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Polyester astar", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Ön fermuar", Type = "Adet" ,IsDeleted=false   },
                new Material() { Name = "Cep fermuar", Type = "Adet"  ,IsDeleted=false  },
                new Material() { Name = "İç cep fermuarı", Type = "Adet" ,IsDeleted=false   },
                new Material() { Name = "Elcik", Type = "Adet"  ,IsDeleted=false  },
                new Material() { Name = "Garni", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Çıtçıt", Type = "Adet"  ,IsDeleted=false  },
                new Material() { Name = "Cırt", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Ribana", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Arma", Type = "Adet"  ,IsDeleted=false  },
                new Material() { Name = "Nakış", Type = "Adet" ,IsDeleted=false   },
                new Material() { Name = "Baskı", Type = "Adet" ,IsDeleted=false   },
                new Material() { Name = "Grogren", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Kürk", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Stoper", Type = "Adet" ,IsDeleted=false   },
                new Material() { Name = "Stoper lastiği", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Kemer lastiği", Type = "M" ,IsDeleted=false   },
                new Material() { Name = "Ütü paket", Type = "Adet" ,IsDeleted=false   },
                new Material() { Name = "Etiket,jelatin,koli", Type = "Adet"  ,IsDeleted=false  },
                new Material() { Name = "Genel gider", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Depolama gideri", Type = "M"  ,IsDeleted=false  },
                new Material() { Name = "Brs yıllık prim", Type = "Adet"  ,IsDeleted=false  },
                new Material() { Name = "Fason işçiliği", Type = "Adet" ,IsDeleted=false   },
            };
            foreach (var item in materials)
            {
                 DatabaseService.CreateMetarial(item);
            }
        }
    }
}
