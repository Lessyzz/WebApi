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
            var entities = await context.Products.Where(Product => Product.ProductSellerId == sellerId).Include(p => p.Category).ToListAsync();
            return entities;
        }

        public async Task<Product> GetProductById(string productId)
        {
            var entity = await context.Products.Include(p => p.ProductSeller).FirstOrDefaultAsync(Product => Product.Id == productId);
            return entity!;
        }
        
        public async Task<List<BasketProduct>> GetBasketProducts(string userId)
        {
            var entities = await context.BasketProducts
                .Where(b => b.UserId == userId)
                .Include(p => p.Product)
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

            var features = JsonConvert.SerializeObject(addProductDto.FeatureValues);
            
            var product = new Product
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                Price = addProductDto.Price,
                Images = addProductDto.Images,
                CategoryId = addProductDto.CategoryId,
                Discount = addProductDto.Discount,
                ProductSellerId = addProductDto.ProductSellerId,
                Quantity = addProductDto.Quantity,
                Features = features
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
        }        

        // Tek seferde istediğin kadar sepete eklemeye yarıyor
        public async Task AddProductToBasketProductWithQuantity(string productId, string userId, int quantity)
        {
            var basketProductAlready = await context.BasketProducts.FirstOrDefaultAsync(Product => Product.ProductId == productId && Product.UserId == userId);
            if (basketProductAlready != null) // Eğer ürün zaten sepette var ise miktar kadar arttır.
            {
                basketProductAlready.Quantity += quantity;
                await context.SaveChangesAsync();
            }
            else // Eğer ürün sepette yok ise sepete miktar kadar ekle.
            {
                var product = await context.Products.FirstOrDefaultAsync(Product => Product.Id == productId);
                var basketProduct = new BasketProduct
                {
                    ProductId = productId,
                    Product = product,
                    UserId = userId,
                    Quantity = quantity,
                };

                await context.BasketProducts.AddAsync(basketProduct);
                await context.SaveChangesAsync();
            }
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

        public async Task BuyAllBasketProducts(string userId)
        {
            var basketProducts = await context.BasketProducts.Where(Product => Product.UserId == userId).ToListAsync();
            
            double totalProductPrice = 0;
            var paymentId = Guid.NewGuid().ToString();
            foreach (var basketProduct in basketProducts)
            {
                var product = await context.Products.FirstOrDefaultAsync(Product => Product.Id == basketProduct.ProductId);
                if (product != null && product.Quantity >= basketProduct.Quantity)
                {
                    // Ürün fiyatını hesapla
                    double productTotalPrice = product.GetDiscountedPrice() * basketProduct.Quantity;
                    totalProductPrice += productTotalPrice;

                    // double shippingCost = totalProductPrice >= 500 ? 0 : 29.90;

                    // double taxAmount = totalProductPrice * 0.18; // 18% KDV

                    // double finalPrice = totalProductPrice + taxAmount + shippingCost;

                    context.PaidProducts.Add(new PaidProduct
                    {
                        ProductId = basketProduct.ProductId,
                        PaymentId = paymentId,
                        Quantity = basketProduct.Quantity,
                        UserId = userId,
                        TotalPrice = productTotalPrice,
                        SellerId = product.ProductSellerId,
                    });

                    // Stok miktarını azalt
                    product.Quantity -= basketProduct.Quantity;
                }
            }

            context.BasketProducts.RemoveRange(basketProducts);

            await context.SaveChangesAsync();
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
    
        public async Task UpdateProduct(Product updateProductDto)
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
                if (updateProductDto.Features != null) product.Features = updateProductDto.Features;
                
                await context.SaveChangesAsync();
            }
        }
        

        public async Task AddPromotionCode(string code, double discount)
        {
            var promotionCode = new PromotionCode
            {
                Name = code,
                Discount = discount
            };
            await context.PromotionCodes.AddAsync(promotionCode);
            await context.SaveChangesAsync();
        }

        public async Task RemovePromotionCode(string code)
        {
            var promotionCode = await context.PromotionCodes.FirstOrDefaultAsync(p => p.Name == code);
            if (promotionCode != null)
            {
                context.PromotionCodes.Remove(promotionCode);
                await context.SaveChangesAsync();
            }
        }

        public async Task<double> CheckPromotionCode(string code)
        {
            var promotionCode = await context.PromotionCodes.FirstOrDefaultAsync(p => p.Name == code);
            if (promotionCode != null)
            {
                return promotionCode.Discount;
            }
            return 0;
        }

        public async Task<List<Product>> GetProductsByCategoryId(int categoryId)
        {
            // First, find all child category IDs recursively
            var allCategoryIds = new HashSet<int> { categoryId };
            var categoriesToProcess = new Queue<int>();
            categoriesToProcess.Enqueue(categoryId);
    
            while (categoriesToProcess.Count > 0)
            {
                var currentCategoryId = categoriesToProcess.Dequeue();
                var childCategories = await context.Categories
                    .Where(c => c.ParentCategoryId == currentCategoryId)
                    .Select(c => c.Id)
                    .ToListAsync();
        
                foreach (var childId in childCategories)
                {
                    if (allCategoryIds.Add(childId))
                    {
                        categoriesToProcess.Enqueue(childId);
                    }
                }
            }
    
            var products = await context.Products
                .Where(product => allCategoryIds.Contains(product.CategoryId))
                .ToListAsync();
    
            return products;
        }

        // Get orders
        public async Task<List<PaidProduct>> GetOrders(string userId)
        {
            var orders = await context.PaidProducts.Where(Product => Product.UserId == userId).ToListAsync();
            return orders;
        }
    }
}