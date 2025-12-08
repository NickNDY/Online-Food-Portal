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

        public bool setModification { get; set; }

        public ModificationModel modificationModel { get; set; }

        public OrderModificationModel(int id, int order_items_id, int modifications_id, ModificationModel modificationModel)
        {
            this.id = id;
            this.order_items_id = order_items_id;
            this.modifications_id = modifications_id;
            this.modificationModel = modificationModel;
        }
    }
}
