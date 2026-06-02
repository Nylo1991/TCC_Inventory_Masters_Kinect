using Google.Cloud.Firestore;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configurações de Serviços ---
string jsonPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<FirebaseService>();

builder.Services.AddScoped<ParceirosRepository>();
builder.Services.AddScoped<MedicaoVolumeRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<ParametrosSistemaRepository>();
builder.Services.AddScoped<UsuariosRepository>();

// Configuração do CORS (deve ser a primeira coisa a ser definida antes do build)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// --- Pipeline de Middleware (A ORDEM É O PONTO CRÍTICO) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 1. Roteamento deve vir primeiro
app.UseRouting();

// 2. CORS deve vir DEPOIS do Routing e ANTES de Autorização e Endpoints
app.UseCors("AllowAll");

app.UseAuthorization();

// 3. Mapeamento de Hubs e Controllers
app.MapHub<MedicaoHub>("/medicaoHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();