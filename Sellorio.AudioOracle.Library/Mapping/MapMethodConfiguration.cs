using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;

namespace Sellorio.AudioOracle.Library.Mapping
{
    public class MapMethodConfiguration<TMapper>
    {
        private readonly IMapperConfigurationExpression _configure;
        private readonly List<(Type From, Type To)> _registeredMappers = new();

        internal MapMethodConfiguration(IMapperConfigurationExpression configure)
        {
            _configure = configure;
        }

        public MapMethodConfiguration<TMapper> AddWithConfig<TFrom, TTo>(Expression<Func<TFrom, TTo>> mapMethod, Action<IMappingExpression<TFrom, TTo>> configure)
        {
            var map = _configure.CreateMap<TFrom, TTo>();
            _registeredMappers.Add((typeof(TFrom), typeof(TTo)));
            configure?.Invoke(map);
            return this;
        }

        internal void AddRemainingMapMethods()
        {
            foreach (var method in typeof(TMapper).GetMethods().Where(x => x.ReturnType != null && x.ReturnType != typeof(void)))
            {
                var parameters = method.GetParameters();

                if (parameters.Length == 1)
                {
                    var from = parameters[0].ParameterType;
                    var to = method.ReturnType;

                    if (!_registeredMappers.Contains((from, to)))
                    {
                        _configure.CreateMap(from, to);
                    }
                }
            }
        }
    }
}
