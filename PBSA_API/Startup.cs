using DataService.Infrastructure;
using DataService.Models;
using DataService.Repositories;
using DataService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace PBSA_API
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
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //Add AutoMapper
            var autoMapperConfig = new AutoMapper.MapperConfiguration(cfg => {
                cfg.AddProfile(new AutoMapperProfile());
            });
            var mapper = autoMapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            //Add Swagger
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            //Add context and Transient for DI
            services.AddDbContext<PBSAContext>();

            //Repositories
            services.AddTransient<IAspNetUserRepository, AspNetUserRepository>();
            services.AddTransient<IBarberRepository, BarberRepository>();
            services.AddTransient<IBarberScheduleRepository, BarberScheduleRepository>();
            services.AddTransient<IBarberServiceRepository, BarberServiceRepository>();
            services.AddTransient<IBookingRepository, BookingRepository>();
            services.AddTransient<IBookingServiceRepository, BookingServiceRepository>();
            services.AddTransient<IBookingHistoryRepository, BookingHistoryRepository>();
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICustomerLocationRepository, CustomerLocationRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IDistrictRepository, DistrictRepository>();
            services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddTransient<IRefBookingStateRepository, RefBookingStateRepository>();
            services.AddTransient<IRefComplaintStateRepository, RefComplaintStateRepository>();
            services.AddTransient<IRefComplaintTypeRepository, RefComplaintTypeRepository>();
            services.AddTransient<IRefCurrencyCodeRepository, RefCurrencyCodeRepository>();
            services.AddTransient<IRefPaymentTypeRepository, RefPaymentTypeRepository>();
            services.AddTransient<IRefPhotoTypeRepository, RefPhotoTypeRepository>();
            services.AddTransient<IRefTransactionStateRepository, RefTransactionStateRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();

            //Services
            services.AddTransient<IBarberService, BarberService>();
            services.AddTransient<IBarberScheduleService, BarberScheduleService>();
            services.AddTransient<IBookingService, BookingService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IRefTablesService, RefTablesService>();
            services.AddTransient<IPaymentMethodService, PaymentMethodService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "PBSA");
            });

            app.UseMvc();
        }
    }
}
