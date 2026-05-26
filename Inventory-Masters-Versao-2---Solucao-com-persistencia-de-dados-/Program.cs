using Google.Cloud.Firestore;
using InventoryMaster.Data;
using InventoryMaster.Hubs;
using InventoryMasters.Repositories;
using InventoryMasters.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

string credentialPath = builder.Configuration["Firebase:CredentialsPath"];

if (!string.IsNullOrEmpty(credentialPath))
{
    Environment.SetEnvironmentVariable(
        "GOOGLE_APPLICATION_CREDENTIALS",
        credentialPath
    );
}

string projectId = builder.Configuration["Firebase:ProjectId"];

builder.Services.AddSingleton(sp =>
{
    return FirestoreDb.Create(projectId);
});

builder.Services.AddSingleton<FirebaseService>();

builder.Services.AddScoped<ParceiroRepository>();
builder.Services.AddScoped<UsuarioRepository>();


builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseSession();

app.UseAuthorization();


app.MapRazorPages();

app.MapHub<ResiduosHub>("/residuosHub");

app.Run();