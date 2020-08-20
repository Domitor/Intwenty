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
        private IIntwentyDataService DataRepository { get; }
        private IntwentySettings Settings { get; }

        public IntwentyStringLocalizerFactory(IIntwentyDataService ds, IOptions<IntwentySettings> settings)
        {
            DataRepository = ds;
            Settings = settings.Value; ;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            return new IntwentyStringLocalizer(DataRepository, Settings);
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new IntwentyStringLocalizer(DataRepository, Settings);
        }
    }
    
}
