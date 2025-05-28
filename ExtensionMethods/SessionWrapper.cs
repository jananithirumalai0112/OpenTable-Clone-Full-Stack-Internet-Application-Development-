using Microsoft.AspNetCore.Http;
using System.Text.Json;
using OpenTableApp.Models;
using System.Collections.Generic;

namespace OpenTableApp.ExtensionMethods
{
    public static class SessionWrapper
    {
        // Generic Session: Set/Get Object
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        // Cookie: Set/Get Object
        public static void SetObject<T>(this IResponseCookies cookies, string key, T value, int days = 7)
        {
            var options = new CookieOptions { Expires = DateTimeOffset.Now.AddDays(days) };
            cookies.Append(key, JsonSerializer.Serialize(value), options);
        }

        public static T? GetObject<T>(this IRequestCookieCollection cookies, string key)
        {
            var value = cookies[key];
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        // Custom helpers for Cart
        public static void SetCartItems(this ISession session, List<CartItem> cartItems)
        {
            session.SetObject("CartItems", cartItems);
        }

        public static List<CartItem> GetCartItems(this ISession session)
        {
            return session.GetObject<List<CartItem>>("CartItems") ?? new List<CartItem>();
        }

        public static void SetCartCount(this ISession session, int count)
        {
            session.SetInt32("CartCount", count);
        }

        public static int GetCartCount(this ISession session)
        {
            return session.GetInt32("CartCount") ?? 0;
        }
    }
}
