namespace BarBuddy.Server.Entities
{
    public class RegistrationToken : BaseEntity
    {
        // ALT:
        // public long LocationId { get; set; }
        // NEU:
        public long EntityId { get; set; }

        public string Token { get; set; }
    }
}
