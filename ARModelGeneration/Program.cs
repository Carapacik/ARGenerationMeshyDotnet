using ARModelGeneration;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var fileStorageEnv = Environment.GetEnvironmentVariable("FILE_STORAGE")?.Trim('"');
var staticStorageDirectoryPath =
    fileStorageEnv ?? builder.Configuration.GetSection("FileStorageBasePath").Value!;
builder.Services.AddSingleton(new FileStorageSettings(staticStorageDirectoryPath));
builder.Services.AddControllers();
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Meshy",
        Description = "Generate Models with Meshy",
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
