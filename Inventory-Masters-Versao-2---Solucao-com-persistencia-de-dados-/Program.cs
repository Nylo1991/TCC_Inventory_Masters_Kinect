using InventoryMaster.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Serviços
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// SignalR
builder.Services.AddSignalR();

// CORS (para WPF ou outros clientes)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin());
});

var app = builder.Build();

// Pipeline HTTP
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

// Hub SignalR
app.MapHub<ResiduosHub>("/residuosHub");

using (var conn = InventoryMaster.Data.DataBase.GetConnection())
{
    conn.Open();
}

app.Run();