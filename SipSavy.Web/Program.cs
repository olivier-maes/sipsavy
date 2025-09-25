using SipSavy.Data;
using SipSavy.Web.Views;
using SipSavy.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Blazor
builder.Services.AddRazorComponents();

// Aspire service defaults
builder.AddServiceDefaults();

// Data
builder.AddNpgsqlDbContext<AppDbContext>("sipsavy");

builder.Services.AddScoped<IQueryFacade, QueryFacade>();

// Mediator
builder.Services.AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; });

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
app.MapRazorComponents<App>();

// Aspire default endpoints for health checks, etc. (only in development)
app.MapDefaultEndpoints();

app.Run();