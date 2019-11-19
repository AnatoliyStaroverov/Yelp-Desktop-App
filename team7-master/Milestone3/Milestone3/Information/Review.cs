namespace Team7GUI
{
    class Review : Information
    {
        public string Reviewid { get; set; }
        public string Userid { get; set; }
        public string Businessid { get; set; }
        public string Reviewtext { get; set; }
        public string Username { get; set; }
        public string Businessname { get; set; }
        public double Stars { get; set; }
        public string Reviewdate { get; set; }
        public int Funny { get; set; }
        public int Useful { get; set; }
        public int Cool { get; set; }

        public Review() { }
    }
}
