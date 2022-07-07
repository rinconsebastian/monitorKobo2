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

        public DbSet<KoProject> KoProject { get; set; }
        public DbSet<KoField> KoField { get; set; }
        public DbSet<KoVariable> KoVariable { get; set; }
        public DbSet<KoState> KoDataState { get; set; }

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


                new Policy() { id = 12, nombre = "Ver registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Ver", group = 5  },
                new Policy() { id = 13, nombre = "Editar registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Editar", group = 5  },
                new Policy() { id = 14, nombre = "Validar registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Validar", group = 5  },
                new Policy() { id = 15, nombre = "Informe registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Listado", group = 5  },
                new Policy() { id = 17, nombre = "Imprimir registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imprimir", group = 5  },
                new Policy() { id = 23, nombre = "Carcelar registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Cancelar", group = 5  },

                new Policy() { id = 18, nombre = "Exportar listados", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Exportar.Listado", group = 6  },

                new Policy() { id = 19, nombre = "Cambiar imagenes registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imagen.Cambiar", group = 5  },
                new Policy() { id = 20, nombre = "Restablecer imagenes registro", claim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imagen.Restablecer", group = 5  },

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
                 new IdentityRoleClaim<string>() { Id =1, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.General", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =2, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Responsable", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =3, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =4, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =5, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =6, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =7, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Ver", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =8, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Editar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =9, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Administrar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =10, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Actualizar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =11, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Listado", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =12, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Usuario", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =13, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Ver", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =14, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Editar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =15, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Validar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =16, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Listado", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =17, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imprimir", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =18, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Exportar.Listado", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =19, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imagen.Cambiar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =20, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Imagen.Restablecer", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =21, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Crear", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =22, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Administrar", ClaimValue = "1"},
                 new IdentityRoleClaim<string>() { Id =23, RoleId = "1", ClaimType= "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Registro.Cancelar", ClaimValue = "1"},
                };
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasData(policiesRol);

            //Entidad por defecto
            var responsables = new List<Responsable>(){
                new Responsable() { Id = 1, Nombre = "Entidad" },
                new Responsable() { Id = 2, Nombre = "[CDR] Coordinación", IdJefe = 1 },
            };

            modelBuilder.Entity<Responsable>().HasData(responsables);

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
                Id = 1,
                Logo = "/images/favicon1.png",
                Favicon = "/images/favicon.png",
                Entidad = "PNUD-AUNAP",
                NombreApp="App Consulta",
                DescripcionApp = "Acuerdo AUNAP - PNUD 2022",
                NombrePlan = "Coordinación",
                ColorPrincipal = "#4287F5",
                ColorTextoHeader = "#FFFFFF",
                ColorTextoPrincipal = "#0448B5",
                Contacto = "rinconsebastian@gmail.com",
            };
            modelBuilder.Entity<Configuracion>().HasData(config);

            modelBuilder.Entity<Pollster>().HasIndex(u => u.DNI).IsUnique();

            //Estados datos
            var dataStates = new List<KoState>
            {
                new KoState() { Id = 1, Class="", Label = "NO"},
                new KoState() { Id = 2, Class="", Label = "Pendiente"},
                new KoState() { Id = 3, Class="bg-warning", Label = "Borrador"},
                new KoState() { Id = 4, Class="bg-success", Label = "Completo", Print= true},
                new KoState() { Id = 5, Class="bg-danger", Label = "Cancelado"},
                new KoState() { Id = 6, Class="bg-info", Label = "Impreso", Print= true},
                new KoState() { Id = 7, Class="bg-danger", Label = "Carné vigente"},
                new KoState() { Id = 8, Class="bg-danger", Label = "Duplicado"},
                };
            modelBuilder.Entity<KoState>().HasData(dataStates);

            //Variables de registro: zonas y artes de pesca
            /*var variables = new List<KoVariable>
            {
                new KoVariable() { Id = 1, Key="1", Value = "Arroyo", Group = "Zona"},
                };
            modelBuilder.Entity<KoVariable>().HasData(variables);*/

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(true);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
