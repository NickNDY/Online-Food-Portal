namespace Online_Food_Portal.Models
{
    /// <summary>
    /// Order Model linking an order to a user with relevant status information
    /// </summary>
    public class OrderModel
    {
        public int id { get; set; }

        public decimal subtotal { get; set; }

        public DateTime date_placed { get; set; }

        public bool submitted { get; set; }

        public bool cancelled { get; set; }

        public bool completed { get; set; }

        public bool picked_up { get; set; }

        public int users_id { get; set; }

        public OrderModel(int id, decimal subtotal, DateTime date_placed, bool submitted, bool cancelled, bool completed, bool picked_up, int users_id)
        {
            this.id = id;
            this.subtotal = subtotal;
            this.date_placed = date_placed;
            this.submitted = submitted;
            this.cancelled = cancelled;
            this.completed = completed;
            this.picked_up = picked_up;
            this.users_id = users_id;
        }
    }
}
