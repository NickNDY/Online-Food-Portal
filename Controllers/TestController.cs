using Microsoft.AspNetCore.Mvc;
using Online_Food_Portal.Interfaces;
using Online_Food_Portal.Models;
using System.Collections;

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
        private readonly IUserService userService;

        public TestController(IItemService itemService, IModificationService modificationService, IOrderService orderService, IPasswordService passwordService, IStoreSettingsService storeSettingsService, IUserService userService)
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
        [Route("")]
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
            ViewBag.results = ItemAndModificationTest();

            return PartialView("TestResults");
        }

        #region Item & Modification Tests
        private List<string> ItemAndModificationTest()
        {
            List<string> results = new List<string>();
            ItemModel itemModel;
            ModificationModel modificationModel;

            results.AddRange(CreateItemTest(out itemModel));
            results.AddRange(ReadItemTest(itemModel));
            results.AddRange(UpdateItemTest(itemModel));

            results.AddRange(CreateModificationTest(itemModel, out modificationModel));
            results.AddRange(ReadModificationTest(modificationModel));
            results.AddRange(UpdateModificationTest(modificationModel));

            results.AddRange(DeleteItemAndModificationTest(itemModel, modificationModel));

            // Summarize results
            if (results.Where(x => x.EndsWith("Fail")).Count() == 0)
                results.Insert(0, "Test Results: Pass");
            else
                results.Insert(0, "Test Results: Fail");

            return results;
        }

        private List<string> CreateItemTest(out ItemModel itemModel)
        {
            ///
            // [CREATE Item]
            ///
            // Define new item
            itemModel = new ItemModel(-1, "Test Item", "Test Item Description", 13.59m, 500000, false);

            // Create item and get newly created entry ID
            itemModel.id = itemService.CreateItem(itemModel.name, itemModel.description, itemModel.price, itemModel.stock, itemModel.hidden);

            return new List<string>
            {
                "--CREATE Item Test",
                $"Created Item: {(itemModel.id != -1 ? "Pass" : "Fail")}"
            };
        }

        private List<string> ReadItemTest(ItemModel itemModel)
        {
            ///
            // [READ Item]
            ///
            // Compare stored item
            List<string> results = new List<string> { "--READ Item Test" };
            results.AddRange(CompareStoredItem(itemModel));

            return results;
        }

        private List<string> UpdateItemTest(ItemModel itemModel)
        {
            ///
            // [UPDATE Item]
            ///
            // Change item
            itemModel.name += " 2";
            itemModel.description += " 2";
            itemModel.price += 1.5m;
            itemModel.stock += 250000;
            itemModel.hidden = true;

            // Update item
            List<string> results = new List<string>
            {
                "--UPDATE Item Test",
                $"Updated Item: {(itemService.UpdateItem(itemModel) == 1 ? "Pass" : "Fail")}"
            };
            results.AddRange(CompareStoredItem(itemModel));

            return results;
        }

        private List<string> DeleteItemAndModificationTest(ItemModel itemModel, ModificationModel modificationModel)
        {
            return new List<string>
            {
                "--DELETE Modification Test",
                $"Deleted Modification: {(modificationService.DeleteModification(modificationModel.id) == 1 ? "Pass" : "Fail")}",
                $"Confirmed Modification Deletion: {(modificationService.GetModification(modificationModel.id) == null ? "Pass" : "Fail")}",
                $"Recreated Modification: {((modificationModel.id = modificationService.CreateModification(modificationModel.name, modificationModel.description, modificationModel.price_offset, modificationModel.stock, modificationModel.defaultModification, modificationModel.hidden, modificationModel.items_id)) != -1 ? "Pass" : "Fail")}",
                "--DELETE Item Test",
                $"Deleted Item: {(itemService.DeleteItem(itemModel.id) == 1 ? "Pass" : "Fail")}",
                $"Confirmed Item Deletion: {(itemService.GetItem(itemModel.id) == null ? "Pass" : "Fail")}",
                $"Confirmed Cascade Modification Deletion: {(modificationService.GetModification(modificationModel.id) == null ? "Pass" : "Fail")}"
            };
        }

        private List<string> CreateModificationTest(ItemModel itemModel, out ModificationModel modificationModel)
        {
            ///
            // [CREATE Modification]
            ///
            //Define new modification
            modificationModel = new ModificationModel(-1, "Test Modification", "Test Modification Description", 2.12m, 123456, true, false, itemModel.id);

            // Create modification and get newly created entry ID
            modificationModel.id = modificationService.CreateModification(modificationModel.name, modificationModel.description, modificationModel.price_offset, modificationModel.stock, modificationModel.defaultModification, modificationModel.hidden, modificationModel.items_id);

            return new List<string>
            {
                "--CREATE Modification Test",
                $"Created Modification: {(modificationModel.id != -1 ? "Pass" : "Fail")}"
            };
        }

        private List<string> ReadModificationTest(ModificationModel modificationModel)
        {
            ///
            // [READ Modification]
            ///
            List<string> results = new List<string> { "--READ Modification Test" };
            results.AddRange(CompareStoredModification(modificationModel));

            return results;
        }

        private List<string> UpdateModificationTest(ModificationModel modificationModel)
        {
            ///
            // [UPDATE Modification]
            ///
            // Change modification
            modificationModel.name += " 4";
            modificationModel.description += " 4";
            modificationModel.price_offset += 0.25m;
            modificationModel.stock += 1234;
            modificationModel.defaultModification = false;
            modificationModel.hidden = true;

            // Update modification
            List<string> results = new List<string>
            {
                "--UPDATE Modification Test",
                $"Updated Modification: {(modificationService.UpdateModification(modificationModel) == 1 ? "Pass" : "Fail")}"
            };
            results.AddRange(CompareStoredModification(modificationModel));

            return results;
        }

        private List<string> CompareStoredItem(ItemModel itemModel)
        {
            ItemModel? retrievedItem = itemService.GetItem(itemModel.id);

            return new List<string>
            {
                $"Retrieved Item: {(retrievedItem != null ? "Pass" : "Fail")}",
                $"Matched Item: {(itemModel.Equals((object?)retrievedItem) ? "Pass" : "Fail")}"
            };
        }

        private List<string> CompareStoredModification(ModificationModel modificationModel)
        {
            ModificationModel? retrievedModification = modificationService.GetModification(modificationModel.id);

            return new List<string>
            {
                $"Retrieved Modification: {(retrievedModification != null ? "Pass" : "Fail")}",
                $"Matched Modification: {(modificationModel.Equals((object?)retrievedModification) ? "Pass" : "Fail")}"
            };
        }

        #endregion

        [Route("OrderService")]
        public IActionResult TestOrderService()
        {
            List<string> results = new List<string>();

            ///
            // [CREATE Order]
            //
            int createdOrderId = orderService.CreateOrder(1);
            results.Add("--CREATE Order Test");
            results.Add($"Created Order: {(createdOrderId > 0 ? "Pass" : "Fail")}");

            ///
            // [READ Order]
            ///
            OrderModel? createdOrder = orderService.GetOrder(createdOrderId);
            results.Add("--READ Order Test");
            results.Add($"Read Order: {(createdOrder != null ? "Pass" : "Fail")}");

            ///
            // Create/Read Failure Short-Circuit
            ///
            if (createdOrder == null)
            {
                results.Add("--UPDATE Order Test");
                results.Add("Updated Order: Fail");

                results.Add("--DELETE Order Test");
                results.Add("Deleted Order: Fail");

                ViewBag.results = results;

                return PartialView("TestResults");
            }

            ///
            // [UPDATE Order]
            ///
            createdOrder.subtotal = 45.23m;
            createdOrder.date_placed = DateTime.Now.AddSeconds(30);
            createdOrder.submitted = !createdOrder.submitted;
            createdOrder.cancelled = !createdOrder.cancelled;
            createdOrder.completed = !createdOrder.completed;
            createdOrder.picked_up = !createdOrder.picked_up;

            results.Add("--UPDATE Order Test");
            results.Add($"Updated Order: {(orderService.UpdateOrder(createdOrder) == 1 ? "Pass" : "Fail")}");

            ///
            // [READ Updated Order]
            ///
            OrderModel? updatedOrder = orderService.GetOrder(createdOrderId);
            results.Add($"Verified Updated Order: {(createdOrder.Equals((object?)updatedOrder) ? "Pass" : "Fail")}");

                ///
                // [DELETE Order]
                ///
                results.Add("--DELETE Order Test");
            results.Add($"Deleted Order: {(orderService.DeleteOrder(createdOrderId) == 1 ? "Pass" : "Fail")}");
            results.Add($"Verified Deleted Order: {(orderService.GetOrder(createdOrderId) == null ? "Pass" : "Fail")}");

            // Summarize results
            if (results.Where(x => x.EndsWith("Fail")).Count() == 0)
                results.Insert(0, "Test Results: Pass");
            else
                results.Insert(0, "Test Results: Fail");

            ViewBag.results = results;

            return PartialView("TestResults");

        }

        [Route("PasswordService")]
        public IActionResult TestPasswordService()
        {
            List<string> results = new List<string>();

            // Generate randomized password
            string password = $"Test Password {new Random().Next(5000, 10000)}";
            // Encrypt password
            string encryptedPassword = passwordService.EncryptPassword(password);

            // Test password != encrypted password
            results.Add("--Encrypt Password Test");
            results.Add($"Encrypted Password: {(string.Compare(password, encryptedPassword) != 0 ? "Pass" : "Fail")}");
            results.Add($"Password Encryption: {password} -> {encryptedPassword}");

            // Test encrypted password verifies with stored password
            results.Add("--Verify Password Test");
            results.Add($"Verified Password: {(passwordService.VerifyPassword(password, encryptedPassword) ? "Pass" : "Fail")}");

            // Summarize results
            if (results.Where(x => x.EndsWith("Fail")).Count() == 0)
                results.Insert(0, "Test Results: Pass");
            else
                results.Insert(0, "Test Results: Fail");

            ViewBag.results = results;

            return PartialView("TestResults");
        }

        [Route("StoreService")]
        public IActionResult TestStoreSettingsService()
        {
            List<string> results = new List<string>();

            // Store old settings
            StoreSettingsModel originalSettings = storeSettingsService.GetStoreSettings();

            // Generate new randomized settings
            Random rnd = new Random();
            bool[] array = new bool[] { rnd.NextDouble() >= 0.5, rnd.NextDouble() >= 0.5, rnd.NextDouble() >= 0.5, rnd.NextDouble() >= 0.5, rnd.NextDouble() >= 0.5, rnd.NextDouble() >= 0.5, rnd.NextDouble() >= 0.5 };
            StoreSettingsModel newSettings = new StoreSettingsModel(TimeSpan.FromMinutes(rnd.Next(50, 1440)), TimeSpan.FromMinutes(rnd.Next(50, 1440)), array, false, "New Address", "New Phone");

            // Test saving new settings
            results.Add("--UPDATE Settings Test");
            results.Add($"Save Test: {(storeSettingsService.SetStoreSettings(newSettings) == 1 ? "Pass" : "Fail")}");

            // Test retrieving new settings
            results.Add("--READ Settings Test");
            results.Add($"Read Test: {(newSettings.Equals(storeSettingsService.GetStoreSettings()) ? "Pass" : "Fail")}");

            // Restore old settings
            storeSettingsService.SetStoreSettings(originalSettings);

            // Summarize results
            if (results.Where(x => x.EndsWith("Fail")).Count() == 0)
                results.Insert(0, "Test Results: Pass");
            else
                results.Insert(0, "Test Results: Fail");

            ViewBag.results = results;

            return PartialView("TestResults");
        }

        [Route("UserService")]
        public IActionResult TestUserService()
        {
            List<string> results = new List<string>();

            // Generate randomized user name
            Random rnd = new Random();
            string username = $"Test User {rnd.Next(1000, 500000)}";

            // Test creating new user
            results.Add("--CREATE User Test");
            results.Add($"Create User: {(userService.CreateUser(username) == 1 ? "Pass" : "Fail")}");

            // Test retrieving new user
            UserModel? user = userService.GetUserByUsername(username);
            results.Add("--READ User Test");
            results.Add($"Read User: {(user != null ? "Pass" : "Fail")}");

            // Test deleting user by id
            if (user == null)
            {
                results.Add("--DELETE User Test By ID");
                results.Add("Delete User By ID: Fail");
            }
            else
            {
                results.Add("--DELETE User Test By ID");
                results.Add($"Delete User By ID: {(userService.DeleteUserById(user.id) == 1 ? "Pass" : "Fail")}");
            }

            // Verify user deleted
            if (user == null)
            {
                results.Add("--Verify User Deletion Test");
                results.Add("Verify User Deletion: Fail");
            }
            else
            {
                results.Add("--Verify User Deletion Test");
                results.Add($"Verify User Deletion: {(userService.GetUserByUsername(user.username) == null ? "Pass" : "Fail")}");
            }

            // Summarize results
            if (results.Where(x => x.EndsWith("Fail")).Count() == 0)
                results.Insert(0, "Test Results: Pass");
            else
                results.Insert(0, "Test Results: Fail");

            ViewBag.results = results;

            return PartialView("TestResults");
        }
    }
}
