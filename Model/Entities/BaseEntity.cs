using SQLite;

namespace TimesheetApp.Model.Entities
{
    public abstract class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public BaseEntity() { }
       
        public BaseEntity Clone() => MemberwiseClone() as BaseEntity;

        public virtual Task<bool?> Validate(object entities) => throw new NotImplementedException();
    }
}
