using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Drawing;

namespace Online_Food_Portal.Controllers
{
    [Route("Administrative")]
    [Authorize(Roles = "Administrator")]
    [ApiController]
    public class AdministrativeController(IItemService itemService, IStoreSettingsService storeSettingsService, IModificationService modificationService) : Controller
    {
        private readonly IItemService itemService = itemService;
        private readonly IModificationService modificationService = modificationService;
        private readonly IStoreSettingsService storeSettingsService = storeSettingsService;

        public class ItemCreationModel
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

            public string image { get { return Path.Exists(imageLocation) ? imageName : "/images/itemplaceholder.jpg"; } }

            public string imageLocation { get { return Path.Join(ItemModel.webRootPath, imageName); } }

            public string imageName { get { return $"/images/{id}{name}.jpg"; } }

            public List<ModificationCreationModel> modifications { get; set; }

            public ItemCreationModel(int id, string name, string description, decimal price, int stock, bool hidden, List<ModificationCreationModel> modifications)
            {
                this.id = id;
                this.name = name;
                this.description = description;
                this.price = price;
                this.stock = stock;
                this.hidden = hidden;
                this.modifications = modifications;
            }

            public ItemCreationModel()
            {
                id = -1;
                name = description = "";
                price = 1;
                stock = 0;
                hidden = false;
                modifications = new List<ModificationCreationModel>();
            }
        }

        public class ModificationCreationModel
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

            public ModificationCreationModel(int id, string name, string description, decimal price_offset, int stock, bool defaultModification, bool hidden)
            {
                this.id = id;
                this.name = name;
                this.description = description;
                this.price_offset = price_offset;
                this.stock = stock;
                this.defaultModification = defaultModification;
                this.hidden = hidden;
            }

