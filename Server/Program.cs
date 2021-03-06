using Server.Data;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;


var MyAllowSpeceficOrigins = "CorsPolicy";
var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Add permission for CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpeceficOrigins,
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// CONFIGURE FOR SQLITE
builder.Services.AddDbContext<AppDBContext>(options =>
    // Point to appsettings configuration
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // THIS IS THE WAY
// Other configs....
builder.Services.AddEndpointsApiExplorer();
// DTO
builder.Services.AddAutoMapper(typeof(DTOMappings));
//
builder.Services.AddSwaggerGen();

// Convert route endpoints to lowercase.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
// ----------- [BUILD CONFIGURATION MANTAIN IN APP] -----------


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Change
    app.UseDeveloperExceptionPage();
}

// Outside
    app.UseSwagger();
    app.UseSwaggerUI(swaggerUIOptions =>
    {
        swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Oriol Blog API");
        swaggerUIOptions.RoutePrefix = string.Empty;
    });

app.UseHttpsRedirection();

// Added static files
app.UseStaticFiles();



// Example returns all categories from database....
app.MapGet("/test", async (AppDBContext db) =>
    await db.Categories.ToListAsync());

// Activate CORS
app.UseCors(MyAllowSpeceficOrigins);

// Get & Register all endpoints of controllers
app.MapControllers();

// --- FINALLY RUN API ---
app.Run();

