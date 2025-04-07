using MudBlazor.Services;
using VehicleInformationChecker.Components;
using VehicleInformationChecker.Components.Services.SearchRegistration;

var builder = WebApplication.CreateBuilder(args);

// Get APIs VES Key
var vesApiKey = builder.Configuration["APIs:VES:Key"];

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the SearchRegistrationEventService
builder.Services.AddSingleton<ISearchRegistrationEventService, SearchRegistrationEventService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
