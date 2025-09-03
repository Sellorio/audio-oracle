using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sellorio.AudioOracle.Library.Mapping;

public abstract class StaticMapperBase<TMapper>
{
    private readonly IMapper _mapper;

    protected StaticMapperBase()
    {
        _mapper =
            new MapperConfiguration(o =>
            {
                var configuration = new MapMethodConfiguration<TMapper>(o);
                Configure(configuration);
                configuration.AddRemainingMapMethods();
            },
            NullLoggerFactory.Instance)
                .CreateMapper();
    }

    protected virtual void Configure(MapMethodConfiguration<TMapper> configure)
    {
    }

    protected TTo Map<TTo>(object from)
    {
        return _mapper.Map<TTo>(from);
    }
}
