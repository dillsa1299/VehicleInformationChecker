using MudBlazor.Services;
using VehicleInformationChecker.Components;
using VehicleInformationChecker.Components.Services.SearchRegistration;
using VehicleInformationChecker.Components.Services.SearchRegistrationService;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

// Register additional services
builder.Services.AddSingleton<ISearchRegistrationEventService, SearchRegistrationEventService>();
builder.Services.AddSingleton<ISearchRegistrationService>(provider =>
{
    var httpClient = provider.GetRequiredService<HttpClient>();
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new SearchRegistrationService(httpClient, configuration);
});

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
