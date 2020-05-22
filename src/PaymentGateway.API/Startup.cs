using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentGateway.Data.Context;
using PaymentGateway.Domain.Banking;
using PaymentGateway.Domain.Banking.Interfaces;
using PaymentGateway.Domain.DataEncryption;
using PaymentGateway.Domain.DataEncryption.Interfaces;
using PaymentGateway.Domain.PaymentProcessing;
using PaymentGateway.Domain.PaymentProcessing.Interfaces;
using PaymentGateway.Domain.PaymentRepository;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using Refit;

namespace PaymentGateway.API
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
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payment Gateway", Version = "v1" });
            });

            services.AddDbContext<PaymentContext>(options => options.UseInMemoryDatabase(databaseName: "ContextDatabase"));

            services.AddScoped<IBankingHandler, BankingHandler>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentHandler, PaymentHandler>();
            services.AddSingleton<IDataEncryptor>(x =>
                new DataEncryptor(Configuration.GetSection("Encryption:PublicKey").Value,
                    Configuration.GetSection("Encryption:PrivateKey").Value));

            services.AddRefitClient<IAcquirerBankingService>().ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("Apis:AcquiringBankApi:Url").Value));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseElmahIoExtensionsLogging();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
