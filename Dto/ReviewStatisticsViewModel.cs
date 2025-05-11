namespace WebApi.Dto;

public class ReviewStatisticsViewModel
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> RatingCounts { get; set; } = new Dictionary<int, int>();
    public Dictionary<int, double> RatingPercentages { get; set; } = new Dictionary<int, double>();
    public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
}