using Intwenty.Data.Entity;
using Intwenty.Model;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Intwenty.Data.Localization
{
    public class IntwentyStringLocalizer : IStringLocalizer
    {

        private IIntwentyDataService DataRepository { get; }
        private IntwentySettings Settings { get; }

        public IntwentyStringLocalizer(IIntwentyDataService ds, IntwentySettings settings)
        {
            DataRepository = ds;
            Settings = settings;
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));

                var list = DataRepository.GetDbObjectMapper().GetAll<TranslationItem>();
                var trans = list.Find(p => p.Key == name);
                if (trans == null)
                    return new LocalizedString(name, name);

                if (string.IsNullOrEmpty(trans.Text))
                    return new LocalizedString(name, name);

                return new LocalizedString(name, trans.Text);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null) throw new ArgumentNullException(nameof(name));

                var list = DataRepository.GetDbObjectMapper().GetAll<TranslationItem>();
                var trans = list.Find(p => p.Key == name);
                if (trans == null)
                    return new LocalizedString(name, name);

                if (string.IsNullOrEmpty(trans.Text))
                    return new LocalizedString(name, name);

                return new LocalizedString(name, trans.Text);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return DataRepository.GetDbObjectMapper().GetAll<TranslationItem>().Select(p => new LocalizedString(p.Key, p.Text)).ToList();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
