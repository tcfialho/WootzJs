﻿using System;
using System.Linq;
using System.Runtime.WootzJs;

namespace WootzJs.Web
{
    public static class DocumentExtensions
    {
        public static Element GetElementByTagName(this Document document, string tagName)
        {
            var result = document.GetElementsByTagName(tagName);
            return result.Length > 0 ? result[0] : null;
        }

        public static Event CreateCustomEvent(this Document document, string eventType, object args)
        {
            try
            {
                return Jsni.@new(Jsni.reference("CustomEvent"), eventType, args.As<JsObject>()).As<Event>();
            } 
            catch (Exception e)
            {
                var evt = document.CreateEvent("CustomEvent");
                evt.As<JsObject>().member("initCustomEvent").invoke(eventType, false, true, args.As<JsObject>());
                return evt;
            }
        }
    }
}