namespace ContratacaoService.Domain.Entities.Base
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; set; } = false;

        protected Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        protected void RegisterUpdate()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            RegisterUpdate();
        }
    }
}
