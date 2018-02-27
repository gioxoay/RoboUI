using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace RoboUI
{
    internal sealed class CssTextWriter : TextWriter
    {
        private TextWriter _writer;

        private static Hashtable attrKeyLookupTable;

        private static CssTextWriter.AttributeInformation[] attrNameLookupArray;

        public override Encoding Encoding => this._writer.Encoding;

        public override string NewLine
        {
            get => this._writer.NewLine;
            set => this._writer.NewLine = value;
        }

        static CssTextWriter()
        {
            CssTextWriter.attrKeyLookupTable = new Hashtable(43);
            CssTextWriter.attrNameLookupArray = new CssTextWriter.AttributeInformation[43];
            CssTextWriter.RegisterAttribute("background-color", HtmlTextWriterStyle.BackgroundColor);
            CssTextWriter.RegisterAttribute("background-image", HtmlTextWriterStyle.BackgroundImage, true, true);
            CssTextWriter.RegisterAttribute("border-collapse", HtmlTextWriterStyle.BorderCollapse);
            CssTextWriter.RegisterAttribute("border-color", HtmlTextWriterStyle.BorderColor);
            CssTextWriter.RegisterAttribute("border-style", HtmlTextWriterStyle.BorderStyle);
            CssTextWriter.RegisterAttribute("border-width", HtmlTextWriterStyle.BorderWidth);
            CssTextWriter.RegisterAttribute("color", HtmlTextWriterStyle.Color);
            CssTextWriter.RegisterAttribute("cursor", HtmlTextWriterStyle.Cursor);
            CssTextWriter.RegisterAttribute("direction", HtmlTextWriterStyle.Direction);
            CssTextWriter.RegisterAttribute("display", HtmlTextWriterStyle.Display);
            CssTextWriter.RegisterAttribute("filter", HtmlTextWriterStyle.Filter);
            CssTextWriter.RegisterAttribute("font-family", HtmlTextWriterStyle.FontFamily, true);
            CssTextWriter.RegisterAttribute("font-size", HtmlTextWriterStyle.FontSize);
            CssTextWriter.RegisterAttribute("font-style", HtmlTextWriterStyle.FontStyle);
            CssTextWriter.RegisterAttribute("font-variant", HtmlTextWriterStyle.FontVariant);
            CssTextWriter.RegisterAttribute("font-weight", HtmlTextWriterStyle.FontWeight);
            CssTextWriter.RegisterAttribute("height", HtmlTextWriterStyle.Height);
            CssTextWriter.RegisterAttribute("left", HtmlTextWriterStyle.Left);
            CssTextWriter.RegisterAttribute("list-style-image", HtmlTextWriterStyle.ListStyleImage, true, true);
            CssTextWriter.RegisterAttribute("list-style-type", HtmlTextWriterStyle.ListStyleType);
            CssTextWriter.RegisterAttribute("margin", HtmlTextWriterStyle.Margin);
            CssTextWriter.RegisterAttribute("margin-bottom", HtmlTextWriterStyle.MarginBottom);
            CssTextWriter.RegisterAttribute("margin-left", HtmlTextWriterStyle.MarginLeft);
            CssTextWriter.RegisterAttribute("margin-right", HtmlTextWriterStyle.MarginRight);
            CssTextWriter.RegisterAttribute("margin-top", HtmlTextWriterStyle.MarginTop);
            CssTextWriter.RegisterAttribute("overflow-x", HtmlTextWriterStyle.OverflowX);
            CssTextWriter.RegisterAttribute("overflow-y", HtmlTextWriterStyle.OverflowY);
            CssTextWriter.RegisterAttribute("overflow", HtmlTextWriterStyle.Overflow);
            CssTextWriter.RegisterAttribute("padding", HtmlTextWriterStyle.Padding);
            CssTextWriter.RegisterAttribute("padding-bottom", HtmlTextWriterStyle.PaddingBottom);
            CssTextWriter.RegisterAttribute("padding-left", HtmlTextWriterStyle.PaddingLeft);
            CssTextWriter.RegisterAttribute("padding-right", HtmlTextWriterStyle.PaddingRight);
            CssTextWriter.RegisterAttribute("padding-top", HtmlTextWriterStyle.PaddingTop);
            CssTextWriter.RegisterAttribute("position", HtmlTextWriterStyle.Position);
            CssTextWriter.RegisterAttribute("text-align", HtmlTextWriterStyle.TextAlign);
            CssTextWriter.RegisterAttribute("text-decoration", HtmlTextWriterStyle.TextDecoration);
            CssTextWriter.RegisterAttribute("text-overflow", HtmlTextWriterStyle.TextOverflow);
            CssTextWriter.RegisterAttribute("top", HtmlTextWriterStyle.Top);
            CssTextWriter.RegisterAttribute("vertical-align", HtmlTextWriterStyle.VerticalAlign);
            CssTextWriter.RegisterAttribute("visibility", HtmlTextWriterStyle.Visibility);
            CssTextWriter.RegisterAttribute("width", HtmlTextWriterStyle.Width);
            CssTextWriter.RegisterAttribute("white-space", HtmlTextWriterStyle.WhiteSpace);
            CssTextWriter.RegisterAttribute("z-index", HtmlTextWriterStyle.ZIndex);
        }

        public CssTextWriter(TextWriter writer)
        {
            this._writer = writer;
        }

        public override void Flush()
        {
            this._writer.Flush();
        }

        public static HtmlTextWriterStyle GetStyleKey(string styleName)
        {
            if (!string.IsNullOrEmpty(styleName))
            {
                object item = CssTextWriter.attrKeyLookupTable[styleName.ToLowerInvariant()];
                if (item != null)
                {
                    return (HtmlTextWriterStyle)item;
                }
            }
            return HtmlTextWriterStyle.BackgroundImage | HtmlTextWriterStyle.BorderCollapse | HtmlTextWriterStyle.BorderColor | HtmlTextWriterStyle.BorderStyle | HtmlTextWriterStyle.BorderWidth | HtmlTextWriterStyle.Color | HtmlTextWriterStyle.FontFamily | HtmlTextWriterStyle.FontSize | HtmlTextWriterStyle.FontStyle | HtmlTextWriterStyle.FontWeight | HtmlTextWriterStyle.Height | HtmlTextWriterStyle.TextDecoration | HtmlTextWriterStyle.Width | HtmlTextWriterStyle.ListStyleImage | HtmlTextWriterStyle.ListStyleType | HtmlTextWriterStyle.Cursor | HtmlTextWriterStyle.Direction | HtmlTextWriterStyle.Display | HtmlTextWriterStyle.Filter | HtmlTextWriterStyle.FontVariant | HtmlTextWriterStyle.Left | HtmlTextWriterStyle.Margin | HtmlTextWriterStyle.MarginBottom | HtmlTextWriterStyle.MarginLeft | HtmlTextWriterStyle.MarginRight | HtmlTextWriterStyle.MarginTop | HtmlTextWriterStyle.Overflow | HtmlTextWriterStyle.OverflowX | HtmlTextWriterStyle.OverflowY | HtmlTextWriterStyle.Padding | HtmlTextWriterStyle.PaddingBottom | HtmlTextWriterStyle.PaddingLeft | HtmlTextWriterStyle.PaddingRight | HtmlTextWriterStyle.PaddingTop | HtmlTextWriterStyle.Position | HtmlTextWriterStyle.TextAlign | HtmlTextWriterStyle.VerticalAlign | HtmlTextWriterStyle.TextOverflow | HtmlTextWriterStyle.Top | HtmlTextWriterStyle.Visibility | HtmlTextWriterStyle.WhiteSpace | HtmlTextWriterStyle.ZIndex;
        }

        public static string GetStyleName(HtmlTextWriterStyle styleKey)
        {
            if (styleKey < HtmlTextWriterStyle.BackgroundColor || (int)styleKey >= (int)CssTextWriter.attrNameLookupArray.Length)
            {
                return string.Empty;
            }
            return CssTextWriter.attrNameLookupArray[(int)styleKey].name;
        }

        public static bool IsStyleEncoded(HtmlTextWriterStyle styleKey)
        {
            if (styleKey < HtmlTextWriterStyle.BackgroundColor || (int)styleKey >= (int)CssTextWriter.attrNameLookupArray.Length)
            {
                return true;
            }
            return CssTextWriter.attrNameLookupArray[(int)styleKey].encode;
        }

        internal static void RegisterAttribute(string name, HtmlTextWriterStyle key)
        {
            CssTextWriter.RegisterAttribute(name, key, false, false);
        }

        internal static void RegisterAttribute(string name, HtmlTextWriterStyle key, bool encode)
        {
            CssTextWriter.RegisterAttribute(name, key, encode, false);
        }

        internal static void RegisterAttribute(string name, HtmlTextWriterStyle key, bool encode, bool isUrl)
        {
            string lower = name.ToLowerInvariant();
            CssTextWriter.attrKeyLookupTable.Add(lower, key);
            if ((int)key < (int)CssTextWriter.attrNameLookupArray.Length)
            {
                CssTextWriter.attrNameLookupArray[(int)key] = new CssTextWriter.AttributeInformation(name, encode, isUrl);
            }
        }

        public override void Write(string s)
        {
            this._writer.Write(s);
        }

        public override void Write(bool value)
        {
            this._writer.Write(value);
        }

        public override void Write(char value)
        {
            this._writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            this._writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            this._writer.Write(buffer, index, count);
        }

        public override void Write(double value)
        {
            this._writer.Write(value);
        }

        public override void Write(float value)
        {
            this._writer.Write(value);
        }

        public override void Write(int value)
        {
            this._writer.Write(value);
        }

        public override void Write(long value)
        {
            this._writer.Write(value);
        }

        public override void Write(object value)
        {
            this._writer.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            this._writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            this._writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, params object[] arg)
        {
            this._writer.Write(format, arg);
        }

        public void WriteAttribute(string name, string value)
        {
            CssTextWriter.WriteAttribute(this._writer, CssTextWriter.GetStyleKey(name), name, value);
        }

        public void WriteAttribute(HtmlTextWriterStyle key, string value)
        {
            CssTextWriter.WriteAttribute(this._writer, key, CssTextWriter.GetStyleName(key), value);
        }

        private static void WriteAttribute(TextWriter writer, HtmlTextWriterStyle key, string name, string value)
        {
            writer.Write(name);
            writer.Write(':');
            bool flag = false;
            if (key != (HtmlTextWriterStyle.BackgroundImage | HtmlTextWriterStyle.BorderCollapse | HtmlTextWriterStyle.BorderColor | HtmlTextWriterStyle.BorderStyle | HtmlTextWriterStyle.BorderWidth | HtmlTextWriterStyle.Color | HtmlTextWriterStyle.FontFamily | HtmlTextWriterStyle.FontSize | HtmlTextWriterStyle.FontStyle | HtmlTextWriterStyle.FontWeight | HtmlTextWriterStyle.Height | HtmlTextWriterStyle.TextDecoration | HtmlTextWriterStyle.Width | HtmlTextWriterStyle.ListStyleImage | HtmlTextWriterStyle.ListStyleType | HtmlTextWriterStyle.Cursor | HtmlTextWriterStyle.Direction | HtmlTextWriterStyle.Display | HtmlTextWriterStyle.Filter | HtmlTextWriterStyle.FontVariant | HtmlTextWriterStyle.Left | HtmlTextWriterStyle.Margin | HtmlTextWriterStyle.MarginBottom | HtmlTextWriterStyle.MarginLeft | HtmlTextWriterStyle.MarginRight | HtmlTextWriterStyle.MarginTop | HtmlTextWriterStyle.Overflow | HtmlTextWriterStyle.OverflowX | HtmlTextWriterStyle.OverflowY | HtmlTextWriterStyle.Padding | HtmlTextWriterStyle.PaddingBottom | HtmlTextWriterStyle.PaddingLeft | HtmlTextWriterStyle.PaddingRight | HtmlTextWriterStyle.PaddingTop | HtmlTextWriterStyle.Position | HtmlTextWriterStyle.TextAlign | HtmlTextWriterStyle.VerticalAlign | HtmlTextWriterStyle.TextOverflow | HtmlTextWriterStyle.Top | HtmlTextWriterStyle.Visibility | HtmlTextWriterStyle.WhiteSpace | HtmlTextWriterStyle.ZIndex))
            {
                flag = CssTextWriter.attrNameLookupArray[(int)key].isUrl;
            }
            if (flag)
            {
                CssTextWriter.WriteUrlAttribute(writer, value);
            }
            else
            {
                writer.Write(value);
            }
            writer.Write(';');
        }

        internal static void WriteAttributes(TextWriter writer, RenderStyle[] styles, int count)
        {
            for (int i = 0; i < count; i++)
            {
                RenderStyle renderStyle = styles[i];
                CssTextWriter.WriteAttribute(writer, renderStyle.key, renderStyle.name, renderStyle.@value);
            }
        }

        public void WriteBeginCssRule(string selector)
        {
            this._writer.Write(selector);
            this._writer.Write(" { ");
        }

        public void WriteEndCssRule()
        {
            this._writer.WriteLine(" }");
        }

        public override void WriteLine(string s)
        {
            this._writer.WriteLine(s);
        }

        public override void WriteLine()
        {
            this._writer.WriteLine();
        }

        public override void WriteLine(bool value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(char value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(char[] buffer)
        {
            this._writer.WriteLine(buffer);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            this._writer.WriteLine(buffer, index, count);
        }

        public override void WriteLine(double value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(float value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(int value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(long value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(object value)
        {
            this._writer.WriteLine(value);
        }

        public override void WriteLine(string format, object arg0)
        {
            this._writer.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            this._writer.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            this._writer.WriteLine(format, arg);
        }

        public override void WriteLine(uint value)
        {
            this._writer.WriteLine(value);
        }

        internal static void WriteUrlAttribute(TextWriter writer, string url)
        {
            string str = url;
            char[] chrArray = new char[] { '\'', '\"' };
            char? nullable = null;
            if (url.StartsWith("url("))
            {
                int num = 4;
                int length = url.Length - 4;
                if (url.EndsWith(")"))
                {
                    length--;
                }
                str = url.Substring(num, length).Trim();
            }
            char[] chrArray1 = chrArray;
            int num1 = 0;
            while (num1 < (int)chrArray1.Length)
            {
                char chr = chrArray1[num1];
                if (!str.StartsWith(chr.ToString()) || !str.EndsWith(chr.ToString()))
                {
                    num1++;
                }
                else
                {
                    str = str.Trim(new char[] { chr });
                    nullable = new char?(chr);
                    break;
                }
            }
            writer.Write("url(");
            if (nullable.HasValue)
            {
                writer.Write(nullable);
            }
            writer.Write(WebUtility.UrlEncode(str));
            if (nullable.HasValue)
            {
                writer.Write(nullable);
            }
            writer.Write(")");
        }

        private struct AttributeInformation
        {
            public string name;

            public bool isUrl;

            public bool encode;

            public AttributeInformation(string name, bool encode, bool isUrl)
            {
                this.name = name;
                this.encode = encode;
                this.isUrl = isUrl;
            }
        }
    }
}
