using System.Collections.Generic;

namespace Oyadieyie3D.Models
{
    public class ExpandableListDataPump
    {
        public static Dictionary<string, List<string>> GetData()
        {
            var expandableListDetail = new Dictionary<string, List<string>>();
            List<string> location = new List<string>();
            location.Add("Kasoa");
            location.Add("Kasoa");
            location.Add("Kasoa");
            location.Add("Kasoa");
            location.Add("Kasoa");

            List<string> paymentMethods = new List<string>();
            paymentMethods.Add("Cash");
            paymentMethods.Add("Mobile money");
            paymentMethods.Add("Visa/Debit card");

            expandableListDetail.Add("Locations", location);
            expandableListDetail.Add("payment", paymentMethods);

            return expandableListDetail;
        }
    }
}