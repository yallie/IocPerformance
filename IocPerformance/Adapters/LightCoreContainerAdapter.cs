﻿using LightCore;
using LightCore.Lifecycle;

namespace IocPerformance.Adapters
{
    public sealed class LightCoreContainerAdapter : IContainerAdapter
    {
        private IContainer container;

        public void Prepare()
        {
            var builder = new ContainerBuilder();

            builder.Register<IInterface1, Implementation1>().ControlledBy<SingletonLifecycle>();
            builder.Register<IInterface2, Implementation2>().ControlledBy<TransientLifecycle>();
            builder.Register<ICombined, Combined>().ControlledBy<TransientLifecycle>();

            this.container = builder.Build();
        }

        public T Resolve<T>() where T : class
        {
            return this.container.Resolve<T>();
        }

        public void Dispose()
        {
            // Allow the container and everything it references to be disposed.
            this.container = null;
        }
    }
}