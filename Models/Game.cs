using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PAUP.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int Category { get; set; }
        public string Description { get; set; }
    }
}