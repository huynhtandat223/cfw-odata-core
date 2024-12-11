using CFW.ODataCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.ConfigureWarnings(warnings =>
//        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

var sqliteConnection = new SqliteConnection("DataSource=:memory:");
sqliteConnection.Open();
builder.Services.AddDbContext<AppDbContext>((s, option) =>
{
    option.UseSqlite(sqliteConnection);
    option.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
});

builder.Services
    .AddGenericODataEndpoints([typeof(Program).Assembly]);

var app = builder.Build();

app.UseGenericODataEndpoints();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.EnsureCreated();

app.Run();
