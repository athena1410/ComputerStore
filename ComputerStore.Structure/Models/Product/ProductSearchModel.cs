namespace ComputerStore.Structure.Models.Product
{
    public class ProductSearchModel
    {
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }

        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }
        public int[] CategoryIds { get; set; }

    }
}
