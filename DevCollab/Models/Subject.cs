﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevCollab.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
        public string Title { get; set; }


        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Content { get; set; }


        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }


        public virtual Category? Category { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }


        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }
    }
}