            public ModificationCreationModel()
            {
                id = -1;
                name = description = "";
                price_offset = 1;
                stock = 0;
                defaultModification = true;
                hidden = false;
            }
        }

        /// <summary>
        /// Menu Page for Administrators
        /// </summary>
        /// <returns>The page showing the complete menu for administrative actions</returns>
        [Route("Home")]
        public IActionResult Home()
        {
            return View(itemService.GetItems(false));
        }

        /// <summary>
        /// Item Creation/Modification Page
        /// </summary>
        /// <param name="itemId">The ID of the item to modify, or 0 for item creation</param>
        /// <returns></returns>
        [Route("ItemCreation/{itemId}")]
        [Route("ItemCreation")]
        [HttpGet]
        public IActionResult ItemCreation(int itemId)
        {
            if (itemId == 0)
                return View(new ItemCreationModel());

            ItemModel? itemModel = itemService.GetItem(itemId);
            if (itemModel == null)
                return View(new ItemCreationModel());

            List<ModificationModel> modifications = modificationService.GetModificationsByItemId(itemId);

            ItemDTO itemDTO = new ItemDTO(itemModel, modifications);

            ItemCreationModel model = new ItemCreationModel(
                    itemDTO.ItemModel.id, itemDTO.ItemModel.name, itemDTO.ItemModel.description, itemDTO.ItemModel.price,
                    itemDTO.ItemModel.stock, itemDTO.ItemModel.hidden, [.. modifications.Select(x => new ModificationCreationModel(x.id, x.name, x.description, x.price_offset, x.stock, x.defaultModification, x.hidden))]);

            return View(model);
        }

        /// <summary>
        /// Item Creation/Modification Post route
        /// </summary>
        /// <param name="itemModel">The posted item model to create or update</param>
        /// <returns>An HttpResponseMessage with the HttpStatusCode and message depending on the results of the item update/creation request</returns>
        [Route("ItemCreation")]
        [HttpPost]
        public HttpResponseMessage ItemCreation(ItemCreationModel itemCreationModel)
        {
            if (itemCreationModel.name.Length == 0)
                return GenerateResponse(HttpStatusCode.BadRequest, "Item name is required");

            if (itemCreationModel.id <= 0)
            {
                itemCreationModel.id = itemService.CreateItem(itemCreationModel.name, itemCreationModel.description, itemCreationModel.price, itemCreationModel.stock, itemCreationModel.hidden);

                if (itemCreationModel.id <= 0)
                    return GenerateResponse(HttpStatusCode.FailedDependency, "Item failed to create");
            }
            else
            {
                ItemModel? item = itemService.GetItem(itemCreationModel.id);

                if (item != null)
                {
                    item.name = itemCreationModel.name;
                    item.description = itemCreationModel.description;
                    item.price = itemCreationModel.price;
                    item.stock = itemCreationModel.stock;
                    item.hidden = itemCreationModel.hidden;

                    itemService.UpdateItem(item);
                }
                else
                    return GenerateResponse(HttpStatusCode.NotFound, "Item not found for update");
            }

            foreach (ModificationCreationModel modificationCreationModel in itemCreationModel.modifications)
            {
                if (modificationCreationModel.name.Length == 0)
                    continue;

                if (modificationCreationModel.id <= 0)
                {
                    modificationService.CreateModification(modificationCreationModel.name, modificationCreationModel.description, modificationCreationModel.price_offset, modificationCreationModel.stock, modificationCreationModel.defaultModification, modificationCreationModel.hidden, itemCreationModel.id);
                }
                else
                {
                    ModificationModel? modification = modificationService.GetModification(modificationCreationModel.id);

                    if (modification != null)
                    {
                        modification.name = modificationCreationModel.name;
                        modification.description = modificationCreationModel.description;
                        modification.price_offset = modificationCreationModel.price_offset;
                        modification.stock = modificationCreationModel.stock;
                        modification.defaultModification = modificationCreationModel.defaultModification;
                        modification.hidden = modificationCreationModel.hidden;

                        modificationService.UpdateModification(modification);
                    }
                }
            }

            return GenerateResponse(HttpStatusCode.OK, "Item and modifications created or updated successfully");
        }

        /// <summary>
        /// Page for updating the image of a specific item
        /// </summary>
        /// <param name="itemId">The ID of the item to update the image of</param>
        /// <returns>The page containing the form allowing the upload of an image and a picture showing the current image</returns>
        [Route("ItemImage/{itemId}")]
        public IActionResult ItemImage(int itemId)
        {
            ItemModel? item = itemService.GetItem(itemId);

            if (item == null)
                return NotFound();

            return View(item);
        }

        /// <summary>
        /// Route for posting an image for a specific item
        /// </summary>
        /// <param name="image">The image included in the form for the item</param>
        /// <param name="itemId">The ID of the item to update the image of</param>
        /// <returns>An HttpResponseMessage with the HttpStatusCode and message depending on the results of the image update request</returns>
        [Route("UploadImage/{itemId}")]
        [HttpPost]
        public IActionResult UploadImage([FromForm] IFormFile image, int itemId)
        {
            System.Diagnostics.Debug.WriteLine($"Image uploaded with ItemID: {itemId}");

            if (image == null || image.Length == 0)
                return BadRequest();

            ItemModel? item = itemService.GetItem(itemId);

            if (item == null)
                return NotFound();

            using (MemoryStream stream = new MemoryStream())
            {
                image.CopyTo(stream);
                using (Image bitmap = Image.FromStream(stream))
                {
                    using (Image resizedImage = new Bitmap(bitmap, new Size(256, 256)))
                    {
                        
                        resizedImage.Save(item.imageLocation, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }

            return RedirectToAction("Home", "Administrative");
        }

        /// <summary>
        /// Store Settings Page
        /// </summary>
        /// <returns>The page showing store settings</returns>
        [Route("StoreSettings")]
        [HttpGet]
        public IActionResult StoreSettings()
        {
            return View(storeSettingsService.GetStoreSettings());
        }

        /// <summary>
        /// Store Settings Update Post Page
        /// </summary>
        /// <param name="model">The Store Settings Model posted to the page</param>
        /// <returns>An HttpResponseMessage with the HttpStatusCode and message depending on the results of the settings update</returns>
        [Route("StoreSettings")]
        [HttpPost]
        public HttpResponseMessage StoreSettings(StoreSettingsModel model)
        {
            if (storeSettingsService.SetStoreSettings(model) == 1)
                return GenerateResponse(HttpStatusCode.OK, "Store settings successfully updated");

            return GenerateResponse(HttpStatusCode.FailedDependency, "Store settings failed to update");
        }

        private HttpResponseMessage GenerateResponse(HttpStatusCode code, string content)
        {
            HttpResponseMessage message = new HttpResponseMessage(code);

            message.ReasonPhrase = content;

            return message;
        }
    }
}
