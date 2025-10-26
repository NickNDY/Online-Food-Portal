using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Online_Food_Portal.Models
{
    /// <summary>
    /// Store Settings model for handling data
    /// </summary>
    public class StoreSettingsModel
    {
        [DisplayName("Store Opening Time")]
        [DataType(DataType.Time)]
        public TimeSpan open_time { get; set; }

        [DisplayName("Store Closing Time")]
        [DataType(DataType.Time)]
        public TimeSpan close_time { get; set; }

        [DisplayName("Store Business Days")]
        public BitArray business_days { get; set; }

        [DisplayName("Store Ordering Enabled")]
        public bool ordering_enabled { get; set; }

        [DisplayName("Store Address")]
        public string store_address { get; set; }

        [DisplayName("Store Phone Number")]
        public string store_phone { get; set; }

        public StoreSettingsModel(TimeSpan open_time, TimeSpan close_time, BitArray business_days, bool ordering_enabled, string store_address, string store_phone)
        {
            this.open_time = open_time;
            this.close_time = close_time;
            this.business_days = business_days;
            this.ordering_enabled = ordering_enabled;
            this.store_address = store_address;
            this.store_phone = store_phone;
        }
    }
}
