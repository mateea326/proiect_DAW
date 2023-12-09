using Microsoft.AspNetCore.Identity;

namespace DevCollab.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Answer> Answers { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}
