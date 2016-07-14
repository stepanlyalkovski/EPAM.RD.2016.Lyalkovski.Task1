using System;
using System.Collections.Generic;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Entities
{
    [Serializable]
    public class User : IEquatable<User>, IEntity
    {
        public int Id { get; set; }
        public string PersonalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }

        public Gender Gender { get; set; } = new Gender();

        public List<VisaRecord> VisaRecords { get; set; }



        public bool Equals(User other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;
            if (this.GetType() != other.GetType())
                return false;
            return this.PersonalId == other.PersonalId
                   && this.FirstName == other.FirstName
                   && this.LastName == other.LastName;

        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;


            if (this.GetType() != other.GetType())
                return false;

            return this.Equals(other as User);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            //TODO Check for null
            hash = (hash * 7) + FirstName.GetHashCode();
            hash = (hash * 7) + LastName.GetHashCode();
            hash = (hash * 7) + PersonalId.GetHashCode();

            return hash;
        }
    }

    public enum Gender
    {
        None,
        Female,
        Male
    }
    [Serializable]
    public struct VisaRecord
    {
        public string Country { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}