using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace Open.Core.Extensions.Nhibernate.Tests.Maps
{
    public class Image : IEntity
    {
        public virtual int Id { get; set; }
        public virtual byte[] image { get; set; }
        
        public virtual string Name { get; set; }
    }
    public class ImageMap : ClassMap<Image> {
        public ImageMap()
        {
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.image).CustomType<BinaryBlobType>();
            Map(x => x.Name).Length(256);
        }
    }
}