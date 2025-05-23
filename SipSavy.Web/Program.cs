using SipSavy.Web.Views;
using SipSavy.ServiceDefaults;
using SipSavy.Web.Infrastructure.Relational;

var builder = WebApplication.CreateBuilder(args);

// Blazor
builder.Services.AddRazorComponents();

// Aspire service defaults
builder.AddServiceDefaults();

builder.AddSqlServerDbContext<AppDbContext>("sipsavy");

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