namespace Online_Food_Portal.Models
{
    /// <summary>
    /// Order Modification model linking modifications to an order item
    /// </summary>
    public class OrderModificationModel
    {
        public int id { get; set; }

        public int order_items_id { get; set; }

        public int modifications_id { get; set; }
    }
}
