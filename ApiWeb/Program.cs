using System.Runtime.Intrinsics.X86;
using System;
using System.IO.Pipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var config = app.Configuration;



app.MapGet("/", () => "Hello World!");

app.MapGet("/addHeader", (HttpResponse response)=>{
    response.Headers.Add("teste", "Sthepany Batista");
    return new {Name = "Sthepany Batista", Age = 35};
});

// Passing infos by BODY Json
app.MapPost("/newPost", (Product product, HttpRequest request) => {
    return product.Code + " - " + product.Name;
});

//Passing infos by URL QUERY 
// Exp: api.app.com/users?datastart={date}&dataend={date}
app.MapGet("/getProduct", ([FromQuery] string dateStart,[FromQuery] string dateEnd) => {
    return dateStart + " - " + dateEnd;
});

//Passing infos by URL PATH
// Exp: api.app.com/users/{info}
app.MapGet("getProduct/{code}", ([FromRoute] string code) => {
    return code;
});

//Passing infos by HEADER
app.MapGet("/getProductByHeader", (HttpRequest request)=>{
    return request.Headers["product-code"].ToString();
});

app.MapGet("/product/{id}", ([FromRoute] int Id, ApplicationDbContext context) =>{
   var product = context.Products.Include(p => p.Category).Include(p => p.tags).Where(p => p.Id == Id).First();
   if(product != null)
   {
        return Results.Ok(product);
   }
   return Results.NotFound();
   
});

app.MapPost("/product", (ProductDto productRequest, ApplicationDbContext context)=>{
    var Category = context.Category.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product{
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = Category
    };
    if(productRequest.Tags != null)
    {
        product.tags = new List<Tag>();
        foreach(var item in productRequest.Tags){
            product.tags.Add(new Tag{Name = item});
        }
    }
    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created($"/products/{product.Id}", product.Code);
});

app.MapPut("/product/{id}", ([FromRoute] int Id, ProductDto productRequest, ApplicationDbContext context)=>{
        var product = context.Products.Include(p => p.Category).Include(p => p.tags).Where(p => p.Id == Id).First();
        var Category = context.Category.Where(c => c.Id == productRequest.CategoryId).First();

        product.Code =productRequest.Code;
        product.Name = productRequest.Name;
        product.Description = productRequest.Description;
        product.Category =  Category;
        product.tags = new List<Tag>();
        if(productRequest.Tags != null)
        {
            product.tags = new List<Tag>();
            foreach(var item in productRequest.Tags){
                product.tags.Add(new Tag{Name = item});
            }
        }

        context.SaveChanges();
        return Results.Ok();
});

app.MapDelete("/product/{id}", ([FromRoute] int Id, ApplicationDbContext context)=>{
    var product = context.Products.Where(p => p.Id == Id).First();
    context.Products.Remove(product);
    context.SaveChanges();
    return Results.Ok();
});

app.MapGet("/baseConfigs", (IConfiguration config)=>{
    return Results.Ok(config.GetSection("Logging"));
});

app.Run();
