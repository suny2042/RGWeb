using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<RGWeb.ViewModels.IContentViewModel, RGWeb.ViewModels.ContentViewModel>();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTU5MTcwQDMxMzkyZTM0MmUzMGRnSUwwelF2Y0YvOTdjOU1nQ3BFQVlMdVNacXViYkFrR1dyNTZHbmFrWTg9");
builder.Services.AddSyncfusionBlazor();

builder.Configuration                       // 읽을 json 파일은 아래에 추가
    .AddJsonFile("serversettings.json", optional: true, reloadOnChange: true)
    ;

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
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
