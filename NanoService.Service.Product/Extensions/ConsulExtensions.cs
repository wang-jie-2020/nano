using Consul;

namespace NanoService.Service.Product.Extensions
{
    public static class ConsulExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            ConsulClient client = new ConsulClient(consul =>
            {
                consul.Address = new Uri(configuration["consul:server:url"]);
                consul.Datacenter = "nano";
            });

            var ip = configuration["consul:service:ip"];
            var port = int.Parse(configuration["consul:service:port"]);
            var name = configuration["consul:service:name"];
            var health = configuration["consul:service:health"];

            var registration = new AgentServiceRegistration()
            {
                ID = name + "-" + Guid.NewGuid().ToString(),
                Name = name,
                Address = ip,
                Port = port,
                Tags = new[] { $"urlprefix-/{name}" },

                Checks = new[] {
                        new AgentServiceCheck()
                        {
                            Interval = TimeSpan.FromSeconds(10),
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                            HTTP = $"http://{ip}:{port}{health}",
                            Timeout = TimeSpan.FromSeconds(5)
                        }},
            };

            client.Agent.ServiceRegister(registration).Wait();

            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(() =>
            {
                client.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}






