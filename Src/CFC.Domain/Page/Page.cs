namespace CFC.Domain
{
    using SharpArch.Domain.DomainModel;

    public class Page : Entity
    {
        public virtual string Title { get; set; }
        public virtual string Name { get; set; }
        public virtual string Content { get; set; }
    }
}