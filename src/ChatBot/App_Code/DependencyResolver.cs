using System;
using System.Collections.Generic;
using System.Web;
using ChatBot.Data;
using ChatBot.Data.Models;
using ChatBot.Data.Repositories;
using ChatBot.Services.Auth;
using ChatBot.Services.Logging;
using ChatBot.Services.OpenAI;
using ChatBot.Services.Security;

namespace ChatBot
{
    /// <summary>
    /// Simple dependency injection container
    /// </summary>
    public static class DependencyResolver
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes the dependency resolver and registers services
        /// </summary>
        public static void Initialize()
        {
            // Ensure database exists
            DbManager.EnsureDatabaseExists();

            // Register services
            RegisterSingleton<ISecurityService>(new SecurityService());
            
            // Repositories
            RegisterSingleton<IRepository<ConfigModel>>(new ConfigRepository(Resolve<ISecurityService>()));
            RegisterSingleton<IRepository<ErrorModel>>(new ErrorRepository());
            RegisterSingleton<IRepository<ChatModel>>(new ChatRepository());
            RegisterSingleton<IRepository<UserModel>>(new UserRepository(Resolve<ISecurityService>()));
            
            // Services
            RegisterSingleton<IErrorLogger>(new ErrorLogger(Resolve<IRepository<ErrorModel>>()));
            RegisterSingleton<IOpenAIService>(new OpenAIService(Resolve<IRepository<ConfigModel>>()));
            RegisterSingleton<IAuthManager>(new AuthManager(
                Resolve<IRepository<UserModel>>(),
                Resolve<IRepository<ConfigModel>>(),
                Resolve<ISecurityService>()));
        }

        /// <summary>
        /// Registers a singleton service
        /// </summary>
        public static void RegisterSingleton<T>(T instance) where T : class
        {
            _services[typeof(T)] = instance;
        }

        /// <summary>
        /// Resolves a service
        /// </summary>
        public static T Resolve<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered");
        }
    }
}