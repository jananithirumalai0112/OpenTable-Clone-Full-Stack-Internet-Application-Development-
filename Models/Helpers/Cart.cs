using System.Text.Json;
using Microsoft.AspNetCore.Http;
using OpenTableApp.Models;
using System.Collections.Generic;
using System.Linq;
using OpenTableApp.ExtensionMethods;

namespace OpenTableApp.Helpers
{
    public class Cart
    {
        private const string CartKey = "CartItems";
        private const string CountKey = "CartCount";

        private List<CartItem> items;
        private ISession session;
        private IRequestCookieCollection requestCookies;
        private IResponseCookies responseCookies;

        public Cart(HttpContext context)
        {
            session = context.Session;
            requestCookies = context.Request.Cookies;
            responseCookies = context.Response.Cookies;

            // Always start from session if available
            items = session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
        }

        public List<CartItem> List => items;
        public int Count => items.Count;

        public void Add(CartItem item)
        {
            items.Add(item);
            Save();
        }

        public void Remove(int id)
        {
            var item = items.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                items.Remove(item);
                Save();
            }
        }

        public void Clear()
        {
            items.Clear();
            Save();
        }

        public void Save()
        {
            if (items.Count == 0)
            {
                session.Remove(CartKey);
                session.Remove(CountKey);
                responseCookies.Delete(CartKey);
                responseCookies.Delete(CountKey);
            }
            else
            {
                session.SetObject(CartKey, items);
                session.SetInt32(CountKey, items.Count);
                responseCookies.SetObject(CartKey, items, 7);
                responseCookies.Append(CountKey, items.Count.ToString(), new CookieOptions { Expires = DateTimeOffset.Now.AddDays(7) });
            }
        }

        public void LoadFromCookieIfNeeded()
        {
            var cookieItems = requestCookies.GetObject<List<CartItem>>(CartKey);

            // ✅ Overwrite if session is empty or cookie has more
            if ((items == null || items.Count == 0) && cookieItems != null && cookieItems.Count > 0)
            {
                items = cookieItems!;
                Save(); // sync back to session and cookie
            }
        }
    }
}
