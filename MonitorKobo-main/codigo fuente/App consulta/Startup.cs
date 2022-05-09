using App_consulta.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

using App_consulta.Models;
using App_consulta.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace App_consulta
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                new MariaDbServerVersion(new Version(10, 3, 24)),
                 mySqlOptions => mySqlOptions
                            .CharSetBehavior(CharSetBehavior.NeverAppend))
                .EnableSensitiveDataLogging()
                    .EnableDetailedErrors());


            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(
            //    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.AddControllersWithViews();



            services.AddAuthorization(options =>
            {
                options.AddPolicy("Configuracion.General", policy =>
                                  policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.General", "1", "2", "3", "4", "5"));
                options.AddPolicy("Configuracion.Logs", policy =>
                             policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Logs", "1"));
                options.AddPolicy("Configuracion.Responsable", policy =>
                              policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Configuracion.Responsable", "1"));
                options.AddPolicy("Responsable.Editar", policy =>
                                  policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Responsable.Editar", "1"));
                options.AddPolicy("Rol.Editar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Rol.Editar", "1"));
                options.AddPolicy("Usuario.Editar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Usuario.Editar", "1"));

                options.AddPolicy("Encuestador.Ver", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Ver", "1"));
                options.AddPolicy("Encuestador.Editar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Editar", "1"));
                options.AddPolicy("Encuestador.Administrar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestador.Administrar", "1"));


                options.AddPolicy("Encuestas.Actualizar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Actualizar", "1"));
                options.AddPolicy("Encuestas.Listado", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Listado", "1"));
                options.AddPolicy("Encuestas.Usuario", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Encuestas.Usuario", "1"));

                options.AddPolicy("Formalizacion.Ver", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Ver", "1"));
                options.AddPolicy("Formalizacion.Editar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Editar", "1"));
                options.AddPolicy("Formalizacion.Validar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Validar", "1"));
                options.AddPolicy("Formalizacion.Listado", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Listado", "1"));
                options.AddPolicy("Formalizacion.Imprimir", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imprimir", "1"));
                
                options.AddPolicy("Formalizacion.Cancelar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Cancelar", "1"));

                options.AddPolicy("Formalizacion.Imagen.Cambiar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imagen.Cambiar", "1"));
                options.AddPolicy("Formalizacion.Imagen.Restablecer", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Formalizacion.Imagen.Restablecer", "1"));

                options.AddPolicy("Exportar.Listado", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Exportar.Listado", "1"));

                options.AddPolicy("Solicitud.Crear", policy =>
                          policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Crear", "1"));
                options.AddPolicy("Solicitud.Administrar", policy =>
                                policy.RequireClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/Solicitud.Administrar", "1"));

            });


            services.AddTransient<IEmailSender, EmailSender>(i =>
               new EmailSender(
                   Configuration["EmailSender:Host"],
                   Configuration.GetValue<int>("EmailSender:Port"),
                   Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                   Configuration["EmailSender:UserName"],
                   Configuration["EmailSender:Password"],
                   Configuration["EmailSender:displayName"]
               )
           );

          services.AddRazorPages().AddMvcOptions(
              options =>{
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                    _ => "Este campo es obligatorio. ");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
