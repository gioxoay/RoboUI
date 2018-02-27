using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace RoboUI
{
    public class HtmlTextWriter : TextWriter
    {
        private HtmlTextWriter.Layout _currentLayout = new HtmlTextWriter.Layout(HorizontalAlign.NotSet, true);

        private HtmlTextWriter.Layout _currentWrittenLayout;

        private TextWriter writer;

        private int indentLevel;

        private bool tabsPending;

        private string tabString;

        public const char TagLeftChar = '<';

        public const char TagRightChar = '>';

        public const string SelfClosingChars = " /";

        public const string SelfClosingTagEnd = " />";

        public const string EndTagLeftChars = "</";

        public const char DoubleQuoteChar = '\"';

        public const char SingleQuoteChar = '\'';

        public const char SpaceChar = ' ';

        public const char EqualsChar = '=';

        public const char SlashChar = '/';

        public const string EqualsDoubleQuoteString = "=\"";

        public const char SemicolonChar = ';';

        public const char StyleEqualsChar = ':';

        public const string DefaultTabString = "\t";

        internal const string DesignerRegionAttributeName = "_designerRegion";

        private static Hashtable _tagKeyLookupTable;

        private static Hashtable _attrKeyLookupTable;

        private static HtmlTextWriter.TagInformation[] _tagNameLookupArray;

        private static HtmlTextWriter.AttributeInformation[] _attrNameLookupArray;

        private HtmlTextWriter.RenderAttribute[] _attrList;

        private int _attrCount;

        private int _endTagCount;

        private HtmlTextWriter.TagStackEntry[] _endTags;

        private int _inlineCount;

        private bool _isDescendant;

        private RenderStyle[] _styleList;

        private int _styleCount;

        private int _tagIndex;

        private HtmlTextWriterTag _tagKey;

        private string _tagName;

        public override Encoding Encoding => this.writer.Encoding;

        public int Indent
        {
            get => this.indentLevel;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.indentLevel = value;
            }
        }

        public TextWriter InnerWriter
        {
            get => this.writer;
            set => this.writer = value;
        }

        public override string NewLine
        {
            get => this.writer.NewLine;
            set => this.writer.NewLine = value;
        }

        internal virtual bool RenderDivAroundHiddenInputs => true;

        protected HtmlTextWriterTag TagKey
        {
            get => this._tagKey;
            set
            {
                this._tagIndex = (int)value;
                if (this._tagIndex < 0 || this._tagIndex >= (int)HtmlTextWriter._tagNameLookupArray.Length)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._tagKey = value;
                if (value != HtmlTextWriterTag.Unknown)
                {
                    this._tagName = HtmlTextWriter._tagNameLookupArray[this._tagIndex].name;
                }
            }
        }

        protected string TagName
        {
            get => this._tagName;
            set
            {
                this._tagName = value;
                this._tagKey = this.GetTagKey(this._tagName);
                this._tagIndex = (int)this._tagKey;
            }
        }

        static HtmlTextWriter()
        {
            HtmlTextWriter._tagKeyLookupTable = new Hashtable(97);
            HtmlTextWriter._tagNameLookupArray = new HtmlTextWriter.TagInformation[97];
            HtmlTextWriter.RegisterTag(string.Empty, HtmlTextWriterTag.Unknown, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("a", HtmlTextWriterTag.A, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("acronym", HtmlTextWriterTag.Acronym, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("address", HtmlTextWriterTag.Address, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("area", HtmlTextWriterTag.Area, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("b", HtmlTextWriterTag.B, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("base", HtmlTextWriterTag.Base, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("basefont", HtmlTextWriterTag.Basefont, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("bdo", HtmlTextWriterTag.Bdo, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("bgsound", HtmlTextWriterTag.Bgsound, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("big", HtmlTextWriterTag.Big, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("blockquote", HtmlTextWriterTag.Blockquote, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("body", HtmlTextWriterTag.Body, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("br", HtmlTextWriterTag.Br, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("button", HtmlTextWriterTag.Button, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("caption", HtmlTextWriterTag.Caption, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("center", HtmlTextWriterTag.Center, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("cite", HtmlTextWriterTag.Cite, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("code", HtmlTextWriterTag.Code, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("col", HtmlTextWriterTag.Col, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("colgroup", HtmlTextWriterTag.Colgroup, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("del", HtmlTextWriterTag.Del, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("dd", HtmlTextWriterTag.Dd, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("dfn", HtmlTextWriterTag.Dfn, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("dir", HtmlTextWriterTag.Dir, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("div", HtmlTextWriterTag.Div, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("dl", HtmlTextWriterTag.Dl, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("dt", HtmlTextWriterTag.Dt, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("em", HtmlTextWriterTag.Em, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("embed", HtmlTextWriterTag.Embed, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("fieldset", HtmlTextWriterTag.Fieldset, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("font", HtmlTextWriterTag.Font, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("form", HtmlTextWriterTag.Form, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("frame", HtmlTextWriterTag.Frame, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("frameset", HtmlTextWriterTag.Frameset, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("h1", HtmlTextWriterTag.H1, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("h2", HtmlTextWriterTag.H2, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("h3", HtmlTextWriterTag.H3, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("h4", HtmlTextWriterTag.H4, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("h5", HtmlTextWriterTag.H5, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("h6", HtmlTextWriterTag.H6, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("head", HtmlTextWriterTag.Head, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("hr", HtmlTextWriterTag.Hr, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("html", HtmlTextWriterTag.Html, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("i", HtmlTextWriterTag.I, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("iframe", HtmlTextWriterTag.Iframe, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("img", HtmlTextWriterTag.Img, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("input", HtmlTextWriterTag.Input, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("ins", HtmlTextWriterTag.Ins, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("isindex", HtmlTextWriterTag.Isindex, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("kbd", HtmlTextWriterTag.Kbd, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("label", HtmlTextWriterTag.Label, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("legend", HtmlTextWriterTag.Legend, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("li", HtmlTextWriterTag.Li, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("link", HtmlTextWriterTag.Link, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("map", HtmlTextWriterTag.Map, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("marquee", HtmlTextWriterTag.Marquee, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("menu", HtmlTextWriterTag.Menu, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("meta", HtmlTextWriterTag.Meta, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("nobr", HtmlTextWriterTag.Nobr, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("noframes", HtmlTextWriterTag.Noframes, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("noscript", HtmlTextWriterTag.Noscript, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("object", HtmlTextWriterTag.Object, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("ol", HtmlTextWriterTag.Ol, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("option", HtmlTextWriterTag.Option, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("p", HtmlTextWriterTag.P, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("param", HtmlTextWriterTag.Param, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("pre", HtmlTextWriterTag.Pre, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("ruby", HtmlTextWriterTag.Ruby, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("rt", HtmlTextWriterTag.Rt, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("q", HtmlTextWriterTag.Q, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("s", HtmlTextWriterTag.S, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("samp", HtmlTextWriterTag.Samp, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("script", HtmlTextWriterTag.Script, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("select", HtmlTextWriterTag.Select, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("small", HtmlTextWriterTag.Small, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("span", HtmlTextWriterTag.Span, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("strike", HtmlTextWriterTag.Strike, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("strong", HtmlTextWriterTag.Strong, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("style", HtmlTextWriterTag.Style, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("sub", HtmlTextWriterTag.Sub, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("sup", HtmlTextWriterTag.Sup, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("table", HtmlTextWriterTag.Table, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("tbody", HtmlTextWriterTag.Tbody, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("td", HtmlTextWriterTag.Td, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("textarea", HtmlTextWriterTag.Textarea, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("tfoot", HtmlTextWriterTag.Tfoot, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("th", HtmlTextWriterTag.Th, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("thead", HtmlTextWriterTag.Thead, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("title", HtmlTextWriterTag.Title, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("tr", HtmlTextWriterTag.Tr, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("tt", HtmlTextWriterTag.Tt, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("u", HtmlTextWriterTag.U, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("ul", HtmlTextWriterTag.Ul, HtmlTextWriter.TagType.Other);
            HtmlTextWriter.RegisterTag("var", HtmlTextWriterTag.Var, HtmlTextWriter.TagType.Inline);
            HtmlTextWriter.RegisterTag("wbr", HtmlTextWriterTag.Wbr, HtmlTextWriter.TagType.NonClosing);
            HtmlTextWriter.RegisterTag("xml", HtmlTextWriterTag.Xml, HtmlTextWriter.TagType.Other);
            HtmlTextWriter._attrKeyLookupTable = new Hashtable(54);
            HtmlTextWriter._attrNameLookupArray = new HtmlTextWriter.AttributeInformation[54];
            HtmlTextWriter.RegisterAttribute("abbr", HtmlTextWriterAttribute.Abbr, true);
            HtmlTextWriter.RegisterAttribute("accesskey", HtmlTextWriterAttribute.Accesskey, true);
            HtmlTextWriter.RegisterAttribute("align", HtmlTextWriterAttribute.Align, false);
            HtmlTextWriter.RegisterAttribute("alt", HtmlTextWriterAttribute.Alt, true);
            HtmlTextWriter.RegisterAttribute("autocomplete", HtmlTextWriterAttribute.AutoComplete, false);
            HtmlTextWriter.RegisterAttribute("axis", HtmlTextWriterAttribute.Axis, true);
            HtmlTextWriter.RegisterAttribute("background", HtmlTextWriterAttribute.Background, true, true);
            HtmlTextWriter.RegisterAttribute("bgcolor", HtmlTextWriterAttribute.Bgcolor, false);
            HtmlTextWriter.RegisterAttribute("border", HtmlTextWriterAttribute.Border, false);
            HtmlTextWriter.RegisterAttribute("bordercolor", HtmlTextWriterAttribute.Bordercolor, false);
            HtmlTextWriter.RegisterAttribute("cellpadding", HtmlTextWriterAttribute.Cellpadding, false);
            HtmlTextWriter.RegisterAttribute("cellspacing", HtmlTextWriterAttribute.Cellspacing, false);
            HtmlTextWriter.RegisterAttribute("checked", HtmlTextWriterAttribute.Checked, false);
            HtmlTextWriter.RegisterAttribute("class", HtmlTextWriterAttribute.Class, true);
            HtmlTextWriter.RegisterAttribute("cols", HtmlTextWriterAttribute.Cols, false);
            HtmlTextWriter.RegisterAttribute("colspan", HtmlTextWriterAttribute.Colspan, false);
            HtmlTextWriter.RegisterAttribute("content", HtmlTextWriterAttribute.Content, true);
            HtmlTextWriter.RegisterAttribute("coords", HtmlTextWriterAttribute.Coords, false);
            HtmlTextWriter.RegisterAttribute("dir", HtmlTextWriterAttribute.Dir, false);
            HtmlTextWriter.RegisterAttribute("disabled", HtmlTextWriterAttribute.Disabled, false);
            HtmlTextWriter.RegisterAttribute("for", HtmlTextWriterAttribute.For, false);
            HtmlTextWriter.RegisterAttribute("headers", HtmlTextWriterAttribute.Headers, true);
            HtmlTextWriter.RegisterAttribute("height", HtmlTextWriterAttribute.Height, false);
            HtmlTextWriter.RegisterAttribute("href", HtmlTextWriterAttribute.Href, true, true);
            HtmlTextWriter.RegisterAttribute("id", HtmlTextWriterAttribute.Id, false);
            HtmlTextWriter.RegisterAttribute("longdesc", HtmlTextWriterAttribute.Longdesc, true, true);
            HtmlTextWriter.RegisterAttribute("maxlength", HtmlTextWriterAttribute.Maxlength, false);
            HtmlTextWriter.RegisterAttribute("multiple", HtmlTextWriterAttribute.Multiple, false);
            HtmlTextWriter.RegisterAttribute("name", HtmlTextWriterAttribute.Name, false);
            HtmlTextWriter.RegisterAttribute("nowrap", HtmlTextWriterAttribute.Nowrap, false);
            HtmlTextWriter.RegisterAttribute("onclick", HtmlTextWriterAttribute.Onclick, true);
            HtmlTextWriter.RegisterAttribute("onchange", HtmlTextWriterAttribute.Onchange, true);
            HtmlTextWriter.RegisterAttribute("readonly", HtmlTextWriterAttribute.ReadOnly, false);
            HtmlTextWriter.RegisterAttribute("rel", HtmlTextWriterAttribute.Rel, false);
            HtmlTextWriter.RegisterAttribute("rows", HtmlTextWriterAttribute.Rows, false);
            HtmlTextWriter.RegisterAttribute("rowspan", HtmlTextWriterAttribute.Rowspan, false);
            HtmlTextWriter.RegisterAttribute("rules", HtmlTextWriterAttribute.Rules, false);
            HtmlTextWriter.RegisterAttribute("scope", HtmlTextWriterAttribute.Scope, false);
            HtmlTextWriter.RegisterAttribute("selected", HtmlTextWriterAttribute.Selected, false);
            HtmlTextWriter.RegisterAttribute("shape", HtmlTextWriterAttribute.Shape, false);
            HtmlTextWriter.RegisterAttribute("size", HtmlTextWriterAttribute.Size, false);
            HtmlTextWriter.RegisterAttribute("src", HtmlTextWriterAttribute.Src, true, true);
            HtmlTextWriter.RegisterAttribute("style", HtmlTextWriterAttribute.Style, false);
            HtmlTextWriter.RegisterAttribute("tabindex", HtmlTextWriterAttribute.Tabindex, false);
            HtmlTextWriter.RegisterAttribute("target", HtmlTextWriterAttribute.Target, false);
            HtmlTextWriter.RegisterAttribute("title", HtmlTextWriterAttribute.Title, true);
            HtmlTextWriter.RegisterAttribute("type", HtmlTextWriterAttribute.Type, false);
            HtmlTextWriter.RegisterAttribute("usemap", HtmlTextWriterAttribute.Usemap, false);
            HtmlTextWriter.RegisterAttribute("valign", HtmlTextWriterAttribute.Valign, false);
            HtmlTextWriter.RegisterAttribute("value", HtmlTextWriterAttribute.Value, true);
            HtmlTextWriter.RegisterAttribute("vcard_name", HtmlTextWriterAttribute.VCardName, false);
            HtmlTextWriter.RegisterAttribute("width", HtmlTextWriterAttribute.Width, false);
            HtmlTextWriter.RegisterAttribute("wrap", HtmlTextWriterAttribute.Wrap, false);
            HtmlTextWriter.RegisterAttribute("_designerRegion", HtmlTextWriterAttribute.DesignerRegion, false);
        }

        public HtmlTextWriter(TextWriter writer) : this(writer, "\t")
        {
        }

        public HtmlTextWriter(TextWriter writer, string tabString) : base(CultureInfo.InvariantCulture)
        {
            this.writer = writer;
            this.tabString = tabString;
            this.indentLevel = 0;
            this.tabsPending = false;
            this._isDescendant = base.GetType() != typeof(HtmlTextWriter);
            this._attrCount = 0;
            this._styleCount = 0;
            this._endTagCount = 0;
            this._inlineCount = 0;
        }

        public virtual void AddAttribute(string name, string value)
        {
            HtmlTextWriterAttribute attributeKey = this.GetAttributeKey(name);
            value = this.EncodeAttributeValue(attributeKey, value);
            this.AddAttribute(name, value, attributeKey);
        }

        public virtual void AddAttribute(string name, string value, bool fEndode)
        {
            value = this.EncodeAttributeValue(value, fEndode);
            this.AddAttribute(name, value, this.GetAttributeKey(name));
        }

        public virtual void AddAttribute(HtmlTextWriterAttribute key, string value)
        {
            int num = (int)key;
            if (num >= 0 && num < (int)HtmlTextWriter._attrNameLookupArray.Length)
            {
                HtmlTextWriter.AttributeInformation attributeInformation = HtmlTextWriter._attrNameLookupArray[num];
                this.AddAttribute(attributeInformation.name, value, key, attributeInformation.encode, attributeInformation.isUrl);
            }
        }

        public virtual void AddAttribute(HtmlTextWriterAttribute key, string value, bool fEncode)
        {
            int num = (int)key;
            if (num >= 0 && num < (int)HtmlTextWriter._attrNameLookupArray.Length)
            {
                HtmlTextWriter.AttributeInformation attributeInformation = HtmlTextWriter._attrNameLookupArray[num];
                this.AddAttribute(attributeInformation.name, value, key, fEncode, attributeInformation.isUrl);
            }
        }

        protected virtual void AddAttribute(string name, string value, HtmlTextWriterAttribute key)
        {
            this.AddAttribute(name, value, key, false, false);
        }

        private void AddAttribute(string name, string value, HtmlTextWriterAttribute key, bool encode, bool isUrl)
        {
            HtmlTextWriter.RenderAttribute renderAttribute = new HtmlTextWriter.RenderAttribute();
            if (this._attrList == null)
            {
                this._attrList = new HtmlTextWriter.RenderAttribute[20];
            }
            else if (this._attrCount >= (int)this._attrList.Length)
            {
                HtmlTextWriter.RenderAttribute[] renderAttributeArray = new HtmlTextWriter.RenderAttribute[(int)this._attrList.Length * 2];
                Array.Copy(this._attrList, renderAttributeArray, (int)this._attrList.Length);
                this._attrList = renderAttributeArray;
            }
            renderAttribute.name = name;
            renderAttribute.@value = value;
            renderAttribute.key = key;
            renderAttribute.encode = encode;
            renderAttribute.isUrl = isUrl;
            this._attrList[this._attrCount] = renderAttribute;
            this._attrCount++;
        }

        public virtual void AddStyleAttribute(string name, string value)
        {
            this.AddStyleAttribute(name, value, CssTextWriter.GetStyleKey(name));
        }

        public virtual void AddStyleAttribute(HtmlTextWriterStyle key, string value)
        {
            this.AddStyleAttribute(CssTextWriter.GetStyleName(key), value, key);
        }

        protected virtual void AddStyleAttribute(string name, string value, HtmlTextWriterStyle key)
        {
            RenderStyle renderStyle = new RenderStyle();
            if (this._styleList == null)
            {
                this._styleList = new RenderStyle[20];
            }
            else if (this._styleCount > (int)this._styleList.Length)
            {
                RenderStyle[] renderStyleArray = new RenderStyle[(int)this._styleList.Length * 2];
                Array.Copy(this._styleList, renderStyleArray, (int)this._styleList.Length);
                this._styleList = renderStyleArray;
            }
            renderStyle.name = name;
            renderStyle.key = key;
            string str = value;
            if (CssTextWriter.IsStyleEncoded(key))
            {
                str = WebUtility.HtmlEncode(value);
            }
            renderStyle.@value = str;
            this._styleList[this._styleCount] = renderStyle;
            this._styleCount++;
        }

        public virtual void BeginRender()
        {
        }
        
        protected string EncodeAttributeValue(string value, bool fEncode)
        {
            if (value == null)
            {
                return null;
            }
            if (!fEncode)
            {
                return value;
            }
            return WebUtility.HtmlEncode(value);
        }

        protected virtual string EncodeAttributeValue(HtmlTextWriterAttribute attrKey, string value)
        {
            bool flag = true;
            if (HtmlTextWriterAttribute.Accesskey <= attrKey && (int)attrKey < (int)HtmlTextWriter._attrNameLookupArray.Length)
            {
                flag = HtmlTextWriter._attrNameLookupArray[(int)attrKey].encode;
            }
            return this.EncodeAttributeValue(value, flag);
        }

        protected string EncodeUrl(string url)
        {
            if (UrlPath.IsUncSharePath(url))
            {
                return url;
            }
            return WebUtility.UrlEncode(url);
        }

        public virtual void EndRender()
        {
        }

        protected virtual void FilterAttributes()
        {
            int num = 0;
            for (int i = 0; i < this._styleCount; i++)
            {
                RenderStyle renderStyle = this._styleList[i];
                if (this.OnStyleAttributeRender(renderStyle.name, renderStyle.@value, renderStyle.key))
                {
                    this._styleList[num] = renderStyle;
                    num++;
                }
            }
            this._styleCount = num;
            int num1 = 0;
            for (int j = 0; j < this._attrCount; j++)
            {
                HtmlTextWriter.RenderAttribute renderAttribute = this._attrList[j];
                if (this.OnAttributeRender(renderAttribute.name, renderAttribute.@value, renderAttribute.key))
                {
                    this._attrList[num1] = renderAttribute;
                    num1++;
                }
            }
            this._attrCount = num1;
        }

        public override void Flush()
        {
            this.writer.Flush();
        }

        protected HtmlTextWriterAttribute GetAttributeKey(string attrName)
        {
            if (!string.IsNullOrEmpty(attrName))
            {
                object item = HtmlTextWriter._attrKeyLookupTable[attrName.ToLowerInvariant()];
                if (item != null)
                {
                    return (HtmlTextWriterAttribute)item;
                }
            }
            return HtmlTextWriterAttribute.Align | HtmlTextWriterAttribute.Alt | HtmlTextWriterAttribute.Background | HtmlTextWriterAttribute.Bgcolor | HtmlTextWriterAttribute.Border | HtmlTextWriterAttribute.Bordercolor | HtmlTextWriterAttribute.Cellpadding | HtmlTextWriterAttribute.Cellspacing | HtmlTextWriterAttribute.Checked | HtmlTextWriterAttribute.Class | HtmlTextWriterAttribute.Cols | HtmlTextWriterAttribute.Colspan | HtmlTextWriterAttribute.Disabled | HtmlTextWriterAttribute.For | HtmlTextWriterAttribute.Height | HtmlTextWriterAttribute.Href | HtmlTextWriterAttribute.Id | HtmlTextWriterAttribute.Maxlength | HtmlTextWriterAttribute.Multiple | HtmlTextWriterAttribute.Name | HtmlTextWriterAttribute.Nowrap | HtmlTextWriterAttribute.Onchange | HtmlTextWriterAttribute.Onclick | HtmlTextWriterAttribute.ReadOnly | HtmlTextWriterAttribute.Rows | HtmlTextWriterAttribute.Rowspan | HtmlTextWriterAttribute.Rules | HtmlTextWriterAttribute.Selected | HtmlTextWriterAttribute.Size | HtmlTextWriterAttribute.Src | HtmlTextWriterAttribute.Style | HtmlTextWriterAttribute.Tabindex | HtmlTextWriterAttribute.Target | HtmlTextWriterAttribute.Title | HtmlTextWriterAttribute.Type | HtmlTextWriterAttribute.Valign | HtmlTextWriterAttribute.Value | HtmlTextWriterAttribute.Width | HtmlTextWriterAttribute.Wrap | HtmlTextWriterAttribute.Abbr | HtmlTextWriterAttribute.AutoComplete | HtmlTextWriterAttribute.Axis | HtmlTextWriterAttribute.Content | HtmlTextWriterAttribute.Coords | HtmlTextWriterAttribute.DesignerRegion | HtmlTextWriterAttribute.Dir | HtmlTextWriterAttribute.Headers | HtmlTextWriterAttribute.Longdesc | HtmlTextWriterAttribute.Rel | HtmlTextWriterAttribute.Scope | HtmlTextWriterAttribute.Shape | HtmlTextWriterAttribute.Usemap | HtmlTextWriterAttribute.VCardName;
        }

        protected string GetAttributeName(HtmlTextWriterAttribute attrKey)
        {
            if (attrKey < HtmlTextWriterAttribute.Accesskey || (int)attrKey >= (int)HtmlTextWriter._attrNameLookupArray.Length)
            {
                return string.Empty;
            }
            return HtmlTextWriter._attrNameLookupArray[(int)attrKey].name;
        }

        protected HtmlTextWriterStyle GetStyleKey(string styleName)
        {
            return CssTextWriter.GetStyleKey(styleName);
        }

        protected string GetStyleName(HtmlTextWriterStyle styleKey)
        {
            return CssTextWriter.GetStyleName(styleKey);
        }

        protected virtual HtmlTextWriterTag GetTagKey(string tagName)
        {
            if (!string.IsNullOrEmpty(tagName))
            {
                object item = HtmlTextWriter._tagKeyLookupTable[tagName.ToLowerInvariant()];
                if (item != null)
                {
                    return (HtmlTextWriterTag)item;
                }
            }
            return HtmlTextWriterTag.Unknown;
        }

        protected virtual string GetTagName(HtmlTextWriterTag tagKey)
        {
            int num = (int)tagKey;
            if (num < 0 || num >= (int)HtmlTextWriter._tagNameLookupArray.Length)
            {
                return string.Empty;
            }
            return HtmlTextWriter._tagNameLookupArray[num].name;
        }

        protected bool IsAttributeDefined(HtmlTextWriterAttribute key)
        {
            for (int i = 0; i < this._attrCount; i++)
            {
                if (this._attrList[i].key == key)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool IsAttributeDefined(HtmlTextWriterAttribute key, out string value)
        {
            value = null;
            for (int i = 0; i < this._attrCount; i++)
            {
                if (this._attrList[i].key == key)
                {
                    value = this._attrList[i].@value;
                    return true;
                }
            }
            return false;
        }

        protected bool IsStyleAttributeDefined(HtmlTextWriterStyle key)
        {
            for (int i = 0; i < this._styleCount; i++)
            {
                if (this._styleList[i].key == key)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool IsStyleAttributeDefined(HtmlTextWriterStyle key, out string value)
        {
            value = null;
            for (int i = 0; i < this._styleCount; i++)
            {
                if (this._styleList[i].key == key)
                {
                    value = this._styleList[i].@value;
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsValidFormAttribute(string attribute)
        {
            return true;
        }

        protected virtual bool OnAttributeRender(string name, string value, HtmlTextWriterAttribute key)
        {
            return true;
        }

        protected virtual bool OnStyleAttributeRender(string name, string value, HtmlTextWriterStyle key)
        {
            return true;
        }

        protected virtual bool OnTagRender(string name, HtmlTextWriterTag key)
        {
            return true;
        }

        internal virtual void OpenDiv()
        {
            this.OpenDiv(this._currentLayout, (this._currentLayout == null ? false : this._currentLayout.Align != HorizontalAlign.NotSet), (this._currentLayout == null ? false : !this._currentLayout.Wrap));
        }

        private void OpenDiv(HtmlTextWriter.Layout layout, bool writeHorizontalAlign, bool writeWrapping)
        {
            string str;
            this.WriteBeginTag("div");
            if (writeHorizontalAlign)
            {
                HorizontalAlign align = layout.Align;
                if (align == HorizontalAlign.Center)
                {
                    str = "text-align:center";
                }
                else
                {
                    str = (align != HorizontalAlign.Right ? "text-align:left" : "text-align:right");
                }
                this.WriteAttribute("style", str);
            }
            if (writeWrapping)
            {
                this.WriteAttribute("mode", (layout.Wrap ? "wrap" : "nowrap"));
            }
            this.Write('>');
            this._currentWrittenLayout = layout;
        }

        protected virtual void OutputTabs()
        {
            if (this.tabsPending)
            {
                for (int i = 0; i < this.indentLevel; i++)
                {
                    this.writer.Write(this.tabString);
                }
                this.tabsPending = false;
            }
        }

        protected string PopEndTag()
        {
            if (this._endTagCount <= 0)
            {
                throw new InvalidOperationException("A PopEndTag was called without a corresponding PushEndTag.");
            }
            this._endTagCount--;
            this.TagKey = this._endTags[this._endTagCount].tagKey;
            return this._endTags[this._endTagCount].endTagText;
        }

        protected void PushEndTag(string endTag)
        {
            if (this._endTags == null)
            {
                this._endTags = new HtmlTextWriter.TagStackEntry[16];
            }
            else if (this._endTagCount >= (int)this._endTags.Length)
            {
                HtmlTextWriter.TagStackEntry[] tagStackEntryArray = new HtmlTextWriter.TagStackEntry[(int)this._endTags.Length * 2];
                Array.Copy(this._endTags, tagStackEntryArray, (int)this._endTags.Length);
                this._endTags = tagStackEntryArray;
            }
            this._endTags[this._endTagCount].tagKey = this._tagKey;
            this._endTags[this._endTagCount].endTagText = endTag;
            this._endTagCount++;
        }

        protected static void RegisterAttribute(string name, HtmlTextWriterAttribute key)
        {
            HtmlTextWriter.RegisterAttribute(name, key, false);
        }

        private static void RegisterAttribute(string name, HtmlTextWriterAttribute key, bool encode)
        {
            HtmlTextWriter.RegisterAttribute(name, key, encode, false);
        }

        private static void RegisterAttribute(string name, HtmlTextWriterAttribute key, bool encode, bool isUrl)
        {
            string lower = name.ToLowerInvariant();
            HtmlTextWriter._attrKeyLookupTable.Add(lower, key);
            if ((int)key < (int)HtmlTextWriter._attrNameLookupArray.Length)
            {
                HtmlTextWriter._attrNameLookupArray[(int)key] = new HtmlTextWriter.AttributeInformation(name, encode, isUrl);
            }
        }

        protected static void RegisterStyle(string name, HtmlTextWriterStyle key)
        {
            CssTextWriter.RegisterAttribute(name, key);
        }

        protected static void RegisterTag(string name, HtmlTextWriterTag key)
        {
            HtmlTextWriter.RegisterTag(name, key, HtmlTextWriter.TagType.Other);
        }

        private static void RegisterTag(string name, HtmlTextWriterTag key, HtmlTextWriter.TagType type)
        {
            string lower = name.ToLowerInvariant();
            HtmlTextWriter._tagKeyLookupTable.Add(lower, key);
            string str = null;
            if (type != HtmlTextWriter.TagType.NonClosing && key != HtmlTextWriterTag.Unknown)
            {
                char chr = '>';
                str = string.Concat("</", lower, chr.ToString());
            }
            if ((int)key < (int)HtmlTextWriter._tagNameLookupArray.Length)
            {
                HtmlTextWriter._tagNameLookupArray[(int)key] = new HtmlTextWriter.TagInformation(name, type, str);
            }
        }

        protected virtual string RenderAfterContent()
        {
            return null;
        }

        protected virtual string RenderAfterTag()
        {
            return null;
        }

        protected virtual string RenderBeforeContent()
        {
            return null;
        }

        protected virtual string RenderBeforeTag()
        {
            return null;
        }

        public virtual void RenderBeginTag(string tagName)
        {
            this.TagName = tagName;
            this.RenderBeginTag(this._tagKey);
        }

        public virtual void RenderBeginTag(HtmlTextWriterTag tagKey)
        {
            string str;
            this.TagKey = tagKey;
            bool flag = true;
            if (this._isDescendant)
            {
                flag = this.OnTagRender(this._tagName, this._tagKey);
                this.FilterAttributes();
                string str1 = this.RenderBeforeTag();
                if (str1 != null)
                {
                    if (this.tabsPending)
                    {
                        this.OutputTabs();
                    }
                    this.writer.Write(str1);
                }
            }
            HtmlTextWriter.TagInformation tagInformation = HtmlTextWriter._tagNameLookupArray[this._tagIndex];
            HtmlTextWriter.TagType tagType = tagInformation.tagType;
            bool flag1 = (!flag ? false : tagType != HtmlTextWriter.TagType.NonClosing);
            if (flag1)
            {
                str = tagInformation.closingTag;
            }
            else
            {
                str = null;
            }
            string str2 = str;
            if (flag)
            {
                if (this.tabsPending)
                {
                    this.OutputTabs();
                }
                this.writer.Write('<');
                this.writer.Write(this._tagName);
                string str3 = null;
                for (int i = 0; i < this._attrCount; i++)
                {
                    HtmlTextWriter.RenderAttribute renderAttribute = this._attrList[i];
                    if (renderAttribute.key != HtmlTextWriterAttribute.Style)
                    {
                        this.writer.Write(' ');
                        this.writer.Write(renderAttribute.name);
                        if (renderAttribute.@value != null)
                        {
                            this.writer.Write("=\"");
                            string str4 = renderAttribute.@value;
                            if (renderAttribute.isUrl && (renderAttribute.key != HtmlTextWriterAttribute.Href || !str4.StartsWith("javascript:", StringComparison.Ordinal)))
                            {
                                str4 = this.EncodeUrl(str4);
                            }
                            if (!renderAttribute.encode)
                            {
                                this.writer.Write(str4);
                            }
                            else
                            {
                                this.WriteHtmlAttributeEncode(str4);
                            }
                            this.writer.Write('\"');
                        }
                    }
                    else
                    {
                        str3 = renderAttribute.@value;
                    }
                }
                if (this._styleCount > 0 || str3 != null)
                {
                    this.writer.Write(' ');
                    this.writer.Write("style");
                    this.writer.Write("=\"");
                    CssTextWriter.WriteAttributes(this.writer, this._styleList, this._styleCount);
                    if (str3 != null)
                    {
                        this.writer.Write(str3);
                    }
                    this.writer.Write('\"');
                }
                if (tagType != HtmlTextWriter.TagType.NonClosing)
                {
                    this.writer.Write('>');
                }
                else
                {
                    this.writer.Write(" />");
                }
            }
            string str5 = this.RenderBeforeContent();
            if (str5 != null)
            {
                if (this.tabsPending)
                {
                    this.OutputTabs();
                }
                this.writer.Write(str5);
            }
            if (flag1)
            {
                if (tagType != HtmlTextWriter.TagType.Inline)
                {
                    this.WriteLine();
                    this.Indent = this.Indent + 1;
                }
                else
                {
                    this._inlineCount++;
                }
                if (str2 == null)
                {
                    char chr = '>';
                    str2 = string.Concat("</", this._tagName, chr.ToString());
                }
            }
            if (this._isDescendant)
            {
                string str6 = this.RenderAfterTag();
                if (str6 != null)
                {
                    str2 = (str2 == null ? str6 : string.Concat(str6, str2));
                }
                string str7 = this.RenderAfterContent();
                if (str7 != null)
                {
                    str2 = (str2 == null ? str7 : string.Concat(str7, str2));
                }
            }
            this.PushEndTag(str2);
            this._attrCount = 0;
            this._styleCount = 0;
        }

        public virtual void RenderEndTag()
        {
            string str = this.PopEndTag();
            if (str != null)
            {
                if (HtmlTextWriter._tagNameLookupArray[this._tagIndex].tagType == HtmlTextWriter.TagType.Inline)
                {
                    this._inlineCount--;
                    this.Write(str);
                    return;
                }
                this.WriteLine();
                this.Indent = this.Indent - 1;
                this.Write(str);
            }
        }

        public override void Write(string s)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(s);
        }

        public override void Write(bool value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(char value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(char[] buffer)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(buffer);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(buffer, index, count);
        }

        public override void Write(double value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(float value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(int value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(long value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(object value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(value);
        }

        public override void Write(string format, object arg0)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(format, arg0, arg1);
        }

        public override void Write(string format, params object[] arg)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write(format, arg);
        }

        public virtual void WriteAttribute(string name, string value)
        {
            this.WriteAttribute(name, value, false);
        }

        public virtual void WriteAttribute(string name, string value, bool fEncode)
        {
            this.writer.Write(' ');
            this.writer.Write(name);
            if (value != null)
            {
                this.writer.Write("=\"");
                if (!fEncode)
                {
                    this.writer.Write(value);
                }
                else
                {
                    this.WriteHtmlAttributeEncode(value);
                }
                this.writer.Write('\"');
            }
        }

        public virtual void WriteBeginTag(string tagName)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write('<');
            this.writer.Write(tagName);
        }

        public virtual void WriteBreak()
        {
            this.Write("<br />");
        }

        public virtual void WriteEndTag(string tagName)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write('<');
            this.writer.Write('/');
            this.writer.Write(tagName);
            this.writer.Write('>');
        }

        public virtual void WriteFullBeginTag(string tagName)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.Write('<');
            this.writer.Write(tagName);
            this.writer.Write('>');
        }

        internal void WriteHtmlAttributeEncode(string s)
        {
            HtmlEncoder.Default.Encode(writer, s);
        }

        public override void WriteLine(string s)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(s);
            this.tabsPending = true;
        }

        public override void WriteLine()
        {
            this.writer.WriteLine();
            this.tabsPending = true;
        }

        public override void WriteLine(bool value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(char value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(char[] buffer)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(buffer);
            this.tabsPending = true;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(buffer, index, count);
            this.tabsPending = true;
        }

        public override void WriteLine(double value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(float value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(int value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(long value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(object value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public override void WriteLine(string format, object arg0)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(format, arg0);
            this.tabsPending = true;
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(format, arg0, arg1);
            this.tabsPending = true;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(format, arg);
            this.tabsPending = true;
        }

        public override void WriteLine(uint value)
        {
            if (this.tabsPending)
            {
                this.OutputTabs();
            }
            this.writer.WriteLine(value);
            this.tabsPending = true;
        }

        public void WriteLineNoTabs(string s)
        {
            this.writer.WriteLine(s);
            this.tabsPending = true;
        }

        internal void WriteObsoleteBreak()
        {
            this.Write("<br>");
        }

        public virtual void WriteStyleAttribute(string name, string value)
        {
            this.WriteStyleAttribute(name, value, false);
        }

        public virtual void WriteStyleAttribute(string name, string value, bool fEncode)
        {
            this.writer.Write(name);
            this.writer.Write(':');
            if (!fEncode)
            {
                this.writer.Write(value);
            }
            else
            {
                this.WriteHtmlAttributeEncode(value);
            }
            this.writer.Write(';');
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

        internal class Layout
        {
            private bool _wrap;

            private HorizontalAlign _align;

            public HorizontalAlign Align
            {
                get => this._align;
                set => this._align = value;
            }

            public bool Wrap
            {
                get => this._wrap;
                set => this._wrap = value;
            }

            public Layout(HorizontalAlign alignment, bool wrapping)
            {
                this.Align = alignment;
                this.Wrap = wrapping;
            }
        }

        private struct RenderAttribute
        {
            public string name;

            public string @value;

            public HtmlTextWriterAttribute key;

            public bool encode;

            public bool isUrl;
        }

        private struct TagInformation
        {
            public string name;

            public HtmlTextWriter.TagType tagType;

            public string closingTag;

            public TagInformation(string name, HtmlTextWriter.TagType tagType, string closingTag)
            {
                this.name = name;
                this.tagType = tagType;
                this.closingTag = closingTag;
            }
        }

        private struct TagStackEntry
        {
            public HtmlTextWriterTag tagKey;

            public string endTagText;
        }

        private enum TagType
        {
            Inline,
            NonClosing,
            Other
        }

        private static class UrlPath
        {
            internal static bool IsUncSharePath(string path)
            {
                if (path.Length > 2 && IsDirectorySeparatorChar(path[0]) && IsDirectorySeparatorChar(path[1]))
                {
                    return true;
                }
                return false;
            }

            private static bool IsDirectorySeparatorChar(char ch)
            {
                if (ch == '\\')
                {
                    return true;
                }
                return ch == '/';
            }
        }

        public void AddAttributes(IDictionary<string, object> attributes)
        {
            foreach (var attribute in attributes)
            {
                AddAttribute(attribute.Key, Convert.ToString(attribute.Value));
            }
        }

        public void AddAttributeIfHave(HtmlTextWriterAttribute key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                AddAttribute(key, value);
            }
        }
    }
}