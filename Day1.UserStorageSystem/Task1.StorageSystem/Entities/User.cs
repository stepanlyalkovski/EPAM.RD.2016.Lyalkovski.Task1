namespace Task1.StorageSystem.Entities
{
    using System;
    using System.Collections.Generic;

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

    [Serializable]
    public class User : IEquatable<User>, ICloneable
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
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return PersonalId == other.PersonalId
                   && FirstName == other.FirstName
                   && LastName == other.LastName;
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return Equals(other as User);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            if (FirstName != null)
            {
                hash = (hash * 7) + FirstName.GetHashCode();
            }

            if (LastName != null)
            {
                hash = (hash * 7) + LastName.GetHashCode();
            }

            if (PersonalId != null)
            {
                hash = (hash * 7) + PersonalId.GetHashCode();
            }

            return hash;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public User Clone()
        {
            return CloneUtility.DeepClone(this);
        }
    }
}