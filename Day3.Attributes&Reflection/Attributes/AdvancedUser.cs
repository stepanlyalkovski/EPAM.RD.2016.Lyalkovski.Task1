using System.ComponentModel;

namespace Attributes
{
    public class AdvancedUser : User
    {
        public int _externalId;

        [DefaultValue(3443454)]
        public int ExternalId
        {
            get
            {
                return _externalId;
            }
        }

        [MatchParameterWithProperty("id", "Id")]
        [MatchParameterWithProperty("externalId", "ExternalId")]
        public AdvancedUser(int id, int externalId) : base(id)
        {
            _externalId = externalId;
        }

        public override bool Equals(object obj)
        {
            var other = obj as AdvancedUser;
            if (other == null)
                return false;

            var isEqual =  other.FirstName == this.FirstName
                   && other.LastName == this.LastName
                   && other.Id == this.Id
                   && other.ExternalId == this.ExternalId;

            return isEqual;
        }
    }
}
