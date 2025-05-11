using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Services;

public interface IReviewService
{
    Task<ReviewStatisticsViewModel> GetProductReviewsAsync(string productId);
    Task<bool> AddReviewAsync(Review review);
    Task<bool> UpdateReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(int reviewId);
    Task<bool> ApproveReviewAsync(int reviewId);
}

public class ReviewService(EfContext context) : IReviewService
{
    public async Task<ReviewStatisticsViewModel> GetProductReviewsAsync(string productId)
    {
        // Get all approved reviews for this product
        var reviews = await context.Reviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.ReviewDate)
            .ToListAsync();

        // Calculate statistics
        var statistics = new ReviewStatisticsViewModel
        {
            TotalReviews = reviews.Count
        };

        if (reviews.Any())
        {
            statistics.AverageRating = Math.Round(reviews.Average(r => r.Rating), 1);

            // Calculate rating counts and percentages
            for (int i = 1; i <= 5; i++)
            {
                int count = reviews.Count(r => r.Rating == i);
                double percentage = reviews.Any() ? Math.Round((double)count / reviews.Count * 100) : 0;

                statistics.RatingCounts[i] = count;
                statistics.RatingPercentages[i] = percentage;
            }

            // Map to view models
            statistics.Reviews = reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                UserName = r.User.Username,
                UserProfilePicture = "/images/users/default.webp",
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate
            }).ToList();
        }

        return statistics;
    }

    public async Task<bool> AddReviewAsync(Review review)
    {
        try
        {
            context.Reviews.Add(review);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateReviewAsync(Review review)
    {
        try
        {
            context.Reviews.Update(review);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        try
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ApproveReviewAsync(int reviewId)
    {
        try
        {
            var review = await context.Reviews.FindAsync(reviewId);
            if (review == null)
                return false;

            review.IsApproved = true;
            context.Reviews.Update(review);
            await context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}