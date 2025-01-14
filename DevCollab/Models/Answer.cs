﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevCollab.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Conținutul răspunsului este obligatoriu")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        public int? SubjectId { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Subject? Subject { get; set; }
    }

}
