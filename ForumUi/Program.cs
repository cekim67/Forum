var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpClient servisini ekle
builder.Services.AddHttpClient();

// Session servislerini ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session middleware'i ekle (UseRouting'den sonra, UseAuthorization'dan önce)
app.UseSession();

app.UseAuthorization();

// Route konfigürasyonu
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Topics}/{action=Index}/{id?}");

app.Run();