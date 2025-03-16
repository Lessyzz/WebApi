using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Hubs;
using WebApi.Models;
using WebApi.Dto;

namespace WebApi.Data
{
    public class ProductDatabaseController(EfContext context, JWTTokenSystem jWtTokenSystem, IHubContext<NotificationHub> hubContext)
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        
        public async Task<List<Product>> GetProducts()
        {
            var entities = await context.Products.Take(20).ToListAsync();
            return entities;
        }

        public async Task<Product> GetProductById(string productId)
        {
            var entity = await context.Products.FirstOrDefaultAsync(Product => Product.Id == productId);
            return entity!;
        }
        
        public async Task<List<BasketProduct>> GetBasketProducts(string userId)
        {
            var entities = await context.BasketProducts
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return entities;
        }
        
        public async Task AddProduct(AddProductDto addProductDto, string imageUrl)
        {
            List<string> images = new List<string>();
            var imagesStr = imageUrl.Split(',');
            foreach (var image in imagesStr)
            {
                images.Add(image);
            }
            
            var product = new Product
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Price = addProductDto.Price,
                Image = images,
                Categories = addProductDto.Categories,
                Discount = addProductDto.Discount,
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
        }        
        
        public async Task AddProductToBasketProduct(string productId, string userId)
        {
            var basketProductAlready = await context.BasketProducts.FirstOrDefaultAsync(Product => Product.ProductId == productId && Product.UserId == userId);
            if (basketProductAlready != null) // Eğer ürün zaten sepette var ise sayıyı 1 arttır.
            {
                basketProductAlready.Quantity++;
                await context.SaveChangesAsync();
            }
            else // Eğer ürün sepette yok ise sepete ekle.
            {
                var product = await context.Products.FirstOrDefaultAsync(Product => Product.Id == productId);
                var basketProduct = new BasketProduct
                {
                    ProductId = productId,
                    Product = product,
                    UserId = userId,
                    Quantity = 1,
                };

                await context.BasketProducts.AddAsync(basketProduct);
                await context.SaveChangesAsync();
            }
        }       
        
    }
}