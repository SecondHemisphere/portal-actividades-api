namespace ApiPortalActividades.DTOs.Dashboard
{
    public class DashboardTotalsDto
    {
        public int TotalActivities { get; set; }
        public int TotalOrganizers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalCategories { get; set; }
        public int TotalRatings { get; set; }
    }

    public class ActivitiesByCategoryDto
    {
        public string CategoryName { get; set; }
        public int TotalActivities { get; set; }
    }

    public class TopRatingsDto
    {
        public string ActivityTitle { get; set; }
        public double AvgRating { get; set; }
    }
}
