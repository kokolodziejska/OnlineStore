using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace OnlineStore.Web.Binders
{
    public class CustomDecimalModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder binder = new CustomDecimalModelBinder();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(decimal) ||
                context.Metadata.ModelType == typeof(decimal?))
            {
                return binder;
            }

            return null;
        }
    }
}
