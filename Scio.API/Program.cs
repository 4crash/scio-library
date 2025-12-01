using Scio.API.Services;
using Scio.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7186")  // Only HTTPS Blazor client
              .WithMethods("GET", "POST")  // Only allow specific methods
              .WithHeaders("Content-Type")
              .AllowCredentials();
    });
});

var app = builder.Build();

// Add security headers middleware
app.Use(async (context, next) =>
{
    // Enforce HTTPS
    if (!context.Request.IsHttps)
    {
        var httpsUrl = $"https://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
        context.Response.Redirect(httpsUrl, permanent: true);
        return;
    }

    // Security headers - use Append instead of Add for IHeaderDictionary
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

    await next();
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors("AllowBlazor");
app.MapControllers();

app.Run();
