using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetiSusun.Core.Interfaces;
using RetiSusun.Core.Services;
using RetiSusun.Data;
using RetiSusun.Desktop.Forms;

namespace RetiSusun.Desktop;

static class Program
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Setup Dependency Injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        // Initialize database
        using (var scope = ServiceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<RetiSusunDbContext>();
            context.Database.EnsureCreated();
        }

        // Start with login form
        Application.Run(new LoginForm());
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        // Database
        services.AddDbContext<RetiSusunDbContext>(options =>
            options.UseSqlite("Data Source=retisusun.db"));

        // Services
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IRestockingService, RestockingService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
    }
}
