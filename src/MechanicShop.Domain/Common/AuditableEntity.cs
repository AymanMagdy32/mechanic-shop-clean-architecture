namespace MechanicShop.Domain.Common
{
    public abstract class AuditableEntity : Entity
    {
        protected AuditableEntity() 
        {}
       protected AuditableEntity(Guid id ) : base(id)
       {}
        public DateTimeOffset CreatedAt { get; set; } 
        public string? CreatedBy { get; set; } = string.Empty; 
        public DateTimeOffset? LastModifiedAt { get; set; } 
        public string? LastModifiedBy { get; set; } = string.Empty; 


    }
} 