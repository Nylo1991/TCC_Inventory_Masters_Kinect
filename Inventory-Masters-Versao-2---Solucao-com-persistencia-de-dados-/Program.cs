using InventoryMaster.Data;
using InventoryMaster.Hubs;
using Google.Cloud.Firestore;
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


using InventoryMasters.Services;
using InventoryMasters.Repositories;

var builder = WebApplication.CreateBuilder(args);



string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "inventorymasters_firebase.json");
if (File.Exists(path))
{
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
}

builder.Services.AddSingleton<InventoryMasters.Services.FirebaseService>();


string projectId = "inventorymasters";
builder.Services.AddSingleton(sp =>
{
    return FirestoreDb.Create(projectId);
});


builder.Services.AddScoped<ParceiroRepository>();
builder.Services.AddScoped<InventoryMasters.Repositories.UsuarioRepository>();

// Serviços nativos do ASP.NET Core
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Comunicação em tempo real com SignalR
builder.Services.AddSignalR();

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});


var app = builder.Build();



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

app.MapRazorPages();

// Rotas do SignalR
app.MapHub<ResiduosHub>("/residuosHub");

// Inicia o servidor
app.Run();