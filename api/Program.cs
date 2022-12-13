using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/addHeader", (HttpResponse response)=>{
    response.Headers.Add("teste", "Sthepany Batista");
    return new {Name = "Sthepany Batista", Age = 35};
});

// Passing infos by BODY Json
app.MapPost("/newPost", (Product product) => {
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
}

public class Product{
    public string Code { get; set; }
    public string Name { get; set; }
}
