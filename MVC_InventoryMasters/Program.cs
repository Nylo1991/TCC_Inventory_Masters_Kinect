using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Firebase
string jsonPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);
string projectId = "inventorymasters";

// 2. Adiciona Serviços ao Container de Dependência
builder.Services.AddControllersWithViews();

// Registra serviços e repositórios
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddScoped<ParceirosRepository>();
builder.Services.AddScoped<MedicaoVolumeRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<ParametrosSistemaRepository>();
builder.Services.AddScoped<UsuariosRepository>();

// Configuração CORS (Essencial para conectar o Kinect externamente)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configurações SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// 3. Pipeline de Middleware 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS deve vir após Routing e antes da Autenticação/Hubs
app.UseCors("AllowAll");

app.UseAuthorization();

// Mapeamento dos Hubs e Controllers
app.MapHub<MedicaoHub>("/medicaoHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();