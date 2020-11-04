﻿using Intwenty.Entity;
using Intwenty.Interface;
using Intwenty.Model;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Intwenty.Localization
{
    public class IntwentyStringLocalizer : IStringLocalizer
    {

        private IIntwentyModelService ModelService { get; }
        private IntwentySettings Settings { get; }

        public IntwentyStringLocalizer(IIntwentyModelService ms, IntwentySettings settings)
        {
            ModelService = ms;
            Settings = settings;
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null) 
                    throw new ArgumentNullException(nameof(name));

                /*
                 * HOW TO GET CURRENT CULTURE
                System.Threading.Thread.CurrentThread.CurrentUICulture 
                System.Threading.Thread.CurrentThread.CurrentCulture      
                System.Globalization.CultureInfo.CurrentUICulture 
                System.Globalization.CultureInfo.CurrentCulture 
                Settings.Value.DefaultCulture 
                */


                string culture = Settings.DefaultCulture;

                if (Settings.LocalizationMethod != LocalizationMethods.SiteLocalization)
                   culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

                if (string.IsNullOrEmpty(culture))
                    throw new InvalidOperationException("Can't get current culture");

                var list = ModelService.GetTranslations();
                var trans = list.Find(p => p.Key == name && p.Culture == culture);
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
                if (name == null) 
                    throw new ArgumentNullException(nameof(name));


                string culture = Settings.DefaultCulture;

                if (Settings.LocalizationMethod != LocalizationMethods.SiteLocalization)
                    culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

                if (string.IsNullOrEmpty(culture))
                    throw new InvalidOperationException("Can't get current culture");

                var list = ModelService.GetTranslations();
                var trans = list.Find(p => p.Key == name && p.Culture == culture);
                if (trans == null)
                    return new LocalizedString(name, name);

                if (string.IsNullOrEmpty(trans.Text))
                    return new LocalizedString(name, name);

                if (arguments != null)
                    return new LocalizedString(name, String.Format(trans.Text, arguments));
                else
                    return new LocalizedString(name, trans.Text);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var culture = Settings.DefaultCulture;

            if (string.IsNullOrEmpty(culture))
                throw new InvalidOperationException("Missing culture in settingfile");

            var list = ModelService.GetTranslations();
            return list.Where(z=> z.Culture==culture).Select(p => new LocalizedString(p.Key, p.Text)).ToList();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
