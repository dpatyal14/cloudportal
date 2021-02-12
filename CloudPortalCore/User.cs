using Newtonsoft.Json;

namespace CloudPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
    public enum AuthenticationMode
    {
        AppDriver,
        AppKiosk,
        AppWarehouse,
        Portal
    }

    public enum UserPriveleges
    {
        DeleteShipment,
        OutboundShipments,
        Deliveries,
        AddUsers,
        UnlockShipments,
        WarehouseApp
    }
}
