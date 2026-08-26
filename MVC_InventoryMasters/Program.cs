using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.Cookies;
using MVC_InventoryMasters.Hubs;
using MVC_InventoryMasters.Repositories;
using MVC_InventoryMasters.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configurações de Serviços ---
string jsonPath = Path.Combine(AppContext.BaseDirectory, "firebase-service-account.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<FirebaseService>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acesso/Login";
        options.AccessDeniedPath = "/Acesso/Negado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddScoped<ContextoUsuarioService>();
builder.Services.AddScoped<PermissaoService>();
builder.Services.AddScoped<TokenAcessoKinectService>();
builder.Services.AddScoped<EmailTokenService>();
builder.Services.AddScoped<EmpresasRepository>();
builder.Services.AddScoped<LogsSistemaRepository>();
builder.Services.AddScoped<TokensAcessoKinectRepository>();
builder.Services.AddScoped<ParceirosRepository>();
builder.Services.AddScoped<MedicaoVolumeRepository>();
builder.Services.AddScoped<NotificacaoRepository>();
builder.Services.AddScoped<ParametrosSistemaRepository>();
builder.Services.AddScoped<ITokenAcessoKinectService>(sp => sp.GetRequiredService<TokenAcessoKinectService>());
builder.Services.AddScoped<IEmailTokenService>(sp => sp.GetRequiredService<EmailTokenService>());
builder.Services.AddScoped<ILogsSistemaRepository>(sp => sp.GetRequiredService<LogsSistemaRepository>());
builder.Services.AddScoped<IParceirosRepository>(sp => sp.GetRequiredService<ParceirosRepository>());
builder.Services.AddScoped<IPerfisRepository>(sp => sp.GetRequiredService<PerfisRepository>());
builder.Services.AddScoped<IUsuariosRepository>(sp => sp.GetRequiredService<UsuariosRepository>());
builder.Services.AddScoped<IMedicaoVolumeRepository>(sp =>
    sp.GetRequiredService<MedicaoVolumeRepository>());
builder.Services.AddScoped<INotificacaoRepository>(sp =>
    sp.GetRequiredService<NotificacaoRepository>());
builder.Services.AddScoped<IParametrosSistemaRepository>(sp =>
    sp.GetRequiredService<ParametrosSistemaRepository>());
builder.Services.AddScoped<UsuariosRepository>();
builder.Services.AddScoped<PerfisRepository>();

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

app.UseAuthentication();
app.UseAuthorization();

// 3. Mapeamento de Hubs e Controllers
app.MapHub<MedicaoHub>("/medicaoHub");
app.MapHub<NotificacaoHub>("/notificacaoHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
