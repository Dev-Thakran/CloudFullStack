using Microsoft.AspNetCore.Identity;
namespace CloudFullStackMVC.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? AccessibilityNeeds { get; set; }
    public bool DrivesElectricVehicle { get; set; }
}
