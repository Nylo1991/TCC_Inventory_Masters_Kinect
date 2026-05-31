using Google.Cloud.Firestore;
using Microsoft.Extensions.DependencyInjection;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura as credenciais e o ID do projeto do Firebase
string jsonPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);

string projectId = "inventorymasters"; // Substitua pelo ID real do seu projeto se for diferente

// 2. Adiciona os Serviços ao Container de Dependência do .NET
builder.Services.AddControllersWithViews();

// Registra o FirebaseService passando o ID do projeto
builder.Services.AddSingleton<FirebaseService>();

// Registra os repositórios que vão usar o FirebaseService
builder.Services.AddScoped<ParceirosRepository>();
builder.Services.AddScoped<MedicaoVolumeRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<ParametrosSistemaRepository>();
builder.Services.AddScoped<UsuariosRepository>();

//Configurações SignalR
builder.Services.AddSignalR();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapHub<MedicaoHub>("/medicaoHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();