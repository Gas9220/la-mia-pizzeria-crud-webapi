using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using la_mia_pizzeria_crud_mvc.ValidationAttributes;

namespace la_mia_pizzeria_crud_mvc.Models
{
    public class Pizza
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name can't be empty")]
        [StringLength(20, ErrorMessage = "Max 20 characters")]
        public string Name { get; set; }

        [Column("photo_url")]
        [Url(ErrorMessage = "Invalid url format")]
        [Required(ErrorMessage = "Url can't be empty")]
        public string PhotoUrl { get; set; }

        [Required(ErrorMessage = "Description can't be empty")]
        [StringLength(500, ErrorMessage = "Max 500 characters")]
        [FiveWordsDescription]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price can't be empty")]
        [Range(1, 150, ErrorMessage = "Price should be between 1€ and 150€")]
        public float Price { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public List<Ingredient>? Ingredients { get; set; }

        public Pizza() { }

        public Pizza(int id, string name, string photoUrl, string description, float price) { 
            Id = id;
            Name = name;
            PhotoUrl = photoUrl;
            Description = description;
            Price = price;
        }
    }
}
