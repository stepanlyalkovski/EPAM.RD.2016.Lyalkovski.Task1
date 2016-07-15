namespace Attributes
{
    // Should be applied to assembly only.
    public class InstantiateAdvancedUserAttribute : InstantiateUserAttribute
    {
        public int ExternalId { get; set; }
        public InstantiateAdvancedUserAttribute()
        {
            
        }
        public InstantiateAdvancedUserAttribute(int id, string firstName, string lastName, int externalId) : base(id, firstName, lastName)
        {            
            ExternalId = externalId;
        }

        
    }
}
