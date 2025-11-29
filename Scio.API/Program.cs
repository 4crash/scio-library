using Scio.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5186",
            "http://localhost:5173",
            "http://localhost:3000",
            "https://localhost:7207",
            "https://localhost:7095"
        )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowBlazor");
app.MapControllers();

app.Run();
