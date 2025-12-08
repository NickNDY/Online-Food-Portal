namespace Online_Food_Portal.Models
{
    public class OrderItemDTO(OrderItemModel orderItemModel, List<OrderModificationModel> orderModificationModels)
    {
        public OrderItemModel OrderItemModel { get; set; } = orderItemModel;
        public List<OrderModificationModel> OrderModificationModels { get; set; } = orderModificationModels;
        public string Display { get { return $"{OrderItemModel.itemModel.name} x{OrderItemModel.quantity}"; } }
    }
}
