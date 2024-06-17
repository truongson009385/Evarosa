using Imageflow.Server;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Evarosa.Data;
using Evarosa.Services.Impl;
using Evarosa.Services;
using Microsoft.AspNetCore.Rewrite;
using Evarosa.Services.Impl;
using Evarosa.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));

// Add services to the container.
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = "member";
    o.DefaultChallengeScheme = "member";
})
    .AddCookie("member", o =>
    {
        o.LoginPath = "/dang-nhap";
        o.LogoutPath = "/dang-xuat";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        o.AccessDeniedPath = "/access-denied";
    })
    .AddCookie("vcms", o =>
    {
        o.LoginPath = "/Vcms/Login";
        o.LogoutPath = "/Vcms/Logout";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        o.AccessDeniedPath = "/Vcms/AccessDenied";
    });



builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddSingleton<IAppService, AppService>();
builder.Services.AddScoped<UnitOfWork, UnitOfWork>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

var app = builder.Build();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseRewriter();
    app.UseRewriter(new RewriteOptions().AddRedirectToNonWwwPermanent().AddRedirectToHttpsPermanent());
}

app.UseImageflow(new ImageflowMiddlewareOptions()
    .SetMapWebRoot(true)
    .MapPath("/contents", Path.Combine(builder.Environment.ContentRootPath, "Uploads"))
    .SetAllowCaching(true)
    .SetDefaultCacheControlString("public, max-age=2592000")
    .AddCommandDefault("webp.quality", "90")
    );

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/contents"
});

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
