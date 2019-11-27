using DependencyInjectionSample;
using DotVue;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Pages
{
    public class DI : ViewModel
    {
        public Guid Transient { get; set; }
        public Guid Scoped { get; set; }
        public Guid Singleton { get; set; }
        public Guid SingletonInstance { get; set; }

        public Guid Transient2 { get; set; }
        public Guid Scoped2 { get; set; }
        public Guid Singleton2 { get; set; }
        public Guid SingletonInstance2 { get; set; }

        public Guid Transient3 { get; set; }
        public Guid Scoped3 { get; set; }
        public Guid Singleton3 { get; set; }
        public Guid SingletonInstance3 { get; set; }

        private readonly IServiceProvider _serviceProvider;
        private readonly OperationService _operationService;
        private readonly IOperationTransient _transientOperation;
        private readonly IOperationScoped _scopedOperation;
        private readonly IOperationSingleton _singletonOperation;
        private readonly IOperationSingletonInstance _singletonInstanceOperation;

        public DI(IServiceProvider serviceProvider,
            OperationService operationService,
            IOperationTransient transientOperation,
            IOperationScoped scopedOperation,
            IOperationSingleton singletonOperation,
            IOperationSingletonInstance singletonInstanceOperation)
        {
            _serviceProvider = serviceProvider;
            _operationService = operationService;
            _transientOperation = transientOperation;
            _scopedOperation = scopedOperation;
            _singletonOperation = singletonOperation;
            _singletonInstanceOperation = singletonInstanceOperation;
        }

        protected override void OnCreated()
        {
            this.Transient = _transientOperation.OperationId;
            this.Scoped = _scopedOperation.OperationId;
            this.Singleton = _singletonOperation.OperationId;
            this.SingletonInstance = _singletonInstanceOperation.OperationId;

            this.Transient2 = _operationService.TransientOperation.OperationId;
            this.Scoped2 = _operationService.ScopedOperation.OperationId;
            this.Singleton2 = _operationService.SingletonOperation.OperationId;
            this.SingletonInstance2 = _operationService.SingletonInstanceOperation.OperationId;

            this.Transient3 = _serviceProvider.GetService<IOperationTransient>().OperationId;
            this.Scoped3 = _serviceProvider.GetService<IOperationScoped>().OperationId;
            this.Singleton3 = _serviceProvider.GetService<IOperationSingleton>().OperationId;
            this.SingletonInstance3 = _serviceProvider.GetService<IOperationSingletonInstance>().OperationId;
        }
    }
}