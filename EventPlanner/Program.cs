using EventPlanner.Data;
namespace EventPlanner;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<EventDb>();
        
        // Add session services
        builder.Services.AddDistributedMemoryCache(); // Required for session state
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
            options.Cookie.HttpOnly = true; // Ensure the session cookie is HTTP-only
            options.Cookie.IsEssential = true; // Mark session cookie as essential
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        
        app.UseSession();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Events}/{action=Index}/{id?}");

        app.Run();
    }
}