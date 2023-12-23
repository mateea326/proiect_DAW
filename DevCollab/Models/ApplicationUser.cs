using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevCollab.Models
{
    public class ApplicationUser : IdentityUser
    {
        // un user poate posta mai multe raspunsuri
        public virtual ICollection<Answer>? Answers { get; set; }

        // un user poate posta mai multe intrebari
        public virtual ICollection<Subject>? Subjects { get; set; }


        // atribute suplimentare adaugate pentru user
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        // variabila in care vom retine rolurile existente in baza de date
        // pentru popularea unui dropdown list
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}