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

        public OrderModificationModel(int id, int modifications_id, int order_items_id, bool setModification, ModificationModel modificationModel)
        {
            this.id = id;
            this.modifications_id = modifications_id;
            this.order_items_id = order_items_id;
            this.setModification = setModification;
            this.modificationModel = modificationModel;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is OrderModificationModel)) return false;
            
            return Equals((OrderModificationModel)obj);
        }

        public bool Equals(OrderModificationModel other)
        {
            return
                id == other.id &&
                order_items_id == other.order_items_id &&
                modifications_id == other.modifications_id &&
                setModification == other.setModification &&
                modificationModel.Equals(other.modificationModel);
        }
    }
}
