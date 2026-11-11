namespace CloudFullStackMVC.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool DrivesElectricVehicle { get; set; }
        public string? AccessibilityNeeds { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsAdmin => Roles.Contains("Admin");
    }
}