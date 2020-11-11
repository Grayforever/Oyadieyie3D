namespace Oyadieyie3D.Models
{
    public class Client
    {
        public string ClientName { get; set; }

        public double Rating { get; set; }

        public string ItemTitle { get; set; }

        public string ItemDescription { get; set; }

        public int ItemImgUrl { get; set; }

        public int MapImgUrl { get; set; }

        public string OpeningHours { get; set; }

        public Client(Builder builder)
        {
            ClientName = builder.ClientName;
            Rating = builder.Rating;
            ItemTitle = builder.ItemTitle;
            ItemDescription = builder.ItemDescription;
            ItemImgUrl = builder.ItemImgUrl;
            MapImgUrl = builder.MapImgUrl;
            OpeningHours = builder.OpeningHours;
        }

        public class Builder
        {
            public string ClientName;

            public double Rating;

            public string ItemTitle;

            public string ItemDescription;

            public int ItemImgUrl;

            public int MapImgUrl;

            public string OpeningHours;

            public Builder SetClientName(string clientName)
            {
                ClientName = clientName;
                return this;
            }

            public Builder SetRating(double rating)
            {
                Rating = rating;
                return this;
            }

            public Builder SetItemTitle(string itemTitle)
            {
                ItemTitle = itemTitle;
                return this;
            }

            public Builder SetItemDescription(string description)
            {
                ItemDescription = description;
                return this;
            }

            public Builder SetImageUrl(int imgUrl)
            {
                ItemImgUrl = imgUrl;
                return this;
            }

            public Builder SetMapUrl(int mapUrl)
            {
                MapImgUrl = mapUrl;
                return this;
            }

            public Builder SetOpeningHours(string hours)
            {
                OpeningHours = hours;
                return this;
            }

            public Client Build()
            {
                return new Client(this);
            }
        }
    }
}