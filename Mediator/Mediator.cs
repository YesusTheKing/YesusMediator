using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YesusMediator.Handlers;
using YesusMediator.Requests;

namespace YesusMediator.Mediators
{
    public class Mediator : IMediator
    {
        private readonly Func<Type,object> _serviceProvider;
        private readonly IDictionary<Type, Type> _classes;
        public Mediator(IDictionary<Type, Type> classes, Func<Type,object> serviceProvider)
        {
            _classes = classes;
            _serviceProvider = serviceProvider;
        }
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            var requestType = request.GetType();
            if(!_classes.ContainsKey(requestType))
            {
                throw new Exception($"Handler is not implemented error {requestType.Name}");
            }

            var requestHandlerType = _classes[requestType];
            var _handler = _serviceProvider(requestHandlerType);
            

            //return await ((IHandler<IRequest<TResponse>,TResponse>) _handler).HandleAsync(request);

            return await (Task<TResponse>)_handler.GetType().GetMethod("HandleAsync")!.Invoke(_handler, new [] { request })!;

        }
    }
}
