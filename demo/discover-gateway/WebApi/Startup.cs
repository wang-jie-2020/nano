using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            var consulConfig = new
            {
                IP = Configuration["consul:IP"],
                Port = int.Parse(Configuration["consul:port"]),
                ServiceName = Configuration["consul:serviceName"],
                ConsulIP = Configuration["consul:server_ip"],
                ConsulPort = int.Parse(Configuration["consul:server_port"])
            };

            var consulClient = new ConsulClient(x =>
                x.Address = new Uri($"http://{consulConfig.ConsulIP}:{consulConfig.ConsulPort}"));//����ע��� Consul ��ַ

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//����������ú�ע��
                Interval = TimeSpan.FromSeconds(10),//�������ʱ���������߳�Ϊ�������
                HTTP = $"http://{consulConfig.IP}:{consulConfig.Port}/api/health",//��������ַ
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = Guid.NewGuid().ToString(),
                Name = consulConfig.ServiceName,
                Address = consulConfig.IP,
                Port = consulConfig.Port,
                Tags = new[] { $"urlprefix-/{consulConfig.ServiceName}" }//��� urlprefix-/servicename ��ʽ�� tag ��ǩ���Ա� Fabio ʶ��
            };

            consulClient.Agent.ServiceRegister(registration).Wait();//��������ʱע�ᣬ�ڲ�ʵ����ʵ����ʹ�� Consul API ����ע�ᣨHttpClient����
            lifetime.ApplicationStopping.Register(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();//����ֹͣʱȡ��ע��
                });
        }
    }
}
