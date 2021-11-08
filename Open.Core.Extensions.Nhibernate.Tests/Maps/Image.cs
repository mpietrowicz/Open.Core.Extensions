using FluentNHibernate.Mapping;
using NHibernate.Type;

namespace Open.Core.Extensions.Nhibernate.Tests.Maps
{
    public class Image
    {
        public virtual string Id { get; set; }
        public virtual byte[] image { get; set; }
        
        public virtual string Name { get; set; }
    }
    public class ImageMap : ClassMap<Image> {
        public ImageMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.image).CustomType<BinaryBlobType>();
        }
    }
}