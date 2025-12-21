namespace Online_Food_Portal.Models
{
    public class OrderDTO
    {
        public OrderModel order { get; set; }
        public List<OrderItemDTO> items { get; set; }

        public decimal subtotal { get { return items.Sum(x => x.OrderItemModel.itemModel.price + x.OrderModificationModels.Sum(y => y.modificationModel.price_offset)); } }

        public OrderDTO(OrderModel order, List<OrderItemDTO> items)
        {
            this.order = order;
            this.items = items;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is OrderDTO)) return false;

            return Equals((OrderDTO)obj);
        }

        public bool Equals(OrderDTO other)
        {
            return
                order.Equals(other.order) &&
                CompareItems(other.items);
        }

        private bool CompareItems(List<OrderItemDTO> otherItems)
        {
            if (items.Count != otherItems.Count) return false;

            for (int i = 0; i < items.Count; i++)
                if (!items[i].Equals(otherItems[i])) return false;

            return true;
        }
    }
}
