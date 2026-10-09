using OdontoSoft.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Data.DAL
{
    internal class DbDataInitializer
    {
        public static void Seed(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<DocumentoTipo>()
            //    .HasData(
            //        new DocumentoTipo { Id = 1, Codigo = "DNI", Descripcion = "DNI" },
            //        new DocumentoTipo { Id = 2, Codigo = "CUIT", Descripcion = "CUIT" },
            //        new DocumentoTipo { Id = 3, Codigo = "CUIL", Descripcion = "CUIL" },
            //        new DocumentoTipo { Id = 4, Codigo = "CDI", Descripcion = "CDI" },
            //        new DocumentoTipo { Id = 5, Codigo = "LE", Descripcion = "LE" },
            //        new DocumentoTipo { Id = 6, Codigo = "LC", Descripcion = "LC" },
            //        new DocumentoTipo { Id = 7, Codigo = "PASAPORTE", Descripcion = "PASAPORTE" }
            //);


            ////Color
            //modelBuilder.Entity<Color>()
            //    .HasData(
            //       new Color { Id=2, Descripcion = "Blanco" },
            //       new Color { Id = 1, Descripcion = "Negro" },
            //       new Color { Id = 3, Descripcion = "Gris plata" },
            //       new Color { Id = 4, Descripcion = "Gris oscuro" },
            //       new Color { Id = 5, Descripcion = "Bordó" },
            //       new Color { Id = 6, Descripcion = "Rojo" },
            //       new Color { Id = 7, Descripcion = "Azul" }
            //);

            ////Sucursal
            //modelBuilder.Entity<Sucursal>()
            //    .HasData(
            //       new Sucursal { Id = 1, Nombre = "Sucursal 1", NombreCorto="Suc 1" },
            //       new Sucursal { Id = 2, Nombre = "Sucursal 2", NombreCorto = "Suc 2" },
            //       new Sucursal { Id = 3, Nombre = "Sucursal 3", NombreCorto = "Suc 3" },      
            //       new Sucursal { Id = 4, Nombre = "Sucursal 4" , NombreCorto = "Suc 4" }
            //);            

            ////Perfil
            //modelBuilder.Entity<Perfil>()
            //    .HasData(
            //       new Perfil { Id = 1, Descripcion = "UsuarioInterno", Codigo="INTERNO" },
            //       new Perfil { Id = 2, Descripcion = "Cliente", Codigo = "CLIENTE" }
            //);


        }
    }
}
