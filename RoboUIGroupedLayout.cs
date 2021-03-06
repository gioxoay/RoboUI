﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace RoboUI
{
    public class RoboUIGroupedLayout<TModel>
    {
        private readonly ICollection<string> properties;
        private readonly string title;

        internal RoboUIGroupedLayout(string title)
        {
            properties = new List<string>();
            this.title = title;
            Column = 1;
        }

        public int Column { get; set; }

        public string CssClass { get; set; }

        public bool EnableGrid { get; set; }

        public bool EnableScrollbar { get; set; }

        public string FormGroupWrapperEndHtml { get; set; }

        public string FormGroupWrapperStartHtml { get; set; }

        public ICollection<string> Properties => properties;

        public string Title => title;

        public RoboUIGroupedLayout<TModel> Add(string property)
        {
            properties.Add(property);
            return this;
        }

        public RoboUIGroupedLayout<TModel> Add<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            properties.Add(ExpressionHelper.GetExpressionText(expression));
            return this;
        }
    }
}
