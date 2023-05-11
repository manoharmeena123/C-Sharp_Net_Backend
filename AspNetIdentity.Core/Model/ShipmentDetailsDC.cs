using System.Collections.Generic;

namespace AngularJSAuthentication.DataContracts.ShipRocketDTO
{
    public class ShipmentDetailsDC
    {
        public List<Datum> data { get; set; }
        public Meta meta { get; set; }
    }

    public class Meta
    {
        public Pagination pagination { get; set; }
    }

    public class Datum
    {
        public string number { get; set; }
        public string code { get; set; }
        public int id { get; set; }
        public int order_id { get; set; }
        public List<Product> products { get; set; }
        public string awb { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public int channel_id { get; set; }
        public string channel_name { get; set; }
        public string base_channel_code { get; set; }
        public string payment_method { get; set; }
        public Charges charges { get; set; }
    }

    public class Links
    {
    }

    public class Pagination
    {
        public int total { get; set; }
        public int count { get; set; }
        public int per_page { get; set; }
        public int current_page { get; set; }
        public int total_pages { get; set; }
        public Links links { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public string sku { get; set; }
        public int quantity { get; set; }
    }

    public class Charges
    {
        public string zone { get; set; }
        public string cod_charges { get; set; }
        public string applied_weight_amount { get; set; }
        public string freight_charges { get; set; }
        public string applied_weight { get; set; }
        public string charged_weight { get; set; }
        public string charge_weight_amount { get; set; }
        public string charged_weight_amount_rto { get; set; }
        public string applied_weight_amount_rto { get; set; }
    }
}