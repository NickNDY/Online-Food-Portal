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

        public ItemModel itemModel { get; set; }

        public OrderItemModel(int id, int quantity, int orders_id, int items_id, ItemModel itemModel)
        {
            this.id = id;
            this.quantity = quantity;
            this.orders_id = orders_id;
            this.items_id = items_id;
            this.itemModel = itemModel;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is OrderItemModel)) return false;
            
            return Equals((OrderItemModel)obj);
        }

        public bool Equals(OrderItemModel other)
        {
            return
                id == other.id &&
                quantity == other.quantity &&
                orders_id == other.orders_id &&
                items_id == other.items_id &&
                itemModel.Equals(other.itemModel);
        }
    }
}
