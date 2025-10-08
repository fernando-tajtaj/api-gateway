using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAngular");

var logosPath = Environment.GetEnvironmentVariable("LOGOS_PATH");
if (string.IsNullOrEmpty(logosPath))
{
    // En Docker será /app/logos
    // En local será ./logos (relativa al ejecutable)
    logosPath = Path.Combine(Directory.GetCurrentDirectory(), "logos");
}

// Crear directorio si no existe
try
{
    if (!Directory.Exists(logosPath))
    {
        Directory.CreateDirectory(logosPath);
        Console.WriteLine($"Carpeta de logos creada en: {logosPath}");
    }
    else
    {
        Console.WriteLine($"Carpeta de logos: {logosPath}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error creando carpeta de logos: {ex.Message}");
}

// Servir archivos estáticos de logos
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(logosPath),
    RequestPath = "/logos",
    OnPrepareResponse = ctx =>
    {
        // Cache por 1 día
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=86400");
    }
});

// Middleware para logs de archivos estáticos (útil para debug)
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/logos"))
    {
        Console.WriteLine($"Solicitando archivo: {context.Request.Path}");
    }
    await next();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();
Console.WriteLine($"Ruta de logos: {logosPath}");
app.Run();
