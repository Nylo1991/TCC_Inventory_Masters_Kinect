using Google.Cloud.Firestore;
using InventoryMaster.Data;
using InventoryMaster.Hubs;
using InventoryMasters.Repositories;
using InventoryMasters.Services;

var builder = WebApplication.CreateBuilder(args);


// =====================================
// LOGS
// =====================================

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// =====================================
// FIREBASE
// =====================================

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


// =====================================
// REPOSITORIES
// =====================================

builder.Services.AddScoped<ParceiroRepository>();
builder.Services.AddScoped<UsuarioRepository>();


// =====================================
// ASP.NET
// =====================================

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// =====================================
// SIGNALR
// =====================================

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});


// =====================================
// CORS
// =====================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:5001",
                "https://localhost:7001",
                "https://SEU-DOMINIO.onrender.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// =====================================
// BUILD
// =====================================

var app = builder.Build();


// =====================================
// PIPELINE
// =====================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseSession();

app.UseAuthorization();

app.UseWebSockets();


// =====================================
// MAPS
// =====================================

app.MapRazorPages();

app.MapHub<ResiduosHub>("/residuosHub");


// =====================================
// RUN
// =====================================

app.Run();