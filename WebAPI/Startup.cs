using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.Cursos;
using AutoMapper;
using Dominio;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistencia;
using Persistencia.DapperConexion;
using Persistencia.DapperConexion.Instructor;
using Persistencia.DapperConexion.Paginacion;
using Seguridad.TokenSeguridad;
using WebAPI.Middleware;

namespace WebAPI
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
            //==== Creamos el Servicio de DbContext ====
            services.AddDbContext<CursosOnlineContext>(opt => {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //===============================

            //==== Creamos el Servicio de Dapper ====
            //Esta libreria nos sirve para trabajar con stored procedure de mejor manera que con EF
            services.AddOptions();
            services.Configure<ConexionConfiguracion>(Configuration.GetSection("ConnectionStrings"));
            
            //Configuro el servicio de FactoryConnection
            services.AddTransient<IFactoryConnection, FactoryConnection>();
            //Configuro el servicio de InstructorRepositorio
            services.AddScoped<IInstructor, InstructorRepositorio>();
            //===============================

            //==== Creamos el Servicio de IMediator ====
            services.AddMediatR(typeof(Consulta.Manejador).Assembly);
            //===============================

            //==== Creamos el Servicio de JWT ====
            services.AddScoped<IJwtGenerador, JwtGenerador>();
            //===============================

            //==== Creamos el Servicio de Autentificacion para JWT====
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, //Cualquier request tiene que ser validado
                    IssuerSigningKey = key, //Palabra secreta
                    ValidateAudience = false, //Quien puede crear los Token (Ip especificas)
                    ValidateIssuer = false //A quien podemos enviar los Token
                };
            });
            //===============================

            //==== Creamos el Servicio de UsuarioSesion ====
            services.AddScoped<IUsuarioSesion, UsuarioSesion>();
            //===============================

            //==== Creamos el Servicio para AutoMapper ====
            services.AddAutoMapper(typeof(Consulta.Manejador));
            //===============================

            //==== Creamos el Servicio para Swagger ====
            //Esta libreria nos sirve para documenta APIs
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Services para mantenimiento de cursos",
                    Version = "v1"
                });
                c.CustomSchemaIds(c => c.FullName); //Indico que trabaje con la ruta completa de la clase
            });
            //===============================

            //==== Creamos el Servicio para Paginacion ====
            services.AddScoped<IPaginacion, PaginacionRepositorio>();
            //===============================

            //==== Agregamos el FluentValidation, Core Identity y Authorize ====
            //Configuro que los metodos de los controller tengan la autorizacion (esten logueados), antes de procesar un request de un cliente
            //Hacemos la configuracion para indicar que archivo debe validar
            //Configuramos el DbContext Roles y administrador de accesos
            services.AddControllers(opt => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
               .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());
            var builder = services.AddIdentityCore<Usuario>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            identityBuilder.AddEntityFrameworkStores<CursosOnlineContext>(); //El contexto
            identityBuilder.AddSignInManager<SignInManager<Usuario>>(); //Administrador de acceso
            services.TryAddSingleton<ISystemClock, SystemClock>();
            //===============================
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //==== Agrego mi Middleware ====
            app.UseMiddleware<ManejadorErrorMiddleware>();
            //===============================

            //Si esta en ambiente de desarrollo
            if (env.IsDevelopment())
            {
                //Me da mayor detalle de las excepciones que puedan ocurrir
                //app.UseDeveloperExceptionPage();
            }

            //HTTPS, necesita un certificado de seguridad que se necesita para el ambiente de Produccion
            //En ambiente de desarrollo no hace falta
            //app.UseHttpsRedirection();

            app.UseAuthentication(); //Para usar autentificacion

            app.UseRouting();

            app.UseAuthorization();

            //==== Agrego Middleware para Swagger ====
            //Para habilitar la interfaz grafica, genera una pagina web
            app.UseSwagger();

            app.UseSwaggerUI(c => {
                //Configuro el EndPoint para ver la pagina web
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursos Online v1");

            });
            //===============================

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
