using System;
using System.IO.Pipes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var config = app.Configuration;
Console.WriteLine(config.GetSection("Logging"));

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


public static class ProductRepository{
    public static List<Product> Products { get; set; }
    public static void Add(Product product)
    {
        if(Products == null)
            Products = new List<Product>();
        
        Products.Add(product);
    }   

    public static Product GetBy(string code){
        return Products.First(p => p.Code == code);
    }

    public static void UpdateBy(string code, string name){
        int queryResult = Products.FindIndex(x => x.Code==code);
        Products[queryResult].Name = name;
    }

    public static void DeleteBy(string code){
        if(Products.Find(pdct => pdct.Code == code) == null){
            throw new NullReferenceException();
        }
        else{
            Products.Remove(Products.Find(pdct => pdct.Code == code));
        }
    }
}
public class Product{
    public string Code { get; set; }
    public string Name { get; set; }
    }

public class ApplicationDbContext: DbContext{
    public DbSet<Product> Products {get; set;}
    

}

