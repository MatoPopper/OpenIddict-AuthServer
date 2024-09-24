using Microsoft.AspNetCore.Identity;

namespace IdentityUsers.Models.Results
{
    public class CreateUserResult
    {
        public AuthUser CreatedUser { get; set; } = null!;
        public List<IdentityError>? Errors { get; set; }
    }
}
