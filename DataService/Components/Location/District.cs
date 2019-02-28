namespace DataService.Components.Location
{
    public class District
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Type { get; set; }
        public string Name_with_type { get; set; }
        public string Path { get; set; }
        public string Path_with_type { get; set; }
        public string Parent_code { get; set; }

        public District()
        {
        }
    }
}