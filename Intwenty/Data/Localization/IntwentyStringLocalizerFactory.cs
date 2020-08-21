using Intwenty.Model;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Intwenty.Data.Localization
{
    
    public class IntwentyStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly ConcurrentDictionary<string, IntwentyStringLocalizer> Cache = new ConcurrentDictionary<string, IntwentyStringLocalizer>();

        private IIntwentyDataService DataRepository { get; }
        private IntwentySettings Settings { get; }

        public IntwentyStringLocalizerFactory(IIntwentyDataService ds, IOptions<IntwentySettings> settings)
        {
            DataRepository = ds;
            Settings = settings.Value; ;
        }

        public IStringLocalizer Create(string basename, string location)
        {
            if (basename == null)
            {
                throw new ArgumentNullException(nameof(basename));
            }

            IntwentyStringLocalizer value = null;
            if (Cache.TryGetValue(basename, out value))
            {
                return value;
            }

            value = new IntwentyStringLocalizer(DataRepository, Settings);

            Cache.TryAdd(basename, value);

            return value;
            
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var basename = resourceSource.GetTypeInfo().FullName;

            IntwentyStringLocalizer value = null;
            if (Cache.TryGetValue(basename, out value))
            {
                return value;
            }

            value = new IntwentyStringLocalizer(DataRepository, Settings);

            Cache.TryAdd(basename, value);

            return value;
        }
    }
    
}
