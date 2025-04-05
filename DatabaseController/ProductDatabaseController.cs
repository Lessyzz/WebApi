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

        public async Task<List<Product>> GetProductsAsSeller(string sellerId)
        {
            var entities = await context.Products.Where(Product => Product.ProductSellerId == sellerId).ToListAsync();
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
        
        public async Task<List<PaidProduct>> GetPaidProducts(string userId)
        {
            var entities = await context.PaidProducts
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return entities;
        }

        public async Task<List<PaidProduct>> GetPaidProductsAsSeller(string sellerId)
        {
            var entities = await context.PaidProducts
                .Where(b => b.SellerId == sellerId)
                .ToListAsync();
            return entities;
        }

        public async Task AddProduct(AddProductDto addProductDto)
        {
            var product = new Product
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Price = addProductDto.Price,
                Images = addProductDto.Images,
                CategoryId = addProductDto.CategoryId,
                Discount = addProductDto.Discount,
                ProductSellerId = addProductDto.ProductSellerId,
                Quantity = addProductDto.Quantity
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
        
        public async Task RemoveProductFromBasketProduct(string productId, string userId)
        {
            var basketProduct = await context.BasketProducts.FirstOrDefaultAsync(Product => Product.ProductId == productId && Product.UserId == userId);
            if (basketProduct != null)
            {
                context.BasketProducts.Remove(basketProduct);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveOneProductFromBasketProduct(string productId, string userId)
        {
            var basketProduct = await context.BasketProducts.FirstOrDefaultAsync(Product => Product.ProductId == productId && Product.UserId == userId);
            if (basketProduct != null)
            {
                if (basketProduct.Quantity > 1) basketProduct.Quantity--;
                else context.BasketProducts.Remove(basketProduct);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllProductsFromBasketProduct(string userId)
        {
            var basketProducts = await context.BasketProducts.Where(Product => Product.UserId == userId).ToListAsync();
            if (basketProducts != null)
            {
                context.BasketProducts.RemoveRange(basketProducts);
                await context.SaveChangesAsync();
            }
        }

        public async Task BuyProduct(BuyProductDto buyProductDto)
        {
            var product = await context.Products.FirstOrDefaultAsync(Product => Product.Id == buyProductDto.ProductId);
            if (product != null && product.Quantity >= buyProductDto.Quantity)
            {
                // Satın alım tamamlandı. Önceki alışverişler kısmına eklendi.
                context.PaidProducts.Add(new PaidProduct
                {
                    ProductId = buyProductDto.ProductId,
                    Quantity = buyProductDto.Quantity,
                    UserId = buyProductDto.UserId,
                    TotalPrice = product.Price * buyProductDto.Quantity,
                    SellerId = product.ProductSellerId,
                });

                // Ürünün stok sayısı düşürüldü.
                product.Quantity -= buyProductDto.Quantity;
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveProduct(string productId)
        {
            var product = await context.Products.FirstOrDefaultAsync(Product => Product.Id == productId);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
        }
    
        public async Task UpdateProduct(UpdateProductDto updateProductDto)
        {
            var product = await context.Products.FirstOrDefaultAsync(Product => Product.Id == updateProductDto.Id);
            if (product != null)
            {
                if (updateProductDto.Name != null) product.Name = updateProductDto.Name;
                if (updateProductDto.Description != null) product.Description = updateProductDto.Description;
                if (updateProductDto.Price != 0) product.Price = updateProductDto.Price;
                if (updateProductDto.Images != null) product.Images = updateProductDto.Images;
                if (updateProductDto.CategoryId != null) product.CategoryId = updateProductDto.CategoryId;
                if (updateProductDto.Discount != 0) product.Discount = updateProductDto.Discount;
                if (updateProductDto.Quantity != 0) product.Quantity = updateProductDto.Quantity;
                if (updateProductDto.ProductSellerId != null) product.ProductSellerId = updateProductDto.ProductSellerId;
                await context.SaveChangesAsync();
            }
        }
    }
}