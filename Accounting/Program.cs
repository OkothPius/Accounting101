using DAL_ACCOUNTING.Data;

var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("ConnectionStringName");

//builder.Services.AddSingleton<DatabaseContext>(provider => new DatabaseContext(connectionString));
builder.Services.AddScoped<DatabaseContext>(provider => new DatabaseContext(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Journals}/{action=Index}/{id?}");


app.Run();
