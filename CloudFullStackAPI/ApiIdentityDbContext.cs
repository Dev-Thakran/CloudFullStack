using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudFullStackAPI
{
    public class ApiIdentityDbContext(DbContextOptions<ApiIdentityDbContext> options) : IdentityDbContext<IdentityUser>(options) { }
}
