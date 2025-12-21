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
        public bool[] business_days { get; set; }

        [DisplayName("Store Ordering Enabled")]
        public bool ordering_enabled { get; set; }

        [DisplayName("Store Address")]
        public string store_address { get; set; }

        [DisplayName("Store Phone Number")]
        public string store_phone { get; set; }

        public StoreSettingsModel(TimeSpan open_time, TimeSpan close_time, bool[] business_days, bool ordering_enabled, string store_address, string store_phone)
        {
            this.open_time = open_time;
            this.close_time = close_time;
            this.business_days = business_days;
            this.ordering_enabled = ordering_enabled;
            this.store_address = store_address;
            this.store_phone = store_phone;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is StoreSettingsModel)) return false;

            return Equals((StoreSettingsModel)obj);
        }

        public bool Equals(StoreSettingsModel other)
        {
            return
                open_time == other.open_time &&
                close_time == other.close_time &&
                CompareBoolArray(business_days, other.business_days) &&
                ordering_enabled == other.ordering_enabled &&
                string.Compare(store_address, other.store_address) == 0 &&
                string.Compare(store_phone, other.store_phone) == 0;
        }

        private bool CompareBoolArray(bool[] a, bool[] b)
        {
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;

            return true;
        }
    }
}
