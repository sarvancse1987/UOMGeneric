using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.DataAccess.GenericRepository.Helper
{
    public class ContextHelper : IContextHelper
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;
        public ContextHelper(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Selected Entities can be skipped by validating Tenant/Base properties
        /// In such case, context will be bypassed
        /// </summary>
        /// <returns></returns>
        public bool IsSkippingBaseProperties(string entityName)
        {
            //get entity name
            List<string> nameList = entityName.Split('.').ToList<string>();
            entityName = nameList[nameList.Count - 1];
            if (string.IsNullOrEmpty(entityName))
                return false;

            //get entity list to skip validation
            string skipList = _configuration.GetSection("contextSkipOn").Value;

            //if no list found to be skipped, apply filter on the given entity
            if (String.IsNullOrEmpty(skipList))
                return false;

            //if list of items found to be skipped, check current entity is in the list
            //if entity matches, filter will not be applying on the given entity
            List<string> entities = skipList.Split(',').ToList<string>();
            var filteredEntity = entities.FirstOrDefault(entityToCheck => entityToCheck.Contains(entityName.Trim()));

            if (filteredEntity != null)
                return true;

            return false;
        }

        public JObject GetUser()
        {
            try
            {
                var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                accessToken = accessToken != null ? accessToken.Split(' ')[1] : "";
                if (!String.IsNullOrEmpty(accessToken))
                {
                    return new JObject();
                }

                //throw new ApiException("Unauthorized", 401);
            }
            catch (Exception ex)
            {
                //throw new ApiException(ex.StackTrace);
                throw;
            }
            return new JObject();
        }

        public int GetUserId()
        {
            try
            {
                var user = GetUser();
                return Convert.ToInt32(user["userId"]);
            }
            catch (Exception ex)
            {
                //throw new ApiException(ex.StackTrace);
                throw;
            }
        }

        public DateTime GetUTCDateTime()
        {
            return DateTime.UtcNow;
        }
        public IList<string> GetUserRoles()
        {
            var user = this.GetUser();
            var roles = from c in user["userProfiles"].SelectMany(i => i["roles"]).Values<string>()
                        group c by c
                        into g
                        orderby g.Count() descending
                        select g.Key;// new { Role = g.Key};
            var rs = roles.ToList();
            return rs;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ContextHelper() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
