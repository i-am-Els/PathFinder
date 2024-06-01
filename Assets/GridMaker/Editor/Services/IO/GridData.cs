namespace GridMaker.Editor.Services.IO
{
    public class GridData
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string BasePrefab { get; set; }
    }
}