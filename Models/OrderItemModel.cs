namespace Online_Food_Portal.Models
{
    /// <summary>
    /// Order Item model linking an item to an order
    /// </summary>
    public class OrderItemModel
    {
        public int id { get; set; }

        public int quantity { get; set; }

        public int orders_id { get; set; }

        public int items_id { get; set; }

        public OrderItemModel(int id, int quantity, int orders_id, int items_id)
        {
            this.id = id;
            this.quantity = quantity;
            this.orders_id = orders_id;
            this.items_id = items_id;
        }
    }
}
