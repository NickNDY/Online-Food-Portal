namespace Online_Food_Portal.Models
{
    public class ItemDTO
    {
        public ItemModel ItemModel { get; set; }
        public List<ModificationModel> Modifications { get; set; }

        public decimal DefaultPrice { get { return ItemModel.price + Modifications.Where(x => x.defaultModification).Sum(x => x.price_offset); } }

        public ItemDTO(ItemModel itemModel, List<ModificationModel> modifications)
        {
            ItemModel = itemModel;
            Modifications = modifications;
        }
    }
}
