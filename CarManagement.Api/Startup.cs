// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using CarManagement.Api.Brokers.DateTimes;
using CarManagement.Api.Brokers.Loggings;
using CarManagement.Api.Brokers.Storages;
using CarManagement.Api.Services.Foundations.Addresss;
using CarManagement.Api.Services.Foundations.Cars;
using CarManagement.Api.Services.Foundations.CarModels;
using CarManagement.Api.Services.Foundations.CarTypes;
using CarManagement.Api.Services.Foundations.Categories;
using CarManagement.Api.Services.Foundations.DriverLicenses;
using CarManagement.Api.Services.Foundations.Offers;
using CarManagement.Api.Services.Foundations.OfferTypes;
using CarManagement.Api.Services.Foundations.Penalties;
using CarManagement.Api.Services.Foundations.Services;
using CarManagement.Api.Services.Foundations.ServiceTypes;
using CarManagement.Api.Services.Foundations.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using CarManagement.Api.Services.Foundations.Authorizations;
using CarManagement.Api.Services.Foundations.Authorizations.Models;
using CarManagement.Api.Services.Foundations.Security;

namespace CarManagement.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddOData(options =>
                    options.Select().Filter().OrderBy().Count().Expand());

            services.AddDbContext<StorageBroker>();

            services.AddCors(option =>
            {
                option.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Enter valid token"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                    Array.Empty<string>()
                    }
                });
            });

            AddSettingModels(services);
            AddBrokers(services);
            AddFoundationServices(services);
        }

        private void AddSettingModels(IServiceCollection services)
        {
            services.Configure<JwtSettings>(this.Configuration.GetSection(nameof(JwtSettings)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(config =>
                config.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "CarManagement.Api v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("MyPolicy");
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private static void AddBrokers(IServiceCollection services)
        {
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IAddressService, AddressService>();
            services.AddTransient<ICarService, CarService>();
            services.AddTransient<ICarModelService, CarModelService>();
            services.AddTransient<ICarTypeService, CarTypeService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IDriverLicenseService, DriverLicenseService>();
            services.AddTransient<IOfferService, OfferService>();
            services.AddTransient<IOfferTypeService, OfferTypeService>();
            services.AddTransient<IPenaltyService, PenaltyService>();
            services.AddTransient<IServiceService, ServiceService>();
            services.AddTransient<IServiceTypeService, ServiceTypeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAccessTokenGeneratorService, AccessTokenGeneratorService>();
            services.AddTransient<IPasswordHasherService, PasswordHasherService>();
        }
    }
}
