namespace Team7GUI
{
    public class Business : Information
    {
        public string Businessid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int Zipcode { get; set; }
        public double Stars { get; set; }
        public int Reviewcount { get; set; }
        public int Numcheckins { get; set; }
        public double Reviewrating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Isopen { get; set; }

        public Business() { }

        public override string ToString()
        {
            return "Current Business: " + Name;
        }
    }
}
