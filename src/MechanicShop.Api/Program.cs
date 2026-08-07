using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);


QuestPDF.Settings.License = LicenseType.Community;




var app = builder.Build();

app.Run();
