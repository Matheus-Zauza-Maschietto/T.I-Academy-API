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


