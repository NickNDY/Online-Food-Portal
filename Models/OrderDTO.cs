namespace Online_Food_Portal.Models
{
    public class OrderDTO
    {
        public OrderModel order { get; set; }
        public List<OrderItemDTO> items { get; set; }

        public OrderDTO(OrderModel order, List<OrderItemDTO> items)
        {
            this.order = order;
            this.items = items;
        }
    }
}
