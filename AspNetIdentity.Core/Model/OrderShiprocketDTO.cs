using System;
using System.Collections.Generic;

namespace AngularJSAuthentication.DataContracts.ShipRocketDTO
{
    public class OrderShiprocketDTO
    {
        public class OrderItem
        {
            public string name { get; set; }
            public string sku { get; set; }
            public int units { get; set; }
            public string selling_price { get; set; }
            public string discount { get; set; }
            public string tax { get; set; }
            public int hsn { get; set; }
        }

        public class ShiprocketCreateRoot
        {
            public string order_id { get; set; }
            public string order_date { get; set; }
            public string pickup_location { get; set; }
            public string channel_id { get; set; }
            public string comment { get; set; }
            public string billing_customer_name { get; set; }
            public string billing_last_name { get; set; }
            public string billing_address { get; set; }
            public string billing_address_2 { get; set; }
            public string billing_city { get; set; }
            public string billing_pincode { get; set; }
            public string billing_state { get; set; }
            public string billing_country { get; set; }
            public string billing_email { get; set; }
            public string billing_phone { get; set; }
            public bool shipping_is_billing { get; set; }
            public string shipping_customer_name { get; set; }
            public string shipping_last_name { get; set; }
            public string shipping_address { get; set; }
            public string shipping_address_2 { get; set; }
            public string shipping_city { get; set; }
            public string shipping_pincode { get; set; }
            public string shipping_country { get; set; }
            public string shipping_state { get; set; }
            public string shipping_email { get; set; }
            public string shipping_phone { get; set; }
            public List<OrderItem> order_items { get; set; }
            public string payment_method { get; set; }
            public int shipping_charges { get; set; }
            public int giftwrap_charges { get; set; }
            public int transaction_charges { get; set; }
            public int total_discount { get; set; }
            public int sub_total { get; set; }
            public int length { get; set; }
            public int breadth { get; set; }
            public int height { get; set; }
            public double weight { get; set; }
        }

        public class CreateCustomOrderRoot
        {
            public int order_id { get; set; }
            public int shipment_id { get; set; }
            public string status { get; set; }
            public int status_code { get; set; }
            public int onboarding_completed_now { get; set; }
            public string awb_code { get; set; }
            public string courier_company_id { get; set; }
            public string courier_name { get; set; }
        }

        public class TrackOrderData
        {
            public int track_status { get; set; }

            //public int shipment_status { get; set; }
            public List<ShipmentTrack> ShipmentTracks { get; set; }
        }

        public class ShipmentTrack
        {
            public int id { get; set; }
            public string awb_code { get; set; }
            public int courier_company_id { get; set; }
            public int shipment_id { get; set; }
            public int order_id { get; set; }
            public DateTime pickup_date { get; set; }
            public DateTime delivered_date { get; set; }
            public string weight { get; set; }
            public int packages { get; set; }
            public string current_status { get; set; }
            public string delivered_to { get; set; }
            public string destination { get; set; }
            public string consignee_name { get; set; }
            public string origin { get; set; }
            public string courier_agent_details { get; set; }
        }

        public class ShipmentTrackDc
        {
            public List<ShipmentTrack> ShipmentTracks { get; set; }
        }
    }
}