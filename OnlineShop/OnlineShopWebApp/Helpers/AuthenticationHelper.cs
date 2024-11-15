using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers
{
    public class AuthenticationHelper(IHttpContextAccessor httpContextAccessor)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Execute async function depending on user authenticated status
        /// </summary>        
        /// <returns>T functions result</returns>
        /// <param name="authenticatedFunction">Function for authentication case(async)</param>
        /// <param name="unauthenticatedFunction">Function for unauthenticated case(async)</param>
        public async Task<T> ExecuteBasedOnAuthenticationAsync<T>(Func<Task<T>> authenticatedFunction, Func<Task<T>> unauthenticatedFunction)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                return await authenticatedFunction();
            }

            return await unauthenticatedFunction();
        }

        /// <summary>
        /// Execute async action depending on user authenticated status
        /// </summary>        
        /// <returns>Task</returns>
        /// <param name="authenticatedAction">Action for authentication case (async)</param>
        /// <param name="unauthenticatedAction">Action for unauthenticated case(async)</param>
        public async Task ExecuteBasedOnAuthenticationAsync(Func<Task> authenticatedAction, Func<Task> unauthenticatedAction)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

            if (isAuthenticated)
            {
                await authenticatedAction();
            }

            await unauthenticatedAction();
        }

        /// <summary>
        /// Execute async action for authenticated and sync action for authenticated
        /// </summary>        
        /// <returns>Task</returns>
        /// <param name="authenticatedAction">Action for authentication case (async)</param>
        /// <param name="unauthenticatedAction">Action for unauthenticated case(sync)</param>
        public async Task ExecuteBasedOnAuthenticationAsync(Func<Task> authenticatedAction, Action unauthenticatedAction)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                await authenticatedAction();
            }

            unauthenticatedAction();
        }
    }
}
