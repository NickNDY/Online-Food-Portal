namespace Online_Food_Portal.Models
{
    public class OrderItemDTO(OrderItemModel orderItemModel, List<OrderModificationModel> orderModificationModels)
    {
        public OrderItemModel OrderItemModel { get; set; } = orderItemModel;
        public List<OrderModificationModel> OrderModificationModels { get; set; } = orderModificationModels;
        public string Display { get { return $"{OrderItemModel.itemModel.name} x{OrderItemModel.quantity}"; } }
        public decimal Price { get { return (OrderItemModel.itemModel.price + OrderModificationModels.Sum(x => x.modificationModel.price_offset)) * (decimal)OrderItemModel.quantity; } }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is OrderItemDTO)) return false;

            return Equals((OrderItemDTO)obj);
        }

        public bool Equals(OrderItemDTO other)
        {
            return
                OrderItemModel.Equals(other.OrderItemModel) &&
                CompareOrderModifications(other.OrderModificationModels);
        }

        private bool CompareOrderModifications(List<OrderModificationModel> otherModifications)
        {
            if (OrderModificationModels.Count != otherModifications.Count) return false;

            for (int i = 0; i < OrderModificationModels.Count; i++)
                if (!OrderModificationModels[i].Equals(otherModifications[i])) return false;

            return true;
        }
    }
}
