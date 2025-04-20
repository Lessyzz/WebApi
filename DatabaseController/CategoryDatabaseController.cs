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
    public class CategoryDatabaseController(EfContext context, JWTTokenSystem jWtTokenSystem, IHubContext<NotificationHub> hubContext)
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        
        
        public async Task<List<Category>> GetCategories()
        {
            var entities = await context.Categories.ToListAsync();
            return entities;
        }  
        
        public async Task AddCategory(AddCategoryDto addCategoryDto)
        {
            var category = new Category
            {
                Name = addCategoryDto.Name,
                Features = addCategoryDto.Features,
                ParentCategoryId = addCategoryDto.ParentCategoryId,
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }       

        public async Task UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var category = await context.Categories.FirstOrDefaultAsync(Category => Category.Id == updateCategoryDto.Id);
            
            if (category != null) {
                if (!string.IsNullOrEmpty(updateCategoryDto.Name)) category.Name = updateCategoryDto.Name;
                if (updateCategoryDto.ParentCategoryId != 0) category.ParentCategoryId = updateCategoryDto.ParentCategoryId;
                await context.SaveChangesAsync();
            }
            
        }
    }
}