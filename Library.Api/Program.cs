using Library;
using Library.Core;
using Library.Core.Model.Front.Book;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, typeof(Program).Assembly.Location.Replace(".dll", ".xml")));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, typeof(BookDetails).Assembly.Location.Replace(".dll", ".xml")));
});

builder.Services.AddDbContext<LibraryDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryConnection")));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
