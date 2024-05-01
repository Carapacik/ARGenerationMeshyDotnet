var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var fileStorageEnv = Environment.GetEnvironmentVariable("FILE_STORAGE")?.Trim('"');
var staticStorageDirectoryPath =
    fileStorageEnv ?? builder.Configuration.GetSection("FileStorageSettings:BasePath").Value!;
builder.Services.AddSingleton(new FileStorageSettings(staticStorageDirectoryPath));
builder.Services.AddControllers();
builder.Services.AddScoped<FileStorageService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
