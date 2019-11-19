namespace Team7GUI
{
    class Users : Information
    {
        public string Userid { get; set; }
        public string Name { get; set; }
        public string Yelpingsince { get; set; }
        public int Reviewcount { get; set; }
        public int Fans { get; set; }
        public double Averagestars { get; set; }
        public int Funny { get; set; }
        public int Useful { get; set; }
        public int Cool { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Users() { }

        public override string ToString()
        {
            return Name + ", " + Userid;
        }
    }
}
