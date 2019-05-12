using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBO.DataTransport.DBOAuth.DataModel.Pluralazing
{
    public class Pluralizer : IPluralizer
    {
        public Pluralizer()
        {
            foreach (var item in _replacementItems)
            {
                _replacementBackItems[item.Value] = item.Key;
            }
        }

        private static readonly Dictionary<string, string> _replacementBackItems = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> _replacementItems = new Dictionary<string, string>
        {
            {
                "AspNetRoleClaims","AspNetRoleClaim"
            },
            {
                "AspNetRoles","AspNetRole"
            },
            {
                "AspNetUserClaims","AspNetUserClaim"
            },
            {
                "AspNetUserLogins","AspNetUserLogin"
            },
            {
                "AspNetUserRoles","AspNetUserRole"
            },
            {
                "AspNetUsers","AspNetUser"
            },
            {
                "AspNetUserTokens","AspNetUserToken"
            },
        };

        public string Pluralize(string identifier) => _replacementBackItems.TryGetValue(identifier, out var cName) ? cName : identifier;

        public string Singularize(string identifier) => _replacementItems.TryGetValue(identifier, out var cName) ? cName : identifier;
    }
}
