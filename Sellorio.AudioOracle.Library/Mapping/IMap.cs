namespace Sellorio.AudioOracle.Library.Mapping;

public interface IMap<TFrom, TTo>
{
    TTo Map(TFrom from);
}
