using App_consulta.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace App_consulta.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Responsable> Responsable { get; set; }

        public DbSet<Policy> Policy { get; set; }

        public DbSet<Configuracion> Configuracion { get; set; }

        public DbSet<LogModel> Log { get; set; }

        public DbSet<LocationLevel> LocationLevel { get; set; }

        public DbSet<Location> Location { get; set; }

        public DbSet<Pollster> Pollster { get; set; }

        public DbSet<RequestUser> RequestUser { get; set; }

        public DbSet<FormalizationConfig> FormalizationConfig { get; set; }
        public DbSet<FormalizationVariable> FormalizationVariable { get; set; }

        public DbSet<Formalization> Formalization { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Tabla de politicas
            var policies = new List<Policy>
            {
                new Policy() { id = 1, nombre = "Ver Configuración general", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.General", group = 1 },
                new Policy() { id = 2, nombre = "Configuración dependencia", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Responsable", group = 1},
                new Policy() { id = 3, nombre = "Editar dependencias", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", group = 2 },
                new Policy() { id = 4, nombre = "Editar roles", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", group = 2 },
                new Policy() { id = 5, nombre = "Editar usuarios", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", group = 2 },
                new Policy() { id = 6, nombre = "Ver registro actividad", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", group = 2 },

                new Policy() { id = 8, nombre = "Ver encuestador", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Ver", group = 3 },
                new Policy() { id = 7, nombre = "Editar encuestador", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Editar", group = 3 },
                new Policy() { id = 9, nombre = "Administrar encuestador", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Administrar", group = 3 },


                new Policy() { id = 10, nombre = "Actualizar encuestas", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Actualizar", group = 4},
                new Policy() { id = 11, nombre = "Informe encuestas", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Listado", group = 4 },
                new Policy() { id = 16, nombre = "Ver encuestas por usuario", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Usuario", group = 4 },


                new Policy() { id = 12, nombre = "Ver formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Ver", group = 5  },
                new Policy() { id = 13, nombre = "Editar formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Editar", group = 5  },
                new Policy() { id = 14, nombre = "Validar formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Validar", group = 5  },
                new Policy() { id = 15, nombre = "Informe formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Listado", group = 5  },
                new Policy() { id = 17, nombre = "Imprimir formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imprimir", group = 5  },

                new Policy() { id = 18, nombre = "Exportar listados", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Exportar.Listado", group = 6  },

                new Policy() { id = 19, nombre = "Cambiar imagenes formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imagen.Cambiar", group = 5  },
                new Policy() { id = 20, nombre = "Restablecer imagenes formalización", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imagen.Restablecer", group = 5  },

                new Policy() { id = 21, nombre = "Crear solicitudes", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Crear", group = 7  },
                new Policy() { id = 22, nombre = "Administrar solicitudes", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Administrar", group = 7  },
                };
            modelBuilder.Entity<Policy>().HasData(policies);

            //Rol administrador
            var rol = new ApplicationRole() { Id = "1", ConcurrencyStamp = "97f6ff5b-6816-44fc-8e6f-bbdedd1223f9", Name = "Administrador", NormalizedName = "ADMINISTRADOR" };
            modelBuilder.Entity<ApplicationRole>().HasData(rol);

            //Permisos rol administrador
            var policiesRol = new List<IdentityRoleClaim<string>>
            {
                new IdentityRoleClaim<string>() { Id =1, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.General", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =2, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Responsable", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =3, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =4, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =5, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =6, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =7, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Editar", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =8, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Ver", ClaimValue = "1"},
                new IdentityRoleClaim<string>() { Id =9, RoleId = "1", ClaimType="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Administrar", ClaimValue = "1"},
                };
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(policiesRol);

            //Entidad por defecto
            var responsable = new Responsable() { Id = 1, Nombre = "Entidad", Editar = true };
            modelBuilder.Entity<Responsable>().HasData(responsable);

            //Usuario administrador
            var user = new ApplicationUser()
            {
                Id = "1",
                Nombre = "Admin",
                Apellido = "",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                IDDependencia = 1,
                LockoutEnabled = false,
                PasswordHash = "AQAAAAEAACcQAAAAECPDxHYYnrFlyL6ghv6NFqs7g9ZlRCuHRIgzChzRa5GDZpnwsj563VfwncgzZt+OTw==",
                SecurityStamp = "NNK44MKHKTBOV6DHXJ4BT2Q3SYO3WQC2",
                ConcurrencyStamp = "05622443-5cfd-4389-8879-4523ac4c5aee"
            };
            modelBuilder.Entity<ApplicationUser>().HasData(user);

            //Roles del usuario administrador
            var userRol = new IdentityUserRole<string>() { RoleId = "1", UserId = "1" };
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(userRol);


            //Configuración inicial
            var config = new Configuracion()
            {
                id = 1,

                Logo = "/images/SIE.png",
                contacto = "rinconsebastian@gmail.com",
                activo = true,
                Entidad = "Entidad",
                NombrePlan = "Plan",
                libre = true,
                colorPrincipal = "#52a3a1",
                colorTextoHeader = "#ffffff",
                colorTextoPrincipal = "#00000"
            };
            modelBuilder.Entity<Configuracion>().HasData(config);

            modelBuilder.Entity<Pollster>().HasIndex(u => u.DNI).IsUnique();

            //Permisos rol administrador
            var formalizacionConfigs = new List<FormalizationConfig>
            {
                new FormalizationConfig() { Id = 1, Name="Id Kobo", Field = "IdKobo", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 2, Name="Número registro", Field = "NumeroRegistro", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 3, Name="Nombre y apellidos", Field = "Name", Value = "", Group = 1},
                new FormalizationConfig() { Id = 4, Name="Fecha solicitud", Field = "FechaSolicitud", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 5, Name="Cédula pescador" ,Field = "Cedula", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 6, Name="Departamento", Field = "Departamento", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 7, Name="Municipio", Field = "Municipio", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 8, Name="Área de pesca", Field = "AreaPesca", Value = "_id", Group = 1},
                new FormalizationConfig() { Id = 9, Name="Artes de pesca", Field = "ArtesPesca", Value = "_id"},
                new FormalizationConfig() { Id =10, Name="Nombre asociación", Field = "NombreAsociacion", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =11, Name="Foto pescador", Field = "ImgPescador", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =12, Name="Foto solicitud carnet", Field = "ImgSolicitudCarnet", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =13, Name="Foto certificación", Field = "ImgCertificacion", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =14, Name="Foto cédula (anverso)", Field = "ImgCedulaAnverso", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =15, Name="Foto cédula (reverso)", Field = "ImgCedulaReverso", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =16, Name="Firma digital", Field = "ImgFirmaDigital", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =17, Name="Cedula Encuestador", Field = "Encuestador", Value = "_id", Group = 1},
                new FormalizationConfig() { Id =18, Name="Zona pesca (Tipo)", Field = "zonaTipo", Value = "_id", Group = 2},
                new FormalizationConfig() { Id =19, Name="Zona pesca (Nombre)", Field = "zonaNombre", Value = "_id", Group = 2},
                new FormalizationConfig() { Id =20, Name="Zona pesca (Otro)", Field = "zonaOtro", Value = "_id", Group = 2},
                };
            modelBuilder.Entity<FormalizationConfig>().HasData(formalizacionConfigs);


            //Variables de formalización: zonas y artes de pesca
            var variables = new List<FormalizationVariable>
            {
                new FormalizationVariable() { Id = 1, Key="1", Value = "Arroyo", Group = "Zona"},
                new FormalizationVariable() { Id = 2, Key="2", Value = "Canal", Group = "Zona"},
                new FormalizationVariable() { Id = 3, Key="3", Value = "Ciénaga", Group = "Zona"},
                new FormalizationVariable() { Id = 4, Key="4", Value = "Estanque", Group = "Zona"},
                new FormalizationVariable() { Id = 5, Key="5", Value = "Laguna", Group = "Zona"},
                new FormalizationVariable() { Id = 6, Key="6", Value = "Lago", Group = "Zona"},
                new FormalizationVariable() { Id = 7, Key="7", Value = "Presa", Group = "Zona"},
                new FormalizationVariable() { Id = 8, Key="8", Value = "Quebrada", Group = "Zona"},
                new FormalizationVariable() { Id = 9, Key="9", Value = "Riachuelo", Group = "Zona"},
                new FormalizationVariable() { Id = 10, Key="10", Value = "Río", Group = "Zona"},
                new FormalizationVariable() { Id = 11, Key="11", Value = "Sector de río", Group = "Zona"},
                new FormalizationVariable() { Id = 12, Key="14", Value = "Embalse", Group = "Zona"},

                new FormalizationVariable() { Id = 13, Key="1", Value = "Arpón", Group = "Arte"},
                new FormalizationVariable() { Id = 14, Key="2", Value = "Atarraya", Group = "Arte"},
                new FormalizationVariable() { Id = 15, Key="3", Value = "Boliche", Group = "Arte"},
                new FormalizationVariable() { Id = 16, Key="4", Value = "Chinchorra", Group = "Arte"},
                new FormalizationVariable() { Id = 17, Key="5", Value = "Chinchorro", Group = "Arte"},
                new FormalizationVariable() { Id = 18, Key="6", Value = "Cóngolo / canasta", Group = "Arte"},
                new FormalizationVariable() { Id = 19, Key="7", Value = "Línea de mano", Group = "Arte"},
                new FormalizationVariable() { Id = 20, Key="8", Value = "Palangre", Group = "Arte"},
                new FormalizationVariable() { Id = 21, Key="9", Value = "Redes de enmalle", Group = "Arte"},
                new FormalizationVariable() { Id = 22, Key="10", Value = "Trampas/nasas", Group = "Arte"},
                new FormalizationVariable() { Id = 23, Key="11", Value = "Trasmallo", Group = "Arte"},
                new FormalizationVariable() { Id = 24, Key="12", Value = "Otro", Group = "Arte"},
                };
            modelBuilder.Entity<FormalizationVariable>().HasData(variables);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(true);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
