namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreSTLLRecord : BaseEntity<StoreSTLLRecord>
    {
        public static StoreSTLLRecord Get(string usCode)
        {
            return FirstOrDefault(e => e.StoreCode == usCode);
        }

        public void Save()
        {
            if (Any(e => e.Id == this.Id))
            {
                this.Update();
            }
            else
            {
                this.Add();
            }
        }
    }
}
