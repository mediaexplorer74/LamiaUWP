using System.Globalization;
using NGettext;

namespace LamiaSimulation
{

    /*
     * Stolen from the NGettext examples folder (MIT licensed)
     * https://github.com/VitaliiTsilnyk/NGettext/tree/master/doc/examples
     * Usage:
     *		T._("Hello, World!"); // GetString
     *		T._n("You have {0} apple.", "You have {0} apples.", count, count); // GetPluralString
     *		T._p("Context", "Hello, World!"); // GetParticularString
     *		T._pn("Context", "You have {0} apple.", "You have {0} apples.", count, count); // GetParticularPluralString
     */
    internal class T
    {
        private static readonly ICatalog _Catalog = new Catalog(
            "LamiaSimulation", "./locale", new CultureInfo("en-GB")
        );

        public static string _(string text)
        {
            return _Catalog.GetString(text);
        }

        public static string[] _(string[] texts)
        {
            var i = 0;
            foreach(var text in texts)
            {
                texts[i] = _Catalog.GetString(text);
                i++;
            }
            return texts;
        }

        public static string _(string text, params object[] args)
        {
            return _Catalog.GetString(text, args);
        }

        public static string _n(string text, string pluralText, long n)
        {
            return _Catalog.GetPluralString(text, pluralText, n);
        }

        public static string _n(string text, string pluralText, long n, params object[] args)
        {
            return _Catalog.GetPluralString(text, pluralText, n, args);
        }

        public static string _p(string context, string text)
        {
            return _Catalog.GetParticularString(context, text);
        }

        public static string _p(string context, string text, params object[] args)
        {
            return _Catalog.GetParticularString(context, text, args);
        }

        public static string _pn(string context, string text, string pluralText, long n)
        {
            return _Catalog.GetParticularPluralString(context, text, pluralText, n);
        }

        public static string _pn(string context, string text, string pluralText, long n, params object[] args)
        {
            return _Catalog.GetParticularPluralString(context, text, pluralText, n, args);
        }
    }
}
