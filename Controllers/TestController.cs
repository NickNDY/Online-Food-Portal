using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;

namespace Online_Food_Portal.Controllers
{
    [Route("Test")]
    public class TestController : Controller
    {
        private readonly IItemService itemService;
        private readonly IModificationService modificationService;
        private readonly IOrderService orderService;
        private readonly IPasswordService passwordService;
        private readonly IStoreSettingsService storeSettingsService;
        private readonly IAuthenticationService userService;

        public TestController(IItemService itemService, IModificationService modificationService, IOrderService orderService, IPasswordService passwordService, IStoreSettingsService storeSettingsService, IAuthenticationService userService)
        {
            this.itemService = itemService;
            this.modificationService = modificationService;
            this.orderService = orderService;
            this.passwordService = passwordService;
            this.storeSettingsService = storeSettingsService;
            this.userService = userService;
        }

        /// <summary>
        /// Testing home page showing the test cases and results when executed
        /// </summary>
        /// <returns>The testing home page</returns>
        [Route("Index")]
        public IActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// The test case for item CRUD functionality
        /// </summary>
        /// <returns>The test results in HTML format</returns>
        [Route("ItemService")]
        public IActionResult TestItemService()
        {
            List<string> resultsList = new List<string>();

            ItemDTO? itemDTO = null;

            resultsList.AddRange(CreateItemTest(out itemDTO));

            resultsList.AddRange(UpdateItemTest(itemDTO));

            ViewBag.results = resultsList;

            return PartialView("TestResults");
        }

        private List<string> CreateItemTest(out ItemDTO? itemDTO)
        {
            string
                itemName = "Test Item",
                itemDescription = "Test Item Description",
                modificationName = "Test Modification",
                modificationDescription = "Test Modification Description";
            decimal
                itemPrice = 13.59m,
                modificationPrice = 2.12m;
            int
                itemStock = 500000,
                modificationStock = 123456;
            bool
                hiddenItem = true,
                hiddenModification = true,
                defaultModification = true,
                createdItem = false,
                createdModification = false,
                retrievedItem = false,
                matchedItem = false,
                matchedModification = false;
            int
                createdItemId,
                createdModificationId;

            createdItemId = itemService.CreateItem(itemName, itemDescription, itemPrice, itemStock, hiddenItem);

            createdItem = createdItemId != -1;

            if (createdItem)
            {
                createdModificationId = modificationService.CreateModification(modificationName, modificationDescription, modificationPrice, modificationStock, defaultModification, hiddenModification, createdItemId);

                createdModification = createdModificationId != -1;

                ItemModel? itemModel = itemService.GetItem(createdItemId);

                if (itemModel != null)
                    itemDTO = new ItemDTO(itemModel, modificationService.GetModificationsByItemId(createdItemId));
                else
                    itemDTO = null;

                retrievedItem = itemDTO != null;

                if (retrievedItem && itemDTO != null)
                {
                    matchedItem =
                        String.Compare(itemDTO.ItemModel.name, itemName) == 0 &&
                        String.Compare(itemDTO.ItemModel.description, itemDescription) == 0 &&
                        itemDTO.ItemModel.price == itemPrice &&
                        itemDTO.ItemModel.stock == itemStock &&
                        itemDTO.ItemModel.hidden == hiddenItem;

                    matchedModification =
                        itemDTO.Modifications.Count == 1 &&
                        String.Compare(itemDTO.Modifications[0].name, modificationName) == 0 &&
                        String.Compare(itemDTO.Modifications[0].description, modificationDescription) == 0 &&
                        itemDTO.Modifications[0].price_offset == modificationPrice &&
                        itemDTO.Modifications[0].stock == modificationStock &&
                        itemDTO.Modifications[0].defaultModification == defaultModification &&
                        itemDTO.Modifications[0].hidden == hiddenModification;
                }
            }
            else itemDTO = null;

            return new List<string>
            {
                "--Item Creation Test Results",
                $"Created Item: {createdItem}",
                $"Created Modification: {createdModification}",
                $"Retrieved Item: {retrievedItem}",
                $"Matched Item: {matchedItem}",
                $"Matched Modification: {matchedModification}",
                $"Test Results: {(createdItem && createdModification && retrievedItem && matchedItem && matchedModification ? "Passed" : "Failed")}"
            };
        }

        private List<string> UpdateItemTest(ItemDTO? itemDTO)
        {
            if (itemDTO == null)
            {
                return new List<string>
                {
                    "--Item Update Test Results",
                    "Failed Due to Null Item"
                };
            }

            string
                itemName = "Updated Test Item",
                itemDescription = "Updated Test Item Description",
                modificationName = "Updated Test Modification",
                modificationDescription = "Updated Test Modification Description";
            decimal
                itemPrice = 19.59m,
                modificationPrice = 3.99m;
            int
                itemStock = 750000,
                modificationStock = 234567;
            bool
                hiddenItem = false,
                hiddenModification = false,
                defaultModification = false,
                updatedItem = false,
                updatedModification = false,
                retrievedItem = false,
                matchedItem = false,
                matchedModification = false;
            int affectedRows;

            itemDTO.ItemModel.name = itemName;
            itemDTO.ItemModel.description = itemDescription;
            itemDTO.ItemModel.price = itemPrice;
            itemDTO.ItemModel.stock = itemStock;
            itemDTO.ItemModel.hidden = hiddenItem;

            itemDTO.Modifications[0].name = modificationName;
            itemDTO.Modifications[0].description = modificationDescription;
            itemDTO.Modifications[0].price_offset = modificationPrice;
            itemDTO.Modifications[0].stock = modificationStock;
            itemDTO.Modifications[0].hidden = hiddenModification;
            itemDTO.Modifications[0].defaultModification = defaultModification;

            affectedRows = itemService.UpdateItem(itemDTO.ItemModel);
            updatedItem = affectedRows == 1;

            affectedRows = modificationService.UpdateModification(itemDTO.Modifications[0]);
            updatedModification = affectedRows == 1;

            ItemModel? itemModel = itemService.GetItem(itemDTO.ItemModel.id);

            if (itemModel != null)
                itemDTO = new ItemDTO(itemModel, modificationService.GetModificationsByItemId(itemDTO.ItemModel.id));
            else
                itemDTO = null;

            retrievedItem = itemDTO != null;

            return new List<string>()
            {
                "--Item Update Test Results",
                $"Updated Item: {updatedItem}",
                $"Updated Modification: {updatedModification}"
            };
        }

        private List<string> DeleteItemTest(ItemDTO? itemDTO)
        {
            if (itemDTO == null)
            {
                return new List<string>
                {
                    "--Item Delete Test Results",
                    "Failed Due to Null Item"
                };
            }

            throw new NotImplementedException();
        }

        public IActionResult TestModificationService()
        {
            return View();
        }

        public IActionResult TestOrderService()
        {
            return View();
        }

        public IActionResult TestPasswordService()
        {
            return View();
        }

        public IActionResult TestStoreSettingsService()
        {
            return View();
        }

        public IActionResult TestUserService()
        {
            return View();
        }
    }
}
