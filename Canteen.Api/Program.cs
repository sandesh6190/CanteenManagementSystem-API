using Canteen.Api.Extensions;
using Canteen.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(x => x.Filters.Add<GlobalExceptionFilterAttribute>());
builder.Services.JsonConfig(builder.Configuration);
builder.Services.CoreConfigure(builder.Configuration);
builder.Services.ConfigureDi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.Run();