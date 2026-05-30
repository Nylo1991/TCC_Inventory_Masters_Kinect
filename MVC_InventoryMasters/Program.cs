using Google.Cloud.Firestore;
using System.IO;
using MVC_InventoryMasters.Repositories; // Ajuste conforme o nome real do seu namespace

var builder = WebApplication.CreateBuilder(args);

// Define o caminho para o arquivo JSON de credenciais do Firebase
string jsonPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);

// Registra os repositórios injetando o ID do projeto que está no JSON
string projectId = "inventorymasters";

builder.Services.AddScoped<ParceirosRepository>(sp => new ParceirosRepository(projectId));
builder.Services.AddScoped<MedicaoVolumeRepository>(sp => new MedicaoVolumeRepository(projectId));
builder.Services.AddScoped<NotificacaoRepository>(sp => new NotificacaoRepository(projectId));
builder.Services.AddScoped<ParametrosSistemaRepository>(sp => new ParametrosSistemaRepository(projectId));

builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();