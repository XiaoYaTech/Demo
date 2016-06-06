namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreBEInfo : BaseEntity<StoreBEInfo>
    {

        public bool ISNewAttachedKiosk { get; set; }

        public bool ISNewRemoteKiosk { get; set; }

        public bool ISNewMcCafe { get; set; }

        public bool ISNewMDS { get; set; }
    }
}
