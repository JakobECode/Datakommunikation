var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
	options.AddPolicy("MyAllowSpecificOrigins", p =>
	{
		p.WithOrigins("https://localhost:7045/TemperatureHub")
					  .AllowAnyMethod()
					  .AllowAnyHeader();
	});
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("MyAllowSpecificOrigins");
app.UseAuthorization();

app.MapRazorPages();
app.Use(async (context, next) =>
{

	context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src-elem https://cdnjs.cloudflare.com 'self' 'unsafe-inline'; ");
	await next();
});


app.Run();
