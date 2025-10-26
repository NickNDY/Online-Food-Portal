using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Online_Food_Portal.Models
{
    /// <summary>
    /// ItemModel for handling data
    /// </summary>
    public class ItemModel
    {
        [ScaffoldColumn(false)]
        public int id { get; set; }

        [DisplayName("Item Name")]
        [Required(ErrorMessage = "Item Name is a required field")]
        [StringLength(64, ErrorMessage = "Name must be 2-64 characters", MinimumLength = 2)]
        public string name { get; set; }

        [DisplayName("Item Description")]
        [Required(ErrorMessage = "Item Description is a required field")]
        [StringLength(1024, ErrorMessage = "Item Description must be 3-1024 characters", MinimumLength = 3)]
        public string description { get; set; }

        [DisplayName("Item Price")]
        [Required(ErrorMessage = "Item Price is a required field")]
        [Range(0, 999.99, ErrorMessage = "Item Price must be between 0-999.99")]
        public decimal price { get; set; }

        [DisplayName("Item Quantity")]
        [Required(ErrorMessage = "Item Quantity is a required field, put -1 for unlimited")]
        [Range(-1, 10000, ErrorMessage = "Item Quantity must be between -1-10000")]
        public int stock { get; set; }

        [DisplayName("Disabled Item")]
        [Required(ErrorMessage = "Disabled Item is a required field")]
        public bool hidden { get; set; }

        public ItemModel(int id, string name, string description, decimal price, int stock, bool hidden)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.price = price;
            this.stock = stock;
            this.hidden = hidden;
        }
    }
}
