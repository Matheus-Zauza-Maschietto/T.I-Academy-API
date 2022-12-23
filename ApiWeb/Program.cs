using System;
using System.IO.Pipes;
using Microsoft.AspNetCore.Mvc;

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

app.MapGet("/product/{code}", ([FromRoute] string code) =>{
    try{
        Product productName = ProductRepository.GetBy(code);
        return Results.Ok(productName);
    }
    catch{
        return Results.BadRequest();
    }   
    
});

app.MapPost("/product", (Product product)=>{
    try{
    ProductRepository.Add(
    new Product{
        Name = product.Name,
        Code = product.Code
    });
        return Results.Created($"/product/{product.Code}", product.Code);
    }
    catch{
        return Results.BadRequest();
    }
});

app.MapPut("/product", (Product product)=>{
    try{
        ProductRepository.UpdateBy(product.Code, product.Name);
        return Results.Ok();
    }
    catch{
        return Results.BadRequest();
    }
});

app.MapDelete("/product/{code}", ([FromRoute] string Code)=>{
    try{
        ProductRepository.DeleteBy(Code);
        return Results.Ok();
    }
    catch{
        return Results.BadRequest();
        
    }
});

app.MapGet("/baseConfigs", (IConfiguration config)=>{
    return Results.Ok(config.GetSection("Logging"));
});

app.Run();
