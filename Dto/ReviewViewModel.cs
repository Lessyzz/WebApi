namespace WebApi.Dto;

public class ReviewViewModel
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string UserProfilePicture { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime ReviewDate { get; set; }
}