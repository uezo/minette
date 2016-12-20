namespace Minette.Message.Attachment
{
    public class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Location()
        {
            Latitude = 0;
            Longitude = 0;
        }
        public Location(decimal latitude, decimal longitude)
        {
            this.Latitude = latitude;
            this.Longitude = Longitude;
        }
        public Location(string latitude, string longitude)
        {
            this.Latitude = decimal.Parse(latitude);
            this.Longitude = decimal.Parse(longitude);
        }
    }
}
