using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Online_Food_Portal.Models
{
    /// <summary>
    /// Modification Model for handling data
    /// </summary>
    public class ModificationModel
    {
        [ScaffoldColumn(false)]
        public int id { get; set; }

        [DisplayName("Modification Name")]
        [Required(ErrorMessage = "Modification Name is a required field")]
        [StringLength(64, ErrorMessage = "Modification Name must be 2-64 characters", MinimumLength = 2)]
        public string name { get; set; }

        [DisplayName("Modification Description")]
        [StringLength(64, ErrorMessage = "Modification Description must be no greater than 512 characters")]
        public string description { get; set; }

        [DisplayName("Modification Price")]
        [Range(0.0, 999.99, ErrorMessage = "Modification Price must be between 0-999.99")]
        public decimal price_offset { get; set; }

        [DisplayName("Modification Quantity")]
        [Required(ErrorMessage = "Modification Quantity is a required field, put -1 for unlimited")]
        [Range(-1, 10000, ErrorMessage = "Item Quantity must be between -1-10000")]
        public int stock { get; set; }

        [DisplayName("Default Modification")]
        public bool defaultModification { get; set; }

        [DisplayName("Disabled Modification")]
        public bool hidden { get; set; }

        [ScaffoldColumn(false)]
        public int items_id { get; set; }

        public ModificationModel(int id, string name, string description, decimal price_offset, int stock, bool defaultModification, bool hidden, int items_id)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.price_offset = price_offset;
            this.stock = stock;
            this.defaultModification = defaultModification;
            this.hidden = hidden;
            this.items_id = items_id;
        }
    }
}
